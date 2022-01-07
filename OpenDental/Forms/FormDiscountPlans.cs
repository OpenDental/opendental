using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormDiscountPlans:FormODBase {
		public bool IsSelectionMode;
		public DiscountPlan DiscountPlanSelected;

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
			List<long> listDiscountPlanNums=listDiscountPlans.Select(x => x.DiscountPlanNum).ToList();
			List<DiscountPlans.CountPerPlan> listCountPerPlan= DiscountPlans.GetPatCountsForPlans(listDiscountPlanNums);
			listDiscountPlans.Sort(DiscountPlanComparer);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableDiscountPlans","Description"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDiscountPlans","Fee Schedule"),170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDiscountPlans","Adjustment Type"),checkShowHidden.Checked ? 150 : 170 );
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDiscountPlans","Pats"),40);
			gridMain.ListGridColumns.Add(col);
			if(checkShowHidden.Checked) {
				col=new GridColumn(Lan.g("TableDiscountPlans","Hidden"),20,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			int selectedIdx=-1;
			for(int i=0;i<listDiscountPlans.Count;i++) {
				Def def=Defs.GetDef(DefCat.AdjTypes,listDiscountPlans[i].DefNum);
				row=new GridRow();
				row.Cells.Add(listDiscountPlans[i].Description);
				row.Cells.Add(FeeScheds.GetDescription(listDiscountPlans[i].FeeSchedNum));
				row.Cells.Add((def==null) ? "" : def.ItemName);
				DiscountPlans.CountPerPlan countPerPlan=listCountPerPlan.FirstOrDefault(x => x.DiscountPlanNum==listDiscountPlans[i].DiscountPlanNum);
				if(countPerPlan==null) {
					row.Cells.Add("0");
				}
				else {
					row.Cells.Add(countPerPlan.Count.ToString());
				}
				if(checkShowHidden.Checked) {
					row.Cells.Add(listDiscountPlans[i].IsHidden ? "X" : "");
				}
				row.Tag=listDiscountPlans[i];
				gridMain.ListGridRows.Add(row);
				if(DiscountPlanSelected!=null && listDiscountPlans[i].DiscountPlanNum==DiscountPlanSelected.DiscountPlanNum) {
					selectedIdx=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIdx,true);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DiscountPlanSelected=(DiscountPlan)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			if(IsSelectionMode) {	
				DialogResult=DialogResult.OK;
				return;
			}
			using FormDiscountPlanEdit formDiscountPlanEdit=new FormDiscountPlanEdit();
			formDiscountPlanEdit.DiscountPlanCur=DiscountPlanSelected.Copy();
			if(formDiscountPlanEdit.ShowDialog()==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit)) {
				return;
			}
			DiscountPlan discountPlan=new DiscountPlan();
			discountPlan.IsNew=true;
			using FormDiscountPlanEdit formDiscountPlanEdit=new FormDiscountPlanEdit();
			formDiscountPlanEdit.DiscountPlanCur=discountPlan;
			if(formDiscountPlanEdit.ShowDialog()==DialogResult.OK) {
				DiscountPlanSelected=discountPlan;
				FillGrid();
			}
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit)) {
				return;
			}
			using FormDiscountPlanMerge formDiscountPlanMerge=new FormDiscountPlanMerge();
			formDiscountPlanMerge.ShowDialog();
			FillGrid();
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsSelectionMode) {
				DialogResult=DialogResult.OK;
				return;
			}
			//IsSelectionMode
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an entry.");
				return;
			}
			DiscountPlanSelected=(DiscountPlan)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private static int DiscountPlanComparer(DiscountPlan discountPlan1,DiscountPlan discountPlan2) {
			if(!discountPlan1.IsHidden && discountPlan2.IsHidden) {
				return -1;
			}
			else if(discountPlan1.IsHidden && !discountPlan2.IsHidden) {
				return 1;
			}
			return discountPlan1.Description.CompareTo(discountPlan2.Description);
		}

	}
}