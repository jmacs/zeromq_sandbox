using Nodelet;

namespace Nodes.Broker
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new BrokerNodelet().Start(options);
            });
        }
    }
}
