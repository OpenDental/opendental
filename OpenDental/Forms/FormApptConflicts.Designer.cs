namespace OpenDental {
	partial class FormApptConflicts {
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
			this.butPrint = new OpenDental.UI.Button();
			this.gridConflicts = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.contextRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemSelectPatient = new System.Windows.Forms.MenuItem();
			this.menuItemPin = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// butPrint
			// 
			this.butPrint.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(386, 636);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 24;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridConflicts
			// 
			this.gridConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridConflicts.Location = new System.Drawing.Point(10, 12);
			this.gridConflicts.Name = "gridConflicts";
			this.gridConflicts.Size = new System.Drawing.Size(836, 616);
			this.gridConflicts.TabIndex = 23;
			this.gridConflicts.Title = "Conflicting Appointments";
			this.gridConflicts.TranslationName = "TableApptConflicts";
			this.gridConflicts.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridConflicts_DoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(759, 636);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(87, 24);
			this.butClose.TabIndex = 22;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// contextRightClick
			// 
			this.contextRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSelectPatient,
            this.menuItemPin});
			// 
			// menuItemSelectPatient
			// 
			this.menuItemSelectPatient.Index = 0;
			this.menuItemSelectPatient.Text = "Select Patient";
			this.menuItemSelectPatient.Click += new System.EventHandler(this.menuItemSelectPatient_Click);
			// 
			// menuItemPin
			// 
			this.menuItemPin.Index = 1;
			this.menuItemPin.Text = "Send to Pinboard";
			this.menuItemPin.Click += new System.EventHandler(this.menuItemPin_Click);
			// 
			// FormApptConflicts
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(858, 672);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridConflicts);
			this.Controls.Add(this.butClose);
			this.Name = "FormApptConflicts";
			this.Text = "Appointment Conflicts";
			this.Load += new System.EventHandler(this.FormApptConflicts_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butPrint;
		private UI.GridOD gridConflicts;
		private UI.Button butClose;
		private System.Windows.Forms.ContextMenu contextRightClick;
		private System.Windows.Forms.MenuItem menuItemSelectPatient;
		private System.Windows.Forms.MenuItem menuItemPin;
	}
}