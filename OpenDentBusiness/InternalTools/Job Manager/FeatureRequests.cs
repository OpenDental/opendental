using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class FeatureRequests {

		///<Summary>Checks requestIDs in list for incompletes. Returns false if incomplete exists.</Summary>
		public static bool CheckForCompletion(List<long> listRequestIDs) {
			if(listRequestIDs.IsNullOrEmpty()) {
				return true;//Invalid input provided, true means that no incompletes exist.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listRequestIDs);
			}
			string count="";
			DataAction.RunBugsHQ(() => {
				string command="SELECT COUNT(*) FROM request "
					+"WHERE Approval!="+POut.Int((int)ApprovalEnum.Complete)+" "
					+"AND RequestId IN ("+String.Join(",",listRequestIDs)+")";
				count=Db.GetCount(command);
			},false);
			return (count=="0");
		}
		
		///<Summary>Will not mark completed feature requests as in progress.</Summary>
		public static void MarkAsInProgress(List<long> listRequestIDs) {
			if(listRequestIDs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRequestIDs);
				return;
			}
			DataAction.RunBugsHQ(() => {
				string command="UPDATE request SET Approval="+POut.Int((int)ApprovalEnum.InProgress)+" "
					+"WHERE Approval!="+POut.Int((int)ApprovalEnum.Complete)+" "
					+"AND RequestId IN ("+String.Join(",",listRequestIDs)+")";
				Db.NonQ(command);
			},false);
		}

		///<summary>Will only move feature requests that are "In Progress" to "Approved".</summary>
		public static void MarkAsApproved(List<long> listRequestIDs) {
			if(listRequestIDs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRequestIDs);
				return;
			}
			DataAction.RunBugsHQ(() => {
				string command="UPDATE request SET Approval="+POut.Int((int)ApprovalEnum.Approved)+" "
					+"WHERE Approval="+POut.Int((int)ApprovalEnum.InProgress)+" "
					+"AND RequestId IN ("+String.Join(",",listRequestIDs)+")";
				Db.NonQ(command);
			},false);
		}
		
		///<Summary></Summary>
		public static void CompleteRequests(List<long> listRequestIDs,string versionText) {
			if(listRequestIDs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRequestIDs,versionText);
			}
			DataAction.RunBugsHQ(() => {
				string versionString="\r\n\r\nCompleted in version "+POut.String(versionText);
				string command="UPDATE request SET Approval="+POut.Int((int)ApprovalEnum.Complete)+", "
					+"Detail=CONCAT( Detail , '"+versionString+"' ) "
					+"WHERE RequestId IN ("+String.Join(",",listRequestIDs)+")";
				Db.NonQ(command);
			},false);
		}

		///<summary>Gets all feature request.  Optionally pass in a list of Request IDs to only get those feature requests.</summary>
		public static List<FeatureRequest> GetAll(List<long> listRequestIDs=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FeatureRequest>>(MethodBase.GetCurrentMethod(),listRequestIDs);
			}
			DataTable table=new DataTable();
			DataAction.RunBugsHQ(() => {
				#region WebServiceCustomerUpdates.FeatureRequestGetList
				string command="SELECT request.RequestId,Approval,Description,Difficulty,"
					+"vote.AmountPledged,IFNULL(vote.Points,0) AS points,vote.IsCritical,request.PatNum,"
					+"TotalCritical,TotalPledged,TotalPoints,Weight "
					+"FROM request "
					+"LEFT JOIN vote ON vote.PatNum=1486 AND vote.RequestId=request.RequestId "
					+"WHERE Approval IN ("+POut.Int((int)ApprovalEnum.New)
						+","+POut.Int((int)ApprovalEnum.Approved)
						+","+POut.Int((int)ApprovalEnum.InProgress)
						+","+POut.Int((int)ApprovalEnum.Complete)+") ";
				if(!listRequestIDs.IsNullOrEmpty()) {
					command+=$"AND request.RequestId IN ({string.Join(",",listRequestIDs)}) ";
				}
				command+="ORDER BY Approval, Weight DESC, points DESC";
				DataTable raw=Db.GetTable(command);
				DataRow row;
				table.TableName="Table";
				table.Columns.Add("approval");
				table.Columns.Add("Description");
				table.Columns.Add("Difficulty");
				table.Columns.Add("isMine");
				table.Columns.Add("myVotes");
				table.Columns.Add("RequestId");
				table.Columns.Add("totalVotes");
				table.Columns.Add("Weight");
				table.Columns.Add("personalVotes");
				table.Columns.Add("personalCrit");
				table.Columns.Add("personalPledged");
				double myPledge;
				bool myCritical;
				double totalPledged;
				int totalCritical;
				for(int i=0;i<raw.Rows.Count;i++){
					row=table.NewRow();
					row["RequestId"]=raw.Rows[i]["RequestId"].ToString();
					//myVotes,myCritical,myPledge------------------------------------------------------
					row["myVotes"]=raw.Rows[i]["Points"].ToString();
					row["personalVotes"]=raw.Rows[i]["Points"].ToString();
					if(row["myVotes"].ToString()=="0"){
						row["myVotes"]="";
					}
					myCritical=PIn.Bool(raw.Rows[i]["IsCritical"].ToString());
					if(myCritical==true){
						row["personalCrit"]="1";
						if(row["myVotes"].ToString()!=""){
							row["myVotes"]+="\r\n";
						}
						row["myVotes"]+="Critical";
					}
					else {
						row["personalCrit"]="0";
					}
					myPledge=PIn.Double(raw.Rows[i]["AmountPledged"].ToString());
					if(myPledge!=0){
						if(row["myVotes"].ToString()!=""){
							row["myVotes"]+="\r\n";
						}
						row["myVotes"]+=myPledge.ToString("c0");
						row["personalPledged"]=myPledge.ToString();
					}
					else {
						row["personalPledged"]="0";
					}
					//TotalPoints,TotalCritical,TotalPledged-----------------------------------------------
					row["totalVotes"]=raw.Rows[i]["TotalPoints"].ToString();
					if(row["totalVotes"].ToString()=="0"){
						row["totalVotes"]="";
					}
					totalCritical=PIn.Int(raw.Rows[i]["TotalCritical"].ToString());
					if(totalCritical!=0){
						if(row["totalVotes"].ToString()!=""){
							row["totalVotes"]+="\r\n";
						}
						row["totalVotes"]+="Critical:"+totalCritical.ToString();
					}
					totalPledged=PIn.Double(raw.Rows[i]["TotalPledged"].ToString());
					if(totalPledged!=0){
						if(row["totalVotes"].ToString()!=""){
							row["totalVotes"]+="\r\n";
						}
						row["totalVotes"]+=totalPledged.ToString("c0");
					}
					//end
					row["approval"]=((ApprovalEnum)PIn.Int(raw.Rows[i]["Approval"].ToString())).ToString();
					if(raw.Rows[i]["PatNum"].ToString()=="1486") {
						row["isMine"]="X";
					}
					else{
						row["isMine"]="";
					}
					row["Difficulty"]=raw.Rows[i]["Difficulty"].ToString();
					row["Description"]=raw.Rows[i]["Description"].ToString();
					row["Weight"]=raw.Rows[i]["Weight"].ToString();
					table.Rows.Add(row);
				}
				#endregion
			},false);
			List<FeatureRequest> listFeatureRequests=new List<FeatureRequest>();
			foreach(DataRow dataRow in table.Rows) {
				FeatureRequest req=new FeatureRequest();
				#region Convert DataTable Into FeatureRequest
				long.TryParse(dataRow["RequestId"].ToString(),out req.FeatReqNum);
				string[] votes=dataRow["totalVotes"].ToString().Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
				string vote=votes.FirstOrDefault(x => !x.StartsWith("Critical") && !x.StartsWith("$"));
				if(!string.IsNullOrEmpty(vote)) {
					long.TryParse(vote,out req.Votes);
				}
				vote=votes.FirstOrDefault(x => x.StartsWith("Critical"));
				if(!string.IsNullOrEmpty(vote)) {
					long.TryParse(vote,out req.Critical);
				}
				vote=votes.FirstOrDefault(x => x.StartsWith("$"));
				if(!string.IsNullOrEmpty(vote)) {
					float.TryParse(vote,out req.Pledge);
				}
				req.Difficulty=PIn.Long(dataRow["Difficulty"].ToString());
				req.Weight=PIn.Float(dataRow["Weight"].ToString());
				req.Approval=dataRow["Weight"].ToString();
				req.Description=dataRow["Description"].ToString();
				#endregion
				listFeatureRequests.Add(req);
			}
			return listFeatureRequests;
		}

	}
}