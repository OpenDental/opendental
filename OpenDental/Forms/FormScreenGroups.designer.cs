using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormScreenGroups {
		private System.ComponentModel.IContainer components=null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScreenGroups));
			this.textDateFrom = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateTo = new System.Windows.Forms.TextBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butToday = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(150, 52);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(69, 20);
			this.textDateFrom.TabIndex = 74;
			this.textDateFrom.Validating += new System.ComponentModel.CancelEventHandler(this.textDateFrom_Validating);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(218, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 13);
			this.label2.TabIndex = 77;
			this.label2.Text = "To";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(243, 52);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(75, 20);
			this.textDateTo.TabIndex = 76;
			this.textDateTo.Validating += new System.ComponentModel.CancelEventHandler(this.textDateTo_Validating);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(326, 51);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(55, 21);
			this.butRefresh.TabIndex = 78;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(441, 48);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(70, 24);
			this.butAdd.TabIndex = 79;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClose.Location = new System.Drawing.Point(442, 540);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(70, 24);
			this.butClose.TabIndex = 79;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(13, 82);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(499, 444);
			this.gridMain.TabIndex = 80;
			this.gridMain.Title = "Screening Groups";
			this.gridMain.TranslationName = "TableGroups";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(167, 13);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(39, 24);
			this.butLeft.TabIndex = 78;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(307, 13);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(39, 24);
			this.butRight.TabIndex = 78;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(215, 13);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(83, 24);
			this.butToday.TabIndex = 78;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(92, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 77;
			this.label1.Text = "From";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormScreenGroups
			// 
			this.ClientSize = new System.Drawing.Size(524, 576);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormScreenGroups";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Screening Groups";
			this.Load += new System.EventHandler(this.FormScreenings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TextBox textDateFrom;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butRefresh;
		private System.Windows.Forms.TextBox textDateTo;
		private OpenDental.UI.Button butAdd;
		private UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.Button butLeft;
		private UI.Button butRight;
		private UI.Button butToday;
		private Label label1;
	}
}
