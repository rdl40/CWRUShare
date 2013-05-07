using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace CWRUShare
{
    [Serializable()]
    public class UserList
    {
        protected Dictionary<string, DateTime> Users;
        private List<String> _activePeers;

        public UserList()
        {
            Users = new Dictionary<string, DateTime>();
            _activePeers = new List<string>();
        }

        public void AddUser(string ipAddress)
        {
            Users.Add(ipAddress, DateTime.Now.AddMinutes(5));
        }

        public void UpdateUser(string ipAddress)
        {
            if (Users.ContainsKey(ipAddress))
            {
                Users[ipAddress] = DateTime.Now.AddMinutes(5);
            }
        }

        public void MergeUserList(UserList other)
        {
            foreach (var dateTime in other.Users)
            {
                if (Users.ContainsKey(dateTime.Key))
                {
                    Users[dateTime.Key] = dateTime.Value;
                }
            }
        }

        private void PopulateActivePeers()
        {
            _activePeers = new List<string>();

            foreach (var user in Users)
            {
                if (DateTime.Compare(user.Value, DateTime.Now) > 0)
                {
                    _activePeers.Add(user.Key);
                }
            }
        }

        public List<IPEndPoint> GetActivePeers()
        {
            List<IPEndPoint> toBeReturned = new List<IPEndPoint>();

            foreach (var address in _activePeers)
            {
                toBeReturned.Add(new IPEndPoint(IPAddress.Parse(address), 14242));
            }

            return toBeReturned;
        }

        public IPEndPoint GetRandomPeer()
        {
            PopulateActivePeers();

            if (_activePeers.Count >= 1)
            {
                return new IPEndPoint((new Random().Next(_activePeers.Count)), 14242);
            }
            else
            {
                return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14242);
            }
        }

        public byte[] ToByteArray()
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryForm.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static UserList FromByteArray(byte[] data)
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(data, 0, data.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (UserList)binaryForm.Deserialize(memoryStream);
        }

    }
}