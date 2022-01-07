using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAccounting {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAccounting));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.checkInactive = new System.Windows.Forms.CheckBox();
			this.labelDate = new System.Windows.Forms.Label();
			this.butToday = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.textDate = new OpenDental.ValidDate();
			this.gridMain = new OpenDental.UI.GridOD();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "Add.gif");
			this.imageListMain.Images.SetKeyName(1, "editPencil.gif");
			this.imageListMain.Images.SetKeyName(2, "butExport.gif");
			// 
			// checkInactive
			// 
			this.checkInactive.AutoSize = true;
			this.checkInactive.Location = new System.Drawing.Point(313, 58);
			this.checkInactive.Name = "checkInactive";
			this.checkInactive.Size = new System.Drawing.Size(150, 17);
			this.checkInactive.TabIndex = 2;
			this.checkInactive.Text = "Include Inactive Accounts";
			this.checkInactive.UseVisualStyleBackColor = true;
			this.checkInactive.Click += new System.EventHandler(this.checkInactive_Click);
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(3, 57);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(72, 18);
			this.labelDate.TabIndex = 7;
			this.labelDate.Text = "As of Date";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(238, 56);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(70, 23);
			this.butToday.TabIndex = 10;
			this.butToday.Text = "Today";
			this.butToday.UseVisualStyleBackColor = true;
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(163, 56);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(70, 23);
			this.butRefresh.TabIndex = 9;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(76, 58);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(81, 20);
			this.textDate.TabIndex = 8;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(0, 81);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(492, 450);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Chart of Accounts";
			this.gridMain.TranslationName = "TableChartOfAccounts";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 24);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(492, 25);
			this.ToolBarMain.TabIndex = 0;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(492, 24);
			this.menuMain.TabIndex = 11;
			// 
			// FormAccounting
			// 
			this.ClientSize = new System.Drawing.Size(492, 531);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.checkInactive);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAccounting";
			this.Text = "Accounting";
			this.Load += new System.EventHandler(this.FormAccounting_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.ToolBarOD ToolBarMain;
		private OpenDental.UI.GridOD gridMain;
		private CheckBox checkInactive;
		private ImageList imageListMain;
		private OpenDental.UI.Button butRefresh;
		private ValidDate textDate;
		private Label labelDate;
		private OpenDental.UI.Button butToday;
		private OpenDental.UI.MenuOD menuMain;
	}
}
