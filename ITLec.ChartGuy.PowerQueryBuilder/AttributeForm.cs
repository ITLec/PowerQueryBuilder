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

            textBoxDisplayName.Text = attributeFormMessage.CurrentPowerQueryAttribute.DisplayName;
            checkBoxAddFormattedValue.Enabled = attributeFormMessage.CanAddFormattedValue;
            var attributeMetadata = attributeFormMessage.CurrentPowerQueryAttribute.AttributeMetadata;

            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            attributeFormResponse = new AttributeFormResponse();
            attributeFormResponse.CurrentPowerQueryAttribute = attributeFormMessage.CurrentPowerQueryAttribute;

            attributeFormResponse.CurrentPowerQueryAttribute.DisplayName = textBoxDisplayName.Text;
            if (checkBoxAddFormattedValue.Checked)
            {
                attributeFormResponse.NewFields.Add(FetchXmlQueryHelper.FormattedPowerQueryAttribute(attributeFormResponse.CurrentPowerQueryAttribute));
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
