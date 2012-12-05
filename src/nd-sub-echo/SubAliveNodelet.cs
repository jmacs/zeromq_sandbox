using System;
using System.Text;
using ZeroMQ;

namespace Nodes.SubEcho
{
    /// <summary>
    /// A simple subscriber client that echos messages that it subscribes to.
    /// </summary>
    public class SubEchoNodelet
    {
        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (var subscriber = context.CreateSocket(SocketType.SUB))
            {
				
                if (string.IsNullOrEmpty(options.SubscriptionChannel))
                {
                    Console.WriteLine("Subscribing to all channels.");
                    subscriber.SubscribeAll();
                }
                else
                {
                    Console.WriteLine("Subscribing to: {0}", options.SubscriptionChannel);
                    subscriber.Subscribe(Encoding.Unicode.GetBytes(options.SubscriptionChannel));
                }

                var endpoint = string.Format("tcp://{0}", options.SocketConnect);
                Console.WriteLine("Connecting to: {0}", endpoint);
                subscriber.Connect(endpoint);

                while (true)
                {
                    var message = subscriber.Receive(Encoding.Unicode);
                    Console.WriteLine("Recieved message: {0}", message);
                }
            }
        }

    }
 
}
