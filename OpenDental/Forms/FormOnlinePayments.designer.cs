namespace OpenDental {
	partial class FormOnlinePayments {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOnlinePayments));
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.comboClinic = new OpenDental.UI.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.comboPaymentSource = new OpenDental.UI.ComboBox();
			this.labelPaymentSource = new System.Windows.Forms.Label();
			this.comboProcessStatus = new OpenDental.UI.ComboBox();
			this.labelProcessStatus = new System.Windows.Forms.Label();
			this.dateStart = new System.Windows.Forms.DateTimePicker();
			this.dateEnd = new System.Windows.Forms.DateTimePicker();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(181, 48);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(180, 22);
			this.menuItemGoTo.Text = "Go To Account";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(410, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(159, 21);
			this.comboClinic.TabIndex = 55;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(309, 14);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(100, 16);
			this.labelClinic.TabIndex = 54;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenu;
			this.gridMain.Location = new System.Drawing.Point(12, 93);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(657, 446);
			this.gridMain.TabIndex = 30;
			this.gridMain.Title = "Payment Information";
			this.gridMain.TranslationName = "TablePendingPayments";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseDown);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(593, 9);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 56;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(593, 552);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboPaymentSource
			// 
			this.comboPaymentSource.Location = new System.Drawing.Point(410, 66);
			this.comboPaymentSource.Name = "comboPaymentSource";
			this.comboPaymentSource.SelectionModeMulti = true;
			this.comboPaymentSource.Size = new System.Drawing.Size(159, 21);
			this.comboPaymentSource.TabIndex = 58;
			// 
			// labelPaymentSource
			// 
			this.labelPaymentSource.Location = new System.Drawing.Point(309, 69);
			this.labelPaymentSource.Name = "labelPaymentSource";
			this.labelPaymentSource.Size = new System.Drawing.Size(100, 16);
			this.labelPaymentSource.TabIndex = 57;
			this.labelPaymentSource.Text = "Payment Source";
			this.labelPaymentSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProcessStatus
			// 
			this.comboProcessStatus.Location = new System.Drawing.Point(410, 39);
			this.comboProcessStatus.Name = "comboProcessStatus";
			this.comboProcessStatus.SelectionModeMulti = true;
			this.comboProcessStatus.Size = new System.Drawing.Size(159, 21);
			this.comboProcessStatus.TabIndex = 60;
			// 
			// labelProcessStatus
			// 
			this.labelProcessStatus.Location = new System.Drawing.Point(309, 41);
			this.labelProcessStatus.Name = "labelProcessStatus";
			this.labelProcessStatus.Size = new System.Drawing.Size(100, 16);
			this.labelProcessStatus.TabIndex = 59;
			this.labelProcessStatus.Text = "Process Status";
			this.labelProcessStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateStart
			// 
			this.dateStart.Location = new System.Drawing.Point(79, 12);
			this.dateStart.Name = "dateStart";
			this.dateStart.Size = new System.Drawing.Size(200, 20);
			this.dateStart.TabIndex = 61;
			// 
			// dateEnd
			// 
			this.dateEnd.Location = new System.Drawing.Point(79, 39);
			this.dateEnd.Name = "dateEnd";
			this.dateEnd.Size = new System.Drawing.Size(200, 20);
			this.dateEnd.TabIndex = 62;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 41);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 16);
			this.label10.TabIndex = 90;
			this.label10.Text = "To";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 14);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 16);
			this.label9.TabIndex = 91;
			this.label9.Text = "From";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormOnlinePayments
			// 
			this.ClientSize = new System.Drawing.Size(682, 588);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.dateEnd);
			this.Controls.Add(this.dateStart);
			this.Controls.Add(this.comboProcessStatus);
			this.Controls.Add(this.labelProcessStatus);
			this.Controls.Add(this.comboPaymentSource);
			this.Controls.Add(this.labelPaymentSource);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOnlinePayments";
			this.Text = "Online Payments";
			this.Load += new System.EventHandler(this.FormPendingOnlinePayments_Load);
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private OpenDental.UI.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private UI.Button butRefresh;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private UI.ComboBox comboPaymentSource;
		private System.Windows.Forms.Label labelPaymentSource;
		private UI.ComboBox comboProcessStatus;
		private System.Windows.Forms.Label labelProcessStatus;
		private System.Windows.Forms.DateTimePicker dateStart;
		private System.Windows.Forms.DateTimePicker dateEnd;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
	}
}