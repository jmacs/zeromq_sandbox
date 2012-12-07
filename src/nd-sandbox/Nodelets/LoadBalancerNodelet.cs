using System;
using System.Text;
using ZeroMQ;
using System.Threading;

namespace Nodes.Sandbox.Nodelets
{
	public class LoadBalancerNodelet
	{
        public void Start (object ooptions)
        {
            var options = ooptions as Options;

            using (var context = ZmqContext.Create())
            using (var router = context.CreateSocket(SocketType.ROUTER))
            using (var client = context.CreateSocket(SocketType.REQ))
            {
                var routerEndpoint = string.Format ("tcp://{0}", options.SocketBind);
                Console.WriteLine ("Router socket binding to: {0}", routerEndpoint);
                router.Bind(routerEndpoint);

                var clientEndpoint = string.Format ("tcp://{0}", options.SocketConnection);
                Console.WriteLine ("Client socket connecting to: {0}", clientEndpoint);
                client.Connect(clientEndpoint);

                try
                {
                    while(true)
                    {
                        var address = router.Receive(Encoding.Unicode);
                        var empty = router.Receive(Encoding.Unicode);
                        var message = router.Receive(Encoding.Unicode);
                        Console.WriteLine("Router recieved ready message from {0}", address);

                        Console.WriteLine("Requesting work from server...");
                        client.Send(string.Empty, Encoding.Unicode);
                        var workload = client.Receive(Encoding.Unicode);
                        Console.WriteLine("Forwarding workload '{0}'", workload);

                        router.SendMore(address, Encoding.Unicode);
                        router.SendMore(string.Empty, Encoding.Unicode);
                        router.Send(workload, Encoding.Unicode);
                    }

                } 
                catch (Exception ex)
                {
                    Console.WriteLine("router failed: {0}", ex.Message);
                }

            }
        }
	}
 
}
