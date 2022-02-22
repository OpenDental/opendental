using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
using CodeBase;
using System.Data;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using System.ComponentModel;

namespace OpenDentBusiness {


	///<summary>A limited fee class that eliminates unused fields. Used by the FeeCache to keep memory usage down.</summary>
	[XmlType(TypeName="A")]
	public class FeeLim {
		///<summary>Primary key.</summary>
		[XmlElement("A",typeof(long))]
		public long FeeNum;
		///<summary>The amount usually charged.  If an amount is unknown, then the entire Fee entry is deleted from the database.  
		///The absence of a fee is shown in the user interface as a blank entry.
		///For clinic and/or provider fees, amount can be set to -1 which indicates that their fee should be blank and not use the default fee.
		///</summary>
		[XmlElement("B",typeof(double))]
		public double Amount;
		///<summary>FK to feesched.FeeSchedNum.</summary>
		[XmlElement("C",typeof(long))]
		public long FeeSched;
		///<summary>FK to procedurecode.CodeNum.</summary>
		[XmlElement("D",typeof(long))]
		public long CodeNum;
		///<summary>FK to clinic.ClinicNum. (Used only if localization of fees for a feesched is enabled)</summary>
		[XmlElement("E",typeof(long)),DefaultValue(0L)]
		public long ClinicNum;
		///<summary>FK to provider.ProvNum. (Used only if localization of fees for a feesched is enabled)</summary>
		[XmlElement("F",typeof(long)),DefaultValue(0L)]
		public long ProvNum;
		///<summary>Date. Used in securitylogs to track when fees are updated or modified.</summary>
		[XmlElement("G",typeof(DateTime))]
		public DateTime SecDateTEdit;

		/// <summary>Enable casting between Fee and Cached Fee. Will fail if attempting to cast a null. Casting will always return a deep copy.</summary>
		public static explicit operator Fee(FeeLim f) {
			return new Fee() {
				FeeNum=f.FeeNum,
				Amount=f.Amount,
				FeeSched=f.FeeSched,
				CodeNum=f.CodeNum,
				ClinicNum=f.ClinicNum,
				ProvNum=f.ProvNum,
				SecDateTEdit=f.SecDateTEdit
			};
		}

		/// <summary>Enable casting between Fee and Cached Fee. Will fail if attempting to cast a null. Casting will always return a deep copy.</summary>
		public static explicit operator FeeLim(Fee f) {
			return new FeeLim() {
				FeeNum=f.FeeNum,
				Amount=f.Amount,
				FeeSched=f.FeeSched,
				CodeNum=f.CodeNum,
				ClinicNum=f.ClinicNum,
				ProvNum=f.ProvNum,
				SecDateTEdit=f.SecDateTEdit
			};
		}

		///<summary>Return a deep copy.</summary>
		public FeeLim Copy() {
			return (FeeLim)this.MemberwiseClone();
		}
	}

}
