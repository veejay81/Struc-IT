var app = angular.module("musicsearchApp", []);
app.controller("MusicBrainz",
    function($scope, $http) {
        var siteUrl = "http://musicbrainz.org/ws/2"; $scope.showResults = false; $scope.favourites = {}; $scope.shortListArtist = {}; $scope.favouriteMusicList = {};
        var lastFmApiUrl = "http://ws.audioscrobbler.com/2.0/?method=artist.search&api_key=36ec72d3d051ac287de9bed61fe2657e&format=json&artist=";

        //Upon click of the test the values is sent to search option
        $("#SearchTxt").keypress(function (e) { if (e.which === 13) { $("#search").click(); } });

        //Upon the click of search the details are sent to URL and later the used to fetch using Http get
        $("#search") .click(function () {
            $scope.showResults = false; if ($("#SearchTxt").val() === '') { alert("Please enter a text...."); return; }
            $scope.artists = []; var searchUrl = siteUrl + "/artist?" + $(":input").serialize(); 
                if ($("li.active a").text() !== "MusicBrainz") { searchUrl = lastFmApiUrl + encodeURIComponent($("#SearchTxt").val()); }
            // get the details of the Artist from the URL entered
                $http.get(searchUrl) .then(function(response) {
                    if (response.data.artists) {
                        $scope.artists = $scope.SaveResults(response.data.artists, true); return;
                    }
                    if (response.data.results && response.data.results.artistmatches.artist) {
                        $scope.artists = $scope.SaveResults(response.data.results.artistmatches.artist, true);
                    } },
                        function(error) { alert("Please check your internet connection..."); });
            });
        
        $scope.Releases1 = function (_aid, _index) {
            var isArtist = false; var $showElement = document.getElementById("show" + _index); var $hideElement = document.getElementById("hide" + _index);
            var $resultsElement = document.getElementById("results" + _index);
            if ($showElement.className === "hide") { $showElement.className = "show"; $hideElement.className = "hide"; $resultsElement.className = "hide"; }
            else { $showElement.className = "hide"; $hideElement.className = "show"; $resultsElement.className = "show"; }
        }

        //Get the details of the release based on the Get functionality
        $scope.Releases = function(_aid, _index) { var releaseUrl = siteUrl + "/release?artist=" + _aid + "&inc=labels+recordings&fmt=json";
            var isArtist = false; var $showElement = document.getElementById("show" + _index); var $hideElement = document.getElementById("hide" + _index);
            var $resultsElement = document.getElementById("results" + _index);
            if ($showElement.className === "hide") { $showElement.className = "show"; $hideElement.className = "hide"; $resultsElement.className = "hide"; }
            else { $showElement.className = "hide"; $hideElement.className = "show"; $resultsElement.className = "show"; }
            var filterdValue = $scope.artistFilter(_aid);
            if (filterdValue === undefined) {
                alert("No releases found");
                return;
            }
            if ($(filterdValue) && $(filterdValue)[0].releases.length === 0) { $http.get(releaseUrl) .then(function(response) {
                if (response.data.releases.length > 0 && filterdValue) {
                    $(filterdValue)[0].releases = $scope.SaveResults(response.data.releases, isArtist);
                                $resultsElement.className = "show"; } else { $resultsElement.className = "hide"; } },
                        function(error) { alert("Please check your internet connection ..."); }); } $scope.showResults = !$scope.showResults; };

        //The Artist need to be filtered 
        $scope.artistFilter = function (id) {
            var filterDetailList = jQuery.grep($scope.artists,
                    function (art) {
                        if (art.mbid === id) { return art; }
                        return undefined;
                    });
            return filterDetailList;
        };

        // This scope is going to save the results retrieved.
        $scope.SaveResults = function (items, isArtist) {
            var list = [];
            $(items).each(function(i, item) { var newItem = isArtist ? $scope.SaveArtistResults(item) : $scope.GetReleaseDet(item); list.push(newItem); });
            return list;
        };

        //get all the details from the Artists
        $scope.SaveArtistResults = function(artistSource) {
            return {
                name: artistSource.name,
                mbid: artistSource.mbid || artistSource.id,
                url: artistSource.url || '',
                image: artistSource.image || [],
                releases: [],
                show: true,
                isFavorite: false
            };
        };

        //Get the release details from the specified URL
        $scope.GetReleaseDet = function(releaseSource) {
            return {
                title: releaseSource.title,
                label: releaseSource["label-info"][0] ? releaseSource["label-info"][0].label.name : '',
                numberOfTracks: releaseSource.media[0]["track-count"],
                date: releaseSource.date && releaseSource.date.split('-') ? releaseSource.date.split('-')[0] : '',
                ReleaseID: releaseSource.id,
                isFavorite: false
            };
        };

        //Get the Short List details to make sure the Local Storage is set to Update 
        $scope.ShortListDetails = function (mbid, name) {
            $scope.shortListArtist[mbid] = name; if (localStorage) { localStorage.setItem("shortListArtist", JSON.stringify($scope.shortListArtist)); }
        }

        //Insert the details of the selected item to my Fav list
        $scope.InsertMyfav = function(mbid, name) {
            $scope.favourites[mbid] = name; $scope.UpdateLocalmemoryStrg();
        };
        $scope.InsertMyfavRelease = function (mbid, name, date, title, label, numberOfTracks) {
            var release = [];
            var key = mbid;
            var object = { key: key, date: date, title: title, numberOfTracks: numberOfTracks, label: label };
            if ($scope.favouriteMusicList[name.name] == undefined) {
                $scope.favouriteMusicList[name.name] = name; release.push(object);
                $scope.favouriteMusicList[name.name].releaseFav = release;
            }
            else if ($scope.favouriteMusicList[name.name].releaseFav.length > 0) {
                for (var k in $scope.favouriteMusicList[name.name].releaseFav) {
                    if (typeof $scope.favouriteMusicList[name.name].releaseFav[k] !== 'function') {
                        if (key == $scope.favouriteMusicList[name.name].releaseFav[k].key) {
                            return false;
                        }
                    }
                }
                $scope.favouriteMusicList[name.name].releaseFav[$scope.favouriteMusicList[name.name].releaseFav.length] = object;
            }
            else { release.push(object); $scope.favouriteMusicList[name.name].releaseFav = release; }
            $scope.UpdateLocalmemoryStrg1();
        };
        //Delete the favaorites from the list based on the icon click
        $scope.Deletefav = function (mbid, index) {
            delete $scope.favouriteMusicList[mbid];
            $scope.UpdateLocalmemoryStrg1();
        }

        //Update the local memory storage and keep the list updated.
        $scope.UpdateLocalmemoryStrg = function() {
            if (localStorage) {
                localStorage.setItem("favourites", JSON.stringify($scope.favourites));
            }
        };

        $scope.UpdateLocalmemoryStrg1 = function () {
            if (localStorage) {
                localStorage.setItem("favouriteMusicList", JSON.stringify($scope.favouriteMusicList));
            }
        };
        //Check the local memory and get the details based on the items passed
        $scope.LocalCacheMem = function (storageName) {
            if (storageName === "favourites") {
                var itemsRetrieved = localStorage.favourites;
                //Loop through each List and get the Key details
                $scope.favourites = JSON.parse(itemsRetrieved);
                for (var key in $scope.favourites) {
                    $scope.favourites[key].ObjName = key;
                }
            }

            if (storageName === "favouriteMusicList") {
                var itemsRetrieved1 = localStorage.favouriteMusicList;
                //Loop through each List and get the Key details
                $scope.favouriteMusicList = JSON.parse(itemsRetrieved1);
                for (var key in $scope.favouriteMusicList) {
                    $scope.favouriteMusicList[key].ObjName = key;
                }
            }

            if (storageName === "shortlist") {
                var shortlist = localStorage.shortListArtist;
                $scope.shortListArtist = JSON.parse(shortlist);
            }
            return {key:"NoItems", value:"No short listed Items"};
        };
        
    });