namespace OpenDental {
	partial class FormVersionPrompt {
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
			this.butLast3 = new OpenDental.UI.Button();
			this.butLast2 = new OpenDental.UI.Button();
			this.butLast1 = new OpenDental.UI.Button();
			this.textVersions = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butHead = new OpenDental.UI.Button();
			this.butBackport = new OpenDental.UI.Button();
			this.butUnversioned = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butLast3
			// 
			this.butLast3.Location = new System.Drawing.Point(282, 30);
			this.butLast3.Name = "butLast3";
			this.butLast3.Size = new System.Drawing.Size(58, 23);
			this.butLast3.TabIndex = 42;
			this.butLast3.Text = "Last 3";
			this.butLast3.UseVisualStyleBackColor = true;
			this.butLast3.Click += new System.EventHandler(this.butLast3_Click);
			// 
			// butLast2
			// 
			this.butLast2.Location = new System.Drawing.Point(218, 30);
			this.butLast2.Name = "butLast2";
			this.butLast2.Size = new System.Drawing.Size(58, 23);
			this.butLast2.TabIndex = 41;
			this.butLast2.Text = "Last 2";
			this.butLast2.UseVisualStyleBackColor = true;
			this.butLast2.Click += new System.EventHandler(this.butLast2_Click);
			// 
			// butLast1
			// 
			this.butLast1.Location = new System.Drawing.Point(154, 30);
			this.butLast1.Name = "butLast1";
			this.butLast1.Size = new System.Drawing.Size(58, 23);
			this.butLast1.TabIndex = 40;
			this.butLast1.Text = "Last 1";
			this.butLast1.UseVisualStyleBackColor = true;
			this.butLast1.Click += new System.EventHandler(this.butLast1_Click);
			// 
			// textVersions
			// 
			this.textVersions.Location = new System.Drawing.Point(68, 59);
			this.textVersions.Name = "textVersions";
			this.textVersions.Size = new System.Drawing.Size(269, 20);
			this.textVersions.TabIndex = 39;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 59);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(61, 18);
			this.label6.TabIndex = 38;
			this.label6.Text = "Version(s)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(278, 107);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 23);
			this.butCancel.TabIndex = 44;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(207, 107);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65, 23);
			this.butOK.TabIndex = 43;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butHead
			// 
			this.butHead.Location = new System.Drawing.Point(85, 30);
			this.butHead.Name = "butHead";
			this.butHead.Size = new System.Drawing.Size(63, 23);
			this.butHead.TabIndex = 45;
			this.butHead.Text = "Head Only";
			this.butHead.UseVisualStyleBackColor = true;
			this.butHead.Click += new System.EventHandler(this.butHead_Click);
			// 
			// butBackport
			// 
			this.butBackport.Location = new System.Drawing.Point(14, 107);
			this.butBackport.Name = "butBackport";
			this.butBackport.Size = new System.Drawing.Size(65, 23);
			this.butBackport.TabIndex = 46;
			this.butBackport.Text = "Backport";
			this.butBackport.UseVisualStyleBackColor = true;
			this.butBackport.Click += new System.EventHandler(this.butBackport_Click);
			// 
			// butUnversioned
			// 
			this.butUnversioned.Location = new System.Drawing.Point(6, 30);
			this.butUnversioned.Name = "butUnversioned";
			this.butUnversioned.Size = new System.Drawing.Size(73, 23);
			this.butUnversioned.TabIndex = 47;
			this.butUnversioned.Text = "Unversioned";
			this.butUnversioned.UseVisualStyleBackColor = true;
			this.butUnversioned.Click += new System.EventHandler(this.butUnversioned_Click);
			// 
			// FormVersionPrompt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(349, 142);
			this.Controls.Add(this.butUnversioned);
			this.Controls.Add(this.butBackport);
			this.Controls.Add(this.butHead);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butLast3);
			this.Controls.Add(this.butLast2);
			this.Controls.Add(this.butLast1);
			this.Controls.Add(this.textVersions);
			this.Controls.Add(this.label6);
			this.Name = "FormVersionPrompt";
			this.Text = "Enter a Version";
			this.Load += new System.EventHandler(this.FormVersionPrompt_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butLast3;
		private UI.Button butLast2;
		private UI.Button butLast1;
		private System.Windows.Forms.TextBox textVersions;
		private System.Windows.Forms.Label label6;
		private UI.Button butCancel;
		private UI.Button butOK;
		private UI.Button butHead;
		private UI.Button butBackport;
		private UI.Button butUnversioned;
	}
}