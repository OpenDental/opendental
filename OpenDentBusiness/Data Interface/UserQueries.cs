using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeBase;

namespace OpenDentBusiness{

///<summary></summary>
	public class UserQueries{	
		///<summary>List of commands that modify a database. Typically used to parse out modification queries for MassEmail user queries.</summary>
		public static List<string> ListMassEmailBlacklistCmds=new List<string> {
			"INSERT",
			"DELETE",
			"ALTER",
			"DROP",
			"ADD",
			"BACKUP",
			"COLUMN",
			"CREATE",
			"SET",
			"UPDATE",
			"TRUNCATE",
		};

		///<summary>Returns the list of variables in the query contained within the passed-in SET statement.
		///Pass in one SET statement. Used in conjunction with GetListVals.</summary>
		public static List<QuerySetStmtObject> GetListQuerySetStmtObjs(string setStmt) {
			Meth.NoCheckMiddleTierRole();
			List<string> listStrSplits=SplitQuery(setStmt,false,",");
			for(int i=0;i < listStrSplits.Count;i++) {
				Regex regex=new Regex(@"\s*set\s+",RegexOptions.IgnoreCase);
				listStrSplits[i]=regex.Replace(listStrSplits[i],"");
			}
			TrimList(listStrSplits);
			listStrSplits.RemoveAll(x => string.IsNullOrWhiteSpace(x) || !x.StartsWith("@") || x.StartsWith("@_"));
			List<QuerySetStmtObject> listQuerySetStmtObjects = new List<QuerySetStmtObject>();
			for(int i=0;i < listStrSplits.Count;i++) {
				QuerySetStmtObject querySetStmtObject = new QuerySetStmtObject();
				querySetStmtObject.Stmt=setStmt;
				querySetStmtObject.Variable=listStrSplits[i].Split(new char[] { '=' },2).First();
				querySetStmtObject.Value=listStrSplits[i].Split(new char[] { '=' },2).Last();
				listQuerySetStmtObjects.Add(querySetStmtObject);
			}
			return listQuerySetStmtObjects;
		}

		///<summary>Splits the given query string on the passed-in split string parameters. 
		///DOES NOT split on the split strings when within single quotes, double quotes, parans, or case/if/concat statements.</summary>
		public static List<string> SplitQuery(string strQuery,bool includeDelimeters=false,params string[] listSplitStrs) {
			Meth.NoCheckMiddleTierRole();
			List<string> listStrSplits = new List<string>(); //returned list of strings.
			string strTotal = "";
			char charQuoteMode = '-'; //tracks whether we are currently within quotes.
			Stack<string> stackFuncs = new Stack<string>(); //tracks whether we are currently within a CASE, IF, or CONCAT statement.
			foreach(char c in strQuery) {//jordan ok to leave foreach
				if(charQuoteMode != '-') {
					if(c == charQuoteMode) {
						charQuoteMode='-';
					}
					strTotal+=c;
				}
				else if(stackFuncs.Count > 0) {
					if((strTotal + c).ToLower().EndsWith("case")) {
						stackFuncs.Push("end");
					}
					else if((strTotal + c).ToLower().EndsWith("(")) {
						stackFuncs.Push(")");
					}
					else if((strTotal + c).ToLower().EndsWith(stackFuncs.Peek())) {
						stackFuncs.Pop();
					}
					if(c.In('\'','"')) {
						//Function has quotes. Set quote mode.
						charQuoteMode=c;
					}
					//Only split string if we are not in quote mode and not in a function.
					if(charQuoteMode=='-' && stackFuncs.Count==0 && listSplitStrs.Contains(c.ToString())) {
						AddTotalStrToList(c,includeDelimeters,ref strTotal,ref listStrSplits);
					}
					else {
						strTotal+=c;
					}
				}
				else {
					if((strTotal + c).ToLower().EndsWith("case")) {
						stackFuncs.Push("end");
						strTotal+=c;
					}
					else if((strTotal + c).ToLower().EndsWith("(")) {
						stackFuncs.Push(")");
						strTotal+=c;
					}
					else if(listSplitStrs.Contains(c.ToString())) {
						AddTotalStrToList(c,includeDelimeters,ref strTotal,ref listStrSplits);
					}
					else {
						if(c == '\'' || c =='"') {
							charQuoteMode = c;
						}
						strTotal+=c;
					}
				}
			}
			listStrSplits.Add(strTotal);
			return listStrSplits;
		}

		///<summary>Adds the totalStr to the listStrSplit passed in and then clears the totalStr.  Sets totalStr to the delimeter if includeDelimeters
		///is true.</summary>
		private static void AddTotalStrToList(char c,bool includeDelimeters,ref string strTotal,ref List<string> listStrSplits) {
			if(includeDelimeters) {
				strTotal+=c;
			}
			listStrSplits.Add(strTotal);
			strTotal="";
		}

