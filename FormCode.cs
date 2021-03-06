using Microsoft.Office.InfoPath;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.SharePoint.Client;

namespace apfm
{
    public partial class FormCode
    {
        // 启用浏览器功能的表单不支持成员变量。
        // 请使用代码从 FormState 词典
        // 写入和读取这些值，如下所示:
        //

        private object _arrayListID
        {
            get
            {
                return FormState["_arrayListID"];
            }
            set
            {
                FormState["_arrayListID"] = value;
            }
        }

        // 注意: 以下是 Microsoft InfoPath 所需的过程。
        // 可以使用 Microsoft InfoPath 对其进行修改。

        public void InternalStartup()
        {
            EventManager.FormEvents.Loading += new LoadingEventHandler(FormEvents_Loading);
            ((ButtonEvent)EventManager.ControlEvents["Button_Select_MP"]).Clicked += new ClickedEventHandler(Button_Select_MP_Clicked);
            ((ButtonEvent)EventManager.ControlEvents["Button_OK_MP"]).Clicked += new ClickedEventHandler(Button_OK_MP_Clicked);
            ((ButtonEvent)EventManager.ControlEvents["Button_Select_AD"]).Clicked += new ClickedEventHandler(Button_Select_AD_Clicked);
            ((ButtonEvent)EventManager.ControlEvents["Button_OK_AD"]).Clicked += new ClickedEventHandler(Button_OK_AD_Clicked);
            ((ButtonEvent)EventManager.ControlEvents["Button_GetLastFormInfo"]).Clicked += new ClickedEventHandler(Button_GetLastFormInfo_Clicked);
        }

        public void FormEvents_Loading(object sender, LoadingEventArgs e)
        {
            // 在此处编写代码。
            XPathNavigator root = MainDataSource.CreateNavigator();
            string code = root.SelectSingleNode("/my:myFields/my:code", NamespaceManager).Value;
            if (code == "")
            {
                DeleteAllRows("/my:myFields/my:MediaPlanning/my:MP/my:MPs");
                DeleteAllRows("/my:myFields/my:ArtDisplay/my:AD/my:ADs");

                //DeleteAllRows("/my:myFields/my:SPContent/my:SPC/my:SPCs");
            }
        }

        public void Button_Select_MP_Clicked(object sender, ClickedEventArgs e)
        {
            // 在此处编写代码。
            string node = "/my:myFields/my:MediaPlanning/my:MP/my:MPs";

            //获取已选记录
            ArrayList ArrayListID = GetListID(node, "MP");
            _arrayListID = ArrayListID;

            //获取媒体项目
            ListItemCollection collListItems = GetListItems(node, "MP", "MediaResources");

            foreach (ListItem theItem in collListItems)
            {
                string MPListID = Convert.ToString(theItem["ID"]);
                //string MediaType = Convert.ToString(theItem["MediaType"]);
                string MediaProject = Convert.ToString(theItem["MediaProject"]);
                string MPSpecification = Convert.ToString(theItem["Specification"]);
                string MPPrice = Convert.ToString(theItem["Price"]);
                string MPResourcesNumber = Convert.ToString(theItem["ResourcesNumber"]);
                string MediaAgency = Convert.ToString(theItem["MediaAgency"]);
                string MPContractNumber = Convert.ToString(theItem["ContractNumber"]);
                string MPContractLife = Convert.ToString(theItem["ContractLife"]);

                string[] fields = new string[] { "MPCheck", "MediaType", "MediaProject", "MPBeginDate", "MPEndDate", "MPDays", "MPContent", "MPSpecification", "MPPrice", "MPResourcesNumber", "MediaAgency", "MPContractNumber", "MPContractLife", "MPSubtotal", "MPListID", "MPIsManual", "MPIsHide" };
                string[] values = new string[] { IsExist(MPListID, ArrayListID) == true ? "true" : "false", "", MediaProject, "", "", "0", "", MPSpecification, MPPrice, MPResourcesNumber, MediaAgency, MPContractNumber, MPContractLife, "0", MPListID, "0", "0" };
                AddRow("/my:myFields/my:MediaPlanning/my:MP", "my:MPs", fields, values);
            }
        }

