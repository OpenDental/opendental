using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using Newtonsoft.Json;
using PdfSharp.Pdf;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileDataBytes{
	private static Random _random=new Random();
		#region Get Methods
		
		///<summary></summary>
		public static bool IsValidUnlockCode(string rawUnlockCode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),rawUnlockCode);
			}
			return (TryGetForUnlockCode(rawUnlockCode)!=null);
		}

		///<summary></summary>
		public static MobileDataByte TryGetForUnlockCode(string rawUnlockCode){
			if(rawUnlockCode.IsNullOrEmpty()){
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MobileDataByte>(MethodBase.GetCurrentMethod(),rawUnlockCode);
			}
			string command=$@"SELECT * FROM mobiledatabyte 
				WHERE RawBase64Code='{Convert.ToBase64String(Encoding.UTF8.GetBytes(rawUnlockCode))}'";
			return Crud.MobileDataByteCrud.SelectOne(command);
		}
		
		///<summary>Gets one MobileDataByte from the db.</summary>
		public static MobileDataByte GetOne(long mobileDataByteNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<MobileDataByte>(MethodBase.GetCurrentMethod(),mobileDataByteNum);
			}
			return Crud.MobileDataByteCrud.SelectOne(mobileDataByteNum);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(MobileDataByte mobileDataByte){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				mobileDataByte.MobileDataByteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileDataByte);
				return mobileDataByte.MobileDataByteNum;
			}
			return Crud.MobileDataByteCrud.Insert(mobileDataByte);
		}
		///<summary></summary>
		public static void Update(MobileDataByte mobileDataByte){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileDataByte);
				return;
			}
			Crud.MobileDataByteCrud.Update(mobileDataByte);
		}
		///<summary></summary>
		public static void Delete(long mobileDataByteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileDataByteNum);
				return;
			}
			Crud.MobileDataByteCrud.Delete(mobileDataByteNum);
		}

		///<summary>Called by eConnector to remove expired rows.</summary>
		public static void ClearExpiredRows(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			Db.NonQ($@"DELETE FROM mobiledatabyte 
				WHERE DateTimeEntry <={POut.Date(DateTime.Today.AddDays(-1))}
				OR DateTimeExpires <= {POut.Date(DateTime.Now)}"
			);
		}

		///<summary>Use this as a reference for how to create PDFs in WebApps (or without a reference to OpenDental).</summary>
		public static long CreateAndInsertTreatmentPlan(long mobileAppDeviceNum,long patNum) {
			Meth.NoCheckMiddleTierRole();
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			//Only saved treatment plans can be sent to the eClipboard, so get only the most recently saved TP.
			List<TreatPlan> listTreatPlans=null;
			TreatPlan treatPlan=null;
			if(listTreatPlans==null || listTreatPlans.Count<1) {
				return -1;
			}
			treatPlan=listTreatPlans[0];
			treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetSheetsDefault(SheetTypeEnum.TreatmentPlan,Clinics.ClinicNum),patNum);
			//These are all of the different sheet parameters that can be added to a treatment plan
			sheet.Parameters.Add(new SheetParameter(true,"TreatPlan") { ParamValue=treatPlan });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowDiscountNotAutomatic") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowDiscount") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowMaxDed") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowSubTotals") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowTotals") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowCompleted") { ParamValue=PrefC.GetBool(PrefName.TreatPlanShowCompleted) });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowFees") { ParamValue=true });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowIns") { ParamValue=!PrefC.GetBool(PrefName.EasyHideInsurance) });
			sheet.Parameters.Add(new SheetParameter(true,"toothChartImg") { ParamValue=SheetFramework.ToothChartHelper.GetImage(patNum,
				PrefC.GetBool(PrefName.TreatPlanShowCompleted),treatPlan) 
			});
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			bool hasPracticeSig=sheet.SheetFields.Any(x => x.FieldType==SheetFieldType.SigBoxPractice);
			PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheet);
			MobileDataByte mobileDataByte=null;
			try {
				mobileDataByte=InsertTreatPlanPDF(pdfDocument,treatPlan,hasPracticeSig,null);
				//new List<string>() { treatPlan.Heading,false.ToString(),treatPlan.DateTP.Ticks.ToString() }
			}
			catch {
				return -1;
			}
			//Update the treatment plan's MobileAppDeviceNum, so we know it is on an eClipboard device
			//treatPlan.MobileAppDeviceNum=mobileAppDeviceNum;
			//TreatPlans.Update(treatPlan);
			if(mobileDataByte==null) {
				return -1;
			}
			return mobileDataByte.MobileDataByteNum;
		}
			
		///<summary>Throws Exceptions. Saves the given doc in the MobileDataByte table. Returns the mobileDataByteNum if no exception occurs.</summary>
		public static long InsertPDF(PdfDocument pdfDocument,long patNum,string unlockCode,eActionType eActionType,List<string> listTagVals=null)
		{
			Meth.NoCheckMiddleTierRole();
			long mobileDataByteNum=-1;
			string byteBase64;
			using MemoryStream memoryStream=new MemoryStream();
			pdfDocument.Save(memoryStream);
			byteBase64=Convert.ToBase64String(memoryStream.ToArray());
			string tagData="";
			if(!listTagVals.IsNullOrEmpty()){
				tagData=Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("###",listTagVals)));
			}
			MobileDataByte mobileDataByte=new MobileDataByte();
			mobileDataByte.PatNum=patNum;
			mobileDataByte.RawBase64Data=byteBase64;
			mobileDataByte.RawBase64Code=(unlockCode.IsNullOrEmpty()?"":Convert.ToBase64String(Encoding.UTF8.GetBytes(unlockCode)));
			mobileDataByte.RawBase64Tag=tagData;
			mobileDataByte.ActionType=eActionType;
			mobileDataByteNum=Insert(mobileDataByte);
			return mobileDataByteNum;
		}

		public static long CreateAndInsertPerioChartPdf(Patient patient,PerioExam perioExam) {
			Meth.NoCheckMiddleTierRole();
			PdfDocument pdfDocument=PerioExams.CreatePerioPDF(patient,perioExam);
			long mobileDataByteNum=0;
			try {
				mobileDataByteNum=InsertPDF(pdfDocument,patient.PatNum,null,eActionType.PerioExam);
			}
			catch {
				//Returning a -1 means that the MobileDataByte couldn't be inserted into the table. 
				return -1;
			}
			return mobileDataByteNum;
		}
		
		///<summary>Throws Exception.
		///Helper method that calls TryInsertPDF(...) for treatment plan PDFs.</summary>
		public static MobileDataByte InsertTreatPlanPDF(PdfDocument pdfDocument,TreatPlan treatPlan,bool hasPracticeSig,string unlockCode){
			Meth.NoCheckMiddleTierRole();
			MobileDataByte mobileDataByte=null;
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(Clinics.ClinicNum)) {
				throw new Exception($"This practice or clinic is not signed up for eClipboard.\r\nGo to eServices | Signup Portal to sign up.");
			}
			//If there is no heading present, than UTF8.GetBytes ignores empty strings, which means it doesn't put an empty header into the list of
			//tags that gets sent over to eClipboard.
			string treatPlanHeading=treatPlan.Heading.IsNullOrEmpty()?"(No heading)":treatPlan.Heading;
			List<string> listTagValues=new List<string>() { treatPlanHeading,treatPlan.TreatPlanNum.ToString(),hasPracticeSig.ToString(),
				treatPlan.DateTP.Ticks.ToString() };
			long mobileDataByteNum=-1;
			mobileDataByteNum=InsertPDF(pdfDocument,treatPlan.PatNum,unlockCode,eActionType.TreatmentPlan,listTagValues);
			if(mobileDataByteNum>-1){
				mobileDataByte=GetOne(mobileDataByteNum);
			}
			return mobileDataByte;
		}

		/// <summary>Throws Exception.
		/// Returns inserted MobileDataByte if no exceptions occur.</summary>
		public static MobileDataByte InsertPatientCheckin(Patient patient,string unlockCode) {
			Meth.NoCheckMiddleTierRole();
			MobileDataByte mobileDataByte = new MobileDataByte();
			mobileDataByte.PatNum = patient.PatNum;
			mobileDataByte.RawBase64Data = patient.PatNum.ToString();
			mobileDataByte.RawBase64Code = (unlockCode.IsNullOrEmpty() ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(unlockCode)));
			mobileDataByte.RawBase64Tag = Convert.ToBase64String(Encoding.UTF8.GetBytes(Security.CurUser.UserNum.ToString()));
			mobileDataByte.ActionType = eActionType.Checkin;
			mobileDataByte.MobileDataByteNum=Insert(mobileDataByte);
			return mobileDataByte;
		}

		///<summary>Code returned will not be in db. 
		///Returns null if no code could be generated in 100 tries that is not already in db.</summary>
		public static string GenerateUnlockCode(){
			Meth.NoCheckMiddleTierRole();
			string input="0123456789";
			string funcMakeUnlockCode() {
				string code=new string(Enumerable.Range(0,6).Select(x => input[_random.Next(0,input.Length)]).ToArray());
				return code;
			}
			string unlockCode=funcMakeUnlockCode();
			int tries=0;
			while(true){
				if(!IsValidUnlockCode(unlockCode)) {
					break;
				}
				if(++tries>100){
					return null;
				}
				unlockCode=funcMakeUnlockCode();
			}
			return unlockCode;
		}

		#endregion Modification Methods
	}
}