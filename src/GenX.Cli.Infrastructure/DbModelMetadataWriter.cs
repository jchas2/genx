﻿using GenX.Cli.Core;
using System;
using System.Linq;
using System.Xml;

namespace GenX.Cli.Infrastructure
{
    public sealed class DbModelMetadataWriter : BaseMetadataWriter, IMetadataWriter<DbModel>
    {
        private readonly IOutputWriter _outputWriter;

        public DbModelMetadataWriter(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public XmlDocument Write(DbModel dbModel)
        {
            _outputWriter.Output.WriteLine(StringResources.WritingMetadataDbModelToXml);

            var document = new XmlDocument();
            var rootElement = CreateHeaderElement(document, this.GetType().ToString());

            document.AppendChild(
                document.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            document.AppendChild(rootElement);

            var databaseElement = CreateElement(document, "dataStructure", "dbname", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);
            rootElement.AppendChild(databaseElement);

            var entitiesElement = CreateElement(document, Constants.MetadataEntities, Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);
            entitiesElement.Prefix = Constants.ElementNameSpacePrefix;

            CreateEntitiesXml(document, dbModel, entitiesElement);
            databaseElement.AppendChild(entitiesElement);

            return document;
        }

        private void CreateEntitiesXml(XmlDocument document, DbModel dbModel, XmlElement entitiesElement)
        {
            foreach (var dbEntity in dbModel.Entities)
            {
                _outputWriter.Output.WriteLine(
                    string.Format(StringResources.WritingMetadataEntity, dbEntity.Name));

                entitiesElement.AppendChild(
                    CreateEntityXml(document, dbEntity));
            }
        }

        private XmlNode CreateEntityXml(XmlDocument document, DbEntity dbEntity)
        {
            string originalName = dbEntity.Name;
            string entityName = dbEntity.Name.ToCompressedString();

            var entityNode = CreateElement(
                document, 
                "entity", 
                entityName, 
                Constants.ElementNameSpacePrefix, 
                Constants.ElementNameSpace);

            entityNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            entityNode.Attributes.Append(
                CreateAttribute(document, "camelcase", entityName.ToCamelCase()));

            var columnsNode = entityNode.AppendChild(
                CreateElement(document, "entitycolumns", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace));

            foreach (var dbColumn in dbEntity.Columns)
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.WritingMetadataColumn, dbColumn.Name));

                var columnNode = CreateColumnXml(document, dbEntity, dbColumn);
                columnsNode.AppendChild(columnNode);

                if (dbColumn.ForeignKeys.Any())
                {
                    _outputWriter.Verbose.WriteLine(StringResources.WritingMetadataForeignKeys);

                    var foreignKeysNode = CreateElement(
                        document,
                        "foreignkeys",
                        Constants.ElementNameSpacePrefix,
                        Constants.ElementNameSpace);

                    columnNode.AppendChild(foreignKeysNode);

                    foreach (var foreignKey in dbColumn.ForeignKeys)
                    {
                        _outputWriter.Verbose.WriteLine(
                            string.Format($"  [{foreignKey.ForeignKeyEntity}].[{foreignKey.ForeignKeyColumn}] -> [{foreignKey.PrimaryKeyEntity}].[{foreignKey.PrimaryKeyColumn}]"));

                        var foreignKeyNode = CreateElement(document, "foreignkey", foreignKey.ForeignKeyColumn, Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);

                        foreignKeyNode.Attributes.Append(
                            CreateAttribute(document, "primarykeyentity", foreignKey.PrimaryKeyEntity));

                        foreignKeyNode.Attributes.Append(
                            CreateAttribute(document, "primarykeycolumn", foreignKey.PrimaryKeyColumn));

                        foreignKeysNode.AppendChild(foreignKeyNode);
                    }
                }
            }

            entityNode.AppendChild(columnsNode);

            var relationshipsNode = CreateElement(document, "relationships", Constants.ElementNameSpacePrefix, Constants.ElementNameSpace);
            entityNode.AppendChild(relationshipsNode);

            if (dbEntity.Relationships.Any())
            {
                _outputWriter.Verbose.WriteLine(StringResources.WritingMetadataRelationships);

                foreach (var relationshipKey in dbEntity.Relationships)
                {
                    string relationship = string.Format($"{relationshipKey.ForeignKeyEntity}.{relationshipKey.ForeignKeyColumn}_{relationshipKey.PrimaryKeyEntity}.{relationshipKey.PrimaryKeyColumn}");

                    _outputWriter.Verbose.WriteLine(relationship);

                    var relationshipNode = CreateElement(
                        document, 
                        "relationship", 
                        relationship, 
                        Constants.ElementNameSpacePrefix, 
                        Constants.ElementNameSpace);

                    relationshipNode.Attributes.Append(
                        CreateAttribute(document, "foreignkeyentity", relationshipKey.ForeignKeyEntity));

                    relationshipNode.Attributes.Append(
                        CreateAttribute(document, "foreignkeyentitycamelcase", relationshipKey.ForeignKeyEntity.ToCamelCase()));

                    relationshipNode.Attributes.Append(
                        CreateAttribute(document, "foreignkeycolumn", relationshipKey.ForeignKeyColumn));

                    relationshipNode.Attributes.Append(
                        CreateAttribute(document, "foreignkeycolumncamelcase", relationshipKey.ForeignKeyColumn.ToCamelCase()));

                    relationshipsNode.AppendChild(relationshipNode);
                }
            }

            return entityNode;
        }

        private XmlNode CreateColumnXml(XmlDocument document, DbEntity dbEntity, DbColumn dbColumn)
        {
            string originalName = dbColumn.Name;
            string columnName = originalName.ToCompressedString();

            var columnNode = CreateElement(
                document, "column", 
                columnName, 
                Constants.ElementNameSpacePrefix, 
                Constants.ElementNameSpace);

            columnNode.Attributes.Append(
                CreateAttribute(document, "originalname", originalName));

            columnNode.Attributes.Append(
                CreateAttribute(document, "label", columnName));

            columnNode.Attributes.Append(
                CreateAttribute(document, "camelcase",
                    columnName.Substring(0, 1).ToLower() +
                        columnName.Substring(1, columnName.Length - 1)));

            columnNode.Attributes.Append(
                CreateAttribute(document, "datatype", dbColumn.DataType));

            columnNode.Attributes.Append(
                CreateAttribute(document, "maxlength", dbColumn.Size.HasValue ? dbColumn.Size.Value.ToString() : ""));

            columnNode.Attributes.Append(
                CreateAttribute(document, "allownulls", dbColumn.IsNullable ? "true" : "false"));

            columnNode.Attributes.Append(
                CreateAttribute(document, "isprimarykey", 
                    dbEntity.PrimaryKeys.Any(pk => 
                        pk.Name.Equals(dbColumn.Name, StringComparison.CurrentCultureIgnoreCase)) ? "true" : "false"));

            // TODO: AutoIncrement Attribute. 
            //columnNode.Attributes.Append(CreateAttribute("IsAutoIncrement", "False"));

            return columnNode;
        }
    }
}
