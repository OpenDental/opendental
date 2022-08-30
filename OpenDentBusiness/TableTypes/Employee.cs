using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>An employee at the dental office.</summary>
	[Serializable]
	public class Employee:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmployeeNum;
		///<summary>Employee's last name.</summary>
		public string LName;
		///<summary>First name.</summary>
		public string FName;
		///<summary>Middle initial or name.</summary>
		public string MiddleI; 
		///<summary>If hidden, the employee will not show on the list.</summary>
		public bool IsHidden;
		///<summary>This is just text used to quickly display the clockstatus.  eg Working,Break,Lunch,Home, etc.</summary>
		public string ClockStatus;
		///<summary>The phone extension for the employee.  e.g. 101,102,etc.  This field is only visible for user editing if the pref DockPhonePanelShow is true (1).</summary>
		public int PhoneExt;
		///<summary>Used to store the payroll identification number used to generate payroll reports. ADP uses six digit number between 000051 and 999999.</summary>
		public string PayrollID;
		///<summary></summary>
		public string WirelessPhone;
		///<summary></summary>
		public string EmailWork;
		///<summary></summary>
		public string EmailPersonal;
		///<summary></summary>
		public bool IsFurloughed;
		///<summary></summary>
		public bool IsWorkingHome;
		///<summary>FK to employee.EmployeeNum</summary>
		public long ReportsTo;

		///<summary></summary>
		public Employee Copy() {
			return (Employee)this.MemberwiseClone();
		}

	}
	

	

	
	

}













