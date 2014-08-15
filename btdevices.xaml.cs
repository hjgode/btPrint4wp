using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Networking.Proximity;
using Microsoft.Phone.Tasks;

namespace btPrint4wp
{
    public partial class btdevices : PhoneApplicationPage
    {
        PeerInformation _peerInformation = null;
        public btdevices()
        {
            InitializeComponent();
            fillList();
        }
        async void fillList()
        {
            try
            {
                //PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                PeerFinder.AlternateIdentities["Bluetooth:SDP"] = "{00001101-0000-1000-8000-00805F9B34FB}";

                var peerList = await PeerFinder.FindAllPeersAsync();
                if (peerList.Count > 0)
                {
                    List<btdevice> peernames = new List<btdevice>();
                    foreach (PeerInformation pi in peerList)
                        peernames.Add(new btdevice(pi));

                    btlist.ItemsSource = peernames;
                }
                else
                {
                    MessageBox.Show("No active peers");
                }
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is MainPage && txtPeername.Text!="")
            {
                (e.Content as MainPage).peerInformation = _peerInformation;
                (e.Content as MainPage).textBoxPeer.Text = _peerInformation.DisplayName;
                addLog("Demo file " + _peerInformation.DisplayName + " selected");
            }
            base.OnNavigatedFrom(e);
        }

        private void btlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btdevice btd = (btdevice)btlist.SelectedItem;
            _peerInformation = btd.peerinfo;
            txtPeername.Text = btd.ToString();
        }

        void showBTSettings()
        {
            var task = new ConnectionSettingsTask();
            task.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            task.Show();

        }

        void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _peerInformation = ((btdevice) (btlist.SelectedItem)).peerinfo;
            txtPeername.Text = _peerInformation.DisplayName;
            NavigationService.GoBack();
        }

        class btdevice
        {
            public PeerInformation peerinfo;
            public override string ToString()
            {
                return peerinfo.DisplayName;
            }
            public btdevice(PeerInformation pi)
            {
                peerinfo = pi;
            }
        }
    }
}