using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AutoNoteT {
		///<summary>Creates an autonote. Set doInsert to true to insert the new autonote.</summary>
		public static AutoNote CreateAutoNote(string autoNoteName="",string mainText="",long category=0,bool doInsert=true) {
			AutoNote autoNote=new AutoNote {
				AutoNoteName=autoNoteName,
				MainText=mainText,
				Category=category
			};
			if(doInsert) {
				AutoNotes.Insert(autoNote);
			}
			return autoNote;
		}

		///<summary>Deletes everything from the autonote table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearAutoNoteTable() {
			string command="DELETE FROM autonote WHERE AutoNoteNum > 0";
			OpenDentBusiness.DataCore.NonQ(command);
		}
	}
}
