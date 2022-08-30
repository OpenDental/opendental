using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class HieClinicT {
		public static void ClearHieClinicTable() {
			string command="DELETE FROM hieclinic WHERE HieClinicNum > 0";
			DataCore.NonQ(command);
		}

		public static HieClinic CreateAndInsert(TimeSpan timeToExport,long clinicNum=0,HieCarrierFlags flag=HieCarrierFlags.AllCarriers,bool isEnabled=false)
		{
			HieClinic hieClinic=new HieClinic();
			hieClinic.ClinicNum=clinicNum;
			hieClinic.SupportedCarrierFlags=flag;
			hieClinic.PathExportCCD="";
			if(isEnabled) {
				hieClinic.PathExportCCD=@"C:\Temp\";
			}
			hieClinic.IsEnabled=isEnabled;
			hieClinic.TimeOfDayExportCCD=timeToExport;
			HieClinics.Insert(hieClinic);
			return hieClinic;
		}
	}
}
