using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness {

	public static class TsiMsgConstructor {

		///<summary>Returns a message string used to place a patient or guarantor account with TSI for collection.</summary>
		public static string GeneratePlacement(PatAging patAge,string clientID,TsiServiceType serviceType) {
			Family fam=Patients.GetFamily(patAge.PatNum);
			Patient pat=fam.ListPats.FirstOrDefault(x => x.PatNum==patAge.PatNum);
			Patient guar=fam.ListPats[0];
			if(pat==null || guar==null) {
				throw new ApplicationException("Invalid PatNum.  Please contact support.");
			}
			string[] fieldVals=new string[33] {
				clientID,
				"",//since PatNums can be larger than 10 digits, we will send in field 3 which can hold up to 20 digits
				POut.Long(patAge.Guarantor),
				guar.GetNameLFnoPref(),
				guar.Address,
				guar.Address2,
				guar.City,
				guar.State,
				guar.Zip,
				guar.HmPhone,
				guar.WirelessPhone,
				guar.SSN,
				guar.Birthdate.ToString("MMddyyyy"),
				gGetPatType(guar,pat),
				pat.FName,
				pat.LName,
				pat.HmPhone,
				pat.WirelessPhone,
				pat.Birthdate.ToString("MMddyyyy"),
				pat.SSN,
				pat.Address,
				pat.Address2,
				pat.City,
				pat.State,
				pat.Zip,
				pat.Country,//country will be blank unless HQ
				gGetLanguageString(pat.Language),//will be blank if not set
				patAge.DateBalBegan.ToString("MMddyyyy"),
				POut.Double(patAge.AmountDue),//POut.Double so it will be in the format XXX.XX with 2 decimal places,
				patAge.DateLastPay.ToString("MMddyyyy"),
				Math.Max((int)serviceType,1).ToString(),//(enum 0 based, send 1 for Accelerator - 0 and ProfitRecovery - 1) 1 - AcceleratorPr, 2 - ProfessionalCollections
				((int)TsiServiceCode.Diplomatic+1).ToString(),//(enum 0 based, plus 1 to send to TSI) 1 - Diplomatic, 2 - Intensive or 3 - Bad Check.
				"Transferred to TSI" };
			return fieldVals.Aggregate((a,b) => (a??"")+"|"+(b??""));
		}

		public static string GenerateUpdate(long patNum,string clientID,TsiTransType transType,double transAmount,double newBal) {
			string[] fieldVals=new string[6] {
				clientID,
				POut.Long(patNum),
				transType.ToString(),
				DateTime.Today.ToString("MMddyyyy"),
				POut.Double(Math.Abs(transAmount)),//msgs sent with pos amt, Transworld uses tran type to determine whether it increases or decreases amt owed
				POut.Double(newBal)
			};
			return fieldVals.Aggregate((a,b) => (a??"")+"|"+(b??""));
		}

		///<summary>0=self or default,1=spouse or significant other,2=parent or guardian,3=child,4=other.
		///Defaults to 4 - other if not able to determine pat relationship to guar.</summary>
		private static string gGetPatType(Patient guar,Patient pat) {
			string retval="";
			List<Guardian> listGuardians=Guardians.Refresh(pat.PatNum);
			Guardian guard=listGuardians.Find(x => x.PatNumGuardian==guar.PatNum);
			if(guard!=null) {
				switch(guard.Relationship) {
					case GuardianRelationship.Self:
						retval="0";//self
						break;
					case GuardianRelationship.Spouse:
					case GuardianRelationship.LifePartner:
						retval="1";//spouse or significant other
						break;
					case GuardianRelationship.Father:
					case GuardianRelationship.Stepfather:
					case GuardianRelationship.Mother:
					case GuardianRelationship.Stepmother:
					case GuardianRelationship.Parent:
					case GuardianRelationship.CareGiver:
					case GuardianRelationship.Guardian:
						retval="2";//parent or guardian
						break;
					case GuardianRelationship.Child:
					case GuardianRelationship.Stepchild:
					case GuardianRelationship.FosterChild:
						retval="3";//child
						break;
					case GuardianRelationship.Grandfather:
					case GuardianRelationship.Grandmother:
					case GuardianRelationship.Grandparent:
					case GuardianRelationship.Grandchild:
					case GuardianRelationship.Sitter:
					case GuardianRelationship.Brother:
					case GuardianRelationship.Other:
					case GuardianRelationship.Sibling:
					case GuardianRelationship.Sister:
					case GuardianRelationship.Friend:
					default:
						retval="4";//other
						break;
				}
			}
			else {
				if(guar.PatNum==pat.PatNum) {
					retval="0";//self or default
				}
				else if(guar.Position==PatientPosition.Married && pat.Position==PatientPosition.Married) {
					retval="1";//spouse or significant other
				}
				else if(guar.Position==PatientPosition.Child && pat.Position!=PatientPosition.Child) {
					retval="2";//pat is parent or guardian of guarantor child?? not likely to happen that the guarantor is the child, but just in case
				}
				else if(guar.Position!=PatientPosition.Child && pat.Position==PatientPosition.Child) {
					retval="3";//pat is child of guar
				}
				else {
					retval="4";//can't determine relationship, default to other
				}
			}
			return retval;
		}

		///<summary>Returns display name of 3 letter language abbr or custom language name if not found.  Will return empty string, not null.</summary>
		private static string gGetLanguageString(string language) {
			string retval="";
			CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(language);
			if(culture==null) {//custom language or language is null or empty
				retval=language;
			}
			else {
				retval=culture.DisplayName;
			}
			return retval??"";
		}

		public static string GetPlacementFileHeader() {
			return @"CLIENT NUMBER|TRANSMITTAL NUMBER|DEBTOR REFERENCE|DEBTOR NAME|ADDRESS|ADDRESS2|CITY|STATE|ZIP|DEBTOR PHONE|SECONDARY PHONE|DEBTOR SSN|DEBTOR DATE OF BIRTH|PATIENTTYPE|PATIENTFIRSTNAME|PATIENTLASTNAME|PATIENTPHONE1|PATIENTPHONE2|PATIENTDOB|PATIENTSSN|PATIENTADDRESS|PATIENTADDRESS2|PATIENTCITY|PATIENTSTATE|PATIENTZIP|PATIENTCOUNTRY|LANGUAGE|DATE OF DEBT|AMOUNT DUE|DATE OF LAST PAY|SERVICETYPE|SERVICECODE|COMMENTS";
		}

		public static string GetUpdateFileHeader() {
			return @"CLIENT NUMBER|TRANSMITTAL/REFERENCE NUMBER|TRANSACTION TYPE|TRANSACTION DATE|TRANSACTION AMOUNT|NEW BALANCE";
		}

	}
}
