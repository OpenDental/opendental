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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			Crud.WikiPageCrud.Update(wikiPage);
		}

		public static void Update(WikiPage wikiPage,WikiPage oldWikiPage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		private static WikiPage _wikiPageMaster;

		///<summary></summary>
		public static WikiPage WikiPageMaster {
			get {
				if(_wikiPageMaster==null) {
					RefreshCache();
				}
				return _wikiPageMaster;
			}
			set {
				_wikiPageMaster=value;
			}
		}

		///<summary></summary>
		public static DataTable RefreshCache() {
			//No need to check MiddleTierRole; Calls GetTableRemotelyIfNeeded().
			string command="SELECT * FROM wikipage WHERE PageTitle='_Master' AND IsDraft=0";//There is currently no way for a master page to be a draft.
			DataTable table=Cache.GetTableRemotelyIfNeeded(MethodBase.GetCurrentMethod(),command);
			table.TableName="WikiPage";
			FillCache(table);
			return table;
		}

		public static void ClearCache() {
			_wikiPageMaster=null;
		}

		///<summary></summary>
		public static void FillCache(DataTable table) {
			//No need to check MiddleTierRole; no call to db.
			_wikiPageMaster=Crud.WikiPageCrud.TableToList(table)[0];
		}
		#endregion CachePattern
		public static WikiPage GetWikiPage(long wikiPageNum) {
			//No need to check MiddleTierRole; no call to db.
			WikiPage wikiPage=GetWikiPages(new List<long>() { wikiPageNum }).FirstOrDefault();
			return wikiPage;
		}

		///<summary>Returns a list of non draft wiki pages.</summary>
		public static List<WikiPage> GetWikiPages(List<long> listWikiPageNums) {
			if(listWikiPageNums==null || listWikiPageNums.Count==0) {
				return new List<WikiPage>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),listWikiPageNums);
			}
			string command="SELECT * FROM wikipage  "
				+"WHERE IsDraft=0 "
				+"AND WikiPageNum IN ("+string.Join(",",listWikiPageNums.Select(x => POut.Long(x)))+")";
			return Crud.WikiPageCrud.SelectMany(command);
		}

		///<summary>Returns null if page does not exist. Does not return drafts.</summary>
		public static WikiPage GetByTitle(string pageTitle, bool isDeleted=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WikiPage>(MethodBase.GetCurrentMethod(),pageTitle,isDeleted);
			}
			string command="SELECT * FROM wikipage "
				+"WHERE PageTitle='"+POut.String(pageTitle)+"' "
				+"AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted);
			return Crud.WikiPageCrud.SelectOne(command);
		}

		///<summary>Returns empty list if page does not exist.</summary>
		public static List<WikiPage> GetDraftsByTitle(string pageTitle) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			string command="SELECT * FROM wikipage WHERE PageTitle='"+POut.String(pageTitle)+"' AND IsDraft=1";
			return Crud.WikiPageCrud.SelectMany(command);
		}

		public static void WikiPageRestore(WikiPage wikiPageRestored,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPageRestored,userNum);
				return;
			}
			//Update the wikipage with new user and flip the IsDelete flag.
			wikiPageRestored.IsDeleted=false;
			wikiPageRestored.UserNum=userNum;
			wikiPageRestored.DateTimeSaved=MiscData.GetNowDateTime();
			Crud.WikiPageCrud.Update(wikiPageRestored);
		}

		///<summary>Archives first by moving to WikiPageHist if it already exists.  Then, in either case, it inserts the new page.
		///Does not delete drafts.</summary>
		public static long InsertAndArchive(WikiPage wikiPage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				wikiPage.WikiPageNum=Meth.GetLong(MethodBase.GetCurrentMethod(),wikiPage);
				return wikiPage.WikiPageNum;
			}
			wikiPage.PageContentPlainText=MarkupEdit.ConvertToPlainText(wikiPage.PageContent);
			wikiPage.IsDraft=false;
			WikiPage wikiPageExisting=GetByTitle(wikiPage.PageTitle);
			if(wikiPageExisting!=null) {
				WikiPageHist wikiPageHist=PageToHist(wikiPageExisting);
				WikiPageHists.Insert(wikiPageHist);
				wikiPage.DateTimeSaved=MiscData.GetNowDateTime();
				//Old behavior was to delete the wiki page and then always insert.
				//It was changed to Update here for storing wiki page references by WikiPageNum instead of PageTitle
				//See JobNum 4429 for additional information.
				Crud.WikiPageCrud.Update(wikiPage);
				return wikiPage.WikiPageNum;
			}
			//Deleted(archived) wp with the same page title should be updated with new page content. No need to create a new wp if the wikipage exist already.
			WikiPage wikiPageDeleted=GetByTitle(wikiPage.PageTitle,isDeleted:true);
			if(wikiPageDeleted!=null) {
				//No need to add history since we already added the history when we archived it.
				wikiPage.WikiPageNum=wikiPageDeleted.WikiPageNum;
				wikiPage.DateTimeSaved=wikiPageDeleted.DateTimeSaved;
				Crud.WikiPageCrud.Update(wikiPage);
				return wikiPage.WikiPageNum;
			}
			//At this point the wp does not exist. Insert new a new wikipage.
			return Crud.WikiPageCrud.Insert(wikiPage);
		}

		///<summary>Should only be used for inserting drafts.  Inserting a non-draft wikipage should call InsertAndArchive.</summary>
		public static void InsertAsDraft(WikiPage wikiPage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			wikiPage.IsDraft=true;
			Crud.WikiPageCrud.Insert(wikiPage);
		}

		///<summary>Throws Exceptions, surround with try catch. Should only be used for updating drafts.  Updating a non-draft wikipage should never happen.</summary>
		public static void UpdateDraft(WikiPage wikiPage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
				return;
			}
			if(!wikiPage.IsDraft) {
				throw new Exception("Can only use for updating drafts.");
			}
			Crud.WikiPageCrud.Update(wikiPage);
		}

		///<summary>Searches keywords, title, content.  Does not return pagetitles for drafts.</summary>
		public static List<string> GetForSearch(string searchText,bool ignoreContent,bool isDeleted=false, bool isExactSearch=false,bool showMainPages=false,bool searchForLinks=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),searchText,ignoreContent,isDeleted,isExactSearch,showMainPages,searchForLinks);
			}
			List<string> listPageTitles=new List<string>();
			DataTable tableResults=new DataTable();
			string[] stringArraySearchTokens;
			List<string> listTitlesWithKeyMain=new List<string>();
			if(isExactSearch) {
				stringArraySearchTokens=new string[] { POut.String(searchText) };
			} 
			else {
				stringArraySearchTokens=POut.String(searchText).Split(' ');
			}
			string command="";
			//Match keywords first-----------------------------------------------------------------------------------
			//When a page has a wikipage link, we save it as [[WikiPageNum]] in the page content. Get a list of WikiPageNums that match the search term.
			//We will use the WikiPageNum to search for pages with links to the search term
			List<long> listWikiPageNumsPageTitle=new List<long>();
			command=
				"SELECT WikiPageNum,PageTitle,KeyWords FROM wikipage "
				// \_ represents a literal _ because _ has a special meaning in LIKE clauses.
				//The second \ is just to escape the first \.  The other option would be to pass the \ through POut.String.
				+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
			for(int i=0;i<stringArraySearchTokens.Length;i++) {
				command+=$"AND KeyWords LIKE '%{POut.String(stringArraySearchTokens[i])}%' ";
			}
			command+=
				"GROUP BY PageTitle "
				+"ORDER BY PageTitle";
			tableResults=Db.GetTable(command);
			for(int i=0;i<tableResults.Rows.Count;i++) {
				string pageTitle=tableResults.Rows[i]["PageTitle"].ToString();
				if(listPageTitles.Contains(pageTitle)) {
					continue;
				}
				listPageTitles.Add(pageTitle);
				listWikiPageNumsPageTitle.Add(PIn.Long(tableResults.Rows[i]["WikiPageNum"].ToString()));
				if(showMainPages && HasMainKeyword(tableResults.Rows[i]["KeyWords"].ToString())) {
					listTitlesWithKeyMain.Add(pageTitle);
				}
			}
			//Match PageTitle Second-----------------------------------------------------------------------------------
			command=
				"SELECT WikiPageNum,PageTitle,KeyWords FROM wikipage "
				+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
				+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
			for(int i=0;i<stringArraySearchTokens.Length;i++) {
				command+="AND PageTitle LIKE '%"+POut.String(stringArraySearchTokens[i])+"%' ";
			}
			command+=
				"GROUP BY PageTitle "
				+"ORDER BY PageTitle";
			tableResults=Db.GetTable(command);
			for(int i=0;i<tableResults.Rows.Count;i++) {
				string pageTitle=tableResults.Rows[i]["PageTitle"].ToString();
				if(listPageTitles.Contains(pageTitle)) {
					continue;
				}
				listPageTitles.Add(pageTitle);
				listWikiPageNumsPageTitle.Add(PIn.Long(tableResults.Rows[i]["WikiPageNum"].ToString()));
				if(showMainPages && HasMainKeyword(tableResults.Rows[i]["KeyWords"].ToString())) {
					listTitlesWithKeyMain.Add(pageTitle);
				}
			}
			//Match Content third-----------------------------------------------------------------------------------
			if(!ignoreContent) {
				command=
					"SELECT PageTitle,KeyWords FROM wikipage "
					+"WHERE PageTitle NOT LIKE '\\_%' AND IsDraft=0 "
					+"AND IsDeleted="+POut.Bool(isDeleted)+" ";
				for(int i=0;i<stringArraySearchTokens.Length;i++) {
					command+="AND ";
					if(i==0) {
						command+="((";
					}
					command+="PageContentPlainText LIKE '%"+POut.String(stringArraySearchTokens[i])+"%' ";
				}
				command+=") ";
				if(!listWikiPageNumsPageTitle.IsNullOrEmpty() && searchForLinks) {
					for(int i=0;i<listWikiPageNumsPageTitle.Count;i++) {
						if(i!=0) {
							command+=" ";
						}
						command+="OR PageContent LIKE '%[["+listWikiPageNumsPageTitle[i]+"]]%'";
					}
				}
				command+=") ";
				command+=
					"GROUP BY PageTitle "
					+"ORDER BY PageTitle";
				tableResults=Db.GetTable(command);
				for(int i=0;i<tableResults.Rows.Count;i++) {
					string pageTitle=tableResults.Rows[i]["PageTitle"].ToString();
					if(listPageTitles.Contains(pageTitle)) {
						continue;
					}
					listPageTitles.Add(pageTitle);
					if(showMainPages && HasMainKeyword(tableResults.Rows[i]["KeyWords"].ToString())) {
						listTitlesWithKeyMain.Add(pageTitle);
					}
				}
			}
			//Show main pages at the top if checked and found some. listTitlesWithKeyMain will always be empty if showMainPages is false.
			if(listTitlesWithKeyMain.Count>0) {
				for(int i=0;i<listPageTitles.Count;i++) {
					if(!listTitlesWithKeyMain.Contains(listPageTitles[i])) {
						listTitlesWithKeyMain.Add(listPageTitles[i]);
					}
				}
				return listTitlesWithKeyMain;
			}
			return listPageTitles;
		}

		///<summary>Returns true if the passed in comma delimited keyword string contains 'Main' by itself. Otherwise false. Not case sensitive.</summary>
		public static bool HasMainKeyword(string keywords){
			List<string> listKeywords=keywords.ToLower().Split(',').ToList(); //Get each keyword
			for(int j=0;j<listKeywords.Count;j++) {
				listKeywords[j]=listKeywords[j].Trim(' ');
				if(listKeywords[j].Equals("main")) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns a list of all pages that reference "PageTitle".  No historical pages or drafts.</summary>
		public static List<WikiPage> GetIncomingLinks(string pageTitle) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			List<WikiPage> listWikiPages=new List<WikiPage>();
			WikiPage wikiPage=GetByTitle(pageTitle);
			if(wikiPage!=null) {
				string command="SELECT * FROM wikipage WHERE PageContent LIKE '%[["+POut.Long(wikiPage.WikiPageNum)+"]]%' AND IsDraft=0 AND IsDeleted=0 ORDER BY PageTitle";
				listWikiPages=Crud.WikiPageCrud.SelectMany(command);
			}
			return listWikiPages;
		}

		///<summary>Validation was already done in FormWikiRename to make sure that the page does not already exist in WikiPage table.
		///But what if the page already exists in WikiPageHistory?  In that case, previous history for the other page would start showing as history for
		///the newly renamed page, which is fine.  Also renamed drafts, so that we can still match them to their parent wiki page.</summary>
		public static void Rename(WikiPage wikiPage, string pageTitleNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage,pageTitleNew);
				return;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			wikiPage.UserNum=Security.CurUser.UserNum;
			//a later improvement would be to validate again here in the business layer.
			InsertAndArchive(wikiPage);
			//Rename all pages in both tables: wikiPage and wikiPageHist.
			string command="UPDATE wikipage SET PageTitle='"+POut.String(pageTitleNew)+"'WHERE PageTitle='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			command="UPDATE wikipagehist SET PageTitle='"+POut.String(pageTitleNew)+"'WHERE PageTitle='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			//Update all home pages for users.
			command="UPDATE userodpref SET ValueString='"+POut.String(pageTitleNew)+"' "
				+"WHERE FkeyType="+POut.Int((int)UserOdFkeyType.WikiHomePage)+" "
				+"AND ValueString='"+POut.String(wikiPage.PageTitle)+"'";
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.UserOdPrefs);
			UserOdPrefs.RefreshCache();
			return;
		}

		///<summary>Used in TranslateToXhtml to know whether to mark a page as not exists.</summary>
		public static List<bool> CheckPageNamesExist(List<string> pageTitles) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			List<bool> listExists=new List<bool>();
			for(int p=0;p<pageTitles.Count;p++) {
				bool existsThisPage=false;
				for(int i=0;i<table.Rows.Count;i++) {
					if(table.Rows[i]["PageTitle"].ToString().ToLower()==pageTitles[p].ToLower()) {
						existsThisPage=true;
						break;
					}
				}
				listExists.Add(existsThisPage);
			}
			return listExists;
		}

		/*///<summary>Update may be implemented when versioning is improved.</summary>
		public static void Update(WikiPage wikiPage){
			Insert(wikiPage);
			//if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
			//  Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPage);
			//  return;
			//}
			//Crud.WikiPageCrud.Update(wikiPage);
		}*/

		///<summary>Surround with try/catch.  Typically returns something similar to \\SERVER\OpenDentImages\Wiki</summary>
		public static string GetWikiPath() {
			//No need to check MiddleTierRole; no call to db.
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),pageContent);
			}
			MatchCollection matchCollection=Regex.Matches(pageContent,@"\[\[.+?\]\]");//Find [[ and ]] pairs in the pageContent string.
			List<string> listPageTitleLinks=new List<string>();
			for(int i=0;i<matchCollection.Count;i++) {
				string pageTitle=matchCollection[i].Value;
				if(!IsWikiLink(pageTitle)) {
					continue;
				}
				pageTitle=pageTitle.TrimStart('[').TrimEnd(']');
				listPageTitleLinks.Add(pageTitle);
			}
			//Getting the MIN(WikiPageNum) is safe because there will always only be 1 non-draft reference to the PageTitle.
			//This is because we reuse WikiPageNums instead of deleting and reinserting like before 17.4.
			string command=@"SELECT PageTitle,MIN(WikiPageNum) WikiPageNum FROM wikipage 
				WHERE IsDraft=0 AND PageTitle IN('"+string.Join("','",listPageTitleLinks.Select(x => POut.String(x)))+@"')
				GROUP BY PageTitle";
			DataTable table=Db.GetTable(command);
			List<WikiPage> listWikiPages=new List<WikiPage>();
			StringBuilder stringBuilderContent=new StringBuilder(pageContent);
			for(int i=0;i<table.Rows.Count;i++) {
				WikiPage wikiPage=new WikiPage();
				wikiPage.PageTitle=PIn.String(table.Rows[i]["PageTitle"].ToString());
				wikiPage.WikiPageNum=PIn.Long(table.Rows[i]["WikiPageNum"].ToString());
				listWikiPages.Add(wikiPage);
			}
			for(int i=0;i<listPageTitleLinks.Count;i++) {
				string replace;
				WikiPage wikiPage=listWikiPages.Find(x=>x.PageTitle==listPageTitleLinks[i]);
				if(wikiPage is null) {
					replace="[[0]]";//wiki page does not exist. replace with wikipagenum=0
				}
				else {
					replace="[["+wikiPage.WikiPageNum+"]]";
				}
				stringBuilderContent.Replace("[["+listPageTitleLinks[i]+"]]",replace);
			}
			return stringBuilderContent.ToString();
		}

		public static List<string> GetMissingPageTitles(string pageContent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),pageContent);
			}
			List<string> listInvalidLinks=new List<string>();
			MatchCollection matchCollection=Regex.Matches(pageContent,@"\[\[.+?\]\]");//Find [[ and ]] pairs in the pageContent string.
			List<string> listWikiLinks=new List<string>();
			for(int i=0;i<matchCollection.Count;i++) {
				string val=matchCollection[i].Value;
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
			HashSet<string> hashSetValidLinks=new HashSet<string>(Db.GetListString(command));
			listInvalidLinks=listWikiLinks.FindAll(x => !hashSetValidLinks.Contains(x));
			return listInvalidLinks;
		}

		public static bool IsWikiLink(string strVal) {
			if(strVal.Contains("[[img:")
				|| strVal.Contains("[[keywords:")
				|| strVal.Contains("[[file:")
				|| strVal.Contains("[[folder:")
				|| strVal.Contains("[[list:")
				|| strVal.Contains("[[color:")
				|| strVal.Contains("[[filecloud:")
				|| strVal.Contains("[[foldercloud:")
				|| strVal.Contains("[[font:"))
			{
				return false;
			}
			return true;
		}

		public static List<long> GetWikiPageNumsFromPageContent(string pageContent) {
			//No need to check MiddleTierRole; no call to db.
			List<long> listWikiPageNums=new List<long>();
			MatchCollection matchCollection=Regex.Matches(pageContent,@"\[\[.+?\]\]");
			for(int i=0;i<matchCollection.Count;i++) {
				string wikiPageNum=matchCollection[i].Value;
				if(!IsWikiLink(wikiPageNum)) {
					continue;
				}
				//The current match is similar to our wiki page links.  The contents between the brackets should be a foreign key to another wiki page.
				wikiPageNum=wikiPageNum.TrimStart('[').TrimEnd(']');
				listWikiPageNums.Add(PIn.Long(wikiPageNum));
			}
			return listWikiPageNums;
		}

		public static string GetWikiPageContentWithWikiPageTitles(string pageContent) {
			//No need to check MiddleTierRole; no call to db.
			List<long> listWikiPageNums=GetWikiPageNumsFromPageContent(pageContent);
			List<WikiPage> listWikiPages=GetWikiPages(listWikiPageNums);
			int numInvalid=1;
			MatchCollection matchCollection=Regex.Matches(pageContent,@"\[\[.+?\]\]");
			for(int i=0;i<matchCollection.Count;i++) {
				string val=matchCollection[i].Value;
				if(!IsWikiLink(val)) {
					continue;
				}
				//The current match is similar to our wiki page links.  The contents between the brackets should be a foreign key to another wiki page.
				val=val.TrimStart('[').TrimEnd(']');
				WikiPage wikiPage=listWikiPages.FirstOrDefault(x => x.WikiPageNum==PIn.Long(val,false));
				string replace;
				if(wikiPage!=null) {
					replace="[["+wikiPage.PageTitle+"]]";
				}
				else {
					replace="[[INVALID WIKIPAGE LINK "+numInvalid+++"]]";//Wiki page does not exist.
				}
				Regex regex=new Regex(Regex.Escape(matchCollection[i].Value));
				//Replace the first instance of the match with the wiki page name (or unknown if not found).
				pageContent=regex.Replace(pageContent,replace,1);
			}
			return pageContent;
		}

		///<summary>Throws exceptions, surround with Try catch.Only delete wiki drafts with this function.  Normal wiki pages cannot be deleted, only archived.</summary>
		public static void DeleteDraft(WikiPage wikiPage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static void Archive(string pageTitle,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pageTitle,userNum);
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
			wikiPageHist.UserNum=userNum;
			wikiPageHist.DateTimeSaved=MiscData.GetNowDateTime();
			WikiPageHists.Insert(wikiPageHist);
			//Now mark the wikipage as IsDeleted 
			wikiPage.IsDeleted=true;
			wikiPage.DateTimeSaved=MiscData.GetNowDateTime();
			Crud.WikiPageCrud.Update(wikiPage);
			//Remove all associated home pages for all users.
			UserOdPrefs.DeleteForValueString(0,UserOdFkeyType.WikiHomePage,pageTitle);
			Signalods.SetInvalid(InvalidType.UserOdPrefs);
			UserOdPrefs.RefreshCache();
		}

		public static WikiPageHist PageToHist(WikiPage wikiPage) {
			//No need to check MiddleTierRole; no call to db.
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WikiPage>(MethodBase.GetCurrentMethod(),wikiPageNum);
			}
			return Crud.WikiPageCrud.SelectOne(wikiPageNum);
		}

		///<summary>Updates the links to wpOld to wpNew in the wikipage page wikiPageCur.</summary>
		///<param name="wikiPageNum">This is the wikipage that will get updated.</param>
		///<param name="wikiPageNumOld">This is the wikipage that will be used to find references in wikiPageCur.</param>
		///<param name="wikiPageNumNew">This is the wikipage that will be used to replace the references to.</param>
		public static void UpdateWikiPageReferences(long wikiPageNum,long wikiPageNumOld,long wikiPageNumNew) {
			//No need to check MiddleTierRole; no call to db.
			WikiPage wikiPage=GetOne(wikiPageNum);//Getting from the database to ensure the WikiPageNums have not been replaced with PageTitles.
			if(wikiPage==null) {
				return;
			}
			WikiPage wikiPageOld=wikiPage.Copy();
			StringBuilder sbPageContent=new StringBuilder(wikiPage.PageContent);
			StringTools.RegReplace(sbPageContent,$@"\[\[{wikiPageNumOld}\]\]","[["+wikiPageNumNew+"]]");
			wikiPage.PageContent=sbPageContent.ToString();
			WikiPages.Update(wikiPage,wikiPageOld);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<WikiPage> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiPage>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM wikipage WHERE PatNum = "+POut.Long(patNum);
			return Crud.WikiPageCrud.SelectMany(command);
		}
		*/
	}
}