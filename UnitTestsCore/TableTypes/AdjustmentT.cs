using OpenDentBusiness;
using System;
using System.Collections.Generic;

namespace UnitTestsCore {
	public class AdjustmentT {

		///<summary>Deletes everything from the adjustment table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearAdjustmentTable() {
			string command="DELETE FROM adjustment WHERE AdjNum > 0";
			DataCore.NonQ(command);
		}

		public static Adjustment MakeAdjustment(long patNum,double adjAmt,DateTime adjDate=default(DateTime),DateTime procDate=default(DateTime)
			,long procNum=0,long provNum=0,long adjType=0,long clinicNum=0,bool doInsert=true) 
		{
			Adjustment adjustment=new Adjustment();
			if(adjDate==default(DateTime)) {
				adjDate=DateTime.Today;
			}
			if(procDate==default(DateTime)) {
				procDate=DateTime.Today;
			}
			adjustment.PatNum=patNum;
			adjustment.AdjAmt=adjAmt;
			adjustment.ProcNum=procNum;
			adjustment.ProvNum=provNum;
			adjustment.AdjDate=adjDate;
			adjustment.ProcDate=procDate;
			adjustment.AdjType=adjType;
			adjustment.ClinicNum=clinicNum;
			if(doInsert) {
				Adjustments.Insert(adjustment);
			}
			return adjustment;
		}

		public static void InsertMany(List<Adjustment> listAdjustments) {
			OpenDentBusiness.Crud.AdjustmentCrud.InsertMany(listAdjustments);
		}
	}
}
