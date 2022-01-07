using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PhoneMetrics{

		///<summary></summary>
		public static long Insert(PhoneMetric phoneMetric){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				phoneMetric.PhoneMetricNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneMetric);
				return phoneMetric.PhoneMetricNum;
			}
			return Crud.PhoneMetricCrud.Insert(phoneMetric);
		}
		
		///<summary>Returns the average number of minutes behind rounded down for each half hour from 5:00 AM - 7:00 PM.</summary>
		public static int[] AverageMinutesBehind(DateTime date){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<int[]>(MethodBase.GetCurrentMethod(),date);
			}
			DateTime startTime=new DateTime(date.Year,date.Month,date.Day,5,0,0);
			DateTime endTime=new DateTime(date.Year,date.Month,date.Day,19,0,0);
			if(date.DayOfWeek==DayOfWeek.Saturday) {
				startTime=new DateTime(date.Year,date.Month,date.Day,7,0,0);
				endTime=new DateTime(date.Year,date.Month,date.Day,16,0,0);
			}
			else if(date.DayOfWeek==DayOfWeek.Sunday) {
				startTime=new DateTime(date.Year,date.Month,date.Day,7,0,0);
				endTime=new DateTime(date.Year,date.Month,date.Day,12,0,0);
			}
			string command="SELECT * FROM phonemetric WHERE DateTimeEntry BETWEEN "+POut.DateT(startTime)+" AND "+POut.DateT(endTime);
			List<PhoneMetric> listPhoneMetrics=Crud.PhoneMetricCrud.SelectMany(command);
			int[] avgMinBehind=new int[28];//Used in FormGraphEmployeeTime. One "bucket" every half hour.
			int numerator;
			int denominator;
			startTime=new DateTime(date.Year,date.Month,date.Day,5,0,0);
			endTime=new DateTime(date.Year,date.Month,date.Day,5,30,0);
			for(int i=0;i<28;i++) {
				numerator=0;
				denominator=0;
				//reuse startTime and endTime for 30 minute intervals
				for(int j=0;j<listPhoneMetrics.Count;j++) {
					if(startTime<listPhoneMetrics[j].DateTimeEntry && listPhoneMetrics[j].DateTimeEntry < endTime) { //startTime < time < endTime
						numerator+=listPhoneMetrics[j].MinutesBehind;
						denominator++;
					}
				}
				if(denominator>0) {
					avgMinBehind[i]=numerator/denominator;//denominator should usually be 30. Result will be rounded down due to integer math.
				}
				else {
					avgMinBehind[i]=0;
				}
				startTime=startTime.AddMinutes(30);
				endTime=endTime.AddMinutes(30);
			}
			return avgMinBehind;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<PhoneMetric> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneMetric>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM phonemetric WHERE PatNum = "+POut.Long(patNum);
			return Crud.PhoneMetricCrud.SelectMany(command);
		}

		///<summary>Gets one PhoneMetric from the db.</summary>
		public static PhoneMetric GetOne(long phoneMetricNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PhoneMetric>(MethodBase.GetCurrentMethod(),phoneMetricNum);
			}
			return Crud.PhoneMetricCrud.SelectOne(phoneMetricNum);
		}

		///<summary></summary>
		public static void Update(PhoneMetric phoneMetric){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneMetric);
				return;
			}
			Crud.PhoneMetricCrud.Update(phoneMetric);
		}

		///<summary></summary>
		public static void Delete(long phoneMetricNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneMetricNum);
				return;
			}
			string command= "DELETE FROM phonemetric WHERE PhoneMetricNum = "+POut.Long(phoneMetricNum);
			Db.NonQ(command);
		}
		*/



	}
}