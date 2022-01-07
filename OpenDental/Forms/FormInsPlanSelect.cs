using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
///<summary>Lists all insurance plans for which the supplied patient is the subscriber. Lets you select an insurance plan based on a patNum. SelectedPlan will contain the plan selected.</summary>
	public partial class FormInsPlanSelect : FormODBase {
		//private OpenDental.TableInsPlans tbPlans;
		///<summary></summary>
		public Relat PatRelat;
		///<summary>Set to true to view the relationship selection</summary>
		public bool ViewRelat;
		private Patient PatCur;
		private Family FamCur;
		///<summary>After closing this form, this will contain the selected plan.  May be null to indicate none.</summary>
		public InsPlan SelectedPlan;
		private List <InsPlan> PlanList;
		private long PatNum;
		public bool ShowNoneButton;
		private List<InsSub> SubList;
		public InsSub SelectedSub;
		//List of PatPlans for PatCur.
		private List<PatPlan> _listPatCurPatPlans;

		///<summary></summary>
		public FormInsPlanSelect(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			PatNum=patNum;
			//usage: eg. from coverage.  Since can be totally new subscriber, get all plans for them.
			FamCur=Patients.GetFamily(PatNum);
			PatCur=FamCur.GetPatient(PatNum);
			SubList=InsSubs.RefreshForFam(FamCur);
			PlanList=InsPlans.RefreshForSubList(SubList);
			_listPatCurPatPlans=PatPlans.Refresh(PatNum);
			if(_listPatCurPatPlans.Count==1) {
				try {
					PatRelat=_listPatCurPatPlans[0].Relationship;
					SelectedSub=SubList.FirstOrDefault(x => x.InsSubNum==_listPatCurPatPlans[0].InsSubNum);
					SelectedPlan=InsPlans.GetPlan(SelectedSub.PlanNum,PlanList);
					if(SelectedSub==null || SelectedPlan==null) {
						throw new ApplicationException();
					}
				}
				catch {
					PatRelat=0;
					SelectedSub=null;
					SelectedPlan=null;
				}
			}
			//tbPlans.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbPlans_CellDoubleClicked);
			Lan.F(this);
		}

		private void FormInsPlansSelect_Load(object sender, System.EventArgs e) {
			if(!ViewRelat){
				labelRelat.Visible=false;
				listRelat.Visible=false;
			}
			FillPlanData();
			if(!ShowNoneButton) {
				butNone.Visible=false;
			}
    }

		private void FillPlanData(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			//col=new ODGridColumn(Lan.g("TableInsPlans","#"),20);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Subscriber"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Ins Carrier"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Effect."),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Term."),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Used By"),90);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//PatPlan[] patPlanArray;
			InsPlan plan;
			for(int i=0;i<SubList.Count;i++) {
				if(!checkShowPlansNotInUse.Checked && //Only show insurance plans for PatCur.
					!_listPatCurPatPlans.Exists(x => x.InsSubNum==SubList[i].InsSubNum))
				{
					continue;
				}
				row=new GridRow();
				row.Tag=SubList[i];
				//row.Cells.Add((gridMain.Rows.Count+1).ToString());
				row.Cells.Add(FamCur.GetNameInFamLF(SubList[i].Subscriber));
				plan=InsPlans.GetPlan(SubList[i].PlanNum,PlanList);
				row.Cells.Add(Carriers.GetName(plan.CarrierNum));
				if(SubList[i].DateEffective.Year<1880)
					row.Cells.Add("");
				else
					row.Cells.Add(SubList[i].DateEffective.ToString("d"));
				if(SubList[i].DateTerm.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(SubList[i].DateTerm.ToString("d"));
				}
				int countPatPlans=PatPlans.GetCountBySubNum(SubList[i].InsSubNum);
				row.Cells.Add(countPatPlans.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			listRelat.Items.Clear();
			listRelat.Items.AddEnums<Relat>();
		}
		
		private void checkPlansNotInUse_Click(object sender,EventArgs e) {
			FillPlanData();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(ViewRelat && listRelat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a relationship first."));
				return;
			}
			if(ViewRelat) {
				PatRelat=listRelat.GetSelected<Relat>();
			}
			SelectedSub=(InsSub)gridMain.ListGridRows[e.Row].Tag;
			SelectedPlan=InsPlans.GetPlan(SelectedSub.PlanNum,PlanList);
			DialogResult=DialogResult.OK;
		}

		private void butNone_Click(object sender,EventArgs e) {
			SelectedPlan=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select a plan first."));
				return;
			}
			if(ViewRelat && listRelat.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select a relationship first."));
				return;
			}
			if(ViewRelat){
				PatRelat=listRelat.GetSelected<Relat>();
			}
			SelectedSub=(InsSub)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			SelectedPlan=InsPlans.GetPlan(SelectedSub.PlanNum,PlanList);
      DialogResult=DialogResult.OK;		
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		//cancel already handled
	}
}