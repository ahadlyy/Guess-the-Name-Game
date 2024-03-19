


using System.Diagnostics;


namespace Client_Project
{
    public partial class GameForm : Form
    {
        Client client;
        int roomId;

        string wordArray = "";

        internal GameForm(Client client, int roomId, string wordArray)
        {
            InitializeComponent();
            this.client = client;
            this.wordArray = wordArray;
            this.roomId = roomId;
            client.SendResponse += handleResponseEvent;
        }



        private void handleResponseEvent(string message)
        {
            string[] response = message.Split(';');
            if (IsHandleCreated)
            {
                if (response[0] == Utilities.CONTINUEPLAYING)
                {
                    // player choices
                    string[] result = response[1].Split(",");
                    string wordArray = result[0];
                    string playerName = result[1];
                    Invoke(() => handleForm(wordArray, playerName));
                }
                else if (response[0] == Utilities.SWITCHPLAYING)
                {
                    string[] result = response[1].Split(",");
                    string wordArray = result[0];
                    string playerName = result[1];
                    Invoke(() => handleFormSwitch(wordArray, playerName));
                }
                else if (response[0] == Utilities.DIMMBUTTON)
                {
                    string clickedBtn = response[1];
                    Invoke(() => disableBtnClicked(clickedBtn));
                }
                else if (response[0] == Utilities.WINNINGMESSAGE)
                {
                    string winnerMessage = "Winner Winner Chicken Dinner";
                    Invoke(() => requestToPlayAgainForm(winnerMessage, client, roomId ));

                    Debug.WriteLine(winnerMessage + " room id === " + roomId);
                }
                else if (response[0] == Utilities.LOSEMESSAGE)
                {
                    string loserMessage = "You lose, hard luck";
                    Invoke(() => requestToPlayAgainForm(loserMessage, client, roomId ));
                }
                else if (response[0] == Utilities.RESETGUITOPLAYAGAIN)
                {
                    Invoke(() => resetGUI());

                }
                else if (response[0] == Utilities.WAITINGFORMLAYER)
                {
                    Invoke(() => showWaitingForm());
                }
                else if (response[0] == Utilities.CLOSEFORMPLAYER)
                {
                    Invoke(() => closeGameForm());
                }
                else if (response[0] == Utilities.HIDEBUTTONSFORPLAYER2)
                {
                    Invoke(() => hideBtns());
                }
            }

        }

        private void showWaitingForm()
        {
            WaitingForm waiting = new WaitingForm(client);
            waiting.Show();
            closeGameForm();
        }


        private void closeGameForm()
        {
            this.Close();
        }


        private void resetGUI()
        {
            showBtns();
            resetButtonStates();
        }

        private void requestToPlayAgainForm(string message, Client client, int roomId )
        {
            RequestToPlayAgainForm winner = new RequestToPlayAgainForm(message, client, roomId);
            winner.ShowDialog();
        }

        private void handleFormSwitch(string playersChoice, string playerName)
        {
            label1.Text = playersChoice;
            playerLabel2.Text = playerName + "'s Turn";
            playerLabel2.Visible = true;
            playerLabel1.Visible = false;
            hideBtns();
        }


        private void handleForm(string playersChoice, string playerName)
        {
            label1.Text = playersChoice;
            playerLabel1.Text = playerName + "'s Turn";
            playerLabel1.Visible = true;
            playerLabel2.Visible = false;
            showBtns();
        }


        private void hideBtns()
        {
            foreach (Control control in this.Controls)
            {
                if ((control is Button)&& (control as Button).Text.ToString() != "Leave")
                {
                    control.Hide();
                }
            }
        }

        private void showBtns()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Show();
                }
            }
        }

        private void disableBtnClicked(string clickedBtn)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    if (clickedBtn == control.Text)
                    {
                        control.Enabled = false;
                    }
                }
            }
        }

        private void resetButtonStates()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Enabled = true;
                }
            }
            this.label1.Text = "new Word";

        }
        private void LetterButton_Click(object? sender, EventArgs e)
        {
            Button button = sender as Button;
            string guessedLetter = button.Text.Trim(); // Get the letter from the button's text
            string guessedLetterRequest = $"{Utilities.GUESSEDCHAR};{client.clientId},{roomId},{guessedLetter}";
            client.sendData(guessedLetterRequest);
        }


        private void GameForm_Load(object sender, EventArgs e)
        {
            
            if (IsHandleCreated)
            {
                Invoke(() => label1.Text = wordArray);
            }
        }

        private void LeaveBtn_Click(object sender, EventArgs e)
        {
            var request = $"{Utilities.LEAVE};{roomId},{client.clientId}"; 
            client.sendData(request);
        }
    }
}
