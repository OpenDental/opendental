namespace OpenDental{
	partial class FormCreditCardManage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreditCardManage));
			this.butClose = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.labelStoreCCNumWarning = new System.Windows.Forms.Label();
			this.butMoveTo = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAddRecCharge = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(910, 312);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(82, 26);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(910, 148);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 26);
			this.butDown.TabIndex = 37;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(910, 119);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 26);
			this.butUp.TabIndex = 38;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(872, 12);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(120, 26);
			this.butAdd.TabIndex = 36;
			this.butAdd.Text = "&Add New Card";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// labelStoreCCNumWarning
			// 
			this.labelStoreCCNumWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelStoreCCNumWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelStoreCCNumWarning.Location = new System.Drawing.Point(12, 344);
			this.labelStoreCCNumWarning.Name = "labelStoreCCNumWarning";
			this.labelStoreCCNumWarning.Size = new System.Drawing.Size(280, 30);
			this.labelStoreCCNumWarning.TabIndex = 39;
			this.labelStoreCCNumWarning.Text = "You should turn off the option in Module Setup for \"allow storing credit card num" +
    "bers\" in order to start using tokens.";
			this.labelStoreCCNumWarning.Visible = false;
			// 
			// butMoveTo
			// 
			this.butMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMoveTo.Location = new System.Drawing.Point(872, 76);
			this.butMoveTo.Name = "butMoveTo";
			this.butMoveTo.Size = new System.Drawing.Size(120, 26);
			this.butMoveTo.TabIndex = 40;
			this.butMoveTo.Text = "Move To Patient";
			this.butMoveTo.Click += new System.EventHandler(this.butMoveTo_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(848, 328);
			this.gridMain.TabIndex = 41;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableManage";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAddRecCharge
			// 
			this.butAddRecCharge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddRecCharge.Location = new System.Drawing.Point(872, 44);
			this.butAddRecCharge.Name = "butAddRecCharge";
			this.butAddRecCharge.Size = new System.Drawing.Size(120, 26);
			this.butAddRecCharge.TabIndex = 42;
			this.butAddRecCharge.Text = "&Reuse Existing Card";
			this.butAddRecCharge.Click += new System.EventHandler(this.butReuseCard_Click);
			// 
			// FormCreditCardManage
			// 
			this.ClientSize = new System.Drawing.Size(1004, 383);
			this.Controls.Add(this.butAddRecCharge);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butMoveTo);
			this.Controls.Add(this.labelStoreCCNumWarning);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCreditCardManage";
			this.Text = "Credit Card Manage";
			this.Load += new System.EventHandler(this.FormCreditCardManage_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.Button butDown;
		private UI.Button butUp;
		private UI.Button butAdd;
		private System.Windows.Forms.Label labelStoreCCNumWarning;
		private UI.Button butMoveTo;
		private UI.GridOD gridMain;
    private UI.Button butAddRecCharge;
    }
}