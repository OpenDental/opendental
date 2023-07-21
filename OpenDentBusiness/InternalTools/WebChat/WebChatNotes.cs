using CodeBase;
using OpenDentBusiness.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class WebChatNotes {

		///<summary>Also sets primary key and DateTcreated.</summary>
		public static long Insert(WebChatNote webChatNote) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webChatNote.WebChatNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webChatNote);
				return webChatNote.WebChatNoteNum;
			}
			long webChatNoteNum=0;
			WebChatMisc.DbAction(delegate() {
				webChatNoteNum=WebChatNoteCrud.Insert(webChatNote);
			});
			WebChatMisc.DbAction(delegate() {
				Signalods.SetInvalid(InvalidType.WebChatSessions);//Signal OD HQ to refresh sessions.
			},isWebChatDb:false);
			return webChatNoteNum;
		}

		public static WebChatNote GetOne(long webChatNoteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WebChatNote>(MethodBase.GetCurrentMethod(),webChatNoteNum);
			}
			WebChatNote webChatNote=null;
			WebChatMisc.DbAction(delegate() {
				webChatNote=WebChatNoteCrud.SelectOne(webChatNoteNum);
			});
			return webChatNote;
		}

		public static void Update(WebChatNote webChatNote,bool hasSignal=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webChatNote,hasSignal);
				return;
			}
			WebChatMisc.DbAction(delegate() {
				Crud.WebChatNoteCrud.Update(webChatNote);
			});
			if(hasSignal) {
				WebChatMisc.DbAction(delegate() {
					Signalods.SetInvalid(InvalidType.WebChatSessions);//Signal OD HQ to refresh sessions.
				},isWebChatDb:false);
			}
		}

		public static void Delete(long webChatNoteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webChatNoteNum);
				return;
			}
			WebChatMisc.DbAction(delegate() {
				WebChatNoteCrud.Delete(webChatNoteNum);
			});
		}

		public static List<WebChatNote> GetAllForSessions(List<long> listWebChatSessionNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebChatNote>>(MethodBase.GetCurrentMethod(),listWebChatSessionNums);
			}
			List<WebChatNote> listWebChatNotes=null;
			WebChatMisc.DbAction(delegate() {
				string command="SELECT * FROM webchatnote WHERE WebChatSessionNum IN ("+String.Join(",",listWebChatSessionNums.Select(x => POut.Long(x)))+")";
				listWebChatNotes=WebChatNoteCrud.SelectMany(command);
			});
			return listWebChatNotes;

		}

		///<summary>Returns a formatted string for display in the Chart and Account modules. Format is mimicks to task note in UserControlTasks.cs.</summary>
		public static string FormatForDisplay(List<WebChatNote> listWebChatNotes) {
			//No need to check MiddleTierRole; no call to db.
			if(listWebChatNotes.IsNullOrEmpty()) {
				return "";
			}
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<listWebChatNotes.Count;i++) {
				stringBuilder.Append("==");
				stringBuilder.Append(listWebChatNotes[i].TechName);
				stringBuilder.Append(" - ");
				stringBuilder.Append(listWebChatNotes[i].DateTimeNote);
				stringBuilder.Append(" - ");
				stringBuilder.Append(listWebChatNotes[i].Note);
				if(i < listWebChatNotes.Count-1) {
					stringBuilder.Append("\r\n");
				}
			}
			return stringBuilder.ToString();
		}
	}
}