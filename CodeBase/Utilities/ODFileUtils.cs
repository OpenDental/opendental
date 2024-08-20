using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
#if !DOT_NET_STANDARD
using System.Drawing.Imaging;
#endif
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
#if !DOT_NET_CORE && !DOT_NET_STANDARD
using System.Windows.Forms;
#endif

namespace CodeBase {
	public class ODFileUtils {

		[DllImport("kernel32.dll",SetLastError=true,CharSet=CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,out ulong lpFreebytesAvailable,out ulong lpTotalNumberOfBytes,out ulong lpTotalNumberOfFreeBytes);

		///<summary>This is a class scope variable in order to ensure that the random value is only seeded once for each time OD is launched.
		///Otherwise, if instantiated more often, then the same random numbers are generated over and over again.</summary>
		private static Random _rand=new Random();
		private static List<string> _listKnownFileTypes=null;

		///<summary>Removes a trailing path separator from the given string if one exists.</summary>
		public static string RemoveTrailingSeparators(string path){
			while(path!=null && path.Length>0 && (path[path.Length-1]=='\\' || path[path.Length-1]=='/')) {
				path=path.Substring(0,path.Length-1);
			}
			return path;
		}

		public static string CombinePaths(string path1,string path2) {
			return CombinePaths(new string[] { path1,path2 });
		}

		public static string CombinePaths(string path1,string path2,char separator) {
			return CombinePaths(new string[] { path1,path2 },separator);
		}

		public static string CombinePaths(string path1,string path2,string path3) {
			return CombinePaths(new string[] { path1,path2,path3 });
		}

		public static string CombinePaths(string path1,string path2,string path3,char separator) {
			return CombinePaths(new string[] { path1,path2,path3 },separator);
		}

		public static string CombinePaths(string path1,string path2,string path3,string path4) {
			return CombinePaths(new string[] { path1,path2,path3,path4 });
		}

		public static string CombinePaths(string path1,string path2,string path3,string path4,char separator) {
			return CombinePaths(new string[] { path1,path2,path3,path4 },separator);
		}

		///<summary>OS independent path cominations. Ensures that each of the given path pieces are separated by the correct path separator for the current operating system. There is guaranteed not to be a trailing path separator at the end of the returned string (to accomodate file paths), unless the last specified path piece in the array is the empty string.</summary>
		public static string CombinePaths(string[] paths){
			string finalPath="";
			for(int i=0;i<paths.Length;i++){
				string path=RemoveTrailingSeparators(paths[i]);
				//Add an appropriate slash to divide the path peices, but do not use a trailing slash on the last piece.
				if(i<paths.Length-1){
					if(path!=null && path.Length>0){
						path=path+Path.DirectorySeparatorChar;
					}
				}
				finalPath=finalPath+path;
			}
			return finalPath;
		}

		///<summary>Ensures that each of the given path pieces are separated by the passed in separator character. 
		///There is guaranteed not to be a trailing path separator at the end of the returned string (to accomodate file paths), 
		///unless the last specified path piece in the array is the empty string.</summary>
		public static string CombinePaths(string[] paths,char separator) {
			return CombinePaths(paths).Replace(Path.DirectorySeparatorChar,separator);
		}

		///<summary>This function takes a valid folder path.  Accepts UNC paths as well.  freeBytesAvail will contain the free space in bytes of the drive containing the folder.
		///It returns false if the function fails.</summary>
		public static bool GetDiskFreeSpace(string folder,out ulong freeBytesAvail) {
			freeBytesAvail=0;
			if(!folder.EndsWith("\\")) {
				folder+="\\";
			}
			ulong totBytes=0;
			ulong totFreeBytes=0;
			if(GetDiskFreeSpaceEx(folder,out freeBytesAvail,out totBytes,out totFreeBytes)) {
				return true;
			}
			else {
				return false;
			}
		}

		///<summary>Creates a new randomly named file in the given directory path with the given extension and returns the full path to the new file.
		///The file name will include the local date and time down to the second.</summary>
		public static string CreateRandomFile(string dir,string ext,string prefix=""){
			if(ext.Length>0 && ext[0]!='.'){
				ext='.'+ext;
			}
			bool fileCreated=false;
			string filePath="";
			const string randChrs="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			do{
				string fileName=prefix;
				for(int i=0;i<6;i++){
					fileName+=randChrs[_rand.Next(0,randChrs.Length-1)];
				}
				fileName+=DateTime.Now.ToString("yyyyMMddhhmmss");
				filePath=CombinePaths(dir,fileName+ext);
				FileStream fs=null;
				try{
					fs=File.Create(filePath);
					fs.Dispose();
					fileCreated=true;
				}
				catch{
				}
			}while(!fileCreated);
			return filePath;
		}

