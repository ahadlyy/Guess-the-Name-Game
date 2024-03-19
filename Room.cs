using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Multi_Client_Project
{
    internal class Room
    {
       

        public  int roomId { get; set; }
        public string roomCategory { get; set; }
        public ClientHandler player1 { get; set; }
        public ClientHandler? player2 { get; set; }
        Random random = new Random();

        public string playerCreatedRoomId { get; set; }

        List<String> categories = new List<string>(){ "SCIENCE" , "SPORT" , "HISTORY"};



        public Room(ClientHandler client , String roomCategory , int roomId , string creatorId   ) 
        {
            player1 = client;
            this.roomCategory = roomCategory;
            this.roomId = roomId ;
            this.playerCreatedRoomId = creatorId;
            
        }

        public override string ToString()
        {
            return  $"{this.roomId},{this.roomCategory},{this.player1.id},{this.player1.clientName}:" ; 
        }

    }
   
}
