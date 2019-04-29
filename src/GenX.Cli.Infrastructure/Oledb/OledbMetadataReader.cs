using GenX.Cli.Core;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GenX.Cli.Infrastructure.Oledb
{
    public class OledbMetadataReader : IMetadataReader
    {
        public IEnumerable<string> ReadEntityNames(string filename)
        {
            var document = XDocument.Load(filename);
            var entities = document.Descendants(XName.Get(Constants.MetadataLocalName, Constants.MetadataNamespaceName));

            foreach (var elemnent in entities.Elements())
            {
                yield return elemnent.Attribute("Name").Value;
            }
        }
    }
}
