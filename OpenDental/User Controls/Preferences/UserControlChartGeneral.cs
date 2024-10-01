using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlChartGeneral:UserControl {
		
		#region Fields - Private
		///<summary>Helps store and set the Prefs for desease, medications, and alergies in the Chart Module when clicking OK on those forms.</summary>
		private long _diseaseDefNum;//DiseaseDef
		private long _medicationNum;
		private long _alergyDefNum;//AllergyDef
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlChartGeneral() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butAllergiesIndicateNone_Click(object sender,EventArgs e) {
			using FormAllergySetup formA=new FormAllergySetup();
			formA.IsSelectionMode=true;
			formA.ShowDialog();
			if(formA.DialogResult!=DialogResult.OK) {
				return;
			}
			_alergyDefNum=formA.AllergyDefNumSelected;
			textAllergiesIndicateNone.Text=AllergyDefs.GetOne(formA.AllergyDefNumSelected).Description;
		}

		private void butDiagnosisCode_Click(object sender,EventArgs e) {
			if(checkDxIcdVersion.Checked) {//ICD-10
				using FormIcd10s formI=new FormIcd10s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textICD9DefaultForNewProcs.Text=formI.Icd10Selected.Icd10Code;
				}
			}
			else {//ICD-9
				using FormIcd9s formI=new FormIcd9s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textICD9DefaultForNewProcs.Text=formI.ICD9Selected.ICD9Code;
				}
			}
		}

		private void butMedicationsIndicateNone_Click(object sender,EventArgs e) {
			using FormMedications formM=new FormMedications();
			formM.IsSelectionMode=true;
			formM.ShowDialog();
			if(formM.DialogResult!=DialogResult.OK) {
				return;
			}
			_medicationNum=formM.SelectedMedicationNum;
			textMedicationsIndicateNone.Text=Medications.GetDescription(formM.SelectedMedicationNum);
		}

		private void butProblemsIndicateNone_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formD=new FormDiseaseDefs();
			formD.IsSelectionMode=true;
			formD.ShowDialog();
			if(formD.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			_diseaseDefNum=formD.ListDiseaseDefsSelected[0].DiseaseDefNum;
			textProblemsIndicateNone.Text=formD.ListDiseaseDefsSelected[0].DiseaseName;
		}

		private void checkDxIcdVersion_Click(object sender,EventArgs e) {
			SetIcdLabels();
		}

		private void linkLabelIsAlertRadiologyProcsEnabledDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			try {
				Process.Start("https://opendental.com/manual/ehrcpoeradapprove.html");
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+"https://opendental.com/manual/ehrcpoeradapprove.html"+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private void SetIcdLabels() {
			byte icdVersion=9;
			if(checkDxIcdVersion.Checked) {
				icdVersion=10;
			}
			labelIcdCodeDefault.Text=Lan.g(this,"Default ICD")+"-"+icdVersion+" "+Lan.g(this,"code for new procedures and when set complete");
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillChartGeneral() {
			comboToothNomenclature.Items.Add(Lan.g(this,"Universal (Common in the US, 1-32)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"FDI Notation (International, 11-48)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"Haderup (Danish)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"Palmer (Ortho)"));
			comboToothNomenclature.SelectedIndex = PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelToothNomenclature.Visible=false;
				comboToothNomenclature.Visible=false;
			}
			checkAutoClearEntryStatus.Checked=PrefC.GetBool(PrefName.AutoResetTPEntryStatus);
			textProblemsIndicateNone.Text=DiseaseDefs.GetName(PrefC.GetLong(PrefName.ProblemsIndicateNone)); //DB maint to fix corruption
			_diseaseDefNum=PrefC.GetLong(PrefName.ProblemsIndicateNone);
			textMedicationsIndicateNone.Text=Medications.GetDescription(PrefC.GetLong(PrefName.MedicationsIndicateNone)); //DB maint to fix corruption
			_medicationNum=PrefC.GetLong(PrefName.MedicationsIndicateNone);
			textAllergiesIndicateNone.Text=AllergyDefs.GetDescription(PrefC.GetLong(PrefName.AllergiesIndicateNone)); //DB maint to fix corruption
			_alergyDefNum=PrefC.GetLong(PrefName.AllergiesIndicateNone);
			checkChartNonPatientWarn.Checked=PrefC.GetBool(PrefName.ChartNonPatientWarn);
			checkMedicalFeeUsedForNewProcs.Checked=PrefC.GetBool(PrefName.MedicalFeeUsedForNewProcs);
			checkProvColorChart.Checked=PrefC.GetBool(PrefName.UseProviderColorsInChart);
			checkPerioSkipMissingTeeth.Checked=PrefC.GetBool(PrefName.PerioSkipMissingTeeth);
			checkPerioTreatImplantsAsNotMissing.Checked=PrefC.GetBool(PrefName.PerioTreatImplantsAsNotMissing);
			if(PrefC.GetByte(PrefName.DxIcdVersion)==9) {
				checkDxIcdVersion.Checked=false;
			}
			else {//ICD-10
				checkDxIcdVersion.Checked=true;
			}
			SetIcdLabels();
			textICD9DefaultForNewProcs.Text=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			textMedDefaultStopDays.Text=PrefC.GetString(PrefName.MedDefaultStopDays);
			checkScreeningsUseSheets.Checked=PrefC.GetBool(PrefName.ScreeningsUseSheets);
			for(int i=0;i<Enum.GetNames(typeof(ProcCodeListSort)).Length;i++) {
				comboProcCodeListSort.Items.Add(Enum.GetNames(typeof(ProcCodeListSort))[i]);
			}
			comboProcCodeListSort.SelectedIndex=PrefC.GetInt(PrefName.ProcCodeListSortOrder);
			checkBoxRxClinicUseSelected.Checked=PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected);
			if(!PrefC.HasClinicsEnabled) {
				checkBoxRxClinicUseSelected.Visible=false;
			}
			checkIsAlertRadiologyProcsEnabled.Checked=PrefC.GetBool(PrefName.IsAlertRadiologyProcsEnabled);
			checkShowPlannedApptPrompt.Checked=PrefC.GetBool(PrefName.ShowPlannedAppointmentPrompt);
			checkChartOrthoTabAutomaticCheckboxes.Checked=PrefC.GetBool(PrefName.ChartOrthoTabAutomaticCheckboxes);
		}

		public bool SaveChartGeneral() {
			int daysStop=0;
			if(!int.TryParse(textMedDefaultStopDays.Text,out daysStop)) {
				MsgBox.Show(this,"Days until medication order stop date entered was is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(daysStop<0) {
				MsgBox.Show(this,"Days until medication order stop date cannot be a negative number.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.AutoResetTPEntryStatus,checkAutoClearEntryStatus.Checked);
			Changed|=Prefs.UpdateLong(PrefName.ProblemsIndicateNone,_diseaseDefNum);
			Changed|=Prefs.UpdateLong(PrefName.MedicationsIndicateNone,_medicationNum);
			Changed|=Prefs.UpdateLong(PrefName.AllergiesIndicateNone,_alergyDefNum);
			Changed|=Prefs.UpdateLong(PrefName.UseInternationalToothNumbers,comboToothNomenclature.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.MedicalFeeUsedForNewProcs,checkMedicalFeeUsedForNewProcs.Checked);
			Changed|=Prefs.UpdateByte(PrefName.DxIcdVersion,(byte)(checkDxIcdVersion.Checked?10:9));
			Changed|=Prefs.UpdateString(PrefName.ICD9DefaultForNewProcs,textICD9DefaultForNewProcs.Text);
			Changed|=Prefs.UpdateBool(PrefName.ChartNonPatientWarn,checkChartNonPatientWarn.Checked);
			Changed|=Prefs.UpdateInt(PrefName.MedDefaultStopDays,daysStop);
			Changed|=Prefs.UpdateBool(PrefName.UseProviderColorsInChart,checkProvColorChart.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PerioSkipMissingTeeth,checkPerioSkipMissingTeeth.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PerioTreatImplantsAsNotMissing,checkPerioTreatImplantsAsNotMissing.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ScreeningsUseSheets,checkScreeningsUseSheets.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ProcCodeListSortOrder,comboProcCodeListSort.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.ElectronicRxClinicUseSelected,checkBoxRxClinicUseSelected.Checked);
			Changed|=Prefs.UpdateBool(PrefName.IsAlertRadiologyProcsEnabled,checkIsAlertRadiologyProcsEnabled.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowPlannedAppointmentPrompt,checkShowPlannedApptPrompt.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ChartOrthoTabAutomaticCheckboxes,checkChartOrthoTabAutomaticCheckboxes.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
