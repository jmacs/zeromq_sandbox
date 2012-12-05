using Nodelet;

namespace Nodes.ReqPing
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new ReqPingNodelet().Start(options);
            });
        }
    }
}
