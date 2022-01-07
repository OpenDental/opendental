using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormMedications : FormODBase {
		///<summary></summary>
		public bool IsSelectionMode;
		///<summary>the number returned if using select mode.</summary>
		public long SelectedMedicationNum;

		///<summary>Set isAll to true to start in the All Medications tab, or false to start in the Meds In Use tab.</summary>
		public FormMedications() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedications_Load(object sender, System.EventArgs e) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("US")) {//Not United States
				//Medications missing generic/brand are not visible for foreigners because there will be no data available.
				//This type of data can only be created in the United States for customers using NewCrop.
				tabMedications.TabPages.Remove(tabMissing);
			}
			FillTab();
			if(IsSelectionMode){
				this.Text=Lan.g(this,"Select Medication");
			}
			else{
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
			}
		}

		///<summary>Forces cursor to start in the search textbox.</summary>
		private void FormMedications_Shown(object sender,EventArgs e) {
			textSearch.Focus();//We've previously had issues with the tabindex not working.
		}

		private void tabMedications_SelectedIndexChanged(object sender,EventArgs e) {
			FillTab();
		}

		private void FillTab() {
			if(tabMedications.SelectedIndex==0) {//All Medication
				FillGridAllMedications();
			}
			else if(tabMedications.SelectedIndex==1) {//Missing Generic/Brand
				FillGridMissing();
			}
		}

		private void FillGridAllMedications(bool shouldRetainSelection=true){
			Medication medSelected=null;
			if(shouldRetainSelection && gridAllMedications.GetSelectedIndex()!=-1) {
				medSelected=(Medication)gridAllMedications.ListGridRows[gridAllMedications.GetSelectedIndex()].Tag;
			}
			List <long> listInUseMedicationNums=Medications.GetAllInUseMedicationNums();
			int sortColIndex=gridAllMedications.SortedByColumnIdx;
			bool isSortAscending=gridAllMedications.SortedIsAscending;
			gridAllMedications.BeginUpdate();
			gridAllMedications.ListGridColumns.Clear();
			//The order of these columns is important.  See gridAllMedications_CellClick()
			GridColumn col=new GridColumn(Lan.g(this,"Drug Name"),120,GridSortingStrategy.StringCompare);
			gridAllMedications.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Generic Name"),120,GridSortingStrategy.StringCompare);
			gridAllMedications.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"InUse"),55,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridAllMedications.ListGridColumns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				col=new GridColumn(Lan.g(this,"RxNorm"),70,GridSortingStrategy.StringCompare);
				gridAllMedications.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Notes for Generic"),250,GridSortingStrategy.StringCompare);
			gridAllMedications.ListGridColumns.Add(col);
			gridAllMedications.ListGridRows.Clear();
			List <Medication> listMeds=Medications.GetList(textSearch.Text);
			foreach(Medication med in listMeds) {
				GridRow row=new GridRow();
				row.Tag=med;
				if(med.MedicationNum==med.GenericNum) {//isGeneric
					row.Cells.Add(med.MedName);
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(med.MedName);
					row.Cells.Add(Medications.GetGenericName(med.GenericNum));
				}
				if(listInUseMedicationNums.Contains(med.MedicationNum)) {
					row.Cells.Add("X");//InUse
				}
				else {
					row.Cells.Add("");//InUse
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					if(med.RxCui==0) {
						row.Cells.Add(Lan.g(this,"(select)"));
						row.Cells[row.Cells.Count-1].Bold=YN.Yes;
					}
					else {
						row.Cells.Add(med.RxCui.ToString());
					}
				}
				row.Cells.Add(med.Notes);
				gridAllMedications.ListGridRows.Add(row);
			}
			gridAllMedications.EndUpdate();
			gridAllMedications.SortForced(sortColIndex,isSortAscending);
			if(medSelected!=null) {//Will be null if nothing is selected.
				for(int i=0;i<gridAllMedications.ListGridRows.Count;i++) {
					Medication medCur=(Medication)gridAllMedications.ListGridRows[i].Tag;
					if(medCur.MedicationNum==medSelected.MedicationNum) {
						gridAllMedications.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void FillGridMissing() {
			int sortColIndex=gridMissing.SortedByColumnIdx;
			bool isSortAscending=gridMissing.SortedIsAscending;
			gridMissing.BeginUpdate();
			gridMissing.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"RxNorm"),70,GridSortingStrategy.StringCompare);
			gridMissing.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Drug Description"),140,GridSortingStrategy.StringCompare){ IsWidthDynamic=true };
			gridMissing.ListGridColumns.Add(col);
			gridMissing.ListGridRows.Clear();
			List<MedicationPat> listMedPats=MedicationPats.GetAllMissingMedications();
			Dictionary <string,List<MedicationPat>> dictMissingUnique=new Dictionary<string,List<MedicationPat>>();
			foreach(MedicationPat medPat in listMedPats) {
				string key=medPat.RxCui.ToString()+" - "+medPat.MedDescript.ToLower().Trim();
				if(!dictMissingUnique.ContainsKey(key)) {
					dictMissingUnique[key]=new List<MedicationPat>();
					GridRow row=new GridRow();
					row.Tag=dictMissingUnique[key];
					if(medPat.RxCui==0) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(medPat.RxCui.ToString());
					}
					row.Cells.Add(medPat.MedDescript);
					gridMissing.ListGridRows.Add(row);
				}
				dictMissingUnique[key].Add(medPat);
			}
			gridMissing.EndUpdate();
			gridMissing.SortForced(sortColIndex,isSortAscending);
		}

		private void butAddGeneric_Click(object sender, System.EventArgs e) {
			Medication MedicationCur=new Medication();
			Medications.Insert(MedicationCur);//so that we will have the primary key
			MedicationCur.GenericNum=MedicationCur.MedicationNum;
			using FormMedicationEdit FormME=new FormMedicationEdit();
			FormME.MedicationCur=MedicationCur;
			FormME.IsNew=true;
			FormME.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
			FillTab();
		}

		private void butAddBrand_Click(object sender, System.EventArgs e) {
			if(gridAllMedications.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"You must first highlight the generic medication from the list.  If it is not already on the list, then you must add it first."));
				return;
			}
			Medication medSelected=(Medication)gridAllMedications.ListGridRows[gridAllMedications.GetSelectedIndex()].Tag;
			if(medSelected.MedicationNum!=medSelected.GenericNum){
				MessageBox.Show(Lan.g(this,"The selected medication is not generic."));
				return;
			}
			Medication MedicationCur=new Medication();
			Medications.Insert(MedicationCur);//so that we will have the primary key
			MedicationCur.GenericNum=medSelected.MedicationNum;
			using FormMedicationEdit FormME=new FormMedicationEdit();
			FormME.MedicationCur=MedicationCur;
			FormME.IsNew=true;
			FormME.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
			FillTab();
		}
		
		private void butImportMedications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			List<ODTuple<Medication,string>> listImportMeds=new List<ODTuple<Medication,string>>();
			//Leaving code here to include later when we have developed a default medications list available for download.
			//if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Click Yes to download and import default medication list.\r\nClick No to import from a file.")) {
			//	Cursor=Cursors.WaitCursor;
			//	listImportMeds=DownloadDefaultMedications();//Import from OpenDental.com
			//}
			//else {//Prompt for file.
			string fileName=GetFilenameFromUser(true);
			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			try {
				Cursor=Cursors.WaitCursor;
				listImportMeds=MedicationL.GetMedicationsFromFile(fileName);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				string msg=Lans.g(this,"Error accessing file. Close all programs using file and try again.");
				MessageBox.Show(this,msg+"\r\n: "+ex.Message);
				return;
			}
			//}
			int countImportedMedications=MedicationL.ImportMedications(listImportMeds,Medications.GetList());
			int countDuplicateMedications=listImportMeds.Count-countImportedMedications;
			DataValid.SetInvalid(InvalidType.Medications);
			Cursor=Cursors.Default;
			MessageBox.Show(this,POut.Int(countDuplicateMedications)+" "+Lan.g(this,"duplicate medications found.")+"\r\n"
				+POut.Int(countImportedMedications)+" "+Lan.g(this,"medications imported."));
			FillTab();
		}

		///<summary>Attempts to download the default medication list from HQ.
		///If there is an exception returns an empty list after showing the user an error prompt.</summary>
		private List<ODTuple<Medication,string>> DownloadDefaultMedications() {
			List<ODTuple<Medication,string>> listMedsNew=new List<ODTuple<Medication,string>>();
			string tempFile="";
			try {
				tempFile=MedicationL.DownloadDefaultMedicationsFile();
				listMedsNew=MedicationL.GetMedicationsFromFile(tempFile,true);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Failed to download medications.")+"\r\n"+ex.Message);
			}
			return listMedsNew;
		}

		private void butExportMedications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			int countExportedMeds=0;
			string fileName;
			if(ODBuild.IsWeb()) {
				fileName="ExportedMedications.txt";
			}
			else {
				//Prompt for file.
				fileName=GetFilenameFromUser(false);
			}
			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			try {
				Cursor=Cursors.WaitCursor;
				countExportedMeds=MedicationL.ExportMedications(fileName,Medications.GetList());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				string msg=Lans.g(this,"Error: ");
				MessageBox.Show(this,msg+": "+ex.Message);
			}
			Cursor=Cursors.Default;
			MessageBox.Show(this,POut.Int(countExportedMeds)+" "+Lan.g(this,"medications exported to:")+" "+fileName);
		}

		///<summary>When isImport is true, prompts users to select file and returns the full file path if OK clicked, otherwise an empty string.
		///When isImport is false (export), prompts user to select a file destination if OK clicked, otherwise an empty string..</summary>
		private string GetFilenameFromUser(bool isImport) {
			Cursor=Cursors.WaitCursor;
			string initialDirectory="";
			if(Directory.Exists(PrefC.GetString(PrefName.DocPath))) {
				initialDirectory=PrefC.GetString(PrefName.DocPath);
			}
			else if(Directory.Exists("C:\\")) {
				initialDirectory="C:\\";
			}
			FileDialog dlg=null;
			if(isImport) {
				dlg=new OpenFileDialog();
			}
			else {//Export
				dlg=new SaveFileDialog();
				dlg.Filter="(*.txt)|*.txt|All files (*.*)|*.*";
				dlg.FilterIndex=1;
				dlg.DefaultExt=".txt";
				dlg.FileName="ExportedMedications.txt";
			}
			dlg.InitialDirectory=initialDirectory;
			if(dlg.ShowDialog()!=DialogResult.OK) {
				dlg.Dispose();
				dlg=null;
			}
			else if(isImport && !File.Exists(dlg.FileName)){
				MsgBox.Show(this,"Error accessing file.");
				dlg.Dispose();
				dlg=null;
			}
			Cursor=Cursors.Default;
			string fileName="";
			if(dlg!=null) {
				fileName=dlg.FileName;
				dlg.Dispose();
			}
			return fileName;
		}
		
		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillTab();
		}

		private void gridAllMedications_CellClick(object sender,ODGridClickEventArgs e) {
			Medication med=(Medication)gridAllMedications.ListGridRows[e.Row].Tag;
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && e.Col==3) {//United States RxNorm Column
				using FormRxNorms formRxNorm=new FormRxNorms();
				formRxNorm.IsSelectionMode=true;
				formRxNorm.InitSearchCodeOrDescript=med.MedName;
				formRxNorm.ShowDialog();
				if(formRxNorm.DialogResult==DialogResult.OK) {
					med.RxCui=PIn.Long(formRxNorm.SelectedRxNorm.RxCui);
					//The following behavior mimics FormMedicationEdit OK click.
					Medications.Update(med);
					MedicationPats.UpdateRxCuiForMedication(med.MedicationNum,med.RxCui);
					DataValid.SetInvalid(InvalidType.Medications);
				}
				FillTab();
			}
		}

		private void gridAllMedications_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Medication med=(Medication)gridAllMedications.ListGridRows[e.Row].Tag;
			med=Medications.GetMedication(med.MedicationNum);
			if(med==null) {//Possible to delete the medication from a separate WS while medication loaded in memory.
				MsgBox.Show(this,"An error occurred loading medication.");
				return;
			}
			if(IsSelectionMode){
				SelectedMedicationNum=med.MedicationNum;
				DialogResult=DialogResult.OK;
			}
			else{//normal mode from main menu
				if(!CultureInfo.CurrentCulture.Name.EndsWith("US") || e.Col!=3) {//Not United States RxNorm Column
					using FormMedicationEdit FormME=new FormMedicationEdit();
					FormME.MedicationCur=med;
					FormME.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
					FillTab();
				}
			}
		}

		private void butConvertGeneric_Click(object sender,EventArgs e) {
			if(gridMissing.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item from the list before attempting to convert.");
				return;
			}
			List<MedicationPat> listMedPats=(List<MedicationPat>)gridMissing.ListGridRows[gridMissing.SelectedIndices[0]].Tag;
			List<Medication> listRxCuiMeds=null;
			Medication medGeneric=null;
			if(listMedPats[0].RxCui!=0) {
				listRxCuiMeds=Medications.GetAllMedsByRxCui(listMedPats[0].RxCui);
				medGeneric=listRxCuiMeds.FirstOrDefault(x => x.MedicationNum==x.GenericNum);
				if(medGeneric==null && listRxCuiMeds.FirstOrDefault(x => x.MedicationNum!=x.GenericNum)!=null) {//A Brand Medication exists with matching RxCui.
					MsgBox.Show(this,"A brand medication matching the RxNorm of the selected medication already exists in the medication list.  "
						+"You cannot create a generic for the selected medication.  Use the Convert to Brand button instead.");
					return;
				}
			}
			if(listRxCuiMeds==null || listRxCuiMeds.Count==0) {//No medications found matching the RxCui
				medGeneric=new Medication();
				medGeneric.MedName=listMedPats[0].MedDescript;
				medGeneric.RxCui=listMedPats[0].RxCui;
				Medications.Insert(medGeneric);//To get primary key.
				medGeneric.GenericNum=medGeneric.MedicationNum;
				Medications.Update(medGeneric);//Now that we have primary key, flag the medication as a generic.
				using FormMedicationEdit FormME=new FormMedicationEdit();
				FormME.MedicationCur=medGeneric;
				FormME.IsNew=true;
				FormME.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
				if(FormME.DialogResult!=DialogResult.OK) {
					return;//User canceled.
				}
			}
			else if(medGeneric!=null &&
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A generic medication matching the RxNorm of the selected medication already exists in the medication list.  "
					+"Click OK to use the existing medication as the generic for the selected medication, or click Cancel to abort."))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			MedicationPats.UpdateMedicationNumForMany(medGeneric.MedicationNum,listMedPats.Select(x => x.MedicationPatNum).ToList());
			FillTab();
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
		}

		private void butConvertBrand_Click(object sender,EventArgs e) {
			if(gridMissing.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item from the list before attempting to convert.");
				return;
			}
			List<MedicationPat> listMedPats=(List<MedicationPat>)gridMissing.ListGridRows[gridMissing.SelectedIndices[0]].Tag;
			List<Medication> listRxCuiMeds=null;
			Medication medBrand=null;
			if(listMedPats[0].RxCui!=0) {
				listRxCuiMeds=Medications.GetAllMedsByRxCui(listMedPats[0].RxCui);
				medBrand=listRxCuiMeds.FirstOrDefault(x => x.MedicationNum!=x.GenericNum);
				if(medBrand==null && listRxCuiMeds.FirstOrDefault(x => x.MedicationNum==x.GenericNum)!=null) {//A Generic Medication exists with matching RxCui.
					MsgBox.Show(this,"A generic medication matching the RxNorm of the selected medication already exists in the medication list.  "
						+"You cannot create a brand for the selected medication.  Use the Convert to Generic button instead.");
					return;
				}
			}
			if(listRxCuiMeds==null || listRxCuiMeds.Count==0) {//No medications found matching the RxCui
				Medication medGeneric=null;
				if(gridAllMedications.SelectedIndices.Length > 0) {
					medGeneric=(Medication)gridAllMedications.ListGridRows[gridAllMedications.SelectedIndices[0]].Tag;
					if(medGeneric.MedicationNum!=medGeneric.GenericNum) {
						medGeneric=null;//The selected medication is a brand medication, not a generic medication.
					}
				}
				if(medGeneric==null) {
					MsgBox.Show(this,"Please select a generic medication from the All Medications tab before attempting to convert.  "
						+"The selected medication will be used as the generic medication for the new brand medication.");
					return;
				}
				medBrand=new Medication();
				medBrand.MedName=listMedPats[0].MedDescript;
				medBrand.RxCui=listMedPats[0].RxCui;
				medBrand.GenericNum=medGeneric.MedicationNum;
				Medications.Insert(medBrand);
				using FormMedicationEdit FormME=new FormMedicationEdit();
				FormME.MedicationCur=medBrand;
				FormME.IsNew=true;
				FormME.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
				if(FormME.DialogResult!=DialogResult.OK) {
					return;//User canceled.
				}
			}
			else if(medBrand!=null &&
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A brand medication matching the RxNorm of the selected medication already exists in the medication list.  "
					+"Click OK to use the existing medication as the brand for the selected medication, or click Cancel to abort."))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			MedicationPats.UpdateMedicationNumForMany(medBrand.MedicationNum,listMedPats.Select(x => x.MedicationPatNum).ToList());
			FillTab();
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//this button is not visible if not selection mode.
			if(gridAllMedications.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			SelectedMedicationNum=((Medication)gridAllMedications.ListGridRows[gridAllMedications.GetSelectedIndex()].Tag).MedicationNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















