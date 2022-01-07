using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used in dental schools.  eg OP 732 Operative Dentistry Clinic II.</summary>
	[Serializable]
	public class SchoolCourse:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SchoolCourseNum;
		///<summary>Alphanumeric.  eg PEDO 732.</summary>
		public string CourseID;
		///<summary>eg: Pediatric Dentistry Clinic II</summary>
		public string Descript;
		
		///<summary></summary>
		public SchoolCourse Copy(){
			SchoolCourse sc=new SchoolCourse();
			sc.SchoolCourseNum=SchoolCourseNum;
			sc.CourseID=CourseID;
			sc.Descript=Descript;
			return sc;
		}

	}

	

	


}




















