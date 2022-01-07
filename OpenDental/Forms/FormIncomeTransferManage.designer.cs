namespace OpenDental{
	partial class FormIncomeTransferManage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIncomeTransferManage));
			this.gridImbalances = new OpenDental.UI.GridOD();
			this.gridTransfers = new OpenDental.UI.GridOD();
			this.butTransfer = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.checkShowBreakdown = new System.Windows.Forms.CheckBox();
			this.datePickerAsOf = new System.Windows.Forms.DateTimePicker();
			this.labelAsOfDate = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butFIFO = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridImbalances
			// 
			this.gridImbalances.AllowSortingByColumn = true;
			this.gridImbalances.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridImbalances.HasDropDowns = true;
			this.gridImbalances.Location = new System.Drawing.Point(426, 99);
			this.gridImbalances.Name = "gridImbalances";
			this.gridImbalances.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridImbalances.Size = new System.Drawing.Size(640, 465);
			this.gridImbalances.TabIndex = 13;
			this.gridImbalances.Title = "Provider/Family Balances";
			this.gridImbalances.TranslationName = "TableImbalances";
			// 
			// gridTransfers
			// 
			this.gridTransfers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridTransfers.Location = new System.Drawing.Point(12, 99);
			this.gridTransfers.Name = "gridTransfers";
			this.gridTransfers.Size = new System.Drawing.Size(408, 465);
			this.gridTransfers.TabIndex = 12;
			this.gridTransfers.Title = "Existing Transfers (editable)";
			this.gridTransfers.TranslationName = "TableTransfers";
			this.gridTransfers.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTransfers_CellDoubleClick);
			// 
			// butTransfer
			// 
			this.butTransfer.Image = global::OpenDental.Properties.Resources.Left;
			this.butTransfer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butTransfer.Location = new System.Drawing.Point(426, 71);
			this.butTransfer.Name = "butTransfer";
			this.butTransfer.Size = new System.Drawing.Size(94, 24);
			this.butTransfer.TabIndex = 18;
			this.butTransfer.Text = "Transfer";
			this.butTransfer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butTransfer.Click += new System.EventHandler(this.butTransfer_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1011, 575);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkShowBreakdown
			// 
			this.checkShowBreakdown.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowBreakdown.Location = new System.Drawing.Point(918, 12);
			this.checkShowBreakdown.Name = "checkShowBreakdown";
			this.checkShowBreakdown.Size = new System.Drawing.Size(148, 20);
			this.checkShowBreakdown.TabIndex = 157;
			this.checkShowBreakdown.Text = "Show Breakdown";
			this.checkShowBreakdown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowBreakdown.UseVisualStyleBackColor = true;
			this.checkShowBreakdown.CheckedChanged += new System.EventHandler(this.checkShowBreakdown_CheckedChanged);
			// 
			// datePickerAsOf
			// 
			this.datePickerAsOf.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerAsOf.Location = new System.Drawing.Point(508, 12);
			this.datePickerAsOf.Name = "datePickerAsOf";
			this.datePickerAsOf.Size = new System.Drawing.Size(112, 20);
			this.datePickerAsOf.TabIndex = 158;
			this.datePickerAsOf.ValueChanged += new System.EventHandler(this.datePickerTransfer_ValueChanged);
			// 
			// labelAsOfDate
			// 
			this.labelAsOfDate.Location = new System.Drawing.Point(424, 15);
			this.labelAsOfDate.Name = "labelAsOfDate";
			this.labelAsOfDate.Size = new System.Drawing.Size(78, 13);
			this.labelAsOfDate.TabIndex = 159;
			this.labelAsOfDate.Text = "As of Date";
			this.labelAsOfDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(395, 84);
			this.label1.TabIndex = 160;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// butFIFO
			// 
			this.butFIFO.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFIFO.Location = new System.Drawing.Point(3, 3);
			this.butFIFO.Name = "butFIFO";
			this.butFIFO.Size = new System.Drawing.Size(93, 24);
			this.butFIFO.TabIndex = 162;
			this.butFIFO.Text = "Transfer FIFO";
			this.butFIFO.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butFIFO.Click += new System.EventHandler(this.butFIFO_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(102, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(308, 43);
			this.label6.TabIndex = 163;
			this.label6.Text = "This is only used once to fix historical data. It allocates on a FIFO basis witho" +
    "ut regard for provider or patient.  For ongoing allocations by provider, use the" +
    " main Transfer button.";
			// 
			// label8
			// 
			this.label8.ForeColor = System.Drawing.Color.Black;
			this.label8.Location = new System.Drawing.Point(626, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(258, 19);
			this.label8.TabIndex = 164;
			this.label8.Text = "Will ignore procedures and payments after this date.";
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.butFIFO);
			this.groupBoxOD1.Controls.Add(this.label6);
			this.groupBoxOD1.Location = new System.Drawing.Point(653, 40);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(413, 51);
			this.groupBoxOD1.TabIndex = 165;
			this.groupBoxOD1.TabStop = false;
			// 
			// FormIncomeTransferManage
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1095, 606);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.datePickerAsOf);
			this.Controls.Add(this.labelAsOfDate);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.checkShowBreakdown);
			this.Controls.Add(this.butTransfer);
			this.Controls.Add(this.gridImbalances);
			this.Controls.Add(this.gridTransfers);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormIncomeTransferManage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Income Transfer Manager";
			this.Load += new System.EventHandler(this.FormIncomeTransferManage_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridTransfers;
		private UI.GridOD gridImbalances;
		private UI.Button butTransfer;
		private System.Windows.Forms.CheckBox checkShowBreakdown;
		private System.Windows.Forms.DateTimePicker datePickerAsOf;
		private System.Windows.Forms.Label labelAsOfDate;
		private System.Windows.Forms.Label label1;
		private UI.Button butFIFO;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private UI.GroupBoxOD groupBoxOD1;
	}
}