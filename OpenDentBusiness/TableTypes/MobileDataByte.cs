using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Table used to send various types of objects as bytes to ODXam applications. Example is a PDF for TxPlan or PaymentPlan. Row gets added here, sent to mobile device, and then consumed. Old rows are ignored.</summary>
	[Serializable()]
	public class MobileDataByte:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileDataByteNum;
		///<summary>The bytes in Base64.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string RawBase64Data;
		///<summary>The unlock code in Base64. Blank if no unlock code required to retrieve.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string RawBase64Code;
		///<summary>Misc data in Base64</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string RawBase64Tag;
		///<summary>Can start out as 0.</summary>
		public long PatNum;
		///<summary>Enum:eActionType Stores the intended action associated to this rows data.</summary>
		public eActionType ActionType;
		///<summary>The DateTime this row was entered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>The DateTime that this row should be removed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeExpires;
	}

	///<summary>Actions representing different endpoints in mobile app.</summary>
	public enum eActionType {
		///<summary>0 - Placeholder</summary>
		None,
		///<summary>1 - Row is associated to a TP pdf to be viewed in eClipboard.</summary>
		TreatmentPlan,
		///<summary>2 - Instructs eClipboard to present patient with payment window.</summary>
		MakePayment,
		///<summary>3 - </summary>
		PaymentPlan,
		///<summary>4 - Associated with a list of perio exams.</summary>
		PerioExam,
		///<summary>5 - Used by eClilpboard to fill out exam sheets.</summary>
		ExamSheet,
		///<summary>6 - Used by eClilpboard to checkin using QR code.</summary>
		Checkin
	}
}
