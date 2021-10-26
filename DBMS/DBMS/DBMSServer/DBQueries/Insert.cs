using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMSServer.DBQueries
{
    class Insert : Query
    {
        private string DatabaseName;
        private string TableName;
        private string RecordsString;
        private List<KeyValuePair<string, string>> Records;
        private List<ColumnInfo> ColumnsInfo;
        private MongoDBAcess MongoDB;
        private List<KeyValuePair<string, int>> PrimaryKeyPositions;


        public Insert(string _databaseName, string _tableName, string _records) : base(Commands.INSERT_INTO_TABLE)
        {
            DatabaseName = _databaseName;
            TableName = _tableName;
            RecordsString = _records;
            Records = new List<KeyValuePair<string, string>>();
            ColumnsInfo = new List<ColumnInfo>();
            PrimaryKeyPositions = new List<KeyValuePair<string, int>>();
        }

        public override void ParseAttributes()
        {
            var recordsArray = RecordsString.Split('|');

            // Create Key-Value pairs that will be added to the MongoDB 
            foreach (var newRecord in recordsArray)
            {
                var keyValuePair = newRecord.Split('*');

                // Special handling for tables with one column, where key = value bc I said so 
                if (keyValuePair.Length == 1)
                {
                    Records.Add(new KeyValuePair<string, string>(keyValuePair[0], keyValuePair[0]));
                }
                else
                {
                    Records.Add(new KeyValuePair<string, string>(keyValuePair[0], keyValuePair[1]));
                }
            }

            // Get the information about the table columns 
            var columnInfoString = DatabaseManager.FetchTableColumns(DatabaseName, TableName);
            foreach (var columnInfo in columnInfoString.Split(';')[1].Split('|'))
            {
                ColumnsInfo.Add(new ColumnInfo(columnInfo));
            }

            // Get a list of Primary Key names + the positions within the table structure of the PK column
            for (int idx = 0; idx < ColumnsInfo.Count; idx++)
            {
                if (ColumnsInfo[idx].PK)
                {
                    PrimaryKeyPositions.Add(new KeyValuePair<string, int>(ColumnsInfo[idx].ColumnName, idx));
                }
            }
            // Initialize MongoDB Access class
            MongoDB = new MongoDBAcess(DatabaseName);
        }

        public override void PerformXMLActions()
        {
            try
            {
                foreach (var keyValuePairs in Records)
                {

                    if (CheckDuplicatePK(keyValuePairs.Key))
                    {
                        throw new Exception("Table " + TableName + " already contains a record with Primary Key " + keyValuePairs.Key + "!");
                    }

                    // All checks have passed => Insert new record into all relevant files

                    InsertRecord(keyValuePairs.Key, keyValuePairs.Value);
                }
            }
            catch (Exception ex)
            {
                // Exception can come from different checks (PK, UK, FK, Index) => just throw back the message to the DatabaseManager
                throw new Exception(ex.Message);
            }
        }

        private bool CheckDuplicatePK(string key)
        {
            try
            {
                return MongoDB.CollectionContainsKey(TableName, key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private void InsertRecord(string key, string value)
        {
            try
            {
                // Insert into main table data file 
                MongoDB.InsertKVIntoCollection(TableName, key, value);

                // Insert into any index files 
                foreach (var indexFile in TableUtils.GetIndexFiles(DatabaseName, TableName))
                {
                    var indexKey = "";
                    var recordColumns = (key + '#' + value).Split('#');

                    // Build the key from the specified Index KV file 
                    foreach (var indexColumn in indexFile.IndexColumns)
                    {
                        indexKey += recordColumns[ColumnsInfo.FindIndex(elem => elem.ColumnName == indexColumn)] + '#';
                    }
                    indexKey = indexKey.Remove(indexKey.Length - 1);

                    if (indexFile.IsUnique)
                    {
                        MongoDB.InsertKVIntoCollection(indexFile.IndexFileName, indexKey, key);
                    }
                    else
                    {
                        if (MongoDB.CollectionContainsKey(indexFile.IndexFileName, indexKey))
                        {
                            // Append the new record PK to the value of the index key 
                            var indexValue = MongoDB.GetRecordValueWithKey(indexFile.IndexFileName, indexKey) + '#' + key;
                            MongoDB.RemoveKVFromCollection(indexFile.IndexFileName, indexKey);
                            MongoDB.InsertKVIntoCollection(indexFile.IndexFileName, indexKey, indexValue);
                        }
                        else
                        {
                            MongoDB.InsertKVIntoCollection(indexFile.IndexFileName, indexKey, key);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

