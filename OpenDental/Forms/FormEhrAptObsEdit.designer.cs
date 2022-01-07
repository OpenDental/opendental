namespace OpenDental{
	partial class FormEhrAptObsEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrAptObsEdit));
			this.comboUnits = new System.Windows.Forms.ComboBox();
			this.labelValueUnitsNote = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textValue = new System.Windows.Forms.TextBox();
			this.labelValue = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listValueType = new OpenDental.UI.ListBoxOD();
			this.butPickValueIcd10 = new OpenDental.UI.Button();
			this.butPickValueIcd9 = new OpenDental.UI.Button();
			this.butPickValueSnomedct = new OpenDental.UI.Button();
			this.butPickValueLoinc = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboObservationQuestion = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboUnits
			// 
			this.comboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnits.FormattingEnabled = true;
			this.comboUnits.Location = new System.Drawing.Point(143, 165);
			this.comboUnits.Name = "comboUnits";
			this.comboUnits.Size = new System.Drawing.Size(113, 21);
			this.comboUnits.TabIndex = 303;
			// 
			// labelValueUnitsNote
			// 
			this.labelValueUnitsNote.Location = new System.Drawing.Point(262, 165);
			this.labelValueUnitsNote.Name = "labelValueUnitsNote";
			this.labelValueUnitsNote.Size = new System.Drawing.Size(260, 17);
			this.labelValueUnitsNote.TabIndex = 298;
			this.labelValueUnitsNote.Text = "(Only needed if Value Type is Numeric)";
			this.labelValueUnitsNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 165);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(131, 17);
			this.label5.TabIndex = 294;
			this.label5.Text = "Value Units";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textValue
			// 
			this.textValue.Location = new System.Drawing.Point(143, 139);
			this.textValue.Name = "textValue";
			this.textValue.Size = new System.Drawing.Size(309, 20);
			this.textValue.TabIndex = 291;
			// 
			// labelValue
			// 
			this.labelValue.Location = new System.Drawing.Point(8, 139);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(134, 17);
			this.labelValue.TabIndex = 290;
			this.labelValue.Text = "Value";
			this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(137, 17);
			this.label1.TabIndex = 287;
			this.label1.Text = "Value Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listValueType
			// 
			this.listValueType.Location = new System.Drawing.Point(143, 64);
			this.listValueType.Name = "listValueType";
			this.listValueType.Size = new System.Drawing.Size(113, 69);
			this.listValueType.TabIndex = 286;
			this.listValueType.SelectedIndexChanged += new System.EventHandler(this.listValueType_SelectedIndexChanged);
			// 
			// butPickValueIcd10
			// 
			this.butPickValueIcd10.Location = new System.Drawing.Point(339, 99);
			this.butPickValueIcd10.Name = "butPickValueIcd10";
			this.butPickValueIcd10.Size = new System.Drawing.Size(48, 20);
			this.butPickValueIcd10.TabIndex = 312;
			this.butPickValueIcd10.Text = "ICD10";
			this.butPickValueIcd10.Click += new System.EventHandler(this.butPickValueIcd10_Click);
			// 
			// butPickValueIcd9
			// 
			this.butPickValueIcd9.Location = new System.Drawing.Point(339, 73);
			this.butPickValueIcd9.Name = "butPickValueIcd9";
			this.butPickValueIcd9.Size = new System.Drawing.Size(48, 20);
			this.butPickValueIcd9.TabIndex = 311;
			this.butPickValueIcd9.Text = "ICD9";
			this.butPickValueIcd9.Click += new System.EventHandler(this.butPickValueIcd9_Click);
			// 
			// butPickValueSnomedct
			// 
			this.butPickValueSnomedct.Location = new System.Drawing.Point(259, 99);
			this.butPickValueSnomedct.Name = "butPickValueSnomedct";
			this.butPickValueSnomedct.Size = new System.Drawing.Size(74, 20);
			this.butPickValueSnomedct.TabIndex = 310;
			this.butPickValueSnomedct.Text = "SNOMEDCT";
			this.butPickValueSnomedct.Click += new System.EventHandler(this.butPickValueSnomedct_Click);
			// 
			// butPickValueLoinc
			// 
			this.butPickValueLoinc.Location = new System.Drawing.Point(259, 73);
			this.butPickValueLoinc.Name = "butPickValueLoinc";
			this.butPickValueLoinc.Size = new System.Drawing.Size(74, 20);
			this.butPickValueLoinc.TabIndex = 309;
			this.butPickValueLoinc.Text = "LOINC";
			this.butPickValueLoinc.Click += new System.EventHandler(this.butPickValueLoinc_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(5, 236);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 302;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(362, 234);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 285;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(443, 234);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 284;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboObservationQuestion
			// 
			this.comboObservationQuestion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboObservationQuestion.FormattingEnabled = true;
			this.comboObservationQuestion.Location = new System.Drawing.Point(143, 37);
			this.comboObservationQuestion.Name = "comboObservationQuestion";
			this.comboObservationQuestion.Size = new System.Drawing.Size(113, 21);
			this.comboObservationQuestion.TabIndex = 314;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 17);
			this.label2.TabIndex = 313;
			this.label2.Text = "Observation Question";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEhrAptObsEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(526, 272);
			this.Controls.Add(this.comboObservationQuestion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butPickValueIcd10);
			this.Controls.Add(this.butPickValueIcd9);
			this.Controls.Add(this.butPickValueSnomedct);
			this.Controls.Add(this.butPickValueLoinc);
			this.Controls.Add(this.comboUnits);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelValueUnitsNote);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.labelValue);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listValueType);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrAptObsEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Appointment Observation";
			this.Load += new System.EventHandler(this.FormEhrAptObsEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboUnits;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelValueUnitsNote;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textValue;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.Label label1;
		private UI.ListBoxOD listValueType;
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.Button butPickValueLoinc;
		private UI.Button butPickValueSnomedct;
		private UI.Button butPickValueIcd9;
		private UI.Button butPickValueIcd10;
		private System.Windows.Forms.ComboBox comboObservationQuestion;
		private System.Windows.Forms.Label label2;

	}
}