using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utils;

namespace DBMSServer
{
    class TableUtils
    {
        public static bool IsTableReferenced(string databaseName, string tableName)
        {
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            XElement givenDB = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(databaseName));

            var referencedTables = Array.FindAll(givenDB.Descendants("ReferencedTable").ToArray(), elem => elem.Value == tableName);
            if (referencedTables.Count() != 0)
            {
                // Table references exist
                return true;
            }

            return false;
        }


        public static List<IndexFileData> GetIndexFiles(string databaseName, string tableName)
        {
            var indexFiles = new List<IndexFileData>();
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement givenDB = Array.Find(xmlDocument.Element("Databases").Descendants("Database").ToArray(),
                                            elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement givenTable = Array.Find(givenDB.Descendants("Table").ToArray(),
                                            elem => elem.Attribute("tableName").Value == tableName);
            XElement[] indexFileNodes = givenTable.Descendants("IndexFiles").Descendants("IndexFile").ToArray();

            foreach (var indexFile in indexFileNodes)
            {
                var indexName = indexFile.Attribute("indexName").Value;
                var indexUnique = indexFile.Attribute("isUnique").Value;
                var indexColumns = new List<string>();

                foreach (var column in indexFile.Descendants("IndexAttribute").ToArray())
                {
                    indexColumns.Add(column.Value);
                }

                indexFiles.Add(new IndexFileData(indexName, bool.Parse(indexUnique), indexColumns));
            }

            return indexFiles;
        }

        public static List<string> GetUniqueFiles(string databaseName, string tableName)
        {
            var uniqueFiles = new List<string>();
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement givenDB = Array.Find(xmlDocument.Element("Databases").Descendants("Database").ToArray(),
                                            elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement givenTable = Array.Find(givenDB.Descendants("Table").ToArray(),
                                            elem => elem.Attribute("tableName").Value == tableName);
            XElement[] uniqueKeysNode = givenTable.Descendants("UniqueKeys").Descendants("UniqueKeyColumn").ToArray();

            foreach (var uniqueKey in uniqueKeysNode)
            {
                uniqueFiles.Add(uniqueKey.Attribute("fileName").Value);
            }

            return uniqueFiles;
        }

        public static List<string> GetPrimaryKey(string databaseName, string tableName)
        {
            var primaryKeys = new List<string>();
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement givenDB = Array.Find(xmlDocument.Element("Databases").Descendants("Database").ToArray(),
                                            elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement givenTable = Array.Find(givenDB.Descendants("Table").ToArray(),
                                            elem => elem.Attribute("tableName").Value == tableName);
            XElement[] primaryKeyNodes = givenTable.Descendants("PrimaryKey").Descendants("PrimaryKeyColumn").ToArray();

            foreach (var node in primaryKeyNodes)
            {
                primaryKeys.Add(node.Value);
            }

            return primaryKeys;
        }

        public static List<string> GetTableColumns(string databaseName, string tableName)
        {
            var columns = new List<string>();
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement givenDB = Array.Find(xmlDocument.Element("Databases").Descendants("Database").ToArray(),
                                            elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement givenTable = Array.Find(givenDB.Descendants("Table").ToArray(),
                                            elem => elem.Attribute("tableName").Value == tableName);
            XElement[] columnNodes = givenTable.Descendants("Structure").Descendants("Column").ToArray();

            foreach (var columnNode in columnNodes)
            {
                columns.Add(columnNode.Attribute("columnName").Value);
            }

            return columns;
        }
    }
}

