using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDentBusiness.UI {
	///<summary>A helper class used add reference links to context menus</summary>
	public class PopupHelper {
		#region Methods - Private
		private static void OpenWikiPage(string pageTitle) {
			if(WikiPages.NavPageDelegate!=null) {
				WikiPages.NavPageDelegate.Invoke(pageTitle);
			}
		}

		private static void OpenPatNum(long patNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				Object sender="OpenDental";
				MessageBox.Show(Lans.g(sender.GetType().Name,"Patient does not exist."));
				return;
			}
			else if(Patients.NavPatDelegate!=null) {
				Patients.NavPatDelegate.Invoke(pat,true);
			}
		}

		private static void OpenTaskNum(long taskNum) {
			if(Tasks.NavTaskDelegate!=null) {
				Tasks.NavTaskDelegate.Invoke(taskNum);
			}
		}

		private static void OpenWebPage(string url) {
			try {
				if(!url.ToLower().StartsWith("http")) {
					url=@"http://"+url;
				}
				Process.Start(url);
			}
			catch {
				MessageBox.Show(Lans.g("PopupHelper","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again."),Lans.g("PopupHelper","Attention"));
			}
		}

		private static void OpenUNCPath(string folderPath) {
			//It is significantly faster to check if the directory exists before calling Process.Start() in the case that you have an invalid path.
			//Everything is a directory, scrubbed all specific files.
			bool isValidPath=Directory.Exists(folderPath);
			if(isValidPath) {
				try {
					Process.Start(folderPath);
				}
				catch(Exception e) {
					MessageBox.Show(e.Message);
				}
			}
			else {
				MessageBox.Show(Lans.g("PopupHelper","Failed to open file location. Please make sure file path is valid."));
			}
		}

		///<summary>Regular expresion used to help identify URLs. This is not all encompassing, there will be URLs that do not match this but this should work for 99%. May match URLs inside of parenthesis. These should be trimmed on both sides.</summary>
		private static string UrlRegexString() {
			string singleMatch=@"(http:\/\/|https:\/\/)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=,()]*)";
			//The first match matches normal URLs. The second match matches URLs surrounded in parenthesis. It also checks the next character
			//after a closing parenthesis to make sure the parenthesis was not simply found somewhere in the URL. The next character can be
			//punctuation and it will match fine. Otherwise, it will default to the normal URL match. The parenthesis are stripped out after the
			//match
			string retVal=$@"({singleMatch})|(\({singleMatch}\)(?!([-a-zA-Z0-9\@:%_\+~#\&//=()])))";
			return retVal;
		}
		#endregion Methods - Private

		#region Methods - Public
		///<summary>For a given context menu item, returns a sorted list of menu item links. This supports wiki, patient, task, URL, web, and file explorer.</summary>
		public static List<MenuItem> GetContextMenuItemLinks(string contextMenuItemText,bool doShowPatNumLinks,bool doShowTaskNumLinks) {
			List<MenuItem> listMenuItemLinks=new List<MenuItem>();
			List<string> listStringMatches=new List<string>();
			List<long> listNumMatches=new List<long>();
			bool doWikiLogic=false;//Default the Wiki logic to false
			try {
				//NOTE: if this preference is changed while the program is open there MAY be some lingering wiki links in the context menu. 
				//It is not worth it to force users to log off and back on again, or to run the link removal code below EVERY time, even if the pref is disabled.
				doWikiLogic=PrefC.GetBool(PrefName.WikiDetectLinks);//if this fails then we do not have a pref table or a wiki, so don't bother going with this part.
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			if(doWikiLogic) {
				listStringMatches=Regex.Matches(contextMenuItemText,@"\[\[.+?]]")
					.OfType<Match>()
					.Select(m => m.Groups[0].Value.Trim('[').Trim(']'))
					.Distinct()
					.ToList();
				for(int i=0;i<listStringMatches.Count;i++) {
					string curMatch=listStringMatches[i]; //To avoid lazy eval
					EventHandler onClick=(s,eArg) => { OpenWikiPage(curMatch); };
					listMenuItemLinks.Add(new MenuItem("Wiki - "+listStringMatches[i],onClick));
				}
			}
			listStringMatches=GetURLsFromText(contextMenuItemText);
			for(int i=0;i<listStringMatches.Count;i++) {
				try {
					MailAddress emailAddress=new MailAddress(listStringMatches[i]);
					continue;//'match' is a valid email address, which at this time we don't want to create a ContextMenu Web link for.
				}
				catch(FormatException fe) {
					fe.DoNothing();//Not a valid email address format, so it should be a web link.  Carry on to creating the item in the ContextMenu.
				}
				string title=listStringMatches[i];
				if(title.Length>24) {
					title=title.Substring(0,24)+"...";
				}
				string curMatch=listStringMatches[i]; //To avoid lazy eval
				EventHandler onClick=(s,eArg)=> { OpenWebPage(curMatch); };
				listMenuItemLinks.Add(new MenuItem("Web - "+title,onClick));
			}
			listStringMatches=ODFileUtils.GetFilePathsFromText(contextMenuItemText);
			for(int i=0;i<listStringMatches.Count;i++) {
				string curMatch=listStringMatches[i]; //To avoid lazy eval
				EventHandler onClick=(s,eArg) => { OpenUNCPath(curMatch); };
				listMenuItemLinks.Add(new MenuItem("File Explorer - "+listStringMatches[i],onClick));
			}
			if(doShowPatNumLinks) {
				listNumMatches=GetPatNumsFromText(contextMenuItemText);
				for(int i=0;i<listNumMatches.Count;i++) {
					long curMatch=listNumMatches[i];
					EventHandler onClick=(s,eArg) => { OpenPatNum(curMatch); };
					listMenuItemLinks.Add(new MenuItem("PatNum - "+listNumMatches[i],onClick));
				}
			}
			if(doShowTaskNumLinks) {
				listNumMatches=GetTaskNumsFromText(contextMenuItemText);
				for(int i=0;i<listNumMatches.Count;i++) {
					long curMatch=listNumMatches[i];
					EventHandler onClick=(s,eArg) => { OpenTaskNum(curMatch); };
					listMenuItemLinks.Add(new MenuItem("TaskNum - "+listNumMatches[i],onClick));
				}
			}
			listMenuItemLinks=listMenuItemLinks.OrderByDescending(x => x.Text=="-").ThenBy(x => x.Text).ToList();//alphabetize the link items.
			return listMenuItemLinks;
		}

		///<summary>Returns a list of strings from the given text that are URLs.</summary>
		public static List<string> GetURLsFromText(string text) {
			string urlRegexString=UrlRegexString();
			List<string> listStringMatches=Regex.Matches(text,urlRegexString)
				.OfType<Match>()
				.Select(m => m.Groups[0].Value)
				.Distinct()
				.ToList();
			for(int i=listStringMatches.Count-1;i>=0;i--) {
				if(listStringMatches[i].StartsWith("(") && listStringMatches[i].EndsWith(")")) {
					listStringMatches[i]=listStringMatches[i].Substring(1,listStringMatches[i].Length-2);
				}
				Regex rgx=new Regex(@"[\\]{1}");
				if(rgx.IsMatch(listStringMatches[i])) {
					listStringMatches.RemoveAt(i);
				}
			}
			return listStringMatches;
		}

		public static List<long> GetPatNumsFromText(string text) {
			//If this Regex pattern is ever changed, we may need to change the Select statement below.
			string strPatNum="patnum:";
			List<long> listNumMatches=Regex.Matches(text,$@"{strPatNum}\d+",RegexOptions.IgnoreCase)
				.OfType<Match>()
				.Select(x => PIn.Long(x.Groups[0].Value.Substring(strPatNum.Length),false))//Get pat num out of text.
				.Distinct()
				.ToList();
			return listNumMatches;
		}

		public static List<long> GetTaskNumsFromText(string text) {
			//If this Regex pattern is ever changed, we may need to change the Select statement below.
			string strTaskNum="tasknum:";
			List<long> listNumMatches=Regex.Matches(text,$@"{strTaskNum}\d+",RegexOptions.IgnoreCase)
				.OfType<Match>()
				.Select(x => PIn.Long(x.Groups[0].Value.Substring(strTaskNum.Length),false))//Get task num out of text.
				.Distinct()
				.ToList();
			return listNumMatches;
		}

		#endregion Methods - Public
	}
}
