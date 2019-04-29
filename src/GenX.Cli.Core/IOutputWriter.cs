namespace GenX.Cli.Core
{
    public interface IOutputWriter
    {
        bool IsVerbose { set; }
        IConsole Output { get; }
        IConsole Error { get; }
        IConsole Verbose { get; }
    }
}
