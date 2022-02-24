using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormTrojanCollect : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butHelp;
		private Label label1;
		private Label labelGuarantor;
		private Label labelAddress;
		private Label labelCityStZip;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label labelSSN;
		private Label labelDOB;
		private Label labelPhone;
		private Label label10;
		private Label labelEmpPhone;
		private Label labelEmployer;
		private Label label13;
		private Label labelPatient;
		private Label label15;
		private Label label16;
		private RadioButton radioDiplomatic;
		private RadioButton radioFirm;
		private RadioButton radioSkip;
		private IContainer components;
		private Patient _patCur;
		private Patient _guarCur;
		private ValidDate textDate;
		private GroupBox groupTransactionType;
		private ValidDouble textAmount;
		private UI.MenuOD menuMain;
		private Employer _empCur;

		///<summary></summary>
		public FormTrojanCollect(Patient pat) {
			_patCur=pat;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Clean up any resources being used.</summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		///<summary>Required method for Designer support - do not modify the contents of this method with the code editor.</summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrojanCollect));
			this.label1 = new System.Windows.Forms.Label();
			this.labelGuarantor = new System.Windows.Forms.Label();
			this.labelAddress = new System.Windows.Forms.Label();
			this.labelCityStZip = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labelSSN = new System.Windows.Forms.Label();
			this.labelDOB = new System.Windows.Forms.Label();
			this.labelPhone = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.labelEmpPhone = new System.Windows.Forms.Label();
			this.labelEmployer = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.labelPatient = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.radioDiplomatic = new System.Windows.Forms.RadioButton();
			this.radioFirm = new System.Windows.Forms.RadioButton();
			this.radioSkip = new System.Windows.Forms.RadioButton();
			this.butHelp = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDate = new OpenDental.ValidDate();
			this.groupTransactionType = new System.Windows.Forms.GroupBox();
			this.textAmount = new OpenDental.ValidDouble();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.groupTransactionType.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 18);
			this.label1.TabIndex = 3;
			this.label1.Text = "Financially Responsible Person:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelGuarantor
			// 
			this.labelGuarantor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelGuarantor.Location = new System.Drawing.Point(18, 63);
			this.labelGuarantor.Name = "labelGuarantor";
			this.labelGuarantor.Size = new System.Drawing.Size(202, 18);
			this.labelGuarantor.TabIndex = 4;
			this.labelGuarantor.Text = "Joe Smith";
			this.labelGuarantor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAddress
			// 
			this.labelAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAddress.Location = new System.Drawing.Point(18, 81);
			this.labelAddress.Name = "labelAddress";
			this.labelAddress.Size = new System.Drawing.Size(202, 18);
			this.labelAddress.TabIndex = 5;
			this.labelAddress.Text = "123 E St.";
			this.labelAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCityStZip
			// 
			this.labelCityStZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCityStZip.Location = new System.Drawing.Point(18, 99);
			this.labelCityStZip.Name = "labelCityStZip";
			this.labelCityStZip.Size = new System.Drawing.Size(202, 18);
			this.labelCityStZip.TabIndex = 6;
			this.labelCityStZip.Text = "Los Angeles, CA 20212";
			this.labelCityStZip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(227, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 18);
			this.label4.TabIndex = 7;
			this.label4.Text = "SS#:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(227, 81);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 18);
			this.label5.TabIndex = 8;
			this.label5.Text = "DOB:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(227, 99);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 18);
			this.label6.TabIndex = 9;
			this.label6.Text = "Phone:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSSN
			// 
			this.labelSSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSSN.Location = new System.Drawing.Point(328, 63);
			this.labelSSN.Name = "labelSSN";
			this.labelSSN.Size = new System.Drawing.Size(154, 18);
			this.labelSSN.TabIndex = 10;
			this.labelSSN.Text = "123-12-1234";
			this.labelSSN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDOB
			// 
			this.labelDOB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDOB.Location = new System.Drawing.Point(328, 81);
			this.labelDOB.Name = "labelDOB";
			this.labelDOB.Size = new System.Drawing.Size(154, 18);
			this.labelDOB.TabIndex = 11;
			this.labelDOB.Text = "01/10/1980";
			this.labelDOB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPhone
			// 
			this.labelPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPhone.Location = new System.Drawing.Point(328, 99);
			this.labelPhone.Name = "labelPhone";
			this.labelPhone.Size = new System.Drawing.Size(154, 18);
			this.labelPhone.TabIndex = 12;
			this.labelPhone.Text = "(310)555-1212";
			this.labelPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 123);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(208, 18);
			this.label10.TabIndex = 13;
			this.label10.Text = "Employer:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelEmpPhone
			// 
			this.labelEmpPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEmpPhone.Location = new System.Drawing.Point(18, 163);
			this.labelEmpPhone.Name = "labelEmpPhone";
			this.labelEmpPhone.Size = new System.Drawing.Size(202, 18);
			this.labelEmpPhone.TabIndex = 15;
			this.labelEmpPhone.Text = "(310)665-5544";
			this.labelEmpPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelEmployer
			// 
			this.labelEmployer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEmployer.Location = new System.Drawing.Point(18, 145);
			this.labelEmployer.Name = "labelEmployer";
			this.labelEmployer.Size = new System.Drawing.Size(202, 18);
			this.labelEmployer.TabIndex = 14;
			this.labelEmployer.Text = "Ace, Inc.";
			this.labelEmployer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(12, 187);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(208, 18);
			this.label13.TabIndex = 16;
			this.label13.Text = "Patient:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPatient
			// 
			this.labelPatient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPatient.Location = new System.Drawing.Point(18, 209);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(202, 18);
			this.labelPatient.TabIndex = 17;
			this.labelPatient.Text = "Mary Smith";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(227, 164);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(100, 18);
			this.label15.TabIndex = 19;
			this.label15.Text = "Amount of debt";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(227, 144);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(100, 18);
			this.label16.TabIndex = 18;
			this.label16.Text = "Delinquency Date";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioDiplomatic
			// 
			this.radioDiplomatic.Checked = true;
			this.radioDiplomatic.Location = new System.Drawing.Point(9, 16);
			this.radioDiplomatic.Name = "radioDiplomatic";
			this.radioDiplomatic.Size = new System.Drawing.Size(81, 18);
			this.radioDiplomatic.TabIndex = 23;
			this.radioDiplomatic.TabStop = true;
			this.radioDiplomatic.Text = "Diplomatic";
			this.radioDiplomatic.UseVisualStyleBackColor = true;
			// 
			// radioFirm
			// 
			this.radioFirm.Location = new System.Drawing.Point(97, 16);
			this.radioFirm.Name = "radioFirm";
			this.radioFirm.Size = new System.Drawing.Size(53, 18);
			this.radioFirm.TabIndex = 24;
			this.radioFirm.Text = "Firm";
			this.radioFirm.UseVisualStyleBackColor = true;
			// 
			// radioSkip
			// 
			this.radioSkip.Location = new System.Drawing.Point(157, 16);
			this.radioSkip.Name = "radioSkip";
			this.radioSkip.Size = new System.Drawing.Size(50, 18);
			this.radioSkip.TabIndex = 25;
			this.radioSkip.Text = "Skip";
			this.radioSkip.UseVisualStyleBackColor = true;
			// 
			// butHelp
			// 
			this.butHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butHelp.Location = new System.Drawing.Point(263, 249);
			this.butHelp.Name = "butHelp";
			this.butHelp.Size = new System.Drawing.Size(75, 26);
			this.butHelp.TabIndex = 2;
			this.butHelp.Text = "Help";
			this.butHelp.Click += new System.EventHandler(this.butHelp_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butOK.Location = new System.Drawing.Point(12, 249);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(182, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "OK Send Transaction to Trojan";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(407, 249);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			// 
			// textDate
			// 
			this.textDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textDate.Location = new System.Drawing.Point(328, 143);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(92, 20);
			this.textDate.TabIndex = 26;
			this.textDate.Text = "01/25/2007";
			// 
			// groupTransactionType
			// 
			this.groupTransactionType.Controls.Add(this.radioDiplomatic);
			this.groupTransactionType.Controls.Add(this.radioFirm);
			this.groupTransactionType.Controls.Add(this.radioSkip);
			this.groupTransactionType.Location = new System.Drawing.Point(227, 192);
			this.groupTransactionType.Name = "groupTransactionType";
			this.groupTransactionType.Size = new System.Drawing.Size(255, 40);
			this.groupTransactionType.TabIndex = 27;
			this.groupTransactionType.TabStop = false;
			this.groupTransactionType.Text = "Transaction Type";
			// 
			// textAmount
			// 
			this.textAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textAmount.Location = new System.Drawing.Point(328, 163);
			this.textAmount.MaxVal = 9999999D;
			this.textAmount.MinVal = 25D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(70, 20);
			this.textAmount.TabIndex = 28;
			this.textAmount.Text = "123.45";
			this.textAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(494, 24);
			this.menuMain.TabIndex = 29;
			// 
			// FormTrojanCollect
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(494, 287);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.groupTransactionType);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.labelEmpPhone);
			this.Controls.Add(this.labelEmployer);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.labelPhone);
			this.Controls.Add(this.labelDOB);
			this.Controls.Add(this.labelSSN);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelCityStZip);
			this.Controls.Add(this.labelAddress);
			this.Controls.Add(this.labelGuarantor);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butHelp);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTrojanCollect";
			this.ShowInTaskbar = false;
			this.Text = "Send a Collection Transaction To Trojan";
			this.Load += new System.EventHandler(this.FormTrojanCollect_Load);
			this.groupTransactionType.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormTrojanCollect_Load(object sender,EventArgs e) {
			if(_patCur==null) {
				MsgBox.Show(this,"Please select a patient first.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_guarCur=Patients.GetPat(_patCur.Guarantor);
			if(_guarCur.EmployerNum>0) {
				_empCur=Employers.GetEmployer(_guarCur.EmployerNum);
			}
			if(_guarCur.LName.Length==0){
				MsgBox.Show(this,"Missing guarantor last name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.FName.Length==0) {
				MsgBox.Show(this,"Missing guarantor first name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Regex.IsMatch(_guarCur.SSN,@"^\d{9}$")) {
				MsgBox.Show(this,"Guarantor SSN must be exactly 9 digits.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.Address.Length==0) {
				MsgBox.Show(this,"Missing guarantor address.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.City.Length==0) {
				MsgBox.Show(this,"Missing guarantor city.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.State.Length!=2) {
				MsgBox.Show(this,"Guarantor state must be 2 characters.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.Zip.Length<5) {
				MsgBox.Show(this,"Invalid guarantor zip.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			labelGuarantor.Text=_guarCur.GetNameFL();
			labelAddress.Text=_guarCur.Address;
			if(!string.IsNullOrEmpty(_guarCur.Address2)){
				labelAddress.Text+=", "+_guarCur.Address2;
			}
			labelCityStZip.Text=_guarCur.City+", "+_guarCur.State+" "+_guarCur.Zip;
			labelSSN.Text=_guarCur.SSN.Substring(0,3)+"-"+_guarCur.SSN.Substring(3,2)+"-"+_guarCur.SSN.Substring(5,4);
			labelDOB.Text=_guarCur.Birthdate.Year<1880?"":_guarCur.Birthdate.ToShortDateString();
			labelPhone.Text=Clip(_guarCur.HmPhone,13);
			labelEmployer.Text=_empCur?.EmpName??"";
			labelEmpPhone.Text=_empCur?.Phone??"";
			labelPatient.Text=_patCur.GetNameFL();
			DateTime lastProcDate=TrojanQueries.GetMaxProcedureDate(_guarCur.PatNum);
			DateTime lastPayDate=TrojanQueries.GetMaxPaymentDate(_guarCur.PatNum);
			textDate.Text=(lastPayDate>lastProcDate?lastPayDate:lastProcDate).ToShortDateString();
			textAmount.Text=_guarCur.BalTotal.ToString("F2");
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using(FormTrojanCollectSetup FormT=new FormTrojanCollectSetup()) {
				if(FormT.ShowDialog()==DialogResult.Cancel) {
					return;
				}
			}
			if(!Programs.IsEnabled(ProgramName.TrojanExpressCollect)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>Clips the input string to the specified length.  Also strips out any *, tabs, newlines, etc.  If inputstr is null or empty returns empty string.</summary>
		private string Clip(string inputstr,int length){
			if(string.IsNullOrEmpty(inputstr)) {
				return "";
			}
			string retval=inputstr.Replace("*","").Replace("\r","").Replace("\n","").Replace("\t","");
			if(retval.Length>length){
				retval=retval.Substring(0,length);
			}
			return retval;
		}

		private void butHelp_Click(object sender,EventArgs e) {
			using(FormTrojanHelp FormH=new FormTrojanHelp()) {
				FormH.ShowDialog();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textAmount.IsValid()) {
				MsgBox.Show(this,"Please fix debt amount.");
				return;
			}
			double amtDebt=PIn.Double(textAmount.Text);
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Date is not valid.");
				return;
			}
			DateTime dateDelinquency=PIn.Date(textDate.Text);
			if(dateDelinquency.Year<1950) {
				MessageBox.Show("Date is not valid.");
				return;
			}
			if(dateDelinquency>DateTime.Today) {
				MsgBox.Show(this,"Date cannot be a future date.");
				return;
			}
			long programNum=Programs.GetProgramNum(ProgramName.TrojanExpressCollect);
			string password=ProgramProperties.GetPropVal(programNum,"Password");
			if(!Regex.IsMatch(password,@"^[A-Z]{2}\d{4}$")) {
				MsgBox.Show(this,"Password is not in correct format. Must be like this: AB1234");
				return;
			}
			string folderPath=ProgramProperties.GetPropVal(programNum,"FolderPath");
			if(string.IsNullOrEmpty(folderPath)){
				MsgBox.Show(this,"Export folder has not been setup yet.  Please go to Setup at the top of this window.");
				return;
			}
			long billingType=PIn.Long(ProgramProperties.GetPropVal(programNum,"BillingType"));
			if(billingType==0) {
				MsgBox.Show(this,"Billing type has not been setup yet.  Please go to Setup at the top of this window.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!File.Exists(ODFileUtils.CombinePaths(folderPath,"TROBEN.HB"))){
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"The Trojan Communicator is not installed or is not configured for the folder")+": "
					+folderPath+".  "+Lan.g(this,"Please contact Trojan Software Support at 800-451-9723 x1 or x2"));
				return;
			}
			try {
				File.Delete(ODFileUtils.CombinePaths(folderPath,"TROBEN.HB"));
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"There was an error attempting to delete a file from the export folder path.  Check folder permissions and/or try running as administrator.");
				return;
			}
			using(FileSystemWatcher watcher=new FileSystemWatcher(folderPath,"TROBEN.HB")) {
				if(watcher.WaitForChanged(WatcherChangeTypes.Created,10000).TimedOut) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"The Trojan Communicator is not running. Please check it.");
					return;
				}
			}
			StringBuilder str=new StringBuilder();
			if(radioDiplomatic.Checked){
				str.Append("D*");
			}
			else if(radioFirm.Checked) {
				str.Append("F*");
			}
			else if(radioSkip.Checked) {
				str.Append("S*");
			}
			str.Append(Clip(_patCur.LName,18)+"*");
			str.Append(Clip(_patCur.FName,18)+"*");
			str.Append(Clip(_patCur.MiddleI,1)+"*");
			str.Append(Clip(_guarCur.LName,18)+"*");//validated
			str.Append(Clip(_guarCur.FName,18)+"*");//validated
			str.Append(Clip(_guarCur.MiddleI,1)+"*");
			str.Append(_guarCur.SSN.Substring(0,3)+"-"+_guarCur.SSN.Substring(3,2)+"-"+_guarCur.SSN.Substring(5,4)+"*");//validated
			if(_guarCur.Birthdate.Year>=1880) {
				str.Append(_guarCur.Birthdate.ToShortDateString());
			}
			str.Append("*");
			str.Append(Clip(_guarCur.HmPhone,13)+"*");
			str.Append(Clip(_empCur?.EmpName,35)+"*");
			str.Append(Clip(_empCur?.Phone,13)+"*");
			string address=_guarCur.Address;//validated
			if(!string.IsNullOrEmpty(_guarCur.Address2)){
				address+=", "+_guarCur.Address2;
			}
			str.Append(Clip(address,30)+"*");
			str.Append(Clip(_guarCur.City,20)+"*");//validated
			str.Append(Clip(_guarCur.State,2)+"*");//validated
			str.Append(Clip(_guarCur.Zip,5)+"*");//validated
			str.Append(amtDebt.ToString("F2")+"*");//validated
			str.Append(dateDelinquency.ToShortDateString()+"*");//validated
			str.Append(password+"*");//validated
			str.AppendLine(Clip(Security.CurUser.UserName,25));//There is always a logged in user
			int thisNum=TrojanQueries.GetUniqueFileNum();
			string outputFile="CT"+thisNum.ToString().PadLeft(6,'0')+".TRO";
			try {
				File.AppendAllText(ODFileUtils.CombinePaths(folderPath,outputFile),str.ToString());
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"There was an error writing to the export file.  Check folder permissions and/or try running as administrator.");
				return;
			}
			using(FileSystemWatcher watcher=new FileSystemWatcher(folderPath,outputFile)) {
				if(watcher.WaitForChanged(WatcherChangeTypes.Deleted,10000).TimedOut) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Warning!! Request was not sent to Trojan within the 10 second limit.");
					return;
				}
			}
			Patients.UpdateFamilyBillingType(billingType,_patCur.Guarantor);
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

	}
}





















