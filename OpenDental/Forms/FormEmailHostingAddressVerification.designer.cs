namespace OpenDental {
	partial class FormEmailHostingAddressVerification {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailHostingAddressVerification));
			this.label2 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.comboBoxClinicPicker1 = new OpenDental.UI.ComboBoxClinicPicker();
			this.butRefresh = new OpenDental.UI.Button();
			this.labelNotActivated = new System.Windows.Forms.Label();
			this.checkUseNoReply = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(506, 17);
			this.label2.TabIndex = 13;
			this.label2.Text = "Verified email addresses may be used to send Mass Email or Secure Email notificat" +
    "ions.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(19, 33);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(871, 437);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Email Addresses";
			this.gridMain.TranslationName = "TableEmailAddresses";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(734, 478);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 3;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(815, 478);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// comboBoxClinicPicker1
			// 
			this.comboBoxClinicPicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxClinicPicker1.HqDescription = "Headquarters";
			this.comboBoxClinicPicker1.IncludeUnassigned = true;
			this.comboBoxClinicPicker1.Location = new System.Drawing.Point(609, 5);
			this.comboBoxClinicPicker1.Name = "comboBoxClinicPicker1";
			this.comboBoxClinicPicker1.Size = new System.Drawing.Size(200, 21);
			this.comboBoxClinicPicker1.TabIndex = 15;
			this.comboBoxClinicPicker1.SelectionChangeCommitted += new System.EventHandler(this.comboBoxClinicPicker1_SelectionChangeCommitted);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(815, 3);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 16;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// labelNotActivated
			// 
			this.labelNotActivated.ForeColor = System.Drawing.Color.Red;
			this.labelNotActivated.Location = new System.Drawing.Point(443, 7);
			this.labelNotActivated.Name = "labelNotActivated";
			this.labelNotActivated.Size = new System.Drawing.Size(163, 17);
			this.labelNotActivated.TabIndex = 17;
			this.labelNotActivated.Text = "* Clinic is not signed up";
			this.labelNotActivated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUseNoReply
			// 
			this.checkUseNoReply.Location = new System.Drawing.Point(19, 478);
			this.checkUseNoReply.Name = "checkUseNoReply";
			this.checkUseNoReply.Size = new System.Drawing.Size(287, 24);
			this.checkUseNoReply.TabIndex = 18;
			this.checkUseNoReply.Text = "Use \'NoReply\' as default email address";
			this.checkUseNoReply.UseVisualStyleBackColor = true;
			this.checkUseNoReply.Click += new System.EventHandler(this.checkUseNoReply_Click);
			// 
			// FormEmailHostingAddressVerification
			// 
			this.ClientSize = new System.Drawing.Size(910, 510);
			this.Controls.Add(this.checkUseNoReply);
			this.Controls.Add(this.labelNotActivated);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.comboBoxClinicPicker1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailHostingAddressVerification";
			this.Text = "Hosted Email Address Verification";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEmailHostingAddressVerification_FormClosing);
			this.Load += new System.EventHandler(this.FormEmailHostingAddressVerification_Load);
			this.Shown += new System.EventHandler(this.FormEmailAddressVerification_Shown);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label2;
		private UI.Button butDelete;
		private UI.ComboBoxClinicPicker comboBoxClinicPicker1;
		private UI.Button butRefresh;
		private System.Windows.Forms.Label labelNotActivated;
		private System.Windows.Forms.CheckBox checkUseNoReply;
	}
}