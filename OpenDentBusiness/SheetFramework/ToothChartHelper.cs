using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sparks3D;
using CodeBase;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace OpenDentBusiness.SheetFramework{
	public class ToothChartHelper{
		//there should only be two methods in this class because the Sparks3D.dll might not be present, and we don't want to hit this class unless we have to.
		
		///<summary>Do not call this from OpenDental.exe. There's a better version there in SheetPrinting.  This generates an Image of the ToothChart.  Set isForSheet=true for dimensions and colors appropriate for Sheets, false for dimensions and colors appropriate for display in Patient Dashboard (matches dimensions for). Set isForWinForms to true if there is an associated window handle. Set isForPerio to true in order to get an image of the Perio Chart.</summary>
		public static Image GetImage(long patNum,bool showCompleted,TreatPlan treatPlan=null,List<Procedure> listProceduresFilteredOverride=null
			,bool isInPatientDashboard=false,bool isForWinForms=false,bool isForPerio=false,PerioExam perioExam=null
			,List<PerioMeasure> listPerioMeasures=null,int width=500,int height=370,bool isSmaller=false,bool isForChartModule=false) 
		{
			//Get all the data needed
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			List<ToothInitial> toothInitialList=patNum==0?new List<ToothInitial>():ToothInitials.GetPatientData(patNum);
			//We passed in a list of procs we want to see on the toothchart.  Use that.
			List<Procedure> listProceduresFiltered=new List<Procedure>();
			if(listProceduresFilteredOverride!=null) {
				listProceduresFiltered=listProceduresFilteredOverride;
			}
			else if(patNum>0) {//Custom list of procedures was not passed in, go get all for the patient like we always have.
				List<Procedure> listProceduresAll=Procedures.Refresh(patNum);
				if(!isForChartModule) {
					listProceduresFiltered=SheetPrinting.FilterProceduresForToothChart(listProceduresAll,treatPlan,showCompleted);
				}
				else {
					listProceduresFiltered=listProceduresAll;
				}
			}
			listProceduresFiltered.Sort(SheetPrinting.CompareProcListFiltered);
			if(listPerioMeasures==null) {
				listPerioMeasures=new List<PerioMeasure>();
				if(perioExam!=null) {
					listPerioMeasures=PerioMeasures.GetAllForExam(perioExam.PerioExamNum);
				}
			}
			//Setup perio lists
			Dictionary<PrefName,Color> dictPrefToColor=new Dictionary<PrefName,Color>();
			List<Procedure> listProcsForPerio=new List<Procedure>();
			List<ProcedureCode> listProcedureCodesForPerio=new List<ProcedureCode>();
			List<Def> listDefsPerio=new List<Def>();
			if(isForPerio){
				listDefsPerio=Defs.GetDefsForCategory(DefCat.MiscColors,true);
				dictPrefToColor.Add(PrefName.PerioColorCAL,PrefC.GetColor(PrefName.PerioColorCAL));
				dictPrefToColor.Add(PrefName.PerioColorFurcations,PrefC.GetColor(PrefName.PerioColorFurcations));
				dictPrefToColor.Add(PrefName.PerioColorFurcationsRed,PrefC.GetColor(PrefName.PerioColorFurcationsRed));
				dictPrefToColor.Add(PrefName.PerioColorGM,PrefC.GetColor(PrefName.PerioColorGM));
				dictPrefToColor.Add(PrefName.PerioColorMGJ,PrefC.GetColor(PrefName.PerioColorMGJ));
				dictPrefToColor.Add(PrefName.PerioColorProbing,PrefC.GetColor(PrefName.PerioColorProbing));
				dictPrefToColor.Add(PrefName.PerioColorProbingRed,PrefC.GetColor(PrefName.PerioColorProbingRed));
				listProcsForPerio=Procedures.Refresh(patNum);
				listProcedureCodesForPerio=ProcedureCodes.GetWhere(x=>x.CodeNum.In(listProcsForPerio.Select(x=>x.CodeNum).ToArray()));
			}
			ToothChartHelperData args=GetToothChartData(listDefs,
				listProceduresFiltered,
				toothInitialList,
				toothNumberingNomenclature,
				listPerioMeasures,
				dictPrefToColor,
				listDefsPerio,
				listProcsForPerio,
				listProcedureCodesForPerio,
				isInPatientDashboard,
				isForWinForms,
				isForPerio,
				width,
				height,
				isSmaller,
				isForChartModule);
			//Actually set the stage / draw the tooth image
			return GetToothChartImageProcess(args.ListDefs,args.ListProceduresFiltered,args.ToothInitialList,args.ToothNumberingNomenclature,args.ListPerioMeasures,
				args.DictPrefToColor,args.ListDefsPerio,args.ListProcsForPerio,args.ListProcedureCodesForPerio,args.ListProcedureCodesForProcs,args.IsInPatientDashboard,
				args.IsForWinForms,args.IsForPerio,args.Width,args.Height,args.IsSmaller,args.IsForChartModule);
		}

		///<summary>Do not call this from OpenDental.exe. There's a better version there in SheetPrinting.  This generates an Image of the ToothChart.  Set isForSheet=true for dimensions and colors appropriate for Sheets, false for dimensions and colors appropriate for display in Patient Dashboard (matches dimensions for). Set isForWinForms to true if there is an associated window handle. Set isForPerio to true in order to get an image of the Perio Chart.</summary>
		public static ToothChartHelperData GetDAta(long patNum,bool showCompleted,TreatPlan treatPlan=null,List<Procedure> listProceduresFilteredOverride=null
			,bool isInPatientDashboard=false,bool isForWinForms=false,bool isForPerio=false,PerioExam perioExam=null
			,List<PerioMeasure> listPerioMeasures=null,int width=500,int height=370,bool isSmaller=false,bool isForChartModule=false) 
		{
			//Get all the data needed
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			List<ToothInitial> toothInitialList=patNum==0?new List<ToothInitial>():ToothInitials.GetPatientData(patNum);
			//We passed in a list of procs we want to see on the toothchart.  Use that.
			List<Procedure> listProceduresFiltered=new List<Procedure>();
			if(listProceduresFilteredOverride!=null) {
				listProceduresFiltered=listProceduresFilteredOverride;
			}
			else if(patNum>0) {//Custom list of procedures was not passed in, go get all for the patient like we always have.
				List<Procedure> listProceduresAll=Procedures.Refresh(patNum);
				if(!isForChartModule) {
					listProceduresFiltered=SheetPrinting.FilterProceduresForToothChart(listProceduresAll,treatPlan,showCompleted);
				}
				else {
					listProceduresFiltered=listProceduresAll;
				}
			}
			listProceduresFiltered.Sort(SheetPrinting.CompareProcListFiltered);
			if(listPerioMeasures==null) {
				listPerioMeasures=new List<PerioMeasure>();
				if(perioExam!=null) {
					listPerioMeasures=PerioMeasures.GetAllForExam(perioExam.PerioExamNum);
				}
			}
			//Setup perio lists
			Dictionary<PrefName,Color> dictPrefToColor=new Dictionary<PrefName,Color>();
			List<Procedure> listProcsForPerio=new List<Procedure>();
			List<ProcedureCode> listProcedureCodesForPerio=new List<ProcedureCode>();
			List<Def> listDefsPerio=new List<Def>();
			if(isForPerio){
				listDefsPerio=Defs.GetDefsForCategory(DefCat.MiscColors,true);
				dictPrefToColor.Add(PrefName.PerioColorCAL,PrefC.GetColor(PrefName.PerioColorCAL));
				dictPrefToColor.Add(PrefName.PerioColorFurcations,PrefC.GetColor(PrefName.PerioColorFurcations));
				dictPrefToColor.Add(PrefName.PerioColorFurcationsRed,PrefC.GetColor(PrefName.PerioColorFurcationsRed));
				dictPrefToColor.Add(PrefName.PerioColorGM,PrefC.GetColor(PrefName.PerioColorGM));
				dictPrefToColor.Add(PrefName.PerioColorMGJ,PrefC.GetColor(PrefName.PerioColorMGJ));
				dictPrefToColor.Add(PrefName.PerioColorProbing,PrefC.GetColor(PrefName.PerioColorProbing));
				dictPrefToColor.Add(PrefName.PerioColorProbingRed,PrefC.GetColor(PrefName.PerioColorProbingRed));
				listProcsForPerio=Procedures.Refresh(patNum);
				listProcedureCodesForPerio=ProcedureCodes.GetWhere(x=>x.CodeNum.In(listProcsForPerio.Select(x=>x.CodeNum).ToArray()));
			}
			ToothChartHelperData args=GetToothChartData(listDefs,
				listProceduresFiltered,
				toothInitialList,
				toothNumberingNomenclature,
				listPerioMeasures,
				dictPrefToColor,
				listDefsPerio,
				listProcsForPerio,
				listProcedureCodesForPerio,
				isInPatientDashboard,
				isForWinForms,
				isForPerio,
				width,
				height,
				isSmaller,
				isForChartModule);
			//Actually set the stage / draw the tooth image
			return args;
		}

		public static Image GetToothChartImageProcess(List<Def> ListDefs,List<Procedure> ListProceduresFiltered,List<ToothInitial> ToothInitialList,OpenDentBusiness.ToothNumberingNomenclature ToothNumberingNomenclature,
			List<OpenDentBusiness.PerioMeasure> ListPerioMeasures, Dictionary<PrefName,Color> DictPrefToColor,List<Def> ListDefsPerio,List<Procedure> ListProcsForPerio,List<ProcedureCode> ListProcedureCodesForPerio,
			List<ProcedureCode> ListProcedureCodesForProcs,bool IsInPatientDashboard,bool IsForWinForms,bool IsForPerio,int Width,int Height,bool IsSmaller,bool IsForChartModule){
			int colorBackgroundIndex=14;
			int colorTextIndex=15;
			//determine the width and height for the image based on what it is for (perio, chart, etc)
			if(IsForPerio) {
				//The width and height come from FormPerioGraphical in which it excplicitly says not to change width and height of the ToothChartWrapper
				Width=649;
				Height=696;
			}
			else if(IsInPatientDashboard) {
				colorBackgroundIndex=10;
				colorTextIndex=11;
				Width=410;
				Height=307;
			}
			else if(IsForChartModule) {
				colorBackgroundIndex=10;
				colorTextIndex=11;
				if(Width<500 || Height<370) {
					Width=500;
					Height=370;
				}
				//The scale of the width of the canvas to the bitmap, which could be smaller than the bitmap
				double scaleWidth=Width/500;
				//Same explanation as above, but for the height 
				double scaleHeight=Height/370;
				//Doesn't matter which scale we choose, just needs to be the scaled correctly, so pick that
				double scale=scaleWidth<scaleHeight?scaleWidth:scaleHeight;
				Width=Width*(int)scale;
				Height=Height*(int)scale;
			}
			ToothChart toothChart=null;
			//if(ToothChartRelay.IsSparks3DPresent){
				toothChart=new ToothChart(IsForWinForms);//Needs to be false, to tell ToothChart that there is no window
				toothChart.Size=new Size(Width,Height);
			//}
			toothChart.ResetTeeth();
			toothChart.ColorBackgroundMain=ListDefs[colorBackgroundIndex].ItemColor;
			toothChart.ColorText=ListDefs[colorTextIndex].ItemColor;
			switch(ToothNumberingNomenclature){
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
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<ToothInitialList.Count;i++) {
				if(ToothInitialList[i].InitialType==ToothInitialType.Primary) {
					toothChart.SetPrimary(ToothInitialList[i].ToothNum);
				}
			}
			for(int i=0;i<ToothInitialList.Count;i++) {
				switch(ToothInitialList[i].InitialType) {
					case ToothInitialType.Missing:
						if(!IsForPerio) {//Missing teeth still appear on the perio chart
							toothChart.SetMissing(ToothInitialList[i].ToothNum);
						}
						break;
					case ToothInitialType.Hidden:
						if(!IsForPerio) {//Hidden teeth still appear on the perio chart
							toothChart.SetHidden(ToothInitialList[i].ToothNum);
						}
						break;
					case ToothInitialType.Rotate:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,ToothInitialList[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,0,ToothInitialList[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,0,0,ToothInitialList[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,ToothInitialList[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,ToothInitialList[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						toothChart.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,0,ToothInitialList[i].Movement);
						break;
					case ToothInitialType.Drawing:
						toothChart.AddDrawingSegment(ToothInitialList[i].DrawingSegment,ToothInitialList[i].ColorDraw);
						break;
					case ToothInitialType.Text:
						toothChart.AddText(ToothInitialList[i].GetTextString(), ToothInitialList[i].GetTextPoint(), ToothInitialList[i].ColorDraw, ToothInitialList[i].ToothInitialNum);
						break;
				}
			}
			//Draw tooth chart, either normally or in perio mode. This section of code for perio chart mimics code found in FormPerioGraphical.LayoutTeeth().
			if(IsForPerio) {
				toothChart.ColorCAL=DictPrefToColor[PrefName.PerioColorCAL];//PrefC.GetColor(PrefName.PerioColorCAL);
				toothChart.ColorFurcations=DictPrefToColor[PrefName.PerioColorFurcations];//PrefC.GetColor(PrefName.PerioColorFurcations);
				toothChart.ColorFurcationsRed=DictPrefToColor[PrefName.PerioColorFurcationsRed];//PrefC.GetColor(PrefName.PerioColorFurcationsRed);
				toothChart.ColorGM=DictPrefToColor[PrefName.PerioColorGM];//PrefC.GetColor(PrefName.PerioColorGM);
				toothChart.ColorMGJ=DictPrefToColor[PrefName.PerioColorMGJ];//PrefC.GetColor(PrefName.PerioColorMGJ);	
				toothChart.ColorProbing=DictPrefToColor[PrefName.PerioColorProbing];//PrefC.GetColor(PrefName.PerioColorProbing);
				toothChart.ColorProbingRed=DictPrefToColor[PrefName.PerioColorProbingRed];//PrefC.GetColor(PrefName.PerioColorProbingRed);
				toothChart.ColorSuppuration=ListDefsPerio[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
				toothChart.ColorBleeding=ListDefsPerio[(int)DefCatMiscColors.PerioBleeding].ItemColor;
				for (int i=0;i<ListPerioMeasures.Count;i++) {
					if(ListPerioMeasures[i].SequenceType==OpenDentBusiness.PerioSequenceType.SkipTooth) {
						toothChart.SetMissing(ListPerioMeasures[i].IntTooth.ToString());
					} 
					else if(ListPerioMeasures[i].SequenceType==OpenDentBusiness.PerioSequenceType.Mobility) {
						int mob=ListPerioMeasures[i].ToothValue;
						Color color=Color.Black;
						if(mob>=PrefC.GetInt(PrefName.PerioRedMob)) {
							color=Color.Red;
						}
						if(mob!=-1) {//-1 represents no measurement taken.
							toothChart.SetMobility(ListPerioMeasures[i].IntTooth.ToString(),mob.ToString(),color);
						}
					} 
					else {
						toothChart.AddPerioMeasure(ListPerioMeasures[i].IntTooth,ConvertPerioSequenceType(ListPerioMeasures[i].SequenceType),ListPerioMeasures[i].MBvalue,
							ListPerioMeasures[i].Bvalue,ListPerioMeasures[i].DBvalue,ListPerioMeasures[i].MLvalue,ListPerioMeasures[i].Lvalue,ListPerioMeasures[i].DLvalue);
					}
				}
				for(int t=1;t<=32;t++){
					List<Procedure> listProcsForTooth=ListProcsForPerio.FindAll(x=>x.ToothNum==t.ToString() && (x.ProcStatus==ProcStat.C || x.ProcStatus==ProcStat.EC || x.ProcStatus==ProcStat.EO));
					bool isImplant=false;
					bool isCrown=false;
					for(int p=0;p<listProcsForTooth.Count;p++) {
						ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcsForTooth[p].CodeNum,ListProcedureCodesForPerio); //Passing in list skips db
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
				DrawProcsGraphicsProcess(ListProceduresFiltered,toothChart,ToothInitialList,ListProcedureCodesForProcs,ListDefs);
			}
			toothChart.RenderScene();
			Image retVal=toothChart.GetBitmap();
			return retVal;
		}

		public static void DrawProcsGraphicsProcess(List<Procedure> procList,ToothChart toothChart,List<ToothInitial> toothInitialList,List<ProcedureCode> listProcedureCodes,List<Def> ListDefs) {
			Procedure proc;
			string[] teeth;
			System.Drawing.Color cLight=System.Drawing.Color.White;
			System.Drawing.Color cDark=System.Drawing.Color.White;
			List<Def> listDefs=ListDefs;
			for(int i=0;i<procList.Count;i++) {
				proc=procList[i];
				//if(proc.ProcStatus!=procStat) {
				//  continue;
				//}
				if(proc.HideGraphics) {
					continue;
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).PaintType==ToothPaintingType.Extraction && (
					proc.ProcStatus==ProcStat.C
					|| proc.ProcStatus==ProcStat.EC
					|| proc.ProcStatus==ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).GraphicColor==System.Drawing.Color.FromArgb(0)) {
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
					cDark=ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).GraphicColor;
					cLight=ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).GraphicColor;
				}
				switch(ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).PaintType) {
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
						if(ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).TreatArea==TreatmentArea.Tooth && proc.ToothNum!=""){
							toothChart.SetSpaceMaintainer(proc.ToothNum,cDark);
						}
						else if((ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).TreatArea==TreatmentArea.ToothRange && proc.ToothRange!="") 
							|| (ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).AreaAlsoToothRange==true && proc.ToothRange!=""))
						{
							teeth=proc.ToothRange.Split(',');
							for(int t=0;t<teeth.Length;t++) {
								toothChart.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						else if(ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).TreatArea==TreatmentArea.Quad){
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
						toothChart.SetText(proc.ToothNum,cDark,ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes).PaintText);
						break;
					case ToothPaintingType.Veneer:
						toothChart.SetVeneer(proc.ToothNum,cLight);
						break;
				}

			}

		}

		public static ToothChartHelperData GetToothChartData(List<Def> listDefs,List<Procedure> listProceduresFiltered,List<ToothInitial> toothInitialList,
		ToothNumberingNomenclature toothNumberingNomenclature,List<PerioMeasure> listPerioMeasures, Dictionary<PrefName,Color> dictPrefToColor,List<Def> listDefsPerio,
		List<Procedure> listProcsForPerio,List<ProcedureCode> listProcedureCodesForPerio,
		bool isInPatientDashboard=false,bool isForWinForms=false,bool isForPerio=false,
		int width=500,int height=370,bool isSmaller=false,bool isForChartModule=false) {
			List<ProcedureCode> listProcedureCodesForProcs=ProcedureCodes.GetWhere(x=>x.CodeNum.In(listProceduresFiltered.Select(x=>x.CodeNum).ToArray()));
			ToothChartHelperData toothChartHelperData=new ToothChartHelperData();
			toothChartHelperData.ListDefs=listDefs;
			toothChartHelperData.ListProceduresFiltered=listProceduresFiltered;
			toothChartHelperData.ToothInitialList=toothInitialList;
			toothChartHelperData.ToothNumberingNomenclature=toothNumberingNomenclature;
			toothChartHelperData.ListPerioMeasures=listPerioMeasures;
			toothChartHelperData.DictPrefToColor=dictPrefToColor;
			toothChartHelperData.ListDefsPerio=listDefsPerio;
			toothChartHelperData.ListProcsForPerio=listProcsForPerio;
			toothChartHelperData.ListProcedureCodesForPerio=listProcedureCodesForPerio;
			toothChartHelperData.ListProcedureCodesForProcs=listProcedureCodesForProcs;
			toothChartHelperData.IsInPatientDashboard=isInPatientDashboard;
			toothChartHelperData.IsForWinForms=isForWinForms;
			toothChartHelperData.IsForPerio=isForPerio;
			toothChartHelperData.Width=width;
			toothChartHelperData.Height=height;
			toothChartHelperData.IsSmaller=isSmaller;
			toothChartHelperData.IsForChartModule=isForChartModule;
			return toothChartHelperData;
		}

		///<summary>This is used internally to convert PerioSequenceType from ODBussiness.PerioSequenceType to Sparks3D.PerioSequenceType.</summary>
		private static Sparks3D.PerioSequenceType ConvertPerioSequenceType(PerioSequenceType sequenceType) {
			//This code mimics code found in Sparks3DInterface.AddPerioMeasure(...)
			switch(sequenceType) {
				default:
					throw new ApplicationException("PerioSequenceType not allowed.");
				case PerioSequenceType.BleedSupPlaqCalc:
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
	public class ToothChartHelperData {
 		public List<Def> ListDefs=new List<Def>();
		public List<Procedure> ListProceduresFiltered=new List<Procedure>();
		public List<ToothInitial> ToothInitialList=new List<ToothInitial>();
		public OpenDentBusiness.ToothNumberingNomenclature ToothNumberingNomenclature;
		public List<OpenDentBusiness.PerioMeasure> ListPerioMeasures=new List<OpenDentBusiness.PerioMeasure>();
		public Dictionary<PrefName,Color> DictPrefToColor=new Dictionary<PrefName,Color>();
		public List<Def> ListDefsPerio=new List<Def>();
		public List<Procedure> ListProcsForPerio=new List<Procedure>();
		public List<ProcedureCode> ListProcedureCodesForPerio;
		public List<ProcedureCode> ListProcedureCodesForProcs;
		public bool IsInPatientDashboard=false;
		public bool IsForWinForms=false;
		public bool IsForPerio=false;
		public int Width=500;
		public int Height=370;
		public bool IsSmaller=false;
		public bool IsForChartModule=false;
	}
}
