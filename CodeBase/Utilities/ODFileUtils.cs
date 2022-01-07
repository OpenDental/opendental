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
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client to start the process locally.</summary>
		public static void ProcessStart(Process process) {
			if(ODBuild.IsWeb()) {
				//We will only use the FileName and Arguments from the process's StartInfo.  Only non-web builds utilize the entire process.
				ProcessStart(process.StartInfo.FileName,process.StartInfo.Arguments);
			}
			else {
				process.Start();
			}
		}
		
		///<summary>Start a new process with the given path and arguments.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client to start the process locally.</summary>
		///<param name="doWaitForODCloudClientResponse">If true, will wait for ODCloudClient and throw any exceptions from it.</param>
		///<param name="createDirIfNeeded">If included, will create the directory if it doesn't exist.</param>
		public static Process ProcessStart(string path,string commandLineArgs="",bool doWaitForODCloudClientResponse=false,string createDirIfNeeded="") {
			if(ODBuild.IsWeb()) {
				ODCloudClient.LaunchFileWithODCloudClient(path,commandLineArgs,doWaitForResponse:doWaitForODCloudClientResponse,
					createDirIfNeeded:createDirIfNeeded);
				return null;
			}
			if(!string.IsNullOrEmpty(createDirIfNeeded) && !Directory.Exists(createDirIfNeeded)) {
				Directory.CreateDirectory(createDirIfNeeded);
			}
			return Process.Start(path,commandLineArgs);
		}
		
		///<summary>Write the given text to the given file.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO.</summary>
		public static void WriteAllText(string filePath,string text) {
			if(ODBuild.IsWeb()) {
				ODCloudClient.LaunchFileWithODCloudClient(extraFilePath:filePath,extraFileData:text);
			}
			else {
				File.WriteAllText(filePath,text);
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath) {
			return WriteAllTextThenStart(filePath,fileText,processPath,"");
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,string processPath,string commandLineArgs) {
			if(ODBuild.IsWeb()) {
				ODCloudClient.LaunchFileWithODCloudClient(processPath,commandLineArgs,filePath,fileText);
				return null;
			}
			else {
				File.WriteAllText(filePath,fileText);
				return Process.Start(processPath,commandLineArgs);
			}
		}
		
		///<summary>Write the given text to the given file, then start a new process with the given path.  
		///If using a WEB compiled version of Open Dental, pass through to the odcloud client for File IO and to start the process locally.</summary>
		public static Process WriteAllTextThenStart(string filePath,string fileText,Encoding encoding,string processPath,string commandLineArgs) {
			if(ODBuild.IsWeb()) {
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
		public static Process WriteAllBytesThenStart(string filePath,byte[] fileBytes,string processPath,string commandLineArgs) {
			if(ODBuild.IsWeb()) {
				string byteString=Convert.ToBase64String(fileBytes);
				ODCloudClient.LaunchFileWithODCloudClient(processPath,commandLineArgs,filePath,byteString,"binary");
				return null;
			}
			else {
				File.WriteAllBytes(filePath,fileBytes);
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

	}
}
