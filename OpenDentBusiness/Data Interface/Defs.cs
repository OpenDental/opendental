using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness {
	public class Defs {
		#region Get Methods
		///<summary>Gets all definitions from the database.  The order mimics the cache.</summary>
		public static Def[][] GetArrayLongNoCache() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Def[][]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
			DataTable table=Db.GetTable(command);
			List<Def> list=Crud.DefCrud.TableToList(table);
			Def[][] arrayLong=new Def[Enum.GetValues(typeof(DefCat)).Length][];
			for(int j=0;j<Enum.GetValues(typeof(DefCat)).Length;j++) {
				arrayLong[j]=GetForCategory(j,true,list);
			}
			return arrayLong;
		}

		///<summary>Gets all non-hidden definitions from the database.  The order mimics the cache.</summary>
		public static Def[][] GetArrayShortNoCache() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Def[][]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
			DataTable table=Db.GetTable(command);
			List<Def> list=Crud.DefCrud.TableToList(table);
			Def[][] arrayShort=new Def[Enum.GetValues(typeof(DefCat)).Length][];
			for(int j=0;j<Enum.GetValues(typeof(DefCat)).Length;j++) {
				arrayShort[j]=GetForCategory(j,false,list);
			}
			return arrayShort;
		}

		///<summary>Used by the refresh method above.</summary>
		private static Def[] GetForCategory(int catIndex,bool includeHidden,List<Def> list) {
			//No need to check RemotingRole; no call to db.
			List<Def> retVal=new List<Def>();
			for(int i=0;i<list.Count;i++) {
				if((int)list[i].Category!=catIndex){
					continue;
				}
				if(list[i].IsHidden && !includeHidden){
					continue;
				}
				retVal.Add(list[i]);
			}
			return retVal.ToArray();
		}

		///<summary>Gets an array of definitions from the database.</summary>
		public static Def[] GetCatList(int myCat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Def[]>(MethodBase.GetCurrentMethod(),myCat);
			}
			string command=
				"SELECT * from definition"
				+" WHERE category = '"+myCat+"'"
				+" ORDER BY ItemOrder";
			return Crud.DefCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets a list of defs from the list of defnums and passed-in cat.</summary>
		public static List<Def> GetDefs(DefCat Cat,List<long> listDefNums) {
			//No need to check RemotingRole; no call to db.
			return GetDefsForCategory(Cat).FindAll(x => listDefNums.Contains(x.DefNum));
		}

		///<summary>Get one def from Long.  Returns null if not found.  Only used for very limited situations.
		///Other Get functions tend to be much more useful since they don't return null.
		///There is also BIG potential for silent bugs if you use this.ItemOrder instead of GetOrder().</summary>
		public static Def GetDef(DefCat myCat,long myDefNum,List<Def> listDefs=null) {
			//No need to check RemotingRole; no call to db.
			listDefs=listDefs??Defs.GetDefsForCategory(myCat);
			return listDefs.FirstOrDefault(x => x.DefNum==myDefNum);
		}

		///<summary>Returns the Def with the exact itemName passed in.  Returns null if not found.
		///If itemName is blank, then it returns the first def in the category.</summary>
		public static Def GetDefByExactName(DefCat myCat,string itemName) {
			//No need to check RemotingRole; no call to db.
			return GetDef(myCat,GetByExactName(myCat,itemName));
		}

		///<summary>Returns 0 if it can't find the named def.  If the name is blank, then it returns the first def in the category.</summary>
		public static long GetByExactName(DefCat myCat,string itemName) {
			//No need to check RemotingRole; no call to db.
			List<Def> listDefs=Defs.GetDefsForCategory(myCat);
			if(itemName=="" && listDefs.Count>0) {
				return listDefs[0].DefNum;//return the first one in the list
			}
			Def def=listDefs.FirstOrDefault(x => x.ItemName==itemName);
			if(def==null){
				return 0;
			}
			return def.DefNum;
		}

		///<summary>Returns the named def.  If it can't find the name, then it returns the first def in the category.</summary>
		public static long GetByExactNameNeverZero(DefCat myCat,string itemName) {
			//No need to check RemotingRole; no call to db.
			List<Def> listDefs=Defs.GetDefsForCategory(myCat);
			Def def;
			//We have been getting bug submissions from customers where listDefs will be null (e.g. DefCat.ProviderSpecialties cat itemName "General")
			//Therefore, we should check for null or and entirely empty category first before looking for a match.
			if(listDefs==null || listDefs.Count==0) {
				//There are no defs for the category passed in, create one because this method should never return zero.
				def=new Def();
				def.Category=myCat;
				def.ItemOrder=0;
				def.ItemName=itemName;
				Defs.Insert(def);
				Defs.RefreshCache();
				return def.DefNum;
			}
			//From this point on, we know our list of definitions contains at least one def.
			def=listDefs.FirstOrDefault(x => x.ItemName==itemName);
			if(def!=null) {
				return def.DefNum;
			}
			//Couldn't find a match so return the first definition from our list as a last resort.
			return listDefs[0].DefNum;
		}

		///<summary>Returns defs from the AdjTypes that contain '+' in the ItemValue column.</summary>
		public static List<Def> GetPositiveAdjTypes() {
			//No need to check RemotingRole; no call to db.
			return Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="+");
		}

		///<summary>Returns defs from the AdjTypes that contain '-' in the ItemValue column.</summary>
		public static List<Def> GetNegativeAdjTypes() {
			//No need to check RemotingRole; no call to db.
			return Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="-");
		}

		///<summary>Returns defs from the AdjTypes that contain 'dp' in the ItemValue column.</summary>
		public static List<Def> GetDiscountPlanAdjTypes() {
			//No need to check RemotingRole; no call to db.
			return Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="dp");
		}

		///<summary>Set includeHidden to true to include defs marked as 'Do Not Show on Account'.</summary>
		public static List<Def> GetUnearnedDefs(bool includeHidden=false,bool isShort=false) {
			//No need to check RemotingRole; no call to db.
			List<Def> listUnearnedDefs=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,isShort);
			if(!includeHidden) {
				listUnearnedDefs.RemoveAll(x => !string.IsNullOrEmpty(x.ItemValue));
			}
			return listUnearnedDefs;
		}

		///<summary>Only gets defs marked as 'Do Not Show on Account'.</summary>
		public static List<Def> GetHiddenUnearnedDefs(bool isShort=false) {
			//No need to check RemotingRole; no call to db.
			return Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,isShort).FindAll(x => !string.IsNullOrEmpty(x.ItemValue));
		}

		///<summary>Returns a DefNum for the special image category specified.  Returns 0 if no match found.</summary>
		public static long GetImageCat(ImageCategorySpecial specialCat) {
			//No need to check RemotingRole; no call to db.
			Def def=Defs.GetDefsForCategory(DefCat.ImageCats,true).FirstOrDefault(x => x.ItemValue.Contains(specialCat.ToString()));
			return (def==null ? 0 : def.DefNum);
		}

		///<summary>Gets the order of the def within Short or -1 if not found.</summary>
		public static int GetOrder(DefCat myCat,long myDefNum) {
			//No need to check RemotingRole; no call to db.
			//gets the index in the list of unhidden (the Short list).
			return Defs.GetDefsForCategory(myCat,true).FindIndex(x => x.DefNum==myDefNum);
		}

		///<summary>Returns empty string if no match found.</summary>
		public static string GetValue(DefCat myCat,long myDefNum) {
			//No need to check RemotingRole; no call to db.
			Def def=Defs.GetDefsForCategory(myCat).LastOrDefault(x => x.DefNum==myDefNum);
			return (def==null ? "" : def.ItemValue);
		}

		///<summary>Returns Color.White if no match found. Pass in a list of defs to save from making deep copies of the cache if you are going to 
		///call this method repeatedly.</summary>
		public static Color GetColor(DefCat myCat,long myDefNum,List<Def> listDefs=null) {
			//No need to check RemotingRole; no call to db.
			listDefs=listDefs??Defs.GetDefsForCategory(myCat);
			Def def=listDefs.LastOrDefault(x => x.DefNum==myDefNum);
			return (def==null ? Color.White : def.ItemColor);
		}

		///<summary></summary>
		public static bool GetHidden(DefCat myCat,long myDefNum) {
			//No need to check RemotingRole; no call to db.
			Def def=GetDef(myCat,myDefNum);
			return (def==null ? false : def.IsHidden);
		}

		///<summary>Pass in a list of all defs to save from making deep copies of the cache if you are going to call this method repeatedly.</summary>
		public static string GetName(DefCat myCat,long myDefNum,List<Def> listDefs=null) {
			//No need to check RemotingRole; no call to db.
			if(myDefNum==0) {
				return "";
			}
			Def def=GetDef(myCat,myDefNum,listDefs);
			return (def==null ? "" : def.ItemName);
		}

		///<summary>Gets the name of the def without requiring a category.  If it's a hidden def, it tacks (hidden) onto the end.  Use for single defs, not in a loop situation.</summary>
		public static string GetNameWithHidden(long defNum) {
			//No need to check RemotingRole; no call to db.
			if(defNum==0) {
				return "";
			}
			foreach(DefCat defCat in Enum.GetValues(typeof(DefCat))){
				Def def=GetDef(defCat,defNum);
				if(def==null){
					continue;
				}
				if(def.IsHidden){
					return def.ItemName+" "+Lans.g("Defs","(hidden)");
				}
				return def.ItemName;
			}
			return "";		
		}

		public static List<Def> GetDefsNoCache(DefCat defCat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Def>>(MethodBase.GetCurrentMethod(),defCat);
			}
			string command = "SELECT * FROM definition WHERE Category = "+POut.Int((int)defCat);
			return Crud.DefCrud.SelectMany(command);
		}

		///<summary>Returns definitions that are associated to the defCat, fKey, and defLinkType passed in.</summary>
		public static List<Def> GetDefsByDefLinkFKey(DefCat defCat,long fKey,DefLinkType defLinkType) {
			//No need to check RemotingRole; no call to db.
			List<DefLink> listDefLinks=DefLinks.GetDefLinksByType(defLinkType).FindAll(x => x.FKey==fKey);
			return Defs.GetDefs(defCat,listDefLinks.Select(x => x.DefNum).Distinct().ToList());
		}

		///<summary>Throws an exception if there are no definitions in the category provided.  This is to preserve old behavior.</summary>
		public static Def GetFirstForCategory(DefCat defCat,bool isShort=false) {
			//No need to check RemotingRole; no call to db.
			List<Def> listDefs=GetDefsForCategory(defCat,isShort);
			return listDefs.First();
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(Def def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				def.DefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.DefNum;
			}
			return Crud.DefCrud.Insert(def);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(Def def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.DefCrud.Update(def);
		}

		///<summary></summary>
		public static void HideDef(Def def) {
			//No need to check RemotingRole; no call to db.
			def.IsHidden=true;
			Defs.Update(def);
		}
		#endregion

		#region Delete
		///<summary>CAUTION.  This does not perform all validations.  Throws exceptions.</summary>
		public static void Delete(Def def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command;
			List<string> listCommands=new List<string>();
			switch(def.Category) {
				case DefCat.ClaimCustomTracking:
					listCommands.Add("SELECT COUNT(*) FROM securitylog WHERE DefNum="+POut.Long(def.DefNum));
					listCommands.Add("SELECT COUNT(*) FROM claim WHERE CustomTracking="+POut.Long(def.DefNum));
					break;
				case DefCat.ClaimErrorCode:
					listCommands.Add("SELECT COUNT(*) FROM claimtracking WHERE TrackingErrorDefNum="+POut.Long(def.DefNum));
					break;
				case DefCat.InsurancePaymentType:
					listCommands.Add("SELECT COUNT(*) FROM claimpayment WHERE PayType="+POut.Long(def.DefNum));
					break;
				case DefCat.SupplyCats:
					listCommands.Add("SELECT COUNT(*) FROM supply WHERE Category="+POut.Long(def.DefNum));
					break;
				case DefCat.AccountQuickCharge:
					break;//Users can delete AcctProcQuickCharge entries.  Nothing has an FKey to a AcctProcQuickCharge Def so no need to check anything.
				case DefCat.AutoNoteCats:
					AutoNotes.RemoveFromCategory(def.DefNum);//set any autonotes assinged to this category to 0 (unassigned), user already warned about this
					listCommands.Add("SELECT COUNT(*) FROM autonote WHERE Category="+POut.Long(def.DefNum));//just in case update failed or concurrency issue
					break;
				case DefCat.WebSchedNewPatApptTypes:
					DefCountValid(DefCat.WebSchedNewPatApptTypes);
					break;
				case DefCat.WebSchedExistingApptTypes:
					DefCountValid(DefCat.WebSchedExistingApptTypes);
					break;
				default:
					throw new ApplicationException("NOT Allowed to delete this type of def.");
			}
			for(int i=0;i<listCommands.Count;i++) {
				if(Db.GetCount(listCommands[i])!="0") {
					throw new ApplicationException(Lans.g("Defs","Def is in use.  Not allowed to delete."));
				}
			}
			command="DELETE FROM definition WHERE DefNum="+POut.Long(def.DefNum);
			Db.NonQ(command);
			command="UPDATE definition SET ItemOrder=ItemOrder-1 "
				+"WHERE Category="+POut.Long((int)def.Category)
				+" AND ItemOrder > "+POut.Long(def.ItemOrder);
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods

		/// <summary>Helper method that throws an Application Exception if the count of remaining defs of a given category is not greater than 1.</summary>
		private static void DefCountValid(DefCat cat) {
			//No remoting role check; private method.
			//Do not let the user delete the last WebSchedExistingApptType definition. Must be at least one.
			string command="SELECT COUNT(*) FROM definition WHERE Category="+POut.Int((int)cat);
			if(PIn.Int(Db.GetCount(command),false)<=1) {
				throw new ApplicationException("NOT Allowed to delete the last def of this type.");
			}
		}

		///<summary>Returns true if the passed-in def is deprecated.  This method must be updated whenever another def is deprecated.</summary>
		public static bool IsDefDeprecated(Def def) {
			if(def.Category==DefCat.AccountColors && def.ItemName=="Received Pre-Auth") {
				return true;
			}
			return false;
		}

		///<summary>Returns true if the category needs at least one of its definitions to be unhidden.
		///The list of categories in the if statement of this method should only include those that can be hidden.</summary>
		public static bool NeedOneUnhidden(DefCat category) {
			if(//Definitions of categories that are commented out can be hidden and we want to allow users to hide ALL of them if they so desire
				category==DefCat.AdjTypes
				//|| category==DefCat.AccountQuickCharge
				|| category==DefCat.ApptConfirmed
				|| category==DefCat.ApptProcsQuickAdd
				//|| category==DefCat.AutoDeposit
				|| category==DefCat.BillingTypes
				|| category==DefCat.BlockoutTypes
				//|| category==DefCat.CarrierGroupNames
				|| category==DefCat.ClaimCustomTracking
				|| category==DefCat.ClaimPaymentGroups
				|| category==DefCat.ClaimPaymentTracking
				//|| category==DefCat.ClinicSpecialty
				|| category==DefCat.CommLogTypes
				|| category==DefCat.ContactCategories
				|| category==DefCat.Diagnosis
				|| category==DefCat.ImageCats
				|| category==DefCat.InsuranceFilingCodeGroup
				//|| category==DefCat.InsuranceVerificationGroup
				|| category==DefCat.LetterMergeCats
				|| category==DefCat.PaymentTypes
				//|| category==DefCat.PayPlanCategories
				|| category==DefCat.PaySplitUnearnedType
				|| category==DefCat.ProcButtonCats
				|| category==DefCat.ProcCodeCats
				|| category==DefCat.Prognosis
				|| category==DefCat.ProviderSpecialties
				|| category==DefCat.RecallUnschedStatus
				|| category==DefCat.TaskPriorities
				//|| category==DefCat.TimeCardAdjTypes
				|| category==DefCat.TxPriorities)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if there are any entries in definition that do not have a Category named "General".  
		///Returning false means the user has ProcButtonCategory customizations.</summary>
		public static bool HasCustomCategories() {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProcButtonCats);
			foreach(Def defCur in listDefs) {
				if(!defCur.ItemName.Equals("General")) { 
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if this definition is in use within the program. Consider enhancing this method if you add a definition category.
		///Does not check patient billing type or provider specialty since those are handled in their S-class.</summary>
		public static bool IsDefinitionInUse(Def def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),def);
			}
			List<string> listStrCommands=new List<string>();
			switch(def.Category) {
				case DefCat.AdjTypes:
					if(new[] {
							PrefName.BrokenAppointmentAdjustmentType,
							PrefName.TreatPlanDiscountAdjustmentType,
							PrefName.BillingChargeAdjustmentType,
							PrefName.FinanceChargeAdjustmentType,
							PrefName.LateChargeAdjustmentType,
							PrefName.SalesTaxAdjustmentType,
							PrefName.RefundAdjustmentType,
						}.Any(x => PrefC.GetLong(x)==def.DefNum))
					{
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM adjustment WHERE AdjType="+POut.Long(def.DefNum));
					break;
				case DefCat.ApptConfirmed:
					if(new[] {
							PrefName.AppointmentTimeArrivedTrigger,
							PrefName.AppointmentTimeSeatedTrigger,
							PrefName.AppointmentTimeDismissedTrigger,
							PrefName.WebSchedNewPatConfirmStatus,
							PrefName.WebSchedRecallConfirmStatus,
						}.Any(x => PrefC.GetLong(x)==def.DefNum))
					{
						return true;
					}
					if(new[] { PrefName.ApptEConfirmStatusSent,PrefName.ApptEConfirmStatusAccepted,PrefName.ApptEConfirmStatusDeclined,PrefName.ApptEConfirmStatusSendFailed }
						.Any(x => PrefC.GetLong(x)==def.DefNum))
					{
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM appointment WHERE Confirmed="+POut.Long(def.DefNum));
					break;
				case DefCat.AutoNoteCats:
					listStrCommands.Add("SELECT COUNT(*) FROM autonote WHERE Category="+POut.Long(def.DefNum));
					break;
				case DefCat.BillingTypes:
					bool isClinicDefaultBillingType=ClinicPrefs.GetPrefAllClinics(PrefName.PracticeDefaultBillType).Any(x => x.ValueString==def.DefNum.ToString());
					if(isClinicDefaultBillingType) {
						return true;
					}
					break;
				case DefCat.ContactCategories:
					listStrCommands.Add("SELECT COUNT(*) FROM contact WHERE Category="+POut.Long(def.DefNum));
					break;
				case DefCat.Diagnosis:
					listStrCommands.Add("SELECT COUNT(*) FROM procedurelog WHERE Dx="+POut.Long(def.DefNum));
					break;
				case DefCat.ImageCats:
					listStrCommands.Add("SELECT COUNT(*) FROM document WHERE DocCategory="+POut.Long(def.DefNum));
					listStrCommands.Add("SELECT COUNT(*) FROM sheetfielddef WHERE FieldType="+POut.Int((int)SheetFieldType.PatImage)+" AND FieldName="+POut.Long(def.DefNum));
					break;
				case DefCat.PaymentTypes:
					if(ListTools.In(def.DefNum,PrefC.GetLong(PrefName.RecurringChargesPayTypeCC),PrefC.GetLong(PrefName.AccountingCashPaymentType))) {
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM payment WHERE PayType="+POut.Long(def.DefNum));
					break;
				case DefCat.PaySplitUnearnedType:
					if(ListTools.In(def.DefNum,PrefC.GetLong(PrefName.TpUnearnedType))) {
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM paysplit WHERE UnearnedType="+POut.Long(def.DefNum));
					break;
				case DefCat.Prognosis:
					listStrCommands.Add("SELECT COUNT(*) FROM procedurelog WHERE Prognosis="+POut.Long(def.DefNum));
					break;
				case DefCat.RecallUnschedStatus:
					if(ListTools.In(def.DefNum,
						PrefC.GetLong(PrefName.RecallStatusMailed),
						PrefC.GetLong(PrefName.RecallStatusTexted),
						PrefC.GetLong(PrefName.RecallStatusEmailed),
						PrefC.GetLong(PrefName.RecallStatusEmailedTexted))) 
					{
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM appointment WHERE UnschedStatus="+POut.Long(def.DefNum));
					listStrCommands.Add("SELECT COUNT(*) FROM recall WHERE RecallStatus="+POut.Long(def.DefNum));
					break;
				case DefCat.TaskPriorities:
					listStrCommands.Add("SELECT COUNT(*) FROM task WHERE PriorityDefNum="+POut.Long(def.DefNum));
					break;
				case DefCat.TxPriorities:
					listStrCommands.Add("SELECT COUNT(*) FROM procedurelog WHERE Priority="+POut.Long(def.DefNum));
					break;
				case DefCat.CommLogTypes:
					listStrCommands.Add("SELECT COUNT(*) FROM commlog WHERE CommType="+POut.Long(def.DefNum));
					break;
				default:
					break;
			}
			return listStrCommands.Any(x => Db.GetCount(x)!="0");
		}

		///<summary>Merges old document DocCategory(FK DefNum) into the new DocCategory(FK DefNum).</summary>
		public static void MergeImageCatDefNums(long defNumFrom, long defNumTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNumFrom,defNumTo);
				return;
			}
			string command="UPDATE document"
			+" SET DocCategory="+POut.Long(defNumTo)
			+" WHERE DocCategory="+POut.Long(defNumFrom);
			Db.NonQ(command);
			command="UPDATE mount"
			+" SET DocCategory="+POut.Long(defNumTo)
			+" WHERE DocCategory="+POut.Long(defNumFrom);
			Db.NonQ(command);
			command="UPDATE lettermerge"
			+" SET Category="+POut.Long(defNumTo)
			+" WHERE Category="+POut.Long(defNumFrom);
			Db.NonQ(command);
			command="UPDATE sheetfielddef"
			+" SET FieldName='"+POut.Long(defNumTo)+"'"
			+" WHERE FieldType="+POut.Int((int)SheetFieldType.PatImage)+" AND FieldName='"+POut.Long(defNumFrom)+"'";
			Db.NonQ(command);
			command="UPDATE sheetfield"
			+" SET FieldName='"+POut.Long(defNumTo)+"'"
			+" WHERE FieldType="+POut.Int((int)SheetFieldType.PatImage)+" AND FieldName='"+POut.Long(defNumFrom)+"'";
			Db.NonQ(command);
			long progNum=Programs.GetProgramNum(ProgramName.XVWeb);
			if(progNum!=0) {
				command="UPDATE programproperty"
				+" SET PropertyValue='"+POut.Long(defNumTo)+"'"
				+" WHERE ProgramNum="+POut.Long(progNum)+" AND PropertyDesc='ImageCategory' AND PropertyValue='"+POut.Long(defNumFrom)+"'";
				Db.NonQ(command);
			}
		}

		#endregion

		#region CachePattern

		///<summary>The def cache technically does not utilize the PK column as the Key in the dictionary but it can safely utilize CacheDictAbs.
		///The reason it can safely use this version of our dict cache paradigm because the key is guaranteed to be unique and not duplicate.</summary>
		private class DefCache:CacheDictAbs<Def,DefCat,List<Def>> {
			protected override List<Def> GetCacheFromDb() {
				string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
				return Crud.DefCrud.SelectMany(command);
			}
			protected override List<Def> TableToList(DataTable table) {
				return Crud.DefCrud.TableToList(table);
			}
			protected override Def Copy(Def provider) {
				return provider.Copy();
			}
			protected override DataTable DictToTable(Dictionary<DefCat,List<Def>> dictAllItems) {
				return Crud.DefCrud.ListToTable(dictAllItems.SelectMany(x => x.Value).ToList(),"Def");
			}
			protected override void FillCacheIfNeeded() {
				Defs.GetTableFromCache(false);
			}
			protected override DefCat GetDictKey(Def tableBase) {
				return tableBase.Category;
			}
			protected override List<Def> GetDictValue(Def tableBase) {
				throw new NotImplementedException();
			}
			protected override List<Def> GetShortValue(List<Def> value) {
				return value.FindAll(x => !x.IsHidden);
			}
			protected override List<Def> CopyDictValue(List<Def> value) {
				return value.Select(x => x.Copy()).ToList();
			}
			protected override Dictionary<DefCat,List<Def>> GetDictFromList(List<Def> listAllItems) {
				Dictionary<DefCat,List<Def>> dict=new Dictionary<DefCat, List<Def>>();
				//An entry is required even if the category is currently empty.  E.g. ClinicSpecialties aren't required nor prefilled with any defs.
				foreach(DefCat category in Enum.GetValues(typeof(DefCat))) {
					dict.Add(category,new List<Def>());
				}
				Dictionary<DefCat,List<Def>> dictAllItems=listAllItems.GroupBy(x => x.Category).ToDictionary(x => x.Key,x => x.ToList());
				foreach(KeyValuePair<DefCat,List<Def>> kvp in dictAllItems) {
					dict[kvp.Key]=kvp.Value;
				}
				return dict;
			}
			protected override List<DefCat> GetDictShortKeys(List<Def> listAllItems) {
				//The defs cache is unique in the sense that it doesn't matter what items came over from the database.
				//No matter what (short or long) all values in the enum DefCat must be a key.
				return Enum.GetValues(typeof(DefCat)).Cast<DefCat>().ToList();
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DefCache _defCache=new DefCache();

		public static Dictionary<DefCat,List<Def>> GetDeepCopy(bool isShort=false) {
			return _defCache.GetDeepCopy(isShort);
		}

		///<summary>Set isShort to true to exclude the Defs.IsHidden.</summary>
		public static List<Def> GetDefsForCategory(DefCat defCat,bool isShort=false) {
			return _defCache.GetOne(defCat,isShort);
		}

		public static bool GetDictIsNull() {
			return _defCache.DictIsNull();
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_defCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_defCache.FillCacheFromTable(table);
				return table;
			}
			return _defCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
	}

	///<summary></summary>
	public enum ImageCategorySpecial {
		///<summary>Show in Chart module.</summary>
		X,
		///<summary>Show in patient forms.</summary>
		F,
		/// <summary>Show in patient portal.</summary>
		L,
		///<summary>Patient picture (only one)</summary>
		P,
		///<summary>Statements (only one)</summary>
		S,
		///<summary>Graphical tooth charts and perio charts (only one)</summary>
		T,
		/// <summary>Treatment plan (only one)</summary>
		R,
		/// <summary>Expanded by default.</summary>
		E
	}


}
