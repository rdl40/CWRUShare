using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.PeerToPeer;
using System.Threading;
using CWRUShare;
using Lidgren.Network;

namespace CWRUNet
{
    public static class ConnectionManager
    {
        private static NetServer server;
        private static NetPeerConfiguration config;
        private static UserList userList;
        private static bool isConnected;

        static ConnectionManager()
        {
            config = new NetPeerConfiguration("CWRUShare");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.Port = 14242;
            isConnected = false;

            server = new NetServer(config);
            server.Start();
        }

        public static void SetUserList(UserList reference)
        {
            userList = reference;
        }

        public static UserList ExportUserList()
        {
            return userList;
        }
        
        public static void Listen(SendOrPostCallback listener)
        {
            server.RegisterReceivedCallback(listener);
        }

        public static void Send()
        {
            server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14242));
        }

        public static void ExtendedDiscovery()
        {
            for(int x = 60; x <= 70; x++)
            {
                 for(int y = 70; y <= 75; y++)
                 {
                     Console.WriteLine(IPAddress.Parse(String.Format("129.22.{0}.{1}", x, y)));
                     server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse(String.Format("129.22.{0}.{1}", x, y)), 14242));
                 }
            }

            Console.WriteLine("deep discovery finished");
        }

        public static void Discover()
        {
            server.DiscoverLocalPeers(14242);

            List<IPEndPoint> temp = userList.GetActivePeers();

            foreach (var endpoint in temp)
            {
                server.DiscoverKnownPeer(endpoint);
            }

            server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse("129.22.63.71"), 14242));
        }

        internal static void Ping(IPEndPoint location)
        {
            Messages message = new Messages();
            message.MessageType = Message.Ping;
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Data = message.ToByteArray();
            server.SendUnconnectedMessage(msg, location);
        }

        internal static void ReplyToPing(NetIncomingMessage msg)
        {
            Messages message = new Messages();
            message.MessageType = Message.Ping;
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Data = message.ToByteArray();
            server.SendMessage(outgoingMessage, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }

        internal static void SendFileList(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveFileList(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveUserList(NetIncomingMessage msg)
        {
            Messages message = Messages.FromByteArray(msg.Data);
            userList.MergeUserList((UserList) message.Data);
        }

        internal static void SendFiles(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveFiles(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        internal static void PingReplyRecieved(NetIncomingMessage msg)
        {
            userList.UpdateUser(msg.SenderConnection.RemoteEndPoint.Address.ToString());
        }

        internal static void SendUserList(NetIncomingMessage msg)
        {
            Messages message = new Messages();
            message.MessageType = Message.RecieveUserList;
            message.Data = userList;
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Data = message.ToByteArray();
            server.SendUnconnectedMessage(outgoingMessage, msg.SenderEndPoint);
        }

        public static bool IsConnected()
        {
            return isConnected;
        }

        internal static void ReplyToDiscovery(NetIncomingMessage msg)
        {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            Messages message = new Messages();
            message.MessageType = Message.DiscoveryReply;
            outgoingMessage.Data = message.ToByteArray();
            Console.WriteLine("Discovery reply length:" + outgoingMessage.Data.Length + "\nAddress: " + msg.SenderEndPoint.Address.ToString());
            server.SendUnconnectedMessage(outgoingMessage, msg.SenderEndPoint);
        }

        internal static void RequestUserList(IPEndPoint peer)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            Messages message = new Messages();
            message.MessageType = Message.RequestUserList;
            msg.Data = message.ToByteArray();
            server.SendUnconnectedMessage(msg, peer);
        }

        internal static void RecievedDiscoveryReply(NetIncomingMessage msg)
        {
            userList.AddUser(msg.SenderEndPoint.Address.ToString());
            isConnected = true;
            RequestUserList(msg.SenderEndPoint);
        }

        public static List<string> GetUserList()
        {
            List<string> toBeReturned = new List<string>();
            foreach (var user in userList.GetActivePeers())
            {
                toBeReturned.Add(user.Address.ToString());
            }

            return toBeReturned;
        }

    }
}
