using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClinics {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClinics));
			this.groupClinicOrder = new System.Windows.Forms.GroupBox();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.checkOrderAlphabetical = new System.Windows.Forms.CheckBox();
			this.groupMovePats = new System.Windows.Forms.GroupBox();
			this.butMovePats = new OpenDental.UI.Button();
			this.butClinicPick = new OpenDental.UI.Button();
			this.textMoveTo = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butSelectNone = new OpenDental.UI.Button();
			this.groupClinicOrder.SuspendLayout();
			this.groupMovePats.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupClinicOrder
			// 
			this.groupClinicOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupClinicOrder.Controls.Add(this.butUp);
			this.groupClinicOrder.Controls.Add(this.butDown);
			this.groupClinicOrder.Controls.Add(this.checkOrderAlphabetical);
			this.groupClinicOrder.Location = new System.Drawing.Point(668, 126);
			this.groupClinicOrder.Name = "groupClinicOrder";
			this.groupClinicOrder.Size = new System.Drawing.Size(238, 91);
			this.groupClinicOrder.TabIndex = 20;
			this.groupClinicOrder.TabStop = false;
			this.groupClinicOrder.Text = "Clinic Order";
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(6, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 26);
			this.butUp.TabIndex = 4;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(6, 54);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 26);
			this.butDown.TabIndex = 5;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// checkOrderAlphabetical
			// 
			this.checkOrderAlphabetical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderAlphabetical.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrderAlphabetical.Location = new System.Drawing.Point(113, 40);
			this.checkOrderAlphabetical.Name = "checkOrderAlphabetical";
			this.checkOrderAlphabetical.Size = new System.Drawing.Size(120, 18);
			this.checkOrderAlphabetical.TabIndex = 16;
			this.checkOrderAlphabetical.Text = "Order Alphabetical";
			this.checkOrderAlphabetical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderAlphabetical.UseVisualStyleBackColor = true;
			this.checkOrderAlphabetical.Click += new System.EventHandler(this.checkOrderAlphabetical_Click);
			// 
			// groupMovePats
			// 
			this.groupMovePats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupMovePats.Controls.Add(this.butMovePats);
			this.groupMovePats.Controls.Add(this.butClinicPick);
			this.groupMovePats.Controls.Add(this.textMoveTo);
			this.groupMovePats.Controls.Add(this.label2);
			this.groupMovePats.Location = new System.Drawing.Point(668, 37);
			this.groupMovePats.Name = "groupMovePats";
			this.groupMovePats.Size = new System.Drawing.Size(238, 83);
			this.groupMovePats.TabIndex = 18;
			this.groupMovePats.TabStop = false;
			this.groupMovePats.Text = "Move Patients";
			// 
			// butMovePats
			// 
			this.butMovePats.Location = new System.Drawing.Point(154, 46);
			this.butMovePats.Name = "butMovePats";
			this.butMovePats.Size = new System.Drawing.Size(75, 26);
			this.butMovePats.TabIndex = 15;
			this.butMovePats.Text = "&Move";
			this.butMovePats.UseVisualStyleBackColor = true;
			this.butMovePats.Click += new System.EventHandler(this.butMovePats_Click);
			// 
			// butClinicPick
			// 
			this.butClinicPick.Location = new System.Drawing.Point(206, 20);
			this.butClinicPick.Name = "butClinicPick";
			this.butClinicPick.Size = new System.Drawing.Size(23, 20);
			this.butClinicPick.TabIndex = 23;
			this.butClinicPick.Text = "...";
			this.butClinicPick.Click += new System.EventHandler(this.butClinicPick_Click);
			// 
			// textMoveTo
			// 
			this.textMoveTo.Location = new System.Drawing.Point(67, 20);
			this.textMoveTo.MaxLength = 15;
			this.textMoveTo.Name = "textMoveTo";
			this.textMoveTo.ReadOnly = true;
			this.textMoveTo.Size = new System.Drawing.Size(135, 20);
			this.textMoveTo.TabIndex = 22;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 18);
			this.label2.TabIndex = 18;
			this.label2.Text = "To Clinic";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Checked = true;
			this.checkShowHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowHidden.Location = new System.Drawing.Point(506, 12);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(124, 18);
			this.checkShowHidden.TabIndex = 17;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.UseVisualStyleBackColor = true;
			this.checkShowHidden.CheckedChanged += new System.EventHandler(this.checkShowHidden_CheckedChanged);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(827, 541);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Visible = false;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 37);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(650, 562);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Clinics";
			this.gridMain.TranslationName = "TableClinics";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(351, 18);
			this.label1.TabIndex = 11;
			this.label1.Text = "This is usually only used if you have multiple locations";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(827, 223);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(827, 573);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectAll.Location = new System.Drawing.Point(668, 223);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(81, 26);
			this.butSelectAll.TabIndex = 21;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.UseVisualStyleBackColor = true;
			this.butSelectAll.Visible = false;
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// butSelectNone
			// 
			this.butSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectNone.Location = new System.Drawing.Point(668, 255);
			this.butSelectNone.Name = "butSelectNone";
			this.butSelectNone.Size = new System.Drawing.Size(81, 26);
			this.butSelectNone.TabIndex = 22;
			this.butSelectNone.Text = "Select None";
			this.butSelectNone.UseVisualStyleBackColor = true;
			this.butSelectNone.Visible = false;
			this.butSelectNone.Click += new System.EventHandler(this.butSelectNone_Click);
			// 
			// FormClinics
			// 
			this.ClientSize = new System.Drawing.Size(914, 611);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butSelectNone);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.groupClinicOrder);
			this.Controls.Add(this.groupMovePats);
			this.Controls.Add(this.checkShowHidden);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClinics";
			this.ShowInTaskbar = false;
			this.Text = "Clinics";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClinics_Closing);
			this.Load += new System.EventHandler(this.FormClinics_Load);
			this.groupClinicOrder.ResumeLayout(false);
			this.groupMovePats.ResumeLayout(false);
			this.groupMovePats.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private UI.GridOD gridMain;
		private UI.Button butOK;
		private GroupBox groupMovePats;
		private UI.Button butMovePats;
		private UI.Button butClinicPick;
		private TextBox textMoveTo;
		private Label label2;
		private GroupBox groupClinicOrder;
		private UI.Button butUp;
		private UI.Button butDown;
		private UI.Button butSelectAll;
		private UI.Button butSelectNone;
		private CheckBox checkShowHidden;
		private CheckBox checkOrderAlphabetical;
	}
}
