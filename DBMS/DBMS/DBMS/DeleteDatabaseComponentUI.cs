using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMS
{
    class DeleteDatabaseComponentUI
    {
        private Client tcpClient;
        public DeleteDatabaseComponentUI(Client client)
        {
            tcpClient = client;
            buildComponent();
        }

        private void buildComponent()
        {
            string databaseName = "";
            Console.WriteLine("Insert the name of the database: ");
            databaseName = Console.ReadLine();
            this.DisplayQueryResult(Commands.DROP_DATABASE, databaseName);

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
