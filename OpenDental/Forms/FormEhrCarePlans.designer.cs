namespace OpenDental{
	partial class FormEhrCarePlans {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrCarePlans));
			this.gridCarePlans = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridCarePlans
			// 
			this.gridCarePlans.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridCarePlans.Location = new System.Drawing.Point(12, 12);
			this.gridCarePlans.Name = "gridCarePlans";
			this.gridCarePlans.Size = new System.Drawing.Size(958, 653);
			this.gridCarePlans.TabIndex = 4;
			this.gridCarePlans.Title = null;
			this.gridCarePlans.TranslationName = "TableCarePlans";
			this.gridCarePlans.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCarePlans_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(895, 671);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 671);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(86, 24);
			this.butAdd.TabIndex = 72;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// FormEhrCarePlans
			// 
			this.ClientSize = new System.Drawing.Size(982, 707);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridCarePlans);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrCarePlans";
			this.Text = "Care Plans";
			this.Load += new System.EventHandler(this.FormEhrCarePlans_Load);
			this.Resize += new System.EventHandler(this.FormEhrCarePlans_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridCarePlans;
		private UI.Button butAdd;
	}
}