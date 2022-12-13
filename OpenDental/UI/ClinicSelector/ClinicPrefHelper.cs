using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace OpenDental{
	public class ClinicPrefHelper{
		///<summary>This list includes all clinics as well the "default" pref with clinicnum=0.  It includes only those prefnames that are used on the form where this helper is present.  All items are mixed, and we filter.</summary>
		private List<ClinicPref> _listClinicPrefs=new List<ClinicPref>();
		///<summary>List of prefs that this helper will manage.</summary>
		private List<PrefName> _listPrefNames=new List<PrefName>();

		public ClinicPrefHelper(params PrefName[] prefNames) {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,doIncludeHQ:true);
			listClinics.Add(Clinics.GetPracticeAsClinicZero());
			foreach(PrefName prefName in prefNames.Distinct()) {
				//Explicity load the entire list because some forms using the clinicPrefHelper might have a "use defaults" pref
				//that this ClinicPrefHelper doesn't and shouldn't know about. We will clean up unneccessary clinic prefs when 
				//we call SyncPrefs.
				foreach(Clinic clinic in listClinics.Distinct()) {
					_listClinicPrefs.Add(new ClinicPref(){
						PrefName=prefName,
						ClinicNum=clinic.ClinicNum,
						ValueString=ClinicPrefs.GetPrefValue(prefName,clinic.ClinicNum)
					});
				}
				_listPrefNames.Add(prefName);
			}
		}

		///<summary>For all types, so pass in string val.  This changes the value in the clinicpref that we have in memory.  It will be synched later.</summary>
		public void ValChangedByUser(PrefName prefName,long clinicNum,string newVal){
			if(clinicNum<0) { //Shouldn't happen...
				return;
			}
			//Update the current value for the pref that we are storing in the list
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName);
			if(clinicPref==null) { //Doesn't exist so create one
				clinicPref=new ClinicPref() {
					PrefName=prefName,
					ClinicNum=clinicNum
				};
				_listClinicPrefs.Add(clinicPref);
			}
			clinicPref.ValueString=newVal;
		}

		///<summary>If there is no val for this clinic, then it uses the default pref, which is also in the available list.</summary>
		public bool GetBoolVal(PrefName prefName,long clinicNum){
			if(clinicNum<0) { //Shouldn't happen
				return false;
			}
			if(_listClinicPrefs.Any(x => x.ClinicNum==clinicNum && x.PrefName==prefName)) { //we've already loaded this item, just load it's checked value
				return PIn.Bool(_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName).ValueString);
			}
			return PIn.Bool(_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==0 && x.PrefName==prefName).ValueString);
		}

		///<summary></summary>
		public string GetStringVal(PrefName prefName,long clinicNum){
			if(clinicNum<0) { //Shouldn't happen
				return "";
			}
			if(_listClinicPrefs.Any(x => x.ClinicNum==clinicNum && x.PrefName==prefName)) { //we've already loaded this item, just load it's checked value
				return _listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName).ValueString;
			}
			return _listClinicPrefs.FirstOrDefault(x => x.ClinicNum==0 && x.PrefName==prefName).ValueString;
		}

		///<summary>For ClinicNum==0</summary>
		public bool GetDefaultBoolVal(PrefName prefName){
			return PIn.Bool(_listClinicPrefs.First(x => x.ClinicNum==0 && x.PrefName==prefName).ValueString);
		}

		///<summary>For ClinicNum==0</summary>
		public string GetDefaultStringVal(PrefName prefName){
			return _listClinicPrefs.First(x => x.ClinicNum==0 && x.PrefName==prefName).ValueString;
		}

		public List<ClinicPref> GetWhere(PrefName prefName,string valueString) {
			return _listClinicPrefs.FindAll(x => x.PrefName==prefName && x.ValueString==valueString);
		}

		///<summary>Save all pref changes relating to prefs that were added in Init().</summary>>
		public bool SyncAllPrefs() {
			bool ret=false;
			foreach(PrefName prefName in _listPrefNames) {
				if(SyncPref(prefName)) {
					ret=true;
				}
			}
			return ret;
		}

		///<summary>Save all pref changes relating to the given pref. PrefName must have been included in Init().
		///It is suggested that you use SyncAllPrefs(), it is safer.</summary>>
		public bool SyncPref(PrefName prefName) {					
			//We ensured that our list had default (ClinicNum 0) prefs when we included defaults in Init(). Should always be available.
			string hqValue=_listClinicPrefs.First(x => x.ClinicNum==0 && x.PrefName==prefName).ValueString;
			//Save the default (HQ) pref first.
			bool didSave=prefName.Update(hqValue);
			//Our list will likely have clinic-specific entries which are identical to HQ defaults. 
			//In this case, remove those duplicates so we don't save them to the db.
			_listClinicPrefs.RemoveAll(x => x.ClinicNum!=0 && x.PrefName==prefName && x.ValueString.Equals(hqValue));
			List<ClinicPref> listNonDefaultClinicPrefs=_listClinicPrefs.FindAll(x => x.ClinicNum>0 && x.PrefName==prefName);
			if(ClinicPrefs.Sync(listNonDefaultClinicPrefs,ClinicPrefs.GetPrefAllClinics(prefName))){
				didSave=true;
			}
			if(didSave) {
				Signalods.SetInvalid(InvalidType.ClinicPrefs);
				ClinicPrefs.RefreshCache();
			}
			return didSave;
		}

		///<summary></summary>
		public List<long> GetClinicsWithChanges() {
			List<long> ret=new List<long>();
			foreach(PrefName prefName in _listPrefNames) {
				ret.AddRange(GetClinicsWithChanges(prefName));
			}
			return ret.Distinct().ToList();			
		}
				
		///<summary>Essentially a "sync" method although it doesn't save to the db. Takes the list of new clinic preference values
		///and compares it to the database. Returns a list of clinics that have had their preference values changed.</summary>
		public List<long> GetClinicsWithChanges(PrefName prefName) {
			List<ClinicPref> listClinicPrefsThisPrefName=_listClinicPrefs.FindAll(x => x.PrefName==prefName);						
			List<long> listRet=new List<long>();
			List<ClinicPref> listDb=ClinicPrefs.GetPrefAllClinics(prefName,includeDefault:true);
			listRet.AddRange(listClinicPrefsThisPrefName.Where(x => 
				//Get the items from the new list that aren't in the old list 
				!listDb.Select(y => y.ClinicNum).Contains(x.ClinicNum)
				//AND that aren't using the default preference value
				&& x.ValueString!=PrefC.GetString(prefName))
				//Add the clinic nums
				.Select(x => x.ClinicNum));
			//Add any items that have been deleted or updated.
			foreach(ClinicPref oldCp in listDb) {
				ClinicPref newCp=listClinicPrefsThisPrefName.FirstOrDefault(x => x.ClinicNum==oldCp.ClinicNum);
				if(newCp==null) { //Item was in db and now is not.
					listRet.Add(oldCp.ClinicNum);
					continue;
				}
				if(newCp.ValueString!=oldCp.ValueString) { //Item has changed.
					listRet.Add(oldCp.ClinicNum);
				}
			}
			return listRet.Distinct().ToList();
		}

		///<summary>SyncAll should be called before using this to clear out any non-existant (in the db) prefs from _listClinicPrefs.</summary>
		public bool ClinicHasClinicPref(PrefName prefName,long clinicNum) {
			if(clinicNum<0) { //Shouldn't happen
				return false;
			}
			if(_listClinicPrefs.Any(x => x.ClinicNum==clinicNum && x.PrefName==prefName)) { //we've already loaded this item, just load it's checked value
				return true;
			}
			return false;
		}

		public void DeleteClinicPref(PrefName prefName,long clinicNum) {
			if(clinicNum<=0) { //We don't delete the default pref.
				return;
			}
			if(_listClinicPrefs.Any(x => x.ClinicNum==clinicNum && x.PrefName==prefName)) { //we've already loaded this item, just load it's checked value
				ClinicPref prefToRemove=_listClinicPrefs.First(x => x.ClinicNum==clinicNum && x.PrefName==prefName);
				ClinicPrefs.Delete(prefToRemove.ClinicPrefNum);
				_listClinicPrefs.Remove(prefToRemove);
			}
		}
	}
}