        public void Button_OK_MP_Clicked(object sender, ClickedEventArgs e)
        {
            // 在此处编写代码。
            string node = "/my:myFields/my:MediaPlanning/my:MP/my:MPs";
            DoConfirm(node, "MP");
        }

        public void Button_Select_AD_Clicked(object sender, ClickedEventArgs e)
        {
            // 在此处编写代码。
            string node = "/my:myFields/my:ArtDisplay/my:AD/my:ADs";

            //获取已选记录
            ArrayList ArrayListID = GetListID(node, "AD");
            _arrayListID = ArrayListID;

            //获取美陈项目
            ListItemCollection collListItems = GetListItems(node, "AD", "ArtAndDisplayResources");
            foreach (ListItem theItem in collListItems)
            {
                string ADListID = Convert.ToString(theItem["ID"]);
                string ADProject = Convert.ToString(theItem["Project"]);
                string ADSpecification = Convert.ToString(theItem["Specification"]);
                string ADPrice = Convert.ToString(theItem["Price"]);
                string ADCompany = Convert.ToString(theItem["Company"]);
                string ADContractNumber = Convert.ToString(theItem["ContractNumber"]);
                string ADContractLife = Convert.ToString(theItem["ContractLife"]);

                string[] fields = new string[] { "ADCheck", "ADProject", "ADBeginDate", "ADEndDate", "ADDays", "ADContent", "ADRequirement", "ADSpecification", "ADPrice", "ADAmount", "ADCompany", "ADContractNumber", "ADContractLife", "ADSubtotal", "ADListID", "ADIsManual", "ADIsHide" };
                string[] values = new string[] { IsExist(ADListID, ArrayListID) == true ? "true" : "false", ADProject, "", "", "0", "", "", ADSpecification, ADPrice, "0", ADCompany, ADContractNumber, ADContractLife, "0", ADListID, "0", "0" };
                AddRow("/my:myFields/my:ArtDisplay/my:AD", "my:ADs", fields, values);
            }
        }

        public void Button_OK_AD_Clicked(object sender, ClickedEventArgs e)
        {
            // 在此处编写代码。
            string node = "/my:myFields/my:ArtDisplay/my:AD/my:ADs";
            DoConfirm(node, "AD");
        }

        public void Button_GetLastFormInfo_Clicked(object sender, ClickedEventArgs e)
        {
            // 在此处编写代码。
            XPathNavigator root = MainDataSource.CreateNavigator();
            XPathNavigator listID = root.SelectSingleNode("/my:myFields/my:BasicInfo/my:WF/my:WFListID", NamespaceManager);
            if (listID != null)
                DoSearch("SM百货企划活动案审批表", listID.Value);
        }

        public bool IsExist(string item, object list)
        {
            ArrayList arrayList = (ArrayList)list;
            bool ret = false;
            for (int i = 0; i < arrayList.Count; i++)
            {
                if (arrayList[i].ToString() != item)
                    continue;

                return true;
            }
            return ret;
        }

        public void DeleteAllRows(string node)
        {
            XPathNavigator domNav = MainDataSource.CreateNavigator();
            XPathNodeIterator rows = domNav.Select(node, NamespaceManager);

            int count = rows.Count;
            if (count > 0)
                for (int i = count; i >= 1; i--)
                {
                    XPathNavigator itemNav = domNav.SelectSingleNode(node + "[" + i.ToString() + "]", NamespaceManager);

                    // Delete the row  
                    if (itemNav != null)
                        itemNav.DeleteSelf();
                }
        }

        public void AddRow(string parentNode, string currentNode, string[] fields, string[] values)
        {
            XPathNavigator domNav = MainDataSource.CreateNavigator();

            XmlDocument doc = new XmlDocument();
            XmlNode group = doc.CreateElement(currentNode, NamespaceManager.LookupNamespace("my"));

            XmlNode field;
            XmlNode node;

            for (int i = 0; i < fields.Length; i++)
            {
                field = doc.CreateElement(fields[i], NamespaceManager.LookupNamespace("my"));
                node = group.AppendChild(field);
                if (values[i] != null)
                    node.InnerText = values[i];
            }

            doc.AppendChild(group);

            domNav.SelectSingleNode(parentNode, NamespaceManager).AppendChild(doc.DocumentElement.CreateNavigator());
        }

