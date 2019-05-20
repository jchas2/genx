using FluentAssertions;
using GenX.Cli.Core.Commands.Generate;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Infrastructure.Tests.Assets;
using GenX.Cli.Tests.Utilities;
using GenX.Cli.Tests.Utilities.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests
{
    public class WhenUsingTransformer
    {
        [Fact]
        public void Should_Transform_To_CSharp_File()
        {
            using (var tempXmlFile = new TempFile(".xml"))
            using (var tempXslFile = new TempFile(".xslt"))
            {
                string buffer = Encoding.UTF8.GetString(
                    ManifestResourceStream.Get(
                        Assembly.GetExecutingAssembly(), AssetsConstants.StandardDbMetadataFile));

                tempXmlFile.WriteAllText(buffer);

                buffer = Encoding.UTF8.GetString(
                    ManifestResourceStream.Get(
                        Assembly.GetExecutingAssembly(), AssetsConstants.TransformFile));

                tempXslFile.WriteAllText(buffer);

                var configuration = new Configuration()
                {
                    MetadataPath = tempXmlFile.Filename,
                    XsltPath = tempXslFile.Filename,
                    OutputDirectory = Path.GetDirectoryName(tempXmlFile.Filename),
                    OutputFileExtension = ".cs",
                    OutputFilename = "Products",
                    OutputFileNameSuffix = "Test",
                    Parameters = new List<Configuration.ParameterConfiguration>()
                    {
                        new Configuration.ParameterConfiguration() { Name = "EntityName", Value = "Products" },
                        new Configuration.ParameterConfiguration() { Name = "ApplicationName", Value = "Genx" }
                    }
                };

                var transformer = new Transformer(new OutputWriter());
                string outputPath = transformer.Transform(configuration);
                string[] output = File.ReadAllLines(outputPath);

                output[1].Should().Be("using System;");
                output[3].Should().Be("namespace Genx.Test");
                output[4].Should().Be("{");
                output[5].Should().Be("    public class ProductsTest");
                output[6].Should().Be("    {");
                output[7].Should().Be("        public ProductsTest(string message)");
                output[8].Should().Be("        {");
                output[9].Should().Be("            Console.WriteLine(\"Test!\");");
                output[10].Should().Be("        }");
                output[11].Should().Be("    }");
                output[12].Should().Be("}");
            }
        }
    }
}
