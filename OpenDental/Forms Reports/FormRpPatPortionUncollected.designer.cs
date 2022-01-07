using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPatPortionUncollected {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPatPortionUncollected));
			this.butClose = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboBoxClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridOD = new OpenDental.UI.GridOD();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemSeeAccount = new System.Windows.Forms.ToolStripMenuItem();
			this.odDateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.butPrint = new OpenDental.UI.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(702, 438);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butRefresh.Location = new System.Drawing.Point(689, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboBoxClinicPicker
			// 
			this.comboBoxClinicPicker.IncludeAll = true;
			this.comboBoxClinicPicker.IncludeHiddenInAll = true;
			this.comboBoxClinicPicker.IncludeUnassigned = true;
			this.comboBoxClinicPicker.Location = new System.Drawing.Point(471, 12);
			this.comboBoxClinicPicker.Name = "comboBoxClinicPicker";
			this.comboBoxClinicPicker.SelectionModeMulti = true;
			this.comboBoxClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboBoxClinicPicker.TabIndex = 56;
			// 
			// gridOD
			// 
			this.gridOD.AllowSortingByColumn = true;
			this.gridOD.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOD.ContextMenuStrip = this.contextMenuStrip1;
			this.gridOD.Location = new System.Drawing.Point(12, 42);
			this.gridOD.Name = "gridOD";
			this.gridOD.Size = new System.Drawing.Size(765, 390);
			this.gridOD.TabIndex = 57;
			this.gridOD.Text = "gridOD";
			this.gridOD.TitleVisible = false;
			this.gridOD.TranslationName = "TableReport";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSeeAccount});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
			// 
			// toolStripMenuItemSeeAccount
			// 
			this.toolStripMenuItemSeeAccount.Name = "toolStripMenuItemSeeAccount";
			this.toolStripMenuItemSeeAccount.Size = new System.Drawing.Size(140, 22);
			this.toolStripMenuItemSeeAccount.Text = "See Account";
			this.toolStripMenuItemSeeAccount.Click += new System.EventHandler(this.menuItemAccount_Click);
			// 
			// odDateRangePicker
			// 
			this.odDateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.odDateRangePicker.Location = new System.Drawing.Point(12, 12);
			this.odDateRangePicker.Name = "odDateRangePicker";
			this.odDateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.odDateRangePicker.TabIndex = 58;
			// 
			// butPrint
			// 
			this.butPrint.Location = new System.Drawing.Point(12, 438);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 59;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// FormRpPatPortionUncollected
			// 
			this.ClientSize = new System.Drawing.Size(787, 476);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.odDateRangePicker);
			this.Controls.Add(this.gridOD);
			this.Controls.Add(this.comboBoxClinicPicker);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormRpPatPortionUncollected";
			this.Text = "Patient Portion Uncollected Report";
			this.Load += new System.EventHandler(this.FormPaymentSheet_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butRefresh;
		private UI.ComboBoxClinicPicker comboBoxClinicPicker;
		private UI.GridOD gridOD;
		private UI.ODDateRangePicker odDateRangePicker;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem toolStripMenuItemSeeAccount;
		private UI.Button butPrint;
	}
}
