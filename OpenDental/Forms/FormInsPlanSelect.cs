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
		public Relat Relat;
		///<summary>Set to true to view the relationship selection</summary>
		public bool ViewRelat;
		private Patient _patient;
		private Family _family;
		///<summary>After closing this form, this will contain the selected plan.  May be null to indicate none.</summary>
		public InsPlan InsPlanSelected;
		private List <InsPlan> _listInsPlans;
		private long _patNum;
		public bool ShowNoneButton;
		private List<InsSub> _listInsSubs;
		public InsSub InsSubSelected;
		//List of PatPlans for PatCur.
		private List<PatPlan> _listPatPlans;

		///<summary></summary>
		public FormInsPlanSelect(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			_patNum=patNum;
			//usage: eg. from coverage.  Since can be totally new subscriber, get all plans for them.
			_family=Patients.GetFamily(_patNum);
			_patient=_family.GetPatient(_patNum);
			_listInsSubs=InsSubs.RefreshForFam(_family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_listPatPlans=PatPlans.Refresh(_patNum);
			Lan.F(this);
			if(_listPatPlans.Count!=1){
				return;
			}
			Relat=_listPatPlans[0].Relationship;
			InsSubSelected=_listInsSubs.FirstOrDefault(x => x.InsSubNum==_listPatPlans[0].InsSubNum);
			if(InsSubSelected!=null){
				InsPlanSelected=InsPlans.GetPlan(InsSubSelected.PlanNum,_listInsPlans);
			}
			if(InsSubSelected==null || InsPlanSelected==null) {
				Relat=0;
				InsSubSelected=null;
				InsPlanSelected=null;
			}
			//tbPlans.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbPlans_CellDoubleClicked);
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
			gridMain.Columns.Clear();
			GridColumn col;
			//col=new ODGridColumn(Lan.g("TableInsPlans","#"),20);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Subscriber"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Ins Carrier"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Effect."),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Term."),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Used By"),90);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//PatPlan[] patPlanArray;
			InsPlan insPlan;
			for(int i=0;i<_listInsSubs.Count;i++) {
				if(!checkShowPlansNotInUse.Checked && //Only show insurance plans for PatCur.
					!_listPatPlans.Exists(x => x.InsSubNum==_listInsSubs[i].InsSubNum))
				{
					continue;
				}
				row=new GridRow();
				row.Tag=_listInsSubs[i];
				//row.Cells.Add((gridMain.Rows.Count+1).ToString());
				row.Cells.Add(_family.GetNameInFamLF(_listInsSubs[i].Subscriber));
				insPlan=InsPlans.GetPlan(_listInsSubs[i].PlanNum,_listInsPlans);
				row.Cells.Add(Carriers.GetName(insPlan.CarrierNum));
				if(_listInsSubs[i].DateEffective.Year<1880)
					row.Cells.Add("");
				else
					row.Cells.Add(_listInsSubs[i].DateEffective.ToString("d"));
				if(_listInsSubs[i].DateTerm.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listInsSubs[i].DateTerm.ToString("d"));
				}
				int countPatPlans=PatPlans.GetCountBySubNum(_listInsSubs[i].InsSubNum);
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
				Relat=listRelat.GetSelected<Relat>();
			}
			InsSubSelected=(InsSub)gridMain.ListGridRows[e.Row].Tag;
			InsPlanSelected=InsPlans.GetPlan(InsSubSelected.PlanNum,_listInsPlans);
			DialogResult=DialogResult.OK;
		}

		private void butNone_Click(object sender,EventArgs e) {
			InsPlanSelected=null;
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
				Relat=listRelat.GetSelected<Relat>();
			}
			InsSubSelected=(InsSub)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			InsPlanSelected=InsPlans.GetPlan(InsSubSelected.PlanNum,_listInsPlans);
			DialogResult=DialogResult.OK;		
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		//cancel already handled
	}
}