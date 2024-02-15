namespace OpenDental{
	partial class FormPhoneGraphDateEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneGraphDateEdit));
			this.butEditSchedule = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.textSuperPeak = new System.Windows.Forms.TextBox();
			this.textPeak = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listSupers = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butEditSchedule
			// 
			this.butEditSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditSchedule.Location = new System.Drawing.Point(12, 616);
			this.butEditSchedule.Name = "butEditSchedule";
			this.butEditSchedule.Size = new System.Drawing.Size(0, 0);
			this.butEditSchedule.TabIndex = 50;
			this.butEditSchedule.Text = "&Edit Schedule";
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(386, 616);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 51;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textSuperPeak
			// 
			this.textSuperPeak.Location = new System.Drawing.Point(153, 179);
			this.textSuperPeak.Name = "textSuperPeak";
			this.textSuperPeak.Size = new System.Drawing.Size(100, 20);
			this.textSuperPeak.TabIndex = 3;
			// 
			// textPeak
			// 
			this.textPeak.Location = new System.Drawing.Point(153, 150);
			this.textPeak.Name = "textPeak";
			this.textPeak.Size = new System.Drawing.Size(100, 20);
			this.textPeak.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 173);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(143, 32);
			this.label2.TabIndex = 1;
			this.label2.Text = "Super Peak\r\n(max number emps to show)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 151);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Peak of red line";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listSupers
			// 
			this.listSupers.IntegralHeight = false;
			this.listSupers.Location = new System.Drawing.Point(307, 13);
			this.listSupers.Name = "listSupers";
			this.listSupers.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSupers.Size = new System.Drawing.Size(137, 589);
			this.listSupers.TabIndex = 52;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(119, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(182, 66);
			this.label3.TabIndex = 53;
			this.label3.Text = "Employees who report to the selected supervisors will be included in the Max Pres" +
    "cheduled Off box";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormPhoneGraphDateEdit
			// 
			this.ClientSize = new System.Drawing.Size(473, 652);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listSupers);
			this.Controls.Add(this.textSuperPeak);
			this.Controls.Add(this.textPeak);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butEditSchedule);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneGraphDateEdit";
			this.Text = "Phone Graph Edits";
			this.Load += new System.EventHandler(this.FormPhoneGraphDateEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butEditSchedule;
		private System.Windows.Forms.TextBox textSuperPeak;
		private System.Windows.Forms.TextBox textPeak;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private UI.Button butSave;
		private OpenDental.UI.ListBox listSupers;
		private System.Windows.Forms.Label label3;
	}
}