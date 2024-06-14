namespace OpenDental {
	partial class FormContinuityOfCareDocument {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContinuityOfCareDocument));
			this.labelTitle = new System.Windows.Forms.Label();
			this.butExportToFile = new OpenDental.UI.Button();
			this.butShowAndPrint = new OpenDental.UI.Button();
			this.labelInstructions = new System.Windows.Forms.Label();
			this.textInstructions = new OpenDental.ODtextBox();
			this.SuspendLayout();
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(14, 15);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(259, 43);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "A Continuity of Care Document (CCD) is used to share a patient health summary ele" +
    "ctronically.";
			// 
			// butExportToFile
			// 
			this.butExportToFile.Location = new System.Drawing.Point(120, 61);
			this.butExportToFile.Name = "butExportToFile";
			this.butExportToFile.Size = new System.Drawing.Size(95, 24);
			this.butExportToFile.TabIndex = 2;
			this.butExportToFile.Text = "Export to File";
			this.butExportToFile.Click += new System.EventHandler(this.butExportToFile_Click);
			// 
			// butShowAndPrint
			// 
			this.butShowAndPrint.Location = new System.Drawing.Point(17, 61);
			this.butShowAndPrint.Name = "butShowAndPrint";
			this.butShowAndPrint.Size = new System.Drawing.Size(95, 24);
			this.butShowAndPrint.TabIndex = 1;
			this.butShowAndPrint.Text = "Show and Print";
			this.butShowAndPrint.Click += new System.EventHandler(this.butShowAndPrint_Click);
			// 
			// labelInstructions
			// 
			this.labelInstructions.Location = new System.Drawing.Point(17, 99);
			this.labelInstructions.Name = "labelInstructions";
			this.labelInstructions.Size = new System.Drawing.Size(304, 18);
			this.labelInstructions.TabIndex = 0;
			this.labelInstructions.Text = "Optional Instructions for Patient to show on CCD";
			this.labelInstructions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textInstructions
			// 
			this.textInstructions.AcceptsTab = true;
			this.textInstructions.BackColor = System.Drawing.SystemColors.Window;
			this.textInstructions.DetectLinksEnabled = false;
			this.textInstructions.DetectUrls = false;
			this.textInstructions.Location = new System.Drawing.Point(17, 120);
			this.textInstructions.Name = "textInstructions";
			this.textInstructions.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.MedicalSummary;
			this.textInstructions.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textInstructions.Size = new System.Drawing.Size(256, 73);
			this.textInstructions.TabIndex = 3;
			this.textInstructions.Text = "";
			// 
			// FormContinuityOfCareDocument
			// 
			this.ClientSize = new System.Drawing.Size(284, 209);
			this.Controls.Add(this.textInstructions);
			this.Controls.Add(this.butExportToFile);
			this.Controls.Add(this.labelInstructions);
			this.Controls.Add(this.butShowAndPrint);
			this.Controls.Add(this.labelTitle);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormContinuityOfCareDocument";
			this.Text = "Continuity of Care";
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label labelTitle;
		private UI.Button butExportToFile;
		private UI.Button butShowAndPrint;
		private System.Windows.Forms.Label labelInstructions;
		private ODtextBox textInstructions;
	}
}