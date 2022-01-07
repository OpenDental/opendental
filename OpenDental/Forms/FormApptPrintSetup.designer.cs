namespace OpenDental{
	partial class FormApptPrintSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptPrintSetup));
			this.butSave = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.groupBoxPrintRouting = new System.Windows.Forms.GroupBox();
			this.butSelectedView = new OpenDental.UI.Button();
			this.butSelectedDay = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboStop = new System.Windows.Forms.ComboBox();
			this.comboStart = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textFontSize = new OpenDental.ValidNum();
			this.labelStopTime = new System.Windows.Forms.Label();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.labelFontSize = new System.Windows.Forms.Label();
			this.textColumnsPerPage = new OpenDental.ValidNum();
			this.labelColumnsPerPage = new System.Windows.Forms.Label();
			this.groupBoxPrintRouting.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSave.Location = new System.Drawing.Point(20, 138);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 82;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(317, 138);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&Print";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(335, 302);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPreview.Location = new System.Drawing.Point(236, 138);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(75, 24);
			this.butPreview.TabIndex = 92;
			this.butPreview.Text = "&Preview";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// groupBoxPrintRouting
			// 
			this.groupBoxPrintRouting.Controls.Add(this.butSelectedView);
			this.groupBoxPrintRouting.Controls.Add(this.butSelectedDay);
			this.groupBoxPrintRouting.Location = new System.Drawing.Point(13, 205);
			this.groupBoxPrintRouting.Name = "groupBoxPrintRouting";
			this.groupBoxPrintRouting.Size = new System.Drawing.Size(169, 91);
			this.groupBoxPrintRouting.TabIndex = 93;
			this.groupBoxPrintRouting.TabStop = false;
			this.groupBoxPrintRouting.Text = "Print Routing Slips";
			// 
			// butSelectedView
			// 
			this.butSelectedView.Location = new System.Drawing.Point(21, 54);
			this.butSelectedView.Name = "butSelectedView";
			this.butSelectedView.Size = new System.Drawing.Size(120, 24);
			this.butSelectedView.TabIndex = 1;
			this.butSelectedView.Text = "Current View Only";
			this.butSelectedView.UseVisualStyleBackColor = true;
			this.butSelectedView.Click += new System.EventHandler(this.butCurrentView_Click);
			// 
			// butSelectedDay
			// 
			this.butSelectedDay.Location = new System.Drawing.Point(21, 24);
			this.butSelectedDay.Name = "butSelectedDay";
			this.butSelectedDay.Size = new System.Drawing.Size(120, 24);
			this.butSelectedDay.TabIndex = 0;
			this.butSelectedDay.Text = "All for Day";
			this.butSelectedDay.UseVisualStyleBackColor = true;
			this.butSelectedDay.Click += new System.EventHandler(this.butAllForDay_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboStop);
			this.groupBox1.Controls.Add(this.butPreview);
			this.groupBox1.Controls.Add(this.comboStart);
			this.groupBox1.Controls.Add(this.butSave);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.butOK);
			this.groupBox1.Controls.Add(this.textFontSize);
			this.groupBox1.Controls.Add(this.labelStopTime);
			this.groupBox1.Controls.Add(this.labelStartTime);
			this.groupBox1.Controls.Add(this.labelFontSize);
			this.groupBox1.Controls.Add(this.textColumnsPerPage);
			this.groupBox1.Controls.Add(this.labelColumnsPerPage);
			this.groupBox1.Location = new System.Drawing.Point(13, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(398, 171);
			this.groupBox1.TabIndex = 94;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Print Appointments";
			// 
			// comboStop
			// 
			this.comboStop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStop.FormattingEnabled = true;
			this.comboStop.Location = new System.Drawing.Point(153, 49);
			this.comboStop.Name = "comboStop";
			this.comboStop.Size = new System.Drawing.Size(143, 21);
			this.comboStop.TabIndex = 100;
			// 
			// comboStart
			// 
			this.comboStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStart.FormattingEnabled = true;
			this.comboStart.Location = new System.Drawing.Point(153, 25);
			this.comboStart.Name = "comboStart";
			this.comboStart.Size = new System.Drawing.Size(142, 21);
			this.comboStart.TabIndex = 99;
			// 
			// label3
			// 
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label3.Location = new System.Drawing.Point(209, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 15);
			this.label3.TabIndex = 98;
			this.label3.Text = "Between 2 and 50";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textFontSize
			// 
			this.textFontSize.Location = new System.Drawing.Point(153, 100);
			this.textFontSize.MaxVal = 50;
			this.textFontSize.MinVal = 2;
			this.textFontSize.Name = "textFontSize";
			this.textFontSize.Size = new System.Drawing.Size(50, 20);
			this.textFontSize.TabIndex = 97;
			// 
			// labelStopTime
			// 
			this.labelStopTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelStopTime.Location = new System.Drawing.Point(52, 51);
			this.labelStopTime.Name = "labelStopTime";
			this.labelStopTime.Size = new System.Drawing.Size(95, 15);
			this.labelStopTime.TabIndex = 96;
			this.labelStopTime.Text = "Stop time";
			this.labelStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStartTime
			// 
			this.labelStartTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelStartTime.Location = new System.Drawing.Point(52, 25);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(95, 15);
			this.labelStartTime.TabIndex = 95;
			this.labelStartTime.Text = "Start time";
			this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFontSize
			// 
			this.labelFontSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelFontSize.Location = new System.Drawing.Point(52, 103);
			this.labelFontSize.Name = "labelFontSize";
			this.labelFontSize.Size = new System.Drawing.Size(95, 15);
			this.labelFontSize.TabIndex = 94;
			this.labelFontSize.Text = "Font size";
			this.labelFontSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textColumnsPerPage
			// 
			this.textColumnsPerPage.Location = new System.Drawing.Point(153, 74);
			this.textColumnsPerPage.MaxVal = 255;
			this.textColumnsPerPage.MinVal = 0;
			this.textColumnsPerPage.Name = "textColumnsPerPage";
			this.textColumnsPerPage.ShowZero = false;
			this.textColumnsPerPage.Size = new System.Drawing.Size(50, 20);
			this.textColumnsPerPage.TabIndex = 93;
			// 
			// labelColumnsPerPage
			// 
			this.labelColumnsPerPage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelColumnsPerPage.Location = new System.Drawing.Point(19, 77);
			this.labelColumnsPerPage.Name = "labelColumnsPerPage";
			this.labelColumnsPerPage.Size = new System.Drawing.Size(128, 15);
			this.labelColumnsPerPage.TabIndex = 92;
			this.labelColumnsPerPage.Text = "Operatories per page";
			this.labelColumnsPerPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormApptPrintSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(423, 338);
			this.Controls.Add(this.groupBoxPrintRouting);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptPrintSetup";
			this.Text = "Form Appt Print Setup";
			this.Load += new System.EventHandler(this.FormApptPrintSetup_Load);
			this.groupBoxPrintRouting.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butSave;
		private UI.Button butPreview;
		private System.Windows.Forms.GroupBox groupBoxPrintRouting;
		private UI.Button butSelectedView;
		private UI.Button butSelectedDay;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboStop;
		private System.Windows.Forms.ComboBox comboStart;
		private System.Windows.Forms.Label label3;
		private ValidNum textFontSize;
		private System.Windows.Forms.Label labelStopTime;
		private System.Windows.Forms.Label labelStartTime;
		private System.Windows.Forms.Label labelFontSize;
		private ValidNum textColumnsPerPage;
		private System.Windows.Forms.Label labelColumnsPerPage;
	}
}