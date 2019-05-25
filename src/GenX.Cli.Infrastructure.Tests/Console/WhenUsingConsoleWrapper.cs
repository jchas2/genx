using FluentAssertions;
using GenX.Cli.Infrastructure.Console;
using System.IO;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests.Console
{
    public class WhenUsingConsoleWrapper
    {
        [Fact]
        public void Should_Write_Single_Line()
        {
            const string Message = "Single Line Text";
            var writer = new StringWriter();
            var consoleWrapper = new ConsoleWrapper(writer);
            consoleWrapper.Write(Message);
            writer.ToString().Should().Be(Message);
        }
    }
}
