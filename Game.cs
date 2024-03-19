
using System.Diagnostics;


namespace Server_Multi_Client_Project
{
    internal class Game
    {


        private static string[] words;

        private string selectedWord="";

        static bool fileExistence = true;

        public bool ? player1Accepted =null  ; 
        public bool ? player2Accepted = null ;

        public char[] wordArray;

        public List<Watcher> watchers = new List<Watcher>(); 


        string realTimeUpdatingWordRightChecker = "";
        string realTimeUpdatingWordWrongChecker = "";


        public Room room ;

        static Game()
        {
            checkFileExistence();
        }


        public Game(Room startedRoom)
        {
            this.room = startedRoom;

            getWordFromFile(room.roomCategory);

            initWordArray();

        }

        private void initWordArray()
        {
            wordArray = new char[selectedWord.Length];
            wordArray = Enumerable.Repeat('-', selectedWord.Length).ToArray();
        }

        public string GetWordArrayToStartGame()
        {
            return prepareWordArray(wordArray);
        }

      


        private void getWordFromFile(string category)
        {
            Debug.WriteLine("i am checking for the file to get the word");
            if (fileExistence)
            {
           
                string[] filteredWords;
                if (category.ToLower() == "random")
                {
                    filteredWords = words;
                }
                else
                {
                    filteredWords = words.Where(word => word.StartsWith(category + ":")).ToArray();
                    if (filteredWords.Length == 0)
                    {
                        MessageBox.Show($"No words found for category '{category}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Select a random word
                Random random = new Random();
                selectedWord = filteredWords[2].Split(':')[1].Trim();
            }
        }

        


        private static  void checkFileExistence()
        {
            // Read words from the file
            string filePath = "D:\\ITI(PD&BI)\\C# .Net\\New folder\\Server_Multi_Client_Project\\bin\\Debug\\net8.0-windows\\words.txt";
            if (File.Exists(filePath))
            {
                words = File.ReadAllLines(filePath);
                if (words.Length == 0)
                {
                    Debug.WriteLine("The file is empty. Please add words to continue.");
                    fileExistence = false;
                    return;
                }

                Debug.WriteLine("words length  = " + words.Length);

            }
            else
            {
                MessageBox.Show("File 'words.txt' not found. Please create the file with words to continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        public void receivedCharHandler(string receivedChar , int playerId , string playerName)
        {
            dimmButton(receivedChar);
            bool letterFound = false;

            if (selectedWord != null)
            {

                for (int i = 0; i < selectedWord.Length ; i++)
                {
                    if (selectedWord[i] == receivedChar[0])
                    {
                        wordArray[i] = receivedChar[0];
                        letterFound = true;
                        if(room.player1.clientName == playerName)
                        {
                            realTimeUpdatingWordRightChecker = $"{Utilities.CONTINUEPLAYING};{prepareWordArray(wordArray)},{room.player1.clientName}";
                            realTimeUpdatingWordWrongChecker = $"{Utilities.SWITCHPLAYING};{prepareWordArray(wordArray)},{room.player1.clientName}";
                        }
                        else
                        {
                            realTimeUpdatingWordRightChecker = $"{Utilities.CONTINUEPLAYING};{prepareWordArray(wordArray)},{playerName}";
                            realTimeUpdatingWordWrongChecker = $"{Utilities.SWITCHPLAYING};{prepareWordArray(wordArray)},{playerName}";
                        }
                        if(watchers.Count> 0) {
                            foreach(var watcher in watchers) {
                                watcher.sendMessage(realTimeUpdatingWordRightChecker); 
                            }
                        }
                    }
                }


                if (letterFound)
                {
                    Debug.WriteLine("playerrr Id = " + playerId + " room player1 id = " + room.player1.id + " room player2 id " + room.player2.id);
                    if (playerId == room.player1.id)
                    {
                        room.player1.sendMessage(realTimeUpdatingWordRightChecker);
                        room.player2.sendMessage(realTimeUpdatingWordWrongChecker);
                    }
                    else
                    {
                        room.player2.sendMessage(realTimeUpdatingWordRightChecker);
                        room.player1.sendMessage(realTimeUpdatingWordWrongChecker);
                    }
                }
                else
                {
                    if (playerId == room.player1.id)
                    {
                        realTimeUpdatingWordRightChecker = $"{Utilities.CONTINUEPLAYING};{prepareWordArray(wordArray)},{room.player2.clientName}";
                        realTimeUpdatingWordWrongChecker = $"{Utilities.SWITCHPLAYING};{prepareWordArray(wordArray)},{room.player2.clientName}";

                        room.player2.sendMessage(realTimeUpdatingWordRightChecker);
                        room.player1.sendMessage(realTimeUpdatingWordWrongChecker);
                    }
                    else
                    {
                        realTimeUpdatingWordRightChecker = $"{Utilities.CONTINUEPLAYING};{prepareWordArray(wordArray)},{room.player1.clientName}";
                        realTimeUpdatingWordWrongChecker = $"{Utilities.SWITCHPLAYING};{prepareWordArray(wordArray)},{room.player1.clientName}";

                        room.player1.sendMessage(realTimeUpdatingWordRightChecker);
                        room.player2.sendMessage(realTimeUpdatingWordWrongChecker);
                    }
                }
                if (!wordArray.Contains('-'))
                {
                    int winner = playerId;
                    endGame(winner);
                }

                 letterFound = false; 
            }

        }


        private void endGame(int winner )
        {
            if (room.player1.id == winner)
            {
                var request = $"{Utilities.WINNINGMESSAGE};,{room.player1.clientName}"; 

                if (watchers.Count > 0)
                {
                    foreach (var watcher in watchers)
                    {
                        watcher.sendMessage(request);
                    }
                }

                SaveGameResult(room.player1.clientName , room.player2.clientName);
                room.player1.sendMessage(Utilities.WINNINGMESSAGE); 
                room.player2.sendMessage(Utilities.LOSEMESSAGE); 

            }
            else
            {
                var request = $"{Utilities.WINNINGMESSAGE};,{room.player2.clientName}";

                if (watchers.Count > 0)
                {
                    foreach (var watcher in watchers)
                    {
                        watcher.sendMessage(request);
                    }
                }

                SaveGameResult(room.player2.clientName, room.player1.clientName);
                room.player2.sendMessage(Utilities.WINNINGMESSAGE);
                room.player1.sendMessage(Utilities.LOSEMESSAGE);
            }

        }

        private void SaveGameResult(string winner , string loser)
        {
            string filePath = "GameResults.txt";
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"Player 1 {winner} , Player 2 {loser} ");
            }
        }
        private string prepareWordArray(char[] wordArray)
        {
            string result = new string( wordArray.ToArray());
            return result; 
        }
        private void dimmButton(string receivedChar)
        {
            string requestToDimmButton = $"{Utilities.DIMMBUTTON};{receivedChar}";
            room.player1.sendMessage(requestToDimmButton);
            room.player2.sendMessage(requestToDimmButton);
        }


        public void resetGuiToPlayers()
        {
            if(player1Accepted!=null && player2Accepted!=null)
            {
                if(player1Accepted==true && player2Accepted==true)
                {
                    
                   initWordArray(); 
                   getWordFromFile(room.roomCategory);
                   room.player1.sendMessage(Utilities.RESETGUITOPLAYAGAIN);
                   room.player2.sendMessage(Utilities.RESETGUITOPLAYAGAIN);
                }
                else
                {
                    room.player1.sendMessage(Utilities.CLOSEFORMPLAYER);
                    room.player2.sendMessage(Utilities.CLOSEFORMPLAYER);
                    room.player2 = null;
                    room.player1 = null;
                }

                player1Accepted = null;
                player2Accepted = null;
            }
        }
        public override string ToString()
        {
            return $"{room.roomId},{room.roomCategory},{room.player1.clientName},{room.player1.id},{room.player2.clientName},{room.player2.id}:";
        }


    }
}
