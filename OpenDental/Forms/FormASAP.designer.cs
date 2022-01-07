using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormASAP {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormASAP));
			this.butClose = new OpenDental.UI.Button();
			this.menuApptsRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuApptSelectPatient = new System.Windows.Forms.ToolStripMenuItem();
			this.menuApptSeeChart = new System.Windows.Forms.ToolStripMenuItem();
			this.menuApptSendToPinboard = new System.Windows.Forms.ToolStripMenuItem();
			this.menuApptRemoveFromASAP = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSelectPatient = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSeeChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRemoveFromASAP = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRecallsRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemMakeAppt = new System.Windows.Forms.ToolStripMenuItem();
			this.butPrint = new OpenDental.UI.Button();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butText = new OpenDental.UI.Button();
			this.comboEnd = new System.Windows.Forms.ComboBox();
			this.comboStart = new System.Windows.Forms.ComboBox();
			this.labelEnd = new System.Windows.Forms.Label();
			this.labelStart = new System.Windows.Forms.Label();
			this.butSendWebSched = new OpenDental.UI.Button();
			this.butWebSchedHist = new OpenDental.UI.Button();
			this.groupWebSched = new System.Windows.Forms.GroupBox();
			this.butWebSchedNotify = new OpenDental.UI.Button();
			this.labelOperatory = new System.Windows.Forms.Label();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageAppts = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.comboShowHygiene = new OpenDental.UI.ComboBoxOD();
			this.comboAptStatus = new OpenDental.UI.ComboBoxOD();
			this.labelAptStatus = new System.Windows.Forms.Label();
			this.gridAppts = new OpenDental.UI.GridOD();
			this.tabPageRecalls = new System.Windows.Forms.TabPage();
			this.gridRecalls = new OpenDental.UI.GridOD();
			this.textDateStart = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboNumberReminders = new System.Windows.Forms.ComboBox();
			this.textDateEnd = new OpenDental.ValidDate();
			this.label3 = new System.Windows.Forms.Label();
			this.checkGroupFamilies = new System.Windows.Forms.CheckBox();
			this.gridWebSched = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.codeRangeFilter = new OpenDental.UI.ODCodeRangeFilter();
			this.labelCodeRange = new System.Windows.Forms.Label();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.menuApptsRightClick.SuspendLayout();
			this.menuRecallsRightClick.SuspendLayout();
			this.groupWebSched.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPageAppts.SuspendLayout();
			this.tabPageRecalls.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(966, 597);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(87, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// menuApptsRightClick
			// 
			this.menuApptsRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuApptSelectPatient,
            this.menuApptSeeChart,
            this.menuApptSendToPinboard,
            this.menuApptRemoveFromASAP});
			this.menuApptsRightClick.Name = "_menuRightClick";
			this.menuApptsRightClick.Size = new System.Drawing.Size(179, 92);
			// 
			// menuApptSelectPatient
			// 
			this.menuApptSelectPatient.Name = "menuApptSelectPatient";
			this.menuApptSelectPatient.Size = new System.Drawing.Size(178, 22);
			this.menuApptSelectPatient.Text = "Select Patient";
			this.menuApptSelectPatient.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// menuApptSeeChart
			// 
			this.menuApptSeeChart.Name = "menuApptSeeChart";
			this.menuApptSeeChart.Size = new System.Drawing.Size(178, 22);
			this.menuApptSeeChart.Text = "See Chart";
			this.menuApptSeeChart.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// menuApptSendToPinboard
			// 
			this.menuApptSendToPinboard.Name = "menuApptSendToPinboard";
			this.menuApptSendToPinboard.Size = new System.Drawing.Size(178, 22);
			this.menuApptSendToPinboard.Text = "Send to Pinboard";
			this.menuApptSendToPinboard.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// menuApptRemoveFromASAP
			// 
			this.menuApptRemoveFromASAP.Name = "menuApptRemoveFromASAP";
			this.menuApptRemoveFromASAP.Size = new System.Drawing.Size(178, 22);
			this.menuApptRemoveFromASAP.Text = "Remove from ASAP";
			this.menuApptRemoveFromASAP.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// toolStripMenuItemSelectPatient
			// 
			this.toolStripMenuItemSelectPatient.Name = "toolStripMenuItemSelectPatient";
			this.toolStripMenuItemSelectPatient.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemSelectPatient.Text = "Select Patient";
			this.toolStripMenuItemSelectPatient.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// toolStripMenuItemSeeChart
			// 
			this.toolStripMenuItemSeeChart.Name = "toolStripMenuItemSeeChart";
			this.toolStripMenuItemSeeChart.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemSeeChart.Text = "See Chart";
			this.toolStripMenuItemSeeChart.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// toolStripMenuItemRemoveFromASAP
			// 
			this.toolStripMenuItemRemoveFromASAP.Name = "toolStripMenuItemRemoveFromASAP";
			this.toolStripMenuItemRemoveFromASAP.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemRemoveFromASAP.Text = "Remove from ASAP";
			this.toolStripMenuItemRemoveFromASAP.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// menuRecallsRightClick
			// 
			this.menuRecallsRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectPatient,
            this.toolStripMenuItemSeeChart,
            this.toolStripMenuItemMakeAppt,
            this.toolStripMenuItemRemoveFromASAP});
			this.menuRecallsRightClick.Name = "_menuRightClick";
			this.menuRecallsRightClick.Size = new System.Drawing.Size(179, 92);
			// 
			// toolStripMenuItemMakeAppt
			// 
			this.toolStripMenuItemMakeAppt.Name = "toolStripMenuItemMakeAppt";
			this.toolStripMenuItemMakeAppt.Size = new System.Drawing.Size(178, 22);
			this.toolStripMenuItemMakeAppt.Text = "Make Appointment";
			this.toolStripMenuItemMakeAppt.Click += new System.EventHandler(this.gridMenuRight_click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(967, 506);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 21;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(85, 32);
			this.comboProv.MaxDropDownItems = 40;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(181, 21);
			this.comboProv.TabIndex = 33;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 14);
			this.label4.TabIndex = 32;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(623, 32);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(181, 21);
			this.comboSite.TabIndex = 37;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(551, 36);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(71, 14);
			this.labelSite.TabIndex = 36;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(334, 32);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(203, 21);
			this.comboClinic.TabIndex = 39;
			// 
			// butText
			// 
			this.butText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butText.Icon = OpenDental.UI.EnumIcons.Text;
			this.butText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butText.Location = new System.Drawing.Point(967, 202);
			this.butText.Name = "butText";
			this.butText.Size = new System.Drawing.Size(87, 24);
			this.butText.TabIndex = 62;
			this.butText.Text = "Text";
			this.butText.Click += new System.EventHandler(this.butText_Click);
			// 
			// comboEnd
			// 
			this.comboEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEnd.FormattingEnabled = true;
			this.comboEnd.Location = new System.Drawing.Point(15, 101);
			this.comboEnd.MaxDropDownItems = 48;
			this.comboEnd.Name = "comboEnd";
			this.comboEnd.Size = new System.Drawing.Size(108, 21);
			this.comboEnd.TabIndex = 66;
			this.comboEnd.Visible = false;
			this.comboEnd.SelectionChangeCommitted += new System.EventHandler(this.comboEnd_SelectionChangeCommitted);
			// 
			// comboStart
			// 
			this.comboStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStart.FormattingEnabled = true;
			this.comboStart.Location = new System.Drawing.Point(15, 59);
			this.comboStart.MaxDropDownItems = 48;
			this.comboStart.Name = "comboStart";
			this.comboStart.Size = new System.Drawing.Size(108, 21);
			this.comboStart.TabIndex = 65;
			this.comboStart.Visible = false;
			this.comboStart.SelectedIndexChanged += new System.EventHandler(this.comboStart_SelectedIndexChanged);
			// 
			// labelEnd
			// 
			this.labelEnd.Location = new System.Drawing.Point(17, 82);
			this.labelEnd.Name = "labelEnd";
			this.labelEnd.Size = new System.Drawing.Size(106, 16);
			this.labelEnd.TabIndex = 64;
			this.labelEnd.Text = "End Time";
			this.labelEnd.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelEnd.Visible = false;
			// 
			// labelStart
			// 
			this.labelStart.Location = new System.Drawing.Point(16, 42);
			this.labelStart.Name = "labelStart";
			this.labelStart.Size = new System.Drawing.Size(107, 16);
			this.labelStart.TabIndex = 63;
			this.labelStart.Text = "Start Time";
			this.labelStart.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelStart.Visible = false;
			// 
			// butSendWebSched
			// 
			this.butSendWebSched.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSendWebSched.Location = new System.Drawing.Point(15, 136);
			this.butSendWebSched.Name = "butSendWebSched";
			this.butSendWebSched.Size = new System.Drawing.Size(108, 24);
			this.butSendWebSched.TabIndex = 67;
			this.butSendWebSched.Text = "Send";
			this.butSendWebSched.Click += new System.EventHandler(this.butWebSched_Click);
			// 
			// butWebSchedHist
			// 
			this.butWebSchedHist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butWebSchedHist.Location = new System.Drawing.Point(15, 166);
			this.butWebSchedHist.Name = "butWebSchedHist";
			this.butWebSchedHist.Size = new System.Drawing.Size(108, 24);
			this.butWebSchedHist.TabIndex = 68;
			this.butWebSchedHist.Text = "History";
			this.butWebSchedHist.Click += new System.EventHandler(this.butWebSchedHist_Click);
			// 
			// groupWebSched
			// 
			this.groupWebSched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupWebSched.Controls.Add(this.butWebSchedNotify);
			this.groupWebSched.Controls.Add(this.labelOperatory);
			this.groupWebSched.Controls.Add(this.butSendWebSched);
			this.groupWebSched.Controls.Add(this.butWebSchedHist);
			this.groupWebSched.Controls.Add(this.labelEnd);
			this.groupWebSched.Controls.Add(this.comboEnd);
			this.groupWebSched.Controls.Add(this.comboStart);
			this.groupWebSched.Controls.Add(this.labelStart);
			this.groupWebSched.Location = new System.Drawing.Point(940, 259);
			this.groupWebSched.Name = "groupWebSched";
			this.groupWebSched.Size = new System.Drawing.Size(140, 229);
			this.groupWebSched.TabIndex = 69;
			this.groupWebSched.TabStop = false;
			this.groupWebSched.Text = "Web Sched ASAP";
			// 
			// butWebSchedNotify
			// 
			this.butWebSchedNotify.Location = new System.Drawing.Point(15, 196);
			this.butWebSchedNotify.Name = "butWebSchedNotify";
			this.butWebSchedNotify.Size = new System.Drawing.Size(109, 24);
			this.butWebSchedNotify.TabIndex = 75;
			this.butWebSchedNotify.Text = "Notification Settings";
			this.butWebSchedNotify.UseVisualStyleBackColor = true;
			this.butWebSchedNotify.Click += new System.EventHandler(this.butWebSchedNotify_Click);
			// 
			// labelOperatory
			// 
			this.labelOperatory.Location = new System.Drawing.Point(6, 16);
			this.labelOperatory.Name = "labelOperatory";
			this.labelOperatory.Size = new System.Drawing.Size(130, 26);
			this.labelOperatory.TabIndex = 69;
			this.labelOperatory.Text = "Operatory:";
			this.labelOperatory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelOperatory.Visible = false;
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.Location = new System.Drawing.Point(12, 78);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.tabControl);
			this.splitContainer.Panel1MinSize = 50;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.gridWebSched);
			this.splitContainer.Panel2MinSize = 0;
			this.splitContainer.Size = new System.Drawing.Size(920, 546);
			this.splitContainer.SplitterDistance = 409;
			this.splitContainer.TabIndex = 70;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageAppts);
			this.tabControl.Controls.Add(this.tabPageRecalls);
			this.tabControl.Location = new System.Drawing.Point(0, 3);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(920, 407);
			this.tabControl.TabIndex = 73;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPageAppts
			// 
			this.tabPageAppts.BackColor = System.Drawing.Color.Transparent;
			this.tabPageAppts.Controls.Add(this.label5);
			this.tabPageAppts.Controls.Add(this.comboShowHygiene);
			this.tabPageAppts.Controls.Add(this.comboAptStatus);
			this.tabPageAppts.Controls.Add(this.labelAptStatus);
			this.tabPageAppts.Controls.Add(this.gridAppts);
			this.tabPageAppts.Location = new System.Drawing.Point(4, 22);
			this.tabPageAppts.Name = "tabPageAppts";
			this.tabPageAppts.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAppts.Size = new System.Drawing.Size(912, 381);
			this.tabPageAppts.TabIndex = 0;
			this.tabPageAppts.Text = "Appointments";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(335, 10);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(61, 14);
			this.label5.TabIndex = 86;
			this.label5.Text = "Show";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboShowHygiene
			// 
			this.comboShowHygiene.Location = new System.Drawing.Point(402, 6);
			this.comboShowHygiene.Name = "comboShowHygiene";
			this.comboShowHygiene.Size = new System.Drawing.Size(189, 21);
			this.comboShowHygiene.TabIndex = 85;
			this.comboShowHygiene.SelectionChangeCommitted += new System.EventHandler(this.comboShowHygiene_SelectionChangeCommitted);
			// 
			// comboAptStatus
			// 
			this.comboAptStatus.BackColor = System.Drawing.SystemColors.Window;
			this.comboAptStatus.Location = new System.Drawing.Point(125, 6);
			this.comboAptStatus.Name = "comboAptStatus";
			this.comboAptStatus.SelectionModeMulti = true;
			this.comboAptStatus.Size = new System.Drawing.Size(181, 21);
			this.comboAptStatus.TabIndex = 84;
			// 
			// labelAptStatus
			// 
			this.labelAptStatus.Location = new System.Drawing.Point(8, 10);
			this.labelAptStatus.Name = "labelAptStatus";
			this.labelAptStatus.Size = new System.Drawing.Size(115, 14);
			this.labelAptStatus.TabIndex = 83;
			this.labelAptStatus.Text = "Appointment Status";
			this.labelAptStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridAppts
			// 
			this.gridAppts.AllowSortingByColumn = true;
			this.gridAppts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAppts.ContextMenuStrip = this.menuApptsRightClick;
			this.gridAppts.HScrollVisible = true;
			this.gridAppts.Location = new System.Drawing.Point(1, 33);
			this.gridAppts.Name = "gridAppts";
			this.gridAppts.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAppts.Size = new System.Drawing.Size(905, 348);
			this.gridAppts.TabIndex = 8;
			this.gridAppts.Title = "Appointment ASAP List";
			this.gridAppts.TranslationName = "TableASAP";
			this.gridAppts.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAppts_CellDoubleClick);
			this.gridAppts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridAppts_MouseUp);
			// 
			// tabPageRecalls
			// 
			this.tabPageRecalls.BackColor = System.Drawing.Color.Transparent;
			this.tabPageRecalls.Controls.Add(this.gridRecalls);
			this.tabPageRecalls.Controls.Add(this.textDateStart);
			this.tabPageRecalls.Controls.Add(this.label1);
			this.tabPageRecalls.Controls.Add(this.label2);
			this.tabPageRecalls.Controls.Add(this.comboNumberReminders);
			this.tabPageRecalls.Controls.Add(this.textDateEnd);
			this.tabPageRecalls.Controls.Add(this.label3);
			this.tabPageRecalls.Controls.Add(this.checkGroupFamilies);
			this.tabPageRecalls.Location = new System.Drawing.Point(4, 22);
			this.tabPageRecalls.Name = "tabPageRecalls";
			this.tabPageRecalls.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRecalls.Size = new System.Drawing.Size(912, 381);
			this.tabPageRecalls.TabIndex = 1;
			this.tabPageRecalls.Text = "Unscheduled Recalls";
			// 
			// gridRecalls
			// 
			this.gridRecalls.AllowSortingByColumn = true;
			this.gridRecalls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRecalls.ContextMenuStrip = this.menuRecallsRightClick;
			this.gridRecalls.HScrollVisible = true;
			this.gridRecalls.Location = new System.Drawing.Point(1, 55);
			this.gridRecalls.Name = "gridRecalls";
			this.gridRecalls.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridRecalls.Size = new System.Drawing.Size(910, 325);
			this.gridRecalls.TabIndex = 9;
			this.gridRecalls.Title = "Recall ASAP List";
			this.gridRecalls.TranslationName = "TableASAP";
			this.gridRecalls.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRecalls_CellDoubleClick);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(91, 5);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(136, 20);
			this.textDateStart.TabIndex = 81;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 14);
			this.label1.TabIndex = 79;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 14);
			this.label2.TabIndex = 80;
			this.label2.Text = "End Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboNumberReminders
			// 
			this.comboNumberReminders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboNumberReminders.Location = new System.Drawing.Point(354, 27);
			this.comboNumberReminders.MaxDropDownItems = 40;
			this.comboNumberReminders.Name = "comboNumberReminders";
			this.comboNumberReminders.Size = new System.Drawing.Size(82, 21);
			this.comboNumberReminders.TabIndex = 78;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(91, 29);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(136, 20);
			this.textDateEnd.TabIndex = 82;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(233, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 14);
			this.label3.TabIndex = 77;
			this.label3.Text = "Show Reminders";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGroupFamilies
			// 
			this.checkGroupFamilies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.Location = new System.Drawing.Point(279, 5);
			this.checkGroupFamilies.Name = "checkGroupFamilies";
			this.checkGroupFamilies.Size = new System.Drawing.Size(157, 18);
			this.checkGroupFamilies.TabIndex = 74;
			this.checkGroupFamilies.Text = "Group Families";
			this.checkGroupFamilies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.UseVisualStyleBackColor = true;
			// 
			// gridWebSched
			// 
			this.gridWebSched.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebSched.ContextMenuStrip = this.menuApptsRightClick;
			this.gridWebSched.Location = new System.Drawing.Point(5, 3);
			this.gridWebSched.Name = "gridWebSched";
			this.gridWebSched.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSched.Size = new System.Drawing.Size(905, 127);
			this.gridWebSched.TabIndex = 9;
			this.gridWebSched.TabStop = false;
			this.gridWebSched.Title = "Web Sched ASAP Messages";
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(966, 85);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(87, 24);
			this.butRefresh.TabIndex = 71;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// codeRangeFilter
			// 
			this.codeRangeFilter.Location = new System.Drawing.Point(903, 32);
			this.codeRangeFilter.Name = "codeRangeFilter";
			this.codeRangeFilter.Size = new System.Drawing.Size(150, 37);
			this.codeRangeFilter.TabIndex = 72;
			// 
			// labelCodeRange
			// 
			this.labelCodeRange.Location = new System.Drawing.Point(810, 35);
			this.labelCodeRange.Name = "labelCodeRange";
			this.labelCodeRange.Size = new System.Drawing.Size(87, 17);
			this.labelCodeRange.TabIndex = 73;
			this.labelCodeRange.Text = "Code Range";
			this.labelCodeRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1089, 24);
			this.menuMain.TabIndex = 74;
			// 
			// FormASAP
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1089, 635);
			this.Controls.Add(this.labelCodeRange);
			this.Controls.Add(this.codeRangeFilter);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.groupWebSched);
			this.Controls.Add(this.butText);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.labelSite);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormASAP";
			this.Text = "ASAP List";
			this.Load += new System.EventHandler(this.FormASAP_Load);
			this.Shown += new System.EventHandler(this.FormASAP_Shown);
			this.menuApptsRightClick.ResumeLayout(false);
			this.menuRecallsRightClick.ResumeLayout(false);
			this.groupWebSched.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPageAppts.ResumeLayout(false);
			this.tabPageRecalls.ResumeLayout(false);
			this.tabPageRecalls.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridAppts;
		private OpenDental.UI.Button butPrint;
		private ComboBox comboProv;
		private Label label4;
		private ComboBox comboSite;
		private Label labelSite;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private ToolStripMenuItem toolStripMenuItemSelectPatient;
		private ToolStripMenuItem toolStripMenuItemSeeChart;
		private ToolStripMenuItem menuApptSendToPinboard;
		private ToolStripMenuItem toolStripMenuItemRemoveFromASAP;
		private ToolStripMenuItem toolStripMenuItemMakeAppt;
		private UI.Button butText;
		private ContextMenuStrip menuApptsRightClick;
		private ContextMenuStrip menuRecallsRightClick;
		private ComboBox comboEnd;
		private ComboBox comboStart;
		private Label labelEnd;
		private Label labelStart;
		private UI.Button butSendWebSched;
		private UI.Button butWebSchedHist;
		private GroupBox groupWebSched;
		private SplitContainer splitContainer;
		private OpenDental.UI.GridOD gridWebSched;
		private Label labelOperatory;
		private TabControl tabControl;
		private TabPage tabPageAppts;
		private TabPage tabPageRecalls;
		private OpenDental.UI.GridOD gridRecalls;
		private ComboBox comboNumberReminders;
		private Label label3;
		private CheckBox checkGroupFamilies;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private Label label2;
		private Label label1;
		private OpenDental.UI.ComboBoxOD comboAptStatus;
		private Label labelAptStatus;
		private ToolStripMenuItem menuApptSelectPatient;
		private ToolStripMenuItem menuApptSeeChart;
		private ToolStripMenuItem menuApptRemoveFromASAP;
		private UI.Button butRefresh;
		private OpenDental.UI.ODCodeRangeFilter codeRangeFilter;
		private Label labelCodeRange;
		private OpenDental.UI.MenuOD menuMain;
		private UI.Button butWebSchedNotify;
		private Label label5;
		private OpenDental.UI.ComboBoxOD comboShowHygiene;
	}
}
