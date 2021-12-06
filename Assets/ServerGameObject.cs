using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerStuff;
using System.IO;

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
            Debug.LogError("Failed To Connect To Server, Not Trying Again Cause You are stupid");
        }
    }

    public void SendServerMesage(string message)
    {
        Server.SendMessageToServer(message);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
