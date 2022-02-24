using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDatabaseMaintenance {
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseMaintenance));
			this.butClose = new OpenDental.UI.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butCheck = new OpenDental.UI.Button();
			this.checkShow = new System.Windows.Forms.CheckBox();
			this.butPrint = new OpenDental.UI.Button();
			this.butFix = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unhideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butNone = new OpenDental.UI.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.tabControlDBM = new System.Windows.Forms.TabControl();
			this.tabChecks = new System.Windows.Forms.TabPage();
			this.butStopDBM = new OpenDental.UI.Button();
			this.tabHidden = new System.Windows.Forms.TabPage();
			this.butSelectAll = new OpenDental.UI.Button();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.gridHidden = new OpenDental.UI.GridOD();
			this.tabOld = new System.Windows.Forms.TabPage();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.butStopDBMOld = new OpenDental.UI.Button();
			this.textNoneOld = new System.Windows.Forms.TextBox();
			this.butNoneOld = new OpenDental.UI.Button();
			this.butFixOld = new OpenDental.UI.Button();
			this.butCheckOld = new OpenDental.UI.Button();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.gridOld = new OpenDental.UI.GridOD();
			this.tabTools = new System.Windows.Forms.TabPage();
			this.groupBoxUpdateInProg = new System.Windows.Forms.GroupBox();
			this.labelUpdateInProgress = new System.Windows.Forms.Label();
			this.textBoxUpdateInProg = new System.Windows.Forms.TextBox();
			this.butClearUpdateInProgress = new OpenDental.UI.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.labelSkipCheckTable = new System.Windows.Forms.Label();
			this.gridTools = new OpenDental.UI.GridOD();
			this.butFixTools = new OpenDental.UI.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.tabControlDBM.SuspendLayout();
			this.tabChecks.SuspendLayout();
			this.tabHidden.SuspendLayout();
			this.tabOld.SuspendLayout();
			this.tabTools.SuspendLayout();
			this.groupBoxUpdateInProg.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(747, 654);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(6, 6);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(779, 40);
			this.textBox1.TabIndex = 1;
			this.textBox1.TabStop = false;
			this.textBox1.Text = "This tool will check the entire database for any improper settings, inconsistenci" +
    "es, or corruption.\r\nA log is automatically saved in RepairLog.txt if user has pe" +
    "rmission.";
			// 
			// butCheck
			// 
			this.butCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCheck.Location = new System.Drawing.Point(301, 570);
			this.butCheck.Name = "butCheck";
			this.butCheck.Size = new System.Drawing.Size(75, 26);
			this.butCheck.TabIndex = 4;
			this.butCheck.Text = "C&heck";
			this.butCheck.Click += new System.EventHandler(this.butCheck_Click);
			// 
			// checkShow
			// 
			this.checkShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShow.Location = new System.Drawing.Point(6, 515);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(447, 20);
			this.checkShow.TabIndex = 1;
			this.checkShow.Text = "Show me everything in the log  (only for advanced users)";
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(6, 570);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 26);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butFix
			// 
			this.butFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butFix.Location = new System.Drawing.Point(426, 570);
			this.butFix.Name = "butFix";
			this.butFix.Size = new System.Drawing.Size(75, 26);
			this.butFix.TabIndex = 5;
			this.butFix.Text = "&Fix";
			this.butFix.Click += new System.EventHandler(this.butFix_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenuStrip1;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(6, 52);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(790, 457);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Database Methods";
			this.gridMain.TranslationName = "TableClaimPaySplits";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.unhideToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(113, 48);
			// 
			// hideToolStripMenuItem
			// 
			this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
			this.hideToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.hideToolStripMenuItem.Text = "Hide";
			this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
			// 
			// unhideToolStripMenuItem
			// 
			this.unhideToolStripMenuItem.Name = "unhideToolStripMenuItem";
			this.unhideToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.unhideToolStripMenuItem.Text = "Unhide";
			this.unhideToolStripMenuItem.Click += new System.EventHandler(this.unhideToolStripMenuItem_Click);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNone.Location = new System.Drawing.Point(721, 515);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 26);
			this.butNone.TabIndex = 2;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.BackColor = System.Drawing.SystemColors.Control;
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox2.Location = new System.Drawing.Point(350, 515);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(365, 26);
			this.textBox2.TabIndex = 99;
			this.textBox2.TabStop = false;
			this.textBox2.Text = "No selections will cause all database methods to run.\r\nOtherwise only selected me" +
    "thods will run.\r\n";
			this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// tabControlDBM
			// 
			this.tabControlDBM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlDBM.Controls.Add(this.tabChecks);
			this.tabControlDBM.Controls.Add(this.tabHidden);
			this.tabControlDBM.Controls.Add(this.tabOld);
			this.tabControlDBM.Controls.Add(this.tabTools);
			this.tabControlDBM.Location = new System.Drawing.Point(12, 12);
			this.tabControlDBM.Name = "tabControlDBM";
			this.tabControlDBM.SelectedIndex = 0;
			this.tabControlDBM.Size = new System.Drawing.Size(810, 637);
			this.tabControlDBM.TabIndex = 0;
			// 
			// tabChecks
			// 
			this.tabChecks.BackColor = System.Drawing.SystemColors.Control;
			this.tabChecks.Controls.Add(this.butStopDBM);
			this.tabChecks.Controls.Add(this.textBox1);
			this.tabChecks.Controls.Add(this.butFix);
			this.tabChecks.Controls.Add(this.butPrint);
			this.tabChecks.Controls.Add(this.textBox2);
			this.tabChecks.Controls.Add(this.butCheck);
			this.tabChecks.Controls.Add(this.checkShow);
			this.tabChecks.Controls.Add(this.butNone);
			this.tabChecks.Controls.Add(this.gridMain);
			this.tabChecks.Location = new System.Drawing.Point(4, 22);
			this.tabChecks.Name = "tabChecks";
			this.tabChecks.Padding = new System.Windows.Forms.Padding(3);
			this.tabChecks.Size = new System.Drawing.Size(802, 611);
			this.tabChecks.TabIndex = 0;
			this.tabChecks.Text = "Checks";
			// 
			// butStopDBM
			// 
			this.butStopDBM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butStopDBM.Enabled = false;
			this.butStopDBM.Location = new System.Drawing.Point(545, 570);
			this.butStopDBM.Name = "butStopDBM";
			this.butStopDBM.Size = new System.Drawing.Size(75, 26);
			this.butStopDBM.TabIndex = 6;
			this.butStopDBM.Text = "&Stop DBM";
			this.butStopDBM.Click += new System.EventHandler(this.butStopDBM_Click);
			// 
			// tabHidden
			// 
			this.tabHidden.BackColor = System.Drawing.Color.Transparent;
			this.tabHidden.Controls.Add(this.butSelectAll);
			this.tabHidden.Controls.Add(this.textBox4);
			this.tabHidden.Controls.Add(this.gridHidden);
			this.tabHidden.Location = new System.Drawing.Point(4, 22);
			this.tabHidden.Name = "tabHidden";
			this.tabHidden.Padding = new System.Windows.Forms.Padding(3);
			this.tabHidden.Size = new System.Drawing.Size(802, 611);
			this.tabHidden.TabIndex = 2;
			this.tabHidden.Text = "Hidden";
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectAll.Location = new System.Drawing.Point(721, 496);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(75, 26);
			this.butSelectAll.TabIndex = 101;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// textBox4
			// 
			this.textBox4.BackColor = System.Drawing.SystemColors.Control;
			this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox4.Location = new System.Drawing.Point(6, 3);
			this.textBox4.Multiline = true;
			this.textBox4.Name = "textBox4";
			this.textBox4.ReadOnly = true;
			this.textBox4.Size = new System.Drawing.Size(779, 40);
			this.textBox4.TabIndex = 3;
			this.textBox4.TabStop = false;
			this.textBox4.Text = "This table shows all of the hidden database maintenance methods. You can unhide a" +
    " method by selecting a method, right clicking, and select Unhide.\r\n\r\n";
			// 
			// gridHidden
			// 
			this.gridHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridHidden.ContextMenuStrip = this.contextMenuStrip1;
			this.gridHidden.HasMultilineHeaders = true;
			this.gridHidden.Location = new System.Drawing.Point(6, 52);
			this.gridHidden.Name = "gridHidden";
			this.gridHidden.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridHidden.Size = new System.Drawing.Size(790, 438);
			this.gridHidden.TabIndex = 2;
			this.gridHidden.Title = "Hidden Methods";
			this.gridHidden.TranslationName = "TableHiddenDbmMethods";
			this.gridHidden.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridHidden_MouseUp);
			// 
			// tabOld
			// 
			this.tabOld.BackColor = System.Drawing.Color.Transparent;
			this.tabOld.Controls.Add(this.checkShowHidden);
			this.tabOld.Controls.Add(this.butStopDBMOld);
			this.tabOld.Controls.Add(this.textNoneOld);
			this.tabOld.Controls.Add(this.butNoneOld);
			this.tabOld.Controls.Add(this.butFixOld);
			this.tabOld.Controls.Add(this.butCheckOld);
			this.tabOld.Controls.Add(this.textBox5);
			this.tabOld.Controls.Add(this.gridOld);
			this.tabOld.Location = new System.Drawing.Point(4, 22);
			this.tabOld.Name = "tabOld";
			this.tabOld.Padding = new System.Windows.Forms.Padding(3);
			this.tabOld.Size = new System.Drawing.Size(802, 611);
			this.tabOld.TabIndex = 3;
			this.tabOld.Text = "Old";
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShowHidden.Location = new System.Drawing.Point(6, 563);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(134, 17);
			this.checkShowHidden.TabIndex = 103;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.UseVisualStyleBackColor = true;
			this.checkShowHidden.CheckedChanged += new System.EventHandler(this.checkShowHidden_CheckedChanged);
			// 
			// butStopDBMOld
			// 
			this.butStopDBMOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butStopDBMOld.Enabled = false;
			this.butStopDBMOld.Location = new System.Drawing.Point(545, 551);
			this.butStopDBMOld.Name = "butStopDBMOld";
			this.butStopDBMOld.Size = new System.Drawing.Size(75, 26);
			this.butStopDBMOld.TabIndex = 102;
			this.butStopDBMOld.Text = "&Stop DBM";
			this.butStopDBMOld.Click += new System.EventHandler(this.butStopDBMOld_Click);
			// 
			// textNoneOld
			// 
			this.textNoneOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textNoneOld.BackColor = System.Drawing.SystemColors.Control;
			this.textNoneOld.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textNoneOld.Location = new System.Drawing.Point(350, 496);
			this.textNoneOld.Multiline = true;
			this.textNoneOld.Name = "textNoneOld";
			this.textNoneOld.ReadOnly = true;
			this.textNoneOld.Size = new System.Drawing.Size(365, 26);
			this.textNoneOld.TabIndex = 101;
			this.textNoneOld.TabStop = false;
			this.textNoneOld.Text = "No selections will cause all database checks to run.\r\nOtherwise only selected che" +
    "cks will run.\r\n";
			this.textNoneOld.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butNoneOld
			// 
			this.butNoneOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNoneOld.Location = new System.Drawing.Point(721, 496);
			this.butNoneOld.Name = "butNoneOld";
			this.butNoneOld.Size = new System.Drawing.Size(75, 26);
			this.butNoneOld.TabIndex = 100;
			this.butNoneOld.Text = "None";
			this.butNoneOld.Click += new System.EventHandler(this.butNoneOld_Click);
			// 
			// butFixOld
			// 
			this.butFixOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butFixOld.Location = new System.Drawing.Point(423, 551);
			this.butFixOld.Name = "butFixOld";
			this.butFixOld.Size = new System.Drawing.Size(75, 26);
			this.butFixOld.TabIndex = 7;
			this.butFixOld.Text = "&Fix";
			this.butFixOld.Click += new System.EventHandler(this.butFixOld_Click);
			// 
			// butCheckOld
			// 
			this.butCheckOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCheckOld.Location = new System.Drawing.Point(298, 551);
			this.butCheckOld.Name = "butCheckOld";
			this.butCheckOld.Size = new System.Drawing.Size(75, 26);
			this.butCheckOld.TabIndex = 6;
			this.butCheckOld.Text = "C&heck";
			this.butCheckOld.Click += new System.EventHandler(this.butCheckOld_Click);
			// 
			// textBox5
			// 
			this.textBox5.BackColor = System.Drawing.SystemColors.Control;
			this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox5.Location = new System.Drawing.Point(6, 3);
			this.textBox5.Multiline = true;
			this.textBox5.Name = "textBox5";
			this.textBox5.ReadOnly = true;
			this.textBox5.Size = new System.Drawing.Size(779, 40);
			this.textBox5.TabIndex = 5;
			this.textBox5.TabStop = false;
			this.textBox5.Text = "This table shows database maintenance methods that have been deemed no longer nec" +
    "essary. Should not be ran unless directly told to do so.\r\n\r\n";
			// 
			// gridOld
			// 
			this.gridOld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOld.ContextMenuStrip = this.contextMenuStrip1;
			this.gridOld.HasMultilineHeaders = true;
			this.gridOld.Location = new System.Drawing.Point(6, 52);
			this.gridOld.Name = "gridOld";
			this.gridOld.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOld.Size = new System.Drawing.Size(790, 438);
			this.gridOld.TabIndex = 4;
			this.gridOld.Title = "Old Methods";
			this.gridOld.TranslationName = "TableOldDbmMethods";
			this.gridOld.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOld_CellDoubleClick);
			this.gridOld.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridOld_MouseUp);
			// 
			// tabTools
			// 
			this.tabTools.BackColor = System.Drawing.SystemColors.Control;
			this.tabTools.Controls.Add(this.butFixTools);
			this.tabTools.Controls.Add(this.gridTools);
			this.tabTools.Controls.Add(this.groupBoxUpdateInProg);
			this.tabTools.Controls.Add(this.textBox3);
			this.tabTools.Location = new System.Drawing.Point(4, 22);
			this.tabTools.Name = "tabTools";
			this.tabTools.Padding = new System.Windows.Forms.Padding(3);
			this.tabTools.Size = new System.Drawing.Size(802, 611);
			this.tabTools.TabIndex = 1;
			this.tabTools.Text = "Tools";
			// 
			// groupBoxUpdateInProg
			// 
			this.groupBoxUpdateInProg.Controls.Add(this.labelUpdateInProgress);
			this.groupBoxUpdateInProg.Controls.Add(this.textBoxUpdateInProg);
			this.groupBoxUpdateInProg.Controls.Add(this.butClearUpdateInProgress);
			this.groupBoxUpdateInProg.Location = new System.Drawing.Point(6, 8);
			this.groupBoxUpdateInProg.Name = "groupBoxUpdateInProg";
			this.groupBoxUpdateInProg.Size = new System.Drawing.Size(790, 66);
			this.groupBoxUpdateInProg.TabIndex = 57;
			this.groupBoxUpdateInProg.TabStop = false;
			this.groupBoxUpdateInProg.Text = "Update in progress on computer: ";
			// 
			// labelUpdateInProgress
			// 
			this.labelUpdateInProgress.Location = new System.Drawing.Point(6, 17);
			this.labelUpdateInProgress.Name = "labelUpdateInProgress";
			this.labelUpdateInProgress.Size = new System.Drawing.Size(778, 18);
			this.labelUpdateInProgress.TabIndex = 58;
			this.labelUpdateInProgress.Text = "Clear this value only if you are unable to start the program on other workstation" +
    "s and you are sure an update is not currently in progress.";
			this.labelUpdateInProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxUpdateInProg
			// 
			this.textBoxUpdateInProg.Location = new System.Drawing.Point(6, 41);
			this.textBoxUpdateInProg.Name = "textBoxUpdateInProg";
			this.textBoxUpdateInProg.ReadOnly = true;
			this.textBoxUpdateInProg.Size = new System.Drawing.Size(149, 20);
			this.textBoxUpdateInProg.TabIndex = 55;
			// 
			// butClearUpdateInProgress
			// 
			this.butClearUpdateInProgress.Location = new System.Drawing.Point(161, 39);
			this.butClearUpdateInProgress.Name = "butClearUpdateInProgress";
			this.butClearUpdateInProgress.Size = new System.Drawing.Size(78, 23);
			this.butClearUpdateInProgress.TabIndex = 54;
			this.butClearUpdateInProgress.Text = "Clear";
			this.butClearUpdateInProgress.UseVisualStyleBackColor = true;
			this.butClearUpdateInProgress.Click += new System.EventHandler(this.butClearUpdateInProgress_Click);
			// 
			// textBox3
			// 
			this.textBox3.BackColor = System.Drawing.SystemColors.Control;
			this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox3.Location = new System.Drawing.Point(6, 84);
			this.textBox3.Multiline = true;
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(790, 40);
			this.textBox3.TabIndex = 51;
			this.textBox3.TabStop = false;
			this.textBox3.Text = resources.GetString("textBox3.Text");
			// 
			// labelSkipCheckTable
			// 
			this.labelSkipCheckTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSkipCheckTable.ForeColor = System.Drawing.Color.Red;
			this.labelSkipCheckTable.Location = new System.Drawing.Point(587, 661);
			this.labelSkipCheckTable.Name = "labelSkipCheckTable";
			this.labelSkipCheckTable.Size = new System.Drawing.Size(144, 16);
			this.labelSkipCheckTable.TabIndex = 2;
			this.labelSkipCheckTable.Text = "Table check is disabled";
			this.labelSkipCheckTable.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.labelSkipCheckTable.Visible = false;
			// 
			// gridTools
			// 
			this.gridTools.Location = new System.Drawing.Point(0, 130);
			this.gridTools.Name = "gridTools";
			this.gridTools.Size = new System.Drawing.Size(802, 443);
			this.gridTools.TabIndex = 66;
			// 
			// butFixTools
			// 
			this.butFixTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butFixTools.Location = new System.Drawing.Point(721, 579);
			this.butFixTools.Name = "butFixTools";
			this.butFixTools.Size = new System.Drawing.Size(75, 26);
			this.butFixTools.TabIndex = 67;
			this.butFixTools.Text = "&Fix";
			this.butFixTools.Click += new System.EventHandler(this.butFixTools_Click);
			// 
			// FormDatabaseMaintenance
			// 
			this.AcceptButton = this.butCheck;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(834, 692);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelSkipCheckTable);
			this.Controls.Add(this.tabControlDBM);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDatabaseMaintenance";
			this.ShowInTaskbar = false;
			this.Text = "Database Maintenance";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDatabaseMaintenance_FormClosing);
			this.Load += new System.EventHandler(this.FormDatabaseMaintenance_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			this.tabControlDBM.ResumeLayout(false);
			this.tabChecks.ResumeLayout(false);
			this.tabChecks.PerformLayout();
			this.tabHidden.ResumeLayout(false);
			this.tabHidden.PerformLayout();
			this.tabOld.ResumeLayout(false);
			this.tabOld.PerformLayout();
			this.tabTools.ResumeLayout(false);
			this.tabTools.PerformLayout();
			this.groupBoxUpdateInProg.ResumeLayout(false);
			this.groupBoxUpdateInProg.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textBox1;
		private OpenDental.UI.Button butCheck;
		private CheckBox checkShow;
		private UI.Button butFix;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.GridOD gridMain;
		private UI.Button butNone;
		private TextBox textBox2;
		private TabControl tabControlDBM;
		private TabPage tabChecks;
		private TabPage tabTools;
		private TextBox textBox3;
		private Label labelSkipCheckTable;
		private GroupBox groupBoxUpdateInProg;
		private Label labelUpdateInProgress;
		private TextBox textBoxUpdateInProg;
		private UI.Button butClearUpdateInProgress;
		private TabPage tabHidden;
		private TextBox textBox4;
		private OpenDental.UI.GridOD gridHidden;
		private TabPage tabOld;
		private TextBox textBox5;
		private OpenDental.UI.GridOD gridOld;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem hideToolStripMenuItem;
		private ToolStripMenuItem unhideToolStripMenuItem;
		private UI.Button butFixOld;
		private UI.Button butCheckOld;
		private TextBox textNoneOld;
		private UI.Button butNoneOld;
		private UI.Button butSelectAll;
		private UI.Button butStopDBM;
		private UI.Button butStopDBMOld;
		private CheckBox checkShowHidden;
		private OpenDental.UI.GridOD gridTools;
		private UI.Button butFixTools;
	}
}
