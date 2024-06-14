namespace OpenDental{
	partial class FormOrthoHardwareAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoHardwareAdd));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.labelComments = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 34);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(539, 591);
			this.gridMain.TabIndex = 32;
			this.gridMain.TranslationName = "TableOrthoHardwareSpecs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butOK.Location = new System.Drawing.Point(476, 631);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 33;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelComments
			// 
			this.labelComments.Location = new System.Drawing.Point(39, 9);
			this.labelComments.Name = "labelComments";
			this.labelComments.Size = new System.Drawing.Size(244, 16);
			this.labelComments.TabIndex = 105;
			this.labelComments.Text = "Select one or more hardware items";
			this.labelComments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormOrthoHardwareAdd
			// 
			this.ClientSize = new System.Drawing.Size(563, 667);
			this.Controls.Add(this.labelComments);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoHardwareAdd";
			this.Text = "Add Ortho Hardware";
			this.Load += new System.EventHandler(this.FormOrthoHardwareAdd_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private UI.Button butOK;
		private System.Windows.Forms.Label labelComments;
	}
}