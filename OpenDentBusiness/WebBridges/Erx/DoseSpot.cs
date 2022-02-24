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
		public static long DoseSpotCustomerPatNum {
			get {
				string rootExternal=GetDoseSpotRoot();
				long retVal=0;
				//Gets the value of the last section of the root, which for DoseSpot/Internal OIDs is the PatNum of the office in the OD HQ database.
				//This is grabbed instead of asking OD HQ for the patnum associated to the account 
				// because there might be an edge case where the registration key for this office got moved to a new PatNum,
				// and that would hide every DoseSpot OID link.
				try {
					retVal=PIn.Long(rootExternal.Substring(rootExternal.LastIndexOf(".")+1));
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				return retVal;
			}
		}

		///<summary>Returns the rootExternal that identifies OIDs as Dose Spot </summary>
		public static string GetDoseSpotRoot() {
			//No need to check RemotingRole; no call to db.
			//The advantage of returning the root from the database is that there could be a scary edge case where 
			// the PatNum of the office's regkey in OD HQ database could have been changed
			OIDExternal root=OIDExternals.GetByPartialRootExternal(_doseSpotOid);
			if(root==null) {
				root=new OIDExternal();
				root.IDType=IdentifierType.Root;
				root.rootExternal=OIDInternals.OpenDentalOID+"."+_doseSpotPatNum+"."+OIDInternals.CustomerPatNum;
				OIDExternals.Insert(root);
			}
			return root.rootExternal;
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
			AlertItem alert=AlertItems.RefreshForType(AlertType.DoseSpotClinicRegistered)
				.FirstOrDefault(x => x.FKey==clinicErx.ClinicErxNum);
			if(alert!=null) {
				return;//alert already exists
			}
      Clinic clinic=Clinics.GetClinic(clinicErx.ClinicNum);
      List<ProgramProperty> listDoseSpotClinicProperties=ProgramProperties.GetForProgram(Programs.GetProgramNum(ProgramName.eRx))
          .FindAll(x => x.ClinicNum==0
            && (x.PropertyDesc==Erx.PropertyDescs.ClinicID || x.PropertyDesc==Erx.PropertyDescs.ClinicKey)
            && !string.IsNullOrWhiteSpace(x.PropertyValue));
      if(clinic!=null || clinicAutomaticallyAttached) {
        //A clinic was associated with the clinicerx successfully, no user action needed.
        alert=new AlertItem {
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
        alert=new AlertItem {
          Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.OpenForm,
          Description=Lans.g("DoseSpot","Select clinic to assign ID"),
          Severity=SeverityType.Low,
          Type=AlertType.DoseSpotClinicRegistered,
          ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because we don't know which clinic to display in.
          FKey=clinicErx.ClinicErxNum,
          FormToOpen=FormType.FormDoseSpotAssignClinicId,
        };
      }
      AlertItems.Insert(alert);
		}

		///<summary>Handles assigning Dose Spot ID to a user with matching NPI.
		///If multiple or no matches, creates form for manual selection of user.</summary>
		public static void MakeProviderErxAlert(ProviderErx providerErx) {
			AlertItem alert=AlertItems.RefreshForType(AlertType.DoseSpotProviderRegistered)
				.FirstOrDefault(x => x.FKey==providerErx.ProviderErxNum);
			if(alert!=null) {
				return;//alert already exists
			}
			//get a list of users that correspond to a non-hidden provider
			List<Provider> listProviders=Providers.GetWhere(x => x.NationalProvID==providerErx.NationalProviderID,true);
			List<Userod> listDoseUsers=Userods.GetWhere(x => listProviders.Exists(y => y.ProvNum==x.ProvNum),true);//Only consider non-hidden users.
			if(listDoseUsers.Count==1) {//One provider matched so simply notify the office and set the DoseSpot User Id.
				alert=new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.ShowItemValue,
					Description=Lans.g("DoseSpot","User automatically assigned."),
					Severity=SeverityType.Low,
					Type=AlertType.DoseSpotProviderRegistered,
					FKey=providerErx.ProviderErxNum,
					ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because providers aren't clinic specific
					ItemValue=Lans.g("DoseSpot","User: ")+listDoseUsers[0].UserNum+", "+listDoseUsers[0].UserName+" "
					+Lans.g("DoseSpot","has been assigned a DoseSpot User ID of: ")+providerErx.UserId,
				};
				AlertItems.Insert(alert);
				//set userodpref to UserId
				Program programErx=Programs.GetCur(ProgramName.eRx);
				UserOdPref userDosePref=UserOdPrefs.GetByCompositeKey(listDoseUsers[0].UserNum,programErx.ProgramNum,UserOdFkeyType.Program);
				userDosePref.ValueString=providerErx.UserId;//assign DoseSpot User ID
				if(userDosePref.IsNew) {
					userDosePref.Fkey=programErx.ProgramNum;
					UserOdPrefs.Insert(userDosePref);
				}
				else {
					UserOdPrefs.Update(userDosePref);
				}
			}
			else {//More than one or no user associated to the NPI, generate alert with form to have the office choose which user to assign.
				alert=new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.OpenForm,
					Description=Lans.g("DoseSpot","Select user to assign ID"),
					Severity=SeverityType.Low,
					Type=AlertType.DoseSpotProviderRegistered,
					FKey=providerErx.ProviderErxNum,
					ClinicNum=-1,//Show in all clinics.  We only want 1 alert, but that alert can be processed from any clinic because providers aren't clinic specific
					FormToOpen=FormType.FormDoseSpotAssignUserId,
				};
				AlertItems.Insert(alert);
			}
		}

		///<summary>Returns true if the passed in accountId is a DoseSpot account id.</summary>
		public static bool IsDoseSpotAccountId(string accountId) {
			//No need to check RemotingRole; no call to db.
			return accountId.ToUpper().StartsWith("DS;");//ToUpper because user might have manually typed in.
		}

		///<summary>Creates a unique account id for DoseSpot.  Uses the same generation logic as NewCrop, with DS; preceeding it.</summary>
		public static string GenerateAccountId(long patNum) {
			string retVal="DS;"+POut.Long(patNum);
			retVal+="-"+CodeBase.MiscUtils.CreateRandomAlphaNumericString(3);
			long checkSum=patNum;
			checkSum+=Convert.ToByte(retVal[retVal.IndexOf('-')+1])*3;
			checkSum+=Convert.ToByte(retVal[retVal.IndexOf('-')+2])*5;
			checkSum+=Convert.ToByte(retVal[retVal.IndexOf('-')+3])*7;
			retVal+=(checkSum%100).ToString().PadLeft(2,'0');
			return retVal;
		}

		///<summary>The comments on each line come directly from the DoseSpot API Guide 12.8.pdf.  Creates a Single Sign On Code for DoseSpot.</summary>
		public static string CreateSsoCode(string clinicKey,bool isQueryStr=true) {
			//No need to check RemotingRole; no call to db.
			string retVal="";//1. You have been provided a clinic key (in UTF8).
			string phrase=Get32CharPhrase();//2. Create a random phrase 32 characters long in UTF8
			string phraseAndKey=phrase;
			phraseAndKey+=clinicKey;//3. Append the key to the phrase
			byte[] arrayBytes=GetBytesFromUTF8(phraseAndKey);//4. Get the value in Bytes from UTF8 String
			byte[] arrayHashedBytes=GetSHA512Hash(arrayBytes);//5. Use SHA512 to hash the byte value you just received
			string base64hash=Convert.ToBase64String(arrayHashedBytes);//6. Get a Base64String out of the hash that you created
			base64hash=RemoveExtraEqualSigns(base64hash);//7. If there are two = signs at the end, then remove them
			retVal=phrase+base64hash;//8. Prepend the same random phrase from step 2 to your code
			if(isQueryStr) {//9. If the SingleSignOnCode is going to be passed in a query string, be sure to UrlEncode the entire code
				retVal=WebUtility.UrlEncode(retVal);
			}
			return retVal;
		}

		///<summary>The comments on each line come directly from the DoseSpot API Guide 12.8.pdf.  Creates a Single Sign On User ID Verify for DoseSpot.</summary>
		public static string CreateSsoUserIdVerify(string clinicKey,string userID,bool isQueryStr=true) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			string phrase=Get32CharPhrase();
			string idPhraseAndKey=phrase.Substring(0,22);//1. Grab the first 22 characters of the phrase used in CreateSSOCode from step 1 of CreateSsoCode
			idPhraseAndKey=userID+idPhraseAndKey;//2. Append to the UserID string the 22 characters grabbed from step one
			idPhraseAndKey+=clinicKey;//3. Append the key to the string created in 2b
			byte[] arrayBytes=GetBytesFromUTF8(idPhraseAndKey);//4. Get the Byte value of the string
			byte[] arrayHashedBytes=GetSHA512Hash(arrayBytes);//5. Use SHA512 to hash the byte value you just received
			string base64hash=Convert.ToBase64String(arrayHashedBytes);//6. Get a Base64String out of the hash that you created
			retVal=RemoveExtraEqualSigns(base64hash);//7. If there are two = signs at the end, then remove them
			if(isQueryStr) {//8. If the SingleSignOnUserIdVerify is going to be passed in a query string, be sure to UrlEncode the entire code
				retVal=WebUtility.UrlEncode(retVal);
			}
			_randomPhrase32=null;
			return retVal;
		}

		///<summary>Can throw exceptions.  Returns true if changes were made to medications.</summary>
		public static bool SyncPrescriptionsFromDoseSpot(string clinicID,string clinicKey,string userID,long patNum,Action<List<RxPat>> onRxAdd=null) {
			//No need to check RemotingRole; no call to db.
			OIDExternal oidPatID=DoseSpot.GetDoseSpotPatID(patNum);
			if(oidPatID==null) {
				return false;//We don't have a PatID from DoseSpot for this patient.  Therefore there is nothing to sync with.
			}
			Patient patCur=Patients.GetPat(patNum);
			List<long> listActiveMedicationPatNums=new List<long>();
			Dictionary<int,string> dictPharmacyNames=new Dictionary<int,string>();
			List<RxPat> listNewRx=new List<RxPat>();
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			//Get rid of any deleted prescriptions.
			List<DoseSpotPrescription> prescriptions=DoseSpotREST.GetPrescriptions(token,oidPatID.IDExternal);
			List<DoseSpotMedicationWrapper> listDoseSpotMedWrapper=prescriptions.Select(x => new DoseSpotMedicationWrapper(x,null)).ToList();
			//Add self reported medications.
			listDoseSpotMedWrapper.AddRange(DoseSpotREST.GetSelfReported(token,oidPatID.IDExternal).Select(x => new DoseSpotMedicationWrapper(null,x)).ToList());
			listDoseSpotMedWrapper=listDoseSpotMedWrapper.FindAll(x => x.MedicationStatus!=DoseSpotREST.MedicationStatus.Deleted);
			foreach(DoseSpotMedicationWrapper medication in listDoseSpotMedWrapper) {
				RxPat rxOld=null;
				if(medication.IsSelfReported) {
					//Get self reported that originated in OD
					rxOld=RxPats.GetErxByIdForPat(Erx.OpenDentalErxPrefix+medication.MedicationId.ToString(),patNum);
					if(rxOld==null) {
						//Get self reported that originated in DS
						rxOld=RxPats.GetErxByIdForPat(Erx.DoseSpotPatReportedPrefix+medication.MedicationId.ToString(),patNum);
					}
				}
				else {
					//Isn't self reported, Guid won't have a prefix.
					rxOld=RxPats.GetErxByIdForPat(medication.MedicationId.ToString(),patNum);
				}
				RxPat rx=new RxPat();
				long rxCui=medication.RxCUI;//If this is zero either DoseSpot didn't send the value or there was an issue casting from string to long.
				rx.IsControlled=(PIn.Int(medication.Schedule)!=0);//Controlled if Schedule is I,II,III,IV,V
				rx.DosageCode="";
				rx.SendStatus=RxSendStatus.Unsent;
				switch(medication.PrescriptionStatus) {
					case DoseSpotREST.PrescriptionStatus.PharmacyVerified:
					case DoseSpotREST.PrescriptionStatus.eRxSent:
						rx.SendStatus=RxSendStatus.SentElect;
						break;
					case DoseSpotREST.PrescriptionStatus.FaxSent:
						rx.SendStatus=RxSendStatus.Faxed;
						break;
					case DoseSpotREST.PrescriptionStatus.Printed:
						rx.SendStatus=RxSendStatus.Printed;
						break;
					case DoseSpotREST.PrescriptionStatus.Sending:
						rx.SendStatus=RxSendStatus.Pending;
						break;
					case DoseSpotREST.PrescriptionStatus.Deleted:
						if(rxOld==null) {
							//DoseSpot sent a deleted medication that we don't have a record of. Skip it.
							continue;
						}
						MedicationPat medicationPat = MedicationPats.GetMedicationOrderByErxIdAndPat(rxOld.ErxGuid,rxOld.PatNum);
						RxPats.Delete(rxOld.RxNum);
						SecurityLog securityLog = new SecurityLog();
						securityLog.UserNum=0;//Don't attach to user since this is being done by DoseSpot
						securityLog.CompName=Security.CurComputerName;
						securityLog.PermType=Permissions.RxEdit;
						securityLog.FKey=0;
						securityLog.LogSource=LogSources.eRx;
						securityLog.LogText="FROM("+rxOld.RxDate.ToShortDateString()+","+rxOld.Drug+","+rxOld.ProvNum+","+rxOld.Disp+","+rxOld.Refills+")"+"\r\nTO 'deleted' change made by DoseSpot eRx.";
						securityLog.PatNum=rxOld.PatNum;
						securityLog.DefNum=0;
						securityLog.DefNumError=0;
						securityLog.DateTPrevious=DateTime.MinValue;
						SecurityLogs.MakeLogEntry(securityLog);
						if(medicationPat!=null) {
							MedicationPats.Delete(medicationPat);
							securityLog.PermType=Permissions.PatMedicationListEdit;
							securityLog.LogText=(medicationPat.MedicationNum==0 ? medicationPat.MedDescript : Medications.GetMedication(medicationPat.MedicationNum).MedName
								)+" deleted by DoseSpot"+"\r\n"
								+(String.IsNullOrEmpty(medicationPat.PatNote) ? "" : "Pat note: "+medicationPat.PatNote);
							securityLog.PatNum=medicationPat.PatNum;//probably the same but better safe than sorry.
							securityLog.SecurityLogNum=0;//Reset primary key to guarantee insert will work no matter what changes get made to securitylog code.
							SecurityLogs.MakeLogEntry(securityLog);
						}
						break;
					case DoseSpotREST.PrescriptionStatus.Error:
					case DoseSpotREST.PrescriptionStatus.EpcsError:
						continue;//Skip these medications since DoseSpot is saying that they are invalid
					case DoseSpotREST.PrescriptionStatus.Edited:
					case DoseSpotREST.PrescriptionStatus.Entered:
					case DoseSpotREST.PrescriptionStatus.EpcsSigned:
					case DoseSpotREST.PrescriptionStatus.ReadyToSign:
					case DoseSpotREST.PrescriptionStatus.Requested:
					default:
						rx.SendStatus=RxSendStatus.Unsent;
						break;
				}
				rx.Refills=medication.Refills;
				rx.Disp=medication.Quantity;//In DoseSpot, the Quanitity textbox's label says "Dispense".
				rx.Drug=medication.DisplayName;
				if(medication.PharmacyId.HasValue) {
					try {
						if(!dictPharmacyNames.ContainsKey(medication.PharmacyId.Value)) {
							dictPharmacyNames.Add(medication.PharmacyId.Value,DoseSpotREST.GetPharmacyName(token,medication.PharmacyId.Value));
						}
						rx.ErxPharmacyInfo=dictPharmacyNames[medication.PharmacyId.Value];
					}
					catch(Exception ex) {
						ex.DoNothing();
						//Do nothing.  It was a nicety anyways.
					}
				}
				rx.PatNum=patNum;
				rx.Sig=medication.Directions;
				rx.Notes=medication.RxNotes;
				rx.RxDate=DateTime.MinValue;
				//If none of dates have values, the RxDate will be MinValue.
				//This is acceptable if DoseSpot doesn't give us anything, which should never happen.
				if(medication.DateWritten.HasValue) {
					rx.RxDate=medication.DateWritten.Value;
				}
				else if(medication.DateReported.HasValue) {
					rx.RxDate=medication.DateReported.Value;
				}
				else if(medication.DateLastFilled.HasValue) {
					rx.RxDate=medication.DateLastFilled.Value;
				}
				else if(medication.DateInactive.HasValue) {
					rx.RxDate=medication.DateInactive.Value;
				}
				//Save DoseSpot's unique ID into our rx
				int doseSpotMedId=(int?)medication.MedicationId??0;//If this changes, we need to ensure that Erx.IsFromDoseSpot() is updated to match.
				rx.ErxGuid=doseSpotMedId.ToString();
				bool isProv=false;
				if(medication.IsSelfReported) {//Self Reported medications won't have a prescriber number
					if(rxOld==null) {//Rx doesn't exist in the database.  This probably originated from DoseSpot
						MedicationPat medPat=MedicationPats.GetMedicationOrderByErxIdAndPat(Erx.OpenDentalErxPrefix+medication.MedicationId.ToString(),patNum);
						if(medPat==null) {//If there isn't a record of the medication 
							medPat=MedicationPats.GetMedicationOrderByErxIdAndPat(Erx.DoseSpotPatReportedPrefix+medication.MedicationId.ToString(),patNum);
						}
						if(medPat==null) {//If medPat is null at this point we don't have a record of this patient having the medication, so it probably was just made in DoseSpot.
							rx.ErxGuid=Erx.DoseSpotPatReportedPrefix+medication.MedicationId;
						}
						else {
							rx.ErxGuid=medPat.ErxGuid;//Maintain the ErxGuid that was assigned for the MedicationPat that already exists.
						}
					}
					else {
						rx.ErxGuid=rxOld.ErxGuid;//Maintain the ErxGuid that was already assigned for the Rx.
					}
				}
				else {
					//The prescriber ID for each medication is the doctor that approved the prescription.
					UserOdPref userPref=UserOdPrefs.GetByFkeyAndFkeyType(Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program)
					.FirstOrDefault(x => x.ValueString==medication.PrescriberId.ToString());
					if(userPref==null) {//The Dose Spot User ID from this medication is not present in Open Dental.
						continue;//I don't know if we want to do anything with this.  Maybe we want to just get the ErxLog from before this medication was made.
					}
					Userod user=Userods.GetUser(userPref.UserNum);
					Provider prov=new Provider();
					isProv=!Erx.IsUserAnEmployee(user);
					if(isProv) {//A user always be a provider if there is a ProvNum > 0
						prov=Providers.GetProv(user.ProvNum);
					}
					else {
						prov=Providers.GetProv(patCur.PriProv);
					}
					rx.ProvNum=prov.ProvNum;
				}
				//These fields are possibly set above, preserve old values if they are not.
				if(rxOld!=null) {
					rx.Disp=rxOld.Disp; //The medication Disp currently always returns 0. Preserve the old value.
					rx.Refills=rx.Refills==null ? rxOld.Refills : rx.Refills;
					rx.Notes=rxOld.Notes.IsNullOrEmpty() ? rx.Notes : rxOld.Notes;//Preserve the note already in OD no matter what if there is one.
					rx.PatientInstruction=rx.PatientInstruction.IsNullOrEmpty() ? rxOld.PatientInstruction : rx.PatientInstruction;
					rx.ErxPharmacyInfo=rx.ErxPharmacyInfo==null ? rxOld.ErxPharmacyInfo : rx.ErxPharmacyInfo;
					rx.IsControlled=rxOld.IsControlled;
					if(rxOld.RxDate.Year>1880) {
						rx.RxDate=rxOld.RxDate;
					}
				}
				long medicationPatNum=0;
				if(Erx.IsDoseSpotPatReported(rx.ErxGuid) || Erx.IsTwoWayIntegrated(rx.ErxGuid)) {//For DoseSpot self reported, do not insert a prescription.
					medicationPatNum=Erx.InsertOrUpdateErxMedication(rxOld,rx,rxCui,medication.DisplayName,medication.GenericProductName,isProv,false);
				}
				else {
					medicationPatNum=Erx.InsertOrUpdateErxMedication(rxOld,rx,rxCui,medication.DisplayName,medication.GenericProductName,isProv);
				}
				if(rxOld==null) {//Only add the rx if it is new.  We don't want to trigger automation for existing prescriptions.
					listNewRx.Add(rx);
				}
				if(medication.MedicationStatus==DoseSpotREST.MedicationStatus.Active) {
					listActiveMedicationPatNums.Add(medicationPatNum);
				}
			}
			List<MedicationPat> listAllMedicationsForPatient=MedicationPats.Refresh(patNum,false);
			for(int i=0;i<listAllMedicationsForPatient.Count;i++) {
			//This loop should update the end date for: Perscriptions made in DoseSpot, Medications made in DoseSpot, and Medications made in OD
				string eRxGuidCur=listAllMedicationsForPatient[i].ErxGuid;
				if(!Erx.IsFromDoseSpot(eRxGuidCur) && !Erx.IsDoseSpotPatReported(eRxGuidCur) && !Erx.IsTwoWayIntegrated(eRxGuidCur))
				{
					continue;//This medication is not synced with DoseSpot, don't update.
				}
				if(listActiveMedicationPatNums.Contains(listAllMedicationsForPatient[i].MedicationPatNum)) {
					continue;//The medication is still active.
				}
				if(listAllMedicationsForPatient[i].DateStop.Year>1880) {
					continue;//The medication is already discontinued in Open Dental.
				}
				//The medication was discontinued inside the eRx interface.
				DoseSpotMedicationWrapper doseSpotMedication=listDoseSpotMedWrapper.FirstOrDefault(x => Erx.OpenDentalErxPrefix+x.MedicationId.ToString()==eRxGuidCur);
				if(doseSpotMedication==null) {
					doseSpotMedication=listDoseSpotMedWrapper.FirstOrDefault(x => Erx.DoseSpotPatReportedPrefix+x.MedicationId.ToString()==eRxGuidCur);
				}
				//Try to get the date stop.
				if(doseSpotMedication!=null && doseSpotMedication.DateInactive!=null) {
					listAllMedicationsForPatient[i].DateStop=doseSpotMedication.DateInactive.Value;
				}
				else {
					listAllMedicationsForPatient[i].DateStop=DateTime.Today.AddDays(-1);//Discontinue the medication as of yesterday so that it will immediately show as discontinued.
				}
				MedicationPats.Update(listAllMedicationsForPatient[i]);//Discontinue the medication inside OD to match what shows in the eRx interface.
				string medDescript=listAllMedicationsForPatient[i].MedDescript;
				if(listAllMedicationsForPatient[i].MedicationNum!=0) {
					medDescript=Medications.GetMedication(listAllMedicationsForPatient[i].MedicationNum).MedName;
				}
				SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,listAllMedicationsForPatient[i].PatNum,medDescript+" DoseSpot set inactive",LogSources.eRx);
			}
			if(onRxAdd!=null && listNewRx.Count!=0) {
				onRxAdd(listNewRx);
			}
			return true;
		}

		///<summary></summary>
		public static void SyncPrescriptionsToDoseSpot(string clinicID,string clinicKey,string userID,long patNum) {
			string token=DoseSpotREST.GetToken(userID,clinicID,clinicKey);
			//No need to check RemotingRole; no call to db.
			OIDExternal oidPatID=DoseSpot.GetDoseSpotPatID(patNum);
			if(oidPatID==null) {
				return;//We don't have a PatID from DoseSpot for this patient.  Therefore there is nothing to sync with.
			}
			List<MedicationPat> listAllMedicationsForPatient=MedicationPats.Refresh(patNum,true).FindAll(x => !Erx.IsFromDoseSpot(x.ErxGuid));
			if(listAllMedicationsForPatient.Count==0) {
				return;//There are no medications to send to DoseSpot.
			}
			foreach(MedicationPat medicationPat in listAllMedicationsForPatient) {
				//Medications originating from DS are filtered out when the list is retrieved.
				DoseSpotSelfReported doseSpotSelfReported=DoseSpotREST.MedicationPatToDoseSpotSelfReport(medicationPat);
				if(doseSpotSelfReported.DisplayName.IsNullOrEmpty()) {
					//Couldn't get a name from the medicationpat or the medication, don't send to DS without a name.
					continue;
				}
				if(doseSpotSelfReported.SelfReportedMedicationId>0) {
					//We were able to get the external ID for this self reported from the medicationpat, therefore we want to do an edit.
					try {
						DoseSpotREST.PutSelfReportedMedications(token,oidPatID.IDExternal,doseSpotSelfReported);
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
					medicationPat.ErxGuid=Erx.OpenDentalErxPrefix+DoseSpotREST.PostSelfReportedMedications(token,oidPatID.IDExternal,doseSpotSelfReported);
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
		public static string GetSingleSignOnQueryString(string clinicID,string clinicKey,string userID,string onBehalfOfUserId,Patient pat) {
			//No need to check RemotingRole; no call to db.
			//Pass in false for isQueryString because we will URLEncode the values below in QueryStringAddParameter.  It was a bug where we were double encoding the data.
			DoseSpotService.SingleSignOn sso=GetSingleSignOn(clinicID,clinicKey,userID,false);
			StringBuilder sb=new StringBuilder();
			QueryStringAddParameter(sb,"SingleSignOnCode",sso.SingleSignOnCode);
			QueryStringAddParameter(sb,"SingleSignOnUserId",POut.Int(sso.SingleSignOnUserId));
			QueryStringAddParameter(sb,"SingleSignOnUserIdVerify",sso.SingleSignOnUserIdVerify);
			QueryStringAddParameter(sb,"SingleSignOnClinicId",POut.Int(sso.SingleSignOnClinicId));
			if(!String.IsNullOrWhiteSpace(onBehalfOfUserId)) {
				QueryStringAddParameter(sb,"OnBehalfOfUserId",POut.String(onBehalfOfUserId));
			}
			if(pat==null) {
				QueryStringAddParameter(sb,"RefillsErrors",POut.Int(1));//Request transmission errors
			}
			else {
				OIDExternal oIDExternal=DoseSpot.GetDoseSpotPatID(pat.PatNum);
				if(oIDExternal!=null) {
					QueryStringAddParameter(sb,"PatientId",oIDExternal.IDExternal);
				}
			}
			return sb.ToString();
		}

		///<summary>Throws exceptions.
		///Gets the clinicID/clinicKey regarding the passed in clinicNum.
		///Will register the passed in clinicNum with DoseSpot if it isn't already.
		///Validates if the passed in doseSpotUserID is empty.</summary>
		public static void GetClinicIdAndKey(long clinicNum,string doseSpotUserID,Program programErx,List<ProgramProperty> listErxProperties
			,out string clinicID,out string clinicKey)
		{
			//No need to check RemotingRole; no call to db.
			clinicID="";
			clinicKey="";
			if(programErx==null) {
				programErx=Programs.GetCur(ProgramName.eRx);
			}
			if(listErxProperties==null) {
				listErxProperties=ProgramProperties.GetForProgram(programErx.ProgramNum)
					.FindAll(x => x.ClinicNum==clinicNum
						&& (x.PropertyDesc==Erx.PropertyDescs.ClinicID || x.PropertyDesc==Erx.PropertyDescs.ClinicKey));
			}
			ProgramProperty ppClinicID=listErxProperties.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			ProgramProperty ppClinicKey=listErxProperties.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			//If the current clinic doesn't have a clinic id/key, use a different clinic to make them.
			if(ppClinicID==null || string.IsNullOrWhiteSpace(ppClinicID.PropertyValue)
				|| ppClinicKey==null || string.IsNullOrWhiteSpace(ppClinicKey.PropertyValue))
			{
				throw new ODException(((clinicNum==0)?"HQ ":Clinics.GetAbbr(clinicNum)+" ")
					+Lans.g("DoseSpot","is missing a valid ClinicID or Clinic Key.  This should have been entered when setting up DoseSpot."));
			}
			else {
				clinicID=ppClinicID.PropertyValue;
				clinicKey=ppClinicKey.PropertyValue;
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
			Clinic clinicCur=GetClinicOrPracticeInfo(clinicNum);
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
			DoseSpotREST.PostClinic(token,clinicCur,out clinicIdNew,out clinicKeyNew);
			long erxProgramNum=Programs.GetCur(ProgramName.eRx).ProgramNum;
			List<ProgramProperty> listPropsForClinic=ProgramProperties.GetListForProgramAndClinic(erxProgramNum,clinicNum);
			ProgramProperty ppClinicID=listPropsForClinic.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			ProgramProperty ppClinicKey=listPropsForClinic.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			//Update the database with the new id/key.
			InsertOrUpdate(ppClinicID,erxProgramNum,Erx.PropertyDescs.ClinicID,clinicIdNew,clinicNum);
			InsertOrUpdate(ppClinicKey,erxProgramNum,Erx.PropertyDescs.ClinicKey,clinicKeyNew,clinicNum);
			//Ensure cache is not stale after setting the values.
			Cache.Refresh(InvalidType.Programs);
		}

		///<summary>Throws exceptions when validating clinic/practice info/provider.
		///Will register the passed in userNum if that user is not already registered.
		///Updates the passed in user's UserOdPref for DoseSpot User ID</summary>
		public static string GetUserID(Userod userCur,long clinicNum) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			Clinic clinicCur=GetClinicOrPracticeInfo(clinicNum);
			//At this point we know that we have a valid clinic/practice info and valid provider.
			Program programErx=Programs.GetCur(ProgramName.eRx);
			//Get the DoseSpotID for the current user
			UserOdPref userPrefDoseSpotID=GetDoseSpotUserIdFromPref(userCur.UserNum,clinicNum);
			//If the current user doesn't have a valid User ID, go retreive one from DoseSpot.
			if(userPrefDoseSpotID==null || string.IsNullOrWhiteSpace(userPrefDoseSpotID.ValueString)) {
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
				retVal=userPrefDoseSpotID.ValueString;
			}
			return retVal;
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
			List<ClinicErx> listClinicErxsToSend=ClinicErxs.GetWhere(x => x.EnabledStatus!=ErxStatus.Enabled);
			//Currently we do not have any intention of disabling clinics from HQ since there is no cost associated to adding a clinic.
			//Because of this, don't make extra web calls to check if HQ has tried to disable any clinics.
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("ErxClinicAccessRequest");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();//End reg key
				writer.WriteStartElement("RegKeyDisabledOverride");
				//Allow disabled regkeys to use eRx.  This functionality matches how we handle a disabled regkey for providererx
				//providererx in CustUpdates only cares that the regkey is valid and associated to a patnum in ODHQ
				writer.WriteString("true");
				writer.WriteEndElement();//End reg key disabled override
				foreach(ClinicErx clinicErx in listClinicErxsToSend) {
					writer.WriteStartElement("Clinic");
					writer.WriteAttributeString("ClinicDesc",clinicErx.ClinicDesc);
					writer.WriteAttributeString("EnabledStatus",((int)clinicErx.EnabledStatus).ToString());
					writer.WriteAttributeString("ClinicId",clinicErx.ClinicId);
					writer.WriteAttributeString("ClinicKey",clinicErx.ClinicKey);
					writer.WriteEndElement();//End Clinic
				}
				writer.WriteEndElement();//End ErxAccessRequest
			}
#if DEBUG
			OpenDentBusiness.localhost.Service1 updateService=new OpenDentBusiness.localhost.Service1();

#else
			OpenDentBusiness.customerUpdates.Service1 updateService=new OpenDentBusiness.customerUpdates.Service1();
			updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			bool isCacheRefreshNeeded=false;
			try {
				string result=updateService.GetClinicErxAccess(strbuild.ToString());
				XmlDocument doc=new XmlDocument();
				doc.LoadXml(result);
				XmlNodeList listNodes=doc.SelectNodes("//Clinic");
				for(int i=0;i<listNodes.Count;i++) {//Loop through clinics.
					XmlNode nodeClinic=listNodes[i];
					string clinicDesc="";
					string clinicId="";
					string clinicKey="";
					ErxStatus clinicEnabledStatus=ErxStatus.Disabled;
					for(int j=0;j<nodeClinic.Attributes.Count;j++) {//Loop through the attributes for the current provider.
						XmlAttribute attribute=nodeClinic.Attributes[j];
						if(attribute.Name=="ClinicDesc") {
							clinicDesc=attribute.Value;
						}
						else if(attribute.Name=="EnabledStatus") {
							clinicEnabledStatus=PIn.Enum<ErxStatus>(PIn.Int(attribute.Value));
						}
						else if(attribute.Name=="ClinicId") {
							clinicId=attribute.Value;
						}
						else if(attribute.Name=="ClinicKey") {
							clinicKey=attribute.Value;
						}
					}
					ClinicErx oldClinicErx=ClinicErxs.GetByClinicIdAndKey(clinicId,clinicKey);
					if(oldClinicErx==null) {
						continue;
					}
					ClinicErx clinicErxFromHqCur=oldClinicErx.Copy();
					clinicErxFromHqCur.EnabledStatus=clinicEnabledStatus;
					clinicErxFromHqCur.ClinicId=clinicId;
					clinicErxFromHqCur.ClinicKey=clinicKey;
					//Dont need to set the ErxType here because it's not something that can be changed by HQ.
					if(ClinicErxs.Update(clinicErxFromHqCur,oldClinicErx)) {
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
			Program programErx=Programs.GetCur(ProgramName.eRx);
			UserOdPref retVal=UserOdPrefs.GetByCompositeKey(userNum,programErx.ProgramNum,UserOdFkeyType.Program,clinicNum);
			if(clinicNum!=0 && retVal.IsNew || string.IsNullOrWhiteSpace(retVal.ValueString)) {
				retVal=UserOdPrefs.GetByCompositeKey(userNum,programErx.ProgramNum,UserOdFkeyType.Program,0);//Try the default userodpref if the clinic specific one is empty.
			}
			return retVal;
		}
		
		public static OIDExternal CreateOIDForPatient(int doseSpotPatID,long patNum) {
			OIDExternal oidExternalForPat=new OIDExternal();
			oidExternalForPat.rootExternal=DoseSpot.GetDoseSpotRoot()+"."+POut.Int((int)IdentifierType.Patient);
			oidExternalForPat.IDExternal=doseSpotPatID.ToString();
			oidExternalForPat.IDInternal=patNum;
			oidExternalForPat.IDType=IdentifierType.Patient;
			OIDExternals.Insert(oidExternalForPat);
			return oidExternalForPat;
		}

		///<summary>Creates ClinicErx entries for every clinic that has DoseSpot ClinicID/ClinicKey values.
		///This will be used when sending the clinics to ODHQ for enabling/disabling.
		///This will create ClinicNum 0 entries, thus supporting offices without clinics enabled, as well as clinics using the "Headquarters" clinic.</summary>
		private static void MakeClinicErxsForDoseSpot() {
			long programNum=Programs.GetCur(ProgramName.eRx).ProgramNum;
			List<ProgramProperty> listClinicIDs=ProgramProperties.GetWhere(x => x.ProgramNum==programNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			List<ProgramProperty> listClinicKeys=ProgramProperties.GetWhere(x => x.ProgramNum==programNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			bool isRefreshNeeded=false;
			foreach(ProgramProperty ppClinicId in listClinicIDs) {
				ProgramProperty ppClinicKey=listClinicKeys.FirstOrDefault(x => x.ClinicNum==ppClinicId.ClinicNum);
				if(ppClinicKey==null || string.IsNullOrWhiteSpace(ppClinicKey.PropertyValue) || string.IsNullOrWhiteSpace(ppClinicId.PropertyValue)) {
					continue;
				}
				ClinicErx clinicErxCur=ClinicErxs.GetByClinicNum(ppClinicId.ClinicNum);
				if(clinicErxCur==null) {
					clinicErxCur=new ClinicErx();
					clinicErxCur.ClinicNum=ppClinicId.ClinicNum;
					clinicErxCur.ClinicId=ppClinicId.PropertyValue;
					clinicErxCur.ClinicKey=ppClinicKey.PropertyValue;
					clinicErxCur.ClinicDesc=Clinics.GetDesc(ppClinicId.ClinicNum);
					clinicErxCur.EnabledStatus=ErxStatus.PendingAccountId;
					ClinicErxs.Insert(clinicErxCur);
				}
				else {
					clinicErxCur.ClinicId=ppClinicId.PropertyValue;
					clinicErxCur.ClinicKey=ppClinicKey.PropertyValue;
					ClinicErxs.Update(clinicErxCur);
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
			Clinic clinicCur=Clinics.GetClinic(clinicNum);
			bool isPractice=false;
			if(clinicCur==null) {//Make a fake ClinicNum 0 clinic containing practice info for validation/registering a new clinician if needed.
				clinicCur=new Clinic();
				clinicCur.Abbr=PrefC.GetString(PrefName.PracticeTitle);
				clinicCur.Address=PrefC.GetString(PrefName.PracticeAddress);
				clinicCur.Address2=PrefC.GetString(PrefName.PracticeAddress2);
				clinicCur.City=PrefC.GetString(PrefName.PracticeCity);
				clinicCur.State=PrefC.GetString(PrefName.PracticeST);
				clinicCur.Zip=PrefC.GetString(PrefName.PracticeZip);
				clinicCur.Phone=PrefC.GetString(PrefName.PracticePhone);
				clinicCur.Fax=PrefC.GetString(PrefName.PracticeFax);
				isPractice=true;
			}
			ValidateClinic(clinicCur,isPractice);
			//At this point we know the clinic is valid since we did not throw an exception.
			return clinicCur;
		}

		///<summary>Inserts a new ProgramProperty into the database if the passed in ppCur is null.
		///If ppCur is not null, it just sets the PropertyValue and updates.</summary>
		private static void InsertOrUpdate(ProgramProperty ppCur,long programNum,string propDesc,string propValue,long clinicNum) {
			if(ppCur==null) {
				ppCur=new ProgramProperty();
				ppCur.ProgramNum=programNum;
				ppCur.PropertyDesc=propDesc;
				ppCur.PropertyValue=propValue;
				ppCur.ClinicNum=clinicNum;
				ProgramProperties.Insert(ppCur);
			}
			else {
				ppCur.PropertyValue=propValue;
				ProgramProperties.Update(ppCur);
			}
		}

		///<summary>Generates a DoseSpot clinic based on the passed in clinic.
		///The returned DoseSpot clinic does not have the ClinicID set.</summary>
		private static DoseSpotService.Clinic MakeDoseSpotClinic(Clinic clinic) {
			DoseSpotService.Clinic retVal=new DoseSpotService.Clinic();
			retVal.Address1=clinic.Address;
			retVal.Address2=clinic.Address2;
			retVal.City=clinic.City;
			retVal.ClinicName=clinic.Abbr;
			retVal.ClinicNameLongForm=clinic.Description;
			retVal.PrimaryFax=clinic.Fax;
			retVal.PrimaryPhone=clinic.Phone;
			//This is a required field but there is no way to set this value in Open Dental.
			//Since it's a clinic it seems safe to assume that it's a work number
			retVal.PrimaryPhoneType="Work";
			retVal.State=clinic.State.ToUpper();
			retVal.ZipCode=clinic.Zip;
			return retVal;
		}

		///<summary>Makes a clinician out of the tables in OD that make up what DoseSpot considers a clinician.
		///Clinic in this instance can also be a fake ClinicNum 0 clinic that contains practice information.
		///When an employee is the reason for making a clinician, isProxyClinician must be set to true.
		///If isProxyClinician is incorrectly set, the employee will have access to send prescriptions in DoseSpot.</summary>
		private static DoseSpotService.Clinician MakeDoseSpotClinician(Provider prov,Clinic clinic,string emailAddress,bool isProxyClinician) {
			DoseSpotService.Clinician retVal=new DoseSpotService.Clinician();
			retVal.Address1=clinic.Address;
			retVal.Address2=clinic.Address2;
			retVal.City=clinic.City;
			retVal.DateOfBirth=prov.Birthdate;
			retVal.DEANumber=ProviderClinics.GetDEANum(prov.ProvNum,clinic.ClinicNum);//
			retVal.Email=emailAddress;//Email should have been validated by now.
			retVal.FirstName=prov.FName;
			retVal.Gender="Unknown";//This is a required field but we do not store this information.
			retVal.IsProxyClinician=isProxyClinician;
			//retVal.IsReportingClinician=false;//This field was not present in the API documentation and weren't required when testing.
			retVal.LastName=prov.LName;
			retVal.MiddleName=prov.MI;
			retVal.NPINumber=prov.NationalProvID;
			retVal.PrimaryFax=clinic.Fax;
			retVal.PrimaryPhone=clinic.Phone;
			//This is a required field but there is no way to set this value in Open Dental.
			//Since it's a clinic phone number it seems safe to assume that it's a work number
			retVal.PrimaryPhoneType="Work";
			retVal.State=clinic.State.ToUpper();
			retVal.Suffix=prov.Suffix;
			retVal.ZipCode=clinic.Zip;
			return retVal;
		}

		///<summary>If isQueryString is false, the parameter format will assume json</summary>
		private static StringBuilder AddParameter(StringBuilder queryString,string paramName,string paramValue,bool isQueryString) {
			if(isQueryString) {
				return QueryStringAddParameter(queryString,paramName,paramValue);
			}
			return JsonAddParameter(queryString,paramName,paramValue);
		}

		///<summary>Adds a query parameter and that parameter's value to the string builder</summary>
		private static StringBuilder QueryStringAddParameter(StringBuilder queryString,string paramName,string paramValue) {
			queryString.Append("&"+paramName+"=");
			if(paramName!=null) {
				queryString.Append(Uri.EscapeDataString(paramValue));
			}
			return queryString;
		}

		///<summary>Adds a query parameter and that parameter's value to the string builder</summary>
		private static StringBuilder JsonAddParameter(StringBuilder jsonString,string paramName,string paramValue) {
			if(paramName!=null && !string.IsNullOrWhiteSpace(paramValue)) {
				jsonString.AppendLine("\t"+"\""+paramName+"\""+":"+"\""+paramValue+"\",");
			}
			return jsonString;
		}

		///<summary>Generates a SingleSignOn to use for DoseSpot API calls.</summary>
		private static DoseSpotService.SingleSignOn GetSingleSignOn(string clinicID,string clinicKey,string userID,bool isQueryString) {
			clinicID=clinicID.Trim();
			clinicKey=clinicKey.Trim();
			userID=userID.Trim();
			string ssoCode=CreateSsoCode(clinicKey,isQueryString);
			string ssoUserIdVerify=CreateSsoUserIdVerify(clinicKey,userID,isQueryString);
			DoseSpotService.SingleSignOn retVal=new DoseSpotService.SingleSignOn();
			retVal.SingleSignOnClinicId=PIn.Int(clinicID);
			retVal.SingleSignOnUserId=PIn.Int(userID);
			retVal.SingleSignOnPhraseLength=32;
			retVal.SingleSignOnCode=ssoCode;
			retVal.SingleSignOnUserIdVerify=ssoUserIdVerify;
			return retVal;
		}

		///<summary>Throws exceptions if anything about the provider is invalid.
		///Not throwing an exception means that the provider is valid</summary>
		public static void ValidateProvider(Provider prov,long clinicNum=0) {
			//No need to check RemotingRole; no call to db.
			if(prov==null) {
				throw new Exception("Invalid provider.");
			}
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,clinicNum);
			StringBuilder sbErrors=new StringBuilder();
			if(prov.IsErxEnabled==ErxEnabledStatus.Disabled) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Erx is disabled for provider.  "
					+"To enable, edit provider in Lists | Providers and acknowledge Electronic Prescription fees."));
			}
			if(prov.IsHidden) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Provider is hidden"));
			}
			if(prov.IsNotPerson) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Provider must be a person"));
			}
			string fname=prov.FName.Trim();
			if(fname=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","First name missing"));
			}
			if(Regex.Replace(fname,"[^A-Za-z\\- ]*","")!=fname) {
				sbErrors.AppendLine(Lans.g("DoseSpot","First name can only contain letters, dashes, or spaces"));
			}
			string lname=prov.LName.Trim();
			if(lname=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","Last name missing"));
			}
			string deaNum="";
			if(provClinic!=null) {
				deaNum=provClinic.DEANum;
			}
			if(deaNum.ToLower()!="none" && !Regex.IsMatch(deaNum,"^[A-Za-z]{2}[0-9]{7}$") ) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Provider DEA Number must be 2 letters followed by 7 digits.  If no DEA Number, enter NONE."));
			}
			string npi=Regex.Replace(prov.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
			if(npi.Length!=10) {
				sbErrors.AppendLine(Lans.g("DoseSpot","NPI must be exactly 10 digits"));
			}
			if(provClinic==null || provClinic.StateLicense=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","State license missing"));
			}
			if(provClinic==null || !USlocales.IsValidAbbr(provClinic.StateWhereLicensed)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","State where licensed invalid"));
			}
			if(prov.Birthdate.Year<1880) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Birthdate invalid"));
			}
			if(sbErrors.ToString().Length>0) {
				string clinicText="";
				if(PrefC.HasClinicsEnabled) {
					clinicText=" "+Lans.g("DoseSpot","in clinic")+" "+(clinicNum==0?Lans.g("DoseSpot","Headquarters"):Clinics.GetAbbr(clinicNum));
				}
				throw new ODException(Lans.g("DoseSpot","Issues found for provider")+" "+prov.Abbr+clinicText+":\r\n"+sbErrors.ToString());
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
			StringBuilder sbErrors=new StringBuilder();
			if(clinic.IsHidden) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Clinic is hidden"));
			}
			if(string.IsNullOrWhiteSpace(clinic.Phone)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Phone number is blank"));
			}
			else if(!IsPhoneNumberValid(clinic.Phone)) {//If the phone number isn't valid, DoseSpot will break.
				sbErrors.AppendLine(Lans.g("DoseSpot","Phone number invalid: ")+clinic.Phone);
			}
			if(string.IsNullOrWhiteSpace(clinic.Fax)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Fax number is blank"));
			}
			else if(!IsPhoneNumberValid(clinic.Fax)) {//If the fax number isn't valid, DoseSpot will break.
				sbErrors.AppendLine(Lans.g("DoseSpot","Fax number invalid: ")+clinic.Fax);
			}
			if(clinic.Address=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","Address is blank"));
			}
			if(IsAddressPOBox(clinic.Address)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Address cannot be a PO BOX"));
			}
			if(clinic.City=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","City is blank"));
			}
			if(string.IsNullOrWhiteSpace(clinic.State)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","State abbreviation is blank"));
			}
			else if(clinic.State.Length<=2 && (clinic.State=="" || (clinic.State!="" && !USlocales.IsValidAbbr(clinic.State)))) {
				//Don't validate state values that are longer than 2 characters.
				sbErrors.AppendLine(Lans.g("DoseSpot","State abbreviation is invalid"));
			}
			if(clinic.Zip=="" && !Regex.IsMatch(clinic.Zip,@"^([0-9]{9})$|^([0-9]{5}-[0-9]{4})$|^([0-9]{5})$")) {//Blank, or #####, or #####-####, or #########
				sbErrors.AppendLine(Lans.g("DoseSpot","Zip invalid."));
			}
			if(sbErrors.ToString().Length>0) {
				if(isPractice) {
					throw new ODException(Lans.g("DoseSpot","Issues found for practice information:")+"\r\n"+sbErrors.ToString());
				}
				throw new ODException(Lans.g("DoseSpot","Issues found for clinic")+" "+clinic.Abbr+":\r\n"+sbErrors.ToString());
			}
		}

		///<summary>Search the given address for a PO Box format.  Searches for the string "PO" case insensitive with or without periods. Ignores any match that has a non-space character before it (ex. "123 Ampo St" should be ignored even though it has 'po ')</summary>
		public static bool IsAddressPOBox(string address) {
			string regex=@".*( |^)P\.?O\.? .*";
			return Regex.IsMatch(address,regex,RegexOptions.IgnoreCase);
		}

		///<summary>Throws exceptions for invalid Patient data.</summary>
		public static void ValidatePatientData(Patient pat) {
			string primaryPhone=GetPhoneAndType(pat,0,out string phoneType);
			StringBuilder sbErrors=new StringBuilder();
			if(pat.FName=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","Missing first name."));
			}
			if(pat.LName=="") {
				sbErrors.AppendLine(Lans.g("DoseSpot","Missing last name."));
			}
			if(pat.Birthdate.Year<1880) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Missing birthdate."));
			}
			if(pat.Birthdate>DateTime.Today) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Invalid birthdate."));
			}
			if(pat.Address.Length==0) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Missing address."));
			}
			if(pat.City.Length<2) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Invalid city."));
			}
			if(string.IsNullOrWhiteSpace(pat.State)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Blank state abbreviation."));
			}
			else if(pat.State.Length<=2 && !USlocales.IsValidAbbr(pat.State)) {//Don't validate state values that are longer than 2 characters.
				sbErrors.AppendLine(Lans.g("DoseSpot","Invalid state abbreviation."));
			}
			if(string.IsNullOrWhiteSpace(pat.Zip)) {
				sbErrors.AppendLine(Lans.g("DoseSpot","Blank zip."));
			}
			else if(!Regex.IsMatch(pat.Zip,@"^([0-9]{9})$|^([0-9]{5}-[0-9]{4})$|^([0-9]{5})$")) {//#####, #####-####, or #########
				sbErrors.AppendLine(Lans.g("DoseSpot","Invalid zip."));
			}
			if(!IsPhoneNumberValid(primaryPhone)) {//If the primary phone number isn't valid, DoseSpot will break.
				sbErrors.AppendLine(Lans.g("DoseSpot","Invalid phone number: ")+primaryPhone);
			}
			if(sbErrors.ToString().Length>0) {
				throw new ODException(Lans.g("DoseSpot","Issues found for current patient:")+"\r\n"+sbErrors.ToString());
			}
		}

		///<summary>If isQueryString is false, this will be built as a json object</summary>
		public static string GetPatientData(Patient pat,bool isQueryString=true) {
			ValidatePatientData(pat);
			string primaryPhone=GetPhoneAndType(pat,0,out string phoneType);
			StringBuilder sb=new StringBuilder();
			OIDExternal oidExternalPat=DoseSpot.GetDoseSpotPatID(pat.PatNum);
			if(oidExternalPat!=null) {
				sb=AddParameter(sb,"PatientId",oidExternalPat.IDExternal,isQueryString);
			}
			sb=AddParameter(sb,"Prefix",Tidy(pat.Title,10),isQueryString);
			sb=AddParameter(sb,"FirstName",Tidy(pat.FName,35),isQueryString);
			sb=AddParameter(sb,"MiddleName",Tidy(pat.MiddleI,35),isQueryString);
			sb=AddParameter(sb,"LastName",Tidy(pat.LName,35),isQueryString);
			sb=AddParameter(sb,"Suffix","",isQueryString);//I don't see where we store suffixes.
			sb=AddParameter(sb,"DateOfBirth",pat.Birthdate.ToShortDateString(),isQueryString);
			sb=AddParameter(sb,"Gender",pat.Gender.ToString(),isQueryString);
			sb=AddParameter(sb,"Address1",Tidy(pat.Address,35),isQueryString);
			sb=AddParameter(sb,"Address2",Tidy(pat.Address2,35),isQueryString);
			sb=AddParameter(sb,"City",Tidy(pat.City,35),isQueryString);
			sb=AddParameter(sb,"State",pat.State,isQueryString);
			sb=AddParameter(sb,"ZipCode",pat.Zip,isQueryString);
			sb=AddParameter(sb,"PrimaryPhone",primaryPhone,isQueryString);
			sb=AddParameter(sb,"PrimaryPhoneType",phoneType,isQueryString);
			sb=AddParameter(sb,"PhoneAdditional1",GetPhoneAndType(pat,1,out phoneType),isQueryString);
			sb=AddParameter(sb,"PhoneAdditionalType1",phoneType,isQueryString);
			sb=AddParameter(sb,"PhoneAdditional2",GetPhoneAndType(pat,2,out phoneType),isQueryString);
			sb=AddParameter(sb,"PhoneAdditionalType2",phoneType,isQueryString);
			//sb=AddParameter(sb,"Height",Height,isQueryString);
			//sb=AddParameter(sb,"Weight",Weight,isQueryString);
			//sb=AddParameter(sb,"HeightMetric",HeightMetric,isQueryString);
			//sb=AddParameter(sb,"WeightMetric",WeightMetric,isQueryString);
			return sb.ToString();
		}

		///<summary>Set patient medication history consent for DoseSpot. Optionally pass in parameters for programErx and listProgramProperties.
		///If left null they will be evaluated and set.  Throws exceptions.</summary>
		public static void SetMedicationHistConsent(Patient pat,long clinicNum,Program programErx=null,List<ProgramProperty> listProgramProperties=null) {
			ValidatePatientData(pat);
			//Get Token
			string doseSpotUserID=GetUserID(Security.CurUser,clinicNum);
			GetClinicIdAndKey(clinicNum,doseSpotUserID,programErx,listProgramProperties,out string doseSpotClinicID,out string doseSpotClinicKey);
			string token=DoseSpotREST.GetToken(doseSpotUserID,doseSpotClinicID,doseSpotClinicKey);
			//Get DoseSpotPatID
			OIDExternal oidPatID=GetDoseSpotPatID(pat.PatNum);
			if(oidPatID==null) {
				//Create a DoseSpot patient and save it for future uses with this patient.
				oidPatID=CreateOIDForPatient(PIn.Int(DoseSpotREST.AddPatient(token,pat)),pat.PatNum);
			}
			else {
				DoseSpotREST.EditPatient(token,pat,oidPatID.IDExternal);
			}
			//POST patient consent to DoseSpot
			DoseSpotREST.PostMedicationHistoryConsent(token,oidPatID.IDExternal);
		}

		private static string Tidy(string value,int length) {
			if(value.Length<=length) {
				return value;
			}
			return value.Substring(0,length);
		}

		///<summary>Valid values for ordinal are 0 (for primary),1, or 2.</summary>
		internal static string GetPhoneAndType(Patient pat,int ordinal,out string phoneType) {
			List<string> listTypes=new List<string>();
			List<string> listPhones=new List<string>();
			if(IsPhoneNumberValid(pat.HmPhone)) {
				listTypes.Add("Home");
				listPhones.Add(pat.HmPhone);
			}
			if(IsPhoneNumberValid(pat.WirelessPhone)) {
				listTypes.Add("Cell");
				listPhones.Add(pat.WirelessPhone);
			}
			if(IsPhoneNumberValid(pat.WkPhone)) {
				listTypes.Add("Work");
				listPhones.Add(pat.WkPhone);
			}
			if(ordinal >= listPhones.Count) {
				phoneType="";
				return "";
			}
			phoneType=listTypes[ordinal];
			string retVal=listPhones[ordinal].Replace("(","").Replace(")","").Replace("-","");//remove all formatting as DoseSpot doesn't allow it.
			if(retVal.Length==11 && retVal[0]=='1') {
				retVal=retVal.Substring(1);//Remove leading 1 from phone number since DoseSpot thinks that invalid.
			}
			return retVal;
		}

		public static bool IsPhoneNumberValid(string phoneNumber) {
			string patternPhone=@"^1?\s*-?\s*(\d{3}|\(\s*\d{3}\s*\))\s*-?\s*\d{3}\s*-?\s*\d{4}(X\d{0,9})?";
			Regex pattern=new Regex(patternPhone, RegexOptions.IgnoreCase);
			if(phoneNumber!=null) {
				phoneNumber=phoneNumber.Trim();
			}
			if(string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length>=35) {//Max length of 35 is what the DoseSpot example app checks for, there is no documentation supporting it.
				return false;
			}
			if(!pattern.IsMatch(phoneNumber)) {//The regex was taken directly from the DoseSpot example app
				return false;
			}
			string phoneDigits=Regex.Replace(phoneNumber,@"[^0-9]","");//Remove all non-digit characters.
			//Per DoseSpot on 11/15/18, any number starting with 0 or 1 will be rejected by SureScripts
			if(phoneDigits.StartsWith("0") || phoneDigits.StartsWith("1")) {
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
			using(SHA512 shaM=new SHA512Managed()) {//Use SHA512 to hash the byte value you just received
				arrayHash=shaM.ComputeHash(arrayBytesToHash);
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


		public static void GetNotificationCounts(string authToken,out int RefillRequests,out int TransactionErrors,out int PendingPrescriptions) {
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
			RefillRequests=resObj.RefillRequestsCount;
			TransactionErrors=resObj.TransactionErrorsCount;
			PendingPrescriptions=resObj.PendingPrescriptionsCount;
		}
		#endregion
		#region POST

		///<summary></summary>
		public static string AddPatient(string authToken,Patient pat) {
			Vitalsign vitalsign=Vitalsigns.GetOneWithValidHeightAndWeight(pat.PatNum);
			string primaryPhone=DoseSpot.GetPhoneAndType(pat,0,out string phoneType);
			if(pat.Age<18 && vitalsign==null) {
				throw new ODException(Lans.g("DoseSpot","All patients under 18 must have a vital sign reading that includes height and weight. "
					+"To add a vital sign to the patient, go to the 'Chart' module, and double click on the pink medical area. "
					+"There is a tab in the next window labeled vitals, click it and add a vital reading that includes height and weight."));
			}
			string body=JsonConvert.SerializeObject(
				new {
					FirstName=pat.FName.Trim(),
					LastName=pat.LName.Trim(),
					DateOfBirth=pat.Birthdate,
					Gender=pat.Gender+1,
					Address1=pat.Address.Trim(),
					City=pat.City.Trim(),
					State=pat.State.Trim(),
					ZipCode=pat.Zip.Trim(),
					PrimaryPhone=primaryPhone,
					PrimaryPhoneType=phoneType,
					Active="true"
				});
			if(vitalsign!=null) {
				body=JsonConvert.SerializeObject(
				new {
					FirstName = pat.FName.Trim(),
					LastName = pat.LName.Trim(),
					DateOfBirth = pat.Birthdate,
					Gender = pat.Gender+1,
					Address1 = pat.Address.Trim(),
					City = pat.City.Trim(),
					State = pat.State.Trim(),
					ZipCode = pat.Zip.Trim(),
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
		public static string EditPatient(string authToken,Patient pat,string doseSpotPatId) {
			Vitalsign vitalsign=Vitalsigns.GetOneWithValidHeightAndWeight(pat.PatNum);
			string primaryPhone=DoseSpot.GetPhoneAndType(pat,0,out string phoneType);
			if(pat.Age<18 && vitalsign==null) {
				throw new ODException(Lans.g("DoseSpot","All patients under 18 must have a vital sign reading that includes height and weight. "
					+"To add a vital sign to the patient, go to the 'Chart' module, and double click on the pink medical area. "
					+"There is a tab in the next window labeled vitals, click it and add a vital reading that includes height and weight."));
			}
			string body=JsonConvert.SerializeObject(
				new {
					FirstName=pat.FName.Trim(),
					LastName=pat.LName.Trim(),
					DateOfBirth=pat.Birthdate,
					Gender=pat.Gender+1,
					Address1=pat.Address.Trim(),
					City=pat.City.Trim(),
					State=pat.State.Trim(),
					ZipCode=pat.Zip.Trim(),
					PrimaryPhone=primaryPhone.Trim(),
					PrimaryPhoneType=phoneType.Trim(),
					Active="true"
				});
			if(vitalsign!=null) {
				body=JsonConvert.SerializeObject(
				new {
					FirstName = pat.FName.Trim(),
					LastName = pat.LName.Trim(),
					DateOfBirth = pat.Birthdate,
					Gender = pat.Gender+1,
					Address1 = pat.Address.Trim(),
					City = pat.City.Trim(),
					State = pat.State.Trim(),
					ZipCode = pat.Zip.Trim(),
					PrimaryPhone = primaryPhone.Trim(),
					PrimaryPhoneType = phoneType.Trim(),
					Weight=vitalsign.Weight,
					WeightMetric=1,
					Height=vitalsign.Height,
					HeightMetric=1,
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
		public static void PostAddClinicToClinicGroup(string authToken, List<string> listClinicIDs, string clinicGroupName) {
			string body=JsonConvert.SerializeObject(
				new {
					ClinicIDs=String.Join(",\r\n",listClinicIDs),
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
						DisplayName=doseSpotSelfReported.DisplayName.Trim(),
						Status=(int)doseSpotSelfReported.MedicationStatus,
						InactiveDate=doseSpotSelfReported.DateInactive,
						Comment=doseSpotSelfReported.Comment.Trim()
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
		private static T Request<T>(ApiRoute route,HttpMethod method,string authHeader,string body,T responseType,string acceptType = "application/json",params string[] listRouteIDs) {
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.Accept]=acceptType;
				client.Headers[HttpRequestHeader.ContentType]=acceptType;
				client.Headers[HttpRequestHeader.Authorization]=authHeader;
				client.Encoding=UnicodeEncoding.UTF8;
				//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
				try {
					string res="";
					//Only GET and POST are supported currently.
					if(method==HttpMethod.Get) {
						res=client.DownloadString(GetApiUrl(route,listRouteIDs));
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(GetApiUrl(route,listRouteIDs),HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(GetApiUrl(route,listRouteIDs),HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+method.Method);
					}
					if(ODBuild.IsDebug()) {
						if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
							return (T)Convert.ChangeType(res,typeof(T));
						}
					}
					return JsonConvert.DeserializeAnonymousType(res,responseType);
				}
				catch(WebException wex) {
					if(!(wex.Response is HttpWebResponse)) {
						throw new ODException(Lans.g("DoseSpot","Could not connect to the DoseSpot server:")+"\r\n"+wex.Message,wex);
					}
					string res="";
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("DoseSpot","Invalid DoseSpot credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
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
		private static string GetApiUrl(ApiRoute route,params string[] listRouteIDs) {
			string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.DoseSpotURL,"https://my.dosespot.com/webapi");
			if(ODBuild.IsDebug()) {
				apiUrl="https://my.staging.dosespot.com/webapi";
			}
			switch(route) {
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
					apiUrl+=$"/api/patients/{listRouteIDs[0]}";
					break;
				case ApiRoute.GetNotificationCounts:
					apiUrl+=$"/api/notifications/counts";
					break;
				case ApiRoute.GetPharmacy:
					//routeId[0]=pharmacyId
					apiUrl+=$"/api/pharmacies/{listRouteIDs[0]}";
					break;
				case ApiRoute.GetPrescriptions:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{listRouteIDs[0]}/prescriptions";
					break;
				case ApiRoute.GetSelfReportedMedications:
					apiUrl+=$"/api/patients/{listRouteIDs[0]}/selfReportedMedications";
					break;
				case ApiRoute.LogMedicationHistoryConsent:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{listRouteIDs[0]}/logMedicationHistoryConsent";
					break;
				case ApiRoute.PostClinic:
					apiUrl+=$"/api/clinics";
					break;
				case ApiRoute.PostClinicGroup:
					apiUrl+=$"/api/clinics/clinicGroup";
					break;
				case ApiRoute.PutSelfReportedMedications:
					//routeId[0]=PatientId, routeId[1]=selfReportedMedicationId
					apiUrl+=$"/api/patients/{listRouteIDs[0]}/selfReportedMedications/freetext/{listRouteIDs[1]}";
					break;
				case ApiRoute.PostSelfReportedMedications:
					//routeId[0]=PatientId
					apiUrl+=$"/api/patients/{listRouteIDs[0]}/selfReportedMedications/freetext";
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
		public static DoseSpotSelfReported MedicationPatToDoseSpotSelfReport(MedicationPat medPat) {
			DoseSpotSelfReported medSelfReported=new DoseSpotSelfReported();
			medSelfReported.SelfReportedMedicationId=0;
			//If we have the ErxGuid then it has already been sent to DS.
			if(!medPat.ErxGuid.IsNullOrEmpty()) {
				//Remove any possible prefixes from the ErxGuid. It will either be a DS prefix, an OD prefix, or have no prefix at all if it came from an Rx.
				int ID=0;
				string guid=medPat.ErxGuid;
				//If it has the unsent prefix we want to intentionally set the ID to 0 so we post a new self reported and get an ID back from DS.
				if(!guid.StartsWith(Erx.UnsentPrefix)) {
					guid=Regex.Replace(guid,Erx.DoseSpotPatReportedPrefix,"");//Remove DS prefix.
					guid=Regex.Replace(guid,Erx.OpenDentalErxPrefix,"");//Remove OD prefix.
					int.TryParse(guid,out ID);
				}
				medSelfReported.SelfReportedMedicationId=ID;
			}
			//Set the DisplayName.
			if(String.IsNullOrEmpty(medPat.MedDescript) && medPat.MedicationNum!=0) {
				Medication med=Medications.GetMedication(medPat.MedicationNum);
				medSelfReported.DisplayName=med.MedName;
			}
			else {
				medSelfReported.DisplayName=medPat.MedDescript;
			}
			//Strip any newlines before sending. Newlines cause parsing errors for the DoseSpot API. The changes to this note will be synced back into OD when the eRx window is closed.
			medSelfReported.Comment=Regex.Replace(medPat.PatNote,@"\n|\r"," ");
			//500 characters is the max size for the comment field. Going over 500 will return a 400 error from the DoseSpot API.
			if(medSelfReported.Comment.Length>500) {
				SecurityLogs.MakeLogEntry(Permissions.LogDoseSpotMedicationNoteEdit,medPat.PatNum,"Medication patient note automatically reduced to 500 characters for sending to DoseSpot. Original note: "+"\n"
					+medPat.PatNote);
				medSelfReported.Comment=medSelfReported.Comment.Substring(0,500);
			}
			//Set the MedicationStatus.
			medSelfReported.MedicationStatus=MedicationStatus.Discontinued;
			if(medPat.DateStop.Year<1880 || medPat.DateStop>=DateTime.Today) {
				medSelfReported.MedicationStatus=MedicationStatus.Active;
			}
			else if(medPat.DateStop<DateTime.Today) {
				medSelfReported.MedicationStatus=MedicationStatus.Completed;
				medSelfReported.DateInactive=medPat.DateStop;
				//A comment is required when a medication has been discontinued (figured out through testing, not in docs)
				if(medSelfReported.Comment.IsNullOrEmpty()) {
					//We were infinitely adding this note which was causing the comment to be greater than 500 characters and caused a 400 from DS.
					medSelfReported.Comment="Discontinued in Open Dental";
				}
			}
			return medSelfReported;
		}

	}

	///<summary>This is a class to reflect the response object from DoseSpot's RESTful JSON objects.  
	///Fields are nullable due to DoseSpot returning null values in some of their fields during testing, 
	///as well as to safeguard against missing fields in the future.</summary>
	public class DoseSpotPrescription {
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
		readonly DoseSpotSelfReported Medication;
		readonly DoseSpotPrescription Prescription;

		///<summary>True is a self reported medication, false implies a prescription.</summary>
		public bool IsSelfReported {
			get {
				return Medication!=null;
			}
		}
		public DateTime? DateInactive {
			get {
				if(IsSelfReported) {
					return Medication.DateInactive;
				}
				return Prescription.DateInactive;
			}
		}
		public string DisplayName {
			get {
				if(IsSelfReported) {
					return Medication.DisplayName;
				}
				return Prescription.DisplayName;
			}
		}
		public DateTime? DateReported {
			get {
				if(IsSelfReported) {
					return Medication.DateReported;
				}
				return Prescription.EffectiveDate;
			}
		}
		public string GenericProductName {
			get {
				if(IsSelfReported) {
					return Medication.GenericProductName;
				}
				return Prescription.GenericProductName;
			}
		}
		public DateTime? DateLastFilled {
			get {
				if(IsSelfReported) {
					return null;
				}
				return Prescription.LastFillDate;
			}
		}
		public long? MedicationId {
			get {
				if(IsSelfReported) {
					return Medication.SelfReportedMedicationId;
				}
				return Prescription.PrescriptionId;
			}
		}
		public DoseSpotREST.MedicationStatus MedicationStatus {
			get {
				if(IsSelfReported) {
					return Medication.MedicationStatus;
				}
				return Prescription.MedicationStatus;
			}
		}
		public int? PharmacyId {
			get {
				if(IsSelfReported) {
					return null;
				}
				return Prescription.PharmacyId;
			}
		}
		public string Directions {
			get {
				if(IsSelfReported) {
					return Medication.Comment;
				}
				return Prescription.Directions;
			}
		}
		public string RxNotes {
			get {
				if(IsSelfReported) {
					return null;
				}
				return Prescription.PharmacyNotes;
			}
		}
		public int? PrescriberId {
			get {
				if(IsSelfReported) {
					return null;
				}
				return Prescription.PrescriberId;
			}
		}
		public DoseSpotREST.PrescriptionStatus PrescriptionStatus {
			get {
				if(IsSelfReported) {
					//This makes it fall through to the base case and do nothing.
					return DoseSpotREST.PrescriptionStatus.NotAPresciption;
				}
				return Prescription.Status;
			}
		}
		public string Quantity {
			get {
				if(IsSelfReported) {
					return Medication.Quantity;
				}
				return Prescription.Quantity;
			}
		}
		public string Refills {
			get {
				if(IsSelfReported) {
					return Medication.Refills;
				}
				return Prescription.Refills;
			}
		}
		public long RxCUI {
			get {
				if(IsSelfReported) {
					if(!Medication.RxCUI.IsNullOrEmpty()) {
						return PIn.Long(Medication.RxCUI,false);//Cast from string to long is intentional.
					}
				}
				else {
					if(!Prescription.RxCUI.IsNullOrEmpty()) {
						return PIn.Long(Prescription.RxCUI,false);//Cast from string to long is intentional.
					}
				}
				//Something went wrong.
				return 0;
			}
		}
		public string Schedule {
			get {
				if(IsSelfReported) {
					return Medication.Schedule;
				}
				return Prescription.Schedule;
			}
		}
		public DateTime? DateWritten {
			get {
				if(IsSelfReported) {
					return Medication.WrittenDate;
				}
				return Prescription.WrittenDate;
			}
		}

		public DoseSpotMedicationWrapper(DoseSpotPrescription prescription, DoseSpotSelfReported medication) {
			Prescription=prescription;
			Medication=medication;
		}
	}
}
