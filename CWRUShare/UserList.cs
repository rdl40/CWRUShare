using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CWRUShare
{
    [Serializable()]
    public class UserList
    {
        protected Dictionary<string, DateTime> users;

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
                    users[dateTime.Key] = DateTime.Now.AddMinutes(5);
                }
                else
                {
                    users.Remove(dateTime.Key);
                }
            }
        }

    }
}