namespace OpenDental{
	partial class FormSetupWizard {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetupWizard));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAll = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "iButton_Blue.png");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(391, 52);
			this.label1.TabIndex = 5;
			this.label1.Text = "Double click a row to set it up individually. \r\nDouble click a category to set up" +
    " all for that category.\r\nClick \"Set Up All\" to go through the entire setup seque" +
    "nce.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasDropDowns = true;
			this.gridMain.Location = new System.Drawing.Point(12, 60);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(388, 289);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Setup";
			this.gridMain.TranslationName = "TableSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAll.Location = new System.Drawing.Point(12, 364);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 24);
			this.butAll.TabIndex = 6;
			this.butAll.Text = "Set Up All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(325, 364);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormSetupWizard
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(411, 400);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSetupWizard";
			this.Text = "Setup Wizard";
			this.Load += new System.EventHandler(this.FormSetupWizard_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private UI.Button butAll;
	}
}