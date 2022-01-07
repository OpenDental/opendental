using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormEhrPatientExport:FormODBase {
		private DataTable _table;
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private List<Site> _listSites;

		public FormEhrPatientExport() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrPatientExport_Load(object sender,EventArgs e) {
			comboProv.Items.Add(Lan.g(this,"All"));
			comboProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(!PrefC.HasClinicsEnabled) {
				comboClinic.Visible=false;
				labelClinic.Visible=false;
			}
			else {
				comboClinic.Items.Add(Lan.g(this,"All"));
				comboClinic.SelectedIndex=0;
				_listClinics=Clinics.GetDeepCopy(true);
				foreach(Clinic clin in _listClinics) {
					comboClinic.Items.Add(clin.Abbr);
				}
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else {
				comboSite.Items.Add(Lan.g(this,"All"));
				comboSite.SelectedIndex=0;
				_listSites=Sites.GetDeepCopy();
				for(int i=0;i<_listSites.Count;i++) {
					comboSite.Items.Add(_listSites[i].Description);
				}
			}
		}

		private void butSearch_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			FillGridMain();
		}

		private void FillGridMain() {
			//Get filters from user input
			string firstName="";
			if(textFName.Text!="") {
				firstName=textFName.Text;
			}
			string lastName="";
			if(textLName.Text!="") {
				lastName=textLName.Text;
			}
			int patNum=0;
			try {
				if(textPatNum.Text!="") {
					patNum=PIn.Int(textPatNum.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Invalid PatNum");
				return;
			}
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedIndex!=0) {
				clinicNum=_listClinics[comboClinic.SelectedIndex-1].ClinicNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			//Get table
			_table=Patients.GetExportList(patNum,firstName,lastName,provNum,clinicNum,siteNum);
			//Create grid			
			//Patient Name | Primary Provider | Date Last Visit | Clinic | Site 
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("PatNum",60);
			col.SortingStrategy=GridSortingStrategy.AmountParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Patient Name",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Primary Provider",110);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn("Clinic",110);
				gridMain.ListGridColumns.Add(col);
			}
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				col=new GridColumn("Site",110);
				gridMain.ListGridColumns.Add(col);
			}
			//Fill grid
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["PatNum"].ToString());
				row.Cells.Add(_table.Rows[i]["LName"].ToString()+", "+_table.Rows[i]["FName"].ToString());
				row.Cells.Add(_table.Rows[i]["Provider"].ToString());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(_table.Rows[i]["Clinic"].ToString());
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					row.Cells.Add(_table.Rows[i]["Site"].ToString());
				}
				row.Tag=PIn.Long(_table.Rows[i]["PatNum"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butExport_Click(object sender,EventArgs e) {
			string strCcdValidationErrors=EhrCCD.ValidateSettings();
			if(strCcdValidationErrors!="") {//Do not even try to export if global settings are invalid.
				MessageBox.Show(strCcdValidationErrors);//We do not want to use translations here, because the text is dynamic. The errors are generated in the business layer, and Lan.g() is not available there.
				return;
			}
			FolderBrowserDialog dlg=new FolderBrowserDialog();
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK) {
				return;
			}
			DateTime dateNow=DateTime.Now;
			string folderPath=ODFileUtils.CombinePaths(dlg.SelectedPath,(dateNow.Year+"_"+dateNow.Month+"_"+dateNow.Day));
			if(Directory.Exists(folderPath)) {
				int loopCount=1;
				while(Directory.Exists(folderPath+"_"+loopCount)) {
					loopCount++;
				}
				folderPath=folderPath+"_"+loopCount;
			}
			try {
				Directory.CreateDirectory(folderPath);
			}
			catch {
				MessageBox.Show("Error, Could not create folder");
				return;
			}
			this.Cursor=Cursors.WaitCursor;
			Patient patCur;
			string fileName;
			int numSkipped=0;  //Number of patients skipped. Set to -1 if only one patient was selected and had CcdValidationErrors.
			string patientsSkipped="";  //Names of the patients that were skipped, so we can tell the user which ones didn't export correctly.
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				patCur=Patients.GetPat((long)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag);//Cannot use GetLim because more information is needed in the CCD message generation below.
				strCcdValidationErrors=EhrCCD.ValidatePatient(patCur);
				if(strCcdValidationErrors!="") {
					if(gridMain.SelectedIndices.Length==1) {
						numSkipped=-1; //Set to -1 so we know below to not show the "exported" message.
						MessageBox.Show(Lan.g(this,"Patient not exported due to the following errors")+":\r\n"+strCcdValidationErrors);
						continue;
					}
					//If one patient is missing the required information for export, then simply skip the patient. We do not want to popup a message,
					//because it would be hard to get through the export if many patients were missing required information.
					numSkipped++;
					patientsSkipped+="\r\n"+patCur.LName+", "+patCur.FName;
					continue;
				}
				fileName="";
				string lName=patCur.LName;
				for(int j=0;j<lName.Length;j++) {  //Strip all non-letters from FName
					if(Char.IsLetter(lName,j)) {
						fileName+=lName.Substring(j,1);
					}
				}
				fileName+="_";
				string fName=patCur.FName;
				for(int k=0;k<fName.Length;k++) {  //Strip all non-letters from LName
					if(Char.IsLetter(fName,k)) {
						fileName+=fName.Substring(k,1);
					}
				}
				fileName+="_"+patCur.PatNum;  //LName_FName_PatNum
				string ccd=EhrCCD.GeneratePatientExport(patCur);
				try {
					File.WriteAllText(ODFileUtils.CombinePaths(folderPath,fileName+".xml"),ccd);
				}
				catch {
					MessageBox.Show("Error, Could not create xml file");
					this.Cursor=Cursors.Default;
					return;
				}
			}
			if(numSkipped==-1) {	//Will be -1 if only one patient was selected, and it did not export correctly.
				this.Cursor=Cursors.Default;
				return;//Don't display "Exported" to the user because the CCD was not exported.
			}
			try {
				File.WriteAllText(ODFileUtils.CombinePaths(folderPath,"CCD.xsl"),FormEHR.GetEhrResource("CCD"));
			}
			catch {
				MessageBox.Show("Error, Could not create stylesheet file");
			}
			string strMsg=Lan.g(this,"Exported");
			if(numSkipped>0) {
				strMsg+=". "+Lan.g(this,"Patients skipped due to missing information")+": "+numSkipped+patientsSkipped;
				MsgBoxCopyPaste msgCP=new MsgBoxCopyPaste(strMsg);
				msgCP.Show();
			}
			else {
				MessageBox.Show(strMsg);
			}
			this.Cursor=Cursors.Default;
		}

		private void butExportAll_Click(object sender,EventArgs e) {
			//Export all active patients
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}