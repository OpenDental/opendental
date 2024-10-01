namespace OpenDental{
	partial class FormInvoiceItemSelect {
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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInvoiceItemSelect));
			this.label1 = new System.Windows.Forms.Label();
			this._gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butNone = new OpenDental.UI.Button();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.checkIsFilteringZeroAmount = new OpenDental.UI.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(587, 26);
			this.label1.TabIndex = 3;
			this.label1.Text = "The list below contains adjustments, completed procedures, and pay plan charges t" +
    "hat have not been attached to an invoice.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _gridMain
			// 
			this._gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridMain.Location = new System.Drawing.Point(12, 34);
			this._gridMain.Name = "_gridMain";
			this._gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this._gridMain.Size = new System.Drawing.Size(587, 518);
			this._gridMain.TabIndex = 140;
			this._gridMain.Title = "Invoice Items";
			this._gridMain.TranslationName = "TableInvoiceItems";
			this._gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(620, 526);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAll.Location = new System.Drawing.Point(605, 122);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 26);
			this.butAll.TabIndex = 141;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNone.Location = new System.Drawing.Point(605, 154);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 26);
			this.butNone.TabIndex = 142;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkIsFilteringZeroAmount);
			this.groupBox1.Location = new System.Drawing.Point(605, 34);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(108, 82);
			this.groupBox1.TabIndex = 143;
			this.groupBox1.Text = "Filter";
			// 
			// checkIsFilteringZeroAmount
			// 
			this.checkIsFilteringZeroAmount.Checked = true;
			this.checkIsFilteringZeroAmount.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIsFilteringZeroAmount.Location = new System.Drawing.Point(3, 25);
			this.checkIsFilteringZeroAmount.Name = "checkIsFilteringZeroAmount";
			this.checkIsFilteringZeroAmount.Size = new System.Drawing.Size(102, 35);
			this.checkIsFilteringZeroAmount.TabIndex = 0;
			this.checkIsFilteringZeroAmount.Text = "Exclude $0 Fee";
			this.checkIsFilteringZeroAmount.Click += new System.EventHandler(this.checkIsFilteringZeroAmount_Click);
			// 
			// FormInvoiceItemSelect
			// 
			this.ClientSize = new System.Drawing.Size(715, 564);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this._gridMain);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInvoiceItemSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Invoice Items";
			this.Load += new System.EventHandler(this.FormInvoiceItemSelect_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private UI.Button butAll;
		private UI.Button butNone;
		private UI.GroupBox groupBox1;
		private UI.CheckBox checkIsFilteringZeroAmount;
	}
}