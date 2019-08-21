using FluentAssertions;
using GenX.Cli.Core;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Tests.Utilities.FileSystem;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests
{
    public class WhenUsingDbModelMetadataWriter
    {
        private DbModel _model = new DbModel()
        {
            Entities = new List<DbEntity>()
            {
                new DbEntity()
                {
                    Name = "Category", Schema = "dbo", Columns = new List<DbColumn>()
                    {
                        new DbColumn() { Name = "CategoryId", DataType = "integer", IsNullable = false, Size = 4, ForeignKeys = new List<ForeignKey>() },
                        new DbColumn() { Name = "CategoryName", DataType = "nvarchar", IsNullable = false, Size = 255, ForeignKeys = new List<ForeignKey>() }
                    },
                    PrimaryKeys = new List<PrimaryKey>() { new PrimaryKey() { Name = "PK_CategoryId", Entity = "Category "} },
                    Relationships = new List<ForeignKey>() { new ForeignKey() { ForeignKeyEntity = "Product", ForeignKeyColumn = "CategoryId", PrimaryKeyEntity = "Category", PrimaryKeyColumn = "CategoryId" } }
                },
                new DbEntity()
                {
                    Name = "Product", Schema = "dbo", Columns = new List<DbColumn>()
                    {
                        new DbColumn() { Name = "ProductId", DataType = "integer", IsNullable = false, Size = 4, ForeignKeys = new List<ForeignKey>() },
                        new DbColumn() { Name = "ProductName", DataType = "nvarchar", IsNullable = false, Size = 255, ForeignKeys = new List<ForeignKey>() },
                        new DbColumn() { Name = "CategoryId", DataType = "integer", IsNullable = false, Size = 4, ForeignKeys = new List<ForeignKey>()
                            { new ForeignKey() { ForeignKeyColumn = "CategoryId", ForeignKeyEntity = "Product", PrimaryKeyColumn = "CategoryId", PrimaryKeyEntity = "Category" } } },
                    },
                    PrimaryKeys = new List<PrimaryKey>() { new PrimaryKey() { Name = "PK_Id", Entity = "Product" } },
                    Relationships = new List<ForeignKey>()
                },
            }
        };

        [Fact]
        public void Should_Write_Metadata_File()
        {
            using (var tempFile = new TempFile(".xml"))
            {
                var writer = new DbModelMetadataWriter(new OutputWriter());
                var document = writer.Write(_model);
                document.Save(tempFile.Filename);
                var lines = File.ReadAllLines(tempFile.Filename);

                lines[0].Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                lines[1].Should().Be("<mdr:metadataroot metadataprovider=\"GenX.Cli.Infrastructure.DbModelMetadataWriter\" freeform=\"true\" xmlns:mdr=\"http://genx.com/metadataroot\">");
                lines[2].Should().Be("  <md:dataStructure name=\"dbname\" xmlns:md=\"http://genx.com/metadata\">");
                lines[3].Should().Be("    <md:entities>");
                lines[4].Should().Be("      <md:entity name=\"Category\" originalname=\"Category\" camelcase=\"category\">");
                lines[5].Should().Be("        <md:entitycolumns>");
                lines[6].Should().Be("          <md:column name=\"CategoryId\" originalname=\"CategoryId\" label=\"CategoryId\" camelcase=\"categoryId\" datatype=\"integer\" maxlength=\"4\" allownulls=\"false\" isprimarykey=\"false\" />");
                lines[7].Should().Be("          <md:column name=\"CategoryName\" originalname=\"CategoryName\" label=\"CategoryName\" camelcase=\"categoryName\" datatype=\"nvarchar\" maxlength=\"255\" allownulls=\"false\" isprimarykey=\"false\" />");
                lines[8].Should().Be("        </md:entitycolumns>");
                lines[9].Should().Be("        <md:relationships>");
                lines[10].Should().Be("          <md:relationship name=\"Product.CategoryId_Category.CategoryId\" foreignkeyentity=\"Product\" foreignkeyentitycamelcase=\"product\" foreignkeycolumn=\"CategoryId\" foreignkeycolumncamelcase=\"categoryId\" />");
                lines[11].Should().Be("        </md:relationships>");
                lines[12].Should().Be("      </md:entity>");
                lines[13].Should().Be("      <md:entity name=\"Product\" originalname=\"Product\" camelcase=\"product\">");
                lines[14].Should().Be("        <md:entitycolumns>");
                lines[15].Should().Be("          <md:column name=\"ProductId\" originalname=\"ProductId\" label=\"ProductId\" camelcase=\"productId\" datatype=\"integer\" maxlength=\"4\" allownulls=\"false\" isprimarykey=\"false\" />");
                lines[16].Should().Be("          <md:column name=\"ProductName\" originalname=\"ProductName\" label=\"ProductName\" camelcase=\"productName\" datatype=\"nvarchar\" maxlength=\"255\" allownulls=\"false\" isprimarykey=\"false\" />");
                lines[17].Should().Be("          <md:column name=\"CategoryId\" originalname=\"CategoryId\" label=\"CategoryId\" camelcase=\"categoryId\" datatype=\"integer\" maxlength=\"4\" allownulls=\"false\" isprimarykey=\"false\">");
                lines[18].Should().Be("            <md:foreignkeys>");
                lines[19].Should().Be("              <md:foreignkey name=\"CategoryId\" primarykeyentity=\"Category\" primarykeycolumn=\"CategoryId\" />");
                lines[20].Should().Be("            </md:foreignkeys>");
                lines[21].Should().Be("          </md:column>");
                lines[22].Should().Be("        </md:entitycolumns>");
                lines[23].Should().Be("        <md:relationships />");
                lines[24].Should().Be("      </md:entity>");
                lines[25].Should().Be("    </md:entities>");
                lines[26].Should().Be("  </md:dataStructure>");
                lines[27].Should().Be("</mdr:metadataroot>");
            }
        }
    }
}

