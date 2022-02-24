namespace OpenDental{
	partial class FormTaskInboxSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskInboxSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listMain = new OpenDental.UI.ListBoxOD();
			this.butSet = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(645, 113);
			this.label1.TabIndex = 4;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(116, 114);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(231, 393);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableInboxAssignments";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(625, 442);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(625, 483);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listMain
			// 
			this.listMain.Location = new System.Drawing.Point(495, 146);
			this.listMain.Name = "listMain";
			this.listMain.Size = new System.Drawing.Size(120, 264);
			this.listMain.TabIndex = 6;
			// 
			// butSet
			// 
			this.butSet.Image = global::OpenDental.Properties.Resources.Left;
			this.butSet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSet.Location = new System.Drawing.Point(414, 228);
			this.butSet.Name = "butSet";
			this.butSet.Size = new System.Drawing.Size(75, 24);
			this.butSet.TabIndex = 7;
			this.butSet.Text = "Set";
			this.butSet.Click += new System.EventHandler(this.butSet_Click);
			// 
			// FormTaskInboxSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(725, 534);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butSet);
			this.Controls.Add(this.listMain);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskInboxSetup";
			this.Text = "Task Inbox Setup";
			this.Load += new System.EventHandler(this.FormTaskInboxSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.ListBoxOD listMain;
		private OpenDental.UI.Button butSet;
	}
}