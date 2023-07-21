using System;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using WebServiceSerializer;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormCloudSessionLimit:FormODBase {
		public FormCloudSessionLimit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCloudSessionLimit_Load(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.SecurityAdmin)) {
				butOK.Enabled=true;
			}
			string officeData=PayloadHelper.CreatePayload("",eServiceCode.Undefined);
			string thinfinityLicenseUsers="";
			Cursor=Cursors.WaitCursor;
			try {
				thinfinityLicenseUsers=WebSerializer.DeserializePrimitive<long>( WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetThinfinityLicenseUsers(officeData)).ToString();
			}
			catch(Exception ex) {
				ex.DoNothing();
				thinfinityLicenseUsers="Error";
			}
			finally {
				Cursor=Cursors.Default;
			}
			labelMonthlyCost.Text=Lan.g(this,"Your current agreement includes ") + thinfinityLicenseUsers + Lan.g(this," concurrent sessions.");
			textCurrentSessions.Text=PrefC.GetString(PrefName.CloudSessionLimit);
			validNumNewSessions.Text=PrefC.GetString(PrefName.CloudSessionLimit);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!validNumNewSessions.IsValid()) {
				MsgBox.Show(Lan.g(this,"Please input a valid number."));
				return;
			}
			//Less than one session
			if(PIn.Int(validNumNewSessions.Text) <= 0) {
				MsgBox.Show(Lan.g(this,"You cannot have less than 1 session."));
				return;
			}
			//Check against hidden pref
			int cloudSessionCap=PIn.Int(GetHiddenPrefString(PrefName.CloudSessionLimitCap));
			if(cloudSessionCap>0 && PIn.Int(validNumNewSessions.Text) > cloudSessionCap) {
				MsgBox.Show(Lan.g(this,"Please contact support to increase the concurrent sessions above")+" "+cloudSessionCap+".");
				return;
			}
			//No change
			if(textCurrentSessions.Text==validNumNewSessions.Text) {
				DialogResult=DialogResult.OK;
				return;
			}
			//Confirmation
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g(this,"Do you want to set your number of concurrent sessions to")+" "+validNumNewSessions.Text+"?")) {
				return;
			}
			//Send data
			string officeData=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(PIn.Int(validNumNewSessions.Text),"MaxSessions"),eServiceCode.Undefined);
			string result;
			Cursor=Cursors.WaitCursor;
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().UpsertCloudMaxSessions(officeData);
				PayloadHelper.CheckForError(result);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MsgBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,WebSerializer.DeserializeTag<string>(result,"Response"));
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Maximum number of allowed Cloud sessions has been updated to "+validNumNewSessions.Text);
			if(Prefs.UpdateString(PrefName.CloudSessionLimit,validNumNewSessions.Text)) {
				Signalods.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		///<summary>Returns the ValueString of a pref or a blank string if that pref is not found in the database.</summary>
		private string GetHiddenPrefString(PrefName prefName) {
			Pref prefHidden;
			try {
				prefHidden=Prefs.GetOne(prefName);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "";
			}
			return prefHidden.ValueString;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
