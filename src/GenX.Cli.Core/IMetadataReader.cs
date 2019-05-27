using System.Collections.Generic;

namespace GenX.Cli.Core
{
    public interface IMetadataReader
    {
        IEnumerable<string> ReadNames(string filename);
        IEnumerable<string> ReadNames(string filename, string filter);
    }
}
