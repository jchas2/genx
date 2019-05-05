using System.Xml;

namespace GenX.Cli.Infrastructure
{
    public abstract class BaseMetadataWriter
    {
        protected XmlElement CreateElement(
            XmlDocument document,
            string elementName,
            string elementPrefix,
            string elementNameSpace) =>
                document.CreateElement(elementPrefix, elementName, elementNameSpace);

        protected XmlElement CreateElement(
            XmlDocument document,
            string elementName,
            string name,
            string elementPrefix,
            string elementNameSpace)
        {
            var element = document.CreateElement(elementPrefix, elementName, elementNameSpace);

            element.Attributes.Append(
                CreateAttribute(document, "name", name));

            return element;
        }

        protected XmlElement CreateHeaderElement(XmlDocument document, string type)
        {
            var rootElement = document.CreateElement(Constants.RootElementNameSpacePrefix, "metadataroot", Constants.RootElementNameSpace);

            rootElement.Attributes.Append(
                CreateAttribute(document, "metadataprovider", type));

            rootElement.Attributes.Append(
                CreateAttribute(document, "freeform", "true"));

            return rootElement;
        }

        protected XmlAttribute CreateAttribute(XmlDocument document, string name, string value)
        {
            var attribute = document.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }
    }
}
