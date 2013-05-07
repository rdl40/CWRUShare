using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Windows.Threading;
using Lidgren.Network;
using CWRUNet;


namespace CWRUShare
{
    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileList fileList;
        private UserList users;
        private string downloadDirectory;
        private DispatcherTimer peerListTimer;
        private DispatcherTimer discoveryTimer;



        private static IPAddress thisAddress;
        private static IPEndPoint fileListRequestSource;

        public MainWindow()
        {
            InitializeComponent();

            downloadDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CWRUShare\\";
            // Get the c:\ directory.

            users = new UserList();

            //users.AddUser("192.168.1.1");

            fileList = new FileList();

            fileList.PopulateFileList(downloadDirectory);


            foreach (File file in fileList.GetFileList())
            {
                ListViewItem item = new ListViewItem();
                item.Content = file.Name;
                item.Tag = file.ID;

                fileView.Items.Add(item);
            }

            thisAddress = (Dns.GetHostEntry(Dns.GetHostName())).AddressList[0];

            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    thisAddress = address;
                    break;
                }
            }

            ConnectionManager.Listen(new SendOrPostCallback(Listener));
            ConnectionManager.SetUserList(users);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //buildFolderView(new ShareManager());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void UpdatePeerView()
        {
            peerView.Items.Clear();
            foreach (var user in ConnectionManager.GetUserList())
            {
                peerView.Items.Add(user);
            }
        }

        private void DiscoveryTimerTick(object sender, EventArgs e)
        {
            Console.WriteLine("starting deep discovery");
            ConnectionManager.ExtendedDiscovery();
            discoveryTimer.Stop();
        }

        private void PeerListTimerTick(object sender, EventArgs e)
        {
            UpdatePeerView();
        }

        public static void Listener(object peer)
        {
            var msg = ((NetServer) peer).ReadMessage();

            if (msg.MessageType != NetIncomingMessageType.UnconnectedData && msg.MessageType != NetIncomingMessageType.Data)
            {
                if (msg.MessageType == NetIncomingMessageType.DiscoveryRequest)
                {
                    ConnectionManager.ReplyToDiscovery(msg);
                }

                return;
            }

            if (msg.SenderEndPoint.Address.Equals(thisAddress))
            {
                return;
            }
            

            Console.WriteLine("Recieved Message: " + msg.MessageType);

            Console.WriteLine(msg.SenderEndPoint.Address.ToString());
            Console.WriteLine(msg.Data.Length);
            Messages message = Messages.FromByteArray(msg.Data);

            switch (message.MessageType)
            {
                case Message.DiscoveryReply:
                    Console.Write("Discovery reply recieved");
                    ConnectionManager.RecievedDiscoveryReply(msg);
                    break;
                case Message.Ping:
                    Console.Write("Ping recieved");
                    ConnectionManager.ReplyToPing(msg);
                    break;
                case Message.PingReply:
                    Console.Write("PingReply recieved");
                    ConnectionManager.PingReplyRecieved(msg);
                    break;
                case Message.RequestFileList:
                    Console.Write("RequestFileList recieved");
                    ConnectionManager.SendFileList(msg);
                    break;
                case Message.RecieveFileList:
                    if (msg.SenderEndPoint.Equals(fileListRequestSource))
                    {
                        ConnectionManager.RecieveFileList(msg);
                    }
                    break;
                case Message.RequestUserList:
                    Console.Write("RequestUserList recieved");
                    ConnectionManager.SendUserList(msg);

                    break;
                case Message.RecieveUserList:
                    Console.Write("RecieveUserList recieved");
                    ConnectionManager.RecieveUserList(msg);
                    break;
                case Message.RequestFiles:
                    Console.Write("REquestFiles recieved");
                    ConnectionManager.SendFiles(msg);
                    break;
                case Message.RecieveFile:
                    Console.Write("RecieveFile recieved");
                    ConnectionManager.RecieveFiles(msg);
                    break;
                case Message.Leaving:
                    Console.Write("Leaving recieved");
                    break;

                default:
                    Console.WriteLine(message.MessageType.ToString());
                    break;
            }
        }

        public async static void Sender()
        {
            var tasker = Task.Factory.StartNew(ConnectionManager.Send);
            await tasker;
        }

        private void viewFilesButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(downloadDirectory);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            ConnectionManager.Discover();
            discoveryTimer = new DispatcherTimer();
            discoveryTimer.Interval = TimeSpan.FromSeconds(10);
            discoveryTimer.Tick += DiscoveryTimerTick;
            discoveryTimer.Start();

            peerListTimer = new DispatcherTimer();
            peerListTimer.Interval = TimeSpan.FromSeconds(10);
            peerListTimer.Tick += PeerListTimerTick;
            peerListTimer.Start();

            Console.WriteLine("Timer started");
        }

    }
}
