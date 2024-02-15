namespace UnitTests
{
	partial class Form2dDrawingTests
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			d.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelD2D = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// panelD2D
			// 
			this.panelD2D.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelD2D.Location = new System.Drawing.Point(50, 308);
			this.panelD2D.Name = "panelD2D";
			this.panelD2D.Size = new System.Drawing.Size(238, 196);
			this.panelD2D.TabIndex = 157;
			this.panelD2D.Paint += new System.Windows.Forms.PaintEventHandler(this.panelD2D_Paint);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(50, 37);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(238, 234);
			this.panel1.TabIndex = 156;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(47, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(270, 18);
			this.label2.TabIndex = 157;
			this.label2.Text = "Testing GDI+ line thicknesses";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(47, 287);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(270, 18);
			this.label1.TabIndex = 158;
			this.label1.Text = "Direct2D drawing";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// Form2dDrawingTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 532);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panelD2D);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.panel1);
			this.Name = "Form2dDrawingTests";
			this.Text = "FormApptProvSliderTest";
			this.Load += new System.EventHandler(this.Form2dDrawingTests_Load);
			this.SizeChanged += new System.EventHandler(this.Form2dDrawingTests_SizeChanged);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel panelD2D;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}