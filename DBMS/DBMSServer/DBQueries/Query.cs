using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DBMSServer.DBQueries
{
    public abstract class Query
    {
        public string QueryCommand;

        public Query(string _queryType)
        {
            QueryCommand = _queryType;
        }

        public Query()
        {

        }

        public abstract void ParseAttributes();

        // methods return String instead of Bool because they're meant to return specific response codes (see class Responses from Utils)
        public virtual string ValidateQuery()
        {
            return Commands.MapCommandToSuccessResponse(QueryCommand);
        }

        public virtual void PerformXMLActions()
        {

        }

        public string Execute()
        {
            try
            {
                ParseAttributes();
                var validationResult = ValidateQuery();
                if (validationResult == Commands.MapCommandToSuccessResponse(QueryCommand))
                {
                    PerformXMLActions();
                }
                return validationResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
