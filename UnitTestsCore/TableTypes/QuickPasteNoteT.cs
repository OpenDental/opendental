using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore {
	public class QuickPasteNoteT {

		///<summary>Creates a quickpastenote. Set doInsert to true to insert the new quickpastenote.</summary>
		///<param name="quickPasteCatNum">quickpastecat.QuickPasteCatNum</param>
		///<param name="itemOrder">Order of note within category</param>
		public static QuickPasteNote CreateQuickPasteNote(long quickPasteCatNum,int itemOrder,string note,string abbreviation,bool doInsert=true) {
			QuickPasteNote quickPasteNote=new QuickPasteNote {
				QuickPasteCatNum=quickPasteCatNum,
				ItemOrder=itemOrder,
				Note=note,
				Abbreviation=abbreviation
			};
			if(doInsert) {
				QuickPasteNotes.Insert(quickPasteNote);
			}
			return quickPasteNote;
		}

		///<summary>Clears the quickpastenote table. Does not truncate as to not let the PKs repeat.</summary>
		public static void ClearQuickPasteNoteTable() {
			string command="DELETE FROM quickpastenote";
			DataCore.NonQ(command);
			QuickPasteNotes.RefreshCache();
		}

	}
}
