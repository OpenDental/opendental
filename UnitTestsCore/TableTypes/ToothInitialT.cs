using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ToothInitialT {

		///<summary>Creates an entry for the tooth intial table. Set isValid to false to generate one with an invalid DrawingSegment for testing DBM</summary>
		public static ToothInitial CreateToothInitial(Patient pat,string drawingSegment) {
			ToothInitial newToothInit=new ToothInitial() {
				PatNum=pat.PatNum,
				ToothNum="32",
				InitialType=ToothInitialType.Drawing,
				DrawingSegment=drawingSegment
			};
			ToothInitials.Insert(newToothInit);
			return newToothInit;
		}

		public static void ClearTable() {
			string command="DELETE FROM toothinitial";
			DataCore.NonQ(command);
		}
	}
}
