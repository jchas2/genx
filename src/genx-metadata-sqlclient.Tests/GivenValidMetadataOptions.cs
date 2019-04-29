using GenX.Cli.Core;
using GenX.Cli.Core.Commands.MetaDataSqlClient;
using GenX.Cli.Infrastructure.Console;
using Moq;
using System.Linq;
using System.Xml;
using Xunit;

namespace genx_metadata_sqlclient.Tests
{
    public class GivenValidMetadataOptions
    {
        [Theory]
        [InlineData(new string[] {
            "--metadata",
            @"Data Source=localhost;Initial Catalog=DB_NAME;Integrated Security=true;",
            @"\.metadata.xml" },
            ExitCode.Success)]
        public void When_Using_All_Available_Options(string[] args, ExitCode exitCode)
        {
            var schemaReader = new Mock<IDbSchemaReader>();
            var metadataWriter = new Mock<IMetadataWriter>();
            var document = new Mock<XmlDocument>();

            schemaReader.Setup(reader => reader.Read(
                It.IsAny<string>())).Returns(new DbModel());

            metadataWriter.Setup(writer => writer.WriteEntities(
                It.IsAny<DbModel>())).Returns(document.Object);

            var command = new MetadataSqlClientCommand(
                args.ToList(),
                schemaReader.Object,
                metadataWriter.Object,
                new OutputWriter());

            var result = command.Execute();

            Assert.True(result == exitCode);
        }
    }
}
