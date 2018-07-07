using ITLec.ChartGuy.PowerQueryBuilder.FetchXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
    public partial class AttributeLookupForm : Form
    {
        LookupFormMessage attributeFormMessage = null;

     public AttributeFormResponse attributeFormResponse = null;
        public AttributeLookupForm(LookupFormMessage _attributeFormMessage)
        {
            InitializeComponent();
            attributeFormMessage = _attributeFormMessage;

            SetForm();
        }

        private void SetForm()
        {

            textBoxDisplayName.Text = attributeFormMessage.CurrentPowerQueryAttribute.DisplayName;

            var attributeMetadata = attributeFormMessage.CurrentPowerQueryAttribute.AttributeMetadata;


            if (attributeMetadata !=null)
            {
                if(attributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                {
                    groupBoxLookup.Visible = true;

                    checkBoxAddLookupGuid.Enabled = attributeFormMessage.CanAddLookupGuid;

                    checkBoxAddEntityLogicalName.Enabled = attributeFormMessage.CanAddLookupLogicalName;

                    checkBoxAddFormattedValue.Enabled = attributeFormMessage.CanAddFormattedValue;

                }
                else
                {
                    groupBoxLookup.Visible = false;
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            attributeFormResponse = new AttributeFormResponse();
            attributeFormResponse.CurrentPowerQueryAttribute = attributeFormMessage.CurrentPowerQueryAttribute;
            attributeFormResponse.CurrentPowerQueryAttribute.DisplayName = textBoxDisplayName.Name;
            if (checkBoxAddLookupGuid.Checked)
            {
                PowerQueryAttribute guidAttribute = FetchXmlQueryHelper.LookupGuidPowerQueryAttribute(attributeFormMessage.CurrentPowerQueryAttribute);// new PowerQueryAttribute();
                attributeFormResponse.NewFields.Add(guidAttribute);
            }
            if (checkBoxAddEntityLogicalName.Checked)
            {
                PowerQueryAttribute guidAttribute = FetchXmlQueryHelper.LogicalLookupPowerQueryAttribute(attributeFormMessage.CurrentPowerQueryAttribute);// new PowerQueryAttribute();
                attributeFormResponse.NewFields.Add(guidAttribute);
            }

            if (checkBoxAddFormattedValue.Checked)
            {
                PowerQueryAttribute formattedAttribute = FetchXmlQueryHelper.FormattedPowerQueryAttribute(attributeFormMessage.CurrentPowerQueryAttribute);
                //guidAttribute.Name = $"_{attributeFormMessage.CurrentPowerQueryAttribute.Name}_value@Microsoft.Dynamics.CRM.lookuplogicalname"; // $"_{attributeFormMessage.AttributeLogicName}_value";
                //guidAttribute.DisplayName = textBoxDisplayName.Text + " Enity Logical Name";
                //guidAttribute.Type = "EnityLogicalName";
                attributeFormResponse.NewFields.Add(formattedAttribute);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }


}
