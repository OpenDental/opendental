namespace OpenDental{
	partial class FormCertificationEdit{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCertificationEdit));
			this.listBoxCategories = new OpenDental.UI.ListBox();
			this.labelCategories = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.labelDescription = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelWikiPage = new System.Windows.Forms.Label();
			this.textWikiPage = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// listBoxCategories
			// 
			this.listBoxCategories.IntegralHeight = false;
			this.listBoxCategories.Location = new System.Drawing.Point(115, 73);
			this.listBoxCategories.Name = "listBoxCategories";
			this.listBoxCategories.Size = new System.Drawing.Size(180, 200);
			this.listBoxCategories.TabIndex = 3;
			this.listBoxCategories.Text = "listBoxOD1";
			// 
			// labelCategories
			// 
			this.labelCategories.Location = new System.Drawing.Point(12, 77);
			this.labelCategories.Name = "labelCategories";
			this.labelCategories.Size = new System.Drawing.Size(102, 34);
			this.labelCategories.TabIndex = 13;
			this.labelCategories.Text = "Category";
			this.labelCategories.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(332, 312);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(12, 27);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(102, 17);
			this.labelDescription.TabIndex = 11;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(115, 23);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(292, 20);
			this.textDescription.TabIndex = 1;
			// 
			// labelWikiPage
			// 
			this.labelWikiPage.Location = new System.Drawing.Point(12, 52);
			this.labelWikiPage.Name = "labelWikiPage";
			this.labelWikiPage.Size = new System.Drawing.Size(102, 17);
			this.labelWikiPage.TabIndex = 12;
			this.labelWikiPage.Text = "Wiki Page";
			this.labelWikiPage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWikiPage
			// 
			this.textWikiPage.Location = new System.Drawing.Point(115, 48);
			this.textWikiPage.Name = "textWikiPage";
			this.textWikiPage.Size = new System.Drawing.Size(292, 20);
			this.textWikiPage.TabIndex = 2;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 310);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(25, 278);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(104, 18);
			this.checkIsHidden.TabIndex = 4;
			this.checkIsHidden.Text = "Hidden";
			// 
			// FormCertificationEdit
			// 
			this.ClientSize = new System.Drawing.Size(426, 348);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.labelWikiPage);
			this.Controls.Add(this.textWikiPage);
			this.Controls.Add(this.labelCategories);
			this.Controls.Add(this.listBoxCategories);
			this.Controls.Add(this.checkIsHidden);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCertificationEdit";
			this.Text = "Certification Edit";
			this.Load += new System.EventHandler(this.FormCertificationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.ListBox listBoxCategories;
		private System.Windows.Forms.Label labelCategories;
		private UI.Button butSave;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelWikiPage;
		private System.Windows.Forms.TextBox textWikiPage;
		private UI.Button butDelete;
		private OpenDental.UI.CheckBox checkIsHidden;
	}
}