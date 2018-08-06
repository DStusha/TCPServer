using System;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;

namespace TcpServer
{
    public class ServerClient
    {
        public TcpClient MyClient { get; set; }
        public String Name { get; set; }
        public List<Message> Messages { get; set; }
        public IPEndPoint Point { get; set; }

        public ServerClient(TcpClient tcpClient)
        {
            MyClient = tcpClient;
            Point = (IPEndPoint)MyClient.Client.RemoteEndPoint;
        }

        public void Process()
        {
            NetworkStream stream = null;
            Messages = new List<Message>();
            try
            {
                stream = MyClient.GetStream();
                Name = Listen(stream);
                Console.WriteLine("Пользователь "+Name+" подключился");
                Send(stream,"Добро пожаловать!");
                int k = 0;
                while (k!=3){
                    k = Convert.ToInt32(Listen(stream));
                    String message = "";
                    switch (k)
                    {
                        case 1:
                            message = "";
                            for (int i=0;i<Server.Clients.Count;i++)
                            {
                                if(Server.Clients[i].Name!=this.Name)message = message + " " + i.ToString() + " " + Server.Clients[i].Name + " " ;
                            }
                            Send(stream, message);
                            SendToClient(Listen(stream));
                            Send(stream,"Сообщение отправлено");
                            break;
                        case 2:
                            message = "";
                            int num = 0;
                            foreach (Message m in Messages)
                            {
                                num++;
                                message = message + num.ToString()+"." + "Отправитель: " + m.Sender.Name + " Сообщение: " +m.Text + " ";
                            }
                            Send(stream, message);
                            break;
                        case 3:
                            Console.WriteLine("Пользователь " + Name + " отключился");
                            Server.Clients.Remove(this); 
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
                if (stream != null)
                    stream.Close();
                if (MyClient != null)
                    MyClient.Close();
            }
        }

        private void SendToClient(String message)
        {
            int index = Convert.ToInt32(message.Substring(0, 1));
            String mess = message.Substring(2);
            Server.Clients[index].Messages.Add(new Message(this, mess));
            Server.Clients[index].Messages[0].Print();
            Send(Server.Clients[index].MyClient.GetStream(), "Вам письмо!");
        }

        private String Listen(NetworkStream stream)
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            return builder.ToString();
        }

        private void Send(NetworkStream stream, String message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
