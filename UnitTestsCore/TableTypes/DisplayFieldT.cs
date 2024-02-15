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

		public static DisplayField CreateDisplayField(string internalName,int width,DisplayFieldCategory category,int itemOrder) {
			DisplayField displayField=new DisplayField(internalName,width,category);
			displayField.ItemOrder=itemOrder;
			long displayFieldNum=DisplayFields.Insert(displayField);
			displayField.DisplayFieldNum=displayFieldNum;
			DisplayFields.Update(displayField);
			return displayField;
		}

		public static void AddDefaultChartPatInfoFields(bool doAddAllExtras=false) {
			DisplayFieldCategory category=DisplayFieldCategory.ChartPatientInformation;
			CreateDisplayField("Age",0,category,0);
			CreateDisplayField("Preferred Pronoun",0,category,1);
			CreateDisplayField("ABC0",0,category,2);
			CreateDisplayField("Billing Type",0,category,3);
			CreateDisplayField("Referred From",0,category,4);
			CreateDisplayField("Date First Visit",0,category,5);
			CreateDisplayField("Prov. (Pri, Sec)",0,category,6);
			CreateDisplayField("Pri Ins",0,category,7);
			CreateDisplayField("Sec Ins",0,category,8);
			CreateDisplayField("Payor Types",0,category,9);
			CreateDisplayField("Premedicate",0,category,10);
			CreateDisplayField("Problems",0,category,11);
			CreateDisplayField("Med Urgent",0,category,12);
			CreateDisplayField("Medical Summary",0,category,13);
			CreateDisplayField("Service Notes",0,category,14);
			CreateDisplayField("Medications",0,category,15);
			CreateDisplayField("Allergies",0,category,16);
			CreateDisplayField("Pat Restrictions",0,category,17);
			if(doAddAllExtras) {
				CreateDisplayField("PatFields",0,category,18);
				CreateDisplayField("Birthdate",0,category,19);
				CreateDisplayField("City",0,category,20);
				CreateDisplayField("AskToArriveEarly",0,category,21);
				CreateDisplayField("Super Head",0,category,22);
				CreateDisplayField("Patient Portal",0,category,23);
				CreateDisplayField("Broken Appts",0,category,24);
				CreateDisplayField("Tobacco Use",0,category,25);
				CreateDisplayField("Specialty",0,category,26);
			}
		}
	}
}
