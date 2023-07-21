using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileAllergy:FormODBase {
		public List<AllergyDef> ListAllergyDefsNew;
		public List<Allergy> ListAllergiesNew;
		private List<Allergy> _listAllergiesReconcile;
		private List<AllergyDef> _listAllergyDefs;
		private List<Allergy> _listAllergies;
		private Patient _patient;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileAllergy(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		private void FormReconcileAllergy_Load(object sender,EventArgs e) {
			for(int i=0;i<ListAllergiesNew.Count;i++) {
				ListAllergiesNew[i].PatNum=_patient.PatNum;
			}
			FillExistingGrid();//Done first so that _listAllergyCur and _listAllergyDefCur are populated.
			_listAllergiesReconcile=new List<Allergy>(_listAllergies);
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
			for(int i=0;i<ListAllergiesNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listAllergyDefs.Count;j++) {
					//if(_listAllergyDefCur[j].SnomedAllergyTo==ListAllergyDefNew[i].SnomedAllergyTo) {//TODO: Change to UNII
					//	isValid=false;
					//	break;
					//}
					if(_listAllergyDefs[j].MedicationNum==ListAllergyDefsNew[i].MedicationNum) {//Check Medications to determine if the Reconcile list already has that MedicationNum
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listAllergiesReconcile.Add(ListAllergiesNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridAllergyImport.BeginUpdate();
			gridAllergyImport.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyImport.Columns.Add(col);
			col=new GridColumn("Description",200);
			gridAllergyImport.Columns.Add(col);
			col=new GridColumn("Reaction",100);
			gridAllergyImport.Columns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyImport.Columns.Add(col);
			gridAllergyImport.ListGridRows.Clear();
			GridRow row;
			//ListAllergyNew and ListAllergyDefNew should be a 1:1 ratio so we can use the same loop for both.
			for(int i=0;i<ListAllergiesNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListAllergyDefsNew[i].Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListAllergyDefsNew[i].Description);
				}
				if(ListAllergiesNew[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListAllergiesNew[i].Reaction);
				}
				if(ListAllergiesNew[i].StatusIsActive) {
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
			gridAllergyExisting.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyExisting.Columns.Add(col);
			col=new GridColumn("Description",200);
			gridAllergyExisting.Columns.Add(col);
			col=new GridColumn("Reaction",100);
			gridAllergyExisting.Columns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyExisting.Columns.Add(col);
			gridAllergyExisting.ListGridRows.Clear();
			_listAllergies=Allergies.GetAll(_patient.PatNum,false);
			List<long> listAllergyDefNums=new List<long>();
			for(int h=0;h<_listAllergies.Count;h++) {
				if(_listAllergies[h].AllergyDefNum > 0) {
					listAllergyDefNums.Add(_listAllergies[h].AllergyDefNum);
				}
			}
			_listAllergyDefs=AllergyDefs.GetMultAllergyDefs(listAllergyDefNums);
			GridRow row;
			AllergyDef allergyDef;
			for(int i=0;i<_listAllergies.Count;i++) {
				row=new GridRow();
				allergyDef=new AllergyDef();
				allergyDef=AllergyDefs.GetOne(_listAllergies[i].AllergyDefNum,_listAllergyDefs);
				row.Cells.Add(_listAllergies[i].DateTStamp.ToShortDateString());
				if(allergyDef.Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(allergyDef.Description);
				}
				if(_listAllergies[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listAllergies[i].Reaction);
				}
				if(_listAllergies[i].StatusIsActive) {
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
			gridAllergyReconcile.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",90,HorizontalAlignment.Center);
			gridAllergyReconcile.Columns.Add(col);
			col=new GridColumn("Description",400);
			gridAllergyReconcile.Columns.Add(col);
			col=new GridColumn("Reaction",300);
			gridAllergyReconcile.Columns.Add(col);
			col=new GridColumn("Inactive",80,HorizontalAlignment.Center);
			gridAllergyReconcile.Columns.Add(col);
			col=new GridColumn("Is Incoming",100,HorizontalAlignment.Center);
			gridAllergyReconcile.Columns.Add(col);
			gridAllergyReconcile.ListGridRows.Clear();
			GridRow row;
			AllergyDef allergyDef=new AllergyDef();
			for(int i=0;i<_listAllergiesReconcile.Count;i++) {
				row=new GridRow();
				allergyDef=new AllergyDef();
				if(_listAllergiesReconcile[i].IsNew) {
					//To find the allergy def for new allergies, get the index of the matching allergy in ListAllergyNew, and use that index in ListAllergyDefNew because they are 1 to 1 lists.
					allergyDef=ListAllergyDefsNew[ListAllergiesNew.IndexOf(_listAllergiesReconcile[i])];
				}
				for(int j=0;j<_listAllergyDefs.Count;j++) {
					if(_listAllergiesReconcile[i].AllergyDefNum > 0 && _listAllergiesReconcile[i].AllergyDefNum==_listAllergyDefs[j].AllergyDefNum) {
						allergyDef=_listAllergyDefs[j];//Gets the allergydef matching the allergy so we can use it to populate the grid
						break;
					}
				}
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(allergyDef.Description==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(allergyDef.Description);
				}
				if(_listAllergiesReconcile[i].Reaction==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listAllergiesReconcile[i].Reaction);
				}
				if(_listAllergiesReconcile[i].StatusIsActive) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				row.Cells.Add(_listAllergiesReconcile[i].IsNew?"X":"");
				gridAllergyReconcile.ListGridRows.Add(row);
			}
			gridAllergyReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridAllergyImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Allergy allergy;
			AllergyDef allergyDef;
			AllergyDef allergyDefR=null;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridAllergyImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridAllergyImport and ListAllergyNew are a 1:1 list we can use the selected index position to get our allergy
				allergy=ListAllergiesNew[gridAllergyImport.SelectedIndices[i]];
				allergyDef=ListAllergyDefsNew[gridAllergyImport.SelectedIndices[i]];//ListAllergyDefNew is also a 1:1 to gridAllergyImport.
				for(int j=0;j<_listAllergiesReconcile.Count;j++) {
					if(_listAllergiesReconcile[j].IsNew) {
						allergyDefR=ListAllergyDefsNew[ListAllergiesNew.IndexOf(_listAllergiesReconcile[j])];
					}
					else {
						allergyDefR=AllergyDefs.GetOne(_listAllergiesReconcile[j].AllergyDefNum);
					}
					if(allergyDefR==null) {
						continue;
					}
					//if(alDR.SnomedAllergyTo!="" && alDR.SnomedAllergyTo!=null && alDR.SnomedAllergyTo==alD.SnomedAllergyTo) {//TODO: Change to UNII
					//	isValid=false;
					//	skipCount++;
					//	break;
					//}
					if(allergyDefR.MedicationNum!=0 && allergyDefR.MedicationNum==allergyDef.MedicationNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listAllergiesReconcile.Add(allergy);
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
			Allergy allergy;
			AllergyDef allergyDef;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridAllergyExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridAllergyExisting and _listAllergyCur are a 1:1 list we can use the selected index position to get our allergy
				allergy=_listAllergies[gridAllergyExisting.SelectedIndices[i]];
				allergyDef=AllergyDefs.GetOne(allergy.AllergyDefNum,_listAllergyDefs);
				if(_listAllergiesReconcile.Count==0) {
					_listAllergiesReconcile.Add(allergy);
					continue;
				}
				for(int j=0;j<_listAllergiesReconcile.Count;j++) {
					if(!_listAllergiesReconcile[j].IsNew && _listAllergiesReconcile[j].AllergyNum==allergy.AllergyNum) {//If not new, then from existing list.  Check allergynums
						isValid=false;
						skipCount++;
						break;
					}
					if(_listAllergiesReconcile[j].IsNew) {
						//This is an allergy that is coming in from and external source.
						AllergyDef allergyDefNew=null;
						int index=ListAllergiesNew.IndexOf(_listAllergiesReconcile[j]);//Find the corresponding allergy def by looping through the incoming allergies.
						if(index==-1) {
							continue;//This should not happen
						}
						allergyDefNew=ListAllergyDefsNew[index];//Incoming allergy and allergy def lists are 1 to 1 so we can use the same index.
						if(allergyDefNew!=null && allergyDefNew.MedicationNum==allergyDef.MedicationNum) {
							isValid=false;
							skipCount++;
							break;
						}
					}
					if(allergy.AllergyDefNum==_listAllergiesReconcile[j].AllergyDefNum) {
						isValid=false;
						skipCount++;
						break;
					}
					if(!isValid) {
						break;
					}
				}
				if(isValid) {
					_listAllergiesReconcile.Add(allergy);
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
			Allergy allergy;
			for(int i=gridAllergyReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				allergy=_listAllergiesReconcile[gridAllergyReconcile.SelectedIndices[i]];
				_listAllergiesReconcile.Remove(allergy);
			}
			FillReconcileGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listAllergiesReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing allergies to be removed.  Continue?")) {
					return;
				}
			}
			Allergy allergy;
			AllergyDef allergyDef;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listAllergies.Count;i++) {//Start looping through all current allergies
				isActive=false;
				allergy=_listAllergies[i];
				allergyDef=AllergyDefs.GetOne(allergy.AllergyDefNum,_listAllergyDefs);
				for(int j=0;j<_listAllergiesReconcile.Count;j++) {//Compare each reconcile allergy to the current allergy
					AllergyDef allergyDefCur=AllergyDefs.GetOne(_listAllergiesReconcile[j].AllergyDefNum,_listAllergyDefs);
					if(_listAllergiesReconcile[j].AllergyDefNum==_listAllergies[i].AllergyDefNum) {//Has identical AllergyDefNums
						isActive=true;
						break;
					}
					if(allergyDefCur==null) {
						continue;
					}
					//if(alDR.SnomedAllergyTo!="" && alDR.SnomedAllergyTo==alD.SnomedAllergyTo) {//TODO: Change to UNII
					//	isActive=true;
					//	break;
					//}
					if(allergyDefCur.MedicationNum!=0 && allergyDefCur.MedicationNum==allergyDef.MedicationNum) {//Has a Snomed code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {
					_listAllergies[i].StatusIsActive=isActive;
					Allergies.Update(_listAllergies[i]);
				}
			}
			//Always update every current allergy for the patient so that DateTStamp reflects the last reconcile date.
			if(_listAllergies.Count>0) {
				Allergies.ResetTimeStamps(_patient.PatNum,true);
			}
			AllergyDef allergyDefU;
			int index;
			for(int j=0;j<_listAllergiesReconcile.Count;j++) {
				if(!_listAllergiesReconcile[j].IsNew) {
					continue;
				}
				index=ListAllergiesNew.IndexOf(_listAllergiesReconcile[j]);//Returns -1 if not found.
				if(index<0) {
					continue;
				}
				//Insert the AllergyDef and Allergy if needed.
				if(ListAllergyDefsNew[index].MedicationNum!=0) {
					allergyDefU=AllergyDefs.GetAllergyDefFromMedication(ListAllergyDefsNew[index].MedicationNum);
				}
				else {
					allergyDefU=null;//remove once UNII is implemented
					//alDU=AllergyDefs.GetAllergyDefFromCode(ListAllergyDefNew[index].SnomedAllergyTo);//TODO: Change to UNII
				}
				if(allergyDefU==null) {//db is missing the def
					ListAllergiesNew[index].AllergyDefNum=AllergyDefs.Insert(ListAllergyDefsNew[index]);
				}
				else {
					ListAllergiesNew[index].AllergyDefNum=allergyDefU.AllergyDefNum;//Set the allergydefnum on the allergy.
				}
				Allergies.Insert(ListAllergiesNew[index]);
			}
			//TODO: Make an allergy measure event if one is needed for MU3.
			//EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			//newMeasureEvent.DateTEvent=DateTime.Now;
			//newMeasureEvent.EventType=EhrMeasureEventType.AllergyReconcile;
			//newMeasureEvent.PatNum=PatCur.PatNum;
			//newMeasureEvent.MoreInfo="";
			//EhrMeasureEvents.Insert(newMeasureEvent);
			for(int i=0;i<_listAllergiesReconcile.Count;i++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) {
					AllergyDef allergyDefInter=AllergyDefs.GetOne(_listAllergiesReconcile[i].AllergyDefNum);
					using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
					formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(allergyDefInter,_patient);
					formCDSIntervention.ShowIfRequired(false);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}