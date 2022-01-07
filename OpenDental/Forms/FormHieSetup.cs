using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormHieSetup:FormODBase {
		private long _previouslySelectedClinicNum;
		private List<HieClinic> _listHieClinics;

		public FormHieSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHieSetup_Load(object sender,EventArgs e) {
			_listHieClinics=HieClinics.Refresh();
			AddNeededHieClinic(0);
			if(PrefC.HasClinicsEnabled) {
				comboClinic.SelectedClinicNum=0;
				_previouslySelectedClinicNum=comboClinic.SelectedClinicNum;
			}
			FillUIFromFields();
		}

		private void AddNeededHieClinic(long clinicNum) {
			if(!_listHieClinics.Any(x => x.ClinicNum==clinicNum)) {
				HieClinic hieClinicHQ=_listHieClinics.FirstOrDefault(x => x.ClinicNum==0);
				if(hieClinicHQ==null) {
					//Default HQ Time of day export to 9pm and enabled
					hieClinicHQ=new HieClinic(clinicNum,TimeSpan.FromHours(21),isEnabled:true);
				}
				//Default new HIE clinics to the HQ values.
				_listHieClinics.Add(new HieClinic(clinicNum,hieClinicHQ.TimeOfDayExportCCD,
					isEnabled:true,carrierFlags:hieClinicHQ.SupportedCarrierFlags,pathExportCCD:hieClinicHQ.PathExportCCD));
			}
		}

		private void FillUIFromFields() {
			HieClinic hieClinicCur=_listHieClinics.FirstOrDefault(x => x.ClinicNum==comboClinic.SelectedClinicNum);
			checkEnabled.Checked=hieClinicCur.IsEnabled;
			checkMedicaidOnly.Checked=hieClinicCur.SupportedCarrierFlags.HasFlag(HieCarrierFlags.Medicaid);
			textExportPath.Text=hieClinicCur.PathExportCCD;
			dateTimeOfExportCCD.Value=DateTime.Today+hieClinicCur.TimeOfDayExportCCD;
		}

		///<summary>Updates the hieclinic values with the values on the form. Validation should happen before calling this method.</summary>
		private void FillFieldsFromUI(long clinicNum) {
			_listHieClinics.FindAll(x => x.ClinicNum==clinicNum)
				.ForEach(x => x.IsEnabled=checkEnabled.Checked);
			_listHieClinics.FindAll(x => x.ClinicNum==clinicNum)
				.ForEach(x => x.PathExportCCD=textExportPath.Text); ;
			_listHieClinics.FindAll(x => x.ClinicNum==clinicNum)
				.ForEach(x => x.TimeOfDayExportCCD=dateTimeOfExportCCD.Value.TimeOfDay);
			HieCarrierFlags hieCarrierFlagSelected=HieCarrierFlags.AllCarriers;
			if(checkMedicaidOnly.Checked) {
				hieCarrierFlagSelected|=HieCarrierFlags.Medicaid;
			}
			_listHieClinics.FindAll(x => x.ClinicNum==clinicNum)
				.ForEach(x => x.SupportedCarrierFlags=hieCarrierFlagSelected);
		}

		private bool IsValid() {
			if(!checkEnabled.Checked) {//Disabled
				return true;
			}
			//Hieclinic is enabled
			if(string.IsNullOrEmpty(textExportPath.Text)) {
				MsgBox.Show(this,$"{labelExportPath.Text} cannot be blank.");
				return false;
			}
			if(!Directory.Exists(textExportPath.Text)
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Export path does not exist or is inaccessible.\r\nAttempt to create?"))
			{
				try {
					Directory.CreateDirectory(textExportPath.Text);
					MsgBox.Show(this,"Folder created.");
				}
				catch {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Not able to create folder.\r\nContinue anyway?")) {
						return false;
					}
				}
			}
			return true;
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			folderBrowserDialog.SelectedPath=textExportPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.OK) {
				textExportPath.Text=folderBrowserDialog.SelectedPath;
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedClinicNum==_previouslySelectedClinicNum) {
				return;
			}
			if(!IsValid()) {
				comboClinic.SelectedClinicNum=_previouslySelectedClinicNum;
				return;
			}
			FillFieldsFromUI(_previouslySelectedClinicNum);
			_previouslySelectedClinicNum=comboClinic.SelectedClinicNum;
			AddNeededHieClinic(comboClinic.SelectedClinicNum);
			FillUIFromFields();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			FillFieldsFromUI(comboClinic.SelectedClinicNum);
			HieClinics.Sync(_listHieClinics);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}