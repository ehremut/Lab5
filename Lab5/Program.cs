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

        private static bool isRunning = true;
        private static UdpClient udpClient;
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("What's your name? ");
            userName = Console.ReadLine();
            int PORT = 9876;
            udpClient = new UdpClient {ExclusiveAddressUse = false};
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            Task rec = Receiver();
            Task send = SendFunc();
            await rec;
            await send;
        }

        private static async Task Receiver()
        {
            while (isRunning)
            {
                var recvBuffer = await udpClient.ReceiveAsync();
                Console.WriteLine(Encoding.UTF8.GetString(recvBuffer.Buffer));
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
                    
                }
                else
                {
                    message = $"{userName}: {message}";
                    var data = Encoding.UTF8.GetBytes(message);
                    await udpClient.SendAsync(data, data.Length, "255.255.255.255", 9876);
                }
                
            }
        }
    }
}