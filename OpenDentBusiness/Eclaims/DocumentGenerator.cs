using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness.Eclaims {
	public class DocumentGenerator {

		#region Internal Classes and Structures

		private abstract class RenderPrim {//Graphical element. Is always as wide as the page.
			public abstract float Height(Graphics g);
			public abstract void Render(Graphics g,float Y);
			public abstract RenderPrim Clone();
			///<summary>Returns true if the space taken up by this primitive is at least 1 square pixel.</summary>
			public abstract bool HasDims();
		};
		private class BlankPrim:RenderPrim {
			float height;
			public BlankPrim(float pHeight) {
				height=pHeight;
			}
			public override float Height(Graphics g) {
				return height;
			}
			public override void Render(Graphics g,float Y) {
				return;
			}
			public override RenderPrim Clone() {
				return new BlankPrim(this.height);
			}
			public override bool HasDims() {
				return (height>0);
			}
		}
		private class PageBreak:RenderPrim{
			public override float Height(Graphics g) {
				return 0;//Has a height but depends on the final page rendering.
			}
			public override void Render(Graphics g,float Y) {
			}
			public override RenderPrim Clone() {
				return new PageBreak();
			}
			public override bool HasDims() {
				return false;
			}
		}
		private class RenderStr:RenderPrim {
			Font font;
			Pen pen;
			Size layout;
			string text;
			float x;
			float yOffset;
			public RenderStr(Font pFont,Pen pPen,Size pLayout,string pText,float pX,float pYOffset) {
				font=pFont;
				pen=pPen;
				layout=pLayout;
				text=pText;
				x=pX;
				yOffset=pYOffset;
			}
			public override float Height(Graphics g) {
				return yOffset+g.MeasureString(text,font,layout).Height;
			}
			public override void Render(Graphics g,float Y) {
				g.DrawString(text,font,pen.Brush,new Rectangle((int)Math.Floor(x),(int)Math.Floor(Y+yOffset),layout.Width,
					layout.Height));
			}
			public override RenderPrim Clone() {
				return new RenderStr(this.font,this.pen,this.layout,this.text,this.x,this.yOffset);
			}
			public override bool HasDims() {
				return (text.Length>0);
			}
		};
		private class RenderHLine:RenderPrim {
			Pen pen;
			float x1;
			float x2;
			float yOffset;
			public RenderHLine(Pen pPen,float pX1,float pX2,float pYOffset) {
				pen=pPen;
				x1=pX1;
				x2=pX2;
				yOffset=pYOffset;
			}
			public override float Height(Graphics g) {
				return yOffset+pen.Width;
			}
			public override void Render(Graphics g,float Y) {
				Y=(float)Math.Ceiling(yOffset+Y);
				g.DrawLine(pen,x1,Y,x2,Y);
			}
			public override RenderPrim Clone() {
				return new RenderHLine(this.pen,this.x1,this.x2,this.yOffset);
			}
			public override bool HasDims() {
				return (pen.Width>0 && x1!=x2);
			}
		};

		#endregion

		#region Internal Variables

		private List<RenderPrim> renderContainer=new List<RenderPrim>();
		///<summary>Contains an array of elements/render containers for the whole document.</summary>
		private List<List<RenderPrim>> documentContainer=new List<List<RenderPrim>>();
		///<summary>Used to store indentation x-values</summary>
		private List<float> xstack=new List<float>();
		///<summary>Current dimensions for each printed page.</summary>
		public Rectangle bounds=new Rectangle(0,0,0,0);
		///<summary>Current font.</summary>
		public Font standardFont=new Font(FontFamily.GenericMonospace,10);

		#endregion

		#region APIs

		public void PrintPage(Graphics g,int pageNum){
			int page=0;
			int numPageElements=0;
			int offset=0;
			do{
				offset=numPageElements;
				numPageElements=CalcNumElementsInWholePage(g,numPageElements);
				page++;
			}while(pageNum>page);
			float yWritten=0;
			for(int i=0;i<numPageElements;i++) {
				if(offset+i<documentContainer.Count) {
					List<RenderPrim> renderGroup=documentContainer[offset+i];
					foreach(RenderPrim prim in renderGroup) {
						prim.Render(g,bounds.Top+yWritten);
					}
					yWritten+=CalcElementHeight(g,renderGroup);
				}
			}
		}

		public int CalcTotalPages(Graphics g) {
			int pages=1;
			int elementsProcessed=CalcNumElementsInWholePage(g,0);
			while(documentContainer.Count-elementsProcessed>0) {
				pages++;
				elementsProcessed+=CalcNumElementsInWholePage(g,elementsProcessed);
			}
			return pages;
		}

		private float CalcElementHeight(Graphics g,List<RenderPrim> element) {
			float maxy=0;
			foreach(RenderPrim prim in element) {
				maxy=Math.Max(maxy,prim.Height(g));
			}
			return maxy;
		}

		private int CalcNumElementsInWholePage(Graphics g,int offset) {
			float totalY=0;
			int numElements=0;
			float maxy=0;
			int i;
			for(i=offset;i<documentContainer.Count;i++){
				//Page breaks end the current page.
				if(documentContainer[i].Count==1){
					Type elementType=documentContainer[i][0].GetType();
					if(elementType==typeof(PageBreak)){
						numElements++;
						break;
					}
				}
				maxy=CalcElementHeight(g,documentContainer[i]);
				if(totalY+maxy < bounds.Height){
					numElements++;
					totalY+=maxy;
				}else{
					break;
				}
			}
			return((numElements<1 && documentContainer.Count-offset>0)?1:numElements);//At least one element
		}

		public SizeF DrawField(Graphics g,string fieldName,string value,bool alwaysShow,float X,float Y) {
			return DrawField(g,fieldName,value,alwaysShow,X,Y,": ");
		}

		///<summary>Prints the contents of the field if they are non-empty. Also prints the field's name if it is not empty, or if alwaysShowName is true.</summary>
		public SizeF DrawField(Graphics g,string fieldName,string value,bool alwaysShow,float X,float Y,string divideStr) {
			if(fieldName==null) {//should never happen
				MessageBox.Show(this.ToString()+".DrawField: Internal error, attempt to render null field name (Out of memory?)");
				return new SizeF(0,0);
			}
			if(value==null) {//Allow null to count as empty string.
				value="";
			}
			if(alwaysShow || value.Length>0) {
				SizeF size1=DrawString(g,fieldName+divideStr,X,Y);
				SizeF size2=DrawString(g,value,X+size1.Width,Y);
				//For most situations when the fields are put in a good place on the output page where the field label fits
				//in the page bounds and the field value has a reasonable amount of horizontal space, the width will simply
				//be the width of the two rendered strings added together. This will be the common case and is the only 
				//relevant possibility.
				return new SizeF(size1.Width+size2.Width,Math.Max(size1.Height,size2.Height));
			}
			return new SizeF(0,0);
		}

		///<summary>The purpose of this method is to test each string before printing.  It should only print strings that belong on the current page.</summary>
		public SizeF DrawString(Graphics g,string text,float X,float Y) {
			return DrawString(g,text,X,Y,standardFont);
		}

		///<summary>The purpose of this method is to test each string before printing.  It should only print strings that belong on the current page.  y gets passed in as if it were all one long page.</summary>
		public SizeF DrawString(Graphics g,string text,float X,float Y,Font font) {
			return DrawString(g,text,X,Y,font,-1);
		}

		///<summary>The purpose of this method is to test each string before printing.  It should only print strings that belong on the current page.  y gets passed in as if it were all one long page. Use maxPixelWidth to define a maximum width for the output string, which will be truncated to the nearest character that fits within the amount specified. Use maxPixelWidth=-1 to remove width limit.</summary>
		public SizeF DrawString(Graphics g,string text,float X,float Y,Font font,int maxPixelWidth) {
			if(text==null) {
				text="";
			}
			Size renderArea=new Size(Convert.ToInt32(Math.Truncate(bounds.Right-X+1)),bounds.Height);
			if(maxPixelWidth>=0) {
				renderArea.Width=Math.Min(maxPixelWidth,renderArea.Width);
			}
			SizeF size=g.MeasureString(text,font,renderArea);
			renderContainer.Add(new RenderStr(font,Pens.Black,renderArea,text,X,Y));
			return size;
		}

		public SizeF HorizontalLine(Graphics g,Pen pen,float x1,float x2,float Y) {
			renderContainer.Add(new RenderHLine(pen,x1,x2,Y));
			return new SizeF(x2-x1,pen.Width);
		}

		///<summary>Pushes/Stores the current global x value onto the front of the stack to be used for indentaion.</summary>
		public void PushX(float x) {
			xstack.Add(x);//Adds to the end of the list.
		}

		///<summary>Pops/Removes the x value on the front of the stack and resets the x value to the next x value in the x stack. </summary>
		public float PopX() {
			if(xstack.Count > 0) {
				xstack.RemoveAt(xstack.Count-1);
			}
			return ResetX();
		}

		///<summary>Gets the x value on the front of the stack and sets the global x value to the returned value. Does not remove the x value from the x stack.</summary>
		public float ResetX() {
			float x=bounds.Left;
			if(xstack.Count > 0) {
				x=xstack[xstack.Count-1];
			}
			return x;
		}

		///<summary>Returns the x-value of the beginning of the left-hand side of the element.</summary>
		public float StartElement(float extraJumpY,bool alwaysJump) {
			//Get max height of the container elements.
			bool hasDims=false;
			foreach(RenderPrim prim in renderContainer) {
				if(prim.HasDims()) {
					hasDims=true;
					break;
				}
			}
			documentContainer.Add(ClonePrims());
			if((hasDims && extraJumpY>0) || alwaysJump) {	//This prevents getting odd offsets for text rows near the beginning of a page,
				//or from creating a series of white-space.
				renderContainer.Clear();
				renderContainer.Add(new BlankPrim(extraJumpY));
				documentContainer.Add(ClonePrims());
			}
			renderContainer.Clear();
			return ResetX();
		}

		public float StartElement(float extraJumpY) {
			return StartElement(extraJumpY,false);
		}

		///<summary>Same as StartElement(0).</summary>
		public float StartElement() {
			return StartElement(0);
		}

		///<summary>Inserts a page break at the very end of the document to force proceeding elements to begin at the top of the next page.</summary>
		public void NextPage(){
			StartElement();
			renderContainer.Add(new PageBreak());
			StartElement();
		}

		private List<RenderPrim> ClonePrims() {
			List<RenderPrim> dup=new List<RenderPrim>();
			foreach(RenderPrim prim in renderContainer) {
				dup.Add(prim.Clone());
			}
			return dup;
		}

		#endregion

	}
}
