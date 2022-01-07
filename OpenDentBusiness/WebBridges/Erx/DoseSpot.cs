using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace OpenDentBusiness {

	public class DoseSpot {

		private static string _doseSpotPatNum="25128";//25128 is DoseSpot's patnum in the OD HQ database.
		private static string _doseSpotOid="2.16.840.1.113883.3.4337.25128";
		private static string _randomPhrase32=null;

		#region OID

		///<summary>The PatNum associated to this office's regkey in the OD HQ database.  The patnum will be parsed from the database instead 
		/// of making web calls because Dose Spot would explode if the patnum from OD HQ changed ever</summary>
		public static long PatNumDoseSpotCustomer {
			get {
				string rootExternal=GetDoseSpotRoot();
				long patNum=0;
				//Gets the value of the last section of the root, which for DoseSpot/Internal OIDs is the PatNum of the office in the OD HQ database.
				//This is grabbed instead of asking OD HQ for the patnum associated to the account 
				// because there might be an edge case where the registration key for this office got moved to a new PatNum,
				// and that would hide every DoseSpot OID link.
				try {
					patNum=PIn.Long(rootExternal.Substring(rootExternal.LastIndexOf(".")+1));
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				return patNum;
			}
		}

		///<summary>Returns the rootExternal that identifies OIDs as Dose Spot </summary>
		public static string GetDoseSpotRoot() {
			//No need to check RemotingRole; no call to db.
			//The advantage of returning the root from the database is that there could be a scary edge case where 
			// the PatNum of the office's regkey in OD HQ database could have been changed
			OIDExternal oIDExternal=OIDExternals.GetByPartialRootExternal(_doseSpotOid);
			if(oIDExternal==null) {
				oIDExternal=new OIDExternal();
				oIDExternal.IDType=IdentifierType.Root;
				oIDExternal.rootExternal=OIDInternals.OpenDentalOID+"."+_doseSpotPatNum+"."+OIDInternals.CustomerPatNum;
				OIDExternals.Insert(oIDExternal);
			}
			return oIDExternal.rootExternal;
		}

		///<summary>Gets the OIDExternal corresponding to Dose Spot and the patnum given.  Returns null if no match found.</summary>
		public static OIDExternal GetDoseSpotPatID(long patNum) {
			//No remoting role check needed
			return OIDExternals.GetOidExternal(GetDoseSpotRoot()+"."+POut.Int((int)IdentifierType.Patient),patNum,IdentifierType.Patient);
		}

		///<summary>Gets the OIDExternal corresponding to Dose Spot oid given.  Returns null if no match found.
		///If its not null this means the practice has used DoseSpot at least once.</summary>
		public static OIDExternal GetDoseSpotRootOid() {
			//No remoting role check needed
			return OIDExternals.GetByPartialRootExternal(_doseSpotOid);
		}

		#endregion

		public static void MakeClinicErxAlert(ClinicErx clinicErx,bool clinicAutomaticallyAttached) {
			AlertItem alertItem=AlertItems.RefreshForType(AlertType.DoseSpotClinicRegistered)
				.FirstOrDefault(x => x.FKey==clinicErx.ClinicErxNum);
			if(alertItem!=null) {
				return;//alert already exists
			}
      Clinic clinic=Clinics.GetClinic(clinicErx.ClinicNum);
      List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(Programs.GetProgramNum(ProgramName.eRx))
          .FindAll(x => x.ClinicNum==0
            && (x.PropertyDesc==Erx.PropertyDescs.ClinicID || x.PropertyDesc==Erx.PropertyDescs.ClinicKey)
            && !string.IsNullOrWhiteSpace(x.PropertyValue));
      if(clinic!=null || clinicAutomaticallyAttached) {
        //A clinic was associated with the clinicerx successfully, no user action needed.
        alertItem=new AlertItem {
          Actions=ActionType.MarkAsRead | ActionType.Delete,
          Description=(clinicErx.ClinicDesc=="" ? "Headquarters" : clinicErx.ClinicDesc)+" "+Lans.g("DoseSpot","has been registered"),
          Severity=SeverityType.Low,
          Type=AlertType.DoseSpotClinicRegistered,
          FKey=clinicErx.ClinicErxNum,
          ClinicNum=clinicErx.ClinicNum,
        };
      }
      else {
        //User action needed to make a link to an existing clinic that was registered.
        alertItem=new AlertItem {
          Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.OpenForm,
          Description=Lans.g("DoseSpot","Select clinic to assign ID"),
          Severity=SeverityType.Low,
          Type=AlertType.DoseSpotClinicRegistered,
          ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because we don't know which clinic to display in.
          FKey=clinicErx.ClinicErxNum,
          FormToOpen=FormType.FormDoseSpotAssignClinicId,
        };
      }
      AlertItems.Insert(alertItem);
		}

		///<summary>Handles assigning Dose Spot ID to a user with matching NPI.
		///If multiple or no matches, creates form for manual selection of user.</summary>
		public static void MakeProviderErxAlert(ProviderErx providerErx) {
			AlertItem alertItem=AlertItems.RefreshForType(AlertType.DoseSpotProviderRegistered)
				.FirstOrDefault(x => x.FKey==providerErx.ProviderErxNum);
			if(alertItem!=null) {
				return;//alert already exists
			}
			//get a list of users that correspond to a non-hidden provider
			List<Provider> listProviders=Providers.GetWhere(x => x.NationalProvID==providerErx.NationalProviderID,true);
			List<Userod> listUserods=Userods.GetWhere(x => listProviders.Exists(y => y.ProvNum==x.ProvNum),true);//Only consider non-hidden users.
			if(listUserods.Count==1) {//One provider matched so simply notify the office and set the DoseSpot User Id.
				alertItem=new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.ShowItemValue,
					Description=Lans.g("DoseSpot","User automatically assigned."),
					Severity=SeverityType.Low,
					Type=AlertType.DoseSpotProviderRegistered,
					FKey=providerErx.ProviderErxNum,
					ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because providers aren't clinic specific
					ItemValue=Lans.g("DoseSpot","User: ")+listUserods[0].UserNum+", "+listUserods[0].UserName+" "
					+Lans.g("DoseSpot","has been assigned a DoseSpot User ID of: ")+providerErx.UserId,
				};
				AlertItems.Insert(alertItem);
				//set userodpref to UserId
				Program program=Programs.GetCur(ProgramName.eRx);
				UserOdPref userOdPref=UserOdPrefs.GetByCompositeKey(listUserods[0].UserNum,program.ProgramNum,UserOdFkeyType.Program);
				userOdPref.ValueString=providerErx.UserId;//assign DoseSpot User ID
				if(userOdPref.IsNew) {
					userOdPref.Fkey=program.ProgramNum;
					UserOdPrefs.Insert(userOdPref);
				}
				else {
					UserOdPrefs.Update(userOdPref);
				}
			}
			else {//More than one or no user associated to the NPI, generate alert with form to have the office choose which user to assign.
				alertItem=new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.OpenForm,
					Description=Lans.g("DoseSpot","Select user to assign ID"),
					Severity=SeverityType.Low,
					Type=AlertType.DoseSpotProviderRegistered,
					FKey=providerErx.ProviderErxNum,
					ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because providers aren't clinic specific
					FormToOpen=FormType.FormDoseSpotAssignUserId,
				};
				AlertItems.Insert(alertItem);
			}
		}

		///<summary>Returns true if the passed in accountId is a DoseSpot account id.</summary>
		public static bool IsDoseSpotAccountId(string accountId) {
			//No need to check RemotingRole; no call to db.
			return accountId.ToUpper().StartsWith("DS;");//ToUpper because user might have manually typed in.
		}

		///<summary>Creates a unique account id for DoseSpot.  Uses the same generation logic as NewCrop, with DS; preceeding it.</summary>
		public static string GenerateAccountId(long patNum) {
			string accountId="DS;"+POut.Long(patNum);
			accountId+="-"+CodeBase.MiscUtils.CreateRandomAlphaNumericString(3);
			long checkSum=patNum;
			checkSum+=Convert.ToByte(accountId[accountId.IndexOf('-')+1])*3;
			checkSum+=Convert.ToByte(accountId[accountId.IndexOf('-')+2])*5;
			checkSum+=Convert.ToByte(accountId[accountId.IndexOf('-')+3])*7;
			accountId+=(checkSum%100).ToString().PadLeft(2,'0');
			return accountId;
		}

		///<summary>The comments on each line come directly from the DoseSpot API Guide 12.8.pdf.  Creates a Single Sign On Code for DoseSpot.</summary>
		public static string CreateSsoCode(string clinicKey,bool isQueryStr=true) {
			//No need to check RemotingRole; no call to db.
			string singleSignOnCode="";//1. You have been provided a clinic key (in UTF8).
			string phrase=Get32CharPhrase();//2. Create a random phrase 32 characters long in UTF8
			string phraseAndKey=phrase;
			phraseAndKey+=clinicKey;//3. Append the key to the phrase
			byte[] arrayBytes=GetBytesFromUTF8(phraseAndKey);//4. Get the value in Bytes from UTF8 String
			byte[] arrayHashedBytes=GetSHA512Hash(arrayBytes);//5. Use SHA512 to hash the byte value you just received
			string base64hash=Convert.ToBase64String(arrayHashedBytes);//6. Get a Base64String out of the hash that you created
			base64hash=RemoveExtraEqualSigns(base64hash);//7. If there are two = signs at the end, then remove them
			singleSignOnCode=phrase+base64hash;//8. Prepend the same random phrase from step 2 to your code
			if(isQueryStr) {//9. If the SingleSignOnCode is going to be passed in a query string, be sure to UrlEncode the entire code
				singleSignOnCode=WebUtility.UrlEncode(singleSignOnCode);
			}
			return singleSignOnCode;
		}

		///<summary>The comments on each line come directly from the DoseSpot API Guide 12.8.pdf.  Creates a Single Sign On User ID Verify for DoseSpot.</summary>
		public static string CreateSsoUserIdVerify(string clinicKey,string userID,bool isQueryStr=true) {
			//No need to check RemotingRole; no call to db.
			string singleSignOnCode="";
			string phrase=Get32CharPhrase();
			string idPhraseAndKey=phrase.Substring(0,22);//1. Grab the first 22 characters of the phrase used in CreateSSOCode from step 1 of CreateSsoCode
			idPhraseAndKey=userID+idPhraseAndKey;//2. Append to the UserID string the 22 characters grabbed from step one
			idPhraseAndKey+=clinicKey;//3. Append the key to the string created in 2b
			byte[] arrayBytes=GetBytesFromUTF8(idPhraseAndKey);//4. Get the Byte value of the string
			byte[] arrayHashedBytes=GetSHA512Hash(arrayBytes);//5. Use SHA512 to hash the byte value you just received
			string base64hash=Convert.ToBase64String(arrayHashedBytes);//6. Get a Base64String out of the hash that you created
			singleSignOnCode=RemoveExtraEqualSigns(base64hash);//7. If there are two = signs at the end, then remove them
			if(isQueryStr) {//8. If the SingleSignOnUserIdVerify is going to be passed in a query string, be sure to UrlEncode the entire code
				singleSignOnCode=WebUtility.UrlEncode(singleSignOnCode);
			}
			_randomPhrase32=null;
			return singleSignOnCode;
		}

		///<summary>Can throw exceptions.  Returns true if changes were made to medications.</summary>
		public static bool SyncPrescriptionsFromDoseSpot(string clinicID,string clinicKey,string userID,long patNum,Action<List<RxPat>> onRxAdd=null) {
			//No need to check RemotingRole; no call to db.
			OIDExternal oIDExternal=DoseSpot.GetDoseSpotPatID(patNum);
			if(oIDExternal==null) {
				return false;//We don't have a PatID from DoseSpot for this patient.  Therefore there is nothing to sync with.
			}
			Patient patient=Patients.GetPat(patNum);
			List<long> listMedicationPatNumsActive=new List<long>();
			Dictionary<int,string> dictPharmacyIdToPharmacyName=new Dictionary<int,string>();
			List<RxPat> listRxPats=new List<RxPat>();
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			//Get rid of any deleted prescriptions.
			List<DoseSpotPrescription> listDoseSpotPerscriptions=DoseSpotREST.GetPrescriptions(token,oIDExternal.IDExternal);
			List<DoseSpotMedicationWrapper> listDoseSpotMedicationWrappers=listDoseSpotPerscriptions.Select(x => new DoseSpotMedicationWrapper(x,null)).ToList();
			//Add self reported medications.
			listDoseSpotMedicationWrappers.AddRange(DoseSpotREST.GetSelfReported(token,oIDExternal.IDExternal).Select(x => new DoseSpotMedicationWrapper(null,x)).ToList());
			listDoseSpotMedicationWrappers=listDoseSpotMedicationWrappers.FindAll(x => x.MedicationStatus!=DoseSpotREST.MedicationStatus.Deleted);
			foreach(DoseSpotMedicationWrapper doseSpotMedicationWrapper in listDoseSpotMedicationWrappers) {
				RxPat rxPatOld=null;
				if(doseSpotMedicationWrapper.IsSelfReported) {
					//Get self reported that originated in OD
					rxPatOld=RxPats.GetErxByIdForPat(Erx.OpenDentalErxPrefix+doseSpotMedicationWrapper.MedicationId.ToString(),patNum);
					if(rxPatOld==null) {
						//Get self reported that originated in DS
						rxPatOld=RxPats.GetErxByIdForPat(Erx.DoseSpotPatReportedPrefix+doseSpotMedicationWrapper.MedicationId.ToString(),patNum);
					}
				}
				else {
					//Isn't self reported, Guid won't have a prefix.
					rxPatOld=RxPats.GetErxByIdForPat(doseSpotMedicationWrapper.MedicationId.ToString(),patNum);
				}
				RxPat rxPat=new RxPat();
				long rxCui=doseSpotMedicationWrapper.RxCUI;//If this is zero either DoseSpot didn't send the value or there was an issue casting from string to long.
				rxPat.IsControlled=(PIn.Int(doseSpotMedicationWrapper.Schedule)!=0);//Controlled if Schedule is I,II,III,IV,V
				rxPat.DosageCode="";
				rxPat.SendStatus=RxSendStatus.Unsent;
				switch(doseSpotMedicationWrapper.PrescriptionStatus) {
					case DoseSpotREST.PrescriptionStatus.PharmacyVerified:
					case DoseSpotREST.PrescriptionStatus.eRxSent:
						rxPat.SendStatus=RxSendStatus.SentElect;
						break;
					case DoseSpotREST.PrescriptionStatus.FaxSent:
						rxPat.SendStatus=RxSendStatus.Faxed;
						break;
					case DoseSpotREST.PrescriptionStatus.Printed:
						rxPat.SendStatus=RxSendStatus.Printed;
						break;
					case DoseSpotREST.PrescriptionStatus.Sending:
						rxPat.SendStatus=RxSendStatus.Pending;
						break;
					case DoseSpotREST.PrescriptionStatus.Deleted:
					case DoseSpotREST.PrescriptionStatus.Error:
					case DoseSpotREST.PrescriptionStatus.EpcsError:
						continue;//Skip these medications since DoseSpot is saying that they are invalid
					case DoseSpotREST.PrescriptionStatus.Edited:
					case DoseSpotREST.PrescriptionStatus.Entered:
					case DoseSpotREST.PrescriptionStatus.EpcsSigned:
					case DoseSpotREST.PrescriptionStatus.ReadyToSign:
					case DoseSpotREST.PrescriptionStatus.Requested:
					default:
						rxPat.SendStatus=RxSendStatus.Unsent;
						break;
				}
				rxPat.Refills=doseSpotMedicationWrapper.Refills;
				rxPat.Disp=doseSpotMedicationWrapper.Quantity;//In DoseSpot, the Quanitity textbox's label says "Dispense".
				rxPat.Drug=doseSpotMedicationWrapper.DisplayName;
				if(doseSpotMedicationWrapper.PharmacyId.HasValue) {
					try {
						if(!dictPharmacyIdToPharmacyName.ContainsKey(doseSpotMedicationWrapper.PharmacyId.Value)) {
							dictPharmacyIdToPharmacyName.Add(doseSpotMedicationWrapper.PharmacyId.Value,DoseSpotREST.GetPharmacyName(token,doseSpotMedicationWrapper.PharmacyId.Value));
						}
						rxPat.ErxPharmacyInfo=dictPharmacyIdToPharmacyName[doseSpotMedicationWrapper.PharmacyId.Value];
					}
					catch(Exception ex) {
						ex.DoNothing();
						//Do nothing.  It was a nicety anyways.
					}
				}
				rxPat.PatNum=patNum;
				rxPat.Sig=doseSpotMedicationWrapper.Directions;
				rxPat.Notes=doseSpotMedicationWrapper.RxNotes;
				rxPat.RxDate=DateTime.MinValue;
				//If none of dates have values, the RxDate will be MinValue.
				//This is acceptable if DoseSpot doesn't give us anything, which should never happen.
				if(doseSpotMedicationWrapper.DateWritten.HasValue) {
					rxPat.RxDate=doseSpotMedicationWrapper.DateWritten.Value;
				}
				else if(doseSpotMedicationWrapper.DateReported.HasValue) {
					rxPat.RxDate=doseSpotMedicationWrapper.DateReported.Value;
				}
				else if(doseSpotMedicationWrapper.DateLastFilled.HasValue) {
					rxPat.RxDate=doseSpotMedicationWrapper.DateLastFilled.Value;
				}
				else if(doseSpotMedicationWrapper.DateInactive.HasValue) {
					rxPat.RxDate=doseSpotMedicationWrapper.DateInactive.Value;
				}
				//Save DoseSpot's unique ID into our rx
				int doseSpotMedId=(int?)doseSpotMedicationWrapper.MedicationId??0;//If this changes, we need to ensure that Erx.IsFromDoseSpot() is updated to match.
				rxPat.ErxGuid=doseSpotMedId.ToString();
				bool isProvider=false;
				if(doseSpotMedicationWrapper.IsSelfReported) {//Self Reported medications won't have a prescriber number
					if(rxPatOld==null) {//Rx doesn't exist in the database.  This probably originated from DoseSpot
						MedicationPat medicationPat=MedicationPats.GetMedicationOrderByErxIdAndPat(Erx.OpenDentalErxPrefix+doseSpotMedicationWrapper.MedicationId.ToString(),patNum);
						if(medicationPat==null) {//If there isn't a record of the medication 
							medicationPat=MedicationPats.GetMedicationOrderByErxIdAndPat(Erx.DoseSpotPatReportedPrefix+doseSpotMedicationWrapper.MedicationId.ToString(),patNum);
						}
						if(medicationPat==null) {//If medPat is null at this point we don't have a record of this patient having the medication, so it probably was just made in DoseSpot.
							rxPat.ErxGuid=Erx.DoseSpotPatReportedPrefix+doseSpotMedicationWrapper.MedicationId;
						}
						else {
							rxPat.ErxGuid=medicationPat.ErxGuid;//Maintain the ErxGuid that was assigned for the MedicationPat that already exists.
						}
					}
					else {
						rxPat.ErxGuid=rxPatOld.ErxGuid;//Maintain the ErxGuid that was already assigned for the Rx.
					}
				}
				else {
					//The prescriber ID for each medication is the doctor that approved the prescription.
					UserOdPref userOdPref=UserOdPrefs.GetByFkeyAndFkeyType(Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program)
					.FirstOrDefault(x => x.ValueString==doseSpotMedicationWrapper.PrescriberId.ToString());
					if(userOdPref==null) {//The Dose Spot User ID from this medication is not present in Open Dental.
						continue;//I don't know if we want to do anything with this.  Maybe we want to just get the ErxLog from before this medication was made.
					}
					Userod userod=Userods.GetUser(userOdPref.UserNum);
					Provider provider=new Provider();
					isProvider=!Erx.IsUserAnEmployee(userod);
					if(isProvider) {//A user always be a provider if there is a ProvNum > 0
						provider=Providers.GetProv(userod.ProvNum);
					}
					else {
						provider=Providers.GetProv(patient.PriProv);
					}
					rxPat.ProvNum=provider.ProvNum;
				}
				//These fields are possibly set above, preserve old values if they are not.
				if(rxPatOld!=null) {
					rxPat.Disp=rxPatOld.Disp; //The medication Disp currently always returns 0. Preserve the old value.
					rxPat.Refills=rxPat.Refills==null ? rxPatOld.Refills : rxPat.Refills;
					rxPat.Notes=rxPatOld.Notes.IsNullOrEmpty() ? rxPat.Notes : rxPatOld.Notes;//Preserve the note already in OD no matter what if there is one.
					rxPat.PatientInstruction=rxPat.PatientInstruction.IsNullOrEmpty() ? rxPatOld.PatientInstruction : rxPat.PatientInstruction;
					rxPat.ErxPharmacyInfo=rxPat.ErxPharmacyInfo==null ? rxPatOld.ErxPharmacyInfo : rxPat.ErxPharmacyInfo;
					rxPat.IsControlled=rxPatOld.IsControlled;
					if(rxPatOld.RxDate.Year>1880) {
						rxPat.RxDate=rxPatOld.RxDate;
					}
				}
				long medicationPatNum=0;
				if(Erx.IsDoseSpotPatReported(rxPat.ErxGuid) || Erx.IsTwoWayIntegrated(rxPat.ErxGuid)) {//For DoseSpot self reported, do not insert a prescription.
					medicationPatNum=Erx.InsertOrUpdateErxMedication(rxPatOld,rxPat,rxCui,doseSpotMedicationWrapper.DisplayName,doseSpotMedicationWrapper.GenericProductName,isProvider,false);
				}
				else {
					medicationPatNum=Erx.InsertOrUpdateErxMedication(rxPatOld,rxPat,rxCui,doseSpotMedicationWrapper.DisplayName,doseSpotMedicationWrapper.GenericProductName,isProvider);
				}
				if(rxPatOld==null) {//Only add the rx if it is new.  We don't want to trigger automation for existing prescriptions.
					listRxPats.Add(rxPat);
				}
				if(doseSpotMedicationWrapper.MedicationStatus==DoseSpotREST.MedicationStatus.Active) {
					listMedicationPatNumsActive.Add(medicationPatNum);
				}
			}
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patNum,false);
			for(int i=0;i<listMedicationPats.Count;i++) {
			//This loop should update the end date for: Perscriptions made in DoseSpot, Medications made in DoseSpot, and Medications made in OD
				string eRxGuidCur=listMedicationPats[i].ErxGuid;
				if(!Erx.IsFromDoseSpot(eRxGuidCur) && !Erx.IsDoseSpotPatReported(eRxGuidCur) && !Erx.IsTwoWayIntegrated(eRxGuidCur))
				{
					continue;//This medication is not synced with DoseSpot, don't update.
				}
				if(listMedicationPatNumsActive.Contains(listMedicationPats[i].MedicationPatNum)) {
					continue;//The medication is still active.
				}
				if(listMedicationPats[i].DateStop.Year>1880) {
					continue;//The medication is already discontinued in Open Dental.
				}
				//The medication was discontinued inside the eRx interface.
				DoseSpotMedicationWrapper doseSpotMedicationWrapper=listDoseSpotMedicationWrappers.FirstOrDefault(x => Erx.OpenDentalErxPrefix+x.MedicationId.ToString()==eRxGuidCur);
				if(doseSpotMedicationWrapper==null) {
					doseSpotMedicationWrapper=listDoseSpotMedicationWrappers.FirstOrDefault(x => Erx.DoseSpotPatReportedPrefix+x.MedicationId.ToString()==eRxGuidCur);
				}
				if(doseSpotMedicationWrapper==null) {
					//We don't have a medication from DS for this medicationpat, kick out.
					continue;
				}
				//Try to get the date stop.
				if(doseSpotMedicationWrapper.DateInactive!=null) {
					listMedicationPats[i].DateStop=doseSpotMedicationWrapper.DateInactive.Value;
				}
				else {
					listMedicationPats[i].DateStop=DateTime.Today.AddDays(-1);//Discontinue the medication as of yesterday so that it will immediately show as discontinued.
				}
				MedicationPats.Update(listMedicationPats[i]);//Discontinue the medication inside OD to match what shows in the eRx interface.
				string medDescript=listMedicationPats[i].MedDescript;
				if(listMedicationPats[i].MedicationNum!=0) {
					medDescript=Medications.GetMedication(listMedicationPats[i].MedicationNum).MedName;
				}
				SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,listMedicationPats[i].PatNum,medDescript+" DoseSpot set inactive",LogSources.eRx);
			}
			if(onRxAdd!=null && listRxPats.Count!=0) {
				onRxAdd(listRxPats);
			}
			return true;
		}

		///<summary></summary>
		public static void SyncPrescriptionsToDoseSpot(string clinicID,string clinicKey,string userID,long patNum) {
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			//No need to check RemotingRole; no call to db.
			OIDExternal oIDExternal=DoseSpot.GetDoseSpotPatID(patNum);
			if(oIDExternal==null) {
				return;//We don't have a PatID from DoseSpot for this patient.  Therefore there is nothing to sync with.
			}
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patNum,true).FindAll(x => !Erx.IsFromDoseSpot(x.ErxGuid));
			if(listMedicationPats.Count==0) {
				return;//There are no medications to send to DoseSpot.
			}
			foreach(MedicationPat medicationPat in listMedicationPats) {
				//Medications originating from DS are filtered out when the list is retrieved.
				DoseSpotSelfReported doseSpotSelfReported=DoseSpotREST.MedicationPatToDoseSpotSelfReport(medicationPat);
				if(doseSpotSelfReported.DisplayName.IsNullOrEmpty()) {
					//Couldn't get a name from the medicationpat or the medication, don't send to DS without a name.
					continue;
				}
				if(doseSpotSelfReported.SelfReportedMedicationId>0) {
					//We were able to get the external ID for this self reported from the medicationpat, therefore we want to do an edit.
					try {
						DoseSpotREST.PutSelfReportedMedications(token,oIDExternal.IDExternal,doseSpotSelfReported);
					}
					catch(ODException e) {
						if(e.Message.Contains("does not belong to PatientID")) {
							//skip these, get past a bug in the DS API where it returns medications for the wrong patient.
							continue;
						}
						else {
							throw e;
						}
					}
				}
				else {
					//Save the ID returned by DoseSpot.
					RxPat rxPat=null;
					if(medicationPat.ErxGuid.StartsWith(Erx.UnsentPrefix)) {
						rxPat=RxPats.GetErxByIdForPat(medicationPat.ErxGuid,medicationPat.PatNum);
					}
					medicationPat.ErxGuid=Erx.OpenDentalErxPrefix+DoseSpotREST.PostSelfReportedMedications(token,oIDExternal.IDExternal,doseSpotSelfReported);
					if(rxPat!=null) {
						rxPat.ErxGuid=medicationPat.ErxGuid;
						RxPats.Update(rxPat);
					}
				}
				//Update the medPat to store an ErxGuid and the returned ID from DoseSpot so that we don't keep sending this unnecessarily.
				MedicationPats.Update(medicationPat);
			}
		}

		///<summary>Returns true if at least one of the counts is greater than 0.
		///This is used for notifying prescribers when they need to take action is DoseSpot.</summary>
		public static bool GetPrescriberNotificationCounts(string clinicID,string clinicKey,string userID
			,out int countRefillReqs,out int countTransactionErrors,out int countPendingPrescriptionsCount)
		{
			//No need to check RemotingRole; no call to db.
			countRefillReqs=0;
			countTransactionErrors=0;
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			DoseSpotREST.GetNotificationCounts(token,out countRefillReqs,out countTransactionErrors,out countPendingPrescriptionsCount);
			#region SOAP - Deprecated
			//DoseSpotService.API api=new DoseSpotService.API();
			//			if(ODBuild.IsDebug()) {
			//				api.Url="https://my.staging.dosespot.com/api/12/api.asmx?wsdl";
			//			}
			//			DoseSpotService.GetPrescriberNotificationCountsRequest req=new DoseSpotService.GetPrescriberNotificationCountsRequest();
			//			req.SingleSignOn=GetSingleSignOn(clinicID,clinicKey,userID,false);
			//			DoseSpotService.GetPrescriberNotificationCountsResponse res=api.GetPrescriberNotificationCounts(req);
			//			if(res.Result!=null && res.Result.ResultCode.ToLower()=="error") {
			//				ODException.ErrorCodes errorCode=ODException.ErrorCodes.NotDefined;
			//				if(Erx.IsUserAnEmployee(Security.CurUser) && res.Result.ResultDescription.Contains("not authorized") || res.Result.ResultDescription.Contains("unauthorized")) {
			//					errorCode=ODException.ErrorCodes.DoseSpotNotAuthorized;
			//				}
			//				throw new ODException(res.Result.ResultDescription,errorCode);
			//			}
			//			countRefillReqs=res.RefillRequestsCount;
			//			countTransactionErrors=res.TransactionErrorsCount;
			//			countPendingPrescriptionsCount=res.PendingPrescriptionsCount;
			#endregion
			return (countRefillReqs>0 || countTransactionErrors>0 || countPendingPrescriptionsCount>0);//Return true if there is anything that the prescriber needs to see/take action on.
		}

		///<summary>If pat is null, it is assumed that we are making a SSO request for errors and refill requests.
		///Throws exceptions for invalid Patient data.</summary>
		public static string GetSingleSignOnQueryString(string clinicID,string clinicKey,string userID,string onBehalfOfUserId,Patient patient) {
			//No need to check RemotingRole; no call to db.
			//Pass in false for isQueryString because we will URLEncode the values below in QueryStringAddParameter.  It was a bug where we were double encoding the data.
			DoseSpotService.SingleSignOn singleSignOn=GetSingleSignOn(clinicID,clinicKey,userID,false);
			StringBuilder stringBuilder=new StringBuilder();
			QueryStringAddParameter(stringBuilder,"SingleSignOnCode",singleSignOn.SingleSignOnCode);
			QueryStringAddParameter(stringBuilder,"SingleSignOnUserId",POut.Int(singleSignOn.SingleSignOnUserId));
			QueryStringAddParameter(stringBuilder,"SingleSignOnUserIdVerify",singleSignOn.SingleSignOnUserIdVerify);
			QueryStringAddParameter(stringBuilder,"SingleSignOnClinicId",POut.Int(singleSignOn.SingleSignOnClinicId));
			if(!String.IsNullOrWhiteSpace(onBehalfOfUserId)) {
				QueryStringAddParameter(stringBuilder,"OnBehalfOfUserId",POut.String(onBehalfOfUserId));
			}
			if(patient==null) {
				QueryStringAddParameter(stringBuilder,"RefillsErrors",POut.Int(1));//Request transmission errors
			}
			else {
				OIDExternal oIDExternal=DoseSpot.GetDoseSpotPatID(patient.PatNum);
				if(oIDExternal!=null) {
					QueryStringAddParameter(stringBuilder,"PatientId",oIDExternal.IDExternal);
				}
			}
			return stringBuilder.ToString();
		}

		///<summary>Throws exceptions.
		///Gets the clinicID/clinicKey regarding the passed in clinicNum.
		///Will register the passed in clinicNum with DoseSpot if it isn't already.
		///Validates if the passed in doseSpotUserID is empty.</summary>
		public static void GetClinicIdAndKey(long clinicNum,string doseSpotUserID,Program program,List<ProgramProperty> listProgramProperties
			,out string clinicID,out string clinicKey)
		{
			//No need to check RemotingRole; no call to db.
			clinicID="";
			clinicKey="";
			if(program==null) {
				program=Programs.GetCur(ProgramName.eRx);
			}
			if(listProgramProperties==null) {
				listProgramProperties=ProgramProperties.GetForProgram(program.ProgramNum)
					.FindAll(x => x.ClinicNum==clinicNum
						&& (x.PropertyDesc==Erx.PropertyDescs.ClinicID || x.PropertyDesc==Erx.PropertyDescs.ClinicKey));
			}
			ProgramProperty programPropertyClinicID=listProgramProperties.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			ProgramProperty programPropertyClinicKey=listProgramProperties.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			//If the current clinic doesn't have a clinic id/key, use a different clinic to make them.
			if(programPropertyClinicID==null || string.IsNullOrWhiteSpace(programPropertyClinicID.PropertyValue)
				|| programPropertyClinicKey==null || string.IsNullOrWhiteSpace(programPropertyClinicKey.PropertyValue))
			{
				throw new ODException(((clinicNum==0)?"HQ ":Clinics.GetAbbr(clinicNum)+" ")
					+Lans.g("DoseSpot","is missing a valid ClinicID or Clinic Key.  This should have been entered when setting up DoseSpot."));
			}
			else {
				clinicID=programPropertyClinicID.PropertyValue;
				clinicKey=programPropertyClinicKey.PropertyValue;
			}
		}

		///<summary>Throws Exceptions if clinic is deemed invalid for DoseSpot.
		///Register the passed in clinicNum in DoseSpot, and pass out the registered ID/key.
		///Will save the new clinic id/clinic key into program properties for the given clinic and refreshes cache.</summary>
		public static void RegisterClinic(long clinicNum,string clinicID,string clinicKey,string userID
			,out string clinicIdNew,out string clinicKeyNew) 
		{
			//No need to check RemotingRole; no call to db.
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			Clinic clinic=GetClinicOrPracticeInfo(clinicNum);
			#region SOAP - Deprecated
			//			DoseSpotService.API api=new DoseSpotService.API();
			//			DoseSpotService.ClinicAddMessage req=new DoseSpotService.ClinicAddMessage();
			//			req.Clinic=MakeDoseSpotClinic(clinicCur);
			//			req.SingleSignOn=GetSingleSignOn(clinicID,clinicKey,userID,false);
			//			if(ODBuild.IsDebug()) {
			//				//This code will output the XML into the console.  This may be needed for DoseSpot when troubleshooting issues.
			//				//This XML will be the soap body and exclude the header and envelope.
			//				System.Xml.Serialization.XmlSerializer xml=new System.Xml.Serialization.XmlSerializer(req.GetType());
			//				xml.Serialize(Console.Out,req);
			//			}
			//DoseSpotService.ClinicAddResultMessage res=api.ClinicAdd(req);
			//if(res.Result!=null && res.Result.ResultCode.ToLower().Contains("error")) {
			//	throw new Exception(res.Result.ResultDescription);
			//}
			#endregion
			DoseSpotREST.PostClinic(token,clinic,out clinicIdNew,out clinicKeyNew);
			long programNumErx=Programs.GetCur(ProgramName.eRx).ProgramNum;
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetListForProgramAndClinic(programNumErx,clinicNum);
			ProgramProperty programPropertyClinicID=listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			ProgramProperty programPropertyClinicKey=listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			//Update the database with the new id/key.
			InsertOrUpdate(programPropertyClinicID,programNumErx,Erx.PropertyDescs.ClinicID,clinicIdNew,clinicNum);
			InsertOrUpdate(programPropertyClinicKey,programNumErx,Erx.PropertyDescs.ClinicKey,clinicKeyNew,clinicNum);
			//Ensure cache is not stale after setting the values.
			Cache.Refresh(InvalidType.Programs);
		}

		///<summary>Throws exceptions when validating clinic/practice info/provider.
		///Updates the passed in user's UserOdPref for DoseSpot User ID</summary>
		public static string GetUserID(Userod userod,long clinicNum) {
			//No need to check RemotingRole; no call to db.
			string doseSpotUserID="";
			Clinic clinic=GetClinicOrPracticeInfo(clinicNum);
			//At this point we know that we have a valid clinic/practice info and valid provider.
			Program program=Programs.GetCur(ProgramName.eRx);
			//Get the DoseSpotID for the current user
			UserOdPref userOdPref=GetDoseSpotUserIdFromPref(userod.UserNum,clinicNum);
			//If the current user doesn't have a valid User ID, go retreive one from DoseSpot.
			if(userOdPref==null || string.IsNullOrWhiteSpace(userOdPref.ValueString)) {
				//If there is no UserId for this user, throw an exception.  The below code was when we thought the Podio database matched the DoseSpot database.
				//The below code would add a proxy clinician via the API and give back the DoseSpot User ID.
				//This was causing issues with Podio and making sure the proxy clinician has access to the appropriate clinics.
				throw new ODException("Missing DoseSpot User ID for user.  Call support to register provider or proxy user, then enter User ID into security window.");
				#region Old Proxy User Registration
				//        UserOdPref otherRegisteredClinician=UserOdPrefs.GetAllByFkeyAndFkeyType(programErx.ProgramNum,UserOdFkeyType.Program)
				//          .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.ValueString) && Userods.GetUser(x.UserNum).ProvNum!=0);
				//        //userCur.ProvNum==0 means that this is a real clinician.  
				//        //We can add proxy clinicians for no charge, but actual clinicians will incur a fee.
				//        if(!Erx.IsUserAnEmployee(userCur) || otherRegisteredClinician==null) {
				//          //Either the prov isn't registered, or there are no credentials to create the proxy clinician.
				//          //Either way, we want the user to know they need to register the provider.
				//          throw new ODException("Missing DoseSpot User ID for provider.  Call support to register provider, then enter User ID into security window.");
				//        }
				//        //Get the provider from the doseSpotUserID we are using.  This ensures that DoseSpot knows the provider is valid,
				//        //instead of passing in the patient's primary provider, which may not be registered in DoseSpot.
				//        Provider provOther=Providers.GetProv(Userods.GetUser(otherRegisteredClinician.UserNum).ProvNum);
				//				ValidateProvider(provOther,clinicNum);
				//				string defaultDoseSpotUserID=otherRegisteredClinician.ValueString;
				//				string clinicID="";
				//				string clinicKey="";
				//				GetClinicIdAndKey(clinicNum,defaultDoseSpotUserID,programErx,null,out clinicID,out clinicKey);
				//				DoseSpotService.API api=new DoseSpotService.API();
				//				DoseSpotService.ClinicianAddMessage req=new DoseSpotService.ClinicianAddMessage();
				//				req.SingleSignOn=GetSingleSignOn(clinicID,clinicKey,defaultDoseSpotUserID,false);
				//				EmailAddress email=EmailAddresses.GetForUser(userCur.UserNum);
				//				if(email==null || string.IsNullOrWhiteSpace(email.EmailUsername)) {
				//					throw new ODException("Invalid email address for the current user.");
				//				}
				//				req.Clinician=MakeDoseSpotClinician(provOther,clinicCur,email.EmailUsername,true);//If the user isn't a provider, they are a proxy clinician.
				//				if(ODBuild.IsDebug()) {
				//					//This code will output the XML into the console.  This may be needed for DoseSpot when troubleshooting issues.
				//					//This XML will be the soap body and exclude the header and envelope.
				//					System.Xml.Serialization.XmlSerializer xml=new System.Xml.Serialization.XmlSerializer(req.GetType());
				//					xml.Serialize(Console.Out,req);
				//				}
				//				DoseSpotService.ClinicianAddResultsMessage res=api.ClinicianAdd(req);
				//				if(res.Result!=null && (res.Result.ResultCode.ToLower().Contains("error") || res.Clinician==null)) {
				//					throw new Exception(res.Result.ResultDescription);
				//				}
				//				retVal=res.Clinician.ClinicianId.ToString();
				//				//Since userPrefDoseSpotID can't be null, we just overwrite all of the fields to be sure that they are correct.
				//				userPrefDoseSpotID.UserNum=userCur.UserNum;
				//				userPrefDoseSpotID.Fkey=programErx.ProgramNum;
				//				userPrefDoseSpotID.FkeyType=UserOdFkeyType.Program;
				//				userPrefDoseSpotID.ValueString=retVal;
				//				if(userPrefDoseSpotID.IsNew) {
				//					UserOdPrefs.Insert(userPrefDoseSpotID);
				//				}
				//				else {
				//					UserOdPrefs.Update(userPrefDoseSpotID);
				//				}
				#endregion
			}
			else {
				doseSpotUserID=userOdPref.ValueString;
			}
			return doseSpotUserID;
		}

		///<summary>Parses the passed in queryString for the value of the passed in parameter name.
		///Returns null if a match isn't found.</summary>
		public static string GetQueryParameterFromQueryString(string queryString,string parameterNameToReturn) {
			//No need to check RemotingRole; no call to db.
			if(!string.IsNullOrWhiteSpace(parameterNameToReturn)) {
				if(queryString[0]=='?') {
					queryString=queryString.Substring(1);
				}
				string[] arrayQueries=queryString.Split('&');
				foreach(string queryCur in arrayQueries) {
					if(!String.IsNullOrWhiteSpace(queryCur) && queryCur.Contains("=")) {
						string[] arrayQueryCur=queryCur.Split('=');
						string paramNameCur=arrayQueryCur[0];
						if(paramNameCur!=null && paramNameCur.ToUpper().Equals(parameterNameToReturn.ToUpper())) {
							return arrayQueryCur[1];
						}
					}
				}
			}
			return null;
		}

		///<summary>Returns true if changes were made and cache needs to be refreshed.</summary>
		public static bool SyncClinicErxsWithHQ() {
			//No need to check RemotingRole; no call to db.
			MakeClinicErxsForDoseSpot();
			List<ClinicErx> listClinicErxs=ClinicErxs.GetWhere(x => x.EnabledStatus!=ErxStatus.Enabled);
			//Currently we do not have any intention of disabling clinics from HQ since there is no cost associated to adding a clinic.
			//Because of this, don't make extra web calls to check if HQ has tried to disable any clinics.
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteStartElement("ErxClinicAccessRequest");
				xmlWriter.WriteStartElement("RegistrationKey");
				xmlWriter.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				xmlWriter.WriteEndElement();//End reg key
				xmlWriter.WriteStartElement("RegKeyDisabledOverride");
				//Allow disabled regkeys to use eRx.  This functionality matches how we handle a disabled regkey for providererx
				//providererx in CustUpdates only cares that the regkey is valid and associated to a patnum in ODHQ
				xmlWriter.WriteString("true");
				xmlWriter.WriteEndElement();//End reg key disabled override
				foreach(ClinicErx clinicErx in listClinicErxs) {
					xmlWriter.WriteStartElement("Clinic");
					xmlWriter.WriteAttributeString("ClinicDesc",clinicErx.ClinicDesc);
					xmlWriter.WriteAttributeString("EnabledStatus",((int)clinicErx.EnabledStatus).ToString());
					xmlWriter.WriteAttributeString("ClinicId",clinicErx.ClinicId);
					xmlWriter.WriteAttributeString("ClinicKey",clinicErx.ClinicKey);
					xmlWriter.WriteEndElement();//End Clinic
				}
				xmlWriter.WriteEndElement();//End ErxAccessRequest
			}
