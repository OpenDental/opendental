namespace OpenDental{
	partial class FormClaimProcBlueBookLog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimProcBlueBookLog));
			this.butCancel = new OpenDental.UI.Button();
			this.gridInsBlueBookLog = new OpenDental.UI.GridOD();
			this.labelBlueBookOff = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(977, 588);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.ButCancel_Click);
			// 
			// gridInsBlueBookLog
			// 
			this.gridInsBlueBookLog.AllowSortingByColumn = true;
			this.gridInsBlueBookLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridInsBlueBookLog.HasMultilineHeaders = true;
			this.gridInsBlueBookLog.Location = new System.Drawing.Point(12, 12);
			this.gridInsBlueBookLog.MaximumSize = new System.Drawing.Size(1246, 735);
			this.gridInsBlueBookLog.Name = "gridInsBlueBookLog";
			this.gridInsBlueBookLog.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridInsBlueBookLog.Size = new System.Drawing.Size(1040, 564);
			this.gridInsBlueBookLog.TabIndex = 267;
			this.gridInsBlueBookLog.Title = "Insurance Blue Book Log History";
			// 
			// labelBlueBookOff
			// 
			this.labelBlueBookOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelBlueBookOff.ForeColor = System.Drawing.Color.Red;
			this.labelBlueBookOff.Location = new System.Drawing.Point(12, 588);
			this.labelBlueBookOff.Name = "labelBlueBookOff";
			this.labelBlueBookOff.Size = new System.Drawing.Size(959, 24);
			this.labelBlueBookOff.TabIndex = 268;
			this.labelBlueBookOff.Text = "The Blue Book feature is off. The current estimate will not be based off any " +
    "of the logs above.";
			this.labelBlueBookOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormClaimProcBlueBookLog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1064, 624);
			this.Controls.Add(this.labelBlueBookOff);
			this.Controls.Add(this.gridInsBlueBookLog);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimProcBlueBookLog";
			this.Text = "Insurance Blue Book Log";
			this.Load += new System.EventHandler(this.FormClaimProcBlueBookLog_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridInsBlueBookLog;
		private System.Windows.Forms.Label labelBlueBookOff;
	}
}