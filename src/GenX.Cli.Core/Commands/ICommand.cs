namespace GenX.Cli.Core.Commands
{
    public interface ICommand
    {
        ExitCode Execute();
    }
}
