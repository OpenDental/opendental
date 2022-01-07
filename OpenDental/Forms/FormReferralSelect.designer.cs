using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReferralSelect {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReferralSelect));
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelResultCount = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkPreferred = new System.Windows.Forms.CheckBox();
			this.checkShowOther = new System.Windows.Forms.CheckBox();
			this.checkShowDoctor = new System.Windows.Forms.CheckBox();
			this.checkShowPat = new System.Windows.Forms.CheckBox();
			this.comboClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkHidden
			// 
			this.checkHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(364, 13);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(75, 16);
			this.checkHidden.TabIndex = 11;
			this.checkHidden.Text = "Hidden";
			this.checkHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Click += new System.EventHandler(this.checkHidden_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(8, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1007, 610);
			this.gridMain.TabIndex = 15;
			this.gridMain.Title = "Select Referral";
			this.gridMain.TranslationName = "TableSelectReferral";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(56, 14);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(201, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 14);
			this.label1.TabIndex = 17;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelResultCount
			// 
			this.labelResultCount.Location = new System.Drawing.Point(258, 17);
			this.labelResultCount.Name = "labelResultCount";
			this.labelResultCount.Size = new System.Drawing.Size(108, 14);
			this.labelResultCount.TabIndex = 18;
			this.labelResultCount.Text = "# results found";
			this.labelResultCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(8, 661);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 12;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.butCancel.Location = new System.Drawing.Point(940, 661);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(852, 661);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkPreferred);
			this.groupBox1.Controls.Add(this.checkShowOther);
			this.groupBox1.Controls.Add(this.checkShowDoctor);
			this.groupBox1.Controls.Add(this.checkShowPat);
			this.groupBox1.Controls.Add(this.checkHidden);
			this.groupBox1.Location = new System.Drawing.Point(349, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(450, 33);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show";
			// 
			// checkPreferred
			// 
			this.checkPreferred.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferred.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPreferred.Location = new System.Drawing.Point(261, 13);
			this.checkPreferred.Name = "checkPreferred";
			this.checkPreferred.Size = new System.Drawing.Size(94, 16);
			this.checkPreferred.TabIndex = 23;
			this.checkPreferred.Text = "Preferred Only";
			this.checkPreferred.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferred.Click += new System.EventHandler(this.checkPreferred_Click);
			// 
			// checkShowOther
			// 
			this.checkShowOther.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOther.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowOther.Location = new System.Drawing.Point(177, 13);
			this.checkShowOther.Name = "checkShowOther";
			this.checkShowOther.Size = new System.Drawing.Size(75, 16);
			this.checkShowOther.TabIndex = 20;
			this.checkShowOther.Text = "Other";
			this.checkShowOther.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOther.Click += new System.EventHandler(this.checkShowOther_Click);
			// 
			// checkShowDoctor
			// 
			this.checkShowDoctor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDoctor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowDoctor.Location = new System.Drawing.Point(89, 13);
			this.checkShowDoctor.Name = "checkShowDoctor";
			this.checkShowDoctor.Size = new System.Drawing.Size(84, 16);
			this.checkShowDoctor.TabIndex = 21;
			this.checkShowDoctor.Text = "Doctor";
			this.checkShowDoctor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDoctor.Click += new System.EventHandler(this.checkShowDoctor_Click);
			// 
			// checkShowPat
			// 
			this.checkShowPat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowPat.Location = new System.Drawing.Point(3, 13);
			this.checkShowPat.Name = "checkShowPat";
			this.checkShowPat.Size = new System.Drawing.Size(84, 16);
			this.checkShowPat.TabIndex = 22;
			this.checkShowPat.Text = "Patient";
			this.checkShowPat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPat.Click += new System.EventHandler(this.checkShowPat_Click);
			// 
			// comboClinicPicker
			// 
			this.comboClinicPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinicPicker.IncludeAll = true;
			this.comboClinicPicker.IncludeUnassigned = true;
			this.comboClinicPicker.Location = new System.Drawing.Point(815, 13);
			this.comboClinicPicker.Name = "comboClinicPicker";
			this.comboClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboClinicPicker.TabIndex = 20;
			this.comboClinicPicker.SelectionChangeCommitted += new System.EventHandler(this.comboClinicPicker_SelectionChangeCommitted);
			// 
			// FormReferralSelect
			// 
			this.ClientSize = new System.Drawing.Size(1029, 696);
			this.Controls.Add(this.comboClinicPicker);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelResultCount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReferralSelect";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Referrals";
			this.Load += new System.EventHandler(this.FormReferralSelect_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkHidden;
		private OpenDental.UI.Button butAdd;
		private UI.GridOD gridMain;
		private TextBox textSearch;
		private Label label1;
		private Label labelResultCount;
		private GroupBox groupBox1;
		private CheckBox checkShowOther;
		private CheckBox checkShowDoctor;
		private CheckBox checkShowPat;
		private CheckBox checkPreferred;
		private UI.ComboBoxClinicPicker comboClinicPicker;
	}
}
