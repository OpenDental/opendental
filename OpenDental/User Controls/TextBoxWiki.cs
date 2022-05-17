using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class TextBoxWiki:UserControl {
		///<summary>This gets set externally to prevent flicker while dragging text.</summary>
		private bool paintIsBlocked;
		//these fields allow dragging text to a new position
		///<summary>Once this is set to true, it will stay true until mouse up.</summary>
		private bool MouseIsDownOnSelectedText;
		private int SelectionStartMem;
		private int SelectionLengthMem;
		private Point MouseDownLocation;
		private Bitmap BitmapWhileDragging;
		///<summary></summary>
		[Category("Property Changed"),Description("Occurs when value of Text property is changed.")]
		public event EventHandler TextChangedWiki=null;
	
		public TextBoxWiki() {
			InitializeComponent();
		}

		#region Properties
		///<summary></summary>
		public ContextMenuStrip ContextMenuStripWiki {
			get {
				return textBoxMain.ContextMenuStrip;
			}
			set {
				textBoxMain.ContextMenuStrip=value;
			}
		}

		///<summary></summary>
		public string SelectedText {
			get {
				return textBoxMain.SelectedText;
			}
			set {
				textBoxMain.SelectedText=value;
			}
		}
		
		///<summary></summary>
		public int SelectionLength {
			get {
				return textBoxMain.SelectionLength;
			}
			set {
				textBoxMain.SelectionLength=value;
			}
		}
		
		///<summary></summary>
		public int SelectionStart{
			get {
				return textBoxMain.SelectionStart;
			}
			set {
				textBoxMain.SelectionStart=value;
			}
		}
		
		///<summary></summary>
		public bool ReadOnly{
			get {
				return textBoxMain.ReadOnly;
			}
			set {
				textBoxMain.ReadOnly=value;
				textBoxMain.BackColor=SystemColors.Window;
			}
		}
		#endregion Properties

		#region overrides
		protected override void OnPaint(PaintEventArgs pe) {
			base.OnPaint(pe);
			if(paintIsBlocked) {
				pe.Graphics.DrawImage(BitmapWhileDragging,new Point(0,0));
			}
		}

		///<summary></summary>
		public override string Text {
			get {
				return textBoxMain.Text;
			}
			set {
				textBoxMain.Text=value;
			}
		}

		public override Font Font {
			get {
				return base.Font;
			}
			set {
				base.Font = value;
				textBoxMain.Font=value;
			}
		}

		#endregion overrides

		#region Passthrough Methods
		public void Copy(){
			textBoxMain.Copy();
		}

		public void Cut() {
			textBoxMain.Cut();
		}

		public int GetCharIndexFromPosition(Point pt) {
			return textBoxMain.GetCharIndexFromPosition(pt);
		}

		public void Paste() {
			textBoxMain.Paste();
		}

		public void Paste(string text) {
			textBoxMain.Paste(text);
		}

		public void Undo() {
			textBoxMain.Undo();
		}
		#endregion Passthrough Methods

		///<summary>This gets set externally to prevent flicker while dragging text.</summary>
		private void BlockPainting(bool newVal) {
			if(newVal) {
				BitmapWhileDragging=new Bitmap(textBoxMain.Width,textBoxMain.Height);
				Rectangle bounds=new Rectangle(new Point(0,0),BitmapWhileDragging.Size);
				textBoxMain.DrawToBitmap(BitmapWhileDragging,bounds);
				paintIsBlocked=true;
				this.Invalidate();
				textBoxMain.Visible=false;
			}
			else {
				textBoxMain.Visible=true;
				paintIsBlocked=false;
				if(BitmapWhileDragging!=null) {
					BitmapWhileDragging.Dispose();
					BitmapWhileDragging=null;
				}
			}
			//this.Invalidate();
		}

		#region Events
		private void textBoxMain_DoubleClick(object sender,EventArgs e) {
			//the default behavior selects the word, but also selects some junk surrounding text.
			string str=textBoxMain.SelectedText;
			int charsRemovedEnd=0;
			int charsRemovedStart=0;
			while(true) {
				if(str.EndsWith(" ")
					|| str.EndsWith(".")
					|| str.EndsWith(",")
					|| str.EndsWith("!")
					|| str.EndsWith("]")
					|| str.EndsWith("}")
					|| str.EndsWith(">")
					|| str.EndsWith("'")
					|| str.EndsWith("\"")
					|| str.EndsWith("|")
					|| str.EndsWith("-")) 
				{
					str=str.Substring(0,str.Length-1);
					charsRemovedEnd++;
				}
				else if(str.StartsWith(" ")
					|| str.StartsWith(".")
					|| str.StartsWith(",")
					|| str.StartsWith("!")
					|| str.StartsWith("[")
					|| str.StartsWith("{")
					|| str.StartsWith("<")
					|| str.StartsWith("/")
					|| str.StartsWith("'")
					|| str.StartsWith("\"")
					|| str.StartsWith("|")
					|| str.StartsWith("-")) 
				{
					str=str.Substring(1,str.Length-1);
					charsRemovedStart++;
				}
				else if(str.Contains("<")){
					charsRemovedEnd+=(str.Length-str.IndexOf("<"));
					str=str.Substring(0,str.IndexOf("<"));
				}
				else if(str.Contains(">")) {
					charsRemovedStart+=(str.IndexOf(">")+1);
					str=str.Substring(str.IndexOf(">")+1);
				}
				else{
					break;
				}
			}
			textBoxMain.SelectionStart+=charsRemovedStart;
			textBoxMain.SelectionLength-=(charsRemovedStart+charsRemovedEnd);
		}

		private void textBoxMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			this.OnMouseDoubleClick(e);
		}

		private void textBoxMain_KeyPress(object sender,KeyPressEventArgs e) {
			this.OnKeyPress(e);
		}

		private void textBoxMain_KeyDown(object sender,KeyEventArgs e) {
			//the KeyPress event was useless because it wasn't even firing for the Delete key.
			switch(e.KeyCode) {
				case Keys.ControlKey:
				case Keys.ShiftKey:
					//don't reset the SelectionLengthMem to 0 because the text has not be deselected yet.
					break;
				default:
					SelectionLengthMem=0;//typing a char such as backspace is a common way to move from selected text to a state where no text is selected. 
					break;
				case Keys.Tab:
					textBoxMain.Paste("   ");
					SelectionLengthMem=0;
					e.SuppressKeyPress=true;
					break;
			}
		}

		private void textBoxMain_MouseDown(object sender,MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) != MouseButtons.Left) {
				return;
			}
			//unfortunately, by the time this fires, the text is already deselected, but we remembered it
			if(SelectionLengthMem==0) {
				return;
			}
			//some text must have been selected already, and we need to decide whether we are going to try to drag the selected text.
			if(textBoxMain.GetCharIndexFromPosition(e.Location)<SelectionStartMem) {
				return;//clicked on a point before the selected text
			}
			if(textBoxMain.GetCharIndexFromPosition(e.Location)>SelectionStartMem+SelectionLengthMem) {
				return;//clicked on a point after the selected text
			}
			MouseDownLocation=e.Location;
			//reselect the text for visual cue
			textBoxMain.SelectionStart=SelectionStartMem;
			textBoxMain.SelectionLength=SelectionLengthMem;
			MouseIsDownOnSelectedText=true;
		}

		private void textBoxMain_MouseMove(object sender,MouseEventArgs e) {
			if(!MouseIsDownOnSelectedText) {
				return;
			}
			//the textbox has internal code for reselecting text as we move.  Need to override.
			textBoxMain.SelectionStart=SelectionStartMem;
			textBoxMain.SelectionLength=SelectionLengthMem;
			if(paintIsBlocked) {
				//painting already blocked to prevent flicker	
			}
			else {//this is the first time the mouse has moved at all since the mouse down on selected text
				textBoxMain.Cursor=Cursors.Arrow;
				BlockPainting(true);//block painting to prevent flicker
			}
		}

		private void textBoxMain_MouseUp(object sender,MouseEventArgs e) {
			if(MouseIsDownOnSelectedText) {
				int charIndexMouseUp=textBoxMain.GetCharIndexFromPosition(e.Location);
				if(Math.Abs(e.X-MouseDownLocation.X)<4 && Math.Abs(e.Y-MouseDownLocation.Y)<4) {
					//if mouse didn't move very much between down and up, then it's a click instead of a drag, so deselect the selected text
					textBoxMain.SelectionStart=charIndexMouseUp;
					textBoxMain.SelectionLength=0;
				}
				else {//drag
					//remember the selected text
					string selectedText=textBoxMain.SelectedText;
					if(charIndexMouseUp<SelectionStartMem) {//new position is before old position
						//remove selected text from content
						textBoxMain.Text=textBoxMain.Text.Substring(0,SelectionStartMem)+textBoxMain.Text.Substring(SelectionStartMem+SelectionLengthMem);
						//add the text back in at the new spot
						textBoxMain.Text=textBoxMain.Text.Substring(0,charIndexMouseUp)+selectedText+textBoxMain.Text.Substring(charIndexMouseUp);
						//highlight the newly moved text for visual cue
						textBoxMain.SelectionStart=charIndexMouseUp;
						textBoxMain.SelectionLength=SelectionLengthMem;
					}
					else if(charIndexMouseUp < SelectionStartMem+SelectionLengthMem){//new position is within the selected text
						//do nothing
					}
					else {//new position is after old position
						//add the text in at the new spot
						textBoxMain.Text=textBoxMain.Text.Substring(0,charIndexMouseUp)+selectedText+textBoxMain.Text.Substring(charIndexMouseUp);
						//then, remove it from the old spot
						textBoxMain.Text=textBoxMain.Text.Substring(0,SelectionStartMem)+textBoxMain.Text.Substring(SelectionStartMem+SelectionLengthMem);
						//highlight the newly moved text for visual cue
						textBoxMain.SelectionStart=charIndexMouseUp-SelectionLengthMem;
						textBoxMain.SelectionLength=SelectionLengthMem;
					}
				}
				//whether it was a click or drag, reset all our variables
				BlockPainting(false);
				MouseDownLocation=new Point();
				SelectionStartMem=textBoxMain.SelectionStart;//these two actually are important in case we want to drag the text again
				SelectionLengthMem=textBoxMain.SelectionLength;
				MouseIsDownOnSelectedText=false;
				textBoxMain.Cursor=Cursors.IBeam;
			}
			else {
				//this will happen after text is initially highlighted
				SelectionStartMem=textBoxMain.SelectionStart;
				SelectionLengthMem=textBoxMain.SelectionLength;
			}
		}

		private void textBoxMain_TextChanged(object sender,EventArgs e) {
			if(TextChangedWiki!=null) {
				TextChangedWiki(this,new EventArgs());
			}
		}
		#endregion Events

		

		

		

		

		

	}
}
