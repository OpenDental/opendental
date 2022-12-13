
namespace UnitTests{
	partial class FormSplitter {
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
			this.splitterLR = new OpenDental.UI.SplitterOD();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitterTB = new OpenDental.UI.SplitterOD();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.textPercent = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textLocation = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// splitterLR
			// 
			this.splitterLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.splitterLR.Location = new System.Drawing.Point(356, 0);
			this.splitterLR.Name = "splitterLR";
			this.splitterLR.Orientation = OpenDental.UI.EnumSplitterOrientation.LeftRight;
			this.splitterLR.PanelRight = this.panel2;
			this.splitterLR.Size = new System.Drawing.Size(5, 145);
			this.splitterLR.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(356, 145);
			this.panel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.panel2.Location = new System.Drawing.Point(361, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(320, 145);
			this.panel2.TabIndex = 3;
			// 
			// splitterTB
			// 
			this.splitterTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.splitterTB.Location = new System.Drawing.Point(0, 295);
			this.splitterTB.Name = "splitterTB";
			this.splitterTB.Orientation = OpenDental.UI.EnumSplitterOrientation.TopBottom;
			this.splitterTB.PanelBottom = this.panel4;
			this.splitterTB.Size = new System.Drawing.Size(681, 5);
			this.splitterTB.TabIndex = 4;
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.panel4.Location = new System.Drawing.Point(0, 300);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(681, 187);
			this.panel4.TabIndex = 4;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.panel3.Location = new System.Drawing.Point(0, 145);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(681, 150);
			this.panel3.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(248, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "percent";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPercent
			// 
			this.textPercent.Location = new System.Drawing.Point(352, 51);
			this.textPercent.Name = "textPercent";
			this.textPercent.Size = new System.Drawing.Size(100, 20);
			this.textPercent.TabIndex = 2;
			this.textPercent.TextChanged += new System.EventHandler(this.textPercent_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(248, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "location";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLocation
			// 
			this.textLocation.Location = new System.Drawing.Point(352, 25);
			this.textLocation.Name = "textLocation";
			this.textLocation.Size = new System.Drawing.Size(100, 20);
			this.textLocation.TabIndex = 0;
			this.textLocation.TextChanged += new System.EventHandler(this.textLocation_TextChanged);
			// 
			// FormSplitter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(682, 487);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPercent);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textLocation);
			this.Controls.Add(this.splitterTB);
			this.Controls.Add(this.splitterLR);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "FormSplitter";
			this.Text = "FormSplitter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.SplitterOD splitterLR;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private OpenDental.UI.SplitterOD splitterTB;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPercent;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textLocation;
	}
}