using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TreatPlanParams{
		#region Methods - Get
		///<summary></summary>
		public static List<TreatPlanParam> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlanParam>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplanparam WHERE PatNum = "+POut.Long(patNum);
			return Crud.TreatPlanParamCrud.SelectMany(command);
		}
		
		///<summary>Gets one TreatPlanParam from the db.</summary>
		public static TreatPlanParam GetOne(long treatPlanParamNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TreatPlanParam>(MethodBase.GetCurrentMethod(),treatPlanParamNum);
			}
			return Crud.TreatPlanParamCrud.SelectOne(treatPlanParamNum);
		}

		///<summary>Gets one TreatPlanParam from the db based on the given TreatPlanNum.</summary>
		public static TreatPlanParam GetOneByTreatPlanNum(long treatPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TreatPlanParam>(MethodBase.GetCurrentMethod(),treatPlanNum);
			}
			string command=$"SELECT * FROM treatplanparam WHERE TreatPlanNum={treatPlanNum}";
			TreatPlanParam treatPlanParam=Crud.TreatPlanParamCrud.SelectOne(command);
			if(treatPlanParam!=null) {
				return treatPlanParam;
			}
			treatPlanParam=new TreatPlanParam();
			treatPlanParam.ShowCompleted=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			treatPlanParam.ShowDiscount=true;
			treatPlanParam.ShowFees=true;
			treatPlanParam.ShowIns=!PrefC.GetBool(PrefName.EasyHideInsurance);
			treatPlanParam.ShowMaxDed=true;
			treatPlanParam.ShowSubTotals=true;
			treatPlanParam.ShowTotals=true;
			return treatPlanParam;
		}
		#endregion Methods - Get
		#region Methods - Modify
		public static long Insert(TreatPlanParam treatPlanParam){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				treatPlanParam.TreatPlanParamNum=Meth.GetLong(MethodBase.GetCurrentMethod(),treatPlanParam);
				return treatPlanParam.TreatPlanParamNum;
			}
			return Crud.TreatPlanParamCrud.Insert(treatPlanParam);
		}

		public static void Update(TreatPlanParam treatPlanParam){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanParam);
				return;
			}
			Crud.TreatPlanParamCrud.Update(treatPlanParam);
		}

		public static void Delete(long treatPlanParamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanParamNum);
				return;
			}
			Crud.TreatPlanParamCrud.Delete(treatPlanParamNum);
		}

		///<summary>Deletes a single TreatPlanParam from the db based on the given TreatPlanNum.</summary>
		public static void DeleteByTreatPlanNum(long treatPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanNum);
				return;
			}
			string command=$"DELETE FROM treatplanparam WHERE TreatPlanNum={POut.Long(treatPlanNum)}";
			Db.NonQ(command);
		}

		///<summary>Deletes all TreatPlanParams from the db that have the given PatNum.</summary>
		public static void RemoveAllByPatNum(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command=$"DELETE FROM treatplanparam WHERE PatNum={POut.Long(patNum)}";
			Db.NonQ(command);
		}
		#endregion Methods - Modify



	}
}