namespace OpenDentalImaging{
	partial class FormImagingDevices {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImagingDevices));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textComputer = new System.Windows.Forms.TextBox();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1067, 527);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(32, 35);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1008, 516);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Imaging Devices";
			this.gridMain.TranslationName = "TableImagingDevices";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(1067, 35);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 5;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 18);
			this.label2.TabIndex = 10;
			this.label2.Text = "This Computer Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textComputer
			// 
			this.textComputer.Location = new System.Drawing.Point(167, 9);
			this.textComputer.Name = "textComputer";
			this.textComputer.ReadOnly = true;
			this.textComputer.Size = new System.Drawing.Size(182, 20);
			this.textComputer.TabIndex = 9;
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(1067, 188);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 26);
			this.butDown.TabIndex = 38;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(1067, 156);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 26);
			this.butUp.TabIndex = 39;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// FormImagingDevices
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1154, 563);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textComputer);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImagingDevices";
			this.Text = "Imaging Devices";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormImagingDevices_FormClosing);
			this.Load += new System.EventHandler(this.FormImagingDevices_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textComputer;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
	}
}