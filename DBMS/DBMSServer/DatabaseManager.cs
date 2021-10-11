using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utils;
using DBMSServer.DBQueries;

namespace DBMSServer
{
    class DatabaseManager
    {
        public static string ExecuteCommand(string command)
        {
            var commandSplit = command.Split(';');
            var executionResponse = "";
            switch (commandSplit[0])
            {
                case Commands.GET_ALL_DATABASES:
                    {
                        executionResponse = FetchDatabases();
                    }
                    break;
                case Commands.CREATE_DATABASE:
                    {
                        executionResponse = new CreateDB(commandSplit[1]).Execute();
                    }
                    break;
                case Commands.DROP_DATABASE:
                    {
                        executionResponse = new DropDB(commandSplit[1]).Execute();
                    }
                    break;
                case Commands.CREATE_TABLE:
                    {
                        executionResponse = new CreateTable(commandSplit[1]).Execute();
                    }
                    break;
                case Commands.DROP_TABLE:
                    {
                        executionResponse = new DropTable(commandSplit[1], commandSplit[2]).Execute();
                    }
                    break;
                case Commands.GET_ALL_TABLES:
                    {
                        executionResponse = FetchTables(commandSplit[1]);
                    }
                    break;
                case Commands.CREATE_INDEX:
                    {
                        executionResponse = FetchColumns(commandSplit[1], commandSplit[2]);
                    }
                    break;
               
            }
            return executionResponse;
        }

        private static string FetchDatabases()
        {
            var databaseNames = Responses.GENERAL_DISPLAY_ENTRIES + ';';
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"SGBDCatalog.xml");
            XmlElement xmlRoot = xmlDocument.DocumentElement;

            foreach (XmlNode childNode in xmlRoot.ChildNodes)
            {
                databaseNames += childNode.Attributes.GetNamedItem("databaseName").Value + '|';
            }
            return databaseNames.Remove(databaseNames.Length - 1);
        }

        private static string FetchTables(string dbName)
        {
            var tableNames = Responses.GENERAL_DISPLAY_ENTRIES + ';';
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            XElement givenDB = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(dbName));

            XElement[] databasesTables = givenDB.Descendants("Table").ToArray();

            foreach (var table in databasesTables)
            {
                tableNames += table.Attribute("tableName").Value + "|";
            }

