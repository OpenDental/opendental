namespace OpenDental{
	partial class FormSheetFieldExam {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetFieldExam));
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxExamSheets = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.listBoxAvailFields = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 16);
			this.label2.TabIndex = 86;
			this.label2.Text = "Exam Sheet";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listExamSheets
			// 
			this.listBoxExamSheets.Location = new System.Drawing.Point(15, 66);
			this.listBoxExamSheets.Name = "listExamSheets";
			this.listBoxExamSheets.Size = new System.Drawing.Size(142, 290);
			this.listBoxExamSheets.TabIndex = 85;
			this.listBoxExamSheets.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listExamSheets_MouseClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(393, 33);
			this.label1.TabIndex = 87;
			this.label1.Text = "The value for this field will be retrieved later from the most recent exam sheet." +
    "";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(274, 376);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(355, 376);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(193, 47);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(108, 16);
			this.label10.TabIndex = 102;
			this.label10.Text = "Available Fields";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listAvailFields
			// 
			this.listBoxAvailFields.Location = new System.Drawing.Point(195, 66);
			this.listBoxAvailFields.Name = "listAvailFields";
			this.listBoxAvailFields.Size = new System.Drawing.Size(142, 290);
			this.listBoxAvailFields.TabIndex = 101;
			this.listBoxAvailFields.DoubleClick += new System.EventHandler(this.listAvailFields_DoubleClick);
			// 
			// FormSheetFieldExam
			// 
			this.ClientSize = new System.Drawing.Size(442, 412);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.listBoxAvailFields);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listBoxExamSheets);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetFieldExam";
			this.Text = "Edit Exam Sheet Field";
			this.Load += new System.EventHandler(this.FormSheetFieldDefEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listBoxExamSheets;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label10;
		private OpenDental.UI.ListBoxOD listBoxAvailFields;
	}
}