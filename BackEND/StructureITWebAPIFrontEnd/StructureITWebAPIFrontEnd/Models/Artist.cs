using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace StructureITWebAPIFrontEnd.Models
{
    public class Artists
    {
       [Key]
        public string ArtistName { get; set; }
        public string Uniqueidentifier { get; set; }
        public string Country { get; set; }
        public string Aliases { get; set; }

    }

    public class PagedResult<T>
    {
        public class PagingInfo
        {
            public int page { get; set; }

            public int pageSize { get; set; }

            public long numberOfSearchResults { get; set; }

            public long numberOfPages { get; set; }

        }
        public List<T> Data { get; private set; }

        public PagingInfo Paging { get; private set; }

        public PagedResult(IEnumerable<T> items, int pageNo, int pageSize, long totalRecordCount)
        {
            Data = new List<T>(items);
            Paging = new PagingInfo
            {
                numberOfSearchResults = totalRecordCount,
                page = pageNo,
                pageSize = pageSize,
                numberOfPages = totalRecordCount > 0
                    ? (int)Math.Ceiling(totalRecordCount / (double)pageSize)
                    : 0,

            };
        }


    }
    [XmlRoot("metadata", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class ReleaseMetadata
    {
        /// <summary>
        /// Gets or sets the release-list collection.
        /// </summary>
        [XmlElement("release-list")]
        public ReleaseList Collection { get; set; }
    }
    [Serializable()]
    [XmlRoot("release-list", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class ReleaseList
    {
        /// <summary>
        /// Gets or sets the list of releases.
        /// </summary>
        [XmlElement("release")]
        public List<releases> Items { get; set; }
    }
    [Serializable()]
    public class otherArtist
    {
        public string id { get; set; }
        public string name { get; set; }

    }

    [XmlRoot("label-info")]
    public class LabelInfo
    {

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [XmlElement("label")]
        public Label Label { get; set; }
    }
    [Serializable()]
    [XmlRoot("metadata", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class releases
    {
        [XmlAttribute("id")]
        public string releaseId { get; set; }
        [XmlElement("title")]
        public string title { get; set; }
        [XmlElement("status")]
        public string status { get; set; }

        [XmlArray("label-info-list")]
        [XmlArrayItem("label-info")]
        public List<LabelInfo> Labels { get; set; }

        [XmlElement("medium-list")]
        public MediumList numberOfTracks { get; set; }

        [XmlArray("artist-credit")]
        [XmlArrayItem("name-credit")]
        public List<NameCredit> otherArtists { get; set; }
    }
    [XmlRoot("medium-list", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class MediumList
    {
        /// <summary>
        /// Gets or sets the medium track count.
        /// </summary>
        /// <remarks>
        /// Only available in the result of a release search (???).
        /// </remarks>
        [XmlElement(ElementName = "track-count")]
        public int TrackCount { get; set; }


    }
    [XmlRoot("name-credit")]
    public class NameCredit
    {
        /// <summary>
        /// Gets or sets the artist.
        /// </summary>
        [XmlElement("artist")]
        public OtherArtist Artist { get; set; }
    }
    [XmlRoot("artist", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class OtherArtist
    {

        /// <summary>
        /// Gets or sets the MusicBrainz id.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

    }
    public class Label
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
