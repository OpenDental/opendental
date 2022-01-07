namespace UnitTests {
	partial class FormValidTextTests {
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
			this.textDate = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSetDate = new OpenDental.UI.Button();
			this.butSetEmpty = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(135, 45);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(65, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 18);
			this.label2.TabIndex = 1;
			this.label2.Text = "Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(135, 87);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(64, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 18);
			this.label1.TabIndex = 3;
			this.label1.Text = "Text";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSetDate
			// 
			this.butSetDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSetDate.Location = new System.Drawing.Point(257, 42);
			this.butSetDate.Name = "butSetDate";
			this.butSetDate.Size = new System.Drawing.Size(75, 24);
			this.butSetDate.TabIndex = 74;
			this.butSetDate.Text = "SetDate";
			this.butSetDate.UseVisualStyleBackColor = true;
			this.butSetDate.Click += new System.EventHandler(this.butSetDate_Click);
			// 
			// butSetEmpty
			// 
			this.butSetEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSetEmpty.Location = new System.Drawing.Point(338, 42);
			this.butSetEmpty.Name = "butSetEmpty";
			this.butSetEmpty.Size = new System.Drawing.Size(75, 24);
			this.butSetEmpty.TabIndex = 75;
			this.butSetEmpty.Text = "SetEmpty";
			this.butSetEmpty.UseVisualStyleBackColor = true;
			this.butSetEmpty.Click += new System.EventHandler(this.butSetEmpty_Click);
			// 
			// FormValidTextTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(748, 407);
			this.Controls.Add(this.butSetEmpty);
			this.Controls.Add(this.butSetDate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDate);
			this.Name = "FormValidTextTests";
			this.Text = "FormValidTextTests";
			this.Load += new System.EventHandler(this.FormValidTextTests_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butSetDate;
		private OpenDental.UI.Button butSetEmpty;
	}
}