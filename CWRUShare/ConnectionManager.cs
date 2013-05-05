using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.PeerToPeer;
using System.Threading;
using Lidgren.Network;

namespace CWRUNet
{
    public static class ConnectionManager
    {
        private static NetServer server;
        private static NetPeerConfiguration config;

        static ConnectionManager()
        {
            config = new NetPeerConfiguration("Hello");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.Port = 14242;

            server = new NetServer(config);
            server.Start();
        }
        
        public static void Listen(SendOrPostCallback listener)
        {
            server.RegisterReceivedCallback(listener);
        }

        public static void Send()
        {
            server.DiscoverLocalPeers(14242);
        }
    }
}
