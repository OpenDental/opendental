using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LabResults{
		public static List<LabResult> GetForPanel(long labPanelNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),labPanelNum);
			}
			string command="SELECT * FROM labresult WHERE LabPanelNum = "+POut.Long(labPanelNum);
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long labResultNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labResultNum);
				return;
			}
			string command= "DELETE FROM labresult WHERE LabResultNum = "+POut.Long(labResultNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all Lab Results associated with Lab Panel.</summary>
		public static void DeleteForPanel(long labPanelNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labresult WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceLabResultNums(DateTime dateChangedSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateChangedSince);
			}
			string command="SELECT LabResultNum FROM labresult WHERE DateTStamp > "+POut.DateT(dateChangedSince);
			DataTable table=Db.GetTable(command);
			List<long> listLabResultNums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				listLabResultNums.Add(PIn.Long(table.Rows[i]["LabResultNum"].ToString()));
			}
			return listLabResultNums;
		}

		///<summary>Used along with GetChangedSinceLabResultNums</summary>
		public static List<LabResult> GetMultLabResults(List<long> listLabResultNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),listLabResultNums);
			}
			string strLabResultNums="";
			DataTable table;
			if(listLabResultNums.Count>0) {
				for(int i=0;i<listLabResultNums.Count;i++) {
					if(i>0) {
						strLabResultNums+="OR ";
					}
					strLabResultNums+="LabResultNum='"+listLabResultNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM labresult WHERE "+strLabResultNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			return Crud.LabResultCrud.TableToList(table);
		}

		///<summary>Get all lab results for one patient.</summary>
		public static List<LabResult> GetAllForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labresult WHERE LabPanelNum IN (SELECT LabPanelNum FROM labpanel WHERE PatNum="+POut.Long(patNum)+")";
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary>Insert new lab results.</summary>
		public static long Insert(LabResult labResult) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				labResult.LabResultNum=Meth.GetLong(MethodBase.GetCurrentMethod(),labResult);
				return labResult.LabResultNum;
			}
			return Crud.LabResultCrud.Insert(labResult);
		}

		///<summary>Update existing lab results.</summary>
		public static void Update(LabResult labResult){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labResult);
				return;
			}
			Crud.LabResultCrud.Update(labResult);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<LabResult> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labresult WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabResultCrud.SelectMany(command);
		}

		///<summary>Gets one LabResult from the db.</summary>
		public static LabResult GetOne(long labResultNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<LabResult>(MethodBase.GetCurrentMethod(),labResultNum);
			}
			return Crud.LabResultCrud.SelectOne(labResultNum);
		}

		

		*/

		/// <summary>Returns the text for a SnomedAllergy Enum as it should appear in human readable form for a CCD.</summary>
		public static string GetAbnormalFlagDesc(LabAbnormalFlag labAbnormalFlag) {
			string result;
			switch(labAbnormalFlag){
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