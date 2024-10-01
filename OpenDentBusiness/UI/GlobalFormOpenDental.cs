using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>These global methods can be called from anywhere. FormOpenDental subscribes to the resulting events so that it can do things like refresh a module or select a patient. Even some classes in OpenDentBusiness take advantage of this, although those sections really don't belong in the business layer if they are telling FormOpenDental what to do. These will replace the public static methods in FormOpenDental that use the singleton pattern. The main reason for switching to this event pattern is that the WPF windows don't have access to FormOpenDental. Also see CodeBased.ODEvent which is being deprecated. Also, see DataValid.EventInvalid, which handles all Signalod events.</summary>
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
		///<summary>Subscribe to this even from any WPF Frm in order to be notified of any incoming signal. See example in FrmODBase discussion at top.</summary>
		public static event EventHandler<List<Signalod>> EventProcessSignalODs;

		///<summary></summary>
		public static void ProcessSignalODs(List<Signalod> listSignalods){
			EventProcessSignalODs?.Invoke(null,listSignalods);
		}

		///<summary></summary>
		public static void LockODForMountAcquire(bool isEnabled){
			EventLockODForMountAcquire?.Invoke(null,isEnabled);
		}

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
