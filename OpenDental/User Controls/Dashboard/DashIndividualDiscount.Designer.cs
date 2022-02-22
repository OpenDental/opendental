namespace OpenDental {
	partial class DashIndividualDiscount {
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
			this.groupBoxIndDiscount = new OpenDental.UI.GroupBoxOD();
			this.textUsedAdj = new System.Windows.Forms.TextBox();
			this.textRemainingAdj = new System.Windows.Forms.TextBox();
			this.textMaxAdj = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBoxIndDiscount.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxIndDiscount
			// 
			this.groupBoxIndDiscount.Controls.Add(this.textUsedAdj);
			this.groupBoxIndDiscount.Controls.Add(this.textRemainingAdj);
			this.groupBoxIndDiscount.Controls.Add(this.textMaxAdj);
			this.groupBoxIndDiscount.Controls.Add(this.label11);
			this.groupBoxIndDiscount.Controls.Add(this.label18);
			this.groupBoxIndDiscount.Controls.Add(this.label12);
			this.groupBoxIndDiscount.Location = new System.Drawing.Point(2, 0);
			this.groupBoxIndDiscount.Name = "groupBoxIndDiscount";
			this.groupBoxIndDiscount.Size = new System.Drawing.Size(166, 83);
			this.groupBoxIndDiscount.TabIndex = 69;
			this.groupBoxIndDiscount.TabStop = false;
			this.groupBoxIndDiscount.Text = "Discount Plan";
			// 
			// textAccumulatedAdj
			// 
			this.textUsedAdj.BackColor = System.Drawing.Color.White;
			this.textUsedAdj.Location = new System.Drawing.Point(99, 37);
			this.textUsedAdj.Name = "textAccumulatedAdj";
			this.textUsedAdj.ReadOnly = true;
			this.textUsedAdj.Size = new System.Drawing.Size(60, 20);
			this.textUsedAdj.TabIndex = 45;
			this.textUsedAdj.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textRemainingAdj
			// 
			this.textRemainingAdj.BackColor = System.Drawing.Color.White;
			this.textRemainingAdj.Location = new System.Drawing.Point(99, 57);
			this.textRemainingAdj.Name = "textRemainingAdj";
			this.textRemainingAdj.ReadOnly = true;
			this.textRemainingAdj.Size = new System.Drawing.Size(60, 20);
			this.textRemainingAdj.TabIndex = 51;
			this.textRemainingAdj.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textMaxAdj
			// 
			this.textMaxAdj.BackColor = System.Drawing.Color.White;
			this.textMaxAdj.Location = new System.Drawing.Point(99, 17);
			this.textMaxAdj.Name = "textMaxAdj";
			this.textMaxAdj.ReadOnly = true;
			this.textMaxAdj.Size = new System.Drawing.Size(60, 20);
			this.textMaxAdj.TabIndex = 38;
			this.textMaxAdj.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(4, 19);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(89, 15);
			this.label11.TabIndex = 32;
			this.label11.Text = "Annual Max";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(2, 59);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(91, 15);
			this.label18.TabIndex = 50;
			this.label18.Text = "Adj Remaining";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(4, 39);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(89, 15);
			this.label12.TabIndex = 33;
			this.label12.Text = "Adj Used";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// DashIndividualDiscount
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxIndDiscount);
			this.Name = "DashIndividualDiscount";
			this.Size = new System.Drawing.Size(170, 84);
			this.groupBoxIndDiscount.ResumeLayout(false);
			this.groupBoxIndDiscount.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GroupBoxOD groupBoxIndDiscount;
		private System.Windows.Forms.TextBox textUsedAdj;
		private System.Windows.Forms.TextBox textRemainingAdj;
		private System.Windows.Forms.TextBox textMaxAdj;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label12;
	}
}
