using System.Collections.Generic;
using System.Xml;

namespace GenX.Cli.Core.Commands.MetaDataSqlClient
{
    public class MetadataSqlClientCommand : ICommand
    {
        private readonly List<string> _args;
        private readonly IDbSchemaReader _schemaReader;
        private readonly IMetadataWriter _metadataWriter;
        private readonly IOutputWriter _outputWriter;

        public MetadataSqlClientCommand(
            List<string> args,
            IDbSchemaReader schemaReader,
            IMetadataWriter metadataWriter,
            IOutputWriter outputWriter)
        {
            _args = args;
            _schemaReader = schemaReader;
            _metadataWriter = metadataWriter;
            _outputWriter = outputWriter;
        }

        public ExitCode Execute()
        {
            const int RequiredArgs = 2;

            if (_args.Count < RequiredArgs)
            {
                _outputWriter.Output.WriteLine(StringResources.HelpMetadataDbParameters);
                return ExitCode.Error;
            }

            string connectionString = _args[0];
            string filename = _args[1];

            var dbModel = _schemaReader.Read(connectionString);
            if (dbModel == null)
            {
                return ExitCode.Error;
            }

            var document = _metadataWriter.WriteEntities(dbModel);
            if (document == null)
            {
                return ExitCode.Error;
            }

            try
            {
                document.Save(filename);
            }
            catch (XmlException ex)
            {
                _outputWriter.Output.WriteLine(
                    string.Format(StringResources.ErrorSavingMetadataFile, filename, ex.Message));

                return ExitCode.Error;
            }

            return ExitCode.Success;
        }
    }
}

