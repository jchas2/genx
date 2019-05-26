using FluentAssertions;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Infrastructure.Dotnet;
using GenX.Cli.Infrastructure.Tests.Assets;
using GenX.Cli.Tests.Utilities;
using System.Reflection;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public class WhenUsingDotnetAssemblyReader
    {
        [Fact]
        public void Should_Read_Test_Assembly()
        {
            byte[] buffer = ManifestResourceStream.Get(
                    Assembly.GetExecutingAssembly(), AssetsConstants.TestAssembly);

            var assemblyReader = new DotnetAssemblyReader(new OutputWriter());
            var model = assemblyReader.Read(buffer, string.Empty);

            model.Types.Count.Should().Be(2);

            var internalStaticClass = model.Types[0];
            internalStaticClass.Name.Should().Be("InternalStaticClass");
            internalStaticClass.Constructors.Count.Should().Be(0);
            internalStaticClass.Methods.Count.Should().Be(4);

            internalStaticClass.Methods[0].Should().BeEquals();
            internalStaticClass.Methods[1].Should().BeGetHashCode();
            internalStaticClass.Methods[2].Should().BeGetType();
            internalStaticClass.Methods[3].Should().BeToString();

            var publicGenericClass = model.Types[1];
            publicGenericClass.Name.Should().Be("PublicGenericClassT`1");
            publicGenericClass.Constructors.Count.Should().Be(1);
            publicGenericClass.Constructors[0].Name.Should().Be(".ctor");
            publicGenericClass.Constructors[0].Parameters.Count.Should().Be(1);
            publicGenericClass.Constructors[0].Parameters[0].Name.Should().Be("variable");
            publicGenericClass.Constructors[0].Parameters[0].CLRType.Should().Be("T");

            publicGenericClass.Properties.Count.Should().Be(1);
            publicGenericClass.Properties[0].Name.Should().Be("PublicInstanceVariable");
            publicGenericClass.Properties[0].CLRType.Should().Be("T");
            publicGenericClass.Properties[0].CanRead.Should().Be(true);
            publicGenericClass.Properties[0].CanWrite.Should().Be(true);

            publicGenericClass.Methods.Count.Should().Be(6);

            internalStaticClass.Methods[0].Should().BeEquals();
            internalStaticClass.Methods[1].Should().BeGetHashCode();
            internalStaticClass.Methods[2].Should().BeGetType();

            publicGenericClass.Methods[3].Name.Should().Be("PublicInstanceCalculatePoint");
            publicGenericClass.Methods[3].CLRType.Should().Be("System.Drawing.Point");
            publicGenericClass.Methods[3].Parameters.Count.Should().Be(2);
            publicGenericClass.Methods[3].Parameters[0].Name.Should().Be("x");
            publicGenericClass.Methods[3].Parameters[0].CLRType.Should().Be("System.Int32");
            publicGenericClass.Methods[3].Parameters[1].Name.Should().Be("y");
            publicGenericClass.Methods[3].Parameters[1].CLRType.Should().Be("System.Int32");

            publicGenericClass.Methods[4].Name.Should().Be("PublicInstanceDoNothing");
            publicGenericClass.Methods[4].CLRType.Should().Be("System.Void");
            publicGenericClass.Methods[4].Parameters.Count.Should().Be(0);

            publicGenericClass.Methods[5].Should().BeToString();
        }
    }
}
