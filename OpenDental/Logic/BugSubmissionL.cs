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
		public static Bug AddBug(BugSubmission sub,Job job=null) {
			using FormBugEdit formBE=new FormBugEdit(new List<BugSubmission>() { sub });
			formBE.IsNew=true;
			formBE.BugCur=Bugs.GetNewBugForUser();
			if(job!=null && job.Category==JobCategory.Enhancement) {
				formBE.BugCur.Description="(Enhancement)";
			}
			if(formBE.ShowDialog()!=DialogResult.OK) {
				return null;
			}
			BugSubmissions.UpdateBugIds(formBE.BugCur.BugId,new List<BugSubmission> { sub });
			return formBE.BugCur;
		}

		public static long CreateTask(Patient pat,BugSubmission sub) {
			//Button is only enabled if _patCur is not null (user has 1 row selected).
			//Mimics FormOpenDental.OnTask_Click()
			using FormTaskListSelect FormT=new FormTaskListSelect(TaskObjectType.Patient);
			//FormT.Location=new Point(50,50);
			FormT.Text=Lan.g(FormT,"Add Task")+" - "+FormT.Text;
			FormT.ShowDialog();
			if(FormT.DialogResult!=DialogResult.OK) {
				return 0;
			}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			if(pat.PatNum!=0) {
				task.KeyNum=pat.PatNum;
				task.ObjectType=TaskObjectType.Patient;
			}
			task.TaskListNum=FormT.ListSelectedLists[0];
			task.UserNum=Security.CurUser.UserNum;
			//Mimics the ?bug quick note at HQ.
			task.Descript=BugSubmissions.GetSubmissionDescription(pat,sub);
            task.PriorityDefNum=Defs.GetDefsForCategory(DefCat.TaskPriorities).FirstOrDefault(x => x.ItemName.ToLower().Contains("yellow"))?.DefNum??0;
			using FormTaskEdit FormTE=new FormTaskEdit(task,taskOld);
			FormTE.IsNew=true;
			FormTE.ShowDialog();
			return task.TaskNum;
		}

		public static Bug AddBugAndJob(FormODBase form,List<BugSubmission> listSelectedSubs,Patient pat) {
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return null;
			}
			if(listSelectedSubs.Count==0) {
				MsgBox.Show(form,"You must select a bug submission to create a job for.");
				return null;
			}
			Job jobNew=new Job();
			jobNew.Category=JobCategory.Bug;
			using InputBox titleBox=new InputBox("Provide a brief title for the job.");
			if(titleBox.ShowDialog()!=DialogResult.OK) {
				return null;
			}
			if(String.IsNullOrEmpty(titleBox.textResult.Text)) {
				MsgBox.Show(form,"You must type a title to create a job.");
				return null;
			}
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listJobPriorities.Count==0) {
				MsgBox.Show(form,"You have no priorities setup in definitions.");
				return null;
			}
			jobNew.Title=titleBox.textResult.Text;
			long priorityNum=0;
			priorityNum=listJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault")).DefNum;
			jobNew.Priority=priorityNum==0?listJobPriorities.First().DefNum:priorityNum;
			jobNew.PhaseCur=JobPhase.Concept;
			jobNew.UserNumConcept=Security.CurUser.UserNum;
			Bug bugNew=new Bug();
			bugNew=Bugs.GetNewBugForUser();
			using InputBox bugDescription=new InputBox("Provide a brief description for the bug. This will appear in the bug tracker.",jobNew.Title);
			if(bugDescription.ShowDialog()!=DialogResult.OK) {
				return null;
			}
			if(String.IsNullOrEmpty(bugDescription.textResult.Text)) {
				MsgBox.Show(form,"You must type a description to create a bug.");
				return null;
			}
			using FormVersionPrompt FormVP=new FormVersionPrompt("Enter versions found");
			FormVP.ShowDialog();
			if(FormVP.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(FormVP.VersionText)) {
				MsgBox.Show(form,"You must enter a version to create a bug.");
				return null;
			}
			bugNew.Status_=BugStatus.Accepted;
			bugNew.VersionsFound=FormVP.VersionText;
			bugNew.Description=bugDescription.textResult.Text;
			BugSubmission sub=listSelectedSubs.First();
			jobNew.Requirements=BugSubmissions.GetSubmissionDescription(pat,sub);
			Jobs.Insert(jobNew);
			JobLink jobLinkNew=new JobLink();
			jobLinkNew.LinkType=JobLinkType.Bug;
			jobLinkNew.JobNum=jobNew.JobNum;
			jobLinkNew.FKey=Bugs.Insert(bugNew);
			JobLinks.Insert(jobLinkNew);
			BugSubmissions.UpdateBugIds(bugNew.BugId,listSelectedSubs);
			if(MsgBox.Show(form,MsgBoxButtons.YesNo,"Would you like to create a task too?")) {
				long taskNum=CreateTask(pat,sub);
				if(taskNum!=0) {
					jobLinkNew=new JobLink();
					jobLinkNew.LinkType=JobLinkType.Task;
					jobLinkNew.JobNum=jobNew.JobNum;
					jobLinkNew.FKey=taskNum;
					JobLinks.Insert(jobLinkNew);
				}
			}
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
			FormOpenDental.S_GoToJob(jobNew.JobNum);
			return bugNew;
		}

		public static bool TryAssociateSimilarBugSubmissions(Point? pointFormLocaiton=null, bool isVerbose=true) {
			List<BugSubmission> listAllSubs=BugSubmissions.GetAll();
			List<BugSubmission> listUnattachedSubs=listAllSubs.Where(x => x.BugId==0).ToList();
			if(listUnattachedSubs.Count==0) {
				if(isVerbose) {
					MsgBox.Show("FormBugSubmissions","All submissions are associated to bugs already.");
				}
				return false;
			}
			//Dictionary where key is a BugId and the value is list of submissions associated to that BugID.
			//StackTraces are unique and if there are duplicate stack trace entries we select the one with the newest version.
			Dictionary<long,List<BugSubmission>> dictAttachedSubs=listAllSubs.Where(x => x.BugId!=0)
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
					List<BugSubmission> listSimilarBugSubs=listUnattachedSubs.Where(x => 
						x.ExceptionStackTrace==sub.ExceptionStackTrace//Find submissions that are not attached to bugs with identical ExceptionStackTrace
					).ToList();
					if(listSimilarBugSubs.Count==0) {
						continue;//All submissions with this stack trace are attached to a bug.
					}
					listUnattachedSubs.RemoveAll(x => listSimilarBugSubs.Contains(x));
					dictSimilarBugSubs[key].AddRange(listSimilarBugSubs);
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
			List<long> listBugIds=listAllSubs.Where(x => x.BugId!=0).Select(x => x.BugId).ToList();
			List<JobLink> listLinks=JobLinks.GetManyForType(JobLinkType.Bug,listBugIds);
			List<Bug> listBugs=Bugs.GetMany(listBugIds);
			StringBuilder issueSubmissionPrompt=new StringBuilder();
			foreach(KeyValuePair<long,List<BugSubmission>> pair in dictSimilarBugSubs) {
				Bug bugFixed=listBugs.FirstOrDefault(x => x.BugId==pair.Key && !string.IsNullOrEmpty(x.VersionsFixed));
				if(bugFixed!=null) {
					List<BugSubmission> listIssueSubs=pair.Value.Where(x => new Version(x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))>=new Version(bugFixed.VersionsFixed.Split(';').Last())).ToList();
					if(listIssueSubs.Count>0) {
						List<JobLink> listBugJobLinks=listLinks.FindAll(x => x.FKey==bugFixed.BugId);
						List<Job> listBugJobs=Jobs.GetMany(listBugJobLinks.Select(x => x.JobNum).ToList());
						if(issueSubmissionPrompt.Length==0) {
							issueSubmissionPrompt.AppendLine("The following completed jobs have submissions from newer versions then the jobs reported fixed version: ");
						}
						listBugJobs.ForEach(x => issueSubmissionPrompt.AppendLine("- "+" ("+x.Category.ToString().Substring(0,1)+x.JobNum+")"+x.Title));
						pair.Value.RemoveAll(x => listIssueSubs.Contains(x));
						if(pair.Value.Count==0) {
							continue;
						}
					}
				}
				if(!isAutoAssign) {
					using FormBugSubmissions formGroupBugSubs=new FormBugSubmissions(formBugSubmissionMode:FormBugSubmissionMode.ValidationMode);
					formGroupBugSubs.ListBugSubmissionsViewed=pair.Value;//Add unnattached submissions to grid
					formGroupBugSubs.ListBugSubmissionsViewed.AddRange(dictAttachedSubs[pair.Key]);//Add already attached submissions to grid
					formGroupBugSubs.StartPosition=FormStartPosition.Manual;
					Point newLoc=pointFormLocaiton??new Point(0,0);
					newLoc.X+=10;//Offset
					newLoc.Y+=10;
					formGroupBugSubs.Location=newLoc;
					if(formGroupBugSubs.ShowDialog()!=DialogResult.OK) {
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
			List<BugSubmission> listAllSubs=BugSubmissions.GetAll();
			List<BugSubmission> listToBeHiddenSubs=listAllSubs.Where(x => !x.IsHidden 
				&& listAllSubs.Any(y => y!=x && y.ExceptionStackTrace==x.ExceptionStackTrace && y.IsHidden)
			).ToList();
			if(listToBeHiddenSubs.Count==0) {
				return false;
			}
			listToBeHiddenSubs.ForEach(x => x.IsHidden=true);
			BugSubmissions.UpdateMany(listToBeHiddenSubs,"IsHidden");
			return true;
		}

		public static double CalculateSimilarity(string source,string target) {
			if((source==null) || (target==null)) {
				return 0;
			}
			string src=BugSubmissions.SimplifyStackTrace(source);
			string tar=BugSubmissions.SimplifyStackTrace(target);
			src=src.Replace("\r","").Replace("\n","");
			tar=tar.Replace("\r","").Replace("\n","");
			if(src.Equals(tar)) {
				return 100;
			}
			int stepsToSame=ComputeLevenshteinDistance(src,tar);
			return (1.0-((double)stepsToSame/Math.Max(src.Length,tar.Length)))*100;
		}

		private static int ComputeLevenshteinDistance(string source,string target) {
			int sourceWordCount=source.Length;
			int targetWordCount=target.Length;
			// Step 1
			if(sourceWordCount==0)
				return targetWordCount;
			if(targetWordCount==0)
				return sourceWordCount;
			int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];
			// Step 2
			for(int i=0;i<=sourceWordCount;distance[i,0]=i++) ;//Init arrays
			for(int j=0;j<=targetWordCount;distance[0,j]=j++) ;
			for(int i=1;i<=sourceWordCount;i++) {
				for(int j=1;j<=targetWordCount;j++) {
					// Step 3
					int cost=(target[j-1]== source[i-1]) ? 0 : 1;
					// Step 4
					distance[i,j]=Math.Min(Math.Min(distance[i-1,j]+1,distance[i,j-1]+1),distance[i-1,j-1]+cost);
				}
			}
			return distance[sourceWordCount,targetWordCount];
		}

	}
}
