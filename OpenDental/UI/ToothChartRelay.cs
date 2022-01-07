using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using SparksToothChart;

namespace OpenDental{
	///<summary>Relays commands to either the old SparksToothChart.ToothChartWrapper or the new Sparks3d.ToothChart.</summary>
	public class ToothChartRelay{
		///<summary>This is set when the program starts up.  If true, it will load up the new tooth chart in many places.</summary>
		public static bool IsSparks3DPresent;
		private Sparks3DInterface _sparks3DInterface;
		private ToothChartWrapper _toothChartWrapper;

		public ToothChartRelay(bool hasHwnd=true){
			if(IsSparks3DPresent){
				try{
					_sparks3DInterface=new Sparks3DInterface(hasHwnd);
				}
				catch{
					//Probably older version of Windows that does not support DirectX 11.1.
					//which means Win7 instead of Win8+
					IsSparks3DPresent=false;
					return;
				}
				_sparks3DInterface.SegmentDrawn += _sparks3DInterface_SegmentDrawn;
				_sparks3DInterface.TextMoved+=_sparks3DInterface_TextMoved;
				_sparks3DInterface.ToothSelectionsChanged += _sparks3DInterface_ToothSelectionsChanged;
			}
		}

		#region Events
		public event SparksToothChart.ToothChartDrawEventHandler SegmentDrawn=null;

		private void _sparks3DInterface_SegmentDrawn(object sender, ToothChartDrawEventArgs e){
			SegmentDrawn?.Invoke(sender,e);//simple bubble
		}

		public event EventHandler<TextMovedEventArgs> TextMoved=null;

		private void _sparks3DInterface_TextMoved(object sender,TextMovedEventArgs e) {
			TextMoved?.Invoke(sender,e);
		}

		public event SparksToothChart.ToothChartSelectionEventHandler ToothSelectionsChanged=null;

		private void _sparks3DInterface_ToothSelectionsChanged(object sender){
			ToothSelectionsChanged?.Invoke(sender);//simple bubble
		}
		#endregion Events

		#region Properties
		public int Bottom{
			get{
				if(IsSparks3DPresent){
					return _sparks3DInterface.Bottom;
				}
				else{
					return _toothChartWrapper.Bottom;
				}
			}
		}

