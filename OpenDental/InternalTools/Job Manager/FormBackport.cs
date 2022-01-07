using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using static OpenDentBusiness.Backports;

namespace OpenDental {
	public partial class FormBackport:FormODBase {
		///<summary>The path in the textPath when the data is refreshed.</summary>
		private string _pathOnRefresh;
		///<summary>The path in the textIgnoreList when the data is refreshed.</summary>
		private string _ignoreListNameOnRefresh;
		///<summary>This holds the current file changes.</summary>
		private List<ODFileChanges> _listFileChanges;
		///<summary>This holds the previously selected rows in the grid. This allows the user to select files and continue to double click to view 
		///svn differences without losing the selection.</summary>
		private int[] _arrayPreviousSelected;
		///<summary>This stores all the previously backported files and records the results of the backporting process.</summary>
		private List<ODBackportResult> _listBackportResults=new List<ODBackportResult>();
		///<summary>The current project on refresh. Provides support for SharedProjects.</summary>
		private BackportProject _currentProject=BackportProjects.Unknown;
		///<summary>The action to end the progress bar. Kept to have only one instance on this form.</summary>
		private Action _progressBarAction=null;

		///<summary>Returns a list of all versions that are currently available to be backported to.</summary>
		private List<string> _listAvailableVersions {
			get {
				return listBoxVersions.Items.GetAll<string>();
			}
		}

		///<summary>Returns a list of versions that are currently selected to be backported to.</summary>
		private List<string> _listSelectedVersions {
			get {
				return listBoxVersions.GetListSelected<string>();
			}
		}

		private List<string> _listComboOptions {
			get {
				return new List<string> { 
					$@"C:\development\OPEN DENTAL SUBVERSION",
					$@"C:\development\Shared Projects Subversion\OpenDentalWebApps",
					$@"C:\development\Shared Projects Subversion\ODXam",
					$@"C:\development\Shared Projects Subversion\CDT",
					$@"C:\development\Shared Projects Subversion\EHR",
					$@"C:\development\Shared Projects Subversion\ODCrypt",
					$@"C:\development\Shared Projects Subversion\OpenDentalHelp",
					$@"C:\development\Shared Projects Subversion\OpenDentalService",
					$@"C:\development\Shared Projects Subversion\PhoneTrackingServer",
					$@"C:\development\Shared Projects Subversion\ODCloudAdmin",
					$@"C:\development\Shared Projects Subversion\Bridges" };
			}
		}

		public FormBackport(long jobNum=0) {
			InitializeComponent();
			InitializeLayoutManager();
			//Set the job num if they passed one in.
			if(jobNum!=0) {
				validJobNum.Text=jobNum.ToString();
			}
			FillGrid();
			comboPath.Items.AddRange(_listComboOptions.ToArray());
		}

