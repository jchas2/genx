using System.Collections.Generic;

namespace GenX.Cli.Core
{
    public interface IMetadataReader
    {
        IEnumerable<string> ReadEntityNames(string filename);
    }
}
