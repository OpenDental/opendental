using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSchedule {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchedule));
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxProvs = new OpenDental.UI.ListBox();
			this.checkWarnProvOverlap = new OpenDental.UI.CheckBox();
			this.checkPracticeNotes = new OpenDental.UI.CheckBox();
			this.listBoxEmps = new OpenDental.UI.ListBox();
			this.tabControl1 = new OpenDental.UI.TabControl();
			this.tabPageProv = new OpenDental.UI.TabPage();
			this.tabPageEmp = new OpenDental.UI.TabPage();
			this.checkClinicNotes = new OpenDental.UI.CheckBox();
			this.butClearWeek = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.textDateTo = new OpenDental.ValidDate();
			this.textDateFrom = new OpenDental.ValidDate();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkShowClinicSchedules = new OpenDental.UI.CheckBox();
			this.groupCopy = new OpenDental.UI.GroupBox();
			this.butCopyWeek = new OpenDental.UI.Button();
			this.butCopyDay = new OpenDental.UI.Button();
			this.textClipboard = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupPaste = new OpenDental.UI.GroupBox();
			this.butRepeat = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.checkReplace = new OpenDental.UI.CheckBox();
			this.textRepeat = new System.Windows.Forms.TextBox();
			this.butPaste = new OpenDental.UI.Button();
			this.groupBoxFilter = new OpenDental.UI.GroupBox();
			this.groupBoxWeekFilter = new OpenDental.UI.GroupBox();
			this.radioButtonWeekEnd = new System.Windows.Forms.RadioButton();
			this.radioButtonFullWeek = new System.Windows.Forms.RadioButton();
			this.radioButtonWorkWeek = new System.Windows.Forms.RadioButton();
			this.tabControl1.SuspendLayout();
			this.tabPageProv.SuspendLayout();
			this.tabPageEmp.SuspendLayout();
			this.groupCopy.SuspendLayout();
			this.groupPaste.SuspendLayout();
			this.groupBoxFilter.SuspendLayout();
			this.groupBoxWeekFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(94, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 15);
			this.label2.TabIndex = 9;
			this.label2.Text = "To Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 15);
			this.label1.TabIndex = 7;
			this.label1.Text = "From Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxProvs
			// 
			this.listBoxProvs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxProvs.Location = new System.Drawing.Point(3, 3);
			this.listBoxProvs.Name = "listBoxProvs";
			this.listBoxProvs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxProvs.Size = new System.Drawing.Size(164, 147);
			this.listBoxProvs.TabIndex = 23;
			this.listBoxProvs.SelectedIndexChanged += new System.EventHandler(this.listProv_SelectedIndexChanged);
			this.listBoxProvs.Click += new System.EventHandler(this.listProv_Click);
			this.listBoxProvs.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
			// 
			// checkWarnProvOverlap
			// 
			this.checkWarnProvOverlap.Checked = true;
			this.checkWarnProvOverlap.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkWarnProvOverlap.Location = new System.Drawing.Point(6, 14);
			this.checkWarnProvOverlap.Name = "checkWarnProvOverlap";
			this.checkWarnProvOverlap.Size = new System.Drawing.Size(176, 18);
			this.checkWarnProvOverlap.TabIndex = 33;
			this.checkWarnProvOverlap.Text = "Warn on Provider Overlap";
			// 
			// checkPracticeNotes
			// 
			this.checkPracticeNotes.Checked = true;
			this.checkPracticeNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkPracticeNotes.Location = new System.Drawing.Point(6, 87);
			this.checkPracticeNotes.Name = "checkPracticeNotes";
			this.checkPracticeNotes.Size = new System.Drawing.Size(198, 17);
			this.checkPracticeNotes.TabIndex = 28;
			this.checkPracticeNotes.Text = "Show Practice Holidays and Notes";
			this.checkPracticeNotes.Click += new System.EventHandler(this.checkPracticeNotes_Click);
			// 
			// listBoxEmps
			// 
			this.listBoxEmps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxEmps.Location = new System.Drawing.Point(3, 3);
			this.listBoxEmps.Name = "listBoxEmps";
			this.listBoxEmps.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxEmps.Size = new System.Drawing.Size(164, 147);
			this.listBoxEmps.TabIndex = 30;
			this.listBoxEmps.SelectedIndexChanged += new System.EventHandler(this.listEmp_SelectedIndexChanged);
			this.listBoxEmps.Click += new System.EventHandler(this.listEmp_Click);
			this.listBoxEmps.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageProv);
			this.tabControl1.Controls.Add(this.tabPageEmp);
			this.tabControl1.Location = new System.Drawing.Point(7, 178);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Size = new System.Drawing.Size(174, 180);
			this.tabControl1.TabIndex = 36;
			// 
			// tabPageProv
			// 
			this.tabPageProv.Controls.Add(this.listBoxProvs);
			this.tabPageProv.Location = new System.Drawing.Point(2, 21);
			this.tabPageProv.Name = "tabPageProv";
			this.tabPageProv.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageProv.Size = new System.Drawing.Size(170, 157);
			this.tabPageProv.TabIndex = 0;
			this.tabPageProv.Text = "Providers (0)";
			// 
			// tabPageEmp
			// 
			this.tabPageEmp.Controls.Add(this.listBoxEmps);
			this.tabPageEmp.Location = new System.Drawing.Point(2, 21);
			this.tabPageEmp.Name = "tabPageEmp";
			this.tabPageEmp.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageEmp.Size = new System.Drawing.Size(170, 157);
			this.tabPageEmp.TabIndex = 1;
			this.tabPageEmp.Text = "Employees (0)";
			// 
			// checkClinicNotes
			// 
			this.checkClinicNotes.Checked = true;
			this.checkClinicNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkClinicNotes.Location = new System.Drawing.Point(6, 107);
			this.checkClinicNotes.Name = "checkClinicNotes";
			this.checkClinicNotes.Size = new System.Drawing.Size(199, 17);
			this.checkClinicNotes.TabIndex = 37;
			this.checkClinicNotes.Text = "Show Clinic Holidays and Notes";
			this.checkClinicNotes.Click += new System.EventHandler(this.checkClinicNotes_Click);
			// 
			// butClearWeek
			// 
			this.butClearWeek.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butClearWeek.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClearWeek.Location = new System.Drawing.Point(14, 360);
			this.butClearWeek.Name = "butClearWeek";
			this.butClearWeek.Size = new System.Drawing.Size(119, 24);
			this.butClearWeek.TabIndex = 27;
			this.butClearWeek.Text = "Clear Week";
			this.butClearWeek.Click += new System.EventHandler(this.butClearWeek_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(921, 732);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(90, 24);
			this.butPrint.TabIndex = 26;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(96, 65);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(85, 20);
			this.textDateTo.TabIndex = 10;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(6, 65);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(85, 20);
			this.textDateFrom.TabIndex = 8;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(51, 21);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 11;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(223, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(761, 714);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Schedule";
			this.gridMain.TranslationName = "TableSchedule";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(2, 130);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(189, 21);
			this.comboClinic.TabIndex = 35;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// checkShowClinicSchedules
			// 
			this.checkShowClinicSchedules.Location = new System.Drawing.Point(6, 155);
			this.checkShowClinicSchedules.Name = "checkShowClinicSchedules";
			this.checkShowClinicSchedules.Size = new System.Drawing.Size(189, 17);
			this.checkShowClinicSchedules.TabIndex = 38;
			this.checkShowClinicSchedules.Text = "Limit to Ops in Clinic";
			this.checkShowClinicSchedules.CheckedChanged += new System.EventHandler(this.checkShowClinicSchedules_CheckedChanged);
			// 
			// groupCopy
			// 
			this.groupCopy.Controls.Add(this.butCopyWeek);
			this.groupCopy.Controls.Add(this.butCopyDay);
			this.groupCopy.Controls.Add(this.textClipboard);
			this.groupCopy.Controls.Add(this.label3);
			this.groupCopy.Location = new System.Drawing.Point(5, 492);
			this.groupCopy.Name = "groupCopy";
			this.groupCopy.Size = new System.Drawing.Size(206, 113);
			this.groupCopy.TabIndex = 39;
			this.groupCopy.Text = "Copy";
			// 
			// butCopyWeek
			// 
			this.butCopyWeek.Location = new System.Drawing.Point(6, 83);
			this.butCopyWeek.Name = "butCopyWeek";
			this.butCopyWeek.Size = new System.Drawing.Size(75, 24);
			this.butCopyWeek.TabIndex = 28;
			this.butCopyWeek.Text = "Copy Week";
			this.butCopyWeek.Click += new System.EventHandler(this.butCopyWeek_Click);
			// 
			// butCopyDay
			// 
			this.butCopyDay.Location = new System.Drawing.Point(6, 56);
			this.butCopyDay.Name = "butCopyDay";
			this.butCopyDay.Size = new System.Drawing.Size(75, 24);
			this.butCopyDay.TabIndex = 27;
			this.butCopyDay.Text = "Copy Day";
			this.butCopyDay.Click += new System.EventHandler(this.butCopyDay_Click);
			// 
			// textClipboard
			// 
			this.textClipboard.Location = new System.Drawing.Point(6, 33);
			this.textClipboard.Name = "textClipboard";
			this.textClipboard.ReadOnly = true;
			this.textClipboard.Size = new System.Drawing.Size(176, 20);
			this.textClipboard.TabIndex = 26;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(146, 14);
			this.label3.TabIndex = 8;
			this.label3.Text = "Clipboard Contents";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupPaste
			// 
			this.groupPaste.Controls.Add(this.butRepeat);
			this.groupPaste.Controls.Add(this.label4);
			this.groupPaste.Controls.Add(this.checkWarnProvOverlap);
			this.groupPaste.Controls.Add(this.checkReplace);
			this.groupPaste.Controls.Add(this.textRepeat);
			this.groupPaste.Controls.Add(this.butPaste);
			this.groupPaste.Location = new System.Drawing.Point(5, 611);
			this.groupPaste.Name = "groupPaste";
			this.groupPaste.Size = new System.Drawing.Size(206, 109);
			this.groupPaste.TabIndex = 40;
			this.groupPaste.Text = "Paste";
			// 
			// butRepeat
			// 
			this.butRepeat.Location = new System.Drawing.Point(6, 79);
			this.butRepeat.Name = "butRepeat";
			this.butRepeat.Size = new System.Drawing.Size(75, 24);
			this.butRepeat.TabIndex = 30;
			this.butRepeat.Text = "Repeat";
			this.butRepeat.Click += new System.EventHandler(this.butRepeat_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(70, 85);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 14);
			this.label4.TabIndex = 32;
			this.label4.Text = "#";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// checkReplace
			// 
			this.checkReplace.Checked = true;
			this.checkReplace.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkReplace.Location = new System.Drawing.Point(6, 34);
			this.checkReplace.Name = "checkReplace";
			this.checkReplace.Size = new System.Drawing.Size(146, 18);
			this.checkReplace.TabIndex = 31;
			this.checkReplace.Text = "Replace Existing";
			// 
			// textRepeat
			// 
			this.textRepeat.Location = new System.Drawing.Point(110, 82);
			this.textRepeat.Name = "textRepeat";
			this.textRepeat.Size = new System.Drawing.Size(39, 20);
			this.textRepeat.TabIndex = 31;
			this.textRepeat.Text = "1";
			// 
			// butPaste
			// 
			this.butPaste.Location = new System.Drawing.Point(6, 52);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(75, 24);
			this.butPaste.TabIndex = 29;
			this.butPaste.Text = "Paste";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// groupBoxFilter
			// 
			this.groupBoxFilter.Controls.Add(this.groupBoxWeekFilter);
			this.groupBoxFilter.Controls.Add(this.butRefresh);
			this.groupBoxFilter.Controls.Add(this.comboClinic);
			this.groupBoxFilter.Controls.Add(this.butClearWeek);
			this.groupBoxFilter.Controls.Add(this.tabControl1);
			this.groupBoxFilter.Controls.Add(this.checkPracticeNotes);
			this.groupBoxFilter.Controls.Add(this.textDateTo);
			this.groupBoxFilter.Controls.Add(this.label1);
			this.groupBoxFilter.Controls.Add(this.textDateFrom);
			this.groupBoxFilter.Controls.Add(this.checkShowClinicSchedules);
			this.groupBoxFilter.Controls.Add(this.checkClinicNotes);
			this.groupBoxFilter.Controls.Add(this.label2);
			this.groupBoxFilter.Location = new System.Drawing.Point(5, 8);
			this.groupBoxFilter.Name = "groupBoxFilter";
			this.groupBoxFilter.Size = new System.Drawing.Size(206, 478);
			this.groupBoxFilter.TabIndex = 49;
			this.groupBoxFilter.Text = "Filters";
			// 
			// groupBoxWeekFilter
			// 
			this.groupBoxWeekFilter.Controls.Add(this.radioButtonWeekEnd);
			this.groupBoxWeekFilter.Controls.Add(this.radioButtonFullWeek);
			this.groupBoxWeekFilter.Controls.Add(this.radioButtonWorkWeek);
			this.groupBoxWeekFilter.Location = new System.Drawing.Point(7, 390);
			this.groupBoxWeekFilter.Name = "groupBoxWeekFilter";
			this.groupBoxWeekFilter.Size = new System.Drawing.Size(177, 84);
			this.groupBoxWeekFilter.TabIndex = 50;
			this.groupBoxWeekFilter.Text = "Week Filter";
			// 
			// radioButtonWeekEnd
			// 
			this.radioButtonWeekEnd.AutoSize = true;
			this.radioButtonWeekEnd.Location = new System.Drawing.Point(6, 65);
			this.radioButtonWeekEnd.Name = "radioButtonWeekEnd";
			this.radioButtonWeekEnd.Size = new System.Drawing.Size(72, 17);
			this.radioButtonWeekEnd.TabIndex = 2;
			this.radioButtonWeekEnd.TabStop = true;
			this.radioButtonWeekEnd.Text = "Weekend";
			this.radioButtonWeekEnd.UseVisualStyleBackColor = true;
			this.radioButtonWeekEnd.CheckedChanged += new System.EventHandler(this.radioButtonWeekEnd_CheckedChanged);
			// 
			// radioButtonFullWeek
			// 
			this.radioButtonFullWeek.AutoSize = true;
			this.radioButtonFullWeek.Location = new System.Drawing.Point(6, 42);
			this.radioButtonFullWeek.Name = "radioButtonFullWeek";
			this.radioButtonFullWeek.Size = new System.Drawing.Size(73, 17);
			this.radioButtonFullWeek.TabIndex = 1;
			this.radioButtonFullWeek.TabStop = true;
			this.radioButtonFullWeek.Text = "Full Week";
			this.radioButtonFullWeek.UseVisualStyleBackColor = true;
			this.radioButtonFullWeek.CheckedChanged += new System.EventHandler(this.radioButtonFullWeek_CheckedChanged);
			// 
			// radioButtonWorkWeek
			// 
			this.radioButtonWorkWeek.AutoSize = true;
			this.radioButtonWorkWeek.Location = new System.Drawing.Point(6, 19);
			this.radioButtonWorkWeek.Name = "radioButtonWorkWeek";
			this.radioButtonWorkWeek.Size = new System.Drawing.Size(83, 17);
			this.radioButtonWorkWeek.TabIndex = 0;
			this.radioButtonWorkWeek.TabStop = true;
			this.radioButtonWorkWeek.Text = "Work Week";
			this.radioButtonWorkWeek.UseVisualStyleBackColor = true;
			this.radioButtonWorkWeek.CheckedChanged += new System.EventHandler(this.radioButtonWorkWeek_CheckedChanged);
			// 
			// FormSchedule
			// 
			this.ClientSize = new System.Drawing.Size(1023, 761);
			this.Controls.Add(this.groupBoxFilter);
			this.Controls.Add(this.groupCopy);
			this.Controls.Add(this.groupPaste);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "FormSchedule";
			this.ShowInTaskbar = false;
			this.Text = "Schedule";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSchedule_FormClosing);
			this.Load += new System.EventHandler(this.FormSchedule_Load);
			this.ResizeBegin += new System.EventHandler(this.FormSchedule_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.FormSchedule_ResizeEnd);
			this.Resize += new System.EventHandler(this.FormSchedule_Resize);
			this.tabControl1.ResumeLayout(false);
			this.tabPageProv.ResumeLayout(false);
			this.tabPageEmp.ResumeLayout(false);
			this.groupCopy.ResumeLayout(false);
			this.groupCopy.PerformLayout();
			this.groupPaste.ResumeLayout(false);
			this.groupPaste.PerformLayout();
			this.groupBoxFilter.ResumeLayout(false);
			this.groupBoxFilter.PerformLayout();
			this.groupBoxWeekFilter.ResumeLayout(false);
			this.groupBoxWeekFilter.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butRefresh;
		private ValidDate textDateTo;
		private Label label2;
		private ValidDate textDateFrom;
		private Label label1;
		private OpenDental.UI.ListBox listBoxProvs;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butClearWeek;
		private OpenDental.UI.CheckBox checkPracticeNotes;
		private OpenDental.UI.ListBox listBoxEmps;
		private OpenDental.UI.TabControl tabControl1;
		private OpenDental.UI.TabPage tabPageProv;
		private OpenDental.UI.TabPage tabPageEmp;
		private OpenDental.UI.CheckBox checkClinicNotes;
		private UI.ComboBoxClinicPicker comboClinic;
		private OpenDental.UI.CheckBox checkShowClinicSchedules;
		private OpenDental.UI.CheckBox checkWarnProvOverlap;
		private OpenDental.UI.GroupBox groupCopy;
		private UI.Button butCopyWeek;
		private UI.Button butCopyDay;
		private TextBox textClipboard;
		private Label label3;
		private OpenDental.UI.GroupBox groupPaste;
		private UI.Button butRepeat;
		private Label label4;
		private OpenDental.UI.CheckBox checkReplace;
		private TextBox textRepeat;
		private UI.Button butPaste;
		private OpenDental.UI.GroupBox groupBoxFilter;
		private OpenDental.UI.GroupBox groupBoxWeekFilter;
		private RadioButton radioButtonWeekEnd;
		private RadioButton radioButtonFullWeek;
		private RadioButton radioButtonWorkWeek;
	}
}
