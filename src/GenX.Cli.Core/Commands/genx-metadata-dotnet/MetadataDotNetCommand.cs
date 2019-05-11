using System.Collections.Generic;
using System.Xml;

namespace GenX.Cli.Core.Commands.MetadataDotNet
{
    public class MetadataDotNetCommand : ICommand
    {
        private readonly List<string> _args;
        private readonly IAssemblyReader _assemblyReader;
        private readonly IMetadataWriter<AssemblyModel> _metadataWriter;
        private readonly IOutputWriter _outputWriter;

        public MetadataDotNetCommand(
            List<string> args,
            IAssemblyReader assemblyReader,
            IMetadataWriter<AssemblyModel> metadataWriter,
            IOutputWriter outputWriter)
        {
            _args = args;
            _assemblyReader = assemblyReader;
            _metadataWriter = metadataWriter;
            _outputWriter = outputWriter;
        }

        public ExitCode Execute()
        {
            const int RequiredArgs = 2;

            if (_args.Count < RequiredArgs)
            {
                _outputWriter.Output.WriteLine(StringResources.HelpMetadataDotnetParameters);
                return ExitCode.Error;
            }

            string assemblyname = _args[0];
            string filename = _args[1];
            string namespaceFilter = _args.Count > RequiredArgs
                ? _args[2]
                : "*";

            var assemblyModel = _assemblyReader.Read(assemblyname, namespaceFilter);

            if (assemblyModel == null)
            {
                return ExitCode.Error;
            }

            var document = _metadataWriter.Write(assemblyModel);
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
