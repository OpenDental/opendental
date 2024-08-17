using System.Drawing;

namespace OpenDental {
	partial class FormUserQuery {

		#region Windows Form Designer generated code
		#region Fields
		public System.Windows.Forms.RadioButton radioHuman;
		public System.Windows.Forms.TextBox textTitle;
		private System.ComponentModel.Container components = null;// Required designer variable.
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butCopy;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butFavorite;
		private OpenDental.UI.Button butPaste;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butPrintPreview;
		private OpenDental.UI.Button butSubmit;
		private OpenDental.UI.CheckBox checkReportServer;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog2;
		private System.Windows.Forms.RadioButton radioRaw;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		#endregion

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserQuery));
			this.butClose = new OpenDental.UI.Button();
			this.butPaste = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butFavorite = new OpenDental.UI.Button();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboAlignment = new OpenDental.UI.ComboBox();
			this.radioHuman = new System.Windows.Forms.RadioButton();
			this.checkNumberedRows = new OpenDental.UI.CheckBox();
			this.radioRaw = new System.Windows.Forms.RadioButton();
			this.checkWordWrap = new OpenDental.UI.CheckBox();
			this.butSubmit = new OpenDental.UI.Button();
			this.printPreviewDialog2 = new System.Windows.Forms.PrintPreviewDialog();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			this.butPrint = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butPrintPreview = new OpenDental.UI.Button();
			this.textQuery = new OpenDental.ODcodeBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkReportServer = new OpenDental.UI.CheckBox();
			this.labelRowCount = new System.Windows.Forms.Label();
			this._gridResults = new OpenDental.UI.GridOD();
			this.splitContainer = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.splitterPanel1.SuspendLayout();
			this.splitterPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1197, 690);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(70, 24);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butPaste
			// 
			this.butPaste.Image = global::OpenDental.Properties.Resources.butPaste;
			this.butPaste.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPaste.Location = new System.Drawing.Point(78, 56);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(65, 23);
			this.butPaste.TabIndex = 11;
			this.butPaste.Text = "Paste";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// butCopy
			// 
			this.butCopy.Image = global::OpenDental.Properties.Resources.Copy;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(3, 56);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(72, 23);
			this.butCopy.TabIndex = 10;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// textTitle
			// 
			this.textTitle.Location = new System.Drawing.Point(64, 2);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(219, 20);
			this.textTitle.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Title";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(3, 32);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(140, 23);
			this.butAdd.TabIndex = 3;
			this.butAdd.Text = "Add To Favorites";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butFavorite
			// 
			this.butFavorite.Location = new System.Drawing.Point(3, 8);
			this.butFavorite.Name = "butFavorite";
			this.butFavorite.Size = new System.Drawing.Size(140, 23);
			this.butFavorite.TabIndex = 2;
			this.butFavorite.Text = "Favorites";
			this.butFavorite.Click += new System.EventHandler(this.butFavorites_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboAlignment);
			this.groupBox1.Controls.Add(this.radioHuman);
			this.groupBox1.Controls.Add(this.checkNumberedRows);
			this.groupBox1.Controls.Add(this.radioRaw);
			this.groupBox1.Controls.Add(this.checkWordWrap);
			this.groupBox1.Location = new System.Drawing.Point(194, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(394, 85);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.Text = "Format";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(132, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 18);
			this.label2.TabIndex = 18;
			this.label2.Text = "Column Alignment";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAlignment
			// 
			this.comboAlignment.Location = new System.Drawing.Point(248, 14);
			this.comboAlignment.Name = "comboAlignment";
			this.comboAlignment.Size = new System.Drawing.Size(121, 21);
			this.comboAlignment.TabIndex = 18;
			this.comboAlignment.Text = "comboBoxOD1";
			this.comboAlignment.SelectionChangeCommitted += new System.EventHandler(this.comboAlignment_SelectionChangeCommitted);
			// 
			// radioHuman
			// 
			this.radioHuman.Checked = true;
			this.radioHuman.Location = new System.Drawing.Point(6, 20);
			this.radioHuman.Name = "radioHuman";
			this.radioHuman.Size = new System.Drawing.Size(125, 18);
			this.radioHuman.TabIndex = 0;
			this.radioHuman.TabStop = true;
			this.radioHuman.Text = "Human-readable";
			this.radioHuman.Click += new System.EventHandler(this.radioHuman_Click);
			// 
			// checkNumberedRows
			// 
			this.checkNumberedRows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNumberedRows.Location = new System.Drawing.Point(125, 39);
			this.checkNumberedRows.Name = "checkNumberedRows";
			this.checkNumberedRows.Size = new System.Drawing.Size(138, 18);
			this.checkNumberedRows.TabIndex = 21;
			this.checkNumberedRows.Text = "Show Row Numbers";
			this.checkNumberedRows.Click += new System.EventHandler(this.checkNumberedRows_Clicked);
			// 
			// radioRaw
			// 
			this.radioRaw.Location = new System.Drawing.Point(6, 43);
			this.radioRaw.Name = "radioRaw";
			this.radioRaw.Size = new System.Drawing.Size(104, 18);
			this.radioRaw.TabIndex = 1;
			this.radioRaw.Text = "Raw";
			this.radioRaw.Click += new System.EventHandler(this.radioRaw_Click);
			// 
			// checkWordWrap
			// 
			this.checkWordWrap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWordWrap.Location = new System.Drawing.Point(125, 59);
			this.checkWordWrap.Name = "checkWordWrap";
			this.checkWordWrap.Size = new System.Drawing.Size(138, 18);
			this.checkWordWrap.TabIndex = 20;
			this.checkWordWrap.Text = "Enable word wrap";
			this.checkWordWrap.CheckedChanged += new System.EventHandler(this.checkWordWrap_CheckedChanged);
			// 
			// butSubmit
			// 
			this.butSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butSubmit.Location = new System.Drawing.Point(3, 80);
			this.butSubmit.Name = "butSubmit";
			this.butSubmit.Size = new System.Drawing.Size(102, 23);
			this.butSubmit.TabIndex = 6;
			this.butSubmit.Text = "&Submit Query";
			this.butSubmit.Click += new System.EventHandler(this.butSubmit_Click);
			// 
			// printPreviewDialog2
			// 
			this.printPreviewDialog2.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDialog2.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDialog2.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDialog2.Enabled = true;
			this.printPreviewDialog2.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog2.Icon")));
			this.printPreviewDialog2.Name = "printPreviewDialog2";
			this.printPreviewDialog2.Visible = false;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(1121, 690);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(70, 24);
			this.butPrint.TabIndex = 13;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(1036, 690);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 14;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrintPreview
			// 
			this.butPrintPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPrintPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintPreview.Location = new System.Drawing.Point(936, 690);
			this.butPrintPreview.Name = "butPrintPreview";
			this.butPrintPreview.Size = new System.Drawing.Size(94, 24);
			this.butPrintPreview.TabIndex = 16;
			this.butPrintPreview.Text = "P&rint Preview";
			this.butPrintPreview.Click += new System.EventHandler(this.butPrintPreview_Click);
			// 
			// textQuery
			// 
			this.textQuery.AcceptsTab = true;
			this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textQuery.DetectUrls = false;
			this.textQuery.Font = new System.Drawing.Font("Courier New", 9F);
			this.textQuery.Location = new System.Drawing.Point(3, 7);
			this.textQuery.Name = "textQuery";
			this.textQuery.Size = new System.Drawing.Size(666, 260);
			this.textQuery.TabIndex = 0;
			this.textQuery.Text = "";
			this.textQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textQuery_KeyDown);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.checkReportServer);
			this.panel1.Controls.Add(this.butFavorite);
			this.panel1.Controls.Add(this.butAdd);
			this.panel1.Controls.Add(this.butPaste);
			this.panel1.Controls.Add(this.butSubmit);
			this.panel1.Controls.Add(this.butCopy);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Location = new System.Drawing.Point(676, 7);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(591, 162);
			this.panel1.TabIndex = 17;
			// 
			// checkReportServer
			// 
			this.checkReportServer.Location = new System.Drawing.Point(5, 106);
			this.checkReportServer.Name = "checkReportServer";
			this.checkReportServer.Size = new System.Drawing.Size(183, 23);
			this.checkReportServer.TabIndex = 12;
			this.checkReportServer.Text = "Run on report server";
			this.checkReportServer.Visible = false;
			// 
			// labelRowCount
			// 
			this.labelRowCount.Location = new System.Drawing.Point(289, 0);
			this.labelRowCount.Name = "labelRowCount";
			this.labelRowCount.Size = new System.Drawing.Size(245, 23);
			this.labelRowCount.TabIndex = 18;
			this.labelRowCount.Text = "Row Count: ";
			this.labelRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelRowCount.Visible = false;
			// 
			// _gridResults
			// 
			this._gridResults.AllowSortingByColumn = true;
			this._gridResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._gridResults.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._gridResults.HScrollVisible = true;
			this._gridResults.Location = new System.Drawing.Point(0, 27);
			this._gridResults.Name = "_gridResults";
			this._gridResults.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this._gridResults.Size = new System.Drawing.Size(1271, 384);
			this._gridResults.TabIndex = 10;
			this._gridResults.TitleVisible = false;
			this._gridResults.WrapText = false;
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.Controls.Add(this.splitterPanel1);
			this.splitContainer.Controls.Add(this.splitterPanel2);
			this.splitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainer.Location = new System.Drawing.Point(2, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer.Panel1 = this.splitterPanel1;
			this.splitContainer.Panel2 = this.splitterPanel2;
			this.splitContainer.Size = new System.Drawing.Size(1274, 684);
			this.splitContainer.SplitterDistance = 268;
			this.splitContainer.TabIndex = 18;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Controls.Add(this.panel1);
			this.splitterPanel1.Controls.Add(this.textQuery);
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(1274, 268);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Controls.Add(this.labelRowCount);
			this.splitterPanel2.Controls.Add(this._gridResults);
			this.splitterPanel2.Controls.Add(this.label1);
			this.splitterPanel2.Controls.Add(this.textTitle);
			this.splitterPanel2.Location = new System.Drawing.Point(0, 272);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(1274, 412);
			this.splitterPanel2.TabIndex = 14;
			// 
			// FormUserQuery
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1276, 720);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPrintPreview);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butExport);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormUserQuery";
			this.Text = "User Query";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormQuery_FormClosing);
			this.Load += new System.EventHandler(this.FormQuery_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormQuery_Layout);
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.splitterPanel1.ResumeLayout(false);
			this.splitterPanel2.ResumeLayout(false);
			this.splitterPanel2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public ODcodeBox textQuery;
		private UI.GridOD _gridResults;
		private OpenDental.UI.CheckBox checkWordWrap;
		private System.Windows.Forms.Label labelRowCount;
		private OpenDental.UI.CheckBox checkNumberedRows;
		private System.Windows.Forms.Label label2;
		private UI.ComboBox comboAlignment;
		private UI.SplitContainer splitContainer;
		private UI.SplitterPanel splitterPanel1;
		private UI.SplitterPanel splitterPanel2;
	}
}
