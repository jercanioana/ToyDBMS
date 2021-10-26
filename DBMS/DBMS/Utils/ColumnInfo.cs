using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ColumnInfo
    {
        public ColumnInfo(string command)
        {
            var columnStruct = command.Split('#');
            ColumnName = columnStruct[0];

            for (var i = 1; i < columnStruct.Length; i++)
            {
                switch (columnStruct[i])
                {
                    case "PK":
                        {
                            PK = true;
                        }
                        break;
                    case "NULL":
                        {
                            NonNull = true;
                        }
                        break;
                    default:
                        {
                            try
                            {
                                var types = columnStruct[i].Split('-');
                                Type = types[0];
                                int.TryParse(types[1], out int val);
                                Lenght = val;
                            }
                            catch (Exception)
                            {
                                Type = columnStruct[i];
                            }
                        }
                        break;
                }
            }
        }

        public string ColumnName { get; set; }
        public bool PK { get; set; } = false;
        public bool NonNull { get; set; } = false;
        public string Type { get; set; }
        public int Lenght { get; set; } = -1;
    }
}

