using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpServer
{
    public class Server
    {
        const int port = 8888;
        static TcpListener listener;
        public static List<ServerClient> Clients { get; set; }

        static void Main(string[] args)
        {
            Clients = new List<ServerClient>();
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ServerClient clientObject = new ServerClient(client);
                    Clients.Add(clientObject);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
