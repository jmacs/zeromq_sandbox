using System;
using System.Text;
using ZeroMQ;
using System.Threading;

namespace Nodes.Sandbox.Nodelets
{
	public class LoadBalancerWorkerNodelet
	{
        public void Start (object ooptions)
        {
            var options = ooptions as Options;
            var randomizer = new Random(DateTime.Now.Millisecond);

            using (var context = ZmqContext.Create())
            using (var worker = context.CreateSocket(SocketType.REQ))
            {
                var endpoint = string.Format ("tcp://{0}", options.SocketConnection);
                Console.WriteLine ("Connectiong worker request socket to: {0}", endpoint);
                worker.Identity = Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
                worker.Connect(endpoint);

                Console.WriteLine("ID ({0}) started", Encoding.Unicode.GetString(worker.Identity));
                
                var end = false;
                var total = 0;

                try
                {
                    while (!end)
                    {
                        //  Tell the router we're ready for work
                        Console.WriteLine("Waiting for work.");
                        worker.Send("Ready", Encoding.Unicode);

                        //  Get workload from router, until finished
                        string workload = worker.Receive(Encoding.Unicode);

                        if (workload.Equals("END"))
                        {
                            end = true;
                        }
                        else
                        {
                            total++;
                            int sleeptime = int.Parse(workload);
                            Console.Write("Workload '{0}' recieved, performing work...", sleeptime);
                            Thread.Sleep(randomizer.Next(1, sleeptime)); //  Simulate 'work'
                            Console.WriteLine("done.");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Worker failed: {0}, total: {1}", ex.Message, total);
                }


                Console.WriteLine("ID ({0}) processed: {1} tasks", Encoding.Unicode.GetString(worker.Identity), total);
            }

        }
	}
 
}
