using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormJournal {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJournal));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.labelDateFrom = new System.Windows.Forms.Label();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.labelDateTo = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.calendarFrom = new System.Windows.Forms.MonthCalendar();
			this.butDropFrom = new OpenDental.UI.Button();
			this.butDropTo = new OpenDental.UI.Button();
			this.calendarTo = new System.Windows.Forms.MonthCalendar();
			this.textAmountFrom = new OpenDental.ValidDouble();
			this.labelAmout = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textFindText = new System.Windows.Forms.TextBox();
			this.textAmountTo = new OpenDental.ValidDouble();
			this.labelAmountMax = new System.Windows.Forms.Label();
			this.labelOptional = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "Add.gif");
			this.imageListMain.Images.SetKeyName(1, "print.gif");
			this.imageListMain.Images.SetKeyName(2, "butExport.gif");
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1044, 25);
			this.ToolBarMain.TabIndex = 0;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// labelDateFrom
			// 
			this.labelDateFrom.Location = new System.Drawing.Point(6, 34);
			this.labelDateFrom.Name = "labelDateFrom";
			this.labelDateFrom.Size = new System.Drawing.Size(71, 17);
			this.labelDateFrom.TabIndex = 0;
			this.labelDateFrom.Text = "From";
			this.labelDateFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(78, 32);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(81, 20);
			this.textDateFrom.TabIndex = 1;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(268, 32);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(81, 20);
			this.textDateTo.TabIndex = 5;
			// 
			// labelDateTo
			// 
			this.labelDateTo.Location = new System.Drawing.Point(195, 34);
			this.labelDateTo.Name = "labelDateTo";
			this.labelDateTo.Size = new System.Drawing.Size(72, 17);
			this.labelDateTo.TabIndex = 0;
			this.labelDateTo.Text = "To";
			this.labelDateTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(961, 31);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 10;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// calendarFrom
			// 
			this.calendarFrom.Location = new System.Drawing.Point(5, 56);
			this.calendarFrom.MaxSelectionCount = 1;
			this.calendarFrom.Name = "calendarFrom";
			this.calendarFrom.TabIndex = 4;
			this.calendarFrom.Visible = false;
			this.calendarFrom.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarFrom_DateSelected);
			// 
			// butDropFrom
			// 
			this.butDropFrom.Location = new System.Drawing.Point(161, 31);
			this.butDropFrom.Name = "butDropFrom";
			this.butDropFrom.Size = new System.Drawing.Size(22, 22);
			this.butDropFrom.TabIndex = 3;
			this.butDropFrom.Text = "V";
			this.butDropFrom.UseVisualStyleBackColor = true;
			this.butDropFrom.Click += new System.EventHandler(this.butDropFrom_Click);
			// 
			// butDropTo
			// 
			this.butDropTo.Location = new System.Drawing.Point(351, 31);
			this.butDropTo.Name = "butDropTo";
			this.butDropTo.Size = new System.Drawing.Size(22, 22);
			this.butDropTo.TabIndex = 6;
			this.butDropTo.Text = "V";
			this.butDropTo.UseVisualStyleBackColor = true;
			this.butDropTo.Click += new System.EventHandler(this.butDropTo_Click);
			// 
			// calendarTo
			// 
			this.calendarTo.Location = new System.Drawing.Point(231, 56);
			this.calendarTo.MaxSelectionCount = 1;
			this.calendarTo.Name = "calendarTo";
			this.calendarTo.TabIndex = 7;
			this.calendarTo.Visible = false;
			this.calendarTo.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarTo_DateSelected);
			// 
			// textAmountFrom
			// 
			this.textAmountFrom.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textAmountFrom.Location = new System.Drawing.Point(516, 32);
			this.textAmountFrom.MaxVal = 100000000D;
			this.textAmountFrom.MinVal = -100000000D;
			this.textAmountFrom.Name = "textAmountFrom";
			this.textAmountFrom.Size = new System.Drawing.Size(81, 20);
			this.textAmountFrom.TabIndex = 8;
			// 
			// labelAmout
			// 
			this.labelAmout.Location = new System.Drawing.Point(452, 34);
			this.labelAmout.Name = "labelAmout";
			this.labelAmout.Size = new System.Drawing.Size(63, 17);
			this.labelAmout.TabIndex = 0;
			this.labelAmout.Text = "Amt";
			this.labelAmout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(791, 34);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Find Text";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFindText
			// 
			this.textFindText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFindText.Location = new System.Drawing.Point(855, 32);
			this.textFindText.Name = "textFindText";
			this.textFindText.Size = new System.Drawing.Size(78, 20);
			this.textFindText.TabIndex = 9;
			// 
			// textAmountTo
			// 
			this.textAmountTo.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textAmountTo.Location = new System.Drawing.Point(656, 32);
			this.textAmountTo.MaxVal = 100000000D;
			this.textAmountTo.MinVal = -100000000D;
			this.textAmountTo.Name = "textAmountTo";
			this.textAmountTo.Size = new System.Drawing.Size(81, 20);
			this.textAmountTo.TabIndex = 12;
			// 
			// labelAmountMax
			// 
			this.labelAmountMax.Location = new System.Drawing.Point(599, 34);
			this.labelAmountMax.Name = "labelAmountMax";
			this.labelAmountMax.Size = new System.Drawing.Size(56, 17);
			this.labelAmountMax.TabIndex = 13;
			this.labelAmountMax.Text = "Amt Max";
			this.labelAmountMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOptional
			// 
			this.labelOptional.Location = new System.Drawing.Point(656, 54);
			this.labelOptional.Name = "labelOptional";
			this.labelOptional.Size = new System.Drawing.Size(80, 17);
			this.labelOptional.TabIndex = 14;
			this.labelOptional.Text = "(optional)";
			this.labelOptional.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(0, 74);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1044, 597);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableJournal";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormJournal
			// 
			this.ClientSize = new System.Drawing.Size(1044, 671);
			this.Controls.Add(this.calendarFrom);
			this.Controls.Add(this.calendarTo);
			this.Controls.Add(this.labelOptional);
			this.Controls.Add(this.labelAmout);
			this.Controls.Add(this.textAmountTo);
			this.Controls.Add(this.labelAmountMax);
			this.Controls.Add(this.textFindText);
			this.Controls.Add(this.textAmountFrom);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butDropTo);
			this.Controls.Add(this.butDropFrom);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.labelDateTo);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.labelDateFrom);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.ToolBarMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormJournal";
			this.ShowInTaskbar = false;
			this.Text = "Transaction History";
			this.Load += new System.EventHandler(this.FormJournal_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.ToolBarOD ToolBarMain;
		private OpenDental.UI.GridOD gridMain;
		private ImageList imageListMain;
		private Label labelDateFrom;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private Label labelDateTo;
		private OpenDental.UI.Button butRefresh;
		private MonthCalendar calendarFrom;
		private OpenDental.UI.Button butDropFrom;
		private OpenDental.UI.Button butDropTo;
		private MonthCalendar calendarTo;
		private ValidDouble textAmountFrom;
		private Label labelAmout;
		private Label label4;
		private TextBox textFindText;
		private ValidDouble textAmountTo;
		private Label labelAmountMax;
		private Label labelOptional;
	}
}
