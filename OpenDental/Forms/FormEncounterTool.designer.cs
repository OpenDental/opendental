namespace OpenDental{
	partial class FormEncounterTool {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEncounterTool));
			this.butRun = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelEncWarning = new System.Windows.Forms.Label();
			this.textEncCodeDescript = new System.Windows.Forms.TextBox();
			this.butEncCpt = new OpenDental.UI.Button();
			this.comboEncCodes = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textEncCodeValue = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butEncHcpcs = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.butEncSnomed = new OpenDental.UI.Button();
			this.butEncCdt = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location = new System.Drawing.Point(288, 280);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 24);
			this.butRun.TabIndex = 8;
			this.butRun.Text = "&Run";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(369, 280);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(12, 58);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(115, 17);
			this.label8.TabIndex = 166;
			this.label8.Text = "Start Date";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(220, 57);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(84, 17);
			this.label7.TabIndex = 165;
			this.label7.Text = "End Date";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(307, 57);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 152;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(130, 57);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 151;
			// 
			// labelEncWarning
			// 
			this.labelEncWarning.ForeColor = System.Drawing.Color.Red;
			this.labelEncWarning.Location = new System.Drawing.Point(34, 242);
			this.labelEncWarning.Name = "labelEncWarning";
			this.labelEncWarning.Size = new System.Drawing.Size(403, 26);
			this.labelEncWarning.TabIndex = 164;
			this.labelEncWarning.Text = "Warning: In order for patients to be considered for CQM calculations, you will ha" +
    "ve to manually create encounters with a qualified code specific to each measure." +
    "";
			// 
			// textEncCodeDescript
			// 
			this.textEncCodeDescript.AcceptsTab = true;
			this.textEncCodeDescript.Location = new System.Drawing.Point(130, 108);
			this.textEncCodeDescript.MaxLength = 2147483647;
			this.textEncCodeDescript.Multiline = true;
			this.textEncCodeDescript.Name = "textEncCodeDescript";
			this.textEncCodeDescript.ReadOnly = true;
			this.textEncCodeDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textEncCodeDescript.Size = new System.Drawing.Size(320, 46);
			this.textEncCodeDescript.TabIndex = 158;
			// 
			// butEncCpt
			// 
			this.butEncCpt.Location = new System.Drawing.Point(294, 187);
			this.butEncCpt.Name = "butEncCpt";
			this.butEncCpt.Size = new System.Drawing.Size(75, 24);
			this.butEncCpt.TabIndex = 156;
			this.butEncCpt.Text = "CPT";
			this.butEncCpt.Click += new System.EventHandler(this.butEncCpt_Click);
			// 
			// comboEncCodes
			// 
			this.comboEncCodes.FormattingEnabled = true;
			this.comboEncCodes.Location = new System.Drawing.Point(130, 83);
			this.comboEncCodes.Name = "comboEncCodes";
			this.comboEncCodes.Size = new System.Drawing.Size(158, 21);
			this.comboEncCodes.TabIndex = 153;
			this.comboEncCodes.SelectionChangeCommitted += new System.EventHandler(this.comboEncCodes_SelectionChangeCommitted);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(12, 218);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(115, 17);
			this.label5.TabIndex = 163;
			this.label5.Text = "Code";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEncCodeValue
			// 
			this.textEncCodeValue.Location = new System.Drawing.Point(130, 217);
			this.textEncCodeValue.Name = "textEncCodeValue";
			this.textEncCodeValue.ReadOnly = true;
			this.textEncCodeValue.Size = new System.Drawing.Size(158, 20);
			this.textEncCodeValue.TabIndex = 162;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(432, 45);
			this.label1.TabIndex = 167;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 108);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(115, 17);
			this.label2.TabIndex = 159;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 84);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(115, 17);
			this.label4.TabIndex = 160;
			this.label4.Text = "Recommended Codes";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEncHcpcs
			// 
			this.butEncHcpcs.Location = new System.Drawing.Point(375, 187);
			this.butEncHcpcs.Name = "butEncHcpcs";
			this.butEncHcpcs.Size = new System.Drawing.Size(75, 24);
			this.butEncHcpcs.TabIndex = 157;
			this.butEncHcpcs.Text = "HCPCS";
			this.butEncHcpcs.Click += new System.EventHandler(this.butEncHcpcs_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(130, 158);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(276, 26);
			this.label3.TabIndex = 161;
			this.label3.Text = "Choosing a code not in the recommended list might make it more difficult to incre" +
    "ase your CQM percentages.";
			// 
			// butEncSnomed
			// 
			this.butEncSnomed.Location = new System.Drawing.Point(130, 187);
			this.butEncSnomed.Name = "butEncSnomed";
			this.butEncSnomed.Size = new System.Drawing.Size(77, 24);
			this.butEncSnomed.TabIndex = 154;
			this.butEncSnomed.Text = "SNOMED CT";
			this.butEncSnomed.Click += new System.EventHandler(this.butEncSnomed_Click);
			// 
			// butEncCdt
			// 
			this.butEncCdt.Location = new System.Drawing.Point(213, 187);
			this.butEncCdt.Name = "butEncCdt";
			this.butEncCdt.Size = new System.Drawing.Size(75, 24);
			this.butEncCdt.TabIndex = 155;
			this.butEncCdt.Text = "CDT";
			this.butEncCdt.Click += new System.EventHandler(this.butEncCdt_Click);
			// 
			// FormEncounterTool
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(456, 316);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.labelEncWarning);
			this.Controls.Add(this.textEncCodeDescript);
			this.Controls.Add(this.butEncCpt);
			this.Controls.Add(this.comboEncCodes);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textEncCodeValue);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butEncHcpcs);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butEncSnomed);
			this.Controls.Add(this.butEncCdt);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEncounterTool";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Insert Encounters";
			this.Load += new System.EventHandler(this.FormEncounterTool_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butRun;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label labelEncWarning;
		private System.Windows.Forms.TextBox textEncCodeDescript;
		private UI.Button butEncCpt;
		private System.Windows.Forms.ComboBox comboEncCodes;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textEncCodeValue;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private UI.Button butEncHcpcs;
		private System.Windows.Forms.Label label3;
		private UI.Button butEncSnomed;
		private UI.Button butEncCdt;
	}
}