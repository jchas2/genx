using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public static class ParameterExtensions
    {
        public static ParameterAssertions Should(this Parameter parameter)
        {
            return new ParameterAssertions(parameter);
        }
    }
}
