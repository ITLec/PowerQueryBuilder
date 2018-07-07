namespace ITLec.ChartGuy.PowerQueryBuilder
{
    partial class AttributeLookupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxDisplayName = new System.Windows.Forms.TextBox();
            this.groupBoxLookup = new System.Windows.Forms.GroupBox();
            this.checkBoxAddEntityLogicalName = new System.Windows.Forms.CheckBox();
            this.checkBoxAddLookupGuid = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxAddFormattedValue = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxLookup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxLookup, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(368, 199);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxDisplayName);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display Name";
            // 
            // textBoxDisplayName
            // 
            this.textBoxDisplayName.Location = new System.Drawing.Point(95, 21);
            this.textBoxDisplayName.Name = "textBoxDisplayName";
            this.textBoxDisplayName.Size = new System.Drawing.Size(239, 22);
            this.textBoxDisplayName.TabIndex = 0;
            // 
            // groupBoxLookup
            // 
            this.groupBoxLookup.Controls.Add(this.checkBoxAddFormattedValue);
            this.groupBoxLookup.Controls.Add(this.checkBoxAddEntityLogicalName);
            this.groupBoxLookup.Controls.Add(this.checkBoxAddLookupGuid);
            this.groupBoxLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLookup.Location = new System.Drawing.Point(3, 80);
            this.groupBoxLookup.Name = "groupBoxLookup";
            this.groupBoxLookup.Size = new System.Drawing.Size(362, 71);
            this.groupBoxLookup.TabIndex = 1;
            this.groupBoxLookup.TabStop = false;
            this.groupBoxLookup.Text = "LookUp Options";
            // 
            // checkBoxAddEntityLogicalName
            // 
            this.checkBoxAddEntityLogicalName.AutoSize = true;
            this.checkBoxAddEntityLogicalName.Location = new System.Drawing.Point(190, 21);
            this.checkBoxAddEntityLogicalName.Name = "checkBoxAddEntityLogicalName";
            this.checkBoxAddEntityLogicalName.Size = new System.Drawing.Size(145, 21);
            this.checkBoxAddEntityLogicalName.TabIndex = 2;
            this.checkBoxAddEntityLogicalName.Text = "Add Logical Name";
            this.checkBoxAddEntityLogicalName.UseVisualStyleBackColor = true;
            // 
            // checkBoxAddLookupGuid
            // 
            this.checkBoxAddLookupGuid.AutoSize = true;
            this.checkBoxAddLookupGuid.Location = new System.Drawing.Point(23, 21);
            this.checkBoxAddLookupGuid.Name = "checkBoxAddLookupGuid";
            this.checkBoxAddLookupGuid.Size = new System.Drawing.Size(144, 21);
            this.checkBoxAddLookupGuid.TabIndex = 1;
            this.checkBoxAddLookupGuid.Text = "Add Lookup GUID";
            this.checkBoxAddLookupGuid.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 157);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(362, 28);
            this.panel1.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(277, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(63, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxAddFormattedValue
            // 
            this.checkBoxAddFormattedValue.AutoSize = true;
            this.checkBoxAddFormattedValue.Location = new System.Drawing.Point(23, 44);
            this.checkBoxAddFormattedValue.Name = "checkBoxAddFormattedValue";
            this.checkBoxAddFormattedValue.Size = new System.Drawing.Size(159, 21);
            this.checkBoxAddFormattedValue.TabIndex = 3;
            this.checkBoxAddFormattedValue.Text = "Add Formated Value";
            this.checkBoxAddFormattedValue.UseVisualStyleBackColor = true;
            // 
            // AttributeLookupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 199);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AttributeLookupForm";
            this.Text = "AttributeForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxLookup.ResumeLayout(false);
            this.groupBoxLookup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxDisplayName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBoxLookup;
        private System.Windows.Forms.CheckBox checkBoxAddLookupGuid;
        private System.Windows.Forms.CheckBox checkBoxAddEntityLogicalName;
        private System.Windows.Forms.CheckBox checkBoxAddFormattedValue;
    }
}