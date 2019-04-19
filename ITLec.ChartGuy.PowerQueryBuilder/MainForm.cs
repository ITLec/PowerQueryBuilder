// PROJECT : ITLec.ChartGuy.PowerQueryBuilder

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using ITLec.ChartGuy.PowerQueryBuilder.Forms;
using ITLec.ChartGuy.PowerQueryBuilder.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Tanguy.WinForm.Utilities.DelegatesHelpers;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using ITLec.ChartGuy.PowerQueryBuilder.FetchXml;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
    public partial class MainForm : PluginControlBase, IGitHubPlugin, IHelpPlugin, IMessageBusHost
    {
        #region IGitHubPlugin
        public string HelpUrl { get { return "https://crmchartguy.com/power-query-builder/"; } }

        public string UserName
        {
            get
            {
                return "ITLec";
            }
        }
        public string RepositoryName { get { return "PowerQueryBuilder"; } }
        #endregion

        #region Variables
        private List<EntityMetadata> entitiesCache;
        private ListViewItem[] listViewItemsCache;
        private List<ListViewItem> sourceViewsItems;
        private List<ListViewItem> allAttributesListViewItemCache;
        private List<ListViewItem> selectedAttributesListViewItemCache;
        private List<ListViewItem> fetchXmlAttributesListViewItemCache;
        EntityMetadata CurrentEntityMetadataWithItems = null;
        #endregion
        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            allAttributesListViewItemCache = new List<ListViewItem>();
            selectedAttributesListViewItemCache = new List<ListViewItem>();
            fetchXmlAttributesListViewItemCache = new List<ListViewItem>();
        }

        #endregion Constructor


        bool CanClearSelectedGrid = true;

        #region Main ToolStrip Handlers

        #region Fill Entities

        private void OnSearchKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            var entityName = txtSearchEntity.Text;
            if (string.IsNullOrWhiteSpace(entityName))
            {
                lvEntities.BeginUpdate();
                lvEntities.Items.Clear();
                lvEntities.Items.AddRange(listViewItemsCache);
                lvEntities.EndUpdate();
            }
            else
            {
                lvEntities.BeginUpdate();
                lvEntities.Items.Clear();
                var filteredItems = listViewItemsCache
                    .Where(item => (item.Text.StartsWith(entityName, StringComparison.OrdinalIgnoreCase) ||
                    item.Tag.ToString().StartsWith(entityName, StringComparison.OrdinalIgnoreCase)
                    ))
                    .ToArray();
                lvEntities.Items.AddRange(filteredItems);
                lvEntities.EndUpdate();
            }

        }
        private void LoadEntities()
        {
            txtSearchEntity.Text = string.Empty;
            lvEntities.Items.Clear();
            listViewAllFields.Items.Clear();

            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }
            gbEntities.Enabled = false;

            lvSourceViews.Items.Clear();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities...",
                Work = (bw, e) =>
                {
                    e.Result = MetadataHelper.RetrieveEntities(Service);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        string errorMessage = CrmExceptionHelper.GetErrorMessage(e.Error, true);
                        CommonDelegates.DisplayMessageBox(ParentForm, errorMessage, "Error", MessageBoxButtons.OK,
                                                          MessageBoxIcon.Error);
                    }
                    else
                    {
                        entitiesCache = (List<EntityMetadata>)e.Result;
                        lvEntities.Items.Clear();
                        var list = new List<ListViewItem>();
                        foreach (EntityMetadata emd in (List<EntityMetadata>)e.Result)
                        {
                            var item = new ListViewItem { Text = emd.DisplayName.UserLocalizedLabel.Label, Tag = emd.LogicalName };
                            item.SubItems.Add(emd.LogicalName);
                            list.Add(item);
                        }

                        this.listViewItemsCache = list.ToArray();
                        lvEntities.Items.AddRange(listViewItemsCache);

                        gbEntities.Enabled = true;
                    }
                }
            });
        }

        private void TsbLoadEntitiesClick(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);

            mainFetchXmltoolStripDropDownButton.Enabled = true;
        }

        #endregion Fill Entities



        #endregion Main ToolStrip Handlers

        #region Views
        public static string FetchXml = "";
        private void LvSourceViewsSelectedIndexChanged(object sender, EventArgs e)
        {
            FillAttributes();
            if (lvSourceViews.SelectedItems.Count > 0)
            {
                lvSourceViews.SelectedIndexChanged -= LvSourceViewsSelectedIndexChanged;
                lvSourceViews.Enabled = false;

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading view items...",
                    AsyncArgument = lvSourceViews.SelectedItems[0].Tag,
                    Work = (bw, evt) =>
                    {
                        Entity currentSelectedView = (Entity)evt.Argument;
                        string layoutXml = currentSelectedView["layoutxml"].ToString();
                        string fetchXml = currentSelectedView.Contains("fetchxml")
                                              ? currentSelectedView["fetchxml"].ToString()
                                              : string.Empty;

                        XmlDocument fetchDoc = new XmlDocument();
                        fetchDoc.LoadXml(fetchXml);

                        FetchXml = fetchXml; //FetchXmlHelper.GetFilterXmlStr(fetchXml);
                        var headers = new List<ColumnHeader>();



                        //var selectedItems = listViewSelectedFields.Items;

                        //foreach (ListViewItem item in selectedItems)
                        //{
                        //    MoveItemFormListViewToAnother(listViewSelectedFields, listViewAllFields, (PowerQueryAttribute) item.Tag);
                        //}

                        foreach (XmlNode columnNode in fetchDoc.SelectNodes("/fetch/entity/attribute"))
                        {

                            /////////////


                            if (!columnNode.Attributes["name"].Value.Contains("."))
                            {
                                //todo if sub-entity
                                // string fieldDisplayName = MetadataHelper.RetrieveAttributeDisplayName(CurrentEntityMetadataWithItems,
                                //                                                           columnNode.Attributes["name"].Value,
                                //                                                           fetchXml, Service);

                                AttributeMetadata attribute = (from attr in CurrentEntityMetadataWithItems.Attributes
                                                               where attr.LogicalName == columnNode.Attributes["name"].Value
                                                               select attr).FirstOrDefault();
                                if (attribute != null)
                                {
                                    PowerQueryAttribute powerQueryAttribute = PowerQueryAttribute.GetPowerQueryAttributeByMetadata(attribute);
                                    AddItemToSelectedAttributeslistView(powerQueryAttribute);
                                }
                            }


                            ///////////////
                        }
                    },
                    PostWorkCallBack = evt =>
                    {
                        if (evt.Error != null)
                        {
                            MessageBox.Show(ParentForm, "Error while displaying view: " + evt.Error.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        lvSourceViews.SelectedIndexChanged += LvSourceViewsSelectedIndexChanged;
                        lvSourceViews.Enabled = true;
                        EnableVisableListViewSelectedFields();
                    }
                });
            }


        }
        private void BwFillViewsDoWork(object sender, DoWorkEventArgs e)
        {
            string entityName = e.Argument.ToString(); ;
            FillViews(entityName);
            CurrentEntityMetadataWithItems = MetadataHelper.RetrieveEntity(entityName, Service);
        }


        private void FillViews(string entityLogicalName, List<Entity> viewsList)
        {

            viewsList.AddRange(ViewHelper.RetrieveUserViews(entityLogicalName, entitiesCache, Service));

            sourceViewsItems = new List<ListViewItem>();

            foreach (Entity view in viewsList)
            {
                bool display = true;

                var item = new ListViewItem(view["name"].ToString());
                item.Tag = view;

                #region Gestion de l'image associée à la vue

                switch ((int)view["querytype"])
                {
                    case ViewHelper.VIEW_BASIC:
                        {
                            if (view.LogicalName == "savedquery")
                            {
                                if ((bool)view["isdefault"])
                                {
                                    item.SubItems.Add("Default public view");
                                    item.ImageIndex = 3;
                                }
                                else
                                {
                                    item.SubItems.Add("Public view");
                                    item.ImageIndex = 0;
                                }
                            }
                            else
                            {
                                item.SubItems.Add("User view");
                                item.ImageIndex = 6;
                            }
                        }
                        break;

                    case ViewHelper.VIEW_ADVANCEDFIND:
                        {
                            item.SubItems.Add("Advanced find view");
                            item.ImageIndex = 1;
                        }
                        break;

                    case ViewHelper.VIEW_ASSOCIATED:
                        {
                            item.SubItems.Add("Associated view");
                            item.ImageIndex = 2;
                        }
                        break;

                    case ViewHelper.VIEW_QUICKFIND:
                        {
                            item.SubItems.Add("QuickFind view");
                            item.ImageIndex = 5;
                        }
                        break;

                    case ViewHelper.VIEW_SEARCH:
                        {
                            item.SubItems.Add("Lookup view");
                            item.ImageIndex = 4;
                        }
                        break;

                    default:
                        {
                            //item.SubItems.Add(view["name"].ToString());
                            display = false;
                        }
                        break;
                }

                #endregion Gestion de l'image associée à la vue

                if (display)
                {
                    // Add view to each list of views (source and target)
                    ListViewItem clonedItem = (ListViewItem)item.Clone();

                    sourceViewsItems.Add(item);
                    //ListViewDelegates.AddItem(lvSourceViews, item);

                    if (view.Contains("iscustomizable") && ((BooleanManagedProperty)view["iscustomizable"]).Value == false
                        && view.Contains("ismanaged") && (bool)view["ismanaged"])
                    {
                        clonedItem.ForeColor = Color.Gray;
                        clonedItem.ToolTipText = "This managed view has not been defined as customizable";
                    }

                }
            }
        }

        private void FillViews(string entityName)
        {
            string entityLogicalName = entityName;

            List<Entity> viewsList = ViewHelper.RetrieveViews(entityLogicalName, entitiesCache, Service);
            viewsList.AddRange(ViewHelper.RetrieveUserViews(entityLogicalName, entitiesCache, Service));

            sourceViewsItems = new List<ListViewItem>();

            foreach (Entity view in viewsList)
            {
                bool display = true;

                var item = new ListViewItem(view["name"].ToString());
                item.Tag = view;

                #region Gestion de l'image associée à la vue

                switch ((int)view["querytype"])
                {
                    case ViewHelper.VIEW_BASIC:
                        {
                            if (view.LogicalName == "savedquery")
                            {
                                if ((bool)view["isdefault"])
                                {
                                    item.SubItems.Add("Default public view");
                                    item.ImageIndex = 3;
                                }
                                else
                                {
                                    item.SubItems.Add("Public view");
                                    item.ImageIndex = 0;
                                }
                            }
                            else
                            {
                                item.SubItems.Add("User view");
                                item.ImageIndex = 6;
                            }
                        }
                        break;

                    case ViewHelper.VIEW_ADVANCEDFIND:
                        {
                            item.SubItems.Add("Advanced find view");
                            item.ImageIndex = 1;
                        }
                        break;

                    case ViewHelper.VIEW_ASSOCIATED:
                        {
                            item.SubItems.Add("Associated view");
                            item.ImageIndex = 2;
                        }
                        break;

                    case ViewHelper.VIEW_QUICKFIND:
                        {
                            item.SubItems.Add("QuickFind view");
                            item.ImageIndex = 5;
                        }
                        break;

                    case ViewHelper.VIEW_SEARCH:
                        {
                            item.SubItems.Add("Lookup view");
                            item.ImageIndex = 4;
                        }
                        break;

                    default:
                        {
                            //item.SubItems.Add(view["name"].ToString());
                            display = false;
                        }
                        break;
                }

                #endregion Gestion de l'image associée à la vue

                if (display)
                {
                    // Add view to each list of views (source and target)
                    ListViewItem clonedItem = (ListViewItem)item.Clone();

                    sourceViewsItems.Add(item);
                    //ListViewDelegates.AddItem(lvSourceViews, item);

                    if (view.Contains("iscustomizable") && ((BooleanManagedProperty)view["iscustomizable"]).Value == false
                        && view.Contains("ismanaged") && (bool)view["ismanaged"])
                    {
                        clonedItem.ForeColor = Color.Gray;
                        clonedItem.ToolTipText = "This managed view has not been defined as customizable";
                    }
                    
                }
            }
        }

        private void BwFillViewsRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            if (sourceViewsItems.Count == 0)
            {
                MessageBox.Show(this, "This entity does not contain any view", "Warning", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.Default;
            gbSourceViews.Enabled = true;
            lvSourceViews.Items.AddRange(sourceViewsItems.ToArray());

            FillAttributes();
            EnableVisableListViewSelectedFields();
        }

        private void lvEntities_SelectedIndexChanged(object sender, EventArgs eventArgs)
        {

            if (lvEntities.SelectedItems.Count > 0 && lvEntities.SelectedItems[0] != null && lvEntities.SelectedItems[0].Tag != null)
            {
                string entityLogicalName = lvEntities.SelectedItems[0].Tag.ToString();
                lvEntitiesSelectedIndexChanged(entityLogicalName, "");
            }
        }

        private void lvEntitiesSelectedIndexChanged(string entityLogicalName, string fetchXml)
        {
            ClearForm();
            listViewAllFields.Items.Clear();
            

            //if (lvEntities.SelectedItems.Count > 0)
            //{


                // Reinit other controls
                lvSourceViews.Items.Clear();

                Cursor = Cursors.WaitCursor;
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Retrieving Attributes ...",
                    Work = (bw, e) =>
                    {
                        List<Entity> viewsList = ViewHelper.RetrieveViews(entityLogicalName, entitiesCache, Service);

                        CurrentEntityMetadataWithItems = MetadataHelper.RetrieveEntity(entityLogicalName, Service);
                        e.Result = viewsList;
                    },
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            string errorMessage = CrmExceptionHelper.GetErrorMessage(e.Error, true);
                            CommonDelegates.DisplayMessageBox(ParentForm, errorMessage, "Error", MessageBoxButtons.OK,
                                                              MessageBoxIcon.Error);
                        }
                        else
                        {
                            List<Entity> viewsList = (List<Entity>)e.Result;
                            FillViews(entityLogicalName, viewsList);



                            gbSourceViews.Enabled = true;
                            lvSourceViews.Items.AddRange(sourceViewsItems.ToArray());

                            FillAttributes();

                            if(!string.IsNullOrEmpty( fetchXml))
                            {
                                FetchXml = fetchXml;
                                updateSelectedFieldsGridWithFetchXml();
                            }
                            else
                            {
                                FetchXml = "";
                            }
                            EnableVisableListViewSelectedFields();
                            Cursor = Cursors.Default;
                        }
                    }
                });
            //}
        }

        private void updateSelectedFieldsGridWithFetchXml()
        {
            XmlDocument fetchDoc = new XmlDocument();
            fetchDoc.LoadXml(FetchXml);

            string entityName = fetchDoc?.DocumentElement?.SelectSingleNode("/fetch/entity").Attributes["name"].Value;


            foreach (XmlNode columnNode in fetchDoc.SelectNodes("/fetch/entity/attribute"))
            {
                /////////////
                //   columnNode.app

                if (!columnNode.Attributes["name"].Value.Contains("."))
                {
                    AttributeMetadata attribute = (from attr in CurrentEntityMetadataWithItems.Attributes
                                                   where attr.LogicalName == columnNode.Attributes["name"].Value
                                                   select attr).FirstOrDefault();
                    if (attribute != null)
                    {
                        PowerQueryAttribute powerQueryAttribute = PowerQueryAttribute.GetPowerQueryAttributeByMetadata(attribute);
                        AddItemToSelectedAttributeslistView(powerQueryAttribute);
                    }
                }
            }
        }

        private void ClearForm()
        {

            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }
            
            ClearTabs();
        }

        private void ClearTabs()
        {
            ClearODataResultTab();
            ClearFetchXmlTab();
            ClearOptionSetTab();
            ClearServiceRootURLTab();
        }

        #endregion Fill Views
        
        private void TsbCloseThisTabClick(object sender, EventArgs e)
        {
            CloseTool();
        }
        private void buttonRemoveFromSelectedFields_Click(object sender, EventArgs e)
        {
            
            var checkedItems = listViewSelectedFields.CheckedItems;
            //    CopyItemsFromSelectedFieldsToAllFields(checkedItems);

            List<ListViewItem> selectedAttributesListViewItemCacheTmp = new List<ListViewItem>( selectedAttributesListViewItemCache);
            foreach (ListViewItem item in checkedItems)
            {
                selectedAttributesListViewItemCache.Remove(item);
            }

            

            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }


            ClearTabs();
            listViewSelectedFields.Items.AddRange(selectedAttributesListViewItemCache.ToArray());

            RefreshAllAttributesList();
            EnableVisableListViewSelectedFields();
        }
        
        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedIndex == 0)
            {
                tsbLoadEntities.Visible = true;
                mainFetchXmltoolStripDropDownButton.Visible = true;
            }
            else
            {
                tsbLoadEntities.Visible = false;
                mainFetchXmltoolStripDropDownButton.Visible = false;
            }

            if (tabMain.SelectedIndex == 2)
            {
                tsbGenerateFechXml.Visible = true;
                tsbUpdateFetchXml.Visible = false;
            }
            else
            {
                tsbGenerateFechXml.Visible = false;
                tsbUpdateFetchXml.Visible = true;
            }


        }

        private void tsbGenerateOData_Click(object sender, EventArgs eventArgs)
        {
            ClearODataResultTab();
            
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Generating OData Query...",
                Work = (bw, e) =>
                {
                    e.Result =   GeneratePowerBIODataQuery();
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        string errorMessage = CrmExceptionHelper.GetErrorMessage(e.Error, true);
                        CommonDelegates.DisplayMessageBox(ParentForm, errorMessage, "Error", MessageBoxButtons.OK,
                                                          MessageBoxIcon.Error);
                    }
                    else
                    {
                        string finalMainQuery = (string)e.Result;

                        CreateTabPage(tabODataResult, "tabPageMainQuery", $"Main Query ({CurrentEntityMetadataWithItems.DisplayName.UserLocalizedLabel.Label}) - ({CurrentEntityMetadataWithItems.LogicalName})", "txt_tabPageMainQuery", finalMainQuery);
                        
                        tabMain.SelectedTab = tabPageFinalResult;
                    }
                }
            });
        }

        private void CreateTabPage(TabControl tabControl,string tabPageName, string tabPageText, string textBoxFieldName, string queryString)
        {

            if (!tabControl.TabPages.ContainsKey(tabPageName))
            {
                TabPage tabPage = new TabPage();
                tabPage.Name = tabPageName;
                tabPage.Text = tabPageText;
                System.Windows.Forms.RichTextBox txtMainQuery = new System.Windows.Forms.RichTextBox();
                txtMainQuery.Name = textBoxFieldName;
                txtMainQuery.Dock = DockStyle.Fill;
                tabPage.Controls.Add(txtMainQuery);
                tabControl.TabPages.Add(tabPage);

                txtMainQuery.Text = queryString;
            }
            else
            {
                TabPage tapPageMainQuery = tabControl.TabPages[tabPageName];

                foreach (Control control in tapPageMainQuery.Controls)
                {
                    if (control is System.Windows.Forms.RichTextBox)
                    {
                        System.Windows.Forms.RichTextBox txtMainQuery = (System.Windows.Forms.RichTextBox)control;
                        if (txtMainQuery.Name == textBoxFieldName)
                        {
                            txtMainQuery.Text = queryString;
                        }
                    }
                }
            }
        }
        
        void AddItemToSelectedAttributeslistView( PowerQueryAttribute _powerQueryAttribute)
        {
            //listViewSelectedFields
            bool canAddField = true;
            foreach (ListViewItem item in selectedAttributesListViewItemCache)
            {
                if (((PowerQueryAttribute)item.Tag).Name == _powerQueryAttribute.Name)
                {
                    canAddField = false;
                }
            }
            if (canAddField)
            {
                var selectedFieldItem = new ListViewItem { Text = _powerQueryAttribute.DisplayName, Tag = _powerQueryAttribute };
                selectedFieldItem.SubItems.Add(_powerQueryAttribute.Name);
                selectedFieldItem.SubItems.Add(_powerQueryAttribute.Type);
                selectedAttributesListViewItemCache.Add(selectedFieldItem);
            }



            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }

            listViewSelectedFields.Items.AddRange(selectedAttributesListViewItemCache.ToArray());


        }
        
        private void tabODataResult_ControlAdded(object sender, ControlEventArgs e)
        {
        }
        void EnableVisableListViewSelectedFields()
        {

            bool isVisable = false;

            if (listViewSelectedFields.Items.Count > 0)
            {
                isVisable = true;
            }


            listViewSelectedFields.Enabled = isVisable;
            tsbGenerateOData.Enabled = isVisable;
            tsbUpdateFetchXml.Enabled = isVisable;
            tsbGenerateFechXml.Enabled = isVisable;
            toolStripButtonGenerateServiceRootURL.Enabled = isVisable;
            toolStripButtonOptionSet.Enabled = isVisable;
        }

        private void tsbGenerateFechXml_Click(object sender, EventArgs eventArgs)
        {

            FetchXmlQuery fetchXmlQuery = new FetchXmlQuery(CurrentEntityMetadataWithItems, listViewFetchXmlConfig.Items.Cast<ListViewItem>().Select(e => (PowerQueryAttribute)e.Tag).ToList());
            fetchXmlQuery.HasRecordURL = checkBoxFetchXml_HasRecordURL.Checked;
              fetchXmlQuery.FetchXml = txtFetchXml.Text;
            fetchXmlQuery.IsUseAllAttributesOption = checkBoxUseAllAttributes.Checked;
            string msg = fetchXmlQuery.Validate();

            if (string.IsNullOrEmpty(msg))
            {
                CreateTabPage(tabFetchXmlResult, "tabPageMainQuery", $"Main Query ({CurrentEntityMetadataWithItems.DisplayName.UserLocalizedLabel.Label}) - ({CurrentEntityMetadataWithItems.LogicalName})", "txt_tabPageMainQuery", fetchXmlQuery.FetchXmlQueryString);
                tabFetchXmlResult.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show(msg);
            }

            

        }
        

        private void txtSearchEntity_TextChanged(object sender, EventArgs e)
        {

        }


        private void listViewSelectedFields_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void toolStripButtonOptionSet_Click(object sender, EventArgs e)
        {

            ClearOptionSetTab();

            GeneratePowerBIOptionSet();

            tabMain.SelectedTab = tabPageOptionSet;
        }
        #region Selected Attributes

        #region Selected Attributes ListView

        private void buttonAddToSelectedList_Click(object sender, EventArgs e)
        {
            var checkedItems = listViewAllFields.CheckedItems;
                selectedAttributesListViewItemCache = listViewSelectedFields.Items.Cast<ListViewItem>().ToList();

            foreach (ListViewItem item in checkedItems)
            {
                var powerQueryAttribute = (PowerQueryAttribute)item.Tag;
                bool canAddItem = true;
                foreach (var selectedAttribute in selectedAttributesListViewItemCache)
                {
                    var selectedPowerQueryAttribute = (PowerQueryAttribute)selectedAttribute.Tag;
                    if (powerQueryAttribute.DisplayName == selectedPowerQueryAttribute.DisplayName)
                    {
                    //    MessageBox.Show("Can not add two attributes with the same name");
                        canAddItem = false;
                    }
                }

                if (canAddItem)
                {
                    selectedAttributesListViewItemCache.Add((ListViewItem)item.Clone());
                }
            }



            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }
            ClearTabs();
            listViewSelectedFields.Items.AddRange(selectedAttributesListViewItemCache.ToArray());

            RefreshAllAttributesList();
            EnableVisableListViewSelectedFields();
        }


        #region Select All Selected Attributes ListView

        private void listViewSelectedFields_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {

            if ((e.ColumnIndex == 0))
            {
                CheckBox cck = new CheckBox();
                // With...
                Text = "";
                Visible = true;
                listViewSelectedFields.SuspendLayout();
                e.DrawBackground();
                cck.BackColor = Color.Transparent;
                cck.UseVisualStyleBackColor = true;

                cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
                cck.Size = new Size((cck.GetPreferredSize(new Size((e.Bounds.Width - 1), e.Bounds.Height)).Width + 1), e.Bounds.Height);
                cck.Location = new Point(3, 0);
                listViewSelectedFields.Controls.Add(cck);
                cck.Show();
                cck.BringToFront();
                e.DrawText((TextFormatFlags.VerticalCenter | TextFormatFlags.Left));
                cck.Click += new EventHandler(listViewSelectedFields_Bink);
                listViewSelectedFields.ResumeLayout(true);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listViewSelectedFields_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listViewSelectedFields_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

            e.DrawDefault = true;
        }

        private void listViewSelectedFields_Bink(object sender, System.EventArgs e)
        {
            CheckBox test = sender as CheckBox;

            for (int i = 0; i < listViewSelectedFields.Items.Count; i++)
            {
                listViewSelectedFields.Items[i].Checked = test.Checked;
            }
        }
        #endregion


        #region DragDrop Selected Attributes ListView
        // The LVItem being dragged
        private ListViewItem _itemDnD = null;
        private void listViewSelectedFields_MouseDown(object sender, MouseEventArgs e)
        {

            _itemDnD = listViewSelectedFields.GetItemAt(e.X, e.Y);
            // if the LV is still empty, no item will be found anyway, so we don't have to consider this case
        }
        private void listViewSelectedFields_DragDrop(object sender, DragEventArgs e)
        {
            EnableVisableListViewSelectedFields();
        }

        private void listViewSelectedFields_MouseMove(object sender, MouseEventArgs e)
        {

            if (_itemDnD == null)
                return;

            // Show the user that a drag operation is happening
            Cursor = Cursors.Hand;

            // calculate the bottom of the last item in the LV so that you don't have to stop your drag at the last item
            int lastItemBottom = Math.Min(e.Y, listViewSelectedFields.Items[listViewSelectedFields.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1);

            // use 0 instead of e.X so that you don't have to keep inside the columns while dragging
            ListViewItem itemOver = listViewSelectedFields.GetItemAt(0, lastItemBottom);

            if (itemOver == null)
                return;

            Rectangle rc = itemOver.GetBounds(ItemBoundsPortion.Entire);
            if (e.Y < rc.Top + (rc.Height / 2))
            {
                listViewSelectedFields.LineBefore = itemOver.Index;
                listViewSelectedFields.LineAfter = -1;
            }
            else
            {
                listViewSelectedFields.LineBefore = -1;
                listViewSelectedFields.LineAfter = itemOver.Index;
            }

            // invalidate the LV so that the insertion line is shown
            listViewSelectedFields.Invalidate();
        }

        private void listViewSelectedFields_MouseUp(object sender, MouseEventArgs e)
        {

            if (_itemDnD == null)
                return;

            try
            {
                // calculate the bottom of the last item in the LV so that you don't have to stop your drag at the last item
                int lastItemBottom = Math.Min(e.Y, listViewSelectedFields.Items[listViewSelectedFields.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1);

                // use 0 instead of e.X so that you don't have to keep inside the columns while dragging
                ListViewItem itemOver = listViewSelectedFields.GetItemAt(0, lastItemBottom);

                if (itemOver == null)
                    return;

                Rectangle rc = itemOver.GetBounds(ItemBoundsPortion.Entire);

                // find out if we insert before or after the item the mouse is over
                bool insertBefore;
                if (e.Y < rc.Top + (rc.Height / 2))
                {
                    insertBefore = true;
                }
                else
                {
                    insertBefore = false;
                }

                if (_itemDnD != itemOver) // if we dropped the item on itself, nothing is to be done
                {
                    if (insertBefore)
                    {
                        listViewSelectedFields.Items.Remove(_itemDnD);
                        listViewSelectedFields.Items.Insert(itemOver.Index, _itemDnD);
                    }
                    else
                    {
                        listViewSelectedFields.Items.Remove(_itemDnD);
                        listViewSelectedFields.Items.Insert(itemOver.Index + 1, _itemDnD);
                    }
                }

                // clear the insertion line
                listViewSelectedFields.LineAfter =
                listViewSelectedFields.LineBefore = -1;

                listViewSelectedFields.Invalidate();

            }
            finally
            {
                // finish drag&drop operation
                _itemDnD = null;
                Cursor = Cursors.Default;
            }
        }

        private void listViewSelectedFields_DoubleClick(object sender, EventArgs eventArgs)
        {

            var firstSelectedItem = listViewSelectedFields.SelectedItems[0];
            var currentPowerQueryAttribute = ((PowerQueryAttribute)firstSelectedItem.Tag);
            var attributeLogicName = currentPowerQueryAttribute.Name;

            var attributeDisplayName = firstSelectedItem.Text;
            var attributeMetadata = CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == attributeLogicName).FirstOrDefault();

            
            {

                AttributeFormMessage attributeFormMessage = new AttributeFormMessage(currentPowerQueryAttribute);
                attributeFormMessage.CanAddFormattedValue = false;



                using (var form = new AttributeForm(attributeFormMessage))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var attributeFormResponse = form.attributeFormResponse;
                        listViewSelectedFields.SelectedItems[0].Text = attributeFormResponse.CurrentPowerQueryAttribute.DisplayName;

                        listViewSelectedFields.SelectedItems[0].Tag = attributeFormResponse.CurrentPowerQueryAttribute;
                    }
                }
            }

        }
        #endregion
        #endregion

        #endregion
        #region All Attribute

        private void RefreshAllAttributesList()
        {
            List<ListViewItem> allAttributesList = new List<ListViewItem>(allAttributesListViewItemCache);

            foreach (ListViewItem selectedItem in listViewSelectedFields.Items)
            {
                var selectedItemPowerQueryAttribute = (PowerQueryAttribute)selectedItem.Tag;

                for (int i = allAttributesList.Count - 1; i >= 0; i--)
                {
                    var attributePowerQueryAttribute = (PowerQueryAttribute)allAttributesList[i].Tag;
                    if (attributePowerQueryAttribute.Name == selectedItemPowerQueryAttribute.Name)
                    {
                        allAttributesList.RemoveAt(i);
                    }
                }
            }
            List<ListViewItem> allAttributesListTmp = new List<ListViewItem>(allAttributesList);

            foreach (ListViewItem selectedItem in allAttributesListTmp)
            {
                var powerQueryAttribute = (PowerQueryAttribute)selectedItem.Tag;
                if (!powerQueryAttribute.Name.ToLower().Contains(textBoxAllAttributeFilter.Text.ToLower()) && !powerQueryAttribute.DisplayName.ToLower().Contains(textBoxAllAttributeFilter.Text.ToLower()))
                {
                    allAttributesList.Remove(selectedItem);
                }
            }

            listViewAllFields.Items.Clear();

            listViewAllFields.Items.AddRange(allAttributesList.ToArray());


        }

        private void FillAttributes()
        {
            var entityMetaData = CurrentEntityMetadataWithItems;

            listViewAllFields.Items.Clear();


            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }
            textBoxAllAttributeFilter.Text = "";

            List<ListViewItem> listItems = new List<ListViewItem>();
            foreach (var attribute in entityMetaData.Attributes)
            {
                if (attribute.IsValidForAdvancedFind.Value)
                {


                    bool canAddField = true;
                    foreach (ListViewItem item in listViewAllFields.Items)
                    {
                        if (((PowerQueryAttribute)item.Tag).Name == attribute.LogicalName)
                        {
                            canAddField = false;
                        }
                    }
                    if (canAddField)
                    {
                        PowerQueryAttribute powerQueryAttribute = PowerQueryAttribute.GetPowerQueryAttributeByMetadata(attribute);

                        if (powerQueryAttribute.CanBeAdded)
                        {
                            listItems.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(powerQueryAttribute));
                        }
                    }
                }

            }

            allAttributesListViewItemCache = listItems;
            listViewAllFields.Items.Clear();
            listViewAllFields.Items.AddRange(allAttributesListViewItemCache.ToArray());
            selectedAttributesListViewItemCache = new List<ListViewItem>();



            if (CanClearSelectedGrid == true)
            {
                listViewSelectedFields.Items.Clear();
            }
            else
            {
                CanClearSelectedGrid = true;
            }
            listViewSelectedFields.Items.AddRange(selectedAttributesListViewItemCache.ToArray());
        }
        private void listViewAllFields_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if ((e.ColumnIndex == 0))
            {
                CheckBox cck = new CheckBox();
                // With...
                Text = "";
                Visible = true;
                listViewAllFields.SuspendLayout();
                e.DrawBackground();
                cck.BackColor = Color.Transparent;
                cck.UseVisualStyleBackColor = true;

                cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
                cck.Size = new Size((cck.GetPreferredSize(new Size((e.Bounds.Width - 1), e.Bounds.Height)).Width + 1), e.Bounds.Height);
                cck.Location = new Point(3, 0);
                listViewAllFields.Controls.Add(cck);
                cck.Show();
                cck.BringToFront();
                e.DrawText((TextFormatFlags.VerticalCenter | TextFormatFlags.Left));
                cck.Click += new EventHandler(listViewAllFields_Bink);
                listViewAllFields.ResumeLayout(true);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listViewAllFields_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;

        }

        private void listViewAllFields_Bink(object sender, System.EventArgs e)
        {
            CheckBox test = sender as CheckBox;

            for (int i = 0; i < listViewAllFields.Items.Count; i++)
            {
                listViewAllFields.Items[i].Checked = test.Checked;
            }
        }

        private void listViewAllFields_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;

        }
        private void textBoxAllAttributeFilter_KeyUp(object sender, KeyEventArgs e)
        {
            RefreshAllAttributesList();
        }
        private void textBoxAllAttributeFilter_TextChanged(object sender, EventArgs e)
        {

            RefreshAllAttributesList();
        }
        #endregion
        #region OData

        private void ClearODataResultTab()
        {
            //  return;
            foreach (TabPage tapPage in tabODataResult.TabPages)
            {
                tabODataResult.TabPages.Remove(tapPage);
            }
        }
        string GeneratePowerBIODataQuery()
        {

            List<string> picklistNames = new List<string>();



            //   var selectedEntity = lvEntities.SelectedItems[0].Tag.ToString();
            string entityCollectionName = MetadataHelper.RetrieveEntity(CurrentEntityMetadataWithItems.LogicalName, Service).LogicalCollectionName;
            string step1 = string.Format(@"let
    Source = OData.Feed(ServiceRootURL, null, [Implementation=""2.0""]),
    entity_table = Source{{[Name = ""{0}"", Signature = ""table""]}}[Data],", entityCollectionName);



            string selectedColumns = "";

            string renameColumns = "";

            foreach (ListViewItem listItem in listViewSelectedFields.Items)
            {
                var currentPowerQueryAttribute = (PowerQueryAttribute)listItem.Tag;
                if (!currentPowerQueryAttribute.Name.Contains("@"))
                {
                    if (selectedColumns == "")
                    {
                        selectedColumns = currentPowerQueryAttribute.ODataAttributeDetail.PowerBISelectColumnValue;// string.Format(@"""{0}""", currentPowerQueryAttribute.Name);
                        renameColumns = currentPowerQueryAttribute.ODataAttributeDetail.PowerBIRenameColumnValue; //string.Format(@"{{""{0}"",""{1}""}}", currentPowerQueryAttribute.Name, currentPowerQueryAttribute.DisplayName);
                    }
                    else
                    {
                        selectedColumns = string.Format(@"{0},{1}", selectedColumns, currentPowerQueryAttribute.ODataAttributeDetail.PowerBISelectColumnValue);
                        renameColumns = string.Format(@"{0},{1}", renameColumns, currentPowerQueryAttribute.ODataAttributeDetail.PowerBIRenameColumnValue);
                    }



                }


            }
            string step2 = string.Format(@"    #""Removed Other Columns"" = Table.SelectColumns(entity_table,{{{0}}}),", selectedColumns);

            string step3 = string.Format(@"    #""Renamed Columns"" = Table.RenameColumns( #""Removed Other Columns"",{{{0}}})", renameColumns);
            string lastStep = @"in #""Renamed Columns""";

            var finalMainQuery = step1 + "\n" + step2 + "\n" + step3 + "\n" + lastStep;

            return finalMainQuery;
        }

        #endregion
        #region FechXml


        private void buttonGenerateFetchXml_Click(object sender, EventArgs eventArgs)
        {
        }
        void AddFetchXmlAttributeslistView(PowerQueryAttribute _powerQueryAttribute)
        {
            //listViewSelectedFields
            bool canAddField = true;
            foreach (ListViewItem item in fetchXmlAttributesListViewItemCache)
            {
                if (((PowerQueryAttribute)item.Tag).Name == _powerQueryAttribute.Name)
                {
                    canAddField = false;
                }
            }
            if (canAddField)
            {
                var selectedFieldItem = new ListViewItem { Text = _powerQueryAttribute.DisplayName, Tag = _powerQueryAttribute };
                selectedFieldItem.SubItems.Add(_powerQueryAttribute.Name);
                selectedFieldItem.SubItems.Add(_powerQueryAttribute.Type);
                fetchXmlAttributesListViewItemCache.Add(selectedFieldItem);
            }
            listViewFetchXmlConfig.Items.Clear();
            listViewFetchXmlConfig.Items.AddRange(fetchXmlAttributesListViewItemCache.ToArray());


        }

        private void listViewFetchXmlConfig_DoubleClick(object sender, EventArgs eventArgs)
        {

            var firstSelectedItem = listViewFetchXmlConfig.SelectedItems[0];
            var currentPowerQueryAttribute = ((PowerQueryAttribute)firstSelectedItem.Tag);


            if (currentPowerQueryAttribute.Type == AttributeType.Uniqueidentifier)
            {
                return;
            }


                var attributeLogicName = currentPowerQueryAttribute.Name;

            var attributeDisplayName = firstSelectedItem.Text;
            var attributeMetadata = CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == attributeLogicName).FirstOrDefault();



            if (attributeMetadata != null && attributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata && !attributeLogicName.StartsWith("_") && !attributeLogicName.Contains("."))
            {
                LookupFormMessage lookupFormMessage = new LookupFormMessage(currentPowerQueryAttribute);
                //lookupFormMessage.EntityMetadataWithItems = CurrentEntityMetadataWithItems;


                var formattedPowerQueryAttribute = FetchXmlQueryHelper.FormattedPowerQueryAttribute(currentPowerQueryAttribute);
                var lookupGuidPowerQueryAttribute = FetchXmlQueryHelper.LookupGuidPowerQueryAttribute(currentPowerQueryAttribute);
                var logicalLookupPowerQueryAttribute = FetchXmlQueryHelper.LogicalLookupPowerQueryAttribute(currentPowerQueryAttribute);
                bool canAddGuidField = true;
                bool canAddLookupLogicalName = true;
                bool canAddFormattedName = true;
                foreach (ListViewItem item in listViewFetchXmlConfig.Items)
                {
                    if (((PowerQueryAttribute)item.Tag).Name == lookupGuidPowerQueryAttribute.Name)
                    {
                        canAddGuidField = false;
                    }
                    if (((PowerQueryAttribute)item.Tag).Name == lookupGuidPowerQueryAttribute.Name)
                    {
                        canAddLookupLogicalName = false;
                    }
                    if (((PowerQueryAttribute)item.Tag).Name == formattedPowerQueryAttribute.Name )
                    {
                        canAddFormattedName = false;
                    }
                }
                lookupFormMessage.CanAddLookupGuid = canAddGuidField;
                lookupFormMessage.CanAddLookupLogicalName = canAddLookupLogicalName;

                lookupFormMessage.CanAddFormattedValue = canAddFormattedName;

                using (var form = new AttributeLookupForm(lookupFormMessage))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var attributeFormResponse = form.attributeFormResponse;

                        foreach (var powerQueryAttribute in attributeFormResponse.NewFields)
                        {
                            AddFetchXmlAttributeslistView(powerQueryAttribute);
                        }
                        listViewFetchXmlConfig.SelectedItems[0].Text = attributeFormResponse.CurrentPowerQueryAttribute.DisplayName;
                        listViewFetchXmlConfig.SelectedItems[0].Tag = attributeFormResponse.CurrentPowerQueryAttribute;
                    }
                }
            }
            else
            {

                AttributeFormMessage attributeFormMessage = new AttributeFormMessage(currentPowerQueryAttribute);



                bool canAddFormattedValueField = true;

                var formattedPowerQueryAttribute = FetchXmlQueryHelper.FormattedPowerQueryAttribute(currentPowerQueryAttribute);

                if (formattedPowerQueryAttribute != null)
                {
                    foreach (ListViewItem item in listViewFetchXmlConfig.Items)
                    {
                        var itemPowerQueryAttribute = (PowerQueryAttribute)item.Tag;
                        if (itemPowerQueryAttribute.Name == formattedPowerQueryAttribute.Name
                            || currentPowerQueryAttribute.Type == "FormattedValue"
                            || currentPowerQueryAttribute.Name.StartsWith("_")
                            || currentPowerQueryAttribute.Name.Contains("@"))
                        {
                            canAddFormattedValueField = false;
                            break;
                        }
                    }
                }else
                {
                    canAddFormattedValueField = false;
                }
                attributeFormMessage.CanAddFormattedValue = canAddFormattedValueField;



                using (var form = new AttributeForm(attributeFormMessage))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var attributeFormResponse = form.attributeFormResponse;
                        foreach (var powerQueryAttribute in attributeFormResponse.NewFields)
                        {
                            AddFetchXmlAttributeslistView(powerQueryAttribute);
                        }
                        listViewFetchXmlConfig.SelectedItems[0].Text = attributeFormResponse.CurrentPowerQueryAttribute.DisplayName;

                        listViewFetchXmlConfig.SelectedItems[0].Tag = attributeFormResponse.CurrentPowerQueryAttribute;
                    }
                }
            }
        }


        private void UpdateFetchXmlListView()
        {
            //   listViewFetchXmlConfig.Items.Clear();
            fetchXmlAttributesListViewItemCache = new List<ListViewItem>();

            bool isEntityIdFieldExisted = false;
            string entityIdFieldExisted = CurrentEntityMetadataWithItems.PrimaryIdAttribute;//.PrimaryNameAttribute;  //.LogicalName + "id";
            foreach (ListViewItem item in listViewSelectedFields.Items)
            {
                ListViewItem clonedItem = (ListViewItem)item.Clone();
                var _powerQueryAtt = PowerQueryAttribute.GetNewObject((PowerQueryAttribute) clonedItem.Tag);

                if(entityIdFieldExisted == _powerQueryAtt.Name)
                {
                    isEntityIdFieldExisted = true;
                }

                string displayName = _powerQueryAtt.DisplayName;

                if (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.BooleanAttributeMetadata
                    )
                {
                    string displayname = _powerQueryAtt.DisplayName;

                    _powerQueryAtt.DisplayName = _powerQueryAtt.Name;
                    var _powerQueryAttGuid = FetchXmlQueryHelper.LookupGuidPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                    if (_powerQueryAttGuid != null)
                    {
                        fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttGuid));
                    }

                    if (_powerQueryAtt.Type == "Customer" || _powerQueryAtt.Type == "Owner")
                    {
                        var _powerQueryAttLogicalName = FetchXmlQueryHelper.LogicalLookupPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                        if (_powerQueryAttLogicalName != null)
                        {
                            fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttLogicalName));
                        }
                    }
                        var _powerQueryAttFormatted = FetchXmlQueryHelper.FormattedPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                        if (_powerQueryAttFormatted != null)
                        {
                            _powerQueryAttFormatted.DisplayName = displayname;
                            fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttFormatted));
                        }
                }

                else if (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
                {

                    var _powerQueryAttFormatted = FetchXmlQueryHelper.FormattedPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                    _powerQueryAttFormatted.DisplayName = _powerQueryAtt.DisplayName;
                    if (_powerQueryAttFormatted != null)
                    {
                        fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttFormatted));

                    }
                }

                else if (
                     !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)
                  //   && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
                     && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.MoneyAttributeMetadata)
                     && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.IntegerAttributeMetadata)
                     && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.MemoAttributeMetadata)
                     )
                {

                    var _powerQueryAttFormatted = FetchXmlQueryHelper.FormattedPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                    if (_powerQueryAttFormatted != null)
                    {
                        fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttFormatted));

                    }
                }
                //else if    (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
                //{

                //}
                if (!(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata))
                {
                    if  (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata


                  //  || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata

                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata
                    || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.BooleanAttributeMetadata)
                    {
                        _powerQueryAtt.DisplayName = displayName + $" ({_powerQueryAtt.Name})";
                    }else if(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
                    {
                        _powerQueryAtt.DisplayName = _powerQueryAtt.Name;
                    }
                    fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAtt));
                }
            }
            //Adding EntityId field
            if (!isEntityIdFieldExisted)
            {
                var idAttribute = PowerQueryAttribute.GetPowerQueryAttributeByMetadata(CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == entityIdFieldExisted).FirstOrDefault());
                if (idAttribute != null)
                {
                    fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(idAttribute));
                }
            }
            //listViewFetchXmlConfig.Items.AddRange(fetchXmlAttributesListViewItemCache.ToArray());
        }

        private void linkLabelListViewFetchXmlSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool isSelectAll = (linkLabelListViewFetchXmlSelectAll.Text == "Select All") ? true : false;
            foreach (ListViewItem lvi in listViewFetchXmlConfig.Items)
            {
                lvi.Checked = isSelectAll;
            }
            linkLabelListViewFetchXmlSelectAll.Text = (linkLabelListViewFetchXmlSelectAll.Text == "Select All") ? "Unselect All": "Select All" ;

        }

        private void linkLabelListViewFetchXml_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fetchXmlAttributesListViewItemCache = new List<ListViewItem>(listViewFetchXmlConfig.Items.Cast<ListViewItem>().ToList());

            foreach (ListViewItem lvi in listViewFetchXmlConfig.CheckedItems)
            {
                fetchXmlAttributesListViewItemCache.Remove(lvi);
            }

            ClearFetchXmlTab();
            listViewFetchXmlConfig.Items.AddRange(fetchXmlAttributesListViewItemCache.ToArray());
            txtFetchXml.Text = FetchXml;
        }
        private void ClearFetchXmlTab()
        {
            listViewFetchXmlConfig.Items.Clear();

            txtFetchXml.Text = "";
            txtFetchXml.Text = "";

            foreach (TabPage tapPage in tabFetchXmlResult.TabPages)
            {
                if (tapPage.Text != "FetchXml Config")
                {
                    tabFetchXmlResult.TabPages.Remove(tapPage);
                }
            }
        }
        #region FetchXml Listview DragDrop

        private void listViewFetchXmlConfig_MouseDown(object sender, MouseEventArgs e)
        {

            _itemDnD = listViewFetchXmlConfig.GetItemAt(e.X, e.Y);
        }

        private void listViewFetchXmlConfig_MouseMove(object sender, MouseEventArgs e)
        {

            if (_itemDnD == null)
                return;

            // Show the user that a drag operation is happening
            Cursor = Cursors.Hand;

            // calculate the bottom of the last item in the LV so that you don't have to stop your drag at the last item
            int lastItemBottom = Math.Min(e.Y, listViewFetchXmlConfig.Items[listViewFetchXmlConfig.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1);

            // use 0 instead of e.X so that you don't have to keep inside the columns while dragging
            ListViewItem itemOver = listViewFetchXmlConfig.GetItemAt(0, lastItemBottom);

            if (itemOver == null)
                return;

            Rectangle rc = itemOver.GetBounds(ItemBoundsPortion.Entire);
            if (e.Y < rc.Top + (rc.Height / 2))
            {
                listViewFetchXmlConfig.LineBefore = itemOver.Index;
                listViewFetchXmlConfig.LineAfter = -1;
            }
            else
            {
                listViewFetchXmlConfig.LineBefore = -1;
                listViewFetchXmlConfig.LineAfter = itemOver.Index;
            }

            // invalidate the LV so that the insertion line is shown
            listViewFetchXmlConfig.Invalidate();
        }

        private void listViewFetchXmlConfig_MouseUp(object sender, MouseEventArgs e)
        {


            if (_itemDnD == null)
                return;

            try
            {
                // calculate the bottom of the last item in the LV so that you don't have to stop your drag at the last item
                int lastItemBottom = Math.Min(e.Y, listViewFetchXmlConfig.Items[listViewFetchXmlConfig.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1);

                // use 0 instead of e.X so that you don't have to keep inside the columns while dragging
                ListViewItem itemOver = listViewFetchXmlConfig.GetItemAt(0, lastItemBottom);

                if (itemOver == null)
                    return;

                Rectangle rc = itemOver.GetBounds(ItemBoundsPortion.Entire);

                // find out if we insert before or after the item the mouse is over
                bool insertBefore;
                if (e.Y < rc.Top + (rc.Height / 2))
                {
                    insertBefore = true;
                }
                else
                {
                    insertBefore = false;
                }

                if (_itemDnD != itemOver) // if we dropped the item on itself, nothing is to be done
                {
                    if (insertBefore)
                    {
                        listViewFetchXmlConfig.Items.Remove(_itemDnD);
                        listViewFetchXmlConfig.Items.Insert(itemOver.Index, _itemDnD);
                    }
                    else
                    {
                        listViewFetchXmlConfig.Items.Remove(_itemDnD);
                        listViewFetchXmlConfig.Items.Insert(itemOver.Index + 1, _itemDnD);
                    }
                }

                // clear the insertion line
                listViewFetchXmlConfig.LineAfter =
                listViewFetchXmlConfig.LineBefore = -1;

                listViewFetchXmlConfig.Invalidate();

            }
            finally
            {
                // finish drag&drop operation
                _itemDnD = null;
                Cursor = Cursors.Default;
            }
        }
        #endregion
        #endregion
        #region ServiceURL

        private void GenerateSeviceURL(TabControl tabControl)
        {
            RetrieveCurrentOrganizationResponse retrieveCurrentOrganizationResponse = (RetrieveCurrentOrganizationResponse)base.Service.Execute(new RetrieveCurrentOrganizationRequest());
            string arg = ((DataCollection<Microsoft.Xrm.Sdk.Organization.EndpointType, string>)retrieveCurrentOrganizationResponse.Detail.Endpoints)[Microsoft.Xrm.Sdk.Organization.EndpointType.WebApplication];
            Version version = Version.Parse(retrieveCurrentOrganizationResponse.Detail.OrganizationVersion);
            // string ServiceAPIURL = $"{arg}api/data/v{version.ToString(2)}";

            string versionVal = version.ToString(2);// (version.ToString(2) == "9.0") ? "8.2" : version.ToString(2);
                                                    //   string ServiceAPIURL = $"{arg}api/data/v{versionVal}";
                                                       string ServiceAPIURL = $@"=Dyn365CEBaseURL & ""/api/data/v{versionVal}""";



            CreateTabPage(tabControl, $"tabPageDyn365CEBaseURL", $"Dyn365CEBaseURL", $"txt_tabPageOrgURL", arg.Remove(arg.Length-1));

            CreateTabPage(tabControl, $"tabPageServiceRootURL", $"ServiceRootURL", $"txt_tabPageServiceRootURL", ServiceAPIURL);

        }


        private void toolStripButtonGenerateServiceRootURL_Click(object sender, EventArgs e)
        {

            ClearServiceRootURLTab();

            GeneratePowerBIServiceRootURL();

            tabMain.SelectedTab = tabPageServiceRootURL;
        }
        private void GeneratePowerBIServiceRootURL()
        {
            GenerateSeviceURL(tabControlServiceRootURL);
        }
        private void ClearServiceRootURLTab()
        {
            foreach (TabPage tapPage in tabControlServiceRootURL.TabPages)
            {
                tabControlServiceRootURL.TabPages.Remove(tapPage);
            }
        }
        #endregion
        #region OptionSet

        private void GenerateEnumOptionSet(TabControl _tabControl, AttributeMetadata attributeMetadata)
        {

            string str =
$@"let
    Source = Json.Document(Web.Contents(ServiceRootURL & ""/GlobalOptionSetDefinitions({attributeMetadata.MetadataId.Value.ToString()})"")),
    Options = Source[Options],
    #""Converted to Table"" = Table.FromList(Options, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", {{""Label"", ""Value""}}, {{""Label"", ""Value""}}),
    #""Expanded Column1.Label"" = Table.ExpandRecordColumn(#""Expanded Column1"", ""Label"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded Column1.Label.UserLocalizedLabel"" = Table.ExpandRecordColumn(#""Expanded Column1.Label"", ""UserLocalizedLabel"", {{""Label""}}, {{""Budget""}})
in
    #""Expanded Column1.Label.UserLocalizedLabel""";

            CreateTabPage(_tabControl, $"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }
        private void GenerateStateOptionSet(TabControl _tabControl, AttributeMetadata attributeMetadata)
        {
            string str =
$@"let
    Source = Json.Document(Web.Contents(ServiceRootURL & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.StateAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
    OptionSet = Source[OptionSet],
    Options = OptionSet[Options],
    #""Converted to Table"" = Table.FromList(Options, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", {{""Value"", ""Color"", ""Label"", ""Description""}}, {{""Value"", ""Color"", ""Label"", ""Description""}}),
    #""Expanded Label"" = Table.ExpandRecordColumn(#""Expanded Column1"", ""Label"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel"" = Table.ExpandRecordColumn(#""Expanded Label"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label""}}),
    #""Expanded Description"" = Table.ExpandRecordColumn(#""Expanded UserLocalizedLabel"", ""Description"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel1"" = Table.ExpandRecordColumn(#""Expanded Description"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label.1""}}),
    #""Renamed Columns"" = Table.RenameColumns(#""Expanded UserLocalizedLabel1"",{{{{""Label.1"", ""Description""}}}})
in
    #""Renamed Columns""";

            CreateTabPage(_tabControl, $"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }
        private void GenerateStatusOptionSet(TabControl _tabControl, AttributeMetadata attributeMetadata)
        {
            string str = $@"let
    Source = Json.Document(Web.Contents(ServiceRootURL & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
    OptionSet = Source[OptionSet],
    Options = OptionSet[Options],
    #""Converted to Table"" = Table.FromList(Options, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", {{""Value"", ""Color"", ""Label"", ""Description""}}, {{""Value"", ""Color"", ""Label"", ""Description""}}),
    #""Expanded Label"" = Table.ExpandRecordColumn(#""Expanded Column1"", ""Label"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel"" = Table.ExpandRecordColumn(#""Expanded Label"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label""}}),
    #""Expanded Description"" = Table.ExpandRecordColumn(#""Expanded UserLocalizedLabel"", ""Description"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel1"" = Table.ExpandRecordColumn(#""Expanded Description"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label.1""}}),
    #""Renamed Columns"" = Table.RenameColumns(#""Expanded UserLocalizedLabel1"",{{{{""Label.1"", ""Description""}}}})
in
    #""Renamed Columns""";

            CreateTabPage(_tabControl, $"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }

        private void GenerateLocalOptionSet(TabControl _tabControl, AttributeMetadata attributeMetadata)
        {
            string str =
 $@"let
    Source = Json.Document(Web.Contents(ServiceRootURL & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
    OptionSet = Source[OptionSet],
    Options = OptionSet[Options],
    #""Converted to Table"" = Table.FromList(Options, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", {{""Value"", ""Color"", ""Label"", ""Description""}}, {{""Value"", ""Color"", ""Label"", ""Description""}}),
    #""Expanded Label"" = Table.ExpandRecordColumn(#""Expanded Column1"", ""Label"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel"" = Table.ExpandRecordColumn(#""Expanded Label"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label""}}),
    #""Expanded Description"" = Table.ExpandRecordColumn(#""Expanded UserLocalizedLabel"", ""Description"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded UserLocalizedLabel1"" = Table.ExpandRecordColumn(#""Expanded Description"", ""UserLocalizedLabel"", {{""Label""}}, {{""Label.1""}}),
    #""Renamed Columns"" = Table.RenameColumns(#""Expanded UserLocalizedLabel1"",{{{{""Label.1"", ""Description""}}}})
in
    #""Renamed Columns""";

            CreateTabPage(_tabControl, $"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }

        private void GeneratePowerBIOptionSet()
        {
            foreach (ListViewItem listViewItem in selectedAttributesListViewItemCache)
            {
                var currentPowerQueryAttribute = (PowerQueryAttribute)listViewItem.Tag;
                var fieldMetadata = CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == currentPowerQueryAttribute.Name.ToString()).FirstOrDefault();


                if (fieldMetadata != null)
                {
                    if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata))
                    {
                        GenerateLocalOptionSet(tabControlOptionSet, fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata))
                    {
                        GenerateStatusOptionSet(tabControlOptionSet, fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata))
                    {
                        GenerateStateOptionSet(tabControlOptionSet, fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.EnumAttributeMetadata))
                    {
                        GenerateEnumOptionSet(tabControlOptionSet, fieldMetadata);
                    }
                }
            }
        }

        private void ClearOptionSetTab()
        {
            foreach (TabPage tapPage in tabControlOptionSet.TabPages)
            {
                tabODataResult.TabPages.Remove(tapPage);
            }
        }
        #endregion

        private void tsbUpdateFetchXml_Click(object sender, EventArgs e)
        {
             ClearFetchXmlTab();
            UpdateFetchXmlListView();

            AddLinkEntities();
            listViewFetchXmlConfig.Items.Clear();
            listViewFetchXmlConfig.Items.AddRange(fetchXmlAttributesListViewItemCache.ToArray());
            tabMain.SelectedTab = tabPageFetchXml;
            txtFetchXml.Text = FetchXml;
        }

        private void AddLinkEntities()
        {
            foreach (var linkEntity in FetchXmlHelper.GetLinkedEntitiesAttributes( FetchXml, Service))
            {


                string displayNameAttributeMetadataInMainEntity = linkEntity.LinkedAttributeMetadata.LogicalName;

                if (linkEntity.LinkedAttributeMetadata.DisplayName != null && linkEntity.LinkedAttributeMetadata.DisplayName.UserLocalizedLabel != null && linkEntity.LinkedAttributeMetadata.DisplayName.UserLocalizedLabel.Label != null)
                {

                    displayNameAttributeMetadataInMainEntity = linkEntity.LinkedAttributeMetadata.DisplayName.UserLocalizedLabel.Label;
                }

                foreach (var _powerQueryAtt in linkEntity.PowerQueryAttributeList)
                {
                    // fetchXmlAttributesListViewItemCache(powerQueryAttribute);
                   
                    if (true)
                    {








                        string displayName = _powerQueryAtt.DisplayName;

                        if (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.BooleanAttributeMetadata
                            )
                        {
                            string displayname = _powerQueryAtt.DisplayName;

                            _powerQueryAtt.DisplayName = _powerQueryAtt.Name;
                            var _powerQueryAttGuid = FetchXmlQueryHelper.LookupGuidPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                            if (_powerQueryAttGuid != null)
                            {

                                _powerQueryAttGuid.DisplayName = _powerQueryAttGuid.DisplayName + $" ({displayNameAttributeMetadataInMainEntity})";
                                fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttGuid));
                            }

                            if (_powerQueryAtt.Type == "Customer" || _powerQueryAtt.Type == "Owner")
                            {
                                var _powerQueryAttLogicalName = FetchXmlQueryHelper.LogicalLookupPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                                if (_powerQueryAttLogicalName != null)
                                {
                                    _powerQueryAttLogicalName.DisplayName = _powerQueryAttLogicalName.DisplayName + $" ({displayNameAttributeMetadataInMainEntity})";
                                    fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttLogicalName));
                                }
                            }
                            var _powerQueryAttFormatted = FetchXmlQueryHelper.FormattedPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                            if (_powerQueryAttFormatted != null)
                            {
                                _powerQueryAttFormatted.DisplayName = _powerQueryAttFormatted.DisplayName + $" ({displayNameAttributeMetadataInMainEntity})";
                                fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttFormatted));
                            }
                        }
                        else if (
                             !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)
                             && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata)
                             && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.MoneyAttributeMetadata)
                             && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.IntegerAttributeMetadata)
                             && !(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.MemoAttributeMetadata)
                             )
                        {

                            var _powerQueryAttFormatted = FetchXmlQueryHelper.FormattedPowerQueryAttribute(PowerQueryAttribute.GetNewObject(_powerQueryAtt));
                            if (_powerQueryAttFormatted != null)
                            {
                                _powerQueryAttFormatted.DisplayName = _powerQueryAttFormatted.DisplayName + $" ({displayNameAttributeMetadataInMainEntity})";
                                fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAttFormatted));

                            }
                        }
                        if (!(_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata))
                        {
                            if (_powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata
                            || _powerQueryAtt.AttributeMetadata is Microsoft.Xrm.Sdk.Metadata.BooleanAttributeMetadata)
                            {
                                _powerQueryAtt.DisplayName = displayName + $" ({displayNameAttributeMetadataInMainEntity})";
                            }
                            fetchXmlAttributesListViewItemCache.Add(PowerQueryAttribute.GetListViewItemByPowerQueryAttribute(_powerQueryAtt));
                        }











                    }
                }
            }
        }

        public event EventHandler<XrmToolBox.Extensibility.MessageBusEventArgs> OnOutgoingMessage;

        public void OnIncomingMessage(XrmToolBox.Extensibility.MessageBusEventArgs message)
        {
            if (message.SourcePlugin == "FetchXML Builder" &&
                message.TargetArgument is string)
            {
              SetFetchXml(  message.TargetArgument);
            }
        }
        private void GetFromFXB()
        {
            var messageBusEventArgs = new MessageBusEventArgs("FetchXML Builder")
            {
                //SourcePlugin = "Bulk Data Updater"
            };
            messageBusEventArgs.TargetArgument = FetchXml;
            OnOutgoingMessage(this, messageBusEventArgs);
        }
        private void toolStripButtonLoadFechXml_Click(object sender, EventArgs e)
        {
            return;
            
        }

        private void SetFetchXml(string fetchXml)
        {
            FetchXml = fetchXml;


            XmlDocument fetchDoc = new XmlDocument();
            fetchDoc.LoadXml(FetchXml);

            string entityName = fetchDoc?.DocumentElement?.SelectSingleNode("/fetch/entity").Attributes["name"].Value;


            lvEntitiesSelectedIndexChanged(entityName, FetchXml);
            return;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading FetchXml items...",
                Work = (bw, evt) =>
                {
                    //     lvEntitiesSelectedIndexChanged();


                    List<Entity> viewsList = ViewHelper.RetrieveViews(entityName, entitiesCache, Service);

                    if (CurrentEntityMetadataWithItems == null)
                        CurrentEntityMetadataWithItems = MetadataHelper.RetrieveEntity(entityName, Service);
                    evt.Result = viewsList;

                },
                PostWorkCallBack = evt =>
                {
                    if (evt.Error != null)
                    {
                        MessageBox.Show(ParentForm, "Error while displaying FetchXml: " + evt.Error.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {


                        /*  
                                               foreach (ListViewItem item in lvEntities.Items)
                                               {
                                                   if (item.Tag.ToString() == entityName)
                                                   {
                                                       item.Selected = true;
                                                       break;
                                                   }
                                               }
                                                return;
                                                   List<Entity> viewsList = (List<Entity>)evt.Result;
                                                   FillViews(entityName, viewsList);



                                                   gbSourceViews.Enabled = true;
                                                   lvSourceViews.Items.AddRange(sourceViewsItems.ToArray());

                                                   FillAttributes();
                                                   EnableVisableListViewSelectedFields();
                                                   */


                        Cursor = Cursors.Default;
                    }
                }
            });

        }

        private void editFetchXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var form = new FetchXml.FetchXmlForm(FetchXml))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var fetchXml = form.FetchXml;


                    SetFetchXml(fetchXml);
                }
            }
        }

        private void openFetchXmlBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetFromFXB();
        }
    }
}
