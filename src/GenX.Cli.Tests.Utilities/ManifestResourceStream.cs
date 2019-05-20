using System.Reflection;

namespace GenX.Cli.Tests.Utilities
{
    public static class ManifestResourceStream
    {
        public static byte[] Get(Assembly assembly, string manifestResourceName)
        {
            byte[] buffer;

            using (var stream = assembly.GetManifestResourceStream(manifestResourceName))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
            }

            return buffer;
        }
    }
}
