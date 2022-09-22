using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace OpenDental.Reporting.Allocators {
	/// <summary>
	/// <seealso cref="IAllocator"/>
	/// Here is the model.
	///		Payments arrive.  When they arrive an allocation event is to occur. The payment
	///		can then be allocated by the set of rules given by the allocator which a programmer
	///		will create.  The Allocator created must conform to the method rules called by the
	///		allocator interface which is really simple.
	/// 
	/// The Allocators in the AllocatorCollection will be called.  The allocators must be created by
	/// a programmer and added to the static list that is creted in the private method CreateAllocators.
	/// </summary>
	public class AllocatorCollection {
		private static List<Allocator> _AllocatorList = null;

		/// <summary>
		/// jsparks-NOT GETTING CALLED.
		/// The only place that it was being used was in ProcedureL.SetCompleteInAppt().  But once that method was moved into the business layer,
		/// it was no longer possible to make a call into OpenDental UI layer.
		/// So for this to work, it will all need to be moved into the business layer.
		/// Calls all of the allocators created for this patient.
		/// Determines the guarantor form this patient first
		/// 
		/// 
		/// Points of Entry Identified in OD
		///		1)  ContrAccount.ToolBarMain_ButtonClick(...)  Added code to run allocator after user is finished with clicked tasks
		///		2)	ContrAccount.gridAccount_CellDoubleClick(...)  Double Click means that an edit was potentialy occuring run allocator.
		///		3)  ContrAccount.gridRepeat_CellDoubleClick(...)  I am not familiar with Payment Plans or Repeating Charges need to check against tool
		///		4)  ContrAccount.gridPayPlan_CellDoubleClick(...) I am not familiar with Payment Plans or Repeating Charges need to check against tool
		///		5)  ContrChart.gridProg_CellDoubleClick(...)  Indicates Procedure was potentially changed. Runs allocator if any of the dialogs returned DialogResult.OK
		///		6)	ContrChart.EnterTypedCode(...) 
		///		7)  ContrChart.ProcButtonClicked(...)
		///		8)  Procedures.SetCompleteInAppt(...) // Called from Set Complete Checkmark (butComplete) in the appointment module.
		///		
		/// Points of Entry That Need Atttention to
		/// </summary>
		public static void CallAll_Allocators(int iPatient) {
			// Find Guarantor first
			int iGuarantor = GetGuarantor(iPatient);
			if(_AllocatorList == null) {
				CreateAllocators();// <--- Add your allocator to this list.  
			}
			if(iGuarantor != 0) // cannot allocate for no patient
				foreach(Allocator alc in _AllocatorList) //<--- if your allocator is not based on guarantor then you will need to make sure it is not part of this list that is called but make your own.
					alc.Allocate(iGuarantor);
		}

		public static void CallAll_UnAllocators(int iPatient) {
			// Find Guarantor first
			int iGuarantor = GetGuarantor(iPatient);
			if(_AllocatorList == null) {
				CreateAllocators();
			}
			if(iGuarantor != 0) // cannot allocate for no patient
				foreach(Allocator alc in _AllocatorList)
					alc.DeAllocate(iGuarantor);
		}
		/// <summary>
		/// Only calle from with in AllocatorCollection so 
		/// don't have to worry about opendental calling this.
		/// </summary>
		private static void CreateAllocators() {
			_AllocatorList = new List<Allocator>();
			_AllocatorList.Add(new OpenDental.Reporting.Allocators.MyAllocator1_ProviderPayment());
		}

		private static int GetGuarantor(int Patient) {
			int rVal = 0;
			try {
				rVal = int.Parse(Db.GetTableOld("SELECT guarantor FROM patient WHERE patnum= " + Patient.ToString()).Rows[0][0].ToString());
			}
			catch { }
			return rVal;
		}
	}


}
