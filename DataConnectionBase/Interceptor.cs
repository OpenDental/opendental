using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using MySql.Data.MySqlClient;

namespace DataConnectionBase {
	///<summary> </summary>
	public class Interceptor: MySql.Data.MySqlClient.BaseCommandInterceptor {

		///<summary>Filters out collations with NULL id (e.g. UCA-14.0.0) from SHOW COLLATION command.</summary>
		public override bool ExecuteReader(string command, CommandBehavior behavior, ref MySqlDataReader returnValue) {
			if(string.IsNullOrEmpty(command) || !command.Equals("SHOW COLLATION",StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			using(MySqlCommand mysqlCommand=ActiveConnection.CreateCommand()) {
				mysqlCommand.CommandText="SHOW COLLATION WHERE Id IS NOT NULL";
				returnValue=mysqlCommand.ExecuteReader(behavior);
			}
			return true;
		}
	}
}