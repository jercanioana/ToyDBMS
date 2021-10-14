using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMS
{
    class DeleteTableComponentUI
    {

        private Client tcpClient;
        public DeleteTableComponentUI(Client client)
        {
            tcpClient = client;
            buildComponent();
        }

        private void buildComponent()
        {
            string tableName = "";
            Console.WriteLine("Insert the name of the table: ");
            tableName = Console.ReadLine();
            this.DisplayQueryResult(Commands.DROP_TABLE, tableName);

        }

        private void DisplayQueryResult(string action, string name)
        {

            var tableName = name;

            if (tableName == "")
            {
                Console.WriteLine("Table name cannot be empty!");


            }
            else
            {
                tcpClient.Write(action + ";" + tableName);
                Console.WriteLine(tcpClient.ReadFromServer());

            }

        }
    }
}
