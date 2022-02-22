using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.User_Controls;

namespace OpenDental {
	public partial class FormEServicesWebSchedAdvanced:FormODBase {
		#region Fields
		///<summary>List of a List that holds information on what preferences have been changed</summary>
		private List<ClinicPref> _listClinicPrefs=new List<ClinicPref>();
		/// <summary>Used for retrieving eService data.</summary>
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		#endregion Fields

		public FormEServicesWebSchedAdvanced(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesWebSchedAdvanced_Load(object sender,EventArgs e) {
			if(_signupOut==null) {
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			FillGridApptHostedURLs();
		}

		#region Methods - Private
		private void AddClinicPrefToList(PrefName prefName,long clinicNum) {
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,clinicNum);
			if(clinicPref!=null) {
				_listClinicPrefs.Add(clinicPref);
			}
		}

		private class EServiceClinicName {
			public WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService SignupOutEService;
			public string ClinicName;
		}


		private void FillGridApptHostedURLs() {
			panelHostedURLs.Controls.Clear();
			List<Clinic> listClinicsAll=Clinics.GetDeepCopy();
			List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> listSignupOutEServices=
				WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.WebSchedNewPatAppt);
			List<EServiceClinicName> listEServiceClinicNames=new List<EServiceClinicName>();
			Clinic clinic;
			for(int i=0;i<listSignupOutEServices.Count;i++) {
				if(PrefC.HasClinicsEnabled){
					if(!listClinicsAll.Exists(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum)) {
						continue;
					}
					if(listClinicsAll.Find(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum).IsHidden) {
						continue;
					}
				}
				else {//clinics off
					if(listSignupOutEServices[i].ClinicNum!=0) {//only show headquarters
						continue;
					}
				}
				EServiceClinicName eServiceClinicName=new EServiceClinicName();
				clinic=listClinicsAll.FirstOrDefault(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum);
				if(clinic==null) {
					eServiceClinicName.ClinicName="N\\A";
				}
				else {
					eServiceClinicName.ClinicName=clinic.Abbr;
				}
				eServiceClinicName.SignupOutEService=listSignupOutEServices[i];
				listEServiceClinicNames.Add(eServiceClinicName);
			}
			listEServiceClinicNames=listEServiceClinicNames.OrderBy(x => x.ClinicName).ToList();
			_listClinicPrefs.Clear();
			for(int i=0;i<listEServiceClinicNames.Count();i++) {
				UserControlHostedURL userControlHostedURL=new UserControlHostedURL(listEServiceClinicNames[i].SignupOutEService);
				userControlHostedURL.LayoutManager=LayoutManager;
				if(!PrefC.HasClinicsEnabled || listEServiceClinicNames.Count()==1) {
					userControlHostedURL.IsExpanded=true;
					userControlHostedURL.DoHideExpandButton=true;
				}
				else{
					userControlHostedURL.IsExpanded=false;
					userControlHostedURL.DoHideExpandButton=false;
				}
				Lan.C(this,userControlHostedURL);
				LayoutManager.AddUnscaled(userControlHostedURL,panelHostedURLs);
				LayoutManager.MoveWidth(userControlHostedURL,LayoutManager.Scale(userControlHostedURL.Width));
				if(listEServiceClinicNames[i].SignupOutEService.ClinicNum==0) {
					continue;
				}
				else {
					AddClinicPrefToList(PrefName.WebSchedNewPatAllowChildren,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthEmail,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthText,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatWebFormsURL,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatWebFormsURL,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthEmail,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthText,listEServiceClinicNames[i].SignupOutEService.ClinicNum);
				}
			}
		}

		private ClinicPref GetClinicPrefToSave(long clinicNum,PrefName prefName,string value) {
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName)?.Clone();
			if(clinicPref==null) {
				return new ClinicPref(clinicNum,prefName,value);
			}
			clinicPref.ValueString=value;
			return clinicPref;
		}
		#endregion Methods - Private

		private void SaveWebSchedAdvanced() {
			List<ClinicPref> listClinicPrefs=new List<ClinicPref>();
			for(int i=0;i<panelHostedURLs.Controls.Count;i++) {
				if(panelHostedURLs.Controls[i].GetType()!=typeof(UserControlHostedURL)) {
					continue;
				}
				UserControlHostedURL userControlHostedURL=(UserControlHostedURL)panelHostedURLs.Controls[i];
				long clinicNum=userControlHostedURL.GetClinicNum();
				string strAllowChildren=userControlHostedURL.GetPrefValue(PrefName.WebSchedNewPatAllowChildren);
				string strNewPatDoAuthEmail=userControlHostedURL.GetPrefValue(PrefName.WebSchedNewPatDoAuthEmail);
				string strNewPatDoAuthText=userControlHostedURL.GetPrefValue(PrefName.WebSchedNewPatDoAuthText);
				string strExistingPatDoAuthEmail=userControlHostedURL.GetPrefValue(PrefName.WebSchedExistingPatDoAuthEmail);
				string strExistingPatDoAuthText=userControlHostedURL.GetPrefValue(PrefName.WebSchedExistingPatDoAuthText);
				string webFormsURL=userControlHostedURL.GetPrefValue(PrefName.WebSchedNewPatWebFormsURL);
				string webFormsExistingPatURL=userControlHostedURL.GetPrefValue(PrefName.WebSchedExistingPatWebFormsURL);
				if(clinicNum==0) {
					Prefs.UpdateString(PrefName.WebSchedNewPatAllowChildren,strAllowChildren);
					Prefs.UpdateString(PrefName.WebSchedNewPatDoAuthEmail,strNewPatDoAuthEmail);
					Prefs.UpdateString(PrefName.WebSchedNewPatDoAuthText,strNewPatDoAuthText);
					Prefs.UpdateString(PrefName.WebSchedExistingPatDoAuthEmail,strExistingPatDoAuthEmail);
					Prefs.UpdateString(PrefName.WebSchedExistingPatDoAuthText,strExistingPatDoAuthText);
					Prefs.UpdateString(PrefName.WebSchedNewPatWebFormsURL,webFormsURL);
					Prefs.UpdateString(PrefName.WebSchedExistingPatWebFormsURL,webFormsExistingPatURL);
					continue;
				}
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatAllowChildren,strAllowChildren));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatDoAuthEmail,strNewPatDoAuthEmail));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatDoAuthText,strNewPatDoAuthText));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedExistingPatDoAuthEmail,strExistingPatDoAuthEmail));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedExistingPatDoAuthText,strExistingPatDoAuthText));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatWebFormsURL,webFormsURL));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedExistingPatWebFormsURL,webFormsExistingPatURL));
			}
			if(ClinicPrefs.Sync(listClinicPrefs,_listClinicPrefs)) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveWebSchedAdvanced();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}