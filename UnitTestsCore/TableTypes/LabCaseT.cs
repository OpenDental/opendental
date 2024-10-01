using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class LabCaseT {

		///<summary>Creates a labcase.</summary>
		public static LabCase CreateLabCase(long patNum,long laboratoryNum,long provNum,long aptNum=0,long plannedAptNum=0,DateTime dateTimeDue=default,
			DateTime dateTimeCreated=default,DateTime dateTimeSent=default,DateTime dateTimeRecd=default,DateTime dateTimeChecked=default,string instructions="",
			double labFee=0,DateTime dateTStamp=default,string invoiceNum="")
		{
			LabCase labcase=new LabCase() {
				PatNum=patNum,
				LaboratoryNum=laboratoryNum,
				AptNum=aptNum,
				PlannedAptNum=plannedAptNum,
				DateTimeDue=dateTimeDue,
				DateTimeCreated=dateTimeCreated,
				DateTimeSent=dateTimeSent,
				DateTimeRecd=dateTimeRecd,
				DateTimeChecked=dateTimeChecked,
				ProvNum=provNum,
				Instructions=instructions,
				LabFee=labFee,
				DateTStamp=dateTStamp,
				InvoiceNum=invoiceNum
			};
			LabCases.Insert(labcase);
			return labcase;
		}

		///<summary>Deletes everything from the labcase table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearLabCaseTable() {
			string command="DELETE FROM labcase";
			DataCore.NonQ(command);
		}

	}
}
