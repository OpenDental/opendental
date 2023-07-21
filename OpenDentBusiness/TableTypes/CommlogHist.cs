using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	[Serializable]
	[CrudTable(IsLargeTable=true,IsMissingInGeneral=true)]
	public class CommlogHist:TableBase {
		#region CommlogHist rows
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CommlogHistNum;
		///<summary>Raw Text of customer phone number that the tech was talking to.</summary>
		public string CustomerNumberRaw;
		///<summary>Enum:CommlogHistSource Indicates what event triggered this CommlogHist row to be created.</summary>
		public CommlogHistSource HistSource;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Track Date Created for commloghists. Value for existing commlogs show as blank in the UI. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		#endregion CommlogHist rows

		#region Copy/Paste Inheritance from Commlog
		///<summary>Copied from Commlog.</summary>
		public long CommlogNum;
		///<summary>Copied from Commlog.</summary>
		public long PatNum;
		///<summary>Copied from Commlog.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime CommDateTime;
		///<summary>Copied from Commlog.</summary>
		public long CommType;
		///<summary>Copied from Commlog.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText | CrudSpecialColType.CleanText)]
		public string Note;
		///<summary>Copied from Commlog.</summary>
		public CommItemMode Mode_;
		///<summary>Copied from Commlog.</summary>
		public CommSentOrReceived SentOrReceived;
		///<summary>Copied from Commlog.</summary>
		public long UserNum;
		///<summary>Copied from Commlog.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Signature;
		///<summary>Copied from Commlog.</summary>
		public bool SigIsTopaz;
		///<summary>Copied from Commlog.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEnd;
		///<summary>Copied from Commlog.</summary>
		public CommItemSource CommSource;
		///<summary>Copied from Commlog.</summary>
		public long ProgramNum;
		///<summary>Copied from Commlog.</summary>
		public long ReferralNum;
		///<summary>Copied from Commlog.</summary>
		public EnumCommReferralBehavior CommReferralBehavior;
		#endregion Copy/Paste Inheritance from Commlog

		///<summary>Returns a copy of this CommlogHist.</summary>
		public CommlogHist Copy() {
			return (CommlogHist)MemberwiseClone();
		}

		///<summary>Enable casting between Commlog and CommlogHist. Will fail if attempting to cast a null. Casting will always return a deep copy.</summary>
		public static Commlog ConvertToCommlog(CommlogHist commlogHist) {
			Commlog commlog=new Commlog();
			commlog.CommlogNum=commlogHist.CommlogNum;
			commlog.PatNum=commlogHist.PatNum;
			commlog.CommDateTime=commlogHist.CommDateTime;
			commlog.CommType=commlogHist.CommType;
			commlog.Note=commlogHist.Note;
			commlog.Mode_=commlogHist.Mode_;
			commlog.SentOrReceived=commlogHist.SentOrReceived;
			commlog.UserNum=commlogHist.UserNum;
			commlog.Signature=commlogHist.Signature;
			commlog.SigIsTopaz=commlogHist.SigIsTopaz;
			commlog.DateTStamp=commlogHist.DateTStamp;
			commlog.DateTimeEnd=commlogHist.DateTimeEnd;
			commlog.CommSource=commlogHist.CommSource;
			commlog.ProgramNum=commlogHist.ProgramNum;
			commlog.DateTEntry=commlogHist.DateTEntry;
			commlog.ReferralNum=commlogHist.ReferralNum;
			commlog.CommReferralBehavior=commlogHist.CommReferralBehavior;
			return commlog;
		}
	}

	///<summary>Main events throughout the program that trigger commloghist entries to be made.</summary>
	public enum CommlogHistSource {
		///<summary>When the Phone Tracking Server (PTS) has put the phone tile associated to the employee logged in into the WrapUp status.</summary>
		WrapUp,
	}


	/*
	command="DROP TABLE IF EXISTS commloghist";
Db.NonQ(command);
command=@"CREATE TABLE commloghist (
	CommlogHistNum bigint NOT NULL auto_increment PRIMARY KEY,
	CustomerNumberRaw varchar(255) NOT NULL,
	HistSource tinyint NOT NULL,
	DateTStamp timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
	CommlogNum bigint NOT NULL,
	PatNum bigint NOT NULL,
	CommDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
	CommType bigint NOT NULL,
	Note text NOT NULL,
	Mode_ tinyint NOT NULL,
	SentOrReceived tinyint NOT NULL,
	UserNum bigint NOT NULL,
	Signature text NOT NULL,
	SigIsTopaz tinyint NOT NULL,
	DateTimeEnd datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
	CommSource tinyint NOT NULL,
	ProgramNum bigint NOT NULL,
	ReferralNum bigint NOT NULL,
	CommReferralBehavior tinyint NOT NULL,
	INDEX(CommlogNum),
	INDEX(PatNum),
	INDEX(CommType),
	INDEX(UserNum),
	INDEX(ProgramNum),
	INDEX(ReferralNum)
	) DEFAULT CHARSET=utf8";
Db.NonQ(command);
	*/
}
