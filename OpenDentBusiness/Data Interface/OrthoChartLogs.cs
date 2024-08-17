﻿using System;
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
		public static void Log(string logData,string computerName,long patNum,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),logData,computerName,patNum,userNum);
				return;
			}
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

		public static void Log(string logData,string computerName,OrthoChartRow orthoChartRow,long userNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),logData,computerName,orthoChartRow,userNum);
				return;
			}
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

		public static void Log(string logData,List<OrthoChart> listOrthoCharts,long orthoChartRowNum,string computerName,long patNum,long userNum) {
			if(!PrefC.GetBool(PrefName.OrthoChartLoggingOn)) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),logData,listOrthoCharts,orthoChartRowNum,computerName,patNum,userNum);
				return;
			}
			logData+=Environment.NewLine+"OrthoChart Values:"+Environment.NewLine;
			foreach(OrthoChart orthoChart in listOrthoCharts) {
				logData+=$"	OrthoChartNum:{orthoChart.OrthoChartNum} - {orthoChart.FieldName}:{orthoChart.FieldValue}{Environment.NewLine}";
			}
			OrthoChartLog orthoChartLog=new OrthoChartLog();
			orthoChartLog.LogData=logData;
			orthoChartLog.ComputerName=computerName;
			orthoChartLog.PatNum=patNum;
			orthoChartLog.UserNum=userNum;
			orthoChartLog.OrthoChartRowNum=orthoChartRowNum;
			orthoChartLog.DateTimeLog=DateTime.Now;
			OrthoChartLogCrud.Insert(orthoChartLog);
		}

		public static void LogDb(string logData,string computerName,long orthoChartRowNum,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),logData,computerName,orthoChartRowNum,userNum);
				return;
			}
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

		public static void LogDb(string logData,string computerName,OrthoChartRow orthoChartRow,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),logData,computerName,orthoChartRow,userNum);
				return;
			}
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