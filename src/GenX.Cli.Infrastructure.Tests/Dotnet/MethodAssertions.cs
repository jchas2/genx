using FluentAssertions;
using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public class MethodAssertions
    {
        private readonly Method _method;

        public MethodAssertions(Method method) => _method = method;

        public void BeToString()
        {
            _method.Name.Should().Be("ToString");
            _method.CLRType.Should().Be("System.String");
            _method.Parameters.Count.Should().Be(0);
        }

        public void BeEquals()
        {
            _method.Name.Should().Be("Equals");
            _method.CLRType.Should().Be("System.Boolean");
            _method.Parameters.Count.Should().Be(1);
            _method.Parameters[0].Name.Should().Be("obj");
        }

        public void BeGetHashCode()
        {
            _method.Name.Should().Be("GetHashCode");
            _method.CLRType.Should().Be("System.Int32");
            _method.Parameters.Count.Should().Be(0);
        }

        public void BeGetType()
        {
            _method.Name.Should().Be("GetType");
            _method.CLRType.Should().Be("System.Type");
            _method.Parameters.Count.Should().Be(0);
        }
    }
}

