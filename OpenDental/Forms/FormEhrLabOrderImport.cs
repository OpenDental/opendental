using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEhrLabOrderImport:FormODBase {
		public string Hl7LabMessage;
		private List<EhrLab> ListEhrLabs;
		public Patient PatCur;

		public FormEhrLabOrderImport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEhrLabOrders_Load(object sender,EventArgs e) {
			ListEhrLabs=EhrLabs.ProcessHl7Message(Hl7LabMessage,true);
			AttachPatientHelper();
			FillPatientPicker();
			FillPatientInfo();
			FillGrid();
			for(int i=0;i<ListEhrLabs.Count;i++){//check for existing labs in DB.
				if(EhrLabs.GetByGUID(ListEhrLabs[i].PlacerOrderUniversalID,ListEhrLabs[i].PlacerOrderNum)!=null
					|| EhrLabs.GetByGUID(ListEhrLabs[i].FillerOrderUniversalID,ListEhrLabs[i].FillerOrderNum)!=null) {
					labelExistingLab.Visible=true;
					break;
				}
			}
		}

		private void AttachPatientHelper() {
			Patient patAttach=EhrLabs.FindAttachedPatient(Hl7LabMessage);
			if(patAttach==null){
				return;//no reccomended patient
			}
			else if(PatCur==null){
				PatCur=patAttach;
			}
			else if(PatCur.PatNum!=patAttach.PatNum) {
				MsgBox.Show(this,"Patient mismatch. Selected patient does not match detected patient.");//will only happen if we set PatCur from somewhere else. Probably wont ever happen.
				PatCur=patAttach;
			}
			else {
				//I dunno what to put here; maybe a little picture of a dog wearing a fireman costume?
			}
		}

		private void FillPatientPicker() {
			if(PatCur==null) {
				textName.Text="";
				return;
			}
			textName.Text=PatCur.GetNameFL();
		}

		///<summary>Fills patient information from message contents, not from PatCur.</summary>
		private void FillPatientInfo() {
			string[] PIDFields;
			try {
				PIDFields=Hl7LabMessage.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries)[1].Split('|');
			}
			catch {
				return;//invalid HL7Message
			}
			//patient name(s)
			for(int i=0;i<PIDFields[5].Split('~').Length;i++) {
				string patName="";
				try{patName+=PIDFields[5].Split('~')[i].Split('^')[4]+" ";}catch{}//Prefix
				try{patName+=PIDFields[5].Split('~')[i].Split('^')[1]+" ";}catch{}//FName
				try{patName+=PIDFields[5].Split('~')[i].Split('^')[2]+" ";}catch{}//Middle Name(s)
				try{patName+=PIDFields[5].Split('~')[i].Split('^')[0]+" ";}catch{}//Last Name
				try{patName+=PIDFields[5].Split('~')[i].Split('^')[3]    ;}catch{}//Suffix
				listBoxNames.Items.Add(patName);
			}
			//Birthdate
			textBirthdate.Text=PIDFields[7];
			//Gender
			textGender.Text=PIDFields[8];
			//Race(s)
			for(int i=0;i<PIDFields[10].Split('~').Length;i++) {
				try{listBoxRaces.Items.Add(PIDFields[10].Split('~')[i].Split('^')[1]);}catch{}
				try{listBoxRaces.Items.Add(PIDFields[10].Split('~')[i].Split('^')[4]);}catch{}
			}
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Date Time",80);//Formatted yyyyMMdd
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Placer Order Number",180);//Should be PK but might not be. Instead use Placer Order Num.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Filler Order Number",180);//Should be PK but might not be. Instead use Placer Order Num.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Results",80);//Or date of latest result? or both?
			gridMain.ListGridColumns.Add(col);
			//ListEhrLabs = EhrLabs.GetAllForPat(PatCur.PatNum);//do not update here, all this lab information is cached.
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListEhrLabs.Count;i++) {
				row=new GridRow();
				string dateSt=ListEhrLabs[i].ObservationDateTimeStart.Substring(0,8);//stored in DB as yyyyMMddhhmmss-zzzz
				DateTime dateT=PIn.Date(dateSt.Substring(4,2)+"/"+dateSt.Substring(6,2)+"/"+dateSt.Substring(0,4));
				row.Cells.Add(dateT.ToShortDateString());//date only
				row.Cells.Add(ListEhrLabs[i].PlacerOrderNum);
				row.Cells.Add(ListEhrLabs[i].FillerOrderNum);
				row.Cells.Add(ListEhrLabs[i].ListEhrLabResults.Count.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabOrderEdit2014 FormLOE=new FormEhrLabOrderEdit2014();
			FormLOE.EhrLabCur=ListEhrLabs[e.Row];
			FormLOE.IsImport=true;
			FormLOE.ShowDialog();
			//if(FormLOE.DialogResult!=DialogResult.OK) {
			//	return;
			//}
			//EhrLabs.SaveToDB(FormLOE.EhrLabCur);
			//for(int i=0;i<FormLOE.EhrLabCur.ListEhrLabResults.Count;i++) {
			//	if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS){
			//		FormCDSIntervention FormCDSI=new FormCDSIntervention();
			//		FormCDSI.ListCDSI=EhrTriggers.TriggerMatch(FormLOE.EhrLabCur.ListEhrLabResults[i],PatCur);
			//		FormCDSI.ShowIfRequired(false);
			//	}
			//}
			//TODO:maybe add more code here for when we come back from form... In case we delete a lab from the form.
		}

		private void butPatSelect_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			PatCur=Patients.GetPat(FormPS.SelectedPatNum);
			FillPatientPicker();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(PatCur==null) {
				MsgBox.Show(this,"Please attach to patient first.");
				return;
			}
			//Check lab dates to see if these labs already exist.
			for(int i=0;i<ListEhrLabs.Count;i++) {
				EhrLab tempLab=null;//lab from DB if it exists.
				tempLab=EhrLabs.GetByGUID(ListEhrLabs[i].PlacerOrderUniversalID,ListEhrLabs[i].PlacerOrderNum);
				if(tempLab==null){
					tempLab=EhrLabs.GetByGUID(ListEhrLabs[i].FillerOrderUniversalID,ListEhrLabs[i].FillerOrderNum);
				}
				if(tempLab!=null) {
					//validate Date of Lab and attached patient.
					//Date
					if(tempLab.ResultDateTime.CompareTo(ListEhrLabs[i].ResultDateTime)<0) {//string compare dates will return 1+ if tempLab Date is greater.
						MsgBox.Show(this,"This lab already exists in the database and has a more recent timestamp.");
						continue;
					}
					if(PatCur.PatNum!=tempLab.PatNum) {
						//do nothing. We are importing an updated lab result and the previous lab result was attached to the wrong patient.
						//or do something. later maybe.
					}
				}
				ListEhrLabs[i].PatNum=PatCur.PatNum; 
				Provider prov=Providers.GetProv(Security.CurUser.ProvNum);
				if(Security.CurUser.ProvNum!=0 && EhrProvKeys.GetKeysByFLName(prov.LName,prov.FName).Count>0) {//The user who is currently logged in is a provider and has a valid EHR key.
					ListEhrLabs[i].IsCpoe=true;
				}
				ListEhrLabs[i]=EhrLabs.SaveToDB(ListEhrLabs[i]);//SAVE
				for(int j=0;j<ListEhrLabs[i].ListEhrLabResults.Count;j++) {//EHR TRIGGER
					if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
						using FormCDSIntervention FormCDSI=new FormCDSIntervention();
						FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(ListEhrLabs[i].ListEhrLabResults[j],PatCur);
						FormCDSI.ShowIfRequired(false);
					}
				}
			}
			DialogResult=DialogResult.OK;
			//Done!
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		






	}
}
