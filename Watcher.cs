using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Multi_Client_Project
{
    internal class Watcher :ClientHandler
    {

        public Watcher(Socket socket ) : base(socket)
        {
         
        }
    }
}
