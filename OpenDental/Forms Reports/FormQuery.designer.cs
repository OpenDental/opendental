namespace OpenDental {
	partial class FormQuery {

		#region Windows Form Designer generated code
		#region Fields
		public System.Windows.Forms.RadioButton radioHuman;
		public System.Windows.Forms.TextBox textTitle;
		public OpenDental.ODtextBox textQuery;
		private System.ComponentModel.Container components = null;// Required designer variable.
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butBack;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butCopy;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butExportExcel;
		private OpenDental.UI.Button butFavorite;
		private OpenDental.UI.Button butFwd;
		private OpenDental.UI.Button butPaste;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butPrintPreview;
		private OpenDental.UI.Button butQView;
		private OpenDental.UI.Button butSubmit;
		private System.Windows.Forms.CheckBox checkReportServer;
		private System.Windows.Forms.DataGrid grid2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelTotPages;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridTableStyle myGridTS;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panelZoom;
		private OpenDental.UI.ODPrintPreviewControl printPreviewControl2;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog2;
		private System.Windows.Forms.RadioButton radioRaw;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private System.Windows.Forms.SplitContainer splitContainerQuery;
		#endregion

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQuery));
			this.butClose = new OpenDental.UI.Button();
			this.grid2 = new System.Windows.Forms.DataGrid();
			this.textQuery = new OpenDental.ODtextBox();
			this.butExportExcel = new OpenDental.UI.Button();
			this.butPaste = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butFavorite = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioHuman = new System.Windows.Forms.RadioButton();
			this.radioRaw = new System.Windows.Forms.RadioButton();
			this.butSubmit = new OpenDental.UI.Button();
			this.printPreviewDialog2 = new System.Windows.Forms.PrintPreviewDialog();
			this.printPreviewControl2 = new OpenDental.UI.ODPrintPreviewControl();
			this.panelZoom = new System.Windows.Forms.Panel();
			this.labelTotPages = new System.Windows.Forms.Label();
			this.butBack = new OpenDental.UI.Button();
			this.butFwd = new OpenDental.UI.Button();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			this.butPrint = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butQView = new OpenDental.UI.Button();
			this.butPrintPreview = new OpenDental.UI.Button();
			this.splitContainerQuery = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkReportServer = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.grid2)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.panelZoom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerQuery)).BeginInit();
			this.splitContainerQuery.Panel1.SuspendLayout();
			this.splitContainerQuery.Panel2.SuspendLayout();
			this.splitContainerQuery.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1151, 666);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(70, 24);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// grid2
			// 
			this.grid2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid2.BackgroundColor = System.Drawing.Color.Gainsboro;
			this.grid2.DataMember = "";
			this.grid2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.grid2.Location = new System.Drawing.Point(3, 25);
			this.grid2.Name = "grid2";
			this.grid2.ReadOnly = true;
			this.grid2.Size = new System.Drawing.Size(1474, 500);
			this.grid2.TabIndex = 2;
			// 
			// textQuery
			// 
			this.textQuery.AcceptsTab = true;
			this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textQuery.BackColor = System.Drawing.SystemColors.Window;
			this.textQuery.DetectLinksEnabled = false;
			this.textQuery.DetectUrls = false;
			this.textQuery.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textQuery.Location = new System.Drawing.Point(0, 0);
			this.textQuery.Margin = new System.Windows.Forms.Padding(3, 3, 3, 25);
			this.textQuery.Name = "textQuery";
			this.textQuery.QuickPasteType = OpenDentBusiness.QuickPasteType.Query;
			this.textQuery.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textQuery.Size = new System.Drawing.Size(557, 123);
			this.textQuery.SpellCheckIsEnabled = false;
			this.textQuery.TabIndex = 16;
			this.textQuery.Text = "";
			this.textQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textQuery_KeyDown);
			// 
			// butExportExcel
			// 
			this.butExportExcel.Image = global::OpenDental.Properties.Resources.butExportExcel;
			this.butExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExportExcel.Location = new System.Drawing.Point(159, 71);
			this.butExportExcel.Name = "butExportExcel";
			this.butExportExcel.Size = new System.Drawing.Size(79, 26);
			this.butExportExcel.TabIndex = 7;
			this.butExportExcel.Text = "Excel";
			this.butExportExcel.Visible = false;
			this.butExportExcel.Click += new System.EventHandler(this.butExportExcel_Click);
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
			this.textTitle.Location = new System.Drawing.Point(63, 3);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(219, 20);
			this.textTitle.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 5);
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
			this.groupBox1.Controls.Add(this.radioHuman);
			this.groupBox1.Controls.Add(this.radioRaw);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(162, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(122, 58);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Format";
			// 
			// radioHuman
			// 
			this.radioHuman.Checked = true;
			this.radioHuman.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioHuman.Location = new System.Drawing.Point(10, 16);
			this.radioHuman.Name = "radioHuman";
			this.radioHuman.Size = new System.Drawing.Size(108, 16);
			this.radioHuman.TabIndex = 0;
			this.radioHuman.TabStop = true;
			this.radioHuman.Text = "Human-readable";
			this.radioHuman.Click += new System.EventHandler(this.radioHuman_Click);
			// 
			// radioRaw
			// 
			this.radioRaw.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioRaw.Location = new System.Drawing.Point(10, 34);
			this.radioRaw.Name = "radioRaw";
			this.radioRaw.Size = new System.Drawing.Size(104, 16);
			this.radioRaw.TabIndex = 1;
			this.radioRaw.Text = "Raw";
			this.radioRaw.Click += new System.EventHandler(this.radioRaw_Click);
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
			// printPreviewControl2
			// 
			this.printPreviewControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.printPreviewControl2.AutoZoom = false;
			this.printPreviewControl2.Location = new System.Drawing.Point(6, 136);
			this.printPreviewControl2.Name = "printPreviewControl2";
			this.printPreviewControl2.Size = new System.Drawing.Size(313, 636);
			this.printPreviewControl2.TabIndex = 5;
			this.printPreviewControl2.Visible = false;
			this.printPreviewControl2.Zoom = 1D;
			// 
			// panelZoom
			// 
			this.panelZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panelZoom.Controls.Add(this.labelTotPages);
			this.panelZoom.Controls.Add(this.butBack);
			this.panelZoom.Controls.Add(this.butFwd);
			this.panelZoom.Location = new System.Drawing.Point(644, 666);
			this.panelZoom.Name = "panelZoom";
			this.panelZoom.Size = new System.Drawing.Size(142, 23);
			this.panelZoom.TabIndex = 0;
			this.panelZoom.Visible = false;
			// 
			// labelTotPages
			// 
			this.labelTotPages.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTotPages.Location = new System.Drawing.Point(29, 3);
			this.labelTotPages.Name = "labelTotPages";
			this.labelTotPages.Size = new System.Drawing.Size(87, 18);
			this.labelTotPages.TabIndex = 11;
			this.labelTotPages.Text = "25 / 26";
			this.labelTotPages.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butBack
			// 
			this.butBack.Image = global::OpenDental.Properties.Resources.Left;
			this.butBack.Location = new System.Drawing.Point(6, 0);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(18, 23);
			this.butBack.TabIndex = 17;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// butFwd
			// 
			this.butFwd.AdjustImageLocation = new System.Drawing.Point(1, 0);
			this.butFwd.Image = global::OpenDental.Properties.Resources.Right;
			this.butFwd.Location = new System.Drawing.Point(118, 0);
			this.butFwd.Name = "butFwd";
			this.butFwd.Size = new System.Drawing.Size(18, 23);
			this.butFwd.TabIndex = 18;
			this.butFwd.Click += new System.EventHandler(this.butFwd_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(1075, 666);
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
			this.butExport.Location = new System.Drawing.Point(990, 666);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 14;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butQView
			// 
			this.butQView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butQView.Image = global::OpenDental.Properties.Resources.butQView;
			this.butQView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butQView.Location = new System.Drawing.Point(792, 666);
			this.butQView.Name = "butQView";
			this.butQView.Size = new System.Drawing.Size(92, 24);
			this.butQView.TabIndex = 15;
			this.butQView.Text = "&Query View";
			this.butQView.Click += new System.EventHandler(this.butQView_Click);
			// 
			// butPrintPreview
			// 
			this.butPrintPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPrintPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintPreview.Location = new System.Drawing.Point(890, 666);
			this.butPrintPreview.Name = "butPrintPreview";
			this.butPrintPreview.Size = new System.Drawing.Size(94, 24);
			this.butPrintPreview.TabIndex = 16;
			this.butPrintPreview.Text = "P&rint Preview";
			this.butPrintPreview.Click += new System.EventHandler(this.butPrintPreview_Click);
			// 
			// splitContainerQuery
			// 
			this.splitContainerQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainerQuery.Location = new System.Drawing.Point(0, 0);
			this.splitContainerQuery.Name = "splitContainerQuery";
			this.splitContainerQuery.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerQuery.Panel1
			// 
			this.splitContainerQuery.Panel1.Controls.Add(this.panel1);
			this.splitContainerQuery.Panel1.Controls.Add(this.textQuery);
			this.splitContainerQuery.Panel1MinSize = 105;
			// 
			// splitContainerQuery.Panel2
			// 
			this.splitContainerQuery.Panel2.Controls.Add(this.grid2);
			this.splitContainerQuery.Panel2.Controls.Add(this.label1);
			this.splitContainerQuery.Panel2.Controls.Add(this.textTitle);
			this.splitContainerQuery.Panel2MinSize = 200;
			this.splitContainerQuery.Size = new System.Drawing.Size(1230, 660);
			this.splitContainerQuery.SplitterDistance = 126;
			this.splitContainerQuery.TabIndex = 17;
			this.splitContainerQuery.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerQuery_SplitterMoved);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.checkReportServer);
			this.panel1.Controls.Add(this.butFavorite);
			this.panel1.Controls.Add(this.butExportExcel);
			this.panel1.Controls.Add(this.butAdd);
			this.panel1.Controls.Add(this.butPaste);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.butSubmit);
			this.panel1.Controls.Add(this.butCopy);
			this.panel1.Location = new System.Drawing.Point(560, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(400, 143);
			this.panel1.TabIndex = 17;
			// 
			// checkReportServer
			// 
			this.checkReportServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportServer.Location = new System.Drawing.Point(5, 106);
			this.checkReportServer.Name = "checkReportServer";
			this.checkReportServer.Size = new System.Drawing.Size(243, 23);
			this.checkReportServer.TabIndex = 12;
			this.checkReportServer.Text = "Run on report server";
			this.checkReportServer.UseVisualStyleBackColor = true;
			this.checkReportServer.Visible = false;
			// 
			// FormQuery
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.butQView);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.splitContainerQuery);
			this.Controls.Add(this.butPrintPreview);
			this.Controls.Add(this.printPreviewControl2);
			this.Controls.Add(this.panelZoom);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butExport);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQuery";
			this.Text = "User Query";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormQuery_FormClosing);
			this.Load += new System.EventHandler(this.FormQuery_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormQuery_Layout);
			((System.ComponentModel.ISupportInitialize)(this.grid2)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.panelZoom.ResumeLayout(false);
			this.splitContainerQuery.Panel1.ResumeLayout(false);
			this.splitContainerQuery.Panel2.ResumeLayout(false);
			this.splitContainerQuery.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerQuery)).EndInit();
			this.splitContainerQuery.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}		
	}
}
