namespace CodeBase {
	public class JavaScriptUtil {
		private const double JS_MAX_SAFE_INTEGER = 9007199254740991; 
		
		/// <summary>Verify that the value will be able to fit inside of JavaScript's number primitive. True if it is, otherwise false.</summary>
		public static bool DoesLongConformToJsNumber(long value) {
			//JavaScript's number primitive cannot handle values larger than this without losing accuracy.
			return (value <= JS_MAX_SAFE_INTEGER);
		}
	}
}
