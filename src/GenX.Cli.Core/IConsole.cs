namespace GenX.Cli.Core
{
    public interface IConsole
    {
        void Write(string message);
        void WriteLine(string message);
        void WriteLine();
    }
}
