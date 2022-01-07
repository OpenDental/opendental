using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>.</summary>
	[Serializable()]
	public class HL7Def:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long HL7DefNum;
		///<summary></summary>
		public string Description;
		///<summary>Enum:ModeTxHL7 File, TcpIp.</summary>
		public ModeTxHL7 ModeTx;
		///<summary>Used for File mode and for SFTP mode.  For file mode, this is the folder for inbound HL7 messages.
		///For SFTP mode, this is the relative path from the SFTP root directory to the directory where the result messages can be found.
		///The root or home directory '.' can be included in the path but is not necessary.  Examples: /./results or /results or results.</summary>
		public string IncomingFolder;
		///<summary>Only used for File mode</summary>
		public string OutgoingFolder;
		///<summary>Only used for tcpip mode. Example: 1461</summary>
		public string IncomingPort;
		///<summary>Only used for tcpip mode. Example: 192.168.0.23:1462</summary>
		public string OutgoingIpPort;
		///<summary>Only relevant for outgoing. Incoming field separators are defined in MSH. Default |.</summary>
		public string FieldSeparator;
		///<summary>Only relevant for outgoing. Incoming field separators are defined in MSH. Default ^.</summary>
		public string ComponentSeparator;
		///<summary>Only relevant for outgoing. Incoming field separators are defined in MSH. Default &amp;.</summary>
		public string SubcomponentSeparator;
		///<summary>Only relevant for outgoing. Incoming field separators are defined in MSH. Default ~.</summary>
		public string RepetitionSeparator;
		///<summary>Only relevant for outgoing. Incoming field separators are defined in MSH. Default \.</summary>
		public string EscapeCharacter;
		///<summary>If this is set, then there will be no child tables. Internal types are fully defined within the C# code rather than in the database.</summary>
		public bool IsInternal;
		///<summary>Enum:HL7InternalType Stored in db as string, but used in OD as enum HL7InternalType. Example: eCWTight.  This will always have a value because we always start with a copy of some internal type.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL7InternalType InternalType;
		///<summary>Example: 12.2.14. This will be empty if IsInternal. This records the version at which they made their copy. We might have made significant improvements since their copy.</summary>
		public string InternalTypeVersion;
		///<summary>.</summary>
		public bool IsEnabled;
		///<summary></summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The machine name of the computer where the OpenDentHL7 service for this def is running.</summary>
		public string HL7Server;
		///<summary>The name of the HL7 service for this def.  Must begin with OpenDent...</summary>
		public string HL7ServiceName;
		///<summary>Enum:HL7ShowDemographics Hide,Show,Change,ChangeAndAdd</summary>
		public HL7ShowDemographics ShowDemographics;
		///<summary>Show Appointments module.</summary>
		public bool ShowAppts;
		///<summary>Show Account module</summary>
		public bool ShowAccount;
		///<summary>Send the quadrant in the tooth number component instead of the surface component of the FT1.26 field of the outgoing DFT messages.  Only for eCW.</summary>
		public bool IsQuadAsToothNum;
		///<summary>FK to definition.DefNum.  Image category used by MedLab HL7 interfaces when storing PDFs received via inbound HL7 messages.</summary>
		public long LabResultImageCat;
		///<summary>The username for logging into the Sftp server.</summary>
		public string SftpUsername;
		///<summary>The password used with the SftpUsername to log into the Sftp server.  This won't be displayed to the user but will be stored as encrypted text in the db.</summary>
		public string SftpPassword;
		///<summary>The socket used to connect to the Sftp server for retrieving inbound HL7 messages.  Currently only used by MedLabv2_3 interfaces.
		///This will be the address:port of the Sftp server to connect to for retrieving lab results.  Example: server.address.com:20020.</summary>
		public string SftpInSocket;
		///<summary>For eCW HL7 interfaces only.  False by default.  When false, D codes sent in outbound DFT messages will be limited to 5 characters.
		///Any additional characters will be stripped off when generating the HL7 message.  When true, D codes will not be truncated.</summary>
		public bool HasLongDCodes;
		///<summary>If true a message box will warn users if they try to send procedures from the chart module that are not attached to an 
		///appointment.</summary>
		public bool IsProcApptEnforced;


		///<Summary>List of messages associated with this hierarchical definition.  Use items in this list to get to items lower in the hierarchy.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<HL7DefMessage> hl7DefMessages;

		///<summary></summary>
		public HL7Def Clone() {
			return (HL7Def)this.MemberwiseClone();
		}

		public void AddMessage(HL7DefMessage msg,MessageTypeHL7 messageType,MessageStructureHL7 messageStructure,InOutHL7 inOrOut,int itemOrder,string note) {
			if(hl7DefMessages==null) {
				hl7DefMessages=new List<HL7DefMessage>();
			}
			msg.MessageType=messageType;
			msg.MessageStructure=messageStructure;
			msg.InOrOut=inOrOut;
			msg.ItemOrder=itemOrder;
			msg.Note=note;
			this.hl7DefMessages.Add(msg);
		}

		public void AddMessage(HL7DefMessage msg,MessageTypeHL7 messageType,MessageStructureHL7 messageStructure,InOutHL7 inOrOut,int itemOrder) {
			AddMessage(msg,messageType,messageStructure,inOrOut,itemOrder,"");
		}

	}

	///<summary></summary>
	public enum ModeTxHL7 {
		///<summary>0</summary>
		File,
		///<summary>1</summary>
		TcpIp,
		///<summary>2.  Used for MedLab HL7 transmission, currently only LabCorp.</summary>
		Sftp
	}

	///<summary></summary>
	public enum HL7ShowDemographics {
		///<summary>Cannot see or change.</summary>
		Hide,
		///<summary>Can see, but not change.</summary>
		Show,
		///<summary>Can change, but not add patients.  Might get overwritten by next incoming message.</summary>
		Change,
		///<summary>Can change and add patients.  Might get overwritten by next incoming message.</summary>
		ChangeAndAdd
	}

	///<summary>Stored in the database as a string.  The items in this enumeration can be freely rearranged without damaging the database.  But can't change spelling or remove existing items.</summary>
	public enum HL7InternalType {
		///<summary><para>Message structure is identical to eCWTight, minor changes in the program like showing patient demographics and the account module.</para>
		///<para>Like eCWTight, eCW dictates the patients' PatNums in PID.2, so we try to locate the patient with that PatNum.</para>
		///<para>If not found we do not attempt to use PID.4 ChartNumber or name, we assume new patient and insert.</para></summary>
		eCWFull,
		///<summary><para>Only Incoming ADT messages are processed, OD is responsible for adding patients so we assign PatNum.</para>
		///<para>The incoming messages patient ID in PID.2 is stored as ChartNumber and PID.4 is not processed.</para>
		///<para>The Account and Chart modules are visible and the users can change and add patients in OD.  No outgoing messages.</para></summary>
		eCWStandalone,
		///<summary><para>Patient demographics are hidden as well as account and appt modules.</para>
		///<para>We let eCW dictate the PatNum values in PID.2 and trust that they are unique and longs (no string characters).</para>
		///<para>Unlike Standalone, if the pat isn't found by PID.2 PatNum we don't try to locate the pat by PID.4 ChartNumber or name, we assume it's a new pat.</para></summary>
		eCWTight,
		///<summary><para>Account and Appointment modules are visible and users can change and add patients.</para>
		///<para>Only outgoing DFT message defined, no incoming messages are processed.</para></summary>
		Centricity,
		///<summary><para>Our default behavior for processing and sending HL7 messages.</para>
		///<para>Send and receive ADT and SIU messages, receive DFT messages.</para>
		///<para>The v2.6 documentation claims both PID.2 and PID.4 are only retained for backward compatibility and PID.3 is now required and used for a list of patient IDs.</para>
		///<para>We will still put ChartNumber in PID.2 (used to be referred to as 'external ID' by HL7 doc) for outgoing msgs and look for our PatNum in PID.2 for incoming msgs.</para>
		///<para>We will now also check PID.3 for a repitition that contains our PatNum as part of the CX data type.</para>
		///<para>Account and Appointments module are visible and users can change and add patients.</para></summary>
		HL7v2_6,
		///<summary>This is currently used for LabCorp and is based on HL7 version 2.3 specifications.
		///<para>This interface has been built to the LabCorp standards and may not match the HL7 version 2.3 specs exactly.</para></summary>
		MedLabv2_3
	}

	
}