            return tableNames.Remove(tableNames.Length - 1);
        }

        private static string FetchColumns(string dbName, string tbName)
        {
            var columnNames = Responses.GENERAL_DISPLAY_ENTRIES + ';';
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            XElement givenDB = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(dbName));
            XElement[] databasesTables = givenDB.Descendants("Table").ToArray();
            XElement givenTable = Array.Find(databasesTables, elem => elem.Attribute("tableName").Value.Equals(tbName));
            XElement[] tableColumnsNodes = givenTable.Descendants("Structure").Descendants("Column").ToArray();

            foreach (var col in tableColumnsNodes)
            {
                columnNames += col.Attribute("columnName").Value + "|";
            }

            return columnNames.Remove(columnNames.Length - 1);
        }

        private static string FetchTableStructureInformation(string databaseName, string tableName)
        {
            var columnInfo = Responses.GENERAL_DISPLAY_ENTRIES + ';';
            var xmlDocument = XDocument.Load(@"GBDCatalog.xml");

            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            XElement givenDatabase = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement[] databasesTables = givenDatabase.Descendants("Table").ToArray();
            XElement givenTable = Array.Find(databasesTables, elem => elem.Attribute("tableName").Value.Equals(tableName));

            XElement[] tableColumnsNodes = givenTable.Descendants("Structure").Descendants("Column").ToArray();

            // Get the names of the columns that are primary keys
            var primaryKeyNames = new List<string>();
            XElement[] primaryKeyNodes = givenTable.Descendants("PrimaryKey").Descendants("PrimaryKeyColumn").ToArray();
            foreach (var primaryKey in primaryKeyNodes)
            {
                primaryKeyNames.Add(primaryKey.Value);
            }

            // Get column structure information
            foreach (var column in tableColumnsNodes)
            {
                columnInfo += column.Attribute("columnName").Value;
                if (primaryKeyNames.Contains(column.Attribute("columnName").Value))
                {
                    columnInfo += "(PK): ";
                }
                else
                {
                    columnInfo += ": ";
                }

                try
                {
                    columnInfo += column.Attribute("type").Value + "(" + column.Attribute("length").Value + "), ";
                }
                catch (System.NullReferenceException)
                {
                    columnInfo += column.Attribute("type").Value + ", ";
                }


                if (bool.Parse(column.Attribute("allowsNulls").Value))
                {
                    columnInfo += "Allows NULLs, ";
                }

               
                // Remove the last semicolon 
                columnInfo = columnInfo.Remove(columnInfo.Length - 2);
                columnInfo += "|";
            }

            // Get index information
            XElement[] indexNodes = givenTable.Descendants("IndexFiles").Descendants("IndexFile").ToArray();
            foreach (var indexFile in indexNodes)
            {
                columnInfo += indexFile.Attribute("indexName").Value + ": ";
                if (bool.Parse(indexFile.Attribute("isUnique").Value))
                {
                    columnInfo += "Unique Index" + "|";
                }
                else
                {
                    columnInfo += "Non-unique Index" + "|";
                }
            }
            // Remove the last separator
            return columnInfo.Remove(columnInfo.Length - 1);
        }

        public static string FetchTableColumns(string databaseName, string tableName)
        {
            string pkString = "";
            string columnInfo = "";
            var messageInfo = Responses.GENERAL_DISPLAY_ENTRIES + ';';
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");

            XElement[] databasesNodes = xmlDocument.Element("Databases").Descendants("Database").ToArray();
            XElement givenDatabase = Array.Find(databasesNodes, elem => elem.Attribute("databaseName").Value.Equals(databaseName));
            XElement[] databasesTables = givenDatabase.Descendants("Table").ToArray();
            XElement givenTable = Array.Find(databasesTables, elem => elem.Attribute("tableName").Value.Equals(tableName));

            XElement[] tableColumnsNodes = givenTable.Descendants("Structure").Descendants("Column").ToArray();

            // Get the names of the columns that are primary keys
            var primaryKeyNames = new List<string>();
            XElement[] primaryKeyNodes = givenTable.Descendants("PrimaryKey").Descendants("PrimaryKeyColumn").ToArray();
            foreach (var primaryKey in primaryKeyNodes)
            {
                primaryKeyNames.Add(primaryKey.Value);
            }

           

            // Get column structure information
            foreach (var column in tableColumnsNodes)
            {
                if (primaryKeyNames.Contains(column.Attribute("columnName").Value))
                {
                    pkString += column.Attribute("columnName").Value;
                    pkString += "#" + "PK";


                    if (bool.Parse(column.Attribute("allowsNulls").Value))
                    {
                        pkString += "#" + "NULL";
                    }

                    try
                    {
                        pkString += "#" + column.Attribute("type").Value + '-' + column.Attribute("length").Value;
                    }
                    catch (System.NullReferenceException)
                    {
                        pkString += "#" + column.Attribute("type").Value;
                    }
                    pkString += '|';
                }
                else
                {
                    columnInfo += column.Attribute("columnName").Value;


                    if (bool.Parse(column.Attribute("allowsNulls").Value))
                    {
                        columnInfo += "#" + "NULL";
                    }

                    try
                    {
                        columnInfo += "#" + column.Attribute("type").Value + '-' + column.Attribute("length").Value;
                    }
                    catch (System.NullReferenceException)
                    {
                        columnInfo += "#" + column.Attribute("type").Value;
                    }
                    columnInfo += "|";
                }
            }
            messageInfo = messageInfo + pkString + columnInfo;
            return messageInfo.Remove(messageInfo.Length - 1);
        }

    }
}
