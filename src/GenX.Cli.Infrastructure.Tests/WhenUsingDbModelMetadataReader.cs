using FluentAssertions;
using GenX.Cli.Tests.Utilities.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests
{
    public class WhenUsingDbModelMetadataReader
    {
        private const string StandardDbMetadataFile =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<mdr:metadataroot metadataprovider = ""GenX.Cli.Infrastructure.DbModelMetadataWriter"" freeform=""true"" xmlns:mdr=""http://genx.com/metadataroot"">
  <md:dataStructure name = ""dbname"" xmlns:md=""http://genx.com/metadata"">
    <md:entities>
      <md:entity name = ""Products"" originalname=""Products"" camelcase=""products"">
        <md:entitycolumns>
          <md:column name = ""ProductID"" originalname=""ProductID"" label=""ProductID"" camelcase=""productID"" datatype=""int"" maxlength=""-1"" allownulls=""false"" isprimarykey=""false"" />
          <md:column name = ""ProductName"" originalname=""ProductName"" label=""ProductName"" camelcase=""productName"" datatype=""nvarchar"" maxlength=""40"" allownulls=""false"" isprimarykey=""false"" />
        </md:entitycolumns>
        <md:relationships />
      </md:entity>
      <md:entity name = ""Categories"" originalname=""Categories"" camelcase=""categories"">
        <md:entitycolumns>
          <md:column name = ""CategoryID"" originalname=""CategoryID"" label=""CategoryID"" camelcase=""categoryID"" datatype=""int"" maxlength=""-1"" allownulls=""false"" isprimarykey=""true"" />
          <md:column name = ""CategoryName"" originalname=""CategoryName"" label=""CategoryName"" camelcase=""categoryName"" datatype=""nvarchar"" maxlength=""15"" allownulls=""false"" isprimarykey=""false"" />
        </md:entitycolumns>
        <md:relationships>
          <md:relationship name = ""Products.CategoryID_Categories.CategoryID"" foreignkeyentity=""Products"" foreignkeycolumn=""CategoryID"" />
        </md:relationships>
      </md:entity>
    </md:entities>
  </md:dataStructure>
</mdr:metadataroot>
";

        private const string BogusDbMetadataFile = 
@"<?xml version=""1.0"" encoding=""utf-8""?>
<bogusfile>
    <boguselement></boguselement>
</bogusfile>
";

        private const string BogusXmlFile = @"
This is plain text
";

        [Fact]
        public void Should_Read_Names_From_Db_Metadata_File()
        {
            using (var tempFile = new TempFile(".xml"))
            {
                tempFile.WriteAllText(StandardDbMetadataFile);
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
