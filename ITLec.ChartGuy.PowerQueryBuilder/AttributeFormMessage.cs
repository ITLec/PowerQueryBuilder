using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
    public class AttributeFormMessage
    {

        public PowerQueryAttribute CurrentPowerQueryAttribute;
        public AttributeFormMessage(PowerQueryAttribute powerQueryAttribute)
        {
            CurrentPowerQueryAttribute = powerQueryAttribute;
        }
        public bool CanAddFormattedValue { get; set; }
    }
    public class LookupFormMessage : AttributeFormMessage
    {

        public LookupFormMessage(PowerQueryAttribute powerQueryAttribute):base(powerQueryAttribute)
        {
        }

        public bool CanAddLookupRelation { get; set; }
        public bool CanAddLookupGuid { get; set; }
        public bool CanAddLookupLogicalName { get; internal set; }
    }
}
