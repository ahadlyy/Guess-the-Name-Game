using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server_Multi_Client_Project
{


    class ClientHandler 
    {
        public string clientName { get; set; }
        public  int id { get; set; }
        public Socket socket;

        public NetworkStream clientStream;
        public BinaryReader clientReader;
        public BinaryWriter clientWriter ;



        public ClientHandler( Socket socket  ) { 
            this.socket  = socket;
            clientStream = new NetworkStream(socket);
            clientReader = new BinaryReader(clientStream);
            clientWriter = new BinaryWriter(clientStream);
        }

    

        public void sendMessage(string message) {
            Debug.WriteLine($"iam in send message and message {message}");
            clientWriter.Write(message);
        }

        public String receiveMessage() {
            string receivedMessage = clientReader.ReadString();
            Debug.WriteLine($"iam in receive message {receivedMessage} ");
            return receivedMessage;
        }  

    }
}
