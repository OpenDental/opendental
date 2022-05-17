namespace OpenDental {
	partial class FormMultiVisitGroup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultiVisitGroup));
			this.butCancel = new OpenDental.UI.Button();
			this.gridGroupedProcs = new OpenDental.UI.GridOD();
			this.butUngroup = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(647, 525);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridGroupedProcs
			// 
			this.gridGroupedProcs.AllowSortingByColumn = true;
			this.gridGroupedProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridGroupedProcs.Location = new System.Drawing.Point(13, 13);
			this.gridGroupedProcs.Name = "gridGroupedProcs";
			this.gridGroupedProcs.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridGroupedProcs.Size = new System.Drawing.Size(709, 506);
			this.gridGroupedProcs.TabIndex = 3;
			this.gridGroupedProcs.Title = "Multi-Visit Procedure Progress Notes";
			this.gridGroupedProcs.TranslationName = "TableMultiVisitGroup";
			this.gridGroupedProcs.Click += new System.EventHandler(this.gridGroupedProcs_Click);
			// 
			// butUngroup
			// 
			this.butUngroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUngroup.Location = new System.Drawing.Point(566, 525);
			this.butUngroup.Name = "butUngroup";
			this.butUngroup.Size = new System.Drawing.Size(75, 24);
			this.butUngroup.TabIndex = 4;
			this.butUngroup.Text = "&Ungroup";
			this.butUngroup.Click += new System.EventHandler(this.butUngroup_Click);
			// 
			// FormMultiVisitGroup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(734, 561);
			this.Controls.Add(this.butUngroup);
			this.Controls.Add(this.gridGroupedProcs);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMultiVisitGroup";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "View Multi Visit Group";
			this.Load += new System.EventHandler(this.FormMultiVisitGroup_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridGroupedProcs;
		private UI.Button butUngroup;
	}
}