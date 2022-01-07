using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LabResults{
		public static List<LabResult> GetForPanel(long labPanelNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),labPanelNum);
			}
			string command="SELECT * FROM labresult WHERE LabPanelNum = "+POut.Long(labPanelNum);
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long labResultNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labResultNum);
				return;
			}
			string command= "DELETE FROM labresult WHERE LabResultNum = "+POut.Long(labResultNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all Lab Results associated with Lab Panel.</summary>
		public static void DeleteForPanel(long labPanelNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labresult WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceLabResultNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT LabResultNum FROM labresult WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> labresultNums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				labresultNums.Add(PIn.Long(dt.Rows[i]["LabResultNum"].ToString()));
			}
			return labresultNums;
		}

		///<summary>Used along with GetChangedSinceLabResultNums</summary>
		public static List<LabResult> GetMultLabResults(List<long> labresultNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),labresultNums);
			}
			string strLabResultNums="";
			DataTable table;
			if(labresultNums.Count>0) {
				for(int i=0;i<labresultNums.Count;i++) {
					if(i>0) {
						strLabResultNums+="OR ";
					}
					strLabResultNums+="LabResultNum='"+labresultNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM labresult WHERE "+strLabResultNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			LabResult[] multLabResults=Crud.LabResultCrud.TableToList(table).ToArray();
			List<LabResult> LabResultList=new List<LabResult>(multLabResults);
			return LabResultList;
		}

		///<summary>Get all lab results for one patient.</summary>
		public static List<LabResult> GetAllForPatient(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labresult WHERE LabPanelNum IN (SELECT LabPanelNum FROM labpanel WHERE PatNum="+POut.Long(patNum)+")";
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary>Insert new lab results.</summary>
		public static long Insert(LabResult labResult) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				labResult.LabResultNum=Meth.GetLong(MethodBase.GetCurrentMethod(),labResult);
				return labResult.LabResultNum;
			}
			return Crud.LabResultCrud.Insert(labResult);
		}

		///<summary>Update existing lab results.</summary>
		public static void Update(LabResult labResult){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labResult);
				return;
			}
			Crud.LabResultCrud.Update(labResult);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<LabResult> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labresult WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary>Gets one LabResult from the db.</summary>
		public static LabResult GetOne(long labResultNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<LabResult>(MethodBase.GetCurrentMethod(),labResultNum);
			}
			return Crud.LabResultCrud.SelectOne(labResultNum);
		}

		

		*/

		/// <summary>Returns the text for a SnomedAllergy Enum as it should appear in human readable form for a CCD.</summary>
		public static string GetAbnormalFlagDesc(LabAbnormalFlag abnormalFlag) {
			string result;
			switch(abnormalFlag){
				case LabAbnormalFlag.Above:
					result="above high normal";
					break;
				case LabAbnormalFlag.Normal:
					result="normal";
					break;
				case LabAbnormalFlag.Below:
					result="below normal";
					break;
				case LabAbnormalFlag.None:
					result="";
					break;
				default:
					result="Error";
					break;
			}
			return result;
		}
	}
}