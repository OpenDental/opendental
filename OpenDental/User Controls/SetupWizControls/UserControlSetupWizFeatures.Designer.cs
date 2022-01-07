namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizFeatures {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlSetupWizFeatures));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelExplanation = new System.Windows.Forms.Label();
			this.panelInfo = new System.Windows.Forms.Panel();
			this.labelInfoMedIns = new System.Windows.Forms.Label();
			this.labelInfoEHR = new System.Windows.Forms.Label();
			this.labelInfoClinical = new System.Windows.Forms.Label();
			this.labelInfoClinics = new System.Windows.Forms.Label();
			this.labelInfoInsurance = new System.Windows.Forms.Label();
			this.labelInfoCapitation = new System.Windows.Forms.Label();
			this.labelInfoMedicaid = new System.Windows.Forms.Label();
			this.labelFieldType = new System.Windows.Forms.Label();
			this.checkEhr = new System.Windows.Forms.CheckBox();
			this.checkMedicalIns = new System.Windows.Forms.CheckBox();
			this.checkInsurance = new System.Windows.Forms.CheckBox();
			this.checkNoClinics = new System.Windows.Forms.CheckBox();
			this.checkClinical = new System.Windows.Forms.CheckBox();
			this.checkMedicaid = new System.Windows.Forms.CheckBox();
			this.checkCapitation = new System.Windows.Forms.CheckBox();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.panelInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "iButton_Blue.png");
			this.imageList1.Images.SetKeyName(1, "iButton_Green.png");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelExplanation);
			this.groupBox1.Location = new System.Drawing.Point(324, 136);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(542, 229);
			this.groupBox1.TabIndex = 109;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Feature Information";
			// 
			// labelExplanation
			// 
			this.labelExplanation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExplanation.ImageKey = "(none)";
			this.labelExplanation.Location = new System.Drawing.Point(3, 16);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(536, 210);
			this.labelExplanation.TabIndex = 106;
			this.labelExplanation.Text = "Click on an info icon to view information about the item clicked.";
			this.labelExplanation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelInfo
			// 
			this.panelInfo.Controls.Add(this.labelInfoMedIns);
			this.panelInfo.Controls.Add(this.labelInfoEHR);
			this.panelInfo.Controls.Add(this.labelInfoClinical);
			this.panelInfo.Controls.Add(this.labelInfoClinics);
			this.panelInfo.Controls.Add(this.labelInfoInsurance);
			this.panelInfo.Controls.Add(this.labelInfoCapitation);
			this.panelInfo.Controls.Add(this.labelInfoMedicaid);
			this.panelInfo.Location = new System.Drawing.Point(293, 89);
			this.panelInfo.Name = "panelInfo";
			this.panelInfo.Size = new System.Drawing.Size(23, 358);
			this.panelInfo.TabIndex = 108;
			// 
			// labelInfoMedIns
			// 
			this.labelInfoMedIns.ImageIndex = 0;
			this.labelInfoMedIns.ImageList = this.imageList1;
			this.labelInfoMedIns.Location = new System.Drawing.Point(1, 277);
			this.labelInfoMedIns.Name = "labelInfoMedIns";
			this.labelInfoMedIns.Size = new System.Drawing.Size(20, 20);
			this.labelInfoMedIns.TabIndex = 100;
			this.labelInfoMedIns.Tag = resources.GetString("labelInfoMedIns.Tag");
			this.labelInfoMedIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoMedIns.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoEHR
			// 
			this.labelInfoEHR.ImageIndex = 0;
			this.labelInfoEHR.ImageList = this.imageList1;
			this.labelInfoEHR.Location = new System.Drawing.Point(1, 332);
			this.labelInfoEHR.Name = "labelInfoEHR";
			this.labelInfoEHR.Size = new System.Drawing.Size(20, 20);
			this.labelInfoEHR.TabIndex = 101;
			this.labelInfoEHR.Tag = resources.GetString("labelInfoEHR.Tag");
			this.labelInfoEHR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoEHR.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoClinical
			// 
			this.labelInfoClinical.ImageIndex = 0;
			this.labelInfoClinical.ImageList = this.imageList1;
			this.labelInfoClinical.Location = new System.Drawing.Point(1, 167);
			this.labelInfoClinical.Name = "labelInfoClinical";
			this.labelInfoClinical.Size = new System.Drawing.Size(20, 20);
			this.labelInfoClinical.TabIndex = 96;
			this.labelInfoClinical.Tag = resources.GetString("labelInfoClinical.Tag");
			this.labelInfoClinical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoClinical.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoClinics
			// 
			this.labelInfoClinics.ImageIndex = 0;
			this.labelInfoClinics.ImageList = this.imageList1;
			this.labelInfoClinics.Location = new System.Drawing.Point(1, 222);
			this.labelInfoClinics.Name = "labelInfoClinics";
			this.labelInfoClinics.Size = new System.Drawing.Size(20, 20);
			this.labelInfoClinics.TabIndex = 98;
			this.labelInfoClinics.Tag = "Turn on Clinics (e.g. for practices with multiple locations). \r\nClinics can be us" +
    "ed when you have multiple locations. Once clinics are set up, you can assign cli" +
    "nics throughout Open Dental. ";
			this.labelInfoClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoClinics.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoInsurance
			// 
			this.labelInfoInsurance.ImageIndex = 0;
			this.labelInfoInsurance.ImageList = this.imageList1;
			this.labelInfoInsurance.Location = new System.Drawing.Point(1, 112);
			this.labelInfoInsurance.Name = "labelInfoInsurance";
			this.labelInfoInsurance.Size = new System.Drawing.Size(20, 20);
			this.labelInfoInsurance.TabIndex = 95;
			this.labelInfoInsurance.Tag = "Turns on All Insurance. \r\nThis feature will show insurance information in the Fam" +
    "ily module and insurance estimates in the Treatment Plan module. ";
			this.labelInfoInsurance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoInsurance.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoCapitation
			// 
			this.labelInfoCapitation.ImageIndex = 0;
			this.labelInfoCapitation.ImageList = this.imageList1;
			this.labelInfoCapitation.Location = new System.Drawing.Point(1, 2);
			this.labelInfoCapitation.Name = "labelInfoCapitation";
			this.labelInfoCapitation.Size = new System.Drawing.Size(20, 20);
			this.labelInfoCapitation.TabIndex = 90;
			this.labelInfoCapitation.Tag = resources.GetString("labelInfoCapitation.Tag");
			this.labelInfoCapitation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoCapitation.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelInfoMedicaid
			// 
			this.labelInfoMedicaid.ImageIndex = 0;
			this.labelInfoMedicaid.ImageList = this.imageList1;
			this.labelInfoMedicaid.Location = new System.Drawing.Point(1, 57);
			this.labelInfoMedicaid.Name = "labelInfoMedicaid";
			this.labelInfoMedicaid.Size = new System.Drawing.Size(20, 20);
			this.labelInfoMedicaid.TabIndex = 91;
			this.labelInfoMedicaid.Tag = resources.GetString("labelInfoMedicaid.Tag");
			this.labelInfoMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelInfoMedicaid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelInfo_MouseClick);
			// 
			// labelFieldType
			// 
			this.labelFieldType.Location = new System.Drawing.Point(28, 57);
			this.labelFieldType.Name = "labelFieldType";
			this.labelFieldType.Size = new System.Drawing.Size(288, 29);
			this.labelFieldType.TabIndex = 89;
			this.labelFieldType.Text = "Check the boxes that apply to your practice. \r\nThese settings will apply to all c" +
    "omputers.";
			this.labelFieldType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEhr
			// 
			this.checkEhr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEhr.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEhr.Location = new System.Drawing.Point(6, 421);
			this.checkEhr.Name = "checkEhr";
			this.checkEhr.Size = new System.Drawing.Size(285, 20);
			this.checkEhr.TabIndex = 32;
			this.checkEhr.Text = "EHR";
			this.checkEhr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMedicalIns
			// 
			this.checkMedicalIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicalIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicalIns.Location = new System.Drawing.Point(6, 366);
			this.checkMedicalIns.Name = "checkMedicalIns";
			this.checkMedicalIns.Size = new System.Drawing.Size(285, 20);
			this.checkMedicalIns.TabIndex = 31;
			this.checkMedicalIns.Text = "Medical Insurance";
			this.checkMedicalIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsurance
			// 
			this.checkInsurance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurance.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsurance.Location = new System.Drawing.Point(6, 201);
			this.checkInsurance.Name = "checkInsurance";
			this.checkInsurance.Size = new System.Drawing.Size(285, 20);
			this.checkInsurance.TabIndex = 29;
			this.checkInsurance.Text = "All Insurance";
			this.checkInsurance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNoClinics
			// 
			this.checkNoClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoClinics.Location = new System.Drawing.Point(6, 311);
			this.checkNoClinics.Name = "checkNoClinics";
			this.checkNoClinics.Size = new System.Drawing.Size(285, 20);
			this.checkNoClinics.TabIndex = 26;
			this.checkNoClinics.Text = "Clinics (multiple office locations)";
			this.checkNoClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClinical
			// 
			this.checkClinical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinical.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinical.Location = new System.Drawing.Point(6, 256);
			this.checkClinical.Name = "checkClinical";
			this.checkClinical.Size = new System.Drawing.Size(285, 20);
			this.checkClinical.TabIndex = 23;
			this.checkClinical.Text = "Clinical (computers in operatories)";
			this.checkClinical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMedicaid
			// 
			this.checkMedicaid.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicaid.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicaid.Location = new System.Drawing.Point(6, 146);
			this.checkMedicaid.Name = "checkMedicaid";
			this.checkMedicaid.Size = new System.Drawing.Size(285, 20);
			this.checkMedicaid.TabIndex = 21;
			this.checkMedicaid.Text = "Medicaid";
			this.checkMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkCapitation
			// 
			this.checkCapitation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCapitation.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCapitation.Location = new System.Drawing.Point(6, 91);
			this.checkCapitation.Name = "checkCapitation";
			this.checkCapitation.Size = new System.Drawing.Size(285, 20);
			this.checkCapitation.TabIndex = 20;
			this.checkCapitation.Text = "Capitation";
			this.checkCapitation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAdvanced
			// 
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdvanced.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdvanced.Location = new System.Drawing.Point(845, 456);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(82, 26);
			this.butAdvanced.TabIndex = 111;
			this.butAdvanced.Text = "Advanced";
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(631, 414);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 39);
			this.label1.TabIndex = 110;
			this.label1.Text = "Further modifications to this list can be made by going to Setup -> Advanced Setu" +
    "p -> Show Features, or by clicking \"Advanced\".";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UserControlSetupWizFeatures
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panelInfo);
			this.Controls.Add(this.labelFieldType);
			this.Controls.Add(this.checkEhr);
			this.Controls.Add(this.checkMedicalIns);
			this.Controls.Add(this.checkInsurance);
			this.Controls.Add(this.checkNoClinics);
			this.Controls.Add(this.checkClinical);
			this.Controls.Add(this.checkMedicaid);
			this.Controls.Add(this.checkCapitation);
			this.Name = "UserControlSetupWizFeatures";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizFeatures_Load);
			this.groupBox1.ResumeLayout(false);
			this.panelInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox checkEhr;
		private System.Windows.Forms.CheckBox checkMedicalIns;
		private System.Windows.Forms.CheckBox checkInsurance;
		private System.Windows.Forms.CheckBox checkNoClinics;
		private System.Windows.Forms.CheckBox checkClinical;
		private System.Windows.Forms.CheckBox checkMedicaid;
		private System.Windows.Forms.CheckBox checkCapitation;
		private System.Windows.Forms.Label labelFieldType;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label labelInfoCapitation;
		private System.Windows.Forms.Label labelInfoMedicaid;
		private System.Windows.Forms.Label labelExplanation;
		private System.Windows.Forms.Panel panelInfo;
		private System.Windows.Forms.Label labelInfoMedIns;
		private System.Windows.Forms.Label labelInfoEHR;
		private System.Windows.Forms.Label labelInfoClinical;
		private System.Windows.Forms.Label labelInfoClinics;
		private System.Windows.Forms.Label labelInfoInsurance;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butAdvanced;
		private System.Windows.Forms.Label label1;
	}
}
