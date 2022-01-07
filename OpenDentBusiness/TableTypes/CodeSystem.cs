using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Used for tracking code systems imported to OD. HL7OID used for sending messages.  This must be a database table in order to keep track of VersionCur between sessions.</summary>
	[Serializable()]
	public class CodeSystem:TableBase{
		///<summary>Primary key. Not currently referenced anywhere.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CodeSystemNum;
		///<summary>.</summary>
		public string CodeSystemName;
		///<summary>Only used for display, not actually interpreted. Updated by Code System importer.  Examples: 2013 or 1</summary>
		public string VersionCur;
		///<summary>Only used for display, not actually interpreted. Updated by Convert DB script.</summary>
		public string VersionAvail;
		///<summary>Example: 2.16.840.1.113883.6.13</summary>
		public string HL7OID;
		///<summary>Notes to display to user. Examples: "CDT codes distributed via program updates.", "CPT codes require purchase and download from www.ama.com</summary>
		public string Note;

		///<summary></summary>
		public CodeSystem Clone() {
			return (CodeSystem)this.MemberwiseClone();
		}

		///<summary></summary>
		public CodeSystem() {
			
		}

		///<summary>Used to generate version specific list.</summary>
		public CodeSystem(string name) {
			CodeSystemName=name;
		}

	}

	
}




