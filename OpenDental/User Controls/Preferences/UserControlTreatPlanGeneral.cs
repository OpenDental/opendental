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
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlTreatPlanGeneral:UserControl {

		#region Fields - Private
		private List<Def> _listDefsNegAdjTypes;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlTreatPlanGeneral() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void UserControlTreatPlanGeneral_Load(object sender,EventArgs e) {
			if(PrefC.RandomKeys) {
				groupTreatPlanSort.Visible=false;
			}
		}

		private void linkLabelProcDiscountTypeDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://www.opendental.com/manual221/treatmentplandiscounts.html");
		}

		private void linkLabelProcDiscountTypeDetails2_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://opendental.com/manual/definitionsadjtypes.html");
		}

		private void radioTreatPlanSortOrder_Click(object sender,EventArgs e) {
			//Sort by order is a false 
			if(PrefC.GetBool(PrefName.TreatPlanSortByTooth)==radioTreatPlanSortOrder.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}

		private void radioTreatPlanSortTooth_Click(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.TreatPlanSortByTooth)!=radioTreatPlanSortTooth.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>The preference passed is assumed to be comma-delimited list of procedure codes.
		///Updates and returns true if all proc codes in textProcCodes are valid. Otherwise, we add these codes to errorMessage and returns false.</summary>
		private bool UpdateProcCodesPref(PrefName prefName,string textProcCodes,string labelText,ref string errorMessage) {
			List<string> listProcCodesInvalid=new List<string>();
			List<string> listProcCodes=textProcCodes
				.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.ToList();
			for(int i=0;i<listProcCodes.Count;i++) {
				if(!ProcedureCodes.GetContainsKey(listProcCodes[i])) {
					listProcCodesInvalid.Add($"'{listProcCodes[i]}'");
				}
			}
			if(listProcCodesInvalid.Count > 0) {
				errorMessage+=$"\r\n  - {labelText}: {string.Join(",",listProcCodesInvalid)}";
				return false;
			}
			//All valid codes in text box.
			return Prefs.UpdateString(prefName,textProcCodes);
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillTreatPlanGeneral() {
			textDiscountPercentage.Text=PrefC.GetDouble(PrefName.TreatPlanDiscountPercent).ToString();
			_listDefsNegAdjTypes=Defs.GetNegativeAdjTypes();
			comboProcDiscountType.Items.AddDefs(_listDefsNegAdjTypes);
			comboProcDiscountType.SetSelectedDefNum(PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType));
			textTreatNote.Text=PrefC.GetString(PrefName.TreatmentPlanNote);
			checkTreatPlanShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkTreatPlanShowCompleted.Visible=false;
			}
			else {
				checkTreatPlanShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			}
			checkTreatPlanItemized.Checked=PrefC.GetBool(PrefName.TreatPlanItemized);
			checkTPSaveSigned.Checked=PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf);
			radioTreatPlanSortTooth.Checked=PrefC.GetBool(PrefName.TreatPlanSortByTooth) || PrefC.RandomKeys;
			groupTreatPlanSort.Enabled=!PrefC.RandomKeys;
			checkPromptSaveTP.Checked=PrefC.GetBool(PrefName.TreatPlanPromptSave);
			textInsHistBW.Text=PrefC.GetString(PrefName.InsHistBWCodes);
			textInsHistDebridement.Text=PrefC.GetString(PrefName.InsHistDebridementCodes);
			textInsHistExam.Text=PrefC.GetString(PrefName.InsHistExamCodes);
			textInsHistFMX.Text=PrefC.GetString(PrefName.InsHistPanoCodes);
			textInsHistPerioMaint.Text=PrefC.GetString(PrefName.InsHistPerioMaintCodes);
			textInsHistPerioLL.Text=PrefC.GetString(PrefName.InsHistPerioLLCodes);
			textInsHistPerioLR.Text=PrefC.GetString(PrefName.InsHistPerioLRCodes);
			textInsHistPerioUL.Text=PrefC.GetString(PrefName.InsHistPerioULCodes);
			textInsHistPerioUR.Text=PrefC.GetString(PrefName.InsHistPerioURCodes);
			textInsHistProphy.Text=PrefC.GetString(PrefName.InsHistProphyCodes);
		}

		public bool SaveTreatPlanGeneral() {
			float percent=0;
			if(!float.TryParse(textDiscountPercentage.Text,out percent)) {
				MsgBox.Show(this,"Procedure discount percent is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(PrefC.GetString(PrefName.TreatmentPlanNote)!=textTreatNote.Text) {
				List<long> listTreatPlanNums=TreatPlans.GetNumsByNote(PrefC.GetString(PrefName.TreatmentPlanNote));//Find active/inactive TP's that match exactly.
				if(listTreatPlanNums.Count>0) {
					DialogResult dr=MessageBox.Show(Lan.g(this,"Unsaved treatment plans found with default notes")+": "+listTreatPlanNums.Count+"\r\n"
						+Lan.g(this,"Would you like to change them now?"),"",MessageBoxButtons.YesNoCancel);
					switch(dr) {
						case DialogResult.Cancel:
							return false;
						case DialogResult.Yes:
						case DialogResult.OK:
							TreatPlans.UpdateNotes(textTreatNote.Text,listTreatPlanNums);//change tp notes
							break;
						default://includes "No"
							//do nothing
							break;
					}
				}
			}
			if(comboProcDiscountType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.TreatPlanDiscountAdjustmentType,comboProcDiscountType.GetSelectedDefNum());
			}
			Changed|=Prefs.UpdateString(PrefName.TreatmentPlanNote,textTreatNote.Text);
			Changed|=Prefs.UpdateBool(PrefName.TreatPlanShowCompleted,checkTreatPlanShowCompleted.Checked);
			Changed|=Prefs.UpdateDouble(PrefName.TreatPlanDiscountPercent,percent);
			Changed|=Prefs.UpdateBool(PrefName.TreatPlanItemized,checkTreatPlanItemized.Checked);
			Changed|=Prefs.UpdateBool(PrefName.TreatPlanSaveSignedToPdf,checkTPSaveSigned.Checked);
			Changed|=Prefs.UpdateBool(PrefName.TreatPlanSortByTooth,radioTreatPlanSortTooth.Checked || PrefC.RandomKeys);
			Changed|=Prefs.UpdateBool(PrefName.TreatPlanPromptSave, checkPromptSaveTP.Checked);
			string errorMessage="";
			Changed|=UpdateProcCodesPref(PrefName.InsHistBWCodes,textInsHistBW.Text,labelInsHistBW.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPanoCodes,textInsHistFMX.Text,labelInsHistFMX.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistExamCodes,textInsHistExam.Text,labelInsHistExam.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistProphyCodes,textInsHistProphy.Text,labelInsHistProphy.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistDebridementCodes,textInsHistDebridement.Text,labelInsHistDebridement.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPerioMaintCodes,textInsHistPerioMaint.Text,labelInsHistPerioMaint.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPerioLLCodes,textInsHistPerioLL.Text,labelInsHistPerioLL.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPerioLRCodes,textInsHistPerioLR.Text,labelInsHistPerioLR.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPerioULCodes,textInsHistPerioUL.Text,labelInsHistPerioUL.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsHistPerioURCodes,textInsHistPerioUR.Text,labelInsHistPerioUR.Text,ref errorMessage);
			if(!string.IsNullOrEmpty(errorMessage)) {//Keep them in the window if invalid codes found.
				if(MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Invalid Treat' Plan procedure codes were detected and should be corrected before saving. " +
					"The following codes need to be fixed:")+$"\n{errorMessage}\n\nDo you want to save anyway?")) {
					return true;
				}
				return false;
			}
			return true;
		}
		#endregion Methods - Public
	}
}
