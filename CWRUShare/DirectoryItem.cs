using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CWRUShare
{
    [Serializable()]
    public class DirectoryItem : ISerializable
    {

        private string name;
        private int size;
        private int icon;

        public DirectoryItem()
        {
            name = "";
            size = 0;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }


        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }

        }

        public int Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
            }
        }


        public DirectoryItem(SerializationInfo info, StreamingContext context)
        {
            name = (string) info.GetValue("Name", typeof (string));
            size = (int) info.GetValue("Size", typeof (int));
            icon = (int) info.GetValue("Icon", typeof (int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", name);
            info.AddValue("Size", size);
            info.AddValue("Icon", icon);
        }
    }

}
