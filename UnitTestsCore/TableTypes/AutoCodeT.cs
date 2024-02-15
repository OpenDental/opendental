using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class AutoCodeT {
		public static AutoCode CreateAutoCode(string desc,bool lessIntrusive=false) {
			AutoCode autoCode=new AutoCode {
				Description=desc,
				LessIntrusive=lessIntrusive,
			};
			autoCode.AutoCodeNum=AutoCodes.Insert(autoCode);
			AutoCodes.RefreshCache();
			return autoCode;
		}
	}
}
