using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTimeCardBenefitRp {
		/// <summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
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
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.checkShowAll = new System.Windows.Forms.CheckBox();
			this.checkIgnore = new System.Windows.Forms.CheckBox();
			this.butExportGrid = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(519, 660);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(86, 24);
			this.butPrint.TabIndex = 22;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(726, 642);
			this.gridMain.TabIndex = 21;
			this.gridMain.Title = "";
			this.gridMain.TranslationName = "TableTimeCard";
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(663, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 20;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkShowAll
			// 
			this.checkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShowAll.AutoSize = true;
			this.checkShowAll.Location = new System.Drawing.Point(12, 665);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(67, 17);
			this.checkShowAll.TabIndex = 23;
			this.checkShowAll.Text = "Show All";
			this.checkShowAll.UseVisualStyleBackColor = true;
			this.checkShowAll.CheckedChanged += new System.EventHandler(this.checkShowAll_CheckedChanged);
			// 
			// checkIgnore
			// 
			this.checkIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkIgnore.AutoSize = true;
			this.checkIgnore.Checked = true;
			this.checkIgnore.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIgnore.Location = new System.Drawing.Point(85, 665);
			this.checkIgnore.Name = "checkIgnore";
			this.checkIgnore.Size = new System.Drawing.Size(124, 17);
			this.checkIgnore.TabIndex = 24;
			this.checkIgnore.Text = "Ignore current month";
			this.checkIgnore.UseVisualStyleBackColor = true;
			this.checkIgnore.CheckedChanged += new System.EventHandler(this.checkIgnore_CheckedChanged);
			// 
			// butExportGrid
			// 
			this.butExportGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExportGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExportGrid.Location = new System.Drawing.Point(431, 660);
			this.butExportGrid.Name = "butExportGrid";
			this.butExportGrid.Size = new System.Drawing.Size(82, 24);
			this.butExportGrid.TabIndex = 128;
			this.butExportGrid.Text = "Export Grid";
			this.butExportGrid.Click += new System.EventHandler(this.butExportGrid_Click);
			// 
			// FormTimeCardBenefitRp
			// 
			this.ClientSize = new System.Drawing.Size(750, 696);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butExportGrid);
			this.Controls.Add(this.checkIgnore);
			this.Controls.Add(this.checkShowAll);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Name = "FormTimeCardBenefitRp";
			this.Text = "Benefit Eligibility Letters Report";
			this.Load += new System.EventHandler(this.FormTimeCardBenefitRp_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private UI.Button butPrint;
		private UI.GridOD gridMain;
		private UI.Button butClose;
		private CheckBox checkShowAll;
		private CheckBox checkIgnore;
		private UI.Button butExportGrid;
	}
}
