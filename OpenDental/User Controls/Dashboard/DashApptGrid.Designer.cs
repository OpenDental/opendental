namespace OpenDental {
	partial class DashApptGrid {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(0, 0);
			this.gridMain.Name = "gridMain";
			this.gridMain.ShowContextMenu = false;
			this.gridMain.Size = new System.Drawing.Size(897, 398);
			this.gridMain.TabIndex = 68;
			this.gridMain.Title = "Appointments for Patient";
			this.gridMain.TranslationName = "FormDisplayFields";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// DashApptGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridMain);
			this.Name = "DashApptGrid";
			this.Size = new System.Drawing.Size(897, 398);
			this.Load += new System.EventHandler(this.DashApptGrid_Load);
			this.SizeChanged += new System.EventHandler(this.DashApptGrid_SizeChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridMain;
	}
}
