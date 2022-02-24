using System.Web.UI;
using OpenDental.User_Controls;

namespace OpenDental{
	partial class FormEServicesWebSchedPat {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesWebSchedPat));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkDoubleBooking = new System.Windows.Forms.CheckBox();
			this.label38 = new System.Windows.Forms.Label();
			this.checkAllowProvSelection = new System.Windows.Forms.CheckBox();
			this.checkNewPatForcePhoneFormatting = new System.Windows.Forms.CheckBox();
			this.labelWebSchedNewPatConfirmStatus = new System.Windows.Forms.Label();
			this.groupBox13 = new System.Windows.Forms.GroupBox();
			this.textApptMessage = new System.Windows.Forms.TextBox();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.butWSNPRestrictedToReasonsEdit = new OpenDental.UI.Button();
			this.labelWSNPRestrictedToReasons = new System.Windows.Forms.Label();
			this.listboxRestrictedToReasons = new OpenDental.UI.ListBoxOD();
			this.listboxIgnoreBlockoutTypes = new OpenDental.UI.ListBoxOD();
			this.label33 = new System.Windows.Forms.Label();
			this.butWebSchedBlockouts = new OpenDental.UI.Button();
			this.gridApptOps = new OpenDental.UI.GridOD();
			this.label42 = new System.Windows.Forms.Label();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.labelTimeSlotDesc = new System.Windows.Forms.Label();
			this.labelNoApptType = new System.Windows.Forms.Label();
			this.labelClinic = new System.Windows.Forms.Label();
			this.labelWSNPAApptType = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBoxOD();
			this.comboDefApptType = new OpenDental.UI.ComboBoxOD();
			this.labelTimeSlots = new System.Windows.Forms.Label();
			this.butWebSchedNewPatApptsToday = new OpenDental.UI.Button();
			this.gridApptTimeSlots = new OpenDental.UI.GridOD();
			this.textApptsDateStart = new OpenDental.ValidDate();
			this.gridReasons = new OpenDental.UI.GridOD();
			this.label40 = new System.Windows.Forms.Label();
			this.groupOtherSettings = new System.Windows.Forms.GroupBox();
			this.butNotify = new OpenDental.UI.Button();
			this.comboConfirmStatuses = new OpenDental.UI.ComboBoxOD();
			this.textApptSearchDays = new OpenDental.ValidNum();
			this.groupBox13.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupOtherSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(987, 461);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1068, 461);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkDoubleBooking
			// 
			this.checkDoubleBooking.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDoubleBooking.Location = new System.Drawing.Point(6, 42);
			this.checkDoubleBooking.Name = "checkDoubleBooking";
			this.checkDoubleBooking.Size = new System.Drawing.Size(216, 18);
			this.checkDoubleBooking.TabIndex = 363;
			this.checkDoubleBooking.Text = "Prevent double booking";
			// 
			// label38
			// 
			this.label38.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label38.Location = new System.Drawing.Point(128, 16);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(245, 15);
			this.label38.TabIndex = 362;
			this.label38.Text = "Double click to edit.";
			this.label38.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// checkAllowProvSelection
			// 
			this.checkAllowProvSelection.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.checkAllowProvSelection.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowProvSelection.Location = new System.Drawing.Point(6, 17);
			this.checkAllowProvSelection.Name = "checkAllowProvSelection";
			this.checkAllowProvSelection.Size = new System.Drawing.Size(216, 18);
			this.checkAllowProvSelection.TabIndex = 360;
			this.checkAllowProvSelection.Text = "Allow patients to select provider";
			// 
			// checkNewPatForcePhoneFormatting
			// 
			this.checkNewPatForcePhoneFormatting.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.checkNewPatForcePhoneFormatting.Location = new System.Drawing.Point(6, 69);
			this.checkNewPatForcePhoneFormatting.Name = "checkNewPatForcePhoneFormatting";
			this.checkNewPatForcePhoneFormatting.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkNewPatForcePhoneFormatting.Size = new System.Drawing.Size(216, 19);
			this.checkNewPatForcePhoneFormatting.TabIndex = 359;
			this.checkNewPatForcePhoneFormatting.Text = "Force US phone number format";
			this.checkNewPatForcePhoneFormatting.UseVisualStyleBackColor = true;
			this.checkNewPatForcePhoneFormatting.Click += new System.EventHandler(this.checkWebSchedNewPatForcePhoneFormatting_Click);
			// 
			// labelWebSchedNewPatConfirmStatus
			// 
			this.labelWebSchedNewPatConfirmStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelWebSchedNewPatConfirmStatus.Location = new System.Drawing.Point(317, 41);
			this.labelWebSchedNewPatConfirmStatus.Name = "labelWebSchedNewPatConfirmStatus";
			this.labelWebSchedNewPatConfirmStatus.Size = new System.Drawing.Size(108, 19);
			this.labelWebSchedNewPatConfirmStatus.TabIndex = 357;
			this.labelWebSchedNewPatConfirmStatus.Text = "Confirm Status";
			this.labelWebSchedNewPatConfirmStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox13
			// 
			this.groupBox13.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox13.Controls.Add(this.textApptMessage);
			this.groupBox13.Location = new System.Drawing.Point(401, 182);
			this.groupBox13.Name = "groupBox13";
			this.groupBox13.Size = new System.Drawing.Size(304, 165);
			this.groupBox13.TabIndex = 356;
			this.groupBox13.TabStop = false;
			this.groupBox13.Text = "Appointment Message";
			// 
			// textApptMessage
			// 
			this.textApptMessage.Location = new System.Drawing.Point(6, 17);
			this.textApptMessage.Multiline = true;
			this.textApptMessage.Name = "textApptMessage";
			this.textApptMessage.Size = new System.Drawing.Size(292, 142);
			this.textApptMessage.TabIndex = 313;
			// 
			// groupBox11
			// 
			this.groupBox11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox11.Controls.Add(this.butWSNPRestrictedToReasonsEdit);
			this.groupBox11.Controls.Add(this.labelWSNPRestrictedToReasons);
			this.groupBox11.Controls.Add(this.listboxRestrictedToReasons);
			this.groupBox11.Controls.Add(this.listboxIgnoreBlockoutTypes);
			this.groupBox11.Controls.Add(this.label33);
			this.groupBox11.Controls.Add(this.butWebSchedBlockouts);
			this.groupBox11.Location = new System.Drawing.Point(711, 182);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(428, 165);
			this.groupBox11.TabIndex = 355;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Allowed Blockout Types";
			// 
			// butWSNPRestrictedToReasonsEdit
			// 
			this.butWSNPRestrictedToReasonsEdit.Location = new System.Drawing.Point(172, 136);
			this.butWSNPRestrictedToReasonsEdit.Name = "butWSNPRestrictedToReasonsEdit";
			this.butWSNPRestrictedToReasonsEdit.Size = new System.Drawing.Size(68, 24);
			this.butWSNPRestrictedToReasonsEdit.TabIndex = 226;
			this.butWSNPRestrictedToReasonsEdit.Text = "Edit";
			this.butWSNPRestrictedToReasonsEdit.UseVisualStyleBackColor = true;
			this.butWSNPRestrictedToReasonsEdit.Click += new System.EventHandler(this.butRestrictedToReasonsEdit_Click);
			// 
			// labelWSNPRestrictedToReasons
			// 
			this.labelWSNPRestrictedToReasons.Location = new System.Drawing.Point(169, 16);
			this.labelWSNPRestrictedToReasons.Name = "labelWSNPRestrictedToReasons";
			this.labelWSNPRestrictedToReasons.Size = new System.Drawing.Size(253, 20);
			this.labelWSNPRestrictedToReasons.TabIndex = 225;
			this.labelWSNPRestrictedToReasons.Text = "Restricted to Reasons";
			this.labelWSNPRestrictedToReasons.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listboxRestrictedToReasons
			// 
			this.listboxRestrictedToReasons.Location = new System.Drawing.Point(172, 37);
			this.listboxRestrictedToReasons.Name = "listboxRestrictedToReasons";
			this.listboxRestrictedToReasons.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listboxRestrictedToReasons.Size = new System.Drawing.Size(250, 95);
			this.listboxRestrictedToReasons.TabIndex = 224;
			// 
			// listboxIgnoreBlockoutTypes
			// 
			this.listboxIgnoreBlockoutTypes.Location = new System.Drawing.Point(6, 37);
			this.listboxIgnoreBlockoutTypes.Name = "listboxIgnoreBlockoutTypes";
			this.listboxIgnoreBlockoutTypes.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listboxIgnoreBlockoutTypes.Size = new System.Drawing.Size(146, 95);
			this.listboxIgnoreBlockoutTypes.TabIndex = 197;
			// 
			// label33
			// 
			this.label33.Location = new System.Drawing.Point(6, 14);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(149, 20);
			this.label33.TabIndex = 223;
			this.label33.Text = "Generally Allowed";
			this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butWebSchedBlockouts
			// 
			this.butWebSchedBlockouts.Location = new System.Drawing.Point(6, 136);
			this.butWebSchedBlockouts.Name = "butWebSchedBlockouts";
			this.butWebSchedBlockouts.Size = new System.Drawing.Size(68, 23);
			this.butWebSchedBlockouts.TabIndex = 197;
			this.butWebSchedBlockouts.Text = "Edit";
			this.butWebSchedBlockouts.Click += new System.EventHandler(this.butBlockouts_Click);
			// 
			// gridApptOps
			// 
			this.gridApptOps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gridApptOps.Location = new System.Drawing.Point(401, 34);
			this.gridApptOps.Name = "gridApptOps";
			this.gridApptOps.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridApptOps.Size = new System.Drawing.Size(738, 142);
			this.gridApptOps.TabIndex = 354;
			this.gridApptOps.Title = "Operatories Considered";
			this.gridApptOps.TranslationName = "FormEServicesSetup";
			this.gridApptOps.WrapText = false;
			this.gridApptOps.DoubleClick += new System.EventHandler(this.gridApptOps_DoubleClick);
			// 
			// label42
			// 
			this.label42.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label42.Location = new System.Drawing.Point(864, 16);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(245, 15);
			this.label42.TabIndex = 353;
			this.label42.Text = "Double click to edit.";
			this.label42.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// groupBox7
			// 
			this.groupBox7.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox7.Controls.Add(this.labelTimeSlotDesc);
			this.groupBox7.Controls.Add(this.labelNoApptType);
			this.groupBox7.Controls.Add(this.labelClinic);
			this.groupBox7.Controls.Add(this.labelWSNPAApptType);
			this.groupBox7.Controls.Add(this.comboClinics);
			this.groupBox7.Controls.Add(this.comboDefApptType);
			this.groupBox7.Controls.Add(this.labelTimeSlots);
			this.groupBox7.Controls.Add(this.butWebSchedNewPatApptsToday);
			this.groupBox7.Controls.Add(this.gridApptTimeSlots);
			this.groupBox7.Controls.Add(this.textApptsDateStart);
			this.groupBox7.Location = new System.Drawing.Point(17, 178);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(378, 273);
			this.groupBox7.TabIndex = 352;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Available Times For Patients";
			// 
			// labelTimeSlotDesc
			// 
			this.labelTimeSlotDesc.Location = new System.Drawing.Point(189, 21);
			this.labelTimeSlotDesc.Name = "labelTimeSlotDesc";
			this.labelTimeSlotDesc.Size = new System.Drawing.Size(179, 41);
			this.labelTimeSlotDesc.TabIndex = 326;
			this.labelTimeSlotDesc.Text = "Pick a date and reason to see what times would show as available to a patient.";
			// 
			// labelNoApptType
			// 
			this.labelNoApptType.ForeColor = System.Drawing.Color.Red;
			this.labelNoApptType.Location = new System.Drawing.Point(186, 203);
			this.labelNoApptType.Name = "labelNoApptType";
			this.labelNoApptType.Size = new System.Drawing.Size(186, 45);
			this.labelNoApptType.TabIndex = 325;
			this.labelNoApptType.Text = "Could not load timeslots because no Existing Patient Appointment Type was found";
			this.labelNoApptType.Visible = false;
			// 
			// labelClinic
			// 
			this.labelClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClinic.Location = new System.Drawing.Point(189, 150);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(179, 16);
			this.labelClinic.TabIndex = 324;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWSNPAApptType
			// 
			this.labelWSNPAApptType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWSNPAApptType.Location = new System.Drawing.Point(189, 101);
			this.labelWSNPAApptType.Name = "labelWSNPAApptType";
			this.labelWSNPAApptType.Size = new System.Drawing.Size(179, 17);
			this.labelWSNPAApptType.TabIndex = 320;
			this.labelWSNPAApptType.Text = "Reason";
			// 
			// comboClinics
			// 
			this.comboClinics.Location = new System.Drawing.Point(189, 169);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(177, 21);
			this.comboClinics.TabIndex = 323;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.comboClinics_SelectionChangeCommitted);
			// 
			// comboDefApptType
			// 
			this.comboDefApptType.Location = new System.Drawing.Point(189, 121);
			this.comboDefApptType.Name = "comboDefApptType";
			this.comboDefApptType.Size = new System.Drawing.Size(177, 21);
			this.comboDefApptType.TabIndex = 319;
			this.comboDefApptType.SelectionChangeCommitted += new System.EventHandler(this.comboApptType_SelectionChangeCommitted);
			// 
			// labelTimeSlots
			// 
			this.labelTimeSlots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTimeSlots.Location = new System.Drawing.Point(3, 251);
			this.labelTimeSlots.Name = "labelTimeSlots";
			this.labelTimeSlots.Size = new System.Drawing.Size(354, 18);
			this.labelTimeSlots.TabIndex = 318;
			this.labelTimeSlots.Text = "Select a clinic to view its available time slots.";
			// 
			// butWebSchedNewPatApptsToday
			// 
			this.butWebSchedNewPatApptsToday.Location = new System.Drawing.Point(278, 72);
			this.butWebSchedNewPatApptsToday.Name = "butWebSchedNewPatApptsToday";
			this.butWebSchedNewPatApptsToday.Size = new System.Drawing.Size(79, 21);
			this.butWebSchedNewPatApptsToday.TabIndex = 309;
			this.butWebSchedNewPatApptsToday.Text = "Today";
			this.butWebSchedNewPatApptsToday.Click += new System.EventHandler(this.butApptsToday_Click);
			// 
			// gridApptTimeSlots
			// 
			this.gridApptTimeSlots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridApptTimeSlots.Location = new System.Drawing.Point(6, 20);
			this.gridApptTimeSlots.Name = "gridApptTimeSlots";
			this.gridApptTimeSlots.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridApptTimeSlots.Size = new System.Drawing.Size(174, 226);
			this.gridApptTimeSlots.TabIndex = 302;
			this.gridApptTimeSlots.Title = "Time Slots";
			this.gridApptTimeSlots.TranslationName = "FormEServicesSetup";
			this.gridApptTimeSlots.WrapText = false;
			// 
			// textApptsDateStart
			// 
			this.textApptsDateStart.Location = new System.Drawing.Point(189, 73);
			this.textApptsDateStart.Name = "textApptsDateStart";
			this.textApptsDateStart.Size = new System.Drawing.Size(79, 20);
			this.textApptsDateStart.TabIndex = 303;
			this.textApptsDateStart.Text = "07/08/2015";
			this.textApptsDateStart.TextChanged += new System.EventHandler(this.textApptsDateStart_TextChanged);
			// 
			// gridReasons
			// 
			this.gridReasons.AllowSelection = false;
			this.gridReasons.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.gridReasons.Location = new System.Drawing.Point(17, 34);
			this.gridReasons.Name = "gridReasons";
			this.gridReasons.Size = new System.Drawing.Size(378, 142);
			this.gridReasons.TabIndex = 351;
			this.gridReasons.Title = "Appointment Types";
			this.gridReasons.TranslationName = "FormEServicesWebSchedPat";
			this.gridReasons.DoubleClick += new System.EventHandler(this.gridReasons_DoubleClick);
			// 
			// label40
			// 
			this.label40.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label40.Location = new System.Drawing.Point(216, 14);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(460, 17);
			this.label40.TabIndex = 348;
			this.label40.Text = "Minimum number of days out an appointment can be scheduled (empty is 0)";
			this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupOtherSettings
			// 
			this.groupOtherSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupOtherSettings.Controls.Add(this.checkAllowProvSelection);
			this.groupOtherSettings.Controls.Add(this.butNotify);
			this.groupOtherSettings.Controls.Add(this.checkDoubleBooking);
			this.groupOtherSettings.Controls.Add(this.checkNewPatForcePhoneFormatting);
			this.groupOtherSettings.Controls.Add(this.comboConfirmStatuses);
			this.groupOtherSettings.Controls.Add(this.labelWebSchedNewPatConfirmStatus);
			this.groupOtherSettings.Controls.Add(this.label40);
			this.groupOtherSettings.Controls.Add(this.textApptSearchDays);
			this.groupOtherSettings.Location = new System.Drawing.Point(401, 351);
			this.groupOtherSettings.Name = "groupOtherSettings";
			this.groupOtherSettings.Size = new System.Drawing.Size(738, 100);
			this.groupOtherSettings.TabIndex = 365;
			this.groupOtherSettings.TabStop = false;
			this.groupOtherSettings.Text = "Other Settings";
			// 
			// butNotify
			// 
			this.butNotify.Location = new System.Drawing.Point(611, 69);
			this.butNotify.Name = "butNotify";
			this.butNotify.Size = new System.Drawing.Size(109, 24);
			this.butNotify.TabIndex = 364;
			this.butNotify.Text = "Notification Settings";
			this.butNotify.UseVisualStyleBackColor = true;
			this.butNotify.Click += new System.EventHandler(this.butNotify_Click);
			// 
			// comboConfirmStatuses
			// 
			this.comboConfirmStatuses.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.comboConfirmStatuses.Location = new System.Drawing.Point(428, 41);
			this.comboConfirmStatuses.Name = "comboConfirmStatuses";
			this.comboConfirmStatuses.Size = new System.Drawing.Size(292, 21);
			this.comboConfirmStatuses.TabIndex = 358;
			// 
			// textApptSearchDays
			// 
			this.textApptSearchDays.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textApptSearchDays.Location = new System.Drawing.Point(682, 13);
			this.textApptSearchDays.MaxVal = 365;
			this.textApptSearchDays.Name = "textApptSearchDays";
			this.textApptSearchDays.ShowZero = false;
			this.textApptSearchDays.Size = new System.Drawing.Size(38, 20);
			this.textApptSearchDays.TabIndex = 349;
			this.textApptSearchDays.Leave += new System.EventHandler(this.textApptSearchDays_Leave);
			this.textApptSearchDays.Validated += new System.EventHandler(this.textApptSearchDays_Validated);
			// 
			// FormEServicesWebSchedPat
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1155, 497);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupOtherSettings);
			this.Controls.Add(this.label38);
			this.Controls.Add(this.groupBox13);
			this.Controls.Add(this.groupBox11);
			this.Controls.Add(this.gridApptOps);
			this.Controls.Add(this.label42);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.gridReasons);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesWebSchedPat";
			this.Text = "Web Sched - Patient";
			this.Load += new System.EventHandler(this.FormEServicesWebSchedPat_Load);
			this.groupBox13.ResumeLayout(false);
			this.groupBox13.PerformLayout();
			this.groupBox11.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupOtherSettings.ResumeLayout(false);
			this.groupOtherSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkDoubleBooking;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.CheckBox checkAllowProvSelection;
		private System.Windows.Forms.CheckBox checkNewPatForcePhoneFormatting;
		private UI.ComboBoxOD comboConfirmStatuses;
		private System.Windows.Forms.Label labelWebSchedNewPatConfirmStatus;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.TextBox textApptMessage;
		private System.Windows.Forms.GroupBox groupBox11;
		private UI.Button butWSNPRestrictedToReasonsEdit;
		private System.Windows.Forms.Label labelWSNPRestrictedToReasons;
		private UI.ListBoxOD listboxRestrictedToReasons;
		private UI.ListBoxOD listboxIgnoreBlockoutTypes;
		private System.Windows.Forms.Label label33;
		private UI.Button butWebSchedBlockouts;
		private UI.GridOD gridApptOps;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.Label labelWSNPAApptType;
		private UI.ComboBoxOD comboClinics;
		private UI.ComboBoxOD comboDefApptType;
		private System.Windows.Forms.Label labelTimeSlots;
		private UI.Button butWebSchedNewPatApptsToday;
		private UI.GridOD gridApptTimeSlots;
		private ValidDate textApptsDateStart;
		private UI.GridOD gridReasons;
		private System.Windows.Forms.Label label40;
		private ValidNum textApptSearchDays;
		private UI.Button butNotify;
		private System.Windows.Forms.Label labelNoApptType;
		private System.Windows.Forms.GroupBox groupOtherSettings;
		private System.Windows.Forms.Label labelTimeSlotDesc;
	}
}