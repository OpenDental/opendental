using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness.HL7 {
	///<summary>A component in HL7 is a subportion of a field.  For example, a name field might have LName and FName components.  Components are 0-based.</summary>
	public class ComponentHL7 {
		public string ComponentVal;
		

		public ComponentHL7(string componentVal,char escCh='\\') {
			if(string.IsNullOrWhiteSpace(componentVal)) {
				ComponentVal="";
				return;
			}
			string e=escCh.ToString();
			string e2=e;
			if(escCh=='\\') {
				//replace \.br\ with &92;.br&92; (ASCII for backslash is 92) so regex won't remove the backslashes
				componentVal=componentVal.Replace(e+".br"+e,"&92;.br&92;");
				e2+=e;
			}
			//remove escape characters, treating escaped character as a literal string and not a delimiter
			ComponentVal=Regex.Replace(componentVal,e2+"(.)?","$1",RegexOptions.Singleline);//if escape char is default ('\')
			if(escCh=='\\') {
				//replace &92;.br&92; with \.br\, return to normal escape sequence for new lines so new lines can be inserted later on in processing
				ComponentVal=ComponentVal.Replace("&92;.br&92;",e+".br"+e);
			}
		}

		public override string ToString() {
			return ComponentVal;
		}
	}
}
