using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormApptViewEditMobile {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptViewEditMobile));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label8 = new System.Windows.Forms.Label();
			this.gridAvailable = new OpenDental.UI.GridOD();
			this.gridApptFieldDefs = new OpenDental.UI.GridOD();
			this.gridPatFieldDefs = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(459, 640);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(359, 640);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 640);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(87, 24);
			this.butDelete.TabIndex = 38;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(11, 525);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(71, 24);
			this.butDown.TabIndex = 50;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(11, 495);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(71, 24);
			this.butUp.TabIndex = 51;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(206, 205);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 52;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(206, 171);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 53;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.gridMain);
			this.groupBox2.Controls.Add(this.butUp);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.butDown);
			this.groupBox2.Location = new System.Drawing.Point(247, 37);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(234, 560);
			this.groupBox2.TabIndex = 59;
			this.groupBox2.Text = "Rows Displayed";
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(11, 18);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(212, 454);
			this.gridMain.TabIndex = 60;
			this.gridMain.Title = "Main List";
			this.gridMain.TranslationName = "TableMainList";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(8, 475);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(203, 17);
			this.label8.TabIndex = 59;
			this.label8.Text = "Move any item within its own list:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridAvailable
			// 
			this.gridAvailable.Location = new System.Drawing.Point(24, 37);
			this.gridAvailable.Name = "gridAvailable";
			this.gridAvailable.Size = new System.Drawing.Size(175, 255);
			this.gridAvailable.TabIndex = 61;
			this.gridAvailable.Title = "Available Rows";
			this.gridAvailable.TranslationName = "TableAvailableRows";
			this.gridAvailable.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAvailable_CellClick);
			// 
			// gridApptFieldDefs
			// 
			this.gridApptFieldDefs.Location = new System.Drawing.Point(24, 298);
			this.gridApptFieldDefs.Name = "gridApptFieldDefs";
			this.gridApptFieldDefs.Size = new System.Drawing.Size(175, 106);
			this.gridApptFieldDefs.TabIndex = 62;
			this.gridApptFieldDefs.Title = "Appt Field Defs";
			this.gridApptFieldDefs.TranslationName = "TableApptFieldDefs";
			this.gridApptFieldDefs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridApptFieldDefs_CellClick);
			// 
			// gridPatFieldDefs
			// 
			this.gridPatFieldDefs.Location = new System.Drawing.Point(24, 410);
			this.gridPatFieldDefs.Name = "gridPatFieldDefs";
			this.gridPatFieldDefs.Size = new System.Drawing.Size(175, 106);
			this.gridPatFieldDefs.TabIndex = 63;
			this.gridPatFieldDefs.Title = "Patient Field Defs";
			this.gridPatFieldDefs.TranslationName = "TablePatFieldDefs";
			this.gridPatFieldDefs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatFieldDefs_CellClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(319, 23);
			this.label1.TabIndex = 141;
			this.label1.Text = "Note: All text will be black in mobile appointment view";
			// 
			// FormApptViewEditMobile
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(559, 673);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridPatFieldDefs);
			this.Controls.Add(this.gridApptFieldDefs);
			this.Controls.Add(this.gridAvailable);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptViewEditMobile";
			this.ShowInTaskbar = false;
			this.Text = "Mobile Appointment View Edit";
			this.Load += new System.EventHandler(this.FormApptViewEdit_Load);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butLeft;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.GroupBox groupBox2;
		private Label label8;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.GridOD gridAvailable;
		private OpenDental.UI.GridOD gridApptFieldDefs;
		private OpenDental.UI.GridOD gridPatFieldDefs;
		private Label label1;
	}
}
