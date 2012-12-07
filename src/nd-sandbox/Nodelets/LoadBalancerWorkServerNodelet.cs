using System;
using ZeroMQ;
using System.Text;

namespace Nodes.Sandbox.Nodelets
{
    public class LoadBalancerWorkServerNodelet
    {
        public void Start (object ooptions)
        {
            var options = ooptions as Options;

            using (var context = ZmqContext.Create())
            using (var server = context.CreateSocket(SocketType.REP))
            {
                var endpoint = string.Format("tcp://{0}", options.SocketBind);
                Console.WriteLine ("Connecting work server socket to: {0}", endpoint);
                server.Bind(endpoint);

                while(true)
                {
                    var message = server.Receive(Encoding.Unicode);
                    Console.WriteLine("Router is requesting work...");
                    var workload = GetWorkloadFromUserInput();
                    server.Send(workload.ToString(), Encoding.Unicode);
                }
            }
        }

        private int GetWorkloadFromUserInput ()
        {
            var inputIsNumber = false;

            while(!inputIsNumber)
            {
                Console.Write("Workload (number): ");
                var input = Console.ReadLine();
                var number = 0;
                inputIsNumber = int.TryParse(input, out number);

                if(!inputIsNumber)
                {
                    Console.WriteLine("Invalid. Must be a number.");
                }
                else
                {
                    return number;
                }
            }

            return 0;
        }

    }
}

