using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEmployeeSelect {
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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmployeeSelect));
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.checkFurloughed = new System.Windows.Forms.CheckBox();
			this.checkNonFurloughed = new System.Windows.Forms.CheckBox();
			this.checkWorkingOffice = new System.Windows.Forms.CheckBox();
			this.checkWorkingHome = new System.Windows.Forms.CheckBox();
			this.butExport = new OpenDental.UI.Button();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(846, 589);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 16;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 589);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(78, 26);
			this.butAdd.TabIndex = 21;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 51);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(909, 523);
			this.gridMain.TabIndex = 22;
			this.gridMain.TranslationName = "FormEmployees";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(261, 589);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 29);
			this.label1.TabIndex = 24;
			this.label1.Text = "Delete all unused employees";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(158, 589);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(97, 26);
			this.butDelete.TabIndex = 17;
			this.butDelete.Text = "Delete All";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkFurloughed
			// 
			this.checkFurloughed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkFurloughed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFurloughed.Checked = true;
			this.checkFurloughed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkFurloughed.Location = new System.Drawing.Point(675, 7);
			this.checkFurloughed.Name = "checkFurloughed";
			this.checkFurloughed.Size = new System.Drawing.Size(104, 18);
			this.checkFurloughed.TabIndex = 25;
			this.checkFurloughed.Text = "Furloughed";
			this.checkFurloughed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFurloughed.UseVisualStyleBackColor = true;
			this.checkFurloughed.CheckedChanged += new System.EventHandler(this.checkFurloughed_CheckedChanged);
			// 
			// checkNonFurloughed
			// 
			this.checkNonFurloughed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkNonFurloughed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNonFurloughed.Checked = true;
			this.checkNonFurloughed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkNonFurloughed.Location = new System.Drawing.Point(675, 26);
			this.checkNonFurloughed.Name = "checkNonFurloughed";
			this.checkNonFurloughed.Size = new System.Drawing.Size(104, 18);
			this.checkNonFurloughed.TabIndex = 26;
			this.checkNonFurloughed.Text = "Non-Furloughed";
			this.checkNonFurloughed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNonFurloughed.UseVisualStyleBackColor = true;
			this.checkNonFurloughed.CheckedChanged += new System.EventHandler(this.checkNonFurloughed_CheckedChanged);
			// 
			// checkWorkingOffice
			// 
			this.checkWorkingOffice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkWorkingOffice.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWorkingOffice.Checked = true;
			this.checkWorkingOffice.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkWorkingOffice.Location = new System.Drawing.Point(793, 26);
			this.checkWorkingOffice.Name = "checkWorkingOffice";
			this.checkWorkingOffice.Size = new System.Drawing.Size(128, 18);
			this.checkWorkingOffice.TabIndex = 28;
			this.checkWorkingOffice.Text = "Working Office";
			this.checkWorkingOffice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWorkingOffice.UseVisualStyleBackColor = true;
			this.checkWorkingOffice.CheckedChanged += new System.EventHandler(this.checkWorkingOffice_CheckedChanged);
			// 
			// checkWorkingHome
			// 
			this.checkWorkingHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkWorkingHome.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWorkingHome.Checked = true;
			this.checkWorkingHome.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkWorkingHome.Location = new System.Drawing.Point(793, 7);
			this.checkWorkingHome.Name = "checkWorkingHome";
			this.checkWorkingHome.Size = new System.Drawing.Size(128, 18);
			this.checkWorkingHome.TabIndex = 27;
			this.checkWorkingHome.Text = "Working From Home";
			this.checkWorkingHome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWorkingHome.UseVisualStyleBackColor = true;
			this.checkWorkingHome.CheckedChanged += new System.EventHandler(this.checkWorkingHome_CheckedChanged);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(553, 589);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(83, 26);
			this.butExport.TabIndex = 29;
			this.butExport.Text = "Export List";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(535, 7);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(104, 18);
			this.checkHidden.TabIndex = 30;
			this.checkHidden.Text = "Hidden";
			this.checkHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.UseVisualStyleBackColor = true;
			this.checkHidden.CheckedChanged += new System.EventHandler(this.checkHidden_CheckedChanged);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(69, 15);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(179, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textSearch_KeyUp);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 18);
			this.label2.TabIndex = 32;
			this.label2.Text = "Search";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEmployeeSelect
			// 
			this.AcceptButton = this.butClose;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(933, 627);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.checkWorkingOffice);
			this.Controls.Add(this.checkWorkingHome);
			this.Controls.Add(this.checkNonFurloughed);
			this.Controls.Add(this.checkFurloughed);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEmployeeSelect";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Employees";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormEmployee_Closing);
			this.Load += new System.EventHandler(this.FormEmployeeSelect_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butDelete;
		private Label label1;
		private CheckBox checkFurloughed;
		private CheckBox checkNonFurloughed;
		private CheckBox checkWorkingOffice;
		private CheckBox checkWorkingHome;
		private UI.Button butExport;
		private CheckBox checkHidden;
		private TextBox textSearch;
		private Label label2;
	}
}
