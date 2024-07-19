using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
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
						byte[] byteArray=CloudStorage.Download(Path.GetDirectoryName(fullPath),Path.GetFileName(fullPath));
						string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(fullPath));
						File.WriteAllBytes(tempFile,byteArray);
						fullPath=tempFile;
					}
				}
				s=s.Replace($"[[img:{imgName}]]","<img src=\""+fullPath+"\"></img>");//"\" />");
			}
			return s;
		}

		///<summary>Determines if the given text contains any HTML tags, ex <a href="opendental.com">opendental.com</a> or any Od wiki syntax tags that
		///would result in HTML tags being added to the text when run through MarkupEdit.TranslateToXhtml()</summary>
		public static bool ContainsOdHtmlTags(string text) {
			Regex tagRegex=new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
			return tagRegex.IsMatch(text??"")||ContainsOdMarkupForEmail(text??"");
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
			//No need to check MiddleTierRole; no call to db.
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
			#region Validate brackets
			//Looks for replacement strings within markupText in order to find missing closing brackets. Ex: "[[color:... [[img:...]]", "[[file:...", "...[[".
			//This handles allowing nested bracketing implementation in the possible future. Ex: [[font-family:...[[color:...]] ...]]".
			int index=0;
			//Keep track of the start index for all replacement strings that are not 'closed' correctly. Ex: "abc[[def" will have '3' as the only value in the stack.
			Stack<int> stackUnclosedIndices=new Stack<int>();
			while(index+1 < markupText.Length) {
				string str=markupText[index].ToString()+markupText[index+1].ToString();
				if(str=="[[") {//If we find double-open brackets, we store the start location in case it is missing closing brackets.
					stackUnclosedIndices.Push(index);
					index+=2;//We can skip over the double-open brackets since they are not in concern anymore.
					continue;
				}
				else if(str=="]]") {//If we find double-close brackets, we remove the most recent location stored since they are a pair.
					if(stackUnclosedIndices.Count > 0) {
						stackUnclosedIndices.Pop();
					}
					index+=2;
					continue;
				}
				index+=1;
			}
			int end=markupText.Length;
			List<string> listInvalidMarkupStrings=new List<string>();
			while(stackUnclosedIndices.Count > 0) {
				int start=stackUnclosedIndices.Pop();
				int length=end-start;
				string invalidMarkupString=markupText.Substring(start,length);
				if(invalidMarkupString.Length > 50) {
					invalidMarkupString=invalidMarkupString.Substring(0,50)+"...";
				}
				listInvalidMarkupStrings.Add(invalidMarkupString);
				end=start;
			}
			index=0;
			string errorMessage="";
			for(int i=listInvalidMarkupStrings.Count-1;i>=0;i--) {//Display invalid replacement strings found chronologically.
				index+=1;
				errorMessage+="\r\n"+index+": \""+listInvalidMarkupStrings[i]+"\"";
			}
			if(!String.IsNullOrWhiteSpace(errorMessage)) {
				throw new ApplicationException(Lans.g("WikiPages","Invalid markup syntax detected. The following have unclosed brackets:")+errorMessage);
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
						byte[] byteArray=CloudStorage.Download(Path.GetDirectoryName(fullPath),Path.GetFileName(fullPath));
						string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(fullPath));
						File.WriteAllBytes(tempFile,byteArray);
						fullPath=tempFile;
					}
					//If our imported image has orientation changes, we want to make sure that gets reflected in the HTML render.
					Image image=FileAtoZ.GetImage(fullPath);
					PropertyItem propertyItem=image.PropertyItems.FirstOrDefault(x=>x.Id==0x0112);//Exif orientation PropertyTagOrientation
					long degreesRotated=0;
					long height=image.Height;
					long width=image.Width;
					long imageShiftPx=0;
					if(propertyItem!=null && propertyItem.Value.Length>0) {
						if(propertyItem.Value[0]==6 || propertyItem.Value[0]==8) {//Image should be rotated 90 or 270 degrees
							degreesRotated=90;
							if(propertyItem.Value[0]==8) {
								degreesRotated=270;
							}
							//Internet Explorer's CSS for transform:rotate will rotate at the center, so if it's a non-square image it has the potential to overlap other content.
							//Need to do the math to find the difference in the dimensions and then shift the image so it will be at it's original location.
							//Positive imageShiftPx means we are rotating a "long" image to a "wide" image, negative means "wide" image to a "long" image.
							//We halve this number because the new image will be centered in the old non-square dimensions.
							imageShiftPx=(height-width)/2;
						}
						//Images that are rotated 180 degrees don't need their height and width changed or shifted because the dimensions will be the same.
						else if(propertyItem.Value[0]==3) {
							degreesRotated=180;
						}
					}
					string imgStyle="";
					if(degreesRotated > 0) {
						imgStyle+=$"style=\"transform:rotate({degreesRotated}deg);position:relative;";
						//"long" to "wide" image needs to shift the image away from the top and right because the long portion is now extended above and away from the left edge of screen.
						//"wide" to "long" image needs to shift the image away from the bottom and left because the wide portion is now extended below and out of left side of screen.
						if(imageShiftPx > 0 ) {
							imgStyle+=$"top:{imageShiftPx}px;right:{imageShiftPx}px;";
						}
						else {
							imgStyle+=$"bottom:{imageShiftPx}px;left:{imageShiftPx}px;";
						}
						imgStyle+="\"";
					}
					//Need a parent container so a rotated image can have its new dimensions specified. Any potential HTML that comes after will not be overlapped.
					string newHtmlValue=$"<img src=\"file:///{fullPath.Replace("\\","/")}\" height=\"{height}px\" width=\"{width}px\" {imgStyle}></img>";
					s=s.Replace(match.Value,newHtmlValue);
					if(image!=null) {//Dispose
						image.Dispose();
					}
					image=null;
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
							strbTable.Append(ProcessParagraph(cells[c],true,false));
							strbTable.AppendLine("</td>");
						}
						strbTable.AppendLine("</tr>");
					}
				}
				strbTable.Append("</table>");
				s=s.Replace(tableStrOrig,strbTable.ToString());
			}
			//Unordered List----------------------------------------------------------------------------------------------------------------
			//Instead of using a regex, this will hunt through the rows in sequence.
			//later nesting by running ***, then **, then *
			s=ProcessList(s,"*");
			//numbered list---------------------------------------------------------------------------------------------------------------------
			s=ProcessList(s,"#");
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
						if(tagName=="img") {//wrap in <p> tags so IE prints properly and rotated images do not overlap other content
							strbSnew.Append(ProcessParagraph(s.Substring(0,iScanSibling),startsWithCR));
						}
						else {
							strbSnew.Append(s.Substring(0,iScanSibling));
						}
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
				s=WikiPages.WikiPageMaster.PageContent.Replace("@@@body@@@",strbOut.ToString());
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
			//We also want to remove links to other wiki pages. We can assume that no internal links with pipes exist, since that is forbidden.
			StringTools.RegReplace(strb,@"\[\[[^|]+?\]\]","");
			//Matches a pair of opening brackets with the first following pair of closing brackets, and everything between.
			//This will capture everything between the first pipe and the first pair of closing brackets.
			//Replaces the whole match with the captured text, aka the tag's content.
			StringTools.RegReplace(strb,@"\[\[.*?\|(.*?)\]\]","$1",RegexOptions.Singleline);
			return strb.ToString();
		}

		///<summary>This will get called repeatedly.  prefixChars is, for now, * or #.  Expects s to be mostly processed with the exception of {{nbsp}}s and prefixChars. Returns the text of the full document with the wiki markup for lists converted to HTML. Public for unit testing.</summary>
		public static string ProcessList(string s,string prefixChars){
			string listTag ="";
			if(prefixChars=="#") {
				listTag="ol";
			}
			else if(prefixChars=="*") {
				listTag="ul";
			}
			string[] lines=s.Split("\n",StringSplitOptions.None);//includes empty elements
			bool isWithinListTag=false;//Keep track of when we enter a list tag and have yet to close it.
			for(int i=0;i<lines.Length;i++) {
				if(!lines[i].Contains(prefixChars)) {
					continue;
				}
				lines[i]=lines[i].Replace("\r","");
				//Exactly matches the format of a table row that has been processed most of the way at this point. Each set of parenthesis is a different match group that will be used below for processing prefixchars into ul/ol and li tags. Example: <td Width="100"><p>*1<br/>*2</p></td>
				string patternListsInTable=@"<td Width=""\d+"">(<p>(.+)</p>)</td>";
				StringBuilder stringBuilder=new StringBuilder();
				Match match=Regex.Match(lines[i],patternListsInTable);
				if(match.Success) {//There are list(s) present in table(s)
					//Groups[2] represents the outermost set of parenthesis in the regex above.
					//Example: In a table row like: <td Width="100"><p>*1<br/>*2</p></td>
					//Groups[2] refers to the contents ofthe opening and closing <td> tags, namely <p>*1<br/>*2</p>
					if(!match.Groups[2].Value.StartsWith(prefixChars) && !match.Groups[2].Value.StartsWith("<br/>")) {
						continue;//This is not a list and simply contains a # or * E.g. 4*4.
					}
					string[] strArrayListItems=match.Groups[2].Value.Split(prefixChars,StringSplitOptions.RemoveEmptyEntries);
					//Loop through the raw list items and surrround with <li></li>
					for(int j = 0;j < strArrayListItems.Length;j++) {
						if(!strArrayListItems[j].StartsWith("<br/>")) {
							stringBuilder.Append(strArrayListItems[j]);
							continue;
						}
						if(j > 0 || strArrayListItems.Length==1) {
							stringBuilder.Append("<li><span class=\"ListItemContent\">"+strArrayListItems[j]+"</span></li>");
						}
					}
					//Replace the following match with the end-result after replacing the list item prefix chars and wrapping it in the item tag.
					//E.g. <p>(.+)</p>  =>  <ol>(.+)</ol>  where (.+) has each # or * turned into <li></li>.
					//Groups[1] Refers to the inner most group of parenthesis. E.g.In a table row like: <td Width="100"><p>*1<br/>*2</p></td>
					//Group[i] would refer to what is between the opening and closing <p> tags, namely *1<br/>*2.
					lines[i]=lines[i].Replace(match.Groups[1].Value,$@"<{listTag}>{stringBuilder}</{listTag}>");
				}
				else {//List(s) are present outside of tables
					string line=lines[i];
					//At this point in the markup processing there will be some other tags present in the text we're parsing.
					//The only tags that will cause errors are <body> tags. Trim them off and add them back after we have wrapped the content in li tags.
					bool addEndBodyTag=false;
					bool addStartBodyTag=false;
					if(line.StartsWith("<body>")) {
						line=line.Substring("<body>".Length);
						addStartBodyTag=true;
					}
					if(line.EndsWith("</body>")) {
						line=line.Substring(0,line.Length-"</body>".Length);
						addEndBodyTag=true;
					}
					if(!line.StartsWith(prefixChars)) {
						continue;//This is not a list and simply contains a # or * E.g. Math is #fun.
					}
					line=line.Substring(prefixChars.Length);//Trim off the prefix chars
					//There is CSS code in our master template that does formatting things on the ListItemContent class specifically.
					line="<li><span class=\"ListItemContent\">"+line+"</span></li>";
					//Add the approriate ol/ul tag if this is the beginning of a list (the previous line is not a list item).
					if(i==0 || (i > 0 && !isWithinListTag)) {
						line=$"<{listTag}>{line}";
						isWithinListTag=true;
					}
					//Add the approriate closing ol/ul tag if this is the end of a list (the next line is not a list item).
					if(i == lines.Length-1 || !lines[i+1].Contains(prefixChars)) {
						line=$"{line}</{listTag}>";
						isWithinListTag=false;
					}
					//Add in the body tags if we removed them
					if(addStartBodyTag) {
						line="<body>"+line;
					}
					if(addEndBodyTag) {
						line=line+"</body>";
					}
					lines[i]=lines[i].Replace(lines[i],line);
				}
			}
			return string.Join("\n",lines);
		}

		///<summary>This will wrap the text in p tags as well as handle internal carriage returns.  startsWithCR is only used on the first paragraph 
		///for the unusual case where the entire content starts with a CR.  This prevents stripping it off.</summary>
		private static string ProcessParagraph(string paragraph,bool startsWithCR,bool removeTrailingCR=true) {
			if(paragraph.StartsWith("\n") && !startsWithCR) {
				paragraph=paragraph.Substring(1);
			}
			if(paragraph=="") {//this must come after the first CR is stripped off, but before the ending CR is stripped off.
				return "";
			}
			if(paragraph.EndsWith("\n") && removeTrailingCR) {//trailing CR remove
				paragraph=paragraph.Substring(0,paragraph.Length-1);
			}
			string strP="";
			//images rotated 90 and 270 degrees need their paragraph to be sized correctly
			if(paragraph.StartsWith("<img") && (paragraph.Contains("transform:rotate(90") || paragraph.Contains("transform:rotate(270"))) {
				strP+="<p";
				strP+=" style=\"";
				List<string> listHeightAndWidth=paragraph.Split(' ').Where(x=>x.StartsWith("height") || x.StartsWith("width")).ToList();
				for(int i=0;i<listHeightAndWidth.Count;i++) {
					listHeightAndWidth[i]=listHeightAndWidth[i].Replace('=',':').Replace("\"","");
					//width and height will swap so paragraph is the right dimensions to hold the rotated image
					if(listHeightAndWidth[i].StartsWith("height")) {
						listHeightAndWidth[i]=listHeightAndWidth[i].Replace("height","width");
					}
					else {
						listHeightAndWidth[i]=listHeightAndWidth[i].Replace("width","height");
					}
					strP+=listHeightAndWidth[i]+";";
				}
				strP+="\">";
			}
			else{
				strP+="<p>";
			}
			//if the paragraph starts with any number of spaces followed by a tag such as <b> or <span>, then we need to force those spaces to show.
			if(paragraph.StartsWith(" ") && paragraph.TrimStart(' ').StartsWith("<")) {
				paragraph="{{nbsp}}"+paragraph.Substring(1);//this will later be converted to &nbsp;
			}
			paragraph=paragraph.Replace("\n","<br/>");//We tried </p><p>, but that didn't allow bold, italic, or color to span lines.
			paragraph=strP+paragraph+"</p>";//surround paragraph with tags
			paragraph=paragraph.Replace("<p> ","<p>{{nbsp}}");//spaces at the beginnings of paragraphs
			paragraph=paragraph.Replace("<br/> ","<br/>{{nbsp}}");//spaces at beginnings of lines
			paragraph=paragraph.Replace("<br/></p>","<br/>{{nbsp}}</p>");//have a cr show if it's at the end of a paragraph
			return paragraph;
		}

	}

}
