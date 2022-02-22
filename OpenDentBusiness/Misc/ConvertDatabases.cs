using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {
		private static Version _latestVersion;
		private static List<ConvertDatabasesMethodInfo> _listConvertMethods;

		///<summary>Gets a list of convert databases method infos and their corresponding version information based on their method name.</summary>
		private static List<ConvertDatabasesMethodInfo> ListConvertMethods {
			get {
				if(_listConvertMethods==null) {
					_listConvertMethods=GetAllVersions();
				}
				return _listConvertMethods;
			}
		}
		
		///<summary>Returns a version object that correlates to the last convert databases method on file.</summary>
		public static Version LatestVersion {
			get {
				if(_latestVersion==null) {
					_latestVersion=ListConvertMethods[ListConvertMethods.Count-1].VersionCur;
				}
				return _latestVersion;
			}
		}

		///<summary>Uses reflection to get all "version" methods from the ConvertDatabasesX classes that match the "ToX_X_X" pattern.
		///Also sorts the methods in the correct order of which they should be invoked.</summary>
		private static List<ConvertDatabasesMethodInfo> GetAllVersions() {
			//Get all the private methods from the ConvertDatabases class via reflection.
			MethodInfo[] arrayConvertDbMethods=(typeof(ConvertDatabases)).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
			//Sort the methods so that they are numerically in the order that we require they be invoked in.
			List<ConvertDatabasesMethodInfo> listConvertMethods=new List<ConvertDatabasesMethodInfo>();
			foreach(MethodInfo methodInfo in arrayConvertDbMethods) {
				//Make sure that the only methods we add to our list match our ToX_X_X pattern.
				if(!Regex.Match(methodInfo.Name,ConvertDatabasesMethodInfo.PATTERN_METHOD_INFO,RegexOptions.IgnoreCase).Success) {
					continue;//This method does not follow our pattern and is most likely a helper method.
				}
				listConvertMethods.Add(new ConvertDatabasesMethodInfo(methodInfo));
			}
			//Make sure that the list of methods are sorted in ascending order (least to greatest).
			listConvertMethods.Sort((ConvertDatabasesMethodInfo x,ConvertDatabasesMethodInfo y) => { return x.VersionCur.CompareTo(y.VersionCur); });
			return listConvertMethods;
		}

		///<summary>Uses reflection to invoke private methods of the ConvertDatabase class in order from least to greatest if needed.
		///The old way of converting the database was to manually daisy chain methods together.
		///The new way is to just add a method that follows a strict naming pattern which this method will invoke when needed.</summary>
		public static void InvokeConvertMethods() {
			DataConnection.CommandTimeout=43200;//12 hours, because conversion commands may take longer to run.
			ConvertDatabases.To2_8_2();//begins going through the chain of conversion steps
			Logger.DoVerboseLoggingArgs doVerboseLogging=Logger.DoVerboseLogging;
			ODException.SwallowAnyException(() => {
				//Need to run queries here because PrefC has not been initialized.
				string command="SELECT ValueString FROM preference WHERE PrefName='HasVerboseLogging'";
				string valueString=Db.GetScalar(command);
				if(valueString.ToLower().Split(',').ToList().Exists(x => x==Environment.MachineName.ToLower())) {
					Logger.DoVerboseLogging=() => true;
					//Switch logger to a directory that won't have permissions issues.
					Logger.UseMyDocsDirectory();
				}
				Logger.LogVerbose("Starting convert script");
			});
			//Continue going through the chain of conversion methods starting at v17.1.1 via reflection.
			//Loop through the list of convert databases methods from front to back because it has already been sorted (least to greatest).
			foreach(ConvertDatabasesMethodInfo convertMethodInfo in ListConvertMethods) {
				//This pattern of using reflection to invoke our convert methods started in v17.1 so we will skip all methods prior to that version.
				if(convertMethodInfo.VersionCur < new Version(17,1)) {
					continue;
				}
				//Skip all methods that are below or equal to our "from" version.
				if(convertMethodInfo.VersionCur<=FromVersion) {
					continue;
				}
				//This convert method needs to be invoked.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: " //No translations in convert script.
					+convertMethodInfo.VersionCur.ToString(3));//Only show the major, minor, build (preserves old functionality).
				try {
					//Use reflection to invoke the private static method.
					convertMethodInfo.MethodInfoCur.Invoke(null,new object[] { });
				}
				catch(Exception ex) {
					string message=Lans.g("ClassConvertDatabase","Convert Database failed ");
					try { 
						string methodName=convertMethodInfo.MethodInfoCur.Name;
						if(!string.IsNullOrEmpty(methodName)) {
							message+=Lans.g("ClassConvertDatabase","during: ")+methodName+"() ";
						}
						string command=Db.LastCommand;
						if(!string.IsNullOrEmpty(command)) {
							message+=Lans.g("ClassConvertDatabase","while running: ")+command+";";
						}
					}
					catch(Exception e) {
						e.DoNothing();//If this fails for any reason then just continue.
					}
					throw new Exception(message+"  "+ex.Message+"  "+ex.InnerException.Message,ex.InnerException);
				}
				//Update the preference that keeps track of what version Open Dental has successfully upgraded to.
				//Always require major, minor, build, revision.  Will throw an exception if the revision was not explicitly set (which we always set).
				Prefs.UpdateStringNoCache(PrefName.DataBaseVersion,convertMethodInfo.VersionCur.ToString(4));
			}
			ODException.SwallowAnyException(() => {
				Logger.LogVerbose("Ending convert script");
				Logger.DoVerboseLogging=doVerboseLogging;
			});
			DataConnection.CommandTimeout=3600;//Set back to default of 1 hour.
		}
	}

	///<summary>A helper class to quickly manage convert databases methods.  Provides access to the corresponding MethodInfo and Version.</summary>
	public class ConvertDatabasesMethodInfo {
		///<summary>This is the regular expression pattern used to match our convert databases method version pattern of "ToX_X_X".</summary>
		public const string PATTERN_METHOD_INFO=@"^To([0-9]+)_([0-9]+)_([0-9]+)$";
		private MethodInfo _methodInfo;
		private Version _version;

		public MethodInfo MethodInfoCur {
			get {
				return _methodInfo;
			}
		}

		public Version VersionCur {
			get {
				return _version;
			}
		}

		///<summary>The method info passed in should have a name that follows the ToX_X_X pattern.
		///Throws an exception if pattern not followed.</summary>
		public ConvertDatabasesMethodInfo(MethodInfo methodInfo) {
			_methodInfo=methodInfo;
			_version=GetVersionFromConvertMethod(methodInfo);
		}

		///<summary>Uses a regular expression to extract a version from the name of the method passed in.
		///The method info passed in should have a name that follows the ToX_X_X pattern.
		///Throws an exception if the method name pattern was not followed.</summary>
		private Version GetVersionFromConvertMethod(MethodInfo methodInfo) {
			Match match=Regex.Match(methodInfo.Name,ConvertDatabasesMethodInfo.PATTERN_METHOD_INFO,RegexOptions.IgnoreCase);
			if(!match.Success) {
				throw new ApplicationException("Invalid convert databases method passed into GetVersionFromConvertMethod.");
			}
			int major=PIn.Int(match.Result("$1"));
			int minor=PIn.Int(match.Result("$2"));
			int build=PIn.Int(match.Result("$3"));
			return new Version(major,minor,build,0);
		}
	}
}
