using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Health Information Exchange clinic settings. This table stores settings for generating automatic CCDs.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class HieClinic:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long HieClinicNum;
		///<summary>FK to clinic.ClincNum.</summary>
		public long ClinicNum;
		///<summary>Enum:HieCarrierFlags AllPatient=0,Medicaid=1.  Indicates the supported carrier, bitwise.</summary>
		public HieCarrierFlags SupportedCarrierFlags;
		///<summary>The path to export CCD. This field will not be blank when enabled.</summary>
		public string PathExportCCD;
		///<summary>The time to export CCD.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong)]
		public TimeSpan TimeOfDayExportCCD;
		///<summary></summary>
		public bool IsEnabled;


		///<summary>Returns true if time to export ccds.</summary>
		public bool IsTimeToProcess() {
			TimeSpan timeStart=TimeOfDayExportCCD;
			//Time end will be time start plus 1 Hour
			TimeSpan timeEnd=TimeOfDayExportCCDEnd;
			TimeSpan timeNow=DateTime_.Now.TimeOfDay;
			if(timeStart<=timeEnd) {
				//start and end time are in the same day.
				return timeNow>=timeStart && timeNow<=timeEnd;
			}
			//Start and end times are on different days.
			return timeNow>=timeStart || timeNow<=timeEnd;
		}

		public TimeSpan TimeOfDayExportCCDEnd => TimeOfDayExportCCD+TimeSpan.FromHours(1);

		public HieClinic() {

		}

		public HieClinic(long clinicNum,TimeSpan timeOfDateExportCCD,bool isEnabled=true,
			HieCarrierFlags carrierFlags=HieCarrierFlags.AllCarriers,string pathExportCCD="") 
		{
			ClinicNum=clinicNum;
			TimeOfDayExportCCD=timeOfDateExportCCD;
			IsEnabled=isEnabled;
			SupportedCarrierFlags=carrierFlags;
			PathExportCCD=pathExportCCD;
		}
	}

	///<summary>Bit wise.</summary>
	[Flags]
	public enum HieCarrierFlags {
		///<summary>No carrier set. All carriers are supported.</summary>
		AllCarriers=0,
		Medicaid=1,
	}

}
