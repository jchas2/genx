using GenX.Cli.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenX.Cli.Infrastructure.SqlClient
{
    public class SqlClientSchemaReader : IDbSchemaReader
    {
        private readonly ISqlClientConnectionFactory _connectionFactory;
        private readonly IOutputWriter _outputWriter;

        public SqlClientSchemaReader(ISqlClientConnectionFactory connectionFactory, IOutputWriter outputWriter)
        {
            _connectionFactory = connectionFactory;
            _outputWriter = outputWriter;
        }

        public DbModel Read(string connectionString)
        {
            return MapDbModel(connectionString);
        }

        private DbModel MapDbModel(string connectionString)
        {
            _outputWriter.Output.WriteLine(StringResources.ReadingSqlClientMetadata);

            using (var connection = _connectionFactory.Create(connectionString))
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.OpeningDatabaseWithConnectionString, connectionString));

                connection.Open();

                _outputWriter.Verbose.WriteLine(StringResources.SucessfullyOpenedConnection);
                _outputWriter.Verbose.WriteLine(StringResources.ReadingSchemaColumns);

                var dbModel = new DbModel();
                var allColumnsSchemaTable = connection.GetSchema("Columns");

                var rows = allColumnsSchemaTable.AsEnumerable()
                                                .Select(info => new
                                                {
                                                    TableCatalog = info["TABLE_CATALOG"],
                                                    TableSchema = info["TABLE_SCHEMA"],
                                                    TableName = info["TABLE_NAME"],
                                                    ColumnName = info["COLUMN_NAME"],
                                                    DataType = info["DATA_TYPE"],
                                                    MaxLength = info["CHARACTER_MAXIMUM_LENGTH"],
                                                    IsNullable = info["IS_NULLABLE"]
                                                })
                                                .ToList();

                _outputWriter.Verbose.WriteLine(StringResources.DeterminingUniqueSchemas);

                var schemas = rows.Select(field => field.TableSchema.ToString())
                                  .Distinct()
                                  .ToList();

                foreach (var schema in schemas)
                {
                    _outputWriter.Verbose.WriteLine(
                        string.Format(
                            StringResources.ReadingSchema, schema));

                    _outputWriter.Verbose.WriteLine(StringResources.DeterminingUniqueEntities);

                    var entities = rows.Where(field => field.TableSchema.ToString() == schema)
                                       .Select(field => field.TableName.ToString())
                                       .Distinct()
                                       .ToList();

                    var allForeignKeys = new List<ForeignKey>();

                    entities.ForEach(entity => 
                        allForeignKeys.AddRange(
                            GetForeignKeys(entity, connection)));

                    foreach (var entity in entities)
                    {
                        _outputWriter.Output.WriteLine(
                            string.Format(StringResources.ReadingEntity, schema, entity));

                        var dbEntity = new DbEntity { Name = entity, Schema = schema };

                        var entityColumns = rows.Where(field => field.TableSchema.ToString() == schema && field.TableName.ToString() == entity)
                                                .ToList();

                        entityColumns.ForEach(e =>
                            dbEntity.Columns.Add(
                                new DbColumn
                                {
                                    Name = e.ColumnName.ToString(),
                                    DataType = e.DataType.ToString(),
                                    Size = e.MaxLength != DBNull.Value ? int.Parse(e.MaxLength.ToString()) : -1,
                                    IsNullable = false
                                }));

                        foreach (var column in dbEntity.Columns)
                        {
                            _outputWriter.Verbose.WriteLine(
                                FormatColumn(column));

                            var foreignKeys = allForeignKeys.Where(field => field.ForeignKeyEntity.Equals(entity, StringComparison.CurrentCultureIgnoreCase) &&
                                                                            field.ForeignKeyColumn.Equals(column.Name))
                                                            .ToList();

                            foreach (var fk in foreignKeys)
                            {
                                column.ForeignKeys.Add(fk);

                                _outputWriter.Verbose.WriteLine(
                                    string.Format($"    [{fk.ForeignKeyEntity}].[{fk.ForeignKeyColumn}] -> [{fk.PrimaryKeyEntity}].[{fk.PrimaryKeyColumn}]"));
                            }
                        }

                        dbEntity.PrimaryKeys.AddRange(
                            GetPrimaryKeys(entity, connection));

                        var relationShipKeys = allForeignKeys.Where(field => field.PrimaryKeyEntity.Equals(entity, StringComparison.CurrentCultureIgnoreCase))
                                                             .ToList();

                        dbEntity.Relationships.AddRange(relationShipKeys);
                        dbModel.Entities.Add(dbEntity);
                    }
                }

                return dbModel;
            }
        }

        private List<PrimaryKey> GetPrimaryKeys(string table, ISqlClientConnection connection)
        {
            _outputWriter.Verbose.WriteLine(
                string.Format(StringResources.ReadingPrimaryKeys, table));

            var restrictions = new string[4];
            restrictions[2] = table;

            var indexTable = connection.GetSchema("IndexColumns", restrictions);

            var primaryKeys = indexTable.AsEnumerable()
                                        .Where(info => info["constraint_name"].ToString().StartsWith("PK_"))
                                        .Select(info => new PrimaryKey
                                        {
                                            Entity = info["table_name"].ToString(),
                                            Name = info["column_name"].ToString()
                                            //TableSchema = info["table_schema"],
                                            //TableName = info["table_name"],
                                            //ColumnName = info["column_name"],
                                            //ConstraintSchema = info["constraint_schema"],
                                            //ConstraintName = info["constraint_name"],
                                            //KeyType = info["KeyType"]
                                        })
                                        .ToList();

            primaryKeys.ForEach(pk =>
                _outputWriter.Verbose.WriteLine(
                    string.Format($"  [{pk.Name}]")));

            return primaryKeys;
        }

        private List<ForeignKey> GetForeignKeys(string table, ISqlClientConnection connection)
        {
            _outputWriter.Verbose.WriteLine(
                string.Format(StringResources.ReadingForeignKeys, table));

            var foreignKeys = new List<ForeignKey>();

            var command = connection.CreateCommand();
            command.CommandText = string.Format("exec sp_fkeys '{0}'", table);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                foreignKeys.Add(new ForeignKey()
                {
                    PrimaryKeyEntity = reader["PKTABLE_NAME"].ToString(),
                    PrimaryKeyColumn = reader["PKCOLUMN_NAME"].ToString(),
                    ForeignKeyEntity = reader["FKTABLE_NAME"].ToString(),
                    ForeignKeyColumn = reader["FKCOLUMN_NAME"].ToString()
                });
            }

            reader.Close();

            foreignKeys.ForEach(fk =>
                _outputWriter.Verbose.WriteLine(
                    string.Format($"  [{fk.ForeignKeyEntity}].[{fk.ForeignKeyColumn}] -> [{fk.PrimaryKeyEntity}].[{fk.PrimaryKeyColumn}]")));

            return foreignKeys;
        }

        private string FormatColumn(DbColumn column)
        {
            string columnString = string.Format($"  [{column.Name}] [{column.DataType}]");

            if (column.Size.HasValue && column.Size.Value > 0)
            {
                columnString += string.Format($"({column.Size.Value})");
            }

            columnString += column.IsNullable
                ? " NULL"
                : " NOT NULL";

            return columnString;
        }
    }
}
