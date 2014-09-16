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
    public partial class DemoFilesPage : PhoneApplicationPage
    {
        StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        public string filename = "";
//        List<demoFile> demoFiles=new List<demoFile>();
//        DemoFilesXMLBased myDemoFiles = new DemoFilesXMLBased(); //build a list of xml print infos

            //either use file based list
            //DemoFilesFileBased demoFiles = new DemoFilesFileBased();
            
            // or use xml based file list
        //static to get same result on page back and for navigation, otherwise listbox is empty on subsequent navigateTo
        static DemoFilesXMLBased demoFiles = new DemoFilesXMLBased();

        public DemoFilesPage()
        {
            InitializeComponent();
            filelist.ItemsSource = null;
            filelist.ItemsSource = demoFiles.demofiles;// myDemoFiles.myDemoFilesXML;
        }

        //public class demoFile:IDemoFile
        //{
        //    public string filename { get; set; }
        //    public string filetype { get; set; }
        //    public string filedescription { get; set; }
        //    public string fileimage {get; set;}
        //    public string filehelp { get; set; }
        //    public demoFile(string name)
        //    {
        //        filename = name;
        //        filetype = filename.Substring(0, 4);
                
        //        if (filetype.StartsWith("fp"))
        //            fileimage = "images/fp.gif";
        //        else if (filetype.StartsWith("csim"))
        //            fileimage = "images/csim.gif";
        //        else if (filetype.StartsWith("escp"))
        //            fileimage = "images/escp.gif";
        //        else if (filetype.StartsWith("ipl"))
        //            fileimage = "images/ipl.gif";
        //        else if (filetype.StartsWith("xsim"))
        //            fileimage = "images/xsim.gif";
        //        else
        //            fileimage = "images/pb42.gif";
        //    }
        //    public override string ToString()
        //    {
        //        return filename;
        //    }
        //}
        //async void readFilesList()
        //{
        //    StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;//.GetFolderAsync("/printfiles");
        //    StorageFolder PrintfilesFolder = await InstallationFolder.GetFolderAsync("printfiles");

        //    IReadOnlyList<StorageFile> filelist = await PrintfilesFolder.GetFilesAsync();// .GetItemsAsync();

        //    IStorageItem[] files = filelist.ToArray();
        //    List<string> filenamelist = new List<string>();

        //    foreach (StorageFile i in filelist)
        //    {
        //        if (i.Name.EndsWith("prn"))
        //        {
        //            filenamelist.Add(i.Name);
        //            demoFile df = new demoFile(i.Name);
        //            df.filedescription = myDemoFiles.getDescription(i.Name);
        //            df.filehelp = myDemoFiles.getHelp(i.Name);

        //            demoFiles.Add(df);
        //        }
        //        System.Diagnostics.Debug.WriteLine(i.Name);
        //    }
        //    //listBox.ItemsSource = filenamelist;

        //    //StorageFile file = await InstallationFolder.GetFileAsync(CountriesFile);

        //}

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filelist.SelectedIndex != -1)
            {
                DemoFile df = (DemoFile)filelist.SelectedItem;
                txtFilename.Text = df.filename;
                filename = txtFilename.Text;
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var buttonElement = (Button)e.OriginalSource;
            filename = buttonElement.Tag.ToString();
            txtFilename.Text = filename;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (filelist.Items.Count > 0)
            {
                filelist.ItemsSource = null;
            }
            demoFiles = new DemoFilesXMLBased();

            filelist.ItemsSource = demoFiles.demofiles;// myDemoFiles.myDemoFilesXML;
        }
    }
}