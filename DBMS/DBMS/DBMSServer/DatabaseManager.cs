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
                case Commands.CREATE_UNIQUE_INDEX:
                    {
                        executionResponse = new CreateIndex(commandSplit[0], true, commandSplit[1], commandSplit[2], commandSplit[3], commandSplit[4]).Execute();
                    }
                    break;

            }
            return executionResponse;
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
                    columnInfo += "#" + column.Attribute("type").Value;
                    columnInfo += "|";
                }
            }
            messageInfo = messageInfo + pkString + columnInfo;
            return messageInfo.Remove(messageInfo.Length - 1);
        }

    }
}
