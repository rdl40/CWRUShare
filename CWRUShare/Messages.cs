using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWRUShare
{

    enum Message {Ping, RequestUserList, RequestFileList, RequestFiles, Leaving}

    [Serializable()]
    internal class Messages
    {
        public Message MessageType { get; set; }

        public object Data { get; set; }
    }
}
