using System;
using System.Text;
using ZeroMQ;
using ZeroMQ.Devices;
using System.Threading;
using System.Collections.Generic;

namespace Nodes.Broker
{
    // the example in the guide use the v2 version of clrzmq
    // here's the new way to poll: https://github.com/zeromq/clrzmq/issues/104
    public class BrokerNodelet
    {
        public void Start(Options options)
        {
            using(var context = ZmqContext.Create())
            using(var frontend = context.CreateSocket(SocketType.ROUTER))
            using(var backend = context.CreateSocket(SocketType.ROUTER))
            using(var poller = new Poller(new[]{frontend,backend}))
            {
                var frontendEndpoint = string.Format ("tcp://{0}", options.SocketFrontEnd);
                Console.WriteLine ("Front-end socket binding to: {0}", frontendEndpoint);
                frontend.Bind(frontendEndpoint);
                frontend.ReceiveReady += FrontEndReady;

                var backendEndpoint = string.Format ("tcp://{0}", options.SocketBackEnd);
                Console.WriteLine ("Back-end socket connecting to: {0}", backendEndpoint);
                backend.Connect(backendEndpoint);
                backend.ReceiveReady += BackEndReady;

                while (true)
                {
                    poller.Poll();
                }

            }
        }

        private void FrontEndReady(object sender, SocketEventArgs e)
        {

        }

        private void BackEndReady(object sender, SocketEventArgs e)
        {

        }
    } 
}
