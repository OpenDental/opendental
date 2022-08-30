/*This attempt at directly casting data from the database into the correct type failed.  For any given type that we use in OD, the type coming from the database can vary significantly.  The problem is compounded when the data comes from multiple connectors and multiple database types.  Strings are the way to go except in unusual circumstances.
 * 
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Parameter object coming in from database.</summary>
	public class PoIn {
		///<summary></summary>
		public static bool Bool(object obj) {
			try {
				return ((int)obj)==1;
			}
			catch(Exception e) {
				Type t=obj.GetType();
			}
			return false;
		}

		///<summary></summary>
		public static byte Byte(object obj) {
			return (byte)obj;
		}

		///<summary></summary>
		public static long Long(object obj) {
			return (long)obj;
		}

		///<summary></summary>
		public static string String(object obj) {
			//This strategy was tested to take zero ticks on a null.
			return obj.ToString();
			//This strategy also takes zero ticks on a null, but does not seem to have any advantage.
			//bool isNull=(obj.GetType()==typeof(System.DBNull));
			//if(isNull) {
			//	return "";
			//}
			//return (string)obj;
		}

	

	}
}*/
