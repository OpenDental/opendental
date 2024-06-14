using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class ProcNotes{
		#region Get Methods
		///<summary>Returns all procedure notes for the procedure passed in.  Returned list is not sorted.</summary>
		public static List<ProcNote> GetProcNotesForProc(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ProcNote>>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM procnote WHERE ProcNum="+POut.Long(procNum);
			return Crud.ProcNoteCrud.SelectMany(command);
		}
		#endregion
		
		public static long Insert(ProcNote procNote){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				procNote.ProcNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),procNote);
				return procNote.ProcNoteNum;
			}
			return Crud.ProcNoteCrud.Insert(procNote);
		}

		public static ProcNote GetProcNotesForPat(long patNum, DateTime dateStart, DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ProcNote>(MethodBase.GetCurrentMethod(),patNum,dateStart,dateEnd);
			}
			string query="SELECT procnote.* FROM procnote "
				+"INNER JOIN procedurelog ON procedurelog.ProcNum=procnote.ProcNum "
				+"WHERE procnote.PatNum="+POut.Long(patNum)+" "
				+"AND procnote.EntryDateTime BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND procedurelog.ProcStatus!="+POut.Int((int)ProcStat.D)+" "
				+"ORDER BY procnote.EntryDateTime DESC";
			string command=DbHelper.LimitOrderBy(query,1);
			return Crud.ProcNoteCrud.SelectOne(command);
		}

		///<summary>Returns a list of ProcNums from listProcNums where the most recent ProcNote for the proc is signed.</summary>
		public static List<long> GetIsProcNoteSigned(List<long> listProcNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			if(listProcNums.Count==0) {
				return new List<long>();
			}
			string command="SELECT * FROM procnote WHERE ProcNum IN ("+string.Join(",",listProcNums.Select(x => POut.Long(x)))+")";
			List<ProcNote> listProcNotes=Crud.ProcNoteCrud.SelectMany(command);//get all ProcNotes with ProcNum in the supplied list
			if(listProcNotes.Count==0) {
				return new List<long>();
			}
			return listProcNotes
				.GroupBy(x => x.ProcNum,(x,y) => y.Aggregate((y1,y2) => y1.EntryDateTime>y2.EntryDateTime?y1:y2))//group by ProcNum, get most recent ProcNote
				.Where(x => !string.IsNullOrWhiteSpace(x.Signature))//where the most recent ProcNote is signed
				.Select(x => x.ProcNum).ToList();//return list of ProcNums
		}

		///<summary>Modifies currentNote and returns the new note string. Also checks PrefName.ProcPromptForAutoNote and remots auto notes if needed.</summary>
		public static string SetProcCompleteNoteHelper(bool isQuickAdd,Procedure procedure,Procedure procedureOld,long provNum,string currentNote="") {
			string procNoteDefault="";
			if(isQuickAdd) {//Quick Procs should insert both TP Default Note and C Default Note.
				procNoteDefault=ProcCodeNotes.GetNote(provNum,procedure.CodeNum,ProcStat.TP);
				if(!string.IsNullOrEmpty(procNoteDefault)) {
					procNoteDefault+="\r\n";
				}
			}
			if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C) {//Only append the default note if the procedure changed status to Completed
				procNoteDefault+=ProcCodeNotes.GetNote(provNum,procedure.CodeNum,ProcStat.C);
				if(currentNote!="" && procNoteDefault!="") { //check to see if a default note is defined.
					currentNote+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
				}
				if(!string.IsNullOrEmpty(procNoteDefault)) {
					currentNote+=procNoteDefault;
				}
			}
			if(procedure.ProcStatus==ProcStat.TP && procedureOld.ProcStatus==ProcStat.D){//Append the TP note if the user had to enter a tooth number or quadrant
				procNoteDefault+=ProcCodeNotes.GetNote(provNum,procedure.CodeNum,ProcStat.TP);
				if(currentNote!="" && procNoteDefault!="") { //check to see if a default note is defined.
					currentNote+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
				}
				if(!string.IsNullOrEmpty(procNoteDefault)) {
					currentNote+=procNoteDefault;
				}
			}
			if(!PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {
				//Users do not want to be prompted for auto notes, so remove them all from the procedure note.
				currentNote=Regex.Replace(currentNote,@"\[\[.+?\]\]","");
			}
			return currentNote;
		}
		
		/*
		///<summary></summary>
		internal static bool PreviousNoteExists(int procNum){
			string command="SELECT COUNT(*) FROM procnote WHERE ProcNum="+POut.PInt(procNum);
			DataConnection dcon=new DataConnection();
			if(dcon.GetCount(command)=="0"){
				return false;
			}
			return true;
		}*/
	}



}
