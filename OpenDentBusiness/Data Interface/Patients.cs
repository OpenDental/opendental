using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.AutoComm;

namespace OpenDentBusiness {
	
	///<summary></summary>
	public class Patients {
		public const string LANGUAGE_DECLINED_TO_SPECIFY="Declined to Specify";
		///<summary>Defines delegate signature to be used for Patients.NavPatDelegate.</summary>
		public delegate void NavToPatDelegate(Patient pat,bool isRefreshCurModule,bool isApptRefreshDataPat=true,bool hasForcedRefresh=false);
		///<summary>Sent in from FormOpenDental. Allows static method for business layer to cause patient navigation in FormOpenDental.</summary>
		public static NavToPatDelegate NavPatDelegate;

		#region Get Methods
		///<summary>Returns a list of all potential clones for the patient passed in.  The list returned will always contain the patNum passed in.
		///It is okay for patNum passed in to be a clone, a master, or even a patient that is not even related to clones at all.</summary>
		public static List<long> GetClonePatNumsAll(long patNum) {
			//No need to check RemotingRole; no call to db.
			long patNumOriginal=patNum;
			//Figure out if the patNum passed in is in fact the original patient and if it isn't, go get it from the database.
			if(PatientLinks.IsPatientAClone(patNum)) {
				patNumOriginal=PatientLinks.GetOriginalPatNumFromClone(patNum);
			}
			return PatientLinks.GetPatNumsLinkedFromRecursive(patNumOriginal,PatientLinkType.Clone);
		}

		///<summary>Returns a Def representing the patient specialty associated through DefLinks to the passed in Patient.
		///Returns null if no specialty found.</summary>
		public static Def GetPatientSpecialtyDef(long patNum) {
			Dictionary<Patient,Def> dictSpecialties=GetClonesAndSpecialtiesForPatients(new List<long> { patNum });
			if(dictSpecialties.Keys.Any(x => x.PatNum==patNum)) {
				return dictSpecialties.FirstOrDefault(x => x.Key.PatNum==patNum).Value;
			}
			return null;
		}

		///<summary>Gets all potential clones and their corresponding specialty for the patient passed in.
		///Even if the patNum passed in is itself a clone, all clones for the clone's master will still get returned.
		///The returned dictionary will always contain the master patient so that it can be displayed to the user if desired.
		///Specialties are only important if clinics are enabled.  If clinics are disabled then the corresponding Def will be null.</summary>
		public static SerializableDictionary<Patient,Def> GetClonesAndSpecialties(long patNum) {
			//No need to check RemotingRole; no call to db.
			return GetClonesAndSpecialtiesForPatients(GetClonePatNumsAll(patNum));
		}

		///<summary>Gets all potential clones and their corresponding specialty for the original patients passed in.
		///The returned dictionary will always contain the master patient so that it can be displayed to the user if desired.
		///Specialties are only important if clinics are enabled.  If clinics are disabled then the corresponding Def will be null.</summary>
		public static SerializableDictionary<Patient,Def> GetClonesAndSpecialtiesForPatients(List<long> listPatNums) {
			//No need to check RemotingRole; no call to db.
			//Get every single patientlink possible whether the patNum passed in was the master patient or the clone patient.
			SerializableDictionary<Patient,Def> dictCloneSpecialty=new SerializableDictionary<Patient, Def>();
			if(listPatNums==null || listPatNums.Count==0) {
				return dictCloneSpecialty;//No clones found.
			}
			//Get all of the clones.
			Patient[] arrayPatientClones=Patients.GetMultPats(listPatNums.Distinct().ToList());
			if(arrayPatientClones==null || arrayPatientClones.Length==0) {
				return dictCloneSpecialty;//No patients for clone links found.
			}
			List<DefLink> listPatDefLink=DefLinks.GetDefLinksByType(DefLinkType.Patient);
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ClinicSpecialty);
			foreach(Patient clone in arrayPatientClones) {
				DefLink defLink=listPatDefLink.FirstOrDefault(x => x.FKey==clone.PatNum);
				Def specialty=null;
				if(defLink!=null) {
					specialty=Defs.GetDef(DefCat.ClinicSpecialty,defLink.DefNum,listDefs);//Can return null which is fine.
				}
				dictCloneSpecialty[clone]=specialty;
			}
			return dictCloneSpecialty;
		}
		
		///<summary>Returns the master or original patient for the clone passed in otherwise returns the patient passed in if patCur is not a clone.
		///Will return null if the patCur is a clone but the master or original patient could not be found in the database.</summary>
		public static Patient GetOriginalPatientForClone(Patient patCur) {
			//No need to check RemotingRole; no call to db.
			if(patCur==null || !IsPatientAClone(patCur.PatNum)) {
				return patCur;
			}
			//Go get the master or original patient from the database for the clone patient passed in.
			return GetPat(PatientLinks.GetOriginalPatNumFromClone(patCur.PatNum));
		}

