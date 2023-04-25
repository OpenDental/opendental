using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This table holds rows for linking a definition object to another object.  Allows for a many-to-many relationship between definitions and other object types.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true,HasBatchWriteMethods=true)]
	public class DefLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DefLinkNum;
		///<summary>FK to definition.DefNum. The definition that is linked to </summary>
		public long DefNum;
		///<summary>A foreign key to a table associated with the DefLinkType. Uses include:  ClinicNum with DefLinkType ClinicSpecialty, PatNum with DefLinkType Patient.</summary>
		public long FKey;
		///<summary>Enum:DefLinkType The type of link.</summary>
		public DefLinkType LinkType;

		///<summary></summary>
		public DefLink Copy() {
			return (DefLink)this.MemberwiseClone();
		}
	}

	///<summary>The manner in which a definition is linked.</summary>
	public enum DefLinkType {
		///<summary>0. Specialties for a clinic.</summary>
		ClinicSpecialty,
		///<summary>1. One definition of Category DefCat.ClinicSpecialty is linked to one patient.PatNum. This is how specialties are assigned to patient clones. 2022-05-14-Jordan: I see no reason for this convoluted linkage. Maybe someone was trying to avoid the simpler solution of adding a Specialty column to the patient table?</summary>
		Patient,
		///<summary>2. One definition can be linked to multiple appointment types. See definition.Category: DefCat.WebSchedNewPatApptTypes and WebSchedExistingApptTypes</summary>
		AppointmentType,
		///<summary>3. One definition can be linked to multiple operatories where definition.Category=(DefCat.WebSchedNewPatApptTypes(42) or WebSchedExistingApptTypes(51)), deflink.DefNum=definition.DefNum,  deflink.LinkType=3, and deflink.FKey=operatory.OperatoryNum.</summary>
		Operatory,
		///<summary>4. The definition is linked to another definition that is in the BlockoutType category.  Used by WebSched for restricting available time slots.</summary>
		BlockoutType,
		///<summary>5. The definition is linked to a recall type.  Used by WebSched for identifying available time slots for recalls.</summary>
		RecallType,
	}

}
