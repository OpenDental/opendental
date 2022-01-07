using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparks3D;
using System.Windows.Forms;

//Notes about how this DLL works:  There are two DLLs and a readme.txt nested inside a subfolder called Sparks3D. 
//First of all, as a separate issue, the DLL must be present in the Required dlls folder in order to compile.  No local copy is made during compile.
//We will install a Sparks3D subfolder in the application folder as part of the distro.
//When it runs, either in the debugger, or by clicking on the exe, it loads the dll from the subfolder of the application folder.
//This page says we need to help C# find the dll, but C# seems to find it just fine as long as the name is "Sparks3D"
//https://stackoverflow.com/questions/11428304/c-sharp-get-dll-out-of-a-subfolder 

//Notes about the references to the DLL:  OD needs to run whether the Sparks3D.dll is present or not.  
//A strategy is used, where the only references to Sparks3D are in this one class/file.  
//And then, we ensure that this class is never hit unless Sparks3D.dll is present, preventing it from ever crashing.

namespace OpenDental.UI{
	///<summary></summary>
	public class Sparks3DInterface{
		private ToothChart _toothChart;

		public Sparks3DInterface(bool hasHwnd=true){
			_toothChart=new ToothChart(hasHwnd);
			_toothChart.SegmentDrawn += _toothChart_SegmentDrawn;
			_toothChart.TextMoved+=_toothChart_TextMoved;
			_toothChart.ToothSelectionsChanged += _toothChart_ToothSelectionsChanged;
		}

		

		#region Events - Raise
		private void _toothChart_SegmentDrawn(object sender, Sparks3D.StringEventArgs e){
			//bubble the event, changing to a different type of event args
			SegmentDrawn?.Invoke(sender,new SparksToothChart.ToothChartDrawEventArgs(e.Str));
		}

		public event SparksToothChart.ToothChartDrawEventHandler SegmentDrawn=null;


		private void _toothChart_TextMoved(object sender,Sparks3D.TextMovedEventArgs e) {
			//convert it from Sparks3D.TextMovedEventArgs to OpenDental.TextMovedEventArgs
			OpenDental.TextMovedEventArgs textMovedEventArgs=new OpenDental.TextMovedEventArgs(){ColorNew=e.ColorNew,LocationNew=e.LocationNew,ToothInitialNum=e.ToothInitialNum };
			TextMoved?.Invoke(sender,textMovedEventArgs);
		}

		public event EventHandler<TextMovedEventArgs> TextMoved=null;

		private void _toothChart_ToothSelectionsChanged(object sender, EventArgs e){
			//bubble the event
			ToothSelectionsChanged?.Invoke(sender);
		}

		public event SparksToothChart.ToothChartSelectionEventHandler ToothSelectionsChanged=null;
		#endregion Events - Raise

		#region Properties
		public int Bottom{
			get{
				return _toothChart.Bottom;
			}
		}

		public Color ColorBackgroundMain{
			set{
				_toothChart.ColorBackgroundMain=value;
			}
		}

		public Color ColorDrawingCurrently{
			set{
				_toothChart.ColorDrawingCurrently=value;
			}
		}

		public Color ColorText{
			set{
				_toothChart.ColorText=value;
			}
		}

		public Color ColorTextHighlightBack{
			set{
				_toothChart.ColorTextHighlightBack=value;
			}
		}

		public Color ColorTextHighlightFore{
			set{
				_toothChart.ColorTextHighlightFore=value;
			}
		}

		public SparksToothChart.CursorTool CursorTool{
			get{
				switch(_toothChart.CursorTool){
					default:
					case EnumCursorTool.Pointer:
						return SparksToothChart.CursorTool.Pointer;
					case EnumCursorTool.Pen:
						return SparksToothChart.CursorTool.Pen;
					case EnumCursorTool.Eraser:
						return SparksToothChart.CursorTool.Eraser;
					case EnumCursorTool.ColorChanger:
						return SparksToothChart.CursorTool.ColorChanger;
					case EnumCursorTool.MoveText:
						return SparksToothChart.CursorTool.MoveText;
				}
			}
			set{
				switch(value){
					default:
					case SparksToothChart.CursorTool.Pointer:
						_toothChart.CursorTool=EnumCursorTool.Pointer;
						break;
					case SparksToothChart.CursorTool.Pen:
						_toothChart.CursorTool=EnumCursorTool.Pen;
						break;
					case SparksToothChart.CursorTool.Eraser:
						_toothChart.CursorTool=EnumCursorTool.Eraser;
						break;
					case SparksToothChart.CursorTool.ColorChanger:
						_toothChart.CursorTool=EnumCursorTool.ColorChanger;
						break;
					case SparksToothChart.CursorTool.MoveText:
						_toothChart.CursorTool=EnumCursorTool.MoveText;
						break;
				}
			}
		}

		public bool Enabled{
			set{
				_toothChart.Enabled=value;
			}
		}

		public int Height{
			get{
				return _toothChart.Height;
			}
		}

		public bool IsPerioMode{
			set{
				_toothChart.SetPerioMode();
			}
		}

		public List<string> SelectedTeeth{
			get{
				return _toothChart.SelectedTeeth;
			}
		}

		public int Width{
			get{
				return _toothChart.Width;
			}
		}
		#endregion Properties

		#region Methods
		public void AddDrawingSegment(string drawingSegment,Color color){
			_toothChart.AddDrawingSegment(drawingSegment,color);
		}

