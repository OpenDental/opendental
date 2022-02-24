namespace UnitTests
{
	partial class FormGridTest
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
			this.button1 = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.butMaximized = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.button1.Location = new System.Drawing.Point(719, 249);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "get width";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasDropDowns = true;
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.ShowContextMenu = false;
			this.gridMain.Size = new System.Drawing.Size(668, 518);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Test Grid";
			this.gridMain.TranslationName = "test";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(705, 361);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(89, 20);
			this.textBox1.TabIndex = 0;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(704, 403);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 1;
			// 
			// butMaximized
			// 
			this.butMaximized.Location = new System.Drawing.Point(719, 152);
			this.butMaximized.Name = "butMaximized";
			this.butMaximized.Size = new System.Drawing.Size(75, 24);
			this.butMaximized.TabIndex = 70;
			this.butMaximized.Text = "Maximized";
			this.butMaximized.UseVisualStyleBackColor = true;
			this.butMaximized.Click += new System.EventHandler(this.butMaximized_Click);
			// 
			// FormGridTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(823, 542);
			this.Controls.Add(this.butMaximized);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.gridMain);
			this.Name = "FormGridTest";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormGridTest";
			this.Load += new System.EventHandler(this.FormGridTest_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private OpenDental.UI.Button butMaximized;
	}
}