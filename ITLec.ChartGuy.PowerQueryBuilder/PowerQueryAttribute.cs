using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
   public  class PowerQueryAttribute
    {
        public PowerQueryAttribute ParentPowerQueryAttribute { get; set; }
        public  AttributeMetadata AttributeMetadata { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }


        public string Type
        {
            get;
            set;
        }
        
        private bool hasFormattedValue = false;
        public bool HasFormattedValue
        {
            get
            {
                bool retVal = false;
                if ((AttributeMetadata != null &&  !Name.StartsWith("_") && !Name.Contains(".") && !Name.Contains("@")
                && Type != "Uniqueidentifier" && Type != "String") 
                    
                    
                    || (hasFormattedValue == true))
                {
                    retVal = true;
                }
                return retVal;
            }
            set
            {
                hasFormattedValue = value;
            }
        }

        public FetchXmlAttributeDetail FetchXmlAttributeDetail {

            get
            {
                return new FetchXmlAttributeDetail(Name, DisplayName, Type, AttributeMetadata);
            }
        }
        public ODataAttributeDetail ODataAttributeDetail
        {
            get
            {
                return new ODataAttributeDetail(Name, DisplayName, Type, AttributeMetadata);
            }
        }
        private bool canBeAdded = false;
        public bool CanBeAdded
        {
            get
            {
                bool retVal = false;
                if((AttributeMetadata.AttributeType.ToString() != "Virtual") ||(canBeAdded == true ))
                {
                    retVal = true;
                }
                return retVal;
            }
            set
            {
                canBeAdded = value;
            }
        }
        public static PowerQueryAttribute GetPowerQueryAttributeByMetadata(AttributeMetadata attribute)
        {

            string displayName = attribute.LogicalName;

            if (attribute.DisplayName != null && attribute.DisplayName.UserLocalizedLabel != null && attribute.DisplayName.UserLocalizedLabel.Label != null)
            {

                displayName = attribute.DisplayName.UserLocalizedLabel.Label;
            }

            string itemName = attribute.LogicalName;
            PowerQueryAttribute powerQueryAttribute = new PowerQueryAttribute();
            powerQueryAttribute.Name = itemName;
            powerQueryAttribute.DisplayName = displayName;
            powerQueryAttribute.AttributeMetadata = attribute;


            if (attribute is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
            {
                Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata dateTimeAttribute = attribute as Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata;
                powerQueryAttribute.Type = dateTimeAttribute.Format.ToString();
            }
            else
            {
                powerQueryAttribute.Type = attribute.AttributeType.Value.ToString();
            }

            return powerQueryAttribute;
        }
        public static PowerQueryAttribute GetPowerQueryByListViewItem(ListViewItem listViewItem)
        {
            return (PowerQueryAttribute)listViewItem.Tag;
        }
        public static ListViewItem GetListViewItemByPowerQueryAttribute(PowerQueryAttribute powerQueryAttribute)
        {
            var selectedFieldItem = new ListViewItem { Text = powerQueryAttribute.DisplayName, Tag = powerQueryAttribute };
            selectedFieldItem.SubItems.Add(powerQueryAttribute.Name);
            selectedFieldItem.SubItems.Add(powerQueryAttribute.Type);
            return selectedFieldItem;
        }
        public static PowerQueryAttribute GetNewObject(PowerQueryAttribute powerQueryAttribute)
        {
            PowerQueryAttribute _powerQueryAttribute = new PowerQueryBuilder.PowerQueryAttribute();
            _powerQueryAttribute.AttributeMetadata = powerQueryAttribute.AttributeMetadata;
            _powerQueryAttribute.CanBeAdded = powerQueryAttribute.CanBeAdded;

            _powerQueryAttribute.Name = powerQueryAttribute.Name;
            _powerQueryAttribute.DisplayName = powerQueryAttribute.DisplayName;

            _powerQueryAttribute.HasFormattedValue = powerQueryAttribute.HasFormattedValue;

            if (powerQueryAttribute.ParentPowerQueryAttribute != null)
            {
                _powerQueryAttribute.ParentPowerQueryAttribute = PowerQueryAttribute.GetNewObject(powerQueryAttribute.ParentPowerQueryAttribute);
            }

            _powerQueryAttribute.Type = powerQueryAttribute.Type;


            return _powerQueryAttribute;
        }
    }

    public class FetchXmlAttributeDetail
    {
        public FetchXmlAttributeDetail(string name, string displayName,string type, AttributeMetadata attributeMetadata)
        {
            _AttributeMetadata = attributeMetadata;
            _Name = name;
            _DisplayName = displayName;
            _Type = type;
        }
        bool? isVisable;
        public bool? IsVisable
        {
            get
            {
                if(_Type == "Lookup" || _Type == "Customer" || _Type == "Owner")
                {
                    return false;
                }
                return isVisable;
            }
            set
            {
                isVisable = value;
            }
        }
        string _Type { get; set; }
        AttributeMetadata _AttributeMetadata { get; set; }
        string _Name { get; set; }
        string _DisplayName { get; set; }
        public string PowerBIType
        {

            get
            {
                if (_Name.ToLower().Contains("latitude") || _Name.ToLower().Contains("longitude"))
                    return "Currency.Type";

                string type = _Type;
                if (string.IsNullOrEmpty(type)&&_AttributeMetadata != null )
                {
                     type = _AttributeMetadata.AttributeType.ToString();
                }
                type = type.ToLower();
                    if (type.Contains("dateonly"))
                    {
                        return "type date";
                }
                else if (type.Contains("date"))
                {
                    return "type datetimezone";
                }
                else if (type == "money")
                {
                    return "type number";
                }
                else if (type.Contains("int"))
                    {
                        return "type number";
                    }
                    else if (type.Contains("number"))
                    {
                        return "Int64.Type";
                    }
                    else if (type.Contains("Option Set – Value"))
                    {
                        return "Int64.Type";
                    }
                


                return "type text";
            }
        }

        public string PowerBIExpandRecordColumnValue
        {
            get
            {
                string retVal = _Name;
                if (_AttributeMetadata != null && _AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                {
                    retVal = $"_{_Name}_value@OData.Community.Display.V1.FormattedValue";
                }

                return retVal;
            }
        }

        public string PowerBITransformColumnTypeValue//{"Owner (ownerid)", type text}

        {
            get
            {


                return string.Format(@"{{""{0}"", {1}}}", _DisplayName, PowerBIType);
            }
        }
        public string PowerBIRenameColumnsValue//{"_ownerid_value", "Owner (ownerid)"}

        {
            get
            {
                return string.Format(@"{{""{0}"", ""{1}""}}", PowerBIExpandRecordColumnValue, _DisplayName);
            }
        }


    }
    public class ODataAttributeDetail
    {
        public string PowerBISelectColumnValue
        {

            get
            {
                string retVal = _Name;
                if (_Type.ToLower() == "customer")
                {
                    retVal = string.Format(@"""{0}_account"",""{0}_contact""", retVal);
                }
                else
                {
                    retVal = string.Format(@"""{0}""", retVal);
                }

                return retVal;
            }
        }
        public string PowerBIRenameColumnValue
        {

            get
            {
                string retVal = _Name;
                if (_Type.ToLower() == "customer")
                {
                    retVal = string.Format(@"{{""{0}_account"",""{1}(Account)""}},{{""{0}_contact"",""{1}(Contact)""}}", _Name, _DisplayName);
                }
                else
                {
                    retVal = string.Format(@"{{""{0}"",""{1}""}}", _Name, _DisplayName);// string.Format(@"""{0}""", retVal);
                }

                return retVal;
            }
        }
        public ODataAttributeDetail(string name, string displayName,string type, AttributeMetadata attributeMetadata)
        {
            _AttributeMetadata = attributeMetadata;
            _Name = name;
            _Type = type;
            _DisplayName = displayName;
        }

        string _Type { get; set; }

        AttributeMetadata _AttributeMetadata { get; set; }
        string _Name { get; set; }
        string _DisplayName { get; set; }


        public string PowerBIType
        {

            get
            {
                if (_Name.ToLower().Contains("latitude") || _Name.ToLower().Contains("longitude"))
                    return "each Number.Round(_, 5), Currency.Type";

                string type = _Type;
                if (string.IsNullOrEmpty(type) && _AttributeMetadata != null)
                {
                    type = _AttributeMetadata.AttributeType.ToString();
                }

                type = type.ToLower();



                if (type.Contains("dateonly"))
                {
                    return "type date";
                }
                else if (type.Contains("date"))
                {
                    return "type datetimezone";
                }
                else if (type == "money")
                {
                    return "type number";
                }
                else if (type.Contains("int"))
                {
                    return "type number";
                }
                else if (type.Contains("number"))
                {
                    return "Int64.Type";
                }
                else if (type.Contains("Option Set – Value"))
                {
                    return "Int64.Type";
                }



                return "type text";
                return "type text";
            }
        }

        public string PowerBIExpandRecordColumnValue
        {
            get
            {

                return "todo";
                //string retVal = _Name;
                //if (_AttributeMetadata != null && _AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                //{
                //    retVal = $"_{_Name}_value@OData.Community.Display.V1.FormattedValue";
                //}

                //return retVal;
            }
        }

        public string PowerBITransformColumnTypeValue//{"Owner (ownerid)", type text}

        {
            get
            {


                return "todo";
                // return string.Format(@"{{""{0}"", {1}}}", _DisplayName, PowerBIType);
            }
        }


    }
}
