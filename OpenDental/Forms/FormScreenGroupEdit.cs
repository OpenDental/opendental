using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormScreenGroupEdit:FormODBase {
		private IContainer components=null;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label labelProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.ComboBox comboPlaceService;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.ComboBox comboCounty;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelScreener;
		private System.Windows.Forms.TextBox textScreenDate;
		private System.Windows.Forms.TextBox textProvName;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.ComboBox comboGradeSchool;
		private ScreenGroup _screenGroup;
		private UI.Button butCancel;
		private UI.Button butDelete;
		private UI.GridOD gridMain;
		private List<OpenDentBusiness.Screen> _listScreens;
		private GridOD gridScreenPats;
		private List<ScreenPat> _listScreenPats;
		///<summary>Stale deep copy of _listScreenPats to use with sync.</summary>
		private List<ScreenPat> _listScreenPatsOld;
		private UI.Button button1;
		private UI.Button butRemovePat;
		private UI.Button butStartScreens;
		private Label label4;
		private ComboBox comboSheetDefs;
		private Label labelSheet;
		private List<Provider> _listProvs;
		private ContextMenu patContextMenu;
		private MenuItem menuItemUnknown;
		private MenuItem menuItemAllowed;
		private MenuItem menuItemNoPermission;
		private MenuItem menuItemRefused;
		private MenuItem menuItemAbsent;
		private MenuItem menuItemBehavior;
		private MenuItem menuItemOther;
		private List<SheetDef> _listScreeningSheetDefs;

		///<summary></summary>
		public FormScreenGroupEdit(ScreenGroup screenGroup) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			gridScreenPats.ContextMenu=patContextMenu;
			Lan.F(this);
			_screenGroup=screenGroup;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScreenGroupEdit));
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.labelProv = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textScreenDate = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.comboPlaceService = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboCounty = new System.Windows.Forms.ComboBox();
			this.comboGradeSchool = new System.Windows.Forms.ComboBox();
			this.textProvName = new System.Windows.Forms.TextBox();
			this.labelScreener = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.gridScreenPats = new OpenDental.UI.GridOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butStartScreens = new OpenDental.UI.Button();
			this.butRemovePat = new OpenDental.UI.Button();
			this.button1 = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboSheetDefs = new System.Windows.Forms.ComboBox();
			this.labelSheet = new System.Windows.Forms.Label();
			this.patContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemUnknown = new System.Windows.Forms.MenuItem();
			this.menuItemAllowed = new System.Windows.Forms.MenuItem();
			this.menuItemNoPermission = new System.Windows.Forms.MenuItem();
			this.menuItemRefused = new System.Windows.Forms.MenuItem();
			this.menuItemAbsent = new System.Windows.Forms.MenuItem();
			this.menuItemBehavior = new System.Windows.Forms.MenuItem();
			this.menuItemOther = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(10, 113);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(93, 16);
			this.label14.TabIndex = 12;
			this.label14.Text = "School";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(1, 92);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(101, 15);
			this.label13.TabIndex = 11;
			this.label13.Text = "County";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelProv
			// 
			this.labelProv.Location = new System.Drawing.Point(2, 71);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(101, 16);
			this.labelProv.TabIndex = 50;
			this.labelProv.Text = "or Prov";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 17);
			this.label1.TabIndex = 51;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textScreenDate
			// 
			this.textScreenDate.Location = new System.Drawing.Point(102, 9);
			this.textScreenDate.Name = "textScreenDate";
			this.textScreenDate.Size = new System.Drawing.Size(64, 20);
			this.textScreenDate.TabIndex = 6;
			this.textScreenDate.Validating += new System.ComponentModel.CancelEventHandler(this.textScreenDate_Validating);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(102, 29);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(173, 20);
			this.textDescription.TabIndex = 0;
			// 
			// comboProv
			// 
			this.comboProv.BackColor = System.Drawing.SystemColors.Window;
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(102, 69);
			this.comboProv.MaxDropDownItems = 25;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(173, 21);
			this.comboProv.TabIndex = 2;
			this.comboProv.SelectedIndexChanged += new System.EventHandler(this.comboProv_SelectedIndexChanged);
			this.comboProv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboProv_KeyDown);
			// 
			// comboPlaceService
			// 
			this.comboPlaceService.BackColor = System.Drawing.SystemColors.Window;
			this.comboPlaceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlaceService.Location = new System.Drawing.Point(102, 132);
			this.comboPlaceService.MaxDropDownItems = 25;
			this.comboPlaceService.Name = "comboPlaceService";
			this.comboPlaceService.Size = new System.Drawing.Size(173, 21);
			this.comboPlaceService.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 133);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 17);
			this.label2.TabIndex = 119;
			this.label2.Text = "Location";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 128;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCounty
			// 
			this.comboCounty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCounty.Location = new System.Drawing.Point(102, 90);
			this.comboCounty.Name = "comboCounty";
			this.comboCounty.Size = new System.Drawing.Size(173, 21);
			this.comboCounty.TabIndex = 3;
			this.comboCounty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboCounty_KeyDown);
			// 
			// comboGradeSchool
			// 
			this.comboGradeSchool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGradeSchool.Location = new System.Drawing.Point(102, 111);
			this.comboGradeSchool.Name = "comboGradeSchool";
			this.comboGradeSchool.Size = new System.Drawing.Size(173, 21);
			this.comboGradeSchool.TabIndex = 4;
			this.comboGradeSchool.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboGradeSchool_KeyDown);
			// 
			// textProvName
			// 
			this.textProvName.Location = new System.Drawing.Point(102, 49);
			this.textProvName.Name = "textProvName";
			this.textProvName.Size = new System.Drawing.Size(173, 20);
			this.textProvName.TabIndex = 1;
			this.textProvName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textProvName_KeyUp);
			// 
			// labelScreener
			// 
			this.labelScreener.Location = new System.Drawing.Point(3, 51);
			this.labelScreener.Name = "labelScreener";
			this.labelScreener.Size = new System.Drawing.Size(99, 16);
			this.labelScreener.TabIndex = 142;
			this.labelScreener.Text = "Screener";
			this.labelScreener.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(472, 4);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(390, 17);
			this.label4.TabIndex = 152;
			this.label4.Text = "Right click patient to set screening permission.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridScreenPats
			// 
			this.gridScreenPats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridScreenPats.Location = new System.Drawing.Point(472, 21);
			this.gridScreenPats.Name = "gridScreenPats";
			this.gridScreenPats.Size = new System.Drawing.Size(390, 144);
			this.gridScreenPats.TabIndex = 148;
			this.gridScreenPats.Title = "Patients for Screening";
			this.gridScreenPats.TranslationName = "TableScreeningGroupPatients";
			this.gridScreenPats.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridScreenPats_CellDoubleClick);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(2, 165);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(860, 438);
			this.gridMain.TabIndex = 147;
			this.gridMain.Title = "Screenings";
			this.gridMain.TranslationName = "TableScreenings";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butStartScreens
			// 
			this.butStartScreens.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butStartScreens.Location = new System.Drawing.Point(375, 106);
			this.butStartScreens.Name = "butStartScreens";
			this.butStartScreens.Size = new System.Drawing.Size(92, 23);
			this.butStartScreens.TabIndex = 9;
			this.butStartScreens.Text = "Screen Patients";
			this.butStartScreens.UseVisualStyleBackColor = true;
			this.butStartScreens.Click += new System.EventHandler(this.butStartScreens_Click);
			// 
			// butRemovePat
			// 
			this.butRemovePat.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemovePat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemovePat.Location = new System.Drawing.Point(392, 52);
			this.butRemovePat.Name = "butRemovePat";
			this.butRemovePat.Size = new System.Drawing.Size(75, 23);
			this.butRemovePat.TabIndex = 8;
			this.butRemovePat.Text = "Remove";
			this.butRemovePat.UseVisualStyleBackColor = true;
			this.butRemovePat.Click += new System.EventHandler(this.butRemovePat_Click);
			// 
			// button1
			// 
			this.button1.Icon = OpenDental.UI.EnumIcons.Add;
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(392, 23);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Add";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.butAddPat_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(173, 613);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(114, 24);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "Add Anonymous";
			this.butAdd.Click += new System.EventHandler(this.butAddAnonymous_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 613);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(70, 24);
			this.butDelete.TabIndex = 13;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCancel.Location = new System.Drawing.Point(782, 613);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(70, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butOK.Location = new System.Drawing.Point(706, 613);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(70, 24);
			this.butOK.TabIndex = 11;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboSheetDefs
			// 
			this.comboSheetDefs.BackColor = System.Drawing.SystemColors.Window;
			this.comboSheetDefs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSheetDefs.Location = new System.Drawing.Point(375, 138);
			this.comboSheetDefs.MaxDropDownItems = 25;
			this.comboSheetDefs.Name = "comboSheetDefs";
			this.comboSheetDefs.Size = new System.Drawing.Size(92, 21);
			this.comboSheetDefs.TabIndex = 153;
			// 
			// labelSheet
			// 
			this.labelSheet.Location = new System.Drawing.Point(286, 141);
			this.labelSheet.Name = "labelSheet";
			this.labelSheet.Size = new System.Drawing.Size(88, 17);
			this.labelSheet.TabIndex = 154;
			this.labelSheet.Text = "Sheet";
			this.labelSheet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// patContextMenu
			// 
			this.patContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemUnknown,
            this.menuItemAllowed,
            this.menuItemNoPermission,
            this.menuItemRefused,
            this.menuItemAbsent,
            this.menuItemBehavior,
            this.menuItemOther});
			// 
			// menuItemUnknown
			// 
			this.menuItemUnknown.Index = 0;
			this.menuItemUnknown.Text = "Unknown";
			this.menuItemUnknown.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemAllowed
			// 
			this.menuItemAllowed.Index = 1;
			this.menuItemAllowed.Text = "Allowed";
			this.menuItemAllowed.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemNoPermission
			// 
			this.menuItemNoPermission.Index = 2;
			this.menuItemNoPermission.Text = "NoPermission";
			this.menuItemNoPermission.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemRefused
			// 
			this.menuItemRefused.Index = 3;
			this.menuItemRefused.Text = "Refused";
			this.menuItemRefused.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemAbsent
			// 
			this.menuItemAbsent.Index = 4;
			this.menuItemAbsent.Text = "Absent";
			this.menuItemAbsent.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemBehavior
			// 
			this.menuItemBehavior.Index = 5;
			this.menuItemBehavior.Text = "Behavior";
			this.menuItemBehavior.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// menuItemOther
			// 
			this.menuItemOther.Index = 6;
			this.menuItemOther.Text = "Other";
			this.menuItemOther.Click += new System.EventHandler(this.patContextMenuItem_Click);
			// 
			// FormScreenGroupEdit
			// 
			this.ClientSize = new System.Drawing.Size(864, 641);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.comboSheetDefs);
			this.Controls.Add(this.labelSheet);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butStartScreens);
			this.Controls.Add(this.butRemovePat);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.gridScreenPats);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.textProvName);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.labelProv);
			this.Controls.Add(this.labelScreener);
			this.Controls.Add(this.comboGradeSchool);
			this.Controls.Add(this.comboCounty);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.textScreenDate);
			this.Controls.Add(this.comboPlaceService);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormScreenGroupEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Screening Group";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormScreenGroupEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormScreenGroup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormScreenGroup_Load(object sender, System.EventArgs e) {
			if(_screenGroup.IsNew) {
				ScreenGroups.Insert(_screenGroup);
			}
			_listScreenPats=ScreenPats.GetForScreenGroup(_screenGroup.ScreenGroupNum);
			_listScreenPatsOld=_listScreenPats.Select(x => x.Clone()).ToList();
			FillGrid();
			FillScreenPats();
			textScreenDate.Text=_screenGroup.SGDate.ToShortDateString();
			textDescription.Text=_screenGroup.Description;
			textProvName.Text=_screenGroup.ProvName;//has to be filled before provnum
			_listProvs=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProvs.Count;i++) {
				comboProv.Items.Add(_listProvs[i].Abbr);
				if(_screenGroup.ProvNum==_listProvs[i].ProvNum) {
					comboProv.SelectedIndex=i;
				}
			}
			string[] CountiesListNames=Counties.GetListNames();
			comboCounty.Items.AddRange(CountiesListNames);
			if(_screenGroup.County==null) {
				_screenGroup.County="";//prevents the next line from crashing
			}
			comboCounty.SelectedIndex=comboCounty.Items.IndexOf(_screenGroup.County);//"" etc OK
			foreach(Site site in Sites.GetDeepCopy()) {
				comboGradeSchool.Items.Add(site.Description);
			}
			if(_screenGroup.GradeSchool==null) {
				_screenGroup.GradeSchool="";//prevents the next line from crashing
			}
			comboGradeSchool.SelectedIndex=comboGradeSchool.Items.IndexOf(_screenGroup.GradeSchool);//"" etc OK
			comboPlaceService.Items.AddRange(Enum.GetNames(typeof(PlaceOfService)));
			comboPlaceService.SelectedIndex=(int)_screenGroup.PlaceService;
			_listScreeningSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Screening);
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				comboSheetDefs.Items.Add(Lan.g(this,"Default"));
				foreach(SheetDef def in _listScreeningSheetDefs) {
					comboSheetDefs.Items.Add(def.Description);
				}
				if(_screenGroup.SheetDefNum==0) {
					comboSheetDefs.SelectedIndex=0;
				}
				else {
					int idx=_listScreeningSheetDefs.FindIndex(x => x.SheetDefNum==_screenGroup.SheetDefNum);
					if(idx==-1) {//Sheet def deleted
						comboSheetDefs.SelectedIndex=0;//Default
					}
					else {
						comboSheetDefs.SelectedIndex=idx+1;
					}
				}
			}
			else {
				labelSheet.Visible=false;
				comboSheetDefs.Visible=false;
			}
		}

		private void FillGrid() {
			_listScreens=Screens.GetScreensForGroup(_screenGroup.ScreenGroupNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"#"),30);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Name"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Grade"),55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Age"),40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Race"),105);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Sex"),45);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Urgency"),70);
			gridMain.ListGridColumns.Add(col);
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g(this,"Caries"),45,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"ECC"),30,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"CarExp"),50,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"ExSeal"),45,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"NeedSeal"),60,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"NoTeeth"),55,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Comments"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(OpenDentBusiness.Screen screen in _listScreens) {
				row=new GridRow();
				row.Cells.Add(screen.ScreenGroupOrder.ToString());
				ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.ScreenPatNum==screen.ScreenPatNum);
				row.Cells.Add((screenPat==null)?"Anonymous":Patients.GetLim(screenPat.PatNum).GetNameLF());
				row.Cells.Add(screen.GradeLevel.ToString());
				row.Cells.Add(screen.Age.ToString());
				row.Cells.Add(screen.RaceOld.ToString());
				row.Cells.Add(screen.Gender.ToString());
				row.Cells.Add(screen.Urgency.ToString());
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(screen.HasCaries==YN.Yes ? "X":"");
					row.Cells.Add(screen.EarlyChildCaries==YN.Yes ? "X":"");
					row.Cells.Add(screen.CariesExperience==YN.Yes ? "X":"");
					row.Cells.Add(screen.ExistingSealants==YN.Yes ? "X":"");
					row.Cells.Add(screen.NeedsSealants==YN.Yes ? "X":"");
					row.Cells.Add(screen.MissingAllTeeth==YN.Yes ? "X":"");
				}
				row.Cells.Add(screen.Comments);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.Title=Lan.g(this,"Screenings")+" - "+_listScreens.Count;
			gridMain.EndUpdate();
		}

		private void FillScreenPats() {
			gridScreenPats.BeginUpdate();
			gridScreenPats.Title=Lan.g(this,"Patients for Screening")+" - "+_listScreenPats.Count;
			gridScreenPats.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),200);
			gridScreenPats.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Permission"),100);
			gridScreenPats.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Screened"),90,HorizontalAlignment.Center);
			gridScreenPats.ListGridColumns.Add(col);
			gridScreenPats.ListGridRows.Clear();
			GridRow row;
			foreach(ScreenPat screenPat in _listScreenPats) {
				row=new GridRow();
				Patient pat=Patients.GetLim(screenPat.PatNum);
				row.Cells.Add(pat.GetNameLF());
				row.Cells.Add(screenPat.PatScreenPerm.ToString());
				OpenDentBusiness.Screen screen=_listScreens.FirstOrDefault(x => x.ScreenPatNum==screenPat.ScreenPatNum);
				row.Cells.Add((screen==null)?"":"X");
				gridScreenPats.ListGridRows.Add(row);
			}
			gridScreenPats.EndUpdate();
		}

		///<summary></summary>
		private void AddAnonymousScreens() {
			while(true) {
				using FormScreenEdit FormSE=new FormScreenEdit();
				FormSE.ScreenGroupCur=_screenGroup;
				FormSE.IsNew=true;
				if(_listScreens.Count==0) {
					FormSE.ScreenCur=new OpenDentBusiness.Screen();
					FormSE.ScreenCur.ScreenGroupOrder=1;
				}
				else {
					FormSE.ScreenCur=_listScreens[_listScreens.Count-1];//'remembers' the last entry
					FormSE.ScreenCur.ScreenGroupOrder=FormSE.ScreenCur.ScreenGroupOrder+1;//increments for next
				}
				//For Anonymous patients always default to unknowns.
				FormSE.ScreenCur.Gender=PatientGender.Unknown;
				FormSE.ScreenCur.RaceOld=PatientRaceOld.Unknown;
				FormSE.ShowDialog();
				if(FormSE.DialogResult!=DialogResult.OK) {
					return;
				}
				FormSE.ScreenCur.ScreenGroupOrder++;
				FillGrid();
			}
		}

		///<summary></summary>
		private void AddAnonymousScreensForSheets() {
			//Get the first custom Screening sheet or use the internal one
			SheetDef sheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.Screening);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			sheet.IsNew=true;
			SheetParameter.SetParameter(sheet,"ScreenGroupNum",_screenGroup.ScreenGroupNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			//Create a valid screen so that we can create a screening sheet with the corresponding ScreenNum.
			OpenDentBusiness.Screen screen=new OpenDentBusiness.Screen();
			screen.ScreenGroupNum=_screenGroup.ScreenGroupNum;
			screen.ScreenGroupOrder=1;
			if(_listScreens.Count!=0) {
				screen.ScreenGroupOrder=_listScreens.Last().ScreenGroupOrder+1;//increments for next
			}
			while(true) {
				//Trick the sheet into thinking it is a "new" screen but keep all the other values from the previous screening.
				screen.IsNew=true;
				screen.ScreenNum=0;
				//For Anonymous patients always default to unknowns.
				foreach(SheetField field in sheet.SheetFields.FindAll(x => x.FieldType==SheetFieldType.ComboBox)) {
					int startIndex=field.FieldValue.IndexOf(';');
					if(startIndex < 0) {
						continue;//Not a valid value for sheet field type of combo box, do nothing.
					}
					switch(field.FieldName) {
						case "Race/Ethnicity":
						case "Gender":
							field.FieldValue="Unknown"+field.FieldValue.Substring(startIndex);
							break;
					}
				}
				using FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheet);
				FormSFE.ShowDialog();
				if(FormSFE.DialogResult!=DialogResult.OK) {
					return;
				}
				Screens.CreateScreenFromSheet(sheet,screen);
				screen.ScreenGroupOrder++;
				FillGrid();
			}
		}

		private void StartScreensForPats() {
			if(_listScreenPats.Count==0) {
				MsgBox.Show(this,"No patients for screening.");
				return;
			}
			using FormScreenEdit FormSE=new FormScreenEdit();
			FormSE.ScreenGroupCur=_screenGroup;
			FormSE.IsNew=true;
			int selectedIdx=gridScreenPats.GetSelectedIndex();
			int i=selectedIdx;
			if(i==-1) {
				i=0;
			}
			while(true) {
				ScreenPat screenPat=_listScreenPats[i];
				if(screenPat.PatScreenPerm!=PatScreenPerm.Allowed) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//Skip people who aren't allowed
				}
				if(_listScreens.FirstOrDefault(x => x.ScreenPatNum==screenPat.ScreenPatNum)!=null) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//If they already have a screen, don't make a new one.  We might think about opening up their old one for editing at this point.
				}
				FormSE.ScreenPatCur=screenPat;
				if(_listScreens.Count==0) {
					FormSE.ScreenCur=new OpenDentBusiness.Screen();
					FormSE.ScreenCur.ScreenGroupOrder=1;
				}
				else {
					FormSE.ScreenCur=_listScreens[_listScreens.Count-1].Copy();//'remembers' the last entry, needs to be a deep copy so we don't change screen information.
					FormSE.ScreenCur.ScreenGroupOrder=FormSE.ScreenCur.ScreenGroupOrder+1;//increments for next
				}
				Patient pat=Patients.GetPat(screenPat.PatNum);//Get a patient so we can pre-fill some of the information (age/sex/birthdate/grade)
				FormSE.ScreenCur.Age=(pat.Birthdate==DateTime.MinValue) ? FormSE.ScreenCur.Age : (byte)pat.Age;
				FormSE.ScreenCur.Birthdate=(pat.Birthdate==DateTime.MinValue) ? FormSE.ScreenCur.Birthdate : pat.Birthdate;
				FormSE.ScreenCur.Gender=pat.Gender;//Default value in pat edit is male. No way of knowing if it's intentional or not, just use it.
				FormSE.ScreenCur.GradeLevel=(pat.GradeLevel==0) ? FormSE.ScreenCur.GradeLevel : pat.GradeLevel;//Default value is Unknown, use pat's grade if it's not unknown.
				FormSE.ScreenCur.RaceOld=PatientRaceOld.Unknown;//Default to unknown. Patient Edit doesn't have the same type of race as screen edit.
				FormSE.ScreenCur.Urgency=pat.Urgency;
				if(FormSE.ShowDialog()!=DialogResult.OK) {
					break;
				}
				FormSE.ScreenCur.ScreenGroupOrder++;
				FillGrid();
				i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
				if(i==selectedIdx && selectedIdx!=-1) {
					break;
				}
				if(i==0 && selectedIdx==-1) {
					break;
				}
			}
			FillScreenPats();
		}

		private void StartScreensForPatsWithSheets() {
			//Get the first custom Screening sheet or use the internal one
			SheetDef sheetDef=null;
			if(comboSheetDefs.SelectedIndex==0) {
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.Screening);
			}
			else {
				sheetDef=_listScreeningSheetDefs[comboSheetDefs.SelectedIndex-1];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			int selectedIdx=gridScreenPats.GetSelectedIndex();
			int i=selectedIdx;
			if(i==-1) {
				i=0;
			}
			while(true) {
				ScreenPat screenPat=_listScreenPats[i];
				if(screenPat.PatScreenPerm!=PatScreenPerm.Allowed) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//Skip people who aren't allowed
				}
				if(_listScreens.FirstOrDefault(x => x.ScreenPatNum==screenPat.ScreenPatNum)!=null) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//If they already have a screen, don't make a new one.  We might think about opening up their old one for editing at this point.
				}
				Sheet sheet=SheetUtil.CreateSheet(sheetDef);
				sheet.IsNew=true;
				sheet.PatNum=screenPat.PatNum;
				long provNum=0;
				if(comboProv.SelectedIndex!=-1) {
					provNum=_listProvs[comboProv.SelectedIndex].ProvNum;
				}
				SheetParameter.SetParameter(sheet,"ScreenGroupNum",_screenGroup.ScreenGroupNum);//I think we may need this.
				SheetParameter.SetParameter(sheet,"PatNum",screenPat.PatNum);
				SheetParameter.SetParameter(sheet,"ProvNum",provNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				//Create a valid screen so that we can create a screening sheet with the corresponding ScreenNum.
				OpenDentBusiness.Screen screen=new OpenDentBusiness.Screen();
				screen.ScreenPatNum=screenPat.ScreenPatNum;
				screen.ScreenGroupNum=_screenGroup.ScreenGroupNum;
				screen.ScreenGroupOrder=1;
				if(_listScreens.Count!=0) {
					screen.ScreenGroupOrder=_listScreens.Last().ScreenGroupOrder+1;//increments for next
				}
				List<SheetField> listChartOrigVals=new List<SheetField>();
				SheetField sheetFieldTreatment=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantTreatment" && x.FieldType==SheetFieldType.ScreenChart);
				SheetField sheetFieldComplete=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantComplete" && x.FieldType==SheetFieldType.ScreenChart);
				listChartOrigVals.Add(sheetFieldTreatment==null ? null : sheetFieldTreatment.Copy());//Adds null entry if it's not found, which is fine.
				listChartOrigVals.Add(sheetFieldComplete==null ? null : sheetFieldComplete.Copy());//Adds null entry if it's not found, which is fine.
				List<SheetField> listProcOrigVals=new List<SheetField>();
				SheetField sheetFieldAssess=sheet.SheetFields.Find(x=>x.FieldName=="AssessmentProc" && x.FieldType==SheetFieldType.CheckBox);
				SheetField sheetFieldFluoride=sheet.SheetFields.Find(x=>x.FieldName=="FluorideProc" && x.FieldType==SheetFieldType.CheckBox);
				if(sheetFieldAssess!=null) {
					listProcOrigVals.Add(sheetFieldAssess.Copy());
				}
				if(sheetFieldFluoride!=null) {
					listProcOrigVals.Add(sheetFieldFluoride.Copy());
				}
				List<SheetField> listSheetFieldProcs=sheet.SheetFields.FindAll(x => x.FieldName.StartsWith("Proc") && x.FieldType==SheetFieldType.CheckBox);
				listSheetFieldProcs.ForEach(x => listProcOrigVals.Add(x.Copy()));//Adds other proc checkbox fields.
				using FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheet);
				FormSFE.ShowDialog();
				if(FormSFE.DialogResult!=DialogResult.OK) {
					break;
				}
				if(FormSFE.SheetCur!=null) {//It wasn't deleted, create a screen.
					Screens.CreateScreenFromSheet(sheet,screen);
					//Now try and process the screening chart for treatment planned sealants.
					if(ProcedureCodes.GetCodeNum("D1351")==0) {
						MsgBox.Show(this,"The required sealant code is not present in the database.  The screening chart will not be processed.");
						break;
					}
					//Process both TP and Compl charts.
					Screens.ProcessScreenChart(sheet,ScreenChartType.TP|ScreenChartType.C,provNum,FormSFE.SheetCur.SheetNum,listChartOrigVals,listProcOrigVals);
				}
				screen.ScreenGroupOrder++;
				FillGrid();
				i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
				if(i==selectedIdx && selectedIdx!=-1) {
					break;
				}
				if(i==0 && selectedIdx==-1) {
					break;
				}
			}
			FillScreenPats();
		}

		private void ViewScreenForPat(OpenDentBusiness.Screen screenCur) {
			using FormScreenEdit FormSE=new FormScreenEdit();
			FormSE.ScreenGroupCur=_screenGroup;
			FormSE.IsNew=false;
			FormSE.ScreenCur=screenCur;
			ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.ScreenPatNum==screenCur.ScreenPatNum);
			FormSE.ScreenPatCur=screenPat;//Null represents anonymous.
			FormSE.ShowDialog();
		}

		private void ViewScreenForPatWithSheets(OpenDentBusiness.Screen screenCur) {
			Sheet sheet=Sheets.GetSheet(screenCur.SheetNum);
			if(sheet==null) {
				MsgBox.Show(this,"Sheet no longer exists.  It may have been deleted from the Chart Module.");
				return;
			}
			List<SheetField> listChartOrigVals=new List<SheetField>();
			SheetField sheetFieldTreatment=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantTreatment");
			SheetField sheetFieldComplete=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantComplete");
			listChartOrigVals.Add(sheetFieldTreatment==null ? null : sheetFieldTreatment.Copy());//Adds null entry if it's not found, which is fine.
			listChartOrigVals.Add(sheetFieldComplete==null ? null : sheetFieldComplete.Copy());//Adds null entry if it's not found, which is fine.
			List<SheetField> listProcOrigVals=new List<SheetField>();
			SheetField sheetFieldAssess=sheet.SheetFields.Find(x=>x.FieldName=="AssessmentProc");
			SheetField sheetFieldFluoride=sheet.SheetFields.Find(x=>x.FieldName=="FluorideProc");
			if(sheetFieldAssess!=null) {
				listProcOrigVals.Add(sheetFieldAssess.Copy());
			}
			if(sheetFieldFluoride!=null) {
				listProcOrigVals.Add(sheetFieldFluoride.Copy());
			}
			List<SheetField> listSheetFieldProcs=sheet.SheetFields.FindAll(x => x.FieldName.StartsWith("Proc") && x.FieldType==SheetFieldType.CheckBox);
			listSheetFieldProcs.ForEach(x => listProcOrigVals.Add(x.Copy()));//Adds other proc checkbox fields.
			using FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheet);
			if(FormSFE.ShowDialog()==DialogResult.OK) {
				//Now try and process the screening chart for completed sealants.
				if(FormSFE.SheetCur==null) {
					return;
				}
				if(ProcedureCodes.GetCodeNum("D1351")==0) {
					MsgBox.Show(this,"The required sealant code is not present in the database.  The screening chart will not be processed.");
					return;
				}
				long provNum=0;
				if(comboProv.SelectedIndex!=-1) {
					provNum=_listProvs[comboProv.SelectedIndex].ProvNum;
				}
				Screens.ProcessScreenChart(sheet,ScreenChartType.TP|ScreenChartType.C,provNum,FormSFE.SheetCur.SheetNum,listChartOrigVals,listProcOrigVals);						
			}
			_listScreens[gridMain.GetSelectedIndex()]=Screens.CreateScreenFromSheet(sheet,_listScreens[gridMain.GetSelectedIndex()]);
		}

		private void patContextMenuItem_Click(object sender,EventArgs e) {
			if(gridScreenPats.GetSelectedIndex()==-1) {
				return;
			}
			int idx=patContextMenu.MenuItems.IndexOf((MenuItem)sender);
			_listScreenPats[gridScreenPats.GetSelectedIndex()].PatScreenPerm=(PatScreenPerm)idx;
			FillScreenPats();
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			using FormScreenEdit FormSE=new FormScreenEdit();
			FormSE.ScreenCur=_listScreens[gridMain.SelectedIndices[0]];
			FormSE.ScreenGroupCur=_screenGroup;
			FormSE.ShowDialog();
			if(FormSE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void textScreenDate_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			try{
				DateTime.Parse(textScreenDate.Text);
			}
			catch{
				MessageBox.Show("Date invalid");
				e.Cancel=true;
			}
		}

		private void textProvName_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			comboProv.SelectedIndex=-1;//set the provnum to none.
		}

		private void comboProv_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(comboProv.SelectedIndex!=-1) {//if a prov was selected
				//set the provname accordingly
				textProvName.Text=_listProvs[comboProv.SelectedIndex].LName+", "
					+_listProvs[comboProv.SelectedIndex].FName;
			}
		}

		private void comboProv_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboProv.SelectedIndex=-1;
			}
		}

		private void comboCounty_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboCounty.SelectedIndex=-1;
			}
		}

		private void comboGradeSchool_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboGradeSchool.SelectedIndex=-1;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				ViewScreenForPatWithSheets(_listScreens[e.Row]);
			}
			else {
				ViewScreenForPat(_listScreens[e.Row]);
			}
			FillGrid();
			FillScreenPats();
		}
		
		private void gridScreenPats_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Family fam=Patients.GetFamily(_listScreenPats[gridScreenPats.GetSelectedIndex()].PatNum);
			Patient pat=fam.GetPatient(_listScreenPats[gridScreenPats.GetSelectedIndex()].PatNum);
			using FormPatientEdit FormPE=new FormPatientEdit(pat,fam);
			if(FormPE.ShowDialog()==DialogResult.OK) {//Information may have changed. Look for screens for this patient that may need changing.
				ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.PatNum==pat.PatNum);
				foreach(OpenDentBusiness.Screen screen in _listScreens){
					if(screen.ScreenPatNum==screenPat.ScreenPatNum) {//Found a screen belonging to the edited patient.
						//Don't unintelligently overwrite the screen's values.  They may have entered in patient information not pertaining to the screen such as address.
						screen.Birthdate=(pat.Birthdate==DateTime.MinValue)?screen.Birthdate:pat.Birthdate;//If birthdate isn't entered in pat select it will be mindate.
						screen.Age=(pat.Birthdate==DateTime.MinValue)?screen.Age:(byte)pat.Age;
						screen.Gender=pat.Gender;//Default value in pat edit is male. No way of knowing if it's intentional or not, just use it.
						screen.GradeLevel=(pat.GradeLevel==0)?screen.GradeLevel:pat.GradeLevel;//Default value is Unknown, use pat's grade if it's not unknown.
						Screens.Update(screen);
						FillGrid();
					}
				}
			}
		}

		private void butAddAnonymous_Click(object sender,System.EventArgs e) {
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				AddAnonymousScreensForSheets();
			}
			else {
				AddAnonymousScreens();
			}
		}

		private void butAddPat_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult==DialogResult.OK) {
				ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.PatNum==FormPS.SelectedPatNum);
				if(screenPat!=null) {
					MsgBox.Show(this,"Cannot add patient already in screen group.");
					for(int i=0;i<_listScreenPats.Count;i++) {
						if(_listScreenPats[i].ScreenPatNum==screenPat.ScreenPatNum) {
							gridScreenPats.SetSelected(i,true);
							break;
						}
					}
					return;
				}
				screenPat=new ScreenPat();
				screenPat.PatNum=FormPS.SelectedPatNum;
				screenPat.PatScreenPerm=PatScreenPerm.Unknown;
				screenPat.ScreenGroupNum=_screenGroup.ScreenGroupNum;
				ScreenPats.Insert(screenPat);
				_listScreenPats.Add(screenPat);
				_listScreenPatsOld.Add(screenPat.Clone());
				FillScreenPats();
			}
		}
		
		private void butRemovePat_Click(object sender,EventArgs e) {
			if(gridScreenPats.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a patient to remove first.");
				return;
			}
			_listScreenPats.RemoveAt(gridScreenPats.GetSelectedIndex());
			FillScreenPats();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this screening group? All screenings in this group will be deleted.")) {
				return;
			}
			ScreenGroups.Delete(_screenGroup);//Also deletes screens.
			DialogResult=DialogResult.OK;
		}

		private void butStartScreens_Click(object sender,EventArgs e) {
			if(_listScreenPats.Count==0) {
				MsgBox.Show(this,"No patients to screen.");
				return;
			}
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				StartScreensForPatsWithSheets();
			}
			else {
				StartScreensForPats();
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Description field cannot be blank.");
				textDescription.Focus();
				return;
			}
			_screenGroup.SGDate=PIn.Date(textScreenDate.Text);
			_screenGroup.Description=textDescription.Text;
			_screenGroup.ProvName=textProvName.Text;
			_screenGroup.ProvNum=comboProv.SelectedIndex==-1 ? 0 : _listProvs[comboProv.SelectedIndex].ProvNum;//ProvNum 0 is OK for screens.
			if(comboCounty.SelectedIndex==-1) {
				_screenGroup.County="";
			}
			else {
				_screenGroup.County=comboCounty.SelectedItem.ToString();
			}
			if(comboGradeSchool.SelectedIndex==-1) {
				_screenGroup.GradeSchool="";
			}
			else {
				_screenGroup.GradeSchool=comboGradeSchool.SelectedItem.ToString();
			}
			_screenGroup.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			if(comboSheetDefs.SelectedIndex<1) {
				_screenGroup.SheetDefNum=0;
			}
			else {
				_screenGroup.SheetDefNum=_listScreeningSheetDefs[comboSheetDefs.SelectedIndex-1].SheetDefNum;
			}
			ScreenPats.Sync(_listScreenPats,_listScreenPatsOld);
			ScreenGroups.Update(_screenGroup);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormScreenGroupEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_screenGroup.IsNew && DialogResult==DialogResult.Cancel) {
				ScreenGroups.Delete(_screenGroup);//Also deletes screens.
			}
		}
		

		

		

		

		

		

		

		

		

		

		

		

		

		

		

		


		

		

		


	}
}





















