using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	///<summary>Has methods for both Sheet and SheetField. Use SheetDefT for defs.</summary>
	public class SheetT {
		///<summary>Deletes everything from the sheet and sheetfield table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearSheetAndSheetFieldTable() {
			string command="DELETE FROM sheet WHERE sheetnum > 0";
			DataCore.NonQ(command);
			command="DELETE FROM sheetfield WHERE sheetfieldnum > 0";
			DataCore.NonQ(command);
		}
	}
}
