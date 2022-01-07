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
	/// <summary>
	/// </summary>
	public partial class FormPatFieldDefs:FormODBase {
		private bool _hasChanged;
		private List<PatFieldDef> _listPatFieldDefs;
		private bool _isSelectionMode;
		public PatFieldDef SelectedPatFieldDef;

		///<summary>Stale deep copy of _listPatFieldDefs to use with sync.</summary>
		private List<PatFieldDef> _listPatFieldDefsOld;
		///<summary></summary>
		public FormPatFieldDefs(bool isSelectionModeOnly=false)
		{
			//
			// Required for Windows Form Designer support
			//
			_isSelectionMode=isSelectionModeOnly;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatFieldDefs_Load(object sender, System.EventArgs e) {
			_listPatFieldDefs=PatFieldDefs.GetDeepCopy();
			checkDisplayRenamed.Checked=PrefC.GetBool(PrefName.DisplayRenamedPatFields);
			if(_isSelectionMode) {
				_listPatFieldDefs=_listPatFieldDefs.FindAll(x => !x.IsHidden);
				butAdd.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
			}
			_listPatFieldDefsOld=_listPatFieldDefs.Select(x => x.Copy()).ToList();
			LayoutMenu();
			FillGrid();
		}
		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			gridMain.AllowSortingByColumn=true;
			col=new GridColumn(Lan.g(this,"Field Name"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Field Type"),100);
			gridMain.ListGridColumns.Add(col);
			if(!_isSelectionMode) {
				col=new GridColumn(Lan.g(this,"Hidden"),150,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listPatFieldDefs[i].FieldName);
				row.Cells.Add(_listPatFieldDefs[i].FieldType.GetDescription());
				if(!_isSelectionMode) {
					row.Cells.Add(_listPatFieldDefs[i].IsHidden?"X":"");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isSelectionMode) {
				SelectedPatFieldDef=_listPatFieldDefs[e.Row];
				DialogResult=DialogResult.OK;
				return;
			}
			using FormPatFieldDefEdit FormPFDE=new FormPatFieldDefEdit();
			FormPFDE.FieldDef=_listPatFieldDefs[e.Row];
			FormPFDE.ShowDialog();
			if(FormPFDE.DialogResult==DialogResult.OK) {
				if(FormPFDE.FieldDef==null) {
					_listPatFieldDefs.Remove(_listPatFieldDefs[e.Row]);
				}
				else {
					_listPatFieldDefs[e.Row]=FormPFDE.FieldDef;
				}
				_hasChanged=true;
				FillGrid();
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormFieldDefLink FormFDL=new FormFieldDefLink(FieldLocations.Family);
			FormFDL.ShowDialog();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			Array.Copy(gridMain.SelectedIndices,selected,gridMain.SelectedIndices.Length);
			if(selected[0]==0) {
				return;
			}
			for(int i=0;i<selected.Length;i++) {
				_listPatFieldDefs.Reverse(selected[i]-1,2);
			}
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				_listPatFieldDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]-1,true);
			}
			_hasChanged=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			Array.Copy(gridMain.SelectedIndices,selected,gridMain.SelectedIndices.Length);
			if(selected[selected.Length-1]==_listPatFieldDefs.Count-1) {
				return;
			}
			for(int i=selected.Length-1;i>=0;i--) {//go backwards
				_listPatFieldDefs.Reverse(selected[i],2);
			}
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				_listPatFieldDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]+1,true);
			}
			_hasChanged=true;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//Employers.Cur=new Employer();
			PatFieldDef def=new PatFieldDef();
			def.ItemOrder=_listPatFieldDefs.Count;
			using FormPatFieldDefEdit FormPFDE=new FormPatFieldDefEdit();
			FormPFDE.FieldDef=def;
			FormPFDE.IsNew=true;
			FormPFDE.ShowDialog();
			if(FormPFDE.DialogResult==DialogResult.OK) {
				_hasChanged=true;
				_listPatFieldDefs.Add(FormPFDE.FieldDef);
				FillGrid();
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			if(Prefs.UpdateBool(PrefName.DisplayRenamedPatFields,checkDisplayRenamed.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Close();
		}

		private void FormPatFieldDefs_FormClosing(object sender,FormClosingEventArgs e) {
			//Fix the item order just in case there was a duplicate.
			for(int i=0;i<_listPatFieldDefs.Count;i++) {
				if(_listPatFieldDefs[i].ItemOrder!=i) {
					_hasChanged=true;
				}
				_listPatFieldDefs[i].ItemOrder=i;
			}
			if(_hasChanged) {
				PatFieldDefs.Sync(_listPatFieldDefs,_listPatFieldDefsOld);//Update if anything has changed
				DataValid.SetInvalid(InvalidType.PatFields);
			}
		}
	}
}