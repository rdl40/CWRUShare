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

        static ConnectionManager()
        {
            config = new NetPeerConfiguration("CWRUShare");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.Port = 14242;

            server = new NetServer(config);
            server.Start();
        }

        public static void SetUserList(UserList reference)
        {
            userList = reference;
        }
        
        public static void Listen(SendOrPostCallback listener)
        {
            server.RegisterReceivedCallback(listener);
        }

        public static void Send()
        {

            server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14242));
            //for(int x = 0; x <= 255; x++)
            //{
            //     for(int y = 1; y <= 254; y++)
            //     {
            //         server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse(String.Format("129.22.{0}.{1}", x, y)), 14242));
            //         Console.WriteLine(String.Format("129.22.{0}.{1}", x, y));
            //         Thread.Sleep(1);
            //     }
            //}
        }

        public static void Discover()
        {
            server.DiscoverLocalPeers(14242);

            foreach (var endpoint in userList.GetActivePeers())
            {
                server.DiscoverKnownPeer(endpoint);
            }
        }

        internal static void Ping(IPEndPoint location)
        {
            //server.SendMessage(server.CreateMessage("CWRUNet!"), new NetConnection()
        }

        internal static void ReplyToPing(NetIncomingMessage message)
        {
            server.SendMessage(server.CreateMessage("CWRUNet!"), message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }

        internal static void SendFileList(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveFileList(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveUserList(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void SendFiles(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void RecieveFiles(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void PingReplyRecieved(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        internal static void SendUserList(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        public static bool IsConnected()
        {
            return false;
        }
    }
}
