using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using SparksToothChart;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormToothChartingBig:FormODBase {
		private bool _showBySelectedTeeth;
		public List<ToothInitial> ListToothInitials;
		private ToothChartRelay _toothChartRelay;
		///<summary>This is the new Sparks3D toothChart.</summary>
		private Control toothChart;
		public List<DataRow> ListDataRowsProcs;
		public List<Appointment> ListAppointments;
		public Patient PatientCur;
		public List<OrthoHardware> ListOrthoHardwares;
		public bool IsOrthoMode;

		///<summary></summary>
		public FormToothChartingBig(bool showBySelectedTeeth)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_showBySelectedTeeth=showBySelectedTeeth;
		}

		private void FormToothChartingBig_Load(object sender,EventArgs e) {
			this.Text=Lan.g(this,"Big Tooth Chart")+" - "+PatientCur.GetNameLF()+" - "+PatientCur.PatNum;
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
			_toothChartRelay.SetOrthoMode(IsOrthoMode);
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(IsOrthoMode && toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				_toothChartRelay.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
			}
			else{
				_toothChartRelay.SetToothNumberingNomenclature(toothNumberingNomenclature);
			}
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			_toothChartRelay.ColorBackgroundMain=listDefs[10].ItemColor;
			_toothChartRelay.ColorText=listDefs[11].ItemColor;
			_toothChartRelay.ColorTextHighlightFore=listDefs[12].ItemColor;
			_toothChartRelay.ColorTextHighlightBack=listDefs[13].ItemColor;
			//remember which teeth were selected
			List<string> listSelectedTeeth=new List<string>(toothChartWrapper.SelectedTeeth);
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
			if(_showBySelectedTeeth) {
				for(int i=0;i<listSelectedTeeth.Count;i++) {
					_toothChartRelay.SetSelected(listSelectedTeeth[i],true);
				}
			}
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<ListToothInitials.Count;i++) {
				if(ListToothInitials[i].InitialType==ToothInitialType.Primary) {
					_toothChartRelay.SetPrimary(ListToothInitials[i].ToothNum);
				}
			}
			for(int i=0;i<ListToothInitials.Count;i++) {
				switch(ListToothInitials[i].InitialType) {
					case ToothInitialType.Missing:
						_toothChartRelay.SetMissing(ListToothInitials[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						_toothChartRelay.SetHidden(ListToothInitials[i].ToothNum);
						break;
					//case ToothInitialType.Primary:
					//	break;
					case ToothInitialType.Rotate:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,ListToothInitials[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,0,ListToothInitials[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,0,0,ListToothInitials[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,0,0,0,ListToothInitials[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,0,0,0,0,ListToothInitials[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						_toothChartRelay.MoveTooth(ListToothInitials[i].ToothNum,0,0,0,0,0,ListToothInitials[i].Movement);
						break;
					case ToothInitialType.Drawing:
						_toothChartRelay.AddDrawingSegment(ListToothInitials[i].Copy());
						break;
					case ToothInitialType.Text:
						_toothChartRelay.AddText(ListToothInitials[i].GetTextString(), ListToothInitials[i].GetTextPoint(), ListToothInitials[i].ColorDraw, ListToothInitials[i].ToothInitialNum);
						break;
				}
			}
			DrawProcGraphics();
			DrawOrthoHardware();
			_toothChartRelay.EndUpdate();
			//FillMovementsAndHidden();
			Cursor=Cursors.Default;
		}

		private void DrawProcGraphics() {
			//this requires: ProcStatus, ProcCode, ToothNum, HideGraphics, Surf, and ToothRange.  All need to be raw database values.
			string[] stringArrayTeeth;
			List<long> listProvNums=_toothChartRelay.GetPertinentProvNumsForToothColorPref(Security.CurUser,PatientCur,ListAppointments);
			for(int i=0;i<ListDataRowsProcs.Count;i++) {
				if(ListDataRowsProcs[i]["HideGraphics"].ToString()=="1") {
					continue;
				}
				if(ProcedureCodes.GetProcCode(ListDataRowsProcs[i]["ProcCode"].ToString()).PaintType==ToothPaintingType.Extraction && (
					PIn.Long(ListDataRowsProcs[i]["ProcStatus"].ToString())==(int)ProcStat.C
					|| PIn.Long(ListDataRowsProcs[i]["ProcStatus"].ToString())==(int)ProcStat.EC
					|| PIn.Long(ListDataRowsProcs[i]["ProcStatus"].ToString())==(int)ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				ProcedureCode procedureCode = ProcedureCodes.GetProcCode(ListDataRowsProcs[i]["ProcCode"].ToString());
				ProcStat procStat = (ProcStat)PIn.Long(ListDataRowsProcs[i]["ProcStatus"].ToString());
				long provNum=PIn.Long(ListDataRowsProcs[i]["ProvNum"].ToString());
				bool doApplyColorPref=_toothChartRelay.DoesToothColorPrefApply(listProvNums,provNum);
				_toothChartRelay.GetToothColors(procedureCode,procStat,doApplyColorPref,out Color cDark,out Color cLight);
				switch(ProcedureCodes.GetProcCode(ListDataRowsProcs[i]["ProcCode"].ToString()).PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(ListToothInitials,ListDataRowsProcs[i]["ToothNum"].ToString())) {
							_toothChartRelay.SetPontic(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						}
						else {
							_toothChartRelay.SetCrown(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(ListToothInitials,ListDataRowsProcs[i]["ToothNum"].ToString())) {
							_toothChartRelay.SetPontic(ListDataRowsProcs[i]["ToothNum"].ToString(),cLight);
						}
						else {
							_toothChartRelay.SetCrown(ListDataRowsProcs[i]["ToothNum"].ToString(),cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						_toothChartRelay.SetCrown(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.CrownLight:
						_toothChartRelay.SetCrown(ListDataRowsProcs[i]["ToothNum"].ToString(),cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(ListDataRowsProcs[i]["Surf"].ToString()=="U") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(ListDataRowsProcs[i]["Surf"].ToString()=="L") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							stringArrayTeeth=ListDataRowsProcs[i]["ToothRange"].ToString().Split(new char[] { ',' });
						}
						for(int t=0;t<stringArrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ListToothInitials,stringArrayTeeth[t])) {
								_toothChartRelay.SetPontic(stringArrayTeeth[t],cDark);
							}
							else {
								_toothChartRelay.SetCrown(stringArrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(ListDataRowsProcs[i]["Surf"].ToString()=="U") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(ListDataRowsProcs[i]["Surf"].ToString()=="L") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							stringArrayTeeth=ListDataRowsProcs[i]["ToothRange"].ToString().Split(new char[] { ',' });
						}
						for(int t=0;t<stringArrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ListToothInitials,stringArrayTeeth[t])) {
								_toothChartRelay.SetPontic(stringArrayTeeth[t],cLight);
							}
							else {
								_toothChartRelay.SetCrown(stringArrayTeeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						_toothChartRelay.SetBigX(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.FillingDark:
						_toothChartRelay.SetSurfaceColors(ListDataRowsProcs[i]["ToothNum"].ToString(),ListDataRowsProcs[i]["Surf"].ToString(),cDark);
						break;
					case ToothPaintingType.FillingLight:
						_toothChartRelay.SetSurfaceColors(ListDataRowsProcs[i]["ToothNum"].ToString(),ListDataRowsProcs[i]["Surf"].ToString(),cLight);
						break;
					case ToothPaintingType.Implant:
						_toothChartRelay.SetImplant(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.PostBU:
						_toothChartRelay.SetBU(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RCT:
						_toothChartRelay.SetRCT(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						_toothChartRelay.SetRetainedRoot(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.Sealant:
						_toothChartRelay.SetSealant(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(procedureCode.TreatArea==TreatmentArea.Tooth && ListDataRowsProcs[i]["ToothNum"].ToString()!=""){
							_toothChartRelay.SetSpaceMaintainer(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark);
						}
						else if((procedureCode.TreatArea==TreatmentArea.ToothRange && ListDataRowsProcs[i]["ToothRange"].ToString()!="") 
							|| (procedureCode.AreaAlsoToothRange==true && ListDataRowsProcs[i]["ToothRange"].ToString()!=""))
						{
							stringArrayTeeth=ListDataRowsProcs[i]["ToothRange"].ToString().Split(',');
							for(int t=0;t<stringArrayTeeth.Length;t++) {
								_toothChartRelay.SetSpaceMaintainer(stringArrayTeeth[t],cDark);
							}
						}
						else if(procedureCode.TreatArea==TreatmentArea.Quad){
							stringArrayTeeth=new string[0];
							if(ListDataRowsProcs[i]["Surf"].ToString()=="UR") {
								stringArrayTeeth=new string[] {"4","5","6","7","8"};
							}
							if(ListDataRowsProcs[i]["Surf"].ToString()=="UL") {
								stringArrayTeeth=new string[] {"9","10","11","12","13"};
							}
							if(ListDataRowsProcs[i]["Surf"].ToString()=="LL") {
								stringArrayTeeth=new string[] {"20","21","22","23","24"};
							}
							if(ListDataRowsProcs[i]["Surf"].ToString()=="LR") {
								stringArrayTeeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<stringArrayTeeth.Length;t++) {//could still be length 0
								_toothChartRelay.SetSpaceMaintainer(stringArrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						_toothChartRelay.SetText(ListDataRowsProcs[i]["ToothNum"].ToString(),cDark,ProcedureCodes.GetProcCode(ListDataRowsProcs[i]["ProcCode"].ToString()).PaintText);
						break;
					case ToothPaintingType.Veneer:
						_toothChartRelay.SetVeneer(ListDataRowsProcs[i]["ToothNum"].ToString(),cLight);
						break;
				}
			}
		}

		private void DrawOrthoHardware(){
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			List<OrthoHardwares.OrthoWire> listOrthoWires=new List<OrthoHardwares.OrthoWire>();//also used for elastics
			for(int i=0;i<ListOrthoHardwares.Count;i++){
				OrthoHardwareSpec orthoHardwareSpec=listOrthoHardwareSpecs.Find(x=>x.OrthoHardwareSpecNum==ListOrthoHardwares[i].OrthoHardwareSpecNum);
				if(ListOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Bracket){
					_toothChartRelay.SetBracket(ListOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor);
				}
				if(ListOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Wire){
					listOrthoWires.AddRange(OrthoHardwares.GetWires(ListOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor));
				}
				if(ListOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Elastic){
					listOrthoWires.AddRange(OrthoHardwares.GetElastics(ListOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor));
				}
			}
			for(int i=0;i<listOrthoWires.Count;i++){
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.BetweenBrackets){
					_toothChartRelay.AddOrthoWireBetweenBrackets(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ToothIDend,listOrthoWires[i].ColorDraw);
				}
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.InBracket){
					_toothChartRelay.AddOrthoWireInBracket(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ColorDraw);
				}
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.Elastic){
					_toothChartRelay.AddOrthoElastic(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ToothIDend,listOrthoWires[i].ColorDraw);
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