		///<summary>Returns a string with SQL comments removed.
		///E.g. removes /**/ and -- SQL comments from the query passed in.</summary>
		public static string RemoveSQLComments(string queryText) {
			Meth.NoCheckMiddleTierRole();
			Regex regexBlockComments = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/");
			Regex regexLineComments = new Regex(@"--.*");
			string queryNoComments = regexBlockComments.Replace(queryText,"");
			queryNoComments = regexLineComments.Replace(queryNoComments,"");
			return queryNoComments;
		}

		///<summary>Helper method to remove leading and trailing spaces from every element in a list of strings.</summary>
		public static void TrimList(List<string> listTrims) {
			for(int i = 0;i < listTrims.Count;i++) {
				listTrims[i]=listTrims[i].Trim();
			}
		}

		///<summary>Takes the passed-in query text and returns a list of SET statements within the query. Pass in the entire query.</summary>
		public static List<string> ParseSetStatements(string queryText) {
			Meth.NoCheckMiddleTierRole();
			queryText=RemoveSQLComments(queryText);
			List<string> listParsedSetStmts=new List<string>();//Returned list of set statements.
			List<String> listSplitQueries=SplitQuery(queryText,true,";");
			for(int i=0;i<listSplitQueries.Count;i++) {
				//The list of set statements returned from SplitQuery will include the delimiter(";"). Split each of the set statements using the c# splitter 
				//with the delimiter ";" again incase the query's set statements have invalid apostrophes. We can do this because we don't allow users to enter
				//";" inside a SET statement value.
				listParsedSetStmts.AddRange(listSplitQueries[i].Split(";",StringSplitOptions.RemoveEmptyEntries));
			}
			TrimList(listParsedSetStmts);
			listParsedSetStmts.RemoveAll(x => string.IsNullOrEmpty(x));
			listParsedSetStmts=listParsedSetStmts.FindAll(x => x.ToLower().StartsWith("set "));
			return listParsedSetStmts;
		}

		///<summary>Takes in a string and returns true if it is safe to run in mass email user queries.</summary>
		public static bool ValidateQueryForMassEmail(string command) {
			Meth.NoCheckMiddleTierRole();
			command=RemoveSQLComments(command);
			Regex regexSchema=new Regex(@"SELECT.*[PatNum|\*].*FROM.*",RegexOptions.Singleline|RegexOptions.IgnoreCase);
			Regex regexBlacklist=new Regex(@$"\b({string.Join("|",ListMassEmailBlacklistCmds.Select(x => POut.String(x)))})\b",RegexOptions.IgnoreCase);
			if(!regexSchema.IsMatch(command)) {
				return false;
			}
			if(regexBlacklist.IsMatch(command)) {
				return false;
			}
			return true;
		}
	
		#region CachePattern

		private class UserQueryCache:CacheListAbs<UserQuery> {
			protected override UserQuery Copy(UserQuery userQuery) {
				return userQuery.Copy();
			}

			protected override void FillCacheIfNeeded() {
				UserQueries.GetTableFromCache(false);
			}

			protected override List<UserQuery> GetCacheFromDb() {
				string command="SELECT * FROM userquery ORDER BY description";
				return Crud.UserQueryCrud.SelectMany(command);
			}

			protected override DataTable ListToTable(List<UserQuery> listUserQueries) {
				return Crud.UserQueryCrud.ListToTable(listUserQueries,"UserQuery");
			}

			protected override List<UserQuery> TableToList(DataTable table) {
				return Crud.UserQueryCrud.TableToList(table);
			}

			protected override bool IsInListShort(UserQuery userQuery) {
				return  userQuery.IsReleased;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserQueryCache _userQueryCache=new UserQueryCache();

		public static List<UserQuery> GetDeepCopy(bool isShort=false) {
			return _userQueryCache.GetDeepCopy(isShort);
		}

		public static List<UserQuery> GetWhere(Predicate<UserQuery> match,bool isShort=false) {
			return _userQueryCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_userQueryCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool refreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),refreshCache);
				_userQueryCache.FillCacheFromTable(table);
				return table;
			}
			return _userQueryCache.GetTableFromCache(refreshCache);
		}

		public static void ClearCache() {
			_userQueryCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static long Insert(UserQuery userQuery){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				userQuery.QueryNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userQuery);
				return userQuery.QueryNum;
			}
			return Crud.UserQueryCrud.Insert(userQuery);
		}
		
		///<summary></summary>
		public static void Delete(UserQuery userQuery){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userQuery);
				return;
			}
			string command = "DELETE from userquery WHERE querynum = '"+POut.Long(userQuery.QueryNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Update(UserQuery userQuery){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userQuery);
				return;
			}
			Crud.UserQueryCrud.Update(userQuery);
		}
	}

	///<summary>A tiny class that contains a single SET statement's variable, value, and the entire statement.</summary>
	public class QuerySetStmtObject {
		public string Variable;
		public string Value;
		public string Stmt;
	}
}