using System;
using System.Collections.Generic;

namespace OpenDentBusiness {
	///<summary>Each row represents a field that is required to be filled out.</summary>
	[Serializable]
	public class RequiredField:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RequiredFieldNum;
		///<summary>Enum:RequiredFieldType . The area of the program that uses this field.</summary>
		public RequiredFieldType FieldType;
		///<summary>Enum:RequiredFieldName </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public RequiredFieldName FieldName;
		///<summary>This is not a data column but is stored in a seperate table named RequiredFieldCondition.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<RequiredFieldCondition> _listRequiredFieldConditions;

		public List<RequiredFieldCondition> ListRequiredFieldConditions {
			get {
				if(_listRequiredFieldConditions==null) {
					if(RequiredFieldNum==0) {
						_listRequiredFieldConditions=new List<RequiredFieldCondition>();
					}
					else {
						_listRequiredFieldConditions=RequiredFieldConditions.GetForRequiredField(RequiredFieldNum);
					}
				}
				return _listRequiredFieldConditions;
			}
			set {
				_listRequiredFieldConditions=value;
			}
		}

		///<summary>Refreshes the list holding the requirefieldconditions for this requiredfield.</summary>
		public void RefreshConditions() {
			_listRequiredFieldConditions=null;
			RequiredFieldConditions.RefreshCache();
		}

		///<summary></summary>
		public RequiredField Clone() {
			return (RequiredField)this.MemberwiseClone();
		}
	}

	///<summary>The part of the program where this required field is used.</summary>
	public enum RequiredFieldType {
		///<summary>0 - Edit Patient Information window and Add Family window.</summary>
		PatientInfo,
		///<summary>1 - Edit Claim Payment window.</summary>
		InsPayEdit
	}

	///<summary>This enum is stored as a string, so the order of values can be rearranged.</summary>
	public enum RequiredFieldName {
		///<summary></summary>
		Address,
		///<summary></summary>
		Address2,
		///<summary></summary>
		AddressPhoneNotes,
		///<summary></summary>
		AdmitDate,
		///<summary></summary>
		AskArriveEarly,
		///<summary></summary>
		BatchNumber,
		///<summary></summary>
		BillingType,
		///<summary></summary>
		Birthdate,
		///<summary></summary>
		Carrier,
		///<summary></summary>
		ChartNumber,
		///<summary></summary>
		CheckDate,
		///<summary></summary>
		CheckNumber,
		///<summary></summary>
		City,
		///<summary></summary>
		Clinic,
		///<summary></summary>
		CollegeName,
		///<summary></summary>
		County,
		///<summary></summary>
		CreditType,
		///<summary></summary>
		DateFirstVisit,
		///<summary></summary>
		DateTimeDeceased,
		///<summary></summary>
		DepositAccountNumber,
		///<summary></summary>
		DepositDate,
		///<summary></summary>
		EligibilityExceptCode,
		///<summary></summary>
		EmailAddress,
		///<summary></summary>
		EmergencyName,
		///<summary></summary>
		EmergencyPhone,
		///<summary></summary>
		Employer,
		///<summary></summary>
		Ethnicity,
		///<summary></summary>
		FeeSchedule,
		///<summary></summary>
		FirstName,
		///<summary></summary>
		Gender,
		///<summary></summary>
		GenderIdentity,
		///<summary></summary>
		GradeLevel,
		///<summary></summary>
		GroupName,
		///<summary></summary>
		GroupNum,
		///<summary></summary>
		HomePhone,
		///<summary></summary>
		InsPayEditClinic,
		///<summary></summary>
		InsurancePhone,
		///<summary></summary>
		InsuranceSubscriber,
		///<summary></summary>
		InsuranceSubscriberID,
		///<summary></summary>
		Language,
		///<summary></summary>
		LastName,
		///<summary></summary>
		PaymentAmount,
		///<summary></summary>
		PaymentType,
		///<summary></summary>
		Position,
		///<summary></summary>
		MedicaidID,
		///<summary></summary>
		MedicaidState,
		///<summary></summary>
		MiddleInitial,
		///<summary></summary>
		MothersMaidenFirstName,
		///<summary></summary>
		MothersMaidenLastName,
		///<summary></summary>
		PatientStatus,
		///<summary></summary>
		PreferConfirmMethod,
		///<summary></summary>
		PreferContactMethod,
		///<summary></summary>
		PreferRecallMethod,
		///<summary></summary>
		PreferredName,
		///<summary></summary>
		PrimaryProvider,
		///<summary></summary>
		Race,
		///<summary></summary>
		ReferredFrom,
		///<summary></summary>
		ResponsibleParty,
		///<summary></summary>
		Salutation,
		///<summary></summary>
		SecondaryProvider,
		///<summary></summary>
		SexualOrientation,
		///<summary></summary>
		Site,
		///<summary></summary>
		SocialSecurityNumber,
		///<summary></summary>
		State,
		///<summary></summary>
		StudentStatus,
		///<summary></summary>
		TextOK,
		///<summary></summary>
		Title,
		///<summary></summary>
		TreatmentUrgency,
		///<summary></summary>
		TrophyFolder,
		///<summary></summary>
		Ward,
		///<summary></summary>
		WirelessPhone,
		///<summary></summary>
		WorkPhone,
		///<summary></summary>
		Zip
	}
}