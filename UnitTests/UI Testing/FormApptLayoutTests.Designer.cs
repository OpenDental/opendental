namespace UnitTests
{
	partial class FormApptLayoutTests
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
			this.label7 = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.textDetails = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textResult = new System.Windows.Forms.TextBox();
			this.butCopyToClipboard = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label3 = new System.Windows.Forms.Label();
			this.butTestRectSpeed = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 9);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(99, 18);
			this.label7.TabIndex = 23;
			this.label7.Text = "Tests";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(654, 95);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(303, 303);
			this.pictureBox.TabIndex = 25;
			this.pictureBox.TabStop = false;
			// 
			// textDetails
			// 
			this.textDetails.Location = new System.Drawing.Point(654, 30);
			this.textDetails.Multiline = true;
			this.textDetails.Name = "textDetails";
			this.textDetails.Size = new System.Drawing.Size(303, 44);
			this.textDetails.TabIndex = 26;
			this.textDetails.Visible = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(651, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 18);
			this.label1.TabIndex = 27;
			this.label1.Text = "Details";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label1.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(651, 402);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 18);
			this.label2.TabIndex = 29;
			this.label2.Text = "Result";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textResult
			// 
			this.textResult.Location = new System.Drawing.Point(654, 423);
			this.textResult.Multiline = true;
			this.textResult.Name = "textResult";
			this.textResult.Size = new System.Drawing.Size(303, 44);
			this.textResult.TabIndex = 28;
			// 
			// butCopyToClipboard
			// 
			this.butCopyToClipboard.Location = new System.Drawing.Point(852, 473);
			this.butCopyToClipboard.Name = "butCopyToClipboard";
			this.butCopyToClipboard.Size = new System.Drawing.Size(105, 23);
			this.butCopyToClipboard.TabIndex = 30;
			this.butCopyToClipboard.Text = "Copy to Clipboard";
			this.butCopyToClipboard.UseVisualStyleBackColor = true;
			this.butCopyToClipboard.Click += new System.EventHandler(this.ButCopyToClipboard_Click);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(12, 30);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(622, 546);
			this.gridMain.TabIndex = 31;
			this.gridMain.TranslationName = "test";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.GridMain_CellClick);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(651, 500);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(123, 29);
			this.label3.TabIndex = 32;
			this.label3.Text = "Speed of 20000 rectangle comparisons";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butTestRectSpeed
			// 
			this.butTestRectSpeed.Location = new System.Drawing.Point(654, 532);
			this.butTestRectSpeed.Name = "butTestRectSpeed";
			this.butTestRectSpeed.Size = new System.Drawing.Size(105, 23);
			this.butTestRectSpeed.TabIndex = 33;
			this.butTestRectSpeed.Text = "Test";
			this.butTestRectSpeed.UseVisualStyleBackColor = true;
			this.butTestRectSpeed.Click += new System.EventHandler(this.ButTestRectSpeed_Click);
			// 
			// FormApptLayoutTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(979, 588);
			this.Controls.Add(this.butTestRectSpeed);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCopyToClipboard);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textResult);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDetails);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.label7);
			this.Name = "FormApptLayoutTests";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Appointment Layout Tests";
			this.Load += new System.EventHandler(this.FormApptLayoutTests_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.TextBox textDetails;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textResult;
		private System.Windows.Forms.Button butCopyToClipboard;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button butTestRectSpeed;
	}
}