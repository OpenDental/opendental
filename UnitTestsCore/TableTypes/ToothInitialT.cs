using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ToothInitialT {

		///<summary>Creates an entry for the toothinitial table. Set isValid to false to generate one with an invalid DrawingSegment for testing DBM</summary>
		public static ToothInitial CreateToothInitial(Patient pat,string drawingSegment,string toothNum="32",ToothInitialType toothInitialType=ToothInitialType.Drawing,
			Color colorDraw=default,float movement=0,string drawText="")
		{
			ToothInitial newToothInit=new ToothInitial() {
				PatNum=pat.PatNum,
				ToothNum=toothNum,
				InitialType=toothInitialType,
				DrawingSegment=drawingSegment,
				ColorDraw=colorDraw,
				Movement=movement,
				DrawText=drawText
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
