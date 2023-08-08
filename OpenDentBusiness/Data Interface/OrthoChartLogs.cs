using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoChartLogs{
		public static void Log(string logData,string computerName,long patNum,long userNum){
			if(!PrefC.GetBool(PrefName.OrthoChartLoggingOn)) {
				return;
			}
			OrthoChartLog orthoChartLog=new OrthoChartLog();
			orthoChartLog.LogData=logData;
			orthoChartLog.ComputerName=computerName;
			orthoChartLog.PatNum=patNum;
			orthoChartLog.UserNum=userNum;
			orthoChartLog.DateTimeLog=DateTime.Now;
			OrthoChartLogCrud.Insert(orthoChartLog);
		}

		public static void Log(string logData,string computerName,OrthoChartRow orthoChartRow,long userNum=0){
			if(!PrefC.GetBool(PrefName.OrthoChartLoggingOn)) {
				return;
			}
			OrthoChartLog orthoChartLog=new OrthoChartLog();
			orthoChartLog.LogData=logData;
			orthoChartLog.ComputerName=computerName;
			orthoChartLog.PatNum=orthoChartRow.PatNum;
			orthoChartLog.DateTimeService=orthoChartRow.DateTimeService;
			if(userNum==0) {
				orthoChartLog.UserNum=orthoChartRow.UserNum;
			}
			else {
				orthoChartLog.UserNum=userNum;
			}
			orthoChartLog.ProvNum=orthoChartRow.ProvNum;
			orthoChartLog.OrthoChartRowNum=orthoChartRow.OrthoChartRowNum;
			orthoChartLog.DateTimeLog=DateTime.Now;
			OrthoChartLogCrud.Insert(orthoChartLog);
		}

		public static void LogDb(string logData,string computerName,long orthoChartRowNum,long userNum){
			if(!PrefC.GetBool(PrefName.OrthoChartLoggingOn)) {
				return;
			}
			//unfortunately, we don't have the OrthoChartRow object here to be able to save
			OrthoChartLog orthoChartLog=new OrthoChartLog();
			orthoChartLog.LogData=logData;
			orthoChartLog.ComputerName=computerName;
			orthoChartLog.OrthoChartRowNum=orthoChartRowNum;
			orthoChartLog.UserNum=userNum;
			orthoChartLog.DateTimeLog=DateTime.Now;
			OrthoChartLogCrud.Insert(orthoChartLog);
		}

		public static void LogDb(string logData,string computerName,OrthoChartRow orthoChartRow,long userNum){
			if(!PrefC.GetBool(PrefName.OrthoChartLoggingOn)) {
				return;
			}
			OrthoChartLog orthoChartLog=new OrthoChartLog();
			orthoChartLog.LogData=logData;
			orthoChartLog.ComputerName=computerName;
			orthoChartLog.PatNum=orthoChartRow.PatNum;
			orthoChartLog.DateTimeService=orthoChartRow.DateTimeService;
			orthoChartLog.UserNum=userNum;
			orthoChartLog.ProvNum=orthoChartRow.ProvNum;
			orthoChartLog.OrthoChartRowNum=orthoChartRow.OrthoChartRowNum;
			orthoChartLog.DateTimeLog=DateTime.Now;
			OrthoChartLogCrud.Insert(orthoChartLog);
		}
	}
}