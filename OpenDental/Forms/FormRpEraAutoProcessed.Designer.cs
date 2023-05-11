namespace OpenDental{
	partial class FormRpEraAutoProcessed {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpEraAutoProcessed));
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.checkShowAcknowledged = new System.Windows.Forms.CheckBox();
			this.textCarrier = new OpenDental.ODtextBox();
			this.textCheckTrace = new OpenDental.ODtextBox();
			this.labelCheckTrace = new System.Windows.Forms.Label();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.gridEras = new OpenDental.UI.GridOD();
			this.gridClaims = new OpenDental.UI.GridOD();
			this.butAcknowledge = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.listAutoProcessStatuses = new OpenDental.UI.ListBoxOD();
			this.labelAutoProcessStatus = new System.Windows.Forms.Label();
			this.butSelectEob = new OpenDental.UI.Button();
			this.gridClaimProcedureAdjustments = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.dateRangePicker.Location = new System.Drawing.Point(12, 13);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.dateRangePicker.TabIndex = 0;
			// 
			// checkShowAcknowledged
			// 
			this.checkShowAcknowledged.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAcknowledged.Location = new System.Drawing.Point(901, 64);
			this.checkShowAcknowledged.Name = "checkShowAcknowledged";
			this.checkShowAcknowledged.Size = new System.Drawing.Size(162, 17);
			this.checkShowAcknowledged.TabIndex = 5;
			this.checkShowAcknowledged.Text = "Show Acknowledged";
			this.checkShowAcknowledged.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrier
			// 
			this.textCarrier.AcceptsTab = true;
			this.textCarrier.BackColor = System.Drawing.SystemColors.Window;
			this.textCarrier.DetectLinksEnabled = false;
			this.textCarrier.DetectUrls = false;
			this.textCarrier.Location = new System.Drawing.Point(599, 5);
			this.textCarrier.Multiline = false;
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
			this.textCarrier.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCarrier.Size = new System.Drawing.Size(161, 21);
			this.textCarrier.TabIndex = 1;
			this.textCarrier.Text = "";
			// 
			// textCheckTrace
			// 
			this.textCheckTrace.AcceptsTab = true;
			this.textCheckTrace.BackColor = System.Drawing.SystemColors.Window;
			this.textCheckTrace.DetectLinksEnabled = false;
			this.textCheckTrace.DetectUrls = false;
			this.textCheckTrace.Location = new System.Drawing.Point(599, 26);
			this.textCheckTrace.Multiline = false;
			this.textCheckTrace.Name = "textCheckTrace";
			this.textCheckTrace.QuickPasteType = OpenDentBusiness.QuickPasteType.FinancialNotes;
			this.textCheckTrace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCheckTrace.Size = new System.Drawing.Size(161, 21);
			this.textCheckTrace.TabIndex = 2;
			this.textCheckTrace.Text = "";
			// 
			// labelCheckTrace
			// 
			this.labelCheckTrace.Location = new System.Drawing.Point(469, 28);
			this.labelCheckTrace.Name = "labelCheckTrace";
			this.labelCheckTrace.Size = new System.Drawing.Size(130, 16);
			this.labelCheckTrace.TabIndex = 275;
			this.labelCheckTrace.Text = "Check# or EFT Trace#";
			this.labelCheckTrace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCarrier
			// 
			this.labelCarrier.Location = new System.Drawing.Point(544, 5);
			this.labelCarrier.Name = "labelCarrier";
			this.labelCarrier.Size = new System.Drawing.Size(55, 16);
			this.labelCarrier.TabIndex = 274;
			this.labelCarrier.Text = "Carrier";
			this.labelCarrier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(1137, 57);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 8;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1137, 666);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 12;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridEras
			// 
			this.gridEras.AllowSortingByColumn = true;
			this.gridEras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEras.Location = new System.Drawing.Point(12, 87);
			this.gridEras.Name = "gridEras";
			this.gridEras.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEras.Size = new System.Drawing.Size(1200, 149);
			this.gridEras.TabIndex = 9;
			this.gridEras.Title = "ERAs";
			this.gridEras.TranslationName = "TableEtrans835s";
			this.gridEras.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEras_CellDoubleClick);
			this.gridEras.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridEras_MouseUp);
			// 
			// gridClaims
			// 
			this.gridClaims.AllowSortingByColumn = true;
			this.gridClaims.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClaims.Location = new System.Drawing.Point(12, 242);
			this.gridClaims.Name = "gridClaims";
			this.gridClaims.Size = new System.Drawing.Size(1200, 288);
			this.gridClaims.TabIndex = 10;
			this.gridClaims.Title = "Claims";
			this.gridClaims.TranslationName = "TableEtrans835s";
			this.gridClaims.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClaims_CellDoubleClick);
			this.gridClaims.SelectionCommitted += new System.EventHandler(this.gridClaims_SelectionCommitted);
			// 
			// butAcknowledge
			// 
			this.butAcknowledge.Location = new System.Drawing.Point(12, 57);
			this.butAcknowledge.Name = "butAcknowledge";
			this.butAcknowledge.Size = new System.Drawing.Size(144, 24);
			this.butAcknowledge.TabIndex = 6;
			this.butAcknowledge.Text = "Acknowledge ERA(s)";
			this.butAcknowledge.UseVisualStyleBackColor = true;
			this.butAcknowledge.Click += new System.EventHandler(this.butAcknowledge_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(562, 47);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(198, 21);
			this.comboClinics.TabIndex = 3;
			// 
			// listAutoProcessStatuses
			// 
			this.listAutoProcessStatuses.Location = new System.Drawing.Point(910, 5);
			this.listAutoProcessStatuses.Name = "listAutoProcessStatuses";
			this.listAutoProcessStatuses.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAutoProcessStatuses.Size = new System.Drawing.Size(153, 56);
			this.listAutoProcessStatuses.TabIndex = 4;
			// 
			// labelAutoProcessStatus
			// 
			this.labelAutoProcessStatus.Location = new System.Drawing.Point(781, 5);
			this.labelAutoProcessStatus.Name = "labelAutoProcessStatus";
			this.labelAutoProcessStatus.Size = new System.Drawing.Size(128, 16);
			this.labelAutoProcessStatus.TabIndex = 283;
			this.labelAutoProcessStatus.Text = "Auto Processed Result";
			this.labelAutoProcessStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSelectEob
			// 
			this.butSelectEob.Location = new System.Drawing.Point(162, 57);
			this.butSelectEob.Name = "butSelectEob";
			this.butSelectEob.Size = new System.Drawing.Size(90, 24);
			this.butSelectEob.TabIndex = 7;
			this.butSelectEob.Text = "Select EOB";
			this.butSelectEob.UseVisualStyleBackColor = true;
			this.butSelectEob.Visible = false;
			this.butSelectEob.Click += new System.EventHandler(this.butSelectEob_Click);
			// 
			// gridClaimProcedureAdjustments
			// 
			this.gridClaimProcedureAdjustments.AllowSortingByColumn = true;
			this.gridClaimProcedureAdjustments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClaimProcedureAdjustments.Location = new System.Drawing.Point(12, 536);
			this.gridClaimProcedureAdjustments.Name = "gridClaimProcedureAdjustments";
			this.gridClaimProcedureAdjustments.Size = new System.Drawing.Size(1119, 154);
			this.gridClaimProcedureAdjustments.TabIndex = 11;
			this.gridClaimProcedureAdjustments.Title = "Claim Procedure Adjustments";
			this.gridClaimProcedureAdjustments.TranslationName = "TableEtrans835s";
			// 
			// FormRpEraAutoProcessed
			// 
			this.ClientSize = new System.Drawing.Size(1224, 696);
			this.Controls.Add(this.gridClaimProcedureAdjustments);
			this.Controls.Add(this.butSelectEob);
			this.Controls.Add(this.listAutoProcessStatuses);
			this.Controls.Add(this.labelAutoProcessStatus);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.butAcknowledge);
			this.Controls.Add(this.gridClaims);
			this.Controls.Add(this.gridEras);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.textCheckTrace);
			this.Controls.Add(this.labelCheckTrace);
			this.Controls.Add(this.labelCarrier);
			this.Controls.Add(this.checkShowAcknowledged);
			this.Controls.Add(this.dateRangePicker);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpEraAutoProcessed";
			this.Text = "ERAs Automatically Processed";
			this.Load += new System.EventHandler(this.FormRpEraAutoProcessed_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ODDateRangePicker dateRangePicker;
		private System.Windows.Forms.CheckBox checkShowAcknowledged;
		private ODtextBox textCarrier;
		private ODtextBox textCheckTrace;
		private System.Windows.Forms.Label labelCheckTrace;
		private System.Windows.Forms.Label labelCarrier;
		private UI.Button butRefresh;
		private UI.Button butClose;
		private UI.GridOD gridEras;
		private UI.GridOD gridClaims;
		private UI.Button butAcknowledge;
		private UI.ComboBoxClinicPicker comboClinics;
		private UI.ListBoxOD listAutoProcessStatuses;
		private System.Windows.Forms.Label labelAutoProcessStatus;
		private UI.Button butSelectEob;
		private UI.GridOD gridClaimProcedureAdjustments;
	}
}