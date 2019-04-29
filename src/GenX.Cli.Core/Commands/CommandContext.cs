using System.Collections.Generic;

namespace GenX.Cli.Core.Commands
{
    public class CommandContext
    {
        public bool AttachDebugger { get; set; } = false;
        public bool VerboseLogging { get; set; } = false;
        public bool NoLogging { get; set; } = false;
        public bool NoLogo { get; set; } = false;
        public List<string> CommandArgs { get; set; }
    }
}
