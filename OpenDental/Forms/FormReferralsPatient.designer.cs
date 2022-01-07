using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReferralsPatient {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReferralsPatient));
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkShowAll = new System.Windows.Forms.CheckBox();
			this.butAddTo = new OpenDental.UI.Button();
			this.butSlip = new OpenDental.UI.Button();
			this.butAddFrom = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butAddCustom = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(839, 261);
			this.gridMain.TabIndex = 74;
			this.gridMain.Title = "Referrals Attached";
			this.gridMain.TranslationName = "TableRefList";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkShowAll
			// 
			this.checkShowAll.Location = new System.Drawing.Point(560, 18);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(162, 20);
			this.checkShowAll.TabIndex = 25;
			this.checkShowAll.Text = "Show All";
			this.checkShowAll.UseVisualStyleBackColor = true;
			this.checkShowAll.Click += new System.EventHandler(this.checkShowAll_Click);
			// 
			// butAddTo
			// 
			this.butAddTo.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTo.Location = new System.Drawing.Point(127, 10);
			this.butAddTo.Name = "butAddTo";
			this.butAddTo.Size = new System.Drawing.Size(94, 24);
			this.butAddTo.TabIndex = 5;
			this.butAddTo.Text = "Refer To";
			this.butAddTo.Click += new System.EventHandler(this.butAddTo_Click);
			// 
			// butSlip
			// 
			this.butSlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSlip.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSlip.Location = new System.Drawing.Point(12, 317);
			this.butSlip.Name = "butSlip";
			this.butSlip.Size = new System.Drawing.Size(86, 24);
			this.butSlip.TabIndex = 10;
			this.butSlip.Text = "Referral Slip";
			this.butSlip.Click += new System.EventHandler(this.butSlip_Click);
			// 
			// butAddFrom
			// 
			this.butAddFrom.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddFrom.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddFrom.Location = new System.Drawing.Point(12, 10);
			this.butAddFrom.Name = "butAddFrom";
			this.butAddFrom.Size = new System.Drawing.Size(109, 24);
			this.butAddFrom.TabIndex = 1;
			this.butAddFrom.Text = "Referred From";
			this.butAddFrom.Click += new System.EventHandler(this.butAddFrom_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(776, 317);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 35;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(695, 316);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 30;
			this.butOK.Text = "OK";
			this.butOK.Visible = false;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(314, 317);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 26);
			this.butUp.TabIndex = 15;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(402, 317);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 26);
			this.butDown.TabIndex = 20;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butAddCustom
			// 
			this.butAddCustom.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCustom.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCustom.Location = new System.Drawing.Point(227, 10);
			this.butAddCustom.Name = "butAddCustom";
			this.butAddCustom.Size = new System.Drawing.Size(109, 24);
			this.butAddCustom.TabIndex = 75;
			this.butAddCustom.Text = "Refer Custom";
			this.butAddCustom.Click += new System.EventHandler(this.butAddCustom_Click);
			// 
			// FormReferralsPatient
			// 
			this.ClientSize = new System.Drawing.Size(863, 352);
			this.Controls.Add(this.butAddCustom);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkShowAll);
			this.Controls.Add(this.butAddTo);
			this.Controls.Add(this.butSlip);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAddFrom);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReferralsPatient";
			this.ShowInTaskbar = false;
			this.Text = "Referrals for Patient";
			this.Load += new System.EventHandler(this.FormReferralsPatient_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private UI.Button butOK;
		private OpenDental.UI.Button butAddFrom;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butSlip;
		private UI.Button butAddTo;
		private CheckBox checkShowAll;
		private UI.Button butUp;
		private UI.Button butDown;
		private UI.Button butAddCustom;
	}
}
