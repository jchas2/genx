using GenX.Cli.Core;
using GenX.Cli.Core.Commands.Generate;
using GenX.Cli.Infrastructure.Console;
using System.Linq;
using Xunit;
using Moq;
using System.Collections.Generic;

namespace genx_generate.Tests
{
    public class GivenValidGenerateOptions
    {
        [Theory]
        [InlineData(new string[] {
            "--metadata", @"\.metadata.xml",
            "--xslt", @".\repository.xslt",
            "--xsltparam", "Namespace:Acme.Repository", "--xsltparam", "Company:Acme",
            "--outputprefix", "I",
            "--outputsuffix", "Repository",
            "--outputExtension", ".cs",
            "--outputdir", @"\output" }, 
            ExitCode.Success)]
        [InlineData(new string[] {
            "--md", @"\.metadata.xml",
            "--x", @".\repository.xslt",
            "--xp", "Namespace:Acme.Repository", "--xp", "Company:Acme",
            "--op", "I",
            "--os", "Repository",
            "--oe", ".cs",
            "--od", @"\output" },
            ExitCode.Success)]
        public void When_Using_All_Available_Options(string[] args, ExitCode exitCode)
        {
            var directoryWrapper = new Mock<IDirectory>();
            var fileWrapper = new Mock<IFile>();
            var metadataReader = new Mock<IMetadataReader>();
            var transformer = new Mock<ITransformer>();

            fileWrapper.Setup(file => file.Exists(
                It.IsAny<string>())).Returns(true);

            directoryWrapper.Setup(dir => dir.Exists(
                It.IsAny<string>())).Returns(true);

            var entityNames = new List<string> { "entity1" };
            metadataReader.Setup(reader => reader.ReadEntityNames(It.IsAny<string>())).Returns(entityNames);

            var command = new GenerateCommand(
                args.ToList(), 
                transformer.Object, 
                metadataReader.Object, 
                directoryWrapper.Object, 
                fileWrapper.Object, 
                new OutputWriter());

            var result = command.Execute();

            Assert.True(result == exitCode);
        }
    }
}
