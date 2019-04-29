using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.Console
{
    public class OutputWriter : IOutputWriter
    {
        public OutputWriter()
        {
            Output = new ConsoleWrapper(System.Console.Out);
            Error = new ConsoleWrapper(System.Console.Error);
            Verbose = new ConsoleWrapper(null);
        }

        public bool IsVerbose
        {
            set { Verbose = new ConsoleWrapper(System.Console.Out); }
        }

        public IConsole Output { get; private set; }
        public IConsole Error { get; private set; }
        public IConsole Verbose { get; private set; }
    }
}
