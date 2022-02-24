using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class EServiceLogs {
		#region Get Methods
		///<summary>Gets one EServiceLog from the db.</summary>
		public static EServiceLog GetOne(long eServiceLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EServiceLog>(MethodBase.GetCurrentMethod(),eServiceLogNum);
			}
			return Crud.EServiceLogCrud.SelectOne(eServiceLogNum);
		}

		///<summary>Gets Table for specified clinic within date range.
		///If clinics are disabled or a -2 is passed in, the clinic filter will be ommitted.</summary>
		public static List<EServiceLog> GetEServiceLog(long clinicNum,DateTime startDate,DateTime endDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EServiceLog>>(MethodBase.GetCurrentMethod(),clinicNum,startDate,endDate);
			}
			string command=$"SELECT * FROM eservicelog WHERE LogDateTime BETWEEN {POut.DateT(startDate)} AND {POut.DateT(endDate)}";
			if(PrefC.HasClinicsEnabled && clinicNum!=-2) { //-2 is the 'All' identifier
				command+=$" AND ClinicNum={POut.Long(clinicNum)}";
			}
			return Crud.EServiceLogCrud.SelectMany(command);
		}

		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static long Insert(EServiceLog eServiceLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				eServiceLog.EServiceLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eServiceLog);
				return eServiceLog.EServiceLogNum;
			}
			return Crud.EServiceLogCrud.Insert(eServiceLog);
		}
		#endregion Modification Methods
		#region Misc Methods
		///<summary>Makes a new EServices log entry. PatNum can be 0.</summary>
		public static void MakeLogEntry(eServiceAction eServiceAction,eServiceType eServiceType,FKeyType keyType,
			long patNum=0,long clinicNum=0,long FKey=0,string logGuid="") {
			if(logGuid=="") {
				logGuid=Guid.NewGuid().ToString();
			}
			//No need to check RemotingRole; no call to db
			Insert(
				new EServiceLog() {
					LogGuid=logGuid,
					PatNum=patNum,
					ClinicNum=clinicNum,
					KeyType=keyType,
					FKey=FKey,
					EServiceAction=eServiceAction,
					EServiceType=eServiceType
				});
		}
		
		/// <summary>Pass in an eServiceType to retrieve a list of actions that are associated with that specific type. This list is not
		/// being alphabetized. The calling function will need to do that if it is required.
		/// Note: When adding an Action to this list remember to associate the action with the eServiceType AND the default value.</summary>
		public static List<eServiceAction> GetEServiceActions(eServiceType EServiceType) {
			List<eServiceAction> listEServiceActions=new List<eServiceAction>();
			switch(EServiceType) {
				case eServiceType.ApptConfirmations:
					listEServiceActions.Add(eServiceAction.CONFConfirmedAppt);
					break;
				case eServiceType.EClipboard:
					listEServiceActions.Add(eServiceAction.ECAddedForm);
					listEServiceActions.Add(eServiceAction.ECLoggedIn);
					listEServiceActions.Add(eServiceAction.ECCompletedForm);
					break;
				case eServiceType.PatientPortal:
					listEServiceActions.Add(eServiceAction.PPLoggedIn);
					listEServiceActions.Add(eServiceAction.PPMadePayment);
					break;
				case eServiceType.WebForms:
					listEServiceActions.Add(eServiceAction.WFCompletedForm);
					break;
				case eServiceType.WSGeneral:
				case eServiceType.WSNewPat:
				case eServiceType.WSExistingPat:
				case eServiceType.WSRecall:
				case eServiceType.WSAsap:
					listEServiceActions.Add(eServiceAction.WSHomeView);
					listEServiceActions.Add(eServiceAction.WSIdentify);
					listEServiceActions.Add(eServiceAction.WSMonthSwitch);
					listEServiceActions.Add(eServiceAction.WSDateTimeNo);
					listEServiceActions.Add(eServiceAction.WSDateTimeYes);
					listEServiceActions.Add(eServiceAction.WSMovedAppt);
					listEServiceActions.Add(eServiceAction.WSConfirmationPopup);
					listEServiceActions.Add(eServiceAction.WSAppointmentScheduledFromServer);
					listEServiceActions.Add(eServiceAction.WSAppointmentScheduleFromClient);
					listEServiceActions.Add(eServiceAction.WSRecallAlreadyScheduled);
					listEServiceActions.Add(eServiceAction.WSRecallNotFound);
					listEServiceActions.Add(eServiceAction.WSScheduler);
					listEServiceActions.Add(eServiceAction.WSServiceSelect);
					listEServiceActions.Add(eServiceAction.WSTimeSlotChoose);
					listEServiceActions.Add(eServiceAction.WSTwoFactorPassed);
					listEServiceActions.Add(eServiceAction.WSTwoFactorSent);
					break;
				default:
					listEServiceActions.Add(eServiceAction.WSHomeView);
					listEServiceActions.Add(eServiceAction.WSIdentify);
					listEServiceActions.Add(eServiceAction.WSMonthSwitch);
					listEServiceActions.Add(eServiceAction.WSDateTimeNo);
					listEServiceActions.Add(eServiceAction.WSDateTimeYes);
					listEServiceActions.Add(eServiceAction.WSMovedAppt);
					listEServiceActions.Add(eServiceAction.WSConfirmationPopup);
					listEServiceActions.Add(eServiceAction.WSAppointmentScheduledFromServer);
					listEServiceActions.Add(eServiceAction.WSAppointmentScheduleFromClient);
					listEServiceActions.Add(eServiceAction.WSRecallAlreadyScheduled);
					listEServiceActions.Add(eServiceAction.WSRecallNotFound);
					listEServiceActions.Add(eServiceAction.WSScheduler);
					listEServiceActions.Add(eServiceAction.WSServiceSelect);
					listEServiceActions.Add(eServiceAction.WSTimeSlotChoose);
					listEServiceActions.Add(eServiceAction.WSTwoFactorPassed);
					listEServiceActions.Add(eServiceAction.WSTwoFactorSent);
					listEServiceActions.Add(eServiceAction.ECAddedForm);
					listEServiceActions.Add(eServiceAction.ECLoggedIn);
					listEServiceActions.Add(eServiceAction.ECCompletedForm);
					listEServiceActions.Add(eServiceAction.CONFConfirmedAppt);
					listEServiceActions.Add(eServiceAction.WFCompletedForm);
					listEServiceActions.Add(eServiceAction.PPLoggedIn);
					listEServiceActions.Add(eServiceAction.PPMadePayment);
					break;
			}
			return listEServiceActions;
		}
		#endregion Misc Methods
	}
}