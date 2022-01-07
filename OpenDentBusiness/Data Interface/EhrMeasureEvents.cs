using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrMeasureEvents{
		#region Insert

		///<summary></summary>
		public static void InsertMany(List<EhrMeasureEvent> listEhrMeasureEvents) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEhrMeasureEvents);
				return;
			}
			Crud.EhrMeasureEventCrud.InsertMany(listEhrMeasureEvents);
		}

		#endregion

		///<summary>Gets a list of MeasureEvents.  Primarily used in FormEhrMeasureEvents.  Pass in true to get all EhrMeasureEvents for the date range.  Passing in true will ignore the specified measure event type.</summary>
		public static List<EhrMeasureEvent> GetAllByTypeFromDB(DateTime dateStart,DateTime dateEnd,EhrMeasureEventType measureEventType,bool isAll) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrMeasureEvent>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,measureEventType,isAll);
			}
			string command="SELECT * FROM ehrmeasureevent "
				+"WHERE DateTEvent >= "+POut.DateT(dateStart)+" "
				+"AND DateTEvent <= "+POut.DateT(dateEnd)+" ";
			if(!isAll) {
				command+="AND EventType = "+POut.Int((int)measureEventType)+" ";
			}
			command+="ORDER BY EventType,DateTEvent,PatNum";
			return Crud.EhrMeasureEventCrud.SelectMany(command);
		}

		///<summary>Gets the MoreInfo column from the most recent event of the specified type. Returns blank if none exists.</summary>
		public static string GetLatestInfoByType(EhrMeasureEventType measureEventType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),measureEventType);
			}
			string command="SELECT * FROM ehrmeasureevent WHERE EventType="+POut.Int((int)measureEventType)
			+" ORDER BY DateTEvent DESC LIMIT 1";
			EhrMeasureEvent measureEvent=Crud.EhrMeasureEventCrud.SelectOne(command);
			if(measureEvent==null) {
				return "";
			}
			else {
				return measureEvent.MoreInfo;
			}
		}

		///<summary>Ordered by dateT</summary>
		public static List<EhrMeasureEvent> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrMeasureEvent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrmeasureevent WHERE PatNum = "+POut.Long(patNum)+" "
				+"ORDER BY DateTEvent";
			return Crud.EhrMeasureEventCrud.SelectMany(command);
		}

		///<summary>Ordered by dateT</summary>
		public static List<EhrMeasureEvent> RefreshByType(long patNum,params EhrMeasureEventType[] ehrMeasureEventTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrMeasureEvent>>(MethodBase.GetCurrentMethod(),patNum,ehrMeasureEventTypes);
			}
			string command="SELECT * FROM ehrmeasureevent WHERE (";
			for(int i=0;i<ehrMeasureEventTypes.Length;i++) {
				if(i>0) {
					command+="OR ";
				}
				command+="EventType = "+POut.Int((int)ehrMeasureEventTypes[i])+" ";
			}
			command+=") AND PatNum = "+POut.Long(patNum)+" "
				+"ORDER BY DateTEvent";
			return Crud.EhrMeasureEventCrud.SelectMany(command);
		}

		///<summary>Creates a measure event for the patient and event type passed in.  Used by eServices.</summary>
		public static long CreateEventForPat(long patNum,EhrMeasureEventType measureEventType) {
			//No need to check RemotingRole; no call to db.
			EhrMeasureEvent measureEvent=new EhrMeasureEvent();
			measureEvent.DateTEvent=DateTime.Now;
			measureEvent.EventType=measureEventType;
			measureEvent.PatNum=patNum;
			measureEvent.MoreInfo="";
			return Insert(measureEvent);
		}

		///<summary></summary>
		public static long Insert(EhrMeasureEvent ehrMeasureEvent){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrMeasureEvent.EhrMeasureEventNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrMeasureEvent);
				return ehrMeasureEvent.EhrMeasureEventNum;
			}
			return Crud.EhrMeasureEventCrud.Insert(ehrMeasureEvent);
		}

		///<summary></summary>
		public static void Update(EhrMeasureEvent ehrMeasureEvent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrMeasureEvent);
				return;
			}
			Crud.EhrMeasureEventCrud.Update(ehrMeasureEvent);
		}

		///<summary></summary>
		public static void Delete(long ehrMeasureEventNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrMeasureEventNum);
				return;
			}
			string command= "DELETE FROM ehrmeasureevent WHERE EhrMeasureEventNum = "+POut.Long(ehrMeasureEventNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<EhrMeasureEvent> GetByType(List<EhrMeasureEvent> listMeasures,EhrMeasureEventType eventType) {
			//No need to check RemotingRole; no call to db.
			List<EhrMeasureEvent> retVal=new List<EhrMeasureEvent>();
			for(int i=0;i<listMeasures.Count;i++) {
				if(listMeasures[i].EventType==eventType) {
					retVal.Add(listMeasures[i]);
				}
			}
			return retVal;
		}

		///<summary>Gets codes (SNOMEDCT) from CodeValueResult for EhrMeasureEvents with DateTEvent within the last year for the given EhrMeasureEventType.
		///Result list is grouped by code.</summary>
		public static List<string> GetListCodesUsedForType(EhrMeasureEventType eventType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),eventType);
			}
			string command="SELECT CodeValueResult FROM ehrmeasureevent "
				+"WHERE EventType="+POut.Int((int)eventType)+" "
				+"AND CodeValueResult!='' "
				+"AND "+DbHelper.DtimeToDate("DateTEvent")+">="+POut.Date(MiscData.GetNowDateTime().AddYears(-1))+" "
				+"GROUP BY CodeValueResult";
			return Db.GetListString(command);
		}

		/*
		
		///<summary>Gets one EhrMeasureEvent from the db.</summary>
		public static EhrMeasureEvent GetOne(long ehrMeasureEventNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrMeasureEvent>(MethodBase.GetCurrentMethod(),ehrMeasureEventNum);
			}
			return Crud.EhrMeasureEventCrud.SelectOne(ehrMeasureEventNum);
		}

		

		*/
	}
}