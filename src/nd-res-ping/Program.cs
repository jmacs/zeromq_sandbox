using Nodelet;

namespace Nodes.ResPing
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new ResPingNodelet().Start(options);
            });
        }
    }
}
