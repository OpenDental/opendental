using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileMedication:FormODBase {
		public List<MedicationPat> ListMedicationPatNew;
		private List<MedicationPat> _listMedicationPatReconcile;
		private List<MedicationPat> _listMedicationPatCur;
		private List<Medication> _listMedicationCur;
		private Patient _patCur;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileMedication(Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
		}

		private void FormReconcileMedication_Load(object sender,EventArgs e) {
			for(int index=0;index<ListMedicationPatNew.Count;index++) {
				ListMedicationPatNew[index].PatNum=_patCur.PatNum;
			}
			FillExistingGrid();
			_listMedicationPatReconcile=new List<MedicationPat>(_listMedicationPatCur);
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
			for(int i=0;i<ListMedicationPatNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listMedicationPatReconcile.Count;j++) {
					if(_listMedicationPatReconcile[j].RxCui==ListMedicationPatNew[i].RxCui) {
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatReconcile.Add(ListMedicationPatNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridMedImport.BeginUpdate();
			gridMedImport.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridMedImport.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedImport.ListGridColumns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedImport.ListGridColumns.Add(col);
			col=new GridColumn("Description",220);
			gridMedImport.ListGridColumns.Add(col);
			gridMedImport.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListMedicationPatNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListMedicationPatNew[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatNew[i].DateStart.ToShortDateString());
				}
				if(ListMedicationPatNew[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatNew[i].DateStop.ToShortDateString());
				}
				if(ListMedicationPatNew[i].MedDescript==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListMedicationPatNew[i].MedDescript);
				}
				gridMedImport.ListGridRows.Add(row);
			}
			gridMedImport.EndUpdate();
		}

		private void FillExistingGrid() {
			gridMedExisting.BeginUpdate();
			gridMedExisting.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridMedExisting.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedExisting.ListGridColumns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedExisting.ListGridColumns.Add(col);
			col=new GridColumn("Description",320);
			gridMedExisting.ListGridColumns.Add(col);
			gridMedExisting.ListGridRows.Clear();
			_listMedicationPatCur=MedicationPats.GetMedPatsForReconcile(_patCur.PatNum);
			List<long> medicationNums=new List<long>();
			for(int h=0;h<_listMedicationPatCur.Count;h++) {
				if(_listMedicationPatCur[h].MedicationNum > 0) {
					medicationNums.Add(_listMedicationPatCur[h].MedicationNum);
				}
			}
			_listMedicationCur=Medications.GetMultMedications(medicationNums);
			GridRow row;
			Medication med;
			for(int i=0;i<_listMedicationPatCur.Count;i++) {
				row=new GridRow();
				med=Medications.GetMedication(_listMedicationPatCur[i].MedicationNum);//Possibly change if we decided to postpone caching medications
				row.Cells.Add(_listMedicationPatCur[i].DateTStamp.ToShortDateString());
				if(_listMedicationPatCur[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatCur[i].DateStart.ToShortDateString());
				}
				if(_listMedicationPatCur[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatCur[i].DateStop.ToShortDateString());
				}
				if(med.MedName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(med.MedName);
				}
				gridMedExisting.ListGridRows.Add(row);
			}
			gridMedExisting.EndUpdate();
		}

		private void FillReconcileGrid() {
			gridMedReconcile.BeginUpdate();
			gridMedReconcile.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",130,HorizontalAlignment.Center);
			gridMedReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridMedReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Date Stop",100,HorizontalAlignment.Center);
			gridMedReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Description",350);
			gridMedReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Notes",150);
			gridMedReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Is Incoming",50,HorizontalAlignment.Center);
			gridMedReconcile.ListGridColumns.Add(col);
			gridMedReconcile.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMedicationPatReconcile.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(_listMedicationPatReconcile[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatReconcile[i].DateStart.ToShortDateString());
				}
				if(_listMedicationPatReconcile[i].DateStop.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatReconcile[i].DateStop.ToShortDateString());
				}
				if(_listMedicationPatReconcile[i].IsNew) {
					if(_listMedicationPatReconcile[i].MedDescript==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_listMedicationPatReconcile[i].MedDescript);
					}
				}
				else {
					Medication med=Medications.GetMedication(_listMedicationPatReconcile[i].MedicationNum);
					if(med.MedName==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(med.MedName);
					}
				}
				if(_listMedicationPatReconcile[i].PatNote==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listMedicationPatReconcile[i].PatNote);
				}
				row.Cells.Add(_listMedicationPatReconcile[i].IsNew?"X":"");
				gridMedReconcile.ListGridRows.Add(row);
			}
			gridMedReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridMedImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			MedicationPat medP;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridMedImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridMedImport and ListMedicationPatNew are a 1:1 list we can use the selected index position to get our medP
				medP=ListMedicationPatNew[gridMedImport.SelectedIndices[i]];
				for(int j=0;j<gridMedReconcile.ListGridRows.Count;j++) {
					if(medP.RxCui > 0 && medP.RxCui==_listMedicationPatReconcile[j].RxCui) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatReconcile.Add(medP);
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
			MedicationPat medP;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridMedExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridMedImport and ListMedicationPatNew are a 1:1 list we can use the selected index position to get our medP
				medP=_listMedicationPatCur[gridMedExisting.SelectedIndices[i]];
				for(int j=0;j<gridMedReconcile.ListGridRows.Count;j++) {
					if(medP.RxCui > 0 && medP.RxCui==_listMedicationPatReconcile[j].RxCui) {
						isValid=false;
						skipCount++;
						break;
					}
					if(medP.MedicationNum==_listMedicationPatReconcile[j].MedicationNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listMedicationPatReconcile.Add(medP);
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
			MedicationPat medP;
			for(int i=gridMedReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				medP=_listMedicationPatReconcile[gridMedReconcile.SelectedIndices[i]];
				_listMedicationPatReconcile.Remove(medP);
			}
			FillReconcileGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listMedicationPatReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing medications to be removed.  Continue?")) {
					return;
				}
			}
			MedicationPat medP;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listMedicationPatCur.Count;i++) {//Start looping through all current medications
				isActive=false;
				medP=_listMedicationPatCur[i];
				for(int j=0;j<_listMedicationPatReconcile.Count;j++) {//Compare each reconcile medication to the current medication
					if(medP.RxCui > 0 && medP.RxCui==_listMedicationPatReconcile[j].RxCui && _listMedicationPatReconcile[j].MedicationNum==_listMedicationPatCur[i].MedicationNum) {//Has an RxNorm code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {//Update current medications.
					_listMedicationPatCur[i].DateStop=DateTime.Now;//Set the current DateStop to today (to set the medication as discontinued)
					MedicationPats.Update(_listMedicationPatCur[i]);
				}
			}
			//Always update every current medication for the patient so that DateTStamp reflects the last reconcile date.
			if(_listMedicationPatCur.Count>0) {
				MedicationPats.ResetTimeStamps(_patCur.PatNum,true);
			}
			Medication med;
			int index;
			for(int j=0;j<_listMedicationPatReconcile.Count;j++) {
				index=ListMedicationPatNew.IndexOf(_listMedicationPatReconcile[j]);
				if(index<0) {
					continue;
				}
				if(_listMedicationPatReconcile[j]==ListMedicationPatNew[index]) {
					med=Medications.GetMedicationFromDbByRxCui(_listMedicationPatReconcile[j].RxCui);
					if(med==null) {
						med=new Medication();
						med.MedName=ListMedicationPatNew[index].MedDescript;
						med.RxCui=ListMedicationPatNew[index].RxCui;
						ListMedicationPatNew[index].MedicationNum=Medications.Insert(med);
						med.GenericNum=med.MedicationNum;
						Medications.Update(med);
					}
					else {
						ListMedicationPatNew[index].MedicationNum=med.MedicationNum;
					}
					ListMedicationPatNew[index].ProvNum=0;//Since imported, set provnum to 0 so it does not affect CPOE.
					MedicationPats.Insert(ListMedicationPatNew[index]);
				}
			}
			EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.EventType=EhrMeasureEventType.MedicationReconcile;
			newMeasureEvent.PatNum=_patCur.PatNum;
			newMeasureEvent.MoreInfo="";
			EhrMeasureEvents.Insert(newMeasureEvent);
			for(int inter=0;inter<_listMedicationPatReconcile.Count;inter++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) {
					Medication medInter=Medications.GetMedicationFromDbByRxCui(_listMedicationPatReconcile[inter].RxCui);
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(medInter,_patCur);
					FormCDSI.ShowIfRequired(false);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}