using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;

namespace OpenDental {
	public partial class FormEServicesAutoMsgingPreferences:FormODBase {
		private List<ClinicPref> _listClinicPrefs=new List<ClinicPref>();
		private string _webSheetIdDefaults;
		private bool _under18SendToGuarantorDefault;

		public FormEServicesAutoMsgingPreferences() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesAutoMsgingPreferences_Load(object sender,EventArgs e) {
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,suppressMessage:true);
			_webSheetIdDefaults=PrefC.GetString(PrefName.ApptNewPatientThankYouWebSheetDefID);
			_under18SendToGuarantorDefault=PrefC.GetBool(PrefName.AutoCommUnder18SendToGuarantor);
			LoadWebFormPrefs();
			List<PrefName> listPrefNames=new List<PrefName> { PrefName.ApptNewPatientThankYouWebSheetDefID, PrefName.AutoMsgingUseDefaultPref, PrefName.AutoCommUnder18SendToGuarantor };
			_listClinicPrefs=ClinicPrefs.GetWhere(x => listPrefNames.Contains(x.PrefName));
			SetClinicComboBox();
			LoadDefaultPreferences();
			checkUseDefaultPrefs.Enabled=allowEdit;
			butOK.Enabled=allowEdit;
			EnablePreferenceControls(allowEdit);
		}

		private void SetClinicComboBox() {
			string hqDescription="Practice";
			if(PrefC.HasClinicsEnabled) {
				hqDescription="Defaults";
			}
			comboClinic.HqDescription=hqDescription;
			comboClinic.ForceShowUnassigned=true;
			comboClinic.IncludeAll=false;
			comboClinic.SelectedClinicNum=0;
			checkUseDefaultPrefs.Visible=false;
		}

		private void EnablePreferenceControls(bool allowEdit) {
			checkSendToGuarantorForMinors.Enabled=allowEdit;
			groupNewPat.Enabled=allowEdit;
		}

