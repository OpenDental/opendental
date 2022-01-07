using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpProcNote {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcNote));
			this.butClose = new OpenDental.UI.Button();
			this.checkNoNotes = new System.Windows.Forms.CheckBox();
			this.checkUnsignedNote = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupFilter = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioProcDate = new System.Windows.Forms.RadioButton();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.radioProc = new System.Windows.Forms.RadioButton();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboProvs = new OpenDental.UI.ComboBoxOD();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.butRefresh = new OpenDental.UI.Button();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.goToChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butExport = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkShowExcludedCodes = new System.Windows.Forms.CheckBox();
			this.groupFilter.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(796, 524);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkNoNotes
			// 
			this.checkNoNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoNotes.Location = new System.Drawing.Point(6, 16);
			this.checkNoNotes.Name = "checkNoNotes";
			this.checkNoNotes.Size = new System.Drawing.Size(350, 21);
			this.checkNoNotes.TabIndex = 0;
			this.checkNoNotes.Text = "Include procedures with no notes on any procedure for the same day";
			this.checkNoNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoNotes.UseVisualStyleBackColor = true;
			// 
			// checkUnsignedNote
			// 
			this.checkUnsignedNote.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnsignedNote.Location = new System.Drawing.Point(6, 38);
			this.checkUnsignedNote.Name = "checkUnsignedNote";
			this.checkUnsignedNote.Size = new System.Drawing.Size(350, 21);
			this.checkUnsignedNote.TabIndex = 1;
			this.checkUnsignedNote.Text = "Include procedures with a note that is unsigned";
			this.checkUnsignedNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnsignedNote.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(384, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 21);
			this.label1.TabIndex = 49;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupFilter
			// 
			this.groupFilter.Controls.Add(this.checkShowExcludedCodes);
			this.groupFilter.Controls.Add(this.groupBox1);
			this.groupFilter.Controls.Add(this.comboClinics);
			this.groupFilter.Controls.Add(this.comboProvs);
			this.groupFilter.Controls.Add(this.dateRangePicker);
			this.groupFilter.Controls.Add(this.butRefresh);
			this.groupFilter.Controls.Add(this.checkUnsignedNote);
			this.groupFilter.Controls.Add(this.checkNoNotes);
			this.groupFilter.Controls.Add(this.label1);
			this.groupFilter.Location = new System.Drawing.Point(12, 0);
			this.groupFilter.Name = "groupFilter";
			this.groupFilter.Size = new System.Drawing.Size(859, 118);
			this.groupFilter.TabIndex = 0;
			this.groupFilter.Text = "Filters";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioProcDate);
			this.groupBox1.Controls.Add(this.radioPatient);
			this.groupBox1.Controls.Add(this.radioProc);
			this.groupBox1.Location = new System.Drawing.Point(638, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(125, 79);
			this.groupBox1.TabIndex = 52;
			this.groupBox1.Text = "Group By";
			// 
			// radioProcDate
			// 
			this.radioProcDate.Location = new System.Drawing.Point(8, 53);
			this.radioProcDate.Name = "radioProcDate";
			this.radioProcDate.Size = new System.Drawing.Size(111, 18);
			this.radioProcDate.TabIndex = 2;
			this.radioProcDate.Text = "Date and Patient";
			this.radioProcDate.UseVisualStyleBackColor = true;
			this.radioProcDate.MouseCaptureChanged += new System.EventHandler(this.radioProcDate_MouseCaptureChanged);
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(8, 35);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.Size = new System.Drawing.Size(104, 18);
			this.radioPatient.TabIndex = 1;
			this.radioPatient.Text = "Patient";
			this.radioPatient.UseVisualStyleBackColor = true;
			this.radioPatient.MouseCaptureChanged += new System.EventHandler(this.radioPatient_MouseCaptureChanged);
			// 
			// radioProc
			// 
			this.radioProc.Checked = true;
			this.radioProc.Location = new System.Drawing.Point(8, 17);
			this.radioProc.Name = "radioProc";
			this.radioProc.Size = new System.Drawing.Size(104, 18);
			this.radioProc.TabIndex = 0;
			this.radioProc.TabStop = true;
			this.radioProc.Text = "Procedure";
			this.radioProc.UseVisualStyleBackColor = true;
			this.radioProc.MouseCaptureChanged += new System.EventHandler(this.radioProc_MouseCaptureChanged);
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeHiddenInAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(431, 17);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(197, 21);
			this.comboClinics.TabIndex = 3;
			// 
			// comboProvs
			// 
			this.comboProvs.BackColor = System.Drawing.SystemColors.Window;
			this.comboProvs.Location = new System.Drawing.Point(468, 38);
			this.comboProvs.Name = "comboProvs";
			this.comboProvs.SelectionModeMulti = true;
			this.comboProvs.Size = new System.Drawing.Size(160, 21);
			this.comboProvs.TabIndex = 4;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(169, 85);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(300, 21);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(446, 27);
			this.dateRangePicker.TabIndex = 5;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(769, 17);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 6;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToChartToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(125, 26);
			// 
			// goToChartToolStripMenuItem
			// 
			this.goToChartToolStripMenuItem.Name = "goToChartToolStripMenuItem";
			this.goToChartToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.goToChartToolStripMenuItem.Text = "See Chart";
			this.goToChartToolStripMenuItem.Click += new System.EventHandler(this.goToChartToolStripMenuItem_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(97, 526);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 3;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(12, 526);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 2;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenuStrip;
			this.gridMain.Location = new System.Drawing.Point(12, 124);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(859, 394);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Incomplete Procedure Notes";
			this.gridMain.TranslationName = "TableIncompleteProcNotes";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkShowExcludedCodes
			// 
			this.checkShowExcludedCodes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowExcludedCodes.Location = new System.Drawing.Point(6, 59);
			this.checkShowExcludedCodes.Name = "checkShowExcludedCodes";
			this.checkShowExcludedCodes.Size = new System.Drawing.Size(350, 21);
			this.checkShowExcludedCodes.TabIndex = 53;
			this.checkShowExcludedCodes.Text = "Show Excluded Codes";
			this.checkShowExcludedCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowExcludedCodes.UseVisualStyleBackColor = true;
			// 
			// FormRpProcNote
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(891, 572);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupFilter);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpProcNote";
			this.Text = "Incomplete Procedure Notes Report";
			this.Load += new System.EventHandler(this.FormRpProcNote_Load);
			this.groupFilter.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private CheckBox checkNoNotes;
		private CheckBox checkUnsignedNote;
		private Label label1;
		private GroupBox groupFilter;
		private UI.GridOD gridMain;
		private UI.Button butRefresh;
		private UI.ODDateRangePicker dateRangePicker;
		private UI.ComboBoxOD comboProvs;
		private UI.Button butExport;
		private UI.Button butPrint;
		private UI.ComboBoxClinicPicker comboClinics;
		private GroupBox groupBox1;
		private RadioButton radioPatient;
		private RadioButton radioProc;
		private RadioButton radioProcDate;
		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem goToChartToolStripMenuItem;
		private CheckBox checkShowExcludedCodes;
	}
}
