namespace OpenDental{
	partial class FormEServicesMobileAppDeviceManage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesMobileAppDeviceManage));
			this.label10 = new System.Windows.Forms.Label();
			this.labelEClipboardNotSignedUp = new System.Windows.Forms.Label();
			this.clinicPickerEClipboard = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridMobileAppDevices = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(578, 12);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(367, 19);
			this.label10.TabIndex = 280;
			this.label10.Text = "To add devices to this list, log in to the mobile app via the device. ";
			// 
			// labelEClipboardNotSignedUp
			// 
			this.labelEClipboardNotSignedUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEClipboardNotSignedUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEClipboardNotSignedUp.Location = new System.Drawing.Point(218, 12);
			this.labelEClipboardNotSignedUp.Name = "labelEClipboardNotSignedUp";
			this.labelEClipboardNotSignedUp.Size = new System.Drawing.Size(374, 19);
			this.labelEClipboardNotSignedUp.TabIndex = 279;
			this.labelEClipboardNotSignedUp.Text = "Go to the Signup Portal to enable eClipboard";
			this.labelEClipboardNotSignedUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// clinicPickerEClipboard
			// 
			this.clinicPickerEClipboard.HqDescription = "Default";
			this.clinicPickerEClipboard.IncludeUnassigned = true;
			this.clinicPickerEClipboard.Location = new System.Drawing.Point(12, 12);
			this.clinicPickerEClipboard.Name = "clinicPickerEClipboard";
			this.clinicPickerEClipboard.Size = new System.Drawing.Size(200, 21);
			this.clinicPickerEClipboard.TabIndex = 278;
			this.clinicPickerEClipboard.SelectionChangeCommitted += new System.EventHandler(this.clinicPickerEClipboard_SelectionChangeCommitted);
			// 
			// gridMobileAppDevices
			// 
			this.gridMobileAppDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMobileAppDevices.Location = new System.Drawing.Point(12, 39);
			this.gridMobileAppDevices.Name = "gridMobileAppDevices";
			this.gridMobileAppDevices.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMobileAppDevices.Size = new System.Drawing.Size(933, 425);
			this.gridMobileAppDevices.TabIndex = 277;
			this.gridMobileAppDevices.Title = "Mobile App Devices";
			this.gridMobileAppDevices.TranslationName = "Checkin Devices";
			this.gridMobileAppDevices.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMobileAppDevices_CellClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(886, 470);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 276;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormEServicesMobileAppDeviceManage
			// 
			this.ClientSize = new System.Drawing.Size(973, 506);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.labelEClipboardNotSignedUp);
			this.Controls.Add(this.clinicPickerEClipboard);
			this.Controls.Add(this.gridMobileAppDevices);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesMobileAppDeviceManage";
			this.Text = "Device Manager";
			this.Load += new System.EventHandler(this.FormEServicesMobileAppDeviceManage_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelEClipboardNotSignedUp;
		private UI.ComboBoxClinicPicker clinicPickerEClipboard;
		private UI.GridOD gridMobileAppDevices;
		private UI.Button butOK;
	}
}