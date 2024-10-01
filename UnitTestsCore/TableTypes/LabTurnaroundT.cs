using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class LabTurnaroundT {

		///<summary>Creates a labturnaround.</summary>
		public static LabTurnaround CreateLabTurnaround(long laboratoryNum,string description,int daysPublished,int daysActual)
		{
			LabTurnaround labturnaround=new LabTurnaround() {
				LaboratoryNum=laboratoryNum,
				Description=description,
				DaysPublished=daysPublished,
				DaysActual=daysActual
			};
			labturnaround.LabTurnaroundNum=LabTurnarounds.Insert(labturnaround);
			return labturnaround;
		}

		///<summary>Deletes everything from the labturnaround table.</summary>
		public static void ClearLabTurnaroundTable() {
			string command="DELETE FROM labturnaround";
			DataCore.NonQ(command);
		}

		///<summary>Creates and returns a list of 5 labturnarounds with all fields utilized and varied, except for LaboratoryNum 1 which is used twice. Useful for testing a labturnaround search.</summary>
		public static List<OpenDentBusiness.LabTurnaround> CreateVariedLabTurnaroundSet() {
			long odbLaboratoryNum1=LaboratoryT.CreateLaboratory().LaboratoryNum;
			long odbLaboratoryNum2=LaboratoryT.CreateLaboratory().LaboratoryNum;
			long odbLaboratoryNum3=LaboratoryT.CreateLaboratory().LaboratoryNum;
			long odbLaboratoryNum4=LaboratoryT.CreateLaboratory().LaboratoryNum;
			List<OpenDentBusiness.LabTurnaround> listLabTurnarounds=new List<OpenDentBusiness.LabTurnaround>();
			listLabTurnarounds.Add(new OpenDentBusiness.LabTurnaround {
				LaboratoryNum=odbLaboratoryNum1,
				Description="Denture Biteblocks",
				DaysPublished=2,
				DaysActual=3
			});
			listLabTurnarounds.Add(new OpenDentBusiness.LabTurnaround {
				LaboratoryNum=odbLaboratoryNum1,
				Description="RPD Framework",
				DaysPublished=5,
				DaysActual=7
			});
			listLabTurnarounds.Add(new OpenDentBusiness.LabTurnaround {
				LaboratoryNum=odbLaboratoryNum2,
				Description="RPD Framework & Setup Teeth",
				DaysPublished=11,
				DaysActual=13
			});
			listLabTurnarounds.Add(new OpenDentBusiness.LabTurnaround {
				LaboratoryNum=odbLaboratoryNum3,
				Description="RPD Setup Teeth",
				DaysPublished=17,
				DaysActual=19
			});
			listLabTurnarounds.Add(new OpenDentBusiness.LabTurnaround {
				LaboratoryNum=odbLaboratoryNum4,
				Description="Denture Reline",
				DaysPublished=23,
				DaysActual=29
			});
			List<OpenDentBusiness.LabTurnaround> listOdbLabTurnaroundsReturned=new List<OpenDentBusiness.LabTurnaround>();
			for(int i=0;i<listLabTurnarounds.Count;i++) {
				long labTurnaroundNum=OpenDentBusiness.LabTurnarounds.Insert(listLabTurnarounds[i]);
				listOdbLabTurnaroundsReturned.Add(OpenDentBusiness.LabTurnarounds.GetOneLabTurnaroundForApi(labTurnaroundNum));
			}
			return listOdbLabTurnaroundsReturned;
		}
	}
}