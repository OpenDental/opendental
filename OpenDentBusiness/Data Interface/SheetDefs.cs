using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SheetDefs{
		#region Misc Methods

		///<summary>Returns true if this sheet def is allowed to bypass the global lock date.</summary>
		public static bool CanBypassLockDate(long sheetDefNum) {
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			if(sheetDef==null) {
				return false;
			}
			return (sheetDef.BypassGlobalLock==BypassLockStatus.BypassAlways);
		}

		///<summary>Returns true if any StaticText fields on the sheet def contain any of the StaticTextFields passed in. Otherwise false.</summary>
		public static bool ContainsStaticFields(SheetDef sheetDef,params StaticTextField[] arrayStaticTextFields) {
			if(sheetDef.SheetFieldDefs.IsNullOrEmpty() || arrayStaticTextFields.IsNullOrEmpty()) {
				return false;
			}
			List<string> listStaticTextFields=arrayStaticTextFields.Select(x => x.ToReplacementString()).ToList();
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++) {
				if(sheetDef.SheetFieldDefs[i].FieldType!=SheetFieldType.StaticText) {
					continue;
				}
				if(listStaticTextFields.Any(x => sheetDef.SheetFieldDefs[i].FieldValue.Contains(x))) {
					return true;
				}
			}
			return false;
		}

		///<summary>SheetType must either by PatientForm of MedicalHistory.</summary>
		public static bool IsWebFormAllowed(SheetTypeEnum sheetType) {
			return ListTools.In(sheetType,SheetTypeEnum.PatientForm,SheetTypeEnum.MedicalHistory);
		}

		///<summary>SheetType must either by PatientForm of MedicalHistory.</summary>
		public static bool IsMobileAllowed(SheetTypeEnum sheetType) {
			return (IsWebFormAllowed(sheetType) || sheetType==SheetTypeEnum.Consent);
		}

		///<summary>Determines if a sheetDef is of a SheetTypeEnum that describes a Dashboard.</summary>
		public static bool IsDashboardType(SheetDef sheetDef) {
			return IsDashboardType(sheetDef.SheetType);
		}

		///<summary>Determines if a SheetTypeEnum is a Dashboard.</summary>
		public static bool IsDashboardType(SheetTypeEnum sheetType) {
			if(ListTools.In(sheetType,SheetTypeEnum.PatientDashboard,SheetTypeEnum.PatientDashboardWidget)) {
				return true;
			}
			return false;
		}

		#endregion

		#region CachePattern

		private class SheetDefCache : CacheListAbs<SheetDef> {
			///<summary>Ordered by Description and then SheetDefNum to be a deterministic sorting.  This matches the sorting in GetInternalOrCustom().
			///So the order in the grid matches the order when choosing a sheetdef for use.</summary>
			protected override List<SheetDef> GetCacheFromDb() {
				string command="SELECT * FROM sheetdef ORDER BY Description,SheetDefNum";
				return Crud.SheetDefCrud.SelectMany(command);
			}
			protected override List<SheetDef> TableToList(DataTable table) {
				return Crud.SheetDefCrud.TableToList(table);
			}
			protected override SheetDef Copy(SheetDef sheetDef) {
				return sheetDef.Copy();
			}
			protected override DataTable ListToTable(List<SheetDef> listSheetDefs) {
				return Crud.SheetDefCrud.ListToTable(listSheetDefs,"SheetDef");
			}
			protected override void FillCacheIfNeeded() {
				SheetDefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SheetDefCache _sheetDefCache=new SheetDefCache();

		public static List<SheetDef> GetDeepCopy(bool isShort=false) {
			return _sheetDefCache.GetDeepCopy(isShort);
		}

		public static List<SheetDef> GetWhere(Predicate<SheetDef> match,bool isShort=false) {
			return _sheetDefCache.GetWhere(match,isShort);
		}

		public static SheetDef GetFirstOrDefault(Func<SheetDef,bool> match,bool isShort=false) {
			return _sheetDefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_sheetDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_sheetDefCache.FillCacheFromTable(table);
				return table;
			}
			return _sheetDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<Summary>Gets one SheetDef from the cache.  Also includes the fields and parameters for the sheetdef.</Summary>
		public static SheetDef GetSheetDef(long sheetDefNum,bool hasExceptions=true) {
			//No need to check RemotingRole; no call to db.
			SheetDef sheetdef=GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			if(hasExceptions || sheetdef!=null) {
				GetFieldsAndParameters(sheetdef);
			}
			return sheetdef;
		}

		///<summary>Updates the SheetDef only.  Does not included attached fields.</summary>
		public static long Update(SheetDef sheetDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				sheetDef.SheetDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheetDef);
				return sheetDef.SheetDefNum;
			}
			Crud.SheetDefCrud.Update(sheetDef);
			return sheetDef.SheetDefNum;
		}

		///<summary>Includes all attached fields.  Intelligently inserts, updates, or deletes old fields.</summary>
		///<param name="isOldSheetDuplicate">True if the sheetDef being created is a copy of a custom sheet that has a DateTCreated of 0001-01-01.
		///DateTCreated determines whether or not text fields will be shifted up 5 pixels when PDF is created from sheet to fix bug job B16020.</param>
		public static long InsertOrUpdate(SheetDef sheetDef,bool isOldSheetDuplicate=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				sheetDef.SheetDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheetDef,isOldSheetDuplicate);
				return sheetDef.SheetDefNum;
			}
			if(sheetDef.IsNew){
				if(!isOldSheetDuplicate) {
					sheetDef.DateTCreated=MiscData.GetNowDateTime();
				}
				sheetDef.SheetDefNum=Crud.SheetDefCrud.Insert(sheetDef);
			}
			else{
				Crud.SheetDefCrud.Update(sheetDef);
			}
			foreach(SheetFieldDef field in sheetDef.SheetFieldDefs){
				field.SheetDefNum=sheetDef.SheetDefNum;
			}
			SheetFieldDefs.Sync(sheetDef.SheetFieldDefs,sheetDef.SheetDefNum);
			return sheetDef.SheetDefNum;
		}

		///<summary></summary>
		public static void DeleteObject(long sheetDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetDefNum);
				return;
			}
			//validate that not already in use by a refferral.
			string command="SELECT LName,FName FROM referral WHERE Slip="+POut.Long(sheetDefNum);
			DataTable table=Db.GetTable(command);
			//int count=PIn.PInt(Db.GetCount(command));
			string referralNames="";
			for(int i=0;i<table.Rows.Count;i++){
				if(i>0){
					referralNames+=", ";
				}
				referralNames+=table.Rows[i]["FName"].ToString()+" "+table.Rows[i]["LName"].ToString();
			}
			if(table.Rows.Count>0){
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is already in use by referrals. Not allowed to delete.")+" "+referralNames);
			}
			//validate that not already in use by automation.
			command="SELECT AutomationNum FROM automation WHERE SheetDefNum="+POut.Long(sheetDefNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0){
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use by automation. Not allowed to delete."));
			}
			//validate that not already in use by a laboratory
			command="SELECT Description FROM laboratory WHERE Slip="+POut.Long(sheetDefNum);
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use by laboratories. Not allowed to delete.")
					+"\r\n"+string.Join(", ",table.Select().Select(x => x["Description"].ToString())));
			}
			//validate that not already in use as a default sheet
			List<PrefName> listSheetDefaultPrefs=new List<PrefName>() {
				PrefName.SheetsDefaultChartModule,
				PrefName.SheetsDefaultRx,
				PrefName.SheetsDefaultLimited,
				PrefName.SheetsDefaultStatement,
				PrefName.SheetsDefaultInvoice,
				PrefName.SheetsDefaultReceipt,
				PrefName.SheetsDefaultTreatmentPlan,
			};
			if(listSheetDefaultPrefs.Any(x => PrefC.GetLong(x)==sheetDefNum)) {
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use as a default sheet. Not allowed to delete."));
			}
			//validate that not already in use by clinicPref.
			List<PrefName> listClinicSheetDefaultPrefs=new List<PrefName>() {
				PrefName.SheetsDefaultRx,
				PrefName.SheetsDefaultChartModule,
				PrefName.SheetsDefaultTreatmentPlan,
			};
			command="SELECT ClinicNum "
				+"FROM clinicpref "
				+"WHERE ValueString='"+POut.Long(sheetDefNum)+"' "
				+"AND PrefName IN("+string.Join(",",listClinicSheetDefaultPrefs.Select(x => "'"+x.ToString()+"'"))+") ";
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use by clinics. Not allowed to delete.")
					+"\r\n"+string.Join(", ",table.Select().Select(x => Clinics.GetAbbr(PIn.Long(x["ClinicNum"].ToString())))));
			}
			//validate that not already in use by eClipboard
			command="SELECT EClipboardSheetDefNum,ClinicNum FROM eclipboardsheetdef WHERE SheetDefNum="+POut.Long(sheetDefNum);
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				if(PrefC.HasClinicsEnabled) {
					throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use by eClipboard. Not allowed to delete.")
					+"\r\n"+string.Join(", ",table.Select()
						.Select(x => Clinics.GetAbbr(PIn.Long(x["ClinicNum"].ToString())))
						.Select(x => string.IsNullOrEmpty(x) ? "Default" : x)));
				}
				else {
					throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use by eClipboard. Not allowed to delete."));
				}
				
			}
			command="DELETE FROM sheetfielddef WHERE SheetDefNum="+POut.Long(sheetDefNum);
			Db.NonQ(command);
			Crud.SheetDefCrud.Delete(sheetDefNum);
		}

		///<summary>Sheetdefs and sheetfielddefs are archived separately.
		///So when we need to use a sheetdef, we must run this method to pull all the associated fields from the archive.
		///Then it will be ready for printing, copying, etc.</summary>
		public static void GetFieldsAndParameters(SheetDef sheetdef){
			//No need to check RemotingRole; no call to db.
			//images first
			sheetdef.SheetFieldDefs=SheetFieldDefs.GetWhere(x => x.SheetDefNum==sheetdef.SheetDefNum && x.FieldType==SheetFieldType.Image);
			//then all other fields
			sheetdef.SheetFieldDefs.AddRange(SheetFieldDefs.GetWhere(x => x.SheetDefNum==sheetdef.SheetDefNum 
				&& x.FieldType!=SheetFieldType.Image
				&& x.FieldType!=SheetFieldType.Parameter));//Defs never store parameters. Fields store filled parameters, but that's different.
			sheetdef.Parameters=SheetParameter.GetForType(sheetdef.SheetType);
		}

		///<summary>Gets all custom sheetdefs(without fields or parameters) for a particular type.</summary>
		public static List<SheetDef> GetCustomForType(SheetTypeEnum sheettype){
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.SheetType==sheettype);
		}

		///<summary>Gets the description from the cache.</summary>
		public static string GetDescription(long sheetDefNum) {
			//No need to check RemotingRole; no call to db.
			SheetDef sheetDef=GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			return (sheetDef==null ? "" : sheetDef.Description);
		}

		public static SheetDef GetInternalOrCustom(SheetInternalType sheetInternalType) {
			SheetDef retVal=SheetsInternal.GetSheetDef(sheetInternalType);
			SheetDef custom=GetCustomForType(retVal.SheetType).OrderBy(x => x.Description).ThenBy(x => x.SheetDefNum).FirstOrDefault();
			if(custom!=null) {
				retVal=GetSheetDef(custom.SheetDefNum);
			}
			return retVal;
		}

		///<summary>Returns the SheetTypeEnum and Sheet Def defaults for a clinic, if clinics is not on/or user is altering HQ settings 
		///it will instead return user defaults, if neither is present then it will return the pratice default.</summary>
		public static Dictionary<SheetTypeEnum,SheetDef> GetDefaultSheetDefs(long clinicNum=0,params SheetTypeEnum[] arrSheetTypes) {
			//No need to check RemotingRole; no call to db.
			Dictionary<SheetTypeEnum,SheetDef> retVal=new Dictionary<SheetTypeEnum,SheetDef>();
			foreach(SheetTypeEnum sheetTypeEnum in arrSheetTypes) {
				SheetDef defaultSheetDef=GetSheetDef(PrefC.GetDefaultSheetDefNum(sheetTypeEnum),false);
				if(clinicNum==0) {
					if(defaultSheetDef==null) {
						defaultSheetDef=SheetsInternal.GetSheetDef(sheetTypeEnum);
					}
					retVal.Add(sheetTypeEnum,defaultSheetDef);
				}
				else {
					ClinicPref clinicPrefCur=ClinicPrefs.GetPref(Prefs.GetSheetDefPref(sheetTypeEnum),clinicNum);
					defaultSheetDef=SheetsInternal.GetSheetDef(sheetTypeEnum);
					if(clinicPrefCur!=null && PIn.Long(clinicPrefCur.ValueString)!=0) {//If ValueString is 0 then we want to keep it as the internal sheet def.
						defaultSheetDef=GetSheetDef(PIn.Long(clinicPrefCur.ValueString),false);
					}
					if(clinicPrefCur!=null) {//If there was a row in the clinicpref table, add whatever the sheetdef was to the retval dictionary.
						retVal.Add(sheetTypeEnum,defaultSheetDef);
					}
				}
			}
			return retVal;
		}

		///<summary>Passing in a clinicNum of 0 will use the base default sheet def.  Otherwise returns the clinic specific default sheetdef.</summary>
		public static SheetDef GetSheetsDefault(SheetTypeEnum sheetType,long clinicNum=0) {
			//No need to check RemotingRole; no call to db.
			ClinicPref clinicPrefCur=ClinicPrefs.GetPref(Prefs.GetSheetDefPref(sheetType),clinicNum);
			SheetDef defaultSheetDef;
			if(clinicPrefCur==null) {//If there wasn't a row for the specific clinic, use the base default sheetdef
				defaultSheetDef=GetSheetDef(PrefC.GetDefaultSheetDefNum(sheetType),false);
				if(defaultSheetDef==null) {
					defaultSheetDef=SheetsInternal.GetSheetDef(sheetType);
				}
				return defaultSheetDef;//Return the base default sheetdef
			}
			//Clinic specific sheet def found
			if(PIn.Long(clinicPrefCur.ValueString)==0) {//If ValueString is 0 then we want to keep it as the internal sheet def.
				defaultSheetDef=SheetsInternal.GetSheetDef(sheetType);
			}
			else {
				defaultSheetDef=GetSheetDef(PIn.Long(clinicPrefCur.ValueString),false);
			}
			return defaultSheetDef;
		}

		///<summary>Returns a dictionary such that the key is a specific ClinicNum from given listClinicNums and the value is a
		///dictionary such that its key is a SheetTypeEnum and the value is the corresponding SheetDef for the ClinicNum/SheetTypeEnum combo.
		///A ClinicNum will only exist in the returned Dictionaries keys if an associated SheetTypeEnum and SheetDef could be found.</summary>
		public static Dictionary<long,Dictionary<SheetTypeEnum,SheetDef>> GetSheetDefDefaults(List<long> listClinicNums,params SheetTypeEnum[] arrSheetTypes) {
			//No need to check RemotingRole; no call to db.
			Dictionary<long,Dictionary<SheetTypeEnum,SheetDef>> retVal=new Dictionary<long,Dictionary<SheetTypeEnum,SheetDef>>();
			foreach(long clinicNum in new List<long>() { 0 }.Concat(listClinicNums).Distinct()) {//If listClinicNums is empty (clinics are disabled) this will still run for 0 (headquarters).
				Dictionary<SheetTypeEnum,SheetDef> dictDefaultSheetDefs=GetDefaultSheetDefs(clinicNum,arrSheetTypes);
				if(dictDefaultSheetDefs.Count>0 && !retVal.ContainsKey(clinicNum)) {
					//Only add items that contain return values. If given listClinicNums contains a 0 then the following line would throw an exception.
					retVal.Add(clinicNum,dictDefaultSheetDefs);
				}
			}
			return retVal;
		}

		///<summary>Sets the FieldName for each SheetFieldDef in sheetDef.SheetFieldDefs to the Def.DefNum defined as the Patient Image definition.
		///Defaults to the first definition in the Image category if Patient Image is not defined.
		///This is necessary because the resource for the internal sheet likely does not contain a valid Def primary key.</summary>
		public static void SetPatImageFieldNames(SheetDef sheetDef) {
			//We need to figure out which Image Category should be used for any PatImage SheetFieldDefs.
			List<Def> listImageDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long defNum=0;
			//A user can define a specific image category as being the Patient Picture definition, see FormDefEditImages.butOK_Click().
			//SheetFieldDef.FieldName corresponds to Def.DefNum for a PatImage type SheetFieldDef.
			Def def=listImageDefs.FirstOrDefault(x => x.ItemValue.Contains("P"));
			if(def==null) {
				def=listImageDefs.FirstOrDefault();//Default to the first image category definition if one isn't defined as the Patient Image definition.
			}
			if(def==null) {//No Image Category definitions setup.
				defNum=0;
			}
			else {
				defNum=def.DefNum;
			}
			foreach(SheetFieldDef sheetFieldDef in sheetDef.SheetFieldDefs) {
				if(sheetFieldDef.FieldType!=SheetFieldType.PatImage) {
					continue;
				}
				sheetFieldDef.FieldName=POut.Long(defNum);
			}
		}
	}
}