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
    public class ShareManager
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

        public void AddIcon(int index, System.Drawing.Icon icon)
        {
            if (!icons.ContainsKey(index))
            {
                icons.Add(index, icon);
            }
        }

        public void AddData(int index, DirectoryItem item)
        {
            data.Add(index, item);
        }

        public void AddInstructions(string instruction)
        {
            instructions += instruction + "\n";
        }

        public string GetInstructions()
        {
            return instructions;
        }

        public void ResetNameHolder()
        {
            nameholder = 0;
        }

        public int GetNextNameHolder()
        {
            Console.WriteLine(nameholder);
            return nameholder++;
        }

        public DirectoryItem GetDirectoryItemFromNameHolder(int index)
        {
            return data[index];
        }

        public System.Drawing.Icon GetDirectoryItemIcon(int index)
        {
            return icons[index];
        }
    }
}
