using ITLec.ChartGuy.PowerQueryBuilder.Helpers;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder.FetchXml
{
  public  class FetchXmlQuery
    {

        List<PowerQueryAttribute> powerQueryAttributeList = new List<PowerQueryAttribute>();
        EntityMetadata currentEntityMetadataWithItems = null;
        string filterXml = "";
     public EntityMetadata CurrentEntityMetadataWithItems
        {
            get
            {
                return currentEntityMetadataWithItems;
            }
        }
      public  List<PowerQueryAttribute> PowerQueryAttributeList
        {
            get
            {
                return powerQueryAttributeList;
            }
        }

        public bool HasRecordURL
        {
            get;
            set;

        }


        public string FilterXml
        {
            get
            {
                return filterXml;
            }
            set
            {
                filterXml = value;
            }
        }
        public FetchXmlQuery(EntityMetadata _currentEntityMetadataWithItems,List<PowerQueryAttribute> _powerQueryAttributeList)
        {
            currentEntityMetadataWithItems = _currentEntityMetadataWithItems;
            powerQueryAttributeList = _powerQueryAttributeList;
        }

        public string Validate()
        {
            string retVal = "";

            List<PowerQueryAttribute> tmpList = new List<PowerQueryAttribute>();
            //1- Add All parent Attributes first
            foreach (var att in PowerQueryAttributeList)
            {
                if (att.ParentPowerQueryAttribute != null)
                {
                    var countParentTmp = tmpList.Where(e => ((e.Name == att.ParentPowerQueryAttribute.Name) || e.DisplayName == att.ParentPowerQueryAttribute.DisplayName)).ToList().Count;

                    if (countParentTmp == 0)
                    {

                        var parent= PowerQueryAttributeList.Where(e => ((e.Name == att.ParentPowerQueryAttribute.Name) )).FirstOrDefault();

                        //if (countParentTmp > 0)
                        //{
                        //    att.ParentPowerQueryAttribute.FetchXmlAttributeDetail.IsVisable = false;
                        //}

                        if (parent != null)
                        {
                            tmpList.Add(parent);
                        }
                        else
                        {

                            tmpList.Add(att.ParentPowerQueryAttribute);
                        }
                    }
                }

                //&&e.Type != "Lookup"
                var countAtt = tmpList.Where(e => ((e.Name == att.Name) || e.DisplayName == att.DisplayName)).ToList().Count;

                if (countAtt == 0)
                {
                    tmpList.Add(att);
                }

                else
                {
                  var attInParent=  PowerQueryAttributeList.Where(e => e.ParentPowerQueryAttribute != null && e.ParentPowerQueryAttribute.Name == att.Name).FirstOrDefault();
                    if (attInParent == null)
                    {
                        var _countNames = tmpList.Where(e => (e.Name == att.Name)).FirstOrDefault();
                        if (_countNames != null)
                        {
                            retVal = $"The attribute ({_countNames.Name}) is existed multiple times, Please remove it first" + Environment.NewLine + retVal;
                        }
                        var _countDisplayNames = tmpList.Where(e => (e.DisplayName == att.DisplayName)).FirstOrDefault();
                        if (_countDisplayNames != null)
                        {
                            retVal = $"Please change the Display Name ({_countDisplayNames.DisplayName}), for this attribute ({_countDisplayNames.Name})" + Environment.NewLine + retVal;
                        }
                    }
                }

            }


            powerQueryAttributeList= tmpList;
            return retVal;
        }

        public string FetchXmlQueryString
        {
            get
            {
                string finalMainQuery = "";



                List<string> picklistNames = new List<string>();
                string attributeNameStep1 = "";
                string expandedColumns = "";
                string renameColumns = "";
                string transformColumnTypes = "";
                string emptyList = "";
                foreach (PowerQueryAttribute currectPowerQuertAttribute in powerQueryAttributeList)
                {
                    //  var currectPowerQuertAttribute = (PowerQueryAttribute)listItem.Tag;
                    if (!(currectPowerQuertAttribute).Name.StartsWith("_") && !(currectPowerQuertAttribute).Name.Contains("."))
                    {
                        attributeNameStep1 = attributeNameStep1 + Environment.NewLine + string.Format(@"					<attribute name=""""{0}""""/>", currectPowerQuertAttribute.Name);
                    }

                    if (!currectPowerQuertAttribute.FetchXmlAttributeDetail.IsVisable.HasValue || currectPowerQuertAttribute.FetchXmlAttributeDetail.IsVisable == true)
                    {
                        if (expandedColumns == "")
                        {
                            expandedColumns = string.Format(@"                                            ""{0}""", currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIExpandRecordColumnValue);
                        }
                        else
                        {
                            expandedColumns = $@"{expandedColumns},{Environment.NewLine}                                            ""{currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIExpandRecordColumnValue}""";
                        }



                        if (renameColumns == "")
                        {
                            renameColumns = $@"                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIRenameColumnsValue}";
                            transformColumnTypes = $@"                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBITransformColumnTypeValue}";
                            emptyList = $@"                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIEmptyListValue}";
                        }
                        else
                        {
                            renameColumns = $@"{renameColumns},{Environment.NewLine}                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIRenameColumnsValue}";
                            transformColumnTypes = $@"{transformColumnTypes},{Environment.NewLine}                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBITransformColumnTypeValue}";
                            emptyList = $@"{emptyList},{Environment.NewLine}                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIEmptyListValue}";
                        }
                    }

                }


                attributeNameStep1 = (powerQueryAttributeList.Count > 25) ? "                                   <all-attributes />" : attributeNameStep1;

                filterXml = filterXml.Replace(@"""", @"""""");






                finalMainQuery = $@"let
    GetResults = (z as text, x as number) =>
        let
            S = Json.Document(Web.Contents(ServiceRootURL & ""/{currentEntityMetadataWithItems.LogicalCollectionName}"", [Headers=[Prefer=""odata.include-annotations=*""],Query=[fetchXml=""
			<fetch page="""""" & Text.From(x) & """""" paging-cookie="""""" & z & """""">
				<entity name=""""{currentEntityMetadataWithItems.LogicalName}"""">{attributeNameStep1}
                    {filterXml}
				</entity>
			</fetch>""]])),
            P = try Xml.Document(S[#""@Microsoft.Dynamics.CRM.fetchxmlpagingcookie""]) otherwise null,
            R = if P <> null 
                then List.Combine({{S[value],@GetResults(Text.Replace(Text.Replace(Text.Replace(Uri.Parts(""http://a.b?d="" & Uri.Parts(""http://a.b?d="" & P{{0}}[Attributes]{{1}}[Value])[Query][d])[Query][d], "">"", ""&gt;""), ""<"", ""&lt;""), """""""", ""&quot;""), x + 1)}})
                else S[value]
        in
            R,
    ResultsList = GetResults("""", 1),
    ResultsTable = if List.IsEmpty(ResultsList) 
                    then #table(
                                    type table[ {emptyList}  ],{{}})
                    else #""Converted to Table"",
	#""Converted to Table"" = Table.FromList(ResultsList, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
#""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", 
                                        {{
{expandedColumns}
                                        }}, 

                                        {{
{expandedColumns}
                                        }}),
    #""Renamed Columns"" = Table.RenameColumns(#""Expanded Column1"",
                                        {{
{renameColumns}
                                        }}),
    #""Changed Type"" = Table.TransformColumnTypes(#""Renamed Columns"",
{{
{transformColumnTypes}
}})
";

                finalMainQuery = finalMainQuery +( HasRecordURL ? $@",   
#""Added Link"" = Table.AddColumn(#""Changed Type"", ""Link"", each Dyn365CEBaseURL & ""/main.aspx?etn={currentEntityMetadataWithItems.LogicalName}&pagetype=entityrecord&id=%7b""& [{currentEntityMetadataWithItems.LogicalName}id]&""%7d"")" : 
@"");

                finalMainQuery = finalMainQuery + $@",
#""Result"" = if List.IsEmpty(ResultsList) 
                then ResultsTable
                    else #""{(HasRecordURL? "Added Link":"Changed Type")}""
in
    #""Result""";

                return finalMainQuery;
            }
        }
        public string FetchXmlQueryString_______
        {
            get
            {
                string finalMainQuery = "";



                List<string> picklistNames = new List<string>();

                string step1 = @"let
    GetResults = (z as text, x as number) =>
        let
            S = Json.Document(Web.Contents(ServiceRootURL & ""/{0}"", [Headers=[Prefer=""odata.include-annotations=*""],Query=[fetchXml=""
			<fetch page="""""" & Text.From(x) & """""" paging-cookie="""""" & z & """""">
				<entity name=""""{1}"""">{2}
                    {3}
				</entity>
			</fetch>""]])),
            P = try Xml.Document(S[#""@Microsoft.Dynamics.CRM.fetchxmlpagingcookie""]) otherwise null,
            R = if P <> null 
                then List.Combine({{S[value],@GetResults(Text.Replace(Text.Replace(Text.Replace(Uri.Parts(""http://a.b?d="" & Uri.Parts(""http://a.b?d="" & P{{0}}[Attributes]{{1}}[Value])[Query][d])[Query][d], "">"", ""&gt;""), ""<"", ""&lt;""), """""""", ""&quot;""), x + 1)}})
                else S[value]
        in
            R,
    ResultsList = GetResults("""", 1),
	#""Converted to Table"" = Table.FromList(ResultsList, Splitter.SplitByNothing(), null, null, ExtraValues.Error),

";

                string attributeNameStep1 = "";


                string expandedColumns = "";
                string renameColumns = "";
                string transformColumnTypes = "";
                foreach (PowerQueryAttribute currectPowerQuertAttribute in powerQueryAttributeList)
                {
                    //  var currectPowerQuertAttribute = (PowerQueryAttribute)listItem.Tag;
                    if (!(currectPowerQuertAttribute).Name.StartsWith("_") && !(currectPowerQuertAttribute).Name.Contains("."))
                    {
                        attributeNameStep1 = attributeNameStep1 + Environment.NewLine + string.Format(@"					<attribute name=""""{0}""""/>", currectPowerQuertAttribute.Name);
                    }


                    if (!currectPowerQuertAttribute.FetchXmlAttributeDetail.IsVisable.HasValue || currectPowerQuertAttribute.FetchXmlAttributeDetail.IsVisable == true)
                    {
                        if (expandedColumns == "")
                        {
                            expandedColumns = string.Format(@"                                            ""{0}""", currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIExpandRecordColumnValue);
                        }
                        else
                        {
                            expandedColumns = $@"{expandedColumns},{Environment.NewLine}                                            ""{currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIExpandRecordColumnValue}""";
                        }



                        if (renameColumns == "")
                        {
                            renameColumns = $@"                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIRenameColumnsValue}";
                            transformColumnTypes = $@"                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBITransformColumnTypeValue}";
                        }
                        else
                        {
                            renameColumns = $@"{renameColumns},{Environment.NewLine}                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBIRenameColumnsValue}";
                            transformColumnTypes = $@"{transformColumnTypes},{Environment.NewLine}                                            {currectPowerQuertAttribute.FetchXmlAttributeDetail.PowerBITransformColumnTypeValue}";
                        }
                    }

                }


                attributeNameStep1 = (powerQueryAttributeList.Count > 25) ? "                                   <all-attributes />" : attributeNameStep1;

                filterXml = filterXml.Replace(@"""", @"""""");
                step1 = string.Format(step1, currentEntityMetadataWithItems.LogicalCollectionName, currentEntityMetadataWithItems.LogicalName, attributeNameStep1, filterXml);



                string step2ExpandRecordColumn = $@"#""Expanded Column1"" = Table.ExpandRecordColumn(#""Converted to Table"", ""Column1"", 
                                        {{
{expandedColumns}
                                        }}, 

                                        {{
{expandedColumns}
                                        }}),";

                string step3RenamedColumns = $@"    #""Renamed Columns"" = Table.RenameColumns(#""Expanded Column1"",
                                        {{
{renameColumns}
                                        }}),

";

                string lastStep = $@"    #""Changed Type"" = Table.TransformColumnTypes(#""Renamed Columns"",
{{
{transformColumnTypes}
}})
in
    #""Changed Type""";
                finalMainQuery = step1 + "\n" + step2ExpandRecordColumn + "\n" + step3RenamedColumns + lastStep;

                return finalMainQuery;
            }
        }
    }
}
