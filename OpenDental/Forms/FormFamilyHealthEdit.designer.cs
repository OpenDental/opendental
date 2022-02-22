namespace OpenDental{
	partial class FormFamilyHealthEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFamilyHealthEdit));
			this.listRelationship = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textName = new OpenDental.ODtextBox();
			this.textSnomed = new OpenDental.ODtextBox();
			this.textProblem = new OpenDental.ODtextBox();
			this.butPick = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listRelationship
			// 
			this.listRelationship.Location = new System.Drawing.Point(129, 29);
			this.listRelationship.Name = "listRelationship";
			this.listRelationship.Size = new System.Drawing.Size(145, 43);
			this.listRelationship.TabIndex = 63;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 130);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(114, 20);
			this.label3.TabIndex = 72;
			this.label3.Text = "SNOMED CT Code";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 20);
			this.label1.TabIndex = 75;
			this.label1.Text = "Problem";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(114, 20);
			this.label4.TabIndex = 76;
			this.label4.Text = "Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(114, 20);
			this.label2.TabIndex = 77;
			this.label2.Text = "Relationship";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.AcceptsTab = true;
			this.textName.DetectUrls = false;
			this.textName.Location = new System.Drawing.Point(129, 78);
			this.textName.Multiline = false;
			this.textName.Name = "textName";
			this.textName.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textName.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textName.Size = new System.Drawing.Size(145, 20);
			this.textName.TabIndex = 74;
			this.textName.Text = "";
			// 
			// textSnomed
			// 
			this.textSnomed.AcceptsTab = true;
			this.textSnomed.DetectUrls = false;
			this.textSnomed.Location = new System.Drawing.Point(129, 130);
			this.textSnomed.Multiline = false;
			this.textSnomed.Name = "textSnomed";
			this.textSnomed.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textSnomed.ReadOnly = true;
			this.textSnomed.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSnomed.Size = new System.Drawing.Size(207, 20);
			this.textSnomed.TabIndex = 68;
			this.textSnomed.Text = "";
			// 
			// textProblem
			// 
			this.textProblem.AcceptsTab = true;
			this.textProblem.DetectUrls = false;
			this.textProblem.Location = new System.Drawing.Point(129, 104);
			this.textProblem.Multiline = false;
			this.textProblem.Name = "textProblem";
			this.textProblem.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textProblem.ReadOnly = true;
			this.textProblem.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textProblem.Size = new System.Drawing.Size(337, 20);
			this.textProblem.TabIndex = 67;
			this.textProblem.Text = "";
			// 
			// butPick
			// 
			this.butPick.Location = new System.Drawing.Point(472, 104);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(52, 20);
			this.butPick.TabIndex = 60;
			this.butPick.Text = "Pick";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(21, 184);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 22;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(391, 184);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(472, 184);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormFamilyHealthEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(559, 220);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textSnomed);
			this.Controls.Add(this.textProblem);
			this.Controls.Add(this.listRelationship);
			this.Controls.Add(this.butPick);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFamilyHealthEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Family Health Edit";
			this.Load += new System.EventHandler(this.FormFamilyHealthEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private UI.Button butPick;
		private OpenDental.UI.ListBoxOD listRelationship;
		private ODtextBox textProblem;
		private ODtextBox textSnomed;
		private System.Windows.Forms.Label label3;
		private ODtextBox textName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
	}
}