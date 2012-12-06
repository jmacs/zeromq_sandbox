using System;
using System.Text;
using ZeroMQ;
using System.Threading;

namespace Nodes.Sandbox
{
    public class SandboxNodelet
    {
        public void Start(Options options)
        {
            //sometimes if you start debugging immediately after you stop debugging you get errors
            //im assuming its because the socket port hasn't been disposed yet, 
            //not sure if this means i need to explicity disconnect the socket rather than wait for the
            //using block to call close in the garbage collector
			var n2 = new RecieveDealerMessageNodelet();
			new Thread(new ParameterizedThreadStart(n2.Start)).Start(options);

            //give some time for the REP socket to initialize
			Thread.Sleep(5000);

			var n1 = new DealerFireAndForgetNodelet();
			new Thread(new ParameterizedThreadStart(n1.Start)).Start(options);



        }

    } 
}
