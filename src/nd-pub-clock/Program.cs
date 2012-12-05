using System;
using Nodelet;

namespace Nodes.PubClock
{
	class Program
	{
		public static int Main(string[] args)
		{
			return new NodeletContext().Execute(() => {
                var options = Options.Parse(args);
                if (options.IsValid) new PubClockNodelet().Start(options);
            });
		}
	}
}
