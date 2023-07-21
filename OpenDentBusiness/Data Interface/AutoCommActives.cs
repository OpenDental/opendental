using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class AutoCommActives {
		///<summary></summary>
		public static bool IsForEmail(CommType commType) {
			return EnumTools.GetAttributeOrDefault<CommTypeAttribute>(commType).ContactMethod==ContactMethod.Email;
		}

		public static bool IsForEmail(string commTypeStr) {
			if(!Enum.TryParse(commTypeStr,out CommType commType)) {
				return false;
			}
			return IsForEmail(commType);
		}
	}
}
