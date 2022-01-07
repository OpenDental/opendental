namespace OpenDental {
	partial class FormEhrEncryption {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrEncryption));
			this.butEncrypt = new System.Windows.Forms.Button();
			this.butDecrypt = new System.Windows.Forms.Button();
			this.textInput = new System.Windows.Forms.TextBox();
			this.textResult = new System.Windows.Forms.TextBox();
			this.labelInput = new System.Windows.Forms.Label();
			this.labelOutput = new System.Windows.Forms.Label();
			this.butClose = new System.Windows.Forms.Button();
			this.butTransmit = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butEncrypt
			// 
			this.butEncrypt.Location = new System.Drawing.Point(296, 98);
			this.butEncrypt.Name = "butEncrypt";
			this.butEncrypt.Size = new System.Drawing.Size(81, 24);
			this.butEncrypt.TabIndex = 1;
			this.butEncrypt.Text = "Encrypt >>";
			this.butEncrypt.UseVisualStyleBackColor = true;
			this.butEncrypt.Click += new System.EventHandler(this.butEncrypt_Click);
			// 
			// butDecrypt
			// 
			this.butDecrypt.Location = new System.Drawing.Point(296, 160);
			this.butDecrypt.Name = "butDecrypt";
			this.butDecrypt.Size = new System.Drawing.Size(81, 24);
			this.butDecrypt.TabIndex = 2;
			this.butDecrypt.Text = "Decrypt >>";
			this.butDecrypt.UseVisualStyleBackColor = true;
			this.butDecrypt.Click += new System.EventHandler(this.butDecrypt_Click);
			// 
			// textInput
			// 
			this.textInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textInput.Location = new System.Drawing.Point(12, 35);
			this.textInput.Multiline = true;
			this.textInput.Name = "textInput";
			this.textInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textInput.Size = new System.Drawing.Size(269, 234);
			this.textInput.TabIndex = 3;
			// 
			// textResult
			// 
			this.textResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textResult.Location = new System.Drawing.Point(389, 35);
			this.textResult.Multiline = true;
			this.textResult.Name = "textResult";
			this.textResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textResult.Size = new System.Drawing.Size(270, 234);
			this.textResult.TabIndex = 4;
			// 
			// labelInput
			// 
			this.labelInput.Location = new System.Drawing.Point(12, 15);
			this.labelInput.Name = "labelInput";
			this.labelInput.Size = new System.Drawing.Size(233, 17);
			this.labelInput.TabIndex = 5;
			this.labelInput.Text = "Enter text to encrypt or decrypt";
			this.labelInput.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelOutput
			// 
			this.labelOutput.Location = new System.Drawing.Point(389, 15);
			this.labelOutput.Name = "labelOutput";
			this.labelOutput.Size = new System.Drawing.Size(124, 17);
			this.labelOutput.TabIndex = 6;
			this.labelOutput.Text = "Result";
			this.labelOutput.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(586, 278);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(73, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butTransmit
			// 
			this.butTransmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTransmit.Location = new System.Drawing.Point(484, 278);
			this.butTransmit.Name = "butTransmit";
			this.butTransmit.Size = new System.Drawing.Size(85, 24);
			this.butTransmit.TabIndex = 8;
			this.butTransmit.Text = "Transmit";
			this.butTransmit.UseVisualStyleBackColor = true;
			this.butTransmit.Click += new System.EventHandler(this.butTransmit_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(283, 134);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "( uses AES-128 )";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FormEhrEncryption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(680, 319);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butTransmit);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelOutput);
			this.Controls.Add(this.labelInput);
			this.Controls.Add(this.textResult);
			this.Controls.Add(this.textInput);
			this.Controls.Add(this.butDecrypt);
			this.Controls.Add(this.butEncrypt);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrEncryption";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Encryption";
			this.Load += new System.EventHandler(this.FormEncryption_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butEncrypt;
		private System.Windows.Forms.Button butDecrypt;
		private System.Windows.Forms.TextBox textInput;
		private System.Windows.Forms.TextBox textResult;
		private System.Windows.Forms.Label labelInput;
		private System.Windows.Forms.Label labelOutput;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butTransmit;
		private System.Windows.Forms.Label label2;

	}
}