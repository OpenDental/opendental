using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CodeBase;
using OpenDentBusiness;

namespace WpfControls.UI {
	//=====WARNING! THERE IS A DUPLICATE OF THIS FILE OVER IN OpenDentBusiness.UI.PopupHelper.
	//=====UNTIL THAT FILE IS COMPLETELY DEPRECATED, BOTH FILES MUST BE KEPT IN SYNC.
	//=====ANY CHANGES TO ONE MUST ALSO BE MADE IN THE OTHER.
	///<summary>A helper class used add reference links to context menus</summary>
	public class PopupHelper2 {
		#region Methods - Public
		///<summary>For a given context menu item, returns a sorted list of menu item links. This supports wiki, patient, task, Job, URL, web, and file explorer.</summary>
		public static List<MenuItem> GetContextMenuItemLinks(string contextMenuItemText,bool rightClickLinks) {
			List<MenuItem> listMenuItemsLinks=new List<MenuItem>();
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
					string pageName=listStringMatches[i]; //To avoid lazy eval
					EventHandler eventHandler=(s,eArg) => { OpenWikiPage(pageName); };
					listMenuItemsLinks.Add(new MenuItem("Wiki - "+listStringMatches[i],eventHandler,tag:"autolink"));
				}
			}
			listStringMatches=GetURLsFromText(contextMenuItemText);
			for(int i=0;i<listStringMatches.Count;i++) {
				string title=listStringMatches[i];
				if(title.Length>24) {
					title=title.Substring(0,24)+"...";
				}
				string strMatch=listStringMatches[i]; //To avoid lazy eval
				EventHandler eventHandler=(s,eArg)=> { OpenWebPage(strMatch); };
				listMenuItemsLinks.Add(new MenuItem("Web - "+title,eventHandler,tag:"autolink"));
			}
			listStringMatches=ODFileUtils.GetFilePathsFromText(contextMenuItemText);
			for(int i=0;i<listStringMatches.Count;i++) {
				string strMatch=listStringMatches[i]; //To avoid lazy eval
				EventHandler eventHandler=(s,eArg) => { OpenUNCPath(strMatch); };
				listMenuItemsLinks.Add(new MenuItem("File Explorer - "+listStringMatches[i],eventHandler,tag:"autolink"));
			}
			if(rightClickLinks) {
				listNumMatches=GetPatNumsFromText(contextMenuItemText);
				for(int i=0;i<listNumMatches.Count;i++) {
					long patNum=listNumMatches[i];
					EventHandler eventHandler=(s,eArg) => { OpenPatNum(patNum); };
					listMenuItemsLinks.Add(new MenuItem("PatNum - "+listNumMatches[i],eventHandler,tag:"autolink"));
				}
				listNumMatches=GetTaskNumsFromText(contextMenuItemText);
				for(int i=0;i<listNumMatches.Count;i++) {
					long taskNum=listNumMatches[i];
					EventHandler eventHandler=(s,eArg) => { OpenTaskNum(taskNum); };
					listMenuItemsLinks.Add(new MenuItem("TaskNum - "+listNumMatches[i],eventHandler,tag:"autolink"));
				}
				if(PrefC.IsODHQ) {
					listNumMatches=GetJobNumsFromText(contextMenuItemText);
					for(int i = 0;i<listNumMatches.Count;i++) {
						long jobNum = listNumMatches[i];
						EventHandler eventHandler = (s,eArg) => { OpenJobNum(jobNum); };
						listMenuItemsLinks.Add(new MenuItem("JobNum - "+listNumMatches[i],eventHandler,tag:"autolink"));
					}
				}
			}
			listMenuItemsLinks=listMenuItemsLinks.OrderByDescending(x => x.Header.ToString()=="-").ThenBy(x => x.Header.ToString()).ToList();//alphabetize the link items.
			return listMenuItemsLinks;
		}

		///<summary>Returns a list of strings from the given text that are URLs.</summary>
		public static List<string> GetURLsFromText(string text) {
			//Regular expresion used to help identify URLs. This is not all encompassing.
			//There will be URLs that do not match this but this should work for 99%.
			//The url regex is generous enough to match urls fine and excludes emails well, but matches some files too.
			//These files get cleaned out though.
			string urlPattern=@"(?<!@)\b(?:https?:\/\/)?(?:www\.)?(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(?:\/[^\s]*)?\b(?!(?:\\))";
			List<string> listStringMatches=Regex.Matches(text,urlPattern)
				.OfType<Match>()
				.Select(m => m.Groups[0].Value)
				.Distinct()
				.ToList();
			for(int i=listStringMatches.Count-1;i>=0;i--) {
				if(listStringMatches[i].StartsWith("(") && listStringMatches[i].EndsWith(")")) {
					listStringMatches[i]=listStringMatches[i].Substring(1,listStringMatches[i].Length-2);
				}
				if(ODFileUtils.IsKnownFileType(listStringMatches[i])){
					listStringMatches.RemoveAt(i);
				}
				listStringMatches[i]=listStringMatches[i].TrimEnd('.');
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

		public static List<long> GetJobNumsFromText(string text) {
			//If this Regex pattern is ever changed, we may need to change the Select statement below.
			string strJobNum = "jobnum:";
			List<long> listNumMatches = Regex.Matches(text,$@"{strJobNum}\d+",RegexOptions.IgnoreCase)
				.OfType<Match>()
				.Select(x => PIn.Long(x.Groups[0].Value.Substring(strJobNum.Length),false))//Get Job num out of text.
				.Distinct()
				.ToList();
			return listNumMatches;
		}

		#endregion Methods - Public

		#region Methods - Private
		private static void OpenWikiPage(string pageTitle) {
			if(WikiPages.NavPageDelegate!=null) {
				WikiPages.NavPageDelegate.Invoke(pageTitle);
			}
		}

		private static void OpenPatNum(long patNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				OpenDental.MsgBox.Show(Lans.g("OpenDental","Patient does not exist."));
				return;
			}
			GlobalFormOpenDental.PatientSelected(pat,true);
		}

		private static void OpenTaskNum(long taskNum) {
			if(Tasks.NavTaskDelegate!=null) {
				Tasks.NavTaskDelegate.Invoke(taskNum);
			}
		}

		private static void OpenJobNum(long jobNum) {
			if(Jobs.NavJobDelegate!=null) {
				Jobs.NavJobDelegate.Invoke(jobNum);
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
				OpenDental.MsgBox.Show(Lans.g("PopupHelper","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again."),Lans.g("PopupHelper","Attention"));
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
					OpenDental.MsgBox.Show(e.Message);
				}
			}
			else {
				OpenDental.MsgBox.Show(Lans.g("PopupHelper","Failed to open file location. Please make sure file path is valid."));
			}
		}
		#endregion Methods - Private


	}
}
