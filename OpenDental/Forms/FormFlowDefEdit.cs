using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using Microsoft.VisualBasic;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFlowDefEdit:FormODBase {

		private FlowDef _flowDef;
		private List<FlowActionDef> _listFlowActionDefs;
		private List<FlowDefLink> _listFlowDefLinks;
		private bool _hasChanged=false;
		private string _descriptionOrig;


		public FormFlowDefEdit(long clinicNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_flowDef=new FlowDef() { IsNew=true };
			_flowDef.ClinicNum=clinicNum;
			_listFlowActionDefs=new List<FlowActionDef>();
			_listFlowDefLinks=new List<FlowDefLink>();
		}

		public FormFlowDefEdit(FlowDef patientFlowDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_flowDef=patientFlowDef;
			_listFlowActionDefs=FlowActionDefs.GetAllByFlowDef(_flowDef.FlowDefNum);
			_listFlowDefLinks=FlowDefLinks.GetWhere(x => x.FlowDefNum==_flowDef.FlowDefNum);
			_descriptionOrig=patientFlowDef.Description;
		}

		#region load methods
		public void FormPatientFlowEdit_Load(object sender,EventArgs e) {
			textBoxDescription.Text=_flowDef.Description;
			labelGenAppts.Visible=false;
			comboActionType.Items.AddEnums<EnumFlowActionType>();
			comboLinkType.Items.AddListEnum<EnumFlowType>(new List<EnumFlowType>() { EnumFlowType.General, EnumFlowType.Appointment, EnumFlowType.BillingType } );
			FillActionsGrid();
			FillLinkTypesGrid();
		}

		private void FillActionsGrid() {
			gridPatientFlowActions.BeginUpdate();
			gridPatientFlowActions.ListGridRows.Clear();
			gridPatientFlowActions.Columns.Clear();
			gridPatientFlowActions.Columns.Add(new UI.GridColumn() { Heading="Action Type" });
			_listFlowActionDefs.ForEach(x => {
				gridPatientFlowActions.ListGridRows.Add(
					new UI.GridRow(x.FlowActionType.GetDescription()) { Tag=x }
					);
			});
			gridPatientFlowActions.EndUpdate();
		}

		private void FillLinkTypesGrid() {
			gridLinkTypes.BeginUpdate();
			gridLinkTypes.ListGridRows.Clear();
			gridLinkTypes.Columns.Clear();
			gridLinkTypes.Columns.Add(new UI.GridColumn() { Heading="Triggers" });
			_listFlowDefLinks.ForEach(x => {
				string columnVal=x.FlowType.GetDescription();
				if(x.Fkey > 0) {
					switch(x.FlowType) {
						case EnumFlowType.Appointment:
							columnVal=$"Appt Type: {AppointmentTypes.GetName(x.Fkey)}";
							break;
						case EnumFlowType.BillingType:
							columnVal=$"Billing Type: {Defs.GetName(DefCat.BillingTypes,x.Fkey)}";
							break;
						default:
							break;
					}
				}
				gridLinkTypes.ListGridRows.Add(
					new UI.GridRow(columnVal) { Tag=x }
					);
			});
			gridLinkTypes.EndUpdate();
		}
		#endregion

		#region helper methods
		/// <summary>Resets item ordering based on what is currently in the list.</summary>
		private void ReorderListItemOrders() {
			int i=0;
			_listFlowActionDefs.ForEach(x => x.ItemOrder=i++);
		}

		private void Save() {
			_flowDef.Description=textBoxDescription.Text;
			if(_flowDef.IsNew) {
				_flowDef.UserNumCreated=Security.CurUser.UserNum;
				long defNum=FlowDefs.Insert(_flowDef);
				_flowDef.FlowDefNum = defNum;//set this here because we are going to use it later.
				_listFlowActionDefs.ForEach(x => {
					x.FlowDefNum=defNum;
					FlowActionDefs.Upsert(x);
				});
			}
			else if(_hasChanged || _flowDef.Description != _descriptionOrig) {
				_flowDef.UserNumModified=Security.CurUser.UserNum;
				FlowDefs.Update(_flowDef);
				_listFlowActionDefs.ForEach(x => {
					x.FlowDefNum=_flowDef.FlowDefNum;
					FlowActionDefs.Upsert(x);
				});
			}
			//We blindly delete here because we only want to have what is currently selected, and only then if they have the appropriate link type selected.
			FlowDefLinks.DeleteAll(_flowDef.FlowDefNum);
			_listFlowDefLinks.ForEach(x => {
				x.FlowDefNum=_flowDef.FlowDefNum;
				FlowDefLinks.Insert(x);
			});
			Cache.Refresh(InvalidType.FlowDef, InvalidType.FlowActionDef, InvalidType.FlowDefLink);
		}

		#endregion

		#region Event Handlers

		private void butAdd_Click(object sender,EventArgs e) {
			if(comboActionType.SelectedItem == null) {
				MsgBox.Show(this, "Please select an Action to add.");
				return;
			}
			_listFlowActionDefs.Add(new FlowActionDef() { 
				FlowActionType=(EnumFlowActionType)comboActionType.SelectedItem,
				ItemOrder=_listFlowActionDefs.Count 
			});
			FillActionsGrid();
			_hasChanged=true;
			gridPatientFlowActions.SetSelected(_listFlowActionDefs.Count-1);
		}

		private void butRemove_Click(object sender,EventArgs e) {
			_listFlowActionDefs.Remove(gridPatientFlowActions.SelectedTag<FlowActionDef>());
			ReorderListItemOrders();
			FillActionsGrid();
			_hasChanged=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			FlowActionDef patientFlowActionDef = gridPatientFlowActions.SelectedTag<FlowActionDef>();
			if(patientFlowActionDef==null || patientFlowActionDef.ItemOrder==0) {
				return;
			}
			_listFlowActionDefs.Reverse(_listFlowActionDefs.IndexOf(patientFlowActionDef)-1,2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridPatientFlowActions.SetSelected(_listFlowActionDefs.IndexOf(patientFlowActionDef));
			_hasChanged=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			FlowActionDef patientFlowActionDef = gridPatientFlowActions.SelectedTag<FlowActionDef>();
			if(patientFlowActionDef==null || patientFlowActionDef.ItemOrder==_listFlowActionDefs.Count) {
				return;
			}
			_listFlowActionDefs.Reverse(_listFlowActionDefs.IndexOf(patientFlowActionDef),2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridPatientFlowActions.SetSelected(_listFlowActionDefs.IndexOf(patientFlowActionDef));
			_hasChanged=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this Patient Flow?")) {
				return;
			}
			_listFlowActionDefs.ForEach(x => FlowActionDefs.Delete(x.FlowActionDefNum));
			FlowDefLinks.DeleteAll(_flowDef.FlowDefNum);
			FlowDefs.Delete(_flowDef.FlowDefNum);
			Cache.Refresh(InvalidType.FlowDef);
			DialogResult=DialogResult.OK;
		}

		private void comboLinkTypes_SelectionChangeCommitted(object sender, EventArgs e) {
			butAddLinkType.Enabled=true;
			labelGenAppts.Visible=false;
			butAddSpecificTypes.Visible=false;
			switch(comboLinkType.SelectedItem) {
				case EnumFlowType.General:
					break;
				case EnumFlowType.Appointment:
					butAddSpecificTypes.Visible =true;
					labelGenAppts.Visible=true;
					break;
				default:
					break;
			}
		}

		private void butAddLinkType_Click(object sender,EventArgs e) {
			if(comboLinkType.SelectedItem == null) {
				MsgBox.Show(this, "Please select a trigger to add.");
				return;
			}
			FlowDefLink flowDefLink = new FlowDefLink();
			flowDefLink.Fkey=0;
			flowDefLink.FlowDefNum=_flowDef.FlowDefNum;
			flowDefLink.FlowType = (EnumFlowType)comboLinkType.SelectedItem;
			if (flowDefLink.FlowType == EnumFlowType.BillingType) {
				OpenFormBillingTypes();
				return;
			}
			_listFlowDefLinks.Add(flowDefLink);
			FillLinkTypesGrid();
		}

		private void butAddSpecificTypes_Click(object sender,EventArgs e) {
			switch(comboLinkType.SelectedItem) {
				case EnumFlowType.Appointment:
					OpenFormApptTypes();
					break;
				default:
					break;
			}
		}

		private void OpenFormBillingTypes() {
			List<Def> selectedDefs = Defs.GetDefs(DefCat.BillingTypes,_listFlowDefLinks.Where(x => x.FlowType==EnumFlowType.BillingType && x.Fkey>0).Select(x => x.Fkey).ToList());
			using FormDefinitionPicker formDefPicker = new FormDefinitionPicker(DefCat.BillingTypes,selectedDefs);
			formDefPicker.IsMultiSelectionMode=true;
			if(formDefPicker.ShowDialog() == DialogResult.OK) {
				formDefPicker.ListDefsSelected.ForEach(billingType => {
					_listFlowDefLinks.Add(new FlowDefLink() { Fkey=billingType.DefNum,FlowType=EnumFlowType.BillingType,FlowDefNum=_flowDef.FlowDefNum });
				});
				FillLinkTypesGrid();
			}
		}

		private void OpenFormApptTypes() {
			using FormApptTypes formApptTypes = new FormApptTypes();
			formApptTypes.IsSelectionMode=true;
			formApptTypes.AllowMultipleSelections=true;
			List<long> listApptTypesSelected = _listFlowDefLinks.Where(x => x.FlowType==EnumFlowType.Appointment && x.Fkey>0).Select(y => y.Fkey).ToList();
			formApptTypes.ListAppointmentTypesSelected = AppointmentTypes.GetWhere(x => listApptTypesSelected.Contains(x.AppointmentTypeNum));
			if(formApptTypes.ShowDialog() == DialogResult.OK) {
				formApptTypes.ListAppointmentTypesSelected.ForEach(apptType => {
					_listFlowDefLinks.Add(new FlowDefLink() { Fkey=apptType.AppointmentTypeNum,FlowType=EnumFlowType.Appointment,FlowDefNum=_flowDef.FlowDefNum });
				});
				FillLinkTypesGrid();
			}
		}

		private void butRemoveLinkType_Click(object sender,EventArgs e) {
			_listFlowDefLinks.Remove(gridLinkTypes.SelectedTag<FlowDefLink>());
			FillLinkTypesGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textBoxDescription.Text.Trim().IsNullOrEmpty()) {
				MsgBox.Show(this,"Description cannot be blank. Please enter a description.");
				return;
			}
			if(_listFlowActionDefs.IsNullOrEmpty()) {
				MsgBox.Show(this, "Must have at least one action in flow definition.");
				return;
			}
			Save();
			Cache.Refresh(InvalidType.FlowDef);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion

		
	}
}