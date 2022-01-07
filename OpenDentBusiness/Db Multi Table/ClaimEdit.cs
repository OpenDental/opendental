using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	public class ClaimEdit {

		public static LoadData GetLoadData(Patient pat,Family fam,Claim claim) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),pat,fam,claim);
			}
			LoadData data=new LoadData();
			data.ListPatPlans=PatPlans.Refresh(pat.PatNum);
			data.ListInsSubs=InsSubs.RefreshForFam(fam);
			data.ListInsPlans=InsPlans.RefreshForSubList(data.ListInsSubs);
			data.ListClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			data.ListProcs=Procedures.Refresh(pat.PatNum);
			data.ListClaimValCodes=ClaimValCodeLogs.GetForClaim(claim.ClaimNum);
			data.ClaimCondCodeLogCur=ClaimCondCodeLogs.GetByClaimNum(claim.ClaimNum);
			data.TablePayments=ClaimPayments.GetForClaim(claim.ClaimNum);
			data.TablePayments.TableName="ClaimPayments";
			data.ListToothInitials=ToothInitials.Refresh(pat.PatNum);
			data.ListCustomStatusEntries=ClaimTrackings.RefreshForClaim(ClaimTrackingType.StatusHistory,claim.ClaimNum);
			data.DoShowPatResp=PrefC.GetEnum<YN>(PrefName.ClaimEditShowPatResponsibility)==YN.Yes
				|| PrefC.GetEnum<YN>(PrefName.ClaimEditShowPatResponsibility)==YN.Unknown && !PrefC.GetYN(PrefName.ClaimEditShowPayTracking);
			return data;
		}

		///<summary>Updates the claim to the database.</summary>
		public static UpdateData UpdateClaim(Claim ClaimCur,List<ClaimValCodeLog> listClaimValCodes,ClaimCondCodeLog claimCondCodeLog,
			List<Procedure> listProcsToUpdatePlaceOfService,Patient pat,bool doMakeSecLog,Permissions permissionToLog) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UpdateData>(MethodBase.GetCurrentMethod(),ClaimCur,listClaimValCodes,claimCondCodeLog,listProcsToUpdatePlaceOfService,
					pat,doMakeSecLog,permissionToLog);
			}
			UpdateData data=new UpdateData();
			Claims.Update(ClaimCur);
			if(listClaimValCodes!=null) {
				ClaimValCodeLogs.UpdateList(listClaimValCodes);
			}
			if(claimCondCodeLog!=null) {
				if(claimCondCodeLog.IsNew) {
					ClaimCondCodeLogs.Insert(claimCondCodeLog);
				}
				else {
					ClaimCondCodeLogs.Update(claimCondCodeLog);
				}
			}
			foreach(Procedure proc in listProcsToUpdatePlaceOfService) {
				Procedure oldProc=proc.Copy();
				proc.PlaceService=ClaimCur.PlaceService;
				Procedures.Update(proc,oldProc);
			}
			if(doMakeSecLog) {
				SecurityLogs.MakeLogEntry(permissionToLog,ClaimCur.PatNum,
					pat.GetNameLF()+", Date of service: "+ClaimCur.DateService.ToShortDateString());
			}
			data.ListSendQueueItems=Claims.GetQueueList(ClaimCur.ClaimNum,ClaimCur.ClinicNum,0);
			return data;
		}

		///<summary>Most of the data needed when updating a claim.</summary>
		[Serializable]
		public class UpdateData {
			public ClaimSendQueueItem[] ListSendQueueItems;
		}

		///<summary>Most of the data needed to load FormClaimEdit.</summary>
		[Serializable]
		public class LoadData {
			public List<PatPlan> ListPatPlans;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			public List<ClaimProc> ListClaimProcs;
			public List<Procedure> ListProcs;
			public List<ClaimValCodeLog> ListClaimValCodes;
			public ClaimCondCodeLog ClaimCondCodeLogCur;
			[XmlIgnore]
			public DataTable TablePayments;
			public List<ToothInitial> ListToothInitials;
			public List<ClaimTracking> ListCustomStatusEntries;
			///<summary>True if the Pat Resp column is showing in gridProc.</summary>
			public bool DoShowPatResp;

			[XmlElement(nameof(TablePayments))]
			public string TablePaymentsXml {
				get {
					if(TablePayments==null) {
						return null;
					}
					return XmlConverter.TableToXml(TablePayments);
				}
				set {
					if(value==null) {
						TablePayments=null;
						return;
					}
					TablePayments=XmlConverter.XmlToTable(value);
				}
			}
		}
	}
}
