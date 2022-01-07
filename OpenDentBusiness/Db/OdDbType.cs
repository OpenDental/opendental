using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	public enum OdDbType {
		///<summary>C#:bool, MySql:tinyint(3)or(1), Oracle:number(3), </summary>
		Bool,
		///<summary>C#:byte, MySql:tinyint unsigned, Oracle:number(3), Range:0-255.</summary>
		Byte,
		///<summary>C#:double, MySql:double, Oracle:number(38,8), Need to change C# type to Decimal.  Need to change MySQL type.</summary>
		Currency,
		///<summary>C#:DateTime, MySql:date, Oracle:date, 0000-00-00 not allowed in Oracle and causes problems in MySql</summary>
		Date,
		///<summary>C#:DateTime, MySql:datetime, Oracle:date, </summary>
		DateTime,
		///<summary>C#:DateTime, MySql:timestamp, Oracle:date + trigger, </summary>
		DateTimeStamp,
		///<summary>C#:enum, MySql:tinyint, Oracle:number(3), </summary>
		Enum,
		///<summary>C#:float, MySql:float, Oracle:number(38,8), </summary>
		Float,
		///<summary>C#:int32, MySql:int,smallint(if careful), Oracle:number(11), Range:-2,147,483,647-2,147,483,647.  Also used for colors</summary>
		Int,
		///<summary>C#:long, MySql:bigint, Oracle:number(20), Range:–9,223,372,036,854,775,808 to 9,223,372,036,854,775,807</summary>
		Long,
		///<summary>C#:string, MySql:text,mediumtext, Oracle:varchar2,clob, Range:256+. MaxSizes: MySql.text=65k, Oracle.varchar2=4k.</summary>
		Text,
		///<summary>C#:TimeSpan, MySql:time, Oracle:date, Range:Valid time of day.</summary>
		TimeOfDay,
		///<summary>C#:TimeSpan, MySql:time, Oracle:varchar2, Range:Pos or neg spans of many days.  Oracle has no such type.</summary>
		TimeSpan,
		///<summary>C#:string, MySql:varchar(255), Oracle:varchar2(255), MaxSize:255</summary>
		VarChar255
	}

	

}
