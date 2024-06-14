//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class BugSubmissionCrud {
		///<summary>Gets one BugSubmission object from the database using the primary key.  Returns null if not found.</summary>
		public static BugSubmission SelectOne(long bugSubmissionNum) {
			string command="SELECT * FROM bugsubmission "
				+"WHERE BugSubmissionNum = "+POut.Long(bugSubmissionNum);
			List<BugSubmission> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one BugSubmission object from the database using a query.</summary>
		public static BugSubmission SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<BugSubmission> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of BugSubmission objects from the database using a query.</summary>
		public static List<BugSubmission> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<BugSubmission> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<BugSubmission> TableToList(DataTable table) {
			List<BugSubmission> retVal=new List<BugSubmission>();
			BugSubmission bugSubmission;
			foreach(DataRow row in table.Rows) {
				bugSubmission=new BugSubmission();
				bugSubmission.BugSubmissionNum    = PIn.Long  (row["BugSubmissionNum"].ToString());
				bugSubmission.SubmissionDateTime  = PIn.DateT (row["SubmissionDateTime"].ToString());
				bugSubmission.BugId               = PIn.Long  (row["BugId"].ToString());
				bugSubmission.RegKey              = PIn.String(row["RegKey"].ToString());
				bugSubmission.DbVersion           = PIn.String(row["DbVersion"].ToString());
				bugSubmission.ExceptionMessageText= PIn.String(row["ExceptionMessageText"].ToString());
				bugSubmission.ExceptionStackTrace = PIn.String(row["ExceptionStackTrace"].ToString());
				bugSubmission.DbInfoJson          = PIn.String(row["DbInfoJson"].ToString());
				bugSubmission.DevNote             = PIn.String(row["DevNote"].ToString());
				bugSubmission.IsHidden            = PIn.Bool  (row["IsHidden"].ToString());
				bugSubmission.CategoryTags        = PIn.String(row["CategoryTags"].ToString());
				bugSubmission.BugSubmissionHashNum= PIn.Long  (row["BugSubmissionHashNum"].ToString());
				retVal.Add(bugSubmission);
			}
			return retVal;
		}

		///<summary>Converts a list of BugSubmission into a DataTable.</summary>
		public static DataTable ListToTable(List<BugSubmission> listBugSubmissions,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="BugSubmission";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("BugSubmissionNum");
			table.Columns.Add("SubmissionDateTime");
			table.Columns.Add("BugId");
			table.Columns.Add("RegKey");
			table.Columns.Add("DbVersion");
			table.Columns.Add("ExceptionMessageText");
			table.Columns.Add("ExceptionStackTrace");
			table.Columns.Add("DbInfoJson");
			table.Columns.Add("DevNote");
			table.Columns.Add("IsHidden");
			table.Columns.Add("CategoryTags");
			table.Columns.Add("BugSubmissionHashNum");
			foreach(BugSubmission bugSubmission in listBugSubmissions) {
				table.Rows.Add(new object[] {
					POut.Long  (bugSubmission.BugSubmissionNum),
					POut.DateT (bugSubmission.SubmissionDateTime,false),
					POut.Long  (bugSubmission.BugId),
					            bugSubmission.RegKey,
					            bugSubmission.DbVersion,
					            bugSubmission.ExceptionMessageText,
					            bugSubmission.ExceptionStackTrace,
					            bugSubmission.DbInfoJson,
					            bugSubmission.DevNote,
					POut.Bool  (bugSubmission.IsHidden),
					            bugSubmission.CategoryTags,
					POut.Long  (bugSubmission.BugSubmissionHashNum),
				});
			}
			return table;
		}

		///<summary>Inserts one BugSubmission into the database.  Returns the new priKey.</summary>
		public static long Insert(BugSubmission bugSubmission) {
			return Insert(bugSubmission,false);
		}

		///<summary>Inserts one BugSubmission into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(BugSubmission bugSubmission,bool useExistingPK) {
			string command="INSERT INTO bugsubmission (";
			if(useExistingPK) {
				command+="BugSubmissionNum,";
			}
			command+="SubmissionDateTime,BugId,RegKey,DbVersion,ExceptionMessageText,ExceptionStackTrace,DbInfoJson,DevNote,IsHidden,CategoryTags,BugSubmissionHashNum) VALUES(";
			if(useExistingPK) {
				command+=POut.Long(bugSubmission.BugSubmissionNum)+",";
			}
			command+=
				     DbHelper.Now()+","
				+    POut.Long  (bugSubmission.BugId)+","
				+"'"+POut.String(bugSubmission.RegKey)+"',"
				+"'"+POut.String(bugSubmission.DbVersion)+"',"
				+"'"+POut.String(bugSubmission.ExceptionMessageText)+"',"
				+    DbHelper.ParamChar+"paramExceptionStackTrace,"
				+    DbHelper.ParamChar+"paramDbInfoJson,"
				+    DbHelper.ParamChar+"paramDevNote,"
				+    POut.Bool  (bugSubmission.IsHidden)+","
				+    DbHelper.ParamChar+"paramCategoryTags,"
				+    POut.Long  (bugSubmission.BugSubmissionHashNum)+")";
			if(bugSubmission.ExceptionStackTrace==null) {
				bugSubmission.ExceptionStackTrace="";
			}
			OdSqlParameter paramExceptionStackTrace=new OdSqlParameter("paramExceptionStackTrace",OdDbType.Text,POut.StringParam(bugSubmission.ExceptionStackTrace));
			if(bugSubmission.DbInfoJson==null) {
				bugSubmission.DbInfoJson="";
			}
			OdSqlParameter paramDbInfoJson=new OdSqlParameter("paramDbInfoJson",OdDbType.Text,POut.StringParam(bugSubmission.DbInfoJson));
			if(bugSubmission.DevNote==null) {
				bugSubmission.DevNote="";
			}
			OdSqlParameter paramDevNote=new OdSqlParameter("paramDevNote",OdDbType.Text,POut.StringParam(bugSubmission.DevNote));
			if(bugSubmission.CategoryTags==null) {
				bugSubmission.CategoryTags="";
			}
			OdSqlParameter paramCategoryTags=new OdSqlParameter("paramCategoryTags",OdDbType.Text,POut.StringParam(bugSubmission.CategoryTags));
			if(useExistingPK) {
				Db.NonQ(command,paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
			}
			else {
				bugSubmission.BugSubmissionNum=Db.NonQ(command,true,"BugSubmissionNum","bugSubmission",paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
			}
			return bugSubmission.BugSubmissionNum;
		}

		///<summary>Inserts one BugSubmission into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(BugSubmission bugSubmission) {
			return InsertNoCache(bugSubmission,false);
		}

		///<summary>Inserts one BugSubmission into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(BugSubmission bugSubmission,bool useExistingPK) {
			string command="INSERT INTO bugsubmission (";
			if(useExistingPK) {
				command+="BugSubmissionNum,";
			}
			command+="SubmissionDateTime,BugId,RegKey,DbVersion,ExceptionMessageText,ExceptionStackTrace,DbInfoJson,DevNote,IsHidden,CategoryTags,BugSubmissionHashNum) VALUES(";
			if(useExistingPK) {
				command+=POut.Long(bugSubmission.BugSubmissionNum)+",";
			}
			command+=
				     DbHelper.Now()+","
				+    POut.Long  (bugSubmission.BugId)+","
				+"'"+POut.String(bugSubmission.RegKey)+"',"
				+"'"+POut.String(bugSubmission.DbVersion)+"',"
				+"'"+POut.String(bugSubmission.ExceptionMessageText)+"',"
				+    DbHelper.ParamChar+"paramExceptionStackTrace,"
				+    DbHelper.ParamChar+"paramDbInfoJson,"
				+    DbHelper.ParamChar+"paramDevNote,"
				+    POut.Bool  (bugSubmission.IsHidden)+","
				+    DbHelper.ParamChar+"paramCategoryTags,"
				+    POut.Long  (bugSubmission.BugSubmissionHashNum)+")";
			if(bugSubmission.ExceptionStackTrace==null) {
				bugSubmission.ExceptionStackTrace="";
			}
			OdSqlParameter paramExceptionStackTrace=new OdSqlParameter("paramExceptionStackTrace",OdDbType.Text,POut.StringParam(bugSubmission.ExceptionStackTrace));
			if(bugSubmission.DbInfoJson==null) {
				bugSubmission.DbInfoJson="";
			}
			OdSqlParameter paramDbInfoJson=new OdSqlParameter("paramDbInfoJson",OdDbType.Text,POut.StringParam(bugSubmission.DbInfoJson));
			if(bugSubmission.DevNote==null) {
				bugSubmission.DevNote="";
			}
			OdSqlParameter paramDevNote=new OdSqlParameter("paramDevNote",OdDbType.Text,POut.StringParam(bugSubmission.DevNote));
			if(bugSubmission.CategoryTags==null) {
				bugSubmission.CategoryTags="";
			}
			OdSqlParameter paramCategoryTags=new OdSqlParameter("paramCategoryTags",OdDbType.Text,POut.StringParam(bugSubmission.CategoryTags));
			if(useExistingPK) {
				Db.NonQ(command,paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
			}
			else {
				bugSubmission.BugSubmissionNum=Db.NonQ(command,true,"BugSubmissionNum","bugSubmission",paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
			}
			return bugSubmission.BugSubmissionNum;
		}

		///<summary>Updates one BugSubmission in the database.</summary>
		public static void Update(BugSubmission bugSubmission) {
			string command="UPDATE bugsubmission SET "
				//SubmissionDateTime not allowed to change
				+"BugId               =  "+POut.Long  (bugSubmission.BugId)+", "
				+"RegKey              = '"+POut.String(bugSubmission.RegKey)+"', "
				+"DbVersion           = '"+POut.String(bugSubmission.DbVersion)+"', "
				+"ExceptionMessageText= '"+POut.String(bugSubmission.ExceptionMessageText)+"', "
				+"ExceptionStackTrace =  "+DbHelper.ParamChar+"paramExceptionStackTrace, "
				+"DbInfoJson          =  "+DbHelper.ParamChar+"paramDbInfoJson, "
				+"DevNote             =  "+DbHelper.ParamChar+"paramDevNote, "
				+"IsHidden            =  "+POut.Bool  (bugSubmission.IsHidden)+", "
				+"CategoryTags        =  "+DbHelper.ParamChar+"paramCategoryTags, "
				+"BugSubmissionHashNum=  "+POut.Long  (bugSubmission.BugSubmissionHashNum)+" "
				+"WHERE BugSubmissionNum = "+POut.Long(bugSubmission.BugSubmissionNum);
			if(bugSubmission.ExceptionStackTrace==null) {
				bugSubmission.ExceptionStackTrace="";
			}
			OdSqlParameter paramExceptionStackTrace=new OdSqlParameter("paramExceptionStackTrace",OdDbType.Text,POut.StringParam(bugSubmission.ExceptionStackTrace));
			if(bugSubmission.DbInfoJson==null) {
				bugSubmission.DbInfoJson="";
			}
			OdSqlParameter paramDbInfoJson=new OdSqlParameter("paramDbInfoJson",OdDbType.Text,POut.StringParam(bugSubmission.DbInfoJson));
			if(bugSubmission.DevNote==null) {
				bugSubmission.DevNote="";
			}
			OdSqlParameter paramDevNote=new OdSqlParameter("paramDevNote",OdDbType.Text,POut.StringParam(bugSubmission.DevNote));
			if(bugSubmission.CategoryTags==null) {
				bugSubmission.CategoryTags="";
			}
			OdSqlParameter paramCategoryTags=new OdSqlParameter("paramCategoryTags",OdDbType.Text,POut.StringParam(bugSubmission.CategoryTags));
			Db.NonQ(command,paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
		}

		///<summary>Updates one BugSubmission in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(BugSubmission bugSubmission,BugSubmission oldBugSubmission) {
			string command="";
			//SubmissionDateTime not allowed to change
			if(bugSubmission.BugId != oldBugSubmission.BugId) {
				if(command!="") { command+=",";}
				command+="BugId = "+POut.Long(bugSubmission.BugId)+"";
			}
			if(bugSubmission.RegKey != oldBugSubmission.RegKey) {
				if(command!="") { command+=",";}
				command+="RegKey = '"+POut.String(bugSubmission.RegKey)+"'";
			}
			if(bugSubmission.DbVersion != oldBugSubmission.DbVersion) {
				if(command!="") { command+=",";}
				command+="DbVersion = '"+POut.String(bugSubmission.DbVersion)+"'";
			}
			if(bugSubmission.ExceptionMessageText != oldBugSubmission.ExceptionMessageText) {
				if(command!="") { command+=",";}
				command+="ExceptionMessageText = '"+POut.String(bugSubmission.ExceptionMessageText)+"'";
			}
			if(bugSubmission.ExceptionStackTrace != oldBugSubmission.ExceptionStackTrace) {
				if(command!="") { command+=",";}
				command+="ExceptionStackTrace = "+DbHelper.ParamChar+"paramExceptionStackTrace";
			}
			if(bugSubmission.DbInfoJson != oldBugSubmission.DbInfoJson) {
				if(command!="") { command+=",";}
				command+="DbInfoJson = "+DbHelper.ParamChar+"paramDbInfoJson";
			}
			if(bugSubmission.DevNote != oldBugSubmission.DevNote) {
				if(command!="") { command+=",";}
				command+="DevNote = "+DbHelper.ParamChar+"paramDevNote";
			}
			if(bugSubmission.IsHidden != oldBugSubmission.IsHidden) {
				if(command!="") { command+=",";}
				command+="IsHidden = "+POut.Bool(bugSubmission.IsHidden)+"";
			}
			if(bugSubmission.CategoryTags != oldBugSubmission.CategoryTags) {
				if(command!="") { command+=",";}
				command+="CategoryTags = "+DbHelper.ParamChar+"paramCategoryTags";
			}
			if(bugSubmission.BugSubmissionHashNum != oldBugSubmission.BugSubmissionHashNum) {
				if(command!="") { command+=",";}
				command+="BugSubmissionHashNum = "+POut.Long(bugSubmission.BugSubmissionHashNum)+"";
			}
			if(command=="") {
				return false;
			}
			if(bugSubmission.ExceptionStackTrace==null) {
				bugSubmission.ExceptionStackTrace="";
			}
			OdSqlParameter paramExceptionStackTrace=new OdSqlParameter("paramExceptionStackTrace",OdDbType.Text,POut.StringParam(bugSubmission.ExceptionStackTrace));
			if(bugSubmission.DbInfoJson==null) {
				bugSubmission.DbInfoJson="";
			}
			OdSqlParameter paramDbInfoJson=new OdSqlParameter("paramDbInfoJson",OdDbType.Text,POut.StringParam(bugSubmission.DbInfoJson));
			if(bugSubmission.DevNote==null) {
				bugSubmission.DevNote="";
			}
			OdSqlParameter paramDevNote=new OdSqlParameter("paramDevNote",OdDbType.Text,POut.StringParam(bugSubmission.DevNote));
			if(bugSubmission.CategoryTags==null) {
				bugSubmission.CategoryTags="";
			}
			OdSqlParameter paramCategoryTags=new OdSqlParameter("paramCategoryTags",OdDbType.Text,POut.StringParam(bugSubmission.CategoryTags));
			command="UPDATE bugsubmission SET "+command
				+" WHERE BugSubmissionNum = "+POut.Long(bugSubmission.BugSubmissionNum);
			Db.NonQ(command,paramExceptionStackTrace,paramDbInfoJson,paramDevNote,paramCategoryTags);
			return true;
		}

		///<summary>Returns true if Update(BugSubmission,BugSubmission) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(BugSubmission bugSubmission,BugSubmission oldBugSubmission) {
			//SubmissionDateTime not allowed to change
			if(bugSubmission.BugId != oldBugSubmission.BugId) {
				return true;
			}
			if(bugSubmission.RegKey != oldBugSubmission.RegKey) {
				return true;
			}
			if(bugSubmission.DbVersion != oldBugSubmission.DbVersion) {
				return true;
			}
			if(bugSubmission.ExceptionMessageText != oldBugSubmission.ExceptionMessageText) {
				return true;
			}
			if(bugSubmission.ExceptionStackTrace != oldBugSubmission.ExceptionStackTrace) {
				return true;
			}
			if(bugSubmission.DbInfoJson != oldBugSubmission.DbInfoJson) {
				return true;
			}
			if(bugSubmission.DevNote != oldBugSubmission.DevNote) {
				return true;
			}
			if(bugSubmission.IsHidden != oldBugSubmission.IsHidden) {
				return true;
			}
			if(bugSubmission.CategoryTags != oldBugSubmission.CategoryTags) {
				return true;
			}
			if(bugSubmission.BugSubmissionHashNum != oldBugSubmission.BugSubmissionHashNum) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one BugSubmission from the database.</summary>
		public static void Delete(long bugSubmissionNum) {
			string command="DELETE FROM bugsubmission "
				+"WHERE BugSubmissionNum = "+POut.Long(bugSubmissionNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many BugSubmissions from the database.</summary>
		public static void DeleteMany(List<long> listBugSubmissionNums) {
			if(listBugSubmissionNums==null || listBugSubmissionNums.Count==0) {
				return;
			}
			string command="DELETE FROM bugsubmission "
				+"WHERE BugSubmissionNum IN("+string.Join(",",listBugSubmissionNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}