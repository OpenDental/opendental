namespace OpenDental {
	partial class FormEhrVaccinePatEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrVaccinePatEdit));
			this.label5 = new System.Windows.Forms.Label();
			this.labelAmount = new System.Windows.Forms.Label();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.comboVaccine = new System.Windows.Forms.ComboBox();
			this.labelVaccine = new System.Windows.Forms.Label();
			this.comboUnits = new System.Windows.Forms.ComboBox();
			this.textManufacturer = new System.Windows.Forms.TextBox();
			this.labelDateTimeStartStop = new System.Windows.Forms.Label();
			this.textDateTimeStart = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textLotNum = new System.Windows.Forms.TextBox();
			this.textDateTimeStop = new System.Windows.Forms.TextBox();
			this.labelDateTimeStop = new System.Windows.Forms.Label();
			this.labelDocument = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textFilledCity = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textFilledSt = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.listCompletionStatus = new OpenDental.UI.ListBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.listAdministrationNote = new OpenDental.UI.ListBoxOD();
			this.label9 = new System.Windows.Forms.Label();
			this.comboProvNumOrdering = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.comboProvNumAdministering = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.listRefusalReason = new OpenDental.UI.ListBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.listAction = new OpenDental.UI.ListBoxOD();
			this.label15 = new System.Windows.Forms.Label();
			this.comboAdministrationRoute = new System.Windows.Forms.ComboBox();
			this.comboAdministrationSite = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.gridObservations = new OpenDental.UI.GridOD();
			this.butUngroupObservations = new OpenDental.UI.Button();
			this.butGroupObservations = new OpenDental.UI.Button();
			this.butAddObservation = new OpenDental.UI.Button();
			this.button1 = new OpenDental.UI.Button();
			this.button2 = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butNoneProvAdministering = new OpenDental.UI.Button();
			this.butNoneProvOrdering = new OpenDental.UI.Button();
			this.textDateExpiration = new OpenDental.ValidDate();
			this.butPickProvAdministering = new OpenDental.UI.Button();
			this.butPickProvOrdering = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(130, 17);
			this.label5.TabIndex = 12;
			this.label5.Text = "Manufacturer";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(5, 128);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(130, 17);
			this.labelAmount.TabIndex = 10;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(136, 128);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(63, 20);
			this.textAmount.TabIndex = 3;
			// 
			// comboVaccine
			// 
			this.comboVaccine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboVaccine.FormattingEnabled = true;
			this.comboVaccine.Location = new System.Drawing.Point(136, 23);
			this.comboVaccine.Name = "comboVaccine";
			this.comboVaccine.Size = new System.Drawing.Size(201, 21);
			this.comboVaccine.TabIndex = 0;
			this.comboVaccine.SelectedIndexChanged += new System.EventHandler(this.comboVaccine_SelectedIndexChanged);
			// 
			// labelVaccine
			// 
			this.labelVaccine.Location = new System.Drawing.Point(5, 23);
			this.labelVaccine.Name = "labelVaccine";
			this.labelVaccine.Size = new System.Drawing.Size(130, 17);
			this.labelVaccine.TabIndex = 13;
			this.labelVaccine.Text = "Vaccine Def";
			this.labelVaccine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUnits
			// 
			this.comboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnits.FormattingEnabled = true;
			this.comboUnits.Location = new System.Drawing.Point(136, 155);
			this.comboUnits.Name = "comboUnits";
			this.comboUnits.Size = new System.Drawing.Size(63, 21);
			this.comboUnits.TabIndex = 4;
			// 
			// textManufacturer
			// 
			this.textManufacturer.Location = new System.Drawing.Point(136, 48);
			this.textManufacturer.Name = "textManufacturer";
			this.textManufacturer.ReadOnly = true;
			this.textManufacturer.Size = new System.Drawing.Size(201, 20);
			this.textManufacturer.TabIndex = 14;
			this.textManufacturer.TabStop = false;
			// 
			// labelDateTimeStartStop
			// 
			this.labelDateTimeStartStop.Location = new System.Drawing.Point(5, 74);
			this.labelDateTimeStartStop.Name = "labelDateTimeStartStop";
			this.labelDateTimeStartStop.Size = new System.Drawing.Size(130, 17);
			this.labelDateTimeStartStop.TabIndex = 11;
			this.labelDateTimeStartStop.Text = "Date Time Start";
			this.labelDateTimeStartStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTimeStart
			// 
			this.textDateTimeStart.Location = new System.Drawing.Point(136, 74);
			this.textDateTimeStart.Name = "textDateTimeStart";
			this.textDateTimeStart.Size = new System.Drawing.Size(151, 20);
			this.textDateTimeStart.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 182);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "Lot Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLotNum
			// 
			this.textLotNum.Location = new System.Drawing.Point(136, 182);
			this.textLotNum.Name = "textLotNum";
			this.textLotNum.Size = new System.Drawing.Size(118, 20);
			this.textLotNum.TabIndex = 5;
			// 
			// textDateTimeStop
			// 
			this.textDateTimeStop.Location = new System.Drawing.Point(136, 101);
			this.textDateTimeStop.Name = "textDateTimeStop";
			this.textDateTimeStop.Size = new System.Drawing.Size(151, 20);
			this.textDateTimeStop.TabIndex = 2;
			// 
			// labelDateTimeStop
			// 
			this.labelDateTimeStop.Location = new System.Drawing.Point(5, 101);
			this.labelDateTimeStop.Name = "labelDateTimeStop";
			this.labelDateTimeStop.Size = new System.Drawing.Size(130, 17);
			this.labelDateTimeStop.TabIndex = 11;
			this.labelDateTimeStop.Text = "Date Time Stop";
			this.labelDateTimeStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDocument
			// 
			this.labelDocument.Location = new System.Drawing.Point(136, 371);
			this.labelDocument.Name = "labelDocument";
			this.labelDocument.Size = new System.Drawing.Size(336, 45);
			this.labelDocument.TabIndex = 16;
			this.labelDocument.Text = "Document reason not given below.  Reason can include a specific allergy, adverse " +
    "effect, intollerance, patient declines, specific disease, etc.";
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(136, 419);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(336, 75);
			this.textNote.TabIndex = 17;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 419);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(130, 17);
			this.label3.TabIndex = 18;
			this.label3.Text = "Note";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 155);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 17);
			this.label1.TabIndex = 19;
			this.label1.Text = "Units";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFilledCity
			// 
			this.textFilledCity.Location = new System.Drawing.Point(624, 23);
			this.textFilledCity.Name = "textFilledCity";
			this.textFilledCity.Size = new System.Drawing.Size(151, 20);
			this.textFilledCity.TabIndex = 20;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(494, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(130, 17);
			this.label4.TabIndex = 21;
			this.label4.Text = "City Where Filled";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFilledSt
			// 
			this.textFilledSt.Location = new System.Drawing.Point(624, 48);
			this.textFilledSt.Name = "textFilledSt";
			this.textFilledSt.Size = new System.Drawing.Size(151, 20);
			this.textFilledSt.TabIndex = 22;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(494, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(130, 17);
			this.label6.TabIndex = 23;
			this.label6.Text = "State Where Filled";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listCompletionStatus
			// 
			this.listCompletionStatus.Location = new System.Drawing.Point(136, 234);
			this.listCompletionStatus.Name = "listCompletionStatus";
			this.listCompletionStatus.Size = new System.Drawing.Size(151, 56);
			this.listCompletionStatus.TabIndex = 24;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(5, 234);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(130, 17);
			this.label7.TabIndex = 25;
			this.label7.Text = "Completion Status";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(494, 208);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(130, 17);
			this.label8.TabIndex = 27;
			this.label8.Text = "Administration Note";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listAdministrationNote
			// 
			this.listAdministrationNote.Location = new System.Drawing.Point(624, 208);
			this.listAdministrationNote.Name = "listAdministrationNote";
			this.listAdministrationNote.Size = new System.Drawing.Size(151, 121);
			this.listAdministrationNote.TabIndex = 26;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(494, 74);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(130, 17);
			this.label9.TabIndex = 28;
			this.label9.Text = "User";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProvNumOrdering
			// 
			this.comboProvNumOrdering.Location = new System.Drawing.Point(624, 101);
			this.comboProvNumOrdering.MaxDropDownItems = 30;
			this.comboProvNumOrdering.Name = "comboProvNumOrdering";
			this.comboProvNumOrdering.Size = new System.Drawing.Size(254, 21);
			this.comboProvNumOrdering.TabIndex = 262;
			this.comboProvNumOrdering.SelectionChangeCommitted += new System.EventHandler(this.comboProvNumOrdering_SelectionChangeCommitted);
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(494, 101);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(130, 17);
			this.label10.TabIndex = 261;
			this.label10.Text = "Ordering Provider";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProvNumAdministering
			// 
			this.comboProvNumAdministering.Location = new System.Drawing.Point(624, 128);
			this.comboProvNumAdministering.MaxDropDownItems = 30;
			this.comboProvNumAdministering.Name = "comboProvNumAdministering";
			this.comboProvNumAdministering.Size = new System.Drawing.Size(254, 21);
			this.comboProvNumAdministering.TabIndex = 265;
			this.comboProvNumAdministering.SelectionChangeCommitted += new System.EventHandler(this.comboProvNumAdministering_SelectionChangeCommitted);
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(494, 128);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(130, 17);
			this.label11.TabIndex = 264;
			this.label11.Text = "Administering Provider";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(5, 208);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(130, 17);
			this.label12.TabIndex = 268;
			this.label12.Text = "Date Expiration";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(5, 296);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(130, 17);
			this.label13.TabIndex = 270;
			this.label13.Text = "Refusal Reason";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listRefusalReason
			// 
			this.listRefusalReason.Location = new System.Drawing.Point(136, 296);
			this.listRefusalReason.Name = "listRefusalReason";
			this.listRefusalReason.Size = new System.Drawing.Size(151, 69);
			this.listRefusalReason.TabIndex = 269;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(494, 335);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(130, 17);
			this.label14.TabIndex = 272;
			this.label14.Text = "Action";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listAction
			// 
			this.listAction.Location = new System.Drawing.Point(624, 335);
			this.listAction.Name = "listAction";
			this.listAction.Size = new System.Drawing.Size(151, 43);
			this.listAction.TabIndex = 271;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(494, 155);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(130, 17);
			this.label15.TabIndex = 274;
			this.label15.Text = "Administration Route";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAdministrationRoute
			// 
			this.comboAdministrationRoute.FormattingEnabled = true;
			this.comboAdministrationRoute.Location = new System.Drawing.Point(624, 155);
			this.comboAdministrationRoute.Name = "comboAdministrationRoute";
			this.comboAdministrationRoute.Size = new System.Drawing.Size(151, 21);
			this.comboAdministrationRoute.TabIndex = 275;
			// 
			// comboAdministrationSite
			// 
			this.comboAdministrationSite.FormattingEnabled = true;
			this.comboAdministrationSite.Location = new System.Drawing.Point(624, 182);
			this.comboAdministrationSite.Name = "comboAdministrationSite";
			this.comboAdministrationSite.Size = new System.Drawing.Size(151, 21);
			this.comboAdministrationSite.TabIndex = 277;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(494, 182);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(130, 17);
			this.label16.TabIndex = 276;
			this.label16.Text = "Administration Site";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(624, 74);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(151, 20);
			this.textUser.TabIndex = 283;
			// 
			// gridObservations
			// 
			this.gridObservations.Location = new System.Drawing.Point(624, 384);
			this.gridObservations.Name = "gridObservations";
			this.gridObservations.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridObservations.Size = new System.Drawing.Size(254, 110);
			this.gridObservations.TabIndex = 284;
			this.gridObservations.Title = "Observations";
			this.gridObservations.TranslationName = "TableObservations";
			this.gridObservations.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridObservations_CellDoubleClick);
			this.gridObservations.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridObservations_CellClick);
			// 
			// butUngroupObservations
			// 
			this.butUngroupObservations.Location = new System.Drawing.Point(803, 497);
			this.butUngroupObservations.Name = "butUngroupObservations";
			this.butUngroupObservations.Size = new System.Drawing.Size(75, 22);
			this.butUngroupObservations.TabIndex = 287;
			this.butUngroupObservations.Text = "Ungroup";
			this.butUngroupObservations.Click += new System.EventHandler(this.butUngroupObservations_Click);
			// 
			// butGroupObservations
			// 
			this.butGroupObservations.Location = new System.Drawing.Point(725, 497);
			this.butGroupObservations.Name = "butGroupObservations";
			this.butGroupObservations.Size = new System.Drawing.Size(75, 22);
			this.butGroupObservations.TabIndex = 286;
			this.butGroupObservations.Text = "Group";
			this.butGroupObservations.Click += new System.EventHandler(this.butGroupObservations_Click);
			// 
			// butAddObservation
			// 
			this.butAddObservation.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddObservation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddObservation.Location = new System.Drawing.Point(624, 497);
			this.butAddObservation.Name = "butAddObservation";
			this.butAddObservation.Size = new System.Drawing.Size(75, 22);
			this.butAddObservation.TabIndex = 285;
			this.butAddObservation.Text = "&Add";
			this.butAddObservation.Click += new System.EventHandler(this.butAddObservation_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(772, 563);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(92, 24);
			this.button1.TabIndex = 282;
			this.button1.Text = "&OK";
			this.button1.Click += new System.EventHandler(this.butOK_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(870, 563);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(92, 24);
			this.button2.TabIndex = 281;
			this.button2.Text = "&Cancel";
			this.button2.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(11, 563);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 280;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butNoneProvAdministering
			// 
			this.butNoneProvAdministering.Location = new System.Drawing.Point(901, 128);
			this.butNoneProvAdministering.Name = "butNoneProvAdministering";
			this.butNoneProvAdministering.Size = new System.Drawing.Size(44, 21);
			this.butNoneProvAdministering.TabIndex = 279;
			this.butNoneProvAdministering.Text = "None";
			this.butNoneProvAdministering.Click += new System.EventHandler(this.butNoneProvAdministering_Click);
			// 
			// butNoneProvOrdering
			// 
			this.butNoneProvOrdering.Location = new System.Drawing.Point(901, 101);
			this.butNoneProvOrdering.Name = "butNoneProvOrdering";
			this.butNoneProvOrdering.Size = new System.Drawing.Size(44, 21);
			this.butNoneProvOrdering.TabIndex = 278;
			this.butNoneProvOrdering.Text = "None";
			this.butNoneProvOrdering.Click += new System.EventHandler(this.butNoneProvOrdering_Click);
			// 
			// textDateExpiration
			// 
			this.textDateExpiration.Location = new System.Drawing.Point(136, 208);
			this.textDateExpiration.Name = "textDateExpiration";
			this.textDateExpiration.Size = new System.Drawing.Size(151, 20);
			this.textDateExpiration.TabIndex = 267;
			// 
			// butPickProvAdministering
			// 
			this.butPickProvAdministering.Location = new System.Drawing.Point(880, 128);
			this.butPickProvAdministering.Name = "butPickProvAdministering";
			this.butPickProvAdministering.Size = new System.Drawing.Size(18, 21);
			this.butPickProvAdministering.TabIndex = 266;
			this.butPickProvAdministering.Text = "...";
			this.butPickProvAdministering.Click += new System.EventHandler(this.butPickProvAdministering_Click);
			// 
			// butPickProvOrdering
			// 
			this.butPickProvOrdering.Location = new System.Drawing.Point(880, 101);
			this.butPickProvOrdering.Name = "butPickProvOrdering";
			this.butPickProvOrdering.Size = new System.Drawing.Size(18, 21);
			this.butPickProvOrdering.TabIndex = 263;
			this.butPickProvOrdering.Text = "...";
			this.butPickProvOrdering.Click += new System.EventHandler(this.butPickProvOrdering_Click);
			// 
			// FormEhrVaccinePatEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(974, 599);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.butUngroupObservations);
			this.Controls.Add(this.butGroupObservations);
			this.Controls.Add(this.butAddObservation);
			this.Controls.Add(this.gridObservations);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butNoneProvAdministering);
			this.Controls.Add(this.butNoneProvOrdering);
			this.Controls.Add(this.comboAdministrationSite);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.comboAdministrationRoute);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.listAction);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.listRefusalReason);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textDateExpiration);
			this.Controls.Add(this.comboProvNumAdministering);
			this.Controls.Add(this.butPickProvAdministering);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.comboProvNumOrdering);
			this.Controls.Add(this.butPickProvOrdering);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.listAdministrationNote);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listCompletionStatus);
			this.Controls.Add(this.textFilledSt);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textFilledCity);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelDocument);
			this.Controls.Add(this.textDateTimeStop);
			this.Controls.Add(this.textDateTimeStart);
			this.Controls.Add(this.labelDateTimeStop);
			this.Controls.Add(this.labelDateTimeStartStop);
			this.Controls.Add(this.comboUnits);
			this.Controls.Add(this.comboVaccine);
			this.Controls.Add(this.labelVaccine);
			this.Controls.Add(this.textLotNum);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.textManufacturer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelAmount);
			this.Controls.Add(this.label5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrVaccinePatEdit";
			this.Text = "Vaccine Edit";
			this.Load += new System.EventHandler(this.FormVaccinePatEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelAmount;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.ComboBox comboVaccine;
		private System.Windows.Forms.Label labelVaccine;
		private System.Windows.Forms.ComboBox comboUnits;
		private System.Windows.Forms.TextBox textManufacturer;
		private System.Windows.Forms.Label labelDateTimeStartStop;
		private System.Windows.Forms.TextBox textDateTimeStart;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textLotNum;
		private System.Windows.Forms.TextBox textDateTimeStop;
		private System.Windows.Forms.Label labelDateTimeStop;
		private System.Windows.Forms.Label labelDocument;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFilledCity;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textFilledSt;
		private System.Windows.Forms.Label label6;
		private UI.ListBoxOD listCompletionStatus;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private UI.ListBoxOD listAdministrationNote;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboProvNumOrdering;
		private UI.Button butPickProvOrdering;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox comboProvNumAdministering;
		private UI.Button butPickProvAdministering;
		private System.Windows.Forms.Label label11;
		private ValidDate textDateExpiration;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private UI.ListBoxOD listRefusalReason;
		private System.Windows.Forms.Label label14;
		private UI.ListBoxOD listAction;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.ComboBox comboAdministrationRoute;
		private System.Windows.Forms.ComboBox comboAdministrationSite;
		private System.Windows.Forms.Label label16;
		private UI.Button butNoneProvOrdering;
		private UI.Button butNoneProvAdministering;
		private UI.Button butDelete;
		private UI.Button button1;
		private UI.Button button2;
		private System.Windows.Forms.TextBox textUser;
		private UI.GridOD gridObservations;
		private UI.Button butAddObservation;
		private UI.Button butGroupObservations;
		private UI.Button butUngroupObservations;
	}
}