using System;
using System.Collections;

namespace OpenDentBusiness.Mobile{
	///<summary>One row for each patient.  Unlike main program, this does not include deleted patients.  Primary key is first two fields combined.</summary>
	[Serializable()]
	[CrudTable(IsMobile=true)]
	public class Patientm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long PatNum;
		/// <summary>Last name.</summary>
		public string LName;
		/// <summary>First name.</summary>
		public string FName;
		/// <summary>Middle initial or name.</summary>
		public string MiddleI;
		/// <summary>Preferred name, aka nickname.</summary>
		public string Preferred;
		/// <summary>Enum:PatientStatus</summary>
		public PatientStatus PatStatus;
		/// <summary>Enum:PatientGender</summary>
		public PatientGender Gender;
		/// <summary>Enum:PatientPosition Marital status would probably be a better name for this column.</summary>
		public PatientPosition Position;
		/// <summary>Age is not stored in the database.  Age is always calculated as needed from birthdate.</summary>
		public DateTime Birthdate;
		/// <summary>.</summary>
		public string Address;
		/// <summary>Optional second address line.</summary>
		public string Address2;
		/// <summary>.</summary>
		public string City;
		/// <summary>2 Char in USA</summary>
		public string State;
		/// <summary>Postal code.  For Canadian claims, it must be ANANAN.  No validation gets done except there.</summary>
		public string Zip;
		/// <summary>Home phone. Includes any punctuation</summary>
		public string HmPhone;
		/// <summary>.</summary>
		public string WkPhone;
		/// <summary>.</summary>
		public string WirelessPhone;
		/// <summary>FK to patientm.PatNum.  Head of household.</summary>
		public long Guarantor;
		/// <summary>Derived from Birthdate.  Not in the database table.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public int Age;
		/// <summary>.</summary>
		public string Email;
		/// <summary>Address or phone note.  Will probably limit to first 255 characters of patient.AddrNote.</summary>
		public string AddrNote;
		/// <summary>FK to clinic.ClinicNum. Can be zero if not attached to a clinic or no clinics set up.</summary>
		public long ClinicNum;
		/// <summary>Insurance Estimate for entire family.</summary>
		public double InsEst;
		/// <summary>Total balance for entire family before insurance estimate.  Not the same as the sum of the 4 aging balances because this can be negative.  Only stored with guarantor.</summary>
		public double BalTotal;
		/// <summary>Enum:ContactMethod</summary>
		public ContactMethod PreferContactMethod;
		///<summary>If this is blank, then the chart info for this patient will not be uploaded.  If this has a value, then this is the password that a patient must use to access their info online.</summary>
		public string OnlinePassword;
		
		///<summary>Returns a copy of this Patientm.</summary>
		public Patientm Copy(){
			return (Patientm)this.MemberwiseClone();
		}

		public override string ToString() {
			return "Patient: "+GetNameLF();
		}

		///<summary>LName, 'Preferred' FName M</summary>
		public string GetNameLF(){
			return Patients.GetNameLF(LName,FName,Preferred,MiddleI);
		}

		///<summary>FName 'Preferred' M LName</summary>
		public string GetNameFL(){
			return Patients.GetNameFL(LName,FName,Preferred,MiddleI);
		}

		///<summary>FName M LName</summary>
		public string GetNameFLnoPref() {
			return Patients.GetNameFLnoPref(LName,FName,MiddleI);
		}

		///<summary>FName/Preferred LName</summary>
		public string GetNameFirstOrPrefL(){
			return Patients.GetNameFirstOrPrefL(LName,FName,Preferred);
		}

		///<summary>FName/Preferred M. LName</summary>
		public string GetNameFirstOrPrefML(){
			return Patients.GetNameFirstOrPrefML(LName,FName,Preferred,MiddleI);
		}

		//<summary>Title FName M LName</summary>
		//public string GetNameFLFormal() {
		//	return Patients.GetNameFLFormal(LName,FName,MiddleI,Title);
		//}

		///<summary>Includes preferred.</summary>
		public string GetNameFirst() {
			return Patients.GetNameFirst(FName,Preferred);
		}

		///<summary></summary>
		public string GetNameFirstOrPreferred() {
			return Patients.GetNameFirstOrPreferred(FName,Preferred);
		}

		
	}

}










