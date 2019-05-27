using GenX.Cli.Core;
using System.Xml;

namespace GenX.Cli.Infrastructure.Dotnet
{
    public class DotnetAssemblyMetadataWriter : BaseMetadataWriter, IMetadataWriter<AssemblyModel>
    {
        private readonly IOutputWriter _outputWriter;

        public DotnetAssemblyMetadataWriter(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public XmlDocument Write(AssemblyModel model)
        {
            _outputWriter.Output.WriteLine(StringResources.WritingMetadataAssemblyModelToXml);

            var document = new XmlDocument();
            var rootElement = CreateHeaderElement(document, this.GetType().ToString());

            document.AppendChild(
                document.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            document.AppendChild(rootElement);

            var databaseElement = CreateElement(document, "dataStructure", model.Name, Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);
            rootElement.AppendChild(databaseElement);

            var entitiesElement = CreateElement(document, Constants.MetadataEntities, Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);
            entitiesElement.Prefix = Constants.ElementNameSpacePrefix;

            CreateTypesXml(document, model, entitiesElement);
            databaseElement.AppendChild(entitiesElement);

            return document;
        }

        private void CreateTypesXml(XmlDocument document, AssemblyModel model, XmlElement parentElement)
        {
            foreach (var type in model.Types)
            {
                _outputWriter.Output.WriteLine(
                    string.Format(StringResources.WritingMetadataType, type.Name));

                parentElement.AppendChild(
                    CreateTypeXml(document, type));
            }
        }

        private XmlNode CreateTypeXml(XmlDocument document, TypeEntity type)
        {
            string originalName = type.Name;
            string entityName = type.Name.ToCompressedString();

            var typeNode = CreateElement(
                document,
                "type",
                entityName,
                Constants.ElementNameSpacePrefix,
                Constants.ElementNameSpace);

            typeNode.Attributes.Append(
                CreateAttribute(document, "namespace", type.Namespace));

            typeNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            typeNode.Attributes.Append(
                CreateAttribute(document, "camelcase", originalName.ToCamelCase()));

            var constructorsNode = typeNode.AppendChild(
                CreateElement(document, "constructors", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

            foreach(var constructor in type.Constructors)
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.WritingMetadataConstructor, constructor.Name));

                var constructorNode = CreateMethodXml(document, "constructor", constructor);

                var parametersNode = typeNode.AppendChild(
                    CreateElement(document, "parameters", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

                constructor.Parameters.ForEach(parm => parametersNode.AppendChild(
                    CreateParamterXml(document, parm)));

                constructorNode.AppendChild(parametersNode);
                constructorsNode.AppendChild(constructorNode);
            }

            typeNode.AppendChild(constructorsNode);

            var propertiesNode = typeNode.AppendChild(
                CreateElement(document, "properties", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

            foreach (var property in type.Properties)
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.WritingMetadataProperty, property.Name));

                var propertyNode = CreatePropertyXml(document, property);
                propertiesNode.AppendChild(propertyNode);
            }

            typeNode.AppendChild(propertiesNode);

            var methodsNode = typeNode.AppendChild(
                CreateElement(document, "methods", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

            foreach (var method in type.Methods)
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.WritingMetadataMethod, method.Name));

                var methodNode = CreateMethodXml(document, "method", method);

                var parametersNode = typeNode.AppendChild(
                    CreateElement(document, "parameters", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

                method.Parameters.ForEach(parm => parametersNode.AppendChild(
                    CreateParamterXml(document, parm)));

                methodNode.AppendChild(parametersNode);
                methodsNode.AppendChild(methodNode);
            }

            typeNode.AppendChild(methodsNode);
            return typeNode;
        }

        private XmlNode CreateMethodXml(XmlDocument document, string elementName, Method method)
        {
            string originalName = method.Name;
            string methodName = originalName.ToCompressedString();

            var methodNode = CreateElement(
                document, elementName,
                methodName,
                Constants.ElementNameSpacePrefix,
                Constants.ElementNameSpace);

            methodNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            methodNode.Attributes.Append(
                CreateAttribute(document, "label", methodName));

            methodNode.Attributes.Append(
                CreateAttribute(document, "camelcase",
                    originalName.Substring(0, 1).ToLower() +
                        originalName.Substring(1, originalName.Length - 1)));

            methodNode.Attributes.Append(
                CreateAttribute(document, "clrtype", method.CLRType));

            methodNode.Attributes.Append(
                CreateAttribute(document, "isabstract", method.IsAbstract.ToString()));

            methodNode.Attributes.Append(
                CreateAttribute(document, "isfinal", method.IsFinal.ToString()));

            methodNode.Attributes.Append(
                CreateAttribute(document, "ishidebysig", method.IsHideBySig.ToString()));

            methodNode.Attributes.Append(
                CreateAttribute(document, "isstatic", method.IsStatic.ToString()));

            methodNode.Attributes.Append(
                CreateAttribute(document, "isvirtual", method.IsVirtual.ToString()));

            return methodNode;
        }

        private XmlNode CreatePropertyXml(XmlDocument document, Property property)
        {
            string originalName = property.Name;
            string propertyName = originalName.ToCompressedString();

            var propertyNode = CreateElement(
                document, "property",
                propertyName,
                Constants.ElementNameSpacePrefix,
                Constants.ElementNameSpace);

            propertyNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            propertyNode.Attributes.Append(
                CreateAttribute(document, "label", propertyName));

            propertyNode.Attributes.Append(
                CreateAttribute(document, "camelcase",
                    originalName.Substring(0, 1).ToLower() +
                        originalName.Substring(1, originalName.Length - 1)));

            propertyNode.Attributes.Append(
                CreateAttribute(document, "clrtype", property.CLRType));

            propertyNode.Attributes.Append(
                CreateAttribute(document, "canread", property.CanRead.ToString()));

            propertyNode.Attributes.Append(
                CreateAttribute(document, "canwrite", property.CanWrite.ToString()));

            return propertyNode;
        }

        private XmlNode CreateParamterXml(XmlDocument document, Parameter parameter)
        {
            string originalName = parameter.Name;
            string parameterName = originalName.ToCompressedString();

            var parameterNode = CreateElement(
                document, 
                "parameter",
                parameterName,
                Constants.ElementNameSpacePrefix,
                Constants.ElementNameSpace);

            parameterNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "label", parameterName));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "camelcase",
                    originalName.Substring(0, 1).ToLower() +
                        originalName.Substring(1, originalName.Length - 1)));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "clrtype", parameter.CLRType));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "defaultvalue", parameter.DefaultValue));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "isretval", parameter.IsRetVal.ToString()));

            parameterNode.Attributes.Append(
                CreateAttribute(document, "position", parameter.Position.ToString()));

            return parameterNode;
        }
    }
}
