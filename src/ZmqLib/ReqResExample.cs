using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZmqLib
{
    public class ReqResExample
    {
        public const int RequestsCount = 2;
        public const int ClientsCount = 2;
        public static List<Thread> Clients = new List<Thread>(ClientsCount);

        public static void Execute()
        {
            Console.WriteLine("creating clients");
            foreach (var i in Enumerable.Range(0, ClientsCount))
            {
                Clients.Add(new Thread(Client));
                Clients[i].Start();
            }

            Thread.Sleep(2000);

            Server();

            Console.WriteLine("done");
        }

        public static void Client()
        {
            // ZMQ Context and client socket
            using (var context = ZmqContext.Create())
            using (var client = context.CreateSocket(SocketType.REQ))
            {
                client.Connect("tcp://localhost:5555");

                var request = Guid.NewGuid() + " ";
                for (var requestNum = 0; requestNum < RequestsCount; requestNum++)
                {
                    Console.WriteLine("\nSending request {0}...", requestNum);
                    client.Send(request + DateTime.Now.Ticks, Encoding.Unicode);

                    var reply = client.Receive(Encoding.Unicode);
                    Console.WriteLine("Received reply {0}: {1}", requestNum, reply);
                }
            }
        }

        public static void Server()
        {
            Console.WriteLine("Starting server");

            // ZMQ Context, server socket
            using (var context = ZmqContext.Create())
            using (var server = context.CreateSocket(SocketType.REP))
            {
                server.Bind("tcp://*:5555");

                while (true)
                {
                    // Wait for next request from client
                    var message = server.Receive(Encoding.Unicode);

                    // Do Some 'work'
                    Thread.Sleep(1000);

                    // Send reply back to client
                    server.Send(message, Encoding.Unicode);
                }
            }
        }
    }
}