		public Color ColorBackgroundMain{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.ColorBackgroundMain=value;
				}
				else{
					_toothChartWrapper.ColorBackground=value;
				}
			}
		}

		public Color ColorDrawing{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.ColorDrawingCurrently=value;
				}
				else{
					_toothChartWrapper.ColorDrawing=value;
				}
			}
		}

		public Color ColorText{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.ColorText=value;
				}
				else{
					_toothChartWrapper.ColorText=value;
				}
			}
		}

		public Color ColorTextHighlightBack{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.ColorTextHighlightBack=value;
				}
				else{
					_toothChartWrapper.ColorBackHighlight=value;
				}
			}
		}

		public Color ColorTextHighlightFore{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.ColorTextHighlightFore=value;
				}
				else{
					_toothChartWrapper.ColorTextHighlight=value;
				}
			}
		}

		public CursorTool CursorTool{
			get{
				if(IsSparks3DPresent){
					return _sparks3DInterface.CursorTool;
				}
				else{
					return _toothChartWrapper.CursorTool;
				}
			}
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.CursorTool=value;
				}
				else{
					_toothChartWrapper.CursorTool=value;
				}
			}
		}

		public bool Enabled{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.Enabled=value;
				}
				else{
					_toothChartWrapper.Enabled=value;
				}
			}
		}

		public int Height{
			get{
				if(IsSparks3DPresent){
					return _sparks3DInterface.Height;
				}
				else{
					return _toothChartWrapper.Height;
				}
			}
		}

		public bool IsPerioMode{
			set{
				if(IsSparks3DPresent){
					_sparks3DInterface.IsPerioMode=true;
				}
				else{
					_toothChartWrapper.PerioMode=true;
				}
			}
		}

		public List<string> SelectedTeeth{
			get{
				if(IsSparks3DPresent){
					return _sparks3DInterface.SelectedTeeth;
				}
				else{
					return _toothChartWrapper.SelectedTeeth;
				}
			}
		}

		public int Width{
			get{
				if(IsSparks3DPresent){
					return _sparks3DInterface.Width;
				}
				else{
					return _toothChartWrapper.Width;
				}
			}
		}
		#endregion Properties

		#region Methods
		public void AddDrawingSegment(ToothInitial drawingSegment){
			if(IsSparks3DPresent){
				_sparks3DInterface.AddDrawingSegment(drawingSegment.DrawingSegment,drawingSegment.ColorDraw);
			}
			else{
				_toothChartWrapper.AddDrawingSegment(drawingSegment);
			}
		}

		public void AddPerioMeasure(int intTooth,PerioSequenceType sequenceType,int mb,int b,int db,int ml,int l, int dl){
			if(IsSparks3DPresent){
				_sparks3DInterface.AddPerioMeasure(intTooth,sequenceType,mb,b,db,ml,l,dl);
			}
			else{
				_toothChartWrapper.AddPerioMeasure(intTooth,sequenceType,mb,b,db,ml,l,dl);
			}
		}

		///<summary>Not to be confused with SetText.</summary>
		public void AddText(string text,PointF location,Color color,long toothInitialNum){
			if(IsSparks3DPresent){
				_sparks3DInterface.AddText(text,location,color,toothInitialNum);
			}
			else{
				//not supported
			}
		}

		///<summary>Sparks3D.BeginUpdate and toothChartWrapper.SuspendLayout</summary>
		public void BeginUpdate(){
			if(IsSparks3DPresent){
				_sparks3DInterface.BeginUpdate();
			}
			else{
				_toothChartWrapper.SuspendLayout();//badly named
			}
		}

		public void DisposeControl(){
			if(IsSparks3DPresent){
				_sparks3DInterface?.Dispose();
			}
			else{
				_toothChartWrapper.Dispose();
			}
		}

		///<summary>Sparks3D.EndUpdate and toothChartWrapper.ResumeLayout</summary>
		public void EndUpdate(){
			if(IsSparks3DPresent){
				_sparks3DInterface.EndUpdate();
			}
			else{
				_toothChartWrapper.ResumeLayout();//badly named
			}
		}

		public Bitmap GetBitmap(){
			if(IsSparks3DPresent){
				return _sparks3DInterface.GetBitmap();
			}
			else{
				return _toothChartWrapper.GetBitmap();
			}
		}

		public Control GetToothChart(){
			return _sparks3DInterface.GetToothChart();
		}

		public void MoveTooth(string toothID,float rotate,float tipM,float tipB,float shiftM,float shiftO,float shiftB){
			if(IsSparks3DPresent){
				_sparks3DInterface.MoveTooth(toothID,rotate,tipM,tipB,shiftM,shiftO,shiftB);
			}
			else{
				_toothChartWrapper.MoveTooth(toothID,rotate,tipM,tipB,shiftM,shiftO,shiftB);
			}
		}

		public void ResetTeeth(){
			if(IsSparks3DPresent){
				_sparks3DInterface.ResetTeeth();
			}
			else{
				_toothChartWrapper.ResetTeeth();
			}
		}

		public void SetBigX(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetBigX(toothID,color);
			}
			else{
				_toothChartWrapper.SetBigX(toothID,color);
			}
		}

		public void SetBU(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetBU(toothID,color);
			}
			else{
				_toothChartWrapper.SetBU(toothID,color);
			}
		}

		public void SetCrown(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetCrown(toothID,color);
			}
			else{
				_toothChartWrapper.SetCrown(toothID,color);
			}
		}

		public void SetHidden(string toothID){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetHidden(toothID);
			}
			else{
				_toothChartWrapper.SetHidden(toothID);
			}
		}

		public void SetImplant(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetImplant(toothID,color);
			}
			else{
				_toothChartWrapper.SetImplant(toothID,color);
			}
		}

		public void SetMissing(string toothID){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetMissing(toothID);
			}
			else{
				_toothChartWrapper.SetMissing(toothID);
			}
		}

		public void SetMobility(string toothID,string mobility,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetMobility(toothID,mobility,color);
			}
			else{
				_toothChartWrapper.SetMobility(toothID,mobility,color);
			}
		}

		public void SetPerioColors(Color colorBleeding,Color colorSuppuration,Color colorProbing,Color colorProbingRed,Color colorGM,Color colorCAL,
			Color colorMGJ,Color colorFurcations,Color colorFurcationsRed,int redLimitProbing,int redLimitFurcations){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetPerioColors(colorBleeding,colorSuppuration,colorProbing,colorProbingRed,colorGM,colorCAL,
					colorMGJ,colorFurcations,colorFurcationsRed,redLimitProbing,redLimitFurcations);
			}
			else{
				_toothChartWrapper.ColorBleeding=colorBleeding;
				_toothChartWrapper.ColorSuppuration=colorSuppuration;
				_toothChartWrapper.ColorProbing=colorProbing;
				_toothChartWrapper.ColorProbingRed=colorProbingRed;
				_toothChartWrapper.ColorGingivalMargin=colorGM;
				_toothChartWrapper.ColorCAL=colorCAL;
				_toothChartWrapper.ColorMGJ=colorMGJ;
				_toothChartWrapper.ColorFurcations=colorFurcations;
				_toothChartWrapper.ColorFurcationsRed=colorFurcationsRed;
				_toothChartWrapper.RedLimitProbing=redLimitProbing;
				_toothChartWrapper.RedLimitFurcations=redLimitFurcations;
			}
		}

		public void SetPontic(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetPontic(toothID,color);
			}
			else{
				_toothChartWrapper.SetPontic(toothID,color);
			}
		}

		public void SetPrimary(string toothID){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetPrimary(toothID);
			}
			else{
				_toothChartWrapper.SetPrimary(toothID);
			}
		}

		public void SetRCT(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetRCT(toothID,color);
			}
			else{
				_toothChartWrapper.SetRCT(toothID,color);
			}
		}

		public void SetRetainedRoot(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetRetainedRoot(toothID,color);
			}
			else{
				//not supported
			}
		}

		public void SetSealant(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetSealant(toothID,color);
			}
			else{
				_toothChartWrapper.SetSealant(toothID,color);
			}
		}

		public void SetSelected(string toothID,bool setValue){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetSelected(toothID,setValue);
			}
			else{
				_toothChartWrapper.SetSelected(toothID,setValue);
			}
		}

		public void SetSpaceMaintainer(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetSpaceMaintainer(toothID,color);
			}
			else{
				//not supported
			}
		}

		public void SetSurfaceColors(string toothID,string surfaces,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetSurfaceColors(toothID,surfaces,color);
			}
			else{
				_toothChartWrapper.SetSurfaceColors(toothID,surfaces,color);
			}
		}

		public void SetText(string toothID,Color color,string text){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetText(toothID,color,text);
			}
			else{
				if(text=="W"){
					_toothChartWrapper.SetWatch(toothID,color);
				}
				//Other text not supported
			}
		}

		public void SetToothChartWrapper(ToothChartWrapper toothChartWrapper){
			_toothChartWrapper=toothChartWrapper;
		}

		public void SetToothNumberingNomenclature(ToothNumberingNomenclature toothNumberingNomenclature){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetToothNumberingNomenclature(toothNumberingNomenclature);
			}
			else{
				_toothChartWrapper.SetToothNumberingNomenclature(toothNumberingNomenclature);
			}
		}

		public void SetVeneer(string toothID,Color color){
			if(IsSparks3DPresent){
				_sparks3DInterface.SetVeneer(toothID,color);
			}
			else{
				_toothChartWrapper.SetVeneer(toothID,color);
			}
		}
		#endregion Methods

		#region Colors
		private static List<UserOdPref> _listUserOdPrefsToothChartColor;
		
		/// <summary> Checks to see if preferences are up to date. If they are, no database query is made. However if a change occurs for a urser the list needs to update iteslf
		private static List<UserOdPref> GetListUserOdPrefs() {
			if(_listUserOdPrefsToothChartColor==null) {
				RefreshToothColorsPrefs();
			}
			return _listUserOdPrefsToothChartColor;
		}
		///<summary>Checks ProvNums only if the logged in user has 'ToothChartUsesDiffColorByProv' user pref on</summary>
		public bool DoesToothColorPrefApply(List<long> listCurProvNums,long procProvNum) {
			//This first check is just being extra careful.
			return HasToothColorUserPref() && !ListTools.In(procProvNum,listCurProvNums);
		}

		///<summary>
		///Returns ProvNums related to the passed in entities in a specific order. Returns the first of the following conditions: 
		///	1. Returns an empty list if the user currently logged in does not have ToothChartUsesDiffColorByProv user pref. 
		///	2. Returns the ProvNum of the user currently logged in if one is set. 
		///	3. Returns all primary providers for any appointment for the patient passed in that is scheduled for today. 
		///	4. Returns the primary provider if none of the previous conditions were met. Can return an empty list.
		///	</summary>
		public List<long> GetPertinentProvNumsForToothColorPref(Userod user,Patient patCur,List<Appointment> listAppts) {
			List<long> listCurProvNums=new List<long>();
			if(!HasToothColorUserPref()) {//We will leave this method if the current user doesn't have the pref
				return new List<long>();
			}
			//First, checks if current user is a provider
			if(user?.ProvNum!=0) {
				return new List<long>() { Security.CurUser.ProvNum };
			}
			if(patCur==null) {
				return new List<long>();
			}
			//Second, grabs the primary provider on today's appointment for the current patient. This list has already been filtered but just in case
			List<long> listApptProvNums=listAppts?.Where(x=>x.AptDateTime.Date==DateTime.Today && x.PatNum==patCur.PatNum)
				.Select(x=>x.ProvNum).ToList();
			if(!listApptProvNums.IsNullOrEmpty()) {
				return listApptProvNums;
			}
			//Finally, return the patient's primary provider. 
			return new List<long>() { patCur.PriProv };
		}

		///<summary>Sets colors for the teeth in the graphical tooth chart.  Pass doUseEcEoForCProcs=true if C procs should be colored like EO/EC.</summary>
		public bool GetToothColors(ProcedureCode curProc,ProcStat procStatus,bool doUseEcEoForCProcs,out Color cDark,out Color cLight) {
			cDark=Color.White;
			cLight=Color.White;
			List<Def> listDefsToothColors=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
			if(curProc.GraphicColor!=Color.FromArgb(0)) {
				cDark=curProc.GraphicColor;
				cLight=curProc.GraphicColor;
				return true;
			}
			switch(procStatus) {
				case ProcStat.C:					
					if(doUseEcEoForCProcs) {//Whoever is currently viewing the ToothChart wants other Provider's C procs to show as EC
						cDark=listDefsToothColors[3].ItemColor;//same values as ProcStat.EO
						cLight=listDefsToothColors[8].ItemColor;
					}
					else {
						cDark=listDefsToothColors[1].ItemColor;
						cLight=listDefsToothColors[6].ItemColor;
					}
					return true;
				case ProcStat.TP:
					cDark=listDefsToothColors[0].ItemColor;
					cLight=listDefsToothColors[5].ItemColor;
					return true;
				case ProcStat.EC:
					cDark=listDefsToothColors[2].ItemColor;
					cLight=listDefsToothColors[7].ItemColor;
					return true;
				case ProcStat.EO:
					cDark=listDefsToothColors[3].ItemColor;
					cLight=listDefsToothColors[8].ItemColor;
					return true;
				case ProcStat.R:
					cDark=listDefsToothColors[4].ItemColor;
					cLight=listDefsToothColors[9].ItemColor;
					return true;
				case ProcStat.Cn:
					cDark=listDefsToothColors[16].ItemColor;
					cLight=listDefsToothColors[17].ItemColor;
					return true;
				case ProcStat.D://Can happen with invalidated locked procs.
				default:
					cDark=Color.White;
					cLight=Color.White;
					return false;//Don't draw.
			}
		}

		///<summary>Will update list of users with preference if a change is made while OD is running</summary>
		public static void RefreshToothColorsPrefs() {
			_listUserOdPrefsToothChartColor=UserOdPrefs.GetByFkeyType(UserOdFkeyType.ToothChartUsesDiffColorByProv);
		}

		///<summary>Returns true if the currently logged in user has ColorChartUsesDiffByProv preference</summary>
		public bool HasToothColorUserPref(){
			return ListTools.In(Security.CurUser.UserNum,GetListUserOdPrefs().Select(x=>x.UserNum));
		}

		#endregion Colors

	}

	#region Other Class
	///<summary>This looks just like the one in Sparks3D, but it's in a different namespace.</summary>
	public class TextMovedEventArgs:EventArgs{
		public Color ColorNew { get; set; }
		public PointF LocationNew { get; set; }
		public long ToothInitialNum { get; set; }
	}
	#endregion Other Class
}
