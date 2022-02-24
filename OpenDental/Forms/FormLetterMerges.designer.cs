using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLetterMerges {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLetterMerges));
			this.butCancel = new OpenDental.UI.Button();
			this.listCategories = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.pd2 = new System.Drawing.Printing.PrintDocument();
			this.butMerge = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listLetters = new OpenDental.UI.ListBoxOD();
			this.butEditCats = new OpenDental.UI.Button();
			this.butCreateData = new OpenDental.UI.Button();
			this.butEditTemplate = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboImageCategory = new OpenDental.UI.ComboBoxOD();
			this.labelImageCategory = new System.Windows.Forms.Label();
			this.butViewData = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(462, 405);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(79, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listCategories
			// 
			this.listCategories.Location = new System.Drawing.Point(15, 33);
			this.listCategories.Name = "listCategories";
			this.listCategories.Size = new System.Drawing.Size(164, 368);
			this.listCategories.TabIndex = 2;
			this.listCategories.Click += new System.EventHandler(this.listCategories_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 14);
			this.label1.TabIndex = 3;
			this.label1.Text = "Categories";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(206, 408);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 24);
			this.butAdd.TabIndex = 7;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butMerge
			// 
			this.butMerge.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butMerge.Location = new System.Drawing.Point(38, 84);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(79, 24);
			this.butMerge.TabIndex = 17;
			this.butMerge.Text = "Print";
			this.butMerge.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(205, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(124, 14);
			this.label3.TabIndex = 19;
			this.label3.Text = "Letters";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listLetters
			// 
			this.listLetters.Location = new System.Drawing.Point(206, 33);
			this.listLetters.Name = "listLetters";
			this.listLetters.Size = new System.Drawing.Size(164, 368);
			this.listLetters.TabIndex = 18;
			this.listLetters.Click += new System.EventHandler(this.listLetters_Click);
			this.listLetters.DoubleClick += new System.EventHandler(this.listLetters_DoubleClick);
			// 
			// butEditCats
			// 
			this.butEditCats.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditCats.Location = new System.Drawing.Point(14, 408);
			this.butEditCats.Name = "butEditCats";
			this.butEditCats.Size = new System.Drawing.Size(98, 24);
			this.butEditCats.TabIndex = 20;
			this.butEditCats.Text = "Edit Categories";
			this.butEditCats.Click += new System.EventHandler(this.butEditCats_Click);
			// 
			// butCreateData
			// 
			this.butCreateData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCreateData.Location = new System.Drawing.Point(38, 22);
			this.butCreateData.Name = "butCreateData";
			this.butCreateData.Size = new System.Drawing.Size(79, 24);
			this.butCreateData.TabIndex = 21;
			this.butCreateData.Text = "Data File";
			this.butCreateData.Click += new System.EventHandler(this.butCreateData_Click);
			// 
			// butEditTemplate
			// 
			this.butEditTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditTemplate.Location = new System.Drawing.Point(449, 348);
			this.butEditTemplate.Name = "butEditTemplate";
			this.butEditTemplate.Size = new System.Drawing.Size(92, 24);
			this.butEditTemplate.TabIndex = 22;
			this.butEditTemplate.Text = "Edit Template";
			this.butEditTemplate.Click += new System.EventHandler(this.butEditTemplate_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboImageCategory);
			this.groupBox1.Controls.Add(this.labelImageCategory);
			this.groupBox1.Controls.Add(this.butViewData);
			this.groupBox1.Controls.Add(this.butPreview);
			this.groupBox1.Controls.Add(this.butMerge);
			this.groupBox1.Controls.Add(this.butCreateData);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(415, 128);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(158, 197);
			this.groupBox1.TabIndex = 23;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Create";
			// 
			// comboImageCategory
			// 
			this.comboImageCategory.Location = new System.Drawing.Point(15, 164);
			this.comboImageCategory.Name = "comboImageCategory";
			this.comboImageCategory.Size = new System.Drawing.Size(137, 21);
			this.comboImageCategory.TabIndex = 37;
			// 
			// labelImageCategory
			// 
			this.labelImageCategory.Location = new System.Drawing.Point(13, 147);
			this.labelImageCategory.Name = "labelImageCategory";
			this.labelImageCategory.Size = new System.Drawing.Size(124, 14);
			this.labelImageCategory.TabIndex = 38;
			this.labelImageCategory.Text = "Image Folder";
			this.labelImageCategory.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butViewData
			// 
			this.butViewData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butViewData.Location = new System.Drawing.Point(38, 53);
			this.butViewData.Name = "butViewData";
			this.butViewData.Size = new System.Drawing.Size(79, 24);
			this.butViewData.TabIndex = 23;
			this.butViewData.Text = "View Data";
			this.butViewData.Click += new System.EventHandler(this.butViewData_Click);
			// 
			// butPreview
			// 
			this.butPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPreview.Location = new System.Drawing.Point(38, 115);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(79, 24);
			this.butPreview.TabIndex = 22;
			this.butPreview.Text = "Preview";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// FormLetterMerges
			// 
			this.ClientSize = new System.Drawing.Size(579, 446);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butEditTemplate);
			this.Controls.Add(this.butEditCats);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listLetters);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listCategories);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLetterMerges";
			this.ShowInTaskbar = false;
			this.Text = "Letter Merge";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormLetterMerges_Closing);
			this.Load += new System.EventHandler(this.FormLetterMerges_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listCategories;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listLetters;
		private OpenDental.UI.Button butEditCats;
		private OpenDental.UI.Button butMerge;
		private OpenDental.UI.Button butCreateData;
		private OpenDental.UI.Button butEditTemplate;
		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.UI.Button butViewData;
		private OpenDental.UI.Button butPreview;
		private UI.ComboBoxOD comboImageCategory;
		private Label labelImageCategory;
		private System.Drawing.Printing.PrintDocument pd2;
	}
}
