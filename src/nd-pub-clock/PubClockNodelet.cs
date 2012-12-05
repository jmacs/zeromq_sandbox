using System;
using System.Text;
using ZeroMQ;
using System.Diagnostics;
using System.Threading;

namespace Nodes.PubClock
{
    /// <summary>
    /// A publisher nodelet that pushes the current time to a socket.
    /// </summary>
    public class PubClockNodelet
    {
        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        /// <exception cref="System.InvalidOperationException">Connection Socket is invalid; cannot be null or whitespace.</exception>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (var publisher = context.CreateSocket(SocketType.PUB))
            {
                BindSocket(publisher, string.Format("tcp://{0}",options.SocketBind));

                ForwardAllMessages(publisher);
            }
        }

        /// <summary>
        /// Binds the back end server side socket to the specified endpoint.
        /// </summary>
        /// <param name="backend">The backend socket.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <exception cref="System.Exception"></exception>
        private static void BindSocket(ZmqSocket socket, string endpoint)
        {
            try
            {
                Console.WriteLine("Binding to {0}", endpoint);
                socket.Bind(endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Problem binding to endpoint '{0}'", endpoint), ex);
            }
        }

        /// <summary>
        /// The system time is pushed and then sleeps for a random amount of time.  
        /// </summary>
        /// <param name="frontend">The frontend.</param>
        /// <param name="backend">The backend.</param>
        private static void ForwardAllMessages(ZmqSocket socket)
        {
			var randomizer = new Random(DateTime.Now.Millisecond);

            while (true)
            {
                socket.Send(DateTime.UtcNow.Ticks.ToString(), Encoding.Unicode);
				var sleepMs = randomizer.Next(500, 2000);
				Thread.Sleep(sleepMs);
            }
        }
    }

    
}