		private Clinic GetSelectedClinic() {
			return comboClinic.GetSelectedClinic();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			bool isDefaultClinic=GetSelectedClinic().ClinicNum==0;
			checkUseDefaultPrefs.Visible=!isDefaultClinic;
			checkUseDefaultPrefs.Checked=!isDefaultClinic && SetUseDefaultCheckbox(PrefName.AutoMsgingUseDefaultPref);
			if(isDefaultClinic || checkUseDefaultPrefs.Checked) {
				LoadDefaultPreferences();
			}
			else {
				LoadClinicPreferences();
			}
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,suppressMessage:true) && (isDefaultClinic || !checkUseDefaultPrefs.Checked);
			EnablePreferenceControls(allowEdit);
		}

		private string ValidateChanges() {
			StringBuilder stringBuilderError=new StringBuilder();
			List<string> listWebSheetDefIDs=_webSheetIdDefaults.Split(',').ToList();
			if(listWebSheetDefIDs.Any(x => x=="0") && listWebSheetDefIDs.Any(x => x!="0")) {
				stringBuilderError.AppendLine("Default Web Forms preference cannot contain 'None' when other forms are selected.");
			}
			for(int i=0;i<_listClinicPrefs.Count;i++) {
				if(_listClinicPrefs[i].ClinicNum==0) {
					continue;
				}
				listWebSheetDefIDs=_listClinicPrefs[i].ValueString.Split(',').ToList();
				if(listWebSheetDefIDs.Any(x => x=="0") && listWebSheetDefIDs.Any(x => x!="0")) {
					stringBuilderError.AppendLine(Clinics.GetAbbr(_listClinicPrefs[i].ClinicNum)+" Web Forms preference cannot contain 'None' when other forms are selected.");
				}
			}
			string error="";
			if(stringBuilderError.Length>0) {
				error="Please fix the following errors:\r\n"+stringBuilderError.ToString();
			}
			return error;
		}

		private void SaveToDb() {
			bool changedPrefs=false;
			changedPrefs|=Prefs.UpdateString(PrefName.ApptNewPatientThankYouWebSheetDefID,_webSheetIdDefaults);
			changedPrefs|=Prefs.UpdateBool(PrefName.AutoCommUnder18SendToGuarantor,checkSendToGuarantorForMinors.Checked);
			bool changedClinicPrefs=changedPrefs;
			for(int i=0;i<_listClinicPrefs.Count;i++) {
				changedClinicPrefs|=ClinicPrefs.Upsert(_listClinicPrefs[i].PrefName,_listClinicPrefs[i].ClinicNum,_listClinicPrefs[i].ValueString);
			}
			if(changedPrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(changedClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private bool SetUseDefaultCheckbox(PrefName prefName) {
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.PrefName==prefName && x.ClinicNum==GetSelectedClinic().ClinicNum);
			return clinicPref!=null && PIn.Bool(clinicPref.ValueString);
		}

		private void LoadWebFormPrefs() {
			listBoxWebForms.Items.Clear();
			List<WebForms_SheetDef> listSheetDefs=new List<WebForms_SheetDef>();
			listBoxWebForms.Items.Add("None",new WebForms_SheetDef { WebSheetDefID=0 });
			if(!WebForms_SheetDefs.TryDownloadSheetDefs(out listSheetDefs)) {
				MsgBox.Show(this,"Failed to download sheet definitions.");
				return;
			}
			listBoxWebForms.Items.AddList(listSheetDefs,(x)=>x.Description);
		}

		private void LoadDefaultPreferences() {
			List<long> listWebSheetDefIds=_webSheetIdDefaults.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
			if(listWebSheetDefIds.IsNullOrEmpty()) {
				listBoxWebForms.SetSelected(0);
				return;
			}
			listBoxWebForms.SelectedIndices.Clear();
			List<int> listSelectedIndicies=new List<int>();
			for(int i=0;i<listBoxWebForms.Items.Count;i++) {
				if(listWebSheetDefIds.Contains(((WebForms_SheetDef)listBoxWebForms.Items.GetObjectAt(i)).WebSheetDefID)) {
					listSelectedIndicies.Add(i);
				}
			}
			listBoxWebForms.SelectedIndices=listSelectedIndicies;
			checkSendToGuarantorForMinors.Checked=_under18SendToGuarantorDefault;
		}

		private void LoadClinicPreferences() {
			Clinic clinic=GetSelectedClinic();
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.PrefName==PrefName.ApptNewPatientThankYouWebSheetDefID && x.ClinicNum==clinic.ClinicNum);
			List<long> listWebSheetDefIds=new List<long>();
			if(clinicPref!=null) {
				listWebSheetDefIds=clinicPref.ValueString.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x,false)).ToList();
				//If non-zero entries exist, remove all 0 entries.
				if(listWebSheetDefIds.Any(x => x==0)) {
					listWebSheetDefIds=listWebSheetDefIds.FindAll(x => x!=0);
				}
			}
			else {
				clinicPref=new ClinicPref(clinic.ClinicNum,PrefName.ApptNewPatientThankYouWebSheetDefID,"0");
				_listClinicPrefs.Add(clinicPref);
				listWebSheetDefIds.Add(0);
			}
			List<int> listSelectedIndicies=new List<int>();
			for(int i=0;i<listBoxWebForms.Items.Count;i++) {
				if(listWebSheetDefIds.Contains(((WebForms_SheetDef)listBoxWebForms.Items.GetObjectAt(i)).WebSheetDefID)) {
					listSelectedIndicies.Add(i);
				}
			}
			listBoxWebForms.SelectedIndices=listSelectedIndicies;
			clinicPref=_listClinicPrefs.FirstOrDefault(x => x.PrefName==PrefName.AutoCommUnder18SendToGuarantor && x.ClinicNum==clinic.ClinicNum);
			bool doSendToGuarantor=false;
			if(clinicPref!=null) {
				doSendToGuarantor=PIn.Bool(clinicPref.ValueString);
			}
			checkSendToGuarantorForMinors.Checked=doSendToGuarantor;
		}

		private void checkUseDefaultPrefs_Click(object sender,EventArgs e) {
			Clinic clinic=GetSelectedClinic();
			if(clinic.ClinicNum==0) {
				return;//Clinic 0, we don't need to do anything here.
			}
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinic.ClinicNum && x.PrefName==PrefName.AutoMsgingUseDefaultPref);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(clinic.ClinicNum,PrefName.AutoMsgingUseDefaultPref,valueBool:false);
				_listClinicPrefs.Add(clinicPref);
			}
			//TURNING DEFAULTS OFF
			if(!checkUseDefaultPrefs.Checked && PIn.Bool(clinicPref.ValueString)) {//Default switched off
				clinicPref.ValueString=POut.Bool(false);
				LoadClinicPreferences();
			}
			//TURNING DEFAULTS ON
			else if(checkUseDefaultPrefs.Checked && !PIn.Bool(clinicPref.ValueString)) {//Default switched on
				clinicPref.ValueString=POut.Bool(true);
				LoadDefaultPreferences();
			}
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,suppressMessage:true) && !checkUseDefaultPrefs.Checked;
			EnablePreferenceControls(allowEdit);
		}

		private void checkSendToGuarantorForMinors_Click(object sender,EventArgs e) {
			Clinic clinic=GetSelectedClinic();
			if(clinic.ClinicNum==0) {
				_under18SendToGuarantorDefault=checkSendToGuarantorForMinors.Checked;
				return;
			}
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinic.ClinicNum && x.PrefName==PrefName.AutoCommUnder18SendToGuarantor);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(clinic.ClinicNum,PrefName.AutoCommUnder18SendToGuarantor,valueBool:checkSendToGuarantorForMinors.Checked);
				_listClinicPrefs.Add(clinicPref);
				return;
			}
			clinicPref.ValueString=POut.Bool(checkSendToGuarantorForMinors.Checked);
		}

		private void listBoxWebForm_SelectionChangeCommitted(object sender,EventArgs e) {
			Clinic clinic=GetSelectedClinic();
			string webSheetDefIDs=string.Join(",",listBoxWebForms.GetListSelected<WebForms_SheetDef>().Select(x => x.WebSheetDefID));
			if(clinic.ClinicNum==0) {
				_webSheetIdDefaults=webSheetDefIDs;
				return;
			}
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinic.ClinicNum && x.PrefName==PrefName.ApptNewPatientThankYouWebSheetDefID);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(clinic.ClinicNum,PrefName.ApptNewPatientThankYouWebSheetDefID,valueString:POut.String(webSheetDefIDs));
				_listClinicPrefs.Add(clinicPref);
				return;
			}
			clinicPref.ValueString=POut.String(webSheetDefIDs);
		}

		private void butOK_Click(object sender,EventArgs e) {
			string error=ValidateChanges();
			if(!string.IsNullOrWhiteSpace(error)) {
				MessageBox.Show(Lan.g(this,error));
				return;
			}
			SaveToDb();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}