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
		private int RowsProbing;
		///<summary>First dimension is either UF, UL, LF, or LL. Second dim is an array of the types of rows showing in that section.</summary>
		private PerioSequenceType[][] RowTypes;
		///<summary>Width of the left column that holds descriptions and dates.</summary> 
		private int _widthLeft;
		///<summary>Height of the 'tooth' sections. Right now, it just holds the tooth number.</summary> 
		private int _heightTooth;
		///<summary>Color of the outer border and the major black dividers.</summary>
		private Color _colorLinesMain;
		///<summary>Color of the background section of the shorter inner rows.</summary>
		private Color cBackShort;
		///<summary>Color of a highlighted cell.</summary>
		private Color cHi;
		///<summary>Color of the text of a skipped tooth.</summary>
		private Color cSkip;
		///<summary>Color of the vertical lines between each tooth.</summary>
		private Color _colorLinesMinor;
		//<summary>Color of the minor horizontal lines between rows.</summary>
		//private Color cHoriz;
		///<summary>Color of the main background.</summary>
		private Color cBack;
		//<summary>Color of the horizontal lines in the shorter inner rows.</summary>
		//private Color cHorizShort;
		///<summary>Color of red probing depths.</summary>
		private Color cRedText;
		///<summary>Color of the dot over a number representing blood.</summary>
		private Color cBlood;
		///<summary>Color of the dot over a number representing suppuration.</summary>
		private Color cSupp;
		///<summary>Color of the dot over a number representing plaque.</summary>
		private Color cPlaque;
		///<summary>Color of the dot over a number representing calculus.</summary>
		private Color cCalc;
		///<summary>Color of previous measurements from a different exam. Slightly grey.</summary>
		private Color cOldText;
		///<summary>Color of previous red measurements from a different exam. Lighter red.</summary>
		private Color cOldTextRed;
		///<summary>This data array gets filled when loading an exam. It is altered as the user makes changes, and then the results are saved to the db by reading from this array.</summary>
		private PerioCell[,] DataArray;
		///<summary>Since it's complex to compute X coordinate of each cell, the values are stored in this array.  Used by GetBounds and mouse clicking.</summary>
		private float[] _colPosArray;
		///<summary>Since it's complex to compute Y coordinate of each cell, the values are stored in this array.  Used by GetBounds and mouse clicking.</summary>
		private float[] _rowPosArray;
		///<summary>Stores the column,row of the currently selected cell. Null if none selected.</summary>
		public Point CurCell;
		///<summary>Auto advance direction.</summary>
		public AutoAdvanceDirection Direction;
		///<summary>The index in PerioExams.List of the currently selected exam.</summary>
		private int selectedExam;
		///<summary>the offset when there are more rows than will display. Value is set at the same time as SelectedExam. So usually 0. Otherwise 1,2,3 or....</summary>
		private int ProbingOffset;
		///<summary>Keeps track of what has changed for current exam.  This object contains an toothnum int and a PerioSequenceType enum.</summary>
		public List<PerioMeasureItem> listChangedMeasurements;
		///<summary>Valid values 1-32 int. User can highlight teeth to mark them as skip tooth. The highighting is done completely separately from the normal highlighting functionality because multiple teeth can be highlighted.</summary>
		private ArrayList selectedTeeth;
		///<summary>Valid values 1-32 int. Applies only to the current exam. Loaded from the db durring LoadData().</summary>
		private List<int> skippedTeeth;
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
		public bool perioEdit;
		///<summary>True if all gingival margin values entered should be positive. (Stored in DB as negative.)</summary>
		public bool GingMargPlus;
		///<summary>Used to determine if a tooth had an implant proc.</summary>
		private static List<Procedure> _listPatProcs=null;
		///<summary>If false, only show exams from today.</summary>
		private bool _doShowCurrentExamOnly=true;

		///<summary>A rectangle that represents the drawn perio chart within the control itself, this changes based on zoom settings.</summary>
		public Rectangle RectBoundsShowing {
			get{
				return new Rectangle(new Point(0,0),new Size(_widthShowing+2,_heightShowing+2));//Add 2 pixels to the top and bottom for the border.
			}
		}

		///<summary>The index in PerioExams.List of the currently selected exam.</summary>
		public int SelectedExam{
			get{
				return selectedExam;
			}
			set{
				selectedExam=value;
				SetProbingOffset();
			}
		}

		///<summary>If false, only show exams from today.</summary>
		public bool DoShowCurrentExamOnly {
			get {
				return _doShowCurrentExamOnly;
			}
			set {
				_doShowCurrentExamOnly=value;
				SetProbingOffset();
			}
		}

		private void SetProbingOffset() {
			ProbingOffset=0;
			if(_doShowCurrentExamOnly) {
				ProbingOffset=selectedExam;
			}
			else if(selectedExam>RowsProbing-1) {
				ProbingOffset=selectedExam-RowsProbing+1;
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
			_listPatProcs=null;
			//InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			DoubleBuffered=true;
			this.BackColor = System.Drawing.SystemColors.Window;
			_colorLinesMain=Color.Black;
			//cBackShort=Color.FromArgb(237,237,237);//larger numbers will make it whiter
			cBackShort=Color.FromArgb(225,225,225);
			cHi=Color.FromArgb(158,146,142);//Color.DarkSalmon;
			cSkip=Color.LightGray;
			_colorLinesMinor=Color.Silver;
			//cHoriz=Color.LightGray;
			cBack=Color.White;
			//cHorizShort=Color.Silver;//or darkgrey
			cRedText=Color.Red;
			SetColors();
			cOldText=Color.FromArgb(120,120,120);
			cOldTextRed=Color.FromArgb(200,80,80);
			RowsProbing=6;
			RowTypes=new PerioSequenceType[4][];
			//Upper facial:
			RowTypes[0]=new PerioSequenceType[5+RowsProbing];
			RowTypes[0][0]=PerioSequenceType.Mobility;
			RowTypes[0][1]=PerioSequenceType.Furcation;
			RowTypes[0][2]=PerioSequenceType.CAL;
			RowTypes[0][3]=PerioSequenceType.GingMargin;
			RowTypes[0][4]=PerioSequenceType.MGJ;
			for(int i=0;i<RowsProbing;i++){
				RowTypes[0][5+i]=PerioSequenceType.Probing;
			}
			//Upper lingual:
			RowTypes[1]=new PerioSequenceType[3+RowsProbing];
			RowTypes[1][0]=PerioSequenceType.Furcation;
			RowTypes[1][1]=PerioSequenceType.CAL;
			RowTypes[1][2]=PerioSequenceType.GingMargin;
			for(int i=0;i<RowsProbing;i++){
				RowTypes[1][3+i]=PerioSequenceType.Probing;
			}
			//Lower lingual:
			RowTypes[2]=new PerioSequenceType[4+RowsProbing];
			RowTypes[2][0]=PerioSequenceType.Furcation;
			RowTypes[2][1]=PerioSequenceType.CAL;
			RowTypes[2][2]=PerioSequenceType.GingMargin;
			RowTypes[2][3]=PerioSequenceType.MGJ;
			for(int i=0;i<RowsProbing;i++){
				RowTypes[2][4+i]=PerioSequenceType.Probing;
			}
			//Lower facial:
			RowTypes[3]=new PerioSequenceType[5+RowsProbing];
			RowTypes[3][0]=PerioSequenceType.Mobility;
			RowTypes[3][1]=PerioSequenceType.Furcation;
			RowTypes[3][2]=PerioSequenceType.CAL;
			RowTypes[3][3]=PerioSequenceType.GingMargin;
			RowTypes[3][4]=PerioSequenceType.MGJ;
			for(int i=0;i<RowsProbing;i++){
				RowTypes[3][5+i]=PerioSequenceType.Probing;
			}
			ClearDataArray();
			CurCell=new Point(-1,-1);//my way of setting it to null.
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

		/*
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ContrPerio
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Name = "ContrPerio";
		}
		#endregion
		*/

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
				cBlood=Color.FromArgb(240,20,20);
				cSupp=Color.FromArgb(255,160,0);
				cPlaque=Color.FromArgb(240,20,20);
				cCalc=Color.FromArgb(255,160,0);
			}
			else {
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
				cBlood=listDefs[(int)DefCatMiscColors.PerioBleeding].ItemColor;
				cSupp=listDefs[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
				cPlaque=listDefs[(int)DefCatMiscColors.PerioPlaque].ItemColor;
				cCalc=listDefs[(int)DefCatMiscColors.PerioCalculus].ItemColor;
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
			int yPos1=1+RowsProbing*(_heightProb+1);
			int yPos2=yPos1+(RowTypes[0].Length-RowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(cBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+_heightTooth+1;
			yPos2=yPos1+(RowTypes[1].Length-RowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(cBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+RowsProbing*(_heightProb+1)+1+RowsProbing*(_heightProb+1);
			yPos2=yPos1+(RowTypes[2].Length-RowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(cBackShort),0,top,_widthShowing,bottom-top);
			yPos1=yPos2+1+_heightTooth+1;
			yPos2=yPos1+(RowTypes[3].Length-RowsProbing)*(_heightShort+1)-1;
			top=yPos1;
			bottom=yPos2;
			g.FillRectangle(new SolidBrush(cBackShort),0,top,_widthShowing,bottom-top);
		}

		///<summary>Draws the greyed out background for the skipped teeth.</summary>
		private void DrawSkippedTeeth(System.Windows.Forms.PaintEventArgs e){
			if(skippedTeeth==null || skippedTeeth.Count==0)
				return;
			Graphics g=e.Graphics;
			float xLoc=0;
			float yLoc=0;
			float h=0;
			float w=0;
			RectangleF bounds;//used in the loop to represent the bounds of the entire tooth to be greyed
			for(int i=0;i<skippedTeeth.Count;i++){
				if((int)skippedTeeth[i]<17){//max tooth
					xLoc=1+_widthLeft+1+((int)skippedTeeth[i]-1)*_widthTooth;
					//xLoc=1+Wleft+1+(col-1)*(Wmeas+1);
					yLoc=1;
					h=_rowPosArray[GetTableRow(1,RowTypes[1].Length-1)]-yLoc+_heightProb;
					w=_widthTooth;
				}
				else{//mand tooth
					xLoc=1+_widthLeft+1+(33-(int)skippedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(2,RowTypes[2].Length-1)];
					h=_rowPosArray[GetTableRow(3,RowTypes[3].Length-1)]-yLoc+_heightProb;
					w=_widthTooth;
				}
				bounds=new RectangleF(xLoc,yLoc,w,h);
				int top=(int)bounds.Top;
				int bottom=(int)bounds.Bottom;
				int left=(int)bounds.Left;
				int right=(int)bounds.Right;
				//test clipping rect later
				//MessageBox.Show(bounds.ToString());
				g.FillRectangle(new SolidBrush(cBackShort),left,top
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
			float h=0;
			float w=0;
			RectangleF bounds;//used in the loop to represent the bounds to be greyed
			for(int i=0;i<selectedTeeth.Count;i++){
				if((int)selectedTeeth[i]<17){//max tooth
					xLoc=1+_widthLeft+1+((int)selectedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(true)];
					h=_heightTooth;
					w=_widthTooth;
				}
				else{//mand tooth
					xLoc=1+_widthLeft+1+(33-(int)selectedTeeth[i]-1)*_widthTooth;
					yLoc=_rowPosArray[GetTableRow(false)];
					h=_heightTooth;
					w=_widthTooth;
				}
				bounds=new RectangleF(xLoc,yLoc,w,h);
				int top=(int)bounds.Top;
				int bottom=(int)bounds.Bottom;
				int left=(int)bounds.Left;
				int right=(int)bounds.Right;
				//test clipping rect later
				g.FillRectangle(new SolidBrush(cHi),left,top
					,right-left,bottom-top);
			}
		}

		///<summary>Draws the highlighted background for the current cell.</summary>
		private void DrawCurCell(System.Windows.Forms.PaintEventArgs e){
			if(CurCell.X==-1){
				return;
			}
			if(!perioEdit) {
				return;
			}
			Graphics g=e.Graphics;
			RectangleF bounds=GetBounds(CurCell.X,CurCell.Y);
			if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.Probing){
				bounds=new RectangleF(bounds.X,bounds.Y+_heightProb-_heightShort,
					bounds.Width,_heightShort);
			}
			int top=(int)bounds.Top;
			int bottom=(int)bounds.Bottom;
			int left=(int)bounds.Left;
			int right=(int)bounds.Right;
			if(e.ClipRectangle.Bottom>=bounds.Top && e.ClipRectangle.Top<=bounds.Bottom
				&& e.ClipRectangle.Right>=bounds.Left && e.ClipRectangle.Left<=bounds.Right)
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
				g.FillRectangle(new SolidBrush(cHi),left,top
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
			y=(int)_rowPosArray[GetTableRow(1,RowTypes[1].Length)];
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
			for(int i=0;i<DataArray.GetLength(1);i++){//loop through all rows in the table
				DrawRow(i,e);
			}
		}

		private void DrawRow(int row,PaintEventArgs e){
			Graphics g=e.Graphics;
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;//.AntiAlias;
			//e.Graphics.PixelOffsetMode=PixelOffsetMode.HighQuality;
			//e.Graphics.TextRenderingHint=TextRenderingHint.AntiAlias;
			Font font;
			Color textColor;
			StringFormat format=new StringFormat();
			RectangleF rect;
			bool drawOld;
			int redThresh=0;
			int cellValue=0;
			for(int i=0;i<DataArray.GetLength(0);i++){//loop through all columns in the row
				rect=GetBounds(i,row);
				font=(Font)Font.Clone();
				//font=new Font("Arial",8,FontStyle.Regular);
				textColor=Color.Black;
				//test for clip later
				if(i==0){//first column, dates and row labels
					float x=rect.Right-g.MeasureString(DataArray[i,row].Text,font).Width-1;
					float y=rect.Top-1;
					if(GetSection(row)!=-1 && RowTypes[GetSection(row)][GetSectionRow(row)]==PerioSequenceType.Probing){//date
						y+=LayoutManager.ScaleF(3);
					}
					e.Graphics.DrawString(DataArray[i,row].Text,font,new SolidBrush(textColor),x,y);
					continue;
				}
				else if(GetSection(row)==-1){//tooth row
					//Debug.WriteLine("row:"+row+" col:"+i);
					font=new Font(Font,FontStyle.Bold);
					format.Alignment=StringAlignment.Center;
					rect=new RectangleF(rect.X,rect.Y+2,rect.Width,rect.Height);
					e.Graphics.DrawString(DataArray[i,row].Text,font,
						new SolidBrush(textColor),rect,format);
					//e.Graphics.DrawString(DataArray[i,row].Text,Font,Brushes.Black,rect);
					continue;
				}
				format.Alignment=StringAlignment.Center;//center left/right
				if(RowTypes[GetSection(row)][GetSectionRow(row)]==PerioSequenceType.Probing){
					if((DataArray[i,row].Bleeding & BleedingFlags.Plaque) > 0){
						e.Graphics.FillRectangle(new SolidBrush(cPlaque),rect.X+0,rect.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((DataArray[i,row].Bleeding & BleedingFlags.Calculus) > 0){
						e.Graphics.FillRectangle(new SolidBrush(cCalc),rect.X+LayoutManager.ScaleF(2.5f),rect.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((DataArray[i,row].Bleeding & BleedingFlags.Blood) > 0){
						e.Graphics.FillRectangle(new SolidBrush(cBlood),rect.X+LayoutManager.ScaleF(5f),rect.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					if((DataArray[i,row].Bleeding & BleedingFlags.Suppuration) > 0){
						e.Graphics.FillRectangle(new SolidBrush(cSupp),rect.X+LayoutManager.ScaleF(7.5f),rect.Y,LayoutManager.ScaleF(2.5f),LayoutManager.ScaleF(3.5f));
					}
					rect=new RectangleF(rect.X,rect.Y+4,rect.Width,rect.Height);
				}
				if((DataArray[i,row].Text==null || DataArray[i,row].Text=="")
						&& (DataArray[i,row].OldText==null || DataArray[i,row].OldText=="")){
					continue;//no text to draw
				}
				if(DataArray[i,row].Text==null || DataArray[i,row].Text==""){//No data recorded by user for the current cell. Display the old text from the previous perio exam.
					drawOld=true;
					cellValue=PIn.Int(DataArray[i,row].OldText);
					if(cellValue>100) {//used for negative numbers
						cellValue=100-cellValue;//i.e. 100-103 = -3
					}
					textColor=Color.Gray;
				}
				else{
					drawOld=false;
					cellValue=PIn.Int(DataArray[i,row].Text);
					if(cellValue>100) {//used for negative numbers
						cellValue=100-cellValue;//i.e. 100-103 = -3
					}
					textColor=Color.Black;
					if(!perioEdit) {
						textColor=Color.Gray;
					}
				}
				//test for red
				switch(RowTypes[GetSection(row)][GetSectionRow(row)]){
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
				if((RowTypes[GetSection(row)][GetSectionRow(row)]
					==PerioSequenceType.MGJ && cellValue<=redThresh)
					||(RowTypes[GetSection(row)][GetSectionRow(row)]
					!=PerioSequenceType.MGJ && cellValue>=redThresh))
				{
					if(drawOld) {
						textColor=cOldTextRed;
					}
					else {
						textColor=cRedText;
						if(!perioEdit) {
							textColor=cOldTextRed;
						}
					}
					font=new Font(Font,FontStyle.Bold);
				}
				//if number is two digits:
				if(cellValue>9){
					font=new Font("SmallFont",LayoutManager.ScaleF(7));
					rect=new RectangleF(rect.X-2,rect.Y+1,rect.Width+5,rect.Height);
				}
				//if number is negative. "+" symbol is wider than "1" symbol so hand it seperately here.
				if(cellValue<0) {
					font=new Font("SmallFont",LayoutManager.ScaleF(7));
					rect=new RectangleF(rect.X-3,rect.Y+1,rect.Width+5,rect.Height);//shift text left, 1 more pixel than usual.
				}
				//e.Graphics.DrawString(cellValue.ToString(),Font,Brushes.Black,rect);
				if((RowTypes[GetSection(row)][GetSectionRow(row)]==PerioSequenceType.GingMargin)) {
					e.Graphics.DrawString(cellValue.ToString().Replace('-','+'),font,new SolidBrush(textColor),rect.X,rect.Y);//replace '-' with '+' for Gingival Margin Hyperplasia
				}
				else {
					e.Graphics.DrawString(cellValue.ToString(),font,new SolidBrush(textColor),rect.X,rect.Y);
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
			else if(RowTypes[GetSection(row)][GetSectionRow(row)]==PerioSequenceType.Probing){//probing
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
			_rowPosArray=new float[DataArray.GetLength(1)];
			//int curRow=0;
			int yPos=0;
			//U facial
			for(int i=RowTypes[0].Length-1;i>=0;i--){
				_rowPosArray[GetTableRow(0,i)]=yPos;
				//MessageBox.Show(GetTableRow(0,i));
				if(RowTypes[0][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			_rowPosArray[GetTableRow(true)]=yPos;
			yPos+=_heightTooth+1;
			//upper lingual
			for(int i=0;i<RowTypes[1].Length;i++){
				_rowPosArray[GetTableRow(1,i)]=yPos;
				if(RowTypes[1][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			yPos+=1;//makes a double line between u and L
			//lower lingual
			//MessageBox.Show(GetTableRow(2,3).ToString());
			for(int i=RowTypes[2].Length-1;i>=0;i--){
				_rowPosArray[GetTableRow(2,i)]=yPos;
				if(RowTypes[2][i]==PerioSequenceType.Probing){
					yPos+=_heightProb+1;
				}
				else{
					yPos+=_heightShort+1;
				}
			}
			_rowPosArray[GetTableRow(false)]=yPos;
			yPos+=_heightTooth+1;
			//lower facial
			for(int i=0;i<RowTypes[3].Length;i++){
				_rowPosArray[GetTableRow(3,i)]=yPos;
				if(RowTypes[3][i]==PerioSequenceType.Probing){
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
		public void LoadData(bool doSelectCell=true){
			ClearDataArray();
			selectedTeeth=new ArrayList();
			skippedTeeth=new List<int>();
			//reset the list of modified teeth
			listChangedMeasurements=new List<PerioMeasureItem>();
			if(selectedExam==-1){
				return;
			}
			FillDates();
			Point curCell;
			PerioCell perioCellCur;
			//int examI=selectedExam;
			string cellText="";
			int cellBleed=0;
			for(int examI=0;examI<=selectedExam;examI++){//exams, earliest through current
				for(int seqI=0;seqI<PerioMeasures.List.GetLength(1);seqI++){//sequences
					for(int toothI=1;toothI<PerioMeasures.List.GetLength(2);toothI++){//measurements
						if(PerioMeasures.List[examI,seqI,toothI]==null)//.PerioMeasureNum==0)
							continue;//no measurement for this seq and tooth
						for(int surfI=0;surfI<Enum.GetNames(typeof(PerioSurf)).Length;surfI++){//surfaces(6or7)
							if(seqI==(int)PerioSequenceType.SkipTooth){
								//There is nothing in the data array to fill because it is not user editable.
								if(surfI!=(int)PerioSurf.None){
									continue;
								}
								if(examI!=selectedExam){//only mark skipped teeth for current exam
									continue;
								}
								if(PerioMeasures.List[examI,seqI,toothI].ToothValue==1){
									skippedTeeth.Add(toothI);
								}
								continue;
							}
							else if(seqI==(int)PerioSequenceType.Mobility){
								if(surfI!=(int)PerioSurf.None){
									continue;
								}
								curCell=GetCell(examI,PerioMeasures.List[examI,seqI,toothI].SequenceType,toothI,PerioSurf.B);
								perioCellCur=GetPerioCell(curCell,false);
								cellText=PerioMeasures.List[examI,seqI,toothI].ToothValue.ToString();
								if(cellText=="-1"){
									cellText="";
								}
								if(examI==selectedExam) {
									perioCellCur.Text=cellText;
									DataArray[curCell.X,curCell.Y]=perioCellCur;
								}
								else {
									perioCellCur.OldText=cellText;
									DataArray[curCell.X,curCell.Y]=perioCellCur;
								}
								continue;
							}
							else if(seqI==(int)PerioSequenceType.Bleeding){
								if(surfI==(int)PerioSurf.None){
									continue;
								}
								curCell=GetCell(examI,PerioSequenceType.Probing,toothI,(PerioSurf)surfI);
								if(curCell.X==-1 || curCell.Y==-1)
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
								perioCellCur=GetPerioCell(curCell,false);
								if(cellBleed==-1) {//this shouldn't happen, but just in case
									cellBleed=0;
								}
								perioCellCur.Bleeding=(BleedingFlags)cellBleed;
								DataArray[curCell.X,curCell.Y]=perioCellCur;
								continue;
							}
							curCell=GetCell(examI,PerioMeasures.List[examI,seqI,toothI].SequenceType,toothI,(PerioSurf)surfI);
							if(curCell.X==-1 || curCell.Y==-1)
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
							perioCellCur=GetPerioCell(curCell,false);
							if (cellText=="-1") {//seqI==2 means it is gingival margin.
								cellText="";
							}
							if(examI==selectedExam) {
								perioCellCur.Text=cellText;
								DataArray[curCell.X,curCell.Y]=perioCellCur;
							}
							else {
								perioCellCur.OldText=cellText;
								DataArray[curCell.X,curCell.Y]=perioCellCur;
							}
							//calculate CAL. All ging will have been done before any probing.
							if(seqI==(int)PerioSequenceType.Probing){
								CalculateCAL(curCell,false);
							}
						}//for surfI
					}//for toothI
				}//for seqI
			}//for examI
			//Start in the very first cell on the first tooth and loop through teeth until we come across one that is not missing.
			if(doSelectCell) {
				CurCell=new Point(1,GetTableRow(selectedExam,0,PerioSequenceType.Probing));
				OnDirectionChangedLeft();//Always start looping to the left.
				if(skippedTeeth.Count==32) {
					return;
				}
				int curTooth=GetToothNumCur(GetSection(CurCell.Y));
				while(skippedTeeth.Contains(curTooth)) {
					AdvanceCell();//Advance forward 3 times, since there are 3 measurements per tooth.
					AdvanceCell();
					AdvanceCell();
					curTooth=GetToothNumCur(GetSection(CurCell.Y));
				}
			}
		}

		///<summary>Used in LoadData.</summary>
		private void FillDates(){
			int tableRow;
			for(int examI=0;examI<selectedExam+1;examI++){//-ProbingOffset;examI++){
				for(int section=0;section<4;section++){
					tableRow=GetTableRow(examI,section,PerioSequenceType.Probing);
					if(tableRow==-1)
						continue;
					DataArray[0,tableRow].Text
						=PerioExams.ListExams[examI].ExamDate.ToShortDateString();
						//=PerioExams.List[examI+ProbingOffset].ExamDate.ToShortDateString();
				}
			}
		}

		///<summary>Used in LoadData.</summary>
		private Point GetCell(int examIndex,PerioSequenceType seqType,int intTooth,PerioSurf surf){
			int col=0;
			int row=0;
			if(intTooth<=16){
				col=(intTooth*3)-2;//left-most column
				if(surf==PerioSurf.B || surf==PerioSurf.L){
					col++;
				}
				if(intTooth<=8){
					if(surf==PerioSurf.MB || surf==PerioSurf.ML)
						col+=2;
				}
				else{//9-16
					if(surf==PerioSurf.DB || surf==PerioSurf.DL)
						col+=2;
				}
			}
			else{//17-32
				col=((33-intTooth)*3)-2;//left-most column
				if(surf==PerioSurf.B || surf==PerioSurf.L){
					col++;
				}
				if(intTooth>=25){
					if(surf==PerioSurf.MB || surf==PerioSurf.ML)
						col+=2;
				}
				else{//17-24
					if(surf==PerioSurf.DB || surf==PerioSurf.DL)
						col+=2;
				}
			}
			int section;
			if(intTooth<=16){
				if(surf==PerioSurf.MB || surf==PerioSurf.B || surf==PerioSurf.DB){
					section=0;
				}
				else{//Lingual
					section=1;
				}
			}
			else{//17-32
				if(surf==PerioSurf.MB || surf==PerioSurf.B || surf==PerioSurf.DB){
					section=3;
				}
				else{//Lingual
					section=2;
				}
			}
			row=GetTableRow(examIndex,section,seqType);
			return new Point(col,row);
		}

		///<summary>Returns the starting point(position) for the passed in tooth.</summary>
		public Point GetAutoAdvanceCustomPoint(int toothNum,PerioSurface surf,PerioSequenceType seqType,bool isReverse) {
			int x=-1;
			int y=-1;
			bool isFacial=surf==PerioSurface.Facial;
			if(isFacial) {
				if(toothNum.Between(1,16)) {
					x=(toothNum)*3-(isReverse ? 0 : 2);//left most column. if isReverse, than right most column.
					if(seqType==PerioSequenceType.Mobility) {
						x+=(isReverse ? -1 : 1);//Middle column
					}
				}
				else {//Tooth 17-32
					x=(32-toothNum)*3+ (isReverse ? 1 : 3);//Right most column. if isReverse, get left most column
					if(seqType==PerioSequenceType.Mobility) {
						x+=(isReverse ? 1 : -1);//middle column
					}
				}
			}
			else {//Lingual
				if(toothNum.Between(1,16)) {
					x=(toothNum)*3 + (isReverse ? -2 : 0);//Right most column. If isReverse, get left most column.
				}
				else {//Tooth 17-32
					x=(32-toothNum)*3 + (isReverse ? 3 : 1);//Left most column. If isReverse, get right most column.
				}
			}
			y=GetTableRow(SelectedExam,GetSection(toothNum,surf),seqType);
			return new Point(x,y);
		}

		public PerioSequenceType GetSequenceForPoint(Point point) {
			return RowTypes[GetSection(point.Y)][GetSectionRow(point.Y)];
		}

		///<summary>Saves the cur exam measurements to the db by looping through each tooth and deciding whether the data for that tooth has changed.  If so, it either updates or inserts a measurement.  It won't delete a measurement if all values for that tooth are cleared, but just sets that measurement to all -1's.</summary>
		public void SaveCurExam(PerioExam PerioExamCur) {
			PerioMeasure PerioMeasureCur;
			if(listChangedMeasurements.Count==0) {
				return;//no changes. No need to insert/update periomeasures.
			}
			PerioExam perioExamFromDb=PerioExams.GetOnePerioExam(PerioExamCur.PerioExamNum);
			if(perioExamFromDb==null || PerioExamCur.DateTMeasureEdit!=perioExamFromDb.DateTMeasureEdit) {
				//something has changed
				MsgBox.Show(this,"This perio exam has been altered by another user. A new exam has been created and saved.");
				PerioExamCur.PerioExamNum=0;
				PerioExams.Insert(PerioExamCur);
				List<PerioExam> perioExamList=PerioExams.GetExamsList(PerioExamCur.PatNum);
				PerioMeasures.Refresh(PerioExamCur.PatNum,perioExamList);
				//isNewExam=true;
				PerioExamCur.IsNew=true;
			}
			//continue saving periomeasures.Update exam with new datetime.
			if(!PerioExamCur.IsNew) {
				PerioExamCur.DateTMeasureEdit=MiscData.GetNowDateTime();
				PerioExams.Update(PerioExamCur);
			}
			//In case a tooth was toggled multiple times
			listChangedMeasurements=listChangedMeasurements.Distinct().ToList();
			//If a tooth is marked as bleeding, it will only have a PerioSequence.Probing entry in HasChanged.	We need to add a HasChanged entry for
			//Bleeding as well.
			List<PerioMeasureItem> listBleeding=listChangedMeasurements.Where(x => x.SeqType==PerioSequenceType.Probing)
				.Select(x => new PerioMeasureItem(PerioSequenceType.Bleeding,x.ToothNum)).ToList();
			listChangedMeasurements.AddRange(listBleeding);
			foreach(PerioMeasureItem measureItem in listChangedMeasurements) {
				PerioSequenceType seqI=measureItem.SeqType;
				int toothI=measureItem.ToothNum;
				//new measurement
				if(PerioMeasures.List[selectedExam,(int)seqI,toothI]==null || PerioExamCur.IsNew) {//.PerioMeasureNum==0){
					//MessageBox.Show(toothI.ToString());
					PerioMeasureCur=new PerioMeasure();
					PerioMeasureCur.PerioExamNum=PerioExamCur.PerioExamNum;
					PerioMeasureCur.SequenceType=seqI;
					PerioMeasureCur.IntTooth=toothI;
				}
				else{
					PerioMeasureCur=PerioMeasures.List[selectedExam,(int)seqI,toothI];
					//PerioExam
					//Sequence
					//tooth
				}
				if(seqI==PerioSequenceType.Mobility || seqI==PerioSequenceType.SkipTooth){
					PerioMeasureCur.MBvalue=-1;
					PerioMeasureCur.Bvalue=-1;
					PerioMeasureCur.DBvalue=-1;
					PerioMeasureCur.MLvalue=-1;
					PerioMeasureCur.Lvalue=-1;
					PerioMeasureCur.DLvalue=-1;
					if(seqI==PerioSequenceType.Mobility){
						PerioMeasureCur.ToothValue
							=GetCellValue(selectedExam,seqI,toothI,PerioSurf.B);
					}
					else{//skiptooth
						//skipped teeth are only saved when user marks them, not as part of regular saving.
					}
				}
				else if(seqI==PerioSequenceType.Bleeding){
					PerioMeasureCur.ToothValue=-1;
					PerioMeasureCur.MBvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.MB);
					PerioMeasureCur.Bvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.B);
					PerioMeasureCur.DBvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.DB);
					PerioMeasureCur.MLvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.ML);
					PerioMeasureCur.Lvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.L);
					PerioMeasureCur.DLvalue
						=GetCellBleedValue(selectedExam,toothI,PerioSurf.DL);
				}
				else{
					PerioMeasureCur.ToothValue=-1;
					PerioMeasureCur.MBvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.MB);
					PerioMeasureCur.Bvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.B);
					PerioMeasureCur.DBvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.DB);
					PerioMeasureCur.MLvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.ML);
					PerioMeasureCur.Lvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.L);
					PerioMeasureCur.DLvalue
						=GetCellValue(selectedExam,seqI,toothI,PerioSurf.DL);
				}
				//then to the database
				if(seqI==PerioSequenceType.Mobility && PerioMeasureCur.ToothValue==-1 && !PerioExamCur.IsNew) {
					PerioMeasures.Delete(PerioMeasureCur);//-1 is an invalid value for mobility, keep Db bloat down
				}
				else if(PerioMeasures.List[selectedExam,(int)seqI,toothI]==null || PerioExamCur.IsNew) {
					PerioMeasures.Insert(PerioMeasureCur);
				}
				else{
					PerioMeasures.Update(PerioMeasureCur);
				}
			}//for seqI
		}

		///<summary>Used in SaveCurExam to retrieve data from grid to save it.</summary>
		private int GetCellValue(int examIndex,PerioSequenceType seqType,int intTooth,PerioSurf surf){
			Point curCell=GetCell(examIndex,seqType,intTooth,surf);
			if(curCell.X==-1 || curCell.Y==-1){
				return -1;
			}
			//if(intTooth==4)
			//MessageBox.Show(DataArray[curCell.X,curCell.Y].Text);
			if(DataArray[curCell.X,curCell.Y].Text==null || DataArray[curCell.X,curCell.Y].Text==""){
				//MessageBox.Show("empty");
				return -1;
			}
			//MessageBox.Show("full");
			return PIn.Int(DataArray[curCell.X,curCell.Y].Text);
		}

		public string GetCellText(int x,int y) {
			if(x==-1 || y==-1 || DataArray[x,y].Text==null) {
				return "";
			}
			return DataArray[x,y].Text;
		}

		///<summary>Used in SaveCurExam to retrieve data from grid to save it.</summary>
		private int GetCellBleedValue(int examIndex,int intTooth,PerioSurf surf){
			Point curCell=GetCell(examIndex,PerioSequenceType.Probing,intTooth,surf);
			if(curCell.X==-1 || curCell.Y==-1){
				return 0;
			}
			return (int)DataArray[curCell.X,curCell.Y].Bleeding;
		}

		private void ClearDataArray(){
			//MessageBox.Show("clearing");
			DataArray=new PerioCell[49,RowTypes[0].Length+RowTypes[1].Length
				+RowTypes[2].Length+RowTypes[3].Length+2];//the 2 is for the tooth cells.
			//int curX=0;
			int curY=0;
			for(int sect=0;sect<4;sect++){
				for(int i=0;i<RowTypes[sect].Length;i++){
					curY=GetTableRow(sect,i);
					switch(RowTypes[sect][i]){
						case PerioSequenceType.Mobility:
							DataArray[0,curY].Text=Lan.g(this,"Mobility");
							break;
						case PerioSequenceType.Furcation:
							DataArray[0,curY].Text=Lan.g(this,"Furc");
							break;
						case PerioSequenceType.CAL:
							DataArray[0,curY].Text=Lan.g(this,"auto CAL");
							break;
						case PerioSequenceType.GingMargin:
							DataArray[0,curY].Text=Lan.g(this,"Ging Marg");
							break;
						case PerioSequenceType.MGJ:
							DataArray[0,curY].Text=Lan.g(this,"MGJ");
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
			curY=GetTableRow(true);
			try{
				for(int i=1;i<=16;i++){
					DataArray[3*i-1,curY].Text=Tooth.ToInternat(i.ToString());
					if(IsImplant(i)) {
						DataArray[3*i-1,curY].Text+="i";
					}
				}
				curY=GetTableRow(false);
				for(int i=1;i<=16;i++){
					DataArray[3*i-1,curY].Text=Tooth.ToInternat((33-i).ToString());
					if(IsImplant(33-i)) {
						DataArray[3*i-1,curY].Text+="i";
					}
				}
			}
			catch{
				//won't work in design mode
			}
		}

		///<summary>Returns true if the toothNum passed in has ever had an implant before.</summary>
		public static bool IsImplant(int toothNum) {
			if(_listPatProcs==null) {//Not initialized yet.
				_listPatProcs=Procedures.Refresh(FormOpenDental.CurPatNum);
			}
			List<Procedure> listProcsForTooth=_listPatProcs.FindAll(x => x.ToothNum==toothNum.ToString() && ListTools.In(x.ProcStatus,
				ProcStat.C,ProcStat.EC,ProcStat.EO));
			for(int i = 0;i<listProcsForTooth.Count;i++) {
				ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcsForTooth[i].CodeNum);
				if(procCode.PaintType==ToothPaintingType.Implant) {
					return true;
				}
			}
			return false;
		}

		///<summary>Used in GetCell during LoadData. Also used in AdvanceCell when looping to a new section.</summary>
		private int GetTableRow(int examIndex,int section,PerioSequenceType seqType){
			if(seqType==PerioSequenceType.Probing || seqType==PerioSequenceType.Bleeding){
				if(examIndex-ProbingOffset<0)//older exam that won't fit.
					return -1;
				int sectionRow=examIndex-ProbingOffset//correct for offset
					+RowTypes[section].Length-RowsProbing;//plus number of non-probing rows
				return GetTableRow(section,sectionRow);
			}
			//for types other than probing and bleeding, do a loop through the non-probing rows:
			for(int i=0;i<RowTypes[section].Length-RowsProbing;i++){
				if(RowTypes[section][i]==seqType)
					return GetTableRow(section,i);
			}
			//MessageBox.Show("Error in GetTableRows: seqType not found");
			return -1;
		}

		private int GetTableRow(int section,int sectionRow){
			int retVal=0;
			if(section==0){
				retVal=RowTypes[0].Length-sectionRow-1;
			}
			else if(section==1){
				retVal=RowTypes[0].Length+1+sectionRow;
			}
			else if(section==2){
				retVal=RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length-sectionRow-1;
			}
			else
				retVal=RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length+1+sectionRow;
			return retVal;
		}

		///<summary>If true, then returns the row of the max teeth, otherwise mand.</summary>
		private int GetTableRow(bool getMax){
			if(getMax){
				return RowTypes[0].Length;
			}
			return RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length;
		}

		
		///<summary>Returns -1 if tooth row and not section row.  0 = Maxillary Facial, 1 = Maxillary Lingual, 2 = Mandible Facial, 3 = Mandible Lingual.
		///Used in GetBounds, DrawRow, and OnMouseDown.</summary>
		private int GetSection(int tableRow){
			if(tableRow<RowTypes[0].Length) {//0 = Maxillary Facial
				return 0;
			}
			if(tableRow==RowTypes[0].Length){
				return -1;//max teeth
			}
			if(tableRow<RowTypes[0].Length+1+RowTypes[1].Length) {//1 = Maxillary Lingual
				return 1;
			}
			if(tableRow<RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length) {//2 = Mandible Facial
				return 2;
			}
			if(tableRow==RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length) {//3 = Mandible Lingual
				return -1;//mand teeth
			}
			return 3;
		}

		///<summary>Returns the section for the passed in tooth. The section is used to determine the Y position of a point.</summary>
		public int GetSection(int intTooth,PerioSurface surf) {
			int section;
			if(intTooth<=16) {
				if(surf==PerioSurface.Facial) {
					section=0;//0 = Maxillary Facial
				}
				else {//Lingual
					section=1;//1 = Maxillary Lingual
				}
			}
			else {//17-32
				if(surf==PerioSurface.Facial) {
					section=3;//3 = Mandible Lingual
				}
				else {//Lingual
					section=2;//2 = Mandible Facial
				}
			}
			return section;
		}

		///<summary>Returns -1 if a tooth row and not a section row.  Used in GetBounds,SetHasChanged, AdvanceCell, and DrawRow</summary>
		private int GetSectionRow(int tableRow){
			if(tableRow<RowTypes[0].Length){
				return RowTypes[0].Length-tableRow-1;
			}
			//return 0;
			if(tableRow==RowTypes[0].Length){
				return -1;//max teeth
			}
			if(tableRow<RowTypes[0].Length+1+RowTypes[1].Length){
				return tableRow-RowTypes[0].Length-1;
			}
			if(tableRow<RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length){
				return RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length-tableRow-1;//-1?
			}
			if(tableRow==RowTypes[0].Length+1+RowTypes[1].Length+RowTypes[2].Length){
				return -1;//mand teeth
			}
			return tableRow-RowTypes[0].Length-1-RowTypes[1].Length-RowTypes[2].Length-1;//-1?
		}

		///<summary>Gets the current cell as a col,row based on the x-y pixel coordinate supplied.</summary>
		private Point GetCellFromPixel(int x,int y){
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
			return new Point(col,row);
		}

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e){
			if(!perioEdit) {
				return;
			}
			base.OnMouseDown(e);
			Point newCell=GetCellFromPixel(e.X,e.Y);
			if(newCell.X==0){
				return;//Left column only for dates
			}
			int section=GetSection(newCell.Y);
			if(section==-1){//clicked on a toothNum
				int intTooth=(int)Math.Ceiling((double)newCell.X/3);
				if(GetTableRow(false)==newCell.Y){//if clicked on mand
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
			int sectRow=GetSectionRow(newCell.Y);
			if(sectRow<0 || sectRow>=RowTypes[section].Length) {
				//User clicked on a tooth row, not a section row.
				return;
			}
			if(RowTypes[section][sectRow]==PerioSequenceType.Probing){
				if(this.selectedExam-ProbingOffset//correct for offset
					+RowTypes[section].Length-RowsProbing//plus non-probing rows
					!=sectRow)
				{
					return;//not allowed to click on probing rows other than selectedRow
				}
			}
			else if(RowTypes[section][sectRow]==PerioSequenceType.Mobility){
				if(Math.IEEERemainder(((double)newCell.X+1),3) != 0){//{2,5,8,11};examples of acceptable cols
					return;//for mobility, not allowed to click on anything but B
				}
			}
			else if(RowTypes[section][sectRow]==PerioSequenceType.CAL){
				return;//never allowed to edit CAL
			}
			if(section==0)
				OnDirectionChangedLeft();
			else if(section==1)
				OnDirectionChangedRight();
			else if(section==2)
				OnDirectionChangedRight();
			else if(section==3)
				OnDirectionChangedLeft();
			SetNewCell(newCell.X,newCell.Y);
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
			if(!perioEdit) {
				return;
			}
			if (selectedExam == -1)
			{
				MessageBox.Show(Lan.g(this, "Please add or select an exam first in the list to the left."));
				return;
			}
			PerioCell curPerioCell=GetPerioCell(CurCell,false);
			//MessageBox.Show("key down");
			//e.Handled=true;
			//base.OnKeyDown (e);
			if(e.KeyValue>=96 && e.KeyValue<=105){//keypad 0 through 9
				if(e.Control){
					if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.GingMargin) {
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
					if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.GingMargin) {//gm
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
			else if(e.KeyCode==Keys.Back){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
						AdvanceCell(true);
						ClearValue();
					}
				}
				else{
					AdvanceCell(true);
					ClearValue();
				}
			}
			else if(e.KeyCode==Keys.Delete){
				ClearValue();
			}
			else if(e.KeyCode==Keys.Left){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
						if(Direction==AutoAdvanceDirection.Right) {
							AdvanceCell();
						}
						else if(Direction==AutoAdvanceDirection.Custom) {
							AdvanceCell(ListTools.In(GetSection(CurCell.Y),0,2));
						}
						else {
							AdvanceCell(true);
						}
					}
				}
				else{
					if(Direction==AutoAdvanceDirection.Right) {
						AdvanceCell();
					}
					else if(Direction==AutoAdvanceDirection.Custom) {
						AdvanceCell(ListTools.In(GetSection(CurCell.Y),0,2));
					}
					else {
						AdvanceCell(true);
					}
				}
			}
			else if(e.KeyCode==Keys.Right){
				if(ThreeAtATime){
					for(int i=0;i<3;i++){
						if(Direction==AutoAdvanceDirection.Right) {
							AdvanceCell(true);
						}
						else if(Direction==AutoAdvanceDirection.Custom) {
							AdvanceCell(ListTools.In(GetSection(CurCell.Y),1,3));
						}
						else {
							AdvanceCell();
						}
					}
				}
				else{
					if(Direction==AutoAdvanceDirection.Right) {
						AdvanceCell(true);
					}
					else if(Direction==AutoAdvanceDirection.Custom) {
						AdvanceCell(ListTools.In(GetSection(CurCell.Y),1,3));
					}
					else {
						AdvanceCell();
					}
				}
			}
			//else{
			//	return;
			//}
		}

		public void KeyPressed(KeyEventArgs e) {
			OnKeyDown(e);
		}
 
		///<summary>Accepts button clicks from window rather than the usual keyboard entry.  All validation MUST be done before the value is sent here.  Only valid values are b,s,p,or c. Numbers entered using overload.</summary>
		public void ButtonPressed(string keyValue){
			if(!perioEdit) {
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
			if(CurCell.X==-1) {
				return;
			}
			if(!perioEdit) {
				return;
			}
			if(GingMargPlus && RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.GingMargin) {
				//Possible values for keyValue are 0-19,101-109
				if(keyValue<100) {
					keyValue=keyValue%10;//in case the +10 was down when the number was pressed on the onscreen keypad.
					if(keyValue!=0) {//add 100 to represent negative values unless they pressed 0
						keyValue+=100;
					}
				}
				//Possible values for keyValue at this point are 0, 101-109.
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

		///<summary>Only valid values are b,s,p, and c.</summary>
		private void EnterValue(string keyValue){
			if(CurCell.X==-1) {
				return;
			}
			if(keyValue !="b" && keyValue !="s" && keyValue !="p" && keyValue !="c"){
				MessageBox.Show("Only b,s,p, and c are allowed");//just for debugging
				return;
			}
			PerioCell perioCell=GetPerioCell(CurCell,false);
			bool curCellHasText=false;
			if(ThreeAtATime){
				//don't backup
			}
			else if(perioCell.Text!=null && perioCell.Text!=""){
				curCellHasText=true;
				//so enter value for current cell
			}
			else{
				curCellHasText=false;
				AdvanceCell(true);//so backup
				perioCell=DataArray[CurCell.X,CurCell.Y];
				if(perioCell.Text==null || perioCell.Text=="") {//the previous cell is also empty
					curCellHasText=true;//treat it like a cell with text
					AdvanceCell();//go forward again
					perioCell=DataArray[CurCell.X,CurCell.Y];
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
			DataArray[CurCell.X,CurCell.Y]=perioCell;
			Invalidate(Rectangle.Ceiling(GetBounds(CurCell.X,CurCell.Y)));
			SetHasChanged(CurCell.X,CurCell.Y);
			if(ThreeAtATime){
				AdvanceCell();
			}
			else if(!curCellHasText){
				AdvanceCell();//to return to original location
			}
		}

		///<summary>Only valid values are numbers 0-19, and 101-109. Validation should be done before sending here.</summary>
		private void EnterValue(int keyValue){
			if(CurCell.X==-1) {
				return;
			}
			if((keyValue < 0 || keyValue > 19) 
				&& RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]!=PerioSequenceType.GingMargin){//large values are allowed for GingMargin to represent hyperplasia (e.g. 101 to 109 represent -1 to -9)
				MessageBox.Show("Only values 0 through 19 allowed");//just for debugging
				return;
			}
			PerioCell cur=GetPerioCell(CurCell,false);
			//might be able to eliminate these two lines
			cur.Text=keyValue.ToString();
			DataArray[CurCell.X,CurCell.Y]=cur;
			Invalidate(Rectangle.Ceiling(GetBounds(CurCell.X,CurCell.Y)));
			SetHasChanged(CurCell.X,CurCell.Y);
			if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.Probing){ 
				CalculateCAL(CurCell,true);
			}
			else if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.GingMargin){
				CalculateCAL(new Point(CurCell.X,GetTableRow
					(selectedExam,GetSection(CurCell.Y),PerioSequenceType.Probing)),true);
			}
			AdvanceCell();
		}

		///<summary>Sets a mark for the the CurCell for the type passed in. Make sure to set the CurCell to the point you want to mark before you call this method. </summary>
		public void SetBleedingFlagForPoint(Point point,BleedingFlags type) {
			if(point==null || point.X==-1) {
				return;
			}
			PerioCell perioCellCur=GetPerioCell(point,false);
			perioCellCur.Bleeding^=type;
			DataArray[point.X,point.Y]=perioCellCur;
			Invalidate(Rectangle.Ceiling(GetBounds(point.X,point.Y)));
			SetHasChanged(point.X,point.Y);
		}

		private void CalculateCAL(Point probingCell,bool alsoInvalidate){
			Point calLoc=new Point(probingCell.X,GetTableRow
				(selectedExam,GetSection(probingCell.Y),PerioSequenceType.CAL));
			Point gingLoc=new Point(probingCell.X,GetTableRow
				(selectedExam,GetSection(probingCell.Y),PerioSequenceType.GingMargin));
			//PerioCell calCell=DataArray[calLoc.X,calLoc.Y];
			if(DataArray[probingCell.X,probingCell.Y].Text==null 
				|| DataArray[probingCell.X,probingCell.Y].Text==""
				|| DataArray[gingLoc.X,gingLoc.Y].Text==null 
				|| DataArray[gingLoc.X,gingLoc.Y].Text=="")
			{
				DataArray[calLoc.X,calLoc.Y].Text="";
				if(alsoInvalidate){
					Invalidate(Rectangle.Ceiling(GetBounds(calLoc.X,calLoc.Y)));
				}
				return;
			}
			int probValue=PIn.Int(DataArray[probingCell.X,probingCell.Y].Text);
			int gingValue=PIn.Int(DataArray[gingLoc.X,gingLoc.Y].Text);
			if(gingValue>100) {
				gingValue=100-gingValue;
			}
			DataArray[calLoc.X,calLoc.Y].Text=(gingValue+probValue).ToString();
			if(alsoInvalidate){
				Invalidate(Rectangle.Ceiling(GetBounds(calLoc.X,calLoc.Y)));
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
			if(sectionRow==-1 || sectionRow>=RowTypes[section].Count() || intTooth>32 || intTooth<1) {
				return;
			}
			PerioSequenceType seqI=RowTypes[section][sectionRow];
			if((int)seqI>PerioMeasures.List.GetLength(1)) {
				return;
			}
			listChangedMeasurements.Add(new PerioMeasureItem(seqI,intTooth));
		}

		///<summary>Changes the cell that the cursor is in.</summary>
		public void SetNewCell(int x,int y){
			//MessageBox.Show(x.ToString()+","+y.ToString());
			RectangleF oldRect=new Rectangle(0,0,0,0);
			bool invalidateOld=false;
			if(CurCell.X!=-1){
				oldRect=GetBounds(CurCell.X,CurCell.Y);
				invalidateOld=true;
			}
			CurCell=new Point(x,y);
			if(invalidateOld){
				Invalidate(Rectangle.Ceiling(oldRect));
			}
			Invalidate(Rectangle.Ceiling(GetBounds(CurCell.X,CurCell.Y)));
		}

		///<summary>Most of the time use GetSection() to calculate the section.  A section corresponds to the maxillary/mandible facial/lingual 
		///(4 sections total, see GetSection() summary)</summary>
		private int GetToothNumCur(int section) {
			return GetToothNumFromPoint(section,CurCell);
		}

		private int GetToothNumFromPoint(int section,Point cellCur) {
			int intTooth=(int)Math.Ceiling((double)cellCur.X/3);
			if(section==2||section==3) {//if on mand
				intTooth=33-intTooth;//wrap
			}
			return intTooth;
		}

		public PerioCell GetCurrentCell() {
			if(CurCell.X < 0 || CurCell.Y < 0) {
				return new PerioCell();
			}
			return GetPerioCell(CurCell,true);
		}
		
		public PerioCell GetPerioCellFromPoint(Point pointCur) {
			return GetPerioCell(pointCur,true);
		}

		///<summary>Returns PerioCell for the point passed in. Sets PerioCell in DataArray based on the point passed in. Option to not set PerioCell Text.</summary>
		private PerioCell GetPerioCell(Point curCell,bool doSetText) {
			PerioCell retVal=DataArray[curCell.X,curCell.Y];
			if(curCell.Y>=12 && curCell.Y<=30) {
				retVal.Surface=PerioSurface.Lingual;
			}
			else {
				retVal.Surface=PerioSurface.Facial;
			}
			if(curCell.Y<=20) {
				retVal.ToothNum=(curCell.X+2)/3;
			}
			else {
				retVal.ToothNum=33-(curCell.X+2)/3;
			}
			if(retVal.Surface==PerioSurface.Facial) {
				retVal.ProbingPosition=(curCell.X-1)%3+1;
			}
			else {
				retVal.ProbingPosition=3-(curCell.X-1)%3;
			}
			if(doSetText) {
				retVal.Text=GetCellText(curCell.X,curCell.Y);
			}
			return retVal;
		}

		private void AdvanceCell(bool isReverse){
			if(!TryFindNextCell(CurCell,isReverse,out Point pointToSet)) {
				return;
			}
			SetNewCell(pointToSet.X,pointToSet.Y);
		}

		///<summary>Finds the next cell based on where the cursor currently is. Returns false if there is no next cell.</summary>
		public bool TryFindNextCell(Point curCell,bool isReverse,out Point nextCell,bool doSetDirection=true) {
			nextCell=curCell;
			PerioSequenceType seqType=RowTypes[GetSection(nextCell.Y)][GetSectionRow(nextCell.Y)];
			bool startedOnSkipped=false;//special situation:
			int section=GetSection(nextCell.Y);//used to test skipped teeth
			int newSection=section;//in case it doesn't change
			int intTooth=GetToothNumCur(section);//used to test skipped teeth
			if(skippedTeeth.Contains(intTooth)){
				startedOnSkipped=true;
			}
			int limit=0;
			int count=0;//count the number of positions skipped.
			while(limit<400){//the 400 limit is just a last resort. Should always break manually.
				limit++;
				//to the right
				section=GetSection(nextCell.Y);
				if(Direction==AutoAdvanceDirection.Custom) {//custom path
					return TryAutoAdvanceCustom(curCell,startedOnSkipped,isReverse,out nextCell);
				}
				if((Direction==AutoAdvanceDirection.Left && isReverse) || (Direction==AutoAdvanceDirection.Right && !isReverse)) {//right
					if(!TryAutoAdvanceRight(nextCell,section,doSetDirection,seqType,startedOnSkipped,ref count,out nextCell)) {
						return false;
					}
				}
				//to the left
				else{//((DirectionIsRight && isBackspace) || !DirectionIsRight){
					if(!TryAutoAdvanceLeft(nextCell,section,doSetDirection,seqType,out nextCell)) {
						return false;
					}
				}
				if(startedOnSkipped) {//since we started on a skipped tooth
					return true;//we can continue entry on a skipped tooth.
				}
				intTooth=GetToothNumFromPoint(GetSection(nextCell.Y),nextCell);
				bool locIsValid=true;//used when testing for skipped tooth and mobility location
				if(skippedTeeth.Contains(intTooth)) {//if we are on a skipped tooth
					locIsValid=false;
				}
				if(RowTypes[GetSection(nextCell.Y)][GetSectionRow(nextCell.Y)]==PerioSequenceType.Mobility) {
					if(Math.IEEERemainder(((double)nextCell.X+1),3) != 0) {//{2,5,8,11};examples of acceptable cols
						locIsValid=false;//for mobility, not allowed to click on anything but B
					}
				}
				if(locIsValid) {
					return true;
				}
				//otherwise, continue to loop in search of a valid loc
			}//while
			return false;
		}

		///<summary>Helper method that moves the current point to the right(when viewing the mouth and not the perio chart). When viewing the perio
		///chart, this will move the cell to the left.</summary>
		private bool TryAutoAdvanceRight(Point pointCellCur,int section,bool doSetDirection,PerioSequenceType seqType,bool startedOnSkipped,ref int count,
			out Point nextCell) 
		{
			nextCell=pointCellCur;
			int newSection=0;
			int newRow=0;
			int intTooth=GetToothNumCur(section);
			if(nextCell.X==1 && !startedOnSkipped) {//if first column
				if(section==0 && skippedTeeth.Contains(1) && intTooth==1) {
					nextCell=new Point(count+1,nextCell.Y);
					return true;//tooth 1 is missing. Jump back.
				}
				else if(section==0) {//usually in reverse
					return false;//no jump.  This is the starting point.
				}
				else if(section==1) {
					newSection=3;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(doSetDirection) {
						OnDirectionChangedLeft();
					}
				}
				else if(section==2 && skippedTeeth.Contains(32)) {
					if(nextCell.X==1) {
						nextCell=new Point(count+1,nextCell.Y);
						return true;//tooth 32 is missing. Jump back.
					}
					else {
						return false;//probably should never happen.
					}
				}
				else if(section==2) {
					return false;//no jump.  End of all sequences.
				}
				else if(section==3) {//usually in reverse
					newSection=1;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(newRow!=-1 && doSetDirection) {
						OnDirectionChangedRight();
					}
				}
				if(newRow==-1) {//MGJ and mobility aren't present in all 4 sections, so can't loop normally
					if(RowTypes[section][GetSectionRow(nextCell.Y)]==PerioSequenceType.Mobility) {
						if(section==3) {//usually in reverse
							newSection=0;
							nextCell=new Point(1+16*3,GetTableRow(selectedExam,newSection,PerioSequenceType.Mobility));
						}
					}
					else if(RowTypes[section][GetSectionRow(nextCell.Y)]==PerioSequenceType.MGJ) {
						//section 3. in reverse
						newSection=0;
						nextCell=new Point(16*3,GetTableRow(selectedExam,newSection,PerioSequenceType.MGJ));
					}
				}
				else {
					nextCell=new Point(nextCell.X,newRow);
				}
			}
			//Check to see if tooth #16 is missing.
			else if(section==1 && nextCell.X==1 && startedOnSkipped) {
				//Tooth 16 is missing so we need to jump down to section 3 (tooth 32) and start going the other direction.
				newSection=3;
				nextCell=new Point(1,GetTableRow(selectedExam,newSection,seqType));
				if(doSetDirection) {
					OnDirectionChangedLeft();
				}
			}
			else if(section==0 && skippedTeeth.Contains(1) //skipped first tooth on first row
				|| section==2 && skippedTeeth.Contains(32) //skipped first tooth on bottom row
				|| section==3 && skippedTeeth.Count==32) //skipped all teeth and are on last row
			{
				count++;//used jump back to last non missing tooth
				if(section==2 && nextCell.X==1) {
					return false;//manually clicking on missing tooth 32 and entering data
				}
				else if(section==0 && nextCell.X==1) {
					return false;//manually clicking on missing tooth 1 and entering data
				}
				else if(section==3 && nextCell.X==1) {
					return false;//stop on first tooth
				}
				bool sectionMissing = true;
				for(int tooth = 17;tooth<=32;++tooth) {
					if(!skippedTeeth.Contains(tooth)) {
						sectionMissing = false;
					}
				}
				if(section == 3 && sectionMissing) {
					return false;
				}
				else {
					nextCell=new Point(nextCell.X-1,nextCell.Y);//standard advance
				}
			}
			else {//standard advance with no jumping
				nextCell=new Point(nextCell.X-1,nextCell.Y);
			}
			return true;
		}

		///<summary>Helper method that moves the current point to the left(when viewing the mouth and not the perio chart). When viewing the perio
		///chart, this will move the cell to the right.</summary>
		private bool TryAutoAdvanceLeft(Point pointCur,int section,bool doSetDirection,PerioSequenceType seqType,out Point nextCell) {
			nextCell=pointCur;
			int newRow=0;
			int newSection=section;
			if(nextCell.X==DataArray.GetLength(0)-1) {//if last column
				if(section==0) {
					newSection=1;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(newRow!=-1 && doSetDirection) {
						OnDirectionChangedRight();
					}
				}
				else if(section==1) {//usually in reverse
					newSection=0;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(doSetDirection) {
						OnDirectionChangedLeft();
					}
				}
				else if(section==2) {//usually in reverse
					newSection=3;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(doSetDirection) {
						OnDirectionChangedLeft();
					}
				}
				else if(section==3) {
					newSection=2;
					newRow=GetTableRow(selectedExam,newSection,seqType);
					if(newRow!=-1 && doSetDirection)
						OnDirectionChangedRight();
				}
				if(newRow==-1) {//MGJ and mobility aren't present in all 4 sections, so can't loop normally
					if(RowTypes[section][GetSectionRow(nextCell.Y)]==PerioSequenceType.Mobility) {
						if(section==0) {
							newSection=3;
							nextCell=new Point(1,GetTableRow(selectedExam,newSection,PerioSequenceType.Mobility));
						}
						if(section==3) {
							return false;//end of sequence
						}
					}
					else if(RowTypes[section][GetSectionRow(nextCell.Y)]==PerioSequenceType.MGJ) {
						//section 0
						newSection=3;
						nextCell=new Point(1,GetTableRow(selectedExam,newSection,PerioSequenceType.MGJ));
					}
				}
				else {
					nextCell=new Point(nextCell.X,newRow);
				}
				bool sectionMissing = true;
				for(int tooth = 17;tooth<=32;++tooth) {
					if(!skippedTeeth.Contains(tooth)) {
						sectionMissing = false;
					}
				}
				if(section == 3 && sectionMissing) {
					return false;
				}
			}
			else {//standard advance with no jumping
				nextCell=new Point(nextCell.X+1,nextCell.Y);
			}
			return true;
		}

		///<summary>Moves the current point to the next position.</summary>
		private bool TryAutoAdvanceCustom(Point pointCur,bool startedOnSkipped,bool isReverse,out Point pointNext) {
			PerioCell perioCellCur=GetPerioCellFromPoint(pointCur);
			pointNext=pointCur;
			PerioSequenceType seqType=GetSequenceForPoint(pointNext);
			int section=GetSection(pointNext.Y);//used to test skipped teeth
			List<AutoAdvanceCustom> listCustomPaths=AutoAdvanceCustom.GetCustomPaths();
			AutoAdvanceCustom customPathCur=listCustomPaths.FirstOrDefault(x => x.ToothNum==perioCellCur.ToothNum && x.Surface==perioCellCur.Surface);
			if(customPathCur==null) {
				//We some how could not find the next point. Return the initial point.
				return true;
			}
			//listCustomPaths should be in the order the preference specified(sequential order) so the next and previous custom paths will be the actual
			//next and previous paths.
			AutoAdvanceCustom customPathNext=GetAutoAdvanceCustomFromList(customPathCur,doGetNext:true,seqType,listCustomPaths);
			//Use the logical exclusive OR operator to figure out if we need the next point to be left or right.
			bool doAdvanceRight=isReverse ^ IsCustomAutoAdvanceRight(customPathCur,customPathNext,seqType);
			int countNotUsed=0;
			bool startedOnCellOne=pointNext.X==1;
			//Advance pointNext to the next cell position.
			if(doAdvanceRight) {
				if(!TryAutoAdvanceLeft(pointNext,section,false,seqType,out pointNext) && !startedOnCellOne) {
					return false;
				}
			}
			else {
				if(!TryAutoAdvanceRight(pointNext,section,false,seqType,startedOnSkipped,ref countNotUsed,out pointNext) && !startedOnCellOne) {
					return false;
				}
			}
			PerioCell perioCellNext=GetPerioCellFromPoint(pointNext);
			//TryAutoAdvanceLeft and TryAutoAdvanceRight are helper methods that get the next point position using the default auto advance path
			//1-16F -> 16-1L -> 32-17F -> 17-32L. The methods will not advance to the right(left when viewing the perio chart) when on the first position i.e 32L. 
			//We need to go to the next tooth if the previous path is not null and isReverse.
			//or if the sequence is mobility(only has the middle(facial) surface available for entry).
			AutoAdvanceCustom customPathPrev=GetAutoAdvanceCustomFromList(customPathCur,doGetNext:false,seqType,listCustomPaths);
			bool skipSameToothCheck=(startedOnCellOne && pointNext.X==1 && customPathCur.Surface==PerioSurface.Lingual && customPathPrev!=null)
				|| seqType==PerioSequenceType.Mobility;
			if(!skipSameToothCheck && perioCellCur.ToothNum==perioCellNext.ToothNum && perioCellCur.Surface==perioCellNext.Surface) {
				//Same tooth,different position. Return true. pointNext was already set in TryAutoAdvanceLeft and TryAutoAdvanceRight
				return true;
			}
			//The point returned in TryAutoAdvanceLeft or TryAutoAdvanceRight is on a differnt tooth, get the next tooth from the listCustomPath.
			if(isReverse) {
				customPathNext=customPathPrev;//The next tooth should be the previous one. 
			}
			if(customPathNext==null) {
				//The end of the list. Set the return point equal to the initial point. No need to move.
				pointNext=pointCur;
				return true;
			}
			//return the point for the next tooth in path.
			pointNext=GetAutoAdvanceCustomPoint(customPathNext.ToothNum,customPathNext.Surface,seqType,isReverse);
			return true;
		}

		///<summary>Returns either the next or previous AutoAdvanceCustom from the list. Considers skipped teeth. Recursive method.</summary>
		public AutoAdvanceCustom GetAutoAdvanceCustomFromList(AutoAdvanceCustom pathCur,bool doGetNext,PerioSequenceType seqType,
			List<AutoAdvanceCustom> listPaths) 
		{
			AutoAdvanceCustom pathPerioCustom=doGetNext ? GetNext(pathCur,listPaths) : GetPrevious(pathCur,listPaths);
			if(pathPerioCustom==null) {
				return null;//Most likely the last one
			}
			if((seqType==PerioSequenceType.MGJ && pathPerioCustom.ToothNum.Between(1,16) || seqType==PerioSequenceType.Mobility) 
				&& pathPerioCustom.Surface==PerioSurface.Lingual) 
			{
				//The perio chart does not have MGJ for Lingual teeth 1-16. Find the next available AutoAdvanceCustom from list.
				AutoAdvanceCustom perioPathMGJ=pathPerioCustom;
				return GetAutoAdvanceCustomFromList(perioPathMGJ,doGetNext,seqType,listPaths);
			}
			//Check to make sure the perioPath is not marked as skipped.
			if(pathPerioCustom!=null && ListTools.In(pathPerioCustom.ToothNum,skippedTeeth)) {
				return GetAutoAdvanceCustomFromList(pathPerioCustom,doGetNext,seqType,listPaths);
			}
			return pathPerioCustom;
		}

		public static AutoAdvanceCustom GetNext(AutoAdvanceCustom current,List<AutoAdvanceCustom> listPaths) {
			try {
				return listPaths.SkipWhile(x => !x.Equals(current)).Skip(1).First();
			}
			catch {
				return null;
			}
		}

		public static AutoAdvanceCustom GetPrevious(AutoAdvanceCustom current,List<AutoAdvanceCustom> listPaths) {
			try {
				return listPaths.TakeWhile(x => !x.Equals(current)).Last();
			}
			catch {
				return null;
			}
		}

		///<summary>Returns true if the custom auto advance should go to the right direction.</summary>
		public bool IsCustomAutoAdvanceRight(AutoAdvanceCustom pathCur,AutoAdvanceCustom pathNext,PerioSequenceType seqType) {
			//This method will need to change if we add other custom paths.
			if(pathCur==null) {
				return false;
			}
			int toothNumCur=pathCur.ToothNum;
			int toothNumNext=pathNext?.ToothNum??pathCur.ToothNum;
			PerioSurface surfCur=pathCur.Surface;
			PerioSurface surfNext=pathNext?.Surface??surfCur;
			int sectionCur=GetSection(toothNumCur,surfCur);
			int sectionNext=GetSection(toothNumNext,surfNext);
			if(toothNumNext.Between(1,16)) {
				//If the next tooth number is between 1 and 16, the direction will go right in 3 cases: 
				//1. If the next tooth surface is facial
				//2. toothNumCur is 32-17L(going from 17L to 16L).
				//3. the next tooth is 1-16 but in different section.
				if(surfNext==PerioSurface.Facial || toothNumCur>=17 || sectionCur!=sectionNext) {
					return true;
				}
			}
			else {//toothNumNext between 17-32
				//If the next tooth number is between 17 and 32, the direction will go right in 2 cases: 
				//1. The next tooth surface is Lingual (except when going from 32F to 32L)
				//2. toothNumCur is <=16(going from 1-16F to 17-32F)
				if((surfNext==PerioSurface.Lingual && surfCur==surfNext) || toothNumCur<=16) {
					return true;//32-17L or going from 16F to 17F
				}
			}
			return false;
		}

		public bool IsCellTextEmpty(Point point) {
			return string.IsNullOrEmpty(GetCellText(point.X,point.Y));
		}

		private void AdvanceCell(){
			AdvanceCell(false);
		}

		private void ClearValue(){
			//MessageBox.Show(DataArray.GetLength(0).ToString());
			//MessageBox.Show(DataArray.GetLength(1).ToString());
			PerioCell cur=DataArray[CurCell.X,CurCell.Y];
			cur.Text="";
			DataArray[CurCell.X,CurCell.Y]=cur;
			SetHasChanged(CurCell.X,CurCell.Y);
			Invalidate(Rectangle.Ceiling(GetBounds(CurCell.X,CurCell.Y)));
			if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.Probing){ 
				CalculateCAL(CurCell,true);
			}
			else if(RowTypes[GetSection(CurCell.Y)][GetSectionRow(CurCell.Y)]==PerioSequenceType.GingMargin){
				CalculateCAL(new Point(CurCell.X,GetTableRow
					(selectedExam,GetSection(CurCell.Y),PerioSequenceType.Probing)),true);
			}
		}

		///<summary></summary>
		public void ToggleSkip(long perioExamNum) {
			if(selectedTeeth.Count==0){
				MessageBox.Show(Lan.g(this,"Please select teeth first."));
				return;
			}
			for(int i=0;i<selectedTeeth.Count;i++){
				if(skippedTeeth.Contains((int)selectedTeeth[i])){//if the tooth was already marked skipped
					skippedTeeth.Remove((int)selectedTeeth[i]);
				}
				else{//tooth was not already marked skipped
					skippedTeeth.Add((int)selectedTeeth[i]);
				}
			}
			PerioMeasures.SetSkipped(perioExamNum,skippedTeeth);
			selectedTeeth=new ArrayList();
			Invalidate();
		}

		///<summary></summary>
		protected void OnDirectionChangedRight(){
			if(DirectionChangedRight != null && Direction!=AutoAdvanceDirection.Custom){
				Direction=AutoAdvanceDirection.Right;
				EventArgs eArgs=new EventArgs();
				DirectionChangedRight(this,eArgs);
			}
		}

		///<summary></summary>
		protected void OnDirectionChangedLeft(){
			if(DirectionChangedLeft!=null && Direction!=AutoAdvanceDirection.Custom) {
				Direction=AutoAdvanceDirection.Left;
				EventArgs eArgs=new EventArgs();
				DirectionChangedLeft(this,eArgs);
			}
		}

		///<summary></summary>
		public string ComputeOrionPlaqueIndex() {
			if(this.selectedExam==-1) {
				return "";
			}
			int counter=0;
			List<PerioMeasure> pm=PerioMeasures.GetAllForExam(PerioExams.ListExams[selectedExam].PerioExamNum);
			for(int i=0;i<pm.Count;i++) {
				if(pm[i].SequenceType==PerioSequenceType.Bleeding) {
					//If tooth has plaque on any of the six points
					if(((BleedingFlags)pm[i].MBvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)pm[i].Bvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)pm[i].DBvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)pm[i].MLvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)pm[i].Lvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
					if(((BleedingFlags)pm[i].DLvalue & BleedingFlags.Plaque)==BleedingFlags.Plaque) {
						counter++;
						continue;
					}
				}
			}
			if(counter==0 || skippedTeeth.Count==32) {
				return (0).ToString("F0");
			}
			return (100*counter/((32-skippedTeeth.Count))).ToString("F0");
		}

		///<summary></summary>
		public string ComputeIndex(BleedingFlags bleedFlag){
			if(this.selectedExam==-1){
				return "";
			}
			int counter=0;
			for(int section=0;section<4;section++){
				for(int x=1;x<1+3*16;x++){
					if((DataArray[x,GetTableRow(selectedExam,section,PerioSequenceType.Probing)].Bleeding 
						& bleedFlag)>0)
					{
						counter++;
					}
				}
			}
			if(counter==0) {
				return (0).ToString("F0");
			}
			int unskippedTeeth=(32-skippedTeeth.Count);
			if(unskippedTeeth==0) {
				return "";
			}
			return (100*counter/(unskippedTeeth*6)).ToString("F0");
		}

		///<summary>Returns a list of strings, each between "1" and "32" (or similar international #'s), representing the teeth with red values based on prefs.  The result can be used to print summary, or can be counted to show # of teeth.</summary>
		public ArrayList CountTeeth(PerioSequenceType seqType){
			if(selectedExam==-1){
				return new ArrayList();
			}
			int prefVal=0;
			switch(seqType){
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
			ArrayList retList=new ArrayList();
			string cellText="";
			int intTooth=0;
			int row=0;
			for(int section=0;section<4;section++){
				for(int x=1;x<1+3*16;x++){
					row=GetTableRow(selectedExam,section,seqType);
					if(row==-1)
						continue;//eg MGJ or Mobility
					cellText=DataArray[x,row].Text;
					if(cellText==null || cellText==""){
						continue;
					}
					if((seqType==PerioSequenceType.MGJ && PIn.Long(cellText)<=prefVal)
						|| (seqType!=PerioSequenceType.MGJ && PIn.Long(cellText)>=prefVal)){
						intTooth=(int)Math.Ceiling((double)x/3);
						if(section==2 || section==3){//if mand
							intTooth=33-intTooth;
						}
						if(!retList.Contains(Tooth.ToInternat(intTooth.ToString()))){
							retList.Add(Tooth.ToInternat(intTooth.ToString()));
						}
					}
				}
			}
			return retList;
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

	

	

	///<summary></summary>
	public struct PerioCell{
		///<summary>The value to display for this exam. Overwrites any oldText from previous exams.</summary>
		public string Text;
		///<summary>None, blood, pus, or both</summary>
		public BleedingFlags Bleeding;
		///<summary>Values from previous exams. Slightly greyed out.</summary>
		public string OldText;
		public int ToothNum;
		public int ProbingPosition;
		public PerioSurface Surface;
	}

	public class AutoAdvanceCustom {
		public int ToothNum;
		public PerioSurface Surface;

		public AutoAdvanceCustom(int toothNum,PerioSurface surf) {
			ToothNum=toothNum;
			Surface=surf;
		}

		///<summary>Returns the default path for perio auto advance. 1-16F, 16-1L,32-17F,17-32L.</summary>
		public static List<AutoAdvanceCustom> GetDefaultPath() {
			List<AutoAdvanceCustom> listPaths=new List<AutoAdvanceCustom>();
			//1-16F
			listPaths.AddRange(Enumerable.Range(1,16).Select(x => new AutoAdvanceCustom(x,PerioSurface.Facial)));
			//16-1L
			listPaths.AddRange(Enumerable.Range(1,16).OrderByDescending(x => x).Select(x => new AutoAdvanceCustom(x,PerioSurface.Lingual)));
			//32-17F
			listPaths.AddRange(Enumerable.Range(17,16).OrderByDescending(x => x).Select(x => new AutoAdvanceCustom(x,PerioSurface.Facial)));
			//17-32L
			listPaths.AddRange(Enumerable.Range(17,16).Select(x => new AutoAdvanceCustom(x,PerioSurface.Lingual)));
			return listPaths;
		}

		///<summary>Auto advance custom path 1-16F,17-32F,32-17L,16-1L.</summary>
		public static List<AutoAdvanceCustom> GetCustomPaths() {
			List<AutoAdvanceCustom> listPaths=new List<AutoAdvanceCustom>();
			//1-16F
			listPaths.AddRange(Enumerable.Range(1,16).Select(x => new AutoAdvanceCustom(x,PerioSurface.Facial)));
			//17-32F
			listPaths.AddRange(Enumerable.Range(17,16).Select(x => new AutoAdvanceCustom(x,PerioSurface.Facial)));
			//32-17L
			listPaths.AddRange(Enumerable.Range(17,16).OrderByDescending(x => x).Select(x => new AutoAdvanceCustom(x,PerioSurface.Lingual)));
			//16-1L
			listPaths.AddRange(Enumerable.Range(1,16).OrderByDescending(x => x).Select(x => new AutoAdvanceCustom(x,PerioSurface.Lingual)));
			return listPaths;
		}
	}

	///<summary></summary>
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

	public enum AutoAdvanceDirection {
		///<summary>0</summary>
		Left,
		///<summary>1</summary>
		Right,
		///<summary>2</summary>
		Custom,
	}
}
















