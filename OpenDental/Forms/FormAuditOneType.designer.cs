using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAuditOneType {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAuditOneType));
			this.grid = new OpenDental.UI.GridOD();
			this.labelDisclaimer = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid.Location = new System.Drawing.Point(8, 21);
			this.grid.Name = "grid";
			this.grid.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.grid.Size = new System.Drawing.Size(889, 602);
			this.grid.TabIndex = 2;
			this.grid.Title = "Audit Trail";
			this.grid.TranslationName = "TableAudit";
			// 
			// labelDisclaimer
			// 
			this.labelDisclaimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDisclaimer.ForeColor = System.Drawing.Color.Firebrick;
			this.labelDisclaimer.Location = new System.Drawing.Point(8, 3);
			this.labelDisclaimer.Name = "labelDisclaimer";
			this.labelDisclaimer.Size = new System.Drawing.Size(780, 15);
			this.labelDisclaimer.TabIndex = 3;
			this.labelDisclaimer.Text = "Changes made to this appointment before the update to 12.3 will not be reflected " +
    "below, but can be found in the regular audit trail.";
			this.labelDisclaimer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormAuditOneType
			// 
			this.ClientSize = new System.Drawing.Size(905, 634);
			this.Controls.Add(this.labelDisclaimer);
			this.Controls.Add(this.grid);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAuditOneType";
			this.ShowInTaskbar = false;
			this.Text = "Audit Trail";
			this.Load += new System.EventHandler(this.FormAuditOneType_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private Label labelDisclaimer;
		private OpenDental.UI.GridOD grid;
	}
}
