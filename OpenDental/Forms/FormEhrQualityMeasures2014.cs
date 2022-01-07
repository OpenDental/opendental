using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using System.IO;
#if EHRTEST
using EHR;
#endif

namespace OpenDental {
	public partial class FormEhrQualityMeasures2014:FormODBase {
		private List<QualityMeasure> listQ;
		private List<Provider> listProvsKeyed;
		private long _provNum;
		private DateTime _dateStart;
		private DateTime _dateEnd;
		public long selectedPatNum;
		private List<Provider> _listProviders;

		public FormEhrQualityMeasures2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormQualityMeasures_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			listProvsKeyed=new List<Provider>();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				string ehrKey="";
				int yearValue=0;
				List<EhrProvKey> listProvKeys=EhrProvKeys.GetKeysByFLName(_listProviders[i].LName,_listProviders[i].FName);
				if(listProvKeys.Count!=0) {
					ehrKey=listProvKeys[0].ProvKey;
					yearValue=listProvKeys[0].YearValue;
				}
				if(FormEHR.ProvKeyIsValid(_listProviders[i].LName,_listProviders[i].FName,yearValue,ehrKey)) {
					//EHR has been valid.
					listProvsKeyed.Add(_listProviders[i]);
				}
			}
			if(listProvsKeyed.Count==0) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"No providers found with ehr keys.");
				return;
			}
			for(int i=0;i<listProvsKeyed.Count;i++) {
				comboProv.Items.Add(listProvsKeyed[i].GetLongDesc());
				if(Security.CurUser.ProvNum==listProvsKeyed[i].ProvNum) {
					comboProv.SelectedIndex=i;
				}
			}
			textDateStart.Text=(new DateTime(DateTime.Now.Year,1,1)).ToShortDateString();
			textDateEnd.Text=(new DateTime(DateTime.Now.Year,12,31)).ToShortDateString();
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void FillGrid() {
			if(comboProv.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateEnd=PIn.Date(textDateEnd.Text);
			if(dateStart==DateTime.MinValue || dateEnd==DateTime.MinValue) {
				MsgBox.Show(this,"Fix date format and try again.");
				return;
			}
			_dateStart=dateStart;
			_dateEnd=dateEnd;
			_provNum=listProvsKeyed[comboProv.SelectedIndex].ProvNum;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Id",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Denominator",75,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Numerator",65,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exclusion",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exception",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("NotMet",50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("PerformanceRate",100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			listQ=QualityMeasures.GetAll2014(dateStart,dateEnd,_provNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listQ.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listQ[i].Id);
				row.Cells.Add(listQ[i].Descript);
				row.Cells.Add(listQ[i].Denominator.ToString());
				row.Cells.Add(listQ[i].Numerator.ToString());
				row.Cells.Add(listQ[i].Exclusions.ToString());
				row.Cells.Add(listQ[i].Exceptions.ToString());
				row.Cells.Add(listQ[i].NotMet.ToString());
				row.Cells.Add(listQ[i].Numerator.ToString()+"/"+(listQ[i].Numerator+listQ[i].NotMet).ToString()
					+"  = "+listQ[i].PerformanceRate.ToString()+"%");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Launches edit window for double clicked item.</summary>
		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormEhrQualityMeasureEdit2014 FormQME=new FormEhrQualityMeasureEdit2014();
			FormQME.MeasureCur=listQ[e.Row];
			FormQME.ShowDialog();
			if(FormQME.DialogResult==DialogResult.OK && FormQME.selectedPatNum!=0) {
				selectedPatNum=FormQME.selectedPatNum;
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void butCreateQRDAs_Click(object sender,EventArgs e) {
			if(comboProv.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			if(listQ==null) {
				MsgBox.Show(this,"Click Refresh first.");
				return;
			}
			long provSelected=listProvsKeyed[comboProv.SelectedIndex].ProvNum;
			if(_provNum!=provSelected) {
				MsgBox.Show(this,"The values in the grid do not apply to the provider selected.  Click Refresh first.");
				return;
			}
			Provider provDefault=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			long provNumLegal=provDefault.ProvNum;
			//The practice default provider may be set to a non-person, like Apple Tree Dental, in which case there is no first name allowed and an NPI number does not make sense.
			//Prompt user to select the provider to set as the legal authenticator for the QRDA documents.
			//The Legal Authenticator must have a valid first name, last name, and NPI number and is the "single person legally responsible for the document" and "must be a person".
			if(provDefault.IsNotPerson) {
				MsgBox.Show(this,"The practice default provider is marked 'Not a Person'.  Please select the provider legally responsible for the documents.  The provider must have a first name, last name, and NPI number.");
				using FormProviderPick FormPP=new FormProviderPick();
				if(FormPP.ShowDialog()!=DialogResult.OK) {
					return;
				}
				if(Providers.GetProv(FormPP.SelectedProvNum).IsNotPerson) {
					MsgBox.Show(this,"The selected provider was marked 'Not a person'.");
					return;
				}
				provNumLegal=FormPP.SelectedProvNum;
			}
			using FolderBrowserDialog fbd = new FolderBrowserDialog();
			if(fbd.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string folderPath=fbd.SelectedPath+"\\"+"CQMs_"+DateTime.Today.ToString("MM_dd_yyyy");
			if(System.IO.Directory.Exists(folderPath)) {//if the folder already exists
				//find a unique folder name
				int uniqueID=1;
				string originalPath=folderPath;
				do {
					folderPath=originalPath+"_"+uniqueID.ToString();
					uniqueID++;
				}
				while(System.IO.Directory.Exists(folderPath));
			}
			try {
				System.IO.Directory.CreateDirectory(folderPath);
			}
			catch(Exception ex) {
				MessageBox.Show("Folder was not created: "+ex.Message);
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				QualityMeasures.GenerateQRDA(listQ,_provNum,_dateStart,_dateEnd,folderPath,provNumLegal);//folderPath is a new directory created within the chosen directory
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				if(ex.InnerException != null && ex.InnerException.Message=="true") {
					using FormOIDRegistryInternal FormOIDs=new FormOIDRegistryInternal();
					FormOIDs.ShowDialog();
				}
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"QRDA files have been created within the selected directory.");
		}

		private void butSubmit_Click(object sender,EventArgs e) {
			if(listQ==null) {
				MsgBox.Show(this,"Click Refresh first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				//EmailMessages.SendTestUnsecure("QRDA","qrda.xml",GenerateQRDA());
				//code to export will need to include the cda.xsl style sheet as well as the cda.xsd
				//using FolderBrowserDialog dlg=new FolderBrowserDialog();
				//dlg.SelectedPath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());//Default to patient image folder.
				//DialogResult result=dlg.ShowDialog();
				//if(result!=DialogResult.OK) {
				//	return;
				//}
				//if(File.Exists(Path.Combine(dlg.SelectedPath,"ccd.xml"))) {
				//	if(MessageBox.Show("Overwrite existing ccd.xml?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				//		return;
				//	}
				//}
				//File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xml"),ccd);
				//File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xsl"),FormEHR.GetEhrResource("CCD"));
				//EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
				//newMeasureEvent.DateTEvent = DateTime.Now;
				//newMeasureEvent.EventType = EhrMeasureEventType.ClinicalSummaryProvidedToPt;
				//newMeasureEvent.PatNum = PatCur.PatNum;
				//EhrMeasureEvents.Insert(newMeasureEvent);
				//FillGridEHRMeasureEvents();
				//MessageBox.Show("Exported");	
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Sent");
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	

	

		

	}
}
