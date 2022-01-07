using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparks3D;

namespace OpenDentBusiness.SheetFramework{
	public class ToothChartHelper{
		//there should only be two methods in this class because the Sparks3D.dll might not be present, and we don't want to hit this class unless we have to.
	
		///<summary>Do not call this from OpenDental.exe.  There's a better version there in SheetPrinting.  This generates an Image of the ToothChart.  Set isForSheet=true for dimensions and colors appropriate for Sheets, false for dimensions and colors appropriate for display in Patient Dashboard (matches dimensions for). Set isForWinForms to true if there is an associated window handle.</summary>
		public static Image GetImage(long patNum,bool showCompleted,TreatPlan treatPlan=null,List<Procedure> listProceduresFilteredOverride=null
			,bool isInPatientDashboard=false,bool isForWinForms=false) 
		{
			int colorBackgroundIndex=14;
			int colorTextIndex=15;
			int width=500;
			int height=370;
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
						toothChart.SetMissing(toothInitialList[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						toothChart.SetHidden(toothInitialList[i].ToothNum);
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
			//Draw tooth chart
			DrawProcsGraphics(listProceduresFiltered,toothChart,toothInitialList);
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
						else if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.ToothRange && proc.ToothRange!=""){
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




	}
}
