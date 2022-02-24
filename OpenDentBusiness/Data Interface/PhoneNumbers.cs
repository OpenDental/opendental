using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PhoneNumbers{
		public static int SyncBatchSize=5000;
		public static List<PhoneNumber> GetPhoneNumbers(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneNumber>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM phonenumber WHERE PatNum="+POut.Long(patNum);
			return Crud.PhoneNumberCrud.SelectMany(command);
		}

		public static PhoneNumber GetByVal(string phoneNumberVal) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PhoneNumber>(MethodBase.GetCurrentMethod(),phoneNumberVal);
			}
			string command="SELECT * FROM phonenumber WHERE PhoneNumberVal='"+POut.String(phoneNumberVal)+"'";
			return Crud.PhoneNumberCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(PhoneNumber phoneNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				phoneNumber.PhoneNumberNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneNumber);
				return phoneNumber.PhoneNumberNum;
			}
			return Crud.PhoneNumberCrud.Insert(phoneNumber);
		}

		///<summary></summary>
		public static void Update(PhoneNumber phoneNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneNumber);
				return;
			}
			Crud.PhoneNumberCrud.Update(phoneNumber);
		}

		public static void SyncAllPats() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//Get all PhoneNumbers we will delete later, anything except 'Other' that has a PhoneNumberVal.
			ProgressBarEvent.Fire(ODEventType.ProgressBar,Lans.g("PhoneNumber","Initializing..."));
			string command=$"SELECT PhoneNumberNum FROM phonenumber WHERE PhoneType!={(int)PhoneType.Other} OR PhoneNumberVal=''";
			List<long> listPhoneNumberNumsToDelete=Db.GetList(command,x => PIn.Long(x["PhoneNumberNum"].ToString()));
			//Per clinic, including 0 clinic.
			List<Clinic> listClinics=Clinics.GetWhere(x => true).Concat(new List<Clinic> { Clinics.GetPracticeAsClinicZero() }).ToList();
			for(int i=0;i<listClinics.Count;i++) {
				Clinic clinic=listClinics[i];
				long clinicNum=clinic.ClinicNum;
				//Per Patient table phone number field.
				foreach(PhoneType phoneType in new List<PhoneType> { PhoneType.HmPhone,PhoneType.WkPhone,PhoneType.WirelessPhone }) {
					AddPhoneNumbers(clinic,i,listClinics.Count,phoneType);
				}
			}
			//Remove old PhoneNumbers in batches of 5000
			ProgressBarEvent.Fire(ODEventType.ProgressBar,Lans.g("PhoneNumber","Cleaning up..."));
			while(listPhoneNumberNumsToDelete.Count>0) {
				command=$"DELETE FROM phonenumber WHERE PhoneNumberNum IN ({string.Join(",",listPhoneNumberNumsToDelete.Take(SyncBatchSize).Select(x => POut.Long(x)))})";
				Db.NonQ(command);
				listPhoneNumberNumsToDelete.RemoveRange(0,Math.Min(listPhoneNumberNumsToDelete.Count,SyncBatchSize));
			}
		}

		///<summary>Adds entries to PhoneNumber table based on Patient table for given clinicNum and PhoneType.</summary>
		private static void AddPhoneNumbers(Clinic clinic,int clinicIndex,int countClinics,PhoneType phoneType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinic,clinicIndex,countClinics,phoneType);
				return;
			}
			//Map PhoneType to Patient phone number field.
			string field=phoneType switch {
				PhoneType.HmPhone => nameof(Patient.HmPhone),
				PhoneType.WkPhone => nameof(Patient.WkPhone),
				PhoneType.WirelessPhone => nameof(Patient.WirelessPhone),
				_ => "",
			};
			if(string.IsNullOrWhiteSpace(field)) {
				return;//Skip on unknown field.
			}
			//Get PatNums for all the patients with any value in this phone number field.
			string command=$"SELECT PatNum FROM patient WHERE {POut.String(field)}!='' AND ClinicNum={POut.Long(clinic.ClinicNum)}";
			List<long> listPatNums=Db.GetListLong(command);
			int countPatNums=listPatNums.Count;
			int countPatsProcessed=0;
			while(listPatNums.Count>0) {
				//Process in batches.
				List<long> listPatNumsBatch=listPatNums.Take(SyncBatchSize).ToList();
				countPatsProcessed+=listPatNumsBatch.Count;
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lans.g("PhoneNumber","Processing")
					+$" ({clinicIndex+1}/{countClinics}): {clinic.Abbr} {phoneType.GetDescription()} {countPatsProcessed}/{countPatNums}");
				//PhoneNumberNum,PatNum,PhoneNumberVal,PhoneNumberDigits,PhoneType
				command=$@"SELECT 
					0 PhoneNumberNum,
					PatNum,
					{POut.String(field)} PhoneNumberVal,
					'' PhoneNumberDigits,
					{(int)phoneType} PhoneType
				FROM patient
				WHERE PatNum IN ({string.Join(",",listPatNumsBatch.Select(x => POut.Long(x)))})";
				List<PhoneNumber> listPhoneNumbers=Crud.PhoneNumberCrud.SelectMany(command);
				//Normalize PhoneNumberDigits field.
				listPhoneNumbers.ForEach(x => x.PhoneNumberDigits=RemoveNonDigitsAndTrimStart(x.PhoneNumberVal));
				//Ignore empty phone numbers.
				listPhoneNumbers.RemoveAll(x => x.PhoneType!=PhoneType.Other && string.IsNullOrEmpty(x.PhoneNumberDigits));
				Crud.PhoneNumberCrud.InsertMany(listPhoneNumbers);
				listPatNums.RemoveRange(0,Math.Min(listPatNums.Count,SyncBatchSize));
			}
		}

		///<summary>Syncs patient HmPhone, WkPhone, and WirelessPhone to the PhoneNumber table.  Will delete extra PhoneNumber table rows of each type
		///and any rows for numbers that are now blank in the patient table.</summary>
		public static void SyncPat(Patient pat) {
			SyncPats(new List<Patient>() { pat });
		}

		///<summary>Syncs patient HmPhone, WkPhone, and WirelessPhone to the PhoneNumber table.  Will delete extra PhoneNumber table rows of each type
		///and any rows for numbers that are now blank in the patient table.</summary>
		public static void SyncPats(List<Patient> listPats) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPats);
				return;
			}
			if(listPats.Count==0) {
				return;
			}
			string command=$@"DELETE FROM phonenumber
				WHERE PatNum IN ({string.Join(",",listPats.Select(x => POut.Long(x.PatNum)))}) AND PhoneType!={(int)PhoneType.Other}";
			Db.NonQ(command);
			List<PhoneNumber> listForInsert=listPats
				.SelectMany(x => Enumerable.Range(1,3)
					.Select(y => {
						string phNumCur=y==1?x.HmPhone:(y==2?x.WkPhone:(y==3?x.WirelessPhone:""));
						return new PhoneNumber() {
							PatNum=x.PatNum,
							PhoneNumberVal=phNumCur,
							PhoneNumberDigits=RemoveNonDigitsAndTrimStart(phNumCur),
							PhoneType=(PhoneType)y
						};
					}))
				.Where(x => !string.IsNullOrEmpty(x.PhoneNumberVal) && !string.IsNullOrEmpty(x.PhoneNumberDigits)).ToList();
			if(listForInsert.Count>0) {
				Crud.PhoneNumberCrud.InsertMany(listForInsert);
			}
		}

		public static void DeleteObject(long phoneNumberNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneNumberNum);
				return;
			}
			Crud.PhoneNumberCrud.Delete(phoneNumberNum);
		}

		///<summary>Removes non-digit chars and any leading 0's and 1's.</summary>
		public static string RemoveNonDigitsAndTrimStart(string phNum) {
			if(string.IsNullOrEmpty(phNum)) {
				return "";
			}
			//Not using Char.IsDigit because it includes characters like '٣' and '෯'
			return new string(phNum.Where(x => x>='0' && x<='9').ToArray()).TrimStart('0','1');
		}
	}
}