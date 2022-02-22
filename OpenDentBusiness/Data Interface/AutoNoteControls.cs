using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	public class AutoNoteControls {
		#region Cache Pattern

		private class AutoNoteControlCache : CacheListAbs<AutoNoteControl> {
			protected override List<AutoNoteControl> GetCacheFromDb() {
				string command="SELECT * FROM autonotecontrol ORDER BY Descript";
				return Crud.AutoNoteControlCrud.SelectMany(command);
			}
			protected override List<AutoNoteControl> TableToList(DataTable table) {
				return Crud.AutoNoteControlCrud.TableToList(table);
			}
			protected override AutoNoteControl Copy(AutoNoteControl autoNoteControl) {
				return autoNoteControl.Copy();
			}
			protected override DataTable ListToTable(List<AutoNoteControl> listAutoNoteControls) {
				return Crud.AutoNoteControlCrud.ListToTable(listAutoNoteControls,"AutoNoteControl");
			}
			protected override void FillCacheIfNeeded() {
				AutoNoteControls.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoNoteControlCache _autoNoteControlCache=new AutoNoteControlCache();

		public static List<AutoNoteControl> GetDeepCopy(bool isShort=false) {
			return _autoNoteControlCache.GetDeepCopy(isShort);
		}

		private static AutoNoteControl GetFirstOrDefault(Func<AutoNoteControl,bool> match,bool isShort=false) {
			return _autoNoteControlCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoNoteControlCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoNoteControlCache.FillCacheFromTable(table);
				return table;
			}
			return _autoNoteControlCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		public static long Insert(AutoNoteControl autoNoteControl) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				autoNoteControl.AutoNoteControlNum=Meth.GetLong(MethodBase.GetCurrentMethod(),autoNoteControl);
				return autoNoteControl.AutoNoteControlNum;
			}
			return Crud.AutoNoteControlCrud.Insert(autoNoteControl);
		}

		public static void InsertBatch(List<SerializableAutoNoteControl> listSerializableAutoNoteControls) {
			if(listSerializableAutoNoteControls==null || listSerializableAutoNoteControls.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSerializableAutoNoteControls);
				return;
			}
			List<AutoNoteControl> listAutoNoteControls=new List<AutoNoteControl>();
			foreach(SerializableAutoNoteControl control in listSerializableAutoNoteControls) {
				AutoNoteControl newControl=new AutoNoteControl();
				newControl.ControlLabel=control.ControlLabel;
				newControl.ControlOptions=control.ControlOptions;
				newControl.ControlType=control.ControlType;
				newControl.Descript=control.Descript;
				listAutoNoteControls.Add(newControl);
			}
			Crud.AutoNoteControlCrud.InsertMany(listAutoNoteControls);
		}

		public static void Update(AutoNoteControl autoNoteControl) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoNoteControl);
				return;
			}
			Crud.AutoNoteControlCrud.Update(autoNoteControl);
		}

		public static void Delete(long autoNoteControlNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoNoteControlNum);
				return;
			}
			//no validation for now.
			string command="DELETE FROM autonotecontrol WHERE AutoNoteControlNum="+POut.Long(autoNoteControlNum);
			Db.NonQ(command);
		}

		///<summary>Will return null if can't match.</summary>
		public static AutoNoteControl GetByDescript(string descript) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.Descript==descript);
		}

		/// <summary>Takes in a list of control descripts and returns a list of AutoNoteControls ready for JSON serialization
		/// (currently only used for exporting AutoNotes).</summary>
		public static List<SerializableAutoNoteControl> GetSerializableAutoNoteControls(List<AutoNoteControl> listAutoNoteControls) {
			//No need to check RemotingRole; no call to db.
			return listAutoNoteControls.Select(x => new SerializableAutoNoteControl(x)).ToList();
		}

		///<summary>Returns a list of unique AutoNoteControls based on parsing a list of AutoNotes' MainText (used for exporting AutoNoteControls).</summary>
		public static List<AutoNoteControl> GetListByParsingAutoNoteText(List<SerializableAutoNote> listSerializableAutoNotes) {
			//No need to check RemotingRole; no call to db.
			List<AutoNoteControl> listAutoNoteControls=new List<AutoNoteControl>();
			List<Match> listPrompts=new List<Match>();
			//Find all prompts in all the provided AutoNotes
			foreach(SerializableAutoNote autoNote in listSerializableAutoNotes) {
				listPrompts.AddRange(GetPrompts(autoNote.MainText));
			}
			//Clean up the text for every discovered prompt to retrieve the appropriate AutoNoteControl if it exists
			foreach(Match control in listPrompts) {
				string descript=control.ToString();
				//Trimming down the match to just the Descript of the prompt text
				descript=descript.Replace("[Prompt:\"","");
				descript=descript.Replace("\"]","");
				AutoNoteControl newControl=GetByDescript(descript);
				if(newControl!=null) {
					listAutoNoteControls.Add(newControl); 
				}
			}
			return listAutoNoteControls.DistinctBy(x => x.Descript).ToList();
		}

		///<summary>Returns all the auto note prompts within the noteText.</summary>
		public static List<Match> GetPrompts(string noteText) {
			//No need to check RemotingRole; no call to db.
			//Prompts are stored in the form [Prompt: "PromptName"]
			return Regex.Matches(noteText,@"\[Prompt:""[a-zA-Z_0-9 ]+""\]").OfType<Match>().ToList();
		}

		///<summary>(Only used during AutoNote import process) Takes in a List of SerializableAutoNoteControl and SerializableAutoNotes and trims 
		///the Control list down to only the unique items. This appends numbers to names if names are duplicated but prompt text is new and, in 
		///that case, changes the prompt name in the associated AutoNote.MainText. This operates on the objects passed in so there is no return
		///value.</summary>
		public static void RemoveDuplicatesFromList(List<SerializableAutoNoteControl> listSerializableAutoNoteControls,List<SerializableAutoNote> listSerializableAutoNotes) {
			List<string> listTrueDuplicates=new List<string>();
			foreach(SerializableAutoNoteControl curControl in listSerializableAutoNoteControls) {
				bool wasNameChanged=false;
				AutoNoteControl dbControl=GetByDescript(curControl.Descript);
				int dupNum=0;
				string name=curControl.Descript; //Used to hold the current name so we can safely change the name while we check for duplicates
				while(dbControl!=null) { //While the control name is not unique
					if(dbControl.ControlOptions==curControl.ControlOptions 
						&& dbControl.ControlType==curControl.ControlType)  //ControlLabel is omitted because it serves no functional purpose in a duplication check
					{
						listTrueDuplicates.Add(curControl.Descript); //Add this to a list of true duplicates so it doesn't get reimported
						break;
					}
					dupNum++;
					curControl.Descript=string.Join("_",name,dupNum.ToString());
					wasNameChanged=true;
					dbControl=GetByDescript(curControl.Descript);
				}
				if(wasNameChanged) { //If the name changed, replace all instances of it across the new AutoNotes
					foreach(SerializableAutoNote note in listSerializableAutoNotes) {
						note.MainText=note.MainText.Replace("[Prompt:\""+name+"\"]","[Prompt:\""+curControl.Descript+"\"]");
					}
				}
			}
			listSerializableAutoNoteControls.RemoveAll(x => ListTools.In(x.Descript,listTrueDuplicates));
		}
	}

	/// <summary>A helper class that minimizes the amount of information that will get serialized during export.</summary>
	public class SerializableAutoNoteControl {
		///<summary>Corresponds to the AutoNoteControl.ControlLabel field.</summary>
		public string ControlLabel;
		///<summary>Corresponds to the AutoNoteControl.ControlOptions field.</summary>
		public string ControlOptions;
		///<summary>Corresponds to AutoNoteControl.ConrolType field.</summary>
		public string ControlType;
		///<summary>Corresponds to AutoNoteControl.Descript field.</summary>
		public string Descript;

		/// <summary>Required for serialization. Do not use.</summary>
		public SerializableAutoNoteControl() {
		}

		public SerializableAutoNoteControl(AutoNoteControl control) {
			ControlLabel=control.ControlLabel;
			ControlOptions=control.ControlOptions;
			ControlType=control.ControlType;
			Descript=control.Descript;
		}
	}
}

