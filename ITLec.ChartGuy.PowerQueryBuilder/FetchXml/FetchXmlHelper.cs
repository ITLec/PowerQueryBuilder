using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ITLec.ChartGuy.PowerQueryBuilder.FetchXml
{
   public class FetchXmlHelper
    {

        public static string GetFilterXmlStr(string fetchXml)
        {
            if (string.IsNullOrEmpty(fetchXml))
            {
                return string.Empty;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fetchXml);
            XmlNode xmlNode = xmlDocument?.DocumentElement?.SelectSingleNode("/fetch/entity/filter");
            return xmlNode?.OuterXml ?? string.Empty;
        }
    }
}
