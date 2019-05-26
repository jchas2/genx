using GenX.Cli.Core;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public static class MethodExtensions
    {
        public static MethodAssertions Should(this Method method)
        {
            return new MethodAssertions(method);
        }
    }
}
