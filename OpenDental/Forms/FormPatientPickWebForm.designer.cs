namespace OpenDental{
	partial class FormPatientPickWebForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientPickWebForm));
			this.butCancel = new OpenDental.UI.Button();
			this.labelExplanation = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupWebFormInfo = new System.Windows.Forms.GroupBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.textClinic = new System.Windows.Forms.TextBox();
			this.butView = new OpenDental.UI.Button();
			this.textBirthdate = new OpenDental.ValidDate();
			this.label9 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.butSelect = new OpenDental.UI.Button();
			this.butNew = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.butSkip = new OpenDental.UI.Button();
			this.butDiscard = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupWebFormInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(539, 451);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelExplanation
			// 
			this.labelExplanation.Location = new System.Drawing.Point(12, 5);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(597, 42);
			this.labelExplanation.TabIndex = 4;
			this.labelExplanation.Text = "An exact matching patient could not be found for this submitted web form.";
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(15, 183);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(483, 161);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Close Matches";
			this.gridMain.TranslationName = "TableCloseMatch";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupWebFormInfo
			// 
			this.groupWebFormInfo.Controls.Add(this.labelClinic);
			this.groupWebFormInfo.Controls.Add(this.textClinic);
			this.groupWebFormInfo.Controls.Add(this.butView);
			this.groupWebFormInfo.Controls.Add(this.textBirthdate);
			this.groupWebFormInfo.Controls.Add(this.label9);
			this.groupWebFormInfo.Controls.Add(this.textFName);
			this.groupWebFormInfo.Controls.Add(this.textLName);
			this.groupWebFormInfo.Controls.Add(this.label3);
			this.groupWebFormInfo.Controls.Add(this.label2);
			this.groupWebFormInfo.Location = new System.Drawing.Point(15, 28);
			this.groupWebFormInfo.Name = "groupWebFormInfo";
			this.groupWebFormInfo.Size = new System.Drawing.Size(350, 134);
			this.groupWebFormInfo.TabIndex = 6;
			this.groupWebFormInfo.TabStop = false;
			this.groupWebFormInfo.Text = "Web Form Info";
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(6, 86);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(105, 14);
			this.labelClinic.TabIndex = 15;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelClinic.Visible = false;
			// 
			// textClinic
			// 
			this.textClinic.Enabled = false;
			this.textClinic.Location = new System.Drawing.Point(114, 83);
			this.textClinic.MaxLength = 100;
			this.textClinic.Name = "textClinic";
			this.textClinic.ReadOnly = true;
			this.textClinic.Size = new System.Drawing.Size(228, 20);
			this.textClinic.TabIndex = 14;
			this.textClinic.Visible = false;
			// 
			// butView
			// 
			this.butView.Location = new System.Drawing.Point(114, 106);
			this.butView.Name = "butView";
			this.butView.Size = new System.Drawing.Size(82, 24);
			this.butView.TabIndex = 13;
			this.butView.Text = "Preview";
			this.butView.Click += new System.EventHandler(this.butView_Click);
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(114, 62);
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.ReadOnly = true;
			this.textBirthdate.Size = new System.Drawing.Size(82, 20);
			this.textBirthdate.TabIndex = 10;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(7, 66);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(105, 14);
			this.label9.TabIndex = 9;
			this.label9.Text = "Birthdate";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(114, 41);
			this.textFName.MaxLength = 100;
			this.textFName.Name = "textFName";
			this.textFName.ReadOnly = true;
			this.textFName.Size = new System.Drawing.Size(228, 20);
			this.textFName.TabIndex = 5;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(114, 20);
			this.textLName.MaxLength = 100;
			this.textLName.Name = "textLName";
			this.textLName.ReadOnly = true;
			this.textLName.Size = new System.Drawing.Size(228, 20);
			this.textLName.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(108, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "First Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Last Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 165);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(389, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "You can choose a patient by double clicking in this list";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 350);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(281, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "Or, you can select a patient from the main list.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSelect
			// 
			this.butSelect.Location = new System.Drawing.Point(15, 374);
			this.butSelect.Name = "butSelect";
			this.butSelect.Size = new System.Drawing.Size(75, 24);
			this.butSelect.TabIndex = 9;
			this.butSelect.Text = "Select";
			this.butSelect.Click += new System.EventHandler(this.butSelect_Click);
			// 
			// butNew
			// 
			this.butNew.Location = new System.Drawing.Point(15, 434);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(75, 24);
			this.butNew.TabIndex = 11;
			this.butNew.Text = "New";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 410);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(389, 20);
			this.label6.TabIndex = 10;
			this.label6.Text = "Or, you can create a new patient to attach this form to.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSkip
			// 
			this.butSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSkip.Location = new System.Drawing.Point(444, 451);
			this.butSkip.Name = "butSkip";
			this.butSkip.Size = new System.Drawing.Size(75, 24);
			this.butSkip.TabIndex = 12;
			this.butSkip.Text = "&Skip";
			this.butSkip.Click += new System.EventHandler(this.butSkip_Click);
			// 
			// butDiscard
			// 
			this.butDiscard.Location = new System.Drawing.Point(302, 374);
			this.butDiscard.Name = "butDiscard";
			this.butDiscard.Size = new System.Drawing.Size(75, 24);
			this.butDiscard.TabIndex = 13;
			this.butDiscard.Text = "Discard All";
			this.butDiscard.Click += new System.EventHandler(this.butDiscard_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(299, 350);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(310, 20);
			this.label1.TabIndex = 14;
			this.label1.Text = "Or, you can discard all web forms for this patient.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormPatientPickWebForm
			// 
			this.ClientSize = new System.Drawing.Size(626, 487);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butSkip);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDiscard);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butSelect);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupWebFormInfo);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelExplanation);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientPickWebForm";
			this.Text = "Pick Patient for Web Form";
			this.Load += new System.EventHandler(this.FormPatientPickWebForm_Load);
			this.groupWebFormInfo.ResumeLayout(false);
			this.groupWebFormInfo.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelExplanation;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupWebFormInfo;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private ValidDate textBirthdate;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private UI.Button butSelect;
		private UI.Button butNew;
		private System.Windows.Forms.Label label6;
		private UI.Button butSkip;
		private UI.Button butView;
		private UI.Button butDiscard;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.TextBox textClinic;
	}
}