using OpenDental.customerUpdates;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormFeatureRequest:FormODBase {
		private ODDataTable table;
		///<summary>Set to true when this window was launched from HQ.  Allows the window to be put into management mode.</summary>
		private bool _isAdminMode;
		///<summary>Used in the JobManager system for attaching features to jobs.</summary>
		public bool IsSelectionMode;
		///<summary>Only for IsSelectionMode, returns the selected num.</summary>
		public long SelectedFeatureNum=0;
		/// <summary>Used to fill grid in FillGrid. Filters operate on this local copy. Fill by calling RefreshRequestTable()</summary>
		private ODDataTable _tableRequests;

		///<summary></summary>
		public FormFeatureRequest()
		{
			components=new System.ComponentModel.Container();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeatureRequest_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode) {
				this.Text="Select a Feature Request";
				butClose.Text="Cancel";
				labelVote.Visible=false;
			}
			else {//normal for customer
				butOK.Visible=false;
				butClose.Text="Close";
				butEdit.Visible=false;
			}
			_tableRequests=new ODDataTable();
			FillGrid(); //Fills column headings on load
			/*
				if(Security.IsAuthorized(Permissions.Setup,true)) {
					butCheck2.Visible=true;
				}
				else {
					textConnectionMessage.Text=Lan.g(this,"Not authorized for")+" "+GroupPermissions.GetDesc(Permissions.Setup);
				}
			*/
			//if(!Synch()){
			//	return;
			//}
			//FillGrid();
		}

		private void butSearch_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			RefreshRequestTable();
			FillGrid();
		}

		///<summary>Makes a web call to WebServiceCustomersUpdate in order to get an updated list of feature requests.</summary>
		private void RefreshRequestTable() {
			//Always clear the table first.
			_tableRequests=new ODDataTable();
			Cursor=Cursors.WaitCursor;
			//Yes, this would be slicker if it were asynchronous, but no time right now.
			#region prepare the xml document to send
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings); 
			xmlWriter.WriteStartElement("FeatureRequestGetList");
			xmlWriter.WriteStartElement("RegistrationKey");
			xmlWriter.WriteString(PrefC.GetString(PrefName.RegistrationKey));
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("SearchString");
			xmlWriter.WriteString(textSearch.Text);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			#endregion prepare the xml document to send
			#region Send the message and get the result
			string result="";
			try {
				result=updateService.FeatureRequestGetList(stringBuilder.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			#endregion  Send the message and get the result
			Cursor=Cursors.Default;
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			#region Process errors
			XmlNode xmlNode=xmlDocument.SelectSingleNode("//Error");
			if(xmlNode!=null) {
				//textConnectionMessage.Text=node.InnerText;
				MessageBox.Show(xmlNode.InnerText,"Error");
				return;
			}
			xmlNode=xmlDocument.SelectSingleNode("//KeyDisabled");
			if(xmlNode==null) {
				//no error, and no disabled message
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				//textConnectionMessage.Text=node.InnerText;
				MessageBox.Show(xmlNode.InnerText);
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				return;
			}
			#endregion Process errors
			#region Process a valid return value
			xmlNode=xmlDocument.SelectSingleNode("//IsAdminMode");
			if(xmlNode.InnerText=="true") {
				_isAdminMode=true;
			}
			else {
				_isAdminMode=false;
			}
			xmlNode=xmlDocument.SelectSingleNode("//ResultTable");
			#endregion Process a valid return value
			_tableRequests=new ODDataTable(xmlNode.InnerXml);
		}

		private void FillGrid() {
			table=_tableRequests;
			table.Rows.Sort(FeatureRequestSort);//Sort user submited/voted features to the top. 
			//FillGrid used to start here------------------------------------------------
			long selectedRequestId=0;
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex!=-1){
				if(table.Rows.Count>selectedIndex){
					selectedRequestId=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["RequestId"]);
				}
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRequest","Req#"),40,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Mine"),40,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","My Votes"),60,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Total Votes"),70,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Comments"),70,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Version\r\nCompleted"),75,GridSortingStrategy.VersionNumber);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Diff"),40,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Weight"),45,HorizontalAlignment.Right,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Approval"),90,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Description"),500,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				if((checkMine.Checked && PIn.String(table.Rows[i]["isMine"])!="X") 
					|| (checkMyVotes.Checked && PIn.String(table.Rows[i]["myVotes"])==""))
				{
					continue;
				}
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["RequestId"]);
				row.Cells.Add(table.Rows[i]["isMine"]);
				row.Cells.Add(table.Rows[i]["myVotes"]);
				row.Cells.Add(table.Rows[i]["totalVotes"]);
				row.Cells.Add(table.Rows[i]["TotalComments"]);
				row.Cells.Add(table.Rows[i]["versionCompleted"]);
				row.Cells.Add(table.Rows[i]["Difficulty"]);
				row.Cells.Add(PIn.Double(table.Rows[i]["Weight"]).ToString("F"));
				row.Cells.Add(table.Rows[i]["approval"]);
				row.Cells.Add(table.Rows[i]["Description"]);
				//If they voted or pledged on this feature, mark it so they can see. Can be re-added when/if they need to be more visible.
				if(table.Rows[i]["approval"].ToString()!="Complete"){
					if(table.Rows[i]["isMine"].ToString()!="" 
						|| table.Rows[i]["personalVotes"].ToString()!="0"
						|| table.Rows[i]["personalCrit"].ToString()!="0") 
					{
						row.ColorBackG=Color.FromArgb(255,255,230);//light yellow.
					}
				}
				gridMain.ListGridRows.Add(row);
				row.Tag=table.Rows[i];
			}
			gridMain.EndUpdate();
			for(int i=0;i<table.Rows.Count;i++){
				if(selectedRequestId.ToString()==table.Rows[i]["RequestId"]){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void buttonAdd_Click(object sender,EventArgs e) {
			using FormRequestEdit formRequestEdit=new FormRequestEdit();
			//FormR.IsNew=true;
			formRequestEdit.IsAdminMode=_isAdminMode;
			formRequestEdit.ShowDialog();
			textSearch.Text="";//so we can see our new request
			RefreshRequestTable();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ODDataRow gridRow=(ODDataRow)gridMain.ListGridRows[e.Row].Tag;
			if(IsSelectionMode) {
				SelectedFeatureNum=PIn.Long(gridRow["RequestId"]);
				DialogResult=DialogResult.OK;
				return;
			}
			using FormRequestEdit formRequestEdit=new FormRequestEdit();
			formRequestEdit.RequestId=PIn.Long(gridRow["RequestId"]);
			formRequestEdit.IsAdminMode=_isAdminMode;
			formRequestEdit.ShowDialog();
			//The user could have voted towards this request or done something else and we need to refresh the table and the grid.
			//If the user had sorted the grid by a specific column, this call will lose their column sort and put it back to the default.
			RefreshRequestTable();
			FillGrid();
		}

		private void checkMine_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkMyVotes_CheckedChanged(object sender,EventArgs e) {
			FillGrid();	
		}

		private void butEdit_Click(object sender,EventArgs e) {
			//only visible in selection mode because double click would select
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a feature request.");
				return;
			}
			ODDataRow gridRow=(ODDataRow)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			using FormRequestEdit formRequestEdit=new FormRequestEdit();
			formRequestEdit.RequestId=PIn.Long(gridRow["RequestId"]);
			formRequestEdit.IsAdminMode=_isAdminMode;
			formRequestEdit.ShowDialog();
			//The user could have voted towards this request or done something else and we need to refresh the table and the grid.
			//If the user had sorted the grid by a specific column, this call will lose their column sort and put it back to the default.
			RefreshRequestTable();
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a feature request.");
				return;
			}
			SelectedFeatureNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["RequestId"]);
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			if(IsSelectionMode) {
				DialogResult=DialogResult.Cancel;
			}
			Close();
		}

		private void FormUpdate_FormClosing(object sender,FormClosingEventArgs e) {
			
		}

		///<summary>For sorting FRs because we do not have access to the ApprovalEnum in Resuests.cs in the WebServiceCustomerUpdates solution.</summary>
		private static List<string> _arrayApprovalEnumStrings=new List<string> {
			"New",//0, although this comes back as -1 now.
			"NeedsClarification",//1, deprecated
			"Redundant",//2, deprecated
			"TooBroad",//3, deprecated
			"NotARequest",//4, deprecated
			"AlreadyDone",//5, deprecated
			"Obsolete",//6, deprecated
			"Approved",//7
			"InProgress",//8
			"Complete"//9
		};

		///<summary>Used to sort feature requests for user to the top of the list.</summary>
		private int FeatureRequestSort(ODDataRow gridRowX,ODDataRow gridRowY) {
			if(_isAdminMode) {
				//Sorting order
				//	1) New items to the bottom (these are coming back now as "" instead of "new", so -1 for comparison
				//	2) Approval
				//	3) Weight
				//	4) Request ID if all else fails
				//Part 1
				int xIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==gridRowX["approval"].ToString());//-1 if not found
				int yIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==gridRowY["approval"].ToString());
				if(xIdx!=yIdx) { 
					if(xIdx==-1 || yIdx==-1) { 
						return (xIdx==-1).CompareTo(yIdx==-1);
					}
				}
				//Part 2
				if(xIdx!=yIdx) {
					return xIdx.CompareTo(yIdx);
				}
				//Part 3
				if(gridRowX["Weight"].ToString()!=gridRowY["Weight"].ToString()) {
					return -(PIn.Double(gridRowX["Weight"].ToString()).CompareTo(PIn.Double(gridRowY["Weight"].ToString())));//Larger number of votes go to the top
				}
				//Part 4
				return PIn.Long(gridRowX["RequestId"].ToString()).CompareTo(PIn.Long(gridRowY["RequestId"].ToString()));
			}
			else { //Typical sorting order for all users (non-HQ)
				int xIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==gridRowX["approval"].ToString());
				int yIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==gridRowY["approval"].ToString());
				//1:  Personal requests to top
				bool isPersonalX=false;
				bool isPersonalY=false;
				isPersonalX=gridRowX["personalVotes"].ToString()!="0" || gridRowX["personalCrit"].ToString()!="0" || gridRowX["personalPledged"].ToString()!="0";
				isPersonalY=gridRowY["personalVotes"].ToString()!="0" || gridRowY["personalCrit"].ToString()!="0" || gridRowY["personalPledged"].ToString()!="0";
				if(isPersonalX!=isPersonalY 
					&& gridRowX["approval"].ToString()!="Complete"  
					&& gridRowY["approval"].ToString()!="Complete") 
				{
					return -isPersonalX.CompareTo(isPersonalY);//negative comparison to move personal items to top.
				}
				//Part 2: Personal requests sorted by weight
				if(isPersonalX && isPersonalY
					&& gridRowX["approval"].ToString()!="Complete"  
					&& gridRowY["approval"].ToString()!="Complete") 
				{
					if(gridRowX["Weight"].ToString()!=gridRowY["Weight"].ToString()) {
						return -(PIn.Double(gridRowX["Weight"].ToString()).CompareTo(PIn.Double(gridRowY["Weight"].ToString())));//Larger number of votes go to the top
					}
				}
				//Part 3; Sort "Mine" entries above non-"Mine" entries.  "Mine" means any feature that this office has submitted.
				if(gridRowX["isMine"].ToString()!=gridRowY["isMine"].ToString()
					&& gridRowX["approval"].ToString()!="Complete"  
					&& gridRowY["approval"].ToString()!="Complete") 
				{//One is the customer's, the other isn't.
					return -(gridRowX["isMine"].ToString().CompareTo(gridRowY["isMine"].ToString()));//It will either be "" or X, and "X" goes to the top
				}
				//Part 4: New items to the bottom
				if(xIdx!=yIdx) {
					if(xIdx==-1||yIdx==-1) {
						return (xIdx==-1).CompareTo(yIdx==-1);
					}
				}
				//Part 5: Approval
				if(xIdx!=yIdx) {
					return xIdx.CompareTo(yIdx);
				}
				//Part 6: Weight by magnitude
				if(gridRowX["Weight"].ToString()!=gridRowY["Weight"].ToString()) {
					return -(PIn.Double(gridRowX["Weight"].ToString()).CompareTo(PIn.Double(gridRowY["Weight"].ToString())));//Larger number of votes go to the top
				}
				//Part 7: Request ID
				return PIn.Long(gridRowX["RequestId"].ToString()).CompareTo(PIn.Long(gridRowY["RequestId"].ToString()));
			}
		}
	}

	
}

























