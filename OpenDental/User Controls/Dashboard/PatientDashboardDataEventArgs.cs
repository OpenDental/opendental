using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This class is in flux. We are moving away from the LoadData model and will instead be using the PatientData object in the future. But it has to be changed over gradually, so this class will be a little bit chaotic during the transition.</summary>
	public class PatientDashboardDataEventArgs : IDisposable {
		public Family Fam;
		public Patient Pat;
		public PatientNote PatNote;
		public List<InsSub> ListInsSubs;
		public List<InsPlan> ListInsPlans;
		public List<PatPlan> ListPatPlans;
		public List<Benefit> ListBenefits;
		public DiscountPlan DiscountPlan;
		public DiscountPlanSub DiscountPlanSub;
		public List<Claim> ListClaims;
		public List<ClaimProcHist> HistList;
		public List<Procedure> ListProcedures;
		public List<TreatPlan> ListTreatPlans;
		public List<Document> ListDocuments;
		public List<Appointment> ListAppts;
		public List<PlannedAppt> ListPlannedAppts;
		public List<ToothInitial> ListToothInitials;
		public Patient SuperFamHead;
		public DataTable TableProgNotes;
		public Image ImageToothChart;
		public Bitmap BitmapImagesModule;
		public StaticTextData StaticTextData;

		public PatientDashboardDataEventArgs() {
			//This is used for Images module and for Chart module

		}

		public PatientDashboardDataEventArgs(object data) {
			if(data==null) {
				return;
			}
			if(data.GetType()==typeof(PatientDashboardDataEventArgs)) {//Images module
				PatientDashboardDataEventArgsFactory((PatientDashboardDataEventArgs)data);
			}
			else if(data.GetType()==typeof(Patient)) {
				PatientDashboardDataEventArgsFactory((Patient)data);
			}
			else if(data.GetType()==typeof(FamilyModules.LoadData)) {
				PatientDashboardDataEventArgsFactory((FamilyModules.LoadData)data);
			}
			else if(data.GetType()==typeof(AccountModules.LoadData)) {
				PatientDashboardDataEventArgsFactory((AccountModules.LoadData)data);
			}
			else if(data.GetType()==typeof(TPModuleData)) {
				PatientDashboardDataEventArgsFactory((TPModuleData)data);
			}
			//else if(data.GetType()==typeof(ChartModules.LoadData)) {
			//	PatientDashboardDataEventArgsFactory((ChartModules.LoadData)data);
			//}
			else {
				throw new NotImplementedException("Unable to import "+data.GetType()+" into PatientDashboardDataEventArgs.");//Unexpected data object.
			}
			//No "LoadData" class for Images and Manage modules.
			CreateStaticTextData();
		}

		//Appointment Module
		private void PatientDashboardDataEventArgsFactory(Patient data) {
			Pat=data;
		}

		//Images Module
		private void PatientDashboardDataEventArgsFactory(PatientDashboardDataEventArgs data) {
			Fam=data.Fam;
			Pat=data.Pat;
			PatNote=data.PatNote;
			ListInsSubs=data.ListInsSubs;
			ListInsPlans=data.ListInsPlans;
			ListPatPlans=data.ListPatPlans;
			ListBenefits=data.ListBenefits;
			DiscountPlan=data.DiscountPlan;
			ListClaims=data.ListClaims;
			HistList=data.HistList;
			ListProcedures=data.ListProcedures;
			ListTreatPlans=data.ListTreatPlans;
			ListDocuments=data.ListDocuments;
			ListAppts=data.ListAppts;
			ListPlannedAppts=data.ListPlannedAppts;
			ListToothInitials=data.ListToothInitials;
			TableProgNotes=data.TableProgNotes;
			#region IDisposables
			//There might not be enough memory available to load the images passed in.
			ODException.SwallowAnyException(() => ImageToothChart=data.ImageToothChart is null ? null : new Bitmap(data.ImageToothChart));
			ODException.SwallowAnyException(() => BitmapImagesModule=data.BitmapImagesModule is null ? null : new Bitmap(data.BitmapImagesModule));
			#endregion
			StaticTextData=data.StaticTextData;
		}

		public void Dispose() {
			ImageToothChart?.Dispose();
			ImageToothChart=null;
			BitmapImagesModule?.Dispose();
			BitmapImagesModule=null;
		}

		//Family Module
		private void PatientDashboardDataEventArgsFactory(FamilyModules.LoadData data) {
			Fam=data.Fam;
			Pat=data.Pat;
			PatNote=data.PatNote;
			ListInsSubs=data.ListInsSubs;
			ListInsPlans=data.ListInsPlans;
			ListPatPlans=data.ListPatPlans;
			ListBenefits=data.ListBenefits;
			DiscountPlan=data.DiscountPlan;
		}

		//Account Module
		private void PatientDashboardDataEventArgsFactory(AccountModules.LoadData data) {
			//DataSetMain=data.DataSetMain;  Not implementing this for now, as I don't think it's necessary.
			Fam=data.Fam;
			PatNote=data.PatNote;
			ListInsSubs=data.ListInsSubs;
			ListInsPlans=data.ListInsPlans;
			ListPatPlans=data.ListPatPlans;
			ListBenefits=data.ListBenefits;
			ListClaims=data.ListClaims;
			HistList=data.HistList;
			//AccountModules.LoadData does not have a Pat, so we will figure it out from the data we do have.
			long patNum=GetPatNumFromData();
			Pat=Fam?.GetPatient(patNum);//No db calls.
		}

		//Treatment Plan Module
		private void PatientDashboardDataEventArgsFactory(TPModuleData data) {
			Fam=data.Fam;
			Pat=data.Pat;
			DiscountPlan=data.DiscountPlan;
			DiscountPlanSub=data.DiscountPlanSub;
			ListInsSubs=data.SubList;
			ListInsPlans=data.InsPlanList;
			ListPatPlans=data.PatPlanList;
			ListBenefits=data.BenefitList;
			ListClaims=data.ClaimList;
			HistList=data.HistList;
			ListProcedures=data.ListProcedures;
			ListTreatPlans=data.ListTreatPlans;
		}

		public List<PlannedAppt> ExtractPlannedAppts(Patient pat,DataTable tablePlannedAppts) {
			List<PlannedAppt> listPlannedAppts=new List<PlannedAppt>();
			//Essentially a TableToList method, but since the DataTable doesn't have a PatNum row, pull from the Pat object.
			for(int i=0;i<tablePlannedAppts.Rows.Count;i++) {
				DataRow row=tablePlannedAppts.Rows[i];
				PlannedAppt plannedAppt=new PlannedAppt();
				plannedAppt.PlannedApptNum= PIn.Long  (row["PlannedApptNum"].ToString());
				plannedAppt.PatNum        = pat.PatNum;//data.TablePlannedAppts will not have PatNum column.
				plannedAppt.AptNum        = PIn.Long  (row["AptNum"].ToString());
				plannedAppt.ItemOrder     = PIn.Int   (row["ItemOrder"].ToString());
				listPlannedAppts.Add(plannedAppt);
			}
			return listPlannedAppts.OrderBy(x => x.ItemOrder).ToList();
		}

		private void CreateStaticTextData() {
			if(Pat==null) {
				return;
			}
			//Procedure CodeNums-------------------------------------------------------------------------------------------------------------
			//Bracketed proccodes are based on prefs and are subject to change
			//The local lists made for each proccode are initialized here to speed up the looping logic below
			List<long> listProcCodeNums=new List<long>();
			//listInsBenPanoCodes consists of the following proccodes by default: [D0210, D0330]
			List<long> listInsBenPanoCodes=new List<long>(); //Used Later
			//listInsBenExamCodes consists of the following proccodes by default: [D0120, D0150]
			List<long> listInsBenExamCodes=new List<long>(); //Used Later
			//listInsBenProphyCodes consists of the following proccodes by default: [D1110, D1120]
			List<long> listInsBenProphyCodes=new List<long>(); //Used later
			//listInsBenBWCodes consists of the following proccodes by default: [D0272,D0274]
			List<long> listInsBenBWCodes=new List<long>(); //Used later
			//listInsBenPerioMaintCodes consists of the following proccodes by default: [D4910]
			List<long> listInsBenPerioMaintCodes=new List<long>(); //Used later
			//listInsBenSRPCodes consists of the following proccodes by default: [D4341, D4342]
			List<long> listInsBenSRPCodes=new List<long>(); //Used later
			listProcCodeNums=SheetFiller.GetListProcCodeNumsForStaticText(listInsBenPanoCodes,listInsBenExamCodes,listInsBenProphyCodes,ref listInsBenPerioMaintCodes
			,ref listInsBenBWCodes,ref listInsBenSRPCodes);
			//Create a new StaticTextData object, using data we already have, supplementing with queried data where necessary.
			StaticTextData=new StaticTextData();
			StaticTextData.PatNote=PatNote;
			StaticTextData.ListInsSubs=ListInsSubs;
			StaticTextData.ListInsPlans=ListInsPlans;
			StaticTextData.ListPatPlans=ListPatPlans;
			StaticTextData.ListBenefits=ListBenefits;
			StaticTextData.HistList=HistList;
			StaticTextData.ListTreatPlans=ListTreatPlans;
			StaticTextData.ListAppts=ListAppts;
			StaticTextData.ListFutureApptsForFam=ListAppts?.FindAll(x => x.AptDateTime>DateTime.Now && x.AptStatus==ApptStatus.Scheduled);
			//StaticTextData.ListFamPopups=Popups.GetForFamily(Pat);//Will be handled by StaticTextData.GetStaticTextData() if necessary.
			StaticTextData.ListProceduresSome=ListProcedures?.FindAll(x => listProcCodeNums.Contains(x.CodeNum));
			StaticTextData.ListDocuments=ListDocuments;
			StaticTextData.ListProceduresPat=ListProcedures;
			StaticTextData.ListPlannedAppts=ListPlannedAppts;
		}

		private long GetPatNumFromData() {
			if(PatNote!=null) {
				return PatNote.PatNum;
			}
			else if(ListClaims!=null && ListClaims.Count>0) {
				return ListClaims.FirstOrDefault().PatNum;
			}
			else if(ListPatPlans!=null && ListPatPlans.Count>0) {
				return ListPatPlans.FirstOrDefault().PatNum;
			}
			return 0;
		}
	}
}
