using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.
	///Most schema changes are done directly on our live database as needed.  Base object for use in the job tracking system.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class Job:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey = true)]
		public long JobNum;
		///<summary>FK to userod.UserNum.  Used for Query Creator as well.</summary>
		public long UserNumConcept;
		///<summary>FK to userod.UserNum.  Used for Query Reviewer as well.</summary>
		public long UserNumExpert;
		///<summary>FK to userod.UserNum.  Used for Query Owner as well.</summary>
		public long UserNumEngineer;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumApproverConcept;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumApproverJob;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumApproverChange;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumDocumenter;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumCustContact;
		///<summary>FK to userod.UserNum. If set, this is the user currently editing the job. Once saved, this is set to 0.</summary>
		public long UserNumCheckout;
		///<summary>FK to userod.UserNum. If set, the job is waiting on clarification from the user indicated. 
		///Set back to 0 once clarified. Not actually used yet.</summary>
		public long UserNumInfo;
		///<summary>FK to job.JobNum.</summary>
		public long ParentNum;
		///<summary>The date/time that the customer was contacted.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeCustContact;
		///<summary>FK to definition.DefNum</summary>
		public long Priority;
		///<summary>Classifies the type of the job.  E.g. Feature, Bug, etc.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public JobCategory Category;
		///<summary>String representation of the version that this job is for. Example: Head Only(18.1), 15.4.19, 16.1.1, etc.</summary>
		public string JobVersion;
		///<summary>The estimated time a job will take to code.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeEstimateDevelopment;
		///<summary>Deprecated: The actual time a job took.  Use HoursActual (or something similar to the logic within) instead.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeActual;
		///<summary>The date/time that the job was created.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>The implementation of the job. RTF content of the main body of the Job.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]//Text
		public string Implementation;
		///<summary>Used to record what was documented for this job and where it was documented.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]//Text
		public string Documentation;
		///<summary>The short title of the job.</summary>
		public string Title;
		///<summary>The current status of the job.  Historical statuses for this job can be found in the jobevent table.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public JobPhase PhaseCur;
		///<summary>Applies to Several status.</summary>
		public bool IsApprovalNeeded;
		///<summary>Not yet used. Will be used for tracking acknowledgement of Bugs by Nathan. Should not halt development.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime AckDateTime;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumQuoter;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumApproverQuote;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNumCustQuote;
		///<summary>The requirements of the job. RTF content of the main requirements of the Job.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]//Text
		public string Requirements;
		///<summary>FK to userod.UserNum.  The user that has taken or was assigned this job in order to perform testing.</summary>
		public long UserNumTester;
		///<summary>FK to definition.DefNum.  Since testing can happen at during any phase, the testing department has this separate Priority.</summary>
		public long PriorityTesting;
		///<summary>Used to mark the date and time the testing was completed.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeTested;
		///<summary>The estimated time a job will take to write the concept.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeEstimateConcept;
		///<summary>The estimated time a job will take to write the writeup.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeEstimateWriteup;
		///<summary>The estimated time a job will take to review.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeEstimateReview;
		///<summary>List of JobRequirement objects. Stored as a JSON string to display in a grid.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]//Text
		public string RequirementsJSON;
		///<summary>Enum:JobPatternReviewProject The project that this job is associated to.</summary>
		public JobPatternReviewProject PatternReviewProject;
		///<summary>Enum:JobPatternReviewStatus The pattern review status of the job.</summary>
		public JobPatternReviewStatus PatternReviewStatus;
		///<summary>Enum:JobProposedVersion The proposed version a job will be completed for.</summary>
		public JobProposedVersion ProposedVersion;
		///<summary>Used to mark the date and time the concept approval was made.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeConceptApproval;
		///<summary>Used to mark the date and time the job approval was made.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeJobApproval;
		///<summary>Used to mark the date and time the development was implemented.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeImplemented;
		///<summary>The time it took to test the job.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan TimeTesting;
		///<summary>Used to mark a job as not tested.</summary>
		public bool IsNotTested;

		//The following variables should be filled by the class that uses them, not filled from an S class.
		//Just a convenient way to package a job for passing around in the job manager.
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobLink> ListJobLinks=new List<JobLink>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobNote> ListJobNotes=new List<JobNote>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobReview> ListJobReviews=new List<JobReview>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobReview> ListJobTimeLogs=new List<JobReview>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobQuote> ListJobQuotes=new List<JobQuote>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobNotification> ListJobNotifications = new List<JobNotification>();
		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<JobActiveLink> ListJobActiveLinks = new List<JobActiveLink>();

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeEstimateDevelopment),typeof(long))]
		public long TimeEstimateDevelopmentXml {
			get {
				return TimeEstimateDevelopment.Ticks;
			}
			set {
				TimeEstimateDevelopment=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeActual),typeof(long))]
		public long TimeActualXml {
			get {
				return TimeActual.Ticks;
			}
			set {
				TimeActual=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeEstimateConcept),typeof(long))]
		public long TimeEstimateConceptXml {
			get {
				return TimeEstimateConcept.Ticks;
			}
			set {
				TimeEstimateConcept=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeEstimateWriteup),typeof(long))]
		public long TimeEstimateWriteupXml {
			get {
				return TimeEstimateWriteup.Ticks;
			}
			set {
				TimeEstimateWriteup=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeEstimateReview),typeof(long))]
		public long TimeEstimateReviewXml {
			get {
				return TimeEstimateReview.Ticks;
			}
			set {
				TimeEstimateReview=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes.</summary>
		[XmlElement(nameof(TimeTesting),typeof(long))]
		public long TimeTestingXml {
			get {
				return TimeTesting.Ticks;
			}
			set {
				TimeTesting=TimeSpan.FromTicks(value);
			}
		}

		public Job() {
			JobVersion="";
			Requirements="";
			Implementation="";
			Title="";
		}

		///<summary></summary>
		public Job Copy() {
			Job job=(Job)this.MemberwiseClone();
			job.ListJobLinks=this.ListJobLinks.Select(x => x.Copy()).ToList();
			job.ListJobNotes=this.ListJobNotes.Select(x => x.Copy()).ToList();
			job.ListJobReviews=this.ListJobReviews.Select(x => x.Copy()).ToList();
			job.ListJobTimeLogs=this.ListJobTimeLogs.Select(x => x.Copy()).ToList();
			job.ListJobQuotes=this.ListJobQuotes.Select(x => x.Copy()).ToList();
			job.ListJobNotifications=this.ListJobNotifications.Select(x => x.Copy()).ToList();
			job.ListJobActiveLinks=this.ListJobActiveLinks.Select(x => x.Copy()).ToList();
			return job;
		}

		[XmlIgnore,JsonIgnore]
		///<summary>The actual hours a job has taken so far.</summary>
		public double HoursActual {
			get {
				return Math.Round(ListJobTimeLogs.Sum(x => x.TimeReview.TotalHours)+(2*ListJobReviews.Sum(x => x.TimeReview.TotalHours)),2);
			}
		}		
		
		[XmlIgnore,JsonIgnore]
		///<summary>The actual hours a job has taken so far. Only counts review time once.</summary>
		public double HoursActualSingleReviewTime {
			get {
				return Math.Round(ListJobTimeLogs.Sum(x => x.TimeReview.TotalHours)+(ListJobReviews.Sum(x => x.TimeReview.TotalHours)),2);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take.</summary>
		public double HoursEstimateConcept {
			get {
				return TimeEstimateConcept.TotalHours;
			}
			set {
				TimeEstimateConcept=TimeSpan.FromHours(value);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take.</summary>
		public double HoursEstimateWriteup {
			get {
				return TimeEstimateWriteup.TotalHours;
			}
			set {
				TimeEstimateWriteup=TimeSpan.FromHours(value);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take.</summary>
		public double HoursEstimateDevelopment {
			get {
				return TimeEstimateDevelopment.TotalHours;
			}
			set {
				TimeEstimateDevelopment=TimeSpan.FromHours(value);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take.</summary>
		public double HoursEstimateReview {
			get {
				return TimeEstimateReview.TotalHours;
			}
			set {
				TimeEstimateReview=TimeSpan.FromHours(value);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take.</summary>
		public double HoursEstimate {
			get {
				return Math.Round((TimeEstimateConcept+TimeEstimateWriteup+TimeEstimateDevelopment).TotalHours+(TimeEstimateReview.TotalHours*2),2);
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>The estimated hours a job will take. Only counts review time once.</summary>
		public double HoursEstimateSingleReviewTime {
			get {
				return Math.Round((TimeEstimateConcept+TimeEstimateWriteup+TimeEstimateDevelopment).TotalHours+(TimeEstimateReview.TotalHours),2);
			}
		}		

		[XmlIgnore,JsonIgnore]
		///<summary>The testing hours a spent on a job.</summary>
		public double HoursTesting {
				get {
					return TimeTesting.TotalHours;
				}
				set {
					TimeTesting=TimeSpan.FromHours(value);
				}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>Returns userNum of the person assigned to the next task for a job, 0 if unnassigned.</summary>
		public long OwnerNum {
			get {
				if(UserNumInfo>0) {
					return UserNumInfo;
				}
				switch(PhaseCur) {
					case JobPhase.Concept:
						if(IsApprovalNeeded) {
							return UserNumApproverConcept;
						}
						return UserNumConcept;
					case JobPhase.Quote:
						if(IsApprovalNeeded) {
							return UserNumApproverQuote;
						}
						else if(UserNumApproverQuote==0) {
							return UserNumQuoter;
						}
						return UserNumCustContact;
					case JobPhase.Definition:
						if(IsApprovalNeeded || UserNumExpert==0) {
							return UserNumApproverJob;
						}
						return UserNumExpert;
					case JobPhase.Development:
						if(IsApprovalNeeded || UserNumExpert==0) {
							return UserNumApproverJob;
						}
						if(UserNumEngineer==0) {
							return UserNumExpert;
						}
						if(ListJobReviews.Any(x => x.ReviewStatus!=JobReviewStatus.Done 
								&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalWork 
								&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalReview 
								&& x.ReviewStatus!=JobReviewStatus.SaveCommit 
								&& x.ReviewStatus!=JobReviewStatus.SaveCommitted))
						{
							JobReview review=ListJobReviews.FirstOrDefault(x => x.ReviewStatus!=JobReviewStatus.Done 
								&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalWork 
								&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalReview
								&& x.ReviewStatus!=JobReviewStatus.SaveCommit 
								&& x.ReviewStatus!=JobReviewStatus.SaveCommitted);
							if(review!=null) {
								return review.ReviewerNum;
							}
							return 0;
						}
						return UserNumEngineer;
					case JobPhase.Documentation:
						return UserNumDocumenter;
					case JobPhase.Complete:
						if(DateTimeCustContact.Year<1880) {
							return UserNumCustContact;
						}
						return 0;
					case JobPhase.Cancelled:
					default:
						return 0;
				}
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary>Same as GetOwnerAction() but wrapped in a Property for convenience. </summary>
		public JobAction OwnerAction {
			get {
				if(this.UserNumInfo>0) {
					return JobAction.AnswerQuestion;
				}
				switch(this.PhaseCur) {
					case JobPhase.Concept:
						if(this.IsApprovalNeeded) {
							return JobAction.ApproveConcept;
						}
						return JobAction.WriteConcept;
					case JobPhase.Quote:
						if(this.IsApprovalNeeded) {
							return JobAction.ApproveQuote;
						}
						else if(this.UserNumCustQuote==0) {
							return JobAction.NeedsQuote;
						}
						return JobAction.NeedsCustomerQuoteApproval;
					case JobPhase.Definition:
						if(this.IsApprovalNeeded) {
							return JobAction.ApproveJob;
						}
						if(this.UserNumExpert==0) {
							return JobAction.AssignExpert;
						}
						return JobAction.WriteJob;
					case JobPhase.Development:
						if(this.IsApprovalNeeded) {
							return JobAction.ApproveChanges;
						}
						if(this.UserNumExpert==0) {
							return JobAction.AssignExpert;
						}
						if(this.UserNumEngineer==0) {
							return JobAction.AssignEngineer;
						}
						//If at least one review is marked done then the job needs no more reviews.
						if(this.ListJobReviews.Any(x => x.ReviewStatus==JobReviewStatus.Done)) {
							return JobAction.WriteCode;
						}
						if(this.ListJobReviews.Any(x => x.ReviewStatus!=JobReviewStatus.Done 
							&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalWork 
							&& x.ReviewStatus!=JobReviewStatus.NeedsAdditionalReview
							&& x.ReviewStatus!=JobReviewStatus.SaveCommit
							&& x.ReviewStatus!=JobReviewStatus.SaveCommitted)) //.Any on empty list returns false.
						{
							return JobAction.ReviewCode;
						}
						return JobAction.WriteCode;
					case JobPhase.Documentation:
						if(UserNumDocumenter!=0) {
							return JobAction.Document;
						}
						else {
							return JobAction.NeedsTechnicalWriter;
						}
					case JobPhase.Complete:
						if(DateTimeCustContact.Year<1880) {
							return JobAction.ContactCustomer;
						}
						return JobAction.None;
					case JobPhase.Cancelled:
						return JobAction.None;
					default:
						return JobAction.UnknownJobPhase;
				}
			}
		}

		///<summary>Similar To owner action, but allows you to specify a user.</summary>
		public JobAction ActionForUser(Userod user) {
			if(user==null) {
				return OwnerAction;//should not happen, just a precaution
			}
			if(user.UserNum==0) {
				if(OwnerNum==0) {
					return OwnerAction;
				}
				return JobAction.None;
			}
			JobAction baseAction = OwnerAction;
			switch(baseAction) {
				case JobAction.WriteConcept:
					if(JobPermissions.IsAuthorized(JobPerm.Concept,true,user.UserNum) && (this.UserNumConcept==0 || this.UserNumConcept==user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.AnswerQuestion:
					if(this.UserNumInfo==user.UserNum) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.NeedsQuote:
					if(JobPermissions.IsAuthorized(JobPerm.Writeup,true,user.UserNum) && this.UserNumQuoter==user.UserNum) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.WaitForQuote:
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true,user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.ApproveQuote:
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true,user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.NeedsCustomerQuoteApproval:
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true,user.UserNum) && this.UserNumCustQuote==user.UserNum) {
						return baseAction;
					}
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true,user.UserNum)) {
						return JobAction.WaitForQuote;
					}
					return JobAction.Undefined;
				case JobAction.WriteJob:
					if(JobPermissions.IsAuthorized(JobPerm.Writeup,true,user.UserNum) && (this.UserNumExpert==0 || this.UserNumExpert==user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.AssignEngineer:
					if(this.UserNumExpert==user.UserNum) {
						return baseAction;
					}
					if(JobPermissions.IsAuthorized(JobPerm.Engineer,true,user.UserNum)) {
						return JobAction.TakeJob;
					}
					return JobAction.Undefined;
				case JobAction.WriteCode:
					if(this.UserNumEngineer==user.UserNum) {
						return baseAction;
					}
					if(this.UserNumExpert==user.UserNum) {
						return JobAction.Advise;
					}
					return JobAction.Undefined;
				case JobAction.ReviewCode:
					if(this.ListJobReviews.Any(x=> !ListTools.In(x.ReviewStatus,JobReviewStatus.Done,JobReviewStatus.NeedsAdditionalReview,JobReviewStatus.NeedsAdditionalWork,JobReviewStatus.SaveCommit,JobReviewStatus.SaveCommitted) 
					&& (x.ReviewerNum==user.UserNum || (x.ReviewerNum==0 && JobPermissions.IsAuthorized(JobPerm.Writeup,true,user.UserNum))))) {
						return baseAction;
					}
					if(this.UserNumEngineer==user.UserNum) {
						return JobAction.WaitForReview;
					}
					return JobAction.Undefined;
				case JobAction.ApproveConcept:
				case JobAction.ApproveJob:
				case JobAction.ApproveChanges:
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true,user.UserNum)) {
						return baseAction;
					}
					if((this.UserNumConcept==user.UserNum && PhaseCur==JobPhase.Concept) || this.UserNumEngineer==user.UserNum || this.UserNumExpert==user.UserNum) {
						return JobAction.WaitForApproval;
					}
					return JobAction.Undefined;
				case JobAction.AssignExpert:	
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true,user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				case JobAction.NeedsTechnicalWriter:
				case JobAction.Document:
					if(JobPermissions.IsAuthorized(JobPerm.Documentation,true,user.UserNum)) {
						return baseAction;
					}
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true,user.UserNum) && (UserNumCustContact==0 || UserNumCustContact==user.UserNum) && DateTimeCustContact.Year<1880) {
						return JobAction.ContactCustomerPreDoc;
					}
					return JobAction.Undefined;
				case JobAction.ContactCustomer:
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true,user.UserNum) && (UserNumCustContact==0 || UserNumCustContact==user.UserNum)) {
						return baseAction;
					}
					return JobAction.Undefined;
				default:
					if(ListJobLinks.Exists(x => x.LinkType==JobLinkType.Subscriber && x.FKey==user.UserNum && !ListTools.In(PhaseCur,JobPhase.Cancelled,JobPhase.Complete))) {
						return JobAction.Watching;
					}
					return baseAction;
			}
		}

		///<summary>Returns a string formated with the first character of the job category followed by the JobNum and then the title.
		///E.g. "F2457 - Title of the job will follow just after a hyphen surrounded by spaces."
		///Used primarily to display a Job in the tree view.</summary>
		public override string ToString() {
			return Category.ToString().Substring(0,1)+JobNum+" - "+Title;
		}
	}

	///<summary></summary>
	[Serializable]
	public class JobRequirement {
		///<summary></summary>
		public string Description;
		///<summary></summary>
		public bool HasExpert;
		///<summary></summary>
		public bool HasEngineer;
		///<summary></summary>
		public bool HasReviewer;
	
		///<summary></summary>
		public JobRequirement() {
		}
		
		///<summary></summary>
		public JobRequirement(string description,bool hasExpert=false,bool hasEngineer=false,bool hasReviewer=false) {
			Description=description;
			HasExpert=hasExpert;
			HasEngineer=hasEngineer;
			HasReviewer=hasReviewer;
		}
	}

	public enum JobPhase {
		///<summary>0 -</summary>
		Concept, //From Concept, NeedsConceptApproval.
		///<summary>1 -</summary>
		Definition, //From ConceptApproved, CurrentlyWriting, NeedsJobApproval, NeedsJobClarification
		///<summary>2 -</summary>
		Development, //From JobApproved,Assigned, CurrentlyWorkingOn, OnHoldExpert, ReadyForReview, OnHoldEngineer, ReadyToAssign
		///<summary>3 -</summary>
		Documentation, //From ReadyToBeDocumented, NeedsDocumentationClarification
		///<summary>4 -</summary>
		Complete, //From Complete, NotifyCustomer
		///<summary>5 -</summary>
		Cancelled, //From Rescinded, Deleted
		///<summary>6 -</summary>
		Quote,
	}

	///<summary>Used to designate which solution or project that a programming pattern is designated for.</summary>
	public enum JobPatternReviewProject {
		///<summary>0 - Not specified.</summary>
		None,
		///<summary>1 - Any exe or DLL directly related to Open Dental.</summary>
		OD,
		///<summary>2 - Other projects that do not have an explicit value in this enumeration.</summary>
		Other, 
	}

	///<summary>Review statuses designated specifically for patterns found within jobs.</summary>
	public enum JobPatternReviewStatus {
		///<summary>0 -</summary>
		None,
		///<summary>1 -</summary>
		NotNeeded, 
		///<summary>2 -</summary>
		AwaitingApproval,
		///<summary>3 -</summary>
		Approved, 
		///<summary>4 -</summary>
		Tentative, 
		///<summary>5 -</summary>
		NotApproved, 
	}

	///<summary>Never Stored in the DB. Only used for sorting and displaying. The order these values are ordered in this enum is the order they will be displayed in.</summary>
	public enum JobAction {
		[Description("Active Job")]
		ActiveJob,
		[Description("Review Code")]
		ReviewCode,
		[Description("Needs Quote")]
		NeedsQuote,
		[Description("Approve Quote")]
		ApproveQuote,
		[Description("Approve Changes")]
		ApproveChanges,
		[Description("Approve Concept")]
		ApproveConcept,
		[Description("Approve Job")]
		ApproveJob,
		[Description("Answer Question")]
		AnswerQuestion,
		[Description("Write Concept")]
		WriteConcept,
		[Description("Write Job")]
		WriteJob,
		[Description("Customer Quote")]
		NeedsCustomerQuoteApproval,
		[Description("Assign Expert")]
		AssignExpert,
		[Description("Assign Engineer")]
		AssignEngineer,
		[Description("Write Code")]
		WriteCode,
		[Description("Take Job")]
		TakeJob,
		[Description("Document")]
		Document,
		[Description("Assign Technical Writer")]
		NeedsTechnicalWriter,
		[Description("Advise")]
		Advise,
		[Description("Wait for Approval")]
		WaitForApproval,
		[Description("Wait for Quote")]
		WaitForQuote,
		[Description("Wait for Review")]
		WaitForReview,
		[Description("Contact Customer Pre-Documentation")]
		ContactCustomerPreDoc,
		[Description("Contact Customer")]
		ContactCustomer,
		[Description("Watching")]
		Watching,
		[Description("None")]
		None,
		[Description("Unknown Job Phase")]
		UnknownJobPhase,
		[Description("")]
		Undefined
	}


	public enum JobStatus {
		///<summary>0 -</summary>
		Concept, //From Concept, NeedsConceptApproval.
		///<summary>1 -</summary>
		Definiton, //Writeup, //From ConceptApproved, CurrentlyWriting, NeedsJobApproval, NeedsJobClarification
		///<summary>2 -</summary>
		Development, //From JobApproved,Assigned, CurrentlyWorkingOn, OnHoldExpert, ReadyForReview, OnHoldEngineer, ReadyToAssign
		///<summary>3 -</summary>
		Documentation, //From ReadyToBeDocumented, NeedsDocumentationClarification
		///<summary>4 -</summary>
		Complete, //From Complete, NotifyCustomer
		///<summary>5 -</summary>
		Cancelled //From Rescinded, Deleted
	}

	public enum JobCategory {
		///<summary>0 -</summary>
		Feature,
		///<summary>1 -</summary>
		Bug,
		///<summary>2 -</summary>
		Enhancement,
		///<summary>3 -</summary>
		Query,
		///<summary>4 -</summary>
		ProgramBridge,
		///<summary>5 -</summary>
		InternalRequest,
		///<summary>6 -</summary>
		HqRequest,
		///<summary>7 -</summary>
		Conversion,
		///<summary>8 -</summary>
		Research,
		///<summary>9 -</summary>
		SpecialProject,
		///<summary>10 -</summary>
		NeedNoApproval,
		///<summary>11 -</summary>
		MarketingDesign,
		///<summary>12 -</summary>
		UnresolvedIssue,
	}

	public enum JobProposedVersion {
		///<summary>0 -</summary>
		Unknown,
		///<summary>1 -</summary>
		Current,
		///<summary>2 -</summary>
		Next,
		///<summary>3 -</summary>
		Future,
	}

}


