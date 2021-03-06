﻿using System;
using Nodelet;

namespace Nodes.Sandbox
{
    public class Options : CommandLineOptionsBase
    {
        [Option("c", "connect", Required = false, HelpText = "Connection socket endpoint.")]
        public string SocketConnection { get; set; }

		[Option("b", "bind", Required = false, HelpText = "Bind socket endpoint.")]
        public string SocketBind { get; set; }

        [Option("s", "switch", Required = false, HelpText = "A stupid hack for controlling what nodelets to use in the tests.")]
        public string Switch { get; set; }

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
