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
    public class DirectoryItem
    {
        public DirectoryItem()
        {
            Name = "";
            Size = 0;
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public int Size { get; set; }

        public int Icon { get; set; }


        //public DirectoryItem(SerializationInfo info, StreamingContext context)
        //{
        //    Name = (string) info.GetValue("Name", typeof (string));
        //    Size = (int) info.GetValue("Size", typeof (int));
        //    Icon = (int) info.GetValue("Icon", typeof (int));
        //}

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Name", Name);
        //    info.AddValue("Size", Size);
        //    info.AddValue("Icon", Icon);
        //}
    }

}
