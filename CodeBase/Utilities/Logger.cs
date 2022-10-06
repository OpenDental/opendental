using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;

namespace CodeBase {
	///<summary>Used to log messages to our internal log file, or to other resources, such as message boxes.</summary>
	public partial class Logger{
		///<summary>Levels of logging severity to indicate importance.</summary> 
		public enum Severity{
			NONE=0,//Must be first.
			DEBUG=1,
			INFO=2,
			WARNING=3,
			ERROR=4,
			FATAL_ERROR=5,
		};

		///<summary>The number of bytes it takes to move the current log to the backup/old log, to prevent the log files from growing infinately.</summary>
		private const int logRollByteCount=1048576;
		
		private string logFile="";
		///<summary>Specifies the current logging level. Any severity less than the given level is not logged.</summary> 
#if(DEBUG)
		public Severity level=Severity.DEBUG;
#else
		public Severity level=Severity.NONE;
#endif
		#region WebCore Logger Copy
		public static int MAX_FILE_SIZE_KB=1000;
		private static Dictionary<string /*sub-directory, can be empty string (not null though)*/,object[]/*{StreamWriter, Create DateTime} the file currently linked to this sub-directory*/> _files=new Dictionary<string,object[]>();
		private static object _lock=new object();
		private const string CLEANUP_DIR="CleanupLogger";
		private static ODThread _threadLoggerCleanup=null;
		#endregion
		///<summary>Boolean used to determine what directory the logger will write to.</summary>
		private static bool _canUseMyDocsDir=false;
		///<summary>Can be set to change the directory the logger will write to.</summary>
		private static string _loggerDirOverride="";

		#region WebCore Logger Copy
		public const string DATETIME_FORMAT="MM/dd/yy HH:mm:ss:fff";
		private static DateTime _lastLoggerCleanup=DateTime.MinValue;
		///<summary>Can be set to change the directory the logger will write to.</summary>
		public static string LoggerDirOverride {
			get {
				lock(_lock) {
					return _loggerDirOverride;
				}
			}
			set {
				lock(_lock) {
					_loggerDirOverride=value;
				}
			}
		}

		#region Parseable Logging
		///<summary>This method takes a string that should be some kind of an identifier (usually method name) for the method that is being logged. 
		///The optional string is for any additional information the implementer finds useful to be in the log string. 
		///LogPath determines the directory to log to and LogPhase determines whether the logger is a "Start" line or a "Stop" line.</summary>
		public static void LogToPath(string log,LogPath path,LogPhase logPhase,string optionalDesc="") {
			string logWrite=GetCallingMethod()+" "+log;
			switch(logPhase) {
				case LogPhase.Unspecified:
					break;
				case LogPhase.Start:
					logWrite+=" start";
					break;
				case LogPhase.End:
					logWrite+=" end";
					break;
			}
			if(optionalDesc!="") {
				logWrite+=" ... "+optionalDesc;
			}
			LogVerbose(logWrite,path.ToString()+"\\"+Process.GetCurrentProcess().Id.ToString());
		}

		///<summary>Accomplishes the same function as LogSignals, but by using an action we can save on messy looking code for sections that need heavy logging.
		///Should generally be used on one line statements. Jordan approves exception to rule against using anonymous methods inside arguments as long as it's a one liner. Don't throw a big block in here.</summary>
		public static void LogAction(string log,LogPath logPath,Action action,string optionalDesc="") {
			LogToPath(log,logPath,LogPhase.Start,optionalDesc);
			action();
			LogToPath(log,logPath,LogPhase.End);
		}

		///<summary>Accomplishes the same function as LogAction, but only logs if the given Action exceeds the given milliseconds.
		///Should generally be used on one line statements.</summary>
		public static void LogActionIfOverTimeLimit(string log,LogPath logPath,Action action,int milliseconds=5000) {
			Stopwatch stopwatch=new Stopwatch();
			stopwatch.Start();
			action();
			stopwatch.Stop();
			if(stopwatch.Elapsed>TimeSpan.FromMilliseconds(milliseconds)) {
				LogToPath(log,logPath,LogPhase.Unspecified,$"Took too long: {(int)stopwatch.Elapsed.TotalSeconds} seconds");
			}
		}

