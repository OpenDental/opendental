using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormScreenGroupEdit {
		private System.ComponentModel.IContainer components=null;

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
		private UI.Button butCancel;
		private UI.Button butDelete;
		private UI.GridOD gridMain;
		private UI.GridOD gridScreenPats;
		private UI.Button button1;
		private UI.Button butRemovePat;
		private UI.Button butStartScreens;
		private Label label4;
		private ComboBox comboSheetDefs;
		private Label labelSheet;
		private ContextMenu patContextMenu;
		private MenuItem menuItemUnknown;
		private MenuItem menuItemAllowed;
		private MenuItem menuItemNoPermission;
		private MenuItem menuItemRefused;
		private MenuItem menuItemAbsent;
		private MenuItem menuItemBehavior;
		private MenuItem menuItemOther;
	}
}
