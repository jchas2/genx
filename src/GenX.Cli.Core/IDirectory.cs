namespace GenX.Cli.Core
{
    public interface IDirectory
    {
        bool Exists(string path);
        string GetCurrentDirectory();
        void CreateDirectory(string path);
    }
}
