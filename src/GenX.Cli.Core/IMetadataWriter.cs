using System.Xml;

namespace GenX.Cli.Core
{
    public interface IMetadataWriter
    {
        XmlDocument WriteEntities(DbModel dbModel);
    }
}
