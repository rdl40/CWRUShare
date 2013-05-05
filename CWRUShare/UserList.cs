using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CWRUShare
{
    [Serializable()]
    static class UserList
    {
        private static List<User> users;

        public static void AddUser(string ipAddress)
        {
            users.Add(new User(ipAddress, DateTime.Now.AddMinutes(5)));
        }

        public static void UpdateUser(string ipAddress)
        {
            
            User user = users.Find(delegate(User e)
                {
                    if (e.IPAddress == ipAddress)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            if (user != null)
            {
                user.Expiration = DateTime.Now.AddMinutes(5);
            }
        }

    }

    [Serializable()]
    class User
    {
        public User(string ipAddress, DateTime expiration)
        {
            this.IPAddress = ipAddress;
            this.Expiration = expiration;
        }

        public string IPAddress { get; set; }

        public DateTime Expiration { get; set; }
    }
}
