using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.PeerToPeer;

namespace CWRUNet
{
    static class ConnectionManager
    {

        public static void RegisterUser()
        {
            PeerName name = new PeerName("eecs441", PeerNameType.Unsecured);
            PeerNameRegistration registration = new PeerNameRegistration();

            registration.PeerName = name;
            registration.Port = 6011;
            registration.Cloud = Cloud.Global;
            registration.Comment = SettingsManager.getComment();
        }

        public static void ResolveUsers()
        {
            PeerNameResolver resolver = new PeerNameResolver();
            PeerName name = new PeerName("eecs441");

            PeerNameRecordCollection results = resolver.Resolve(name);

            //do some stuff

        }





    }
}
