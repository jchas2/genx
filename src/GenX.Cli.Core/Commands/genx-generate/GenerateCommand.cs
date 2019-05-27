using GenX.Cli.Core.Commands.Generate.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenX.Cli.Core.Commands.Generate
{
    public class GenerateCommand : ICommand
    {
        private readonly List<string> _args;
        private readonly ITransformer _transformer;
        private readonly IMetadataReader _metadataReader;
        private readonly IDirectory _directoryWrapper;
        private readonly IFile _fileWrapper;
        private readonly IOutputWriter _outputWriter;

        public GenerateCommand(
            List<string> args,
            ITransformer transformer,
            IMetadataReader metadataReader,
            IDirectory directoryWrapper,
            IFile fileWrapper,
            IOutputWriter outputWriter)
        {
            _args = args;
            _transformer = transformer;
            _metadataReader = metadataReader;
            _directoryWrapper = directoryWrapper;
            _fileWrapper = fileWrapper;
            _outputWriter = outputWriter;
        }

        public ExitCode Execute()
        {
            var configuration = ParseArguments();

            if (!ValidConfiguration(configuration))
            {
                configuration.Messages.Where(message => message.IsError == true)
                                      .ToList()
                                      .ForEach(message => _outputWriter.Error.WriteLine(message.Message));
                return ExitCode.Error;
            }

            SetPreconditions(configuration);

            var entities = string.IsNullOrEmpty(configuration.MetadataFilter)
                ? _metadataReader.ReadNames(configuration.MetadataPath).ToList()
                : _metadataReader.ReadNames(configuration.MetadataPath, configuration.MetadataFilter).ToList();

            DoTransform(entities, configuration);

            return ExitCode.Success;
        }

        private void DoTransform(List<string> entities, Configuration configuration)
        {
            foreach (var entity in entities)
            {
                configuration.OutputFilename = entity;

                var entityParam = configuration.Parameters.SingleOrDefault(param => 
                    param.Name.Equals("EntityName", StringComparison.CurrentCultureIgnoreCase));

                if (entityParam == null)
                {
                    configuration.Parameters.Add(new Configuration.ParameterConfiguration { Name = "EntityName", Value = entity });
                }
                else
                {
                    entityParam.Value = entity;
                }

                _transformer.Transform(configuration);                
            }
        }

        private void SetPreconditions(Configuration configuration)
        {
            if (string.IsNullOrEmpty(configuration.OutputDirectory))
            {
                configuration.OutputDirectory = _directoryWrapper.GetCurrentDirectory();
            }

            if (!_directoryWrapper.Exists(configuration.OutputDirectory))
            {
                _directoryWrapper.CreateDirectory(configuration.OutputDirectory);
            }
        }

        private Configuration ParseArguments()
        {
            var tokeniser = new Tokeniser();
            var tokens = tokeniser.Tokenise(_args).ToList();

            var parser = new GenerateCommandParser();
            var configuration = parser.Parse(tokens);

            return configuration;
        }

        private bool ValidConfiguration(Configuration configuration)
        {
            if (configuration.Messages.Any(message => message.IsError == true))
                return false;

            if (!_fileWrapper.Exists(configuration.MetadataPath))
            {
                configuration.Messages.Add(new Configuration.MessageInfo(
                    string.Format(StringResources.MetadataFileDoesNotExist, configuration.MetadataPath), true));
            }

            if (!_fileWrapper.Exists(configuration.XsltPath))
            {
                configuration.Messages.Add(new Configuration.MessageInfo(
                    string.Format(StringResources.TransformFileDoesNotExist, configuration.XsltPath), true));
            }

            if (string.IsNullOrEmpty(configuration.OutputFileExtension))
            {
                configuration.Messages.Add(new Configuration.MessageInfo(StringResources.OutputExtensionRequired, true));
            }

            return configuration.Messages.Count(message => message.IsError == true) == 0;
        }
    }
}
