namespace OpenDental {
	partial class FormEhrHash {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrHash));
			this.textHash = new System.Windows.Forms.TextBox();
			this.butGererate = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.butTransmit = new System.Windows.Forms.Button();
			this.butClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textHash
			// 
			this.textHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textHash.Location = new System.Drawing.Point(192, 220);
			this.textHash.Name = "textHash";
			this.textHash.Size = new System.Drawing.Size(357, 20);
			this.textHash.TabIndex = 1;
			// 
			// butGererate
			// 
			this.butGererate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGererate.Location = new System.Drawing.Point(12, 218);
			this.butGererate.Name = "butGererate";
			this.butGererate.Size = new System.Drawing.Size(104, 23);
			this.butGererate.TabIndex = 2;
			this.butGererate.Text = "Generate Hash";
			this.butGererate.UseVisualStyleBackColor = true;
			this.butGererate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(299, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "Enter data below";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(27, 244);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(133, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "( uses SHA-1 )";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(131, 221);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "Hash";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMessage
			// 
			this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessage.Location = new System.Drawing.Point(12, 34);
			this.textMessage.Multiline = true;
			this.textMessage.Name = "textMessage";
			this.textMessage.Size = new System.Drawing.Size(537, 178);
			this.textMessage.TabIndex = 0;
			// 
			// butTransmit
			// 
			this.butTransmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTransmit.Location = new System.Drawing.Point(386, 250);
			this.butTransmit.Name = "butTransmit";
			this.butTransmit.Size = new System.Drawing.Size(75, 23);
			this.butTransmit.TabIndex = 6;
			this.butTransmit.Text = "Transmit";
			this.butTransmit.UseVisualStyleBackColor = true;
			this.butTransmit.Click += new System.EventHandler(this.butTransmit_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(474, 250);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormEhrHash
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(561, 281);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butTransmit);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butGererate);
			this.Controls.Add(this.textHash);
			this.Controls.Add(this.textMessage);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrHash";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Hash";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textHash;
		private System.Windows.Forms.Button butGererate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textMessage;
		private System.Windows.Forms.Button butTransmit;
		private System.Windows.Forms.Button butClose;

	}
}