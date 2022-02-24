using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMedications {
		/// <summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedications));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butAddGeneric = new OpenDental.UI.Button();
			this.butAddBrand = new OpenDental.UI.Button();
			this.gridAllMedications = new OpenDental.UI.GridOD();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabMedications = new System.Windows.Forms.TabControl();
			this.tabAllMedications = new System.Windows.Forms.TabPage();
			this.butExportMedications = new OpenDental.UI.Button();
			this.butImportMedications = new OpenDental.UI.Button();
			this.tabMissing = new System.Windows.Forms.TabPage();
			this.butConvertBrand = new OpenDental.UI.Button();
			this.butConvertGeneric = new OpenDental.UI.Button();
			this.gridMissing = new OpenDental.UI.GridOD();
			this.tabMedications.SuspendLayout();
			this.tabAllMedications.SuspendLayout();
			this.tabMissing.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(858, 635);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(777, 635);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAddGeneric
			// 
			this.butAddGeneric.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddGeneric.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGeneric.Location = new System.Drawing.Point(6, 6);
			this.butAddGeneric.Name = "butAddGeneric";
			this.butAddGeneric.Size = new System.Drawing.Size(113, 26);
			this.butAddGeneric.TabIndex = 33;
			this.butAddGeneric.Text = "Add Generic";
			this.butAddGeneric.Click += new System.EventHandler(this.butAddGeneric_Click);
			// 
			// butAddBrand
			// 
			this.butAddBrand.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddBrand.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddBrand.Location = new System.Drawing.Point(125, 6);
			this.butAddBrand.Name = "butAddBrand";
			this.butAddBrand.Size = new System.Drawing.Size(113, 26);
			this.butAddBrand.TabIndex = 34;
			this.butAddBrand.Text = "Add Brand";
			this.butAddBrand.Click += new System.EventHandler(this.butAddBrand_Click);
			// 
			// gridAllMedications
			// 
			this.gridAllMedications.AllowSortingByColumn = true;
			this.gridAllMedications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAllMedications.Location = new System.Drawing.Point(5, 38);
			this.gridAllMedications.Name = "gridAllMedications";
			this.gridAllMedications.Size = new System.Drawing.Size(907, 558);
			this.gridAllMedications.TabIndex = 37;
			this.gridAllMedications.Title = "All Medications";
			this.gridAllMedications.TranslationName = "FormMedications";
			this.gridAllMedications.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAllMedications_CellDoubleClick);
			this.gridAllMedications.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAllMedications_CellClick);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(367, 9);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(195, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(239, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(127, 17);
			this.label1.TabIndex = 39;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tabMedications
			// 
			this.tabMedications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabMedications.Controls.Add(this.tabAllMedications);
			this.tabMedications.Controls.Add(this.tabMissing);
			this.tabMedications.Location = new System.Drawing.Point(9, 3);
			this.tabMedications.Name = "tabMedications";
			this.tabMedications.SelectedIndex = 0;
			this.tabMedications.Size = new System.Drawing.Size(924, 626);
			this.tabMedications.TabIndex = 40;
			this.tabMedications.SelectedIndexChanged += new System.EventHandler(this.tabMedications_SelectedIndexChanged);
			// 
			// tabAllMedications
			// 
			this.tabAllMedications.Controls.Add(this.butExportMedications);
			this.tabAllMedications.Controls.Add(this.butImportMedications);
			this.tabAllMedications.Controls.Add(this.gridAllMedications);
			this.tabAllMedications.Controls.Add(this.textSearch);
			this.tabAllMedications.Controls.Add(this.butAddGeneric);
			this.tabAllMedications.Controls.Add(this.label1);
			this.tabAllMedications.Controls.Add(this.butAddBrand);
			this.tabAllMedications.Location = new System.Drawing.Point(4, 22);
			this.tabAllMedications.Name = "tabAllMedications";
			this.tabAllMedications.Padding = new System.Windows.Forms.Padding(3);
			this.tabAllMedications.Size = new System.Drawing.Size(916, 600);
			this.tabAllMedications.TabIndex = 0;
			this.tabAllMedications.Text = "All Medications";
			this.tabAllMedications.UseVisualStyleBackColor = true;
			// 
			// butExportMedications
			// 
			this.butExportMedications.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butExportMedications.Location = new System.Drawing.Point(825, 6);
			this.butExportMedications.Name = "butExportMedications";
			this.butExportMedications.Size = new System.Drawing.Size(85, 26);
			this.butExportMedications.TabIndex = 42;
			this.butExportMedications.Text = "Export";
			this.butExportMedications.Click += new System.EventHandler(this.butExportMedications_Click);
			// 
			// butImportMedications
			// 
			this.butImportMedications.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butImportMedications.Location = new System.Drawing.Point(734, 6);
			this.butImportMedications.Name = "butImportMedications";
			this.butImportMedications.Size = new System.Drawing.Size(85, 26);
			this.butImportMedications.TabIndex = 41;
			this.butImportMedications.Text = "Import";
			this.butImportMedications.Click += new System.EventHandler(this.butImportMedications_Click);
			// 
			// tabMissing
			// 
			this.tabMissing.Controls.Add(this.butConvertBrand);
			this.tabMissing.Controls.Add(this.butConvertGeneric);
			this.tabMissing.Controls.Add(this.gridMissing);
			this.tabMissing.Location = new System.Drawing.Point(4, 22);
			this.tabMissing.Name = "tabMissing";
			this.tabMissing.Size = new System.Drawing.Size(916, 600);
			this.tabMissing.TabIndex = 2;
			this.tabMissing.Text = "Missing Generic/Brand";
			this.tabMissing.UseVisualStyleBackColor = true;
			// 
			// butConvertBrand
			// 
			this.butConvertBrand.Icon = OpenDental.UI.EnumIcons.Add;
			this.butConvertBrand.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butConvertBrand.Location = new System.Drawing.Point(161, 6);
			this.butConvertBrand.Name = "butConvertBrand";
			this.butConvertBrand.Size = new System.Drawing.Size(150, 26);
			this.butConvertBrand.TabIndex = 40;
			this.butConvertBrand.Text = "Convert To Brand";
			this.butConvertBrand.Click += new System.EventHandler(this.butConvertBrand_Click);
			// 
			// butConvertGeneric
			// 
			this.butConvertGeneric.Icon = OpenDental.UI.EnumIcons.Add;
			this.butConvertGeneric.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butConvertGeneric.Location = new System.Drawing.Point(5, 6);
			this.butConvertGeneric.Name = "butConvertGeneric";
			this.butConvertGeneric.Size = new System.Drawing.Size(150, 26);
			this.butConvertGeneric.TabIndex = 39;
			this.butConvertGeneric.Text = "Convert To Generic";
			this.butConvertGeneric.Click += new System.EventHandler(this.butConvertGeneric_Click);
			// 
			// gridMissing
			// 
			this.gridMissing.AllowSortingByColumn = true;
			this.gridMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMissing.Location = new System.Drawing.Point(5, 38);
			this.gridMissing.Name = "gridMissing";
			this.gridMissing.Size = new System.Drawing.Size(907, 559);
			this.gridMissing.TabIndex = 38;
			this.gridMissing.Title = "Medications Missing Generic or Brand";
			this.gridMissing.TranslationName = "FormMedications";
			// 
			// FormMedications
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(941, 671);
			this.Controls.Add(this.tabMedications);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMedications";
			this.ShowInTaskbar = false;
			this.Text = "Medications";
			this.Load += new System.EventHandler(this.FormMedications_Load);
			this.Shown += new System.EventHandler(this.FormMedications_Shown);
			this.tabMedications.ResumeLayout(false);
			this.tabAllMedications.ResumeLayout(false);
			this.tabAllMedications.PerformLayout();
			this.tabMissing.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butAddGeneric;
		private OpenDental.UI.Button butAddBrand;
		private OpenDental.UI.GridOD gridAllMedications;
		public TextBox textSearch;
		private Label label1;
		private TabControl tabMedications;
		private TabPage tabAllMedications;
		private TabPage tabMissing;
		private UI.GridOD gridMissing;
		private UI.Button butConvertBrand;
		private UI.Button butImportMedications;
		private UI.Button butExportMedications;
		private UI.Button butConvertGeneric;
	}
}
