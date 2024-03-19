using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Multi_Client_Project
{


    internal class Server
    {

        private TcpListener serverSocket;
        String receive;

        IPAddress localAddr;

        int port = 15000;
        private static int clientID = 1;
        private static int roomID = 1;
        private static int gameID = 1;

        CancellationTokenSource tokenSource2;

        readonly object _lock = new object();

        List<ClientHandler> clientRef = new List<ClientHandler>();
        List<Room> rooms = new List<Room>();

        static   Dictionary<int, ClientHandler> list_clients = new Dictionary<int, ClientHandler>();

        static  Dictionary<int, Room> list_rooms = new Dictionary<int, Room>();

        public static   Dictionary<int, Game> list_games = new Dictionary<int, Game>();

        public delegate void StringEventHandler(string message , ClientHandler client);
        public event StringEventHandler SendResponse;

        private readonly object lockObject = new object();



        public delegate void receivedCharHandler(string receivedChar ,int roomId);
        public static  event receivedCharHandler ReceivedChar;



        public Server() {
            localAddr = IPAddress.Parse("127.0.0.1");
            serverSocket = new TcpListener(localAddr, port);
            serverSocket.Start();
            serverSocket.BeginAcceptSocket(OnClientConnected, null);
            this.SendResponse += handleResponseFromClient;

        }

        private void handleResponseFromClient(string message , ClientHandler client)
        {
            string[] data = message.Split(';'); 

            if (data[0] == Utilities.LOGINREQUEST)
            {
                string info = $"{Utilities.LOGINSUCESS};{client.id}";  
                client.sendMessage(info);
                client.clientName = data[1] ;

            }
            else if (data[0] == Utilities.JOINROOM)
            {
                client.sendMessage(Utilities.ROOMJOINEDSUCESSFULLY+";"+prepareRooms(rooms));
                updateAllRoomsStates();  
            } 
            else if (data[0] ==Utilities.CONFIGUREROOM)
            {
                string[] roomData =   data[1].Split(",");
                Room room = new Room( client , roomData[0], roomID, roomData[1]);
                lock (_lock)
                {
                    list_rooms.Add(roomID , room);
                    rooms.Add(room);
                    roomID++; 
                }
                updateRoomsView();
                if (rooms.Count > 0)
                {
                    client.sendMessage(Utilities.ROOMCREATEDSUCESSFULLY+";"+prepareRooms(rooms));

                }
            }
            else if (data[0]==Utilities.DELETEROOM)
            {
                string deletedId =  data[1];
                deleteRoomFromRoomList(deletedId);
                client.sendMessage(Utilities.ROOMDELETEDSUCESSFULLY+";");
            }
            else if (data[0]==Utilities.REQUESTTOJOINROOM)
            {   
                string[] response = data[1].Split(",");
                sendRequestToPlayer1(response );
            }
            else if (data[0] == Utilities.STARTGAME)
            {
                string [] player2Name_RoomId_player1_player2 = data[1].Split(',');
                int roomId = int.Parse(player2Name_RoomId_player1_player2[1]);
                int player2Id = int.Parse(player2Name_RoomId_player1_player2[3]);
                int player1Id = int.Parse(player2Name_RoomId_player1_player2[2]);

                list_rooms[roomId].player2 = list_clients[player2Id];

                Room startedRoom = list_rooms[roomId];

                Game game = new Game(startedRoom);

                lock (_lock)
                {
                    list_games.Add(roomId , game);
                    updateWatcherRoomsView();
                }
                startGameForTwoPlayers(roomId ,game);
            }
            else if (data[0] == Utilities.GUESSEDCHAR)
            {
                string[] playerId_RoomId_Char = data[1].Split(',');
                int playerId = int.Parse( playerId_RoomId_Char[0]);
                int roomId = int.Parse(playerId_RoomId_Char[1]);
                string receivedChar = playerId_RoomId_Char[2];
                string playerName = list_clients[playerId].clientName;
                Game game =  list_games[roomId];
                game.receivedCharHandler(receivedChar , playerId , playerName); 
            }
            else if (data[0] == Utilities.ACCEPTREQUESTTOPLAYAGAIN)
            {
                string[] roomId_playerId = data[1].Split(',');
                int roomId = int.Parse(roomId_playerId[0]);
                int playerId = int.Parse(roomId_playerId[1]); 

                Game game = list_games[roomId]; 
                if(game.room.player1.id == playerId )
                {
                    game.player1Accepted = true;
                }
                else
                {
                    game.player2Accepted = true;
                }

                game.resetGuiToPlayers();

            }

            else if (data[0] == Utilities.REFUSEREQUESTTOPLAYAGAIN)
            {
                string[] roomId_playerId = data[1].Split(',');
                int roomId = int.Parse(roomId_playerId[0]);
                int playerId = int.Parse(roomId_playerId[1]);

                Game game = list_games[roomId];
                if (game.room.player1.id == playerId)
                {
                    game.player1Accepted = false;
                }
                else
                {
                    game.player2Accepted = false;
                    
                }
         
                if(game.player1Accepted !=null && game.player2Accepted != null)
                {
                    updatedRemovedItems(roomId);
                }

                game.resetGuiToPlayers();
                roomID--;
            }

            else if (data[0] == Utilities.WATCHRUNNINGROOMSREQUEST)
            {
                client.sendMessage(Utilities.WATCHRUNNINGROOMSRESPONSE + ";" + prepareRunningRooms(list_games));
            }

            else if (data[0] == Utilities.WATCHROOMREQUEST)
            {
                string [] response = data[1].Split(",");
                int roomId = int.Parse(response[0]);
                int watcherId = int.Parse(response[1]);

                ClientHandler watcherClient = list_clients[watcherId];
                if (watcherClient != null)
                {
                    Watcher watcher = new Watcher(watcherClient.socket);
                    list_games[roomId].watchers.Add(watcher);
                }
                var wordArray = list_games[roomId].GetWordArrayToStartGame(); 
                string request = $"{Utilities.WATCHROOMRESPOSE};{roomId},{wordArray}";
                watcherClient.sendMessage(request); 
            }
            else if (data[0] == Utilities.STOPWATCHING)
            {
                string[] response = data[1].Split(","); 
                int roomId = int.Parse(response[0]);
                int clientId = int.Parse(response[1]);
                if(list_games.Count > 0)
                {
                   var game =  list_games[roomId];
                    game.watchers.ForEach(watcher =>
                    {
                        if(watcher.id == clientId)
                        {
                           game.watchers.Remove(watcher);
                        }
                    });
                }
            }
            else if (data[0] == Utilities.LEAVE)
            {
                string[] response = data[1].Split(","); 
                int roomId = int.Parse(response[0]);
                int playerId = int.Parse(response[1]);
                Game game = list_games[roomId];
                Room room = list_games[roomId].room; 
                room.player1.sendMessage(Utilities.CLOSEFORMPLAYER);
                room.player2.sendMessage(Utilities.CLOSEFORMPLAYER);
                room.player1 = null; 
                room.player2 = null;
                game.player2Accepted = null;
                game.player1Accepted = null;

                updatedRemovedItems(roomId);

                updateRoomsView();
            }
        }

        private void updatedRemovedItems(int roomId)
        {
            lock (_lock)
            {
                if (list_games.ContainsKey(roomId))
                {
                    list_games.Remove(roomId);
                    updateWatcherRoomsView();
                }
                else
                {
                    MessageBox.Show("  list games doesnot have this key ");
                }

                if (list_rooms.ContainsKey(roomId))
                {
                    rooms.Remove(list_rooms[roomId]);
                    list_rooms.Remove(roomId);
                    updateRoomsView();
                }
                else
                {
                    MessageBox.Show("list rooms does not have this key ");
                }
            }
        }

        private void updateAllRoomsStates()
        {
            string runningRoomsIDS = "";

            if (list_games.Count > 0)
            {
                foreach (var game in list_games.Values)
                {
                    runningRoomsIDS += game.room.roomId.ToString();
                    runningRoomsIDS += ":";
                }
                string request = $"{Utilities.UPDATEALLROOMSCOMPLETED};{runningRoomsIDS}";
                sendToAllClients(request);
            }
        }

        private string prepareRunningRooms(Dictionary<int, Game> list_games)
        {
            string ans = ""; 
            if(list_games.Count > 0) {
                foreach(Game game in list_games.Values) {
                    ans += game.ToString(); 
                }
                return ans;
            }
            return "";
        }


        private void startGameForTwoPlayers(int roomId, Game game)
        {
            string playGameRequest = $"{Utilities.PLAYGAME};{roomId},{game.GetWordArrayToStartGame()}";
            string playGameRequestInRoomViewShow = $"{Utilities.PLAYGAMEINROOMVIEWSHOW};{roomId},{game.GetWordArrayToStartGame()}";

            list_rooms[roomId].player1.sendMessage(playGameRequest);
            list_rooms[roomId].player2.sendMessage(playGameRequestInRoomViewShow);
            list_rooms[roomId].player2.sendMessage(Utilities.HIDEBUTTONSFORPLAYER2);

            updateRoomStates(roomId);

        }

        private void updateRoomStates(int roomId)
        {
            string request = $"{Utilities.ROOMCOMPLETED};{roomId}";
            sendToAllClients(request);
        }

        private void sendToAllClients(string request)
        {
            if(list_clients.Count>0)
            {
                foreach (var client in list_clients.Values)
                {
                    client.sendMessage(request);
                }
            }
        }



        private void sendRequestToPlayer1(string[] response )
        {
            string roomId = response[0];
            string player2Id = response[1];
            string Player2Name = list_clients[int.Parse(player2Id)].clientName;

            if (list_rooms.ContainsKey(int.Parse(roomId)))
            {
                int parseRoomId = int.Parse(roomId);
                list_rooms[parseRoomId].player1.sendMessage($"{Utilities.PLAYER2REQUESTTOJOIN};{Player2Name},{roomId},{player2Id}");
            }
            else
            {
            }

        }

        private void deleteRoomFromRoomList(string deletedId)
        {
            lock (lockObject)
            {
                foreach(Room room in list_rooms.Values) {
                    if (room.player1.id.ToString() == deletedId)
                    {
                        list_rooms.Remove(int.Parse(deletedId));
                        rooms.Remove(room);
                    }
                }

                updateRoomsView();
            }
        }

        private void updateRoomsView()
        {
            var request = $"{Utilities.UPDATEROOMSVIEW};{prepareRooms(rooms)}"; 

            sendToAllClients(request);
        }

        private void updateWatcherRoomsView()
        {
            var request = $"{Utilities.UPDATEWATCHERROOMSVIEW};{prepareRunningRooms(list_games)}";

            sendToAllClients(request);
        }
        private string prepareRooms(List<Room> rooms)
        {   string ans = string.Empty;
            if(rooms.Count > 0) {

                foreach (var room in rooms)
                {
                    ans += room.ToString();
                }
                return ans;
            }
            return "";
        }

        void OnClientConnected(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAcceptSocket(ar);

                ClientHandler clientHand = new ClientHandler(clientSocket);
                
                Task.Run(() => readFromClient(clientHand));

                clientRef.Add(clientHand);

                clientHand.id = clientID;
                lock (_lock)
                {
                    list_clients.Add(clientID, clientHand);
                }
                clientID++; 
                serverSocket.BeginAcceptSocket(OnClientConnected, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Socket exception: " + ex.Message);
            }
        }

        void readFromClient(ClientHandler clientHand)
        {
            try
            {
                while (clientHand.socket.Connected)
                {
                    try
                    {
                        receive = clientHand.receiveMessage();
                        SendResponse(receive , clientHand); 
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }catch(Exception ex)
            {
                
            }

        }
    }


}



