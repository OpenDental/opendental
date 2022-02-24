namespace OpenDental{
	partial class FormAllergyDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAllergyDefEdit));
			this.labelDescription = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboSnomedAllergyType = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textMedication = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textUnii = new System.Windows.Forms.TextBox();
			this.butNoneUnii = new OpenDental.UI.Button();
			this.butNone = new OpenDental.UI.Button();
			this.butUniiSelect = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butMedicationSelect = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(18, 27);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(130, 20);
			this.labelDescription.TabIndex = 6;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(151, 27);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(276, 20);
			this.textDescription.TabIndex = 7;
			// 
			// checkHidden
			// 
			this.checkHidden.Location = new System.Drawing.Point(33, 207);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkHidden.Size = new System.Drawing.Size(132, 24);
			this.checkHidden.TabIndex = 8;
			this.checkHidden.Text = "Is Hidden";
			this.checkHidden.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(109, 20);
			this.label2.TabIndex = 20;
			this.label2.Text = "Medication";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSnomedAllergyType
			// 
			this.comboSnomedAllergyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSnomedAllergyType.FormattingEnabled = true;
			this.comboSnomedAllergyType.Location = new System.Drawing.Point(133, 22);
			this.comboSnomedAllergyType.Name = "comboSnomedAllergyType";
			this.comboSnomedAllergyType.Size = new System.Drawing.Size(276, 21);
			this.comboSnomedAllergyType.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(123, 20);
			this.label3.TabIndex = 18;
			this.label3.Text = "Allergy Type";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMedication
			// 
			this.textMedication.Location = new System.Drawing.Point(118, 49);
			this.textMedication.Name = "textMedication";
			this.textMedication.ReadOnly = true;
			this.textMedication.Size = new System.Drawing.Size(276, 20);
			this.textMedication.TabIndex = 7;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.comboSnomedAllergyType);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(18, 57);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(518, 144);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Only used in EHR for CCDs.  Most offices can ignore this section";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textUnii);
			this.groupBox2.Controls.Add(this.butNoneUnii);
			this.groupBox2.Controls.Add(this.butNone);
			this.groupBox2.Controls.Add(this.butUniiSelect);
			this.groupBox2.Controls.Add(this.textMedication);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.butMedicationSelect);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(15, 55);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(493, 83);
			this.groupBox2.TabIndex = 26;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Allergen (only one)";
			// 
			// textUnii
			// 
			this.textUnii.Location = new System.Drawing.Point(118, 25);
			this.textUnii.Name = "textUnii";
			this.textUnii.Size = new System.Drawing.Size(276, 20);
			this.textUnii.TabIndex = 21;
			// 
			// butNoneUnii
			// 
			this.butNoneUnii.Enabled = false;
			this.butNoneUnii.Location = new System.Drawing.Point(423, 24);
			this.butNoneUnii.Name = "butNoneUnii";
			this.butNoneUnii.Size = new System.Drawing.Size(51, 22);
			this.butNoneUnii.TabIndex = 24;
			this.butNoneUnii.Text = "None";
			this.butNoneUnii.Click += new System.EventHandler(this.butNoneUniiTo_Click);
			// 
			// butNone
			// 
			this.butNone.Location = new System.Drawing.Point(423, 48);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(51, 22);
			this.butNone.TabIndex = 9;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butUniiSelect
			// 
			this.butUniiSelect.Enabled = false;
			this.butUniiSelect.Location = new System.Drawing.Point(398, 24);
			this.butUniiSelect.Name = "butUniiSelect";
			this.butUniiSelect.Size = new System.Drawing.Size(22, 22);
			this.butUniiSelect.TabIndex = 23;
			this.butUniiSelect.Text = "...";
			this.butUniiSelect.Click += new System.EventHandler(this.butUniiToSelect_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(109, 20);
			this.label1.TabIndex = 22;
			this.label1.Text = "UNII";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butMedicationSelect
			// 
			this.butMedicationSelect.Location = new System.Drawing.Point(398, 48);
			this.butMedicationSelect.Name = "butMedicationSelect";
			this.butMedicationSelect.Size = new System.Drawing.Size(22, 22);
			this.butMedicationSelect.TabIndex = 3;
			this.butMedicationSelect.Text = "...";
			this.butMedicationSelect.Click += new System.EventHandler(this.butMedicationSelect_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(480, 245);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(399, 245);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 245);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormAllergyDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(571, 284);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAllergyDefEdit";
			this.Text = "Allergy Def Edit";
			this.Load += new System.EventHandler(this.FormAllergyEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.CheckBox checkHidden;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboSnomedAllergyType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textMedication;
		private UI.Button butMedicationSelect;
		private UI.Button butNone;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textUnii;
		private UI.Button butUniiSelect;
		private UI.Button butNoneUnii;
		private System.Windows.Forms.GroupBox groupBox2;
	}
}