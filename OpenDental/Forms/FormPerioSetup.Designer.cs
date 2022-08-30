
namespace OpenDental {
	partial class FormPerioSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPerioSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textF1 = new System.Windows.Forms.TextBox();
			this.textL1 = new System.Windows.Forms.TextBox();
			this.textL32 = new System.Windows.Forms.TextBox();
			this.textF32 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butAll323 = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(385, 208);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(466, 208);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(28, 91);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Upper Facial";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(31, 117);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(91, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Upper Lingual";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(31, 143);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Lower Lingual";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(31, 169);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(91, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Lower Facial";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textF1
			// 
			this.textF1.Location = new System.Drawing.Point(128, 88);
			this.textF1.MaxLength = 63;
			this.textF1.Name = "textF1";
			this.textF1.Size = new System.Drawing.Size(393, 20);
			this.textF1.TabIndex = 18;
			this.textF1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			// 
			// textL1
			// 
			this.textL1.Location = new System.Drawing.Point(128, 114);
			this.textL1.MaxLength = 63;
			this.textL1.Name = "textL1";
			this.textL1.Size = new System.Drawing.Size(393, 20);
			this.textL1.TabIndex = 19;
			this.textL1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			// 
			// textL32
			// 
			this.textL32.Location = new System.Drawing.Point(128, 140);
			this.textL32.MaxLength = 63;
			this.textL32.Name = "textL32";
			this.textL32.Size = new System.Drawing.Size(393, 20);
			this.textL32.TabIndex = 20;
			this.textL32.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			// 
			// textF32
			// 
			this.textF32.Location = new System.Drawing.Point(128, 166);
			this.textF32.MaxLength = 63;
			this.textF32.Name = "textF32";
			this.textF32.Size = new System.Drawing.Size(393, 20);
			this.textF32.TabIndex = 21;
			this.textF32.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(50, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(454, 62);
			this.label5.TabIndex = 22;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// butAll323
			// 
			this.butAll323.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAll323.Location = new System.Drawing.Point(75, 208);
			this.butAll323.Name = "butAll323";
			this.butAll323.Size = new System.Drawing.Size(81, 24);
			this.butAll323.TabIndex = 23;
			this.butAll323.Text = "Set All 323";
			this.butAll323.Click += new System.EventHandler(this.butAll323_Click);
			// 
			// FormPerioSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(568, 249);
			this.Controls.Add(this.butAll323);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textF32);
			this.Controls.Add(this.textL32);
			this.Controls.Add(this.textL1);
			this.Controls.Add(this.textF1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPerioSetup";
			this.Text = "Perio Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textF1;
		private System.Windows.Forms.TextBox textL1;
		private System.Windows.Forms.TextBox textL32;
		private System.Windows.Forms.TextBox textF32;
		private System.Windows.Forms.Label label5;
		private UI.Button butAll323;
	}
}