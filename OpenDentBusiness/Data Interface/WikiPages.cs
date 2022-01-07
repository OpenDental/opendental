using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CodeBase;
using OpenDentBusiness.FileIO;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WikiPages{
		#region Update
		public static void Update(WikiPage wikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			Crud.WikiPageCrud.Update(wikiPage);
		}

		public static void Update(WikiPage wikiPage,WikiPage oldWikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage,oldWikiPage);
				return;
			}
			Crud.WikiPageCrud.Update(wikiPage,oldWikiPage);
		}
		#endregion

		///<summary>Defines delegate signature to be used for WikiPages.NavPageDelegate.</summary>
		public delegate void NavToPageDelegate(string pageTitle);

		///<summary>Sent in from FormOpendental. Allows static method for business layer to cause wikipage navigation in FormOpendental.</summary>
		public static NavToPageDelegate NavPageDelegate;

		#region CachePattern
		///<summary>The only wiki page that gets cached is the master page.</summary>
		private static WikiPage masterPage;

		///<summary></summary>
		public static WikiPage MasterPage {
			get {
				if(masterPage==null) {
					RefreshCache();
				}
				return masterPage;
			}
			set {
				masterPage=value;
			}
		}

		///<summary></summary>
		public static DataTable RefreshCache() {
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			string command="SELECT * FROM wikipage WHERE PageTitle='_Master' AND IsDraft=0";//There is currently no way for a master page to be a draft.
			DataTable table=Cache.GetTableRemotelyIfNeeded(MethodBase.GetCurrentMethod(),command);
			table.TableName="WikiPage";
			FillCache(table);
			return table;
		}

		///<summary></summary>
		public static void FillCache(DataTable table) {
			//No need to check RemotingRole; no call to db.
			masterPage=Crud.WikiPageCrud.TableToList(table)[0];
		}
		#endregion CachePattern

		public static WikiPage GetWikiPage(long wikiPageNum) {
			//No need to check RemotingRole; no call to db.
			return GetWikiPages(new List<long>() { wikiPageNum }).FirstOrDefault();
		}

		///<summary>Returns a list of non draft wiki pages.</summary>
		public static List<WikiPage> GetWikiPages(List<long> listWikiPageNums) {
			if(listWikiPageNums==null || listWikiPageNums.Count==0) {
				return new List<WikiPage>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),listWikiPageNums);
			}
			string command="SELECT * FROM wikipage  "
				+"WHERE IsDraft=0 "
				+"AND WikiPageNum IN ("+string.Join(",",listWikiPageNums.Select(x => POut.Long(x)))+")";
			return Crud.WikiPageCrud.SelectMany(command);
		}

		///<summary>Returns null if page does not exist. Does not return drafts.</summary>
		public static WikiPage GetByTitle(string pageTitle, bool isDeleted=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<WikiPage>(MethodBase.GetCurrentMethod(),pageTitle,isDeleted);
			}
			string command="SELECT * FROM wikipage "
				+"WHERE PageTitle='"+POut.String(pageTitle)+"' "
				+"AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted);
			return Crud.WikiPageCrud.SelectOne(command);
		}

		///<summary>Returns a list of pages with PageTitle LIKE '%searchText%'.  Excludes titles that start with underscore.
		///Does not return drafts.</summary>
		public static List<WikiPage> GetByTitleContains(string searchText,bool doIncludeArchived) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),searchText,doIncludeArchived);
			}
			string command="SELECT * FROM wikipage "+
				"WHERE PageTitle NOT LIKE '\\_%' "+
				"AND IsDraft=0 "+
				"AND PageTitle LIKE '%"+POut.String(searchText)+"%' ";
			if(!doIncludeArchived) {
				command+="AND IsDeleted=0 ";
            }
			command+="ORDER BY PageTitle";
			return Crud.WikiPageCrud.SelectMany(command);
		}

		///<summary>Returns a table of wiki page titles that match searchText. Ordered by exact matches followed by alphabetical. The table will include the following columns: PageTitleDisplay, WikiPageNum. Excludes titles that start with underscore.  Does not return drafts.  Prepends "(Archived)" to PageTitleDisplay for deleted pages.</summary>
		public static DataTable GetTitlesByTitleContains(string searchText,bool includeDeleted=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),searchText,includeDeleted);
			}
			string command="SELECT IF(IsDeleted, CONCAT('(Archived) ', PageTitle), PageTitle) as PageTitleDisplay, WikiPageNum FROM wikipage "+
				"WHERE PageTitle NOT LIKE '\\_%' "+
				"AND IsDraft=0 "+
				"AND PageTitle LIKE '%"+POut.String(searchText)+"%' ";
			if(!includeDeleted) {
				command+="AND IsDeleted=0 ";
			}
			command+="ORDER BY (PageTitle='"+POut.String(searchText)+"') DESC, PageTitle";  // return exact matches first
			DataTable tableResults=Db.GetTable(command);
			return tableResults;
		}

		///<summary>Returns empty list if page does not exist.</summary>
		public static List<WikiPage> GetDraftsByTitle(string pageTitle) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			string command="SELECT * FROM wikipage WHERE PageTitle='"+POut.String(pageTitle)+"' AND IsDraft=1";
			return Crud.WikiPageCrud.SelectMany(command);
		}

		public static void WikiPageRestore(WikiPage wikiPageRestored,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPageRestored,userNum);
				return;
			}
			//Update the wikipage with new user and flip the IsDelete flag.
			wikiPageRestored.IsDeleted=false;
			wikiPageRestored.UserNum=userNum;
			wikiPageRestored.DateTimeSaved=MiscData.GetNowDateTime();
			Crud.WikiPageCrud.Update(wikiPageRestored);
		}

		///<summary>Used when saving a page to check and fix the capitalization on each internal link. 
		///So the returned pagetitle might have different capitalization than the supplied pagetitle.
		///Does not return drafts.</summary>
		public static string GetTitle(string pageTitle) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),pageTitle);
			}
			string command="SELECT PageTitle FROM wikipage WHERE PageTitle = '"+POut.String(pageTitle)+"' AND IsDraft=0";
			return Db.GetScalar(command);
		}

		///<summary>Archives first by moving to WikiPageHist if it already exists.  Then, in either case, it inserts the new page.
		///Does not delete drafts.</summary>
		public static long InsertAndArchive(WikiPage wikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				wikiPage.WikiPageNum=Meth.GetLong(MethodBase.GetCurrentMethod(),wikiPage);
				return wikiPage.WikiPageNum;
			}
			wikiPage.PageContentPlainText=MarkupEdit.ConvertToPlainText(wikiPage.PageContent);
			wikiPage.IsDraft=false;
			WikiPage wpExisting=GetByTitle(wikiPage.PageTitle);
			if(wpExisting!=null) {
				WikiPageHist wpHist=PageToHist(wpExisting);
				WikiPageHists.Insert(wpHist);
				wikiPage.DateTimeSaved=MiscData.GetNowDateTime();
				//Old behavior was to delete the wiki page and then always insert.
				//It was changed to Update here for storing wiki page references by WikiPageNum instead of PageTitle
				//See JobNum 4429 for additional information.
				Crud.WikiPageCrud.Update(wikiPage);
				return wikiPage.WikiPageNum;
			}
			//Deleted(archived) wp with the same page title should be updated with new page content. No need to create a new wp if the wikipage exist already.
			WikiPage wpDeleted=GetByTitle(wikiPage.PageTitle,isDeleted:true);
			if(wpDeleted!=null) {
				//No need to add history since we already added the history when we archived it.
				wikiPage.WikiPageNum=wpDeleted.WikiPageNum;
				wikiPage.DateTimeSaved=wpDeleted.DateTimeSaved;
				Crud.WikiPageCrud.Update(wikiPage);
				return wikiPage.WikiPageNum;
			}
			//At this point the wp does not exist. Insert new a new wikipage.
			return Crud.WikiPageCrud.Insert(wikiPage);
		}

		///<summary>Should only be used for inserting drafts.  Inserting a non-draft wikipage should call InsertAndArchive.</summary>
		public static void InsertAsDraft(WikiPage wikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			wikiPage.IsDraft=true;
			Crud.WikiPageCrud.Insert(wikiPage);
		}

		///<summary>Throws Exceptions, surround with try catch. Should only be used for updating drafts.  Updating a non-draft wikipage should never happen.</summary>
		public static void UpdateDraft(WikiPage wikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			if(!wikiPage.IsDraft) {
				throw new Exception("Can only use for updating drafts.");
			}
			Crud.WikiPageCrud.Update(wikiPage);
		}

        ///<summary>Searches keywords, title, content.  Does not return pagetitles for drafts.</summary>
        public static List<string> GetForSearch(string searchText,bool ignoreContent,bool isDeleted=false, bool isExactSearch=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),searchText,ignoreContent,isDeleted,isExactSearch);
			}
			List<string> retVal=new List<string>();
			DataTable tableResults=new DataTable();
			string[] searchTokens;
			if(isExactSearch) {
				searchTokens=new string[] { POut.String(searchText) };
			} 
			else {
				searchTokens=POut.String(searchText).Split(' ');
			}
			string command="";
			//Match keywords first-----------------------------------------------------------------------------------
			//When a page has a wikipage link, we save it as [[WikiPageNum]] in the page content. Get a list of WikiPageNums that match the search term.
			//We will use the WikiPageNum to search for pages with links to the search term
			List<long> listWikiPageNumsPageTitle=new List<long>();
			command=
				"SELECT WikiPageNum,PageTitle FROM wikipage "
				// \_ represents a literal _ because _ has a special meaning in LIKE clauses.
				//The second \ is just to escape the first \.  The other option would be to pass the \ through POut.String.
				+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=$"AND KeyWords LIKE '%{POut.String(searchTokens[i])}%' ";
			}
			command+=
				"GROUP BY PageTitle "
				+"ORDER BY PageTitle";
			tableResults=Db.GetTable(command);
			for(int i=0;i<tableResults.Rows.Count;i++) {
				if(!retVal.Contains(tableResults.Rows[i]["PageTitle"].ToString())) {
					retVal.Add(tableResults.Rows[i]["PageTitle"].ToString());
					listWikiPageNumsPageTitle.Add(PIn.Long(tableResults.Rows[i]["WikiPageNum"].ToString()));
				}
			}
			//Match PageTitle Second-----------------------------------------------------------------------------------
			command=
				"SELECT WikiPageNum,PageTitle FROM wikipage "
				+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+="AND PageTitle LIKE '%"+POut.String(searchTokens[i])+"%' ";
			}
			command+=
				"GROUP BY PageTitle "
				+"ORDER BY PageTitle";
			tableResults=Db.GetTable(command);
			for(int i=0;i<tableResults.Rows.Count;i++) {
				if(!retVal.Contains(tableResults.Rows[i]["PageTitle"].ToString())) {
					retVal.Add(tableResults.Rows[i]["PageTitle"].ToString());
					listWikiPageNumsPageTitle.Add(PIn.Long(tableResults.Rows[i]["WikiPageNum"].ToString()));
				}
			}
			//Match Content third-----------------------------------------------------------------------------------
			if(!ignoreContent) {
				command=
					"SELECT PageTitle FROM wikipage "
					+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
					+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
				for(int i=0;i<searchTokens.Length;i++) {
					command+="AND ";
					if(i==0) {
						command+="((";
					}
					command+="PageContentPlainText LIKE '%"+POut.String(searchTokens[i])+"%' ";
				}
				command+=") ";
				if(!listWikiPageNumsPageTitle.IsNullOrEmpty()) {
					command+=string.Join(" ",listWikiPageNumsPageTitle.Select(x => $"OR PageContent LIKE '%[[{x}]]%'"));
				}
				command+=") ";
				command+=
					"GROUP BY PageTitle "
					+"ORDER BY PageTitle";
				tableResults=Db.GetTable(command);
				for(int i=0;i<tableResults.Rows.Count;i++) {
					if(!retVal.Contains(tableResults.Rows[i]["PageTitle"].ToString())) {
						retVal.Add(tableResults.Rows[i]["PageTitle"].ToString());
					}
				}
			}
			return retVal;
		}

		///<summary>Returns a list of all pages that reference "PageTitle".  No historical pages or drafts.</summary>
		public static List<WikiPage> GetIncomingLinks(string pageTitle) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			List<WikiPage> retVal=new List<WikiPage>();
			WikiPage wp=GetByTitle(pageTitle);
			if(wp!=null) {
				string command="SELECT * FROM wikipage WHERE PageContent LIKE '%[["+POut.Long(wp.WikiPageNum)+"]]%' AND IsDraft=0 AND IsDeleted=0 ORDER BY PageTitle";
				retVal=Crud.WikiPageCrud.SelectMany(command);
			}
			return retVal;
		}

		///<summary>Validation was already done in FormWikiRename to make sure that the page does not already exist in WikiPage table.
		///But what if the page already exists in WikiPageHistory?  In that case, previous history for the other page would start showing as history for
		///the newly renamed page, which is fine.  Also renamed drafts, so that we can still match them to their parent wiki page.</summary>
		public static void Rename(WikiPage wikiPage, string newPageTitle) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage,newPageTitle);
				return;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			wikiPage.UserNum=Security.CurUser.UserNum;
			//a later improvement would be to validate again here in the business layer.
			InsertAndArchive(wikiPage);
			//Rename all pages in both tables: wikiPage and wikiPageHist.
			string command="UPDATE wikipage SET PageTitle='"+POut.String(newPageTitle)+"'WHERE PageTitle='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			command="UPDATE wikipagehist SET PageTitle='"+POut.String(newPageTitle)+"'WHERE PageTitle='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			//Update all home pages for users.
			command="UPDATE userodpref SET ValueString='"+POut.String(newPageTitle)+"' "
				+"WHERE FkeyType="+POut.Int((int)UserOdFkeyType.WikiHomePage)+" "
				+"AND ValueString='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			return;
		}

		///<summary>Used in TranslateToXhtml to know whether to mark a page as not exists.</summary>
		public static List<bool> CheckPageNamesExist(List<string> pageTitles) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<bool>>(MethodBase.GetCurrentMethod(),pageTitles);
			}
			string command="SELECT PageTitle FROM wikipage WHERE ";
			for(int i=0;i<pageTitles.Count;i++){
				if(i>0) {
					command+="OR ";
				}
				command+="PageTitle='"+POut.String(pageTitles[i])+"' ";
			}
			DataTable table=Db.GetTable(command);
			List<bool> retVal=new List<bool>();
			for(int p=0;p<pageTitles.Count;p++) {
				bool valForThisPage=false;
				for(int i=0;i<table.Rows.Count;i++) {
					if(table.Rows[i]["PageTitle"].ToString().ToLower()==pageTitles[p].ToLower()) {
						valForThisPage=true;
						break;
					}
				}
				retVal.Add(valForThisPage);
			}
			return retVal;
		}

		/*///<summary>Update may be implemented when versioning is improved.</summary>
		public static void Update(WikiPage wikiPage){
			Insert(wikiPage);
			//if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
			//  Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
			//  return;
			//}
			//Crud.WikiPageCrud.Update(wikiPage);
		}*/

		///<summary>Surround with try/catch.  Typically returns something similar to \\SERVER\OpenDentImages\Wiki</summary>
		public static string GetWikiPath() {
			//No need to check RemotingRole; no call to db.
			string wikiPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				throw new ApplicationException(Lans.g("WikiPages","Must be using AtoZ folders."));
			}
			wikiPath=CloudStorage.PathTidy(Path.Combine(ImageStore.GetPreferredAtoZpath(),"Wiki"));
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(wikiPath)) {
				Directory.CreateDirectory(wikiPath);
			}
			return wikiPath;
		} 
				
		///<summary>When this is called, all WikiPage links in pageContent should be [[PageTitle]] and NOT [[WikiPageNum]],
			///otherwise this will invalidate every wiki page link.</summary>
		public static string ConvertTitlesToPageNums(string pageContent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),pageContent);
			}
			MatchCollection mc=Regex.Matches(pageContent,@"\[\[.+?\]\]");//Find [[ and ]] pairs in the pageContent string.
			List<string> listWikiLinks=new List<string>();
			foreach(Match match in mc) {
				string val=match.Value;
				if(!IsWikiLink(val)) {
					continue;
				}
				val=val.TrimStart('[').TrimEnd(']');
				listWikiLinks.Add(val);
			}
			//Getting the MIN(WikiPageNum) is safe because there will always only be 1 non-draft reference to the PageTitle.
			//This is because we reuse WikiPageNums instead of deleting and reinserting like before 17.4.
			string command=@"SELECT PageTitle,MIN(WikiPageNum) WikiPageNum FROM wikipage 
				WHERE IsDraft=0 AND PageTitle IN('"+string.Join("','",listWikiLinks.Select(x => POut.String(x)))+@"')
				GROUP BY PageTitle";
			Dictionary<string,long> dictTitleToNum=Db.GetTable(command).Select()
				.ToDictionary(x => PIn.String(x["PageTitle"].ToString()),x => PIn.Long(x["WikiPageNum"].ToString()));
			StringBuilder sbPageContent=new StringBuilder(pageContent);
			foreach(string wikiLink in listWikiLinks) {
				string replace;
				long wikiPageNum;
				if(dictTitleToNum.TryGetValue(wikiLink,out wikiPageNum)) {
					replace="[["+wikiPageNum+"]]";
				}
				else {
					replace="[[0]]";//wiki page does not exist. replace with wikipagenum=0
				}
				sbPageContent.Replace("[["+wikiLink+"]]",replace);
			}
			return sbPageContent.ToString();
		}

		public static List<string> GetMissingPageTitles(string pageContent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),pageContent);
			}
			List<string> listInvalidLinks=new List<string>();
			MatchCollection mc=Regex.Matches(pageContent,@"\[\[.+?\]\]");//Find [[ and ]] pairs in the pageContent string.
			List<string> listWikiLinks=new List<string>();
			foreach(Match match in mc) {
				string val=match.Value;
				if(!IsWikiLink(val) || val.Contains("INVALID WIKIPAGE LINK")) {
					continue;
				}
				val=val.TrimStart('[').TrimEnd(']');
				listWikiLinks.Add(val);
			}
			if(listWikiLinks.Count==0) {
				return listInvalidLinks;
			}
			string command=@"SELECT PageTitle FROM wikipage 
				WHERE IsDraft=0 AND PageTitle IN('"+string.Join("','",listWikiLinks.Select(x => POut.String(x)))+"')";
			HashSet<string> setValidLinks=new HashSet<string>(Db.GetListString(command));
			listInvalidLinks=listWikiLinks.FindAll(x => !setValidLinks.Contains(x));
			return listInvalidLinks;
		}

		public static bool IsWikiLink(string val) {
			if(val.Contains("[[img:")
				|| val.Contains("[[keywords:")
				|| val.Contains("[[file:")
				|| val.Contains("[[folder:")
				|| val.Contains("[[list:")
				|| val.Contains("[[color:")
				|| val.Contains("[[filecloud:")
				|| val.Contains("[[foldercloud:")
				|| val.Contains("[[font:"))
			{
				return false;
			}
			return true;
		}

		public static List<long> GetWikiPageNumsFromPageContent(string pageContent) {
			//No need to check RemotingRole; no call to db.
			List<long> retVal=new List<long>();
			MatchCollection mc=Regex.Matches(pageContent,@"\[\[.+?\]\]");
			foreach(Match match in mc) {
				string wikiPageNum=match.Value;
				if(!IsWikiLink(wikiPageNum)) {
					continue;
				}
				//The current match is similar to our wiki page links.  The contents between the brackets should be a foreign key to another wiki page.
				wikiPageNum=wikiPageNum.TrimStart('[').TrimEnd(']');
				retVal.Add(PIn.Long(wikiPageNum));
			}
			return retVal;
		}

		public static string GetWikiPageContentWithWikiPageTitles(string pageContent) {
			//No need to check RemotingRole; no call to db.
			List<long> listWikiPageNums=GetWikiPageNumsFromPageContent(pageContent);
			List<WikiPage> listWikiPages=GetWikiPages(listWikiPageNums);
			int numInvalid=1;
			MatchCollection mc=Regex.Matches(pageContent,@"\[\[.+?\]\]");
			foreach(Match match in mc) {
				string val=match.Value;
				if(!IsWikiLink(val)) {
					continue;
				}
				//The current match is similar to our wiki page links.  The contents between the brackets should be a foreign key to another wiki page.
				val=val.TrimStart('[').TrimEnd(']');
				WikiPage wp=listWikiPages.FirstOrDefault(x => x.WikiPageNum==PIn.Long(val,false));
				string replace;
				if(wp!=null) {
					replace="[["+wp.PageTitle+"]]";
				}
				else {
					replace="[[INVALID WIKIPAGE LINK "+numInvalid+++"]]";//Wiki page does not exist.
				}
				Regex regex=new Regex(Regex.Escape(match.Value));
				//Replace the first instance of the match with the wiki page name (or unknown if not found).
				pageContent=regex.Replace(pageContent,replace,1);
			}
			return pageContent;
		}

		///<summary>Throws exceptions, surround with Try catch.Only delete wiki drafts with this function.  Normal wiki pages cannot be deleted, only archived.</summary>
		public static void DeleteDraft(WikiPage wikiPage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			if(!wikiPage.IsDraft) {
				throw new Exception("Can only use for deleting drafts.");
			}
			Crud.WikiPageCrud.Delete(wikiPage.WikiPageNum);
		}

		///<summary>Creates historical entry of deletion into wikiPageHist, and deletes current non-draft page from WikiPage.
		///For middle tier purposes we need to have the currently logged in user passed into this method.</summary>
		public static void Archive(string pageTitle,long userNumCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pageTitle,userNumCur);
				return;
			}
			WikiPage wikiPage=GetByTitle(pageTitle);
			if(wikiPage==null) {
				return;//The wiki page could not be found by the page title, nothing to do.
			}
			WikiPageHist wikiPageHist=PageToHist(wikiPage);
			//preserve the existing page with user credentials
			WikiPageHists.Insert(wikiPageHist);
			//make entry to show who deleted the page
			wikiPageHist.IsDeleted=true;
			wikiPageHist.UserNum=userNumCur;
			wikiPageHist.DateTimeSaved=MiscData.GetNowDateTime();
			WikiPageHists.Insert(wikiPageHist);
			//Now mark the wikipage as IsDeleted 
			wikiPage.IsDeleted=true;
			wikiPage.DateTimeSaved=MiscData.GetNowDateTime();
			Crud.WikiPageCrud.Update(wikiPage);
			//Remove all associated home pages for all users.
			UserOdPrefs.DeleteForValueString(0,UserOdFkeyType.WikiHomePage,pageTitle);
		}

		public static WikiPageHist PageToHist(WikiPage wikiPage) {
			//No need to check RemotingRole; no call to db.
			WikiPageHist wikiPageHist=new WikiPageHist();
			wikiPageHist.UserNum=wikiPage.UserNum;
			wikiPageHist.PageTitle=wikiPage.PageTitle;
			wikiPageHist.PageContent=wikiPage.PageContent;
			wikiPageHist.DateTimeSaved=wikiPage.DateTimeSaved;//This gets set to NOW if this page is then inserted
			wikiPageHist.IsDeleted=false;
			return wikiPageHist;
		}

		public static bool IsWikiPageTitleValid(string pageTitle,out string errorMsg) {
			errorMsg="";
			if(pageTitle.Contains("#")) {
				errorMsg=Lans.g("WikiPages","Page title cannot contain the pound character.");
				return false;
			}
			if(pageTitle.StartsWith("_")) {
				errorMsg=Lans.g("WikiPages","Page title cannot start with the underscore character.");
				return false;
			}
			if(pageTitle.Contains("\"")) {
				errorMsg=Lans.g("WikiPages","Page title cannot contain double quotes.");
				return false;
			}
			if(pageTitle.Contains("\r") || pageTitle.Contains("\n")) {
				errorMsg=Lans.g("WikiPages","Page title cannot contain carriage return.");//there is also no way to enter one.
				return false;
			}
			return true;
		}

		///<summary>Gets one WikiPage from the db.</summary>
		public static WikiPage GetOne(long wikiPageNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<WikiPage>(MethodBase.GetCurrentMethod(),wikiPageNum);
			}
			return Crud.WikiPageCrud.SelectOne(wikiPageNum);
		}

		///<summary>Updates the links to wpOld to wpNew in the wikipage page wikiPageCur.</summary>
		///<param name="wikiPageCurNum">This is the wikipage that will get updated.</param>
		///<param name="wpOldNum">This is the wikipage that will be used to find references in wikiPageCur.</param>
		///<param name="wpNewNum">This is the wikipage that will be used to replace the references to.</param>
		public static void UpdateWikiPageReferences(long wikiPageCurNum,long wpOldNum,long wpNewNum) {
			//No need to check RemotingRole; no call to db.
			WikiPage wikiPageCur=GetOne(wikiPageCurNum);//Getting from the database to ensure the WikiPageNums have not been replaced with PageTitles.
			if(wikiPageCur==null) {
				return;
			}
			WikiPage wikiPageOld=wikiPageCur.Copy();
			StringBuilder sbPageContent=new StringBuilder(wikiPageCur.PageContent);
			StringTools.RegReplace(sbPageContent,$@"\[\[{wpOldNum}\]\]","[["+wpNewNum+"]]");
			wikiPageCur.PageContent=sbPageContent.ToString();
			WikiPages.Update(wikiPageCur,wikiPageOld);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<WikiPage> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM wikipage WHERE PatNum = "+POut.Long(patNum);
			return Crud.WikiPageCrud.SelectMany(command);
		}
		*/
	}
}