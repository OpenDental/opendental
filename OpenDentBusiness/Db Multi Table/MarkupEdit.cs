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
using OpenDentBusiness.FileIO;

namespace OpenDentBusiness {
	public class MarkupEdit {
		///<summary>Used for editing wiki and HTML emails</summary>
		private static string _lanThis="MarkupEdit";
		private const string _odWikiImage=@"\[\[(img:).+?\]\]";
		private const string _odWikiKeyword=@"\[\[(keywords:).*?\]\]";
		private const string _odWikiFile=@"\[\[(file:).*?\]\]";
		private const string _odWikiFolder=@"\[\[(folder:).*?\]\]";
		private const string _odWikiFilecloud=@"\[\[(filecloud:).*?\]\]";
		private const string _odWikiFoldercloud=@"\[\[(foldercloud:).*?\]\]";
		private const string _odWikiColor=@"\[\[(color:).*?\]\]";
		private const string _odWikiFont=@"\[\[(font:).*?\]\]";
		private const string _odWikiTable=@"\{\|\n.+?\n\|\}";
		private const string _htmlTag="(?<!&)<.+?(?<!&)>";

		///<summary>Recursive.</summary>
		public static void ValidateNodes(XmlNodeList nodes) {
			foreach(XmlNode node in nodes) {
				if(node.NodeType==XmlNodeType.Comment) {
					throw new ApplicationException("The comment tag <!-- --> "+node.Name+" is not allowed.");
				}
				if(node.NodeType==XmlNodeType.ProcessingInstruction) {
					throw new ApplicationException("The XML processing instruction <?xml ?> "+node.Name+" is not allowed.");
				}
				if(node.NodeType==XmlNodeType.XmlDeclaration) {
					throw new ApplicationException("XML declarations like <?xml ?> "+node.Name+"> are not allowed.");
				}
				if(node.NodeType!=XmlNodeType.Element){
					continue;
				}
				//check child nodes for nested duplicate
				switch(node.Name) {
					case "i":
					case "b":
					case "h1":
					case "h2":
					case "h3":
						//These are all valid nodes that are allowed.
						break;
					case "div":
						//The only thing div is used for right now is to designate bookmarks within the page.
						//Therefore we require that there be one and only one attribute and that attribute can only be "id".
						if(node.Attributes.Count!=1 || node.Attributes[0].Name!="id") {
							throw new ApplicationException(Lans.g(_lanThis,"All <div> tags MUST be identified by the 'id' attribute."));
						}
						break;
					case "a":
						//a is an allowed node but can only have one attribute; href
						for(int i=0;i<node.Attributes.Count;i++) {
							if(node.Attributes[i].Name!="href") {
								throw new ApplicationException(node.Attributes[i].Name+" attribute is not allowed on <a> tag.");
							}
							//We know the only attribute is "href", make sure the user didn't manually type out a "wiki" link using <a>.  They need to use [[ ]].
							if(node.Attributes[i].InnerText.StartsWith("wiki:")) {
								throw new ApplicationException("wiki: is not allowed in an <a> tag.  Use [[ ]] instead of <a>.");
							}
						}
						break;
					case "img":
						throw new ApplicationException("Image tags are not allowed. Instead use [[img: ... ]]");
					default:
						throw new ApplicationException("<"+node.Name+"> is not one of the allowed tags. To display as plain text, escape the brackets with ampersands. I.e. \"&<"+node.Name+"&>\"");
				}
				ValidateNodes(node.ChildNodes);
				ValidateDuplicateNesting(node.Name,node.ChildNodes);
			}
		}

		///<summary>Recursive.</summary>
		public static void ValidateDuplicateNesting(string nodeName,XmlNodeList nodes) {
			foreach(XmlNode node in nodes) {
				if(node.NodeType!=XmlNodeType.Element) {
					continue;
				}
				if(nodeName==node.Name) {
					throw new ApplicationException("There are multiple <"+node.Name+"> tags nested within each other.  Remove the unneeded tags.");
				}
				ValidateDuplicateNesting(nodeName,node.ChildNodes);
			}
		}

		///<summary>Returns all of the names of the images in the markup.</summary>
		public static List<string> GetImageNames(string markup) {		
			//[[img:myimage.jpg]]------------------------------------------------------------------------------------------------------------
			MatchCollection matches=Regex.Matches(markup,_odWikiImage);
			List<string> retVal=new List<string>();
			foreach(Match match in matches) {
				retVal.Add(match.Value.Substring(match.Value.IndexOf(":") + 1).TrimEnd("]".ToCharArray()));
			}
			return retVal;
		}

