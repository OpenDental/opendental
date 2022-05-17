namespace OpenDental{
	partial class FormInterventionEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInterventionEdit));
			this.textDate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboCodeSet = new System.Windows.Forms.ComboBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textNote = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.checkPatientDeclined = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(90, 13);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(80, 20);
			this.textDate.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Date";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(12, 441);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(78, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "Note";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(176, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Code Set";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCodeSet
			// 
			this.comboCodeSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCodeSet.DropDownWidth = 125;
			this.comboCodeSet.Location = new System.Drawing.Point(257, 12);
			this.comboCodeSet.MaxDropDownItems = 30;
			this.comboCodeSet.Name = "comboCodeSet";
			this.comboCodeSet.Size = new System.Drawing.Size(180, 21);
			this.comboCodeSet.TabIndex = 2;
			this.comboCodeSet.SelectionChangeCommitted += new System.EventHandler(this.comboCodeSet_SelectionChangeCommitted);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 39);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(628, 394);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Intervention Codes";
			this.gridMain.TranslationName = "TableIntervention";
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.Location = new System.Drawing.Point(90, 439);
			this.textNote.MaxLength = 2147483647;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(550, 60);
			this.textNote.TabIndex = 4;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(565, 513);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(484, 513);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 513);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(76, 24);
			this.butDelete.TabIndex = 7;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkPatientDeclined
			// 
			this.checkPatientDeclined.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientDeclined.Location = new System.Drawing.Point(506, 13);
			this.checkPatientDeclined.Name = "checkPatientDeclined";
			this.checkPatientDeclined.Size = new System.Drawing.Size(134, 18);
			this.checkPatientDeclined.TabIndex = 8;
			this.checkPatientDeclined.Text = "Patient Declined";
			this.checkPatientDeclined.UseVisualStyleBackColor = true;
			// 
			// FormInterventionEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(652, 548);
			this.Controls.Add(this.checkPatientDeclined);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboCodeSet);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label5);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInterventionEdit";
			this.Text = "Intervention";
			this.Load += new System.EventHandler(this.FormInterventionEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboCodeSet;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textNote;
		private UI.Button butCancel;
		private UI.Button butOK;
		private UI.Button butDelete;
		private System.Windows.Forms.CheckBox checkPatientDeclined;
	}
}