using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Storage;

namespace btPrint4wp
{
    public partial class Page1 : PhoneApplicationPage
    {
        StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        public string filename = "";

        public Page1()
        {
            InitializeComponent();
            readFilesList();
        }
        async void readFilesList()
        {
            //string CountriesFile = @"Assets\Countries.xml";
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;//.GetFolderAsync("/printfiles");
            StorageFolder PrintfilesFolder = await InstallationFolder.GetFolderAsync("printfiles");

            IReadOnlyList<StorageFile> filelist = await PrintfilesFolder.GetFilesAsync();// .GetItemsAsync();
            
            IStorageItem[] files = filelist.ToArray();
            List<string> filenamelist = new List<string>();

            foreach (StorageFile i in filelist)
            {
                if (i.Name.EndsWith("prn"))
                    filenamelist.Add(i.Name);
                System.Diagnostics.Debug.WriteLine(i.Name);
            }
            listBox.ItemsSource = filenamelist;

            //StorageFile file = await InstallationFolder.GetFileAsync(CountriesFile);

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtFilename.Text = listBox.SelectedItem.ToString();
            filename = txtFilename.Text;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is MainPage)
            {
                (e.Content as MainPage).sFileName=filename;
                (e.Content as MainPage).txtFile.Text = filename;
                addLog("Demo file " + filename + " selected");
            }
            base.OnNavigatedFrom(e);
        }
        void addLog(string s)
        {
//            string t = txtLog.Text;
//            txtLog.Text = s + "\r\n" + t;
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}