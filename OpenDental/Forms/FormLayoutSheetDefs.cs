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
	///<summary>Form currently is not used, but we might bring it back some day.</summary>
	public partial class FormLayoutSheetDefs:FormODBase {

		///<summary>True when a sheet could have been added,edited or deleted.
		///When true InvalidType.Sheets is set invalid.</summary>
		private bool _isSignalNeeded;
		///<summary>List of sheetDefs to show in gridOtherLayouts.</summary>
		private List<SheetDef> _listOtherSheetDefs;
		///<summary>List of sheetDefs to show in gridCustomLayouts.</summary>
		private List<SheetDef> _listCustomSheetDefs;

		public FormLayoutSheetDefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLayoutSheetDefs_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)) {
				butNew.Enabled=false;
				butCopy.Enabled=false;
				butDuplicate.Enabled=false;
				gridCustomLayouts.Enabled=false;
			}
			RefreshAndFillGrids();
		}

		private void RefreshAndFillGrids(long selectedSheetNum=-1) {
			#region Refresh both sheetDef lists
			SheetDefs.RefreshCache();
			SheetFieldDefs.RefreshCache();
			_listOtherSheetDefs=new List<SheetDef>() { SheetsInternal.GetSheetDef(SheetInternalType.ChartModule) };//Must always be first in list.
			List<SheetDef> listCustomSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ChartModule);
			//_listOtherSheetDefs.AddRange(listCustomSheetDefs.FindAll(x => x.UserNum!=Security.CurUser.UserNum));//All other users.
			//_listCustomSheetDefs=listCustomSheetDefs.FindAll(x => x.UserNum==Security.CurUser.UserNum);
			#endregion
			FillGrid(gridOtherLayouts,_listOtherSheetDefs,selectedSheetNum);
			FillGrid(gridCustomLayouts,_listCustomSheetDefs,selectedSheetNum);
		}

		///<summary>Fills either grid since both grids contain the same data type and thus the same columns.</summary>
		private void FillGrid(GridOD grid,List<SheetDef> listSheetDefs,long selectedSheetNum=-1) {
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn("Description",120){ IsWidthDynamic=true });
			grid.ListGridColumns.Add(new GridColumn("User",120));
			grid.ListGridRows.Clear();
			int selectedRowIndex=-1;
			foreach(SheetDef sheetDef in listSheetDefs) {
				if(sheetDef.SheetDefNum==selectedSheetNum) {
					selectedRowIndex=grid.ListGridRows.Count;
				}
				//grid.Rows.Add(new ODGridRow(sheetDef.Description,Userods.GetName(sheetDef.UserNum)));
			}
			grid.EndUpdate();
			if(selectedRowIndex!=-1) {
				grid.SetSelected(selectedRowIndex,true);
			}
		}

		private void gridOtherLayouts_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDef sheetDef=_listOtherSheetDefs[e.Row];
			if(sheetDef.SheetDefNum!=0) {//Is not the internal sheet.
				SheetDefs.GetFieldsAndParameters(sheetDef);//Other user custom sheetdefs need their fields filled.
			}
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			Dpi.SetAware();
			formSheetDefEdit.IsInternal=true;//So the current user cannot edit, including custom sheets for other users.
			formSheetDefEdit.ShowDialog();
		}

		private void gridOtherLayouts_Click(object sender,EventArgs e) {
			gridCustomLayouts.SetAll(false);
		}

		private void gridCustomLayouts_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDef sheetDef=_listCustomSheetDefs[e.Row];
			SheetDefs.GetFieldsAndParameters(sheetDef);
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			Dpi.SetAware();
			if(formSheetDefEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_isSignalNeeded=true;
			RefreshAndFillGrids(sheetDef.SheetDefNum);
		}

		private void gridCustomLayouts_Click(object sender,EventArgs e) {
			gridOtherLayouts.SetAll(false);
		}

		private void butCopy_Click(object sender,EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			if(gridOtherLayouts.SelectedIndices.Length==0) {
				MsgBox.Show("Please select a sheet from Internal and Other User Layouts before copying.");
				return;
			}
			SheetDef sheetDef=_listOtherSheetDefs[gridOtherLayouts.SelectedIndices[0]].Copy();
			if(sheetDef.SheetDefNum!=0) {//Is not the internal sheet.
				SheetDefs.GetFieldsAndParameters(sheetDef);//Other user custom sheetdefs need their fields filled.
			}
			//sheetDef.UserNum=Security.CurUser.UserNum;
			sheetDef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetDef);
			_isSignalNeeded=true;
			RefreshAndFillGrids(sheetDef.SheetDefNum);
		}

		private void butNew_Click(object sender,EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			SheetDef sheetDefNew=_listOtherSheetDefs[0].Copy();//First item is always internal.  Only 1 interal sheet def per instance of this window.
			sheetDefNew.Description="Custom Layout "+(_listCustomSheetDefs.Count+1).ToString();
			sheetDefNew.IsNew=true;
			//sheetDefNew.UserNum=Security.CurUser.UserNum;
			using FormSheetDef formSheetDef=new FormSheetDef();
			formSheetDef.IsInitial=false;//User can not change SheetType.
			formSheetDef.IsReadOnly=false;
			formSheetDef.SheetDefCur=sheetDefNew;
			if(formSheetDef.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDefNew);
			Dpi.SetAware();
			formSheetDefEdit.ShowDialog();//It will be saved to db inside this form.
			_isSignalNeeded=true;
			RefreshAndFillGrids(sheetDefNew.SheetDefNum);
		}

		private void butDuplicate_Click(object sender,EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			if(gridCustomLayouts.SelectedIndices.Length==0) {
				MsgBox.Show("Please select a sheet from My Custom Layouts before duplicating.");
				return;
			}
			SheetDef sheetDef=_listCustomSheetDefs[gridCustomLayouts.SelectedIndices[0]].Copy();
			//sheetDef.UserNum=Security.CurUser.UserNum;
			sheetDef.Description=sheetDef.Description+" 2";
			SheetDefs.GetFieldsAndParameters(sheetDef);
			sheetDef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetDef);
			RefreshAndFillGrids(sheetDef.SheetDefNum);
		}

		private void butClose_Click(object sender,EventArgs e) {
			
		}

		private void FormModuleSheetDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isSignalNeeded) {
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
			if(_isSignalNeeded) {
				DataValid.SetInvalid(InvalidType.Sheets);
			}
		}

	}
}