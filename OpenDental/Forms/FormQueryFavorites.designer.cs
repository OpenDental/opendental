using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormQueryFavorites {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQueryFavorites));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label5 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.butShowHide = new OpenDental.UI.Button();
			this.checkWrapText = new OpenDental.UI.CheckBox();
			this.textQuery = new OpenDental.ODtextBox();
			this.splitContainer = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.splitContainer.SuspendLayout();
			this.splitterPanel1.SuspendLayout();
			this.splitterPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(917, 622);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(998, 622);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(368, 576);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(82, 24);
			this.butAdd.TabIndex = 34;
			this.butAdd.Text = "&New";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(6, 576);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 24);
			this.butDelete.TabIndex = 35;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEdit.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEdit.Location = new System.Drawing.Point(280, 576);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(82, 24);
			this.butEdit.TabIndex = 36;
			this.butEdit.Text = "Edit";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(7, 54);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(443, 516);
			this.gridMain.TabIndex = 38;
			this.gridMain.TitleVisible = false;
			this.gridMain.TranslationName = "TableQueryFavorites";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(7, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(173, 16);
			this.label5.TabIndex = 163;
			this.label5.Text = "Search:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(7, 31);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(443, 20);
			this.textSearch.TabIndex = 162;
			this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
			// 
			// butShowHide
			// 
			this.butShowHide.Location = new System.Drawing.Point(368, 2);
			this.butShowHide.Name = "butShowHide";
			this.butShowHide.Size = new System.Drawing.Size(82, 25);
			this.butShowHide.TabIndex = 40;
			this.butShowHide.Text = "Show Text >";
			this.butShowHide.UseVisualStyleBackColor = true;
			this.butShowHide.Click += new System.EventHandler(this.butShowHide_Click);
			// 
			// checkWrapText
			// 
			this.checkWrapText.Location = new System.Drawing.Point(5, 6);
			this.checkWrapText.Name = "checkWrapText";
			this.checkWrapText.Size = new System.Drawing.Size(227, 24);
			this.checkWrapText.TabIndex = 1;
			this.checkWrapText.Text = "Wrap Text";
			this.checkWrapText.CheckedChanged += new System.EventHandler(this.checkWrapText_CheckedChanged);
			// 
			// textQuery
			// 
			this.textQuery.AcceptsTab = true;
			this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textQuery.BackColor = System.Drawing.SystemColors.Control;
			this.textQuery.DetectLinksEnabled = false;
			this.textQuery.DetectUrls = false;
			this.textQuery.Location = new System.Drawing.Point(4, 32);
			this.textQuery.Name = "textQuery";
			this.textQuery.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.ReadOnly;
			this.textQuery.ReadOnly = true;
			this.textQuery.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textQuery.Size = new System.Drawing.Size(616, 568);
			this.textQuery.TabIndex = 0;
			this.textQuery.Text = "";
			this.textQuery.WordWrap = false;
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.ColorBorder = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.splitContainer.ColorSplitter = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.splitContainer.Controls.Add(this.splitterPanel1);
			this.splitContainer.Controls.Add(this.splitterPanel2);
			this.splitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainer.IsSplitterFixed = true;
			this.splitContainer.Location = new System.Drawing.Point(5, 12);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1 = this.splitterPanel1;
			this.splitContainer.Panel2 = this.splitterPanel2;
			this.splitContainer.Size = new System.Drawing.Size(1078, 604);
			this.splitContainer.SplitterDistance = 454;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 40;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Controls.Add(this.label5);
			this.splitterPanel1.Controls.Add(this.textSearch);
			this.splitterPanel1.Controls.Add(this.butEdit);
			this.splitterPanel1.Controls.Add(this.butShowHide);
			this.splitterPanel1.Controls.Add(this.butAdd);
			this.splitterPanel1.Controls.Add(this.butDelete);
			this.splitterPanel1.Controls.Add(this.gridMain);
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(454, 604);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Controls.Add(this.checkWrapText);
			this.splitterPanel2.Controls.Add(this.textQuery);
			this.splitterPanel2.Location = new System.Drawing.Point(455, 0);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(623, 604);
			this.splitterPanel2.TabIndex = 14;
			// 
			// FormQueryFavorites
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1084, 656);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormQueryFavorites";
			this.ShowInTaskbar = false;
			this.Text = "User Query Favorites";
			this.Load += new System.EventHandler(this.FormQueryFormulate_Load);
			this.SizeChanged += new System.EventHandler(this.FormQueryFavorites_SizeChanged);
			this.splitContainer.ResumeLayout(false);
			this.splitterPanel1.ResumeLayout(false);
			this.splitterPanel1.PerformLayout();
			this.splitterPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Form Controls
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butEdit;
		private UI.GridOD gridMain;
		private UI.Button butShowHide;
		private ODtextBox textQuery;
		private OpenDental.UI.CheckBox checkWrapText;
		private Label label5;
		private TextBox textSearch;
		#endregion

		private UI.SplitContainer splitContainer;
		private UI.SplitterPanel splitterPanel1;
		private UI.SplitterPanel splitterPanel2;
	}
}