		///<summary> Gets any patient whose wireless, home, or work number matches the passed phone number. Be careful with what you pass in as 
		///phoneNumber. If you pass in '1', you will get almost every patient.</summary>
		public static List<Patient> GetPatientsByPhone(string phoneNumber,string countryCode,List<PhoneType> listPhoneTypes=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),phoneNumber,countryCode,listPhoneTypes);
			}
			phoneNumber??="";//Avoid any null reference exceptions.
			//Default to search for the three main phone types.
			if(listPhoneTypes.IsNullOrEmpty()) {
				listPhoneTypes=new List<PhoneType> { PhoneType.WirelessPhone,PhoneType.HmPhone,PhoneType.WkPhone };
			}
			List<Patient> listPats;
			/* Example output:
			 * SELECT patient.* FROM patient WHERE patient.PatStatus NOT IN (3,4) AND (patient.WirelessPhone REGEXP <phonenumber> OR patient.HmPhone REGEXP <phonenumber>...)
			 * SELECT patient.* FROM patient INNER JOIN phonenumber ON phonenumber.PatNum=patient.PatNum WHERE patient.PatStatus NOT IN (3,4) AND (phonenumber.PhoneNumberDigits=<phonenumber>)
			 */
			try {
				string select=$"SELECT patient.* FROM patient ";
				string join="";
				string where=$"WHERE patient.PatStatus NOT IN ({POut.Int((int)PatientStatus.Archived)},{POut.Int((int)PatientStatus.Deleted)}) ";
				if(PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)) {//Search the PhoneNumber table
					join="INNER JOIN phonenumber ON phonenumber.PatNum=patient.PatNum ";
					where+=$"AND ({GetPhoneNumberWhereClause(phoneNumber,listPhoneTypes)}) ";
				}
				else {//Search the Patient table only			
					where+=$"AND ({GetPatientPhoneRegexpClause(phoneNumber,countryCode,listPhoneTypes)}) ";
				}
				string groupby="GROUP BY patient.PatNum";
				string command=select+join+where+groupby;
				listPats=Crud.PatientCrud.SelectMany(command);
			}
			catch {	
				//should only happen if phone number is blank, if so, return empty list below, with appropriate structure 
				listPats=new List<Patient>();
			}
			return listPats;
		}

		///<summary>Returns a MySQL clause to search the phonenumber table for an exact match.</summary>
		private static string GetPhoneNumberWhereClause(string phoneNumber,List<PhoneType> listPhoneTypes) {
			string strPhoneDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneNumber);//Digits only, strip leading 0/1
			string where=$"phonenumber.PhoneNumberDigits='{POut.String(strPhoneDigits)}' ";//PhoneNumber.PhoneNumberDigits exactly equals RemoveDigitsAndTrimStart()
			string strPhoneTypes=string.Join(",",listPhoneTypes.Select(x => POut.Int((int)x)));
			where+=$"AND phonenumber.PhoneType IN ({strPhoneTypes})";//For these PhoneTypes
			return where;
		}

		///<summary>Returns a MySQL clause to search the patient table for a phone number with REGEXP</summary>
		private static string GetPatientPhoneRegexpClause(string phoneNumber,string countryCode,List<PhoneType> listPhoneTypes) {
			List<string> listPhoneFields=listPhoneTypes.Select(x => x switch {
				PhoneType.WirelessPhone => nameof(Patient.WirelessPhone),
				PhoneType.HmPhone => nameof(Patient.HmPhone),
				PhoneType.WkPhone => nameof(Patient.WkPhone),
				_ => null,//No other fields available
			}).Where(x => x!=null).ToList();
			string phoneRegexp=ConvertPhoneToRegexp(phoneNumber,countryCode);
			//DO NOT POut THIS REGEX. They have been cleaned for use in this function by ConvertPhoneToRegexp.
			return string.Join(" OR ",listPhoneFields.Select(x => DbHelper.Regexp(POut.String($"patient.{x}"),phoneRegexp)));
		}

		///<summary>Expands a phone number into a string that can be used to ignore punctuation in a phone number.
		///Any string that passes through this function does not need to, and should not, go through POut.String()</summary>
		private static string ConvertPhoneToRegexp(string phoneRaw,string countryCode) {
			//Strip all non-numeric characters just in case.
			string retVal=new string(phoneRaw.Where(x => char.IsDigit(x)).ToArray());
			string prefix="";
			string wildcard="[^0-9]*";//any quantity of any non-digit character
			switch(countryCode.ToUpper()) {
				case "US":
				case "CA":
					//Number prefixed with a country and not prefixed with a country code should both be prefixed with a country code.
					//EG: Both of the following should yield the same 11-digit string... 80012345678, 180012345678 == 180012345678.
					if(retVal.Length==11 && retVal[0]=='1') { //We have an 11-digit number coming in that starts with a 1
						//Prefix with {0,1} in order to make country code optional.
						prefix=retVal[0]+"{0,1}"+wildcard;
						//Remove the first char, which we just included in the prefix above.
						retVal=retVal.Substring(1);
					}
					break;
			}		
			if(string.IsNullOrEmpty(retVal)) {
				throw new Exception("Phone number cannot be blank.");
			}
			//Add back the optional prefix from above and converto a RegEx.
			//Ex. for 1(503)363-5432
			//[^0-9]*1{0,1}[^0-9]*5[^0-9]*0[^0-9]*3[^0-9]*3[^0-9]*6[^0-9]*3[^0-9]*5[^0-9]*4[^0-9]*3[^0-9]*2[^0-9]*
			retVal=wildcard+prefix+string.Join(wildcard,retVal.ToArray())+wildcard;			
			return retVal;
		}

		///<summary>Gets an AgingList for all patients who are not deleted and are not archived with a $0 balance.  Includes the list of tsitranslogs, a
		///value indicating whether or not insurance is pending, and a value indicating whether or not there are any unsent procs for each pataging.
		///Only used for the A/R Manager.</summary>
		public static List<PatAging> GetAgingList(long clinicNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatAging>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			long collectionBillType=Defs.GetDefsForCategory(DefCat.BillingTypes,true).FirstOrDefault(x => x.ItemValue.ToLower()=="c")?.DefNum??0;
			string guarAndClinicNum="";
			string guarClinicJoin="";
			string guarGroupBy="GROUP BY p.Guarantor";
			if(PrefC.HasClinicsEnabled) {
				guarAndClinicNum=$@"
					AND guar.ClinicNum={POut.Long(clinicNum)}";
				guarClinicJoin=$@"
				INNER JOIN patient guar ON p.Guarantor=guar.PatNum{guarAndClinicNum}";
				guarGroupBy="GROUP BY guar.PatNum";
			}
			string command=$@"SELECT guar.PatNum,guar.Bal_0_30,guar.Bal_31_60,guar.Bal_61_90,guar.BalOver90,guar.BalTotal,guar.InsEst,
				guar.BalTotal-guar.InsEst AS $pat,guar.PayPlanDue,guar.LName,guar.FName,guar.Preferred,guar.MiddleI,guar.PriProv,guar.BillingType,
				guar.ClinicNum,guar.Address,guar.City,guar.State,guar.Zip,guar.Birthdate
				FROM patient guar
				WHERE guar.PatNum=guar.Guarantor{guarAndClinicNum}
				AND guar.PatStatus!={POut.Int((int)PatientStatus.Deleted)}
				AND (
					guar.PatStatus!={POut.Int((int)PatientStatus.Archived)}
					OR ABS(guar.BalTotal)>0.005{(collectionBillType==0?"":$@"
					OR guar.BillingType={POut.Long(collectionBillType)}")}
				)";
			Dictionary<long,PatAging> dictAll=new Dictionary<long,PatAging>();
			using DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return new List<PatAging>();
			}
			foreach(DataRow row in table.Rows) {
				long patNum=PIn.Long(row["PatNum"].ToString());
				dictAll[patNum]=new PatAging() {
					PatNum=patNum,
					Guarantor=patNum,
					Bal_0_30=PIn.Double(row["Bal_0_30"].ToString()),
					Bal_31_60=PIn.Double(row["Bal_31_60"].ToString()),
					Bal_61_90=PIn.Double(row["Bal_61_90"].ToString()),
					BalOver90=PIn.Double(row["BalOver90"].ToString()),
					BalTotal=PIn.Double(row["BalTotal"].ToString()),
					InsEst=PIn.Double(row["InsEst"].ToString()),
					AmountDue=PIn.Double(row["$pat"].ToString()),
					PayPlanDue=PIn.Double(row["PayPlanDue"].ToString()),
					PatName=Patients.GetNameLF(PIn.String(row["LName"].ToString()),PIn.String(row["FName"].ToString()),
							PIn.String(row["Preferred"].ToString()),PIn.String(row["MiddleI"].ToString())),
					PriProv=PIn.Long(row["PriProv"].ToString()),
					BillingType=PIn.Long(row["BillingType"].ToString()),
					ClinicNum=PIn.Long(row["ClinicNum"].ToString()),
					Address=PIn.String(row["Address"].ToString()),
					City=PIn.String(row["City"].ToString()),
					State=PIn.String(row["State"].ToString()),
					Zip=PIn.String(row["Zip"].ToString()),
					Birthdate=PIn.Date(row["Birthdate"].ToString()),
					//the following values will be set below, if applicable
					ListTsiLogs=new List<TsiTransLog>(),
					DateLastPay=DateTime.MinValue,
					HasInsPending=false,
					DateLastProc=DateTime.MinValue,
					HasUnsentProcs=false,
					DateBalBegan=DateTime.MinValue
				};
			}
			TsiTransLogs.SelectMany(dictAll.Keys.ToList())
				.GroupBy(x => x.PatNum)
				.ForEach(x => dictAll[x.Key].ListTsiLogs=x.OrderByDescending(y => y.TransDateTime).ToList());
			command=$@"SELECT p.Guarantor,MAX(p.DatePay) DateLastPay
				FROM (
					SELECT patient.Guarantor,MAX(paysplit.DatePay) DatePay
					FROM paysplit
					INNER JOIN payment ON payment.PayNum=paysplit.PayNum
					INNER JOIN patient ON paysplit.PatNum=patient.PatNum
					WHERE payment.PayType!=0
					GROUP BY paysplit.PayNum,patient.Guarantor
					HAVING SUM(paysplit.SplitAmt)!=0
					ORDER BY NULL
				) p{guarClinicJoin}
				{guarGroupBy}
				ORDER BY NULL";
			using DataTable tableDateLastPay=Db.GetTable(command);
			foreach(DataRow row in tableDateLastPay.Rows) {
				long guarNum=PIn.Long(row["Guarantor"].ToString());
				if(!dictAll.ContainsKey(guarNum)) {
					continue;
				}
				dictAll[guarNum].DateLastPay=PIn.Date(row["DateLastPay"].ToString());
			}
			command=$@"SELECT DISTINCT p.Guarantor
				FROM patient p{guarClinicJoin}
				INNER JOIN claim ON p.PatNum=claim.PatNum
					AND claim.ClaimStatus IN ('U','H','W','S')
					AND claim.ClaimType IN ('P','S','Other')";
			Db.GetListLong(command).FindAll(x => dictAll.ContainsKey(x)).ForEach(x => dictAll[x].HasInsPending=true);
			command=$@"SELECT p.Guarantor,MAX(procedurelog.ProcDate) MaxProcDate
				FROM patient p{guarClinicJoin}
				INNER JOIN procedurelog ON procedurelog.PatNum=p.PatNum
				WHERE procedurelog.ProcFee>0
				AND procedurelog.ProcStatus=2
				{guarGroupBy}
				ORDER BY NULL";
			using DataTable tableMaxProcDate=Db.GetTable(command);
			foreach(DataRow row in tableMaxProcDate.Rows) {
				long guarNum=PIn.Long(row["Guarantor"].ToString());
				if(!dictAll.ContainsKey(guarNum)) {
					continue;
				}
				dictAll[guarNum].DateLastProc=PIn.Date(row["MaxProcDate"].ToString());
			}
			command=$@"SELECT DISTINCT p.Guarantor
				FROM patient p{guarClinicJoin}
				INNER JOIN procedurelog ON procedurelog.PatNum=p.PatNum
				INNER JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum
				WHERE procedurelog.ProcFee>0
				AND procedurelog.ProcStatus=2
				AND procedurelog.ProcDate>CURDATE()-INTERVAL 6 MONTH
				AND claimproc.NoBillIns=0
				AND claimproc.Status=6";
			Db.GetListLong(command).FindAll(x => dictAll.ContainsKey(x)).ForEach(x => dictAll[x].HasUnsentProcs=true);
			return dictAll.Values.ToList();
		}

		public static void SetDateBalBegan(long clinicNum,ref List<PatAging> listPatAgingAll,ref List<ClinicBalBegans> listClinicBalBegans) {
			//No need to check RemotingRole; no call to db and uses ref parameters.
			Dictionary<long,PatAging> dictAll=listPatAgingAll.ToDictionary(x => x.PatNum);
			if(!listClinicBalBegans.Any(x => x.ClinicNum==clinicNum)) {
				listClinicBalBegans.Add(new ClinicBalBegans(clinicNum,Ledgers.GetDateBalanceBegan(clinicNum)));//uses today's date, doesn't consider super families
			}
			Dictionary<long,DateTime> dictDateBals=listClinicBalBegans.First(x => x.ClinicNum==clinicNum).DictGuarDateBals;//guaranteed to contain clinicNum from above
			foreach(long patNum in dictAll.Keys) {
				if(!dictDateBals.ContainsKey(patNum)) {
					continue;
				}
				dictAll[patNum].DateBalBegan=dictDateBals[patNum];
			}
		}

		///<summary>Used by the OpenDentalService Transworld thread to sync accounts sent to collection.</summary>
		public static List<long> GetListCollectionGuarNums(bool doIncludeSuspended=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),doIncludeSuspended);
			}
			List<Def> listBillTypes=Defs.GetDefsForCategory(DefCat.BillingTypes,true).FindAll(x => x.ItemValue.ToLower()=="c");
			List<long> listSuspendedGuarNums=new List<long>();
			if(doIncludeSuspended) {
				listSuspendedGuarNums=TsiTransLogs.GetSuspendedGuarNums();
			}
			if(listBillTypes.Count==0) {
				return listSuspendedGuarNums;//no collection billing type, return suspended guar nums, could be empty
			}
			string command="SELECT patient.Guarantor "
				+"FROM patient "
				+"WHERE patient.PatNum=patient.Guarantor "
				+"AND patient.BillingType IN ("+string.Join(",",listBillTypes.Select(x => POut.Long(x.DefNum)))+")";
			return Db.GetListLong(command).Union(listSuspendedGuarNums).ToList();
		}

		///<summary>Used to determine whether or not the guarantor of a family is sent to collections.  Used in order to prompt the user to specify
		///whether the payment or adjustment being entered on a collection patient came from Transworld and therefore shouldn't be sent to Transworld.</summary>
		public static bool IsGuarCollections(long guarNum,bool includeSuspended=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),guarNum,includeSuspended);
			}
			if(includeSuspended && TsiTransLogs.IsGuarSuspended(guarNum)) {
				return true;
			}
			Def billTypeColl=Defs.GetDefsForCategory(DefCat.BillingTypes,true).FirstOrDefault(x => x.ItemValue.ToLower()=="c");
			if(billTypeColl==null) {
				return false;//if not suspended and no billing type marked as collection billing type, return false, guar not a collection guar
			}
			string command="SELECT 1 isGuarCollection "
				+"FROM patient "
				+"WHERE PatNum="+POut.Long(guarNum)+" "
				+"AND PatNum=Guarantor "
				+"AND BillingType="+POut.Long(billTypeColl.DefNum)+" "
				+DbHelper.LimitAnd(1);
			return PIn.Bool(Db.GetScalar(command));
		}

		///<summary>Fetches all Gurantor patnums who have family members where some have a positive estimated balance and some negative.</summary>
		public static List<long> GetAllTransferGuarantors() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			//We use SUM() in this query so the GROUP BY behaves correctly and summarizes GreaterThan and Lessthan.
			string command=@"SELECT family.Guarantor FROM ( 
					SELECT patient.Guarantor, 
					SUM(CASE WHEN patient.EstBalance>0 THEN 1 ELSE 0 END) AS GreaterThan, 
					SUM(CASE WHEN patient.EstBalance<0 THEN 1 ELSE 0 END) AS LessThan 
				FROM patient 
				WHERE PatStatus="+POut.Int((int)PatientStatus.Patient)+@"
				GROUP BY patient.Guarantor
				HAVING GreaterThan>0 AND LessThan>0) family";
			return Db.GetListLong(command);
		}

		public static List<long> GetGuarantorsForPatNums(List<long> listPatNums) {
			if(listPatNums.IsNullOrEmpty()) {
				return new List<long>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			//If two patients in the same family are passed in, it will still only return that families guarantor once.
			string command="SELECT DISTINCT Guarantor FROM patient WHERE PatNum IN ("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") ";
			return Db.GetListLong(command);
		}

		///<summary>Returns a list of guarantors in charge of families that have had financial data changed after the specified date.
		///The time portion of the date passed in is ignored in order to include families that have changes on or after the date passed in.</summary>
		public static List<long> GetGuarantorsWithFinancialDataChangedAfterDate(DateTime dateChanged) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateChanged);
			}
			//Find all of the PatNums that have had financial data changed since the specified date (at midnight).
			//Utilize POut.Date instead of POut.DateT so that the time portion is excluded thus allowing the changes on the specified date to be returned.
			//E.g. an adjustment with a SecDateTEdit of 2016-04-19 17:38:36 will be returned when SecDateTEdit > '2016-04-19' is used within the WHERE clause.
			//Also, utilize UNION instead of UNION ALL because we do not want duplicates to be returned.
			string command=$@"SELECT DISTINCT PatNum FROM adjustment WHERE SecDateTEdit > {POut.Date(dateChanged)}
				UNION
				SELECT DISTINCT PatNum FROM claimproc WHERE SecDateTEdit > {POut.Date(dateChanged)}
				UNION
				SELECT DISTINCT PatNum FROM payplancharge WHERE SecDateTEdit > {POut.Date(dateChanged)}
				UNION
				SELECT DISTINCT PatNum FROM paysplit WHERE SecDateTEdit > {POut.Date(dateChanged)}
				UNION
				SELECT DISTINCT PatNum FROM procedurelog WHERE DateTStamp > {POut.Date(dateChanged)}";
			List<long> listPatNums=Db.GetListLong(command);
			//Return a list of the guarantors for the PatNums.
			return GetGuarantorsForPatNums(listPatNums);
		}

		///<summary>Returns a list of PatNums for every guarantor in the database that hasn't been flagged as deleted.</summary>
		public static List<long> GetAllGuarantors() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			string command=$"SELECT DISTINCT Guarantor FROM patient WHERE patient.PatStatus!={POut.Int((int)PatientStatus.Deleted)}";
			return Db.GetListLong(command);
		}

		public static List<long> GetAllGuarantorsWithFamilies() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT DISTINCT Guarantor FROM patient WHERE Guarantor<>PatNum";
			return Db.GetListLong(command);
		}

		///<summary>Returns a list of PatNums for every guarantor in the database that has at least one other patient in the family.</summary>
		public static List<long> GetAllGuarantorsWithFamiliesAlphabetical() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			//In the future we may want to limit the status of guarantors returned.
			//Ignore patients flagged as deleted to prevent returning guarantors that appear to not have any other family members in their family.
			string command=$@"SELECT DISTINCT guar.PatNum FROM patient 
				INNER JOIN patient guar ON guar.PatNum=patient.Guarantor
				WHERE patient.Guarantor!=patient.PatNum
				AND patient.PatStatus!={POut.Int((int)PatientStatus.Deleted)}
				ORDER BY guar.LName, guar.FName, guar.PatNum";
			return Db.GetListLong(command);
		}

		public static Patient GetGuarForPat(long patNum) {
			if(patNum==0) {
				return null;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$@"SELECT * FROM patient guar
				WHERE guar.PatNum=(SELECT patient.Guarantor FROM patient WHERE patient.PatNum={POut.Long(patNum)})";
			return Crud.PatientCrud.SelectOne(command);
		}

		public static DataTable GetPatientsWithFirstLastAppointments(List<PatientStatus> listPatStatus,bool doExcludePatsWithFutureAppts,
			List<long> listClinicNums,int ageFrom,int ageTo,DateTime dateExcludeSeenSince,DateTime dateExcludeNotSeenSince,List<Def> listBillingType,int contactMethod=-1
			,List<long> listPatNums=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listPatStatus,doExcludePatsWithFutureAppts,listClinicNums,ageFrom,ageTo,dateExcludeSeenSince
					,dateExcludeNotSeenSince,listBillingType,contactMethod,listPatNums);
			}
			string command=@$"SELECT patient.PatNum,patient.FName,patient.LName,patient.Email,patient.PatStatus,patient.PreferContactMethod,patient.ClinicNum,
				patient.Preferred,patient.Birthdate,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip,patient.Country,apt.DateTimeLastApt,
				DATE(apt.DateTimeNextApt) DateTimeNextApt,a.AptNum NextAptNum
				FROM patient 
				LEFT JOIN (
					SELECT appointment.PatNum PatNum,
					MAX(DATE((CASE WHEN appointment.AptDateTime < CURDATE() AND appointment.AptStatus={POut.Int((int)ApptStatus.Complete)} 
						THEN appointment.AptDateTime ELSE NULL END))) DateTimeLastApt,
					MIN((CASE WHEN appointment.AptDateTime >= CURDATE() AND appointment.AptStatus IN ({POut.Int((int)ApptStatus.Scheduled)}
						,{POut.Int((int)ApptStatus.ASAP)}) THEN appointment.AptDateTime ELSE NULL END)) DateTimeNextApt
					FROM appointment
					WHERE appointment.AptStatus IN ({POut.Int((int)ApptStatus.Complete)},{POut.Int((int)ApptStatus.ASAP)},{POut.Int((int)ApptStatus.Scheduled)})
					GROUP BY appointment.PatNum
				)apt ON apt.PatNum=patient.PatNum
				LEFT JOIN appointment a ON apt.DateTimeNextApt = a.AptDateTime AND apt.PatNum = a.PatNum
				WHERE patient.PatStatus IN ({string.Join(",",listPatStatus.Select(x => POut.Int((int)x)))}) 
				AND (((YEAR(CURDATE()) - YEAR(patient.Birthdate)) - (RIGHT(CURDATE(),5)< RIGHT(patient.Birthdate,5))) BETWEEN {ageFrom} AND {ageTo} 
					OR YEAR(patient.Birthdate) = '0001') ";//minval year
			if(PrefC.HasClinicsEnabled && listClinicNums.Count>0) {
				command+=$"AND patient.ClinicNum IN ({string.Join(",",listClinicNums.Select(x => POut.Long(x)))}) ";
			}
			if(contactMethod!=-1) {
				command+=$"AND patient.PreferContactMethod={contactMethod} ";
			}
			if(doExcludePatsWithFutureAppts) {
				command+="AND apt.DateTimeNextApt IS NULL ";
			}
			if(!listBillingType.IsNullOrEmpty()){
				command+=$"AND patient.BillingType IN({string.Join(",",listBillingType.Select(x => POut.Long(x.DefNum)))}) ";
			}
			if(dateExcludeSeenSince!=DateTime.MinValue) {
				//Exclude patients that have been seen since (after) the passed in date
				command+=@$"AND (DATE(apt.DateTimeLastApt) < {POut.Date(dateExcludeSeenSince)} OR apt.DateTimeLastApt IS NULL)";
			}
			if(dateExcludeNotSeenSince!=DateTime.MinValue) {
				//Exclude patients that have not been seen since (before) the passed in date
				command+=@$"AND DATE(apt.DateTimeLastApt) >= {POut.Date(dateExcludeNotSeenSince)} ";
			}
			if(listPatNums!=null && listPatNums.Count>0) {
				command+=@$"AND patient.PatNum IN ({string.Join(",",listPatNums.Select(x => POut.Long(x)))}) ";
			}
			command+="ORDER BY patient.LName,patient.FName ";
			return Db.GetTable(command);
		}

		#endregion

		#region Insert
		///<summary>Creates a clone from the patient passed in and then links them together as master and clone via the patientlink table.
		///After the patient has been cloned successfully, this method will call SynchCloneWithPatient().
		///That synch method will take care of synching all fields that should be synched for a brand new clone.</summary>
		public static Patient CreateCloneAndSynch(Patient patient,Family familyCur=null,List<InsPlan> listInsPlans=null,List<InsSub> listInsSubs=null
			,List<Benefit> listBenefits=null,long primaryProvNum=0,long clinicNum=0)
		{
			//No need to check RemotingRole; no call to db.
			Patient patientSynch=CreateClone(patient,primaryProvNum,clinicNum);
			SynchCloneWithPatient(patient,patientSynch,familyCur,listInsPlans,listInsSubs,listBenefits);
			return patientSynch;
		}

		///<summary>Creates a clone from the patient passed in and then links them together as master and clone via the patientlink table.
		///This method only sets a few crucial variables on the patient clone returned.  Call any additional synch methods afterwards.
		///The clone that was created will be returned.  Optionally pass in a primary provider and / or clinic that should be used.</summary>
		public static Patient CreateClone(Patient patient,long primaryProvNum=0,long clinicNum=0) {
			//No need to check RemotingRole; no call to db.
			Patient patientSynch=new Patient();
			patientSynch.LName=patient.LName.ToUpper();
			patientSynch.FName=patient.FName.ToUpper();
			patientSynch.MiddleI=patient.MiddleI.ToUpper();
			patientSynch.Birthdate=patient.Birthdate;
			//PriPro is intentionally not synched so the clone can be assigned to a different provider for tracking production.
			if(primaryProvNum==0) {
				primaryProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			patientSynch.PriProv=primaryProvNum;
			patientSynch.ClinicNum=clinicNum;
			Patients.Insert(patientSynch,false);
			SecurityLogs.MakeLogEntry(Permissions.PatientCreate,patientSynch.PatNum,Lans.g("ContrFamily","Created from Family Module Clones Add button."));
			PatientLinks.Insert(new PatientLink() {
				PatNumFrom=patient.PatNum,
				PatNumTo=patientSynch.PatNum,
				LinkType=PatientLinkType.Clone,
			});
			#region Family / Super Family
			//Go get the clone from the database so that fields will be refreshed to their non-null values, i.e. '' instead of null
			patientSynch=Patients.GetPat(patientSynch.PatNum);
			Patient patientSynchOld=patientSynch.Copy();
			//Now that the clone has been inserted and has a primary key we can consider what family and/or super family the clone should be part of.
			if(PrefC.GetBool(PrefName.CloneCreateSuperFamily)) {
				//Put the clone into their own family.
				patientSynch.Guarantor=patientSynch.PatNum;
				//But then put the clone into the super family of the master (creating one if the master isn't already part of a super family).
				long superFamilyNum=patient.SuperFamily;
				if(superFamilyNum < 1) {
					//Forcefully create a new super family, make the master patient the super family head, and then put the clone into that super family.
					Patients.AssignToSuperfamily(patient.Guarantor,patient.Guarantor);//Moves other family members into the super family as well.
					superFamilyNum=patient.Guarantor;
				}
				//Do the guts of what AssignToSuperfamily() would have done but for our patientSynch object so that we save a db call.
				patientSynch.HasSuperBilling=true;
				patientSynch.SuperFamily=superFamilyNum;
			}
			else {
				//The preference to force using super families is off so we will only put the clone into a super family if the original is part of one.
				patientSynch.Guarantor=patient.Guarantor;
				patientSynch.SuperFamily=patient.SuperFamily;
			}
			//Save any family or super family changes to the database.  Other family members would have already been affected by this point.
			Update(patientSynch,patientSynchOld);
			#endregion
			return patientSynch;
		}
		#endregion

		#region Update
		///<summary>Synchs all clones related to the patient passed in with it's current information.  Returns a string representing what happened.
		///Optionally pass in the lists of insurance information to save db calls within a loop.</summary>
		public static string SynchClonesWithPatient(Patient patient,Family familyCur=null,List<InsPlan> listInsPlans=null
			,List<InsSub> listInsSubs=null,List<Benefit> listBenefits=null,List<PatPlan> listPatPlans=null) 
		{
			//No need to check RemotingRole; no call to db.
			StringBuilder stringBuilder=new StringBuilder();
			//Get all clones for the patient passed in and then synch each one and return a string regarding what happened during the synch.
			long patNumOriginal=patient.PatNum;
			//Figure out if the patNum passed in is in fact the original patient and if it isn't, go get it from the database.
			if(PatientLinks.IsPatientAClone(patient.PatNum)) {
				patNumOriginal=PatientLinks.GetOriginalPatNumFromClone(patient.PatNum);
			}
			//Now that we know the PatNum of the original or master patient we can get all corresponding clones.
			List<long> listPatNumClones=PatientLinks.GetPatNumsLinkedFromRecursive(patNumOriginal,PatientLinkType.Clone);
			//We now have every single clone PatNum but need to remove the one that is associated to patient so that we don't synch it.
			listPatNumClones.RemoveAll(x => x==patient.PatNum);
			Patient[] arraySynchPatients=GetMultPats(listPatNumClones);
			//Loop through all remaining clones and synch them with the patient that was passed in.
			foreach(Patient patientSynch in arraySynchPatients) {
				string changes=SynchCloneWithPatient(patient,patientSynch,familyCur,listInsPlans,listInsSubs,listBenefits,listPatPlans);
				if(!string.IsNullOrWhiteSpace(changes)) {
					stringBuilder.AppendLine(Lans.g("ContrFamily","The following changes were made to the patient")
							+" "+patientSynch.PatNum+" - "+Patients.GetNameFL(patientSynch.LName,patientSynch.FName,patientSynch.Preferred,patientSynch.MiddleI)
							+":\r\n"+changes);
				}
			}
			return stringBuilder.ToString();
		}

		///<summary>Synchs current information for patient to patientSynch passed in.  Returns a string representing what happened.
		///Optionally pass in the list of PatPlans for the clone and non-clone in order to potentially save db calls.</summary>
		public static string SynchCloneWithPatient(Patient patient,Patient patientSynch,Family familyCur=null,List<InsPlan> listInsPlans=null
			,List<InsSub> listInsSubs=null,List<Benefit> listBenefits=null,List<PatPlan> listPatPlans=null,List<PatPlan> listPatPlansForSynch=null) 
		{
			//No need to check RemotingRole; no call to db.
			Patient patCloneOld=patientSynch.Copy();
			PatientCloneDemographicChanges patientCloneDemoChanges=SynchCloneDemographics(patient,patientSynch);
			if(Update(patientSynch,patCloneOld)
				&& PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)
				&& patientCloneDemoChanges.ListFieldsCleared
					.Union(patientCloneDemoChanges.ListFieldsUpdated.Select(y => y.FieldName))
					.Any(x => ListTools.In(x,"Home Phone","Wireless Phone","Work Phone")))
			{
				PhoneNumbers.SyncPat(patientSynch);
			}
			InsertBillTypeChangeSecurityLogEntry(patCloneOld,patientSynch);
			string strDataUpdated="";
			string strChngFrom=" "+Lans.g("ContrFamily","changed from")+" ";
			string strChngTo=" "+Lans.g("ContrFamily","to")+" ";
			string strBlank=Lans.g("ContrFamily","blank");
			foreach(PatientCloneField patientCloneField in patientCloneDemoChanges.ListFieldsUpdated) {
				strDataUpdated+=Lans.g("ContrFamily",patientCloneField.FieldName)+strChngFrom;
				strDataUpdated+=(string.IsNullOrEmpty(patientCloneField.OldValue)) ? strBlank : patientCloneField.OldValue;
				strDataUpdated+=strChngTo;
				strDataUpdated+=(string.IsNullOrEmpty(patientCloneField.NewValue)) ? strBlank : patientCloneField.NewValue;
				strDataUpdated+="\r\n";
			}
			if(familyCur==null) {
				familyCur=Patients.GetFamily(patient.PatNum);
			}
			if(listInsSubs==null) {
				listInsSubs=InsSubs.RefreshForFam(familyCur);
			}
			if(listInsPlans==null) {
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			}
			if(listBenefits==null) {
				listBenefits=Benefits.Refresh(PatPlans.Refresh(patient.PatNum),listInsSubs);
			}
			PatientClonePatPlanChanges patientClonePatPlanChanges=SynchClonePatPlans(patient,patientSynch,familyCur,listInsPlans,listInsSubs,listBenefits
				,listPatPlans,listPatPlansForSynch);
			strDataUpdated+=patientClonePatPlanChanges.StrDataUpdated;
			return strDataUpdated;
		}

		///<summary>Synchs the demographics from patient to patientSynch.
		///Returns a PatientCloneSynch object that represents specifics regarding anything that changed during the synching process.
		///This method does not synch the family or the super family on purpose.</summary>
		private static PatientCloneDemographicChanges SynchCloneDemographics(Patient patient,Patient patientSynch) {
			//No need to check RemotingRole; no call to db and private method.
			PatientCloneDemographicChanges patientCloneDemoChanges=new PatientCloneDemographicChanges();
			//We allow users to synch clones to clones now.  Therefore, we need to always go to the database and figure out the PatNum of the original.
			long patNumOriginal=patient.PatNum;
			if(PatientLinks.IsPatientAClone(patient.PatNum)) {
				//The patient that is going to synch its settings to patientSynch must be a clone, go get the PatNum of the original patient.
				patNumOriginal=PatientLinks.GetOriginalPatNumFromClone(patient.PatNum);
			}
			bool isSynchTheMaster=(patientSynch.PatNum==patNumOriginal);
			#region Synch Clone Data - Patient Demographics
			patientCloneDemoChanges.ListFieldsUpdated=new List<PatientCloneField>();
			patientCloneDemoChanges.ListFieldsCleared=new List<string>();
			if(patientSynch.FName.ToLower()!=patient.FName.ToLower()) {
				if(patientSynch.FName!="" && patient.FName=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("First Name");
				}
				string fName=patient.FName.ToUpper();
				if(isSynchTheMaster) {//We are synching a clone to the master, do NOT update the master's field to all caps.
					fName=StringTools.ToUpperFirstOnly(patient.FName);
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("First Name",patientSynch.FName,fName));
				patientSynch.FName=fName;
			}
			if(patientSynch.LName.ToLower()!=patient.LName.ToLower()) {
				if(patientSynch.LName!="" && patient.LName=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Last Name");
				}
				string lName=patient.LName.ToUpper();
				if(isSynchTheMaster) {//We are synching a clone to the master, do NOT update the master's field to all caps.
					lName=StringTools.ToUpperFirstOnly(patient.LName);
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Last Name",patientSynch.LName,lName));
				patientSynch.LName=lName;
			}
			if(patientSynch.Title!=patient.Title) {
				if(patientSynch.Title!="" && patient.Title=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Title");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Title",patientSynch.Title,patient.Title));
				patientSynch.Title=patient.Title;
			}
			if(patientSynch.Preferred.ToLower()!=patient.Preferred.ToLower()) {
				if(patientSynch.Preferred!="" && patient.Preferred=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Preferred Name");
				}
				string preferred=patient.Preferred.ToUpper();
				if(isSynchTheMaster) {//We are synching a clone to the master, do NOT update the master's field to all caps.
					preferred=StringTools.ToUpperFirstOnly(patient.Preferred);
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Preferred Name",patientSynch.Preferred,preferred));
				patientSynch.Preferred=preferred;
			}
			if(patientSynch.MiddleI.ToLower()!=patient.MiddleI.ToLower()) {
				if(patientSynch.MiddleI!=""	&& patient.MiddleI=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Middle Initial");
				}
				string middleI=patient.MiddleI.ToUpper();
				if(isSynchTheMaster) {//We are synching a clone to the master, do NOT update the master's field to all caps.
					middleI=StringTools.ToUpperFirstOnly(patient.MiddleI);
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Middle Initial",patientSynch.MiddleI,middleI));
				patientSynch.MiddleI=middleI;
			}
			if(patientSynch.Birthdate!=patient.Birthdate) {
				if(patientSynch.Birthdate.Year > 1880 && patient.Birthdate.Year < 1880) {
					patientCloneDemoChanges.ListFieldsCleared.Add("Birthdate");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Birthdate",patientSynch.Birthdate.ToShortDateString(),patient.Birthdate.ToShortDateString()));
				patientSynch.Birthdate=patient.Birthdate;
			}
			//As of v17.2, it is desirable to allow patient clones to be in different families... whatever.
			//if(patientSynch.Guarantor!=patient.Guarantor) {
			//	Patient patCloneGuar=Patients.GetPat(patientSynch.Guarantor);
			//	Patient patNonCloneGuar=Patients.GetPat(patient.Guarantor);
			//	string strPatCloneGuarName="";
			//	string strPatNonCloneGuarName="";
			//	if(patCloneGuar!=null) {
			//		strPatCloneGuarName=Patients.GetNameFL(patCloneGuar.LName,patCloneGuar.FName,patCloneGuar.Preferred,patCloneGuar.MiddleI);
			//	}
			//	if(patNonCloneGuar!=null) {
			//		strPatNonCloneGuarName=Patients.GetNameFL(patNonCloneGuar.LName,patNonCloneGuar.FName,patNonCloneGuar.Preferred,patNonCloneGuar.MiddleI);
			//	}
			//	patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Guarantor",patientSynch.Guarantor.ToString()+" - "+strPatCloneGuarName,patient.Guarantor.ToString()+" - "+strPatNonCloneGuarName));
			//	patientSynch.Guarantor=patient.Guarantor;
			//}
			if(patientSynch.ResponsParty!=patient.ResponsParty) {
				Patient patCloneRespPart=Patients.GetPat(patientSynch.ResponsParty);
				Patient patNonCloneRespPart=Patients.GetPat(patient.ResponsParty);
				string strPatCloneRespPartName="";
				string strPatNonCloneRespPartName="";
				if(patCloneRespPart!=null) {
					strPatCloneRespPartName=Patients.GetNameFL(patCloneRespPart.LName,patCloneRespPart.FName,patCloneRespPart.Preferred,patCloneRespPart.MiddleI);
				}
				if(patNonCloneRespPart!=null) {
					strPatNonCloneRespPartName=Patients.GetNameFL(patNonCloneRespPart.LName,patNonCloneRespPart.FName,patNonCloneRespPart.Preferred,patNonCloneRespPart.MiddleI);
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Responsible Party",patientSynch.ResponsParty.ToString()+" - "+strPatCloneRespPartName,patient.ResponsParty.ToString()+" - "+strPatNonCloneRespPartName ));
				patientSynch.ResponsParty=patient.ResponsParty;
			}
			//As of v17.2, it is desirable to allow patient clones to be in different super families... whatever.
			//if(patientSynch.SuperFamily!=patient.SuperFamily) {
			//	Patient patCloneSupFam=Patients.GetPat(patientSynch.SuperFamily);
			//	Patient patNonCloneSupFam=Patients.GetPat(patient.SuperFamily);
			//	string strPatCloneSupFamName="";
			//	string strPatNonCloneSupFamName="";
			//	if(patCloneSupFam!=null) {
			//		strPatCloneSupFamName=Patients.GetNameFL(patCloneSupFam.LName,patCloneSupFam.FName,patCloneSupFam.Preferred,patCloneSupFam.MiddleI);
			//	}
			//	if(patNonCloneSupFam!=null) {
			//		strPatNonCloneSupFamName=Patients.GetNameFL(patNonCloneSupFam.LName,patNonCloneSupFam.FName,patNonCloneSupFam.Preferred,patNonCloneSupFam.MiddleI);
			//	}
			//	patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Super Family",patientSynch.SuperFamily.ToString()+" - "+strPatCloneSupFamName,patient.SuperFamily.ToString()+" - "+strPatNonCloneSupFamName ));
			//	patientSynch.SuperFamily=patient.SuperFamily;
			//}
			if(patientSynch.PatStatus!=patient.PatStatus) {
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Patient Status",patientSynch.PatStatus.ToString(),patient.PatStatus.ToString() ));
				patientSynch.PatStatus=patient.PatStatus;
			}
			if(patientSynch.Gender!=patient.Gender) {
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Gender",patientSynch.Gender.ToString(),patient.Gender.ToString() ));
				patientSynch.Gender=patient.Gender;
			}
			if(patientSynch.Language!=patient.Language) {
				string strPatCloneLang="";
				string strPatNonCloneLang="";
				try {
					strPatCloneLang=CodeBase.MiscUtils.GetCultureFromThreeLetter(patientSynch.Language).DisplayName;
				}
				catch {
					strPatCloneLang=patientSynch.Language;
				}
				try {
					strPatNonCloneLang=CodeBase.MiscUtils.GetCultureFromThreeLetter(patient.Language).DisplayName;
				}
				catch {
					strPatNonCloneLang=patient.Language;
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Language",strPatCloneLang,strPatNonCloneLang));
				patientSynch.Language=patient.Language;
			}
			if(patientSynch.SSN!=patient.SSN) {
				if(patientSynch.SSN!=""	&& patient.SSN=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("SSN");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("SSN",patientSynch.SSN,patient.SSN));
				patientSynch.SSN=patient.SSN;
			}
			if(patientSynch.Position!=patient.Position) {
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Position",patientSynch.Position.ToString(),patient.Position.ToString()));
				patientSynch.Position=patient.Position;
			}
			if(patientSynch.Address!=patient.Address) {
				if(patientSynch.Address!=""	&& patient.Address=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Address");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Address",patientSynch.Address,patient.Address));
				patientSynch.Address=patient.Address;
			}
			if(patientSynch.Address2!=patient.Address2) {
				if(patientSynch.Address2!="" && patient.Address2=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Address2");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Address2",patientSynch.Address2,patient.Address2));
				patientSynch.Address2=patient.Address2;
			}
			if(patientSynch.City!=patient.City) {
				if(patientSynch.City!="" && patient.City=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("City");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("City",patientSynch.City,patient.City));
				patientSynch.City=patient.City;
			}
			if(patientSynch.State!=patient.State) {
				if(patientSynch.State!=""	&& patient.State=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("State");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("State",patientSynch.State,patient.State));
				patientSynch.State=patient.State;
			}
			if(patientSynch.Zip!=patient.Zip) {
				if(patientSynch.Zip!=""	&& patient.Zip=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Zip");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Zip",patientSynch.Zip,patient.Zip));
				patientSynch.Zip=patient.Zip;
			}
			if(patientSynch.County!=patient.County) {
				if(patientSynch.County!=""	&& patient.County=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("County");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("County",patientSynch.County,patient.County));
				patientSynch.County=patient.County;
			}
			if(patientSynch.AddrNote!=patient.AddrNote) {
				if(patientSynch.AddrNote!=""	&& patient.AddrNote=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Address Note");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Address Note",patientSynch.AddrNote,patient.AddrNote));
				patientSynch.AddrNote=patient.AddrNote;
			}
			if(patientSynch.HmPhone!=patient.HmPhone) {
				if(patientSynch.HmPhone!=""	&& patient.HmPhone=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Home Phone");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Home Phone",patientSynch.HmPhone,patient.HmPhone));
				patientSynch.HmPhone=patient.HmPhone;
			}
			if(patientSynch.WirelessPhone!=patient.WirelessPhone) {
				if(patientSynch.WirelessPhone!=""	&& patient.WirelessPhone=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Wireless Phone");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Wireless Phone",patientSynch.WirelessPhone,patient.WirelessPhone));
				patientSynch.WirelessPhone=patient.WirelessPhone;
			}
			if(patientSynch.WkPhone!=patient.WkPhone) {
				if(patientSynch.WkPhone!=""	&& patient.WkPhone=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Work Phone");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Work Phone",patientSynch.WkPhone,patient.WkPhone));
				patientSynch.WkPhone=patient.WkPhone;
			}
			if(patientSynch.Email!=patient.Email) {
				if(patientSynch.Email!=""	&& patient.Email=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Email");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Email",patientSynch.Email,patient.Email));
				patientSynch.Email=patient.Email;
			}
			if(patientSynch.TxtMsgOk!=patient.TxtMsgOk) {
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("TxtMsgOk",patientSynch.TxtMsgOk.ToString(),patient.TxtMsgOk.ToString()));
				patientSynch.TxtMsgOk=patient.TxtMsgOk;
			}
			if(patientSynch.BillingType!=patient.BillingType) {
				Def defCloneBillingType=Defs.GetDef(DefCat.BillingTypes,patientSynch.BillingType);
				Def defNonCloneBillingType=Defs.GetDef(DefCat.BillingTypes,patient.BillingType);
				string cloneBillType=(defCloneBillingType==null ? "" : defCloneBillingType.ItemName);
				string nonCloneBillType=(defNonCloneBillingType==null ? "" : defNonCloneBillingType.ItemName);
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Billing Type",cloneBillType,nonCloneBillType));
				patientSynch.BillingType=patient.BillingType;
			}
			if(patientSynch.FeeSched!=patient.FeeSched) {
				FeeSched cloneFeeSchedObj=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==patientSynch.FeeSched);
				FeeSched nonCloneFeeSchedObj=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==patient.FeeSched);
				string cloneFeeSched=(cloneFeeSchedObj==null ? "" : cloneFeeSchedObj.Description);
				string nonCloneFeeSched=(nonCloneFeeSchedObj==null ? "" : nonCloneFeeSchedObj.Description);
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Fee Schedule",cloneFeeSched,nonCloneFeeSched));
				patientSynch.FeeSched=patient.FeeSched;
			}
			if(patientSynch.CreditType!=patient.CreditType) {
				if(patientSynch.CreditType!=""	&& patient.CreditType=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Credit Type");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Credit Type",patientSynch.CreditType,patient.CreditType));
				patientSynch.CreditType=patient.CreditType;
			}
			if(patientSynch.MedicaidID!=patient.MedicaidID) {
				if(patientSynch.MedicaidID!=""	&& patient.MedicaidID=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Medicaid ID");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Medicaid ID",patientSynch.MedicaidID,patient.MedicaidID));
				patientSynch.MedicaidID=patient.MedicaidID;
			}
			if(patientSynch.MedUrgNote!=patient.MedUrgNote) {
				if(patientSynch.MedUrgNote!=""	&& patient.MedUrgNote=="") {
					patientCloneDemoChanges.ListFieldsCleared.Add("Medical Urgent Note");
				}
				patientCloneDemoChanges.ListFieldsUpdated.Add(new PatientCloneField("Medical Urgent Note",patientSynch.MedUrgNote,patient.MedUrgNote));
				patientSynch.MedUrgNote=patient.MedUrgNote;
			}
			#endregion Synch Clone Data - Patient Demographics
			return patientCloneDemoChanges;
		}

		///<summary>Synchs the pat plan information from patient to patientSynch passed in.
		///Returns a PatientClonePatPlanChanges object that represents specifics regarding anything that changed during the synch.
		///Optionally pass in the lists of insurance information in order to potentially save db calls.</summary>
		public static PatientClonePatPlanChanges SynchClonePatPlans(Patient patient,Patient patientSynch,Family familyCur,List<InsPlan> listInsPlans
			,List<InsSub> listInsSubs,List<Benefit> listBenefits,List<PatPlan> listPatPlans=null,List<PatPlan> listPatPlansForSynch=null)
		{
			//No need to check RemotingRole; no call to db and private method with arguments that act like out parameters.
			//TODO: correct all messages so that they don't refer to "the clone" or "the original".
			PatientClonePatPlanChanges patientClonePatPlanChanges=new PatientClonePatPlanChanges();
			#region Synch Clone Data - PatPlans
			patientClonePatPlanChanges.PatPlansChanged=false;
			patientClonePatPlanChanges.PatPlansInserted=false;
			patientClonePatPlanChanges.StrDataUpdated="";
			if(listPatPlans==null) {
				listPatPlans=PatPlans.Refresh(patient.PatNum);//ordered by ordinal
			}
			if(listPatPlansForSynch==null) {
				listPatPlansForSynch=PatPlans.Refresh(patientSynch.PatNum);//ordered by ordinal
			}
			List<Claim> claimList=Claims.Refresh(patientSynch.PatNum);//used to determine if the patplan we are going to drop is attached to a claim with today's date
			for(int i=claimList.Count-1;i>-1;i--) {//remove any claims that do not have a date of today, we are only concerned with claims with today's date
				if(claimList[i].DateService==DateTime.Today) {
					continue;
				}
				claimList.RemoveAt(i);
			}
			//if the clone has more patplans than the non-clone, drop the additional patplans
			//we will compute all estimates for the clone after all of the patplan adding/dropping/rearranging
			for(int i=listPatPlansForSynch.Count-1;i>listPatPlans.Count-1;i--) {
				InsSub insSubCloneCur=InsSubs.GetOne(listPatPlansForSynch[i].InsSubNum);
				//we will drop the clone's patplan because the clone has more patplans than the non-clone
				//before we can drop the plan, we have to make sure there is not a claim with today's date
				bool isAttachedToClaim=false;
				for(int j=0;j<claimList.Count;j++) {//claimList will only contain claims with DateService=Today
					if(claimList[j].PlanNum!=insSubCloneCur.PlanNum) {//different insplan
						continue;
					}
					patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","Insurance Plans do not match.  "
						+"Due to a claim with today's date we cannot synch the plans, the issue must be corrected manually on the following plan")
						+": "+InsPlans.GetDescript(insSubCloneCur.PlanNum,familyCur,listInsPlans,listPatPlansForSynch[i].InsSubNum,listInsSubs)+".\r\n";
					isAttachedToClaim=true;
					break;
				}
				if(isAttachedToClaim) {//we will continue trying to drop non-clone additional plans, but only if no claim for today exists
					continue;
				}
				patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The following insurance plan was dropped due to it not existing with the same ordinal on the original patient")+": "
					+InsPlans.GetDescript(insSubCloneCur.PlanNum,familyCur,listInsPlans,listPatPlansForSynch[i].InsSubNum,listInsSubs)+".\r\n";
				patientClonePatPlanChanges.PatPlansChanged=true;
				PatPlans.DeleteNonContiguous(listPatPlansForSynch[i].PatPlanNum);
				listPatPlansForSynch.RemoveAt(i);
			}
			for(int i=0;i<listPatPlans.Count;i++) {
				InsSub insSubNonCloneCur=InsSubs.GetOne(listPatPlans[i].InsSubNum);
				string insPlanNonCloneDescriptCur=InsPlans.GetDescript(insSubNonCloneCur.PlanNum,familyCur,listInsPlans,listPatPlans[i].InsSubNum,listInsSubs);
				if(listPatPlansForSynch.Count<i+1) {//if there is not a PatPlan at this ordinal position for the clone, add a new one that is an exact copy, with correct PatNum of course
					PatPlan patPlanNew=listPatPlans[i].Copy();
					patPlanNew.PatNum=patientSynch.PatNum;
					PatPlans.Insert(patPlanNew);
					patientClonePatPlanChanges.PatPlansInserted=true;
					patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The following insurance was added")+": "+insPlanNonCloneDescriptCur+".\r\n";
					patientClonePatPlanChanges.PatPlansChanged=true;
					continue;
				}
				InsSub insSubCloneCur=InsSubs.GetOne(listPatPlansForSynch[i].InsSubNum);
				string insPlanCloneDescriptCur=InsPlans.GetDescript(insSubCloneCur.PlanNum,familyCur,listInsPlans,listPatPlansForSynch[i].InsSubNum,listInsSubs);
				if(listPatPlans[i].InsSubNum!=listPatPlansForSynch[i].InsSubNum) {//both pats have a patplan at this ordinal, but the clone's is pointing to a different inssub
					//we will drop the clone's patplan and add the non-clone's patplan
					//before we can drop the plan, we have to make sure there is not a claim with today's date
					bool isAttachedToClaim=false;
					for(int j=0;j<claimList.Count;j++) {//claimList will only contain claims with DateService=Today
						if(claimList[j].PlanNum!=insSubCloneCur.PlanNum) {//different insplan
							continue;
						}
						patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","Insurance Plans do not match.  "
							+"Due to a claim with today's date we cannot synch the plans, the issue must be corrected manually on the following plan")
							+": "+insPlanCloneDescriptCur+".\r\n";
						isAttachedToClaim=true;
						break;
					}
					if(isAttachedToClaim) {//if we cannot change this plan to match the non-clone's plan at the same ordinal, we will synch the rest of the plans and let the user know to fix manually
						continue;
					}
					patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The following plan was updated to match the selected patient's plan")+": "+insPlanCloneDescriptCur+".\r\n";
					patientClonePatPlanChanges.PatPlansChanged=true;
					PatPlans.DeleteNonContiguous(listPatPlansForSynch[i].PatPlanNum);//we use the NonContiguous version because we are going to insert into this same ordinal, compute estimates will happen at the end of all the changes
					PatPlan patPlanCopy=listPatPlans[i].Copy();
					patPlanCopy.PatNum=patientSynch.PatNum;
					PatPlans.Insert(patPlanCopy);
				}
				else {
					//both clone and non-clone have the same patplan.InsSubNum at this position in their list, just make sure all data in the patplans match
					if(listPatPlans[i].Ordinal!=listPatPlansForSynch[i].Ordinal) {
						patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The ordinal of the insurance plan")+" "+insPlanCloneDescriptCur+" "
							+Lans.g("ContrFamily","was updated to")+" "+listPatPlans[i].Ordinal.ToString()+".\r\n";
						patientClonePatPlanChanges.PatPlansChanged=true;
						listPatPlansForSynch[i].Ordinal=listPatPlans[i].Ordinal;
					}
					if(listPatPlans[i].IsPending!=listPatPlansForSynch[i].IsPending) {
						patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The pending status of the insurance plan")+" "+insPlanCloneDescriptCur+" "
							+Lans.g("ContrFamily","was updated to")+" "+listPatPlans[i].IsPending.ToString()+".\r\n";
						patientClonePatPlanChanges.PatPlansChanged=true;
						listPatPlansForSynch[i].IsPending=listPatPlans[i].IsPending;
					}
					if(listPatPlans[i].Relationship!=listPatPlansForSynch[i].Relationship) {
						patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The relationship to the subscriber of the insurance plan")+" "+insPlanCloneDescriptCur+" "
							+Lans.g("ContrFamily","was updated to")+" "+listPatPlans[i].Relationship.ToString()+".\r\n";
						patientClonePatPlanChanges.PatPlansChanged=true;
						listPatPlansForSynch[i].Relationship=listPatPlans[i].Relationship;
					}
					if(listPatPlans[i].PatID!=listPatPlansForSynch[i].PatID) {
						patientClonePatPlanChanges.StrDataUpdated+=Lans.g("ContrFamily","The patient ID of the insurance plan")+" "+insPlanCloneDescriptCur+" "
							+Lans.g("ContrFamily","was updated to")+" "+listPatPlans[i].PatID+".\r\n";
						patientClonePatPlanChanges.PatPlansChanged=true;
						listPatPlansForSynch[i].PatID=listPatPlans[i].PatID;
					}
					PatPlans.Update(listPatPlansForSynch[i]);
				}
			}
			if(patientClonePatPlanChanges.PatPlansInserted) {
				SecurityLogs.MakeLogEntry(Permissions.PatPlanCreate,0,Lans.g("ContrFamily","One or more PatPlans created via Synch Clone tool."));
			}
			if(patientClonePatPlanChanges.PatPlansChanged) {
				//compute all estimates for clone after making changes to the patplans
				List<ClaimProc> claimProcs=ClaimProcs.Refresh(patientSynch.PatNum);
				List<Procedure> procs=Procedures.Refresh(patientSynch.PatNum);
				listPatPlansForSynch=PatPlans.Refresh(patientSynch.PatNum);
				listInsSubs=InsSubs.RefreshForFam(familyCur);
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				listBenefits=Benefits.Refresh(listPatPlansForSynch,listInsSubs);
				Procedures.ComputeEstimatesForAll(patientSynch.PatNum,claimProcs,procs,listInsPlans,listPatPlansForSynch,listBenefits,patientSynch.Age,listInsSubs);
				Patients.SetHasIns(patientSynch.PatNum);
			}
			#endregion Synch Clone Data - PatPlans
			return patientClonePatPlanChanges;
		}
		#endregion

		#region Misc Methods

		///<summary>Returns true if the patient passed in is a clone otherwise false.</summary>
		public static bool IsPatientAClone(long patNum) {
			//No need to check RemotingRole; no call to db.
			return PatientLinks.IsPatientAClone(patNum);
		}

		///<summary>Returns true if the patient passed in is a clone or the original patient of clones, otherwise false.
		///This method is helpful when trying to determine if the patient passed in is related in any way to the clone system.</summary>
		public static bool IsPatientACloneOrOriginal(long patNum) {
			//No need to check RemotingRole; no call to db.
			return PatientLinks.IsPatientACloneOrOriginal(patNum);
		}

		///<summary>Returns true if one patient is a clone of the other or if both are clones of the same master, otherwise false.
		///Always returns false if patNum1 and patNum2 are the same PatNum.</summary>
		public static bool ArePatientsClonesOfEachOther(long patNum1,long patNum2) {
			//No need to check RemotingRole; no call to db.
			return PatientLinks.ArePatientsClonesOfEachOther(patNum1,patNum2);
		}

		///<summary>Replaces all patient fields in the given message with the given patient's information.  Returns the resulting string.
		///Replaces: [FName], [LName], [LNameLetter], [NameF], [NameFL], [PatNum], 
		///[ChartNumber], [HmPhone], [WkPhone], [WirelessPhone], [ReferredFromProvNameFL], etc.</summary>
		public static string ReplacePatient(string message,Patient pat,bool isHtmlEmail=false) {
			if(pat==null) {
				return message;
			}
			//Use patient's preferred name if they have one, otherwise default to first name.
			string strPatPreferredOrFName=pat.FName;
			if(!string.IsNullOrWhiteSpace(pat.Preferred)) {
				strPatPreferredOrFName=pat.Preferred;
			}
			StringBuilder template=new StringBuilder(message);
			ReplaceTags.ReplaceOneTag(template,"[FName]",pat.FName,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[LName]",pat.LName,isHtmlEmail);
			string lNameLetter="";
			if(pat.LName?.Length>0) {
				lNameLetter=pat.LName.Substring(0,1).ToUpper();
			}
			ReplaceTags.ReplaceOneTag(template,"[LNameLetter]",lNameLetter,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[NameF]",pat.FName,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[NameFL]",Patients.GetNameFL(pat.LName,pat.FName,pat.Preferred,pat.MiddleI),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[NameLF]",GetNameLF(pat),isHtmlEmail);;
			ReplaceTags.ReplaceOneTag(template,"[NamePreferredOrFirst]",strPatPreferredOrFName,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[PatNum]",pat.PatNum.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ChartNumber]",pat.ChartNumber,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[HmPhone]",pat.HmPhone,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[WkPhone]",pat.WkPhone,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Gender]",pat.Gender.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Email]",pat.Email,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ProvNum]",pat.PriProv.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ClinicNum]",pat.ClinicNum.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[WirelessPhone]",pat.WirelessPhone,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Birthdate]",pat.Birthdate.ToShortDateString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Birthdate_yyyyMMdd]",pat.Birthdate.ToString("yyyyMMdd"),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[SSN]",pat.SSN,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Address]",pat.Address,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Address2]",pat.Address2,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[City]",pat.City,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[State]",pat.State,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[Zip]",pat.Zip,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[MonthlyCardsOnFile]",CreditCards.GetMonthlyCardsOnFile(pat.PatNum),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[PatientTitle]",pat.Title,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[PatientMiddleInitial]",pat.MiddleI,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[PatientPreferredName]",pat.Preferred,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[PrimaryProviderNameFLSuffix]",Providers.GetFormalName(pat.PriProv),isHtmlEmail);
			if(message.Contains("[ReferredFromProvNameFL]")) {
				Referral patRef = Referrals.GetReferralForPat(pat.PatNum);
				if(patRef!=null) {
					ReplaceTags.ReplaceOneTag(template,"[ReferredFromProvNameFL]",Patients.GetNameFL(patRef.LName,patRef.FName,"",""),isHtmlEmail);
				}
				else {
					ReplaceTags.ReplaceOneTag(template,"[ReferredFromProvNameFL]","",isHtmlEmail);
				}
			}
			if(message.Contains("[PatientGenderMF]")) {
				if(pat.Gender==PatientGender.Male) {
					ReplaceTags.ReplaceOneTag(template,"[PatientGenderMF]","M",isHtmlEmail);
				}
				else if(pat.Gender==PatientGender.Female) {
					ReplaceTags.ReplaceOneTag(template,"[PatientGenderMF]","F",isHtmlEmail);
				}
				else {
					ReplaceTags.ReplaceOneTag(template,"[PatientGenderMF]","",isHtmlEmail);
				}
			}
			return template.ToString();
		}

		///<summary>Replaces all patient guarantor fields in the given message with the given patient's guarantor information.  Returns the resulting string.
		///Replaces: [GuarantorPatnum], [GuarantorTitle], [GuarantorNameF], [GuarantorNameL], [GuarantorMiddleInitial],etc.</summary>
		public static string ReplaceGuarantor(string message,Patient pat) {
			if(pat==null) {
				return message;
			}
			Patient guar=Patients.GetPat(pat.Guarantor);
			if(guar==null) {
				return message;
			}
			string retVal=message;
			retVal=retVal.Replace("[GuarantorPatNum]",guar.PatNum.ToString());
			retVal=retVal.Replace("[GuarantorTitle]",guar.Title);
			retVal=retVal.Replace("[GuarantorNameF]",guar.FName);
			retVal=retVal.Replace("[GuarantorNameL]",guar.LName);
			retVal=retVal.Replace("[GuarantorMiddleInitial]",guar.MiddleI);
			retVal=retVal.Replace("[GuarantorHmPhone]",guar.HmPhone);
			retVal=retVal.Replace("[GuarantorWkPhone]",guar.WkPhone);
			retVal=retVal.Replace("[GuarantorAddress]",guar.Address);
			retVal=retVal.Replace("[GuarantorAddress2]",guar.Address2);
			retVal=retVal.Replace("[GuarantorCity]",guar.City);
			retVal=retVal.Replace("[GuarantorState]",guar.State);
			retVal=retVal.Replace("[GuarantorZip]",guar.Zip);
			return retVal;
		}

		///<summary>Returns true if the replacement field is PHI. Case insensitive.</summary>
		public static bool IsFieldPHI(string field) {
			return ListTools.In(field.ToLower(),ListPHIFields.Select(x => x.ToLower()));
		}

		///<summary>Returns true if the text contains a replacement field that is PHI. Case insensitive.</summary>
		public static bool DoesContainPHIField(string text) {
			string textLower=text.ToLower();
			return Patients.ListPHIFields.Select(x => x.ToLower()).Any(x => textLower.Contains(x));
		}

		///<summary>The list of fields that are considered PHI. 
		///<para/>According to the United States Electronic Code of Federal Regulations Title 45 160.103, protected health information is individually 
		///identifiable health information that:
		///"... (1) Is created or received by a health care provider, health plan, employer, or health care clearinghouse; and
		///(2) Relates to the past, present, or future physical or mental health or condition of an individual; the provision of health care to an individual; or the past, present, or future payment for the provision of health care to an individual; and
		///(i) That identifies the individual; or
		///(ii) With respect to which there is a reasonable basis to believe the information can be used to identify the individual".
		///(https://www.ecfr.gov/cgi-bin/text-idx?SID=2f948e08dbf4b32b8e30a4f0ac6f66cf&amp;mc=true&amp;node=se45.1.160_1103&amp;rgn=div8)</summary>
		public static List<string> ListPHIFields {
			get {
				return new List<string> {
					"[LName]",
					"[NameFL]",
					"[NameLF]",
					"[WirelessPhone]",
					"[HmPhone]",
					"[WkPhone]",
					"[Birthdate]",
					"[Birthdate_yyyyMMdd]",
					"[SSN]",
					"[Address]",
					"[Address2]",
					"[City]",
					"[Zip]",
				};
			}
		}

		///<summary>For sales tax. True if the patient zipcode matches the pattern "12345" or "12345-6789" </summary>
		public static bool HasValidUSZipCode(Patient patient) {
			//No need to check RemotingRole; no call to db.
			//Patient pat=Patients.GetPat(patNum);
			//Regular Expression found at:
			//https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s14.html
			string regexp="^[0-9]{5}(?:-[0-9]{4})?$";
			if(Regex.IsMatch(patient.Zip,regexp)) {
				return true;
			}
			return false;
		}

		#endregion

		///<summary>Creates and inserts a "new patient" using the information passed in.  Validation must be done prior to calling this.
		///securityLogMsg is typically set to something that lets the customer know where this new patient was created from.
		///Used by multiple applications so be very careful when changing this method.  E.g. Open Dental and Web Sched.</summary>
		public static Patient CreateNewPatient(string lName,string fName,DateTime birthDate,long priProv,long clinicNum,string securityLogMsg
			,LogSources logSource=LogSources.None,string email="",string hmPhone="",string wirelessPhone="",PatientStatus patStatus=PatientStatus.Patient,
			long patNum=0,bool setTxtOk=false)
		{
			//No need to check RemotingRole; no call to db.
			Patient patient=new Patient();
			patient.LName=CreateNewPatientNameHelper(lName);
			patient.FName=CreateNewPatientNameHelper(fName);
			bool doUseExistingPK=false;
			if(patNum>0) {
				patient.PatNum=patNum;
				doUseExistingPK=true;
			}
			patient.Birthdate=birthDate;
			patient.PatStatus=patStatus;
			patient.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,clinicNum));
			patient.PriProv=priProv;
			patient.Gender=PatientGender.Unknown;
			patient.ClinicNum=clinicNum;
			patient.Email=email;
			patient.HmPhone=TelephoneNumbers.AutoFormat(hmPhone);
			patient.WirelessPhone=TelephoneNumbers.AutoFormat(wirelessPhone);
			if(setTxtOk && !wirelessPhone.IsNullOrEmpty()) {
				patient.TxtMsgOk=YN.Yes;
			}
			Patients.Insert(patient,doUseExistingPK);
			SecurityLogs.MakeLogEntry(Permissions.PatientCreate,patient.PatNum,securityLogMsg,logSource);
			CustReference custRef=new CustReference();
			custRef.PatNum=patient.PatNum;
			CustReferences.Insert(custRef);
			Patient PatOld=patient.Copy();
			patient.Guarantor=patient.PatNum;
			Patients.Update(patient,PatOld);
			return patient;
		}
		
		///<summary>Helper method to address the situation where a patient's first or last name is only one character long but is still saved to the DB.</summary>
		private static string CreateNewPatientNameHelper(string name) {
			if(name.Length==1) {
				return name.ToUpper();
			}
			else if(name.Length>1) {//eg Sp
				return name.Substring(0,1).ToUpper()+name.Substring(1);
			}
			else {
				return "";
			}
		}

		///<summary>Returns a Family object for the supplied patNum.  Use Family.GetPatient to extract the desired patient from the family.</summary>
		public static Family GetFamily(long patNum) {
			//No need to check RemotingRole; no call to db.
			return ODMethodsT.Coalesce(GetFamilies(new List<long>() { patNum }).FirstOrDefault(),new Family());
		}

		public static List<Family> GetFamilies(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Family>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums==null || listPatNums.Count < 1) {
				return new List<OpenDentBusiness.Family>();
			}
			string command=@"SELECT DISTINCT f.*,CASE WHEN f.Guarantor != f.PatNum THEN 1 ELSE 0 END AS IsNotGuar 
				FROM patient p
				INNER JOIN patient f ON f.Guarantor=p.Guarantor
				WHERE p.PatNum IN ("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+@")
				ORDER BY IsNotGuar, f.Birthdate";
			List<Family> listFamilies=new List<Family>();
			List<Patient> listPatients=Crud.PatientCrud.SelectMany(command);
			foreach(Patient patient in listPatients) {
				patient.Age = DateToAge(patient.Birthdate);
			}
			Dictionary<long,List<Patient>> dictFamilyPatients=listPatients.GroupBy(x => x.Guarantor)
				.ToDictionary(y => y.Key,y => y.ToList());
			foreach(KeyValuePair<long,List<Patient>> kvp in dictFamilyPatients) {
				Family family=new Family();
				family.ListPats=kvp.Value.ToArray();
				listFamilies.Add(family);
			}
			return listFamilies;
		}

		///<summary>Returns a list of patients that have the associated FeeSchedNum.  Used when attempting to hide FeeScheds.</summary>
		public static List<Patient> GetForFeeSched(long feeSchedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),feeSchedNum);
			}
			string command="SELECT * FROM patient WHERE FeeSched="+POut.Long(feeSchedNum);
			return Crud.PatientCrud.SelectMany(command);
		}

		/// <summary>Returns a patient, or null, based on an internally defined or externally defined globaly unique identifier.  This can be an OID, GUID, IID, UUID, etc.</summary>
		/// <param name="IDNumber">The extension portion of the GUID/OID.  Example: 333224444 if using SSN as a the unique identifier</param>
		/// <param name="OID">root OID that the IDNumber extends.  Example: 2.16.840.1.113883.4.1 is the OID for the Social Security Numbers.</param>
		public static Patient GetByGUID(string IDNumber, string OID){
			if(OID==OIDInternals.GetForType(IdentifierType.Patient).IDRoot) {//OID matches the localy defined patnum OID.
				return Patients.GetPat(PIn.Long(IDNumber));
			}
			else {
				OIDExternal oidExt=OIDExternals.GetByRootAndExtension(OID,IDNumber);
				if(oidExt==null || oidExt.IDType!=IdentifierType.Patient) {
					return null;//OID either not found, or does not represent a patient.
				}
				return Patients.GetPat(oidExt.IDInternal);
			}
		}

		///<summary>This is a way to get a single patient from the database if you don't already have a family object to use.  Will return null if not found.</summary>
		public static Patient GetPat(long patNum) {
			if(patNum==0) {
				return null;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod(),patNum);
			} 
			Patient pat=Crud.PatientCrud.SelectOne(patNum);
			if(pat==null) {
				return null;//used in eCW bridge
			}
			pat.Age = DateToAge(pat.Birthdate);
			return pat;
		}

		///<summary>Will return null if not found.</summary>
		public static Patient GetPatByChartNumber(string chartNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod(),chartNumber);
			}
			if(chartNumber=="") {
				return null;
			}
			string command="SELECT * FROM patient WHERE ChartNumber='"+POut.String(chartNumber)+"'";
			Patient pat=null;
			try {
				pat=Crud.PatientCrud.SelectOne(command);
			}
			catch { }
			if(pat==null) {
				return null;
			}
			pat.Age = DateToAge(pat.Birthdate);
			return pat;
		}

		///<summary>Will return null if not found.</summary>
		public static Patient GetPatBySSN(string ssn) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod(),ssn);
			}
			if(ssn=="") {
				return null;
			}
			string command="SELECT * FROM patient WHERE SSN='"+POut.String(ssn)+"'";
			Patient pat=null;
			try {
				pat=Crud.PatientCrud.SelectOne(command);
			}
			catch { }
			if(pat==null) {
				return null;
			}
			pat.Age = DateToAge(pat.Birthdate);
			return pat;
		}

		///<summary>Gets all of the PatNums for the family members of the PatNums passed in.  Returns a distinct list of PatNums.</summary>
		public static List<long> GetAllFamilyPatNums(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums==null || listPatNums.Count<1) {
				return new List<long>();
			}
			string command="SELECT patient.PatNum FROM patient "
				+"INNER JOIN ("
					+"SELECT DISTINCT Guarantor FROM patient WHERE PatNum IN ("+string.Join(",",listPatNums)+")"
					+") guarnums ON guarnums.Guarantor=patient.Guarantor "
				+"WHERE patient.PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			return Db.GetListLong(command);
		}

		///<summary>Gets all of the PatNums for the family members of the Guarantor nums passed in.  Returns a distinct list of PatNums that will include
		///the guarantor PatNums passed in and will include all PatStatuses including archived and deleted.  Used in Ledgers.cs for aging.</summary>
		public static List<long> GetAllFamilyPatNumsForGuars(List<long> listGuarNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listGuarNums);
			}
			if(listGuarNums==null || listGuarNums.Count<1) {
				return new List<long>();
			}
			string command="SELECT PatNum FROM patient WHERE Guarantor IN ("+string.Join(",",listGuarNums)+")";
			return Db.GetListLong(command);
		}

		public static List<Patient> GetAllPatientsForGuarantor(long guarantorNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),guarantorNum);
			}
			string command="SELECT * FROM patient WHERE Guarantor="+POut.Long(guarantorNum);
			return Crud.PatientCrud.SelectMany(command);
		}

		public static List<long> GetAllFamilyPatNumsForSuperFam(List<long> listSuperFamNums) {
			listSuperFamNums?.RemoveAll(x => x<=0);//if list is not null, remove all nums <= 0
			if((listSuperFamNums?.Count??0)==0) {//if list is null or has no nums > 0, return new list
				return new List<long>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listSuperFamNums);
			}
			string command="SELECT PatNum FROM patient WHERE SuperFamily IN ("+string.Join(",",listSuperFamNums.Distinct())+")";
			return Db.GetListLong(command);
		}

		public static List<Patient> GetChangedSince(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT * FROM patient WHERE DateTStamp > "+POut.DateT(changedSince);
			//command+=" "+DbHelper.LimitAnd(1000);
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Used if the number of records are very large, in which case using GetChangedSince(DateTime changedSince) is not the preffered route due to memory problems caused by large recordsets. </summary>
		public static List<long> GetChangedSincePatNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT PatNum From patient WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> patnums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				patnums.Add(PIn.Long(dt.Rows[i]["PatNum"].ToString()));
			}
			return patnums;
		}

		public static List<PatientWithServerDT> GetPatientsSimpleForApi(int limit,int offset,string lName,string fName,
			DateTime birthdate,int patStatus,long clinicNum,DateTime dateTStamp,long priProv)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientWithServerDT>>(MethodBase.GetCurrentMethod(),limit,offset,lName,fName,birthdate,patStatus,clinicNum,dateTStamp,priProv);
			}
			string command="SELECT * FROM patient WHERE DateTStamp >= "+POut.DateT(dateTStamp)+" ";
			if(!lName.IsNullOrEmpty()) {
				command+="AND LName LIKE '%"+POut.String(lName)+"%' ";
			}
			if(!fName.IsNullOrEmpty()) {
				command+="AND FName LIKE '%"+POut.String(fName)+"%' ";
			}
			if(patStatus>-1) {
				command+="AND PatStatus="+POut.Int(patStatus)+" ";
			}
			if(clinicNum>-1) {
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(birthdate>DateTime.MinValue) {
				command+="AND patient.Birthdate="+POut.DateT(birthdate)+" ";
			}
			if(priProv>-1) {
				command+="AND PriProv="+POut.Long(priProv)+" ";
			}
			command+="ORDER BY PatNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before patients for rigorous inclusion of patient
			List<Patient> listPatients=Crud.PatientCrud.SelectMany(command);
			List<PatientWithServerDT> listPatientWithServerDTs=new List<PatientWithServerDT>();
			for(int i=0;i<listPatients.Count;i++) {
				PatientWithServerDT patientWithServerDT=new PatientWithServerDT();
				patientWithServerDT.PatientCur=listPatients[i];
				patientWithServerDT.DateTimeServer=dateTimeServer;
				listPatientWithServerDTs.Add(patientWithServerDT);
			}
			return listPatientWithServerDTs;
		}

		/// <summary>Gets PatNums of patients whose online password is  blank</summary>
		public static List<long> GetPatNumsForDeletion() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT PatNum FROM patient "
				+"LEFT JOIN userweb ON userweb.FKey=patient.PatNum "
					+"AND userweb.FKeyType="+POut.Int((int)UserWebFKeyType.PatientPortal)+" "
				+"WHERE userweb.FKey IS NULL OR userweb.Password='' ";
			return Db.GetListLong(command);
		}

		///<summary>ONLY for new patients. Set includePatNum to true for use the patnum from the import function.  Used in HL7.  Otherwise, uses InsertID to fill PatNum.</summary>
		public static long Insert(Patient pat,bool useExistingPK) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pat.PatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pat,useExistingPK);
				return pat.PatNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			pat.SecUserNumEntry=Security.CurUser.UserNum;
			pat.PatNum=Crud.PatientCrud.Insert(pat,useExistingPK);
			pat.SecurityHash=HashFields(pat);
			Crud.PatientCrud.Update(pat);
			if(PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)) {
				PhoneNumbers.SyncPat(pat);
			}
			return pat.PatNum;
		}

		///<summary>Updates only the changed columns and returns true if changes were made.  Supply the old Patient object to compare for changes.</summary>
		public static bool Update(Patient patient,Patient oldPatient) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patient,oldPatient);
			}
			if(IsPatientHashValid(oldPatient)) {
				patient.SecurityHash=HashFields(patient);
			}
			bool retval=Crud.PatientCrud.Update(patient,oldPatient);
			if(PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)
				&& (patient.HmPhone!=oldPatient.HmPhone
					|| patient.WkPhone!=oldPatient.WkPhone
					|| patient.WirelessPhone!=oldPatient.WirelessPhone))
			{
				PhoneNumbers.SyncPat(patient);
			}
			return retval;
		}

		///<summary>This is only used when entering a new patient and user clicks cancel.  It used to actually delete the patient, but that will mess up
		///UAppoint synch function.  DateTStamp needs to track deleted patients. So now, the PatStatus is simply changed to 4.</summary>
		public static void Delete(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat);
				return;
			}
			string command="UPDATE patient SET PatStatus="+POut.Long((int)PatientStatus.Deleted)+", "
				+"Guarantor=PatNum "
				+"WHERE PatNum ="+pat.PatNum.ToString();
			Db.NonQ(command);
			//no need to call PhoneNumbers.SyncPat since only the status and guar are changed here
		}

		///<summary>Only used for the Select Patient dialog.  Pass in a billing type of 0 for all billing types.</summary>
		public static DataTable GetPtDataTable(PtTableSearchParams ptSearchArgs)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),ptSearchArgs);
			}
			string exactMatchSnippet=GetExactMatchSnippet(ptSearchArgs);
			string phonedigits=StringTools.StripNonDigits(ptSearchArgs.Phone);
			string regexp="";
			for(int i=0;i<phonedigits.Length;i++){
				if(i!=0){
					regexp+="[^0-9]*";//zero or more intervening digits that are not numbers
				}
				if(i==3) {//If there is more than three digits and the first digit is 1, make it optional.
					if(phonedigits.StartsWith("1")) {
						regexp="1?"+regexp.Substring(1);
					}
					else {
						regexp="1?[^0-9]*"+regexp;//add a leading 1 so that 1-800 numbers can show up simply by typing in 800 followed by the number.
					}
				}
				regexp+=phonedigits[i];
			}
			regexp=SOut.String(regexp);//just escape necessary characters here one time
			//Replaces spaces and punctation with wildcards because users should be able to type the following example and match certain addresses:
			//Search term: "4145 S Court St" should match "4145 S. Court St." in the database.
			string strAddress=Regex.Replace(ptSearchArgs.Address, @"[°\-.,:;_""'/\\)(#\s&]","%");
			string phDigitsTrimmed=phonedigits.TrimStart('0','1');
			//a single digit search is faster using REGEXP, so only use the phonenumber table if the pref is set and the phDigitsTrimmed length>1
			bool usePhonenumTable=PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable) && phDigitsTrimmed.Length>0;
			bool useExactMatch=PrefC.GetBool(PrefName.EnterpriseExactMatchPhone);
			int exactMatchPhoneDigits=PrefC.GetInt(PrefName.EnterpriseExactMatchPhoneNumDigits);
			string phoneNumSearch="";
			string likeQueryString=$@"
					AND phonenumber.PhoneNumberDigits LIKE '%{POut.String(phDigitsTrimmed)}%'";
			if(usePhonenumTable) {
				if(useExactMatch && phDigitsTrimmed.Length==exactMatchPhoneDigits) {
					phoneNumSearch=$@"
					AND phonenumber.PhoneNumberDigits = '{POut.String(phDigitsTrimmed)}'";
				}
				else {
					phoneNumSearch=likeQueryString;
				}
			}
			string command=$@"SELECT DISTINCT patient.PatNum,patient.LName,patient.FName,patient.MiddleI,patient.Preferred,patient.Birthdate,patient.SSN,
				patient.HmPhone,patient.WkPhone,patient.Address,patient.PatStatus,patient.BillingType,patient.ChartNumber,patient.City,patient.State,
				patient.PriProv,patient.SiteNum,patient.Email,patient.Country,patient.ClinicNum,patient.SecProv,patient.WirelessPhone,
				{exactMatchSnippet} isExactMatch,"
				//using this sub-select instead of joining these two tables because the runtime is much better this way
				//Example: large db with joins single clinic 19.8 sec, all clinics 32.484 sec; with sub-select single clinic 0.054 sec, all clinics 0.007 sec
			+(!ptSearchArgs.HasSpecialty?"''":$@"
				(
					SELECT definition.ItemName FROM definition
					INNER JOIN deflink ON definition.DefNum=deflink.DefNum
					WHERE deflink.LinkType={SOut.Int((int)DefLinkType.Patient)}
					AND deflink.FKey=patient.PatNum
					AND definition.Category={SOut.Int((int)DefCat.ClinicSpecialty)}
					LIMIT 1
				)")+$@" Specialty,"//always include Specialty column, only populate if displaying specialty field
			+(!PrefC.GetBool(PrefName.DistributorKey)?"":$@"
				GROUP_CONCAT(DISTINCT phonenumber.PhoneNumberVal) AS OtherPhone,registrationkey.RegKey,")
			+(string.IsNullOrEmpty(ptSearchArgs.InvoiceNumber)?"'' ":"statement.")+$@"StatementNum
				FROM patient"
			+(!usePhonenumTable?"":$@"
				INNER JOIN phonenumber ON phonenumber.PatNum=patient.PatNum")
			+(!PrefC.GetBool(PrefName.DistributorKey)?"":$@"{(usePhonenumTable?"":$@"
				LEFT JOIN phonenumber ON phonenumber.PatNum=patient.PatNum
					AND phonenumber.PhoneType={SOut.Int((int)PhoneType.Other)}
				{(string.IsNullOrEmpty(regexp)?"":$@"
					AND phonenumber.PhoneNumberVal REGEXP '{regexp}'")}")}
				LEFT JOIN registrationkey ON patient.PatNum=registrationkey.PatNum")
			+(string.IsNullOrEmpty(ptSearchArgs.SubscriberId)?"":$@"
				LEFT JOIN patplan ON patplan.PatNum=patient.PatNum
				LEFT JOIN inssub ON patplan.InsSubNum=inssub.InsSubNum")
			+(string.IsNullOrEmpty(ptSearchArgs.InvoiceNumber)?"":$@"
				LEFT JOIN statement ON statement.PatNum=patient.PatNum AND statement.IsInvoice")+$@"
				WHERE patient.PatStatus NOT IN({SOut.Int((int)PatientStatus.Deleted)}"
				+(!ptSearchArgs.HideInactive?"":$@",{SOut.Int((int)PatientStatus.Inactive)}")
				+(ptSearchArgs.ShowArchived?"":$@",{SOut.Int((int)PatientStatus.Archived)},{SOut.Int((int)PatientStatus.Deceased)}")+")"
				+(ptSearchArgs.ShowMerged?"":$@"
				AND patient.PatNum NOT IN (
					SELECT DISTINCT pl1.PatNumFrom 
					FROM patientlink pl1
					LEFT JOIN patientlink pl2 ON pl2.PatNumTo=pl1.PatNumFrom
						AND	pl2.DateTimeLink > pl1.DateTimeLink
						AND pl2.LinkType={SOut.Int((int)PatientLinkType.Merge)}
					WHERE pl2.PatNumTo IS NULL
					AND pl1.LinkType={SOut.Int((int)PatientLinkType.Merge)}
				) ")
				+(string.IsNullOrEmpty(ptSearchArgs.LName)?"":$@"
				AND (
					patient.LName LIKE '{(ptSearchArgs.DoLimit?"":"%")+ptSearchArgs.LName}%'{(!PrefC.GetBool(PrefName.DistributorKey)?"":$@"
					OR patient.Preferred LIKE '{(ptSearchArgs.DoLimit?"":"%")+ptSearchArgs.LName}%'")}
				) ")
				+(string.IsNullOrEmpty(ptSearchArgs.FName)?"":$@"
				AND (
					patient.FName LIKE '{ptSearchArgs.FName}%'"
					//Nathan has approved the preferred name search for first name only. It is not intended to work with last name for our customers.
					+$@"{((!PrefC.GetBool(PrefName.DistributorKey) && !PrefC.GetBool(PrefName.PatientSelectUseFNameForPreferred))?"":$@"
					OR patient.Preferred LIKE '{ptSearchArgs.FName}%'")}
				) ")
				+(string.IsNullOrEmpty(regexp) || usePhonenumTable?"":$@"
				AND (
					patient.HmPhone REGEXP '{regexp}'
					OR patient.WkPhone REGEXP '{regexp}'
					OR patient.WirelessPhone REGEXP '{regexp}'{(!PrefC.GetBool(PrefName.DistributorKey)?"":$@"
					OR phonenumber.PhoneNumberVal REGEXP '{regexp}'")}
				) ")
				+phoneNumSearch
				+(string.IsNullOrEmpty(strAddress)?"":$@"
				AND (
					patient.Address LIKE '%{strAddress}%'{(!PrefC.IsODHQ?"":$@"
					OR patient.Address2 LIKE '%{strAddress}%'")}
				)")
				+(string.IsNullOrEmpty(ptSearchArgs.City)?"":$@"
				AND patient.City LIKE '{ptSearchArgs.City}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.State)?"":$@"
				AND patient.State LIKE '{ptSearchArgs.State}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.Ssn)?"":$@"
				AND patient.SSN LIKE '{ptSearchArgs.Ssn}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.ChartNumber)?"":$@"
				AND patient.ChartNumber LIKE '{ptSearchArgs.ChartNumber}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.Email)?"":$@"
				AND patient.Email LIKE '%{ptSearchArgs.Email}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.Country)?"":$@"
				AND patient.Country LIKE '%{ptSearchArgs.Country}%'")//LIKE is case insensitive in mysql.
				+(string.IsNullOrEmpty(ptSearchArgs.RegKey)?"":$@"
				AND registrationkey.RegKey LIKE '%{ptSearchArgs.RegKey}%'")//LIKE is case insensitive in mysql.
				+(ptSearchArgs.BillingType==0?"":$@"
				AND patient.BillingType={SOut.Long(ptSearchArgs.BillingType)}")
				+(!ptSearchArgs.GuarOnly?"":$@"
				AND patient.PatNum=patient.Guarantor")
				+(ptSearchArgs.SiteNum==0?"":$@"
				AND patient.SiteNum={ptSearchArgs.SiteNum}")
				+(string.IsNullOrEmpty(ptSearchArgs.SubscriberId)?"":$@"
				AND inssub.SubscriberId LIKE '{ptSearchArgs.SubscriberId}%'")//LIKE is case insensitive in mysql.
				+((ptSearchArgs.Birthdate.Year<1880 || ptSearchArgs.Birthdate.Year>2100)?"":$@"
				AND patient.Birthdate={SOut.Date(ptSearchArgs.Birthdate)}")
				//Only include patients who are assigned to the clinic and also patients who are not assigned to any clinic
				+(string.IsNullOrEmpty(ptSearchArgs.ClinicNums)?"":$@"
				AND (
					patient.ClinicNum IN (0,{ptSearchArgs.ClinicNums})
					OR EXISTS (
						SELECT 1 FROM appointment
						WHERE ClinicNum IN ({ptSearchArgs.ClinicNums})
						AND patient.PatNum=appointment.PatNum
					)
				)")
				//jordan I don't think unassigned patients should be included.  They will usually search by name anyway
				+(string.IsNullOrEmpty(ptSearchArgs.ClinicName)?"":$@"
				AND (
					EXISTS(
						SELECT 1 FROM clinic
						WHERE clinic.Abbr LIKE '{ptSearchArgs.ClinicName}%'
						AND patient.ClinicNum=clinic.ClinicNum
					)
				)")
				//Do a mathematical comparison for the patNumStr.
				+$@"
				{DbHelper.LongBetween("patient.PatNum",ptSearchArgs.PatNumStr)}"
				//Do a mathematical comparison for the invoiceNumber.
				+$@"
				{DbHelper.LongBetween("statement.StatementNum",ptSearchArgs.InvoiceNumber)}"
				//NOTE: This filter will superceed all filters set above.  Negate all filters above and select pats based solely on being in explicitPatNums
				+(ptSearchArgs.ListExplicitPatNums.IsNullOrEmpty()?"":$@"
				AND FALSE
				OR patient.PatNum IN ({string.Join(",",ptSearchArgs.ListExplicitPatNums)})")
				+(!PrefC.GetBool(PrefName.DistributorKey)?"":$@"
				GROUP BY patient.PatNum");
			if(!usePhonenumTable) {
				command+=$@"
				ORDER BY {((ptSearchArgs.InitialPatNum==0 || !ptSearchArgs.DoLimit) ? "" : $@"patient.PatNum={ptSearchArgs.InitialPatNum} DESC,")}"
				+$@"isExactMatch DESC,patient.LName,patient.FName";
				if(ptSearchArgs.DoLimit) {
					command=DbHelper.LimitOrderBy(command,40);
				}
			}
			else if(ptSearchArgs.DoLimit) {
				command+=DbHelper.LimitAnd(41);
			}
			DataTable table=Db.GetTable(command);
			if(usePhonenumTable && useExactMatch && phDigitsTrimmed.Length==exactMatchPhoneDigits && table.Rows.Count==0) {
				command=command.Replace(phoneNumSearch,likeQueryString);
				table=Db.GetTable(command);
			}
			DataRow[] arrayRows=table.Select();
			if(usePhonenumTable) {
				if((ptSearchArgs.InitialPatNum>0 && ptSearchArgs.DoLimit) || table.Rows.Count<41) {
					arrayRows=table.Select().OrderByDescending(x => x["PatNum"].ToString()==ptSearchArgs.InitialPatNum.ToString())
						.ThenByDescending(x => x["isExactMatch"].ToString()=="1")
						.ThenBy(x => x["LName"].ToString())
						.ThenBy(x => x["FName"].ToString()).ToArray();
				}
				else if(ptSearchArgs.DoLimit && arrayRows.Length>40) {
					arrayRows=arrayRows.Take(40).ToArray();
				}
			}
			Dictionary<string,Tuple<DateTime,DateTime>> dictNextLastApts=new Dictionary<string,Tuple<DateTime,DateTime>>();
			if(ptSearchArgs.HasNextLastVisit && ptSearchArgs.DoLimit && table.Rows.Count>0) {
				List<string> listPatNums=table.Select().Select(x => x["PatNum"].ToString()).ToList();
				command=$@"SELECT PatNum,
					COALESCE(MIN(CASE WHEN AptStatus={SOut.Int((int)ApptStatus.Scheduled)} AND AptDateTime>={DbHelper.Now()}
						THEN AptDateTime END),{SOut.DateT(DateTime.MinValue)}) NextVisit,
					COALESCE(MAX(CASE WHEN AptStatus={SOut.Int((int)ApptStatus.Complete)} AND AptDateTime<={DbHelper.Now()}
						THEN AptDateTime END),{SOut.DateT(DateTime.MinValue)}) LastVisit
					FROM appointment 
					WHERE AptStatus IN({SOut.Int((int)ApptStatus.Scheduled)},{SOut.Int((int)ApptStatus.Complete)})
				  AND PatNum IN ({string.Join(",",listPatNums)})
					GROUP BY PatNum";
				dictNextLastApts=Db.GetTable(command).Select()
					.ToDictionary(x => x["PatNum"].ToString(),x => Tuple.Create(SIn.DateT(x["NextVisit"].ToString()),SIn.DateT(x["LastVisit"].ToString())));
			}
			DataTable PtDataTable=table.Clone();//does not copy any data
			PtDataTable.TableName="table";
			PtDataTable.Columns.Add("age");
			PtDataTable.Columns.Add("clinic");
			PtDataTable.Columns.Add("site");
			//lastVisit and nextVisit are not part of PtDataTable and need to be added manually from the corresponding dictionary.
			PtDataTable.Columns.Add("lastVisit");
			PtDataTable.Columns.Add("nextVisit");
			PtDataTable.Columns.OfType<DataColumn>().ForEach(x => x.DataType=typeof(string));
			DataRow r;
			DateTime date;
			foreach(DataRow dRow in arrayRows) {
				r=PtDataTable.NewRow();
				r["PatNum"]=dRow["PatNum"].ToString();
				r["LName"]=dRow["LName"].ToString();
				r["FName"]=dRow["FName"].ToString();
				r["MiddleI"]=dRow["MiddleI"].ToString();
				r["Preferred"]=dRow["Preferred"].ToString();
				date=SIn.Date(dRow["Birthdate"].ToString());
				if(date.Year>1880){
					r["age"]=DateToAge(date);
					r["Birthdate"]=date.ToShortDateString();
				}
				else{
					r["age"]="";
					r["Birthdate"]="";
				}
				r["SSN"]=dRow["SSN"].ToString();
				r["HmPhone"]=dRow["HmPhone"].ToString();
				r["WkPhone"]=dRow["WkPhone"].ToString();
				r["Address"]=dRow["Address"].ToString();
				r["PatStatus"]=((PatientStatus)SIn.Int(dRow["PatStatus"].ToString())).ToString();
				r["BillingType"]=Defs.GetName(DefCat.BillingTypes,SIn.Long(dRow["BillingType"].ToString()));
				r["ChartNumber"]=dRow["ChartNumber"].ToString();
				r["City"]=dRow["City"].ToString();
				r["State"]=dRow["State"].ToString();
				r["PriProv"]=Providers.GetAbbr(SIn.Long(dRow["PriProv"].ToString()));
				r["site"]=Sites.GetDescription(SIn.Long(dRow["SiteNum"].ToString()));
				r["Email"]=dRow["Email"].ToString();
				r["Country"]=dRow["Country"].ToString();
				long clinicNum=PIn.Long(dRow["ClinicNum"].ToString());
				r["ClinicNum"]=clinicNum;
				r["clinic"]=Clinics.GetAbbr(clinicNum);
				if(PrefC.GetBool(PrefName.DistributorKey)) {//if for OD HQ
					r["OtherPhone"]=dRow["OtherPhone"].ToString();
					r["RegKey"]=dRow["RegKey"].ToString();
				}
				r["StatementNum"]=dRow["StatementNum"].ToString();
				r["WirelessPhone"]=dRow["WirelessPhone"].ToString();
				r["SecProv"]=Providers.GetAbbr(SIn.Long(dRow["SecProv"].ToString()));
				r["nextVisit"]="";
				r["lastVisit"]="";
				if(dictNextLastApts.TryGetValue(dRow["PatNum"].ToString(),out Tuple<DateTime,DateTime> tupleNextLastVisitDates)) {
					date=tupleNextLastVisitDates.Item1;
					if(date.Year>1880) {//if the date is valid
						r["nextVisit"]=date.ToShortDateString();
					}
					date=tupleNextLastVisitDates.Item2;
					if(date.Year>1880) {//if the date is valid
						r["lastVisit"]=date.ToShortDateString();
					}
				}
				r["isExactMatch"]=dRow["isExactMatch"].ToString();
				r["Specialty"]=dRow["Specialty"].ToString();
				PtDataTable.Rows.Add(r);
			}
			return PtDataTable;
		}

		///<summary>Returns a query snippet intended to be used within the ORDER BY clause in order to push exact matches towards the top.
		///Returns '0' if there were no search parameters set that the user could type into (e.g. LName, FName, etc).
		///Note: some of the clauses in the snippet are dependent on join clauses of the query constructed in GetPtDataTable().</summary>
		private static string GetExactMatchSnippet(PtTableSearchParams args) {
			//No need to check RemotingRole; private method and no call to db.
			if(DataConnection.DBtype!=DatabaseType.MySql) {//Oracle
				return "'0'";
			}
			List<string> listClauses=new List<string>();
			listClauses.Add(string.IsNullOrEmpty(args.LName) ? "" : "(patient.LName='"+args.LName+"')");
			listClauses.Add(string.IsNullOrEmpty(args.FName) ? "" : "(patient.FName='"+args.FName+"')");
			listClauses.Add(string.IsNullOrEmpty(args.Phone) ? "" : 
				"(patient.WirelessPhone='"+args.Phone+"' OR patient.HmPhone='"+args.Phone+"' OR patient.WkPhone='"+args.Phone+"'"
				+(!PrefC.GetBool(PrefName.DistributorKey)?"": " OR phonenumber.PhoneNumberVal='"+args.Phone+"'") //Join
				+")");
			listClauses.Add(string.IsNullOrEmpty(args.Address) ? "" : "(patient.Address='"+args.Address+"')");
			listClauses.Add(string.IsNullOrEmpty(args.City) ? "" : "(patient.City='"+args.City+"')");
			listClauses.Add(string.IsNullOrEmpty(args.State) ? "" : "(patient.State='"+args.State+"')");
			listClauses.Add(string.IsNullOrEmpty(args.Ssn) ? "" : "(patient.Ssn='"+args.Ssn+"')");
			listClauses.Add(string.IsNullOrEmpty(args.PatNumStr) ? "" : "(patient.PatNum='"+args.PatNumStr+"')");
			listClauses.Add(string.IsNullOrEmpty(args.ChartNumber) ? "" : "(patient.ChartNumber='"+args.ChartNumber+"')");
			listClauses.Add(string.IsNullOrEmpty(args.SubscriberId) ? "" : "(inssub.SubscriberId='"+args.SubscriberId+"')"); //Join
			listClauses.Add(string.IsNullOrEmpty(args.Email) ? "" : "(patient.Email='"+args.Email+"')");
			listClauses.Add(string.IsNullOrEmpty(args.Country) ? "" : "(patient.Country='"+args.Country+"')");
			listClauses.Add((string.IsNullOrEmpty(args.RegKey) || !PrefC.GetBool(PrefName.DistributorKey)) ? "" : "(registrationkey.RegKey='"+args.RegKey+"')"); //Join
			listClauses.Add(args.Birthdate.Year<1880 ? "" : "(patient.Birthdate="+SOut.Date(args.Birthdate)+")");
			listClauses.Add(string.IsNullOrEmpty(args.InvoiceNumber) ? "" : "(statement.StatementNum='"+args.InvoiceNumber+"')");
			listClauses.RemoveAll(string.IsNullOrEmpty);
			if(listClauses.Count>0) {
				return "("+string.Join(" AND ",listClauses)+")";
			}
			return "'0'";
		}

		public static bool HasPatientPortalAccess(long patNum) {
			UserWeb uwCur=UserWebs.GetByFKeyAndType(patNum,UserWebFKeyType.PatientPortal);
			if(uwCur!=null && uwCur.PasswordHash!="") {
				return true;
			}
			return false;
		}

		///<summary>Used when filling appointments for an entire day. Gets a list of Pats, multPats, of all the specified patients.  Then, use GetOnePat to pull one patient from this list.  This process requires only one call to the database.</summary>
		public static Patient[] GetMultPats(List<long> patNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient[]>(MethodBase.GetCurrentMethod(),patNums);
			}
			DataTable table=new DataTable();
			if(patNums.Count>0) {
				string command="SELECT * FROM patient WHERE PatNum IN ("+String.Join<long>(",",patNums)+") ";
				table=Db.GetTable(command);
			}
			Patient[] multPats=Crud.PatientCrud.TableToList(table).ToArray();
			return multPats;
		}

		///<summary>Get all patients who have a corresponding entry in the RegistrationKey table. DO NOT REMOVE! Used by OD WebApps solution.</summary>
		public static List<Patient> GetPatientsWithRegKeys() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM patient WHERE PatNum IN (SELECT PatNum FROM registrationkey)";
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>First call GetMultPats to fill the list of multPats. Then, use this to return one patient from that list.</summary>
		public static Patient GetOnePat(Patient[] multPats,long patNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<multPats.Length;i++){
				if(multPats[i].PatNum==patNum){
					return multPats[i];
				}
			}
			return new Patient();
		}

		/// <summary>Gets the most useful fields from the db for the given patnum.  If invalid PatNum, returns new Patient rather than null.</summary>
		public static Patient GetLim(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod(),patNum);
			}
			if(patNum==0){
				return new Patient();
			}
			string command= 
				"SELECT PatNum,LName,FName,MiddleI,Preferred,CreditType,Guarantor,HasIns,SSN,Birthdate,ClinicNum " 
				+"FROM patient "
				+"WHERE PatNum = '"+patNum.ToString()+"'";
 			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return new Patient();
			}
			Patient Lim=new Patient();
			Lim.PatNum     = PIn.Long  (table.Rows[0][0].ToString());
			Lim.LName      = PIn.String(table.Rows[0][1].ToString());
			Lim.FName      = PIn.String(table.Rows[0][2].ToString());
			Lim.MiddleI    = PIn.String(table.Rows[0][3].ToString());
			Lim.Preferred  = PIn.String(table.Rows[0][4].ToString());
			Lim.CreditType = PIn.String(table.Rows[0][5].ToString());
			Lim.Guarantor  = PIn.Long  (table.Rows[0][6].ToString());
			Lim.HasIns     = PIn.String(table.Rows[0][7].ToString());
			Lim.SSN        = PIn.String(table.Rows[0][8].ToString());
			Lim.Birthdate  = PIn.DateT (table.Rows[0][9].ToString());
			Lim.ClinicNum  = PIn.Long  (table.Rows[0][10].ToString());
			return Lim;
		}

		///<summary></summary>
		public static SerializableDictionary<long,string> GetStatesForPats(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,string>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			SerializableDictionary<long,string> retVal=new SerializableDictionary<long,string>();
			if(listPatNums==null || listPatNums.Count < 1) {
				return new SerializableDictionary<long,string>();
			}
			string command= "SELECT PatNum,State " 
				+"FROM patient "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums)+")";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				Patient patLim=new Patient();
				patLim.PatNum     = PIn.Long	(table.Rows[i]["PatNum"].ToString());
				patLim.State      = PIn.String(table.Rows[i]["State"].ToString());
				retVal.Add(patLim.PatNum,patLim.State);
			}
			return retVal;
		}

		///<summary>Gets only PatNum, FName, LName, Birthdate, and PatStatus for use in 834 matching to reduce memory consumption compared to getting the complete patient table.</summary>
		public static List<PatientFor834Import> GetAllPatsFor834Imports() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientFor834Import>>(MethodBase.GetCurrentMethod());
			}
			List<PatientFor834Import> retVal=new List<PatientFor834Import>();
			string command= "SELECT PatNum,LName,FName,BirthDate,PatStatus FROM patient";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				PatientFor834Import patLim=new PatientFor834Import();
				patLim.PatNum     = PIn.Long(row["PatNum"].ToString());
				patLim.LName      = PIn.String(row["LName"].ToString());
				patLim.FName      = PIn.String(row["FName"].ToString());
				patLim.Birthdate  = PIn.Date(row["Birthdate"].ToString());
				patLim.PatStatus  = PIn.Enum<PatientStatus>(row["PatStatus"].ToString());
				retVal.Add(patLim);
			}
			return retVal;
		}

		///<summary>Gets nine of the most useful fields from the db for the given PatNums, with option to include patient.ClinicNum.</summary>
		public static List<Patient> GetLimForPats(List<long> listPatNums,bool doIncludeClinicNum=false) {
			if(listPatNums==null || listPatNums.Count < 1) {
				return new List<Patient>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),listPatNums,doIncludeClinicNum);
			}
			List<Patient> retVal=new List<Patient>();
			string command= "SELECT PatNum,LName,FName,MiddleI,Preferred,CreditType,Guarantor,HasIns,SSN"+(doIncludeClinicNum?",ClinicNum":"")+" "
				+"FROM patient "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums)+")";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				Patient patLim=new Patient();
				patLim.PatNum     = PIn.Long	(row["PatNum"].ToString());
				patLim.LName      = PIn.String(row["LName"].ToString());
				patLim.FName      = PIn.String(row["FName"].ToString());
				patLim.MiddleI    = PIn.String(row["MiddleI"].ToString());
				patLim.Preferred  = PIn.String(row["Preferred"].ToString());
				patLim.CreditType = PIn.String(row["CreditType"].ToString());
				patLim.Guarantor  = PIn.Long	(row["Guarantor"].ToString());
				patLim.HasIns     = PIn.String(row["HasIns"].ToString());
				patLim.SSN        = PIn.String(row["SSN"].ToString());
				if(doIncludeClinicNum) {
					patLim.ClinicNum= PIn.Long  (row["ClinicNum"].ToString());
				}
				retVal.Add(patLim);
			}
			return retVal;
		}
		
		///<summary>Gets the patient and provider balances for all patients in the family.  Used from the payment window to help visualize and automate the family splits.</summary>
		public static DataTable GetPaymentStartingBalances(long guarNum,long excludePayNum) {
			return GetPaymentStartingBalances(guarNum,excludePayNum,false);
		}

		///<summary>Gets the patient and provider balances for all patients in the family.  Used from the payment window to help visualize and automate the family splits. groupByProv means group by provider only not provider/clinic.</summary>
		public static DataTable GetPaymentStartingBalances(long guarNum,long excludePayNum,bool groupByProv) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),guarNum,excludePayNum,groupByProv);
			}
			//This method no longer uses a temporary table due to the problems it was causing replication users.
			//The in-memory table name was left as "tempfambal" for nostalgic purposes because veteran engineers know exactly where to look when "tempfambal" is mentioned.
			//This query will be using UNION ALLs so that duplicate-row removal does not occur. 
			string command=@"
					SELECT tempfambal.PatNum,tempfambal.ProvNum,
						tempfambal.ClinicNum,ROUND(SUM(tempfambal.AmtBal),3) StartBal,
						ROUND(SUM(tempfambal.AmtBal-tempfambal.InsEst),3) AfterIns,patient.FName,patient.Preferred,0.0 EndBal,
						CASE WHEN patient.Guarantor!=patient.PatNum THEN 1 ELSE 0 END IsNotGuar,patient.Birthdate,tempfambal.UnearnedType
					FROM(
						/*Completed procedures*/
						(SELECT patient.PatNum,procedurelog.ProvNum,procedurelog.ClinicNum,
						SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) AmtBal,0 InsEst,0 UnearnedType
						FROM procedurelog,patient
						WHERE patient.PatNum=procedurelog.PatNum
						AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@"
						AND patient.Guarantor="+POut.Long(guarNum)+@"
						GROUP BY patient.PatNum,procedurelog.ProvNum,procedurelog.ClinicNum)
					UNION ALL			
						/*Received insurance payments*/
						(SELECT patient.PatNum,claimproc.ProvNum,claimproc.ClinicNum,-SUM(claimproc.InsPayAmt)-SUM(claimproc.Writeoff) AmtBal,0 InsEst,0 UnearnedType
						FROM claimproc,patient
						WHERE patient.PatNum=claimproc.PatNum
						AND (claimproc.Status="+POut.Int((int)ClaimProcStatus.Received)+@" 
							OR claimproc.Status="+POut.Int((int)ClaimProcStatus.Supplemental)+@" 
							OR claimproc.Status="+POut.Int((int)ClaimProcStatus.CapClaim)+@" 
							OR claimproc.Status="+POut.Int((int)ClaimProcStatus.CapComplete)+@")
						AND patient.Guarantor="+POut.Long(guarNum)+@"
						AND claimproc.PayPlanNum = 0
						GROUP BY patient.PatNum,claimproc.ProvNum,claimproc.ClinicNum)
					UNION ALL
						/*Insurance estimates*/
						(SELECT patient.PatNum,claimproc.ProvNum,claimproc.ClinicNum,0 AmtBal,SUM(claimproc.InsPayEst)+SUM(claimproc.Writeoff) InsEst,0 UnearnedType
						FROM claimproc,patient
						WHERE patient.PatNum=claimproc.PatNum
						AND claimproc.Status="+POut.Int((int)ClaimProcStatus.NotReceived)+@"
						AND patient.Guarantor="+POut.Long(guarNum)+@"
						GROUP BY patient.PatNum,claimproc.ProvNum,claimproc.ClinicNum)
					UNION ALL
						/*Adjustments*/
						(SELECT patient.PatNum,adjustment.ProvNum,adjustment.ClinicNum,SUM(adjustment.AdjAmt) AmtBal,0 InsEst,0 UnearnedType
						FROM adjustment,patient
						WHERE patient.PatNum=adjustment.PatNum
						AND patient.Guarantor="+POut.Long(guarNum)+@"
						GROUP BY patient.PatNum,adjustment.ProvNum,adjustment.ClinicNum)
					UNION ALL
						/*Patient payments*/
						(SELECT patient.PatNum,paysplit.ProvNum,paysplit.ClinicNum,-SUM(SplitAmt) AmtBal,0 InsEst,paysplit.UnearnedType
						FROM paysplit,patient
						WHERE patient.PatNum=paysplit.PatNum
						AND paysplit.PayNum!="+POut.Long(excludePayNum)+@"
						AND patient.Guarantor="+POut.Long(guarNum);
			if(PrefC.GetInt(PrefName.PayPlansVersion)==1) { //for payplans v1, exclude paysplits attached to payplans
				command+=@"
						AND paysplit.PayPlanNum=0 ";
			}
			command+=@"
						GROUP BY patient.PatNum,paysplit.ProvNum,paysplit.ClinicNum)
					UNION ALL	
						(SELECT patient.PatNum,payplancharge.ProvNum,payplancharge.ClinicNum,-payplan.CompletedAmt ";
			if(PrefC.GetInt(PrefName.PayPlansVersion)==2) {
				command+="+ SUM(CASE WHEN payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+@"
						AND payplancharge.ChargeDate <= CURDATE() THEN payplancharge.Principal + payplancharge.Interest ELSE 0 END) ";
			}
			command+=@"AmtBal,0 InsEst,0 UnearnedType
						FROM payplancharge
						INNER JOIN payplan ON payplan.PayPlanNum=payplancharge.PayPlanNum
						INNER JOIN patient ON patient.PatNum=payplancharge.PatNum AND patient.Guarantor="+POut.Long(guarNum)+@"
						GROUP BY payplan.PayPlanNum,payplan.CompletedAmt,patient.PatNum,payplancharge.ProvNum,payplancharge.ClinicNum)
					) tempfambal,patient
					WHERE tempfambal.PatNum=patient.PatNum 
					GROUP BY tempfambal.PatNum,tempfambal.ProvNum,";
			if(!groupByProv) {
				command+=@"tempfambal.ClinicNum,";
			}
			command+=@"patient.FName,patient.Preferred";
			//Probably an unnecessary MySQL / Oracle split but I didn't want to affect the old GROUP BY functionality for MySQL just be Oracle is lame.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+=@"
					HAVING ((StartBal>0.005 OR StartBal<-0.005) OR (AfterIns>0.005 OR AfterIns<-0.005))
					ORDER BY IsNotGuar,Birthdate,ProvNum,FName,Preferred";
			}
			else {//Oracle.
				command+=@",(CASE WHEN Guarantor!=patient.PatNum THEN 1 ELSE 0 END),Birthdate
					HAVING ((SUM(AmtBal)>0.005 OR SUM(AmtBal)<-0.005) OR (SUM(AmtBal-tempfambal.InsEst)>0.005 OR SUM(AmtBal-tempfambal.InsEst)<-0.005))
					ORDER BY IsNotGuar,patient.Birthdate,tempfambal.ProvNum,patient.FName,patient.Preferred";
			}
			return Db.GetTable(command);
		}

		///<summary></summary>
		public static void ChangeGuarantorToCur(Family Fam,Patient patientOld){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Fam,patientOld);
				return;
			}
			//Move famfinurgnote to current patient:
			Patient patient=patientOld.Copy();
			patient.FamFinUrgNote=Fam.ListPats[0].FamFinUrgNote;
			Patients.Update(patient,patientOld); //FamFinUrgNote is used in SecurityHash, Update will handle rehashing
			//Move family financial note to current patient:
			string command="SELECT FamFinancial FROM patientnote "
				+"WHERE PatNum = "+POut.Long(patient.Guarantor);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==1){
				command= "UPDATE patientnote SET "
					+"FamFinancial = '"+POut.String(table.Rows[0][0].ToString())+"' "
					+"WHERE PatNum = "+POut.Long(patient.PatNum);
				Db.NonQ(command);
			}
			command= "UPDATE patientnote SET FamFinancial = '' "
				+"WHERE PatNum = "+POut.Long(patient.Guarantor);
			Db.NonQ(command);
			//change guarantor of all family members:
			List<Patient> listPatients=Patients.GetAllPatientsForGuarantor(patientOld.Guarantor);
			for(int i=0;i<listPatients.Count;i++) {
				patient=listPatients[i].Copy();
				listPatients[i].Guarantor=patientOld.PatNum;
				Patients.Update(listPatients[i],patient);//Guarantor is used in SecurityHash, Update will handle rehashing
			}
		}
		
		///<summary></summary>
		public static void CombineGuarantors(Family Fam,Patient Pat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Fam,Pat);
				return;
			}
			string command="";
			//concat cur notes with guarantor notes. Requires rehashing.
			List<Patient> listPatients=GetAllPatientsForGuarantor(Pat.Guarantor);
			for(int i=0;i<listPatients.Count;i++) {
				Patient patientNew=listPatients[i].Copy();
				patientNew.FamFinUrgNote=Fam.ListPats[0].FamFinUrgNote+Pat.FamFinUrgNote;
				Update(patientNew,listPatients[i]);
			}
			//delete cur notes. Requires rehashing.
			Patient patientOld=Pat.Copy();
			Pat.FamFinUrgNote="";
			Update(Pat,patientOld);
			//concat family financial notes
			PatientNote PatientNoteCur=PatientNotes.Refresh(Pat.PatNum,Pat.Guarantor);
			//patientnote table must have been refreshed for this to work.
			//Makes sure there are entries for patient and for guarantor.
			//Also, PatientNotes.cur.FamFinancial will now have the guar info in it.
			string strGuar=PatientNoteCur.FamFinancial;
			command= 
				"SELECT famfinancial "
				+"FROM patientnote WHERE patnum ='"+POut.Long(Pat.PatNum)+"'";
			//MessageBox.Show(string command);
			DataTable table=Db.GetTable(command);
			string strCur=PIn.String(table.Rows[0][0].ToString());
			command= 
				"UPDATE patientnote SET "
				+"famfinancial = '"+POut.String(strGuar+strCur)+"' "
				+"WHERE patnum = '"+Pat.Guarantor.ToString()+"'";
			Db.NonQ(command);
			//delete cur financial notes
			command= 
				"UPDATE patientnote SET "
				+"famfinancial = ''"
				+"WHERE patnum = '"+Pat.PatNum.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>Key=patNum, value=formatted name. Stop using this.  Switch to GetPatientNameList(). Even better: Just use GetLimForPats, and then use LINQ on that list as needed.</summary>
		[Obsolete()]
		public static Dictionary<long,string> GetPatientNames(List<long> listPatNums) {
			return Patients.GetLimForPats(listPatNums)
				.ToDictionary(x => x.PatNum,x => x.GetNameLF());
		}

		///<summary>Used for some reference lists.</summary>
		public class PatientName {
			public long PatNum;
			public string Name;
		}

		///<summary>Used to get a list of patient names with their associated PatNum. Returns list of PatientName objects</summary>
		public static List<PatientName> GetPatientNameList(List<long> listPatNums) {
			List<Patient> listPatients = Patients.GetLimForPats(listPatNums);
			List<PatientName> listPatientNames = new List<PatientName>();
			for(int i = 0;i<listPatients.Count;++i) {
				PatientName patientName = new PatientName();
				patientName.PatNum=listPatients[i].PatNum;
				patientName.Name=listPatients[i].GetNameLF();
				listPatientNames.Add(patientName);
			}
			return listPatientNames;
		}

		///<summary>DEPRECATED. This method should not be used because it will take a long time on large databases. 
		///Call Patients.GetPatientNames() instead.
		///Key=patNum, value=formatted name.</summary>
		[Obsolete("Call Patients.GetPatientNames(List<long>) instead.")]
		public static Dictionary<long,string> GetAllPatientNames() {
			//No need to check RemotingRole; no call to db.
			DataTable table=GetAllPatientNamesTable();
			Dictionary<long,string> dict=new Dictionary<long,string>();
			long patnum;
			string lname,fname,middlei,preferred;
			for(int i=0;i<table.Rows.Count;i++) {
				patnum=PIn.Long(table.Rows[i][0].ToString());
				lname=PIn.String(table.Rows[i][1].ToString());
				fname=PIn.String(table.Rows[i][2].ToString());
				middlei=PIn.String(table.Rows[i][3].ToString());
				preferred=PIn.String(table.Rows[i][4].ToString());
				if(preferred=="") {
					dict.Add(patnum,lname+", "+fname+" "+middlei);
				}
				else {
					dict.Add(patnum,lname+", '"+preferred+"' "+fname+" "+middlei);
				}
			}
			return dict;
		}

		public static DataTable GetAllPatientNamesTable() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SELECT patnum,lname,fname,middlei,preferred "
				+"FROM patient";
			DataTable table=Db.GetTable(command);
			return table;
		}

		///<summary>Useful when you expect to individually examine most of the patients in the database during a data import.  Excludes deleted patients.
		///Saves time and database calls to call this method once and keep a short term cache than it is to run a series of select statements.</summary>
		public static List<Patient> GetAllPatients() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM patient WHERE PatStatus != "+POut.Int((int)PatientStatus.Deleted);
			return Crud.PatientCrud.SelectMany(command);
		}

		/// <summary>Determines if all Patients in the Superfamily have ths same HmPhone, Address, Address2, City, State, Country, and Zip.</summary>
		/// <param name="pat">A Patient in the Superfamily.</param>
		/// <param name="isArchivedIncluded">Includes Archived Patients if true, excludes Archived Patients from logic if false.</param>
		/// <returns></returns>
		public static bool SuperFamHasSameAddrPhone(Patient pat,bool isArchivedIncluded) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),pat,isArchivedIncluded);
			}
			string command="SELECT COUNT(*) FROM patient WHERE SuperFamily="+POut.Long(pat.SuperFamily)+" "
				+"AND (HmPhone!='"+POut.String(pat.HmPhone.ToString())+"' "
							+"OR Address!='"+POut.String(pat.Address.ToString())+"' "
							+"OR Address2!='"+POut.String(pat.Address2.ToString())+"' "
							+"OR City!='"+POut.String(pat.City.ToString())+"' "
							+"OR State!='"+POut.String(pat.State.ToString())+"' "
							+"OR Country!='"+POut.String(pat.Country.ToString())+"' "
							+"OR Zip!='"+POut.String(pat.Zip.ToString())+"') ";
			if(!isArchivedIncluded) {
				command+="AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			if(PIn.Int(Db.GetCount(command))==0) {//Everybody in the superfamily has the same information
				return true;
			}
			return false;//At least one patient in the superfamily has different information
		}

		///<summary>Updates all address information for patients within a family or super family to the address information of the patient passed in.</summary>
		///<param name="pat">The patient whose information will be synced to others within the family or super family.</param>
		///<param name="isSuperFam">Indicates whether the address information should be synced to the family(patient.guarantor) or to the 
		///super family(patient.SuperFamily). True indicates to sync the information to the super family.</param>
		///<param name="isAuthArchivedEdit">Indicates whether Archived patients in the family/superfamily should be synced.</param>
		public static void UpdateAddressForFam(Patient pat,bool isSuperFam,bool isAuthArchivedEdit) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,isSuperFam,isAuthArchivedEdit);
				return;
			}
			string strWhere="";
			if(isSuperFam) {
				strWhere+=" WHERE SuperFamily = "+POut.Long(pat.SuperFamily);
			}
			else {
				strWhere+=" WHERE Guarantor = "+POut.Long(pat.Guarantor);
			}
			if(!isAuthArchivedEdit) {
				strWhere+=" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			//Get the list of patients before the changes.
			string strSelect="SELECT * FROM patient "+strWhere;
			List<Patient> listPatsOld=Crud.PatientCrud.SelectMany(strSelect);
			string command="UPDATE patient SET "
				+"Address = '"    +POut.String(pat.Address)+"'"
				+",Address2 = '"   +POut.String(pat.Address2)+"'"
				+",City = '"       +POut.String(pat.City)+"'"
				+",State = '"      +POut.String(pat.State)+"'"
				+",Country = '"    +POut.String(pat.Country)+"'"
				+",Zip = '"        +POut.String(pat.Zip)+"'"
				+",HmPhone = '"    +POut.String(pat.HmPhone)+"'"
				+strWhere;
			Db.NonQ(command);
			//Get the list of patients after the changes
			List<Patient> listPatsNew=Crud.PatientCrud.SelectMany(strSelect);
			//Add securitylog entries for any changes made.
			bool didPhoneChange=false;
			foreach(Patient patOld in listPatsOld) {
				Patient patNew=listPatsNew.FirstOrDefault(x => x.PatNum==patOld.PatNum);
				if(patNew==null) {
					continue;//This shouldn't happen.
				}
				didPhoneChange|=patOld.HmPhone!=patNew.HmPhone;
				InsertAddressChangeSecurityLogEntry(patOld,patNew);
			}
			if(didPhoneChange && PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)) {
				PhoneNumbers.SyncPats(listPatsNew);
			}
		}

		///<summary>Adds a securitylog entry if any of the patient's information (patient name, status, or address) is changed.</summary>
		public static void InsertAddressChangeSecurityLogEntry(Patient patOld,Patient patCur) {
			//No need to check RemotingRole; no call to db.
			StringBuilder secLogText=new StringBuilder();
			secLogText.Append(SecurityLogEntryHelper(patOld.PatStatus.GetDescription(),patCur.PatStatus.GetDescription(),"status"));
			secLogText.Append(SecurityLogEntryHelper(patOld.LName,patCur.LName,"last name"));
			secLogText.Append(SecurityLogEntryHelper(patOld.FName,patCur.FName,"first name"));
			secLogText.Append(SecurityLogEntryHelper(patOld.WkPhone,patCur.WkPhone,"work phone"));
			secLogText.Append(SecurityLogEntryHelper(patOld.WirelessPhone,patCur.WirelessPhone,"wireless phone"));
			secLogText.Append(SecurityLogEntryHelper(patOld.HmPhone,patCur.HmPhone,"home phone"));
			secLogText.Append(SecurityLogEntryHelper(patOld.Address,patCur.Address,"address"));
			secLogText.Append(SecurityLogEntryHelper(patOld.Address2,patCur.Address2,"address 2"));
			secLogText.Append(SecurityLogEntryHelper(patOld.City,patCur.City,"city"));
			secLogText.Append(SecurityLogEntryHelper(patOld.State,patCur.State,"state"));
			secLogText.Append(SecurityLogEntryHelper(patOld.Country,patCur.Country,"country"));
			secLogText.Append(SecurityLogEntryHelper(patOld.Zip,patCur.Zip,"zip code"));
			secLogText.Append(SecurityLogEntryHelper(patOld.TxtMsgOk.ToString(),patCur.TxtMsgOk.ToString(),"Text OK"));
			secLogText.Append(SecurityLogEntryHelper(patOld.Email,patCur.Email,"Email"));
			if(secLogText.ToString()!="") {
				SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patCur.PatNum,secLogText.ToString());
			}
		}

		///<summary>Adds a PatientBillingEdit securitylog entry if the patient's billing type is changed.</summary>
		public static void InsertBillTypeChangeSecurityLogEntry(Patient patOld,Patient patCur) {
			//No need to check RemotingRole; no call to db.
			string strLog=SecurityLogEntryHelper(Defs.GetName(DefCat.BillingTypes,patOld.BillingType),Defs.GetName(DefCat.BillingTypes,patCur.BillingType),
				"billing type");
			if(string.IsNullOrEmpty(strLog)) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,patCur.PatNum,strLog);
		}

		///<summary>Adds a PatPriProvEdit securitylog entry if the patient's primary provider is changed.</summary>
		public static void InsertPrimaryProviderChangeSecurityLogEntry(Patient patOld,Patient patCur) {
			//No need to check RemotingRole; no call to db.
			string strLog=SecurityLogEntryHelper(patOld.PriProv==0?"'blank'":Providers.GetLongDesc(patOld.PriProv),
				patCur.PriProv==0?"'blank'":Providers.GetLongDesc(patCur.PriProv),
				"Primary Provider");
			if(string.IsNullOrEmpty(strLog)) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,patCur.PatNum,strLog);
		}

		///<summary>Returns a line that can be used in a security log entry if the entries are changed.</summary>
		private static string SecurityLogEntryHelper(string oldVal,string newVal,string textInLog) {
			//No need to check RemotingRole; private method.
			if(oldVal!=newVal) {
				return "Patient "+textInLog+" changed from '"+oldVal+"' to '"+newVal+"'\r\n";
			}
			return "";
		}

		public static void UpdateBillingProviderForFam(Patient pat,bool isAuthPriProvEdit,bool isAuthArchivedEdit) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,isAuthPriProvEdit,isAuthArchivedEdit);
				return;
			}
			string strWhere=" WHERE Guarantor = "+POut.Long(pat.Guarantor);
			//Get the list of patients before the changes.
			string strSelect="SELECT * FROM patient "+strWhere;
			List<Patient> listPatsOld=Crud.PatientCrud.SelectMany(strSelect);
			string command="UPDATE patient SET "
				+"credittype      = '"+POut.String(pat.CreditType)+"',";
			if(isAuthPriProvEdit) {
				command+="priprov = "+POut.Long(pat.PriProv)+",";
			}
			command+=
				 "secprov         = "+POut.Long(pat.SecProv)+","
				+"feesched        = "+POut.Long(pat.FeeSched)+","
				+"billingtype     = "+POut.Long(pat.BillingType)+" "
				+strWhere;
			if(!isAuthArchivedEdit) {
				command+=" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			Db.NonQ(command);
			//Get the list of patients after the changes
			List<Patient> listPatsNew=Crud.PatientCrud.SelectMany(strSelect);
			foreach(Patient patOld in listPatsOld) {
				Patient patNew=listPatsNew.FirstOrDefault(x => x.PatNum==patOld.PatNum);
				if(patNew==null) {
					continue;//This shouldn't happen.
				}
				InsertBillTypeChangeSecurityLogEntry(patOld,patNew);
				InsertPrimaryProviderChangeSecurityLogEntry(patOld,patNew);
			}
		}

		///<summary>Used in patient terminal, aka sheet import.  Synchs less fields than the normal synch.</summary>
		public static void UpdateAddressForFamTerminal(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat);
				return;
			}
			string command= "UPDATE patient SET " 
				+"Address = '"    +POut.String(pat.Address)+"'"
				+",Address2 = '"   +POut.String(pat.Address2)+"'"
				+",City = '"       +POut.String(pat.City)+"'"
				+",State = '"      +POut.String(pat.State)+"'"
				+",Zip = '"        +POut.String(pat.Zip)+"'"
				+",HmPhone = '"    +POut.String(pat.HmPhone)+"'"
				+" WHERE guarantor = '"+POut.Long(pat.Guarantor)+"'";
			Db.NonQ(command);
			if(PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)) {
				PhoneNumbers.SyncPats(GetFamily(pat.PatNum).ListPats.ToList());
			}
		}

		///<summary>Updates the 'AskToArriveEarly' field for all members of this patient's family.</summary>
		///<param name="isAuthArchivedEdit">Indicates whether Archived patients in the family should be synced.</param>
		public static void UpdateArriveEarlyForFam(Patient pat,bool isAuthArchivedEdit){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,isAuthArchivedEdit);
				return;
			}
			string command= "UPDATE patient SET " 
				+"AskToArriveEarly = '"   +POut.Int(pat.AskToArriveEarly)+"'"
				+" WHERE guarantor = '"+POut.Long(pat.Guarantor)+"'";
			if(!isAuthArchivedEdit) {
				command+=" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			DataTable table=Db.GetTable(command);
		}

		///<summary></summary>
		public static void UpdateNotesForFam(Patient pat,bool isAuthArchivedEdit){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,isAuthArchivedEdit);
				return;
			}
			string command= "UPDATE patient SET " 
				+"addrnote = '"   +POut.String(pat.AddrNote)+"'"
				+" WHERE guarantor = '"+POut.Long(pat.Guarantor)+"'";
			if(!isAuthArchivedEdit) {
				command+=" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			Db.NonQ(command);
		}

		///<summary>Updates every family members' Email, WirelessPhone, WkPhone, and TxtMsgOk to the passed in patient object.</summary>
		public static void UpdateEmailPhoneForFam(Patient pat,bool isAuthArchivedEdit) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,isAuthArchivedEdit);
				return;
			}
			string strWhere=" WHERE Guarantor = "+POut.Long(pat.Guarantor);
			//Get the list of patients before the changes.
			string strSelect="SELECT * FROM patient "+strWhere;
			List<Patient> listPatsOld=Crud.PatientCrud.SelectMany(strSelect);
			string command= "UPDATE patient SET "
				+"Email='"+POut.String(pat.Email)+"'"
				+",WirelessPhone='"+POut.String(pat.WirelessPhone)+"'"
				+",WkPhone='"+POut.String(pat.WkPhone)+"'"
				+",TxtMsgOk='"+POut.Int((int)pat.TxtMsgOk)+"'"
				+strWhere;
			if(!isAuthArchivedEdit) {
				command+=" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Archived);
			}
			Db.NonQ(command);
			if(PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable)) {
				PhoneNumbers.SyncPats(GetFamily(pat.PatNum).ListPats.ToList());
			}
			//Get the list of patients after the changes
			List<Patient> listPatsNew=Crud.PatientCrud.SelectMany(strSelect);
			foreach(Patient patOld in listPatsOld) {
				Patient patNew=listPatsNew.FirstOrDefault(x => x.PatNum==patOld.PatNum);
				if(patNew==null) {
					continue;//This shouldn't happen.
				}
				InsertAddressChangeSecurityLogEntry(patOld,patNew);
			}
		}

		///<summary>Does not udpate the patient in the database. Takes the new patient data and old patient data. If the new patient status  is 
		///archived, deceased, inactive, nonpatient, or prospective, disable all recalls for this patient. If the new patient status is patient
		///and the old patient status is different, re-active any previously disabled recalls attached to the patient.</summary>
		public static void UpdateRecalls(Patient patNew, Patient patOld,string sender) {
			//if patient is inactive, deceased, etc., then disable any recalls
			if(patNew.PatStatus==PatientStatus.Archived
				|| patNew.PatStatus==PatientStatus.Deceased
				|| patNew.PatStatus==PatientStatus.Inactive
				|| patNew.PatStatus==PatientStatus.NonPatient
				|| patNew.PatStatus==PatientStatus.Prospective)
			{
				List<Recall> recalls=Recalls.GetList(patNew.PatNum);
				for(int i=0;i<recalls.Count;i++){
					if(!recalls[i].IsDisabled || recalls[i].DateDue.Year > 1880) {
						recalls[i].IsDisabled=true;
						recalls[i].DateDue=DateTime.MinValue;
						Recalls.Update(recalls[i]);
						SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recalls[i].PatNum,"Recall disabled from the "+sender+".");
					}
				}
			}
			//if patient was re-activated, then re-enable any recalls
			else if(patNew.PatStatus!=patOld.PatStatus && patNew.PatStatus==PatientStatus.Patient) {//if changed patstatus, and new status is Patient
				List<Recall> recalls=Recalls.GetList(patNew.PatNum);
				for(int i=0;i<recalls.Count;i++) {
					if(recalls[i].IsDisabled) {
						recalls[i].IsDisabled=false;
						Recalls.Update(recalls[i]);
						SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recalls[i].PatNum,"Recall re-enabled from the "+sender+".");
					}
				}
				Recalls.Synch(patNew.PatNum);
			}
		}

		///<summary>This is used in the Billing dialog and with Finance/Billing Charges.</summary>
		public static List<PatAging> GetAgingList(string age,DateTime lastStatement,List<long> billingNums,bool excludeAddr,bool excludeNeg,
			double excludeLessThan,bool excludeInactive,bool ignoreInPerson,List<long> clinicNums,bool isSuperStatements,bool isSinglePatient,
			List<long> listPendingInsPatNums,List<long> listUnsentPatNums,SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions,
			bool excludeNoTil=false,bool excludeNotBilledSince=false,bool isFinanceBilling=false,List<long> listInsSubNums=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatAging>>(MethodBase.GetCurrentMethod(),age,lastStatement,billingNums,excludeAddr,excludeNeg,excludeLessThan,
					excludeInactive,ignoreInPerson,clinicNums,isSuperStatements,isSinglePatient,listPendingInsPatNums,listUnsentPatNums,dictPatAgingTransactions,
					excludeNoTil,excludeNotBilledSince,isFinanceBilling,listInsSubNums);
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				//We are going to purposefully throw an exception so that users will call in and complain.
				throw new ApplicationException(Lans.g("Patients","Aging not currently supported by Oracle.  Please call us for support."));
			}
			List <int> listPatStatusExclude=new List<int>();
			listPatStatusExclude.Add((int)PatientStatus.Deleted);//Always hide deleted.
			if(excludeInactive){
				listPatStatusExclude.Add((int)PatientStatus.Inactive);
			}
			string guarOrPat="";
			if(isSinglePatient) {
				guarOrPat="pat";
			}
			else {
				guarOrPat="guar";
			}
			List<string> listWhereAnds=new List<string>();
			string strMinusIns="";
			if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
				strMinusIns="-guar.InsEst";
			}
			string strBalExclude="(ROUND(guar.BalTotal"+strMinusIns+",3) >= ROUND("+POut.Double(excludeLessThan)+",3) OR guar.PayPlanDue > 0";
			if(!excludeNeg) {//include credits
				strBalExclude+=" OR ROUND(guar.BalTotal"+strMinusIns+",3) < 0";
			}
			strBalExclude+=")";
			listWhereAnds.Add(strBalExclude);
			if(!isFinanceBilling) {
				switch(age) {//age 0 means include all
					case "30":
					listWhereAnds.Add("(guar.Bal_31_60>0 OR guar.Bal_61_90>0 OR guar.BalOver90>0 OR guar.PayPlanDue>0)");
					break;
					case "60":
					listWhereAnds.Add("(guar.Bal_61_90>0 OR guar.BalOver90>0 OR guar.PayPlanDue>0)");
					break;
					case "90":
					listWhereAnds.Add("(guar.BalOver90>0 OR guar.PayPlanDue>0)");
					break;
				}
			}
			else {
				listWhereAnds.Add("(guar.Bal_0_30 + guar.Bal_31_60 + guar.Bal_61_90 + guar.BalOver90 - guar.InsEst > '0.005')");
			}
			if(billingNums.Count>0) {//if billingNums.Count==0, then we'll include all billing types
				listWhereAnds.Add("guar.BillingType IN ("+string.Join(",",billingNums.Select(x => POut.Long(x)))+")");
			}
			if(excludeAddr){
				listWhereAnds.Add("guar.Zip!=''");
			}
			if(excludeNoTil) {
				listWhereAnds.Add("guar.HasSignedTil");
			}
			if(clinicNums.Count>0) {
				listWhereAnds.Add("guar.ClinicNum IN ("+string.Join(",",clinicNums.Select(x => POut.Long(x)))+")");
			}
			listWhereAnds.Add("(guar.PatStatus!="+POut.Int((int)PatientStatus.Archived)+" OR ROUND(guar.BalTotal,3) != 0)");//Hide archived patients with PatBal=0.
			if(!listInsSubNums.IsNullOrEmpty()) {
				listWhereAnds.Add("(inssub.Subscriber IS NULL OR inssub.InsSubNum NOT IN ("+string.Join(",",listInsSubNums)+"))");
			}
			string command="";
			command="SELECT "+guarOrPat+".PatNum,"+guarOrPat+".FName,"+guarOrPat+".MiddleI,"+guarOrPat+".Preferred,"+guarOrPat+".LName,"+guarOrPat+".ClinicNum,guar.SuperFamily,"
				+"guar.HasSuperBilling,guar.BillingType,"
				+"guar.Bal_0_30,guar.Bal_31_60,guar.Bal_61_90,guar.BalOver90,guar.BalTotal,guar.InsEst,guar.PayPlanDue,"
				+"COALESCE(MAX(statement.DateSent),'0001-01-01') AS lastStatement,guar.PriProv,guar.Zip,guar.PatStatus,guar.HasSignedTil "
				+"FROM patient guar "
				+"INNER JOIN patient pat ON guar.PatNum=pat.Guarantor AND pat.PatStatus NOT IN ("+string.Join(",",listPatStatusExclude)+") "
				+"LEFT JOIN statement ON "+guarOrPat+".PatNum=statement.PatNum "
					+(ignoreInPerson?("AND statement.Mode_!="+POut.Int((int)StatementMode.InPerson)+" "):"");
				if(!listInsSubNums.IsNullOrEmpty()) {
				 command+="LEFT JOIN inssub ON pat.PatNum=inssub.Subscriber ";
				}
				command+="WHERE "+string.Join(" AND ",listWhereAnds)+" "
				+"GROUP BY "+guarOrPat+".PatNum "
				+"ORDER BY "+guarOrPat+".LName,"+guarOrPat+".FName ";
			DataTable table=Db.GetTable(command);
			List<PatAging> agingList=new List<PatAging>();
			if(table.Rows.Count<1) {
				return agingList;
			}
			List<DataRow> listDataRowsFromTable=table.Select().ToList();
			if(isSuperStatements) {
				List<long> listSuperNums=listDataRowsFromTable //get all of the super heads that have members with balances.
					.FindAll(x => x["HasSuperBilling"].ToString()=="1" && x["SuperFamily"].ToString()!="0")
					.Select(x => PIn.Long(x["SuperFamily"].ToString())).Distinct().ToList();
				List<long> listPatNums=listDataRowsFromTable.Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
				//the super family heads that did not have balances, but whose members had balances.
				List<long> listSuperFamilyNumsNeeded=listSuperNums.FindAll(x => !ListTools.In(x,listPatNums)); 
				if(listSuperFamilyNumsNeeded.Count>0) {
					//get our super family heads that did not have balances but whose members have balances.
					command="SELECT "+guarOrPat+".PatNum,"+guarOrPat+".FName,"+guarOrPat+".MiddleI,"+guarOrPat+".Preferred,"+guarOrPat+".LName,"+guarOrPat+".ClinicNum,guar.SuperFamily,"
					+"guar.HasSuperBilling,guar.BillingType,"
					+"guar.Bal_0_30,guar.Bal_31_60,guar.Bal_61_90,guar.BalOver90,guar.BalTotal,guar.InsEst,guar.PayPlanDue,"
					+"COALESCE(MAX(statement.DateSent),'0001-01-01') AS lastStatement "
					+"FROM patient guar "
					+"LEFT JOIN statement ON "+guarOrPat+".PatNum=statement.PatNum "
					+"WHERE "+guarOrPat+".PatNum IN("+string.Join(",",listSuperFamilyNumsNeeded.Select(x => POut.Long(x)))+") "
					+"GROUP BY "+guarOrPat+".PatNum "
					+"ORDER BY "+guarOrPat+".LName,"+guarOrPat+".FName ";
					DataTable superHeadTable=Db.GetTable(command);
					if(superHeadTable.Rows.Count>0) {
						table.Merge(superHeadTable);
						DataView combinedView=table.DefaultView;
						combinedView.Sort="LName,FName";
						table=combinedView.ToTable();
					}
				}
			}
			List<string> listSuperFamNums=listDataRowsFromTable.Select(x => x["SuperFamily"].ToString()).Where(x => x!="0").Distinct().ToList();
			//Create a dictionary for each super family head member and create a PatAging object that will represent the entire super family.
			Dictionary<long,PatAging> dictSuperFamPatAging=new Dictionary<long,PatAging>();
			if(listSuperFamNums.Count>0) {
				command="SELECT supe.PatNum,supe.LName,supe.FName,supe.MiddleI,supe.Preferred,supe.SuperFamily,supe.BillingType,supe.ClinicNum,"
					+"COALESCE(MAX(statement.DateSent),'0001-01-01') lastSuperStatement "
					+"FROM patient supe "
					+"LEFT JOIN statement ON supe.PatNum=statement.SuperFamily "
						+(ignoreInPerson?("AND statement.Mode_!="+POut.Int((int)StatementMode.InPerson)+" "):"")
					+"WHERE supe.PatNum=supe.SuperFamily "
					+"AND supe.HasSuperBilling=1 "
					+"GROUP BY supe.PatNum "
					+"ORDER BY NULL";
				dictSuperFamPatAging=Db.GetTable(command).Select().Where(x => listSuperFamNums.Contains(x["PatNum"].ToString()))
					.ToDictionary(x => PIn.Long(x["PatNum"].ToString()),x => new PatAging() {
						PatNum=PIn.Long(x["PatNum"].ToString()),
						DateLastStatement=PIn.Date(x["lastSuperStatement"].ToString()),
						SuperFamily=PIn.Long(x["SuperFamily"].ToString()),
						HasSuperBilling=true,//query only returns super heads who do have super billing
						PatName=Patients.GetNameLF(PIn.String(x["LName"].ToString()),PIn.String(x["FName"].ToString()),PIn.String(x["Preferred"].ToString()),
							PIn.String(x["MiddleI"].ToString())),
						BillingType=PIn.Long(x["BillingType"].ToString()),
						ClinicNum=PIn.Long(x["ClinicNum"].ToString())
					});
			}
			//Only worry about looping through the entire data table for super family shenanigans if there are any super family head members present.
			if(dictSuperFamPatAging.Count>0 && isSuperStatements) {
				PatAging patAgingCur;
				//Now that we know all of the super family heads, loop through all the other patients that showed up in the outstanding aging list
				//and add each super family memeber's aging to their corresponding super family head PatAging entry in the dictionary.
				foreach(DataRow rCur in table.Rows) {
					if(rCur["HasSuperBilling"].ToString()!="1" || rCur["SuperFamily"].ToString()=="0") {
						continue;
					}
					if(!dictSuperFamPatAging.TryGetValue(PIn.Long(rCur["SuperFamily"].ToString()),out patAgingCur)) {
						continue;//super head must not have super billing
					}
					patAgingCur.Bal_0_30+=PIn.Double(rCur["Bal_0_30"].ToString());
					patAgingCur.Bal_31_60+=PIn.Double(rCur["Bal_31_60"].ToString());
					patAgingCur.Bal_61_90+=PIn.Double(rCur["Bal_61_90"].ToString());
					patAgingCur.BalOver90+=PIn.Double(rCur["BalOver90"].ToString());
					patAgingCur.BalTotal+=PIn.Double(rCur["BalTotal"].ToString());
					patAgingCur.InsEst+=PIn.Double(rCur["InsEst"].ToString());
					patAgingCur.AmountDue=patAgingCur.BalTotal-patAgingCur.InsEst;
					patAgingCur.PayPlanDue+=PIn.Double(rCur["PayPlanDue"].ToString());
				}
			}
			PatAging patage;
			DateTime dateLastStatement;
			DateTime maxDate;
			foreach(DataRow rowCur in table.Rows) {
				patage=new PatAging();
				patage.PatNum=PIn.Long(rowCur["PatNum"].ToString());
				patage.SuperFamily=PIn.Long(rowCur["SuperFamily"].ToString());
				patage.HasSuperBilling=PIn.Bool(rowCur["HasSuperBilling"].ToString());
				patage.HasSignedTil=PIn.Bool(rowCur["HasSignedTil"].ToString());
				patage.ClinicNum=PIn.Long(rowCur["ClinicNum"].ToString());
				dateLastStatement=DateTime.MinValue;
				patage.PriProv=PIn.Long(rowCur["PriProv"].ToString());
				patage.Zip=PIn.String(rowCur["Zip"].ToString());
				patage.PatStatus=PIn.Enum<PatientStatus>(rowCur["PatStatus"].ToString());
				PatAging superPat;
				if(patage.HasSuperBilling && dictSuperFamPatAging.TryGetValue(patage.SuperFamily,out superPat)) {
					dateLastStatement=superPat.DateLastStatement;
				}
				//If pat HasSuperBilling and super head has received a super statement, dateLastStatement will be the more recent date of the last super 
				//statement or the last family statement, regardless of the isSuperStatements flag.  If the guar is not in a super family with super billing, 
				//dateLastStatement will be the date of their last family statement (not-super statement).
				dateLastStatement=new[] { dateLastStatement,PIn.Date(rowCur["lastStatement"].ToString()) }.Max();
				maxDate=DateTime.MinValue;
				//dict will not contain any values if 'includeChanged' is false
				if(dictPatAgingTransactions.TryGetValue(patage.PatNum,out List<PatAgingTransaction> listPatAgingTransactions)) {
					maxDate=AgingData.GetDateLastTrans(listPatAgingTransactions,dateLastStatement);
				}
				if((!isFinanceBilling && dateLastStatement.Date>=new[] { lastStatement.AddDays(1),maxDate }.Max().Date)
					|| listPendingInsPatNums.Contains(patage.PatNum) //list only filled if excluding pending ins
					|| listUnsentPatNums.Contains(patage.PatNum) //list only filled if excluding unsent procs
					|| (isSuperStatements && patage.HasSuperBilling && patage.PatNum!=patage.SuperFamily) //included in super statement and not the super head, skip
					|| (dateLastStatement.Date<lastStatement.Date && excludeNotBilledSince))//exclude account not billed since date
				{
					continue;//this patient is excluded, skip
				}
				//if not generating super statements OR this guar doesn't have super billing OR this guar's super family num isn't in the dictionary
				//then this guar will get a non-super statement (regular single family statement)
				if(!isSuperStatements || !patage.HasSuperBilling || !dictSuperFamPatAging.TryGetValue(patage.SuperFamily,out patage)) {
					patage.Bal_0_30=PIn.Double(rowCur["Bal_0_30"].ToString());
					patage.Bal_31_60=PIn.Double(rowCur["Bal_31_60"].ToString());
					patage.Bal_61_90=PIn.Double(rowCur["Bal_61_90"].ToString());
					patage.BalOver90=PIn.Double(rowCur["BalOver90"].ToString());
					patage.BalTotal=PIn.Double(rowCur["BalTotal"].ToString());
					patage.InsEst=PIn.Double(rowCur["InsEst"].ToString());
					patage.AmountDue=patage.BalTotal-patage.InsEst;
					patage.PayPlanDue =PIn.Double(rowCur["PayPlanDue"].ToString());
					patage.DateLastStatement=PIn.Date(rowCur["lastStatement"].ToString());
					patage.PatName=Patients.GetNameLF(PIn.String(rowCur["LName"].ToString()),PIn.String(rowCur["FName"].ToString()),
						PIn.String(rowCur["Preferred"].ToString()),PIn.String(rowCur["MiddleI"].ToString()));
					patage.BillingType=PIn.Long(rowCur["BillingType"].ToString());
				}
				agingList.Add(patage);
			}
			return agingList;
		}

		///<summary>Will include negative and zero bals if doIncludeZeroBalance is true.  If including zero bals and isGuarsOnly is false, this will
		///include non-guars as well as guars with a zero bal.  Will only include pats with PatNum=Guarantor is isGuarsOnly is true.  Will include all
		///pats if listGuarantors is null or empty.  Will include all billing types if listBillingTypeNums is null or empty.  Filters by pat or guar
		///ClinicNum if listClinicNums is provided.</summary>
		public static List<PatAging> GetAgingListSimple(List<long> listBillingTypeNums,List<long> listGuarantors,bool doIncludeZeroBalance=false,
			bool isGuarsOnly=false,List<long> listClinicNums=null,bool doIncludeSuperFamilyHeads=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatAging>>(MethodBase.GetCurrentMethod(),listBillingTypeNums,listGuarantors,doIncludeZeroBalance,isGuarsOnly,
					listClinicNums,doIncludeSuperFamilyHeads);
			}
			List<string> listWhereAnds=new List<string>();
			if(!doIncludeZeroBalance) {
				listWhereAnds.Add("Bal_0_30 + Bal_31_60 + Bal_61_90 + BalOver90 - InsEst > '0.005'");//more that 1/2 cent
			}
			if(!listBillingTypeNums.IsNullOrEmpty()) {
				listWhereAnds.Add("BillingType IN ("+string.Join(",",listBillingTypeNums)+")");
			}
			if(!listGuarantors.IsNullOrEmpty()) {
				listWhereAnds.Add("PatNum IN ("+string.Join(",",listGuarantors)+")");
			}
			if(!listClinicNums.IsNullOrEmpty()) {
				listWhereAnds.Add("ClinicNum IN ("+string.Join(",",listClinicNums)+")");
			}
			if(isGuarsOnly) {
				listWhereAnds.Add("PatNum=Guarantor");
			}
			string whereClause="";
			if(listWhereAnds.Count>0) {
				whereClause="WHERE ("+string.Join(" AND ",listWhereAnds)+") ";
			}
			if(doIncludeSuperFamilyHeads) {
				if(string.IsNullOrEmpty(whereClause)) {
					whereClause+="WHERE ";
				}
				else {
					whereClause+="OR ";
				}
				whereClause+="PatNum=SuperFamily ";
			}
			string command=$@"SELECT PatNum,Bal_0_30,Bal_31_60,Bal_61_90,BalOver90,BalTotal,InsEst,LName,FName,MiddleI,Preferred,PriProv,BillingType,
				Guarantor,SuperFamily,HasSuperBilling,ClinicNum
				FROM patient
				{whereClause}
				ORDER BY LName,FName";
			return Db.GetList(command,x =>
				new PatAging() {
					PatNum          = PIn.Long(x["PatNum"].ToString()),
					Bal_0_30        = PIn.Double(x["Bal_0_30"].ToString()),
					Bal_31_60       = PIn.Double(x["Bal_31_60"].ToString()),
					Bal_61_90       = PIn.Double(x["Bal_61_90"].ToString()),
					BalOver90       = PIn.Double(x["BalOver90"].ToString()),
					BalTotal        = PIn.Double(x["BalTotal"].ToString()),
					InsEst          = PIn.Double(x["InsEst"].ToString()),
					PatName         = Patients.GetNameLF(PIn.String(x["LName"].ToString()),PIn.String(x["FName"].ToString()),PIn.String(x["Preferred"].ToString()),
						PIn.String(x["MiddleI"].ToString())),
					AmountDue       = PIn.Double(x["BalTotal"].ToString())-PIn.Double(x["InsEst"].ToString()),
					PriProv         = PIn.Long(x["PriProv"].ToString()),
					BillingType     = PIn.Long(x["BillingType"].ToString()),
					Guarantor       = PIn.Long(x["Guarantor"].ToString()),
					SuperFamily     = PIn.Long(x["SuperFamily"].ToString()),
					HasSuperBilling = PIn.Bool(x["HasSuperBilling"].ToString()),
					ClinicNum       = PIn.Long(x["ClinicNum"].ToString())
				});
		}

		///<summary>Used only by the OpenDentalService Transworld thread to sync accounts sent for collection.  Gets a list of PatAgings for the guars
		///identified by the PatNums in listGuarNums.  Will return all, even negative bals.  Does not consider SuperFamilies, only individual guars.</summary>
		public static List<PatAging> GetAgingListFromGuarNums(List<long> listGuarNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatAging>>(MethodBase.GetCurrentMethod(),listGuarNums);
			}
			if(listGuarNums.Count<1) {
				return new List<PatAging>();
			}
			string command="SELECT PatNum,Guarantor,LName,FName,MiddleI,PriProv,BillingType,ClinicNum,Bal_0_30,Bal_31_60,Bal_61_90,BalOver90,BalTotal,InsEst "
				+"FROM patient "
				+"WHERE patient.PatNum IN ("+string.Join(",",listGuarNums.Select(x => POut.Long(x)))+") "
				+"AND patient.Guarantor=patient.PatNum";
			List<PatAging> listPatAgings=Db.GetTable(command).Select().Select(x => new PatAging() {
					PatNum     =PIn.Long(x["PatNum"].ToString()),
					Guarantor  =PIn.Long(x["Guarantor"].ToString()),
					PatName    =PIn.String(x["LName"].ToString())+", "+PIn.String(x["FName"].ToString())+" "+PIn.String(x["MiddleI"].ToString()),
					PriProv    =PIn.Long(x["PriProv"].ToString()),
					BillingType=PIn.Long(x["BillingType"].ToString()),
					ClinicNum  =PIn.Long(x["ClinicNum"].ToString()),
					Bal_0_30   =PIn.Double(x["Bal_0_30"].ToString()),
					Bal_31_60  =PIn.Double(x["Bal_31_60"].ToString()),
					Bal_61_90  =PIn.Double(x["Bal_61_90"].ToString()),
					BalOver90  =PIn.Double(x["BalOver90"].ToString()),
					BalTotal   =PIn.Double(x["BalTotal"].ToString()),
					InsEst     =PIn.Double(x["InsEst"].ToString()),
					AmountDue  =PIn.Double(x["BalTotal"].ToString())-PIn.Double(x["InsEst"].ToString()),
					ListTsiLogs=new List<TsiTransLog>()
				}).ToList();
			if(listPatAgings.Count==0) {
				return listPatAgings;
			}
			Dictionary<long,List<TsiTransLog>> dictPatNumListTrans=TsiTransLogs.SelectMany(listGuarNums)
				.GroupBy(x => x.PatNum)
				.ToDictionary(x => x.Key,x => x.OrderByDescending(y => y.TransDateTime).ToList());
			foreach(PatAging patAgingCur in listPatAgings) {
				if(!dictPatNumListTrans.TryGetValue(patAgingCur.Guarantor,out patAgingCur.ListTsiLogs)) {
					patAgingCur.ListTsiLogs=new List<TsiTransLog>();
				}
			}
			return listPatAgings;
		}

		///<summary>Gets the next available integer chart number.  Will later add a where clause based on preferred format.</summary>
		public static string GetNextChartNum(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string command="SELECT ChartNumber from patient WHERE "
				+DbHelper.Regexp("ChartNumber","^[0-9]+$")+" "//matches any positive number of digits
				+"ORDER BY (chartnumber+0) DESC";//1/13/05 by Keyush Shaw-added 0.
			command=DbHelper.LimitOrderBy(command,1);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){//no existing chart numbers
				return "1";
			}
			string lastChartNum=PIn.String(table.Rows[0][0].ToString());
			//or could add more match conditions
			try {
				return (Convert.ToInt64(lastChartNum)+1).ToString();
			}
			catch {
				throw new ApplicationException(lastChartNum+" is an existing ChartNumber.  It's too big to convert to a long int, so it's not possible to add one to automatically increment.");
			}
		}

		///<summary>Returns the name(only one) of the patient using this chartnumber.</summary>
		public static string ChartNumUsedBy(string chartNum,long excludePatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),chartNum,excludePatNum);
			}
			string command="SELECT LName,FName from patient WHERE "
				+"ChartNumber = '"+POut.String(chartNum)
				+"' AND PatNum != '"+excludePatNum.ToString()+"'";
			DataTable table=Db.GetTable(command);
			string retVal="";
			if(table.Rows.Count!=0){//found duplicate chart number
				retVal=PIn.String(table.Rows[0][1].ToString())+" "+PIn.String(table.Rows[0][0].ToString());
			}
			return retVal;
		}

		///<summary>Used in the patient select window to determine if a trial version user is over their limit.</summary>
		public static int GetNumberPatients(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT Count(*) FROM patient";
			DataTable table=Db.GetTable(command);
			return PIn.Int(table.Rows[0][0].ToString());
		}

		///<summary>Makes a call to the db to figure out if the current HasIns status is correct.  If not, then it changes it.</summary>
		public static void SetHasIns(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="SELECT patient.HasIns,COUNT(patplan.PatNum) FROM patient "
				+"LEFT JOIN patplan ON patplan.PatNum=patient.PatNum"
				+" WHERE patient.PatNum="+POut.Long(patNum)
				+" GROUP BY patplan.PatNum,patient.HasIns";
			DataTable table=Db.GetTable(command);
			string newVal="";
			if(table.Rows[0][1].ToString()!="0"){
				newVal="I";
			}
			if(newVal!=table.Rows[0][0].ToString()){
				command="UPDATE patient SET HasIns='"+POut.String(newVal)
					+"' WHERE PatNum="+POut.Long(patNum);
				Db.NonQ(command);
			}
		}

		///<summary>Gets the provider for this patient.  If provNum==0, then it gets the practice default prov.
		///If no practice default set, returns the first non-hidden ProvNum from the provider cache.</summary>
		public static long GetProvNum(Patient pat) {
			//No need to check RemotingRole; no call to db.
			long retval=pat.PriProv;
			if(retval==0) {
				retval=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			if(retval==0) {
				retval=Providers.GetFirstOrDefault(x => true,true)?.ProvNum??0;
			}
			return retval;
		}

		///<summary>Calls Patients.GetProvNum after getting the patient with this patNum. Gets the provider for this patient.  If pat.PriProv==0, then it
		///gets the practice default prov.  If no practice default set, returns the first non-hidden ProvNum from the provider cache.</summary>
		public static long GetProvNum(long patNum) {
			//No need to check RemotingRole; no call to db.
			return GetProvNum(GetPat(patNum));
		}

		///<summary>Gets the list of all valid patient primary keys. Allows user to specify whether to include non-deleted patients. Used when checking for missing ADA procedure codes after a user has begun entering them manually. This function is necessary because not all patient numbers are necessarily consecutive (say if the database was created due to a conversion from another program and the customer wanted to keep their old patient ids after the conversion).</summary>
		public static long[] GetAllPatNums(bool hasDeleted) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<long[]>(MethodBase.GetCurrentMethod(),hasDeleted);
			}
			string command="";
			if(hasDeleted) {
				command="SELECT PatNum FROM patient";
			}
			else {
				command="SELECT PatNum FROM patient WHERE patient.PatStatus!=4";
			}
			DataTable dt=Db.GetTable(command);
			long[] patnums=new long[dt.Rows.Count];
			for(int i=0;i<patnums.Length;i++){
				patnums[i]=PIn.Long(dt.Rows[i]["PatNum"].ToString());
			}
			return patnums;
		}

		///<summary>Converts a date to an age. If age is over 115, then returns 0.</summary>
		public static int DateToAge(DateTime date){
			//No need to check RemotingRole; no call to db.
			if(date.Year<1880)
				return 0;
			if(date.Month < DateTime.Now.Month){//birthday in previous month
				return DateTime.Now.Year-date.Year;
			}
			if(date.Month == DateTime.Now.Month && date.Day <= DateTime.Now.Day){//birthday in this month
				return DateTime.Now.Year-date.Year;
			}
			return DateTime.Now.Year-date.Year-1;
		}

		///<summary>Converts a date to an age. If age is over 115, then returns 0.</summary>
		public static int DateToAge(DateTime birthdate,DateTime asofDate) {
			//No need to check RemotingRole; no call to db.
			if(birthdate.Year<1880)
				return 0;
			if(birthdate.Month < asofDate.Month) {//birthday in previous month
				return asofDate.Year-birthdate.Year;
			}
			if(birthdate.Month == asofDate.Month && birthdate.Day <= asofDate.Day) {//birthday in this month
				return asofDate.Year-birthdate.Year;
			}
			return asofDate.Year-birthdate.Year-1;
		}

		///<summary>If zero, returns empty string.  Otherwise returns simple year.  Also see PatientLogic.DateToAgeString().</summary>
		public static string AgeToString(int age){
			//No need to check RemotingRole; no call to db.
			if(age==0) {
				return "";
			}
			else {
				return age.ToString();
			}
		}

		public static void ReformatAllPhoneNumbers() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string oldTel;
			string newTel;
			string idNum;
			string command="select * from patient";
			DataTable table=Db.GetTable(command);
			List<long> listPatNumsForPhNumSync=new List<long>();
			bool doSyncPhNumTable=PrefC.GetYN(PrefName.PatientPhoneUsePhonenumberTable);
			for(int i=0;i<table.Rows.Count;i++) {
				long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				idNum=PIn.String(table.Rows[i][0].ToString());
				//home
				oldTel=PIn.String(table.Rows[i][15].ToString());
				newTel=TelephoneNumbers.ReFormat(oldTel);
				if(oldTel!=newTel) {
					command="UPDATE patient SET hmphone = '"
						+POut.String(newTel)+"' WHERE patNum = '"+idNum+"'";
					Db.NonQ(command);
					if(doSyncPhNumTable) {
						listPatNumsForPhNumSync.Add(patNum);
					}
				}
				//wk:
				oldTel=PIn.String(table.Rows[i][16].ToString());
				newTel=TelephoneNumbers.ReFormat(oldTel);
				if(oldTel!=newTel) {
					command="UPDATE patient SET wkphone = '"
						+POut.String(newTel)+"' WHERE patNum = '"+idNum+"'";
					Db.NonQ(command);
					if(doSyncPhNumTable) {
						listPatNumsForPhNumSync.Add(patNum);
					}
				}
				//wireless
				oldTel=PIn.String(table.Rows[i][17].ToString());
				newTel=TelephoneNumbers.ReFormat(oldTel);
				if(oldTel!=newTel) {
					command="UPDATE patient SET wirelessphone = '"
						+POut.String(newTel)+"' WHERE patNum = '"+idNum+"'";
					Db.NonQ(command);
					if(doSyncPhNumTable) {
						listPatNumsForPhNumSync.Add(patNum);
					}
				}
			}
			if(doSyncPhNumTable && listPatNumsForPhNumSync.Count>0) {
				PhoneNumbers.SyncPats(GetMultPats(listPatNumsForPhNumSync.Distinct().ToList()).ToList());
			}
			command="SELECT * from carrier";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				idNum=PIn.String(table.Rows[i][0].ToString());
				//ph
				oldTel=PIn.String(table.Rows[i][7].ToString());
				newTel=TelephoneNumbers.ReFormat(oldTel);
				if(oldTel!=newTel) {
					command="UPDATE carrier SET Phone = '"
						+POut.String(newTel)+"' WHERE CarrierNum = '"+idNum+"'";
					Db.NonQ(command);
				}
			}
			command="SELECT PatNum,ICEPhone FROM patientnote";
			table=Db.GetTable(command);
			for(int i = 0;i<table.Rows.Count;i++) {
				idNum=PIn.String(table.Rows[i]["PatNum"].ToString());
				oldTel=PIn.String(table.Rows[i]["ICEPhone"].ToString());
				newTel=TelephoneNumbers.ReFormat(oldTel);
				if(oldTel!=newTel) {
					command="UPDATE patientnote SET ICEPhone='"+POut.String(newTel)+"' WHERE PatNum="+idNum;
					Db.NonQ(command);
				}
			}
		}

		public static DataTable GetGuarantorInfo(long PatientID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatientID);
			}
			string command=@"SELECT FName,MiddleI,LName,Guarantor,Address,
								Address2,City,State,Zip,Email,EstBalance,
								BalTotal,Bal_0_30,Bal_31_60,Bal_61_90,BalOver90
						FROM Patient Where Patnum="+PatientID+
				" AND patnum=guarantor";
			return Db.GetTable(command);
		}

		///<summary>Will return 0 if can't find exact matching pat.</summary>
		public static long GetPatNumByNameAndBirthday(string lName,string fName,DateTime birthdate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),lName,fName,birthdate);
			}
			string command="SELECT PatNum FROM patient WHERE "
				+"LName='"+POut.String(lName)+"' "
				+"AND FName='"+POut.String(fName)+"' "
				+"AND Birthdate="+POut.Date(birthdate)+" "
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Archived)+" "//Not Archived
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);//Not Deleted
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Will return an empty list if it can't find exact matching pat.</summary>
		//Search is case-insensitive by default, since patient.LName and patient.FName collation is utf8_general_ci (ci=case-insensitive)
		public static List<long> GetListPatNumsByNameAndBirthday(string lName,string fName,DateTime birthdate,bool isPreferredMatch=false,bool isExactMatch=true,long clinicNum=-1) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),lName,fName,birthdate,isPreferredMatch,isExactMatch,clinicNum);
			}
			string createComparator(string col,string val,bool isExact) => isExact switch {
				//Match exactly (case insensitive):  ex, 'John'='John'
				true	=> $"{col}='{POut.String(val)}'",
				//Match with 0 or more chars before, and 0 or more chars after:  ex, 'Liz' is like 'Elizabeth'
				false => $"{col} LIKE '%{POut.String(val)}%'",
			};
			string createNameClause(string col,string val,bool isPreferred,bool isExact) => isPreferred switch {
				//Match both col or Preferred: ex, FName or Preferred
				true	=> $"({createComparator(col,val,isExact)} OR {createComparator("Preferred",val,isExact)})",
				//Match only col: ex, FName only
				false	=> createComparator(col,val,isExact),
			};
			string command="SELECT PatNum FROM patient "
				+"WHERE Birthdate="+POut.Date(birthdate)+" "
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Archived)+" "//Not Archived
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Deleted)+" "//Not Deleted
				+"AND "+createNameClause("LName",lName,false,true)+" "//LName is always 'exact' match
				+"AND "+createNameClause("FName",fName,isPreferredMatch,isExactMatch);//FName may be 'exact' or 'partial' match, and may include Preferred
			if(clinicNum>=0) {
				command+=" AND ClinicNum="+POut.Long(clinicNum);
			}
			return Db.GetListLong(command);
		}

		///<summary>Returns a list of all patients within listSortedPatients which match the given pat.LName, pat.FName and pat.Birthdate.
		///Ignores case and leading/trailing space.  The listSortedPatients MUST be sorted by LName, then FName, then Birthdate or else the result will be
		///wrong.  Call listSortedPatients.Sort() before calling this function.  This function uses a binary search to much more efficiently locate
		///matches than a linear search would be able to.</summary>
		public static List<Patient> GetPatientsByNameAndBirthday(Patient pat,List <Patient> listSortedPatients) {
			if(pat.LName.Trim().ToLower().Length==0 || pat.FName.Trim().ToLower().Length==0 || pat.Birthdate.Year < 1880) {
				//We do not allow a match unless Last Name, First Name, AND birthdate are specified.  Otherwise at match could be meaningless.
				return new List<Patient>();
			}
			int patIdx=listSortedPatients.BinarySearch(pat);//If there are multiple matches, then this will only return one of the indexes randomly.
			if(patIdx < 0) {
				//No matches found.
				return new List<Patient>();
			}
			//The matched indicies will all be consecutive and will include the returned index from the binary search, because the list is sorted.
			int beginIdx=patIdx;
			for(int i=patIdx-1;i >= 0 && pat.CompareTo(listSortedPatients[i])==0;i--) {
				beginIdx=i;
			}
			int endIdx=patIdx;
			for(int i=patIdx+1;i < listSortedPatients.Count && pat.CompareTo(listSortedPatients[i])==0;i++) {
				endIdx=i;
			}
			List <Patient> listPatientMatches=new List<Patient>();
			for(int i=beginIdx;i<=endIdx;i++) {
				listPatientMatches.Add(listSortedPatients[i]);
			}
			return listPatientMatches;
		}

		///<summary>Returns the PatNums with the same name and birthday as passed in. The email and the phone numbers passed in will only be considered
		///if there is more than one patient with the same name and birthday. If a patient's family member's email or phone matches the ones passed in,
		///then that patient will be included.</summary>
		public static List<long> GetPatNumsByNameBirthdayEmailAndPhone(string lName,string fName,DateTime birthDate,string email,
			List<string> listPhones) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),lName,fName,birthDate,email,listPhones);
			}
			//Get all potential matches by name and birthdate first.
			List<long> listMatchingNameDOB=GetListPatNumsByNameAndBirthday(lName,fName,birthDate);
			if(listMatchingNameDOB.Count < 2) {
				return listMatchingNameDOB;//One or no matches via name and birth date so no need to waste time checking for phone/email matches in the fam.
			}
			//There are some potential duplicates found in the database.  Now we need to make sure that the email OR the phone is already on file.
			//We are going to look at every single phone number and email address on all family members just in case.
			string command="SELECT patient.PatNum,patient.Guarantor,fam.PatNum AS FamMemberPatNum,"
				+"fam.Email,fam.HmPhone,fam.WkPhone,fam.WirelessPhone,COALESCE(phonenumber.PhoneNumberVal,'') AS OtherPhone "
				+"FROM patient "
				+"INNER JOIN patient g ON g.PatNum=patient.Guarantor "
				+"INNER JOIN patient fam ON fam.Guarantor=g.PatNum "
				+"LEFT JOIN phonenumber ON phonenumber.PatNum=fam.PatNum "
				+"WHERE patient.PatNum IN ("+string.Join(",",listMatchingNameDOB)+") "
				+"AND fam.PatStatus != "+POut.Int((int)PatientStatus.Deleted);
			listPhones=listPhones.Where(x => !string.IsNullOrEmpty(x)) //Get rid of blank numbers
				.Select(x => StringTools.StripNonDigits(x)).ToList();//Get rid of non-digit characters
			List<long> listMatchingContacts=Db.GetTable(command).Rows.Cast<DataRow>()
				.Where(x => PIn.String(x["Email"].ToString())==email
					|| listPhones.Any(y => y==StringTools.StripNonDigits(PIn.String(x["HmPhone"].ToString())))
					|| listPhones.Any(y => y==StringTools.StripNonDigits(PIn.String(x["WkPhone"].ToString())))
					|| listPhones.Any(y => y==StringTools.StripNonDigits(PIn.String(x["WirelessPhone"].ToString())))
					|| listPhones.Any(y => y==StringTools.StripNonDigits(PIn.String(x["OtherPhone"].ToString()))))
				.Select(x => PIn.Long(x["PatNum"].ToString())).Distinct().ToList();
			if(listMatchingContacts.Count > 0) {//We have found at least one match based on contact info.
				return listMatchingContacts;
			}
			//There weren't any matches found from contact info.
			return listMatchingNameDOB;
		}

		///<summary>Returns true if there is an exact match in the database based on the lName, fName, and birthDate passed in.
		///Also, the phone number or the email must match at least one phone number or email on file for any patient within the family.
		///Otherwise we assume a match is not within the database because some offices have multiple clinics and we need strict matching.</summary>
		public static bool GetHasDuplicateForNameBirthdayEmailAndPhone(string lName,string fName,DateTime birthDate,string email,string phone) {
			return GetHasDuplicateForNameBirthdayEmailAndPhone(lName,fName,birthDate,email,new List<string> { phone });
		}

		///<summary>Returns true if there is an exact match in the database based on the lName, fName, and birthDate passed in.
		///Also, one of the phone numbers or the email must match at least one phone number or email on file for any patient within the family.
		///Otherwise we assume a match is not within the database because some offices have multiple clinics and we need strict matching.</summary>
		public static bool GetHasDuplicateForNameBirthdayEmailAndPhone(string lName,string fName,DateTime birthDate,string email,
			List<string> listPhones) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),lName,fName,birthDate,email,listPhones);
			}
			//Get all potential matches by name and brith date first.
			List<long> listPatNums=GetListPatNumsByNameAndBirthday(lName,fName,birthDate);
			if(listPatNums.Count < 1) {
				return false;//No matches via name and birth date so no need to waste time checking for phone / email matches in the family.
			}
			string command="";
			//There are some potential duplicates found in the database.  Now we need to make sure that the email OR the phone is already on file.
			//We are going to look at every single phone number and email address on all family members just in case.
			List<long> listFamilyPatNums=GetAllFamilyPatNums(listPatNums);//Should never return an empty list.
			//Only waste time checking for patients with the same email address if an email was passed in.
			if(!string.IsNullOrEmpty(email)) {
				command="SELECT COUNT(*) FROM patient "
				+"WHERE patient.Email='"+POut.String(email)+"' "
				+"AND PatNum IN ("+string.Join(",",listFamilyPatNums)+")";
				if(Db.GetCount(command)!="0") {
					return true;//The name and birth date match AND someone in the family has the exact email address passed in.  This is consider a duplicate.
				}
			}
			//Query to get all phone numbers from both the patient table and the 
			command="SELECT HmPhone FROM patient WHERE PatNum IN ("+string.Join(",",listFamilyPatNums)+") "
				+"UNION SELECT WkPhone Phone FROM patient WHERE PatNum IN ("+string.Join(",",listFamilyPatNums)+") "
				+"UNION SELECT WirelessPhone Phone FROM patient WHERE PatNum IN ("+string.Join(",",listFamilyPatNums)+") "
				+"UNION SELECT PhoneNumberVal Phone FROM phonenumber WHERE PatNum IN ("+string.Join(",",listFamilyPatNums)+") ";
			List<string> listAllFamilyPhones=Db.GetListString(command).Where(x => !string.IsNullOrEmpty(x)).ToList();
			listPhones=listPhones.Where(x => x != null)
				.Select(x => StringTools.StripNonDigits(x)).ToList();//Get rid of non-digit characters
			//Go through each phone number and strip out all non-digit chars and compare them to the phone passed in.
			foreach(string phoneFamily in listAllFamilyPhones) {
				string phoneFamDigitsOnly=StringTools.StripNonDigits(phoneFamily);
				if(listPhones.Any(x => x.Contains(phoneFamDigitsOnly) || phoneFamDigitsOnly.Contains(x))) {
					return true;//The name and birth date match AND someone in the family has the exact phone passed in.  This is consider a duplicate.
				}
			}
			return false;
		}

		///<summary>Will return 0 if can't find an exact matching pat.  Because it does not include birthdate, it's not specific enough for most situations.</summary>
		public static long GetPatNumByName(string lName,string fName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),lName,fName);
			}
			string command="SELECT PatNum FROM patient WHERE "
				+"LName='"+POut.String(lName)+"' "
				+"AND FName='"+POut.String(fName)+"' "
				+"AND PatStatus!=4 "//not deleted
				+"LIMIT 1";
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Gets a list of patients that have any part of their name (last, first, middle, preferred) that matches the given criteria.
		///Optionally give a clinicNum and the query will only include patients associated with that clinic (patient.ClinicNum).</summary>
		public static List<Patient> GetPatientsByPartialName(string partialName,long clinicNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),partialName,clinicNum);
			}
			string command="SELECT * FROM patient WHERE 1 ";
			List<string> listNames=partialName.Split().Select(x => POut.String(x.ToLower())).ToList();
			foreach(string name in listNames) {
				command+="AND (LName LIKE '%"+POut.String(name)+"%' "
				+"OR FName LIKE '%"+POut.String(name)+"%' "
				+"OR MiddleI LIKE '%"+POut.String(name)+"%' "
				+"OR Preferred LIKE '%"+POut.String(name)+"%') ";
			}
			if(clinicNum > 0) {
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			return Crud.PatientCrud.SelectMany(command);
		}

		/// <summary>When importing webforms, if it can't find an exact match, this method attempts a similar match.</summary>
		public static List<Patient> GetSimilarList(string lName,string fName,DateTime birthdate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),lName,fName,birthdate);
			}
			int subStrIndexlName=2;
			int subStrIndexfName=2;
			if(lName.Length<2) {
				subStrIndexlName=lName.Length;
			}
			if(fName.Length<2) {
				subStrIndexfName=fName.Length;
			}
			string command="SELECT * FROM patient WHERE "
				+"LName LIKE '"+POut.String(lName.Substring(0,subStrIndexlName))+"%' "
				+"AND FName LIKE '"+POut.String(fName.Substring(0,subStrIndexfName))+"%' "
				+"AND (Birthdate="+POut.Date(birthdate)+" "//either a matching bd
				+"OR Birthdate < "+POut.Date(new DateTime(1880,1,1))+") "//or no bd
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Archived)+" "//Not Archived
				+"AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);//Not Deleted
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Returns a list of patients that match last and first name.  Case insensitive depending on table collation.</summary>
		public static List<Patient> GetListByName(string lName,string fName,long PatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),lName,fName,PatNum);
			}
			string command=$@"SELECT * FROM patient
				WHERE PatNum!={POut.Long(PatNum)}
				AND PatStatus!={POut.Int((int)PatientStatus.Deleted)}
				AND FName='{POut.String(fName)}'
				AND LName='{POut.String(lName)}'";
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Returns a list of patients that have the same last name, first name, and birthdate, ignoring case sensitivity, but different patNum.  Used to find duplicate patients that may be clones of the patient identified by the patNum parameter, or are the non-clone version of the patient.  Currently only used with GetCloneAndNonClone to find the non-clone and clone patients for the pateint sent in if they exist.</summary>
		public static List<Patient> GetListByNameAndBirthdate(long patNum,string lName,string fName,DateTime birthdate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),patNum,lName,fName,birthdate);
			}
			string command="SELECT * FROM patient WHERE LName LIKE '"+POut.String(lName)+"' AND FName LIKE '"+POut.String(fName)+"' "
				+"AND Birthdate="+POut.Date(birthdate,true)+" AND PatNum!="+POut.Long(patNum) +" AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			return Crud.PatientCrud.SelectMany(command);
		}

		public static void UpdateFamilyBillingType(long billingType,long Guarantor) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),billingType,Guarantor);
				return;
			}
			string command="UPDATE patient SET BillingType="+POut.Long(billingType)+
				" WHERE Guarantor="+POut.Long(Guarantor);
			Db.NonQ(command);
		}

		public static void UpdateAllFamilyBillingTypes(long billingType,List<long> listGuarNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),billingType,listGuarNums);
				return;
			}
			if(listGuarNums.Count<1) {
				return;
			}
			string command="UPDATE patient SET BillingType="+POut.Long(billingType)+" "
				+"WHERE Guarantor IN ("+string.Join(",",listGuarNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		public static DataTable GetPartialPatientData(long PatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatNum);
			}
			string command="SELECT FName,LName,"+DbHelper.DateFormatColumn("birthdate","%m/%d/%Y")+" BirthDate,Gender "
				+"FROM patient WHERE patient.PatNum="+PatNum;
			return Db.GetTable(command);
		}

		public static DataTable GetPartialPatientData2(long PatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatNum);
			}
			string command=@"SELECT FName,LName,"+DbHelper.DateFormatColumn("birthdate","%m/%d/%Y")+" BirthDate,Gender "
				+"FROM patient WHERE PatNum In (SELECT Guarantor FROM PATIENT WHERE patnum = "+PatNum+")";
			return Db.GetTable(command);
		}

		public static string GetEligibilityDisplayName(long patId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),patId);
			}
			string command = @"SELECT FName,LName,"+DbHelper.DateFormatColumn("birthdate","%m/%d/%Y")+" BirthDate,Gender "
				+"FROM patient WHERE patient.PatNum=" + POut.Long(patId);
			DataTable table = Db.GetTable(command);
			if(table.Rows.Count == 0) {
				return "Patient(???) is Eligible";
			}
			return PIn.String(table.Rows[0][1].ToString()) + ", "+ PIn.String(table.Rows[0][0].ToString()) + " is Eligible";
		}

		///<summary>Only a partial folderName will be sent in.  Not the .rvg part.</summary>
		public static bool IsTrophyFolderInUse(string folderName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),folderName);
			}
			string command ="SELECT COUNT(*) FROM patient WHERE TrophyFolder LIKE '%"+POut.String(folderName)+"%'";
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Used to check if a billing type is in use when user is trying to hide it.</summary>
		public static bool IsBillingTypeInUse(long defNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),defNum);
			}
			string command ="SELECT COUNT(*) FROM patient WHERE BillingType="+POut.Long(defNum)+" AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			if(Db.GetCount(command)!="0") {
				return true;
			}
			command ="SELECT COUNT(*) FROM insplan WHERE BillingType="+POut.Long(defNum);
			if(Db.GetCount(command)!="0") {
				return true;
			}
			//check any prefs that are FK's to the definition.DefNum column and warn if a pref is using the def
			if(new[] {
					PrefName.TransworldPaidInFullBillingType,PrefName.ApptEConfirmStatusSent,PrefName.ApptEConfirmStatusAccepted,
					PrefName.ApptEConfirmStatusDeclined,PrefName.ApptEConfirmStatusSendFailed,PrefName.ApptConfirmExcludeEConfirm,
					PrefName.ApptConfirmExcludeERemind,PrefName.ApptConfirmExcludeESend,PrefName.ApptConfirmExcludeEThankYou,
					PrefName.BrokenAppointmentAdjustmentType,PrefName.ConfirmStatusEmailed,
					PrefName.ConfirmStatusTextMessaged,PrefName.PrepaymentUnearnedType,PrefName.SalesTaxAdjustmentType }
				.Select(x => PrefC.GetString(x))
				.SelectMany(x => x.Split(',').Select(y => PIn.Long(y,false)).Where(y => y>0))//some prefs are comma delimited lists of longs. SelectMany will return a single list of longs
				.Any(x => x==defNum))
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if this is a valid U.S Social Security Number.</summary>
		///<param name="formattedSSN">9 digits with dashes.</param>
		public static bool IsValidSSN(string ssn,out string formattedSSN) {
			if(Regex.IsMatch(ssn,@"^\d{9}$")) {//if just 9 numbers, reformat with dashes.
				ssn=ssn.Substring(0,3)+"-"+ssn.Substring(3,2)+"-"+ssn.Substring(5,4);				
			}
			formattedSSN=ssn;
			return Regex.IsMatch(formattedSSN,@"^\d\d\d-\d\d-\d\d\d\d$");
		}

		///<summary>If the current culture is U.S. and the ssn is 9 digits with dashes, removes the dashes.</summary>
		public static string SSNRemoveDashes(string ssn) {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				if(Regex.IsMatch(ssn,@"^\d\d\d-\d\d-\d\d\d\d$")){
					return ssn.Replace("-","");
				}
			}
			return ssn; //other cultures
		}

		///<summary>Updated 09/16/2020 v19.4.46(Check this convert method when updating merge methods).  To prevent orphaned patients, if patFrom is a guarantor then all family members of patFrom are moved into the family patTo belongs to, and then the merge of the two specified accounts is performed.  Returns false if the merge was canceled by the user.</summary>
		public static bool MergeTwoPatients(long patTo,long patFrom) {
			//No need to check RemotingRole; no call to db.
			if(patTo==patFrom) {
				//Do not merge the same patient onto itself.
				return true;
			}
			//We need to test patfields before doing anything else because the user may wish to cancel and abort the merge.
			PatField[] patToFields=PatFields.Refresh(patTo);
			PatField[] patFromFields=PatFields.Refresh(patFrom);
			List<PatField> patFieldsToDelete=new List<PatField>();
			List<PatField> patFieldsToUpdate=new List<PatField>();
			for(int i=0;i<patFromFields.Length;i++) {
				bool hasMatch=false;
				for(int j=0;j<patToFields.Length;j++) {
					//Check patient fields that are the same to see if they have different values.
					if(patFromFields[i].FieldName==patToFields[j].FieldName) {
						hasMatch=true;
						if(patFromFields[i].FieldValue!=patToFields[j].FieldValue) {
							//Get input from user on which value to use.
							DialogResult result=MessageBox.Show("The two patients being merged have different values set for the patient field:\r\n\""+patFromFields[i].FieldName+"\"\r\n\r\n"
								+"The merge into patient has the value: \""+patToFields[j].FieldValue+"\"\r\n"
								+"The merge from patient has the value: \""+patFromFields[i].FieldValue+"\"\r\n\r\n"
								+"Would you like to overwrite the merge into value with the merge from value?\r\n(Cancel will abort the merge)","Warning",MessageBoxButtons.YesNoCancel);
							if(result==DialogResult.Yes) {
								//User chose to use the merge from patient field info.
								patFromFields[i].PatNum=patTo;
								patFieldsToUpdate.Add(patFromFields[i]);
								patFieldsToDelete.Add(patToFields[j]);
							}
							else if(result==DialogResult.Cancel) {
								return false;//Completely cancels the entire merge.  No changes have been made at this point.
							}
						}
					}
				}
				if(!hasMatch) {//The patient field does not exist in the merge into account.
					patFromFields[i].PatNum=patTo;
					patFieldsToUpdate.Add(patFromFields[i]);
				}
			}
			bool isMergeSuccessful=false;
			int retryCount=5;
			while(--retryCount>=0 && !isMergeSuccessful) {
				try {
					isMergeSuccessful=MergeTwoPatientPointOfNoReturn(patTo,patFrom,patFieldsToDelete,patFieldsToUpdate);
				}
				catch(Exception ex) {
					if(retryCount<=0) {
						throw;//Throw exception after retrying 5 times.
					}
					ex.DoNothing();
				}
			}
			return isMergeSuccessful;
		}

		///<summary>Only call this method after all checks have been done to make sure the user wants these patients merged.</summary>
		public static bool MergeTwoPatientPointOfNoReturn(long patTo,long patFrom,List<PatField> patFieldsToDelete,List<PatField> patFieldsToUpdate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patTo,patFrom,patFieldsToDelete,patFieldsToUpdate);
			}
			string[] patNumForeignKeys=new string[]{
				"adjustment.PatNum",
				"allergy.PatNum",
				"anestheticrecord.PatNum",
				"anesthvsdata.PatNum",
				"appointment.PatNum",
				"asapcomm.PatNum",
				"claim.PatNum",
				"claimproc.PatNum",
				"clinicerx.PatNum",
				"commlog.PatNum",
				"confirmationrequest.PatNum",
				"creditcard.PatNum",
				"custrefentry.PatNumCust",
				"custrefentry.PatNumRef",
				//"custreference.PatNum",  //This is handled below.  We do not want to change patnum, the references form only shows entries for active patients.
				//"discountplansub.PatNum", //This is handled below. We want patients to keep their original discount plans if they have them, and only add under certain conditions.
				"disease.PatNum",
				//"document.PatNum",  //This is handled below when images are stored in the database and on the client side for images stored in the AtoZ folder due to the middle tier.
				"ehramendment.PatNum",
				"ehrcareplan.PatNum",
				"ehrlab.PatNum",
				"ehrmeasureevent.PatNum",
				"ehrnotperformed.PatNum",				
				//"ehrpatient.PatNum",  //This is handled below.  We do not want to change patnum here because there can only be one entry per patient.
				"ehrprovkey.PatNum",
				"ehrquarterlykey.PatNum",
				"ehrsummaryccd.PatNum",
				"emailmessage.PatNum",
				"emailmessage.PatNumSubj",
				"encounter.PatNum",
				"erxlog.PatNum",
				"etrans.PatNum",
				//"famaging.PatNum", //Taken care of down below as this should be the guarantor of the patient being merged into
				"familyhealth.PatNum",
				//formpat.FormPatNum IS NOT a PatNum so it is should not be merged.  It is the primary key.
				"formpat.PatNum",
				"guardian.PatNumChild",  //This may create duplicate entries for a single patient and guardian
				"guardian.PatNumGuardian",  //This may create duplicate entries for a single patient and guardian
				"histappointment.PatNum",
				"hl7msg.PatNum",
				"inssub.Subscriber",
				"installmentplan.PatNum",
				"intervention.PatNum",
				"labcase.PatNum",
				"labpanel.PatNum",
				"medicalorder.PatNum",
				//medicationpat.MedicationPatNum IS NOT a PatNum so it is should not be merged.  It is the primary key.
				"medicationpat.PatNum",
				"medlab.PatNum",
				"mount.PatNum",
				"orthocase.PatNum",
				"orthochart.PatNum",
				//"oidexternal.IDInternal",  //TODO:  Deal with these elegantly below, not always a patnum
				//"patfield.PatNum", //Taken care of below
				"patient.ResponsParty",
				//"patient.PatNum"  //We do not want to change patnum
				//"patient.Guarantor"  //This is taken care of below
				"patient.SuperFamily",  //The patfrom guarantor was changed, so this should be updated
				//"patientlink.PatNumFrom",//We want to leave the link history unchanged so that audit entries display correctly. If we start using this table for other types of linkage besides merges, then we might need to include this column.
				//"patientlink.PatNumTo",//^^Ditto
				//"patientnote.PatNum"	//The patientnote table is ignored because only one record can exist for each patient.  The record in 'patFrom' remains so it can be accessed again if needed.
				"patientportalinvite.PatNum",
				//"patientrace.PatNum", //The patientrace table is ignored because we don't want duplicate races.  We could merge them but we would have to add specific code to stop duplicate races being inserted.
				"patplan.PatNum",
				"payment.PatNum",
				"payortype.PatNum",
				"payplan.Guarantor",//Treated as a patnum, because it is actually a guarantor for the payment plan, and not a patient guarantor.
				"payplan.PatNum",				
				"payplancharge.Guarantor",//Treated as a patnum, because it is actually a guarantor for the payment plan, and not a patient guarantor.
				"payplancharge.PatNum",
				"paysplit.PatNum",
				"perioexam.PatNum",
				"phonenumber.PatNum",
				"plannedappt.PatNum",
				"popup.PatNum",
				"procedurelog.PatNum",
				"procnote.PatNum",
				"proctp.PatNum",
				"providererx.PatNum",  //For non-HQ this should always be 0.
				//question.FormPatNum IS NOT a PatNum so it is should not be merged.  It is a FKey to FormPat.FormPatNum
				"question.PatNum",
				//"recall.PatNum",  //We do not merge recall entries because it would cause duplicate recall entries.  Instead, update current recall entries.
				"recurringcharge.PatNum",
				"refattach.PatNum",
				//"referral.PatNum",  //This is synched with the new information below.
				"registrationkey.PatNum",
				"repeatcharge.PatNum",
				"reqstudent.PatNum",
				"reseller.PatNum",
				"rxpat.PatNum",
				"screenpat.PatNum",
				//screenpat.ScreenPatNum IS NOT a PatNum so it is should not be merged.  It is a primary key.
				//"securitylog.FKey",  //This would only matter when the FKey pointed to a PatNum.  Currently this is only for the PatientPortal permission
				//  which per Allen is not needed to be merged. 11/06/2015.
				//"securitylog.PatNum",//Changing the PatNum of a securitylog record will cause it to show a red (untrusted) in the audit trail.
				//  Best to preserve history in the securitylog and leave the corresponding PatNums static.
				"sheet.PatNum",
				"smsfrommobile.PatNum",
				"smstomobile.PatNum",
				"statement.PatNum",
				//task.KeyNum,  //Taken care of in a seperate step, because it is not always a patnum.
				//taskhist.KeyNum,  //Taken care of in a seperate step, because it is not always a patnum.
				"terminalactive.PatNum",
				"toothinitial.PatNum",
				"treatplan.PatNum",
				"treatplan.ResponsParty",
				//"tsitranslog.PatNum", //Taken care of down below as this should be the guarantor of the patient being merged into
				//vaccineobs.VaccinePatNum IS NOT a PatNum so it is should not be merged. It is the FK to the vaccinepat.VaccinePatNum.
				"vaccinepat.PatNum",
				//vaccinepat.VaccinePatNum IS NOT a PatNum so it is should not be merged. It is the primary key.
				"vitalsign.PatNum",
				"webschedrecall.PatNum",
				"xchargetransaction.PatNum",
				"xwebresponse.PatNum",
			};
			string command="";
			//Update and remove all patfields that were added to the list above.
			for(int i=0;i<patFieldsToDelete.Count;i++) {
				PatFields.Delete(patFieldsToDelete[i]);
			}
			for(int j=0;j<patFieldsToUpdate.Count;j++) {
				PatFields.Update(patFieldsToUpdate[j]);
			}
			Patient patientFrom=Patients.GetPat(patFrom);
			Patient patientTo=Patients.GetPat(patTo);
			//CustReference.  We need to combine patient from and patient into entries to have the into patient information from both.
			CustReference custRefFrom=CustReferences.GetOneByPatNum(patientFrom.PatNum);
			CustReference custRefTo=CustReferences.GetOneByPatNum(patientTo.PatNum);
			if(custRefFrom!=null && custRefTo!=null) { //If either of these are null, do nothing.  This is an internal only table so we didn't bother fixing it/warning users here.
				CustReference newCustRef=new CustReference();
				newCustRef.CustReferenceNum=custRefTo.CustReferenceNum; //Use same primary key so we can update.
				newCustRef.PatNum=patientTo.PatNum;
				if(custRefTo.DateMostRecent > custRefFrom.DateMostRecent) {
					newCustRef.DateMostRecent=custRefTo.DateMostRecent; //Use the most recent date.
				}
				else {
					newCustRef.DateMostRecent=custRefFrom.DateMostRecent; //Use the most recent date.
				}
				if(custRefTo.Note=="") {
					newCustRef.Note=custRefFrom.Note;
				}
				else if(custRefFrom.Note=="") {
					newCustRef.Note=custRefTo.Note;
				}
				else {//Both entries have a note
					newCustRef.Note=(custRefTo.Note+" | "+custRefFrom.Note); /*Combine in a | delimited string*/
				}
				newCustRef.IsBadRef=(custRefFrom.IsBadRef || custRefTo.IsBadRef);  //If either entry is a bad reference, count as a bad reference.
				CustReferences.Update(newCustRef); //Overwrites the old custRefTo entry.
			}
			//Merge ehrpatient.  We only do something here if there is a FROM patient entry and no INTO patient entry, in which case we change the patnum on the row to bring it over.
			EhrPatient ehrPatFrom=EhrPatients.GetOne(patientFrom.PatNum);
			EhrPatient ehrPatTo=EhrPatients.GetOne(patientTo.PatNum);
			if(ehrPatFrom!=null && ehrPatTo==null) {  //There is an entry for the FROM patient, but not the INTO patient.
				ehrPatFrom.PatNum=patientTo.PatNum;
				EhrPatients.Update(ehrPatFrom); //Bring the patfrom entry over to the new.
			}
			//Move the patient documents if they are stored in the database.
			//We do not have to worry about documents having the same name when storing within the database, only physical documents need to be renamed.
			//Physical documents are handled on the client side (not here) due to middle tier issues.
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Storing documents in the database.  Simply update the PatNum column accordingly. 
				//This query cannot be ran below where all the other tables are handled dyncamically because we do NOT want to update the PatNums in the case that documents are stored physically.
				command="UPDATE document "
					+"SET PatNum="+POut.Long(patTo)+" "
					+"WHERE PatNum="+POut.Long(patFrom);
				Db.NonQ(command);
			}
			//If the 'patFrom' had any ties to guardians, they should be deleted to prevent duplicate entries.
			command="DELETE FROM guardian"
				+" WHERE PatNumChild="+POut.Long(patFrom)
				+" OR PatNumGuardian="+POut.Long(patFrom);
			Db.NonQ(command);
			//Merge patient notes prior to updating the patient table, otherwise the wrong notes might bet set.
			PatientNotes.Merge(patientFrom,patientTo);
			//Update all guarantor foreign keys to change them from 'patFrom' to 
			//the guarantor of 'patTo'. This will effectively move all 'patFrom' family members 
			//to the family defined by 'patTo' in the case that 'patFrom' is a guarantor. If
			//the guarantor for 'patTo' is 'patFrom' then set the guarantor for the family instead to
			//'patTo'. If 'patFrom' is not a guarantor, then this command will have no effect and is
			//thus safe to always be run.
			long newGuarantor=patFrom==patientTo.Guarantor ? patientTo.PatNum : patientTo.Guarantor;
			List<Patient> listPatients=GetAllPatientsForGuarantor(patFrom);
			for(int i=0;i<listPatients.Count;i++) {
				Patient patientNew=listPatients[i];
				patientNew.Guarantor=newGuarantor; //Changing guarantor requires rehashing
				Update(patientNew,listPatients[i]);
			}
			//Update tables where the PatNum should be changed to the guarantor of the patient being merged into.
			//Only accomplishes anything if the patFrom is a guarantor. Otherwise, no effect.
			string[] listGuarantorToGuarantor={"famaging", "tsitranslog"};
			for(int i=0;i<listGuarantorToGuarantor.Length;i++) {
				command="UPDATE " +POut.String(listGuarantorToGuarantor[i]) + " "
					+"SET PatNum="+POut.Long(newGuarantor)+" "
					+"WHERE PatNum="+POut.Long(patFrom);
				Db.NonQ(command);
			}
			//At this point, the 'patFrom' is a regular patient and is absoloutely not a guarantor.
			//Now modify all PatNum foreign keys from 'patFrom' to 'patTo' to complete the majority of the
			//merge of the records between the two accounts.			
			for(int i=0;i<patNumForeignKeys.Length;i++) {
				if(DataConnection.DBtype==DatabaseType.Oracle 
					&& patNumForeignKeys[i]=="ehrlab.PatNum") //Oracle does not currently support EHR labs.
				{
					continue;
				}
				string[] tableAndKeyName=patNumForeignKeys[i].Split(new char[] {'.'});
				command="UPDATE "+tableAndKeyName[0]
					+" SET "+tableAndKeyName[1]+"="+POut.Long(patTo)
					+" WHERE "+tableAndKeyName[1]+"="+POut.Long(patFrom);
				Db.NonQ(command);
			}
			//Update the 'HasIns' column, and handle discount plans.
			//If the combined patient has insurance, we want to make sure we update 'HasIns' and 
			//remove any discount plan. If the combined patient has no insurance, only merge in 
			//the 'patFrom' discount plan if 'patTo' doesn't have a discount plan already.
			if(PatPlans.GetPatPlansForPat(patTo).Count>0) { //If the merged patient has insurance
				//Set HasIns to true
				command="UPDATE patient "
					+"SET HasIns='I' "
					+"WHERE PatNum="+POut.Long(patTo);
				Db.NonQ(command);
				//Remove discount plans if necessary
				if(DiscountPlanSubs.HasDiscountPlan(patTo)) {
					DiscountPlanSubs.DeleteForPatient(patTo);
				}
			}
			else if(!DiscountPlanSubs.HasDiscountPlan(patTo)) { //If patTo has no discount plan or insurance
				//Set the discount plan if there isn't one already
				command="UPDATE discountplansub "
					+"SET PatNum="+POut.Long(patTo)+" "
					+"WHERE PatNum="+POut.Long(patFrom);
				Db.NonQ(command);
			}
			//Clean up any remaining discount plans on the old patient.
			if(DiscountPlanSubs.HasDiscountPlan(patFrom)) {
				DiscountPlanSubs.DeleteForPatient(patFrom);
			}
			//We have to move over the tasks belonging to the 'patFrom' patient in a seperate step because
			//the KeyNum field of the task table might be a foreign key to something other than a patnum,
			//including possibly an appointment number.
			command="UPDATE task "
				+"SET KeyNum="+POut.Long(patTo)+" "
				+"WHERE KeyNum="+POut.Long(patFrom)+" AND ObjectType="+((int)TaskObjectType.Patient);
			Db.NonQ(command);
			//We have to move over the tasks belonging to the 'patFrom' patient in a seperate step because the KeyNum field of the taskhist table might be 
			//  a foreign key to something other than a patnum, including possibly an appointment number.
			command="UPDATE taskhist "
				+"SET KeyNum="+POut.Long(patTo)+" "
				+"WHERE KeyNum="+POut.Long(patFrom)+" AND ObjectType="+((int)TaskObjectType.Patient);
			Db.NonQ(command);
			//We have to move over the tasks belonging to the 'patFrom' patient in a seperate step because the IDInternal field of the oidexternal table 
			//  might be a foreign key to something other than a patnum depending on the IDType
			//There are 4 cases:
			//1) Neither patTo nor patFrom have used DoseSpot.  In this case, there is nothing to do.
			//2) Only patTo has used DoseSpot and patFrom has not.  Nothing to do.
			//3) Only patFrom has used DoseSpot and patTo has not.  Move the DoseSpot OID for patFrom to patTo, to preserve DoseSpot eRx history when clicking through.
			//4) Both patTo and patFrom have used DoseSpot.  Do nothing.  DoseSpot history for patFrom will be archived and no longer used.
			OIDExternal doseSpotRoot=DoseSpot.GetDoseSpotRootOid();
			bool hasPatToUsedDoseSpot=false;
			if(doseSpotRoot!=null) {
				OIDExternal oidPatTo=DoseSpot.GetDoseSpotPatID(patTo);
				hasPatToUsedDoseSpot=(oidPatTo!=null);
			}
			command="UPDATE oidexternal "
				+"SET IDInternal="+POut.Long(patTo)+" "
				+"WHERE IDInternal="+POut.Long(patFrom)+" AND IDType='"+(IdentifierType.Patient.ToString())+"' "
				+(hasPatToUsedDoseSpot?"AND rootExternal!='"+DoseSpot.GetDoseSpotRoot()+"."+POut.Int((int)IdentifierType.Patient)+"'":"");
			Db.NonQ(command);
			//Mark the patient where data was pulled from as archived unless the patient is already marked as deceased.
			//We need to have the patient marked either archived or deceased so that it is hidden by default, and
			//we also need the customer to be able to access the account again in case a particular table gets missed
			//in the merge tool after an update to Open Dental. This will allow our customers to remerge the missing
			//data after a bug fix is released. 
			command="UPDATE patient "
				+"SET PatStatus="+((int)PatientStatus.Archived)+" "
				+"WHERE PatNum="+POut.Long(patFrom)+" "
				+"AND PatStatus!="+((int)PatientStatus.Deceased);
			Db.NonQ(command);
			//Set remove PatFrom from the superfamily if they currently belong in one by setting patient.SuperFamily to 0.
			if(patientFrom.SuperFamily!=0) {
				command="UPDATE patient SET patient.SuperFamily=0 WHERE patient.PatNum="+POut.Long(patFrom)+";";
				Db.NonQ(command);
            }
			//Update patplan.Ordinal in case multiple patplans wound up with the same Ordinal
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(patTo).OrderBy(x => x.Ordinal).ToList();
			PatPlan patPlanPrimary=listPatPlans.FirstOrDefault();
			if(patPlanPrimary != null) {
				PatPlans.SetOrdinal(patPlanPrimary.PatPlanNum,patPlanPrimary.Ordinal);//Will reset all other Ordinals to consecutive values.
			}
			//This updates the referrals with the new patient information from the merge.
			Referral referral=Referrals.GetFirstOrDefault(x => x.PatNum==patFrom);
			if(referral!=null) {
				referral.PatNum=patientTo.PatNum;
				referral.LName=patientTo.LName;
				referral.FName=patientTo.FName;
				referral.MName=patientTo.MiddleI;
				referral.Address=patientTo.Address;
				referral.Address2=patientTo.Address2;
				referral.City=patientTo.City;
				referral.ST=patientTo.State;
				referral.SSN=patientTo.SSN;
				referral.Zip=patientTo.Zip;
				referral.Telephone=TelephoneNumbers.FormatNumbersExactTen(patientTo.HmPhone);
				referral.EMail=patientTo.Email;
				Referrals.Update(referral);
				Referrals.RefreshCache();
			}
			Recalls.Synch(patTo);  //Update patient's recalls now that merge is completed.
			if(PrefC.IsODHQ) {
				//Merge webchats.  The webchat database is HQ only.
				WebChatMisc.DbAction(delegate() {
					command=@"UPDATE webchatsession
									SET webchatsession.PatNum=("+POut.Long(patTo)+@") 
									WHERE webchatsession.PatNum=("+POut.Long(patFrom)+")";
					Db.NonQ(command);
				});
			}
			//Create a link from the from patient to the to patient.
			PatientLink patLink=new PatientLink();
			patLink.PatNumFrom=patFrom;
			patLink.PatNumTo=patTo;
			patLink.LinkType=PatientLinkType.Merge;
			PatientLinks.Insert(patLink);
			return true;
		}

		///<summary>LName, 'Preferred' FName M</summary>
		public static string GetNameLF(string LName,string FName,string Preferred,string MiddleI) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			retVal+=LName;
			if(FName!="" || MiddleI!="" || Preferred!="") {
				retVal+=",";
			}
			if(Preferred!="") {
				retVal+=" '"+Preferred+"'";
			}
			if(FName!="") {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=FName;
			}
			if(MiddleI!="") {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=MiddleI;
			}
			return retVal;
		}

		///<summary>LName, 'Preferred' FName M for the patnum passed in.  Uses the database.</summary>
		public static string GetNameLF(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),patNum);
			}
			Patient pat=Patients.GetLim(patNum);
			return GetNameLF(pat);
		}

		///<summary>Does not call DB to retrieve a patient, only uses the passed in object.</summary>
		public static string GetNameLF(Patient pat) {
			string retVal="";
			retVal+=pat.LName;
			if(pat.FName!="" || pat.MiddleI!="" || pat.Preferred!="") {
				retVal+=",";
			}
			if(pat.Preferred!="") {
				retVal+=" '"+pat.Preferred+"'";
			}
			if(pat.FName!="") {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=pat.FName;
			}
			if(pat.MiddleI!="") {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=pat.MiddleI;
			}
			return retVal;
		}

		///<summary>LName, FName M</summary>
		public static string GetNameLFnoPref(string LName,string FName,string MiddleI) {
			return GetNameLF(LName,FName,"",MiddleI);
		}

		///<summary>FName 'Preferred' M LName. Returns empty string if patnum is 0 or if patient can't be found.</summary>
		public static string GetNameFL(long patNum) {
			if(patNum==0) {
				return "";
			}
			Patient pat=GetLim(patNum);
			if(pat==null) {
				return "";
			}
			return GetNameFL(pat.LName,pat.FName,pat.Preferred,pat.MiddleI);
		}

		///<summary>FName 'Preferred' M LName</summary>
		public static string GetNameFL(string LName,string FName,string Preferred,string MiddleI) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			if(FName!="") {
				retVal+=FName;
			}
			if(!string.IsNullOrWhiteSpace(Preferred)) {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+="'"+Preferred+"'";
			}
			if(!string.IsNullOrWhiteSpace(MiddleI)) {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=MiddleI;
			}
			retVal=AddSpaceIfNeeded(retVal);
			retVal+=LName;
			return retVal;
		}

		///<summary>FName M LName</summary>
		public static string GetNameFLnoPref(string LName,string FName,string MiddleI) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			retVal+=FName;
			if(!string.IsNullOrWhiteSpace(MiddleI)) {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=MiddleI;
			}
			retVal=AddSpaceIfNeeded(retVal);
			retVal+=LName;
			return retVal;
		}

		///<summary>FName/Preferred LName</summary>
		public static string GetNameFirstOrPrefL(string LName,string FName,string Preferred) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			if(Preferred=="") {
				retVal+=FName;
			}
			else {
				retVal+=Preferred;
			}
			retVal=AddSpaceIfNeeded(retVal);
			retVal+=LName;
			return retVal;
		}

		///<summary>FName/Preferred M. LName</summary>
		public static string GetNameFirstOrPrefML(string LName,string FName,string Preferred,string MiddleI) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			if(Preferred=="") {
				retVal+=FName;
			}
			else {
				retVal+=Preferred; ;
			}
			if(!string.IsNullOrWhiteSpace(MiddleI)) {
				retVal=AddSpaceIfNeeded(retVal);
				retVal+=MiddleI+".";
			}
			retVal=AddSpaceIfNeeded(retVal);
			retVal+=LName;
			return retVal;
		}

		///<summary>Title FName M LName</summary>
		public static string GetNameFLFormal(string LName,string FName,string MiddleI,string Title) {
			//No need to check RemotingRole; no call to db.
			return string.Join(" ",new[] {Title,FName,MiddleI,LName}.Where(x => !string.IsNullOrEmpty(x)));//returns "" if all strings are null or empty.
		}

		///<summary>Includes preferred.</summary>
		public static string GetNameFirst(string FName,string Preferred) {
			//No need to check RemotingRole; no call to db.
			string retVal=FName;
			if(Preferred!="") {
				retVal+=" '"+Preferred+"'";
			}
			return retVal;
		}

		///<summary>Returns preferred name if one exists, otherwise returns first name.</summary>
		public static string GetNameFirstOrPreferred(string nameFirst,string namePreferred) {
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrWhiteSpace(namePreferred)) {
				return nameFirst??"";
			}
			return namePreferred;
		}

		///<summary>Returns first name if one exists or returns preferred name,otherwise returns last name.</summary>
		public static string GetNameFirstOrPreferredOrLast(string FName,string Preferred,string LName) {
			//No need to check RemotingRole; no call to db.
			if(FName!="") {
				return FName;
			}
			if(Preferred !="") {
				return Preferred;
			}
			return LName;
		}

		///<summary>Adds a space if the passed in string is not empty.  Used for name functions to add a space only when needed.</summary>
		private static string AddSpaceIfNeeded(string name) {
			if(name!="") {
				return name+" ";
			}
			return name;
		}

		///<summary>Dear __.  Does not include the "Dear" or the comma.</summary>
		public static string GetSalutation(string Salutation,string Preferred,string FName) {
			//No need to check RemotingRole; no call to db.
			if(Salutation!="") {
				return Salutation;
			}
			if(Preferred!="") {
				return Preferred;
			}
			return FName;
		}

		/// <summary>Result will be multiline.</summary>
		public static string GetAddressFull(string address,string address2,string city,string state,string zip) {
			//No need to check RemotingRole; no call to db.
			string retVal=address;
			if(address2!="") {
				retVal+="\r\n"+address2;
			}
			retVal+="\r\n"+city+", "+state+" "+zip;
			return retVal;
		}

		/// <summary>Change preferred provider for all patients with provNumFrom to provNumTo.</summary>
		public static void ChangePrimaryProviders(long provNumFrom,long provNumTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),provNumFrom,provNumTo);
				return;
			}
			string command="UPDATE patient SET PriProv="+POut.Long(provNumTo)+" WHERE PriProv="+POut.Long(provNumFrom);
			Db.NonQ(command);
		}

		///<summary>Change secondary provider for all patients with provNumFrom to provNumTo.</summary>
		public static void ChangeSecondaryProviders(long provNumFrom,long provNumTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),provNumFrom,provNumTo);
				return;
			}
			string command="UPDATE patient " 
				+"SET SecProv = '"+provNumTo+"' "
				+"WHERE SecProv = '"+provNumFrom+"'";
			Db.NonQ(command);
		}
		
		/// <summary>Gets all patients whose primary provider PriProv is in the list provNums.</summary>
		public static DataTable GetPatNumsByPriProvs(List<long> listProvNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listProvNums);
			}
			if(listProvNums==null || listProvNums.Count==0) {
				return new DataTable();
			}
			string command="SELECT PatNum,PriProv FROM patient WHERE PriProv IN ("+string.Join(",",listProvNums)+")";
			return Db.GetTable(command);
		}

		///<summary>Gets the PatNum for all patients belonging to a specific clinic. </summary>
		public static List<long> GetPatNumsByClinic(long clinicNum,bool getAllStatuses=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),clinicNum,getAllStatuses);
			}
			string command="SELECT PatNum FROM patient WHERE ClinicNum="+POut.Long(clinicNum);
			if(!getAllStatuses) {
				command+=" AND PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Archived)+","
					+POut.Int((int)PatientStatus.Deceased)+","+POut.Int((int)PatientStatus.NonPatient)+") ";
			}
			return Db.GetListLong(command);
		}

		///<summary>Gets the PatNum and ClinicNum for all patients whose ClinicNum is in listClinicNums.</summary>
		public static DataTable GetPatNumsByClinic(List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			if(listClinicNums==null || listClinicNums.Count==0) {
				return new DataTable();
			}
			string command="SELECT PatNum,ClinicNum FROM patient WHERE ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			return Db.GetTable(command);
		}
		
		/// <summary>Change clinic for all patients with clinicNumFrom to clinicNumTo.</summary>
		public static void ChangeClinicsForAll(long clinicNumFrom,long clinicNumTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicNumFrom,clinicNumTo);
				return;
			}
			string command="UPDATE patient SET ClinicNum="+POut.Long(clinicNumTo)+" WHERE ClinicNum="+POut.Long(clinicNumFrom);
			Db.NonQ(command);
		}
		/// <summary>Find the most used provider for a single patient. Bias towards the most recently used provider if they have done an equal number of procedures.</summary>
		public static long ReassignProvGetMostUsed(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT ProvNum,MAX(ProcDate) MaxProcDate,COUNT(ProvNum) ProcCount "
				+"FROM procedurelog "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"GROUP BY ProvNum";
			DataTable table=Db.GetTable(command);
			long newProv=0;
			int mostVisits=0;
			DateTime maxProcDate=new DateTime();
			for(int i=0;i<table.Rows.Count;i++) {//loop through providers
				if(PIn.Int(table.Rows[i]["ProcCount"].ToString())>mostVisits) {//New leader for most visits.
					mostVisits=PIn.Int(table.Rows[i]["ProcCount"].ToString());
					maxProcDate=PIn.DateT(table.Rows[i]["MaxProcDate"].ToString());
					newProv=PIn.Long(table.Rows[i]["ProvNum"].ToString());
				}
				else if(PIn.Int(table.Rows[i]["ProcCount"].ToString())==mostVisits) {//Tie for most visits, use MaxProcDate as a tie breaker.
					if(PIn.DateT(table.Rows[i]["MaxProcDate"].ToString())>maxProcDate) {
						//mostVisits same as before
						maxProcDate=PIn.DateT(table.Rows[i]["MaxProcDate"].ToString());
						newProv=PIn.Long(table.Rows[i]["ProvNum"].ToString());
					}
				}
			}
			return newProv;
		}

		/// <summary>Change preferred provider PriProv to provNum for patient with PatNum=patNum.</summary>
		public static void ReassignProv(long provNum,List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),provNum,listPatNums);
				return;
			}
			if(listPatNums==null || listPatNums.Count==0) {
				return;
			}
			string command="UPDATE patient SET PriProv="+POut.Long(provNum)+" WHERE PatNum IN ("+string.Join(",",listPatNums)+")";
			Db.NonQ(command);
		}

		///<summary>Gets the number of patients with unknown Zip.</summary>
		public static int GetZipUnknown(DateTime dateFrom, DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command="SELECT COUNT(*) "
				+"FROM patient "
				+"WHERE "+DbHelper.Regexp("Zip","^[0-9]{5}",false)+" "//Does not start with five numbers
				+"AND PatNum IN ( "
					+"SELECT DISTINCT PatNum FROM procedurelog "
					+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
					+"AND DateEntryC >= "+POut.Date(dateFrom)+" "
					+"AND DateEntryC <= "+POut.Date(dateTo)+") "
				+"AND Birthdate<=CURDATE() "//Birthday not in the future (at least 0 years old)
				+"AND Birthdate>SUBDATE(CURDATE(),INTERVAL 200 YEAR) ";//Younger than 200 years old
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Gets the number of qualified patients (having a completed procedure within the given time frame) in zip codes with less than 9 other qualified patients in that same zip code.</summary>
		public static int GetZipOther(DateTime dateFrom, DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command="SELECT SUM(Patients) FROM "
				+"(SELECT SUBSTR(Zip,1,5) Zip_Code,COUNT(*) Patients "//Column headings Zip_Code and Patients are provided by the USD 2010 Manual.
				+"FROM patient "
				+"WHERE "+DbHelper.Regexp("Zip","^[0-9]{5}")+" "//Starts with five numbers
				+"AND PatNum IN ( "
					+"SELECT DISTINCT PatNum FROM procedurelog "
					+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
					+"AND DateEntryC >= "+POut.Date(dateFrom)+" "
					+"AND DateEntryC <= "+POut.Date(dateTo)+") "
				+"AND Birthdate<=CURDATE() "//Birthday not in the future (at least 0 years old)
				+"AND Birthdate>SUBDATE(CURDATE(),INTERVAL 200 YEAR) "//Younger than 200 years old
				+"GROUP BY Zip "
				+"HAVING COUNT(*) < 10) patzip";//Has less than 10 patients in that zip code for the given time frame.
			return PIn.Int(Db.GetCount(command));
		}
		
		///<summary>Gets the total number of patients with completed procedures between dateFrom and dateTo. Also checks for age between 0 and 200.</summary>
		public static int GetPatCount(DateTime dateFrom, DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command="SELECT COUNT(*) "
				+"FROM patient "
				+"WHERE PatNum IN ( "
					+"SELECT DISTINCT PatNum FROM procedurelog "
					+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
					+"AND DateEntryC >= "+POut.Date(dateFrom)+" "
					+"AND DateEntryC <= "+POut.Date(dateTo)+") "
				+"AND Birthdate<=CURDATE() "//Birthday not in the future (at least 0 years old)
				+"AND Birthdate>SUBDATE(CURDATE(),INTERVAL 200 YEAR) ";//Younger than 200 years old
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Counts all patients that are not deleted.</summary>
		public static int GetPatCountAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM patient WHERE PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			return PIn.Int(Db.GetCount(command));
		}


		///<summary>Gets the total number of patients with completed procedures between dateFrom and dateTo who are at least agelow and strictly younger than agehigh.</summary>
		public static int GetAgeGenderCount(int agelow,int agehigh,PatientGender gender,DateTime dateFrom, DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),agelow,agehigh,gender,dateFrom,dateTo);
			}
			bool male=true;//Since all the numbers must add up to equal, we count unknown and other genders as female.
			if(gender!=0) {
				male=false;
			}
			string command="SELECT COUNT(*) "
				+"FROM patient pat "
				+"WHERE PatNum IN ( "
					+"SELECT DISTINCT PatNum FROM procedurelog "
					+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
					+"AND DateEntryC >= "+POut.Date(dateFrom)+" "
					+"AND DateEntryC <= "+POut.Date(dateTo)+") "
				+"AND Gender"+(male?"=0":"!=0")+" "
				+"AND Birthdate<=SUBDATE(CURDATE(),INTERVAL "+agelow+" YEAR) "//Born before this date
				+"AND Birthdate>SUBDATE(CURDATE(),INTERVAL "+agehigh+" YEAR)";//Born after this date
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Gets completed procedures, adjustments, and pay plan charges for a superfamily, ordered by datetime.</summary>
		public static DataTable GetSuperFamProcAdjustsPPCharges(long superFamily) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),superFamily);
			}
			List<Patient> listPatients=Patients.GetBySuperFamily(superFamily);
			List<long> listPatNums=listPatients.Where(x =>
					(x.PatNum==x.Guarantor && x.HasSuperBilling)
					|| (x.PatNum!=x.Guarantor && listPatients.Exists(y => y.PatNum==x.Guarantor && y.HasSuperBilling)))
				.Select(x => x.PatNum).ToList();
			string command="SELECT * FROM ("
				+"SELECT procedurelog.ProcNum AS 'PriKey', procedurelog.ProcDate AS 'Date', procedurelog.PatNum AS 'PatNum', procedurelog.ProvNum AS 'Prov' "
					+",procedurelog.ProcFee AS 'Amount', procedurelog.CodeNum AS 'Code', procedurelog.ToothNum AS 'Tooth', '' AS 'AdjType', '' AS 'ChargeType'"
					+", "+DbHelper.Concat("patient.LName","', '","patient.FName")+" AS 'PatName'"
				+"FROM procedurelog "
				+"INNER JOIN patient ON procedurelog.PatNum=patient.PatNum "
				+"WHERE procedurelog.PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND StatementNum=0 "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
			+"UNION ALL "
				+"SELECT adjustment.AdjNum AS 'PriKey', adjustment.AdjDate AS 'Date', adjustment.PatNum AS 'PatNum', adjustment.ProvNum AS 'Prov'"
					+", adjustment.AdjAmt AS 'Amount', '' AS 'Code', '' AS 'Tooth', adjustment.AdjType AS 'AdjType', '' AS 'ChargeType'"
					+", "+DbHelper.Concat("patient.LName","', '","patient.FName")+" AS 'PatName'"
				+"FROM adjustment "
				+"INNER JOIN patient ON adjustment.PatNum=patient.PatNum "
				+"WHERE adjustment.PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND StatementNum=0 "
			+"UNION ALL "
				+"SELECT payplancharge.PayPlanChargeNum AS 'PriKey', payplancharge.ChargeDate AS 'Date', payplancharge.PatNum AS 'PatNum'"
					+",payplancharge.ProvNum AS 'Prov', payplancharge.Principal+payplancharge.Interest AS 'Amount', '' AS 'Code', '' AS 'Tooth'"
					+",'' AS 'AdjType', payplancharge.ChargeType AS 'ChargeType',"+DbHelper.Concat("patient.LName","', '","patient.FName")+" AS 'PatName'"
				+"FROM payplancharge "
				+"INNER JOIN patient ON payplancharge.PatNum=patient.PatNum "
				+"WHERE payplancharge.PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
				+"AND StatementNum=0 "
				+"AND "+POut.Bool(PrefC.GetInt(PrefName.PayPlansVersion)==(int)PayPlanVersions.AgeCreditsAndDebits)+" "
				+"AND payplancharge.ChargeDate<"+DbHelper.DateAddMonth(DbHelper.Now(),"3")+" "//Only show payplan charges less than 3 mos into the future
			+") procadj ORDER BY procadj.Date DESC";
			return Db.GetTable(command);
		}

		///<summary>Returns a list of patients belonging to the SuperFamily</summary>
		public static List<Patient> GetBySuperFamily(long SuperFamilyNum) {
			if(SuperFamilyNum==0) {
				return new List<Patient>();//return empty list
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),SuperFamilyNum);
			}
			string command="SELECT * FROM patient WHERE SuperFamily="+POut.Long(SuperFamilyNum)
				+" AND patient.PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Returns a list of patients that are the guarantors for the patients in the Super Family</summary>
		public static List<Patient> GetSuperFamilyGuarantors(long SuperFamilyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),SuperFamilyNum);
			}
			if(SuperFamilyNum==0) {
				return new List<Patient>();//return empty list
			}
			//Should also work in Oracle.
			//this query was taking 2.5 seconds on a large database
			//string command = "SELECT DISTINCT * FROM patient WHERE PatNum IN (SELECT Guarantor FROM patient WHERE SuperFamily="+POut.Long(SuperFamilyNum)+") "
			//	+"AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			//optimized to 0.001 second runtime on same db
			string command = "SELECT DISTINCT * FROM patient WHERE SuperFamily="+POut.Long(SuperFamilyNum)
				+" AND PatStatus!="+POut.Int((int)PatientStatus.Deleted)+" AND PatNum=Guarantor";
			return Crud.PatientCrud.TableToList(Db.GetTable(command));
		}

		public static void AssignToSuperfamily(long guarantor,long superFamilyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),guarantor,superFamilyNum);
				return;
			}
			string command="UPDATE patient SET SuperFamily="+POut.Long(superFamilyNum)+", HasSuperBilling=1 WHERE Guarantor="+POut.Long(guarantor);
			Db.NonQ(command);
		}

		public static void MoveSuperFamily(long oldSuperFamilyNum,long newSuperFamilyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),oldSuperFamilyNum,newSuperFamilyNum);
				return;
			}
			if(oldSuperFamilyNum==0) {
				return;
			}
			string command="UPDATE patient SET SuperFamily="+newSuperFamilyNum+" WHERE SuperFamily="+oldSuperFamilyNum;
			Db.NonQ(command);
		}

		public static void DisbandSuperFamily(long SuperFamilyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),SuperFamilyNum);
				return;
			}
			if(SuperFamilyNum==0) {
				return;
			}
			string command = "UPDATE patient SET SuperFamily=0 WHERE SuperFamily="+POut.Long(SuperFamilyNum);
			Db.NonQ(command);
		}

		public static List<Patient> GetPatsForScreenGroup(long screenGroupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),screenGroupNum);
			}
			if(screenGroupNum==0) {
				return new List<Patient>();
			}
			string command = "SELECT * FROM patient WHERE PatNum IN (SELECT PatNum FROM screenpat WHERE ScreenGroupNum="+POut.Long(screenGroupNum)+")";
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Get a list of patients for FormEhrPatientExport. If provNum, clinicNum, or siteNum are =0 get all.</summary>
		public static DataTable GetExportList(long patNum, string firstName,string lastName,long provNum,long clinicNum,long siteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,firstName,lastName,provNum,clinicNum,siteNum);
			}
			string command = "SELECT patient.PatNum, patient.FName, patient.LName, provider.Abbr AS Provider, clinic.Description AS Clinic, site.Description AS Site "
				+"FROM patient "
				+"INNER JOIN provider ON patient.PriProv=provider.ProvNum "
				+"LEFT JOIN clinic ON patient.ClinicNum=clinic.ClinicNum "
				+"LEFT JOIN site ON patient.SiteNum=site.SiteNum "
				+"WHERE patient.PatStatus=0 ";
			if(patNum != 0) {
				command+="AND patient.PatNum LIKE '%"+POut.Long(patNum)+"%' ";
			}
			if(firstName != "") {
				command+="AND patient.FName LIKE '%"+POut.String(firstName)+"%' ";
			}
			if(lastName != "") {
				command+="AND patient.LName LIKE '%"+POut.String(lastName)+"%' ";
			}
			if(provNum>0) {
				command+="AND provider.ProvNum = "+POut.Long(provNum)+" ";
			}
			if(clinicNum>0) {
				command+="AND clinic.ClinicNum = "+POut.Long(clinicNum)+" ";
			}
			if(siteNum>0) {
				command+="AND site.SiteNum = "+POut.Long(siteNum)+" ";
			}
			command+="ORDER BY patient.LName,patient.FName ";
			return (Db.GetTable(command));
		}

		///<summary>Returns a list of Patients of which this PatNum is eligible to view given PHI constraints.</summary>
		public static List<Patient> GetPatientsForPhi(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<long> listPatNums=GetPatNumsForPhi(patNum);
			//If there are duplicates in listPatNums, then they will be removed because of the IN statement in the query below.
			string command=$@"SELECT * 
				FROM patient
				WHERE PatStatus IN ({POut.Enum(PatientStatus.Patient)},{POut.Enum(PatientStatus.NonPatient)},{POut.Enum(PatientStatus.Inactive)})
				AND PatNum IN ({string.Join(",",listPatNums)})";
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Returns a list of PatNum(s) of which this PatNum is eligible to view given PHI constraints.  Used internally and also used by Patient Portal.</summary>
		public static List<long> GetPatNumsForPhi(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patNum);
			string command="";
			if(PrefC.GetBool(PrefName.FamPhiAccess)) { //Include guarantor's family if pref is set.
				//Include any patient where this PatNum is the Guarantor.
				command="SELECT PatNum FROM patient WHERE Guarantor = "+POut.Long(patNum);
				DataTable tablePatientsG=Db.GetTable(command);
				for(int i=0;i<tablePatientsG.Rows.Count;i++) {
					listPatNums.Add(PIn.Long(tablePatientsG.Rows[i]["PatNum"].ToString()));
				}
			}
			//Include any patient where the given patient is the responsible party.
			command="SELECT PatNum FROM patient WHERE ResponsParty = "+POut.Long(patNum);
			DataTable tablePatientsR=Db.GetTable(command);
			for(int i=0;i<tablePatientsR.Rows.Count;i++) {
				listPatNums.Add(PIn.Long(tablePatientsR.Rows[i]["PatNum"].ToString()));
			}
			//Include any patient where this patient is the guardian.
			command="SELECT PatNum FROM patient "
				+"WHERE PatNum IN (SELECT guardian.PatNumChild FROM guardian WHERE guardian.IsGuardian = 1 AND guardian.PatNumGuardian="+POut.Long(patNum)+") ";
			DataTable tablePatientsD=Db.GetTable(command);
			for(int i=0;i<tablePatientsD.Rows.Count;i++) {
				listPatNums.Add(PIn.Long(tablePatientsD.Rows[i]["PatNum"].ToString()));
			}
			return listPatNums.Distinct().ToList();
		}

		///<summary>Validate password against strong password rules. Currently only used for patient portal passwords. Requirements: 8 characters, 1 uppercase character, 1 lowercase character, 1 number. Returns non-empty string if validation failed. Return string will be translated.</summary>
		public static string IsPortalPasswordValid(string newPassword) {
			//No need to check RemotingRole; no call to db.
			if(newPassword.Length<8) {
				return Lans.g("FormPatientPortal","Password must be at least 8 characters long.");
			}
			if(!Regex.IsMatch(newPassword,"[A-Z]+")) {
				return Lans.g("FormPatientPortal","Password must contain an uppercase letter.");
			}
			if(!Regex.IsMatch(newPassword,"[a-z]+")) {
				return Lans.g("FormPatientPortal","Password must contain an lowercase letter.");
			}
			if(!Regex.IsMatch(newPassword,"[0-9]+")) {
				return Lans.g("FormPatientPortal","Password must contain a number.");
			}
			return "";
		}

		///<summary>Returns a distinct list of PatNums for guarantors that have any family member with passed in clinics, or have had work done at passed in clinics.</summary>
		public static string GetClinicGuarantors(string clinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),clinicNums);
			}
			string clinicGuarantors="";
			//Get guarantor of patients with clinic from comma delimited list
			string command="SELECT DISTINCT Guarantor FROM patient WHERE ClinicNum IN ("+clinicNums+")";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				if(i>0 || clinicGuarantors!="") {
					clinicGuarantors+=",";
				}
				clinicGuarantors+=PIn.String(table.Rows[i]["Guarantor"].ToString());
			}
			//Get guarantor of patients who have had work done at clinic in comma delimited list
			command="SELECT DISTINCT Guarantor "
				+"FROM procedurelog "
				+"INNER JOIN patient ON patient.PatNum=procedurelog.PatNum "
					+"AND patient.PatStatus !=4 "
				+"WHERE procedurelog.ProcStatus IN (1,2) "
				+"AND procedurelog.ClinicNum IN ("+clinicNums+")";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				if(i>0 || clinicGuarantors!="") {
					clinicGuarantors+=",";
				}
				clinicGuarantors+=PIn.String(table.Rows[i]["Guarantor"].ToString());
			}
			return clinicGuarantors;
		}

		public static List<Patient> GetPatsByEmailAddress(string emailAddress) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),emailAddress);
			}
			string command="SELECT * FROM patient WHERE Email LIKE '%"+POut.String(emailAddress)+"%'";
			return Crud.PatientCrud.SelectMany(command);
		}

		///<summary>Returns all PatNums for whom the specified PatNum is the Guarantor. If this patient is not a guarantor, returns an empty list. If the
		///patient is a guarantor, this patient's PatNum will be included in the list.</summary>
		public static List<long> GetDependents(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT PatNum FROM patient WHERE Guarantor="+POut.Long(patNum);
			return Db.GetListLong(command);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching patNum as FKey and are related to Patient.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Patient table type.</summary>
		public static void ClearFkey(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Crud.PatientCrud.ClearFkey(patNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching patNums as FKey and are related to Patient.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Patient table type.</summary>
		public static void ClearFkey(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPatNums);
				return;
			}
			Crud.PatientCrud.ClearFkey(listPatNums);
		}

		///<summary>List of all patients in the current family along with any patients associated to payment plans of which a member of this family is the guarantor.
		///Only gets patients that are associated to active plans.</summary>
		public static List<Patient> GetAssociatedPatients(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),patNum);
			}
			//patients associated to payment plans of which any member of this family is the guarantor UNION patients in the family
			string command="SELECT pplans.PatNum,pplans.LName,pplans.FName,pplans.MiddleI,pplans.Preferred,pplans.CreditType,pplans.Guarantor,pplans.HasIns,pplans.SSN "
				+"FROM patient pat "
				+"LEFT JOIN patient fam ON fam.Guarantor = pat.Guarantor "
				+"LEFT JOIN payplan ON payplan.Guarantor = fam.PatNum "
				+"LEFT JOIN patient pplans ON pplans.PatNum = payplan.PatNum "
				+"WHERE pat.PatNum = " +POut.Long(patNum)+" "
				+"AND payplan.IsClosed = 0 "
				+"GROUP BY pplans.PatNum,pplans.LName,pplans.FName,pplans.MiddleI,pplans.Preferred,pplans.CreditType,pplans.Guarantor,pplans.HasIns,pplans.SSN ";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return new List<Patient>();
			}
			List<Patient> listPatLims=new List<Patient>();
			for(int i = 0;i < table.Rows.Count;i++) {
				Patient Lim=new Patient();
				Lim.PatNum     = PIn.Long(table.Rows[i]["PatNum"].ToString());
				Lim.LName      = PIn.String(table.Rows[i]["LName"].ToString());
				Lim.FName      = PIn.String(table.Rows[i]["FName"].ToString());
				Lim.MiddleI    = PIn.String(table.Rows[i]["MiddleI"].ToString());
				Lim.Preferred  = PIn.String(table.Rows[i]["Preferred"].ToString());
				Lim.CreditType = PIn.String(table.Rows[i]["CreditType"].ToString());
				Lim.Guarantor  = PIn.Long(table.Rows[i]["Guarantor"].ToString());
				Lim.HasIns     = PIn.String(table.Rows[i]["HasIns"].ToString());
				Lim.SSN        = PIn.String(table.Rows[i]["SSN"].ToString());
				listPatLims.Add(Lim);
			}
			return listPatLims;
		}

		public static List<PatComm> GetPatComms(List<long> patNums,Clinic clinic,bool isGetFamily=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatComm>>(MethodBase.GetCurrentMethod(),patNums,clinic,isGetFamily);
			}
			List<PatComm> retVal=new List<PatComm>();
			if(patNums.Count<=0) {
				return retVal;
			}
			string command;
			List<long> patNumsSearch=new List<long>(patNums);
			if(isGetFamily) {
				command="SELECT Guarantor FROM patient WHERE PatNum IN ("+string.Join(",",patNumsSearch.Distinct())+")";
				patNumsSearch=patNumsSearch.Union(Db.GetListLong(command)).ToList();//combines and removes duplicates.
			}
			command="SELECT PatNum, PatStatus, PreferConfirmMethod, PreferContactMethod, PreferRecallMethod, PreferContactConfidential, "
				+"TxtMsgOk,HmPhone,WkPhone,WirelessPhone,Email,FName,LName,Preferred,Guarantor,Language,Birthdate,Premed,";
			if(clinic!=null) {
				command+=clinic.ClinicNum;
			}
			command+=" ClinicNum FROM patient WHERE PatNum IN ("+string.Join(",",patNumsSearch.Distinct())+") ";
			bool isUnknownNo=PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo);
			List<Clinic> listAllClinics;
			if(clinic==null) {
				listAllClinics=Clinics.GetDeepCopy().Concat(new[] { Clinics.GetPracticeAsClinicZero() }).ToList();
			}
			else {
				listAllClinics=new List<Clinic> { clinic };
			}
			Dictionary<string,bool> dictEmailValidForClinics=listAllClinics
				.ToDictionary(x => x.ClinicNum.ToString(),x => EmailAddresses.GetFirstOrDefault(y => y.EmailAddressNum==x.EmailAddressNum)!=null);
			Dictionary<string,bool> dictTextingEnabledForClinics=listAllClinics.ToDictionary(x => x.ClinicNum.ToString(),x => Clinics.IsTextingEnabled(x.ClinicNum));
			string curCulture=StringTools.TruncateBeginning(System.Globalization.CultureInfo.CurrentCulture.Name,2);
			Dictionary<string,string> dictClinicCountryCodes=listAllClinics.ToDictionary(x => x.ClinicNum.ToString(),x => SmsPhones.GetFirstOrDefault(y => y.ClinicNum==x.ClinicNum)?.CountryCode??"");
			bool isEmailValidForClinic;
			bool isTextingEnabledForClinic;
			string clinicCountryCode;
			List<PatComm> listPatComms=Db.GetTable(command).Select()
				.Select(x => new PatComm(
					x,
					(dictEmailValidForClinics.TryGetValue(x["ClinicNum"].ToString(),out isEmailValidForClinic)?isEmailValidForClinic:false),
					(dictTextingEnabledForClinics.TryGetValue(x["ClinicNum"].ToString(),out isTextingEnabledForClinic)?isTextingEnabledForClinic:false),
					isUnknownNo,
					curCulture,
					(dictClinicCountryCodes.TryGetValue(x["ClinicNum"].ToString(),out clinicCountryCode)?clinicCountryCode:"")
				)).ToList();			
			listPatComms=AppendCommOptOuts(listPatComms);
			return listPatComms;
		}

		private static List<PatComm> AppendCommOptOuts(List<PatComm> listPatComms) {
			Dictionary<long,CommOptOut> dictOptOuts=CommOptOuts.GetForPats(listPatComms.Select(x => x.PatNum).ToList())
				.GroupBy(x => x.PatNum)
				.ToDictionary(x => x.Key,y => y.First());
			foreach(PatComm patComm in listPatComms) {
				if(dictOptOuts.ContainsKey(patComm.PatNum)) {
					patComm.CommOptOut=dictOptOuts[patComm.PatNum];
				}
			}
			return listPatComms;
		}

		public static List<PatComm> GetPatComms(List<Patient> listPats) {
			//No need to check RemotingRole; no call to db.
			bool isUnknownNo=PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo);
			string curCulture=StringTools.TruncateBeginning(System.Globalization.CultureInfo.CurrentCulture.Name,2);
			List<PatComm> listPatComms=new List<PatComm>();
			foreach(Patient pat in listPats) {
				Clinic clinic=Clinics.GetFirstOrDefault(x => x.ClinicNum==pat.ClinicNum)??Clinics.GetPracticeAsClinicZero();
				bool isEmailValidForClinic=(EmailAddresses.GetFirstOrDefault(x => x.EmailAddressNum==clinic.EmailAddressNum)!=null);
				bool isTextingEnabledForClinic=Clinics.IsTextingEnabled(clinic.ClinicNum);
				string countryCodePhone=SmsPhones.GetFirstOrDefault(x => x.ClinicNum==clinic.ClinicNum)?.CountryCode??"";
				listPatComms.Add(new PatComm(pat,isEmailValidForClinic,isTextingEnabledForClinic,isUnknownNo,curCulture,countryCodePhone));
			}
			listPatComms=AppendCommOptOuts(listPatComms);
			return listPatComms;
		}

		///<summary>Returns list of PatNums such that the PatNum is the max PatNum in it's group of numPerGroup PatNums ordered by PatNum ascending.
		///Example: If there are 1000 PatNums in the db and they are all sequential and each PatStatus is in the list of PatStatuses and the numPerGroup
		///is 500, the returned list would have 2 values in it, 500 and 1000.  Each number is the max PatNum such that if you selected the patients with
		///PatNum greater than the previous entry (or greater than 0 if it is the first entry) and less than or equal to the current entry you would get
		///at most numPerGroup patients (the last group could, of course, have fewer in it).</summary>
		public static List<long> GetPatNumMaxForGroups(int numPerGroup,List<PatientStatus> listPatStatuses) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),numPerGroup,listPatStatuses);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				throw new ApplicationException("GetPatNumMaxForGroups is not Oracle compatible.  Please call support.");
			}
			List<long> retval=new List<long>();
			if(numPerGroup<1) {
				return retval;
			}
			string whereClause="";
			if(listPatStatuses!=null && listPatStatuses.Count>0) {
				whereClause="WHERE PatStatus IN("+string.Join(",",listPatStatuses.Select(x => POut.Int((int)x)))+") ";
			}
			string command;
			long groupMaxPatNum=0;
			int groupNum=0;
			do {
				if(groupNum>0) {//after first loop, groupMaxPatNum will be set and guaranteed to be >0
					retval.Add(groupMaxPatNum);
				}
				command="SELECT MAX(PatNum) FROM (SELECT PatNum FROM patient "+whereClause+"ORDER BY PatNum LIMIT "+groupNum+","+numPerGroup+") patNumGroup";
				groupMaxPatNum=Db.GetLong(command);
				groupNum+=numPerGroup;
			} while(groupMaxPatNum>0);
			return retval;
		}

		///<summary>Gets a list of patients (with limited columns) who have had OR not had TPed procs, completed procs and/or completed appointments 
		///after the specified date, depending on values given.</summary>
		public static List<Patient> GetPatsToChangeStatus(PatientStatus patStatus,DateTime fromDate,bool doIncludeTPProc
			,bool doIncludeCompletedProc,bool doIncludeAppointments,List<long> listClinicNums) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),patStatus,fromDate,doIncludeTPProc,doIncludeCompletedProc,
					doIncludeAppointments,listClinicNums);
			}
			string whereClause="WHERE PatStatus="+POut.Int((int)patStatus)+" AND (";
			//A selectedClinicNum of -2 corresponds to clincs not enabled or all clinics
			if(!listClinicNums.IsNullOrEmpty()) {
				whereClause+="ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ) AND (";
			}
			if(doIncludeTPProc || doIncludeCompletedProc) {//TP or completed proc in date range.
				string procstatus=((doIncludeTPProc && doIncludeCompletedProc) ?"(1,2)":(doIncludeTPProc ? "(1)":"(2)"));
				whereClause+="PatNum NOT IN ("
					+"SELECT DISTINCT PatNum "
					+"FROM procedurelog "
					+"WHERE ProcStatus IN "+procstatus+" "
					+"AND ProcDate > "+SOut.DateT(fromDate,true)+
				") ";
				if(doIncludeAppointments) {//Appt in date range.
					whereClause+="AND ";
				}
			}
			if(doIncludeAppointments) {//Appt in date range.
				whereClause+="PatNum NOT IN ("
					+"SELECT DISTINCT PatNum "
					+"FROM appointment "
					+"WHERE AptDateTime > "+SOut.DateT(fromDate,true)
				+")";
			}
			string command="SELECT PatNum,PatStatus,FName,LName,MiddleI,BirthDate,ClinicNum FROM patient "+whereClause+") ORDER BY PatNum";
			DataTable table=Db.GetTable(command);
			List<Patient> listPatients=new List<Patient>();
			foreach(DataRow row in table.Rows) {
				Patient patCur=new Patient();
				patCur.PatNum=PIn.Long(row["PatNum"].ToString());
				patCur.PatStatus=(PatientStatus)PIn.Long(row["PatStatus"].ToString());
				patCur.FName=PIn.String(row["FName"].ToString());
				patCur.LName=PIn.String(row["LName"].ToString());
				patCur.Preferred="";
				patCur.MiddleI=PIn.String(row["MiddleI"].ToString());
				patCur.Birthdate=PIn.Date(row["BirthDate"].ToString());
				patCur.ClinicNum=PIn.Long(row["ClinicNum"].ToString());
				listPatients.Add(patCur);
			}
			return listPatients;
		}

		///<summary>Formats the passed in SSN for text output.  If doMask=true it will mask the SSN and only show the last 4 digits.
		///If patSSN is null, returns empty string.</summary>
		public static string SSNFormatHelper(string patSSN,bool doMask) {
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(patSSN)) {
				return "";
			}
			//Always display the last four characters.
			if(patSSN.Length<=4) {
				return patSSN;
			}
			string stringSSN=patSSN;
			//Turn all but the last four characters into x's if the return value should be masked.
			if(doMask) {
				stringSSN=stringSSN.Substring(stringSSN.Length-4,4).PadLeft(stringSSN.Length,'x');
			}
			//Apply the US SSN format of xxx-xx-xxxx when the culture is US, and only containted digits before masking.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && stringSSN.Length==9 && patSSN.All(char.IsDigit)) {
				stringSSN=stringSSN.Substring(0,3)+"-"+stringSSN.Substring(3,2)+"-"+stringSSN.Substring(5,4);
			}
			return stringSSN;
			
		}

		///<summary>Formats the passed in Birthdate for text output.  If doMask=true it will return on x's and seperators like xx/xx/xxxx</summary>
		public static string DOBFormatHelper(DateTime patBirthdate,bool doMask) {
			//No need to check RemotingRole; no call to db.
			if(patBirthdate.Year < 1880) {
				return "";//In most places anything older than this (usually minval) is just shown as blank.  Don't bother masking.
			}
			string retval=patBirthdate.ToShortDateString();//This will take care of localization formatting for us. 
			if(doMask) {
				retval=Regex.Replace(retval,"[0-9]","x");//Keep seperator characters, replace all numbers with x
			}
			return retval;
		}

		///<summary>Selects a random patient from the database.</summary>
		public static Patient GetRandomPatient() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Patient>(MethodBase.GetCurrentMethod());
			}
			int attempts=0;
			Patient pat=null;
			while(pat==null && attempts++<1000) {
				string fnameLetter=MiscUtils.CreateRandomAlphaString(1);
				string lnameLetter=MiscUtils.CreateRandomAlphaString(1);
				DateTime birthDate=MiscUtils.GetRandomDate(DateTime.Today.AddYears(-80),DateTime.Today);
				string command=$"SELECT * FROM patient WHERE FName LIKE '{POut.String(fnameLetter)}%' AND LName LIKE '{POut.String(lnameLetter)}%' "
					+$"AND Birthdate > {POut.Date(birthDate)} LIMIT 1";
				pat=Crud.PatientCrud.SelectOne(command);
				if(pat!=null) {
					return pat;
				}
			}
			if(pat==null) {
				throw new ODException("Unable to find a random patient.");
			}
			return pat;
		}

		///<summary>Returns the salted hash for the patient. Will return an empty string if the calling program is unable to use CDT.dll. </summary>
		public static string HashFields(Patient patient) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),patient);
			}
			string unhashedText=patient.PatNum.ToString();
			try {
				return CDT.Class1.CreateSaltedHash(unhashedText);
			}
			catch {
				return "";
			}
		}

		///<summary>Validates the hash string in patient.SecurityHash. Returns true if it matches the expected hash, otherwise false. </summary>
		public static bool IsPatientHashValid(Patient patient) {
			//No need to check RemotingRole; no call to db.
			if(patient==null) {
				return true;
			}
			if(patient.SecurityHash==null) {//When a patient is first created through middle tier and not yet refreshed from db, this can be null and should not show a warning triangle.
				return true;
			}
			//Do not check date, all patients are subject to validation
			//SecDateEntry only get set on Insert, so once or never. It's useless.
			if(patient.SecurityHash==HashFields(patient)) {
				return true;
			}
			return false; 
		}


	}

	///<summary>A helper class to keep track of changes made to clone demographics when synching.</summary>
	[Serializable]
	public class PatientCloneDemographicChanges {
		///<summary>A list of patient fields that have changed for the clone.</summary>
		public List<PatientCloneField> ListFieldsUpdated;
		///<summary>A list of field names that have been cleared due to a clone synch.</summary>
		public List<string> ListFieldsCleared;
	}

	///<summary>A helper class to keep track of changes made to clone PatPlans when synching.</summary>
	[Serializable]
	public class PatientClonePatPlanChanges {
		///<summary>A boolean indicating if there were any PatPlan changes necessary due to a synch.</summary>
		public bool PatPlansChanged;
		///<summary>A boolean indicating if there were any PatPlan inserted due to a synch.</summary>
		public bool PatPlansInserted;
		///<summary>A string that represents all changes made to the clone's PatPlan due to a synch.</summary>
		public string StrDataUpdated;
	}

	///<summary>A helper class to keep track of changes to specific clone fields when synching.</summary>
	[Serializable]
	public class PatientCloneField {
		///<summary>The name of the field that would display to the user.  E.g. "First Name", "Middle Initial", etc.</summary>
		public string FieldName;
		///<summary>The original value of the corresponding FieldName before the synch.</summary>
		public string OldValue;
		///<summary>The value of the corresponding FieldName after the clone has been synched.</summary>
		public string NewValue;

		public PatientCloneField(string fieldName,string oldValue,string newValue) {
			FieldName=fieldName;
			OldValue=oldValue;
			NewValue=newValue;
		}
	}

	///<summary>PatComm gets the fields of the patient table that are needed to determine electronic communications.</summary>
	[Serializable]
	public class PatComm : WebTypes.WebBase {
		public long PatNum;
		public PatientStatus PatStatus;
		public ContactMethod PreferConfirmMethod;
		public ContactMethod PreferContactMethod;
		public ContactMethod PreferRecallMethod;
		public ContactMethod PreferContactConfidential;
		public YN TxtMsgOk;
		public string HmPhone;
		public string WkPhone;
		///<summary>Do not use this number for texting. Use SmsPhone.</summary>
		public string WirelessPhone;
		public string Email;
		public string FName;
		public string LName;
		public string PreferredName;
		public long Guarantor;
		///<summary>Use this number for texting.</summary>
		public string SmsPhone;
		public bool IsEmailAnOption;
		public bool IsSmsAnOption;
		public bool IsSmsPhoneFormatOk;
		///<summary>Initialized to a new CommOptOut so null checking not required.</summary>
		public CommOptOut CommOptOut=new CommOptOut();
		public bool IsTextingEnabledForClinic;
		public bool IsEmailValidForClinic;
		public long ClinicNum;
		public string Language;
		public DateTime Birthdate;
		public bool Premed;

		///<summary>Parameterless constructor required in order to be serialized.  E.g. returns a list of PatComms in Patients.GetPatComms()</summary>
		public PatComm() {
		}

		public PatComm(Patient pat,bool isEmailValidForClinic,bool isTextingEnabledForClinic,bool isUnknownNo,string curCulture,
			string smsPhoneCountryCode) 
		{
			PatNum=pat.PatNum;
			PatStatus=pat.PatStatus;
			PreferConfirmMethod=pat.PreferConfirmMethod;
			PreferContactMethod=pat.PreferContactMethod;
			PreferRecallMethod=pat.PreferRecallMethod;
			PreferContactConfidential=pat.PreferContactConfidential;
			TxtMsgOk=pat.TxtMsgOk;
			HmPhone=pat.HmPhone;
			WkPhone=pat.WkPhone;
			WirelessPhone=pat.WirelessPhone;
			Email=pat.Email;
			FName=pat.FName;
			LName=pat.LName;
			PreferredName=pat.Preferred;
			Guarantor=pat.Guarantor;
			ClinicNum=pat.ClinicNum;
			Language=pat.Language;
			Birthdate=pat.Birthdate;
			Premed=pat.Premed;
			SetSmsEmailFields(isEmailValidForClinic,isTextingEnabledForClinic,isUnknownNo,curCulture,smsPhoneCountryCode);
		}

		public PatComm(DataRow dataRow,bool isEmailValidForClinic,bool isTextingEnabledForClinic,bool isUnknownNo,string curCulture,
			string smsPhoneCountryCode) 
		{
			PatNum=PIn.Long(dataRow["PatNum"].ToString());
			PatStatus=(PatientStatus)PIn.Int(dataRow["PatStatus"].ToString());
			PreferConfirmMethod=(ContactMethod)PIn.Int(dataRow["PreferConfirmMethod"].ToString());
			PreferContactMethod=(ContactMethod)PIn.Int(dataRow["PreferContactMethod"].ToString());
			PreferRecallMethod=(ContactMethod)PIn.Int(dataRow["PreferRecallMethod"].ToString());
			PreferContactConfidential=(ContactMethod)PIn.Int(dataRow["PreferContactConfidential"].ToString());
			TxtMsgOk=(YN)PIn.Int(dataRow["TxtMsgOk"].ToString());
			HmPhone=PIn.String(dataRow["HmPhone"].ToString());
			WkPhone=PIn.String(dataRow["WkPhone"].ToString());
			WirelessPhone=PIn.String(dataRow["WirelessPhone"].ToString());
			Email=PIn.String(dataRow["Email"].ToString());
			FName=PIn.String(dataRow["FName"].ToString());
			PreferredName=PIn.String(dataRow["Preferred"].ToString());
			LName=PIn.String(dataRow["LName"].ToString());
			Guarantor=PIn.Long(dataRow["Guarantor"].ToString());
			ClinicNum=PIn.Long(dataRow["ClinicNum"].ToString());
			Language=PIn.String(dataRow["Language"].ToString());
			Birthdate=PIn.Date(dataRow["Birthdate"].ToString());
			Premed=PIn.Bool(dataRow["Premed"].ToString());
			SetSmsEmailFields(isEmailValidForClinic,isTextingEnabledForClinic,isUnknownNo,curCulture,smsPhoneCountryCode);
		}
		
		private void SetSmsEmailFields(bool isEmailValidForClinic,bool isTextingEnabledForClinic,bool isUnknownNo,string curCulture,
			string smsPhoneCountryCode) 
		{
			IsSmsPhoneFormatOk=false;
			if(TxtMsgOk==YN.No||(isUnknownNo&&TxtMsgOk==YN.Unknown)) {
				SmsPhone="";
			}
			else {
				//Previously chose between WirelessPhone,HmPhone,WkPhone. Now chooses WirelessPhone or nothing at all.
				SmsPhone=(WirelessPhone?.Trim()??"");
				if(string.IsNullOrWhiteSpace(SmsPhone)) {
					SmsPhone="";
				}
				else {
					try {
						SmsPhone=SmsToMobiles.ConvertPhoneToInternational(SmsPhone,curCulture,smsPhoneCountryCode);
						IsSmsPhoneFormatOk=true;
					}
					catch(Exception e) { //Formatting for sms failed to set to empty so we don't try to use it.
						e.DoNothing();
						SmsPhone="";
					}
				}
			}
			IsSmsAnOption=
				//SmsPhone is in proper format for sms send.
				IsSmsPhoneFormatOk
				//Sms is allowed by practice and patient.
				&& (TxtMsgOk==YN.Yes || (TxtMsgOk==YN.Unknown&&!isUnknownNo))
				//Patient has a valid phone number.
				&& !string.IsNullOrWhiteSpace(SmsPhone)
				//Patient is not deceased
				&& PatStatus!=PatientStatus.Deceased
				//Clinic linked to this PatComm supports texting.
				&& isTextingEnabledForClinic;
			IsTextingEnabledForClinic=isTextingEnabledForClinic;	
			IsEmailAnOption=
				//Patient has a valid email.
				EmailAddresses.IsValidEmail(Email)
				//Patient is not deceased
				&& PatStatus!=PatientStatus.Deceased
				//Clinic linked to this PatComm has a valid email.
				&& isEmailValidForClinic;
			IsEmailValidForClinic=isEmailValidForClinic;
		}

		///<summary>Returns the reason that the patient cannot receive text messages.</summary>
		public string GetReasonCantText(CommOptOutType type=0) {
			if(!IsTextingEnabledForClinic) {
				return "Not sending text because texting is not enabled for this clinic.";
			}
			if(TxtMsgOk==YN.No || (TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo))) {
				return "Not sending text because this patient is set to not receive texts.";
			}
			if(string.IsNullOrEmpty(WirelessPhone)) {
				return "Not sending text because this patient does not have a wireless phone entered.";
			}
			if(!IsSmsPhoneFormatOk) {
				return "Not sending text because this patient's wireless phone is not valid.";
			}
			if(!IsSmsAnOption) {
				return "Not sending text because this patient is not able to receive texts.";
			}
			if(CommOptOut.IsOptedOut(CommOptOutMode.Text,type)) {
				return "Not sending text because this patient opted out of receiving automated text messages.";
			}
			return "";
		}

		///<summary>Returns the reason that the patient cannot receive emails.</summary>
		public string GetReasonCantEmail(CommOptOutType type=0) {
			if(!IsEmailValidForClinic) {
				return "Not sending email because email is not enabled for this clinic.";
			}
			if(string.IsNullOrEmpty(Email)) {
				return "Not sending email because this patient does not have an email entered.";
			}
			if(!EmailAddresses.IsValidEmail(Email)) {
				return "Not sending email because this patient's email is not a valid address.";
			}
			if(!IsEmailAnOption) {
				return "Not sending email because this patient is not able to receive emails.";
			}
			if(CommOptOut.IsOptedOut(CommOptOutMode.Email,type)) {
				return "Not sending email because this patient opted out of receiving automated email messages.";
			}
			return "";
		}		

		///<summary>Builds a confirmation message string based on the appropriate preference, given patient, and given date.</summary>
		public static string BuildConfirmMessage(ContactMethod contactMethod,Patient pat,DateTime dateTimeAskedToArrive,DateTime apptDateTime) {
			string template=contactMethod switch {
				ContactMethod.Email=>PrefC.GetString(PrefName.ConfirmEmailMessage),
				ContactMethod.TextMessage=>PrefC.GetString(PrefName.ConfirmTextMessage),
				ContactMethod.Mail=>PrefC.GetString(PrefName.ConfirmPostcardMessage),
				_=>PrefC.GetString(PrefName.ConfirmTextMessage),
			};
			return BuildAppointmentMessage(pat,dateTimeAskedToArrive,apptDateTime,template);
		}

		///<summary>Builds an appointment information message string based on the given template, given patient, and given date.</summary>
		public static string BuildAppointmentMessage(Patient pat,DateTime dateTimeAskedToArrive,DateTime apptDateTime
			,string template="[NameF]:  [date] at [time]",bool isEmail=false) {
			DateTime dateTime=apptDateTime;
			if(dateTimeAskedToArrive.Year>1880) {
				dateTime=dateTimeAskedToArrive;
			}
			if(pat!=null) {
				string name=Patients.GetNameFirstOrPreferred(pat.FName,pat.Preferred);
				Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
				TagReplacer tagReplacer=new TagReplacer();
				AutoCommObj autoCommObj=new AutoCommObj();
				autoCommObj.NameF=pat.FName;
				autoCommObj.NamePreferredOrFirst=name;
				autoCommObj.ProvNum=pat.PriProv;
				template=tagReplacer.ReplaceTags(template,autoCommObj,clinic,isEmail);
			}
			template=template.Replace("[date]",dateTime.ToString(PrefC.PatientCommunicationDateFormat)); //[date] and [time] aren't considered in ReplaceTags
			template=template.Replace("[time]",dateTime.ToShortTimeString());
			return template;
		}

		public string GetFirstOrPreferred() {
			return Patients.GetNameFirstOrPreferred(FName,PreferredName);
		}

		#region Short Codes

		///<summary>Returns true if the clinic is using an eService automated texting feature for which it is set to use Short Codes and the patient is 
		///set to receive sms.</summary>
		public bool IsPatientShortCodeEligible(long clinicNum) {
			return IsSmsAnOption //Patient set to receive sms.
				&& IsAnyShortCodeServiceEnabled(clinicNum); //At least one Short Code eService is enabled
		}

		///<summary>Determines if any of the eServices the clinic is set to use Short Codes are actually enabled.</summary>
		public static bool IsAnyShortCodeServiceEnabled(long clinicNum) {
			return EnumTools.GetFlags((ShortCodeTypeFlag)ClinicPrefs.GetLong(PrefName.ShortCodeApptReminderTypes,clinicNum))
				.Any(x => EnumTools.GetAttributeOrDefault<ShortCodeAttribute>(x).IsServiceEnabled(clinicNum));
		}
		#endregion
	}

	///<summary>Not a database table.  Just used in billing and finance charges.</summary>
	public class PatAging{
		///<summary></summary>
		public long PatNum;
		///<summary></summary>
		public double Bal_0_30;
		///<summary></summary>
		public double Bal_31_60;
		///<summary></summary>
		public double Bal_61_90;
		///<summary></summary>
		public double BalOver90;
		///<summary></summary>
		public double InsEst;
		///<summary></summary>
		public string PatName;
		/// <summary>Enum:PatientStatus</summary>
		public PatientStatus PatStatus;
		///<summary></summary>
		public double BalTotal;
		///<summary></summary>
		public double AmountDue;
		///<summary>The patient priprov to assign the finance charge to.</summary>
		public long PriProv;
		///<summary>The date of the last statement.</summary>
		public DateTime DateLastStatement;
		///<summary>FK to defNum.</summary>
		public long BillingType;
		///<summary>Only set in some areas.</summary>
		public long ClinicNum;
		///<summary></summary>
		public double PayPlanDue;
		///<summary></summary>
		public long SuperFamily;
		///<summary></summary>
		public bool HasSuperBilling;
		///<summary></summary>
		public long Guarantor;
		///<summary>Signed Truth in Lending</summary>
		public bool HasSignedTil;
		///<summary>Only used for Transworld AR Manager.</summary>
		public DateTime DateLastPay;
		///<summary>Only used for Transworld AR Manager.</summary>
		public DateTime DateLastProc;
		///<summary>Only used for Transworld AR Manager.</summary>
		public DateTime DateBalBegan;
		///<summary>Only used for Transworld AR Manager.</summary>
		public string Address;
		///<summary>Only used for Transworld AR Manager.</summary>
		public string City;
		///<summary>Only used for Transworld AR Manager.</summary>
		public string State;
		///<summary>Only used for Transworld AR Manager.  Used to exclude bad addresses from the list.</summary>
		public string Zip;
		///<summary>Only used for Transworld AR Manager.</summary>
		public DateTime Birthdate;
		///<summary>Only used for Transworld AR Manager.</summary>
		public bool HasUnsentProcs;
		///<summary>Only used for Transworld AR Manager.</summary>
		public bool HasInsPending;
		///<summary>Only used for Transworld AR Manager.  All trans sent to TSI for this guarantor, ordered by TransDateTime descending.</summary>
		public List<TsiTransLog> ListTsiLogs;

		///<summary></summary>
		public PatAging Copy() {
			PatAging retval=(PatAging)this.MemberwiseClone();
			retval.ListTsiLogs=this.ListTsiLogs.Select(x => x.Copy()).ToList();
			return retval;
		}
	}

	public class ClinicBalBegans {
		public long ClinicNum;
		public Dictionary<long,DateTime> DictGuarDateBals;

		public ClinicBalBegans(long clinicNum,Dictionary<long,DateTime> dictGuarDateBals) {
			ClinicNum=clinicNum;
			DictGuarDateBals=dictGuarDateBals;
		}
	}
	
	[Serializable]
	public class PtTableSearchParams {
		public bool DoLimit;
		public string LName;
		public string FName;
		public string Phone="";
		public string Address="";
		public bool HideInactive;
		public string City;
		public string State;
		public string Ssn;
		public string PatNumStr="";
		public string ChartNumber;
		public long BillingType;
		public bool GuarOnly;
		public bool ShowArchived;
		public DateTime Birthdate;
		public long SiteNum;
		public string SubscriberId;
		public string Email;
		public string Country;
		public string RegKey;
		public string ClinicNums;
		///<summary>Used in CEMT because we don't have access to ClinicNums</summary>
		public string ClinicName;
		public string InvoiceNumber="";
		public long InitialPatNum;
		public bool ShowMerged;
		public bool HasSpecialty;
		public bool HasNextLastVisit;
		public List<long> ListExplicitPatNums;
		
		///<summary></summary>
		public PtTableSearchParams() { }

		///<summary>SOut's all string values to be used in search query.</summary>
		public PtTableSearchParams(bool doLimit,string lname,string fname,string phone,string address,bool hideInactive,string city,
			string state,string ssn,string patNumStr,string chartNumber,long billingType,bool guarOnly,bool showArchived,DateTime birthdate,long siteNum,
			string subscriberId,string email,string country,string regKey,string clinicNums,string clinicName,string invoiceNumber,List<long> listExplicitPatNums=null,
			long initialPatNum=0,bool showMerged=false,bool hasSpecialty=false,bool hasNextLastVisit=false)
		{
			DoLimit=doLimit;//bool
			LName=SOut.String(lname);
			FName=SOut.String(fname);
			Phone=SOut.String(TelephoneNumbers.AutoFormat(phone));
			Address=SOut.String(address);
			HideInactive=hideInactive;//bool
			City=SOut.String(city);
			State=SOut.String(state);
			Ssn=SOut.String(ssn);
			PatNumStr=SOut.String(patNumStr);
			ChartNumber=SOut.String(chartNumber);
			BillingType=billingType;//long
			GuarOnly=guarOnly;//bool
			ShowArchived=showArchived;//bool
			Birthdate=birthdate;//date
			SiteNum=siteNum;//long
			SubscriberId=SOut.String(subscriberId);
			Email=SOut.String(email);
			Country=SOut.String(country);
			RegKey=SOut.String(new string(regKey.Where(x => char.IsLetterOrDigit(x)).ToArray()));
			ClinicNums=SOut.String(clinicNums);
			ClinicName=SOut.String(clinicName);
			InvoiceNumber=SOut.String(invoiceNumber);
			InitialPatNum=initialPatNum;//long
			ShowMerged=showMerged;//bool
			HasSpecialty=hasSpecialty;//bool
			HasNextLastVisit=hasNextLastVisit;//bool
			ListExplicitPatNums=(listExplicitPatNums??new List<long>());//List<long>
		}

		///<summary></summary>
		public PtTableSearchParams Copy() {
			PtTableSearchParams retval=(PtTableSearchParams)MemberwiseClone();
			retval.ListExplicitPatNums=ListExplicitPatNums.ToList();
			return retval;
		}

		///<summary>Used to determine if the select patient query search params are the same as the previous run or if the query should be run again.</summary>
		public override bool Equals(object obj) {
			if(!(obj is PtTableSearchParams)) {
				return false;
			}
			PtTableSearchParams pTSPOther=obj as PtTableSearchParams;
			if(DoLimit!=pTSPOther.DoLimit
				|| LName!=pTSPOther.LName
				|| FName!=pTSPOther.FName
				|| Phone!=pTSPOther.Phone
				|| Address!=pTSPOther.Address
				|| HideInactive!=pTSPOther.HideInactive
				|| City!=pTSPOther.City
				|| State!=pTSPOther.State
				|| Ssn!=pTSPOther.Ssn
				|| PatNumStr!=pTSPOther.PatNumStr
				|| ChartNumber!=pTSPOther.ChartNumber
				|| BillingType!=pTSPOther.BillingType
				|| GuarOnly!=pTSPOther.GuarOnly
				|| ShowArchived!=pTSPOther.ShowArchived
				|| Birthdate!=pTSPOther.Birthdate
				|| SiteNum!=pTSPOther.SiteNum
				|| SubscriberId!=pTSPOther.SubscriberId
				|| Email!=pTSPOther.Email
				|| Country!=pTSPOther.Country
				|| RegKey!=pTSPOther.RegKey
				|| ClinicNums!=pTSPOther.ClinicNums
				|| ClinicName!=pTSPOther.ClinicName
				|| InvoiceNumber!=pTSPOther.InvoiceNumber
				|| InitialPatNum!=pTSPOther.InitialPatNum
				|| ShowMerged!=pTSPOther.ShowMerged
				|| HasSpecialty!=pTSPOther.HasSpecialty
				|| HasNextLastVisit!=pTSPOther.HasNextLastVisit
				|| (ListExplicitPatNums==null)!=(pTSPOther.ListExplicitPatNums==null)
				|| (ListExplicitPatNums!=null && pTSPOther.ListExplicitPatNums!=null
					&& (ListExplicitPatNums.Except(pTSPOther.ListExplicitPatNums).Any() || pTSPOther.ListExplicitPatNums.Except(ListExplicitPatNums).Any())))
			{
				return false;
			}
			return true;
		}

		///<summary>We must define GetHashCode() because we defined Equals() above, or else we get a warning message.</summary>
		public override int GetHashCode() {
			//Always return the same value (0 is acceptable). This will defer to the Equals override as the tie-breaker, which is what we want in this case.
			return 0;
		}
	}

	public class PatientWithServerDT {
		public Patient PatientCur;
		public DateTime DateTimeServer;
	}

	public class PatientFor834Import:IComparable {
		public long PatNum;
		public string FName;
		public string LName;
		public DateTime Birthdate;
		public PatientStatus PatStatus;
		//This is used for filtering out what we are considering "active" vs "inactive" patients. 
		private static List<PatientStatus> _listPatientStatuses = new List<PatientStatus>() {PatientStatus.Patient,PatientStatus.Inactive,PatientStatus.NonPatient,PatientStatus.Prospective };

		public static explicit operator PatientFor834Import(Patient patient) {
			PatientFor834Import patientLimited = new PatientFor834Import();
			patientLimited.PatNum = patient.PatNum;
			patientLimited.FName = patient.FName;
			patientLimited.LName = patient.LName;
			patientLimited.Birthdate = patient.Birthdate;
			patientLimited.PatStatus = patient.PatStatus;
			return patientLimited;
		}

		///<summary>Useful for sorting and binary searching.  The X12 834 implementation uses this for binary searching to improve efficiency.
		///If this function is changed in the future, it will heavily impact our X12 834 implementation.  Be cautious.  In the end, this function
		///will probably not need to change anyway, since it will only be used for comparing patients when the PatNums are not known.</summary>
		public int CompareTo(object patOther) {
			PatientFor834Import p1=this;
			PatientFor834Import p2=(PatientFor834Import)patOther;
			string lname1=(p1.LName==null)?"":p1.LName;
			string lname2=(p2.LName==null)?"":p2.LName;
			int comp=lname1.Trim().ToLower().CompareTo(lname2.Trim().ToLower());
			if(comp!=0) {
				return comp;
			}
			string fname1=(p1.FName==null)?"":p1.FName;
			string fname2=(p2.FName==null)?"":p2.FName;
			comp=fname1.Trim().ToLower().CompareTo(fname2.Trim().ToLower());
			if(comp!=0) {
				return comp;
			}
			return p1.Birthdate.Date.CompareTo(p2.Birthdate.Date);
		}

		///<summary>Returns a list of all patients within listSortedPatients which match the given pat.LName, pat.FName and pat.Birthdate.
		///Ignores case and leading/trailing space.  The listSortedPatients MUST be sorted by LName, then FName, then Birthdate or else the result will be
		///wrong.  Call listSortedPatients.Sort() before calling this function.  This function uses a binary search to much more efficiently locate
		///matches than a linear search would be able to.</summary>
		public static List<PatientFor834Import> GetPatientLimitedsByNameAndBirthday(Patient patient,List<PatientFor834Import> listSortedPatients) {
			PatientFor834Import pat = (PatientFor834Import)patient;
			if(pat.LName.Trim().ToLower().Length==0 || pat.FName.Trim().ToLower().Length==0 || pat.Birthdate.Year < 1880) {
				//We do not allow a match unless Last Name, First Name, AND birthdate are specified.  Otherwise at match could be meaningless.
				return new List<PatientFor834Import>();
			}
			int patIdx=listSortedPatients.BinarySearch(pat);//If there are multiple matches, then this will only return one of the indexes randomly.
			if(patIdx < 0) {
				//No matches found.
				return new List<PatientFor834Import>();
			}
			//The matched indicies will all be consecutive and will include the returned index from the binary search, because the list is sorted.
			int beginIdx=patIdx;
			for(int i = patIdx-1;i >= 0 && pat.CompareTo(listSortedPatients[i])==0;i--) {
				beginIdx=i;
			}
			int endIdx=patIdx;
			for(int i = patIdx+1;i < listSortedPatients.Count && pat.CompareTo(listSortedPatients[i])==0;i++) {
				endIdx=i;
			}
			List <PatientFor834Import> listPatientMatches=new List<PatientFor834Import>();
			for(int i = beginIdx;i<=endIdx;i++) {
				listPatientMatches.Add(listSortedPatients[i]);
			}
			return listPatientMatches;
		}

		//filters list of potential matches. First checks to see if we have any patient's with a status of Patient, Non-Patient, Inactive, or Prospective.
		//If there are no patients with those status's, don't modify the list. 
		//If there is at least one patient in that status, filter out Archived and Deceased patients from the list/ 
		//We want to make sure that we still show duplicates if they are both archived and/or deceased
		public static void FilterMatchingList(ref List<PatientFor834Import> listPatientMatches) {
			if(listPatientMatches.Where(x => _listPatientStatuses.Contains(x.PatStatus)).ToList().Count == 0) {
				return;
			}
			if(listPatientMatches.Count > 1) {
				listPatientMatches = listPatientMatches
				.Where(x =>
					(!(x.PatStatus == PatientStatus.Archived
					|| x.PatStatus == PatientStatus.Deceased))
				).ToList();
			}
		}
	}

}