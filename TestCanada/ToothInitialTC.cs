using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace TestCanada {
	public class ToothInitialTC {
		///<summary>This helps to visually confirm which teeth have been set to extracted.  It also allows setting missing teeth for NIHB without entering any extractions.  The tooth number passed in should be in international format.</summary>
		public static void SetMissing(string toothNumInternat,long patNum) {
			ToothInitial ti=new ToothInitial();
			ti.PatNum=patNum;
			ti.InitialType=ToothInitialType.Missing;
			ti.ToothNum=Tooth.FromInternat(toothNumInternat);
			ToothInitials.Insert(ti);
		}

	}
}
