using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class FormApptEditOld : System.Windows.Forms.Form{
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textTime;
		private OpenDental.UI.Button butCalcTime;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ListBox listQuickAdd;
		private OpenDental.TableApptProcs tbProc;// Required designer variable.
		///<summary></summary>
		public bool IsNew;
		private Procedure[] arrayProc;
		private ApptProc[] ApptProc2;
		private OpenDental.ValidNum textAddTime;
		private OpenDental.TableTimeBar tbTime;
		private System.Windows.Forms.Button butSlider;
		private bool mouseIsDown;
		private Point	mouseOrigin;
		private Point sliderOrigin;
		///<summary>The string time pattern in the current increment. Not in the 5 minute increment.</summary>
		private StringBuilder strBTime;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupFinancial;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox textWirelessPhone;
		private System.Windows.Forms.TextBox textWkPhone;
		private System.Windows.Forms.TextBox textHmPhone;
		private System.Windows.Forms.GroupBox groupContact;
		private System.Windows.Forms.TextBox textAddrNote;
		private OpenDental.UI.Button butAddComm;
		private OpenDental.TableCommLog tbCommlog;
		private bool procsHaveChanged;
		private System.Windows.Forms.CheckBox checkIsNewPatient;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.ComboBox comboUnschedStatus;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.Button butPin;
		private ArrayList ALCommItems;
		///<summary>Allows the parent form to specify if the pinboard button will be visible.</summary>
		public bool PinIsVisible;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textBalance;
		private System.Windows.Forms.TextBox textCreditType;
		private System.Windows.Forms.TextBox textFeeTotal;
		private System.Windows.Forms.TextBox textBillingType;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textFamilyBal;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		///<summary>If the user clicks on the pinboard button this will be set to true.</summary>
		public bool PinClicked;
		///<summary>The list of all procedures for a patient.</summary>
		private Procedure[] ProcList;
		private OpenDental.ODtextBox textNote;
		///<summary>The list of all claimProcs for a patient.</summary>
		private ClaimProc[] ClaimProcList;
		private Patient pat;
		private Family fam;
		private System.Windows.Forms.ComboBox comboConfirmed;
		private System.Windows.Forms.ComboBox comboProvNum;
		private System.Windows.Forms.ComboBox comboProvHyg;
		private System.Windows.Forms.ComboBox comboAssistant;
		private System.Windows.Forms.ComboBox comboLab;
		private OpenDental.UI.Button butTask;
		private System.Windows.Forms.ComboBox comboSchoolCourse;
		private System.Windows.Forms.ComboBox comboSchoolClass;
		private System.Windows.Forms.ComboBox comboInstructor;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.TextBox textGradePoint;
		private System.Windows.Forms.GroupBox groupDentalSchools;
		private InsPlan[] PlanList;
		private Appointment AptCur;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.CheckBox checkIsHygiene;
		private System.Windows.Forms.Label label24;
		private OpenDental.UI.Button butAudit;
		private Appointment AptOld;
		private PatPlan[] PatPlanList;
		private Commlog[] CommlogList;

		///<summary></summary>
		public FormApptEditOld(Appointment aptCur){
			InitializeComponent();// Required for Windows Form Designer support
			tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbTime_CellClicked);
			tbProc.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbProc_CellClicked);
			tbCommlog.CellDoubleClicked+=new OpenDental.ContrTable.CellEventHandler(tbCommlog_CellDoubleClicked);
			Lan.F(this);
			AptCur=aptCur;
			AptOld=aptCur.Copy();
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptEditOld));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listQuickAdd = new System.Windows.Forms.ListBox();
			this.tbTime = new OpenDental.TableTimeBar();
			this.textTime = new System.Windows.Forms.TextBox();
			this.butCalcTime = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tbProc = new OpenDental.TableApptProcs();
			this.textBalance = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textCreditType = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textFeeTotal = new System.Windows.Forms.TextBox();
			this.textAddTime = new OpenDental.ValidNum();
			this.butSlider = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.groupFinancial = new System.Windows.Forms.GroupBox();
			this.textFamilyBal = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textBillingType = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbCommlog = new OpenDental.TableCommLog();
			this.groupContact = new System.Windows.Forms.GroupBox();
			this.textAddrNote = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textWirelessPhone = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textWkPhone = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.textHmPhone = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.butAddComm = new OpenDental.UI.Button();
			this.checkIsNewPatient = new System.Windows.Forms.CheckBox();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.comboUnschedStatus = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butPin = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.comboConfirmed = new System.Windows.Forms.ComboBox();
			this.comboProvNum = new System.Windows.Forms.ComboBox();
			this.comboProvHyg = new System.Windows.Forms.ComboBox();
			this.comboAssistant = new System.Windows.Forms.ComboBox();
			this.comboLab = new System.Windows.Forms.ComboBox();
			this.butTask = new OpenDental.UI.Button();
			this.groupDentalSchools = new System.Windows.Forms.GroupBox();
			this.textGradePoint = new System.Windows.Forms.TextBox();
			this.comboSchoolCourse = new System.Windows.Forms.ComboBox();
			this.comboSchoolClass = new System.Windows.Forms.ComboBox();
			this.comboInstructor = new System.Windows.Forms.ComboBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.checkIsHygiene = new System.Windows.Forms.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.butAudit = new OpenDental.UI.Button();
			this.groupFinancial.SuspendLayout();
			this.groupContact.SuspendLayout();
			this.groupDentalSchools.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.CornerRadius = 4F;
			this.butOK.Location = new System.Drawing.Point(877,618);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(92,26);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(877,651);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(92,26);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listQuickAdd
			// 
			this.listQuickAdd.Location = new System.Drawing.Point(147,43);
			this.listQuickAdd.Name = "listQuickAdd";
			this.listQuickAdd.Size = new System.Drawing.Size(146,225);
			this.listQuickAdd.TabIndex = 3;
			this.listQuickAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listQuickAdd_MouseDown);
			// 
			// tbTime
			// 
			this.tbTime.BackColor = System.Drawing.SystemColors.Window;
			this.tbTime.Location = new System.Drawing.Point(3,7);
			this.tbTime.Name = "tbTime";
			this.tbTime.ScrollValue = 150;
			this.tbTime.SelectedIndices = new int[0];
			this.tbTime.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbTime.Size = new System.Drawing.Size(15,561);
			this.tbTime.TabIndex = 4;
			// 
			// textTime
			// 
			this.textTime.BackColor = System.Drawing.Color.White;
			this.textTime.Location = new System.Drawing.Point(1,569);
			this.textTime.Name = "textTime";
			this.textTime.ReadOnly = true;
			this.textTime.Size = new System.Drawing.Size(66,20);
			this.textTime.TabIndex = 5;
			// 
			// butCalcTime
			// 
			this.butCalcTime.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butCalcTime.Autosize = true;
			this.butCalcTime.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCalcTime.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCalcTime.CornerRadius = 4F;
			this.butCalcTime.Location = new System.Drawing.Point(3,591);
			this.butCalcTime.Name = "butCalcTime";
			this.butCalcTime.Size = new System.Drawing.Size(32,20);
			this.butCalcTime.TabIndex = 6;
			this.butCalcTime.Text = "Calc";
			this.butCalcTime.Click += new System.EventHandler(this.butCalcTime_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4,616);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84,14);
			this.label1.TabIndex = 7;
			this.label1.Text = "Adj Time:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(741,42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98,16);
			this.label2.TabIndex = 11;
			this.label2.Text = "Dentist";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(741,63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98,16);
			this.label3.TabIndex = 13;
			this.label3.Text = "Hygienist";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(22,8);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(118,15);
			this.labelStatus.TabIndex = 15;
			this.labelStatus.Text = "Status";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(22,99);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(118,16);
			this.label5.TabIndex = 17;
			this.label5.Text = "Confirmed";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(22,276);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(197,16);
			this.label6.TabIndex = 20;
			this.label6.Text = "Appointment Note";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(145,2);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(143,39);
			this.label7.TabIndex = 21;
			this.label7.Text = "Single click on items in this list to add them to the treatment plan.";
			// 
			// tbProc
			// 
			this.tbProc.BackColor = System.Drawing.SystemColors.Window;
			this.tbProc.Location = new System.Drawing.Point(307,43);
			this.tbProc.Name = "tbProc";
			this.tbProc.ScrollValue = 1;
			this.tbProc.SelectedIndices = new int[0];
			this.tbProc.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbProc.Size = new System.Drawing.Size(419,325);
			this.tbProc.TabIndex = 22;
			// 
			// textBalance
			// 
			this.textBalance.Location = new System.Drawing.Point(103,52);
			this.textBalance.Name = "textBalance";
			this.textBalance.ReadOnly = true;
			this.textBalance.Size = new System.Drawing.Size(52,20);
			this.textBalance.TabIndex = 58;
			this.textBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(7,54);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(95,16);
			this.label16.TabIndex = 59;
			this.label16.Text = "Patient Balance";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCreditType
			// 
			this.textCreditType.Location = new System.Drawing.Point(103,12);
			this.textCreditType.Name = "textCreditType";
			this.textCreditType.ReadOnly = true;
			this.textCreditType.Size = new System.Drawing.Size(28,20);
			this.textCreditType.TabIndex = 54;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(7,16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(95,14);
			this.label11.TabIndex = 52;
			this.label11.Text = "Credit Type";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(7,100);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(95,15);
			this.label9.TabIndex = 46;
			this.label9.Text = "Fee This Appt";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textFeeTotal
			// 
			this.textFeeTotal.Location = new System.Drawing.Point(103,98);
			this.textFeeTotal.Name = "textFeeTotal";
			this.textFeeTotal.ReadOnly = true;
			this.textFeeTotal.Size = new System.Drawing.Size(52,20);
			this.textFeeTotal.TabIndex = 49;
			this.textFeeTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textAddTime
			// 
			this.textAddTime.Location = new System.Drawing.Point(4,633);
			this.textAddTime.MaxVal = 255;
			this.textAddTime.MinVal = 0;
			this.textAddTime.Name = "textAddTime";
			this.textAddTime.Size = new System.Drawing.Size(65,20);
			this.textAddTime.TabIndex = 3;
			// 
			// butSlider
			// 
			this.butSlider.BackColor = System.Drawing.SystemColors.ControlDark;
			this.butSlider.Location = new System.Drawing.Point(5,91);
			this.butSlider.Name = "butSlider";
			this.butSlider.Size = new System.Drawing.Size(12,15);
			this.butSlider.TabIndex = 58;
			this.butSlider.UseVisualStyleBackColor = false;
			this.butSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseDown);
			this.butSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseMove);
			this.butSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseUp);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(307,13);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(345,29);
			this.label14.TabIndex = 61;
			this.label14.Text = "From the treatment plan list below, highlight the procedures to attach to this ap" +
    "pointment";
			// 
			// groupFinancial
			// 
			this.groupFinancial.Controls.Add(this.textFamilyBal);
			this.groupFinancial.Controls.Add(this.label10);
			this.groupFinancial.Controls.Add(this.textBillingType);
			this.groupFinancial.Controls.Add(this.label8);
			this.groupFinancial.Controls.Add(this.textBalance);
			this.groupFinancial.Controls.Add(this.textFeeTotal);
			this.groupFinancial.Controls.Add(this.label9);
			this.groupFinancial.Controls.Add(this.label11);
			this.groupFinancial.Controls.Add(this.textCreditType);
			this.groupFinancial.Controls.Add(this.label16);
			this.groupFinancial.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupFinancial.Location = new System.Drawing.Point(738,244);
			this.groupFinancial.Name = "groupFinancial";
			this.groupFinancial.Size = new System.Drawing.Size(229,125);
			this.groupFinancial.TabIndex = 63;
			this.groupFinancial.TabStop = false;
			this.groupFinancial.Text = "Financial";
			// 
			// textFamilyBal
			// 
			this.textFamilyBal.Location = new System.Drawing.Point(103,72);
			this.textFamilyBal.Name = "textFamilyBal";
			this.textFamilyBal.ReadOnly = true;
			this.textFamilyBal.Size = new System.Drawing.Size(52,20);
			this.textFamilyBal.TabIndex = 62;
			this.textFamilyBal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(7,74);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(95,16);
			this.label10.TabIndex = 63;
			this.label10.Text = "Family Balance";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBillingType
			// 
			this.textBillingType.Location = new System.Drawing.Point(103,32);
			this.textBillingType.Name = "textBillingType";
			this.textBillingType.ReadOnly = true;
			this.textBillingType.Size = new System.Drawing.Size(119,20);
			this.textBillingType.TabIndex = 60;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(7,34);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(95,16);
			this.label8.TabIndex = 61;
			this.label8.Text = "Billing Type";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbCommlog
			// 
			this.tbCommlog.BackColor = System.Drawing.SystemColors.Window;
			this.tbCommlog.Location = new System.Drawing.Point(251,378);
			this.tbCommlog.Name = "tbCommlog";
			this.tbCommlog.ScrollValue = 427;
			this.tbCommlog.SelectedIndices = new int[0];
			this.tbCommlog.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbCommlog.Size = new System.Drawing.Size(619,308);
			this.tbCommlog.TabIndex = 64;
			// 
			// groupContact
			// 
			this.groupContact.Controls.Add(this.textAddrNote);
			this.groupContact.Controls.Add(this.label19);
			this.groupContact.Controls.Add(this.textWirelessPhone);
			this.groupContact.Controls.Add(this.label18);
			this.groupContact.Controls.Add(this.textWkPhone);
			this.groupContact.Controls.Add(this.label17);
			this.groupContact.Controls.Add(this.textHmPhone);
			this.groupContact.Controls.Add(this.label15);
			this.groupContact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupContact.Location = new System.Drawing.Point(23,372);
			this.groupContact.Name = "groupContact";
			this.groupContact.Size = new System.Drawing.Size(221,171);
			this.groupContact.TabIndex = 65;
			this.groupContact.TabStop = false;
			this.groupContact.Text = "Contact Info";
			// 
			// textAddrNote
			// 
			this.textAddrNote.BackColor = System.Drawing.SystemColors.Control;
			this.textAddrNote.ForeColor = System.Drawing.Color.DarkRed;
			this.textAddrNote.Location = new System.Drawing.Point(5,100);
			this.textAddrNote.Multiline = true;
			this.textAddrNote.Name = "textAddrNote";
			this.textAddrNote.Size = new System.Drawing.Size(211,64);
			this.textAddrNote.TabIndex = 7;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(6,82);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(168,13);
			this.label19.TabIndex = 6;
			this.label19.Text = "Address and Phone Notes";
			this.label19.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textWirelessPhone
			// 
			this.textWirelessPhone.Location = new System.Drawing.Point(115,57);
			this.textWirelessPhone.Name = "textWirelessPhone";
			this.textWirelessPhone.ReadOnly = true;
			this.textWirelessPhone.Size = new System.Drawing.Size(100,20);
			this.textWirelessPhone.TabIndex = 5;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(15,60);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(100,13);
			this.label18.TabIndex = 4;
			this.label18.Text = "Wireless Phone";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWkPhone
			// 
			this.textWkPhone.Location = new System.Drawing.Point(115,37);
			this.textWkPhone.Name = "textWkPhone";
			this.textWkPhone.ReadOnly = true;
			this.textWkPhone.Size = new System.Drawing.Size(100,20);
			this.textWkPhone.TabIndex = 3;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(16,41);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(100,13);
			this.label17.TabIndex = 2;
			this.label17.Text = "Work Phone";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHmPhone
			// 
			this.textHmPhone.Location = new System.Drawing.Point(115,17);
			this.textHmPhone.Name = "textHmPhone";
			this.textHmPhone.ReadOnly = true;
			this.textHmPhone.Size = new System.Drawing.Size(100,20);
			this.textHmPhone.TabIndex = 1;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(16,22);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(100,13);
			this.label15.TabIndex = 0;
			this.label15.Text = "Home Phone";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAddComm
			// 
			this.butAddComm.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butAddComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddComm.Autosize = true;
			this.butAddComm.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddComm.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddComm.CornerRadius = 4F;
			this.butAddComm.Image = global::OpenDental.Properties.Resources.Add;
			this.butAddComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddComm.Location = new System.Drawing.Point(877,379);
			this.butAddComm.Name = "butAddComm";
			this.butAddComm.Size = new System.Drawing.Size(92,26);
			this.butAddComm.TabIndex = 66;
			this.butAddComm.Text = "Co&mm";
			this.butAddComm.Click += new System.EventHandler(this.butAddComm_Click);
			// 
			// checkIsNewPatient
			// 
			this.checkIsNewPatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsNewPatient.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsNewPatient.Location = new System.Drawing.Point(755,0);
			this.checkIsNewPatient.Name = "checkIsNewPatient";
			this.checkIsNewPatient.Size = new System.Drawing.Size(208,17);
			this.checkIsNewPatient.TabIndex = 67;
			this.checkIsNewPatient.Text = "New Patient";
			this.checkIsNewPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(22,24);
			this.comboStatus.MaxDropDownItems = 10;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(121,21);
			this.comboStatus.TabIndex = 68;
			// 
			// comboUnschedStatus
			// 
			this.comboUnschedStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnschedStatus.Location = new System.Drawing.Point(22,69);
			this.comboUnschedStatus.MaxDropDownItems = 100;
			this.comboUnschedStatus.Name = "comboUnschedStatus";
			this.comboUnschedStatus.Size = new System.Drawing.Size(121,21);
			this.comboUnschedStatus.TabIndex = 70;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(22,53);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(126,15);
			this.label4.TabIndex = 69;
			this.label4.Text = "Unscheduled Status";
			// 
			// butPin
			// 
			this.butPin.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPin.Autosize = true;
			this.butPin.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPin.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPin.CornerRadius = 4F;
			this.butPin.Image = ((System.Drawing.Image)(resources.GetObject("butPin.Image")));
			this.butPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPin.Location = new System.Drawing.Point(877,552);
			this.butPin.Name = "butPin";
			this.butPin.Size = new System.Drawing.Size(92,26);
			this.butPin.TabIndex = 71;
			this.butPin.Text = "&Pinboard";
			this.butPin.Click += new System.EventHandler(this.butPin_Click);
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butDelete.Autosize = true;
			this.butDelete.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDelete.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDelete.CornerRadius = 4F;
			this.butDelete.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(877,585);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92,26);
			this.butDelete.TabIndex = 72;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(741,101);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(98,16);
			this.label12.TabIndex = 74;
			this.label12.Text = "Assistant";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(741,122);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(98,16);
			this.label13.TabIndex = 76;
			this.label13.Text = "Lab Case";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsReturn = true;
			this.textNote.Location = new System.Drawing.Point(22,292);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Appointment;
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(272,73);
			this.textNote.TabIndex = 77;
			// 
			// comboConfirmed
			// 
			this.comboConfirmed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConfirmed.Location = new System.Drawing.Point(22,116);
			this.comboConfirmed.MaxDropDownItems = 30;
			this.comboConfirmed.Name = "comboConfirmed";
			this.comboConfirmed.Size = new System.Drawing.Size(121,21);
			this.comboConfirmed.TabIndex = 78;
			// 
			// comboProvNum
			// 
			this.comboProvNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvNum.Location = new System.Drawing.Point(842,38);
			this.comboProvNum.MaxDropDownItems = 100;
			this.comboProvNum.Name = "comboProvNum";
			this.comboProvNum.Size = new System.Drawing.Size(126,21);
			this.comboProvNum.TabIndex = 79;
			this.comboProvNum.SelectedIndexChanged += new System.EventHandler(this.comboProvNum_SelectedIndexChanged);
			// 
			// comboProvHyg
			// 
			this.comboProvHyg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvHyg.Location = new System.Drawing.Point(842,59);
			this.comboProvHyg.MaxDropDownItems = 30;
			this.comboProvHyg.Name = "comboProvHyg";
			this.comboProvHyg.Size = new System.Drawing.Size(126,21);
			this.comboProvHyg.TabIndex = 80;
			// 
			// comboAssistant
			// 
			this.comboAssistant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAssistant.Location = new System.Drawing.Point(842,97);
			this.comboAssistant.MaxDropDownItems = 30;
			this.comboAssistant.Name = "comboAssistant";
			this.comboAssistant.Size = new System.Drawing.Size(126,21);
			this.comboAssistant.TabIndex = 81;
			// 
			// comboLab
			// 
			this.comboLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLab.Location = new System.Drawing.Point(842,118);
			this.comboLab.MaxDropDownItems = 30;
			this.comboLab.Name = "comboLab";
			this.comboLab.Size = new System.Drawing.Size(126,21);
			this.comboLab.TabIndex = 82;
			// 
			// butTask
			// 
			this.butTask.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTask.Autosize = true;
			this.butTask.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butTask.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butTask.CornerRadius = 4F;
			this.butTask.Location = new System.Drawing.Point(877,519);
			this.butTask.Name = "butTask";
			this.butTask.Size = new System.Drawing.Size(92,26);
			this.butTask.TabIndex = 83;
			this.butTask.Text = "To Task List";
			this.butTask.Click += new System.EventHandler(this.butTask_Click);
			// 
			// groupDentalSchools
			// 
			this.groupDentalSchools.Controls.Add(this.textGradePoint);
			this.groupDentalSchools.Controls.Add(this.comboSchoolCourse);
			this.groupDentalSchools.Controls.Add(this.comboSchoolClass);
			this.groupDentalSchools.Controls.Add(this.comboInstructor);
			this.groupDentalSchools.Controls.Add(this.label20);
			this.groupDentalSchools.Controls.Add(this.label21);
			this.groupDentalSchools.Controls.Add(this.label22);
			this.groupDentalSchools.Controls.Add(this.label23);
			this.groupDentalSchools.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupDentalSchools.Location = new System.Drawing.Point(738,141);
			this.groupDentalSchools.Name = "groupDentalSchools";
			this.groupDentalSchools.Size = new System.Drawing.Size(228,101);
			this.groupDentalSchools.TabIndex = 84;
			this.groupDentalSchools.TabStop = false;
			this.groupDentalSchools.Text = "Dental Schools";
			// 
			// textGradePoint
			// 
			this.textGradePoint.Location = new System.Drawing.Point(93,75);
			this.textGradePoint.Name = "textGradePoint";
			this.textGradePoint.Size = new System.Drawing.Size(63,20);
			this.textGradePoint.TabIndex = 90;
			// 
			// comboSchoolCourse
			// 
			this.comboSchoolCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolCourse.Location = new System.Drawing.Point(93,54);
			this.comboSchoolCourse.MaxDropDownItems = 30;
			this.comboSchoolCourse.Name = "comboSchoolCourse";
			this.comboSchoolCourse.Size = new System.Drawing.Size(130,21);
			this.comboSchoolCourse.TabIndex = 89;
			// 
			// comboSchoolClass
			// 
			this.comboSchoolClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolClass.Location = new System.Drawing.Point(93,33);
			this.comboSchoolClass.MaxDropDownItems = 30;
			this.comboSchoolClass.Name = "comboSchoolClass";
			this.comboSchoolClass.Size = new System.Drawing.Size(130,21);
			this.comboSchoolClass.TabIndex = 88;
			// 
			// comboInstructor
			// 
			this.comboInstructor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboInstructor.Location = new System.Drawing.Point(93,12);
			this.comboInstructor.MaxDropDownItems = 30;
			this.comboInstructor.Name = "comboInstructor";
			this.comboInstructor.Size = new System.Drawing.Size(130,21);
			this.comboInstructor.TabIndex = 87;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(9,78);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(82,16);
			this.label20.TabIndex = 86;
			this.label20.Text = "Grade";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(9,58);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(82,16);
			this.label21.TabIndex = 85;
			this.label21.Text = "Course";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(9,36);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(82,16);
			this.label22.TabIndex = 84;
			this.label22.Text = "Class";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(9,16);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(82,16);
			this.label23.TabIndex = 83;
			this.label23.Text = "Instructor";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(842,17);
			this.comboClinic.MaxDropDownItems = 100;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(126,21);
			this.comboClinic.TabIndex = 86;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(741,21);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(98,16);
			this.labelClinic.TabIndex = 85;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsHygiene
			// 
			this.checkIsHygiene.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHygiene.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHygiene.Location = new System.Drawing.Point(751,81);
			this.checkIsHygiene.Name = "checkIsHygiene";
			this.checkIsHygiene.Size = new System.Drawing.Size(104,16);
			this.checkIsHygiene.TabIndex = 117;
			this.checkIsHygiene.Text = "Is Hygiene";
			this.checkIsHygiene.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(856,81);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(113,16);
			this.label24.TabIndex = 118;
			this.label24.Text = "(use hyg color)";
			// 
			// butAudit
			// 
			this.butAudit.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Autosize = true;
			this.butAudit.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAudit.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAudit.CornerRadius = 4F;
			this.butAudit.Location = new System.Drawing.Point(877,486);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(92,26);
			this.butAudit.TabIndex = 119;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// FormApptEditOld
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(974,695);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.butTask);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textAddTime);
			this.Controls.Add(this.textTime);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butPin);
			this.Controls.Add(this.butAddComm);
			this.Controls.Add(this.butCalcTime);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.checkIsHygiene);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.groupDentalSchools);
			this.Controls.Add(this.comboLab);
			this.Controls.Add(this.comboAssistant);
			this.Controls.Add(this.comboProvHyg);
			this.Controls.Add(this.comboProvNum);
			this.Controls.Add(this.comboConfirmed);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.comboUnschedStatus);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.checkIsNewPatient);
			this.Controls.Add(this.groupContact);
			this.Controls.Add(this.tbCommlog);
			this.Controls.Add(this.groupFinancial);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.butSlider);
			this.Controls.Add(this.tbProc);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbTime);
			this.Controls.Add(this.listQuickAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptEditOld";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Appointment";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormApptEdit_Closing);
			this.Load += new System.EventHandler(this.FormApptEdit_Load);
			this.groupFinancial.ResumeLayout(false);
			this.groupFinancial.PerformLayout();
			this.groupContact.ResumeLayout(false);
			this.groupContact.PerformLayout();
			this.groupDentalSchools.ResumeLayout(false);
			this.groupDentalSchools.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormApptEdit_Load(object sender, System.EventArgs e){
			if(IsNew){
				if(!Security.IsAuthorized(Permissions.AppointmentCreate)){
					DialogResult=DialogResult.Cancel;
					return;
				}
			}
			else{
				if(!Security.IsAuthorized(Permissions.AppointmentEdit)){
					butOK.Enabled=false;
					butDelete.Enabled=false;
					butPin.Enabled=false;
					tbProc.Enabled=false;
				}
			}
			fam=Patients.GetFamily(AptCur.PatNum);
			pat=fam.GetPatient(AptCur.PatNum);
			PlanList=InsPlans.Refresh(fam);
			PatPlanList=PatPlans.Refresh(AptCur.PatNum);
			//CovPats.Refresh(PlanList,PatPlanList);
			if(PrefB.GetBool("EasyHideDentalSchools")){
				groupDentalSchools.Visible=false;
			}
			if(PrefB.GetBool("EasyNoClinics")){
				labelClinic.Visible=false;
				comboClinic.Visible=false;
			}
			if(!PinIsVisible)
				butPin.Visible=false;
			if(AptCur.AptStatus==ApptStatus.Planned){
				Text=Lan.g(this,"Edit Planned Appointment")+" - "+pat.GetNameLF();
				labelStatus.Visible=false;
				comboStatus.Visible=false;
				butDelete.Visible=false;
			}
			else{
				Text=Lan.g(this,"Edit Appointment")+" - "+pat.GetNameLF();
				comboStatus.Items.Add(Lan.g("enumApptStatus","Scheduled"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Complete"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","UnschedList"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","ASAP"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Broken"));
				comboStatus.SelectedIndex=(int)AptCur.AptStatus-1;
			}
			if(IsNew){
				//for this, the appointment has to be created somewhere else first.
				//It might only be temporary, and will be deleted if Cancel is clicked.
				//AptCur=new Appointment();//handled  previously
				//Appointments.SaveApt();
				if(AptCur.Confirmed==0)
					AptCur.Confirmed=DefB.Short[(int)DefCat.ApptConfirmed][0].DefNum;
				if(AptCur.ProvNum==0)
					AptCur.ProvNum=Providers.List[0].ProvNum;
			}
			//for(int i=1;i<Enum.GetNames(typeof(ApptStatus)).Length;i++){//none status is not displayed
			//	listStatus.Items.Add(Enum.GetNames(typeof(ApptStatus))[i]);
			//}
			//convert time pattern from 5 to current increment.
			strBTime=new StringBuilder();
			for(int i=0;i<AptCur.Pattern.Length;i++){
				strBTime.Append(AptCur.Pattern.Substring(i,1));
				i++;
				if(PrefB.GetInt("AppointmentTimeIncrement")==15){
					i++;
				}
			}
			comboUnschedStatus.Items.Add(Lan.g(this,"none"));
			comboUnschedStatus.SelectedIndex=0;
			for(int i=0;i<DefB.Short[(int)DefCat.RecallUnschedStatus].Length;i++){
				comboUnschedStatus.Items.Add(DefB.Short[(int)DefCat.RecallUnschedStatus][i].ItemName);
				if(DefB.Short[(int)DefCat.RecallUnschedStatus][i].DefNum==AptCur.UnschedStatus)
					comboUnschedStatus.SelectedIndex=i+1;
			}
			for(int i=0;i<DefB.Short[(int)DefCat.ApptConfirmed].Length;i++){
				comboConfirmed.Items.Add(DefB.Short[(int)DefCat.ApptConfirmed][i].ItemName);
				if(DefB.Short[(int)DefCat.ApptConfirmed][i].DefNum==AptCur.Confirmed)
					comboConfirmed.SelectedIndex=i;
			}
			textAddTime.MinVal=-1200;
			textAddTime.MaxVal=1200;
			textAddTime.Text=POut.PInt(AptCur.AddTime*PIn.PInt(((Pref)PrefB.HList["AppointmentTimeIncrement"]).ValueString));
			textNote.Text=AptCur.Note;
			for(int i=0;i<DefB.Short[(int)DefCat.ApptProcsQuickAdd].Length;i++){
				listQuickAdd.Items.Add(DefB.Short[(int)DefCat.ApptProcsQuickAdd][i].ItemName);
			}
			//SchoolClassNum must be filled before provider
			comboSchoolClass.Items.Add(Lan.g(this,"none"));
			for(int i=0;i<SchoolClasses.List.Length;i++){
				comboSchoolClass.Items.Add(SchoolClasses.List[i].GradYear.ToString()+"-"+SchoolClasses.List[i].Descript);
			}
			comboClinic.Items.Add(Lan.g(this,"none"));
			comboClinic.SelectedIndex=0;
			for(int i=0;i<Clinics.List.Length;i++){
				comboClinic.Items.Add(Clinics.List[i].Description);
				if(Clinics.List[i].ClinicNum==AptCur.ClinicNum)
					comboClinic.SelectedIndex=i+1;
			}
			for(int i=0;i<Providers.List.Length;i++){
				comboProvNum.Items.Add(Providers.List[i].Abbr);
				if(Providers.List[i].ProvNum==AptCur.ProvNum)
					comboProvNum.SelectedIndex=i;
			}
			comboProvHyg.Items.Add(Lan.g(this,"none"));
			comboProvHyg.SelectedIndex=0;
			for(int i=0;i<Providers.List.Length;i++){
				comboProvHyg.Items.Add(Providers.List[i].Abbr);
				if(Providers.List[i].ProvNum==AptCur.ProvHyg)
					comboProvHyg.SelectedIndex=i+1;
			}
			checkIsHygiene.Checked=AptCur.IsHygiene;
			comboAssistant.Items.Add(Lan.g(this,"none"));
			comboAssistant.SelectedIndex=0;
			for(int i=0;i<Employees.ListShort.Length;i++){
				comboAssistant.Items.Add(Employees.ListShort[i].FName);
				if(Employees.ListShort[i].EmployeeNum==AptCur.Assistant)
					comboAssistant.SelectedIndex=i+1;
			}
			string[] enumLab=Enum.GetNames(typeof(LabCaseOld));
			for(int i=0;i<enumLab.Length;i++){
				comboLab.Items.Add(Lan.g("enumLab",enumLab[i]));
			}
			comboLab.SelectedIndex=(int)AptCur.Lab;
			comboInstructor.Items.Add(Lan.g(this,"none"));
			comboInstructor.SelectedIndex=0;
			for(int i=0;i<Instructors.List.Length;i++){
				comboInstructor.Items.Add(Instructors.List[i].LName+", "+Instructors.List[i].FName+", "+Instructors.List[i].Suffix);
				if(Instructors.List[i].InstructorNum==AptCur.InstructorNum)
					comboInstructor.SelectedIndex=i+1;
			}
			//SchoolClassNum was filled earlier.  Now selected:
			comboSchoolClass.SelectedIndex=0;
			for(int i=0;i<SchoolClasses.List.Length;i++){
				if(SchoolClasses.List[i].SchoolClassNum==AptCur.SchoolClassNum)
					comboSchoolClass.SelectedIndex=i+1;
			}
			comboSchoolCourse.Items.Add(Lan.g(this,"none"));
			comboSchoolCourse.SelectedIndex=0;
			for(int i=0;i<SchoolCourses.List.Length;i++){
				comboSchoolCourse.Items.Add(SchoolCourses.List[i].CourseID+"  "+SchoolCourses.List[i].Descript);
				if(SchoolCourses.List[i].SchoolCourseNum==AptCur.SchoolCourseNum)
					comboSchoolCourse.SelectedIndex=i+1;
			}
			textGradePoint.Text=AptCur.GradePoint.ToString();
			//IsNewPatient is set well before opening this form.
			checkIsNewPatient.Checked=AptCur.IsNewPatient;
			textHmPhone.Text=pat.HmPhone;
			textWkPhone.Text=pat.WkPhone;
			textWirelessPhone.Text=pat.WirelessPhone;
			textAddrNote.Text=pat.AddrNote;
			textCreditType.Text=pat.CreditType;
			textBillingType.Text=DefB.GetName(DefCat.BillingTypes,pat.BillingType);
			textBalance.Text=pat.EstBalance.ToString("F");
			textFamilyBal.Text=fam.List[0].BalTotal.ToString("F");
			if(ContrApptSheet.MinPerIncr==10){
				tbTime.TopBorder[0,6]=Color.Black;
				tbTime.TopBorder[0,12]=Color.Black;
				tbTime.TopBorder[0,18]=Color.Black;
				tbTime.TopBorder[0,24]=Color.Black;
				tbTime.TopBorder[0,30]=Color.Black;
				tbTime.TopBorder[0,36]=Color.Black;
			}
			else{
				tbTime.TopBorder[0,4]=Color.Black;
				tbTime.TopBorder[0,8]=Color.Black;
				tbTime.TopBorder[0,12]=Color.Black;
				tbTime.TopBorder[0,16]=Color.Black;
				tbTime.TopBorder[0,20]=Color.Black;
				tbTime.TopBorder[0,24]=Color.Black;
				tbTime.TopBorder[0,28]=Color.Black;
				tbTime.TopBorder[0,32]=Color.Black;
				tbTime.TopBorder[0,36]=Color.Black;
			}
			FillProcedures();
			FillTime();
			FillComm();
		}

		private void FillProcedures(){
			ProcList=Procedures.Refresh(pat.PatNum);
			ClaimProcList=ClaimProcs.Refresh(pat.PatNum);
			arrayProc=new Procedure[ProcList.Length];
			int countLine=0;
			//step through all procedures for patient and move selected ones to
			//arrayProc array as intermediate, then to ApptProc2 array for display
			if(AptCur.AptStatus==ApptStatus.Planned){
				for(int i=0;i<ProcList.Length;i++){
					if(ProcList[i].PlannedAptNum==AptCur.AptNum){
						arrayProc[countLine]=ProcList[i];
						countLine++;
					}
					else if(ProcList[i].ProcStatus==ProcStat.TP){
						arrayProc[countLine]=ProcList[i];
						countLine++;
					}
				}
			}
			else{//standard appt
				for(int i=0;i<ProcList.Length;i++){
					if(ProcList[i].AptNum==AptCur.AptNum){
						arrayProc[countLine]=ProcList[i];
						countLine++;
					}
					else if(ProcList[i].AptNum!=0){
						//attached to another appt so don't show
					}
					else if(ProcList[i].ProcStatus==ProcStat.TP){
						arrayProc[countLine]=ProcList[i];
						countLine++;
					}
      		else if(ProcList[i].ProcStatus==ProcStat.C
						&& ProcList[i].ProcDate.Date==AptCur.AptDateTime.Date){
						arrayProc[countLine]=ProcList[i];
						countLine++;
					}
				}
			}
			//This is where to convert arrayProc to ApptProc2:
			double feeTotal=0;
			ApptProc2=new ApptProc[countLine];
			for(int i=0;i<ApptProc2.Length;i++){
				ApptProc2[i].Index=i;
				switch (arrayProc[i].ProcStatus){
					case ProcStat.TP: ApptProc2[i].Status="TP"; break;
					case ProcStat.C: ApptProc2[i].Status="C"; break;
					//very rare, but possible:
					case ProcStat.EC: ApptProc2[i].Status="EC"; break;
					case ProcStat.EO: ApptProc2[i].Status="EO"; break;
					case ProcStat.R: ApptProc2[i].Status="R"; break;
				}
				ApptProc2[i].Priority=DefB.GetName(DefCat.TxPriorities,arrayProc[i].Priority);
				ApptProc2[i].ToothNum=arrayProc[i].ToothNum;
				ApptProc2[i].Surf    =arrayProc[i].Surf;
				ApptProc2[i].AbbrDesc=ProcedureCodes.GetProcCode(arrayProc[i].ADACode).Descript;;
				ApptProc2[i].Fee     =arrayProc[i].ProcFee.ToString("F");
			}
			//Then, fill tbProc from ApptProc2:
			tbProc.ResetRows(ApptProc2.Length);
			//tbProc.SetBackGColor(Color.White);
			tbProc.SetGridColor(Color.Gray);
			for (int i=0;i<ApptProc2.Length;i++){
				tbProc.Cell[0,i]=ApptProc2[i].Status;
				tbProc.Cell[1,i]=ApptProc2[i].Priority;
				tbProc.Cell[2,i]=Tooth.ToInternat(ApptProc2[i].ToothNum);
				tbProc.Cell[3,i]=ApptProc2[i].Surf;
				tbProc.Cell[4,i]=ApptProc2[i].AbbrDesc;
				tbProc.Cell[5,i]=ApptProc2[i].Fee;
				if(AptCur.AptStatus==ApptStatus.Planned){
					if(arrayProc[ApptProc2[i].Index].PlannedAptNum==AptCur.AptNum){
						tbProc.ColorRow(i,Color.Silver);
						feeTotal+=arrayProc[ApptProc2[i].Index].ProcFee;
					}
				}
				else{
					if(arrayProc[ApptProc2[i].Index].AptNum==AptCur.AptNum){
						tbProc.ColorRow(i,Color.Silver);
						feeTotal+=arrayProc[ApptProc2[i].Index].ProcFee;
					}
				}
				
			}
			tbProc.LayoutTables();
			textFeeTotal.Text=feeTotal.ToString("F");
		}//end FillProcedures

		private void tbProc_CellClicked(object sender, CellEventArgs e){
			if(textAddTime.errorProvider1.GetError(textAddTime)!=""
				//|| textDateTerm.errorProvider1.GetError(textDateTerm)!=""
				){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(AptCur.AptStatus==ApptStatus.Complete){
				//added procedures would be marked complete when form closes. We'll just stop it here.
				if(!Security.IsAuthorized(Permissions.ProcComplCreate)){
					return;
				}
			}
			procsHaveChanged=true;
			Procedure ProcCur=arrayProc[ApptProc2[e.Row].Index];
			//Procedure ProcOld=ProcCur.Copy();
			if(AptCur.AptStatus==ApptStatus.Planned){
				if(ProcCur.PlannedAptNum==AptCur.AptNum){
					ProcCur.PlannedAptNum=0;
				}
				else{
					ProcCur.PlannedAptNum=AptCur.AptNum;
				}
				Procedures.UpdatePlannedAptNum(ProcCur.ProcNum,ProcCur.PlannedAptNum);
			}
			else{//not Planned
				if(ProcCur.AptNum==AptCur.AptNum){
					ProcCur.AptNum=0;
				}
				else{
					ProcCur.AptNum=AptCur.AptNum;
				}
				Procedures.UpdateAptNum(ProcCur.ProcNum,ProcCur.AptNum);
			}
			//changing the AptNum of a proc does not affect the recall synch, so no synch here.
			//Procedures.Update(ProcCur,ProcOld);
			//ProcCur.Update(ProcOld);
			int scroll=tbProc.ScrollValue;
			FillProcedures();
			tbProc.ScrollValue=scroll;
			CalculateTime();
			FillTime();
		}

		private void CalculateTime(){
			int adjTimeU=PIn.PInt(textAddTime.Text)/PrefB.GetInt("AppointmentTimeIncrement");
			strBTime=new StringBuilder("");
			string procTime="";
			int attachedProcs=0;
			for(int i=0;i<ApptProc2.Length;i++){
				if(AptCur.AptStatus==ApptStatus.Planned){
					if(arrayProc[ApptProc2[i].Index].PlannedAptNum!=AptCur.AptNum){
						continue;
					}
				}
				else{
					if(arrayProc[ApptProc2[i].Index].AptNum!=AptCur.AptNum){
						continue;
					}
				}
				attachedProcs++;
			}
			if(attachedProcs==1){
				for(int i=0;i<ApptProc2.Length;i++){
					if(AptCur.AptStatus==ApptStatus.Planned){
						if(arrayProc[ApptProc2[i].Index].PlannedAptNum!=AptCur.AptNum){
							continue;
						}
					}
					else{
						if(arrayProc[ApptProc2[i].Index].AptNum!=AptCur.AptNum){
							continue;
						}
					}
					procTime=ProcedureCodes.GetProcCode(arrayProc[ApptProc2[i].Index].ADACode).ProcTime;
					strBTime.Append(procTime);
				}
			}
			else{//multiple procs or no procs
				for(int i=0;i<ApptProc2.Length;i++){
					if(AptCur.AptStatus==ApptStatus.Planned){
						if(arrayProc[ApptProc2[i].Index].PlannedAptNum!=AptCur.AptNum){
							continue;
						}
					}
					else{
						if(arrayProc[ApptProc2[i].Index].AptNum!=AptCur.AptNum){
							continue;
						}
					}
					procTime=ProcedureCodes.GetProcCode(arrayProc[ApptProc2[i].Index].ADACode).ProcTime;
					if(procTime.Length<2) continue;
					for(int n=1;n<procTime.Length-1;n++){
						if(procTime.Substring(n,1)=="/"){
							strBTime.Append("/");
						}
						else{
							strBTime.Insert(0,"X");
						}
					}
				}//end for
			}//else multiple procs
			//MessageBox.Show(strBTime.ToString());
			if(adjTimeU!=0){
				if(strBTime.Length==0){//might be useless.
					if(adjTimeU > 0){
						strBTime.Insert(0,"X",adjTimeU);
					}
				}
				else{//not length 0
					double xRatio;
					if((double)strBTime.ToString().LastIndexOf("X")==0)
						xRatio=1;
					else
						xRatio=(double)strBTime.ToString().LastIndexOf("X")/(double)(strBTime.Length-1);
					if(adjTimeU<0){//subtract time
						int xPort=(int)(-adjTimeU*xRatio);
						if(xPort > 0)
							if(xPort>=strBTime.Length)
								strBTime=new StringBuilder("");
							else
								strBTime.Remove(0,xPort);
						int iRemove=strBTime.Length-(-adjTimeU-xPort);
						if(iRemove < 0)
							strBTime=new StringBuilder("");
						else if(adjTimeU+xPort > strBTime.Length){
							strBTime=new StringBuilder("");
						}
						else
							strBTime.Remove(iRemove,-adjTimeU-xPort);
					}
					else{//add time
						//MessageBox.Show("adjTimeU:"+adjTimeU.ToString()+"xratio:"+xRatio.ToString());
						int xPort=(int)Math.Ceiling(adjTimeU*xRatio);
						//MessageBox.Show("xPort:"+xPort.ToString());
						if(xPort > 0)
							strBTime.Insert(0,"X",xPort);
						if(adjTimeU-xPort > 0)
							strBTime.Insert(strBTime.Length-1,"/",adjTimeU-xPort);
					}
				}//end else not length 0
			}//if(adjTimeU!=0)
			if(attachedProcs>1){//multiple procs
				strBTime.Insert(0,"/");
				strBTime.Append("/");
			}
			else if(attachedProcs==0){//0 procs
				strBTime.Append("/");
			}
			if(strBTime.Length>39){
				strBTime.Remove(39,strBTime.Length-39);
			}
		}

		private void FillTime(){
			Color provColor=Providers.List[comboProvNum.SelectedIndex].ProvColor;
			for(int i=0;i<strBTime.Length;i++){
				if(strBTime.ToString(i,1)=="X"){
					tbTime.BackGColor[0,i]=provColor;
					//.Cell[0,i]=strBTime.ToString(i,1);
				}
				else{
					tbTime.BackGColor[0,i]=Color.White;
				}
			}
			for(int i=strBTime.Length;i<tbTime.MaxRows;i++){
				//tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			butSlider.Location=new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+strBTime.Length*14+1));
			textTime.Text=(strBTime.Length*ContrApptSheet.MinPerIncr).ToString();
		}

		private void tbTime_CellClicked(object sender, CellEventArgs e){
			if(e.Row<strBTime.Length){
				if(strBTime[e.Row]=='/'){
					strBTime.Replace('/','X',e.Row,1);
				}
				else{
					strBTime.Replace(strBTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void butSlider_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=true;
			mouseOrigin=new Point(e.X+butSlider.Location.X
				,e.Y+butSlider.Location.Y);
			sliderOrigin=butSlider.Location;
			
		}

		private void butSlider_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!mouseIsDown)return;
			//tempPoint represents the new location of button of smooth dragging.
			Point tempPoint=new Point(sliderOrigin.X
				,sliderOrigin.Y+(e.Y+butSlider.Location.Y)-mouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(tempPoint.Y-tbTime.Location.Y)/14));
			if(step==strBTime.Length)return;
			if(step<1)return;
			if(step>tbTime.MaxRows-1) return;
			if(step>strBTime.Length){
				strBTime.Append('/');
			}
			if(step<strBTime.Length){
				strBTime.Remove(step,1);
			}
			FillTime();
		}

		private void butSlider_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=false;
		}

		private void butCalcTime_Click(object sender, System.EventArgs e) {
			if(textAddTime.errorProvider1.GetError(textAddTime)!=""
				){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			CalculateTime();
			FillTime();
		}

		private void comboProvNum_SelectedIndexChanged(object sender, System.EventArgs e) {
			//this also gets triggered when first filling form, but SchoolClassNum immediately overwritten with correct value
			if(comboProvNum.SelectedIndex==-1){
				return;
			}
			if(Providers.List[comboProvNum.SelectedIndex].SchoolClassNum==0){
				return;
			}
			for(int i=0;i<SchoolClasses.List.Length;i++){
				if(SchoolClasses.List[i].SchoolClassNum==Providers.List[comboProvNum.SelectedIndex].SchoolClassNum)
					comboSchoolClass.SelectedIndex=i+1;
			}
		}

		/// <summary>Fills the commlog table on this form.</summary>
		private void FillComm(){
			CommlogList=Commlogs.Refresh(AptCur.PatNum);
			ALCommItems=new ArrayList();
			for(int i=0;i<CommlogList.Length;i++){
				//if(Commlogs.List[i].CommType==CommItemType.ApptRelated){
				if(CommlogList[i].CommType!=CommItemType.StatementSent){
					ALCommItems.Add(CommlogList[i]);
				}
			}
			tbCommlog.ResetRows(ALCommItems.Count);
			for(int i=0;i<ALCommItems.Count;i++){
				tbCommlog.Cell[0,i]=((Commlog)ALCommItems[i]).CommDateTime.ToShortDateString();
				tbCommlog.Cell[1,i]=((Commlog)ALCommItems[i]).Note;
				if(((Commlog)ALCommItems[i]).CommType==CommItemType.ApptRelated){
					tbCommlog.BackGColor[0,i]=DefB.Long[(int)DefCat.MiscColors][7].ItemColor;
					tbCommlog.BackGColor[1,i]=DefB.Long[(int)DefCat.MiscColors][7].ItemColor;
				}
			}
			tbCommlog.SetGridColor(Color.Gray);
			tbCommlog.LayoutTables();
		}

		private void tbCommlog_CellDoubleClicked(object sender, CellEventArgs e){
			FormCommItem FormCI=new FormCommItem((Commlog)ALCommItems[e.Row]);
			FormCI.ShowDialog();
			FillComm();
		}		

		private void listQuickAdd_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(listQuickAdd.IndexFromPoint(e.X,e.Y)==-1){
				return;
			}
			if(textAddTime.errorProvider1.GetError(textAddTime)!="")
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(AptCur.AptStatus==ApptStatus.Complete){
				//added procedures would be marked complete when form closes. We'll just stop it here.
				if(!Security.IsAuthorized(Permissions.ProcComplCreate)){
					return;
				}
			}
			Procedures.SetDateFirstVisit(AptCur.AptDateTime.Date,1,pat);
			Benefit[] benefitList=Benefits.Refresh(PatPlanList);
			string[] codes=DefB.Short[(int)DefCat.ApptProcsQuickAdd]
				[listQuickAdd.IndexFromPoint(e.X,e.Y)].ItemValue.Split(',');
			for(int i=0;i<codes.Length;i++){
				Procedure ProcCur=new Procedure();
				//maybe test codes in defs before allowing them in the first place(no tooth num)
				//if(ProcCodes.GetProcCode(Procedures.Cur.ADACode). 
				ProcCur.PatNum=AptCur.PatNum;
				if(AptCur.AptStatus!=ApptStatus.Planned)
					ProcCur.AptNum=AptCur.AptNum;
				ProcCur.ADACode=codes[i];
				ProcCur.ProcDate=AptCur.AptDateTime.Date;
				ProcCur.ProcFee=Fees.GetAmount0(ProcCur.ADACode,Fees.GetFeeSched(pat,PlanList,PatPlanList));
				//surf
				//toothnum
				//toothrange
				//priority
				ProcCur.ProcStatus=ProcStat.TP;
				//procnote
				ProcCur.ProvNum=AptCur.ProvNum;
				//Dx
				ProcCur.ClinicNum=AptCur.ClinicNum;
				if(AptCur.AptStatus==ApptStatus.Planned)
          ProcCur.PlannedAptNum=AptCur.AptNum;
				ProcCur.MedicalCode=ProcedureCodes.GetProcCode(ProcCur.ADACode).MedicalCode;
				Procedures.Insert(ProcCur);//recall synch not required
				Procedures.ComputeEstimates(ProcCur,pat.PatNum,ClaimProcList,false,PlanList,PatPlanList,benefitList);
			}
			listQuickAdd.SelectedIndex=-1;
			FillProcedures();
			CalculateTime();
			FillTime();
		}

		private void butAddComm_Click(object sender, System.EventArgs e) {
			Commlog CommlogCur=new Commlog();
			CommlogCur.PatNum=pat.PatNum;
			CommlogCur.CommDateTime=DateTime.Now;
			CommlogCur.CommType=CommItemType.ApptRelated;
			FormCommItem FormCI=new FormCommItem(CommlogCur);
			FormCI.IsNew=true;
			FormCI.ShowDialog();
			FillComm();
		}	

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Delete appointment?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			if(AptCur.AptStatus==ApptStatus.Planned){
				Procedures.UnattachProcsInPlannedAppt(AptCur.AptNum);
			}
			else{
				Procedures.UnattachProcsInAppt(AptCur.AptNum);
			}
			Appointments.Delete(AptCur);
			SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,
				"Delete for patient: "
				+pat.GetNameLF()+", "
				+AptCur.AptDateTime.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butAudit_Click(object sender, System.EventArgs e) {
			FormAuditOneType FormA=new FormAuditOneType(pat.PatNum,
				new Permissions[] {Permissions.AppointmentCreate,Permissions.AppointmentEdit,Permissions.AppointmentMove},
				Lan.g(this,"All Appointments for")+pat.GetNameFL());
			FormA.ShowDialog();
		}

		///<summary>Called from butOK_Click and butPin_Click</summary>
		private bool UpdateToDB(){
			if(textAddTime.errorProvider1.GetError(textAddTime)!=""
				//|| textDateTerm.errorProvider1.GetError(textDateTerm)!=""
				){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(textGradePoint.Text=="0" || textGradePoint.Text==""){
				AptCur.GradePoint=0;
			}
			else{
				try{
					AptCur.GradePoint=PIn.PFloat(textGradePoint.Text);
				}
				catch{
					MsgBox.Show(this,"Grade invalid");
					return false;
				}
			}
			if(AptCur.AptStatus==ApptStatus.Planned){
				;
			}
			else if(comboStatus.SelectedIndex==-1){
				AptCur.AptStatus=ApptStatus.Scheduled;
			}
			else{
				AptCur.AptStatus=(ApptStatus)comboStatus.SelectedIndex+1;
			}
			//if appointment is marked complete and any procedures are not,
			//then set the remaining procedures complete
      if(AptCur.AptStatus==ApptStatus.Complete){
				bool allProcsComplete=true;
				for(int i=0;i<ProcList.Length;i++){
					if(ProcList[i].AptNum!=AptCur.AptNum){
						continue;
					}
					if(ProcList[i].ProcStatus!=ProcStat.C){
						allProcsComplete=false;
						break;
					}
				}
				if(!allProcsComplete){
					if(!Security.IsAuthorized(Permissions.ProcComplCreate)){
						return false;
					}
					Procedures.SetCompleteInAppt(AptCur,PlanList,PatPlanList);
					SecurityLogs.MakeLogEntry(Permissions.ProcComplCreate,pat.PatNum,
						pat.GetNameLF()+" "+AptCur.AptDateTime.ToShortDateString());
				}
			}
			//convert from current increment into 5 minute increment
			//MessageBox.Show(strBTime.ToString());
			StringBuilder savePattern=new StringBuilder();
			for(int i=0;i<strBTime.Length;i++){
				savePattern.Append(strBTime[i]);
				savePattern.Append(strBTime[i]);
				if(PrefB.GetInt("AppointmentTimeIncrement")==15){
					savePattern.Append(strBTime[i]);
				}
			}
			if(savePattern.Length==0){
				savePattern=new StringBuilder("/");
			}
			//MessageBox.Show(savePattern.ToString());
			AptCur.Pattern=savePattern.ToString();
			if(comboUnschedStatus.SelectedIndex==0)//none
				AptCur.UnschedStatus=0;
			else
				AptCur.UnschedStatus
					=DefB.Short[(int)DefCat.RecallUnschedStatus][comboUnschedStatus.SelectedIndex-1].DefNum;
			if(comboConfirmed.SelectedIndex!=-1)
				AptCur.Confirmed
					=DefB.Short[(int)DefCat.ApptConfirmed][comboConfirmed.SelectedIndex].DefNum;
			AptCur.AddTime=(int)(PIn.PInt(textAddTime.Text)/
				PIn.PInt(((Pref)PrefB.HList["AppointmentTimeIncrement"]).ValueString));
			AptCur.Note=textNote.Text;
			if(comboClinic.SelectedIndex==0)//none
				AptCur.ClinicNum=0;
			else
				AptCur.ClinicNum=Clinics.List[comboClinic.SelectedIndex-1].ClinicNum;
			//there should always be a non-hidden primary provider for an appt.
			if(comboProvNum.SelectedIndex==-1)
				AptCur.ProvNum=Providers.List[0].ProvNum;
			else
				AptCur.ProvNum=Providers.List[comboProvNum.SelectedIndex].ProvNum;
			if(comboProvHyg.SelectedIndex==0)//none
				AptCur.ProvHyg=0;
			else
				AptCur.ProvHyg=Providers.List[comboProvHyg.SelectedIndex-1].ProvNum;
			AptCur.IsHygiene=checkIsHygiene.Checked;
			if(comboAssistant.SelectedIndex==0)//none
				AptCur.Assistant=0;
			else
				AptCur.Assistant=Employees.ListShort[comboAssistant.SelectedIndex-1].EmployeeNum;
			if(comboInstructor.SelectedIndex==0)//none
				AptCur.InstructorNum=0;
			else
				AptCur.InstructorNum=Instructors.List[comboInstructor.SelectedIndex-1].InstructorNum;
			if(comboSchoolClass.SelectedIndex==0)//none
				AptCur.SchoolClassNum=0;
			else
				AptCur.SchoolClassNum=SchoolClasses.List[comboSchoolClass.SelectedIndex-1].SchoolClassNum;
			if(comboSchoolCourse.SelectedIndex==0)//none
				AptCur.SchoolCourseNum=0;
			else
				AptCur.SchoolCourseNum=SchoolCourses.List[comboSchoolCourse.SelectedIndex-1].SchoolCourseNum;
			//AptCur.GradePoint //already done at top of this function
			AptCur.Lab=(LabCaseOld)comboLab.SelectedIndex;
			AptCur.IsNewPatient=checkIsNewPatient.Checked;
			AptCur.ProcDescript="";
			if(AptCur.AptStatus==ApptStatus.Planned){
				for(int i=0;i<ProcList.Length;i++){
					if(ProcList[i].PlannedAptNum==AptCur.AptNum){
						AptCur.ProcDescript
							+=ProcedureCodes.GetProcCode(ProcList[i].ADACode).AbbrDesc+", ";
					}
				}
			}
			else{//standard appt
				for(int i=0;i<ProcList.Length;i++){
					if(ProcList[i].AptNum==AptCur.AptNum){
						AptCur.ProcDescript
							+=ProcedureCodes.GetProcCode(ProcList[i].ADACode).AbbrDesc+", ";
					}
				}
			}
			if(AptCur.ProcDescript.Length>1){
				//trims the last space and comma
				AptCur.ProcDescript
					=AptCur.ProcDescript.Substring(0,AptCur.ProcDescript.Length-2);
			}
			try{
				Appointments.InsertOrUpdate(AptCur,AptOld,false);
				AptOld=AptCur.Copy();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return false;
			}
			if(IsNew){
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,pat.PatNum,
					pat.GetNameLF()+", "
					+AptCur.AptDateTime.ToString()
					+AptCur.ProcDescript);
			}
			else{
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,
					pat.GetNameLF()+", "
					+AptCur.AptDateTime.ToString()
					+AptCur.ProcDescript);
			}
			return true;
		}

		private void butTask_Click(object sender, System.EventArgs e) {
			if(!UpdateToDB())
				return;
			FormTaskListSelect FormT=new FormTaskListSelect(TaskObjectType.Appointment,AptCur.AptNum);
			FormT.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!UpdateToDB())
				return;
			DialogResult=DialogResult.OK;
		}

		private void butPin_Click(object sender, System.EventArgs e) {
			if(!UpdateToDB())
				return;
			PinClicked=true;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormApptEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK)
				return;
			if(IsNew){
				if(AptCur.AptStatus==ApptStatus.Planned){
					Procedures.UnattachProcsInPlannedAppt(AptCur.AptNum);
				}
				else{
					Procedures.UnattachProcsInAppt(AptCur.AptNum);
				}
				Appointments.Delete(AptCur);
				DialogResult=DialogResult.Cancel;
				//return;
			}
			else if(procsHaveChanged){
				MessageBox.Show(Lan.g(this,"Warning. Changes you made to procedures have already been saved.  Other changes will not be saved."));
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,
					pat.GetNameLF()+", "
					+AptCur.AptDateTime.ToString()+", "
					+AptCur.ProcDescript);
				DialogResult=DialogResult.OK;//so that appt module will update
				//return;
			}
		}

		

		

		

		

		

			

	}

	///<summary>Procdures within an appointment.</summary>
	public struct ApptProc{
		///<summary></summary>
		public int Index;//represents index within arrayProc
		///<summary></summary>
		public string Status;
		///<summary></summary>
		public string Priority;
		///<summary></summary>
		public string ToothNum;
		///<summary></summary>
		public string Surf;
		///<summary></summary>
		public string AbbrDesc;
		///<summary></summary>
		public string Fee;
	}//end struct ApptProc
}
