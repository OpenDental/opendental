using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormConfirmList {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfirmList));
			this.butClose = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkGroupFamilies = new System.Windows.Forms.CheckBox();
			this.labelConfirmList = new System.Windows.Forms.Label();
			this.labelShowStatus = new System.Windows.Forms.Label();
			this.comboViewStatus = new OpenDental.UI.ComboBoxOD();
			this.comboShowRecall = new System.Windows.Forms.ComboBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textDateTo = new OpenDental.ValidDate();
			this.textDateFrom = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butReport = new OpenDental.UI.Button();
			this.butLabels = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.butPostcards = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.menuRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemSelectPatient = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSeeChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSendToPinboard = new System.Windows.Forms.ToolStripMenuItem();
			this.butPrint = new OpenDental.UI.Button();
			this.butEmail = new OpenDental.UI.Button();
			this.butText = new OpenDental.UI.Button();
			this.comboEmailFrom = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.menuRightClick.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1019, 651);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkGroupFamilies);
			this.groupBox1.Controls.Add(this.labelConfirmList);
			this.groupBox1.Controls.Add(this.labelShowStatus);
			this.groupBox1.Controls.Add(this.comboViewStatus);
			this.groupBox1.Controls.Add(this.comboShowRecall);
			this.groupBox1.Controls.Add(this.comboClinic);
			this.groupBox1.Controls.Add(this.comboProv);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textDateTo);
			this.groupBox1.Controls.Add(this.textDateFrom);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(5, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(737, 63);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// checkGroupFamilies
			// 
			this.checkGroupFamilies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGroupFamilies.Location = new System.Drawing.Point(634, 15);
			this.checkGroupFamilies.Name = "checkGroupFamilies";
			this.checkGroupFamilies.Size = new System.Drawing.Size(97, 20);
			this.checkGroupFamilies.TabIndex = 30;
			this.checkGroupFamilies.Text = "Group Families";
			this.checkGroupFamilies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.UseVisualStyleBackColor = true;
			this.checkGroupFamilies.Click += new System.EventHandler(this.CheckGroupFamilies_Click);
			// 
			// labelConfirmList
			// 
			this.labelConfirmList.Location = new System.Drawing.Point(6, 38);
			this.labelConfirmList.Name = "labelConfirmList";
			this.labelConfirmList.Size = new System.Drawing.Size(71, 14);
			this.labelConfirmList.TabIndex = 29;
			this.labelConfirmList.Text = "Confirm List";
			this.labelConfirmList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelShowStatus
			// 
			this.labelShowStatus.Location = new System.Drawing.Point(6, 16);
			this.labelShowStatus.Name = "labelShowStatus";
			this.labelShowStatus.Size = new System.Drawing.Size(71, 14);
			this.labelShowStatus.TabIndex = 28;
			this.labelShowStatus.Text = "Status";
			this.labelShowStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboViewStatus
			// 
			this.comboViewStatus.Location = new System.Drawing.Point(81, 12);
			this.comboViewStatus.Name = "comboViewStatus";
			this.comboViewStatus.Size = new System.Drawing.Size(121, 21);
			this.comboViewStatus.TabIndex = 27;
			// 
			// comboShowRecall
			// 
			this.comboShowRecall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboShowRecall.FormattingEnabled = true;
			this.comboShowRecall.Items.AddRange(new object[] {
            "All",
            "Recall Only",
            "Exclude Recall",
            "Hygiene Prescheduled"});
			this.comboShowRecall.Location = new System.Drawing.Point(81, 36);
			this.comboShowRecall.Name = "comboShowRecall";
			this.comboShowRecall.Size = new System.Drawing.Size(121, 21);
			this.comboShowRecall.TabIndex = 26;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(410, 37);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(218, 21);
			this.comboClinic.TabIndex = 25;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(447, 14);
			this.comboProv.MaxDropDownItems = 40;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(181, 21);
			this.comboProv.TabIndex = 23;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(380, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 14);
			this.label4.TabIndex = 22;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(280, 37);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(94, 20);
			this.textDateTo.TabIndex = 14;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(280, 14);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(94, 20);
			this.textDateFrom.TabIndex = 13;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(208, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 14);
			this.label2.TabIndex = 12;
			this.label2.Text = "To Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(208, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 14);
			this.label1.TabIndex = 11;
			this.label1.Text = "From Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butReport
			// 
			this.butReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butReport.Location = new System.Drawing.Point(745, 651);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(87, 24);
			this.butReport.TabIndex = 13;
			this.butReport.Text = "R&un Report";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// butLabels
			// 
			this.butLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabels.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabels.Location = new System.Drawing.Point(129, 651);
			this.butLabels.Name = "butLabels";
			this.butLabels.Size = new System.Drawing.Size(102, 24);
			this.butLabels.TabIndex = 14;
			this.butLabels.Text = "Label Preview";
			this.butLabels.Click += new System.EventHandler(this.butLabels_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboStatus);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(745, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(143, 63);
			this.groupBox3.TabIndex = 15;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Set Status";
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(7, 22);
			this.comboStatus.MaxDropDownItems = 40;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(128, 21);
			this.comboStatus.TabIndex = 15;
			this.comboStatus.SelectedIndexChanged += new System.EventHandler(this.comboStatus_SelectedIndexChanged);
			this.comboStatus.SelectionChangeCommitted += new System.EventHandler(this.comboStatus_SelectionChangeCommitted);
			// 
			// butPostcards
			// 
			this.butPostcards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPostcards.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPostcards.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPostcards.Location = new System.Drawing.Point(4, 651);
			this.butPostcards.Name = "butPostcards";
			this.butPostcards.Size = new System.Drawing.Size(119, 24);
			this.butPostcards.TabIndex = 16;
			this.butPostcards.Text = "Postcard Preview";
			this.butPostcards.Click += new System.EventHandler(this.butPostcards_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.menuRightClick;
			this.gridMain.Location = new System.Drawing.Point(4, 69);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1090, 577);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Confirmation List";
			this.gridMain.TranslationName = "TableConfirmList";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridMain.Click += new System.EventHandler(this.grid_Click);
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// menuRightClick
			// 
			this.menuRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectPatient,
            this.toolStripMenuItemSeeChart,
            this.toolStripMenuItemSendToPinboard});
			this.menuRightClick.Name = "_menuRightClick";
			this.menuRightClick.Size = new System.Drawing.Size(166, 70);
			// 
			// toolStripMenuItemSelectPatient
			// 
			this.toolStripMenuItemSelectPatient.Name = "toolStripMenuItemSelectPatient";
			this.toolStripMenuItemSelectPatient.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSelectPatient.Text = "Select Patient";
			this.toolStripMenuItemSelectPatient.Click += new System.EventHandler(this.menuRight_click);
			// 
			// toolStripMenuItemSeeChart
			// 
			this.toolStripMenuItemSeeChart.Name = "toolStripMenuItemSeeChart";
			this.toolStripMenuItemSeeChart.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSeeChart.Text = "See Chart";
			this.toolStripMenuItemSeeChart.Click += new System.EventHandler(this.menuRight_click);
			// 
			// toolStripMenuItemSendToPinboard
			// 
			this.toolStripMenuItemSendToPinboard.Name = "toolStripMenuItemSendToPinboard";
			this.toolStripMenuItemSendToPinboard.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSendToPinboard.Text = "Send to Pinboard";
			this.toolStripMenuItemSendToPinboard.Click += new System.EventHandler(this.menuRight_click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(838, 651);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 20;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butEmail
			// 
			this.butEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEmail.Icon = OpenDental.UI.EnumIcons.Email;
			this.butEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmail.Location = new System.Drawing.Point(237, 651);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(91, 24);
			this.butEmail.TabIndex = 61;
			this.butEmail.Text = "E-Mail";
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butText
			// 
			this.butText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butText.Icon = OpenDental.UI.EnumIcons.Text;
			this.butText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butText.Location = new System.Drawing.Point(333, 651);
			this.butText.Name = "butText";
			this.butText.Size = new System.Drawing.Size(79, 24);
			this.butText.TabIndex = 61;
			this.butText.Text = "Text";
			this.butText.Click += new System.EventHandler(this.butText_Click);
			// 
			// comboEmailFrom
			// 
			this.comboEmailFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFrom.Location = new System.Drawing.Point(6, 22);
			this.comboEmailFrom.MaxDropDownItems = 40;
			this.comboEmailFrom.Name = "comboEmailFrom";
			this.comboEmailFrom.Size = new System.Drawing.Size(191, 21);
			this.comboEmailFrom.TabIndex = 63;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboEmailFrom);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(891, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(203, 63);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Email From";
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(664, 651);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 62;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// FormConfirmList
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1099, 688);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butText);
			this.Controls.Add(this.butEmail);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butPostcards);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.butLabels);
			this.Controls.Add(this.butReport);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormConfirmList";
			this.Text = "Confirmation List";
			this.Load += new System.EventHandler(this.FormConfirmList_Load);
			this.Shown += new System.EventHandler(this.FormConfirmList_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.menuRightClick.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butReport;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox comboStatus;
		private OpenDental.UI.Button butLabels;
		private OpenDental.UI.Button butPostcards;
		private OpenDental.ValidDate textDateFrom;
		private OpenDental.ValidDate textDateTo;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butPrint;
		private ComboBox comboProv;
		private Label label4;
		private OpenDental.UI.Button butEmail;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private ComboBox comboShowRecall;
		private UI.Button butText;
		private ContextMenuStrip menuRightClick;
		private ComboBox comboEmailFrom;
		private GroupBox groupBox2;
		private ToolStripMenuItem toolStripMenuItemSelectPatient;
		private ToolStripMenuItem toolStripMenuItemSeeChart;
		private ToolStripMenuItem toolStripMenuItemSendToPinboard;
		private Label labelConfirmList;
		private Label labelShowStatus;
		private OpenDental.UI.ComboBoxOD comboViewStatus;
		private Label label1;
		private UI.Button butRefresh;
		private CheckBox checkGroupFamilies;
	}
}
