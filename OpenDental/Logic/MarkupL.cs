using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Used for wiki and HTML email editing.</summary>
	public class MarkupL {
		private static string _lanThis="MarkupEdit";

		//remember to call focus outside of this method since it can no longer be called from the form here. 
		public static void AddTag(string tagStart,string tagClose,ODcodeBox codeBox) {
			int startSelection=codeBox.SelectionStart;
			int lengthSelection=codeBox.SelectionLength;
			string str=tagStart+codeBox.SelectedText+tagClose;
			codeBox.SelectedText=str;
			//textContentEmail.Focus();
			if(lengthSelection==0) {//nothing selected, place cursor in middle of new tags
				codeBox.SelectionStart=startSelection+tagStart.Length+lengthSelection;
				return;
			}
			codeBox.SelectionStart=startSelection+str.Length;
			codeBox.SelectionLength=0;
		}

		///<summary>Validates content, and keywords.  isForSaving can be false if just validating for refresh.</summary>
		public static bool ValidateMarkup(ODcodeBox codeBox,bool isForSaving,bool showMsgBox=true,bool isEmail=false) {
			MatchCollection matchCollection;
			//xml validation----------------------------------------------------------------------------------------------------
			string str=codeBox.Text;
			//"<",">", and "&"-----------------------------------------------------------------------------------------------------------
			str=str.Replace("&","&amp;");
			str=str.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
			str=str.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
			str="<body>"+str+"</body>";
			XmlDocument xmlDocument=new XmlDocument();
			StringReader stringReader=new StringReader(str);
			try {
				xmlDocument.Load(stringReader);
			}
			catch(Exception ex){
				if(showMsgBox) {
					MessageBox.Show(ex.Message);
				}
				return false;
			}
			if(!isEmail) {//We are allowing any XHTML markup in emails.
				try{
				//we do it this way to skip checking the main node itself since it's a dummy node.
					MarkupEdit.ValidateNodes(xmlDocument.DocumentElement.ChildNodes);
				}
				catch(Exception ex){
					if(showMsgBox) {
						MessageBox.Show(ex.Message);
					}
				return false;
				}
			}
			//Cannot have CR within tag definition---------------------------------------------------------------------------------
			//(?<!&) means only match strings that do not start with an '&'. This is so we can continue to use '&' as an escape character for '<'.
			//<.*?> means anything as short as possible that is contained inside a tag
			MatchCollection matchCollectionTags=Regex.Matches(codeBox.Text,"(?<!&)<.*?>",RegexOptions.Singleline);
			for(int i=0;i<matchCollectionTags.Count;i++) {
				if(matchCollectionTags[i].ToString().Contains("\n")) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollectionTags[i].Index)+" - "
							+Lans.g(_lanThis,"Tag definitions cannot contain a return line:")+" "+matchCollectionTags[i].Value.Replace("\n",""));
					}
					return false;
				}
			}
			//wiki image validation-----------------------------------------------------------------------------------------------------
			if(!isEmail) {
				string wikiImagePath="";
				try {
					wikiImagePath=WikiPages.GetWikiPath();//this also creates folder if it's missing.
				}
				catch(Exception ex) {
					ex.DoNothing();
					//do nothing, the wikiImagePath is only important if the user adds an image to the wiki page and is checked below
				}
				matchCollection=Regex.Matches(codeBox.Text,@"\[\[(img:).*?\]\]");// [[img:myimage.jpg]]
				if(matchCollection.Count>0 && PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[0].Index)+" - "
							+Lans.g(_lanThis,"Cannot use images in wiki if storing images in database."));
					}
					return false;
				}
				if(isForSaving) {
					for(int i=0;i<matchCollection.Count;i++) {
						string imgPath=FileAtoZ.CombinePaths(wikiImagePath,matchCollection[i].Value.Substring(6).Trim(']'));
						if(!FileAtoZ.Exists(imgPath)) {
							if(showMsgBox) {
								MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[i].Index)+" - "
									+Lans.g(_lanThis,"Not allowed to save because image does not exist:")+" "+imgPath);
							}
							return false;
						}
					}
				}
			}
			//Email image validation----------------------------------------------------------------------------------------------
			if(isEmail) {
				string emailImagePath="";
				try {
					emailImagePath=ImageStore.GetEmailImagePath();
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				matchCollection=Regex.Matches(codeBox.Text,@"\[\[(img:).*?\]\]");
				if(isForSaving) {
					for(int i = 0;i < matchCollection.Count;i++) {
						string imageName=matchCollection[i].Value.Substring(6).Trim(']');
						//Don't validate URL links.
						if(MiscUtils.IsValidHttpUri(imageName)) {
							continue;
						}
						string imgPath=FileAtoZ.CombinePaths(emailImagePath,imageName);
						if(!FileAtoZ.Exists(imgPath)) {
							if(showMsgBox) {
								MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[i].Index)+" - "
									+Lans.g(_lanThis,"Not allowed to save because image does not exist: ")+" "+imgPath);
							}
							return false;
						}
					}
				}
			}
			//List validation-----------------------------------------------------------------------------------------------------
			matchCollection=Regex.Matches(codeBox.Text,@"\[\[(list:).*?\]\]");// [[list:CustomList]]
			for(int m=0;m<matchCollection.Count;m++){
				if(!WikiLists.CheckExists(matchCollection[m].Value.Substring(7).Trim(']'))) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"Wiki list does not exist in database:")+" "+matchCollection[m].Value.Substring(7).Trim(']'));
					}
					return false;
				}
			}
			//spacing around bullets-----------------------------------------------------------------------------------------------
			string[] stringArrayLines=codeBox.Text.Split(new string[] { "\n" },StringSplitOptions.None);
			for(int i=0;i<stringArrayLines.Length;i++) {
				if(stringArrayLines[i].Trim().StartsWith("*")) {
					if(!stringArrayLines[i].StartsWith("*")) {
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+(i+1)+" - "
								+Lans.g(_lanThis,"Stars used for lists may not have a space before them."));
						}
						return false;
					}
					if(stringArrayLines[i].Trim().StartsWith("* ")) {
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+(i+1)+" - "
								+Lans.g(_lanThis,"Stars used for lists may not have a space after them."));
						}
						return false;
					}
				}
				if(stringArrayLines[i].Trim().StartsWith("#")) {
					if(!stringArrayLines[i].StartsWith("#")) {
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+(i+1)+" - "
								+Lans.g(_lanThis,"Hashes used for lists may not have a space before them."));
						}
						return false;
					}
					if(stringArrayLines[i].Trim().StartsWith("# ")) {
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+(i+1)+" - "
								+Lans.g(_lanThis,"Hashes used for lists may not have a space after them."));
						}
						return false;
					}
				}
			}
			//Invalid characters inside of various tags--------------------------------------------
			matchCollection=Regex.Matches(codeBox.Text,@"\[\[.*?\]\]");
			for(int m=0;m<matchCollection.Count;m++){
				if(matchCollection[m].Value.Contains("\"") 
					&& !matchCollection[m].Value.StartsWith("[[color:") 
					&& !matchCollection[m].Value.StartsWith("[[font:")) //allow colored text to have quotes.
				{
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"Link cannot contain double quotes:")+" "+matchCollection[m].Value);
					}
					return false;
				}
				//This is not needed because our regex doesn't even catch them if the span a line break.  It's just interpreted as plain text.
				//if(match.Value.Contains("\r") || match.Value.Contains("\n")) {
				//	MessageBox.Show(Lan.g(this,"Link cannot contain carriage returns: ")+match.Value);
				//	return false;
				//}
				if(matchCollection[m].Value.StartsWith("[[img:")
					|| matchCollection[m].Value.StartsWith("[[keywords:")
					|| matchCollection[m].Value.StartsWith("[[file:")
					|| matchCollection[m].Value.StartsWith("[[folder:")
					|| matchCollection[m].Value.StartsWith("[[list:")
					|| matchCollection[m].Value.StartsWith("[[color:")
 					|| matchCollection[m].Value.StartsWith("[[font:"))
				{
					//other tags
					continue;
				}
				if(matchCollection[m].Value.Contains("|")) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"Internal link cannot contain a pipe character:")+" "+matchCollection[m].Value);
					}
					return false;
				}
			}
			//Table markup rigorously formatted----------------------------------------------------------------------
			//{|
			//!Width="100"|Column Heading 1!!Width="150"|Column Heading 2!!Width="75"|Column Heading 3
			//|- 
			//|Cell 1||Cell 2||Cell 3 
			//|-
			//|Cell A||Cell B||Cell C 
			//|}
			//Although rarely needed, it might still come in handy in certain cases, like paste, or when user doesn't add the |} until later, and other hacks.
			matchCollection = Regex.Matches(str,@"\{\|\n.+?\n\|\}",RegexOptions.Singleline);
			//matches = Regex.Matches(textContent.Text,
			//	@"(?<=(?:\n|<body>))" //Checks for preceding newline or beggining of file
			//	+@"\{\|.+?\n\|\}" //Matches the table markup.
			//	+@"(?=(?:\n|</body>))" //Checks for following newline or end of file
			//	,RegexOptions.Singleline);
			for(int m=0;m<matchCollection.Count;m++){
				stringArrayLines=matchCollection[m].Value.Split(new string[] { "{|\n","\n|-\n","\n|}" },StringSplitOptions.RemoveEmptyEntries);
				if(!stringArrayLines[0].StartsWith("!")) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"The second line of a table markup section must start with ! to indicate column headers."));
					}
					return false;
				}
				if(stringArrayLines[0].StartsWith("! ")) {
					if(showMsgBox) {
						MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"In the table, at line 2, there cannot be a space after the first !"));
					}
					return false;
				}
				string[] stringArrayCells=stringArrayLines[0].Substring(1).Split(new string[] { "!!" },StringSplitOptions.None);//this also strips off the leading !
				for(int c=0;c<stringArrayCells.Length;c++) {
					if(!Regex.IsMatch(stringArrayCells[c],@"^(Width="")\d+""\|")) {//e.g. Width="90"| 
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Error at line:")+" "+codeBox.GetLineFromCharIndex(matchCollection[m].Index)+" - "
							+Lans.g(_lanThis,"In the table markup, each header must be formatted like this: Width=\"#\"|..."));
						}
						return false;
					}
				}
				for(int i=1;i<stringArrayLines.Length;i++) {//loop through the lines after the header
					if(!stringArrayLines[i].StartsWith("|")) {
						if(showMsgBox) {
							MessageBox.Show(Lans.g(_lanThis,"Table rows must start with |.  At line ")+(i+1).ToString()+Lans.g(_lanThis,", this was found instead:")
								+stringArrayLines[i]);
						}
						return false;
					}
				}
			}
			return true;  
		}
	}
}
