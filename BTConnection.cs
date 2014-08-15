using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

using System.Runtime.InteropServices.WindowsRuntime;

using System.IO;

//see http://developer.nokia.com/community/wiki/Windows_Phone_8_communicating_with_Arduino_using_Bluetooth

namespace BTConnection
{
    /// <summary>
    /// Class to control the bluetooth connection to the Arduino.
    /// </summary>
    public class ConnectionManager:IDisposable
    {
        bool _bIsConnected = false;
        /// <summary>
        /// Socket used to communicate with Arduino.
        /// </summary>
        private StreamSocket socket=null;
        
        /// <summary>
        /// DataWriter used to send commands easily.
        /// </summary>
        private DataWriter dataWriter;

        /// <summary>
        /// DataReader used to receive messages easily.
        /// </summary>
        private DataReader dataReader;

        /// <summary>
        /// Thread used to keep reading data from socket.
        /// </summary>
        private BackgroundWorker dataReadWorker;

        public delegate void ConnectedHandler(HostName deviceHostName);
        public event ConnectedHandler ConnectDone;

        /// <summary>
        /// Delegate used by event handler.
        /// </summary>
        /// <param name="message">The message received.</param>
        public delegate void MessageReceivedHandler(string message);

        /// <summary>
        /// Event fired when a new message is received from Arduino.
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        public ConnectionManager()
        {

        }
        /// <summary>
        /// Initialize the manager, should be called in OnNavigatedTo of main page.
        /// </summary>
        void Initialize()
        {
            socket = new StreamSocket();
            dataReadWorker = new BackgroundWorker();
            dataReadWorker.WorkerSupportsCancellation = true;
            //dataReadWorker.DoWork += new DoWorkEventHandler(ReceiveMessages);
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        /// <summary>
        /// Finalize the connection manager, should be called in OnNavigatedFrom of main page.
        /// </summary>
        void Terminate()
        {
            addLog("Terminate started");
            if (dataReadWorker != null)
            {
                dataReadWorker.CancelAsync();
                dataReadWorker.DoWork -= ReceiveMessages;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            dataReadWorker = null;
            socket = null;
            MessageReceived(">>>Disconnected");
            _bIsConnected = false;
            addLog("Terminate ended");
        }

        public bool isConnected
        {
            get
            {
                return _bIsConnected;
            }
        }
        /// <summary>
        /// Connect to the given host device.
        /// </summary>
        /// <param name="deviceHostName">The host device name.</param>
        public async void Connect(HostName deviceHostName)
        {
            if(!_bIsConnected)// if (socket != null)
            {
                Initialize();
                dataReadWorker.DoWork += new DoWorkEventHandler(ReceiveMessages);
                await socket.ConnectAsync(deviceHostName, "1"); //can we use the SPP UUID too? {00001101-0000-1000-8000-00805F9B34FB} 
                
                dataReader = new DataReader(socket.InputStream);
                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                dataReadWorker.RunWorkerAsync();
                
                dataWriter = new DataWriter(socket.OutputStream);
                
                MessageReceived(">>>connected to: " + deviceHostName);
                ConnectDone(deviceHostName);
                _bIsConnected = true;
            }
        }

        public void Disconnect()
        {
            addLog("+++Disconnect");
            dataReadWorker.DoWork -= ReceiveMessages;
            dataReadWorker.CancelAsync();

            //dataReader.DetachStream();
            dataReader.DetachBuffer();
            dataReader.Dispose();
            dataReader = null;

            //dataWriter.DetachStream();
            dataWriter.DetachBuffer();
            dataWriter.Dispose();
            dataWriter = null;

            if (socket != null)
            {
                socket.Dispose();
            }

            _bIsConnected = false;
            socket = null;
            MessageReceived("Disconnect done");
            addLog("---Disconnect");
        }
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Receive messages from the Arduino through bluetooth.
        /// </summary>
        private async void ReceiveMessages(object sender, DoWorkEventArgs e)
        {
            addLog("+++ReceiveMessages started");
            BackgroundWorker worker = sender as BackgroundWorker;
            bool bRunMe = true;
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                addLog("CancellationPending");
                bRunMe = false;
            }
            do
            {
                try
                {
                    await dataReader.LoadAsync(1);                    
                    uint dataLen = dataReader.UnconsumedBufferLength;
                    if (dataLen > 0)
                    {
                        //byte[] buffer = ReadFully(socket.InputStream.AsStreamForRead());
                        //string sMsg = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                        byte[] buf = new byte[dataLen];
                        dataReader.ReadBytes(buf);
                        string sMsg = Encoding.UTF8.GetString(buf, 0, buf.Length);
                        MessageReceived(sMsg);
                    }
                }
                catch (System.ObjectDisposedException ex)
                {
                    addLog("ReceiveMessages ObjectDisposedException: " + ex.Message);
                    bRunMe = false;
                }
                catch (TaskCanceledException ex)
                {
                    addLog("ReceiveMessages TaskCanceledException: " + ex.Message);
                    bRunMe = false;
                }
                catch (System.NullReferenceException ex)
                {
                    addLog("ReceiveMessages NullReferenceException: " + ex.Message);
                    bRunMe = false;
                }
                catch (Exception ex)
                {
                    addLog("ReceiveMessages Exception: " + ex.Message);
                    bRunMe = false;
                }

                /*
                // Read first byte (length of the subsequent message, 255 or less). 
                uint sizeFieldCount = await dataReader.LoadAsync(1);
                if (sizeFieldCount != 1)
                {
                    // The underlying socket was closed before we were able to read the whole data. 
                    return;
                }

                // Read the message. 
                uint messageLength = dataReader.ReadByte();
                uint actualMessageLength = await dataReader.LoadAsync(messageLength);
                if (messageLength != actualMessageLength)
                {
                    // The underlying socket was closed before we were able to read the whole data. 
                    return;
                }
                // Read the message and process it.
                string message = dataReader.ReadString(actualMessageLength);
                MessageReceived(message);
                */
            } while (bRunMe);
            addLog("---ReceiveMessages ended");
        }

        /// <summary>
        /// Send command to the Arduino through bluetooth.
        /// </summary>
        /// <param name="command">The sent command.</param>
        /// <returns>The number of bytes sent</returns>
        public async Task<uint> send(string command)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                //uint commandSize = dataWriter.MeasureString(command);
                //dataWriter.WriteByte((byte)commandSize);
                sentCommandSize = dataWriter.WriteString(command);
                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
            }
            return sentCommandSize;
        }
        public async Task<uint> send(IBuffer buffer)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                //uint commandSize = dataWriter.MeasureString(command);
                //dataWriter.WriteByte((byte)commandSize);
                dataWriter.WriteBuffer(buffer, 0, buffer.Length);
                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
                sentCommandSize = (uint) buffer.Length;
            }
            return sentCommandSize;
        }
        public async Task<uint> send(byte[] buffer)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                //uint commandSize = dataWriter.MeasureString(command);
                //dataWriter.WriteByte((byte)commandSize);
                dataWriter.WriteBuffer(buffer.AsBuffer());
                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
                sentCommandSize = (uint) buffer.Length;
            }
            return sentCommandSize;
        }


        static void addLog(string s){
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