		///<summary>This method finds the method that called the logger. It loops through the first 5 stack frames and returns the full name of the 
		///method that called it. This method carefully excludes its own calling methods from the stack trace.</summary>
		private static string GetCallingMethod() {
			try {
				for(int i=2;i<4;i++) {//Start at stackframe(2) because 0,1, and possibly 2 are the parents of this method.		
						StackFrame frame=new StackFrame(i);
						System.Reflection.MethodBase method=frame.GetMethod();
						if(!method.Name.ToLower().Contains("logtopath") && !method.Name.ToLower().Contains("logaction")) {
							return method.ReflectedType.FullName+"."+method.Name;
						}
				}
			}
			catch(Exception e) {
					e.DoNothing();
			}
			//Return blank if we couldn't find a method name that wasn't ourself in the first 5 frames
			return "";
		}

		public delegate bool DoVerboseLoggingArgs();
		public static DoVerboseLoggingArgs DoVerboseLogging;

		///<summary>If HasVerboseLogging(Environment.MachineName) then Logger.WriteLine(log). Otherwise do nothing.</summary>
		public static void LogVerbose(string log,string subDirectory = "") {
			try {
				if(DoVerboseLogging==null || !DoVerboseLogging()) {
					return;
				}
				if(DateTime.Now.Subtract(_lastLoggerCleanup)>TimeSpan.FromDays(1)) { //Once a day logger cleanup is due.
					Logger.CleanupLoggerDirectoryAsync(0);
					_lastLoggerCleanup=DateTime.Now;
				}
				Logger.WriteLine(log,subDirectory);
			}
			catch(Exception e) {
				e.DoNothing();
			}
		}

