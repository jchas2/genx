using GenX.Cli.Core;
using GenX.Cli.Core.Commands.MetadataDotNet;
using GenX.Cli.Infrastructure.Console;
using Moq;
using System.Linq;
using System.Xml;
using Xunit;

namespace genx_metadata_dotnet.Tests
{
    public class GivenValidMetadataOptions
    {
        [Theory]
        [InlineData(new string[] {
            @"test.dll",
            @"\.test.xml" },
            ExitCode.Success)]
        public void When_Using_All_Available_Options(string[] args, ExitCode exitCode)
        {
            var assemblyReader = new Mock<IAssemblyReader>();
            var metadataWriter = new Mock<IMetadataWriter<AssemblyModel>>();
            var document = new Mock<XmlDocument>();

            assemblyReader.Setup(reader => reader.Read(
                It.IsAny<string>(), It.IsAny<string>())).Returns(new AssemblyModel());

            metadataWriter.Setup(writer => writer.Write(
                It.IsAny<AssemblyModel>())).Returns(document.Object);

            var command = new MetadataDotNetCommand(
                args.ToList(),
                assemblyReader.Object,
                metadataWriter.Object,
                new OutputWriter());

            var result = command.Execute();

            Assert.True(result == exitCode);
        }
    }
}
