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
			this.listBoxFavorites = new OpenDental.UI.ListBox();
			this.SuspendLayout();
			// 
			// listBoxFavorites
			// 
			this.listBoxFavorites.Location = new System.Drawing.Point(13, 12);
			this.listBoxFavorites.Name = "listBoxFavorites";
			this.listBoxFavorites.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listBoxFavorites.Size = new System.Drawing.Size(351, 605);
			this.listBoxFavorites.TabIndex = 5;
			this.listBoxFavorites.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxFavorites_MouseDown);
			// 
			// FormReportsFiltered
			// 
			this.ClientSize = new System.Drawing.Size(391, 638);
			this.Controls.Add(this.listBoxFavorites);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportsFiltered";
			this.Text = "Standard Reports Favorites";
			this.Load += new System.EventHandler(this.FormReportsFiltered_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ListBox listBoxFavorites;
	}
}