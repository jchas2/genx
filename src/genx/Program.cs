using GenX.Cli.Core;
using GenX.Cli.Core.Commands;
using GenX.Cli.Infrastructure;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Infrastructure.Dotnet;
using GenX.Cli.Infrastructure.FileSystem;
using GenX.Cli.Infrastructure.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace genx
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ExitCode exitCode = ExitCode.Success;
            string environmentVariable = Environment.GetEnvironmentVariable("GENX_DEBUGONSTART");

            if (environmentVariable == "1")
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }
            }
            else if (environmentVariable == "2")
            {
                Console.ReadLine();
            }

            try
            {
                exitCode = ProcessCommandLineArgs(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                exitCode = ExitCode.Error;
            }

            return (int)exitCode;
        }

        private static ExitCode ProcessCommandLineArgs(string[] args)
        {
            var commandContext = new CommandContext();
            var outputWriter = new OutputWriter();
            var sqlClientSchemaReader = new SqlClientSchemaReader(outputWriter);
            var assemblyReader = new DotnetAssemblyReader(outputWriter);
            var assemblyMetadataWriter = new DotnetAssemblyMetadataWriter(outputWriter);
            var dbModelMetadataReader = new DbModelMetadataReader();
            var dbModelMetadataWriter = new DbModelMetadataWriter(outputWriter);
            var transformer = new Transformer(outputWriter);
            var directoryWrapper = new DirectoryWrapper();
            var fileWrapper = new FileWrapper();

            var factory = new CommandFactory(
                sqlClientSchemaReader,
                assemblyReader,
                dbModelMetadataWriter,
                assemblyMetadataWriter,
                dbModelMetadataReader,
                transformer,
                outputWriter,
                directoryWrapper,
                fileWrapper,
                commandContext);

            ICommand command = null;
            int argIndex = 0;

            for (; argIndex < args.Length; argIndex++)
            {
                if (IsValidArgument("h", "help", args[argIndex]) || 
                    IsValidArgument("?", args[argIndex]) || 
                    args[argIndex] == "?")
                {
                    return factory.Create("help")
                                  .Execute();
                }
                else if (IsValidArgument("nologo", args[argIndex]))
                {
                    commandContext.NoLogo = true;
                }
                else if (IsValidArgument("verbose", args[argIndex]))
                {
                    commandContext.VerboseLogging = true;
                    outputWriter.IsVerbose = true;
                }
                else if (IsValidArgument("v", "version", args[argIndex]))
                {
                    return factory.Create("version")
                                  .Execute();
                }
                else if (args[argIndex].StartsWith("-", StringComparison.CurrentCultureIgnoreCase))
                {
                    outputWriter.Error.WriteLine($"Unknown startup option: {args[argIndex]}");
                    return ExitCode.Error;
                }
                else
                {
                    string lastArg = args[argIndex];

                    if (string.IsNullOrEmpty(lastArg))
                    {
                        return factory.Create("help")
                                      .Execute();
                    }

                    List<string> commandArgs = (argIndex + 1) >= args.Length
                        ? new List<string>()
                        : args.Skip(argIndex + 1).ToList();

                    commandContext.CommandArgs = commandArgs;
                    PrintBanner(commandContext, outputWriter);
                    command = factory.Create(lastArg);
                    break;
                }
            }

            if (command == null)
            {
                command = factory.Create("help");
            }

            return command.Execute();
        }

        private static void PrintBanner(CommandContext commandContext, IOutputWriter outputWriter)
        {
            if (commandContext.NoLogo == false)
            {
                outputWriter.Output.WriteLine("G E N X  Code Generator");
                outputWriter.Output.WriteLine(
                    string.Format("Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString()));
            }

            if (commandContext.VerboseLogging)
            {
                outputWriter.Verbose.WriteLine();

                outputWriter.Verbose.WriteLine(
                    string.Format("Command Line           : {0}", Environment.CommandLine));

                outputWriter.Verbose.WriteLine(
                    string.Format("Current Directory      : {0}", Environment.CurrentDirectory));

                outputWriter.Verbose.WriteLine(
                    string.Format("64 Bit OS              : {0}", Environment.Is64BitOperatingSystem));

                outputWriter.Verbose.WriteLine(
                    string.Format("64 Bit Process         : {0}", Environment.Is64BitProcess));

                outputWriter.Verbose.WriteLine(
                    string.Format("Machine Name           : {0}", Environment.MachineName));

                outputWriter.Verbose.WriteLine(
                    string.Format("OS Version             : {0}", Environment.OSVersion));

                outputWriter.Verbose.WriteLine(
                    string.Format("Processor Count        : {0}", Environment.ProcessorCount));

                outputWriter.Verbose.WriteLine(
                    string.Format("System Directory       : {0}", Environment.SystemDirectory));

                outputWriter.Verbose.WriteLine(
                    string.Format("System Page Size       : {0}", Environment.SystemPageSize));

                outputWriter.Verbose.WriteLine(
                    string.Format("User Interactive       : {0}", Environment.UserInteractive));

                outputWriter.Verbose.WriteLine(
                    string.Format("User Name              : {0}", Environment.UserName));

                outputWriter.Verbose.WriteLine(
                    string.Format("Dotnet Runtime Version : {0}", Environment.Version));

                outputWriter.Verbose.WriteLine(
                    string.Format("Working Set            : {0}", Environment.WorkingSet));

                outputWriter.Verbose.WriteLine();
            }
        }

        private static bool IsValidArgument(string shortName, string longName, string arg) =>
            IsValidArgument(shortName, arg) ||
            IsValidArgument(longName, arg);

        private static bool IsValidArgument(string name, string arg) =>
             $@"-{name}".Equals(arg, StringComparison.CurrentCultureIgnoreCase) ||
             $@"--{name}".Equals(arg, StringComparison.CurrentCultureIgnoreCase);
    }
}

