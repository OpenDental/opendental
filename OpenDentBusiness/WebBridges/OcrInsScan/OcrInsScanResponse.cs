using System;

namespace OpenDentBusiness{
    /// <summary> Response from Azure form Recognizer. contains many fields that an insurance card might. </summary>
    [Serializable]
	public class OcrInsScanResponse {
        /// <summary> Health insurance provider name.  </summary>
        public string Insurer;
        /// <summary> Owner of card. </summary>
        public Member Member;
        /// <summary> Array of dependents on the card. </summary>
        public Dependent[] Dependents;
        /// <summary> ID number and Prefix. </summary>
        public IdNumber IdNumber;
        /// <summary> Insurance group number. </summary>
        public string GroupNumber;
        /// <summary> Prescription Info. </summary>
        public PrescriptionInfo PrescriptionInfo;
        /// <summary> Pharmacy Benefit Manager for the plan. </summary>
        public string Pbm;
        /// <summary> Date from which the pan is effective. </summary>
        public DateTimeOffset EffectiveDate;
         /// <summary> Array of copay benefits. </summary>
        public Copay[] Copays;
        /// <summary> Payer Info. </summary>
        public Payer Payer;
        /// <summary> Plan Info. </summary>
        public Plan Plan;

		/*
		 Insurer string
         Member object 		
             Member.Name string
             Member.BirthDate date 
             Member.Employer string 
             Member.Gender string
             Member.IdNumberSuffix string
         Dependents array	
            Dependents.* object 		
                Dependents.*.Name string 
         IdNumber object 		
             IdNumber.Prefix string
             IdNumber.Number string
         GroupNumber string
         PrescriptionInfo object 		
             PrescriptionInfo.Issuer string
             PrescriptionInfo.RxBIN string
             PrescriptionInfo.RxPCN string
             PrescriptionInfo.RxGrp string
             PrescriptionInfo.RxId string
             PrescriptionInfo.RxPlan string
         Pbm string
         EffectiveDate date
         Copays array
             Copays.* object 		
                 Copays.*.Benefit string
                 Copays.*.Amount currency
         Payer object 		
            Payer.Id string 
            Payer.Address 
            Payer.PhoneNumber phoneNumber
         Plan object 		
            Plan.Number string
            Plan.Name string
            Plan.Type string
		*/
 
	}

    public class Member {
        public string Name;
        public DateTimeOffset Birthdate;
        //THIS IS GROUP NAME
        public string Employer;
        public string Gender;
        public string IdNumberSuffix;
    }

    public class Dependent {
        public string Name;
    }

    public class IdNumber {
        public string Prefix;
        public string Number;
    }

    public class PrescriptionInfo {
        public string Issuer;
        public string RxBIN;
        public string RxPCN;
        public string RxGrp;
        public string RxId;
        public string RxPlan;
    }

    public  class Copay {
        public string Benefit;
        public double Amount;
    }

    public class Payer {
        public string Id;
        public string Address;
        public string PhoneNumber;
    }

    public class Plan {
        public string Number;
        public string Name;
        public string Type;
    }

}
