using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	public class RpProcNote {
			
		public static DataTable GetData(List<long> listProvNums,List<long> listClinicNums,DateTime dateStart,DateTime dateEnd,bool includeNoNotes,
			bool includeUnsignedNotes,ToothNumberingNomenclature toothNumberFormat,ProcNoteGroupBy groupBy, bool showExcludedCodes = false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listProvNums,listClinicNums,dateStart,dateEnd,includeNoNotes,includeUnsignedNotes,
					toothNumberFormat,groupBy, showExcludedCodes);
			}
			string [] arrayExcludedCodes = PrefName.ReportsIncompleteProcsExcludeCodes.GetValueAsText().Split(",",StringSplitOptions.RemoveEmptyEntries);
			string whereNoNote="";
			string whereUnsignedNote="";
			string whereNotesClause="";
			if(includeNoNotes) {
				whereNoNote=@"
					LEFT JOIN (
						SELECT procedurelog.PatNum,procedurelog.ProcDate,procnote.Note
						FROM procedurelog
						INNER JOIN procnote ON procnote.ProcNum=procedurelog.ProcNum
						INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum
							AND procedurecode.ProcCode NOT IN ('D9986','D9987')
						WHERE procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+@"
						AND (procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" OR (procedurelog.ProcStatus="+POut.Int((int)ProcStat.EC)
						+@" AND procedurecode.ProcCode='~GRP~'))"
						+@" AND procnote.EntryDateTime=(SELECT MAX(lastnote.EntryDateTime) 
							FROM procnote lastnote 
							WHERE procnote.ProcNum=lastnote.ProcNum)"
						+@" GROUP BY procedurelog.ProcNum
							HAVING LENGTH(procnote.Note)>0
					) hasNotes ON hasNotes.PatNum=procedurelog.PatNum AND hasNotes.ProcDate=procedurelog.ProcDate ";
				whereNotesClause=@"AND IF(procedurecode.ProcCode IN ('D9986','D9987'),(n1.Note LIKE '%""""%' OR n1.Note REGEXP '\[Prompt:""[a-zA-Z_0-9]+""\]'),TRUE) "; // if using no notes option, only show broken appointment codes that have empty quotes or prompts
				if(!includeUnsignedNotes) {
					whereNotesClause+="AND (n1.ProcNum IS NOT NULL OR hasNotes.PatNum IS NULL)";
				}
			}
			if(includeUnsignedNotes) {
				if(includeNoNotes) {
					whereNotesClause+="AND (n1.ProcNum IS NOT NULL OR hasNotes.PatNum IS NULL OR unsignedNotes.ProcNum IS NOT NULL)";
				}
				else {
					whereNotesClause+="AND (n1.ProcNum IS NOT NULL OR unsignedNotes.ProcNum IS NOT NULL)";
				}
				whereUnsignedNote=@"
					LEFT JOIN procnote unsignedNotes ON unsignedNotes.ProcNum=procedurelog.ProcNum
						AND unsignedNotes.Signature=''
						AND	TRIM(unsignedNotes.Note)!=''
						AND unsignedNotes.EntryDateTime= (SELECT MAX(n2.EntryDateTime) 
								FROM procnote n2 
								WHERE unsignedNotes.ProcNum = n2.ProcNum) ";
			}
			string command=@"SELECT MAX(procedurelog.ProcDate) ProcDate,MAX(CONCAT(CONCAT(patient.LName, ', '),patient.FName)) PatName,procedurelog.PatNum,
				(CASE WHEN COUNT(procedurelog.ProcNum)=1 THEN MAX(procedurecode.ProcCode) ELSE '' END) ProcCode,
				(CASE WHEN COUNT(procedurelog.ProcNum)=1 THEN MAX(procedurecode.Descript) ELSE '"+Lans.g("FormRpProcNote","Multiple procedures")+@"' END) Descript,
				(CASE WHEN COUNT(procedurelog.ProcNum)=1 THEN MAX(procedurelog.ToothNum) ELSE '' END) ToothNum,
				(CASE WHEN COUNT(procedurelog.ProcNum)=1 THEN MAX(procedurelog.Surf) ELSE '' END) Surf "
				+(includeNoNotes || includeUnsignedNotes?",(CASE WHEN MAX(n1.ProcNum) IS NOT NULL THEN 'X' ELSE '' END) AS Incomplete ":"")
				+(includeNoNotes?",(CASE WHEN MAX(hasNotes.PatNum) IS NULL THEN 'X' ELSE '' END) AS HasNoNote ":"")
				+(includeUnsignedNotes?",(CASE WHEN MAX(unsignedNotes.ProcNum) IS NOT NULL THEN 'X' ELSE '' END) AS HasUnsignedNote ":"")+@" 
				FROM procedurelog
				INNER JOIN patient ON procedurelog.PatNum = patient.PatNum 
				INNER JOIN procedurecode ON procedurelog.CodeNum = procedurecode.CodeNum 
				"+(includeNoNotes || includeUnsignedNotes?"LEFT":"INNER")+@" JOIN procnote n1 ON procedurelog.ProcNum = n1.ProcNum 
					AND (n1.Note LIKE '%""""%' OR n1.Note REGEXP '"+@"\[Prompt:""[a-zA-Z_0-9 ]+""\]') "//looks for either "" (pre 17.3) or [Prompt:"{word}"] (post 17.3)
				+@" AND n1.EntryDateTime= (SELECT MAX(n2.EntryDateTime) 
				FROM procnote n2 
				WHERE n1.ProcNum = n2.ProcNum) "
				+whereNoNote+" "
				+whereUnsignedNote+@"
				WHERE procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+@"
				AND (procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)
				+" OR (procedurelog.ProcStatus="+POut.Int((int)ProcStat.EC)+" "
				+@" AND procedurecode.ProcCode='~GRP~')) ";
				if(!showExcludedCodes) {
					command+=$"AND procedurecode.ProcCode NOT IN ('{string.Join("','",arrayExcludedCodes)}') ";
				}
				command+=whereNotesClause;
			if(listProvNums.Count>0) {
				command+=@"AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(listClinicNums.Count>0) {
				command+=@"AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(groupBy==ProcNoteGroupBy.Patient) {
				command+=@"GROUP BY procedurelog.PatNum ";
			}
			else if(groupBy==ProcNoteGroupBy.DateAndPatient) {
				command+=@"GROUP BY procedurelog.ProcDate,procedurelog.PatNum ";
			}
			else {
				command+="GROUP BY procedurelog.ProcNum ";
			}
			command+=@"ORDER BY ProcDate, LName";
			DataTable table=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			foreach(DataRow row in table.Rows) {
				row["ToothNum"]=Tooth.ToInternat(row["ToothNum"].ToString(),toothNumberFormat);
			}
			return table;
		}

		///<summary>The different ways this report can be grouped.</summary>
		public enum ProcNoteGroupBy {
			///<summary>0</summary>
			Procedure,
			///<summary>1</summary>
			Patient,
			///<summary>2</summary>
			DateAndPatient
		}
	}
}
