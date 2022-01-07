namespace OpenDental {
	partial class FormPatListElementEditEHR2014 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatListElementEditEHR2014));
			this.labelOperand = new System.Windows.Forms.Label();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.labelRestriction = new System.Windows.Forms.Label();
			this.labelCompareString = new System.Windows.Forms.Label();
			this.textCompareString = new System.Windows.Forms.TextBox();
			this.labelLabValue = new System.Windows.Forms.Label();
			this.textLabValue = new System.Windows.Forms.TextBox();
			this.listRestriction = new OpenDental.UI.ListBoxOD();
			this.listOperand = new OpenDental.UI.ListBoxOD();
			this.labelExample = new System.Windows.Forms.Label();
			this.labelAfterDate = new System.Windows.Forms.Label();
			this.labelBeforeDate = new System.Windows.Forms.Label();
			this.butPicker = new System.Windows.Forms.Button();
			this.butSNOMED = new System.Windows.Forms.Button();
			this.textSNOMED = new System.Windows.Forms.TextBox();
			this.labelSNOMED = new System.Windows.Forms.Label();
			this.butProblem = new System.Windows.Forms.Button();
			this.textDateStop = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelProblemSuggest = new System.Windows.Forms.Label();
			this.labelLabValueType = new System.Windows.Forms.Label();
			this.comboUnits = new System.Windows.Forms.ComboBox();
			this.comboLabValueType = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// labelOperand
			// 
			this.labelOperand.Location = new System.Drawing.Point(123, 113);
			this.labelOperand.Name = "labelOperand";
			this.labelOperand.Size = new System.Drawing.Size(61, 17);
			this.labelOperand.TabIndex = 10;
			this.labelOperand.Text = "Operand";
			this.labelOperand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(409, 337);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(328, 337);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(24, 337);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 8;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelRestriction
			// 
			this.labelRestriction.Location = new System.Drawing.Point(109, 12);
			this.labelRestriction.Name = "labelRestriction";
			this.labelRestriction.Size = new System.Drawing.Size(75, 17);
			this.labelRestriction.TabIndex = 13;
			this.labelRestriction.Text = "Restriction";
			this.labelRestriction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCompareString
			// 
			this.labelCompareString.Location = new System.Drawing.Point(7, 163);
			this.labelCompareString.Name = "labelCompareString";
			this.labelCompareString.Size = new System.Drawing.Size(177, 17);
			this.labelCompareString.TabIndex = 9;
			this.labelCompareString.Text = "Compare string";
			this.labelCompareString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCompareString
			// 
			this.textCompareString.Location = new System.Drawing.Point(185, 162);
			this.textCompareString.Name = "textCompareString";
			this.textCompareString.Size = new System.Drawing.Size(137, 20);
			this.textCompareString.TabIndex = 1;
			this.textCompareString.TextChanged += new System.EventHandler(this.textCompareString_TextChanged);
			// 
			// labelLabValue
			// 
			this.labelLabValue.Location = new System.Drawing.Point(60, 212);
			this.labelLabValue.Name = "labelLabValue";
			this.labelLabValue.Size = new System.Drawing.Size(124, 17);
			this.labelLabValue.TabIndex = 14;
			this.labelLabValue.Text = "Lab value";
			this.labelLabValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelLabValue.Visible = false;
			// 
			// textLabValue
			// 
			this.textLabValue.Location = new System.Drawing.Point(185, 210);
			this.textLabValue.Name = "textLabValue";
			this.textLabValue.Size = new System.Drawing.Size(137, 20);
			this.textLabValue.TabIndex = 2;
			this.textLabValue.Visible = false;
			// 
			// listRestriction
			// 
			this.listRestriction.Items.AddRange(new object[] {
            "Birthdate",
            "Disease",
            "Medication",
            "Lab result",
            "Gender",
            "Allergy",
            "Comm Pref"});
			this.listRestriction.Location = new System.Drawing.Point(185, 12);
			this.listRestriction.Name = "listRestriction";
			this.listRestriction.Size = new System.Drawing.Size(75, 95);
			this.listRestriction.TabIndex = 18;
			this.listRestriction.SelectedIndexChanged += new System.EventHandler(this.listRestriction_SelectedIndexChanged);
			// 
			// listOperand
			// 
			this.listOperand.Items.AddRange(new object[] {
            "GreaterThan",
            "LessThan",
            "Equals"});
			this.listOperand.Location = new System.Drawing.Point(185, 113);
			this.listOperand.Name = "listOperand";
			this.listOperand.Size = new System.Drawing.Size(75, 43);
			this.listOperand.TabIndex = 19;
			// 
			// labelExample
			// 
			this.labelExample.Location = new System.Drawing.Point(362, 163);
			this.labelExample.Name = "labelExample";
			this.labelExample.Size = new System.Drawing.Size(119, 17);
			this.labelExample.TabIndex = 20;
			this.labelExample.Text = "Example";
			this.labelExample.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAfterDate
			// 
			this.labelAfterDate.Location = new System.Drawing.Point(7, 267);
			this.labelAfterDate.Name = "labelAfterDate";
			this.labelAfterDate.Size = new System.Drawing.Size(177, 17);
			this.labelAfterDate.TabIndex = 22;
			this.labelAfterDate.Text = "After Date";
			this.labelAfterDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBeforeDate
			// 
			this.labelBeforeDate.Location = new System.Drawing.Point(7, 289);
			this.labelBeforeDate.Name = "labelBeforeDate";
			this.labelBeforeDate.Size = new System.Drawing.Size(177, 17);
			this.labelBeforeDate.TabIndex = 24;
			this.labelBeforeDate.Text = "Before Date";
			this.labelBeforeDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPicker
			// 
			this.butPicker.Location = new System.Drawing.Point(328, 160);
			this.butPicker.Name = "butPicker";
			this.butPicker.Size = new System.Drawing.Size(28, 23);
			this.butPicker.TabIndex = 25;
			this.butPicker.Text = "...";
			this.butPicker.UseVisualStyleBackColor = true;
			this.butPicker.Click += new System.EventHandler(this.butPicker_Click);
			// 
			// butSNOMED
			// 
			this.butSNOMED.Location = new System.Drawing.Point(328, 184);
			this.butSNOMED.Name = "butSNOMED";
			this.butSNOMED.Size = new System.Drawing.Size(80, 23);
			this.butSNOMED.TabIndex = 31;
			this.butSNOMED.Text = "SNOMED CT";
			this.butSNOMED.UseVisualStyleBackColor = true;
			this.butSNOMED.Click += new System.EventHandler(this.butPicker_Click);
			// 
			// textSNOMED
			// 
			this.textSNOMED.Location = new System.Drawing.Point(185, 186);
			this.textSNOMED.Name = "textSNOMED";
			this.textSNOMED.Size = new System.Drawing.Size(137, 20);
			this.textSNOMED.TabIndex = 32;
			this.textSNOMED.TextChanged += new System.EventHandler(this.textSNOMED_TextChanged);
			// 
			// labelSNOMED
			// 
			this.labelSNOMED.Location = new System.Drawing.Point(7, 187);
			this.labelSNOMED.Name = "labelSNOMED";
			this.labelSNOMED.Size = new System.Drawing.Size(177, 17);
			this.labelSNOMED.TabIndex = 33;
			this.labelSNOMED.Text = "SNOMED CT Code";
			this.labelSNOMED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butProblem
			// 
			this.butProblem.Location = new System.Drawing.Point(414, 184);
			this.butProblem.Name = "butProblem";
			this.butProblem.Size = new System.Drawing.Size(70, 23);
			this.butProblem.TabIndex = 34;
			this.butProblem.Text = "Problem";
			this.butProblem.UseVisualStyleBackColor = true;
			this.butProblem.Click += new System.EventHandler(this.butProblem_Click);
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(185, 288);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(137, 20);
			this.textDateStop.TabIndex = 27;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(185, 264);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(137, 20);
			this.textDateStart.TabIndex = 26;
			// 
			// labelProblemSuggest
			// 
			this.labelProblemSuggest.Location = new System.Drawing.Point(328, 243);
			this.labelProblemSuggest.Name = "labelProblemSuggest";
			this.labelProblemSuggest.Size = new System.Drawing.Size(146, 65);
			this.labelProblemSuggest.TabIndex = 35;
			this.labelProblemSuggest.Text = "Note: Picking from problem list above will provide the best results.";
			this.labelProblemSuggest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelLabValueType
			// 
			this.labelLabValueType.Location = new System.Drawing.Point(60, 238);
			this.labelLabValueType.Name = "labelLabValueType";
			this.labelLabValueType.Size = new System.Drawing.Size(124, 17);
			this.labelLabValueType.TabIndex = 37;
			this.labelLabValueType.Text = "Lab Value Type";
			this.labelLabValueType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelLabValueType.Visible = false;
			// 
			// comboUnits
			// 
			this.comboUnits.FormattingEnabled = true;
			this.comboUnits.Location = new System.Drawing.Point(328, 209);
			this.comboUnits.Name = "comboUnits";
			this.comboUnits.Size = new System.Drawing.Size(80, 21);
			this.comboUnits.TabIndex = 226;
			// 
			// comboLabValueType
			// 
			this.comboLabValueType.FormattingEnabled = true;
			this.comboLabValueType.Location = new System.Drawing.Point(185, 237);
			this.comboLabValueType.Name = "comboLabValueType";
			this.comboLabValueType.Size = new System.Drawing.Size(137, 21);
			this.comboLabValueType.TabIndex = 227;
			// 
			// FormPatListElementEditEHR2014
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(496, 372);
			this.Controls.Add(this.comboLabValueType);
			this.Controls.Add(this.comboUnits);
			this.Controls.Add(this.labelLabValueType);
			this.Controls.Add(this.labelProblemSuggest);
			this.Controls.Add(this.butProblem);
			this.Controls.Add(this.labelSNOMED);
			this.Controls.Add(this.textSNOMED);
			this.Controls.Add(this.butSNOMED);
			this.Controls.Add(this.textDateStop);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.butPicker);
			this.Controls.Add(this.labelBeforeDate);
			this.Controls.Add(this.labelAfterDate);
			this.Controls.Add(this.labelExample);
			this.Controls.Add(this.listOperand);
			this.Controls.Add(this.listRestriction);
			this.Controls.Add(this.textLabValue);
			this.Controls.Add(this.labelLabValue);
			this.Controls.Add(this.labelRestriction);
			this.Controls.Add(this.textCompareString);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelCompareString);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelOperand);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatListElementEditEHR2014";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PatList Element Edit";
			this.Load += new System.EventHandler(this.FormPatListElementEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelOperand;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Label labelRestriction;
		private System.Windows.Forms.Label labelCompareString;
		private System.Windows.Forms.TextBox textCompareString;
		private System.Windows.Forms.Label labelLabValue;
		private System.Windows.Forms.TextBox textLabValue;
		private OpenDental.UI.ListBoxOD listRestriction;
		private OpenDental.UI.ListBoxOD listOperand;
		private System.Windows.Forms.Label labelExample;
		private System.Windows.Forms.Label labelAfterDate;
		private System.Windows.Forms.Label labelBeforeDate;
		private System.Windows.Forms.Button butPicker;
		private ValidDate textDateStart;
		private ValidDate textDateStop;
		private System.Windows.Forms.Button butSNOMED;
		private System.Windows.Forms.TextBox textSNOMED;
		private System.Windows.Forms.Label labelSNOMED;
		private System.Windows.Forms.Button butProblem;
		private System.Windows.Forms.Label labelProblemSuggest;
		private System.Windows.Forms.Label labelLabValueType;
		private System.Windows.Forms.ComboBox comboUnits;
		private System.Windows.Forms.ComboBox comboLabValueType;
	}
}