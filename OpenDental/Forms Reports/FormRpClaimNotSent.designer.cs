using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpClaimNotSent {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpClaimNotSent));
			this.butCancel = new OpenDental.UI.Button();
			this.butRunReport = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBoxFilters = new System.Windows.Forms.GroupBox();
			this.comboClinicMulti = new OpenDental.UI.ComboBoxClinicPicker();
			this.odDateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.comboBoxInsFilter = new System.Windows.Forms.ComboBox();
			this.labelClaimFilter = new System.Windows.Forms.Label();
			this.groupBoxFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1049, 591);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butRunReport
			// 
			this.butRunReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRunReport.Location = new System.Drawing.Point(12, 591);
			this.butRunReport.Name = "butRunReport";
			this.butRunReport.Size = new System.Drawing.Size(75, 26);
			this.butRunReport.TabIndex = 3;
			this.butRunReport.Text = "&Run Report";
			this.butRunReport.Click += new System.EventHandler(this.butRunReport_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(1035, 12);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 60;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 65);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1112, 517);
			this.gridMain.TabIndex = 61;
			this.gridMain.Title = "Claims Not Sent";
			this.gridMain.TranslationName = "TableClaimsNotSent";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Controls.Add(this.comboClinicMulti);
			this.groupBoxFilters.Controls.Add(this.odDateRangePicker);
			this.groupBoxFilters.Controls.Add(this.comboBoxInsFilter);
			this.groupBoxFilters.Controls.Add(this.butRefresh);
			this.groupBoxFilters.Controls.Add(this.labelClaimFilter);
			this.groupBoxFilters.Location = new System.Drawing.Point(12, 12);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(1112, 47);
			this.groupBoxFilters.TabIndex = 63;
			this.groupBoxFilters.TabStop = false;
			this.groupBoxFilters.Text = "Filters";
			// 
			// comboClinicMulti
			// 
			this.comboClinicMulti.IncludeAll = true;
			this.comboClinicMulti.IncludeHiddenInAll = true;
			this.comboClinicMulti.IncludeUnassigned = true;
			this.comboClinicMulti.SelectionModeMulti = true;
			this.comboClinicMulti.Location = new System.Drawing.Point(446, 15);
			this.comboClinicMulti.Name = "comboClinicMulti";
			this.comboClinicMulti.Size = new System.Drawing.Size(176, 21);
			this.comboClinicMulti.TabIndex = 67;
			// 
			// odDateRangePicker
			// 
			this.odDateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.odDateRangePicker.Location = new System.Drawing.Point(6, 13);
			this.odDateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.odDateRangePicker.MinimumSize = new System.Drawing.Size(0, 24);
			this.odDateRangePicker.Name = "odDateRangePicker";
			this.odDateRangePicker.Size = new System.Drawing.Size(445, 24);
			this.odDateRangePicker.TabIndex = 66;
			// 
			// comboBoxInsFilter
			// 
			this.comboBoxInsFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxInsFilter.FormattingEnabled = true;
			this.comboBoxInsFilter.Location = new System.Drawing.Point(741, 15);
			this.comboBoxInsFilter.Name = "comboBoxInsFilter";
			this.comboBoxInsFilter.Size = new System.Drawing.Size(121, 21);
			this.comboBoxInsFilter.TabIndex = 65;
			// 
			// labelClaimFilter
			// 
			this.labelClaimFilter.Location = new System.Drawing.Point(645, 18);
			this.labelClaimFilter.Name = "labelClaimFilter";
			this.labelClaimFilter.Size = new System.Drawing.Size(93, 14);
			this.labelClaimFilter.TabIndex = 64;
			this.labelClaimFilter.Text = "Claim Filter";
			this.labelClaimFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormRpClaimNotSent
			// 
			this.AcceptButton = this.butRunReport;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1136, 626);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butRunReport);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpClaimNotSent";
			this.Text = "Claims Not Sent Report";
			this.Load += new System.EventHandler(this.FormClaimNotSent_Load);
			this.groupBoxFilters.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butRunReport;
		private UI.Button butRefresh;
		private UI.GridOD gridMain;
		private GroupBox groupBoxFilters;
		private Label labelClaimFilter;
		private ComboBox comboBoxInsFilter;
		private UI.ODDateRangePicker odDateRangePicker;
		private UI.ComboBoxClinicPicker comboClinicMulti;
	}
}
