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
	public partial class FormFlowDefs:FormODBase {

		private long _clinicNum;
		private ClinicPrefHelper _clinicPrefHelper = new ClinicPrefHelper(PrefName.PatientFlowsUseHQDefaults
			);

		public FormFlowDefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientFlow_Load(object sender,EventArgs e) {
			if(!PrefC.HasClinicsEnabled) {
				comboClinic.Visible=false;
				checkUseDefault.Visible=false;
				_clinicNum=0;
			}
			else {
				_clinicNum = Clinics.ClinicNum;
				checkUseDefault.Checked=_clinicPrefHelper.GetBoolVal(PrefName.PatientFlowsUseHQDefaults,_clinicNum);
				checkUseDefault.Visible=_clinicNum!=0;
			}
			FillGrid();
		}

		private void FillGrid() {
			gridPatientFlows.BeginUpdate();
			gridPatientFlows.ListGridRows.Clear();
			gridPatientFlows.Columns.Clear();
			gridPatientFlows.Columns.Add(new UI.GridColumn() { Heading = "Description" });
			List<FlowDef> listPatientFlows = FlowDefs.GetByClinic(_clinicNum);
			listPatientFlows.ForEach(x => {
				gridPatientFlows.ListGridRows.Add(new UI.GridRow(x.Description) { Tag=x });
			});
			gridPatientFlows.EndUpdate();
		}

		private void ComboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			_clinicNum = comboClinic.SelectedClinicNum;
			checkUseDefault.Checked=_clinicPrefHelper.GetBoolVal(PrefName.PatientFlowsUseHQDefaults, _clinicNum);
			checkUseDefault.Visible=_clinicNum!=0;
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormFlowDefEdit formPatientFlowEdit = new FormFlowDefEdit(_clinicNum);
			if(formPatientFlowEdit.ShowDialog() == DialogResult.Cancel) {
				return;
			}
			Cache.Refresh(InvalidType.FlowDef);
			FillGrid();
		}

		/// <summary>Creates a copy of the selected item as well as the action defs associated with them. Inserts both and refills the gris.</summary>
		private void butDuplicate_ClicK(object sender,EventArgs e) {
			//duplicate currently selected item. Show message if nothing selected.
			if(gridPatientFlows.SelectedTag<FlowDef>() == null) {
				MsgBox.Show(this,"No item selected.");
				return;
			}
			FlowDef patientFlowDefOrig = gridPatientFlows.SelectedTag<FlowDef>();
			FlowDef patientFlowDefCopy = patientFlowDefOrig.Copy();
			long patientFlowDefNum = FlowDefs.Insert(patientFlowDefCopy);
			//get a copy of the actiondefs from the original item, inserting new copies pointing to our new patientflowdef.
			FlowActionDefs.GetAllByFlowDef(patientFlowDefOrig.FlowDefNum)
				.Select(x => FlowActionDefs.Insert(new FlowActionDef() { FlowDefNum = patientFlowDefNum, ItemOrder=x.ItemOrder, FlowActionType=x.FlowActionType }));
			Cache.Refresh(InvalidType.FlowDef);
			FillGrid();
		}

		private void checkUseDefault_Click(object sender,EventArgs e) {
			gridPatientFlows.Enabled=!checkUseDefault.Checked;
			if(checkUseDefault.Checked) {
				butAdd.Enabled=false;
				butDuplicate.Enabled=false;
				gridPatientFlows.Enabled=false;
				labelUseDefaults.Visible=true;
			}
			else {
				butAdd.Enabled=true;
				butDuplicate.Enabled=true;
				gridPatientFlows.Enabled=true;
				labelUseDefaults.Visible=false;
			}
			_clinicPrefHelper.ValChangedByUser(PrefName.PatientFlowsUseHQDefaults,_clinicNum, checkUseDefault.Checked.ToString());
		}

		private void GridCell_DoubleClick(object sender, ODGridClickEventArgs e) {
			_clinicPrefHelper.SyncAllPrefs();
			using FormFlowDefEdit formPatientFlowEdit = new FormFlowDefEdit(gridPatientFlows.SelectedTag<FlowDef>());
			if(formPatientFlowEdit.ShowDialog() == DialogResult.OK) {
				FillGrid();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			_clinicPrefHelper.SyncAllPrefs();
			Cache.Refresh(InvalidType.FlowDef);
			DialogResult=DialogResult.OK;
		}
	}
}