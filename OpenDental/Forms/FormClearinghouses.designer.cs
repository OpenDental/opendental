using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClearinghouses {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClearinghouses));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butEligibility = new OpenDental.UI.Button();
			this.butDefaultMedical = new OpenDental.UI.Button();
			this.butDefaultDental = new OpenDental.UI.Button();
			this.textReportCheckInterval = new System.Windows.Forms.TextBox();
			this.labelReportheckUnits = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelGuide = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.radioInterval = new System.Windows.Forms.RadioButton();
			this.radioTime = new System.Windows.Forms.RadioButton();
			this.textReportCheckTime = new OpenDental.ValidTime();
			this.groupRecieveSettings = new System.Windows.Forms.GroupBox();
			this.checkReceiveReportsService = new System.Windows.Forms.CheckBox();
			this.textReportComputerName = new System.Windows.Forms.TextBox();
			this.butThisComputer = new OpenDental.UI.Button();
			this.labelReportComputerName = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupRecieveSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.butEligibility);
			this.groupBox1.Controls.Add(this.butDefaultMedical);
			this.groupBox1.Controls.Add(this.butDefaultDental);
			this.groupBox1.Location = new System.Drawing.Point(6, 387);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(97, 112);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Set Default";
			// 
			// butEligibility
			// 
			this.butEligibility.Location = new System.Drawing.Point(15, 79);
			this.butEligibility.Name = "butEligibility";
			this.butEligibility.Size = new System.Drawing.Size(75, 24);
			this.butEligibility.TabIndex = 3;
			this.butEligibility.Text = "Eligibility";
			this.butEligibility.Click += new System.EventHandler(this.butEligibility_Click);
			// 
			// butDefaultMedical
			// 
			this.butDefaultMedical.Location = new System.Drawing.Point(15, 49);
			this.butDefaultMedical.Name = "butDefaultMedical";
			this.butDefaultMedical.Size = new System.Drawing.Size(75, 24);
			this.butDefaultMedical.TabIndex = 2;
			this.butDefaultMedical.Text = "Medical";
			this.butDefaultMedical.Click += new System.EventHandler(this.butDefaultMedical_Click);
			// 
			// butDefaultDental
			// 
			this.butDefaultDental.Location = new System.Drawing.Point(15, 19);
			this.butDefaultDental.Name = "butDefaultDental";
			this.butDefaultDental.Size = new System.Drawing.Size(75, 24);
			this.butDefaultDental.TabIndex = 1;
			this.butDefaultDental.Text = "Dental";
			this.butDefaultDental.Click += new System.EventHandler(this.butDefaultDental_Click);
			// 
			// textReportCheckInterval
			// 
			this.textReportCheckInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textReportCheckInterval.Location = new System.Drawing.Point(237, 66);
			this.textReportCheckInterval.MaxLength = 2147483647;
			this.textReportCheckInterval.Multiline = true;
			this.textReportCheckInterval.Name = "textReportCheckInterval";
			this.textReportCheckInterval.Size = new System.Drawing.Size(29, 20);
			this.textReportCheckInterval.TabIndex = 14;
			// 
			// labelReportheckUnits
			// 
			this.labelReportheckUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelReportheckUnits.Location = new System.Drawing.Point(273, 66);
			this.labelReportheckUnits.Name = "labelReportheckUnits";
			this.labelReportheckUnits.Size = new System.Drawing.Size(128, 20);
			this.labelReportheckUnits.TabIndex = 15;
			this.labelReportheckUnits.Text = "minutes (5 to 60)";
			this.labelReportheckUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(805, 385);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 8;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(805, 466);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.HqDescription = "Unassigned/Default";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(675, 17);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(210, 21);
			this.comboClinic.TabIndex = 20;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelGuide
			// 
			this.labelGuide.Location = new System.Drawing.Point(6, -1);
			this.labelGuide.Name = "labelGuide";
			this.labelGuide.Size = new System.Drawing.Size(595, 36);
			this.labelGuide.TabIndex = 22;
			this.labelGuide.Text = resources.GetString("labelGuide.Text");
			this.labelGuide.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(6, 39);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(879, 340);
			this.gridMain.TabIndex = 17;
			this.gridMain.Title = "Clearinghouses";
			this.gridMain.TranslationName = "TableClearinghouses";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// radioInterval
			// 
			this.radioInterval.Checked = true;
			this.radioInterval.Location = new System.Drawing.Point(102, 68);
			this.radioInterval.Name = "radioInterval";
			this.radioInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioInterval.Size = new System.Drawing.Size(134, 17);
			this.radioInterval.TabIndex = 25;
			this.radioInterval.TabStop = true;
			this.radioInterval.Text = "Receive at an interval";
			this.radioInterval.UseVisualStyleBackColor = true;
			this.radioInterval.CheckedChanged += new System.EventHandler(this.radioInterval_CheckedChanged);
			// 
			// radioTime
			// 
			this.radioTime.Location = new System.Drawing.Point(102, 90);
			this.radioTime.Name = "radioTime";
			this.radioTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioTime.Size = new System.Drawing.Size(134, 17);
			this.radioTime.TabIndex = 26;
			this.radioTime.Text = "Receive at a set time";
			this.radioTime.UseVisualStyleBackColor = true;
			// 
			// textReportCheckTime
			// 
			this.textReportCheckTime.Enabled = false;
			this.textReportCheckTime.Location = new System.Drawing.Point(237, 89);
			this.textReportCheckTime.Name = "textReportCheckTime";
			this.textReportCheckTime.Size = new System.Drawing.Size(119, 20);
			this.textReportCheckTime.TabIndex = 28;
			// 
			// groupRecieveSettings
			// 
			this.groupRecieveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupRecieveSettings.Controls.Add(this.checkReceiveReportsService);
			this.groupRecieveSettings.Controls.Add(this.textReportComputerName);
			this.groupRecieveSettings.Controls.Add(this.butThisComputer);
			this.groupRecieveSettings.Controls.Add(this.labelReportComputerName);
			this.groupRecieveSettings.Controls.Add(this.radioInterval);
			this.groupRecieveSettings.Controls.Add(this.textReportCheckTime);
			this.groupRecieveSettings.Controls.Add(this.radioTime);
			this.groupRecieveSettings.Controls.Add(this.textReportCheckInterval);
			this.groupRecieveSettings.Controls.Add(this.labelReportheckUnits);
			this.groupRecieveSettings.Location = new System.Drawing.Point(162, 385);
			this.groupRecieveSettings.Name = "groupRecieveSettings";
			this.groupRecieveSettings.Size = new System.Drawing.Size(571, 115);
			this.groupRecieveSettings.TabIndex = 29;
			this.groupRecieveSettings.TabStop = false;
			this.groupRecieveSettings.Text = "Automatic Report Settings";
			// 
			// checkReceiveReportsService
			// 
			this.checkReceiveReportsService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkReceiveReportsService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReceiveReportsService.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReceiveReportsService.Location = new System.Drawing.Point(22, 17);
			this.checkReceiveReportsService.Name = "checkReceiveReportsService";
			this.checkReceiveReportsService.Size = new System.Drawing.Size(227, 17);
			this.checkReceiveReportsService.TabIndex = 32;
			this.checkReceiveReportsService.TabStop = false;
			this.checkReceiveReportsService.Text = "Receive Reports by Service";
			this.checkReceiveReportsService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReceiveReportsService.CheckedChanged += new System.EventHandler(this.checkReceiveReportsService_CheckedChanged);
			// 
			// textReportComputerName
			// 
			this.textReportComputerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textReportComputerName.Location = new System.Drawing.Point(237, 39);
			this.textReportComputerName.MaxLength = 2147483647;
			this.textReportComputerName.Multiline = true;
			this.textReportComputerName.Name = "textReportComputerName";
			this.textReportComputerName.Size = new System.Drawing.Size(239, 20);
			this.textReportComputerName.TabIndex = 29;
			// 
			// butThisComputer
			// 
			this.butThisComputer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butThisComputer.Location = new System.Drawing.Point(479, 37);
			this.butThisComputer.Name = "butThisComputer";
			this.butThisComputer.Size = new System.Drawing.Size(86, 24);
			this.butThisComputer.TabIndex = 31;
			this.butThisComputer.Text = "This Computer";
			this.butThisComputer.Click += new System.EventHandler(this.butThisComputer_Click);
			// 
			// labelReportComputerName
			// 
			this.labelReportComputerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelReportComputerName.Location = new System.Drawing.Point(6, 39);
			this.labelReportComputerName.Name = "labelReportComputerName";
			this.labelReportComputerName.Size = new System.Drawing.Size(228, 20);
			this.labelReportComputerName.TabIndex = 30;
			this.labelReportComputerName.Text = "Computer To Receive Reports Automatically";
			this.labelReportComputerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormClearinghouses
			// 
			this.ClientSize = new System.Drawing.Size(891, 503);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupRecieveSettings);
			this.Controls.Add(this.labelGuide);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClearinghouses";
			this.ShowInTaskbar = false;
			this.Text = "Clearinghouses";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClearinghouses_Closing);
			this.Load += new System.EventHandler(this.FormClearinghouses_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupRecieveSettings.ResumeLayout(false);
			this.groupRecieveSettings.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private GroupBox groupBox1;
		private UI.Button butDefaultMedical;
		private UI.Button butDefaultDental;
		private TextBox textReportCheckInterval;
		private Label labelReportheckUnits;
		private UI.GridOD gridMain;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private Label labelGuide;
		private UI.Button butEligibility;
		private RadioButton radioInterval;
		private RadioButton radioTime;
		private ValidTime textReportCheckTime;
		private GroupBox groupRecieveSettings;
		private CheckBox checkReceiveReportsService;
		private TextBox textReportComputerName;
		private UI.Button butThisComputer;
		private Label labelReportComputerName;
	}
}
