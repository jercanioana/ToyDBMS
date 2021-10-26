using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.Connect();
            var MainMenuComponentUI = new MainMenuComponentUI(client);

        }
    }
}
