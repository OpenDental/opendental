
namespace OpenDental {
	partial class MassEmailFilter {
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBoxRecipients = new OpenDental.UI.GroupBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.lableDays = new System.Windows.Forms.Label();
			this.textNumDays = new OpenDental.ValidNum();
			this.checkExcludeWithin = new System.Windows.Forms.CheckBox();
			this.groupBoxRecipients.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxRecipients
			// 
			this.groupBoxRecipients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.groupBoxRecipients.Controls.Add(this.label1);
			this.groupBoxRecipients.Controls.Add(this.lableDays);
			this.groupBoxRecipients.Controls.Add(this.textNumDays);
			this.groupBoxRecipients.Controls.Add(this.checkExcludeWithin);
			this.groupBoxRecipients.Location = new System.Drawing.Point(0, 0);
			this.groupBoxRecipients.Name = "groupBoxRecipients";
			this.groupBoxRecipients.Size = new System.Drawing.Size(275, 62);
			this.groupBoxRecipients.TabIndex = 211;
			this.groupBoxRecipients.Text = "Mass Email Recipients";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 18);
			this.label1.TabIndex = 208;
			this.label1.Text = "In the last";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lableDays
			// 
			this.lableDays.Location = new System.Drawing.Point(135, 39);
			this.lableDays.Name = "lableDays";
			this.lableDays.Size = new System.Drawing.Size(61, 18);
			this.lableDays.TabIndex = 207;
			this.lableDays.Text = "days";
			this.lableDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNumDays
			// 
			this.textNumDays.Location = new System.Drawing.Point(93, 38);
			this.textNumDays.MaxVal = 100000;
			this.textNumDays.Name = "textNumDays";
			this.textNumDays.ShowZero = false;
			this.textNumDays.Size = new System.Drawing.Size(39, 20);
			this.textNumDays.TabIndex = 130;
			// 
			// checkExcludeWithin
			// 
			this.checkExcludeWithin.Location = new System.Drawing.Point(6, 19);
			this.checkExcludeWithin.Name = "checkExcludeWithin";
			this.checkExcludeWithin.Size = new System.Drawing.Size(266, 18);
			this.checkExcludeWithin.TabIndex = 129;
			this.checkExcludeWithin.Text = "Exclude patients who received a mass email\r\n";
			this.checkExcludeWithin.UseVisualStyleBackColor = true;
			this.checkExcludeWithin.Click += new System.EventHandler(this.checkExcludeWithin_Click);
			// 
			// MassEmailFilter
			// 
			this.Controls.Add(this.groupBoxRecipients);
			this.Name = "MassEmailFilter";
			this.Size = new System.Drawing.Size(275, 62);
			this.groupBoxRecipients.ResumeLayout(false);
			this.groupBoxRecipients.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBoxOD groupBoxRecipients;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lableDays;
		private ValidNum textNumDays;
		private System.Windows.Forms.CheckBox checkExcludeWithin;
	}
}
