namespace OpenDental{
	partial class FormAdjustmentPicker {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjustmentPicker));
			this.butOK = new OpenDental.UI.Button();
			this.checkUnattached = new OpenDental.UI.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(542, 459);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkUnattached
			// 
			this.checkUnattached.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkUnattached.AutoSize = true;
			this.checkUnattached.Location = new System.Drawing.Point(12, 459);
			this.checkUnattached.Name = "checkUnattached";
			this.checkUnattached.Size = new System.Drawing.Size(136, 17);
			this.checkUnattached.TabIndex = 5;
			this.checkUnattached.Text = "Show Unattached Only";
			this.checkUnattached.Click += new System.EventHandler(this.checkUnattached_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(605, 441);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Adjustments";
			this.gridMain.TranslationName = "TableAjdustmentPicker";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormAdjustmentPicker
			// 
			this.ClientSize = new System.Drawing.Size(629, 495);
			this.Controls.Add(this.checkUnattached);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdjustmentPicker";
			this.Text = "Adjustment Picker";
			this.Load += new System.EventHandler(this.FormAdjustmentPicker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.CheckBox checkUnattached;
		private UI.GridOD gridMain;
	}
}