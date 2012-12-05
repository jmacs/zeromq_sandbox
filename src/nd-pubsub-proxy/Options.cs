using System;
using Nodelet;

namespace Nodes.PubSubProxy
{
    public class Options : CommandLineOptionsBase
    {
        [Option("c", "connect", Required = true, HelpText = "Socket connection endpoint.")]
        public string SocketConnection { get; set; }

        [Option("b", "bind", Required = true, HelpText = "Bind socket endpoint for accepting connections.")]
        public string SocketBind { get; set; }

        public bool IsValid { get; protected set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public static Options Parse(string[] args)
        {
			var options = new Options();
            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            options.IsValid = parser.ParseArguments(args, options);
            return options;
        }
    }
}
