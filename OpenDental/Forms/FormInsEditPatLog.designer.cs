namespace OpenDental{
	partial class FormInsEditPatLog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsEditPatLog));
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(928, 570);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasDropDowns = true;
			this.gridMain.Location = new System.Drawing.Point(8, 34);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(995, 524);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Logs";
			this.gridMain.TranslationName = "TableLogs";
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(12, 4);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.dateRangePicker.TabIndex = 5;
			// 
			// FormInsEditPatLog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1011, 602);
			this.Controls.Add(this.dateRangePicker);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsEditPatLog";
			this.Text = "Insurance Patient Plan Edit Log";
			this.Load += new System.EventHandler(this.FormInsEditLogs_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private UI.ODDateRangePicker dateRangePicker;
	}
}