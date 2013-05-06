using System;
using System.Collections.Generic;
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
using LogicNP.FolderViewControl;
using LogicNP.ShellObjects;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using CWRUNet;
using Lidgren.Network;
using Task = System.Threading.Tasks.Task;
using LogicNP.FileViewControl;

namespace CWRUShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShareManager shareManager = new ShareManager();

        private FOVTreeNode supertemp;

        private UserList users;

        public MainWindow()
        {
            InitializeComponent();

            FolderView.SetSpecialFolderAsRoot(LogicNP.FolderViewControl.SpecialFolders.DRIVES);

            FolderView.FileView = FileView1;

            ConnectionManager.Listen(new SendOrPostCallback(Listener));
        }

        private void FolderView_Loaded(object sender, RoutedEventArgs e)
        {
            bool isFirst = true;
            shareManager = new ShareManager();

            foreach (FOVTreeNode x in FolderView.Nodes)
            {
                if (isFirst)
                {
                    isFirst = false;
                    x.ShowCheckBox = false;
                    supertemp = x;
                    recurseDirectories(shareManager.GetNextNameHolder(), x, shareManager);
                }
            }

            //ConnectionManager.RegisterUser();
        }

        private void recurseDirectories(int parentNode, FOVTreeNode currentNode, ShareManager manager)
        {
            int currentNameHolder = manager.GetNextNameHolder();

            DirectoryItem item = new DirectoryItem();

            item.Name = currentNode.DisplayName;
            item.Icon = currentNode.IconIndex;

            manager.AddIcon(currentNode.IconIndex, currentNode.GetShellIcon(LogicNP.FolderViewControl.ShellIconTypes.Thumbnail));
            manager.AddData(currentNameHolder, item);

            manager.AddInstructions(parentNode + "|" + currentNameHolder);
            Console.WriteLine(parentNode + "|" + currentNameHolder);

            if (currentNode.GetChildren(false) != 0)
                recurseDirectories(currentNameHolder, currentNode.GetChild(false), manager);
            
            if (currentNode.NextSibling != null)
            {
                recurseDirectories(parentNode, currentNode.NextSibling, manager);
            }


        }

        private void treeReducer()
        {
            bool isFirst = true;

            shareManager = new ShareManager();

            foreach (FOVTreeNode x in FolderView.Nodes)
            {
                if (isFirst)
                {
                    isFirst = false;
                    x.ShowCheckBox = false;
                    supertemp = x;
                    recurseDirectories2(shareManager.GetNextNameHolder(), x);
                }
            }
        }

        private void recurseDirectories2(int parentNode, FOVTreeNode currentNode)
        {

            
            int currentNameHolder = shareManager.GetNextNameHolder();

            

            if (currentNode.Checked)
            {
                DirectoryItem item = new DirectoryItem();
                item.Name = currentNode.DisplayName;
                item.Icon = currentNode.IconIndex;

                shareManager.AddIcon(currentNode.IconIndex, currentNode.GetShellIcon(LogicNP.FolderViewControl.ShellIconTypes.Thumbnail));
                shareManager.AddData(currentNameHolder, item);
                shareManager.AddInstructions(parentNode + "|a" + currentNameHolder);

                if (currentNode.GetChildren(true) != 0)
                    recurseDirectories2(currentNameHolder, currentNode.GetChild(true));
            }

            if (currentNode.NextSibling != null)
            {
                recurseDirectories2(parentNode, currentNode.NextSibling);
            }
        }

        private void FolderView2_Loaded(object sender, RoutedEventArgs e)
        {
            buildFolderView(new ShareManager());
        }

        private void buildFolderView(ShareManager share)
        {
            bool isFirst = true;
            FolderView2.SetSpecialFolderAsRoot(LogicNP.FolderViewControl.SpecialFolders.DRIVES);

            ArrayList toBeDelted = new ArrayList();

            foreach (FOVTreeNode x in FolderView2.NodesInReverse)
            {
                x.ShowCheckBox = false;
                toBeDelted.Add(x);
            }

            for (int x = 0; x < toBeDelted.Count - 1; x++)
            {
                ((FOVTreeNode)toBeDelted[x]).Delete();
            }

            int h = share.GetNextNameHolder();
            Console.WriteLine(h + "asdfasdf");


            recurseDirectories(h, supertemp, share);

            Stream stream = File.Open("aidsman.xml", FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, share);
            stream.Close();

            share = null;
            Stream stream2 = File.Open("aidsman.xml", FileMode.Open);
            share = (ShareManager)bformatter.Deserialize(stream2);
            stream2.Close();

            bool firstTime = true;

            Dictionary<int, FOVTreeNode> refholder = new Dictionary<int, FOVTreeNode>();

            using (StringReader reader = new StringReader(share.GetInstructions()))
            {
                Console.WriteLine(share.GetInstructions());
                int lastindex = 1;
                FOVTreeNode tempnode = FolderView2.GetFirstNode();
                refholder.Add(lastindex, tempnode);
                string line;

                while ((line = reader.ReadLine()) != null)
                {

                    if (firstTime)
                    {
                        firstTime = false;
                        continue;
                        //DirectoryItem temp = shareManager.getDirectoryItemFromNameHolder(getSecondHolder(line));
                        //FolderView2.AddCustomNode(FolderView2.GetFirstNode(), NodeAddRelationTypes.AddAsLastChild, temp.Name, temp.Icon,
                        //temp.Icon);
                    }

                    DirectoryItem temp = share.GetDirectoryItemFromNameHolder(getSecondHolder(line));

                    refholder.Add(getSecondHolder(line), FolderView2.AddCustomNode(refholder[getFirstHolder(line)], NodeAddRelationTypes.AddAsLastChild, temp.Name, temp.Icon, temp.Icon));
                }
            }
        }

        private int getFirstHolder(string line)
        {
            return int.Parse(line.Substring(0, line.IndexOf('|')));
        }

        private int getSecondHolder(string line)
        {
            return int.Parse(line.Substring(line.IndexOf('|') + 1));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            buildFolderView(new ShareManager());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //ConnectionManager.ResolveUsers();
            Sender();
        }

        public static void Listener(object peer)
        {

            Console.WriteLine("Recieved message");
            var msg = ((NetServer) peer).ReadMessage();

            Messages message = Messages.FromByteArray(msg.Data);

            switch (message.MessageType)
            {
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

            Console.WriteLine("oooh yaaaa");
        }

        private void FolderView_BeforeCheck(object sender, FolderViewCancelEventArgs e)
        {
            Console.WriteLine("wieners");
        }

        private void FolderView_QueryNodeCheckState(object sender, FolderViewEventArgs e)
        {
            Console.WriteLine("wieners");
        }

        private void FileView1_CurrentFolderChanged(object sender, EventArgs e)
        {
            FOVTreeNode node = FolderView.SelectedNode;
            if (node == null)
                return;

            // Initially, make all items checked/unchecked depending on FolderView node check state.
            FileView1.CheckItems(node.Checked ? CheckTypes.All : CheckTypes.None);

            // sync check states of all items in FileView with those of all child nodes of the 
            // selected node in FolderView
            node = node.GetChild(false);
            while (node != null)
            {
                LogicNP.FileViewControl.ListItem item = FileView1.GetItemFromName(node.DisplayName);
                if (item != null)
                    item.CheckState = (ItemCheckStates)node.CheckState;

                node = node.NextSibling;
            }
        }


    }
}
