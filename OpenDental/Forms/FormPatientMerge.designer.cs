namespace OpenDental{
	partial class FormPatientMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientMerge));
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.textPatToBirthdate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butChangePatientInto = new OpenDental.UI.Button();
			this.textPatientNameInto = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelPatNumInto = new System.Windows.Forms.Label();
			this.textPatNumInto = new System.Windows.Forms.TextBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.textPatFromBirthdate = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butChangePatientFrom = new OpenDental.UI.Button();
			this.textPatientNameFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelPatNumFrom = new System.Windows.Forms.Label();
			this.textPatNumFrom = new System.Windows.Forms.TextBox();
			this.butMerge = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textPatToBirthdate);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.butChangePatientInto);
			this.groupBox1.Controls.Add(this.textPatientNameInto);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.labelPatNumInto);
			this.groupBox1.Controls.Add(this.textPatNumInto);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(668, 88);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patient to merge into. The patient chosen below will be merged into this account." +
    "";
			// 
			// textPatToBirthdate
			// 
			this.textPatToBirthdate.Location = new System.Drawing.Point(396, 37);
			this.textPatToBirthdate.Name = "textPatToBirthdate";
			this.textPatToBirthdate.ReadOnly = true;
			this.textPatToBirthdate.Size = new System.Drawing.Size(126, 20);
			this.textPatToBirthdate.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(393, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Patient Birthdate";
			// 
			// butChangePatientInto
			// 
			this.butChangePatientInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangePatientInto.Location = new System.Drawing.Point(581, 34);
			this.butChangePatientInto.Name = "butChangePatientInto";
			this.butChangePatientInto.Size = new System.Drawing.Size(75, 24);
			this.butChangePatientInto.TabIndex = 4;
			this.butChangePatientInto.Text = "Change";
			this.butChangePatientInto.Click += new System.EventHandler(this.butChangePatientInto_Click);
			// 
			// textPatientNameInto
			// 
			this.textPatientNameInto.Location = new System.Drawing.Point(153, 37);
			this.textPatientNameInto.Name = "textPatientNameInto";
			this.textPatientNameInto.ReadOnly = true;
			this.textPatientNameInto.Size = new System.Drawing.Size(237, 20);
			this.textPatientNameInto.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(150, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Patient Name";
			// 
			// labelPatNumInto
			// 
			this.labelPatNumInto.AutoSize = true;
			this.labelPatNumInto.Location = new System.Drawing.Point(7, 18);
			this.labelPatNumInto.Name = "labelPatNumInto";
			this.labelPatNumInto.Size = new System.Drawing.Size(80, 13);
			this.labelPatNumInto.TabIndex = 1;
			this.labelPatNumInto.Text = "Patient Number";
			// 
			// textPatNumInto
			// 
			this.textPatNumInto.Location = new System.Drawing.Point(6, 37);
			this.textPatNumInto.Name = "textPatNumInto";
			this.textPatNumInto.ReadOnly = true;
			this.textPatNumInto.Size = new System.Drawing.Size(141, 20);
			this.textPatNumInto.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textPatFromBirthdate);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.butChangePatientFrom);
			this.groupBox2.Controls.Add(this.textPatientNameFrom);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.labelPatNumFrom);
			this.groupBox2.Controls.Add(this.textPatNumFrom);
			this.groupBox2.Location = new System.Drawing.Point(12, 112);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(668, 88);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Patient to merge from. This account will be merged into the account above. This a" +
    "ccount will be archived if not marked deceased.";
			// 
			// textPatFromBirthdate
			// 
			this.textPatFromBirthdate.Location = new System.Drawing.Point(396, 37);
			this.textPatFromBirthdate.Name = "textPatFromBirthdate";
			this.textPatFromBirthdate.ReadOnly = true;
			this.textPatFromBirthdate.Size = new System.Drawing.Size(126, 20);
			this.textPatFromBirthdate.TabIndex = 11;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(396, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "Patient Birthdate";
			// 
			// butChangePatientFrom
			// 
			this.butChangePatientFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangePatientFrom.Location = new System.Drawing.Point(580, 34);
			this.butChangePatientFrom.Name = "butChangePatientFrom";
			this.butChangePatientFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangePatientFrom.TabIndex = 9;
			this.butChangePatientFrom.Text = "Change";
			this.butChangePatientFrom.Click += new System.EventHandler(this.butChangePatientFrom_Click);
			// 
			// textPatientNameFrom
			// 
			this.textPatientNameFrom.Location = new System.Drawing.Point(153, 37);
			this.textPatientNameFrom.Name = "textPatientNameFrom";
			this.textPatientNameFrom.ReadOnly = true;
			this.textPatientNameFrom.Size = new System.Drawing.Size(237, 20);
			this.textPatientNameFrom.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(150, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Patient Name";
			// 
			// labelPatNumFrom
			// 
			this.labelPatNumFrom.AutoSize = true;
			this.labelPatNumFrom.Location = new System.Drawing.Point(7, 18);
			this.labelPatNumFrom.Name = "labelPatNumFrom";
			this.labelPatNumFrom.Size = new System.Drawing.Size(80, 13);
			this.labelPatNumFrom.TabIndex = 6;
			this.labelPatNumFrom.Text = "Patient Number";
			// 
			// textPatNumFrom
			// 
			this.textPatNumFrom.Location = new System.Drawing.Point(6, 37);
			this.textPatNumFrom.Name = "textPatNumFrom";
			this.textPatNumFrom.ReadOnly = true;
			this.textPatNumFrom.Size = new System.Drawing.Size(141, 20);
			this.textPatNumFrom.TabIndex = 5;
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(605, 214);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 3;
			this.butMerge.Text = "&Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// FormPatientMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(693, 250);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butMerge);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientMerge";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Merge Patients";
			this.Load += new System.EventHandler(this.FormPatientMerge_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butMerge;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textPatientNameInto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelPatNumInto;
		private System.Windows.Forms.TextBox textPatNumInto;
		private OpenDental.UI.GroupBox groupBox2;
		private OpenDental.UI.Button butChangePatientInto;
		private OpenDental.UI.Button butChangePatientFrom;
		private System.Windows.Forms.TextBox textPatientNameFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelPatNumFrom;
		private System.Windows.Forms.TextBox textPatNumFrom;
		private System.Windows.Forms.TextBox textPatToBirthdate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textPatFromBirthdate;
		private System.Windows.Forms.Label label6;
	}
}