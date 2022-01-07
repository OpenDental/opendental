using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class DbmLogs {
		#region Get Methods
		///<summary>Gets one DbmLog from the db.</summary>
		public static DbmLog GetOne(long dbmLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DbmLog>(MethodBase.GetCurrentMethod(),dbmLogNum);
			}
			return Crud.DbmLogCrud.SelectOne(dbmLogNum);
		}
		
		///<summary>Gets list of DbmLog from the db for the given methodName and date (all day).</summary>
		public static List<DbmLog> GetByMethodName(string methodName,DateTime date) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DbmLog>>(MethodBase.GetCurrentMethod(),methodName,date);
			}
			string command="SELECT * FROM dbmlog WHERE dbmlog.MethodName='"+POut.String(methodName)+"' AND dbmlog.DateTimeEntry>="+POut.DateT(date.Date);
			return Crud.DbmLogCrud.SelectMany(command);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(DbmLog dbmLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				dbmLog.DbmLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dbmLog);
				return dbmLog.DbmLogNum;
			}
			return Crud.DbmLogCrud.Insert(dbmLog);
		}

		public static void InsertMany(List<DbmLog> listDbmLogs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDbmLogs);
				return;
			}
			Crud.DbmLogCrud.InsertMany(listDbmLogs);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(DbmLog dbmLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dbmLog);
				return;
			}
			Crud.DbmLogCrud.Update(dbmLog);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long dbmLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dbmLogNum);
				return;
			}
			Crud.DbmLogCrud.Delete(dbmLogNum);
		}
		#endregion
	}
}