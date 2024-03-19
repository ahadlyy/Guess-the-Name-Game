using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Server_Multi_Client_Project
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Server_FormClosing;

            Server server = new Server();

        }


        private void startBtn_Click(object sender, EventArgs e)
        {

        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Environment.Exit(Environment.ExitCode);
        }

        private void Server_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Environment.Exit(Environment.ExitCode);
        }

        private void showDataBtn_Click(object sender, EventArgs e)
        {
            var data = Server.list_games.Values;
            if(data.Count > 0 && data!=null)
            {
                foreach(var game in data) {
                    
                    string roomID = game.room.roomId.ToString();
                    string roomCtegory = game.room.roomCategory.ToString();
                    string player1ID = game.room.player1.id.ToString();
                    string player2ID = game.room.player2.id.ToString();
                    string player2NAME = game.room.player2.clientName;
                    string player1NAME = game.room.player1.clientName;

                    string roomData = $"{player1NAME}, ID: {player1ID} VS {player2NAME}, ID: {player2ID} [room # {roomID}, category is: {roomCtegory}]"; 

                    listBox1.Items.Add(roomData);
                
                }

            }

        }



    }
}
