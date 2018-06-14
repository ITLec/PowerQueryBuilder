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
    public partial class AttributeForm : Form
    {
        AttributeFormMessage attributeFormMessage = null;

     public   AttributeFormResponse attributeFormResponse = null;
        public AttributeForm(AttributeFormMessage _attributeFormMessage)
        {
            InitializeComponent();
            attributeFormMessage = _attributeFormMessage;

            SetForm();
        }

        private void SetForm()
        {

            textBoxDisplayName.Text = attributeFormMessage.CurrentAttributeDisplayName;

           var attributeMetadata = attributeFormMessage.EntityMetadataWithItems.Attributes.Where(e => e.LogicalName == attributeFormMessage.AttributeLogicName).FirstOrDefault();


            if (attributeMetadata !=null)
            {
                if(attributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                {
                    groupBoxLookup.Visible = true;

                    checkBoxAddLookupGuid.Enabled = attributeFormMessage.CanAddLookupGuid;

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
            attributeFormResponse.AttributeLogicName = attributeFormMessage.AttributeLogicName;
            attributeFormResponse.CurrentAttributeDisplayName = textBoxDisplayName.Text;
            if (checkBoxAddLookupGuid.Checked)
            {
                attributeFormResponse.NewFields.Add($"_{attributeFormMessage.AttributeLogicName}_value");
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
