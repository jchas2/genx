using GenX.Cli.Core.Commands;

namespace GenX.Cli.CommandFactory.Tests
{
    public static class CommandExtensions
    {
        public static CommandAssertions Should(this ICommand command)
        {
            return new CommandAssertions(command);
        }
    }
}
