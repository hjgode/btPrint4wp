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
    public class DemoFilesXML
    {
        StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        public demofiles myFiles = new demofiles();
        public List<demofiles.fileentry> myDemoFilesXML = new List<demofiles.fileentry>();
        public DemoFilesXML()
        {
            init();
        }
        async Task  init()
        {
            myFiles = await deserialize("demofiles.xml");
            myDemoFilesXML.Clear();
            foreach (demofiles.fileentry f in myFiles.fileentries)
                myDemoFilesXML.Add(f);

        }
        public string getDescription(string filename)
        {
            foreach (demofiles.fileentry f in myFiles.fileentries)
            {
                if (f.filename.ToLower() == filename.ToLower())
                    return f.filedescription;
            }
            return "n/a";
        }
        public string getHelp(string filename)
        {
            foreach (demofiles.fileentry f in myFiles.fileentries)
            {
                if (f.filename.ToLower() == filename.ToLower())
                    return f.filehelp;
            }
            return "n/a";
        }


        async Task<demofiles> deserialize(string _filename)
        {
            StorageFolder PrintfilesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("printfiles");
            StorageFile printfile = await PrintfilesFolder.GetFileAsync(_filename);
            if (printfile != null)
            {
                Stream stream = await ReadFileContentsAsync(printfile);
                byte[] bXML = new byte[stream.Length];
                stream.Read(bXML, 0, (int)stream.Length);
                string sXML = Encoding.UTF8.GetString(bXML, 0, bXML.Length);
                myFiles = demofiles.XmlDeserialize(sXML);
                return myFiles;
            }
            return new demofiles();
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
