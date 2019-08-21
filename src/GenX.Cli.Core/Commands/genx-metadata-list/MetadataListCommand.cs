using System.Collections.Generic;
using System.Linq;

namespace GenX.Cli.Core.Commands.MetadataList
{
    public class MetadataListCommand : ICommand
    {
        private readonly List<string> _args;
        private readonly ITransformer _transformer;
        private readonly IMetadataReader _metadataReader;
        private readonly IFile _fileWrapper;
        private readonly IOutputWriter _outputWriter;

        public MetadataListCommand(
            List<string> args,
            IMetadataReader metadataReader,
            IFile fileWrapper,
            IOutputWriter outputWriter)
        {
            _args = args;
            _metadataReader = metadataReader;
            _fileWrapper = fileWrapper;
            _outputWriter = outputWriter;
        }

        public ExitCode Execute()
        {
            const int RequiredArgs = 1;

            if (_args.Count < RequiredArgs)
            {
                _outputWriter.Output.WriteLine(StringResources.HelpMetadataListParameters);
                return ExitCode.Error;
            }

            string filename = _args[0];

            if (!_fileWrapper.Exists(filename))
            {
                _outputWriter.Output.WriteLine(
                    string.Format(StringResources.MetadataFileDoesNotExist, filename));

                return ExitCode.Error;
            }

            var data = _metadataReader.ReadNames(filename)
                .OrderBy(str => str)
                .ToList();

            data.ForEach(str => _outputWriter.Output.WriteLine(str));

            return ExitCode.Success;
        }
    }
}
