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
	public partial class FormFlows:FormODBase {

		private long _clinicNum;
		private List<Flow> _listFlows = new List<Flow>();
		private Patient _patient;

		public FormFlows() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientFlows_Load(object sender,EventArgs e) {
			_clinicNum = Clinics.ClinicNum;
			LayoutMenu();
			FillGrid();
		}

		public void LayoutMenu() {
			menuSetup.BeginUpdate();
			menuSetup.Add(new MenuItemOD("Setup",menuSetup_Click));
			menuSetup.EndUpdate();
		}

		private void FillGrid() {
			_listFlows = Flows.GetAllForClinicInDateRange(_clinicNum, datePicker.GetDateTimeFrom(), datePicker.GetDateTimeTo(), comboClinic.IsAllSelected);
			if(_patient != null)
			{
				_listFlows = _listFlows.Where(x => x.PatNum == _patient.PatNum).ToList();
			}
			gridFlows.BeginUpdate();
			gridFlows.Columns.Clear();
			gridFlows.ListGridRows.Clear();
			gridFlows.Columns.Add(new GridColumn("Date", 150, textAlign: HorizontalAlignment.Center));
			gridFlows.Columns.Add(new GridColumn("Patient", 170,textAlign: HorizontalAlignment.Center));
			gridFlows.Columns.Add(new GridColumn("Description", 225, textAlign: HorizontalAlignment.Center));
			gridFlows.Columns.Add(new GridColumn("Status", 80, textAlign: HorizontalAlignment.Center));
			_listFlows.ForEach(flow => {
				GridRow row = new GridRow();

				row.Cells.Add(new GridCell() { Text = flow.SecDateTEntry.ToString("G") });
				row.Cells.Add(new GridCell() { Text = Patients.GetNameFL(flow.PatNum) });
				row.Cells.Add(new GridCell() { Text = flow.Description });
				row.Cells.Add(new GridCell() { Text = flow.IsComplete ? "Complete" : "Incomplete", ColorText = flow.IsComplete ? Color.ForestGreen : Color.Red });
				row.Tag = flow;
				gridFlows.ListGridRows.Add(row);
			});
			gridFlows.EndUpdate();
		}

		private void menuSetup_Click(object sender,EventArgs e) {
			using FormFlowDefs formPatientFlowDefs = new FormFlowDefs();
			formPatientFlowDefs.ShowDialog();
		}

		private void comboClinic_SelectionChangeCommitted(object sender, EventArgs e) {
			_clinicNum=comboClinic.SelectedClinicNum;
			FillGrid();
		}

		private void butSelectPatient_Click(object sender, EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			if (formPatientSelect.ShowDialog() == DialogResult.OK)
			{
				_patient=Patients.GetPat(formPatientSelect.PatNumSelected);
				textBoxPatName.Text=_patient.GetNameFL();
				FillGrid();
			}
		}


		private void butShowAll_Click(object sender, EventArgs e) {
			_patient = null;
			textBoxPatName.Text = "";
			FillGrid();
		}

		private void gridFlowsCellDoubleClick(object sender, ODGridClickEventArgs e) {
			using FormFlow formFlow = new FormFlow(gridFlows.SelectedTag<Flow>().FlowNum);
			formFlow.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}