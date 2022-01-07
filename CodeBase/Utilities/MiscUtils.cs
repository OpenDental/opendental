using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
#if !DOT_NET_CORE && !DOT_NET_STANDARD
using System.Windows.Forms;
#endif

namespace CodeBase {
	public static class MiscUtils {
		///<summary>The short descriptions for data sizes.  E.g. B, KB, MB, etc.
		///The largest value of long is 9,223,372,036,854,775,808 which is only ~8 exabytes so including zettabytes and yottabytes seems like overkill.
		///Also, the largest value of ulong would be 18,446,744,073,709,551,615 which is only ~16 exabytes.</summary>
		private static string[] _arrayShortDataSizes=new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		private const int KILOBYTE=1024;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd,int wMsg,int wParam,int lParam);

		public static string CreateRandomAlphaNumericString(int length){
			string result="";
			string randChrs="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			for(int i=0;i<length;i++){
				result+=randChrs[ODRandom.Next(0,randChrs.Length-1)];
			}
			return result;
		}

		public static string CreateRandomAlphaString(int length) {
			string result="";
			string randChrs="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
			for(int i=0;i<length;i++) {
				result+=randChrs[ODRandom.Next(0,randChrs.Length-1)];
			}
			return result;
		}

		///<summary>Indicates if the given uri is valid and of the scheme http(s).</summary>
		public static bool IsValidHttpUri(string uri) {
			if(string.IsNullOrWhiteSpace(uri)) {
				return false;
			}
			if(!Uri.TryCreate(uri,UriKind.Absolute,out Uri uriResult)) {
				return false;
			}
			return ListTools.In(uriResult.Scheme,Uri.UriSchemeHttp,Uri.UriSchemeHttps);
		}

		public static string CreateRandomNumericString(int length) {
			string result="";
			string randChrs="0123456789";
			for(int i = 0;i<length;i++) {
				result+=randChrs[ODRandom.Next(0,randChrs.Length-1)];
			}
			return result;
		}

		///<summary>Returns a date that is on or after lowerBound and before upperBound</summary>
		public static DateTime GetRandomDate(DateTime lowerBound,DateTime upperBound) {
			int daysInRange=(int)(upperBound-lowerBound).TotalDays;
			return lowerBound.AddDays(ODRandom.Next(daysInRange));
		}

		///<summary>Includes the start and end dates. Returns an empty list if the start and end is MinValue.</summary>
		public static List<DateTime> GetDatesInRange(DateTime dateTimeStart,DateTime dateTimeEnd) {
			List<DateTime> listDateTimes=new List<DateTime>();
			if(dateTimeStart!=DateTime.MinValue && dateTimeEnd==DateTime.MinValue) {
				return ListTools.FromSingle(dateTimeStart);
			}
			if(dateTimeStart==DateTime.MinValue && dateTimeEnd!=DateTime.MinValue) {
				return ListTools.FromSingle(dateTimeEnd);
            }
			if(dateTimeStart==DateTime.MinValue && dateTimeEnd==DateTime.MinValue) {
				return listDateTimes;
			}
			for(DateTime dateTime=dateTimeStart;dateTime<=dateTimeEnd;dateTime=dateTime.AddDays(1)) {
				listDateTimes.Add(dateTime.Date);
			}
			return listDateTimes;
		}

		///<summary>Displays the bytes passed in into a human readable string. E.g. KB, MB, GB, etc.
		///This method treats 1024 bytes as a single KB instead of 1000 bytes as a single KB.</summary>
		public static string DisplayBytes(long byteCount) {
			string displayByteCount="";
			string displaySize="";
			long upperLimit=KILOBYTE;
			for(int i=0;i<_arrayShortDataSizes.Length;i++) {
				displaySize=_arrayShortDataSizes[i];
				//Artificially stop at 5 (PB) because one zettabyte cannot be represented by a long, in bytes, thus we cannot display the size in exabytes.
				if(upperLimit > byteCount || i>=5) {
					displayByteCount=Math.Round((double)byteCount/(upperLimit/KILOBYTE),2).ToString();
					break;
				}
				upperLimit*=KILOBYTE;
			}
			return displayByteCount+" "+displaySize;//E.g. 1489043 bytesCount yields "1.42 MB"
		}

		///<summary>Converts string into a list of strings where each string in the list is smaller than or equal to the
		///specified size, in Bytes. Always treats the inputString as UTF8 encoded.</summary>
		///<param name="inputString">String to split into chunks</param>
		///<param name="chunkSize">Maximum number of Bytes each string in the list may be</param>
		///<returns></returns>
		public static List<string> CutStringIntoSimilarSizedChunks(string inputString,int chunkSize) {
			List<string> listChunks=new List<string>();
			int to=0;
			int from=0;
			int end=inputString.Length;
			string splitString;
			while(to<end) {
				to=Math.Min(to+chunkSize,end);
				int length=to-from;
				splitString=inputString.Substring(from,length);
				while(Encoding.UTF8.GetByteCount(splitString)>chunkSize) {
					length--;
					to--;
					splitString=inputString.Substring(from,length);
				}
				listChunks.Add(splitString);
				from+=length;
			}
			return listChunks;
		}

		///<summary>Accepts a 3 character string which represents a neutral culture (for example, "eng" for English) in the ISO639-2 format.  Returns null if the three letter ISO639-2 name is not standard (useful for determining custom languages).</summary>
		public static CultureInfo GetCultureFromThreeLetter(string strThreeLetterISOname) {
			if(strThreeLetterISOname==null || strThreeLetterISOname.Length!=3) {//Length check helps quickly identify custom languages.
				return null;
			}
			CultureInfo[] arrayCulturesNeutral=CultureInfo.GetCultures(CultureTypes.NeutralCultures);
			for(int i=0;i<arrayCulturesNeutral.Length;i++) {
				if(arrayCulturesNeutral[i].ThreeLetterISOLanguageName==strThreeLetterISOname) {
					return arrayCulturesNeutral[i];
				}
			}
			return null;
		}

		///<summary>Extension for BETWEEN statement.  Use like this: if(x.Between(0,9)).  By default, it includes both upper and lower bound in test.</summary>
		public static bool Between<T>(this T item,T lowerBound,T upperBound,bool isLowerBoundInclusive=true,bool isUpperBoundInclusive=true) 
			where T:IComparable 
		{
			if(isLowerBoundInclusive && isUpperBoundInclusive) {
				return (item.CompareTo(lowerBound)>=0 && item.CompareTo(upperBound)<=0);
			}
			if(isLowerBoundInclusive && !isUpperBoundInclusive) {
				return (item.CompareTo(lowerBound)>=0 && item.CompareTo(upperBound) < 0);
			}
			if(!isLowerBoundInclusive && isUpperBoundInclusive) {
				return (item.CompareTo(lowerBound) > 0 && item.CompareTo(upperBound)<=0);
			}
			if(!isLowerBoundInclusive && !isUpperBoundInclusive) {
				return (item.CompareTo(lowerBound) > 0 && item.CompareTo(upperBound) < 0);
			}
			return false;//This code is unreachable but the compiler doesn't realize it.
		}

		///<summary>Filters the current IEnumerable of objects based on the func provided.
		///C# does not provide a way to do listObj.Distinct(x => x.Field).  This extension allows us to do listObj.DistinctBy(x => x.Field)</summary>
		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> listSource,Func<T,TKey> keySelector) {
			HashSet<TKey> hashKeys=new HashSet<TKey>();
			foreach(T source in listSource) {
				if(hashKeys.Add(keySelector(source))) {
					yield return source;//Manipulates the current sourceList instead of having to return an entire list.
				}
			}
		}

		///<summary>Loops through the IEnumerable and performs the action on each item.</summary>
		public static void ForEach<T>(this IEnumerable<T> listSource,Action<T> action) {
			foreach(T source in listSource) {
				action(source);
			}
		}

		///<summary>Returns true if the two time slots overlap in time. Slot1 and Slot2 are interchangeable.</summary>
		public static bool DoSlotsOverlap(DateTime slot1Start,DateTime slot1End,DateTime slot2Start,DateTime slot2End) {
			return (slot1End > slot2Start && slot1Start < slot2End);
		}

		///<summary>Returns exception string that includes the threadName if provided and exception type and up to 5 inner exceptions.
		///Used for both bugSubmissions and the MsgBoxCopyPaste shown to customers when a UE occurs.</summary>
		public static string GetExceptionText(Exception e,string threadName=null,bool isUnhandledException=true) {
			string text="";
			if(isUnhandledException) {
				text="Unhandled exception ";
			}
			text+=(string.IsNullOrEmpty(threadName) ?"":"from "+threadName)
				  +(isUnhandledException?":  ":"")
					+(string.IsNullOrEmpty(e.Message)?"No Exception Message":e.Message+"\r\n")
					+(string.IsNullOrEmpty(e.GetType().ToString())?"No Exception Type":e.GetType().ToString())+"\r\n"
					+(string.IsNullOrEmpty(e.StackTrace)?"No StackTrace":e.StackTrace);
			if(e is AggregateException) {
				foreach(Exception innerEx in ((AggregateException)e).InnerExceptions) {
					text+=InnerExceptionToString(innerEx);
				}
			}
			else {
				text+=InnerExceptionToString(e.InnerException);//New lines handled in method.
			}
			return text;
		}

		///<summary>Formats the inner exception (and all its inner exceptions) as a readable string. Okay to pass in an exception with no inner 
		///exception.</summary>
		///<param name="depth">The recursive depth of the current method call.</param>
		public static string InnerExceptionToString(Exception innerEx,int depth=0) {
			if(innerEx==null
				|| depth>=5)//Limit to 5 inner exceptions to prevent infinite recursion
			{
				return "";
			}
			return "\r\n-------------------------------------------\r\n"
				+"Inner exception:  "+innerEx.Message+"\r\n"+innerEx.GetType().ToString()+"\r\n"
				+innerEx.StackTrace
				+InnerExceptionToString(innerEx.InnerException,++depth);
		}

		///<summary>Gets the innermost InnerException that is not null. Recursive.</summary>
		public static Exception GetInnermostException(Exception ex,int depth=0) {
			if(ex.InnerException==null
				|| depth>=100)//Limit to 100 inner exceptions to prevent infinite recursion
			{
				return ex;
			}
			return GetInnermostException(ex.InnerException,++depth);
		}

		///<summary>Returns the exception or inner exception that contains the given message. Returns null otherwise.</summary>
		public static Exception GetExceptionContainingMessage(Exception ex,string message,int depth=0) {
			if(ex.Message.Contains(message)) {
				return ex;
			}
			if(ex.InnerException==null || depth > 100) {//Limit to 100 inner exceptions to prevent stack overflow
				return null;
			}
			return GetExceptionContainingMessage(ex.InnerException,message,++depth);
		}

		///<summary>Attempts to throw the given exception, preserving the original stack trace.</summary>
		public static void PreserveExceptionInfoAndThrow(Exception ex) {
			ExceptionDispatchInfo exInfo=ExceptionDispatchInfo.Capture(ex);
			exInfo.Throw();//This line should actually throw.
		}

		///<summary>Gets the ordinal indicator e.g.passing in 13 will return "th". Translations should be done in the calling class and 
		///should include the number in the translation. This is because different languages have different ordinal rules for each number.</summary>
		public static string GetOrdinalIndicator(string num) {
			try {
				return GetOrdinalIndicator(Convert.ToInt32(num));
			}
			catch {
				return "";//invalid number
			}
		}

		///<summary>Gets the ordinal indicator e.g.passing in 13 will return "th". Translations should be done in the calling class and 
		///should include the number in the translation. This is because different languages have different ordinal rules for each number.</summary>
		public static string GetOrdinalIndicator(int num){
			if(num<=0) {
				return "";
			}
			switch(num % 100) {
				case 11:
				case 12:
				case 13:
					return "th";
			}
			switch(num % 10) {
				case 1:
					return "st";
				case 2:
					return "nd";
				case 3:
					return "rd";
				default:
					return "th";
			}
		}

		///<summary>Gets the most recent date in the past (or today) that is the specified day of week.</summary>
		public static DateTime GetMostRecentDayOfWeek(DateTime date,DayOfWeek dayOfWeek) {
			if(DateTime.MinValue.AddDays(7) > date) {
				throw new ArgumentException("Date must be at least 7 days greater than MinDate: "+date);
			}
			for(int i=0;i<7;i++) {
				DateTime newDate=date.AddDays(-i);
				if(newDate.DayOfWeek==dayOfWeek) {
					return newDate;
				}
			}
			throw new Exception("Unable to find day of the week: "+dayOfWeek.ToString());
		}

		///<summary>Gets the soonest upcoming date in the future (or today) that is the specified day of week.</summary>
		public static DateTime GetUpcomingDayOfWeek(DateTime date,DayOfWeek dayOfWeek) {
			if(DateTime.MaxValue.AddDays(-7) < date) {
				throw new ArgumentException("Date must be at least 7 days smaller than MaxValue: "+date);
			}
			for(int i=0;i<7;i++) {
				DateTime newDate=date.AddDays(i);
				if(newDate.DayOfWeek==dayOfWeek) {
					return newDate;
				}
			}
			throw new Exception("Unable to find day of the week: "+dayOfWeek.ToString());
		}

		///<summary>Not used yet.  This causes a time to be set at an event, like clicking the Appt module.  
		///At the next interesting event, it's used for comparison, then set again. 
		///The result that gets sent to debug output window is a series of intervals, so that we can notice the longest intervals.  
		///The location names that are passed in are the name of what just happened, so that the names match up with the intervals.</summary>
		public static void DebugLogInterval(string locationName){
			//Temporarily add SPEEDTESTING to the CodeBase, properties, Build, conditional compilation symbols section.  But don't commit that change.
			//Also add SPEEDTESTING TO OpenDentBusiness and OpenDental projects.  Also don't commit those changes.
#if !SPEEDTESTING
			return;
#endif
			try{
				TimeSpan timeSpan=DateTime.Now-dateTimeDebugPrevious;
				if(timeSpan>TimeSpan.FromHours(1)){//first time on each run
					Debug.Write("----");
				}
				else if(locationName.StartsWith("-")){//a quick convention for not showing a time that we don't care about.
					Debug.Write("----");
				}
				else{
					Debug.Write(timeSpan.ToString(@"ss\.FFF"));
				}
				Debug.WriteLine(" "+locationName);
				dateTimeDebugPrevious=DateTime.Now;
			}
			catch{
				Debug.WriteLine("Exception, "+locationName);
			}
		}
		private static DateTime dateTimeDebugPrevious;

