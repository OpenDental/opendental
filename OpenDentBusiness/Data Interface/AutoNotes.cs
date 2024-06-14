using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Newtonsoft.Json;
using CodeBase;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using OpenDental.Thinfinity;

namespace OpenDentBusiness {
	public class AutoNotes {
		#region Cache Pattern

		private class AutoNoteCache : CacheListAbs<AutoNote> {
			protected override List<AutoNote> GetCacheFromDb() {
				string command="SELECT * FROM autonote ORDER BY AutoNoteName";
				return Crud.AutoNoteCrud.SelectMany(command);
			}
			protected override List<AutoNote> TableToList(DataTable table) {
				return Crud.AutoNoteCrud.TableToList(table);
			}
			protected override AutoNote Copy(AutoNote autoNote) {
				return autoNote.Copy();
			}
			protected override DataTable ListToTable(List<AutoNote> listAutoNotes) {
				return Crud.AutoNoteCrud.ListToTable(listAutoNotes,"AutoNote");
			}
			protected override void FillCacheIfNeeded() {
				AutoNotes.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoNoteCache _autoNoteCache=new AutoNoteCache();

		public static List<AutoNote> GetDeepCopy(bool isShort=false) {
			return _autoNoteCache.GetDeepCopy(isShort);
		}

		public static List<AutoNote> GetWhere(Predicate<AutoNote> match,bool isShort=false) {
			return _autoNoteCache.GetWhere(match,isShort);
		}

		public static bool GetExists(Predicate<AutoNote> match,bool isShort=false) {
			return _autoNoteCache.GetExists(match,isShort);
		}

		private static AutoNote GetFirstOrDefault(Func<AutoNote,bool> match,bool isShort=false) {
			return _autoNoteCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoNoteCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoNoteCache.FillCacheFromTable(table);
				return table;
			}
			return _autoNoteCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_autoNoteCache.ClearCache();
		}
		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(AutoNote autoNote) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				autoNote.AutoNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),autoNote);
				return autoNote.AutoNoteNum;
			}
			return Crud.AutoNoteCrud.Insert(autoNote);
		}

		public static void InsertBatch(List<SerializableAutoNote> listSerializableAutoNotes) {
			if(listSerializableAutoNotes==null || listSerializableAutoNotes.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSerializableAutoNotes);
				return;
			}
			List<AutoNote> listAutoNotes=new List<AutoNote>();
			for(int i=0;i<listSerializableAutoNotes.Count;i++) {
				AutoNote newNote=new AutoNote();
				newNote.AutoNoteName=listSerializableAutoNotes[i].AutoNoteName;
				newNote.Category=0;//It would be too error-prone trying to select the category, so we'll just leave it as 0.
				newNote.MainText=listSerializableAutoNotes[i].MainText;
				listAutoNotes.Add(newNote);
			}
			Crud.AutoNoteCrud.InsertMany(listAutoNotes);
		}

		///<summary></summary>
		public static void Update(AutoNote autoNote) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoNote);
				return;
			}
			Crud.AutoNoteCrud.Update(autoNote);
		}

		///<summary></summary>
		public static void Delete(long autoNoteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoNoteNum);
				return;
			}
			string command="DELETE FROM autonote "
				+"WHERE AutoNoteNum = "+POut.Long(autoNoteNum);
			Db.NonQ(command);
		}

		public static string GetByTitle(string autoNoteTitle) {
			//No need to check MiddleTierRole; no call to db.
			AutoNote autoNote=GetFirstOrDefault(x => x.AutoNoteName==autoNoteTitle);
			if(autoNote==null) {
				return "";
			}
			return autoNote.MainText;
		}

		/// <summary>Receives a list of AutoNotes and returns a list of AutoNotes ready for JSON serialization
		/// (currently only used for exporting AutoNotes).</summary>
		public static List<SerializableAutoNote> GetSerializableAutoNotes(List<AutoNote> listAutoNotes) {
			//No need to check MiddleTierRole; no call to db.
			return listAutoNotes.Select(x => new SerializableAutoNote(x)).ToList();
		}

		///<summary>Gets an AutoNote by primary key from the database for the API. Returns null if not found.</summary>
		public static AutoNote GetAutoNoteForApi(long autoNoteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<AutoNote>(MethodBase.GetCurrentMethod(),autoNoteNum);
			}
			return Crud.AutoNoteCrud.SelectOne(autoNoteNum);
		}

		///<summary>Returns the AutoNoteName from the passed in promptRepsonse. Returns empty string if no valid AutoNote is found.</summary>
		public static string GetAutoNoteName(string promptResponse) {
			string retVal="";
			Stack<int> stackBrackets=new Stack<int>();
			//The AutoNoteResponseText should be in format "Auto Note Response Text : {AutoNoteName}". Go through each of the charactors in promptResponse
			//and find each of the possible AutoNote names. We need to do this in case the AutoNote name has brackets("{,}") in the name. 
			for(int posCur=0;posCur<promptResponse.Length;posCur++) {
				if(promptResponse[posCur]=='{') {
					stackBrackets.Push(posCur);//add the position of the '{' to the stack.
					continue;
				}
				if(promptResponse[posCur]!='}' || stackBrackets.Count()==0) {//continue if the the stack does not have an open '{', or this is not a '}'
					continue;
				}
				//The posOpenBracket will include the '{'. We will have to remove it.
				int posOpenBracket=stackBrackets.Peek();
				//Get the length of the possible autonote. The length will include the closing '}'. We will also need to remove it.
				int length=posCur-posOpenBracket;
				if(length<1) {
					stackBrackets.Pop();
					continue;
				}
				//Get string of possible AutoNoteName. Remove the bracket from the beginning and end. 
				string autoNoteName=promptResponse.Substring(posOpenBracket+1,length-1);
				if(!string.IsNullOrEmpty(autoNoteName) && AutoNotes.IsValidAutoNote(autoNoteName)) {
					retVal=autoNoteName;
					break;
				}
				//no match found. Remove position from stack and continue.
				stackBrackets.Pop();
			}
			return retVal;//could be empty string if no valid autonote name is found
		}

		///<summary>Returns true if there is a valid AutoNote for the passed in AutoNoteName.</summary>
		public static bool IsValidAutoNote(string autoNoteName) {
			//No need to check MiddleTierRole; no call to db.
			AutoNote autoNote=GetFirstOrDefault(x => x.AutoNoteName==autoNoteName);
			return autoNote!=null;
		}

		///<summary>Sets the autonote.Category=0 for the autonote category DefNum provided.  Returns the number of rows updated.</summary>
		public static long RemoveFromCategory(long autoNoteCatDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),autoNoteCatDefNum);
			}
			string command="UPDATE autonote SET Category=0 WHERE Category="+POut.Long(autoNoteCatDefNum);
			return Db.NonQ(command);
		}

		///<summary>Writes a set of serialized AutoNotes and AutoNoteControls in JSON format to the specified path.</summary>
		public static void WriteAutoNotesToJson(List<SerializableAutoNote> listSerializableAutoNotes,List<SerializableAutoNoteControl> listSerializableAutoNoteControls,
			string path) 
		{
			//No need to check MiddleTierRole; no call to db.
			TransferableAutoNotes transferableAutoNotes=new TransferableAutoNotes(listSerializableAutoNotes,listSerializableAutoNoteControls);
			string json=JsonConvert.SerializeObject(transferableAutoNotes);
			if(ODBuild.IsThinfinity()) {
			ThinfinityUtils.ExportForDownload(path,json);
			}
			else if(ODCloudClient.IsAppStream) {
				File.WriteAllText(path,json);
				ODCloudClient.ExportForAppStream(path,Path.GetFileName(path));
			}
			else {
				File.WriteAllText(path,json);
			}
		}
	}

	/// <summary>A helper class that minimizes the amount of information that will get serialized during export.</summary>
	public class SerializableAutoNote {
		///<summary>Corresponds to the AutoNote.AutoNoteName field.</summary>
		public string AutoNoteName;
		///<summary>Corresponds to the AutoNote.MainText field.</summary>
		public string MainText;

		///<summary>Required for serialization. Do not use.</summary>
		public SerializableAutoNote() {
		}

		public SerializableAutoNote(AutoNote autoNote) {
			AutoNoteName=autoNote.AutoNoteName;
			MainText=autoNote.MainText;
		}
	}

	/// <summary>A helper class that places the serialized AutoNote and AutoNoteControl lists under the umbrella of a single object
	/// (used for exporting to JSON file).</summary>
	public class TransferableAutoNotes {
		public List<SerializableAutoNote> AutoNotes;
		public List<SerializableAutoNoteControl> AutoNoteControls;

		/// <summary>Used for serialization only. Do not use.</summary>
		public TransferableAutoNotes() {
		}

		public TransferableAutoNotes(List<SerializableAutoNote> autoNotes,List<SerializableAutoNoteControl> autoNoteControls) {
			AutoNotes=autoNotes;
			AutoNoteControls=autoNoteControls;
		}
	}
}
