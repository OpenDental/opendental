using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAsapSetup:FormODBase {
		public FormAsapSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAsapSetup_Load(object sender,EventArgs e) {
			comboClinic.ClinicNumSelected=Clinics.ClinicNum;
			FillPrefs();
		}

		private void FillPrefs() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Type"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,""),250);//Random tidbits regarding the template
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Template"),500);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			checkUseDefaults.Checked=true;
			string baseVars=Lan.g(this,"Available variables:")+" [NameF], [Date], [Time], [OfficeName], [OfficePhone]";
			GridRow row;
			row=BuildRowForTemplate(PrefName.ASAPTextTemplate,"Text manual",baseVars);
			gridMain.ListGridRows.Add(row);
			row=BuildRowForTemplate(PrefName.WebSchedAsapTextTemplate,"Web Sched Text",baseVars+", [AsapURL]");
			gridMain.ListGridRows.Add(row);
			row=BuildRowForTemplate(PrefName.WebSchedAsapEmailTemplate,"Web Sched Email Body",baseVars+", [AsapURL]");
			gridMain.ListGridRows.Add(row);
			row=BuildRowForTemplate(PrefName.WebSchedAsapEmailSubj,"Web Sched Email Subject",baseVars);
			gridMain.ListGridRows.Add(row);
			gridMain.EndUpdate();
			if(comboClinic.ClinicNumSelected==0) {//Default Clinic selected
				textWebSchedPerDay.Text=PrefC.GetString(PrefName.WebSchedAsapTextLimit);
				checkAsapPromptEnabled.Checked=PrefC.GetBool(PrefName.AsapPromptEnabled);
				checkUseDefaults.Checked=false;
				return;
			}
			ClinicPref clinicPref=ClinicPrefs.GetPref(PrefName.WebSchedAsapTextLimit,comboClinic.ClinicNumSelected);
			if(clinicPref==null || clinicPref.ValueString==null) {
				textWebSchedPerDay.Text=PrefC.GetString(PrefName.WebSchedAsapTextLimit);
			}
			else {
				textWebSchedPerDay.Text=clinicPref.ValueString;
				checkUseDefaults.Checked=false;
			}
			clinicPref=ClinicPrefs.GetPref(PrefName.AsapPromptEnabled,comboClinic.ClinicNumSelected);
			if(clinicPref==null || clinicPref.ValueString==null) {
				checkAsapPromptEnabled.Checked=PrefC.GetBool(PrefName.AsapPromptEnabled);
			}
			else {
				checkAsapPromptEnabled.Checked=PIn.Bool(clinicPref.ValueString);
				checkUseDefaults.Checked=false;
			}
		}

		///<summary>Creates a row for the passed in template type. Unchecks checkUseDefaults if a clinic-level template is in use.</summary>
		private GridRow BuildRowForTemplate(PrefName prefName,string templateName,string availableVars) {
			string templateText;
			bool doShowDefault=false;
			if(comboClinic.ClinicNumSelected==0) {
				templateText=PrefC.GetString(prefName);
				checkUseDefaults.Checked=false;
			}
			else {
				ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,comboClinic.ClinicNumSelected);
				if(clinicPref==null || clinicPref.ValueString==null) {
					templateText=PrefC.GetString(prefName);
					doShowDefault=true;
				}
				else {
					templateText=clinicPref.ValueString;
					checkUseDefaults.Checked=false;
				}
			}
			GridRow row=new GridRow();
			row.Cells.Add(Lan.g(this,templateName)+(doShowDefault ? " "+Lan.g(this,"(Default)") : ""));
			row.Cells.Add(availableVars);
			row.Cells.Add(templateText);
			row.Tag=prefName;
			return row;
		}

		private void comboClinic_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinic.ClinicNumSelected==0) {
				checkUseDefaults.Visible=false;
			}
			else {
				checkUseDefaults.Visible=true;
			}
			FillPrefs();
		}

		private void checkUseDefaults_Click(object sender,EventArgs e) {
			List<PrefName> listPrefNames=new List<PrefName> {
				PrefName.ASAPTextTemplate,
				PrefName.WebSchedAsapTextTemplate,
				PrefName.WebSchedAsapEmailTemplate,
				PrefName.WebSchedAsapEmailSubj,
				PrefName.WebSchedAsapTextLimit,
				PrefName.AsapPromptEnabled,
			};
			if(checkUseDefaults.Checked) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete custom templates for this clinic and switch to using defaults? This cannot be undone.")) {
					ClinicPrefs.DeletePrefs(comboClinic.ClinicNumSelected,listPrefNames);
					DataValid.SetInvalid(InvalidType.ClinicPrefs);
				}
				else {
					checkUseDefaults.Checked=false;
				}
			}
			else {//Was checked, now user is unchecking it.
				bool wasChanged=false;
				for(int i=0;i<listPrefNames.Count;i++) {
					if(ClinicPrefs.Upsert(listPrefNames[i],comboClinic.ClinicNumSelected,PrefC.GetString(listPrefNames[i]))) {
						wasChanged=true;
					}
				}
				if(wasChanged) {
					DataValid.SetInvalid(InvalidType.ClinicPrefs);
				}
			}
			FillPrefs();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PrefName prefName=(PrefName)gridMain.ListGridRows[e.Row].Tag;
			string curPrefValue=GetClinicPrefValue(prefName);
			EmailType emailType=PIn.Enum<EmailType>(GetClinicPrefValue(PrefName.WebSchedAsapEmailTemplateType));
			string newPrefValue;
			bool isHtmlTemplate=prefName==PrefName.WebSchedAsapEmailTemplate;
			if(isHtmlTemplate) {
				using FormEmailEdit formEmailEdit=new FormEmailEdit {
					MarkupText=curPrefValue,
					DoCheckForDisclaimer=true,
					IsRawAllowed=true,
					IsRaw=emailType==EmailType.RawHtml,
				};
				formEmailEdit.ShowDialog();
				if(formEmailEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				emailType=EmailType.Html;
				if(formEmailEdit.IsRaw) {
					emailType=EmailType.RawHtml;
				}
				newPrefValue=formEmailEdit.MarkupText;
			}
			else {
				FrmRecallMessageEdit frmRecallMessageEdit=new FrmRecallMessageEdit(prefName);
				frmRecallMessageEdit.MessageVal=curPrefValue;
				frmRecallMessageEdit.ShowDialog();
				if(!frmRecallMessageEdit.IsDialogOK) {
					return;
				}
				newPrefValue=frmRecallMessageEdit.MessageVal;
			}
			if(comboClinic.ClinicNumSelected==0) {
				if(Prefs.UpdateString(prefName,newPrefValue) | Prefs.UpdateInt(PrefName.WebSchedAsapEmailTemplateType,(int)emailType)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				if(ClinicPrefs.Upsert(prefName,comboClinic.ClinicNumSelected,newPrefValue) 
					| ClinicPrefs.Upsert(PrefName.WebSchedAsapEmailTemplate,comboClinic.ClinicNumSelected,((int)emailType).ToString())) 
				{
					DataValid.SetInvalid(InvalidType.ClinicPrefs);
				}
			}
			FillPrefs();
		}

		private string GetClinicPrefValue(PrefName prefName) {
			if(comboClinic.ClinicNumSelected==0) {
				return PrefC.GetString(prefName);
			}
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,comboClinic.ClinicNumSelected);
			if(clinicPref==null || string.IsNullOrEmpty(clinicPref.ValueString)) {
				return PrefC.GetString(prefName);
			}
			return clinicPref.ValueString;
		}

		private void textWebSchedPerDay_Leave(object sender,EventArgs e) {
			if(!textWebSchedPerDay.IsValid()) {
				return;
			}
			if(comboClinic.ClinicNumSelected==0) {
				if(Prefs.UpdateString(PrefName.WebSchedAsapTextLimit,textWebSchedPerDay.Text)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				if(ClinicPrefs.Upsert(PrefName.WebSchedAsapTextLimit,comboClinic.ClinicNumSelected,textWebSchedPerDay.Text)) {
					DataValid.SetInvalid(InvalidType.ClinicPrefs);
				}
			}
		}

		private void checkAsapPromptEnabled_Click(object sender,EventArgs e) {
			if(comboClinic.ClinicNumSelected==0) {
				if(Prefs.UpdateBool(PrefName.AsapPromptEnabled,checkAsapPromptEnabled.Checked)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				if(ClinicPrefs.Upsert(PrefName.AsapPromptEnabled,comboClinic.ClinicNumSelected,POut.Bool(checkAsapPromptEnabled.Checked))) {
					DataValid.SetInvalid(InvalidType.ClinicPrefs);
				}
			}
		}

	}
}