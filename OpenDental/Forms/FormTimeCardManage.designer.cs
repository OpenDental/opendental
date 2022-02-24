namespace OpenDental{
	partial class FormTimeCardManage {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCardManage));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textDatePaycheck = new System.Windows.Forms.TextBox();
			this.textDateStop = new System.Windows.Forms.TextBox();
			this.textDateStart = new System.Windows.Forms.TextBox();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butDaily = new OpenDental.UI.Button();
			this.butCompute = new OpenDental.UI.Button();
			this.butPrintAll = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butClearAuto = new OpenDental.UI.Button();
			this.butClearManual = new OpenDental.UI.Button();
			this.butPrintSelected = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butTimeCardBenefits = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textDatePaycheck);
			this.groupBox1.Controls.Add(this.textDateStop);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.butRight);
			this.groupBox1.Controls.Add(this.butLeft);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 33);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(587, 51);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pay Period";
			// 
			// textDatePaycheck
			// 
			this.textDatePaycheck.Location = new System.Drawing.Point(473, 19);
			this.textDatePaycheck.Name = "textDatePaycheck";
			this.textDatePaycheck.ReadOnly = true;
			this.textDatePaycheck.Size = new System.Drawing.Size(100, 20);
			this.textDatePaycheck.TabIndex = 14;
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(244, 29);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.ReadOnly = true;
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 13;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(244, 8);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.ReadOnly = true;
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 12;
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(63, 18);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(39, 24);
			this.butRight.TabIndex = 11;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(13, 18);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(39, 24);
			this.butLeft.TabIndex = 10;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(354, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Paycheck Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(146, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 18);
			this.label2.TabIndex = 6;
			this.label2.Text = "Start Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(143, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "End Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 90);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1025, 541);
			this.gridMain.TabIndex = 16;
			this.gridMain.Title = "Employee Time Cards";
			this.gridMain.TranslationName = "TableTimeCard";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butDaily
			// 
			this.butDaily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDaily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDaily.Location = new System.Drawing.Point(6, 18);
			this.butDaily.Name = "butDaily";
			this.butDaily.Size = new System.Drawing.Size(69, 24);
			this.butDaily.TabIndex = 119;
			this.butDaily.Text = "Daily";
			this.butDaily.Click += new System.EventHandler(this.butDaily_Click);
			// 
			// butCompute
			// 
			this.butCompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCompute.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCompute.Location = new System.Drawing.Point(81, 18);
			this.butCompute.Name = "butCompute";
			this.butCompute.Size = new System.Drawing.Size(72, 24);
			this.butCompute.TabIndex = 118;
			this.butCompute.Text = "Weekly";
			this.butCompute.Click += new System.EventHandler(this.butWeekly_Click);
			// 
			// butPrintAll
			// 
			this.butPrintAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintAll.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrintAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintAll.Location = new System.Drawing.Point(6, 18);
			this.butPrintAll.Name = "butPrintAll";
			this.butPrintAll.Size = new System.Drawing.Size(87, 24);
			this.butPrintAll.TabIndex = 116;
			this.butPrintAll.Text = "&Print All";
			this.butPrintAll.Click += new System.EventHandler(this.butPrintAll_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(962, 655);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butClearAuto
			// 
			this.butClearAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearAuto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClearAuto.Location = new System.Drawing.Point(920, 60);
			this.butClearAuto.Name = "butClearAuto";
			this.butClearAuto.Size = new System.Drawing.Size(117, 24);
			this.butClearAuto.TabIndex = 122;
			this.butClearAuto.Text = "Clear Auto Adjusts";
			this.butClearAuto.Click += new System.EventHandler(this.butClearAuto_Click);
			// 
			// butClearManual
			// 
			this.butClearManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClearManual.Location = new System.Drawing.Point(920, 33);
			this.butClearManual.Name = "butClearManual";
			this.butClearManual.Size = new System.Drawing.Size(117, 24);
			this.butClearManual.TabIndex = 123;
			this.butClearManual.Text = "Clear Manual Adjusts";
			this.butClearManual.Click += new System.EventHandler(this.butClearManual_Click);
			// 
			// butPrintSelected
			// 
			this.butPrintSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintSelected.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrintSelected.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintSelected.Location = new System.Drawing.Point(99, 18);
			this.butPrintSelected.Name = "butPrintSelected";
			this.butPrintSelected.Size = new System.Drawing.Size(109, 24);
			this.butPrintSelected.TabIndex = 124;
			this.butPrintSelected.Text = "Print Selected";
			this.butPrintSelected.Click += new System.EventHandler(this.butPrintSelected_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.butDaily);
			this.groupBox2.Controls.Add(this.butCompute);
			this.groupBox2.Location = new System.Drawing.Point(12, 637);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(160, 48);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Calculations";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox3.Controls.Add(this.butPrintAll);
			this.groupBox3.Controls.Add(this.butPrintSelected);
			this.groupBox3.Location = new System.Drawing.Point(178, 637);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(215, 48);
			this.groupBox3.TabIndex = 125;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Time Cards";
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(641, 61);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(217, 21);
			this.comboClinic.TabIndex = 128;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// butTimeCardBenefits
			// 
			this.butTimeCardBenefits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTimeCardBenefits.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butTimeCardBenefits.Location = new System.Drawing.Point(860, 655);
			this.butTimeCardBenefits.Name = "butTimeCardBenefits";
			this.butTimeCardBenefits.Size = new System.Drawing.Size(70, 24);
			this.butTimeCardBenefits.TabIndex = 119;
			this.butTimeCardBenefits.Text = "Benefits";
			this.butTimeCardBenefits.Visible = false;
			this.butTimeCardBenefits.Click += new System.EventHandler(this.butTimeCardBenefits_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1049, 24);
			this.menuMain.TabIndex = 129;
			// 
			// FormTimeCardManage
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1049, 696);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butTimeCardBenefits);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butClearManual);
			this.Controls.Add(this.butClearAuto);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTimeCardManage";
			this.Text = "Manage Time Cards";
			this.Load += new System.EventHandler(this.FormTimeCardManage_Load);
			this.Shown += new System.EventHandler(this.FormTimeCardManage_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textDatePaycheck;
		private System.Windows.Forms.TextBox textDateStop;
		private System.Windows.Forms.TextBox textDateStart;
		private UI.Button butRight;
		private UI.Button butLeft;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.GridOD gridMain;
		private UI.Button butPrintAll;
		private UI.Button butDaily;
		private UI.Button butCompute;
		private UI.Button butClearAuto;
		private UI.Button butClearManual;
		private UI.Button butPrintSelected;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.Button butTimeCardBenefits;
		private UI.MenuOD menuMain;
	}
}