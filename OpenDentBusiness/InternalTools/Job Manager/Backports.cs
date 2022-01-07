using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class Backports {

		///<summary>Gets a list of modified files with all changes recorded from SVN.</summary>
		public static List<ODFileChanges> GetListOfFiles(string pathOnRefresh,List<string> listVersions,string ignoreListName,BackportProject currentProject) {
			List<ODFileChanges> listFilesChanged=new List<ODFileChanges>();
			string headString="";
			if(currentProject.PatternHead==HeadPattern.head) {
				headString="head";
			}
			else if(currentProject.PatternHead==HeadPattern.Name_head) {
				headString=Enum.GetName(currentProject.Name.GetType(),currentProject.Name)+"_head";
			}
			string rawOutputFilesModified=RunWindowsCommand("svn diff --summarize \""+pathOnRefresh+"\\"+headString+"\"");
			string rawOutputFilesIgnored=RunWindowsCommand("svn status --cl \""+ignoreListName+"\" \""+pathOnRefresh+"\\"+headString+"\"");//Change list
			List<string> listModifiedFiles=ExtractFileNames(rawOutputFilesModified);
			List<string> listIgnoreFiles=ExtractFileNames(rawOutputFilesIgnored);//gets the list of files to not include in the output.
			listModifiedFiles=listModifiedFiles.Except(listIgnoreFiles).ToList();
			foreach(string modFile in listModifiedFiles) {
				ODFileChanges file=new ODFileChanges();
				switch(modFile[0]) {
					case 'M':
						file.ModificationType=FileModificationType.Modified;
						break;
					case 'D':
						file.ModificationType=FileModificationType.Deleted;
						break;
					case 'A':
						file.ModificationType=FileModificationType.Added;
						break;
					default:
						file.ModificationType=FileModificationType.Unknown;
						break;
				}
				file.FilePathHead=modFile.Substring(1).Trim();//Remove the modification type from the beginning to get the file path.
				file.GetFilePaths(listVersions,currentProject,headString);
				listFilesChanged.Add(file);
			}
			//Fill listFilesModified with information
			List<string> listLineChanges=new List<string>();
			foreach(ODFileChanges file in listFilesChanged) {
				listLineChanges.Add(RunWindowsCommand("svn diff "+"\""+file.FilePathHead+"\""));
			}
			//Extract individual changes
			listLineChanges=ExtractModifiedLines(listLineChanges);
			if(listLineChanges.Count==0) {
				return new List<ODFileChanges>();
			}
			//Extracts the exact lines that were changed and stores them in a list of LineChanges
			for(int i=0;i<listLineChanges.Count;i++) {
				listFilesChanged[i].FillChanges(listLineChanges[i]);
			}
			return listFilesChanged;
		}

		///<summary>Extracts the file names from the SVN output.</summary>
		private static List<string> ExtractFileNames(string filesModified) {
			List<string> modifiedFileList=new List<string>();
			//Get Modified Files
			using(StringReader reader=new StringReader(filesModified)) {
				string line="";
				do {
					line=reader.ReadLine();
					if(line!=null && (line.StartsWith("M  ") || line.StartsWith("D  ") || line.StartsWith("A "))) {//? is unversioned files.
						modifiedFileList.Add(line);
					}
				} while(line!=null);
			}
			return modifiedFileList;
		}

		///<summary>Extracts unformatted SVN outputs to a further formatted state. See FormatUpdatesLines for the logic.</summary>
		private static List<string> ExtractModifiedLines(List<string> unformattedOutputs) {
			List<string> lineChange=new List<string>();
			foreach(string output in unformattedOutputs) {
				lineChange.Add(FormatUpdatedLines(output));
			}
			return lineChange;
		}

		///<summary>Extracts only the lines that have been changed.</summary>
		private static string FormatUpdatedLines(string output) {
			string retVal="";
			using(StringReader reader=new StringReader(output)) {
				string line="";
				do {
					line=reader.ReadLine();
					if(line!=null) {
						if(line.StartsWith("+++ ") && line.ToLower().Contains("(working copy)")) {
							line=reader.ReadLine();
							while(line!=null) {
								retVal+=line+"\r\n";
								line=reader.ReadLine();
							}
						}
					}
				} while(line!=null);
			}
			return retVal;
		}

		///<summary>This modifies the files with the passed in changes. The core of the logic for backporting is here.</summary>
		///<param name="changeFile">The file that is being modified.</param>
		///<param name="backportVersion">The version the file is being backported to.</param>
		///<param name="backportResult">The objects where the results are stored.</param>
		public static ResultType ModifyFile(ODFileChanges changeFile,string backportVersion,ODBackportResult backportResult) {
			ResultType result=ResultType.None;
			string filePath=changeFile.DictVersionFilePath[backportVersion];
			//Get the correct encoding of the head file. Filstream.CurrentEncoding is not accurate. Because our code uses special symbols on occasion, we 
			//need to ensure when backporting files, files do not lose characters as they could be crucial. Get the head path as the encoding may have
			//changed.
			Encoding fileEncodingHead=GetEncoding(changeFile.FilePathHead);
			//The encoding of the file to be backported before any changes.
			Encoding fileEncodingOld=GetEncoding(filePath);
			//Read in file with the encoding.
			string originalFile=File.ReadAllText(filePath,fileEncodingOld).Replace("\r\n","\n").Replace("\n","\r\n");
			//Create a copy of the original lines so that we have something to safely manipulate.
			string changedFile=originalFile;
			List<ODLineChange> listFailedLineChanges=new List<ODLineChange>();
			foreach(ODLineChange lineChange in changeFile.ListLineChanges) {
				//If there's at least one match
				if(changedFile.Contains(lineChange.SVNChangesBefore)) {
					//Replace the string to see how many lines it got rid of. This is preferred over Regex.Match.Count as that has been unreliable
					//for special characters.
					string fileNoMatches=changedFile.Replace(lineChange.SVNChangesBefore,"");
					int numberOfMatches=(changedFile.Length-fileNoMatches.Length)/lineChange.SVNChangesBefore.Length;
					if(numberOfMatches==1) {
						//One match. Replace the before text with the after text.
						changedFile=changedFile.Replace(lineChange.SVNChangesBefore,lineChange.SVNChangesAfter);
					}
					else {
						//More than one match. Just fail it. They will have to manually copy over the change.
						listFailedLineChanges.Add(lineChange);
						result=ResultType.Partial;
					}
				}
				else {//no match
					listFailedLineChanges.Add(lineChange);
					result=ResultType.Partial;
				}
			}
			if(changedFile==originalFile) {
				result=ResultType.Failed;
				return result;
			}
			//Overwrites file
			File.WriteAllText(filePath,changedFile,fileEncodingHead);
			if(result==ResultType.None) {//if nothing bad happened, set it to Ok
				result=ResultType.Ok;
			}
			//Update list of failed changes.
			backportResult.UpdateListFailedChanges(backportVersion,listFailedLineChanges);
			return result;
		}

		///<summary>This addes the given file to the given version.</summary>
		///<param name="changeFile">The file that is being added.</param>
		///<param name="backportVersion">The version the file is being backported to.</param>
		public static ResultType AddFile(ODFileChanges changeFile,string backportVersion) {
			try {
				File.Copy(changeFile.FilePathHead,changeFile.DictVersionFilePath[backportVersion]);
				RunWindowsCommand($@"svn add ""{changeFile.DictVersionFilePath[backportVersion]}""");
				return ResultType.Ok;
			}
			catch {
				return ResultType.Failed;
			}
		}

		///<summary>This removes the given file from the given version.</summary>
		///<param name="changeFile">The file that is being deleted.</param>
		///<param name="backportVersion">The version the file is being backported to.</param>
		public static ResultType DeleteFile(ODFileChanges changeFile,string backportVersion) {
			string result=RunWindowsCommand($@"svn delete ""{changeFile.DictVersionFilePath[backportVersion]}""");
			if(result.Contains("D  ")) {
				return ResultType.Ok;
			}
			else {
				return ResultType.Failed;	
			}
		}

		///<summary>Determines a text file's encoding by analyzing its byte order mark (BOM). Defaults to the operating systems current ANSI encoding 
		///when detection of the text file's endianness fails.</summary>
		///<param name="fileName">The text file to analyze.</param>
		private static Encoding GetEncoding(string fileName) {
			//Read the BOM
			var bom=new byte[4];
			using(FileStream file=new FileStream(fileName,FileMode.Open,FileAccess.Read)) {
				file.Read(bom,0,4);
			}
			if(bom[0]==0x2b && bom[1]==0x2f && bom[2]==0x76) {
				return Encoding.UTF7;
			}
			else if(bom[0]==0xef && bom[1]==0xbb && bom[2]==0xbf) {
				return Encoding.UTF8;//This is what our files usually end up being.
			}
			else if(bom[0]==0xff && bom[1]==0xfe) {
				return Encoding.Unicode; //UTF-16LE
			}
			else if(bom[0]==0xfe && bom[1]==0xff) {
				return Encoding.BigEndianUnicode; //UTF-16BE
			}
			else if(bom[0]==0 && bom[1]==0 && bom[2]==0xfe && bom[3]==0xff) {
				return Encoding.UTF32;
			}
			return Encoding.Default;
		}


		///<summary>Runs a given windows terminal command. readOutput specifies whether the output should be read and returned or if the command
		///should simply be run.</summary>
		public static string RunWindowsCommand(string command,bool readOutput=true) {
			Process cmd=new Process();
			cmd.StartInfo.FileName="cmd.exe";
			cmd.StartInfo.RedirectStandardOutput=true;
			cmd.StartInfo.RedirectStandardInput=true;
			cmd.StartInfo.CreateNoWindow=true;
			cmd.StartInfo.UseShellExecute=false;
			cmd.Start();
			cmd.StandardInput.WriteLine(command);
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit(200);
			if(readOutput) {
				return cmd.StandardOutput.ReadToEnd();
			}
			else {
				return "";
			}
		}

		///<summary>Converts a list of strings to one string.</summary>
		public static string ToStringOfLines(List<string> listOfLines) {
			StringBuilder str=new StringBuilder();
			foreach(string line in listOfLines) {
				str.AppendLine(line);
			}
			string text=str.ToString();
			if(text.Length > 2) {//if empty, this would cause a UE
				text=text.Substring(0,text.Length-2);
			}
			return text;//Removes extra new line
		}

		///<summary>Converts a string into a list of its lines.</summary>
		public static List<string> ToListOfLines(string SVN) {
			List<string> lines=new List<string>();
			using(StringReader reader=new StringReader(SVN)) {
				string line="";
				while(line!=null) {
					line=reader.ReadLine();
					if(line!=null) {
						lines.Add(line);
					}
				}
			}
			return lines;
		}

		public enum FileModificationType {
			Modified,
			Added,
			Deleted,
			Unknown,
		}

		public enum ResultType {
			Ok,
			Partial,
			Failed,
			None,
		}

		//NOTE: All projects are hardcoded with their patterns on how the folders are structured. If something changes, the project will not backport.
		//If a project is not in this list, it will not backport.
		//In order to add a new project, add the name to the ProjectName (this should match the name at the beginning of the versioned folder 
		//(e.g. opendental18.1 will be opendental). Add the project to BackportProjects and add it to the list in that class. Be sure to include the
		//type of folder and head patterns. Finally, go to FormBackport.Compile() and check the pattern to find the correct solution or project file.
		public enum ProjectName {
			Unknown,//Default if the program can't sense which project it is.
			CDT, 
			EHR, 		   
			ODCrypt,	  
			opendental,
			OpenDentalHelp, 
			OpenDentalService, 
			OpenDentalWebApps, 			   
			PhoneTrackingServer,
			ODXam,
			ODCloudAdmin,
			Bridges,
			//DatabaseMerge, //There has not been a recent release of this
			//DatabaseSpliter, //There has not been a recent release of this
			//MakeMsi, //No code to backport
			//ODPatientMerge, //There has not been a recent release of this
			//ODPatnumRenumber //There has not been a recent release of this
			//ODRenumberPrimaryKeys //There has not been a recent release of this
		}

		public enum MajorMinorPattern {
			///<summary>Pattern is like CDT_18_2.</summary>
			_Major_Minor,
			///<summary>Pattern is like CDT18.2</summary>
			MajorDotMinor,
		}

		public enum HeadPattern {
			///<summary>Pattern is like path/head</summary>
			head,
			///<summary>Pattern is like path/CDT_head</summary>
			Name_head,
		}

		public static class BackportProjects {
			public static BackportProject Unknown=new BackportProject(ProjectName.Unknown,MajorMinorPattern._Major_Minor,HeadPattern.Name_head);
			public static BackportProject CDT=new BackportProject(ProjectName.CDT,MajorMinorPattern._Major_Minor,HeadPattern.Name_head);
			public static BackportProject EHR=new BackportProject(ProjectName.EHR,MajorMinorPattern._Major_Minor,HeadPattern.Name_head);
			public static BackportProject ODCrypt=new BackportProject(ProjectName.ODCrypt,MajorMinorPattern._Major_Minor,HeadPattern.Name_head);
			public static BackportProject OpenDental=new BackportProject(ProjectName.opendental,MajorMinorPattern.MajorDotMinor,HeadPattern.head);
			public static BackportProject OpenDentalHelp=new BackportProject(ProjectName.OpenDentalHelp,MajorMinorPattern._Major_Minor,HeadPattern.head);
			public static BackportProject OpenDentalService=new BackportProject(ProjectName.OpenDentalService,MajorMinorPattern._Major_Minor,
				HeadPattern.head);
			public static BackportProject OpenDentalWebApps=new BackportProject(ProjectName.OpenDentalWebApps,MajorMinorPattern._Major_Minor,
				HeadPattern.head);
			public static BackportProject PhoneTrackingServer=new BackportProject(ProjectName.PhoneTrackingServer,MajorMinorPattern._Major_Minor,
				HeadPattern.Name_head);
			public static BackportProject ODXam=new BackportProject(ProjectName.ODXam,MajorMinorPattern._Major_Minor,HeadPattern.head);
			public static BackportProject ODCloudAdmin=new BackportProject(ProjectName.ODCloudAdmin,MajorMinorPattern._Major_Minor,
				HeadPattern.head);
			public static BackportProject Bridges=new BackportProject(ProjectName.Bridges,MajorMinorPattern._Major_Minor,HeadPattern.Name_head);
			public static List<BackportProject> ListProjects=new List<BackportProject> { Bridges,CDT,EHR,ODCrypt,OpenDental,OpenDentalHelp,
				OpenDentalService,OpenDentalWebApps,PhoneTrackingServer,ODXam,ODCloudAdmin,Unknown };
		}

		public class BackportProject {
			public ProjectName Name;
			public MajorMinorPattern PatternMajor;
			public HeadPattern PatternHead;

			public BackportProject(ProjectName name,MajorMinorPattern patternMajor,HeadPattern patternHead) {
				Name=name;
				PatternMajor=patternMajor;
				PatternHead=patternHead;
			}

			public BackportProject Copy() {
				return (BackportProject)MemberwiseClone();
			}
		}
		
		///<summary>Interal class used for backporting tool. Stores information about a changed file.</summary>
		public class ODFileChanges {
			///<summary>Full head file path for a given file.</summary>
			public string FilePathHead;
			///<summary>Dictionary that contains the path to the file for each backport version.</summary>
			public Dictionary<string,string> DictVersionFilePath=new Dictionary<string,string>();
			///<summary>File name only for a file. e.g. ContrAppt.cs</summary>
			public string FileName;
			///<summary>The type of modification. e.g. Modified, Added, etc</summary>
			public FileModificationType ModificationType;
			///<summary>A List of line changes according to the SVN output</summary>
			public List<ODLineChange> ListLineChanges=new List<ODLineChange>();

			///<summary>After the FilePathHead is set, this takes the current beta and stable string and filles the rest of the paths in.</summary>
			public void GetFilePaths(List<string> listVersions,BackportProject currentProject,string headString) {
				if(currentProject.PatternMajor==MajorMinorPattern.MajorDotMinor) {
					foreach(string version in listVersions) {
						Version v=new Version(version);
						DictVersionFilePath.Add(version,
							FilePathHead.Replace(headString,Enum.GetName(currentProject.Name.GetType(),currentProject.Name)+v.Major+"."+v.Minor));
					}
				}
				else {//_Major_Minor
					foreach(string version in listVersions) {
						Version v=new Version(version);
						string versionPath=FilePathHead.Replace(headString,Enum.GetName(currentProject.Name.GetType(),
							currentProject.Name)+"_"+v.Major+"_"+v.Minor);
						if(currentProject.Name==ProjectName.ODXam) {
							//ODXam is special as we change more than just the one path name. We also change the folder+csproj name.
							//These replaces will fix both of those.
							versionPath=versionPath.Replace("ODXam.Shared",$"ODXam_{v.Major}_{v.Minor}.Shared");
							versionPath=versionPath.Replace("UnitTests.ODXam",$"UnitTests.ODXam_{v.Major}_{v.Minor}");
						}
						else if(currentProject.Name==ProjectName.OpenDentalWebApps) {
							//Related to the ODXam replacement. ODXam business also has folder name changes. This allows backporting to work.
							versionPath=versionPath.Replace("OpenDentBusiness.ODXam",$"OpenDentBusiness.ODXam_{v.Major}_{v.Minor}");
						}
						DictVersionFilePath.Add(version,versionPath);
					}
				}
				FileName=Path.GetFileName(FilePathHead);
			}

			///<summary>Fills an individual LineChange for each block of code for one file that the SVN command prompt outputs.</summary>
			public void FillChanges(string changes) {
				//ï»¿ represents the byte marker. This can ruin the compare if the bytemarker is not removed.
				changes=changes.Replace("ï»¿","");
				using(StringReader reader=new StringReader(changes)) {
					string line="";
					ODLineChange change;
					do {
						line=reader.ReadLine();
						if(line!=null && Regex.IsMatch(line,"^@@ .* @@$")) {//if it is the beginning of a new LineChange
							change=new ODLineChange();
							//Typical output. Changes in a file more than one lines that was not empty previously.
							//E.g. @@ -100,5 +103,8 @@
							if(Regex.IsMatch(line,"^@@ -[0-9]*,[0-9]* \\+[0-9]*,[0-9]* @@$")) {
								//Substring magic below. This would only break if SVN decides to revamp their command line outputs.
								change.LineNumberBefore=PIn.Long(line.Substring(3,line.IndexOf(",")-3));
								change.NumberofLinesPrintedBefore=PIn.Long(line.Substring(line.IndexOf(",")+1,
									line.IndexOf(" ",line.IndexOf(","))-(line.IndexOf(",")+1)));
								change.LineNumberAfter=PIn.Long(line.Substring(line.IndexOf(" ",line.IndexOf(",")+1)
									,line.LastIndexOf(",")-line.IndexOf(" ",line.IndexOf(",")+1)));
								change.NumberofLinesPrintedAfter=PIn.Long(line.Substring(line.LastIndexOf(",")+1
									,line.LastIndexOf(" ")-line.LastIndexOf(",")));
							}
							//Occurs when a file is empty before and a certain number of lines were added.
							//E.g. @@ -0,0 +4 @@ //4 lines were added to this file.
							else if(Regex.IsMatch(line,"^@@ -[0-9]*,[0-9]* \\+[0-9]* @@$")) {
								change.LineNumberBefore=PIn.Long(line.Substring(3,line.IndexOf(",")-3));
								change.NumberofLinesPrintedBefore=PIn.Long(line.Substring(line.IndexOf(",")+1,
									line.IndexOf(" ",line.IndexOf(","))-(line.IndexOf(",")+1)));
								change.NumberofLinesPrintedAfter=PIn.Long(line.Substring(line.IndexOf(" ",line.IndexOf(",")),line.LastIndexOf(" ")-
									line.IndexOf(" ",line.IndexOf(","))));
							}
							//Occurs when a file is short before and a certain number of lines were added.
							//E.g. @@ -1 +1,4 @@ //4 lines were added to this file.
							else if(Regex.IsMatch(line,"^@@ -[0-9]* \\+[0-9]*,[0-9]* @@$")) {
								change.NumberofLinesPrintedBefore=PIn.Long(line.Substring(line.IndexOf("-"),line.IndexOf(" ",line.IndexOf("-"))
									-line.IndexOf("-")));
								change.LineNumberAfter=PIn.Long(line.Substring(line.IndexOf("+")+1
									,line.LastIndexOf(",")-(line.IndexOf("+")+1)));
								change.NumberofLinesPrintedAfter=PIn.Long(line.Substring(line.LastIndexOf(",")+1
									,line.LastIndexOf(" ")-line.LastIndexOf(",")));
							}
							//Occurs when a file has few lines and changes all of those lines.
							//E.g. @@ -1 +1 @@ //1 line changes.
							else if(Regex.IsMatch(line,"^@@ -[0-9]* \\+[0-9]* @@$")) {
								change.NumberofLinesPrintedBefore=PIn.Long(line.Substring(line.IndexOf("-"),line.IndexOf(" ",line.IndexOf("-"))
									-line.IndexOf("-")));
								change.NumberofLinesPrintedAfter=PIn.Long(line.Substring(line.IndexOf("+"),line.LastIndexOf(" ")-line.IndexOf("+")));
							}
							change.SVNLineChanges="";
							for(int i=0;i<change.NumberofLinesPrintedAfter;i++) {//read in all the actual changes. Read the exact number of lines.
								line=reader.ReadLine();
								if(line.StartsWith("\\ No newline at end of file")) {
									i--;//These can appear near the end of the file. Don't count them or add them. Continue on.
									continue;
								}
								if(line.StartsWith("-")) {
									i--;//subtract as NumberofLinesPrintedAfter does not include those removed.
								}
								change.SVNLineChanges+=line+"\r\n";
							}
							change.FillSVNChanges();
							ListLineChanges.Add(change);
						}
					} while(line!=null);
				}
			}
		}

		///<summary>Internal class used for backport tool. Stores a single line change grouping from an svn output.</summary>
		public class ODLineChange {
			///<summary>Line number in the file before the changes.</summary>
			public long LineNumberBefore;
			///<summary>Line number in the file after the changes.</summary>
			public long LineNumberAfter;
			///<summary>Number of lines in the SVN output that were in the original file.</summary>
			public long NumberofLinesPrintedBefore;
			///<summary>Number of lines in the SVN output that were in the modified file.</summary>
			public long NumberofLinesPrintedAfter;
			///<summary>Raw SVN output.</summary>
			public string SVNLineChanges;
			///<summary>Lines before the changes were made.</summary>
			public string SVNChangesBefore;
			///<summary>Lines after the changes were made.</summary>
			public string SVNChangesAfter;

			///<summary>Fills SVNChangesBefore and SVNChangesAfter after getting the raw SVNOutput. SVN outputs add a space to all lines in order
			///to offset the fact that they add + or - to some lines. These need to be removed before being compared.</summary>
			public void FillSVNChanges() {
				List<string> listSVNChanges=ToListOfLines(SVNLineChanges);
				List<string> listSVNChangesBefore=listSVNChanges.Where(x => !x.StartsWith("+")).ToList();//Grab all non-additions
				for(int i=0;i<listSVNChangesBefore.Count;i++) {
					if(listSVNChangesBefore[i].StartsWith("-")) {
						listSVNChangesBefore[i]=listSVNChangesBefore[i].Substring(1);
					}
					else if(listSVNChangesBefore[i].Count() > 0 && listSVNChangesBefore[i].StartsWith(" ")) {
						//else if as lines with + or - do not get the extra space
						listSVNChangesBefore[i]=listSVNChangesBefore[i].Substring(1);
					}
				}
				SVNChangesBefore=ToStringOfLines(listSVNChangesBefore);
				List<string> listSVNChangesAfter=listSVNChanges.Where(x => !x.StartsWith("-")).ToList();//Grab all lines not being removed.
				for(int i=0;i<listSVNChangesAfter.Count;i++) {
					if(listSVNChangesAfter[i].StartsWith("+")) {
						listSVNChangesAfter[i]=listSVNChangesAfter[i].Substring(1);
					}
					else if(listSVNChangesAfter[i].Count() > 0 && listSVNChangesAfter[i].StartsWith(" ")) {
						//else if as lines with + or - do not get the extra space
						listSVNChangesAfter[i]=listSVNChangesAfter[i].Substring(1);
					}
				}
				SVNChangesAfter=ToStringOfLines(listSVNChangesAfter);
			}
		}

		///<summary>Used in backport tool only. Stores results for a single file for all versions.</summary>
		public class ODBackportResult {
			///<summary>Full head file path for a given file. Acts as the primary key as they will always be unique.</summary>
			public string FilePathHead;
			private List<BackportResult> _listBackportResults=new List<BackportResult>();

			///<summary>Updates the list of failed changes for the given version.</summary>
			///<param name="version">The specific backport version.</param>
			///<param name="listFailedChanges">The new list of failed line changes.</param>
			public void UpdateListFailedChanges(string version,List<ODLineChange> listFailedChanges) {
				BackportResult backportResult=GetBackportResultForVersion(version);
				backportResult.ListFailedChanges=listFailedChanges;
			}

			///<summary>Updates the result for a given version.</summary>
			///<param name="version">The specific backport version.</param>
			///<param name="result">The new result.</param>
			public void UpdateResult(string version,ResultType result) {
				BackportResult backportResult=GetBackportResultForVersion(version);
				backportResult.Result=result;
			}

			///<summary>Returns the result for a given version.</summary>
			///<param name="version">The backport version.</param>
			public ResultType GetResult(string version) {
				return GetBackportResultForVersion(version).Result;
			}

			///<summary>Returns the List of failed line changes for a given version.</summary>
			///<param name="version">The backport version.</param>
			public List<ODLineChange> GetFailedChanges(string version) {
				return GetBackportResultForVersion(version).ListFailedChanges;
			}

			///<summary>Used to get the backport result for the given version. Lazy loads the backport result into the list.</summary>
			private BackportResult GetBackportResultForVersion(string version) {
				BackportResult backportResult=_listBackportResults.FirstOrDefault(x => x.BackportVersion==version);
				if(backportResult==null) {
					backportResult=new BackportResult(version,ResultType.None,new List<ODLineChange>());
					_listBackportResults.Add(backportResult);
				}
				return backportResult;
			}

			///<summary>Private class used for a single BackportVersion that stores all relevant results for the version.</summary>
			private class BackportResult {
				///<summary>The version being backportted.</summary>
				public string BackportVersion;
				///<summary>The result of the backport.</summary>
				public ResultType Result;
				///<summary>A List of line changes that failed to backport.</summary>
				public List<ODLineChange> ListFailedChanges;

				public BackportResult(string version,ResultType result,List<ODLineChange> listFailedChanges) {
					BackportVersion=version;
					Result=result;
					ListFailedChanges=listFailedChanges;
				}
			}
		}

	}

}
