using System;
using System.Text;
using ZeroMQ;

namespace Nodes.SubAlive
{
    /// <summary>
    /// A simple subscriber client that recieves alive messages from oter nodelets.
    /// </summary>
    public class SubAliveNodelet
    {
        private const string Channel = "alive";

        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (var subscriber = context.CreateSocket(SocketType.SUB))
            {
				var endpoint = string.Format("tcp://{0}", options.SocketConnect);

                Console.WriteLine("Subscribing to: {0}", Channel);
                subscriber.Subscribe(Encoding.Unicode.GetBytes(Channel));

                Console.WriteLine("Connecting to: {0}", endpoint);
                subscriber.Connect(endpoint);

                while (true)
                {
                    var message = subscriber.Receive(Encoding.Unicode);
                    Console.WriteLine("Recieved message! {0}", message);
                }
            }
        }

    }
 
}
