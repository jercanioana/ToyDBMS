using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Commands
    {
        // Database Commands
        public const string CREATE_DATABASE = "CREATE_DATABASE";
        public const string DROP_DATABASE = "DROP_DATABASE";
        public const string GET_ALL_DATABASES = "GET_ALL_DATABASES";

        // Table Commands
        public const string CREATE_TABLE = "CREATE_TABLE";
        public const string GET_ALL_TABLES = "GET_ALL_TABLES";
        public const string DROP_TABLE = "DROP_TABLE";
        public const string GET_TABLE_INFORMATION = "GET_TABLE_INFORMATION";

        public const string CREATE_UNIQUE_INDEX = "CREATE_UNIQUE_INDEX";
        public const string DELETE_RECORD = "DELETE_RECORD";
        public const string INSERT_INTO_TABLE = "INSERT_INTO_TABLE";
        public static string MapCommandToSuccessResponse(string command)
        {
            switch (command)
            {
                case CREATE_DATABASE:
                    return Responses.CREATE_DATABASE_SUCCESS;
                case DROP_DATABASE:
                    return Responses.DROP_DATABASE_SUCCESS;
                case CREATE_TABLE:
                    return Responses.CREATE_TABLE_SUCCESS;
                case DROP_TABLE:
                    return Responses.DROP_TABLE_SUCCESS;
            }
            return "";
        }

    }

    public static class Responses
    {
        // General Responses
        public const string GENERAL_SERVER_ERROR = "Server disconnected due to internal error!";
        public const string GENERAL_DISPLAY_ENTRIES = "Entries retrieved successfully";

        // Database Responses
        public const string CREATE_DATABASE_SUCCESS = "Database created successfully";
        public const string CREATE_DATABASE_ALREADY_EXISTS = "A database with the same name already exists!";
        public const string DROP_DATABASE_SUCCESS = "Database deleted successfully!";
        public const string DROP_DATABASE_DOESNT_EXIST = "A database with the given name does not exist!";

        // Table Responses
        public const string CREATE_TABLE_SUCCESS = "Table created successfully!";
        public const string CREATE_TABLE_ALREADY_EXISTS = "A table with the given name alredy exists in the specified database!";
        public const string DROP_TABLE_SUCCESS = "Table deleted successfully!";
        public const string DROP_TABLE_REFERENCED = "Could not drop table because it is referenced by a FOREIGN KEY constraint!";

        public const string DELETE_RECORD_SUCCESS = "Deletion of records ended successfully!";
    }
}
