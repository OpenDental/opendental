using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>User to specify user level permissions used for CDS interventions.  Unlike normal permissions and security, each permission has its own column and each employee has their own row.</summary>
	[Serializable]
	public class CDSPermission:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CDSPermissionNum;
		///<summary>FK to userod.UserNum.  </summary>
		public long UserNum;
		///<summary>True if allowed to edit EHR Triggers.</summary>
		public bool SetupCDS;
		///<summary>True if user should see EHR triggers that are enabled.  If false, no CDS interventions will show.</summary>
		public bool ShowCDS;
		///<summary>True if user can see Infobutton.</summary>
		public bool ShowInfobutton;
		///<summary>True if user can edit to bibliographic information.</summary>
		public bool EditBibliography;
		///<summary>True to enable Problem based CDS interventions for this user.</summary>
		public bool ProblemCDS;
		///<summary>True to enable Medication based CDS interventions for this user.</summary>
		public bool MedicationCDS;
		///<summary>True to enable Allergy based CDS interventions for this user.</summary>
		public bool AllergyCDS;
		///<summary>True to enable Demographic based CDS interventions for this user.</summary>
		public bool DemographicCDS;
		///<summary>True to enable Lab Test based CDS interventions for this user.</summary>
		public bool LabTestCDS;
		///<summary>True to enable Vital Sign based CDS interventions for this user.</summary>
		public bool VitalCDS;

		///<summary></summary>
		public CDSPermission Copy() {
			return (CDSPermission)this.MemberwiseClone();
		}

	}


}