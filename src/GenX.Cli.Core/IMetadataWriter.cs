using System.Xml;

namespace GenX.Cli.Core
{
    public interface IMetadataWriter<T>
    {
        XmlDocument Write(T model);
    }
}
