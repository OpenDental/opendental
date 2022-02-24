namespace OpenDental{
	partial class FormBenefitFrequencies {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBenefitFrequencies));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupDiagnostic = new OpenDental.UI.GroupBoxOD();
			this.comboCancerScreenings = new OpenDental.UI.ComboBoxOD();
			this.textCancerScreenings = new OpenDental.ValidNum();
			this.label1 = new System.Windows.Forms.Label();
			this.comboExams = new OpenDental.UI.ComboBoxOD();
			this.textExams = new OpenDental.ValidNum();
			this.label8 = new System.Windows.Forms.Label();
			this.comboPano = new OpenDental.UI.ComboBoxOD();
			this.textPano = new OpenDental.ValidNum();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBW = new OpenDental.UI.ComboBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.textBW = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.groupPreventive = new OpenDental.UI.GroupBoxOD();
			this.comboSealants = new OpenDental.UI.ComboBoxOD();
			this.textSealants = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.comboFlouride = new OpenDental.UI.ComboBoxOD();
			this.textFlouride = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.comboProphy = new OpenDental.UI.ComboBoxOD();
			this.label9 = new System.Windows.Forms.Label();
			this.textProphy = new OpenDental.ValidNum();
			this.label10 = new System.Windows.Forms.Label();
			this.groupRestorative = new OpenDental.UI.GroupBoxOD();
			this.comboCrown = new OpenDental.UI.ComboBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.textCrown = new OpenDental.ValidNum();
			this.label15 = new System.Windows.Forms.Label();
			this.groupPerio = new OpenDental.UI.GroupBoxOD();
			this.comboPerioMaint = new OpenDental.UI.ComboBoxOD();
			this.textPerioMaint = new OpenDental.ValidNum();
			this.label17 = new System.Windows.Forms.Label();
			this.comboDebridement = new OpenDental.UI.ComboBoxOD();
			this.textDebridement = new OpenDental.ValidNum();
			this.label18 = new System.Windows.Forms.Label();
			this.comboSRP = new OpenDental.UI.ComboBoxOD();
			this.label19 = new System.Windows.Forms.Label();
			this.textSRP = new OpenDental.ValidNum();
			this.label20 = new System.Windows.Forms.Label();
			this.groupProsthodontics = new OpenDental.UI.GroupBoxOD();
			this.comboDentures = new OpenDental.UI.ComboBoxOD();
			this.label24 = new System.Windows.Forms.Label();
			this.textDentures = new OpenDental.ValidNum();
			this.label25 = new System.Windows.Forms.Label();
			this.groupImplants = new OpenDental.UI.GroupBoxOD();
			this.comboImplant = new OpenDental.UI.ComboBoxOD();
			this.textImplant = new OpenDental.ValidNum();
			this.label26 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.groupDiagnostic.SuspendLayout();
			this.groupPreventive.SuspendLayout();
			this.groupRestorative.SuspendLayout();
			this.groupPerio.SuspendLayout();
			this.groupProsthodontics.SuspendLayout();
			this.groupImplants.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(277, 511);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(358, 511);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupDiagnostic
			// 
			this.groupDiagnostic.Controls.Add(this.comboCancerScreenings);
			this.groupDiagnostic.Controls.Add(this.textCancerScreenings);
			this.groupDiagnostic.Controls.Add(this.label1);
			this.groupDiagnostic.Controls.Add(this.comboExams);
			this.groupDiagnostic.Controls.Add(this.textExams);
			this.groupDiagnostic.Controls.Add(this.label8);
			this.groupDiagnostic.Controls.Add(this.comboPano);
			this.groupDiagnostic.Controls.Add(this.textPano);
			this.groupDiagnostic.Controls.Add(this.label7);
			this.groupDiagnostic.Controls.Add(this.comboBW);
			this.groupDiagnostic.Controls.Add(this.label6);
			this.groupDiagnostic.Controls.Add(this.textBW);
			this.groupDiagnostic.Controls.Add(this.label5);
			this.groupDiagnostic.Location = new System.Drawing.Point(46, 12);
			this.groupDiagnostic.Name = "groupDiagnostic";
			this.groupDiagnostic.Size = new System.Drawing.Size(341, 115);
			this.groupDiagnostic.TabIndex = 0;
			this.groupDiagnostic.Text = "Diagnostic";
			// 
			// comboCancerScreenings
			// 
			this.comboCancerScreenings.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboCancerScreenings.Location = new System.Drawing.Point(197, 86);
			this.comboCancerScreenings.Name = "comboCancerScreenings";
			this.comboCancerScreenings.Size = new System.Drawing.Size(136, 21);
			this.comboCancerScreenings.TabIndex = 7;
			// 
			// textCancerScreenings
			// 
			this.textCancerScreenings.Location = new System.Drawing.Point(154, 86);
			this.textCancerScreenings.MaxVal = 255;
			this.textCancerScreenings.MinVal = 0;
			this.textCancerScreenings.Name = "textCancerScreenings";
			this.textCancerScreenings.ShowZero = false;
			this.textCancerScreenings.Size = new System.Drawing.Size(39, 20);
			this.textCancerScreenings.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(45, 85);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 21);
			this.label1.TabIndex = 179;
			this.label1.Text = "Cancer Screenings";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboExams
			// 
			this.comboExams.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboExams.Location = new System.Drawing.Point(197, 65);
			this.comboExams.Name = "comboExams";
			this.comboExams.Size = new System.Drawing.Size(136, 21);
			this.comboExams.TabIndex = 5;
			// 
			// textExams
			// 
			this.textExams.Location = new System.Drawing.Point(154, 65);
			this.textExams.MaxVal = 255;
			this.textExams.MinVal = 0;
			this.textExams.Name = "textExams";
			this.textExams.ShowZero = false;
			this.textExams.Size = new System.Drawing.Size(39, 20);
			this.textExams.TabIndex = 4;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(82, 64);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(70, 21);
			this.label8.TabIndex = 176;
			this.label8.Text = "Exams";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPano
			// 
			this.comboPano.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboPano.Location = new System.Drawing.Point(197, 44);
			this.comboPano.Name = "comboPano";
			this.comboPano.Size = new System.Drawing.Size(136, 21);
			this.comboPano.TabIndex = 3;
			// 
			// textPano
			// 
			this.textPano.Location = new System.Drawing.Point(154, 44);
			this.textPano.MaxVal = 255;
			this.textPano.MinVal = 0;
			this.textPano.Name = "textPano";
			this.textPano.ShowZero = false;
			this.textPano.Size = new System.Drawing.Size(39, 20);
			this.textPano.TabIndex = 2;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(82, 43);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 21);
			this.label7.TabIndex = 173;
			this.label7.Text = "Pano/FMX";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBW
			// 
			this.comboBW.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboBW.Location = new System.Drawing.Point(197, 23);
			this.comboBW.Name = "comboBW";
			this.comboBW.Size = new System.Drawing.Size(136, 21);
			this.comboBW.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(156, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(39, 12);
			this.label6.TabIndex = 170;
			this.label6.Text = "#";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textBW
			// 
			this.textBW.Location = new System.Drawing.Point(154, 23);
			this.textBW.MaxVal = 255;
			this.textBW.MinVal = 0;
			this.textBW.Name = "textBW";
			this.textBW.ShowZero = false;
			this.textBW.Size = new System.Drawing.Size(39, 20);
			this.textBW.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(82, 22);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(70, 21);
			this.label5.TabIndex = 168;
			this.label5.Text = "BWs";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPreventive
			// 
			this.groupPreventive.Controls.Add(this.comboSealants);
			this.groupPreventive.Controls.Add(this.textSealants);
			this.groupPreventive.Controls.Add(this.label2);
			this.groupPreventive.Controls.Add(this.comboFlouride);
			this.groupPreventive.Controls.Add(this.textFlouride);
			this.groupPreventive.Controls.Add(this.label4);
			this.groupPreventive.Controls.Add(this.comboProphy);
			this.groupPreventive.Controls.Add(this.label9);
			this.groupPreventive.Controls.Add(this.textProphy);
			this.groupPreventive.Controls.Add(this.label10);
			this.groupPreventive.Location = new System.Drawing.Point(46, 132);
			this.groupPreventive.Name = "groupPreventive";
			this.groupPreventive.Size = new System.Drawing.Size(341, 93);
			this.groupPreventive.TabIndex = 1;
			this.groupPreventive.Text = "Preventive";
			// 
			// comboSealants
			// 
			this.comboSealants.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboSealants.Location = new System.Drawing.Point(197, 65);
			this.comboSealants.Name = "comboSealants";
			this.comboSealants.Size = new System.Drawing.Size(136, 21);
			this.comboSealants.TabIndex = 5;
			// 
			// textSealants
			// 
			this.textSealants.Location = new System.Drawing.Point(154, 65);
			this.textSealants.MaxVal = 255;
			this.textSealants.MinVal = 0;
			this.textSealants.Name = "textSealants";
			this.textSealants.ShowZero = false;
			this.textSealants.Size = new System.Drawing.Size(39, 20);
			this.textSealants.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 21);
			this.label2.TabIndex = 179;
			this.label2.Text = "Sealants";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFlouride
			// 
			this.comboFlouride.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboFlouride.Location = new System.Drawing.Point(197, 44);
			this.comboFlouride.Name = "comboFlouride";
			this.comboFlouride.Size = new System.Drawing.Size(136, 21);
			this.comboFlouride.TabIndex = 3;
			// 
			// textFlouride
			// 
			this.textFlouride.Location = new System.Drawing.Point(154, 44);
			this.textFlouride.MaxVal = 255;
			this.textFlouride.MinVal = 0;
			this.textFlouride.Name = "textFlouride";
			this.textFlouride.ShowZero = false;
			this.textFlouride.Size = new System.Drawing.Size(39, 20);
			this.textFlouride.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 43);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(134, 21);
			this.label4.TabIndex = 173;
			this.label4.Text = "Fluoride";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProphy
			// 
			this.comboProphy.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboProphy.Location = new System.Drawing.Point(197, 23);
			this.comboProphy.Name = "comboProphy";
			this.comboProphy.Size = new System.Drawing.Size(136, 21);
			this.comboProphy.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(156, 9);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(39, 12);
			this.label9.TabIndex = 170;
			this.label9.Text = "#";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textProphy
			// 
			this.textProphy.Location = new System.Drawing.Point(154, 23);
			this.textProphy.MaxVal = 255;
			this.textProphy.MinVal = 0;
			this.textProphy.Name = "textProphy";
			this.textProphy.ShowZero = false;
			this.textProphy.Size = new System.Drawing.Size(39, 20);
			this.textProphy.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(140, 21);
			this.label10.TabIndex = 168;
			this.label10.Text = "Prophylaxis";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupRestorative
			// 
			this.groupRestorative.Controls.Add(this.comboCrown);
			this.groupRestorative.Controls.Add(this.label14);
			this.groupRestorative.Controls.Add(this.textCrown);
			this.groupRestorative.Controls.Add(this.label15);
			this.groupRestorative.Location = new System.Drawing.Point(46, 232);
			this.groupRestorative.Name = "groupRestorative";
			this.groupRestorative.Size = new System.Drawing.Size(341, 50);
			this.groupRestorative.TabIndex = 2;
			this.groupRestorative.Text = "Restorative";
			// 
			// comboCrown
			// 
			this.comboCrown.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboCrown.Location = new System.Drawing.Point(197, 23);
			this.comboCrown.Name = "comboCrown";
			this.comboCrown.Size = new System.Drawing.Size(136, 21);
			this.comboCrown.TabIndex = 1;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(156, 9);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(39, 12);
			this.label14.TabIndex = 170;
			this.label14.Text = "#";
			this.label14.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textCrown
			// 
			this.textCrown.Location = new System.Drawing.Point(154, 23);
			this.textCrown.MaxVal = 255;
			this.textCrown.MinVal = 0;
			this.textCrown.Name = "textCrown";
			this.textCrown.ShowZero = false;
			this.textCrown.Size = new System.Drawing.Size(39, 20);
			this.textCrown.TabIndex = 0;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(9, 22);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(143, 21);
			this.label15.TabIndex = 168;
			this.label15.Text = "Crowns";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPerio
			// 
			this.groupPerio.Controls.Add(this.comboPerioMaint);
			this.groupPerio.Controls.Add(this.textPerioMaint);
			this.groupPerio.Controls.Add(this.label17);
			this.groupPerio.Controls.Add(this.comboDebridement);
			this.groupPerio.Controls.Add(this.textDebridement);
			this.groupPerio.Controls.Add(this.label18);
			this.groupPerio.Controls.Add(this.comboSRP);
			this.groupPerio.Controls.Add(this.label19);
			this.groupPerio.Controls.Add(this.textSRP);
			this.groupPerio.Controls.Add(this.label20);
			this.groupPerio.Location = new System.Drawing.Point(46, 289);
			this.groupPerio.Name = "groupPerio";
			this.groupPerio.Size = new System.Drawing.Size(341, 93);
			this.groupPerio.TabIndex = 3;
			this.groupPerio.Text = "Periodontal";
			// 
			// comboPerioMaint
			// 
			this.comboPerioMaint.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboPerioMaint.Location = new System.Drawing.Point(197, 66);
			this.comboPerioMaint.Name = "comboPerioMaint";
			this.comboPerioMaint.Size = new System.Drawing.Size(136, 21);
			this.comboPerioMaint.TabIndex = 5;
			// 
			// textPerioMaint
			// 
			this.textPerioMaint.Location = new System.Drawing.Point(154, 66);
			this.textPerioMaint.MaxVal = 255;
			this.textPerioMaint.MinVal = 0;
			this.textPerioMaint.Name = "textPerioMaint";
			this.textPerioMaint.ShowZero = false;
			this.textPerioMaint.Size = new System.Drawing.Size(39, 20);
			this.textPerioMaint.TabIndex = 4;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(45, 65);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(107, 21);
			this.label17.TabIndex = 176;
			this.label17.Text = "Perio Maintenance";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDebridement
			// 
			this.comboDebridement.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboDebridement.Location = new System.Drawing.Point(197, 45);
			this.comboDebridement.Name = "comboDebridement";
			this.comboDebridement.Size = new System.Drawing.Size(136, 21);
			this.comboDebridement.TabIndex = 3;
			// 
			// textDebridement
			// 
			this.textDebridement.Location = new System.Drawing.Point(154, 45);
			this.textDebridement.MaxVal = 255;
			this.textDebridement.MinVal = 0;
			this.textDebridement.Name = "textDebridement";
			this.textDebridement.ShowZero = false;
			this.textDebridement.Size = new System.Drawing.Size(39, 20);
			this.textDebridement.TabIndex = 2;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(3, 44);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(149, 21);
			this.label18.TabIndex = 173;
			this.label18.Text = "Full Debridement";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSRP
			// 
			this.comboSRP.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboSRP.Location = new System.Drawing.Point(197, 24);
			this.comboSRP.Name = "comboSRP";
			this.comboSRP.Size = new System.Drawing.Size(136, 21);
			this.comboSRP.TabIndex = 1;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(155, 9);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(39, 12);
			this.label19.TabIndex = 170;
			this.label19.Text = "#";
			this.label19.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textSRP
			// 
			this.textSRP.Location = new System.Drawing.Point(154, 24);
			this.textSRP.MaxVal = 255;
			this.textSRP.MinVal = 0;
			this.textSRP.Name = "textSRP";
			this.textSRP.ShowZero = false;
			this.textSRP.Size = new System.Drawing.Size(39, 20);
			this.textSRP.TabIndex = 0;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(48, 23);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(104, 21);
			this.label20.TabIndex = 168;
			this.label20.Text = "SRP";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupProsthodontics
			// 
			this.groupProsthodontics.Controls.Add(this.comboDentures);
			this.groupProsthodontics.Controls.Add(this.label24);
			this.groupProsthodontics.Controls.Add(this.textDentures);
			this.groupProsthodontics.Controls.Add(this.label25);
			this.groupProsthodontics.Location = new System.Drawing.Point(46, 388);
			this.groupProsthodontics.Name = "groupProsthodontics";
			this.groupProsthodontics.Size = new System.Drawing.Size(341, 50);
			this.groupProsthodontics.TabIndex = 4;
			this.groupProsthodontics.Text = "Prosthodontics";
			// 
			// comboDentures
			// 
			this.comboDentures.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboDentures.Location = new System.Drawing.Point(197, 23);
			this.comboDentures.Name = "comboDentures";
			this.comboDentures.Size = new System.Drawing.Size(136, 21);
			this.comboDentures.TabIndex = 1;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(156, 9);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(39, 12);
			this.label24.TabIndex = 170;
			this.label24.Text = "#";
			this.label24.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textDentures
			// 
			this.textDentures.Location = new System.Drawing.Point(154, 23);
			this.textDentures.MaxVal = 255;
			this.textDentures.MinVal = 0;
			this.textDentures.Name = "textDentures";
			this.textDentures.ShowZero = false;
			this.textDentures.Size = new System.Drawing.Size(39, 20);
			this.textDentures.TabIndex = 0;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(45, 22);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(107, 21);
			this.label25.TabIndex = 168;
			this.label25.Text = "Dentures/Partials";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupImplants
			// 
			this.groupImplants.Controls.Add(this.comboImplant);
			this.groupImplants.Controls.Add(this.textImplant);
			this.groupImplants.Controls.Add(this.label26);
			this.groupImplants.Controls.Add(this.label29);
			this.groupImplants.Location = new System.Drawing.Point(46, 445);
			this.groupImplants.Name = "groupImplants";
			this.groupImplants.Size = new System.Drawing.Size(341, 50);
			this.groupImplants.TabIndex = 5;
			this.groupImplants.Text = "Implants";
			// 
			// comboImplant
			// 
			this.comboImplant.Items.AddList<object>(new object[] {
			"Every # Years",
			"# Per Benefit Year",
			"Every # Months",
			"# in Last 12 Months"});
			this.comboImplant.Location = new System.Drawing.Point(197, 23);
			this.comboImplant.Name = "comboImplant";
			this.comboImplant.Size = new System.Drawing.Size(136, 21);
			this.comboImplant.TabIndex = 1;
			// 
			// textImplant
			// 
			this.textImplant.Location = new System.Drawing.Point(154, 23);
			this.textImplant.MaxVal = 255;
			this.textImplant.MinVal = 0;
			this.textImplant.Name = "textImplant";
			this.textImplant.ShowZero = false;
			this.textImplant.Size = new System.Drawing.Size(39, 20);
			this.textImplant.TabIndex = 0;
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(45, 22);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(107, 21);
			this.label26.TabIndex = 179;
			this.label26.Text = "Implant";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(156, 9);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(39, 12);
			this.label29.TabIndex = 170;
			this.label29.Text = "#";
			this.label29.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// FormBenefitFrequencies
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(445, 547);
			this.Controls.Add(this.groupPreventive);
			this.Controls.Add(this.groupRestorative);
			this.Controls.Add(this.groupPerio);
			this.Controls.Add(this.groupProsthodontics);
			this.Controls.Add(this.groupImplants);
			this.Controls.Add(this.groupDiagnostic);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBenefitFrequencies";
			this.Text = "Benefit Frequencies";
			this.Load += new System.EventHandler(this.FormBenefitFrequencies_Load);
			this.groupDiagnostic.ResumeLayout(false);
			this.groupDiagnostic.PerformLayout();
			this.groupPreventive.ResumeLayout(false);
			this.groupPreventive.PerformLayout();
			this.groupRestorative.ResumeLayout(false);
			this.groupRestorative.PerformLayout();
			this.groupPerio.ResumeLayout(false);
			this.groupPerio.PerformLayout();
			this.groupProsthodontics.ResumeLayout(false);
			this.groupProsthodontics.PerformLayout();
			this.groupImplants.ResumeLayout(false);
			this.groupImplants.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GroupBoxOD groupDiagnostic;
		private OpenDental.UI.ComboBoxOD comboCancerScreenings;
		private ValidNum textCancerScreenings;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ComboBoxOD comboExams;
		private ValidNum textExams;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.ComboBoxOD comboPano;
		private ValidNum textPano;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.ComboBoxOD comboBW;
		private System.Windows.Forms.Label label6;
		private ValidNum textBW;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.GroupBoxOD groupPreventive;
		private OpenDental.UI.ComboBoxOD comboSealants;
		private ValidNum textSealants;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ComboBoxOD comboFlouride;
		private ValidNum textFlouride;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.ComboBoxOD comboProphy;
		private System.Windows.Forms.Label label9;
		private ValidNum textProphy;
		private System.Windows.Forms.Label label10;
		private OpenDental.UI.GroupBoxOD groupRestorative;
		private OpenDental.UI.ComboBoxOD comboCrown;
		private System.Windows.Forms.Label label14;
		private ValidNum textCrown;
		private System.Windows.Forms.Label label15;
		private OpenDental.UI.GroupBoxOD groupPerio;
		private OpenDental.UI.ComboBoxOD comboPerioMaint;
		private ValidNum textPerioMaint;
		private System.Windows.Forms.Label label17;
		private OpenDental.UI.ComboBoxOD comboDebridement;
		private ValidNum textDebridement;
		private System.Windows.Forms.Label label18;
		private OpenDental.UI.ComboBoxOD comboSRP;
		private System.Windows.Forms.Label label19;
		private ValidNum textSRP;
		private System.Windows.Forms.Label label20;
		private OpenDental.UI.GroupBoxOD groupProsthodontics;
		private OpenDental.UI.ComboBoxOD comboDentures;
		private System.Windows.Forms.Label label24;
		private ValidNum textDentures;
		private System.Windows.Forms.Label label25;
		private OpenDental.UI.GroupBoxOD groupImplants;
		private OpenDental.UI.ComboBoxOD comboImplant;
		private ValidNum textImplant;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label29;
	}
}