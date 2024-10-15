
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
			this.labelMinAge = new System.Windows.Forms.Label();
			this.labelMaxAge = new System.Windows.Forms.Label();
			this.labelFrequency = new System.Windows.Forms.Label();
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
			this.labelMinAgeHelp = new System.Windows.Forms.Label();
			this.groupBehavior = new OpenDental.UI.GroupBox();
			this.labelBehaviorNewHelp = new System.Windows.Forms.Label();
			this.labelBehaviorPreFillHelp = new System.Windows.Forms.Label();
			this.labelBehaviorOnceHelp = new System.Windows.Forms.Label();
			this.radioBehaviorOnce = new System.Windows.Forms.RadioButton();
			this.radioBehaviorPreFill = new System.Windows.Forms.RadioButton();
			this.radioBehaviorNew = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBehavior.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMinAge
			// 
			this.labelMinAge.Location = new System.Drawing.Point(28, 187);
			this.labelMinAge.Name = "labelMinAge";
			this.labelMinAge.Size = new System.Drawing.Size(120, 20);
			this.labelMinAge.TabIndex = 1;
			this.labelMinAge.Text = "Minimum Age";
			this.labelMinAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxAge
			// 
			this.labelMaxAge.Location = new System.Drawing.Point(28, 216);
			this.labelMaxAge.Name = "labelMaxAge";
			this.labelMaxAge.Size = new System.Drawing.Size(120, 20);
			this.labelMaxAge.TabIndex = 2;
			this.labelMaxAge.Text = "Maximum Age";
			this.labelMaxAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequency
			// 
			this.labelFrequency.Location = new System.Drawing.Point(28, 158);
			this.labelFrequency.Name = "labelFrequency";
			this.labelFrequency.Size = new System.Drawing.Size(120, 20);
			this.labelFrequency.TabIndex = 3;
			this.labelFrequency.Text = "Frequency (Days)";
			this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMinAge
			// 
			this.textMinAge.Location = new System.Drawing.Point(150, 188);
			this.textMinAge.Name = "textMinAge";
			this.textMinAge.Size = new System.Drawing.Size(35, 20);
			this.textMinAge.TabIndex = 7;
			// 
			// textMaxAge
			// 
			this.textMaxAge.Location = new System.Drawing.Point(150, 217);
			this.textMaxAge.Name = "textMaxAge";
			this.textMaxAge.Size = new System.Drawing.Size(35, 20);
			this.textMaxAge.TabIndex = 8;
			// 
			// labelFreqNote
			// 
			this.labelFreqNote.Location = new System.Drawing.Point(191, 161);
			this.labelFreqNote.Name = "labelFreqNote";
			this.labelFreqNote.Size = new System.Drawing.Size(549, 20);
			this.labelFreqNote.TabIndex = 9;
			this.labelFreqNote.Text = "How often should the patient be asked to resubmit this form? (In days, where 0 in" +
    "dicates only submit once.)";
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(150, 159);
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.Size = new System.Drawing.Size(35, 20);
			this.textFrequency.TabIndex = 10;
			this.textFrequency.TextChanged += new System.EventHandler(this.textFrequency_TextChanged);
			// 
			// labelSheet
			// 
			this.labelSheet.Location = new System.Drawing.Point(44, 9);
			this.labelSheet.Name = "labelSheet";
			this.labelSheet.Size = new System.Drawing.Size(100, 20);
			this.labelSheet.TabIndex = 11;
			this.labelSheet.Text = "Form";
			this.labelSheet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSheet
			// 
			this.textSheet.Location = new System.Drawing.Point(150, 10);
			this.textSheet.Name = "textSheet";
			this.textSheet.ReadOnly = true;
			this.textSheet.Size = new System.Drawing.Size(144, 20);
			this.textSheet.TabIndex = 12;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(681, 387);
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
			this.butSelectSheetsToIgnore.Location = new System.Drawing.Point(333, 262);
			this.butSelectSheetsToIgnore.Name = "butSelectSheetsToIgnore";
			this.butSelectSheetsToIgnore.Size = new System.Drawing.Size(60, 24);
			this.butSelectSheetsToIgnore.TabIndex = 20;
			this.butSelectSheetsToIgnore.Text = "Edit";
			this.butSelectSheetsToIgnore.UseVisualStyleBackColor = true;
			this.butSelectSheetsToIgnore.Click += new System.EventHandler(this.butSelectSheetsToIgnore_Click);
			// 
			// labelSheetsToIgnore
			// 
			this.labelSheetsToIgnore.Location = new System.Drawing.Point(28, 260);
			this.labelSheetsToIgnore.Name = "labelSheetsToIgnore";
			this.labelSheetsToIgnore.Size = new System.Drawing.Size(120, 20);
			this.labelSheetsToIgnore.TabIndex = 19;
			this.labelSheetsToIgnore.Text = "Sheets to Ignore";
			this.labelSheetsToIgnore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listSheetsToIgnore
			// 
			this.listSheetsToIgnore.Location = new System.Drawing.Point(150, 262);
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
			this.butDelete.Location = new System.Drawing.Point(12, 387);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 80;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelMinAgeHelp
			// 
			this.labelMinAgeHelp.Location = new System.Drawing.Point(191, 188);
			this.labelMinAgeHelp.Name = "labelMinAgeHelp";
			this.labelMinAgeHelp.Size = new System.Drawing.Size(136, 20);
			this.labelMinAgeHelp.TabIndex = 81;
			this.labelMinAgeHelp.Text = "or leave blank";
			this.labelMinAgeHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBehavior
			// 
			this.groupBehavior.BackColor = System.Drawing.Color.White;
			this.groupBehavior.Controls.Add(this.labelBehaviorNewHelp);
			this.groupBehavior.Controls.Add(this.labelBehaviorPreFillHelp);
			this.groupBehavior.Controls.Add(this.labelBehaviorOnceHelp);
			this.groupBehavior.Controls.Add(this.radioBehaviorOnce);
			this.groupBehavior.Controls.Add(this.radioBehaviorPreFill);
			this.groupBehavior.Controls.Add(this.radioBehaviorNew);
			this.groupBehavior.Location = new System.Drawing.Point(89, 40);
			this.groupBehavior.Name = "groupBehavior";
			this.groupBehavior.Size = new System.Drawing.Size(667, 103);
			this.groupBehavior.TabIndex = 250;
			this.groupBehavior.Text = "Behavior";
			// 
			// labelBehaviorNewHelp
			// 
			this.labelBehaviorNewHelp.Location = new System.Drawing.Point(80, 40);
			this.labelBehaviorNewHelp.Name = "labelBehaviorNewHelp";
			this.labelBehaviorNewHelp.Size = new System.Drawing.Size(567, 19);
			this.labelBehaviorNewHelp.TabIndex = 255;
			this.labelBehaviorNewHelp.Text = "Do not use this unless you actually want the form to be blank every time.";
			this.labelBehaviorNewHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBehaviorPreFillHelp
			// 
			this.labelBehaviorPreFillHelp.Location = new System.Drawing.Point(80, 18);
			this.labelBehaviorPreFillHelp.Name = "labelBehaviorPreFillHelp";
			this.labelBehaviorPreFillHelp.Size = new System.Drawing.Size(567, 19);
			this.labelBehaviorPreFillHelp.TabIndex = 254;
			this.labelBehaviorPreFillHelp.Text = "Form will be pre-filled with information from the database if possible.";
			this.labelBehaviorPreFillHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBehaviorOnceHelp
			// 
			this.labelBehaviorOnceHelp.Location = new System.Drawing.Point(80, 66);
			this.labelBehaviorOnceHelp.Name = "labelBehaviorOnceHelp";
			this.labelBehaviorOnceHelp.Size = new System.Drawing.Size(567, 31);
			this.labelBehaviorOnceHelp.TabIndex = 251;
			this.labelBehaviorOnceHelp.Text = "Only fill once per patient. Once filled, patient is not automatically prompted to" +
    " fill again, unless the form has been revised.";
			// 
			// radioBehaviorOnce
			// 
			this.radioBehaviorOnce.Location = new System.Drawing.Point(4, 64);
			this.radioBehaviorOnce.Name = "radioBehaviorOnce";
			this.radioBehaviorOnce.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioBehaviorOnce.Size = new System.Drawing.Size(70, 18);
			this.radioBehaviorOnce.TabIndex = 253;
			this.radioBehaviorOnce.Text = "Once";
			this.radioBehaviorOnce.UseVisualStyleBackColor = true;
			this.radioBehaviorOnce.CheckedChanged += new System.EventHandler(this.radioBehaviorOnce_CheckedChanged);
			// 
			// radioBehaviorPreFill
			// 
			this.radioBehaviorPreFill.Location = new System.Drawing.Point(4, 19);
			this.radioBehaviorPreFill.Name = "radioBehaviorPreFill";
			this.radioBehaviorPreFill.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioBehaviorPreFill.Size = new System.Drawing.Size(70, 18);
			this.radioBehaviorPreFill.TabIndex = 252;
			this.radioBehaviorPreFill.Text = "Pre-fill";
			this.radioBehaviorPreFill.UseVisualStyleBackColor = true;
			// 
			// radioBehaviorNew
			// 
			this.radioBehaviorNew.Location = new System.Drawing.Point(4, 41);
			this.radioBehaviorNew.Name = "radioBehaviorNew";
			this.radioBehaviorNew.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioBehaviorNew.Size = new System.Drawing.Size(70, 18);
			this.radioBehaviorNew.TabIndex = 251;
			this.radioBehaviorNew.Text = "New";
			this.radioBehaviorNew.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(191, 217);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 20);
			this.label1.TabIndex = 251;
			this.label1.Text = "or leave blank";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormEClipboardSheetRule
			// 
			this.ClientSize = new System.Drawing.Size(768, 423);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBehavior);
			this.Controls.Add(this.labelMinAgeHelp);
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
			this.Controls.Add(this.labelFrequency);
			this.Controls.Add(this.labelMaxAge);
			this.Controls.Add(this.labelMinAge);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEClipboardSheetRule";
			this.Text = "eClipboard Sheet Rule";
			this.Load += new System.EventHandler(this.FormEClipboardSheetRule_Load);
			this.groupBehavior.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelMinAge;
		private System.Windows.Forms.Label labelMaxAge;
		private System.Windows.Forms.Label labelFrequency;
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
		private System.Windows.Forms.Label labelMinAgeHelp;
		private UI.GroupBox groupBehavior;
		private System.Windows.Forms.RadioButton radioBehaviorPreFill;
		private System.Windows.Forms.RadioButton radioBehaviorNew;
		private System.Windows.Forms.RadioButton radioBehaviorOnce;
		private System.Windows.Forms.Label labelBehaviorOnceHelp;
		private System.Windows.Forms.Label labelBehaviorPreFillHelp;
		private System.Windows.Forms.Label labelBehaviorNewHelp;
		private System.Windows.Forms.Label label1;
	}
}