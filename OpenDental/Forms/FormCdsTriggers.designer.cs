namespace OpenDental{
	partial class FormCdsTriggers {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCdsTriggers));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAddTrigger = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 33);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(683, 560);
			this.gridMain.TabIndex = 199;
			this.gridMain.Title = "CDS Triggers";
			this.gridMain.TranslationName = "TableCDSTriggers";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAddTrigger
			// 
			this.butAddTrigger.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddTrigger.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTrigger.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTrigger.Location = new System.Drawing.Point(701, 33);
			this.butAddTrigger.Name = "butAddTrigger";
			this.butAddTrigger.Size = new System.Drawing.Size(75, 23);
			this.butAddTrigger.TabIndex = 201;
			this.butAddTrigger.Text = "Add";
			this.butAddTrigger.Click += new System.EventHandler(this.butAddTrigger_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(701, 569);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(788, 24);
			this.menuMain.TabIndex = 202;
			// 
			// FormCdsTriggers
			// 
			this.ClientSize = new System.Drawing.Size(788, 605);
			this.Controls.Add(this.butAddTrigger);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCdsTriggers";
			this.Text = "CDS Triggers";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEhrTriggers_FormClosing);
			this.Load += new System.EventHandler(this.FormEhrTriggers_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private UI.Button butAddTrigger;
		private UI.MenuOD menuMain;
	}
}