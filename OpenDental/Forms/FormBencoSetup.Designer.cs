namespace OpenDental {
	partial class FormBencoSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBencoSetup));
			this.checkEnable = new System.Windows.Forms.CheckBox();
			this.labelHeader = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelPath = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.groupSettings = new System.Windows.Forms.GroupBox();
			this.listToolBars = new OpenDental.UI.ListBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.textProgDesc = new System.Windows.Forms.TextBox();
			this.labelDesc = new System.Windows.Forms.Label();
			this.labelButText = new System.Windows.Forms.Label();
			this.textButText = new System.Windows.Forms.TextBox();
			this.groupSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkEnable
			// 
			this.checkEnable.Location = new System.Drawing.Point(12, 31);
			this.checkEnable.Name = "checkEnable";
			this.checkEnable.Size = new System.Drawing.Size(104, 24);
			this.checkEnable.TabIndex = 0;
			this.checkEnable.Text = "Enable";
			this.checkEnable.UseVisualStyleBackColor = true;
			// 
			// labelHeader
			// 
			this.labelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHeader.Location = new System.Drawing.Point(2, 9);
			this.labelHeader.Name = "labelHeader";
			this.labelHeader.Size = new System.Drawing.Size(441, 23);
			this.labelHeader.TabIndex = 1;
			this.labelHeader.Text = "Turn on the features for Benco Practice Management";
			this.labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(358, 299);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(277, 299);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelPath
			// 
			this.labelPath.Location = new System.Drawing.Point(6, 40);
			this.labelPath.Name = "labelPath";
			this.labelPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelPath.Size = new System.Drawing.Size(118, 18);
			this.labelPath.TabIndex = 4;
			this.labelPath.Text = "Path";
			// 
			// textPath
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.Location = new System.Drawing.Point(131, 38);
			this.textPath.MaxLength = 255;
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(274, 20);
			this.textPath.TabIndex = 5;
			// 
			// groupSettings
			// 
			this.groupSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSettings.Controls.Add(this.listToolBars);
			this.groupSettings.Controls.Add(this.label6);
			this.groupSettings.Controls.Add(this.textProgDesc);
			this.groupSettings.Controls.Add(this.labelDesc);
			this.groupSettings.Controls.Add(this.labelButText);
			this.groupSettings.Controls.Add(this.textButText);
			this.groupSettings.Controls.Add(this.labelPath);
			this.groupSettings.Controls.Add(this.textPath);
			this.groupSettings.Location = new System.Drawing.Point(12, 59);
			this.groupSettings.Name = "groupSettings";
			this.groupSettings.Size = new System.Drawing.Size(421, 234);
			this.groupSettings.TabIndex = 6;
			this.groupSettings.TabStop = false;
			this.groupSettings.Text = "Benco Settings";
			// 
			// listToolBars
			// 
			this.listToolBars.Location = new System.Drawing.Point(131, 105);
			this.listToolBars.Name = "listToolBars";
			this.listToolBars.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listToolBars.Size = new System.Drawing.Size(156, 121);
			this.listToolBars.TabIndex = 57;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(130, 84);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(157, 18);
			this.label6.TabIndex = 58;
			this.label6.Text = "Add a button to these toolbars";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textProgDesc
			// 
			this.textProgDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textProgDesc.Location = new System.Drawing.Point(131, 16);
			this.textProgDesc.MaxLength = 100;
			this.textProgDesc.Name = "textProgDesc";
			this.textProgDesc.Size = new System.Drawing.Size(274, 20);
			this.textProgDesc.TabIndex = 9;
			// 
			// labelDesc
			// 
			this.labelDesc.Location = new System.Drawing.Point(1, 19);
			this.labelDesc.Name = "labelDesc";
			this.labelDesc.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDesc.Size = new System.Drawing.Size(124, 18);
			this.labelDesc.TabIndex = 8;
			this.labelDesc.Text = "Program Description";
			// 
			// labelButText
			// 
			this.labelButText.Location = new System.Drawing.Point(6, 63);
			this.labelButText.Name = "labelButText";
			this.labelButText.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelButText.Size = new System.Drawing.Size(119, 17);
			this.labelButText.TabIndex = 7;
			this.labelButText.Text = "Button Text";
			// 
			// textButText
			// 
			this.textButText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textButText.Location = new System.Drawing.Point(131, 60);
			this.textButText.Name = "textButText";
			this.textButText.Size = new System.Drawing.Size(274, 20);
			this.textButText.TabIndex = 6;
			// 
			// FormBencoSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(445, 329);
			this.Controls.Add(this.groupSettings);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelHeader);
			this.Controls.Add(this.checkEnable);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBencoSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Benco Practice Management Setup";
			this.Load += new System.EventHandler(this.FormBencoSetup_Load);
			this.groupSettings.ResumeLayout(false);
			this.groupSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkEnable;
		private System.Windows.Forms.Label labelHeader;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.Label labelPath;
		private System.Windows.Forms.TextBox textPath;
		private System.Windows.Forms.GroupBox groupSettings;
		private System.Windows.Forms.TextBox textProgDesc;
		private System.Windows.Forms.Label labelDesc;
		private System.Windows.Forms.Label labelButText;
		private System.Windows.Forms.TextBox textButText;
		private OpenDental.UI.ListBoxOD listToolBars;
		private System.Windows.Forms.Label label6;
	}
}