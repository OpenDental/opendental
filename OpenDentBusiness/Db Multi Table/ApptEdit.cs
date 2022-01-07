using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	public class ApptEdit {

		///<summary>Gets the data necessary to load FormApptEdit.</summary>
		public static LoadData GetLoadData(Appointment appointment,bool isNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),appointment,isNew);
			}
			LoadData loadData=new LoadData();
			loadData.ListProceduresForAppointment=Procedures.GetProcsForApptEdit(appointment);
			loadData.ListAppointments=Appointments.GetAppointmentsForProcs(loadData.ListProceduresForAppointment);
			loadData.Family=Patients.GetFamily(appointment.PatNum);
			loadData.ListPatPlans=PatPlans.Refresh(appointment.PatNum);
			loadData.ListInsSubs=InsSubs.RefreshForFam(loadData.Family);
			loadData.ListBenefits=Benefits.Refresh(loadData.ListPatPlans,loadData.ListInsSubs);
			loadData.ListClaimProcHists=ClaimProcs.GetHistList(appointment.PatNum,loadData.ListBenefits,loadData.ListPatPlans,loadData.ListInsPlans,DateTime.Today,loadData.ListInsSubs);
			loadData.ListInsPlans=InsPlans.RefreshForSubList(loadData.ListInsSubs);
			loadData.TableApppointmentFields=Appointments.GetApptFields(appointment.AptNum);
			loadData.TableComms=Appointments.GetCommTable(appointment.PatNum.ToString(),appointment.AptNum);
			loadData.LabCase=(isNew ? null : LabCases.GetForApt(appointment));
			loadData.TablePatients=Appointments.GetPatTable(appointment.PatNum.ToString(),appointment);
			loadData.ListClaimProcs=ClaimProcs.RefreshForProcs(loadData.ListProceduresForAppointment.Select(x => x.ProcNum).ToList());
			loadData.ListAdjustments=Adjustments.GetForProcs(loadData.ListProceduresForAppointment.Select(x => x.ProcNum).ToList());
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				loadData.ListReqStudents=ReqStudents.GetForAppt(appointment.AptNum);
			}
			return loadData;
		}

		///<summary>Returns a list of selected procedures from the QuickAdd list that are being added to the appointment.</summary>
		public static List<Procedure> QuickAddProcs(Appointment appointment,Patient patient,List<string> listProcCodesToAdd,long provNum,
			long provHyg,List<InsSub> listInsSubs,List<InsPlan> listInsPlans,List<PatPlan> listPatPlans,List<Benefit> listBenefits) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),appointment,patient,listProcCodesToAdd,provNum,
					provHyg,listInsSubs,listInsPlans,listPatPlans,listBenefits);
			}
			Procedures.SetDateFirstVisit(appointment.AptDateTime.Date,1,patient);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(appointment.PatNum);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			for(int i=0;i<listProcCodesToAdd.Count;i++) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(listProcCodesToAdd[i]));
			}
			List<long> listProvNumsTreat=new List<long>();
			listProvNumsTreat.Add(provNum);
			listProvNumsTreat.Add(provHyg);//these were both passed in
			List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);//not available in FormApptEdit
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(patient.PatNum,appointment.AptDateTime);
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,listMedicalCodes: null,//no procs to pull medicalCodes from
				listProvNumsTreat,patient.PriProv,patient.SecProv,patient.FeeSched,
				listInsPlans,new List<long>(){appointment.ClinicNum},listAppts: null,//procNums for appt already handled above
				listSubstitutionLinks,discountPlanNum);
				//null,listProvNumsTreat,listProcedureCodes.Select(x=>x.ProvNumDefault).ToList(),
				//pat.PriProv,pat.SecProv,pat.FeeSched,listInsPlans,new List<long>(){apt.ClinicNum},listProcCodesToAdd,null);//procnums for appt already handled above
			List<Procedure> listProceduresAdded=new List<Procedure>();
			//Make a copy of apt with provNum and provHyg, in order to maintain behavior of this method prior to using Procedures.ConstructProcedureForAppt
			//provNum and provHyg are sent in and are the selected provs in FormApptEdit, which may be different than the current provs on apt
			Appointment appointmentCur=appointment.Copy();
			appointmentCur.ProvNum=provNum;
			appointmentCur.ProvHyg=provHyg;
			for(int i=0;i<listProcCodesToAdd.Count;i++) {
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcCodesToAdd[i]);
				Procedure procedure=Procedures.ConstructProcedureForAppt(procedureCode.CodeNum,appointmentCur,patient,listPatPlans,listInsPlans,listInsSubs,listFees);
				Procedures.Insert(procedure);//recall synch not required
				Procedures.ComputeEstimates(procedure,patient.PatNum,ref listClaimProcs,isInitialEntry: true,listInsPlans,listPatPlans,listBenefits,
					histList: null,loopList: null,saveToDb: true,patient.Age,listInsSubs,listClaimProcsAll: null,isClaimProcRemoveNeeded: false,
					useProcDateOnProc: false,listSubstitutionLinks,isForOrtho: false,listFees);
				listProceduresAdded.Add(procedure);
			}
			return new List<Procedure>(listProceduresAdded);
		}

		///<summary>The data necessary to load FormApptEdit.</summary>
		[Serializable]
		public class LoadData {
			public List<Procedure> ListProceduresForAppointment;
			public List<Appointment> ListAppointments;
			public Family Family;
			public List<PatPlan> ListPatPlans;
			public List<Benefit> ListBenefits;
			public List<ClaimProcHist> ListClaimProcHists;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			[XmlIgnore]
			public DataTable TableApppointmentFields;
			[XmlIgnore]
			public DataTable TableComms;
			public LabCase LabCase;
			[XmlIgnore]
			public DataTable TablePatients;
			public List<ReqStudent> ListReqStudents;
			public List<ClaimProc> ListClaimProcs;
			public List<Adjustment> ListAdjustments;


			[XmlElement(nameof(TableApppointmentFields))]
			public string TableApptFieldsXml {
				get {
					if(TableApppointmentFields==null) {
						return null;
					}
					return XmlConverter.TableToXml(TableApppointmentFields);
				}
				set {
					if(value==null) {
						TableApppointmentFields=null;
						return;
					}
					TableApppointmentFields=XmlConverter.XmlToTable(value);
				}
			}

			[XmlElement(nameof(TableComms))]
			public string TableCommsXml {
				get {
					if(TableComms==null) {
						return null;
					}
					return XmlConverter.TableToXml(TableComms);
				}
				set {
					if(value==null) {
						TableComms=null;
						return;
					}
					TableComms=XmlConverter.XmlToTable(value);
				}
			}

			[XmlElement(nameof(TablePatients))]
			public string PatientTableXml {
				get {
					if(TablePatients==null) {
						return null;
					}
					return XmlConverter.TableToXml(TablePatients);
				}
				set {
					if(value==null) {
						TablePatients=null;
						return;
					}
					TablePatients=XmlConverter.XmlToTable(value);
				}
			}

		}
	}
}
