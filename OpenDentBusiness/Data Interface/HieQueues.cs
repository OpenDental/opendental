using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.FileIO;

namespace OpenDentBusiness {
	///<summary></summary>
	public class HieQueues{
		#region Methods - Get
		public static List<HieQueue> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HieQueue>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hiequeue";
			return Crud.HieQueueCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<HieQueue> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HieQueue>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM hiequeue WHERE PatNum = "+POut.Long(patNum);
			return Crud.HieQueueCrud.SelectMany(command);
		}
		
		///<summary>Gets one HieQueue from the db.</summary>
		public static HieQueue GetOne(long hieQueueNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<HieQueue>(MethodBase.GetCurrentMethod(),hieQueueNum);
			}
			return Crud.HieQueueCrud.SelectOne(hieQueueNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(HieQueue hieQueue){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
			
			hieQueue.HieQueueNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hieQueue);
				return hieQueue.HieQueueNum;
			}
			return Crud.HieQueueCrud.Insert(hieQueue);
		}
		///<summary></summary>
		public static void Update(HieQueue hieQueue){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hieQueue);
				return;
			}
			Crud.HieQueueCrud.Update(hieQueue);
		}
		///<summary></summary>
		public static void Delete(long hieQueueNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hieQueueNum);
				return;
			}
			Crud.HieQueueCrud.Delete(hieQueueNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		///<summary>Process outstanding queue records. Does not create ccds if running unit tests.</summary>
		public static string ProcessQueues() {
			//No need to check RemotingRole; no call to db.
			List<HieClinic> listHieClinicEnabled=HieClinics.GetAllEnabled();
			if(listHieClinicEnabled.IsNullOrEmpty()) {
				return "No HIE clinics enabled.";
			}
			List<HieQueue> listHieQueuesAll=GetAll();
			if(listHieQueuesAll.IsNullOrEmpty()) {
				return "No HIE queues to process.";
			}
			List<HieQueue> listHieQueuesToProcess=listHieQueuesAll.DistinctBy(x => x.PatNum).ToList();
			List<long> listPatNumsDistinct=listHieQueuesToProcess.Select(x => x.PatNum).Distinct().ToList();
			List<Patient> listPatients=Patients.GetMultPats(listPatNumsDistinct).ToList();
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPats(listPatNumsDistinct);
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			List<InsPlan> listInsPlans=InsPlans.GetPlans(listInsSubs.Select(x => x.PlanNum).ToList());
			List<Carrier> listCarriers=Carriers.GetCarriers(listInsPlans.Select(x => x.CarrierNum).ToList());
			HieClinic hieClinicHQ=listHieClinicEnabled.FirstOrDefault(x => x.ClinicNum==0);
			List<long> listPatNumsProcessed=new List<long>();
			//Hie queue rows should only be valid for one day. This list will keep track of patnums that could not be processed.
			List<long> listPatNumsFiltered=new List<long>();
			List<string> listLogMsgs=new List<string>();
			for(int i=0;i<listHieQueuesToProcess.Count;i++) {
				try {
					Patient patCur=listPatients.FirstOrDefault(x => x.PatNum==listHieQueuesToProcess[i].PatNum)??Patients.GetPat(listHieQueuesToProcess[i].PatNum);
					if(patCur==null) {
						listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
						listLogMsgs.Add($"Patient object could not be found for PatNum '{listHieQueuesToProcess[i].PatNum}'.");
						continue;
					}
					//Get HIE clinic that is associated to the patient's clinic
					HieClinic hieClinicForPat=listHieClinicEnabled.FirstOrDefault(x => x.ClinicNum==patCur.ClinicNum)??hieClinicHQ;
					if(hieClinicForPat==null) {
						listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
						listLogMsgs.Add($"No HIE clinic found for ClinicNum '{patCur.ClinicNum}' or HQ.");
						continue;
					}
					if(!hieClinicForPat.IsTimeToProcess()) {
						listLogMsgs.Add($"HIE clinic '{Clinics.GetDesc(hieClinicForPat.ClinicNum)}' was skipped because time of day not between {hieClinicForPat.TimeOfDayExportCCD.ToShortTimeString()} and {hieClinicForPat.TimeOfDayExportCCDEnd.ToShortTimeString()}.");
						continue;
					}
					if(!Directory.Exists(hieClinicForPat.PathExportCCD) && !ODInitialize.IsRunningInUnitTest) {
						listLogMsgs.Add($"Export directory does not exist '{hieClinicForPat.PathExportCCD}' for HIE clinic number '{hieClinicForPat.HieClinicNum}'.");
						continue;
					}
					if(hieClinicForPat.SupportedCarrierFlags!=HieCarrierFlags.AllCarriers) {//hieclinic has medicaid flag
						List<PatPlan> listPatPlansForPat=listPatPlans.FindAll(x => x.PatNum==patCur.PatNum);
						if(listPatPlansForPat.IsNullOrEmpty()) {
							listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
							continue;
						}
						List<InsSub> listInsSubsForPat=listInsSubs.FindAll(x => ListTools.In(x.InsSubNum,listPatPlansForPat.Select(x => x.InsSubNum).ToList()));
						if(listInsSubsForPat.IsNullOrEmpty()) {
							listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
							continue;
						}
						List<InsPlan> listInsPlansForPat=listInsPlans.FindAll(x => ListTools.In(x.PlanNum,listInsSubsForPat.Select(x => x.PlanNum)));
						if(listInsPlansForPat.IsNullOrEmpty()) {
							listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
							continue;
						}
						List<Carrier> listCarriersForPat=listCarriers.FindAll(x => ListTools.In(x.CarrierNum,listInsPlansForPat.Select(x => x.CarrierNum).ToList()));
						if(hieClinicForPat.SupportedCarrierFlags.HasFlag(HieCarrierFlags.Medicaid) && !listCarriers.Any(x => ElectIDs.IsMedicaid(x.ElectID))) {
							//Patient does not have any Medicaid insurance plans, continue.
							listPatNumsFiltered.Add(listHieQueuesToProcess[i].PatNum);
							continue;
						}
					}
					if(!ODInitialize.IsRunningInUnitTest) {//Don't create ccd export if running Unit Tests
						//Process summary of care for the patient to the export path specified.
						string ccdTextForPat=EhrCCD.GenerateSummaryOfCare(patCur,canValidate:false);
						string pathCcdExportWOExt=ODFileUtils.CombinePaths(hieClinicForPat.PathExportCCD,$"ccd_{DateTime_.Now.ToString("yyyyMMdd")}_{patCur.PatNum}");
						string fileExt=".xml";
						string pathCcdExport=pathCcdExportWOExt+fileExt;
						if(File.Exists(pathCcdExport)) {
							int loopCount=1;
							while(File.Exists(pathCcdExportWOExt+$"_{loopCount}{fileExt}")) {
								loopCount++;
							}
							pathCcdExport=pathCcdExportWOExt+$"_{loopCount}{fileExt}";
						}
						try {
							File.WriteAllText(pathCcdExport,ccdTextForPat);
						}
						catch(Exception ex) {
							listLogMsgs.Add($"Failed to write ccd file for PatNum {patCur.PatNum} to path {pathCcdExport}.\r\n{ex.Message}");
							continue;//We will try again later.
						}
					}
					EhrMeasureEvents.CreateEventForPat(patCur.PatNum,EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic);
					listPatNumsProcessed.Add(patCur.PatNum);
				}
				catch(Exception ex) {
					listLogMsgs.Add($"Failed to generate CCD for PatNum {listHieQueuesToProcess[i].PatNum}.");
					listLogMsgs.Add($"Error:{ex.Message}");
				}
			}
			List<long> listHieQueueNumsToDeleted=new List<long>();
			if(!listPatNumsFiltered.IsNullOrEmpty()) {
				listLogMsgs.Add($"Count of HIE queues filtered {listHieQueuesToProcess.Count}.");
				//Get the list of hiequeues for patnums that we could not process in memory to delete.
				listHieQueueNumsToDeleted
					.AddRange(listHieQueuesAll.Where(x => ListTools.In(x.PatNum,listPatNumsFiltered)).Select(x => x.HieQueueNum).ToList());
			}
			if(!listPatNumsProcessed.IsNullOrEmpty()) {
				listLogMsgs.Add($"Count of HIE queues processed {listPatNumsProcessed.Count}.");
				//Get the list of hiequeues for patnums that were processed in memory to delete.
				listHieQueueNumsToDeleted
					.AddRange(listHieQueuesAll.Where(x => ListTools.In(x.PatNum,listPatNumsProcessed)).Select(x => x.HieQueueNum).ToList());
			}
			for(int i=0;i<listHieQueueNumsToDeleted.Count;i++) {
				Delete(listHieQueueNumsToDeleted[i]);
			} 
			return string.Join("\r\n",listLogMsgs.Distinct());
		}
		#endregion Methods - Misc



	}
}