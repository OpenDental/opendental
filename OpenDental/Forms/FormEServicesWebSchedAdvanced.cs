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
		private List<ClinicPref> _listClinicPrefsWebSched=new List<ClinicPref>();
		/// <summary>Used for retrieving eService data.</summary>
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
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
				_listClinicPrefsWebSched.Add(clinicPref);
			}
		}

		private class EServiceClinicName {
			public WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService Signup;
			public string ClinicName;
		}


		private void FillGridApptHostedURLs() {
			panelHostedURLs.Controls.Clear();
			List<Clinic> listClinicsAll=Clinics.GetDeepCopy();
			List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> listSignupOutEServices=
				WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.WebSchedNewPatAppt);
			List<EServiceClinicName> listEServiceClinicName=new List<EServiceClinicName>();
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
				eServiceClinicName.Signup=listSignupOutEServices[i];
				listEServiceClinicName.Add(eServiceClinicName);
			}
			listEServiceClinicName=listEServiceClinicName.OrderBy(x => x.ClinicName).ToList();
			_listClinicPrefsWebSched.Clear();
			for(int i=0;i<listEServiceClinicName.Count();i++) {
				UserControlHostedURL userControlHostedURLNewPat=new UserControlHostedURL(listEServiceClinicName[i].Signup);
				userControlHostedURLNewPat.LayoutManager=LayoutManager;
				if(!PrefC.HasClinicsEnabled || listEServiceClinicName.Count()==1) {
					userControlHostedURLNewPat.IsExpanded=true;
					userControlHostedURLNewPat.DoHideExpandButton=true;
				}
				else{
					userControlHostedURLNewPat.IsExpanded=false;
					userControlHostedURLNewPat.DoHideExpandButton=false;
				}
				Lan.C(this,userControlHostedURLNewPat);
				LayoutManager.AddUnscaled(userControlHostedURLNewPat,panelHostedURLs);
				LayoutManager.MoveWidth(userControlHostedURLNewPat,LayoutManager.Scale(userControlHostedURLNewPat.Width));
				if(listEServiceClinicName[i].Signup.ClinicNum==0) {
					continue;
				}
				else {
					AddClinicPrefToList(PrefName.WebSchedNewPatAllowChildren,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthEmail,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthText,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatWebFormsURL,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatWebFormsURL,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthEmail,listEServiceClinicName[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthText,listEServiceClinicName[i].Signup.ClinicNum);
				}
			}
		}

		private ClinicPref GetClinicPrefToSave(long clinicNum,PrefName prefName,string value) {
			ClinicPref clinicPref=_listClinicPrefsWebSched.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName)?.Clone();
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
				UserControlHostedURL urlPanel=(UserControlHostedURL)panelHostedURLs.Controls[i];
				long clinicNum=urlPanel.GetClinicNum();
				string strAllowChildren=urlPanel.GetPrefValue(PrefName.WebSchedNewPatAllowChildren);
				string strNewPatDoAuthEmail=urlPanel.GetPrefValue(PrefName.WebSchedNewPatDoAuthEmail);
				string strNewPatDoAuthText=urlPanel.GetPrefValue(PrefName.WebSchedNewPatDoAuthText);
				string strExistingPatDoAuthEmail=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatDoAuthEmail);
				string strExistingPatDoAuthText=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatDoAuthText);
				string webFormsURL=urlPanel.GetPrefValue(PrefName.WebSchedNewPatWebFormsURL);
				string webFormsExistingPatURL=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatWebFormsURL);
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
			if(ClinicPrefs.Sync(listClinicPrefs,_listClinicPrefsWebSched)) {
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