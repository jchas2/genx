﻿using GenX.Cli.Core.Commands.Generate;
using GenX.Cli.Core.Commands.Help;
using GenX.Cli.Core.Commands.MetadataDotNet;
using GenX.Cli.Core.Commands.MetadataList;
using GenX.Cli.Core.Commands.MetaDataOledb;
using GenX.Cli.Core.Commands.MetaDataSqlClient;
using GenX.Cli.Core.Commands.Version;
using System;
using System.Collections.Generic;

namespace GenX.Cli.Core.Commands
{
    public class CommandFactory
    {
        private readonly Dictionary<string, Func<ICommand>> _commands = new Dictionary<string, Func<ICommand>>();

        public CommandFactory(
            IDbSchemaReader dbSchemaReader,
            IAssemblyReader assemblyReader,
            IMetadataWriter<DbModel> dbModelMetadataWriter,
            IMetadataWriter<AssemblyModel> assemblyMetadataWriter,
            IMetadataReader metadataReader,
            ITransformer transformer,
            IOutputWriter outputWriter,
            IDirectory directoryWrapper,
            IFile fileWrapper,
            CommandContext context)
        {
            RegisterCommand("h", "help", () => new HelpCommand(outputWriter));
            RegisterCommand("v", "version", () => new VersionCommand(outputWriter));

            RegisterCommand("metadata-dotnet", () => new MetadataDotNetCommand(
                context.CommandArgs,
                assemblyReader,
                assemblyMetadataWriter,
                outputWriter));

            RegisterCommand("metadata-sqlclient", () => new MetadataSqlClientCommand(
                context.CommandArgs, 
                dbSchemaReader,
                dbModelMetadataWriter, 
                outputWriter));

            RegisterCommand("metadata-oledb", () => new MetadataOledbCommand());

            RegisterCommand("metadata-list", () => new MetadataListCommand(
                context.CommandArgs,
                metadataReader,
                fileWrapper,
                outputWriter));

            RegisterCommand("generate", () => new GenerateCommand(
                context.CommandArgs,
                transformer,
                metadataReader,
                directoryWrapper,
                fileWrapper,
                outputWriter));
        }

        public ICommand Create(string name)
        {
            ICommand command = null;

            if (_commands.TryGetValue(name, out Func<ICommand> function))
            {
                command = function.Invoke();
            }

            return command;
        }

        private void RegisterCommand(string shortName, string longName, Func<ICommand> function)
        {
            RegisterCommand(shortName, function);
            RegisterCommand(longName, function);
        }

        private void RegisterCommand(string name, Func<ICommand> function)
        {
            if (!_commands.ContainsKey(name))
            {
                _commands.Add(name, function);
            }
        }
    }
}

