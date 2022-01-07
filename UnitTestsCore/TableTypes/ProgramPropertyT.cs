using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ProgramPropertyT {

		///<summary>Generates a single Unit Test ProgramProperty. This method refreshes the cache.</summary>
		public static ProgramProperty CreateProgramProperty(long programNum,string propertyDesc,long clinicNum,string propertyValue="",
			string computerName="")
		{
			ProgramProperty prop=new ProgramProperty();
			prop.ProgramNum=programNum;
			prop.PropertyDesc=propertyDesc;
			prop.PropertyValue=propertyValue;
			prop.ComputerName=computerName;
			prop.ClinicNum=clinicNum;
			ProgramProperties.Insert(prop);
			ProgramProperties.RefreshCache();
			return prop;
		}

		///<summary>Deletes everything from the Unit Test ProgramProperty table.</summary>
		public static void ClearProgamPropertyTable() {
			string command="DELETE FROM programproperty WHERE ProgramPropertyNum > 0";
			DataCore.NonQ(command);
			ProgramProperties.RefreshCache();
		}

		public static void UpdateProgramProperty(ProgramName progName,string propertyDesc,string propertyValue,long clinicNum=0) {
			Program prog=Programs.GetProgram(Programs.GetProgramNum(progName));
			ProgramProperty progProp=ProgramProperties.GetFirstOrDefault(x => x.ProgramNum==prog.ProgramNum && x.PropertyDesc==propertyDesc 
				&& x.ClinicNum==clinicNum);
			if(progProp is null) {
				progProp=new ProgramProperty() {
					ProgramNum=prog.ProgramNum,
					PropertyDesc=propertyDesc
				};
				ProgramProperties.Insert(progProp);
				ProgramProperties.RefreshCache();
			}
			progProp.PropertyValue=propertyValue;
			ProgramProperties.Update(progProp);
		}
	}
}
