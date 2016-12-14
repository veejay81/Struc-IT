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
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace StructureITWebAPIFrontEnd.Controllers
{
    public class AlbumController : ApiController
    {
        private StructureITDBContext db = new StructureITDBContext();

        // GET: /api/Artist/{Artistid}/releases
        [ResponseType(typeof(Artists))]
        public IHttpActionResult GetAlbum(string release_id)
        {
            // var TotalRec = (from m in db.Artists where m.Country.ToLower().Contains(artist_id.ToLower()) select m);
            string twitterRequestTokenUrl = "http://musicbrainz.org/ws/2/release/?query=primarytype:album%20reid:9cc88413-d456-4b96-a0c1-09fa6cc2cf88";
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
                    return Ok(json);
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

    }
}
