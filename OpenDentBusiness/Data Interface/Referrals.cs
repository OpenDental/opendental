using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness{
///<summary></summary>
	public class Referrals{
		#region CachePattern

		private class ReferralCache : CacheListAbs<Referral> {
			protected override List<Referral> GetCacheFromDb() {
				string command="SELECT * FROM referral ORDER BY LName";
				return Crud.ReferralCrud.SelectMany(command);
			}
			protected override List<Referral> TableToList(DataTable table) {
				return Crud.ReferralCrud.TableToList(table);
			}
			protected override Referral Copy(Referral referral) {
				return referral.Copy();
			}
			protected override DataTable ListToTable(List<Referral> listReferrals) {
				return Crud.ReferralCrud.ListToTable(listReferrals,"Referral");
			}
			protected override void FillCacheIfNeeded() {
				Referrals.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Referral referral) {
				return !referral.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ReferralCache _referralCache=new ReferralCache();

		public static bool GetExists(Predicate<Referral> match,bool isShort=false) {
			return _referralCache.GetExists(match,isShort);
		}

		public static List<Referral> GetDeepCopy(bool isShort=false) {
			return _referralCache.GetDeepCopy(isShort);
		}

		public static List<Referral> GetWhere(Predicate<Referral> match,bool isShort=false) {
			return _referralCache.GetWhere(match,isShort);
		}

		public static Referral GetFirstOrDefault(Func<Referral,bool> match,bool isShort=false) {
			return _referralCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_referralCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_referralCache.FillCacheFromTable(table);
				return table;
			}
			return _referralCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(Referral refer) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),refer);
				return;
			}
			Crud.ReferralCrud.Update(refer);
		}

		///<summary></summary>
		public static long Insert(Referral refer) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				refer.ReferralNum=Meth.GetLong(MethodBase.GetCurrentMethod(),refer);
				return refer.ReferralNum;
			}
			return Crud.ReferralCrud.Insert(refer);
		}

		///<summary></summary>
		public static void Delete(Referral refer) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),refer);
				return;
			}
			if(RefAttaches.IsReferralAttached(refer.ReferralNum)) {
				throw new ApplicationException(Lans.g("FormReferralEdit","Cannot delete Referral because it is attached to patients"));
			}
			if(Claims.IsReferralAttached(refer.ReferralNum)) {
				throw new ApplicationException(Lans.g("FormReferralEdit","Cannot delete Referral because it is attached to claims"));
			}
			if(Procedures.IsReferralAttached(refer.ReferralNum)) {
				throw new ApplicationException(Lans.g("FormReferralEdit","Cannot delete Referral because it is attached to procedures"));
			}
			string command="DELETE FROM referral "
				+"WHERE ReferralNum = '"+POut.Long(refer.ReferralNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Get all matching rows where input email is found in the Email column.</summary>
		public static List<Referral> GetEmailMatch(string email) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Referral>>(MethodBase.GetCurrentMethod(),email);
			}
			string command= "SELECT * FROM referral "
				+"WHERE IsDoctor=1 AND UPPER(EMail) LIKE '%"+email.ToUpper()+"%'";
			return Crud.ReferralCrud.SelectMany(command);
		}

		public static Referral GetFromList(long referralNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ReferralNum==referralNum);
		}

		///<summary>Includes title like DMD on the end.</summary>
		public static string GetNameLF(long referralNum) {
			//No need to check RemotingRole; no call to db.
			if(referralNum==0) {
				return "";
			}
			Referral referral=GetFromList(referralNum);
			if(referral==null) {
				return "";
			}
			string retVal=referral.LName;
			if(referral.FName!="") {
				retVal+=", "+referral.FName;
			}
			if(referral.MName!="") {
				retVal+=" "+referral.MName;
			}
			if(referral.Title !="") {
				retVal+=", "+referral.Title;
			}
			//specialty seems to wordy to add here
			return retVal;
		}

		///<summary>Includes title, such as DMD.</summary>
		public static string GetNameFL(long referralNum) {
			//No need to check RemotingRole; no call to db.
			if(referralNum==0) {
				return "";
			}
			Referral referral=GetFromList(referralNum);
			if(referral==null) {
				return "";
			}
			return referral.GetNameFL();
		}

		///<summary></summary>
		public static string GetPhone(long referralNum) {
			//No need to check RemotingRole; no call to db.
			Referral referral=GetFirstOrDefault(x => x.ReferralNum==referralNum);
			if(referral!=null) {
				if(referral.Telephone.Length==10) {
					return referral.Telephone.Substring(0,3)+"-"+referral.Telephone.Substring(3,3)+"-"+referral.Telephone.Substring(6);
				}
				return referral.Telephone;
			}
			return "";
		}

		public static List<Referral> GetAllReferrals() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Referral>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM Referral";
			return Crud.ReferralCrud.SelectMany(command);
		}

		///<summary>Returns a list of Referrals with names similar to the supplied string.  Used in dropdown list from referral field in FormPatientAddAll for faster entry.</summary>
		public static List<Referral> GetSimilarNames(string referralLName) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.LName.ToUpper().IndexOf(referralLName.ToUpper())==0);
		}

		///<summary>Used by API. Returns a single Referral with LName exactly matching the supplied string, or null if no match found. </summary>
		public static Referral GetReferralByLName(string LName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Referral>(MethodBase.GetCurrentMethod(),LName);
			}
			string command="SELECT * FROM Referral WHERE LName LIKE '"+POut.String(LName)+"'";//matches regardless of case 
			return Crud.ReferralCrud.SelectOne(command);
		}

		///<summary>Gets Referral info from memory.  Does not make a call to the database unless needed.
		///Returns the true if the referral for the passed in referralNum could be found and sets the out parameter accordingly.
		///Otherwise returns false and referral will be null.</summary>
		public static bool TryGetReferral(long referralNum,out Referral referral) {
			//No need to check RemotingRole; uses out parameter.
			referral=null;
			try {
				referral=GetReferral(referralNum);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return (referral!=null);
		}

		///<summary>Gets Referral info from memory.  Does not make a call to the database unless needed.
		///Returns the first referral matching the referralNum passed in, null if 0 is passed in, or throws an exception if no match found.</summary>
		private static Referral GetReferral(long referralNum) {
			//No need to check RemotingRole; no call to db.
			if(referralNum==0) {
				return null;
			}
			Referral referral=GetFirstOrDefault(x => x.ReferralNum==referralNum);
			if(referral==null) {
				throw new ApplicationException("Error.  Referral not found: "+referralNum.ToString());
			}
			return referral;
		}

		///<summary>Gets the first referral "from" for the given patient.  Will return null if no "from" found for patient.</summary>
		public static Referral GetReferralForPat(long patNum,List<RefAttach> listRefAttaches=null) {
			//No need to check RemotingRole; no call to db.
			listRefAttaches=listRefAttaches??RefAttaches.Refresh(patNum);
			for(int i=0;i<listRefAttaches.Count;i++) {
				if(listRefAttaches[i].RefType==ReferralType.RefFrom) {
					Referral referral;
					if(TryGetReferral(listRefAttaches[i].ReferralNum,out referral)) {
						return referral;
					}
				}
			}
			return null;
		}

		///<summary>Gets IsDoctors referred "from" referrals for the given patient.  Will return empty list if no "from" and IsDoctor found for patient.</summary>
		public static List<Referral> GetIsDoctorReferralsForPat(long patNum,List<RefAttach> listRefAttaches=null) {
			//No need to check RemotingRole; no call to db.
			List<Referral> retVal=new List<Referral>();
			listRefAttaches=listRefAttaches??RefAttaches.Refresh(patNum);
			for(int i=0;i<listRefAttaches.Count;i++) {
				if(listRefAttaches[i].RefType==ReferralType.RefFrom) {
					Referral referral;
					if(TryGetReferral(listRefAttaches[i].ReferralNum,out referral) && referral.IsDoctor) {
						retVal.Add(referral);
					}
				}
			}
			return retVal;
		}

		///<summary>Replaces all patient's referral "From" and "IsDoctor" fields in the given message.  Returns the resulting string.
		///Replaces: [ReferredFromProvInitialReferralNum], [ReferredFromProvInitialNameF],etc.</summary>
		public static string ReplaceRefProvider(string message,Patient pat) {
			if(pat==null) {
				return message;
			}
			List<Referral> listRefFrom=Referrals.GetIsDoctorReferralsForPat(pat.PatNum);
			if(listRefFrom.Count==0) {
				return message;
			}
			string retVal=message;
			//The oldest referral 'From".
			Referral refOldest=listRefFrom.FirstOrDefault();
			retVal=retVal.Replace("[ReferredFromProvInitialReferralNum]",refOldest.ReferralNum.ToString());
			retVal=retVal.Replace("[ReferredFromProvInitialNameF]",refOldest.FName);
			retVal=retVal.Replace("[ReferredFromProvInitialNameL]",refOldest.LName);
			retVal=retVal.Replace("[ReferredFromProvInitialPhone]",refOldest.Telephone);
			retVal=retVal.Replace("[ReferredFromProvInitialAddress]",refOldest.Address);
			retVal=retVal.Replace("[ReferredFromProvInitialAddress2]",refOldest.Address2);
			retVal=retVal.Replace("[ReferredFromProvInitialCity]",refOldest.City);
			retVal=retVal.Replace("[ReferredFromProvInitialState]",refOldest.ST);
			retVal=retVal.Replace("[ReferredFromProvInitialZip]",refOldest.Zip);
			//The most recent referral "From".
			Referral refNewest=listRefFrom.LastOrDefault();
			retVal=retVal.Replace("[ReferredFromProvMostRecentReferralNum]",refNewest.ReferralNum.ToString());
			retVal=retVal.Replace("[ReferredFromProvMostRecentNameF]",refNewest.FName);
			retVal=retVal.Replace("[ReferredFromProvMostRecentNameL]",refNewest.LName);
			retVal=retVal.Replace("[ReferredFromProvMostRecentPhone]",refNewest.Telephone);
			retVal=retVal.Replace("[ReferredFromProvMostRecentAddress]",refNewest.Address);
			retVal=retVal.Replace("[ReferredFromProvMostRecentAddress2]",refNewest.Address2);
			retVal=retVal.Replace("[ReferredFromProvMostRecentCity]",refNewest.City);
			retVal=retVal.Replace("[ReferredFromProvMostRecentState]",refNewest.ST);
			retVal=retVal.Replace("[ReferredFromProvMostRecentZip]",refNewest.Zip);
			return retVal;
		}

		///<summary>Gets all referrals by RefNum.  Returns an empty list if no matches.</summary>
		public static List<Referral> GetReferrals(List<long> listRefNums) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => listRefNums.Contains(x.ReferralNum));
		}

		///<summary>Merges two referrals into a single referral. Returns false if both referrals are the same.</summary>
		public static bool MergeReferrals(long refNumInto,long refNumFrom) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),refNumInto,refNumFrom);
			}
			if(refNumInto==refNumFrom) {
				//Do not merge the same referral onto itself.
				return false;
			}
			string command="UPDATE claim "
				+"SET ReferringProv="+POut.Long(refNumInto)+" "
				+"WHERE ReferringProv="+POut.Long(refNumFrom);
			Db.NonQ(command);
			command="UPDATE refattach "
				+"SET ReferralNum="+POut.Long(refNumInto)+" "
				+"WHERE ReferralNum="+POut.Long(refNumFrom);
			Db.NonQ(command);
			command="DELETE FROM referralcliniclink "
				+"WHERE ReferralNum="+POut.Long(refNumFrom);
			Db.NonQ(command);
			Crud.ReferralCrud.Delete(refNumFrom);
			return true;
		}

		///<summary>Returns the number of refattaches that this referral has.</summary>
		public static int CountReferralAttach(long referralNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),referralNum);
			}
			string command="SELECT COUNT(*) FROM refattach "
				+"WHERE ReferralNum="+POut.Long(referralNum);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Used to check if a specialty is in use when user is trying to hide it.</summary>
		public static bool IsSpecialtyInUse(long defNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),defNum);
			}
			string command="SELECT COUNT(*) FROM referral WHERE Specialty="+POut.Long(defNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}
	}
}