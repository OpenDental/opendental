using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLabCases {
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLabCases));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkShowAll = new System.Windows.Forms.CheckBox();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItemGoTo = new System.Windows.Forms.MenuItem();
			this.checkShowUnattached = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.textDateTo = new OpenDental.ValidDate();
			this.textDateFrom = new OpenDental.ValidDate();
			this.butClose = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "From Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 4;
			this.label2.Text = "To Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowAll
			// 
			this.checkShowAll.Location = new System.Drawing.Point(361, 37);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(131, 18);
			this.checkShowAll.TabIndex = 7;
			this.checkShowAll.Text = "Show Completed";
			this.checkShowAll.UseVisualStyleBackColor = true;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGoTo});
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Index = 0;
			this.menuItemGoTo.Text = "Go To Appointment";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// checkShowUnattached
			// 
			this.checkShowUnattached.Location = new System.Drawing.Point(361, 14);
			this.checkShowUnattached.Name = "checkShowUnattached";
			this.checkShowUnattached.Size = new System.Drawing.Size(131, 18);
			this.checkShowUnattached.TabIndex = 8;
			this.checkShowUnattached.Text = "Show Unattached";
			this.checkShowUnattached.UseVisualStyleBackColor = true;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(17, 67);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(916, 420);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Lab Cases";
			this.gridMain.TranslationName = "TableLabCases";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(226, 32);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 6;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(116, 35);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(100, 20);
			this.textDateTo.TabIndex = 5;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(116, 9);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(100, 20);
			this.textDateFrom.TabIndex = 3;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(858, 497);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(17, 497);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 53;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(658, 36);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(275, 21);
			this.comboClinic.TabIndex = 54;
			this.comboClinic.SelectedIndexChanged += new System.EventHandler(this.comboClinic_SelectedIndexChanged);
			// 
			// FormLabCases
			// 
			this.ClientSize = new System.Drawing.Size(945, 533);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.checkShowUnattached);
			this.Controls.Add(this.checkShowAll);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLabCases";
			this.ShowInTaskbar = false;
			this.Text = "Lab Cases";
			this.Load += new System.EventHandler(this.FormLabCases_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private Label label1;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private Label label2;
		private OpenDental.UI.Button butRefresh;// Required designer variable.
		private CheckBox checkShowAll;
		private ContextMenu contextMenu1;
		private MenuItem menuItemGoTo;
		private CheckBox checkShowUnattached;
		private UI.Button butPrint;
		private UI.ComboBoxClinicPicker comboClinic;
	}
}
