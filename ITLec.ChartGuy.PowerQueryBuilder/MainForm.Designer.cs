namespace ITLec.ChartGuy.PowerQueryBuilder
{
    partial class MainForm
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsbCloseThisTab = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLoadEntities = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGenerate = new System.Windows.Forms.ToolStripButton();
            this.tsbPublishEntity = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPublishAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tssSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.includeSortingWhenReplicatingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageSelectColumns = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbEntities = new System.Windows.Forms.GroupBox();
            this.lblSearchEntity = new System.Windows.Forms.Label();
            this.txtSearchEntity = new System.Windows.Forms.TextBox();
            this.lvEntities = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonAddToSelectedList = new System.Windows.Forms.Button();
            this.buttonRemoveFromSelectedFields = new System.Windows.Forms.Button();
            this.groupBoxAllFields = new System.Windows.Forms.GroupBox();
            this.listViewAllFields = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxSelectedFields = new System.Windows.Forms.GroupBox();
            this.gbSourceViews = new System.Windows.Forms.GroupBox();
            this.lvSourceViews = new System.Windows.Forms.ListView();
            this.allViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.allViewType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageFinalResult = new System.Windows.Forms.TabPage();
            this.tabControlResult = new System.Windows.Forms.TabControl();
            this.listViewSelectedFields = new ListViewCustomReorder.ListViewEx();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.D365Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPageSelectColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbEntities.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBoxAllFields.SuspendLayout();
            this.groupBoxSelectedFields.SuspendLayout();
            this.gbSourceViews.SuspendLayout();
            this.tabPageFinalResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.AutoSize = false;
            this.tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCloseThisTab,
            this.toolStripSeparator2,
            this.tsbLoadEntities,
            this.toolStripSeparator1,
            this.tsbGenerate,
            this.tsbPublishEntity,
            this.toolStripSeparator3,
            this.tsbPublishAll,
            this.toolStripSeparator4,
            this.tssSettings});
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tsMain.Size = new System.Drawing.Size(1081, 30);
            this.tsMain.TabIndex = 85;
            this.tsMain.Text = "toolStrip1";
            // 
            // tsbCloseThisTab
            // 
            this.tsbCloseThisTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCloseThisTab.Image = ((System.Drawing.Image)(resources.GetObject("tsbCloseThisTab.Image")));
            this.tsbCloseThisTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCloseThisTab.Name = "tsbCloseThisTab";
            this.tsbCloseThisTab.Size = new System.Drawing.Size(24, 27);
            this.tsbCloseThisTab.Text = "Close this tab";
            this.tsbCloseThisTab.Click += new System.EventHandler(this.TsbCloseThisTabClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbLoadEntities
            // 
            this.tsbLoadEntities.Image = ((System.Drawing.Image)(resources.GetObject("tsbLoadEntities.Image")));
            this.tsbLoadEntities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadEntities.Name = "tsbLoadEntities";
            this.tsbLoadEntities.Size = new System.Drawing.Size(118, 27);
            this.tsbLoadEntities.Text = "Load Entities";
            this.tsbLoadEntities.Click += new System.EventHandler(this.TsbLoadEntitiesClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbGenerate
            // 
            this.tsbGenerate.Enabled = false;
            this.tsbGenerate.Image = ((System.Drawing.Image)(resources.GetObject("tsbGenerate.Image")));
            this.tsbGenerate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGenerate.Name = "tsbGenerate";
            this.tsbGenerate.Size = new System.Drawing.Size(97, 27);
            this.tsbGenerate.Text = "Generate ";
            this.tsbGenerate.Visible = false;
            this.tsbGenerate.Click += new System.EventHandler(this.tsbGenerate_Click);
            // 
            // tsbPublishEntity
            // 
            this.tsbPublishEntity.Enabled = false;
            this.tsbPublishEntity.Image = ((System.Drawing.Image)(resources.GetObject("tsbPublishEntity.Image")));
            this.tsbPublishEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPublishEntity.Name = "tsbPublishEntity";
            this.tsbPublishEntity.Size = new System.Drawing.Size(121, 27);
            this.tsbPublishEntity.Text = "Publish entity";
            this.tsbPublishEntity.Visible = false;
            this.tsbPublishEntity.Click += new System.EventHandler(this.TsbPublishEntityClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbPublishAll
            // 
            this.tsbPublishAll.Enabled = false;
            this.tsbPublishAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbPublishAll.Image")));
            this.tsbPublishAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPublishAll.Name = "tsbPublishAll";
            this.tsbPublishAll.Size = new System.Drawing.Size(100, 27);
            this.tsbPublishAll.Text = "Publish all";
            this.tsbPublishAll.Visible = false;
            this.tsbPublishAll.Click += new System.EventHandler(this.TsbPublishAllClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 30);
            // 
            // tssSettings
            // 
            this.tssSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeSortingWhenReplicatingViewToolStripMenuItem});
            this.tssSettings.Image = ((System.Drawing.Image)(resources.GetObject("tssSettings.Image")));
            this.tssSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssSettings.Name = "tssSettings";
            this.tssSettings.Size = new System.Drawing.Size(96, 27);
            this.tssSettings.Text = "Settings";
            this.tssSettings.Visible = false;
            // 
            // includeSortingWhenReplicatingViewToolStripMenuItem
            // 
            this.includeSortingWhenReplicatingViewToolStripMenuItem.Checked = true;
            this.includeSortingWhenReplicatingViewToolStripMenuItem.CheckOnClick = true;
            this.includeSortingWhenReplicatingViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeSortingWhenReplicatingViewToolStripMenuItem.Name = "includeSortingWhenReplicatingViewToolStripMenuItem";
            this.includeSortingWhenReplicatingViewToolStripMenuItem.Size = new System.Drawing.Size(330, 26);
            this.includeSortingWhenReplicatingViewToolStripMenuItem.Text = "Include sorting when replicating view";
            this.includeSortingWhenReplicatingViewToolStripMenuItem.ToolTipText = "If this option is checked, the view will be replicated with the corresponding sor" +
    "ting customization";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ico_16_1039.gif");
            this.imageList1.Images.SetKeyName(1, "ico_16_1039_advFind.gif");
            this.imageList1.Images.SetKeyName(2, "ico_16_1039_associated.gif");
            this.imageList1.Images.SetKeyName(3, "ico_16_1039_default.gif");
            this.imageList1.Images.SetKeyName(4, "ico_16_1039_lookup.gif");
            this.imageList1.Images.SetKeyName(5, "ico_16_1039_quickFind.gif");
            this.imageList1.Images.SetKeyName(6, "userquery.png");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Icon.png");
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageSelectColumns);
            this.tabMain.Controls.Add(this.tabPageFinalResult);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 30);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1081, 605);
            this.tabMain.TabIndex = 86;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabPageSelectColumns
            // 
            this.tabPageSelectColumns.Controls.Add(this.splitContainer1);
            this.tabPageSelectColumns.Location = new System.Drawing.Point(4, 25);
            this.tabPageSelectColumns.Name = "tabPageSelectColumns";
            this.tabPageSelectColumns.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSelectColumns.Size = new System.Drawing.Size(1073, 576);
            this.tabPageSelectColumns.TabIndex = 0;
            this.tabPageSelectColumns.Text = "Select Table & Columns";
            this.tabPageSelectColumns.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbEntities);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel2.Controls.Add(this.gbSourceViews);
            this.splitContainer1.Size = new System.Drawing.Size(1067, 570);
            this.splitContainer1.SplitterDistance = 193;
            this.splitContainer1.TabIndex = 91;
            // 
            // gbEntities
            // 
            this.gbEntities.Controls.Add(this.lblSearchEntity);
            this.gbEntities.Controls.Add(this.txtSearchEntity);
            this.gbEntities.Controls.Add(this.lvEntities);
            this.gbEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntities.Enabled = false;
            this.gbEntities.Location = new System.Drawing.Point(0, 0);
            this.gbEntities.Margin = new System.Windows.Forms.Padding(4);
            this.gbEntities.Name = "gbEntities";
            this.gbEntities.Padding = new System.Windows.Forms.Padding(4);
            this.gbEntities.Size = new System.Drawing.Size(193, 570);
            this.gbEntities.TabIndex = 89;
            this.gbEntities.TabStop = false;
            this.gbEntities.Text = "Entities";
            // 
            // lblSearchEntity
            // 
            this.lblSearchEntity.AutoSize = true;
            this.lblSearchEntity.Location = new System.Drawing.Point(8, 23);
            this.lblSearchEntity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchEntity.Name = "lblSearchEntity";
            this.lblSearchEntity.Size = new System.Drawing.Size(57, 17);
            this.lblSearchEntity.TabIndex = 81;
            this.lblSearchEntity.Text = "Search:";
            // 
            // txtSearchEntity
            // 
            this.txtSearchEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchEntity.Location = new System.Drawing.Point(75, 20);
            this.txtSearchEntity.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearchEntity.Name = "txtSearchEntity";
            this.txtSearchEntity.Size = new System.Drawing.Size(109, 22);
            this.txtSearchEntity.TabIndex = 80;
            this.txtSearchEntity.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnSearchKeyUp);
            // 
            // lvEntities
            // 
            this.lvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvEntities.FullRowSelect = true;
            this.lvEntities.HideSelection = false;
            this.lvEntities.Location = new System.Drawing.Point(8, 52);
            this.lvEntities.Margin = new System.Windows.Forms.Padding(4);
            this.lvEntities.Name = "lvEntities";
            this.lvEntities.Size = new System.Drawing.Size(176, 498);
            this.lvEntities.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEntities.TabIndex = 79;
            this.lvEntities.UseCompatibleStateImageBehavior = false;
            this.lvEntities.View = System.Windows.Forms.View.Details;
            this.lvEntities.SelectedIndexChanged += new System.EventHandler(this.lvEntities_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Display name";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Logical name";
            this.columnHeader2.Width = 100;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxAllFields, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxSelectedFields, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 98);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(870, 472);
            this.tableLayoutPanel1.TabIndex = 98;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.buttonAddToSelectedList, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonRemoveFromSelectedFields, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(403, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(63, 466);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonAddToSelectedList
            // 
            this.buttonAddToSelectedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAddToSelectedList.Location = new System.Drawing.Point(3, 3);
            this.buttonAddToSelectedList.Name = "buttonAddToSelectedList";
            this.buttonAddToSelectedList.Size = new System.Drawing.Size(57, 227);
            this.buttonAddToSelectedList.TabIndex = 94;
            this.buttonAddToSelectedList.Text = "==>";
            this.buttonAddToSelectedList.UseVisualStyleBackColor = true;
            this.buttonAddToSelectedList.Click += new System.EventHandler(this.buttonAddToSelectedList_Click);
            // 
            // buttonRemoveFromSelectedFields
            // 
            this.buttonRemoveFromSelectedFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRemoveFromSelectedFields.Location = new System.Drawing.Point(3, 236);
            this.buttonRemoveFromSelectedFields.Name = "buttonRemoveFromSelectedFields";
            this.buttonRemoveFromSelectedFields.Size = new System.Drawing.Size(57, 227);
            this.buttonRemoveFromSelectedFields.TabIndex = 95;
            this.buttonRemoveFromSelectedFields.Text = "<==";
            this.buttonRemoveFromSelectedFields.UseVisualStyleBackColor = true;
            this.buttonRemoveFromSelectedFields.Click += new System.EventHandler(this.buttonRemoveFromSelectedFields_Click);
            // 
            // groupBoxAllFields
            // 
            this.groupBoxAllFields.Controls.Add(this.listViewAllFields);
            this.groupBoxAllFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAllFields.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAllFields.Name = "groupBoxAllFields";
            this.groupBoxAllFields.Size = new System.Drawing.Size(394, 466);
            this.groupBoxAllFields.TabIndex = 96;
            this.groupBoxAllFields.TabStop = false;
            this.groupBoxAllFields.Text = "All Fields";
            // 
            // listViewAllFields
            // 
            this.listViewAllFields.CheckBoxes = true;
            this.listViewAllFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listViewAllFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAllFields.FullRowSelect = true;
            this.listViewAllFields.HideSelection = false;
            this.listViewAllFields.Location = new System.Drawing.Point(3, 18);
            this.listViewAllFields.Margin = new System.Windows.Forms.Padding(4);
            this.listViewAllFields.Name = "listViewAllFields";
            this.listViewAllFields.Size = new System.Drawing.Size(388, 445);
            this.listViewAllFields.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewAllFields.TabIndex = 93;
            this.listViewAllFields.UseCompatibleStateImageBehavior = false;
            this.listViewAllFields.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Display name";
            this.columnHeader5.Width = 140;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Logical name";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Type";
            // 
            // groupBoxSelectedFields
            // 
            this.groupBoxSelectedFields.Controls.Add(this.listViewSelectedFields);
            this.groupBoxSelectedFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSelectedFields.Location = new System.Drawing.Point(472, 3);
            this.groupBoxSelectedFields.Name = "groupBoxSelectedFields";
            this.groupBoxSelectedFields.Size = new System.Drawing.Size(395, 466);
            this.groupBoxSelectedFields.TabIndex = 97;
            this.groupBoxSelectedFields.TabStop = false;
            this.groupBoxSelectedFields.Text = "Selected Fields";
            // 
            // gbSourceViews
            // 
            this.gbSourceViews.Controls.Add(this.lvSourceViews);
            this.gbSourceViews.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSourceViews.Enabled = false;
            this.gbSourceViews.Location = new System.Drawing.Point(0, 0);
            this.gbSourceViews.Margin = new System.Windows.Forms.Padding(4);
            this.gbSourceViews.Name = "gbSourceViews";
            this.gbSourceViews.Padding = new System.Windows.Forms.Padding(4);
            this.gbSourceViews.Size = new System.Drawing.Size(870, 98);
            this.gbSourceViews.TabIndex = 91;
            this.gbSourceViews.TabStop = false;
            this.gbSourceViews.Text = "Select View";
            // 
            // lvSourceViews
            // 
            this.lvSourceViews.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.allViewName,
            this.allViewType});
            this.lvSourceViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSourceViews.FullRowSelect = true;
            this.lvSourceViews.HideSelection = false;
            this.lvSourceViews.Location = new System.Drawing.Point(4, 19);
            this.lvSourceViews.Margin = new System.Windows.Forms.Padding(4);
            this.lvSourceViews.Name = "lvSourceViews";
            this.lvSourceViews.Size = new System.Drawing.Size(862, 75);
            this.lvSourceViews.SmallImageList = this.imageList1;
            this.lvSourceViews.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvSourceViews.TabIndex = 68;
            this.lvSourceViews.UseCompatibleStateImageBehavior = false;
            this.lvSourceViews.View = System.Windows.Forms.View.Details;
            this.lvSourceViews.SelectedIndexChanged += new System.EventHandler(this.LvSourceViewsSelectedIndexChanged);
            // 
            // allViewName
            // 
            this.allViewName.Text = "View Name";
            this.allViewName.Width = 350;
            // 
            // allViewType
            // 
            this.allViewType.Text = "View Type";
            this.allViewType.Width = 130;
            // 
            // tabPageFinalResult
            // 
            this.tabPageFinalResult.Controls.Add(this.tabControlResult);
            this.tabPageFinalResult.Location = new System.Drawing.Point(4, 25);
            this.tabPageFinalResult.Name = "tabPageFinalResult";
            this.tabPageFinalResult.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFinalResult.Size = new System.Drawing.Size(1073, 576);
            this.tabPageFinalResult.TabIndex = 1;
            this.tabPageFinalResult.Text = "Final Result";
            this.tabPageFinalResult.UseVisualStyleBackColor = true;
            // 
            // tabControlResult
            // 
            this.tabControlResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlResult.Location = new System.Drawing.Point(3, 3);
            this.tabControlResult.Name = "tabControlResult";
            this.tabControlResult.SelectedIndex = 0;
            this.tabControlResult.Size = new System.Drawing.Size(1067, 570);
            this.tabControlResult.TabIndex = 0;
            this.tabControlResult.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.tabControlResult_ControlAdded);
            // 
            // listViewSelectedFields
            // 
            this.listViewSelectedFields.AllowDrop = true;
            this.listViewSelectedFields.CheckBoxes = true;
            this.listViewSelectedFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.D365Type});
            this.listViewSelectedFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSelectedFields.FullRowSelect = true;
            this.listViewSelectedFields.HideSelection = false;
            this.listViewSelectedFields.LineAfter = -1;
            this.listViewSelectedFields.LineBefore = -1;
            this.listViewSelectedFields.Location = new System.Drawing.Point(3, 18);
            this.listViewSelectedFields.Margin = new System.Windows.Forms.Padding(4);
            this.listViewSelectedFields.MultiSelect = false;
            this.listViewSelectedFields.Name = "listViewSelectedFields";
            this.listViewSelectedFields.Size = new System.Drawing.Size(389, 445);
            this.listViewSelectedFields.TabIndex = 92;
            this.listViewSelectedFields.UseCompatibleStateImageBehavior = false;
            this.listViewSelectedFields.View = System.Windows.Forms.View.Details;
            this.listViewSelectedFields.DoubleClick += new System.EventHandler(this.listViewSelectedFields_DoubleClick);
            this.listViewSelectedFields.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewSelectedFields_MouseDown);
            this.listViewSelectedFields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listViewSelectedFields_MouseMove);
            this.listViewSelectedFields.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewSelectedFields_MouseUp);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Display name";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Logical name";
            this.columnHeader4.Width = 100;
            // 
            // D365Type
            // 
            this.D365Type.Text = "D365 Type";
            this.D365Type.Width = 80;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.tsMain);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Size = new System.Drawing.Size(1081, 635);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabPageSelectColumns.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbEntities.ResumeLayout(false);
            this.gbEntities.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBoxAllFields.ResumeLayout(false);
            this.groupBoxSelectedFields.ResumeLayout(false);
            this.gbSourceViews.ResumeLayout(false);
            this.tabPageFinalResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbCloseThisTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbLoadEntities;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbGenerate;
        private System.Windows.Forms.ToolStripButton tsbPublishEntity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbPublishAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton tssSettings;
        private System.Windows.Forms.ToolStripMenuItem includeSortingWhenReplicatingViewToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageSelectColumns;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbEntities;
        private System.Windows.Forms.Label lblSearchEntity;
        private System.Windows.Forms.TextBox txtSearchEntity;
        private System.Windows.Forms.ListView lvEntities;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBoxSelectedFields;
        private ListViewCustomReorder.ListViewEx listViewSelectedFields;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBoxAllFields;
        private System.Windows.Forms.ListView listViewAllFields;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button buttonRemoveFromSelectedFields;
        private System.Windows.Forms.Button buttonAddToSelectedList;
        private System.Windows.Forms.GroupBox gbSourceViews;
        private System.Windows.Forms.ListView lvSourceViews;
        private System.Windows.Forms.ColumnHeader allViewName;
        private System.Windows.Forms.ColumnHeader allViewType;
        private System.Windows.Forms.TabPage tabPageFinalResult;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader D365Type;
        private System.Windows.Forms.TabControl tabControlResult;
    }
}
