using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormPatFieldDefs:FormODBase {
		public PatFieldDef PatFieldDefSelected;
		///<summary>This is not for tracking the pref.</summary>
		private bool _hasChanged;
		private List<PatFieldDef> _listPatFieldDefs;
		private bool _isSelectionMode;

		///<summary></summary>
		public FormPatFieldDefs(bool isSelectionModeOnly=false) {
			_isSelectionMode=isSelectionModeOnly;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatFieldDefs_Load(object sender, EventArgs e) {
			_listPatFieldDefs=PatFieldDefs.GetDeepCopy();
			checkDisplayRenamed.Checked=PrefC.GetBool(PrefName.DisplayRenamedPatFields);
			if(_isSelectionMode) {
				_listPatFieldDefs=_listPatFieldDefs.FindAll(x => !x.IsHidden);
				butAdd.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
			}
			LayoutMenu();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void FillGrid() {
			_listPatFieldDefs=PatFieldDefs.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			gridMain.AllowSortingByColumn=true;
			gridColumn=new GridColumn(Lan.g(this,"Field Name"),200);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lan.g(this,"Field Type"),100);
			gridMain.Columns.Add(gridColumn);
			if(!_isSelectionMode) {
				gridColumn=new GridColumn(Lan.g(this,"Hidden"),150,HorizontalAlignment.Center);
				gridMain.Columns.Add(gridColumn);
			}
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(_listPatFieldDefs[i].FieldName);
				gridRow.Cells.Add(_listPatFieldDefs[i].FieldType.GetDescription());
				if(!_isSelectionMode) {
					gridRow.Cells.Add(_listPatFieldDefs[i].IsHidden?"X":"");
				}
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isSelectionMode) {
				PatFieldDefSelected=_listPatFieldDefs[e.Row];
				DialogResult=DialogResult.OK;
				return;
			}
			using FormPatFieldDefEdit formPatFieldDefEdit=new FormPatFieldDefEdit();
			formPatFieldDefEdit.PatFieldDefCur=_listPatFieldDefs[e.Row];
			formPatFieldDefEdit.ShowDialog();
			if(formPatFieldDefEdit.DialogResult==DialogResult.OK) {
				Cache.ClearCaches(InvalidType.PatFields);
				_hasChanged|=formPatFieldDefEdit.HasChanged;
				FillGrid();
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormFieldDefLink formFieldDefLink=new FormFieldDefLink(FieldLocations.Family);
			formFieldDefLink.ShowDialog();
		}

		private void butUp_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx<0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			if(idx==0) { //Can't go up anymore
				return;
			}
			_listPatFieldDefs[idx].ItemOrder--;
			_listPatFieldDefs[idx-1].ItemOrder++;
			PatFieldDefs.Update(_listPatFieldDefs[idx]);
			PatFieldDefs.Update(_listPatFieldDefs[idx-1]);
			DataValid.SetInvalid(InvalidType.PatFields);
			_hasChanged=true;
			FillGrid();
			gridMain.SetSelected(idx-1);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int idx = gridMain.GetSelectedIndex();
			if(idx<0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			if(idx>=_listPatFieldDefs.Count-1) { //Can't go down anymore
				return;
			}
			_listPatFieldDefs[idx].ItemOrder++;
			_listPatFieldDefs[idx+1].ItemOrder--;
			PatFieldDefs.Update(_listPatFieldDefs[idx]);
			PatFieldDefs.Update(_listPatFieldDefs[idx+1]);
			DataValid.SetInvalid(InvalidType.PatFields);
			_hasChanged=true;
			FillGrid();
			gridMain.SetSelected(idx+1);
		}

		private void butAdd_Click(object sender, EventArgs e) {
			PatFieldDef patFieldDef = new PatFieldDef();
			patFieldDef.ItemOrder=_listPatFieldDefs.Count;
			PatFieldDefs.Insert(patFieldDef);//because the patFieldDef will have child objects and we need a valid FK
			using FormPatFieldDefEdit formPatFieldDefEdit = new FormPatFieldDefEdit();
			formPatFieldDefEdit.PatFieldDefCur=patFieldDef;
			formPatFieldDefEdit.IsNew=true;
			formPatFieldDefEdit.ShowDialog();
			if(formPatFieldDefEdit.DialogResult!=DialogResult.OK) {
				PatFieldDefs.Delete(formPatFieldDefEdit.PatFieldDefCur);
				return;
			}
			DataValid.SetInvalid(InvalidType.PatFields);
			_hasChanged=true;
			FillGrid();
		}

		private void FormPatFieldDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(Prefs.UpdateBool(PrefName.DisplayRenamedPatFields,checkDisplayRenamed.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//Fix the item order just in case there was a duplicate.
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				if(_listPatFieldDefs[i].ItemOrder!=i) {
					_listPatFieldDefs[i].ItemOrder=i;
					PatFieldDefs.Update(_listPatFieldDefs[i]);
					Cache.ClearCaches(InvalidType.PatFields);
					_hasChanged=true;
				}
			}
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.PatFields);
			}
		}
	}
}