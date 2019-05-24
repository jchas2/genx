using System.Reflection;

namespace GenX.Cli.Core.Commands.Version
{
    public class VersionCommand : ICommand
    {
        private readonly IOutputWriter _outputWriter;

        public VersionCommand(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public ExitCode Execute()
        {
            _outputWriter.Output.WriteLine(
                Assembly.GetEntryAssembly().GetName().Version.ToString());

            return ExitCode.Success;
        }
    }
}
