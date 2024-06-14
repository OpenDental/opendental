using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class InsVerifyT {

		///<summary>Deletes everything from the insverify table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearInsVerifyTable() {
			string command="DELETE FROM insverify";
			DataCore.NonQ(command);
		}

		///<summary>Updates the SecDateTEdit of a InsVerify to the number of daysPrevious. Unable to set the SecDateTEdit to anything but DateTime.Now in the CRUD Layer.</summary>
		public static void SetSecDateTEditToDaysPast(InsVerify insVerify,int daysPrevious) {
			string command="UPDATE insverify SET SecDateTEdit = "+ POut.Date(DateTime.Now.AddDays(-daysPrevious))
				+" WHERE InsVerifyNum = "+ POut.Long(insVerify.InsVerifyNum);
			DataCore.NonQ(command);
    }

		///<summary>Creates and returns a list of 4 InsVerifies. Useful for testing an insverify search with different fields for the API.</summary>
		public static List<InsVerify> CreateMultipleInsVerifies() {
			List<InsVerify> listOdbInsVerifies=new List<InsVerify>();
			//Create an InsuranceVerificationStatus def.
			Def odbDef=DefT.CreateDefinition(DefCat.InsuranceVerificationStatus,"InsVerificationStatus");
			#region InsVerify1
			//InsVerify1 - InsuranceBenefit - SecDateTEdit 4 days previous
			InsPlan odbInsPlan=InsPlanT.CreateInsPlan(CarrierT.CreateCarrier("HOCUS").CarrierNum);
			InsVerify odbInsVerify=InsVerifies.GetOneByFKey(odbInsPlan.PlanNum,VerifyTypes.InsuranceBenefit);
			odbInsVerify.DateLastVerified=DateTime.Today.AddDays(-4);
			InsVerifies.Update(odbInsVerify);
			SetSecDateTEditToDaysPast(odbInsVerify,4);
			odbInsVerify=InsVerifies.GetOneByFKey(odbInsPlan.PlanNum,VerifyTypes.InsuranceBenefit);
			listOdbInsVerifies.Add(odbInsVerify);
			#endregion
			#region InsVerify2
			//InsVerify2 - InsuranceBenefit - SecDateTEdit 6 days previous
			InsPlan odbInsPlan2=InsPlanT.CreateInsPlan(CarrierT.CreateCarrier("POCUS").CarrierNum);
			InsVerify odbInsVerify2=InsVerifies.GetOneByFKey(odbInsPlan2.PlanNum,VerifyTypes.InsuranceBenefit);
			odbInsVerify2.DefNum=odbDef.DefNum;
			odbInsVerify2.Note="Need more info";
			odbInsVerify2.DateLastAssigned=DateTime.Today.AddDays(-6);
			InsVerifies.Update(odbInsVerify2);
			SetSecDateTEditToDaysPast(odbInsVerify2,6);
			odbInsVerify2=InsVerifies.GetOneByFKey(odbInsPlan2.PlanNum,VerifyTypes.InsuranceBenefit);
			listOdbInsVerifies.Add(odbInsVerify2);
			#endregion
			#region InsVerify3
			Carrier odbCarrier=CarrierT.CreateCarrier("CAR");
			InsPlan odbInsPlan3=InsPlanT.CreateInsPlan(odbCarrier.CarrierNum);
			//InsVerify3 - InsuranceBenefit - SecDateTEdit 0 days previous
			InsVerify odbInsVerify3=InsVerifies.GetOneByFKey(odbInsPlan3.PlanNum,VerifyTypes.InsuranceBenefit);
			listOdbInsVerifies.Add(odbInsVerify3);
			#endregion
			#region InsVerify4
			//InsVerify4 - PatientEnrollment - SecDateTEdit 8 days previous
			Patient odbPatient=PatientT.CreatePatient();
			InsSub odbInsSub=InsSubT.CreateInsSub(87765676,odbInsPlan3.PlanNum);
			PatPlan odbPatPlan=PatPlanT.CreatePatPlan(1,odbPatient.PatNum,odbInsSub.InsSubNum);
			InsVerify odbInsVerify4=InsVerifies.GetOneByFKey(odbPatPlan.PatPlanNum,VerifyTypes.PatientEnrollment);
			odbInsVerify4.DefNum=odbDef.DefNum;
			odbInsVerify4.Note="Need more info";
			odbInsVerify4.DateLastAssigned=DateTime.Today.AddDays(-8);
			odbInsVerify4.DateLastVerified=DateTime.Today.AddYears(-2);
			InsVerifies.Update(odbInsVerify4);
			SetSecDateTEditToDaysPast(odbInsVerify4,8);
			odbInsVerify4=InsVerifies.GetOneByFKey(odbPatPlan.PatPlanNum,VerifyTypes.PatientEnrollment);
			listOdbInsVerifies.Add(odbInsVerify4);
			#endregion
			return listOdbInsVerifies.OrderBy(x => x.InsVerifyNum).ToList();
		}

	}
}
