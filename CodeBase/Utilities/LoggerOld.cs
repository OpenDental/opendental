using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !DOT_NET_CORE && !DOT_NET_STANDARD
using System.Windows.Forms;
#endif

namespace CodeBase {
	public partial class Logger {

#if !DOT_NET_CORE && !DOT_NET_STANDARD
		public static Logger openlog=new Logger(ODFileUtils.GetProgramDirectory()+"openlog.txt");
#endif

		public Logger(string pLogFile) {
			logFile=pLogFile;
		}

		///<summary>Convert a severity code into a string.</summary>
		public static string SeverityToString(Severity sev) {
			switch(sev) {
				case Severity.NONE:
					return "NONE";
				case Severity.DEBUG:
					return "DEBUG";
				case Severity.INFO:
					return "INFO";
				case Severity.WARNING:
					return "WARNING";
				case Severity.ERROR:
					return "ERROR";
				case Severity.FATAL_ERROR:
					return "FATAL ERROR";
				default:
					break;
			}
			return "UNKNOWN SEVERITY";
		}

		public static int MaxSeverityStringLength() {
			int maxlen=0;
			for(Severity sev = (Severity)1;sev<(Severity)7;sev++) {
				maxlen=Math.Max(maxlen,SeverityToString(sev).Length);
			}
			return maxlen;
		}

		///<summary>Used to write the logger file to the MyDocuments directory to avoid having to run OD as admin. Use sparingly, should only be called once per application instance.</summary>
		public static void UseMyDocsDirectory() {
			lock(_lock) {
				if(_canUseMyDocsDir) {
					return;
				}
				CloseLogger();
				_canUseMyDocsDir=true;
			}
		}

		///<summary>Log a message from an unknown source.</summary>
		public bool Log(string message,Severity severity) {
			return Log(null,"",message,false,severity);
		}

		public bool LogMB(string message,Severity severity) {
			return Log(null,"",message,true,severity);
		}

		public bool Log(Object sender,string sendingFunctionName,string message,Severity severity) {
			return Log(sender,sendingFunctionName,message,false,severity);
		}

		public bool LogMB(Object sender,string sendingFunctionName,string message,Severity severity) {
			return Log(sender,sendingFunctionName,message,true,severity);
		}

		///<summary>Log a message to the log text file and add a description of the sender (for debugging purposes). If sender is null then a description of the sender is not printed. Always returns false so that a calling boolean function can return at the same time that it logs an error message.</summary>
		public bool Log(Object sender,string sendingFunctionName,string message,bool msgBox,Severity severity) {
			if(severity<level) {//Only log messages with a severity matches the current level. This will even skip message boxes.
				return false;
			}
			try {
				if(sender!=null) {
					if(sendingFunctionName!=null && sendingFunctionName.Length>0) {
						message=sender.ToString()+"."+sendingFunctionName+": "+message;
					}
					else {
						message=sender.ToString()+": "+message;
					}
				}
				else if(sendingFunctionName!=null && sendingFunctionName.Length>0) {
					message=sendingFunctionName+": "+message;
				}
				int procId=System.Diagnostics.Process.GetCurrentProcess().Id;
				message=DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")+" "+procId.ToString().PadLeft(6,'0')+" "+
					SeverityToString(severity)+" "+message;
#if DOT_NET_CORE || DOT_NET_STANDARD
			}
			//Allow to throw.
			finally {
			}
#else
				if(msgBox) {
					using MsgBoxCopyPaste mbox=new MsgBoxCopyPaste(message);
					mbox.ShowDialog();
				}
			}
			catch(Exception e) {
				MessageBox.Show(e.ToString());
			}
#endif
			//File access is always exclusive, so if we cannot access the file, we can try again for a little while
			//and hope that the other process will release the file.
			bool tryagain=true;
			int numtries=0;
			while(tryagain && numtries<5) {
				tryagain=false;
				numtries++;
				try {
					if(logFile!=null) {
						//Ensure that the log file always exists before trying to read it.
						if(!File.Exists(logFile)) {
							try {
								FileStream fs=File.Create(logFile);
								fs.Dispose();
							}
							catch {
							}
						}
						else {
							//Make the log file roll into the old log file when it reaches the roll byte size.
							System.IO.StreamReader sr=new System.IO.StreamReader(logFile);
							if(sr!=null) {
								Stream st=sr.BaseStream;
								long fileLength=st.Length;
								if(fileLength>=logRollByteCount) {
									try {
										File.Copy(logFile,logFile+".old.txt");
									}
									catch {
									}
									try {
										File.Delete(logFile);
									}
									catch {
									}
									fileLength=0;
								}
								st.Dispose();
								sr.Dispose();
								if(fileLength<1) {
									try {
										FileStream fs=File.Create(logFile);
										fs.Dispose();
									}
									catch {
									}
								}
							}
						}
						//Re-open the log file 
						System.IO.StreamWriter sw=new System.IO.StreamWriter(logFile,true);//Open the file exclusively.
						if(sw!=null) {
							sw.WriteLine(message);
							sw.Flush();
							sw.Dispose();//Close the file to allow exclusive access by other instances of OpenDental.
						}
					}
				}
				catch {
					tryagain=true;
				}
			}
			return false;
		}
	}
}
