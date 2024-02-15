namespace OpenDental{
	partial class FormSheetProcSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetProcSelect));
			this.butOK = new OpenDental.UI.Button();
			this.gridProcs = new OpenDental.UI.GridOD();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(563, 660);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridProcs
			// 
			this.gridProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProcs.Location = new System.Drawing.Point(12, 33);
			this.gridProcs.Name = "gridProcs";
			this.gridProcs.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProcs.Size = new System.Drawing.Size(626, 611);
			this.gridProcs.TabIndex = 70;
			this.gridProcs.Title = "Procedures";
			this.gridProcs.TranslationName = "Table Procedures";
			// 
			// labelTitle
			// 
			this.labelTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelTitle.Location = new System.Drawing.Point(12, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(323, 21);
			this.labelTitle.TabIndex = 71;
			this.labelTitle.Text = "Select one or more Procedures to display on the selected sheet";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormSheetProcSelect
			// 
			this.ClientSize = new System.Drawing.Size(650, 696);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.gridProcs);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetProcSelect";
			this.Text = "Sheet Procedure Select";
			this.Load += new System.EventHandler(this.FormSheetProcSelect_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.GridOD gridProcs;
		private System.Windows.Forms.Label labelTitle;
	}
}