using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Xml;
using System.Xml.Linq;

namespace DBMSServer.DBQueries
{
    public class CreateDB : Query
    {
        private string DatabaseName;
        public CreateDB(string _queryAttributes) : base(Commands.CREATE_DATABASE)
        {
            DatabaseName = _queryAttributes;
        }

        public override void ParseAttributes()
        {

        }

        public override string ValidateQuery()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"SGBDCatalog.xml");
            XmlElement xmlRoot = xmlDocument.DocumentElement;

            foreach (XmlNode childNode in xmlRoot.ChildNodes)
            {
                if (childNode.Attributes.GetNamedItem("databaseName").Value.Equals(DatabaseName))
                    return Responses.CREATE_DATABASE_ALREADY_EXISTS;
            }
            return Commands.MapCommandToSuccessResponse(QueryCommand);
        }

        public override void PerformXMLActions()
        {
            var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");
            XElement xmlElement = xmlDocument.Element("Databases");
            xmlElement.Add(new XElement("Database", new XAttribute("databaseName", DatabaseName)));
            xmlDocument.Save(@"SGBDCatalog.xml");
        }


    }
}

