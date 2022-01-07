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

		///<summary>Gets the data necesary to load FormApptEdit.</summary>
		public static LoadData GetLoadData(Appointment AptCur,bool IsNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),AptCur,IsNew);
			}
			LoadData data=new LoadData();
			data.ListProcsForAppt=Procedures.GetProcsForApptEdit(AptCur);
			data.ListAppointments=Appointments.GetAppointmentsForProcs(data.ListProcsForAppt);
			data.Family=Patients.GetFamily(AptCur.PatNum);
			data.ListPatPlans=PatPlans.Refresh(AptCur.PatNum);
			data.ListInsSubs=InsSubs.RefreshForFam(data.Family);
			data.ListBenefits=Benefits.Refresh(data.ListPatPlans,data.ListInsSubs);
			data.ListClaimProcHists=ClaimProcs.GetHistList(AptCur.PatNum,data.ListBenefits,data.ListPatPlans,data.ListInsPlans,DateTime.Today,data.ListInsSubs);
			data.ListInsPlans=InsPlans.RefreshForSubList(data.ListInsSubs);
			data.TableApptFields=Appointments.GetApptFields(AptCur.AptNum);
			data.TableComms=Appointments.GetCommTable(AptCur.PatNum.ToString(),AptCur.AptNum);
			data.Lab=(IsNew ? null : LabCases.GetForApt(AptCur));
			data.PatientTable=Appointments.GetPatTable(AptCur.PatNum.ToString(),AptCur);
			data.ListClaimProcs=ClaimProcs.RefreshForProcs(data.ListProcsForAppt.Select(x => x.ProcNum).ToList());
			data.ListAdjustments=Adjustments.GetForProcs(data.ListProcsForAppt.Select(x => x.ProcNum).ToList());
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				data.ListStudents=ReqStudents.GetForAppt(AptCur.AptNum);
			}
			return data;
		}

		///<summary>Adds procedures to the appointment.</summary>
		///<returns>First item of tuple is the newly added procedures. Second item is all procedures for the appointment.</returns>
		public static ODTuple<List<Procedure>,List<Procedure>> QuickAddProcs(Appointment apt,Patient pat,List<string> listProcCodesToAdd,long provNum,
			long provHyg,List<InsSub> SubList,List<InsPlan> listInsPlans,List<PatPlan> listPatPlans,List<Benefit> listBenefits) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ODTuple<List<Procedure>,List<Procedure>>>(MethodBase.GetCurrentMethod(),apt,pat,listProcCodesToAdd,provNum,
					provHyg,SubList,listInsPlans,listPatPlans,listBenefits);
			}
			Procedures.SetDateFirstVisit(apt.AptDateTime.Date,1,pat);
			List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(apt.PatNum);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			foreach(string procCode in listProcCodesToAdd) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(procCode));
			}
			List<long> listProvNumsTreat=new List<long>();
			listProvNumsTreat.Add(provNum);
			listProvNumsTreat.Add(provHyg);//these were both passed in
			List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);//not available in FormApptEdit
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum,apt.AptDateTime);
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,null,//no procs to pull medicalCodes from
				listProvNumsTreat,pat.PriProv,pat.SecProv,pat.FeeSched,
				listInsPlans,new List<long>(){apt.ClinicNum},null,//procNums for appt already handled above
				listSubstLinks,discountPlanNum);
				//null,listProvNumsTreat,listProcedureCodes.Select(x=>x.ProvNumDefault).ToList(),
				//pat.PriProv,pat.SecProv,pat.FeeSched,listInsPlans,new List<long>(){apt.ClinicNum},listProcCodesToAdd,null);//procnums for appt already handled above
			List<Procedure> listAddedProcs=new List<Procedure>();
			//Make a copy of apt with provNum and provHyg, in order to maintain behavior of this method prior to using Procedures.ConstructProcedureForAppt
			//provNum and provHyg are sent in and are the selected provs in FormApptEdit, which may be different than the current provs on apt
			Appointment aptCur=apt.Copy();
			aptCur.ProvNum=provNum;
			aptCur.ProvHyg=provHyg;
			foreach(string procCode in listProcCodesToAdd) {
				ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(procCode);
				Procedure proc=Procedures.ConstructProcedureForAppt(procCodeCur.CodeNum,aptCur,pat,listPatPlans,listInsPlans,SubList,listFees);
				Procedures.Insert(proc);//recall synch not required
				Procedures.ComputeEstimates(proc,pat.PatNum,ref ClaimProcList,true,listInsPlans,listPatPlans,listBenefits,
					null,null,true,
					pat.Age,SubList,
					null,false,false,listSubstLinks,false,
					listFees);
				listAddedProcs.Add(proc);
			}
			return new ODTuple<List<Procedure>,List<Procedure>>(listAddedProcs,Procedures.GetProcsForApptEdit(apt));
		}

		///<summary>The data necesary to load FormApptEdit.</summary>
		[Serializable]
		public class LoadData {
			public List<Procedure> ListProcsForAppt;
			public List<Appointment> ListAppointments;
			public Family Family;
			public List<PatPlan> ListPatPlans;
			public List<Benefit> ListBenefits;
			public List<ClaimProcHist> ListClaimProcHists;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			[XmlIgnore]
			public DataTable TableApptFields;
			[XmlIgnore]
			public DataTable TableComms;
			public LabCase Lab;
			[XmlIgnore]
			public DataTable PatientTable;
			public List<ReqStudent> ListStudents;
			public List<ClaimProc> ListClaimProcs;
			public List<Adjustment> ListAdjustments;


			[XmlElement(nameof(TableApptFields))]
			public string TableApptFieldsXml {
				get {
					if(TableApptFields==null) {
						return null;
					}
					return XmlConverter.TableToXml(TableApptFields);
				}
				set {
					if(value==null) {
						TableApptFields=null;
						return;
					}
					TableApptFields=XmlConverter.XmlToTable(value);
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

			[XmlElement(nameof(PatientTable))]
			public string PatientTableXml {
				get {
					if(PatientTable==null) {
						return null;
					}
					return XmlConverter.TableToXml(PatientTable);
				}
				set {
					if(value==null) {
						PatientTable=null;
						return;
					}
					PatientTable=XmlConverter.XmlToTable(value);
				}
			}

		}
	}
}
