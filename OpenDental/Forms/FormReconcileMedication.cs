using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileMedication:FormODBase {
		public List<MedicationPat> ListMedicationPatsNew;
		private List<MedicationPat> _listMedicationPatsReconcile;
		private List<MedicationPat> _listMedicationPats;
		private List<Medication> _listMedications;
		private Patient _patient;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileMedication(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		private void FormReconcileMedication_Load(object sender,EventArgs e) {
			for(int i=0;i<ListMedicationPatsNew.Count;i++) {
				ListMedicationPatsNew[i].PatNum=_patient.PatNum;
			}
			FillExistingGrid();
			_listMedicationPatsReconcile=new List<MedicationPat>(_listMedicationPats);
			#region Delete after testing
			//ListMedicationPatNew=new List<MedicationPat>();
			//MedicationPat medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//medP.MedDescript="Valpax";
			//medP.PatNote="Two a day";
			//medP.RxCui=542687;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//medP.MedDescript="Usept";
			//medP.PatNote="Three a day";
			//medP.RxCui=405384;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(1));
			//medP.MedDescript="SmileGuard";
			//medP.PatNote="Two a day";
			//medP.RxCui=1038751;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//medP.MedDescript="Slozem";
			//medP.PatNote="One a day";
			//medP.RxCui=151154;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(6));
			//medP.MedDescript="Prax";
			//medP.PatNote="Four a day";
			//medP.RxCui=219336;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//medP.MedDescript="PrameGel";
			//medP.PatNote="Two a day";
			//medP.RxCui=93822;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//medP.MedDescript="Pramotic";
			//medP.PatNote="Five a day";
			//medP.RxCui=405268;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(3));
			//medP.MedDescript="Medetomidine";
			//medP.PatNote="Three a day";
			//medP.RxCui=52016;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			//medP=new MedicationPat();
			//medP.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//medP.MedDescript="Medcodin";
			//medP.PatNote="One a day";
			//medP.RxCui=218274;
			//medP.IsNew=true;
			//medP.PatNum=PatCur.PatNum;
			//ListMedicationPatNew.Add(medP);
			#endregion
			//Automation to initially fill reconcile grid with "recommended" rows.
			bool isValid;
			for(int i=0;i<ListMedicationPatsNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listMedicationPatsReconcile.Count;j++) {
					if(_listMedicationPatsReconcile[j].RxCui==ListMedicationPatsNew[i].RxCui) {
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatsReconcile.Add(ListMedicationPatsNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridMedImport.BeginUpdate();
			gridMedImport.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridMedImport.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedImport.Columns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedImport.Columns.Add(col);
			col=new GridColumn("Description",220);
			gridMedImport.Columns.Add(col);
			gridMedImport.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListMedicationPatsNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListMedicationPatsNew[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatsNew[i].DateStart.ToShortDateString());
				}
				if(ListMedicationPatsNew[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatsNew[i].DateStop.ToShortDateString());
				}
				if(ListMedicationPatsNew[i].MedDescript==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatsNew[i].MedDescript);
				}
				gridMedImport.ListGridRows.Add(row);
			}
			gridMedImport.EndUpdate();
		}

		private void FillExistingGrid() {
			gridMedExisting.BeginUpdate();
			gridMedExisting.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridMedExisting.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedExisting.Columns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedExisting.Columns.Add(col);
			col=new GridColumn("Description",320);
			gridMedExisting.Columns.Add(col);
			gridMedExisting.ListGridRows.Clear();
			_listMedicationPats=MedicationPats.GetMedPatsForReconcile(_patient.PatNum);
			List<long> medicationNums=new List<long>();
			for(int i=0;i<_listMedicationPats.Count;i++) {
				if(_listMedicationPats[i].MedicationNum > 0) {
					medicationNums.Add(_listMedicationPats[i].MedicationNum);
				}
			}
			_listMedications=Medications.GetMultMedications(medicationNums);
			GridRow row;
			Medication medication;
			for(int i=0;i<_listMedicationPats.Count;i++) {
				row=new GridRow();
				medication=Medications.GetMedication(_listMedicationPats[i].MedicationNum);//Possibly change if we decided to postpone caching medications
				row.Cells.Add(_listMedicationPats[i].DateTStamp.ToShortDateString());
				if(_listMedicationPats[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPats[i].DateStart.ToShortDateString());
				}
				if(_listMedicationPats[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPats[i].DateStop.ToShortDateString());
				}
				if(medication.MedName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(medication.MedName);
				}
				gridMedExisting.ListGridRows.Add(row);
			}
			gridMedExisting.EndUpdate();
		}

		private void FillReconcileGrid() {
			gridMedReconcile.BeginUpdate();
			gridMedReconcile.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",130,HorizontalAlignment.Center);
			gridMedReconcile.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedReconcile.Columns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedReconcile.Columns.Add(col);
			col=new GridColumn("Description",350);
			gridMedReconcile.Columns.Add(col);
			col=new GridColumn("Notes",150);
			gridMedReconcile.Columns.Add(col);
			col=new GridColumn("Is Incoming",50,HorizontalAlignment.Center);
			gridMedReconcile.Columns.Add(col);
			gridMedReconcile.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMedicationPatsReconcile.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(_listMedicationPatsReconcile[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatsReconcile[i].DateStart.ToShortDateString());
				}
				if(_listMedicationPatsReconcile[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatsReconcile[i].DateStop.ToShortDateString());
				}
				if(_listMedicationPatsReconcile[i].IsNew) {
					if(_listMedicationPatsReconcile[i].MedDescript==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_listMedicationPatsReconcile[i].MedDescript);
					}
				}
				else {
					Medication medication=Medications.GetMedication(_listMedicationPatsReconcile[i].MedicationNum);
					if(medication.MedName==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(medication.MedName);
					}
				}
				if(_listMedicationPatsReconcile[i].PatNote==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatsReconcile[i].PatNote);
				}
				row.Cells.Add(_listMedicationPatsReconcile[i].IsNew?"X":"");
				gridMedReconcile.ListGridRows.Add(row);
			}
			gridMedReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridMedImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			MedicationPat medicationPat;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridMedImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridMedImport and ListMedicationPatNew are a 1:1 list we can use the selected index position to get our medP
				medicationPat=ListMedicationPatsNew[gridMedImport.SelectedIndices[i]];
				for(int j=0;j<gridMedReconcile.ListGridRows.Count;j++) {
					if(medicationPat.RxCui > 0 && medicationPat.RxCui==_listMedicationPatsReconcile[j].RxCui) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatsReconcile.Add(medicationPat);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because medication already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butAddExist_Click(object sender,EventArgs e) {
			if(gridMedExisting.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			MedicationPat medicationPat;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridMedExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridMedImport and ListMedicationPatNew are a 1:1 list we can use the selected index position to get our medP
				medicationPat=_listMedicationPats[gridMedExisting.SelectedIndices[i]];
				for(int j=0;j<gridMedReconcile.ListGridRows.Count;j++) {
					if(medicationPat.RxCui > 0 && medicationPat.RxCui==_listMedicationPatsReconcile[j].RxCui) {
						isValid=false;
						skipCount++;
						break;
					}
					if(medicationPat.MedicationNum==_listMedicationPatsReconcile[j].MedicationNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatsReconcile.Add(medicationPat);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because medication already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butRemoveRec_Click(object sender,EventArgs e) {
			if(gridMedReconcile.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to remove");
				return;
			}
			MedicationPat medicationPat;
			for(int i=gridMedReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				medicationPat=_listMedicationPatsReconcile[gridMedReconcile.SelectedIndices[i]];
				_listMedicationPatsReconcile.Remove(medicationPat);
			}
			FillReconcileGrid();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_listMedicationPatsReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing medications to be removed.  Continue?")) {
					return;
				}
			}
			MedicationPat medicationPat;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listMedicationPats.Count;i++) {//Start looping through all current medications
				isActive=false;
				medicationPat=_listMedicationPats[i];
				for(int j=0;j<_listMedicationPatsReconcile.Count;j++) {//Compare each reconcile medication to the current medication
					if(medicationPat.RxCui > 0 && medicationPat.RxCui==_listMedicationPatsReconcile[j].RxCui && _listMedicationPatsReconcile[j].MedicationNum==_listMedicationPats[i].MedicationNum) {//Has an RxNorm code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {//Update current medications.
					_listMedicationPats[i].DateStop=DateTime.Now;//Set the current DateStop to today (to set the medication as discontinued)
					MedicationPats.Update(_listMedicationPats[i]);
				}
			}
			//Always update every current medication for the patient so that DateTStamp reflects the last reconcile date.
			if(_listMedicationPats.Count>0) {
				MedicationPats.ResetTimeStamps(_patient.PatNum,true);
			}
			Medication medication;
			int index;
			for(int i=0;i<_listMedicationPatsReconcile.Count;i++) {
				index=ListMedicationPatsNew.IndexOf(_listMedicationPatsReconcile[i]);
				if(index<0) {
					continue;
				}
				if(_listMedicationPatsReconcile[i]!=ListMedicationPatsNew[index]) {
					continue;
				}
				medication=Medications.GetMedicationFromDbByRxCui(_listMedicationPatsReconcile[i].RxCui);
				if(medication==null) {
					medication=new Medication();
					medication.MedName=ListMedicationPatsNew[index].MedDescript;
					medication.RxCui=ListMedicationPatsNew[index].RxCui;
					ListMedicationPatsNew[index].MedicationNum=Medications.Insert(medication);
					medication.GenericNum=medication.MedicationNum;
					Medications.Update(medication);
				}
				else {
					ListMedicationPatsNew[index].MedicationNum=medication.MedicationNum;
				}
				ListMedicationPatsNew[index].ProvNum=0;//Since imported, set provnum to 0 so it does not affect CPOE.
				MedicationPats.Insert(ListMedicationPatsNew[index]);
			}
			EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=DateTime.Now;
			ehrMeasureEvent.EventType=EhrMeasureEventType.MedicationReconcile;
			ehrMeasureEvent.PatNum=_patient.PatNum;
			ehrMeasureEvent.MoreInfo="";
			EhrMeasureEvents.Insert(ehrMeasureEvent);
			for(int i=0;i<_listMedicationPatsReconcile.Count;i++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) {
					Medication medicationInter=Medications.GetMedicationFromDbByRxCui(_listMedicationPatsReconcile[i].RxCui);
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(medicationInter,_patient);
					FormCDSI.ShowIfRequired();
				}
			}
			DialogResult=DialogResult.OK;
		}

	}
}