namespace OpenDental{
	partial class FormCdsTriggerEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCdsTriggerEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butAddSnomed = new OpenDental.UI.Button();
			this.butAddIcd10 = new OpenDental.UI.Button();
			this.butAddIcd9 = new OpenDental.UI.Button();
			this.butAddProblem = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butAddCvx = new OpenDental.UI.Button();
			this.butAddRxNorm = new OpenDental.UI.Button();
			this.butAddMed = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.butAddAllergy = new OpenDental.UI.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butAddGender = new OpenDental.UI.Button();
			this.butAddAge = new OpenDental.UI.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butAddLab = new OpenDental.UI.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.butAddBMI = new OpenDental.UI.Button();
			this.butAddBP = new OpenDental.UI.Button();
			this.butAddWeight = new OpenDental.UI.Button();
			this.butAddHeight = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboCardinality = new System.Windows.Forms.ComboBox();
			this.labelCardinality = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textInstruction = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBibliography = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(652, 616);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 616);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 122;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(571, 616);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 121;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butAddSnomed);
			this.groupBox1.Controls.Add(this.butAddIcd10);
			this.groupBox1.Controls.Add(this.butAddIcd9);
			this.groupBox1.Controls.Add(this.butAddProblem);
			this.groupBox1.Location = new System.Drawing.Point(12, 72);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(114, 132);
			this.groupBox1.TabIndex = 123;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Problems";
			// 
			// butAddSnomed
			// 
			this.butAddSnomed.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddSnomed.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSnomed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSnomed.Location = new System.Drawing.Point(6, 102);
			this.butAddSnomed.Name = "butAddSnomed";
			this.butAddSnomed.Size = new System.Drawing.Size(99, 23);
			this.butAddSnomed.TabIndex = 206;
			this.butAddSnomed.Text = "SNOMEDCT";
			this.butAddSnomed.Click += new System.EventHandler(this.butAddSnomed_Click);
			// 
			// butAddIcd10
			// 
			this.butAddIcd10.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddIcd10.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddIcd10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddIcd10.Location = new System.Drawing.Point(6, 73);
			this.butAddIcd10.Name = "butAddIcd10";
			this.butAddIcd10.Size = new System.Drawing.Size(99, 23);
			this.butAddIcd10.TabIndex = 205;
			this.butAddIcd10.Text = "ICD10";
			this.butAddIcd10.Click += new System.EventHandler(this.butAddIcd10_Click);
			// 
			// butAddIcd9
			// 
			this.butAddIcd9.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddIcd9.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddIcd9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddIcd9.Location = new System.Drawing.Point(6, 44);
			this.butAddIcd9.Name = "butAddIcd9";
			this.butAddIcd9.Size = new System.Drawing.Size(99, 23);
			this.butAddIcd9.TabIndex = 204;
			this.butAddIcd9.Text = "ICD9";
			this.butAddIcd9.Click += new System.EventHandler(this.butAddIcd9_Click);
			// 
			// butAddProblem
			// 
			this.butAddProblem.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddProblem.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddProblem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddProblem.Location = new System.Drawing.Point(6, 15);
			this.butAddProblem.Name = "butAddProblem";
			this.butAddProblem.Size = new System.Drawing.Size(99, 23);
			this.butAddProblem.TabIndex = 203;
			this.butAddProblem.Text = "Problem";
			this.butAddProblem.Click += new System.EventHandler(this.butAddProblem_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(138, 77);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(589, 392);
			this.gridMain.TabIndex = 202;
			this.gridMain.Title = "Trigger Conditions";
			this.gridMain.WrapText = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butAddCvx);
			this.groupBox2.Controls.Add(this.butAddRxNorm);
			this.groupBox2.Controls.Add(this.butAddMed);
			this.groupBox2.Location = new System.Drawing.Point(12, 205);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(114, 102);
			this.groupBox2.TabIndex = 207;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Medications";
			// 
			// butAddCvx
			// 
			this.butAddCvx.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddCvx.Enabled = false;
			this.butAddCvx.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCvx.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCvx.Location = new System.Drawing.Point(6, 73);
			this.butAddCvx.Name = "butAddCvx";
			this.butAddCvx.Size = new System.Drawing.Size(99, 23);
			this.butAddCvx.TabIndex = 205;
			this.butAddCvx.Text = "Cvx";
			this.butAddCvx.Click += new System.EventHandler(this.butAddCvx_Click);
			// 
			// butAddRxNorm
			// 
			this.butAddRxNorm.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddRxNorm.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddRxNorm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRxNorm.Location = new System.Drawing.Point(6, 44);
			this.butAddRxNorm.Name = "butAddRxNorm";
			this.butAddRxNorm.Size = new System.Drawing.Size(99, 23);
			this.butAddRxNorm.TabIndex = 204;
			this.butAddRxNorm.Text = "RxNorm";
			this.butAddRxNorm.Click += new System.EventHandler(this.butAddRxNorm_Click);
			// 
			// butAddMed
			// 
			this.butAddMed.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddMed.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddMed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddMed.Location = new System.Drawing.Point(6, 15);
			this.butAddMed.Name = "butAddMed";
			this.butAddMed.Size = new System.Drawing.Size(99, 23);
			this.butAddMed.TabIndex = 203;
			this.butAddMed.Text = "Med";
			this.butAddMed.Click += new System.EventHandler(this.butAddMed_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.butAddAllergy);
			this.groupBox3.Location = new System.Drawing.Point(12, 309);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(114, 45);
			this.groupBox3.TabIndex = 208;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Allergies";
			// 
			// butAddAllergy
			// 
			this.butAddAllergy.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddAllergy.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAllergy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAllergy.Location = new System.Drawing.Point(6, 15);
			this.butAddAllergy.Name = "butAddAllergy";
			this.butAddAllergy.Size = new System.Drawing.Size(99, 23);
			this.butAddAllergy.TabIndex = 203;
			this.butAddAllergy.Text = "Allergy";
			this.butAddAllergy.Click += new System.EventHandler(this.butAddAllergy_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.butAddGender);
			this.groupBox4.Controls.Add(this.butAddAge);
			this.groupBox4.Location = new System.Drawing.Point(12, 356);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(114, 73);
			this.groupBox4.TabIndex = 209;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Demographics";
			// 
			// butAddGender
			// 
			this.butAddGender.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddGender.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddGender.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGender.Location = new System.Drawing.Point(6, 44);
			this.butAddGender.Name = "butAddGender";
			this.butAddGender.Size = new System.Drawing.Size(99, 23);
			this.butAddGender.TabIndex = 204;
			this.butAddGender.Text = "Gender";
			this.butAddGender.Click += new System.EventHandler(this.butAddGender_Click);
			// 
			// butAddAge
			// 
			this.butAddAge.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddAge.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAge.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAge.Location = new System.Drawing.Point(6, 15);
			this.butAddAge.Name = "butAddAge";
			this.butAddAge.Size = new System.Drawing.Size(99, 23);
			this.butAddAge.TabIndex = 203;
			this.butAddAge.Text = "Age";
			this.butAddAge.Click += new System.EventHandler(this.butAddAge_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.butAddLab);
			this.groupBox5.Location = new System.Drawing.Point(12, 431);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(114, 45);
			this.groupBox5.TabIndex = 210;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Lab Results";
			// 
			// butAddLab
			// 
			this.butAddLab.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddLab.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddLab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLab.Location = new System.Drawing.Point(6, 15);
			this.butAddLab.Name = "butAddLab";
			this.butAddLab.Size = new System.Drawing.Size(99, 23);
			this.butAddLab.TabIndex = 203;
			this.butAddLab.Text = "Lab";
			this.butAddLab.Click += new System.EventHandler(this.butAddLab_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.butAddBMI);
			this.groupBox6.Controls.Add(this.butAddBP);
			this.groupBox6.Controls.Add(this.butAddWeight);
			this.groupBox6.Controls.Add(this.butAddHeight);
			this.groupBox6.Location = new System.Drawing.Point(12, 478);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(114, 132);
			this.groupBox6.TabIndex = 211;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Vital Signs";
			// 
			// butAddBMI
			// 
			this.butAddBMI.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddBMI.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddBMI.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddBMI.Location = new System.Drawing.Point(6, 102);
			this.butAddBMI.Name = "butAddBMI";
			this.butAddBMI.Size = new System.Drawing.Size(99, 23);
			this.butAddBMI.TabIndex = 206;
			this.butAddBMI.Text = "BMI";
			this.butAddBMI.Click += new System.EventHandler(this.butAddBMI_Click);
			// 
			// butAddBP
			// 
			this.butAddBP.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddBP.Enabled = false;
			this.butAddBP.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddBP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddBP.Location = new System.Drawing.Point(6, 73);
			this.butAddBP.Name = "butAddBP";
			this.butAddBP.Size = new System.Drawing.Size(99, 23);
			this.butAddBP.TabIndex = 205;
			this.butAddBP.Text = "BP";
			this.butAddBP.Click += new System.EventHandler(this.butAddBP_Click);
			// 
			// butAddWeight
			// 
			this.butAddWeight.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddWeight.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddWeight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddWeight.Location = new System.Drawing.Point(6, 44);
			this.butAddWeight.Name = "butAddWeight";
			this.butAddWeight.Size = new System.Drawing.Size(99, 23);
			this.butAddWeight.TabIndex = 204;
			this.butAddWeight.Text = "Weight";
			this.butAddWeight.Click += new System.EventHandler(this.butAddWeight_Click);
			// 
			// butAddHeight
			// 
			this.butAddHeight.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddHeight.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddHeight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddHeight.Location = new System.Drawing.Point(6, 15);
			this.butAddHeight.Name = "butAddHeight";
			this.butAddHeight.Size = new System.Drawing.Size(99, 23);
			this.butAddHeight.TabIndex = 203;
			this.butAddHeight.Text = "Height";
			this.butAddHeight.Click += new System.EventHandler(this.butAddHeight_Click);
			// 
			// textDescription
			// 
			this.textDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textDescription.Location = new System.Drawing.Point(138, 12);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(388, 20);
			this.textDescription.TabIndex = 213;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(35, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 17);
			this.label2.TabIndex = 212;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCardinality
			// 
			this.comboCardinality.FormattingEnabled = true;
			this.comboCardinality.Location = new System.Drawing.Point(138, 44);
			this.comboCardinality.Name = "comboCardinality";
			this.comboCardinality.Size = new System.Drawing.Size(158, 21);
			this.comboCardinality.TabIndex = 216;
			this.comboCardinality.SelectedIndexChanged += new System.EventHandler(this.comboCardinality_SelectedIndexChanged);
			// 
			// labelCardinality
			// 
			this.labelCardinality.Location = new System.Drawing.Point(302, 35);
			this.labelCardinality.Name = "labelCardinality";
			this.labelCardinality.Size = new System.Drawing.Size(425, 39);
			this.labelCardinality.TabIndex = 214;
			this.labelCardinality.Text = "For this trigger to provide Clinical Decision Support, only one of the conditions" +
    " below must be met.";
			this.labelCardinality.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(45, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 17);
			this.label4.TabIndex = 215;
			this.label4.Text = "Cardinality";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(138, 478);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 17);
			this.label3.TabIndex = 218;
			this.label3.Text = "Instructions";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInstruction
			// 
			this.textInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textInstruction.Location = new System.Drawing.Point(233, 478);
			this.textInstruction.Multiline = true;
			this.textInstruction.Name = "textInstruction";
			this.textInstruction.Size = new System.Drawing.Size(494, 84);
			this.textInstruction.TabIndex = 217;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(138, 568);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 17);
			this.label1.TabIndex = 220;
			this.label1.Text = "Bibliography";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBibliography
			// 
			this.textBibliography.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBibliography.Location = new System.Drawing.Point(233, 568);
			this.textBibliography.Multiline = true;
			this.textBibliography.Name = "textBibliography";
			this.textBibliography.Size = new System.Drawing.Size(494, 42);
			this.textBibliography.TabIndex = 219;
			// 
			// FormCdsTriggerEdit
			// 
			this.ClientSize = new System.Drawing.Size(739, 652);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBibliography);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textInstruction);
			this.Controls.Add(this.comboCardinality);
			this.Controls.Add(this.labelCardinality);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCdsTriggerEdit";
			this.Text = "CDS Trigger Edit";
			this.Load += new System.EventHandler(this.FormEhrTriggerEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private UI.Button butOK;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butAddSnomed;
		private UI.Button butAddIcd10;
		private UI.Button butAddIcd9;
		private UI.Button butAddProblem;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupBox2;
		private UI.Button butAddCvx;
		private UI.Button butAddRxNorm;
		private UI.Button butAddMed;
		private System.Windows.Forms.GroupBox groupBox3;
		private UI.Button butAddAllergy;
		private System.Windows.Forms.GroupBox groupBox4;
		private UI.Button butAddGender;
		private UI.Button butAddAge;
		private System.Windows.Forms.GroupBox groupBox5;
		private UI.Button butAddLab;
		private System.Windows.Forms.GroupBox groupBox6;
		private UI.Button butAddWeight;
		private UI.Button butAddHeight;
		private UI.Button butAddBMI;
		private UI.Button butAddBP;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboCardinality;
		private System.Windows.Forms.Label labelCardinality;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textInstruction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBibliography;
	}
}