		///<summary>Throws exceptions when there are permission issues.  Creates a new randomly named subdirectory inside the given directory path and returns the full path to the new subfolder.</summary>
		public static string CreateRandomFolder(string dir) {
			bool isFolderCreated=false;
			string folderPath="";
			const string randChrs="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			do {
				string subDirName="";
				for(int i=0;i<6;i++) {
					subDirName+=randChrs[_rand.Next(0,randChrs.Length-1)];
				}
				subDirName+=DateTime.Now.ToString("yyyyMMddhhmmss");
				folderPath=CombinePaths(dir,subDirName);
				if(!Directory.Exists(folderPath)) {
					Directory.CreateDirectory(folderPath);
					isFolderCreated=true;
				}
			} while(!isFolderCreated);
			return folderPath;
		}

		///<summary>Appends the suffix at the end of the file name but before the extension.</summary>
		public static string AppendSuffix(string filePath,string suffix) {
			string ext=Path.GetExtension(filePath);
			return CombinePaths(Path.GetDirectoryName(filePath),Path.GetFileNameWithoutExtension(filePath)+suffix+ext);
		}

		///<summary>Removes invalid characters from the passed in file name.</summary>
		public static string CleanFileName(string fileName) {
			return string.Join("_",fileName.Split(Path.GetInvalidFileNameChars()));
		}

		///<summary>Start the given process.  
		///If using a Thinfinity compiled version of Open Dental or this is an AppStream instance, pass through to the odcloud client to start the process locally.</summary>
		public static void ProcessStart(Process process) {
			if(ODEnvironment.IsCloudInstance) {
				//We will only use the FileName and Arguments from the process's StartInfo.  Only non-web builds utilize the entire process.
				ProcessStart(process.StartInfo.FileName,process.StartInfo.Arguments);
			}
			else {
				process.Start();
			}
		}
		
		///<summary>Start a new process with the given path and arguments.  
		///If using a THINFINITY compiled version of Open Dental, pass through to the odcloud client to start the process locally.</summary>
		///<param name="doWaitForODCloudClientResponse">If true, will wait for ODCloudClient and throw any exceptions from it.</param>
		///<param name="createDirIfNeeded">If included, will create the directory if it doesn't exist.</param>
		public static Process ProcessStart(string path,string commandLineArgs="",bool doWaitForODCloudClientResponse=false,string createDirIfNeeded="",bool tryLaunch=false) {
			if(ODEnvironment.IsCloudInstance) {
				ODCloudClient.LaunchFileWithODCloudClient(path,commandLineArgs,doWaitForResponse:doWaitForODCloudClientResponse,
					createDirIfNeeded:createDirIfNeeded,tryLaunch:tryLaunch);
				return null;
			}
			if(!string.IsNullOrEmpty(createDirIfNeeded) && !Directory.Exists(createDirIfNeeded)) {
				Directory.CreateDirectory(createDirIfNeeded);
			}
			return Process.Start(path,commandLineArgs);
		}
		
