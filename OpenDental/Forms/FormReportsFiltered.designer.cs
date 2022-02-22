namespace OpenDental{
	partial class FormReportsFiltered{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportsFiltered));
			this.butCancel = new OpenDental.UI.Button();
			this.listBoxFavorites = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(304, 602);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listBoxFavorites
			// 
			this.listBoxFavorites.Location = new System.Drawing.Point(13, 12);
			this.listBoxFavorites.Name = "listBoxFavorites";
			this.listBoxFavorites.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listBoxFavorites.Size = new System.Drawing.Size(351, 563);
			this.listBoxFavorites.TabIndex = 5;
			this.listBoxFavorites.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxFavorites_MouseDown);
			// 
			// FormReportsFiltered
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(391, 638);
			this.Controls.Add(this.listBoxFavorites);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportsFiltered";
			this.Text = "Standard Reports Favorites";
			this.Load += new System.EventHandler(this.FormReportsFiltered_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.ListBoxOD listBoxFavorites;
	}
}