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
			this.butCancel = new OpenDental.UI.Button();
			this.butEditSchedule = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textSuperPeak = new System.Windows.Forms.TextBox();
			this.textPeak = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listSupers = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(387, 616);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
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
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(306, 616);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 51;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
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
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butEditSchedule);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneGraphDateEdit";
			this.Text = "Phone Graph Edits";
			this.Load += new System.EventHandler(this.FormPhoneGraphDateEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.Button butEditSchedule;
		private System.Windows.Forms.TextBox textSuperPeak;
		private System.Windows.Forms.TextBox textPeak;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private OpenDental.UI.ListBoxOD listSupers;
		private System.Windows.Forms.Label label3;
	}
}