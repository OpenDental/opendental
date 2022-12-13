using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using CodeBase;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PerioExams{
		///<summary>Bad pattern. This is public static because it would be hard to pass it into ContrPerio.  Only used by UI.</summary>
		public static List<PerioExam> ListExams;

		///<summary>Most recent date last.  All exams loaded, even if not displayed.</summary>
		public static void Refresh(long patNum) {
			//No need to check MiddleTierRole; no call to db.
			DataTable table=GetExamsTable(patNum);
			ListExams=new List<PerioExam>();
			PerioExam exam;
			for(int i=0;i<table.Rows.Count;i++){
				exam=new PerioExam();
				exam.PerioExamNum= PIn.Long   (table.Rows[i][0].ToString());
				exam.PatNum      = PIn.Long(table.Rows[i][1].ToString());
				exam.ExamDate    = PIn.Date(table.Rows[i][2].ToString());
				exam.ProvNum     = PIn.Long(table.Rows[i][3].ToString());
				exam.DateTMeasureEdit     = PIn.DateT(table.Rows[i][4].ToString());
				exam.Note				 = PIn.String(table.Rows[i][5].ToString());
				ListExams.Add(exam);
			}
			//return list;
			//PerioMeasures.Refresh(patNum);
		}

		public static DataTable GetExamsTable(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * from perioexam"
				+" WHERE PatNum = '"+patNum.ToString()+"'"
				+" ORDER BY perioexam.ExamDate";
			DataTable table=Db.GetTable(command);
			return table;
		}

		public static List<PerioExam> GetExamsList(long patNum) {
			//No need to check MiddleTierRole; no call to db.
			return Crud.PerioExamCrud.TableToList(GetExamsTable(patNum));
		}

		///<summary></summary>
		public static void Update(PerioExam Cur){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.PerioExamCrud.Update(Cur);
		}

		///<summary></summary>
		public static bool Update(PerioExam perioExam,PerioExam perioExamOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),perioExam,perioExamOld);
			}
			return Crud.PerioExamCrud.Update(perioExam,perioExamOld);
		}

		///<summary></summary>
		public static long Insert(PerioExam Cur) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Cur.PerioExamNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.PerioExamNum;
			}
			return Crud.PerioExamCrud.Insert(Cur);
		}

		///<summary>Creates a new perio exam for the given patient. Returns that perio exam. Handles setting default skipped teeth/implants. Does not create a security log entry.</summary>
		public static PerioExam CreateNewExam(Patient pat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PerioExam>(MethodBase.GetCurrentMethod(),pat);
			}
			PerioExam newExam=new PerioExam {
				PatNum=pat.PatNum,
				ExamDate=DateTime.Today,
				ProvNum=pat.PriProv,
				DateTMeasureEdit=MiscData.GetNowDateTime()
			};
			Insert(newExam);
			PerioMeasures.SetSkipped(newExam.PerioExamNum,GetSkippedTeethForExam(pat,newExam));
			return newExam;
		}

		///<summary>Returns the toothNums from 1-32 to skip for the given patient.</summary>
		private static List<int> GetSkippedTeethForExam(Patient pat,PerioExam examCur) {
			List<int> listSkippedTeeth=new List<int>();
			List<PerioExam> listOtherExamsForPat=GetExamsList(pat.PatNum)
				.Where(x => x.PerioExamNum!=examCur.PerioExamNum)
				.OrderBy(x => x.ExamDate)
				.ToList();
			//If any other perio exams exist, we'll use the latest exam for the skipped tooth.
			if(!listOtherExamsForPat.IsNullOrEmpty()) {
				listSkippedTeeth=PerioMeasures.GetSkipped(listOtherExamsForPat.Last().PerioExamNum);
			}
			//For patient's first perio chart, any teeth marked missing are automatically marked skipped.
			else if(PrefC.GetBool(PrefName.PerioSkipMissingTeeth)) {
				//Procedures will only be queried for as needed.
				List<Procedure> listProcs=null;
				foreach(string missingTooth in ToothInitials.GetMissingOrHiddenTeeth(ToothInitials.GetPatientData(pat.PatNum))) {
					if(missingTooth.CompareTo("A")>=0 && missingTooth.CompareTo("Z")<=0) {
						//If is a letter (not a number)
						//Skipped teeth are only recorded by tooth number within the perio exam.
						continue;
					}
					int toothNum=PIn.Int(missingTooth);
					//Check if this tooth has had an implant done AND the office has the preference to SHOW implants
					if(PrefC.GetBool(PrefName.PerioTreatImplantsAsNotMissing)) {
						if(listProcs==null) {
							listProcs=Procedures.Refresh(pat.PatNum);
						}
						if(IsToothImplant(toothNum,listProcs)) {
							//Remove the tooth from the list of skipped teeth if it exists.
							listSkippedTeeth.RemoveAll(x => x==toothNum);
							//We do not want to add it back to the list below.
							continue;
						}
					}
					//This tooth is missing and we know it is not an implant OR the office has the preference to ignore implants.
					//Simply add it to our list of skipped teeth.
					listSkippedTeeth.Add(toothNum);
				}
			}
			return listSkippedTeeth;
		}

		///<summary>Returns true if the toothNum passed in has ever had an implant before. Based on the given patient procedures.</summary>
		private static bool IsToothImplant(int toothNum,List<Procedure> listProcsForPatient) {
			return listProcsForPatient
				.FindAll(x => x.ToothNum==toothNum.ToString() && x.ProcStatus.In(ProcStat.C,ProcStat.EC,ProcStat.EO))
				//ProcedureCodes are cached.
				.Any(x => ProcedureCodes.GetProcCode(x.CodeNum).PaintType==ToothPaintingType.Implant);
		}

		///<summary></summary>
		public static void Delete(PerioExam Cur){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command= "DELETE from perioexam WHERE PerioExamNum = '"+Cur.PerioExamNum.ToString()+"'";
			Db.NonQ(command);
			command= "DELETE from periomeasure WHERE PerioExamNum = '"+Cur.PerioExamNum.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>Used by PerioMeasures when refreshing to organize array.</summary>
		public static int GetExamIndex(List<PerioExam> list,long perioExamNum) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<list.Count;i++) {
				if(list[i].PerioExamNum==perioExamNum) {
					return i;
				}
			}
			//MessageBox.Show("Error. PerioExamNum not in list: "+perioExamNum.ToString());
			return 0;
		}

		///<summary>Used by ContrPerio to get a perio exam.</summary>
		public static PerioExam GetOnePerioExam(long perioExamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PerioExam>(MethodBase.GetCurrentMethod(),perioExamNum);
			}
			return Crud.PerioExamCrud.SelectOne(perioExamNum);
		}

		#region ODXam Methods
		///<summary>Do not use this method in OpenDental.exe. This method is to be used in ODXam only. Creates a PDF of the Perio Chart to 
		///be used in eClipboard.</summary>
		public static PdfDocument CreatePerioPDF(Patient pat,PerioExam perioExam,List<PerioMeasure> listPerioMeasures=null) {
			Rectangle marginBounds=new Rectangle();
			//The GraphicsHelper is used to specifically convert pixels to the correct points that PdfSharp uses, so it will be used in several places
			//The numbers used for width, height, etc, were taken from our printing logic
			marginBounds.X=(int)GraphicsHelper.PixelsToPoints(40);
			marginBounds.Y=(int)GraphicsHelper.PixelsToPoints(60);
			marginBounds.Width=(int)GraphicsHelper.PixelsToPoints(750);
			marginBounds.Height=(int)GraphicsHelper.PixelsToPoints(1000);
			PdfDocument document=new PdfDocument();
			PdfPage page=document.AddPage();
			XGraphics gx=XGraphics.FromPdfPage(page);
			gx.SmoothingMode=XSmoothingMode.HighQuality;
			string clinicName="";
			if(pat.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			//We don't get to make use of our printing/sheet drawing logic here, so we have to manually add the margin bounds
			float y=GraphicsHelper.PixelsToPoints(20)+marginBounds.Y;
			//This code mostly mimics code from FormPerioGraphical. The only realy difference is manually adding the margins and we use PdfSharp's
			//X objects instead (XSize==Size, XFont==Font,XGraphics==Graphics, etc). 
			XSize sizeStr;
			XFont font=new XFont("Arial",15);
			string titleStr="Periodontal Examination";
			sizeStr=gx.MeasureString(titleStr,font);
			gx.DrawString(titleStr,font,Brushes.Black,new PointF(marginBounds.X+marginBounds.Width/2f-(float)sizeStr.Width/2f,y));
			y+=(float)sizeStr.Height;
			//Clinic Name
			font=new XFont("Arial",11);
			sizeStr=gx.MeasureString(clinicName,font);
			gx.DrawString(clinicName,font,Brushes.Black,new PointF(marginBounds.X+marginBounds.Width/2f-(float)sizeStr.Width/2f,y));
			y+=(float)sizeStr.Height;
			//PatientName
			string patNameStr=pat.GetNameFLFormal();
			sizeStr=gx.MeasureString(patNameStr,font);
			gx.DrawString(patNameStr,font,Brushes.Black,new PointF(marginBounds.X+marginBounds.Width/2f-(float)sizeStr.Width/2f,y));
			y+=(float)sizeStr.Height;
			//We put the exam date instead of the current date because the exam date isn't anywhere else on the printout.
			string examDateStr=perioExam.ExamDate.ToShortDateString();//Locale specific exam date.
			sizeStr=gx.MeasureString(examDateStr,font);
			gx.DrawString(examDateStr,font,Brushes.Black,new PointF(marginBounds.X+marginBounds.Width/2f-(float)sizeStr.Width/2f,y));
			y+=(float)sizeStr.Height;
			SizeF sizeAvail=new SizeF(marginBounds.Width,marginBounds.Height-y);
			Bitmap bitmapPerioChart=(Bitmap)SheetFramework.ToothChartHelper.GetImage(pat.PatNum,false,isForPerio:true,perioExam:perioExam);
			XImage xI=XImage.FromGdiPlusImage(bitmapPerioChart);
			float pdfWidth=GraphicsHelper.PixelsToPoints(xI.PixelWidth);
			float pdfHeight=GraphicsHelper.PixelsToPoints(xI.PixelHeight);
			float ratioBitmapToOutput=pdfHeight/(float)sizeAvail.Height;
			if(pdfWidth/pdfHeight //ratio WtoH of bitmap
				> //if bitmap is proportionally wider than page space
				(float)sizeAvail.Width/(float)sizeAvail.Height) //ratio WtoH of page
			{
				ratioBitmapToOutput=pdfWidth/(float)sizeAvail.Width;
			}
			SizeF sizeBitmapOut=new SizeF(pdfWidth/ratioBitmapToOutput,pdfHeight/ratioBitmapToOutput);
			gx.DrawImage(xI,
				x:marginBounds.X+(sizeAvail.Width/2f-sizeBitmapOut.Width/2f),
				y:y,
				width:sizeBitmapOut.Width,
				height:sizeBitmapOut.Height);
			//Dispose of PdfSharp objects
			gx.Dispose();
			gx=null;
			xI.Dispose();
			xI=null;
			if(bitmapPerioChart!=null) {
				bitmapPerioChart.Dispose();
				bitmapPerioChart=null;
			}
			GC.Collect();//We are done creating the pdf so we can forcefully clean up all the objects and controls that were used.
			return document;
		}
		#endregion ODXam Methods
	}
}