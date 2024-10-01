using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class LanguagePatT {
		
		///<summary>Deletes everything from the languagepat table.</summary>
		public static void ClearLanguagePatTable() {
			string command="DELETE FROM languagepat";
			DataCore.NonQ(command);
		}
	}
}