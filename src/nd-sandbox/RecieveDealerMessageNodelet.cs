using System;
using System.Text;
using ZeroMQ;
using System.Linq;

namespace Nodes.Sandbox
{
	public class RecieveDealerMessageNodelet
	{
		public void Start(object ooptions)
		{
			var options = ooptions as Options;

			using (var context = ZmqContext.Create())
            using (var server = context.CreateSocket(SocketType.REP))
            {
                var endpoint = string.Format("tcp://{0}", options.SocketBind);
                Console.WriteLine("Binding to: {0}", endpoint);
                server.Bind(endpoint);

                var receivedFrames = 0;
				while(true)
				{
                    try
                    {
                        var message = server.Receive(Encoding.Unicode);
                        Console.WriteLine("   received message {0}, message {1}", receivedFrames, message);
                        receivedFrames++;
                        //REP has to send a response, if we do two sequential receives we'll crash
                        server.Send("", Encoding.Unicode); 

                        /* If we used a ROUTER (the async REP), we get a bunch of 
                         * frame data at the begining of the message like this:
                         *    received message 0, message 欀䖋
                               received message 1, message 
                               received message 2, message Hello
                               received message 3, message 欀䖋
                               received message 4, message 
                               received message 5, message Hello
                               received message 6, message 欀䖋

                                looks like the messages are front loaded 
                                with the sender socket data followed by null space
                                http://zguide.zeromq.org/page:all#toc49
                         * */
                    }
                    catch(ZmqSocketException ex)
                    {
                        //the error message are unhelpful, not sure if this is related to my
                        //mono installation of zeromq or if the error message are just sucky
                        //this is not good from a debug/tracing perspective...
                        //the error is usually "No such file or directory". 
                        //code is 2. name is ENOENT, I havea feeling those errors are OS related, will have to try in Windows...
                        //google isn't much help either; no good leads searching that error information as of Dec 2012
                        Console.WriteLine("[{0}{1}] - {2}", ex.ErrorCode, ex.ErrorName, ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        break;
                    }
					
				}

			}
			
		}

	}
}
