using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmSheetPicker:FrmODBase {
		public SheetTypeEnum SheetType;
		///<summary>When set before opening the form, this list will be used to display to the user instead of querying for sheets of the same sheet type.</summary>
		public List<SheetDef> ListSheetDefs;
		///<summary>Only if OK.</summary>
		public List<SheetDef> ListSheetDefsSelected;
		//private bool showingInternalSheetDefs;
		//private bool showingInternalMed;
		///<summary>Stores the indices of the sheetDefs already added to SelectedSheetDefs.  Prevents adding the same one twice.  Only used with terminal.</summary>
		private List<int> _listIndexesAdded;
		///<summary>On closing, this will be true if the ToTerminal button was used and if the selected sheets should be sent to a terminal.</summary>
		public bool DoTerminalSend;
		///<summary>It will always be hidden anyway if sheetType is not PatientForm.  So this is only useful if it is a PatientForm and you also want to hide the button.</summary>
		public bool HideKioskButton;
		public bool AllowMultiSelect;
		/// <summary>Only true when opened by sheet Pre-Fill. In this case, we have already filled ListSheets, and dont want additional sheetDefs added.</summary>
		public bool IsPreFill=false;
		/// <summary>List of sheet def nums that will be excluded from the queried list on load.</summary>
		public List<long> ListSheetDefNumsExclude;
		public bool IsWebForm;

		public FrmSheetPicker(bool isWebForm = false):base() {
			InitializeComponent();
			_listIndexesAdded=new List<int>();
			IsWebForm = isWebForm;
			Load+=FrmSheetPicker_Load;
			listMain.MouseDoubleClick+=listMain_DoubleClick;
			PreviewKeyDown+=FrmSheetPicker_PreviewKeyDown;
			textSearch.TextChanged+=textSearch_TextChanged;
		}

		private void FrmSheetPicker_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(AllowMultiSelect) {
				listMain.SelectionMode=SelectionMode.MultiExtended;
			}
			if(ListSheetDefs.IsNullOrEmpty()) {
				ListSheetDefs=SheetDefs.GetCustomForType(SheetType);
			}
			if(!ListSheetDefNumsExclude.IsNullOrEmpty()) {
				ListSheetDefs.RemoveAll(x=>ListSheetDefNumsExclude.Contains(x.SheetDefNum));
			}
			if(ListSheetDefs.Count==0 && SheetType==SheetTypeEnum.PatientForm) {
				//showingInternalSheetDefs=true;
				ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.PatientRegistration));
				ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.FinancialAgreement));
				ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.HIPAA));
				ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.COVID19));
				//Medical History and Consent forms happen below.
			}
			//If we are pre-filling, the sheetDefs we are interested in should already be in ListSheets and we don't need to get any more.
			if(!IsPreFill && SheetType==SheetTypeEnum.PatientForm) {//we will also show medical history, and possibly consent forms
				List<SheetDef> listSheetDefsMed=SheetDefs.GetCustomForType(SheetTypeEnum.MedicalHistory);
				List<SheetDef> listSheetDefsCon=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
				if(listSheetDefsMed.Count==0) {
					//showingInternalMed=true;
					ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.MedicalHist));
				}
				else {//if user has added any of their own medical history forms
					for(int i=0;i<listSheetDefsMed.Count;i++) {
						ListSheetDefs.Add(listSheetDefsMed[i]);
					}
				}
				labelSheetType.Text=Lans.g(this,"Patient Forms and Medical Histories");//Change name?
				if(!IsWebForm && PrefC.GetBool(PrefName.PatientFormsShowConsent)) {//only if they want to see consent forms with patient forms.
					if(listSheetDefsCon.Count==0) {//use internal consent forms
						ListSheetDefs.Add(SheetsInternal.GetSheetDef(SheetInternalType.Consent));
					}
					else {//if user has added any of their own consent forms
						for(int i=0;i<listSheetDefsCon.Count;i++) {
							ListSheetDefs.Add(listSheetDefsCon[i]);
						}
					}
					labelSheetType.Text=Lans.g(this,"Patient, Consent, and Medical History Forms");
				}
			}
			else {
				labelSheetType.Text=Lans.g("enumSheetTypeEnum",SheetType.ToString());
				butTerminal.Visible=false;
				labelTerminal.Visible=false;
			}
			if(HideKioskButton){
				butTerminal.Visible=false;
				labelTerminal.Visible=false;
			}
			if(SheetType==SheetTypeEnum.PatientForm) {
				ListSheetDefs=ListSheetDefs.OrderBy(x => x.Description).ToList();
			}
			FillListBox();
		}

		private void listMain_DoubleClick(object sender,MouseButtonEventArgs e) {
			if(listMain.Items.Count<=0 || listMain.SelectedIndices.Count!=1) {
				return;
			}
			ListSheetDefsSelected=new List<SheetDef>();
			SheetDef sheetDef=(SheetDef)listMain.Items.GetObjectAt(listMain.SelectedIndices[0]);
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
			ListSheetDefsSelected.Add(sheetDef);
			DoTerminalSend=false;
			IsDialogOK=true;
		}

		private void butTerminal_Click(object sender,EventArgs e) {
			//only visible when used from patient forms window.
			if(listMain.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			if(ListSheetDefsSelected==null) {
				ListSheetDefsSelected=new List<SheetDef>();
			}
			SheetDef sheetDef;
			for(int i=0;i<listMain.SelectedIndices.Count;i++) {
				//test to make sure this sheetDef was not already added
				if(_listIndexesAdded.Contains(listMain.SelectedIndices[i])){
					continue;
				}
				_listIndexesAdded.Add(listMain.SelectedIndices[i]);
				sheetDef=(SheetDef)listMain.Items.GetObjectAt(listMain.SelectedIndices[i]);
				if(sheetDef.SheetDefNum>0) {
					SheetDefs.GetFieldsAndParameters(sheetDef);
				}
				ListSheetDefsSelected.Add(sheetDef);
			}
			DoTerminalSend=true;
			IsDialogOK=true;
		}

		private void FrmSheetPicker_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void FillListBox(){
			List<long> listSheetDefNumsSelected=listMain.GetListSelected<SheetDef>().Select(x => x.SheetDefNum).ToList();
			listMain.Items.Clear();
			List<SheetDef> listSheetDefs=ListSheetDefs.FindAll(x => x.Description.ToLower().Contains(textSearch.Text.ToLower().Trim()));
			listMain.Items.AddList(listSheetDefs,x => x.Description);
			//reselect
			for(int i=0;i<listMain.Items.Count;i++){
				if(listSheetDefNumsSelected.Contains(((SheetDef)listMain.Items.GetObjectAt(i)).SheetDefNum)){
					listMain.SetSelected(i);
				}
			}
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillListBox();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listMain.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select one item first.");
				return;
			}
			ListSheetDefsSelected=new List<SheetDef>();
			/*
			if(sheetDef.SheetType==SheetTypeEnum.PatientForm && !showingInternalSheetDefs) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			if(sheetDef.SheetType==SheetTypeEnum.MedicalHistory && !showingInternalMed) {
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}*/
			if(AllowMultiSelect) {
				ListSheetDefsSelected=listMain.GetListSelected<SheetDef>();
				for(int i=0;i<ListSheetDefsSelected.Count;i++){
					if(ListSheetDefsSelected[i].SheetDefNum!=0){
						SheetDefs.GetFieldsAndParameters(ListSheetDefsSelected[i]);
					}
				}
			}
			else {
				SheetDef sheetDef=(SheetDef)listMain.Items.GetObjectAt(listMain.SelectedIndices[0]);
				if(sheetDef.SheetDefNum!=0) {
					SheetDefs.GetFieldsAndParameters(sheetDef);
				}
				ListSheetDefsSelected.Add(sheetDef);
			}
			DoTerminalSend=false;
			IsDialogOK=true;
		}

	}
}