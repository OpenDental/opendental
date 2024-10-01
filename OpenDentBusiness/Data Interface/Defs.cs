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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Def[][]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
			DataTable table=Db.GetTable(command);
			List<Def> listDefs=Crud.DefCrud.TableToList(table);
			Def[][] defArray=new Def[Enum.GetValues(typeof(DefCat)).Length][];
			for(int j=0;j<Enum.GetValues(typeof(DefCat)).Length;j++) {
				defArray[j]=GetForCategory(j,true,listDefs);
			}
			return defArray;
		}

		///<summary>Gets all non-hidden definitions from the database.  The order mimics the cache.</summary>
		public static Def[][] GetArrayShortNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Def[][]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
			DataTable table=Db.GetTable(command);
			List<Def> listDefs=Crud.DefCrud.TableToList(table);
			Def[][] defArray=new Def[Enum.GetValues(typeof(DefCat)).Length][];
			for(int j=0;j<Enum.GetValues(typeof(DefCat)).Length;j++) {
				defArray[j]=GetForCategory(j,false,listDefs);
			}
			return defArray;
		}

		///<summary>Used by the refresh method above.</summary>
		private static Def[] GetForCategory(int idxCat,bool includeHidden,List<Def> listDefs) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefsRet=new List<Def>();
			for(int i=0;i<listDefs.Count;i++) {
				if((int)listDefs[i].Category!=idxCat){
					continue;
				}
				if(listDefs[i].IsHidden && !includeHidden){
					continue;
				}
				listDefsRet.Add(listDefs[i]);
			}
			return listDefsRet.ToArray();
		}

		///<summary>Gets an array of definitions from the database.</summary>
		public static Def[] GetCatList(int category){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Def[]>(MethodBase.GetCurrentMethod(),category);
			}
			string command=
				"SELECT * from definition"
				+" WHERE category = '"+category+"'"
				+" ORDER BY ItemOrder";
			return Crud.DefCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets a list of defs from the list of defnums and passed-in cat.</summary>
		public static List<Def> GetDefs(DefCat defCat,List<long> listDefNums) {
			Meth.NoCheckMiddleTierRole();
			return GetDefsForCategory(defCat).FindAll(x => listDefNums.Contains(x.DefNum));
		}

		///<summary>Get one def from Long.  Returns null if not found.  Only used for very limited situations.
		///Other Get functions tend to be much more useful since they don't return null.
		///There is also BIG potential for silent bugs if you use this.ItemOrder instead of GetOrder().</summary>
		public static Def GetDef(DefCat defCat,long defNum,List<Def> listDefs=null) {
			Meth.NoCheckMiddleTierRole();
			listDefs=listDefs??Defs.GetDefsForCategory(defCat);
			return listDefs.FirstOrDefault(x => x.DefNum==defNum);
		}

		///<summary>Returns the Def with the exact itemName passed in.  Returns null if not found.
		///If itemName is blank, then it returns the first def in the category.</summary>
		public static Def GetDefByExactName(DefCat defCat,string itemName) {
			Meth.NoCheckMiddleTierRole();
			return GetDef(defCat,GetByExactName(defCat,itemName));
		}

		///<summary>Set isShort to true to exclude hidden defs. Returns 0 if it can't find the named def.  If the name is blank, then it returns the first def in the category.</summary>
		public static long GetByExactName(DefCat defCat,string itemName,bool isShort=false){
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(defCat,isShort);
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
		public static long GetByExactNameNeverZero(DefCat defCat,string itemName) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(defCat);
			Def def;
			//We have been getting bug submissions from customers where listDefs will be null (e.g. DefCat.ProviderSpecialties cat itemName "General")
			//Therefore, we should check for null or and entirely empty category first before looking for a match.
			if(listDefs==null || listDefs.Count==0) {
				//There are no defs for the category passed in, create one because this method should never return zero.
				def=new Def();
				def.Category=defCat;
				def.ItemOrder=0;
				def.ItemName=itemName;
				Defs.Insert(def);
				Defs.RefreshCache();
				string logText=Lans.g("Defintions","Definition created:")+" "+def.ItemName+" "
					+Lans.g("Defintions","with category:")+" "+def.Category.GetDescription();
				SecurityLogs.MakeLogEntry(EnumPermType.DefEdit,0,logText);
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

		///<summary>Returns defs from the AdjTypes that contain '+' in the ItemValue column.
		///Optionally set considerPermission true to exclude adjustment types that the user currently logged in does not have access to.</summary>
		public static List<Def> GetPositiveAdjTypes(bool considerPermission=false) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="+");
			if(considerPermission) { 
				return listDefs.FindAll(x => GroupPermissions.HasPermissionForAdjType(x));
			}
			return listDefs;
		}

		///<summary>Returns defs from the AdjTypes that contain '-' in the ItemValue column.
		///Optionally set considerPermission true to exclude adjustment types that the user currently logged in does not have access to.</summary>
		public static List<Def> GetNegativeAdjTypes(bool considerPermission=false) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="-");
			if(considerPermission) { 
				return listDefs.FindAll(x => GroupPermissions.HasPermissionForAdjType(x));
			}
			return listDefs;
		}

		///<summary>Returns defs from the AdjTypes that contain 'dp' in the ItemValue column.</summary>
		public static List<Def> GetDiscountPlanAdjTypes() {
			Meth.NoCheckMiddleTierRole();
			return Defs.GetDefsForCategory(DefCat.AdjTypes,true).FindAll(x => x.ItemValue=="dp");
		}

		///<summary>Set includeHidden to true to include defs marked as 'Do Not Show on Account'.</summary>
		public static List<Def> GetUnearnedDefs(bool includeHidden=false,bool isShort=false) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,isShort);
			if(!includeHidden) {
				listDefs.RemoveAll(x => !string.IsNullOrEmpty(x.ItemValue));
			}
			return listDefs;
		}

		///<summary>Only gets defs marked as 'Do Not Show on Account'.</summary>
		public static List<Def> GetHiddenUnearnedDefs(bool isShort=false) {
			Meth.NoCheckMiddleTierRole();
			return Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,isShort).FindAll(x => !string.IsNullOrEmpty(x.ItemValue));
		}

		///<summary>Returns a DefNum for the special image category specified.  Returns 0 if no match found.</summary>
		public static long GetImageCat(ImageCategorySpecial imageCategorySpecial) {
			Meth.NoCheckMiddleTierRole();
			Def def=Defs.GetDefsForCategory(DefCat.ImageCats,true).FirstOrDefault(x => x.ItemValue.Contains(imageCategorySpecial.ToString()));
			if(def==null) {
				return 0;
			}
			return def.DefNum;
		}

		///<summary>Gets the order of the def within Short or -1 if not found.</summary>
		public static int GetOrder(DefCat defCat,long defNum) {
			Meth.NoCheckMiddleTierRole();
			//gets the index in the list of unhidden (the Short list).
			return Defs.GetDefsForCategory(defCat,true).FindIndex(x => x.DefNum==defNum);
		}

		///<summary>Returns empty string if no match found.</summary>
		public static string GetValue(DefCat defCat,long defNum) {
			Meth.NoCheckMiddleTierRole();
			Def def=Defs.GetDefsForCategory(defCat).LastOrDefault(x => x.DefNum==defNum);
			if(def==null) {
				return "";
			}
			return def.ItemValue;
		}

		///<summary>Returns Color.White if no match found. Pass in a list of defs to save from making deep copies of the cache if you are going to 
		///call this method repeatedly.</summary>
		public static Color GetColor(DefCat defCat,long defNum,List<Def> listDefs=null) {
			Meth.NoCheckMiddleTierRole();
			listDefs=listDefs??Defs.GetDefsForCategory(defCat);
			Def def=listDefs.LastOrDefault(x => x.DefNum==defNum);
			if(def==null) {
				return Color.White;
			}
			return def.ItemColor;
		}

		///<summary></summary>
		public static bool GetHidden(DefCat defCat,long defNum) {
			Meth.NoCheckMiddleTierRole();
			Def def=GetDef(defCat,defNum);
			if(def==null) {
				return false;
			}
			return def.IsHidden;
		}

		///<summary>Pass in a list of all defs to save from making deep copies of the cache if you are going to call this method repeatedly.</summary>
		public static string GetName(DefCat defCat,long defNum,List<Def> listDefs=null) {
			Meth.NoCheckMiddleTierRole();
			if(defNum==0) {
				return "";
			}
			Def def=GetDef(defCat,defNum,listDefs);
			if(def==null) {
				return "";
			}
			return def.ItemName;
		}

		///<summary>Gets the name of the def without requiring a category.  If it's a hidden def, it tacks (hidden) onto the end.  Use for single defs, not in a loop situation.</summary>
		public static string GetNameWithHidden(long defNum) {
			Meth.NoCheckMiddleTierRole();
			if(defNum==0) {
				return "";
			}
			string[] defCatNameArray=Enum.GetNames(typeof(DefCat));
			for(int i=0;i<defCatNameArray.Length;i++) {
				Def def=GetDef((DefCat)i,defNum);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Def>>(MethodBase.GetCurrentMethod(),defCat);
			}
			string command="SELECT * FROM definition WHERE Category = "+POut.Int((int)defCat);
			return Crud.DefCrud.SelectMany(command);
		}

		///<summary>Returns definitions that are associated to the defCat, fKey, and defLinkType passed in.</summary>
		public static List<Def> GetDefsByDefLinkFKey(DefCat defCat,long fKey,DefLinkType defLinkType) {
			Meth.NoCheckMiddleTierRole();
			List<DefLink> listDefLinks=DefLinks.GetDefLinksByType(defLinkType).FindAll(x => x.FKey==fKey);
			return Defs.GetDefs(defCat,listDefLinks.Select(x => x.DefNum).Distinct().ToList());
		}

		///<summary>Throws an exception if there are no definitions in the category provided.  This is to preserve old behavior.</summary>
		public static Def GetFirstForCategory(DefCat defCat,bool isShort=false) {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=GetDefsForCategory(defCat,isShort);
			return listDefs.First();
		}
		#endregion

		#region Insert
		///<summary>Make sure to also do a security log entry. Use DefL for that if in the UI layer.</summary>
		public static long Insert(Def def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				def.DefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.DefNum;
			}
			return Crud.DefCrud.Insert(def);
		}
		#endregion

		#region Update
		///<summary>Make sure to also do a security log entry. Use DefL for that if in the UI layer.</summary>
		public static void Update(Def def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.DefCrud.Update(def);
		}

		///<summary>Make sure to also do a security log entry. Use DefL for that if in the UI layer.</summary>
		public static void HideDef(Def def) {
			Meth.NoCheckMiddleTierRole();
			def.IsHidden=true;
			Defs.Update(def);
		}
		#endregion

		#region Delete
		///<summary>CAUTION.  This does not perform all validations.  Throws exceptions.</summary>
		public static void Delete(Def def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
					AutoNotes.RemoveFromCategory(def.DefNum);//set any autonotes assigned to this category to 0 (unassigned), user already warned about this
					List<Def> listDefsAutoNote=GetDefsForCategory(DefCat.AutoNoteCats);
					List<Def> listDefsChilden=listDefsAutoNote.FindAll(x => x.ItemValue==def.DefNum.ToString());
					for(int i=0;i<listDefsChilden.Count;i++) {
						//Set item value to empty string to remove previous linkage to parent category.
						listDefsChilden[i].ItemValue="";
						Update(listDefsChilden[i]);
						string logText=Lans.g("Defintions","Definition edited:")+" "+listDefsChilden[i].ItemName+" "
						+Lans.g("Defintions","with category:")+" "+listDefsChilden[i].Category.GetDescription();
						SecurityLogs.MakeLogEntry(EnumPermType.DefEdit,0,logText);
					}
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
		private static void DefCountValid(DefCat defCat) {
			Meth.NoCheckMiddleTierRole();//Private method.
			//Do not let the user delete the last WebSchedExistingApptType definition. Must be at least one.
			string command="SELECT COUNT(*) FROM definition WHERE Category="+POut.Int((int)defCat);
			if(PIn.Int(Db.GetCount(command),false)<=1) {
				throw new ApplicationException("NOT Allowed to delete the last def of this type.");
			}
		}

		///<summary>Returns true if the passed-in def is deprecated.  This method must be updated whenever another def is deprecated.</summary>
		public static bool IsDefDeprecated(Def def) {
			Meth.NoCheckMiddleTierRole();
			if(def.Category==DefCat.AccountColors && def.ItemName=="Received Pre-Auth") {
				return true;
			}
			return false;
		}

		///<summary>Returns true if the category needs at least one of its definitions to be unhidden.
		///The list of categories in the if statement of this method should only include those that can be hidden.</summary>
		public static bool NeedOneUnhidden(DefCat defCat) {
			Meth.NoCheckMiddleTierRole();
			if(//Definitions of categories that are commented out can be hidden and we want to allow users to hide ALL of them if they so desire
				defCat==DefCat.AdjTypes
				//|| defCat==DefCat.AccountQuickCharge
				|| defCat==DefCat.ApptConfirmed
				|| defCat==DefCat.ApptProcsQuickAdd
				//|| defCat==DefCat.AutoDeposit
				|| defCat==DefCat.BillingTypes
				|| defCat==DefCat.BlockoutTypes
				//|| defCat==DefCat.CarrierGroupNames
				|| defCat==DefCat.ClaimCustomTracking
				|| defCat==DefCat.ClaimPaymentGroups
				|| defCat==DefCat.ClaimPaymentTracking
				//|| defCat==DefCat.ClinicSpecialty
				|| defCat==DefCat.CommLogTypes
				|| defCat==DefCat.ContactCategories
				|| defCat==DefCat.Diagnosis
				|| defCat==DefCat.ImageCats
				|| defCat==DefCat.InsuranceFilingCodeGroup
				//|| defCat==DefCat.InsuranceVerificationGroup
				|| defCat==DefCat.LetterMergeCats
				|| defCat==DefCat.PaymentTypes
				//|| defCat==DefCat.PayPlanCategories
				|| defCat==DefCat.PaySplitUnearnedType
				|| defCat==DefCat.ProcButtonCats
				|| defCat==DefCat.ProcCodeCats
				|| defCat==DefCat.Prognosis
				|| defCat==DefCat.ProviderSpecialties
				|| defCat==DefCat.RecallUnschedStatus
				|| defCat==DefCat.TaskPriorities
				//|| defCat==DefCat.TimeCardAdjTypes
				|| defCat==DefCat.TxPriorities)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if there are any entries in definition that do not have a Category named "General".  
		///Returning false means the user has ProcButtonCategory customizations.</summary>
		public static bool HasCustomCategories() {
			Meth.NoCheckMiddleTierRole();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProcButtonCats);
			for(int i=0;i<listDefs.Count;i++) {
				if(!listDefs[i].ItemName.Equals("General")) { 
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if this definition is in use within the program. Consider enhancing this method if you add a definition category.
		///Does not check patient billing type or provider specialty since those are handled in their S-class.</summary>
		public static bool IsDefinitionInUse(Def def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),def);
			}
			List<string> listStrCommands=new List<string>();
			switch(def.Category) {
				case DefCat.AdjTypes:
					if(def.DefNum.In(
						PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType),
						PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType),
						PrefC.GetLong(PrefName.BillingChargeAdjustmentType),
						PrefC.GetLong(PrefName.FinanceChargeAdjustmentType),
						PrefC.GetLong(PrefName.LateChargeAdjustmentType),
						PrefC.GetLong(PrefName.SalesTaxAdjustmentType),
						PrefC.GetLong(PrefName.RefundAdjustmentType)))
					{
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM adjustment WHERE AdjType="+POut.Long(def.DefNum));
					break;
				case DefCat.ApptConfirmed:
					if(def.DefNum.In(
						PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger),
						PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger),
						PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
						PrefC.GetLong(PrefName.WebSchedNewPatConfirmStatus),
						PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus)))
					{
						return true;
					}
					if(def.DefNum.In(
						PrefC.GetLong(PrefName.ApptEConfirmStatusSent),
						PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted),
						PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined),
						PrefC.GetLong(PrefName.ApptEConfirmStatusSendFailed)))
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
					if(isClinicDefaultBillingType || PrefC.GetLong(PrefName.PracticeDefaultBillType)==def.DefNum) { //If practice default billing type is equal to the definitions DefNum, then in use.
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
					listStrCommands.Add("SELECT COUNT(*) FROM mountdef WHERE DefaultCat="+POut.Long(def.DefNum));
					if(def.DefNum!=0 && def.DefNum==PrefC.GetLong(PrefName.TaskAttachmentCategory)) {
						return true;
					}
					break;
				case DefCat.PaymentTypes:
					if(def.DefNum.In(PrefC.GetLong(PrefName.RecurringChargesPayTypeCC),PrefC.GetLong(PrefName.AccountingCashPaymentType)) || Defs.IsPaymentTypeInUse(def)) {
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM payment WHERE PayType="+POut.Long(def.DefNum));
					break;
				case DefCat.InsurancePaymentType:
					if(def.DefNum.In(
						PrefC.GetLong(PrefName.EraChkPaymentType),
						PrefC.GetLong(PrefName.EraAchPaymentType),
						PrefC.GetLong(PrefName.EraFwtPaymentType),
						PrefC.GetLong(PrefName.EraDefaultPaymentType)))
					{
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM claimpayment WHERE PayType="+POut.Long(def.DefNum));
					break;
				case DefCat.PaySplitUnearnedType:
					if(def.DefNum.In(PrefC.GetLong(PrefName.TpUnearnedType))) {
						return true;
					}
					listStrCommands.Add("SELECT COUNT(*) FROM paysplit WHERE UnearnedType="+POut.Long(def.DefNum));
					break;
				case DefCat.Prognosis:
					listStrCommands.Add("SELECT COUNT(*) FROM procedurelog WHERE Prognosis="+POut.Long(def.DefNum));
					break;
				case DefCat.RecallUnschedStatus:
					if(def.DefNum.In(
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

		///<summary>Takes a list of DefNums and updates the list to remove defNumFrom or replace with defNumTo if applicable. Returns either a null if there defNumFrom not in the list, or a comma-delimited string of defNums.</summary>
		public static List<string> RemoveOrReplaceDefNum(List<string> listStrDefNums,string defNumFrom,string defNumTo) {
			Meth.NoCheckMiddleTierRole();
			if(listStrDefNums.IsNullOrEmpty() || !listStrDefNums.Contains(defNumFrom)) {
				return null;//Nothing to update.
			}
			if(listStrDefNums.Contains(defNumTo)) {//defNumFrom and defNumTo are in the list, we just need remove defNumFrom.
				listStrDefNums.Remove(defNumFrom);
				return listStrDefNums;
			}
			//defNumFrom is in the list but defNumTo is not, replace defNumFrom.
			for(int i=0;i<listStrDefNums.Count;i++) {
				if(listStrDefNums[i]==defNumFrom) {
					listStrDefNums[i]=defNumTo;
				}
			}
			return listStrDefNums;
		}

		///<summary>Merges old Billing Type(FK DefNum) into the new Billing Type(FK DefNum).</summary>
		public static void MergeBillingTypeDefNums(long defNumFrom, long defNumTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNumFrom,defNumTo);
				return;
			}
			string strDefNumTo=POut.Long(defNumTo);
			string strDefNumFrom=POut.Long(defNumFrom);
			#region Tables with FK to Billing Type DefNum
			string command=$@"UPDATE dunning 
				SET BillingType={strDefNumTo} 
				WHERE BillingType={strDefNumFrom}";
			Db.NonQ(command);
			command=$@"UPDATE insplan 
				SET BillingType={strDefNumTo} 
				WHERE BillingType={strDefNumFrom}";
			Db.NonQ(command);
			command=$@"UPDATE patient 
				SET BillingType={strDefNumTo} 
				WHERE BillingType={strDefNumFrom}";
			Db.NonQ(command);
			command=$@"UPDATE procedurelog 
				SET BillingTypeOne={strDefNumTo} 
				WHERE BillingTypeOne={strDefNumFrom}";
			Db.NonQ(command);
			command=$@"UPDATE procedurelog 
				SET BillingTypeTwo={strDefNumTo} 
				WHERE BillingTypeTwo={strDefNumFrom}";
			Db.NonQ(command);
			if(PrefC.IsODHQ){
				command=$@"UPDATE reseller 
					SET BillingType={strDefNumTo} 
					WHERE BillingType={strDefNumFrom}";
				Db.NonQ(command);
			}
			#endregion
			#region Prefs
			command=$@"UPDATE preference 
				SET ValueString='{strDefNumTo}' 
				WHERE PrefName='{PrefName.PracticeDefaultBillType}' 
				AND ValueString='{strDefNumFrom}'";
			Db.NonQ(command);
			command=$@"UPDATE preference 
				SET ValueString='{strDefNumTo}' 
				WHERE PrefName='{PrefName.TransworldPaidInFullBillingType}' 
				AND ValueString='{strDefNumFrom}'";
			Db.NonQ(command);
			//Pref with comma-delimited list of Billing Type DefNums
			Prefs.UpdateDefNumsForPref(PrefName.ArManagerBillingTypes,strDefNumFrom,strDefNumTo);
			Prefs.UpdateDefNumsForPref(PrefName.BillingSelectBillingTypes,strDefNumFrom,strDefNumTo);
			Prefs.UpdateDefNumsForPref(PrefName.LateChargeDefaultBillingTypes,strDefNumFrom,strDefNumTo);
			#endregion
			#region ClinicPrefs
			command=$@"UPDATE clinicpref 
				SET ValueString='{strDefNumTo}' 
				WHERE PrefName='{PrefName.PracticeDefaultBillType}' 
				AND ValueString='{strDefNumFrom}'";
			Db.NonQ(command);
			//Pref with comma-delimited list of Billing Type DefNums
			ClinicPrefs.UpdateDefNumsForClinicPref(PrefName.BillingSelectBillingTypes,strDefNumFrom,strDefNumTo);
			#endregion
			#region ProgramProperties
			long programNum=Programs.GetProgramNum(ProgramName.TrojanExpressCollect);
			command=$@"UPDATE programproperty 
				SET PropertyValue='{strDefNumTo}' 
				WHERE ProgramNum={POut.Long(programNum)} 
				AND PropertyDesc='BillingType' 
				AND PropertyValue='{strDefNumFrom}'";
			Db.NonQ(command);
			#endregion
		}

		///<summary>Merges old document DocCategory(FK DefNum) into the new DocCategory(FK DefNum).</summary>
		public static void MergeImageCatDefNums(long defNumFrom, long defNumTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			long programNum=Programs.GetProgramNum(ProgramName.XVWeb);
			if(programNum!=0) {
				command="UPDATE programproperty"
					+" SET PropertyValue='"+POut.Long(defNumTo)+"'"
					+" WHERE ProgramNum="+POut.Long(programNum)+" AND PropertyDesc='ImageCategory' AND PropertyValue='"+POut.Long(defNumFrom)+"'";
				Db.NonQ(command);
			}
		}

		/// <summary>Returns true if the def is the default payment type for any of the payment programs, false otherwise </summary>
		public static bool IsPaymentTypeInUse(Def def) {
			Meth.NoCheckMiddleTierRole();
			if(def.Category!=DefCat.PaymentTypes) {
				return false;
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetWhere(x=>x.PropertyDesc.In("PaymentType","PaySimple Payment Type CC","PaySimple Payment Type ACH","EdgeExpressPaymentType"));
			if(listProgramProperties.Any(x=>x.PropertyValue==def.DefNum.ToString())) {
				return true;
			}
			return false;
		}

		#endregion

		#region CachePattern

		///<summary>The def cache technically does not utilize the PK column as the Key in the dictionary but it can safely utilize CacheDictAbs.
		///The reason it can safely use this version of our dict cache paradigm because the key is guaranteed to be unique and not duplicate.</summary>
		private class DefCache:CacheDictAbs<Def,DefCat,List<Def>> {
			protected override List<Def> GetCacheFromDb() {
				Meth.NoCheckMiddleTierRole();
				string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
				return Crud.DefCrud.SelectMany(command);
			}

			protected override List<Def> TableToList(DataTable table) {
				Meth.NoCheckMiddleTierRole();
				return Crud.DefCrud.TableToList(table);
			}

			protected override Def Copy(Def def) {
				Meth.NoCheckMiddleTierRole();
				return def.Copy();
			}

			protected override DataTable DictToTable(Dictionary<DefCat,List<Def>> dictAllItems) {
				Meth.NoCheckMiddleTierRole();
				return Crud.DefCrud.ListToTable(dictAllItems.SelectMany(x => x.Value).ToList(),"Def");
			}

			protected override void FillCacheIfNeeded() {
				Meth.NoCheckMiddleTierRole();
				Defs.GetTableFromCache(false);
			}

			protected override DefCat GetDictKey(Def def) {
				Meth.NoCheckMiddleTierRole();
				return def.Category;
			}

			protected override List<Def> GetDictValue(Def def) {
				throw new NotImplementedException();
			}

			protected override List<Def> GetShortValue(List<Def> listDefs) {
				Meth.NoCheckMiddleTierRole();
				return listDefs.FindAll(x => !x.IsHidden);
			}

			protected override List<Def> CopyDictValue(List<Def> listDefs) {
				Meth.NoCheckMiddleTierRole();
				return listDefs.Select(x => x.Copy()).ToList();
			}

			protected override Dictionary<DefCat,List<Def>> GetDictFromList(List<Def> listDefs) {
				Meth.NoCheckMiddleTierRole();
				Dictionary<DefCat,List<Def>> dict=new Dictionary<DefCat, List<Def>>();
				//An entry is required even if the category is currently empty.  E.g. ClinicSpecialties aren't required nor prefilled with any defs.
				string[] defCatNameArray=Enum.GetNames(typeof(DefCat));
				for(int i=0;i<defCatNameArray.Length;i++) {
					dict.Add((DefCat)i,new List<Def>());
				}
				Dictionary<DefCat,List<Def>> dictAllItems=listDefs.GroupBy(x => x.Category).ToDictionary(x => x.Key,x => x.ToList());
				foreach(KeyValuePair<DefCat,List<Def>> kvp in dictAllItems) {
					dict[kvp.Key]=kvp.Value;
				}
				return dict;
			}

			protected override List<DefCat> GetDictShortKeys(List<Def> listDefs) {
				Meth.NoCheckMiddleTierRole();
				//The defs cache is unique in the sense that it doesn't matter what items came over from the database.
				//No matter what (short or long) all values in the enum DefCat must be a key.
				return Enum.GetValues(typeof(DefCat)).Cast<DefCat>().ToList();
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DefCache _defCache=new DefCache();

		public static Dictionary<DefCat,List<Def>> GetDeepCopy(bool isShort=false) {
			Meth.NoCheckMiddleTierRole();
			return _defCache.GetDeepCopy(isShort);
		}

		///<summary>Set isShort to true to exclude the Defs.IsHidden.</summary>
		public static List<Def> GetDefsForCategory(DefCat defCat,bool isShort=false) {
			Meth.NoCheckMiddleTierRole();
			return _defCache.GetOne(defCat,isShort);
		}

		public static bool GetDictIsNull() {
			Meth.NoCheckMiddleTierRole();
			return _defCache.DictIsNull();
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			Meth.NoCheckMiddleTierRole();
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			Meth.NoCheckMiddleTierRole();
			_defCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_defCache.FillCacheFromTable(table);
				return table;
			}
			return _defCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			Meth.NoCheckMiddleTierRole();
			_defCache.ClearCache();
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
		E,
		///<summary>Claim Responses.</summary>
		N,
		///<summary>Autosave Forms (only one)</summary>
		U,
		///<summary>Claim Attachments (only one)</summary>
		C,
	}


}
