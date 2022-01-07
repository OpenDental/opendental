namespace OpenDental {
	partial class FormEhrLabSpecimenEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabSpecimenEdit));
			this.butCancel = new System.Windows.Forms.Button();
			this.butSave = new System.Windows.Forms.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label44 = new System.Windows.Forms.Label();
			this.textSpecimenTypeCodeSystemNameAlt = new System.Windows.Forms.TextBox();
			this.label43 = new System.Windows.Forms.Label();
			this.textSpecimenTypeCodeSystemName = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textSpecimenTypeText = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.textSpecimenTypeID = new System.Windows.Forms.TextBox();
			this.textSpecimenTypeTextOriginal = new System.Windows.Forms.TextBox();
			this.textSpecimenTypeTextAlt = new System.Windows.Forms.TextBox();
			this.label34 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.textSpecimenTypeIDAlt = new System.Windows.Forms.TextBox();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.textCollectionDateTimeStart = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textCollectionDateTimeEnd = new System.Windows.Forms.TextBox();
			this.gridReject = new OpenDental.UI.GridOD();
			this.gridCondition = new OpenDental.UI.GridOD();
			this.groupBox6.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(512, 357);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(431, 357);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 23);
			this.butSave.TabIndex = 10;
			this.butSave.Text = "Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label44);
			this.groupBox6.Controls.Add(this.textSpecimenTypeCodeSystemNameAlt);
			this.groupBox6.Controls.Add(this.label43);
			this.groupBox6.Controls.Add(this.textSpecimenTypeCodeSystemName);
			this.groupBox6.Controls.Add(this.label14);
			this.groupBox6.Controls.Add(this.textSpecimenTypeText);
			this.groupBox6.Controls.Add(this.label15);
			this.groupBox6.Controls.Add(this.label36);
			this.groupBox6.Controls.Add(this.textSpecimenTypeID);
			this.groupBox6.Controls.Add(this.textSpecimenTypeTextOriginal);
			this.groupBox6.Controls.Add(this.textSpecimenTypeTextAlt);
			this.groupBox6.Controls.Add(this.label34);
			this.groupBox6.Controls.Add(this.label35);
			this.groupBox6.Controls.Add(this.textSpecimenTypeIDAlt);
			this.groupBox6.Location = new System.Drawing.Point(12, 77);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(274, 170);
			this.groupBox6.TabIndex = 12;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Specimen Type Identifiers";
			// 
			// label44
			// 
			this.label44.Location = new System.Drawing.Point(6, 81);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(127, 17);
			this.label44.TabIndex = 256;
			this.label44.Text = "Alt Code System";
			this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenTypeCodeSystemNameAlt
			// 
			this.textSpecimenTypeCodeSystemNameAlt.Location = new System.Drawing.Point(134, 80);
			this.textSpecimenTypeCodeSystemNameAlt.Name = "textSpecimenTypeCodeSystemNameAlt";
			this.textSpecimenTypeCodeSystemNameAlt.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeCodeSystemNameAlt.TabIndex = 3;
			// 
			// label43
			// 
			this.label43.Location = new System.Drawing.Point(6, 18);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(127, 17);
			this.label43.TabIndex = 254;
			this.label43.Text = "Code System";
			this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenTypeCodeSystemName
			// 
			this.textSpecimenTypeCodeSystemName.Location = new System.Drawing.Point(134, 17);
			this.textSpecimenTypeCodeSystemName.Name = "textSpecimenTypeCodeSystemName";
			this.textSpecimenTypeCodeSystemName.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeCodeSystemName.TabIndex = 0;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 39);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(127, 17);
			this.label14.TabIndex = 2;
			this.label14.Text = "Observation ID";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenTypeText
			// 
			this.textSpecimenTypeText.Location = new System.Drawing.Point(134, 59);
			this.textSpecimenTypeText.Multiline = true;
			this.textSpecimenTypeText.Name = "textSpecimenTypeText";
			this.textSpecimenTypeText.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeText.TabIndex = 2;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(6, 60);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(128, 17);
			this.label15.TabIndex = 228;
			this.label15.Text = "Observation Text";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label36
			// 
			this.label36.Location = new System.Drawing.Point(6, 145);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(128, 17);
			this.label36.TabIndex = 250;
			this.label36.Text = "Original Text";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenTypeID
			// 
			this.textSpecimenTypeID.Location = new System.Drawing.Point(134, 38);
			this.textSpecimenTypeID.Name = "textSpecimenTypeID";
			this.textSpecimenTypeID.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeID.TabIndex = 1;
			// 
			// textSpecimenTypeTextOriginal
			// 
			this.textSpecimenTypeTextOriginal.Location = new System.Drawing.Point(134, 143);
			this.textSpecimenTypeTextOriginal.Multiline = true;
			this.textSpecimenTypeTextOriginal.Name = "textSpecimenTypeTextOriginal";
			this.textSpecimenTypeTextOriginal.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeTextOriginal.TabIndex = 6;
			// 
			// textSpecimenTypeTextAlt
			// 
			this.textSpecimenTypeTextAlt.Location = new System.Drawing.Point(134, 122);
			this.textSpecimenTypeTextAlt.Multiline = true;
			this.textSpecimenTypeTextAlt.Name = "textSpecimenTypeTextAlt";
			this.textSpecimenTypeTextAlt.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeTextAlt.TabIndex = 5;
			// 
			// label34
			// 
			this.label34.Location = new System.Drawing.Point(6, 102);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(127, 17);
			this.label34.TabIndex = 246;
			this.label34.Text = "Alt Observation ID";
			this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label35
			// 
			this.label35.Location = new System.Drawing.Point(5, 123);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(128, 17);
			this.label35.TabIndex = 248;
			this.label35.Text = "Alt Observation Text";
			this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenTypeIDAlt
			// 
			this.textSpecimenTypeIDAlt.Location = new System.Drawing.Point(134, 101);
			this.textSpecimenTypeIDAlt.Name = "textSpecimenTypeIDAlt";
			this.textSpecimenTypeIDAlt.Size = new System.Drawing.Size(135, 20);
			this.textSpecimenTypeIDAlt.TabIndex = 4;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.textCollectionDateTimeStart);
			this.groupBox8.Controls.Add(this.label17);
			this.groupBox8.Controls.Add(this.label18);
			this.groupBox8.Controls.Add(this.textCollectionDateTimeEnd);
			this.groupBox8.Location = new System.Drawing.Point(12, 12);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(274, 59);
			this.groupBox8.TabIndex = 263;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Collection Times";
			// 
			// textCollectionDateTimeStart
			// 
			this.textCollectionDateTimeStart.Location = new System.Drawing.Point(134, 13);
			this.textCollectionDateTimeStart.Name = "textCollectionDateTimeStart";
			this.textCollectionDateTimeStart.Size = new System.Drawing.Size(135, 20);
			this.textCollectionDateTimeStart.TabIndex = 0;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(6, 15);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(127, 17);
			this.label17.TabIndex = 263;
			this.label17.Text = "Date/Time Start";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(6, 36);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(127, 17);
			this.label18.TabIndex = 261;
			this.label18.Text = "Date/Time Stop";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCollectionDateTimeEnd
			// 
			this.textCollectionDateTimeEnd.Location = new System.Drawing.Point(134, 34);
			this.textCollectionDateTimeEnd.Name = "textCollectionDateTimeEnd";
			this.textCollectionDateTimeEnd.Size = new System.Drawing.Size(135, 20);
			this.textCollectionDateTimeEnd.TabIndex = 1;
			// 
			// gridReject
			// 
			this.gridReject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridReject.Location = new System.Drawing.Point(292, 12);
			this.gridReject.Name = "gridReject";
			this.gridReject.Size = new System.Drawing.Size(295, 166);
			this.gridReject.TabIndex = 11;
			this.gridReject.Title = "Reject Reasons";
			this.gridReject.TranslationName = "TableReject";
			// 
			// gridCondition
			// 
			this.gridCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridCondition.Location = new System.Drawing.Point(292, 185);
			this.gridCondition.Name = "gridCondition";
			this.gridCondition.Size = new System.Drawing.Size(295, 166);
			this.gridCondition.TabIndex = 5;
			this.gridCondition.Title = "Condition Codes";
			this.gridCondition.TranslationName = "TableCodes";
			// 
			// FormEhrLabSpecimenEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(599, 392);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.gridReject);
			this.Controls.Add(this.gridCondition);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabSpecimenEdit";
			this.Text = "Lab Specimen";
			this.Load += new System.EventHandler(this.FormEhrLabOrders_Load);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridCondition;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butSave;
		private UI.GridOD gridReject;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.TextBox textSpecimenTypeCodeSystemNameAlt;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.TextBox textSpecimenTypeCodeSystemName;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textSpecimenTypeText;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.TextBox textSpecimenTypeID;
		private System.Windows.Forms.TextBox textSpecimenTypeTextOriginal;
		private System.Windows.Forms.TextBox textSpecimenTypeTextAlt;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.TextBox textSpecimenTypeIDAlt;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.TextBox textCollectionDateTimeStart;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textCollectionDateTimeEnd;
	}
}