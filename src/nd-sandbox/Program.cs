using Nodelet;

namespace Nodes.Sandbox
{
    class Program
    {
        static int Main(string[] args)
        {
            return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new SandboxNodelet().Start(options);
            });
        }
    }
}
