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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textPatToBirthdate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butChangePatientInto = new OpenDental.UI.Button();
			this.textPatientNameInto = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textPatientIDInto = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textPatFromBirthdate = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butChangePatientFrom = new OpenDental.UI.Button();
			this.textPatientNameFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textPatientIDFrom = new System.Windows.Forms.TextBox();
			this.butMerge = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
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
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textPatientIDInto);
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Patient ID";
			// 
			// textPatientIDInto
			// 
			this.textPatientIDInto.Location = new System.Drawing.Point(6, 37);
			this.textPatientIDInto.Name = "textPatientIDInto";
			this.textPatientIDInto.ReadOnly = true;
			this.textPatientIDInto.Size = new System.Drawing.Size(141, 20);
			this.textPatientIDInto.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textPatFromBirthdate);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.butChangePatientFrom);
			this.groupBox2.Controls.Add(this.textPatientNameFrom);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textPatientIDFrom);
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Patient ID";
			// 
			// textPatientIDFrom
			// 
			this.textPatientIDFrom.Location = new System.Drawing.Point(6, 37);
			this.textPatientIDFrom.Name = "textPatientIDFrom";
			this.textPatientIDFrom.ReadOnly = true;
			this.textPatientIDFrom.Size = new System.Drawing.Size(141, 20);
			this.textPatientIDFrom.TabIndex = 5;
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(509, 213);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 3;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(593, 213);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormPatientMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(693, 250);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.butCancel);
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
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textPatientNameInto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPatientIDInto;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butChangePatientInto;
		private OpenDental.UI.Button butChangePatientFrom;
		private System.Windows.Forms.TextBox textPatientNameFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPatientIDFrom;
		private System.Windows.Forms.TextBox textPatToBirthdate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textPatFromBirthdate;
		private System.Windows.Forms.Label label6;
	}
}