		#endregion
		public static string GetDirectory(string subDirectory) {
			subDirectory=ScrubSubDirPath(subDirectory);
			string ret = "";
			bool canUseMyDocsDir;
			string loggerDirOverride;
			lock (_lock) {
				canUseMyDocsDir=_canUseMyDocsDir;
				loggerDirOverride=LoggerDirOverride;
			}
			//Could make this a ternary operator but it is incredibly long.
			if(canUseMyDocsDir) {
				//The logger file is sometimes blocked by Windows unless OD is ran as admin. This is a work around to avoid that file block by writing to MyDocuments.
				ret=System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Logger");
			}
			else if(!string.IsNullOrEmpty(loggerDirOverride)) {
				ret=loggerDirOverride;
			}
			else if(File.Exists(ODFileUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory,"web.config"))) {//Note this is case insensitive
				//We are using the existence of a web.config file to determine if this is a web application. While there is a more official way to do this,
				//that way requires System.Web.dll which is not available for .NET Standard.
				FileInfo fi=new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
				string drive=Path.GetPathRoot(fi.FullName);
				//For example, an application at "E:\patientviewer.com\SignupPortal" will have its logger folder at 
				//"E:\ProgramData\OpenDental\Logs\patientviewer.com\SignupPortal\Logger".
				ret=ODFileUtils.CombinePaths(new string[] { drive,"ProgramData","OpenDental","Logs",
					StringTools.SubstringAfter(AppDomain.CurrentDomain.BaseDirectory,drive),"Logger" });
				if(!Directory.Exists(ret)) {
					Directory.CreateDirectory(ret);
				}
			}
			else {
				ret=ODFileUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory,"Logger");
			}
			if(!string.IsNullOrEmpty(subDirectory)) {
				ret=ODFileUtils.CombinePaths(ret,subDirectory);
			}
			return ret;
		}

		public static void WriteLine(string line,string subDirectory) {
			WriteLine(line,subDirectory,false,true);
		}

		public static void WriteLine(string line,string subDirectory,bool singleFileOnly,bool includeTimestamp) {
			lock (_lock) {
				subDirectory=ScrubSubDirPath(subDirectory);
				StreamWriter file = Open(subDirectory,singleFileOnly);
				if(file==null) {
					return;
				}
				string timeStamp=includeTimestamp?(DateTime.Now.ToString(DATETIME_FORMAT)+"\t"):"";
				file.WriteLine(timeStamp+line);
			}
		}

		public static void WriteError(string line,string subDirectory) {
			WriteLine("failed - "+line,subDirectory,false,true);
		}

		public static void WriteException(Exception e,string subDirectory) {
			WriteError(MiscUtils.GetExceptionText(e),subDirectory);
		}

		public static void CloseLogger() {
			lock (_lock) {
				while(_files.Count>=1) {
					IEnumerator enumerator = _files.Keys.GetEnumerator();
					if(enumerator==null||!enumerator.MoveNext()) {
						break;
					}
					CloseFile((string)enumerator.Current);
				}
			}
		}

		///<summary>Starts a thread which cleans up the Logger directory. 
		///Thread will be interrupted and stopped if doInterrupt returns true.
		///Thread can also be interrupted from it's dorman state if the Action which is returned is invoked.
		///Delete all files and folders under the Logger directory which are older than 30 days. 
		///This method can take several minutes if there are are lot of files to process.
		///If run once and exit is desired then set frequencyMS=0. Otherwise set frequencyMS to desired frequency of full scan.
		///It is suggested that this scan run no more than once per day.</summary>
		public static Action CleanupLoggerDirectoryAsync(int frequencyMS,Func<bool> doInterrupt=null,Action<string> onScanningItem=null
			,Action onExit=null,int cleanIfOlderThanDays=30) {
			Action onCancel=new Action(() => {
				if(_threadLoggerCleanup!=null) {
					_threadLoggerCleanup.QuitAsync(true);
				}
			});
			if(_threadLoggerCleanup!=null) { //Still running from 1 day ago. That would be a problem.
				return onCancel;
			}
			//Start a thread so we do not interrupt the rest of the AccountMaintThread operations.
			_threadLoggerCleanup=new ODThread(frequencyMS,new ODThread.WorkerDelegate((th) => {
				CleanupLoggerDirectory(doInterrupt,onScanningItem,cleanIfOlderThanDays);
			}));
			_threadLoggerCleanup.AddExitHandler(new ODThread.WorkerDelegate((th) => {
				_threadLoggerCleanup=null;
				if(onExit!=null) {
					onExit();
				}
			}));
			_threadLoggerCleanup.AddExceptionHandler(new ODThread.ExceptionDelegate((e) => { WriteException(e,CLEANUP_DIR); }));
			_threadLoggerCleanup.Start(true);
			return onCancel;
		}

		///<summary>Cleans up the Logger directory. 
		///Can be interrupted and stopped if doInterrupt returns true.
		///Delete all files and folders under the Logger directory which are older than 30 days. 
		///This method can take several minutes if there are are lot of files to process.</summary>
		public static void CleanupLoggerDirectory(Func<bool> doInterrupt=null,Action<string> onScanningItem=null,int cleanIfOlderThanDays=30) {			
			try {
				Func<bool> quitPremature=new Func<bool>(() => {
					if(doInterrupt==null) {
						return false;
					}
					return doInterrupt();
				});
				Action<string> status=new Action<string>((s) => {
					WriteLine(s,CLEANUP_DIR);
				});
				Action<Exception> statusE=new Action<Exception>((e) => {
					WriteException(e,CLEANUP_DIR);
				});
				Action<int,int,bool,string> logIfNecessary=new Action<int, int, bool, string>((numerator,denominator,forceLog,itemName) => {
					if(!string.IsNullOrEmpty(itemName) && onScanningItem!=null) {
						onScanningItem(itemName);
					}
					//Log a max of 100 times.
					int mod=denominator/100;
					if(mod<=0) { //Guard against divide by 0.
						mod=1;
					}
					if(!forceLog && (numerator % mod != 0)) { //Not time to log yet.
						return;
					}
					int percentDone;
					if(denominator<=0) {
						percentDone=100;
					}
					else {
						percentDone=(int)(numerator/(double)denominator*100);
					}
					status("Scanned "+numerator.ToString()+" of "+denominator.ToString()+". "+percentDone.ToString()+"% done.");
					if(!string.IsNullOrEmpty(itemName)) {
						status("Current scan item: "+itemName);
					}
				});
				TimeSpan tsOldThan=TimeSpan.FromDays(cleanIfOlderThanDays);
				DirectoryInfo di=new DirectoryInfo(GetDirectory(""));
				#region Delete files
				//Get all files recursively under this directory.
				status("Scanning files.");
				status("Querying files in directory: "+di.FullName);
				logIfNecessary(1,10000,false,di.FullName);
				List<FileInfo> files=new List<FileInfo>(di.GetFiles("*.*",SearchOption.AllDirectories))
					//Filter down to those files that are old enough to be deleted.
					.FindAll(x => x.CreationTime<DateTime.Today.Add(-tsOldThan));
				status("Deleting "+files.Count.ToString()+" files... "+(files.Sum(x => x.Length)/1024/(double)1024).ToString("N2")+" MB");
				FileInfo file;
				//Delete each of the files.
				for(int iCurFile=0;iCurFile<files.Count;iCurFile++) {
					try {
						file=files[iCurFile];
						if(quitPremature()) {
							logIfNecessary(iCurFile,files.Count,true,"");
							status("Quit file delete premature");
							return;
						}
						logIfNecessary(iCurFile,files.Count,false,file.FullName);
						file.Delete();
					}
					catch(Exception e) {
						statusE(e);
					}
				}
				status("File scan complete.");
				#endregion
				#region Delete directories
				//The files are now gone so it is safe to recursively delete all directories that may now be empty.
				//Count all directories and sub-directories so we can keep track of progress.
				int totalDirectories=Directory.GetDirectories(di.FullName,"*.*",SearchOption.AllDirectories).Length;
				int iCurFolder=0;
				//Delete the specified directory if it is empty. Optionally recursively delete any subdirectories which are empty.
				//Declared null at first so it can be called recursively from within it's own definition.
				Action<string> deleteEmptyDir=null;
				deleteEmptyDir=new Action<string>((diCur) => {
					#region deleteEmptyDir
					try {
						if(quitPremature()) {
							logIfNecessary(iCurFolder,totalDirectories,true,"");
							status("Quit directory delete premature");
							return;
						}
						foreach(string d in Directory.GetDirectories(diCur,"*.*",SearchOption.TopDirectoryOnly)) { //All directories that should delete must delete in order to return true for this directory.
							deleteEmptyDir(d);
						}
						logIfNecessary(iCurFolder,totalDirectories,false,diCur);
						iCurFolder++;
						//Throws exception if directory is not empty. That's what we want.
						Directory.Delete(diCur,false);
					}
					catch(IOException e) {
						//Directory is most likely not empty. 
						//It is much faster to try to delete the directory and have it fail than to query the diretory for existing files beforehand.
						e.DoNothing();
					}
					catch(Exception e) {
						//If we get here then a directory that should have been deleted was not deleted.
						statusE(e);
					}
					#endregion
				});
				status("Scanning "+totalDirectories.ToString()+" directories.");
				deleteEmptyDir(di.FullName);
				status("Directory scan complete.");
				#endregion
			}
			catch(Exception e) {
				WriteException(e,CLEANUP_DIR);
			}
		}
		
		private static void CloseFile(string subDirectory) {
			try {
				lock (_lock) {
					StreamWriter file = null;
					DateTime created = DateTime.Now;
					if(!TryGetFile(subDirectory,out file,out created)) {
						return;
					}
					file.Dispose();
					_files.Remove(subDirectory);
				}
			}
			catch { }
		}

		public static void ParseLogs(string directory) {
			try {
				DirectoryInfo di = new DirectoryInfo(directory);
				if(!di.Exists) {
					return;
				}
				using(StreamWriter sw = new StreamWriter(ODFileUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory,"Errors - "+DateTime.Now.ToString("MM-dd-yy HH-mm-ss")+".txt"))) {
					ParseDirectory(di,sw);
					foreach(DirectoryInfo diSub in di.GetDirectories()) {
						ParseDirectory(diSub,sw);
					}
				}
			}
			catch(Exception e) {
				throw e;
			}
		}

		private static void ParseDirectory(DirectoryInfo di,StreamWriter sw) {
			FileInfo[] files = di.GetFiles("*.txt");
			foreach(FileInfo fi in files) {
				using(StreamReader sr = new StreamReader(fi.FullName)) {
					string line = "";
					string lower = "";
					while(sr.Peek()>0) {
						line=sr.ReadLine();
						lower=line.ToLower();
						if(!lower.Contains("failed")) {
							continue;
						}
						sw.WriteLine(line);
					}
				}
			}
		}

		public static bool SingleFileLoggerExists(string subDirectory) {
			subDirectory=ScrubSubDirPath(subDirectory);
			FileInfo fi = new FileInfo(ODFileUtils.CombinePaths(GetDirectory(subDirectory),subDirectory+".txt"));
			return fi.Exists;
		}

		private static bool TryGetFile(string subDirectory,out StreamWriter file,out DateTime created) {
			file=null;
			created=DateTime.MinValue;
			lock (_lock) {
				object[] obj = null;
				if(!_files.TryGetValue(subDirectory,out obj)) {
					return false;
				}
				file=(StreamWriter)obj[0];
				created=(DateTime)obj[1];
				return true;
			}
		}

		private static StreamWriter Open(string subDirectory,bool singleFileOnly) {
			try {
				lock (_lock) {
					StreamWriter file = null;
					DateTime created = DateTime.MinValue;
					if(TryGetFile(subDirectory,out file,out created)) { //file has been created
						if(singleFileOnly) {
							return file;
						}
						if(DateTime.Today==created.Date) { //it was created today
							if((file.BaseStream.Length/1024)<=MAX_FILE_SIZE_KB) { //it is within the acceptable size limit
								return file;
							}
						}
						CloseFile(subDirectory);
					}
					file=new StreamWriter(GetFileName(subDirectory,DateTime.Today,singleFileOnly),true);
					file.AutoFlush=true;
					_files[subDirectory]=new object[] { file,DateTime.Now };
					return file;
				}
			}
			catch {
				return null;
			}
		}

		///<summary>The full path and file name will be kept safe by Logger itself. Logger will not create a path name that won't comply with Windows file system.
		///The one exception is where we let the implementer set the subDirectory. This subDirectory could be any string so we will scrub that string here and make it comply.</summary>
		private static string ScrubSubDirPath(string dirIn) {
			//Remove any extra directory delimiters from the subDirectory.
			string dirOut=dirIn.TrimEnd('\\').TrimStart('\\');
			//This exhaustive list was found here. https://stackoverflow.com/a/33608950
			var replaceThese=Path.GetInvalidPathChars().Union(new char[] { ':', '?', '/', '!', '*', '%', '.', ',', }).ToList().FindAll(x => dirOut.Contains(x));
			foreach(char c in replaceThese) {
				dirOut=dirOut.Replace(c,'-');
			}
			return dirOut;
		}

		private static string GetFileNameSingleFileOnly(string subDirectory) {
			DirectoryInfo di = new DirectoryInfo(GetDirectory(subDirectory));
			FileInfo fi = new FileInfo(ODFileUtils.CombinePaths(di.FullName,subDirectory+".txt"));
			if(!di.Exists) {
				di.Create();
			}
			return fi.FullName;
		}

		private static string GetFileName(string subDirectory,DateTime date,bool singleFileOnly) {
			if(singleFileOnly) {
				return GetFileNameSingleFileOnly(subDirectory);
			}
			string formattedDate = date.ToString("yy-MM-dd");
			DirectoryInfo di = new DirectoryInfo(ODFileUtils.CombinePaths(GetDirectory(subDirectory),formattedDate));
			if(!di.Exists) {
				di.Create();
			}
			int fileNum = 1;
			do {
				FileInfo fi = new FileInfo(ODFileUtils.CombinePaths(di.FullName,formattedDate+" ("+fileNum.ToString("D3")+").txt"));
				if(!fi.Exists) { //file doesn't exist yet
					return fi.FullName;
				}
				if((fi.Length/1024)<=MAX_FILE_SIZE_KB) { //file is small enough to use
					return fi.FullName;
				}
				if(++fileNum>=1000) { //only create 1000 files max
					List<FileInfo> fileInfos = new List<FileInfo>(di.GetFiles(formattedDate+"*"));
					fileInfos.Sort(SortFileByModifiedTimeDesc);
					fileInfos[0].Delete();
					return fileInfos[0].FullName;
				}
			} while(true);
		}

		private static int SortFileByModifiedTimeDesc(FileInfo x,FileInfo y) {
			return x.LastWriteTime.CompareTo(y.LastWriteTime);
		}

		public delegate void WriteLineDelegate(string data,LogLevel logLevel);

		public class LoggerEventArgs:EventArgs {
			public string Data;
			public LogLevel LogLevel;
			public LoggerEventArgs(string data,LogLevel logLevel) {
				Data=data;
				LogLevel=logLevel;
			}
		};
		#endregion

		public interface IWriteLine {
			LogLevel LogLevel { get; set; }
			void WriteLine(string data,LogLevel logLevel,string subDirectory = "");
		}

		public interface IWriteClinicLogs : IWriteLine {
			long ClinicNum { get; set; }
		}
	}

	public class ClinicLogger:Logger.IWriteClinicLogs {
		public LogLevel LogLevel {
			get {
				return _logger.LogLevel;
			}
			set {
				_logger.LogLevel=value;
			}
		}

		[ThreadStatic]
		protected static long _clinicNum;
		public long ClinicNum { 
			get {
				return _clinicNum;
			}
			set {
				_clinicNum=value;
			}
		}
		protected Func<long,string> _getName;
		protected Func<bool> _hasClinics;
		protected readonly Logger.IWriteLine _logger;

		public ClinicLogger(Logger.IWriteLine logger,Func<long,string> getName=null,Func<bool> hasClinics=null) {
			_logger=logger;
			_getName=getName;
			_hasClinics=hasClinics;
		}

		protected string GetDirectory(string subDirectory="") {
			string clinicStr="";//Start at base directory for HQ clinic.
			if(ClinicNum!=0) {
				clinicStr=$"Clinic_{ClinicNum}"+((_getName is null) ? "" : $"_{_getName(ClinicNum)}");
			}
			else if(!(_hasClinics is null)) {
				try {
					if(_hasClinics()) {
						clinicStr="HQ";
					}
				}
				catch(Exception e) {
					//Invoker is not in a state to figure out if clinics are enabled yet, log in base directory.
				}
			}
			return Path.Combine(clinicStr,subDirectory);
		}

		public virtual void WriteLine(string data,LogLevel logLevel,string subDirectory = "") {
			_logger.WriteLine(data,logLevel,GetDirectory(subDirectory));
		}
	}

	///<summary>Class that writes to the specified logger directory.</summary>
	public class LogWriter : Logger.IWriteLine {
		public virtual LogLevel LogLevel { get; set; }
		public string BaseDirectory;
		///<summary>Optional action that can be performed at the end of WriteLine.</summary>
		public Action<string,LogLevel> EndWriteLine;

		///<summary>For serialization.</summary>
		public LogWriter() {
		}

		public LogWriter(LogLevel logLevel,string baseDirectory) {
			LogLevel=logLevel;
			BaseDirectory=baseDirectory;
		}

		public virtual void WriteLine(string data,LogLevel logLevel,string subDirectory="") {
			if(logLevel>LogLevel) {
				return;
			}
			Logger.WriteLine(data,ODFileUtils.CombinePaths(BaseDirectory,subDirectory));
			EndWriteLine?.Invoke(data,logLevel);
		}
	}

	///<summary>Class that generates a unique ID and includes it in every line it logs.</summary>
	public class LogRequest : LogWriter {
		private string _requestId;

		///<summary>For serialization.</summary>
		public LogRequest() {
		}

		public LogRequest(LogLevel logLevel,string baseDirectory) : base(logLevel,baseDirectory) {
			_requestId=Guid.NewGuid().ToString();
		}

		public override void WriteLine(string data,LogLevel logLevel,string subDirectory="") {
			base.WriteLine("Request ID: "+_requestId+"\tMethod: "+GetCallingMethod()+"\t"+data,logLevel,subDirectory);
		}

		///<summary>Gets the name of the method that is calling WriteLine().</summary>
		private string GetCallingMethod() {
			try {
				for(int i=1;i<4;i++) {//Start at stackframe(1) because 0 is the parent of this method.		
					StackFrame frame=new StackFrame(i);
					System.Reflection.MethodBase method=frame.GetMethod();
					if(!method.Name.ToLower().Contains("writeline")) {
						return method.ReflectedType.FullName+"."+method.Name;
					}
				}
			}
			catch(Exception e) {
				e.DoNothing();
			}
			//Return blank if we couldn't find a method name that wasn't ourself in the first 5 frames
			return "";
		}
	}

	///<summary>An implementation of the IWriteLine interface that allows you to specify a delegate to run.</summary>
	public class LogDelegate:Logger.IWriteLine {
		public LogLevel LogLevel { get; set; }
		private Logger.WriteLineDelegate _del;

		///<summary>For serialization.</summary>
		public LogDelegate() {
			_del=new Logger.WriteLineDelegate((data,logLevel) => { });
		}

		public LogDelegate(Logger.WriteLineDelegate del) {
			_del=del;
		}

		public void WriteLine(string data,LogLevel logLevel,string subDirectory="") {
			_del(data,logLevel);
		}
	}

	///<summary>0=Error, 1=Information, 2=Verbose</summary>
	public enum LogLevel {
		///<summary>0 Logs only errors.</summary>
		Error = 0,
		///<summary>1 Logs information plus errors.</summary>
		Information = 1,
		///<summary>2 Most verbose form of logging (use sparingly for very specific troubleshooting). Logs all entries all the time.</summary>
		Verbose = 2
	}

	///<summary>Used by LogToPath to decide if it needs to make a start entry, end entry, or unspecified for empirical log entries.</summary>
	public enum LogPhase {
		Unspecified,
		Start,
		End,
	}

	///<summary>Used by LogToPath to decide which folder to log to.</summary>
	public enum LogPath {
		Signals,
		ChartModule,
		AccountModule,
		OrthoChart,
		Threads,
		Startup,
	}
}
