namespace OpenDental{
	partial class FormProvStudentBulkEdit {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProvStudentBulkEdit));
			this.butBulkEdit = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.gridStudents = new OpenDental.UI.GridOD();
			this.label7 = new System.Windows.Forms.Label();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.comboClass = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.butOutlineColor = new System.Windows.Forms.Button();
			this.labelColor = new System.Windows.Forms.Label();
			this.butColor = new System.Windows.Forms.Button();
			this.groupCreateUsers = new System.Windows.Forms.GroupBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupCreateUsers.SuspendLayout();
			this.SuspendLayout();
			// 
			// butBulkEdit
			// 
			this.butBulkEdit.Location = new System.Drawing.Point(84, 65);
			this.butBulkEdit.Name = "butBulkEdit";
			this.butBulkEdit.Size = new System.Drawing.Size(75, 24);
			this.butBulkEdit.TabIndex = 3;
			this.butBulkEdit.Text = "Save";
			this.butBulkEdit.Click += new System.EventHandler(this.butBulkEdit_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(496, 458);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridStudents
			// 
			this.gridStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridStudents.Location = new System.Drawing.Point(12, 23);
			this.gridStudents.Name = "gridStudents";
			this.gridStudents.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridStudents.Size = new System.Drawing.Size(388, 459);
			this.gridStudents.TabIndex = 14;
			this.gridStudents.Title = "Students";
			this.gridStudents.TranslationName = "TableStudent";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(402, 67);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 18);
			this.label7.TabIndex = 29;
			this.label7.Text = "ProvNum";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textProvNum
			// 
			this.textProvNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textProvNum.Location = new System.Drawing.Point(405, 88);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.Size = new System.Drawing.Size(166, 20);
			this.textProvNum.TabIndex = 2;
			// 
			// comboClass
			// 
			this.comboClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClass.FormattingEnabled = true;
			this.comboClass.Location = new System.Drawing.Point(405, 44);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(166, 21);
			this.comboClass.TabIndex = 1;
			this.comboClass.SelectionChangeCommitted += new System.EventHandler(this.comboClass_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(405, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 18);
			this.label1.TabIndex = 27;
			this.label1.Text = "Classes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 42);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(117, 16);
			this.label14.TabIndex = 49;
			this.label14.Text = "Highlight Outline Color";
			this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butOutlineColor
			// 
			this.butOutlineColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butOutlineColor.Location = new System.Drawing.Point(129, 38);
			this.butOutlineColor.Name = "butOutlineColor";
			this.butOutlineColor.Size = new System.Drawing.Size(30, 20);
			this.butOutlineColor.TabIndex = 2;
			this.butOutlineColor.Click += new System.EventHandler(this.butOutlineColor_Click);
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(6, 22);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(117, 16);
			this.labelColor.TabIndex = 46;
			this.labelColor.Text = "Appointment Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(129, 18);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 1;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// groupCreateUsers
			// 
			this.groupCreateUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupCreateUsers.Controls.Add(this.label14);
			this.groupCreateUsers.Controls.Add(this.butColor);
			this.groupCreateUsers.Controls.Add(this.butOutlineColor);
			this.groupCreateUsers.Controls.Add(this.labelColor);
			this.groupCreateUsers.Controls.Add(this.butBulkEdit);
			this.groupCreateUsers.Location = new System.Drawing.Point(406, 114);
			this.groupCreateUsers.Name = "groupCreateUsers";
			this.groupCreateUsers.Size = new System.Drawing.Size(165, 95);
			this.groupCreateUsers.TabIndex = 3;
			this.groupCreateUsers.TabStop = false;
			this.groupCreateUsers.Text = "Edit Students";
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// FormProvStudentBulkEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(583, 494);
			this.Controls.Add(this.groupCreateUsers);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textProvNum);
			this.Controls.Add(this.comboClass);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridStudents);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProvStudentBulkEdit";
			this.Text = "Student Bulk Edit";
			this.Load += new System.EventHandler(this.FormProvStudentBulkEdit_Load);
			this.groupCreateUsers.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butBulkEdit;
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridStudents;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textProvNum;
		private System.Windows.Forms.ComboBox comboClass;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button butOutlineColor;
		private System.Windows.Forms.Label labelColor;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.GroupBox groupCreateUsers;
		private System.Windows.Forms.ColorDialog colorDialog1;
	}
}