using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OpenDental.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRegistrationKeyEdit : FormODBase {
		public RegistrationKey RegKey;

		///<summary></summary>
		public FormRegistrationKeyEdit(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRegistrationKeyEdit_Load(object sender,EventArgs e) {
			if(RegKey.RegKey.Length==16){
				textKey.Text=RegKey.RegKey.Substring(0,4)+"-"+RegKey.RegKey.Substring(4,4)+"-"
					+RegKey.RegKey.Substring(8,4)+"-"+RegKey.RegKey.Substring(12,4);
			}
			else{
				textKey.Text=RegKey.RegKey;
			}
			textPatNum.Text=RegKey.PatNum.ToString();
			checkForeign.Checked=RegKey.IsForeign;
			checkFree.Checked=RegKey.IsFreeVersion;
			checkTesting.Checked=RegKey.IsOnlyForTesting;
			checkResellerCustomer.Checked=RegKey.IsResellerCustomer;
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				checkResellerCustomer.Enabled=true;
				label13.Enabled=true;
			}
			//checkServerVersion.Checked=RegKey.UsesServerVersion;
			textDateStarted.Text=RegKey.DateStarted.ToShortDateString();
			if(RegKey.DateDisabled.Year>1880){
				textDateDisabled.Text=RegKey.DateDisabled.ToShortDateString();
			}
			if(RegKey.DateEnded.Year>1880){
				textDateEnded.Text=RegKey.DateEnded.ToShortDateString();
			}
			textVotesAllotted.Text=RegKey.VotesAllotted.ToString();
			textNote.Text=RegKey.Note;
			//Make the practice title reset button visible for HQ.
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				butPracticeTitleReset.Visible=true;
			}
			checkEarlyAccess.Checked=RegKey.HasEarlyAccess;
			if(PrefC.IsODHQ) {
				FillGrid();
			}
		}

		private void FillGrid() {
			DataTable table=Bugs.GetCustomerVersionsForRegKey(RegKey.RegKey);
			gridMain.BeginUpdate();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Version To"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Version From"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"IP Address"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow newRow;
			for(int i=0;i<table.Rows.Count;i++) {
				newRow=new GridRow();
				newRow.Cells.Add(PIn.Date(table.Rows[i]["DateTimeUpdate"].ToString()).ToShortDateString());
				newRow.Cells.Add(table.Rows[i]["VersionTo"].ToString());
				newRow.Cells.Add(table.Rows[i]["VersionFrom"].ToString());
				newRow.Cells.Add(table.Rows[i]["LocalIPAddress"].ToString());
				gridMain.ListGridRows.Add(newRow);
			}
			gridMain.EndUpdate();
		}

		private void butMoveTo_Click(object sender,EventArgs e) {
			using FormPatientSelect form=new FormPatientSelect();
			if(form.ShowDialog()!=DialogResult.OK) {
				return;
			}
			RegKey.PatNum=form.SelectedPatNum;
			RegistrationKeys.Update(RegKey);
			MessageBox.Show("Registration key moved successfully");
			DialogResult=DialogResult.OK;//Chart module grid will refresh after closing this form, showing that the key is no longer in the ptinfo grid of the chart.
		}

		private void checkForeign_Click(object sender,EventArgs e) {
			checkForeign.Checked=RegKey.IsForeign;//don't allow user to change
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateEnded.Text=DateTime.Today.ToShortDateString();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			try{
				RegistrationKeys.Delete(RegKey.RegistrationKeyNum);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
			Close();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateStarted.IsValid()
				|| !textDateDisabled.IsValid()
				|| !textDateEnded.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			try {
				PIn.Int(textVotesAllotted.Text);
			}
			catch {
				MsgBox.Show(this,"Votes Allotted is invalid.");
				return;
			}
			//RegKey.RegKey=textKey.Text;//It's read only.
			RegKey.DateStarted=PIn.Date(textDateStarted.Text);
			RegKey.DateDisabled=PIn.Date(textDateDisabled.Text);
			RegKey.DateEnded=PIn.Date(textDateEnded.Text);
			RegKey.IsFreeVersion=checkFree.Checked;
			RegKey.IsOnlyForTesting=checkTesting.Checked;
			//Check if Reseller status was changed and create commlog if so.
			if(RegKey.IsResellerCustomer!=checkResellerCustomer.Checked) {
				createResellerChangeCommlog();
			}
			RegKey.IsResellerCustomer=checkResellerCustomer.Checked;
			//RegKey.UsesServerVersion=checkServerVersion.Checked;
			RegKey.VotesAllotted=PIn.Int(textVotesAllotted.Text);
			RegKey.Note=textNote.Text;
			RegKey.HasEarlyAccess=checkEarlyAccess.Checked;
			RegistrationKeys.Update(RegKey);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		/// <summary>Inserts RegistrationKey with blank PracticeTitle into bugs database so next time cusotmer hits the update service it will reset their PracticeTitle.</summary>
		private void butPracticeTitleReset_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to reset the Practice Title associated with this Registration Key? This should only be done if they are getting a message saying, \"Practice title given does not match the practice title on record,\" when connecting to the Patient Portal. It will be cleared out of the database and filled in with the appropriate Practice Title next time they connect using this Registration Key.")) {
				return;
			}
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("PracticeTitleReset");
					writer.WriteStartElement("RegistrationKey");
						writer.WriteString(RegKey.RegKey);
					writer.WriteEndElement();
				writer.WriteEndElement();
			}
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				proxy.Credentials=cred;
				updateService.Proxy=proxy;
			}
			updateService.PracticeTitleReset(strbuild.ToString());//may throw error
		}

		private void createResellerChangeCommlog() {
			//Should only be enabled if form accessed with SecurityAdmin privileges
			if(!checkResellerCustomer.Enabled) {
				return;
			}
			string checkState=checkResellerCustomer.Checked?"checked":"unchecked";
			long userNum=Security.CurUser.UserNum;
			DataAction.RunCustomers(() => {
				Commlog comm= new Commlog()
				{
					PatNum=RegKey.PatNum,
					SentOrReceived=CommSentOrReceived.Neither,
					CommDateTime=DateTime.Now,
					CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.ODHQ),
					Mode_=CommItemMode.None,
					Note=$"User {userNum} changed Reseller checkbox to {checkState} in Registration Key Edit form.",
					UserNum=userNum
				};
				Commlogs.Insert(comm);
			});
		}
	}
}





















