using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    public class Message
    {
        public ServerClient Sender { get; set; }
        public String Text { get; set; }

        public Message(ServerClient tcpClient, String message)
        {
            Sender = tcpClient;
            Text = message;
        }

        public void Print()
        {
            Console.WriteLine(Sender.Name);
            Console.WriteLine(Text);
        }
    }
}