		///<summary>Converts an image markup tag like [[img:myimage.jpeg]] to html.</summary>
		private static string TranslateEmailImages(string s) {
			List<string> listImageNames=GetImageNames(s);
			foreach(string imgName in listImageNames) {
				string fullPath;
				if(MiscUtils.IsValidHttpUri(imgName)) {
					fullPath=imgName;
				}
				else {
					//Not a url. An A-Z image.
					string imagePath="";
					try {
						imagePath=ImageStore.GetEmailImagePath();
					}
					catch(Exception ex) {
						ex.DoNothing();
						throw;
					}
					fullPath=FileAtoZ.CombinePaths(imagePath,POut.String(imgName));
					if(CloudStorage.IsCloudStorage) {				
						//WebBrowser needs to have a local file to open, so we download the images to temp files.	
						OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(Path.GetDirectoryName(fullPath),Path.GetFileName(fullPath));
						string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(fullPath));
						File.WriteAllBytes(tempFile,state.FileContent);
						fullPath=tempFile;
					}
				}
				s=s.Replace($"[[img:{imgName}]]","<img src=\""+fullPath+"\"></img>");//"\" />");
			}
			return s;
		}
		
		///<summary>Determines if the given text contains any OD Markup tags that would be replaced with HTML tags in an email.</summary>
		public static bool ContainsOdMarkupForEmail(string text) {
			List<string> listOdWikiRegex=new List<string> {
				_odWikiImage,
				_odWikiColor,
				_odWikiFont,
				_odWikiTable,
			};
			//[[odmarkup]] or OD ordered/unorderd lists
			return listOdWikiRegex.Select(x => Regex.Matches(text,x,RegexOptions.Singleline)).Any(x => x.Count>0)
				|| ProcessList(text,"#")!=text || ProcessList(text,"*")!=text;
		}

