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
			this.butSave = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(790, 12);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(367, 19);
			this.label10.TabIndex = 280;
			this.label10.Text = "To add devices to this list, log in to the mobile app via the device. ";
			// 
			// labelEClipboardNotSignedUp
			// 
			this.labelEClipboardNotSignedUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEClipboardNotSignedUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEClipboardNotSignedUp.Location = new System.Drawing.Point(214, 12);
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
			this.gridMobileAppDevices.Size = new System.Drawing.Size(1145, 425);
			this.gridMobileAppDevices.TabIndex = 277;
			this.gridMobileAppDevices.Title = "Mobile App Devices";
			this.gridMobileAppDevices.TranslationName = "Checkin Devices";
			this.gridMobileAppDevices.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMobileAppDevices_CellClick);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(1082, 470);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 276;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// FormEServicesMobileAppDeviceManage
			// 
			this.ClientSize = new System.Drawing.Size(1169, 506);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.labelEClipboardNotSignedUp);
			this.Controls.Add(this.clinicPickerEClipboard);
			this.Controls.Add(this.gridMobileAppDevices);
			this.Controls.Add(this.butSave);
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
		private UI.Button butSave;
	}
}