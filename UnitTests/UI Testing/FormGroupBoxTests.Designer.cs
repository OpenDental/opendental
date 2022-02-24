namespace UnitTests
{
	partial class FormGroupBoxTests
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBoxOD7 = new OpenDental.UI.GroupBoxOD();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupCustom = new OpenDental.UI.GroupBoxOD();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBoxOD3 = new OpenDental.UI.GroupBoxOD();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBoxOD7.SuspendLayout();
			this.groupCustom.SuspendLayout();
			this.groupBoxOD3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(28, 17);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Microsoft";
			// 
			// groupBoxOD7
			// 
			this.groupBoxOD7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.groupBoxOD7.Controls.Add(this.textBox8);
			this.groupBoxOD7.Controls.Add(this.label8);
			this.groupBoxOD7.Location = new System.Drawing.Point(261, 146);
			this.groupBoxOD7.Name = "groupBoxOD7";
			this.groupBoxOD7.Size = new System.Drawing.Size(200, 100);
			this.groupBoxOD7.TabIndex = 5;
			this.groupBoxOD7.TabStop = false;
			this.groupBoxOD7.Text = "IsLighter";
			// 
			// textBox8
			// 
			this.textBox8.Location = new System.Drawing.Point(74, 43);
			this.textBox8.Name = "textBox8";
			this.textBox8.Size = new System.Drawing.Size(100, 20);
			this.textBox8.TabIndex = 2;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(33, 46);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(35, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "label8";
			// 
			// groupCustom
			// 
			this.groupCustom.BackColor = System.Drawing.Color.White;
			this.groupCustom.Controls.Add(this.textBox5);
			this.groupCustom.Controls.Add(this.label5);
			this.groupCustom.IsLighter = false;
			this.groupCustom.Location = new System.Drawing.Point(28, 146);
			this.groupCustom.Name = "groupCustom";
			this.groupCustom.Size = new System.Drawing.Size(200, 100);
			this.groupCustom.TabIndex = 7;
			this.groupCustom.TabStop = false;
			this.groupCustom.Text = "Custom";
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(71, 41);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(100, 20);
			this.textBox5.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(30, 44);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(35, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "label5";
			// 
			// groupBoxOD3
			// 
			this.groupBoxOD3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxOD3.Controls.Add(this.textBox6);
			this.groupBoxOD3.Controls.Add(this.label6);
			this.groupBoxOD3.IsLighter = false;
			this.groupBoxOD3.Location = new System.Drawing.Point(261, 17);
			this.groupBoxOD3.Name = "groupBoxOD3";
			this.groupBoxOD3.Size = new System.Drawing.Size(200, 100);
			this.groupBoxOD3.TabIndex = 4;
			this.groupBoxOD3.TabStop = false;
			this.groupBoxOD3.Text = "IsLighter false";
			// 
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(74, 41);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(100, 20);
			this.textBox6.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(33, 44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(35, 13);
			this.label6.TabIndex = 6;
			this.label6.Text = "label6";
			// 
			// FormGroupBoxTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(539, 297);
			this.Controls.Add(this.groupBoxOD7);
			this.Controls.Add(this.groupCustom);
			this.Controls.Add(this.groupBoxOD3);
			this.Controls.Add(this.groupBox1);
			this.Name = "FormGroupBoxTests";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormGroupBoxColors";
			this.groupBoxOD7.ResumeLayout(false);
			this.groupBoxOD7.PerformLayout();
			this.groupCustom.ResumeLayout(false);
			this.groupCustom.PerformLayout();
			this.groupBoxOD3.ResumeLayout(false);
			this.groupBoxOD3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.UI.GroupBoxOD groupBoxOD3;
		private OpenDental.UI.GroupBoxOD groupCustom;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.GroupBoxOD groupBoxOD7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.Label label8;
	}
}