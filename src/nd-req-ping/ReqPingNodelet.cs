using System;
using System.Text;
using ZeroMQ;
using System.Diagnostics;
using System.Linq;

namespace Nodes.ReqPing
{
    /// <summary>
    /// A simple pinging nodelet.
    /// </summary>
    public class ReqPingNodelet
    {
		const int NumberOfMessages = 4;

        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (var client = context.CreateSocket(SocketType.REQ))
            {
				Trace.Assert(string.IsNullOrWhiteSpace(options.SocketConnection), 
				             "Connection socket is invalid; cannot be null or whitespace.");

				var endpoint = string.Format("tcp://{0}", options.SocketConnection);
				Console.WriteLine("Connecting to: {0}", endpoint);
				client.Connect(endpoint);

				var sw = new Stopwatch();

				foreach(var i in Enumerable.Range(1, NumberOfMessages))
				{
					Console.WriteLine("Message {0} of {1} sent", i, NumberOfMessages);
					var message = DateTime.UtcNow.Ticks.ToString();
					sw.Restart();
					client.Send(message, Encoding.Unicode);
					var reply = client.Receive(Encoding.Unicode);
					sw.Stop();
					var echo = message == reply ? 1 : 0;
					Console.WriteLine("{0}: Reply received in {1}ms", echo, sw.ElapsedMilliseconds);
				}
                
            }
        }

    }
 
}
