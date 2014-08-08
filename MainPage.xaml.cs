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

namespace btPrint4wp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //var allowBluetooth = Windows.Networking.Proximity.PeerFinder.allowBluetooth;
            //Windows.Networking.Proximity.PeerFinder.allowBluetooth = allowBluetooth;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void btSearch_Click(object sender, RoutedEventArgs e)
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var peerList = await PeerFinder.FindAllPeersAsync();
            if (peerList.Count > 0)
            {
                textBoxPeer.Text = peerList[0].DisplayName;
                for (int i = 0; i < peerList.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}: '{1}'", i, peerList[i].DisplayName));
                }
            }
            else MessageBox.Show("No active peers");
        }

        private async void btConnect_Click(object sender, RoutedEventArgs e)
        {
            var peerList = await PeerFinder.FindAllPeersAsync();
            PeerInformation pi = peerList[0];
            if (peerList.Count > 0)
                textBoxPeer.Text = peerList[0].DisplayName;
            StreamSocket socket = new StreamSocket();
            //await socket.ConnectAsync(peerList[0].HostName, "0");
            try
            {
                await socket.ConnectAsync(pi.HostName, "1");//"{21EC2020-3AEA-1069-A2DD-08002B30309D}"); //SPP connect
                string s = "Hello\n";
                
                var buffer = System.Text.Encoding.UTF8.GetBytes(s);
                uint u = await socket.OutputStream.WriteAsync(WindowsRuntimeBufferExtensions.AsBuffer(buffer));
                System.Diagnostics.Debug.WriteLine("written: " + u.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }
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