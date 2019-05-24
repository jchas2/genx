using FluentAssertions;
using GenX.Cli.Core;
using GenX.Cli.Infrastructure.Console;
using GenX.Cli.Infrastructure.SqlClient;
using Moq;
using System;
using System.Data;
using Xunit;

namespace GenX.Cli.Infrastructure.Tests.SqlClient
{
    public class WhenUsingSqlClientSchemaReader
    {
        [Fact]
        public void Should_Read_Sql_Schema()
        {
            var dataReaderMock = SetupNoForeignKeysDataReader();
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(command => command.ExecuteReader()).Returns(dataReaderMock.Object);

            var (columnsDataTable, indexesDataTable) = SetupSchemaTables();
            var connectionMock = new Mock<ISqlClientConnection>();
            connectionMock.Setup(conn => conn.GetSchema(It.Is<string>(str => str == "Columns"))).Returns(columnsDataTable);
            connectionMock.Setup(conn => conn.GetSchema(It.Is<string>(str => str ==  "IndexColumns"), It.IsAny<string[]>())).Returns(indexesDataTable);
            connectionMock.Setup(conn => conn.CreateCommand()).Returns(commandMock.Object);

            var connectionFactoryMock = new Mock<ISqlClientConnectionFactory>();
            connectionFactoryMock.Setup(factory => factory.Create(It.IsAny<string>())).Returns(connectionMock.Object);

            var schemaReader = new SqlClientSchemaReader(connectionFactoryMock.Object, new OutputWriter());
            var model = schemaReader.Read("Integrated Security=true;Initial Catalog=NorthWind;Server=Localhost;Persist Security Info=False");

            model.Entities.Count.Should().Be(1);

            var entity = model.Entities[0];
            entity.Name.Should().Be("Categories");
            entity.Schema.Should().Be("dbo");

            entity.Columns.Count.Should().Be(2);

            var categoryId = entity.Columns[0];
            categoryId.Name.Should().Be("CategoryId");
            categoryId.DataType.Should().Be("int");
            categoryId.Size.Should().Be(-1);
            categoryId.IsNullable.Should().Be(false);

            var categoryName = entity.Columns[1];
            categoryName.Name.Should().Be("CategoryName");
            categoryName.DataType.Should().Be("nvarchar");
            categoryName.Size.Should().Be(255);
            categoryName.IsNullable.Should().Be(false);

            entity.PrimaryKeys.Count.Should().Be(1);

            var primaryKey = entity.PrimaryKeys[0];
            primaryKey.Entity.Should().Be("Categories");
            primaryKey.Name.Should().Be("CategoryId");
        }

        private (DataTable, DataTable) SetupSchemaTables()
        {
            var columnsSchema = new DataTable();
            columnsSchema.Columns.AddRange(new DataColumn[] { new DataColumn("TABLE_CATALOG"), new DataColumn("TABLE_SCHEMA"), new DataColumn("TABLE_NAME"),
                new DataColumn("COLUMN_NAME"), new DataColumn("DATA_TYPE"), new DataColumn("CHARACTER_MAXIMUM_LENGTH"), new DataColumn("IS_NULLABLE") });

            columnsSchema.Rows.Add("Northwind", "dbo", "Categories", "CategoryId", "int", DBNull.Value, "NO");
            columnsSchema.Rows.Add("Northwind", "dbo", "Categories", "CategoryName", "nvarchar", 255, "NO");

            var indexesSchema = new DataTable();
            indexesSchema.Columns.AddRange(new DataColumn[] { new DataColumn("constraint_name"), new DataColumn("table_name"), new DataColumn("column_name") });

            indexesSchema.Rows.Add("PK_CategoryId", "Categories", "CategoryId");

            return (columnsSchema, indexesSchema);
        }

        private Mock<IDataReader> SetupNoForeignKeysDataReader()
        {
            var dataReaderMock = SetupDataReaderSchema();
            dataReaderMock.SetupSequence(reader => reader.Read())
                .Returns(false);

            return dataReaderMock;
        }

        private Mock<IDataReader> SetupDataReaderSchema()
        {
            string[] columns = { "PKTABLE_NAME", "PKCOLUMN_NAME", "FKTABLE_NAME", "FKCOLUMN_NAME" };
            var dataReaderMock = new Mock<IDataReader>();
            dataReaderMock.Setup(reader => reader.FieldCount).Returns(columns.Length);

            for (int i = 0; i < columns.Length; i++)
            {
                dataReaderMock.Setup(reader => reader.GetName(i)).Returns(columns[i]);
                dataReaderMock.Setup(reader => reader.GetFieldType(i)).Returns(typeof(string));
            }

            return dataReaderMock;
        }
    }
}
