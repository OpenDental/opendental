//FormSheetDefDefaults.cs for job 24428
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSheetDefDefaults:FormODBase {
		///<summary>Used to keep track of the previously selected clinic. When the clinic is changed, this is used to decide what clinic the sheetdefs should be updated under</summary>
		private long _previousSelectedClinicNum;

		public FormSheetDefDefaults() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetDefDefaults_Load(object sender,EventArgs e) {
			//Set the previously selected clinic to the clinic that is selected in the combo box upon load.
			_previousSelectedClinicNum=comboClinicDefault.SelectedClinicNum;
			//Independent ComboBoxODs
			FillSheetDefComboBox(comboInvoice,SheetTypeEnum.Statement,PrefName.SheetsDefaultInvoice);
			FillSheetDefComboBox(comboReceipt,SheetTypeEnum.Statement,PrefName.SheetsDefaultReceipt);
			FillSheetDefComboBox(comboLimited,SheetTypeEnum.Statement,PrefName.SheetsDefaultLimited);
			FillSheetDefComboBox(comboStatements,SheetTypeEnum.Statement,PrefName.SheetsDefaultStatement);
			//Clinic Dependent comboBoxeODs
			FillSheetDefComboBox(comboRx,SheetTypeEnum.Rx,PrefName.SheetsDefaultRx);
			FillSheetDefComboBox(comboBoxChartLayout,SheetTypeEnum.ChartModule,PrefName.SheetsDefaultChartModule);
			FillSheetDefComboBox(comboTreatmentPlan,SheetTypeEnum.TreatmentPlan,PrefName.SheetsDefaultTreatmentPlan);
		}

		///<summary>Fills the combo box passed in with all of the sheet defs available for the passed-in sheet type, and sets the selected index based on either clinic or default prefs.</summary>
		private void FillSheetDefComboBox(UI.ComboBoxOD comboBox,SheetTypeEnum sheetType, PrefName prefName) {
			//Gets the sheetDef name and prepends 'Internal' if the SheetDef is internal.
			Func<SheetDef,string> funcGetSheetDefName = x => {
				if(x.SheetDefNum==0) {
					return "Internal - "+x.Description;
				}
				return x.Description;
			};
			List<SheetDef> listSheetDefs=new List<SheetDef>();
			listSheetDefs.Add(SheetsInternal.GetSheetDef(sheetType));//Add the internal sheets.
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(sheetType));//Add custom sheets.
			comboBox.Items.Clear();
			comboBox.Items.AddList(listSheetDefs,funcGetSheetDefName);
			SelectComboBoxesDefault(comboBox,prefName);
		}

		///<summary>Calls for an update to all of the respective clinicpref and preference entries.</summary>
		private void UpdateDefaultSheets() {
			//Independent Sheets
			UpdateSheetDefDefault(comboInvoice,PrefName.SheetsDefaultInvoice,isIndependentOfClinic:true);
			UpdateSheetDefDefault(comboReceipt,PrefName.SheetsDefaultReceipt,isIndependentOfClinic:true);
			UpdateSheetDefDefault(comboLimited,PrefName.SheetsDefaultLimited,isIndependentOfClinic:true);
			UpdateSheetDefDefault(comboStatements,PrefName.SheetsDefaultStatement,isIndependentOfClinic:true);
			//Clinic Dependent Sheets
			UpdateSheetDefDefault(comboRx,PrefName.SheetsDefaultRx);
			UpdateSheetDefDefault(comboBoxChartLayout,PrefName.SheetsDefaultChartModule);
			UpdateSheetDefDefault(comboTreatmentPlan,PrefName.SheetsDefaultTreatmentPlan);
		}

		///<summary>Updates the ClinicPref.SheetDefNum for a given prefName. If clinics are disabled, or isIndependentOfClinic is set to true,
		///then the 'preference' table is updated.</summary>
		private void UpdateSheetDefDefault(UI.ComboBoxOD comboBox,PrefName prefName,bool isIndependentOfClinic=false) {
			if(comboBox.SelectedIndex==-1) {
				return;
			}
			if(comboClinicDefault.SelectedClinicNum==0 || isIndependentOfClinic) {
				Pref defaultPref=Prefs.GetPref(prefName.GetDescription());
				defaultPref.ValueString=comboBox.GetSelected<SheetDef>().SheetDefNum.ToString();
				Prefs.Update(defaultPref);
				Prefs.RefreshCache();
				return;
			}
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,comboClinicDefault.SelectedClinicNum);
			if(clinicPref==null) {//If the clinic pref hasn't been created yet.
				if(comboBox.GetSelected<SheetDef>().SheetDefNum!=PrefC.GetLong(prefName)) {//and the selected sheet def is the different from the default preference
					ClinicPrefs.Insert(new ClinicPref(comboClinicDefault.SelectedClinicNum,prefName,comboBox.GetSelected<SheetDef>().SheetDefNum.ToString()));//insert the clinic pref
				}//No 'else' needed because the else would assume the default
			}
			else {//If the clinic pref has been created
				if(comboBox.GetSelected<SheetDef>().SheetDefNum==PrefC.GetLong(prefName)) {//and the selected sheet def is the same as the default preferance
					ClinicPrefs.Delete(clinicPref.ClinicPrefNum);
				}
				else {//and the selected sheet def is different from the default preferance
					clinicPref.ValueString=comboBox.GetSelected<SheetDef>().SheetDefNum.ToString();//Set the value string to the selected SheetDefNum
					ClinicPrefs.Update(clinicPref);
				}
			}
			ClinicPrefs.RefreshCache();
		}

		///<summary>Helper to do selection on comboboxes, abstracted to work with any Combobox in the form and any SheetTypeEnum.</summary>
		private void SelectComboBoxesDefault(UI.ComboBoxOD comboBox,PrefName prefName) {
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,comboClinicDefault.SelectedClinicNum);
			if(clinicPref==null || comboClinicDefault.SelectedClinicNum==0) {
				Pref pref=Prefs.GetPref(prefName.GetDescription());
				comboBox.SetSelectedKey<SheetDef>(PIn.Long(pref.ValueString),x=>x.SheetDefNum);
			}
			else {
				comboBox.SetSelectedKey<SheetDef>(PIn.Long(clinicPref.ValueString),x=>x.SheetDefNum);
			}
		}

		private void comboClinicDefault_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinicDefault.SelectedClinicNum==_previousSelectedClinicNum) {
				return;
			}
			long tempClinicNum=comboClinicDefault.SelectedClinicNum;//Store the selected clinic while we make updates to the previous one
			comboClinicDefault.SelectedClinicNum=_previousSelectedClinicNum;//Set the selected clinic to the previous one
			//Only Clinic dependent combBoxes
			bool isStale=(ClinicDependentComboBoxes_Validate(comboBoxChartLayout,PrefName.SheetsDefaultChartModule)
				| ClinicDependentComboBoxes_Validate(comboRx,PrefName.SheetsDefaultRx)
				| ClinicDependentComboBoxes_Validate(comboTreatmentPlan,PrefName.SheetsDefaultTreatmentPlan));
			if(isStale && MsgBox.Show(this, MsgBoxButtons.YesNo, Lan.g(this,"Would you like to save your changes for the selected clinic?"))) {
				UpdateDefaultSheets();
			}
			comboClinicDefault.SelectedClinicNum=tempClinicNum;//Set the selected clinic to the newly selected clinic
			//Only Clinic Dependant comboBoxODs
			SelectComboBoxesDefault(comboRx,PrefName.SheetsDefaultRx);
			SelectComboBoxesDefault(comboBoxChartLayout,PrefName.SheetsDefaultChartModule);
			SelectComboBoxesDefault(comboTreatmentPlan,PrefName.SheetsDefaultTreatmentPlan);
			_previousSelectedClinicNum=comboClinicDefault.SelectedClinicNum;//Store the newly selected clinic for when we have to run this event again.
		}

		///<summary>Returns true if the comboboxes selected key is different from the stored key in the db</summary>
		private bool ClinicDependentComboBoxes_Validate(UI.ComboBoxOD comboBox, PrefName prefName) {
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,comboClinicDefault.SelectedClinicNum);
			if(clinicPref==null || comboClinicDefault.SelectedClinicNum==0) {
				Pref pref=Prefs.GetPref(prefName.GetDescription());
				return comboBox.GetSelectedKey<SheetDef>(x=>x.SheetDefNum)!=PIn.Long(pref.ValueString);//Inverse to tell if there was a change
			} 
			else {
				return comboBox.GetSelectedKey<SheetDef>(x=>x.SheetDefNum)!=PIn.Long(clinicPref.ValueString);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			UpdateDefaultSheets();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}