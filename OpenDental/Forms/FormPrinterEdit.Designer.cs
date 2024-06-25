namespace OpenDental {
	partial class FormPrinterEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrinterEdit));
			this.butSave = new OpenDental.UI.Button();
			this.checkPrompt = new OpenDental.UI.CheckBox();
			this.comboPrinter = new OpenDental.UI.ComboBox();
			this.labelPrinter = new System.Windows.Forms.Label();
			this.checkVirtualPrinter = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textFileExtension = new OpenDental.ODtextBox();
			this.labelSituation = new System.Windows.Forms.Label();
			this.textSituation = new OpenDental.ODtextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(429, 237);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkPrompt
			// 
			this.checkPrompt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrompt.Location = new System.Drawing.Point(47, 113);
			this.checkPrompt.Name = "checkPrompt";
			this.checkPrompt.Size = new System.Drawing.Size(83, 15);
			this.checkPrompt.TabIndex = 24;
			this.checkPrompt.Text = "Prompt";
			// 
			// comboPrinter
			// 
			this.comboPrinter.Location = new System.Drawing.Point(117, 72);
			this.comboPrinter.Name = "comboPrinter";
			this.comboPrinter.Size = new System.Drawing.Size(281, 21);
			this.comboPrinter.TabIndex = 169;
			// 
			// labelPrinter
			// 
			this.labelPrinter.Location = new System.Drawing.Point(16, 74);
			this.labelPrinter.Name = "labelPrinter";
			this.labelPrinter.Size = new System.Drawing.Size(99, 16);
			this.labelPrinter.TabIndex = 168;
			this.labelPrinter.Text = "Printer";
			this.labelPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkVirtualPrinter
			// 
			this.checkVirtualPrinter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkVirtualPrinter.Location = new System.Drawing.Point(-3, 144);
			this.checkVirtualPrinter.Name = "checkVirtualPrinter";
			this.checkVirtualPrinter.Size = new System.Drawing.Size(133, 15);
			this.checkVirtualPrinter.TabIndex = 171;
			this.checkVirtualPrinter.Text = "Virtual Printer";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 171);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 16);
			this.label1.TabIndex = 173;
			this.label1.Text = "File Extension";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFileExtension
			// 
			this.textFileExtension.AcceptsTab = true;
			this.textFileExtension.BackColor = System.Drawing.SystemColors.Window;
			this.textFileExtension.DetectUrls = false;
			this.textFileExtension.Location = new System.Drawing.Point(117, 169);
			this.textFileExtension.Name = "textFileExtension";
			this.textFileExtension.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Letter;
			this.textFileExtension.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textFileExtension.Size = new System.Drawing.Size(63, 21);
			this.textFileExtension.TabIndex = 174;
			this.textFileExtension.Text = "";
			// 
			// labelSituation
			// 
			this.labelSituation.Location = new System.Drawing.Point(16, 44);
			this.labelSituation.Name = "labelSituation";
			this.labelSituation.Size = new System.Drawing.Size(99, 16);
			this.labelSituation.TabIndex = 175;
			this.labelSituation.Text = "Situation";
			this.labelSituation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSituation
			// 
			this.textSituation.AcceptsTab = true;
			this.textSituation.BackColor = System.Drawing.SystemColors.Control;
			this.textSituation.DetectUrls = false;
			this.textSituation.Enabled = false;
			this.textSituation.Location = new System.Drawing.Point(117, 42);
			this.textSituation.Name = "textSituation";
			this.textSituation.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.FAQ;
			this.textSituation.ReadOnly = true;
			this.textSituation.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSituation.Size = new System.Drawing.Size(165, 21);
			this.textSituation.TabIndex = 176;
			this.textSituation.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(134, 108);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(368, 31);
			this.label11.TabIndex = 177;
			this.label11.Text = "It is recommended to use the prompt option for most functions.  But if you are us" +
    "ing a default, it is not necessary to check the box";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(354, 18);
			this.label2.TabIndex = 178;
			this.label2.Text = "These settings only apply to this workstation";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(186, 166);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(233, 33);
			this.label3.TabIndex = 179;
			this.label3.Text = "The file extension the selected virtual printer writes to (i.e. pdf, xps, etc.)";
			// 
			// FormPrinterEdit
			// 
			this.ClientSize = new System.Drawing.Size(516, 273);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textSituation);
			this.Controls.Add(this.labelSituation);
			this.Controls.Add(this.textFileExtension);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkVirtualPrinter);
			this.Controls.Add(this.comboPrinter);
			this.Controls.Add(this.labelPrinter);
			this.Controls.Add(this.checkPrompt);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrinterEdit";
			this.Text = "Edit Printer";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.CheckBox checkPrompt;
		private UI.ComboBox comboPrinter;
		private System.Windows.Forms.Label labelPrinter;
		private UI.CheckBox checkVirtualPrinter;
		private System.Windows.Forms.Label label1;
		private ODtextBox textFileExtension;
		private System.Windows.Forms.Label labelSituation;
		private ODtextBox textSituation;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}