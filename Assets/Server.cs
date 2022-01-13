using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;

namespace ServerStuff
{
    public class Server
    {
        static TcpClient client;
        static byte[] data;
        static string recieved;
        static int id;
        public static string name;
        static int GameID;
        public static UnityEvent<int> introStart = new UnityEvent<int>();
        public static void RunCommand(string fullcommand)
        {
            int command = int.Parse(fullcommand[2].ToString());
            string[] args = fullcommand.Substring(3).Split(',');
            switch (command)
            {
                case 3: // rejected
                    {
                        Debug.Log("Ok something was rejected, now what was it");
                        if (args[0] == "name")
                        {
                            Debug.Log("Name is not actually, " + name);
                            name = null;
                        }
                    }break;
                case 4: //accepted
                    {
                        Debug.Log("Ok something was accepted, now what was it");
                        if (args[0] == "name")
                        {
                            Debug.Log("Name is now, " + name);
                            Debug.Log(name);
                        }
                        if (args[0] == "id")
                        {
                            id = int.Parse(args[1]);
                            Debug.Log("id is now, " + id);
                            Debug.Log(id.ToString());
                        }
                    }
                    break;
                case 5: //StartGame
                    {
                        if(args[0] == "Trivas&Tribulations")
                        {
                            GameID = 1;
                        }
                    }break;
                case 6: //Game Actions
                    {
                        switch(args[0])
                        {
                            //Generics
                            case "intro": { introStart.Invoke(GameID); }break;
                            default: break;
                        }
                    }break;
                default: break;
            }
        }

        public static string GetArg(string command, int index)
        {
            command = command.Substring(3);
            string[] args = command.Split(',');
            return args[index];
        }
        public static bool isConnected()
        {
            return client.Connected;
        }
        public static void ListenToServer() //client is good friend, be like client
        {
            byte[] bytes = new byte[256];
            while (true)
            {
                NetworkStream stream = client.GetStream();
                int i;
                try
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        string sdata = null;
                        // Translate data bytes to a ASCII string.
                        sdata = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        if (sdata.Substring(0, 2) == "*&")
                        {
                            RunCommand(sdata);
                        }
                        else
                        {
                            Debug.Log(sdata);
                        }
                    }
                }catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        public static void SendMessageToServer(string message)
        {
            NetworkStream stream = client.GetStream();
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("sent: " + message + " to server");
        }

        public static void DisconnectFromServer()
        {
            client.Close();
        }

        public static bool ConnectToServer(string ip, Int32 port)
        {
            try
            {
                client = new TcpClient(ip, port);
                SendMessageToServer("*&0");
                Task.Run(() => ListenToServer());
            }
            catch
            {
                return false;
            }
            return true;

        }
    }
}

