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

	public partial class FormEServicesTexting:FormODBase {
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

		private Clinic GetSmsClinicSelected() {
			if(gridSmsSummary.GetSelectedIndex()<0) {
				return new Clinic();
			}
			return ((Clinic)gridSmsSummary.ListGridRows[gridSmsSummary.GetSelectedIndex()].Tag)??new Clinic();
		}

		public FormEServicesTexting(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesTexting_Load(object sender,EventArgs e) {
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			butDefaultClinic.Visible=PrefC.HasClinicsEnabled;
			butDefaultClinicClear.Visible=PrefC.HasClinicsEnabled;
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			butDefaultClinic.Enabled=allowEdit;
			butDefaultClinicClear.Enabled=allowEdit;
			FillGridSmsUsage();
			FillOptInSettings();
		}

		private void FillOptInSettings() {
			labelUnsavedShortCodeChanges.Visible=false;
			checkOptInPrompt.Checked=ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,comboShortCodeClinic.SelectedClinicNum);
			textShortCodeOptInClinicTitle.Text=GetShortCodeOptInClinicTitle();
		}

		///<summary></summary>
		private string GetShortCodeOptInClinicTitle() {			
			//Clinic 0 will be saved as a ClinicPref, not a practice wide Pref, so do not includeDefault here.
			string title=ClinicPrefs.GetPrefValue(PrefName.ShortCodeOptInClinicTitle,comboShortCodeClinic.SelectedClinicNum);
			if(!string.IsNullOrWhiteSpace(title)) {
				return title;
			}
			title=Clinics.GetDesc(comboShortCodeClinic.SelectedClinicNum);
			if(comboShortCodeClinic.SelectedClinicNum==0) {
				title=PrefC.GetString(PrefName.PracticeTitle);
			}
			if(!string.IsNullOrWhiteSpace(title)) {
				return title;
			}
			//In case a clinic had an empty Description
			return PrefC.GetString(PrefName.PracticeTitle);
		}

		private void comboShortCodeClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillOptInSettings();
		}		

		private void textShortCodeOptInClinicTitle_TextChanged(object sender,EventArgs e) {
			labelUnsavedShortCodeChanges.Visible=AreShortCodeSettingsUnsaved();
		}

		private void checkOptInPrompt_Click(object sender,EventArgs e) {
			if(!checkOptInPrompt.Checked) {
				string script=PrefC.GetString(PrefName.ShortCodeOptInOnApptCompleteOffScript);
				if(string.IsNullOrWhiteSpace(script)) {
					script="By disabling this prompt, you agree to obtain verbal confirmation from patients to send Appt Texts.";
				}
				string prompt=Lan.g(this,script);
				MsgBox.Show(this,prompt);
			}
			labelUnsavedShortCodeChanges.Visible=AreShortCodeSettingsUnsaved();
		}

		private bool AreShortCodeSettingsUnsaved() {
			return textShortCodeOptInClinicTitle.Text!=GetShortCodeOptInClinicTitle() 
				|| checkOptInPrompt.Checked!=ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,comboShortCodeClinic.SelectedClinicNum);
		}

		private void butSaveShortCodes_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textShortCodeOptInClinicTitle.Text)) {
				string err=Lan.g(this,"Not allowed to set ")+labelShortCodeOptInClinicTitle.Text+Lan.g(this," to an empty value.");
				MessageBox.Show(err);
				return;
			}
			bool doSetInvalidClinicPrefs=false;
			bool doSetInvalidPrefs=false;
			//Only Update/Upsert if the textbox doesn't match the current setting.
			if(textShortCodeOptInClinicTitle.Text!=GetShortCodeOptInClinicTitle()) {
				if(comboShortCodeClinic.SelectedClinicNum==0) {
					doSetInvalidPrefs=Prefs.UpdateString(PrefName.ShortCodeOptInClinicTitle,textShortCodeOptInClinicTitle.Text);
				}
				else {
					doSetInvalidClinicPrefs=ClinicPrefs.Upsert(PrefName.ShortCodeOptInClinicTitle,comboShortCodeClinic.SelectedClinicNum,
						textShortCodeOptInClinicTitle.Text);
				}
			}
			if(comboShortCodeClinic.SelectedClinicNum==0) {
					doSetInvalidPrefs|=Prefs.UpdateBool(PrefName.ShortCodeOptInOnApptComplete,checkOptInPrompt.Checked);
				}
			else {
				doSetInvalidClinicPrefs|=ClinicPrefs.Upsert(PrefName.ShortCodeOptInOnApptComplete,comboShortCodeClinic.SelectedClinicNum
					,POut.Bool(checkOptInPrompt.Checked));
			}
			if(doSetInvalidPrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			FillOptInSettings();
		}

		private void butDefaultClinic_Click(object sender,EventArgs e) {
			if(GetSmsClinicSelected().ClinicNum==0) {
				MsgBox.Show(this,"Select clinic to make default.");
				return;
			}
			Prefs.UpdateLong(PrefName.TextingDefaultClinicNum,(GetSmsClinicSelected().ClinicNum));
			Signalods.SetInvalid(InvalidType.Prefs);
			FillGridSmsUsage();
		}

		private void butDefaultClinicClear_Click(object sender,EventArgs e) {
			Prefs.UpdateLong(PrefName.TextingDefaultClinicNum,0);
			Signalods.SetInvalid(InvalidType.Prefs);
			FillGridSmsUsage();
		}

		private class EServicesSmsPhone {
			public long ClinicNum;
			public string PhoneNumber;
			public string CountryCode;
			public int SentMonth;
			public double SentCharge;
			public double SentDiscount;
			public double SentPreDiscount;
			public int RcvMonth;
			public double RcvCharge;
		}

		private void FillGridSmsUsage() {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
			if(!PrefC.HasClinicsEnabled) { //No clinics so just get the practice as a clinic.
				listClinics.Clear();
				listClinics.Add(Clinics.GetPracticeAsClinicZero());
			}
			DataTable tablePhones=SmsPhones.GetSmsUsageLocal(listClinics.Select(x => x.ClinicNum).ToList(),dateTimePickerSms.Value,
				WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutPhone.ToSmsPhones(_signupOut.Phones));
			List<EServicesSmsPhone> listEServicesSmsPhones=tablePhones.Rows.Cast<DataRow>().Select(x => new EServicesSmsPhone {
					ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
					PhoneNumber=x["PhoneNumber"].ToString(),
					CountryCode=x["CountryCode"].ToString(),
					SentMonth=PIn.Int(x["SentMonth"].ToString()),
					SentCharge=PIn.Double(x["SentCharge"].ToString()),
					SentDiscount=PIn.Double(x["SentDiscount"].ToString()),
					SentPreDiscount=PIn.Double(x["SentPreDiscount"].ToString()),
					RcvMonth=PIn.Int(x["ReceivedMonth"].ToString()),
					RcvCharge=PIn.Double(x["ReceivedCharge"].ToString())
			}).ToList();
			bool doShowDiscount=listEServicesSmsPhones.Any(x => CompareDouble.IsGreaterThan(x.SentDiscount,0));
			gridSmsSummary.BeginUpdate();
			gridSmsSummary.ListGridColumns.Clear();
			if(PrefC.HasClinicsEnabled) {
				gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Default"),80) { TextAlign=HorizontalAlignment.Center });
			}
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Location"),170,HorizontalAlignment.Left));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Subscribed"),80,HorizontalAlignment.Center));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Primary\r\nPhone Number"),105,HorizontalAlignment.Center));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Country\r\nCode"),60,HorizontalAlignment.Center));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Limit"),80,HorizontalAlignment.Right));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Sent\r\nFor Month"),70,HorizontalAlignment.Right));
			if(doShowDiscount) {
				gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Sent\r\nPre-Discount"),80,HorizontalAlignment.Right));
				gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Sent\r\nDiscount"),70,HorizontalAlignment.Right));
			}
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Sent\r\nCharges"),70,HorizontalAlignment.Right));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Received\r\nFor Month"),70,HorizontalAlignment.Right));
			gridSmsSummary.ListGridColumns.Add(new GridColumn(Lan.g(this,"Received\r\nCharges"),70,HorizontalAlignment.Right));
			gridSmsSummary.ListGridRows.Clear();
			for(int i=0;i<listClinics.Count;i++) {
				GridRow row=new GridRow();
				if(PrefC.HasClinicsEnabled) { //Default texting clinic?
					row.Cells.Add(listClinics[i].ClinicNum==PrefC.GetLong(PrefName.TextingDefaultClinicNum) ? "X" : "");
				}
				row.Cells.Add(listClinics[i].Abbr); //Location.
				EServicesSmsPhone eServicesSmsPhone=listEServicesSmsPhones.FirstOrDefault(x => x.ClinicNum==listClinics[i].ClinicNum);
				if(eServicesSmsPhone==null) {
					row.Cells.Add("No");//subscribed
					row.Cells.Add("");//phone number
					row.Cells.Add("");//country code
					row.Cells.Add((0f).ToString("c",new CultureInfo("en-US")));//montly limit
					row.Cells.Add("0");//Sent Month
					if(doShowDiscount) {
						row.Cells.Add((0f).ToString("c",new CultureInfo("en-US")));//Sent Pre-Discount
						row.Cells.Add((0f).ToString("c",new CultureInfo("en-US")));//Sent Discount
					}
					row.Cells.Add((0f).ToString("c",new CultureInfo("en-US")));//Sent Charge
					row.Cells.Add("0");//Rcvd Month
					row.Cells.Add((0f).ToString("c",new CultureInfo("en-US")));//Rcvd Charge
				}
				else {
					row.Cells.Add(listClinics[i].SmsContractDate.Year>1800 ? Lan.g(this,"Yes") : Lan.g(this,"No"));
					row.Cells.Add(eServicesSmsPhone.PhoneNumber);
					row.Cells.Add(eServicesSmsPhone.CountryCode);
					row.Cells.Add(listClinics[i].SmsMonthlyLimit.ToString("c",new CultureInfo("en-US")));//Charge this month (Must always be in USD)
					row.Cells.Add(eServicesSmsPhone.SentMonth.ToString());
					if(doShowDiscount) {
						row.Cells.Add(eServicesSmsPhone.SentPreDiscount.ToString("c",new CultureInfo("en-US")));
						row.Cells.Add(eServicesSmsPhone.SentDiscount.ToString("c",new CultureInfo("en-US")));
					}
					row.Cells.Add(eServicesSmsPhone.SentCharge.ToString("c",new CultureInfo("en-US")));
					row.Cells.Add(eServicesSmsPhone.RcvMonth.ToString());
					row.Cells.Add(eServicesSmsPhone.RcvCharge.ToString("c",new CultureInfo("en-US")));
				}
				row.Tag=listClinics[i];
				gridSmsSummary.ListGridRows.Add(row);
			}
			if(listClinics.Count>1) {//Total row if there is more than one clinic (Will not display for practice because practice will have no clinics.
				GridRow row=new GridRow();
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add(Lans.g(this,"Total"));
				row.Cells.Add(listClinics.Where(x => listEServicesSmsPhones.Any(y => y.ClinicNum==x.ClinicNum)).Sum(x => x.SmsMonthlyLimit).ToString("c",new CultureInfo("en-US")));
				row.Cells.Add(listEServicesSmsPhones.Sum(x => x.SentMonth).ToString());
				if(doShowDiscount) {
					row.Cells.Add(listEServicesSmsPhones.Sum(x => x.SentPreDiscount).ToString("c",new CultureInfo("en-US")));
					row.Cells.Add(listEServicesSmsPhones.Sum(x => x.SentDiscount).ToString("c",new CultureInfo("en-US")));
				}
				row.Cells.Add(listEServicesSmsPhones.Sum(x => x.SentCharge).ToString("c",new CultureInfo("en-US")));
				row.Cells.Add(listEServicesSmsPhones.Sum(x => x.RcvMonth).ToString());
				row.Cells.Add(listEServicesSmsPhones.Sum(x => x.RcvCharge).ToString("c",new CultureInfo("en-US")));
				row.ColorBackG=Color.LightYellow;
				gridSmsSummary.ListGridRows.Add(row);
			}
			gridSmsSummary.EndUpdate();
		}

		private void butBackMonth_Click(object sender,EventArgs e) {
			dateTimePickerSms.Value=dateTimePickerSms.Value.AddMonths(-1);
		}

		private void butFwdMonth_Click(object sender,EventArgs e) {
			dateTimePickerSms.Value=dateTimePickerSms.Value.AddMonths(1);//triggers refresh
		}

		private void butThisMonth_Click(object sender,EventArgs e) {
			dateTimePickerSms.Value=DateTime.Now.Date;//triggers refresh
		}

		private void dateTimePickerSms_ValueChanged(object sender,EventArgs e) {
			FillGridSmsUsage();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}