		///<summary>Surround with try/catch.  Also aggregates the content into the master page (unless specified to not).  
		///If isPreviewOnly, then the internal links will not be checked to see if the page exists, as it would make the refresh sluggish.  
		///And isPreviewOnly also changes the pointer so that the page looks non-clickable.
		///For emails, this only gets called while in the email edit window. The returned string will be used to switch between plain and html text.
		///</summary>
		public static string TranslateToXhtml(string markupText,bool isPreviewOnly,bool hasWikiPageTitles=false,bool isEmail=false,bool canAggregate=true,
			float scale=1) {
			//No need to check RemotingRole; no call to db.
			#region Basic Xml Validation
			string s=markupText;
			MatchCollection matches;
			//"<",">", and "&"-----------------------------------------------------------------------------------------------------------
			s=s.Replace("&","&amp;");
			s=s.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
			s=s.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
			s="<body>"+s+"</body>";
			XmlDocument doc=new XmlDocument();
			using(StringReader reader=new StringReader(s)) {
				doc.Load(reader);
			}
			#endregion
			#region regex replacements
			if(isEmail) {
				s=TranslateEmailImages(s);//handle email images and wiki images separately.
			}
			else {
				//[[img:myimage.gif]]------------------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiImage);
				foreach(Match match in matches) {
					string imgName = match.Value.Substring(match.Value.IndexOf(":")+1).TrimEnd("]".ToCharArray());
					string wikiPath="";
					try {
						wikiPath=WikiPages.GetWikiPath();
					}
					catch(Exception ex) {
						ex.DoNothing();
						throw;
					}
					string fullPath=FileAtoZ.CombinePaths(wikiPath,POut.String(imgName));
					if(CloudStorage.IsCloudStorage) {				
						//WebBrowser needs to have a local file to open, so we download the images to temp files.	
						OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(Path.GetDirectoryName(fullPath),Path.GetFileName(fullPath));
						string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(fullPath));
						File.WriteAllBytes(tempFile,state.FileContent);
						fullPath=tempFile;
					}
					s=s.Replace(match.Value,"<img src=\"file:///"+fullPath.Replace("\\","/")+"\"></img>");
				}
				//[[keywords: key1, key2, etc.]]------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiKeyword);
				foreach(Match match in matches) {//should be only one
					s=s.Replace(match.Value,"<span class=\"keywords\">keywords:"+match.Value.Substring(11).TrimEnd("]".ToCharArray())+"</span>");
				}
				//[[file:C:\eaula.txt]]------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiFile);
				foreach(Match match in matches) {
					string fileName=match.Value.Replace("[[file:","").TrimEnd(']');
					s=s.Replace(match.Value,"<a href=\"wikifile:"+fileName+"\">file:"+fileName+"</a>");
				}
				//[[folder:\\serverfiles\storage\]]------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiFolder);
				foreach(Match match in matches) {
					string folderName=match.Value.Replace("[[folder:","").TrimEnd(']');
					s=s.Replace(match.Value,"<a href=\"folder:"+folderName+"\">folder:"+folderName+"</a>");
				}
				//[[filecloud:AtoZ/SheetImages/happyclown.jpg]]------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiFilecloud);
				foreach(Match match in matches) {
					string fileName=CloudStorage.PathTidy(match.Value.Replace("[[filecloud:","").TrimEnd(']'));
					s=s.Replace(match.Value,"<a href=\"wikifilecloud:"+fileName+"\">filecloud:"+fileName+"</a>");
				}
				//[[foldercloud:AtoZ/PenguinPictures/]]------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,_odWikiFoldercloud);
				foreach(Match match in matches) {
					string folderName=CloudStorage.PathTidy(match.Value.Replace("[[foldercloud:","").TrimEnd(']'));
					s=s.Replace(match.Value,"<a href=\"foldercloud:"+folderName+"\">foldercloud:"+folderName+"</a>");
				}
			}
			//Color and text are for both wiki and email. It's important we do this before Internal Link or else the translation may not work. 
			//[[color:red|text]]----------------------------------------------------------------------------------------------------------------
			matches = Regex.Matches(s,_odWikiColor,RegexOptions.Singleline);//.*? matches as few as possible.
			foreach(Match match in matches) {
				//string[] paragraphs = match.Value.Split(new string[] { "\n" },StringSplitOptions.None);
				string tempText="<span style=\"color:";
				string[] tokens = match.Value.Split('|');
				if(tokens.Length<2) {//not enough tokens
					continue;
				}
				if(tokens[0].Split(':').Length!=2) {//Must have a color token and a color value seperated by a colon, no more no less.
					continue;
				}
				for(int i=0;i<tokens.Length;i++) {
					if(i==0) {
						tempText+=tokens[0].Split(':')[1]+";\">";//close <span> tag
						continue;
					}
					tempText+=(i>1?"|":"")+tokens[i];
				}
				tempText=tempText.TrimEnd(']');
				tempText+="</span>";
				s=s.Replace(match.Value,tempText);
			}
			//[[font-family:courier|text]]----------------------------------------------------------------------------------------------------------------
			matches = Regex.Matches(s,_odWikiFont,RegexOptions.Singleline);//.*? matches as few as possible.
			foreach(Match match in matches) {
				//string[] paragraphs = match.Value.Split(new string[] { "\n" },StringSplitOptions.None);
				string tempText="<span style=\"font-family:";
				string[] tokens = match.Value.Split('|');
				if(tokens.Length<2) {//not enough tokens
					continue;
				}
				if(tokens[0].Split(':').Length!=2) {//Must have a color token and a color value seperated by a colon, no more no less.
					continue;
				}
				for(int i=0;i<tokens.Length;i++) {
					if(i==0) {
						tempText+=tokens[0].Split(':')[1]+";\">";//close <span> tag
						continue;
					}
					tempText+=(i>1?"|":"")+tokens[i];
				}
				tempText=tempText.TrimEnd(']');
				tempText+="</span>";
				s=s.Replace(match.Value,tempText);
			}
			if(!isEmail) {
				//[[InternalLink]]--------------------------------------------------------------------------------------------------------------
				matches=Regex.Matches(s,@"\[\[.+?\]\]");
				List<string> pageNamesToCheck=new List<string>();
				List<bool> pageNamesExist=new List<bool>();
				string styleNotExists="";
				if(hasWikiPageTitles) {
					if(!isPreviewOnly) {
						foreach(Match match in matches) {
							//The '&' was replaced with '&amp;' above, so we change it back before looking for a wiki page with that name.
							pageNamesToCheck.Add(match.Value.Trim('[',']').Replace("&amp;","&"));  
						}
						if(pageNamesToCheck.Count>0) {
							pageNamesExist=WikiPages.CheckPageNamesExist(pageNamesToCheck);//this gets a list of bools for all pagenames in one shot.  One query.
						}
					}
					foreach(Match match in matches) {
						styleNotExists="";
						if(!isPreviewOnly) {
							//The '&' was replaced with '&amp;' above, so we change it back before looking for a wiki page with that name.
							string pageName=match.Value.Trim('[',']').Replace("&amp;","&");  
							int idx=pageNamesToCheck.IndexOf(pageName);
							if(!pageNamesExist[idx]) {
								styleNotExists="class='PageNotExists' ";
							}
						}
						s=s.Replace(match.Value,"<a "+styleNotExists+"href=\""+"wiki:"+match.Value.Trim('[',']')/*.Replace(" ","_")*/
							+"\">"+match.Value.Trim('[',']')+"</a>");
					}
				}
				else {
					List<long> listWikiPageNums=WikiPages.GetWikiPageNumsFromPageContent(s);
					List<WikiPage> listWikiPages=WikiPages.GetWikiPages(listWikiPageNums);
					int numInvalid=1;
					foreach(Match match in matches) {
						WikiPage wp=listWikiPages.FirstOrDefault(x => x.WikiPageNum==PIn.Long(match.Value.TrimStart('[').TrimEnd(']')));
						string pageName;
						if(wp!=null) {
							pageName=wp.PageTitle;
						}
						else {
							pageName="INVALID WIKIPAGE LINK "+numInvalid++;
						}
						if(!isPreviewOnly) {
							styleNotExists="";
							if(wp==null) {
								styleNotExists="class='PageNotExists' ";
							}
						}
						pageName=pageName.Replace("&","&amp;").Replace("&amp;<","&lt;").Replace("&amp;>","&gt;");
						string replace="<a "+styleNotExists+"href=\""+"wiki:"+pageName/*.Replace(" ","_")*/+"\">"+pageName+"</a>";
						Regex regex=new Regex(Regex.Escape(match.Value));
						//Replace the first instance of the match with the wiki page name (or unknown if not found).
						s=regex.Replace(s, replace, 1);
					}
				}
			}
			//Unordered List----------------------------------------------------------------------------------------------------------------
			//Instead of using a regex, this will hunt through the rows in sequence.
			//later nesting by running ***, then **, then *
			s=ProcessList(s,"*");
			//numbered list---------------------------------------------------------------------------------------------------------------------
			s=ProcessList(s,"#");
			//table-------------------------------------------------------------------------------------------------------------------------
			//{|
			//!Width="100"|Column Heading 1!!Width="150"|Column Heading 2!!Width=""|Column Heading 3
			//|- 
			//|Cell 1||Cell 2||Cell 3 
			//|-
			//|Cell A||Cell B||Cell C 
			//|}
			//There are many ways to parse this.  Our strategy is to do it in a way that the generated xml is never invalid.
			//As the user types, the above example will frequently be in a state of partial completeness, and the parsing should gracefully continue anyway.
			//rigorous enforcement only happens when validating during a save, not here.
			matches = Regex.Matches(s,_odWikiTable,RegexOptions.Singleline);
			foreach(Match match in matches) {
				//If there isn't a new line before the start of the table markup or after the end, the match group value will be an empty string
				//Tables must start with "'newline'{|" and end with "|}'newline'"
				string tableStrOrig=match.Value;
				StringBuilder strbTable=new StringBuilder();
				string[] lines=tableStrOrig.Split(new string[] { "{|\n","\n|-\n","\n|}" },StringSplitOptions.RemoveEmptyEntries);
				strbTable.AppendLine("<table>");
				List<string> colWidths=new List<string>();
				for(int i=0;i<lines.Length;i++) {
					if(lines[i].StartsWith("!")) {//header
						strbTable.AppendLine("<tr>");
						lines[i]=lines[i].Substring(1);//strips off the leading !
						string[] cells=lines[i].Split(new string[] {"!!"},StringSplitOptions.None);
						colWidths.Clear();
						for(int c=0;c<cells.Length;c++){
							if(Regex.IsMatch(cells[c],@"(Width="")\d+""\|")){//e.g. Width="90"|
								strbTable.Append("<th ");
								string width=cells[c].Substring(7);//90"|Column Heading 1
								width=width.Substring(0,width.IndexOf("\""));//90
								colWidths.Add(width);
								strbTable.Append("Width=\""+width+"\">");
								strbTable.Append(ProcessParagraph(cells[c].Substring(cells[c].IndexOf("|")+1),false));//surround with p tags. Allow CR in header.
								strbTable.AppendLine("</th>");
							}
							else {
								strbTable.Append("<th>");
								strbTable.Append(ProcessParagraph(cells[c],false));//surround with p tags. Allow CR in header.
								strbTable.AppendLine("</th>");
							}
						}
						strbTable.AppendLine("</tr>");
					}
					else if(lines[i].Trim()=="|-"){
						//totally ignore these rows
					}
					else{//normal row
						strbTable.AppendLine("<tr>");
						lines[i]=lines[i].Substring(1);//strips off the leading |
						string[] cells=lines[i].Split(new string[] {"||"},StringSplitOptions.None);
						for(int c=0;c<cells.Length;c++) {
							strbTable.Append("<td Width=\""+colWidths[c]+"\">");
							strbTable.Append(ProcessParagraph(cells[c],false));
							strbTable.AppendLine("</td>");
						}
						strbTable.AppendLine("</tr>");
					}
				}
				strbTable.Append("</table>");
				s=s.Replace(tableStrOrig,strbTable.ToString());
			}
			#endregion regex replacements
			#region paragraph grouping
			StringBuilder strbSnew=new StringBuilder();
			//a paragraph is defined as all text between sibling tags, even if just a \n.
			int iScanInParagraph=0;//scan starting at the beginning of s.  S gets chopped from the start each time we grab a paragraph or a sibiling element.
			//The scanning position represents the verified paragraph content, and does not advance beyond that.
			//move <body> tag over.
			strbSnew.Append("<body>");
			s=s.Substring(6);
			bool startsWithCR=false;//todo: handle one leading CR if there is no text preceding it.
			if(s.StartsWith("\n")) {
				startsWithCR=true;
			}
			string tagName; 
			Match tagCurMatch;
			while(true) {//loop to either construct a paragraph, or to immediately add the next tag to strbSnew.
				iScanInParagraph=s.IndexOf("<",iScanInParagraph);//Advance the scanner to the start of the next tag
				if(iScanInParagraph==-1) {//there aren't any more tags, so current paragraph goes to end of string.  This won't happen
					throw new ApplicationException(Lans.g("WikiPages","No tags found."));
					//strbSnew.Append(ProcessParagraph(s));
				}
				if(s.Substring(iScanInParagraph).StartsWith("</body>")) {
					strbSnew.Append(ProcessParagraph(s.Substring(0,iScanInParagraph),startsWithCR));
					//startsWithCR=false;
					//strbSnew.Append("</body>");
					s="";
					iScanInParagraph=0;
					break;
				}
				tagName="";
				tagCurMatch=Regex.Match(s.Substring(iScanInParagraph),"^<.*?>");//regMatch);//.*? means any char, zero or more, as few as possible
				if(tagCurMatch==null) {
					//shouldn't happen unless closing bracket is missing
					throw new ApplicationException(Lans.g("WikiPages","Unexpected tag:")+" "+s.Substring(iScanInParagraph));
				}
				if(tagCurMatch.Value.Trim('<','>').EndsWith("/")) {
					//self terminating tags NOT are allowed
					//this should catch all non-allowed self-terminating tags i.e. <br />, <inherits />, etc...
					throw new ApplicationException(Lans.g("WikiPages","All elements must have a beginning and ending tag. Unexpected tag:")+" "+s.Substring(iScanInParagraph));
				}
				//Nesting of identical tags causes problems: 
				//<h1><h1>some text</h1></h1>
				//The first <h1> will match with the first </h1>.
				//We don't have time to support this outlier, so we will catch it in the validator when they save.
				//One possible strategy here might be:
				//idxNestedDuplicate=s.IndexOf("<"+tagName+">");
				//if(idxNestedDuplicate<s.IndexOf("</"+tagName+">"){
				//
				//}
				//Another possible strategy might be to use regular expressions.
				tagName=tagCurMatch.Value.Split(new string[] { "<"," ",">" },StringSplitOptions.RemoveEmptyEntries)[0];//works with tags like <i>, <span ...>, and <img .../>
				if(s.IndexOf("</"+tagName+">")==-1) {//this will happen if no ending tag.
					throw new ApplicationException(Lans.g("WikiPages","No ending tag:")+" "+s.Substring(iScanInParagraph));
				}
				switch(tagName){
					case "a":
					case "b":
					case "div":
					case "i": 
					case "span":
						iScanInParagraph=s.IndexOf("</"+tagName+">",iScanInParagraph)+3+tagName.Length;
						continue;//continues scanning this paragraph.
					case "h1": 
					case "h2": 
					case "h3": 
					case "ol": 
					case "ul": 
					case "table":
					case "img"://can NOT be self-terminating
						if(iScanInParagraph==0) {//s starts with a non-paragraph tag, so there is no partially assembled paragraph to process.
							//do nothing
						}
						else {//we are already part way into assembling a paragraph.  
							strbSnew.Append(ProcessParagraph(s.Substring(0,iScanInParagraph),startsWithCR));
							startsWithCR=false;//subsequent paragraphs will not need this
							s=s.Substring(iScanInParagraph);//chop off start of s
							iScanInParagraph=0;
						}
						//scan to the end of this element
						int iScanSibling=s.IndexOf("</"+tagName+">")+3+tagName.Length;
						//tags without a closing tag were caught above.
						//move the non-paragraph content over to s new.
						strbSnew.Append(s.Substring(0,iScanSibling));
						s=s.Substring(iScanSibling);
						//scanning will start a totally new paragraph
						break;
					default:
						if(isEmail) {
							iScanInParagraph=s.IndexOf("</"+tagName+">",iScanInParagraph)+3+tagName.Length;
							continue;//continues scanning this paragraph
						}
						throw new ApplicationException(Lans.g("WikiPages","Unexpected tag:")+" "+s.Substring(iScanInParagraph));
				}
			}
			strbSnew.Append("</body>");
			#endregion
			#region aggregation
			doc=new XmlDocument();
			using(StringReader reader=new StringReader(strbSnew.ToString())) {
				doc.Load(reader);
			}
			StringBuilder strbOut=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="\t";
			settings.OmitXmlDeclaration=true;
			settings.NewLineChars="\n";
			using(XmlWriter writer=XmlWriter.Create(strbOut,settings)) {
				doc.WriteTo(writer);
			}
			//spaces can't be handled prior to this point because &nbsp; crashes the xml parser.
			strbOut.Replace("  ","&nbsp;&nbsp;");//handle extra spaces. 
			strbOut.Replace("<td></td>","<td>&nbsp;</td>");//force blank table cells to show not collapsed
			strbOut.Replace("<th></th>","<th>&nbsp;</th>");//and blank table headers
			strbOut.Replace("{{nbsp}}","&nbsp;");//couldn't add the &nbsp; earlier because 
			strbOut.Replace("<p></p>","<p>&nbsp;</p>");//probably redundant but harmless
			//aggregate with master
			if(isEmail) {
				if(canAggregate) {
					s=PrefC.GetString(PrefName.EmailMasterTemplate).Replace("@@@body@@@",strbOut.ToString());
					return s;
				}
				return strbOut.ToString();
			}
			else {
				s=WikiPages.MasterPage.PageContent.Replace("@@@body@@@",strbOut.ToString());
			}
			#endregion aggregation
			#region scaling
			if(scale==1) {
				//Do nothing
			}
			else {
				//Adjust the font size and table column widths of the wiki to account for any "Zoom" changes.
				string fontTextRegexPattern=@"font-size:\s*\d+\.?\d?pt";//To find each font size text
				string fontNumRegexPattern=@"\d+\.?\d?";//To find only the font size itself
				MatchCollection matchCollection=Regex.Matches(s,fontTextRegexPattern);
				for(int i=matchCollection.Count-1;i>=0;i--) {//Walk through pageContent backwards to correctly rebuild the string 
					Match matchFontNumOnly=Regex.Match(matchCollection[i].Value,fontNumRegexPattern);//Find the font value itself
					string[] arrayFontText=Regex.Split(matchCollection[i].Value,fontNumRegexPattern);//Separate the other text around the font value
					string fontNumUpdate=Convert.ToString(Math.Round(scale*PIn.Float(matchFontNumOnly.Value),1));//Adjust the font value to the nearest tenth
					string fontTextUpdate=arrayFontText[0]+fontNumUpdate+arrayFontText[1];//Rebuild the font text with the updated font value
					s=s.Substring(0,matchCollection[i].Index)+fontTextUpdate+s.Substring(matchCollection[i].Index+matchCollection[i].Length);//Rebuild pageContent css
				}
				string colTextRegexPattern=@"t(h|d)\sWidth=""\d+""";//To find each col size text
				string colNumRegexPattern=@"\d+";//To find only the col width itself
				matchCollection=Regex.Matches(s,colTextRegexPattern);
				for(int i=matchCollection.Count-1;i>=0;i--) {//Walk through pageContent backwards to correctly rebuild the string 
					Match matchColNumOnly=Regex.Match(matchCollection[i].Value,colNumRegexPattern);//Find the col value itself
					string[] arrayColText=Regex.Split(matchCollection[i].Value,colNumRegexPattern);//Separate the other text around the col value
					string colNumUpdate=Convert.ToString(Math.Round(scale*PIn.Float(matchColNumOnly.Value),1));//Adjust the col value to the nearest tenth
					string colTextUpdate=arrayColText[0]+colNumUpdate+arrayColText[1];//Rebuild the col text with the updated col value
					s=s.Substring(0,matchCollection[i].Index)+colTextUpdate+s.Substring(matchCollection[i].Index+matchCollection[i].Length);//Rebuild pageContent body
				}
			}
			#endregion scaling
			/*
			//js This code is buggy.  It will need very detailed comments and careful review before/if we ever turn it back on.
			if(isPreviewOnly) {
				//do not change cursor from pointer to IBeam to Hand as you move the cursor around the preview page
				s=s.Replace("*{\n\t","*{\n\tcursor:default;\n\t");
				//do not underline links if you hover over them in the preview window
				s=s.Replace("a:hover{\n\ttext-decoration:underline;","a:hover{\n\t");
			}*/
			return s;
		}

		/// <summary>Uses regex to remove the formatting around OD Markup strings and HTML tags</summary>
		public static string ConvertMarkupToPlainText(string rawText) {
			StringBuilder strb=new StringBuilder(rawText);
			//strip image
			StringTools.RegReplace(strb,@"\[\[img:(?=[^\[\]]*?\]\])|(?<=\[\[img:[^\[\]]*?)\]\]","");
			//strip font
			StringTools.RegReplace(strb,@"\[\[font:[^\[\]]*?\|(?=[^\[\]]*?\]\])|(?<=\[\[font:[^\[\]]*?\|[^\]]*?)\]\]","");
			//strip color
			StringTools.RegReplace(strb,@"\[\[color:[^\[\]]*?\|(?=[^\[\]]*?\]\])|(?<=\[\[color:[^\[\]]*?\|[^\]]*?)\]\]","");
			//strip table
			//Remove headers
			StringTools.RegReplace(strb,@"(?<={\|.*?)!.*?\|(?=.*?\|})"," ",RegexOptions.Singleline);
			//Remove row start
			StringTools.RegReplace(strb,@"(?<={\|.*?)\n\|-(?=.*?\|})|(?<={\|.*?\n)\|(?=.*?\|})"," ",RegexOptions.Singleline);
			//Remove row innards
			StringTools.RegReplace(strb,@"(?<={\|.*?)\|\|(?=.*?\|})"," ",RegexOptions.Singleline);
			//Remove table beginning / end
			StringTools.RegReplace(strb,@"{\|(?=.*?\|})|(?<={\|.*?)\|}","",RegexOptions.Singleline);
			//The regex pattern below will match anything enclosed within "<" and ">". However for our wiki pages, we use "&" as an escape character
			//for "<" and ">", so we do not want to match "&<" or "&>". We know that this will not perfectly parse all HTML tags, but it is good enough
			//to use for searching.
			StringTools.RegReplace(strb,_htmlTag,"");
			return strb.ToString();
		}

		///<summary>This method removes HTML tags and wiki page links from the wiki page text.</summary>
		public static string ConvertToPlainText(string rawWikipageText) {
			StringBuilder strb=new StringBuilder(rawWikipageText);
			//The regex pattern below will match anything enclosed within "<" and ">". However for our wiki pages, we use "&" as an escape character
			//for "<" and ">", so we do not want to match "&<" or "&>". We know that this will not perfectly parse all HTML tags, but it is good enough
			//to use for searching.
			StringTools.RegReplace(strb,_htmlTag,"");
			//We also want to remove links to other wiki pages.
			StringTools.RegReplace(strb,@"\[\[.+?\]\]","");
			return strb.ToString();
		}

		///<summary>This will get called repeatedly.  prefixChars is, for now, * or #.  Returns the altered text of the full document.</summary>
		private static string ProcessList(string s,string prefixChars){
			string[] lines=s.Split(new string[] { "\n" },StringSplitOptions.None);//includes empty elements
			string blockOriginal=null;//once a list is found, this string will be filled with the original text.
			StringBuilder strb=null;//this will contain the final output enclosed in <ul> or <ol> tags.
			for(int i=0;i<lines.Length;i++) {
				if(blockOriginal==null) {//we are still hunting for the first line of a list.
					if(lines[i].StartsWith(prefixChars) || lines[i].StartsWith("<body>"+prefixChars)) {//we found the first line of a list.
						blockOriginal=lines[i];
						if(!lines[i].EndsWith("</body>")) {//not the last line of the document, a list item can be on the last line, if it is, do not add the \n
							blockOriginal+="\n";
						}
						strb=new StringBuilder();
						if(lines[i].StartsWith("<body>")) {
							strb.Append("<body>");
							lines[i]=lines[i].Substring(6);//strip off body opening tag
						}
						if(prefixChars.Contains("*")) {
							strb.Append("<ul>\n");
						}
						else if(prefixChars.Contains("#")) {
							strb.Append("<ol>\n");
						}
						lines[i]=lines[i].Substring(prefixChars.Length);//strip off the prefixChars
						strb.Append("<li><span class='ListItemContent'>");
						//lines[i]=lines[i].Replace("  ","[[nbsp]][[nbsp]]");//handle extra spaces.  We may move this to someplace more global
						if(lines[i].EndsWith("</body>")) {
							strb.Append(lines[i].Substring(0,lines[i].Length-7));
						}
						else {
							strb.Append(lines[i]);
						}
						strb.Append("</span></li>\n");
						if(lines[i].EndsWith("</body>")) {//ends with body tag, so last line is a list item, close the body and replace in original string
							if(prefixChars.Contains("*")) {
								strb.Append("</ul>");
							}
							else if(prefixChars.Contains("#")) {
								strb.Append("</ol>");
							}
							strb.Append("</body>");
							//manually replace just the first occurance of the identified list.
							s=s.Substring(0,s.IndexOf(blockOriginal))
							+strb.ToString()
							+s.Substring(s.IndexOf(blockOriginal)+blockOriginal.Length);
							blockOriginal=null;
						}
					}
					else {//no list
						//nothing to do
					}
				}
				else {//we are already building our list
					if(lines[i].StartsWith(prefixChars)) {//we found another line of a list.  Could be a middle line or the last line.
						blockOriginal+=lines[i];
						if(!lines[i].EndsWith("</body>")) {
							blockOriginal+="\n";
						}
						lines[i]=lines[i].Substring(prefixChars.Length);//strip off the prefixChars
						strb.Append("<li><span class='ListItemContent'>");
						//lines[i]=lines[i].Replace("  ","[[nbsp]][[nbsp]]");//handle extra spaces.  We may move this to someplace more global
						if(lines[i].EndsWith("</body>")) {
							strb.Append(lines[i].Substring(0,lines[i].Length-7));
						}
						else {
							strb.Append(lines[i]);
						}
						strb.Append("</span></li>\n");
						if(lines[i].EndsWith("</body>")) {
							if(prefixChars.Contains("*")) {
								strb.Append("</ul>\n");
							}
							else if(prefixChars.Contains("#")) {
								strb.Append("</ol>\n");
							}
							strb.Append("</body>");
							//manually replace just the first occurance of the identified list.
							s=s.Substring(0,s.IndexOf(blockOriginal))
							+strb.ToString()
							+s.Substring(s.IndexOf(blockOriginal)+blockOriginal.Length);
							blockOriginal=null;
						}
					}
					else {//end of list.  The previous line was the last line.
						if(prefixChars.Contains("*")) {
							strb.Append("</ul>\n");
						}
						else if(prefixChars.Contains("#")) {
							strb.Append("</ol>\n");
						}
						//manually replace just the first occurance of the identified list.
						s=s.Substring(0,s.IndexOf(blockOriginal))
							+strb.ToString()
							+s.Substring(s.IndexOf(blockOriginal)+blockOriginal.Length);
						//s=s.Replace(blockOriginal,strb.ToString()); //old strategy, buggy.
						blockOriginal=null;
					}
				}
			}
			return s;
		}

		///<summary>This will wrap the text in p tags as well as handle internal carriage returns.  startsWithCR is only used on the first paragraph 
		///for the unusual case where the entire content starts with a CR.  This prevents stripping it off.</summary>
		private static string ProcessParagraph(string paragraph,bool startsWithCR) {
			if(paragraph.StartsWith("\n") && !startsWithCR) {
				paragraph=paragraph.Substring(1);
			}
			if(paragraph=="") {//this must come after the first CR is stripped off, but before the ending CR is stripped off.
				return "";
			}
			if(paragraph.EndsWith("\n")) {//trailing CR remove
				paragraph=paragraph.Substring(0,paragraph.Length-1);
			}
			//if the paragraph starts with any number of spaces followed by a tag such as <b> or <span>, then we need to force those spaces to show.
			if(paragraph.StartsWith(" ") && paragraph.TrimStart(' ').StartsWith("<")) {
				paragraph="{{nbsp}}"+paragraph.Substring(1);//this will later be converted to &nbsp;
			}
			paragraph=paragraph.Replace("\n","<br/>");//We tried </p><p>, but that didn't allow bold, italic, or color to span lines.
			paragraph="<p>"+paragraph+"</p>";//surround paragraph with tags
			paragraph=paragraph.Replace("<p> ","<p>{{nbsp}}");//spaces at the beginnings of paragraphs
			paragraph=paragraph.Replace("<br/> ","<br/>{{nbsp}}");//spaces at beginnings of lines
			paragraph=paragraph.Replace("<br/></p>","<br/>{{nbsp}}</p>");//have a cr show if it's at the end of a paragraph
			return paragraph;
		}

	}

}
