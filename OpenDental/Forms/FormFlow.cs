using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFlow:FormODBase {

		private long _patientFlowNum;
		private List<FlowAction> _listActions;
		private Flow _flow;

		public FormFlow(long patientFlowNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patientFlowNum=patientFlowNum;
		}

		private void FormPatientFlow_Load(object sender,EventArgs e) {
			_flow = Flows.GetOne(_patientFlowNum);
			_listActions = FlowActions.GetListForFlow(_patientFlowNum);
			textDate.Text = _flow.SecDateTEntry.ToString("G");
			textDescription.Text = _flow.Description;
			textPatName.Text = Patients.GetNameFL(_flow.PatNum);
			FillGrid();
		}

		private void FillGrid() {
			gridActions.BeginUpdate();
			gridActions.Columns.Clear();
			gridActions.ListGridRows.Clear();
			gridActions.Columns.Add(new UI.GridColumn("Action Type",150, textAlign: HorizontalAlignment.Center));
			gridActions.Columns.Add(new UI.GridColumn("Status", 80, textAlign: HorizontalAlignment.Center));
			gridActions.Columns.Add(new UI.GridColumn("Completed By", 120, textAlign: HorizontalAlignment.Center));
			gridActions.Columns.Add(new UI.GridColumn("Date Complete", 150, textAlign: HorizontalAlignment.Center));
			_listActions.ForEach(action => {
				GridRow row = new GridRow();
				row.Cells.Add(new GridCell(action.FlowActionType.GetDescription()));
				row.Cells.Add(new GridCell() { Text = action.IsComplete ? "Complete" : "Incomplete", ColorText = action.IsComplete ? Color.ForestGreen : Color.Red });
				row.Cells.Add(new GridCell(action.UserNum == 0 ? "" : Userods.GetName(action.UserNum)));
				row.Cells.Add(new GridCell(action.DateTimeComplete == DateTime.MinValue ? "" : action.DateTimeComplete.ToString("G")));
				gridActions.ListGridRows.Add(row);
			});
			gridActions.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}


	}
}