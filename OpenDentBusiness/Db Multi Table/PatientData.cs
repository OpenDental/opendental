using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {//Jordan is the only one allowed to edit this file.
	///<summary>This is a class to store and organize all data for one patient. It's shared between all modules.</summary>
	[Serializable]
	public partial class PatientData {
		//There is one instance of this object at the level of FormOpenDental. References are passed to each module. This object is never reinitialized.
		//Data is grabbed as Lists or single rows.
		//We might eventually deprecate the strategy of grabbing DataTables, but the transition would take a long time.
		//In that case, we would instead process lists into datatables at the client level using methods in OpenDentBusiness.
		//This would give us access to formalized data from lists rather than just human-readable data in tables for grids.
		//In some cases, this would be tricky because the Lists would need to include rows from other patients.
		//Example is when a PaySplit for a child shows in the account for the guarantor.
		//Lists all start out null.
		//Lists include all rows for the patient without any date or visibility filters.
		//Lists include all columns.
		//All Lists do clear when clicking module buttons, but this may change in the distant future.
		//When new data is needed, only the Lists currently needed get retrieved. Many lists remain null because we don't necessarily need them.
		//Never touch this object or any Lists from another thread. Multithreading is neither needed nor supported.
		//You can pass a reference of this object to any popup window to have access to all the patient's data. This includes multiple layers of windows.
		//This also doubles as the way to see which patient is currently selected.
		//It might seem a little odd to have separation between Clear and FillIfNeeded, but allows for some powerful scaling.
		//You can just generally use them as a pair: Clear, then FillIfNeeded. Or ClearAndFill().

		///<summary>Family is deprecated, so use List Patients wherever possible. This is just here to smooth the transition.</summary>
		public Family Family;
		public long PatNum;
		public Patient Patient;

		///<summary>This also clears out the lists.</summary>
		public void SetPatNum(long patNum){
			ClearAll();
			PatNum=patNum;
		}

		///<summary>When changing patients or clicking on a module button, this clears all the lists. This does not clear the PatNum. Typically follow with setting PatNum and FillIfNeeded.</summary>
		public void ClearAll(){
			EnumPdTable[] patDataTypeArray=(EnumPdTable[])Enum.GetValues(typeof(EnumPdTable));
			Clear(patDataTypeArray);
			Family=null;
			Patient=null;
			PatientNote=null;
		}

		///<summary>Equivalent to Clear followed by FillIfNeeded. This is frequently used when you only need to refresh one or two lists in PatientData. It's also perfectly acceptable to directly call the db like this: Pd.ListAppointments=Appointments.GetPatientData(patNum).</summary>
		public void ClearAndFill(params EnumPdTable[] patDataTypes){
			Clear(patDataTypes);
			FillIfNeeded(patDataTypes);
		}
	}

	///<summary>This enum is not used in the database, so it's safe to rename and reorganize as needed. These should be alphabetical where used throughout program.</summary>
	public enum EnumPdTable{
		//The first section is out of alphabetical order because of dependencies. There are plans to fix this issue.
		///<summary>This is a list of patients in the family. It also fills the Patient and Family objects. It's listed first to minimize dependencies messing up the alphabetizing.</summary>
		Patient,
		[PdMeta(Dependencies ="Patient")]
		InsSub,
		[PdMeta(Dependencies ="InsSub")]
		InsPlan,
		PatPlan,
		[PdMeta(Dependencies ="PatPlan,InsSub")]
		Benefit,
		ClaimProc,
		[PdMeta(Dependencies ="Benefit,PatPlan,InsPlan,InsSub")]
		//Beginning of normal alphabetical section-----------------
		Adjustment,
		[PdMeta(Plural="Allergies")]
		Allergy,
		Appointment,
		ClaimProcHist,
		Disease,
		Document,
		///<summary>This only gets TobaccoUseAssessed events.</summary>
		EhrMeasureEvent,
		MedicationPat,
		Mount,
		///<summary>Includes both OrthoChart and OrthoChartRow.</summary>
		OrthoChart,
		OrthoHardware,
		PatField,
		[PdMeta(FieldType=EnumPdFieldType.SingleObject,Dependencies ="Patient")]
		PatientNote,
		[PdMeta(FieldType=EnumPdFieldType.SingleObject,Dependencies ="Patient")]
		PatientSuperFamHead,
		PatRestriction,
		PayorType,
		PaySplit,
		Procedure,
		ProcGroupItem,
		ProcMultiVisit,
		[PdMeta(Plural="RefAttaches")]
		RefAttach,
		[PdMeta(FieldType=EnumPdFieldType.Table,ComplexGetter=EnumPdComplexGetter.ChartProgressNotes)]
		TableProgNotes,
		[PdMeta(FieldType=EnumPdFieldType.Table,Dependencies="TableProgNotes")]
		TablePlannedAppts,
		ToothInitial,
		[PdMeta(FieldType=EnumPdFieldType.SingleObject)]
		UserWebHasPortalAccess
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class PdMeta : Attribute {
		public EnumPdFieldType FieldType{get; set; }
		public EnumPdComplexGetter ComplexGetter{get;set; }
		///<summary>Comma delimited list of EnumPdTable types that this depends on.</summary>
		public string Dependencies{get;set; }
		public string Plural;
	}

	public enum EnumPdFieldType{
		List,
		SingleObject,
		Table
	}

	public enum EnumPdComplexGetter{
		None,
		ChartProgressNotes
	}
}


//Ignore these notes:
//Concurrency Issues:
//One set of problems that we're trying to eventually solve is concurrency issues.
//If someone at a different workstation changes data, how fast can we get it to show on your screen?
//Currently, you can ensure fresh data by clicking a module button.
//But you could also have data that is 20 minutes stale sitting on your screen.
//This is not generally a problem because you click to dive into any data editing, which is another chance to refresh.
//You also refresh when you click around between modules, but that can waste a lot of db calls.
//Possible improvement 1:
//We could add a timer so that data does not get refreshed by clicking between modules unless it's maybe 10 seconds stale.
//This improvement could be added at any time after the foundation is in place.
//Possible improvement 2:
//Add another table with a row that gets inserted any time anyone makes a change in any table for that patient.
//This new table would have rows that are short lived, maybe something like 30 minutes.
//This could, in theory, save a lot of database calls because you could use it instead of a timer.
//Both of the above possible improvements could not be done until our overhaul is complete.
//The overhaul won't make things worse, but we also shouldn't to try to solve any concurrency issues with the initial overhaul.
//Nobody is really complaining about concurrency or too many queries when loading modules,
//so our goal with the overhaul is to duplicate existing behavior, not improve it.