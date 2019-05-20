using FluentAssertions;
using GenX.Cli.Tests.Utilities;
using GenX.Cli.Tests.Utilities.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests
{
    public class WhenUsingDbModelMetadataReader
    {
        private const string StandardDbMetadataFile = "GenX.Cli.Infrastructure.Tests.Assets.valid_metadata.xml";

        private const string BogusDbMetadataFile = 
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<bogusfile>
  <boguselement></boguselement>
</bogusfile>
";
        private const string BogusXmlFile = @"This is plain text";

        [Fact]
        public void Should_Read_Names_From_Db_Metadata_File()
        {
            using (var tempFile = new TempFile(".xml"))
            {
                string buffer = System.Text.Encoding.UTF8.GetString(
                    ManifestResourceStream.Get(
                        Assembly.GetExecutingAssembly(), StandardDbMetadataFile));

                tempFile.WriteAllText(buffer);
                var reader = new DbModelMetadataReader();
                var names = reader.ReadNames(tempFile.Filename).ToList();

                Assert.True(names.Count == 2);

                names[0].Should().Be("Products");
                names[1].Should().Be("Categories");
            }
        }

        [Fact]
        public void Should_Read_Zero_Names_From_Db_Metadata_File()
        {
            using (var tempFile = new TempFile(".xml"))
            {
                tempFile.WriteAllText(BogusDbMetadataFile);
                var reader = new DbModelMetadataReader();
                var names = reader.ReadNames(tempFile.Filename).ToList();

                Assert.True(names.Count == 0);
            }
        }

        [Fact]
        public void Should_Fail_To_Read_Bogus_Db_Metadata_File()
        {
            using (var tempFile = new TempFile(".xml"))
            {
                tempFile.WriteAllText(BogusXmlFile);
                var reader = new DbModelMetadataReader();

                var readNames = new Func<List<string>>(() =>
                    reader.ReadNames(tempFile.Filename).ToList());

                Assert.Throws<XmlException>(readNames);
            }
        }
    }
}
