using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<summary>For Dental Schools.  The purpose of this table changed significantly in version 4.5.  This now only stores completed requirements.  There can be multiple completed requirements of each ReqNeededNum.  No need to synchronize any longer.</summary>
	[Serializable]
	public class ReqStudent:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReqStudentNum;
		///<summary>FK to reqneeded.ReqNeededNum.</summary>
		public long ReqNeededNum;
		///<summary>.</summary>
		public string Descript;
		///<summary>FK to schoolcourse.SchoolCourseNum.  Never 0.</summary>
		public long SchoolCourseNum;
		///<summary>FK to provider.ProvNum.  The student.  Never 0.</summary>
		public long ProvNum;
		///<summary>FK to appointment.AptNum.</summary>
		public long AptNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>FK to provider.ProvNum</summary>
		public long InstructorNum;
		///<summary>The date that the requirement was completed.</summary>
		public DateTime DateCompleted;

		///<summary></summary>
		public ReqStudent Copy(){
			ReqStudent r=new ReqStudent();
			r.ReqStudentNum=ReqStudentNum;
			r.ReqNeededNum=ReqNeededNum;
			r.Descript=Descript;
			r.SchoolCourseNum=SchoolCourseNum;
			r.ProvNum=ProvNum;
			r.AptNum=AptNum;
			r.PatNum=PatNum;
			r.InstructorNum=InstructorNum;
			r.DateCompleted=DateCompleted;
			return r;
		}
	}


}
