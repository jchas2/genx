namespace GenX.Cli.Core.Commands.Help
{
    public class HelpCommand : ICommand
    {
        private readonly IOutputWriter _outputWriter;

        public HelpCommand(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public ExitCode Execute()
        {
            _outputWriter.Output.WriteLine(HelpText.Text);
            return ExitCode.Success;
        }
    }
}
