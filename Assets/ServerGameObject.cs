using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerStuff;
using System.IO;
using TMPro;

public class ServerGameObject : MonoBehaviour
{
    string path = "./config.txt";
    string ip;

    // Start is called before the first frame update
    void Start()
    {
        if(!File.Exists(path))
        {
            File.Create(path);
            File.WriteAllText(path, "192.168.56.60");
        }
        ip = File.ReadAllText(path);
        bool isServerConnected = Server.ConnectToServer(ip, 13000);
        if (!isServerConnected)
        {
            Debug.LogError("Failed To Connect To Server, Idiot forgot to start the server again");
        }
    }

    public void SendServerMesage(string message)
    {
        Server.SendMessageToServer(message);
        if(message.Substring(0, 2) == "*&")
        {
            int command = int.Parse(message[2].ToString());
            string[] args = message.Substring(3).Split(',');
            Debug.Log("Sent Command with args: " + args);
            if (command == 2)
            {
                Server.name = args[0];
            }
        }
    }

    void OnApplicationQuit()
    {
        Server.DisconnectFromServer();
    }


    public enum PrefixType
    {
        Player,
        ConsoleLog,
        PlayerExecuteCommand,
        NoPrefix
    }

    // Update is called once per frame
    void Update()
    {

    }
}
