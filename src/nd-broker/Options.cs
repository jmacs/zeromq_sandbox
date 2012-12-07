using System;
using Nodelet;

namespace Nodes.Broker
{
    public class Options : CommandLineOptionsBase
    {
        [Option("b", "backend", Required = false, HelpText = "Back-end bind socket endpoint.")]
        public string SocketBackEnd { get; set; }

		[Option("f", "frontend", Required = false, HelpText = "Front-end bind socket endpoint.")]
        public string SocketFrontEnd { get; set; }
       
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
