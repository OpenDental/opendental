using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsEditPatLogs{
		#region Get Methods
		///<summary></summary>
		public static List<InsEditPatLog> GetLogsForPatPlan(long patPlanNum,long insSubNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<InsEditPatLog>>(MethodBase.GetCurrentMethod(),patPlanNum,insSubNum);
			}
			List<string> listWhereOrs=new List<string>();
			if(patPlanNum>0) {
				listWhereOrs.Add($"(LogType={POut.Int((int)InsEditPatLogType.PatPlan)} AND FKey = {POut.Long(patPlanNum)})");
			}
			if(insSubNum>0) {
				listWhereOrs.Add($"(LogType={POut.Int((int)InsEditPatLogType.Subscriber)} AND FKey = "+POut.Long(insSubNum)+")");
				listWhereOrs.Add($"(LogType={POut.Int((int)InsEditPatLogType.Adjustment)} AND ParentKey={POut.Long(insSubNum)})");
			}
			string command=@"SELECT * FROM inseditpatlog
				WHERE "+string.Join(@"
				OR ",listWhereOrs);
			List<InsEditPatLog> listInsEditPatLogs=Crud.InsEditPatLogCrud.SelectMany(command);//get all of the logs
			List<InsVerifyHist> listInsVerifyHists=InsVerifyHists.GetForFKeyByType(patPlanNum,VerifyTypes.PatientEnrollment)
				.OrderByDescending(x => x.SecDateTEdit)
				//Before 19.3.32, SecDateTEdit was not getting set. We'll use InsVerifyHistNum as a fallback.
				.ThenByDescending(x => x.InsVerifyHistNum).ToList();
			//The most recent inserted insverifyhist will be the current insverify. Go through the list of insverifyhist and construct inseditpatlog.
			//These inseditpatlog will not get inserted into the db.
			InsVerify insVerify=InsVerifies.GetOneByFKey(patPlanNum,VerifyTypes.PatientEnrollment);
			for(int i = 0;i<listInsVerifyHists.Count;i++) {
				InsVerifyHist insVerifyHistNew=listInsVerifyHists[i];
				InsVerifyHist insVerifyHistOld=null;
				if(i<listInsVerifyHists.Count-1) {
					insVerifyHistOld=listInsVerifyHists[i+1];//get next
				}
				//If the insVerifyOld is null, then this was a new verification. 
				string oldVal=insVerifyHistOld==null ? Lans.g("FormInsEditPatLog","NEW") : insVerifyHistOld.DateLastVerified.ToShortDateString();
				listInsEditPatLogs.Add(new InsEditPatLog {
					FKey=patPlanNum,
					LogType=InsEditPatLogType.PatPlan,
					FieldName=Lans.g("FormInsEditPatLog","Eligibility Last Verified"),
					OldValue=oldVal,
					NewValue=insVerifyHistNew.DateLastVerified.ToShortDateString(),
					UserNum=insVerifyHistNew.VerifyUserNum,
					DateTStamp=insVerifyHistNew.SecDateTEdit,
					ParentKey=0,
					Description="",
				});
			}
			return listInsEditPatLogs.OrderBy(x => x.DateTStamp)
				.ThenBy(x => x.LogType)
				.ThenBy(x => x.FKey)
				.ThenBy(x => x.FieldName)
				.ThenBy(x => x.InsEditPatLogNum).ToList();
		}
		
		public static void InsertMany(List<InsEditPatLog> listLogs) {
			if(listLogs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLogs);
				return;
			}
			Crud.InsEditPatLogCrud.InsertMany(listLogs);
			return;
		}

		#endregion Get Methods

		#region Misc Methods
		///<summary>Insert log entries.
		///Pass in null for ItemOld if the item was just inserted. Pass in null for ItemCur if the item was just deleted.
		///Both itemCur and itemOld cannot be null.</summary>
		public static void MakeLogEntry<T>(T itemCur,T itemOld,InsEditPatLogType insEditPatLogType) {
			List<InsEditPatLog> listInsEditPatLogsToInsert=CreateLogs(itemCur,itemOld,insEditPatLogType);
			InsertMany(listInsEditPatLogsToInsert);
		}

		///<summary>Creates log entries.</summary>
		private static List<InsEditPatLog> CreateLogs<T>(T itemCur,T itemOld,InsEditPatLogType insEditPatLogType) {
			Meth.NoCheckMiddleTierRole();
			List<InsEditPatLog> listInsEditPatLogsToInsert=new List<InsEditPatLog>();
			T priKeyItem=itemCur==null ? itemOld : itemCur;
			FieldInfo[] fieldInfoArray=priKeyItem.GetType().GetFields();
			FieldInfo fieldInfoPriKey=fieldInfoArray
				.Where(x => x.IsDefined(typeof(CrudColumnAttribute)))
				.Where(x => ((CrudColumnAttribute)x.GetCustomAttribute(typeof(CrudColumnAttribute))).IsPriKey).First();
			long priKey=(long)fieldInfoPriKey.GetValue(priKeyItem);
			string priKeyColName=fieldInfoPriKey.Name;
			long parentKey=0;
			string descript="";
			switch(insEditPatLogType) { 
				case InsEditPatLogType.Adjustment:
					descript="Insurance Benefit";
					parentKey=priKeyItem.GetType() == typeof(ClaimProc) ? ((ClaimProc)(object)priKeyItem).InsSubNum : 0;
					break;
				default:
					break;
			}
			long curUserNum=Security.CurUser?.UserNum??0;
			InsEditPatLog insEditPatLog;
			if(itemOld == null) { //new, just inserted. Show PriKey Column only.
				insEditPatLog=new InsEditPatLog() {
					FieldName=priKeyColName,
					UserNum=curUserNum,
					OldValue=Lans.g("FormInsEditPatLog","NEW"),
					NewValue=priKey.ToString(),
					LogType=insEditPatLogType,
					FKey=priKey,
					ParentKey=parentKey,
					Description=descript,
				};
				listInsEditPatLogsToInsert.Add(insEditPatLog);
				return listInsEditPatLogsToInsert;
			}
			foreach(FieldInfo tableColumn in fieldInfoArray) {
				if(!IsValidLogColumn(priKeyItem,tableColumn.Name,insEditPatLogType)) {
					continue; //Skip if not a column we want to log
				}
				object valOld=tableColumn.GetValue(itemOld)??"";
				if(itemCur==null) { //deleted, show all deleted columns
					insEditPatLog=new InsEditPatLog() {
						FieldName=tableColumn.Name,
						UserNum=curUserNum,
						OldValue=valOld.ToString(),
						NewValue=Lans.g("FormInsEditPatLog","DELETED"),
						LogType=insEditPatLogType,
						FKey=priKey,
						ParentKey=parentKey,
						Description=descript,
					};
					listInsEditPatLogsToInsert.Add(insEditPatLog);
					continue;
				}
				object valCur=tableColumn.GetValue(itemCur)??"";
				string strValCur=valCur.ToString();
				string strValOld=valOld.ToString();
				if(strValCur==strValOld) {
					continue;
				}
				//updated, just show changes.
				if(insEditPatLogType==InsEditPatLogType.Subscriber) {
					if(tableColumn.Name==nameof(InsSub.Subscriber)) {
						strValCur=strValCur+" - "+Patients.GetPat(PIn.Long(strValCur,false))?.GetNameFL()??"";//This will be the name of the current subscriber.
						strValOld=strValOld+" - "+Patients.GetPat(PIn.Long(strValOld,false))?.GetNameFL()??"";
					}
				}
				else if(insEditPatLogType==InsEditPatLogType.PatPlan) {
					//The checkbox does not have its own column in the patplan table. Mimic the logic from FormOrthoPat.cs when the checkbox is altered.
					if(tableColumn.Name==nameof(PatPlan.OrthoAutoFeeBilledOverride) && (strValOld=="-1" || strValCur=="-1")) {
						PatPlan patPlan=priKeyItem as PatPlan;
						InsPlan insPlanForPatPlan=InsPlans.GetByInsSubs(new List<long>{ patPlan.InsSubNum }).FirstOrDefault();
						//The UseDefaultFee check box was altered. Create a new log entry for the UseDefaultFee value.
						insEditPatLog=new InsEditPatLog() {
							FieldName="OrthoAutoFeeUseDefaultFee",
							UserNum=curUserNum,
							LogType=insEditPatLogType,
							FKey=priKey,
							ParentKey=parentKey,
							Description=descript,
						};
						if(strValOld=="-1" && strValCur!="-1") {//UseDefaultFee was unchecked.
							insEditPatLog.OldValue="True";
							insEditPatLog.NewValue="False";
							//UseDefaultFee was unchecked. The Patplan.OrthoAutoFeeBilledOverride has -1. Get the actual value from InsPlan. If the InsPlan is null,
							//set the strValOld equal to the strValuCur. This will make it so we don't insert a log for the OrthoAutoFeeBilledOverride.
							strValOld=insPlanForPatPlan?.OrthoAutoFeeBilled.ToString()??strValCur;
						}
						else {//UseDefaultFee was checked.
							insEditPatLog.OldValue="False";
							insEditPatLog.NewValue="True";
							//UseDefaultFee was checked. Get the current OrthoAutoFeeBilled value from InsPlan. If the InsPlan is null,
							//set the strValCur equal to the strValOld. This will make it so we don't insert a log for the OrthoAutoFeeBilledOverride.
							strValCur=insPlanForPatPlan?.OrthoAutoFeeBilled.ToString()??strValOld;
						}
						listInsEditPatLogsToInsert.Add(insEditPatLog);
						if(strValOld==strValCur) {
							continue;//Don't log change to fee since it did not change
						}
					}
				}
				insEditPatLog=new InsEditPatLog() {
					FieldName=tableColumn.Name,
					UserNum=curUserNum,
					OldValue=strValOld,
					NewValue=strValCur,
					LogType=insEditPatLogType,
					FKey=priKey,
					ParentKey=parentKey,
					Description=descript,
				};
				listInsEditPatLogsToInsert.Add(insEditPatLog);
			}
			return listInsEditPatLogsToInsert;
		}

		///<summary>Returns true if the column passed should be logged.</summary>
		private static bool IsValidLogColumn<T>(T priKeyItem,string colName,InsEditPatLogType insEditPatLogType) {
			if(priKeyItem is ClaimProc) {
				if(insEditPatLogType==InsEditPatLogType.Adjustment 
					&& colName.ToLower().In(nameof(ClaimProc.InsPayAmt).ToLower(),nameof(ClaimProc.ProcDate).ToLower(),nameof(ClaimProc.DedApplied).ToLower()))
				{
					return true;
				}
			}
			else if(priKeyItem is PatPlan) {
				if(colName.ToLower().In(nameof(PatPlan.Relationship).ToLower(),
					nameof(PatPlan.PatID).ToLower(),
					nameof(PatPlan.Ordinal).ToLower(),
					nameof(PatPlan.IsPending).ToLower(),
					nameof(PatPlan.OrthoAutoFeeBilledOverride).ToLower(),
					nameof(PatPlan.OrthoAutoNextClaimDate).ToLower())) 
				{
					return true;
				}
			}
			else if(priKeyItem is InsSub) {
				if(colName.ToLower().In(nameof(InsSub.Subscriber).ToLower(),
					nameof(InsSub.SubscriberID).ToLower(),
					nameof(InsSub.DateEffective).ToLower(),
					nameof(InsSub.DateTerm).ToLower(),
					nameof(InsSub.ReleaseInfo).ToLower(),
					nameof(InsSub.AssignBen).ToLower(),
					nameof(InsSub.SubscNote).ToLower())) 
				{
					return true;
				}
			}
			return false;
		}
		#endregion Misc Methods
	}
}