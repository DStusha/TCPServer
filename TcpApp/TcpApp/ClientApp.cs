using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpApp
{
    class ClientApp
    {
        const int port = 8888;
        //задаем IP адрес компьютера, на котором запущен сервер
        const string address = "127.0.0.1";
        private static NetworkStream stream;
        private static TcpClient client;

        static void Main(string[] args)
        {
            try
            {
                client = new TcpClient(address, port);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Thread clientThread = new Thread(new ThreadStart(StartClient));
            clientThread.Start();
            Thread listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();
        }

        static void StartClient()
        {
            try
            {
                int k=0;
                Console.WriteLine("Введите имя");
                Send(Console.ReadLine());
                Thread.Sleep(100);
                while (k!=3)
                {
                    PrintMenu();
                    string message = Console.ReadLine();
                    k = Convert.ToInt32(message);
                    Send(message);
                    switch (k)
                    {
                        case 1:
                            Console.WriteLine("Выберите получателя");
                            message = Console.ReadLine();
                            Console.WriteLine("Введите сообщение");
                            message = message + " " + Console.ReadLine();
                            Send(message);
                            break;
                        case 2:
                            break;
                        case 3:
                            stream = null;
                            Console.WriteLine("До свидания!");                            
                            Console.ReadLine();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("1 - отправить письмо");
            Console.WriteLine("2 - просмотреть письма");
            Console.WriteLine("3 - отключиться");
        }

        static private void Listen()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            while (stream!=null && client!=null)
            {
                builder.Clear();
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream!=null && stream.DataAvailable);
                if(stream!=null)Console.WriteLine("Сервер: {0}", builder.ToString());
            }
        }

        static private void Send(String message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
