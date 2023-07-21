using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PerioMeasureT {

		public static PerioMeasure CreatePerioMeasure(long perioExamNum,PerioSequenceType perioSequenceType,int intTooth,
			int toothValue=-1,int mbValue=-1,int bValue=-1,int dbValue=-1,int mlValue=-1,int lValue=-1,int dlValue=-1)
		{
			PerioMeasure perioMeasure=new PerioMeasure();
			perioMeasure.PerioExamNum=perioExamNum;
			perioMeasure.SequenceType=perioSequenceType;
			perioMeasure.IntTooth=intTooth;
			perioMeasure.ToothValue=toothValue;
			perioMeasure.MBvalue=mbValue;
			perioMeasure.Bvalue=bValue;
			perioMeasure.DBvalue=dbValue;
			perioMeasure.MLvalue=mlValue;
			perioMeasure.Lvalue=lValue;
			perioMeasure.DLvalue=dlValue;
			perioMeasure.PerioMeasureNum=PerioMeasures.Insert(perioMeasure);
			return perioMeasure;
		}

		///<summary>Deletes everything from the periomeasure table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPerioMeasureTable() {
			string command="DELETE FROM periomeasure WHERE PerioMeasureNum > 0";
			DataCore.NonQ(command);
		}

	}
}
