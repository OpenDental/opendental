namespace OpenDental {
	partial class FormApptReminderRuleAggEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptReminderRuleAggEdit));
			this.butSave = new OpenDental.UI.Button();
			this.tabControl1 = new OpenDental.UI.TabControl();
			this.tabPageDefault = new OpenDental.UI.TabPage();
			this.panelMain = new OpenDental.UI.PanelOD();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(581, 567);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageDefault);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Size = new System.Drawing.Size(667, 561);
			this.tabControl1.TabIndex = 19;
			// 
			// tabPageDefault
			// 
			this.tabPageDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabPageDefault.Location = new System.Drawing.Point(2, 21);
			this.tabPageDefault.Name = "tabPageDefault";
			this.tabPageDefault.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDefault.Size = new System.Drawing.Size(663, 538);
			this.tabPageDefault.TabIndex = 0;
			this.tabPageDefault.Text = "Default";
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMain.Location = new System.Drawing.Point(2, 21);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(663, 538);
			this.panelMain.TabIndex = 0;
			this.panelMain.Visible = false;
			// 
			// FormApptReminderRuleAggEdit
			// 
			this.ClientSize = new System.Drawing.Size(668, 603);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptReminderRuleAggEdit";
			this.Text = "Automated Messages Advanced Settings";
			this.Load += new System.EventHandler(this.FormApptReminderRuleEdit_Load);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.TabControl tabControl1;
		private OpenDental.UI.TabPage tabPageDefault;
		private UI.PanelOD panelMain;
	}
}