using GenX.Cli.Core;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GenX.Cli.Infrastructure
{
    public sealed class DbModelMetadataReader : IMetadataReader
    {
        public IEnumerable<string> ReadNames(string filename)
        {
            var document = XDocument.Load(filename);
            var entities = document.Descendants(XName.Get(Constants.MetadataEntities, Constants.MetadataNamespaceName));

            foreach (var elemnent in entities.Elements())
            {
                yield return elemnent.Attribute("name").Value;
            }
        }
    }
}
