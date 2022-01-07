using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness {
	///<summary></summary>
	public class InsEditLogs {
		///<summary>Gets logs from the passed in datetime and before.</summary>
		public static List<InsEditLog> GetLogsForPlan(long planNum,long carrierNum,long employerNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsEditLog>>(MethodBase.GetCurrentMethod(),planNum,carrierNum,employerNum);
			}
			List<long> listCarrierNums=InsEditLogs.GetAssociatedCarrierNums(planNum);
			listCarrierNums.Add(carrierNum);
			List<InsEditLog> retVal=new List<InsEditLog>();
			string command=@"SELECT PlanNum FROM insplan WHERE PlanNum = "+POut.Long(planNum);
			long insPlanNum=Db.GetLong(command);
			command=@"SELECT CarrierNum FROM carrier WHERE CarrierNum IN ("+string.Join(",",listCarrierNums)+@")";
			listCarrierNums=Db.GetListLong(command);
			command=@"SELECT EmployerNum FROM employer WHERE EmployerNum="+POut.Long(employerNum);
			long empNum=Db.GetLong(command);
			List<string> listWhereOrs=new List<string>();
			if(insPlanNum>0) {
				listWhereOrs.Add("(LogType="+POut.Int((int)InsEditLogType.InsPlan)+" AND FKey = "+POut.Long(insPlanNum)+")");
			}
			if(listCarrierNums.Count>0) {
				listWhereOrs.Add("(LogType="+POut.Int((int)InsEditLogType.Carrier)+" AND FKey IN ("+string.Join(",",listCarrierNums)+"))");
			}
			if(empNum>0) {
				listWhereOrs.Add("(LogType="+POut.Int((int)InsEditLogType.Employer)+" AND FKey="+POut.Long(empNum)+")");
			}
			listWhereOrs.Add("(LogType="+POut.Int((int)InsEditLogType.Benefit)+" AND ParentKey="+POut.Long(planNum)+")");
			command=@"SELECT * FROM inseditlog
				WHERE "+string.Join(@"
				OR ",listWhereOrs);
			List<InsEditLog> listLogs=Crud.InsEditLogCrud.SelectMany(command);//get all of the logs
			List<InsVerifyHist> listInsVerifyHists=InsVerifyHists.GetForFKeyByType(planNum,VerifyTypes.InsuranceBenefit)
				.OrderByDescending(x => x.SecDateTEdit)
				//Before 19.3.32, SecDateTEdit was not getting set. We'll use InsVerifyHistNum as a fallback.
				.ThenByDescending(x => x.InsVerifyHistNum).ToList();
			InsVerify insVerifyCur=InsVerifies.GetOneByFKey(planNum,VerifyTypes.InsuranceBenefit);
			//The most recent inserted insverifyhist will be the current insverify. Go through the list of insverifyhist and construct inseditlog objects.
			//These inseditlogs will not get inserted into the db. 
			for(int i=0;i<listInsVerifyHists.Count;i++) {
				InsVerifyHist insVerifyNew=listInsVerifyHists[i];
				InsVerifyHist insVerifyOld=null;
				if(i<listInsVerifyHists.Count-1) {
					insVerifyOld=listInsVerifyHists[i+1];//get next
				}
				//If the insVerifyOld is null, then the previous value was MinDate. 
				string oldVal=insVerifyOld==null ? Lans.g("FormInsEditLog","NEW") : insVerifyOld.DateLastVerified.ToShortDateString();
				listLogs.Add(new InsEditLog {
					FKey=insPlanNum,
					LogType=InsEditLogType.InsPlan,
					FieldName=Lans.g("FormInsEditLog","Benefits Last Verified"),
					OldValue=oldVal,
					NewValue=insVerifyNew.DateLastVerified.ToShortDateString(),
					UserNum=insVerifyNew.VerifyUserNum,
					DateTStamp=insVerifyNew.SecDateTEdit,
					ParentKey=0,
					Description="",
				});
			}
			//get any logs that show that InsPlan's PlanNum changed
			return GetChangedLogs(listLogs).OrderBy(x => x.DateTStamp)
				.ThenBy(x => x.LogType!=InsEditLogType.InsPlan)
				.ThenBy(x => x.LogType!=InsEditLogType.Carrier)
				.ThenBy(x => x.LogType!=InsEditLogType.Employer)
				.ThenBy(x => x.LogType!=InsEditLogType.Benefit)
				.ThenBy(x => x.FKey)
				//primary keys first
				.ThenBy(x => x.LogType==InsEditLogType.Benefit?(x.FieldName!="BenefitNum")
					:x.LogType==InsEditLogType.Carrier?(x.FieldName!="CarrierNum")
					:x.LogType==InsEditLogType.Employer?(x.FieldName!="EmployerNum")
					:(x.FieldName!="PlanNum")).ToList();
		}

		///<summary>Gets all logs with the passed-in FKey of the specified LogType.
		///Only returns logs that occurred before the passed-in log.
		///Called from GetChangedLogs and, between the two methods, recursively retrieves logs linked to the logs that are returned from this method.</summary>
		private static List<InsEditLog> GetLinkedLogs(long FKey,InsEditLogType logType,InsEditLog logCur,List<InsEditLog> listLogs) {
			//No need to check RemotingRole; private static.
			string command="SELECT * FROM inseditlog "
				+"WHERE FKey = "+POut.Long(FKey)+" "
				+"AND LogType = "+POut.Int((int)logType)+" "
				+"AND DateTStamp < "+POut.DateT(logCur.DateTStamp)+" "
				+"AND InsEditLogNum NOT IN( "+string.Join(",",listLogs.Select(x => POut.Long(x.InsEditLogNum)).ToList())+")";
			List<InsEditLog> listLinkedLogs=Crud.InsEditLogCrud.SelectMany(command);
			GetChangedLogs(listLinkedLogs);
			return listLinkedLogs;
		}

		///<summary>Looks for logs that show that the insplan or carrier changed and retrieves the previous insplan/carrier's information.
		///Called from GetLinkedLogs and, between the two methods, recursively retrieves logs linked to the logs that are returned from this method.</summary>
		private static List<InsEditLog> GetChangedLogs(List<InsEditLog> listLogs) {
			//No need to check RemotingRole; no call to db.
			List<InsEditLog> listInsPlanChangedLogs = listLogs.FindAll(x =>
				((x.LogType == InsEditLogType.InsPlan && x.FieldName == "PlanNum")
				|| (x.LogType == InsEditLogType.Carrier && x.FieldName == "CarrierNum"))
				&& x.OldValue != "NEW" && x.NewValue != "DELETED").ToList();
			foreach(InsEditLog editLogCur in listInsPlanChangedLogs) {
				if(editLogCur.FieldName == "PlanNum") {
					InsPlan oldPlan = InsPlans.GetPlan(PIn.Long(editLogCur.OldValue),null);					
					if(oldPlan!=null) {
						listLogs.AddRange(GetLinkedLogs(oldPlan.CarrierNum,InsEditLogType.Carrier,editLogCur,listLogs));
					}
					listLogs.AddRange(GetLinkedLogs(PIn.Long(editLogCur.OldValue),InsEditLogType.InsPlan,editLogCur,listLogs));
				}
				if(editLogCur.FieldName == "CarrierNum") {
					listLogs.AddRange(GetLinkedLogs(PIn.Long(editLogCur.OldValue),InsEditLogType.Carrier,editLogCur,listLogs));
				}
			}
			return listLogs;
		}

		/// <summary>Gets a list of carrierNums that can all be linked to the passed in carrierNum via Insurance Edit Log entries for
		/// carrierNum changes.</summary>
		public static List<long> GetAssociatedCarrierNums(long insPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),insPlanNum);
			}
			//Get carrierNums associated to this insPlanNum, using carrierNum as a starting point.
			string command=@"SELECT inseditlog.OldValue,inseditlog.NewValue
				FROM inseditlog
				WHERE inseditlog.LogType="+POut.Long((long)InsEditLogType.InsPlan)+@"
					AND inseditlog.FieldName='CarrierNum' 
					AND inseditlog.OldValue!=inseditlog.NewValue
					AND inseditlog.OldValue!=0 
					AND inseditlog.NewValue!=0
					AND inseditlog.FKey="+POut.Long(insPlanNum);
			DataTable table=Db.GetTable(command);
			List<long> retVal=new List<long>();
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(PIn.Long(table.Rows[i][0].ToString()));
				retVal.Add(PIn.Long(table.Rows[i][1].ToString()));
			}
			return retVal.Distinct().ToList();
		}

		///<summary>Automatic log entry. Fills in table and column names based on items passed in. 
		///Compares whole table excluding CrudColumnSpecialTypes of DateEntry, DateTEntry, ExcludeFromUpdate, and TimeStamp.
		///Pass in null for ItemOld if the item was just inserted. Pass in null for ItemCur if the item was just deleted.
		///Both itemCur and itemOld cannot be null.</summary>
		public static void MakeLogEntry<T>(T itemCur,T itemOld, InsEditLogType logType, long userNumCur) {
			//No need to check RemotingRole; no call to db.
			T priKeyItem = itemCur==null ? itemOld : itemCur;
			FieldInfo priKeyField = priKeyItem.GetType().GetFields().Where(x => x.IsDefined(typeof(CrudColumnAttribute))
			 && ((CrudColumnAttribute)x.GetCustomAttribute(typeof(CrudColumnAttribute))).IsPriKey).First();
			long priKey = (long) priKeyField.GetValue(priKeyItem);
			string priKeyColName = priKeyField.Name;
			long parentKey = priKeyItem.GetType() == typeof(Benefit) ? ((Benefit)(object)priKeyItem).PlanNum : 0; //parentKey only filled for Benefits.
			string descript = "";
			switch(logType) { //always default the descript to the priKeyItem (preferring current unless deleted).
				case InsEditLogType.InsPlan:
					descript = priKeyItem is InsPlan ? ((priKeyItem as InsPlan).GroupNum + " - " + (priKeyItem as InsPlan).GroupName) : "";
					break;
				case InsEditLogType.Carrier:
					descript = priKeyItem is Carrier ? ((priKeyItem as Carrier).CarrierNum + " - " + (priKeyItem as Carrier).CarrierName) : "";
					break;
				case InsEditLogType.Benefit:
					descript = priKeyItem is Benefit ? Benefits.GetCategoryString(priKeyItem as Benefit) : "";
					break;
				case InsEditLogType.Employer:
					descript = (priKeyItem as Employer)?.EmpName ?? "";
					break;
				default:
					break;
			}
			InsEditLog logCur;
			if(itemOld == null) { //new, just inserted. Show PriKey Column only.
				logCur = new InsEditLog() {
					FieldName = priKeyColName,
					UserNum = userNumCur,
					OldValue = "NEW",
					NewValue = priKey.ToString(),
					LogType = logType,
					FKey = priKey,
					ParentKey = parentKey,
					Description = descript,
				};
				Insert(logCur);
				return;
			}
			List<InsEditLog> listLogsForInsert=new List<InsEditLog>();
			foreach(FieldInfo prop in priKeyItem.GetType().GetFields()) {
				if(prop.IsDefined(typeof(CrudColumnAttribute))
				&& (((CrudColumnAttribute)prop.GetCustomAttribute(typeof(CrudColumnAttribute))).SpecialType.HasFlag(CrudSpecialColType.DateEntry)
				 || ((CrudColumnAttribute)prop.GetCustomAttribute(typeof(CrudColumnAttribute))).SpecialType.HasFlag(CrudSpecialColType.DateTEntry)
				 || ((CrudColumnAttribute)prop.GetCustomAttribute(typeof(CrudColumnAttribute))).SpecialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)
				 || ((CrudColumnAttribute)prop.GetCustomAttribute(typeof(CrudColumnAttribute))).SpecialType.HasFlag(CrudSpecialColType.TimeStamp))) 
				{
					continue; //skip logs that are not user editable.
				}
				object valOld = prop.GetValue(itemOld)??"";
				if(itemCur==null) { //deleted, show all deleted columns
					logCur = new InsEditLog() {
						FieldName = prop.Name,
						UserNum = userNumCur,
						OldValue = valOld.ToString(),
						NewValue = "DELETED",
						LogType = logType,
						FKey = priKey,
						ParentKey = parentKey,
						Description = descript,
					};
				}
				else { //updated, just show changes.
					object valCur = prop.GetValue(itemCur)??"";
					if(valCur.ToString() == valOld.ToString()) {
						continue;
					}
					logCur = new InsEditLog() {
						FieldName = prop.Name,
						UserNum = userNumCur,
						OldValue = valOld.ToString(),
						NewValue = valCur.ToString(),
						LogType = logType,
						FKey = priKey,
						ParentKey = parentKey,
						Description = descript,
					};
				}
				listLogsForInsert.Add(logCur);
			}
			if(listLogsForInsert.Count>0) {
				InsertMany(listLogsForInsert);
			}
		}

		public static void InsertMany(List<InsEditLog> listLogs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLogs);
				return;
			}
			Crud.InsEditLogCrud.InsertMany(listLogs);
			return;
		}

		public static long Insert(InsEditLog logCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				logCur.InsEditLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),logCur);
				return logCur.InsEditLogNum;
			}
			return Crud.InsEditLogCrud.Insert(logCur);
		}

		///<summary>Manual log entry. Creates a new InsEditLog based on information passed in. PKey should be 0 unless LogType = Benefit. 
		///Use the automatic MakeLogEntry overload if possible. This only be used when manual UPDATE/INSERT/DELETE queries are run on the logged tables.</summary>
		public static InsEditLog MakeLogEntry(string fieldName,long userNum,string oldVal,string newVal,InsEditLogType logType,long fKey,long pKey,string descript,bool doInsert=true) {
			//No need to check RemotingRole; no call to db.
			InsEditLog logCur=new InsEditLog() {
				FieldName=fieldName,
				UserNum=userNum,
				OldValue=oldVal,
				NewValue=newVal,
				LogType=logType,
				FKey=fKey,
				ParentKey=pKey,
				Description=descript,
			};
			if(doInsert) {
				Insert(logCur);
			}
			return logCur;
		}

		public static void DeletePreInsertedLogsForPlanNum(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum);
				return;
			}
			string command="DELETE FROM inseditlog "
				+"WHERE LogType="+POut.Int((int)InsEditLogType.Benefit)+" AND ParentKey="+POut.Long(planNum);
			Db.NonQ(command);
			command="DELETE FROM inseditlog "
				+"WHERE LogType="+POut.Int((int)InsEditLogType.InsPlan)+" AND FKey="+POut.Long(planNum);
			Db.NonQ(command);
		}
	}
}