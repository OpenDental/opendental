using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PerioExamT {

		public static PerioExam CreatePerioExam(long patNum,DateTime dateExam,long provNum=0,string note="")
		{
			PerioExam perioExam=new PerioExam();
			perioExam.PatNum=patNum;
			perioExam.ExamDate=dateExam;
			perioExam.ProvNum=provNum;
			perioExam.Note=note;
			perioExam.PerioExamNum=PerioExams.Insert(perioExam);
			return perioExam;
		}

		///<summary>Deletes everything from the perioexam and periomeasure tables. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPerioTables() {
			string command="DELETE FROM perioexam WHERE PerioExamNum > 0";
			DataCore.NonQ(command);
			command="DELETE FROM periomeasure WHERE PerioMeasureNum > 0";
			DataCore.NonQ(command);
		}

	}
}
