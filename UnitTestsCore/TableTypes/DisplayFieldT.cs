using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsWeb {
	public class DisplayFieldT {
		public static void ClearTable() {
			string command=$"DELETE FROM DisplayField WHERE DisplayFieldNum > 0";
			DataCore.NonQ(command);
		}

		public static DisplayField CreateDisplayField(string internalName,int width,DisplayFieldCategory category) {
			DisplayField displayField=new DisplayField(internalName,width,category);
			long displayFieldNum=DisplayFields.Insert(displayField);
			displayField.DisplayFieldNum=displayFieldNum;
			DisplayFields.Update(displayField);
			return displayField;
		}

		public static void AddDefaultChartPatInfoFields(bool addAllExtras=false) {
			DisplayFieldCategory category=DisplayFieldCategory.ChartPatientInformation;
			CreateDisplayField("Age",0,category);
			CreateDisplayField("Preferred Pronoun",0,category);
			CreateDisplayField("ABC0",0,category);
			CreateDisplayField("Billing Type",0,category);
			CreateDisplayField("Referred From",0,category);
			CreateDisplayField("Date First Visit",0,category);
			CreateDisplayField("Prov. (Pri, Sec)",0,category);
			CreateDisplayField("Pri Ins",0,category);
			CreateDisplayField("Sec Ins",0,category);
			CreateDisplayField("Payor Types",0,category);
			CreateDisplayField("Premedicate",0,category);
			CreateDisplayField("Problems",0,category);
			CreateDisplayField("Med Urgent",0,category);
			CreateDisplayField("Medical Summary",0,category);
			CreateDisplayField("Service Notes",0,category);
			CreateDisplayField("Medications",0,category);
			CreateDisplayField("Allergies",0,category);
			CreateDisplayField("Pat Restrictions",0,category);
			if(addAllExtras) {
				CreateDisplayField("PatFields",0,category);
				CreateDisplayField("Birthdate",0,category);
				CreateDisplayField("City",0,category);
				CreateDisplayField("AskToArriveEarly",0,category);
				CreateDisplayField("Super Head",0,category);
				CreateDisplayField("Patient Portal",0,category);
				CreateDisplayField("Broken Appts",0,category);
				CreateDisplayField("Tobacco Use",0,category);
				CreateDisplayField("Specialty",0,category);
			}
		}
	}
}
