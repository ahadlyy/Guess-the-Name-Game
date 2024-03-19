using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Net.Http;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_Project
{


    public partial class Login : Form
    {
        Client client;

        string response = "";

        public Login()
        {
            InitializeComponent();
        }
        private void loginBtn_Click(object sender, EventArgs e)
        {
            client = new Client();
            client.SendResponse += handleResponseFromServer;
            string userName = useName_textBox.Text.Trim();
            if (userName != null && userName != "")
            {
                string info = $"{Utilities.LOGINREQUEST};{userName}";
                client.clientName = userName;
                var task2 = Task.Run(() => client.receiveData());
                var task1 = Task.Run(() => client.sendData(info));
            }
            else
            {
                MessageBox.Show("Please enter your user name ");
            }
        }
        private void handleResponseFromServer(string response)
        {
            string[] data = response.Split(';');
            if (data[0] == Utilities.LOGINSUCESS)
            {
                client.clientId = data[1];
                Invoke(() => this.Hide());
                Invoke(() => createHome());
            }
        }
        void createHome()
        {
            Home create = new Home(client);
            create.Show();
        }
        public static void CloseConnection()
        {
        }

    }
}
