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
///<summary>Lists all insurance plans for which the supplied patient is the subscriber. Lets you select an insurance plan based on a patNum. InsPlanSelected will contain the plan selected.</summary>
	public partial class FormClaimCreate : FormODBase {
		//private OpenDental.TableInsPlans tbPlans;
		///<summary></summary>
		public Relat RelatPat;
		//<summary>Set to true to view the relationship selection</summary>
		//public bool ViewRelat;
		//private Patient PatientCur;
		private Family _family;
		///<summary>After closing this form, this will contain the selected plan.</summary>
		public InsPlan InsPlanSelected;
		public InsSub InsSubSelected;
		private List <InsPlan> _listInsPlans;
		private long _patNum;
		//public long ClaimFormNum;
		private List<InsSub> _listInsSubs;
		//List of PatPlans for PatCur.
		private List<PatPlan> _listPatPlans;

		///<summary></summary>
		public FormClaimCreate(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			_patNum=patNum;
			//tbPlans.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbPlans_CellDoubleClicked);
			Lan.F(this);
		}

		private void FormClaimCreate_Load(object sender, System.EventArgs e) {
			//usage: eg. from coverage.  Since can be totally new subscriber, get all plans for them.
			_family=Patients.GetFamily(_patNum);
			//PatientCur=FamilyCur.GetPatient(PatNum);
			_listInsSubs=InsSubs.RefreshForFam(_family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_listPatPlans=PatPlans.Refresh(_patNum);
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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInsPlans","Plan"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Subscriber"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Ins Carrier"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Effect."),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInsPlans","Date Term."),90);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<GridRow> listGridRows=new List<GridRow>(); //create a list of gridrows so that we can order them by Ordinal after creating them.
			for(int i=0;i<_listInsSubs.Count;i++) {
				row=new GridRow();
				if(!checkShowPlansNotInUse.Checked && //Only show insurance plans for PatCur.
					!_listPatPlans.Exists(x => x.InsSubNum==_listInsSubs[i].InsSubNum)) 
				{
					continue;
				}
				else if(checkShowPlansNotInUse.Checked && !_listPatPlans.Exists(x => x.InsSubNum==_listInsSubs[i].InsSubNum)) {
					row.Cells.Add("Not Used");
				}
				else {
					PatPlan patPlan=_listPatPlans.FirstOrDefault(x => x.InsSubNum==_listInsSubs[i].InsSubNum);
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
				InsPlan insPlan=InsPlans.GetPlan(_listInsSubs[i].PlanNum,_listInsPlans);
				row.Tag=_listInsSubs[i];
				row.Cells.Add(_family.GetNameInFamLF(_listInsSubs[i].Subscriber));
				string carrierName=Carriers.GetName(insPlan.CarrierNum);
				row.Cells.Add(carrierName);
				if(_listInsSubs[i].DateEffective.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listInsSubs[i].DateEffective.ToString("d"));
				}
				if(_listInsSubs[i].DateTerm.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listInsSubs[i].DateTerm.ToString("d"));
				}
				listGridRows.Add(row);
			}
			listGridRows=listGridRows.OrderBy(x => x.Cells[0].Text != "Pri")
				.ThenBy(x => x.Cells[0].Text !="Sec")
				.ThenBy(x => x.Cells[0].Text !="Other")
				.ThenBy(x => x.Cells[0].Text !="Not Used").ToList();
			for(int i=0; i<listGridRows.Count;i++) {
				gridMain.ListGridRows.Add(listGridRows[i]);
			}
			gridMain.EndUpdate();
			listRelat.Items.Clear();
			string[] stringArrayEnumRelats=Enum.GetNames(typeof(Relat));
			for(int i=0;i<stringArrayEnumRelats.Length;i++){
				listRelat.Items.Add(Lan.g("enumRelat",stringArrayEnumRelats[i]));
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
			RelatPat=(Relat)listRelat.SelectedIndex;
			InsSubSelected=(InsSub)gridMain.ListGridRows[e.Row].Tag;
			InsPlanSelected=InsPlans.GetPlan(InsSubSelected.PlanNum,_listInsPlans);
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
			RelatPat=(Relat)listRelat.SelectedIndex;
			InsSubSelected=(InsSub)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			InsPlanSelected=InsPlans.GetPlan(InsSubSelected.PlanNum,_listInsPlans);
			//ClaimFormNum=ClaimForms.ListShort[comboClaimForm.SelectedIndex].ClaimFormNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		//cancel already handled
	}
}