namespace OpenDental{
	partial class FormApptBreakRequired {
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components=null;

		///<summary>Clean up any resources being used.</summary>
		///<param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		///<summary>Required method for Designer support - do not modify the contents of this method with the code editor.</summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptBreakRequired));
			this.butMissed = new OpenDental.UI.Button();
			this.butCancelled = new OpenDental.UI.Button();
			this.labelBreakAppt = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butMissed
			// 
			this.butMissed.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butMissed.Location = new System.Drawing.Point(45, 51);
			this.butMissed.Name = "butMissed";
			this.butMissed.Size = new System.Drawing.Size(91, 24);
			this.butMissed.TabIndex = 3;
			this.butMissed.Text = "&Missed";
			this.butMissed.Click += new System.EventHandler(this.butMissed_Click);
			// 
			// butCancelled
			// 
			this.butCancelled.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butCancelled.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancelled.Location = new System.Drawing.Point(45, 81);
			this.butCancelled.Name = "butCancelled";
			this.butCancelled.Size = new System.Drawing.Size(91, 24);
			this.butCancelled.TabIndex = 2;
			this.butCancelled.Text = "&Cancelled";
			this.butCancelled.Click += new System.EventHandler(this.butCancelled_Click);
			// 
			// labelBreakAppt
			// 
			this.labelBreakAppt.AllowDrop = true;
			this.labelBreakAppt.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelBreakAppt.Location = new System.Drawing.Point(12, 9);
			this.labelBreakAppt.Name = "labelBreakAppt";
			this.labelBreakAppt.Size = new System.Drawing.Size(310, 39);
			this.labelBreakAppt.TabIndex = 4;
			this.labelBreakAppt.Text = "Before an appointment can be moved or deleted it must first be broken. Please spe" +
    "cify whether it was missed or cancelled.";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(236, 114);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormApptBreakRequired
			// 
			this.ClientSize = new System.Drawing.Size(323, 150);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelBreakAppt);
			this.Controls.Add(this.butMissed);
			this.Controls.Add(this.butCancelled);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptBreakRequired";
			this.Text = "Break Appointment";
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butMissed;
		private OpenDental.UI.Button butCancelled;
		private System.Windows.Forms.Label labelBreakAppt;
		private UI.Button butCancel;
	}
}