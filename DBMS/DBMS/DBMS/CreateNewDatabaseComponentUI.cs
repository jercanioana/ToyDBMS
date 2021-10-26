using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMS
{
    class CreateNewDatabaseComponentUI
    {
        private Client tcpClient;
        public CreateNewDatabaseComponentUI(Client client)
        {
            tcpClient = client;
            buildComponent();
        }

        private void buildComponent()
        {
            
            Console.WriteLine("Insert the name of the database: ");
            string databaseName = Console.ReadLine();
            this.DisplayQueryResult(Commands.CREATE_DATABASE, databaseName);

        }

        private void DisplayQueryResult(string action, string name)
        {

            var databaseName = name;

            if (databaseName == "")
            {
                Console.WriteLine("Database name cannot be empty!");
               
                
            }
            else
            {
                tcpClient.Write(action + ";" + databaseName);
                Console.WriteLine(tcpClient.ReadFromServer());
               
            }
           
        }
    }
}

