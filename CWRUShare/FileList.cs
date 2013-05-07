using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CWRUShare
{
    [Serializable()]
    public class FileList
    {
        private List<File> files;

        public FileList()
        {
            files = new List<File>();
        }

        public void PopulateFileList(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);}





    }

    [Serializable()]
    public class File
    {
        Guid ID { get; set; }
        string Name { get; set; }

        [IgnoreDataMember]
        string Path { get; set; }
 
    }


}
