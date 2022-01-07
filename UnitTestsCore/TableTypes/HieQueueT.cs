using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class HieQueueT {
		///<summary>Clears the hiequeue table.</summary>
		public static void ClearHieQueueTable() {
			string command="DELETE FROM hiequeue WHERE HieQueueNum > 0";
			DataCore.NonQ(command);
		}

		///<summary>Creates Hie Queues for the Hie clinics passed in. Will create new patients for the numOfQueuesToCreate specified.</summary>
		public static List<HieQueue> CreateForHieClinics(List<HieClinic> listHieClinics,int numOfQueuesToCreate,bool doCreateMedicaidInsplan=true) {
			List<HieQueue> listHieQueues=new List<HieQueue>();
			Carrier carrierMedicaid=null;
			for(int i=0;i<listHieClinics.Count;i++) {
				for(int j=0;j<numOfQueuesToCreate;j++) {
					Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+listHieClinics[i].HieClinicNum,clinicNum:listHieClinics[i].ClinicNum);
					if(listHieClinics[i].SupportedCarrierFlags.HasFlag(HieCarrierFlags.Medicaid) && doCreateMedicaidInsplan) {
						if(carrierMedicaid==null) {
							carrierMedicaid=CreateMedicaidCarrier();
						}
						CreatePatPlanWithMedicaidCarrierToPatNum(patient.PatNum,carrierMedicaid.CarrierNum);
					}
					HieQueue hieQueue=new HieQueue(patient.PatNum);
					listHieQueues.Add(hieQueue);
					HieQueues.Insert(hieQueue);
				}
			}
			return listHieQueues;
		}

		public static List<HieQueue> GetAllForPats(List<long> listPatNums) {
			if(listPatNums.IsNullOrEmpty()) {
				return new List<HieQueue>();
			}
			return HieQueues.GetAll().FindAll(x => ListTools.In(x.PatNum,listPatNums));
		}

		///<summary>Returns a Medicaid Carrier. Clears all ElectID rows and inserts a carrier associated to a medicaid PayorID. Refreshes Carrier and ElectID caches.</summary>
		private static Carrier CreateMedicaidCarrier() {
			string command="DELETE FROM electid WHERE ElectIDNum>0";
			DataCore.NonQ(command);
			ElectIDs.RefreshCache();
			string carrierMedcaidName=MethodBase.GetCurrentMethod().Name+$"_Medicaid";
			ElectID electId=new ElectID();
			electId.PayorID=$"12345";
			electId.IsMedicaid=true;
			electId.CarrierName=carrierMedcaidName;
			ElectIDs.Insert(electId);
			ElectIDs.RefreshCache();
			Carrier carrierMedicaid=CarrierT.CreateCarrier(carrierMedcaidName,"123 boring st","boring","OR","97306",electID:electId.PayorID);
			Carriers.RefreshCache();
			return carrierMedicaid;
		}

		///<summary>Creates a patplan for the patient passed in with the Medicaid CarrierNum passed in.</summary>
		private static void CreatePatPlanWithMedicaidCarrierToPatNum(long patNum,long medicaidCarrierNum) {
			InsPlan insPlan=InsPlanT.CreateInsPlan(medicaidCarrierNum,groupNum:"XYZ123");//Group number should be >3 characters so it passes insverify validation
			InsSub insSub=InsSubT.CreateInsSub(patNum,insPlan.PlanNum);
			_=PatPlanT.CreatePatPlan(1,patNum,insSub.InsSubNum);
		}
	}
}
