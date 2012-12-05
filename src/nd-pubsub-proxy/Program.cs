using Nodelet;

namespace Nodes.PubSubProxy
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new PubSubProxyNodelet().Start(options);
            });
        }
    }
}
