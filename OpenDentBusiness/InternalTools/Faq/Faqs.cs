using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary>This class requires that you switch database context to the manual publisher database (jordans_mp on server 174). This should be done by using the DataAction pattern.</summary>
	public class Faqs {
		private static int _versionStable=0;

		///<summary>Returns a list of Faq's that contain the given question text.  Does not have to be an exact match.</summary>
		public static List<Faq> GetByQuestion(string questionText){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Faq>>(MethodBase.GetCurrentMethod(),questionText);
			}
			string command=$"SELECT * FROM faq WHERE QuestionText LIKE '%{POut.String(questionText)}%'";
			List<Faq> retVal=new List<Faq>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Crud.FaqCrud.SelectMany(command);
			});
			return retVal;
		}
		
		///<summary>Gets one Faq from the db.</summary>
		public static Faq GetOne(long faqNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<Faq>(MethodBase.GetCurrentMethod(),faqNum);
			}
			Faq retVal=null;
			DataAction.RunManualPublisherHQ(() => {
				retVal=Crud.FaqCrud.SelectOne(faqNum);
			});
			return retVal;
		}

		public static List<Faq> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<Faq>>(MethodBase.GetCurrentMethod());
			}
			string command=$"SELECT * FROM faq";
			List<Faq> retVal=new List<Faq>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Crud.FaqCrud.SelectMany(command);
			});
			return retVal;
		}

		///<summary>Returns a list of Faq objects containing the given manualPageName.  Does not have to be an exact match.</summary>
		public static List<Faq> GetForPageName(string manualPageName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Faq>>(MethodBase.GetCurrentMethod(),manualPageName);
			}
			string command=$@"
				SELECT faq.*
				FROM manualpage
				INNER JOIN faqmanualpagelink ON manualpage.ManualPageNum=faqmanualpagelink.ManualPageNum
				INNER JOIN faq ON faqmanualpagelink.FaqNum=faq.FaqNum
				WHERE manualpage.FileName LIKE '%{manualPageName}%'
				GROUP BY faqmanualpagelink.FaqNum
			";
			List<Faq> retVal=new List<Faq>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Crud.FaqCrud.SelectMany(command);
			});
			return retVal;
		}

		///<summary>Returns a single manualPageNum for the given manualPageName and manualVersion.</summary>
		public static long GetOneManualPageNum(string manualPageName,int manualVersion) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),manualPageName,manualVersion);
			}
			string command=$@"
				SELECT ManualPageNum
				FROM manualpage
				WHERE manualpage.FileName='{manualPageName}'
				AND VersionOd={manualVersion}
			";
			long manualPageNum=0;
			DataAction.RunManualPublisherHQ(() => {
				manualPageNum=Db.GetLong(command);
			});
			return manualPageNum;
		}

		///<summary>Returns a list of manual page names that the given FaqNum is currently linked to.</summary>
		public static List<string> GetLinkedManualPages(long faqNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),faqNum);
			}
			string command=$@"
				SELECT manualpage.FileName
				FROM faqmanualpagelink
				INNER JOIN manualpage ON manualpage.ManualPageNum=faqmanualpagelink.ManualPageNum
				WHERE faqmanualpagelink.FaqNum={faqNum}
			";
			//This list will typically be 1, but sometimes a handful if the FAQ is used by multiple pages.
			List<string> retVal=new List<string>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Db.GetListString(command);
			});
			return retVal;
		}

		///<summary>Grabs FaqNums linked to invalid ManualPageNums and returns a list of ManualPage objects</summary>
		public static List<ManualPage> GetListUnlinkedFaq() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ManualPage>>(MethodBase.GetCurrentMethod());
			}
			string command=$@"
				SELECT faqmanualpagelink.FaqNum, faqmanualpagelink.ManualPageNum
				FROM faqmanualpagelink
				LEFT JOIN manualpage ON manualpage.ManualPageNum=faqmanualpagelink.ManualPageNum
				WHERE manualpage.FileName IS NULL GROUP BY faqnum";
			DataTable table=new DataTable();
			List<ManualPage> listFaqHelpers=new List<ManualPage>();
			DataAction.RunManualPublisherHQ(() => {
				table=Db.GetTable(command);
			});
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow row=table.Rows[i];
				ManualPage faqHelper=new ManualPage();
				faqHelper.FaqNum=PIn.Long(row["FaqNum"].ToString());
				faqHelper.ManualPageNum=PIn.Long(row["ManualPageNum"].ToString());
				listFaqHelpers.Add(faqHelper);
			}
			return listFaqHelpers;
		}
		
		///<summary>Gets all manual page names from database. Excludes entries in the manualpage table that are used for the manual index.</summary>
		public static List<string> GetAllManualPageNamesForVersions(List<int> listVersions) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			string command=$@"
				SELECT manualpage.FileName
				FROM manualpage
				WHERE FileName NOT LIKE '+%' ";
			if(!listVersions.IsNullOrEmpty()) {
				command+=$"AND VersionOd IN ({ string.Join(",",listVersions) }) ";
			}
			command+="ORDER BY manualpage.FileName;";
			List<string> retVal=new List<string>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Db.GetListString(command).Distinct().ToList();
			});
			return retVal;
		}

		///<summary>Gets all Faq's associated to the given manual page name and version, filtered to the date range specified by dateFrom and dateTo. 
		///If version is blank will query for all versions.</summary>
		public static List<Faq> GetAllForNameAndVersion(string pageName,int version,DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Faq>>(MethodBase.GetCurrentMethod(),pageName,version,dateFrom,dateTo);
			}
			string command=$@"
			SELECT faq.*
			FROM faqmanualpagelink
			INNER JOIN manualpage ON manualpage.ManualPageNum=faqmanualpagelink.ManualPageNum
			INNER JOIN faq ON faq.FaqNum=faqmanualpagelink.FaqNum
			WHERE manualpage.FileName LIKE '%{POut.String(pageName)}%' 
			AND DATE(faq.DateTEntry) >= {POut.Date(dateFrom)} AND DATE(faq.DateTEntry) <= {POut.Date(dateTo)}";
			if(version>0) {
				command+=$"AND faq.ManualVersion={POut.Int(version)}";
			}
			List<Faq> retVal=new List<Faq>();
			DataAction.RunManualPublisherHQ(() => {
				retVal=Crud.FaqCrud.SelectMany(command);
			});
			return retVal;
		}

		///<summary>Gets the 'stable' version to the manual publisher. This will always be in the form '183', '184', '191'. If error occurs, returns the current program version.</summary>
		public static int GetStableManualVersion() {
			try {
				if(_versionStable==0) {
					_versionStable=PIn.Int(WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetStableManualVersion());
				}
				return _versionStable;
			}
			catch(Exception ex) {
				ex.DoNothing();
				//Error getting stable manual version from HQ. Default to current program version
				//Program version is in the format 20.1.3.0
				Version versionProg=new Version(PrefC.GetStringNoCache(PrefName.ProgramVersion));
				return PIn.Int($"{versionProg.Major}{versionProg.Minor }");
			}
		}
		
		/// <summary>Gets manualpage FileNames associated to each Faq and stores them in the ManualPageName column for each Faq.</summary>
		public static List<Faq> GetManualPageNamesForFaqs(List<Faq> listFaqs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Faq>>(MethodBase.GetCurrentMethod(),listFaqs);
			}
			if(listFaqs.Count==0) {
				return listFaqs;
			}
			List<long> listFaqNums=listFaqs.Select(x => x.FaqNum).Distinct().ToList();
			string command=@"
				SELECT FaqNum, FileName 
				FROM faqmanualpagelink 
				INNER JOIN manualpage ON manualpage.ManualPageNum=faqmanualpagelink.ManualPageNum 
				WHERE faqmanualpagelink.FaqNum IN ("+POut.String(string.Join(",",listFaqNums))+") "+
				"ORDER BY faqmanualpagelink.FaqNum";
			//If there is one faq attached to two pages, there will be two rows.
			DataTable table=new DataTable();
			DataAction.RunManualPublisherHQ(() => {
				table=Db.GetTable(command);
			});
			//The list of faqs is missing the ManualPageName for each item.
			//If there is one faq attached to two pages, there will be two identical rows.
			for(int i=0;i<table.Rows.Count;i++) {
				Faq faq=listFaqs.Find(x => x.ManualPageName==null //ignore the one we already filled
					&& x.FaqNum==PIn.Long(table.Rows[i]["FaqNum"].ToString()));
				if(faq!=null) {
					faq.ManualPageName=table.Rows[i]["FileName"].ToString();
				}
			}
			return listFaqs;
		}

		///<summary></summary>
		public static long Insert(Faq faq){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				faq.FaqNum=Meth.GetLong(MethodBase.GetCurrentMethod(),faq);
				return faq.FaqNum;
			}
			long faqNum=0;
			DataAction.RunManualPublisherHQ(() => {
				faqNum=Crud.FaqCrud.Insert(faq);
			});
			return faqNum;
		}

		///<summary>Inserts a row into jordans_mp.faqmanualpagelink for each of the specified pages to link the given FaqNum to.
		///This linker table allows a many to many relationship between faq objects and manualpages i.e.- a manual page can have many faq's
		///and faq's can be linked to many manual pages. Only links to the manualpage of the specified version.</summary>
		public static void CreateManualPageLinks(long faqNum,List<string> listPagesToLink,int manualVersion) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),faqNum,listPagesToLink,manualVersion);
				return;
			}
			DataAction.RunManualPublisherHQ(() => {
				foreach(string page in listPagesToLink) {
					long manualPageNum=GetOneManualPageNum(page,manualVersion);
					if(manualPageNum!=0) {
						Db.NonQ($@"INSERT INTO faqmanualpagelink(ManualPageNum,FaqNum) VALUES ({manualPageNum},{faqNum})");
					}
				}
			});
		}
	
		///<summary></summary>
		public static void Update(Faq faq){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),faq);
				return;
			}
			DataAction.RunManualPublisherHQ(() => {
				Crud.FaqCrud.Update(faq);
			});
		}
		
		///<summary></summary>
		public static void Delete(long faqNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),faqNum);
				return;
			}
			DataAction.RunManualPublisherHQ(() => {
				Crud.FaqCrud.Delete(faqNum);
			});
		}

		///<summary>Deletes all faqmanualpagelink rows that have the given faqNum</summary>
		public static void DeleteManualPageLinkForFaqNum(long faqNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),faqNum);
				return;
			}
			string command=$@"DELETE FROM faqmanualpagelink WHERE FaqNum={faqNum}";
			DataAction.RunManualPublisherHQ(() => {
				Db.NonQ(command);
			});
		}
	}

	///<summary>Corresponds to manualpage table in jordans_mp database.</summary>
	public class ManualPage {
		///<summary>FileName in manualpage table. Used in the cache.</summary>
		public string FileName;
		///<summary>Version that the manualpage and FAQ are linked to. Used in the cache.</summary>
		public string VersionOd;
		///<summary>WinFormName associated with the FileName and Version. Used in the cache.</summary>
		public string WinFormName;
		///<summary>Normally not set unless "Show Unlinked FAQs" is checked in FormFaqPicker.cs</summary>
		public long FaqNum;
		///<summary>Normally not set unless "Show Unlinked FAQs" is checked in FormFaqPicker.cs</summary>
		public long ManualPageNum;
	}
}