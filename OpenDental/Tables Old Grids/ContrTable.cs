/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Windows.Forms;

namespace OpenDental{
	///<summary></summary>
	public class ContrTable : System.Windows.Forms.UserControl{
		private System.ComponentModel.Container components = null;// Required designer variable.
		//initial arbitrary 10x10 table
		///<summary></summary>
		public int MaxRows=10;
		///<summary></summary>
		public int MaxCols=10;
		///<summary></summary>
		public string[,] Cell=new string[10,10];
		///<summary></summary>
		public float[,] FontSize = new float[10,10];
		///<summary></summary>
		public bool[,] FontBold = new bool[10,10];
		///<summary></summary>
		public Color[,] FontColor= new Color[10,10];
		//public int[,] FontAlign = new int[10,10];
		///<summary></summary>
		public Color[,] BackGColor= new Color[10,10];
		///<summary></summary>
		public Color[,] LeftBorder= new Color[10,10];
		///<summary></summary>
		public Color[,] TopBorder= new Color[10,10];
		///<summary></summary>
		public int[] RowHeight= new int[10];
		///<summary></summary>
		public int[] ColWidth=new int[10];
		private int[] colPos = new int[10];
		private int[] rowPos = new int[10];
		///<summary></summary>
		public HorizontalAlignment[] ColAlign = new HorizontalAlignment[10];
		///<summary></summary>
		public string Heading="";
		///<summary></summary>
		public bool HeadingIsPresent=true;//heading can only be present if fields are present
		///<summary></summary>
		public bool FieldsArePresent=true;
		///<summary></summary>
		public string[] Fields = new string[10];
		///<summary></summary>
		public bool ShowScroll=false;
		private int scrollWidth=0;
		private Font fontSS;
		private System.Windows.Forms.Panel panelHead;
		private System.Windows.Forms.Panel panelScroll;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private Font fontB;
		private OpenDental.ContrPanelTable panelTable;
		private Point mouseDownPosition;
		private Font myFont=new Font("Microsoft Sans Serif",12);//only initialized to prevent complile error
		private float myFontSize=0;
		private FontStyle myFontStyle=FontStyle.Regular;
		private Color myFontColor=Color.Black;
		private FontFamily myFontFamily=FontFamily.GenericSansSerif;
		private int offset=0;//distance from left of cell to begin of text
    private bool ControlIsDown;
		//public Color GridColor=Color.Gray;
		///<summary></summary>
		public bool[,] IsOverflow;
		///<summary></summary>
		public Color DefaultBackGColor=Color.White;
		///<summary></summary>
		public Color DefaultGridColor=Color.Gray;
		///<summary>Use this for tables with single selection.</summary>
		public int SelectedRow=-1;
		///<summary></summary>
		public static int SelectedTable;//for arrays of tables
		///<summary></summary>
		public int MySelectedTable;//for arrays of tables
		///<summary></summary>
		public int[] SelectedRows;
		///<summary></summary>
		public  ArrayList SelectedRowsAL;
		private SelectionMode selectionMode;
		private int[] selectedIndices;
		//private ArrayList SelectedIAL;//only used for tracking selectedIndices	
		//private int scrollValue;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		///<summary></summary>
		public ContrTable(){
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			this.panelTable.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panelTable_MouseWheel);
			fontSS = new Font("Microsoft Sans Serif", 8);
			fontB = new Font("Microsoft Sans Serif",(float)8.25,FontStyle.Bold);
			selectedIndices=new int[0];
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private void InitializeComponent()
		{
			this.panelScroll = new System.Windows.Forms.Panel();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.panelTable = new OpenDental.ContrPanelTable();
			this.panelHead = new System.Windows.Forms.Panel();
			this.panelScroll.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelScroll
			// 
			this.panelScroll.BackColor = System.Drawing.SystemColors.Control;
			this.panelScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelScroll.Controls.Add(this.vScrollBar1);
			this.panelScroll.Controls.Add(this.panelTable);
			this.panelScroll.Location = new System.Drawing.Point(0, 100);
			this.panelScroll.Name = "panelScroll";
			this.panelScroll.Size = new System.Drawing.Size(464, 160);
			this.panelScroll.TabIndex = 1;
			this.panelScroll.Paint += new System.Windows.Forms.PaintEventHandler(this.panelScroll_Paint);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1.LargeChange = 50;
			this.vScrollBar1.Location = new System.Drawing.Point(445, 0);
			this.vScrollBar1.Maximum = 200;
			this.vScrollBar1.Minimum = 1;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 158);
			this.vScrollBar1.SmallChange = 50;
			this.vScrollBar1.TabIndex = 1;
			this.vScrollBar1.Value = 150;
			this.vScrollBar1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.vScrollBar1_KeyPress);
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// panelTable
			// 
			this.panelTable.BackColor = System.Drawing.SystemColors.Window;
			this.panelTable.Location = new System.Drawing.Point(0, 0);
			this.panelTable.Name = "panelTable";
			this.panelTable.Size = new System.Drawing.Size(420, 168);
			this.panelTable.TabIndex = 2;
			this.panelTable.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.panelTable_KeyPress);
			this.panelTable.Click += new System.EventHandler(this.panelTable_Click);
			this.panelTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTable_MouseUp);
			this.panelTable.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTable_Paint);
			this.panelTable.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panelTable_KeyUp);
			this.panelTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panelTable_KeyDown);
			this.panelTable.DoubleClick += new System.EventHandler(this.panelTable_DoubleClick);
			this.panelTable.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panelTable_MouseWheel);
			this.panelTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTable_MouseDown);
			// 
			// panelHead
			// 
			this.panelHead.AutoScroll = true;
			this.panelHead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelHead.Location = new System.Drawing.Point(1, 1);
			this.panelHead.Name = "panelHead";
			this.panelHead.Size = new System.Drawing.Size(436, 84);
			this.panelHead.TabIndex = 2;
			this.panelHead.Click += new System.EventHandler(this.panelHead_Click);
			this.panelHead.Paint += new System.Windows.Forms.PaintEventHandler(this.panelHead_Paint);
			// 
			// ContrTable
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.panelScroll);
			this.Controls.Add(this.panelHead);
			this.Name = "ContrTable";
			this.Size = new System.Drawing.Size(484, 356);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ContrTable_Paint);
			this.panelScroll.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary></summary>
		[Category("Behavior"),
			Description("Exactly like the listBox.SelectionMode, except no MultiSimple.")
		]
		public SelectionMode SelectionMode{
			get{ 
				return selectionMode; 
			}
			set{ 
				selectionMode=value;
			}
		}

		///<summary>Gets or sets the position of the scrollbar.</summary>
		public int ScrollValue{
			get{ 
				return vScrollBar1.Value; 
			}
			set{ 
				if(value>vScrollBar1.Maximum) 
					value=vScrollBar1.Maximum;
				if(value<vScrollBar1.Minimum)
					value=vScrollBar1.Minimum;
				vScrollBar1.Value=value;
				LayoutManager.MoveLocation(panelTable,new Point(0,-value));
			}
		}

		///<summary>Holds the int values of the indices of the selected rows.  Use for multiple row selection tables.</summary>
    [Description("Holds the int values of the indexes of the selected rows")]
		public int[] SelectedIndices{
			get{ 
				return selectedIndices; 
			}
			set{ 
				/*selectedIndices=value;
				for(int i=0;i<SelectedIAL.Count;i++){
					ColorRow((int)SelectedIAL[i],Color.White);
				}
				SelectedIAL.Clear();
				for(int i=0;i<selectedIndices.Length;i++){
					SelectedIAL.Add(selectedIndices[i]);
					ColorRow((int)SelectedIAL[i],SystemColors.Highlight);
				}*/
			}
		}

		///<summary></summary>
		public void SetSelected(int index,bool setValue){
			ArrayList SelectedIAL=new ArrayList();
			for(int i=0;i<selectedIndices.Length;i++){
				SelectedIAL.Add(selectedIndices[i]);
			}
			if(setValue){//select specified index
				if(SelectedIAL.Contains(index)){
					;//already set
				}
				else{
					SelectedIAL.Add(index);
					ColorRow(index,Color.Silver);//SystemColors.Highlight);
				}
			}
			else{//unselect specified index
				if(SelectedIAL.Contains(index)){
					SelectedIAL.Remove(index);
					ColorRow(index,Color.White);
				}
				else{
					;//already unselected
				}
			}
      selectedIndices=new int[SelectedIAL.Count];
      for(int i=0;i<SelectedIAL.Count;i++){
        selectedIndices[i]=(int)SelectedIAL[i];
      }
			SelectedIAL=null;
		}

		///<summary></summary>
		public void SetSelected(int[] iArray,bool setValue){//allows setting multiple values all at once
			ArrayList SelectedIAL=new ArrayList();
			for(int i=0;i<selectedIndices.Length;i++){
				SelectedIAL.Add(selectedIndices[i]);
			}
			for(int i=0;i<iArray.Length;i++){
				if(setValue){//select specified index
					if(SelectedIAL.Contains(iArray[i])){
						;//already set
					}
					else{
						SelectedIAL.Add(iArray[i]);
						ColorRow(iArray[i],Color.Silver);//SystemColors.Highlight);
					}
				}
				else{//unselect specified index
					if(SelectedIAL.Contains(iArray[i])){
						SelectedIAL.Remove(iArray[i]);
						ColorRow(iArray[i],Color.White);
					}
					else{
						;//already unselected
					}
				}
			}
      selectedIndices=new int[SelectedIAL.Count];
      for(int i=0;i<SelectedIAL.Count;i++){
        selectedIndices[i]=(int)SelectedIAL[i];
      }
			SelectedIAL=null;
		}

		///<summary></summary>
		public void SetSelected(bool setValue){//sets all to specified value, and only redraws affected rows.
																					 //Alternative would be to use ResetRows if clearing.
			ArrayList SelectedIAL=new ArrayList();
			for(int i=0;i<selectedIndices.Length;i++){
				SelectedIAL.Add(selectedIndices[i]);
			}
			if(setValue){//select all{
				selectedIndices=new int[MaxRows];
				for(int i=0;i<MaxRows;i++){
					if(!SelectedIAL.Contains(i)){
						ColorRow(i,Color.Silver);//SystemColors.Highlight);
					}
					selectedIndices[i]=i;
				}
			}
			else{//unselect all
				for(int i=0;i<SelectedIAL.Count;i++){
					ColorRow((int)SelectedIAL[i],Color.White);
				}
				selectedIndices=new int[0];
			}
			SelectedIAL=null;
		}

		///<summary>Sets the selected row and colors it. Resiliant enough to handle a bad value.</summary>
		public void SetSelectedRow(int rowValue){
			if(SelectedRow!=-1)
				ColorRow(SelectedRow,Color.White);
			SelectedRow=rowValue;
			if(SelectedRow>MaxRows-1)
				SelectedRow=-1;
			if(SelectedRow!=-1)
				ColorRow(SelectedRow,Color.Silver);
		}

		///<summary></summary>
		public void InstantClassesPar(){
			Cell = new string[MaxCols,MaxRows];
			FontSize = new float[MaxCols,MaxRows];
			FontBold = new bool[MaxCols,MaxRows];
			FontColor = new Color[MaxCols,MaxRows];
			//FontAlign = new int[MaxCols,MaxRows];
			BackGColor= new Color[MaxCols,MaxRows];
			LeftBorder= new Color[MaxCols,MaxRows];
			TopBorder = new Color[MaxCols,MaxRows];
			RowHeight = new int[MaxRows];
			ColWidth = new int[MaxCols];
			ColAlign = new HorizontalAlignment[MaxCols];
			Fields = new string[MaxCols];
			IsOverflow = new bool[MaxCols,MaxRows];
			colPos = new int[MaxCols];
			rowPos = new int[MaxRows];
			selectedIndices=new int[0];
			//Lan.C(this, new System.Windows.Forms.Control[] {
			//	this.panelHead,
			//	this.panelScroll,
			//	this.panelTable,
			//});
		}

		///<summary></summary>
		public void ResetRows(int maxRows){
			MaxRows=maxRows;
			Cell = new string[MaxCols,MaxRows];
			FontSize = new float[MaxCols,MaxRows];
			FontBold = new bool[MaxCols,MaxRows];
			FontColor = new Color[MaxCols,MaxRows];
			//FontAlign = new int[MaxCols,MaxRows];
			BackGColor= new Color[MaxCols,MaxRows];
			LeftBorder= new Color[MaxCols,MaxRows];
			TopBorder = new Color[MaxCols,MaxRows];
			RowHeight = new int[MaxRows];
			//ColWidth = new int[MaxCols];
			//ColAlign = new int[MaxCols];
			//Fields = new string[MaxCols];
			IsOverflow = new bool[MaxCols,MaxRows];
			//colPos = new int[MaxCols];
			rowPos = new int[MaxRows];
			SetRowHeight(0,MaxRows-1,14);
			SetBackGColor(Color.White);
			selectedIndices=new int[0];
			if(SelectedRow >= maxRows)
				SelectedRow=-1;
      //SelectedIAL=new ArrayList();
		}

		///<summary></summary>
		public void SetBackGColor(Color myColor){
			for (int i=0; i<MaxRows; i++){
				for (int j=0; j<MaxCols; j++){
					BackGColor[j,i]=myColor;
				}
			}
		}

		///<summary>Lays out the control and refreshes. Option to preserve the scroll position.</summary>
		///<param name="preserveScroll">Set to true to preserve the scroll position, or omit to not.</param>
		public void LayoutTables(bool preserveScroll){
			int scroll=ScrollValue;//use this to preserveScroll
			if(ShowScroll)
				scrollWidth=17;
			else 
				scrollWidth=0;
			if(MaxRows!=0)
				rowPos[0]=0;
			for(int i=1;i<MaxRows;i++){
				rowPos[i]=rowPos[i-1]+RowHeight[i-1];
			}
			//MessageBox.Show(Height.ToString());
			if(!ShowScroll && MaxRows>0)//true 50
				if(FieldsArePresent && HeadingIsPresent)
					Height=rowPos[MaxRows-1]+RowHeight[MaxRows-1]+1+17+15+1;
				else if (FieldsArePresent)
					Height=rowPos[MaxRows-1]+RowHeight[MaxRows-1]+1+15+2;
				else{
					Height=rowPos[MaxRows-1]+RowHeight[MaxRows-1]+1;
				}
			colPos[0]=0;
			for(int i=1;i<MaxCols;i++){
				colPos[i]=colPos[i-1]+ColWidth[i-1];
			}
			if(ColWidth[MaxCols-1]!=0){
				LayoutManager.MoveWidth(panelHead,colPos[MaxCols-1]+ColWidth[MaxCols-1]+scrollWidth);
				Width=panelHead.Width+2;
				if (FieldsArePresent && HeadingIsPresent)
					LayoutManager.MoveHeight(panelHead,17+15+1);
				else if (FieldsArePresent)
					LayoutManager.MoveHeight(panelHead,15+2);
				else{
					panelHead.Visible=false;
					LayoutManager.MoveHeight(panelHead,1);
				}
				LayoutManager.MoveWidth(panelScroll,Width-2);
				LayoutManager.MoveWidth(panelTable,Width-scrollWidth-2);
				if (MaxRows==0) 
					LayoutManager.MoveHeight(panelTable,0);
				else 
					LayoutManager.MoveHeight(panelTable,rowPos[MaxRows-1]+RowHeight[MaxRows-1]);
				//Point tempPoint = panelScroll.Location;
				//tempPoint.Y=panelHead.Height-1;
				LayoutManager.MoveLocation(panelScroll,new Point(1,panelHead.Height-1));
				LayoutManager.MoveHeight(panelScroll,this.Height-panelHead.Height);
								//scrollbar:
				if(ShowScroll){
					//if(panelScroll.Height<0){//prevents a bug
					//	return;
					//}
					if(panelTable.Height<panelScroll.Height){
						vScrollBar1.Enabled=false;
						//even though the scroll won't actually move, we need this line so that preserve scroll will work properly
						vScrollBar1.Maximum=1;
						vScrollBar1.Value=1;
						LayoutManager.MoveLocation(panelTable,new Point(0,-1));
					}
					else{
						vScrollBar1.Enabled=true;
						vScrollBar1.Minimum=1;
						vScrollBar1.Maximum=panelTable.Height+2;
						vScrollBar1.LargeChange=panelScroll.Height;
						vScrollBar1.SmallChange=3*14;//(3 rows)
						if(panelTable.Height==0)//vScrollBar.Value cannot=0
							vScrollBar1.Value=1;
						else
							vScrollBar1.Value=panelTable.Height-panelScroll.Height+2;
						LayoutManager.MoveLocation(panelTable,new Point(0,-vScrollBar1.Value));
					}
				}
				else{//Scroll not showing
					vScrollBar1.Visible=false;
					LayoutManager.MoveLocation(panelTable,new Point(0,-1));
				}
			}//end if ColWidth not 0
			if(preserveScroll)
				ScrollValue=scroll;
			Refresh();
		}//end Layout Tables

		///<summary>Lays out the control without preserving the scroll position.</summary>
		public void LayoutTables(){
			LayoutTables(false);
		}

		///<summary></summary>
		public void SetRowHeight(int rowStart, int rowStop, int rowHeight){
		  for (int i=rowStart; i<=rowStop; i++){
				RowHeight[i]=rowHeight;
			}
		}

		///<summary></summary>
		public void ColorRow(int row, Color myColor){
			if(row>MaxRows-1)
				return;
			//float Foffset=0;
			Graphics grfx = panelTable.CreateGraphics();
			int i=row;
			//background and left border for overflow cells
			for(int j=0;j<MaxCols;j++){
				BackGColor[j,i]=myColor;
				if(IsOverflow[j,i]==true){		
					grfx.FillRectangle(new SolidBrush(BackGColor[j,i]),colPos[j]+1,rowPos[i]+1,ColWidth[j]-1,RowHeight[i]-1);
					//if(DefaultBackGColor==myColor) LeftBorder[j,i]=DefaultGridColor;
					//else LeftBorder[j,i]=myColor;
					grfx.DrawLine(new Pen(LeftBorder[j,i]),colPos[j],rowPos[i]+1,colPos[j],rowPos[i]+RowHeight[i]-1);
				}
			}
			//background for nonoverflow cells, borders, and text
			for(int j=0;j<MaxCols;j++){	
				//background and left border
				if(IsOverflow[j,i]==false){		
					grfx.FillRectangle(new SolidBrush(BackGColor[j,i]),colPos[j]+1,rowPos[i]+1,ColWidth[j]-1,RowHeight[i]-1);
					grfx.DrawLine(new Pen(LeftBorder[j,i]),colPos[j],rowPos[i]+1,colPos[j],rowPos[i]+RowHeight[i]);
				}
				//top border...(not needed)
				//text
				//offset=(int)(((float)Width-scrollWidth-grfx.MeasureString(Heading,myFont).Width)/2);
				if (FontBold[j,i]) {
					myFontStyle=FontStyle.Bold;
					myFontFamily=FontFamily.GenericSansSerif;
				}
				else{
					myFontStyle=FontStyle.Regular;
					myFontFamily=FontFamily.GenericSansSerif;
				}
				if (FontSize[j,i]!=0) myFontSize=FontSize[j,i];
				//else if (FontBold[j,i]) myFontSize=8.5f;
				else myFontSize=8.5f;
				//myFont=new Font(myFontName,myFontSize,myFontStyle);
				myFont=new Font(FontFamily.GenericSansSerif,myFontSize,myFontStyle);
				if(FontColor[j,i].IsEmpty){
					myFontColor=Color.Black;
				}
				else{
					myFontColor=FontColor[j,i];
				}
				if (ColAlign[j]==HorizontalAlignment.Center){
					offset=(int)(((float)ColWidth[j]-grfx.MeasureString(Cell[j,i],myFont).Width)/2);
				}
				else if (ColAlign[j]==HorizontalAlignment.Right){
					if(myFontStyle==FontStyle.Bold)
						offset=ColWidth[j]-(int)Math.Round(System.Convert.ToDouble(grfx.MeasureString(Cell[j,i],myFont).Width))-1;
					else
						offset=ColWidth[j]-(int)Math.Round(System.Convert.ToDouble(grfx.MeasureString(Cell[j,i],myFont).Width*.92))-1;
						//the .92 factor was the only way I could get the numbers to right-align properly
				}
				else offset = 1;
				//grfx.TextRenderingHint=(TextRenderingHint)3;
				grfx.DrawString(Cell[j,i],myFont,new SolidBrush(myFontColor),(float)colPos[j]+offset,rowPos[i]+1);
			}//end for
			if(row==MaxRows-1){//if last row
				grfx.DrawLine(new Pen(Color.Black),0,panelTable.Height-1,panelTable.Width-1,panelTable.Height-1);
			}
			grfx.Dispose();
			//Refresh();//Makes it flicker
		}

		///<summary></summary>
		public void SetTextColorRow(int row, Color myColor){
			for(int j=0;j<MaxCols;j++){
				FontColor[j,row]=myColor;
			}
		}

		///<summary></summary>
		public void SetBackColorRow(int row, Color myColor){
			for(int j=0;j<MaxCols;j++){
				BackGColor[j,row]=myColor;
			}
		}

		///<summary></summary>
		public void SetGridColor(Color myColor){
			DefaultGridColor=myColor;
			for (int i=0; i<MaxRows; i++){
				for (int j=0; j<MaxCols; j++){
					LeftBorder[j,i]=myColor;
					TopBorder[j,i]=myColor;
				}
			}
			
			//Refresh();
		}

		///<summary></summary>
		protected void panelHead_Paint(object sender, System.Windows.Forms.PaintEventArgs pea) {
			if (FieldsArePresent==false) return;
			Graphics grfx = pea.Graphics;
			Pen penB = new Pen(Color.Black);
			Pen penGr = new Pen(Color.LightGray);
			Pen penW = new Pen(Color.White);
			grfx.FillRectangle(new SolidBrush(Color.LightGray),0,0,panelHead.Width,panelHead.Height);
			int fieldsLoc;
			if (HeadingIsPresent) fieldsLoc=17;
			else fieldsLoc=0;
			if (HeadingIsPresent){
				grfx.FillRectangle(new SolidBrush(Color.White),0,0,panelHead.Width,fieldsLoc);
				grfx.DrawLine(penW,0,0,panelHead.Width,0);
				//grfx.DrawLine(penW,1,1,panelHead.Width,1);
				grfx.DrawLine(penB,0,fieldsLoc-1,panelHead.Width,fieldsLoc-1);
				grfx.DrawLine(penW,0,0,0,fieldsLoc-2);
				//grfx.DrawLine(penW,1,1,1,fieldsLoc-2);
			}
			//Fields row
			grfx.DrawLine(penW,1,fieldsLoc,panelHead.Width,fieldsLoc);
			grfx.DrawLine(penW,0,fieldsLoc+1,0,fieldsLoc+15+1);
			//grfx.DrawLine(penW,1,fieldsLoc+1,1,fieldsLoc+14+1);
			for (int j=1; j<MaxCols; j++){
				grfx.DrawLine(penB,colPos[j],fieldsLoc-1,colPos[j],fieldsLoc+14+1);
				grfx.DrawLine(penW,colPos[j]+1,fieldsLoc,colPos[j]+1,fieldsLoc+14+1);
			}
			//myFontFamily=FontFamily.GenericSansSerif;
			//grfx.DrawLine(penB,panelHead.Width-3,fieldsLoc+1,panelHead.Width-3,fieldsLoc+14+1);
			Font myFont=new Font(FontFamily.GenericSansSerif,10f,FontStyle.Bold);
			int offset=(int)(((float)panelHead.Width-scrollWidth-grfx.MeasureString(Heading,myFont).Width)/2);
			if (HeadingIsPresent)
				//grfx.TextRenderingHint=TextRenderingHint.AntiAlias;
				grfx.DrawString(Heading,myFont,new SolidBrush(Color.Black),offset,0);
				//grfx.TextRenderingHint=TextRenderingHint.SystemDefault;
			myFont=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
			//text
			for (int j=0; j<MaxCols; j++){
				offset=(int)(((float)ColWidth[j]-grfx.MeasureString(Fields[j],myFont).Width)/2);
				grfx.DrawString(Fields[j],myFont,new SolidBrush(Color.Black),colPos[j]+offset,fieldsLoc);
			}//end for j
			grfx=null;
		}//end PanelHead_Paint

		private void panelTable_Paint(object sender, System.Windows.Forms.PaintEventArgs pea) {
			Graphics grfx = pea.Graphics;
			Pen penB = new Pen(Color.Black);
			Pen penGr = new Pen(Color.LightGray);
			int minRow=WorldToLine(pea.ClipRectangle.Top)-1;
			if(minRow<0) minRow=0;
			int maxRow=WorldToLine(pea.ClipRectangle.Bottom)+1;
			if(maxRow>MaxRows) maxRow=MaxRows;
			try{
				for (int i=minRow; i<maxRow; i++){	
					//background and left border for overflow cells
					for (int j=0; j<MaxCols; j++){
						if(IsOverflow[j,i]==true){
							grfx.FillRectangle(new SolidBrush(BackGColor[j,i]),colPos[j]+1,rowPos[i]+1,ColWidth[j]-1,RowHeight[i]-1);
							grfx.DrawLine(new Pen(LeftBorder[j,i]),colPos[j],rowPos[i]+1,colPos[j],rowPos[i]+RowHeight[i]);
						}
					}//end for j
					
					//background for nonoverflow cells,borders, and text
					for (int j=0; j<MaxCols; j++){
						//background and left border
						if(IsOverflow[j,i]==false){
							grfx.FillRectangle(new SolidBrush(BackGColor[j,i]),colPos[j]+1,rowPos[i]+1,ColWidth[j]-1,RowHeight[i]-1);
							grfx.DrawLine(new Pen(LeftBorder[j,i]),colPos[j],rowPos[i]+1,colPos[j],rowPos[i]+RowHeight[i]);
						}
						//top border
						grfx.DrawLine(new Pen(TopBorder[j,i]),colPos[j]+1,rowPos[i],colPos[j]+ColWidth[j],rowPos[i]);
						//text
						if (FontBold[j,i]) {
							myFontStyle=FontStyle.Bold;
							myFontFamily=FontFamily.GenericSansSerif;
						}
						else{
							myFontStyle=FontStyle.Regular;
							myFontFamily=FontFamily.GenericSansSerif;
						}
						if (FontSize[j,i]!=0) myFontSize=FontSize[j,i];
						else if (FontBold[j,i]) myFontSize=8.5f;
						else myFontSize=8.5f;
						//temp:
						//myFontSize=9f;
						//myFontFamily=new FontFamily("Microsoft Sans Serif");
						myFont=new Font(myFontFamily,myFontSize,myFontStyle);
						if(FontColor[j,i].IsEmpty){
							myFontColor=Color.Black;
						}
						else{
							myFontColor=FontColor[j,i];
						}
						if (ColAlign[j]==HorizontalAlignment.Center){
							offset=(int)(((float)ColWidth[j]-grfx.MeasureString(Cell[j,i],myFont).Width)/2);
						}
						else if (ColAlign[j]==HorizontalAlignment.Right){
							if(myFontStyle==FontStyle.Bold)
								offset=ColWidth[j]-(int)Math.Round(System.Convert.ToDouble(grfx.MeasureString(Cell[j,i],myFont).Width))-1;
							else
								offset=ColWidth[j]-(int)Math.Round(System.Convert.ToDouble(grfx.MeasureString(Cell[j,i],myFont).Width*.92))-1;
							//the .92 factor was the only way I could get the numbers to right-align properly
						}
						else offset = 1;
						//grfx.TextRenderingHint=TextRenderingHint.AntiAlias;
						//StringFormat strfmt = new StringFormat();
						//strfmt.Alignment=StringAlignment.Far;
						//strfmt.FormatFlags |=StringFormatFlags.
						grfx.DrawString(Cell[j,i],myFont,new SolidBrush(myFontColor),colPos[j]+offset,rowPos[i]+1);
						//if (Cell[j,i]!=null) grfx.DrawString(Cell[j,i],myFont,new SolidBrush(Color.Black),colPos[j]+ColWidth[j],rowPos[i]+1,strfmt);
					}//end for j
				}//end for i row
			}
			catch{
				//design time
			}
			grfx.DrawLine(penB,0,panelTable.Height-1,panelTable.Width-1,panelTable.Height-1);
			//grfx.DrawLine(penB,0,0,0,panelTable.Height-1);
			grfx=null;
		}//end panelTable_Paint

		private void panelScroll_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			Graphics grfx = e.Graphics;
			Pen penB = new Pen(Color.Black);
			//grfx.DrawLine(penB,0,0,0,panelScroll.Height);
			//grfx.DrawLine(penB,0,panelScroll.Height-3,panelScroll.Width,panelScroll.Height-3);
			//grfx.DrawLine(penB,panelScroll.Width-2,0,panelScroll.Width-2,panelScroll.Height);	
			grfx=null;
		}

		private void ContrTable_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			//This draws the blue border around each table
			Graphics grfx = e.Graphics;
			//Pen penB = new Pen(Color.Black);//bottom, right
			//Pen penW = new Pen(Color.White);//top, left
			Pen penBlue=new Pen(Color.FromArgb(127,157,185));
			//Pen penBlue = new Pen(Color.FromArgb(0,60,116));
			//Pen penGray = new Pen(SystemColors.Control);
			grfx.DrawLine(penBlue,0,0,0,Height-1);//left
			grfx.DrawLine(penBlue,0,Height-1,Width-1,Height-1);//bottom
			grfx.DrawLine(penBlue,0,0,Width,0);//top
			//grfx.DrawLine(penGray,Width,0,Width,Height-1);//right, off the edge
			grfx.DrawLine(penBlue,Width-1,0,Width-1,Height-1);//right
			grfx=null;
		}

		/*public void FillSelectedIAL(){
			SelectedIAL.Clear();
			for(int i=0;i<selectedIndices.Length;i++){
				SelectedIAL.Add(i);
			}
		}*/

		private void panelTable_Click(object sender, System.EventArgs e) {
			//logic moved to mouse up event
		}

		private void panelTable_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			this.OnKeyPress(e);
		}

		private void panelTable_DoubleClick(object sender, System.EventArgs e) {
			//MessageBox.Show("panelTable doubleclicked");
			int myCol=0;
			int myRow=0;
			for (int i=0;i<this.MaxCols;i++){
				if (mouseDownPosition.X>this.colPos[i]) myCol=i;
			}
			for (int i=0;i<this.MaxRows;i++){
				if (mouseDownPosition.Y>this.rowPos[i]) myRow=i;
			}
			OnCellDoubleClicked(new CellEventArgs(myCol,myRow));
		}

		private void panelTable_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			//this.OnMouseDown(e);
			//if(e.Button==MouseButtons.Right)
			
			mouseDownPosition=new Point(e.X,e.Y);
		}

		private void panelTable_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseEventArgs ea=new MouseEventArgs(e.Button,e.Clicks,e.X,e.Y+panelScroll.Top+panelTable.Top,e.Delta);
			this.OnMouseUp(ea);
			if(e.Button==MouseButtons.Right){
				return;
			}
			//this.OnClick(e);
			ArrayList SelectedIAL;
			int myCol=0;
			int myRow=0;
			for (int i=0;i<this.MaxCols;i++){
				if(mouseDownPosition.X>this.colPos[i])
					myCol=i;
			}
			for (int i=0;i<this.MaxRows;i++){
				if (mouseDownPosition.Y>this.rowPos[i]) 
					myRow=i;
			}
			switch(selectionMode){
				case SelectionMode.None:
					break;
				case SelectionMode.One:
					if(SelectedRow!=-1)
						ColorRow(SelectedRow,Color.White);
					ColorRow(myRow,Color.Silver);//SystemColors.Highlight);
					SelectedRow=myRow;
					break;
				/*case SelectRowsMode.OneToggle:
					if(SelectedRow==myRow){
						ColorRow(SelectedRow,Color.White);
						SelectedRow=-1;
					}
					else{
						if(SelectedRow!=-1)
							ColorRow(SelectedRow,Color.White);
						ColorRow(myRow,Color.Silver);
						SelectedRow=myRow;
					}
					break;*/
				case SelectionMode.MultiExtended:
					SelectedIAL=new ArrayList();
					for(int i=0;i<selectedIndices.Length;i++){
						SelectedIAL.Add(selectedIndices[i]);
					}
					if(!ControlIsDown){
						for(int i=0;i<SelectedIAL.Count;i++){
							ColorRow((int)SelectedIAL[i],Color.White);
						}
						SelectedIAL.Clear();
						SelectedIAL.Add(myRow);
						ColorRow(myRow,Color.Silver);//SystemColors.Highlight);
            //}
					}
					else{
						if(SelectedIAL.Contains(myRow)){
							SelectedIAL.Remove(myRow);
							ColorRow(myRow,Color.White);
						}
            else{
						  SelectedIAL.Add(myRow);
						  ColorRow(myRow,Color.Silver);//SystemColors.Highlight);
            }
					}
          selectedIndices=new int[SelectedIAL.Count];
          for(int i=0;i<SelectedIAL.Count;i++){
            selectedIndices[i]=(int)SelectedIAL[i];
          }
					SelectedIAL=null;
					if(selectedIndices.Length==1){
            SelectedRow=selectedIndices[0];
					}
					break;
			}
			SelectedTable=MySelectedTable;//this is to identify one in an array of tables.
			//MessageBox.Show(Row:+" "+myRow+" "+this.ToString());
			OnCellClicked(new CellEventArgs(myCol,myRow));
		}

    private void panelTable_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e){
			if(!ShowScroll) return;
			if(panelTable.Height < panelScroll.Height) return;
			int max=panelTable.Height-panelScroll.Height+3;
			int newScrollVal=vScrollBar1.Value-e.Delta/3;
			if(newScrollVal > max){
				vScrollBar1.Value=max;
			}
			else if(newScrollVal < vScrollBar1.Minimum){
				vScrollBar1.Value=vScrollBar1.Minimum;
			}
			else{
				vScrollBar1.Value=newScrollVal;
			}
			LayoutManager.MoveLocation(panelTable,new Point(0,-vScrollBar1.Value));
    }
		
		private void panelTable_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			this.OnKeyDown(e);
			if (e.KeyCode==Keys.ControlKey){
				ControlIsDown=true;
			}
		}

		private void panelTable_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			this.OnKeyUp(e);
			if (e.KeyCode==Keys.ControlKey){
				ControlIsDown=false;
			}
		}

		private void vScrollBar1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			this.OnKeyPress(e);
		}

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			//Point tempPoint = panelTable.Location;
			//tempPoint.Y=-e.NewValue;
			LayoutManager.MoveLocation(panelTable,new Point(0,-e.NewValue));
			panelTable.Select();
		}

		///<summary></summary>
		public void ScrollToLine(int lineNum){
			if(lineNum<2) lineNum=2;
			int tempLoc=rowPos[lineNum-2];
			if(tempLoc>vScrollBar1.Maximum) tempLoc=vScrollBar1.Maximum;
			if(tempLoc<vScrollBar1.Minimum) tempLoc=vScrollBar1.Minimum;
			vScrollBar1.Value=tempLoc;
			LayoutManager.MoveLocation(panelTable,new Point(0,-tempLoc));
		}

		///<summary></summary>
		public delegate void CellEventHandler(object sender, CellEventArgs e);
		///<summary></summary>
		public event CellEventHandler CellClicked;
		///<summary></summary>
		public event CellEventHandler CellDoubleClicked;
		///<summary></summary>
		protected virtual void OnCellClicked(CellEventArgs e){
			if(CellClicked !=null){
				CellClicked(this,e);
			}
		}

		///<summary></summary>
		protected virtual void OnCellDoubleClicked(CellEventArgs e){
			if(CellDoubleClicked !=null){
				CellDoubleClicked(this,e);
			}
		}

		///<summary></summary>
		public int WorldToLine(int y){
			int retVal=0;
			for(int i=0;i<MaxRows;i++){
				if(y>rowPos[i]){
					retVal=i;
				}
			}
			return retVal;
		}

		private void panelHead_Click(object sender, System.EventArgs e) {
			this.panelTable.Select();
		}

		
		//if (mouseDownPosition.Y>this.rowPos[i]) myRow=i;



	}//end class ContrTable

	///<summary></summary>
	public class CellEventArgs : EventArgs{
		private int col;
		private int row;
		
		///<summary></summary>
		public CellEventArgs(int col, int row) : base(){
			this.row=row;
			this.col=col;
		}

		///<summary></summary>
		public int Col{get{return col;}}
		///<summary></summary>
		public int Row{get{return row;}}
	}

}

