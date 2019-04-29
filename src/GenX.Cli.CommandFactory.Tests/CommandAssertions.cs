using GenX.Cli.Core.Commands;
using Xunit;

namespace GenX.Cli.CommandFactory.Tests
{
    public class CommandAssertions
    {
        private readonly ICommand _command;

        public CommandAssertions(ICommand command) => _command = command;

        public void Be<T>() => Assert.True(_command.GetType() == typeof(T));
    }
}
