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
		public Family FamilyCur;
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;

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
			_listInsSubs=InsSubs.RefreshForFam(FamilyCur);
			if(!InsSubs.ValidatePlanNumForList(_listInsSubs.Select(x => x.InsSubNum).ToList())) {
				_listInsSubs=InsSubs.RefreshForFam(FamilyCur);
			}
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			//=new ODGridColumn(Lan.g("TableInsPlans","#"),20);
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
			for(int i=0;i<_listInsSubs.Count;i++){
				insPlan=InsPlans.GetPlan(_listInsSubs[i].PlanNum,_listInsPlans);
				row=new GridRow();
				row.Cells.Add(FamilyCur.GetNameInFamLF(_listInsSubs[i].Subscriber));
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
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			InsPlan insPlan=InsPlans.GetPlan(_listInsSubs[e.Row].PlanNum,_listInsPlans);
			using FormInsPlan formInsPlan=new FormInsPlan(insPlan,null,_listInsSubs[e.Row]);
			formInsPlan.ShowDialog();
			FillGrid();
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		


		


	}
}





















