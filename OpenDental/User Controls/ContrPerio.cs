using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using SparksToothChart;
using CodeBase;
using WpfControls.UI;

namespace OpenDental
{
	///<summary>Summary description for ContrPerio.</summary>
	public class ContrPerio : System.Windows.Forms.Control{
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.Container components = null;
		///<summary>Because we want to stay on pixel alignment, and because scale varies, there will be some dead space at right and bottom.  This is the height not including that dead space.</summary> 
		private int _heightShowing;
		///<summary>Because we want to stay on pixel alignment, and because scale varies, there will be some dead space at right and bottom.  This is the width not including that dead space.</summary> 
		private int _widthShowing;
		///<summary>33. Width of one tooth.  Each measurement is 10 (scalable), and each border is 1 (scalable).</summary> 
		private int _widthTooth;
		///<summary>Height of one row of probing. Gives a little extra room for bleeding.</summary> 
		private int _heightProb;
		///<summary>Height of one of the shorter rows (non probing).</summary> 
		private int _heightShort;
		///<summary>Rows of probing depths.</summary> 
		private int _rowsProbing;
		///<summary>First dimension is either UF, UL, LF, or LL. Second dim is an array of the types of rows showing in that section.</summary>
		private PerioSequenceType[][] _perioSequenceTypeArray;
		///<summary>Width of the left column that holds descriptions and dates.</summary> 
		private int _widthLeft;
		///<summary>Height of the 'tooth' sections. Right now, it just holds the tooth number.</summary> 
		private int _heightTooth;
		///<summary>Color of the outer border and the major black dividers.</summary>
		private Color _colorLinesMain;
		///<summary>Gray color of the background section of the shorter inner rows.</summary>
		private Color _colorBackShort;
		///<summary>Color of a highlighted cell.</summary>
		private Color _colorHighlight;
		///<summary>Color of the text of a skipped tooth.</summary>
		private Color _colorSkip;
		///<summary>Color of the vertical lines between each tooth.</summary>
		private Color _colorLinesMinor;
		///<summary>Color of the main background.</summary>
		private Color _colorBack;
		///<summary>Color of red probing depths.</summary>
		private Color _colorRedText;
		///<summary>Color of the dot over a number representing blood.</summary>
		private Color _colorBlood;
		///<summary>Color of the dot over a number representing suppuration.</summary>
		private Color _colorSuppuration;
		///<summary>Color of the dot over a number representing plaque.</summary>
		private Color _colorPlaque;
		///<summary>Color of the dot over a number representing calculus.</summary>
		private Color _colorCalc;
		///<summary>Color of previous measurements from a different exam. Slightly grey.</summary>
		private Color _colorOldText;
		///<summary>Color of previous red measurements from a different exam. Lighter red.</summary>
		private Color _colorOldTextRed;
		///<summary>This data array gets filled when loading an exam. It is altered as the user makes changes, and then the results are saved to the db by reading from this array.</summary>
		private PerioCell[,] _perioCellArray;
		///<summary>Since it's complex to compute X coordinate of each cell, the values are stored in this array.  Used by GetBounds and mouse clicking.</summary>
		private float[] _colPosArray;
		///<summary>Since it's complex to compute Y coordinate of each cell, the values are stored in this array.  Used by GetBounds and mouse clicking.</summary>
		private float[] _rowPosArray;
		///<summary>Stores the column,row of the currently selected cell. Null if none selected.</summary>
		public ColRow ColRowSelected;
		///<summary>Auto advance sequence</summary>
		public EnumAdvanceSequence EnumAdvanceSequence_=EnumAdvanceSequence.MaxFirst;
		///<summary>Current direction of the perio exam sequence</summary>
		public EnumCurrentDirection EnumCurrentDirection_;
		///<summary>The index in PerioExams.List of the currently selected exam.</summary>
		private int _idxExamSelected;
		///<summary>the offset when there are more rows than will display. Value is set at the same time as SelectedExam. So usually 0. Otherwise 1,2,3 or....</summary>
		private int _probingOffset;
		///<summary>Keeps track of what has changed for current exam.  This object contains an toothnum int and a PerioSequenceType enum.</summary>
		public List<PerioMeasureItem> ListPerioMeasureItemsChanged;
		///<summary>Valid values 1-32 int. User can highlight teeth to mark them as skip tooth. The highighting is done completely separately from the normal highlighting functionality because multiple teeth can be highlighted.</summary>
		private ArrayList selectedTeeth;
		///<summary>Valid values 1-32 int. Applies only to the current exam. Loaded from the db durring LoadData().</summary>
		private List<int> _listSkippedTeeth;
		///<summary></summary>
		[Category("Property Changed"),Description("Occurs when the control needs to change the auto advance direction to right.")]
		public event EventHandler DirectionChangedRight = null;
		///<summary></summary>
		[Category("Property Changed"),Description("Occurs when the control needs to change the auto advance direction to left.")]
		public event EventHandler DirectionChangedLeft = null;
		///<summary>Causes each data entry to be entered three times. Also, if the data is a bleeding flag entry, then it changes the behavior by causing it to advance also.</summary>
		public bool ThreeAtATime;
		//public PerioExam PerioExamCur;
		///<summary>Perio security:  False will only allow user to see information but not edit.</summary>
		public bool AllowPerioEdit;
		///<summary>True if all gingival margin values entered should be positive. (Stored in DB as negative.)</summary>
		public bool GingMargPlus;
		///<summary>Used to determine if a tooth had an implant proc.</summary>
		private static List<Procedure> _listProcedures=null;
		///<summary>If false, only show exams from today.</summary>
		private bool _showCurrentExamOnly=true;
		///<summary>Used when attempt to auto advance fails due to tooth 1, 32, or all teeth being skipped</summary>
		bool _shouldJumpBack=false;

		///<summary>A rectangle that represents the drawn perio chart within the control itself, this changes based on zoom settings.</summary>
		public Rectangle RectangleBoundsShowing {
			get{
				return new Rectangle(new Point(0,0),new Size(_widthShowing+2,_heightShowing+2));//Add 2 pixels to the top and bottom for the border.
			}
		}

		///<summary>The index in PerioExams.List of the currently selected exam.</summary>
		public int IdxExamSelected{
			get{
				return _idxExamSelected;
			}
			set{
				_idxExamSelected=value;
				SetProbingOffset();
			}
		}

		///<summary>If false, only show exams from today.</summary>
		public bool ShowCurrentExamOnly {
			get {
				return _showCurrentExamOnly;
			}
			set {
				_showCurrentExamOnly=value;
				SetProbingOffset();
			}
		}

		private void SetProbingOffset() {
			_probingOffset=0;
			if(_showCurrentExamOnly) {
				_probingOffset=_idxExamSelected;
			}
			else if(_idxExamSelected>_rowsProbing-1) {
				_probingOffset=_idxExamSelected-_rowsProbing+1;
			}
		}

		///<summary></summary>
		protected override Size DefaultSize{
			get{
				return new Size(590,665);
			}
		}

		///<summary></summary>
		public ContrPerio(){
			//Clearing these cached items from the last instance in case something changed.
			_listProcedures=null;
			//InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			DoubleBuffered=true;
			this.BackColor = System.Drawing.SystemColors.Window;
			_colorLinesMain=Color.Black;
			//cBackShort=Color.FromArgb(237,237,237);//larger numbers will make it whiter
			_colorBackShort=Color.FromArgb(225,225,225);
			_colorHighlight=Color.FromArgb(158,146,142);//Color.DarkSalmon;
			_colorSkip=Color.LightGray;
			_colorLinesMinor=Color.Silver;
			//cHoriz=Color.LightGray;
			_colorBack=Color.White;
			//cHorizShort=Color.Silver;//or darkgrey
			_colorRedText=Color.Red;
			SetColors();
			_colorOldText=Color.FromArgb(120,120,120);
			_colorOldTextRed=Color.FromArgb(200,80,80);
			_rowsProbing=6;
			_perioSequenceTypeArray=new PerioSequenceType[4][];
			//Upper facial:
			_perioSequenceTypeArray[0]=new PerioSequenceType[5+_rowsProbing];
			_perioSequenceTypeArray[0][0]=PerioSequenceType.Mobility;
			_perioSequenceTypeArray[0][1]=PerioSequenceType.Furcation;
			_perioSequenceTypeArray[0][2]=PerioSequenceType.CAL;
			_perioSequenceTypeArray[0][3]=PerioSequenceType.GingMargin;
			_perioSequenceTypeArray[0][4]=PerioSequenceType.MGJ;
			for(int i=0;i<_rowsProbing;i++){
				_perioSequenceTypeArray[0][5+i]=PerioSequenceType.Probing;
			}
			//Upper lingual:
			_perioSequenceTypeArray[1]=new PerioSequenceType[3+_rowsProbing];
			_perioSequenceTypeArray[1][0]=PerioSequenceType.Furcation;
			_perioSequenceTypeArray[1][1]=PerioSequenceType.CAL;
			_perioSequenceTypeArray[1][2]=PerioSequenceType.GingMargin;
			for(int i=0;i<_rowsProbing;i++){
				_perioSequenceTypeArray[1][3+i]=PerioSequenceType.Probing;
			}
			//Lower lingual:
			_perioSequenceTypeArray[2]=new PerioSequenceType[4+_rowsProbing];
			_perioSequenceTypeArray[2][0]=PerioSequenceType.Furcation;
			_perioSequenceTypeArray[2][1]=PerioSequenceType.CAL;
			_perioSequenceTypeArray[2][2]=PerioSequenceType.GingMargin;
			_perioSequenceTypeArray[2][3]=PerioSequenceType.MGJ;
			for(int i=0;i<_rowsProbing;i++){
				_perioSequenceTypeArray[2][4+i]=PerioSequenceType.Probing;
			}
			//Lower facial:
			_perioSequenceTypeArray[3]=new PerioSequenceType[5+_rowsProbing];
			_perioSequenceTypeArray[3][0]=PerioSequenceType.Mobility;
			_perioSequenceTypeArray[3][1]=PerioSequenceType.Furcation;
			_perioSequenceTypeArray[3][2]=PerioSequenceType.CAL;
			_perioSequenceTypeArray[3][3]=PerioSequenceType.GingMargin;
			_perioSequenceTypeArray[3][4]=PerioSequenceType.MGJ;
			for(int i=0;i<_rowsProbing;i++){
				_perioSequenceTypeArray[3][5+i]=PerioSequenceType.Probing;
			}
			ClearDataArray();
			ColRowSelected=new ColRow(-1,-1);//my way of setting it to null.
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			//_widthMeas=LayoutManager.Scale(10);
			_widthTooth=LayoutManager.Scale(33);//3*(10+1)
			_widthLeft=LayoutManager.Scale(72);	//increased from 65 to hold non English-US short dates (dd-mmm-yyyy)
			_heightProb=LayoutManager.Scale(15);
			_heightShort=LayoutManager.Scale(12);
			_heightTooth=LayoutManager.Scale(16);
			FillPosArrays();
			Invalidate();
		}

