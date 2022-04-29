using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sparks3D;
using CodeBase;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace OpenDentBusiness.SheetFramework{
	public class ToothChartHelper{
		//there should only be two methods in this class because the Sparks3D.dll might not be present, and we don't want to hit this class unless we have to.
	
		///<summary>Do not call this from OpenDental.exe.  There's a better version there in SheetPrinting.  This generates an Image of the ToothChart.  Set isForSheet=true for dimensions and colors appropriate for Sheets, false for dimensions and colors appropriate for display in Patient Dashboard (matches dimensions for). Set isForWinForms to true if there is an associated window handle. Set isForPerio to true in order to get an image of the Perio Chart.</summary>
		public static Image GetImage(long patNum,bool showCompleted,TreatPlan treatPlan=null,List<Procedure> listProceduresFilteredOverride=null
			,bool isInPatientDashboard=false,bool isForWinForms=false,bool isForPerio=false,PerioExam perioExam=null
			,List<PerioMeasure> listPerioMeasures=null) 
		{
			int colorBackgroundIndex=14;
			int colorTextIndex=15;
			int width=500;
			int height=370;
			if(isForPerio) {
				//The width and height come from FormPerioGraphical in which it excplicitly says not to change width and height of the ToothChartWrapper
				width=649;
				height=696;
			}
			if(isInPatientDashboard) {
				colorBackgroundIndex=10;
				colorTextIndex=11;
				width=410;
				height=307;
			}
			ToothChart toothChart=null;
			//if(ToothChartRelay.IsSparks3DPresent){
				toothChart=new ToothChart(isForWinForms);//Needs to be false, to tell ToothChart that there is no window
				toothChart.Size=new Size(width,height);
			//}
			toothChart.ResetTeeth();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			toothChart.ColorBackgroundMain=listDefs[colorBackgroundIndex].ItemColor;
			toothChart.ColorText=listDefs[colorTextIndex].ItemColor;
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			switch(toothNumberingNomenclature){
				case OpenDentBusiness.ToothNumberingNomenclature.FDI:
					toothChart.SetToothNumberingNomenclature(Sparks3D.ToothNumberingNomenclature.FDI);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Haderup:
					toothChart.SetToothNumberingNomenclature(Sparks3D.ToothNumberingNomenclature.Haderup);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Palmer:
					toothChart.SetToothNumberingNomenclature(Sparks3D.ToothNumberingNomenclature.Palmer);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Universal:
					toothChart.SetToothNumberingNomenclature(Sparks3D.ToothNumberingNomenclature.Universal);
					break;
			}
			List<ToothInitial> toothInitialList=patNum==0?new List<ToothInitial>():ToothInitials.Refresh(patNum);
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<toothInitialList.Count;i++) {
				if(toothInitialList[i].InitialType==ToothInitialType.Primary) {
					toothChart.SetPrimary(toothInitialList[i].ToothNum);
				}
			}
			for(int i=0;i<toothInitialList.Count;i++) {
				switch(toothInitialList[i].InitialType) {
					case ToothInitialType.Missing:
						if(!isForPerio) {//Missing teeth still appear on the perio chart
							toothChart.SetMissing(toothInitialList[i].ToothNum);
						}
						break;
					case ToothInitialType.Hidden:
						if(!isForPerio) {//Hidden teeth still appear on the perio chart
							toothChart.SetHidden(toothInitialList[i].ToothNum);
						}
						break;
					case ToothInitialType.Rotate:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,toothInitialList[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,0,toothInitialList[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,0,0,toothInitialList[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,0,0,0,toothInitialList[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,0,0,0,0,toothInitialList[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						toothChart.MoveTooth(toothInitialList[i].ToothNum,0,0,0,0,0,toothInitialList[i].Movement);
						break;
					case ToothInitialType.Drawing:
						toothChart.AddDrawingSegment(toothInitialList[i].DrawingSegment,toothInitialList[i].ColorDraw);
						break;
					case ToothInitialType.Text:
						toothChart.AddText(toothInitialList[i].GetTextString(), toothInitialList[i].GetTextPoint(), toothInitialList[i].ColorDraw, toothInitialList[i].ToothInitialNum);
						break;
				}
			}
			//We passed in a list of procs we want to see on the toothchart.  Use that.
			List<Procedure> listProceduresFiltered=new List<Procedure>();
			if(listProceduresFilteredOverride!=null) {
				listProceduresFiltered=listProceduresFilteredOverride;
			}
			else if(patNum>0) {//Custom list of procedures was not passed in, go get all for the patient like we always have.
				List<Procedure> listProceduresAll=Procedures.Refresh(patNum);
				listProceduresFiltered=SheetPrinting.FilterProceduresForToothChart(listProceduresAll,treatPlan,showCompleted);
			}
			listProceduresFiltered.Sort(SheetPrinting.CompareProcListFiltered);
			//Draw tooth chart, either normally or in perio mode. This section of code for perio chart mimics code found in FormPerioGraphical.LayoutTeeth().
			if(isForPerio) {
				List<Def> listDefsPerio=Defs.GetDefsForCategory(DefCat.MiscColors,true);
				toothChart.ColorCAL=PrefC.GetColor(PrefName.PerioColorCAL);
				toothChart.ColorFurcations=PrefC.GetColor(PrefName.PerioColorFurcations);
				toothChart.ColorFurcationsRed=PrefC.GetColor(PrefName.PerioColorFurcationsRed);
				toothChart.ColorGM=PrefC.GetColor(PrefName.PerioColorGM);
				toothChart.ColorMGJ=PrefC.GetColor(PrefName.PerioColorMGJ);	
				toothChart.ColorProbing=PrefC.GetColor(PrefName.PerioColorProbing);
				toothChart.ColorProbingRed=PrefC.GetColor(PrefName.PerioColorProbingRed);
				toothChart.ColorSuppuration=listDefsPerio[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
				toothChart.ColorBleeding=listDefsPerio[(int)DefCatMiscColors.PerioBleeding].ItemColor;
				if(listPerioMeasures==null) {
					listPerioMeasures=new List<PerioMeasure>();
					if(perioExam!=null) {
						listPerioMeasures=PerioMeasures.GetAllForExam(perioExam.PerioExamNum);
					}
				}
				for (int i=0;i<listPerioMeasures.Count;i++) {
					if(listPerioMeasures[i].SequenceType==PerioSequenceType.SkipTooth) {
						toothChart.SetMissing(listPerioMeasures[i].IntTooth.ToString());
					} 
					else if(listPerioMeasures[i].SequenceType==PerioSequenceType.Mobility) {
						int mob=listPerioMeasures[i].ToothValue;
						Color color=Color.Black;
						if(mob>=PrefC.GetInt(PrefName.PerioRedMob)) {
							color=Color.Red;
						}
						if(mob!=-1) {//-1 represents no measurement taken.
							toothChart.SetMobility(listPerioMeasures[i].IntTooth.ToString(),mob.ToString(),color);
						}
					} 
					else {
						toothChart.AddPerioMeasure(listPerioMeasures[i].IntTooth,ConvertPerioSequenceType(listPerioMeasures[i].SequenceType),listPerioMeasures[i].MBvalue,
							listPerioMeasures[i].Bvalue,listPerioMeasures[i].DBvalue,listPerioMeasures[i].MLvalue,listPerioMeasures[i].Lvalue,listPerioMeasures[i].DLvalue);
					}
				}
				List<Procedure> _listProcs=Procedures.Refresh(patNum);
				for(int t=1;t<=32;t++){
					List<Procedure> listProcsForTooth=_listProcs.FindAll(x=>x.ToothNum==t.ToString() && ListTools.In(x.ProcStatus,ProcStat.C,ProcStat.EC,ProcStat.EO));
					bool isImplant=false;
					bool isCrown=false;
					for(int p=0;p<listProcsForTooth.Count;p++) {
						ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcsForTooth[p].CodeNum);
						if(procedureCode.PaintType==ToothPaintingType.Implant) {
							isImplant=true;
						}
						if(procedureCode.PaintType==ToothPaintingType.CrownDark || procedureCode.PaintType==ToothPaintingType.CrownLight) {
							isCrown=true;
						}
					}
					if(isImplant){
						toothChart.SetMissing(t.ToString());
						toothChart.SetImplant(t.ToString(),Color.Gainsboro);
						if(isCrown){
							toothChart.SetCrown(t.ToString(),Color.WhiteSmoke);
						}
					}
				}
				//This needs to be called so that the tooth chart is rendered correctly for perio charts
				toothChart.SetPerioMode();
			}
			else {
				DrawProcsGraphics(listProceduresFiltered,toothChart,toothInitialList);
			}
			toothChart.RenderScene();
			Image retVal=toothChart.GetBitmap();
			return retVal;
		}

		public static void DrawProcsGraphics(List<Procedure> procList,ToothChart toothChart,List<ToothInitial> toothInitialList) {
			Procedure proc;
			string[] teeth;
			System.Drawing.Color cLight=System.Drawing.Color.White;
			System.Drawing.Color cDark=System.Drawing.Color.White;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
			for(int i=0;i<procList.Count;i++) {
				proc=procList[i];
				//if(proc.ProcStatus!=procStat) {
				//  continue;
				//}
				if(proc.HideGraphics) {
					continue;
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum).PaintType==ToothPaintingType.Extraction && (
					proc.ProcStatus==ProcStat.C
					|| proc.ProcStatus==ProcStat.EC
					|| proc.ProcStatus==ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor==System.Drawing.Color.FromArgb(0)) {
					switch(proc.ProcStatus) {
						case ProcStat.C:
							cDark=listDefs[1].ItemColor;
							cLight=listDefs[6].ItemColor;
							break;
						case ProcStat.TP:
							cDark=listDefs[0].ItemColor;
							cLight=listDefs[5].ItemColor;
							break;
						case ProcStat.EC:
							cDark=listDefs[2].ItemColor;
							cLight=listDefs[7].ItemColor;
							break;
						case ProcStat.EO:
							cDark=listDefs[3].ItemColor;
							cLight=listDefs[8].ItemColor;
							break;
						case ProcStat.R:
							cDark=listDefs[4].ItemColor;
							cLight=listDefs[9].ItemColor;
							break;
						case ProcStat.Cn:
							cDark=listDefs[16].ItemColor;
							cLight=listDefs[17].ItemColor;
							break;
						case ProcStat.D://Can happen with invalidated locked procs.
						default:
							continue;//Don't draw.
					}
				}
				else {
					cDark=ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor;
					cLight=ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor;
				}
				switch(ProcedureCodes.GetProcCode(proc.CodeNum).PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(toothInitialList,proc.ToothNum)) {
							toothChart.SetPontic(proc.ToothNum,cDark);
						}
						else {
							toothChart.SetCrown(proc.ToothNum,cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(toothInitialList,proc.ToothNum)) {
							toothChart.SetPontic(proc.ToothNum,cLight);
						}
						else {
							toothChart.SetCrown(proc.ToothNum,cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						toothChart.SetCrown(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.CrownLight:
						toothChart.SetCrown(proc.ToothNum,cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(proc.Surf=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(proc.Surf=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=proc.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(toothInitialList,teeth[t])) {
								toothChart.SetPontic(teeth[t],cDark);
							}
							else {
								toothChart.SetCrown(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(proc.Surf=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(proc.Surf=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=proc.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(toothInitialList,teeth[t])) {
								toothChart.SetPontic(teeth[t],cLight);
							}
							else {
								toothChart.SetCrown(teeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						toothChart.SetBigX(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.FillingDark:
						toothChart.SetSurfaceColors(proc.ToothNum,proc.Surf,cDark);
						break;
					case ToothPaintingType.FillingLight:
						toothChart.SetSurfaceColors(proc.ToothNum,proc.Surf,cLight);
						break;
					case ToothPaintingType.Implant:
						toothChart.SetImplant(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.PostBU:
						toothChart.SetBU(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.RCT:
						toothChart.SetRCT(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						toothChart.SetRetainedRoot(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.Sealant:
						toothChart.SetSealant(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Tooth && proc.ToothNum!=""){
							toothChart.SetSpaceMaintainer(proc.ToothNum,cDark);
						}
						else if((ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.ToothRange && proc.ToothRange!="") 
							|| (ProcedureCodes.GetProcCode(proc.CodeNum).AreaAlsoToothRange==true && proc.ToothRange!=""))
						{
							teeth=proc.ToothRange.Split(',');
							for(int t=0;t<teeth.Length;t++) {
								toothChart.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						else if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Quad){
							teeth=new string[0];
							if(proc.Surf=="UR") {
								teeth=new string[] {"4","5","6","7","8"};
							}
							if(proc.Surf=="UL") {
								teeth=new string[] {"9","10","11","12","13"};
							}
							if(proc.Surf=="LL") {
								teeth=new string[] {"20","21","22","23","24"};
							}
							if(proc.Surf=="LR") {
								teeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<teeth.Length;t++) {//could still be length 0
								toothChart.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						toothChart.SetText(proc.ToothNum,cDark,ProcedureCodes.GetProcCode(proc.CodeNum).PaintText);
						break;
					case ToothPaintingType.Veneer:
						toothChart.SetVeneer(proc.ToothNum,cLight);
						break;
				}
			}
		}

		///<summary>This is used internally to convert PerioSequenceType from ODBussiness.PerioSequenceType to Sparks3D.PerioSequenceType.</summary>
		private static Sparks3D.PerioSequenceType ConvertPerioSequenceType(PerioSequenceType sequenceType) {
			//This code mimics code found in Sparks3DInterface.AddPerioMeasure(...)
			switch(sequenceType) {
				default:
					throw new ApplicationException("PerioSequenceType not allowed.");
				case PerioSequenceType.Bleeding:
					return Sparks3D.PerioSequenceType.Bleeding;
				case PerioSequenceType.CAL:
					return Sparks3D.PerioSequenceType.CAL;
				case PerioSequenceType.Furcation:
					return Sparks3D.PerioSequenceType.Furcation;
				case PerioSequenceType.GingMargin:
					return Sparks3D.PerioSequenceType.GingMargin;
				case PerioSequenceType.MGJ:
					return Sparks3D.PerioSequenceType.MGJ;
				case PerioSequenceType.Probing:
					return Sparks3D.PerioSequenceType.Probing;
			}
		}

	}
}
