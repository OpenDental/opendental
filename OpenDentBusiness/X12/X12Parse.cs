using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	public class X12Parse {

		public static DateTime ToDate(string element) {
			if(element.Length < 8) {
				return DateTime.MinValue;
			}
			int year=PIn.Int(element.Substring(0,4));
			if(year < 1880) {
				return DateTime.MinValue;
			}
			int month=PIn.Int(element.Substring(4,2));
			int day=PIn.Int(element.Substring(6,2));
			DateTime dt=new DateTime(year,month,day);
			return dt;
		}

		public static string UrlDecode(string t) {
			t=t.Replace("%3A",":");
			t=t.Replace("%26","&");
			t=t.Replace("%2F","/");
			t=t.Replace("%3D","=");
			t=t.Replace("%3F","?");
			//there are more we could do later.
			return t;
		}

	}
}
