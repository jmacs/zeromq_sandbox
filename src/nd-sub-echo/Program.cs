using Nodelet;

namespace Nodes.SubEcho
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new SubEchoNodelet().Start(options);
            });
        }
    }
}
