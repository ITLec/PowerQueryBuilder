using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
  public   class AttributeFormMessage
    {
        public string AttributeLogicName { get; set; }
        public string CurrentAttributeDisplayName { get; set; }
        public EntityMetadata EntityMetadataWithItems { get; set; }

        public bool CanAddLookupRelation { get; set; }
        public bool CanAddLookupGuid { get; set; }
    }
}
