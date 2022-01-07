namespace UnitTests
{
	partial class FormApptProvSliderTest
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
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptProvSliderTest));
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.butDeleteProc = new OpenDental.UI.Button();
			this.contrApptProvSlider1 = new OpenDental.UI.ControlApptProvSlider();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(95, 99);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "text";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(95, 156);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 2;
			// 
			// butDeleteProc
			// 
			this.butDeleteProc.Image = ((System.Drawing.Image)(resources.GetObject("butDeleteProc.Image")));
			this.butDeleteProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteProc.Location = new System.Drawing.Point(242, 47);
			this.butDeleteProc.Name = "butDeleteProc";
			this.butDeleteProc.Size = new System.Drawing.Size(75, 24);
			this.butDeleteProc.TabIndex = 155;
			this.butDeleteProc.Text = "Delete";
			// 
			// contrApptProvSlider1
			// 
			this.contrApptProvSlider1.ColorProv = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
			this.contrApptProvSlider1.Location = new System.Drawing.Point(3, 3);
			this.contrApptProvSlider1.Name = "contrApptProvSlider1";
			this.contrApptProvSlider1.Size = new System.Drawing.Size(45, 688);
			this.contrApptProvSlider1.TabIndex = 0;
			// 
			// FormApptProvSliderTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(427, 705);
			this.Controls.Add(this.butDeleteProc);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.contrApptProvSlider1);
			this.Name = "FormApptProvSliderTest";
			this.Text = "FormApptProvSliderTest";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.ControlApptProvSlider contrApptProvSlider1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private OpenDental.UI.Button butDeleteProc;
	}
}