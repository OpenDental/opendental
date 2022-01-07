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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPlansForFamily : FormODBase {
		///<summary>Set this externally.</summary>
		public Family FamCur;
		private List<InsPlan> PlanList;
		private List<InsSub> SubList;

		///<summary></summary>
		public FormPlansForFamily()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPlansForFamily_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			SubList=InsSubs.RefreshForFam(FamCur);
			if(!InsSubs.ValidatePlanNumForList(SubList.Select(x => x.InsSubNum).ToList())) {
				SubList=InsSubs.RefreshForFam(FamCur);
			}
			PlanList=InsPlans.RefreshForSubList(SubList);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			//=new ODGridColumn(Lan.g("TableInsPlans","#"),20);
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
			for(int i=0;i<SubList.Count;i++){
				plan=InsPlans.GetPlan(SubList[i].PlanNum,PlanList);
				row=new GridRow();
				row.Cells.Add(FamCur.GetNameInFamLF(SubList[i].Subscriber));
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
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			InsPlan plan=InsPlans.GetPlan(SubList[e.Row].PlanNum,PlanList);
			using FormInsPlan FormIP=new FormInsPlan(plan,null,SubList[e.Row]);
			FormIP.ShowDialog();
			FillGrid();
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		


		


	}
}





















