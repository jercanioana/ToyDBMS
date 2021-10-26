using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Utils
{
    public abstract class IObserver
    {
        protected TcpClient tcpClient;

        public void Write(string Message)
        {
            var stream = tcpClient.GetStream();
            var toSend = Encoding.ASCII.GetBytes(Message.Trim() + TCPConfigs.Delimiter);
            stream.Write(toSend, 0, toSend.Length);
            stream.Flush();
        }

        public string Read()
        {
            var stream = tcpClient.GetStream();
            var toReceive = new byte[TCPConfigs.MessageLength];
            stream.Read(toReceive, 0, tcpClient.ReceiveBufferSize);
            return Encoding.ASCII.GetString(toReceive).Split(TCPConfigs.Delimiter)[0];
        }

    }

    public abstract class Observable
    {
        private List<IObserver> observers = new List<IObserver>();

        public void Subscribe(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            observers.Remove(observer);
        }


    }

    public static class TCPConfigs
    {
        public const string IP = "127.0.0.1";
        public const int Port = 8888;
        public const int MessageLength = 64 * 1024;
        public const char Delimiter = '$';
    }
}

