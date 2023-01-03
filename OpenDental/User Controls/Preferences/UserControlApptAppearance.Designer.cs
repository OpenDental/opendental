
namespace OpenDental {
	partial class UserControlApptAppearance {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelApptLineColorDetails = new System.Windows.Forms.Label();
			this.labelApptProvbarWidthDetails = new System.Windows.Forms.Label();
			this.labelApptClickDelayDetails = new System.Windows.Forms.Label();
			this.labelApptSchedEnforceSpecialtyDetails = new System.Windows.Forms.Label();
			this.labelApptFontSizeDetails = new System.Windows.Forms.Label();
			this.labelApptBubNoteLengthDetails = new System.Windows.Forms.Label();
			this.labelWaitRoomWarnDetails = new System.Windows.Forms.Label();
			this.butUseOpHygProvDetails = new OpenDental.UI.Button();
			this.butTimeArrivedTriggerDetails = new OpenDental.UI.Button();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.labelApptSchedEnforceSpecialty = new System.Windows.Forms.Label();
			this.checkApptExclamation = new OpenDental.UI.CheckBox();
			this.comboApptSchedEnforceSpecialty = new OpenDental.UI.ComboBox();
			this.checkApptModuleDefaultToWeek = new OpenDental.UI.CheckBox();
			this.checkUseOpHygProv = new OpenDental.UI.CheckBox();
			this.checkApptRefreshEveryMinute = new OpenDental.UI.CheckBox();
			this.comboDelay = new OpenDental.UI.ComboBox();
			this.apptClickDelay = new System.Windows.Forms.Label();
			this.label45 = new System.Windows.Forms.Label();
			this.comboWeekViewStartDay = new OpenDental.UI.ComboBox();
			this.groupBoxBlockouts = new OpenDental.UI.GroupBox();
			this.checkReplaceBlockouts = new OpenDental.UI.CheckBox();
			this.checkSolidBlockouts = new OpenDental.UI.CheckBox();
			this.groupBoxApptBubble = new OpenDental.UI.GroupBox();
			this.textApptBubNoteLength = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.checkApptBubbleDelay = new OpenDental.UI.CheckBox();
			this.checkAppointmentBubblesDisabled = new OpenDental.UI.CheckBox();
			this.groupBoxAppearance = new OpenDental.UI.GroupBox();
			this.label58 = new System.Windows.Forms.Label();
			this.label46 = new System.Windows.Forms.Label();
			this.textApptProvbarWidth = new OpenDental.ValidNum();
			this.label54 = new System.Windows.Forms.Label();
			this.textApptFontSize = new System.Windows.Forms.TextBox();
			this.butApptLineColor = new System.Windows.Forms.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.groupBoxWaitingRoom = new OpenDental.UI.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboTimeArrived = new OpenDental.UI.ComboBox();
			this.comboTimeSeated = new OpenDental.UI.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboTimeDismissed = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textWaitRoomWarn = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.checkWaitingRoomFilterByView = new OpenDental.UI.CheckBox();
			this.butColor = new System.Windows.Forms.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.labelApptRefreshEveryMinuteDetails = new System.Windows.Forms.Label();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxBlockouts.SuspendLayout();
			this.groupBoxApptBubble.SuspendLayout();
			this.groupBoxAppearance.SuspendLayout();
			this.groupBoxWaitingRoom.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelApptLineColorDetails
			// 
			this.labelApptLineColorDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptLineColorDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptLineColorDetails.Location = new System.Drawing.Point(476, 21);
			this.labelApptLineColorDetails.Name = "labelApptLineColorDetails";
			this.labelApptLineColorDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptLineColorDetails.TabIndex = 339;
			this.labelApptLineColorDetails.Text = "horizontal line that indicates current time";
			this.labelApptLineColorDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelApptProvbarWidthDetails
			// 
			this.labelApptProvbarWidthDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptProvbarWidthDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptProvbarWidthDetails.Location = new System.Drawing.Point(476, 66);
			this.labelApptProvbarWidthDetails.Name = "labelApptProvbarWidthDetails";
			this.labelApptProvbarWidthDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptProvbarWidthDetails.TabIndex = 340;
			this.labelApptProvbarWidthDetails.Text = "in pixels; enter 0 to remove; does not affect bars to the left of the operatories" +
    "";
			this.labelApptProvbarWidthDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelApptClickDelayDetails
			// 
			this.labelApptClickDelayDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptClickDelayDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptClickDelayDetails.Location = new System.Drawing.Point(476, 480);
			this.labelApptClickDelayDetails.Name = "labelApptClickDelayDetails";
			this.labelApptClickDelayDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptClickDelayDetails.TabIndex = 343;
			this.labelApptClickDelayDetails.Text = "or a triple click could accidentally cause a procedure to be added";
			this.labelApptClickDelayDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelApptSchedEnforceSpecialtyDetails
			// 
			this.labelApptSchedEnforceSpecialtyDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptSchedEnforceSpecialtyDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptSchedEnforceSpecialtyDetails.Location = new System.Drawing.Point(476, 419);
			this.labelApptSchedEnforceSpecialtyDetails.Name = "labelApptSchedEnforceSpecialtyDetails";
			this.labelApptSchedEnforceSpecialtyDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptSchedEnforceSpecialtyDetails.TabIndex = 344;
			this.labelApptSchedEnforceSpecialtyDetails.Text = "when the patient’s specialty does not match the clinic’s";
			this.labelApptSchedEnforceSpecialtyDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelApptFontSizeDetails
			// 
			this.labelApptFontSizeDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptFontSizeDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptFontSizeDetails.Location = new System.Drawing.Point(476, 44);
			this.labelApptFontSizeDetails.Name = "labelApptFontSizeDetails";
			this.labelApptFontSizeDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptFontSizeDetails.TabIndex = 368;
			this.labelApptFontSizeDetails.Text = "default is 8; decimals allowed; in addition to Zoom";
			this.labelApptFontSizeDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelApptBubNoteLengthDetails
			// 
			this.labelApptBubNoteLengthDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptBubNoteLengthDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptBubNoteLengthDetails.Location = new System.Drawing.Point(476, 122);
			this.labelApptBubNoteLengthDetails.Name = "labelApptBubNoteLengthDetails";
			this.labelApptBubNoteLengthDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptBubNoteLengthDetails.TabIndex = 369;
			this.labelApptBubNoteLengthDetails.Text = "in characters, 0 for no limit";
			this.labelApptBubNoteLengthDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWaitRoomWarnDetails
			// 
			this.labelWaitRoomWarnDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWaitRoomWarnDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelWaitRoomWarnDetails.Location = new System.Drawing.Point(476, 338);
			this.labelWaitRoomWarnDetails.Name = "labelWaitRoomWarnDetails";
			this.labelWaitRoomWarnDetails.Size = new System.Drawing.Size(498, 17);
			this.labelWaitRoomWarnDetails.TabIndex = 370;
			this.labelWaitRoomWarnDetails.Text = "in minutes, 0 to disable";
			this.labelWaitRoomWarnDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butUseOpHygProvDetails
			// 
			this.butUseOpHygProvDetails.ForeColor = System.Drawing.Color.Black;
			this.butUseOpHygProvDetails.Location = new System.Drawing.Point(479, 393);
			this.butUseOpHygProvDetails.Name = "butUseOpHygProvDetails";
			this.butUseOpHygProvDetails.Size = new System.Drawing.Size(64, 21);
			this.butUseOpHygProvDetails.TabIndex = 371;
			this.butUseOpHygProvDetails.Text = "Details";
			this.butUseOpHygProvDetails.Click += new System.EventHandler(this.butUseOpHygProvDetails_Click);
			// 
			// butTimeArrivedTriggerDetails
			// 
			this.butTimeArrivedTriggerDetails.ForeColor = System.Drawing.Color.Black;
			this.butTimeArrivedTriggerDetails.Location = new System.Drawing.Point(479, 248);
			this.butTimeArrivedTriggerDetails.Name = "butTimeArrivedTriggerDetails";
			this.butTimeArrivedTriggerDetails.Size = new System.Drawing.Size(64, 21);
			this.butTimeArrivedTriggerDetails.TabIndex = 367;
			this.butTimeArrivedTriggerDetails.Text = "Details";
			this.butTimeArrivedTriggerDetails.Click += new System.EventHandler(this.butTimeArrivedTriggerDetails_Click);
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.labelApptSchedEnforceSpecialty);
			this.groupBoxOD1.Controls.Add(this.checkApptExclamation);
			this.groupBoxOD1.Controls.Add(this.comboApptSchedEnforceSpecialty);
			this.groupBoxOD1.Controls.Add(this.checkApptModuleDefaultToWeek);
			this.groupBoxOD1.Controls.Add(this.checkUseOpHygProv);
			this.groupBoxOD1.Controls.Add(this.checkApptRefreshEveryMinute);
			this.groupBoxOD1.Controls.Add(this.comboDelay);
			this.groupBoxOD1.Controls.Add(this.apptClickDelay);
			this.groupBoxOD1.Controls.Add(this.label45);
			this.groupBoxOD1.Controls.Add(this.comboWeekViewStartDay);
			this.groupBoxOD1.Location = new System.Drawing.Point(20, 387);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(450, 158);
			this.groupBoxOD1.TabIndex = 330;
			this.groupBoxOD1.Text = "Functionality";
			// 
			// labelApptSchedEnforceSpecialty
			// 
			this.labelApptSchedEnforceSpecialty.Location = new System.Drawing.Point(27, 32);
			this.labelApptSchedEnforceSpecialty.Name = "labelApptSchedEnforceSpecialty";
			this.labelApptSchedEnforceSpecialty.Size = new System.Drawing.Size(247, 17);
			this.labelApptSchedEnforceSpecialty.TabIndex = 333;
			this.labelApptSchedEnforceSpecialty.Text = "Enforce clinic specialties";
			this.labelApptSchedEnforceSpecialty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptExclamation
			// 
			this.checkApptExclamation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptExclamation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptExclamation.Location = new System.Drawing.Point(15, 52);
			this.checkApptExclamation.Name = "checkApptExclamation";
			this.checkApptExclamation.Size = new System.Drawing.Size(425, 17);
			this.checkApptExclamation.TabIndex = 222;
			this.checkApptExclamation.Text = "Show ! on appts for ins not sent, if added to Appt View (might cause slowdown)";
			// 
			// comboApptSchedEnforceSpecialty
			// 
			this.comboApptSchedEnforceSpecialty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboApptSchedEnforceSpecialty.Location = new System.Drawing.Point(277, 29);
			this.comboApptSchedEnforceSpecialty.Name = "comboApptSchedEnforceSpecialty";
			this.comboApptSchedEnforceSpecialty.Size = new System.Drawing.Size(163, 21);
			this.comboApptSchedEnforceSpecialty.TabIndex = 332;
			// 
			// checkApptModuleDefaultToWeek
			// 
			this.checkApptModuleDefaultToWeek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptModuleDefaultToWeek.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleDefaultToWeek.Location = new System.Drawing.Point(15, 71);
			this.checkApptModuleDefaultToWeek.Name = "checkApptModuleDefaultToWeek";
			this.checkApptModuleDefaultToWeek.Size = new System.Drawing.Size(425, 17);
			this.checkApptModuleDefaultToWeek.TabIndex = 220;
			this.checkApptModuleDefaultToWeek.Text = "Appointments Module defaults to week view";
			// 
			// checkUseOpHygProv
			// 
			this.checkUseOpHygProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUseOpHygProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseOpHygProv.Location = new System.Drawing.Point(90, 10);
			this.checkUseOpHygProv.Name = "checkUseOpHygProv";
			this.checkUseOpHygProv.Size = new System.Drawing.Size(350, 17);
			this.checkUseOpHygProv.TabIndex = 331;
			this.checkUseOpHygProv.Text = "Force op\'s hygiene provider as secondary provider";
			// 
			// checkApptRefreshEveryMinute
			// 
			this.checkApptRefreshEveryMinute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptRefreshEveryMinute.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptRefreshEveryMinute.Location = new System.Drawing.Point(34, 113);
			this.checkApptRefreshEveryMinute.Name = "checkApptRefreshEveryMinute";
			this.checkApptRefreshEveryMinute.Size = new System.Drawing.Size(406, 17);
			this.checkApptRefreshEveryMinute.TabIndex = 235;
			this.checkApptRefreshEveryMinute.Text = "Refresh every 60 seconds";
			// 
			// comboDelay
			// 
			this.comboDelay.AllowDrop = true;
			this.comboDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboDelay.Location = new System.Drawing.Point(326, 90);
			this.comboDelay.Name = "comboDelay";
			this.comboDelay.Size = new System.Drawing.Size(114, 21);
			this.comboDelay.TabIndex = 232;
			// 
			// apptClickDelay
			// 
			this.apptClickDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.apptClickDelay.Location = new System.Drawing.Point(17, 93);
			this.apptClickDelay.Name = "apptClickDelay";
			this.apptClickDelay.Size = new System.Drawing.Size(306, 17);
			this.apptClickDelay.TabIndex = 233;
			this.apptClickDelay.Text = "Click delay after opening Appt Edit";
			this.apptClickDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label45
			// 
			this.label45.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label45.Location = new System.Drawing.Point(140, 135);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(183, 17);
			this.label45.TabIndex = 290;
			this.label45.Text = "Week View start day";
			this.label45.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboWeekViewStartDay
			// 
			this.comboWeekViewStartDay.AllowDrop = true;
			this.comboWeekViewStartDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboWeekViewStartDay.Location = new System.Drawing.Point(326, 132);
			this.comboWeekViewStartDay.Name = "comboWeekViewStartDay";
			this.comboWeekViewStartDay.Size = new System.Drawing.Size(114, 21);
			this.comboWeekViewStartDay.TabIndex = 289;
			// 
			// groupBoxBlockouts
			// 
			this.groupBoxBlockouts.Controls.Add(this.checkReplaceBlockouts);
			this.groupBoxBlockouts.Controls.Add(this.checkSolidBlockouts);
			this.groupBoxBlockouts.Location = new System.Drawing.Point(20, 186);
			this.groupBoxBlockouts.Name = "groupBoxBlockouts";
			this.groupBoxBlockouts.Size = new System.Drawing.Size(450, 51);
			this.groupBoxBlockouts.TabIndex = 0;
			this.groupBoxBlockouts.Text = "Blockouts";
			// 
			// checkReplaceBlockouts
			// 
			this.checkReplaceBlockouts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkReplaceBlockouts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReplaceBlockouts.Location = new System.Drawing.Point(15, 29);
			this.checkReplaceBlockouts.Name = "checkReplaceBlockouts";
			this.checkReplaceBlockouts.Size = new System.Drawing.Size(425, 17);
			this.checkReplaceBlockouts.TabIndex = 318;
			this.checkReplaceBlockouts.Text = "Allow \'Block appointment scheduling\' blockouts to replace conflicting blockouts";
			// 
			// checkSolidBlockouts
			// 
			this.checkSolidBlockouts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSolidBlockouts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSolidBlockouts.Location = new System.Drawing.Point(65, 10);
			this.checkSolidBlockouts.Name = "checkSolidBlockouts";
			this.checkSolidBlockouts.Size = new System.Drawing.Size(375, 17);
			this.checkSolidBlockouts.TabIndex = 220;
			this.checkSolidBlockouts.Text = "Use solid blockouts instead of outlines on the Appointments Module";
			// 
			// groupBoxApptBubble
			// 
			this.groupBoxApptBubble.Controls.Add(this.textApptBubNoteLength);
			this.groupBoxApptBubble.Controls.Add(this.label21);
			this.groupBoxApptBubble.Controls.Add(this.checkApptBubbleDelay);
			this.groupBoxApptBubble.Controls.Add(this.checkAppointmentBubblesDisabled);
			this.groupBoxApptBubble.Location = new System.Drawing.Point(20, 111);
			this.groupBoxApptBubble.Name = "groupBoxApptBubble";
			this.groupBoxApptBubble.Size = new System.Drawing.Size(450, 73);
			this.groupBoxApptBubble.TabIndex = 329;
			this.groupBoxApptBubble.Text = "Appointment Bubble";
			// 
			// textApptBubNoteLength
			// 
			this.textApptBubNoteLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptBubNoteLength.Location = new System.Drawing.Point(357, 10);
			this.textApptBubNoteLength.Name = "textApptBubNoteLength";
			this.textApptBubNoteLength.Size = new System.Drawing.Size(83, 20);
			this.textApptBubNoteLength.TabIndex = 304;
			// 
			// label21
			// 
			this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label21.Location = new System.Drawing.Point(108, 13);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(246, 17);
			this.label21.TabIndex = 303;
			this.label21.Text = "Appointment bubble max note length";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptBubbleDelay
			// 
			this.checkApptBubbleDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptBubbleDelay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptBubbleDelay.Location = new System.Drawing.Point(15, 51);
			this.checkApptBubbleDelay.Name = "checkApptBubbleDelay";
			this.checkApptBubbleDelay.Size = new System.Drawing.Size(425, 17);
			this.checkApptBubbleDelay.TabIndex = 221;
			this.checkApptBubbleDelay.Text = "Appointment bubble popup delay";
			// 
			// checkAppointmentBubblesDisabled
			// 
			this.checkAppointmentBubblesDisabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAppointmentBubblesDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentBubblesDisabled.Location = new System.Drawing.Point(40, 32);
			this.checkAppointmentBubblesDisabled.Name = "checkAppointmentBubblesDisabled";
			this.checkAppointmentBubblesDisabled.Size = new System.Drawing.Size(400, 17);
			this.checkAppointmentBubblesDisabled.TabIndex = 234;
			this.checkAppointmentBubblesDisabled.Text = "Default appointment bubble to \'disabled\' for new appointment views";
			// 
			// groupBoxAppearance
			// 
			this.groupBoxAppearance.Controls.Add(this.label58);
			this.groupBoxAppearance.Controls.Add(this.label46);
			this.groupBoxAppearance.Controls.Add(this.textApptProvbarWidth);
			this.groupBoxAppearance.Controls.Add(this.label54);
			this.groupBoxAppearance.Controls.Add(this.textApptFontSize);
			this.groupBoxAppearance.Controls.Add(this.butApptLineColor);
			this.groupBoxAppearance.Controls.Add(this.label25);
			this.groupBoxAppearance.Location = new System.Drawing.Point(20, 10);
			this.groupBoxAppearance.Name = "groupBoxAppearance";
			this.groupBoxAppearance.Size = new System.Drawing.Size(450, 99);
			this.groupBoxAppearance.TabIndex = 328;
			this.groupBoxAppearance.Text = "Appearance";
			// 
			// label58
			// 
			this.label58.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label58.Location = new System.Drawing.Point(72, 58);
			this.label58.Name = "label58";
			this.label58.Size = new System.Drawing.Size(315, 17);
			this.label58.TabIndex = 286;
			this.label58.Text = "Width of provider time bar on left of each appointment";
			this.label58.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label46
			// 
			this.label46.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label46.ForeColor = System.Drawing.Color.DarkRed;
			this.label46.Location = new System.Drawing.Point(198, 76);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(242, 17);
			this.label46.TabIndex = 291;
			this.label46.Text = "Also, see Appt View Edit for more settings";
			this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textApptProvbarWidth
			// 
			this.textApptProvbarWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptProvbarWidth.Location = new System.Drawing.Point(390, 55);
			this.textApptProvbarWidth.MaxVal = 20;
			this.textApptProvbarWidth.Name = "textApptProvbarWidth";
			this.textApptProvbarWidth.Size = new System.Drawing.Size(50, 20);
			this.textApptProvbarWidth.TabIndex = 288;
			// 
			// label54
			// 
			this.label54.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label54.Location = new System.Drawing.Point(14, 36);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(373, 17);
			this.label54.TabIndex = 251;
			this.label54.Text = "Appointment font size";
			this.label54.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textApptFontSize
			// 
			this.textApptFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptFontSize.Location = new System.Drawing.Point(390, 33);
			this.textApptFontSize.Name = "textApptFontSize";
			this.textApptFontSize.Size = new System.Drawing.Size(50, 20);
			this.textApptFontSize.TabIndex = 285;
			// 
			// butApptLineColor
			// 
			this.butApptLineColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butApptLineColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butApptLineColor.Location = new System.Drawing.Point(416, 10);
			this.butApptLineColor.Name = "butApptLineColor";
			this.butApptLineColor.Size = new System.Drawing.Size(24, 21);
			this.butApptLineColor.TabIndex = 226;
			this.butApptLineColor.Click += new System.EventHandler(this.butApptLineColor_Click);
			// 
			// label25
			// 
			this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label25.Location = new System.Drawing.Point(167, 13);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(246, 17);
			this.label25.TabIndex = 224;
			this.label25.Text = "Appointment time line color";
			this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBoxWaitingRoom
			// 
			this.groupBoxWaitingRoom.Controls.Add(this.label3);
			this.groupBoxWaitingRoom.Controls.Add(this.comboTimeArrived);
			this.groupBoxWaitingRoom.Controls.Add(this.comboTimeSeated);
			this.groupBoxWaitingRoom.Controls.Add(this.label5);
			this.groupBoxWaitingRoom.Controls.Add(this.comboTimeDismissed);
			this.groupBoxWaitingRoom.Controls.Add(this.label6);
			this.groupBoxWaitingRoom.Controls.Add(this.textWaitRoomWarn);
			this.groupBoxWaitingRoom.Controls.Add(this.label22);
			this.groupBoxWaitingRoom.Controls.Add(this.checkWaitingRoomFilterByView);
			this.groupBoxWaitingRoom.Controls.Add(this.butColor);
			this.groupBoxWaitingRoom.Controls.Add(this.label23);
			this.groupBoxWaitingRoom.Location = new System.Drawing.Point(20, 239);
			this.groupBoxWaitingRoom.Name = "groupBoxWaitingRoom";
			this.groupBoxWaitingRoom.Size = new System.Drawing.Size(450, 146);
			this.groupBoxWaitingRoom.TabIndex = 327;
			this.groupBoxWaitingRoom.Text = "Waiting Room";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(84, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(190, 17);
			this.label3.TabIndex = 292;
			this.label3.Text = "Time Arrived trigger";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboTimeArrived
			// 
			this.comboTimeArrived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTimeArrived.Location = new System.Drawing.Point(277, 10);
			this.comboTimeArrived.Name = "comboTimeArrived";
			this.comboTimeArrived.Size = new System.Drawing.Size(163, 21);
			this.comboTimeArrived.TabIndex = 291;
			// 
			// comboTimeSeated
			// 
			this.comboTimeSeated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTimeSeated.Location = new System.Drawing.Point(277, 33);
			this.comboTimeSeated.Name = "comboTimeSeated";
			this.comboTimeSeated.Size = new System.Drawing.Size(163, 21);
			this.comboTimeSeated.TabIndex = 293;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(27, 36);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(247, 17);
			this.label5.TabIndex = 294;
			this.label5.Text = "Time Seated (in op) trigger";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboTimeDismissed
			// 
			this.comboTimeDismissed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTimeDismissed.Location = new System.Drawing.Point(277, 56);
			this.comboTimeDismissed.Name = "comboTimeDismissed";
			this.comboTimeDismissed.Size = new System.Drawing.Size(163, 21);
			this.comboTimeDismissed.TabIndex = 295;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(27, 59);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(247, 17);
			this.label6.TabIndex = 296;
			this.label6.Text = "Time Dismissed trigger";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWaitRoomWarn
			// 
			this.textWaitRoomWarn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textWaitRoomWarn.Location = new System.Drawing.Point(357, 98);
			this.textWaitRoomWarn.Name = "textWaitRoomWarn";
			this.textWaitRoomWarn.Size = new System.Drawing.Size(83, 20);
			this.textWaitRoomWarn.TabIndex = 304;
			// 
			// label22
			// 
			this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label22.Location = new System.Drawing.Point(108, 101);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(246, 17);
			this.label22.TabIndex = 303;
			this.label22.Text = "Waiting room alert time";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkWaitingRoomFilterByView
			// 
			this.checkWaitingRoomFilterByView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkWaitingRoomFilterByView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWaitingRoomFilterByView.Location = new System.Drawing.Point(34, 79);
			this.checkWaitingRoomFilterByView.Name = "checkWaitingRoomFilterByView";
			this.checkWaitingRoomFilterByView.Size = new System.Drawing.Size(406, 17);
			this.checkWaitingRoomFilterByView.TabIndex = 300;
			this.checkWaitingRoomFilterByView.Text = "Filter the waiting room based on the selected appointment view";
			// 
			// butColor
			// 
			this.butColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(416, 120);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(24, 21);
			this.butColor.TabIndex = 225;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label23
			// 
			this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label23.Location = new System.Drawing.Point(167, 123);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(246, 17);
			this.label23.TabIndex = 223;
			this.label23.Text = "Waiting room alert color";
			this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelApptRefreshEveryMinuteDetails
			// 
			this.labelApptRefreshEveryMinuteDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelApptRefreshEveryMinuteDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelApptRefreshEveryMinuteDetails.Location = new System.Drawing.Point(476, 499);
			this.labelApptRefreshEveryMinuteDetails.Name = "labelApptRefreshEveryMinuteDetails";
			this.labelApptRefreshEveryMinuteDetails.Size = new System.Drawing.Size(498, 17);
			this.labelApptRefreshEveryMinuteDetails.TabIndex = 372;
			this.labelApptRefreshEveryMinuteDetails.Text = "keeps waiting room times refreshed";
			this.labelApptRefreshEveryMinuteDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlApptAppearance
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelApptRefreshEveryMinuteDetails);
			this.Controls.Add(this.butUseOpHygProvDetails);
			this.Controls.Add(this.labelWaitRoomWarnDetails);
			this.Controls.Add(this.labelApptBubNoteLengthDetails);
			this.Controls.Add(this.labelApptFontSizeDetails);
			this.Controls.Add(this.butTimeArrivedTriggerDetails);
			this.Controls.Add(this.labelApptSchedEnforceSpecialtyDetails);
			this.Controls.Add(this.labelApptClickDelayDetails);
			this.Controls.Add(this.labelApptProvbarWidthDetails);
			this.Controls.Add(this.labelApptLineColorDetails);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.groupBoxBlockouts);
			this.Controls.Add(this.groupBoxApptBubble);
			this.Controls.Add(this.groupBoxAppearance);
			this.Controls.Add(this.groupBoxWaitingRoom);
			this.Name = "UserControlApptAppearance";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxBlockouts.ResumeLayout(false);
			this.groupBoxApptBubble.ResumeLayout(false);
			this.groupBoxApptBubble.PerformLayout();
			this.groupBoxAppearance.ResumeLayout(false);
			this.groupBoxAppearance.PerformLayout();
			this.groupBoxWaitingRoom.ResumeLayout(false);
			this.groupBoxWaitingRoom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label label45;
		private UI.ComboBox comboWeekViewStartDay;
		private OpenDental.UI.CheckBox checkAppointmentBubblesDisabled;
		private ValidNum textApptProvbarWidth;
		private OpenDental.UI.CheckBox checkSolidBlockouts;
		private System.Windows.Forms.Label label58;
		private OpenDental.UI.CheckBox checkApptExclamation;
		private System.Windows.Forms.TextBox textApptFontSize;
		private System.Windows.Forms.Button butApptLineColor;
		private OpenDental.UI.CheckBox checkApptBubbleDelay;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Label label23;
		private UI.ComboBox comboDelay;
		private System.Windows.Forms.Label label25;
		private OpenDental.UI.CheckBox checkApptModuleDefaultToWeek;
		private OpenDental.UI.CheckBox checkApptRefreshEveryMinute;
		private System.Windows.Forms.Label apptClickDelay;
		private System.Windows.Forms.Label label54;
		private UI.GroupBox groupBoxWaitingRoom;
		private System.Windows.Forms.Label label3;
		private UI.ComboBox comboTimeArrived;
		private UI.ComboBox comboTimeSeated;
		private System.Windows.Forms.Label label5;
		private UI.ComboBox comboTimeDismissed;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textWaitRoomWarn;
		private System.Windows.Forms.Label label22;
		private OpenDental.UI.CheckBox checkWaitingRoomFilterByView;
		private UI.GroupBox groupBoxAppearance;
		private UI.GroupBox groupBoxApptBubble;
		private UI.GroupBox groupBoxBlockouts;
		private System.Windows.Forms.TextBox textApptBubNoteLength;
		private System.Windows.Forms.Label label21;
		private OpenDental.UI.CheckBox checkReplaceBlockouts;
		private UI.GroupBox groupBoxOD1;
		private System.Windows.Forms.Label labelApptSchedEnforceSpecialty;
		private UI.ComboBox comboApptSchedEnforceSpecialty;
		private OpenDental.UI.CheckBox checkUseOpHygProv;
		private System.Windows.Forms.Label labelApptLineColorDetails;
		private System.Windows.Forms.Label labelApptProvbarWidthDetails;
		private System.Windows.Forms.Label labelApptClickDelayDetails;
		private System.Windows.Forms.Label labelApptSchedEnforceSpecialtyDetails;
		private UI.Button butTimeArrivedTriggerDetails;
		private System.Windows.Forms.Label labelApptFontSizeDetails;
		private System.Windows.Forms.Label labelApptBubNoteLengthDetails;
		private System.Windows.Forms.Label labelWaitRoomWarnDetails;
		private UI.Button butUseOpHygProvDetails;
		private System.Windows.Forms.Label labelApptRefreshEveryMinuteDetails;
	}
}