		public void AddPerioMeasure(int intTooth,OpenDentBusiness.PerioSequenceType perioSequenceType,int mb,int b,int db,int ml,int l, int dl){
			Sparks3D.PerioSequenceType perioSequenceType3D;
			switch(perioSequenceType){
				default:
					throw new ApplicationException("PerioSequenceType not allowed.");
				case OpenDentBusiness.PerioSequenceType.Furcation:
					perioSequenceType3D=Sparks3D.PerioSequenceType.Furcation;
					break;
				case OpenDentBusiness.PerioSequenceType.GingMargin:
					perioSequenceType3D=Sparks3D.PerioSequenceType.GingMargin;
					break;
				case OpenDentBusiness.PerioSequenceType.MGJ:
					perioSequenceType3D=Sparks3D.PerioSequenceType.MGJ;
					break;
				case OpenDentBusiness.PerioSequenceType.Probing:
					perioSequenceType3D=Sparks3D.PerioSequenceType.Probing;
					break;
				case OpenDentBusiness.PerioSequenceType.Bleeding:
					perioSequenceType3D=Sparks3D.PerioSequenceType.Bleeding;
					break;
				case OpenDentBusiness.PerioSequenceType.CAL:
					perioSequenceType3D=Sparks3D.PerioSequenceType.CAL;
					break;
			}
			_toothChart.AddPerioMeasure(intTooth,perioSequenceType3D,mb,b,db,ml,l,dl);
		}

		public void AddText(string text,PointF location,Color color,long toothInitialNum){
			_toothChart.AddText(text,location,color,toothInitialNum);
		}

		public void BeginUpdate(){
			_toothChart.BeginUpdate();
		}

		public void EndUpdate(){
			_toothChart.EndUpdate();
		}

		public void Dispose(){
			_toothChart?.Dispose();
		}

		public Bitmap GetBitmap(){
			return _toothChart.GetBitmap();
		}

		public Control GetToothChart(){
			return _toothChart;
		}

		public void MoveTooth(string toothID,float rotate,float tipM,float tipB,float shiftM,float shiftO,float shiftB){
			_toothChart.MoveTooth(toothID,rotate,tipM,tipB,shiftM,shiftO,shiftB);
		}

		public void ResetTeeth(){
			_toothChart.ResetTeeth();
		}

		public void SetBigX(string toothID,Color color){
			_toothChart.SetBigX(toothID,color);
		}

		public void SetBU(string toothID,Color color){
			_toothChart.SetBU(toothID,color);
		}
		
		public void SetCrown(string toothID,Color color){
			_toothChart.SetCrown(toothID,color);
		}

		public void SetHidden(string toothID){
			_toothChart.SetHidden(toothID);
		}

		public void SetImplant(string toothID,Color color){
			_toothChart.SetImplant(toothID,color);
		}

		public void SetMissing(string toothID){
			_toothChart.SetMissing(toothID);
		}

		public void SetMobility(string toothID,string mobility,Color color){
			_toothChart.SetMobility(toothID,mobility,color);
		}

		public void SetPerioColors(Color colorBleeding,Color colorSuppuration,Color colorProbing,Color colorProbingRed,Color colorGM,Color colorCAL,
			Color colorMGJ,Color colorFurcations,Color colorFurcationsRed,int redLimitProbing,int redLimitFurcations)
		{
			_toothChart.ColorBleeding=colorBleeding;
			_toothChart.ColorSuppuration=colorSuppuration;
			_toothChart.ColorProbing=colorProbing;
			_toothChart.ColorProbingRed=colorProbingRed;
			_toothChart.ColorGM=colorGM;
			_toothChart.ColorCAL=colorCAL;
			_toothChart.ColorMGJ=colorMGJ;
			_toothChart.ColorFurcations=colorFurcations;
			_toothChart.ColorFurcationsRed=colorFurcationsRed;
			_toothChart.RedLimitProbing=redLimitProbing;
			_toothChart.RedLimitFurcations=redLimitFurcations;
		}

		public void SetPontic(string toothID,Color color){
			_toothChart.SetPontic(toothID,color);
		}

		public void SetPrimary(string toothID){
			_toothChart.SetPrimary(toothID);
		}

		public void SetRCT(string toothID,Color color){
			_toothChart.SetRCT(toothID,color);
		}

		public void SetRetainedRoot(string toothID,Color color){
			_toothChart.SetRetainedRoot(toothID,color);
		}

		public void SetSealant(string toothID,Color color){
			_toothChart.SetSealant(toothID,color);
		}
		
		public void SetSelected(string toothID,bool setValue){
			_toothChart.SetSelected(toothID,setValue);
		}

		public void SetSpaceMaintainer(string toothID,Color color){
			_toothChart.SetSpaceMaintainer(toothID,color);
		}

		public void SetSurfaceColors(string toothID,string surfaces,Color color){
			_toothChart.SetSurfaceColors(toothID,surfaces,color);
		}

		public void SetText(string toothID,Color color,string text){
			_toothChart.SetText(toothID,color,text);
		}

		public void SetToothNumberingNomenclature(OpenDentBusiness.ToothNumberingNomenclature toothNumberingNomenclature){
			switch(toothNumberingNomenclature){
				case OpenDentBusiness.ToothNumberingNomenclature.FDI:
					_toothChart.SetToothNumberingNomenclature(ToothNumberingNomenclature.FDI);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Haderup:
					_toothChart.SetToothNumberingNomenclature(ToothNumberingNomenclature.Haderup);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Palmer:
					_toothChart.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
					break;
				case OpenDentBusiness.ToothNumberingNomenclature.Universal:
					_toothChart.SetToothNumberingNomenclature(ToothNumberingNomenclature.Universal);
					break;
			}
		}

		public void SetVeneer(string toothID,Color color){
			_toothChart.SetVeneer(toothID,color);
		}
		#endregion Methods

	}
}
