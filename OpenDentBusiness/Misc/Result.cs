using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Use this instead of try/catch or outs when you need to return a boolean result along with one or more error message strings.  The strings all start out initialized to empty so that you don't have to test for null.</summary>
	public class Result {
		public bool IsSuccess;
		public string Msg="";
		///<summary>Rarely used, but available if you need it.</summary>
		public string Msg2="";
		///<summary>Rarely used, but available if you need it.</summary>
		public List<string> ListMsgs=new List<string>();

		///<summary>This makes code easier to read by not having a ! at the beginning where you won't notice it.</summary>
		public bool IsFailure(){
			return !IsSuccess;
		}
	}
}