		///<summary>Sets the user editable colors</summary>
		public void SetColors(){
			if(Defs.GetDictIsNull()) {
				_colorBlood=Color.FromArgb(240,20,20);
				_colorSuppuration=Color.FromArgb(255,160,0);
				_colorPlaque=Color.FromArgb(240,20,20);
				_colorCalc=Color.FromArgb(255,160,0);
			}
			else {
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
				_colorBlood=listDefs[(int)DefCatMiscColors.PerioBleeding].ItemColor;
				_colorSuppuration=listDefs[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
				_colorPlaque=listDefs[(int)DefCatMiscColors.PerioPlaque].ItemColor;
				_colorCalc=listDefs[(int)DefCatMiscColors.PerioCalculus].ItemColor;
			}
		}

		///<summary></summary>
		protected override void OnPaint(PaintEventArgs e){
			//base.OnPaint (e);
			//Graphics g=e.Graphics;
			DrawBackground(e);
			DrawSkippedTeeth(e);
			DrawSelectedTeeth(e);
			DrawCurCell(e);
			DrawGridlines(e);
			DrawText(e);
			//DrawTempDots(e);
		}
	
		//private void DrawTempDots(System.Windows.Forms.PaintEventArgs e){
			//Graphics g=e.Graphics;
			//for(int i=0;i<TopCoordinates.Length;i++){
			//	g.DrawLine(Pens.Red,20,TopCoordinates[24],25,TopCoordinates[24]);
			//}
		//}

		private void DrawBackground(System.Windows.Forms.PaintEventArgs e){
			Graphics g=e.Graphics;
			int top;
			int bottom;
			g.FillRectangle(SystemBrushes.Control,ClientRectangle);
			g.FillRectangle(Brushes.White,0,0,_widthShowing,_heightShowing);
			int yPos1=1+_rowsProbing*(_heightProb+1);
			int yPos2=yPos1+(_perioSequenceTypeArray[0].Length-_rowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(_colorBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+_heightTooth+1;
			yPos2=yPos1+(_perioSequenceTypeArray[1].Length-_rowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(_colorBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+_rowsProbing*(_heightProb+1)+1+_rowsProbing*(_heightProb+1);
			yPos2=yPos1+(_perioSequenceTypeArray[2].Length-_rowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(_colorBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+_heightTooth+1;
			yPos2=yPos1+(_perioSequenceTypeArray[3].Length-_rowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(_colorBackShort),0,top,_widthShowing,bottom-top);
		}

		///<summary>Draws the greyed out background for the skipped teeth.</summary>
		private void DrawSkippedTeeth(System.Windows.Forms.PaintEventArgs e){
			if(_listSkippedTeeth==null || _listSkippedTeeth.Count==0)
				return;
			Graphics g=e.Graphics;
			float xLoc=0;
			float yLoc=0;
			float height=0;
			float width=0;
			RectangleF retangleFBounds;//used in the loop to represent the bounds of the entire tooth to be greyed
			for(int i=0;i<_listSkippedTeeth.Count;i++){
				if((int)_listSkippedTeeth[i]<17){//max tooth
					xLoc=1+_widthLeft+1+((int)_listSkippedTeeth[i]-1)*_widthTooth;
					//xLoc=1+Wleft+1+(col-1)*(Wmeas+1);
					yLoc=1;
					height=_rowPosArray[GetTableRow(1,_perioSequenceTypeArray[1].Length-1)]-yLoc+_heightProb;
					width=_widthTooth;
				}
				else{//mand tooth
					xLoc=1+_widthLeft+1+(33-(int)_listSkippedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(2,_perioSequenceTypeArray[2].Length-1)];
					height=_rowPosArray[GetTableRow(3,_perioSequenceTypeArray[3].Length-1)]-yLoc+_heightProb;
					width=_widthTooth;
				}
				retangleFBounds=new RectangleF(xLoc,yLoc,width,height);
				int top=(int)retangleFBounds.Top;
				int bottom=(int)retangleFBounds.Bottom;
				int left=(int)retangleFBounds.Left;
				int right=(int)retangleFBounds.Right;
				//test clipping rect later
				//MessageBox.Show(bounds.ToString());
				g.FillRectangle(new SolidBrush(_colorBackShort),left,top
					,right-left,bottom-top);
			}
		}

		///<summary>Draws the highlighted background for selected teeth(not used very often unless user has been clicking on tooth numbers in preparation for setting skipped teeth. Then, highlighting goes away.</summary>
		private void DrawSelectedTeeth(System.Windows.Forms.PaintEventArgs e){
			if(selectedTeeth==null || selectedTeeth.Count==0)
				return;
			Graphics g=e.Graphics;
			float xLoc=0;
			float yLoc=0;
			float height=0;
			float width=0;
			RectangleF rectangleFBounds;//used in the loop to represent the bounds to be greyed
			for(int i=0;i<selectedTeeth.Count;i++){
				if((int)selectedTeeth[i]<17){//max tooth
					xLoc=1+_widthLeft+1+((int)selectedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(getMaxillary:true)];
					height=_heightTooth;
					width=_widthTooth;
				}
				else{//mand tooth
					xLoc=1+_widthLeft+1+(33-(int)selectedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(getMaxillary:false)];
					height=_heightTooth;
					width=_widthTooth;
				}
				rectangleFBounds=new RectangleF(xLoc,yLoc,width,height);
				int top=(int)rectangleFBounds.Top;
				int bottom=(int)rectangleFBounds.Bottom;
				int left=(int)rectangleFBounds.Left;
				int right=(int)rectangleFBounds.Right;
				//test clipping rect later
				g.FillRectangle(new SolidBrush(_colorHighlight),left,top
					,right-left,bottom-top);
			}
		}

		///<summary>Draws the highlighted background for the current cell.</summary>
		private void DrawCurCell(System.Windows.Forms.PaintEventArgs e){
			if(ColRowSelected.Col==-1){
				return;
			}
			if(!AllowPerioEdit) {
				return;
			}
			Graphics g=e.Graphics;
			RectangleF rectangleFBounds=GetBounds(ColRowSelected.Col,ColRowSelected.Row);
			if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.Probing){
				rectangleFBounds=new RectangleF(rectangleFBounds.X,rectangleFBounds.Y+_heightProb-_heightShort,
					rectangleFBounds.Width,_heightShort);
			}
			int top=(int)rectangleFBounds.Top;
			int bottom=(int)rectangleFBounds.Bottom;
			int left=(int)rectangleFBounds.Left;
			int right=(int)rectangleFBounds.Right;
			if(e.ClipRectangle.Bottom>=rectangleFBounds.Top && e.ClipRectangle.Top<=rectangleFBounds.Bottom
				&& e.ClipRectangle.Right>=rectangleFBounds.Left && e.ClipRectangle.Left<=rectangleFBounds.Right)
			{//if the clipping rect includes any part of the CurCell
				if(e.ClipRectangle.Bottom<=bottom){
					bottom=e.ClipRectangle.Bottom;
				}
				if(e.ClipRectangle.Top>=top){
					top=e.ClipRectangle.Top;
				}
				if(e.ClipRectangle.Right<=right){
					right=e.ClipRectangle.Right;
				}
				if(e.ClipRectangle.Left>=left){
					left=e.ClipRectangle.Left;
				}
				g.FillRectangle(new SolidBrush(_colorHighlight),left,top
					,right-left,bottom-top);
			}
		}

		private void DrawGridlines(System.Windows.Forms.PaintEventArgs e){
			Graphics g=e.Graphics;
			Pen penMain=new Pen(_colorLinesMain);
			Pen penMinor=new Pen(_colorLinesMinor);
			//Lines are generally drawn across top or left of each row or column
			//horizontal minor. Black will draw on top of them where needed.
			for(int i=0;i<_rowPosArray.Length;i++){
				g.DrawLine(penMinor,0,(int)_rowPosArray[i],_widthShowing,(int)_rowPosArray[i]);
			}
			//vertical
			g.DrawLine(penMain,0,0,0,_heightShowing);
			for(int i=1;i<_colPosArray.Length;i+=3){
				if(i==1 || i==25){
					g.DrawLine(penMain,_colPosArray[i],0,_colPosArray[i],_heightShowing);
				}
				else{
					g.DrawLine(penMinor,_colPosArray[i],0,_colPosArray[i],_heightShowing);
				}
			}
			g.DrawLine(penMain,_widthShowing,0,_widthShowing,_heightShowing);
			//Horizontal Main
			g.DrawLine(penMain,0,0,_widthShowing,0);
			int y=(int)_rowPosArray[GetTableRow(0,0,PerioSequenceType.MGJ)];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,0,PerioSequenceType.Mobility)+1];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,1,PerioSequenceType.Furcation)];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,1,PerioSequenceType.GingMargin)+1];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(1,_perioSequenceTypeArray[1].Length)];
			using(Pen penHeavy=new Pen(Color.Black,2f)){
				g.DrawLine(penHeavy,0,y,_widthShowing,y);
			}
			y=(int)_rowPosArray[GetTableRow(0,2,PerioSequenceType.MGJ)];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,2,PerioSequenceType.Furcation)+1];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,3,PerioSequenceType.Mobility)];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			y=(int)_rowPosArray[GetTableRow(0,3,PerioSequenceType.MGJ)+1];
			g.DrawLine(penMain,0,y,_widthShowing,y);
			g.DrawLine(penMain,0,_heightShowing,_widthShowing,_heightShowing);
			penMain.Dispose();
			penMinor.Dispose();
		}

		private void DrawText(System.Windows.Forms.PaintEventArgs e){
			//Graphics g=e.Graphics;
			//int dataL=DataArray.GetLength(1);
			for(int i=0;i<_perioCellArray.GetLength(1);i++){//loop through all rows in the table
				DrawRow(i,e);
			}
		}

		private void DrawRow(int rowIndex,PaintEventArgs e){
			Graphics g=e.Graphics;
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;//.AntiAlias;
			//e.Graphics.PixelOffsetMode=PixelOffsetMode.HighQuality;
			//e.Graphics.TextRenderingHint=TextRenderingHint.AntiAlias;
			Font font;
			Color colorText;
			StringFormat stringFormat=new StringFormat();
			RectangleF rectangleF;
			bool drawOld;
			int redThresh=0;
			int cellValue=0;
			for(int i=0;i<_perioCellArray.GetLength(0);i++){//loop through all columns in the row
				rectangleF=GetBounds(i,rowIndex);
				font=(Font)Font.Clone();
				//font=new Font("Arial",8,FontStyle.Regular);
				colorText=Color.Black;
				//test for clip later
				if(i==0){//first column, dates and row labels
					float x=rectangleF.Right-g.MeasureString(_perioCellArray[i,rowIndex].Text,font).Width-1;
					float y=rectangleF.Top-1;
					if(GetSection(rowIndex)!=-1 && _perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]==PerioSequenceType.Probing){//date
						y+=LayoutManager.ScaleF(3);
					}
					e.Graphics.DrawString(_perioCellArray[i,rowIndex].Text,font,new SolidBrush(colorText),x,y);
					continue;
				}
				else if(GetSection(rowIndex)==-1){//tooth row
					//Debug.WriteLine("row:"+row+" col:"+i);
					font=new Font(Font,FontStyle.Bold);
					stringFormat.Alignment=StringAlignment.Center;
					rectangleF=new RectangleF(rectangleF.X,rectangleF.Y+2,rectangleF.Width,rectangleF.Height);
					e.Graphics.DrawString(_perioCellArray[i,rowIndex].Text,font,
						new SolidBrush(colorText),rectangleF,stringFormat);
					//e.Graphics.DrawString(DataArray[i,row].Text,Font,Brushes.Black,rect);
					continue;
				}
				stringFormat.Alignment=StringAlignment.Center;//center left/right
				if(_perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]==PerioSequenceType.Probing){
					if((_perioCellArray[i,rowIndex].Bleeding & BleedingFlags.Plaque) > 0){
						e.Graphics.FillRectangle(new SolidBrush(_colorPlaque),rectangleF.X+0,rectangleF.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((_perioCellArray[i,rowIndex].Bleeding & BleedingFlags.Calculus) > 0){
						e.Graphics.FillRectangle(new SolidBrush(_colorCalc),rectangleF.X+LayoutManager.ScaleF(2.5f),rectangleF.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((_perioCellArray[i,rowIndex].Bleeding & BleedingFlags.Blood) > 0){
						e.Graphics.FillRectangle(new SolidBrush(_colorBlood),rectangleF.X+LayoutManager.ScaleF(5f),rectangleF.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((_perioCellArray[i,rowIndex].Bleeding & BleedingFlags.Suppuration) > 0){
						e.Graphics.FillRectangle(new SolidBrush(_colorSuppuration),rectangleF.X+LayoutManager.ScaleF(7.5f),rectangleF.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					rectangleF=new RectangleF(rectangleF.X,rectangleF.Y+4,rectangleF.Width,rectangleF.Height);
				}
				if((_perioCellArray[i,rowIndex].Text==null || _perioCellArray[i,rowIndex].Text=="")
						&& (_perioCellArray[i,rowIndex].OldText==null || _perioCellArray[i,rowIndex].OldText=="")){
					continue;//no text to draw
				}
				if(_perioCellArray[i,rowIndex].Text==null || _perioCellArray[i,rowIndex].Text==""){//No data recorded by user for the current cell. Display the old text from the previous perio exam.
					drawOld=true;
					cellValue=PIn.Int(_perioCellArray[i,rowIndex].OldText);
					if(cellValue>100) {//used for negative numbers
						cellValue=100-cellValue;//i.e. 100-103 = -3
					}
					colorText=Color.Gray;
				}
				else{
					drawOld=false;
					cellValue=PIn.Int(_perioCellArray[i,rowIndex].Text);
					if(cellValue>100) {//used for negative numbers
						cellValue=100-cellValue;//i.e. 100-103 = -3
					}
					colorText=Color.Black;
					if(!AllowPerioEdit) {
						colorText=Color.Gray;
					}
				}
				//test for red
				switch(_perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]){
					case PerioSequenceType.Probing:
						redThresh=PrefC.GetInt(PrefName.PerioRedProb);
						break;
					case PerioSequenceType.MGJ:
						redThresh=PrefC.GetInt(PrefName.PerioRedMGJ);
						break;
					case PerioSequenceType.GingMargin:
						redThresh=PrefC.GetInt(PrefName.PerioRedGing);
						break;
					case PerioSequenceType.CAL:
						redThresh=PrefC.GetInt(PrefName.PerioRedCAL);
						break;
					case PerioSequenceType.Furcation:
						redThresh=PrefC.GetInt(PrefName.PerioRedFurc);
						break;
					case PerioSequenceType.Mobility:
						redThresh=PrefC.GetInt(PrefName.PerioRedMob);
						break;
				}
				if((_perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]
					==PerioSequenceType.MGJ && cellValue<=redThresh)
					||(_perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]
					!=PerioSequenceType.MGJ && cellValue>=redThresh))
				{
					if(drawOld) {
						colorText=_colorOldTextRed;
					}
					else {
						colorText=_colorRedText;
						if(!AllowPerioEdit) {
							colorText=_colorOldTextRed;
						}
					}
					font=new Font(Font,FontStyle.Bold);
				}
				//if number is two digits:
				if(cellValue>9){
					font=new Font("SmallFont",LayoutManager.ScaleF(7));
					rectangleF=new RectangleF(rectangleF.X-2,rectangleF.Y+1,rectangleF.Width+5,rectangleF.Height);
				}
				//if number is negative. "+" symbol is wider than "1" symbol so hand it seperately here.
				if(cellValue<0) {
					font=new Font("SmallFont",LayoutManager.ScaleF(7));
					rectangleF=new RectangleF(rectangleF.X-3,rectangleF.Y+1,rectangleF.Width+5,rectangleF.Height);//shift text left, 1 more pixel than usual.
				}
				//e.Graphics.DrawString(cellValue.ToString(),Font,Brushes.Black,rect);
				if((_perioSequenceTypeArray[GetSection(rowIndex)][GetSectionRow(rowIndex)]==PerioSequenceType.GingMargin)) {
					e.Graphics.DrawString(cellValue.ToString().Replace('-','+'),font,new SolidBrush(colorText),rectangleF.X,rectangleF.Y);//replace '-' with '+' for Gingival Margin Hyperplasia
				}
				else {
					e.Graphics.DrawString(cellValue.ToString(),font,new SolidBrush(colorText),rectangleF.X,rectangleF.Y);
				}
			}//i col
		}

		///<summary>Gets the bounds for a single cell.</summary>
		private RectangleF GetBounds(int col,int row){
			float xLoc=_colPosArray[col];

			/*if(col==0){
				xLoc=0;
			}
			else{
				xLoc=1+_widthLeft+1+(col-1)*(_widthTooth/3f);
				//xLoc=1+_widthLeft+1+(col-1)*(_widthMeas+1);
			}*/
			if(GetSection(row)==-1){//tooth row
				xLoc-=_widthTooth/3f-1;
				//xLoc-=_widthMeas;
			}
			float h=0;
			//if(row==24){
			//	MessageBox.Show(RowTypes[GetSection(row)][GetSectionRow(row)].ToString());
				//MessageBox.Show(GetSection(row).ToString()+","+GetSectionRow(row).ToString());
			//}
			//try{
			if(GetSection(row)==-1){//tooth row
				h=_heightTooth;
			}
			else if(_perioSequenceTypeArray[GetSection(row)][GetSectionRow(row)]==PerioSequenceType.Probing){//probing
				h=_heightProb;
			}
			else{
				h=_heightShort+1;//added the 1 so that a lower case y can drop a little lower.
			}
			//}
			//catch{
			//	MessageBox.Show(row.ToString());
			//}
			float w;
			if(GetSection(row)==-1){//tooth row
				w=_widthTooth-3;
				//w=_widthMeas*3;
			}
			else if(col==0){
				w=_widthLeft+1;
			}
			else{
				w=_widthTooth/3f-1;
				//w=_widthMeas;
			}
			//try{
			//if(row==10)
			//	MessageBox.Show(TopCoordinates[row].ToString());
			return new RectangleF(xLoc,_rowPosArray[row],w,h);
			//}
			//catch{
				//MessageBox.Show(row.ToString());
			//}
			//return new RectangleF(0,0,70,20);
		}
		
		///<summary>Fills _rowPosArray and _colPosArray.</summary>
		private void FillPosArrays(){
			_rowPosArray=new float[_perioCellArray.GetLength(1)];
			//int curRow=0;
			int yPos=0;
			//U facial
			for(int i=_perioSequenceTypeArray[0].Length-1;i>=0;i--){
				_rowPosArray[GetTableRow(0,i)]=yPos;
				//MessageBox.Show(GetTableRow(0,i));
				if(_perioSequenceTypeArray[0][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			_rowPosArray[GetTableRow(getMaxillary:true)]=yPos;
			yPos+=_heightTooth+1;
			//upper lingual
			for(int i=0;i<_perioSequenceTypeArray[1].Length;i++){
				_rowPosArray[GetTableRow(1,i)]=yPos;
				if(_perioSequenceTypeArray[1][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			yPos+=1;//makes a double line between u and L
			//lower lingual
			//MessageBox.Show(GetTableRow(2,3).ToString());
			for(int i=_perioSequenceTypeArray[2].Length-1;i>=0;i--){
				_rowPosArray[GetTableRow(2,i)]=yPos;
				if(_perioSequenceTypeArray[2][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			_rowPosArray[GetTableRow(getMaxillary:false)]=yPos;
			yPos+=_heightTooth+1;
			//lower facial
			for(int i=0;i<_perioSequenceTypeArray[3].Length;i++){
				_rowPosArray[GetTableRow(3,i)]=yPos;
				if(_perioSequenceTypeArray[3][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			_heightShowing=yPos;
			_colPosArray=new float[49];
			_colPosArray[0]=0;
			int colPos=1+_widthLeft+1;//We loop through the 16 teeth, since they are ints
			for(int i=0;i<16;i++){//looping 0 to 15
				//there are 3 positions per tooth
				//We are filling positions 1 through 49
				_colPosArray[i*3+1]=colPos;
				_colPosArray[i*3+2]=colPos+_widthTooth/3f;
				_colPosArray[i*3+3]=colPos+_widthTooth*2f/3f;
				colPos+=_widthTooth;
			}
			_widthShowing=colPos-1;
		}

		///<summary>Loads data from the PerioMeasures lists into the visible grid.</summary>
		public void LoadData(bool selectCell=true){
			ClearDataArray();
			selectedTeeth=new ArrayList();
			_listSkippedTeeth=new List<int>();
			//reset the list of modified teeth
			ListPerioMeasureItemsChanged=new List<PerioMeasureItem>();
			if(_idxExamSelected==-1){
				return;
			}
			FillDates();
			ColRow colRowSelected;
			PerioCell perioCellSelected;
			//int examI=selectedExam;
			string cellText="";
			int cellBleed=0;
			for(int examI=0;examI<=_idxExamSelected;examI++){//exams, earliest through current
				for(int seqI=0;seqI<PerioMeasures.List.GetLength(1);seqI++){//sequences
					for(int toothI=1;toothI<PerioMeasures.List.GetLength(2);toothI++){//measurements
						if(PerioMeasures.List[examI,seqI,toothI]==null){//.PerioMeasureNum==0)
							continue;//no measurement for this seq and tooth
						}
						for(int surfI=0;surfI<Enum.GetNames(typeof(PerioSurf)).Length;surfI++){//surfaces(6or7)
							if(seqI==(int)PerioSequenceType.SkipTooth){
								//There is nothing in the data array to fill because it is not user editable.
								if(surfI!=(int)PerioSurf.None){
									continue;
								}
								if(examI!=_idxExamSelected){//only mark skipped teeth for current exam
									continue;
								}
								if(PerioMeasures.List[examI,seqI,toothI].ToothValue==1){
									_listSkippedTeeth.Add(toothI);
								}
								continue;
							}
							if(seqI==(int)PerioSequenceType.Mobility){
								if(surfI!=(int)PerioSurf.None){
									continue;
								}
								colRowSelected=GetCell(examI,PerioMeasures.List[examI,seqI,toothI].SequenceType,toothI,PerioSurf.B);
								perioCellSelected=GetPerioCell(colRowSelected,setText:false);
								cellText=PerioMeasures.List[examI,seqI,toothI].ToothValue.ToString();
								if(cellText=="-1"){
									cellText="";
								}
								if(examI==_idxExamSelected) {
									perioCellSelected.Text=cellText;
									_perioCellArray[colRowSelected.Col,colRowSelected.Row]=perioCellSelected;
								}
								else {
									perioCellSelected.OldText=cellText;
									_perioCellArray[colRowSelected.Col,colRowSelected.Row]=perioCellSelected;
								}
								continue;
							}
							if(seqI==(int)PerioSequenceType.Bleeding){
								if(surfI==(int)PerioSurf.None){
									continue;
								}
								colRowSelected=GetCell(examI,PerioSequenceType.Probing,toothI,(PerioSurf)surfI);
								if(colRowSelected.Col==-1 || colRowSelected.Row==-1)
									//if there are more exams than will fit.
									continue;
								switch(surfI){
									case (int)PerioSurf.B:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].Bvalue;
										break;
									case (int)PerioSurf.DB:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].DBvalue;
										break;
									case (int)PerioSurf.DL:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].DLvalue;
										break;
									case (int)PerioSurf.L:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].Lvalue;
										break;
									case (int)PerioSurf.MB:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].MBvalue;
										break;
									case (int)PerioSurf.ML:
										cellBleed=PerioMeasures.List[examI,seqI,toothI].MLvalue;
										break;
								}
								perioCellSelected=GetPerioCell(colRowSelected,setText:false);
								if(cellBleed==-1) {//this shouldn't happen, but just in case
									cellBleed=0;
								}
								perioCellSelected.Bleeding=(BleedingFlags)cellBleed;
								_perioCellArray[colRowSelected.Col,colRowSelected.Row]=perioCellSelected;
								continue;
							}
							colRowSelected=GetCell(examI,PerioMeasures.List[examI,seqI,toothI].SequenceType,toothI,(PerioSurf)surfI);
							if(colRowSelected.Col==-1 || colRowSelected.Row==-1)
								//for instance, MGJ on Palatal doesn't exist.
								//also, on probing rows, if there are more exams than will fit.
								continue;
							switch(surfI){
								case (int)PerioSurf.B:
									cellText=PerioMeasures.List[examI,seqI,toothI].Bvalue.ToString();
									break;
								case (int)PerioSurf.DB:
									cellText=PerioMeasures.List[examI,seqI,toothI].DBvalue.ToString();
									break;
								case (int)PerioSurf.DL:
									cellText=PerioMeasures.List[examI,seqI,toothI].DLvalue.ToString();
									break;
								case (int)PerioSurf.L:
									cellText=PerioMeasures.List[examI,seqI,toothI].Lvalue.ToString();
									break;
								case (int)PerioSurf.MB:
									cellText=PerioMeasures.List[examI,seqI,toothI].MBvalue.ToString();
									break;
								case (int)PerioSurf.ML:
									cellText=PerioMeasures.List[examI,seqI,toothI].MLvalue.ToString();
									break;
							}//switch surfI
							perioCellSelected=GetPerioCell(colRowSelected,setText:false);
							if (cellText=="-1") {//seqI==2 means it is gingival margin.
								cellText="";
							}
							if(examI==_idxExamSelected) {
								perioCellSelected.Text=cellText;
								_perioCellArray[colRowSelected.Col,colRowSelected.Row]=perioCellSelected;
							}
							else {
								perioCellSelected.OldText=cellText;
								_perioCellArray[colRowSelected.Col,colRowSelected.Row]=perioCellSelected;
							}
							//calculate CAL. All ging will have been done before any probing.
							if(seqI==(int)PerioSequenceType.Probing){
								CalculateCAL(colRowSelected,alsoInvalidate:false);
							}
						}//for surfI
					}//for toothI
				}//for seqI
			}//for examI
			//Start in the very first cell on the first tooth and loop through teeth until we come across one that is not missing.
			if(!selectCell){
				return;
			}
			ColRowSelected=new ColRow(1,GetTableRow(_idxExamSelected,0,PerioSequenceType.Probing));
			ChangeDirectionLeft();//Always start looping to the left.
			if(_listSkippedTeeth.Count==32){
				return;
			}
			int intTooth=GetToothNumCur(GetSection(ColRowSelected.Row));
			while(_listSkippedTeeth.Contains(intTooth)){
				AdvanceCell();//Advance forward 3 times, since there are 3 measurements per tooth.
				AdvanceCell();
				AdvanceCell();
				intTooth=GetToothNumCur(GetSection(ColRowSelected.Row));
			}
		}

		///<summary>Used in LoadData.</summary>
		private void FillDates(){
			int intTableRow;
			for(int examI=0;examI<_idxExamSelected+1;examI++){//-ProbingOffset;examI++){
				for(int sectionI=0;sectionI<4;sectionI++){
					intTableRow=GetTableRow(examI,sectionI,PerioSequenceType.Probing);
					if(intTableRow==-1){
						continue;
					}
					_perioCellArray[0,intTableRow].Text
						=PerioExams.ListExams[examI].ExamDate.ToShortDateString();
						//=PerioExams.List[examI+ProbingOffset].ExamDate.ToShortDateString();
				}
			}
		}

		///<summary>Used in LoadData.</summary>
		private ColRow GetCell(int examIndex,PerioSequenceType perioSequenceType,int intTooth,PerioSurf perioSurf){
			int col=0;
			int row=0;
			if(intTooth<=16){
				col=(intTooth*3)-2;//left-most column
				if(perioSurf==PerioSurf.B || perioSurf==PerioSurf.L){
					col++;
				}
				if(intTooth<=8){
					if(perioSurf==PerioSurf.MB || perioSurf==PerioSurf.ML)
						col+=2;
				}
				else{//9-16
					if(perioSurf==PerioSurf.DB || perioSurf==PerioSurf.DL)
						col+=2;
				}
			}
			else{//17-32
				col=((33-intTooth)*3)-2;//left-most column
				if(perioSurf==PerioSurf.B || perioSurf==PerioSurf.L){
					col++;
				}
				if(intTooth>=25){
					if(perioSurf==PerioSurf.MB || perioSurf==PerioSurf.ML)
						col+=2;
				}
				else{//17-24
					if(perioSurf==PerioSurf.DB || perioSurf==PerioSurf.DL)
						col+=2;
				}
			}
			int section;
			if(intTooth<=16){
				if(perioSurf==PerioSurf.MB || perioSurf==PerioSurf.B || perioSurf==PerioSurf.DB){
					section=0;
				}
				else{//Lingual
					section=1;
				}
			}
			else{//17-32
				if(perioSurf==PerioSurf.MB || perioSurf==PerioSurf.B || perioSurf==PerioSurf.DB){
					section=3;
				}
				else{//Lingual
					section=2;
				}
			}
			row=GetTableRow(examIndex,section,perioSequenceType);
			return new ColRow(col,row);
		}

		///<summary>Returns the starting ColRow(position) for the passed in tooth.</summary>
		public ColRow GetAutoAdvanceColRow(int intTooth,bool isFacial,PerioSequenceType perioSequenceType,bool isReverse) {
			int x=-1;
			int y=-1;
			if(isFacial) {
				if(intTooth.Between(1,16)) {
					x=(intTooth)*3-(isReverse ? 0 : 2);//left most column. if isReverse, than right most column.
					if(perioSequenceType==PerioSequenceType.Mobility) {
						x+=(isReverse ? -1 : 1);//Middle column
					}
				}
				else {//Tooth 17-32
					x=(32-intTooth)*3+ (isReverse ? 1 : 3);//Right most column. if isReverse, get left most column
					if(perioSequenceType==PerioSequenceType.Mobility) {
						x+=(isReverse ? 1 : -1);//middle column
					}
				}
			}
			else {//Lingual
				if(intTooth.Between(1,16)) {
					x=(intTooth)*3 + (isReverse ? -2 : 0);//Right most column. If isReverse, get left most column.
				}
				else {//Tooth 17-32
					x=(32-intTooth)*3 + (isReverse ? 3 : 1);//Left most column. If isReverse, get right most column.
				}
			}
			y=GetTableRow(IdxExamSelected,GetSection(intTooth,isFacial),perioSequenceType);
			return new ColRow(x,y);
		}

		public PerioSequenceType GetSequenceForColRow(ColRow colRow) {
			return _perioSequenceTypeArray[GetSection(colRow.Row)][GetSectionRow(colRow.Row)];
		}

		///<summary>Saves the cur exam measurements to the db by looping through each tooth and deciding whether the data for that tooth has changed.  If so, it either updates or inserts a measurement.  It won't delete a measurement if all values for that tooth are cleared, but just sets that measurement to all -1's.</summary>
		public void SaveCurExam(PerioExam perioExam) {
			PerioMeasure perioMeasure;
			if(ListPerioMeasureItemsChanged.Count==0) {
				return;//no changes. No need to insert/update periomeasures.
			}
			PerioExam perioExamFromDb=PerioExams.GetOnePerioExam(perioExam.PerioExamNum);
			if(perioExamFromDb==null || perioExam.DateTMeasureEdit!=perioExamFromDb.DateTMeasureEdit) {
				//something has changed
				MsgBox.Show(this,"This perio exam has been altered by another user. A new exam has been created and saved.");
				perioExam.PerioExamNum=0;
				PerioExams.Insert(perioExam);
				List<PerioExam> listPerioExams=PerioExams.GetExamsList(perioExam.PatNum);
				PerioMeasures.Refresh(perioExam.PatNum,listPerioExams);
				//isNewExam=true;
				perioExam.IsNew=true;
			}
			//continue saving periomeasures.Update exam with new datetime.
			if(!perioExam.IsNew) {
				perioExam.DateTMeasureEdit=MiscData.GetNowDateTime();
				PerioExams.Update(perioExam);
			}
			//In case a tooth was toggled multiple times
			ListPerioMeasureItemsChanged=ListPerioMeasureItemsChanged.Distinct().ToList();
			//If a tooth is marked as bleeding, it will only have a PerioSequence.Probing entry in HasChanged.	We need to add a HasChanged entry for
			//Bleeding as well.
			List<PerioMeasureItem> listPerioMeasureItemsBleeding=ListPerioMeasureItemsChanged.FindAll(x => x.SeqType==PerioSequenceType.Probing)
				.Select(x => new PerioMeasureItem(PerioSequenceType.Bleeding,x.ToothNum)).ToList();
			ListPerioMeasureItemsChanged.AddRange(listPerioMeasureItemsBleeding);
			for(int i=0;i<ListPerioMeasureItemsChanged.Count();i++){
				PerioSequenceType perioSequenceType=ListPerioMeasureItemsChanged[i].SeqType;
				int toothI=ListPerioMeasureItemsChanged[i].ToothNum;
				//new measurement
				if(PerioMeasures.List[_idxExamSelected,(int)perioSequenceType,toothI]==null || perioExam.IsNew) {//.PerioMeasureNum==0){
					//MessageBox.Show(toothI.ToString());
					perioMeasure=new PerioMeasure();
					perioMeasure.PerioExamNum=perioExam.PerioExamNum;
					perioMeasure.SequenceType=perioSequenceType;
					perioMeasure.IntTooth=toothI;
				}
				else{
					perioMeasure=PerioMeasures.List[_idxExamSelected,(int)perioSequenceType,toothI];
					//PerioExam
					//Sequence
					//tooth
				}
				if(perioSequenceType==PerioSequenceType.Mobility || perioSequenceType==PerioSequenceType.SkipTooth){
					perioMeasure.MBvalue=-1;
					perioMeasure.Bvalue=-1;
					perioMeasure.DBvalue=-1;
					perioMeasure.MLvalue=-1;
					perioMeasure.Lvalue=-1;
					perioMeasure.DLvalue=-1;
					if(perioSequenceType==PerioSequenceType.Mobility){
						perioMeasure.ToothValue
							=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.B);
					}
					else{//skiptooth
						//skipped teeth are only saved when user marks them, not as part of regular saving.
					}
				}
				else if(perioSequenceType==PerioSequenceType.Bleeding){
					perioMeasure.ToothValue=-1;
					perioMeasure.MBvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.MB);
					perioMeasure.Bvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.B);
					perioMeasure.DBvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.DB);
					perioMeasure.MLvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.ML);
					perioMeasure.Lvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.L);
					perioMeasure.DLvalue
						=GetCellBleedValue(_idxExamSelected,toothI,PerioSurf.DL);
				}
				else{
					perioMeasure.ToothValue=-1;
					perioMeasure.MBvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.MB);
					perioMeasure.Bvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.B);
					perioMeasure.DBvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.DB);
					perioMeasure.MLvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.ML);
					perioMeasure.Lvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.L);
					perioMeasure.DLvalue
						=GetCellValue(_idxExamSelected,perioSequenceType,toothI,PerioSurf.DL);
				}
				//then to the database
				if(perioSequenceType==PerioSequenceType.Mobility && perioMeasure.ToothValue==-1 && !perioExam.IsNew) {
					PerioMeasures.Delete(perioMeasure);//-1 is an invalid value for mobility, keep Db bloat down
				}
				else if(PerioMeasures.List[_idxExamSelected,(int)perioSequenceType,toothI]==null || perioExam.IsNew) {
					PerioMeasures.Insert(perioMeasure);
				}
				else{
					PerioMeasures.Update(perioMeasure);
				}
			}//for seqI
		}

		///<summary>Used in SaveCurExam to retrieve data from grid to save it.</summary>
		private int GetCellValue(int examIndex,PerioSequenceType perioSequenceType,int intTooth,PerioSurf perioSurf){
			ColRow colRow=GetCell(examIndex,perioSequenceType,intTooth,perioSurf);
			if(colRow.Col==-1 || colRow.Row==-1){
				return -1;
			}
			//if(intTooth==4)
			//MessageBox.Show(DataArray[curCell.Col,curCell.Row].Text);
			if(_perioCellArray[colRow.Col,colRow.Row].Text==null || _perioCellArray[colRow.Col,colRow.Row].Text==""){
				//MessageBox.Show("empty");
				return -1;
			}
			//MessageBox.Show("full");
			return PIn.Int(_perioCellArray[colRow.Col,colRow.Row].Text);
		}

		public string GetCellText(int x,int y) {
			if(x==-1 || y==-1 || _perioCellArray[x,y].Text==null) {
				return "";
			}
			return _perioCellArray[x,y].Text;
		}

		///<summary>Used in SaveCurExam to retrieve data from grid to save it.</summary>
		private int GetCellBleedValue(int examIndex,int intTooth,PerioSurf perioSurf){
			ColRow colRow=GetCell(examIndex,PerioSequenceType.Probing,intTooth,perioSurf);
			if(colRow.Col==-1 || colRow.Row==-1){
				return 0;
			}
			return (int)_perioCellArray[colRow.Col,colRow.Row].Bleeding;
		}

		private void ClearDataArray(){
			//MessageBox.Show("clearing");
			_perioCellArray=new PerioCell[49,_perioSequenceTypeArray[0].Length+_perioSequenceTypeArray[1].Length
				+_perioSequenceTypeArray[2].Length+_perioSequenceTypeArray[3].Length+2];//the 2 is for the tooth cells.
			//int curX=0;
			int y=0;
			for(int section=0;section<4;section++){
				for(int i=0;i<_perioSequenceTypeArray[section].Length;i++){
					y=GetTableRow(section,i);
					switch(_perioSequenceTypeArray[section][i]){
						case PerioSequenceType.Mobility:
							_perioCellArray[0,y].Text=Lan.g(this,"Mobility");
							break;
						case PerioSequenceType.Furcation:
							_perioCellArray[0,y].Text=Lan.g(this,"Furc");
							break;
						case PerioSequenceType.CAL:
							_perioCellArray[0,y].Text=Lan.g(this,"auto CAL");
							break;
						case PerioSequenceType.GingMargin:
							_perioCellArray[0,y].Text=Lan.g(this,"Ging Marg");
							break;
						case PerioSequenceType.MGJ:
							_perioCellArray[0,y].Text=Lan.g(this,"MGJ");
							break;
						case PerioSequenceType.Probing:
							break;
						default:
							MessageBox.Show("Error in FillDataArray");
							break;
					}
				}
			}
			//draw tooth numbers
			y=GetTableRow(getMaxillary:true);
			try{
				for(int i=1;i<=16;i++){
					_perioCellArray[3*i-1,y].Text=Tooth.Display(i.ToString());
					if(IsImplant(i)) {
						_perioCellArray[3*i-1,y].Text+="i";
					}
				}
				y=GetTableRow(getMaxillary:false);
				for(int i=1;i<=16;i++){
					_perioCellArray[3*i-1,y].Text=Tooth.Display((33-i).ToString());
					if(IsImplant(33-i)) {
						_perioCellArray[3*i-1,y].Text+="i";
					}
				}
			}
			catch{
				//won't work in design mode
			}
		}

		///<summary>Returns true if the toothI passed in has ever had an implant before.</summary>
		public static bool IsImplant(int intTooth) {
			if(_listProcedures==null) {//Not initialized yet.
				_listProcedures=Procedures.Refresh(FormOpenDental.PatNumCur);
			}
			List<Procedure> listProceduresForTooth=_listProcedures.FindAll(x => x.ToothNum==intTooth.ToString() && x.ProcStatus.In(
				ProcStat.C,ProcStat.EC,ProcStat.EO));
			for(int i = 0;i<listProceduresForTooth.Count;i++) {
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresForTooth[i].CodeNum);
				if(procedureCode.PaintType==ToothPaintingType.Implant) {
					return true;
				}
			}
			return false;
		}

		///<summary>Used in GetCell during LoadData. Also used in AdvanceCell when looping to a new section.</summary>
		private int GetTableRow(int examIndex,int section,PerioSequenceType perioSequenceType){
			if(perioSequenceType==PerioSequenceType.Probing || perioSequenceType==PerioSequenceType.Bleeding){
				if(examIndex-_probingOffset<0)//older exam that won't fit.
					return -1;
				int sectionRow=examIndex-_probingOffset//correct for offset
					+_perioSequenceTypeArray[section].Length-_rowsProbing;//plus number of non-probing rows
				return GetTableRow(section,sectionRow);
			}
			//for types other than probing and bleeding, do a loop through the non-probing rows:
			for(int i=0;i<_perioSequenceTypeArray[section].Length-_rowsProbing;i++){
				if(_perioSequenceTypeArray[section][i]==perioSequenceType)
					return GetTableRow(section,i);
			}
			//MessageBox.Show("Error in GetTableRows: seqType not found");
			return -1;
		}

		private int GetTableRow(int section,int sectionRow){
			int retVal=0;
			if(section==0){
				retVal=_perioSequenceTypeArray[0].Length-sectionRow-1;
			}
			else if(section==1){
				retVal=_perioSequenceTypeArray[0].Length+1+sectionRow;
			}
			else if(section==2){
				retVal=_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length-sectionRow-1;
			}
			else
				retVal=_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length+1+sectionRow;
			return retVal;
		}

		///<summary>If true, then returns the row of the max teeth, otherwise mand.</summary>
		private int GetTableRow(bool getMaxillary){
			if(getMaxillary){
				return _perioSequenceTypeArray[0].Length;
			}
			return _perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length;
		}

		
		///<summary>Returns -1 if tooth row and not section row.  0 = Maxillary Facial, 1 = Maxillary Lingual, 2 = Mandible Facial, 3 = Mandible Lingual.
		///Used in GetBounds, DrawRow, and OnMouseDown.</summary>
		private int GetSection(int rowIndex){
			if(rowIndex<_perioSequenceTypeArray[0].Length) {//0 = Maxillary Facial
				return 0;
			}
			if(rowIndex==_perioSequenceTypeArray[0].Length){
				return -1;//max teeth
			}
			if(rowIndex<_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length) {//1 = Maxillary Lingual
				return 1;
			}
			if(rowIndex<_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length) {//2 = Mandible Facial
				return 2;
			}
			if(rowIndex==_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length) {//3 = Mandible Lingual
				return -1;//mand teeth
			}
			return 3;
		}

		///<summary>Returns the section for the passed in tooth. The section is used to determine the Y position of a ColRow.</summary>
		public int GetSection(int intTooth,bool isFacial) {
			int section;
			if(intTooth<=16) {
				if(isFacial) {
					section=0;//0 = Maxillary Facial
				}
				else {//Lingual
					section=1;//1 = Maxillary Lingual
				}
			}
			else {//17-32
				if(isFacial) {
					section=3;//3 = Mandible Lingual
				}
				else {//Lingual
					section=2;//2 = Mandible Facial
				}
			}
			return section;
		}

		///<summary>Returns -1 if a tooth row and not a section row.  Used in GetBounds,SetHasChanged, AdvanceCell, and DrawRow</summary>
		private int GetSectionRow(int tableRowIdx){
			if(tableRowIdx<_perioSequenceTypeArray[0].Length){
				return _perioSequenceTypeArray[0].Length-tableRowIdx-1;
			}
			//return 0;
			if(tableRowIdx==_perioSequenceTypeArray[0].Length){
				return -1;//max teeth
			}
			if(tableRowIdx<_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length){
				return tableRowIdx-_perioSequenceTypeArray[0].Length-1;
			}
			if(tableRowIdx<_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length){
				return _perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length-tableRowIdx-1;//-1?
			}
			if(tableRowIdx==_perioSequenceTypeArray[0].Length+1+_perioSequenceTypeArray[1].Length+_perioSequenceTypeArray[2].Length){
				return -1;//mand teeth
			}
			return tableRowIdx-_perioSequenceTypeArray[0].Length-1-_perioSequenceTypeArray[1].Length-_perioSequenceTypeArray[2].Length-1;//-1?
		}

		///<summary>Gets the current cell as a col,row based on the x-y pixel coordinate supplied.</summary>
		private ColRow GetCellFromPixel(int x,int y){
			int row=0;
			for(int i=0;i<_rowPosArray.Length;i++){
				if(y<_rowPosArray[i]){
					row=i-1;
					break;
				}
				if(i==_rowPosArray.Length-1){//last row
					row=i;
				}
			}
			if(y==-1){
				y=0;
			}
			int col=0;
			for(int i=0;i<_colPosArray.Length;i++){
				if(x<_colPosArray[i]){
					col=i-1;
					break;
				}
				if(i==_colPosArray.Length-1){//last col
					col=i;
				}
			}
			/*
			if(x<=_widthLeft+1){
				col=0;
			}
			else{
				//1+Wleft+1+(col-1)*(Wmeas+1);
				int toothCol=(x-_widthLeft-1)/_widthTooth-1;//0 through 15. This handles the rounding errors.
				col=(x-_widthLeft-1-(toothCol*_widthTooth))/(_widthTooth/3);
				//col=(int)Math.Floor(((double)(x-_widthLeft-1))/((double)(_widthMeas+1)))+1;
			}*/
			if(col==49){
				col=48;
			}
			return new ColRow(col,row);
		}

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e){
			if(!AllowPerioEdit) {
				return;
			}
			base.OnMouseDown(e);
			ColRow colRowNewCell=GetCellFromPixel(e.X,e.Y);
			if(colRowNewCell.Col==0){
				return;//Left column only for dates
			}
			int section=GetSection(colRowNewCell.Row);
			if(section==-1){//clicked on a toothNum
				int intTooth=(int)Math.Ceiling((double)colRowNewCell.Col/3);
				if(GetTableRow(false)==colRowNewCell.Row){//if clicked on mand
					intTooth=33-intTooth;
				}
				if(selectedTeeth.Contains(intTooth)){//tooth was already selected
					selectedTeeth.Remove(intTooth);
				}
				else{//tooth was not selected
					selectedTeeth.Add(intTooth);
				}
				Invalidate();//incomplete: just invalidate the area around the tooth num.
				return;
			}
			if(selectedTeeth.Count>0){//if not clicked on a toothnum, but teeth were selected,
				//then deselect all.
				//todo(some day): loop through each individually and only invalidate small area.
				selectedTeeth=new ArrayList();
				Invalidate();
			}
			int sectRowIdx=GetSectionRow(colRowNewCell.Row);
			if(sectRowIdx<0 || sectRowIdx>=_perioSequenceTypeArray[section].Length) {
				//User clicked on a tooth row, not a section row.
				return;
			}
			if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Probing){
				if(this._idxExamSelected-_probingOffset//correct for offset
					+_perioSequenceTypeArray[section].Length-_rowsProbing//plus non-probing rows
					!=sectRowIdx)
				{
					return;//not allowed to click on probing rows other than selectedRow
				}
			}
			else if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Mobility){
				if(Math.IEEERemainder(((double)colRowNewCell.Col+1),3) != 0){//{2,5,8,11};examples of acceptable cols
					return;//for mobility, not allowed to click on anything but B
				}
			}
			else if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.CAL){
				return;//never allowed to edit CAL
			}
			if(section==0)
				ChangeDirectionLeft();
			else if(section==1)
				ChangeDirectionRight();
			else if(section==2)
				ChangeDirectionRight();
			else if(section==3)
				ChangeDirectionLeft();
			SetNewCell(colRowNewCell.Col,colRowNewCell.Row);
			Focus();
		}

		///<summary></summary>
		protected override bool IsInputKey(Keys keyData) {
			if(keyData==Keys.Left
				|| keyData==Keys.Right
				|| keyData==Keys.Up
				|| keyData==Keys.Down)
				return true;
			return base.IsInputKey(keyData);
		}

		///<summary></summary>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(!AllowPerioEdit) {
				return;
			}
			if (_idxExamSelected == -1)
			{
				MessageBox.Show(Lan.g(this, "Please add or select an exam first in the list to the left."));
				return;
			}
			PerioCell perioCell=GetPerioCell(ColRowSelected,setText:false);
			//MessageBox.Show("key down");
			//e.Handled=true;
			//base.OnKeyDown (e);
			if(e.KeyValue>=96 && e.KeyValue<=105){//keypad 0 through 9
				if(e.Control){
					if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.GingMargin) {
						int val=(e.KeyValue-96);
						ButtonPressed((val==0?0:val+100));//0 if val==0, val+100 if val != 0
					}
					else { //general case
						ButtonPressed(e.KeyValue-96+10);
					}
				}
				else{
					ButtonPressed(e.KeyValue-96);
				}
			}
			else if(e.KeyValue>=48 && e.KeyValue<=57){//0 through 9
				if(e.Control) {
					if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.GingMargin) {//gm
						int val=(e.KeyValue-48);
						ButtonPressed((val==0?0:val+100));//0 if val==0, val+100 if val != 0
					}
					else { //general case
						ButtonPressed(e.KeyValue-48+10);
					}
				}
				else{
					ButtonPressed(e.KeyValue-48);
				}
			}
			else if(e.KeyCode==Keys.B){
				ButtonPressed("b");
			}
			else if(e.KeyCode==Keys.Space){
				ButtonPressed("b");
			}
			else if(e.KeyCode==Keys.S){
				ButtonPressed("s");
			}
			else if(e.KeyCode==Keys.P){
				ButtonPressed("p");
			}
			else if(e.KeyCode==Keys.C){
				ButtonPressed("c");
			}
			else if(e.KeyCode==Keys.J){
				ButtonPressed("j");
			}
			else if(e.KeyCode==Keys.G){
				ButtonPressed("g");
			}
			else if(e.KeyCode==Keys.F){
				ButtonPressed("f");
			}
			else if(e.KeyCode==Keys.M){
				ButtonPressed("m");
			}
			else if(e.KeyCode==Keys.OemPeriod){
				ButtonPressed(".");
			}
			else if(e.KeyCode==Keys.Back){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
							AdvanceCell(isReverse:true);
							ClearValue();
						}
					}
				else{
						AdvanceCell(isReverse:true);
						ClearValue();
					}
				}
			else if(e.KeyCode==Keys.Delete){
				ClearValue();
			}
			else if(e.KeyCode==Keys.Left){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
						if(EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {
							AdvanceCell(GetSection(ColRowSelected.Row).In(0,2));
						}
						else if(EnumCurrentDirection_==EnumCurrentDirection.Left) {
							AdvanceCell(isReverse:true);
						}
						else {
							AdvanceCell();
						}
					}
				}
				else{
					if(EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {
						AdvanceCell(GetSection(ColRowSelected.Row).In(0,2));
					}
					else if(EnumCurrentDirection_==EnumCurrentDirection.Right) {
						AdvanceCell();
					}
					else {
						AdvanceCell(isReverse:true);
					}
				}
			}
			else if(e.KeyCode==Keys.Right){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
						if(EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {
							AdvanceCell(GetSection(ColRowSelected.Row).In(1,3));
						}
						else if(EnumCurrentDirection_==EnumCurrentDirection.Right) {
							AdvanceCell(isReverse:true);
						}
						else {
							AdvanceCell();
						}
					}
				}
				else{
					if(EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {
						AdvanceCell(GetSection(ColRowSelected.Row).In(1,3));
					}
					else if(EnumCurrentDirection_==EnumCurrentDirection.Right) {
						AdvanceCell(isReverse:true);
					}
					else {
						AdvanceCell();
					}
				}
			}
			else if(e.KeyCode==Keys.Up) {
				KeyUp_Clicked(ColRowSelected);
			}
			else if(e.KeyCode==Keys.Down) {
				KeyDown_Clicked(ColRowSelected);
			}
			//else{
			//	return;
			//}
		}

		///<summary></summary>
		private void KeyDown_Clicked(ColRow colRowSelected) {
			if(!AllowPerioEdit) {
				return;
			}
			int colSelected=colRowSelected.Col;
			int rowDownOne=colRowSelected.Row+1;
			int idxMax=_rowPosArray.Length-1;
			for(int i=0;i<idxMax-colRowSelected.Row;i++) {//all of the rows below current row
				int rowNext=rowDownOne+i;
				int sectRowIdx=GetSectionRow(rowNext);
				int section=GetSection(rowNext);
				if(rowNext==-1 ||  rowNext==_rowPosArray.Length) {//Out of bounds.
					SetNewCell(colRowSelected.Col,colRowSelected.Row);
					break;
				}
				if(sectRowIdx==-1 && section==-1) {//Skip "teeth number" rows.  They have no section or sectRowIdx, so they will return as -1
					continue;
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.CAL){//Skip auto cal rows.
					continue;
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Probing){
					if(this._idxExamSelected-_probingOffset//correct for offset
						+_perioSequenceTypeArray[section].Length-_rowsProbing//plus non-probing rows
						!=sectRowIdx)
					{
						continue;
					}
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Mobility) {
					int colLeft=colSelected-1;
					int colRight=colSelected+1;
					if(Math.IEEERemainder(((double)colLeft+1),3)==0) {//Currently on the right col of tooth and need to move to the left to get to the middle
						colSelected=colLeft;
					}
					else if(Math.IEEERemainder(((double)colRight+1),3)==0) {//Currently on the left col of tooth and need to move to the right to get to the middle
						colSelected=colRight;
					}
				}
				SetNewCell(colSelected,rowNext);
				break;
			}
			Focus();
		}

		///<summary></summary>
		private void KeyUp_Clicked(ColRow colRowSelected) {
			if(!AllowPerioEdit) {
				return;
			}
			int colCol=colRowSelected.Col;
			int rowUpOne=colRowSelected.Row-1;
			for(int i=0;i<colRowSelected.Row;i++) {//all the rows above current row
				int rowPrevious=rowUpOne-i;
				int sectRowIdx=GetSectionRow(rowPrevious);
				int section=GetSection(rowPrevious);
				if(rowPrevious==-1 ||  rowPrevious==_rowPosArray.Length) {//Out of bounds.
					SetNewCell(colRowSelected.Col,colRowSelected.Row);
					break;
				}
				if(sectRowIdx==-1 && section==-1) {//Skip "teeth number" rows.  They have no section or sectRowIdx, so they will return as -1
					continue;
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.CAL){//Skip auto cal rows.
					continue;
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Probing){
					if(this._idxExamSelected-_probingOffset//correct for offset
						+_perioSequenceTypeArray[section].Length-_rowsProbing//plus non-probing rows
						!=sectRowIdx)
					{//not the probing row of current perio exam
						continue;
					}
				}
				if(_perioSequenceTypeArray[section][sectRowIdx]==PerioSequenceType.Mobility) {
					int colLeft=colCol-1;
					int colRight=colCol+1;
					if(Math.IEEERemainder(((double)colLeft+1),3)==0) {//Currently on the right col of tooth and need to go to the left
						colCol=colLeft;
					}
					else if(Math.IEEERemainder(((double)colRight+1),3)==0) {//Currently on the left col of tooth and need to go to the right
						colCol=colRight;
					}
				}
				SetNewCell(colCol,rowPrevious);
				break;
				//MsgBox.Show("CRS.Col="+colCol.ToString()+"\nCRS.Row="+colRowPrevious.ToString());//For debug.
			}
			Focus();
		}

		public void KeyPressed(KeyEventArgs e) {
			OnKeyDown(e);
		}
 
		///<summary>Accepts button clicks from window rather than the usual keyboard entry.  All validation MUST be done before the value is sent here.  Only valid values are b,s,p,c,j,g,f,m,or period (.). Numbers entered using overload.</summary>
		public void ButtonPressed(string keyValue){
			if(!AllowPerioEdit) {
				return;
			}
			if(ThreeAtATime){
				for(int i=0;i<3;i++)
					EnterValue(keyValue);
			}
			else
				EnterValue(keyValue);
		}

		///<summary>Accepts button clicks from window rather than the usual keyboard entry.  All validation MUST be done before the value is sent here.  Only valid values are numbers 0 through 19, and 101 to 109.</summary>
		public void ButtonPressed(int keyValue){
			if(ColRowSelected.Col==-1) {
				return;
			}
			if(!AllowPerioEdit) {
				return;
			}
			if(GingMargPlus && _perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.GingMargin) {
				//Possible values for keyValue are 0-19,101-109
				if(keyValue<100) {
					keyValue=keyValue%10;//in case the +10 was down when the number was pressed on the onscreen keypad.
					if(keyValue!=0) {//add 100 to represent negative values unless they pressed 0
						keyValue+=100;
					}
				}
				//Possible values for keyValue at this colrow are 0, 101-109.
			}
			if(ThreeAtATime) {
				for(int i=0;i<3;i++) {
					EnterValue(keyValue);
				}
			}
			else {
				EnterValue(keyValue);
			}
		}

		///<summary>Only valid values are b,s,p,c,j,g,f,m,and period (.)</summary>
		private void EnterValue(string keyValue){
			if(ColRowSelected.Col==-1) {
				return;
			}
			if(keyValue !="b" && keyValue !="s" && keyValue !="p" && keyValue !="c" && keyValue !="j" && keyValue !="g" && keyValue !="f" && keyValue !="m" && keyValue !="."){
				MessageBox.Show("Only b,s,p,c,j,g,f,m,and period (.) are allowed");//just for debugging
				return;
			}
			if(keyValue=="b" || keyValue=="s" || keyValue=="p" || keyValue=="c") {
				PerioCell perioCell=GetPerioCell(ColRowSelected,false);
				bool hasText=false;
				bool isSkippedTooth=(_listSkippedTeeth.Contains(perioCell.ToothNum));
				if(ThreeAtATime){
					//don't backup
				}
				else if(perioCell.Text!=null && perioCell.Text!=""){
					hasText=true;
					//so enter value for current cell
				}
				else if(isSkippedTooth) {
					//The only way to get to a skipped tooth is to click on it.
					//This means there should be less automation.
					//Always put bleeding points, etc., on the current position.
					//Bleeding points don't cause advance anyway, whether on skipped tooth or not.
					hasText=true;
				}
				else{
					hasText=false;
					AdvanceCell(isReverse:true);//so backup
					perioCell=_perioCellArray[ColRowSelected.Col,ColRowSelected.Row];
					if(perioCell.Text==null || perioCell.Text=="") {//the previous cell is also empty
						hasText=true;//treat it like a cell with text
						AdvanceCell();//go forward again
						perioCell=_perioCellArray[ColRowSelected.Col,ColRowSelected.Row];
					}
					//enter value, then advance.
				}
				if(keyValue=="b"){
					if((perioCell.Bleeding & BleedingFlags.Blood)==0)//if it was off
						perioCell.Bleeding=perioCell.Bleeding | BleedingFlags.Blood;//turn it on
					else//if it was on
						perioCell.Bleeding=perioCell.Bleeding & ~BleedingFlags.Blood; //turn it off
				}
				if(keyValue=="s"){
					if((perioCell.Bleeding & BleedingFlags.Suppuration)==0)
						perioCell.Bleeding=perioCell.Bleeding | BleedingFlags.Suppuration;
					else
						perioCell.Bleeding=perioCell.Bleeding & ~BleedingFlags.Suppuration;
				}
				if(keyValue=="p"){
					if((perioCell.Bleeding & BleedingFlags.Plaque)==0)
						perioCell.Bleeding=perioCell.Bleeding | BleedingFlags.Plaque;
					else
						perioCell.Bleeding=perioCell.Bleeding & ~BleedingFlags.Plaque;
				}
				if(keyValue=="c"){
					if((perioCell.Bleeding & BleedingFlags.Calculus)==0)
						perioCell.Bleeding=perioCell.Bleeding | BleedingFlags.Calculus;
					else
						perioCell.Bleeding=perioCell.Bleeding & ~BleedingFlags.Calculus;
				}
				_perioCellArray[ColRowSelected.Col,ColRowSelected.Row]=perioCell;
				Invalidate(Rectangle.Ceiling(GetBounds(ColRowSelected.Col,ColRowSelected.Row)));
				SetHasChanged(ColRowSelected.Col,ColRowSelected.Row);
				if(ThreeAtATime){
					AdvanceCell();
				}
				else if(!hasText){
					AdvanceCell();//to return to original location
				}
				return;
			}
			int section=GetSection(ColRowSelected.Row);
			for(int i=0;i<_rowPosArray.Length;i++) {
				int idxSectRow=GetSectionRow(i);
				if(GetSection(i)!=section) {
					continue;
				}
				if(keyValue=="j") {
					if(_perioSequenceTypeArray[section][idxSectRow]==PerioSequenceType.MGJ) {
						SetNewCell(ColRowSelected.Col,i);
						Focus();
						return;
					}
				}
				if(keyValue=="g") {
					if(_perioSequenceTypeArray[section][idxSectRow]==PerioSequenceType.GingMargin) {
						SetNewCell(ColRowSelected.Col,i);
						Focus();
						return;
					}
				}
				if(keyValue=="f") {
					if(_perioSequenceTypeArray[section][idxSectRow]==PerioSequenceType.Furcation) {
						SetNewCell(ColRowSelected.Col,i);
						Focus();
						return;
					}
				}
				if(keyValue=="m") {
					int colSelected=ColRowSelected.Col;
					int colLeft=colSelected-1;
					int colRight=colSelected+1;
					if(Math.IEEERemainder(((double)colLeft+1),3)==0) {//Currently on the right col of tooth and need to go to the left to get to the middle
						colSelected=colLeft;
					}
					else if(Math.IEEERemainder(((double)colRight+1),3)==0) {//Currently on the left col of tooth and need to go to the right to get to the middle
						colSelected=colRight;
					}
					if(_perioSequenceTypeArray[section][idxSectRow]==PerioSequenceType.Mobility) {
						SetNewCell(colSelected,i);
						Focus();
						return;
					}
				}
				if(keyValue==".") {
					if(_perioSequenceTypeArray[section][idxSectRow]==PerioSequenceType.Probing) {
						if(this._idxExamSelected-_probingOffset//correct for offset
							+_perioSequenceTypeArray[section].Length-_rowsProbing//plus non-probing rows
							!=idxSectRow)
						{//not the probing row of current perio exam
							continue;
						}
						SetNewCell(ColRowSelected.Col,i);
						Focus();
						return;
					}
				}
			}
		}

		///<summary>Only valid values are numbers 0-19, and 101-109. Validation should be done before sending here.</summary>
		private void EnterValue(int keyValue){
			if(ColRowSelected.Col==-1) {
				return;
			}
			if((keyValue < 0 || keyValue > 19) 
				&& _perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]!=PerioSequenceType.GingMargin){//large values are allowed for GingMargin to represent hyperplasia (e.g. 101 to 109 represent -1 to -9)
				MessageBox.Show("Only values 0 through 19 allowed");//just for debugging
				return;
			}
			PerioCell perioCell=GetPerioCell(ColRowSelected,setText:false);
			//might be able to eliminate these two lines
			perioCell.Text=keyValue.ToString();
			_perioCellArray[ColRowSelected.Col,ColRowSelected.Row]=perioCell;
			Invalidate(Rectangle.Ceiling(GetBounds(ColRowSelected.Col,ColRowSelected.Row)));
			SetHasChanged(ColRowSelected.Col,ColRowSelected.Row);
			PerioSequenceType perioSequenceType=_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)];
			if(perioSequenceType==PerioSequenceType.Probing){ 
				CalculateCAL(ColRowSelected,alsoInvalidate:true);
			}
			else if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.GingMargin){
				ColRow colRow=new ColRow(ColRowSelected.Col,GetTableRow
					(_idxExamSelected,GetSection(ColRowSelected.Row),PerioSequenceType.Probing));
				CalculateCAL(colRow,alsoInvalidate:true);
			}
			AdvanceCell();
		}

		///<summary>Sets a mark for the the CurCell for the type passed in. Make sure to set the CurCell to the colrow you want to mark before you call this method. </summary>
		public void SetBleedingFlagForColRow(ColRow colRow,BleedingFlags bleedingFlags) {
			if(colRow.Col==-1) {
				return;
			}
			PerioCell perioCell=GetPerioCell(colRow,false);
			perioCell.Bleeding^=bleedingFlags;
			_perioCellArray[colRow.Col,colRow.Row]=perioCell;
			Invalidate(Rectangle.Ceiling(GetBounds(colRow.Col,colRow.Row)));
			SetHasChanged(colRow.Col,colRow.Row);
		}

		private void CalculateCAL(ColRow colRowProbingCell,bool alsoInvalidate){
			ColRow colRowCalLoc=new ColRow(colRowProbingCell.Col,GetTableRow
				(_idxExamSelected,GetSection(colRowProbingCell.Row),PerioSequenceType.CAL));
			ColRow colRowGingLoc=new ColRow(colRowProbingCell.Col,GetTableRow
				(_idxExamSelected,GetSection(colRowProbingCell.Row),PerioSequenceType.GingMargin));
			//PerioCell calCell=DataArray[calLoc.Col,calLoc.Row];
			if(_perioCellArray[colRowProbingCell.Col,colRowProbingCell.Row].Text==null 
				|| _perioCellArray[colRowProbingCell.Col,colRowProbingCell.Row].Text==""
				|| _perioCellArray[colRowGingLoc.Col,colRowGingLoc.Row].Text==null 
				|| _perioCellArray[colRowGingLoc.Col,colRowGingLoc.Row].Text=="")
			{
				_perioCellArray[colRowCalLoc.Col,colRowCalLoc.Row].Text="";
				if(alsoInvalidate){
					Invalidate(Rectangle.Ceiling(GetBounds(colRowCalLoc.Col,colRowCalLoc.Row)));
				}
				return;
			}
			int probValue=PIn.Int(_perioCellArray[colRowProbingCell.Col,colRowProbingCell.Row].Text);
			int gingValue=PIn.Int(_perioCellArray[colRowGingLoc.Col,colRowGingLoc.Row].Text);
			if(gingValue>100) {
				gingValue=100-gingValue;
			}
			_perioCellArray[colRowCalLoc.Col,colRowCalLoc.Row].Text=(gingValue+probValue).ToString();
			if(alsoInvalidate){
				Invalidate(Rectangle.Ceiling(GetBounds(colRowCalLoc.Col,colRowCalLoc.Row)));
			}
		}

		private void SetHasChanged(int col,int row){
			int section=GetSection(row);
			int sectionRow=GetSectionRow(row);
			int intTooth=(int)Math.Ceiling((double)col/3);//1-16
			if(section==2 || section==3){
				intTooth=33-intTooth;
			}
			//If the section row is outside the array bounds, we simply return.
			if(sectionRow==-1 || sectionRow>=_perioSequenceTypeArray[section].Count() || intTooth>32 || intTooth<1) {
				return;
			}
			PerioSequenceType perioSequenceType=_perioSequenceTypeArray[section][sectionRow];
			if((int)perioSequenceType>PerioMeasures.List.GetLength(1)) {
				return;
			}
			ListPerioMeasureItemsChanged.Add(new PerioMeasureItem(perioSequenceType,intTooth));
		}

		///<summary>Changes the cell that the cursor is in.</summary>
		public void SetNewCell(int x,int y){
			//MessageBox.Show(x.ToString()+","+y.ToString());
			RectangleF rectangleFOld=new Rectangle(0,0,0,0);
			bool invalidateOld=false;
			if(ColRowSelected.Col!=-1){
				rectangleFOld=GetBounds(ColRowSelected.Col,ColRowSelected.Row);
				invalidateOld=true;
			}
			ColRowSelected=new ColRow(x,y);
			if(invalidateOld){
				Invalidate(Rectangle.Ceiling(rectangleFOld));
			}
			Invalidate(Rectangle.Ceiling(GetBounds(ColRowSelected.Col,ColRowSelected.Row)));
		}

		///<summary>Most of the time use GetSection() to calculate the section.  A section corresponds to the maxillary/mandible facial/lingual 
		///(4 sections total, see GetSection() summary)</summary>
		private int GetToothNumCur(int section) {
			return GetToothNumFromColRow(section,ColRowSelected);
		}

		private int GetToothNumFromColRow(int section,ColRow colRow) {
			int intTooth=(int)Math.Ceiling((double)colRow.Col/3);
			if(section==2||section==3) {//if on mand
				intTooth=33-intTooth;//wrap
			}
			return intTooth;
		}

		public PerioCell GetCurrentCell() {
			if(ColRowSelected.Col < 0 || ColRowSelected.Row < 0) {
				return new PerioCell();
			}
			return GetPerioCell(ColRowSelected,true);
		}
		
		public PerioCell GetPerioCellFromColRow(ColRow colRowSelected) {
			return GetPerioCell(colRowSelected,true);
		}

		///<summary>Returns PerioCell for the colrow passed in. Sets PerioCell in DataArray based on the colrow passed in. Option to not set PerioCell Text.</summary>
		private PerioCell GetPerioCell(ColRow colRow,bool setText) {
			PerioCell perioCell=_perioCellArray[colRow.Col,colRow.Row];
			perioCell.IsFacial=true;
			if(colRow.Row>=12 && colRow.Row<=30) {
				perioCell.IsFacial=false;
			}
			if(colRow.Row<=20) {
				perioCell.ToothNum=(colRow.Col+2)/3;
			}
			else {
				perioCell.ToothNum=33-(colRow.Col+2)/3;
			}
			if(perioCell.IsFacial) {
				perioCell.ProbingPosition=(colRow.Col-1)%3+1;
			}
			else {
				perioCell.ProbingPosition=3-(colRow.Col-1)%3;
			}
			if(setText) {
				perioCell.Text=GetCellText(colRow.Col,colRow.Row);
			}
			return perioCell;
		}

		private void AdvanceCell(bool isReverse=false){
			ColRow colRowToSet=TryFindNextCell(ColRowSelected,isReverse);
			if(colRowToSet==null) {
				return;
			}
			SetNewCell(colRowToSet.Col,colRowToSet.Row);
		}

		///<summary>Finds the next cell based on where the cursor currently is. Returns null if there is no next cell.</summary>
		public ColRow TryFindNextCell(ColRow colRow,bool isReverse,bool setDirection=true) {
			ColRow colRowNext=colRow;
			PerioSequenceType perioSequenceType=_perioSequenceTypeArray[GetSection(colRowNext.Row)][GetSectionRow(colRowNext.Row)];
			bool startedOnSkipped=false;//special situation:
			int section=GetSection(colRowNext.Row);//used to test skipped teeth
			int newSection=section;//in case it doesn't change
			int intTooth=GetToothNumCur(section);//used to test skipped teeth
			if(_listSkippedTeeth.Contains(intTooth)){
				startedOnSkipped=true;
			}
			int limit=0;
			int count=0;//count the number of positions skipped.
			while(true){
				if(limit>400){//the 400 limit is just a last resort. Should always break manually.
					break;
				}
				limit++;
				//to the right
				section=GetSection(colRowNext.Row);
				if(EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {//Facials First path
					colRowNext=TryAutoAdvanceFacialsFirst(colRow,startedOnSkipped,isReverse,setDirection);
					if(colRowNext==null){
						return null;
					}
					return colRowNext;
				}
				if((EnumCurrentDirection_==EnumCurrentDirection.Left && isReverse) || (EnumCurrentDirection_==EnumCurrentDirection.Right && !isReverse)) {//right
					colRowNext=TryAutoAdvanceRight(colRowNext,section,setDirection,perioSequenceType,startedOnSkipped,count);
					if(colRowNext==null) {
						return null;
					}
					if(_shouldJumpBack){
						count++;//this could be big number
					}
				}
				//to the left
				else{//((DirectionIsRight && isBackspace) || !DirectionIsRight){
					colRowNext=TryAutoAdvanceLeft(colRowNext,section,setDirection,perioSequenceType);
					if(colRowNext==null) {
						return null;
					}
				}
				intTooth=GetToothNumFromColRow(GetSection(colRowNext.Row),colRowNext);//next tooth
				bool isValidLoc=true;//used when testing for skipped tooth and mobility location
				//We still want to move between skipped teeth, so only mark the next location as invalid if starting on an unskipped tooth and going to a skipped tooth.
				if(_listSkippedTeeth.Contains(intTooth) && !startedOnSkipped) {
					isValidLoc=false;
				}
				if(_perioSequenceTypeArray[GetSection(colRowNext.Row)][GetSectionRow(colRowNext.Row)]==PerioSequenceType.Mobility) {
					if(Math.IEEERemainder(((double)colRowNext.Col+1),3) != 0) {//{2,5,8,11};examples of acceptable cols
						isValidLoc=false;//for mobility, not allowed to click on anything but B
					}
				}
				else if(startedOnSkipped) {//since we started on a skipped tooth
					return colRowNext;//we can continue entry on a skipped tooth.
				}
				if(isValidLoc) {
					return colRowNext;
				}
				//otherwise, continue to loop in search of a valid loc
			}//while
			return null;
		}

		///<summary>Helper method that moves the current colrow to the right(when viewing the mouth and not the perio chart). When viewing the perio
		///chart, this will move the cell to the left.</summary>
		private ColRow TryAutoAdvanceRight(ColRow colRowSelected,int section,bool setDirection,PerioSequenceType perioSequenceType,bool startedOnSkipped,int count) 
		{
			ColRow colRowNext=colRowSelected;
			int newSection=0;
			int newRow=0;
			int intTooth=GetToothNumCur(section);
			_shouldJumpBack=false;
			if(colRowNext.Col==1 && !startedOnSkipped) {//if first column
				if(section==0 && _listSkippedTeeth.Contains(1) && intTooth==1) {
					colRowNext=new ColRow(count+1,colRowNext.Row);
					return colRowNext;//tooth 1 is missing. Jump back.
				}
				else if(section==0) {//usually in reverse
					return null;//no jump.  This is the starting colrow.
				}
				else if(section==1) {
					newSection=3;
					newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
					if(setDirection) {
						ChangeDirectionLeft();
					}
				}
				else if(section==2 && _listSkippedTeeth.Contains(32)) {
					if(colRowNext.Col==1) {
						colRowNext=new ColRow(count+1,colRowNext.Row);
						return colRowNext;//tooth 32 is missing. Jump back.
					}
					else {
						return null;//probably should never happen.
					}
				}
				else if(section==2) {
					return null;//no jump.  End of all sequences.
				}
				else if(section==3) {//usually in reverse
					newSection=1;
					newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
					if(newRow!=-1 && setDirection) {
						ChangeDirectionRight();
					}
				}
				if(newRow==-1) {//MGJ and mobility aren't present in all 4 sections, so can't loop normally
					if(_perioSequenceTypeArray[section][GetSectionRow(colRowNext.Row)]==PerioSequenceType.Mobility) {
						if(section==3) {//usually in reverse
							newSection=0;
							colRowNext=new ColRow(1+16*3,GetTableRow(_idxExamSelected,newSection,PerioSequenceType.Mobility));
						}
					}
					else if(_perioSequenceTypeArray[section][GetSectionRow(colRowNext.Row)]==PerioSequenceType.MGJ) {
						//section 3. in reverse
						newSection=0;
						colRowNext=new ColRow(16*3,GetTableRow(_idxExamSelected,newSection,PerioSequenceType.MGJ));
					}
				}
				else {
					colRowNext=new ColRow(colRowNext.Col,newRow);
				}
			}
			//Check to see if tooth #16 is missing.
			else if(section==1 && colRowNext.Col==1 && startedOnSkipped) {
				//Tooth 16 is missing so we need to jump down to section 3 (tooth 32) and start going the other direction.
				newSection=3;
				colRowNext=new ColRow(1,GetTableRow(_idxExamSelected,newSection,perioSequenceType));
				if(setDirection) {
					ChangeDirectionLeft();
				}
			}
			else if((section==0 && _listSkippedTeeth.Contains(1)) //skipped tooth 1 on facial side
				|| (section==2 && _listSkippedTeeth.Contains(32)) //skipped tooth 32 on lingual side
				|| (section==3 && _listSkippedTeeth.Contains(32)) //skipped tooth 32 on facial side
				|| (section==3 && _listSkippedTeeth.Count==32)) //skipped all teeth and are on last row
			{
				_shouldJumpBack=true;//jump back to last non missing tooth
				if(section==2 && colRowNext.Col==1) {
					return null;//manually clicking on missing tooth 32 and entering data
				}
				else if(section==0 && colRowNext.Col==1) {
					return null;//manually clicking on missing tooth 1 and entering data
				}
				else if(section==3 && colRowNext.Col==1) {
					return null;//stop on first tooth
				}
				//this happens when we are about to move to the lower arch.  Section order is 1,2,4,3.
				bool hasAnyLowerTeeth = false;
				for(int i = 17;i<=32;++i) {
					if(!_listSkippedTeeth.Contains(i)) {
						hasAnyLowerTeeth = true;
					}
				}
				if(section == 3 && !hasAnyLowerTeeth) {
					return null;
				}
				else {
					colRowNext=new ColRow(colRowNext.Col-1,colRowNext.Row);//standard advance
				}
			}
			else {//standard advance with no jumping
				colRowNext=new ColRow(colRowNext.Col-1,colRowNext.Row);
			}
			return colRowNext;
		}

		///<summary>Helper method that moves the current colrow to the left(when viewing the mouth and not the perio chart). When viewing the perio
		///chart, this will move the cell to the right. Returns null if not possible to advance.</summary>
		private ColRow TryAutoAdvanceLeft(ColRow colRowSelected,int section,bool setDirection,PerioSequenceType perioSequenceType) {
			ColRow colRowNext=colRowSelected;
			int newRow=0;
			int newSection=section;
			if(colRowNext.Col!=_perioCellArray.GetLength(0)-1) {//if not last column. standard advance with no jumping
				colRowNext=new ColRow(colRowNext.Col+1,colRowNext.Row);
				return colRowNext;
			}
			if(section==0) {
				newSection=1;
				newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
				if(newRow!=-1 && setDirection) {
					ChangeDirectionRight();
				}
			}
			else if(section==1) {//usually in reverse
				newSection=0;
				newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
				if(setDirection) {
					ChangeDirectionLeft();
				}
			}
			else if(section==2) {//usually in reverse
				newSection=3;
				newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
				if(setDirection) {
					ChangeDirectionLeft();
				}
			}
			else if(section==3) {
				newSection=2;
				newRow=GetTableRow(_idxExamSelected,newSection,perioSequenceType);
				if(newRow!=-1 && setDirection)
					ChangeDirectionRight();
			}
			if(newRow==-1) {//MGJ and mobility aren't present in all 4 sections, so can't loop normally
				if(_perioSequenceTypeArray[section][GetSectionRow(colRowNext.Row)]==PerioSequenceType.Mobility) {
					if(section==0) {
						newSection=3;
						colRowNext=new ColRow(1,GetTableRow(_idxExamSelected,newSection,PerioSequenceType.Mobility));
					}
					if(section==3) {
						return null;//end of sequence
					}
				}
				else if(_perioSequenceTypeArray[section][GetSectionRow(colRowNext.Row)]==PerioSequenceType.MGJ) {
					//section 0
					newSection=3;
					colRowNext=new ColRow(1,GetTableRow(_idxExamSelected,newSection,PerioSequenceType.MGJ));
				}
			}
			else {
				colRowNext=new ColRow(colRowNext.Col,newRow);
			}
			//this happens when we are about to move to the lower arch.  Section order is 1,2,4,3.
			bool hasAnyLowerTeeth = false;
			for(int i = 17;i<=32;++i) {
				if(!_listSkippedTeeth.Contains(i)) {
					hasAnyLowerTeeth = true;
				}
			}
			//We are tentatively in section 3 because of a while loop further up.
			if(section==3 && !hasAnyLowerTeeth) {
				return null;//end of sequence because no lower teeth.
			}
			return colRowNext;
		}

		///<summary>Moves the current colrow to the next position. Returns null if it can't advance.</summary>
		private ColRow TryAutoAdvanceFacialsFirst(ColRow colRowSelected,bool startedOnSkipped,bool isReverse,bool setDirection) {
			PerioCell perioCellSelected=GetPerioCellFromColRow(colRowSelected);
			ColRow colRowNext=colRowSelected;
			PerioSequenceType perioSequenceType=GetSequenceForColRow(colRowNext);
			int section=GetSection(colRowNext.Row);//used to test skipped teeth
			List<PerioAdvancePos> listAutoAdvancePositions=PerioAdvancePos.GetFacialFirstPath();
			bool isFacial=perioCellSelected.IsFacial;
			PerioAdvancePos autoAdvancePosition=listAutoAdvancePositions.FirstOrDefault(x => x.ToothNum==perioCellSelected.ToothNum && x.IsFacial==isFacial);
			if(autoAdvancePosition==null) {
				//We some how could not find the next colrow. Return the initial colrow.
				return colRowNext;
			}
			//listAutoAdvancePositions should be in the order the preference specified(sequential order) so the next and previous positions will be the actual next and previous paths.
			PerioAdvancePos autoAdvancePositionNext=GetAutoAdvancePositionFromList(autoAdvancePosition,getNext:true,perioSequenceType,listAutoAdvancePositions);
			bool shouldAdvanceRight=IsAutoAdvanceRight(autoAdvancePosition,autoAdvancePositionNext,perioSequenceType);
			if(isReverse){
				shouldAdvanceRight=!shouldAdvanceRight;
			}
			int countNotUsed=0;
			bool startedOnCellOne=false;
			if(colRowNext.Col==1){
				startedOnCellOne=true;
			}
			//Advance colrowNext to the next cell position.
			if(shouldAdvanceRight) {
				colRowNext=TryAutoAdvanceLeft(colRowNext,section,setDirection,perioSequenceType);
				if(colRowNext==null) {
					return null;
				}
				if(EnumCurrentDirection_!=EnumCurrentDirection.Left) {
					ChangeDirectionLeft();
				}
			}
			else {
				colRowNext=TryAutoAdvanceRight(colRowNext,section,setDirection,perioSequenceType,startedOnSkipped,countNotUsed);
				if(colRowNext==null) {
					return null;
				}
				if(EnumCurrentDirection_!=EnumCurrentDirection.Right) {
					ChangeDirectionRight();
				}
			}
			PerioCell perioCellNext=GetPerioCellFromColRow(colRowNext);
			//TryAutoAdvanceLeft and TryAutoAdvanceRight are helper methods that get the next colrow position using the default auto advance path
			//1-16F -> 16-1L -> 32-17F -> 17-32L. The methods will not advance to the right(left when viewing the perio chart) when on the first position i.e 32L. 
			//We need to go to the next tooth if the previous path is not null and isReverse.
			//or if the sequence is mobility(only has the middle(facial) surface available for entry).
			PerioAdvancePos autoAdvancePositionPrev=GetAutoAdvancePositionFromList(autoAdvancePosition,getNext:false,perioSequenceType,listAutoAdvancePositions);
			bool didStartOnOneOrAutoAdv = startedOnCellOne && colRowNext.Col==1 && !autoAdvancePosition.IsFacial && autoAdvancePositionPrev!=null;
			bool skipSameToothCheck=didStartOnOneOrAutoAdv || perioSequenceType==PerioSequenceType.Mobility;
			if(!skipSameToothCheck && perioCellSelected.ToothNum==perioCellNext.ToothNum && perioCellSelected.IsFacial==perioCellNext.IsFacial) {
				//Same tooth,different position. Return true. colorowNext was already set in TryAutoAdvanceLeft and TryAutoAdvanceRight
				return colRowNext;
			}
			//The colrow returned in TryAutoAdvanceLeft or TryAutoAdvanceRight is on a differnt tooth, get the next tooth from the listAutoAdvancePositions.
			if(isReverse) {
				autoAdvancePositionNext=autoAdvancePositionPrev;//The next tooth should be the previous one. 
			}
			if(autoAdvancePositionNext==null) {
				//The end of the list. Set the return colrow equal to the initial colrow. No need to move.
				colRowNext=colRowSelected;
				return colRowNext;
			}
			//return the colrow for the next tooth in path.
			colRowNext=GetAutoAdvanceColRow(autoAdvancePositionNext.ToothNum,autoAdvancePositionNext.IsFacial,perioSequenceType,isReverse);
			return colRowNext;
		}

		///<summary>Returns either the next or previous AutoAdvancePositions from the list. Considers skipped teeth. Recursive method.</summary>
		public PerioAdvancePos GetAutoAdvancePositionFromList(PerioAdvancePos autoAdvancePosition,bool getNext,PerioSequenceType perioSequenceType,List<PerioAdvancePos> listAutoAdvancePositions)
		{
			PerioAdvancePos autoAdvancePositionMove=GetPrevious(autoAdvancePosition,listAutoAdvancePositions);
			if(getNext){
				autoAdvancePositionMove=GetNext(autoAdvancePosition,listAutoAdvancePositions); 
			}
			if(autoAdvancePositionMove==null) {
				return null;//Most likely the last one
			}
			bool isMaxillaryMGJ = perioSequenceType==PerioSequenceType.MGJ && autoAdvancePositionMove.ToothNum.Between(1,16);
			bool isMaxillaryMGJOrMobility = isMaxillaryMGJ || perioSequenceType==PerioSequenceType.Mobility;
			if(isMaxillaryMGJOrMobility && !autoAdvancePositionMove.IsFacial) {
				//The perio chart does not have MGJ for Lingual teeth 1-16. Find the next available AutoAdvancePosition from list.
				PerioAdvancePos autoAdvancePositionMGJ=autoAdvancePositionMove;
				return GetAutoAdvancePositionFromList(autoAdvancePositionMGJ,getNext,perioSequenceType,listAutoAdvancePositions);
			}
			//Check to make sure the perioPath is not marked as skipped.
			if(autoAdvancePositionMove!=null && _listSkippedTeeth.Contains(autoAdvancePositionMove.ToothNum)) {
				return GetAutoAdvancePositionFromList(autoAdvancePositionMove,getNext,perioSequenceType,listAutoAdvancePositions);
			}
			return autoAdvancePositionMove;
		}

		public static PerioAdvancePos GetNext(PerioAdvancePos autoAdvancePosition,List<PerioAdvancePos> listAutoAdvancePositions) {
			try {
				return listAutoAdvancePositions.SkipWhile(x => !x.Equals(autoAdvancePosition)).Skip(1).First();
			}
			catch {
				return null;
			}
		}

		public static PerioAdvancePos GetPrevious(PerioAdvancePos autoAdvancePosition,List<PerioAdvancePos> listAutoAdvancePositions) {
			try {
				return listAutoAdvancePositions.TakeWhile(x => !x.Equals(autoAdvancePosition)).Last();
			}
			catch {
				return null;
			}
		}

		///<summary>Used for non-default Advance Sequences. Returns true if the auto advance should go to the right direction.</summary>
		public bool IsAutoAdvanceRight(PerioAdvancePos autoAdvancePosition,PerioAdvancePos autoAdvancePositionNext,PerioSequenceType perioSequenceType) {
			//This method will need to change if we add other non-default paths.
			if(autoAdvancePosition==null) {
				return false;
			}
			int intTooth=autoAdvancePosition.ToothNum;
			int toothNumNext=autoAdvancePositionNext?.ToothNum??autoAdvancePosition.ToothNum;
			if(autoAdvancePositionNext==null){
				autoAdvancePositionNext=autoAdvancePosition;
			}
			int section=GetSection(intTooth,autoAdvancePosition.IsFacial);
			int sectionNext=GetSection(toothNumNext,autoAdvancePositionNext.IsFacial);
			if(toothNumNext.Between(1,16)) {
				//If the next tooth number is between 1 and 16, the direction will go right in 3 cases: 
				//1. If the next tooth surface is facial
				//2. toothNumCur is 32-17L(going from 17L to 16L).
				//3. the next tooth is 1-16 but in different section.
				if(autoAdvancePositionNext.IsFacial || intTooth>=17 || section!=sectionNext) {
					return true;
				}
			}
			else {//toothNumNext between 17-32
				//If the next tooth number is between 17 and 32, the direction will go right in 2 cases: 
				//1. The next tooth surface is Lingual (except when going from 32F to 32L)
				//2. toothNumCur is <=16(going from 1-16F to 17-32F)
				if((!autoAdvancePositionNext.IsFacial && !autoAdvancePosition.IsFacial) || intTooth<=16) {
					return true;//32-17L or going from 16F to 17F
				}
			}
			return false;
		}

		public bool IsCellTextEmpty(ColRow colRow) {
			return string.IsNullOrEmpty(GetCellText(colRow.Col,colRow.Row));
		}

		private void ClearValue(){
			//MessageBox.Show(DataArray.GetLength(0).ToString());
			//MessageBox.Show(DataArray.GetLength(1).ToString());
			PerioCell perioCell=_perioCellArray[ColRowSelected.Col,ColRowSelected.Row];
			perioCell.Text="";
			_perioCellArray[ColRowSelected.Col,ColRowSelected.Row]=perioCell;
			SetHasChanged(ColRowSelected.Col,ColRowSelected.Row);
			Invalidate(Rectangle.Ceiling(GetBounds(ColRowSelected.Col,ColRowSelected.Row)));
			if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.Probing){ 
				CalculateCAL(ColRowSelected,alsoInvalidate:true);
			}
			else if(_perioSequenceTypeArray[GetSection(ColRowSelected.Row)][GetSectionRow(ColRowSelected.Row)]==PerioSequenceType.GingMargin){
				CalculateCAL(new ColRow(ColRowSelected.Col,GetTableRow
					(_idxExamSelected,GetSection(ColRowSelected.Row),PerioSequenceType.Probing)),alsoInvalidate:true);
			}
		}

		///<summary></summary>
		public void ToggleSkip(long perioExamNum) {
			if(selectedTeeth.Count==0){
				MessageBox.Show(Lan.g(this,"Please select teeth first."));
				return;
			}
			for(int i=0;i<selectedTeeth.Count;i++){
				if(_listSkippedTeeth.Contains((int)selectedTeeth[i])){//if the tooth was already marked skipped
					_listSkippedTeeth.Remove((int)selectedTeeth[i]);
				}
				else{//tooth was not already marked skipped
					_listSkippedTeeth.Add((int)selectedTeeth[i]);
				}
			}
			PerioMeasures.SetSkipped(perioExamNum,_listSkippedTeeth);
			selectedTeeth=new ArrayList();
			Invalidate();
		}

		///<summary></summary>
		private void ChangeDirectionRight(){
			EnumCurrentDirection_=EnumCurrentDirection.Right;
			DirectionChangedRight?.Invoke(this,new EventArgs());
		}

		///<summary></summary>
		private void ChangeDirectionLeft(){
			EnumCurrentDirection_=EnumCurrentDirection.Left;
			DirectionChangedLeft?.Invoke(this,new EventArgs());
		}

		///<summary></summary>
		public string ComputeOrionPlaqueIndex() {
			if(this._idxExamSelected==-1) {
				return "";
			}
			int counter=0;
			List<PerioMeasure> listPerioMeasures=PerioMeasures.GetAllForExam(PerioExams.ListExams[_idxExamSelected].PerioExamNum);
			for(int i=0;i<listPerioMeasures.Count;i++) {
				if(listPerioMeasures[i].SequenceType==PerioSequenceType.Bleeding) {
					//If tooth has plaque on any of the six colrows
					if(((BleedingFlags)listPerioMeasures[i].MBvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)listPerioMeasures[i].Bvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)listPerioMeasures[i].DBvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)listPerioMeasures[i].MLvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)listPerioMeasures[i].Lvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)listPerioMeasures[i].DLvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
				}
			}
			if(counter==0 || _listSkippedTeeth.Count==32) {
				return (0).ToString("F0");
			}
			return (100*counter/((32-_listSkippedTeeth.Count))).ToString("F0");
		}

		///<summary></summary>
		public string ComputeIndex(BleedingFlags bleedingFlags){
			if(this._idxExamSelected==-1){
				return "";
			}
			int counter=0;
			for(int section=0;section<4;section++){
				for(int x=1;x<1+3*16;x++){
					if((_perioCellArray[x,GetTableRow(_idxExamSelected,section,PerioSequenceType.Probing)].Bleeding 
						& bleedingFlags)>0)
					{
						counter++;
					}
				}
			}
			if(counter==0) {
				return (0).ToString("F0");
			}
			int unskippedTeeth=(32-_listSkippedTeeth.Count);
			if(unskippedTeeth==0) {
				return "";
			}
			return (100*counter/(unskippedTeeth*6)).ToString("F0");
		}

		///<summary>Returns a list of strings, each between "1" and "32" (or similar international #'s), representing the teeth with red values based on prefs.  The result can be used to print summary, or can be counted to show # of teeth.</summary>
		public List<string> CountTeeth(PerioSequenceType perioSequenceType){
			if(_idxExamSelected==-1){
				return new List<string>();
			}
			int prefVal=0;
			switch(perioSequenceType){
				case PerioSequenceType.Probing:
					prefVal=PrefC.GetInt(PrefName.PerioRedProb);
					break;
				case PerioSequenceType.MGJ:
					prefVal=PrefC.GetInt(PrefName.PerioRedMGJ);
					break;
				case PerioSequenceType.GingMargin:
					prefVal=PrefC.GetInt(PrefName.PerioRedGing);
					break;
				case PerioSequenceType.CAL:
					prefVal=PrefC.GetInt(PrefName.PerioRedCAL);
					break;
				case PerioSequenceType.Furcation:
					prefVal=PrefC.GetInt(PrefName.PerioRedFurc);
					break;
				case PerioSequenceType.Mobility:
					prefVal=PrefC.GetInt(PrefName.PerioRedMob);
					break;
			}
			List<string> listStrings=new List<string>();
			string cellText="";
			int intTooth=0;
			int row=0;
			for(int section=0;section<4;section++){
				for(int x=1;x<1+3*16;x++){
					row=GetTableRow(_idxExamSelected,section,perioSequenceType);
					if(row==-1)
						continue;//eg MGJ or Mobility
					cellText=_perioCellArray[x,row].Text;
					if(cellText==null || cellText==""){
						continue;
					}
					if((perioSequenceType==PerioSequenceType.MGJ && PIn.Long(cellText)<=prefVal)
						|| (perioSequenceType!=PerioSequenceType.MGJ && PIn.Long(cellText)>=prefVal)){
						intTooth=(int)Math.Ceiling((double)x/3);
						if(section==2 || section==3){//if mand
							intTooth=33-intTooth;
						}
						if(!listStrings.Contains(Tooth.Display(intTooth.ToString()))){
							listStrings.Add(Tooth.Display(intTooth.ToString()));
						}
					}
				}
			}
			return listStrings;
		}

		///<summary>Uses this control to draw onto the specified graphics object (the printer).</summary>
		public void DrawChart(Graphics g) {
			PaintEventArgs e=new PaintEventArgs(g,ClientRectangle);
			g.Clear(Color.White);
			DrawBackground(e);
			DrawSkippedTeeth(e);
			//DrawSelectedTeeth(e);
			//DrawCurCell(e);
			DrawGridlines(e);
			DrawText(e);
		}

		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// ContrPerio
			// 
			this.ResumeLayout(false);

		}

		public struct PerioMeasureItem {
			public PerioMeasureItem(PerioSequenceType seqType,int toothNum) {
				this.SeqType=seqType;
				this.ToothNum=toothNum;
			}
			public PerioSequenceType SeqType;
			public int ToothNum;
		}
	}	

	///<summary>6 cells get combined together for a single PerioMeasure db entry.</summary>
	public struct PerioCell{
		///<summary>The value to display for this exam. Overwrites any oldText from previous exams.</summary>
		public string Text;
		///<summary>None, blood, pus, or both</summary>
		public BleedingFlags Bleeding;
		///<summary>Values from previous exams. Slightly greyed out.</summary>
		public string OldText;
		public int ToothNum;
		public int ProbingPosition;
		public bool IsFacial;
	}

	public class PerioAdvancePos {
		public int ToothNum;
		public bool IsFacial;

		public PerioAdvancePos(int toothNum,bool isFacial) {
			ToothNum=toothNum;
			IsFacial=isFacial;
		}

		///<summary>Returns the default path for perio auto advance. 1-16F, 16-1L,32-17F,17-32L.</summary>
		public static List<PerioAdvancePos> GetDefaultPath() {
			List<PerioAdvancePos> listAutoAdvancePositionsPath=new List<PerioAdvancePos>();
			//1-16F
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(1,16).Select(x => new PerioAdvancePos(x,isFacial:true)));
			//16-1L
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(1,16).OrderByDescending(x => x).Select(x => new PerioAdvancePos(x,isFacial:false)));
			//32-17F
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(17,16).OrderByDescending(x => x).Select(x => new PerioAdvancePos(x,isFacial:true)));
			//17-32L
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(17,16).Select(x => new PerioAdvancePos(x,isFacial:false)));
			return listAutoAdvancePositionsPath;
		}

		///<summary>Auto advance facial first path 1-16F,17-32F,32-17L,16-1L.</summary>
		public static List<PerioAdvancePos> GetFacialFirstPath() {
			List<PerioAdvancePos> listAutoAdvancePositionsPath=new List<PerioAdvancePos>();
			//1-16F
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(1,16).Select(x => new PerioAdvancePos(x,isFacial:true)));
			//17-32F
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(17,16).Select(x => new PerioAdvancePos(x,isFacial:true)));
			//32-17L
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(17,16).OrderByDescending(x => x).Select(x => new PerioAdvancePos(x,isFacial:false)));
			//16-1L
			listAutoAdvancePositionsPath.AddRange(Enumerable.Range(1,16).OrderByDescending(x => x).Select(x => new PerioAdvancePos(x,isFacial:false)));
			return listAutoAdvancePositionsPath;
		}
	}

	///<summary>Jordan This is all wrong and needs major overhaul.  There are 6 positions, not 4. Facial and Lingual seem to be doing double duty and mean two entirely different things, depending on context. 1: middle, 2: FvL.</summary>
	public enum PerioSurface {
		///<summary>The surface of the tooth in contact with the lips. This is what the "F" on the perio chart stands for. For teeth in the facial row, 
		///the middle cell of the tooth is considered the facial surface.</summary>
		Facial,
		///<summary>The surface of the tooth that the tongue touchess. This is what the "L" on the perio chart stands for. For teeth in the lingual 
		///row, the middle cell of the tooth is considered the lingual surface.</summary>
		Lingual,
		///<summary>The surface of the tooth facing the back of the mouth. In the perio chart for teeth 1-8 and 25-32, the distal surface is the left cell. 
		///For teeth 9-24, the distal surface is the right cell.</summary>
		Distal,
		///<summary>The surface of the tooth facing the center of the mouth. In the perio chart for teeth 1-8 and 25-32, the mesial surface is the right cell. 
		///For teeth 9-24, the mesial surface is the left cell.</summary>
		Mesial
	}

	public enum EnumAdvanceSequence {
		///<summary>0</summary>
		MaxFirst, 
		///<summary>1</summary>
		FacialsFirst
	}

	public enum EnumCurrentDirection {
		///<summary>0</summary>
		Left, 
		///<summary>1</summary>
		Right
	}
}