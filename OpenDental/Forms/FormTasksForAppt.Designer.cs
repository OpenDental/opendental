namespace OpenDental {
	partial class FormTasksForAppt {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTasksForAppt));
			this.gridMain = new OpenDental.UI.GridOD();
			this.label10 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.DoShowRightClickLinks = true;
			this.gridMain.Location = new System.Drawing.Point(12, 29);
			this.gridMain.Name = "gridMain";
			this.gridMain.NoteSpanStart = 2;
			this.gridMain.NoteSpanStop = 2;
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(934, 651);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Tasks";
			this.gridMain.TranslationName = "TableProg";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellDoubleClick);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 8);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(416, 16);
			this.label10.TabIndex = 183;
			this.label10.Text = "Add new Appointment Tasks from within the Appt Edit window";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormTasksForAppt
			// 
			this.ClientSize = new System.Drawing.Size(958, 692);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTasksForAppt";
			this.Text = "Appointment Tasks";
			this.Load += new System.EventHandler(this.FormTasksForAppt_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label10;
	}
}