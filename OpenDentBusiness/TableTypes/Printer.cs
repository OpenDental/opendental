using System;
using System.Collections;
using CodeBase;

namespace OpenDentBusiness{

	///<summary>One printer selection for one situation for one computer.</summary>
	[Serializable]
	public class Printer:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PrinterNum;
		///<summary>FK to computer.ComputerNum.  This will be changed some day to refer to the computername, because it would make more sense as a key than a cryptic number.</summary>
		public long ComputerNum;
		///<summary>Enum:PrintSituation One of about 10 different situations where printing takes place.  If no printer object exists for a situation, then a default is used and a prompt is displayed.</summary>
		public PrintSituation PrintSit;
		///<summary>The name of the printer as set from the specified computer.</summary>
		public string PrinterName;
		///<summary>If true, then user will be prompted for printer.  Otherwise, print directly with little user interaction.</summary>
		public bool DisplayPrompt;

		///<summary>Returns a copy of the printer.</summary>
    public Printer Clone(){
			return (Printer)this.MemberwiseClone();
		}

		
	}



	



}









