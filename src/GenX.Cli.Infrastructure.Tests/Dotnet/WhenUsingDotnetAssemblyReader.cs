using FluentAssertions;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Infrastructure.Dotnet;
using GenX.Cli.Infrastructure.Tests.Assets;
using GenX.Cli.Tests.Utilities;
using GenX.Cli.Tests.Utilities.FileSystem;
using System.Reflection;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests.Dotnet
{
    public class WhenUsingDotnetAssemblyReader
    {
        [Fact]
        public void Should_Read_Test_Assembly()
        {
            using (var tempfile = new TempFile(".dll"))
            {
                //byte[] buffer = ManifestResourceStream.Get(
                //        Assembly.GetExecutingAssembly(), AssetsConstants.TestAssembly);

                //tempfile.WriteBuffer(buffer);

                //var assemblyReader = new DotnetAssemblyReader(new OutputWriter());
                //var model = assemblyReader.Read(tempfile.Filename, string.Empty);

                //model.Types.Count.Should().Be(2);

                //var internalStaticClass = model.Types[0];
                //internalStaticClass.Name.Should().Be("InternalStaticClass");
                //internalStaticClass.Constructors.Count.Should().Be(0);
                //internalStaticClass.Methods.Count.Should().Be(4);

                //internalStaticClass.Methods[0].Should().BeToString();
                //internalStaticClass.Methods[1].Should().BeEquals();
                //internalStaticClass.Methods[2].Should().BeGetHashCode();
                //internalStaticClass.Methods[3].Should().BeGetType();

                //var publicGenericClass = model.Types[1];
                //publicGenericClass.Name.Should().Be("PublicGenericClass");
                //publicGenericClass.Constructors.Count.Should().Be(1);
                //publicGenericClass.Constructors[0].Name.Should().Be(".ctor");
                //publicGenericClass.Constructors[0].Parameters[0].Name.Should().Be("variable");
                //publicGenericClass.Constructors[0].Parameters[0].CLRType.Should().Be("T");
            }
        }
    }
}
