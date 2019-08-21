namespace GenX.Cli.Core.Commands.Help
{
    internal static class HelpText
    {
        public static readonly string Text =
$@"{StringResources.Usage}: genx [startup-options] [metadata-commands | generation-commands] [options]

{StringResources.StartupOptionsDescription}

startup-options:
    --help              {StringResources.HelpOptionDescription}
    --nologo            {StringResources.NoLogoDescription}
    --nologging|nl      {StringResources.NoLoggingDescription}
    --debug|d           {StringResources.DebugDescription}
    --version|v         {StringResources.VersionDescription}
    --verbose           {StringResources.VerboseDescription}

{StringResources.Usage}: genx [metadata-commands] [options] 

{StringResources.MetadataCommandDescription}

metadata-commands:
    metadata-oledb|oledb <connection-string> <filename>                     {StringResources.GenerateOledbDescription}
    metadata-sqlclient|sql <connection-string> <filename>                   {StringResources.GenerateSqlDescription}
    metadata-dotnet|dotnet <assembly-path> <filename> [<namespace-filter>]  {StringResources.GenerateDotnetDescription}  
    metadata-list <filename>                                                {StringResources.ListDescription}

{StringResources.Usage}: genx [generation-commands] [options]

generation-commands:
    generate|g [metadata-options] [xslt-options]

metadata-options:
    --metadata|md <path> 
    --filter|f <entity-name>

xslt-options:
    --xslt|x <path>
    --xsltparam|xp <paramname:paramvalue>
    --outputprefix|op <prefix>
    --outputsuffix|os <suffix>
    --outputextension|oe <extension>
    --outputdir|od <path>
";
    }
}