#if !DOT_NET_CORE && !DOT_NET_STANDARD

		///<summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database.</summary>
		public static string Encrypt(string encrypt) {
			UTF8Encoding enc=new UTF8Encoding();
			byte[] arrayEncryptBytes=Encoding.UTF8.GetBytes(encrypt);
			MemoryStream ms=new MemoryStream();
			CryptoStream cs=null;
			Aes aes=new AesCryptoServiceProvider();
			aes.Key=enc.GetBytes("AKQjlLUjlcABVbqp");
			aes.IV=new byte[16];
			ICryptoTransform encryptor=aes.CreateEncryptor(aes.Key,aes.IV);
			cs=new CryptoStream(ms,encryptor,CryptoStreamMode.Write);
			cs.Write(arrayEncryptBytes,0,arrayEncryptBytes.Length);
			cs.FlushFinalBlock();
			byte[] retval=new byte[ms.Length];
			ms.Position=0;
			ms.Read(retval,0,(int)ms.Length);
			cs.Dispose();
			ms.Dispose();
			if(aes!=null) {
				aes.Clear();
			}
			return Convert.ToBase64String(retval);
		}

		public static string Decrypt(string encString,bool doThrow = false) {
			try {
				byte[] encrypted=Convert.FromBase64String(encString);
				MemoryStream ms=null;
				CryptoStream cs=null;
				StreamReader sr=null;
				Aes aes=new AesCryptoServiceProvider();
				UTF8Encoding enc=new UTF8Encoding();
				aes.Key=enc.GetBytes("AKQjlLUjlcABVbqp");
				aes.IV=new byte[16];
				ICryptoTransform decryptor=aes.CreateDecryptor(aes.Key,aes.IV);
				ms=new MemoryStream(encrypted);
				cs=new CryptoStream(ms,decryptor,CryptoStreamMode.Read);
				sr=new StreamReader(cs);
				string decrypted=sr.ReadToEnd();
				ms.Dispose();
				cs.Dispose();
				sr.Dispose();
				if(aes!=null) {
					aes.Clear();
				}
				return decrypted;
			}
			catch(Exception e) {
				if(doThrow) {
					throw e;
				}
				MessageBox.Show("Text entered was not valid encrypted text.");
				return "";
			}
		}

		///<summary>Finds installed IE version for this workstation and attempts to modify registry to force browser emualtion to this version.
		///Typically used in conjunction with WebBrowser control to ensure that the WebBrowser is running in the latest available emulation mode.
		///Returns true if the emulation version was previously wrong but was successfully updated. Otherwise returns false.
		///If true is returned than this application will need to be restarted in order for the changes to take effect.</summary>
		public static bool TryUpdateIeEmulation() {
			bool ret=false;
			try {
				int browserVersion;
				//Get the installed IE version.
				using(WebBrowser wb = new WebBrowser()) {
					browserVersion=wb.Version.Major;
				}
				int regVal;
				//Set the appropriate IE version
				if(browserVersion>=11) {
					regVal=11001;
				}
				else if(browserVersion==10) {
					regVal=10001;
				}
				else if(browserVersion==9) {
					regVal=9999;
				}
				else if(browserVersion==8) {
					regVal=8888;
				}
				else if(browserVersion==7) {
					regVal=7000;
				}
				else {//Unknown version.  This will happen when version 12 and beyond are released.
					regVal=browserVersion*1000+1;//Guess the regVal code needed based on the historic pattern.
				}
				//Set the actual key.  This key can be set without admin rights, because it is within the current user's registry store.
				string applicationName=Process.GetCurrentProcess().ProcessName+".exe";//This is OpenDental.vhost.exe when debugging, different for distributors.
				string keyPath=@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
				Microsoft.Win32.RegistryKey key=Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath,true);
				if(key==null) {
					key=Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath);
				}
				object keyValueCur=key.GetValue(applicationName);
				if(keyValueCur==null||keyValueCur.ToString()!=regVal.ToString()) {
					key.SetValue(applicationName,regVal,Microsoft.Win32.RegistryValueKind.DWord);
					ret=true;
				}
				key.Close();
			}
			catch(Exception e) {
				e.DoNothing();
			}
			return ret;
		}

		///<summary>Returns the file name with the extension of the currently executing program.</summary>
		public static string GetCurrentExeFileName() {
			return Path.GetFileName(Application.ExecutablePath);
		}

