using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileBrandingProfiles{
		#region Methods - Get
		public static MobileBrandingProfile GetByClinicNum(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<MobileBrandingProfile>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM mobilebrandingprofile WHERE ClinicNum="+clinicNum;
			return Crud.MobileBrandingProfileCrud.SelectOne(command);
		}
		#endregion
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(MobileBrandingProfile mobileBrandingProfile){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				mobileBrandingProfile.MobileBrandingProfileNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileBrandingProfile);
				return mobileBrandingProfile.MobileBrandingProfileNum;
			}
			return Crud.MobileBrandingProfileCrud.Insert(mobileBrandingProfile);
		}
		///<summary></summary>
		public static void Update(MobileBrandingProfile mobileBrandingProfile){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBrandingProfile);
				return;
			}
			Crud.MobileBrandingProfileCrud.Update(mobileBrandingProfile);
		}

		///<summary></summary>
		public static void Delete(long mobileBrandingProfileNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBrandingProfileNum);
				return;
			}
			Crud.MobileBrandingProfileCrud.Delete(mobileBrandingProfileNum);
		}
		#endregion
		
	}
}