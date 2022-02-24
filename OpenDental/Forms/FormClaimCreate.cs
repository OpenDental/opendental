using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary>Lists all insurance plans for which the supplied patient is the subscriber. Lets you select an insurance plan based on a patNum. SelectedPlan will contain the plan selected.</summary>
	public partial class FormClaimCreate : FormODBase {
		//private OpenDental.TableInsPlans tbPlans;
		///<summary></summary>
		public Relat PatRelat;
		//<summary>Set to true to view the relationship selection</summary>
		//public bool ViewRelat;
		private Patient PatCur;
		private Family FamCur;
		///<summary>After closing this form, this will contain the selected plan.</summary>
		public InsPlan SelectedPlan;
		public InsSub SelectedSub;
		private List <InsPlan> PlanList;
		private long PatNum;
		//public long ClaimFormNum;
		private List<InsSub> SubList;
		//List of PatPlans for PatCur.
		private List<PatPlan> _listPatCurPatPlans;

		///<summary></summary>
		public FormClaimCreate(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			PatNum=patNum;
			//tbPlans.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbPlans_CellDoubleClicked);
			Lan.F(this);
		}

		private void FormClaimCreate_Load(object sender, System.EventArgs e) {
			//usage: eg. from coverage.  Since can be totally new subscriber, get all plans for them.
			FamCur=Patients.GetFamily(PatNum);
			PatCur=FamCur.GetPatient(PatNum);
			SubList=InsSubs.RefreshForFam(FamCur);
			PlanList=InsPlans.RefreshForSubList(SubList);
			_listPatCurPatPlans=PatPlans.Refresh(PatNum);
			FillPlanData();
			//FillClaimForms();
    }

		/*
		private void FillClaimForms(){
			for(int i=0;i<ClaimForms.ListShort.Length;i++) {
				comboClaimForm.Items.Add(ClaimForms.ListShort[i].Description);
				if(ClaimForms.ListShort[i].ClaimFormNum==PrefC.GetLong(PrefName.DefaultClaimForm)) {
					comboClaimForm.SelectedIndex=i;
				}
			}
		}*/

		private void FillPlanData(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInsPlans","Plan"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Subscriber"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Ins Carrier"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Effect."),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Term."),90);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<GridRow> listRows=new List<GridRow>(); //create a list of gridrows so that we can order them by Ordinal after creating them.
			for(int i=0;i<SubList.Count;i++) {
				row=new GridRow();
				if(!checkShowPlansNotInUse.Checked && //Only show insurance plans for PatCur.
					!_listPatCurPatPlans.Exists(x => x.InsSubNum==SubList[i].InsSubNum)) 
				{
					continue;
				}
				else if(checkShowPlansNotInUse.Checked && !_listPatCurPatPlans.Exists(x => x.InsSubNum==SubList[i].InsSubNum)) {
					row.Cells.Add("Not Used");
				}
				else {
					PatPlan patPlan=_listPatCurPatPlans.FirstOrDefault(x => x.InsSubNum==SubList[i].InsSubNum);
					if(patPlan==null) {
						continue;
					}
					if(patPlan.Ordinal==1) {
						row.Cells.Add("Pri");
					}
					else if(patPlan.Ordinal==2) {
						row.Cells.Add("Sec");
					}
					else {
						row.Cells.Add("Other");
					}					
				}
				InsPlan plan=InsPlans.GetPlan(SubList[i].PlanNum,PlanList);
				row.Tag=SubList[i];
				row.Cells.Add(FamCur.GetNameInFamLF(SubList[i].Subscriber));
				row.Cells.Add(Carriers.GetName(plan.CarrierNum));
				if(SubList[i].DateEffective.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(SubList[i].DateEffective.ToString("d"));
				}
				if(SubList[i].DateTerm.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(SubList[i].DateTerm.ToString("d"));
				}
				listRows.Add(row);
			}
			listRows=listRows.OrderBy(x => x.Cells[0].Text != "Pri")
				.ThenBy(x => x.Cells[0].Text !="Sec")
				.ThenBy(x => x.Cells[0].Text !="Other")
				.ThenBy(x => x.Cells[0].Text !="Not Used").ToList();
			for(int i=0; i<listRows.Count;i++) {
				gridMain.ListGridRows.Add(listRows[i]);
			}
			gridMain.EndUpdate();
			listRelat.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(Relat)).Length;i++){
				listRelat.Items.Add(Lan.g("enumRelat",Enum.GetNames(typeof(Relat))[i]));
			}
		}

		private void checkPlansNotInUse_Click(object sender,EventArgs e) {
			FillPlanData();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(listRelat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a relationship first."));
				return;
			}
			PatRelat=(Relat)listRelat.SelectedIndex;
			SelectedSub=(InsSub)gridMain.ListGridRows[e.Row].Tag;
			SelectedPlan=InsPlans.GetPlan(SelectedSub.PlanNum,PlanList);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select a plan first."));
				return;
			}
			if(listRelat.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select a relationship first."));
				return;
			}
			//if(comboClaimForm.SelectedIndex==-1) {
			//	MessageBox.Show(Lan.g(this,"Please select a claimform first."));
			//	return;
			//}
			PatRelat=(Relat)listRelat.SelectedIndex;
			SelectedSub=(InsSub)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			SelectedPlan=InsPlans.GetPlan(SelectedSub.PlanNum,PlanList);
			//ClaimFormNum=ClaimForms.ListShort[comboClaimForm.SelectedIndex].ClaimFormNum;
      DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		//cancel already handled
	}
}