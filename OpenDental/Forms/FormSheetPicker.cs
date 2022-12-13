using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetPicker:FormODBase {
		public SheetTypeEnum SheetType;
		///<summary>When set before opening the form, this list will be used to display to the user instead of querying for sheets of the same sheet type.</summary>
		public List<SheetDef> ListSheets;
		///<summary>Only if OK.</summary>
		public List<SheetDef> SelectedSheetDefs;
		//private bool showingInternalSheetDefs;
		//private bool showingInternalMed;
		///<summary>Stores the indices of the sheetDefs already added to SelectedSheetDefs.  Prevents adding the same one twice.  Only used with terminal.</summary>
		private List<int> _listIndexesAdded;
		///<summary>On closing, this will be true if the ToTerminal button was used and if the selected sheets should be sent to a terminal.</summary>
		public bool TerminalSend;
		///<summary>It will always be hidden anyway if sheetType is not PatientForm.  So this is only useful if it is a PatientForm and you also want to hide the button.</summary>
		public bool HideKioskButton;
		public bool AllowMultiSelect;
		/// <summary>Only true when opened by sheet Pre-Fill. In this case, we have already filled ListSheets, and dont want additional sheetDefs added.</summary>
		public bool IsPreFill=false;
		/// <summary>List of sheet def nums that will be excluded from the queried list on load.</summary>
		public List<long> ListSheetDefsExclude;
		public bool IsWebForm;

		public FormSheetPicker(bool isWebForm = false) {
			InitializeComponent();
			InitializeLayoutManager(); 
			Lan.F(this);
			_listIndexesAdded=new List<int>();
			IsWebForm = isWebForm;
		}

		private void FormSheetPicker_Load(object sender,EventArgs e) {
			if(AllowMultiSelect) {
				listMain.SelectionMode=UI.SelectionMode.MultiExtended;
			}
			if(ListSheets.IsNullOrEmpty()) {
				ListSheets=SheetDefs.GetCustomForType(SheetType);
			}
			if(!ListSheetDefsExclude.IsNullOrEmpty()) {
				ListSheets.RemoveAll(x=>ListSheetDefsExclude.Contains(x.SheetDefNum));
			}
			if(ListSheets.Count==0 && SheetType==SheetTypeEnum.PatientForm) {
				//showingInternalSheetDefs=true;
				ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.PatientRegistration));
				ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.FinancialAgreement));
				ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.HIPAA));
				ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.COVID19));
				//Medical History and Consent forms happen below.
			}
			//If we are pre-filling, the sheetDefs we are interested in should already be in ListSheets and we don't need to get any more.
			if(!IsPreFill && SheetType==SheetTypeEnum.PatientForm) {//we will also show medical history, and possibly consent forms
				List<SheetDef> listMedSheets=SheetDefs.GetCustomForType(SheetTypeEnum.MedicalHistory);
				List<SheetDef> listConSheets=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
				if(listMedSheets.Count==0) {
					//showingInternalMed=true;
					ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.MedicalHistSimple));
				}
				else {//if user has added any of their own medical history forms
					for(int i=0;i<listMedSheets.Count;i++) {
						ListSheets.Add(listMedSheets[i]);
					}
				}
				labelSheetType.Text=Lan.g(this,"Patient Forms and Medical Histories");//Change name?
				if(!IsWebForm && PrefC.GetBool(PrefName.PatientFormsShowConsent)) {//only if they want to see consent forms with patient forms.
					if(listConSheets.Count==0) {//use internal consent forms
						ListSheets.Add(SheetsInternal.GetSheetDef(SheetInternalType.Consent));
					}
					else {//if user has added any of their own consent forms
						for(int i=0;i<listConSheets.Count;i++) {
							ListSheets.Add(listConSheets[i]);
						}
					}
					labelSheetType.Text=Lan.g(this,"Patient, Consent, and Medical History Forms");
				}
			}
			else {
				labelSheetType.Text=Lan.g("enumSheetTypeEnum",SheetType.ToString());
				butTerminal.Visible=false;
				labelTerminal.Visible=false;
			}
			if(HideKioskButton){
				butTerminal.Visible=false;
				labelTerminal.Visible=false;
			}
			listMain.Items.AddList(ListSheets,x => x.Description);
			if(AllowMultiSelect && SelectedSheetDefs!=null) {
				List<int> listSelectedIndexs=ListSheets.Where(x=>SelectedSheetDefs.Any(y=>y.SheetDefNum==x.SheetDefNum)).Select(x=>ListSheets.FindIndex(y=>y.SheetDefNum==x.SheetDefNum)).ToList();
				listMain.SelectedIndices=listSelectedIndexs;
			}
		}

		private void listMain_DoubleClick(object sender,EventArgs e) {
			if(listMain.SelectedIndices.Count!=1) {
				return;
			}
			SelectedSheetDefs=new List<SheetDef>();
			SheetDef sheetDef=ListSheets[listMain.SelectedIndices[0]];
			if(sheetDef.SheetDefNum!=0) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			/*
			if(sheetDef.SheetType==SheetTypeEnum.PatientForm && !showingInternalSheetDefs) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			if(sheetDef.SheetType==SheetTypeEnum.MedicalHistory && !showingInternalMed) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}*/
			SelectedSheetDefs.Add(sheetDef);
			TerminalSend=false;
			DialogResult=DialogResult.OK;
		}

		private void butTerminal_Click(object sender,EventArgs e) {
			//only visible when used from patient forms window.
			if(listMain.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			if(SelectedSheetDefs==null) {
				SelectedSheetDefs=new List<SheetDef>();
			}
			SheetDef sheetDef;
			for(int i=0;i<listMain.SelectedIndices.Count;i++) {
				//test to make sure this sheetDef was not already added
				if(_listIndexesAdded.Contains(listMain.SelectedIndices[i])){
					continue;
				}
				_listIndexesAdded.Add(listMain.SelectedIndices[i]);
				sheetDef=ListSheets[listMain.SelectedIndices[i]];
				if(sheetDef.SheetDefNum>0) {
					SheetDefs.GetFieldsAndParameters(sheetDef);
				}
				SelectedSheetDefs.Add(sheetDef);
			}
			TerminalSend=true;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!AllowMultiSelect && listMain.SelectedIndices.Count!=1){
				MsgBox.Show(this,"Please select one item first.");
				return;
			}
			SelectedSheetDefs=new List<SheetDef>();
			/*
			if(sheetDef.SheetType==SheetTypeEnum.PatientForm && !showingInternalSheetDefs) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			if(sheetDef.SheetType==SheetTypeEnum.MedicalHistory && !showingInternalMed) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}*/
			if(AllowMultiSelect) {
				SelectedSheetDefs=listMain.GetListSelected<SheetDef>();
				SelectedSheetDefs.Where(x=>x.SheetDefNum!=0).ForEach(x=>SheetDefs.GetFieldsAndParameters(x));
			}
			else {
				SheetDef sheetDef=ListSheets[listMain.SelectedIndices[0]];
				if(sheetDef.SheetDefNum!=0) {
					SheetDefs.GetFieldsAndParameters(sheetDef);
				}
				SelectedSheetDefs.Add(sheetDef);
			}
			TerminalSend=false;
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(SelectedSheetDefs==null || SelectedSheetDefs.Count==0 || AllowMultiSelect) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			//TerminalSend will have already been set true in this case.
			DialogResult=DialogResult.OK;
		}

		

		

		

		


	}
}