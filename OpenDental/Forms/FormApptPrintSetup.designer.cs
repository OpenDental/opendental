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
			this.groupBoxPrintRouting = new OpenDental.UI.GroupBoxOD();
			this.butSelectedView = new OpenDental.UI.Button();
			this.butSelectedDay = new OpenDental.UI.Button();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.comboStop = new OpenDental.UI.ComboBoxOD();
			this.comboStart = new OpenDental.UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.textFontSize = new OpenDental.ValidNum();
			this.labelStopTime = new System.Windows.Forms.Label();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.labelFontSize = new System.Windows.Forms.Label();
			this.textColumnsPerPage = new OpenDental.ValidNum();
			this.labelColumnsPerPage = new System.Windows.Forms.Label();
			this.radioFullColor = new System.Windows.Forms.RadioButton();
			this.radioLessColor = new System.Windows.Forms.RadioButton();
			this.radioGrayscale = new System.Windows.Forms.RadioButton();
			this.groupBoxPrintRouting.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSave.Location = new System.Drawing.Point(20, 198);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 82;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(317, 198);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&Print";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(335, 341);
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
			this.butPreview.Location = new System.Drawing.Point(236, 198);
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
			this.groupBoxPrintRouting.Location = new System.Drawing.Point(13, 251);
			this.groupBoxPrintRouting.Name = "groupBoxPrintRouting";
			this.groupBoxPrintRouting.Size = new System.Drawing.Size(169, 91);
			this.groupBoxPrintRouting.TabIndex = 93;
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
			this.groupBox1.Controls.Add(this.radioGrayscale);
			this.groupBox1.Controls.Add(this.radioLessColor);
			this.groupBox1.Controls.Add(this.radioFullColor);
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
			this.groupBox1.Size = new System.Drawing.Size(398, 231);
			this.groupBox1.TabIndex = 94;
			this.groupBox1.Text = "Print Appointments";
			// 
			// comboStop
			// 
			this.comboStop.Location = new System.Drawing.Point(153, 49);
			this.comboStop.Name = "comboStop";
			this.comboStop.Size = new System.Drawing.Size(143, 21);
			this.comboStop.TabIndex = 100;
			// 
			// comboStart
			// 
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
			this.textFontSize.Location = new System.Drawing.Point(153, 96);
			this.textFontSize.MaxVal = 50;
			this.textFontSize.MinVal = 2;
			this.textFontSize.Name = "textFontSize";
			this.textFontSize.Size = new System.Drawing.Size(50, 20);
			this.textFontSize.TabIndex = 97;
			// 
			// labelStopTime
			// 
			this.labelStopTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelStopTime.Location = new System.Drawing.Point(56, 51);
			this.labelStopTime.Name = "labelStopTime";
			this.labelStopTime.Size = new System.Drawing.Size(95, 15);
			this.labelStopTime.TabIndex = 96;
			this.labelStopTime.Text = "Stop time";
			this.labelStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStartTime
			// 
			this.labelStartTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelStartTime.Location = new System.Drawing.Point(56, 27);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(95, 15);
			this.labelStartTime.TabIndex = 95;
			this.labelStartTime.Text = "Start time";
			this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFontSize
			// 
			this.labelFontSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelFontSize.Location = new System.Drawing.Point(56, 98);
			this.labelFontSize.Name = "labelFontSize";
			this.labelFontSize.Size = new System.Drawing.Size(95, 15);
			this.labelFontSize.TabIndex = 94;
			this.labelFontSize.Text = "Font size";
			this.labelFontSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textColumnsPerPage
			// 
			this.textColumnsPerPage.Location = new System.Drawing.Point(153, 73);
			this.textColumnsPerPage.Name = "textColumnsPerPage";
			this.textColumnsPerPage.ShowZero = false;
			this.textColumnsPerPage.Size = new System.Drawing.Size(50, 20);
			this.textColumnsPerPage.TabIndex = 93;
			// 
			// labelColumnsPerPage
			// 
			this.labelColumnsPerPage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelColumnsPerPage.Location = new System.Drawing.Point(23, 75);
			this.labelColumnsPerPage.Name = "labelColumnsPerPage";
			this.labelColumnsPerPage.Size = new System.Drawing.Size(128, 15);
			this.labelColumnsPerPage.TabIndex = 92;
			this.labelColumnsPerPage.Text = "Operatories per page";
			this.labelColumnsPerPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioFullColor
			// 
			this.radioFullColor.AutoSize = true;
			this.radioFullColor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioFullColor.Location = new System.Drawing.Point(103, 119);
			this.radioFullColor.Name = "radioFullColor";
			this.radioFullColor.Size = new System.Drawing.Size(67, 17);
			this.radioFullColor.TabIndex = 236;
			this.radioFullColor.TabStop = true;
			this.radioFullColor.Text = "Full color";
			this.radioFullColor.UseVisualStyleBackColor = true;
			// 
			// radioLessColor
			// 
			this.radioLessColor.AutoSize = true;
			this.radioLessColor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioLessColor.Location = new System.Drawing.Point(97, 142);
			this.radioLessColor.Name = "radioLessColor";
			this.radioLessColor.Size = new System.Drawing.Size(73, 17);
			this.radioLessColor.TabIndex = 237;
			this.radioLessColor.TabStop = true;
			this.radioLessColor.Text = "Less color";
			this.radioLessColor.UseVisualStyleBackColor = true;
			// 
			// radioGrayscale
			// 
			this.radioGrayscale.AutoSize = true;
			this.radioGrayscale.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioGrayscale.Location = new System.Drawing.Point(98, 165);
			this.radioGrayscale.Name = "radioGrayscale";
			this.radioGrayscale.Size = new System.Drawing.Size(72, 17);
			this.radioGrayscale.TabIndex = 238;
			this.radioGrayscale.TabStop = true;
			this.radioGrayscale.Text = "Grayscale";
			this.radioGrayscale.UseVisualStyleBackColor = true;
			// 
			// FormApptPrintSetup
			// 
			this.ClientSize = new System.Drawing.Size(423, 377);
			this.Controls.Add(this.groupBoxPrintRouting);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptPrintSetup";
			this.Text = "Appt Print Setup";
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
		private OpenDental.UI.GroupBoxOD groupBoxPrintRouting;
		private UI.Button butSelectedView;
		private UI.Button butSelectedDay;
		private OpenDental.UI.GroupBoxOD groupBox1;
		private OpenDental.UI.ComboBoxOD comboStop;
		private OpenDental.UI.ComboBoxOD comboStart;
		private System.Windows.Forms.Label label3;
		private ValidNum textFontSize;
		private System.Windows.Forms.Label labelStopTime;
		private System.Windows.Forms.Label labelStartTime;
		private System.Windows.Forms.Label labelFontSize;
		private ValidNum textColumnsPerPage;
		private System.Windows.Forms.Label labelColumnsPerPage;
		private System.Windows.Forms.RadioButton radioFullColor;
		private System.Windows.Forms.RadioButton radioGrayscale;
		private System.Windows.Forms.RadioButton radioLessColor;
	}
}