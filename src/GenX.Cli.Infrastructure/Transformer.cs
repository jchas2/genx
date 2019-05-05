using GenX.Cli.Core;
using GenX.Cli.Core.Commands.Generate;
using System;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Xsl;

namespace GenX.Cli.Infrastructure
{
    public sealed class Transformer : ITransformer
    {
        private readonly IOutputWriter _outputWriter;

        public Transformer(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public void Transform(Configuration configuration)
        {
            var document = LoadMetadataDocument(configuration.MetadataPath);
            var transform = LoadTransform(configuration.XsltPath);

            var argumentList = new XsltArgumentList();
            var navigator = document.CreateNavigator();
            var outputFile = GetOutputFileName(configuration);

            _outputWriter.Verbose.WriteLine(
                string.Format(
                    StringResources.TransformConfig, 
                    configuration.OutputFilename,
                    configuration.XsltPath,
                    configuration.MetadataPath,
                    configuration.OutputDirectory,
                    Path.GetFileName(outputFile)));

            foreach (var param in configuration.Parameters)
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format("    {0}", 
                        string.Format(StringResources.Parameter, param.Name, param.Value)));

                argumentList.AddParam(param.Name, string.Empty, param.Value);
            }

            _outputWriter.Output.WriteLine(
                string.Format(StringResources.GeneratingOutputfile, outputFile));

            using (var writer = new StreamWriter(outputFile, false))
            {
                transform.Transform(navigator, argumentList, writer, null);
                writer.Flush();
            }

            _outputWriter.Verbose.WriteLine();
        }

        private string GetOutputFileName(Configuration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.OutputFileExtension) && !configuration.OutputFileExtension.StartsWith("."))
            {
                configuration.OutputFileExtension = "." + configuration.OutputFileExtension;
            }

            string outputFile = configuration.OutputFileNamePrefix + configuration.OutputFilename + configuration.OutputFileNameSuffix + configuration.OutputFileExtension;
            return Path.Combine(configuration.OutputDirectory, outputFile);
        }

        private XmlDocument LoadMetadataDocument(string filename)
        {
            _outputWriter.Verbose.WriteLine(
                string.Format(StringResources.LoadingMetadataFile, filename));

            try
            {
                var document = new XmlDocument();
                document.Load(filename);
                return document;
            }
            catch (Exception ex) when (ex is XmlException || ex is UnauthorizedAccessException || ex is NotSupportedException)
            {
                _outputWriter.Error.WriteLine(
                    string.Format(
                        StringResources.ErrorLoadingMetadataDocument,
                        filename,
                        ex.Message));

                return null;
            }
        }

        private XslTransform LoadTransform(string filename)
        {
            _outputWriter.Verbose.WriteLine(
                string.Format(StringResources.LoadingXsltFile, filename));

            try
            {
                var transform = new XslTransform();
                transform.Load(filename);
                return transform;
            }
            catch (Exception ex) when (ex is XsltCompileException || ex is SecurityException)
            {
                _outputWriter.Error.WriteLine(
                    string.Format(
                        StringResources.ErrorLoadingTransformDocument,
                        filename,
                        ex.Message));

                return null;
            }
        }
    }
}
