using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace DBMS
{
    public class Client : IObserver
    {
        public Client()
        {
            tcpClient = new TcpClient();
        }

        public void Connect()
        {
            tcpClient.Connect(TCPConfigs.IP, TCPConfigs.Port);
        }

        

        public string ReadFromServer()
        {
            var stream = tcpClient.GetStream();
            stream.ReadTimeout = stream.WriteTimeout = 50000;
            var response = "";
            try
            {
                response = Read();

            }
            catch (Exception)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                ReadFromServer();
            }
            return response;
        }
    }
}