		///<summary>Fills the grid.</summary>
		private void FillGrid() {
			//Remove previously selected 
			_arrayPreviousSelected=null;
			//FillGrid
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Type",75);
			gridMain.ListGridColumns.Add(col);
			foreach(string version in _listAvailableVersions) {
				col=new GridColumn(version,75);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn("File Name",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("File Path",50);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(_listFileChanges!=null) {
				foreach(ODFileChanges changedFile in _listFileChanges) {
					row=new GridRow();
					row.Cells.Add(Enum.GetName(changedFile.ModificationType.GetType(),changedFile.ModificationType));
					foreach(string version in _listAvailableVersions) {
						row.Cells.Add(GetStatusCell(version,changedFile));
					}
					row.Cells.Add(changedFile.FileName);
					//Only paste path after head/
					row.Cells.Add(changedFile.FilePathHead.Substring(changedFile.FilePathHead.IndexOf("head")+4
						,changedFile.FilePathHead.Length-changedFile.FilePathHead.IndexOf("head")-4));
					row.Tag=changedFile;//Tag used to open changes in TortoiseSVN
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		///<summary>Refreshes the data and changes the UI accordingly. Called from FillGrid when dataRefresh is true.</summary>
		private void RefreshData() {
			List<ODThread> listThreadsRunning=ODThread.GetThreadsByGroupName("FormBackport_Refresh");
			//Quit all threads that are still running. The user may have changed the path. This way the grid will not be filled with false information.
			listThreadsRunning.ForEach(x => x.QuitAsync());
			//Store path
			_pathOnRefresh=comboPath.Text.TrimEnd('\\');
			_ignoreListNameOnRefresh=textIgnoreList.Text;
			_currentProject=BackportProjects.Unknown;
			_listFileChanges=new List<ODFileChanges>();
			//Clear Rows
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			gridMain.EndUpdate();
			if(!Directory.Exists(comboPath.Text)) {
				MessageBox.Show("The directory does not exist.");
				return;
			}
			else {
				if(_pathOnRefresh.Contains("OPEN DENTAL SUBVERSION")) {//look for open dental first as its naming scheme does not match the rest.
					_currentProject=BackportProjects.OpenDental;
				}
				else {
					for(int i=0;i<Enum.GetNames(typeof(ProjectName)).Length;i++) {
						if(_pathOnRefresh.Contains(Enum.GetNames(typeof(ProjectName))[i])){
							_currentProject=BackportProjects.ListProjects.Find(x => x.Name==((ProjectName)i));
							break;
						}
					}
				}
			}
			if(_currentProject==BackportProjects.Unknown) {
				MessageBox.Show("Could not find the correct project.");
				return;
			}
			//Get available versions based on folder structure.
			UpdateListVersions();
			Cursor=Cursors.AppStarting;
			//Refresh Data
			string pathOnRefresh=_pathOnRefresh;
			string ignoreListNameOnRefresh=_ignoreListNameOnRefresh;
			BackportProject currentProject=_currentProject.Copy();
			ODThread odThread=new ODThread((o) => {
				List<ODFileChanges> listFileChanges=GetListOfFiles(pathOnRefresh,_listAvailableVersions,ignoreListNameOnRefresh,currentProject);
				this.InvokeIfNotDisposed(() => {//If window quit, this action will not run and the thread will die.
					if(o.HasQuit) {//If the user refreshed the path and this was marked to quit.
						return;
					}
					Cursor=Cursors.Default;
					_listFileChanges=listFileChanges;
					labelCurProj.Text="Current Project: "+Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name);
					FillGrid();
					_progressBarAction?.Invoke();
					_progressBarAction=null;
				});
			});
			odThread.AddExceptionHandler(ex => {
				_progressBarAction?.Invoke();
				_progressBarAction=null;
				this.InvokeIfNotDisposed(() => {//If there's an exception after the form is closed, swallow and do not do anything.
					FriendlyException.Show("Error refreshing data.",ex);
				});
			});
			odThread.GroupName="FormBackport_Refresh";
			odThread.Name="FormBackport_Refresh"+DateTime.Now.Millisecond;
			odThread.Start();
			if(_progressBarAction==null) {
				_progressBarAction=ODProgress.Show();
			}
		}

		///<summary>Updates the list of available versions to backport to. Call this after the backport project has been set. Will
		///update the listbox UI.</summary>
		private void UpdateListVersions() {
			listBoxVersions.Items.Clear();
			//Get all the dictionaries that are at least 4 characters long. This is so we can attempt to pull 
			//the version off the end.
			List<string> listDirectoryPaths=Directory.GetDirectories(_pathOnRefresh).Where(x => x.Length>=4).ToList();
			//For every folder structure, the version is in the last 4 characters. e.g. 18_1, 19.2
			List<string> listVersions=listDirectoryPaths.Select(x => x.Substring(x.Length-4,4).Replace('_','.')).ToList();
			listVersions=listVersions.Where(x => Version.TryParse(x,out Version _)).OrderByDescending(x => x).ToList();
			foreach(string version in listVersions) {
				listBoxVersions.Items.Add(version,version);
			}
			//Fill to reflect the versions
			FillGrid();
		}

		///<summary>Returns an ODGridCell formatted to show the status of the backport.</summary>
		private GridCell GetStatusCell(string backportVersion,ODFileChanges changedFile) {
			GridCell cell=new GridCell();
			ODBackportResult backportResult=_listBackportResults.Find(x => x.FilePathHead==changedFile.FilePathHead);
			if(backportResult!=null) {
				ResultType result=backportResult.GetResult(backportVersion);
				int numFailed=backportResult.GetFailedChanges(backportVersion).Count;
				cell.Text=result.ToString();
				switch(result) {
					case ResultType.Ok:
						cell.ColorText=Color.DarkGreen;
						break;
					case ResultType.Partial:
						cell.ColorText=Color.YellowGreen;
						cell.Text+=" ("+(changedFile.ListLineChanges.Count-numFailed).ToString()+"/"+changedFile.ListLineChanges.Count+")";
						break;
					case ResultType.Failed:
						cell.ColorText=Color.DarkRed;
						break;
					case ResultType.None:
					default:
						cell.ColorText=Color.Black;
						cell.Text="";
						break;
				}
			}
			return cell;
		}

		private void butBackport_Click(object sender,EventArgs e) {
			DoBackport();
		}

		///<summary>Occurs when the click event fires.</summary>
		private bool DoBackport() {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show("Please select at least one file to backport first.");
				return false;
			}
			if(_listSelectedVersions.Count==0) {
				MessageBox.Show("Please select at least one version to backport to.");
				return false;
			}
			//Get only selected files and store in local copy
			List<ODFileChanges> listFileChanges=_listFileChanges.Where(x => gridMain.SelectedTags<ODFileChanges>()
				.Any(y => x.FilePathHead==y.FilePathHead)).ToList();
			//Check only backporting Modified files
			if(listFileChanges.Any(x => !ListTools.In(x.ModificationType,FileModificationType.Modified,FileModificationType.Added,FileModificationType.Deleted))) {
				MessageBox.Show("Only \"Modified\", \"Added\", or\"Deleted\" files can be backported.");
				return false;
			}
			//Make sure it is a known project
			if(_currentProject.Name==ProjectName.Unknown) {
				string currentProjects="";
				for(int i=0;i<Enum.GetNames(typeof(ProjectName)).Length;i++) {
					if((ProjectName)i==ProjectName.Unknown){
						continue;
					}
					if(i > 1) {
						currentProjects+=", ";
					}
					if(i==Enum.GetNames(typeof(ProjectName)).Length-1) {
						currentProjects+="and ";
					}
					currentProjects+=Enum.GetNames(typeof(ProjectName))[i];
				}
				MessageBox.Show("Unknown project. The currently supported projects are "+currentProjects+".\r\n\r\nNote: The opendental folder "
					+"should be called \"OPEN DENTAL SUBVERSION\"");
				return false;
			}
			//Checks that the files are editable before beginning
			if(AreValidFiles(listFileChanges)) {
				//Begin the backportting process
				return BeginBackport(listFileChanges);
			}
			return false;
		}

		///<summary>Checks to make sure the files are able to be open. Returns true if they are and false if they are unavailable.</summary>
		private bool AreValidFiles(List<ODFileChanges> listFileChanges) {
			List<string> listErrors=new List<string>();
			string message="";
			foreach(ODFileChanges change in listFileChanges) {
				//Added files will not exist in the previous versions as we are backporting the add.
				if(change.ModificationType==FileModificationType.Added) {
					continue;
				}
				foreach(string versionToCheck in _listSelectedVersions) {
					try {
						using(FileStream stream=File.Open(change.DictVersionFilePath[versionToCheck],FileMode.Open)) {
							stream.Close();//The file could open, simply close it.
						}
					}
					catch {
						string error=change.FileName+" in "+versionToCheck;
						listErrors.Add(error);
					}
				}
			}
			if(listErrors.Count==0) {
				return true;
			}
			else if(listErrors.Count==1) {
				message="Could not open "+listErrors[0]+". It may be in use or may not exist.";
			}
			else if(listErrors.Count==2){
				message="Could not open "+listErrors[0]+" or "+listErrors[1]+". They may be in use or may not exist.";
			}
			else {//format correctly
				message="Could not open ";
				for(int i=0;i<listErrors.Count;i++) {
					if(i > 0) {
						message+=", ";
					}
					if(i==listErrors.Count-1) {
						message+="and ";
					}
					message+=listErrors[i];
				}
				message+=". They may be in use or may not exist.";
			}
			MessageBox.Show(message);
			return false;
		}

		///<summary>Begins the backporting process. All information in the FileChanges should be filled at this point.</summary>
		public bool BeginBackport(List<ODFileChanges> listFileChanges) {
			bool areAllSuccess=true;
			foreach(string version in _listSelectedVersions) {
				areAllSuccess&=BackportFiles(version,listFileChanges);
			}
			if(!_listSelectedVersions.IsNullOrEmpty()) {
				FillGrid();
			}
			return areAllSuccess;
		}

		///<summary>This backports the files that have not been backported yet.</summary>
		///<param name="backportVersion">The version that is being backported.</param>
		///<param name="listFileChanges">A list of the files that will be backported.</param>
		public bool BackportFiles(string backportVersion,List<ODFileChanges> listFileChanges) {
			bool areAllFilesSuccessful=true;
			foreach(ODFileChanges file in listFileChanges) {
				ODBackportResult backportResult=new ODBackportResult();
				backportResult.FilePathHead=file.FilePathHead;
				if(_listBackportResults.Any(x => x.FilePathHead==file.FilePathHead)) {
					backportResult=_listBackportResults.Find(x => x.FilePathHead==file.FilePathHead);
				}
				ResultType resultType=backportResult.GetResult(backportVersion);
				if(resultType!=ResultType.None) {//Skip already backported files
					areAllFilesSuccessful&=resultType==ResultType.Ok;
					continue;
				}
				if(file.ModificationType==FileModificationType.Modified) {
					resultType=ModifyFile(file,backportVersion,backportResult);
				}
				else if(file.ModificationType==FileModificationType.Added) {
					resultType=AddFile(file,backportVersion);
				}
				else if(file.ModificationType==FileModificationType.Deleted) {
					resultType=DeleteFile(file,backportVersion);
				}
				areAllFilesSuccessful&=resultType==ResultType.Ok;
				backportResult.UpdateResult(backportVersion,resultType);
				if(!_listBackportResults.Any(x => x.FilePathHead==file.FilePathHead)) {//If not added yet, add here.
					_listBackportResults.Add(backportResult);
				}
			}
			return areAllFilesSuccessful;
		}

		///<summary>An exception to the typical compilation pattern. ODXam has one single solution that
		///contains all of the versionsd folders.</summary>
		private void CompileODXam() {
			if(!TryGetVsPath(out string vsPath)) {
				return;
			}
			string path=$"{_pathOnRefresh}\\ODXam_Prod.sln";
			if(!File.Exists(path)) {
				MessageBox.Show("File does not exist.\r\n"+path);
				return;
			}
			Cursor=Cursors.WaitCursor;
			RunWindowsCommand(GenerateCompileCommand(vsPath,path),false);
			Cursor=Cursors.Default;
		}

		///<summary>Compiles the given project by opening it in Visual Studio and automatically beginning the build.</summary>
		private void Compile(string backportVersion) {
			if(_currentProject.Name==ProjectName.Unknown) {
				return;
			}
			Version versionCur=new Version(backportVersion);
			if(!TryGetVsPath(out string vsPath)) {
				return;
			}
			string path=_pathOnRefresh+"\\";
			//add the versioned folder
			if(_currentProject.PatternMajor==MajorMinorPattern.MajorDotMinor) {
				path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+versionCur.Major+"."+versionCur.Minor+"\\";
			}
			else if(_currentProject.PatternMajor==MajorMinorPattern._Major_Minor) {
				path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+"_"+versionCur.Major+"_"+versionCur.Minor+"\\";
			}
			//add the path to the .sln (or .csproj in some cases). These are hardcoded as the patterns are all over the place.
			switch(_currentProject.Name) {
				case ProjectName.Bridges:
					path+="xBridges.sln";
					break;
				case ProjectName.CDT:
					path+="xCDT.sln";
					break;
				case ProjectName.EHR:
					path+="EHR\\xEHR.csproj";
					break;
				case ProjectName.opendental:
					path+="OpenDental"+"_"+versionCur.Major+"_"+versionCur.Minor+".sln";
					break;
				case ProjectName.OpenDentalHelp:
					path+="OpenDentalHelp\\OpenDentalHelp.csproj";
					break;
				case ProjectName.ODCrypt:
				case ProjectName.OpenDentalService:
				case ProjectName.OpenDentalWebApps:
				case ProjectName.PhoneTrackingServer:
				default:
					path+=Enum.GetName(_currentProject.Name.GetType(),_currentProject.Name)+"_"+versionCur.Major+"_"+versionCur.Minor+".sln";
					break;
			}
			if(!File.Exists(path)) {
				MessageBox.Show("File does not exist.\r\n"+path);
				return;
			}
			Cursor=Cursors.WaitCursor;
			RunWindowsCommand(GenerateCompileCommand(vsPath,path),false);
			Cursor=Cursors.Default;
		}

		///<summary>Attempts to get the path for visual studio. Returns true if able to locate.</summary>
		private bool TryGetVsPath(out string vsPath) {
			//Default path to VS 2019.
			vsPath=@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe";
			if(!File.Exists(vsPath)) {
				//VS 2019 does not exist. Check VS 2015.
				vsPath=@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe";
				if(!File.Exists(vsPath)) {
					MessageBox.Show("Cannot find Visual Studio 2019 or 2015.");
					vsPath="";
					return false;
				}
			}
			return true;
		}

		///<summary>Creates the command that can be used in the windows command line to run the specific solution
		///passed in. Validate that solutionPath exists before calling this method.</summary>
		private string GenerateCompileCommand(string vsPath,string solutionPath) {
			return $"\"{vsPath}\"/Run \"{solutionPath}\"";
		}

		///<summary>Brings up the tortoise svn window for the currently selected path.</summary>
		private void Commit() {
			if(_currentProject==BackportProjects.Unknown || !Directory.Exists(_pathOnRefresh)) {
				return;
			}
			string commitMessage="";
			if(PIn.Long(validJobNum.Text,false)!=0) {
				Job job=Jobs.GetOneFilled(PIn.Long(validJobNum.Text));
				if(job!=null) {
					commitMessage=GetCommitMessage(job);
				}
			}
			Cursor=Cursors.WaitCursor;
			Process process=new Process();
			string arguments="/command:commit /path:\""+_pathOnRefresh+"\" /logmsg:\""+commitMessage+"\"";
			ProcessStartInfo startInfo=new ProcessStartInfo("TortoiseProc.exe",arguments);
			process.StartInfo=startInfo;
			process.Start();
			Cursor=Cursors.Default;
		}

		///<summary>Returns the commit message for this job. Job should not be null.</summary>
		private string GetCommitMessage(Job job) {
			string description;
			if(job.Category==JobCategory.Bug) {
				Bug bug=Bugs.GetOne(job.ListJobLinks.Where(x => x.LinkType==JobLinkType.Bug).FirstOrDefault()?.FKey??0);
				string bugDescription="";
				if(bug!=null) {
					bugDescription=bug.Description.Replace("\"","");
				}
				else {
					bugDescription=job.Title;
				}
				description=job.Category.ToString().Substring(0,1)+job.JobNum+" - "
					+bugDescription;
			}
			else {
				description=job.Category.ToString().Substring(0,1)+job.JobNum+" - "+job.Title;
			}
			string reviewers=string.Join(", ",job.ListJobReviews
				.Where(x => x.ReviewStatus==JobReviewStatus.Done || x.ReviewStatus==JobReviewStatus.NeedsAdditionalReview)
				.DistinctBy(x => x.ReviewerNum)
				.Select(x => Userods.GetName(x.ReviewerNum))
				.OrderBy(x => x)
				.ToList());
			return $"{POut.String(description)}\r\nBackported to: {POut.String(job.JobVersion)}\r\nReviewed by: {POut.String(reviewers)}";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshData();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_arrayPreviousSelected!=null) {
				gridMain.SetAll(false);
				gridMain.SetSelected(_arrayPreviousSelected,true);
			}
			string path=((ODFileChanges)gridMain.ListGridRows[e.Row].Tag).FilePathHead;
			string command="TortoiseProc.exe /command:diff /path:\""+path+"\"";
			RunWindowsCommand(command,false);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			_arrayPreviousSelected=gridMain.SelectedIndices;
		}

		private void butBackportAndCommit_Click(object sender,EventArgs e) {
			if(!DoBackport()) {
				return;
			}
			Commit();
		}

		private void butCompileAll_Click(object sender,EventArgs e) {
			//The ODXam project is special. There is one solution that contains all the versions. Regardless of 
			//selected versions, we will open and build the solution.
			if(_currentProject.Name==ProjectName.ODXam) {
				CompileODXam();
				return;
			}
			foreach(string version in _listSelectedVersions) {
				Compile(version);
			}
		}

		private void butFindPath_Click(object sender,EventArgs e) {
			FolderBrowserDialog dialog=new FolderBrowserDialog();
			dialog.SelectedPath=Directory.Exists(comboPath.Text) ? comboPath.Text : $@"C:\development";
			if(DialogResult.Cancel==dialog.ShowDialog()) {
				return;
			}
			comboPath.Text=dialog.SelectedPath;
		}

		///<summary>The point of this code is to have the items in the combo box always reflect the custom development path they type into the combo box.</summary>
		private void comboPath_TextChanged(object sender,EventArgs e) {
			if(comboPath.Items.Count==0) {
				return;
			}
			string pathCur=StringTools.SubstringBefore(comboPath.Items[0] as string,"OPEN DENTAL SUBVERSION");
			string pathAfter="";
			ODException.SwallowAnyException(() => {
				pathAfter=StringTools.SubstringBefore(comboPath.Text,"OPEN DENTAL SUBVERSION");
			});
			if(string.IsNullOrEmpty(pathAfter)) {
				ODException.SwallowAnyException(() => {
					pathAfter=StringTools.SubstringBefore(comboPath.Text,"Shared Projects Subversion");
				});
			}
			if(string.IsNullOrEmpty(pathAfter)) {
				return;
			}
			for(int i=0;i<comboPath.Items.Count;i++) {
				comboPath.Items[i]=(comboPath.Items[i] as string).Replace(pathCur,pathAfter);
			}
		}

		private void butCommit_Click(object sender,EventArgs e) {
			Commit();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

		private void FormBackport_FormClosing(object sender,FormClosingEventArgs e) {
			_progressBarAction?.Invoke();
			_progressBarAction=null;
		}
	}
}
