using CodeBase;
using Microsoft.Win32;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Linq;
using System.IO;
using WebServiceSerializer;
using OpenDentBusiness.WebServiceMainHQ;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDental {

	public partial class FormEServicesMisc:FormODBase {
		private string _formatShortDate="d";
		private string _formatLongDate="D";
		private string _formatMMMMdyyyy="MMMM d, yyyy";
		private string _formatM="m";
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		///<summary>Keeps track if the user selected the Short Date or Long Date format.</summary>
		private bool _wasShortOrLongDateClicked=false;
		
		public FormEServicesMisc(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesMisc_Load(object sender,EventArgs e) {
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			//.NET has a bug in the DateTimePicker control where the text will not get updated and will instead default to showing DateTime.Now.
			//In order to get the control into a mode where it will display the correct value that we set, we need to set the property Checked to true.
			//Today's date will show even when the property is defaulted to true (via the designer), so we need to do it programmatically right here.
			//E.g. set your computer region to Assamese (India) and the DateTimePickers on the Automation Setting tab will both be set to todays date
			// if the tab is NOT set to be the first tab to display (don't ask me why it works then).
			//This is bad for our customers because setting both of the date pickers to the same date and time will cause automation to stop.
			dateRunStart.Checked=true;
			dateRunEnd.Checked=true;
			//Now that the DateTimePicker controls are ready to display the DateTime we set, go ahead and set them.
			//If loading the picker controls with the DateTime fields from the database failed, the date picker controls default to 7 AM and 10 PM.
			ODException.SwallowAnyException(() => {
				dateRunStart.Value=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeStart);
				dateRunEnd.Value=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeEnd);
			});
			labelDateCustom.Text="";
			radioDateShortDate.Text=DateTime.Today.ToString(_formatShortDate);//Formats as '3/15/2018'
			radioDateLongDate.Text=DateTime.Today.ToString(_formatLongDate);//Formats as 'Thursday, March 15, 2018'
			radioDateMMMMdyyyy.Text=DateTime.Today.ToString(_formatMMMMdyyyy);//Formats as 'March 15, 2018'
			radioDatem.Text=DateTime.Today.ToString(_formatM);//Formats as 'March 15'
			string format=PrefC.GetString(PrefName.PatientCommunicationDateFormat);
			if(format==CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) {
				format=_formatShortDate;
			}
			if(format==CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern) {
				format=_formatLongDate;
			}
			if(format==_formatShortDate) {
				radioDateShortDate.Checked=true;
			}
			else if(format==_formatLongDate) {
				radioDateLongDate.Checked=true;
			}
			else if(format==_formatMMMMdyyyy) {
				radioDateMMMMdyyyy.Checked=true;
			}
			else if(format==_formatM) {
				radioDatem.Checked=true;
			}
			else {
					radioDateCustom.Checked=true;
					textDateCustom.Text=PrefC.GetString(PrefName.PatientCommunicationDateFormat);
			}
			DateTime dateLastRun=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			//Hide the Old-style Mobile Synch components 
			//only if mobile synch has not been used for at least a month. If used again, the clock will reset
			if(MiscData.GetNowDateTime().Subtract(dateLastRun.Date).TotalDays>30) {
				groupNotUsed.Visible=true;
				butShowOldMobileSych.Visible=true;
			}
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,suppressMessage:true);
			dateRunStart.Enabled=allowEdit;
			dateRunEnd.Enabled=allowEdit;
			groupDateFormat.Enabled=allowEdit;
		}

		private void SaveTabMisc() {
			Prefs.UpdateDateT(PrefName.AutomaticCommunicationTimeStart,dateRunStart.Value);
			Prefs.UpdateDateT(PrefName.AutomaticCommunicationTimeEnd,dateRunEnd.Value);
			string format=PrefC.GetString(PrefName.PatientCommunicationDateFormat);
			string formatDate;
			if(radioDateShortDate.Checked) {
				if(_wasShortOrLongDateClicked) {
					formatDate=CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
				}
				else {
					formatDate=format;//If the user didn't actually select this, we'll keep the pattern what it was before.
				}
			}
			else if(radioDateLongDate.Checked) {
				if(_wasShortOrLongDateClicked) {
					formatDate=CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
				}
				else {
					formatDate=format;//If the user didn't actually select this, we'll keep the pattern what it was before.
				}
			}
			else if(radioDateMMMMdyyyy.Checked) {
				formatDate=_formatMMMMdyyyy;
			}
			else if(radioDatem.Checked) {
				formatDate=_formatM;
			}
			else {
				formatDate=textDateCustom.Text;
			}
			Prefs.UpdateString(PrefName.PatientCommunicationDateFormat,formatDate);
		}

		private void textDateCustom_TextChanged(object sender,EventArgs e) {
			if(textDateCustom.Text.Trim()=="") {
				labelDateCustom.Text="";
				return;
			}
			try {
				labelDateCustom.Text=DateTime.Now.ToString(textDateCustom.Text);
			}
			catch(Exception ex) {
				ex.DoNothing();
				labelDateCustom.Text="";
			}
		}

		private void radioDateFormat_Click(object sender,EventArgs e) {
			_wasShortOrLongDateClicked=true;
		}

		private void butShowOldMobileSych_Click(object sender,EventArgs e) {
			using FormEServicesMobileSynch formEServicesMobileSynch=new FormEServicesMobileSynch();
			formEServicesMobileSynch.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(radioDateCustom.Checked) {
				bool isValidFormat=true;
				if(textDateCustom.Text.Trim()=="") {
					isValidFormat=false;
				}
				try {
					DateTime.Today.ToString(textDateCustom.Text);
				}
				catch(Exception ex) {
					ex.DoNothing();
					isValidFormat=false;
				}
				if(!isValidFormat) {
					MsgBox.Show(this,"Please enter a valid format in the Custom date format text box.");
					return;
				}
			}
			SaveTabMisc();
			DialogResult=DialogResult.OK;	
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void textDateCustom_KeyPress(object sender,KeyPressEventArgs e) {
			Regex regex=new Regex(@"[0-9]");
			//Block numerical characters to prevent users from accidentally inputting dates rather than a date format.
			if(regex.IsMatch(e.KeyChar.ToString())) {
				e.Handled=true;
			}
		}
	}
}