using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using System.Threading;

namespace ZmqLib
{
    public class LoadBalancingBroker
    {
        public const int WorkersCount = 10;
        public static List<Thread> Workers = new List<Thread>(WorkersCount);

        public static void Execute()
        {
            Console.WriteLine("creating worker processes");
            foreach (var i in Enumerable.Range(0, WorkersCount))
            {
                Workers.Add(new Thread(Worker));
                Workers[i].Start();
            }

            Console.WriteLine("starting router");
            Router();

            Console.WriteLine("done");
            Console.ReadLine();
        }

        public static void Router()
        {
            using (var context = ZmqContext.Create())
            using (var router = context.CreateSocket(SocketType.ROUTER))
            {
                router.Bind("tcp://*:5555");

                foreach (var i in Enumerable.Range(0, WorkersCount))
                {
                    //  LRU worker is next waiting in queue
                    var address = router.Receive(Encoding.Unicode);
                    var empty = router.Receive(Encoding.Unicode);
                    var ready = router.Receive(Encoding.Unicode);

                    router.SendMore(address, Encoding.Unicode);
                    router.SendMore("", Encoding.Unicode);
                    router.Send("This is the workload", Encoding.Unicode);
                }

            }
        }

        public static void Worker()
        {
            var randomizer = new Random(DateTime.Now.Millisecond);

            using (var context = ZmqContext.Create())
            using (var worker = context.CreateSocket(SocketType.REQ))
            {
                worker.Connect("tcp://localhost:5555");

                int total = 0;

                bool end = false;
                while (!end)
                {
                    //  Tell the router we're ready for work
                    worker.Send("Ready", Encoding.Unicode);

                    //  Get workload from router, until finished
                    var workload = worker.Receive(Encoding.Unicode);

                    Console.WriteLine("recieved workload: " + workload);

                    if (workload == "END")
                    {
                        end = true;
                        
                    }
                    else
                    {
                        total++;
                        Console.WriteLine("doing work...");
                        //Thread.Sleep(randomizer.Next(1, 1000)); //  Simulate 'work'
                    }
                }

                Console.WriteLine("ID ({0}) processed: {1} tasks", Encoding.Unicode.GetString(worker.Identity), total);
                
            }
        }
    }
}