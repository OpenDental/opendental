namespace OpenDental{
	partial class FormClaimPayList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimPayList));
			this.butPickPaymentGroup = new OpenDental.UI.Button();
			this.comboPayGroup = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textDateTo = new OpenDental.ValidDate();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.textDateFrom = new OpenDental.ValidDate();
			this.labelToDate = new System.Windows.Forms.Label();
			this.labelFromDate = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butPickPaymentGroup
			// 
			this.butPickPaymentGroup.Location = new System.Drawing.Point(551, 32);
			this.butPickPaymentGroup.Name = "butPickPaymentGroup";
			this.butPickPaymentGroup.Size = new System.Drawing.Size(23, 21);
			this.butPickPaymentGroup.TabIndex = 47;
			this.butPickPaymentGroup.Text = "...";
			this.butPickPaymentGroup.Click += new System.EventHandler(this.butPickPaymentGroup_Click);
			// 
			// comboPayGroup
			// 
			this.comboPayGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayGroup.Location = new System.Drawing.Point(367, 32);
			this.comboPayGroup.MaxDropDownItems = 40;
			this.comboPayGroup.Name = "comboPayGroup";
			this.comboPayGroup.Size = new System.Drawing.Size(181, 21);
			this.comboPayGroup.TabIndex = 26;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(236, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(129, 14);
			this.label1.TabIndex = 25;
			this.label1.Text = "Payment Group";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(779, 29);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(84, 24);
			this.butRefresh.TabIndex = 24;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(331, 6);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(217, 21);
			this.comboClinic.TabIndex = 23;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 60);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(851, 508);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Insurance Payments (EOBs)";
			this.gridMain.TranslationName = "FormClaimPayList";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(121, 33);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(94, 20);
			this.textDateTo.TabIndex = 14;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 576);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 2;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(788, 576);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(121, 10);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(94, 20);
			this.textDateFrom.TabIndex = 13;
			// 
			// labelToDate
			// 
			this.labelToDate.Location = new System.Drawing.Point(30, 35);
			this.labelToDate.Name = "labelToDate";
			this.labelToDate.Size = new System.Drawing.Size(88, 14);
			this.labelToDate.TabIndex = 12;
			this.labelToDate.Text = "To Date";
			this.labelToDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFromDate
			// 
			this.labelFromDate.Location = new System.Drawing.Point(30, 13);
			this.labelFromDate.Name = "labelFromDate";
			this.labelFromDate.Size = new System.Drawing.Size(88, 14);
			this.labelFromDate.TabIndex = 11;
			this.labelFromDate.Text = "From Date";
			this.labelFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormClaimPayList
			// 
			this.ClientSize = new System.Drawing.Size(888, 612);
			this.Controls.Add(this.butPickPaymentGroup);
			this.Controls.Add(this.comboPayGroup);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.labelToDate);
			this.Controls.Add(this.labelFromDate);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimPayList";
			this.Text = "Batch Insurance Payments";
			this.Load += new System.EventHandler(this.FormClaimPayList_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelFromDate;
		private System.Windows.Forms.Label labelToDate;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private UI.Button butRefresh;
		private UI.Button butAdd;
		private System.Windows.Forms.ComboBox comboPayGroup;
		private System.Windows.Forms.Label label1;
		private UI.Button butPickPaymentGroup;
	}
}