namespace OpenDental{
	partial class FormProvStudentEdit {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProvStudentEdit));
			this.label4 = new System.Windows.Forms.Label();
			this.textFirstName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textLastName = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textAbbr = new System.Windows.Forms.TextBox();
			this.comboClass = new OpenDental.UI.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.labelUniqueID = new System.Windows.Forms.Label();
			this.labelPassDescription = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 18);
			this.label4.TabIndex = 23;
			this.label4.Text = "First Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFirstName
			// 
			this.textFirstName.Location = new System.Drawing.Point(117, 99);
			this.textFirstName.MaxLength = 15;
			this.textFirstName.Name = "textFirstName";
			this.textFirstName.Size = new System.Drawing.Size(166, 20);
			this.textFirstName.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 73);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 18);
			this.label1.TabIndex = 25;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLastName
			// 
			this.textLastName.Location = new System.Drawing.Point(117, 73);
			this.textLastName.MaxLength = 15;
			this.textLastName.Name = "textLastName";
			this.textLastName.Size = new System.Drawing.Size(166, 20);
			this.textLastName.TabIndex = 2;
			// 
			// labelUser
			// 
			this.labelUser.Location = new System.Drawing.Point(16, 152);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(101, 18);
			this.labelUser.TabIndex = 27;
			this.labelUser.Text = "User Name";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(117, 151);
			this.textUserName.MaxLength = 15;
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(166, 20);
			this.textUserName.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(101, 18);
			this.label3.TabIndex = 29;
			this.label3.Text = "Abbr";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAbbr
			// 
			this.textAbbr.Location = new System.Drawing.Point(117, 125);
			this.textAbbr.MaxLength = 15;
			this.textAbbr.Name = "textAbbr";
			this.textAbbr.Size = new System.Drawing.Size(60, 20);
			this.textAbbr.TabIndex = 4;
			this.textAbbr.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textAbbr_KeyUp);
			// 
			// comboClass
			// 
			this.comboClass.Location = new System.Drawing.Point(117, 46);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(166, 21);
			this.comboClass.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(15, 47);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(101, 18);
			this.label5.TabIndex = 31;
			this.label5.Text = "Class";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 178);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 18);
			this.label2.TabIndex = 33;
			this.label2.Text = "Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(117, 177);
			this.textPassword.MaxLength = 15;
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(166, 20);
			this.textPassword.TabIndex = 6;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(117, 21);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.ReadOnly = true;
			this.textProvNum.Size = new System.Drawing.Size(97, 20);
			this.textProvNum.TabIndex = 34;
			// 
			// labelUniqueID
			// 
			this.labelUniqueID.Location = new System.Drawing.Point(12, 21);
			this.labelUniqueID.Name = "labelUniqueID";
			this.labelUniqueID.Size = new System.Drawing.Size(105, 18);
			this.labelUniqueID.TabIndex = 35;
			this.labelUniqueID.Text = "ProvNum";
			this.labelUniqueID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPassDescription
			// 
			this.labelPassDescription.Location = new System.Drawing.Point(114, 200);
			this.labelPassDescription.Name = "labelPassDescription";
			this.labelPassDescription.Size = new System.Drawing.Size(169, 37);
			this.labelPassDescription.TabIndex = 247;
			this.labelPassDescription.Text = "To keep the old password, leave the box empty.";
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(329, 213);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 7;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// FormProvStudentEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(416, 246);
			this.Controls.Add(this.labelPassDescription);
			this.Controls.Add(this.labelUniqueID);
			this.Controls.Add(this.textProvNum);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.comboClass);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textAbbr);
			this.Controls.Add(this.labelUser);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textLastName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textFirstName);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProvStudentEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Student Edit";
			this.Load += new System.EventHandler(this.FormProvStudentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textFirstName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textLastName;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textAbbr;
		private OpenDental.UI.ComboBox comboClass;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.TextBox textProvNum;
		private System.Windows.Forms.Label labelUniqueID;
		private System.Windows.Forms.Label labelPassDescription;
	}
}