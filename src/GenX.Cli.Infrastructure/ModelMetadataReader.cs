using GenX.Cli.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GenX.Cli.Infrastructure
{
    public sealed class ModelMetadataReader : IMetadataReader
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

        public IEnumerable<string> ReadNames(string filename, string filter)
        {
            var document = XDocument.Load(filename);
            var filteredElement = document.Descendants(XName.Get(Constants.MetadataEntities, Constants.MetadataNamespaceName))
                .SingleOrDefault(element => element.Attribute("name").Value.Equals(filter, StringComparison.CurrentCultureIgnoreCase));

            if (filteredElement != null)
            {
                yield return filteredElement.Attribute("name").Value;
            }
        }
    }
}
