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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labpanel WHERE PatNum="+POut.Long(patNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<LabPanel> GetPanelsForOrder(long medicalOrderNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),medicalOrderNum);
			}
			if(medicalOrderNum==0) {
				return new List<LabPanel>();
			}
			string command="SELECT * FROM labpanel WHERE MedicalOrderNum="+POut.Long(medicalOrderNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long labPanelNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labpanel WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceLabPanelNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,eligibleForUploadPatNumList);
			}
			string strEligibleForUploadPatNums="";
			DataTable table;
			if(eligibleForUploadPatNumList.Count>0) {
				for(int i=0;i<eligibleForUploadPatNumList.Count;i++) {
					if(i>0) {
						strEligibleForUploadPatNums+="OR ";
					}
					strEligibleForUploadPatNums+="PatNum='"+eligibleForUploadPatNumList[i].ToString()+"' ";
				}
				string command="SELECT LabPanelNum FROM labpanel WHERE DateTStamp > "+POut.DateT(changedSince)+" AND ("+strEligibleForUploadPatNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> labPanelnums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				labPanelnums.Add(PIn.Long(table.Rows[i]["LabPanelNum"].ToString()));
			}
			return labPanelnums;
		}

		///<summary>Used along with GetChangedSinceLabPanelNums</summary>
		public static List<LabPanel> GetMultLabPanels(List<long> labpanelNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),labpanelNums);
			}
			string strLabPanelNums="";
			DataTable table;
			if(labpanelNums.Count>0) {
				for(int i=0;i<labpanelNums.Count;i++) {
					if(i>0) {
						strLabPanelNums+="OR ";
					}
					strLabPanelNums+="LabPanelNum='"+labpanelNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM labpanel WHERE "+strLabPanelNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			LabPanel[] multLabPanels=Crud.LabPanelCrud.TableToList(table).ToArray();
			List<LabPanel> LabPanelList=new List<LabPanel>(multLabPanels);
			return LabPanelList;
		}

		///<summary></summary>
		public static long Insert(LabPanel labPanel) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				labPanel.LabPanelNum=Meth.GetLong(MethodBase.GetCurrentMethod(),labPanel);
				return labPanel.LabPanelNum;
			}
			return Crud.LabPanelCrud.Insert(labPanel);
		}

		///<summary></summary>
		public static void Update(LabPanel labPanel) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanel);
				return;
			}
			Crud.LabPanelCrud.Update(labPanel);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all labpanels of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE labpanel SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}
		
				///<summary>Gets one LabPanel from the db.</summary>
		public static LabPanel GetOne(long labPanelNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<LabPanel>(MethodBase.GetCurrentMethod(),labPanelNum);
			}
			return Crud.LabPanelCrud.SelectOne(labPanelNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.



		 
				///<summary></summary>
		public static List<LabPanel> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LabPanel>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM labpanel WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabPanelCrud.SelectMany(command);
		}

		

		///<summary></summary>
		public static void Delete(long labPanelNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),labPanelNum);
				return;
			}
			string command= "DELETE FROM labpanel WHERE LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}
		*/
	}
}