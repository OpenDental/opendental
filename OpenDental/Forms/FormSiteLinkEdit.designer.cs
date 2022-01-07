namespace OpenDental {
	using System;
	partial class FormSiteLinkEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSiteLinkEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.textOctet1 = new System.Windows.Forms.TextBox();
			this.textOctet3 = new System.Windows.Forms.TextBox();
			this.textOctet2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.comboTriageCoordinator = new OpenDental.UI.ComboBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.butChangeColorInner = new OpenDental.UI.Button();
			this.panelInnerColor = new System.Windows.Forms.Panel();
			this.butChangeColorOuter = new OpenDental.UI.Button();
			this.panelOuterColor = new System.Windows.Forms.Panel();
			this.textSite = new System.Windows.Forms.TextBox();
			this.labelOpsCountPreview = new OpenDental.MapAreaRoomControl();
			this.butChangeForeColor = new OpenDental.UI.Button();
			this.panelForeColor = new System.Windows.Forms.Panel();
			this.label10 = new System.Windows.Forms.Label();
			this.butChangeSiteColor = new OpenDental.UI.Button();
			this.panelSiteColor = new System.Windows.Forms.Panel();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gridConnections = new OpenDental.UI.GridOD();
			this.butDefault = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(557, 551);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(638, 551);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label8.Location = new System.Drawing.Point(235, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(71, 16);
			this.label8.TabIndex = 59;
			this.label8.Text = "Site";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOctet1
			// 
			this.textOctet1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textOctet1.Location = new System.Drawing.Point(309, 59);
			this.textOctet1.Name = "textOctet1";
			this.textOctet1.Size = new System.Drawing.Size(28, 20);
			this.textOctet1.TabIndex = 0;
			this.textOctet1.TextChanged += new System.EventHandler(this.textOctet1_TextChanged);
			// 
			// textOctet3
			// 
			this.textOctet3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textOctet3.Location = new System.Drawing.Point(385, 59);
			this.textOctet3.Name = "textOctet3";
			this.textOctet3.Size = new System.Drawing.Size(28, 20);
			this.textOctet3.TabIndex = 2;
			this.textOctet3.TextChanged += new System.EventHandler(this.textOctet3_TextChanged);
			// 
			// textOctet2
			// 
			this.textOctet2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textOctet2.Location = new System.Drawing.Point(347, 59);
			this.textOctet2.Name = "textOctet2";
			this.textOctet2.Size = new System.Drawing.Size(28, 20);
			this.textOctet2.TabIndex = 1;
			this.textOctet2.TextChanged += new System.EventHandler(this.textOctet2_TextChanged);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label1.Location = new System.Drawing.Point(12, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(294, 16);
			this.label1.TabIndex = 64;
			this.label1.Text = "Starting Default Gateway Octets";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label3.Location = new System.Drawing.Point(338, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(8, 16);
			this.label3.TabIndex = 66;
			this.label3.Text = ".";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label5.Location = new System.Drawing.Point(377, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(8, 16);
			this.label5.TabIndex = 68;
			this.label5.Text = ".";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label6.Location = new System.Drawing.Point(415, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(8, 16);
			this.label6.TabIndex = 69;
			this.label6.Text = ".";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(425, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 20);
			this.label2.TabIndex = 70;
			this.label2.Text = "X";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label7.Location = new System.Drawing.Point(153, 98);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(153, 16);
			this.label7.TabIndex = 71;
			this.label7.Text = "Triage Coordinator";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTriageCoordinator
			// 
			this.comboTriageCoordinator.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.comboTriageCoordinator.Location = new System.Drawing.Point(309, 97);
			this.comboTriageCoordinator.Name = "comboTriageCoordinator";
			this.comboTriageCoordinator.Size = new System.Drawing.Size(144, 21);
			this.comboTriageCoordinator.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label4.Location = new System.Drawing.Point(15, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 16);
			this.label4.TabIndex = 89;
			this.label4.Text = "Inner Color";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label9.Location = new System.Drawing.Point(15, 97);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(68, 16);
			this.label9.TabIndex = 90;
			this.label9.Text = "Outer Color";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butChangeColorInner
			// 
			this.butChangeColorInner.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butChangeColorInner.Location = new System.Drawing.Point(118, 59);
			this.butChangeColorInner.Name = "butChangeColorInner";
			this.butChangeColorInner.Size = new System.Drawing.Size(75, 24);
			this.butChangeColorInner.TabIndex = 4;
			this.butChangeColorInner.Text = "Change";
			this.butChangeColorInner.Click += new System.EventHandler(this.butChangeInnerColor_Click);
			// 
			// panelInnerColor
			// 
			this.panelInnerColor.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.panelInnerColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelInnerColor.Location = new System.Drawing.Point(86, 59);
			this.panelInnerColor.Name = "panelInnerColor";
			this.panelInnerColor.Size = new System.Drawing.Size(24, 24);
			this.panelInnerColor.TabIndex = 129;
			// 
			// butChangeColorOuter
			// 
			this.butChangeColorOuter.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butChangeColorOuter.Location = new System.Drawing.Point(118, 93);
			this.butChangeColorOuter.Name = "butChangeColorOuter";
			this.butChangeColorOuter.Size = new System.Drawing.Size(75, 24);
			this.butChangeColorOuter.TabIndex = 5;
			this.butChangeColorOuter.Text = "Change";
			this.butChangeColorOuter.Click += new System.EventHandler(this.butChangeOuterColor_Click);
			// 
			// panelOuterColor
			// 
			this.panelOuterColor.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.panelOuterColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelOuterColor.Location = new System.Drawing.Point(86, 93);
			this.panelOuterColor.Name = "panelOuterColor";
			this.panelOuterColor.Size = new System.Drawing.Size(24, 24);
			this.panelOuterColor.TabIndex = 131;
			// 
			// textSite
			// 
			this.textSite.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textSite.Location = new System.Drawing.Point(309, 21);
			this.textSite.Name = "textSite";
			this.textSite.ReadOnly = true;
			this.textSite.Size = new System.Drawing.Size(144, 20);
			this.textSite.TabIndex = 132;
			// 
			// labelOpsCountPreview
			// 
			this.labelOpsCountPreview.AllowDragging = false;
			this.labelOpsCountPreview.AllowEdit = false;
			this.labelOpsCountPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelOpsCountPreview.BorderThickness = 1;
			this.labelOpsCountPreview.ChatImage = null;
			this.labelOpsCountPreview.Elapsed = System.TimeSpan.Parse("00:00:00");
			this.labelOpsCountPreview.EmployeeName = null;
			this.labelOpsCountPreview.EmployeeNum = ((long)(0));
			this.labelOpsCountPreview.Empty = false;
			this.labelOpsCountPreview.Extension = null;
			this.labelOpsCountPreview.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOpsCountPreview.FontHeader = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOpsCountPreview.InnerColor = System.Drawing.Color.LightCyan;
			this.labelOpsCountPreview.Location = new System.Drawing.Point(86, 132);
			this.labelOpsCountPreview.Name = "labelOpsCountPreview";
			this.labelOpsCountPreview.OuterColor = System.Drawing.Color.Blue;
			this.labelOpsCountPreview.PhoneCur = null;
			this.labelOpsCountPreview.PhoneImage = null;
			this.labelOpsCountPreview.ProxImage = null;
			this.labelOpsCountPreview.Size = new System.Drawing.Size(107, 70);
			this.labelOpsCountPreview.Status = null;
			this.labelOpsCountPreview.TabIndex = 133;
			this.labelOpsCountPreview.Text = "5";
			this.labelOpsCountPreview.WebChatImage = null;
			// 
			// butChangeForeColor
			// 
			this.butChangeForeColor.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butChangeForeColor.Location = new System.Drawing.Point(118, 25);
			this.butChangeForeColor.Name = "butChangeForeColor";
			this.butChangeForeColor.Size = new System.Drawing.Size(75, 24);
			this.butChangeForeColor.TabIndex = 134;
			this.butChangeForeColor.Text = "Change";
			this.butChangeForeColor.Click += new System.EventHandler(this.butChangeForeColor_Click);
			// 
			// panelForeColor
			// 
			this.panelForeColor.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.panelForeColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelForeColor.Location = new System.Drawing.Point(86, 25);
			this.panelForeColor.Name = "panelForeColor";
			this.panelForeColor.Size = new System.Drawing.Size(24, 24);
			this.panelForeColor.TabIndex = 136;
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label10.Location = new System.Drawing.Point(15, 29);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(68, 16);
			this.label10.TabIndex = 135;
			this.label10.Text = "Fore Color";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butChangeSiteColor
			// 
			this.butChangeSiteColor.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butChangeSiteColor.Location = new System.Drawing.Point(341, 131);
			this.butChangeSiteColor.Name = "butChangeSiteColor";
			this.butChangeSiteColor.Size = new System.Drawing.Size(75, 24);
			this.butChangeSiteColor.TabIndex = 137;
			this.butChangeSiteColor.Text = "Change";
			this.butChangeSiteColor.Click += new System.EventHandler(this.butChangeSiteColor_Click);
			// 
			// panelSiteColor
			// 
			this.panelSiteColor.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.panelSiteColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelSiteColor.Location = new System.Drawing.Point(309, 131);
			this.panelSiteColor.Name = "panelSiteColor";
			this.panelSiteColor.Size = new System.Drawing.Size(24, 24);
			this.panelSiteColor.TabIndex = 139;
			// 
			// label11
			// 
			this.label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label11.Location = new System.Drawing.Point(153, 135);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(153, 16);
			this.label11.TabIndex = 138;
			this.label11.Text = "Site Color";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label12.Location = new System.Drawing.Point(422, 135);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(153, 16);
			this.label12.TabIndex = 140;
			this.label12.Text = "(escalation highlight color)";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.panelInnerColor);
			this.groupBox1.Controls.Add(this.butChangeColorInner);
			this.groupBox1.Controls.Add(this.butChangeForeColor);
			this.groupBox1.Controls.Add(this.panelOuterColor);
			this.groupBox1.Controls.Add(this.panelForeColor);
			this.groupBox1.Controls.Add(this.butChangeColorOuter);
			this.groupBox1.Controls.Add(this.labelOpsCountPreview);
			this.groupBox1.Location = new System.Drawing.Point(224, 168);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 216);
			this.groupBox1.TabIndex = 141;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Triage Counter Colors";
			// 
			// gridConnections
			// 
			this.gridConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridConnections.Location = new System.Drawing.Point(12, 390);
			this.gridConnections.Name = "gridConnections";
			this.gridConnections.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridConnections.Size = new System.Drawing.Size(701, 155);
			this.gridConnections.TabIndex = 142;
			this.gridConnections.Title = "Connection Overrides";
			this.gridConnections.TranslationName = "TableConnectionOverrides";
			// 
			// butDefault
			// 
			this.butDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butDefault.Location = new System.Drawing.Point(342, 551);
			this.butDefault.Name = "butDefault";
			this.butDefault.Size = new System.Drawing.Size(75, 24);
			this.butDefault.TabIndex = 143;
			this.butDefault.Text = "Default";
			this.butDefault.Click += new System.EventHandler(this.ButDefault_Click);
			// 
			// FormSiteLinkEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(725, 587);
			this.Controls.Add(this.butDefault);
			this.Controls.Add(this.gridConnections);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.butChangeSiteColor);
			this.Controls.Add(this.panelSiteColor);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textSite);
			this.Controls.Add(this.comboTriageCoordinator);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textOctet2);
			this.Controls.Add(this.textOctet3);
			this.Controls.Add(this.textOctet1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSiteLinkEdit";
			this.Text = "Site Link Edit";
			this.Load += new System.EventHandler(this.FormSiteLinkEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textOctet1;
		private System.Windows.Forms.TextBox textOctet3;
		private System.Windows.Forms.TextBox textOctet2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.ComboBoxOD comboTriageCoordinator;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label9;
		private UI.Button butChangeColorInner;
		private System.Windows.Forms.Panel panelInnerColor;
		private UI.Button butChangeColorOuter;
		private System.Windows.Forms.Panel panelOuterColor;
		private System.Windows.Forms.TextBox textSite;
		private MapAreaRoomControl labelOpsCountPreview;
		private UI.Button butChangeForeColor;
		private System.Windows.Forms.Panel panelForeColor;
		private System.Windows.Forms.Label label10;
		private UI.Button butChangeSiteColor;
		private System.Windows.Forms.Panel panelSiteColor;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.GridOD gridConnections;
		private UI.Button butDefault;
	}
}