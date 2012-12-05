using System;
using Nodelet;

namespace Nodes.ReqPing
{
    public class Options : CommandLineOptionsBase
    {
        [Option("c", "connect", Required = true, HelpText = "Connection socket endpoint to ping.")]
        public string SocketConnection { get; set; }

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
