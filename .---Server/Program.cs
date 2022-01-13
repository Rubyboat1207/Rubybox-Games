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
        public string name;
        public Player(TcpClient client, NetworkStream steam, int identifier)
        {
            id = identifier;
            stream = steam;
            tcpClient = client;
        }
        public int id;


        public void ManageRequests()
        {
            byte[] bytes = new byte[256];
            string data = null;
            int i2 = 0;
            while (tcpClient.Connected)
            {
                Console.WriteLine("Connected!");
                int i = 0;
                i2++;
                if (i2 > 1)
                {
                    Console.WriteLine("Player Disconnected. Also this is scuffed");
                    Disconect();
                }
                try
                {
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
                    }
                }
                catch (Exception e)
                {
                    Disconect();
                }
            }
            Disconect();
        }

        public void sendClientMessage(string msg)
        {
            byte[] message = System.Text.Encoding.ASCII.GetBytes(msg);
            stream.Write(message);
            if(name != null)
            {
                Console.WriteLine("Sent Message to " + name + " that says: " + msg);
            }
            else
            {
                Console.WriteLine("Sent Message to " + id + " that says: " + msg);
            }
        }
        public void Disconect()
        {
            Console.WriteLine(id + " has disconnected");
            tcpClient.Close();
            ClientManager.playerList.Remove(this);
        }
        public void RunCommand(string fullcommand)
        {
            int command = int.Parse(fullcommand[2].ToString());
            string[] args = fullcommand.Substring(3).Split(',');
            switch (command)
            {
                case 0:
                    {
                        Console.WriteLine("Client Requested ID");
                        sendClientMessage("*&4id," + id.ToString());
                        Console.WriteLine("Assigned ID of " + id);
                    }
                    break;
                case 1:
                    {

                        Console.WriteLine("Client " + id + " requested name: " + args[0]);
                        bool isvalidname = true;
                        foreach(Player pl in ClientManager.playerList)
                        {
                            if(pl.name == args[0])
                            {
                                Console.WriteLine(pl.name + " is already in use");
                                isvalidname = false;
                                break;
                            }
                        }
                        if (isvalidname)
                        {
                            name = args[0];
                            sendClientMessage("*&3name");
                        }
                        else
                        {
                            Console.WriteLine("Client Name Accepted");
                            sendClientMessage("*&4name");
                        }
                    }break;
                    case 2:
                    {
                        Console.WriteLine("echoing: " + fullcommand.Substring(4));
                    }break;
                    case 3://startgameOverride
                    {
                        try
                        {
                            ClientManager.GameID = int.Parse(args[0]);
                            Console.WriteLine("Beingining game {0}", args[0]);
                        }
                        catch
                        {
                            Console.WriteLine("Client Sent Invalid Game");
                        }
                    }
                    break;
                default: break;
            }
        }
    }
    class ClientManager //SERVER
    {
        static Int32 port = 13000;
        static IPAddress localaddr = IPAddress.Parse("0.0.0.0");
        static TcpListener server = new TcpListener(localaddr, port);
        public static List<Player> playerList = new List<Player>();
        static public int GameID = -1;
        //Games:
        //1 - Trivias and Tribulations
        public static int gameState = 0;
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
                System.Threading.Tasks.Task.Run(() => playerList[playerList.Count - 1].ManageRequests());
                if (GameID == 1)
                {
                    if(gameState == 0)
                    {
                        BroadcastCommand("5Trivas&Tribulations");
                        gameState++;
                    }
                }
            }
        }

        static void BroadcastCommand(string cmd)
        {
            foreach(Player player in playerList)
            {
                player.sendClientMessage("*&" + cmd);
            }
        }
    }
}
