using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary>Use ListLong or ListShort to get a cached list of clinics that you can then filter upon.</summary>
	public class Clinics {
		#region Get Methods
		///<summary>Returns a list of clinics that are associated to any clones of the master patient passed in (patNumFrom).
		///This method will include the clinic for the master patient passed in.</summary>
		public static List<Clinic> GetClinicsForClones(long patNumFrom) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Clinic>>(MethodBase.GetCurrentMethod(),patNumFrom);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				throw new ApplicationException("GetClinicsForClones is not currently Oracle compatible.  Please call support.");
			}
			//Always include the master patient's clinic.
			List<long> listPatNumClones=new List<long>() { patNumFrom };
			//Get all clones (PatNumTos) that are associated to the master patient passed in (patNumFrom).
			listPatNumClones.AddRange(PatientLinks.GetPatNumsLinkedFrom(patNumFrom,PatientLinkType.Clone));
			//Get the clinics associated to all of the clones for this patient.
			string command="SELECT * FROM clinic "
				+"INNER JOIN patient ON clinic.ClinicNum=patient.ClinicNum "
				+"WHERE patient.PatNum IN ("+string.Join(",",listPatNumClones)+") "
				+"GROUP BY clinic.ClinicNum";//We only care about unique clinics, it doesn't matter if several clones have the same clinic.
			return Crud.ClinicCrud.SelectMany(command);
		}

		///<summary>Returns a dicitonary such that the key is a clinicNum and the value is a count of patients whith a matching patient.ClinicNum.
		///Excludes all patients with PatStatus of Deleted, Archived, Deceased, or NonPatient unless IsAllStatuses is set to true.</summary>
		public static SerializableDictionary<long,int> GetClinicalPatientCount(bool IsAllStatuses=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,int>(MethodBase.GetCurrentMethod(),IsAllStatuses);
			}
			string command="SELECT ClinicNum,COUNT(*) AS Count FROM patient ";
			if(!IsAllStatuses) {
				command+="WHERE PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Archived)+","
					+POut.Int((int)PatientStatus.Deceased)+","+POut.Int((int)PatientStatus.NonPatient)+") ";
			} 
			command+="GROUP BY ClinicNum";
			return Db.GetTable(command).Select().ToSerializableDictionary(x => PIn.Long(x["ClinicNum"].ToString()),x => PIn.Int(x["Count"].ToString()));
		}

		///<summary>Gets a list of Clinics for a given pharmacyNum.</summary>
		///<param name="pharmacyNum">The primary key of the pharmacy.</param>
		public static List<Clinic> GetClinicsForPharmacy(long pharmacyNum) {
			//No need to check RemotingRole; no call to db.
			SerializableDictionary<long,List<Clinic>> dict=GetDictClinicsForPharmacy(pharmacyNum);
			List<Clinic> listClinics;
			if(!dict.TryGetValue(pharmacyNum,out listClinics)) {
				listClinics=new List<Clinic>();
			}
			return listClinics;
		}

		///<summary>Gets a SerializableDictionary of Lists of Clinics for given pharmacyNums.</summary>
		///<param name="arrPharmacyNums">The primary key of the pharmacy.</param>
		public static SerializableDictionary<long,List<Clinic>> GetDictClinicsForPharmacy(params long[] arrPharmacyNums) {
			SerializableDictionary<long,List<Clinic>> dict=new SerializableDictionary<long,List<Clinic>>();
			if(arrPharmacyNums.Length==0) {
				return dict;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,List<Clinic>>>(MethodBase.GetCurrentMethod(),arrPharmacyNums);
			}
			string command="SELECT pharmclinic.PharmacyNum,clinic.* "
				+"FROM clinic "
				+"INNER JOIN pharmclinic ON pharmclinic.ClinicNum=clinic.ClinicNum "
				+"WHERE pharmclinic.PharmacyNum IN("+string.Join(",",arrPharmacyNums)+") "
				+"ORDER BY clinic.Abbr";
			DataTable table=Db.GetTable(command);
			List<Clinic> listClinics=Crud.ClinicCrud.TableToList(table);
			for(int i=0;i<table.Rows.Count;i++) {
				long pharmacyNum=PIn.Long(table.Rows[i]["PharmacyNum"].ToString());
				Clinic clinic=listClinics[i];//1:1
				List<Clinic> listClinicsPharm;
				if(!dict.TryGetValue(pharmacyNum,out listClinicsPharm)) {
					listClinicsPharm=new List<Clinic>();
					dict.Add(pharmacyNum,listClinicsPharm);
				}
				listClinicsPharm.Add(clinic);
			}
			return dict;
		}
		#endregion

		///<summary>Currently active clinic within OpenDental.  Reflects FormOpenDental.ClinicNum</summary>
		private static long _clinicNum=0;

		#region Cache Pattern

		private class ClinicCache : CacheListAbs<Clinic> {
			protected override List<Clinic> GetCacheFromDb() {
				string command="SELECT * FROM clinic ";
				if(PrefC.GetBool(PrefName.ClinicListIsAlphabetical)) {
					command+="ORDER BY Abbr";
				}
				else {
					command+="ORDER BY ItemOrder";
				}
				List<Clinic> retval=Crud.ClinicCrud.SelectMany(command);
				Dictionary<long,List<DefLink>> dictClinSpecialties=DefLinks.GetDefLinksByType(DefLinkType.Clinic)
					.GroupBy(x => x.FKey)
					.ToDictionary(x => x.Key,x => x.ToList());
				List<DefLink> listLinks;
				retval.ForEach(x => x.ListClinicSpecialtyDefLinks=(dictClinSpecialties.TryGetValue(x.ClinicNum,out listLinks)?listLinks:new List<DefLink>()));
				return retval;
			}
			protected override List<Clinic> TableToList(DataTable table) {
				return Crud.ClinicCrud.TableToList(table);
			}
			protected override Clinic Copy(Clinic clinic) {
				return clinic.Copy();
			}
			protected override DataTable ListToTable(List<Clinic> listClinics) {
				return Crud.ClinicCrud.ListToTable(listClinics,"Clinic");
			}
			protected override void FillCacheIfNeeded() {
				Clinics.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Clinic clinic) {
				return !clinic.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClinicCache _clinicCache=new ClinicCache();

		public static List<Clinic> GetDeepCopy(bool isShort=false) {
			return _clinicCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _clinicCache.GetCount(isShort);
		}

		public static Clinic GetFirst(bool isShort=false) {
			return _clinicCache.GetFirst(isShort);
		}

		public static Clinic GetFirstOrDefault(Func<Clinic,bool> match,bool isShort=false) {
			return _clinicCache.GetFirstOrDefault(match,isShort);
		}

		public static List<Clinic> GetWhere(Predicate<Clinic> match,bool isShort=false) {
			return _clinicCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_clinicCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_clinicCache.FillCacheFromTable(table);
				return table;
			}
			return _clinicCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(Clinic clinic,bool useExistingPK = false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				clinic.ClinicNum=Meth.GetLong(MethodBase.GetCurrentMethod(),clinic,useExistingPK);
				return clinic.ClinicNum;
			}
			return Crud.ClinicCrud.Insert(clinic,useExistingPK);
		}

		///<summary>The currently selected clinic's ClinicNum.  Unlike the one stored in FormOpenDental, this is accessible from the business layer.</summary>
		public static long ClinicNum {
			get { return _clinicNum; }
			set {
				if(_clinicNum==value) {
					return;//no change
				}
				_clinicNum=value;
				if(Security.CurUser==null) {
					return;
				}
				if(PrefC.GetString(PrefName.ClinicTrackLast)!="User") {
					return;
				}
				List<UserOdPref> prefs = UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ClinicLast);//should only be one.
				if(prefs.Count>0) {
					prefs.ForEach(x => x.Fkey=value);
					prefs.ForEach(UserOdPrefs.Update);
					return;
				}
				UserOdPrefs.Insert(
					new UserOdPref() {
						UserNum=Security.CurUser.UserNum,
						FkeyType=UserOdFkeyType.ClinicLast,
						Fkey=value,
					});
			}//end set
		}

		///<summary>Sets Clinics.ClinicNum. Used when logging on to determines what clinic to start with based on user and workstation preferences.</summary>
		public static void LoadClinicNumForUser() {
			_clinicNum=0;//aka headquarters clinic when clinics are enabled.
			if(!PrefC.HasClinicsEnabled || Security.CurUser==null) {
				return;
			}
			List<Clinic> listClinics = Clinics.GetForUserod(Security.CurUser);
			switch(PrefC.GetString(PrefName.ClinicTrackLast)) {
				case "Workstation":
					if(Security.CurUser.ClinicIsRestricted && Security.CurUser.ClinicNum!=ComputerPrefs.LocalComputer.ClinicNum) {//The user is restricted and it's not the clinic this computer has by default
						//User's default clinic isn't the LocalComputer's clinic, see if they have access to the Localcomputer's clinic, if so, use it.
						Clinic clinic=listClinics.Find(x => x.ClinicNum==ComputerPrefs.LocalComputer.ClinicNum);
						if(clinic!=null) {
							_clinicNum=clinic.ClinicNum;
						}
						else {
							_clinicNum=Security.CurUser.ClinicNum;//Use the user's default clinic if they don't have access to LocalComputer's clinic.
						}
					}
					else {//The user is not restricted, just use the clinic in the ComputerPref table.
						_clinicNum=ComputerPrefs.LocalComputer.ClinicNum;
					}
					return;//Error
				case "User":
					List<UserOdPref> prefs = UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ClinicLast);//should only be one or none.
					if(prefs.Count==0) {
						UserOdPref pref =
							new UserOdPref() {
								UserNum=Security.CurUser.UserNum,
								FkeyType=UserOdFkeyType.ClinicLast,
								Fkey=Security.CurUser.ClinicNum//default clinic num
							};
						UserOdPrefs.Insert(pref);
						prefs.Add(pref);
					}
					if(listClinics.Any(x => x.ClinicNum==prefs[0].Fkey)) {//user is restricted and does not have access to the computerpref clinic
						_clinicNum=prefs[0].Fkey;
					}
					return;
				case "None":
				default:
					if(listClinics.Any(x => x.ClinicNum==Security.CurUser.ClinicNum)) {
						_clinicNum=Security.CurUser.ClinicNum;
					}
					break;
			}
		}

		///<summary>Called when logging user off or closing opendental.</summary>
		public static void LogOff() {
			if(!PrefC.HasClinicsEnabled) {
				_clinicNum=0;
				return;
			}
			switch(PrefC.GetString(PrefName.ClinicTrackLast)) {
				case "Workstation":
					ComputerPrefs.LocalComputer.ClinicNum=Clinics.ClinicNum;
					ComputerPrefs.Update(ComputerPrefs.LocalComputer);
					break;
				case "User"://handled below
				case "None":
				default:
					break;
			}
			//We want to always upsert a user pref for the user because we will be looking at it for MobileWeb regardless of the preference for 
			//ClinicTrackLast.
			List<UserOdPref> UserPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ClinicLast);//should only be one or none.
			if(UserPrefs.Count==0) {
				UserOdPref pref=new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.ClinicLast,
					Fkey=Clinics.ClinicNum
				};
				UserOdPrefs.Insert(pref);
			}
			UserPrefs.ForEach(x => {
				x.Fkey=Clinics.ClinicNum;
				UserOdPrefs.Update(x);
			});
			_clinicNum=0;
		}

		///<summary></summary>
		public static void Update(Clinic clinic){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinic);
				return;
			}
			Crud.ClinicCrud.Update(clinic);
		}

		///<summary>Only sync changed fields.</summary>
		public static bool Update(Clinic clinic,Clinic oldClinic) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),clinic,oldClinic);
			}
			return Crud.ClinicCrud.Update(clinic,oldClinic);
		}

		///<summary>Checks dependencies first.  Throws exception if can't delete.</summary>
		public static void Delete(Clinic clinic) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinic);
				return;
			}
			//Check FK dependencies.
			#region Patients
			string command="SELECT LName,FName FROM patient WHERE ClinicNum ="
				+POut.Long(clinic.ClinicNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string pats="";
				for(int i=0;i<table.Rows.Count;i++) {
					pats+="\r";
					if(i==15) {
						pats+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					pats+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because it is in use by the following patients:")+pats);
			}
			#endregion
			#region Payments
			command="SELECT patient.LName,patient.FName FROM patient,payment "
				+"WHERE payment.ClinicNum ="+POut.Long(clinic.ClinicNum)
				+" AND patient.PatNum=payment.PatNum";
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string pats="";
				for(int i=0;i<table.Rows.Count;i++) {
					pats+="\r";
					if(i==15) {
						pats+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					pats+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following patients have payments using it:")+pats);
			}
			#endregion
			#region ClaimPayments
			command="SELECT patient.LName,patient.FName FROM patient,claimproc,claimpayment "
				+"WHERE claimpayment.ClinicNum ="+POut.Long(clinic.ClinicNum)
				+" AND patient.PatNum=claimproc.PatNum"
				+" AND claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
				+"GROUP BY patient.LName,patient.FName,claimpayment.ClaimPaymentNum";
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string pats="";
				for(int i=0;i<table.Rows.Count;i++) {
					pats+="\r";
					if(i==15) {
						pats+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					pats+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following patients have claim payments using it:")+pats);
			}
			#endregion
			#region Appointments
			command="SELECT patient.LName,patient.FName FROM patient,appointment "
				+"WHERE appointment.ClinicNum ="+POut.Long(clinic.ClinicNum)
				+" AND patient.PatNum=appointment.PatNum";
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string pats="";
				for(int i=0;i<table.Rows.Count;i++) {
					pats+="\r";
					if(i==15) {
						pats+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					pats+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following patients have appointments using it:")+pats);
			}
			#endregion
			#region Procedures
			//reassign procedure.ClinicNum=0 if the procs are status D.
			command="SELECT ProcNum FROM procedurelog WHERE ProcStatus="+POut.Int((int)ProcStat.D)+" AND ClinicNum="+POut.Long(clinic.ClinicNum);
			List<long> listProcNums=Db.GetListLong(command);
			if(listProcNums.Count>0) {
				command="UPDATE procedurelog SET ClinicNum=0 WHERE ProcNum IN ("+string.Join(",",listProcNums.Select(x => POut.Long(x)))+")";
				Db.NonQ(command);
			}
			command="SELECT patient.LName,patient.FName FROM patient,procedurelog "
				+"WHERE procedurelog.ClinicNum ="+POut.Long(clinic.ClinicNum)
				+" AND patient.PatNum=procedurelog.PatNum";
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string pats="";
				for(int i=0;i<table.Rows.Count;i++) {
					pats+="\r";
					if(i==15) {
						pats+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					pats+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following patients have procedures using it:")+pats);
			}
			#endregion
			#region Operatories
			command="SELECT OpName FROM operatory "
				+"WHERE ClinicNum ="+POut.Long(clinic.ClinicNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string ops="";
				for(int i=0;i<table.Rows.Count;i++) {
					ops+="\r";
					if(i==15) {
						ops+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					ops+=table.Rows[i]["OpName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following operatories are using it:")+ops);
			}
			#endregion
			#region Userod
			command="SELECT UserName FROM userod "
				+"WHERE ClinicNum ="+POut.Long(clinic.ClinicNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string userNames="";
				for(int i=0;i<table.Rows.Count;i++) {
					userNames+="\r";
					if(i==15) {
						userNames+=Lans.g("Clinics","And")+" "+(table.Rows.Count-i)+" "+Lans.g("Clinics","others");
						break;
					}
					userNames+=table.Rows[i]["UserName"].ToString();
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following Open Dental users are using it:")+userNames);
			}
			#endregion
			#region AlertSub
			command="SELECT DISTINCT UserNum FROM AlertSub "
				+"WHERE ClinicNum ="+POut.Long(clinic.ClinicNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				List<string> listUsers=new List<string>();
				for(int i=0;i<table.Rows.Count;i++) {
					long userNum=PIn.Long(table.Rows[i]["UserNum"].ToString());
					Userod user=Userods.GetUser(userNum);
					if(user==null) {//Should not happen.
						continue;
					}
					listUsers.Add(user.UserName);
				}
				throw new Exception(Lans.g("Clinics","Cannot delete clinic because the following Open Dental users are subscribed to it:")+"\r"+String.Join("\r",listUsers.OrderBy(x => x).ToArray()));
			}
			#endregion
			#region UserClinics
			command="SELECT userod.UserName FROM userclinic INNER JOIN userod ON userclinic.UserNum=userod.UserNum "
				+"WHERE userclinic.ClinicNum="+POut.Long(clinic.ClinicNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string users="";
				for(int i=0;i<table.Rows.Count;i++) {
					if(i > 0) {
						users+=",";
					}
					users+=table.Rows[i][0].ToString();
				}
				throw new Exception(
					Lans.g("Clinics","Cannot delete clinic because the following users are restricted to this clinic in security setup:")+" "+users);
			}
			#endregion
			//End checking for dependencies.
			//Clinic is not being used, OK to delete.
			//Delete clinic specific program properties.
			command="DELETE FROM programproperty WHERE ClinicNum="+POut.Long(clinic.ClinicNum)+" AND ClinicNum!=0";//just in case a programming error tries to delete an invalid clinic.
			Db.NonQ(command);
			Crud.ClinicCrud.Delete(clinic.ClinicNum);
		}

		///<summary>Returns a list of clinicNums with the specified regions' DefNums.</summary>
		public static List<long> GetListByRegion(List<long> listRegionDefNums) {
			List<Clinic> listClinicsForRegion=GetWhere(x => listRegionDefNums.Contains(x.Region));
			return listClinicsForRegion.Select(x => x.ClinicNum).Distinct().ToList();
		}

		///<summary>Returns null if clinic not found.  Pulls from cache.</summary>
		public static Clinic GetClinic(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClinicNum==clinicNum);
		}

		///<summary>Returns null if clinic not found.  Pulls from database.</summary>
		public static Clinic GetClinicNoCache(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Clinic>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM clinic WHERE ClinicNum="+POut.Long(clinicNum);
			return Crud.ClinicCrud.SelectOne(command);
		}

		///<summary>Pulls from cache.  Can contain a null clinic if not found.  Includes hidden clinics.</summary>
		public static List<Clinic> GetClinics(List<long> listClinicNums) {
			//No need to check RemotingRole; no call to db.
			List<Clinic> listClinics=new List<Clinic>();
			for(int i=0;i<listClinicNums.Count;i++) {
				if(listClinicNums[i]==0) {
					continue;
				}
				listClinics.Add(GetClinic(listClinicNums[i]));
			}
			return listClinics;
		}

		///<summary>Syncs two supplied lists of Clinics. Returns true if db changes were made.</summary>
		public static bool Sync(List<Clinic> listNew,List<Clinic> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.ClinicCrud.Sync(listNew,listOld);
		}

		///<summary>Returns the patient's clinic based on the recall passed in.
		///If the patient is no longer associated to a clinic, 
		///  returns the clinic associated to the appointment (scheduled or completed) with the largest date.
		///Returns null if the patient doesn't have a clinic or if the clinics feature is not activate.</summary>
		public static Clinic GetClinicForRecall(long recallNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Clinic>(MethodBase.GetCurrentMethod(),recallNum);
			}
			if(!PrefC.HasClinicsEnabled) {
				return null;
			}
			string command="SELECT patient.ClinicNum FROM patient "
				+"INNER JOIN recall ON patient.PatNum=recall.PatNum "
				+"WHERE recall.RecallNum="+POut.Long(recallNum)+" "
				+DbHelper.LimitAnd(1);
			long patientClinicNum=PIn.Long(DataCore.GetScalar(command));
			if(patientClinicNum>0) {
				return GetFirstOrDefault(x => x.ClinicNum==patientClinicNum);
			}
			//Patient does not have an assigned clinic.  Grab the clinic from a scheduled or completed appointment with the largest date.
			command=@"SELECT appointment.ClinicNum,appointment.AptDateTime 
				FROM appointment
				INNER JOIN recall ON appointment.PatNum=recall.PatNum AND recall.RecallNum="+POut.Long(recallNum)+@"
				WHERE appointment.AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.Complete)+")"+@"
				ORDER BY AptDateTime DESC";
			command=DbHelper.LimitOrderBy(command,1);
			long appointmentClinicNum=PIn.Long(DataCore.GetScalar(command));
			if(appointmentClinicNum>0) {
				return GetFirstOrDefault(x => x.ClinicNum==appointmentClinicNum);
			}
			return null;
		}

		///<summary>Gets a list of all clinics.  Doesn't use the cache.  Includes hidden clinics.</summary>
		public static List<Clinic> GetClinicsNoCache() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Clinic>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM clinic";
			return Crud.ClinicCrud.SelectMany(command);
		}

		///<summary>Returns an empty string for invalid clinicNum.  Will get results for hidden clinics too.</summary>
		public static string GetDesc(long clinicNum,List<Clinic> listClinics=null) {
			//No need to check RemotingRole; no call to db.
			Clinic clinic;
			if(listClinics==null) {//Use the cache
				clinic=GetFirstOrDefault(x => x.ClinicNum==clinicNum);
			}
			else {//Use the custom list passed in.
				clinic=listClinics.FirstOrDefault(x => x.ClinicNum==clinicNum);
			}
			return (clinic==null ? "" : clinic.Description);
		}

		///<summary>Returns an empty string for invalid clinicNums.  Will get results for hidden clinics too.</summary>
		public static string GetAbbr(long clinicNum,List<Clinic> listClinics=null) {
			//No need to check RemotingRole; no call to db.
			Clinic clinic;
			if(listClinics==null) {//Use the cache
				clinic=GetFirstOrDefault(x => x.ClinicNum==clinicNum);
			}
			else {//Use the custom list passed in.
				clinic=listClinics.FirstOrDefault(x => x.ClinicNum==clinicNum);
			}
			return (clinic==null ? "" : clinic.Abbr);
		}

		///<summary>Returns practice default for invalid clinicNums.  Will get results for hidden clinics too.</summary>
		public static PlaceOfService GetPlaceService(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			Clinic clinic=GetFirstOrDefault(x => x.ClinicNum==clinicNum);
			return (clinic==null ? (PlaceOfService)PrefC.GetLong(PrefName.DefaultProcedurePlaceService) : clinic.DefaultPlaceService);
		}

		///<summary>Used by HL7 when parsing incoming messages.  
		///Returns the ClinicNum of the clinic with Description matching exactly (not case sensitive) the description provided.  
		///Returns 0 if no clinic is found with this exact description.  
		///If there is more than one clinic with the same description, this will look for non-hidden ones first, or return the first one in the list.</summary>
		public static long GetByDesc(string description) {
			//No need to check RemotingRole; no call to db.
			//Check non-hidden clinics first.
			Clinic clinic=_clinicCache.GetFirstOrDefault(x => x.Description.ToLower()==description.ToLower(),true);
			//If no match found, check against hidden clinics.
			if(clinic==null) {
				clinic=_clinicCache.GetFirstOrDefault(x => x.Description.ToLower()==description.ToLower());
			}
			return (clinic==null) ? 0 : clinic.ClinicNum;
		}

		///<summary>Returns the Clinic's TimeZone. This must be set by the user or it will be null.</summary>
		public static TimeZoneInfo GetClinicTimeZone(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			if(clinicNum<=0) {
				return null;
			}
			TimeZoneInfo retval=null;
			try {
				retval=TimeZoneInfo.FindSystemTimeZoneById(GetClinicNoCache(clinicNum).TimeZone);
			}
			catch {
				//do nothing, return null if not found
			}
			return retval;
		}

		///<summary>Returns a list of clinics the curUser has permission to access.  
		///If the user is not restricted, the list will contain all of the clinics. Does NOT include hidden clinics (and never should anyway).
		///If doIncludeHQ is true, then it will also include a dummy HQ clinic with ClinicNum=0 using practice info, even if clinics are disabled.</summary>
		public static List<Clinic> GetForUserod(Userod curUser,bool doIncludeHQ=false, string hqClinicName = null) {
			List<Clinic> listClinics=new List<Clinic>();
			//Add HQ clinic if requested, even if clinics are disabled.  Counter-intuitive, but required for offices that had clinics enabled and then
			//turned them off.  If clinics are enabled and the user is restricted this will be filtered out below.
			if(doIncludeHQ) {
				listClinics.Add(GetPracticeAsClinicZero(hqClinicName));
			}
			listClinics.AddRange(GetDeepCopy(true));//don't include hidden clinics
			if(PrefC.HasClinicsEnabled && curUser.ClinicIsRestricted && curUser.ClinicNum!=0) {
				//If Clinics are enabled and user is restricted, then only return clinics the person has permission for.
				List<long> listUserClinicNums=UserClinics.GetForUser(curUser.UserNum).Select(x => x.ClinicNum).ToList();
				listClinics.RemoveAll(x => !listUserClinicNums.Contains(x.ClinicNum));//Remove all clinics that are not in the list of UserClinics.
			}
			return listClinics;
		}

		///<summary>Returns a list of clinics the curUser has permission to access, including hidden. If the user is not restricted, the list will contain all of the clinics.</summary>
		public static List<Clinic> GetAllForUserod(Userod curUser) {
			List<Clinic> listClinics=GetDeepCopy();
			if(!PrefC.HasClinicsEnabled) {
				return listClinics;
			}
			if(curUser.ClinicIsRestricted && curUser.ClinicNum!=0) {
				List<UserClinic> listUserClinics=UserClinics.GetForUser(curUser.UserNum);
				return listClinics.FindAll(x => listUserClinics.Exists(y => y.ClinicNum==x.ClinicNum)).ToList();
			}
			return listClinics;
		}

		///<summary>This method returns true if the given provider is set as the default clinic provider for any clinic.
		///Includes hidden Clinics.</summary>
		public static bool IsDefaultClinicProvider(long provNum) {
			//No need to check RemotingRole; no call to db.
			return (GetFirstOrDefault(x => x.DefaultProv==provNum)!=null);
		}

		///<summary>This method returns true if the given provider is set as the default ins billing provider for any clinic.
		///Includes hidden Clinics.</summary>
		public static bool IsInsBillingProvider(long provNum) {
			//No need to check RemotingRole; no call to db.
			return (GetFirstOrDefault(x => x.InsBillingProv==provNum)!=null);
		}

		///<summary>Gets the default clinic for texting. Returns null if no clinic is set as default.</summary>
		public static Clinic GetDefaultForTexting() {
			return GetFirstOrDefault(x => x.ClinicNum==PrefC.GetLong(PrefName.TextingDefaultClinicNum));
		}

		public static bool IsTextingEnabled(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			if(Plugins.HookMethod(null,"Clinics.IsTextingEnabled_start",clinicNum)) {
				return true;
			}
			if(clinicNum==0) {
				if(PrefC.HasClinicsEnabled) {
					clinicNum=PrefC.GetLong(PrefName.TextingDefaultClinicNum);
				}
				else {
					return SmsPhones.IsIntegratedTextingEnabled();
				}
			}
			Clinic clinic=GetClinic(clinicNum);
			if(clinic==null) {
				return false;//also handles clinicNum=0 which happens when default clinic not initialized.
			}
			return clinic.SmsContractDate.Year>1880;
		}

		///<summary>True when a clinic has activated Email Hosting, which means they have Email Hosting credentials.</summary>
		public static bool HasEmailHostingCredentials(long clinicNum) {			
			if(clinicNum==0) {
				return !string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.MassEmailGuid))
					&& !string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.MassEmailSecret));
			}
			else {
				return !string.IsNullOrWhiteSpace(ClinicPrefs.GetPref(PrefName.MassEmailGuid,clinicNum)?.ValueString)
					&& !string.IsNullOrWhiteSpace(ClinicPrefs.GetPref(PrefName.MassEmailSecret,clinicNum)?.ValueString);
			} 
		}

		public static bool IsMassEmailSignedUp(long clinicNum) {
			HostedEmailStatus status=GetEmailHostingStatus(PrefName.MassEmailStatus,clinicNum);
			return status.HasFlag(HostedEmailStatus.SignedUp) && HasEmailHostingCredentials(clinicNum);
		}

		///<summary>True when a clinic (or the practice) has activated and enabled mass emails.</summary>
		public static bool IsMassEmailEnabled(long clinicNum) {
			return IsMassEmailSignedUp(clinicNum) && GetEmailHostingStatus(PrefName.MassEmailStatus,clinicNum).HasFlag(HostedEmailStatus.Enabled);
		}
		
		public static bool IsSecureEmailSignedUp(long clinicNum) {
			HostedEmailStatus status=GetEmailHostingStatus(PrefName.EmailSecureStatus,clinicNum);
			return status.HasFlag(HostedEmailStatus.SignedUp) && HasEmailHostingCredentials(clinicNum);
		}

		///<summary>True when a clinic (or the practice) has activated and enabled secure emails.</summary>
		public static bool IsSecureEmailEnabled(long clinicNum) {
			return IsSecureEmailSignedUp(clinicNum) && GetEmailHostingStatus(PrefName.EmailSecureStatus,clinicNum).HasFlag(HostedEmailStatus.Enabled);
		}

		private static HostedEmailStatus GetEmailHostingStatus(PrefName prefName,long clinicNum) {			
			HostedEmailStatus emailHostingStatus;
			if(clinicNum==0) {
				emailHostingStatus=PrefC.GetEnum<HostedEmailStatus>(prefName);
			}
			else {
				//Does not default to the practice preference value if not found.  Intentional.
				emailHostingStatus=PIn.Enum<HostedEmailStatus>(ClinicPrefs.GetInt(prefName,clinicNum));
			}
			return emailHostingStatus;
		}

		///<summary>Provide the currently selected clinic num (FormOpenDental.ClinicNum).  If clinics are not enabled, this will return true if the pref
		///PracticeIsMedicalOnly is true.  If clinics are enabled, this will return true if either the headquarters 'clinic' is selected
		///(FormOpenDental.ClinicNum=0) and the pref PracticeIsMedicalOnly is true OR if the currently selected clinic's IsMedicalOnly flag is true.
		///Otherwise returns false.</summary>
		public static bool IsMedicalPracticeOrClinic(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			if(clinicNum==0) {//either headquarters is selected or the clinics feature is not enabled, use practice pref
				return PrefC.GetBool(PrefName.PracticeIsMedicalOnly);
			}
			Clinic clinicCur=Clinics.GetClinic(clinicNum);
			if(clinicCur!=null) {
				return clinicCur.IsMedicalOnly;
			}
			return false;
		}

		///<summary>Returns a clinic object with ClinicNum=0, and values filled using practice level preferences. 
		/// Caution: do not attempt to save the clinic back to the DB. This should be used for read only purposes.</summary>
		public static Clinic GetPracticeAsClinicZero(string clinicName = null) {
			//No need to check RemotingRole; no call to db.
			if(clinicName==null) {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			return new Clinic {
				ClinicNum=0,
				Abbr=clinicName,
				Description=clinicName,
				Address=PrefC.GetString(PrefName.PracticeAddress),
				Address2=PrefC.GetString(PrefName.PracticeAddress2),
				City=PrefC.GetString(PrefName.PracticeCity),
				State=PrefC.GetString(PrefName.PracticeST),
				Zip=PrefC.GetString(PrefName.PracticeZip),
				BillingAddress=PrefC.GetString(PrefName.PracticeBillingAddress),
				BillingAddress2=PrefC.GetString(PrefName.PracticeBillingAddress2),
				BillingCity=PrefC.GetString(PrefName.PracticeBillingCity),
				BillingState=PrefC.GetString(PrefName.PracticeBillingST),
				BillingZip=PrefC.GetString(PrefName.PracticeBillingZip),
				PayToAddress=PrefC.GetString(PrefName.PracticePayToAddress),
				PayToAddress2=PrefC.GetString(PrefName.PracticePayToAddress2),
				PayToCity=PrefC.GetString(PrefName.PracticePayToCity),
				PayToState=PrefC.GetString(PrefName.PracticePayToST),
				PayToZip=PrefC.GetString(PrefName.PracticePayToZip),
				Phone=PrefC.GetString(PrefName.PracticePhone),
				BankNumber=PrefC.GetString(PrefName.PracticeBankNumber),
				DefaultPlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService),
				InsBillingProv=PrefC.GetLong(PrefName.InsBillingProv),
				Fax=PrefC.GetString(PrefName.PracticeFax),
				EmailAddressNum=PrefC.GetLong(PrefName.EmailDefaultAddressNum),
				DefaultProv=PrefC.GetLong(PrefName.PracticeDefaultProv),
				SmsContractDate=PrefC.GetDate(PrefName.SmsContractDate),
				SmsMonthlyLimit=PrefC.GetDouble(PrefName.SmsMonthlyLimit,doUseEnUSFormat:true),
				IsMedicalOnly=PrefC.GetBool(PrefName.PracticeIsMedicalOnly)
			};
		}

		///<summary>Replaces all clinic fields in the given message with the supplied clinic's information.  Returns the resulting string.
		///Will use clinic information when available, otherwise defaults to practice info.
		///Replaces: [OfficePhone], [OfficeFax], [OfficeName], [OfficeAddress], and possibly [EmailDisclaimer]. </summary>
		public static string ReplaceOffice(string message,Clinic clinic,bool isHtmlEmail=false,bool doReplaceDisclaimer=false) {
			StringBuilder template=new StringBuilder(message);
			ReplaceOffice(template,clinic,isHtmlEmail,doReplaceDisclaimer);
			return template.ToString();
		}

		///<summary>Replaces all clinic fields in the given message with the supplied clinic's information.  Returns the resulting string.
		///Will use clinic information when available, otherwise defaults to practice info.
		///Replaces: [OfficePhone], [OfficeFax], [OfficeName], [OfficeAddress], and possibly [EmailDisclaimer]. </summary>
		public static void ReplaceOffice(StringBuilder template,Clinic clinic,bool isHtmlEmail=false,bool doReplaceDisclaimer=false) {
			string officePhone=GetOfficePhone(clinic);
			string officeName=GetOfficeName(clinic);
			string officeAddr=GetOfficeAddress(clinic);
			string officeFax=GetOfficeFax(clinic);
			ReplaceTags.ReplaceOneTag(template,"[OfficePhone]",officePhone,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[OfficeFax]",officeFax,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[OfficeName]",officeName,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[OfficeAddress]",officeAddr,isHtmlEmail);
			if(doReplaceDisclaimer) {
				ReplaceTags.ReplaceOneTag(template,"[EmailDisclaimer]",OpenDentBusiness.EmailMessages.GetEmailDisclaimer(clinic?.ClinicNum??0),isHtmlEmail);
			}
		}

		public static string GetOfficeName(Clinic clinic) {
			string officeName=clinic?.Description;
			if(string.IsNullOrEmpty(officeName)) {
				officeName=PrefC.GetString(PrefName.PracticeTitle);
			}
			return officeName;
		}

		public static string GetOfficeFax(Clinic clinic) {
			string officeFax=clinic?.Fax;
			if(string.IsNullOrEmpty(officeFax)) {
				officeFax=PrefC.GetString(PrefName.PracticeFax);
			}
			return TelephoneNumbers.ReFormat(officeFax);
		}

		public static string GetOfficePhone(Clinic clinic) {
			string officePhone=clinic?.Phone;
			if(string.IsNullOrEmpty(officePhone)) {
				officePhone=PrefC.GetString(PrefName.PracticePhone);
			}
			return TelephoneNumbers.ReFormat(officePhone);
		}

		public static string GetOfficeAddress(Clinic clinic) {
			if(clinic is null || clinic.ClinicNum==0 || string.IsNullOrWhiteSpace(clinic.Address)) {
				return Patients.GetAddressFull(
					PrefC.GetString(PrefName.PracticeAddress),
					PrefC.GetString(PrefName.PracticeAddress2),
					PrefC.GetString(PrefName.PracticeCity),
					PrefC.GetString(PrefName.PracticeST),
					PrefC.GetString(PrefName.PracticeZip)
				);
			}
			return Patients.GetAddressFull(clinic.Address,clinic.Address2,clinic.City,clinic.State,clinic.Zip);
		}
	}
	


}













