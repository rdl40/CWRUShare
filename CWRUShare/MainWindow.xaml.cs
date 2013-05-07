using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private ShareManager shareManager = new ShareManager();

       // private FOVTreeNode supertemp;

        private UserList users;
        private string downloadDirectory;
        private DispatcherTimer discoveryTimer;

        public MainWindow()
        {
            InitializeComponent();

            downloadDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CWRUShare\\";
            // Get the c:\ directory.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(downloadDirectory);

            users = new UserList();

            // For each file in the c:\ directory, create a ListViewItem 
            // and set the icon to the icon extracted from the file. 
            foreach (System.IO.FileInfo file in dir.GetFiles())
            {
                fileView.Items.Add(file.Name);
            }

            ConnectionManager.SetUserList(users);
            ConnectionManager.Listen(new SendOrPostCallback(Listener));
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

        public static void Listener(object peer)
        {

            Console.WriteLine("Recieved message");
            var msg = ((NetServer) peer).ReadMessage();

            if (msg.MessageType != NetIncomingMessageType.Data)
            {
                if (msg.MessageType == NetIncomingMessageType.DiscoveryRequest)
                {
                    ConnectionManager.ReplyToDiscovery(msg);
                }
            }

            Messages message = Messages.FromByteArray(msg.Data);

            switch (message.MessageType)
            {
                case Message.DiscoveryReply:
                    ConnectionManager.RecievedDiscoveryReply(msg);
                    break;
                case Message.Ping:
                    ConnectionManager.ReplyToPing(msg);
                    break;
                case Message.PingReply:
                    ConnectionManager.PingReplyRecieved(msg);
                    break;
                case Message.RequestFileList:
                    ConnectionManager.SendFileList(msg);
                    break;
                case Message.RecieveFileList:
                    ConnectionManager.RecieveFileList(msg);
                    break;
                case Message.RequestUserList:
                    ConnectionManager.SendUserList(msg);
                    break;
                case Message.RecieveUserList:
                    ConnectionManager.RecieveUserList(msg);
                    break;
                case Message.RequestFiles:
                    ConnectionManager.SendFiles(msg);
                    break;
                case Message.RecieveFile:
                    ConnectionManager.RecieveFiles(msg);
                    break;
                case Message.Leaving:
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
            Console.WriteLine("Timer started");
        }

    }
}
