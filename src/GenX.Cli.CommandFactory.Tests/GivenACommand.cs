using GenX.Cli.Core;
using GenX.Cli.Core.Commands;
using GenX.Cli.Core.Commands.Generate;
using GenX.Cli.Core.Commands.Help;
using GenX.Cli.Core.Commands.MetaDataOledb;
using GenX.Cli.Core.Commands.MetaDataSqlClient;
using GenX.Cli.Core.Commands.Version;
using Moq;
using Xunit;
using CmdFactory = GenX.Cli.Core.Commands.CommandFactory;

namespace GenX.Cli.CommandFactory.Tests
{
    public class GivenACommand
    {
        private readonly CmdFactory _commandFactory;

        private readonly Mock<IDbSchemaReader> _dbSchemaReader;
        private readonly Mock<IMetadataWriter> _dbModelMetadataWriter;
        private readonly Mock<IMetadataReader> _dbModelMetadataReader;
        private readonly Mock<ITransformer> _transformer;
        private readonly Mock<IOutputWriter> _outputWriter;
        private readonly Mock<IDirectory> _directoryWrapper;
        private readonly Mock<IFile> _fileWrapper;
        private readonly Mock<CommandContext> _commandContext;

        public GivenACommand()
        {
            _dbSchemaReader = new Mock<IDbSchemaReader>();
            _dbModelMetadataWriter = new Mock<IMetadataWriter>();
            _dbModelMetadataReader = new Mock<IMetadataReader>();
            _transformer = new Mock<ITransformer>();
            _outputWriter = new Mock<IOutputWriter>();
            _directoryWrapper = new Mock<IDirectory>();
            _fileWrapper = new Mock<IFile>();
            _commandContext = new Mock<CommandContext>();

            _commandFactory = new CmdFactory(
                _dbSchemaReader.Object,
                _dbModelMetadataWriter.Object,
                _dbModelMetadataReader.Object,
                _transformer.Object,
                _outputWriter.Object,
                _directoryWrapper.Object,
                _fileWrapper.Object,
                _commandContext.Object);
        }

        [Fact]
        public void Should_Find_Command()
        {
            var cmd = _commandFactory.Create("generate");
            cmd.Should().Be<GenerateCommand>();

            cmd = _commandFactory.Create("help");
            cmd.Should().Be<HelpCommand>();

            cmd = _commandFactory.Create("metadata-oledb");
            cmd.Should().Be<MetadataOledbCommand>();

            cmd = _commandFactory.Create("metadata-sqlclient");
            cmd.Should().Be<MetadataSqlClientCommand>();

            cmd = _commandFactory.Create("version");
            cmd.Should().Be<VersionCommand>();
        }
    }
}
