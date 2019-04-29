using GenX.Cli.Core;
using System.IO;

namespace GenX.Cli.Infrastructure.Console
{
    public class ConsoleWrapper : IConsole
    {
        public ConsoleWrapper(TextWriter writer) => Writer = writer;

        public TextWriter Writer { get; }

        public void WriteLine()
        {
            Writer?.WriteLine();
        }

        public void WriteLine(string message)
        {
            Write(message);
            Writer?.WriteLine();
        }

        public void Write(string message)
        {
            Writer?.Write(message);
        }
    }
}
