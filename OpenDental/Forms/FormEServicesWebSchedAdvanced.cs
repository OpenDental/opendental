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

		private void FillGridApptHostedURLs() {
			panelHostedURLs.Controls.Clear();
			List<Clinic> clinicsAll=Clinics.GetDeepCopy();
			var eServiceData=WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.WebSchedNewPatAppt)
				.Select(x => new {
					Signup=x,
					ClinicName=(clinicsAll.FirstOrDefault(y => y.ClinicNum==x.ClinicNum)??new Clinic() { Abbr="N\\A" }).Abbr
				})
				.Where(x => 
					//When clinics off, only show headquarters
					(!PrefC.HasClinicsEnabled && x.Signup.ClinicNum==0) || 
					//When clinics are on, only show if not hidden.
					(PrefC.HasClinicsEnabled && clinicsAll.Any(y => y.ClinicNum==x.Signup.ClinicNum && !y.IsHidden))
				)
				//Alpha sorted
				.OrderBy(x => x.ClinicName)
				.ToList();
			_listClinicPrefsWebSched.Clear();
			for(int i=0;i<eServiceData.Count();i++) {
				UserControlHostedURL contrNewPatHostedURL=new UserControlHostedURL(eServiceData[i].Signup);
				contrNewPatHostedURL.LayoutManager=LayoutManager;
				if(!PrefC.HasClinicsEnabled || eServiceData.Count()==1) {
					contrNewPatHostedURL.IsExpanded=true;
					contrNewPatHostedURL.DoHideExpandButton=true;
				}
				else{
					contrNewPatHostedURL.IsExpanded=false;
					contrNewPatHostedURL.DoHideExpandButton=false;
				}
				Lan.C(this,contrNewPatHostedURL);
				LayoutManager.AddUnscaled(contrNewPatHostedURL,panelHostedURLs);
				LayoutManager.MoveWidth(contrNewPatHostedURL,LayoutManager.Scale(contrNewPatHostedURL.Width));
				if(eServiceData[i].Signup.ClinicNum==0) {
					continue;
				}
				else {
					AddClinicPrefToList(PrefName.WebSchedNewPatAllowChildren,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthEmail,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthText,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedNewPatWebFormsURL,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatWebFormsURL,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthEmail,eServiceData[i].Signup.ClinicNum);
					AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthText,eServiceData[i].Signup.ClinicNum);
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
				string allowChildren=urlPanel.GetPrefValue(PrefName.WebSchedNewPatAllowChildren);
				string newPatDoAuthEmail=urlPanel.GetPrefValue(PrefName.WebSchedNewPatDoAuthEmail);
				string newPatDoAuthText=urlPanel.GetPrefValue(PrefName.WebSchedNewPatDoAuthText);
				string existingPatDoAuthEmail=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatDoAuthEmail);
				string existingPatDoAuthText=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatDoAuthText);
				string webFormsURL=urlPanel.GetPrefValue(PrefName.WebSchedNewPatWebFormsURL);
				string webFormsExistingPatURL=urlPanel.GetPrefValue(PrefName.WebSchedExistingPatWebFormsURL);
				if(clinicNum==0) {
					Prefs.UpdateString(PrefName.WebSchedNewPatAllowChildren,allowChildren);
					Prefs.UpdateString(PrefName.WebSchedNewPatDoAuthEmail,newPatDoAuthEmail);
					Prefs.UpdateString(PrefName.WebSchedNewPatDoAuthText,newPatDoAuthText);
					Prefs.UpdateString(PrefName.WebSchedExistingPatDoAuthEmail,existingPatDoAuthEmail);
					Prefs.UpdateString(PrefName.WebSchedExistingPatDoAuthText,existingPatDoAuthText);
					Prefs.UpdateString(PrefName.WebSchedNewPatWebFormsURL,webFormsURL);
					Prefs.UpdateString(PrefName.WebSchedExistingPatWebFormsURL,webFormsExistingPatURL);
					continue;
				}
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatAllowChildren,allowChildren));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatDoAuthEmail,newPatDoAuthEmail));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedNewPatDoAuthText,newPatDoAuthText));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedExistingPatDoAuthEmail,existingPatDoAuthEmail));
				listClinicPrefs.Add(GetClinicPrefToSave(clinicNum,PrefName.WebSchedExistingPatDoAuthText,existingPatDoAuthText));
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