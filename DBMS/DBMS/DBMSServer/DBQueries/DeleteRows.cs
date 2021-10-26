using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utils;

namespace DBMSServer.DBQueries
{
    class DeleteRows : Query
    {
        private string DatabaseName;
        private string TableName;
        private string RemovedKey;
        private List<string> ForeignKeyFiles;

        public DeleteRows(string _databaseName, string _tableName, string _removedKey) : base(Commands.DELETE_RECORD)
        {
            DatabaseName = _databaseName;
            TableName = _tableName;
            RemovedKey = _removedKey;
        }

        public override void ParseAttributes()
        {
            // nothing to parse here :) 
        }

        public override string ValidateQuery()
        {

            return Responses.DELETE_RECORD_SUCCESS;
        }

        public override void PerformXMLActions()
        {
            try
            {
                var mongoDB = new MongoDBAcess(DatabaseName);
                if (RemovedKey == "ALL")
                {
                    mongoDB.RemoveAllKVFromCollection(TableName);
                }
                else
                {
                    // Remove record PK from any Index File 
                    RemoveFromIndexFiles(mongoDB);

                    // Remove record from main table collection
                    mongoDB.RemoveKVFromCollection(TableName, RemovedKey);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void RemoveFromIndexFiles(MongoDBAcess mongoDB)
        {
            var indexFiles = TableUtils.GetIndexFiles(DatabaseName, TableName);

            foreach (var index in indexFiles)
            {
                if (index.IsUnique)
                {
                    // Entire KV pair needs to be removed from the file 
                    mongoDB.RemoveByValueFromCollection(index.IndexFileName, RemovedKey);
                }
                else
                {
                    // Only the current key needs to be removed from the Key-Value
                    mongoDB.RemoveValueFromCollection(index.IndexFileName, RemovedKey);
                }
            }
        }
    }
}

