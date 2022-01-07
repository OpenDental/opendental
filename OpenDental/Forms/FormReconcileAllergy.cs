using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileAllergy:FormODBase {
		public List<AllergyDef> ListAllergyDefNew;
		public List<Allergy> ListAllergyNew;
		private List<Allergy> _listAllergyReconcile;
		private List<AllergyDef> _listAllergyDefCur;
		private List<Allergy> _listAllergyCur;
		private Patient _patCur;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileAllergy(Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
		}

		private void FormReconcileAllergy_Load(object sender,EventArgs e) {
			for(int index=0;index<ListAllergyNew.Count;index++) {
				ListAllergyNew[index].PatNum=_patCur.PatNum;
			}
			FillExistingGrid();//Done first so that _listAllergyCur and _listAllergyDefCur are populated.
			_listAllergyReconcile=new List<Allergy>(_listAllergyCur);
			#region Delete after testing
			//-------------------------------Delete after testing
			//ListAllergyNew=new List<Allergy>();
			//Allergy al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//al.SnomedReaction="51242-b";
			//al.Reaction="Hives";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//al.SnomedReaction="66452-b";
			//al.Reaction="Chaffing";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(10));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(10));
			//al.SnomedReaction="48518475-b";
			//al.Reaction="Shivers";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//al.SnomedReaction="5984145-b";
			//al.Reaction="Vomiting";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(9));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(9));
			//al.SnomedReaction="5461238-b";
			//al.Reaction="Swelling";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//al.SnomedReaction="253-b";
			//al.Reaction="Yuck";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(12));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(12));
			//al.SnomedReaction="45451-b";
			//al.Reaction="Epic Swelling";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//al.SnomedReaction="511232-b";
			//al.Reaction="Rashes";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//al=new Allergy();
			//al.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//al.DateAdverseReaction=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//al.SnomedReaction="986321-b";
			//al.Reaction="Death";
			//al.StatusIsActive=true;
			//al.PatNum=PatCur.PatNum;
			//al.IsNew=true;
			//ListAllergyNew.Add(al);
			//ListAllergyDefNew=new List<AllergyDef>();
			//AllergyDef ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//ald.SnomedAllergyTo="51242";
			//ald.SnomedType=SnomedAllergy.FoodIntolerance;
			//ald.Description="Allergy - Milk";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//ald.SnomedAllergyTo="66452";
			//ald.SnomedType=SnomedAllergy.DrugIntolerance;
			//ald.Description="Allergy - Ibuprofen";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(10));
			//ald.SnomedAllergyTo="48518475";
			//ald.SnomedType=SnomedAllergy.AllergyToSubstance;
			//ald.Description="Allergy - Alcohol";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//ald.SnomedAllergyTo="5984145";
			//ald.SnomedType=SnomedAllergy.DrugAllergy;
			//ald.Description="Allergy - Morphine";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(9));
			//ald.SnomedAllergyTo="5461238";
			//ald.SnomedType=SnomedAllergy.AdverseReactionsToFood;
			//ald.Description="Allergy - Nuts";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//ald.SnomedAllergyTo="253";
			//ald.SnomedType=SnomedAllergy.FoodAllergy;
			//ald.Description="Allergy - Tomatoes";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(12));
			//ald.SnomedAllergyTo="45451";
			//ald.SnomedType=SnomedAllergy.AllergyToSubstance;
			//ald.Description="Allergy - Bees";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//ald.SnomedAllergyTo="511232";
			//ald.SnomedType=SnomedAllergy.AllergyToSubstance;
			//ald.Description="Allergy - Latex";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//ald=new AllergyDef();
			//ald.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//ald.SnomedAllergyTo="986321";
			//ald.SnomedType=SnomedAllergy.AdverseReactionsToSubstance;
			//ald.Description="Allergy - Air";
			//ald.IsNew=true;
			//ListAllergyDefNew.Add(ald);
			//-------------------------------
			#endregion
			//Automation to initially fill reconcile grid with "recommended" rows.
			bool isValid;
			for(int i=0;i<ListAllergyNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listAllergyDefCur.Count;j++) {
					//if(_listAllergyDefCur[j].SnomedAllergyTo==ListAllergyDefNew[i].SnomedAllergyTo) {//TODO: Change to UNII
					//	isValid=false;
					//	break;
					//}
					if(_listAllergyDefCur[j].MedicationNum==ListAllergyDefNew[i].MedicationNum) {//Check Medications to determine if the Reconcile list already has that MedicationNum
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listAllergyReconcile.Add(ListAllergyNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridAllergyImport.BeginUpdate();
			gridAllergyImport.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyImport.ListGridColumns.Add(col);
			col=new GridColumn("Description",200);
			gridAllergyImport.ListGridColumns.Add(col);
			col=new GridColumn("Reaction",100);
			gridAllergyImport.ListGridColumns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyImport.ListGridColumns.Add(col);
			gridAllergyImport.ListGridRows.Clear();
			GridRow row;
			//ListAllergyNew and ListAllergyDefNew should be a 1:1 ratio so we can use the same loop for both.
			for(int i=0;i<ListAllergyNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListAllergyDefNew[i].Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListAllergyDefNew[i].Description);
				}
				if(ListAllergyNew[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListAllergyNew[i].Reaction);
				}
				if(ListAllergyNew[i].StatusIsActive) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				gridAllergyImport.ListGridRows.Add(row);
			}
			gridAllergyImport.EndUpdate();
		}

		private void FillExistingGrid() {
			gridAllergyExisting.BeginUpdate();
			gridAllergyExisting.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyExisting.ListGridColumns.Add(col);
			col=new GridColumn("Description",200);
			gridAllergyExisting.ListGridColumns.Add(col);
			col=new GridColumn("Reaction",100);
			gridAllergyExisting.ListGridColumns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyExisting.ListGridColumns.Add(col);
			gridAllergyExisting.ListGridRows.Clear();
			_listAllergyCur=Allergies.GetAll(_patCur.PatNum,false);
			List<long> allergyDefNums=new List<long>();
			for(int h=0;h<_listAllergyCur.Count;h++) {
				if(_listAllergyCur[h].AllergyDefNum > 0) {
					allergyDefNums.Add(_listAllergyCur[h].AllergyDefNum);
				}
			}
			_listAllergyDefCur=AllergyDefs.GetMultAllergyDefs(allergyDefNums);
			GridRow row;
			AllergyDef ald;
			for(int i=0;i<_listAllergyCur.Count;i++) {
				row=new GridRow();
				ald=new AllergyDef();
				ald=AllergyDefs.GetOne(_listAllergyCur[i].AllergyDefNum,_listAllergyDefCur);
				row.Cells.Add(_listAllergyCur[i].DateTStamp.ToShortDateString());
				if(ald.Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ald.Description);
				}
				if(_listAllergyCur[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listAllergyCur[i].Reaction);
				}
				if(_listAllergyCur[i].StatusIsActive) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				gridAllergyExisting.ListGridRows.Add(row);
			}
			gridAllergyExisting.EndUpdate();
		}

		private void FillReconcileGrid() {
			gridAllergyReconcile.BeginUpdate();
			gridAllergyReconcile.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Description",400);
			gridAllergyReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Reaction",300);
			gridAllergyReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Is Incoming",100,HorizontalAlignment.Center);
			gridAllergyReconcile.ListGridColumns.Add(col);
			gridAllergyReconcile.ListGridRows.Clear();
			GridRow row;
			AllergyDef ald=new AllergyDef();
			for(int i=0;i<_listAllergyReconcile.Count;i++) {
				row=new GridRow();
				ald=new AllergyDef();
				if(_listAllergyReconcile[i].IsNew) {
					//To find the allergy def for new allergies, get the index of the matching allergy in ListAllergyNew, and use that index in ListAllergyDefNew because they are 1 to 1 lists.
					ald=ListAllergyDefNew[ListAllergyNew.IndexOf(_listAllergyReconcile[i])];
				}
				for(int j=0;j<_listAllergyDefCur.Count;j++) {
					if(_listAllergyReconcile[i].AllergyDefNum > 0 && _listAllergyReconcile[i].AllergyDefNum==_listAllergyDefCur[j].AllergyDefNum) {
						ald=_listAllergyDefCur[j];//Gets the allergydef matching the allergy so we can use it to populate the grid
						break;
					}
				}
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ald.Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ald.Description);
				}
				if(_listAllergyReconcile[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listAllergyReconcile[i].Reaction);
				}
				if(_listAllergyReconcile[i].StatusIsActive) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				row.Cells.Add(_listAllergyReconcile[i].IsNew?"X":"");
				gridAllergyReconcile.ListGridRows.Add(row);
			}
			gridAllergyReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridAllergyImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Allergy al;
			AllergyDef alD;
			AllergyDef alDR=null;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridAllergyImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridAllergyImport and ListAllergyNew are a 1:1 list we can use the selected index position to get our allergy
				al=ListAllergyNew[gridAllergyImport.SelectedIndices[i]];
				alD=ListAllergyDefNew[gridAllergyImport.SelectedIndices[i]];//ListAllergyDefNew is also a 1:1 to gridAllergyImport.
				for(int j=0;j<_listAllergyReconcile.Count;j++) {
					if(_listAllergyReconcile[j].IsNew) {
						alDR=ListAllergyDefNew[ListAllergyNew.IndexOf(_listAllergyReconcile[j])];
					}
					else {
						alDR=AllergyDefs.GetOne(_listAllergyReconcile[j].AllergyDefNum);
					}
					if(alDR==null) {
						continue;
					}
					//if(alDR.SnomedAllergyTo!="" && alDR.SnomedAllergyTo!=null && alDR.SnomedAllergyTo==alD.SnomedAllergyTo) {//TODO: Change to UNII
					//	isValid=false;
					//	skipCount++;
					//	break;
					//}
					if(alDR.MedicationNum!=0 && alDR.MedicationNum==alD.MedicationNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listAllergyReconcile.Add(al);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because allergy already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butAddExist_Click(object sender,EventArgs e) {
			if(gridAllergyExisting.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Allergy al;
			AllergyDef alD;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridAllergyExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridAllergyExisting and _listAllergyCur are a 1:1 list we can use the selected index position to get our allergy
				al=_listAllergyCur[gridAllergyExisting.SelectedIndices[i]];
				alD=AllergyDefs.GetOne(al.AllergyDefNum,_listAllergyDefCur);
				if(_listAllergyReconcile.Count==0) {
					_listAllergyReconcile.Add(al);
					continue;
				}
				for(int j=0;j<_listAllergyReconcile.Count;j++) {
					if(!_listAllergyReconcile[j].IsNew && _listAllergyReconcile[j].AllergyNum==al.AllergyNum) {//If not new, then from existing list.  Check allergynums
						isValid=false;
						skipCount++;
						break;
					}
					if(_listAllergyReconcile[j].IsNew) {
						//This is an allergy that is coming in from and external source.
						AllergyDef alDN=null;
						int index=ListAllergyNew.IndexOf(_listAllergyReconcile[j]);//Find the corresponding allergy def by looping through the incoming allergies.
						if(index==-1) {
							continue;//This should not happen
						}
						alDN=ListAllergyDefNew[index];//Incoming allergy and allergy def lists are 1 to 1 so we can use the same index.
						if(alDN!=null && alDN.MedicationNum==alD.MedicationNum) {
							isValid=false;
							skipCount++;
							break;
						}
					}
					if(al.AllergyDefNum==_listAllergyReconcile[j].AllergyDefNum) {
						isValid=false;
						skipCount++;
						break;
					}
					if(!isValid) {
						break;
					}
				}
				if(isValid) {
					_listAllergyReconcile.Add(al);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because allergy already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butRemoveRec_Click(object sender,EventArgs e) {
			if(gridAllergyReconcile.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to remove");
				return;
			}
			Allergy al;
			for(int i=gridAllergyReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				al=_listAllergyReconcile[gridAllergyReconcile.SelectedIndices[i]];
				_listAllergyReconcile.Remove(al);
			}
			FillReconcileGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listAllergyReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing allergies to be removed.  Continue?")) {
					return;
				}
			}
			Allergy al;
			AllergyDef alD;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listAllergyCur.Count;i++) {//Start looping through all current allergies
				isActive=false;
				al=_listAllergyCur[i];
				alD=AllergyDefs.GetOne(al.AllergyDefNum,_listAllergyDefCur);
				for(int j=0;j<_listAllergyReconcile.Count;j++) {//Compare each reconcile allergy to the current allergy
					AllergyDef alDR=AllergyDefs.GetOne(_listAllergyReconcile[j].AllergyDefNum,_listAllergyDefCur);
					if(_listAllergyReconcile[j].AllergyDefNum==_listAllergyCur[i].AllergyDefNum) {//Has identical AllergyDefNums
						isActive=true;
						break;
					}
					if(alDR==null) {
						continue;
					}
					//if(alDR.SnomedAllergyTo!="" && alDR.SnomedAllergyTo==alD.SnomedAllergyTo) {//TODO: Change to UNII
					//	isActive=true;
					//	break;
					//}
					if(alDR.MedicationNum!=0 && alDR.MedicationNum==alD.MedicationNum) {//Has a Snomed code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {
					_listAllergyCur[i].StatusIsActive=isActive;
					Allergies.Update(_listAllergyCur[i]);
				}
			}
			//Always update every current allergy for the patient so that DateTStamp reflects the last reconcile date.
			if(_listAllergyCur.Count>0) {
				Allergies.ResetTimeStamps(_patCur.PatNum,true);
			}
			AllergyDef alDU;
			int index;
			for(int j=0;j<_listAllergyReconcile.Count;j++) {
				if(!_listAllergyReconcile[j].IsNew) {
					continue;
				}
				index=ListAllergyNew.IndexOf(_listAllergyReconcile[j]);//Returns -1 if not found.
				if(index<0) {
					continue;
				}
				//Insert the AllergyDef and Allergy if needed.
				if(ListAllergyDefNew[index].MedicationNum!=0) {
					alDU=AllergyDefs.GetAllergyDefFromMedication(ListAllergyDefNew[index].MedicationNum);
				}
				else {
					alDU=null;//remove once UNII is implemented
					//alDU=AllergyDefs.GetAllergyDefFromCode(ListAllergyDefNew[index].SnomedAllergyTo);//TODO: Change to UNII
				}
				if(alDU==null) {//db is missing the def
					ListAllergyNew[index].AllergyDefNum=AllergyDefs.Insert(ListAllergyDefNew[index]);
				}
				else {
					ListAllergyNew[index].AllergyDefNum=alDU.AllergyDefNum;//Set the allergydefnum on the allergy.
				}
				Allergies.Insert(ListAllergyNew[index]);
			}
			//TODO: Make an allergy measure event if one is needed for MU3.
			//EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			//newMeasureEvent.DateTEvent=DateTime.Now;
			//newMeasureEvent.EventType=EhrMeasureEventType.AllergyReconcile;
			//newMeasureEvent.PatNum=PatCur.PatNum;
			//newMeasureEvent.MoreInfo="";
			//EhrMeasureEvents.Insert(newMeasureEvent);
			for(int inter=0;inter<_listAllergyReconcile.Count;inter++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) {
					AllergyDef alDInter=AllergyDefs.GetOne(_listAllergyReconcile[inter].AllergyDefNum);
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(alDInter,_patCur);
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