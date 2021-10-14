using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMS
{
    class CreateTableComponentUI
    {
        private Client client;
        private String databaseName;

        public CreateTableComponentUI(Client client, string databaseName)
        {
            this.client = client;
            this.databaseName = databaseName;
            this.buildComponent();
        }


        private void buildComponent()
        {
            string tableName = "";
            Console.WriteLine("Insert the name of the table: ");
            tableName = Console.ReadLine();
            Console.WriteLine("Insert the number of attributes: ");
            int numberOfAttributes = Int32.Parse(Console.ReadLine());
            String[] attributes = { };
            
            for (int i = 0; i < numberOfAttributes; i++)
            {
                Console.WriteLine("Insert the name, type, PK and NULL for each attribute: ");
                string attributeDetails = Console.ReadLine();
                attributes[i] = attributeDetails;
            }
            var message = Commands.CREATE_TABLE + ";" + this.databaseName + "#" + tableName + "#";
            for (int i = 0; i < attributes.Length; i++)
            {
                String[] details = attributes[i].Split(' ');
                message += details[0] + "|" + details[2] + "|"
                               + details[1] + "|" + details[3] + "#";
            }

            client.Write(message);
            var serverResponse = client.ReadFromServer();
            if (serverResponse == Commands.MapCommandToSuccessResponse(Commands.CREATE_TABLE))
            {
                Console.WriteLine("Query Execution Result" + serverResponse);
                
            }
            else
            {
                Console.WriteLine("Query Execution Result" + serverResponse);
            }

        }

    }
}
