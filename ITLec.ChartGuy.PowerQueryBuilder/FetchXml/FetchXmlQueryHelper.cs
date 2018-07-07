using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder.FetchXml
{
  public  class FetchXmlQueryHelper
    {
        public static PowerQueryAttribute FormattedPowerQueryAttribute(PowerQueryAttribute powerQueryAttribute)
        {
            PowerQueryAttribute formatedPowerQueryAttribute = null;
            if (powerQueryAttribute.HasFormattedValue)
            {
                if ( powerQueryAttribute.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                {
                    var fieldGuidName = $"_{powerQueryAttribute.Name}_value";

                    var fieldNameFormated = $"{fieldGuidName}@OData.Community.Display.V1.FormattedValue";
                    formatedPowerQueryAttribute = new PowerQueryAttribute();
                    formatedPowerQueryAttribute.Name = fieldNameFormated;
                }
                else if(powerQueryAttribute.AttributeMetadata.AttributeType.ToString() != "String"   )
                {
                    var fieldNameFormated = $"{powerQueryAttribute.Name}@OData.Community.Display.V1.FormattedValue";
                    formatedPowerQueryAttribute = new PowerQueryAttribute();
                    formatedPowerQueryAttribute.Name = fieldNameFormated;
                }
            }

            if(formatedPowerQueryAttribute!=null)
            {

                formatedPowerQueryAttribute.Type = "FormattedValue";
                formatedPowerQueryAttribute.DisplayName = powerQueryAttribute.DisplayName + $"(Formatted Value)";
                formatedPowerQueryAttribute.ParentPowerQueryAttribute = powerQueryAttribute;
                formatedPowerQueryAttribute.FetchXmlAttributeDetail.IsVisable = true;
            }

            return formatedPowerQueryAttribute;
        }
        public static PowerQueryAttribute LogicalLookupPowerQueryAttribute(PowerQueryAttribute powerQueryAttribute)
        {
            PowerQueryAttribute formatedPowerQueryAttribute = null;
            if (powerQueryAttribute.HasFormattedValue && powerQueryAttribute.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
            {
                var lookupLogicalName = $"_{powerQueryAttribute.Name}_value@Microsoft.Dynamics.CRM.lookuplogicalname";
                formatedPowerQueryAttribute = new PowerQueryAttribute();
                formatedPowerQueryAttribute.Name = lookupLogicalName;
                formatedPowerQueryAttribute.Type = "LookupEntityLogicalName";
                formatedPowerQueryAttribute.DisplayName = powerQueryAttribute.AttributeMetadata.DisplayName.UserLocalizedLabel.Label +" (Type)";
                formatedPowerQueryAttribute.ParentPowerQueryAttribute = powerQueryAttribute;
            }

            return formatedPowerQueryAttribute;
        }
        public static PowerQueryAttribute LookupGuidPowerQueryAttribute(PowerQueryAttribute powerQueryAttribute)
        {
            PowerQueryAttribute transformedPowerQueryAttribute = null; ;
            if (powerQueryAttribute.HasFormattedValue && powerQueryAttribute.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata )
            {
                var lookupGUIDName = $"_{powerQueryAttribute.Name}_value";
                transformedPowerQueryAttribute = new PowerQueryAttribute();
                transformedPowerQueryAttribute.Name = lookupGUIDName;
                transformedPowerQueryAttribute.Type = "LookupGuid";
                transformedPowerQueryAttribute.DisplayName =  $"{powerQueryAttribute.AttributeMetadata.DisplayName.UserLocalizedLabel.Label} ({powerQueryAttribute.AttributeMetadata.LogicalName})";

                transformedPowerQueryAttribute.ParentPowerQueryAttribute = powerQueryAttribute;
            }

            return transformedPowerQueryAttribute;
        }
    }
}
