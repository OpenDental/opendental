
namespace OpenDental {
	partial class FormEClipboardSheetRule {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardSheetRule));
			this.labelBehavior = new System.Windows.Forms.Label();
			this.labelMinAge = new System.Windows.Forms.Label();
			this.labelMaxAge = new System.Windows.Forms.Label();
			this.labelFrequency = new System.Windows.Forms.Label();
			this.comboBehavior = new OpenDental.UI.ComboBox();
			this.checkMinAge = new OpenDental.UI.CheckBox();
			this.checkMaxAge = new OpenDental.UI.CheckBox();
			this.textMinAge = new System.Windows.Forms.TextBox();
			this.textMaxAge = new System.Windows.Forms.TextBox();
			this.labelFreqNote = new System.Windows.Forms.Label();
			this.textFrequency = new System.Windows.Forms.TextBox();
			this.labelSheet = new System.Windows.Forms.Label();
			this.textSheet = new System.Windows.Forms.TextBox();
			this.butSave = new OpenDental.UI.Button();
			this.butSelectSheetsToIgnore = new OpenDental.UI.Button();
			this.labelSheetsToIgnore = new System.Windows.Forms.Label();
			this.listSheetsToIgnore = new OpenDental.UI.ListBox();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelBehavior
			// 
			this.labelBehavior.Location = new System.Drawing.Point(79, 36);
			this.labelBehavior.Name = "labelBehavior";
			this.labelBehavior.Size = new System.Drawing.Size(120, 20);
			this.labelBehavior.TabIndex = 0;
			this.labelBehavior.Text = "Behavior";
			this.labelBehavior.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMinAge
			// 
			this.labelMinAge.Location = new System.Drawing.Point(79, 145);
			this.labelMinAge.Name = "labelMinAge";
			this.labelMinAge.Size = new System.Drawing.Size(120, 20);
			this.labelMinAge.TabIndex = 1;
			this.labelMinAge.Text = "Minimum Age";
			this.labelMinAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxAge
			// 
			this.labelMaxAge.Location = new System.Drawing.Point(79, 196);
			this.labelMaxAge.Name = "labelMaxAge";
			this.labelMaxAge.Size = new System.Drawing.Size(120, 20);
			this.labelMaxAge.TabIndex = 2;
			this.labelMaxAge.Text = "Maximum Age";
			this.labelMaxAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequency
			// 
			this.labelFrequency.Location = new System.Drawing.Point(79, 92);
			this.labelFrequency.Name = "labelFrequency";
			this.labelFrequency.Size = new System.Drawing.Size(120, 20);
			this.labelFrequency.TabIndex = 3;
			this.labelFrequency.Text = "Frequency (Days)";
			this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBehavior
			// 
			this.comboBehavior.Location = new System.Drawing.Point(205, 36);
			this.comboBehavior.Name = "comboBehavior";
			this.comboBehavior.Size = new System.Drawing.Size(144, 21);
			this.comboBehavior.TabIndex = 4;
			this.comboBehavior.Text = "comboBehavior";
			this.comboBehavior.SelectionChangeCommitted += new System.EventHandler(this.comboBehavior_SelectionChangeCommitted);
			// 
			// checkMinAge
			// 
			this.checkMinAge.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMinAge.Location = new System.Drawing.Point(4, 119);
			this.checkMinAge.Name = "checkMinAge";
			this.checkMinAge.Size = new System.Drawing.Size(215, 20);
			this.checkMinAge.TabIndex = 5;
			this.checkMinAge.Text = "Include minimum age requirement";
			this.checkMinAge.CheckedChanged += new System.EventHandler(this.checkMinAge_CheckedChanged);
			// 
			// checkMaxAge
			// 
			this.checkMaxAge.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMaxAge.Location = new System.Drawing.Point(4, 171);
			this.checkMaxAge.Name = "checkMaxAge";
			this.checkMaxAge.Size = new System.Drawing.Size(215, 20);
			this.checkMaxAge.TabIndex = 6;
			this.checkMaxAge.Text = "Include maximum age requirement";
			this.checkMaxAge.CheckedChanged += new System.EventHandler(this.checkMaxAge_CheckedChanged);
			// 
			// textMinAge
			// 
			this.textMinAge.Location = new System.Drawing.Point(205, 145);
			this.textMinAge.Name = "textMinAge";
			this.textMinAge.Size = new System.Drawing.Size(35, 20);
			this.textMinAge.TabIndex = 7;
			// 
			// textMaxAge
			// 
			this.textMaxAge.Location = new System.Drawing.Point(205, 197);
			this.textMaxAge.Name = "textMaxAge";
			this.textMaxAge.Size = new System.Drawing.Size(35, 20);
			this.textMaxAge.TabIndex = 8;
			// 
			// labelFreqNote
			// 
			this.labelFreqNote.Location = new System.Drawing.Point(79, 62);
			this.labelFreqNote.Name = "labelFreqNote";
			this.labelFreqNote.Size = new System.Drawing.Size(341, 30);
			this.labelFreqNote.TabIndex = 9;
			this.labelFreqNote.Text = "How often should the patient be asked to resubmit this form? (In days, where 0 in" +
    "dicates only submit once.)";
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(205, 93);
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.Size = new System.Drawing.Size(35, 20);
			this.textFrequency.TabIndex = 10;
			this.textFrequency.TextChanged += new System.EventHandler(this.textFrequency_TextChanged);
			// 
			// labelSheet
			// 
			this.labelSheet.Location = new System.Drawing.Point(99, 9);
			this.labelSheet.Name = "labelSheet";
			this.labelSheet.Size = new System.Drawing.Size(100, 20);
			this.labelSheet.TabIndex = 11;
			this.labelSheet.Text = "Form";
			this.labelSheet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSheet
			// 
			this.textSheet.Location = new System.Drawing.Point(205, 10);
			this.textSheet.Name = "textSheet";
			this.textSheet.ReadOnly = true;
			this.textSheet.Size = new System.Drawing.Size(144, 20);
			this.textSheet.TabIndex = 12;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(468, 366);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 13;
			this.butSave.Text = "&Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butSelectSheetsToIgnore
			// 
			this.butSelectSheetsToIgnore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSelectSheetsToIgnore.Location = new System.Drawing.Point(389, 223);
			this.butSelectSheetsToIgnore.Name = "butSelectSheetsToIgnore";
			this.butSelectSheetsToIgnore.Size = new System.Drawing.Size(60, 24);
			this.butSelectSheetsToIgnore.TabIndex = 20;
			this.butSelectSheetsToIgnore.Text = "Edit";
			this.butSelectSheetsToIgnore.UseVisualStyleBackColor = true;
			this.butSelectSheetsToIgnore.Click += new System.EventHandler(this.butSelectSheetsToIgnore_Click);
			// 
			// labelSheetsToIgnore
			// 
			this.labelSheetsToIgnore.Location = new System.Drawing.Point(79, 223);
			this.labelSheetsToIgnore.Name = "labelSheetsToIgnore";
			this.labelSheetsToIgnore.Size = new System.Drawing.Size(120, 20);
			this.labelSheetsToIgnore.TabIndex = 19;
			this.labelSheetsToIgnore.Text = "Sheets to Ignore";
			this.labelSheetsToIgnore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listSheetsToIgnore
			// 
			this.listSheetsToIgnore.Location = new System.Drawing.Point(206, 223);
			this.listSheetsToIgnore.Name = "listSheetsToIgnore";
			this.listSheetsToIgnore.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listSheetsToIgnore.Size = new System.Drawing.Size(177, 95);
			this.listSheetsToIgnore.TabIndex = 21;
			this.listSheetsToIgnore.Text = "listSheetsToIgnore";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 366);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 80;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormEClipboardSheetRule
			// 
			this.ClientSize = new System.Drawing.Size(555, 402);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listSheetsToIgnore);
			this.Controls.Add(this.butSelectSheetsToIgnore);
			this.Controls.Add(this.labelSheetsToIgnore);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.textSheet);
			this.Controls.Add(this.labelSheet);
			this.Controls.Add(this.textFrequency);
			this.Controls.Add(this.labelFreqNote);
			this.Controls.Add(this.textMaxAge);
			this.Controls.Add(this.textMinAge);
			this.Controls.Add(this.checkMaxAge);
			this.Controls.Add(this.checkMinAge);
			this.Controls.Add(this.comboBehavior);
			this.Controls.Add(this.labelFrequency);
			this.Controls.Add(this.labelMaxAge);
			this.Controls.Add(this.labelMinAge);
			this.Controls.Add(this.labelBehavior);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEClipboardSheetRule";
			this.Text = "eClipboard Sheet Rule";
			this.Load += new System.EventHandler(this.FormEClipboardSheetRule_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelBehavior;
		private System.Windows.Forms.Label labelMinAge;
		private System.Windows.Forms.Label labelMaxAge;
		private System.Windows.Forms.Label labelFrequency;
		private UI.ComboBox comboBehavior;
		private OpenDental.UI.CheckBox checkMinAge;
		private OpenDental.UI.CheckBox checkMaxAge;
		private System.Windows.Forms.TextBox textMinAge;
		private System.Windows.Forms.TextBox textMaxAge;
		private System.Windows.Forms.Label labelFreqNote;
		private System.Windows.Forms.TextBox textFrequency;
		private System.Windows.Forms.Label labelSheet;
		private System.Windows.Forms.TextBox textSheet;
		private UI.Button butSave;
		private UI.Button butSelectSheetsToIgnore;
		private System.Windows.Forms.Label labelSheetsToIgnore;
		private UI.ListBox listSheetsToIgnore;
		private UI.Button butDelete;
	}
}