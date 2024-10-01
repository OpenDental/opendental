using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProcNoteT {

		///<summary>Insert and returns a ProcNote.</summary>
		public static ProcNote CreateProcNote(long patNum,long procNum,long userNum=0,string note="") {
			ProcNote procNote=new ProcNote {
				PatNum=patNum,
				ProcNum=procNum,
				UserNum=userNum,
				Note=note
			};
			procNote.ProcNoteNum=ProcNotes.Insert(procNote);
			return procNote;
		}

		///<summary>Deletes everything from the procnote table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearProcNoteTable() {
			string command=$"DELETE FROM procnote WHERE ProcNoteNum > 0";
			DataCore.NonQ(command);
		}


	}
}