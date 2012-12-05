using System;
using System.Text;
using ZeroMQ;
using System.Diagnostics;

namespace Nodes.PubSubProxy
{
    /// <summary>
    /// A pub-sub proxy nodelet used for dynamic discovery. 
    /// The proxy in the "middle" of the network and connects/binds to well-known IP addresses and ports. 
    /// All other processes connect to the proxy, instead of to each other. 
    /// It becomes trivial to add more subscribers or publishers.
    /// </summary>
    public class PubSubProxyNodelet
    {
        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name="options">The nodelet command options.</param>
        /// <exception cref="System.InvalidOperationException">Connection Socket is invalid; cannot be null or whitespace.</exception>
        public void Start(Options options)
        {
            using (var context = ZmqContext.Create())
            using (ZmqSocket frontend = context.CreateSocket(SocketType.SUB),
                             backend = context.CreateSocket(SocketType.PUB))
            {
				Trace.Assert(string.IsNullOrWhiteSpace(options.SocketConnection), 
				             "Connection socket is invalid; cannot be null or whitespace.");

				Trace.Assert(string.IsNullOrWhiteSpace(options.SocketBind), 
				             "Bind socket is invalid; cannot be null or whitespace.");

                ConnectFrontEnd(frontend, string.Format("tcp://{0}", options.SocketConnection));

                BindBackEnd(backend, string.Format("tcp://{0}",options.SocketBind));

                ForwardAllMessages(frontend, backend);
            }
        }

        /// <summary>
        /// Binds the back end server side socket to the specified endpoint.
        /// </summary>
        /// <param name="backend">The backend socket.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <exception cref="System.Exception"></exception>
        private static void BindBackEnd(ZmqSocket backend, string endpoint)
        {
            try
            {
                Console.WriteLine("Binding to {0}", endpoint);
                backend.Bind(endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Problem binding to '{0}'", endpoint), ex);
            }
        }

        /// <summary>
        /// Connects the front end client side socket to the specified endpoint and subscribes to all messages.
        /// </summary>
        /// <param name="frontend">The frontend socket.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <exception cref="System.Exception"></exception>
        private static void ConnectFrontEnd(ZmqSocket frontend, string endpoint)
        {
            try
            {
                Console.WriteLine("Connecting to {0}", endpoint);
                frontend.Connect(endpoint);

                Console.WriteLine("Subscribing to all messages");
                frontend.SubscribeAll();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Problem connecting to '{0}'", endpoint), ex);
            }
        }

        /// <summary>
        /// Goes into an infinate loop where all published messages are forwarded to subscribers.
        /// </summary>
        /// <param name="frontend">The frontend.</param>
        /// <param name="backend">The backend.</param>
        private static void ForwardAllMessages(ZmqSocket frontend, ZmqSocket backend)
        {
            while (true)
            {
                var hasMore = true;
                while (hasMore)
                {
                    var message = frontend.Receive(Encoding.Unicode);
                    hasMore = frontend.ReceiveMore;

                    if (hasMore)
                    {
                        backend.SendMore(message, Encoding.Unicode);
                    }
                    else
                    {
                        backend.Send(message, Encoding.Unicode);
                    }
                }
            }
        }
    }

    
}
