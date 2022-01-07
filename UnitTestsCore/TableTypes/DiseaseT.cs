using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class DiseaseT {

		///<summary>Inserts the new disease and returns it.</summary>
		public static Disease CreateDisease(long patNum,long diseaseDefNum=0,DateTime dateStart=default(DateTime),DateTime dateStop=default(DateTime),
			string patNote="")
		{
			Disease disease=new Disease();
			disease.DateStart=dateStart;
			disease.DateStop=dateStop;
			disease.DiseaseDefNum=diseaseDefNum;
			if(diseaseDefNum==0) {
				disease.DiseaseDefNum=DiseaseDefT.CreateDiseaseDef().DiseaseDefNum;
			}
			disease.PatNote=patNote;
			disease.PatNum=patNum;
			disease.ProbStatus=ProblemStatus.Active;
			Diseases.Insert(disease);
			return disease;
		}

		public static void ClearDiseaseTable() {
			string command="DELETE FROM disease";
			DataCore.NonQ(command);
		}
	}
}
