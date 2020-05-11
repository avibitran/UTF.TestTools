using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace UTF.TestTools.Reporters
{
    [JsonObject(Id = "fileInfo")]
    [XmlType(TypeName = "fileInfo")]
    [Serializable]
    public class FileInfo
    {
        #region Fields
        private string _title = null;
        private string _content = null;
        private string _location = null;
        //private string _contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
        #endregion Fields

        #region Ctor
        public FileInfo()
        { }

        public FileInfo(string location, string title = null)
        {
            this.Location = location;
            this.Title = title;
        }
        #endregion Ctor

        #region Methods
        private string ConvertImageToBase64String(string imageFullPath)
        {
            if (!File.Exists(imageFullPath))
                throw new FileNotFoundException(string.Format("File not found {0}", imageFullPath));

            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageFullPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);

                    string imageFormat = Path.GetExtension(imageFullPath).Replace(".", "");

                    return "data:image/" + imageFormat + ";" + "base64," + base64String;
                }
            }
        }
        #endregion Methods

        #region Properties
        [JsonProperty("title", Order = 0, DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        [XmlAttribute(AttributeName = "title")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [JsonProperty("location", Order = 1, DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        [XmlAttribute(AttributeName = "location")]
        public string Location
        {
            get { return _location; }
            set
            {
                _content = ConvertImageToBase64String(value);
                _location = value;
            }
        }

        public string Content
        {
            get { return _content; }
        }
        #endregion Properties
    }
}
