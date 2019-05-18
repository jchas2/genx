using System;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests
{
    public class WhenUsingStringExtensions
    {
        [Fact]
        public void Should_Convert_To_Camel_Case()
        {
            string pascalCase = "PascalCaseString";
            string camelCase = pascalCase.ToCamelCase();
            Assert.True(camelCase.Equals("pascalCaseString", StringComparison.CurrentCulture));
        }
    }
}
