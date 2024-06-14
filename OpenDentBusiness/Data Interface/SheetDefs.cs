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
			//No need to check MiddleTierRole; no call to db.
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			if(sheetDef==null) {
				return false;
			}
			return (sheetDef.BypassGlobalLock==BypassLockStatus.BypassAlways);
		}

		///<summary>Returns true if any StaticText fields on the sheet def contain any of the StaticTextFields passed in. Otherwise false.</summary>
		public static bool ContainsStaticFields(SheetDef sheetDef,params EnumStaticTextField[] staticTextFieldArray) {
			//No need to check MiddleTierRole; no call to db.
			if(sheetDef.SheetFieldDefs.IsNullOrEmpty() || staticTextFieldArray.IsNullOrEmpty()) {
				return false;
			}
			List<string> listStaticTextFields=staticTextFieldArray.Select(x => x.ToReplacementString()).ToList();
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

		///<summary>Returns true if any Grids on the sheet def contain any of the specific Grids passed in. Otherwise false.</summary>
		public static bool ContainsGrids(SheetDef sheetDef,params string[] gridNameArray) {
			//No need to check MiddleTierRole; no call to db.
			if(sheetDef.SheetFieldDefs.IsNullOrEmpty() || gridNameArray.IsNullOrEmpty()) {
				return false;
			}
			List<string> listGrids=gridNameArray.ToList();
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++) {
				if(sheetDef.SheetFieldDefs[i].FieldType!=SheetFieldType.Grid) {
					continue;
				}
				if(listGrids.Any(x => sheetDef.SheetFieldDefs[i].FieldName.Contains(x))) {
					return true;
				}
			}
			return false;
		}

		///<summary>SheetType must either by PatientForm of MedicalHistory.</summary>
		public static bool IsWebFormAllowed(SheetTypeEnum sheetType) {
			//No need to check MiddleTierRole; no call to db.
			if(sheetType.In(SheetTypeEnum.PatientForm,SheetTypeEnum.MedicalHistory)){
				return true;
			}
			return false;
		}

		///<summary>SheetType must either by PatientForm of MedicalHistory.</summary>
		public static bool IsMobileAllowed(SheetTypeEnum sheetType) {
			//No need to check MiddleTierRole; no call to db.
			if (IsWebFormAllowed(sheetType)) {
				return true;
			}
			if (sheetType==SheetTypeEnum.Consent) {
				return true;
			}
			if (sheetType==SheetTypeEnum.ExamSheet) {
				return true;
			}
			return false;
		}

		///<summary>Determines if a sheetDef is of a SheetTypeEnum that describes a Dashboard.</summary>
		public static bool IsDashboardType(SheetDef sheetDef) {
			//No need to check MiddleTierRole; no call to db.
			return IsDashboardType(sheetDef.SheetType);
		}

		///<summary>Determines if a SheetTypeEnum is a Dashboard.</summary>
		public static bool IsDashboardType(SheetTypeEnum sheetType) {
			//No need to check MiddleTierRole; no call to db.
			if(sheetType.In(SheetTypeEnum.PatientDashboard,SheetTypeEnum.PatientDashboardWidget)) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_sheetDefCache.FillCacheFromTable(table);
				return table;
			}
			return _sheetDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_sheetDefCache.ClearCache();
		}
		#endregion

		///<Summary>Gets one SheetDef from the cache.  Also includes the fields and parameters for the sheetdef.</Summary>
		public static SheetDef GetSheetDef(long sheetDefNum,bool hasExceptions=true) {
			//No need to check MiddleTierRole; no call to db.
			SheetDef sheetDef=GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			if(hasExceptions || sheetDef!=null) {
				GetFieldsAndParameters(sheetDef);
			}
			return sheetDef;
		}

		///<summary>Updates the SheetDef only.  Does not included attached fields.</summary>
		public static long Update(SheetDef sheetDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++) {
				sheetDef.SheetFieldDefs[i].SheetDefNum=sheetDef.SheetDefNum;
			}
			SheetFieldDefs.Sync(sheetDef.SheetFieldDefs,sheetDef.SheetDefNum);
			return sheetDef.SheetDefNum;
		}

		///<summary></summary>
		public static void DeleteObject(long sheetDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			List<PrefName> listPrefNamesDefault=new List<PrefName>();
			listPrefNamesDefault.Add(PrefName.SheetsDefaultChartModule);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultRx);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultLimited);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultStatement);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultInvoice);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultReceipt);
			listPrefNamesDefault.Add(PrefName.SheetsDefaultTreatmentPlan);
			if(listPrefNamesDefault.Any(x => PrefC.GetLong(x)==sheetDefNum)) {
				throw new ApplicationException(Lans.g("sheetDefs","SheetDef is in use as a default sheet. Not allowed to delete."));
			}
			//validate that not already in use by clinicPref.
			List<PrefName> listPrefNamesClinicDefault=new List<PrefName>();
			listPrefNamesClinicDefault.Add(PrefName.SheetsDefaultRx);
			listPrefNamesClinicDefault.Add(PrefName.SheetsDefaultChartModule);
			listPrefNamesClinicDefault.Add(PrefName.SheetsDefaultTreatmentPlan);
			command="SELECT ClinicNum "
				+"FROM clinicpref "
				+"WHERE ValueString='"+POut.Long(sheetDefNum)+"' "
				+"AND PrefName IN("+string.Join(",",listPrefNamesClinicDefault.Select(x => "'"+x.ToString()+"'"))+") ";
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
			command="DELETE FROM grouppermission" 
				+" WHERE FKey="+POut.Long(sheetDefNum)
				+" AND PermType="+POut.Enum<EnumPermType>(EnumPermType.DashboardWidget);
			Db.NonQ(command);
			command="DELETE FROM sheetfielddef WHERE SheetDefNum="+POut.Long(sheetDefNum);
			Db.NonQ(command);
			Crud.SheetDefCrud.Delete(sheetDefNum);
		}

		///<summary>Sheetdefs and sheetfielddefs are archived separately.
		///So when we need to use a sheetdef, we must run this method to pull all the associated fields from the archive.
		///Then it will be ready for printing, copying, etc.</summary>
		public static void GetFieldsAndParameters(SheetDef sheetdef){
			//No need to check MiddleTierRole; no call to db.
			//images first
			sheetdef.SheetFieldDefs=SheetFieldDefs.GetWhere(x => x.SheetDefNum==sheetdef.SheetDefNum && x.FieldType==SheetFieldType.Image);
			//then all other fields
			sheetdef.SheetFieldDefs.AddRange(SheetFieldDefs.GetWhere(x => x.SheetDefNum==sheetdef.SheetDefNum 
				&& x.FieldType!=SheetFieldType.Image
				&& x.FieldType!=SheetFieldType.Parameter));//Defs never store parameters. Fields store filled parameters, but that's different.
			sheetdef.Parameters=SheetParameter.GetForType(sheetdef.SheetType);
		}

		///<summary>Gets all custom sheetdefs(without fields or parameters) for a particular type.</summary>
		public static List<SheetDef> GetCustomForType(SheetTypeEnum sheetType){
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.SheetType==sheetType);
		}

		///<summary>Gets the description from the cache.</summary>
		public static string GetDescription(long sheetDefNum) {
			//No need to check MiddleTierRole; no call to db.
			SheetDef sheetDef=GetFirstOrDefault(x => x.SheetDefNum==sheetDefNum);
			if (sheetDef==null) {
				return "";
			}
			return sheetDef.Description;
		}

		public static SheetDef GetInternalOrCustom(SheetInternalType sheetInternalType) {
			//No need to check MiddleTierRole; no call to db.
			SheetDef sheetDefRetVal=SheetsInternal.GetSheetDef(sheetInternalType);
			SheetDef sheetDefCustom=GetCustomForType(sheetDefRetVal.SheetType).OrderBy(x => x.Description).ThenBy(x => x.SheetDefNum).FirstOrDefault();
			if(sheetDefCustom!=null) {
				sheetDefRetVal=GetSheetDef(sheetDefCustom.SheetDefNum);
			}
			return sheetDefRetVal;
		}

		///<summary>Passing in a clinicNum of 0 will use the base default sheet def.  Otherwise returns the clinic specific default sheetdef.</summary>
		public static SheetDef GetSheetsDefault(SheetTypeEnum sheetType,long clinicNum=0) {
			//No need to check MiddleTierRole; no call to db.
			ClinicPref clinicPref=ClinicPrefs.GetPref(Prefs.GetSheetDefPref(sheetType),clinicNum);
			SheetDef sheetDefDefault;
			if(clinicPref==null) {//If there wasn't a row for the specific clinic, use the base default sheetdef
				sheetDefDefault=GetSheetDef(PrefC.GetDefaultSheetDefNum(sheetType),false);
				if(sheetDefDefault==null) {
					sheetDefDefault=SheetsInternal.GetSheetDef(sheetType);
				}
				return sheetDefDefault;//Return the base default sheetdef
			}
			//Clinic specific sheet def found
			if(PIn.Long(clinicPref.ValueString)==0) {//If ValueString is 0 then we want to keep it as the internal sheet def.
				sheetDefDefault=SheetsInternal.GetSheetDef(sheetType);
			}
			else {
				sheetDefDefault=GetSheetDef(PIn.Long(clinicPref.ValueString),false);
			}
			return sheetDefDefault;
		}

		///<summary>Gets a list of sheetdefs from the DB. Used by the API. If modifying this method, please contact someone from the API team.</summary>
		public static List<SheetDef> GetSheetDefsForApi(int intLimit,int intOffset){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SheetDef>>(MethodBase.GetCurrentMethod(),intLimit,intOffset);
			}
			string command="SELECT * FROM sheetdef ";
			command+="ORDER BY SheetDefNum "
				+"LIMIT "+POut.Int(intOffset)+", "+POut.Int(intLimit);
			return Crud.SheetDefCrud.SelectMany(command);
		}

		///<summary>Sets the FieldName for each SheetFieldDef in sheetDef.SheetFieldDefs to the Def.DefNum defined as the Patient Image definition.
		///Defaults to the first definition in the Image category if Patient Image is not defined.
		///This is necessary because the resource for the internal sheet likely does not contain a valid Def primary key.</summary>
		public static void SetPatImageFieldNames(SheetDef sheetDef) {
			//No need to check MiddleTierRole; no call to db.
			//We need to figure out which Image Category should be used for any PatImage SheetFieldDefs.
			List<Def> listDefsImage=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long defNum=0;
			//A user can define a specific image category as being the Patient Picture definition, see FormDefEditImages.butOK_Click().
			//SheetFieldDef.FieldName corresponds to Def.DefNum for a PatImage type SheetFieldDef.
			Def def=listDefsImage.FirstOrDefault(x => x.ItemValue.Contains("P"));
			if(def==null) {
				def=listDefsImage.FirstOrDefault();//Default to the first image category definition if one isn't defined as the Patient Image definition.
			}
			if(def==null) {//No Image Category definitions setup.
				defNum=0;
			}
			else {
				defNum=def.DefNum;
			}
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++) {
				if(sheetDef.SheetFieldDefs[i].FieldType!=SheetFieldType.PatImage) {
					continue;
				}
				sheetDef.SheetFieldDefs[i].FieldName=POut.Long(defNum);
			}
		}

		public static List<SheetFieldType> GetVisibleButtons(SheetTypeEnum sheetType){
			//No need to check MiddleTierRole; no call to db.
			List<SheetFieldType> listSheetFieldTypes=new List<SheetFieldType>();
			if(sheetType==SheetTypeEnum.ChartModule) {
				listSheetFieldTypes.Add(SheetFieldType.Grid);
				listSheetFieldTypes.Add(SheetFieldType.Special);
				return listSheetFieldTypes;
			}
			if(sheetType==SheetTypeEnum.PatientDashboardWidget) {
				listSheetFieldTypes.Add(SheetFieldType.StaticText);
				listSheetFieldTypes.Add(SheetFieldType.PatImage);
				listSheetFieldTypes.Add(SheetFieldType.Grid);
				listSheetFieldTypes.Add(SheetFieldType.Special);
				listSheetFieldTypes.Add(SheetFieldType.Line);
				listSheetFieldTypes.Add(SheetFieldType.Rectangle);
				return listSheetFieldTypes;
			}
			listSheetFieldTypes.Add(SheetFieldType.OutputText);
			listSheetFieldTypes.Add(SheetFieldType.InputField);
			listSheetFieldTypes.Add(SheetFieldType.StaticText);
			listSheetFieldTypes.Add(SheetFieldType.Image);
			listSheetFieldTypes.Add(SheetFieldType.Line);
			listSheetFieldTypes.Add(SheetFieldType.Rectangle);
			if(!sheetType.In(SheetTypeEnum.ERA,SheetTypeEnum.ERAGridHeader)) {
				listSheetFieldTypes.Add(SheetFieldType.CheckBox);
			}
			if(!sheetType.In(SheetTypeEnum.DepositSlip,SheetTypeEnum.ERA,SheetTypeEnum.ERAGridHeader)) {
				listSheetFieldTypes.Add(SheetFieldType.PatImage);
			}
			if(!sheetType.In(SheetTypeEnum.DepositSlip,
				SheetTypeEnum.ERA,
				SheetTypeEnum.ERAGridHeader,
				SheetTypeEnum.RoutingSlip,
				SheetTypeEnum.LabelCarrier,
				SheetTypeEnum.LabelPatient,
				SheetTypeEnum.LabelReferral,
				SheetTypeEnum.LabelAppointment,
				SheetTypeEnum.Statement,
				SheetTypeEnum.TreatmentPlan)) 
			{
				listSheetFieldTypes.Add(SheetFieldType.ComboBox);
			}
			if(!sheetType.In(SheetTypeEnum.DepositSlip,
				SheetTypeEnum.ERA,
				SheetTypeEnum.ERAGridHeader,
				SheetTypeEnum.RoutingSlip,
				SheetTypeEnum.LabelCarrier,
				SheetTypeEnum.LabelPatient,
				SheetTypeEnum.LabelReferral,
				SheetTypeEnum.LabelAppointment,
				SheetTypeEnum.Statement)) 
			{
				listSheetFieldTypes.Add(SheetFieldType.SigBox);
			}
			if(sheetType==SheetTypeEnum.TreatmentPlan) {
				listSheetFieldTypes.Add(SheetFieldType.SigBoxPractice);
			}
			if(sheetType.In(SheetTypeEnum.TreatmentPlan,SheetTypeEnum.ReferralLetter)) {
				listSheetFieldTypes.Add(SheetFieldType.Special);
			}
			if(sheetType.In(SheetTypeEnum.Statement,SheetTypeEnum.MedLabResults,SheetTypeEnum.TreatmentPlan,SheetTypeEnum.PaymentPlan,
				SheetTypeEnum.ReferralLetter,SheetTypeEnum.ERA,SheetTypeEnum.Consent,SheetTypeEnum.PatientForm,SheetTypeEnum.PatientLetter))
			{
				listSheetFieldTypes.Add(SheetFieldType.Grid);
			}
			if(sheetType==SheetTypeEnum.Screening) {
				listSheetFieldTypes.Add(SheetFieldType.ScreenChart);
			}
			if(SheetDefs.IsMobileAllowed(sheetType)){
				listSheetFieldTypes.Add(SheetFieldType.MobileHeader);
			}
			return listSheetFieldTypes;
		}
	}
}