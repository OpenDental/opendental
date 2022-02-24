namespace OpenDental{
	partial class FormProcLockTool {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcLockTool));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.textDate2 = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textDate1 = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(152, 89);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(243, 89);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDate2
			// 
			this.textDate2.Location = new System.Drawing.Point(200, 45);
			this.textDate2.Name = "textDate2";
			this.textDate2.Size = new System.Drawing.Size(80, 20);
			this.textDate2.TabIndex = 11;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(132, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 19);
			this.label2.TabIndex = 10;
			this.label2.Text = "through";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textDate1
			// 
			this.textDate1.Location = new System.Drawing.Point(51, 45);
			this.textDate1.Name = "textDate1";
			this.textDate1.Size = new System.Drawing.Size(80, 20);
			this.textDate1.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(326, 32);
			this.label1.TabIndex = 8;
			this.label1.Text = "Lock all Complete, EC, and EO procedures between the two dates.";
			// 
			// FormProcLockTool
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(330, 125);
			this.Controls.Add(this.textDate2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDate1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcLockTool";
			this.Text = "Procedure Lock Tool";
			this.Load += new System.EventHandler(this.FormProcLockTool_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private ValidDate textDate2;
		private System.Windows.Forms.Label label2;
		private ValidDate textDate1;
		private System.Windows.Forms.Label label1;
	}
}