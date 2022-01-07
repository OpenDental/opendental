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
		private List<SheetDef> internalList;
		private bool changed;
		List<SheetDef> LabelList;
		private List<SheetDef> _listSheetDefs;

		///<summary>The SheetTypeEnum filter when the form is loaded for both grids.
		///When the list is empty, logical equates to 'All'.</summary>
		private List<SheetTypeEnum> _sheetTypeFilter=new List<SheetTypeEnum>();

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
			_sheetTypeFilter.AddRange(sheetTypeEnums);
		}

		private void FormSheetDefs_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)){
				butNew.Enabled=false;
				butCopy.Enabled=false;
				butCopy2.Enabled=false;
				grid2.Enabled=false;
			}
			internalList=SheetsInternal.GetAllInternal();
			internalList=internalList.OrderBy(x => x.SheetType.ToString()).ToList();
			List<SheetTypeEnum> listSheetTypes=internalList.Select(x => x.SheetType).Distinct().ToList();
			listFilter.Items.AddList(listSheetTypes, x => x.ToString());
			for(int i=0; i<listFilter.Items.Count; i++) {
				if(_sheetTypeFilter.Contains((SheetTypeEnum)listFilter.Items.GetObjectAt(i))) {
					listFilter.SetSelected(i);
				}
			}
			FillGrid1();
			FillGrid2();
			comboLabel.Items.Clear();
			comboLabel.Items.Add(Lan.g(this,"Default"));
			comboLabel.SelectedIndex=0;
			LabelList=new List<SheetDef>();
			for(int i=0;i<_listSheetDefs.Count;i++){
				if(_listSheetDefs[i].SheetType==SheetTypeEnum.LabelPatient){
					LabelList.Add(_listSheetDefs[i].Copy());
				}
			}
			for(int i=0;i<LabelList.Count;i++){
				comboLabel.Items.Add(LabelList[i].Description);
				if(PrefC.GetLong(PrefName.LabelPatientDefaultSheetDefNum)==LabelList[i].SheetDefNum){
					comboLabel.SelectedIndex=i+1;
				}
			}
		}

		private void FillGrid1(){
			grid1.BeginUpdate();
			grid1.ListGridColumns.Clear();
			grid1.ListGridColumns.Add(new GridColumn(Lan.g("TableSheetDef","Description"),100){ IsWidthDynamic=true });
			grid1.ListGridColumns.Add(new GridColumn(Lan.g("TableSheetDef","Type"),100));
			grid1.ListGridRows.Clear();
			foreach(SheetDef internalDef in internalList){
				if(listFilter.SelectedIndices.Count>0 && !listFilter.GetListSelected<SheetTypeEnum>().Contains(internalDef.SheetType)) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(internalDef.Description);
				row.Cells.Add(internalDef.SheetType.ToString());
				row.Tag=internalDef;
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
			grid2.ListGridColumns.Clear();
			grid2.ListGridColumns.Add(new GridColumn(Lan.g("TableSheetDef","Description"),100){ IsWidthDynamic=true });
			grid2.ListGridColumns.Add(new GridColumn(Lan.g("TableSheetDef","Type"),100));
			grid2.ListGridColumns.Add(new GridColumn(Lan.g("TableSheetDef","Use Mobile\r\nLayout"),65,HorizontalAlignment.Center));
			grid2.ListGridRows.Clear();
			int selectedIndex=-1;
			foreach(SheetDef sheetDef in _listSheetDefs){
				if(listFilter.SelectedIndices.Count>0 && !listFilter.GetListSelected<SheetTypeEnum>().Contains(sheetDef.SheetType)) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(sheetDef.Description);
				row.Cells.Add(sheetDef.SheetType.ToString());
				row.Cells.Add(sheetDef.HasMobileLayout?"X":"");
				row.Tag=sheetDef;
				if(selectedSheetDefNum==sheetDef.SheetDefNum) {
					selectedIndex=grid2.ListGridRows.Count;//Zero based index.
				}
				grid2.ListGridRows.Add(row);
			}
			grid2.EndUpdate();
			if(selectedIndex!=-1) {
				try {
					grid2.SetSelected(selectedIndex,true);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}

		private void butNew_Click(object sender, System.EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			//Not allowed to change sheettype once a sheet is created, so we need to let user pick.
			using FormSheetDef FormS=new FormSheetDef();
			FormS.IsInitial=true;
			FormS.IsReadOnly=false;
			SheetDef sheetdef=new SheetDef();
			sheetdef.FontName="Microsoft Sans Serif";
			sheetdef.FontSize=9;
			sheetdef.Height=1100;
			sheetdef.Width=850;
			FormS.SheetDefCur=sheetdef;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			//what about parameters?
			sheetdef.SheetFieldDefs=new List<SheetFieldDef>();
			sheetdef.IsNew=true;
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit FormSD=new FormSheetDefEdit(sheetdef);
			Dpi.SetAware();
			FormSD.ShowDialog();//It will be saved to db inside this form.
			FillGrid2(sheetdef.SheetDefNum);
			changed=true;
		}
		
		private void butCopy2_Click(object sender, EventArgs e) {
			//This button is not enabled unless user has appropriate permission for setup.
			if(grid2.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a sheet from the list above first.");
				return;
			}
			SheetDef sheetdef=grid2.SelectedTag<SheetDef>();
			sheetdef.Description=sheetdef.Description+"2";
			SheetDefs.GetFieldsAndParameters(sheetdef);
			sheetdef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetdef,isOldSheetDuplicate:sheetdef.DateTCreated.Year < 1880);
			FillGrid2(sheetdef.SheetDefNum);
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(grid1.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal sheet from the list above first.");
				return;
			}
			SheetDef sheetdef=grid1.SelectedTag<SheetDef>();
			sheetdef.IsNew=true;
			sheetdef.RevID=1;
			SheetDefs.InsertOrUpdate(sheetdef);
			if(sheetdef.SheetType==SheetTypeEnum.MedicalHistory
				&& (sheetdef.Description=="Medical History New Patient" || sheetdef.Description=="Medical History Update")) 
			{
				MsgBox.Show(this,"This is just a template, it may contain allergies and problems that do not exist in your setup.");
			}
			grid1.SetAll(false);
			FillGrid2(sheetdef.SheetDefNum);
		}

		private void butDefault_Click(object sender,EventArgs e) {
			using FormSheetDefDefaults FormSDD=new FormSheetDefDefaults();
			FormSDD.ShowDialog();
		}

		private void grid1_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit FormS=new FormSheetDefEdit(grid1.SelectedTag<SheetDef>());
			Dpi.SetAware();
			FormS.IsInternal=true;
			FormS.ShowDialog();
		}

		private void grid1_Click(object sender,EventArgs e) {
			if(grid1.GetSelectedIndex()>-1) {
				grid2.SetAll(false);
			}
		}
		
		private void grid2_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDef sheetdef=grid2.SelectedTag<SheetDef>();
			SheetDefs.GetFieldsAndParameters(sheetdef);
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit FormS=new FormSheetDefEdit(sheetdef);
			Dpi.SetAware();
			FormS.ShowDialog();
			FillGrid2(sheetdef.SheetDefNum);
			changed=true;
		}

		private void grid2_Click(object sender,EventArgs e) {
			if(grid2.GetSelectedIndex()>-1) {
				grid1.SetAll(false);
			}
		}

		private void comboLabel_DropDown(object sender,EventArgs e) {
			comboLabel.Items.Clear();
			comboLabel.Items.Add(Lan.g(this,"Default"));
			comboLabel.SelectedIndex=0;
			LabelList=new List<SheetDef>();
			for(int i=0;i<_listSheetDefs.Count;i++){
				if(_listSheetDefs[i].SheetType==SheetTypeEnum.LabelPatient){
					LabelList.Add(_listSheetDefs[i].Copy());
				}
			}
			for(int i=0;i<LabelList.Count;i++){
				comboLabel.Items.Add(LabelList[i].Description);
				if(PrefC.GetLong(PrefName.LabelPatientDefaultSheetDefNum)==LabelList[i].SheetDefNum){
					comboLabel.SelectedIndex=i+1;
				}
			}
		}

		private void comboLabel_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboLabel.SelectedIndex==0){
				Prefs.UpdateLong(PrefName.LabelPatientDefaultSheetDefNum,0);
			}
			else{
				Prefs.UpdateLong(PrefName.LabelPatientDefaultSheetDefNum,LabelList[comboLabel.SelectedIndex-1].SheetDefNum);
			}
			DataValid.SetInvalid(InvalidType.Prefs);
		}
		
		private void listFilter_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid1();
			FillGrid2();
		}

		private void butTools_Click(object sender,EventArgs e) {
			using FormSheetTools formST=new FormSheetTools();
			formST.ShowDialog();
			if(formST.HasSheetsChanged) {
				FillGrid2(formST.ImportedSheetDefNum);
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormSheetDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.Sheets);
			}
		}
	}
}





















