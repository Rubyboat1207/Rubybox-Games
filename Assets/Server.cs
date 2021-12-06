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
    public class Server : MarshalByRefObject
    {
        static TcpClient client;
        static byte[] data;
        static string recieved;
        static UnityEvent<GameObject, Vector3> MoveComponentEvent = new UnityEvent<GameObject, Vector3>();
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void ServerUpdate()
        {
            Debug.Log("Started Running");
            while (true)
            {
                Debug.Log("Running");
            }
        }

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
                            Debug.LogError("GameObject Sent by Server is not Found");
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
                            Debug.LogError("Positions Recived From Server Are Invalid");
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

        static bool GetServerMessageStatus()
        {
            int i;
            NetworkStream stream = client.GetStream();
            if ((i = stream.Read(data, 0, data.Length)) != 0)
            {
                recieved = Encoding.ASCII.GetString(data, 0, i);
                Debug.Log("Received: " + recieved);

                SendMessageToServer("*&Recieved");
                Debug.Log("Sent confirmation");
                return true;
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
            }
            catch
            {
                return false;
            }
            ServerUpdate();
            return true;
        }
    }
}

