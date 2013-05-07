using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CWRUShare
{

    enum Message {DiscoveryReply, Ping, PingReply, RequestUserList, RecieveUserList, RequestFileList, RecieveFileList, RequestFiles, RecieveFile, Leaving}

    [Serializable()]
    internal class Messages
    {
        public Message MessageType { get; set; }

        public object Data { get; set; }

        public byte[] ToByteArray()
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryForm.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static Messages FromByteArray(byte[] data)
        {
            Console.WriteLine(data.Length);
            BinaryFormatter binaryForm = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(data, 0, data.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return ((Messages) binaryForm.Deserialize(memoryStream));
        }
    }
}
