namespace OpenDentalGraph {
	partial class BrokenApptGraphOptionsCtrl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboBrokenProcType = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboAdjType = new System.Windows.Forms.ComboBox();
			this.radioRunAdjs = new System.Windows.Forms.RadioButton();
			this.radioRunApts = new System.Windows.Forms.RadioButton();
			this.radioRunProcs = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.comboBrokenProcType);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.comboAdjType);
			this.groupBox3.Controls.Add(this.radioRunAdjs);
			this.groupBox3.Controls.Add(this.radioRunApts);
			this.groupBox3.Controls.Add(this.radioRunProcs);
			this.groupBox3.Location = new System.Drawing.Point(0, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(391, 131);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Count By";
			// 
			// comboBrokenProcType
			// 
			this.comboBrokenProcType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBrokenProcType.Enabled = false;
			this.comboBrokenProcType.FormattingEnabled = true;
			this.comboBrokenProcType.Location = new System.Drawing.Point(100, 37);
			this.comboBrokenProcType.Name = "comboBrokenProcType";
			this.comboBrokenProcType.Size = new System.Drawing.Size(214, 21);
			this.comboBrokenProcType.TabIndex = 9;
			this.comboBrokenProcType.SelectedIndexChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(97, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(280, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Count the selected adjustment type.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(97, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(280, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Count broken appointments left on the schedule.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(97, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Count completed broken and/or canceled procedures.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboAdjType
			// 
			this.comboAdjType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAdjType.Enabled = false;
			this.comboAdjType.FormattingEnabled = true;
			this.comboAdjType.Location = new System.Drawing.Point(100, 100);
			this.comboAdjType.Name = "comboAdjType";
			this.comboAdjType.Size = new System.Drawing.Size(214, 21);
			this.comboAdjType.TabIndex = 5;
			this.comboAdjType.SelectedIndexChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// radioRunAdjs
			// 
			this.radioRunAdjs.AutoSize = true;
			this.radioRunAdjs.Location = new System.Drawing.Point(6, 80);
			this.radioRunAdjs.Name = "radioRunAdjs";
			this.radioRunAdjs.Size = new System.Drawing.Size(82, 17);
			this.radioRunAdjs.TabIndex = 4;
			this.radioRunAdjs.Text = "Adjustments";
			this.radioRunAdjs.UseVisualStyleBackColor = true;
			this.radioRunAdjs.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// radioRunApts
			// 
			this.radioRunApts.AutoSize = true;
			this.radioRunApts.Location = new System.Drawing.Point(6, 59);
			this.radioRunApts.Name = "radioRunApts";
			this.radioRunApts.Size = new System.Drawing.Size(89, 17);
			this.radioRunApts.TabIndex = 1;
			this.radioRunApts.Text = "Appointments";
			this.radioRunApts.UseVisualStyleBackColor = true;
			this.radioRunApts.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// radioRunProcs
			// 
			this.radioRunProcs.AutoSize = true;
			this.radioRunProcs.Checked = true;
			this.radioRunProcs.Location = new System.Drawing.Point(6, 17);
			this.radioRunProcs.Name = "radioRunProcs";
			this.radioRunProcs.Size = new System.Drawing.Size(79, 17);
			this.radioRunProcs.TabIndex = 3;
			this.radioRunProcs.TabStop = true;
			this.radioRunProcs.Text = "Procedures";
			this.radioRunProcs.UseVisualStyleBackColor = true;
			this.radioRunProcs.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// BrokenApptGraphOptionsCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox3);
			this.Name = "BrokenApptGraphOptionsCtrl";
			this.Size = new System.Drawing.Size(394, 131);
			this.Load += new System.EventHandler(this.BrokenApptGraphOptionsCtrl_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.RadioButton radioRunApts;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioRunAdjs;
		private System.Windows.Forms.RadioButton radioRunProcs;
		private System.Windows.Forms.ComboBox comboAdjType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBrokenProcType;
	}
}
