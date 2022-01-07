using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	///<summary>This class can be used over Middle Tier in place of System.Tuple. Trying to serialize a System.Tuple for Middle Tier will cause an
	///exception because it does not have a parameterless constructor.</summary>
	public class ODTuple<T1, T2> {
		public ODTuple() { }
		public ODTuple(T1 item1,T2 item2) {
			Item1=item1;
			Item2=item2;
		}
		public T1 Item1 { get; set; }
		public T2 Item2 { get; set; }

		public static implicit operator Tuple<T1,T2>(ODTuple<T1,T2> tuple) {
			return Tuple.Create(tuple.Item1,tuple.Item2);
		}

		public static implicit operator ODTuple<T1,T2>(Tuple<T1,T2> tuple) {
			return new ODTuple<T1,T2>(tuple.Item1,tuple.Item2);
		}
	} 
	
	///<summary>This class can be used over Middle Tier in place of System.Tuple. Trying to serialize a System.Tuple for Middle Tier will cause an
	///exception because it does not have a parameterless constructor.</summary>
	public class ODTuple<T1, T2, T3> {
		public ODTuple() { }
		public ODTuple(T1 item1,T2 item2,T3 item3) {
			Item1=item1;
			Item2=item2;
			Item3=item3;
		}
		public T1 Item1 { get; set; }
		public T2 Item2 { get; set; }
		public T3 Item3 { get; set; }

		public static implicit operator Tuple<T1,T2,T3>(ODTuple<T1,T2,T3> tuple) {
			return Tuple.Create(tuple.Item1,tuple.Item2,tuple.Item3);
		}

		public static implicit operator ODTuple<T1,T2,T3>(Tuple<T1,T2,T3> tuple) {
			return new ODTuple<T1,T2,T3>(tuple.Item1,tuple.Item2,tuple.Item3);
		}
	}
}
