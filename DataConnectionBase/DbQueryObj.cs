using System;

namespace DataConnectionBase {
	///<summary>Helper object that holds metadata regarding a query or Middle Tier payload that was executed by the program.</summary>
	public class DbQueryObj {
		///<summary>A globally unique identifier for this particular query.  Useful for updating DateTimeStop when query finishes executing.</summary>
		public Guid GUID;
		///<summary>The full text of the query that was executed.</summary>
		public string Command;
		///<summary>The date and time that this object was created.</summary>
		public DateTime DateTimeInit;
		///<summary>The date and time that the query started executing.</summary>
		public DateTime DateTimeStart;
		///<summary>The date and time that the query stopped executing.</summary>
		public DateTime DateTimeStop;
		///<summary>The calling method name. e.g. GetTable, GetScalar, NonQ, etc.</summary>
		public string MethodName;
		///<summary>The stack trace string. Only used when stack trace user preference is enabled.</summary>
		public string StackTrace;
		///<summary>Returns the difference between DateTimeStop and DateTimeStart if both are valid date times.</summary>
		public TimeSpan Elapsed {
			get {
				if(DateTimeStart==DateTime.MinValue || DateTimeStop==DateTime.MinValue) {
					return TimeSpan.MinValue;
				}
				return (DateTimeStop - DateTimeStart);
			}
		}

		///<summary>Creates a query object that stores the command passed in and automatically sets a GUID and DateTimeStart.</summary>
		public DbQueryObj(string command) {
			GUID=Guid.NewGuid();
			Command=command;
			DateTimeInit=DateTime.Now;
		}

		///<summary>A string representation of what this query object should look like in the log file.</summary>
		public override string ToString() {
			return $"# GUID: {(GUID==null ? "NULL" : GUID.ToString())}{(string.IsNullOrEmpty(MethodName)?"":$"  Method Name: {MethodName}")}\r\n"
				+$"# DateTimeStart: {DateTimeStart.ToString("MM/dd/yyyy hh:mm:ss.fffffff tt")}\r\n"
				+$"# DateTimeStop: {((DateTimeStop==DateTime.MinValue)?"Still Running":DateTimeStop.ToString("MM/dd/yyyy hh:mm:ss.fffffff tt"))}\r\n"
				+$"# Elapsed: {Elapsed.ToString("G")}\r\n"
				+$"{Command}";//Not starting this line with # makes a nice visual break between queries.
		}

		public DbQueryObj Copy() {
			return (DbQueryObj)this.MemberwiseClone();
		}
	}
}
