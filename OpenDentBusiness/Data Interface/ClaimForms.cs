using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimForms {
		#region Cache Pattern

		private class ClaimFormCache : CacheListAbs<ClaimForm> {
			protected override List<ClaimForm> GetCacheFromDb() {
				string command="SELECT * FROM claimform";
				List<ClaimForm> listClaimForms=Crud.ClaimFormCrud.SelectMany(command);
				foreach(ClaimForm cf in listClaimForms) {
					cf.Items=ClaimFormItems.GetListForForm(cf.ClaimFormNum);
				}
				return listClaimForms;
			}
			protected override List<ClaimForm> TableToList(DataTable table) {
				List<ClaimForm> listClaimForms=Crud.ClaimFormCrud.TableToList(table);
				foreach(ClaimForm cf in listClaimForms) {
					cf.Items=ClaimFormItems.GetListForForm(cf.ClaimFormNum);
				}
				return listClaimForms;
			}
			protected override ClaimForm Copy(ClaimForm claimForm) {
				return claimForm.Copy();
			}
			protected override DataTable ListToTable(List<ClaimForm> listClaimForms) {
				return Crud.ClaimFormCrud.ListToTable(listClaimForms,"ClaimForm");
			}
			protected override void FillCacheIfNeeded() {
				ClaimForms.GetTableFromCache(false);
			}
			protected override bool IsInListShort(ClaimForm claimForm) {
				return !claimForm.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClaimFormCache _claimFormCache=new ClaimFormCache();

		public static List<ClaimForm> GetDeepCopy(bool isShort=false) {
			return _claimFormCache.GetDeepCopy(isShort);
		}

		public static ClaimForm GetFirstOrDefault(Func<ClaimForm,bool> match,bool isShort=false) {
			return _claimFormCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_claimFormCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_claimFormCache.FillCacheFromTable(table);
				return table;
			}
			return _claimFormCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary>Inserts this claimform into database and retrieves the new primary key.
		///Assigns all claimformitems to the claimform and inserts them if the bool is true.</summary>
		public static long Insert(ClaimForm cf,bool includeClaimFormItems) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				cf.ClaimFormNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cf,includeClaimFormItems);
				return cf.ClaimFormNum;
			}
			long retVal=Crud.ClaimFormCrud.Insert(cf);
			if(includeClaimFormItems) {
				foreach(ClaimFormItem claimFormItemCur in cf.Items) {
					claimFormItemCur.ClaimFormNum=cf.ClaimFormNum;//so even though the ClaimFormNum is wrong, this line fixes it.
					ClaimFormItems.Insert(claimFormItemCur);
				}
			}
			return retVal;
		}

		///<summary>Can be called externally as part of the conversion sequence.  Surround with try catch.
		///Returns the claimform object from the xml file or xml data passed in that can then be inserted if needed.
		///If xmlData is provided then path will be ignored.  If xmlData is not provided, a valid path is required.</summary>
		public static ClaimForm DeserializeClaimForm(string path,string xmlData) {
			//No need to check RemotingRole; no call to db.
			ClaimForm tempClaimForm = new ClaimForm();
			XmlSerializer serializer = new XmlSerializer(typeof(ClaimForm));
			if(xmlData=="") {//use path
				if(!File.Exists(path)) {
					throw new ApplicationException(Lans.g("FormClaimForm","File does not exist."));
				}
				try {
					using(TextReader reader = new StreamReader(path)) {
						tempClaimForm=(ClaimForm)serializer.Deserialize(reader);
					}
				}
				catch {
					throw new ApplicationException(Lans.g("FormClaimForm","Invalid file format"));
				}
			}
			else {//use xmlData
				try {
					using(TextReader reader = new StringReader(xmlData)) {
						tempClaimForm=(ClaimForm)serializer.Deserialize(reader);
					}
				}
				catch {
					throw new ApplicationException(Lans.g("FormClaimForm","Invalid file format"));
				}
			}
			return tempClaimForm;
		}

		///<summary></summary>
		public static void Update(ClaimForm cf){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cf);
				return;
			}
			//Synch the claim form items associated to this claim form first.
			ClaimFormItems.DeleteAllForClaimForm(cf.ClaimFormNum);
			foreach(ClaimFormItem item in cf.Items) {
				ClaimFormItems.Insert(item);
			}
			//Now we can update any information specific to the claim form itself.
			Crud.ClaimFormCrud.Update(cf);
		}

		///<summary> Called when cancelling out of creating a new claimform, and from the claimform window when clicking delete. Returns true if successful or false if dependencies found.</summary>
		public static bool Delete(ClaimForm cf){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),cf);
			}
			//first, do dependency testing
			string command="SELECT * FROM insplan WHERE claimformnum = '"
				+cf.ClaimFormNum.ToString()+"' ";
			command+=DbHelper.LimitAnd(1);
 			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==1){
				return false;
			}
			//Then, delete the claimform
			command="DELETE FROM claimform "
				+"WHERE ClaimFormNum = '"+POut.Long(cf.ClaimFormNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM claimformitem "
				+"WHERE ClaimFormNum = '"+POut.Long(cf.ClaimFormNum)+"'";
			Db.NonQ(command);
			return true;
		}
		
		///<summary>Returns the claim form specified by the given claimFormNum</summary>
		public static ClaimForm GetClaimForm(long claimFormNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClaimFormNum==claimFormNum);
		}

		///<summary>Returns a list of all internal claims within the OpenDentBusiness resources.  Throws exceptions.</summary>
		public static List<ClaimForm> GetInternalClaims() {
			//No need to check RemotingRole; no call to db.
			List<ClaimForm> listInternalClaimForms = new List<ClaimForm>();
			ResourceSet resources=OpenDentBusiness.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture,true,true);
			foreach(DictionaryEntry item in resources) {
				if(!item.Key.ToString().StartsWith("ClaimForm")) {
					continue;
				}
				//Resources that start with ClaimForm are serialized ClaimForm objects in XML.
				ClaimForm cfCur = ClaimForms.DeserializeClaimForm("",item.Value.ToString());
				cfCur.IsInternal=true;
				listInternalClaimForms.Add(cfCur);
			}
			return listInternalClaimForms;
		}

		///<summary>Returns number of insplans affected.</summary>
		public static long Reassign(long oldClaimFormNum,long newClaimFormNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),oldClaimFormNum,newClaimFormNum);
			}
			string command="UPDATE insplan SET ClaimFormNum="+POut.Long(newClaimFormNum)
				+" WHERE ClaimFormNum="+POut.Long(oldClaimFormNum);
			return Db.NonQ(command);
		}
	}

	



}









