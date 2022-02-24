using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimsSend {
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

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimsSend));
			this.label6 = new System.Windows.Forms.Label();
			this.contextMenuStatus = new System.Windows.Forms.ContextMenu();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panelSplitter = new System.Windows.Forms.Panel();
			this.panelHistory = new System.Windows.Forms.Panel();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.comboHistoryType = new OpenDental.UI.ComboBoxOD();
			this.gridHistory = new OpenDental.UI.GridOD();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ToolBarHistory = new OpenDental.UI.ToolBarOD();
			this.comboClinic = new OpenDental.UI.ComboBoxOD();
			this.labelClinic = new System.Windows.Forms.Label();
			this.contextMenuEclaims = new System.Windows.Forms.ContextMenu();
			this.comboCustomTracking = new System.Windows.Forms.ComboBox();
			this.labelCustomTracking = new System.Windows.Forms.Label();
			this.butNextUnsent = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.contextMenuHistory = new System.Windows.Forms.ContextMenu();
			this.menuItemGoToAccount = new System.Windows.Forms.MenuItem();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.textProc = new System.Windows.Forms.TextBox();
			this.textProv = new System.Windows.Forms.TextBox();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.labelProc = new System.Windows.Forms.Label();
			this.labelProv = new System.Windows.Forms.Label();
			this.panelHistory.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(107, -44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 44);
			this.label6.TabIndex = 21;
			this.label6.Text = "Insurance Claims";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "");
			this.imageList1.Images.SetKeyName(6, "");
			// 
			// panelSplitter
			// 
			this.panelSplitter.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.panelSplitter.Location = new System.Drawing.Point(2, 398);
			this.panelSplitter.Name = "panelSplitter";
			this.panelSplitter.Size = new System.Drawing.Size(961, 6);
			this.panelSplitter.TabIndex = 50;
			this.panelSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseDown);
			this.panelSplitter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseMove);
			this.panelSplitter.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseUp);
			// 
			// panelHistory
			// 
			this.panelHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelHistory.Controls.Add(this.dateRangePicker);
			this.panelHistory.Controls.Add(this.label4);
			this.panelHistory.Controls.Add(this.comboHistoryType);
			this.panelHistory.Controls.Add(this.gridHistory);
			this.panelHistory.Controls.Add(this.panel1);
			this.panelHistory.Location = new System.Drawing.Point(0, 403);
			this.panelHistory.Name = "panelHistory";
			this.panelHistory.Size = new System.Drawing.Size(1231, 286);
			this.panelHistory.TabIndex = 51;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.Location = new System.Drawing.Point(3, 6);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 48;
			this.dateRangePicker.CalendarSelectionChanged += new OpenDental.UI.CalendarSelectionHandler(this.dateRangePicker_CalendarSelection);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(707, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 18);
			this.label4.TabIndex = 47;
			this.label4.Text = "Type";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// comboHistoryType
			// 
			this.comboHistoryType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboHistoryType.BackColor = System.Drawing.SystemColors.Window;
			this.comboHistoryType.Location = new System.Drawing.Point(753, 6);
			this.comboHistoryType.Name = "comboHistoryType";
			this.comboHistoryType.SelectionModeMulti = true;
			this.comboHistoryType.Size = new System.Drawing.Size(100, 21);
			this.comboHistoryType.TabIndex = 45;
			this.comboHistoryType.SelectionChangeCommitted += new System.EventHandler(this.comboHistoryType_SelectionChangeCommitted);
			// 
			// gridHistory
			// 
			this.gridHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridHistory.Location = new System.Drawing.Point(4, 31);
			this.gridHistory.Name = "gridHistory";
			this.gridHistory.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridHistory.Size = new System.Drawing.Size(1218, 252);
			this.gridHistory.TabIndex = 33;
			this.gridHistory.Title = "History";
			this.gridHistory.TranslationName = "TableClaimHistory";
			this.gridHistory.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridHistory_CellDoubleClick);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Controls.Add(this.ToolBarHistory);
			this.panel1.Location = new System.Drawing.Point(867, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(354, 27);
			this.panel1.TabIndex = 44;
			// 
			// ToolBarHistory
			// 
			this.ToolBarHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ToolBarHistory.BackColor = System.Drawing.SystemColors.Control;
			this.ToolBarHistory.ImageList = this.imageList1;
			this.ToolBarHistory.Location = new System.Drawing.Point(1, 1);
			this.ToolBarHistory.Name = "ToolBarHistory";
			this.ToolBarHistory.Size = new System.Drawing.Size(353, 25);
			this.ToolBarHistory.TabIndex = 43;
			this.ToolBarHistory.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarHistory_ButtonClick);
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(74, 26);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(160, 21);
			this.comboClinic.TabIndex = 53;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(7, 29);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(65, 14);
			this.labelClinic.TabIndex = 52;
			this.labelClinic.Text = "Clinic Filter";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// comboCustomTracking
			// 
			this.comboCustomTracking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCustomTracking.Location = new System.Drawing.Point(514, 26);
			this.comboCustomTracking.MaxDropDownItems = 40;
			this.comboCustomTracking.Name = "comboCustomTracking";
			this.comboCustomTracking.Size = new System.Drawing.Size(160, 21);
			this.comboCustomTracking.TabIndex = 55;
			this.comboCustomTracking.SelectionChangeCommitted += new System.EventHandler(this.comboCustomTracking_SelectionChangeCommitted);
			// 
			// labelCustomTracking
			// 
			this.labelCustomTracking.Location = new System.Drawing.Point(370, 29);
			this.labelCustomTracking.Name = "labelCustomTracking";
			this.labelCustomTracking.Size = new System.Drawing.Size(142, 14);
			this.labelCustomTracking.TabIndex = 54;
			this.labelCustomTracking.Text = "Custom Tracking Filter";
			this.labelCustomTracking.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butNextUnsent
			// 
			this.butNextUnsent.Location = new System.Drawing.Point(234, 25);
			this.butNextUnsent.Name = "butNextUnsent";
			this.butNextUnsent.Size = new System.Drawing.Size(74, 23);
			this.butNextUnsent.TabIndex = 56;
			this.butNextUnsent.Text = "Next Unsent";
			this.butNextUnsent.UseVisualStyleBackColor = true;
			this.butNextUnsent.Click += new System.EventHandler(this.butNextUnsent_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(4, 49);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1218, 350);
			this.gridMain.TabIndex = 32;
			this.gridMain.Title = "Claims Waiting to Send";
			this.gridMain.TranslationName = "TableQueue";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.LocationChanged += new System.EventHandler(this.gridMain_LocationChanged);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageList1;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1230, 25);
			this.ToolBarMain.TabIndex = 31;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// contextMenuHistory
			// 
			this.contextMenuHistory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGoToAccount});
			// 
			// menuItemGoToAccount
			// 
			this.menuItemGoToAccount.Index = 0;
			this.menuItemGoToAccount.Text = "Go To Account";
			this.menuItemGoToAccount.Click += new System.EventHandler(this.menuItemHistoryGoToAccount_Click);
			// 
			// textCarrier
			// 
			this.textCarrier.AcceptsTab = true;
			this.textCarrier.BackColor = System.Drawing.SystemColors.Window;
			this.textCarrier.Location = new System.Drawing.Point(775, 26);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(100, 20);
			this.textCarrier.TabIndex = 57;
			// 
			// textProc
			// 
			this.textProc.AcceptsTab = true;
			this.textProc.BackColor = System.Drawing.SystemColors.Window;
			this.textProc.Location = new System.Drawing.Point(1122, 26);
			this.textProc.Name = "textProc";
			this.textProc.Size = new System.Drawing.Size(100, 20);
			this.textProc.TabIndex = 59;
			// 
			// textProv
			// 
			this.textProv.AcceptsTab = true;
			this.textProv.BackColor = System.Drawing.SystemColors.Window;
			this.textProv.Location = new System.Drawing.Point(943, 26);
			this.textProv.Name = "textProv";
			this.textProv.Size = new System.Drawing.Size(100, 20);
			this.textProv.TabIndex = 58;
			// 
			// labelCarrier
			// 
			this.labelCarrier.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelCarrier.Location = new System.Drawing.Point(709, 26);
			this.labelCarrier.Name = "labelCarrier";
			this.labelCarrier.Size = new System.Drawing.Size(65, 20);
			this.labelCarrier.TabIndex = 60;
			this.labelCarrier.Text = "Carrier";
			this.labelCarrier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelProc
			// 
			this.labelProc.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelProc.Location = new System.Drawing.Point(1045, 26);
			this.labelProc.Name = "labelProc";
			this.labelProc.Size = new System.Drawing.Size(76, 20);
			this.labelProc.TabIndex = 61;
			this.labelProc.Text = "Procedure";
			this.labelProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelProv
			// 
			this.labelProv.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelProv.Location = new System.Drawing.Point(877, 26);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(65, 20);
			this.labelProv.TabIndex = 62;
			this.labelProv.Text = "Provider";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormClaimsSend
			// 
			this.ClientSize = new System.Drawing.Size(1230, 691);
			this.Controls.Add(this.labelProv);
			this.Controls.Add(this.labelProc);
			this.Controls.Add(this.labelCarrier);
			this.Controls.Add(this.textProv);
			this.Controls.Add(this.textProc);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.butNextUnsent);
			this.Controls.Add(this.comboCustomTracking);
			this.Controls.Add(this.labelCustomTracking);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.panelHistory);
			this.Controls.Add(this.panelSplitter);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.label6);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimsSend";
			this.Text = "Insurance Claims";
			this.Load += new System.EventHandler(this.FormClaimsSend_Load);
			this.panelHistory.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenu contextMenuStatus;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.GridOD gridHistory;
		private OpenDental.UI.ToolBarOD ToolBarHistory;
		private Panel panelSplitter;
		private Panel panelHistory;
		private Panel panel1;
		private OpenDental.UI.ComboBoxOD comboClinic;
		private Label labelClinic;
		private ComboBox comboCustomTracking;
		private Label labelCustomTracking;
		private OpenDental.UI.ComboBoxOD comboHistoryType;
		private Label label4;
		private ContextMenu contextMenuEclaims;
		private UI.Button butNextUnsent;
		private ContextMenu contextMenuHistory;
		private MenuItem menuItemGoToAccount;
		private OpenDental.UI.ODDateRangePicker dateRangePicker;
		private TextBox textCarrier;
		private TextBox textProc;
		private TextBox textProv;
		private Label labelCarrier;
		private Label labelProc;
		private Label labelProv;
	}
}
