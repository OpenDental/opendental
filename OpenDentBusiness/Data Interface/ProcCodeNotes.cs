using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcCodeNotes{
		#region CachePattern

		private class ProcCodeNoteCache : CacheListAbs<ProcCodeNote> {
			protected override List<ProcCodeNote> GetCacheFromDb() {
				string command="SELECT * FROM proccodenote";
				return Crud.ProcCodeNoteCrud.SelectMany(command);
			}
			protected override List<ProcCodeNote> TableToList(DataTable table) {
				return Crud.ProcCodeNoteCrud.TableToList(table);
			}
			protected override ProcCodeNote Copy(ProcCodeNote procCodeNote) {
				return procCodeNote.Copy();
			}
			protected override DataTable ListToTable(List<ProcCodeNote> listProcCodeNotes) {
				return Crud.ProcCodeNoteCrud.ListToTable(listProcCodeNotes,"ProcCodeNote");
			}
			protected override void FillCacheIfNeeded() {
				ProcCodeNotes.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcCodeNoteCache _procCodeNoteCache=new ProcCodeNoteCache();

		public static List<ProcCodeNote> GetDeepCopy(bool isShort=false) {
			return _procCodeNoteCache.GetDeepCopy(isShort);
		}

		public static ProcCodeNote GetFirstOrDefault(Func<ProcCodeNote,bool> match,bool isShort=false) {
			return _procCodeNoteCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procCodeNoteCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procCodeNoteCache.FillCacheFromTable(table);
				return table;
			}
			return _procCodeNoteCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static List<ProcCodeNote> GetList(long codeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcCodeNote>>(MethodBase.GetCurrentMethod(),codeNum);
			}
			string command="SELECT * FROM proccodenote WHERE CodeNum="+POut.Long(codeNum);
			return Crud.ProcCodeNoteCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(ProcCodeNote note) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				note.ProcCodeNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),note);
				return note.ProcCodeNoteNum;
			}
			return Crud.ProcCodeNoteCrud.Insert(note);
		}

		///<summary></summary>
		public static void Update(ProcCodeNote note){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),note);
				return;
			}
			Crud.ProcCodeNoteCrud.Update(note);
		}

		public static void Delete(long procCodeNoteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procCodeNoteNum);
				return;
			}
			string command="DELETE FROM proccodenote WHERE ProcCodeNoteNum = "+POut.Long(procCodeNoteNum);
			Db.NonQ(command);
		}

		///<summary>Gets the note for the given provider, if one exists.  Otherwise, gets the proccode.defaultnote.
		///Currently procStatus only supports TP or C statuses.</summary>
		public static string GetNote(long provNum,long codeNum, ProcStat procStatus, bool isGroupNote=false) {
			//No need to check RemotingRole; no call to db.
			List<ProcCodeNote> listProcCodeNotes=GetDeepCopy();
			for(int i=0;i<listProcCodeNotes.Count;i++) {
				if(listProcCodeNotes[i].ProvNum!=provNum) {
					continue;
				}
				if(listProcCodeNotes[i].CodeNum!=codeNum) {
					continue;
				}
				//Skip provider specific notes if this is a group note and the procedure is not complete
				// OR if this is NOT a group note and the procedure does not have the desired status.
				if((isGroupNote && listProcCodeNotes[i].ProcStatus!=ProcStat.C) 
					|| (!isGroupNote && listProcCodeNotes[i].ProcStatus!=procStatus))
				{
					continue;
				}
				return listProcCodeNotes[i].Note;
			}
			//A provider specific procedure code note could not be found, use the default for the procedure code.
			if(procStatus==ProcStat.TP) {
					return ProcedureCodes.GetProcCode(codeNum).DefaultTPNote;
				}
			return ProcedureCodes.GetProcCode(codeNum).DefaultNote;
		}

		///<summary>Gets the time pattern for the given provider, if one exists.  Otherwise, gets the proccode.ProcTime.</summary>
		public static string GetTimePattern(long provNum,long codeNum) {
			//No need to check RemotingRole; no call to db.
			ProcCodeNote procCodeNote=GetFirstOrDefault(x => x.ProvNum==provNum && x.CodeNum==codeNum);
			return (procCodeNote==null ? ProcedureCodes.GetProcCode(codeNum).ProcTime : procCodeNote.ProcTime);
		}
	}

}