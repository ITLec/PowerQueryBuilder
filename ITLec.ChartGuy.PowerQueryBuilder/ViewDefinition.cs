using System;
using Microsoft.Xrm.Sdk;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
    internal class ViewDefinition
    {
        private readonly Entity record;

        public ViewDefinition(Entity viewEntity)
        {
            record = viewEntity;
        }

        public Guid Id => record.Id;

        public string Name => record.GetAttributeValue<string>("name");

        public int Type => record.GetAttributeValue<int>("querytype");

        public string LayoutXml
        {
            get
            {
                return record.GetAttributeValue<string>("layoutxml");
            }
            set { record["layoutxml"] = value; }
        }

        public string FetchXml
        {
            get
            {
                return record.GetAttributeValue<string>("fetchxml");
            }
            set
            {
                record["fetchxml"] = value;
            }
        }

        public string LogicalName => record.LogicalName;
    }
}
