using GenX.Cli.Core;
using System.IO;

namespace GenX.Cli.Infrastructure.FileSystem
{
    public class DirectoryWrapper : IDirectory
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