		///<summary>Write the given text to the given file.  
		///If using a Thinfinity compiled version of Open Dental or is an AppStream instance, pass through to the odcloud client for File IO.</summary>
		public static void WriteAllText(string filePath,string text,bool doOverwriteFile=true) {
			if(ODEnvironment.IsCloudInstance) {
				try {
					ODCloudClient.WriteFile(filePath,text,doOverwriteFile);
				}
				catch(ODException ex) {
					if(ex.ErrorCode==(int)ODException.ErrorCodes.FileExists) {
						return;
					}
					throw;
				}
			}
			else {
				if(!File.Exists(filePath) || doOverwriteFile){
					File.WriteAllText(filePath,text);
				}
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a THINFINITY compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath,bool doStartWithoutExtraFile=false) {
			return WriteAllTextThenStart(filePath,fileText,processPath,"",doStartWithoutExtraFile:doStartWithoutExtraFile);
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a THINFINITY compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally. Throws exceptions.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath,string commandLineArgs,bool doStartWithoutExtraFile=false) {
			if(ODEnvironment.IsCloudInstance) {
				ODCloudClient.LaunchFileWithODCloudClient(processPath,commandLineArgs,filePath,fileText,doWaitForResponse:true,doStartWithoutExtraFile:doStartWithoutExtraFile);
				return null;
			}
			else {
				File.WriteAllText(filePath,fileText);
				return Process.Start(processPath,commandLineArgs);
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a Thinfinity compiled version of Open Dental or an AppStream instance, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,Encoding encoding,string processPath,string commandLineArgs) {
			if(ODEnvironment.IsCloudInstance) {
				//Purposefully omit encoding.  This can be an enhancement if needed.
				ODCloudClient.LaunchFileWithODCloudClient(processPath,commandLineArgs,filePath,fileText);
				return null;
			}
			else {
				File.WriteAllText(filePath,fileText,encoding);
				return Process.Start(processPath,commandLineArgs);
			}			
		}

		///<summary>Passes the command to the bash shell to run.</summary>
		public static string Bash(string command) {
			string escapedArgs=command.Replace("\"", "\\\"");
			Process process=new Process {
				StartInfo=new ProcessStartInfo {
					FileName="/bin/bash",
					Arguments=$"-c \"{escapedArgs}\"",
					RedirectStandardOutput=true,
					UseShellExecute=false,
					CreateNoWindow=true,
				}
			};
			process.Start();
			string result=process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return result;
		}

		public static bool IsFileInUse(string filePath) {
			if(!File.Exists(filePath)) {
				return false;
			}
			try {
				FileInfo file=new FileInfo(filePath);
				using(FileStream stream=file.Open(FileMode.Open,FileAccess.Read,FileShare.None)) {
					stream.Close();
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return true;
			}
			return false;
		}

#if !DOT_NET_CORE && !DOT_NET_STANDARD

		///<summary>Returns an organized list of known file extensions. Will return the default list if user's machine isn't windows, and if it is windows then it will check their registry for any additional file types they may use such as .DCM.</summary>
		public static List<string> GetListKnownFileTypes(){
			if(_listKnownFileTypes==null){
				//The list is null and unpopulated so fill it.
				try{
					FillListKnownFileTypes();
				}
				catch(Exception ex) {
					ex.DoNothing();//If the fill failed, it will just be the default list.
				}
			}
			return _listKnownFileTypes;
		}

		///<summary>Throws exceptions. Sets ListKnownFileTypes to the default list of file types. Then tries to add known file types from the registry to the list.</summary>
		private static void FillListKnownFileTypes() {
			_listKnownFileTypes=new List<string>(_listFileTypes);//Make a shallow copy of the default list.
			//This can fail for a variety of reasons, namely if the user doesn't have access to the registry or if the computer is not a windows computer.
			//This gets all the extension filetypes inside of the registry.
			List<string> listRegistryFileTypes=Microsoft.Win32.Registry.ClassesRoot.GetSubKeyNames().ToList();
			for(int i=0;i<listRegistryFileTypes.Count;i++){
				//There are files in this section of the registry that don't start with . We trim those out since we're only looking for file extensions.
				if(!listRegistryFileTypes[i].StartsWith(".")) {
					continue;
				}
				if(_listKnownFileTypes.Contains(listRegistryFileTypes[i].ToLower())) {
					continue;
				}
				//Open up the registry entry. If the content type is not present, it's probably not a file so we remove it.
				if(Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(listRegistryFileTypes[i]).GetValue("Content Type")==null){
					continue;
				}
				_listKnownFileTypes.Add(listRegistryFileTypes[i].ToLower());
			}
			_listKnownFileTypes=_listKnownFileTypes.OrderBy(x=>x).ToList();//Organize the list.
		}

		///<summary>Returns true if it's a known filetype, false otherwise. Can take in a full path or just the extension.</summary>
		public static bool IsKnownFileType(string path){
			if(_listKnownFileTypes==null){
				GetListKnownFileTypes();
			}
			return _listKnownFileTypes.Contains(Path.GetExtension(path.ToLower()));
		}


		///<summary>Write the given filebytes and launches a file.</summary>
		///<param name="filePath">The location to write the bytes to.</param>
		///<param name="fileBytes">The bytes to write to the file.</param>
		///<param name="processPath">The path of the file to launch.</param>
		public static Process WriteAllBytesThenStart(string filePath,byte[] fileBytes,string processPath) {
			return WriteAllBytesThenStart(filePath,fileBytes,processPath,"");
		}

		///<summary>Write the given filebytes and launches a file.</summary>
		///<param name="filePath">The location to write the bytes to.</param>
		///<param name="fileBytes">The bytes to write to the file.</param>
		///<param name="processPath">The path of the file to launch.</param>
		///<param name="commandLineArgs">Command line arguments to pass to processPath.</param>
		///<param name="millisecondsToSleep">Time for thread to sleep between the program writing to a file and launching. Not always necessary. Currently only used for dexis integrator.</param>
		public static Process WriteAllBytesThenStart(string filePath,byte[] fileBytes,string processPath,string commandLineArgs,int millisecondsToSleep=0) {
			if(ODEnvironment.IsCloudInstance) {
				string byteString=Convert.ToBase64String(fileBytes);
				ODCloudClient.LaunchFileWithODCloudClient(processPath,commandLineArgs,filePath,byteString,"binary");
				return null;
			}
			else {
				File.WriteAllBytes(filePath,fileBytes);
				if(millisecondsToSleep>0) {
					System.Threading.Thread.Sleep(millisecondsToSleep);
				}
				if(!string.IsNullOrEmpty(processPath)) {
					return Process.Start(processPath,commandLineArgs);
				}
				return Process.Start(filePath,commandLineArgs);
			}
		}

		///<summary>Reduces image size by changing it to Jpeg format and reducing image quality to 40%.</summary>
		public static string Compress(Bitmap image) {
			using(Bitmap bmp=new Bitmap(image))
			using(MemoryStream ms=new MemoryStream()) {
				ImageCodecInfo[] codecs=ImageCodecInfo.GetImageEncoders();
				ImageCodecInfo jpgEncoder=codecs.First(x => x.FormatID==ImageFormat.Jpeg.Guid);
				System.Drawing.Imaging.Encoder encoder=System.Drawing.Imaging.Encoder.Quality;
				EncoderParameters encoderParameters=new EncoderParameters(1);
				EncoderParameter encoderParameter=new EncoderParameter(encoder,40L);//Reduce quality to 40% of original
				encoderParameters.Param[0]=encoderParameter;
				bmp.Save(ms,jpgEncoder,encoderParameters);
				encoderParameters.Dispose();
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		///<summary>Returns the directory in which the program executable rests. To get the full path for the program executable, use Applicaiton.ExecutablePath.</summary>
		public static string GetProgramDirectory() {
			int endPos=Application.ExecutablePath.LastIndexOf(Path.DirectorySeparatorChar);
			return Application.ExecutablePath.Substring(0,endPos+1);
		}

		/// <summary>Creates a list of all filepaths found in the given text. If path is to specific file (ex.- ~/test/test.txt)
		/// then the parent directory will be returned (~/test).
		/// Overview: Files can start with \\ or, drive colon slash (ex. C:\ or F:/). 
		/// Filepaths that end with folder (no extension) must end with slash followed by either a space (or return), period, comma, semi-colon, or end-of-file
		/// Filepaths that end with an extension (.txt) can be followed with a space (or return), period, comma or semi-colon, or end-of-file and stil be found
		/// Regex breakdown: Split into two groups: \\ and X:\ (where X is any mapped drive letter)
		///		All capturing groups will be found based on the existence of a space (or return), period, comma, semi-colon, or end-of-file
		/// First alternative capuring group: 
		///		(\\\\\w(([\w. \\-]*?)(?=(\\)[\s,.;]|(\\)\z) -- start with \\ and end in slash
		///		| [\w. \\-]*?(\.[a-zA-Z]{1,4}(?=[\s,.;]|\z)))) -- still staring with \\ but ending in a file extension (i.e .txt)
		/// Second alternative capturing group: 
		///		([a-zA-Z]\:(\\|\/) -- start with letter drive immediately followed by colon and a slash.
		///		((\s|\z) | \w[\w-. \\\/]*?((?=(\\|\/)[\s,.;](\\|\/)\z) -- Find all paths without extensions
		///		| (\.[a-zA-Z]{1,4}(?=[\s,.;]))))) -- OR find all paths with a file extension
		/// </summary>
		/// <param name="text">Plain text that could contain filepaths.</param>
		/// <returns>List of UNC Paths</returns>
		public static List<string> GetFilePathsFromText(string text) {
			List<string> listStringMatches=Regex.Matches(text,
				@"(\\\\\w(([\w. \\-]*?)(?=(\\)[\s,.;]|(\\)\z)|[\w. \\-]*?(\.[a-zA-Z]{1,4}(?=[\s,.;]|\z))))|([a-zA-Z]{1}\:(\\|\/)((\s|\z)|\w[\w-. \\\/]*((?=(\\|\/)[\s,.;]|(\\|\/)\z)|(\.[a-zA-Z]{1,4}(?=[\s,.;]|\z)))))")
				.OfType<Match>().Select(m => m.Groups[0].Value).Distinct().ToList();
			List<string> folderPathsOnly=new List<String>(listStringMatches.Count);
			foreach(string match in listStringMatches) {
				string folderPath=match;
				try {
					//In regex we pick up extra white space but we don't want to open the file explorer with a file path with white space attached 
					folderPath=Regex.Replace(folderPath,@"[\s]+$","");
					//If string has extension, assuming specific file
					if(Path.GetExtension(folderPath)!="") {
						//If text is a specific file, truncate to the parent directory
						folderPath=new FileInfo(folderPath).Directory.FullName;
					}
				}
				catch(Exception ex) {
					//We don't want this method to throw any errors. If the path doesn't exist we want to preserve what was found and throw when the 
					//user clicks to navigate to the selected path. See OpenFileExplorer().
					ex.DoNothing();
				}
				folderPathsOnly.Add(folderPath);
			}
			return folderPathsOnly;
		}

#endif

		private static readonly List<string> _listFileTypes=new List<string>(){
			#region Known file endings
			".323",
			".3g2",
			".3gp",
			".3gp2",
			".3gpp",
			".7z",
			".aa",
			".aac",
			".aaf",
			".aax",
			".ac3",
			".aca",
			".accda",
			".accdb",
			".accdc",
			".accde",
			".accdr",
			".accdt",
			".accdw",
			".accft",
			".acx",
			".addin",
			".ade",
			".adobebridge",
			".adp",
			".adt",
			".adts",
			".afm",
			".ai",
			".aif",
			".aifc",
			".aiff",
			".air",
			".amc",
			".application",
			".art",
			".asa",
			".asax",
			".ascx",
			".asd",
			".asf",
			".ashx",
			".asi",
			".asm",
			".asmx",
			".aspx",
			".asr",
			".asx",
			".atom",
			".au",
			".avi",
			".axs",
			".bas",
			".bat",
			".bcpio",
			".bin",
			".bmp",
			".c",
			".cab",
			".caf",
			".calx",
			".cat",
			".cc",
			".cd",
			".cdda",
			".cdf",
			".cer",
			".chm",
			".class",
			".clp",
			".cmx",
			".cnf",
			".cod",
			".config",
			".contact",
			".coverage",
			".cpio",
			".cpp",
			".crd",
			".crl",
			".crt",
			".cs",
			".csdproj",
			".csh",
			".csproj",
			".css",
			".csv",
			".cur",
			".cxx",
			".dat",
			".datasource",
			".dbproj",
			".dcm",
			".dcr",
			".def",
			".deploy",
			".der",
			".dgml",
			".dib",
			".dif",
			".dir",
			".disco",
			".dll",
			".dll.config",
			".dlm",
			".doc",
			".docm",
			".docx",
			".dot",
			".dotm",
			".dotx",
			".dsp",
			".dsw",
			".dtd",
			".dtsconfig",
			".dv",
			".dvi",
			".dwf",
			".dwp",
			".dxr",
			".eml",
			".emz",
			".eot",
			".eps",
			".etl",
			".etx",
			".evy",
			".exe",
			".exe.config",
			".fdf",
			".fif",
			".filters",
			".fla",
			".flr",
			".flv",
			".fsscript",
			".fsx",
			".generictest",
			".gif",
			".group",
			".gsm",
			".gtar",
			".gz",
			".h",
			".hdf",
			".hdml",
			".hhc",
			".hhk",
			".hhp",
			".hlp",
			".hpp",
			".hqx",
			".hta",
			".htc",
			".htm",
			".html",
			".htt",
			".hxa",
			".hxc",
			".hxd",
			".hxe",
			".hxf",
			".hxh",
			".hxi",
			".hxk",
			".hxq",
			".hxr",
			".hxs",
			".hxt",
			".hxv",
			".hxw",
			".hxx",
			".i",
			".ico",
			".ics",
			".idl",
			".ief",
			".iii",
			".inc",
			".inf",
			".inl",
			".ins",
			".ipa",
			".ipg",
			".ipproj",
			".ipsw",
			".iqy",
			".isp",
			".ite",
			".itlp",
			".itms",
			".itpc",
			".ivf",
			".jar",
			".java",
			".jck",
			".jcz",
			".jfif",
			".jnlp",
			".jpb",
			".jpe",
			".jpeg",
			".jpg",
			".js",
			".jsx",
			".jsxbin",
			".latex",
			".library-ms",
			".lit",
			".loadtest",
			".lpk",
			".lsf",
			".lst",
			".lsx",
			".lzh",
			".m13",
			".m14",
			".m1v",
			".m2t",
			".m2ts",
			".m2v",
			".m3u",
			".m3u8",
			".m4a",
			".m4b",
			".m4p",
			".m4r",
			".m4v",
			".mac",
			".mak",
			".man",
			".manifest",
			".map",
			".master",
			".mda",
			".mdb",
			".mde",
			".mdp",
			".me",
			".mfp",
			".mht",
			".mhtml",
			".mid",
			".midi",
			".mix",
			".mk",
			".mmf",
			".mno",
			".mny",
			".mod",
			".mov",
			".movie",
			".mp2",
			".mp2v",
			".mp3",
			".mp4",
			".mp4v",
			".mpa",
			".mpe",
			".mpeg",
			".mpf",
			".mpg",
			".mpp",
			".mpv2",
			".mqv",
			".ms",
			".msi",
			".mso",
			".mts",
			".mtx",
			".mvb",
			".mvc",
			".mxp",
			".nc",
			".nsc",
			".nws",
			".ocx",
			".oda",
			".odc",
			".odh",
			".odl",
			".odp",
			".ods",
			".odt",
			".one",
			".onea",
			".onepkg",
			".onetmp",
			".onetoc",
			".onetoc2",
			".orderedtest",
			".osdx",
			".p10",
			".p12",
			".p7b",
			".p7c",
			".p7m",
			".p7r",
			".p7s",
			".pbm",
			".pcast",
			".pct",
			".pcx",
			".pcz",
			".pdf",
			".pfb",
			".pfm",
			".pfx",
			".pgm",
			".pic",
			".pict",
			".pkgdef",
			".pkgundef",
			".pko",
			".pls",
			".pma",
			".pmc",
			".pml",
			".pmr",
			".pmw",
			".png",
			".pnm",
			".pnt",
			".pntg",
			".pnz",
			".pot",
			".potm",
			".potx",
			".ppa",
			".ppam",
			".ppm",
			".pps",
			".ppsm",
			".ppsx",
			".ppt",
			".pptm",
			".pptx",
			".prf",
			".prm",
			".prx",
			".ps",
			".psc1",
			".psd",
			".psess",
			".psm",
			".psp",
			".pub",
			".pwz",
			".qht",
			".qhtm",
			".qt",
			".qti",
			".qtif",
			".qtl",
			".qxd",
			".ra",
			".ram",
			".rar",
			".ras",
			".rat",
			".rc",
			".rc2",
			".rct",
			".rdlc",
			".resx",
			".rf",
			".rgb",
			".rgs",
			".rm",
			".rmi",
			".rmp",
			".roff",
			".rpm",
			".rqy",
			".rtf",
			".rtx",
			".ruleset",
			".s",
			".safariextz",
			".scd",
			".sct",
			".sd2",
			".sdp",
			".sea",
			".searchconnector-ms",
			".setpay",
			".setreg",
			".settings",
			".sgimb",
			".sgml",
			".sh",
			".shar",
			".shtml",
			".sit",
			".sitemap",
			".skin",
			".sldm",
			".sldx",
			".slk",
			".sln",
			".slupkg-ms",
			".smd",
			".smi",
			".smx",
			".smz",
			".snd",
			".snippet",
			".snp",
			".sol",
			".sor",
			".spc",
			".spl",
			".src",
			".srf",
			".ssisdeploymentmanifest",
			".ssm",
			".sst",
			".stl",
			".sv4cpio",
			".sv4crc",
			".svc",
			".swf",
			".t",
			".tar",
			".tcl",
			".testrunconfig",
			".testsettings",
			".tex",
			".texi",
			".texinfo",
			".tgz",
			".thmx",
			".thn",
			".tif",
			".tiff",
			".tlh",
			".tli",
			".toc",
			".tr",
			".trm",
			".trx",
			".ts",
			".tsv",
			".ttf",
			".tts",
			".txt",
			".u32",
			".uls",
			".user",
			".ustar",
			".vb",
			".vbdproj",
			".vbk",
			".vbproj",
			".vbs",
			".vcf",
			".vcproj",
			".vcs",
			".vcxproj",
			".vddproj",
			".vdp",
			".vdproj",
			".vdx",
			".vml",
			".vscontent",
			".vsct",
			".vsd",
			".vsi",
			".vsix",
			".vsixlangpack",
			".vsixmanifest",
			".vsmdi",
			".vspscc",
			".vss",
			".vsscc",
			".vssettings",
			".vssscc",
			".vst",
			".vstemplate",
			".vsto",
			".vsw",
			".vsx",
			".vtx",
			".wav",
			".wave",
			".wax",
			".wbk",
			".wbmp",
			".wcm",
			".wdb",
			".wdp",
			".webarchive",
			".webtest",
			".wiq",
			".wiz",
			".wks",
			".wlmp",
			".wlpginstall",
			".wlpginstall3",
			".wm",
			".wma",
			".wmd",
			".wmf",
			".wml",
			".wmlc",
			".wmls",
			".wmlsc",
			".wmp",
			".wmv",
			".wmx",
			".wmz",
			".wpl",
			".wps",
			".wri",
			".wrl",
			".wrz",
			".wsc",
			".wsdl",
			".wvx",
			".x",
			".xaf",
			".xaml",
			".xap",
			".xbap",
			".xbm",
			".xdr",
			".xht",
			".xhtml",
			".xla",
			".xlam",
			".xlc",
			".xld",
			".xlk",
			".xll",
			".xlm",
			".xls",
			".xlsb",
			".xlsm",
			".xlsx",
			".xlt",
			".xltm",
			".xltx",
			".xlw",
			".xml",
			".xmta",
			".xof",
			".xoml",
			".xpm",
			".xps",
			".xrm-ms",
			".xsc",
			".xsd",
			".xsf",
			".xsl",
			".xslt",
			".xsn",
			".xss",
			".xtp",
			".xwd",
			".z",
			".zip",
			#endregion
		};
	}
}
