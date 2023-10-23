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
	public partial class FormERouting:FormODBase {

		private long _eRoutingNum;
		private List<ERoutingAction> _listActions;
		private ERouting _eRouting;

		public FormERouting(long eRoutingNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eRoutingNum=eRoutingNum;
		}

		private void FormPatientFlow_Load(object sender,EventArgs e) {
			_eRouting = ERoutings.GetOne(_eRoutingNum);
			_listActions = ERoutingActions.GetListForERouting(_eRoutingNum);
			textDate.Text = _eRouting.SecDateTEntry.ToString("G");
			textDescription.Text = _eRouting.Description;
			textPatName.Text = Patients.GetNameFL(_eRouting.PatNum);
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
				row.Cells.Add(new GridCell(action.ERoutingActionType.GetDescription()));
				row.Cells.Add(new GridCell() { Text = action.IsComplete ? "Complete" : "Incomplete", ColorText = action.IsComplete ? Color.ForestGreen : Color.Red });
				row.Cells.Add(new GridCell(action.UserNum == 0 ? "" : Userods.GetName(action.UserNum)));
				row.Cells.Add(new GridCell(action.DateTimeComplete == DateTime.MinValue ? "" : action.DateTimeComplete.ToString("G")));
				gridActions.ListGridRows.Add(row);
			});
			gridActions.EndUpdate();
		}

	}
}