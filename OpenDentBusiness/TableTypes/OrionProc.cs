using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>This table was built for a single customer, but was then never used.  All the Orion sections of the program can be ignored and stripped out if they are in the way. 1:1 relationship to procedurelog table.  </summary>
	[Serializable]
	public class OrionProc:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrionProcNum;
		///<summary>FK to procedurelog.ProcNum</summary>
		public long ProcNum;
		///<summary>Enum:OrionDPC NotSpecified=0,None=1,_1A=2,_1B=3,_1C=4,_2=5,_3=6,_4=7,_5=8.</summary>
		public OrionDPC DPC;
		///<summary>Enum:OrionDPC None=0,1A=1,1B=2,1C=3,2=4,3=5,4=6,5=7</summary>
		public OrionDPC DPCpost;
		///<summary>System adds days to the diagnosis date based upon the DPC entered for that procedure. If DPC = none the system will return “No Schedule by Date”. </summary>
		public DateTime DateScheduleBy;
		///<summary> Default to current date.  Provider shall have to ability to edit with a previous date, but not a future date.</summary>
		public DateTime DateStopClock;
		///<summary>Enum:OrionStatus None=0,TP=1,C=2,E=4,R=8,RO=16,CS=32,CR=64,CA-Tx=128,CA-ERPD=256,CA-P/D=512,S=1024,ST=2048,W=4096,A=8192</summary>
		public OrionStatus Status2;
		///<summary>.</summary>
		public bool IsOnCall;
		///<summary>Indicates in the clinical note that effective communication was used for this encounter.</summary>
		public bool IsEffectiveComm;
		///<summary>.</summary>
		public bool IsRepair;
		


		public OrionProc Copy() {
			return (OrionProc)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum OrionDPC{
	  ///<summary>0- Not Specified</summary>
	  NotSpecified,
		///<summary>1- None</summary>
	  None,
	  ///<summary>2- Treatment to be scheduled within 1 calendar day</summary>
	  _1A,
	  ///<summary>3- Treatment to be scheduled within 30 calendar days</summary>
	  _1B,
	  ///<summary>4- Treatment to be scheduled within 60 calendar days</summary>
	  _1C,
	  ///<summary>5– Treatment to be scheduled within 120 calendar days</summary>
	  _2,
	  ///<summary>6– Treatment to be scheduled within 1 year</summary>
	  _3,
	  ///<summary>7– No further treatment is needed, no appointment needed</summary>
	  _4,
	  ///<summary>8– No appointment needed </summary>
	  _5		
	}

	///<summary></summary>
	[Flags]
	public enum OrionStatus {
		///<summary>0- None.  While a normal orion proc would never have this status2, it is still needed for flags in ChartViews.  And it's also possible that a status2 slipped through the cracks and was not assigned, leaving it with this value.</summary>
		None=0,
		///<summary>1– Treatment planned</summary>
		TP=1,
		///<summary>2– Completed</summary>
		C=2,
		///<summary>4– Existing prior to incarceration</summary>
		E=4,
		///<summary>8– Refused treatment</summary>
		R=8,
		///<summary>16– Referred out to specialist</summary>
		RO=16,
		///<summary>32– Completed by specialist</summary>
		CS=32,
		///<summary>64– Completed by registry</summary>
		CR=64,
		///<summary>128- Cancelled, tx plan changed</summary>
		CA_Tx=128,
		///<summary>256- Cancelled, eligible parole</summary>
		CA_EPRD=256,
		///<summary>512- Cancelled, parole/discharge</summary>
		CA_PD=512,
		///<summary>1024– Suspended, unacceptable plaque</summary>
		S=1024,
		///<summary>2048- Stop clock, multi visit</summary>
		ST=2048,
		///<summary>4096– Watch</summary>
		W=4096,
		///<summary>8192– Alternative</summary>
		A=8192
	}


}

