using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using btPrint4wp.Resources;

using Windows.Networking.Proximity;
using Windows.Foundation;
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;

using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;

using Microsoft.Phone.Tasks;

using BTConnection;

namespace btPrint4wp
{
    public partial class MainPage : PhoneApplicationPage
    {
        BTConnection.ConnectionManager btConn;
        public PeerInformation peerInformation;
        public string sFileName = "";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            btConn = new ConnectionManager();
            btConn.ConnectDone += btConn_ConnectDone;
            btConn.MessageReceived += btConn_MessageReceived;


            /*
            try
            {
                var allowBluetooth = Windows.Networking.Proximity.PeerFinder.allowBluetooth;
                Windows.Networking.Proximity.PeerFinder.allowBluetooth = allowBluetooth;
            }
            catch (Exception ex) { 
                addLog("Exception: " + ex.Message);
            }
            */
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        void btConn_ConnectDone(Windows.Networking.HostName deviceHostName)
        {
            //await btConn.send("Hello \n");
            addLog("Connected to " + deviceHostName);
            //await btConn.SendCommand("Hello\n");
            //System.Threading.Thread.Sleep(2000);
        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                NavigationService.Navigate(new Uri("/btdevices.xaml", UriKind.Relative));
                
                return;

                /*
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                PeerFinder.AlternateIdentities["Bluetooth:SDP"] = "{00001101-0000-1000-8000-00805F9B34FB}";

                var peerList = await PeerFinder.FindAllPeersAsync();
                if (peerList.Count > 0)
                {
                    textBoxPeer.Text = peerList[0].DisplayName;
                    for (int i = 0; i < peerList.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("{0}: '{1}'", i, peerList[i].DisplayName));
                    }
                    ///ToDo: add code
                    peerInformation = peerList[0];
                }
                else
                {
                    MessageBox.Show("No active peers");
                }
                */
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    MessageBox.Show("Bluetooth is turned off");
                    showBTSettings();
                }
            }
        }

        void showBTSettings()
        {
            var task = new ConnectionSettingsTask();
            task.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            task.Show();

        }
        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            if (peerInformation == null)
            {
                addLog("Please use search first");
                ShellToast toast = new ShellToast();
                toast.Content = "Please use search first";
                toast.Title = "Error";
                toast.Show();
                return;
            }
            if (btConn.isConnected)
            {
                addLog("Already connected. Disconnect first!");
                return;
            }
            btConn.Connect(peerInformation.HostName);
            return;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btConn.isConnected) { 
                    //await btConn.send("Hello 2\n");
                    if (sFileName != "")
                    {
                        printFile(sFileName);
                        addLog("Send done");
                    }
                    else
                    {
                        addLog("no print file selected");
                    }
                }
                else
                {
                    addLog("NOT CONNECTED");
                }
            }
            catch (Exception ex) { addLog("Exception: " + ex.Message); }
        }

        async void printFile(string _filename)
        {
            StorageFolder PrintfilesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("printfiles");
            StorageFile printfile = await PrintfilesFolder.GetFileAsync(_filename);
            if (printfile != null)
            {
                byte[] buf = await ReadFileContentsAsync(printfile);
                await btConn.send(buf);
                addLog("printFile done");
            }
        }

        public async Task<byte[]> ReadFileContentsAsync(StorageFile _fileName)
        {
            try
            {

                var file = await _fileName.OpenReadAsync();
                Stream stream = file.AsStreamForRead();
                byte[] buf = new byte[stream.Length];
                int iRead = stream.Read(buf, 0, buf.Length);
                addLog("read file = " + iRead.ToString());
                return buf;
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        void addLog(string s)
        {
            string t = txtLog.Text;
            txtLog.Text = s + "\r\n" + t;
            System.Diagnostics.Debug.WriteLine(s);
        }

        void btConn_MessageReceived(string message)
        {
            addLog(message);
        }

        private void btDisconnect_Click(object sender, RoutedEventArgs e)
        {
            btConn.Disconnect();
            //btConn = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PrintfilesList.xaml", UriKind.Relative));
            
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Get a reference to the NavigationService that navigated to this Page 
            NavigationService ns = this.NavigationService;
            ns.Navigating += ns_Navigating;            
        }

        void ns_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Uri.ToString() + ":" + e.NavigationMode.ToString());
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}