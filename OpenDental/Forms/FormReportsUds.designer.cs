namespace OpenDental{
	partial class FormReportsUds {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportsUds));
			this.label1 = new System.Windows.Forms.Label();
			this.textFrom = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textTo = new System.Windows.Forms.TextBox();
			this.but9D = new OpenDental.UI.Button();
			this.but6A = new OpenDental.UI.Button();
			this.but5 = new OpenDental.UI.Button();
			this.but4 = new OpenDental.UI.Button();
			this.but3B = new OpenDental.UI.Button();
			this.butPatByZip = new OpenDental.UI.Button();
			this.but3A = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-28, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "From";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFrom
			// 
			this.textFrom.Location = new System.Drawing.Point(78, 20);
			this.textFrom.Name = "textFrom";
			this.textFrom.Size = new System.Drawing.Size(100, 20);
			this.textFrom.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(197, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "To";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTo
			// 
			this.textTo.Location = new System.Drawing.Point(238, 20);
			this.textTo.Name = "textTo";
			this.textTo.Size = new System.Drawing.Size(100, 20);
			this.textTo.TabIndex = 5;
			// 
			// but9D
			// 
			this.but9D.Location = new System.Drawing.Point(32, 246);
			this.but9D.Name = "but9D";
			this.but9D.Size = new System.Drawing.Size(146, 24);
			this.but9D.TabIndex = 3;
			this.but9D.Text = "9D";
			// 
			// but6A
			// 
			this.but6A.Location = new System.Drawing.Point(32, 216);
			this.but6A.Name = "but6A";
			this.but6A.Size = new System.Drawing.Size(146, 24);
			this.but6A.TabIndex = 3;
			this.but6A.Text = "6A";
			// 
			// but5
			// 
			this.but5.Location = new System.Drawing.Point(32, 186);
			this.but5.Name = "but5";
			this.but5.Size = new System.Drawing.Size(146, 24);
			this.but5.TabIndex = 3;
			this.but5.Text = "5 - Staffing";
			// 
			// but4
			// 
			this.but4.Location = new System.Drawing.Point(32, 156);
			this.but4.Name = "but4";
			this.but4.Size = new System.Drawing.Size(146, 24);
			this.but4.TabIndex = 3;
			this.but4.Text = "4 - Patient Characteristics";
			// 
			// but3B
			// 
			this.but3B.Location = new System.Drawing.Point(32, 126);
			this.but3B.Name = "but3B";
			this.but3B.Size = new System.Drawing.Size(146, 24);
			this.but3B.TabIndex = 3;
			this.but3B.Text = "3B - Race Ethicity";
			// 
			// butPatByZip
			// 
			this.butPatByZip.Location = new System.Drawing.Point(32, 66);
			this.butPatByZip.Name = "butPatByZip";
			this.butPatByZip.Size = new System.Drawing.Size(146, 24);
			this.butPatByZip.TabIndex = 3;
			this.butPatByZip.Text = "Patients by Zip Code";
			this.butPatByZip.Click += new System.EventHandler(this.butPatByZip_Click);
			// 
			// but3A
			// 
			this.but3A.Location = new System.Drawing.Point(32, 96);
			this.but3A.Name = "but3A";
			this.but3A.Size = new System.Drawing.Size(146, 24);
			this.but3A.TabIndex = 3;
			this.but3A.Text = "3A - Age Gender";
			this.but3A.Click += new System.EventHandler(this.but3A_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(783, 514);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(391, 274);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(261, 128);
			this.label3.TabIndex = 6;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// FormReportsUds
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(883, 565);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textTo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textFrom);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.but9D);
			this.Controls.Add(this.but6A);
			this.Controls.Add(this.but5);
			this.Controls.Add(this.but4);
			this.Controls.Add(this.but3B);
			this.Controls.Add(this.butPatByZip);
			this.Controls.Add(this.but3A);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportsUds";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "UDS Reporting";
			this.Load += new System.EventHandler(this.FormReportsUds_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.Button but3A;
		private UI.Button but3B;
		private UI.Button but4;
		private UI.Button but5;
		private UI.Button but6A;
		private UI.Button but9D;
		private UI.Button butPatByZip;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFrom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textTo;
		private System.Windows.Forms.Label label3;
	}
}