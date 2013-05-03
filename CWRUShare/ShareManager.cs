using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CWRUShare
{
    [Serializable()]
    public class ShareManager : ISerializable
    {

        private Dictionary<int, System.Drawing.Icon> icons;
        private Dictionary<int, DirectoryItem> data;
        private string instructions;
        private int nameholder;

        public ShareManager()
        {
            icons = new Dictionary<int, Icon>();
            data = new Dictionary<int, DirectoryItem>();
            instructions = "";
            nameholder = 0;
        }

        public void addIcon(int index, System.Drawing.Icon icon)
        {
            if (!icons.ContainsKey(index))
            {
                icons.Add(index, icon);
            }
        }

        public void addData(int index, DirectoryItem item)
        {
            data.Add(index, item);
        }

        public void addInstructions(string instruction)
        {
            instructions += instruction + "\n";
        }

        public string getInstructions()
        {
            return instructions;
        }

        public void resetNameHolder()
        {
            nameholder = 0;
        }

        public int getNextNameHolder()
        {
            Console.WriteLine(nameholder);
            return nameholder++;
        }

        public DirectoryItem getDirectoryItemFromNameHolder(int nameholder)
        {
            return data[nameholder];
        }

        public System.Drawing.Icon getDirectoryItemIcon(int index)
        {
            return icons[index];
        }

        public ShareManager(SerializationInfo info, StreamingContext context)
        {
            icons = (Dictionary<int, System.Drawing.Icon>) info.GetValue("Icons", typeof (Dictionary<int, System.Drawing.Icon>));
            data = (Dictionary<int, DirectoryItem>) info.GetValue("Data", typeof(Dictionary<int, DirectoryItem>));
            instructions = (string) info.GetValue("Instructions", typeof (string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Icons", icons);
            info.AddValue("Data", data);
            info.AddValue("Instructions", instructions);
        }
    }
}
