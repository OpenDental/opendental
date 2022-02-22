namespace OpenDental{
	partial class FormProcEditAll {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcEditAll));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDate = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.butEditAnyway = new OpenDental.UI.Button();
			this.labelClaim = new System.Windows.Forms.Label();
			this.butToday = new OpenDental.UI.Button();
			this.comboProv = new UI.ComboBoxOD();
			this.butMoreProvs = new OpenDental.UI.Button();
			this.labelClinic = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboClinic = new UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(440, 129);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(440, 159);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(107, 29);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(76, 20);
			this.textDate.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 14);
			this.label1.TabIndex = 4;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEditAnyway
			// 
			this.butEditAnyway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditAnyway.Location = new System.Drawing.Point(288, 159);
			this.butEditAnyway.Name = "butEditAnyway";
			this.butEditAnyway.Size = new System.Drawing.Size(96, 24);
			this.butEditAnyway.TabIndex = 53;
			this.butEditAnyway.Text = "&Edit Anyway";
			this.butEditAnyway.Visible = false;
			this.butEditAnyway.Click += new System.EventHandler(this.butEditAnyway_Click);
			// 
			// labelClaim
			// 
			this.labelClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelClaim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelClaim.Location = new System.Drawing.Point(9, 110);
			this.labelClaim.Name = "labelClaim";
			this.labelClaim.Size = new System.Drawing.Size(295, 73);
			this.labelClaim.TabIndex = 52;
			this.labelClaim.Text = "A procedure is attached to a claim, so certain fields should not be edited.  You " +
    "should reprint the claim if any significant changes are made.";
			this.labelClaim.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelClaim.Visible = false;
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(193, 27);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(72, 22);
			this.butToday.TabIndex = 54;
			this.butToday.Text = "Today";
			this.butToday.Visible = false;
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(107, 57);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(133, 21);
			this.comboProv.TabIndex = 178;
			// 
			// butMoreProvs
			// 
			this.butMoreProvs.Location = new System.Drawing.Point(243, 56);
			this.butMoreProvs.Name = "butMoreProvs";
			this.butMoreProvs.Size = new System.Drawing.Size(19, 22);
			this.butMoreProvs.TabIndex = 176;
			this.butMoreProvs.Text = "...";
			this.butMoreProvs.Click += new System.EventHandler(this.butMoreProvs_Click);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(6, 90);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(96, 14);
			this.labelClinic.TabIndex = 175;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelClinic.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 14);
			this.label2.TabIndex = 174;
			this.label2.Text = "Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(107, 86);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(133, 21);
			this.comboClinic.TabIndex = 179;
			this.comboClinic.Visible = false;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(516, 23);
			this.label3.TabIndex = 180;
			this.label3.Text = "Blank fields represent multiple initial values, leave blank to preserve original " +
    "value.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormProcEditAll
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(540, 210);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.butMoreProvs);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.butEditAnyway);
			this.Controls.Add(this.labelClaim);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcEditAll";
			this.Text = "Edit All Procs";
			this.Load += new System.EventHandler(this.FormProcEditAll_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidDate textDate;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butEditAnyway;
		private System.Windows.Forms.Label labelClaim;
		private OpenDental.UI.Button butToday;
		private UI.ComboBoxOD comboProv;
		private UI.Button butMoreProvs;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboClinic;
		private System.Windows.Forms.Label label3;
	}
}