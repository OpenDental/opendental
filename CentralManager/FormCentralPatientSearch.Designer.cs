namespace CentralManager {
	partial class FormCentralPatientSearch {
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
			this.gridPats = new OpenDental.UI.GridOD();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkGuarantors = new System.Windows.Forms.CheckBox();
			this.textConn = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textCountry = new System.Windows.Forms.TextBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textSubscriberID = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textBirthdate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkHideArchived = new System.Windows.Forms.CheckBox();
			this.textChartNumber = new System.Windows.Forms.TextBox();
			this.textSSN = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkHideInactive = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.label4 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textLName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSearch = new OpenDental.UI.Button();
			this.checkLimit = new System.Windows.Forms.CheckBox();
			this.labelFetch = new System.Windows.Forms.Label();
			this.butClose = new OpenDental.UI.Button();
			this.buttonOK = new OpenDental.UI.Button();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridPats
			// 
			this.gridPats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPats.HScrollVisible = true;
			this.gridPats.Location = new System.Drawing.Point(0, 0);
			this.gridPats.Name = "gridPats";
			this.gridPats.Size = new System.Drawing.Size(501, 567);
			this.gridPats.TabIndex = 10;
			this.gridPats.Title = "Patients - Double Click to Launch Connection";
			this.gridPats.TranslationName = "FormPatientSelect";
			this.gridPats.WrapText = false;
			this.gridPats.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPats_CellDoubleClick);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.checkGuarantors);
			this.groupBox2.Controls.Add(this.textConn);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.textCountry);
			this.groupBox2.Controls.Add(this.labelCountry);
			this.groupBox2.Controls.Add(this.textEmail);
			this.groupBox2.Controls.Add(this.labelEmail);
			this.groupBox2.Controls.Add(this.textSubscriberID);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.textBirthdate);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.checkHideArchived);
			this.groupBox2.Controls.Add(this.textChartNumber);
			this.groupBox2.Controls.Add(this.textSSN);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.textState);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textCity);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.checkHideInactive);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textAddress);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textPhone);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textFName);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textLName);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(507, 45);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(220, 371);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Search by:";
			// 
			// checkGuarantors
			// 
			this.checkGuarantors.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGuarantors.Location = new System.Drawing.Point(11, 316);
			this.checkGuarantors.Name = "checkGuarantors";
			this.checkGuarantors.Size = new System.Drawing.Size(202, 16);
			this.checkGuarantors.TabIndex = 49;
			this.checkGuarantors.Text = "Show Guarantors Only";
			// 
			// textConn
			// 
			this.textConn.Location = new System.Drawing.Point(123, 292);
			this.textConn.Name = "textConn";
			this.textConn.Size = new System.Drawing.Size(90, 20);
			this.textConn.TabIndex = 47;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(11, 293);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(113, 17);
			this.label14.TabIndex = 48;
			this.label14.Text = "Connection";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCountry
			// 
			this.textCountry.Location = new System.Drawing.Point(123, 272);
			this.textCountry.Name = "textCountry";
			this.textCountry.Size = new System.Drawing.Size(90, 20);
			this.textCountry.TabIndex = 12;
			// 
			// labelCountry
			// 
			this.labelCountry.Location = new System.Drawing.Point(11, 273);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(113, 17);
			this.labelCountry.TabIndex = 46;
			this.labelCountry.Text = "Country";
			this.labelCountry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(123, 252);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(90, 20);
			this.textEmail.TabIndex = 11;
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(11, 256);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(113, 12);
			this.labelEmail.TabIndex = 43;
			this.labelEmail.Text = "E-mail";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubscriberID
			// 
			this.textSubscriberID.Location = new System.Drawing.Point(123, 232);
			this.textSubscriberID.Name = "textSubscriberID";
			this.textSubscriberID.Size = new System.Drawing.Size(90, 20);
			this.textSubscriberID.TabIndex = 10;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(11, 236);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(113, 12);
			this.label13.TabIndex = 41;
			this.label13.Text = "Subscriber ID";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(123, 212);
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.Size = new System.Drawing.Size(90, 20);
			this.textBirthdate.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 216);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 12);
			this.label2.TabIndex = 27;
			this.label2.Text = "Birthdate";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHideArchived
			// 
			this.checkHideArchived.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideArchived.Location = new System.Drawing.Point(11, 347);
			this.checkHideArchived.Name = "checkHideArchived";
			this.checkHideArchived.Size = new System.Drawing.Size(203, 16);
			this.checkHideArchived.TabIndex = 25;
			this.checkHideArchived.Text = "Hide Archived/Deceased";
			// 
			// textChartNumber
			// 
			this.textChartNumber.Location = new System.Drawing.Point(123, 192);
			this.textChartNumber.Name = "textChartNumber";
			this.textChartNumber.Size = new System.Drawing.Size(90, 20);
			this.textChartNumber.TabIndex = 8;
			// 
			// textSSN
			// 
			this.textSSN.Location = new System.Drawing.Point(123, 152);
			this.textSSN.Name = "textSSN";
			this.textSSN.Size = new System.Drawing.Size(90, 20);
			this.textSSN.TabIndex = 6;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(11, 156);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(112, 12);
			this.label12.TabIndex = 24;
			this.label12.Text = "SSN";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11, 196);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(113, 12);
			this.label10.TabIndex = 20;
			this.label10.Text = "Chart Number";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(123, 172);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(90, 20);
			this.textPatNum.TabIndex = 7;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(11, 176);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(113, 12);
			this.label9.TabIndex = 18;
			this.label9.Text = "Patient Number";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(123, 132);
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(90, 20);
			this.textState.TabIndex = 5;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(11, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(111, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "State";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(123, 112);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(90, 20);
			this.textCity.TabIndex = 4;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(11, 114);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(109, 14);
			this.label7.TabIndex = 14;
			this.label7.Text = "City";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHideInactive
			// 
			this.checkHideInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideInactive.Location = new System.Drawing.Point(11, 331);
			this.checkHideInactive.Name = "checkHideInactive";
			this.checkHideInactive.Size = new System.Drawing.Size(202, 16);
			this.checkHideInactive.TabIndex = 44;
			this.checkHideInactive.Text = "Hide Inactive Patients";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(11, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(202, 14);
			this.label6.TabIndex = 10;
			this.label6.Text = "Hint: enter values in multiple boxes.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(123, 92);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(90, 20);
			this.textAddress.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 95);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "Address";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(123, 72);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(90, 20);
			this.textPhone.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(11, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(112, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Phone (any)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(123, 52);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(90, 20);
			this.textFName.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(111, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "First Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(123, 32);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(90, 20);
			this.textLName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSearch
			// 
			this.butSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearch.Location = new System.Drawing.Point(652, 457);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 24);
			this.butSearch.TabIndex = 34;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearchPats_Click);
			// 
			// checkLimit
			// 
			this.checkLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkLimit.Checked = true;
			this.checkLimit.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkLimit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLimit.Location = new System.Drawing.Point(519, 430);
			this.checkLimit.Name = "checkLimit";
			this.checkLimit.Size = new System.Drawing.Size(208, 16);
			this.checkLimit.TabIndex = 50;
			this.checkLimit.Text = "Limit 30 patients per connection";
			// 
			// labelFetch
			// 
			this.labelFetch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFetch.BackColor = System.Drawing.Color.Transparent;
			this.labelFetch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFetch.ForeColor = System.Drawing.Color.Red;
			this.labelFetch.Location = new System.Drawing.Point(507, 458);
			this.labelFetch.Name = "labelFetch";
			this.labelFetch.Size = new System.Drawing.Size(139, 17);
			this.labelFetch.TabIndex = 51;
			this.labelFetch.Text = "Fetching Results...";
			this.labelFetch.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(652, 532);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 52;
			this.butClose.Text = "Cancel";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(652, 502);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 24);
			this.buttonOK.TabIndex = 53;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormCentralPatientSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(739, 566);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelFetch);
			this.Controls.Add(this.checkLimit);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.gridPats);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCentralPatientSearch";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Search Patients";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCentralPatientSearch_FormClosing);
			this.Load += new System.EventHandler(this.FormCentralPatientSearch_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridPats;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textCountry;
		private System.Windows.Forms.Label labelCountry;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.TextBox textSubscriberID;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBirthdate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkHideArchived;
		private System.Windows.Forms.TextBox textChartNumber;
		private System.Windows.Forms.TextBox textSSN;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkHideInactive;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.Label label5;
		private OpenDental.ValidPhone textPhone;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butSearch;
		private System.Windows.Forms.TextBox textConn;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckBox checkGuarantors;
		private System.Windows.Forms.CheckBox checkLimit;
		private System.Windows.Forms.Label labelFetch;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button buttonOK;
	}
}