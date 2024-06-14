using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientPortal:FormODBase {
		private Patient _patient;
		///<summary>The current UserWeb instance from the passed in Patient.</summary>
		private UserWeb _userWeb;
		///<summary>The unmodified UserWeb instance to compare to the current one when saving changes to the database.</summary>
		private UserWeb _userWebOld;
		///<summary>Keeps track if the user printed the patient's information.  Mainly used to show a reminder when the password changes and the user didn't print.</summary>
		private bool _wasPrinted;
		private bool _isNew;

		public FormPatientPortal(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patient;
		}

		private void FormPatientPortal_Load(object sender,EventArgs e) {
			_userWeb=UserWebs.GetByFKeyAndType(_patient.PatNum,UserWebFKeyType.PatientPortal);
			if(_userWeb==null) {
				_isNew=true;
				_userWeb=new UserWeb();
				_userWeb.UserName=UserWebs.CreateUserNameFromPat(_patient,UserWebFKeyType.PatientPortal,new List<string>());
				_userWeb.FKey=_patient.PatNum;
				_userWeb.FKeyType=UserWebFKeyType.PatientPortal;
				_userWeb.RequireUserNameChange=true;
				_userWeb.Password=Authentication.GeneratePasswordHash("",HashTypes.None);
				UserWebs.Insert(_userWeb);
			}
			_userWebOld=_userWeb.Copy();
			textOnlineUsername.Text=_userWeb.UserName;
			textOnlinePassword.Text="";
			if(_userWeb.PasswordHash!="") {//if a password was already filled in
				butGiveAccess.Text="Remove Online Access";
				//We do not want to show the password hash that is stored in the database so we will fill the online password with asterisks.
				textOnlinePassword.Text="********";
				textOnlinePassword.ReadOnly=false;
				textOnlineUsername.ReadOnly=false;
			}
			textPatientPortalURL.Text=PrefC.GetString(PrefName.PatientPortalURL);
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormEServicesPatientPortal formEServicesPatientPortal=new FormEServicesPatientPortal();
			formEServicesPatientPortal.ShowDialog();
			textPatientPortalURL.Text=PrefC.GetString(PrefName.PatientPortalURL);
		}

		private void butGiveAccess_Click(object sender,EventArgs e) {
			if(butGiveAccess.Text=="Provide Online Access") {//When form open opens with a blank password
				if(PrefC.GetString(PrefName.PatientPortalURL)=="") {
					//User probably hasn't set up the patient portal yet.
					MsgBox.Show(this,"Patient Facing URL is required to be set before granting online access.  Click Setup to set the Patient Facing URL.");
					return;
				}
				string error=UserWebs.ValidatePatientAccess(_patient);
				if(!String.IsNullOrEmpty(error)) { 
					MessageBox.Show(error);
					return;
				}
				Cursor=Cursors.WaitCursor;
				//1. Fill password.
				string passwordGenerated=UserWebs.GenerateRandomPassword(8);
				textOnlinePassword.Text=passwordGenerated;
				//2. Make the username and password editable in case they want to change it.
				textOnlineUsername.ReadOnly=false;
				textOnlinePassword.ReadOnly=false;
				//3. Save password to db.
				// We only save the hash of the generated password.
				_userWeb.LoginDetails=Authentication.GenerateLoginDetailsSHA512(passwordGenerated);
				UserWebs.Update(_userWeb,_userWebOld);
				_userWebOld.LoginDetails=_userWeb.LoginDetails;
				//4. Insert EhrMeasureEvent
				EhrMeasureEvent ehrMeasureEventNew=new EhrMeasureEvent();
				ehrMeasureEventNew.DateTEvent=DateTime.Now;
				ehrMeasureEventNew.EventType=EhrMeasureEventType.OnlineAccessProvided;
				ehrMeasureEventNew.PatNum=_userWeb.FKey;
				ehrMeasureEventNew.MoreInfo="";
				EhrMeasureEvents.Insert(ehrMeasureEventNew);
				//5. Rename button
				butGiveAccess.Text="Remove Online Access";
				Cursor=Cursors.Default;
			}
			else {//remove access
				Cursor=Cursors.WaitCursor;
				//1. Clear password
				textOnlinePassword.Text="";
				//2. Make in uneditable
				textOnlinePassword.ReadOnly=true;
				//3. Save password to db
				_userWeb.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textOnlinePassword.Text);
				UserWebs.Update(_userWeb,_userWebOld);
				_userWebOld.LoginDetails=_userWeb.LoginDetails;
				//4. Rename button
				butGiveAccess.Text="Provide Online Access";
				Cursor=Cursors.Default;
			}
		}

		private void butOpen_Click(object sender,EventArgs e) {
			if(textPatientPortalURL.Text=="") {
				MessageBox.Show("Please use Setup to set the Online Access Link first.");
				return;
			}
			try {
				System.Diagnostics.Process.Start(textPatientPortalURL.Text);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		
		private void butGenerate_Click(object sender,EventArgs e) {
			if(textOnlinePassword.ReadOnly) {
				MessageBox.Show("Please use the Provide Online Access button first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string passwordGenerated=UserWebs.GenerateRandomPassword(8);
			textOnlinePassword.Text=passwordGenerated;
			// We only save the hash of the generated password.
			_userWeb.LoginDetails=Authentication.GenerateLoginDetailsSHA512(passwordGenerated);
			UserWebs.Update(_userWeb,_userWebOld);
			_userWebOld.LoginDetails=_userWeb.LoginDetails;
			Cursor=Cursors.Default;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(textPatientPortalURL.Text=="") {
				MsgBox.Show(this,"Online Access Link required. Please use Setup to set the Online Access Link first.");
				return;
			}
			if(textOnlinePassword.Text=="" || textOnlinePassword.Text=="********") {
				MessageBox.Show("Password required. Please generate a new password.");
				return;
			}
			string error=Patients.IsPortalPasswordValid(textOnlinePassword.Text);
			if(error!="") {//Non-empty string means it was invalid.
				MessageBox.Show(this,error);
				return;
			}
			_wasPrinted=true;
			//Then, print the info that the patient will be given in order for them to log in online.
			PrintPatientInfo();
		}

		private void PrintPatientInfo() {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Patient portal login information printed"),auditPatNum:_patient.PatNum);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font font=new Font("Arial",10,FontStyle.Regular);
			int yPos=bounds.Top+100;
			int center=bounds.X+bounds.Width/2;
			text="Online Access";
			g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,font).Width/2,yPos);
			yPos+=50;
			text="Website: "+textPatientPortalURL.Text;
			g.DrawString(text,font,Brushes.Black,bounds.Left+100,yPos);
			yPos+=25;
			text="Username: "+textOnlineUsername.Text;
			g.DrawString(text,font,Brushes.Black,bounds.Left+100,yPos);
			yPos+=25;
			text="Password: "+textOnlinePassword.Text;
			g.DrawString(text,font,Brushes.Black,bounds.Left+100,yPos);
			g.Dispose();
			e.HasMorePages=false;
		}

		private void butSave_Click(object sender,EventArgs e) {
			bool shouldUpdateUserWeb=false;
			bool shouldPrint=false;
			if(textOnlineUsername.ReadOnly==false) {
				if(textOnlineUsername.Text=="") {
					MsgBox.Show(this,"Online Username cannot be blank.");
					return;
				}
				else if(_userWeb.UserName!=textOnlineUsername.Text) {
					if(UserWebs.UserNameExists(textOnlineUsername.Text,UserWebFKeyType.PatientPortal)) {
						MsgBox.Show(this,"The Online Username already exists.");
						return;
					}
					_userWeb.UserName=textOnlineUsername.Text;
					shouldUpdateUserWeb=true;
					if(!_wasPrinted) {
						shouldPrint=true;
					}
				}
			}
			if(textOnlinePassword.Text!="" && textOnlinePassword.Text!="********") {
				string error=Patients.IsPortalPasswordValid(textOnlinePassword.Text);
				if(error!="") {//Non-empty string means it was invalid.
					MessageBox.Show(this,error);
					return;
				}
				if(!_wasPrinted) {
					shouldPrint=true;
				}
				shouldUpdateUserWeb=true;
				_userWeb.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textOnlinePassword.Text);
			}
			if(shouldPrint) {
				DialogResult dialogResult=MessageBox.Show(Lan.g(this,"Online Username or Password changed but was not printed, would you like to print?")
					,Lan.g(this,"Print Patient Info")
					,MessageBoxButtons.YesNoCancel);
				if(dialogResult==DialogResult.Yes) {
					//Print the showing information.
					PrintPatientInfo();
				}
				else if(dialogResult==DialogResult.No) {
					//User does not want to print.  Do nothing.
				}
				else if(dialogResult==DialogResult.Cancel) {
					return;
				}
			}
			if(shouldUpdateUserWeb) {
				UserWebs.Update(_userWeb,_userWebOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void FormPatientPortal_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.Cancel) {
				return;
			}
			if(_isNew) {
				UserWebs.Delete(_userWeb.UserWebNum);
			}
		}

	}
}