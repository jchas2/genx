using GenX.Cli.Core;
using System.IO;

namespace GenX.Cli.Infrastructure.FileSystem
{
    public class FileWrapper : IFile
    {
        public bool Exists(string filename) => File.Exists(filename);
    }
}
