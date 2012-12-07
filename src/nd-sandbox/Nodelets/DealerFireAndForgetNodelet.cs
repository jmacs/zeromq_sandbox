using System;
using System.Text;
using ZeroMQ;
using System.Linq;

namespace Nodes.Sandbox.Nodelets
{
	public class DealerFireAndForgetNodelet
	{
		public void Start(object ooptions)
		{
			var options = ooptions as Options;

			using (var context = ZmqContext.Create())
            using (var dealer = context.CreateSocket(SocketType.DEALER))
            {
                var endpoint = string.Format("tcp://{0}", options.SocketConnection);
                Console.WriteLine("Connecting to: {0}", endpoint);
                dealer.Connect(endpoint);

				foreach(var i in Enumerable.Range(0, 10))
				{
					Console.WriteLine("Sending message {0}", i);
                    //DEALER has to send an empty frame to be caught by a REP
                    dealer.SendMore("", Encoding.Unicode);
					dealer.Send("Hello", Encoding.Unicode);
                    //DEALER is async, we don't wait for a reply
                    //if this was a REQ we would get an error because we would
                    //do two sequential sends without a receive
				}

				dealer.Disconnect(endpoint);
				//dealer.Close();
			}
		}
	}
 
}
