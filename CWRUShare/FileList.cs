using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            var directoryInfo = new DirectoryInfo(directory);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                files.Add(new File(Guid.NewGuid(), fileInfo.Name, fileInfo.FullName));
            }
        }

        public string GetFilePathFromGuid(Guid id)
        {
            foreach (var file in files)
            {
                if (file.ID.CompareTo(id) == 0)
                {
                    return file.Path;
                }
            }

            return "";
        }

        public List<File> GetFileList()
        {
            return files;
        }

        public byte[] ToByteArray()
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryForm.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static FileList FromByteArray(byte[] data)
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(data, 0, data.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (FileList)binaryForm.Deserialize(memoryStream);
        }
        
    }

    [Serializable()]
    public class File
    {

        public File(Guid id, string name, string path)
        {
            ID = id;
            Name = name;
            Path = path;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }

        [IgnoreDataMember]
        public string Path { get; set; }
 
    }





}
