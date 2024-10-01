namespace OpenDental{
	partial class FormPatientListDiscount {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientListDiscount));
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.AccessibleDescription = "";
			this.gridMain.AccessibleName = "";
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Location = new System.Drawing.Point(22, 26);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(475, 595);
			this.gridMain.TabIndex = 4;
			this.gridMain.Tag = "";
			this.gridMain.Title = "Patients";
			// 
			// FormPatientListDiscount
			// 
			this.ClientSize = new System.Drawing.Size(517, 634);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientListDiscount";
			this.Text = "Patient List";
			this.Load += new System.EventHandler(this.FormPatientListDiscount_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
	}
}