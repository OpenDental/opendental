using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEhrLabOrders:FormODBase {
		public List<EhrLab> ListEhrLabs;
		public Patient PatCur;

		public FormEhrLabOrders() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEhrLabOrders_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Date Time",80,HorizontalAlignment.Center);//Formatted yyyyMMdd
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Placer Order Number",130,HorizontalAlignment.Center);//Should be PK but might not be. Instead use Placer Order Num.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Filler Order Number",130,HorizontalAlignment.Center);//Should be PK but might not be. Instead use Placer Order Num.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Test Performed",430);//Should be PK but might not be. Instead use Placer Order Num.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Results In",80,HorizontalAlignment.Center);//Or date of latest result? or both?
			gridMain.ListGridColumns.Add(col);
			ListEhrLabs = EhrLabs.GetAllForPat(PatCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListEhrLabs.Count;i++) {
				row=new GridRow();
				string dateSt=ListEhrLabs[i].ResultDateTime.PadRight(8,'0').Substring(0,8);//stored in DB as yyyyMMddhhmmss-zzzz
				DateTime dateT=PIn.Date(dateSt.Substring(4,2)+"/"+dateSt.Substring(6,2)+"/"+dateSt.Substring(0,4));
				row.Cells.Add(dateT.ToShortDateString());//date only
				row.Cells.Add(ListEhrLabs[i].PlacerOrderNum);
				row.Cells.Add(ListEhrLabs[i].FillerOrderNum);
				row.Cells.Add(ListEhrLabs[i].UsiText);
				row.Cells.Add(ListEhrLabs[i].ListEhrLabResults.Count.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butImport_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste MBCP = new MsgBoxCopyPaste("Paste HL7 Lab Message Text Here.");
			MBCP.textMain.SelectAll();
			MBCP.ShowDialog();
			if(MBCP.DialogResult!=DialogResult.OK) {
				return;
			}
			List<EhrLab> listEhrLabs;
			try {
				listEhrLabs=EhrLabs.ProcessHl7Message(MBCP.textMain.Text);//Not a typical use of the msg box copy paste
				if(listEhrLabs[0].PatNum==PatCur.PatNum) {//only need to check the first lab.
					//nothing to do here. Imported lab matches the current patient.
				}
				else{//does not match current patient, redirect to import form which displays patient information and is build for importing.
					using FormEhrLabOrderImport FormLOI=new FormEhrLabOrderImport();
					FormLOI.PatCur=PatCur;
					FormLOI.Hl7LabMessage=MBCP.textMain.Text;
					FormLOI.ShowDialog();
					FillGrid();
					return;
				}
				//else if(listEhrLabs[0].PatNum==0) {
				//	if(MessageBox.Show("Lab patient does not match current patient. Lab patient name is "
				//		+MBCP.textMain.Text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries)[1].Split('|')[5].Split('~')[0].Split('^')[1]+" "//first name
				//		+MBCP.textMain.Text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries)[1].Split('|')[5].Split('~')[0].Split('^')[1]+" "//last name
				//		+"\r\nWould you like to import lab for the current patient?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) 
				//	{
				//		return;
				//	}
				//	//User agreed to import current lab(s) for current patient.
				//	for(int i=0;i<listEhrLabs.Count;i++) {
				//		listEhrLabs[i].PatNum=PatCur.PatNum;
				//		//TODO: Import external OIDs and PatIDs so that we can identify this patient next time.
				//	}
				//}
				//else {//Patnum is already associated with another patient.
				//	MessageBox.Show("This lab contains patient information for a different patient. Lab patient name is "
				//		+MBCP.textMain.Text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries)[1].Split('|')[5].Split('~')[0].Split('^')[1]+" "//first name
				//		+MBCP.textMain.Text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries)[1].Split('|')[5].Split('~')[0].Split('^')[1]);
				//	return;
				//}
			}
			catch (Exception Ex){
				MessageBox.Show(this,"Unable to import lab.\r\n"+Ex.Message);
				return;
			}
			for(int i=0;i<listEhrLabs.Count;i++) {
				EhrLab tempLab=null;//lab from DB if it exists.
				tempLab=EhrLabs.GetByGUID(listEhrLabs[i].PlacerOrderUniversalID,listEhrLabs[i].PlacerOrderNum);
				if(tempLab==null){
					tempLab=EhrLabs.GetByGUID(listEhrLabs[i].FillerOrderUniversalID,listEhrLabs[i].FillerOrderNum);
				}
				if(tempLab!=null) {
					//Date validation.
					//if(tempLab.ResultDateTime.CompareTo(listEhrLabs[i].ResultDateTime)<=0) {//string compare dates will return 1+ if tempLab Date is greater.
					//	MsgBox.Show(this,"This lab already exists in the database and has a more recent timestamp.");
					//	continue;
					//}
					//TODO: The code above works, but ignores more recent lab results. Although the lab order my be unchanged there may be updated lab results.
					//It would be better to check for updated results, unfortunately results have no unique identifiers.
				}
				Provider prov=Providers.GetProv(Security.CurUser.ProvNum);
				if(Security.CurUser.ProvNum!=0 && EhrProvKeys.GetKeysByFLName(prov.LName,prov.FName).Count>0) {//The user who is currently logged in is a provider and has a valid EHR key.
					ListEhrLabs[i].IsCpoe=true;
				}
				listEhrLabs[i]=EhrLabs.SaveToDB(listEhrLabs[i]);//SAVE
				for(int j=0;j<listEhrLabs[i].ListEhrLabResults.Count;j++) {//EHR TRIGGER
					if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
						using FormCDSIntervention FormCDSI=new FormCDSIntervention();
						FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(listEhrLabs[i].ListEhrLabResults[j],PatCur);
						FormCDSI.ShowIfRequired(false);
					}
				}
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEhrLabOrderEdit2014 FormLOE=new FormEhrLabOrderEdit2014();
			FormLOE.EhrLabCur=new EhrLab();
			FormLOE.EhrLabCur.PatNum=PatCur.PatNum;
			FormLOE.IsNew=true;
			FormLOE.ShowDialog();
			if(FormLOE.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.EventType=EhrMeasureEventType.CPOE_LabOrdered;//default
			Loinc loinc=Loincs.GetByCode(FormLOE.EhrLabCur.UsiID);
			if(loinc!=null && loinc.ClassType=="RAD") {//short circuit logic
				newMeasureEvent.EventType=EhrMeasureEventType.CPOE_RadOrdered;
			}
			newMeasureEvent.PatNum=FormLOE.EhrLabCur.PatNum;
			newMeasureEvent.MoreInfo="";
			newMeasureEvent.FKey=FormLOE.EhrLabCur.EhrLabNum;
			EhrMeasureEvents.Insert(newMeasureEvent);
			EhrLabs.SaveToDB(FormLOE.EhrLabCur);
			for(int i=0;i<FormLOE.EhrLabCur.ListEhrLabResults.Count;i++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(FormLOE.EhrLabCur.ListEhrLabResults[i],PatCur);
					FormCDSI.ShowIfRequired(false);
				}
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabOrderEdit2014 FormLOE=new FormEhrLabOrderEdit2014();
			FormLOE.EhrLabCur=ListEhrLabs[e.Row];
			FormLOE.ShowDialog();
			if(FormLOE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
			//TODO:maybe add more code here for when we come back from form... In case we delete a lab from the form.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		






	}
}
