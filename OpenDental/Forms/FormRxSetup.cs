using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRxSetup : FormODBase {

		///<summary></summary>
		public FormRxSetup(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRxSetup_Load(object sender, System.EventArgs e) {
			checkProcCodeRequired.Checked=PrefC.GetBool(PrefName.RxHasProc);
			FillGrid();
		}

		private void FillGrid(){
			RxDef[] arrayRxDefs=RxDefs.Refresh();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxSetup","Drug"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Controlled"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Sig"),320);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Disp"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Refills"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Notes"),300);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<arrayRxDefs.Length;i++){
				row=new GridRow();
				row.Cells.Add(arrayRxDefs[i].Drug);
				if(arrayRxDefs[i].IsControlled){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(arrayRxDefs[i].Sig);
				row.Cells.Add(arrayRxDefs[i].Disp);
				row.Cells.Add(arrayRxDefs[i].Refills);
				row.Cells.Add(arrayRxDefs[i].Notes);
				row.Tag=arrayRxDefs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private bool VerifyRxCombineData(RxDef rxDef,List<RxDef> listRxDefs) {
			List<string> listWarnings=new List<string>();
			if(rxDef==null) {//In case it is a completely invalid RxDef
				return false;//should never happen.
			}
			//==================== DRUG NAME ====================
			if(listRxDefs.Any(x=>x.Drug!=rxDef.Drug && !string.IsNullOrWhiteSpace(x.Drug))) {
				listWarnings.Add(Lan.g(this,"Prescription Drug Name"));
			}
			//==================== DIRECTIONS FOR THE PHARMACIST INFO ====================
			if(listRxDefs.Any(x => x.Sig!=rxDef.Sig && !string.IsNullOrWhiteSpace(x.Sig))) {
				listWarnings.Add(Lan.g(this,"Prescription Pharmacist Directions"));
			}
			//==================== AMOUNT TO DISPENSE ====================
			if(listRxDefs.Any(x => x.Disp!=rxDef.Disp && !string.IsNullOrWhiteSpace(x.Disp))) {
				listWarnings.Add(Lan.g(this,"Prescription Amount to Dispense"));
			}
			//==================== REFILLS ====================
			if(listRxDefs.Any(x => x.Refills!=rxDef.Refills && !string.IsNullOrWhiteSpace(x.Refills))) {
				listWarnings.Add(Lan.g(this,"Prescription Number of Refills"));
			}
			if(listWarnings.Count>0) {
				string warningMessage=Lan.g(this,"WARNING!")+" "+Lan.g(this,"Mismatched data has been detected between selected prescriptions")+":\r\n\r\n"
					+string.Join("\r\n",listWarnings)+"\r\n\r\n"
					+Lan.g(this,"Would you like to continue combining prescriptions anyway?");
				if(MessageBox.Show(warningMessage,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return false;
				}
			}
			return true;
		}

		private void checkProcCodeRequired_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.RxHasProc,checkProcCodeRequired.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormRxDefEdit FormE=new FormRxDefEdit(gridMain.SelectedTag<RxDef>());
			FormE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			RxDef RxDefCur=new RxDef();
			RxDefs.Insert(RxDefCur);//It gets deleted if user clicks cancel
			using FormRxDefEdit FormE=new FormRxDefEdit(RxDefCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			FillGrid();
		}

		private void butAdd2_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Must select one and only one item.");
				return;
			}
			RxDef RxDefCur=gridMain.SelectedTag<RxDef>().Copy();
			RxDefs.Insert(RxDefCur);//Now it has a new id.  It gets deleted if user clicks cancel. Alerts not copied.
			using FormRxDefEdit FormE=new FormRxDefEdit(RxDefCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			FillGrid();
		}

		private void butCombine_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RxMerge)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<2){
				MsgBox.Show(this,"Please select multiple items first while holding down the control key.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Combine all these prescriptions into a single prescription?"
				+" This will remove all other prescriptions leaving you with one selection." 
				+" The next window will let you select which prescription to keep when combining.")) 
			{
				return;
			}
			List<RxDef> listRxDefs=gridMain.SelectedTags<RxDef>();
			using FormRxCombine formRxCombine=new FormRxCombine();
			formRxCombine.ListRxDefs=listRxDefs;
			formRxCombine.ShowDialog();
			if(formRxCombine.DialogResult!=DialogResult.OK){
				return;
			}
			RxDef pickedRxDef=formRxCombine.PickedRxDef;
			if(!VerifyRxCombineData(pickedRxDef,listRxDefs)) {
				return;
			}
			//Capture the names of the combined prescriptions before merge for logEntries
			List<string> rxDefNames=new List<string>();
			string rxDefTo=pickedRxDef.Drug;
			for(int i=0;i<listRxDefs.Count;i++) {
				if(listRxDefs[i].RxDefNum==pickedRxDef.RxDefNum) {
					continue;
				}
				rxDefNames.Add(listRxDefs[i].Drug);
			}
			try {
				RxDefs.Combine(listRxDefs.Select(x => x.RxDefNum).ToList(),pickedRxDef.RxDefNum);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			//Prescriptions were combined successfully. Loop through and make a securitylog entry for each one that was changed.
			for(int i=0;i<rxDefNames.Count;i++) {
				SecurityLogs.MakeLogEntry(Permissions.RxMerge,0,Lan.g(this,"Prescription with name")+" "+rxDefNames[i]+" "
					+Lan.g(this,"was merged with")+" "+rxDefTo);
			}
			MsgBox.Show(this,"Merge complete.");
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	}
}
