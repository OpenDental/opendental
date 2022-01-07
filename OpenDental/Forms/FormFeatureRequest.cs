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
		private bool isAdminMode;
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
			#region prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("FeatureRequestGetList");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("SearchString");
				writer.WriteString(textSearch.Text);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			#endregion
			#region Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=updateService.FeatureRequestGetList(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			#endregion
			Cursor=Cursors.Default;
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			#region Process errors------------------------------------------------------------------------------------------------------------
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				//textConnectionMessage.Text=node.InnerText;
				MessageBox.Show(node.InnerText,"Error");
				return;
			}
			node=doc.SelectSingleNode("//KeyDisabled");
			if(node==null) {
				//no error, and no disabled message
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				//textConnectionMessage.Text=node.InnerText;
				MessageBox.Show(node.InnerText);
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				return;
			}
			#endregion
			#region Process a valid return value------------------------------------------------------------------------------------------------
			node=doc.SelectSingleNode("//IsAdminMode");
			if(node.InnerText=="true") {
				isAdminMode=true;
			}
			else {
				isAdminMode=false;
			}
			node=doc.SelectSingleNode("//ResultTable");
			#endregion
			_tableRequests=new ODDataTable(node.InnerXml);
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRequest","Req#"),40,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Mine"),40,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","My Votes"),60,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Total Votes"),70,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Comments"),70,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Version\r\nCompleted"),75,GridSortingStrategy.VersionNumber);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Diff"),40,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Weight"),45,HorizontalAlignment.Right,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Approval"),90,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequest","Description"),500,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
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
			formRequestEdit.IsAdminMode=isAdminMode;
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
			using FormRequestEdit FormR=new FormRequestEdit();
			FormR.RequestId=PIn.Long(gridRow["RequestId"]);
			FormR.IsAdminMode=isAdminMode;
			FormR.ShowDialog();
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
			using FormRequestEdit FormR=new FormRequestEdit();
			FormR.RequestId=PIn.Long(gridRow["RequestId"]);
			FormR.IsAdminMode=isAdminMode;
			FormR.ShowDialog();
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
		private int FeatureRequestSort(ODDataRow x,ODDataRow y) {
			if(isAdminMode) {
				//Sorting order
				//	1) New items to the bottom (these are coming back now as "" instead of "new", so -1 for comparison
				//	2) Approval
				//	3) Weight
				//	4) Request ID if all else fails
				//Part 1
				int xIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==x["approval"].ToString());//-1 if not found
				int yIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==y["approval"].ToString());
				if(xIdx!=yIdx && (xIdx==-1||yIdx==-1)) {// 
					return (xIdx==-1).CompareTo(yIdx==-1);
				}
				//Part 2
				if(xIdx!=yIdx) {
					return xIdx.CompareTo(yIdx);
				}
				//Part 3
				if(x["Weight"].ToString()!=y["Weight"].ToString()) {
					return -(PIn.Double(x["Weight"].ToString()).CompareTo(PIn.Double(y["Weight"].ToString())));//Larger number of votes go to the top
				}
				//Part 4
				return PIn.Long(x["RequestId"].ToString()).CompareTo(PIn.Long(y["RequestId"].ToString()));
			}
			else { //Typical sorting order for all users (non-HQ)
				int xIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==x["approval"].ToString());
				int yIdx=_arrayApprovalEnumStrings.FindIndex(e=>e==y["approval"].ToString());
				//1:  Personal requests to top
				bool xIsPersonal=false;
				bool yIsPersonal=false;
				xIsPersonal=x["personalVotes"].ToString()!="0" || x["personalCrit"].ToString()!="0" || x["personalPledged"].ToString()!="0";
				yIsPersonal=y["personalVotes"].ToString()!="0" || y["personalCrit"].ToString()!="0" || y["personalPledged"].ToString()!="0";
				if(xIsPersonal!=yIsPersonal 
					&& x["approval"].ToString()!="Complete"  
					&& y["approval"].ToString()!="Complete") 
				{
					return -xIsPersonal.CompareTo(yIsPersonal);//negative comparison to move personal items to top.
				}
				//Part 2: Personal requests sorted by weight
				if(xIsPersonal && yIsPersonal
					&& x["approval"].ToString()!="Complete"  
					&& y["approval"].ToString()!="Complete") 
				{
					if(x["Weight"].ToString()!=y["Weight"].ToString()) {
						return -(PIn.Double(x["Weight"].ToString()).CompareTo(PIn.Double(y["Weight"].ToString())));//Larger number of votes go to the top
					}
				}
				//Part 3; Sort "Mine" entries above non-"Mine" entries.  "Mine" means any feature that this office has submitted.
				if(x["isMine"].ToString()!=y["isMine"].ToString()
					&& x["approval"].ToString()!="Complete"  
					&& y["approval"].ToString()!="Complete") 
				{//One is the customer's, the other isn't.
					return -(x["isMine"].ToString().CompareTo(y["isMine"].ToString()));//It will either be "" or X, and "X" goes to the top
				}
				//Part 4: New items to the bottom
				if(xIdx!=yIdx && (xIdx==-1||yIdx==-1)) {
					return (xIdx==-1).CompareTo(yIdx==-1);
				}
				//Part 5: Approval
				if(xIdx!=yIdx) {
					return xIdx.CompareTo(yIdx);
				}
				//Part 6: Weight by magnitude
				if(x["Weight"].ToString()!=y["Weight"].ToString()) {
					return -(PIn.Double(x["Weight"].ToString()).CompareTo(PIn.Double(y["Weight"].ToString())));//Larger number of votes go to the top
				}
				//Part 7: Request ID
				return PIn.Long(x["RequestId"].ToString()).CompareTo(PIn.Long(y["RequestId"].ToString()));
			}
		}
	}

	
}

























