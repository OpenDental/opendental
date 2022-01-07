using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WikiPageHists{
		///<summary>Ordered by dateTimeSaved.  Objects will not have the PageContent field populated.  Use GetPageContent to get the content for a
		///specific revision.</summary>
		public static List<WikiPageHist> GetByTitleNoPageContent(string pageTitle){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiPageHist>>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			string command="SELECT WikiPageNum,UserNum,PageTitle,'' PageContent,DateTimeSaved,IsDeleted "
				+"FROM wikipagehist WHERE PageTitle='"+POut.String(pageTitle)+"' ORDER BY DateTimeSaved";
			return Crud.WikiPageHistCrud.SelectMany(command);
		}

		public static string GetPageContent(long wikiPageNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),wikiPageNum);
			}
			if(wikiPageNum<1) {
				return "";
			}
			string command="SELECT PageContent FROM wikipagehist WHERE WikiPageNum="+POut.Long(wikiPageNum);
			return Db.GetScalar(command);
		}

		///<summary></summary>
		public static List<string> GetDeletedPages(string searchText,bool ignoreContent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),searchText,ignoreContent);
			}
			List<string> retVal=new List<string>();
			DataTable tableResults=new DataTable();
			DataTable tableNewestDateTimes=new DataTable();
			string[] searchTokens = searchText.Split(' ');
			string command="";
			command="SELECT PageTitle, MAX(DateTimeSaved) AS DateTimeSaved FROM wikipagehist GROUP BY PageTitle";
			tableNewestDateTimes=Db.GetTable(command);
			command=
				"SELECT PageTitle,DateTimeSaved FROM wikipagehist "
				// \_ represents a literal _ because _ has a special meaning in LIKE clauses.
				//The second \ is just to escape the first \.  The other option would be to pass the \ through POut.String.
				+"WHERE PageTitle NOT LIKE '\\_%' ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+="AND PageTitle LIKE '%"+POut.String(searchTokens[i])+"%' ";
			}
			command+=
				"AND PageTitle NOT IN (SELECT PageTitle FROM wikipage WHERE IsDraft=0) "//ignore pages that were re-added after they were deleted
				+"AND IsDeleted=1 "
				+"ORDER BY PageTitle";
			tableResults=Db.GetTable(command);
			for(int i=0;i<tableResults.Rows.Count;i++) {
				if(retVal.Contains(tableResults.Rows[i]["PageTitle"].ToString())) {
					//already found this page
					continue;
				}
				for(int j=0;j<tableNewestDateTimes.Rows.Count;j++) {
					if(tableNewestDateTimes.Rows[j]["PageTitle"].ToString()!=tableResults.Rows[i]["PageTitle"].ToString()) {
						//not the right page
						continue;
					}
					if(tableNewestDateTimes.Rows[j]["DateTimeSaved"].ToString()!=tableResults.Rows[i]["DateTimeSaved"].ToString()) {
						//not the right DateTimeSaved
						continue;
					}
					//This page is both deleted and there are no newer revisions of the page exist
					retVal.Add(tableResults.Rows[i]["PageTitle"].ToString());
					break;
				}
			}
			//Match Content Second-----------------------------------------------------------------------------------
			if(!ignoreContent) {
				command=
					"SELECT PageTitle,DateTimeSaved FROM wikipagehist "
					+"WHERE PageTitle NOT LIKE '\\_%' ";
				for(int i=0;i<searchTokens.Length;i++) {
					command+="AND PageContent LIKE '%"+POut.String(searchTokens[i])+"%' ";
				}
				command+=
					"AND PageTitle NOT IN (SELECT PageTitle FROM wikipage WHERE IsDraft=0) "//ignore pages that exist again...
					+"AND IsDeleted=1 "
					+"ORDER BY PageTitle";
				tableResults=Db.GetTable(command);
				for(int i=0;i<tableResults.Rows.Count;i++) {
					if(retVal.Contains(tableResults.Rows[i]["PageTitle"].ToString())) {
						//already found this page
						continue;
					}
					for(int j=0;j<tableNewestDateTimes.Rows.Count;j++) {
						if(tableNewestDateTimes.Rows[j]["PageTitle"].ToString()!=tableResults.Rows[i]["PageTitle"].ToString()) {
							//not the right page
							continue;
						}
						if(tableNewestDateTimes.Rows[j]["DateTimeSaved"].ToString()!=tableResults.Rows[i]["DateTimeSaved"].ToString()) {
							//not the right DateTimeSaved
							continue;
						}
						//This page is both deleted and there are no newer revisions of the page exist
						retVal.Add(tableResults.Rows[i]["PageTitle"].ToString());
						break;
					}
				}
			}
			return retVal;
		}

		///<summary>Only returns the most recently deleted version of the page. Returns null if not found.</summary>
		public static WikiPageHist GetDeletedByTitle(string pageTitle) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<WikiPageHist>(MethodBase.GetCurrentMethod(),pageTitle);
			}
			string command="SELECT * FROM wikipagehist "
										+"WHERE PageTitle = '"+POut.String(pageTitle)+"' "
										+"AND IsDeleted=1 "
										+"AND DateTimeSaved="
											+"(SELECT MAX(DateTimeSaved) "
											+"FROM wikipagehist "
											+"WHERE PageTitle = '"+POut.String(pageTitle)+"' "
											+"AND IsDeleted=1)"
											;
			return Crud.WikiPageHistCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(WikiPageHist wikiPageHist){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				wikiPageHist.WikiPageNum=Meth.GetLong(MethodBase.GetCurrentMethod(),wikiPageHist);
				return wikiPageHist.WikiPageNum;
			}
			return Crud.WikiPageHistCrud.Insert(wikiPageHist);
		}

		public static WikiPage RevertFrom(WikiPageHist wikiPageHist) {
			//Get the existing WikiPage to ensure the WikiPageNum is preserved for links.
			//See JobNum 4429 for the job that made this necessary.
			WikiPage retVal=WikiPages.GetByTitle(wikiPageHist.PageTitle);
			if(retVal==null) {
				retVal=new WikiPage();
			}
			//retVal.WikiPageNum
			//retVal.UserNum
			retVal.PageTitle=wikiPageHist.PageTitle;
			retVal.PageContent=wikiPageHist.PageContent;
			retVal.KeyWords="";
			Match m=Regex.Match(wikiPageHist.PageContent,@"\[\[(keywords:).*?\]\]");
			if(m.Length>0) {
				retVal.KeyWords=m.Value.Substring(11).TrimEnd(']');
			}
			//retVal.DateTimeSaved=DateTime.Now;//gets set when inserted.
			return retVal;
		}



		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one WikiPageHist from the db.</summary>
		public static WikiPageHist GetOne(long wikiPageNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<WikiPageHist>(MethodBase.GetCurrentMethod(),wikiPageNum);
			}
			return Crud.WikiPageHistCrud.SelectOne(wikiPageNum);
		}

		///<summary></summary>
		public static void Update(WikiPageHist wikiPageHist){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPageHist);
				return;
			}
			Crud.WikiPageHistCrud.Update(wikiPageHist);
		}

		///<summary></summary>
		public static void Delete(long wikiPageNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiPageNum);
				return;
			}
			string command= "DELETE FROM wikipagehist WHERE WikiPageNum = "+POut.Long(wikiPageNum);
			Db.NonQ(command);
		}
		*/
	}
}