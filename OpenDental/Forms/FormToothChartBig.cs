using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using SparksToothChart;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormToothChartingBig:FormODBase {
		private bool ShowBySelectedTeeth;
		private List<ToothInitial> ToothInitialList;
		private ToothChartRelay _toothChartRelay;
		///<summary>This is the new Sparks3D toothChart.</summary>
		private Control toothChart;
		private List<DataRow> ProcList;
		public List<Appointment> ListAppts;
		public Patient PatCur;

		///<summary></summary>
		public FormToothChartingBig(bool showBySelectedTeeth,List<ToothInitial> toothInitialList,List<DataRow> procList)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ShowBySelectedTeeth=showBySelectedTeeth;
			ToothInitialList=toothInitialList;
			ProcList=procList;
		}

		private void FormToothChartingBig_Load(object sender,EventArgs e) {
			this.Text=Lan.g(this,"Big Tooth Chart")+" - "+PatCur.GetNameLF()+" - "+PatCur.PatNum;
			_toothChartRelay= new ToothChartRelay();
			_toothChartRelay.SetToothChartWrapper(toothChartWrapper);
			if(ToothChartRelay.IsSparks3DPresent){
				toothChartWrapper.Visible=false;//already not visible
				toothChart=_toothChartRelay.GetToothChart();
				toothChart.Dock = System.Windows.Forms.DockStyle.Fill;
				toothChart.Location=toothChartWrapper.Location;
				toothChart.Size=toothChartWrapper.Size;
				LayoutManager.Add(toothChart,this);
			}
			else{
				toothChartWrapper.Visible=true;
				//ComputerPref computerPref=ComputerPrefs.GetForLocalComputer();
				toothChartWrapper.UseHardware=ComputerPrefs.LocalComputer.GraphicsUseHardware;
				toothChartWrapper.PreferredPixelFormatNumber=ComputerPrefs.LocalComputer.PreferredPixelFormatNum;
				//Must be last preference set, last so that all settings are caried through in the reinitialization this line triggers.
				if(ComputerPrefs.LocalComputer.GraphicsSimple==DrawingMode.Simple2D) {
					toothChartWrapper.DrawMode=DrawingMode.Simple2D;
				}
				else if(ComputerPrefs.LocalComputer.GraphicsSimple==DrawingMode.DirectX) {
					toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
					toothChartWrapper.DrawMode=DrawingMode.DirectX;
				}
				else{
					toothChartWrapper.DrawMode=DrawingMode.OpenGL;
				}
				//The preferred pixel format number changes to the selected pixel format number after a context is chosen.
				ComputerPrefs.LocalComputer.PreferredPixelFormatNum=toothChartWrapper.PreferredPixelFormatNumber;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			FillToothChart();
			//toothChart.Refresh();
		}

		private void FormToothChartingBig_ResizeEnd(object sender,EventArgs e) {
			FillToothChart();
			//toothChart.Refresh();
		}

		///<summary>This is, of course, called when module refreshed.  But it's also called when user sets missing teeth or tooth movements.  In that case, the Progress notes are not refreshed, so it's a little faster.  This also fills in the movement amounts.</summary>
		private void FillToothChart(){
			Cursor=Cursors.WaitCursor;
			_toothChartRelay.BeginUpdate();
			_toothChartRelay.SetToothNumberingNomenclature((ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			_toothChartRelay.ColorBackgroundMain=listDefs[10].ItemColor;
			_toothChartRelay.ColorText=listDefs[11].ItemColor;
			_toothChartRelay.ColorTextHighlightFore=listDefs[12].ItemColor;
			_toothChartRelay.ColorTextHighlightBack=listDefs[13].ItemColor;
			//remember which teeth were selected
			List<string> selectedTeeth=new List<string>(toothChartWrapper.SelectedTeeth);
			//ArrayList selectedTeeth=new ArrayList();//integers 1-32
			//for(int i=0;i<toothChart.SelectedTeeth.Length;i++) {
			//	selectedTeeth.Add(Tooth.ToInt(toothChart.SelectedTeeth[i]));
			//}
			_toothChartRelay.ResetTeeth();
			//if(PatCur==null) {
				//toothChart.ResumeLayout();
				//FillMovementsAndHidden();
				//Cursor=Cursors.Default;
				//return;
			//}
			if(ShowBySelectedTeeth) {
				for(int i=0;i<selectedTeeth.Count;i++) {
					_toothChartRelay.SetSelected(selectedTeeth[i],true);
				}
			}
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<ToothInitialList.Count;i++) {
				if(ToothInitialList[i].InitialType==ToothInitialType.Primary) {
					_toothChartRelay.SetPrimary(ToothInitialList[i].ToothNum);
				}
			}
			for(int i=0;i<ToothInitialList.Count;i++) {
				switch(ToothInitialList[i].InitialType) {
					case ToothInitialType.Missing:
						_toothChartRelay.SetMissing(ToothInitialList[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						_toothChartRelay.SetHidden(ToothInitialList[i].ToothNum);
						break;
					//case ToothInitialType.Primary:
					//	break;
					case ToothInitialType.Rotate:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,ToothInitialList[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,ToothInitialList[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,ToothInitialList[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,ToothInitialList[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,ToothInitialList[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,0,ToothInitialList[i].Movement);
						break;
					case ToothInitialType.Drawing:
						_toothChartRelay.AddDrawingSegment(ToothInitialList[i].Copy());
						break;
					case ToothInitialType.Text:
						_toothChartRelay.AddText(ToothInitialList[i].GetTextString(), ToothInitialList[i].GetTextPoint(), ToothInitialList[i].ColorDraw, ToothInitialList[i].ToothInitialNum);
						break;
				}
			}
			DrawProcGraphics();
			_toothChartRelay.EndUpdate();
			//FillMovementsAndHidden();
			Cursor=Cursors.Default;
		}

		private void DrawProcGraphics() {
			//this requires: ProcStatus, ProcCode, ToothNum, HideGraphics, Surf, and ToothRange.  All need to be raw database values.
			string[] teeth;
			List<long> listCurProvNums=_toothChartRelay.GetPertinentProvNumsForToothColorPref(Security.CurUser,PatCur,ListAppts);
			for(int i=0;i<ProcList.Count;i++) {
				if(ProcList[i]["HideGraphics"].ToString()=="1") {
					continue;
				}
				if(ProcedureCodes.GetProcCode(ProcList[i]["ProcCode"].ToString()).PaintType==ToothPaintingType.Extraction && (
					PIn.Long(ProcList[i]["ProcStatus"].ToString())==(int)ProcStat.C
					|| PIn.Long(ProcList[i]["ProcStatus"].ToString())==(int)ProcStat.EC
					|| PIn.Long(ProcList[i]["ProcStatus"].ToString())==(int)ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				ProcedureCode procCode = ProcedureCodes.GetProcCode(ProcList[i]["ProcCode"].ToString());
				ProcStat procStatus = (ProcStat)PIn.Long(ProcList[i]["ProcStatus"].ToString());
				long procProvNum=PIn.Long(ProcList[i]["ProvNum"].ToString());
				bool doApplyColorPref=_toothChartRelay.DoesToothColorPrefApply(listCurProvNums,procProvNum);
				_toothChartRelay.GetToothColors(procCode,procStatus,doApplyColorPref,out Color cDark,out Color cLight);
				switch(ProcedureCodes.GetProcCode(ProcList[i]["ProcCode"].ToString()).PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,ProcList[i]["ToothNum"].ToString())) {
							_toothChartRelay.SetPontic(ProcList[i]["ToothNum"].ToString(),cDark);
						}
						else {
							_toothChartRelay.SetCrown(ProcList[i]["ToothNum"].ToString(),cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,ProcList[i]["ToothNum"].ToString())) {
							_toothChartRelay.SetPontic(ProcList[i]["ToothNum"].ToString(),cLight);
						}
						else {
							_toothChartRelay.SetCrown(ProcList[i]["ToothNum"].ToString(),cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						_toothChartRelay.SetCrown(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.CrownLight:
						_toothChartRelay.SetCrown(ProcList[i]["ToothNum"].ToString(),cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(ProcList[i]["Surf"].ToString()=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(ProcList[i]["Surf"].ToString()=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=ProcList[i]["ToothRange"].ToString().Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,teeth[t])) {
								_toothChartRelay.SetPontic(teeth[t],cDark);
							}
							else {
								_toothChartRelay.SetCrown(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(ProcList[i]["Surf"].ToString()=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(ProcList[i]["Surf"].ToString()=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=ProcList[i]["ToothRange"].ToString().Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,teeth[t])) {
								_toothChartRelay.SetPontic(teeth[t],cLight);
							}
							else {
								_toothChartRelay.SetCrown(teeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						_toothChartRelay.SetBigX(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.FillingDark:
						_toothChartRelay.SetSurfaceColors(ProcList[i]["ToothNum"].ToString(),ProcList[i]["Surf"].ToString(),cDark);
						break;
					case ToothPaintingType.FillingLight:
						_toothChartRelay.SetSurfaceColors(ProcList[i]["ToothNum"].ToString(),ProcList[i]["Surf"].ToString(),cLight);
						break;
					case ToothPaintingType.Implant:
						_toothChartRelay.SetImplant(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.PostBU:
						_toothChartRelay.SetBU(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RCT:
						_toothChartRelay.SetRCT(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						_toothChartRelay.SetRetainedRoot(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.Sealant:
						_toothChartRelay.SetSealant(ProcList[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(procCode.TreatArea==TreatmentArea.Tooth && ProcList[i]["ToothNum"].ToString()!=""){
							_toothChartRelay.SetSpaceMaintainer(ProcList[i]["ToothNum"].ToString(),cDark);
						}
						else if(procCode.TreatArea==TreatmentArea.ToothRange && ProcList[i]["ToothRange"].ToString()!=""){
							teeth=ProcList[i]["ToothRange"].ToString().Split(',');
							for(int t=0;t<teeth.Length;t++) {
								_toothChartRelay.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						else if(procCode.TreatArea==TreatmentArea.Quad){
							teeth=new string[0];
							if(ProcList[i]["Surf"].ToString()=="UR") {
								teeth=new string[] {"4","5","6","7","8"};
							}
							if(ProcList[i]["Surf"].ToString()=="UL") {
								teeth=new string[] {"9","10","11","12","13"};
							}
							if(ProcList[i]["Surf"].ToString()=="LL") {
								teeth=new string[] {"20","21","22","23","24"};
							}
							if(ProcList[i]["Surf"].ToString()=="LR") {
								teeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<teeth.Length;t++) {//could still be length 0
								_toothChartRelay.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						_toothChartRelay.SetText(ProcList[i]["ToothNum"].ToString(),cDark,ProcedureCodes.GetProcCode(ProcList[i]["ProcCode"].ToString()).PaintText);
						break;
					case ToothPaintingType.Veneer:
						_toothChartRelay.SetVeneer(ProcList[i]["ToothNum"].ToString(),cLight);
						break;
				}
			}
		}

		private void FormToothChartingBig_FormClosed(object sender,FormClosedEventArgs e) {
			//This helps ensure that the tooth chart wrapper is properly disposed of.
			//This step is necessary so that graphics memory does not fill up.
			Dispose();
		}	


	}
}






















