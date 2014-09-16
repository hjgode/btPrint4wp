using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using System.IO;
using Windows.Storage;

namespace btPrint4wp
{
    public class DemoFilesXMLBased:DemoFiles
    {
        StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        public demofilesXML myFiles = new demofilesXML();
        public List<demofilesXML.fileentry> myDemoFilesXML = new List<demofilesXML.fileentry>();

        public DemoFilesXMLBased()
        {
            demofiles.Clear();
            init();
        }

        async void  init()
        {
            myFiles = await deserialize("demofiles.xml");
            myDemoFilesXML.Clear();
            foreach (demofilesXML.fileentry f in myFiles.fileentries)
            {
                myDemoFilesXML.Add(f);
                demofiles.Add(new DemoFile(f.filename, f.filedescription, f.filehelp));
            }
            
        }

        async Task<demofilesXML> deserialize(string _filename)
        {
            //get folder
            StorageFolder PrintfilesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("printfiles");
            //get file
            StorageFile printfile = await PrintfilesFolder.GetFileAsync(_filename);
            if (printfile != null)
            {
                //read file
                Stream stream = await ReadFileContentsAsync(printfile);
                //read byte stream
                byte[] bXML = new byte[stream.Length];
                stream.Read(bXML, 0, (int)stream.Length);
                //convert to string
                string sXML = Encoding.UTF8.GetString(bXML, 0, bXML.Length);
                //deserialize string
                myFiles = demofilesXML.XmlDeserialize(sXML);

                return myFiles;
            }
            return new demofilesXML();
        }

        public async Task<Stream> ReadFileContentsAsync(StorageFile _fileName)
        {
            try
            {

                var file = await _fileName.OpenReadAsync();
                Stream stream = file.AsStreamForRead();                
                return stream;
                /*
                byte[] buf = new byte[stream.Length];
                int iRead = stream.Read(buf, 0, buf.Length);
                //addLog("read file = " + iRead.ToString());
                return buf;
                */
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}
