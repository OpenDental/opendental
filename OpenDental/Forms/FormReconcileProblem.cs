using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileProblem:FormODBase {
		public List<DiseaseDef> ListDiseaseDefsNew;
		public List<Disease> ListDiseasesNew;
		private List<Disease> _listDiseasesReconcile;
		private List<DiseaseDef> _listDiseaseDefs;
		private List<Disease> _listDiseases;
		private Patient _patient;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileProblem(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		private void FormReconcileProblem_Load(object sender,EventArgs e) {
			for(int i=0;i<ListDiseasesNew.Count;i++) {
				ListDiseasesNew[i].PatNum=_patient.PatNum;
			}
			FillExistingGrid();//Done first so that _listReconcileCur and _listReconcileDefCur are populated.
			_listDiseasesReconcile=new List<Disease>(_listDiseases);
			#region Delete After Testing
			//-------------------------------Delete after testing
			//ListProblemNew=new List<Disease>();
			//Disease dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(3));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(3));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Terrible";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Deadly";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Other";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Can't Think";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="What is Next!";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Hmmmm...";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(1));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(1));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Otherthly";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(6));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(6));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Dependant";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//dis=new Disease();
			//dis.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//dis.DateStart=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//dis.PatNum=PatCur.PatNum;
			//dis.ProbStatus=0;
			//dis.PatNote="Shifty";
			//dis.IsNew=true;
			//ListProblemNew.Add(dis);
			//ListProblemDefNew=new List<DiseaseDef>();
			//DiseaseDef disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(3));
			//disD.DiseaseName="Totally Preggers";
			//disD.SnomedCode="54116654";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(5));
			//disD.DiseaseName="Paraplegic";
			//disD.SnomedCode="4651561";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(7));
			//disD.DiseaseName="HIV/AIDS";
			//disD.SnomedCode="2165";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(11));
			//disD.DiseaseName="Milk Addict";
			//disD.SnomedCode="16544633";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(4));
			//disD.DiseaseName="Munchies";
			//disD.SnomedCode="41842384";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(2));
			//disD.DiseaseName="Gaddafid";
			//disD.SnomedCode="48416321";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(1));
			//disD.DiseaseName="D-Tosh Disease";
			//disD.SnomedCode="1847913";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(6));
			//disD.DiseaseName="Uncontrollable Hiccups";
			//disD.SnomedCode="486316";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//disD=new DiseaseDef();
			//disD.DateTStamp=DateTime.Now.Subtract(TimeSpan.FromDays(8));
			//disD.DiseaseName="Explosive Diarrhea";
			//disD.SnomedCode="9874954165";
			//disD.IsHidden=false;
			//disD.IsNew=true;
			//ListProblemDefNew.Add(disD);
			//-------------------------------
			#endregion
			//Automation to initially fill reconcile grid with "recommended" rows.
			bool isValid;
			for(int i=0;i<ListDiseasesNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listDiseaseDefs.Count;j++) {
					if(_listDiseaseDefs[j].SnomedCode==ListDiseaseDefsNew[i].SnomedCode) {
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listDiseasesReconcile.Add(ListDiseasesNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridProbImport.BeginUpdate();
			gridProbImport.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridProbImport.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbImport.Columns.Add(col);
			col=new GridColumn("Problem Name",200);
			gridProbImport.Columns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbImport.Columns.Add(col);
			gridProbImport.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListDiseasesNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListDiseasesNew[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
				row.Cells.Add(ListDiseasesNew[i].DateStart.ToShortDateString());
					}
				if(ListDiseaseDefsNew[i].DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListDiseaseDefsNew[i].DiseaseName);
				}
				if(ListDiseasesNew[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(ListDiseasesNew[i].ProbStatus==ProblemStatus.Resolved) {
					row.Cells.Add("Resolved");
				}
				else {
					row.Cells.Add("Inactive");
				}
				gridProbImport.ListGridRows.Add(row);
			}
			gridProbImport.EndUpdate();
		}

		private void FillExistingGrid() {
			gridProbExisting.BeginUpdate();
			gridProbExisting.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridProbExisting.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbExisting.Columns.Add(col);
			col=new GridColumn("Problem Name",200);
			gridProbExisting.Columns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbExisting.Columns.Add(col);
			gridProbExisting.ListGridRows.Clear();
			_listDiseases=Diseases.Refresh(_patient.PatNum,true);
			List<long> listDiseaseDefNums=new List<long>();
			for(int i=0;i<_listDiseases.Count;i++) {
				if(_listDiseases[i].DiseaseDefNum > 0) {
					listDiseaseDefNums.Add(_listDiseases[i].DiseaseDefNum);
				}
			}
			_listDiseaseDefs=DiseaseDefs.GetMultDiseaseDefs(listDiseaseDefNums);
			GridRow row;
			DiseaseDef diseaseDef;
			for(int i=0;i<_listDiseases.Count;i++) {
				row=new GridRow();
				diseaseDef=new DiseaseDef();
				diseaseDef=DiseaseDefs.GetItem(_listDiseases[i].DiseaseDefNum);
				row.Cells.Add(_listDiseases[i].DateTStamp.ToShortDateString());
				if(_listDiseases[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listDiseases[i].DateStart.ToShortDateString());
				}
				if(diseaseDef.DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(diseaseDef.DiseaseName);
				}
				if(_listDiseases[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(_listDiseases[i].ProbStatus==ProblemStatus.Resolved) {
					row.Cells.Add("Resolved");
				}
				else {
					row.Cells.Add("Inactive");
				}
				gridProbExisting.ListGridRows.Add(row);
			}
			gridProbExisting.EndUpdate();
		}

		private void FillReconcileGrid() {
			gridProbReconcile.BeginUpdate();
			gridProbReconcile.Columns.Clear();
			GridColumn col=new GridColumn("Last Modified",130,HorizontalAlignment.Center);
			gridProbReconcile.Columns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbReconcile.Columns.Add(col);
			col=new GridColumn("Problem Name",260);
			gridProbReconcile.Columns.Add(col);
			col=new GridColumn("Notes",300);
			gridProbReconcile.Columns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbReconcile.Columns.Add(col);
			col=new GridColumn("Is Incoming",50,HorizontalAlignment.Center);
			gridProbReconcile.Columns.Add(col);
			gridProbReconcile.ListGridRows.Clear();
			GridRow row;
			DiseaseDef diseaseDef;
			for(int i=0;i<_listDiseasesReconcile.Count;i++) {
				row=new GridRow();
				diseaseDef=new DiseaseDef();
				if(_listDiseasesReconcile[i].IsNew) {
					//To find the disease def for new disease, get the index of the matching problem in ListProblemNew, and use that index in ListProblemDefNew because they are 1 to 1 lists.
					diseaseDef=ListDiseaseDefsNew[ListDiseasesNew.IndexOf(_listDiseasesReconcile[i])];
				}
				for(int j=0;j<_listDiseaseDefs.Count;j++) {
					if(_listDiseasesReconcile[i].DiseaseDefNum > 0 && _listDiseasesReconcile[i].DiseaseDefNum==_listDiseaseDefs[j].DiseaseDefNum) {
						diseaseDef=_listDiseaseDefs[j];//Gets the diseasedef matching the disease so we can use it to populate the grid
						break;
					}
				}
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(_listDiseasesReconcile[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listDiseasesReconcile[i].DateStart.ToShortDateString());
				}
				if(diseaseDef.DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(diseaseDef.DiseaseName);
				}
				if(_listDiseasesReconcile[i]==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listDiseasesReconcile[i].PatNote);
				}
				if(_listDiseasesReconcile[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(_listDiseasesReconcile[i].ProbStatus==ProblemStatus.Resolved) {
					row.Cells.Add("Resolved");
				}
				else {
					row.Cells.Add("Inactive");
				}
				row.Cells.Add(_listDiseasesReconcile[i].IsNew?"X":"");
				gridProbReconcile.ListGridRows.Add(row);
			}
			gridProbReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridProbImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Disease disease;
			DiseaseDef diseaseDef;
			DiseaseDef diseaseDefR=null;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridProbImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridProbImport and ListProblemPatNew are a 1:1 list we can use the selected index position to get our disease
				disease=ListDiseasesNew[gridProbImport.SelectedIndices[i]];
				diseaseDef=ListDiseaseDefsNew[gridProbImport.SelectedIndices[i]];
				for(int j=0;j<_listDiseasesReconcile.Count;j++) {
					if(_listDiseasesReconcile[j].IsNew) {
						diseaseDefR=ListDiseaseDefsNew[ListDiseasesNew.IndexOf(_listDiseasesReconcile[j])];
					}
					else {
						diseaseDefR=DiseaseDefs.GetItem(_listDiseasesReconcile[j].DiseaseDefNum);
					}
					if(diseaseDefR==null) {
						continue;
					}
					if(diseaseDefR.SnomedCode!="" && diseaseDefR.SnomedCode==diseaseDef.SnomedCode) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listDiseasesReconcile.Add(disease);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because problem already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butAddExist_Click(object sender,EventArgs e) {
			if(gridProbExisting.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Disease disease;
			DiseaseDef diseaseDef;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridProbExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridProbImport and ListProblemPatNew are a 1:1 list we can use the selected index position to get our dis
				disease=_listDiseases[gridProbExisting.SelectedIndices[i]];
				diseaseDef=DiseaseDefs.GetItem(disease.DiseaseDefNum);
				for(int j=0;j<_listDiseasesReconcile.Count;j++) {
					if(_listDiseases[j].IsNew) {
						for(int k=0;k<ListDiseaseDefsNew.Count;k++) {
							if(diseaseDef.SnomedCode!="" && diseaseDef.SnomedCode==ListDiseaseDefsNew[k].SnomedCode) {
								isValid=false;
								skipCount++;
								break;
							}
						}
					}
					if(!isValid) {
						break;
					}
					if(disease.DiseaseDefNum==_listDiseasesReconcile[j].DiseaseDefNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listDiseasesReconcile.Add(disease);
				}
			}
			if(skipCount>0) {
				MessageBox.Show(Lan.g(this," Row(s) skipped because problem already present in the reconcile list")+": "+skipCount);
			}
			FillReconcileGrid();
		}

		private void butRemoveRec_Click(object sender,EventArgs e) {
			if(gridProbReconcile.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to remove");
				return;
			}
			Disease disease;
			for(int i=gridProbReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				disease=_listDiseasesReconcile[gridProbReconcile.SelectedIndices[i]];
				_listDiseasesReconcile.Remove(disease);
			}
			FillReconcileGrid();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_listDiseasesReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing problems to be removed.  Continue?")) {
					return;
				}
			}
			Disease disease;
			DiseaseDef diseaseDef;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listDiseases.Count;i++) {//Start looping through all current diseases
				isActive=false;
				disease=_listDiseases[i];
				diseaseDef=DiseaseDefs.GetItem(disease.DiseaseDefNum);
				for(int j=0;j<_listDiseasesReconcile.Count;j++) {//Compare each reconcile diseases to the current disease
					DiseaseDef diseaseDefR=DiseaseDefs.GetItem(_listDiseasesReconcile[j].DiseaseDefNum);
					if(_listDiseasesReconcile[j].DiseaseDefNum==_listDiseases[i].DiseaseDefNum) {//Has identical DiseaseDefNums
						isActive=true;
						break;
					}
					if(diseaseDefR==null) {
						continue;
					}
					if(diseaseDefR.SnomedCode!="" && diseaseDefR.SnomedCode==diseaseDef.SnomedCode) {//Has a Snomed code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {//Update current problems.
					disease.ProbStatus=ProblemStatus.Inactive;
					Diseases.Update(_listDiseases[i]);
				}
			}
			//Always update every current problem for the patient so that DateTStamp reflects the last reconcile date.
			if(_listDiseases.Count>0) {
				Diseases.ResetTimeStamps(_patient.PatNum,ProblemStatus.Active);
			}
			DiseaseDef diseaseDefU=null;
			int index;
			for(int i=0;i<_listDiseasesReconcile.Count;i++) {
				index=ListDiseasesNew.IndexOf(_listDiseasesReconcile[i]);
				if(index<0) {
					continue;
				}
				if(_listDiseasesReconcile[i]!=ListDiseasesNew[index]) {
					continue;
				}
				diseaseDefU=DiseaseDefs.GetItem(DiseaseDefs.GetNumFromCode(ListDiseaseDefsNew[index].SnomedCode));
				if(diseaseDefU==null) {
					ListDiseasesNew[index].DiseaseDefNum=DiseaseDefs.Insert(ListDiseaseDefsNew[index]);
				}
				else {
					ListDiseasesNew[index].DiseaseDefNum=diseaseDefU.DiseaseDefNum;
				}
				Diseases.Insert(ListDiseasesNew[index]);
			}
			DataValid.SetInvalid(InvalidType.Diseases);
			//EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			//newMeasureEvent.DateTEvent=DateTime.Now;
			//newMeasureEvent.EventType=EhrMeasureEventType.ProblemReconcile;
			//newMeasureEvent.PatNum=PatCur.PatNum;
			//newMeasureEvent.MoreInfo="";
			//EhrMeasureEvents.Insert(newMeasureEvent);
			for(int i=0;i<_listDiseasesReconcile.Count;i++) {
				if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS || !CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS) {
					continue;
				}
				DiseaseDef diseaseDefInter=DiseaseDefs.GetItem(_listDiseasesReconcile[i].DiseaseDefNum);
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(diseaseDefInter,_patient);
				FormCDSI.ShowIfRequired();
			}
			DialogResult=DialogResult.OK;
		}

	}
}