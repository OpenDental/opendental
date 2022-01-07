using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormQuickPaste {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQuickPaste));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listCat = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butDownCat = new OpenDental.UI.Button();
			this.butUpCat = new OpenDental.UI.Button();
			this.butAddCat = new OpenDental.UI.Button();
			this.butDeleteCat = new OpenDental.UI.Button();
			this.butAddNote = new OpenDental.UI.Button();
			this.butDownNote = new OpenDental.UI.Button();
			this.butUpNote = new OpenDental.UI.Button();
			this.butEditNote = new OpenDental.UI.Button();
			this.butDate = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAlphabetize = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioSortByNote = new System.Windows.Forms.RadioButton();
			this.radioSortByAbbrev = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(899, 641);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(818, 641);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listCat
			// 
			this.listCat.Location = new System.Drawing.Point(8, 25);
			this.listCat.Name = "listCat";
			this.listCat.Size = new System.Drawing.Size(169, 316);
			this.listCat.TabIndex = 2;
			this.listCat.DoubleClick += new System.EventHandler(this.listCat_DoubleClick);
			this.listCat.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listCat_MouseDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 19);
			this.label1.TabIndex = 3;
			this.label1.Text = "Category";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(181, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 19);
			this.label2.TabIndex = 5;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDownCat
			// 
			this.butDownCat.Image = global::OpenDental.Properties.Resources.down;
			this.butDownCat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDownCat.Location = new System.Drawing.Point(98, 383);
			this.butDownCat.Name = "butDownCat";
			this.butDownCat.Size = new System.Drawing.Size(79, 26);
			this.butDownCat.TabIndex = 11;
			this.butDownCat.Text = "Down";
			this.butDownCat.Click += new System.EventHandler(this.butDownCat_Click);
			// 
			// butUpCat
			// 
			this.butUpCat.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUpCat.Image = global::OpenDental.Properties.Resources.up;
			this.butUpCat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUpCat.Location = new System.Drawing.Point(98, 348);
			this.butUpCat.Name = "butUpCat";
			this.butUpCat.Size = new System.Drawing.Size(79, 26);
			this.butUpCat.TabIndex = 10;
			this.butUpCat.Text = "Up";
			this.butUpCat.Click += new System.EventHandler(this.butUpCat_Click);
			// 
			// butAddCat
			// 
			this.butAddCat.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCat.Location = new System.Drawing.Point(8, 348);
			this.butAddCat.Name = "butAddCat";
			this.butAddCat.Size = new System.Drawing.Size(79, 26);
			this.butAddCat.TabIndex = 12;
			this.butAddCat.Text = "Add";
			this.butAddCat.Click += new System.EventHandler(this.butAddCat_Click);
			// 
			// butDeleteCat
			// 
			this.butDeleteCat.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteCat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteCat.Location = new System.Drawing.Point(8, 383);
			this.butDeleteCat.Name = "butDeleteCat";
			this.butDeleteCat.Size = new System.Drawing.Size(79, 26);
			this.butDeleteCat.TabIndex = 13;
			this.butDeleteCat.Text = "Delete";
			this.butDeleteCat.Click += new System.EventHandler(this.butDeleteCat_Click);
			// 
			// butAddNote
			// 
			this.butAddNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNote.Location = new System.Drawing.Point(184, 641);
			this.butAddNote.Name = "butAddNote";
			this.butAddNote.Size = new System.Drawing.Size(79, 26);
			this.butAddNote.TabIndex = 16;
			this.butAddNote.Text = "Add";
			this.butAddNote.Click += new System.EventHandler(this.butAddNote_Click);
			// 
			// butDownNote
			// 
			this.butDownNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDownNote.Image = global::OpenDental.Properties.Resources.down;
			this.butDownNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDownNote.Location = new System.Drawing.Point(439, 641);
			this.butDownNote.Name = "butDownNote";
			this.butDownNote.Size = new System.Drawing.Size(79, 26);
			this.butDownNote.TabIndex = 15;
			this.butDownNote.Text = "Down";
			this.butDownNote.Click += new System.EventHandler(this.butDownNote_Click);
			// 
			// butUpNote
			// 
			this.butUpNote.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUpNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUpNote.Image = global::OpenDental.Properties.Resources.up;
			this.butUpNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUpNote.Location = new System.Drawing.Point(354, 641);
			this.butUpNote.Name = "butUpNote";
			this.butUpNote.Size = new System.Drawing.Size(79, 26);
			this.butUpNote.TabIndex = 14;
			this.butUpNote.Text = "Up";
			this.butUpNote.Click += new System.EventHandler(this.butUpNote_Click);
			// 
			// butEditNote
			// 
			this.butEditNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditNote.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditNote.Location = new System.Drawing.Point(269, 641);
			this.butEditNote.Name = "butEditNote";
			this.butEditNote.Size = new System.Drawing.Size(79, 26);
			this.butEditNote.TabIndex = 17;
			this.butEditNote.Text = "Edit";
			this.butEditNote.Click += new System.EventHandler(this.butEditNote_Click);
			// 
			// butDate
			// 
			this.butDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDate.Location = new System.Drawing.Point(737, 641);
			this.butDate.Name = "butDate";
			this.butDate.Size = new System.Drawing.Size(75, 26);
			this.butDate.TabIndex = 18;
			this.butDate.Text = "Date";
			this.butDate.Click += new System.EventHandler(this.butDate_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(184, 28);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(790, 601);
			this.gridMain.TabIndex = 19;
			this.gridMain.Title = "Notes";
			this.gridMain.TranslationName = "TableNotes";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAlphabetize
			// 
			this.butAlphabetize.Location = new System.Drawing.Point(6, 14);
			this.butAlphabetize.Name = "butAlphabetize";
			this.butAlphabetize.Size = new System.Drawing.Size(75, 26);
			this.butAlphabetize.TabIndex = 20;
			this.butAlphabetize.Text = "Alphabetize";
			this.butAlphabetize.Click += new System.EventHandler(this.butAlphabetize_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.radioSortByNote);
			this.groupBox1.Controls.Add(this.radioSortByAbbrev);
			this.groupBox1.Controls.Add(this.butAlphabetize);
			this.groupBox1.Location = new System.Drawing.Point(524, 627);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(207, 46);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			// 
			// radioSortByNote
			// 
			this.radioSortByNote.Location = new System.Drawing.Point(88, 26);
			this.radioSortByNote.Name = "radioSortByNote";
			this.radioSortByNote.Size = new System.Drawing.Size(85, 17);
			this.radioSortByNote.TabIndex = 22;
			this.radioSortByNote.Text = "Note";
			this.radioSortByNote.UseVisualStyleBackColor = true;
			// 
			// radioSortByAbbrev
			// 
			this.radioSortByAbbrev.Checked = true;
			this.radioSortByAbbrev.Location = new System.Drawing.Point(88, 9);
			this.radioSortByAbbrev.Name = "radioSortByAbbrev";
			this.radioSortByAbbrev.Size = new System.Drawing.Size(113, 17);
			this.radioSortByAbbrev.TabIndex = 21;
			this.radioSortByAbbrev.TabStop = true;
			this.radioSortByAbbrev.Text = "Abbreviation";
			this.radioSortByAbbrev.UseVisualStyleBackColor = true;
			// 
			// FormQuickPaste
			// 
			this.ClientSize = new System.Drawing.Size(986, 677);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDate);
			this.Controls.Add(this.butEditNote);
			this.Controls.Add(this.butAddNote);
			this.Controls.Add(this.butDownNote);
			this.Controls.Add(this.butUpNote);
			this.Controls.Add(this.butDeleteCat);
			this.Controls.Add(this.butAddCat);
			this.Controls.Add(this.butDownCat);
			this.Controls.Add(this.butUpCat);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listCat);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormQuickPaste";
			this.ShowInTaskbar = false;
			this.Text = "Quick Paste Notes";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormQuickPaste_FormClosing);
			this.Load += new System.EventHandler(this.FormQuickPaste_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listCat;
		private OpenDental.UI.Button butDownCat;
		private OpenDental.UI.Button butUpCat;
		private OpenDental.UI.Button butAddCat;
		private OpenDental.UI.Button butDeleteCat;
		private OpenDental.UI.Button butAddNote;
		private OpenDental.UI.Button butDownNote;
		private OpenDental.UI.Button butUpNote;
		private OpenDental.UI.Button butEditNote;
		private OpenDental.UI.Button butDate;
		private UI.GridOD gridMain;
		private UI.Button butAlphabetize;
		private GroupBox groupBox1;
		private RadioButton radioSortByNote;
		private RadioButton radioSortByAbbrev;
	}
}
