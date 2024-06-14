namespace OpenDental{
	partial class FormOrthoHardwareEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoHardwareEdit));
			this.butSave = new OpenDental.UI.Button();
			this.labelTeeth = new System.Windows.Forms.Label();
			this.textToothRange = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textType = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateExam = new OpenDental.ValidDate();
			this.label4 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.labelComments = new System.Windows.Forms.Label();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(401, 191);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelTeeth
			// 
			this.labelTeeth.Location = new System.Drawing.Point(30, 88);
			this.labelTeeth.Name = "labelTeeth";
			this.labelTeeth.Size = new System.Drawing.Size(99, 16);
			this.labelTeeth.TabIndex = 26;
			this.labelTeeth.Text = "Teeth";
			this.labelTeeth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textToothRange
			// 
			this.textToothRange.Location = new System.Drawing.Point(131, 85);
			this.textToothRange.Name = "textToothRange";
			this.textToothRange.Size = new System.Drawing.Size(132, 20);
			this.textToothRange.TabIndex = 25;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 16);
			this.label2.TabIndex = 30;
			this.label2.Text = "Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textType
			// 
			this.textType.Location = new System.Drawing.Point(131, 54);
			this.textType.Name = "textType";
			this.textType.ReadOnly = true;
			this.textType.Size = new System.Drawing.Size(99, 20);
			this.textType.TabIndex = 29;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 191);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(88, 24);
			this.butDelete.TabIndex = 31;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(122, 16);
			this.label3.TabIndex = 32;
			this.label3.Text = "Exam Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateExam
			// 
			this.textDateExam.Location = new System.Drawing.Point(131, 21);
			this.textDateExam.Name = "textDateExam";
			this.textDateExam.Size = new System.Drawing.Size(76, 20);
			this.textDateExam.TabIndex = 101;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(48, 118);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(81, 16);
			this.label4.TabIndex = 103;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(131, 116);
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(339, 20);
			this.textNote.TabIndex = 102;
			// 
			// labelComments
			// 
			this.labelComments.Location = new System.Drawing.Point(268, 88);
			this.labelComments.Name = "labelComments";
			this.labelComments.Size = new System.Drawing.Size(244, 16);
			this.labelComments.TabIndex = 104;
			this.labelComments.Text = "Comments";
			this.labelComments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.Location = new System.Drawing.Point(131, 142);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(176, 24);
			this.checkIsHidden.TabIndex = 105;
			this.checkIsHidden.Text = "(removed)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 145);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 16);
			this.label1.TabIndex = 106;
			this.label1.Text = "Hidden";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormOrthoHardwareEdit
			// 
			this.ClientSize = new System.Drawing.Size(488, 227);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.labelComments);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textDateExam);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textType);
			this.Controls.Add(this.labelTeeth);
			this.Controls.Add(this.textToothRange);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoHardwareEdit";
			this.Text = "Edit Ortho Hardware";
			this.Load += new System.EventHandler(this.FormOrthoHardwareEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label labelTeeth;
		private System.Windows.Forms.TextBox textToothRange;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textType;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label3;
		private ValidDate textDateExam;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label labelComments;
		private OpenDental.UI.CheckBox checkIsHidden;
		private System.Windows.Forms.Label label1;
	}
}