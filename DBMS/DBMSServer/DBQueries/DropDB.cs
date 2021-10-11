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
    class DropDB : Query
    {
        private string DatabaseName;
        public DropDB(string _queryAttributes) : base(Commands.DROP_DATABASE)
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
                    return Commands.MapCommandToSuccessResponse(QueryCommand);
            }
            return Responses.DROP_DATABASE_DOESNT_EXIST;
        }

        public override void PerformXMLActions()
        {
            try
            {
                MongoDBAcess.RemoveDatabase(DatabaseName);

                var xmlDocument = XDocument.Load(@"SGBDCatalog.xml");
                XElement deletedXMLTag = null;
                var databasesTags = xmlDocument.Element("Databases").Descendants("Database").ToArray();
                foreach (XElement tag in databasesTags)
                {
                    if (tag.Attribute("databaseName").Value.Equals(DatabaseName))
                    {
                        deletedXMLTag = tag;
                        break;
                    }
                }
                deletedXMLTag.Remove();
                xmlDocument.Save(@"SGBDCatalog.xml");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
