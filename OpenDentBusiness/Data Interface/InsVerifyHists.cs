using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsVerifyHists{
		#region Get Methods
		public static List<InsVerifyHist> GetForFKeyByType(long fKey,VerifyTypes verifyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsVerifyHist>>(MethodBase.GetCurrentMethod(),fKey,verifyType);
			}
			string command=$"SELECT * FROM insverifyhist WHERE FKey={POut.Long(fKey)} AND VerifyType={POut.Int((int)verifyType)}";
			return Crud.InsVerifyHistCrud.SelectMany(command);
		}
		#endregion

		///<summary>Gets one InsVerifyHist from the db.</summary>
		public static InsVerifyHist GetOne(long insVerifyHistNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsVerifyHist>(MethodBase.GetCurrentMethod(),insVerifyHistNum);
			}
			return Crud.InsVerifyHistCrud.SelectOne(insVerifyHistNum);
		}

		///<summary></summary>
		public static long Insert(InsVerifyHist insVerifyHist) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				insVerifyHist.InsVerifyHistNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insVerifyHist);
				return insVerifyHist.InsVerifyHistNum;
			}
			return Crud.InsVerifyHistCrud.Insert(insVerifyHist);
		}

		///<summary></summary>
		public static void Delete(long insVerifyHistNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insVerifyHistNum);
				return;
			}
			Crud.InsVerifyHistCrud.Delete(insVerifyHistNum);
		}
		
		///<summary>If the passed in InsVerify is null, do nothing.  Otherwise, insert the passed in InsVerify into InsVerifyHist and blank out InsVerify's UserNum, Status, and Note. jordan It's unclear to me why this is altering/wiping data from the insverify table.  Needs better documentation.</summary>
		public static void InsertFromInsVerify(InsVerify insVerify) {
			if(insVerify==null) {
				return;
			}
			Insert(new InsVerifyHist(insVerify));
			insVerify.UserNum=0;
			insVerify.DefNum=0;
			insVerify.Note="";
			insVerify.DateLastAssigned=DateTime.MinValue;
			if(PrefC.GetBool(PrefName.InsVerifyFutureDateBenefitYear) && insVerify.AppointmentDateTime>DateTime.MinValue) {
				insVerify.DateLastVerified=insVerify.AppointmentDateTime;
			}
			InsVerifies.Update(insVerify);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<InsVerifyHist> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsVerifyHist>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM insverifyhist WHERE PatNum = "+POut.Long(patNum);
			return Crud.InsVerifyHistCrud.SelectMany(command);
		}
		
		*/
	}
}