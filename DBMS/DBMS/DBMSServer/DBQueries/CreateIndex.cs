using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Xml;
using System.Xml.Linq;

namespace DBMSServer.DBQueries
{
    class CreateIndex : Query
    {
        private string DBName;
        private string TableName;
        private bool indexType;
        private string indexName;
        private string columnsString;
        private string[] columns;
        private List<KeyValuePair<string, int>> ColumnsPositions;
        private List<List<string>> RecordsSplit;
        private List<ColumnInfo> ColumnsInfo;

        public CreateIndex(string command, bool isUnique, string _queryAttributesDB, string _queryAttributesTB, string tableColumns, string idxName) : base(command)
        {
            indexType = isUnique;
            DBName = _queryAttributesDB;
            TableName = _queryAttributesTB;
            indexName = idxName;
            columnsString = tableColumns;
            ColumnsPositions = new List<KeyValuePair<string, int>>();
            RecordsSplit = new List<List<string>>();
            ColumnsInfo = new List<ColumnInfo>();
        }

        public override void ParseAttributes()
        {
            columns = columnsString.Split('|');
        }

        public override string ValidateQuery()
        {
            return Commands.MapCommandToSuccessResponse(base.QueryCommand);
        }

        public override void PerformXMLActions()
        {
            try
            {
                // Create corresponding MongoDB collection for the index 
                var mongoDB = new MongoDBAcess(DBName);

                if (mongoDB.CollectionExists(indexName))
                {
                    throw new Exception("An index for column combination: " + columnsString + " already exists!");
                }

                // If the table already has records => organize the contents in the new index file 
                if (mongoDB.CollectionHasDocuments(TableName))
                {
                    DetermineColumnsPositions(mongoDB);
                    if (indexType == true)
                    {
                        IndexRecordsUnique(mongoDB);
                    }
                    else
                    {
                        IndexRecordsNonUnique(mongoDB);
                    }
                }

                // Add the Index to the XML structure 
                var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");
                XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
                XElement givenDB = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(DBName));
                XElement[] databasesTables = givenDB.Descendants("Table").ToArray();
                XElement givenTable = Array.Find(databasesTables, elem => elem.Attribute("tableName").Value.Equals(TableName));
                XElement indexFilesNode = givenTable.Descendants("IndexFiles").ToArray()[0];
                XElement indexNode = new XElement("IndexFile", new XAttribute("isUnique", indexType), new XAttribute("indexName", indexName));

                foreach (var col in columns)
                {
                    indexNode.Add(new XElement("IndexAttribute", col));
                }

                indexFilesNode.Add(indexNode);
                xmlDocument.Save(@"SGBDCatalog.xml");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void IndexRecordsUnique(MongoDBAcess mongoDB)
        {
            List<KeyValuePair<string, string>> indexedContent = new List<KeyValuePair<string, string>>();
            List<string> indexKeys = new List<string>();

            for (int idx = 0; idx < RecordsSplit.Count(); idx++)
            {
                var indexKey = "";
                foreach (var indexColumn in columns)
                {
                    var columnValue = RecordsSplit[idx][ColumnsPositions.Find(elem => elem.Key == indexColumn).Value];
                    indexKey += columnValue + '#';
                }
                indexKey = indexKey.Remove(indexKey.Length - 1);

                var recordPK = "";
                foreach (var column in ColumnsInfo)
                {
                    if (column.PK)
                    {
                        recordPK += RecordsSplit[idx][ColumnsPositions.Find(elem => elem.Key == column.ColumnName).Value] + '#';
                    }
                }
                recordPK = recordPK.Remove(recordPK.Length - 1);


                if (indexKeys.Exists(elem => elem == indexKey))
                {
                    throw new Exception("Could not create unique Index for table: " + TableName + " due to duplicate values in columns: " + columnsString);
                }
                else
                {
                    indexKeys.Add(indexKey);
                    indexedContent.Add(new KeyValuePair<string, string>(indexKey, recordPK));
                }
            }

            try
            {
                foreach (var indexKeyValue in indexedContent)
                {
                    mongoDB.InsertKVIntoCollection(indexName, indexKeyValue.Key, indexKeyValue.Value);
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not index content for table: " + TableName);
            }

        }

        private void IndexRecordsNonUnique(MongoDBAcess mongoDB)
        {
            List<KeyValuePair<string, string>> indexedContent = new List<KeyValuePair<string, string>>();

            for (int idx = 0; idx < RecordsSplit.Count(); idx++)
            {
                var indexKey = "";
                foreach (var indexColumn in columns)
                {
                    var columnValue = RecordsSplit[idx][ColumnsPositions.Find(elem => elem.Key == indexColumn).Value];
                    indexKey += columnValue + '#';
                }
                indexKey = indexKey.Remove(indexKey.Length - 1);

                var recordPK = "";
                foreach (var column in ColumnsInfo)
                {
                    if (column.PK)
                    {
                        recordPK += RecordsSplit[idx][ColumnsPositions.Find(elem => elem.Key == column.ColumnName).Value] + '#';
                    }
                }
                recordPK = recordPK.Remove(recordPK.Length - 1);

                if (indexedContent.Exists(elem => elem.Key == indexKey))
                {
                    var indexKV = indexedContent.Find(elem => elem.Key == indexKey);
                    indexedContent.Remove(indexKV);
                    var indexValue = indexKV.Value + '#' + recordPK;
                    indexedContent.Add(new KeyValuePair<string, string>(indexKey, indexValue));
                }
                else
                {
                    indexedContent.Add(new KeyValuePair<string, string>(indexKey, recordPK));
                }
            }

            try
            {
                foreach (var indexKeyValue in indexedContent)
                {
                    mongoDB.InsertKVIntoCollection(indexName, indexKeyValue.Key, indexKeyValue.Value);
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not index content for table: " + TableName);
            }

        }

        private void DetermineColumnsPositions(MongoDBAcess mongoDB)
        {
            var columnInfoString = DatabaseManager.FetchTableColumns(DBName, TableName);
            foreach (var info in columnInfoString.Split(';')[1].Split('|'))
            {
                ColumnsInfo.Add(new ColumnInfo(info));
            }

            var columnInfo = DatabaseManager.FetchTableColumns(DBName, TableName).Split(';')[1].Split('|');
            for (int idx = 0; idx < columnInfo.Length; idx++)
            {
                ColumnsPositions.Add(new KeyValuePair<string, int>(columnInfo[idx].Split('#')[0], idx));
            }

            var tableContent = mongoDB.GetEntireCollection(TableName);
            foreach (var record in tableContent)
            {
                var keySplit = record.GetElement("_id").Value.ToString().Split('#');
                var valueSplit = record.GetElement("value").Value.ToString().Split('#');
                RecordsSplit.Add(keySplit.Concat(valueSplit).ToList());
            }
        }
    }
}

