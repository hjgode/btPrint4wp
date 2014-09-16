using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace btPrint4wp
{
    public class DemoFilesFileBased:DemoFiles
    {
        //public List<DemoFile> demofiles { get; private set; }

        public DemoFilesFileBased()
        {
            demofiles = new List<DemoFile>();
            //read files
            readList();
        }

        public async void readList()
        {
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;//.GetFolderAsync("/printfiles");
            StorageFolder PrintfilesFolder = await InstallationFolder.GetFolderAsync("printfiles");

            IReadOnlyList<StorageFile> filelist = await PrintfilesFolder.GetFilesAsync();// .GetItemsAsync();

            IStorageItem[] files = filelist.ToArray();
            List<string> filenamelist = new List<string>();

            foreach (StorageFile i in filelist)
            {
                if (i.Name.EndsWith("prn"))
                {
                    filenamelist.Add(i.Name);
                    DemoFile df = new DemoFile(i.Name, "n/a", "n/a");
                    demofiles.Add(df);
                }
                System.Diagnostics.Debug.WriteLine(i.Name);
            }
        }
    }
}