        public int DeleteNodes(string node, int rowIndex, string ListID, string Field, int Function)
        {
            //Function:
            //1、删除除隐藏行之外的所有此ListID的行（隐藏行条件：IsManual == "1" || IsHide == "1"）
            //2、删除所有此ListID的行

            XPathNavigator root = MainDataSource.CreateNavigator();
            XPathNodeIterator rows = root.Select(node, NamespaceManager);

            int count = rows.Count;
            if (count > 0)
            {
                for (int i = 1; i <= count; i++)
                {
                    XPathNavigator itemNav = root.SelectSingleNode(node + "[" + i.ToString() + "]", NamespaceManager);
                    if (itemNav != null)
                    {
                        string listID = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "ListID", NamespaceManager).Value;

                        if (Function == 1)
                        {
                            string Check = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "Check", NamespaceManager).Value;
                            string IsManual = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "IsManual", NamespaceManager).Value;
                            string IsHide = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "IsHide", NamespaceManager).Value;
                            string BeginDate = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "BeginDate", NamespaceManager).Value;
                            string EndDate = root.SelectSingleNode(node + "[" + i.ToString() + "]/my:" + Field + "EndDate", NamespaceManager).Value;

                            if (IsManual == "1" || IsHide == "1")
                                continue;
                        }

