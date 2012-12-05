using System;
using ZmqLib;

namespace ZmqConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ZeroMQ Sandbox");

            Console.WriteLine("What pattern do you want to use?");
            Console.WriteLine("1) Hello World REQ / RES");
            Console.WriteLine("2) Load Balancing Broker");
            Console.Write("choose: ");
            var pattern = Console.ReadLine();

            switch(pattern)
            {
                case "1":
                    ReqResExample.Execute();
                    break;
                case "2":
                    LoadBalancingBrokerPart2.Execute();
                    break;
            }

        }
    }
}
