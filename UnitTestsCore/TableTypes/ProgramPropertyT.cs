using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ProgramPropertyT {

		///<summary>Generates a single Unit Test ProgramProperty. This method refreshes the cache.</summary>
		public static ProgramProperty CreateProgramProperty(long programNum,string propertyDesc,long clinicNum,string propertyValue="",
			string computerName="")
		{
			ProgramProperty prop=ProgramProperties.GetForProgram(programNum).FirstOrDefault(x => x.PropertyDesc==propertyDesc && x.ClinicNum==clinicNum);
			if(prop==null) {
				prop=new ProgramProperty();
				prop.ProgramNum=programNum;
				prop.PropertyDesc=propertyDesc;
				prop.PropertyValue=propertyValue;
				prop.ComputerName=computerName;
				prop.ClinicNum=clinicNum;
				ProgramProperties.Insert(prop);
			}
			else {
				prop.PropertyValue=propertyValue;
				prop.ComputerName=computerName;
				ProgramProperties.Update(prop);
			}
			ProgramProperties.RefreshCache();
			return prop;
		}

		///<summary>Deletes everything from the Unit Test ProgramProperty table.</summary>
		public static void ClearProgamPropertyTable() {
			string command="DELETE FROM programproperty WHERE ProgramPropertyNum > 0";
			DataCore.NonQ(command);
			ProgramProperties.RefreshCache();
		}

		///<summary>Deletes CareCredit properties only.</summary>
		public static void ClearCareCreditProgamProperties() {
			List<string> listProgPropToDelete=typeof(ProgramProperties.PropertyDescs.CareCredit)
				.GetFields(BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly)
				.Select(x => x.GetValue(null).ToString())
				.ToList();
			string command=$"DELETE FROM programproperty "+
				$"WHERE PropertyDesc IN({string.Join(",",listProgPropToDelete.Select(x => "'"+x+"'"))})";
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
					PropertyDesc=propertyDesc,
					ClinicNum=clinicNum
				};
				ProgramProperties.Insert(progProp);
			}
			progProp.PropertyValue=propertyValue;
			ProgramProperties.Update(progProp);
			ProgramProperties.RefreshCache();
		}
	}
}