#if DEBUG
			OpenDentBusiness.localhost.Service1 service1=new OpenDentBusiness.localhost.Service1();

#else
			OpenDentBusiness.customerUpdates.Service1 service1=new OpenDentBusiness.customerUpdates.Service1();
			service1.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			bool isCacheRefreshNeeded=false;
			try {
				string result=service1.GetClinicErxAccess(stringBuilder.ToString());
				XmlDocument xmlDocument=new XmlDocument();
				xmlDocument.LoadXml(result);
				XmlNodeList xmlNodeList=xmlDocument.SelectNodes("//Clinic");
				for(int i=0;i<xmlNodeList.Count;i++) {//Loop through clinics.
					XmlNode xmlNode=xmlNodeList[i];
					string clinicDesc="";
					string clinicId="";
					string clinicKey="";
					ErxStatus erxStatus=ErxStatus.Disabled;
					for(int j=0;j<xmlNode.Attributes.Count;j++) {//Loop through the attributes for the current provider.
						XmlAttribute xmlAttribute=xmlNode.Attributes[j];
						if(xmlAttribute.Name=="ClinicDesc") {
							clinicDesc=xmlAttribute.Value;
						}
						else if(xmlAttribute.Name=="EnabledStatus") {
							erxStatus=PIn.Enum<ErxStatus>(PIn.Int(xmlAttribute.Value));
						}
						else if(xmlAttribute.Name=="ClinicId") {
							clinicId=xmlAttribute.Value;
						}
						else if(xmlAttribute.Name=="ClinicKey") {
							clinicKey=xmlAttribute.Value;
						}
					}
					ClinicErx clinicErxOld=ClinicErxs.GetByClinicIdAndKey(clinicId,clinicKey);
					if(clinicErxOld==null) {
						continue;
					}
					ClinicErx clinicErx=clinicErxOld.Copy();
					clinicErx.EnabledStatus=erxStatus;
					clinicErx.ClinicId=clinicId;
					clinicErx.ClinicKey=clinicKey;
					//Dont need to set the ErxType here because it's not something that can be changed by HQ.
					if(ClinicErxs.Update(clinicErx,clinicErxOld)) {
						isCacheRefreshNeeded=true;
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				//Failed to contact server and/or update clinicerx row at ODHQ. We will simply use what we already know in the local database.
			}
			return isCacheRefreshNeeded;
		}

		public static UserOdPref GetDoseSpotUserIdFromPref(long userNum,long clinicNum) {
			Program program=Programs.GetCur(ProgramName.eRx);
			UserOdPref userOdPref=UserOdPrefs.GetByCompositeKey(userNum,program.ProgramNum,UserOdFkeyType.Program,clinicNum);
			if(clinicNum!=0 && userOdPref.IsNew || string.IsNullOrWhiteSpace(userOdPref.ValueString)) {
				userOdPref=UserOdPrefs.GetByCompositeKey(userNum,program.ProgramNum,UserOdFkeyType.Program,0);//Try the default userodpref if the clinic specific one is empty.
			}
			return userOdPref;
		}
		
		public static OIDExternal CreateOIDForPatient(int doseSpotPatID,long patNum) {
			OIDExternal oIDExternal=new OIDExternal();
			oIDExternal.rootExternal=DoseSpot.GetDoseSpotRoot()+"."+POut.Int((int)IdentifierType.Patient);
			oIDExternal.IDExternal=doseSpotPatID.ToString();
			oIDExternal.IDInternal=patNum;
			oIDExternal.IDType=IdentifierType.Patient;
			OIDExternals.Insert(oIDExternal);
			return oIDExternal;
		}

		///<summary>Creates ClinicErx entries for every clinic that has DoseSpot ClinicID/ClinicKey values.
		///This will be used when sending the clinics to ODHQ for enabling/disabling.
		///This will create ClinicNum 0 entries, thus supporting offices without clinics enabled, as well as clinics using the "Headquarters" clinic.</summary>
		private static void MakeClinicErxsForDoseSpot() {
			long programNum=Programs.GetCur(ProgramName.eRx).ProgramNum;
			List<ProgramProperty> listProgramPropertiesForClinicID=ProgramProperties.GetWhere(x => x.ProgramNum==programNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			List<ProgramProperty> listProgramPropertiesForClinicKey=ProgramProperties.GetWhere(x => x.ProgramNum==programNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			bool isRefreshNeeded=false;
			foreach(ProgramProperty programProperty in listProgramPropertiesForClinicID) {
				ProgramProperty programPropertyClinicKey=listProgramPropertiesForClinicKey.FirstOrDefault(x => x.ClinicNum==programProperty.ClinicNum);
				if(programPropertyClinicKey==null || string.IsNullOrWhiteSpace(programPropertyClinicKey.PropertyValue) || string.IsNullOrWhiteSpace(programProperty.PropertyValue)) {
					continue;
				}
				ClinicErx clinicErx=ClinicErxs.GetByClinicNum(programProperty.ClinicNum);
				if(clinicErx==null) {
					clinicErx=new ClinicErx();
					clinicErx.ClinicNum=programProperty.ClinicNum;
					clinicErx.ClinicId=programProperty.PropertyValue;
					clinicErx.ClinicKey=programPropertyClinicKey.PropertyValue;
					clinicErx.ClinicDesc=Clinics.GetDesc(programProperty.ClinicNum);
					clinicErx.EnabledStatus=ErxStatus.PendingAccountId;
					ClinicErxs.Insert(clinicErx);
				}
				else {
					clinicErx.ClinicId=programProperty.PropertyValue;
					clinicErx.ClinicKey=programPropertyClinicKey.PropertyValue;
					ClinicErxs.Update(clinicErx);
				}
				isRefreshNeeded=true;
			}
			if(isRefreshNeeded) {
				Cache.Refresh(InvalidType.ClinicErxs);
			}
		}

		///<summary>Throws exceptions if any of the clinic/practice info is invalid.
		///Ensures that the Clinic that is returned is a valid clinic that can turned 
		/// into a DoseSpot Clinic to be used for DoseSpot API calls that require a Clinic (like registering a new clinic). </summary>
		private static Clinic GetClinicOrPracticeInfo(long clinicNum) {
			Clinic clinic=Clinics.GetClinic(clinicNum);
			bool isPractice=false;
			if(clinic==null) {//Make a fake ClinicNum 0 clinic containing practice info for validation/registering a new clinician if needed.
				clinic=new Clinic();
				clinic.Abbr=PrefC.GetString(PrefName.PracticeTitle);
				clinic.Address=PrefC.GetString(PrefName.PracticeAddress);
				clinic.Address2=PrefC.GetString(PrefName.PracticeAddress2);
				clinic.City=PrefC.GetString(PrefName.PracticeCity);
				clinic.State=PrefC.GetString(PrefName.PracticeST);
				clinic.Zip=PrefC.GetString(PrefName.PracticeZip);
				clinic.Phone=PrefC.GetString(PrefName.PracticePhone);
				clinic.Fax=PrefC.GetString(PrefName.PracticeFax);
				isPractice=true;
			}
			ValidateClinic(clinic,isPractice);
			//At this point we know the clinic is valid since we did not throw an exception.
			return clinic;
		}

		///<summary>Inserts a new ProgramProperty into the database if the passed in ppCur is null.
		///If ppCur is not null, it just sets the PropertyValue and updates.</summary>
		private static void InsertOrUpdate(ProgramProperty programProperty,long programNum,string propDesc,string propValue,long clinicNum) {
			if(programProperty==null) {
				programProperty=new ProgramProperty();
				programProperty.ProgramNum=programNum;
				programProperty.PropertyDesc=propDesc;
				programProperty.PropertyValue=propValue;
				programProperty.ClinicNum=clinicNum;
				ProgramProperties.Insert(programProperty);
			}
			else {
				programProperty.PropertyValue=propValue;
				ProgramProperties.Update(programProperty);
			}
		}

		///<summary>Generates a DoseSpot clinic based on the passed in clinic.
		///The returned DoseSpot clinic does not have the ClinicID set.</summary>
		private static DoseSpotService.Clinic MakeDoseSpotClinic(Clinic clinic) {
			DoseSpotService.Clinic dSSClinic=new DoseSpotService.Clinic();
			dSSClinic.Address1=clinic.Address;
			dSSClinic.Address2=clinic.Address2;
			dSSClinic.City=clinic.City;
			dSSClinic.ClinicName=clinic.Abbr;
			dSSClinic.ClinicNameLongForm=clinic.Description;
			dSSClinic.PrimaryFax=clinic.Fax;
			dSSClinic.PrimaryPhone=clinic.Phone;
			//This is a required field but there is no way to set this value in Open Dental.
			//Since it's a clinic it seems safe to assume that it's a work number
			dSSClinic.PrimaryPhoneType="Work";
			dSSClinic.State=clinic.State.ToUpper();
			dSSClinic.ZipCode=clinic.Zip;
			return dSSClinic;
		}

		///<summary>Makes a clinician out of the tables in OD that make up what DoseSpot considers a clinician.
		///Clinic in this instance can also be a fake ClinicNum 0 clinic that contains practice information.
		///When an employee is the reason for making a clinician, isProxyClinician must be set to true.
		///If isProxyClinician is incorrectly set, the employee will have access to send prescriptions in DoseSpot.</summary>
		private static DoseSpotService.Clinician MakeDoseSpotClinician(Provider provider,Clinic clinic,string emailAddress,bool isProxyClinician) {
			DoseSpotService.Clinician dSSClinician=new DoseSpotService.Clinician();
			dSSClinician.Address1=clinic.Address;
			dSSClinician.Address2=clinic.Address2;
			dSSClinician.City=clinic.City;
			dSSClinician.DateOfBirth=provider.Birthdate;
			dSSClinician.DEANumber=ProviderClinics.GetDEANum(provider.ProvNum,clinic.ClinicNum);//
			dSSClinician.Email=emailAddress;//Email should have been validated by now.
			dSSClinician.FirstName=provider.FName;
			dSSClinician.Gender="Unknown";//This is a required field but we do not store this information.
			dSSClinician.IsProxyClinician=isProxyClinician;
			//retVal.IsReportingClinician=false;//This field was not present in the API documentation and weren't required when testing.
			dSSClinician.LastName=provider.LName;
			dSSClinician.MiddleName=provider.MI;
			dSSClinician.NPINumber=provider.NationalProvID;
			dSSClinician.PrimaryFax=clinic.Fax;
			dSSClinician.PrimaryPhone=clinic.Phone;
			//This is a required field but there is no way to set this value in Open Dental.
			//Since it's a clinic phone number it seems safe to assume that it's a work number
			dSSClinician.PrimaryPhoneType="Work";
			dSSClinician.State=clinic.State.ToUpper();
			dSSClinician.Suffix=provider.Suffix;
			dSSClinician.ZipCode=clinic.Zip;
			return dSSClinician;
		}

		///<summary>If isQueryString is false, the parameter format will assume json</summary>
		private static StringBuilder AddParameter(StringBuilder stringBuilder,string paramName,string paramValue,bool isQueryString) {
			if(isQueryString) {
				return QueryStringAddParameter(stringBuilder,paramName,paramValue);
			}
			return JsonAddParameter(stringBuilder,paramName,paramValue);
		}

		///<summary>Adds a query parameter and that parameter's value to the string builder</summary>
		private static StringBuilder QueryStringAddParameter(StringBuilder stringBuilder,string paramName,string paramValue) {
			stringBuilder.Append("&"+paramName+"=");
			if(paramName!=null) {
				stringBuilder.Append(Uri.EscapeDataString(paramValue));
			}
			return stringBuilder;
		}

		///<summary>Adds a query parameter and that parameter's value to the string builder</summary>
		private static StringBuilder JsonAddParameter(StringBuilder stringBuilder,string paramName,string paramValue) {
			if(paramName!=null && !string.IsNullOrWhiteSpace(paramValue)) {
				stringBuilder.AppendLine("\t"+"\""+paramName+"\""+":"+"\""+paramValue+"\",");
			}
			return stringBuilder;
		}

		///<summary>Generates a SingleSignOn to use for DoseSpot API calls.</summary>
		private static DoseSpotService.SingleSignOn GetSingleSignOn(string clinicID,string clinicKey,string userID,bool isQueryString) {
			clinicID=clinicID.Trim();
			clinicKey=clinicKey.Trim();
			userID=userID.Trim();
			string singleSignOnCode=CreateSsoCode(clinicKey,isQueryString);
			string singleSignOnUserIdVerify=CreateSsoUserIdVerify(clinicKey,userID,isQueryString);
			DoseSpotService.SingleSignOn dSSSingleSignOn=new DoseSpotService.SingleSignOn();
			dSSSingleSignOn.SingleSignOnClinicId=PIn.Int(clinicID);
			dSSSingleSignOn.SingleSignOnUserId=PIn.Int(userID);
			dSSSingleSignOn.SingleSignOnPhraseLength=32;
			dSSSingleSignOn.SingleSignOnCode=singleSignOnCode;
			dSSSingleSignOn.SingleSignOnUserIdVerify=singleSignOnUserIdVerify;
			return dSSSingleSignOn;
		}

		///<summary>Throws exceptions if anything about the provider is invalid.
		///Not throwing an exception means that the provider is valid</summary>
		public static void ValidateProvider(Provider provider,long clinicNum=0) {
			//No need to check RemotingRole; no call to db.
			if(provider==null) {
				throw new Exception("Invalid provider.");
			}
			ProviderClinic providerClinic=ProviderClinics.GetOneOrDefault(provider.ProvNum,clinicNum);
			StringBuilder stringBuilder=new StringBuilder();
			if(provider.IsErxEnabled==ErxEnabledStatus.Disabled) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Erx is disabled for provider.  "
					+"To enable, edit provider in Lists | Providers and acknowledge Electronic Prescription fees."));
			}
			if(provider.IsHidden) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Provider is hidden"));
			}
			if(provider.IsNotPerson) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Provider must be a person"));
			}
			string fName=provider.FName.Trim();
			if(fName=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","First name missing"));
			}
			if(Regex.Replace(fName,"[^A-Za-z\\- ]*","")!=fName) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","First name can only contain letters, dashes, or spaces"));
			}
			string lName=provider.LName.Trim();
			if(lName=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Last name missing"));
			}
			string deaNum="";
			if(providerClinic!=null) {
				deaNum=providerClinic.DEANum;
			}
			if(deaNum.ToLower()!="none" && !Regex.IsMatch(deaNum,"^[A-Za-z]{2}[0-9]{7}$") ) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Provider DEA Number must be 2 letters followed by 7 digits.  If no DEA Number, enter NONE."));
			}
			string npi=Regex.Replace(provider.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
			if(npi.Length!=10) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","NPI must be exactly 10 digits"));
			}
			if(providerClinic==null || providerClinic.StateLicense=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","State license missing"));
			}
			if(providerClinic==null || !USlocales.IsValidAbbr(providerClinic.StateWhereLicensed)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","State where licensed invalid"));
			}
			if(provider.Birthdate.Year<1880) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Birthdate invalid"));
			}
			if(stringBuilder.ToString().Length>0) {
				string clinicText="";
				if(PrefC.HasClinicsEnabled) {
					clinicText=" "+Lans.g("DoseSpot","in clinic")+" "+(clinicNum==0?Lans.g("DoseSpot","Headquarters"):Clinics.GetAbbr(clinicNum));
				}
				throw new ODException(Lans.g("DoseSpot","Issues found for provider")+" "+provider.Abbr+clinicText+":\r\n"+stringBuilder.ToString());
			}
		}

		///<summary>Throws exceptions if anything about the clinic is invalid.
		///Not throwing an exception means that the clinic is valid.
		///In the case that the user is validating clinicNum 0 or doesn't have clinics enabled, the clinic here has 
		/// practice information in it.</summary>
		private static void ValidateClinic(Clinic clinic,bool isPractice = false) {
			if(clinic==null) {
				throw new Exception(Lans.g("DoseSpot","Invalid "+(isPractice ? "practice info." : "clinic.")));
			}
			StringBuilder stringBuilder=new StringBuilder();
			if(clinic.IsHidden) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Clinic is hidden"));
			}
			if(string.IsNullOrWhiteSpace(clinic.Phone)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Phone number is blank"));
			}
			else if(!IsPhoneNumberValid(clinic.Phone)) {//If the phone number isn't valid, DoseSpot will break.
				stringBuilder.AppendLine(Lans.g("DoseSpot","Phone number invalid: ")+clinic.Phone);
			}
			if(string.IsNullOrWhiteSpace(clinic.Fax)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Fax number is blank"));
			}
			else if(!IsPhoneNumberValid(clinic.Fax)) {//If the fax number isn't valid, DoseSpot will break.
				stringBuilder.AppendLine(Lans.g("DoseSpot","Fax number invalid: ")+clinic.Fax);
			}
			if(clinic.Address=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Address is blank"));
			}
			if(IsAddressPOBox(clinic.Address)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Address cannot be a PO BOX"));
			}
			if(clinic.City=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","City is blank"));
			}
			if(string.IsNullOrWhiteSpace(clinic.State)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","State abbreviation is blank"));
			}
			else if(clinic.State.Length<=2 && (clinic.State=="" || (clinic.State!="" && !USlocales.IsValidAbbr(clinic.State)))) {
				//Don't validate state values that are longer than 2 characters.
				stringBuilder.AppendLine(Lans.g("DoseSpot","State abbreviation is invalid"));
			}
			if(clinic.Zip=="" && !Regex.IsMatch(clinic.Zip,@"^([0-9]{9})$|^([0-9]{5}-[0-9]{4})$|^([0-9]{5})$")) {//Blank, or #####, or #####-####, or #########
				stringBuilder.AppendLine(Lans.g("DoseSpot","Zip invalid."));
			}
			if(stringBuilder.ToString().Length>0) {
				if(isPractice) {
					throw new ODException(Lans.g("DoseSpot","Issues found for practice information:")+"\r\n"+stringBuilder.ToString());
				}
				throw new ODException(Lans.g("DoseSpot","Issues found for clinic")+" "+clinic.Abbr+":\r\n"+stringBuilder.ToString());
			}
		}

		///<summary>Search the given address for a PO Box format.  Searches for the string "PO" case insensitive with or without periods. Ignores any match that has a non-space character before it (ex. "123 Ampo St" should be ignored even though it has 'po ')</summary>
		public static bool IsAddressPOBox(string address) {
			string regex=@".*( |^)P\.?O\.? .*";
			return Regex.IsMatch(address,regex,RegexOptions.IgnoreCase);
		}

		///<summary>Throws exceptions for invalid Patient data.</summary>
		public static void ValidatePatientData(Patient patient) {
			string primaryPhone=GetPhoneAndType(patient,0,out string phoneType);
			StringBuilder stringBuilder=new StringBuilder();
			if(patient.FName=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Missing first name."));
			}
			if(patient.LName=="") {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Missing last name."));
			}
			if(patient.Birthdate.Year<1880) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Missing birthdate."));
			}
			if(patient.Birthdate>DateTime.Today) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Invalid birthdate."));
			}
			if(patient.Address.Length==0) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Missing address."));
			}
			if(patient.City.Length<2) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Invalid city."));
			}
			if(string.IsNullOrWhiteSpace(patient.State)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Blank state abbreviation."));
			}
			else if(patient.State.Length<=2 && !USlocales.IsValidAbbr(patient.State)) {//Don't validate state values that are longer than 2 characters.
				stringBuilder.AppendLine(Lans.g("DoseSpot","Invalid state abbreviation."));
			}
			if(string.IsNullOrWhiteSpace(patient.Zip)) {
				stringBuilder.AppendLine(Lans.g("DoseSpot","Blank zip."));
			}
			else if(!Regex.IsMatch(patient.Zip,@"^([0-9]{9})$|^([0-9]{5}-[0-9]{4})$|^([0-9]{5})$")) {//#####, #####-####, or #########
				stringBuilder.AppendLine(Lans.g("DoseSpot","Invalid zip."));
			}
			if(!IsPhoneNumberValid(primaryPhone)) {//If the primary phone number isn't valid, DoseSpot will break.
				stringBuilder.AppendLine(Lans.g("DoseSpot","Invalid phone number: ")+primaryPhone);
			}
			if(stringBuilder.ToString().Length>0) {
				throw new ODException(Lans.g("DoseSpot","Issues found for current patient:")+"\r\n"+stringBuilder.ToString());
			}
		}

		///<summary>If isQueryString is false, this will be built as a json object</summary>
		public static string GetPatientData(Patient patient,bool isQueryString=true) {
			ValidatePatientData(patient);
			string primaryPhone=GetPhoneAndType(patient,0,out string phoneType);
			StringBuilder stringBuilder=new StringBuilder();
			OIDExternal oIDExternal=DoseSpot.GetDoseSpotPatID(patient.PatNum);
			if(oIDExternal!=null) {
				stringBuilder=AddParameter(stringBuilder,"PatientId",oIDExternal.IDExternal,isQueryString);
			}
			stringBuilder=AddParameter(stringBuilder,"Prefix",Tidy(patient.Title,10),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"FirstName",Tidy(patient.FName,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"MiddleName",Tidy(patient.MiddleI,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"LastName",Tidy(patient.LName,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"Suffix","",isQueryString);//I don't see where we store suffixes.
			stringBuilder=AddParameter(stringBuilder,"DateOfBirth",patient.Birthdate.ToShortDateString(),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"Gender",patient.Gender.ToString(),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"Address1",Tidy(patient.Address,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"Address2",Tidy(patient.Address2,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"City",Tidy(patient.City,35),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"State",patient.State,isQueryString);
			stringBuilder=AddParameter(stringBuilder,"ZipCode",patient.Zip,isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PrimaryPhone",primaryPhone,isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PrimaryPhoneType",phoneType,isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PhoneAdditional1",GetPhoneAndType(patient,1,out phoneType),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PhoneAdditionalType1",phoneType,isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PhoneAdditional2",GetPhoneAndType(patient,2,out phoneType),isQueryString);
			stringBuilder=AddParameter(stringBuilder,"PhoneAdditionalType2",phoneType,isQueryString);
			//sb=AddParameter(sb,"Height",Height,isQueryString);
			//sb=AddParameter(sb,"Weight",Weight,isQueryString);
			//sb=AddParameter(sb,"HeightMetric",HeightMetric,isQueryString);
			//sb=AddParameter(sb,"WeightMetric",WeightMetric,isQueryString);
			return stringBuilder.ToString();
		}

		///<summary>Set patient medication history consent for DoseSpot. Optionally pass in parameters for programErx and listProgramProperties.
		///If left null they will be evaluated and set.  Throws exceptions.</summary>
		public static void SetMedicationHistConsent(Patient patient,long clinicNum,Program program=null,List<ProgramProperty> listProgramProperties=null) {
			ValidatePatientData(patient);
			//Get Token
			string doseSpotUserID=GetUserID(Security.CurUser,clinicNum);
			GetClinicIdAndKey(clinicNum,doseSpotUserID,program,listProgramProperties,out string doseSpotClinicID,out string doseSpotClinicKey);
			string token=DoseSpotREST.GetToken(doseSpotUserID,doseSpotClinicID,doseSpotClinicKey);
			//Get DoseSpotPatID
			OIDExternal oIDExternal=GetDoseSpotPatID(patient.PatNum);
			if(oIDExternal==null) {
				//Create a DoseSpot patient and save it for future uses with this patient.
				oIDExternal=CreateOIDForPatient(PIn.Int(DoseSpotREST.AddPatient(token,patient)),patient.PatNum);
			}
			else {
				DoseSpotREST.EditPatient(token,patient,oIDExternal.IDExternal);
			}
			//POST patient consent to DoseSpot
			DoseSpotREST.PostMedicationHistoryConsent(token,oIDExternal.IDExternal);
		}

		private static string Tidy(string value,int length) {
			if(value.Length<=length) {
				return value;
			}
			return value.Substring(0,length);
		}

		///<summary>Valid values for ordinal are 0 (for primary),1, or 2.</summary>
		internal static string GetPhoneAndType(Patient patient,int ordinal,out string phoneType) {
			List<string> listPhoneTypes=new List<string>();
			List<string> listPhoneNumbers=new List<string>();
			if(IsPhoneNumberValid(patient.HmPhone)) {
				listPhoneTypes.Add("Home");
				listPhoneNumbers.Add(patient.HmPhone);
			}
			if(IsPhoneNumberValid(patient.WirelessPhone)) {
				listPhoneTypes.Add("Cell");
				listPhoneNumbers.Add(patient.WirelessPhone);
			}
			if(IsPhoneNumberValid(patient.WkPhone)) {
				listPhoneTypes.Add("Work");
				listPhoneNumbers.Add(patient.WkPhone);
			}
			if(ordinal >= listPhoneNumbers.Count) {
				phoneType="";
				return "";
			}
			phoneType=listPhoneTypes[ordinal];
			string phoneNumber=listPhoneNumbers[ordinal].Replace("(","").Replace(")","").Replace("-","");//remove all formatting as DoseSpot doesn't allow it.
			if(phoneNumber.Length==11 && phoneNumber[0]=='1') {
				phoneNumber=phoneNumber.Substring(1);//Remove leading 1 from phone number since DoseSpot thinks that invalid.
			}
			return phoneNumber;
		}

		public static bool IsPhoneNumberValid(string phoneNumber) {
			string patternPhoneNumber=@"^1?\s*-?\s*(\d{3}|\(\s*\d{3}\s*\))\s*-?\s*\d{3}\s*-?\s*\d{4}(X\d{0,9})?";
			Regex regexPhoneNumber=new Regex(patternPhoneNumber, RegexOptions.IgnoreCase);
			if(phoneNumber!=null) {
				phoneNumber=phoneNumber.Trim();
			}
			if(string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length>=35) {//Max length of 35 is what the DoseSpot example app checks for, there is no documentation supporting it.
				return false;
			}
			if(!regexPhoneNumber.IsMatch(phoneNumber)) {//The regex was taken directly from the DoseSpot example app
				return false;
			}
			string phoneNumberDigits=Regex.Replace(phoneNumber,@"[^0-9]","");//Remove all non-digit characters.
			//Per DoseSpot on 11/15/18, any number starting with 0 or 1 will be rejected by SureScripts
			if(phoneNumberDigits.StartsWith("0") || phoneNumberDigits.StartsWith("1")) {
				return false;
			}
			if(!CheckAreaCode(phoneNumber)) {
				return false;
			}
			return true;
		}

		private static bool CheckAreaCode(string phoneNumber) {
			if(string.IsNullOrWhiteSpace(phoneNumber)) {
				return false;
			}
			phoneNumber=Regex.Replace(phoneNumber,@"[^0-9]","");//Remove all non-digit characters.
			string areaCode="";
			if(phoneNumber.Length<=3) {
				return false;
			}
			if(phoneNumber.Substring(0,1)=="1") {//Remove leading 1 for USA.
				areaCode=phoneNumber.Substring(1,3);
			}
			else {
				areaCode=phoneNumber.Substring(0,3);
			}
			//Per DoseSpot, the only invalid area code combination is 555.
			if(areaCode!="555") {
				return true;
			}
			return false;
		}

		///<summary>This phrase is a fallback incase for some reason the implementor didn't pass in a phrase.
		///This is not recommended by DoseSpot because they will detect that the same phrase is being used for multiple requests</summary>
		private static string Get32CharPhrase() {
			if(_randomPhrase32==null) {
				_randomPhrase32=MiscUtils.CreateRandomAlphaNumericString(32);
			}
			return _randomPhrase32;
		}

		private static byte[] GetBytesFromUTF8(string val) {
			return new UTF8Encoding().GetBytes(val);//Get the value in Bytes from UTF8 String
		}

		private static byte[] GetSHA512Hash(byte[] arrayBytesToHash) {
			byte[] arrayHash;
			using(SHA512 sha512Managed=new SHA512Managed()) {//Use SHA512 to hash the byte value you just received
				arrayHash=sha512Managed.ComputeHash(arrayBytesToHash);
			}
			return arrayHash;
		}

		private static string RemoveExtraEqualSigns(string str) {
			if(str.EndsWith("==")) {//If there are two = signs at the end, then remove them
				str=str.Substring(0,str.Length-2);
			}
			return str;
		}
	}

	public class DoseSpotREST {

		#region GET

		public static string GetToken(string userId,string clinicId,string clinicKey) {
			string userName=clinicId;
			string password=MakeEncryptedClinicId(clinicKey,false);
			string basicAuthContent=System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName+":"+password));
			string body=$"grant_type=password&Username={userId}&Password={MakeEncryptedUserId(clinicKey,userId)}";
			var resObj=Request(ApiRoute.Token,HttpMethod.Post,"Basic "+basicAuthContent,body,new {
				access_token=""
			},acceptType:"x-www-form-urlencoded");
			return resObj.access_token;
		}

		///<summary>Gets a list of all prescriptions from DoseSpot for the passed in patientId.  Call GetDoseSpotPatID to convert patnum -> patientId.</summary>
		public static List<DoseSpotPrescription> GetPrescriptions(string authToken,string patientId) {
			//GET, don't need a body
			string body="";
			var resObj=Request(ApiRoute.GetPrescriptions,HttpMethod.Get,"Bearer "+authToken,body,new {
				Items=new List<DoseSpotPrescription>(),
				Result=new {ResultCode="",ResultDescription=""}
			},"application/json",patientId);
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error getting Prescriptions: ")+resObj.Result.ResultDescription);
			}
			return resObj.Items;
		}

		///<summary>Gets a list of all self reported medications from DoseSpot for the passed in patientId.  Call GetDoseSpotPatID to convert patnum -> patientId.</summary>
		public static List<DoseSpotSelfReported> GetSelfReported(string authToken, string patientId) {
			//GET, don't need a body
			string body="";
			var resObj=Request(ApiRoute.GetSelfReportedMedications,HttpMethod.Get,"Bearer "+authToken,body,new {
				Items=new List<DoseSpotSelfReported>(),
				Result=new {ResultCode="",ResultDescription=""}
			},"application/json",patientId);
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error getting self reported medications: ")+resObj.Result.ResultDescription);
			}
			return resObj.Items;
		}

		///<summary>Returns the StoreName in DoseSpot of the given pharmacy.</summary>
		public static string GetPharmacyName(string authToken,int pharmacyID) {
			//GET, don't need a body
			string body="";
			var resObj=Request(ApiRoute.GetPharmacy,HttpMethod.Get,"Bearer "+authToken,body,new {
				Item=new {
					StoreName="",
				},
				Result=new {
					ResultCode="",
					ResultDescription=""
				}
			},"application/json",pharmacyID.ToString());
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR") || resObj.Item==null) {
				throw new ODException(Lans.g("DoseSpot","Error getting Pharmacy: ")+(resObj.Result.ResultCode.ToUpper().Contains("ERROR")
					? resObj.Result.ResultDescription 
					: Lans.g("DoseSpot","Malformed response from DoseSpot")));
			}
			return resObj.Item.StoreName;
		}


		public static void GetNotificationCounts(string authToken,out int refillRequests,out int transactionErrors,out int pendingPerscriptions) {
			//GET, don't need a body
			string body="";
			var resObj=Request(ApiRoute.GetNotificationCounts,HttpMethod.Get,"Bearer "+authToken,body,new {
				RefillRequestsCount=new int(),
				TransactionErrorsCount=new int(),
				PendingPrescriptionsCount=new int(),
				Result=new {
					ResultCode="",
					ResultDescription=""
				}
			});
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				ODException.ErrorCodes errorCode=ODException.ErrorCodes.NotDefined;
				if(Erx.IsUserAnEmployee(Security.CurUser) 
					&& (resObj.Result.ResultDescription.ToLower().Contains("not authorized") || resObj.Result.ResultDescription.ToLower().Contains("unauthorized")))
				{
					errorCode=ODException.ErrorCodes.DoseSpotNotAuthorized;
				}
				throw new ODException(resObj.Result.ResultDescription,errorCode);
			}
			refillRequests=resObj.RefillRequestsCount;
			transactionErrors=resObj.TransactionErrorsCount;
			pendingPerscriptions=resObj.PendingPrescriptionsCount;
		}
		#endregion
		#region POST

		///<summary></summary>
		public static string AddPatient(string authToken,Patient patient) {
			Vitalsign vitalsign=Vitalsigns.GetOneWithValidHeightAndWeight(patient.PatNum);
			string primaryPhone=DoseSpot.GetPhoneAndType(patient,0,out string phoneType);
			if(patient.Age<18 && vitalsign==null) {
				throw new ODException(Lans.g("DoseSpot","All patients under 18 must have a vital sign reading that includes height and weight. "
					+"To add a vital sign to the patient, go to the 'Chart' module, and double click on the pink medical area. "
					+"There is a tab in the next window labeled vitals, click it and add a vital reading that includes height and weight."));
			}
			string body=JsonConvert.SerializeObject(
				new {
					FirstName=patient.FName.Trim(),
					LastName=patient.LName.Trim(),
					DateOfBirth=patient.Birthdate,
					Gender=patient.Gender+1,
					Address1=patient.Address.Trim(),
					City=patient.City.Trim(),
					State=patient.State.Trim(),
					ZipCode=patient.Zip.Trim(),
					PrimaryPhone=primaryPhone,
					PrimaryPhoneType=phoneType,
					Active="true"
				});
			if(vitalsign!=null) {
				body=JsonConvert.SerializeObject(
				new {
					FirstName = patient.FName.Trim(),
					LastName = patient.LName.Trim(),
					DateOfBirth = patient.Birthdate,
					Gender = patient.Gender+1,
					Address1 = patient.Address.Trim(),
					City = patient.City.Trim(),
					State = patient.State.Trim(),
					ZipCode = patient.Zip.Trim(),
					PrimaryPhone = primaryPhone,
					PrimaryPhoneType = phoneType,
					Weight = vitalsign.Weight,
					WeightMetric = 1,
					Height = vitalsign.Height,
					HeightMetric = 1,
					Active = "true"
				});
			}
			var resObj=Request(ApiRoute.AddPatient,HttpMethod.Post,"Bearer "+authToken,body,new {
				Id="",
				Result=new {ResultCode="",ResultDescription=""}
			});
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error adding patient: ")+resObj.Result.ResultDescription);
			}
			return resObj.Id;
		}

		///<summary>Overwrites Patient Information on DoseSpots side.</summary>
		public static string EditPatient(string authToken,Patient patient,string doseSpotPatId) {
			Vitalsign vitalsign=Vitalsigns.GetOneWithValidHeightAndWeight(patient.PatNum);
			string primaryPhone=DoseSpot.GetPhoneAndType(patient,0,out string phoneType);
			if(patient.Age<18 && vitalsign==null) {
				throw new ODException(Lans.g("DoseSpot","All patients under 18 must have a vital sign reading that includes height and weight. "
					+"To add a vital sign to the patient, go to the 'Chart' module, and double click on the pink medical area. "
					+"There is a tab in the next window labeled vitals, click it and add a vital reading that includes height and weight."));
			}
			string body=JsonConvert.SerializeObject(
				new {
					FirstName=patient.FName.Trim(),
					LastName=patient.LName.Trim(),
					DateOfBirth=patient.Birthdate,
					Gender=patient.Gender+1,
					Address1=patient.Address.Trim(),
					City=patient.City.Trim(),
					State=patient.State.Trim(),
					ZipCode=patient.Zip.Trim(),
					PrimaryPhone=primaryPhone.Trim(),
					PrimaryPhoneType=phoneType.Trim(),
					Active="true"
				});
			if(vitalsign!=null) {
				body=JsonConvert.SerializeObject(
				new {
					FirstName = patient.FName.Trim(),
					LastName = patient.LName.Trim(),
					DateOfBirth = patient.Birthdate,
					Gender = patient.Gender+1,
					Address1 = patient.Address.Trim(),
					City = patient.City.Trim(),
					State = patient.State.Trim(),
					ZipCode = patient.Zip.Trim(),
					PrimaryPhone = primaryPhone.Trim(),
					PrimaryPhoneType = phoneType.Trim(),
					Weight = vitalsign.Weight,
					WeightMetric = 1,
					Height = vitalsign.Height,
					HeightMetric = 1,
					Active = "true"
				});
			}
			var resObj=Request(ApiRoute.EditPatient,HttpMethod.Post,"Bearer "+authToken,body,new {
				Id="",
				Result=new {ResultCode="",ResultDescription=""}
			},"application/json",doseSpotPatId);
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error editing patient: ")+resObj.Result.ResultDescription);
			}
			return resObj.Id;
		}

		///<summary>Only used by BCM at HQ.</summary>
		public static void PostAddClinicToClinicGroup(string authToken,List<string> listClinicIDs,string clinicGroupName) {
			List<long> listIDs=new List<long>();
			try {
				listIDs=listClinicIDs.ConvertAll(long.Parse);
			}
			catch {
				throw new ODException(Lans.g("DoseSpot","Error posting Clinics to group, one or more clinic IDs are not valid numbers: "
					+String.Join(",\r\n",listClinicIDs)));
			}
			string body=JsonConvert.SerializeObject(
				new {
					ClinicIDs=listIDs,
					ClinicGroupName=clinicGroupName.Trim()
				});
			var resObj=Request(ApiRoute.PostClinicGroup,HttpMethod.Post,"Bearer "+authToken,body,new {
				Id="",
				Result=new { ResultCode="",ResultDescription=""}
			});
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error posting Clinics to group: ")+resObj.Result.ResultDescription);
			}
		}

		public static void PostMedicationHistoryConsent(string authToken,string patientId) {
			var resObj=Request(ApiRoute.LogMedicationHistoryConsent,HttpMethod.Post,"Bearer "+authToken,"",new {
				Item="",
				Result=new { ResultCode="",ResultDescription=""}
			},"application/json",patientId);
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error posting medication history consent: ")+resObj.Result.ResultDescription);
			}
		}

		///<summary>Add a new clinic to DoseSpot and automatically add the provider from the authToken to that new clinic.
		///On OK this method puts out the new clinicId and clinicKey as assigned by DoseSpot.</summary>
		public static void PostClinic(string authToken,Clinic clinic,out string clinicID,out string clinicKey) {
			clinicID="";
			clinicKey="";
			//From the DoseSpot REST API guide:
			//	Note: After adding a new clinic, you MUST email the the Clinic Name and Clinic ID to DoseSpot:
			//	STAGING: staging-clinicadd-notifications.907f8409@sniffle.dosespot.podio.com OR
			//	PRODUCTION: production-clinicadd-notifications.ef8bed85@sniffle.dosespot.podio.com
			//Phone type "7" means "Primary" which seemed like a good default because I don't see why a clinic needs a phone type.
			string body=JsonConvert.SerializeObject(
				new {
					ClinicName=clinic.Abbr.Trim(),
					Address1=clinic.Address.Trim(),
					City=clinic.City.Trim(),
					State=clinic.State.Trim(),
					ZipCode=clinic.Zip.Trim(),
					PrimaryPhone=clinic.Phone.Trim(),
					PrimaryPhoneType="7"
				});
			var resObj=Request(ApiRoute.PostClinic,HttpMethod.Post,"Bearer "+authToken,body,new {
				ClinicId="",
				ClinicKey="",
				Result=new {ResultCode="",ResultDescription=""}
			});
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error posting clinic: ")+resObj.Result.ResultDescription);
			}
			clinicID=resObj.ClinicId;
			clinicKey=resObj.ClinicKey;
		}

		///<summary>POST a self reported medication to DoseSpot.  Only used for adding new medications.</summary>
		public static int PostSelfReportedMedications(string authToken,string patientId,DoseSpotSelfReported doseSpotSelfReported) {
			if(doseSpotSelfReported==null || doseSpotSelfReported.SelfReportedMedicationId==null) {
				throw new ODException(Lans.g("DoseSpot","Error creating self reported medication ID: Could not convert medication ID to DoseSpot ID."));
			}
			bool doIncludeInactiveDate=doseSpotSelfReported.DateInactive.HasValue && doseSpotSelfReported.DateInactive.Value.Year>1880;
			string body=JsonConvert.SerializeObject(
				new {
					DisplayName=doseSpotSelfReported.DisplayName.Trim(),
					Status=(int)doseSpotSelfReported.MedicationStatus,
					Comment=doseSpotSelfReported.Comment.Trim()
				});
			if(doIncludeInactiveDate) {
				body=JsonConvert.SerializeObject(
					new {
						DisplayName = doseSpotSelfReported.DisplayName.Trim(),
						Status = (int)doseSpotSelfReported.MedicationStatus,
						InactiveDate = doseSpotSelfReported.DateInactive,
						Comment = doseSpotSelfReported.Comment.Trim()
					});
			}
			var resObj=Request(ApiRoute.PostSelfReportedMedications,HttpMethod.Post,"Bearer "+authToken,body,new {
				Id=-1,
				Result=new { ResultCode="",ResultDescription=""}
			},"application/json",patientId,doseSpotSelfReported.SelfReportedMedicationId.ToString());
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error posting medication: ")+resObj.Result.ResultDescription);
			}
			return resObj.Id;
		}

		#endregion
		#region PUT

		///<summary>PUT a self reported medication to DoseSpot.  Only used for updating existing medications per DS API guide.</summary>
		public static void PutSelfReportedMedications(string authToken,string patientId,DoseSpotSelfReported doseSpotSelfReported) {
			if(doseSpotSelfReported==null || doseSpotSelfReported.SelfReportedMedicationId==null) {
				throw new ODException(Lans.g("DoseSpot","Error creating self reported medication ID: Could not convert medication ID to DoseSpot ID."));
			}
			bool doIncludeInactiveDate=doseSpotSelfReported.DateInactive.HasValue && doseSpotSelfReported.DateInactive.Value.Year>1880;
			string body=JsonConvert.SerializeObject(
				new {
					DisplayName=doseSpotSelfReported.DisplayName.Trim(),
					Status=(int)doseSpotSelfReported.MedicationStatus,
					Comment=doseSpotSelfReported.Comment.Trim()
				});
			if(doIncludeInactiveDate) {
				body=JsonConvert.SerializeObject(
					new {
						DisplayName = doseSpotSelfReported.DisplayName.Trim(),
						Status = (int)doseSpotSelfReported.MedicationStatus,
						InactiveDate = doseSpotSelfReported.DateInactive,
						Comment = doseSpotSelfReported.Comment.Trim()
					});
			}
			var resObj=Request(ApiRoute.PutSelfReportedMedications,HttpMethod.Put,"Bearer "+authToken,body,new {
				Id=-1,
				Result=new { ResultCode="",ResultDescription=""}
			},"application/json",patientId,doseSpotSelfReported.SelfReportedMedicationId.ToString());
			if(resObj.Result.ResultCode.ToUpper().Contains("ERROR")) {
				throw new ODException(Lans.g("DoseSpot","Error putting medication: ")+resObj.Result.ResultDescription);
			}
		}

		#endregion

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static T Request<T>(ApiRoute apiRoute,HttpMethod httpMethod,string authHeader,string body,T responseType,string acceptType = "application/json",params string[] listRouteIDs) {
			using(WebClient webClient=new WebClient()) {
				webClient.Headers[HttpRequestHeader.Accept]=acceptType;
				webClient.Headers[HttpRequestHeader.ContentType]=acceptType;
				webClient.Headers[HttpRequestHeader.Authorization]=authHeader;
				webClient.Encoding=UnicodeEncoding.UTF8;
				//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
				try {
					string response="";
					//Only GET and POST are supported currently.
					if(httpMethod==HttpMethod.Get) {
						response=webClient.DownloadString(GetApiUrl(apiRoute,listRouteIDs));
					}
					else if(httpMethod==HttpMethod.Post) {
						response=webClient.UploadString(GetApiUrl(apiRoute,listRouteIDs),HttpMethod.Post.Method,body);
					}
					else if(httpMethod==HttpMethod.Put) {
						response=webClient.UploadString(GetApiUrl(apiRoute,listRouteIDs),HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+httpMethod.Method);
					}
					if(ODBuild.IsDebug()) {
						if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
							return (T)Convert.ChangeType(response,typeof(T));
						}
					}
					return JsonConvert.DeserializeAnonymousType(response,responseType);
				}
				catch(WebException wex) {
					if(!(wex.Response is HttpWebResponse)) {
						throw new ODException(Lans.g("DoseSpot","Could not connect to the DoseSpot server:")+"\r\n"+wex.Message,wex);
					}
					string responseErr="";
					using(var streamReader=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						responseErr=streamReader.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(responseErr)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("DoseSpot","Invalid DoseSpot credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(responseErr) ? "" : "\r\nRaw response:\r\n"+responseErr);
					throw new Exception(errorMsg,wex);//If it got this far and haven't rethrown, simply throw the entire exception.
				}
				catch(Exception ex) {
					//WebClient returned an http status code >= 300
					ex.DoNothing();
					//For now, rethrow error and let whoever is expecting errors to handle them.
					//We may enhance this to care about codes at some point.
					throw;
				}
			}
		}

		///<summary>Returns the full URL according to the route/route id(s) given.  RouteIDs must be added in order left to right as they appear
		///in the API call.</summary>
		private static string GetApiUrl(ApiRoute apiRoute,params string[] arrayRouteIDs) {
			string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.DoseSpotURL,"https://my.dosespot.com/webapi");
			if(ODBuild.IsDebug()) {
				//apiUrl="https://my.staging.dosespot.com/webapi";
			}
			switch(apiRoute) {
				case ApiRoute.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case ApiRoute.Token:
					apiUrl+="/token";
					break;
				case ApiRoute.AddPatient:
					apiUrl+="/api/patients";
					break;
				case ApiRoute.EditPatient:
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}";
					break;
				case ApiRoute.GetNotificationCounts:
					apiUrl+=$"/api/notifications/counts";
					break;
				case ApiRoute.GetPharmacy:
					//routeId[0]=pharmacyId
					apiUrl+=$"/api/pharmacies/{arrayRouteIDs[0]}";
					break;
				case ApiRoute.GetPrescriptions:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}/prescriptions";
					break;
				case ApiRoute.GetSelfReportedMedications:
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}/selfReportedMedications";
					break;
				case ApiRoute.LogMedicationHistoryConsent:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}/logMedicationHistoryConsent";
					break;
				case ApiRoute.PostClinic:
					apiUrl+=$"/api/clinics";
					break;
				case ApiRoute.PostClinicGroup:
					apiUrl+=$"/api/clinics/clinicGroup";
					break;
				case ApiRoute.PutSelfReportedMedications:
					//routeId[0]=PatientId, routeId[1]=selfReportedMedicationId
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}/selfReportedMedications/freetext/{arrayRouteIDs[1]}";
					break;
				case ApiRoute.PostSelfReportedMedications:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{arrayRouteIDs[0]}/selfReportedMedications/freetext";
					break;
				default:
					break;
			}
			return apiUrl;
		}

		private enum ApiRoute {
			Root,
			Token,
			AddPatient,
			EditPatient,
			GetPharmacy,
			GetPrescriptions,
			GetNotificationCounts,
			GetSelfReportedMedications,
			LogMedicationHistoryConsent,
			PostClinic,
			PutSelfReportedMedications,
			PostClinicGroup,
			PostSelfReportedMedications,
		}

		///<summary>Medication statuses used by DoseSpot.  Unknown(0) and Inactive(2) are depricated.  This enum intentionally starts at 0 according to DoseSpot.</summary>
		public enum MedicationStatus {
			Unknown,//Depricated by DoseSpot
			Active,
			Inactive,//Depricated by DoseSpot
			Discontinued,
			Deleted,
			Completed,
			CancelRequested,
			CancelPending,
			Cancelled,
			CancelDenied,
			ChangeD,
		}

		///<summary>Prescription statuses used by DoseSpot, not the same as MedicationStatus.  This enum intentionally starts at 1 according to DoseSpot.</summary>
		public enum PrescriptionStatus {
			Entered=1,
			Printed,
			Sending,
			eRxSent,
			FaxSent,
			Error,
			Deleted,
			Requested,
			Edited,
			EpcsError,
			EpcsSigned,
			ReadyToSign,
			PharmacyVerified,
			///<summary>Dummy status used by DoseSpotMedicationWrapper</summary>
			NotAPresciption,
		}

		///<summary>This method uses the same equation as DoseSpot's other webservice encryption method</summary>
		private static string MakeEncryptedClinicId(string clinicKey,bool isQueryStr=true) {
			return DoseSpot.CreateSsoCode(clinicKey,isQueryStr);
		}

		///<summary>This method uses the same equation as DoseSpot's other webservice encryption method.</summary>
		private static string MakeEncryptedUserId(string clinicKey,string userID,bool isQueryStr=true) {
			return DoseSpot.CreateSsoUserIdVerify(clinicKey,userID,isQueryStr);
		}

		///<summary>Helper function that takes in a MedicationPat and creates a DoseSpot SelfReported object with all required fields for a POST/PUT.</summary>
		public static DoseSpotSelfReported MedicationPatToDoseSpotSelfReport(MedicationPat medicationPat) {
			DoseSpotSelfReported doseSpotSelfReported=new DoseSpotSelfReported();
			doseSpotSelfReported.SelfReportedMedicationId=0;
			//If we have the ErxGuid then it has already been sent to DS.
			if(!medicationPat.ErxGuid.IsNullOrEmpty()) {
				//Remove any possible prefixes from the ErxGuid. It will either be a DS prefix, an OD prefix, or have no prefix at all if it came from an Rx.
				int ID=0;
				string guid=medicationPat.ErxGuid;
				//If it has the unsent prefix we want to intentionally set the ID to 0 so we post a new self reported and get an ID back from DS.
				if(!guid.StartsWith(Erx.UnsentPrefix)) {
					guid=Regex.Replace(guid,Erx.DoseSpotPatReportedPrefix,"");//Remove DS prefix.
					guid=Regex.Replace(guid,Erx.OpenDentalErxPrefix,"");//Remove OD prefix.
					int.TryParse(guid,out ID);
				}
				doseSpotSelfReported.SelfReportedMedicationId=ID;
			}
			//Set the DisplayName.
			if(String.IsNullOrEmpty(medicationPat.MedDescript) && medicationPat.MedicationNum!=0) {
				Medication medication=Medications.GetMedication(medicationPat.MedicationNum);
				doseSpotSelfReported.DisplayName=medication.MedName;
			}
			else {
				doseSpotSelfReported.DisplayName=medicationPat.MedDescript;
			}
			//Strip any newlines before sending. Newlines cause parsing errors for the DoseSpot API. The changes to this note will be synced back into OD when the eRx window is closed.
			doseSpotSelfReported.Comment=Regex.Replace(medicationPat.PatNote,@"\n|\r"," ");
			//500 characters is the max size for the comment field. Going over 500 will return a 400 error from the DoseSpot API.
			if(doseSpotSelfReported.Comment.Length>500) {
				SecurityLogs.MakeLogEntry(Permissions.LogDoseSpotMedicationNoteEdit,medicationPat.PatNum,"Medication patient note automatically reduced to 500 characters for sending to DoseSpot. Original note: "+"\n"
					+medicationPat.PatNote);
				doseSpotSelfReported.Comment=doseSpotSelfReported.Comment.Substring(0,500);
			}
			//Set the MedicationStatus.
			doseSpotSelfReported.MedicationStatus=MedicationStatus.Discontinued;
			if(medicationPat.DateStop.Year<1880 || medicationPat.DateStop>=DateTime.Today) {
				doseSpotSelfReported.MedicationStatus=MedicationStatus.Active;
			}
			else if(medicationPat.DateStop<DateTime.Today) {
				doseSpotSelfReported.MedicationStatus=MedicationStatus.Completed;
				doseSpotSelfReported.DateInactive=medicationPat.DateStop;
				//A comment is required when a medication has been discontinued (figured out through testing, not in docs)
				if(doseSpotSelfReported.Comment.IsNullOrEmpty()) {
					//We were infinitely adding this note which was causing the comment to be greater than 500 characters and caused a 400 from DS.
					doseSpotSelfReported.Comment="Discontinued in Open Dental";
				}
			}
			return doseSpotSelfReported;
		}

	}

	///<summary>This is a class to reflect the response object from DoseSpot's RESTful JSON objects.  
	///Fields are nullable due to DoseSpot returning null values in some of their fields during testing, 
	///as well as to safeguard against missing fields in the future.</summary>
	public class DoseSpotPrescription {
		//DO NOT RENAME ANYTHING IN THIS CLASS. IT IS USED FOR SERIALIZING DOSESPOT API RESPONSES AND NEEDS TO MATCH THE DOSESPOT API DOCS.
		public int? PrescriptionId;
		public DateTime? WrittenDate;
		public string Directions;
		public string Quantity;
		public int? DispenseUnitId;
		public string DispenseUnitDescription;
		public string Refills;
		public int? DaysSupply;
		public int? PharmacyId;
		public string PharmacyNotes;
		public bool? NoSubstitutions;
		public DateTime? EffectiveDate;
		public DateTime? LastFillDate;
		public int? PrescriberId;
		public int? PrescriberAgentId;
		public string RxReferenceNumber;
		public DoseSpotREST.PrescriptionStatus Status;
		public bool? Formulary;
		public int? EligibilityId;
		public int? Type;
		public string NonDoseSpotPrescriptionId;
		public int? PatientMedicationId;
		public DoseSpotREST.MedicationStatus MedicationStatus;
		public string Comment;
		public DateTime? DateInactive;
		public string Encounter;
		public string DoseForm;
		public string Route;
		public string Strength;
		public string GenericProductName;
		public int? LexiGenProductId;
		public int? LexiDrugSynId;
		public int? LexiSynonymTypeId;
		public string LexiGenDrugId;
		///<summary>According to US Dept. HHS RxCUI is a numeric value with max length of 8. DoseSpot sends this value as a string.</summary>
		public string RxCUI;
		public bool? OTC;
		public string NDC;
		public string Schedule;
		public string DisplayName;
		public string MonographPath;
		public string DrugClassification;
	}

	///<summary>This is a class to reflect the response object from DoseSpot's RESTful JSON objects.  
	///Fields are nullable due to DoseSpot returning null values in some of their fields during testing, 
	///as well as to safeguard against missing fields in the future.</summary>
	public class DoseSpotSelfReported {
		//DO NOT RENAME ANYTHING IN THIS CLASS. IT IS USED FOR SERIALIZING DOSESPOT API RESPONSES AND NEEDS TO MATCH THE DOSESPOT API DOCS.
		public int? SelfReportedMedicationId;
		public DateTime? DateReported;
		public DateTime? WrittenDate;
		public string Directions;
		public string Quantity;
		public int? DispenseUnitId;
		public string DispenseUnitDescription;
		public string Refills;
		public int? DaysSupply;
		public int? PatientMedicationId;
		public DoseSpotREST.MedicationStatus MedicationStatus;
		public string Comment;
		public DateTime? DateInactive;
		public string Encounter;
		public string DoseForm;
		public string Route;
		public string Strength;
		public string GenericProductName;
		public int? LexiGenProductId;
		public int? LexiDrugSynId;
		public int? LexiSynonymTypeId;
		public string LexiGenDrugId;
		public string RxCUI;
		public bool? OTC;
		public string NDC;
		public string Schedule;
		public string DisplayName;
		public string MonographPath;
		public string DrugClassification;
	}

	///<summary>Wrapper class to hold prescriptions and medications that we recieve from DoseSpot before converting into RxPats.  Not all fields 
	///for the two classes have been made into properties yet, add them as needed.</summary>
	public class DoseSpotMedicationWrapper {
		readonly DoseSpotSelfReported _doseSpotSelfReported;
		readonly DoseSpotPrescription _doseSpotPrescription;

		///<summary>True is a self reported medication, false implies a prescription.</summary>
		public bool IsSelfReported {
			get {
				return _doseSpotSelfReported!=null;
			}
		}
		public DateTime? DateInactive {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.DateInactive;
				}
				return _doseSpotPrescription.DateInactive;
			}
		}
		public string DisplayName {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.DisplayName;
				}
				return _doseSpotPrescription.DisplayName;
			}
		}
		public DateTime? DateReported {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.DateReported;
				}
				return _doseSpotPrescription.EffectiveDate;
			}
		}
		public string GenericProductName {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.GenericProductName;
				}
				return _doseSpotPrescription.GenericProductName;
			}
		}
		public DateTime? DateLastFilled {
			get {
				if(IsSelfReported) {
					return null;
				}
				return _doseSpotPrescription.LastFillDate;
			}
		}
		public long? MedicationId {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.SelfReportedMedicationId;
				}
				return _doseSpotPrescription.PrescriptionId;
			}
		}
		public DoseSpotREST.MedicationStatus MedicationStatus {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.MedicationStatus;
				}
				return _doseSpotPrescription.MedicationStatus;
			}
		}
		public int? PharmacyId {
			get {
				if(IsSelfReported) {
					return null;
				}
				return _doseSpotPrescription.PharmacyId;
			}
		}
		public string Directions {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.Comment;
				}
				return _doseSpotPrescription.Directions;
			}
		}
		public string RxNotes {
			get {
				if(IsSelfReported) {
					return null;
				}
				return _doseSpotPrescription.PharmacyNotes;
			}
		}
		public int? PrescriberId {
			get {
				if(IsSelfReported) {
					return null;
				}
				return _doseSpotPrescription.PrescriberId;
			}
		}
		public DoseSpotREST.PrescriptionStatus PrescriptionStatus {
			get {
				if(IsSelfReported) {
					//This makes it fall through to the base case and do nothing.
					return DoseSpotREST.PrescriptionStatus.NotAPresciption;
				}
				return _doseSpotPrescription.Status;
			}
		}
		public string Quantity {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.Quantity;
				}
				return _doseSpotPrescription.Quantity;
			}
		}
		public string Refills {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.Refills;
				}
				return _doseSpotPrescription.Refills;
			}
		}
		public long RxCUI {
			get {
				if(IsSelfReported) {
					if(!_doseSpotSelfReported.RxCUI.IsNullOrEmpty()) {
						return PIn.Long(_doseSpotSelfReported.RxCUI,false);//Cast from string to long is intentional.
					}
				}
				else {
					if(!_doseSpotPrescription.RxCUI.IsNullOrEmpty()) {
						return PIn.Long(_doseSpotPrescription.RxCUI,false);//Cast from string to long is intentional.
					}
				}
				//Something went wrong.
				return 0;
			}
		}
		public string Schedule {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.Schedule;
				}
				return _doseSpotPrescription.Schedule;
			}
		}
		public DateTime? DateWritten {
			get {
				if(IsSelfReported) {
					return _doseSpotSelfReported.WrittenDate;
				}
				return _doseSpotPrescription.WrittenDate;
			}
		}

		public DoseSpotMedicationWrapper(DoseSpotPrescription doseSpotPrescription, DoseSpotSelfReported doseSpotSelfReported) {
			_doseSpotPrescription=doseSpotPrescription;
			_doseSpotSelfReported=doseSpotSelfReported;
		}
	}
}
