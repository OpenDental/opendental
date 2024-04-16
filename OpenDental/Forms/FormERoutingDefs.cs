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
	public partial class FormERoutingDefs:FormODBase {

		private long _clinicNum;
		private ClinicPrefHelper _clinicPrefHelper = new ClinicPrefHelper(PrefName.ERoutingUseHQDefaults);

		public FormERoutingDefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientFlow_Load(object sender,EventArgs e) {	
			comboClinic.Visible=PrefC.HasClinicsEnabled;
			ChangeClinic(Clinics.ClinicNum);
			FillGrid();
		}

		private void FillGrid() {
			gridERouting.BeginUpdate();
			gridERouting.ListGridRows.Clear();
			gridERouting.Columns.Clear();
			gridERouting.Columns.Add(new UI.GridColumn() { Heading = "Description" });
			List<ERoutingDef> listERouting = ERoutingDefs.GetByClinic(_clinicNum);
			listERouting.ForEach(x => {
				gridERouting.ListGridRows.Add(new UI.GridRow(x.Description) { Tag=x });
			});
			gridERouting.EndUpdate();
		}

		private void ComboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			ChangeClinic(comboClinic.ClinicNumSelected);
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormERoutingDefEdit formERoutingEdit = new FormERoutingDefEdit(_clinicNum);
			if(formERoutingEdit.ShowDialog() == DialogResult.Cancel) {
				return;
			}
			Cache.Refresh(InvalidType.ERoutingDef);
			FillGrid();
		}

		/// <summary>Creates a copy of the selected item as well as the action defs associated with them. Inserts both and refills the gris.</summary>
		private void butDuplicate_ClicK(object sender,EventArgs e) {
			//duplicate currently selected item. Show message if nothing selected.
			if(gridERouting.SelectedTag<ERoutingDef>() == null) {
				MsgBox.Show(this,"No item selected.");
				return;
			}
			ERoutingDef eRoutingDefOrig=gridERouting.SelectedTag<ERoutingDef>();
			ERoutingDef eRoutingDefCopy=eRoutingDefOrig.Copy();
			long eRoutingDefNum=ERoutingDefs.Insert(eRoutingDefCopy);
			//get a copy of the actiondefs from the original item, inserting new copies pointing to our new patientflowdef.
			List<ERoutingActionDef> listERoutingActions=ERoutingActionDefs.GetAllByERoutingDef(eRoutingDefOrig.ERoutingDefNum);
			for(int i=0; i<listERoutingActions.Count; ++i) {
				ERoutingActionDef newAction=new ERoutingActionDef();
				newAction.ERoutingDefNum=eRoutingDefNum;
				newAction.ItemOrder=listERoutingActions[i].ItemOrder;
				newAction.ERoutingActionType=listERoutingActions[i].ERoutingActionType;
				ERoutingActionDefs.Insert(newAction);
			}
			List<ERoutingDefLink> listERoutingDefLinks=ERoutingDefLinks.GetListERoutingTypesForERoutingDefNum(eRoutingDefOrig.ERoutingDefNum);
			for(int i=0; i<listERoutingDefLinks.Count; ++i) {
				ERoutingDefLink ERoutingDefLink=new ERoutingDefLink();
				ERoutingDefLink.ERoutingDefNum=eRoutingDefNum;
				ERoutingDefLink.ERoutingType=listERoutingDefLinks[i].ERoutingType;
				ERoutingDefLink.Fkey=listERoutingDefLinks[i].Fkey;
				ERoutingDefLinks.Insert(ERoutingDefLink);
			}
			Cache.Refresh(InvalidType.ERoutingDef);
			FillGrid();
		}

		private void checkUseDefault_Click(object sender,EventArgs e) {
			gridERouting.Enabled=!checkUseDefault.Checked;
			if(checkUseDefault.Checked) {
				butAdd.Enabled=false;
				butDuplicate.Enabled=false;
				gridERouting.Enabled=false;
				labelUseDefaults.Visible=true;
			}
			else {
				butAdd.Enabled=true;
				butDuplicate.Enabled=true;
				gridERouting.Enabled=true;
				labelUseDefaults.Visible=false;
			}
			_clinicPrefHelper.ValChangedByUser(PrefName.ERoutingUseHQDefaults,_clinicNum, checkUseDefault.Checked.ToString());
		}

		private void GridCell_DoubleClick(object sender, ODGridClickEventArgs e) {
			_clinicPrefHelper.SyncAllPrefs();
			using FormERoutingDefEdit formERoutingDefEdit = new FormERoutingDefEdit(gridERouting.SelectedTag<ERoutingDef>());
			if(formERoutingDefEdit.ShowDialog() == DialogResult.OK) {
				FillGrid();
			}
		}

		private void FormERoutingDefs_FormClosing(object sender,FormClosingEventArgs e) {
			_clinicPrefHelper.SyncAllPrefs();
			Cache.Refresh(InvalidType.ERoutingDef);
		}

		private void ChangeClinic(long clinicNum) {
			_clinicNum=clinicNum;
			checkUseDefault.Checked=_clinicPrefHelper.GetBoolVal(PrefName.ERoutingUseHQDefaults,_clinicNum);
			checkUseDefault.Visible=_clinicNum!=0;
			labelUseDefaults.Visible=_clinicNum!=0 && checkUseDefault.Checked;
			bool isEditingEnabled=_clinicNum==0 || !_clinicPrefHelper.GetBoolVal(PrefName.ERoutingUseHQDefaults, _clinicNum);
			butAdd.Enabled=isEditingEnabled;
			gridERouting.Enabled=isEditingEnabled;
			butDuplicate.Enabled=isEditingEnabled;
		}
	}
}