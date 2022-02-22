using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Fees {
		///<summary>When the fee cache is going to be filled for the first time by a thread, 
		///make everyone wait until _cache has been filled the first time.</summary>
		public static bool IsFilledByThread=false;

		#region Get Methods
		///<summary>Gets the list of fees by clinic num from the db.</summary>
		public static List<Fee> GetByClinicNum(long clinicNum) {
			return GetByClinicNums(new List<long>() { clinicNum });
		}

		///<summary>Gets the list of fees by clinic nums from the db.</summary>
		public static List<Fee> GetByClinicNums(List<long> listClinicNums) {
			if(listClinicNums.Count==0) {
				return new List<Fee>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * FROM fee WHERE ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			return Crud.FeeCrud.SelectMany(command);
		}
		
		///<summary>Gets the list of fees by feeschednums and clinicnums from the db.  Returns an empty list if listFeeSchedNums is null or empty.
		///Throws an application exception if listClinicNums is null or empty.  Always provide at least one ClinicNum.
		///We throw instead of returning an empty list which would make it look like there are no fees for the fee schedules passed in.
		///If this method returns an empty list it is because no valied fee schedules were given or the database truly doesn't have any fees.</summary>
		public static List<FeeLim> GetByFeeSchedNumsClinicNums(List<long> listFeeSchedNums,List<long> listClinicNums) {
			if(listFeeSchedNums==null || listFeeSchedNums.Count==0) {
				return new List<FeeLim>();//This won't hurt the FeeCache because there will be no corresponding fee schedules to "blank out".
			}
			if(listClinicNums==null || listClinicNums.Count==0) {
				//Returning an empty list here would be detrimental to the FeeCache.
				throw new ApplicationException("Invalid listClinicNums passed into GetByFeeSchedNumsClinicNums()");
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				//Unusual Middle Tier check. This method can cause out of memory exceptions when called over Middle Tier, so we are batching into multiple
				//calls of 100 fee schedules at a time.
				List<FeeLim> listFeeLims=new List<FeeLim>();
				for(int i=0;i<listFeeSchedNums.Count;i+=100) {
					List<long> listFeeSchedsNumsThisBatch=listFeeSchedNums.GetRange(i,Math.Min(100,listFeeSchedNums.Count-i));
					listFeeLims.AddRange(Meth.GetObject<List<FeeLim>>(MethodBase.GetCurrentMethod(),listFeeSchedsNumsThisBatch,listClinicNums));
				}
				return listFeeLims;
			}
			string command="SELECT FeeNum,Amount,FeeSched,CodeNum,ClinicNum,ProvNum,SecDateTEdit FROM fee "
				+"WHERE FeeSched IN ("+string.Join(",",listFeeSchedNums.Select(x => POut.Long(x)))+") "
				+"AND ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			return Db.GetTable(command).AsEnumerable()
				.Select(x => new FeeLim {
					FeeNum=PIn.Long(x["FeeNum"].ToString()),
					Amount=PIn.Double(x["Amount"].ToString()),
					FeeSched=PIn.Long(x["FeeSched"].ToString()),
					CodeNum=PIn.Long(x["CodeNum"].ToString()),
					ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
					ProvNum=PIn.Long(x["ProvNum"].ToString()),
					SecDateTEdit=PIn.DateT(x["SecDateTEdit"].ToString()),
				}).ToList();
		}

		///<summary>Counts the number of fees in the db for this fee sched, including all clinic and prov overrides.</summary>
		public static int GetCountByFeeSchedNum(long feeSchedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),feeSchedNum);
			}
			string command="SELECT COUNT(*) FROM fee WHERE FeeSched ="+POut.Long(feeSchedNum);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Searches for the given codeNum and feeSchedNum and finds the most appropriate match for the clinicNum and provNum.  If listFees is null, it will go to db.</summary>
		public static Fee GetFee(long codeNum,long feeSchedNum,long clinicNum,long provNum,List<Fee> listFees=null) {
			//No need to check RemotingRole; no call to db.
			//use listFees if supplied regardless of the FeesUseCache pref since the fee cache is not really thread safe
			if(listFees!=null) {
				return GetFeeFromList(listFees,codeNum,feeSchedNum,clinicNum,provNum);
			}
			return GetFeeFromDb(codeNum,feeSchedNum,clinicNum,provNum);
		}

		///<summary>Searches the db for a fee with the exact codeNum, feeSchedNum, clinicNum, and provNum provided.  Returns null if no exact match found.
		///The goal of this method is to have a way to check the database for "duplicate" fees before adding more fees to the db. Set doGetExactMatch to
		///true to exactly match all passed in parameters.</summary>
		public static Fee GetFeeFromDb(long codeNum,long feeSchedNum,long clinicNum=0,long provNum=0,bool doGetExactMatch=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Fee>(MethodBase.GetCurrentMethod(),codeNum,feeSchedNum,clinicNum,provNum,doGetExactMatch);
			}
			if(FeeScheds.IsGlobal(feeSchedNum) && !doGetExactMatch) {
				clinicNum=0;
				provNum=0;
			}
			//If the logic changes here, then we need to change FeeCache.GetFee.
			string command=
				//Search for exact match first.  This would include a clinic and provider override.
				@"SELECT fee.*
				FROM fee
				WHERE fee.CodeNum="+POut.Long(codeNum)+@"
				AND fee.FeeSched="+POut.Long(feeSchedNum)+@"
				AND fee.ClinicNum="+POut.Long(clinicNum)+@"
				AND fee.ProvNum="+POut.Long(provNum);
			if(!doGetExactMatch) {
				//Provider override 
				command+=@"
					UNION ALL
					SELECT fee.*
					FROM fee
					WHERE fee.CodeNum="+POut.Long(codeNum)+@"
					AND fee.FeeSched="+POut.Long(feeSchedNum)+@"
					AND fee.ClinicNum=0
					AND fee.ProvNum="+POut.Long(provNum);
				//Clinic override
				command+=@"
					UNION ALL
					SELECT fee.*
					FROM fee
					WHERE fee.CodeNum="+POut.Long(codeNum)+@"
					AND fee.FeeSched="+POut.Long(feeSchedNum)+@"
					AND fee.ClinicNum="+POut.Long(clinicNum)+@"
					AND fee.ProvNum=0";
				//Unassigned clinic with no override
				command+=@"
					UNION ALL
					SELECT fee.*
					FROM fee
					WHERE fee.CodeNum="+POut.Long(codeNum)+@"
					AND fee.FeeSched="+POut.Long(feeSchedNum)+@"
					AND fee.ClinicNum=0
					AND fee.ProvNum=0";
			}
			return Crud.FeeCrud.SelectOne(command);
		}

		///<summary>Same logic as above, in Fees.GetFeeNoCache().  Also, same logic as in FeeCache.GetFee().
		///Typical to pass in a list of fees for just one or a few feescheds so that the search goes quickly.
		///When doGetExactMatch is true, this will return either the fee that matches the parameters exactly, or null if no such fee exists.
		///When doGetExactMatch is false, and the fee schedule is global, we ignore the clinicNum and provNum and return the HQ fee that matches the given codeNum and feeSchedNum.
		///When doGetExactMatch is false, and the fee schedule is not global, and no exact match exists we attempt to return the closest matching fee in this order:
		///1 - The fee with the same codeNum, feeSchedNum, and providerNum, with a clinicNum of 0
		///2 - The fee with the same codeNum, feeSchedNum, and clinicNum, with a providerNum of 0
		///3 - The fee with the same codeNum, feeSchedNum, and both a clinicNum and providerNum of 0
		///If no partial match can be found, return null.</summary>
		private static Fee GetFeeFromList(List<Fee> listFees,long codeNum,long feeSched=0,long clinicNum=0,long provNum=0,bool doGetExactMatch=false){
			//No need to check RemotingRole; no call to db.
			if(FeeScheds.IsGlobal(feeSched) && !doGetExactMatch) {//speed things up here with less loops
				clinicNum=0;
				provNum=0;
			}
			Fee fee=listFees.Find(f => f.CodeNum==codeNum && f.FeeSched==feeSched && f.ClinicNum==clinicNum && f.ProvNum==provNum);
			if(fee!=null){
				return fee;//match found.  Would include a clinic and provider override.
			}
			if(doGetExactMatch || FeeScheds.IsGlobal(feeSched)) {
				return null;//couldn't find exact match
			}
			//no exact match exists, so we look for closest match
			//2: Prov override
			fee=listFees.Find(f => f.CodeNum==codeNum && f.FeeSched==feeSched && f.ClinicNum==0 && f.ProvNum==provNum);
			if(fee!=null){
				return fee;
			}
			//3: Clinic override
			fee=listFees.Find(f => f.CodeNum==codeNum && f.FeeSched==feeSched && f.ClinicNum==clinicNum && f.ProvNum==0);
			if(fee!=null){
				return fee;
			}
			//4: Just unassigned clinic default
			fee=listFees.Find(f => f.CodeNum==codeNum && f.FeeSched==feeSched && f.ClinicNum==0 && f.ProvNum==0);
			//whether it's null or not:
			return fee;
		}

		public static List<Fee> GetListExactForClinicsFromDb(long codeNum,long feeSchedNum,long provNum,List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),codeNum,feeSchedNum,provNum,listClinicNums);
			}
			string command=$@"SELECT fee.* FROM fee
				WHERE fee.CodeNum={POut.Long(codeNum)}
				AND fee.FeeSched={POut.Long(feeSchedNum)}
				AND fee.ProvNum={POut.Long(provNum)}{(listClinicNums.IsNullOrEmpty()?"":$@"
				AND fee.ClinicNum IN({string.Join(",",listClinicNums.Select(POut.Long))})")}";
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Gets fees for up to three feesched/clinic/prov combos. If filtering with a ClinicNum and/or ProvNum, it only includes fees that match that clinicNum/provnum or have zero.  This reduces the result set if there are clinic or provider overrides. This could easily scale to many thousands of clinics and providers.</summary>
		public static List<Fee> GetListForScheds(long feeSched1,long clinic1=0,long prov1=0,long feeSched2=0,long clinic2=0,long prov2=0,long feeSched3=0,long clinic3=0,long prov3=0){
			return GetListForSchedsAndClinics(feeSched1,new List<long> { clinic1 },prov1,feeSched2,new List<long> { clinic2 },prov2,feeSched3,new List<long> { clinic3 },prov3);
		}
		
		///<summary>Gets fees for up to three feesched/clinic/prov combos. If filtering with a ClinicNum and/or ProvNum, it only includes fees that match that clinicNum/provnum or have zero.  This reduces the result set if there are clinic or provider overrides. This could easily scale to many thousands of clinics and providers.</summary>
		public static List<Fee> GetListForSchedsAndClinics(long feeSched1,List<long> listClinics1=null,long prov1=0,long feeSched2=0,List<long> listClinics2=null,
			long prov2=0,long feeSched3=0,List<long> listClinics3=null,long prov3=0)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),feeSched1,listClinics1,prov1,feeSched2,listClinics2,prov2,feeSched3,listClinics3,prov3);
			}
			string command="SELECT * FROM fee WHERE "
				+"(FeeSched="+POut.Long(feeSched1)+" AND ClinicNum IN(0"+(listClinics1.IsNullOrEmpty()?"":(","+string.Join(",",listClinics1)))+") AND ProvNum IN(0,"+POut.Long(prov1)+"))";
			if(feeSched2!=0){
				command+=" OR (FeeSched="+POut.Long(feeSched2)+" AND ClinicNum IN(0"+(listClinics2.IsNullOrEmpty()?"":(","+string.Join(",",listClinics2)))+") AND ProvNum IN(0,"+POut.Long(prov2)+"))";
			}
			if(feeSched3!=0){
				command+=" OR (FeeSched="+POut.Long(feeSched3)+" AND ClinicNum IN(0"+(listClinics3.IsNullOrEmpty()?"":(","+string.Join(",",listClinics3)))+") AND ProvNum IN(0,"+POut.Long(prov3)+"))";
			}
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Gets all possible fees associated with the various objects passed in.  Gets fees from db based on code and fee schedule combos.  Includes all provider overrides.  Includes default/no clinic as well as any specified clinic overrides. Although the list always includes extra fees from scheds that we don't need, it's still a very small list.  That list is then used repeatedly by other code in loops to find the actual individual fee amounts.</summary>
		public static List<Fee> GetListFromObjects(List<ProcedureCode> listProcedureCodes,List<string> listMedicalCodes,List<long> listProvNumsTreat,long patPriProv,
			long patSecProv,long patFeeSched,List<InsPlan> listInsPlans,List<long> listClinicNums,List<Appointment> listAppts,
			List<SubstitutionLink> listSubstLinks,long discountPlan
			//listCodeNums,listProvNumsTreat,listProcCodesProvNumDefault,patPriProv,patSecProv,patFeeSched,listInsPlans,listClinicNums
			//List<long> listProcCodesProvNumDefault
			)
		{
			//listMedicalCodes: it already automatically gets the medical codes from procCodes.  This is just for procs. If no procs yet, it will be null.
			//listMedicalCodes can be done by: listProcedures.Select(x=>x.MedicalCode).ToList();  //this is just the strings
			//One way to get listProvNumsTreat is listProcedures.Select(x=>x.ProvNum).ToList()
			//One way to specify a single provNum in listProvNumsTreat is new List<long>(){provNum}
			//One way to get clinicNums is listProcedures.Select(x=>x.ClinicNum).ToList()
			//Another way to get clinicNums is new List<long>(){clinicNum}.
			//These objects will be cleaned up, so they can have duplicates, zeros, invalid keys, nulls, etc
			//In some cases, we need to pass in a list of appointments to make sure we've included all possible providers, both ProvNum and ProvHyg
			//In that case, it's common to leave listProvNumsTreat null because we clearly do not have any of those providers set yet.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),listProcedureCodes,listMedicalCodes,listProvNumsTreat,patPriProv,
					patSecProv,patFeeSched,listInsPlans,listClinicNums,listAppts,listSubstLinks,discountPlan);
			}
			if(listProcedureCodes==null){
				return new List<Fee>();
			}
			List<long> listCodeNumsOut=new List<long>();
			foreach(ProcedureCode procedureCode in listProcedureCodes){
				if(procedureCode==null){
					continue;
				}
				if(!listCodeNumsOut.Contains(procedureCode.CodeNum)){
					listCodeNumsOut.Add(procedureCode.CodeNum);
				}
				if(ProcedureCodes.IsValidCode(procedureCode.MedicalCode)){
					long codeNumMed=ProcedureCodes.GetCodeNum(procedureCode.MedicalCode);
					if(!listCodeNumsOut.Contains(codeNumMed)){
						listCodeNumsOut.Add(codeNumMed);
					}
				}
				if(ProcedureCodes.IsValidCode(procedureCode.SubstitutionCode)) {
					long codeNumSub=ProcedureCodes.GetCodeNum(procedureCode.SubstitutionCode);
					if(!listCodeNumsOut.Contains(codeNumSub)) {
						listCodeNumsOut.Add(codeNumSub);
					}
				}
			}
			if(listMedicalCodes!=null){
				foreach(string strMedCode in listMedicalCodes){
					if(ProcedureCodes.IsValidCode(strMedCode)){
						long codeNumMed=ProcedureCodes.GetCodeNum(strMedCode);
						if(!listCodeNumsOut.Contains(codeNumMed)){
							listCodeNumsOut.Add(codeNumMed);
						}
					}
				}
			}
			if(listSubstLinks!=null) {
				foreach(SubstitutionLink substitutionLink in listSubstLinks){//Grab all subst codes, since we don't know which ones we will need.
					if(ProcedureCodes.IsValidCode(substitutionLink.SubstitutionCode)){
						long codeNum=ProcedureCodes.GetCodeNum(substitutionLink.SubstitutionCode);
						if(!listCodeNumsOut.Contains(codeNum)){
							listCodeNumsOut.Add(codeNum);
						}
					}
				}
			}
			//Fee schedules. Will potentially include many.=======================================================================================
			List<long> listFeeScheds=new List<long>();
			//Add feesched for first provider (See Claims.CalculateAndUpdate)---------------------------------------------------------------------
			Provider provFirst=Providers.GetFirst();
			if(provFirst!=null && provFirst.FeeSched!=0 && !listFeeScheds.Contains(provFirst.FeeSched)){
				listFeeScheds.Add(provFirst.FeeSched);
			}
			//Add feesched for PracticeDefaultProv------------------------------------------------------------------------------------------------
			Provider provPracticeDefault=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			if(provPracticeDefault!=null && provPracticeDefault.FeeSched!=0 && !listFeeScheds.Contains(provPracticeDefault.FeeSched)){
				listFeeScheds.Add(provPracticeDefault.FeeSched);
			}
			//Add feescheds for all treating providers---------------------------------------------------------------------------------------------
			if(listProvNumsTreat!=null){
				foreach(long provNumTreat in listProvNumsTreat){
					Provider provTreat=Providers.GetProv(provNumTreat);
					if(provTreat!=null && provTreat.FeeSched!=0 && !listFeeScheds.Contains(provTreat.FeeSched)){
						listFeeScheds.Add(provTreat.FeeSched);//treating provs fee scheds
					}
				}
			}
			//Add feescheds for the patient's primary and secondary providers----------------------------------------------------------------------
			Provider providerPatPri=Providers.GetProv(patPriProv);
			if(providerPatPri!=null && providerPatPri.FeeSched!=0 && !listFeeScheds.Contains(providerPatPri.FeeSched)){
				listFeeScheds.Add(providerPatPri.FeeSched);
			}
			Provider providerPatSec=Providers.GetProv(patSecProv);
			if(providerPatSec!=null && providerPatSec.FeeSched!=0 && !listFeeScheds.Contains(providerPatSec.FeeSched)){
				listFeeScheds.Add(providerPatSec.FeeSched);
			}
			//Add feescheds for all procedurecode.ProvNumDefaults---------------------------------------------------------------------------------
			foreach(ProcedureCode procedureCode in listProcedureCodes){
				if(procedureCode==null){
					continue;
				}
				long provNumDefault=procedureCode.ProvNumDefault;
				if(provNumDefault==0){
					continue;
				}
				Provider provDefault=Providers.GetProv(provNumDefault);
				if(provDefault!=null && provDefault.FeeSched!=0 && !listFeeScheds.Contains(provDefault.FeeSched)){
					listFeeScheds.Add(provDefault.FeeSched);
				}
			}
			//Add feescheds for appointment providers---------------------------------------------------------------------------------------------
			if(listAppts!=null){
				foreach(Appointment appointment in listAppts){
					Provider provAppt=Providers.GetProv(appointment.ProvNum);
					if(provAppt!=null && provAppt.FeeSched!=0 && !listFeeScheds.Contains(provAppt.FeeSched)){
						listFeeScheds.Add(provAppt.FeeSched);
					}
					Provider provApptHyg=Providers.GetProv(appointment.ProvHyg);
					if(provApptHyg!=null && provApptHyg.FeeSched!=0 && !listFeeScheds.Contains(provApptHyg.FeeSched)){
						listFeeScheds.Add(provApptHyg.FeeSched);
					}
				}
			}
			//Add feesched for patient.  Rare. --------------------------------------------------------------------------------------------------
			if(patFeeSched!=0){
				if(!listFeeScheds.Contains(patFeeSched)){
					listFeeScheds.Add(patFeeSched);
				}
			}
			//Add feesched for each insplan, both reg and allowed, and Manual Blue Book---------------------------------------------------------------------
			if(listInsPlans!=null){
				foreach(InsPlan insPlan in listInsPlans){
					if(insPlan.FeeSched!=0 && !listFeeScheds.Contains(insPlan.FeeSched)){
						listFeeScheds.Add(insPlan.FeeSched);//insplan feeSched
					}
					if(insPlan.AllowedFeeSched!=0 && !listFeeScheds.Contains(insPlan.AllowedFeeSched)){
						listFeeScheds.Add(insPlan.AllowedFeeSched);//allowed feeSched
					}
					if(insPlan.CopayFeeSched!=0 && !listFeeScheds.Contains(insPlan.CopayFeeSched)) {
						listFeeScheds.Add(insPlan.CopayFeeSched);//copay feeSched
					}
					if(insPlan.ManualFeeSchedNum!=0 && !listFeeScheds.Contains(insPlan.ManualFeeSchedNum)) {
						listFeeScheds.Add(insPlan.ManualFeeSchedNum);//manual blue book feeSched
					}
				}
			}
			if(discountPlan!=0) {
				long discountPlanFeeSched=DiscountPlans.GetPlan(discountPlan).FeeSchedNum;
				if(!listFeeScheds.Contains(discountPlanFeeSched)) {
					listFeeScheds.Add(discountPlanFeeSched);
				}
			}
			//ClinicNums========================================================================================================================
			List<long> listClinicNumsOut=new List<long>();//usually empty or one entry
			if(listClinicNums!=null){
				foreach(long clinicNum in listClinicNums){
					if(clinicNum!=0 && !listClinicNumsOut.Contains(clinicNum)){
						listClinicNumsOut.Add(clinicNum);//proc ClinicNums
					}
				}
			}
			if(listFeeScheds.Count==0 || listProcedureCodes.Count==0){
				return new List<Fee>();
			}
			string command="SELECT * FROM fee WHERE ClinicNum IN(0"
				+$@"{(listClinicNumsOut.Count==0?"":(","+string.Join(",",listClinicNumsOut.Select(x => POut.Long(x)))))}){(listFeeScheds.Count==0?"":$@"
				AND FeeSched IN({string.Join(",",listFeeScheds.Select(x => POut.Long(x)))})")}{(listCodeNumsOut.Count==0?"":$@"
				AND CodeNum IN({string.Join(",",listCodeNumsOut.Select(x => POut.Long(x)))})")}";
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Gets fees that exactly match criteria.</summary>
		public static List<Fee> GetListExact(long feeSched,long clinicNum,long provNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),feeSched,clinicNum,provNum);
			}
			string command="SELECT * FROM fee WHERE "
				+"FeeSched="+POut.Long(feeSched)+" AND ClinicNum="+POut.Long(clinicNum)+" AND ProvNum="+POut.Long(provNum)+" "
				+"GROUP BY CodeNum";//There should not be duplicates, but group by codeNum, just in case.
			return Crud.FeeCrud.SelectMany(command);
		}
		
		///<summary>Overload gets fees that exactly match criteria with a clinic list.</summary>
		public static List<Fee> GetListExact(long feeSched,List<long> listClinicNums,long provNum){
			if(listClinicNums.IsNullOrEmpty()) {
				return new List<Fee>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),feeSched,listClinicNums,provNum);
			}
			string command="SELECT * FROM fee WHERE "
				+"FeeSched="+POut.Long(feeSched)+" AND ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))
				+") AND ProvNum="+POut.Long(provNum);
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Pass in new list and original list.  This will synch everything with Db.  Leave doCheckFeeSchedGroups set to true unless calling from 
		///FeeSchedGroups.</summary>
		public static bool SynchList(List<Fee> listNew,List<Fee> listDB,bool doCheckFeeSchedGroups=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listDB,doCheckFeeSchedGroups);
			}
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				FeeSchedGroups.SyncGroupFees(listNew,listDB);
			}
			return Crud.FeeCrud.Sync(listNew,listDB,0);
		}

		///<summary>Gets from Db.  Returns all fees associated to the procedure code passed in.</summary>
		public static List<Fee> GetFeesForCode(long codeNum,List<long> listClinicNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),codeNum,listClinicNums);
			}
			string command="SELECT * FROM fee WHERE CodeNum="+POut.Long(codeNum)+" ";
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			}
			//ordering was being done in the form. Easier to do it here.
			command+=" ORDER BY ClinicNum,ProvNum";
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Gets fees from Db, not including any prov or clinic overrides.</summary>
		public static List<Fee> GetFeesForCodeNoOverrides(long codeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),codeNum);
			}
			string command="SELECT * FROM fee WHERE CodeNum="+POut.Long(codeNum)+" "
				+"AND ClinicNum=0 AND ProvNum=0";
			return Crud.FeeCrud.SelectMany(command);
		}

		///<summary>Returns an amount if a fee has been entered.  Prefers local clinic fees over HQ fees.  Otherwise returns -1.
		///Not usually used directly.  If you don't pass in a list of Fees, it will go directly to the Db.</summary>
		public static double GetAmount(long codeNum,long feeSchedNum,long clinicNum,long provNum,List<Fee> listFees=null) {
			//No need to check RemotingRole; no call to db.
			if(FeeScheds.GetIsHidden(feeSchedNum)) {
				return -1;//you cannot obtain fees for hidden fee schedules
			}
			Fee fee=GetFee(codeNum,feeSchedNum,clinicNum,provNum,listFees);
			if(fee==null) {
				return -1;
			}
			return fee.Amount;
		}

		///<summary>Almost the same as GetAmount.  But never returns -1;  Returns an amount if a fee has been entered.  
		///Prefers local clinic fees over HQ fees. Returns 0 if code can't be found.
		///If you don't pass in a list of fees, it will go directly to the database.</summary>
		public static double GetAmount0(long codeNum,long feeSched,long clinicNum=0,long provNum=0,List<Fee> listFees=null) {
			//No need to check RemotingRole; no call to db.
			double retVal=GetAmount(codeNum,feeSched,clinicNum,provNum,listFees);
			if(retVal==-1) {
				return 0;
			}
			return retVal;
		}

		

		///<summary>Gets the UCR fee for the provided procedure.</summary>
		public static double GetFeeUCR(Procedure proc) {
			//No need to check RemotingRole; no call to db.
			long provNum=proc.ProvNum;
			if(provNum==0) {//if no prov set, then use practice default.
				provNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider provider=Providers.GetFirstOrDefault(x => x.ProvNum==provNum)??providerFirst;
			//get the fee based on code and prov fee sched
			double ppoFee=GetAmount0(proc.CodeNum,provider.FeeSched,proc.ClinicNum,provNum);
			double ucrFee=proc.ProcFee;
			if(ucrFee > ppoFee) {
				return proc.Quantity * ucrFee;
			}
			else {
				return proc.Quantity * ppoFee;
			}
		}

		public static List<Fee> GetManyByFeeNum(List<long> listFeeNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Fee>>(MethodBase.GetCurrentMethod(),listFeeNums);
			}
			string command="SELECT * FROM fee WHERE FeeNum IN ("+string.Join(",",listFeeNums)+")";
			return Crud.FeeCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Insert
		///<summary>Set doCheckFeeSchedGroups to false when calling this method from FeeSchedGroups to prevent infinitely looping.</summary>
		public static long Insert(Fee fee,bool doCheckFeeSchedGroups=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				fee.FeeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),fee,doCheckFeeSchedGroups);
				return fee.FeeNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			fee.SecUserNumEntry=Security.CurUser.UserNum;
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				FeeSchedGroups.UpsertGroupFees(new List<Fee>() { fee });
			}
			return Crud.FeeCrud.Insert(fee);
		}

		/// <summary>Bulk Insert.  Only set doCheckFeeSchedGroups to false from FeeSchedGroups.</summary>
		public static void InsertMany(List<Fee> listFees,bool doCheckFeeSchedGroups=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFees,doCheckFeeSchedGroups);
				return;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listFees.ForEach(x => x.SecUserNumEntry=Security.CurUser.UserNum);
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				FeeSchedGroups.UpsertGroupFees(listFees);
			}
			Crud.FeeCrud.InsertMany(listFees);
		}
		#endregion Insert

		#region Update
		///<summary>Only set doCheckFeeSchedGroups to false from FeeSchedGroups.</summary>
		public static void Update(Fee fee,Fee oldFee=null,bool doCheckFeeSchedGroups=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fee,oldFee,doCheckFeeSchedGroups);
				return;
			}
			//Check if this fee is associated to a FeeSchedGroup and update the rest of the group as needed.
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				FeeSchedGroups.UpsertGroupFees(new List<Fee>() { fee });
			}
			if(oldFee!=null) {
				Crud.FeeCrud.Update(fee,oldFee);
			}
			else {
				Crud.FeeCrud.Update(fee);
			}
		}
		#endregion Update

		#region Delete
		///<summary>Only set doCheckFeeSchedGroups to false from FeeSchedGroups.</summary>
		public static void Delete(Fee fee,bool doCheckFeeSchedGroups=true) {
			//Even though we do not run a query in this method, there is a lot of back and forth and we should get to the server early to ensure less chattiness.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fee,doCheckFeeSchedGroups);
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				//If this fee isn't in a group don't bother checking.
				if(FeeSchedGroups.GetOneForFeeSchedAndClinic(fee.FeeSched,fee.ClinicNum)!=null) {
					FeeSchedGroups.DeleteGroupFees(new List<long>() { fee.FeeNum });
				}
			}
			Delete(fee.FeeNum);
		}

		///<summary></summary>
		public static void Delete(long feeNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeNum);
				return;
			}
			ClearFkey(feeNum);
			string command="DELETE FROM fee WHERE FeeNum="+feeNum;
			Db.NonQ(command);
		}

		///<summary>Only set doCheckFeeSchedGroups to false from FeeSchedGroups.</summary>
		public static void DeleteMany(List<long> listFeeNums,bool doCheckFeeSchedGroups=true) {
			if(listFeeNums.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFeeNums,doCheckFeeSchedGroups);
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && doCheckFeeSchedGroups) {
				FeeSchedGroups.DeleteGroupFees(listFeeNums);
			}
			ClearFkey(listFeeNums);
			string command="DELETE FROM fee WHERE FeeNum IN ("+string.Join(",",listFeeNums)+")";
			Db.NonQ(command);
		}

		///<summary>Deletes all fees for the supplied FeeSched that aren't for the HQ clinic.</summary>
		public static void DeleteNonHQFeesForSched(long feeSchedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSchedNum);
				return;
			}
			string command="SELECT FeeNum FROM fee WHERE FeeSched="+POut.Long(feeSchedNum)+" AND ClinicNum!=0";
			List<long> listFeeNums=Db.GetListLong(command);
			DeleteMany(listFeeNums);
		}

		/// <summary>Deletes all fees with the exact specified FeeSchedule, ClinicNum, and ProvNum combination.</summary>
		public static void DeleteFees(long feeSched,long clinicNum,long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSched,clinicNum,provNum);
				return;
			}
			string command="DELETE FROM fee WHERE "
				+"FeeSched="+POut.Long(feeSched)+" AND ClinicNum="+POut.Long(clinicNum)+" AND ProvNum="+POut.Long(provNum);
			Db.NonQ(command);
		}
		#endregion Delete

		#region Misc Methods
	

		///<summary>Increases the fees passed in by percent.  Round should be the number of decimal places, either 0,1,or 2.
		///This method will not manipulate listFees passed in, although there is no particular reason for this choice.
		///Simply increases every fee passed in by the percent specified and returns the results.
		///The following parameters are ignored: feeSchedNum, clinicNum, and provNum.</summary>
		public static List<Fee> IncreaseNew(long feeSchedNum,int percent,int round,List<Fee> listFees,long clinicNum,long provNum) {
			//No need to check RemotingRole; no call to db.
			List<Fee> listFeesRetVal=new List<Fee>();
			foreach(Fee fee in listFees) {
				if(fee.Amount==0 || fee.Amount==-1){
					listFeesRetVal.Add(fee.Copy());
					continue;
				}
				double newVal=(double)fee.Amount*(1+(double)percent/100);
				if(round>0) {
					newVal=Math.Round(newVal,round);
				}
				else {
					newVal=Math.Round(newVal,MidpointRounding.AwayFromZero);
				}
				Fee feeNew=fee.Copy();
				feeNew.Amount=newVal;
				listFeesRetVal.Add(feeNew);
			}
			return listFeesRetVal;
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching feeNum as FKey and are related to Fee.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Fee table type.</summary>
		public static void ClearFkey(long feeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeNum);
				return;
			}
			Crud.FeeCrud.ClearFkey(feeNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching feeNums as FKey and are related to Fee.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Fee table type.</summary>
		public static void ClearFkey(List<long> listFeeNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFeeNums);
				return;
			}
			Crud.FeeCrud.ClearFkey(listFeeNums);
		}

		///<summary>Returns true if the feeAmtNewStr is an amount that does not match fee, either because fee is null and feeAmtNewStr is not, or because
		///fee not null and the feeAmtNewStr is an equal amount, including a blank entry.</summary>
		public static bool IsFeeAmtEqual(Fee fee,string feeAmtNewStr) {
			//There is no fee in the database and the user didn't set a new fee value so there is no change.
			if(fee==null && feeAmtNewStr=="") {
				return true;
			}
			//Fee exists, but new amount is the same.
			if(fee!=null && (feeAmtNewStr!="" && fee.Amount==PIn.Double(feeAmtNewStr) || (fee.Amount==-1 && feeAmtNewStr==""))) {
				return true;
			}
			return false;
		}
		#endregion Misc Methods
	}

	///<summary>A class with a fee and an update type. 
	///Used by the FeeCache, to keep an in-memory list of pending changes for saving to the db.</summary>
	public class FeeUpdate {
		public Fee Fee {get;set;}
		/// <summary>Indicates whether the record is an Add, Update, or Delete</summary>
		public FeeUpdateType UpdateType {get;set;}

		///<summary>For serialization.</summary>
		public FeeUpdate() { }

		///<summary></summary>
		public FeeUpdate(Fee fee, FeeUpdateType updateType) {
			Fee=fee;
			UpdateType=updateType;
		}

		///<summary></summary>
		public FeeUpdate Copy() {
			return new FeeUpdate(Fee.Copy(),UpdateType);
		}
	}

	public enum FeeUpdateType {
		Add,
		Update,
		Remove
	}
}