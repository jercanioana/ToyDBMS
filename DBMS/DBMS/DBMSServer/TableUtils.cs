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
    }
}
