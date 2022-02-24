using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RequiredFields{
		#region Get Methods
		/// <summary>Returns a hardcoded list. Needs to be updated whenever a new field or type is added.</summary>
		/// If we are going to continue adding to the RequiredField class, we should consider refactoring to get rid of methods like this one.
		public static List<RequiredFieldName> GetFieldNamesForType(RequiredFieldType type) {
			List<RequiredFieldName> retVal=new List<RequiredFieldName>();
			switch(type) {
				case RequiredFieldType.PatientInfo:
					retVal=new List<RequiredFieldName>{RequiredFieldName.Address,RequiredFieldName.Address2,RequiredFieldName.AddressPhoneNotes,RequiredFieldName.AdmitDate,
						RequiredFieldName.AskArriveEarly,RequiredFieldName.BillingType,RequiredFieldName.Birthdate,RequiredFieldName.Carrier,
						RequiredFieldName.ChartNumber,RequiredFieldName.City,RequiredFieldName.Clinic,RequiredFieldName.CollegeName,RequiredFieldName.County,
						RequiredFieldName.CreditType,RequiredFieldName.DateFirstVisit,RequiredFieldName.DateTimeDeceased,RequiredFieldName.EligibilityExceptCode,
						RequiredFieldName.EmailAddress,RequiredFieldName.EmergencyName,RequiredFieldName.EmergencyPhone,RequiredFieldName.Employer,
						RequiredFieldName.Ethnicity,RequiredFieldName.FeeSchedule,RequiredFieldName.FirstName,RequiredFieldName.Gender,RequiredFieldName.GenderIdentity,
						RequiredFieldName.GradeLevel,RequiredFieldName.GroupName,RequiredFieldName.GroupNum,RequiredFieldName.HomePhone,RequiredFieldName.InsurancePhone,
						RequiredFieldName.InsuranceSubscriber,RequiredFieldName.InsuranceSubscriberID,RequiredFieldName.Language,RequiredFieldName.LastName,
						RequiredFieldName.Position,RequiredFieldName.MedicaidID,RequiredFieldName.MedicaidState,RequiredFieldName.MiddleInitial,
						RequiredFieldName.MothersMaidenFirstName,RequiredFieldName.MothersMaidenLastName,RequiredFieldName.PatientStatus,
						RequiredFieldName.PreferConfirmMethod,RequiredFieldName.PreferContactMethod,RequiredFieldName.PreferRecallMethod,RequiredFieldName.PreferredName,
						RequiredFieldName.PrimaryProvider,RequiredFieldName.Race,RequiredFieldName.ReferredFrom,RequiredFieldName.ResponsibleParty,
						RequiredFieldName.Salutation,RequiredFieldName.SecondaryProvider,RequiredFieldName.SexualOrientation,RequiredFieldName.Site,
						RequiredFieldName.SocialSecurityNumber,RequiredFieldName.State,RequiredFieldName.StudentStatus,RequiredFieldName.TextOK,RequiredFieldName.Title,
						RequiredFieldName.TreatmentUrgency,RequiredFieldName.TrophyFolder,RequiredFieldName.Ward,RequiredFieldName.WirelessPhone,
						RequiredFieldName.WorkPhone,RequiredFieldName.Zip};
					break;
				case RequiredFieldType.InsPayEdit:
					retVal=new List<RequiredFieldName>{ RequiredFieldName.BatchNumber,RequiredFieldName.CheckDate,RequiredFieldName.CheckNumber,
						RequiredFieldName.DepositAccountNumber,RequiredFieldName.DepositDate,RequiredFieldName.InsPayEditClinic,
						RequiredFieldName.PaymentAmount,RequiredFieldName.PaymentType };
					break;
				default:
					break;
			}

			return retVal;
		}
		#endregion

		#region CachePattern

		private class RequiredFieldCache : CacheListAbs<RequiredField> {
			protected override List<RequiredField> GetCacheFromDb() {
				string command="SELECT * FROM requiredfield ORDER BY FieldType,FieldName";
				return Crud.RequiredFieldCrud.SelectMany(command);
			}
			protected override List<RequiredField> TableToList(DataTable table) {
				return Crud.RequiredFieldCrud.TableToList(table);
			}
			protected override RequiredField Copy(RequiredField requiredField) {
				return requiredField.Clone();
			}
			protected override DataTable ListToTable(List<RequiredField> listRequiredFields) {
				return Crud.RequiredFieldCrud.ListToTable(listRequiredFields,"RequiredField");
			}
			protected override void FillCacheIfNeeded() {
				RequiredFields.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static RequiredFieldCache _requiredFieldCache=new RequiredFieldCache();

		public static List<RequiredField> GetDeepCopy(bool isShort=false) {
			return _requiredFieldCache.GetDeepCopy(isShort);
		}

		public static List<RequiredField> GetWhere(Predicate<RequiredField> match,bool isShort=false) {
			return _requiredFieldCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_requiredFieldCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_requiredFieldCache.FillCacheFromTable(table);
				return table;
			}
			return _requiredFieldCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(RequiredField requiredField){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				requiredField.RequiredFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),requiredField);
				return requiredField.RequiredFieldNum;
			}
			return Crud.RequiredFieldCrud.Insert(requiredField);
		}
		
		///<summary></summary>
		public static void Update(RequiredField requiredField){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),requiredField);
				return;
			}
			Crud.RequiredFieldCrud.Update(requiredField);
		}

		///<summary></summary>
		public static void Delete(long requiredFieldNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),requiredFieldNum);
				return;
			}
			string command="DELETE FROM requiredfieldcondition WHERE RequiredFieldNum="+POut.Long(requiredFieldNum);
			Db.NonQ(command);
			Crud.RequiredFieldCrud.Delete(requiredFieldNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<RequiredField> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RequiredField>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM requiredfield WHERE PatNum = "+POut.Long(patNum);
			return Crud.RequiredFieldCrud.SelectMany(command);
		}

		///<summary>Gets one RequiredField from the db.</summary>
		public static RequiredField GetOne(long requiredFieldNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<RequiredField>(MethodBase.GetCurrentMethod(),requiredFieldNum);
			}
			return Crud.RequiredFieldCrud.SelectOne(requiredFieldNum);
		}
		*/
	}
}