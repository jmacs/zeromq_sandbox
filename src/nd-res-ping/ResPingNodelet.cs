using System;
using System.Text;
using ZeroMQ;
using System.Diagnostics;

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
            {
				Trace.Assert(string.IsNullOrWhiteSpace(options.SocketBind), 
				             "Bind socket is invalid; cannot be null or whitespace.");

				var endpoint = string.Format("tcp://{0}", options.SocketBind);
				Console.WriteLine("Binding to: {0}", endpoint);
                server.Bind(endpoint);

                while (true)
                {
                    // Wait for next request from client
                    string message = server.Receive(Encoding.Unicode);
					server.Send(message, Encoding.Unicode);
                }
            }
        }

    }
 
}
