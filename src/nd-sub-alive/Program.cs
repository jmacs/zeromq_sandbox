using Nodelet;

namespace Nodes.SubAlive
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new SubAliveNodelet().Start(options);
            });
        }
    }
}
