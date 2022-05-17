using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {
	/// <summary>This file contains some useful queries, although it is not automated like the main program.  It is expected that these queries will need to be run manually, and that there will be additional management and tuning.  As we get nearer to the production version, we may decide to automate these queries.</summary>
	public class ConvertDatabasem {


	}
}



				

				/*
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS recallm";
					Db.NonQ(command);
					command=@"CREATE TABLE recallm (
						CustomerNum bigint NOT NULL,
						RecallNum bigint NOT NULL,
						PatNum bigint NOT NULL,
						DateDue date NOT NULL DEFAULT '0001-01-01',
						DatePrevious date NOT NULL DEFAULT '0001-01-01',
						RecallStatus bigint NOT NULL,
						Note varchar(255) NOT NULL,
						IsDisabled tinyint NOT NULL,
						DisableUntilBalance double NOT NULL,
						DisableUntilDate date NOT NULL DEFAULT '0001-01-01',
						INDEX(CustomerNum),,
						INDEX(RecallNum),,
						INDEX(PatNum),
						INDEX(RecallStatus)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				*/