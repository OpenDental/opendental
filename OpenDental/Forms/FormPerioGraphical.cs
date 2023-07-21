using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using SparksToothChart;

namespace OpenDental {
	public partial class FormPerioGraphical:FormODBase {
		///<summary>The current perio exam being loaded.</summary>
		private PerioExam _perioExam;
		///<summary>The current patient for the loaded perio exam</summary>
		private Patient _patient;
		//public List<PerioMeasure> ListPerioMeasures; 
		private ToothChartRelay _toothChartRelay;
		///<summary>This is the Sparks3D control.</summary>
		private Control toothChart;

		public FormPerioGraphical(PerioExam perioExam,Patient patient) {
			_perioExam=perioExam;
			_patient=patient;
			InitializeComponent();
			InitializeLayoutManager();
			this.DoubleBuffered=true;
		}

		private void FormPerioGraphic_Load(object sender,EventArgs e) {
			LayoutTeeth();
		}

		private void FormPerioGraphical_ResizeEnd(object sender, EventArgs e){
			//LayoutTeeth();
			//Invalidate();
		}

		private void LayoutTeeth(){
			_toothChartRelay= new ToothChartRelay();
			_toothChartRelay.SetToothChartWrapper(toothChartWrapper);
			if(ToothChartRelay.IsSparks3DPresent){
				toothChartWrapper.Visible=false;
				toothChart=_toothChartRelay.GetToothChart();
				toothChart.Location=toothChartWrapper.Location;
				toothChart.Size=toothChartWrapper.Size;
				toothChart.Anchor=AnchorStyles.Top|AnchorStyles.Left;	
				toothChart.Visible=true;
				LayoutManager.Add(toothChart,this);
				toothChart.BringToFront();
			}
			else{
				toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);//Must be set before draw mode
				toothChartWrapper.DrawMode=DrawingMode.DirectX;
			}
			_toothChartRelay.BeginUpdate();
			_toothChartRelay.SetToothNumberingNomenclature((ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			_toothChartRelay.ColorBackgroundMain=Color.White;
			_toothChartRelay.ColorText=Color.Black;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.MiscColors,true);
			_toothChartRelay.SetPerioColors(
				listDefs[(int)DefCatMiscColors.PerioBleeding].ItemColor,
				listDefs[(int)DefCatMiscColors.PerioSuppuration].ItemColor,
				PrefC.GetColor(PrefName.PerioColorProbing),
				PrefC.GetColor(PrefName.PerioColorProbingRed),
				PrefC.GetColor(PrefName.PerioColorGM),
				PrefC.GetColor(PrefName.PerioColorCAL),
				PrefC.GetColor(PrefName.PerioColorMGJ),
				PrefC.GetColor(PrefName.PerioColorFurcations),
				PrefC.GetColor(PrefName.PerioColorFurcationsRed),
				PrefC.GetInt(PrefName.PerioRedProb),
				PrefC.GetInt(PrefName.PerioRedFurc)
			);
			_toothChartRelay.ResetTeeth();
			_toothChartRelay.SetOrthoMode(false);
			try {
				_toothChartRelay.IsPerioMode=true;
			}
			catch(Exception ex) {//catch is just for old ToothChartWrapper
				MsgBox.Show(this,ex.Message);
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			List<PerioMeasure> listPerioMeasures=PerioMeasures.GetAllForExam(_perioExam.PerioExamNum);
			#region CAL old
			if (!ToothChartRelay.IsSparks3DPresent){
				//compute CAL's for each site.  If a CAL is valid, pass it in.
				PerioMeasure perioMeasureProbe;
				PerioMeasure perioMeasureGM;
				int gm;
				int pd;
				int calMB;
				int calB;
				int calDB;
				int calML;
				int calL;
				int calDL;
				for(int t=1;t<=32;t++) {
					perioMeasureProbe=null;
					perioMeasureGM=null;
					for(int i=0;i<listPerioMeasures.Count;i++) {
						if(listPerioMeasures[i].IntTooth!=t) {
							continue;
						}
						if(listPerioMeasures[i].SequenceType==PerioSequenceType.Probing) {
							perioMeasureProbe=listPerioMeasures[i];
						}
						if(listPerioMeasures[i].SequenceType==PerioSequenceType.GingMargin) {
							perioMeasureGM=listPerioMeasures[i];
						}
					}
					if(perioMeasureProbe==null||perioMeasureGM==null) {
						continue;//to the next tooth
					}
					//mb
					calMB=-1;
					gm=perioMeasureGM.MBvalue;//MBvalue must stay over 100 for hyperplasia, because that's how we're storing it in ToothChartData.ListPerioMeasure.
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3
					}
					pd=perioMeasureProbe.MBvalue;
					if(perioMeasureGM.MBvalue!=-1 && pd!=-1) {
						calMB=gm+pd;
						if(calMB<0) {
							calMB=0;//CALs can't be negative
						}
					}
					//B
					calB=-1;
					gm=perioMeasureGM.Bvalue;
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3 
					}
					pd=perioMeasureProbe.Bvalue;
					if(perioMeasureGM.Bvalue!=-1&&pd!=-1) {
						calB=gm+pd;
						if(calB<0) {
							calB=0;
						}
					}
					//DB
					calDB=-1;
					gm=perioMeasureGM.DBvalue;
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3 
					}
					pd=perioMeasureProbe.DBvalue;
					if(perioMeasureGM.DBvalue!=-1&&pd!=-1) {
						calDB=gm+pd;
						if(calDB<0) {
							calDB=0;
						}
					}
					//ML
					calML=-1;
					gm=perioMeasureGM.MLvalue;
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3 
					}
					pd=perioMeasureProbe.MLvalue;
					if(perioMeasureGM.MLvalue!=-1&&pd!=-1) {
						calML=gm+pd;
						if(calML<0) {
							calML=0;
						}
					}
					//L
					calL=-1;
					gm=perioMeasureGM.Lvalue;
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3 
					}
					pd=perioMeasureProbe.Lvalue;
					if(perioMeasureGM.Lvalue!=-1&&pd!=-1) {
						calL=gm+pd;
						if(calL<0) {
							calL=0;
						}
					}
					//DL
					calDL=-1;
					gm=perioMeasureGM.DLvalue;
					if(gm>100) {//hyperplasia
						gm=100-gm;//e.g. 100-103=-3 
					}
					pd=perioMeasureProbe.DLvalue;
					if(perioMeasureGM.DLvalue!=-1&&pd!=-1) {
						calDL=gm+pd;
						if(calDL<0) {
							calDL=0;
						}
					}
					if(calMB!=-1||calB!=-1||calDB!=-1||calML!=-1||calL!=-1||calDL!=-1){
						_toothChartRelay.AddPerioMeasure(t,PerioSequenceType.CAL,calMB,calB,calDB,calML,calL,calDL);
					}
				}
			}
			#endregion CAL old
			for (int i=0;i<listPerioMeasures.Count;i++) {
				if(listPerioMeasures[i].SequenceType==PerioSequenceType.SkipTooth) {
					_toothChartRelay.SetMissing(listPerioMeasures[i].IntTooth.ToString());
				} 
				else if(listPerioMeasures[i].SequenceType==PerioSequenceType.Mobility) {
					int mob=listPerioMeasures[i].ToothValue;
					Color color=Color.Black;
					if(mob>=PrefC.GetInt(PrefName.PerioRedMob)) {
						color=Color.Red;
					}
					if(mob!=-1) {//-1 represents no measurement taken.
						_toothChartRelay.SetMobility(listPerioMeasures[i].IntTooth.ToString(),mob.ToString(),color);
					}
				} 
				else {
					_toothChartRelay.AddPerioMeasure(listPerioMeasures[i].IntTooth,listPerioMeasures[i].SequenceType,listPerioMeasures[i].MBvalue,listPerioMeasures[i].Bvalue,listPerioMeasures[i].DBvalue,
						listPerioMeasures[i].MLvalue,listPerioMeasures[i].Lvalue,listPerioMeasures[i].DLvalue);
				}
			}
			//if(ToothChartRelay.IsSparks3DPresent){
			List<Procedure> listProcedures=Procedures.Refresh(FormOpenDental.PatNumCur);
			for(int t=1;t<=32;t++){
				List<Procedure> listProceduresForTooth=listProcedures.FindAll(x => x.ToothNum==t.ToString() && x.ProcStatus.In(ProcStat.C,ProcStat.EC,ProcStat.EO));
				bool isImplant=false;
				bool isCrown=false;
				for(int p=0;p<listProceduresForTooth.Count;p++) {
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresForTooth[p].CodeNum);
					if(procedureCode.PaintType==ToothPaintingType.Implant) {
						isImplant=true;
					}
					if(procedureCode.PaintType==ToothPaintingType.CrownDark || procedureCode.PaintType==ToothPaintingType.CrownLight) {
						isCrown=true;
					}
				}
				if(!isImplant){
					continue;
				}
				_toothChartRelay.SetMissing(t.ToString());
				_toothChartRelay.SetImplant(t.ToString(),Color.Gainsboro);
				if(isCrown){
					_toothChartRelay.SetCrown(t.ToString(),Color.WhiteSmoke);
				}
			}
			_toothChartRelay.EndUpdate();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugClassicPreview(pd2_PrintPage,
				Lan.g(this,"Graphical perio chart printed"),
				auditPatNum:_patient.PatNum,
				printSituation:PrintSituation.TPPerio,
				margins:new Margins(50,50,50,50),
				printoutOrigin:PrintoutOrigin.AtMargin);
		}

		private void pd2_PrintPage(object sender,PrintPageEventArgs ev) {//raised for each page to be printed.
			Graphics g=ev.Graphics;
			RenderPerioPrintout(g,_patient,ev.MarginBounds);//,new Rectangle(0,50,ev.MarginBounds.Width,ev.MarginBounds.Height));
		}

		public void RenderPerioPrintout(Graphics g,Patient patient,Rectangle rectangleMarginBounds) {
			string clinicName="";
			//This clinic name could be more accurate here in the future if we make perio exams clinic specific.
			//Perhaps if there were a perioexam.ClinicNum column.
			if(patient.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(patient.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			float y=20;
			SizeF sizeF;
			using Font fontTitle=new Font("Arial",15);
			string titleStr=Lan.g(this,"Periodontal Examination");
			sizeF=g.MeasureString(titleStr,fontTitle);
			g.DrawString(titleStr,fontTitle,Brushes.Black,new PointF(rectangleMarginBounds.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//Clinic Name
			using Font font=new Font("Arial",11);
			sizeF=g.MeasureString(clinicName,font);
			g.DrawString(clinicName,font,Brushes.Black,new PointF(rectangleMarginBounds.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//PatientName
			string patNameStr=_patient.GetNameFLFormal();
			sizeF=g.MeasureString(patNameStr,font);
			g.DrawString(patNameStr,font,Brushes.Black,new PointF(rectangleMarginBounds.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//We put the exam date instead of the current date because the exam date isn't anywhere else on the printout.
			string examDateStr=_perioExam.ExamDate.ToShortDateString();//Locale specific exam date.
			sizeF=g.MeasureString(examDateStr,font);
			g.DrawString(examDateStr,font,Brushes.Black,new PointF(rectangleMarginBounds.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			SizeF sizeFAvailable=new SizeF(rectangleMarginBounds.Width,rectangleMarginBounds.Height-y);
			using Bitmap bitmapToothChart=_toothChartRelay.GetBitmap();
			float ratioBitmapToOutput=(float)bitmapToothChart.Height/(float)sizeFAvailable.Height;
			if((float)bitmapToothChart.Width/(float)bitmapToothChart.Height //ratio WtoH of bitmap
				> //if bitmap is proportionally wider than page space
				(float)sizeFAvailable.Width/(float)sizeFAvailable.Height) //ratio WtoH of page
			{
				ratioBitmapToOutput=(float)bitmapToothChart.Width/(float)sizeFAvailable.Width;
			}
			SizeF sizeFBitmapToOut=new SizeF((float)bitmapToothChart.Width/ratioBitmapToOutput,(float)bitmapToothChart.Height/ratioBitmapToOutput);
			g.DrawImage(bitmapToothChart,
				x:sizeFAvailable.Width/2f-sizeFBitmapToOut.Width/2f,
				y:y,
				width:sizeFBitmapToOut.Width,
				height:sizeFBitmapToOut.Height);
		}

		private void butSetup_Click(object sender,EventArgs e) {
			using FormPerioGraphicalSetup formPerioGraphicalSetup=new FormPerioGraphicalSetup();
			if(formPerioGraphicalSetup.ShowDialog()==DialogResult.OK){
				toothChartWrapper.ColorCAL=PrefC.GetColor(PrefName.PerioColorCAL);
				toothChartWrapper.ColorFurcations=PrefC.GetColor(PrefName.PerioColorFurcations);
				toothChartWrapper.ColorFurcationsRed=PrefC.GetColor(PrefName.PerioColorFurcationsRed);
				toothChartWrapper.ColorGingivalMargin=PrefC.GetColor(PrefName.PerioColorGM);
				toothChartWrapper.ColorMGJ=PrefC.GetColor(PrefName.PerioColorMGJ);	
				toothChartWrapper.ColorProbing=PrefC.GetColor(PrefName.PerioColorProbing);
				toothChartWrapper.ColorProbingRed=PrefC.GetColor(PrefName.PerioColorProbingRed);
				this.toothChartWrapper.Invalidate();
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			long defNumCategory=Defs.GetImageCat(ImageCategorySpecial.T);
			if(defNumCategory==0) {
				MsgBox.Show(this,"In Setup, Definitions, Image Categories, a category needs to be set for graphical tooth charts.");
				return;
			}
			Document document=new Document();
			using Bitmap bitmap=new Bitmap(750,1000);
			using Graphics g=Graphics.FromImage(bitmap);
			g.Clear(Color.White);
			g.CompositingQuality=System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			RenderPerioPrintout(g,_patient,new Rectangle(0,0,bitmap.Width,bitmap.Height));
			try {
				ImageStore.Import(bitmap,defNumCategory,ImageType.Photo,_patient);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save file: ") + ex.Message);
				return;
			}
			MsgBox.Show(this,"Saved.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormPerioGraphical_FormClosing(object sender,FormClosingEventArgs e) {
			//We need to clear out the tooth graphics of the local toothchart, since they are shallow copies of the tooth chart in the Chart module.
			//Otherwise, when the form disposes, the Chart module tooth graphics would also be disposed.
			toothChartWrapper.TcData.ListToothGraphics.Clear();
		}

	
	}
}
