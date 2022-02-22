namespace OpenDental{
	partial class FormEhrProvKeysCustomer {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrProvKeysCustomer));
			this.label1 = new System.Windows.Forms.Label();
			this.textCharge = new System.Windows.Forms.TextBox();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridQ = new OpenDental.UI.GridOD();
			this.butAddQuarterly = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(69, 464);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 37);
			this.label1.TabIndex = 195;
			this.label1.Text = "Monthly repeating charge (in addition to the normal repeating charge for office) " +
    "should be";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCharge
			// 
			this.textCharge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textCharge.Location = new System.Drawing.Point(311, 471);
			this.textCharge.Name = "textCharge";
			this.textCharge.ReadOnly = true;
			this.textCharge.Size = new System.Drawing.Size(62, 20);
			this.textCharge.TabIndex = 196;
			this.textCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(9, 12);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 194;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1010, 470);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSave.Location = new System.Drawing.Point(405, 468);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(94, 24);
			this.butSave.TabIndex = 197;
			this.butSave.Text = "Save To Images";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(505, 458);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 49);
			this.label2.TabIndex = 198;
			this.label2.Text = "To help archive any changes to this list";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(9, 48);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(553, 407);
			this.gridMain.TabIndex = 193;
			this.gridMain.Title = "Provider Keys";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// gridQ
			// 
			this.gridQ.AllowSortingByColumn = true;
			this.gridQ.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridQ.Location = new System.Drawing.Point(568, 48);
			this.gridQ.Name = "gridQ";
			this.gridQ.Size = new System.Drawing.Size(522, 407);
			this.gridQ.TabIndex = 199;
			this.gridQ.Title = "Quarterly Keys";
			this.gridQ.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQ_CellDoubleClick);
			// 
			// butAddQuarterly
			// 
			this.butAddQuarterly.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddQuarterly.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddQuarterly.Location = new System.Drawing.Point(568, 12);
			this.butAddQuarterly.Name = "butAddQuarterly";
			this.butAddQuarterly.Size = new System.Drawing.Size(75, 24);
			this.butAddQuarterly.TabIndex = 200;
			this.butAddQuarterly.Text = "Add";
			this.butAddQuarterly.Click += new System.EventHandler(this.butAddQuarterly_Click);
			// 
			// FormEhrProvKeysCustomer
			// 
			this.ClientSize = new System.Drawing.Size(1097, 506);
			this.Controls.Add(this.butAddQuarterly);
			this.Controls.Add(this.gridQ);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.textCharge);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrProvKeysCustomer";
			this.Text = "Provider Keys for This Family";
			this.Load += new System.EventHandler(this.FormEhrProvKeysCustomer_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCharge;
		private UI.Button butSave;
		private System.Windows.Forms.Label label2;
		private UI.GridOD gridQ;
		private UI.Button butAddQuarterly;
	}
}