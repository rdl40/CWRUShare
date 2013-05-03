﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CWRUShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShareManager shareManager = new ShareManager();

        private FOVTreeNode supertemp;

        public MainWindow()
        {
            InitializeComponent();

            FolderView.SetSpecialFolderAsRoot(LogicNP.FolderViewControl.SpecialFolders.DRIVES);
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
                    recurseDirectories(shareManager.getNextNameHolder(), x, shareManager);
                }
            }
        }

        private void recurseDirectories(int parentNode, FOVTreeNode currentNode, ShareManager manager)
        {
            int currentNameHolder = manager.getNextNameHolder();

            DirectoryItem item = new DirectoryItem();

            item.Name = currentNode.DisplayName;
            item.Icon = currentNode.IconIndex;

            manager.addIcon(currentNode.IconIndex, currentNode.GetShellIcon(ShellIconTypes.Thumbnail));
            manager.addData(currentNameHolder, item);

            manager.addInstructions(parentNode + "|" + currentNameHolder);
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
                    recurseDirectories2(shareManager.getNextNameHolder(), x);
                }
            }
        }

        private void recurseDirectories2(int parentNode, FOVTreeNode currentNode)
        {

            
            int currentNameHolder = shareManager.getNextNameHolder();

            

            if (currentNode.Checked)
            {
                DirectoryItem item = new DirectoryItem();
                item.Name = currentNode.DisplayName;
                item.Icon = currentNode.IconIndex;

                shareManager.addIcon(currentNode.IconIndex, currentNode.GetShellIcon(ShellIconTypes.Thumbnail));
                shareManager.addData(currentNameHolder, item);
                shareManager.addInstructions(parentNode + "|a" + currentNameHolder);

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

            int h = share.getNextNameHolder();
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

            using (StringReader reader = new StringReader(share.getInstructions()))
            {
                Console.WriteLine(share.getInstructions());
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

                    DirectoryItem temp = share.getDirectoryItemFromNameHolder(getSecondHolder(line));

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


    }
}