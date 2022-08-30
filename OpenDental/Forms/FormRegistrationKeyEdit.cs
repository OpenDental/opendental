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
		public RegistrationKey RegistrationKeyCur;

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
			if(RegistrationKeyCur.RegKey.Length==16){
				textKey.Text=RegistrationKeyCur.RegKey.Substring(0,4)+"-"+RegistrationKeyCur.RegKey.Substring(4,4)+"-"
					+RegistrationKeyCur.RegKey.Substring(8,4)+"-"+RegistrationKeyCur.RegKey.Substring(12,4);
			}
			else{
				textKey.Text=RegistrationKeyCur.RegKey;
			}
			textPatNum.Text=RegistrationKeyCur.PatNum.ToString();
			checkForeign.Checked=RegistrationKeyCur.IsForeign;
			checkFree.Checked=RegistrationKeyCur.IsFreeVersion;
			checkTesting.Checked=RegistrationKeyCur.IsOnlyForTesting;
			checkResellerCustomer.Checked=RegistrationKeyCur.IsResellerCustomer;
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				checkResellerCustomer.Enabled=true;
				label13.Enabled=true;
			}
			//checkServerVersion.Checked=RegKey.UsesServerVersion;
			textDateStarted.Text=RegistrationKeyCur.DateStarted.ToShortDateString();
			if(RegistrationKeyCur.DateDisabled.Year>1880){
				textDateDisabled.Text=RegistrationKeyCur.DateDisabled.ToShortDateString();
			}
			if(RegistrationKeyCur.DateEnded.Year>1880){
				textDateEnded.Text=RegistrationKeyCur.DateEnded.ToShortDateString();
			}
			textVotesAllotted.Text=RegistrationKeyCur.VotesAllotted.ToString();
			textNote.Text=RegistrationKeyCur.Note;
			//Make the practice title reset button visible for HQ.
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				butPracticeTitleReset.Visible=true;
			}
			checkEarlyAccess.Checked=RegistrationKeyCur.HasEarlyAccess;
			if(PrefC.IsODHQ) {
				FillGrid();
			}
		}

		private void FillGrid() {
			DataTable table=Bugs.GetCustomerVersionsForRegKey(RegistrationKeyCur.RegKey);
			gridMain.BeginUpdate();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),100,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Version To"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Version From"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"IP Address"),100);
			gridMain.Columns.Add(col);
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
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			RegistrationKeyCur.PatNum=formPatientSelect.PatNumSelected;
			RegistrationKeys.Update(RegistrationKeyCur);
			MessageBox.Show("Registration key moved successfully");
			DialogResult=DialogResult.OK;//Chart module grid will refresh after closing this form, showing that the key is no longer in the ptinfo grid of the chart.
		}

		private void checkForeign_Click(object sender,EventArgs e) {
			checkForeign.Checked=RegistrationKeyCur.IsForeign;//don't allow user to change
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateEnded.Text=DateTime.Today.ToShortDateString();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			try{
				RegistrationKeys.Delete(RegistrationKeyCur.RegistrationKeyNum);
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
			RegistrationKey registrationKeyCopy=RegistrationKeyCur.Copy();
			//RegKey.RegKey=textKey.Text;//It's read only.
			RegistrationKeyCur.DateStarted=PIn.Date(textDateStarted.Text);
			RegistrationKeyCur.DateDisabled=PIn.Date(textDateDisabled.Text);
			RegistrationKeyCur.DateEnded=PIn.Date(textDateEnded.Text);
			RegistrationKeyCur.IsFreeVersion=checkFree.Checked;
			RegistrationKeyCur.IsOnlyForTesting=checkTesting.Checked;
			//Check if Reseller status was changed and create commlog if so.
			if(RegistrationKeyCur.IsResellerCustomer!=checkResellerCustomer.Checked) {
				createResellerChangeCommlog();
			}
			RegistrationKeyCur.IsResellerCustomer=checkResellerCustomer.Checked;
			//RegKey.UsesServerVersion=checkServerVersion.Checked;
			RegistrationKeyCur.VotesAllotted=PIn.Int(textVotesAllotted.Text);
			RegistrationKeyCur.Note=textNote.Text;
			RegistrationKeyCur.HasEarlyAccess=checkEarlyAccess.Checked;
			bool hasChanged=RegistrationKeys.Update(RegistrationKeyCur,registrationKeyCopy);
			if(hasChanged) { //check this statement out
				string logText="The key has been edited.";
				//Does a check for any changes in date and logs those changes
				if(RegistrationKeyCur.DateStarted!=registrationKeyCopy.DateStarted) {
					logText+=" DateStarted from "+registrationKeyCopy.DateStarted.ToShortDateString()+" changed to "+RegistrationKeyCur.DateStarted.ToShortDateString()+".";
				}
				if(RegistrationKeyCur.DateDisabled!=registrationKeyCopy.DateDisabled) {
					logText+=" DateDisabled from "+registrationKeyCopy.DateDisabled.ToShortDateString()+" changed to "+RegistrationKeyCur.DateDisabled.ToShortDateString()+".";
				}
				if(RegistrationKeyCur.DateEnded!=registrationKeyCopy.DateEnded) {
					logText+=" DateEnded from "+registrationKeyCopy.DateEnded.ToShortDateString()+" changed to " +RegistrationKeyCur.DateEnded.ToShortDateString()+".";
				}
				if(RegistrationKeyCur.Note!=registrationKeyCopy.Note) {
					logText+=" The Note field was changed.";
				}
				SecurityLogs.MakeLogEntry(Permissions.RegistrationKeyEdit,RegistrationKeyCur.PatNum,logText);
			}
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
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			using XmlWriter writer=XmlWriter.Create(stringBuilder,xmlWriterSettings);
			writer.WriteStartElement("PracticeTitleReset");
			writer.WriteStartElement("RegistrationKey");
			writer.WriteString(RegistrationKeyCur.RegKey);
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.Close();
			#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy iWebProxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials iCredentials=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				iWebProxy.Credentials=iCredentials;
				updateService.Proxy=iWebProxy;
			}
			updateService.PracticeTitleReset(stringBuilder.ToString());//may throw error
		}

		private void createResellerChangeCommlog() {
			//Should only be enabled if form accessed with SecurityAdmin privileges
			if(!checkResellerCustomer.Enabled) {
				return;
			}
			string checkState=checkResellerCustomer.Checked?"checked":"unchecked";
			long userNum=Security.CurUser.UserNum;
			Commlog commLog= new Commlog();
			commLog.PatNum=RegistrationKeyCur.PatNum;
			commLog.SentOrReceived=CommSentOrReceived.Neither;
			commLog.CommDateTime=DateTime.Now;
			commLog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.ODHQ);
			commLog.Mode_=CommItemMode.None;
			commLog.Note=$"User {userNum} changed Reseller checkbox to {checkState} in Registration Key Edit form.";
			commLog.UserNum=userNum;
			Action action=new Action(()=>{
				Commlogs.Insert(commLog);
			});
			DataAction.RunCustomers(action);
		}
	}
}





















