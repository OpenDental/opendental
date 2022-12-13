using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormSheetDefs:FormODBase {
		//private bool changed;
		//public bool IsSelectionMode;
		//<summary>Only used if IsSelectionMode.  On OK, contains selected siteNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		//public int SelectedSiteNum;
		private List<SheetDef> _listSheetDefsInternal;
		private bool _isChanged;
		private List<SheetDef> _listSheetDefs;

		///<summary>The SheetTypeEnum filter when the form is loaded for both grids.
		///When the list is empty, logical equates to 'All'.</summary>
		private List<SheetTypeEnum> _listSheetTypeEnumsFilter=new List<SheetTypeEnum>();

		public FormSheetDefs() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Set sheetTypeFilter to filter grids to specific set of SheetTypeEnums.</summary>
		public FormSheetDefs(params SheetTypeEnum[] sheetTypeEnums):this() {
			_listSheetTypeEnumsFilter.AddRange(sheetTypeEnums);
		}

		private void FormSheetDefs_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)){
				butNew.Enabled=false;
				butCopy.Enabled=false;
				butCopy2.Enabled=false;
				grid2.Enabled=false;
			}
			_listSheetDefsInternal=SheetsInternal.GetAllInternal();
			_listSheetDefsInternal=_listSheetDefsInternal.OrderBy(x => x.SheetType.ToString()).ToList();
			List<SheetTypeEnum> listSheetTypeEnums=_listSheetDefsInternal.Select(x => x.SheetType).Distinct().ToList();
			listFilter.Items.AddList(listSheetTypeEnums, x => x.ToString());
			for(int i=0; i<listFilter.Items.Count; i++) {
				if(_listSheetTypeEnumsFilter.Contains((SheetTypeEnum)listFilter.Items.GetObjectAt(i))) {
					listFilter.SetSelected(i);
				}
			}
			FillGrid1();
			FillGrid2();
		}

		private void FillGrid1(){
			grid1.BeginUpdate();
			grid1.Columns.Clear();
			grid1.Columns.Add(new GridColumn(Lan.g("TableSheetDef","Description"),100){ IsWidthDynamic=true });
			grid1.Columns.Add(new GridColumn(Lan.g("TableSheetDef","Type"),100));
			grid1.ListGridRows.Clear();
			for(int i=0;i<_listSheetDefsInternal.Count;i++){
				if(listFilter.SelectedIndices.Count>0 && !listFilter.GetListSelected<SheetTypeEnum>().Contains(_listSheetDefsInternal[i].SheetType)) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listSheetDefsInternal[i].Description);
				row.Cells.Add(_listSheetDefsInternal[i].SheetType.ToString());
				row.Tag=_listSheetDefsInternal[i];
				grid1.ListGridRows.Add(row);
			}
			grid1.EndUpdate();
		}

		///<summary>Fills the Custom sheetDef grid. Set selectedSheetDefNum to also select a row.</summary>
		private void FillGrid2(long selectedSheetDefNum=-1){
			SheetDefs.RefreshCache();
			SheetFieldDefs.RefreshCache();
			_listSheetDefs=SheetDefs.GetDeepCopy().FindAll(x => !SheetDefs.IsDashboardType(x));
			grid2.BeginUpdate();
			grid2.Columns.Clear();
			grid2.Columns.Add(new GridColumn(Lan.g("TableSheetDef","Description"),100){ IsWidthDynamic=true });
			grid2.Columns.Add(new GridColumn(Lan.g("TableSheetDef","Type"),100));
			grid2.Columns.Add(new GridColumn(Lan.g("TableSheetDef","Use Mobile\r\nLayout"),65,HorizontalAlignment.Center));
			grid2.ListGridRows.Clear();
			int selectedIndex=-1;
			for(int i=0;i<_listSheetDefs.Count;i++){
				if(listFilter.SelectedIndices.Count>0 && !listFilter.GetListSelected<SheetTypeEnum>().Contains(_listSheetDefs[i].SheetType)) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listSheetDefs[i].Description);
				row.Cells.Add(_listSheetDefs[i].SheetType.ToString());
				row.Cells.Add(_listSheetDefs[i].HasMobileLayout?"X":"");
				row.Tag=_listSheetDefs[i];
				if(selectedSheetDefNum==_listSheetDefs[i].SheetDefNum) {
					selectedIndex=grid2.ListGridRows.Count;//Zero based index.
				}
				grid2.ListGridRows.Add(row);
			}
			grid2.EndUpdate();
			if(selectedIndex==-1) {
				return;
			}
			try {
				grid2.SetSelected(selectedIndex,true);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		private void butNew_Click(object sender, System.EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			//Not allowed to change sheettype once a sheet is created, so we need to let user pick.
			using FormSheetDef formSheetDef=new FormSheetDef();
			formSheetDef.IsInitial=true;
			formSheetDef.IsReadOnly=false;
			SheetDef sheetDef=new SheetDef();
			sheetDef.FontName="Microsoft Sans Serif";
			sheetDef.FontSize=9;
			sheetDef.Height=1100;
			sheetDef.Width=850;
			formSheetDef.SheetDefCur=sheetDef;
			formSheetDef.ShowDialog();
			if(formSheetDef.DialogResult!=DialogResult.OK){
				return;
			}
			//what about parameters?
			sheetDef.SheetFieldDefs=new List<SheetFieldDef>();
			sheetDef.IsNew=true;
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			Dpi.SetAware();
			formSheetDefEdit.ShowDialog();//It will be saved to db inside this form.
			FillGrid2(sheetDef.SheetDefNum);
			_isChanged=true;
		}
		
		private void butCopy2_Click(object sender, EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			if(grid2.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a sheet from the list above first.");
				return;
			}
			SheetDef sheetDef=grid2.SelectedTag<SheetDef>();
			sheetDef.Description=sheetDef.Description+"2";
			SheetDefs.GetFieldsAndParameters(sheetDef);
			sheetDef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetDef,isOldSheetDuplicate:sheetDef.DateTCreated.Year < 1880);
			FillGrid2(sheetDef.SheetDefNum);
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(grid1.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal sheet from the list above first.");
				return;
			}
			SheetDef sheetDef=grid1.SelectedTag<SheetDef>();
			sheetDef.IsNew=true;
			sheetDef.RevID=1;
			SheetDefs.InsertOrUpdate(sheetDef);
			if(sheetDef.SheetType==SheetTypeEnum.MedicalHistory
				&& (sheetDef.Description=="Medical History New Patient" || sheetDef.Description=="Medical History Update")) 
			{
				MsgBox.Show(this,"This is just a template, it may contain allergies and problems that do not exist in your setup.");
			}
			grid1.SetAll(false);
			FillGrid2(sheetDef.SheetDefNum);
		}

		private void butDefault_Click(object sender,EventArgs e) {
			using FormSheetDefDefaults formSheetDefDefaults=new FormSheetDefDefaults();
			formSheetDefDefaults.ShowDialog();
		}

		private void grid1_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(grid1.SelectedTag<SheetDef>());
			Dpi.SetAware();
			formSheetDefEdit.IsInternal=true;
			formSheetDefEdit.ShowDialog();
		}

		private void grid1_Click(object sender,EventArgs e) {
			if(grid1.GetSelectedIndex()>-1) {
				grid2.SetAll(false);
			}
		}
		
		private void grid2_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDef sheetDef=grid2.SelectedTag<SheetDef>();
			SheetDefs.GetFieldsAndParameters(sheetDef);
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			Dpi.SetAware();
			formSheetDefEdit.ShowDialog();
			FillGrid2(sheetDef.SheetDefNum);
			_isChanged=true;
		}

		private void grid2_Click(object sender,EventArgs e) {
			if(grid2.GetSelectedIndex()>-1) {
				grid1.SetAll(false);
			}
		}
		
		private void listFilter_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid1();
			FillGrid2();
		}

		private void butTools_Click(object sender,EventArgs e) {
			using FormSheetTools formSheetTools=new FormSheetTools();
			formSheetTools.ShowDialog();
			if(formSheetTools.HasSheetsChanged) {
				FillGrid2(formSheetTools.SheetDefNumImported);
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormSheetDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Sheets);
			}
		}




	}
}





















