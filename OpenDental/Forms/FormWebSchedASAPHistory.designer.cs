namespace OpenDental{
	partial class FormWebSchedASAPHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedASAPHistory));
			this.butClose = new OpenDental.UI.Button();
			this.gridHistory = new OpenDental.UI.GridOD();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.textFilled = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textTextsSent = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(748, 460);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridHistory
			// 
			this.gridHistory.AllowSortingByColumn = true;
			this.gridHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridHistory.HScrollVisible = true;
			this.gridHistory.Location = new System.Drawing.Point(12, 46);
			this.gridHistory.Name = "gridHistory";
			this.gridHistory.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridHistory.Size = new System.Drawing.Size(811, 400);
			this.gridHistory.TabIndex = 9;
			this.gridHistory.Title = "Web Sched ASAP Messages";
			this.gridHistory.TranslationName = "";
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.SystemColors.Control;
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.Location = new System.Drawing.Point(13, 11);
			this.datePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(453, 22);
			this.datePicker.TabIndex = 10;
			this.datePicker.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.datePicker_CalendarClosed);
			this.datePicker.Leave += new System.EventHandler(this.datePicker_Leave);
			// 
			// textFilled
			// 
			this.textFilled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFilled.Location = new System.Drawing.Point(155, 456);
			this.textFilled.Name = "textFilled";
			this.textFilled.ReadOnly = true;
			this.textFilled.Size = new System.Drawing.Size(50, 20);
			this.textFilled.TabIndex = 42;
			this.textFilled.TabStop = false;
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label20.Location = new System.Drawing.Point(10, 459);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(143, 14);
			this.label20.TabIndex = 43;
			this.label20.Text = "Appointment slots filled";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.SelectionModeMulti = true;
			this.comboClinic.Location = new System.Drawing.Point(625, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(198, 21);
			this.comboClinic.TabIndex = 44;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// textTextsSent
			// 
			this.textTextsSent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTextsSent.Location = new System.Drawing.Point(291, 456);
			this.textTextsSent.Name = "textTextsSent";
			this.textTextsSent.ReadOnly = true;
			this.textTextsSent.Size = new System.Drawing.Size(50, 20);
			this.textTextsSent.TabIndex = 46;
			this.textTextsSent.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(206, 459);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 14);
			this.label1.TabIndex = 47;
			this.label1.Text = "Texts sent";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormWebSchedASAPHistory
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(835, 496);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.textTextsSent);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.textFilled);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.datePicker);
			this.Controls.Add(this.gridHistory);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedASAPHistory";
			this.Text = "Web Sched ASAP History";
			this.Load += new System.EventHandler(this.FormWebSchedASAPHistory_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridHistory;
		private UI.ODDateRangePicker datePicker;
		private System.Windows.Forms.TextBox textFilled;
		private System.Windows.Forms.Label label20;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.TextBox textTextsSent;
		private System.Windows.Forms.Label label1;
	}
}