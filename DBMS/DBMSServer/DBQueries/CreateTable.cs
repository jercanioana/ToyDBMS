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
    public class CreateTable : Query
    {
        private string DatabaseName;
        private string TableName;
        private List<TableColumn> Columns;
        private string QueryAttributes;
        private MongoDBAcess MongoDB;

        public CreateTable(string _queryAttributes) : base(Commands.CREATE_TABLE)
        {
            QueryAttributes = _queryAttributes;
        }

        public override void ParseAttributes()
        {
            var tableAttributes = QueryAttributes.Split('#');
            DatabaseName = tableAttributes[0];
            TableName = tableAttributes[1];
            Columns = new List<TableColumn>();
            

            // Columns definition
            for (int idx = 2; idx < tableAttributes.Length - 1; idx++)
            {
                var columnAttributes = tableAttributes[idx].Split('|');
                var columnName = columnAttributes[0];
                var columnPK = bool.Parse(columnAttributes[2]);
                var columnType = columnAttributes[1];
               // int.TryParse(columnAttributes[3], out var columnLength);
                //var columnUnique = bool.Parse(columnAttributes[4]);
                var columnAllowNull = bool.Parse(columnAttributes[3]);
                Columns.Add(new TableColumn(columnName, columnPK, columnType, columnAllowNull));
            }

            MongoDB = new MongoDBAcess(DatabaseName);
        }

        public override string ValidateQuery()
        {
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");
            XElement givenDatabaseNode = null;
            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            for (int i = 0; i < databasesNodes.Length; i++)
            {
                if (databasesNodes[i].Attribute("databaseName").Value.Equals(DatabaseName))
                {
                    givenDatabaseNode = databasesNodes[i];
                    break;
                }
            }

            bool tableExists = false;
            XElement[] tableNodes = givenDatabaseNode.Descendants("Table").ToArray();
            for (int i = 0; i < tableNodes.Length; i++)
            {
                if (tableNodes[i].Attribute("tableName").Value.Equals(TableName))
                {
                    tableExists = true;
                    break;
                }
            }

            if (tableExists)
            {
                return Responses.CREATE_TABLE_ALREADY_EXISTS;
            }

            return Responses.CREATE_TABLE_SUCCESS;
        }

        public override void PerformXMLActions()
        {
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");
            XElement givenDatabaseNode = null;
            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            for (int i = 0; i < databasesNodes.Length; i++)
            {
                if (databasesNodes[i].Attribute("databaseName").Value.Equals(DatabaseName))
                {
                    givenDatabaseNode = databasesNodes[i];
                    break;
                }
            }

            XElement newTableNode = new XElement("Table");
            XElement structureNode = new XElement("Structure");
            XElement primaryKeyNode = new XElement("PrimaryKey");
            XElement indexFilesAttribute = new XElement("IndexFiles");
            newTableNode.Add(structureNode);
            newTableNode.Add(primaryKeyNode);
            newTableNode.Add(indexFilesAttribute);

            int rowLength = 0;
            foreach (TableColumn tableColumn in Columns)
            {
                XElement columnNode = new XElement("Column",
                                            new XAttribute("allowsNulls", tableColumn.AllowsNulls),
                                            new XAttribute("type", tableColumn.Type),
                                            new XAttribute("columnName", tableColumn.Name));

               /* if (tableColumn.Length != 0)
                {
                    columnNode.SetAttributeValue("length", tableColumn.Length);
                }

                structureNode.Add(columnNode);
                rowLength += tableColumn.Length;
                */
                if (tableColumn.IsPrimaryKey)
                {
                    XElement pkColumnNode = new XElement("PrimaryKeyColumn", tableColumn.Name);
                    primaryKeyNode.Add(pkColumnNode);
                }

                
            }

            

            newTableNode.SetAttributeValue("rowLength", rowLength);
            newTableNode.SetAttributeValue("fileName", TableName);
            newTableNode.SetAttributeValue("tableName", TableName);
            givenDatabaseNode.Add(newTableNode);
            xmlDocument.Save(@"SGBDCatalog.xml");

            // Create the correponding MongoDB Collection for the table 
            try
            {
                var mongoDB = new MongoDBAcess(DatabaseName);
                mongoDB.CreateCollection(TableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        

        

        public List<TableColumn> GetPrimaryKeysCurrentTable()
        {
            var primaryKeys = new List<TableColumn>();
            foreach (TableColumn tableColumn in Columns)
            {
                if (tableColumn.IsPrimaryKey)
                    primaryKeys.Add(tableColumn);
            }
            return primaryKeys;
        }

        public List<TableColumn> GetPrimaryKeysOfGivenTable(string tableName, XElement databaseNode)
        {
            var primaryKeys = new List<string>();

            XElement givenTable = null;
            XElement[] tableNodes = databaseNode.Descendants("Table").ToArray();
            for (int i = 0; i < tableNodes.Length; i++)
            {
                if (tableNodes[i].Attribute("tableName").Value.Equals(tableName))
                {
                    givenTable = tableNodes[i];
                    break;
                }
            }

            XElement[] primaryKeyNodes = givenTable.Descendants("PrimaryKey").Descendants("PrimaryKeyColumn").ToArray();
            foreach (XElement pkNode in primaryKeyNodes)
            {
                primaryKeys.Add(pkNode.Value);
            }

            List<TableColumn> primaryKeysObjects = new List<TableColumn>();
            XElement[] tableColumnsNodes = givenTable.Descendants("Structure").Descendants("Column").ToArray();
            foreach (XElement column in tableColumnsNodes)
            {
                var columnName = column.Attribute("columnName").Value;
                if (primaryKeys.Contains(columnName))
                {
                    var columnLength = column.Attribute("length").Value;
                    var columnType = column.Attribute("type").Value;
                    var columnAllowsNulls = column.Attribute("allowsNulls").Value;
                    primaryKeysObjects.Add(new TableColumn(columnName, false, columnType, bool.Parse(columnAllowsNulls)));
                }
            }
            return primaryKeysObjects;
        }
    }
}

