using System.Drawing;

namespace OpenDental {
	partial class ControlTreat {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components=null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				components?.Dispose();
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlTreat));
			this.label1 = new System.Windows.Forms.Label();
			this.listSetPr = new OpenDental.UI.ListBox();
			this.pd2 = new System.Drawing.Printing.PrintDocument();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridPrint = new OpenDental.UI.GridOD();
			this.gridPreAuth = new OpenDental.UI.GridOD();
			this.gridPlans = new OpenDental.UI.GridOD();
			this.dateTimeTP = new System.Windows.Forms.DateTimePicker();
			this.tabControlShowSort = new OpenDental.UI.TabControl();
			this.tabPageShow = new OpenDental.UI.TabPage();
			this.checkShowDiscount = new OpenDental.UI.CheckBox();
			this.checkShowTotals = new OpenDental.UI.CheckBox();
			this.checkShowMaxDed = new OpenDental.UI.CheckBox();
			this.checkShowSubtotals = new OpenDental.UI.CheckBox();
			this.checkShowFees = new OpenDental.UI.CheckBox();
			this.checkShowIns = new OpenDental.UI.CheckBox();
			this.checkShowCompleted = new OpenDental.UI.CheckBox();
			this.tabPageSort = new OpenDental.UI.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.radioTreatPlanSortTooth = new System.Windows.Forms.RadioButton();
			this.radioTreatPlanSortOrder = new System.Windows.Forms.RadioButton();
			this.tabPagePrint = new OpenDental.UI.TabPage();
			this.checkPrintClassic = new OpenDental.UI.CheckBox();
			this.labelCheckInsFrequency = new System.Windows.Forms.Label();
			this.userControlIndIns = new OpenDental.DashIndividualInsurance();
			this.userControlFamIns = new OpenDental.DashFamilyInsurance();
			this.butPlannedAppt = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butSaveTP = new OpenDental.UI.Button();
			this.butNewTP = new OpenDental.UI.Button();
			this.butInsRem = new OpenDental.UI.Button();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.textNote = new OpenDental.ODtextBox();
			this.butSendToDevice = new OpenDental.UI.Button();
			this.menuConsent = new System.Windows.Forms.ContextMenu();
			this.userControlIndDis = new OpenDental.DashIndividualDiscount();
			this.tabControlShowSort.SuspendLayout();
			this.tabPageShow.SuspendLayout();
			this.tabPageSort.SuspendLayout();
			this.tabPagePrint.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(808, 178);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "Set Priority";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listSetPr
			// 
			this.listSetPr.Location = new System.Drawing.Point(810, 195);
			this.listSetPr.Name = "listSetPr";
			this.listSetPr.Size = new System.Drawing.Size(92, 199);
			this.listSetPr.TabIndex = 5;
			this.listSetPr.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listSetPr_MouseDown);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "");
			this.imageListMain.Images.SetKeyName(1, "");
			this.imageListMain.Images.SetKeyName(2, "");
			this.imageListMain.Images.SetKeyName(3, "Add.gif");
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(0, 182);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(790, 469);
			this.gridMain.TabIndex = 59;
			this.gridMain.Title = "Procedures";
			this.gridMain.TranslationName = "TableTP";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// gridPrint
			// 
			this.gridPrint.Location = new System.Drawing.Point(0, 0);
			this.gridPrint.Name = "gridPrint";
			this.gridPrint.Size = new System.Drawing.Size(150, 150);
			this.gridPrint.TabIndex = 0;
			this.gridPrint.Title = null;
			this.gridPrint.TranslationName = "TablePrint";
			// 
			// gridPreAuth
			// 
			this.gridPreAuth.Location = new System.Drawing.Point(693, 29);
			this.gridPreAuth.Name = "gridPreAuth";
			this.gridPreAuth.Size = new System.Drawing.Size(299, 146);
			this.gridPreAuth.TabIndex = 62;
			this.gridPreAuth.Title = "Pre Authorizations";
			this.gridPreAuth.TranslationName = "TablePreAuth";
			this.gridPreAuth.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPreAuth_CellDoubleClick);
			this.gridPreAuth.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPreAuth_CellClick);
			// 
			// gridPlans
			// 
			this.gridPlans.Location = new System.Drawing.Point(0, 29);
			this.gridPlans.Name = "gridPlans";
			this.gridPlans.Size = new System.Drawing.Size(426, 151);
			this.gridPlans.TabIndex = 60;
			this.gridPlans.Title = "Treatment Plans";
			this.gridPlans.TranslationName = "TableTPList";
			this.gridPlans.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPlans_CellDoubleClick);
			this.gridPlans.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPlans_CellClick);
			// 
			// dateTimeTP
			// 
			this.dateTimeTP.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeTP.Location = new System.Drawing.Point(911, 195);
			this.dateTimeTP.Name = "dateTimeTP";
			this.dateTimeTP.Size = new System.Drawing.Size(81, 20);
			this.dateTimeTP.TabIndex = 71;
			this.dateTimeTP.CloseUp += new System.EventHandler(this.dateTimeTP_OnLeave);
			// 
			// tabControlShowSort
			// 
			this.tabControlShowSort.Controls.Add(this.tabPageShow);
			this.tabControlShowSort.Controls.Add(this.tabPageSort);
			this.tabControlShowSort.Controls.Add(this.tabPagePrint);
			this.tabControlShowSort.Location = new System.Drawing.Point(525, 29);
			this.tabControlShowSort.Name = "tabControlShowSort";
			this.tabControlShowSort.Size = new System.Drawing.Size(166, 151);
			this.tabControlShowSort.TabIndex = 73;
			// 
			// tabPageShow
			// 
			this.tabPageShow.Controls.Add(this.checkShowDiscount);
			this.tabPageShow.Controls.Add(this.checkShowTotals);
			this.tabPageShow.Controls.Add(this.checkShowMaxDed);
			this.tabPageShow.Controls.Add(this.checkShowSubtotals);
			this.tabPageShow.Controls.Add(this.checkShowFees);
			this.tabPageShow.Controls.Add(this.checkShowIns);
			this.tabPageShow.Controls.Add(this.checkShowCompleted);
			this.tabPageShow.Location = new System.Drawing.Point(2, 21);
			this.tabPageShow.Name = "tabPageShow";
			this.tabPageShow.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageShow.Size = new System.Drawing.Size(162, 128);
			this.tabPageShow.TabIndex = 0;
			this.tabPageShow.Text = "Show";
			// 
			// checkShowDiscount
			// 
			this.checkShowDiscount.Checked = true;
			this.checkShowDiscount.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowDiscount.Location = new System.Drawing.Point(23, 71);
			this.checkShowDiscount.Name = "checkShowDiscount";
			this.checkShowDiscount.Size = new System.Drawing.Size(128, 17);
			this.checkShowDiscount.TabIndex = 32;
			this.checkShowDiscount.Text = "Discount";
			this.checkShowDiscount.Click += new System.EventHandler(this.checkShowDiscount_Click);
			// 
			// checkShowTotals
			// 
			this.checkShowTotals.Checked = true;
			this.checkShowTotals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowTotals.Location = new System.Drawing.Point(23, 105);
			this.checkShowTotals.Name = "checkShowTotals";
			this.checkShowTotals.Size = new System.Drawing.Size(128, 15);
			this.checkShowTotals.TabIndex = 31;
			this.checkShowTotals.Text = "Totals";
			this.checkShowTotals.Click += new System.EventHandler(this.checkShowTotals_Click);
			// 
			// checkShowMaxDed
			// 
			this.checkShowMaxDed.Checked = true;
			this.checkShowMaxDed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowMaxDed.Location = new System.Drawing.Point(5, 20);
			this.checkShowMaxDed.Name = "checkShowMaxDed";
			this.checkShowMaxDed.Size = new System.Drawing.Size(146, 17);
			this.checkShowMaxDed.TabIndex = 30;
			this.checkShowMaxDed.Text = "Use Ins Max and Deduct";
			this.checkShowMaxDed.Click += new System.EventHandler(this.checkShowMaxDed_Click);
			// 
			// checkShowSubtotals
			// 
			this.checkShowSubtotals.Checked = true;
			this.checkShowSubtotals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowSubtotals.Location = new System.Drawing.Point(23, 88);
			this.checkShowSubtotals.Name = "checkShowSubtotals";
			this.checkShowSubtotals.Size = new System.Drawing.Size(128, 17);
			this.checkShowSubtotals.TabIndex = 29;
			this.checkShowSubtotals.Text = "Subtotals";
			this.checkShowSubtotals.Click += new System.EventHandler(this.checkShowSubtotals_Click);
			// 
			// checkShowFees
			// 
			this.checkShowFees.Checked = true;
			this.checkShowFees.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowFees.Location = new System.Drawing.Point(5, 37);
			this.checkShowFees.Name = "checkShowFees";
			this.checkShowFees.Size = new System.Drawing.Size(146, 17);
			this.checkShowFees.TabIndex = 28;
			this.checkShowFees.Text = "Fees";
			this.checkShowFees.Click += new System.EventHandler(this.checkShowFees_Click);
			// 
			// checkShowIns
			// 
			this.checkShowIns.Checked = true;
			this.checkShowIns.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowIns.Location = new System.Drawing.Point(23, 54);
			this.checkShowIns.Name = "checkShowIns";
			this.checkShowIns.Size = new System.Drawing.Size(128, 17);
			this.checkShowIns.TabIndex = 27;
			this.checkShowIns.Text = "Insurance Estimates";
			this.checkShowIns.Click += new System.EventHandler(this.checkShowIns_Click);
			// 
			// checkShowCompleted
			// 
			this.checkShowCompleted.Checked = true;
			this.checkShowCompleted.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowCompleted.Location = new System.Drawing.Point(5, 3);
			this.checkShowCompleted.Name = "checkShowCompleted";
			this.checkShowCompleted.Size = new System.Drawing.Size(146, 17);
			this.checkShowCompleted.TabIndex = 26;
			this.checkShowCompleted.Text = "Graphical Completed Tx";
			this.checkShowCompleted.Click += new System.EventHandler(this.checkShowCompleted_Click);
			// 
			// tabPageSort
			// 
			this.tabPageSort.Controls.Add(this.label6);
			this.tabPageSort.Controls.Add(this.radioTreatPlanSortTooth);
			this.tabPageSort.Controls.Add(this.radioTreatPlanSortOrder);
			this.tabPageSort.Location = new System.Drawing.Point(2, 21);
			this.tabPageSort.Name = "tabPageSort";
			this.tabPageSort.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSort.Size = new System.Drawing.Size(162, 128);
			this.tabPageSort.TabIndex = 1;
			this.tabPageSort.Text = "Sort by";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(146, 33);
			this.label6.TabIndex = 74;
			this.label6.Text = "Order procedures by priority, then date, then";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioTreatPlanSortTooth
			// 
			this.radioTreatPlanSortTooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortTooth.Location = new System.Drawing.Point(18, 44);
			this.radioTreatPlanSortTooth.Name = "radioTreatPlanSortTooth";
			this.radioTreatPlanSortTooth.Size = new System.Drawing.Size(116, 15);
			this.radioTreatPlanSortTooth.TabIndex = 73;
			this.radioTreatPlanSortTooth.Text = "Tooth";
			this.radioTreatPlanSortTooth.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortTooth.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioTreatPlanSortTooth_MouseClick);
			// 
			// radioTreatPlanSortOrder
			// 
			this.radioTreatPlanSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortOrder.Checked = true;
			this.radioTreatPlanSortOrder.Location = new System.Drawing.Point(18, 59);
			this.radioTreatPlanSortOrder.Name = "radioTreatPlanSortOrder";
			this.radioTreatPlanSortOrder.Size = new System.Drawing.Size(126, 19);
			this.radioTreatPlanSortOrder.TabIndex = 72;
			this.radioTreatPlanSortOrder.TabStop = true;
			this.radioTreatPlanSortOrder.Text = "Order Entered";
			this.radioTreatPlanSortOrder.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortOrder.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioTreatPlanSortTooth_MouseClick);
			// 
			// tabPagePrint
			// 
			this.tabPagePrint.Controls.Add(this.checkPrintClassic);
			this.tabPagePrint.Location = new System.Drawing.Point(2, 21);
			this.tabPagePrint.Name = "tabPagePrint";
			this.tabPagePrint.Padding = new System.Windows.Forms.Padding(3);
			this.tabPagePrint.Size = new System.Drawing.Size(162, 128);
			this.tabPagePrint.TabIndex = 2;
			this.tabPagePrint.Text = "Printing";
			// 
			// checkPrintClassic
			// 
			this.checkPrintClassic.Location = new System.Drawing.Point(7, 12);
			this.checkPrintClassic.Name = "checkPrintClassic";
			this.checkPrintClassic.Size = new System.Drawing.Size(146, 17);
			this.checkPrintClassic.TabIndex = 27;
			this.checkPrintClassic.Text = "Print using classic view";
			// 
			// labelCheckInsFrequency
			// 
			this.labelCheckInsFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCheckInsFrequency.Location = new System.Drawing.Point(908, 178);
			this.labelCheckInsFrequency.Name = "labelCheckInsFrequency";
			this.labelCheckInsFrequency.Size = new System.Drawing.Size(84, 15);
			this.labelCheckInsFrequency.TabIndex = 74;
			this.labelCheckInsFrequency.Text = "Estimates as of:";
			this.labelCheckInsFrequency.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// userControlIndIns
			// 
			this.userControlIndIns.Location = new System.Drawing.Point(799, 491);
			this.userControlIndIns.Name = "userControlIndIns";
			this.userControlIndIns.Size = new System.Drawing.Size(193, 160);
			this.userControlIndIns.TabIndex = 77;
			// 
			// userControlFamIns
			// 
			this.userControlFamIns.Location = new System.Drawing.Point(799, 411);
			this.userControlFamIns.Name = "userControlFamIns";
			this.userControlFamIns.Size = new System.Drawing.Size(193, 80);
			this.userControlFamIns.TabIndex = 76;
			// 
			// butPlannedAppt
			// 
			this.butPlannedAppt.Icon = OpenDental.UI.EnumIcons.Add;
			this.butPlannedAppt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPlannedAppt.Location = new System.Drawing.Point(429, 153);
			this.butPlannedAppt.Name = "butPlannedAppt";
			this.butPlannedAppt.Size = new System.Drawing.Size(90, 23);
			this.butPlannedAppt.TabIndex = 75;
			this.butPlannedAppt.Text = "Plan Appt";
			this.butPlannedAppt.Click += new System.EventHandler(this.butPlannedAppt_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(429, 122);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(90, 23);
			this.butRefresh.TabIndex = 72;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butSaveTP
			// 
			this.butSaveTP.Image = global::OpenDental.Properties.Resources.Copy;
			this.butSaveTP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSaveTP.Location = new System.Drawing.Point(429, 60);
			this.butSaveTP.Name = "butSaveTP";
			this.butSaveTP.Size = new System.Drawing.Size(90, 23);
			this.butSaveTP.TabIndex = 70;
			this.butSaveTP.Text = "Save TP";
			this.butSaveTP.Click += new System.EventHandler(this.butSaveTP_Click);
			// 
			// butNewTP
			// 
			this.butNewTP.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNewTP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNewTP.Location = new System.Drawing.Point(429, 29);
			this.butNewTP.Name = "butNewTP";
			this.butNewTP.Size = new System.Drawing.Size(90, 23);
			this.butNewTP.TabIndex = 69;
			this.butNewTP.Text = "New TP";
			this.butNewTP.Click += new System.EventHandler(this.butNewTP_Click);
			// 
			// butInsRem
			// 
			this.butInsRem.Location = new System.Drawing.Point(917, 400);
			this.butInsRem.Name = "butInsRem";
			this.butInsRem.Size = new System.Drawing.Size(75, 16);
			this.butInsRem.TabIndex = 68;
			this.butInsRem.Text = "Ins Rem";
			this.butInsRem.Visible = false;
			this.butInsRem.Click += new System.EventHandler(this.butInsRem_Click);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1195, 25);
			this.ToolBarMain.TabIndex = 58;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNote.BackColor = System.Drawing.SystemColors.Control;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(0, 654);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.TreatPlan;
			this.textNote.ReadOnly = true;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(790, 52);
			this.textNote.SpellCheckIsEnabled = false;
			this.textNote.TabIndex = 54;
			this.textNote.Text = "";
			this.textNote.TextChanged += new System.EventHandler(this.textNote_TextChanged);
			this.textNote.Leave += new System.EventHandler(this.textNote_Leave);
			// 
			// butSendToDevice
			// 
			this.butSendToDevice.Image = global::OpenDental.Properties.Resources.arrowRightLine;
			this.butSendToDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSendToDevice.Location = new System.Drawing.Point(429, 91);
			this.butSendToDevice.Name = "butSendToDevice";
			this.butSendToDevice.Size = new System.Drawing.Size(90, 23);
			this.butSendToDevice.TabIndex = 78;
			this.butSendToDevice.Text = "eClipboard";
			this.butSendToDevice.Click += new System.EventHandler(this.butSendToDevice_Click);
			// 
			// menuConsent
			// 
			this.menuConsent.Popup += new System.EventHandler(this.menuConsent_Popup);
			// 
			// userControlIndDis
			// 
			this.userControlIndDis.Location = new System.Drawing.Point(799, 422);
			this.userControlIndDis.Name = "userControlIndDis";
			this.userControlIndDis.Size = new System.Drawing.Size(173, 84);
			this.userControlIndDis.TabIndex = 79;
			// 
			// ControlTreat
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.userControlIndDis);
			this.Controls.Add(this.butSendToDevice);
			this.Controls.Add(this.butInsRem);
			this.Controls.Add(this.userControlIndIns);
			this.Controls.Add(this.userControlFamIns);
			this.Controls.Add(this.butPlannedAppt);
			this.Controls.Add(this.labelCheckInsFrequency);
			this.Controls.Add(this.tabControlShowSort);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.dateTimeTP);
			this.Controls.Add(this.butSaveTP);
			this.Controls.Add(this.butNewTP);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.listSetPr);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridPreAuth);
			this.Controls.Add(this.gridPlans);
			this.Controls.Add(this.textNote);
			this.Name = "ControlTreat";
			this.Size = new System.Drawing.Size(1195, 708);
			this.tabControlShowSort.ResumeLayout(false);
			this.tabPageShow.ResumeLayout(false);
			this.tabPageSort.ResumeLayout(false);
			this.tabPagePrint.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBox listSetPr;
		private UI.Button butRefresh;
		private OpenDental.UI.TabControl tabControlShowSort;
		private OpenDental.UI.TabPage tabPageShow;
		private OpenDental.UI.CheckBox checkShowDiscount;
		private OpenDental.UI.CheckBox checkShowTotals;
		private OpenDental.UI.CheckBox checkShowMaxDed;
		private OpenDental.UI.CheckBox checkShowSubtotals;
		private OpenDental.UI.CheckBox checkShowFees;
		private OpenDental.UI.CheckBox checkShowIns;
		private OpenDental.UI.CheckBox checkShowCompleted;
		private OpenDental.UI.TabPage tabPageSort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioTreatPlanSortTooth;
		private System.Windows.Forms.RadioButton radioTreatPlanSortOrder;
		private System.Windows.Forms.Label labelCheckInsFrequency;
		private OpenDental.UI.TabPage tabPagePrint;
		private OpenDental.UI.CheckBox checkPrintClassic;
		private UI.Button butPlannedAppt;
		private DashFamilyInsurance userControlFamIns;
		private DashIndividualDiscount userControlIndDis;
		private DashIndividualInsurance userControlIndIns;
		private UI.Button butSendToDevice;
		private UI.Button butInsRem;
		private UI.Button butNewTP;
		private UI.Button butSaveTP;
		private System.Drawing.Printing.PrintDocument pd2;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.GridOD gridPrint;
		private OpenDental.UI.GridOD gridPreAuth;
		private System.Windows.Forms.ContextMenu menuConsent;
		private System.Windows.Forms.ImageList imageListMain;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private OpenDental.UI.GridOD gridPlans;
		private System.Windows.Forms.DateTimePicker dateTimeTP;
		private System.Windows.Forms.Control _controlToothChart;
		private ODtextBox textNote;
	}
}