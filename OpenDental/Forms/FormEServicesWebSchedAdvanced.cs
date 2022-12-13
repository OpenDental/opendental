using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.User_Controls;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEServicesWebSchedAdvanced:FormODBase {
		#region Fields
		///<summary>List of a List that holds information on what preferences have been changed</summary>
		private List<ClinicPref> _listClinicPrefs=new List<ClinicPref>();
		/// <summary>Used for retrieving eService data.</summary>
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		/// <summary>Used to fill the Clinics grid.</summary>
		private List<EServiceClinicName> _listEserviceClinicNames;
		/// <summary>Holds all the edited UserControlHostedURLs. Our way of 'saving' before we save to DB.</summary>
		private List<UserControlHostedURL> _listUserControlHostedURLs;
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
			_listUserControlHostedURLs=new List<UserControlHostedURL>();
			_listClinicPrefs=new List<ClinicPref>();
			_listEserviceClinicNames=GetEserviceClinicNames();
			FillGridClinics();
			if(!PrefC.HasClinicsEnabled || _listEserviceClinicNames.Count()==1) {
				LoadAndDisplayClinic(_listEserviceClinicNames.FirstOrDefault());
			}
		
		}

		#region Methods - Private
		private void AddClinicPrefToList(PrefName prefName,long clinicNum) {
			if(_listClinicPrefs.Any(x=>x.ClinicNum==clinicNum && x.PrefName==prefName)) {
				return;//No duplicates
			}
			ClinicPref clinicPref=ClinicPrefs.GetPref(prefName,clinicNum);
			if(clinicPref!=null) {
				_listClinicPrefs.Add(clinicPref);
			}
		}

		private class EServiceClinicName {
			public WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService SignupOutEService;
			public string ClinicName;
		}

		/// <summary>Loads a UserControlHostedURL control and displays it. If there is one previously 'saved' that will be used. Inserts 7 clinicprefs into _listClinicPrefs. </summary>
		private void LoadAndDisplayClinic(EServiceClinicName eserviceClinicName) {
			if(eserviceClinicName==null) {
				return;
			}
			UserControlHostedURL userControlHostedURL = (UserControlHostedURL)panelHostedURLs.Controls.Find("UserControlHostedURL",searchAllChildren: true).FirstOrDefault();
			if(userControlHostedURL==null) {//No controls displaying, so just display the current one.
				LayoutUserControlHostedURL(eserviceClinicName);
			}
			else {//Save the control if we haven't already.
				UserControlHostedURL userControlHostedURLSaved = _listUserControlHostedURLs.FirstOrDefault(x => x.GetClinicNum()==eserviceClinicName.SignupOutEService.ClinicNum);
				if(userControlHostedURLSaved==null) {//We haven't saved this control before.
					_listUserControlHostedURLs.Add(userControlHostedURL);
					LayoutUserControlHostedURL(eserviceClinicName);
				}
				else {//Load the saved control
					panelHostedURLs.Controls.Clear();
					//Save the old control if needed.
					if(_listUserControlHostedURLs.FirstOrDefault(x => x.GetClinicNum()==userControlHostedURLSaved.GetClinicNum()) == null) {
						_listUserControlHostedURLs.Add(userControlHostedURL);
					}
					//panelHostedURLs.SuspendLayout();
					panelHostedURLs.Controls.Add(userControlHostedURLSaved);
					//panelHostedURLs.ResumeLayout();
					//The saved control was already scaled, no need to scale again.
				}
			}
			if(eserviceClinicName.SignupOutEService.ClinicNum==0) {
				return;//There are no ClinicPrefs for headquarters.
			}
			else {
				AddClinicPrefToList(PrefName.WebSchedNewPatAllowChildren,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthEmail,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedNewPatDoAuthText,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedNewPatWebFormsURL,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedExistingPatWebFormsURL,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthEmail,eserviceClinicName.SignupOutEService.ClinicNum);
				AddClinicPrefToList(PrefName.WebSchedExistingPatDoAuthText,eserviceClinicName.SignupOutEService.ClinicNum);
			}
		}

		private void FillGridClinics() {
			gridClinics.BeginUpdate();
			gridClinics.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Clinic",200);
			gridClinics.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Enabled",50,HorizontalAlignment.Center);
			gridClinics.Columns.Add(gridColumn);
			GridRow gridRow;
			GridCell gridCell;
			string strShowEnabled;
			for(int i=0;i<_listEserviceClinicNames.Count;i++) {
				gridRow=new GridRow();
				gridCell=new GridCell(_listEserviceClinicNames[i].ClinicName);
				gridRow.Cells.Add(gridCell);
				strShowEnabled=_listEserviceClinicNames[i].SignupOutEService.IsEnabled ? "X" : "";
				gridCell=new GridCell(strShowEnabled);
				gridRow.Cells.Add(gridCell);
				gridClinics.ListGridRows.Add(gridRow);
			}
			gridClinics.EndUpdate();
		}

		/// <summary>Returns a list of EserviceClinicNames for all the clinics in the DB. If the EserviceClinicName cannot be found, N/A is used. The list is ordered by ClinicName.</summary>
		private List<EServiceClinicName> GetEserviceClinicNames() {
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
					if(listSignupOutEServices[i].ClinicNum==0) {
						eServiceClinicName.ClinicName="Headquarters";
					}
					else {
						eServiceClinicName.ClinicName="N\\A";
					}
				}
				else {
					eServiceClinicName.ClinicName=clinic.Abbr;
				}
				eServiceClinicName.SignupOutEService=listSignupOutEServices[i];
				listEServiceClinicNames.Add(eServiceClinicName);
			}
			return listEServiceClinicNames.OrderBy(x => x.ClinicName).ToList();
		}

		private ClinicPref GetClinicPrefToSave(long clinicNum,PrefName prefName,string value) {
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName)?.Clone();
			if(clinicPref==null) {
				return new ClinicPref(clinicNum,prefName,value);
			}
			clinicPref.ValueString=value;
			return clinicPref;
		}

		/// <summary>Clears and fills panelHostedURLs with a new userCOntrolHostedURL control</summary>
		private void LayoutUserControlHostedURL(EServiceClinicName eserviceClinicName,bool doSave=false) {
			panelHostedURLs.Controls.Clear();
			UserControlHostedURL userControlHostedURL=new UserControlHostedURL(eserviceClinicName.SignupOutEService);
			userControlHostedURL.LayoutManager=LayoutManager;
			if(doSave) {
				_listUserControlHostedURLs.Add(userControlHostedURL);
			}
			Lan.C(this,userControlHostedURL);
			LayoutManager.AddUnscaled(userControlHostedURL,panelHostedURLs);
			LayoutManager.MoveWidth(userControlHostedURL,LayoutManager.Scale(userControlHostedURL.Width));
		}

		#endregion Methods - Private

		private void SaveWebSchedAdvanced() {
			List<ClinicPref> listClinicPrefs=new List<ClinicPref>();
			for(int i=0;i<_listUserControlHostedURLs.Count;i++) {
				long clinicNum=_listUserControlHostedURLs[i].GetClinicNum();
				string strAllowChildren=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedNewPatAllowChildren);
				string strNewPatDoAuthEmail=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedNewPatDoAuthEmail);
				string strNewPatDoAuthText=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedNewPatDoAuthText);
				string strExistingPatDoAuthEmail=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedExistingPatDoAuthEmail);
				string strExistingPatDoAuthText=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedExistingPatDoAuthText);
				string webFormsURL=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedNewPatWebFormsURL);
				string webFormsExistingPatURL=_listUserControlHostedURLs[i].GetPrefValue(PrefName.WebSchedExistingPatWebFormsURL);
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
			//In case only one change was made before saving
			UserControlHostedURL userControlHostedURL = (UserControlHostedURL)panelHostedURLs.Controls.Find("UserControlHostedURL",true).FirstOrDefault();
			if(userControlHostedURL!=null && !_listUserControlHostedURLs.Any(x => x.GetClinicNum()==userControlHostedURL.GetClinicNum())) {//No duplicates.
				_listUserControlHostedURLs.Add(userControlHostedURL);
			}
			SaveWebSchedAdvanced();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void gridClinics_Click(object sender,ODGridClickEventArgs e) {
			LoadAndDisplayClinic(_listEserviceClinicNames[e.Row]);
		}
	}
}