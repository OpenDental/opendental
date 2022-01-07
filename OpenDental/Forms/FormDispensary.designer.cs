namespace OpenDental{
	partial class FormDispensary {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDispensary));
			this.groupDentalSchools = new System.Windows.Forms.GroupBox();
			this.labelClass = new System.Windows.Forms.Label();
			this.comboClass = new System.Windows.Forms.ComboBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.labelUniqueID = new System.Windows.Forms.Label();
			this.groupSupply = new System.Windows.Forms.GroupBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.labelSupDescript = new System.Windows.Forms.Label();
			this.labelCategory = new System.Windows.Forms.Label();
			this.comboCategory = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textSupplier = new System.Windows.Forms.TextBox();
			this.labelSupplier = new System.Windows.Forms.Label();
			this.textSupplyDescript = new System.Windows.Forms.TextBox();
			this.labelSupplyDescript = new System.Windows.Forms.Label();
			this.textSupplyNum = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.gridEquip = new OpenDental.UI.GridOD();
			this.gridDispSupply = new OpenDental.UI.GridOD();
			this.gridSupply = new OpenDental.UI.GridOD();
			this.gridStudents = new OpenDental.UI.GridOD();
			this.butScan = new OpenDental.UI.Button();
			this.butCheckOut = new OpenDental.UI.Button();
			this.butCheckIn = new OpenDental.UI.Button();
			this.groupDentalSchools.SuspendLayout();
			this.groupSupply.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupDentalSchools
			// 
			this.groupDentalSchools.Controls.Add(this.labelClass);
			this.groupDentalSchools.Controls.Add(this.comboClass);
			this.groupDentalSchools.Controls.Add(this.textLName);
			this.groupDentalSchools.Controls.Add(this.label2);
			this.groupDentalSchools.Controls.Add(this.textFName);
			this.groupDentalSchools.Controls.Add(this.label1);
			this.groupDentalSchools.Controls.Add(this.textProvNum);
			this.groupDentalSchools.Controls.Add(this.labelUniqueID);
			this.groupDentalSchools.Location = new System.Drawing.Point(78, 0);
			this.groupDentalSchools.Name = "groupDentalSchools";
			this.groupDentalSchools.Size = new System.Drawing.Size(200, 108);
			this.groupDentalSchools.TabIndex = 14;
			this.groupDentalSchools.TabStop = false;
			this.groupDentalSchools.Text = "Student Filters";
			// 
			// labelClass
			// 
			this.labelClass.Location = new System.Drawing.Point(6, 82);
			this.labelClass.Name = "labelClass";
			this.labelClass.Size = new System.Drawing.Size(68, 18);
			this.labelClass.TabIndex = 33;
			this.labelClass.Text = "Class";
			this.labelClass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClass
			// 
			this.comboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClass.FormattingEnabled = true;
			this.comboClass.Location = new System.Drawing.Point(76, 82);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(118, 21);
			this.comboClass.TabIndex = 4;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(76, 40);
			this.textLName.MaxLength = 15;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(118, 20);
			this.textLName.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 18);
			this.label2.TabIndex = 31;
			this.label2.Text = "LName";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(76, 61);
			this.textFName.MaxLength = 15;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(118, 20);
			this.textFName.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 18);
			this.label1.TabIndex = 29;
			this.label1.Text = "FName";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(76, 19);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.Size = new System.Drawing.Size(118, 20);
			this.textProvNum.TabIndex = 1;
			// 
			// labelUniqueID
			// 
			this.labelUniqueID.Location = new System.Drawing.Point(6, 19);
			this.labelUniqueID.Name = "labelUniqueID";
			this.labelUniqueID.Size = new System.Drawing.Size(68, 18);
			this.labelUniqueID.TabIndex = 27;
			this.labelUniqueID.Text = "ProvNum";
			this.labelUniqueID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupSupply
			// 
			this.groupSupply.Controls.Add(this.butCheckOut);
			this.groupSupply.Controls.Add(this.butCheckIn);
			this.groupSupply.Controls.Add(this.textBox1);
			this.groupSupply.Controls.Add(this.label4);
			this.groupSupply.Controls.Add(this.textBox2);
			this.groupSupply.Controls.Add(this.label5);
			this.groupSupply.Controls.Add(this.textBox3);
			this.groupSupply.Controls.Add(this.labelSupDescript);
			this.groupSupply.Location = new System.Drawing.Point(384, 0);
			this.groupSupply.Name = "groupSupply";
			this.groupSupply.Size = new System.Drawing.Size(200, 118);
			this.groupSupply.TabIndex = 19;
			this.groupSupply.TabStop = false;
			this.groupSupply.Text = "Supplies";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(76, 40);
			this.textBox1.MaxLength = 15;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(118, 20);
			this.textBox1.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 18);
			this.label4.TabIndex = 31;
			this.label4.Text = "LName";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(76, 61);
			this.textBox2.MaxLength = 15;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(118, 20);
			this.textBox2.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 18);
			this.label5.TabIndex = 29;
			this.label5.Text = "FName";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(76, 19);
			this.textBox3.MaxLength = 15;
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(118, 20);
			this.textBox3.TabIndex = 1;
			// 
			// labelSupDescript
			// 
			this.labelSupDescript.Location = new System.Drawing.Point(6, 19);
			this.labelSupDescript.Name = "labelSupDescript";
			this.labelSupDescript.Size = new System.Drawing.Size(68, 18);
			this.labelSupDescript.TabIndex = 27;
			this.labelSupDescript.Text = "Description";
			this.labelSupDescript.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCategory
			// 
			this.labelCategory.Location = new System.Drawing.Point(6, 18);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(89, 18);
			this.labelCategory.TabIndex = 33;
			this.labelCategory.Text = "Category";
			this.labelCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCategory
			// 
			this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCategory.FormattingEnabled = true;
			this.comboCategory.Location = new System.Drawing.Point(98, 18);
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(131, 21);
			this.comboCategory.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textSupplier);
			this.groupBox1.Controls.Add(this.labelSupplier);
			this.groupBox1.Controls.Add(this.comboCategory);
			this.groupBox1.Controls.Add(this.textSupplyDescript);
			this.groupBox1.Controls.Add(this.labelCategory);
			this.groupBox1.Controls.Add(this.labelSupplyDescript);
			this.groupBox1.Controls.Add(this.textSupplyNum);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Location = new System.Drawing.Point(684, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(235, 108);
			this.groupBox1.TabIndex = 34;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Supply Filters";
			// 
			// textSupplier
			// 
			this.textSupplier.Location = new System.Drawing.Point(98, 61);
			this.textSupplier.MaxLength = 15;
			this.textSupplier.Name = "textSupplier";
			this.textSupplier.Size = new System.Drawing.Size(131, 20);
			this.textSupplier.TabIndex = 2;
			// 
			// labelSupplier
			// 
			this.labelSupplier.Location = new System.Drawing.Point(6, 61);
			this.labelSupplier.Name = "labelSupplier";
			this.labelSupplier.Size = new System.Drawing.Size(89, 18);
			this.labelSupplier.TabIndex = 31;
			this.labelSupplier.Text = "Supplier";
			this.labelSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplyDescript
			// 
			this.textSupplyDescript.Location = new System.Drawing.Point(98, 82);
			this.textSupplyDescript.MaxLength = 15;
			this.textSupplyDescript.Name = "textSupplyDescript";
			this.textSupplyDescript.Size = new System.Drawing.Size(131, 20);
			this.textSupplyDescript.TabIndex = 3;
			// 
			// labelSupplyDescript
			// 
			this.labelSupplyDescript.Location = new System.Drawing.Point(6, 82);
			this.labelSupplyDescript.Name = "labelSupplyDescript";
			this.labelSupplyDescript.Size = new System.Drawing.Size(89, 18);
			this.labelSupplyDescript.TabIndex = 29;
			this.labelSupplyDescript.Text = "Description";
			this.labelSupplyDescript.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplyNum
			// 
			this.textSupplyNum.Location = new System.Drawing.Point(98, 40);
			this.textSupplyNum.MaxLength = 15;
			this.textSupplyNum.Name = "textSupplyNum";
			this.textSupplyNum.Size = new System.Drawing.Size(131, 20);
			this.textSupplyNum.TabIndex = 1;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 18);
			this.label7.TabIndex = 27;
			this.label7.Text = "SupplyNum";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridEquip
			// 
			this.gridEquip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridEquip.Location = new System.Drawing.Point(590, 426);
			this.gridEquip.Name = "gridEquip";
			this.gridEquip.Size = new System.Drawing.Size(372, 180);
			this.gridEquip.TabIndex = 18;
			this.gridEquip.Title = "Equipment";
			this.gridEquip.TranslationName = "TableEquipment";
			// 
			// gridDispSupply
			// 
			this.gridDispSupply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridDispSupply.Location = new System.Drawing.Point(0, 426);
			this.gridDispSupply.Name = "gridDispSupply";
			this.gridDispSupply.Size = new System.Drawing.Size(378, 180);
			this.gridDispSupply.TabIndex = 17;
			this.gridDispSupply.Title = "Dispensed Supply";
			this.gridDispSupply.TranslationName = "TableDispensed";
			// 
			// gridSupply
			// 
			this.gridSupply.Location = new System.Drawing.Point(590, 114);
			this.gridSupply.Name = "gridSupply";
			this.gridSupply.Size = new System.Drawing.Size(372, 310);
			this.gridSupply.TabIndex = 16;
			this.gridSupply.Title = "Supply";
			this.gridSupply.TranslationName = "TableSupply";
			// 
			// gridStudents
			// 
			this.gridStudents.Location = new System.Drawing.Point(0, 114);
			this.gridStudents.Name = "gridStudents";
			this.gridStudents.Size = new System.Drawing.Size(378, 310);
			this.gridStudents.TabIndex = 15;
			this.gridStudents.Title = "Students";
			this.gridStudents.TranslationName = "TableStudents";
			this.gridStudents.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridStudents_CellClick);
			// 
			// butScan
			// 
			this.butScan.Location = new System.Drawing.Point(449, 124);
			this.butScan.Name = "butScan";
			this.butScan.Size = new System.Drawing.Size(75, 24);
			this.butScan.TabIndex = 20;
			this.butScan.Text = "Scan";
			// 
			// butCheckOut
			// 
			this.butCheckOut.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butCheckOut.Location = new System.Drawing.Point(119, 84);
			this.butCheckOut.Name = "butCheckOut";
			this.butCheckOut.Size = new System.Drawing.Size(75, 24);
			this.butCheckOut.TabIndex = 33;
			this.butCheckOut.Text = "Check-Out";
			// 
			// butCheckIn
			// 
			this.butCheckIn.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butCheckIn.Location = new System.Drawing.Point(9, 84);
			this.butCheckIn.Name = "butCheckIn";
			this.butCheckIn.Size = new System.Drawing.Size(75, 24);
			this.butCheckIn.TabIndex = 32;
			this.butCheckIn.Text = "Check-In";
			// 
			// FormDispensary
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(974, 612);
			this.Controls.Add(this.butScan);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupSupply);
			this.Controls.Add(this.gridEquip);
			this.Controls.Add(this.gridDispSupply);
			this.Controls.Add(this.gridSupply);
			this.Controls.Add(this.groupDentalSchools);
			this.Controls.Add(this.gridStudents);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDispensary";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dispensary";
			this.Load += new System.EventHandler(this.FormDispensary_Load);
			this.groupDentalSchools.ResumeLayout(false);
			this.groupDentalSchools.PerformLayout();
			this.groupSupply.ResumeLayout(false);
			this.groupSupply.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupDentalSchools;
		private System.Windows.Forms.Label labelClass;
		private System.Windows.Forms.ComboBox comboClass;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textProvNum;
		private System.Windows.Forms.Label labelUniqueID;
		private UI.GridOD gridStudents;
		private UI.GridOD gridSupply;
		private UI.GridOD gridDispSupply;
		private UI.GridOD gridEquip;
		private System.Windows.Forms.GroupBox groupSupply;
		private System.Windows.Forms.Label labelCategory;
		private System.Windows.Forms.ComboBox comboCategory;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label labelSupDescript;
		private UI.Button butScan;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textSupplier;
		private System.Windows.Forms.Label labelSupplier;
		private System.Windows.Forms.TextBox textSupplyDescript;
		private System.Windows.Forms.Label labelSupplyDescript;
		private System.Windows.Forms.TextBox textSupplyNum;
		private System.Windows.Forms.Label label7;
		private UI.Button butCheckOut;
		private UI.Button butCheckIn;
	}
}