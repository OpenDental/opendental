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
	public partial class FormERoutings:FormODBase {

		private long _clinicNum;
		private List<ERouting> _listERouting = new List<ERouting>();
		private Patient _patient;

		public FormERoutings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientFlows_Load(object sender,EventArgs e) {
			_clinicNum = Clinics.ClinicNum;
			datePicker.SetDateTimeFrom(DateTime.Now.AddDays(-7));
			datePicker.SetDateTimeTo(DateTime.Now);
			LayoutMenu();
			FillGrid();
		}

		public void LayoutMenu() {
			menuSetup.BeginUpdate();
			menuSetup.Add(new MenuItemOD("Setup",menuSetup_Click));
			menuSetup.EndUpdate();
		}

		private void FillGrid() {
			_listERouting = ERoutings.GetAllForClinicInDateRange(_clinicNum, datePicker.GetDateTimeFrom(), datePicker.GetDateTimeTo(), comboClinic.IsAllSelected);
			if(_patient != null)
			{
				_listERouting = _listERouting.Where(x => x.PatNum == _patient.PatNum).ToList();
			}
			gridERoutings.BeginUpdate();
			gridERoutings.Columns.Clear();
			gridERoutings.ListGridRows.Clear();
			gridERoutings.Columns.Add(new GridColumn("Date", 150, textAlign: HorizontalAlignment.Center));
			gridERoutings.Columns.Add(new GridColumn("Patient", 170,textAlign: HorizontalAlignment.Center));
			gridERoutings.Columns.Add(new GridColumn("Description", 225, textAlign: HorizontalAlignment.Center));
			gridERoutings.Columns.Add(new GridColumn("Status", 80, textAlign: HorizontalAlignment.Center));
			_listERouting.ForEach(eRouting => {
				GridRow row = new GridRow();
				row.Cells.Add(new GridCell() { Text = eRouting.SecDateTEntry.ToString("G") });
				row.Cells.Add(new GridCell() { Text = Patients.GetNameFL(eRouting.PatNum) });
				row.Cells.Add(new GridCell() { Text = eRouting.Description });
				row.Cells.Add(new GridCell() { Text = eRouting.IsComplete ? "Complete" : "Incomplete", ColorText = eRouting.IsComplete ? Color.ForestGreen : Color.Red });
				row.Tag = eRouting;
				gridERoutings.ListGridRows.Add(row);
			});
			gridERoutings.EndUpdate();
		}

		private void menuSetup_Click(object sender,EventArgs e) {
			using FormERoutingDefs formERoutingDefs = new FormERoutingDefs();
			formERoutingDefs.ShowDialog();
		}

		private void comboClinic_SelectionChangeCommitted(object sender, EventArgs e) {
			_clinicNum=comboClinic.ClinicNumSelected;
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
			using FormERouting formERouting = new FormERouting(gridERoutings.SelectedTag<ERouting>().ERoutingNum);
			formERouting.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

	}
}