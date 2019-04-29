//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.OleDb;
//using System.Linq;
//using System.Xml;
//using GenX.Cli.Core;

//namespace GenX.Cli.Infrastructure.Oledb
//{
//    public class OledbMetadataWriter : IMetadataWriter
//    {
//        public static string RootDataElement = "mde:MetaDataRoot";
//        public static string RootElementNameSpacePrefix = "mde";
//        public static string RootElementNameSpace = "http://www.genx.com.au/MetaDataRoot";
//        public static string ElementNameSpacePrefix = "dbs";
//        public static string ElementNameSpace = "http://www.genx.com.au/Metadata";

//        private List<string> _nameSpaces = new List<string>();
//        private List<string> _nameSpacePrefixes = new List<string>();

//        private XmlDocument _metaDataDocument = new XmlDocument();
//        private List<Constraints> _allForeignKeys = new List<Constraints>();
//        private List<Constraints> _allPrimaryKeys = new List<Constraints>();

//        public OledbMetadataWriter()
//        {
//            _nameSpaces.Add(RootElementNameSpace);
//            _nameSpacePrefixes.Add(RootElementNameSpacePrefix);
//            _nameSpaces.Add(ElementNameSpace);
//            _nameSpacePrefixes.Add(ElementNameSpacePrefix);
//        }

//        public string ConnectionString { get; set; }

//        public XmlDocument WriteDocument()
//        {
//            using (var connection = OpenConnection())
//            {
//                _metaDataDocument = new XmlDocument();
//                XmlElement rootElement = CreateHeaderElement(this.GetType().ToString(), ElementNameSpacePrefix, ElementNameSpace);

//                _metaDataDocument.AppendChild(_metaDataDocument.CreateXmlDeclaration("1.0", "UTF-8", String.Empty));
//                _metaDataDocument.AppendChild(rootElement);

//                XmlElement databaseElement = CreateElement("DataStructure", connection.Database, ElementNameSpacePrefix, ElementNameSpace);
//                rootElement.AppendChild(databaseElement);

//                XmlElement tablesElement = CreateElement("Tables", ElementNameSpacePrefix, ElementNameSpace);
//                tablesElement.Prefix = ElementNameSpacePrefix;

//                DataTable tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
//                CreateTablesXml(connection, tables, _metaDataDocument, tablesElement);

//                DataTable views = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "VIEW" });
//                CreateTablesXml(connection, views, _metaDataDocument, tablesElement);

//                databaseElement.AppendChild(tablesElement);

//                return _metaDataDocument;
//            }
//        }

//        private void CreateTablesXml(OleDbConnection connection, DataTable table, XmlDocument document, XmlElement tablesElement)
//        {
//            foreach (DataRow tableRow in table.Rows)
//            {
//                string originalName = tableRow["TABLE_NAME"].ToString();
//                DataTable primaryKeys = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] { null, null, originalName });
//                GatherPrimaryKeys(primaryKeys);

//                DataTable foreignkeys = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, originalName });
//                GatherForeignKeys(foreignkeys);
//            }

//            foreach (DataRow tableRow in table.Rows)
//            {
//                tablesElement.AppendChild(CreateTableXml(connection, document, tableRow));
//            }
//        }

//        private void GatherPrimaryKeys(DataTable table)
//        {
//            foreach (DataRow tableRow in table.Rows)
//            {
//                _allPrimaryKeys.Add(new PrimaryKeyData
//                {
//                    PrimaryKeyTable = tableRow[2].ToString(),
//                    PrimaryKeyColumn = tableRow[3].ToString()
//                });
//            }
//        }

//        private void GatherForeignKeys(DataTable table)
//        {
//            foreach (DataRow tableRow in table.Rows)
//            {
//                _allForeignKeys.Add(new ForeignKeyData
//                {
//                    PrimaryKeyTable = tableRow[2].ToString(),
//                    PrimaryKeyColumn = tableRow[3].ToString(),
//                    ForeignKeyTable = tableRow[8].ToString(),
//                    ForeignKeyColumn = tableRow[9].ToString()
//                });
//            }
//        }

//        private XmlNode CreateTableXml(OleDbConnection connection, XmlDocument document, DataRow tableRow)
//        {
//            string originalName = tableRow["TABLE_NAME"].ToString();

//            string tableName = Compress(tableRow["TABLE_NAME"].ToString());

//            XmlNode tableNode = CreateElement("Table", tableName, ElementNameSpacePrefix, ElementNameSpace);

//            tableNode.Attributes.Append(CreateAttribute("OriginalName", originalName));
//            tableNode.Attributes.Append(CreateAttribute("CamelCase", originalName.Substring(0, 1).ToLower() + originalName.Substring(1, originalName.Length - 1)));

//            XmlNode columnsNode = tableNode.AppendChild(CreateElement("TableColumns", ElementNameSpacePrefix, ElementNameSpace));

//            DataTable columns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, originalName });

//            foreach (DataRow columnRow in columns.Rows)
//            {
//                string columnName = columnRow["COLUMN_NAME"].ToString();

//                bool isPrimaryKey = _allPrimaryKeys.FirstOrDefault(field => field.PrimaryKeyTable == originalName &&
//                                                                            field.PrimaryKeyColumn == columnName) != null;
//                var columnNode = CreateColumnXml(document, columnRow);
//                columnNode.Attributes.Append(CreateAttribute("IsPrimaryKey", isPrimaryKey.ToString()));
//                columnNode.Attributes.Append(CreateAttribute("AutoIncrement", false.ToString()));

//                columnsNode.AppendChild(columnNode);

//                var foreignKeys = _allForeignKeys.Where(field => field.ForeignKeyTable == originalName &&
//                                                                 field.ForeignKeyColumn == columnName)
//                                                 .ToList();

