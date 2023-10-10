using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using Interop.QBFC10;
using Microsoft.VisualBasic;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormERoutingDefEdit:FormODBase {

		private ERoutingDef _eRoutingDef;
		private List<ERoutingActionDef> _listERoutingActionDefs;
		private List<ERoutingActionDef> _listERoutingActionDefsOld;
		private List<ERoutingDefLink> _listERoutingDefLinks;
		private bool _hasChanged=false;
		private string _descriptionOrig;


		public FormERoutingDefEdit(long clinicNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eRoutingDef=new ERoutingDef() { IsNew=true };
			_eRoutingDef.ClinicNum=clinicNum;
			_listERoutingActionDefs=new List<ERoutingActionDef>();
			_listERoutingActionDefsOld=new List<ERoutingActionDef>();
			_listERoutingDefLinks=new List<ERoutingDefLink>();
		}

		public FormERoutingDefEdit(ERoutingDef eRoutingDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eRoutingDef=eRoutingDef;
			_listERoutingActionDefs=ERoutingActionDefs.GetAllByERoutingDef(_eRoutingDef.ERoutingDefNum);
			_listERoutingActionDefsOld=_listERoutingActionDefs.ToList();
			_listERoutingDefLinks=ERoutingDefLinks.GetWhere(x => x.ERoutingDefNum==_eRoutingDef.ERoutingDefNum);
			_descriptionOrig=eRoutingDef.Description;
		}

		#region load methods
		public void FormPatientFlowEdit_Load(object sender,EventArgs e) {
			textBoxDescription.Text=_eRoutingDef.Description;
			labelGenAppts.Visible=false;
			comboActionType.Items.AddEnums<EnumERoutingActionType>();
			comboLinkType.Items.AddListEnum<EnumERoutingType>(new List<EnumERoutingType>() { EnumERoutingType.General, EnumERoutingType.Appointment, EnumERoutingType.BillingType } );
			FillActionsGrid();
			FillLinkTypesGrid();
		}

		private void FillActionsGrid() {
			gridPatientERoutingActions.BeginUpdate();
			gridPatientERoutingActions.ListGridRows.Clear();
			gridPatientERoutingActions.Columns.Clear();
			gridPatientERoutingActions.Columns.Add(new UI.GridColumn() { Heading="Action Type" });
			_listERoutingActionDefs.ForEach(x => {
				gridPatientERoutingActions.ListGridRows.Add(
					new UI.GridRow(x.ERoutingActionType.GetDescription()) { Tag=x }
					);
			});
			gridPatientERoutingActions.EndUpdate();
		}

		private void FillLinkTypesGrid() {
			gridLinkTypes.BeginUpdate();
			gridLinkTypes.ListGridRows.Clear();
			gridLinkTypes.Columns.Clear();
			gridLinkTypes.Columns.Add(new UI.GridColumn() { Heading="Triggers" });
			_listERoutingDefLinks.ForEach(x => {
				string columnVal=x.ERoutingType.GetDescription();
				if(x.Fkey > 0) {
					switch(x.ERoutingType) {
						case EnumERoutingType.Appointment:
							columnVal=$"Appt Type: {AppointmentTypes.GetName(x.Fkey)}";
							break;
						case EnumERoutingType.BillingType:
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
			_listERoutingActionDefs.ForEach(x => x.ItemOrder=i++);
		}

		private void Save() {
			_eRoutingDef.Description=textBoxDescription.Text;
			if(_eRoutingDef.IsNew) {
				_eRoutingDef.UserNumCreated=Security.CurUser.UserNum;
				long defNum=ERoutingDefs.Insert(_eRoutingDef);
				_eRoutingDef.ERoutingDefNum = defNum;//set this here because we are going to use it later.
				_listERoutingActionDefs.ForEach(x => {
					x.ERoutingDefNum=defNum;
					ERoutingActionDefs.Upsert(x);
				});
			}
			else if(_hasChanged || _eRoutingDef.Description != _descriptionOrig) {
				_eRoutingDef.UserNumModified=Security.CurUser.UserNum;
				ERoutingDefs.Update(_eRoutingDef);
				_listERoutingActionDefs.ForEach(x => {
					x.ERoutingDefNum=_eRoutingDef.ERoutingDefNum;
					ERoutingActionDefs.Upsert(x);
				});
				for(int i=0;i<_listERoutingActionDefsOld.Count;i++) {
					ERoutingActionDef eRoutingActionDef=_listERoutingActionDefs.Find(x => x.ERoutingActionDefNum==_listERoutingActionDefsOld[i].ERoutingActionDefNum);
					if(eRoutingActionDef==null) {
						ERoutingActionDefs.Delete(_listERoutingActionDefsOld[i].ERoutingActionDefNum);
					}
				}
			}
			//We blindly delete here because we only want to have what is currently selected, and only then if they have the appropriate link type selected.
			ERoutingDefLinks.DeleteAll(_eRoutingDef.ERoutingDefNum);
			_listERoutingDefLinks.ForEach(x => {
				x.ERoutingDefNum=_eRoutingDef.ERoutingDefNum;
				ERoutingDefLinks.Insert(x);
			});
			Cache.Refresh(InvalidType.ERoutingDef);
		}

		#endregion

		#region Event Handlers

		private void butAdd_Click(object sender,EventArgs e) {
			if(comboActionType.SelectedItem == null) {
				MsgBox.Show(this,"Please select an Action to add.");
				return;
			}
			_listERoutingActionDefs.Add(new ERoutingActionDef() { 
				ERoutingActionType=(EnumERoutingActionType)comboActionType.SelectedItem,
				ItemOrder=_listERoutingActionDefs.Count 
			});
			FillActionsGrid();
			_hasChanged=true;
			gridPatientERoutingActions.SetSelected(_listERoutingActionDefs.Count-1);
		}

		private void butRemove_Click(object sender,EventArgs e) {
			_listERoutingActionDefs.Remove(gridPatientERoutingActions.SelectedTag<ERoutingActionDef>());
			ReorderListItemOrders();
			FillActionsGrid();
			_hasChanged=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			ERoutingActionDef ERoutingActionDef = gridPatientERoutingActions.SelectedTag<ERoutingActionDef>();
			if(ERoutingActionDef==null || ERoutingActionDef.ItemOrder==0) {
				return;
			}
			_listERoutingActionDefs.Reverse(_listERoutingActionDefs.IndexOf(ERoutingActionDef)-1,2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridPatientERoutingActions.SetSelected(_listERoutingActionDefs.IndexOf(ERoutingActionDef));
			_hasChanged=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			ERoutingActionDef eRoutingActionDef = gridPatientERoutingActions.SelectedTag<ERoutingActionDef>();
			if(eRoutingActionDef==null || eRoutingActionDef.ItemOrder==_listERoutingActionDefs.Count) {
				return;
			}
			_listERoutingActionDefs.Reverse(_listERoutingActionDefs.IndexOf(eRoutingActionDef),2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridPatientERoutingActions.SetSelected(_listERoutingActionDefs.IndexOf(eRoutingActionDef));
			_hasChanged=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this Patient Flow?")) {
				return;
			}
			_listERoutingActionDefs.ForEach(x => ERoutingActionDefs.Delete(x.ERoutingActionDefNum));
			ERoutingDefLinks.DeleteAll(_eRoutingDef.ERoutingDefNum);
			ERoutingDefs.Delete(_eRoutingDef.ERoutingDefNum);
			Cache.Refresh(InvalidType.ERoutingDef);
			DialogResult=DialogResult.OK;
		}

		private void comboLinkTypes_SelectionChangeCommitted(object sender, EventArgs e) {
			butAddLinkType.Enabled=true;
			labelGenAppts.Visible=false;
			butAddSpecificTypes.Visible=false;
			switch(comboLinkType.SelectedItem) {
				case EnumERoutingType.General:
					break;
				case EnumERoutingType.Appointment:
					butAddSpecificTypes.Visible =true;
					labelGenAppts.Visible=true;
					break;
				default:
					break;
			}
		}

		private void butAddLinkType_Click(object sender,EventArgs e) {
			if(comboLinkType.SelectedItem == null) {
				MsgBox.Show(this, "Please select a Link Type to add.");
				return;
			}
			ERoutingDefLink eRoutingDefLink = new ERoutingDefLink();
			eRoutingDefLink.Fkey=0;
			eRoutingDefLink.ERoutingDefNum=_eRoutingDef.ERoutingDefNum;
			eRoutingDefLink.ERoutingType = (EnumERoutingType)comboLinkType.SelectedItem;
			if (eRoutingDefLink.ERoutingType == EnumERoutingType.BillingType) {
				OpenFormBillingTypes();
				return;
			}
			_listERoutingDefLinks.Add(eRoutingDefLink);
			FillLinkTypesGrid();
		}

		private void butAddSpecificTypes_Click(object sender,EventArgs e) {
			switch(comboLinkType.SelectedItem) {
				case EnumERoutingType.Appointment:
					OpenFormApptTypes();
					break;
				default:
					break;
			}
		}

		private void OpenFormBillingTypes() {
			List<Def> selectedDefs = Defs.GetDefs(DefCat.BillingTypes,_listERoutingDefLinks.Where(x => x.ERoutingType==EnumERoutingType.BillingType && x.Fkey>0).Select(x => x.Fkey).ToList());
			using FormDefinitionPicker formDefPicker = new FormDefinitionPicker(DefCat.BillingTypes,selectedDefs);
			formDefPicker.IsMultiSelectionMode=true;
			if(formDefPicker.ShowDialog() == DialogResult.OK) {
				formDefPicker.ListDefsSelected.ForEach(billingType => {
					_listERoutingDefLinks.Add(new ERoutingDefLink() { Fkey=billingType.DefNum,ERoutingType=EnumERoutingType.BillingType,ERoutingDefNum=_eRoutingDef.ERoutingDefNum });
				});
				FillLinkTypesGrid();
			}
		}

		private void OpenFormApptTypes() {
			using FormApptTypes formApptTypes = new FormApptTypes();
			formApptTypes.IsSelectionMode=true;
			formApptTypes.AllowMultipleSelections=true;
			List<long> listApptTypesSelected = _listERoutingDefLinks.Where(x => x.ERoutingType==EnumERoutingType.Appointment && x.Fkey>0).Select(y => y.Fkey).ToList();
			formApptTypes.ListAppointmentTypesSelected = AppointmentTypes.GetWhere(x => listApptTypesSelected.Contains(x.AppointmentTypeNum));
			if(formApptTypes.ShowDialog() == DialogResult.OK) {
				formApptTypes.ListAppointmentTypesSelected.ForEach(apptType => {
					_listERoutingDefLinks.Add(new ERoutingDefLink() { Fkey=apptType.AppointmentTypeNum,ERoutingType=EnumERoutingType.Appointment,ERoutingDefNum=_eRoutingDef.ERoutingDefNum });
				});
				FillLinkTypesGrid();
			}
		}

		private void butRemoveLinkType_Click(object sender,EventArgs e) {
			_listERoutingDefLinks.Remove(gridLinkTypes.SelectedTag<ERoutingDefLink>());
			FillLinkTypesGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textBoxDescription.Text.Trim().IsNullOrEmpty()) {
				MsgBox.Show(this,"Description cannot be blank. Please enter a description.");
				return;
			}
			if(_listERoutingActionDefs.Count==0) {
				MsgBox.Show(this, "Please add at least one action.");
				return;
			}
			Save();
			Cache.Refresh(InvalidType.ERoutingDef);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion

		
	}
}