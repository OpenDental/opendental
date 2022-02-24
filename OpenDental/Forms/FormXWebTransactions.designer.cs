namespace OpenDental{
	partial class FormXWebTransactions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXWebTransactions));
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.openPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.voidPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.processReturnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.butClose = new OpenDental.UI.Button();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo,
            this.openPaymentToolStripMenuItem,
            this.voidPaymentToolStripMenuItem,
            this.processReturnToolStripMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(154, 92);
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(153, 22);
			this.menuItemGoTo.Text = "Go To Account";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// openPaymentToolStripMenuItem
			// 
			this.openPaymentToolStripMenuItem.Name = "openPaymentToolStripMenuItem";
			this.openPaymentToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.openPaymentToolStripMenuItem.Text = "Open Payment";
			this.openPaymentToolStripMenuItem.Click += new System.EventHandler(this.openPaymentToolStripMenuItem_Click);
			// 
			// voidPaymentToolStripMenuItem
			// 
			this.voidPaymentToolStripMenuItem.Name = "voidPaymentToolStripMenuItem";
			this.voidPaymentToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.voidPaymentToolStripMenuItem.Text = "Void Payment";
			this.voidPaymentToolStripMenuItem.Click += new System.EventHandler(this.voidPaymentToolStripMenuItem_Click);
			// 
			// processReturnToolStripMenuItem
			// 
			this.processReturnToolStripMenuItem.Name = "processReturnToolStripMenuItem";
			this.processReturnToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.processReturnToolStripMenuItem.Text = "Process Return";
			this.processReturnToolStripMenuItem.Click += new System.EventHandler(this.processReturnToolStripMenuItem_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 14);
			this.label2.TabIndex = 141;
			this.label2.Text = "From Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(184, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 142;
			this.label3.Text = "To Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenu;
			this.gridMain.Location = new System.Drawing.Point(12, 40);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(789, 454);
			this.gridMain.TabIndex = 140;
			this.gridMain.Title = "Patient Portal Transactions";
			this.gridMain.TranslationName = "TableXWebTrans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseDown);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(719, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 145;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(85, 12);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(90, 20);
			this.textDateFrom.TabIndex = 143;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(254, 12);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(90, 20);
			this.textDateTo.TabIndex = 144;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(726, 521);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(543, 11);
			this.comboClinic.MaxDropDownItems = 40;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(159, 21);
			this.comboClinic.TabIndex = 147;
			// 
			// labelClinic
			// 
			this.labelClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClinic.Location = new System.Drawing.Point(476, 14);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(65, 14);
			this.labelClinic.TabIndex = 146;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// FormXWebTransactions
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(827, 557);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormXWebTransactions";
			this.Text = "Patient Portal Transactions";
			this.Load += new System.EventHandler(this.FormXWebTransactions_Load);
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.Button butRefresh;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private System.Windows.Forms.ToolStripMenuItem openPaymentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem voidPaymentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem processReturnToolStripMenuItem;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
	}
}