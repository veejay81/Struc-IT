using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StructureITWebAPIFrontEnd.Models;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace StructureITWebAPIFrontEnd.Controllers
{
    public class ArtistController : ApiController
    {
        //Initialize a DB object from DB context 
        private StructureITDBContext db = new StructureITDBContext();


        /// <summary>
        /// REST method that returns all the artist details based on the provided input from SQL DB (Connection string uses Entity framework)
        /// </summary>
        /// <param name="ArtistName">Parameted is passed as an ArtistName</param>
        /// <returns></returns>
        [ResponseType(typeof(Artists))]
        public IHttpActionResult GetArtists(string ArtistName)
        {
            var TotalRec = (from m in db.Artists where m.Aliases.ToLower().Contains(ArtistName.ToLower()) || m.ArtistName.ToLower().Contains(ArtistName.ToLower()) select m);
           

            return Ok(TotalRec);
        }

        /// <summary>
        /// REST Method returns the details of all the artist based on the parameters provided (Name,Pagenumber,PageSize) from SQL DB (Connection string uses Entity framework)
        /// </summary>
        /// <param name="ArtistName">Parameted is passed as an ArtistName</param>
        /// <param name="Pagenumber">details of artist fom the page number</param>
        /// <param name="PageSize">details per page</param>
        /// <returns></returns>
        [ResponseType(typeof(Artists))]
        public IHttpActionResult GetArtist(string ArtistName, int Pagenumber, int PageSize)
        {
            int skip = (Convert.ToInt32(Pagenumber) - 1) * PageSize;
           var TotalRec = (from m in db.Artists where m.Aliases.ToLower().Contains(ArtistName.ToLower()) || m.ArtistName.ToLower().Contains(ArtistName.ToLower()) select m);
            
            var matches = (TotalRec).OrderBy(c => c.Aliases).Skip(skip).Take(PageSize).ToList();
            if (matches == null)
            {
                return null;
            }
            return Ok(new PagedResult<Artists>(matches, Convert.ToInt16(Pagenumber), Convert.ToInt16(PageSize), TotalRec.Count()));
        }


        /// <summary>
        /// Returns all the details based on the artist id from musicBraizAPI (musicbrainz.org/ws/2/release/)
        /// </summary>
        /// <param name="artist_id">the artist id is provided as a paramater</param>
        /// <returns></returns>
        [ResponseType(typeof(Artists))]
        public XElement GetArtist(string artist_id)
        {
            var TotalRec = (from m in db.Artists where m.Uniqueidentifier.ToLower().Contains(artist_id.ToLower()) select m);
            string twitterRequestTokenUrl = "http://musicbrainz.org/ws/2/release/?query=arid:" + artist_id;
            try
            {
                var request = WebRequest.Create(twitterRequestTokenUrl) as HttpWebRequest;
                if (request == null)
                    return new XElement("");

                request.UserAgent = "MusicBrainze.API/2.0";
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var xml = XDocument.Load(stream);
                    var xsn = new XmlSerializerNamespaces();
                    xsn.Add("ext", "http://musicbrainz.org/ns/ext#-2.0");
                    string XMLDocument = CreateXml(TotalRec);
                    if(XMLDocument!=null)
                    xml.Root.Elements().FirstOrDefault().Add(XDocument.Parse(XMLDocument).Root.Elements());
                    return xml.Root != null ? xml.Root.Elements().FirstOrDefault() : new XElement("");
                }
            }
            catch (Exception)
            {
                return new XElement("");
            }

        }

        /// <summary>
        /// Get the details of all the album based on the provided release  from musicBraizAPI (musicbrainz.org/ws/2/release/)
        /// </summary>
        /// <param name="release_id">the release id will be provided as parameter</param>
        /// <returns></returns>
        public IHttpActionResult GetAlbum(string release_id)
        {
            string twitterRequestTokenUrl = "http://musicbrainz.org/ws/2/release/?query=primarytype:album%20limit=10%20offset=1%20reid:" + release_id;
          
            try
            {
                var request = WebRequest.Create(twitterRequestTokenUrl) as HttpWebRequest;
                if (request == null)
                    return null;

                request.UserAgent = "MusicBrainze.API/2.0";
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var xml = XDocument.Load(stream);

                    //var xsn = new XmlSerializerNamespaces();
                    //xsn.Add("ext", "http://musicbrainz.org/ns/ext#-2.0");
                    ReleaseMetadata rel = null;
                    using (Stream s = GenerateStreamFromString(xml.ToString()))
                    {


                        XmlSerializer serializer = new XmlSerializer(typeof(ReleaseMetadata));

                        StreamReader reader = new StreamReader(s);
                        rel = (ReleaseMetadata)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    string json = JsonConvert.SerializeObject(rel);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
                    return Ok(doc);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string CreateXml<T>(IQueryable<T> thisQueryable)
        {
            var thisList = thisQueryable.ToList();
            if (thisList.Count > 0)
            {
                var xmlResult = "";
                using (var stringWriter = new StringWriter())
                {
                    using (var xmlWriter = new XmlTextWriter(stringWriter))
                    {
                        var serializer = new XmlSerializer(typeof(List<T>));
                        serializer.Serialize(xmlWriter, thisList);
                    }
                    xmlResult = stringWriter.ToString();
                }
                return xmlResult;
            }
            return null;
        }

    }
}
