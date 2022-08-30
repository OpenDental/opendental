namespace CentralManager {
	partial class FormCentralUserGroups {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralUserGroups));
			this.butAddGroup = new OpenDental.UI.Button();
			this.listGroups = new System.Windows.Forms.ListBox();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butAddGroup
			// 
			this.butAddGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddGroup.Image = ((System.Drawing.Image)(resources.GetObject("butAddGroup.Image")));
			this.butAddGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGroup.Location = new System.Drawing.Point(204, 12);
			this.butAddGroup.Name = "butAddGroup";
			this.butAddGroup.Size = new System.Drawing.Size(80, 24);
			this.butAddGroup.TabIndex = 5;
			this.butAddGroup.Text = "Add";
			this.butAddGroup.Click += new System.EventHandler(this.butAddGroup_Click);
			// 
			// listGroups
			// 
			this.listGroups.FormattingEnabled = true;
			this.listGroups.Location = new System.Drawing.Point(12, 12);
			this.listGroups.Name = "listGroups";
			this.listGroups.Size = new System.Drawing.Size(183, 355);
			this.listGroups.TabIndex = 4;
			this.listGroups.DoubleClick += new System.EventHandler(this.listGroups_DoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(204, 343);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(80, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormCentralUserGroups
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 380);
			this.Controls.Add(this.butAddGroup);
			this.Controls.Add(this.listGroups);
			this.Controls.Add(this.butClose);
			this.Name = "FormCentralUserGroups";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "User Groups";
			this.Load += new System.EventHandler(this.FormCentralUserGroups_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butAddGroup;
		private System.Windows.Forms.ListBox listGroups;
		private OpenDental.UI.Button butClose;
	}
}