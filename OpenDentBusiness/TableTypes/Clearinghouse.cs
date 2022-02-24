using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>Since we can send e-claims to multiple clearinghouses, this table keeps track of each clearinghouse.  Will eventually be used for individual carriers as well if they accept </summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class Clearinghouse:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClearinghouseNum;
		///<summary>Description of this clearinghouse</summary>
		public string Description;
		///<summary>The path to export the X12 file to. \ is now optional.  Can be overridden by clinic-level clearinghouses.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ExportPath;
		///<summary>A list of all payors which should have claims sent to this clearinghouse. Comma delimited with no spaces.  Not necessary if IsDefault.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Payors;
		///<summary>Enum:ElectronicClaimFormat The format of the file that gets sent electronically.</summary>
		public ElectronicClaimFormat Eformat;
		///<summary>Sender ID Qualifier. Usually ZZ, sometimes 30. Seven other values are allowed as specified in X12 document, but probably never used.</summary>
		public string ISA05;
		///<summary>Used in ISA06, GS02, 1000A NM1, and 1000A PER.  If blank, then 810624427 is used to indicate Open Dental.
		///Can be overridden by clinic-level clearinghouses.</summary>
		public string SenderTIN;
		///<summary>Receiver ID Qualifier.  Usually ZZ, sometimes 30. Seven other values are allowed as specified in X12 document, but probably never used.</summary>
		public string ISA07;
		///<summary>Receiver ID. Also used in GS03. Provided by clearinghouse. Examples: BCBSGA or 0135WCH00(webMD)</summary>
		public string ISA08;
		///<summary>"P" for Production or "T" for Test.</summary>
		public string ISA15;
		///<summary>Password is usually combined with the login ID for user validation.  Can be overridden by clinic-level clearinghouses.</summary>
		public string Password;
		///<summary>The path that all incoming response files will be saved to. \ is now optional.
		///Can be overridden by clinic-level clearinghouses.</summary>
		public string ResponsePath;
		///<summary>Enum:EclaimsCommBridge  One of the included hard-coded communications briges.  Or none to just create the claim files without uploading.</summary>
		public EclaimsCommBridge CommBridge;
		///<summary>If applicable, this is the name of the client program to launch.  It is even used by the hard-coded comm bridges,
		///because the user may have changed the installation directory or exe name.  Can be overridden by clinic-level clearinghouses.</summary>
		public string ClientProgram;
		///<summary>Each clearinghouse increments their batch numbers by one each time a claim file is sent.  User never sees this number.  Maxes out at 999, then loops back to 1.  This field must NOT be cached and must be ignored in the code except where it explicitly retrieves it from the db.  Defaults to 0 for brand new clearinghouses, which causes the first batch to go out as #1.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public int LastBatchNumber;
		///<summary>Was not used.  1,2,3,or 4. The port that the modem is connected to if applicable. Always uses 9600 baud and standard settings. Will crash if port or modem not valid.</summary>
		public byte ModemPort;
		///<summary>A clearinghouse usually has a login ID that is used with the password in order to access the remote server.
		///This value is not usualy used within the actual claim.  Can be overridden by clinic-level clearinghouses.</summary>
		public string LoginID;
		///<summary>Used in 1000A NM1 and 1000A PER.  But if SenderTIN is blank, then OPEN DENTAL SOFTWARE is used instead.
		///Can be overridden by clinic-level clearinghouses.</summary>
		public string SenderName;
		///<summary>Used in 1000A PER.  But if SenderTIN is blank, then 8776861248 is used instead.  10 digit phone is required by WebMD and is 
		///universally assumed, so for now, this must be either blank or 10 digits.  Can be overridden by clinic-level clearinghouses.</summary>
		public string SenderTelephone;
		///<summary>Usually the same as ISA08, but at least one clearinghouse uses a different number here.</summary>
		public string GS03;
		///<summary>Authorization information. Almost always blank. Used for Denti-Cal.</summary>
		public string ISA02;
		///<summary>Security information. Almost always blank. Used for Denti-Cal.</summary>
		public string ISA04;
		///<summary>X12 component element separator. Two digit hexadecimal string representing an ASCII character or blank. Usually blank, implying 3A which represents ':'. For Denti-Cal, hexadecimal value 22 must be used, corresponding to '"'.</summary>
		public string ISA16;
		///<summary>X12 data element separator. Two digit hexadecimal string representing an ASCII character or blank. Usually blank, implying 2A which represents '*'. For Denti-Cal, hexadecimal value 1D must be used, corresponding to the "group separator" character which has no visual representation.</summary>
		public string SeparatorData;
		///<summary>X12 segment terminator. Two digit hexadecimal string representing an ASCII character or blank. Usually blank, implying 7E which represents '~'. For Denti-Cal, hexadecimal value 1C must be used, corresponding to the "file separator" character which has no visual representation.</summary>
		public string SeparatorSegment;
		///<summary>FK to clinic.ClinicNum.  ClinicNum=0 for HQ.</summary>
		public long ClinicNum;
		///<summary>FK to clearinghouse.ClearingHouseNum.  Never 0.  Points to the HQ copy of this clearinghouse.
		///If this copy is the HQ copy, then HqClearinghouseNum=ClearinghouseNum.</summary>
		public long HqClearinghouseNum;
		///<summary>Enum:EraBehaviors EraBehaviors.DownloadAndReceive by default.  This flag is implemented individually within each clearinghouse.  Can be overridden by clinic-level clearinghouses.</summary>
		public EraBehaviors IsEraDownloadAllowed;
		///<summary>True by default.  This flag is implemented individually within each clearinghouse.  Can be overridden by clinic-level clearinghouses.</summary>
		public bool IsClaimExportAllowed;
		///<summary>Currently only used for DentalXChange's attachment service. This indicates that the user has set up the attachment service and would like to use it in Open Dental.</summary>
		public bool IsAttachmentSendAllowed;

		public Clearinghouse() {

		}

		///<summary></summary>
		public Clearinghouse Copy() {
			return (Clearinghouse)this.MemberwiseClone();
		}

	}

	///<summary>Sets behavior for importing and receiving ERA benefits and associated claims and claim procedures.</summary>
	public enum EraBehaviors {
		///<summary>0 - Do not download ERAs/EOBs</summary>
		[Description("Do Not Download ERAs")]
		None,
		///<summary>1 - Download ERAs/EOBs, but do not mark claims and claim procedures as 'Received'.</summary>
		[Description("Download ERAs, Do Not Auto Receive")]
		DownloadDoNotReceive,
		///<summary>2 - Download ERAs/EOBs, and mark claims and claim procedures as 'Received'.</summary>
		[Description("Download ERAs and Auto Receive")]
		DownloadAndReceive
	}





}









