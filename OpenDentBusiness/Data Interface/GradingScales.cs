using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class GradingScales{
		///<summary></summary>
		public static List<GradingScale> RefreshList(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<GradingScale>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM gradingscale ";
			return Crud.GradingScaleCrud.SelectMany(command);
		}

		///<summary>Gets one GradingScale from the db.</summary>
		public static GradingScale GetOne(long gradingScaleNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<GradingScale>(MethodBase.GetCurrentMethod(),gradingScaleNum);
			}
			return Crud.GradingScaleCrud.SelectOne(gradingScaleNum);
		}

		public static bool IsDupicateDescription(GradingScale gradingScaleCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),gradingScaleCur);
			}
			string command="SELECT COUNT(*) FROM gradingscale WHERE Description = '"+POut.String(gradingScaleCur.Description)+"' "
				+"AND GradingScaleNum != "+POut.Long(gradingScaleCur.GradingScaleNum);
			int count=PIn.Int(Db.GetCount(command));
			if(count>0) {
				return true;
			}
			return false;
		}

		public static bool IsInUseByEvaluation(GradingScale gradingScaleCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),gradingScaleCur);
			}
			string command="SELECT COUNT(*) FROM evaluation,evaluationcriterion "
				+"WHERE evaluation.GradingScaleNum = "+POut.Long(gradingScaleCur.GradingScaleNum)+" "
				+"OR evaluationcriterion.GradingScaleNum = "+POut.Long(gradingScaleCur.GradingScaleNum);
			int count=PIn.Int(Db.GetCount(command));
			if(count>0) {
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static long Insert(GradingScale gradingScale){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				gradingScale.GradingScaleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),gradingScale);
				return gradingScale.GradingScaleNum;
			}
			return Crud.GradingScaleCrud.Insert(gradingScale);
		}

		///<summary></summary>
		public static void Update(GradingScale gradingScale){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gradingScale);
				return;
			}
			Crud.GradingScaleCrud.Update(gradingScale);
		}

		///<summary>Also deletes attached GradeScaleItems.  Will throw an error if GradeScale is in use.  Be sure to surround with try-catch.</summary>
		public static void Delete(long gradingScaleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gradingScaleNum);
				return;
			}
			string error="";
			string command="SELECT COUNT(*) FROM evaluationdef WHERE GradingScaleNum="+POut.Long(gradingScaleNum);
			if(Db.GetCount(command)!="0") {
				error+=" EvaluationDef,";
			}
			command="SELECT COUNT(*) FROM evaluationcriteriondef WHERE GradingScaleNum="+POut.Long(gradingScaleNum);
			if(Db.GetCount(command)!="0") {
				error+=" EvaluationCriterionDef,";
			}
			command="SELECT COUNT(*) FROM evaluation WHERE GradingScaleNum="+POut.Long(gradingScaleNum);
			if(Db.GetCount(command)!="0") {
				error+=" Evaluation,";
			}
			command="SELECT COUNT(*) FROM evaluationcriterion WHERE GradingScaleNum="+POut.Long(gradingScaleNum);
			if(Db.GetCount(command)!="0") {
				error+=" EvaluationCriterion,";
			}
			if(error!="") {
				throw new ApplicationException(Lans.g("GradingScaleEdit","Grading scale is in use by")+":"+error.TrimEnd(','));
			}
			GradingScaleItems.DeleteAllByGradingScale(gradingScaleNum);
			command= "DELETE FROM gradingscale WHERE GradingScaleNum = "+POut.Long(gradingScaleNum);
			Db.NonQ(command);
		}
	}
}