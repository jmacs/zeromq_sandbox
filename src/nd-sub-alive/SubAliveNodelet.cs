using System;
using System.Text;
using ZeroMQ;

namespace Nodes.SubAlive
{
    /// <summary>
    /// A simple ping response nodelet.
    /// </summary>
    public class SubAliveNodelet
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
				var endpoint = string.Format("tcp://{0}", options.SocketBind);
				Console.WriteLine("Connecting to: {0}", endpoint);
                //subscriber.Subscribe("alive");
                subscriber.Connect(endpoint);

                
            }
        }

    }
 
}
