using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This table is used to keep track of Triage tasks that are claimed by techs. In order to prevent concurrency issues, all operations
	///using this table should be performed against the primary customers database regardless of the database the user is currently connected to.
	///</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class TaskTaken:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskTakenNum;
		///<summary>FK to task.TaskNum. This value should be unique in the table at any one time.</summary>
		public long TaskNum;
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		///<summary>JSON text. Information regarding the user who inserted the tasktaken record as well as which server the insert command was run on.</summary>
		public string LogJson;

		///<summary></summary>
		public TaskTaken Copy() {
			return (TaskTaken)this.MemberwiseClone();
		}
	}
}

/*
CREATE TABLE tasktaken (
  TaskTakenNum bigint NOT NULL auto_increment PRIMARY KEY,
  TaskNum bigint NOT NULL,
  LogJson TEXT,
  UNIQUE INDEX(TaskNum),
  INDEX(LogJson(100))
) ENGINE=InnoDB DEFAULT CHARSET=utf8

INSERT INTO preference(PrefName,ValueString) VALUES
('CustomersHQDatabase','customers'),
('CustomersHQMySqlPassHash','FJ3SlA1plN/SDEczWeaEUg=='),
('CustomersHQMySqlUser','root'),
('CustomersHQServer','server');
*/