                        if (listID == ListID)
                        {
                            itemNav.DeleteSelf();
                            i--;
                            count--;
                            rowIndex--;
                        }
                    }
                }
            }

            return rowIndex;
        }

        public ArrayList GetListID(string node, string field)
        {
            XPathNavigator root = MainDataSource.CreateNavigator();
            ArrayList ArrayListID = new ArrayList();
            XPathNodeIterator selectedRows = root.Select(node, NamespaceManager);
            int rowIndex = 0;
            while (selectedRows.MoveNext())
            {
                rowIndex++;
                XPathNavigator rowItem = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]", NamespaceManager);

                if (rowItem != null)
                {
                    string listID = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "ListID", NamespaceManager).Value;
                    if (listID != "")
                    {
                        ArrayListID.Add(listID);
                        root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "IsHide", NamespaceManager).SetValue("1");
                    }
                }
            }
            return ArrayListID;
        }

        public void DoConfirm(string node, string field)
        {
            XPathNavigator root = MainDataSource.CreateNavigator();
            XPathNodeIterator selectedRows = root.Select(node, NamespaceManager);
            int rowIndex = 0, count = selectedRows.Count;

            for (int i = 0; i < count; i++)
            {
                rowIndex++;
                XPathNavigator rowItem = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]", NamespaceManager);

                if (rowItem != null)
                {
                    string ListID = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "ListID", NamespaceManager).Value;
                    string Check = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "Check", NamespaceManager).Value;
                    string IsManual = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "IsManual", NamespaceManager).Value;
                    string IsHide = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "IsHide", NamespaceManager).Value;
                    string BeginDate = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "BeginDate", NamespaceManager).Value;
                    string EndDate = root.SelectSingleNode(node + "[" + rowIndex.ToString() + "]/my:" + field + "EndDate", NamespaceManager).Value;

                    if (IsManual == "1" || IsHide == "1")
                        continue;

                    if (Check == "true")
                    {
                        if (IsExist(ListID, _arrayListID))
                        {
                            rowIndex = DeleteNodes(node, rowIndex, ListID, field, 1);
                        }
                    }
                    else
                    {
                        rowIndex = DeleteNodes(node, rowIndex, ListID, field, 2);
                    }
                }
            }
        }

        public ListItemCollection GetListItems(string node, string field, string listName)
        {
            XPathNavigator root = MainDataSource.CreateNavigator();
            string applyStore = root.SelectSingleNode("/my:myFields/my:UserInfo/my:ApplyStore", NamespaceManager).Value;

            string url = "http://chinastore.sm.ph/forms/storemarketing";
            ClientContext context = new ClientContext(url);
            context.Credentials = new System.Net.NetworkCredential("svc_cnprdspadmin", "33_JabbAr33$", "smprime");
            List byTitle = context.Web.Lists.GetByTitle(listName);
            context.Load(byTitle);
            context.ExecuteQuery();
            CamlQuery query = new CamlQuery();

            query.ViewXml = @"
            <View><Query>
              <Where>
                <Eq>
                    <FieldRef Name='Title' />
                    <Value Type='Text'>" + applyStore + @"</Value>
                </Eq>
              </Where>
              <OrderBy>
                    <FieldRef Name='ID' />
              </OrderBy>
            </Query></View>";

            ListItemCollection collListItems = byTitle.GetItems(query);
            context.Load(collListItems);
            context.ExecuteQuery();

            return collListItems;
        }

        public ListItemCollection GetListItems(ClientContext context, string listName, string listID)
        {
            List list = context.Web.Lists.GetByTitle(listName);
            CamlQuery query = new CamlQuery();

            query.ViewXml = @"
            <View><Query>
              <Where>
                <Eq>
                    <FieldRef Name='ID' />
                    <Value Type='Counter'>" + listID + @"</Value>
                </Eq>
              </Where>
            </Query></View>";

            ListItemCollection collListItems = list.GetItems(query);
            context.Load(collListItems,
                items => items.Include(
                    item => item.ContentType,
                    item => item.DisplayName,
                    item => item.EffectiveBasePermissions,
                    item => item.EffectiveBasePermissionsForUI,
                    item => item.File,
                    item => item.FileSystemObjectType,
                    item => item.Id));
            context.ExecuteQuery();

            return collListItems;
        }

        public XPathNavigator GetFormNav(ClientContext context, ListItem theItem)
        {
            XPathDocument ipForm = null;
            Microsoft.SharePoint.Client.File file = theItem.File;
            ClientResult<Stream> stream = file.OpenBinaryStream();

            //Load the Stream data for the file
            context.Load(file);
            context.ExecuteQuery();

            if (stream != null)
            {
                Stream ms = stream.Value;
                ipForm = new XPathDocument(ms);
                ms.Close();
            }

            XPathNavigator ipFormNav = ipForm.CreateNavigator();
            ipFormNav.MoveToFollowing(XPathNodeType.Element);

            return ipFormNav;
        }

        public XmlNamespaceManager GetNameSpaceManager(XPathNavigator ipFormNav)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
            foreach (KeyValuePair<string, string> ns in ipFormNav.GetNamespacesInScope(XmlNamespaceScope.All))
            {
                if (ns.Key == String.Empty)
                {
                    nsManager.AddNamespace("def", ns.Value);
                }
                else
                {
                    nsManager.AddNamespace(ns.Key, ns.Value);
                }
            }

            return nsManager;
        }

        public void DoSearch(string listName, string listID)
        {
            XPathNavigator root = MainDataSource.CreateNavigator();
            string url = "http://chinastore.sm.ph/forms/storemarketing";
            ClientContext context = new ClientContext(url);
            context.Credentials = new System.Net.NetworkCredential("svc_cnprdspadmin", "33_JabbAr33$", "smprime");
            ListItemCollection collListItems = GetListItems(context, listName, listID);
            if (collListItems.Count > 0)
            {
                ListItem theItem = collListItems[0];
                XPathNavigator ipFormNav = GetFormNav(context, theItem);
                XmlNamespaceManager nsManager = GetNameSpaceManager(ipFormNav);

                //BasicInfo
                SetFieldValue("/my:myFields/my:BasicInfo/my:Subject", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:ActivityContent", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:LastYearBeginDate", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:LastYearEndDate", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:LastYearPerformance", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:ThisYearBeginDate", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:ThisYearEndDate", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:ThisYearPerformance", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:BudgetPerformance", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:BasicInfo/my:SpecialRemark", ipFormNav, nsManager);

                string[] fields = { "SPNo", "SPTheme", "SPMode", "SPBeginDate", "SPEndDate", "SPDays", "SPEstimatedPerformance", "SPBracket", "SPConversionRate", "SPBusinessType", "SPDetails", "SPVIP", "SPEI" };
                string[] values = null;
                string node = "", subNode = "";
                int rowIndex = 0, subRowIndex = 0;

                //SPContent
                #region
                XPathNodeIterator rows = ipFormNav.Select("/my:myFields/my:SPContent/my:SPC/my:SPCs", nsManager);
                if (rows.Count > 0)
                {
                    DeleteAllRows("/my:myFields/my:SPContent/my:SPC/my:SPCs");
                    while (rows.MoveNext())
                    {
                        rowIndex++;
                        string SPTheme = rows.Current.SelectSingleNode("my:SPTheme", nsManager).Value;
                        string SPMode = rows.Current.SelectSingleNode("my:SPMode", nsManager).Value;
                        string SPBeginDate = rows.Current.SelectSingleNode("my:SPBeginDate", nsManager).Value;
                        string SPEndDate = rows.Current.SelectSingleNode("my:SPEndDate", nsManager).Value;
                        string SPDays = rows.Current.SelectSingleNode("my:SPDays", nsManager).Value;
                        string SPEstimatedPerformance = rows.Current.SelectSingleNode("my:SPEstimatedPerformance", nsManager).Value;
                        string SPBracket = rows.Current.SelectSingleNode("my:SPBracket", nsManager).Value;
                        string SPConversionRate = rows.Current.SelectSingleNode("my:SPConversionRate", nsManager).Value;
                        string SPBusinessType = rows.Current.SelectSingleNode("my:SPBusinessType", nsManager).Value;
                        string SPDetails = rows.Current.SelectSingleNode("my:SPDetails", nsManager).Value;
                        XPathNavigator spvip = rows.Current.SelectSingleNode("my:SPVIP", nsManager);
                        string SPVIP = spvip == null ? "活动费用" : spvip.Value;

                        values = new string[] { "", SPTheme, SPMode, SPBeginDate, SPEndDate, SPDays == "" ? "0" : SPDays, SPEstimatedPerformance == "" ? "0" : SPEstimatedPerformance, SPBracket == "" ? "0" : SPBracket, SPConversionRate == "" ? "0" : SPConversionRate, SPBusinessType, SPDetails, SPVIP, null };
                        AddRow("/my:myFields/my:SPContent/my:SPC", "my:SPCs", fields, values);

                        node = "/my:myFields/my:SPContent/my:SPC/my:SPCs[" + rowIndex.ToString() + "]/my:SPEIs";
                        XPathNodeIterator subRows = ipFormNav.Select(node, nsManager);
                        if (subRows.Count == 0)
                        {
                            DeleteAllRows(node);
                            node = "/my:myFields/my:SPContent/my:SPC/my:SPCs[" + rowIndex.ToString() + "]/my:SPEI/my:SPEIs";
                            subRows = ipFormNav.Select(node, nsManager);
                        }

                        if (subRows.Count > 0)
                        {
                            DeleteAllRows(node);
                            string[] subFields = { "SPExpenseItem", "Gift", "IsLimited", "GiftAmount", "GiftPrice", "ValueAddedGiftPrice", "SpikeRate", "SpikeAmount", "SPSubtotal", "SPRemark" };
                            subRowIndex = 0;
                            while (subRows.MoveNext())
                            {
                                subRowIndex++;
                                subNode = node + "[" + subRowIndex.ToString() + "]/";
                                string SPExpenseItem = ipFormNav.SelectSingleNode(subNode + "my:SPExpenseItem", nsManager).Value;
                                string Gift = ipFormNav.SelectSingleNode(subNode + "my:Gift", nsManager).Value;
                                string IsLimited = ipFormNav.SelectSingleNode(subNode + "my:IsLimited", nsManager).Value;
                                string GiftAmount = ipFormNav.SelectSingleNode(subNode + "my:GiftAmount", nsManager).Value;
                                string GiftPrice = ipFormNav.SelectSingleNode(subNode + "my:GiftPrice", nsManager).Value;
                                string ValueAddedGiftPrice = ipFormNav.SelectSingleNode(subNode + "my:ValueAddedGiftPrice", nsManager).Value;
                                string SpikeRate = ipFormNav.SelectSingleNode(subNode + "my:SpikeRate", nsManager).Value;
                                string SpikeAmount = ipFormNav.SelectSingleNode(subNode + "my:SpikeAmount", nsManager).Value;
                                string SPSubtotal = ipFormNav.SelectSingleNode(subNode + "my:SPSubtotal", nsManager).Value;
                                string SPRemark = ipFormNav.SelectSingleNode(subNode + "my:SPRemark", nsManager).Value;

                                values = new string[] { SPExpenseItem, Gift, IsLimited, GiftAmount == "" ? "0" : GiftAmount, GiftPrice == "" ? "0" : GiftPrice, ValueAddedGiftPrice == "" ? "0" : ValueAddedGiftPrice, SpikeRate == "" ? "0" : SpikeRate, SpikeAmount == "" ? "0" : SpikeAmount, SPSubtotal == "" ? "0" : SPSubtotal, SPRemark };
                                AddRow("/my:myFields/my:SPContent/my:SPC/my:SPCs[" + rowIndex.ToString() + "]/my:SPEI", "my:SPEIs", subFields, values);
                            }
                        }
                    }
                }
                #endregion

                //PRContent
                #region
                rows = ipFormNav.Select("/my:myFields/my:PRContent/my:PRC/my:PRCs", nsManager);
                if (rows.Count > 0)
                {
                    rowIndex = 0;
                    fields = new string[] { "PRNo", "PRTheme", "PRMode", "PRBeginDate", "PREndDate", "PRDays", "ActivityLocation", "Vendor", "Participants", "EstimatedFootfall", "PRDetails", "VendorContent", "PRAtta", "PREI" };
                    DeleteAllRows("/my:myFields/my:PRContent/my:PRC/my:PRCs");
                    while (rows.MoveNext())
                    {
                        rowIndex++;
                        string PRTheme = rows.Current.SelectSingleNode("my:PRTheme", nsManager).Value;
                        string PRMode = rows.Current.SelectSingleNode("my:PRMode", nsManager).Value;
                        string PRBeginDate = rows.Current.SelectSingleNode("my:PRBeginDate", nsManager).Value;
                        string PREndDate = rows.Current.SelectSingleNode("my:PREndDate", nsManager).Value;
                        string PRDays = rows.Current.SelectSingleNode("my:PRDays", nsManager).Value;
                        string ActivityLocation = rows.Current.SelectSingleNode("my:ActivityLocation", nsManager).Value;
                        string Vendor = rows.Current.SelectSingleNode("my:Vendor", nsManager).Value;
                        string Participants = rows.Current.SelectSingleNode("my:Participants", nsManager).Value;
                        string EstimatedFootfall = rows.Current.SelectSingleNode("my:EstimatedFootfall", nsManager).Value;
                        string PRDetails = rows.Current.SelectSingleNode("my:PRDetails", nsManager).Value;
                        string VendorContent = rows.Current.SelectSingleNode("my:VendorContent", nsManager).Value;

                        values = new string[] { "", PRTheme, PRMode, PRBeginDate, PREndDate, PRDays == "" ? "0" : PRDays, ActivityLocation, Vendor, Participants == "" ? "0" : Participants, EstimatedFootfall == "" ? "0" : EstimatedFootfall, PRDetails, VendorContent, null, null };
                        AddRow("/my:myFields/my:PRContent/my:PRC", "my:PRCs", fields, values);

                        node = "/my:myFields/my:PRContent/my:PRC/my:PRCs[" + rowIndex.ToString() + "]/my:PREIs";
                        XPathNodeIterator subRows = ipFormNav.Select(node, nsManager);
                        if (subRows.Count == 0)
                        {
                            DeleteAllRows(node);
                            node = "/my:myFields/my:PRContent/my:PRC/my:PRCs[" + rowIndex.ToString() + "]/my:PREI/my:PREIs";
                            subRows = ipFormNav.Select(node, nsManager);
                        }

                        if (subRows.Count > 0)
                        {
                            DeleteAllRows(node);
                            string[] subFields = { "PRExpenseItem", "PRAmount", "PRPrice", "PRSubtotal" };
                            subRowIndex = 0;
                            while (subRows.MoveNext())
                            {
                                subRowIndex++;
                                subNode = node + "[" + subRowIndex.ToString() + "]/";
                                string PRExpenseItem = ipFormNav.SelectSingleNode(subNode + "my:PRExpenseItem", nsManager).Value;
                                string PRAmount = ipFormNav.SelectSingleNode(subNode + "my:PRAmount", nsManager).Value;
                                string PRPrice = ipFormNav.SelectSingleNode(subNode + "my:PRPrice", nsManager).Value;
                                string PRSubtotal = ipFormNav.SelectSingleNode(subNode + "my:PRSubtotal", nsManager).Value;

                                values = new string[] { PRExpenseItem, PRAmount == "" ? "0" : PRAmount, PRPrice == "" ? "0" : PRPrice, PRSubtotal == "" ? "0" : PRSubtotal };
                                AddRow("/my:myFields/my:PRContent/my:PRC/my:PRCs[" + rowIndex.ToString() + "]/my:PREI", "my:PREIs", subFields, values);
                            }
                        }
                    }
                }
                #endregion

                //MediaPlanning
                #region
                rows = ipFormNav.Select("/my:myFields/my:MediaPlanning/my:MP/my:MPs", nsManager);
                if (rows.Count > 0)
                {
                    rowIndex = 0;
                    fields = new string[] { "MPCheck", "MediaType", "MediaProject", "MPBeginDate", "MPEndDate", "MPDays", "MPContent", "MPSpecification", "MPPrice", "MPResourcesNumber", "MediaAgency", "MPContractNumber", "MPContractLife", "MPSubtotal", "MPListID", "MPIsManual", "MPIsHide" };
                    DeleteAllRows("/my:myFields/my:MediaPlanning/my:MP/my:MPs");
                    while (rows.MoveNext())
                    {
                        rowIndex++;
                        string MediaType = rows.Current.SelectSingleNode("my:MediaType", nsManager).Value;
                        string MediaProject = rows.Current.SelectSingleNode("my:MediaProject", nsManager).Value;
                        string MPBeginDate = rows.Current.SelectSingleNode("my:MPBeginDate", nsManager).Value;
                        string MPEndDate = rows.Current.SelectSingleNode("my:MPEndDate", nsManager).Value;
                        string MPDays = rows.Current.SelectSingleNode("my:MPDays", nsManager).Value;
                        string MPContent = rows.Current.SelectSingleNode("my:MPContent", nsManager).Value;
                        string MPSpecification = rows.Current.SelectSingleNode("my:MPSpecification", nsManager).Value;
                        string MPPrice = rows.Current.SelectSingleNode("my:MPPrice", nsManager).Value;
                        string MPResourcesNumber = rows.Current.SelectSingleNode("my:MPResourcesNumber", nsManager).Value;
                        string MediaAgency = rows.Current.SelectSingleNode("my:MediaAgency", nsManager).Value;
                        string MPContractNumber = rows.Current.SelectSingleNode("my:MPContractNumber", nsManager).Value;
                        string MPContractLife = rows.Current.SelectSingleNode("my:MPContractLife", nsManager).Value;
                        string MPSubtotal = rows.Current.SelectSingleNode("my:MPSubtotal", nsManager).Value;
                        string MPListID = rows.Current.SelectSingleNode("my:MPListID", nsManager).Value;

                        values = new string[] { "false", MediaType, MediaProject, MPBeginDate, MPEndDate, MPDays == "" ? "0" : MPDays, MPContent, MPSpecification, MPPrice, MPResourcesNumber, MediaAgency, MPContractNumber, MPContractLife, MPSubtotal, MPListID, MPListID == "" ? "1" : "0", "0" };
                        AddRow("/my:myFields/my:MediaPlanning/my:MP", "my:MPs", fields, values);
                    }
                }
                #endregion

                //ArtDisplay
                #region
                rows = ipFormNav.Select("/my:myFields/my:ArtDisplay/my:AD/my:ADs", nsManager);
                if (rows.Count > 0)
                {
                    rowIndex = 0;
                    fields = new string[] { "ADCheck", "ADProject", "ADBeginDate", "ADEndDate", "ADDays", "ADContent", "ADRequirement", "ADSpecification", "ADPrice", "ADAmount", "ADCompany", "ADContractNumber", "ADContractLife", "ADSubtotal", "ADListID", "ADIsManual", "ADIsHide" };
                    DeleteAllRows("/my:myFields/my:ArtDisplay/my:AD/my:ADss");
                    while (rows.MoveNext())
                    {
                        rowIndex++;
                        string ADProject = rows.Current.SelectSingleNode("my:ADProject", nsManager).Value;
                        string ADBeginDate = rows.Current.SelectSingleNode("my:ADBeginDate", nsManager).Value;
                        string ADEndDate = rows.Current.SelectSingleNode("my:ADEndDate", nsManager).Value;
                        string ADDays = rows.Current.SelectSingleNode("my:ADDays", nsManager).Value;
                        string ADContent = rows.Current.SelectSingleNode("my:ADContent", nsManager).Value;
                        string ADRequirement = rows.Current.SelectSingleNode("my:ADRequirement", nsManager).Value;
                        string ADSpecification = rows.Current.SelectSingleNode("my:ADSpecification", nsManager).Value;
                        string ADPrice = rows.Current.SelectSingleNode("my:ADPrice", nsManager).Value;
                        string ADAmount = rows.Current.SelectSingleNode("my:ADAmount", nsManager).Value;
                        string ADCompany = rows.Current.SelectSingleNode("my:ADCompany", nsManager).Value;
                        string ADContractNumber = rows.Current.SelectSingleNode("my:ADContractNumber", nsManager).Value;
                        string ADContractLife = rows.Current.SelectSingleNode("my:ADContractLife", nsManager).Value;
                        string ADSubtotal = rows.Current.SelectSingleNode("my:ADSubtotal", nsManager).Value;
                        string ADListID = rows.Current.SelectSingleNode("my:ADListID", nsManager).Value;

                        values = new string[] { "false", ADProject, ADBeginDate, ADEndDate, ADDays == "" ? "0" : ADDays, ADContent, ADRequirement, ADSpecification, ADPrice == "" ? "0" : ADPrice, ADAmount == "" ? "0" : ADAmount, ADCompany, ADContractNumber, ADContractLife, ADSubtotal, ADListID, ADListID == "" ? "1" : "0", "0" };
                        AddRow("/my:myFields/my:ArtDisplay/my:AD", "my:ADs", fields, values);
                    }
                }
                #endregion

                //Comparison
                SetFieldValue("/my:myFields/my:Comparison/my:AnnualActivityExpense", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:Comparison/my:AnnualVIPActivityExpense", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:Comparison/my:AnnualMediaExpense", ipFormNav, nsManager);
                SetFieldValue("/my:myFields/my:Comparison/my:AnnualADExpense", ipFormNav, nsManager);

                //Notes
                SetFieldValue("/my:myFields/my:Notes/my:Remark", ipFormNav, nsManager);
            }
        }

        public void SetFieldValue(string node, XPathNavigator ipFormNav, XmlNamespaceManager nsManager)
        {
            XPathNavigator root = MainDataSource.CreateNavigator();
            XPathNavigator value = ipFormNav.SelectSingleNode(node, nsManager);
            root.SelectSingleNode(node, NamespaceManager).SetValue(value == null ? "" : value.Value);
        }
    }
}
