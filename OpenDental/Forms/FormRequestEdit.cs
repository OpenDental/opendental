using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormRequestEdit:FormODBase {
		public long RequestId;
		public bool IsAdminMode;
		private ODDataTable tableObj;
		private Color colorDisabled;
		private int myPointsUsed=0;
		private int myPointsAllotted=0;
		private int _patNumNew=-1;

		public FormRequestEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			colorDisabled=Color.FromArgb(230, 229, 233);
		}

		private void FormRequestEdit_Load(object sender,EventArgs e) {
			if(IsAdminMode){
				checkIsMine.Visible=false;
				labelSubmitter.Visible=true;
				textSubmitter.Visible=true;
				textSubmitter.BackColor=colorDisabled;
				textDifficulty.BackColor=Color.White;
				textDifficulty.ReadOnly=false;
			}
			else{
				butSetOD.Visible=false;
				textDifficulty.BackColor=colorDisabled;
				comboApproval.Visible=false;
				textRequestId.ReadOnly=true;
			}
			if(RequestId==0){
				checkIsMine.Checked=true;
				textDifficulty.Text="5";
				labelDiscuss.Visible=false;
				butAddDiscuss.Visible=false;
				textNote.Visible=false;
				//gridMain.Height=butDelete.Top-gridMain.Top-4;
				gridMain.Visible=false;
			}
			textRequestId.Text=RequestId.ToString();
			textApproval.BackColor=colorDisabled;
			comboApproval.Items.Add("New");
			comboApproval.Items.Add("Approved");
			comboApproval.Items.Add("InProgress");
			comboApproval.Items.Add("Complete");
			comboApproval.SelectedIndex=0;
			textMyPoints.Text="0";
			//if(RequestId!=0){
				GetOneFromServer();//fills mypointsRemaining
				FillGrid();
			//}
			textMyPointsRemain.BackColor=colorDisabled;
			textTotalPoints.BackColor=colorDisabled;
			textTotalCritical.BackColor=colorDisabled;
			textWeight.BackColor=colorDisabled;
			if(PrefC.IsODHQ && !Security.IsAuthorized(Permissions.FeatureRequestEdit,true)) {
				this.DisableAllExcept(butCancel,butAddDiscuss,textNote,textDetail);
				textDetail.ReadOnly=true;
			}
		}

		private void GetOneFromServer(){
			//get a table with data
			Cursor=Cursors.WaitCursor;
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)){
				writer.WriteStartElement("FeatureRequestGetOne");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("RequestId");
				writer.WriteString(RequestId.ToString());
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=updateService.FeatureRequestGetOne(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			//textConnectionMessage.Text=Lan.g(this,"Connection successful.");
			//Application.DoEvents();
			Cursor=Cursors.Default;
			//MessageBox.Show(result);
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			//Process errors------------------------------------------------------------------------------------------------------------
			string strError=WebServiceRequest.CheckForErrors(doc);
			if(!string.IsNullOrEmpty(strError)) {
				MessageBox.Show(strError,"Error");
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			//Process a valid return value------------------------------------------------------------------------------------------------
			XmlNode node=doc.SelectSingleNode("//ResultTable");
			tableObj=new ODDataTable(node.InnerXml);
			ODDataRow row=tableObj.Rows[0];
			node=doc.SelectSingleNode("//MaxPoints");
			int maxPoints=PIn.Int(node.InnerText);
			node=doc.SelectSingleNode("//MaxCrit");
			int maxCritical=PIn.Int(node.InnerText);
			node=doc.SelectSingleNode("//MaxPledged");
			double maxPledged=PIn.Double(node.InnerText);
			textDescription.Text=row["Description"];
			string detail=row["Detail"];
			detail=detail.Replace("\n","\r\n");
			textDetail.Text=detail;
			checkIsMine.Checked=PIn.Bool(row["isMine"]);
			textDifficulty.Text=row["Difficulty"];
			ApprovalEnum approval=(ApprovalEnum)PIn.Int(row["Approval"]);
			if(IsAdminMode){
				textSubmitter.Text=row["submitter"];
			}
			switch(approval){
				case ApprovalEnum.New:
					comboApproval.SelectedIndex=0;
					break;
				case ApprovalEnum.Approved:
					comboApproval.SelectedIndex=1;
					break;
				case ApprovalEnum.InProgress:
					comboApproval.SelectedIndex=2;
					break;
				case ApprovalEnum.Complete:
					comboApproval.SelectedIndex=3;
					break;
			}
			//textApproval gets set automatically due to comboApproval_SelectedIndexChanged.
			if(!IsAdminMode && PIn.Bool(row["isMine"])//user editing their own request
				&& (ApprovalEnum)approval==ApprovalEnum.New)
			{
				butDelete.Visible=true;
			}
			if(!IsAdminMode){
				if((ApprovalEnum)approval==ApprovalEnum.Approved
					|| (ApprovalEnum)approval==ApprovalEnum.InProgress
					|| (ApprovalEnum)approval==ApprovalEnum.Complete)
				{
					textDescription.BackColor=colorDisabled;
					textDescription.ReadOnly=true;
					textDetail.BackColor=colorDisabled;
					textDetail.ReadOnly=true;
				}
			}
			//Note: If the RegKey is in "Admin mode" the points allotted will always be 100 even though some may be used on other feature requests.
			myPointsUsed=PIn.Int(row["myPointsUsed"]);
			try {
				myPointsAllotted=PIn.Int(row["myPointsAllotted"]);
			}
			catch {
				myPointsAllotted=100;
			}
			//textMyPointsRemain.Text=;this will be filled automatically when myPoints changes
			textMyPoints.Text=row["myPoints"];
			RecalcMyPoints();
			checkIsCritical.Checked=PIn.Bool(row["IsCritical"]);
			textTotalPoints.Text=row["totalPoints"];
			textTotalCritical.Text=row["totalCritical"];
			textWeight.Text=row["Weight"];
		}

		private void textMyPoints_TextChanged(object sender,EventArgs e) {
			RecalcMyPoints();
		}

		private void RecalcMyPoints(){
			try{
				int mypoints=0;
				if(textMyPoints.Text!=""){
					mypoints=Convert.ToInt32(textMyPoints.Text);
				}
				textMyPointsRemain.Text=(myPointsAllotted-myPointsUsed-mypoints).ToString();
			}
			catch{
				textMyPointsRemain.Text="";
			}
		}

		private void butSetOD_Click(object sender,EventArgs e) {
			//only visible in admin, of course.
			textSubmitter.Text="Sparks, Jordan";
			_patNumNew=1486;
		}

		private void comboApproval_SelectedIndexChanged(object sender,EventArgs e) {
			int approval=comboApproval.SelectedIndex;
			switch(approval){
				case 0://new
					textApproval.Text="";
					break;
				case 1:
					textApproval.Text="Approved. Feature is being considered for implementation.";
					break;
				case 2:
					textApproval.Text="In Progress. Feature currently being programmed.";
					break;
				case 3:
					textApproval.Text="Complete. Feature has been implemented.";
					break;
			}
		}

		private void checkIsCritical_Click(object sender,EventArgs e) {
			if(checkIsCritical.Checked){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure this is really critical?  To qualify as critical, there would be no possible workarounds.  The missing feature would probably be seriously impacting the financial status of the office.  It would be serious enough that you might be considering using another software.")){
					checkIsCritical.Checked=false;
					return;
				}
			}
		}

		private void butAddDiscuss_Click(object sender,EventArgs e) {
			//button is not even visible if New
			if(textNote.Text==""){
				MsgBox.Show(this,"Please enter some text first.");
				return;
			}
			if(!SaveDiscuss()){
				return;
			}
			textNote.Text="";
			FillGrid();
		}

		///<summary>Never happens with a new request.</summary>
		private void FillGrid(){
			Cursor=Cursors.WaitCursor;
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)){
				writer.WriteStartElement("FeatureRequestDiscussGetList");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("RequestId");
				writer.WriteString(RequestId.ToString());
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=updateService.FeatureRequestDiscussGetList(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			//textConnectionMessage.Text=Lan.g(this,"Connection successful.");
			//Application.DoEvents();
			Cursor=Cursors.Default;
			//MessageBox.Show(result);
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			//Process errors------------------------------------------------------------------------------------------------------------
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				//textConnectionMessage.Text=node.InnerText;
				MessageBox.Show(node.InnerText,"Error");
				return;
			}
			//Process a valid return value------------------------------------------------------------------------------------------------
			node=doc.SelectSingleNode("//ResultTable");
			ODDataTable table=new ODDataTable(node.InnerXml);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRequestDiscuss","Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRequestDiscuss","Note"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["dateTime"]);
				row.Cells.Add(table.Rows[i]["Note"]);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary></summary>
		private bool SaveDiscuss(){//bool doDelete) {
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)){
				writer.WriteStartElement("FeatureRequestDiscussSubmit");
				//regkey
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				//DiscussId
				//writer.WriteStartElement("DiscussId");
				//writer.WriteString(DiscussIdCur.ToString());//this will be zero for a new entry. We currently only support new entries
				//writer.WriteEndElement();
				//RequestId
				writer.WriteStartElement("RequestId");
				writer.WriteString(RequestId.ToString());
				writer.WriteEndElement();
				//can't pass patnum.  Determined on the server side.
				//date will also be figured on the server side.
				//Note
				writer.WriteStartElement("Note");
				writer.WriteString(textNote.Text);
				writer.WriteEndElement();
				/*if(doDelete){
					//delete
					writer.WriteStartElement("Delete");
					writer.WriteString("true");
					writer.WriteEndElement();
				}*/
			}
			Cursor=Cursors.WaitCursor;
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=updateService.FeatureRequestDiscussSubmit(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return false;
			}
			//textConnectionMessage.Text=Lan.g(this,"Connection successful.");
			//Application.DoEvents();
			Cursor=Cursors.Default;
			//MessageBox.Show(result);
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			//Process errors------------------------------------------------------------------------------------------------------------
			string strError=WebServiceRequest.CheckForErrors(doc);
			if(!string.IsNullOrEmpty(strError)) {
				MessageBox.Show(strError,"Error");
				return false;
			}
			return true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//only visible if isMine
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this entire request?")){
				return;
			}
			if(!SaveChangesToDb(true)){
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Only called when user clicks Delete or OK.  Not called repeatedly when adding discussions.</summary>
		private bool SaveChangesToDb(bool doDelete) {
			#region validation
			//validate---------------------------------------------------------------------------------------------------------
			int difficulty=0;
			int myPoints=0;
			double bounty=0;
			if(!doDelete){
				if(textDescription.Text==""){
					MsgBox.Show(this,"Description cannot be blank.");
					return false;
				}
				try{
					difficulty=int.Parse(textDifficulty.Text);
				}
				catch{
					MsgBox.Show(this,"Difficulty is invalid.");
					return false;
				}
				if(difficulty<0 || difficulty>10){
					MsgBox.Show(this,"Difficulty is invalid.");
					return false;
				}
				if(!IsAdminMode){
					try{
						myPoints=PIn.Int(textMyPoints.Text);//handles "" gracefully
					}
					catch{
						MsgBox.Show(this,"Points is invalid.");
						return false;
					}
					if(difficulty<0 || difficulty>100){
						MsgBox.Show(this,"Points is invalid.");
						return false;
					}
					//still need to validate that they have enough points.
				}
				double myPointsRemain=PIn.Double(textMyPointsRemain.Text);
				if(myPointsRemain<0){
					MsgBox.Show(this,"You have gone over your allotted points.");
					return false;
				}
			}
			//end of validation------------------------------------------------------------------------------------------------
			#endregion validation
			//if user has made no changes, then exit out-------------------------------------------------------------------------
			ApprovalEnum approvalNew=ApprovalEnum.New;
			switch(comboApproval.SelectedIndex){
				case 1:
					approvalNew=ApprovalEnum.Approved;
					break;
				case 2:
					approvalNew=ApprovalEnum.InProgress;
					break;
				case 3:
					approvalNew=ApprovalEnum.Complete;
					break;
			}
			bool changesMade=false;
			if(doDelete){
				changesMade=true;
			}
			if(tableObj==null || tableObj.Rows.Count==0){//new
				changesMade=true;
			}
			else{
				ODDataRow row=tableObj.Rows[0];
				if(textDescription.Text!=row["Description"]){
					changesMade=true;
				}
				if(textDetail.Text!=row["Detail"]){
					changesMade=true;
				}
				if(textDifficulty.Text!=row["Difficulty"]){
					changesMade=true;
				}
				ApprovalEnum approvalOld=(ApprovalEnum)PIn.Int(row["Approval"]);
				if(approvalNew!=approvalOld){
					changesMade=true;
				}
				if(textMyPoints.Text!=row["myPoints"]
					|| checkIsCritical.Checked!=PIn.Bool(row["IsCritical"]))
				{
					changesMade=true;
				}
				if(_patNumNew!=-1){
					changesMade=true;
				}
			}
			if(!changesMade){
				//temporarily show me which ones shortcutted out
				//MessageBox.Show("no changes made");
				return true;
			}
			Cursor=Cursors.WaitCursor;
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)){
				writer.WriteStartElement("FeatureRequestSubmitChanges");
				//regkey
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				//requestId
				writer.WriteStartElement("RequestId");
				writer.WriteString(RequestId.ToString());//this will be zero for a new request.
				writer.WriteEndElement();
				if(doDelete){
					//delete
					writer.WriteStartElement("Delete");
					writer.WriteString("true");//all the other elements will be ignored.
					writer.WriteEndElement();
				}
				if(!textDescription.ReadOnly){
					//description
					writer.WriteStartElement("Description");
					writer.WriteString(textDescription.Text);
					writer.WriteEndElement();
				}
				if(!textDetail.ReadOnly){
					//detail
					writer.WriteStartElement("Detail");
					writer.WriteString(textDetail.Text);
					writer.WriteEndElement();
				}
				if(IsAdminMode
					|| RequestId==0)//This allows the initial difficulty of 5 to get saved.
				{
					//difficulty
					writer.WriteStartElement("Difficulty");
					writer.WriteString(difficulty.ToString());
					writer.WriteEndElement();
				}
				if(_patNumNew!=-1) {
					writer.WriteStartElement("PatNumNew");
					writer.WriteString(_patNumNew.ToString());
					writer.WriteEndElement();
				}
				//approval
				writer.WriteStartElement("Approval");
				writer.WriteString(((int)approvalNew).ToString());
				writer.WriteEndElement();
				if(!IsAdminMode){
					//mypoints
					writer.WriteStartElement("MyPoints");
					writer.WriteString(myPoints.ToString());
					writer.WriteEndElement();
					//iscritical
					writer.WriteStartElement("IsCritical");
					if(checkIsCritical.Checked){
						writer.WriteString("1");
					}
					else{
						writer.WriteString("0");
					}
					writer.WriteEndElement();
					//mypledge
					writer.WriteStartElement("MyPledge");
					writer.WriteString("0");//pledges are deprecated, but we want the xml to stay the same.
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=updateService.FeatureRequestSubmitChanges(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return false;
			}
			//textConnectionMessage.Text=Lan.g(this,"Connection successful.");
			//Application.DoEvents();
			Cursor=Cursors.Default;
			//MessageBox.Show(result);
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			//Process errors------------------------------------------------------------------------------------------------------------
			string strError=WebServiceRequest.CheckForErrors(doc);
			if(!string.IsNullOrEmpty(strError)) {
				MessageBox.Show(strError,"Error");
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textNote.Text!=""){
				MsgBox.Show(this,"You need to save your note first.");
				return;
			}
			if(!SaveChangesToDb(false)){
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		

		

		
	

		
		

		

		
	}

	/*
	///<summary>This object is used to organize all the datafields in FormRequestEdit.  It is used both for admin mode and for regular mode.</summary>
	public class Request{
		///<summary>Once approval is changed to Approved, this cannot be edited by submitter.</summary>
		public string Description;
		///<summary>Once approval is changed to Approved, this cannot be edited by submitter.</summary>
		public string Detail;
		///<summary></summary>
		public DateTime DateTSubmitted;
		///<summary>On the server, this is a PatNum.  Here on the client, it's true false.  This value is never sent to the server.  It's inferred on the server end.</summary>
		public bool IsMine;
		///<summary>Only set by Jordan.</summary>
		public int Difficulty;
		///<summary>Only set by admins.  In non-admin mode, this converts to a wordy description.</summary>
		public ApprovalEnum Approval;
		///<summary>Ignored when in admin mode.</summary>
		public int MyPoints;
		///<summary>Ignored when in admin mode.</summary>
		public bool IsCritical;
		///<summary>Ignored when in admin mode.</summary>
		public double MyPledge;
		///<summary>This is the points remaining with the assumption that zero points are consumed by this request.  Then, the points for the current request are subtracted from the amount before display.</summary>
		public int MyPointsRemain;
		///<summary>Just informational.  Nobody can edit.</summary>
		public string TotalPoints;
		///<summary>Just informational.  Nobody can edit.</summary>
		public string TotalCritical;
		///<summary>Just informational.  Nobody can edit.</summary>
		public string TotalPledged;
	}*/

	public enum ApprovalEnum{
		New,
		NeedsClarification,
		Redundant,
		TooBroad,
		NotARequest,
		AlreadyDone,
		Obsolete,
		Approved,
		InProgress,
		Complete
	}

}