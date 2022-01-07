using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>A certification completed by an employee on a specific date.</summary>
	[Serializable()]
	public class CertEmployee:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CertEmployeeNum;
		///<summary>FK to cert.CertNum.</summary>
		public long CertNum;
		///<summary>FK to employee.EmployeeNum.</summary>
		public long EmployeeNum;
		///<summary></summary>
		public DateTime DateCompleted;
		///<summary>Rarely, a very short note is required.</summary>
		public string Note;
		///<summary>FK to userod.UserNum. The user who made this entry.  Usually some sort of supervisor.</summary>
		public long UserNum;

		///<summary></summary>
		public CertEmployee Copy() {
			return (CertEmployee)this.MemberwiseClone();
		}


	}
}




