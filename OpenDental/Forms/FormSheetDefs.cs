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
	public class FormSheetDefs:FormODBase {
		private OpenDental.UI.Button butNew;
		private OpenDental.UI.Button butClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.GridOD grid2;
		private GridOD grid1;
		private OpenDental.UI.Button butCopy;
		//private bool changed;
		//public bool IsSelectionMode;
		//<summary>Only used if IsSelectionMode.  On OK, contains selected siteNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		//public int SelectedSiteNum;
		private List<SheetDef> internalList;
		private ComboBox comboLabel;
		private bool changed;
		private Label label2;
		private OpenDental.UI.Button butCopy2;
		private UI.Button butTools;
		private UI.Button butDefault;
		List<SheetDef> LabelList;
		private Label label3;
		private List<SheetDef> _listSheetDefs;
		private GroupBox groupBox2;
		private UI.ListBoxOD listFilter;

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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				components?.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetDefs));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboLabel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butDefault = new OpenDental.UI.Button();
			this.butTools = new OpenDental.UI.Button();
			this.butCopy2 = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.grid1 = new OpenDental.UI.GridOD();
			this.grid2 = new OpenDental.UI.GridOD();
			this.butNew = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.listFilter = new OpenDental.UI.ListBoxOD();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboLabel);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(895, 51);
			this.groupBox2.TabIndex = 23;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Label assigned to patient button";
			// 
			// comboLabel
			// 
			this.comboLabel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLabel.FormattingEnabled = true;
			this.comboLabel.Location = new System.Drawing.Point(6, 19);
			this.comboLabel.MaxDropDownItems = 20;
			this.comboLabel.Name = "comboLabel";
			this.comboLabel.Size = new System.Drawing.Size(185, 21);
			this.comboLabel.TabIndex = 1;
			this.comboLabel.DropDown += new System.EventHandler(this.comboLabel_DropDown);
			this.comboLabel.SelectionChangeCommitted += new System.EventHandler(this.comboLabel_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(197, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(505, 28);
			this.label2.TabIndex = 18;
			this.label2.Text = "Most other sheet types are assigned simply by creating custom sheets of the same " +
    "type.\r\nReferral slips are set in the referral edit window of each referral.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 15);
			this.label3.TabIndex = 21;
			this.label3.Text = "Sheet type filter";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDefault
			// 
			this.butDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDefault.Location = new System.Drawing.Point(15, 637);
			this.butDefault.Name = "butDefault";
			this.butDefault.Size = new System.Drawing.Size(75, 24);
			this.butDefault.TabIndex = 19;
			this.butDefault.Text = "Defaults";
			this.butDefault.Click += new System.EventHandler(this.butDefault_Click);
			// 
			// butTools
			// 
			this.butTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTools.Location = new System.Drawing.Point(167, 637);
			this.butTools.Name = "butTools";
			this.butTools.Size = new System.Drawing.Size(75, 24);
			this.butTools.TabIndex = 5;
			this.butTools.Text = "Tools";
			this.butTools.Click += new System.EventHandler(this.butTools_Click);
			// 
			// butCopy2
			// 
			this.butCopy2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy2.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCopy2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy2.Location = new System.Drawing.Point(737, 637);
			this.butCopy2.Name = "butCopy2";
			this.butCopy2.Size = new System.Drawing.Size(89, 24);
			this.butCopy2.TabIndex = 7;
			this.butCopy2.Text = "Duplicate";
			this.butCopy2.Click += new System.EventHandler(this.butCopy2_Click);
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopy.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(484, 637);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 4;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// grid1
			// 
			this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grid1.Location = new System.Drawing.Point(136, 69);
			this.grid1.Name = "grid1";
			this.grid1.Size = new System.Drawing.Size(380, 557);
			this.grid1.TabIndex = 2;
			this.grid1.Title = "Internal";
			this.grid1.TranslationName = "TableInternal";
			this.grid1.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid1_CellDoubleClick);
			this.grid1.Click += new System.EventHandler(this.grid1_Click);
			// 
			// grid2
			// 
			this.grid2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid2.HasMultilineHeaders = true;
			this.grid2.Location = new System.Drawing.Point(528, 69);
			this.grid2.Name = "grid2";
			this.grid2.Size = new System.Drawing.Size(380, 557);
			this.grid2.TabIndex = 3;
			this.grid2.Title = "Custom";
			this.grid2.TranslationName = "TableCustom";
			this.grid2.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid2_CellDoubleClick);
			this.grid2.Click += new System.EventHandler(this.grid2_Click);
			// 
			// butNew
			// 
			this.butNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNew.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(652, 637);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(80, 24);
			this.butNew.TabIndex = 6;
			this.butNew.Text = "New";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(832, 637);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// listFilter
			// 
			this.listFilter.Location = new System.Drawing.Point(4, 88);
			this.listFilter.Name = "listFilter";
			this.listFilter.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listFilter.Size = new System.Drawing.Size(124, 355);
			this.listFilter.TabIndex = 24;
			this.listFilter.SelectedIndexChanged += new System.EventHandler(this.listFilter_SelectedIndexChanged);
			// 
			// FormSheetDefs
			// 
			this.ClientSize = new System.Drawing.Size(919, 671);
			this.Controls.Add(this.listFilter);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butDefault);
			this.Controls.Add(this.butTools);
			this.Controls.Add(this.butCopy2);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.grid1);
			this.Controls.Add(this.grid2);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefs";
			this.Text = "Sheet Defs";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSheetDefs_FormClosing);
			this.Load += new System.EventHandler(this.FormSheetDefs_Load);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

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





















