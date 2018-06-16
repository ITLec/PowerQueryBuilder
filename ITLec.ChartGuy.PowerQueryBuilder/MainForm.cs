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

namespace ITLec.ChartGuy.PowerQueryBuilder
{
    public partial class MainForm : PluginControlBase, IGitHubPlugin, IHelpPlugin
    {
        private List<EntityMetadata> entitiesCache;
        private ListViewItem[] listViewItemsCache;
        private List<ListViewItem> sourceViewsItems;
        private List<ListViewItem> targetViewsItems;
        EntityMetadata CurrentEntityMetadataWithItems = null;
        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            var tt = new ToolTip();
            tt.SetToolTip(lvSourceViews, "Double click on a selected row to display its layout XML");
        }

        #endregion Constructor

        #region Main ToolStrip Handlers

        #region Fill Entities

        private void LoadEntities()
        {
            txtSearchEntity.Text = string.Empty;
            lvEntities.Items.Clear();
            listViewAllFields.Items.Clear();
            listViewSelectedFields.Items.Clear();
            gbEntities.Enabled = false;
            tsbPublishEntity.Enabled = false;
            tsbPublishAll.Enabled = false;

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
        }

        #endregion Fill Entities

        #region Save Views

        private void TsbSaveViewsClick(object sender, EventArgs e)
        {
            tsbPublishEntity.Enabled = false;
            tsbPublishAll.Enabled = false;
            tsbLoadEntities.Enabled = false;

            //var targetViews = lvTargetViews.CheckedItems.Cast<ListViewItem>().Select(i => new ViewDefinition((Entity)i.Tag)).ToList();
            var sourceView = new ViewDefinition((Entity)lvSourceViews.SelectedItems.Cast<ListViewItem>().First().Tag);
            
        }

        #endregion Save Views
        

        #endregion Main ToolStrip Handlers

        #region ListViews Handlers

        #region Fill Views

        private void BwFillViewsDoWork(object sender, DoWorkEventArgs e)
        {
            string entityName = e.Argument.ToString(); ;
            FillViews(entityName);
        }

        private void FillAttributes(string entityName)
        {
          var entityMetaData =  MetadataHelper.RetrieveEntity(entityName, Service);
            listViewAllFields.Items.Clear();
            listViewSelectedFields.Items.Clear();
            foreach(var attribute in entityMetaData.Attributes)
            {

                string displayName = attribute.LogicalName;

                if(attribute.DisplayName != null && attribute.DisplayName.UserLocalizedLabel != null && attribute.DisplayName.UserLocalizedLabel.Label != null)
                {

                    displayName = attribute.DisplayName.UserLocalizedLabel.Label;
                }

                AddItemTolistView(listViewAllFields,attribute.LogicalName, displayName, attribute.AttributeType.Value.ToString());
                //var selectedFieldItem = new ListViewItem { Text = displayName, Tag = attribute.LogicalName };
                //selectedFieldItem.SubItems.Add(attribute.LogicalName);

                //listViewAllFields.Items.Add(selectedFieldItem);
            }
        }

