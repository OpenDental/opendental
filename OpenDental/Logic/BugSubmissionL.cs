using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	class BugSubmissionL {
		
		///<summary>Attempts to add a bug for the given sub and job.
		///Job can be null, only used for pre appending "(Enhancement)" to bug description.
		///Returns null if user canceled.</summary>
		public static Bug AddBug(BugSubmission bugSubmission,Job job=null) {
			using FormBugEdit formBugEdit=new FormBugEdit(new List<BugSubmission>() { bugSubmission });
			formBugEdit.IsNew=true;
			formBugEdit.BugCur=Bugs.GetNewBugForUser();
			if(job!=null && job.Category==JobCategory.Enhancement) {
				formBugEdit.BugCur.Description="(Enhancement)";
			}
			if(formBugEdit.ShowDialog()!=DialogResult.OK) {
				return null;
			}
			BugSubmissions.UpdateBugIds(formBugEdit.BugCur.BugId,new List<BugSubmission> { bugSubmission });
			return formBugEdit.BugCur;
		}

		public static long CreateTask(Patient patient,BugSubmission bugSubmission) {
			//Button is only enabled if _patCur is not null (user has 1 row selected).
			//Mimics FormOpenDental.OnTask_Click()
			using FormTaskListSelect formTaskListSelect=new FormTaskListSelect(TaskObjectType.Patient);
			//FormT.Location=new Point(50,50);
			formTaskListSelect.Text=Lan.g(formTaskListSelect,"Add Task")+" - "+formTaskListSelect.Text;
			formTaskListSelect.ShowDialog();
			if(formTaskListSelect.DialogResult!=DialogResult.OK) {
				return 0;
			}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			if(patient.PatNum!=0) {
				task.KeyNum=patient.PatNum;
				task.ObjectType=TaskObjectType.Patient;
			}
			task.TaskListNum=formTaskListSelect.ListSelectedLists[0];
			task.UserNum=Security.CurUser.UserNum;
			//Mimics the ?bug quick note at HQ.
			task.Descript=BugSubmissions.GetSubmissionDescription(patient,bugSubmission);
            task.PriorityDefNum=Defs.GetDefsForCategory(DefCat.TaskPriorities).FirstOrDefault(x => x.ItemName.ToLower().Contains("yellow"))?.DefNum??0;
			using FormTaskEdit formTaskEdit=new FormTaskEdit(task,taskOld);
			formTaskEdit.IsNew=true;
			formTaskEdit.ShowDialog();
			return task.TaskNum;
		}

		public static Bug AddBugAndJob(FormODBase formODBase,List<BugSubmission> listBugSubmissionsSelected,Patient patient) {
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return null;
			}
			if(listBugSubmissionsSelected.Count==0) {
				MsgBox.Show(formODBase,"You must select a bug submission to create a job for.");
				return null;
			}
			Job job=new Job();
			job.Category=JobCategory.Bug;
			InputBox inputBoxTitle=new InputBox("Provide a brief title for the job.");
			inputBoxTitle.ShowDialog();
			if(inputBoxTitle.IsDialogCancel) {
				return null;
			}
			if(String.IsNullOrEmpty(inputBoxTitle.StringResult)) {
				MsgBox.Show(formODBase,"You must type a title to create a job.");
				return null;
			}
			List<Def> listDefsJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listDefsJobPriorities.Count==0) {
				MsgBox.Show(formODBase,"You have no priorities setup in definitions.");
				return null;
			}
			job.Title=inputBoxTitle.StringResult;
			long priorityNum=0;
			priorityNum=listDefsJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault")).DefNum;
			job.Priority=priorityNum==0?listDefsJobPriorities.First().DefNum:priorityNum;
			job.PhaseCur=JobPhase.Concept;
			job.UserNumConcept=Security.CurUser.UserNum;
			Bug bug=new Bug();
			bug=Bugs.GetNewBugForUser();
			InputBox inputBoxBugDescription=new InputBox("Provide a brief description for the bug. This will appear in the bug tracker.",job.Title);
			inputBoxBugDescription.ShowDialog();
			if(inputBoxBugDescription.IsDialogCancel) {
				return null;
			}
			if(String.IsNullOrEmpty(inputBoxBugDescription.StringResult)) {
				MsgBox.Show(formODBase,"You must type a description to create a bug.");
				return null;
			}
			using FormVersionPrompt formVersionPrompt=new FormVersionPrompt("Enter versions found");
			formVersionPrompt.ShowDialog();
			if(formVersionPrompt.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(formVersionPrompt.VersionText)) {
				MsgBox.Show(formODBase,"You must enter a version to create a bug.");
				return null;
			}
			bug.Status_=BugStatus.Accepted;
			bug.VersionsFound=formVersionPrompt.VersionText;
			bug.Description=inputBoxBugDescription.StringResult;
			BugSubmission sub=listBugSubmissionsSelected.First();
			job.Requirements=BugSubmissions.GetSubmissionDescription(patient,sub);
			Jobs.Insert(job);
			JobLink jobLink=new JobLink();
			jobLink.LinkType=JobLinkType.Bug;
			jobLink.JobNum=job.JobNum;
			jobLink.FKey=Bugs.Insert(bug);
			JobLinks.Insert(jobLink);
			BugSubmissions.UpdateBugIds(bug.BugId,listBugSubmissionsSelected);
			if(MsgBox.Show(formODBase,MsgBoxButtons.YesNo,"Would you like to create a task too?")) {
				long taskNum=CreateTask(patient,sub);
				if(taskNum!=0) {
					jobLink=new JobLink();
					jobLink.LinkType=JobLinkType.Task;
					jobLink.JobNum=job.JobNum;
					jobLink.FKey=taskNum;
					JobLinks.Insert(jobLink);
				}
			}
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			FormOpenDental.S_GoToJob(job.JobNum);
			return bug;
		}

		public static bool TryAssociateSimilarBugSubmissions(Point? pointFormLocaiton=null, bool isVerbose=true) {
			List<BugSubmission> listBugSubmissionsAll=BugSubmissions.GetAll();
			List<BugSubmission> listBugSubmissionsUnattached=listBugSubmissionsAll.Where(x => x.BugId==0).ToList();
			if(listBugSubmissionsUnattached.Count==0) {
				if(isVerbose) {
					MsgBox.Show("FormBugSubmissions","All submissions are associated to bugs already.");
				}
				return false;
			}
			//Dictionary where key is a BugId and the value is list of submissions associated to that BugID.
			//StackTraces are unique and if there are duplicate stack trace entries we select the one with the newest version.
			Dictionary<long,List<BugSubmission>> dictAttachedSubs=listBugSubmissionsAll.Where(x => x.BugId!=0)
				.GroupBy(x => x.BugId)
				.ToDictionary(x => x.Key, x => 
					x.GroupBy(y => y.ExceptionStackTrace)
					//Sub dictionary of unique ExceptionStackStraces as the key and the value is the submission from the highest version.
					.ToDictionary(y => y.Key, y => y.OrderByDescending(z => new Version(z.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))).First())
					.Values.ToList()
				);
			Dictionary<long,List<BugSubmission>> dictSimilarBugSubs=new Dictionary<long,List<BugSubmission>>();
			List<long> listOrderedKeys=dictAttachedSubs.Keys.OrderByDescending(x => x).ToList();
			foreach(long key in listOrderedKeys) {//Loop through submissions that are already attached to bugs
				dictSimilarBugSubs[key]=new List<BugSubmission>();
				foreach(BugSubmission sub in dictAttachedSubs[key]) {//Loop through the unique exception text from the submission with thie highest reported version.
					List<BugSubmission> listBugSubmissionsSimilar=listBugSubmissionsUnattached.Where(x => 
						x.ExceptionStackTrace==sub.ExceptionStackTrace//Find submissions that are not attached to bugs with identical ExceptionStackTrace
					).ToList();
					if(listBugSubmissionsSimilar.Count==0) {
						continue;//All submissions with this stack trace are attached to a bug.
					}
					listBugSubmissionsUnattached.RemoveAll(x => listBugSubmissionsSimilar.Contains(x));
					dictSimilarBugSubs[key].AddRange(listBugSubmissionsSimilar);
				}
			}
			if(dictSimilarBugSubs.All(x => x.Value.Count==0)) {
				if(isVerbose) {
					MsgBox.Show("FormBugSubmissions","All similar submissions are already attached to bugs.  No action needed.");
				}
				return false;
			}
			dictSimilarBugSubs=dictSimilarBugSubs.Where(x => x.Value.Count!=0).ToDictionary(x => x.Key,x => x.Value);
			bool isAutoAssign=true;
			if(isVerbose) {
				isAutoAssign=(MsgBox.Show("FormBugSubmissions",MsgBoxButtons.YesNo,"Click Yes to auto attach duplicate submissions to bugs with identical stack traces?"
					+"\r\nClick No to manually validate all groupings found."));
			}
			List<long> listBugIds=listBugSubmissionsAll.Where(x => x.BugId!=0).Select(x => x.BugId).ToList();
			List<JobLink> listJobLinks=JobLinks.GetManyForType(JobLinkType.Bug,listBugIds);
			List<Bug> listBugs=Bugs.GetMany(listBugIds);
			StringBuilder issueSubmissionPrompt=new StringBuilder();
			foreach(KeyValuePair<long,List<BugSubmission>> pair in dictSimilarBugSubs) {
				Bug bugFixed=listBugs.FirstOrDefault(x => x.BugId==pair.Key && !string.IsNullOrEmpty(x.VersionsFixed));
				if(bugFixed!=null) {
					List<BugSubmission> listIssueSubs=pair.Value.Where(x => new Version(x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))>=new Version(bugFixed.VersionsFixed.Split(';').Last())).ToList();
					if(listIssueSubs.Count>0) {
						List<JobLink> listJobLinksBug=listJobLinks.FindAll(x => x.FKey==bugFixed.BugId);
						List<Job> listJobsBug=Jobs.GetMany(listJobLinksBug.Select(x => x.JobNum).ToList());
						if(issueSubmissionPrompt.Length==0) {
							issueSubmissionPrompt.AppendLine("The following completed jobs have submissions from newer versions then the jobs reported fixed version: ");
						}
						listJobsBug.ForEach(x => issueSubmissionPrompt.AppendLine("- "+" ("+x.Category.ToString().Substring(0,1)+x.JobNum+")"+x.Title));
						pair.Value.RemoveAll(x => listIssueSubs.Contains(x));
						if(pair.Value.Count==0) {
							continue;
						}
					}
				}
				if(!isAutoAssign) {
					using FormBugSubmissions formBugSubmissionsGroup=new FormBugSubmissions(formBugSubmissionMode:FormBugSubmissionMode.ValidationMode);
					formBugSubmissionsGroup.ListBugSubmissionsViewed=pair.Value;//Add unnattached submissions to grid
					formBugSubmissionsGroup.ListBugSubmissionsViewed.AddRange(dictAttachedSubs[pair.Key]);//Add already attached submissions to grid
					formBugSubmissionsGroup.StartPosition=FormStartPosition.Manual;
					Point point=pointFormLocaiton??new Point(0,0);
					point.X+=10;//Offset
					point.Y+=10;
					formBugSubmissionsGroup.Location=point;
					if(formBugSubmissionsGroup.ShowDialog()!=DialogResult.OK) {
						continue;
					}	
				}
				BugSubmissions.UpdateBugIds(pair.Key,pair.Value);
			}
			if(isVerbose) {
				string msg="";
				dictSimilarBugSubs.Keys.ToList().FindAll(x => dictSimilarBugSubs[x].Count>0)
					.ForEach(x => msg+="Bug: "+x+" Found submissions: "+dictSimilarBugSubs[x].Count+"\r\n");
				msg+=issueSubmissionPrompt.ToString();
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg) {Text="Done"};
				msgBoxCopyPaste.ShowDialog();
			}
			return true;
		}

		public static bool HideMatchedBugSubmissions() {
			List<BugSubmission> listBugSubmissionsAll=BugSubmissions.GetAll();
			List<BugSubmission> listBugSubmissionsToBeHidden=listBugSubmissionsAll.Where(x => !x.IsHidden 
				&& listBugSubmissionsAll.Any(y => y!=x && y.ExceptionStackTrace==x.ExceptionStackTrace && y.IsHidden)
			).ToList();
			if(listBugSubmissionsToBeHidden.Count==0) {
				return false;
			}
			listBugSubmissionsToBeHidden.ForEach(x => x.IsHidden=true);
			BugSubmissions.UpdateMany(listBugSubmissionsToBeHidden,"IsHidden");
			return true;
		}

		public static double CalculateSimilarity(string source,string target) {
			if((source==null) || (target==null)) {
				return 0;
			}
			string source2=BugSubmissions.SimplifyStackTrace(source);
			string target2=BugSubmissions.SimplifyStackTrace(target);
			source2=source2.Replace("\r","").Replace("\n","");
			target2=target2.Replace("\r","").Replace("\n","");
			if(source2.Equals(target2)) {
				return 100;
			}
			int stepsToSameNum=ComputeLevenshteinDistance(source2,target2);
			return (1.0-((double)stepsToSameNum/Math.Max(source2.Length,target2.Length)))*100;
		}

		private static int ComputeLevenshteinDistance(string source,string target) {
			int sourceWordCount=source.Length;
			int targetWordCount=target.Length;
			// Step 1
			if(sourceWordCount==0){
				return targetWordCount;
			}
			if(targetWordCount==0){
				return sourceWordCount;
			}
			int[,] intArrayDistance = new int[sourceWordCount + 1, targetWordCount + 1];
			// Step 2
			for(int i=0;i<=sourceWordCount;intArrayDistance[i,0]=i++) ;//Init arrays
			for(int j=0;j<=targetWordCount;intArrayDistance[0,j]=j++) ;
			for(int i=1;i<=sourceWordCount;i++) {
				for(int j=1;j<=targetWordCount;j++) {
					// Step 3
					int cost=(target[j-1]== source[i-1]) ? 0 : 1;
					// Step 4
					intArrayDistance[i,j]=Math.Min(Math.Min(intArrayDistance[i-1,j]+1,intArrayDistance[i,j-1]+1),intArrayDistance[i-1,j-1]+cost);
				}
			}
			return intArrayDistance[sourceWordCount,targetWordCount];
		}

	}
}
