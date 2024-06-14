using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WikiListHists {		
		///<summary>Ordered by dateTimeSaved.  Case insensitive.  Objects will not have the ListHeaders and ListContent fields populated.  Use SelectOne
		///to get the headers and content for a specific revision.</summary>
		public static List<WikiListHist> GetByNameNoContent(string listName){
			if(string.IsNullOrEmpty(listName)) {
				return new List<WikiListHist>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiListHist>>(MethodBase.GetCurrentMethod(),listName);
			}
			string command=$@"SELECT WikiListHistNum,UserNum,ListName,'' ListHeaders,'' ListContent,DateTimeSaved
				FROM wikilisthist WHERE ListName='{POut.String(listName)}' ORDER BY DateTimeSaved";
			return Crud.WikiListHistCrud.SelectMany(command);
		}

		public static WikiListHist SelectOne(long wikiListHistNum) {
			if(wikiListHistNum<=0) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WikiListHist>(MethodBase.GetCurrentMethod(),wikiListHistNum);
			}
			return Crud.WikiListHistCrud.SelectOne(wikiListHistNum);
		}

		///<summary></summary>
		public static long Insert(WikiListHist wikiListHist) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				wikiListHist.WikiListHistNum=Meth.GetLong(MethodBase.GetCurrentMethod(),wikiListHist);
				return wikiListHist.WikiListHistNum;
			}
			return Crud.WikiListHistCrud.Insert(wikiListHist);
		}

		///<summary>Deletes all WikiListHists before the given cutoff date. Returns the number of entries deleted.</summary>
		public static long DeleteBeforeDate(DateTime dateCutoff) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),dateCutoff);
			}
			string command=$"Delete FROM wikilisthist WHERE DateTimeSaved <= {POut.DateT(dateCutoff)} ";
			return Db.NonQ(command);
		}

		///<summary>Does not save to DB. Return null if listName does not exist.
		///Pass in the userod.UserNum of the user that is making the change.  Typically Security.CurUser.UserNum.
		///Security.CurUser cannot be used within this method due to the server side of middle tier.</summary>
		public static WikiListHist GenerateFromName(string listName,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WikiListHist>(MethodBase.GetCurrentMethod(),listName,userNum);
			}
			if(!WikiLists.CheckExists(listName)) {
				return null;
			}
			WikiListHist wikiListHist=new WikiListHist();
			wikiListHist.UserNum=userNum;
			wikiListHist.ListName=listName;
			wikiListHist.DateTimeSaved=DateTime.Now;
			DataTable table=WikiLists.GetByName(listName);
			table.TableName=listName;//required for xmlwriter
			using(var writer=new StringWriter()) {
				table.WriteXml(writer,XmlWriteMode.WriteSchema);
				wikiListHist.ListContent=writer.ToString();
			}
			List<WikiListHeaderWidth> listWikiListHeaderWidths=WikiListHeaderWidths.GetForList(listName);
			if(listWikiListHeaderWidths.Count>0) {
				wikiListHist.ListHeaders=string.Join(";",listWikiListHeaderWidths.Select(x => x.ColName+","+x.ColWidth));
			}
			return wikiListHist;
		}

		///<summary>Drops table in DB.  Recreates Table, then fills with Data.
		///Pass in the userod.UserNum of the user that is making the change.  Typically Security.CurUser.UserNum.</summary>
		public static void RevertFrom(WikiListHist wikiListHist,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiListHist,userNum);
				return;
			}
			if(!WikiLists.CheckExists(wikiListHist.ListName)) {
				return;//should never happen.
			}
			Insert(GenerateFromName(wikiListHist.ListName,userNum));//Save current wiki list content to the history
			List<WikiListHeaderWidth> listWikiListHeaderWidths=WikiListHeaderWidths.GetFromListHist(wikiListHist);//Load header data
			WikiLists.CreateNewWikiList(wikiListHist.ListName,listWikiListHeaderWidths,true);//dropTableIfExists=true, so the existing table and HeaderWidth rows will be dropped
			DataTable tableRevertedContent=new DataTable();
			using(StringReader sr=new StringReader(wikiListHist.ListContent))
			using(XmlReader xmlReader=XmlReader.Create(sr)) {
				tableRevertedContent.ReadXml(xmlReader);
			}
			string commandStart=$@"INSERT INTO wikilist_{POut.String(wikiListHist.ListName)} ({string.Join(",",listWikiListHeaderWidths.Select(x => POut.String(x.ColName)))})
				VALUES ";
			StringBuilder stringBuilder=new StringBuilder(commandStart);
			string strComma="";
			for(int i=0;i<tableRevertedContent.Rows.Count;i++) {
				string strRow="("+string.Join(",",tableRevertedContent.Rows[i].ItemArray.Select(x => "'"+POut.String(x.ToString())+"'"))+")";
				if(stringBuilder.Length+strRow.Length+1>TableBase.MaxAllowedPacketCount) {
					Db.NonQ(stringBuilder.ToString());
					stringBuilder=new StringBuilder(commandStart);
					strComma="";
				}
				stringBuilder.Append(strComma+strRow);
				strComma=",";
				if(i==tableRevertedContent.Rows.Count-1) {
					Db.NonQ(stringBuilder.ToString());
				}
			}
		}

		///<summary>Checks remoting roles. Does not check permissions. Does not check for existing listname. If listname already exists it will "merge" the history.</summary>
		public static void Rename(string wikiListCurName,string wikiListNewName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiListCurName,wikiListNewName);
				return;
			}
			string command="UPDATE wikilisthist SET ListName = '"+POut.String(wikiListNewName)+"' WHERE ListName='"+POut.String(wikiListCurName)+"'";
			Db.NonQ(command);
		}
	}
}