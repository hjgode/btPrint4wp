using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace btPrint4wp
{
    [XmlRoot("files")]
    public class demofiles
    {
        [XmlElement("fileentry")]
        public fileentry[] fileentries;
        
        public class fileentry
        {
            [XmlElement("shortname")]
            public string shortname;
            [XmlElement("description")]
            public string filedescription;
            [XmlElement("help")]
            public string filehelp;
            [XmlIgnore]
            public string fileimage;
            [XmlIgnore]
            string _filename;
            [XmlElement("filename")]
            public string filename
            {
                get { return _filename; }
                set {
                    _filename = value;
                    filetype = _filename.Substring(0, 4);
                    if (filetype.StartsWith("fp"))
                        fileimage = "images/fp.gif";
                    else if (filetype.StartsWith("csim"))
                        fileimage = "images/csim.gif";
                    else if (filetype.StartsWith("escp"))
                        fileimage = "images/escp.gif";
                    else if (filetype.StartsWith("ipl"))
                        fileimage = "images/ipl.gif";
                    else if (filetype.StartsWith("xsim"))
                        fileimage = "images/xsim.gif";
                    else
                        fileimage = "images/pb42.gif";
                }
            }
            public string filetype;
        }

        /// <summary>
        /// Deserializes the XML data contained by the specified System.String
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized</typeparam>
        /// <param name="s">The System.String containing XML data</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static demofiles XmlDeserialize(string s)
        {
            var locker = new object();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(s));
            var reader = XmlReader.Create(ms);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(demofiles));
                lock (locker)
                {
                    var item = (demofiles)xmlSerializer.Deserialize(reader);
                    reader.Close();
                    return item;
                }
            }
            catch
            {
                return default(demofiles);
            }
            finally
            {
                reader.Close();
            }
        }
    }
}