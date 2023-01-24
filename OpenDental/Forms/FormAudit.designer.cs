using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAudit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAudit));
			this.textDateEditedFrom = new OpenDental.ValidDate();
			this.textDateEditedTo = new OpenDental.ValidDate();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textRows = new OpenDental.ValidNum();
			this.label6 = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.butCurrent = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboUser = new OpenDental.UI.ComboBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPermission = new OpenDental.UI.ComboBoxOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.grid = new OpenDental.UI.GridOD();
			this.comboLogSource = new OpenDental.UI.ComboBoxOD();
			this.labelLogSource = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textDateEditedFrom
			// 
			this.textDateEditedFrom.Location = new System.Drawing.Point(933, 18);
			this.textDateEditedFrom.Name = "textDateEditedFrom";
			this.textDateEditedFrom.Size = new System.Drawing.Size(80, 20);
			this.textDateEditedFrom.TabIndex = 276;
			// 
			// textDateEditedTo
			// 
			this.textDateEditedTo.Location = new System.Drawing.Point(933, 41);
			this.textDateEditedTo.Name = "textDateEditedTo";
			this.textDateEditedTo.Size = new System.Drawing.Size(80, 20);
			this.textDateEditedTo.TabIndex = 272;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(813, 19);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(118, 19);
			this.label7.TabIndex = 273;
			this.label7.Text = "Previous From Date";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(867, 44);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 13);
			this.label8.TabIndex = 274;
			this.label8.Text = "To Date";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRows
			// 
			this.textRows.Location = new System.Drawing.Point(771, 41);
			this.textRows.MaxLength = 5;
			this.textRows.MaxVal = 99999;
			this.textRows.MinVal = 1;
			this.textRows.Name = "textRows";
			this.textRows.Size = new System.Drawing.Size(56, 20);
			this.textRows.TabIndex = 268;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(683, 42);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(86, 17);
			this.label6.TabIndex = 61;
			this.label6.Text = "Limit Rows:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPrint
			// 
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(1019, 41);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(82, 24);
			this.butPrint.TabIndex = 60;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(472, 41);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(63, 24);
			this.butCurrent.TabIndex = 59;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(616, 41);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(63, 24);
			this.butAll.TabIndex = 58;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(544, 41);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(63, 24);
			this.butFind.TabIndex = 57;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(472, 18);
			this.textPatient.Name = "textPatient";
			this.textPatient.Size = new System.Drawing.Size(207, 20);
			this.textPatient.TabIndex = 56;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(158, 36);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 13);
			this.label5.TabIndex = 55;
			this.label5.Text = "User";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUser
			// 
			this.comboUser.Location = new System.Drawing.Point(235, 31);
			this.comboUser.Name = "comboUser";
			this.comboUser.Size = new System.Drawing.Size(170, 21);
			this.comboUser.TabIndex = 54;
			this.comboUser.SelectionChangeCommitted += new System.EventHandler(this.comboUser_SelectionChangeCommitted);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(405, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 13);
			this.label4.TabIndex = 52;
			this.label4.Text = "Patient";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(158, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 13);
			this.label1.TabIndex = 51;
			this.label1.Text = "Permission";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPermission
			// 
			this.comboPermission.Location = new System.Drawing.Point(235, 8);
			this.comboPermission.Name = "comboPermission";
			this.comboPermission.Size = new System.Drawing.Size(170, 21);
			this.comboPermission.TabIndex = 50;
			this.comboPermission.SelectionChangeCommitted += new System.EventHandler(this.comboPermission_SelectionChangeCommitted);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(1019, 14);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 49;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(77, 18);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(80, 20);
			this.textDateFrom.TabIndex = 47;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(77, 41);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(80, 20);
			this.textDateTo.TabIndex = 48;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 14);
			this.label2.TabIndex = 45;
			this.label2.Text = "From Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 46;
			this.label3.Text = "To Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// grid
			// 
			this.grid.AllowSortingByColumn = true;
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid.Location = new System.Drawing.Point(8, 81);
			this.grid.Name = "grid";
			this.grid.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.grid.Size = new System.Drawing.Size(1093, 576);
			this.grid.TabIndex = 2;
			this.grid.Title = "Audit Trail";
			this.grid.TranslationName = "TableAudit";
			this.grid.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellDoubleClick);
			// 
			// comboLogSource
			// 
			this.comboLogSource.Location = new System.Drawing.Point(235, 54);
			this.comboLogSource.Name = "comboLogSource";
			this.comboLogSource.Size = new System.Drawing.Size(170, 21);
			this.comboLogSource.TabIndex = 277;
			this.comboLogSource.SelectionChangeCommitted += new System.EventHandler(this.comboLogSource_SelectionChangeCommitted);
			// 
			// labelLogSource
			// 
			this.labelLogSource.Location = new System.Drawing.Point(158, 59);
			this.labelLogSource.Name = "labelLogSource";
			this.labelLogSource.Size = new System.Drawing.Size(75, 13);
			this.labelLogSource.TabIndex = 278;
			this.labelLogSource.Text = "LogSource";
			this.labelLogSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormAudit
			// 
			this.ClientSize = new System.Drawing.Size(1108, 661);
			this.Controls.Add(this.labelLogSource);
			this.Controls.Add(this.comboLogSource);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.textDateEditedFrom);
			this.Controls.Add(this.textDateEditedTo);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textRows);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butCurrent);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this.butFind);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboUser);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboPermission);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAudit";
			this.ShowInTaskbar = false;
			this.Text = "Audit Trail";
			this.Load += new System.EventHandler(this.FormAudit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private ValidDate textDateEditedFrom;
		private ValidDate textDateEditedTo;
		private Label label7;
		private Label label8;
		private ValidNum textRows;
		private Label label6;
		private UI.Button butPrint;
		private UI.Button butCurrent;
		private UI.Button butAll;
		private UI.Button butFind;
		private TextBox textPatient;
		private Label label5;
		private OpenDental.UI.ComboBoxOD comboUser;
		private Label label4;
		private Label label1;
		private OpenDental.UI.ComboBoxOD comboPermission;
		private UI.Button butRefresh;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private Label label2;
		private Label label3;
		private OpenDental.UI.GridOD grid;
		private UI.ComboBoxOD comboLogSource;
		private Label labelLogSource;
	}
}
