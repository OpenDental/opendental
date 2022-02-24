using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InstallmentPlans{
		#region Get Methods
		///<summary>Gets the installment plan for this family.  If none, returns null.</summary>
		public static InstallmentPlan GetOneForFam(long guarNum) {
			//No need to check RemotingRole; no call to db.
			InstallmentPlan installPlan;
			if(GetForFams(new List<long> { guarNum }).TryGetValue(guarNum,out installPlan)) {
				return installPlan;
			}
			return null;
		}

		///<summary>Gets the installment plans for these families.  If there are none for a family, the guarantor will not be present in the dictionary.
		///</summary>
		///<returns>Dictionary where the key is the guarantor num and the value is the installment plan for the family.</returns>
		public static SerializableDictionary<long,InstallmentPlan> GetForFams(List<long> listGuarNums) {
			if(listGuarNums.Count==0) {
				return new SerializableDictionary<long,InstallmentPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,InstallmentPlan>>(MethodBase.GetCurrentMethod(),listGuarNums);
			}
			string command="SELECT * FROM installmentplan WHERE PatNum IN("+string.Join(",",listGuarNums.Select(x => POut.Long(x)))+") ";
			return Crud.InstallmentPlanCrud.SelectMany(command)
				.GroupBy(x => x.PatNum)
				.ToSerializableDictionary(x => x.Key,y => y.First());//Only returning one installment plan per family.
		}

		///<summary>Gets the installment plans for a SuperFamily.  If none, returns empty list.</summary>
		public static List<InstallmentPlan> GetForSuperFam(long superFamNum) {
			//No need to check RemotingRole; no call to db.
			List<InstallmentPlan> listPlans;
			if(GetForSuperFams(new List<long> { superFamNum }).TryGetValue(superFamNum,out listPlans)) {
				return listPlans;
			}
			return new List<InstallmentPlan>();
		}

		///<summary>Gets the installment plans for these  super families.  If there are none for a super family, the super family head will not be 
		///present in the dictionary.</summary>
		///<returns>Dictionary where the key is the super family head and the value is the installment plan for the super family.</returns>
		public static SerializableDictionary<long,List<InstallmentPlan>> GetForSuperFams(List<long> listSuperFamNums) {
			if(listSuperFamNums.Count==0) {
				return new SerializableDictionary<long,List<InstallmentPlan>>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,List<InstallmentPlan>>>(MethodBase.GetCurrentMethod(),listSuperFamNums);
			}
			string command="SELECT installmentplan.*,patient.SuperFamily FROM installmentplan "
				+"INNER JOIN patient ON installmentplan.PatNum=patient.PatNum "
				+"WHERE patient.SuperFamily IN("+string.Join(",",listSuperFamNums.Select(x => POut.Long(x)))+") "
				+"AND patient.HasSuperBilling=1 "
				+"GROUP BY installmentplan.PatNum";
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				command+=",installmentplan.InstallmentPlanNum,installmentplan.DateAgreement,installmentplan.DateFirstPayment"
					+",installmentplan.MonthlyPayment,installmentplan.APR,installmentplan.Note,patient.SuperFamily";
			}
			DataTable table=Db.GetTable(command);
			List<InstallmentPlan> listInstallmentPlans=Crud.InstallmentPlanCrud.TableToList(table);
			SerializableDictionary<long,List<InstallmentPlan>> dictPlans=new SerializableDictionary<long,List<InstallmentPlan>>();
			for(int i = 0;i<table.Rows.Count;i++) {
				long superFamNum=PIn.Long(table.Rows[i]["SuperFamily"].ToString());
				if(!dictPlans.ContainsKey(superFamNum)) {
					dictPlans.Add(superFamNum,new List<InstallmentPlan>());
				}
				dictPlans[superFamNum].Add(listInstallmentPlans[i]);
			}
			return dictPlans;
		}

		#endregion

		///<summary>Gets one InstallmentPlan from the db.</summary>
		public static InstallmentPlan GetOne(long installmentPlanNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<InstallmentPlan>(MethodBase.GetCurrentMethod(),installmentPlanNum);
			}
			return Crud.InstallmentPlanCrud.SelectOne(installmentPlanNum);
		}

		///<summary></summary>
		public static long Insert(InstallmentPlan installmentPlan){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				installmentPlan.InstallmentPlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),installmentPlan);
				return installmentPlan.InstallmentPlanNum;
			}
			return Crud.InstallmentPlanCrud.Insert(installmentPlan);
		}

		///<summary></summary>
		public static void Update(InstallmentPlan installmentPlan){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),installmentPlan);
				return;
			}
			Crud.InstallmentPlanCrud.Update(installmentPlan);
		}

		///<summary></summary>
		public static void Delete(long installmentPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),installmentPlanNum);
				return;
			}
			string command= "DELETE FROM installmentplan WHERE InstallmentPlanNum = "+POut.Long(installmentPlanNum);
			Db.NonQ(command);
		}
	}
}