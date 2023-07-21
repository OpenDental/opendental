namespace UnitTests
{
	partial class FormSandboxJordan
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.button4 = new OpenDental.UI.Button();
			this.lightSignalGrid1 = new OpenDental.UI.LightSignalGrid();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(800, 33);
			this.panel1.TabIndex = 24;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label2);
			this.panel2.Location = new System.Drawing.Point(0, 336);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(800, 22);
			this.panel2.TabIndex = 25;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "label2";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(67, 142);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 24);
			this.button4.TabIndex = 62;
			this.button4.Text = "Window";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// lightSignalGrid1
			// 
			this.lightSignalGrid1.Location = new System.Drawing.Point(295, 86);
			this.lightSignalGrid1.Name = "lightSignalGrid1";
			this.lightSignalGrid1.Size = new System.Drawing.Size(50, 206);
			this.lightSignalGrid1.TabIndex = 21;
			this.lightSignalGrid1.Text = "lightSignalGrid1";
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.Location = new System.Drawing.Point(421, 199);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(238, 114);
			this.textBox1.TabIndex = 63;
			// 
			// FormSandboxJordan
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 496);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.lightSignalGrid1);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "FormSandboxJordan";
			this.Text = "Form Sandbox";
			this.Load += new System.EventHandler(this.FormSandboxJordan_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button button4;
		private OpenDental.UI.LightSignalGrid lightSignalGrid1;
		private System.Windows.Forms.TextBox textBox1;
	}
}