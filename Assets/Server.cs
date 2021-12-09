using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine.Events;
namespace ServerStuff
{
    public class Server
    {
        static TcpClient client;
        static byte[] data;
        static string recieved;
        public static bool RunCommand(string fullcommand)
        {
            int command = int.Parse(fullcommand.Substring(2, 3));
            switch (command)
            {
                case 1:
                    {
                        Debug.Log("Message Recieved From Server");
                        if (!GameObject.Find(GetArg(fullcommand, 0)))
                        {
                            Debug.LogError("GameObject Sent by Server aint there, you done fucked up bruv");
                            break;
                        }
                        try
                        {
                            float x = float.Parse(GetArg(fullcommand, 1));
                            float y = float.Parse(GetArg(fullcommand, 1));
                            float z = float.Parse(GetArg(fullcommand, 1));
                            GameObject.Find(GetArg(fullcommand, 0)).transform.position = new Vector3(x, y, z);
                        }
                        catch
                        {
                            Debug.LogError("Positions Recived From Server Are messed up");
                        }
                        return false;
                    }
                default: break;
            }
            return true;
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
        public static bool GetServerMessageStatus()
        {
            int i;
            try
            {
                NetworkStream stream = client.GetStream();
                if ((i = stream.Read(data, 0, data.Length)) != 0)
                {
                    recieved = Encoding.ASCII.GetString(data, 0, i);
                    Debug.Log("Received: " + recieved);

                    SendMessageToServer("*&1");
                    Debug.Log("Sent confirmation");
                    return true;
                }
            }
            catch
            {
            }
            
            return false;
        }

        public static void SendMessageToServer(string message)
        {
            NetworkStream stream = client.GetStream();
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("sent: " + message + " to server");
        }

        public static bool ConnectToServer(string ip, Int32 port)
        {
            try
            {
                client = new TcpClient(ip, port);
                SendMessageToServer("*&0");
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