//                if (foreignKeys.Count > 0)
//                {
//                    XmlNode foreignKeysNode = CreateElement("ForeignKeys", ElementNameSpacePrefix, ElementNameSpace);
//                    columnNode.AppendChild(foreignKeysNode);

//                    foreach (var foreignKey in foreignKeys)
//                    {
//                        var foreignKeyNode = CreateElement("ForeignKey", foreignKey.ForeignKeyColumn, ElementNameSpacePrefix, ElementNameSpace);
//                        foreignKeyNode.Attributes.Append(CreateAttribute("PrimaryKeyTable", foreignKey.PrimaryKeyTable));
//                        foreignKeyNode.Attributes.Append(CreateAttribute("PrimaryKeyColumn", foreignKey.PrimaryKeyColumn));

//                        foreignKeysNode.AppendChild(foreignKeyNode);
//                    }
//                }
//            }

//            tableNode.AppendChild(columnsNode);

//            XmlNode relationshipsNode = CreateElement("Relationships", ElementNameSpacePrefix, ElementNameSpace);
//            tableNode.AppendChild(relationshipsNode);

//            var relationShipKeys = _allForeignKeys.Where(field => field.PrimaryKeyTable == originalName)
//                                                  .ToList();
//            if (relationShipKeys.Count > 0)
//            {
//                foreach (var relationshipKey in relationShipKeys)
//                {
//                    var relationshipNode = CreateElement("Relationship", string.Format("{0}.{1}_{2}.{3}", relationshipKey.ForeignKeyTable, relationshipKey.ForeignKeyTable, relationshipKey.PrimaryKeyTable, relationshipKey.PrimaryKeyColumn), ElementNameSpacePrefix, ElementNameSpace);
//                    relationshipNode.Attributes.Append(CreateAttribute("ForeignKeyTable", relationshipKey.ForeignKeyTable));
//                    relationshipNode.Attributes.Append(CreateAttribute("ForeignKeyColumn", relationshipKey.ForeignKeyColumn));

//                    relationshipsNode.AppendChild(relationshipNode);
//                }
//            }

//            return tableNode;
//        }

//        private XmlNode CreateColumnXml(XmlDocument document, DataRow columnRow)
//        {
//            string originalName = columnRow["COLUMN_NAME"].ToString();
//            string columnName = Compress(columnRow["COLUMN_NAME"].ToString());

//            XmlNode columnNode = CreateElement("TableColumn", columnName, ElementNameSpacePrefix, ElementNameSpace);

//            columnNode.Attributes.Append(CreateAttribute("OriginalName", originalName));

//            columnNode.Attributes.Append(CreateAttribute("Caption", columnName));

//            columnNode.Attributes.Append(CreateAttribute("CamelCase", originalName.Substring(0, 1).ToLower() + originalName.Substring(1, originalName.Length - 1)));

//            string SQLType = TranslateSQLType(columnRow["DATA_TYPE"].ToString());
//            columnNode.Attributes.Append(CreateAttribute("SQLType", SQLType));

//            if (columnRow["IS_NULLABLE"].ToString() == "True")
//                columnNode.Attributes.Append(CreateAttribute("AllowNulls", "true"));
//            else
//                columnNode.Attributes.Append(CreateAttribute("AllowNulls", "false"));

//            columnNode.Attributes.Append(CreateAttribute("MaxLength", columnRow["CHARACTER_MAXIMUM_LENGTH"].ToString()));

//            //if (isPrimaryKey)
//            //    columnNode.Attributes.Append(CreateAttribute("IsPrimaryKey", "True"));
//            //else
//            //    columnNode.Attributes.Append(CreateAttribute("IsPrimaryKey", "False"));

//            // TODO: AutoIncrement Attribute. 
//            //columnNode.Attributes.Append(CreateAttribute("IsAutoIncrement", "False"));

//            return columnNode;
//        }

//        private XmlElement CreateElement(string elementName, string elementPrefix, string elementNameSpace)
//        {
//            return _metaDataDocument.CreateElement(elementPrefix, elementName, elementNameSpace);
//        }

//        private XmlElement CreateElement(string elementName, string name, string elementPrefix, string elementNameSpace)
//        {
//            XmlElement element = _metaDataDocument.CreateElement(elementPrefix, elementName, elementNameSpace);
//            element.Attributes.Append(CreateAttribute("Name", name));
//            return element;
//        }

//        private XmlElement CreateHeaderElement(string type, string elementPrefix, string elementNameSpace)
//        {
//            XmlElement rootElement = _metaDataDocument.CreateElement(RootElementNameSpacePrefix, "MetaDataRoot", RootElementNameSpace);
//            rootElement.Attributes.Append(CreateAttribute("MetaDataProvider", type));
//            rootElement.Attributes.Append(CreateAttribute("FreeForm", "true"));
//            return rootElement;
//        }

//        private XmlAttribute CreateAttribute(string name, string value)
//        {
//            XmlAttribute attribute = _metaDataDocument.CreateAttribute(name);
//            attribute.Value = value;
//            return attribute;
//        }

//        private string TranslateSQLType(string dataType)
//        {
//            return Enum.Parse(typeof(System.Data.OleDb.OleDbType), dataType, true).ToString();
//        }

//        private string Compress(string stringToCompress)
//        {
//            string result = stringToCompress.Replace(" ", String.Empty);
//            result = result.Replace("_", String.Empty);
//            return result;
//        }

//        private OleDbConnection OpenConnection()
//        {
//            OleDbConnection connection = new OleDbConnection(ConnectionString);

//            try
//            {
//                connection.Open();
//            }
//            catch (Exception e)
//            {
//                Validation.ThrowInternalError(Constants.FailedToConnectToDatabase, e.Message);
//            }

//            return connection;
//        }
//    }
//}
