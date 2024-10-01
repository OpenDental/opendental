using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LabPanels{
		///<summary></summary>
		public static List<LabPanel> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labpanel WHERE PatNum="+POut.Long(patNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<LabPanel> GetPanelsForOrder(long medicalOrderNum) {
			if(medicalOrderNum==0) {
				return new List<LabPanel>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),medicalOrderNum);
			}
			string command="SELECT * FROM labpanel WHERE MedicalOrderNum="+POut.Long(medicalOrderNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long labPanelNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labpanel WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceLabPanelNums(DateTime dateChangedSince,List<long> listPatNumsEligibleForUpload) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateChangedSince,listPatNumsEligibleForUpload);
			}
			string strPatNumsEligibleForUpload="";
			DataTable table;
			if(listPatNumsEligibleForUpload.Count>0) {
				for(int i=0;i<listPatNumsEligibleForUpload.Count;i++) {
					if(i>0) {
						strPatNumsEligibleForUpload+="OR ";
					}
					strPatNumsEligibleForUpload+="PatNum='"+listPatNumsEligibleForUpload[i].ToString()+"' ";
				}
				string command="SELECT LabPanelNum FROM labpanel WHERE DateTStamp > "+POut.DateT(dateChangedSince)+" AND ("+strPatNumsEligibleForUpload+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> listLabPanelNums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				listLabPanelNums.Add(PIn.Long(table.Rows[i]["LabPanelNum"].ToString()));
			}
			return listLabPanelNums;
		}

		///<summary>Used along with GetChangedSinceLabPanelNums</summary>
		public static List<LabPanel> GetMultLabPanels(List<long> listLabPanelNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),listLabPanelNums);
			}
			string strLabPanelNums="";
			DataTable table;
			if(listLabPanelNums.Count>0) {
				for(int i=0;i<listLabPanelNums.Count;i++) {
					if(i>0) {
						strLabPanelNums+="OR ";
					}
					strLabPanelNums+="LabPanelNum='"+listLabPanelNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM labpanel WHERE "+strLabPanelNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			return Crud.LabPanelCrud.TableToList(table);
		}

		///<summary></summary>
		public static long Insert(LabPanel labPanel) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				labPanel.LabPanelNum=Meth.GetLong(MethodBase.GetCurrentMethod(),labPanel);
				return labPanel.LabPanelNum;
			}
			return Crud.LabPanelCrud.Insert(labPanel);
		}

		///<summary></summary>
		public static void Update(LabPanel labPanel) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanel);
				return;
			}
			Crud.LabPanelCrud.Update(labPanel);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all labpanels of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE labpanel SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}
		
				///<summary>Gets one LabPanel from the db.</summary>
		public static LabPanel GetOne(long labPanelNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<LabPanel>(MethodBase.GetCurrentMethod(),labPanelNum);
			}
			return Crud.LabPanelCrud.SelectOne(labPanelNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.



		 
				///<summary></summary>
		public static List<LabPanel> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labpanel WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		

		///<summary></summary>
		public static void Delete(long labPanelNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labpanel WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}
		*/
	}
}