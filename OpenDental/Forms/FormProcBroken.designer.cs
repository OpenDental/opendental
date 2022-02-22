namespace OpenDental{
	partial class FormProcBroken {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcBroken));
			this.textUser = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butPickProv = new OpenDental.UI.Button();
			this.button1 = new OpenDental.UI.Button();
			this.butAutoNoteChart = new OpenDental.UI.Button();
			this.butAutoNoteAccount = new OpenDental.UI.Button();
			this.textChartNotes = new OpenDental.ODtextBox();
			this.textDateEntry = new OpenDental.ValidDate();
			this.textProcDate = new OpenDental.ValidDate();
			this.textAccountNotes = new OpenDental.ODtextBox();
			this.textAmount = new OpenDental.ValidDouble();
			this.butCancel = new OpenDental.UI.Button();
			this.labelAmountDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(126, 132);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(116, 20);
			this.textUser.TabIndex = 0;
			this.textUser.TabStop = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(21, 135);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "User";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(19, 156);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Procedure Notes";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(21, 10);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(104, 16);
			this.label8.TabIndex = 0;
			this.label8.Text = "Entry Date";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(25, 54);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Amount";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(20, 324);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Account Notes";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Procedure Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(126, 97);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(179, 21);
			this.comboProv.TabIndex = 2;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(25, 101);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 14);
			this.label7.TabIndex = 261;
			this.label7.Text = "Provider";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(89, 74);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(216, 21);
			this.comboClinic.TabIndex = 4;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(307, 97);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(18, 21);
			this.butPickProv.TabIndex = 3;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(380, 472);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 9;
			this.button1.Text = "&OK";
			this.button1.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAutoNoteChart
			// 
			this.butAutoNoteChart.Location = new System.Drawing.Point(245, 130);
			this.butAutoNoteChart.Name = "butAutoNoteChart";
			this.butAutoNoteChart.Size = new System.Drawing.Size(80, 22);
			this.butAutoNoteChart.TabIndex = 5;
			this.butAutoNoteChart.Text = "Auto Note";
			this.butAutoNoteChart.Click += new System.EventHandler(this.butAutoNoteChart_Click);
			// 
			// butAutoNoteAccount
			// 
			this.butAutoNoteAccount.Location = new System.Drawing.Point(245, 298);
			this.butAutoNoteAccount.Name = "butAutoNoteAccount";
			this.butAutoNoteAccount.Size = new System.Drawing.Size(80, 22);
			this.butAutoNoteAccount.TabIndex = 7;
			this.butAutoNoteAccount.Text = "Auto Note";
			this.butAutoNoteAccount.Click += new System.EventHandler(this.butAutoNoteAccount_Click);
			// 
			// textChartNotes
			// 
			this.textChartNotes.AcceptsTab = true;
			this.textChartNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textChartNotes.DetectLinksEnabled = false;
			this.textChartNotes.DetectUrls = false;
			this.textChartNotes.Location = new System.Drawing.Point(126, 155);
			this.textChartNotes.Name = "textChartNotes";
			this.textChartNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textChartNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textChartNotes.Size = new System.Drawing.Size(361, 140);
			this.textChartNotes.TabIndex = 6;
			this.textChartNotes.Text = "";
			this.textChartNotes.HasAutoNotes=true;
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(126, 8);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(80, 20);
			this.textDateEntry.TabIndex = 0;
			this.textDateEntry.TabStop = false;
			// 
			// textProcDate
			// 
			this.textProcDate.Location = new System.Drawing.Point(126, 30);
			this.textProcDate.Name = "textProcDate";
			this.textProcDate.Size = new System.Drawing.Size(80, 20);
			this.textProcDate.TabIndex = 10;
			// 
			// textAccountNotes
			// 
			this.textAccountNotes.AcceptsTab = true;
			this.textAccountNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textAccountNotes.DetectLinksEnabled = false;
			this.textAccountNotes.DetectUrls = false;
			this.textAccountNotes.Location = new System.Drawing.Point(126, 323);
			this.textAccountNotes.Name = "textAccountNotes";
			this.textAccountNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Adjustment;
			this.textAccountNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAccountNotes.Size = new System.Drawing.Size(361, 140);
			this.textAccountNotes.TabIndex = 8;
			this.textAccountNotes.Text = "";
			this.textAccountNotes.HasAutoNotes=true;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(126, 52);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = -100000000D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(68, 20);
			this.textAmount.TabIndex = 1;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(461, 472);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelAmountDescription
			// 
			this.labelAmountDescription.Location = new System.Drawing.Point(195, 53);
			this.labelAmountDescription.Name = "labelAmountDescription";
			this.labelAmountDescription.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelAmountDescription.Size = new System.Drawing.Size(240, 17);
			this.labelAmountDescription.TabIndex = 266;
			this.labelAmountDescription.Text = "(Sum of prepayments for attached procedures)";
			this.labelAmountDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormProcBroken
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(548, 508);
			this.Controls.Add(this.labelAmountDescription);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.butAutoNoteChart);
			this.Controls.Add(this.butAutoNoteAccount);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textChartNotes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textProcDate);
			this.Controls.Add(this.textAccountNotes);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcBroken";
			this.Text = "Broken Appointment Procedure";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProcBroken_FormClosing);
			this.Load += new System.EventHandler(this.FormProcBroken_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butAutoNoteChart;
		private UI.Button butAutoNoteAccount;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label6;
		private ODtextBox textChartNotes;
		private System.Windows.Forms.Label label3;
		private ValidDate textDateEntry;
		private System.Windows.Forms.Label label8;
		private ValidDate textProcDate;
		private ODtextBox textAccountNotes;
		private ValidDouble textAmount;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private UI.Button button1;
		private UI.ComboBoxOD comboProv;
		private UI.Button butPickProv;
		private System.Windows.Forms.Label label7;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label labelAmountDescription;
	}
}