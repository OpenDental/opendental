using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
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
			Medication medicationSelected=null;
			if(shouldRetainSelection && gridAllMedications.GetSelectedIndex()!=-1) {
				medicationSelected=(Medication)gridAllMedications.ListGridRows[gridAllMedications.GetSelectedIndex()].Tag;
			}
			List <long> listInUseMedicationNums=Medications.GetAllInUseMedicationNums();
			int sortColIndex=gridAllMedications.GetSortedByColumnIdx();
			bool isSortAscending=gridAllMedications.IsSortedAscending();
			gridAllMedications.BeginUpdate();
			gridAllMedications.Columns.Clear();
			//The order of these columns is important.  See gridAllMedications_CellClick()
			GridColumn col=new GridColumn(Lan.g(this,"Drug Name"),120,GridSortingStrategy.StringCompare);
			gridAllMedications.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Generic Name"),120,GridSortingStrategy.StringCompare);
			gridAllMedications.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"InUse"),55,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridAllMedications.Columns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				col=new GridColumn(Lan.g(this,"RxNorm"),70,GridSortingStrategy.StringCompare);
				gridAllMedications.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Notes for Generic"),250,GridSortingStrategy.StringCompare);
			gridAllMedications.Columns.Add(col);
			gridAllMedications.ListGridRows.Clear();
			List <Medication> listMedications=Medications.GetList(textSearch.Text);
			for(int i=0;i<listMedications.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=listMedications[i];
				if(listMedications[i].MedicationNum==listMedications[i].GenericNum) {//isGeneric
					row.Cells.Add(listMedications[i].MedName);
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(listMedications[i].MedName);
					row.Cells.Add(Medications.GetGenericName(listMedications[i].GenericNum));
				}
				if(listInUseMedicationNums.Contains(listMedications[i].MedicationNum)) {
					row.Cells.Add("X");//InUse
				}
				else {
					row.Cells.Add("");//InUse
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					if(listMedications[i].RxCui==0) {
						row.Cells.Add(Lan.g(this,"(select)"));
						row.Cells[row.Cells.Count-1].Bold=YN.Yes;
					}
					else {
						row.Cells.Add(listMedications[i].RxCui.ToString());
					}
				}
				row.Cells.Add(listMedications[i].Notes);
				gridAllMedications.ListGridRows.Add(row);
			}
			gridAllMedications.EndUpdate();
			gridAllMedications.SortForced(sortColIndex,isSortAscending);
			if(medicationSelected!=null) {//Will be null if nothing is selected.
				for(int i=0;i<gridAllMedications.ListGridRows.Count;i++) {
					Medication medication=(Medication)gridAllMedications.ListGridRows[i].Tag;
					if(medication.MedicationNum==medicationSelected.MedicationNum) {
						gridAllMedications.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void FillGridMissing() {
			int sortColIndex=gridMissing.GetSortedByColumnIdx();
			bool isSortAscending=gridMissing.IsSortedAscending();
			gridMissing.BeginUpdate();
			gridMissing.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"RxNorm"),70,GridSortingStrategy.StringCompare);
			gridMissing.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Drug Description"),140,GridSortingStrategy.StringCompare);
			col.IsWidthDynamic=true;
			gridMissing.Columns.Add(col);
			gridMissing.ListGridRows.Clear();
			List<MedicationPat> listMedicationPats=MedicationPats.GetAllMissingMedications();
			List<List<MedicationPat>> listListsMedicationPatsMissingUnique=new List<List<MedicationPat>>();
			for(int i=0;i<listMedicationPats.Count;i++) {
				List<MedicationPat> listMedicationPatsEntry=null;
				if(!listListsMedicationPatsMissingUnique.IsNullOrEmpty()) {
					listMedicationPatsEntry=listListsMedicationPatsMissingUnique.Find(
						x => x[0].RxCui.ToString()==listMedicationPats[i].RxCui.ToString()
						&& x[0].MedDescript.ToLower().Trim()==listMedicationPats[i].MedDescript.ToLower().Trim()
					);
				}
				if(listMedicationPatsEntry!=null) {
					listMedicationPatsEntry.Add(listMedicationPats[i]);
					continue;
				}
				List<MedicationPat> listMedicationPatsEntryNew=new List<MedicationPat>();
				listMedicationPatsEntryNew.Add(listMedicationPats[i]);
				GridRow row=new GridRow();
				row.Tag=listMedicationPatsEntryNew;
				if(listMedicationPats[i].RxCui==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(listMedicationPats[i].RxCui.ToString());
				}
				row.Cells.Add(listMedicationPats[i].MedDescript);
				gridMissing.ListGridRows.Add(row);
				listListsMedicationPatsMissingUnique.Add(listMedicationPatsEntryNew);
				
			}
			gridMissing.EndUpdate();
			gridMissing.SortForced(sortColIndex,isSortAscending);
		}

		private void butAddGeneric_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			Medication medication=new Medication();
			Medications.Insert(medication);//so that we will have the primary key
			medication.GenericNum=medication.MedicationNum;
			using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
			formMedicationEdit.MedicationCur=medication;
			formMedicationEdit.IsNew=true;
			formMedicationEdit.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
			FillTab();
		}

		private void butAddBrand_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			if(gridAllMedications.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"You must first highlight the generic medication from the list.  If it is not already on the list, then you must add it first."));
				return;
			}
			Medication medicationSelected=(Medication)gridAllMedications.ListGridRows[gridAllMedications.GetSelectedIndex()].Tag;
			if(medicationSelected.MedicationNum!=medicationSelected.GenericNum){
				MessageBox.Show(Lan.g(this,"The selected medication is not generic."));
				return;
			}
			Medication medication=new Medication();
			Medications.Insert(medication);//so that we will have the primary key
			medication.GenericNum=medicationSelected.MedicationNum;
			using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
			formMedicationEdit.MedicationCur=medication;
			formMedicationEdit.IsNew=true;
			formMedicationEdit.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
			FillTab();
		}
		
		private void butImportMedications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			List<Medication> listMedicationImports=new List<Medication>();
			//Leaving code here to include later when we have developed a default medications list available for download.
			//if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Click Yes to download and import default medication list.\r\nClick No to import from a file.")) {
			//	Cursor=Cursors.WaitCursor;
			//	listMedicationImports=DownloadDefaultMedications();//Import from OpenDental.com
			//}
			//else {//Prompt for file.
			string fileName;
			if(!ODBuild.IsThinfinity() && ODCloudClient.IsAppStream) {
				fileName=ODCloudClient.ImportFileForCloud();
			}
			else {
				fileName=GetFilenameFromUser(true);
			}
			if(fileName.IsNullOrEmpty()) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				listMedicationImports=MedicationL.GetMedicationsFromFile(fileName);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				string msg=Lans.g(this,"Error accessing file. Close all programs using file and try again.");
				MessageBox.Show(this,msg+"\r\n: "+ex.Message);
				return;
			}
			//}
			int countImportedMedications=MedicationL.ImportMedications(listMedicationImports,Medications.GetList());
			int countDuplicateMedications=listMedicationImports.Count-countImportedMedications;
			DataValid.SetInvalid(InvalidType.Medications);
			Cursor=Cursors.Default;
			MessageBox.Show(this,POut.Int(countDuplicateMedications)+" "+Lan.g(this,"duplicate medications found.")+"\r\n"
				+POut.Int(countImportedMedications)+" "+Lan.g(this,"medications imported."));
			FillTab();
		}

		///<summary>Attempts to download the default medication list from HQ.
		///If there is an exception returns an empty list after showing the user an error prompt.</summary>
		private List<Medication> DownloadDefaultMedications() {
			List<Medication> listMedicationsNew=new List<Medication>();
			try {
				listMedicationsNew=MedicationL.GetMedicationsFromFile(MedicationL.DownloadDefaultMedicationsFile(),true);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Failed to download medications.")+"\r\n"+ex.Message);
			}
			return listMedicationsNew;
		}

		private void butExportMedications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			int countExportedMeds=0;
			string fileName;
			if(ODBuild.IsThinfinity()) {
				fileName="ExportedMedications.txt";
			}
			else {
				//Prompt for file.
				fileName=GetFilenameFromUser(false);
			}
			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
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
			FileDialog fileDialog=null;
			if(isImport) {
				fileDialog=new OpenFileDialog();
			}
			else {//Export
				fileDialog=new SaveFileDialog();
				fileDialog.Filter="(*.txt)|*.txt|All files (*.*)|*.*";
				fileDialog.FilterIndex=1;
				fileDialog.DefaultExt=".txt";
				fileDialog.FileName="ExportedMedications.txt";
			}
			fileDialog.InitialDirectory=initialDirectory;
			if(fileDialog.ShowDialog()!=DialogResult.OK) {
				fileDialog.Dispose();
				fileDialog=null;
			}
			else if(isImport && !File.Exists(fileDialog.FileName)){
				MsgBox.Show(this,"Error accessing file.");
				fileDialog.Dispose();
				fileDialog=null;
			}
			Cursor=Cursors.Default;
			string fileName="";
			if(fileDialog!=null) {
				fileName=fileDialog.FileName;
				fileDialog.Dispose();
			}
			return fileName;
		}
		
		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillTab();
		}

		private void gridAllMedications_CellClick(object sender,ODGridClickEventArgs e) {
			Medication medication=(Medication)gridAllMedications.ListGridRows[e.Row].Tag;
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && e.Col==3) {//United States RxNorm Column
				using FormRxNorms formRxNorms=new FormRxNorms();
				formRxNorms.IsSelectionMode=true;
				formRxNorms.InitSearchCodeOrDescript=medication.MedName;
				formRxNorms.ShowDialog();
				if(formRxNorms.DialogResult==DialogResult.OK) {
					medication.RxCui=PIn.Long(formRxNorms.RxNormSelected.RxCui);
					//The following behavior mimics FormMedicationEdit OK click.
					Medications.Update(medication);
					MedicationPats.UpdateRxCuiForMedication(medication.MedicationNum,medication.RxCui);
					DataValid.SetInvalid(InvalidType.Medications);
				}
				FillTab();
			}
		}

		private void gridAllMedications_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Medication medication=(Medication)gridAllMedications.ListGridRows[e.Row].Tag;
			medication=Medications.GetMedication(medication.MedicationNum);
			if(medication==null) {//Possible to delete the medication from a separate WS while medication loaded in memory.
				MsgBox.Show(this,"An error occurred loading medication.");
				return;
			}
			if(IsSelectionMode){
				SelectedMedicationNum=medication.MedicationNum;
				DialogResult=DialogResult.OK;
			}
			else{//normal mode from main menu
				if(!CultureInfo.CurrentCulture.Name.EndsWith("US") || e.Col!=3) {//Not United States RxNorm Column
					using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
					formMedicationEdit.MedicationCur=medication;
					formMedicationEdit.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
					FillTab();
				}
			}
		}

		private void butConvertGeneric_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			if(gridMissing.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item from the list before attempting to convert.");
				return;
			}
			List<MedicationPat> listMedicationPats=(List<MedicationPat>)gridMissing.ListGridRows[gridMissing.SelectedIndices[0]].Tag;
			List<Medication> listMedicationRxCuis=null;
			Medication medicationGeneric=null;
			if(listMedicationPats[0].RxCui!=0) {
				listMedicationRxCuis=Medications.GetAllMedsByRxCui(listMedicationPats[0].RxCui);
				medicationGeneric=listMedicationRxCuis.FirstOrDefault(x => x.MedicationNum==x.GenericNum);
				if(medicationGeneric==null && listMedicationRxCuis.FirstOrDefault(x => x.MedicationNum!=x.GenericNum)!=null) {//A Brand Medication exists with matching RxCui.
					MsgBox.Show(this,"A brand medication matching the RxNorm of the selected medication already exists in the medication list.  "
						+"You cannot create a generic for the selected medication.  Use the Convert to Brand button instead.");
					return;
				}
			}
			if(listMedicationRxCuis==null || listMedicationRxCuis.Count==0) {//No medications found matching the RxCui
				medicationGeneric=new Medication();
				medicationGeneric.MedName=listMedicationPats[0].MedDescript;
				medicationGeneric.RxCui=listMedicationPats[0].RxCui;
				Medications.Insert(medicationGeneric);//To get primary key.
				medicationGeneric.GenericNum=medicationGeneric.MedicationNum;
				Medications.Update(medicationGeneric);//Now that we have primary key, flag the medication as a generic.
				using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
				formMedicationEdit.MedicationCur=medicationGeneric;
				formMedicationEdit.IsNew=true;
				formMedicationEdit.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
				if(formMedicationEdit.DialogResult!=DialogResult.OK) {
					return;//User canceled.
				}
			}
			else if(medicationGeneric!=null &&
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A generic medication matching the RxNorm of the selected medication already exists in the medication list.  "
					+"Click OK to use the existing medication as the generic for the selected medication, or click Cancel to abort."))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			MedicationPats.UpdateMedicationNumForMany(medicationGeneric.MedicationNum,listMedicationPats.Select(x => x.MedicationPatNum).ToList());
			FillTab();
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
		}

		private void butConvertBrand_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.MedicationDefEdit)) {
				return;
			}
			if(gridMissing.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item from the list before attempting to convert.");
				return;
			}
			List<MedicationPat> listMedicationPats=(List<MedicationPat>)gridMissing.ListGridRows[gridMissing.SelectedIndices[0]].Tag;
			List<Medication> listMedicationsRxCuis=null;
			Medication medicationBrand=null;
			if(listMedicationPats[0].RxCui!=0) {
				listMedicationsRxCuis=Medications.GetAllMedsByRxCui(listMedicationPats[0].RxCui);
				medicationBrand=listMedicationsRxCuis.FirstOrDefault(x => x.MedicationNum!=x.GenericNum);
				if(medicationBrand==null && listMedicationsRxCuis.FirstOrDefault(x => x.MedicationNum==x.GenericNum)!=null) {//A Generic Medication exists with matching RxCui.
					MsgBox.Show(this,"A generic medication matching the RxNorm of the selected medication already exists in the medication list.  "
						+"You cannot create a brand for the selected medication.  Use the Convert to Generic button instead.");
					return;
				}
			}
			if(listMedicationsRxCuis==null || listMedicationsRxCuis.Count==0) {//No medications found matching the RxCui
				Medication medicationGeneric=null;
				if(gridAllMedications.SelectedIndices.Length > 0) {
					medicationGeneric=(Medication)gridAllMedications.ListGridRows[gridAllMedications.SelectedIndices[0]].Tag;
					if(medicationGeneric.MedicationNum!=medicationGeneric.GenericNum) {
						medicationGeneric=null;//The selected medication is a brand medication, not a generic medication.
					}
				}
				if(medicationGeneric==null) {
					MsgBox.Show(this,"Please select a generic medication from the All Medications tab before attempting to convert.  "
						+"The selected medication will be used as the generic medication for the new brand medication.");
					return;
				}
				medicationBrand=new Medication();
				medicationBrand.MedName=listMedicationPats[0].MedDescript;
				medicationBrand.RxCui=listMedicationPats[0].RxCui;
				medicationBrand.GenericNum=medicationGeneric.MedicationNum;
				Medications.Insert(medicationBrand);
				using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
				formMedicationEdit.MedicationCur=medicationBrand;
				formMedicationEdit.IsNew=true;
				formMedicationEdit.ShowDialog();//This window refreshes the Medication cache if the user clicked OK.
				if(formMedicationEdit.DialogResult!=DialogResult.OK) {
					return;//User canceled.
				}
			}
			else if(medicationBrand!=null &&
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A brand medication matching the RxNorm of the selected medication already exists in the medication list.  "
					+"Click OK to use the existing medication as the brand for the selected medication, or click Cancel to abort."))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			MedicationPats.UpdateMedicationNumForMany(medicationBrand.MedicationNum,listMedicationPats.Select(x => x.MedicationPatNum).ToList());
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

	}
}