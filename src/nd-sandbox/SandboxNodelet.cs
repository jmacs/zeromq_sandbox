using System;
using System.Text;
using ZeroMQ;
using System.Threading;
using System.Collections.Generic;
using Nodes.Sandbox.Nodelets;

namespace Nodes.Sandbox
{
    public class SandboxNodelet
    {
        /// <summary>
        /// Starts the nodelet.
        /// </summary>
        /// <param name='options'>
        /// The program options.
        /// </param>
        public void Start(Options options)
        {
            //TestDealerFireAndForget(options);

            TestLoadBalancingRouter(options); 

        }

        /// <summary>
        /// Creates a broker using a router and spawns a few workers.
        /// The workers notify the broker they are ready for work and the broker dishes out stuff for them to do.
        /// </summary>
        /// <param name='options'>
        /// Options.
        /// </param>
        private void TestLoadBalancingRouter (Options options)
        {
            /* In this experiment we have 2 static nodes the server and 
             * router and N dynamic worker nodes. The process goes like this:
             * 1) A worker boots up and asks the router for work
             * 2) The router acknowledges the request and in turn asks the server for the next work item
             * 3) The server recieves the request and asks the user to enter a workload (a number)
             * 4) The user enters a number which the server sends back to the router
             * 5) The router forwards the workload (the number) to the first available worker
             * 6) The worker recieves the number and sleeps for that amount of time (in ms)
             * 7) Once the worker completes the work (thread finishes sleeping) we start back at step 1
             * 
             * I found that the workers could boot up before the router and everything goes smoothly from there.
             * However, if any of the static nodes of the network go down we have a problem. Workers will hang
             * waiting for work if the router goes down and the router will hang if the server goes down. Basically
             * your screwed if a node is sitting there waiting for a response from a crashed node. It's kind of
             * like if you were browsing youtube and the youtube server went down and hung your browser. In the end, 
             * if the static nodes die the workers must also be restarted. Maybe the workers can 
             * time out, die, and restart?
             * 
             * This is also got me thinking about message reliability. Assuming the server was reading from a queue
             * rather than receiving input from the user. The message is sent from the server to the router and 
             * the server doesnt care that the message actually gets delivered to the workers or that the
             * work is actually performed. I need to do some more reading about distributed systems and see if 
             * there are any answers about reliability.
             */
            if (options.Switch == "server")
            {
                var server = new LoadBalancerWorkServerNodelet ();
                new Thread (new ParameterizedThreadStart (server.Start)).Start (options);//bind
            
            }

            if (options.Switch == "router")
            {
                var n1 = new LoadBalancerNodelet ();
                new Thread (new ParameterizedThreadStart (n1.Start)).Start (options);//bind
            }

            if (options.Switch == "worker")
            {
                var worker = new LoadBalancerWorkerNodelet ();
                new Thread (new ParameterizedThreadStart (worker.Start)).Start (options);
            }
        }

        /// <summary>
        /// Test the dealer with a fire and forget type of communication.
        /// </summary>
        /// <param name='options'>
        /// Options.
        /// </param>
        public static void TestDealerFireAndForget(Options options)
        {
            /*from the zeromq user guide:
         * The side which we expect to "be there" binds: it'll be a server, a broker, a publisher, a collector. 
         * The side that "comes and goes" connects: it'll be clients and workers. Remembering this will help 
         * you design better ØMQ architectures.
         */
            //sometimes if you start debugging immediately after you stop debugging you get errors
            //im assuming its because the socket port hasn't been disposed yet, 
            //not sure if this means i need to explicity disconnect the socket rather than wait for the
            //using block to call close in the garbage collector. maybe monodevelop is hanging onto something
            //i haven't seen this problem when running the app in the terminal
            var n2 = new DealerFireAndForgetRecieveMessageNodelet();
            new Thread(new ParameterizedThreadStart(n2.Start)).Start(options);//bind

            //give some time for the REP socket to initialize
            Thread.Sleep(5000);

            var n1 = new DealerFireAndForgetNodelet();
            new Thread(new ParameterizedThreadStart(n1.Start)).Start(options);//connect
        }

    } 
}
