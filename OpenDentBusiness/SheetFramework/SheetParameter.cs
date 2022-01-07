using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<Summary></Summary>
	[Serializable()]
	public class SheetParameter {
		///<Summary></Summary>
		public bool IsRequired;
		///<Summary>Usually, a columnName.</Summary>
		public string ParamName;
		///<Summary>This is the value which must be set in order to obtain data from the database. It is usually an int primary key.  If running a batch, this may be an array of int.</Summary>
		public object ParamValue;
		
		public SheetParameter Copy(){
			return (SheetParameter)this.MemberwiseClone();
		}	
		
		///<summary>Do not directly use this constructor.</summary>
		public SheetParameter() {
			IsRequired=false;
			ParamName="";
		}

		public SheetParameter(bool isRequired,string paramName) {
			IsRequired=isRequired;
			ParamName=paramName;
		}

		public SheetParameter(bool isRequired,string paramName,string paramValue) {
			IsRequired=isRequired;
			ParamName=paramName;
			ParamValue=paramValue;
		}

		///<Summary>Every sheet has at least one required parameter, usually the primary key of an important table.</Summary>
		public static List<SheetParameter> GetForType(SheetTypeEnum sheetType) {
			List<SheetParameter> list=new List<SheetParameter>();
			switch(sheetType) {
				case SheetTypeEnum.LabelPatient:
					list.Add(new SheetParameter(true,"PatNum"));
					break;
				case SheetTypeEnum.LabelCarrier:
					list.Add(new SheetParameter(true,"CarrierNum"));
					break;
				case SheetTypeEnum.LabelReferral:
					list.Add(new SheetParameter(true,"ReferralNum"));
					break;
				case SheetTypeEnum.ReferralSlip:
					list.Add(new SheetParameter(true,"PatNum"));
					list.Add(new SheetParameter(true,"ReferralNum"));
					break;
				case SheetTypeEnum.LabelAppointment:
					list.Add(new SheetParameter(true,"AptNum"));
					break;
				case SheetTypeEnum.RxInstruction:
				case SheetTypeEnum.Rx:
					list.Add(new SheetParameter(true,"RxNum"));
					break;
				case SheetTypeEnum.Consent:
					list.Add(new SheetParameter(true,"PatNum"));
					break;
				case SheetTypeEnum.PatientLetter:
					list.Add(new SheetParameter(true,"PatNum"));
					list.Add(new SheetParameter(false,"AptNum"));
					break;
				case SheetTypeEnum.ReferralLetter:
					list.Add(new SheetParameter(true,"PatNum"));
					list.Add(new SheetParameter(true,"ReferralNum"));
					list.Add(new SheetParameter(false,"CompletedProcs"));
					list.Add(new SheetParameter(false,"toothChartImg"));
					list.Add(new SheetParameter(false,"AptNum"));
					break;
				case SheetTypeEnum.PatientForm:
					list.Add(new SheetParameter(true,"PatNum"));
					break;
				case SheetTypeEnum.RoutingSlip:
					list.Add(new SheetParameter(true,"AptNum"));
					break;
				case SheetTypeEnum.MedicalHistory:
					list.Add(new SheetParameter(true,"PatNum"));
					break;
				case SheetTypeEnum.LabSlip:
					list.Add(new SheetParameter(true,"PatNum"));
					list.Add(new SheetParameter(true,"LabCaseNum"));
					break;
				case SheetTypeEnum.ExamSheet:
					list.Add(new SheetParameter(true,"PatNum"));
					break;
				case SheetTypeEnum.DepositSlip:
					list.Add(new SheetParameter(true,"DepositNum"));
					break;
				case SheetTypeEnum.Screening:
					list.Add(new SheetParameter(true,"ScreenGroupNum"));
					list.Add(new SheetParameter(false,"PatNum"));
					list.Add(new SheetParameter(false,"ProvNum"));
					break;
				case SheetTypeEnum.PaymentPlan:
					list.Add(new SheetParameter(false,"keyData"));
					break;
				case SheetTypeEnum.RxMulti:
					list.Add(new SheetParameter(true,"ListRxNums"));
					list.Add(new SheetParameter(true,"ListRxSheet"));
					break;
				case SheetTypeEnum.ERA:
					list.Add(new SheetParameter(true,"ERA"));
					list.Add(new SheetParameter(false,"IsSingleClaimPaid"));
					break;
				case SheetTypeEnum.ERAGridHeader:
					list.Add(new SheetParameter(true,"EraClaimPaid"));
					list.Add(new SheetParameter(true,"ClaimIndexNum"));
					break;
				case SheetTypeEnum.TreatmentPlan:
				case SheetTypeEnum.Statement:
				case SheetTypeEnum.MedLabResults:
				default:
					//No required prams for sheet type
					break;
			}
			return list;
		}

		public static void SetParameter(Sheet sheet,string paramName,object paramValue) {
			SheetParameter param=GetParamByName(sheet.Parameters,paramName);
			if(param==null) {
				throw new ApplicationException(Lans.g("Sheet","Parameter not found: ")+paramName);
			}
			param.ParamValue=paramValue;
		}

		public static SheetParameter GetParamByName(List<SheetParameter> parameters,string paramName) {
			foreach(SheetParameter param in parameters) {
				if(param.ParamName==paramName) {
					return param;
				}
			}
			return null;
		}
	}
}
