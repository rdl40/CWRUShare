using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace CWRUShare
{
    [Serializable()]
    public class UserList
    {
        protected Dictionary<string, DateTime> users;
        private List<String> activePeers;

        public void AddUser(string ipAddress)
        {
            users.Add(ipAddress, DateTime.Now.AddMinutes(5));
        }

        public void UpdateUser(string ipAddress)
        {
            if (users.ContainsKey(ipAddress))
            {
                users[ipAddress] = DateTime.Now.AddMinutes(5);
            }
        }

        public void MergeUserList(UserList other)
        {
            foreach (var dateTime in other.users)
            {
                if (users.ContainsKey(dateTime.Key))
                {
                    users[dateTime.Key] = dateTime.Value;
                }
            }
        }

        private void PopulateActivePeers()
        {
            activePeers = new List<string>();

            foreach (var user in users)
            {
                if (DateTime.Compare(user.Value, DateTime.Now) > 0)
                {
                    activePeers.Add(user.Key);
                }
            }
        }

        public IPEndPoint GetActivePeer()
        {
            PopulateActivePeers();

            if (activePeers.Count >= 1)
            {
                return new IPEndPoint((new Random().Next(activePeers.Count)), 14242);
            }
            else
            {
                return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14242);
            }
        }

    }
}