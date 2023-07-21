using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientListDiscount:FormODBase {
		public DiscountPlan DiscountPlanCur;
		public List<string> ListPatNames;

		public FormPatientListDiscount() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientListDiscount_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			if(ListPatNames==null) {
				ListPatNames=DiscountPlans.GetPatNamesForPlan(DiscountPlanCur.DiscountPlanNum)
					.Distinct()
					.OrderBy(x => x)
					.ToList();
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Name"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<ListPatNames.Count;i++) {
				GridRow row=new GridRow(ListPatNames[i]);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}