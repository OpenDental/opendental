using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormDiscountPlans:FormODBase {
		public bool IsSelectionMode;
		public DiscountPlan SelectedPlan;

		public FormDiscountPlans() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDiscountPlans_Load(object sender,EventArgs e) {
			FillGrid();
			if(IsSelectionMode) {
				butMerge.Visible=false;
				checkShowHidden.Visible=false;
			}
		}

		private void FillGrid() {
			List<DiscountPlan> listDiscountPlans=DiscountPlans.GetAll(checkShowHidden.Checked);
			Dictionary<long,int> dictPatientsOnPlan=DiscountPlans.GetPatCountsForPlans(listDiscountPlans.Select(x => x.DiscountPlanNum).ToList());
			listDiscountPlans.Sort(DiscountPlanComparer);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableDiscountPlans","Description"),200));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableDiscountPlans","Fee Schedule"),170));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableDiscountPlans","Adjustment Type"),checkShowHidden.Checked ? 150 : 170 ));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableDiscountPlans","Pats"),40));
			if(checkShowHidden.Checked) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableDiscountPlans","Hidden"),20,HorizontalAlignment.Center));
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			int selectedIdx=-1;
			for(int i=0;i<listDiscountPlans.Count;i++) {
				Def adjType=Defs.GetDef(DefCat.AdjTypes,listDiscountPlans[i].DefNum);
				row=new GridRow();
				row.Cells.Add(listDiscountPlans[i].Description);
				row.Cells.Add(FeeScheds.GetDescription(listDiscountPlans[i].FeeSchedNum));
				row.Cells.Add((adjType==null) ? "" : adjType.ItemName);
				row.Cells.Add(dictPatientsOnPlan.ContainsKey(listDiscountPlans[i].DiscountPlanNum) 
					? dictPatientsOnPlan[listDiscountPlans[i].DiscountPlanNum].ToString() : "0");
				if(checkShowHidden.Checked) {
					row.Cells.Add(listDiscountPlans[i].IsHidden ? "X" : "");
				}
				row.Tag=listDiscountPlans[i];
				gridMain.ListGridRows.Add(row);
				if(SelectedPlan!=null && listDiscountPlans[i].DiscountPlanNum==SelectedPlan.DiscountPlanNum) {
					selectedIdx=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIdx,true);
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			SelectedPlan=(DiscountPlan)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			if(IsSelectionMode) {	
				DialogResult=DialogResult.OK;
				return;
			}
			using FormDiscountPlanEdit FormDPE=new FormDiscountPlanEdit();
			FormDPE.DiscountPlanCur=SelectedPlan.Copy();
			if(FormDPE.ShowDialog()==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit)) {
				return;
			}
			DiscountPlan discountPlan=new DiscountPlan();
			discountPlan.IsNew=true;
			using FormDiscountPlanEdit FormDPE=new FormDiscountPlanEdit();
			FormDPE.DiscountPlanCur=discountPlan;
			if(FormDPE.ShowDialog()==DialogResult.OK) {
				SelectedPlan=discountPlan;
				FillGrid();
			}
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit)) {
				return;
			}
			using FormDiscountPlanMerge FormDPM=new FormDiscountPlanMerge();
			FormDPM.ShowDialog();
			FillGrid();
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(IsSelectionMode) {
				if(gridMain.GetSelectedIndex()==-1) {
					MsgBox.Show(this,"Please select an entry.");
					return;
				}
				SelectedPlan=(DiscountPlan)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private static int DiscountPlanComparer(DiscountPlan plan1,DiscountPlan plan2) {
			if(!plan1.IsHidden && plan2.IsHidden) {
				return -1;
			}
			else if(plan1.IsHidden && !plan2.IsHidden) {
				return 1;
			}
			return plan1.Description.CompareTo(plan2.Description);
		}

	}
}