using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace UdpChat
{
    class Program
    {
        static string userName; // имя пользователя в чате
        static IPEndPoint from;

        private static bool isRunning = true;
        private static UdpClient udpClient;
        
        static void Main(string[] args)
        {
            Console.WriteLine("What's your name? ");
            userName = Console.ReadLine();
            int PORT = 9876;
            from = new IPEndPoint(0, 0);
            udpClient = new UdpClient {ExclusiveAddressUse = false};
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            Receiver().Start();
            SendFunc().Start();
        }

        private static async Task Receiver()
        {
            while (isRunning)
            {
                var recvBuffer = udpClient.Receive(ref from);
                Console.WriteLine(Encoding.UTF8.GetString(recvBuffer));
            }
        }

        private static async Task SendFunc()
        {
            while (isRunning)
            {
                string message = Console.ReadLine();
                if (message == "exit")
                {
                    isRunning = false;
                    await Receiver();
                }
                message = $"{userName}: {message}";
                var data = Encoding.UTF8.GetBytes(message);
                await udpClient.SendAsync(data, data.Length, "255.255.255.255", 9876);
            }
        }
    }
}