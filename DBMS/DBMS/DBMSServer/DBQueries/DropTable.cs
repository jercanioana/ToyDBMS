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
    class DropTable : Query
    {
        private string DBName;
        private string TableName;
        private MongoDBAcess MongoDB;

        public DropTable(string _queryAttributesDB, string _queryAttributesTB) : base(Commands.DROP_TABLE)
        {
            DBName = _queryAttributesDB;
            TableName = _queryAttributesTB;
            MongoDB = new MongoDBAcess(DBName);
        }

        public override void ParseAttributes()
        {

        }

        public override void PerformXMLActions()
        {
            try
            {
                var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

                XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
                XElement givenDB = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(DBName));

                XElement[] databasesTables = givenDB.Descendants("Table").ToArray();
                XElement deletedXMLTag = Array.Find(databasesTables, elem => elem.Attribute("tableName").Value.Equals(TableName));

                // Delete the records from the table, stored in the MongoDB collection
                new DeleteRows(DBName, TableName, "ALL").Execute();

                

                
                XElement[] indexNodes = deletedXMLTag.Descendants("IndexFiles").Descendants("IndexFile").ToArray();
                foreach (var index in indexNodes)
                {
                    MongoDB.RemoveAllKVFromCollection(index.Attribute("indexName").Value);
                }

                MongoDB.RemoveCollection(TableName);
                // Delete the XML content for the table  
                deletedXMLTag.Remove();
                xmlDocument.Save(@"SGBDCatalog.xml");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
