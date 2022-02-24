namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizRegKey {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupProcTools = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butProcCodeTools = new OpenDental.UI.Button();
			this.butChangeRegKey = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textRegKey = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.groupProcTools.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupProcTools
			// 
			this.groupProcTools.Controls.Add(this.label1);
			this.groupProcTools.Controls.Add(this.butProcCodeTools);
			this.groupProcTools.Location = new System.Drawing.Point(244, 281);
			this.groupProcTools.Name = "groupProcTools";
			this.groupProcTools.Size = new System.Drawing.Size(446, 105);
			this.groupProcTools.TabIndex = 61;
			this.groupProcTools.TabStop = false;
			this.groupProcTools.Text = "Procedure Code Tools";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(419, 39);
			this.label1.TabIndex = 60;
			this.label1.Text = "After a valid registration key has been entered, you should run the procedure cod" +
    "e tools to add in the correct procedure codes and remove any trial version \"T\" c" +
    "odes.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butProcCodeTools
			// 
			this.butProcCodeTools.Location = new System.Drawing.Point(303, 66);
			this.butProcCodeTools.Name = "butProcCodeTools";
			this.butProcCodeTools.Size = new System.Drawing.Size(122, 23);
			this.butProcCodeTools.TabIndex = 59;
			this.butProcCodeTools.Text = "Procedure Code Tools";
			this.butProcCodeTools.Click += new System.EventHandler(this.butProcCodeTools_Click);
			// 
			// butChangeRegKey
			// 
			this.butChangeRegKey.Location = new System.Drawing.Point(623, 110);
			this.butChangeRegKey.Name = "butChangeRegKey";
			this.butChangeRegKey.Size = new System.Drawing.Size(67, 23);
			this.butChangeRegKey.TabIndex = 54;
			this.butChangeRegKey.Text = "Change";
			this.butChangeRegKey.Click += new System.EventHandler(this.butChangeRegKey_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(376, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(314, 20);
			this.label4.TabIndex = 53;
			this.label4.Text = "Valid for one office ONLY.  This is tracked.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textRegKey
			// 
			this.textRegKey.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textRegKey.Location = new System.Drawing.Point(244, 88);
			this.textRegKey.Name = "textRegKey";
			this.textRegKey.ReadOnly = true;
			this.textRegKey.Size = new System.Drawing.Size(446, 20);
			this.textRegKey.TabIndex = 51;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(241, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(145, 19);
			this.label2.TabIndex = 52;
			this.label2.Text = "Registration Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAdvanced
			// 
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdvanced.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdvanced.Location = new System.Drawing.Point(845, 462);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(82, 26);
			this.butAdvanced.TabIndex = 112;
			this.butAdvanced.Text = "Advanced";
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(554, 460);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(285, 31);
			this.label7.TabIndex = 111;
			this.label7.Text = "Further changes can be made by going to \r\nHelp -> Update -> Setup or clicking \'Ad" +
    "vanced\'.\r\n";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UserControlSetupWizRegKey
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupProcTools);
			this.Controls.Add(this.butChangeRegKey);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textRegKey);
			this.Controls.Add(this.label2);
			this.Name = "UserControlSetupWizRegKey";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizRegKey_Load);
			this.groupProcTools.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butChangeRegKey;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textRegKey;
		private System.Windows.Forms.Label label2;
		private UI.Button butProcCodeTools;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupProcTools;
		private UI.Button butAdvanced;
		private System.Windows.Forms.Label label7;
	}
}