        private void FillViews(string entityName)
        {
            string entityLogicalName = entityName;

            List<Entity> viewsList = ViewHelper.RetrieveViews(entityLogicalName, entitiesCache, Service);
            viewsList.AddRange(ViewHelper.RetrieveUserViews(entityLogicalName, entitiesCache, Service));

            sourceViewsItems = new List<ListViewItem>();
            targetViewsItems = new List<ListViewItem>();

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

                    targetViewsItems.Add(clonedItem);
                    //ListViewDelegates.AddItem(lvTargetViews, clonedItem);
                }
            }
        }

        private void BwFillViewsRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            gbSourceViews.Enabled = true;
            //kgbTargetViews.Enabled = true;

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

            lvSourceViews.Items.AddRange(sourceViewsItems.ToArray());
            //lvTargetViews.Items.AddRange(targetViewsItems.ToArray());
        }

        private void lvEntities_SelectedIndexChanged(object sender, EventArgs e)
        {

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Entity Attributes...",
                Work = (bw, evt) =>
                {

                    if (lvEntities.SelectedItems.Count > 0)
                    {
                        string entityLogicalName = lvEntities.SelectedItems[0].Tag.ToString();

                        // Reinit other controls
                        lvSourceViews.Items.Clear();

                        Cursor = Cursors.WaitCursor;

                        // Launch treatment
                        var bwFillViews = new BackgroundWorker();
                        bwFillViews.DoWork += BwFillViewsDoWork;
                        bwFillViews.RunWorkerAsync(entityLogicalName);
                        bwFillViews.RunWorkerCompleted += BwFillViewsRunWorkerCompleted;

                        
                        CurrentEntityMetadataWithItems = MetadataHelper.RetrieveEntity(entityLogicalName, Service);

                        FillAttributes(entityLogicalName);
                    }
                }
            });

            ClearFinalResultTab();
        }

        #endregion Fill Views

        #region Display View

        private void LvSourceViewsSelectedIndexChanged(object sender, EventArgs e)
        {

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

                        XmlDocument layoutDoc = new XmlDocument();
                        layoutDoc.LoadXml(layoutXml);



                        var headers = new List<ColumnHeader>();
                        
                        

                        var selectedItems = listViewSelectedFields.Items;
                        //CopyItemsFromSelectedFieldsToAllFields(selectedItems);

                        foreach (ListViewItem item in selectedItems)
                        {
                            MoveItemFormListViewToAnother(listViewSelectedFields, listViewAllFields, item.Tag.ToString());
                        }

                        foreach (XmlNode columnNode in layoutDoc.SelectNodes("grid/row/cell"))
                        {

                            /////////////


                            if (!columnNode.Attributes["name"].Value.Contains("."))
                            {
                                string fieldDisplayName = MetadataHelper.RetrieveAttributeDisplayName(CurrentEntityMetadataWithItems,
                                                                                           columnNode.Attributes["name"].Value,
                                                                                           fetchXml, Service);



                                //AddItemTolistView(listViewSelectedFields,columnNode.Attributes["name"].Value, fieldDisplayName);

                                MoveItemFormListViewToAnother(listViewAllFields, listViewSelectedFields, columnNode.Attributes["name"].Value);

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
                    }
                });
            }
        }

        private void LvTargetViewsItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked && e.Item.ForeColor == Color.Gray)
            {
                MessageBox.Show(this, "This view has not been defined as customizable. It can't be customized!",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Item.Checked = false;
            }

            //if (ListViewDelegates.GetCheckedItems(lvTargetViews).Length > 0)
            //{
            //    tsbSaveViews.Enabled = true;
            //    tsbPublishEntity.Enabled = true;
            //}
            //else
            //{
            //    tsbSaveViews.Enabled = false;
            //    tsbPublishEntity.Enabled = false;
            //}
        }

        #endregion Display View

        #endregion ListViews Handlers

        private void LvEntitiesColumnClick(object sender, ColumnClickEventArgs e)
        {

            lvEntities.Sorting = lvEntities.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            lvEntities.ListViewItemSorter = new ListViewItemComparer(e.Column, lvEntities.Sorting);
        }

        private void LvSourceViewsDoubleClick(object sender, EventArgs e)
        {
            if (lvSourceViews.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvSourceViews.SelectedItems[0];
            var view = (Entity)item.Tag;

            var dialog = new XmlContentDisplayDialog(view["layoutxml"].ToString());
            dialog.ShowDialog(this);
        }

        private void OnSearchKeyUp(object sender, KeyEventArgs e)
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
                    .Where(item => item.Text.StartsWith(entityName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                lvEntities.Items.AddRange(filteredItems);
                lvEntities.EndUpdate();
            }
        }

        private void TsbCloseThisTabClick(object sender, EventArgs e)
        {
            CloseTool();
        }



        private void chkShowSystem_CheckedChanged(object sender, EventArgs e)
        {
          //  FilterTargetViews(chkShowSystem.Checked, chkShowUser.Checked);
         }

        private void chkShowUser_CheckedChanged(object sender, EventArgs e)
        {
            //FilterTargetViews(chkShowSystem.Checked, chkShowUser.Checked);
        }

        private void FilterTargetViews(bool showSystem, bool showUser)
        {
            var filteredViews = targetViewsItems.Where(v =>
                ((Entity) v.Tag).LogicalName == "savedquery" && showSystem
                || ((Entity) v.Tag).LogicalName == "userquery" && showUser
                );

            //lvTargetViews.Items.Clear();

            //lvTargetViews.Items.AddRange(filteredViews.ToArray());
        }

        public string RepositoryName { get { return "ITLec.ChartGuy.PowerQueryBuilder"; } }
        public string UserName { get { return "MscrmTools"; } }
        public string HelpUrl { get { return "https://github.com/ITLec/ITLec.ChartGuy.PowerQueryBuilder/wiki"; } }

        private void buttonAddToSelectedList_Click(object sender, EventArgs e)
        {
            var selectedItems = listViewAllFields.SelectedItems;
         //   CopyItemsFromAllFieldsToSelectedFields(selectedItems);

            foreach(ListViewItem item in selectedItems)
            {
                MoveItemFormListViewToAnother(listViewAllFields, listViewSelectedFields, item.Tag.ToString());
            }

            var checkedItems = listViewAllFields.CheckedItems;
            // CopyItemsFromAllFieldsToSelectedFields(checkedItems);


            foreach (ListViewItem item in checkedItems)
            {
                MoveItemFormListViewToAnother(listViewAllFields, listViewSelectedFields, item.Tag.ToString());
            }
        }
        

        private void buttonRemoveFromSelectedFields_Click(object sender, EventArgs e)
        {

            var selectedItems = listViewSelectedFields.SelectedItems;
            //CopyItemsFromSelectedFieldsToAllFields(selectedItems);

            foreach (ListViewItem item in selectedItems)
            {
                MoveItemFormListViewToAnother(listViewSelectedFields, listViewAllFields, item.Tag.ToString());
            }

            var checkedItems = listViewSelectedFields.CheckedItems;
            //    CopyItemsFromSelectedFieldsToAllFields(checkedItems);


            foreach (ListViewItem item in checkedItems)
            {
                MoveItemFormListViewToAnother( listViewSelectedFields, listViewAllFields, item.Tag.ToString());
            }
        }
        

        private void CopyItemsFromSelectedFieldsToAllFields(ListView.SelectedListViewItemCollection selectedItems)
        {
            foreach (ListViewItem selectedItem in selectedItems)
            {
                listViewSelectedFields .Items.Remove(selectedItem);
                listViewAllFields.Items.Add(selectedItem);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
           if( tabMain.SelectedIndex == 0)
            {
                tsbLoadEntities.Visible = true;
            }
           else
            {
                tsbLoadEntities.Visible = false;
            }
        }

        private void tsbGenerate_Click(object sender, EventArgs e)
        {
            ClearFinalResultTab();
            tabControlResult.SelectTab("tabPageFinalResult");
            GeneratePowerBIODataQuery();

        }

        private void ClearFinalResultTab()
        {
           foreach( TabPage tapPage in tabControlResult.TabPages)
            {
                tabControlResult.TabPages.Remove(tapPage);
            }
        }

        void GeneratePowerBIODataQuery()
        {
            GenerateSeviceURL();


            List<string> picklistNames = new List<string>();



         //   var selectedEntity = lvEntities.SelectedItems[0].Tag.ToString();
            string entityCollectionName = MetadataHelper.RetrieveEntity(CurrentEntityMetadataWithItems.LogicalName, Service).LogicalCollectionName;
            string step1 = string.Format(@"let
    Source = OData.Feed(ServiceRootUrl),
    entity_table = Source{{[Name = ""{0}"", Signature = ""table""]}}[Data],", entityCollectionName);

            string selectedColumns = "";

            string renameColumns = "";

            foreach (ListViewItem listItem in listViewSelectedFields.Items)
            {
                if (selectedColumns == "")
                {
                    selectedColumns = string.Format(@"""{0}""", listItem.Tag);
                    renameColumns = string.Format(@"{{""{0}"",""{1}""}}", listItem.Tag, listItem.Text);
                }
                else
                {
                    selectedColumns = string.Format(@"{0},""{1}""", selectedColumns, listItem.Tag);
                    renameColumns = string.Format(@"{0},{{""{1}"",""{2}""}}", renameColumns, listItem.Tag, listItem.Text);
                }



                var fieldMetadata = CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == listItem.Tag.ToString()).FirstOrDefault();


                if (fieldMetadata != null)
                {
                    if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata))
                    {
                        GenerateLocalOptionSet(fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata))
                    {
                        GenerateStatusOptionSet(fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata))
                    {
                        GenerateStateOptionSet(fieldMetadata);
                    }
                    else if (fieldMetadata.GetType() == typeof(Microsoft.Xrm.Sdk.Metadata.EnumAttributeMetadata))
                    {
                        GenerateEnumOptionSet(fieldMetadata);
                    }
                }


            }
            string step2 = string.Format(@"    #""Removed Other Columns"" = Table.SelectColumns(entity_table,{{{0}}}),", selectedColumns);
            
            string step3 = string.Format(@"    #""Renamed Columns"" = Table.RenameColumns( #""Removed Other Columns"",{{{0}}})", renameColumns);
            string lastStep = @"in #""Renamed Columns""";
            
            var finalMainQuery = step1 + "\n" + step2 + "\n" + step3 + "\n" + lastStep;


            CreateTabPage("tabPageMainQuery", $"Main Query ({CurrentEntityMetadataWithItems.DisplayName.UserLocalizedLabel.Label}) - ({CurrentEntityMetadataWithItems.LogicalName})", "txt_tabPageMainQuery", finalMainQuery);



            tabControlResult.Refresh();
        }

        private void GenerateSeviceURL()
        {

            RetrieveCurrentOrganizationResponse retrieveCurrentOrganizationResponse = (RetrieveCurrentOrganizationResponse)base.Service.Execute(new RetrieveCurrentOrganizationRequest());
            string arg = ((DataCollection<Microsoft.Xrm.Sdk.Organization.EndpointType, string>)retrieveCurrentOrganizationResponse.Detail.Endpoints)[Microsoft.Xrm.Sdk.Organization.EndpointType.WebApplication];
            Version version = Version.Parse(retrieveCurrentOrganizationResponse.Detail.OrganizationVersion);
           // string ServiceAPIURL = $"{arg}api/data/v{version.ToString(2)}";
            string ServiceAPIURL = $"{arg}api/data/v8.2";

            
            CreateTabPage($"tabPageServiceRootURL", $"ServiceRootURL", $"txt_tabPageServiceRootURL", ServiceAPIURL);

        }

        private void GenerateEnumOptionSet(AttributeMetadata attributeMetadata)
        {
            
            string str =
$@"let
    Source = Json.Document(Web.Contents(ServiceRootUrl & ""/GlobalOptionSetDefinitions({attributeMetadata.MetadataId.Value.ToString()})"")),
    Options = Source[Options],
    #""Converted to Table"" = Table.FromList(Options, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", {{""Label"", ""Value""}}, {{""Label"", ""Value""}}),
    #""Expanded Column1.Label"" = Table.ExpandRecordColumn(#""Expanded Column1"", ""Label"", {{""UserLocalizedLabel""}}, {{""UserLocalizedLabel""}}),
    #""Expanded Column1.Label.UserLocalizedLabel"" = Table.ExpandRecordColumn(#""Expanded Column1.Label"", ""UserLocalizedLabel"", {{""Label""}}, {{""Budget""}})
in
    #""Expanded Column1.Label.UserLocalizedLabel""";

            CreateTabPage($"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }

        private void GenerateStateOptionSet(AttributeMetadata attributeMetadata)
        {
            string str =
$@"let
    Source = Json.Document(Web.Contents(ServiceRootUrl & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.StateAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
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

            CreateTabPage($"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }
        private void GenerateStatusOptionSet(AttributeMetadata attributeMetadata)
        {
            string str = $@"let
    Source = Json.Document(Web.Contents(ServiceRootUrl & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
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

            CreateTabPage($"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }


        private void CreateTabPage(string tabPageName, string tabPageText, string textBoxFieldName, string queryString)
        {

            if (!tabControlResult.TabPages.ContainsKey(tabPageName))
            {
                TabPage tabPage = new TabPage();
                tabPage.Name = tabPageName;
                tabPage.Text = tabPageText;
                System.Windows.Forms.RichTextBox txtMainQuery = new System.Windows.Forms.RichTextBox();
                txtMainQuery.Name = textBoxFieldName;
                txtMainQuery.Dock = DockStyle.Fill;
                tabPage.Controls.Add(txtMainQuery);
                tabControlResult.TabPages.Add(tabPage);

                txtMainQuery.Text = queryString;
            }
            else
            {
                TabPage tapPageMainQuery = tabControlResult.TabPages[tabPageName];

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

        private void GenerateLocalOptionSet(AttributeMetadata attributeMetadata)
        {
            string str =
 $@"let
    Source = Json.Document(Web.Contents(ServiceRootUrl & ""/EntityDefinitions(LogicalName='{CurrentEntityMetadataWithItems.LogicalName}')/Attributes(LogicalName='{attributeMetadata.LogicalName}')/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=LogicalName&$expand=OptionSet($select=Options)"")),
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

            CreateTabPage($"tabPage{attributeMetadata.LogicalName}", $"{attributeMetadata.DisplayName.UserLocalizedLabel.Label} ({attributeMetadata.LogicalName})", $"txt_tabPage{attributeMetadata.LogicalName}", str);
        }
        


        // The LVItem being dragged
        private ListViewItem _itemDnD = null;
        private void listViewSelectedFields_MouseDown(object sender, MouseEventArgs e)
        {

            _itemDnD = listViewSelectedFields.GetItemAt(e.X, e.Y);
            // if the LV is still empty, no item will be found anyway, so we don't have to consider this case
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

            var attributeLogicName = firstSelectedItem.Tag.ToString();

            var attributeDisplayName = firstSelectedItem.Text;

            AttributeFormMessage attributeFormMessage = new AttributeFormMessage();
            attributeFormMessage.AttributeLogicName = attributeLogicName;
            attributeFormMessage.EntityMetadataWithItems = CurrentEntityMetadataWithItems;
            attributeFormMessage.CurrentAttributeDisplayName = attributeDisplayName;
            var attributeMetadata = CurrentEntityMetadataWithItems.Attributes.Where(e => e.LogicalName == attributeLogicName).FirstOrDefault();


            if (attributeMetadata != null)
            {
                if (attributeMetadata is Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)
                {
                    var fieldGuidName = $"_{attributeFormMessage.AttributeLogicName}_value";

                    bool canAddGuidField = true;
                   foreach(ListViewItem item in listViewSelectedFields.Items)
                    {
                        if(item.Tag.ToString() == fieldGuidName)
                        {
                            canAddGuidField = false;
                        }
                    }
                    attributeFormMessage.CanAddLookupGuid = canAddGuidField;
                }
            }

            using (var form = new AttributeForm(attributeFormMessage))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var attributeFormResponse = form.attributeFormResponse;

                    foreach(string itemName in attributeFormResponse.NewFields)
                    {
                        AddItemTolistView(listViewSelectedFields, itemName, itemName,"Guid");
                    }
                    listViewSelectedFields.SelectedItems[0].Text = attributeFormResponse.CurrentAttributeDisplayName;
                }
            }
        }

        void MoveItemFormListViewToAnother(ListView fromListView, ListView toListView, string attributeLogicalName)
        {
            var listViewItem = RemoveItemlistView(fromListView, attributeLogicalName);

            AddItemTolistView(toListView, listViewItem);
        }


        void AddItemTolistView(ListView _listView, string itemName, string itemDisplayName, string type)
        {

            bool canAddField = true;
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Tag.ToString() == itemName)
                {
                    canAddField = false;
                }
            }
            if (canAddField)
            {
                var selectedFieldItem = new ListViewItem { Text = itemDisplayName, Tag = itemName };
                selectedFieldItem.SubItems.Add(itemName);
                selectedFieldItem.SubItems.Add(type);
                _listView.Items.Add(selectedFieldItem);
            }
        }


        void AddItemTolistView(ListView _listView, ListViewItem _ListViewItem)
        {


           if(! _listView.Items.Contains(_ListViewItem))
            {
                _listView.Items.Add(_ListViewItem);
            }
        }
        ListViewItem RemoveItemlistView(ListView _listView, string attributeLogicalName)
        {
            ListViewItem removedItem = null;
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Tag.ToString() == attributeLogicalName)
                {
                    removedItem = item;
                    _listView.Items.Remove(item);
                }
            }
            return removedItem;
        }

        private void tabControlResult_ControlAdded(object sender, ControlEventArgs e)
        {
            //if (e.Control.GetType() == typeof(TabPage))
            //{
            //    var ff = (TabPage)e.Control;
            //}
        }

        private void listViewSelectedFields_ControlAdded(object sender, ControlEventArgs e)
        {
            EnableVisableListViewSelectedFields();
        }
        void EnableVisableListViewSelectedFields()
        {
            bool isVisable = false;

            if (listViewSelectedFields.Items.Count > 0)
            {
                isVisable = true;
            }

            listViewSelectedFields.Visible = isVisable;
            listViewSelectedFields.Enabled = isVisable;
            tsbGenerate.Enabled = isVisable;
        }

        private void listViewSelectedFields_DragDrop(object sender, DragEventArgs e)
        {
            EnableVisableListViewSelectedFields();
        }
    }
}