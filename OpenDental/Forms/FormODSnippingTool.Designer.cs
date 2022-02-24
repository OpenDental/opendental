namespace OpenDental {
	partial class FormODSnippingTool {
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.butSnip = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(3, 39);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(373, 13);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			// 
			// butSnip
			// 
			this.butSnip.Location = new System.Drawing.Point(12, 7);
			this.butSnip.Name = "butSnip";
			this.butSnip.Size = new System.Drawing.Size(92, 24);
			this.butSnip.TabIndex = 6;
			this.butSnip.Text = "New";
			this.butSnip.UseVisualStyleBackColor = true;
			this.butSnip.Click += new System.EventHandler(this.buttonNewSnip_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCancel.Location = new System.Drawing.Point(291, 7);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(210, 7);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 7;
			this.butOk.Text = "OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormODSnippingTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(378, 61);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.butSnip);
			this.MaximizeBox = false;
			this.Name = "FormODSnippingTool";
			this.Text = "Snipping Tool";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butSnip;
		private UI.Button butCancel;
		private UI.Button butOk;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}