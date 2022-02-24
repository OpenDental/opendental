using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EvaluationCriterions{
		///<summary>Get all Criterion attached to an Evaluation.</summary>
		public static List<EvaluationCriterion> Refresh(long evaluationNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EvaluationCriterion>>(MethodBase.GetCurrentMethod(),evaluationNum);
			}
			string command="SELECT * FROM evaluationcriterion WHERE EvaluationNum = "+POut.Long(evaluationNum);
			return Crud.EvaluationCriterionCrud.SelectMany(command);
		}

		///<summary>Gets one EvaluationCriterion from the db.</summary>
		public static EvaluationCriterion GetOne(long evaluationCriterionNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EvaluationCriterion>(MethodBase.GetCurrentMethod(),evaluationCriterionNum);
			}
			return Crud.EvaluationCriterionCrud.SelectOne(evaluationCriterionNum);
		}

		///<summary></summary>
		public static long Insert(EvaluationCriterion evaluationCriterion){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				evaluationCriterion.EvaluationCriterionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),evaluationCriterion);
				return evaluationCriterion.EvaluationCriterionNum;
			}
			return Crud.EvaluationCriterionCrud.Insert(evaluationCriterion);
		}

		///<summary></summary>
		public static void Update(EvaluationCriterion evaluationCriterion){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluationCriterion);
				return;
			}
			Crud.EvaluationCriterionCrud.Update(evaluationCriterion);
		}

		///<summary></summary>
		public static void Delete(long evaluationCriterionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluationCriterionNum);
				return;
			}
			string command= "DELETE FROM evaluationcriterion WHERE EvaluationCriterionNum = "+POut.Long(evaluationCriterionNum);
			Db.NonQ(command);
		}
	}
}