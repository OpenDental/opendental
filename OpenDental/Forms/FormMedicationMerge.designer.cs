namespace OpenDental{
	partial class FormMedicationMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedicationMerge));
			this.groupBoxFrom = new System.Windows.Forms.GroupBox();
			this.textGenNumFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textRxFrom = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textMedNameFrom = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textMedNumFrom = new System.Windows.Forms.TextBox();
			this.butChangeMedFrom = new OpenDental.UI.Button();
			this.groupBoxInto = new System.Windows.Forms.GroupBox();
			this.textGenNumInto = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textRxInto = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textMedNameInto = new System.Windows.Forms.TextBox();
			this.butChangeMedInto = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textMedNumInto = new System.Windows.Forms.TextBox();
			this.butMerge = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupBoxFrom.SuspendLayout();
			this.groupBoxInto.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxFrom
			// 
			this.groupBoxFrom.Controls.Add(this.textGenNumFrom);
			this.groupBoxFrom.Controls.Add(this.label3);
			this.groupBoxFrom.Controls.Add(this.textRxFrom);
			this.groupBoxFrom.Controls.Add(this.label4);
			this.groupBoxFrom.Controls.Add(this.textMedNameFrom);
			this.groupBoxFrom.Controls.Add(this.label6);
			this.groupBoxFrom.Controls.Add(this.label8);
			this.groupBoxFrom.Controls.Add(this.textMedNumFrom);
			this.groupBoxFrom.Controls.Add(this.butChangeMedFrom);
			this.groupBoxFrom.Location = new System.Drawing.Point(12, 107);
			this.groupBoxFrom.Name = "groupBoxFrom";
			this.groupBoxFrom.Size = new System.Drawing.Size(665, 91);
			this.groupBoxFrom.TabIndex = 13;
			this.groupBoxFrom.TabStop = false;
			this.groupBoxFrom.Text = "Medication to merge from. This medication will be merged into the one above, then" +
    " deleted.";
			// 
			// textGenNumFrom
			// 
			this.textGenNumFrom.Location = new System.Drawing.Point(152, 38);
			this.textGenNumFrom.Name = "textGenNumFrom";
			this.textGenNumFrom.ReadOnly = true;
			this.textGenNumFrom.Size = new System.Drawing.Size(140, 20);
			this.textGenNumFrom.TabIndex = 16;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(150, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(113, 13);
			this.label3.TabIndex = 15;
			this.label3.Text = "GenericNum";
			// 
			// textRxFrom
			// 
			this.textRxFrom.Location = new System.Drawing.Point(466, 38);
			this.textRxFrom.Name = "textRxFrom";
			this.textRxFrom.ReadOnly = true;
			this.textRxFrom.Size = new System.Drawing.Size(105, 20);
			this.textRxFrom.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(463, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "RxCui";
			// 
			// textMedNameFrom
			// 
			this.textMedNameFrom.Location = new System.Drawing.Point(298, 38);
			this.textMedNameFrom.Name = "textMedNameFrom";
			this.textMedNameFrom.ReadOnly = true;
			this.textMedNameFrom.Size = new System.Drawing.Size(162, 20);
			this.textMedNameFrom.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(295, 22);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(125, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "MedName";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(99, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "MedicationNum";
			// 
			// textMedNumFrom
			// 
			this.textMedNumFrom.Location = new System.Drawing.Point(6, 38);
			this.textMedNumFrom.Name = "textMedNumFrom";
			this.textMedNumFrom.ReadOnly = true;
			this.textMedNumFrom.Size = new System.Drawing.Size(140, 20);
			this.textMedNumFrom.TabIndex = 9;
			// 
			// butChangeMedFrom
			// 
			this.butChangeMedFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeMedFrom.Location = new System.Drawing.Point(577, 35);
			this.butChangeMedFrom.Name = "butChangeMedFrom";
			this.butChangeMedFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangeMedFrom.TabIndex = 4;
			this.butChangeMedFrom.Text = "Change";
			this.butChangeMedFrom.Click += new System.EventHandler(this.butChangeMedFrom_Click);
			// 
			// groupBoxInto
			// 
			this.groupBoxInto.Controls.Add(this.textGenNumInto);
			this.groupBoxInto.Controls.Add(this.label7);
			this.groupBoxInto.Controls.Add(this.textRxInto);
			this.groupBoxInto.Controls.Add(this.label5);
			this.groupBoxInto.Controls.Add(this.textMedNameInto);
			this.groupBoxInto.Controls.Add(this.butChangeMedInto);
			this.groupBoxInto.Controls.Add(this.label2);
			this.groupBoxInto.Controls.Add(this.label1);
			this.groupBoxInto.Controls.Add(this.textMedNumInto);
			this.groupBoxInto.Location = new System.Drawing.Point(12, 12);
			this.groupBoxInto.Name = "groupBoxInto";
			this.groupBoxInto.Size = new System.Drawing.Size(665, 89);
			this.groupBoxInto.TabIndex = 12;
			this.groupBoxInto.TabStop = false;
			this.groupBoxInto.Text = "Medication to merge into. The medication chosen below will be merged into this.";
			// 
			// textGenNumInto
			// 
			this.textGenNumInto.Location = new System.Drawing.Point(152, 37);
			this.textGenNumInto.Name = "textGenNumInto";
			this.textGenNumInto.ReadOnly = true;
			this.textGenNumInto.Size = new System.Drawing.Size(140, 20);
			this.textGenNumInto.TabIndex = 8;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(150, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(113, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "GenericNum";
			// 
			// textRxInto
			// 
			this.textRxInto.Location = new System.Drawing.Point(466, 37);
			this.textRxInto.Name = "textRxInto";
			this.textRxInto.ReadOnly = true;
			this.textRxInto.Size = new System.Drawing.Size(105, 20);
			this.textRxInto.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(463, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "RxCui";
			// 
			// textMedNameInto
			// 
			this.textMedNameInto.Location = new System.Drawing.Point(298, 37);
			this.textMedNameInto.Name = "textMedNameInto";
			this.textMedNameInto.ReadOnly = true;
			this.textMedNameInto.Size = new System.Drawing.Size(162, 20);
			this.textMedNameInto.TabIndex = 3;
			// 
			// butChangeMedInto
			// 
			this.butChangeMedInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeMedInto.Location = new System.Drawing.Point(577, 34);
			this.butChangeMedInto.Name = "butChangeMedInto";
			this.butChangeMedInto.Size = new System.Drawing.Size(75, 24);
			this.butChangeMedInto.TabIndex = 4;
			this.butChangeMedInto.Text = "Change";
			this.butChangeMedInto.Click += new System.EventHandler(this.butChangeMedInto_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(295, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "MedName";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "MedicationNum";
			// 
			// textMedNumInto
			// 
			this.textMedNumInto.Location = new System.Drawing.Point(6, 37);
			this.textMedNumInto.Name = "textMedNumInto";
			this.textMedNumInto.ReadOnly = true;
			this.textMedNumInto.Size = new System.Drawing.Size(140, 20);
			this.textMedNumInto.TabIndex = 0;
			// 
			// butMerge
			// 
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(508, 209);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 11;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// butClose
			// 
			this.butClose.Location = new System.Drawing.Point(589, 209);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 10;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormMedicationMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(689, 245);
			this.Controls.Add(this.groupBoxFrom);
			this.Controls.Add(this.groupBoxInto);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.butClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMedicationMerge";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Merge Medications";
			this.groupBoxFrom.ResumeLayout(false);
			this.groupBoxFrom.PerformLayout();
			this.groupBoxInto.ResumeLayout(false);
			this.groupBoxInto.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxFrom;
		private UI.Button butChangeMedFrom;
		private System.Windows.Forms.GroupBox groupBoxInto;
		private System.Windows.Forms.TextBox textGenNumInto;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textRxInto;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textMedNameInto;
		private UI.Button butChangeMedInto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMedNumInto;
		private UI.Button butMerge;
		private UI.Button butClose;
		private System.Windows.Forms.TextBox textGenNumFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textRxFrom;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textMedNameFrom;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textMedNumFrom;

	}
}