using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace GameServer
{
    class ServerProgram
    {
        static Int32 port = 13000;
        static IPAddress localaddr = IPAddress.Parse("0.0.0.0");
        static TcpListener server = new TcpListener(localaddr, port);
        static void Main()
        {
            Console.WriteLine("Server Starting");
            server.Start();

            byte[] bytes = new byte[256];
            String data = null;
            
            while(true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    if(data.Substring(0,2) == "*&")
                    {
                        if(!RunCommand(data))
                        {
                            break;
                        }
                    }
                    // Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }
            }
        }

        public static bool RunCommand(string fullcommand)
        {
            int command = int.Parse(fullcommand.Substring(2, 3));
            switch (command)
            {
                case 1:
                {
                    Console.Write("Message Recieved From Client");
                    return false;
                }
                default:break;
            }
            return true;
        }
    }
}
