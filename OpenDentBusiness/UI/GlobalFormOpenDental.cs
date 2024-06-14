using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>These global methods can be called from anywhere. FormOpenDental subscribes to the resulting events so that it can do things like refresh a module or select a patient. Even some classes in OpenDentBusiness take advantage of this, although those sections really don't belong in the business layer if they are telling FormOpenDental what to do. These will replace the public static methods in FormOpenDental that use the singleton pattern. The main reason for switching to this event pattern is that the WPF windows don't have access to FormOpenDental.</summary>
	public class GlobalFormOpenDental {
		public delegate void GoToModuleDelegate(EnumModuleType moduleType, DateTime? dateSelected=null,List<long> listPinApptNums=null,
			long selectedAptNum=0,long claimNum=0,long patNum=0,long docNum=0,bool doShowSearch=false);
		public static GoToModuleDelegate GoToModule;
		public static event EventHandler<ModuleEventArgs> EventModuleSelected;
		public static event EventHandler<PatientSelectedEventArgs> EventPatientSelected;
		public static event EventHandler<bool> EventRefreshCurrentModule;
		public static event EventHandler<bool> EventLockODForMountAcquire;
		public delegate bool SendTextDelegate(long patNum, string startingText="");
		public static SendTextDelegate SendTextMessage;

		///<summary></summary>
		public static void LockODForMountAcquire(bool isEnabled){
			EventLockODForMountAcquire?.Invoke(null,isEnabled);
		}

		#region ModuleSelected
		///<summary>Goes directly to an Account.  Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void GotoAccount(long patNum) {
			ModuleEventArgs e=new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Account,0,patNum,0);
			EventModuleSelected?.Invoke(null,e);
		}
		
		///<summary>Goes directly to an existing appointment.</summary>
		public static void GotoAppointment(DateTime dateSelected,long selectedAptNum) {
			ModuleEventArgs e=new ModuleEventArgs(dateSelected,new List<long>(),selectedAptNum,0,0,0,0);
			EventModuleSelected?.Invoke(null,e);
		}

		public static void GotoChart(long patNum){
			ModuleEventArgs e=new ModuleEventArgs(DateTime.MinValue, new List<long>(), 0,EnumModuleType.Chart, 0, patNum, 0);
			EventModuleSelected?.Invoke(null,e);
		}

		///<summary>Goes directly to a claim in someone's Account.</summary>
		public static void GotoClaim(long claimNum) {
			ModuleEventArgs e=new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Account,claimNum,0,0);
			EventModuleSelected?.Invoke(null,e);
		}
		
		///<summary>Goes directly to Family module.  Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void GotoFamily(long patNum) {
			ModuleEventArgs e=new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Family,0,patNum,0);
			EventModuleSelected?.Invoke(null,e);
		}

		///<summary>Puts appointments on pinboard, then jumps to Appointments module with today's date.
		///Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void PinToAppt(List<long> pinAptNums,long patNum) {
			ModuleEventArgs e=new ModuleEventArgs(DateTime.Today,pinAptNums,0,EnumModuleType.Appointments,0,patNum,0);
			EventModuleSelected?.Invoke(null,e);
		}

		/// <summary>Puts appointments on the pinboard and jumps to the Appointment module with a specified date.</summary>
		public static void PinAndGoToAppt(List<long> listAptNums,long patNum,DateTime dateTimeDue) {
			ModuleEventArgs e=new ModuleEventArgs(dateTimeDue,listAptNums,0,EnumModuleType.Appointments,0,patNum,0,doShowSearch:true);
			EventModuleSelected?.Invoke(null,e);
		}
		#endregion ModuleSelected

		///<summary>Happens when any of the modules changes the current patient or when this main form changes the patient.  The calling module should refresh itself.  The current patNum is stored  in the parent form so that when switching modules, the parent form knows which patient to call up for that module. If the call to this is followed by ModuleSelected or GotoModule, set isRefreshCurModule=false to prevent the module from being refreshed twice.  If the current module is ContrAppt and the call to this is preceded by a call to RefreshModuleDataPatient, set isApptRefreshDataPat=false so the query to get the patient does not run twice.</summary>
		public static void PatientSelected(Patient patient,bool isRefreshCurModule,bool isApptRefreshDataPat=true,bool hasForcedRefresh=false) {
			PatientSelectedEventArgs e=new PatientSelectedEventArgs();
			e.Patient_=patient;
			e.IsRefreshCurModule=isRefreshCurModule;
			e.IsApptRefreshDataPat=isApptRefreshDataPat;
			e.HasForcedRefresh=hasForcedRefresh;
			EventPatientSelected?.Invoke(null,e);
		}

		public static void RefreshCurrentModule(bool isClinicRefresh) {
			EventRefreshCurrentModule?.Invoke(null,isClinicRefresh);
		}

	}

	public class PatientSelectedEventArgs{
		public Patient Patient_;
		public bool IsRefreshCurModule;
		public bool IsApptRefreshDataPat=true;
		public bool HasForcedRefresh;
	}

	///<summary></summary>
	public class ModuleEventArgs  {
		///<summary>If going to the ApptModule, this lets you pick a date.</summary>
		public DateTime DateSelected;
		///<summary>The aptNums of the appointments that we want to put on the pinboard of the Apt Module.</summary>
		public List<long> ListPinApptNums;
		public long SelectedAptNum;
		public EnumModuleType ModuleType;
		///<summary>If going to Account module, this lets you pick a claim.</summary>
		public long ClaimNum;
		public long PatNum;
		public long DocNum;//image
		public bool DoShowSearch;

		///<summary></summary>
		public ModuleEventArgs(DateTime dateSelected,List<long> listPinApptNums,long selectedAptNum,EnumModuleType moduleType,
			long claimNum,long patNum,long docNum,bool doShowSearch=false)
		{
			DateSelected=dateSelected;
			ListPinApptNums=listPinApptNums;
			SelectedAptNum=selectedAptNum;
			ModuleType=moduleType;
			ClaimNum=claimNum;
			PatNum=patNum;
			DocNum=docNum;
			DoShowSearch=doShowSearch;
		}
	}

	/// <summary>There is no relationship between the underlying enum values and the idx of each module.  These numbers are not stored in the database and may be freely changed with new versions.  Idx numbers, by contrast, might be stored in db sometimes, although I have not yet found an instance.</summary>
	public enum EnumModuleType{
		None,
		Appointments,
		Family,
		Account,
		TreatPlan,
		Chart,
		Imaging,
		//EcwChart and/or TP?,
		Manage
	}
}
