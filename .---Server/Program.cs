using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace GameServer
{
    class Player
    {
        public TcpClient tcpClient;
        public NetworkStream stream;
        public Player(TcpClient client, NetworkStream steam, int identifier)
        {
            id = identifier;
            stream = steam;
            tcpClient = client;
        }
        public int id;

        public async void ManageRequests()
        {
            byte[] bytes = new byte[256];
            string data = null;
            bool connect = true;
            while(connect)
            {
                Console.WriteLine("Connected!");
                int i;
                string userinput = Console.ReadLine();
                if (userinput != null)
                {
                    Console.WriteLine(Console.ReadLine());
                }
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    if (data.Substring(0, 2) == "*&")
                    {
                        RunCommand(data);
                    }
                    else
                    {
                        Console.WriteLine("Received: {0}", data);
                    }
                    if(!tcpClient.Connected)
                    {
                        ServerProgram.playerList.Remove(this);
                        connect = false;
                    }
                }
            }
            
        }
        public void sendClientMessage(string msg)
        {
            byte[] message = System.Text.Encoding.ASCII.GetBytes(msg);
            this.stream.Write(message);
        }

        public void RunCommand(string fullcommand)
        {
            int command = int.Parse(fullcommand[2].ToString());
            switch (command)
            {
                case 0:
                    {
                        Console.Write("Client Requested ID");
                        sendClientMessage(id.ToString());
                    }
                    break;
                case 1:
                    {
                        Console.Write("Message Recieved From Client");
                    }break;
                    case 2:
                    {
                        Console.Write("echoing: " + fullcommand.Substring(4));
                    }break;
                default: break;
            }
        }
    }
    class ServerProgram //SERVER
    {
        static Int32 port = 13000;
        static IPAddress localaddr = IPAddress.Parse("0.0.0.0");
        static TcpListener server = new TcpListener(localaddr, port);
        public static List<Player> playerList = new List<Player>();

        static void Main()
        {
            Console.WriteLine("Server Starting");
            server.Start();
            while(true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                playerList.Add(new Player(client, client.GetStream(), playerList.Count + 1));
            }
        }
        
    }
}
