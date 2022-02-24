namespace OpenDental{
	partial class FormGuardianEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGuardianEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textFamilyMember = new System.Windows.Forms.TextBox();
			this.comboRelationship = new System.Windows.Forms.ComboBox();
			this.checkIsGuardian = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butPick = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Patient";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(113, 16);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(214, 20);
			this.textPatient.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Family Member";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(111, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Relationship";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFamilyMember
			// 
			this.textFamilyMember.Location = new System.Drawing.Point(113, 41);
			this.textFamilyMember.Name = "textFamilyMember";
			this.textFamilyMember.ReadOnly = true;
			this.textFamilyMember.Size = new System.Drawing.Size(214, 20);
			this.textFamilyMember.TabIndex = 10;
			// 
			// comboRelationship
			// 
			this.comboRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRelationship.FormattingEnabled = true;
			this.comboRelationship.Location = new System.Drawing.Point(113, 85);
			this.comboRelationship.Name = "comboRelationship";
			this.comboRelationship.Size = new System.Drawing.Size(214, 21);
			this.comboRelationship.TabIndex = 69;
			// 
			// checkIsGuardian
			// 
			this.checkIsGuardian.Location = new System.Drawing.Point(2, 64);
			this.checkIsGuardian.Name = "checkIsGuardian";
			this.checkIsGuardian.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsGuardian.Size = new System.Drawing.Size(125, 18);
			this.checkIsGuardian.TabIndex = 70;
			this.checkIsGuardian.Text = "Guardian";
			this.checkIsGuardian.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(133, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(245, 16);
			this.label4.TabIndex = 71;
			this.label4.Text = "Shows relationship on appointments";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 128);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 68;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butPick
			// 
			this.butPick.Location = new System.Drawing.Point(331, 41);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(27, 20);
			this.butPick.TabIndex = 67;
			this.butPick.TabStop = false;
			this.butPick.Text = "...";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(222, 128);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(303, 128);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormGuardianEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(390, 164);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.checkIsGuardian);
			this.Controls.Add(this.comboRelationship);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butPick);
			this.Controls.Add(this.textFamilyMember);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGuardianEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Family Relationship";
			this.Load += new System.EventHandler(this.FormGuardianEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textFamilyMember;
		private OpenDental.UI.Button butPick;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.ComboBox comboRelationship;
		private System.Windows.Forms.CheckBox checkIsGuardian;
		private System.Windows.Forms.Label label4;
	}
}