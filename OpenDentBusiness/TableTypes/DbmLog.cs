using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods = true)]
	public class DbmLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DbmLogNum;
		///<summary>FK to userod.UserNum.  This is the assigned user dbm log.</summary>
		public long UserNum;
		///<summary>Foreign key to any table defined in the DbmLogType Enumeration.</summary>
		public long FKey;
		///<summary>Enum:DbmLogFKeyType The type of log.</summary>
		public DbmLogFKeyType FKeyType;
		///<summary>Enum:DbmLogActionType The type of verification.</summary>
		public DbmLogActionType ActionType;
		///<summary>DateTime the row was added.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>The name of the DBM that created this row.</summary>
		public string MethodName;
		///<summary>The description of exactly what was done.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string LogText;

		public DbmLog() {
			//Need for serialization
		}

		public DbmLog(long userNum,long fKey,DbmLogFKeyType logType,DbmLogActionType actionType,string methodName,string logText) {
			UserNum=userNum;
			LogText=logText;
			FKey=fKey;
			FKeyType=logType;
			ActionType=actionType;
			MethodName=methodName;
			LogText=logText;
		}
		///<summary></summary>
		public DbmLog Copy() {
			return (DbmLog)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum DbmLogFKeyType {
		///<summary>This means FKey should be 0.</summary>
		None,
		///<summary>This means FKey will link to AptNum.</summary>
		Appointment,
		///<summary>This means FKey will link to AutoCodeNum.</summary>
		AutoCode,
		///<summary>This means FKey will link to AutomationNum.</summary>
		Automation,
		///<summary>This means FKey will link to BenefitNum.</summary>
		Benefit,
		///<summary>This means FKey will link to CarrierNum.</summary>
		Carrier,
		///<summary>This means FKey will link to ClaimNum.</summary>
		Claim,
		///<summary>This means FKey will link to ClaimPaymentNum.</summary>
		ClaimPayment,
		///<summary>This means FKey will link to ClaimProcNum.</summary>
		ClaimProc,
		///<summary>This means FKey will link to ClinicNum.</summary>
		Clinic,
		///<summary>This means FKey will link to CreditCardNum.</summary>
		CreditCard,
		///<summary>This means FKey will link to EtransNum.</summary>
		Etrans,
		///<summary>This means FKey will link to FeeNum.</summary>
		Fee,
		///<summary>This means FKey will link to FeeSchedNum.</summary>
		FeeSched,
		///<summary>This means FKey will link to PlanNum.</summary>
		InsPlan,
		///<summary>This means FKey will link to InsSubNum.</summary>
		InsSub,
		///<summary>This means FKey will link to PatPlanNum.</summary>
		PatPlan,
		///<summary>This means FKey will link to PatNum.</summary>
		Patient,
		///<summary>This means FKey will link to PayNum.</summary>
		Payment,
		///<summary>This means FKey will link to PayPlanNum.</summary>
		PayPlan,
		///<summary>This means FKey will link to PayPlanChargeNum.</summary>
		PayPlanCharge,
		///<summary>This means FKey will link to PaySpliteNum.</summary>
		PaySplit,
		///<summary>This means FKey will link to PlannedApptNum.</summary>
		PlannedAppt,
		///<summary>This means FKey will link to ProcNum.</summary>
		Procedure,
		///<summary>This means FKey will link to SecurityLogNum.</summary>
		Securitylog,
		///<summary>This means FKey will link to HistApptNum.</summary>
		HistAppointment,
		///<summary>This means FKey will link to CodeNum.</summary>
		ProcedureCode,
		///<summary>This means FKey will link to ToothInitialNum</summary>
		ToothInitial,
	}

	///<summary></summary>
	public enum DbmLogActionType {
		///<summary>0. This means the action done was an Insert.</summary>
		Insert,
		///<summary>1. This means the action done was an Update</summary>
		Update,
		///<summary>2. This means the action done was a Delete</summary>
		Delete,
	}
}
