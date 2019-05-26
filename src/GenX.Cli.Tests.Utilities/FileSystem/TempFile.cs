using System;
using System.IO;

namespace GenX.Cli.Tests.Utilities.FileSystem
{
    public class TempFile : IDisposable
    {
        public TempFile(string extension)
        {
            Filename = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString() + extension);
        }

        ~TempFile()
        {
            Dispose();
        }

        public string Filename { get; }

        public void WriteAllText(string contents)
        {
            File.WriteAllText(Filename, contents);
        }

        public void WriteBuffer(byte[] buffer)
        {
            File.WriteAllBytes(Filename, buffer);
        }

        public void Dispose()
        {
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
            }
        }
    }
}
