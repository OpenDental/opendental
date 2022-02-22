using System;
using System.Collections;

namespace OpenDentBusiness {
	
	///<summary>Indicates relase of both ordinary and server kinds.</summary>
	public class VersionRelease{
		///<summary>Primary key.</summary>
		public int VersionReleaseId;
		public int MajorNum;
		public int MinorNum;
		public int BuildNum;
		public bool IsForeign;
		public DateTime DateRelease;
		public bool IsBeta;
		public bool HasConvertScript;

		///<summary>Returns a copy of this VersionRelease.</summary>
		public VersionRelease Copy(){
			return (VersionRelease)this.MemberwiseClone();
		}

		///<Summary>Includes the last .0</Summary>
		public string MajMinBuild0() {
			string str=MajorNum.ToString()+"."+MinorNum.ToString()+"."+BuildNum.ToString()+".0";
			return str;
		}
		
	}

	


	


}









