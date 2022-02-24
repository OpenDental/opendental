using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	public struct ODNonNullable<T> where T : class {
		private readonly T value;

		public ODNonNullable(T value) {
			if(value == null) {
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}

		public T Value {
			get {
				if(value == null) {
					throw new NullReferenceException();
				}
				return value;
			}
		}

		public static implicit operator ODNonNullable<T>(T value) {
			return new ODNonNullable<T>(value);
		}

		public static implicit operator T(ODNonNullable<T> wrapper) {
			return wrapper.Value;
		}
	}
}
