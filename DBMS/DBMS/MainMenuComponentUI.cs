using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS
{
    class MainMenuComponentUI
    {

        private Client tcpClient;
        public MainMenuComponentUI(Client client)
        {
            tcpClient = client;
            buildComponent();
        }

        private void buildComponent()
        {
            int command;
            Console.WriteLine("1. Create new database.");
            Console.WriteLine("2. Create new table.");
            Console.WriteLine("3. Delete database.");
            Console.WriteLine("4. Delete table.");
            command = Int32.Parse(Console.ReadLine());
            while (true)
            {
                if(command == 1)
                {
                    var CreateNewDatabaseComponentUI = new CreateNewDatabaseComponentUI(tcpClient);
                }
                if (command == 2)
                {
                    var CreateNewTableComponentUI = new CreateTableComponentUI(tcpClient, "");
                }
                if (command == 3)
                {
                    var deleteDatabaseComponentUI = new DeleteDatabaseComponentUI(tcpClient);
                }
                if (command == 4)
                {
                    var deleteComponentComponentUI = new DeleteTableComponentUI(tcpClient);
                }
            }
        }

    }
}
