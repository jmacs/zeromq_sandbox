﻿using System;
using System.Collections.Generic;
using System.Text;
using ZeroMQ;
using System.Threading;
using ZeroMQ.Devices;

namespace ZmqLib
{
    public class LoadBalancingBrokerPart2
    {
        public static void Execute()
        {
            const int workersToStart = 3;
            int clientsRunning = 10;

            var workers = new List<Thread>();
            var clients = new List<Thread>();

            using (ZmqContext context = ZmqContext.Create())
            {
                using (ZmqSocket frontend = context.CreateSocket(SocketType.ROUTER),
                                 backend = context.CreateSocket(SocketType.ROUTER))
                {

                    frontend.Bind("tcp://*:5555");
                    backend.Bind("tcp://*:5556");

                    for (int clientNumber = 0; clientNumber < clientsRunning; clientNumber++)
                    {
                        clients.Add(new Thread(ClientTask));
                        clients[clientNumber].Start();
                    }

                    for (int workerNumber = 0; workerNumber < workersToStart; workerNumber++)
                    {
                        workers.Add(new Thread(WorkerTask));
                        workers[workerNumber].Start();
                    }

                    //  Logic of LRU loop
                    //  - Poll backend always, frontend only if 1+ worker ready
                    //  - If worker replies, queue worker as ready and forward reply
                    //    to client if necessary
                    //  - If client requests, pop next worker and send request to it
                    var workerQueue = new Queue<string>();

                    //  Handle worker activity on backend
                    backend.ReceiveReady += (sender, e) =>
                                                {

                                                    //  Queue worker address for LRU routing
                                                    string workerAddress = e.Socket.Receive(Encoding.Unicode);
                                                    workerQueue.Enqueue(workerAddress);

                                                    //  Second frame is empty
                                                    string empty = e.Socket.Receive(Encoding.Unicode);

                                                    //  Third frame is READY or else a client reply address
                                                    string clientAddress = e.Socket.Receive(Encoding.Unicode);

                                                    //  If client reply, send rest back to frontend
                                                    if (!clientAddress.Equals("READY"))
                                                    {
                                                        empty = e.Socket.Receive(Encoding.Unicode);
                                                        string reply = e.Socket.Receive(Encoding.Unicode);
                                                        frontend.SendMore(clientAddress, Encoding.Unicode);
                                                        frontend.SendMore("", Encoding.Unicode);
                                                        frontend.Send(reply, Encoding.Unicode);

                                                        clientsRunning--; //  Exit after N messages
                                                    }
                                                };

                    frontend.ReceiveReady += (sender, e) =>
                                                 {
                                                     //  Now get next client request, route to LRU worker
                                                     //  Client request is [address][empty][request]
                                                     string clientAddr = e.Socket.Receive(Encoding.Unicode);
                                                     string empty = e.Socket.Receive(Encoding.Unicode);
                                                     string request = e.Socket.Receive(Encoding.Unicode);

                                                     backend.SendMore(workerQueue.Dequeue(), Encoding.Unicode);
                                                     backend.SendMore("", Encoding.Unicode);
                                                     backend.SendMore(clientAddr, Encoding.Unicode);
                                                     backend.SendMore("", Encoding.Unicode);
                                                     backend.Send(request, Encoding.Unicode);
                                                 };

                    var poller = new Poller(new[] {frontend, backend});
                    while (clientsRunning > 0)
                    {
                        poller.Poll();
                    }

                }
            }
        }

        private static void ClientTask()
        {
            using (var context = ZmqContext.Create())
            using (ZmqSocket client = context.CreateSocket(SocketType.REQ))
            {
                client.Connect("tcp://localhost:5555");

                //  Send request, get reply
                client.Send("HELLO", Encoding.Unicode);
                string reply = client.Receive(Encoding.Unicode);
                Console.WriteLine("Client: {0}", reply);
            }
        }

        private static void WorkerTask()
        {
            using (var context = ZmqContext.Create())
            using (ZmqSocket worker = context.CreateSocket(SocketType.REQ))
            {
                worker.Connect("tcp://localhost:5556");

                //  Tell broker we're ready for work
                worker.Send("READY", Encoding.Unicode);

                while (true)
                {
                    //  Read and save all frames until we get an empty frame
                    //  In this example there is only 1 but it could be more
                    string address = worker.Receive(Encoding.Unicode);
                    string empty = worker.Receive(Encoding.Unicode);

                    //  Get request, send reply
                    string request = worker.Receive(Encoding.Unicode);
                    Console.WriteLine("Worker: {0}", request);

                    worker.SendMore(address, Encoding.Unicode);
                    worker.SendMore("", Encoding.Unicode);
                    worker.Send("OK", Encoding.Unicode);
                }
            }
        }


    }
}