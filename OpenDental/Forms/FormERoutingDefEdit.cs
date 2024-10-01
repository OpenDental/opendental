using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
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

		public FormERoutingDefEdit(ERoutingDef patientFlowDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eRoutingDef=patientFlowDef;
			_listERoutingActionDefs=ERoutingActionDefs.GetAllByERoutingDef(_eRoutingDef.ERoutingDefNum);
			_listERoutingActionDefsOld=_listERoutingActionDefs.ToList();
			_listERoutingDefLinks=ERoutingDefLinks.GetWhere(x => x.ERoutingDefNum==_eRoutingDef.ERoutingDefNum);
			_descriptionOrig=patientFlowDef.Description;
		}

		#region load methods
		public void FormPatientFlowEdit_Load(object sender,EventArgs e) {
			textBoxDescription.Text=_eRoutingDef.Description;
			labelGenAppts.Visible=false;
			comboLinkType.Items.AddListEnum<EnumERoutingType>(new List<EnumERoutingType>() { EnumERoutingType.General, EnumERoutingType.Appointment, EnumERoutingType.BillingType } );
			FillActionsGrid();
			FillLinkTypesGrid();
		}

		private void FillActionsGrid() {
			int indexCur=gridERoutingActions.GetSelectedIndex();
			gridERoutingActions.BeginUpdate();
			gridERoutingActions.ListGridRows.Clear();
			gridERoutingActions.Columns.Clear();
			gridERoutingActions.Columns.Add(new GridColumn() { Heading=Lang.g(nameof(FormERoutingDefEdit),"Action Type"), IsWidthDynamic=true });
			gridERoutingActions.Columns.Add(new GridColumn() { Heading=Lang.g(nameof(FormERoutingDefEdit),"Label Override"), IsWidthDynamic=true});
			gridERoutingActions.Columns.Add(new GridColumn() { Heading=Lang.g(nameof(FormERoutingDefEdit),"Default"), ColWidth=60, TextAlign=HorizontalAlignment.Center});
			_listERoutingActionDefs.ForEach(x => {
				gridERoutingActions.ListGridRows.Add(
					new UI.GridRow(new string[]{x.ERoutingActionType.GetDescription(), x.LabelOverride, x.ForeignKey!=0 ? "X": ""}) { Tag=x }
					);
			});
			gridERoutingActions.EndUpdate();
			if(indexCur < gridERoutingActions.ListGridRows.Count){
				//If there was a previously selected entry, select it again. If it was deleted, who cares.
				gridERoutingActions.SetSelected(indexCur);
			}
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

		/// <summary>Opens an edit form for the chosen eRoutingActionDef.
		/// Refreshes page if saved. </summary>
		private void gridERoutingActions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(sender is GridOD grid) {
				if(grid.ListGridRows[e.Row].Tag is ERoutingActionDef eRoutingActionDef) {
					ShowERoutingActionDefEditHelper(eRoutingActionDef);
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//Automatically launch the edit window upon adding.
			ShowERoutingActionDefEditHelper(null);
		}

		/// <summary>Shows edit form, and manages the list of eRoutingActionDefs for new. </summary>
		private void ShowERoutingActionDefEditHelper(ERoutingActionDef eRoutingActionDef){
		bool isAddingNew=false;
			if(eRoutingActionDef==null){
			//If we are adding a new eRoutingActionDef, instanciate it and set a flag.
				isAddingNew=true;
				eRoutingActionDef=new ERoutingActionDef() { 
					ERoutingActionType=EnumERoutingActionType.None,
					ItemOrder=_listERoutingActionDefs.Count 
				};
			}
			FrmERoutingActionDefEdit frmERoutingActionDefEdit=new FrmERoutingActionDefEdit(eRoutingActionDef);
			if(frmERoutingActionDefEdit.ShowDialog()) {
				//If the eRouting action is new, add it to the list. Set the changed flag and refresh the UI.
				if(isAddingNew){
					_listERoutingActionDefs.Add(eRoutingActionDef);
					gridERoutingActions.SetSelected(_listERoutingActionDefs.Count-1);
				}
			}
			else{
			//User canelled or delted. If the eRoutingActionDef is null, remove it from the list.
				if(frmERoutingActionDefEdit.ERoutingActionDefCur==null){
					//ERoutingActionDef was deleted. re-assign action order.
					_listERoutingActionDefs.Remove(eRoutingActionDef);
					ReorderListItemOrders();
				}
			}
			_hasChanged=true;
			FillActionsGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			ERoutingActionDef eRoutingActionDef = gridERoutingActions.SelectedTag<ERoutingActionDef>();
			if(eRoutingActionDef==null || eRoutingActionDef.ItemOrder==0) {
				return;
			}
			_listERoutingActionDefs.Reverse(_listERoutingActionDefs.IndexOf(eRoutingActionDef)-1,2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridERoutingActions.SetSelected(_listERoutingActionDefs.IndexOf(eRoutingActionDef));
			_hasChanged=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			ERoutingActionDef eRoutingActionDef = gridERoutingActions.SelectedTag<ERoutingActionDef>();
			if(eRoutingActionDef==null || eRoutingActionDef.ItemOrder==_listERoutingActionDefs.Count-1) {
				return;
			}
			_listERoutingActionDefs.Reverse(_listERoutingActionDefs.IndexOf(eRoutingActionDef),2);
			ReorderListItemOrders();
			FillActionsGrid();
			gridERoutingActions.SetSelected(_listERoutingActionDefs.IndexOf(eRoutingActionDef));
			_hasChanged=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this eRouting Def?")) {
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

		private void butSave_Click(object sender,EventArgs e) {
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
		#endregion
	}
}