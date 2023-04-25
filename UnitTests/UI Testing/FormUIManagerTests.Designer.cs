namespace UnitTests
{
	partial class FormUIManagerTests
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
			if(disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUIManagerTests));
			this.butDelete = new OpenDental.UI.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			this.butDelete.Image = ((System.Drawing.Image)(resources.GetObject("butDelete.Image")));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(728, 436);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = false;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			this.butDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butDelete_MouseUp);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.textBox1.Location = new System.Drawing.Point(112, 59);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = "Some text";
			this.textBox1.FontChanged += new System.EventHandler(this.textBox1_FontChanged);
			// 
			// button1
			// 
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(376, 208);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 4;
			this.button1.Text = "Open";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
			// 
			// FormUIManagerTests
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.ClientSize = new System.Drawing.Size(803, 460);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.butDelete);
			this.Name = "FormUIManagerTests";
			this.Text = "UI Manager Tests";
			this.Load += new System.EventHandler(this.FormUIManagerTests_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textBox1;
		private OpenDental.UI.Button button1;
	}
}