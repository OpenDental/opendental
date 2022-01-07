using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReconcileProblem:FormODBase {
		public List<DiseaseDef> ListProblemDefNew;
		public List<Disease> ListProblemNew;
		private List<Disease> _listProblemReconcile;
		private List<DiseaseDef> _listProblemDefCur;
		private List<Disease> _listProblemCur;
		private Patient _patCur;

		///<summary>Patient must be valid.  Do not pass null.</summary>
		public FormReconcileProblem(Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
		}

		private void FormReconcileProblem_Load(object sender,EventArgs e) {
			for(int index=0;index<ListProblemNew.Count;index++) {
				ListProblemNew[index].PatNum=_patCur.PatNum;
			}
			FillExistingGrid();//Done first so that _listReconcileCur and _listReconcileDefCur are populated.
			_listProblemReconcile=new List<Disease>(_listProblemCur);
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
			for(int i=0;i<ListProblemNew.Count;i++) {
				isValid=true;
				for(int j=0;j<_listProblemDefCur.Count;j++) {
					if(_listProblemDefCur[j].SnomedCode==ListProblemDefNew[i].SnomedCode) {
						isValid=false;
						break;
					}
				}
				if(isValid) {
					_listProblemReconcile.Add(ListProblemNew[i]);
				}
			}
			FillImportGrid();
			FillReconcileGrid();
		}

		private void FillImportGrid() {
			gridProbImport.BeginUpdate();
			gridProbImport.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridProbImport.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbImport.ListGridColumns.Add(col);
			col=new GridColumn("Problem Name",200);
			gridProbImport.ListGridColumns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbImport.ListGridColumns.Add(col);
			gridProbImport.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListProblemNew.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(ListProblemNew[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
				row.Cells.Add(ListProblemNew[i].DateStart.ToShortDateString());
					}
				if(ListProblemDefNew[i].DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListProblemDefNew[i].DiseaseName);
				}
				if(ListProblemNew[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(ListProblemNew[i].ProbStatus==ProblemStatus.Resolved) {
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
			gridProbExisting.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",100,HorizontalAlignment.Center);
			gridProbExisting.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbExisting.ListGridColumns.Add(col);
			col=new GridColumn("Problem Name",200);
			gridProbExisting.ListGridColumns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbExisting.ListGridColumns.Add(col);
			gridProbExisting.ListGridRows.Clear();
			_listProblemCur=Diseases.Refresh(_patCur.PatNum,true);
			List<long> problemDefNums=new List<long>();
			for(int h=0;h<_listProblemCur.Count;h++) {
				if(_listProblemCur[h].DiseaseDefNum > 0) {
					problemDefNums.Add(_listProblemCur[h].DiseaseDefNum);
				}
			}
			_listProblemDefCur=DiseaseDefs.GetMultDiseaseDefs(problemDefNums);
			GridRow row;
			DiseaseDef disD;
			for(int i=0;i<_listProblemCur.Count;i++) {
				row=new GridRow();
				disD=new DiseaseDef();
				disD=DiseaseDefs.GetItem(_listProblemCur[i].DiseaseDefNum);
				row.Cells.Add(_listProblemCur[i].DateTStamp.ToShortDateString());
				if(_listProblemCur[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listProblemCur[i].DateStart.ToShortDateString());
				}
				if(disD.DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(disD.DiseaseName);
				}
				if(_listProblemCur[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(_listProblemCur[i].ProbStatus==ProblemStatus.Resolved) {
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
			gridProbReconcile.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Modified",130,HorizontalAlignment.Center);
			gridProbReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Date Start",100,HorizontalAlignment.Center);
			gridProbReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Problem Name",260);
			gridProbReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Notes",300);
			gridProbReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Status",80,HorizontalAlignment.Center);
			gridProbReconcile.ListGridColumns.Add(col);
			col=new GridColumn("Is Incoming",50,HorizontalAlignment.Center);
			gridProbReconcile.ListGridColumns.Add(col);
			gridProbReconcile.ListGridRows.Clear();
			GridRow row;
			DiseaseDef disD;
			for(int i=0;i<_listProblemReconcile.Count;i++) {
				row=new GridRow();
				disD=new DiseaseDef();
				if(_listProblemReconcile[i].IsNew) {
					//To find the disease def for new disease, get the index of the matching problem in ListProblemNew, and use that index in ListProblemDefNew because they are 1 to 1 lists.
					disD=ListProblemDefNew[ListProblemNew.IndexOf(_listProblemReconcile[i])];
				}
				for(int j=0;j<_listProblemDefCur.Count;j++) {
					if(_listProblemReconcile[i].DiseaseDefNum > 0 && _listProblemReconcile[i].DiseaseDefNum==_listProblemDefCur[j].DiseaseDefNum) {
						disD=_listProblemDefCur[j];//Gets the diseasedef matching the disease so we can use it to populate the grid
						break;
					}
				}
				row.Cells.Add(DateTime.Now.ToShortDateString());
				if(_listProblemReconcile[i].DateStart.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listProblemReconcile[i].DateStart.ToShortDateString());
				}
				if(disD.DiseaseName==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(disD.DiseaseName);
				}
				if(_listProblemReconcile[i]==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listProblemReconcile[i].PatNote);
				}
				if(_listProblemReconcile[i].ProbStatus==ProblemStatus.Active) {
					row.Cells.Add("Active");
				}
				else if(_listProblemReconcile[i].ProbStatus==ProblemStatus.Resolved) {
					row.Cells.Add("Resolved");
				}
				else {
					row.Cells.Add("Inactive");
				}
				row.Cells.Add(_listProblemReconcile[i].IsNew?"X":"");
				gridProbReconcile.ListGridRows.Add(row);
			}
			gridProbReconcile.EndUpdate();
		}

		private void butAddNew_Click(object sender,EventArgs e) {
			if(gridProbImport.SelectedIndices.Length==0) {
				MsgBox.Show(this,"A row must be selected to add");
				return;
			}
			Disease dis;
			DiseaseDef disD;
			DiseaseDef disDR=null;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridProbImport.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridProbImport and ListProblemPatNew are a 1:1 list we can use the selected index position to get our disease
				dis=ListProblemNew[gridProbImport.SelectedIndices[i]];
				disD=ListProblemDefNew[gridProbImport.SelectedIndices[i]];
				for(int j=0;j<_listProblemReconcile.Count;j++) {
					if(_listProblemReconcile[j].IsNew) {
						disDR=ListProblemDefNew[ListProblemNew.IndexOf(_listProblemReconcile[j])];
					}
					else {
						disDR=DiseaseDefs.GetItem(_listProblemReconcile[j].DiseaseDefNum);
					}
					if(disDR==null) {
						continue;
					}
					if(disDR.SnomedCode!="" && disDR.SnomedCode==disD.SnomedCode) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listProblemReconcile.Add(dis);
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
			Disease dis;
			DiseaseDef disD;
			int skipCount=0;
			bool isValid;
			for(int i=0;i<gridProbExisting.SelectedIndices.Length;i++) {
				isValid=true;
				//Since gridProbImport and ListProblemPatNew are a 1:1 list we can use the selected index position to get our dis
				dis=_listProblemCur[gridProbExisting.SelectedIndices[i]];
				disD=DiseaseDefs.GetItem(dis.DiseaseDefNum);
				for(int j=0;j<_listProblemReconcile.Count;j++) {
					if(_listProblemCur[j].IsNew) {
						for(int k=0;k<ListProblemDefNew.Count;k++) {
							if(disD.SnomedCode!="" && disD.SnomedCode==ListProblemDefNew[k].SnomedCode) {
								isValid=false;
								skipCount++;
								break;
							}
						}
					}
					if(!isValid) {
						break;
					}
					if(dis.DiseaseDefNum==_listProblemReconcile[j].DiseaseDefNum) {
						isValid=false;
						skipCount++;
						break;
					}
				}
				if(isValid) {
					_listProblemReconcile.Add(dis);
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
			Disease dis;
			for(int i=gridProbReconcile.SelectedIndices.Length-1;i>-1;i--) {//Loop backwards so that we can remove from the list as we go
				dis=_listProblemReconcile[gridProbReconcile.SelectedIndices[i]];
				_listProblemReconcile.Remove(dis);
			}
			FillReconcileGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listProblemReconcile.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The reconcile list is empty which will cause all existing problems to be removed.  Continue?")) {
					return;
				}
			}
			Disease dis;
			DiseaseDef disD;
			bool isActive;
			//Discontinue any current medications that are not present in the reconcile list.
			for(int i=0;i<_listProblemCur.Count;i++) {//Start looping through all current problems
				isActive=false;
				dis=_listProblemCur[i];
				disD=DiseaseDefs.GetItem(dis.DiseaseDefNum);
				for(int j=0;j<_listProblemReconcile.Count;j++) {//Compare each reconcile problem to the current problem
					DiseaseDef disDR=DiseaseDefs.GetItem(_listProblemReconcile[j].DiseaseDefNum);
					if(_listProblemReconcile[j].DiseaseDefNum==_listProblemCur[i].DiseaseDefNum) {//Has identical DiseaseDefNums
						isActive=true;
						break;
					}
					if(disDR==null) {
						continue;
					}
					if(disDR.SnomedCode!="" && disDR.SnomedCode==disD.SnomedCode) {//Has a Snomed code and they are equal
						isActive=true;
						break;
					}
				}
				if(!isActive) {//Update current problems.
					dis.ProbStatus=ProblemStatus.Inactive;
					Diseases.Update(_listProblemCur[i]);
				}
			}
			//Always update every current problem for the patient so that DateTStamp reflects the last reconcile date.
			if(_listProblemCur.Count>0) {
				Diseases.ResetTimeStamps(_patCur.PatNum,ProblemStatus.Active);
			}
			DiseaseDef disDU=null;
			int index;
			for(int j=0;j<_listProblemReconcile.Count;j++) {
				index=ListProblemNew.IndexOf(_listProblemReconcile[j]);
				if(index<0) {
					continue;
				}
				if(_listProblemReconcile[j]==ListProblemNew[index]) {
					disDU=DiseaseDefs.GetItem(DiseaseDefs.GetNumFromCode(ListProblemDefNew[index].SnomedCode));
					if(disDU==null) {
						ListProblemNew[index].DiseaseDefNum=DiseaseDefs.Insert(ListProblemDefNew[index]);
					}
					else {
						ListProblemNew[index].DiseaseDefNum=disDU.DiseaseDefNum;
					}
					Diseases.Insert(ListProblemNew[index]);
				}
			}
			DataValid.SetInvalid(InvalidType.Diseases);
			//EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			//newMeasureEvent.DateTEvent=DateTime.Now;
			//newMeasureEvent.EventType=EhrMeasureEventType.ProblemReconcile;
			//newMeasureEvent.PatNum=PatCur.PatNum;
			//newMeasureEvent.MoreInfo="";
			//EhrMeasureEvents.Insert(newMeasureEvent);
			for(int inter=0;inter<_listProblemReconcile.Count;inter++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS) {
					DiseaseDef disDInter=DiseaseDefs.GetItem(_listProblemReconcile[inter].DiseaseDefNum);
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(disDInter,_patCur);
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