namespace OpenDental {
	partial class FormEhrLabOrderImport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabOrderImport));
			this.butCancel = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSave = new System.Windows.Forms.Button();
			this.butPatSelect = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.textBirthdate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxRaces = new OpenDental.UI.ListBoxOD();
			this.textGender = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxNames = new OpenDental.UI.ListBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.labelExistingLab = new System.Windows.Forms.Label();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(627, 357);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 163);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(690, 188);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Laboratory Orders";
			this.gridMain.TranslationName = "TableOrders";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(546, 357);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 23);
			this.butSave.TabIndex = 10;
			this.butSave.Text = "Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butPatSelect
			// 
			this.butPatSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPatSelect.Location = new System.Drawing.Point(325, 9);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(29, 25);
			this.butPatSelect.TabIndex = 231;
			this.butPatSelect.Text = "...";
			this.butPatSelect.Click += new System.EventHandler(this.butPatSelect_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(27, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 17);
			this.label5.TabIndex = 230;
			this.label5.Text = "Attached Patient";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(126, 12);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(193, 20);
			this.textName.TabIndex = 229;
			this.textName.WordWrap = false;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.textBirthdate);
			this.groupBox5.Controls.Add(this.label2);
			this.groupBox5.Controls.Add(this.listBoxRaces);
			this.groupBox5.Controls.Add(this.textGender);
			this.groupBox5.Controls.Add(this.label1);
			this.groupBox5.Controls.Add(this.listBoxNames);
			this.groupBox5.Controls.Add(this.label7);
			this.groupBox5.Controls.Add(this.label9);
			this.groupBox5.Location = new System.Drawing.Point(12, 38);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(697, 119);
			this.groupBox5.TabIndex = 249;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Patient Information From Message";
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(114, 64);
			this.textBirthdate.MaxLength = 100;
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.Size = new System.Drawing.Size(93, 20);
			this.textBirthdate.TabIndex = 244;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(330, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 16);
			this.label2.TabIndex = 243;
			this.label2.Text = "Race(s)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxRaces
			// 
			this.listBoxRaces.Location = new System.Drawing.Point(411, 17);
			this.listBoxRaces.Name = "listBoxRaces";
			this.listBoxRaces.Size = new System.Drawing.Size(154, 43);
			this.listBoxRaces.TabIndex = 242;
			// 
			// textGender
			// 
			this.textGender.Location = new System.Drawing.Point(114, 88);
			this.textGender.MaxLength = 100;
			this.textGender.Name = "textGender";
			this.textGender.Size = new System.Drawing.Size(93, 20);
			this.textGender.TabIndex = 241;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 240;
			this.label1.Text = "Name(s)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxNames
			// 
			this.listBoxNames.Location = new System.Drawing.Point(114, 17);
			this.listBoxNames.Name = "listBoxNames";
			this.listBoxNames.Size = new System.Drawing.Size(193, 43);
			this.listBoxNames.TabIndex = 239;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(30, 90);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(82, 16);
			this.label7.TabIndex = 237;
			this.label7.Text = "Gender";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(27, 67);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(85, 14);
			this.label9.TabIndex = 235;
			this.label9.Text = "Birthdate";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelExistingLab
			// 
			this.labelExistingLab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExistingLab.ForeColor = System.Drawing.Color.Red;
			this.labelExistingLab.Location = new System.Drawing.Point(154, 360);
			this.labelExistingLab.Name = "labelExistingLab";
			this.labelExistingLab.Size = new System.Drawing.Size(386, 17);
			this.labelExistingLab.TabIndex = 250;
			this.labelExistingLab.Text = "Saving these results will update one or more existing labs.";
			this.labelExistingLab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelExistingLab.Visible = false;
			// 
			// FormEhrLabOrderImport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(714, 392);
			this.Controls.Add(this.labelExistingLab);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.butPatSelect);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabOrderImport";
			this.Text = "Lab Orders";
			this.Load += new System.EventHandler(this.FormEhrLabOrders_Load);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butSave;
		private UI.Button butPatSelect;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listBoxNames;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listBoxRaces;
		private System.Windows.Forms.TextBox textGender;
		private System.Windows.Forms.TextBox textBirthdate;
		private System.Windows.Forms.Label labelExistingLab;
	}
}