#endif

		///<summary>Returns a string representation of a version in the format 'x.x.x.x'. Most likely the string passed in should be in the correct format.
		///Sometimes the passed string will have letters after the build number. This will strip letters.</summary>
		public static string GetVersionFromString(string strVersion) {
			string[] arrayVersion=strVersion.Split('.');
			string majorNum="0";
			string minorNum="0";
			string buildNum="0";
			string revisionNum="0";
			if(arrayVersion.Count()>0) {
				majorNum=CleanVersionNumber(arrayVersion[0]);
			}
			if(arrayVersion.Count()>1) {
				minorNum=CleanVersionNumber(arrayVersion[1]);
			}
			if(arrayVersion.Count()>2) {
				buildNum=CleanVersionNumber(arrayVersion[2]);
			}
			if(arrayVersion.Count()>3) {
				revisionNum=CleanVersionNumber(arrayVersion[3]);
			}
			return int.Parse(majorNum)+"."+int.Parse(minorNum)+"."+int.Parse(buildNum)+"."+int.Parse(revisionNum);
		}

		///<summary>This helper method does the stripping of any alpha characters.</summary>
		private static string CleanVersionNumber(string strToParse) {
			int cleanNum=0;
			List<string> listNums=Regex.Split(strToParse, @"\D+").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
			if(listNums.Count()>0) {
				int.TryParse(listNums[0],out cleanNum);
			}
			return cleanNum.ToString();//return the parsed number as a string
		}

	}

	/// <summary>This class is used to create a tree of actions who depend on each other.
	/// If action x depends on action y, then action x will be a leaf of action y.
	/// When StartNodeAction() is called, the node action is completed, then each leaf action is added to the thread pool.
	/// This is faster than creating a new thread for each action because of the overhead when starting a large amount of threads.
	/// This class is best used for a longer list of actions that are dependent on one another that can be described by a tree graph.
	/// </summary>
	public class ActionNode {
		private Action _actionParent;
		private List<ActionNode> _listChildActionNodes;

		///<summary>Creates an action node with an empty action and no children.  Good for use as a root.</summary>
		public ActionNode() : this(()=> {}) {
		}

		/// <summary>Creates an action node with no children and the passed in action.</summary>
		public ActionNode(Action actionParent) : this(actionParent,new List<ActionNode>()) {
		}
			
		/// <summary>Creates an action node with children via the parameters passed in.</summary>
		public ActionNode(Action actionParent,List<ActionNode> listChildActionNodes) {
			_actionParent=actionParent;
			_listChildActionNodes=listChildActionNodes;
		}

		/// <summary>Synchronously invokes _actionParent on the main thread then asynchronously invokes all _listChildActionNodes (run in parallel)
		/// and then waits until all children and their children's children (utilizing recursion) have finished executing.</summary>
		public void StartNodeAction() {
			//Always complete the parent action on the main thread (synchronous).
			//The ActionNode class is designed to have the children nodes dependant on _actionParent.
			_actionParent.Invoke();
			//Create a list of tasks that will be run asynchronously so that we can "join" with them all before returning.
			List<System.Threading.Tasks.Task> listTask=new List<System.Threading.Tasks.Task>();
			//We don't create a new thread for each because of the overhead.  Adding to the thread pool is more economical and faster.
			foreach(ActionNode n in _listChildActionNodes) {
				//Using Tasks is much faster than individual threads.
				listTask.Add(System.Threading.Tasks.Task.Run(() => n.StartNodeAction()));
			}
			//Wait for all threads to complete before returning.
			System.Threading.Tasks.Task.WaitAll(listTask.ToArray());
		}
	}

	///<summary>System.Random is not thread-safe. 
	///This class syncronizes a single instance of System.Random and performs a lock anytime it is accessed, which makes it thread-safe.</summary>
	public static class ODRandom {
		private static object _lock=new object();
		private static Random _rand=new Random();
		///<summary>Returns a random integer that is within a specified range. minValue is inclusive, maxValue is exclusive. System.Random is not 
		///thread-safe. This method makes it thread-safe.</summary>
		public static int Next(int minValue,int maxValue) {
			lock(_lock) {
				return _rand.Next(minValue,maxValue);
			}
		}
		///<summary>Returns a nonnegative random integer. System.Random is not thread-safe. This method makes it thread-safe.</summary>
		public static int Next() {
			lock(_lock) {
				return _rand.Next();
			}
		}
		///<summary>Returns a nonnegative random integer that is less than the specified maximum. System.Random is not thread-safe. This method makes 
		///it thread-safe.</summary>
		public static int Next(int maxValue) {
			lock(_lock) {
				return _rand.Next(maxValue);
			}
		}
		///<summary>Fills the elements of a specified array of bytes with random numbers. System.Random is not thread-safe. This method makes it 
		///thread-safe.</summary>
		public static void NextBytes(byte[] buffer) {
			lock(_lock) {
				_rand.NextBytes(buffer);
			}
		}

		///<summary>Returns a random floating-point number between 0.0 and 1.0. System.Random is not thread-safe. This method makes it thread-safe.
		///</summary>
		public static double NextDouble() {
			lock(_lock) {
				return _rand.NextDouble();
			}
		}


	}
}
