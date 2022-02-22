using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrQuarterlyKeys{
		///<summary>Pass in a guarantor of 0 when not using from OD tech station.</summary>
		public static List<EhrQuarterlyKey> Refresh(long guarantor){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrQuarterlyKey>>(MethodBase.GetCurrentMethod(),guarantor);
			}
			string command;
			if(guarantor==0){//customer looking at their own quarterly keys
				command="SELECT * FROM ehrquarterlykey WHERE PatNum=0";
			}
			else{//
				command="SELECT ehrquarterlykey.* FROM ehrquarterlykey,patient "
					+"WHERE ehrquarterlykey.PatNum=patient.PatNum "
					+"AND patient.Guarantor="+POut.Long(guarantor)+" "
					+"GROUP BY ehrquarterlykey.EhrQuarterlyKeyNum "
					+"ORDER BY ehrquarterlykey.YearValue,ehrquarterlykey.QuarterValue";
			}
			return Crud.EhrQuarterlyKeyCrud.SelectMany(command);
		}

		public static EhrQuarterlyKey GetKeyThisQuarter() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrQuarterlyKey>(MethodBase.GetCurrentMethod());
			}
			string command;
			int quarter=MonthToQuarter(DateTime.Today.Month);
			command="SELECT * FROM ehrquarterlykey WHERE YearValue="+(DateTime.Today.Year-2000).ToString()+" "
				+"AND QuarterValue="+quarter.ToString()+" "//we don't care about practice title in the query
				+"AND PatNum=0";
			return Crud.EhrQuarterlyKeyCrud.SelectOne(command);
		}

		///<summary></summary>
		public static List<EhrQuarterlyKey> GetAllKeys() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrQuarterlyKey>>(MethodBase.GetCurrentMethod());
			}
			string command;
			command="SELECT * FROM ehrquarterlykey WHERE PatNum=0 ORDER BY YearValue,QuarterValue";
			List<EhrQuarterlyKey> ehrKeys=Crud.EhrQuarterlyKeyCrud.SelectMany(command);
			return ehrKeys;
		}

		///<summary>Returns all keys in the given years.</summary>
		public static List<EhrQuarterlyKey> GetAllKeys(DateTime startDate,DateTime endDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrQuarterlyKey>>(MethodBase.GetCurrentMethod(),startDate,endDate);
			}
			int startYear=startDate.Year-2000;
			int endYear=endDate.Year-2000;
			string command;
			command="SELECT * FROM ehrquarterlykey WHERE (YearValue BETWEEN "+startYear+" "+"AND "+endYear+") "
			+"AND PatNum=0 ORDER BY YearValue,QuarterValue";
			List<EhrQuarterlyKey> ehrKeys=Crud.EhrQuarterlyKeyCrud.SelectMany(command);
			return ehrKeys;
		}

		///<summary>We want to find the first day of the oldest quarter less than or equal to one year old from the latest entered valid key.  validKeys must be sorted ascending.</summary>
		public static DateTime FindStartDate(List<EhrQuarterlyKey> validKeys) {
			//No need to check RemotingRole; no call to db.
			if(validKeys.Count<1) {
				return new DateTime(DateTime.Today.Year,1,1);
			}
			EhrQuarterlyKey ehrKey=validKeys[validKeys.Count-1];
			DateTime latestReportDate=GetLastDayOfQuarter(ehrKey);
			DateTime earliestStartDate=latestReportDate.AddYears(-1).AddDays(1);
			for(int i=validKeys.Count-1;i>-1;i--) {
				ehrKey=validKeys[i];
				if(i==0) {
					break;
				}
				int expectedPrevQuarter=validKeys[i].QuarterValue-1; 
				if(validKeys[i].QuarterValue==1) {
					expectedPrevQuarter=4;
				}
				int prevQuarter=validKeys[i-1].QuarterValue;
				int expectedYear=validKeys[i].YearValue;
				if(validKeys[i].QuarterValue==1) {
					expectedYear=validKeys[i].YearValue-1;
				}
				int prevQuarter_Year=validKeys[i-1].YearValue;
				if(expectedPrevQuarter!=prevQuarter || expectedYear!=prevQuarter_Year) {//There is an quarterly key gap, so we ignore any older quarterly keys.
					break;
				}
				DateTime prevQuarterFirstDay=GetFirstDayOfQuarter(validKeys[i-1]);
				if(prevQuarterFirstDay<earliestStartDate) {
					break;
				}
			}
			return GetFirstDayOfQuarter(ehrKey);
		}

		public static DateTime GetFirstDayOfQuarter(DateTime date) {
			//No need to check RemotingRole; no call to db.
			return new DateTime(date.Year,3*(MonthToQuarter(date.Month)-1)+1,1);
		}
		public static DateTime GetFirstDayOfQuarter(EhrQuarterlyKey ehrKey) {
			//No need to check RemotingRole; no call to db.
			return GetFirstDayOfQuarter(new DateTime(ehrKey.YearValue+2000,ehrKey.QuarterValue*3,1));
		}

		public static DateTime GetLastDayOfQuarter(DateTime date) {
			//No need to check RemotingRole; no call to db.
			return new DateTime(date.Year,1,1).AddMonths(3*MonthToQuarter(date.Month)).AddDays(-1);
		}
		
		public static DateTime GetLastDayOfQuarter(EhrQuarterlyKey ehrKey) {
			//No need to check RemotingRole; no call to db.
			return GetLastDayOfQuarter(new DateTime(ehrKey.YearValue+2000,ehrKey.QuarterValue*3,1));
		}

		public static string ValidateDateRange(List<EhrQuarterlyKey> validKeys,DateTime startDate,DateTime endDate) {
			//No need to check RemotingRole; no call to db.
			if(validKeys.Count==0) {
				return "No Valid Quarterly Keys";
			}
			DateTime startOfFirstQuarter=GetFirstDayOfQuarter(startDate);
			DateTime endOfLastQuarter=GetLastDayOfQuarter(endDate);
			string explanation="";
			int msgCount=0;
			int numberOfQuarters=CalculateQuarters(startDate,endDate);
			for(int j=0;j<numberOfQuarters;j++) {
				bool isValid=false;
				if(startOfFirstQuarter>endOfLastQuarter) {
					break;
				}
				for(int i=0;i<validKeys.Count;i++) {
					if(MonthToQuarter(startOfFirstQuarter.Month)==validKeys[i].QuarterValue && startOfFirstQuarter.Year-2000==validKeys[i].YearValue) {
						isValid=true;
						break;
					}
				}
				if(!isValid) {
					if(explanation=="") {
						explanation="Selected date range is invalid. You are missing these quarterly keys: \r\n";
					}
					explanation+=startOfFirstQuarter.Year+"-Q"+MonthToQuarter(startOfFirstQuarter.Month)+"\r\n";
					msgCount++;
					if(msgCount>8) {
						return explanation;
					}
				}
				startOfFirstQuarter=startOfFirstQuarter.AddMonths(3);
			}
			return explanation;
		}

		///<summary>Gets the count of quarters between the dates inclusive.</summary>
		private static int CalculateQuarters(DateTime startDate,DateTime endDate) {
			//No need to check RemotingRole; no call to db.
			int startQuarter=MonthToQuarter(startDate.Month);
			int endQuarter=MonthToQuarter(endDate.Month);
			return 4*(endDate.Year-startDate.Year)+(endQuarter-startQuarter+1);
		}

		public static int MonthToQuarter(int month) {
			//No need to check RemotingRole; no call to db.
			int quarter=1;
			if(month>=4 && month<=6) {
				quarter=2;
			}
			if(month>=7 && month<=9) {
				quarter=3;
			}
			if(month>=10) {
				quarter=4;
			}
			return quarter;
		}

		///<summary></summary>
		public static long Insert(EhrQuarterlyKey ehrQuarterlyKey){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrQuarterlyKey.EhrQuarterlyKeyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrQuarterlyKey);
				return ehrQuarterlyKey.EhrQuarterlyKeyNum;
			}
			return Crud.EhrQuarterlyKeyCrud.Insert(ehrQuarterlyKey);
		}

		///<summary></summary>
		public static void Update(EhrQuarterlyKey ehrQuarterlyKey){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrQuarterlyKey);
				return;
			}
			Crud.EhrQuarterlyKeyCrud.Update(ehrQuarterlyKey);
		}

		///<summary></summary>
		public static void Delete(long ehrQuarterlyKeyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrQuarterlyKeyNum);
				return;
			}
			string command= "DELETE FROM ehrquarterlykey WHERE EhrQuarterlyKeyNum = "+POut.Long(ehrQuarterlyKeyNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		

		///<summary>Gets one EhrQuarterlyKey from the db.</summary>
		public static EhrQuarterlyKey GetOne(long ehrQuarterlyKeyNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrQuarterlyKey>(MethodBase.GetCurrentMethod(),ehrQuarterlyKeyNum);
			}
			return Crud.EhrQuarterlyKeyCrud.SelectOne(ehrQuarterlyKeyNum);
		}

		
		*/
	}
}