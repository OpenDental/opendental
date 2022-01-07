using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>HQ only table. This table stores voicemails that need to be listened to by triage operators.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class VoiceMail:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long VoiceMailNum;
		///<summary>FK to userod.UserNum. The user who has taken ownership of this voicemail. Will be 0 if no one has claimed it yet.</summary>
		public long UserNum;
		///<summary>FK to patient.PatNum. The patient that matches the phone number. If no patient is found with a matching phone number, this value
		///will be 48015 (Misc.). If multiple patients are found with a matching phone number, this value will be zero.</summary>
		public long PatNum;
		///<summary>The date/time that the voicemail was made. Not necessarily the time the row is entered into the database.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateCreated;
		///<summary>The duration in seconds of the message. Will be -1 if the duration is not known.</summary>
		public int Duration;
		///<summary>The name of the file where the voicemail is stored. It will likely be a .wav file. Absolute file path.</summary>
		public string FileName;
		///<summary>The phone number (digits only) from the caller ID field of the voicemail.</summary>
		public string PhoneNumber;
		///<summary>Enum:VoiceMailStatus Voice mails will be archived for one month before being permanently deleted.</summary>
		public VoiceMailStatus StatusVM;
		///<summary>Any notes that techs want to make for a voicemail.</summary>
		public string Note;
		///<summary>The most recent date/time that the voicemail was claimed by another tech. Used to prevent a tech from claiming a voicemail that
		///has just been claimed by another tech.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateClaimed;

		///<summary>The user name of the tech who has claimed this voicemail.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string UserName;
		///<summary>The patient name that this voicemail is attached to. Will say '(Multiple)' if more than one match is found.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string PatientName;

		///<summary></summary>
		public VoiceMail Copy() {
			return (VoiceMail)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum VoiceMailStatus {
		///<summary>No user has marked this VM deleted.</summary>
		Active,
		///<summary>A user has marked this VM deleted. Will be permanently deleted after a month.</summary>
		Deleted
	}
}

/*
CREATE TABLE voicemail (
		VoiceMailNum BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
		UserNum BIGINT NOT NULL,
		PatNum BIGINT NOT NULL,
		DateCreated DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
		Duration INT NOT NULL,
		FileName VARCHAR(255) NOT NULL,
		PhoneNumber VARCHAR(30) NOT NULL,
		StatusVM TINYINT NOT NULL,
		INDEX(UserNum),
		INDEX(PatNum),
		INDEX(StatusVM)
) DEFAULT CHARSET=utf8

INSERT INTO preference (PrefName,ValueString) VALUES("VoiceMailMonitorHeartBeat","2016-01-01 00:00:00");

INSERT INTO preference (PrefName,ValueString) VALUES("VoiceMailDeleteAfterDays","30");

ALTER TABLE voicemail ADD Note VARCHAR(4000) NOT NULL;

ALTER TABLE voicemail ADD DateClaimed datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
*/
