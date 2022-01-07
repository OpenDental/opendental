namespace OpenDental{
	partial class FormVaccineObsEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVaccineObsEdit));
			this.listValueType = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboObservationQuestion = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textValue = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listCodeSystem = new OpenDental.UI.ListBoxOD();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labelCodeSystemNote = new System.Windows.Forms.Label();
			this.labelValueUnitsNote = new System.Windows.Forms.Label();
			this.textMethodCode = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.labelMethodCodeNote = new System.Windows.Forms.Label();
			this.comboUnits = new System.Windows.Forms.ComboBox();
			this.butDelete = new OpenDental.UI.Button();
			this.textDateObserved = new OpenDental.ValidDate();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butTodayDateObserved = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listValueType
			// 
			this.listValueType.Location = new System.Drawing.Point(144, 40);
			this.listValueType.Name = "listValueType";
			this.listValueType.Size = new System.Drawing.Size(113, 69);
			this.listValueType.TabIndex = 4;
			this.listValueType.SelectedIndexChanged += new System.EventHandler(this.listValueType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Value Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "Observation Question";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboObservationQuestion
			// 
			this.comboObservationQuestion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboObservationQuestion.FormattingEnabled = true;
			this.comboObservationQuestion.Location = new System.Drawing.Point(144, 16);
			this.comboObservationQuestion.Name = "comboObservationQuestion";
			this.comboObservationQuestion.Size = new System.Drawing.Size(186, 21);
			this.comboObservationQuestion.TabIndex = 7;
			this.comboObservationQuestion.SelectionChangeCommitted += new System.EventHandler(this.comboObservationQuestion_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 164);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(140, 17);
			this.label3.TabIndex = 8;
			this.label3.Text = "Value";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textValue
			// 
			this.textValue.Location = new System.Drawing.Point(144, 164);
			this.textValue.Name = "textValue";
			this.textValue.Size = new System.Drawing.Size(113, 20);
			this.textValue.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(140, 17);
			this.label4.TabIndex = 11;
			this.label4.Text = "Value Code System";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listCodeSystem
			// 
			this.listCodeSystem.Location = new System.Drawing.Point(144, 115);
			this.listCodeSystem.Name = "listCodeSystem";
			this.listCodeSystem.Size = new System.Drawing.Size(113, 43);
			this.listCodeSystem.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 190);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(140, 17);
			this.label5.TabIndex = 12;
			this.label5.Text = "Value Units";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 216);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(140, 17);
			this.label6.TabIndex = 14;
			this.label6.Text = "Date Observed";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCodeSystemNote
			// 
			this.labelCodeSystemNote.Location = new System.Drawing.Point(263, 115);
			this.labelCodeSystemNote.Name = "labelCodeSystemNote";
			this.labelCodeSystemNote.Size = new System.Drawing.Size(310, 17);
			this.labelCodeSystemNote.TabIndex = 16;
			this.labelCodeSystemNote.Text = "(Only needed if Value Type is Coded)";
			this.labelCodeSystemNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelValueUnitsNote
			// 
			this.labelValueUnitsNote.Location = new System.Drawing.Point(263, 190);
			this.labelValueUnitsNote.Name = "labelValueUnitsNote";
			this.labelValueUnitsNote.Size = new System.Drawing.Size(310, 17);
			this.labelValueUnitsNote.TabIndex = 17;
			this.labelValueUnitsNote.Text = "(Only needed if Value Type is Numeric)";
			this.labelValueUnitsNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textMethodCode
			// 
			this.textMethodCode.Location = new System.Drawing.Point(144, 242);
			this.textMethodCode.Name = "textMethodCode";
			this.textMethodCode.Size = new System.Drawing.Size(113, 20);
			this.textMethodCode.TabIndex = 19;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(3, 242);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(140, 17);
			this.label7.TabIndex = 18;
			this.label7.Text = "Method Code";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMethodCodeNote
			// 
			this.labelMethodCodeNote.Location = new System.Drawing.Point(263, 242);
			this.labelMethodCodeNote.Name = "labelMethodCodeNote";
			this.labelMethodCodeNote.Size = new System.Drawing.Size(310, 17);
			this.labelMethodCodeNote.TabIndex = 20;
			this.labelMethodCodeNote.Text = "(Only needed if Question is FundPgmEligCat)";
			this.labelMethodCodeNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboUnits
			// 
			this.comboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnits.FormattingEnabled = true;
			this.comboUnits.Location = new System.Drawing.Point(144, 190);
			this.comboUnits.Name = "comboUnits";
			this.comboUnits.Size = new System.Drawing.Size(113, 21);
			this.comboUnits.TabIndex = 282;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 286);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 281;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDateObserved
			// 
			this.textDateObserved.Location = new System.Drawing.Point(144, 216);
			this.textDateObserved.Name = "textDateObserved";
			this.textDateObserved.Size = new System.Drawing.Size(113, 20);
			this.textDateObserved.TabIndex = 15;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(419, 286);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(500, 286);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butTodayDateObserved
			// 
			this.butTodayDateObserved.Location = new System.Drawing.Point(260, 216);
			this.butTodayDateObserved.Name = "butTodayDateObserved";
			this.butTodayDateObserved.Size = new System.Drawing.Size(51, 20);
			this.butTodayDateObserved.TabIndex = 283;
			this.butTodayDateObserved.Text = "Today";
			this.butTodayDateObserved.Click += new System.EventHandler(this.butTodayDateObserved_Click);
			// 
			// FormVaccineObsEdit
			// 
			this.ClientSize = new System.Drawing.Size(587, 322);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butTodayDateObserved);
			this.Controls.Add(this.comboUnits);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelMethodCodeNote);
			this.Controls.Add(this.textMethodCode);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.labelValueUnitsNote);
			this.Controls.Add(this.labelCodeSystemNote);
			this.Controls.Add(this.textDateObserved);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listCodeSystem);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboObservationQuestion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listValueType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormVaccineObsEdit";
			this.Text = "Edit Vaccine Observation";
			this.Load += new System.EventHandler(this.FormVaccineObsEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listValueType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboObservationQuestion;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textValue;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.ListBoxOD listCodeSystem;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private ValidDate textDateObserved;
		private System.Windows.Forms.Label labelCodeSystemNote;
		private System.Windows.Forms.Label labelValueUnitsNote;
		private System.Windows.Forms.TextBox textMethodCode;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelMethodCodeNote;
		private UI.Button butDelete;
		private System.Windows.Forms.ComboBox comboUnits;
		private UI.Button butTodayDateObserved;
	}
}