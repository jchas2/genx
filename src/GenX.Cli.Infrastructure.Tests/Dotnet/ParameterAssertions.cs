using FluentAssertions;
using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public class ParameterAssertions
    {
        private readonly Parameter _parameter;

        public ParameterAssertions(Parameter parameter) => _parameter = parameter;

        public void Be(string prameterName, string clrType)
        {
            _parameter.Name.Should().Be(prameterName);
            _parameter.CLRType.Should().Be(clrType);
        }
    }
}
