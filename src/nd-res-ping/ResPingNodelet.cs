using System;
using System.Text;
using ZeroMQ;

namespace Nodes.ResPing
{
    /// <summary>
    /// A simple ping response nodelet.
    /// </summary>
    public class ResPingNodelet
    {
        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (var server = context.CreateSocket(SocketType.REP))
            using (var publisher = context.CreateSocket(SocketType.PUB))
            {
				var endpoint = string.Format("tcp://{0}", options.SocketBind);
				Console.WriteLine("Binding to: {0}", endpoint);
                server.Bind(endpoint);

                while (true)
                {
                    // Wait for next request from client
                    var message = server.Receive(Encoding.Unicode);
					server.Send(message, Encoding.Unicode);
                }
            }
        }

    }
 
}
