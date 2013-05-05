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

        static ConnectionManager()
        {
            config = new NetPeerConfiguration("CWRUShare");
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
            for(int x = 0; x <= 255; x++)
            {
                 for(int y = 1; y <= 255; y++)
                 {
                     server.DiscoverKnownPeer(new IPEndPoint(IPAddress.Parse(String.Format("129.22.{0}.{1}", x, y)), 14242));
                     Console.WriteLine(String.Format("129.22.{0}.{1}", x, y));
                     Thread.Sleep(1);
                 }
            }
            
        }




    }
}
