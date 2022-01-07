using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.UI{
	///<summary>Open Dental Toolbar. Jordan is the only one allowed to edit this file.</summary>
	[DefaultEvent("ButtonClick")]
	public class ToolBarOD : System.Windows.Forms.UserControl{
		#region Fields
		private ODToolBarButtonCollection buttons=new ODToolBarButtonCollection();
		private ImageList imageList;
		private bool _isMouseDown;
		///<summary>Just holds the scaling factor.</summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>A hot button is either: 1.The button that the mouseDown happened on, regardless of the current position of the mouse, or 2.If the mouse is not down, the button in State.Hover. Keeping track of which one is hot allows faster painting during mouse events.</summary>
		///<summary>Either 1 or 2, depending on the scale.  Float thicknesses don't look good.  Not all lines are affected, only the ones that need to be seen.</summary>
		private int _lineThickness;
		private ODToolBarButton _toolBarButtonHot;
		private ToolTip toolTip1;
		///<summary>This can be set from anywhere to affect all toolbars simultaneously.</summary>
		private ValidNum textPageNav;
		//These brushes all get disposed.
		private LinearGradientBrush _brushTogglePushed=null;
		private LinearGradientBrush _brushTogglePushError=null;
		private LinearGradientBrush _brushMain=null;
		private LinearGradientBrush _brushMainError=null;
		///<summary>Darker</summary>
		private LinearGradientBrush _brushPushed=null;
		private LinearGradientBrush _brushPushedError=null;
		private LinearGradientBrush _brushNotify=null;
		//pens get disposed
		private Pen _penDivider=null;
		private Pen _penOutline=null;
		private Pen _penTogglePushed=null;
		//string formats are disposed
		private StringFormat _stringFormatCenter=null;
		private StringFormat _stringFormatLeft=null;
		///<summary>This is temporarily used to fix artifacts from Direct2D.</summary>
		Timer timer;
		private int _widthDropDown=15;
		#endregion Fields

		#region Fields - Private Static
		//These are shared by all buttons and don't get disposed
		private static Brush _brushText=Brushes.Black;
		private static Brush _brushTextDisabled=SystemBrushes.GrayText;
		#endregion Fields - Private Static

		#region Constructor
		///<summary></summary>
		public ToolBarOD(){
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			DoubleBuffered=true;
			toolTip1 = new ToolTip();
			toolTip1.InitialDelay=1100;
			_stringFormatCenter=new StringFormat();
			_stringFormatCenter.Alignment=StringAlignment.Center;
			_stringFormatCenter.LineAlignment=StringAlignment.Center;
			_stringFormatLeft=new StringFormat();
			_stringFormatLeft.Alignment=StringAlignment.Near;
			_stringFormatLeft.LineAlignment=StringAlignment.Center;
			timer=new Timer();
			timer.Interval=1000;//redraw every 1 second to remove artifacts
			timer.Enabled=true;
			timer.Tick+=timer_Tick;
		}
		#endregion Constructor

		#region Designer
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.Container components = null;

		///<summary>Clean up any resources being used.</summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				components?.Dispose();
				_brushTogglePushed?.Dispose();
				_brushTogglePushError?.Dispose();
				_brushMain?.Dispose();
				_brushMainError?.Dispose();
				_brushPushed?.Dispose();
				_brushPushedError?.Dispose();
				_brushNotify?.Dispose();
				_penDivider?.Dispose();
				_penOutline?.Dispose();
				_penTogglePushed?.Dispose();
				_stringFormatCenter?.Dispose();
				_stringFormatLeft?.Dispose();
				for(int i=0;i<Buttons.Count;i++){
					Buttons[i].Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion Designer

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ToolBarOD
			// 
			this.DoubleBuffered = true;
			this.Name = "ToolBarOD";
			this.ResumeLayout(false);

		}
		#endregion

		#region Events - Raise
		///<summary></summary>
		protected void OnButtonClicked(ODToolBarButton toolBarButton){
			if(toolBarButton.DateTimeLastClicked.AddMilliseconds(SystemInformation.DoubleClickTime)>DateTime.Now) {
				return;
			}
			toolBarButton.DateTimeLastClicked=DateTime.Now;
			ODToolBarButtonClickEventArgs toolBarButtonClickEventArgs=new ODToolBarButtonClickEventArgs(toolBarButton);
			ButtonClick?.Invoke(this,toolBarButtonClickEventArgs);
		}
		[Category("OD")]
		[Description("Occurs when a button is clicked.")]
		public event ODToolBarButtonClickEventHandler ButtonClick=null;

		///<summary></summary>
		private void OnPageNav(int pageNum){
			PageNav?.Invoke(this,new ODToolBarButtonPageNavEventArgs(pageNum));
		}
		[Category("OD")]
		[Description("Occurs when user types number and hits Enter in a page navigation textbox.")]
		public event ODToolBarButtonPageNavEventHandler PageNav=null;
		#endregion Events - Raise

		#region Methods - Event Handlers
		private void TextPageNav_KeyDown(object sender,KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode==Keys.Enter && textPageNav.IsValid()) {
				int pageNum=textPageNav.MaxVal;
				try {
					pageNum=Int32.Parse(textPageNav.Text);
				}
				catch(Exception) {
					return;
				}
				OnPageNav(pageNum);
			}
		}

		private void timer_Tick(object sender,EventArgs e) {
			//this happens soon after the last resize
			Invalidate();
			if(timer.Interval==1000){
				timer.Enabled=false;//so it only runs twice
			}
			timer.Interval=1000;//set it back to the longer interval
		}
		#endregion Methods - Event Handlers

		#region Methods - On Override
		///<summary></summary>
		protected override void OnLoad(EventArgs e){
			base.OnLoad(e);
			InitializeBrushes();
		}

		///<summary>Change the button to a pressed state.</summary>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e){
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left)!=MouseButtons.Left){
				return;
			}
			_isMouseDown=true;
			ODToolBarButton button=HitTest(e.X,e.Y);
			if(button==null){//if there is no current hover button
				return;//don't set a hotButton
			}
			//if(!button.Enabled){
			//	return;//disabled buttons don't respond
			//}
			_toolBarButtonHot=button;
			if(button.Style==ODToolBarButtonStyle.DropDownButton
				&& HitTestDrop(button,e.X,e.Y))
			{
				button.State=ToolBarButtonState.DropPressed;
			}
			else{
				button.State=ToolBarButtonState.Pressed;
				
			}
			Invalidate();//button.Bounds);
		}

		///<summary>Resets button appearance. This will also deactivate the button if it has been pressed but not released. A pressed button will still be hot, however, so that if the mouse enters again, it will behave properly.  Repaints only if necessary.</summary>
		protected override void OnMouseLeave(System.EventArgs e){
			base.OnMouseLeave(e);
			if(_isMouseDown){//mouse is down
				//if a button is hot, it will remain so, even if leave.  As long as mouse is down.
				//,so do nothing.
				//Also, if a button is not hot, nothing will happen when leave
				//,so do nothing.
			}
			else{//mouse is not down
				if(_toolBarButtonHot!=null){//if there was a previous hotButton
					_toolBarButtonHot.State=ToolBarButtonState.Normal;
					Invalidate();//_toolBarButtonHot.Bounds);
					_toolBarButtonHot=null;
				}
			}
		}

		///<summary>This should only happen when mouse enters. Only causes a repaint if needed.</summary>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e){
      base.OnMouseMove(e);
			if(_isMouseDown){
				//regardless of whether a button is hot, nothing changes until the mouse is released.
				//a hot(pressed) button remains so, and no buttons are hot when hover
				//,so do nothing
				return;
			}
			//mouse is not down
			ODToolBarButton toolBarButtonOver=HitTest(e.X,e.Y);
			if(_toolBarButtonHot!=null){//first handle any old hot button
				if(_toolBarButtonHot!=toolBarButtonOver){//if we have moved to hover over a new button, or to hover over nothing
					_toolBarButtonHot.State=ToolBarButtonState.Normal;
					Invalidate();//_toolBarButtonHot.Bounds);
				}
			}
			if(toolBarButtonOver!=null){//then, the new button
				if(_toolBarButtonHot!=toolBarButtonOver){//if we have moved to hover over a new button
					toolTip1.SetToolTip(this,toolBarButtonOver.ToolTipText);
					toolBarButtonOver.State=ToolBarButtonState.Hover;
					Invalidate();//toolBarButtonOver.Bounds);
				}
				else{//Still hovering over the same button as before
					//do nothing.
				}
			}
			else{
				toolTip1.SetToolTip(this,"");
			}
			_toolBarButtonHot=toolBarButtonOver;//this might be null if hovering over nothing.
		}

		///<summary>Change button to hover state and repaint if needed.</summary>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e){
			base.OnMouseUp(e);
      if((e.Button & MouseButtons.Left)!=MouseButtons.Left){
				return;
			}
			//Make sure we registered a MouseDown event inside of OD, otherwise, we might mistakenly act on a mouse_up that did not begin inside OD.
			if(!_isMouseDown) {
				return;
			}
			_isMouseDown=false;
			ODToolBarButton toolBarButton=HitTest(e.X,e.Y);
			if(_toolBarButtonHot==null){//if there was not a previous hotButton
				//do nothing
			}	
			else{//there was a hotButton
				_toolBarButtonHot.State=ToolBarButtonState.Normal;
				//but can't set it null yet, because still need it for testing
				Invalidate();//_toolBarButtonHot.Bounds);//invalidate the old button
				//CLICK: 
				if(_toolBarButtonHot==toolBarButton){//if mouse was released over the same button as it was depressed
					if(!toolBarButton.Enabled){
						//disabled buttons don't respond at all
					}
					else if(toolBarButton.Style==ODToolBarButtonStyle.DropDownButton//if current button is dropdown
						&& HitTestDrop(toolBarButton,e.X,e.Y))//and we are in the dropdown area on the right
					{
						if(toolBarButton.DropDownMenu!=null){//there is a dropdown menu to display
							_toolBarButtonHot=null;
							toolBarButton.State=ToolBarButtonState.Normal;
							Invalidate();//toolBarButton.Bounds);
							ContextMenu contextMenu=toolBarButton.DropDownMenu.GetContextMenu();
							contextMenu.Show(this,new Point(toolBarButton.Bounds.X,toolBarButton.Bounds.Y+toolBarButton.Bounds.Height));
						}
						if(toolBarButton.ContextMenuStripDropDown!=null){
							_toolBarButtonHot=null;
							toolBarButton.State=ToolBarButtonState.Normal;
							Invalidate();
							toolBarButton.ContextMenuStripDropDown.Show(this,new Point(toolBarButton.Bounds.X,toolBarButton.Bounds.Y+toolBarButton.Bounds.Height));
						}
					}
					else if(toolBarButton.Style==ODToolBarButtonStyle.ToggleButton){//if current button is a toggle button
						if(toolBarButton.Pushed){
							toolBarButton.Pushed=false;
						}
						else{
							toolBarButton.Pushed=true;
						}
						OnButtonClicked(toolBarButton);
					}
					else if(toolBarButton.Style==ODToolBarButtonStyle.Label){
						//lables do not respond with click
					}
					else{
						OnButtonClicked(toolBarButton);
					}
					return;//the button will not revert back to hover
				}//end of click section
				else{//there was a hot button, but it did not turn into a click
					_toolBarButtonHot=null;
				}
			}
			if(toolBarButton!=null){//no click, and now there is a hover button, not the same as original button.
				//this section could easily be deleted, since all the user has to do is move the mouse slightly.
				toolBarButton.State=ToolBarButtonState.Hover;
				_toolBarButtonHot=toolBarButton;//set the current hover button to be the new hotbutton
				Invalidate();//toolBarButton.Bounds);
			}
		}

		///<summary>Runs any time the control is invalidated.</summary>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			if(DesignMode){
				e.Graphics.DrawRectangle(Pens.SlateGray,0,0,Width-1,Height-1);
				StringFormat format=new StringFormat();
				format.Alignment=StringAlignment.Center;
				format.LineAlignment=StringAlignment.Center;
				e.Graphics.DrawString(this.Name,Font,Brushes.Black,new Rectangle(0,0,Width,Height),format);
				return;
			}
			//e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
			//OnPaint gets called a lot and the button collection can change while it is in the middle of drawing which can crash the program.
			//It's easier to just swallow any random exception that occurs because OnPaint will most likely get fired again real soon.
			ODException.SwallowAnyException(() => {
				ComputeButtonsizes(e.Graphics);
				for(int i=0;i<buttons.Count;i++) {
					DrawButton(e.Graphics,buttons[i]);
				}
				e.Graphics.DrawLine(Pens.SlateGray,0,Height-1,Width-1,Height-1);
				//For quite some time, the Paint event was never fired here.  Added by Sam and AndrewD so we can subscribe to the Paint event.
				base.OnPaint(e);
			});
		}

		protected override void OnSizeChanged(EventArgs e){
			base.OnSizeChanged(e);
			InitializeBrushes();
			if(timer.Enabled){
				timer.Enabled=false;//restart the timer
			}
			timer.Interval=100;//a bit faster than normal
			timer.Enabled=true;
		}
		#endregion Methods - On Override

		#region Properties
		///<summary>Gets the collection of ODToolBarButton controls assigned to the toolbar control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public ODToolBarButtonCollection Buttons{
			get{
				return buttons;
			}
			//set{
			//}	
		}

		protected override Size DefaultSize => new Size(300,25);

		///<summary>Gets or sets the collection of images available to the toolbar buttons.</summary>
		[Category("OD")]
		[Description("Gets or sets the collection of images available to the toolbar buttons.")]
		[DefaultValue(null)]
		public ImageList ImageList{
			get{
				return imageList;
			}
			set{
				imageList=value;
			}
		}
		#endregion Properties

		#region Methods - Private
		private void ComputeButtonsizes(Graphics g){
			_widthDropDown=LayoutManager.Scale(15);//The width of the dropdown rectangle where the triangle shows.
			int xPos=0;
			int width;
			for(int i=0;i<buttons.Count;i++){
				if(buttons[i].Style==ODToolBarButtonStyle.Separator) {
					width=LayoutManager.Scale(4);
				}
				else if(buttons[i].Style==ODToolBarButtonStyle.PageNav) {
					//Set the size of the button to the width measurement of "0000/0000" with some padding.
					//The page navigation is tailored to print previews and users shouldn't have reports much larger than 9999 pages.
					width=(int)g.MeasureString("0000/0000",Font).Width+LayoutManager.Scale(12);
				}
				else if(buttons[i].Bitmap!=null){
					int widthImage=LayoutManager.Scale(buttons[i].Bitmap.Width);
					if(buttons[i].Text==""){
						width=widthImage+LayoutManager.Scale(7);//slightly wider than high for better 'feel'
					}
					else{
						width=widthImage+(int)g.MeasureString(buttons[i].Text,Font).Width+LayoutManager.Scale(8);
					}
				}
				else if(buttons[i].Icon!=EnumIcons.None){//button with Icon
					int widthImage=LayoutManager.Scale(22);
					if(buttons[i].Text==""){
						width=widthImage+LayoutManager.Scale(7);//slightly wider than high for better 'feel'
					}
					else{
						width=widthImage+(int)g.MeasureString(buttons[i].Text,Font).Width+LayoutManager.Scale(8);
					}
				}
				else if(imageList==null || buttons[i].ImageIndex==-1) {//Normal button
					width=(int)g.MeasureString(buttons[i].Text,Font).Width+LayoutManager.Scale(6);
				}
				else {//Image List button (being deprecated)
					int widthImage=LayoutManager.Scale(imageList.ImageSize.Width);
					if(buttons[i].Text==""){
						width=widthImage+LayoutManager.Scale(7);//slightly wider than high for better 'feel'
					}
					else{
						width=widthImage+(int)g.MeasureString(buttons[i].Text,Font).Width+LayoutManager.Scale(8);
					}
				}
				//Check to see if this button needs a drop down.
				if(buttons[i].Style==ODToolBarButtonStyle.DropDownButton){
					if(buttons[i].Text==""){
						width-=LayoutManager.Scale(6);//10//this way the main button part can be zero width if we just want the dropdown and nothing else.
					}
					width+=_widthDropDown;
				}
				buttons[i].Bounds=new Rectangle(new Point(xPos,0),new Size(width,Height-1));
				xPos+=buttons[i].Bounds.Width;
			}
		}

		///<summary></summary>
		private void DrawButton(Graphics g,ODToolBarButton button){
			if(button.Style==ODToolBarButtonStyle.Separator){
				g.FillRectangle(SystemBrushes.Control,button.Bounds);
				return;
			}
			LinearGradientBrush brushTogglePushed=_brushTogglePushed;
			LinearGradientBrush brushMain=_brushMain;
			LinearGradientBrush brushPushed=_brushPushed;
			//these two gradient colors are a little different than the other colors.
			//They are just a hack for Direct2d to match up that gradient with the DirectX one.
			//They were obtained by color matching from a screenshot.
			Color colorGradientTop=Color.FromArgb(253,253,254);
			Color colorGradientBottom=Color.FromArgb(181,190,215);
			if(button.State==ToolBarButtonState.Pressed){//might be missing a few edge cases
				colorGradientTop=Color.FromArgb(218,218,221);
				colorGradientBottom=Color.FromArgb(151,160,185);
			}
			if(button.IsRed) {//draw with error colors. Only used for DoseSpot
				brushTogglePushed=_brushTogglePushError;
				brushMain=_brushMainError;
				brushPushed=_brushPushedError;
			}
			if(!button.Enabled){
				g.FillRectangle(brushMain,button.Bounds);
			}
			else if(button.Style==ODToolBarButtonStyle.ToggleButton && button.Pushed){
				g.FillRectangle(brushTogglePushed,button.Bounds);
			}
			else if(button.Style==ODToolBarButtonStyle.Label || button.Style==ODToolBarButtonStyle.PageNav){
				g.FillRectangle(brushMain,button.Bounds);
			}
			else switch(button.State){
				case ToolBarButtonState.Normal:
					g.FillRectangle(brushMain,button.Bounds);
					break;
				case ToolBarButtonState.Hover:
					g.FillRectangle(brushMain,button.Bounds);
					break;
				case ToolBarButtonState.Pressed:
					g.FillRectangle(brushPushed,button.Bounds);
					break;
				case ToolBarButtonState.DropPressed:
					//left half looks normal:
					g.FillRectangle(brushMain,new Rectangle(button.Bounds.X,button.Bounds.Y,button.Bounds.Width-_widthDropDown,button.Bounds.Height));
					//right section looks like Pressed:
					g.FillRectangle(brushPushed,new Rectangle(button.Bounds.X+button.Bounds.Width-_widthDropDown,button.Bounds.Y,_widthDropDown,button.Bounds.Height));
					break;
			}
			bool isNotify=(button.Style==ODToolBarButtonStyle.DropDownButton && !string.IsNullOrWhiteSpace(button.NotificationText));//Notifies even when disabled.
			if(isNotify) {//Override dropdown background to show notification color.
				Rectangle rectDropDown=new Rectangle(button.Bounds.X+button.Bounds.Width-_widthDropDown,button.Bounds.Y,_widthDropDown,button.Bounds.Height);
				g.FillRectangle(_brushNotify,rectDropDown);//Fill the dropdown background area with the notification color.
			}
			//Text, lines, images=========================================================================================================================================
			if(button.Style==ODToolBarButtonStyle.Separator){
				GraphicsHelper.DrawLine(g,_penDivider,button.Bounds.Right-1f,button.Bounds.Top,button.Bounds.Right-1f,button.Bounds.Bottom);
				return;
			}
			//draw image and/or text
			Rectangle rectangleText;
			Rectangle rectangleImage;
			int textWidth=button.Bounds.Width;
			if(button.Style==ODToolBarButtonStyle.DropDownButton){
				textWidth-=_widthDropDown;
			}
			Bitmap bitmap=null;
			Size sizeImage=new Size();
			if(button.Icon!=EnumIcons.None){
				sizeImage=new Size(LayoutManager.Scale(22),LayoutManager.Scale(22));
			}
			else if(button.Bitmap!=null){
				bitmap=button.Bitmap;
				sizeImage=new Size(LayoutManager.Scale(bitmap.Width),LayoutManager.Scale(bitmap.Height));
			}
			else if(imageList!=null && button.ImageIndex!=-1 && button.ImageIndex<imageList.Images.Count){
				bitmap=(Bitmap)imageList.Images[button.ImageIndex];
				sizeImage=new Size(LayoutManager.Scale(imageList.ImageSize.Width),LayoutManager.Scale(imageList.ImageSize.Height));
			}
			if(button.Icon!=EnumIcons.None || bitmap!=null){//draw image and text
				if(!button.Enabled){
					rectangleImage=new Rectangle(button.Bounds.X+LayoutManager.Scale(3),button.Bounds.Y+LayoutManager.Scale(1),sizeImage.Width,sizeImage.Height);
					if(button.Icon!=EnumIcons.None){
						IconLibrary.DrawDisabled(g,button.Icon,rectangleImage);
					}
					else{
						Bitmap bitmapDisabled=new Bitmap(bitmap.Width,bitmap.Height);//disabled bitmap is old small size
						Graphics gfx=Graphics.FromImage(bitmapDisabled);
						ControlPaint.DrawImageDisabled(gfx,bitmap,0,0,SystemColors.Control);
						g.DrawImage(bitmapDisabled,rectangleImage);
						gfx.Dispose();
						bitmapDisabled.Dispose();//An alternative would be to build the image once, and store it as part of the button
					}
					rectangleText=new Rectangle(button.Bounds.X+sizeImage.Width+LayoutManager.Scale(3),button.Bounds.Y,
						textWidth-sizeImage.Width-LayoutManager.Scale(3),button.Bounds.Height);
				}
				else if(button.State==ToolBarButtonState.Pressed){//draw slightly down and right
					rectangleImage=new Rectangle(button.Bounds.X+LayoutManager.Scale(4),button.Bounds.Y+LayoutManager.Scale(2),sizeImage.Width,sizeImage.Height);
					if(button.Icon!=EnumIcons.None){
						IconLibrary.Draw(g,button.Icon,rectangleImage,colorGradientTop,colorGradientBottom);
					}
					else{
						g.DrawImage(bitmap,rectangleImage);
					}
					rectangleText=new Rectangle(button.Bounds.X+1+sizeImage.Width+LayoutManager.Scale(3),button.Bounds.Y+LayoutManager.Scale(1),
						textWidth-sizeImage.Width-LayoutManager.Scale(3),button.Bounds.Height);
				}
				else{
					rectangleImage=new Rectangle(button.Bounds.X+LayoutManager.Scale(3),button.Bounds.Y+LayoutManager.Scale(1),sizeImage.Width,sizeImage.Height);
					if(button.Icon!=EnumIcons.None){
						/*using Bitmap bitmapBack=new Bitmap(rectangleImage.Width,rectangleImage.Height);
						using Graphics gBack=Graphics.FromImage(bitmapBack);
						gBack.InterpolationMode=InterpolationMode.HighQualityBicubic;
						gBack.SmoothingMode=SmoothingMode.HighQuality;
						gBack.Clear(Color.Red);
						DrawButtonBackground(gBack,button,rectangleImage);
						IconLibrary.DrawOnBitmap(bitmapBack,button.Icon,new Rectangle(0,0,rectangleImage.Width,rectangleImage.Height));
						g.DrawImage(bitmapBack,rectangleImage);*/
						IconLibrary.Draw(g,button.Icon,rectangleImage,colorGradientTop,colorGradientBottom);
					}
					else{
						g.DrawImage(bitmap,rectangleImage);
					}
					rectangleText=new Rectangle(button.Bounds.X+sizeImage.Width+LayoutManager.Scale(3),button.Bounds.Y,
						textWidth-sizeImage.Width-LayoutManager.Scale(3),button.Bounds.Height);
				}
			}
			else{//only draw text
				if(button.Style==ODToolBarButtonStyle.Label || button.Style==ODToolBarButtonStyle.PageNav){
					rectangleText=new Rectangle(button.Bounds.X,button.Bounds.Y,
						textWidth,button.Bounds.Height);
				}
				else if(button.State==ToolBarButtonState.Pressed){//draw slightly down and right
					rectangleText=new Rectangle(button.Bounds.X+1,button.Bounds.Y+1,
						textWidth,button.Bounds.Height);
				}
				else{
					rectangleText=new Rectangle(button.Bounds.X,button.Bounds.Y,
						textWidth,button.Bounds.Height);
				}
			}
			if(imageList!=null && button.ImageIndex!=-1){//if there is an image
				if(button.Enabled) {
					g.DrawString(button.Text,Font,_brushText,rectangleText,_stringFormatLeft);
				}
				else {
					g.DrawString(button.Text,Font,_brushTextDisabled,rectangleText,_stringFormatLeft);
				}
			}
			else{
				if(button.Enabled) {
					g.DrawString(button.Text,Font,_brushText,rectangleText,_stringFormatCenter);
					//For page navigation buttons we ALWAYS show the text box so that users know they can type in the box to change pages.
					if(button.Style==ODToolBarButtonStyle.PageNav) {
						//Because we do not display zeros to users (e.g. 001 / 135) and instead leave them off (e.g. 1 / 135) the width will be lopsided
						//We need to draw each side of the page navigation (and pad a little) individually.
						SizeF size = g.MeasureString("/",Font);
						g.DrawString(button.PageValue.ToString(),Font,_brushText
							,new Rectangle(rectangleText.Location,new Size((rectangleText.Width/2)-(int)(size.Width/2)-2,rectangleText.Height))
							,new StringFormat() { Alignment=StringAlignment.Far,LineAlignment=StringAlignment.Center });
						g.DrawString(button.PageMax.ToString(),Font,_brushText
							,new Rectangle(new Point(rectangleText.X+(rectangleText.Width/2)+(int)(size.Width/2)+2,rectangleText.Y)
							,new Size((rectangleText.Width/2)-(int)(size.Width/2),rectangleText.Height))
							,new StringFormat() { Alignment=StringAlignment.Near,LineAlignment=StringAlignment.Center });
						//Only add the text box for page navigation once.  
						//However, we need to add the text box here in the paint otherwise we don't know how large to make the text box.
						if(textPageNav==null) {
							textPageNav=new ValidNum();
							textPageNav.Font=Font;
							textPageNav.Size=new Size(button.Bounds.Width/2-LayoutManager.Scale(10),button.Bounds.Height);
							//height gets ignored, but is used below
							textPageNav.KeyDown+=TextPageNav_KeyDown;
							textPageNav.MinVal=1;//There is no such thing as 0 pages in a preview, always set min to 1.
							textPageNav.TextAlign=HorizontalAlignment.Right;
							LayoutManager.Add(textPageNav,this);
						}
						int xCenter=button.Bounds.X+button.Bounds.Width/2;
						int yCenter=button.Bounds.Y+button.Bounds.Height/2;
						int wText=button.Bounds.Width/2-LayoutManager.Scale(10);
						LayoutManager.Move(textPageNav,new Rectangle(
							xCenter-wText-LayoutManager.Scale(4),yCenter-textPageNav.Height/2,wText,textPageNav.Height));
						//LayoutManager.MoveLocation(textPageNav,new Point(button.Bounds.X+1,button.Bounds.Y+2));
						//Nav text changes constantly so we need to update it when redrawing.
						textPageNav.Text=button.PageValue.ToString();
						if(button.PageMax!=0) {
							textPageNav.MaxVal=button.PageMax;
						}
					}
				}
				else {
					g.DrawString(button.Text,Font,_brushTextDisabled,rectangleText,_stringFormatCenter);
				}
			}
			//draw outline
			//Pen penR=penMedium;//new Pen(Color.FromArgb(180,180,180));
			if(!button.Enabled){
				//no ScaleF
				GraphicsHelper.DrawLine(g,_penDivider,button.Bounds.Right-1,button.Bounds.Top,button.Bounds.Right-1,button.Bounds.Bottom-1);//vertical line on the right side
			}
			else if(button.Style==ODToolBarButtonStyle.ToggleButton && button.Pushed){
				g.DrawRectangle(_penOutline,new Rectangle(button.Bounds.X,button.Bounds.Y
					,button.Bounds.Width-1,button.Bounds.Height-1));
				g.DrawRectangle(_penTogglePushed,new Rectangle(button.Bounds.X+1,button.Bounds.Y+1
					,button.Bounds.Width-3,button.Bounds.Height-3));
			}
			else if(button.Style==ODToolBarButtonStyle.Label || button.Style==ODToolBarButtonStyle.PageNav){
				//no outline
				GraphicsHelper.DrawLine(g,_penDivider,button.Bounds.Right-1,button.Bounds.Top,button.Bounds.Right-1,button.Bounds.Bottom-1);//vertical line on the right side
			}
			else switch(button.State){
				case ToolBarButtonState.Normal:
					//no outline
					GraphicsHelper.DrawLine(g,_penDivider,button.Bounds.Right-1,button.Bounds.Top,button.Bounds.Right-1,button.Bounds.Bottom-1);
					break;
				case ToolBarButtonState.Hover:
					g.DrawRectangle(_penOutline,button.Bounds.X-1,button.Bounds.Y,button.Bounds.Width-1,button.Bounds.Height-1);
					break;
				case ToolBarButtonState.Pressed:
					g.DrawRectangle(_penOutline,button.Bounds.X,button.Bounds.Y,button.Bounds.Width-1,button.Bounds.Height-1);
					break;
				case ToolBarButtonState.DropPressed:
					g.DrawRectangle(_penOutline,button.Bounds.X,button.Bounds.Y,button.Bounds.Width-1,button.Bounds.Height-1);
					break;
			}
			if(button.Style==ODToolBarButtonStyle.DropDownButton){
				int adjDown=0;//The distance to push the triangle down to show the notification text.
				if(isNotify) {
					adjDown=LayoutManager.Scale(6);
					//Draw the notification text.
					Size sizeText=TextRenderer.MeasureText(button.NotificationText,Font);
					g.DrawString(button.NotificationText,Font,(button.Enabled?_brushText:_brushTextDisabled),
						button.Bounds.X+button.Bounds.Width+1-(_widthDropDown+sizeText.Width)/2f,button.Bounds.Y+2+sizeText.Height/2f,_stringFormatLeft);
				}
				Point[] triangle=new Point[3];
				triangle[0]=new Point(button.Bounds.X+button.Bounds.Width-LayoutManager.Scale(11)
					,button.Bounds.Y+button.Bounds.Height/2-LayoutManager.Scale(2)+adjDown);
				triangle[1]=new Point(button.Bounds.X+button.Bounds.Width-LayoutManager.Scale(4)
					,button.Bounds.Y+button.Bounds.Height/2-LayoutManager.Scale(2)+adjDown);
				triangle[2]=new Point(button.Bounds.X+button.Bounds.Width-LayoutManager.Scale(8)
					,button.Bounds.Y+button.Bounds.Height/2+LayoutManager.Scale(2)+adjDown);
				if(button.Enabled) {
					g.FillPolygon(_brushText,triangle);
				}
				else {
					g.FillPolygon(_brushTextDisabled,triangle);
				}
				if(button.State!=ToolBarButtonState.Normal && button.Enabled){
					GraphicsHelper.DrawLine(g,_penOutline,button.Bounds.X+button.Bounds.Width-_widthDropDown,button.Bounds.Y
						,button.Bounds.X+button.Bounds.Width-_widthDropDown,button.Bounds.Y+button.Bounds.Height);
				}
			}
		}

		///<summary>Returns the button that contains these coordinates, or null if no hit.</summary>
		private ODToolBarButton HitTest(int x,int y){
			for(int i=0;i<buttons.Count;i++){
				if(buttons[i].Bounds.Contains(x,y)){
					return buttons[i];
				}
			}
			return null;
		}

		private bool HitTestDrop(ODToolBarButton button,int x,int y){
			Rectangle dropRect=new Rectangle(button.Bounds.X+button.Bounds.Width-_widthDropDown,button.Bounds.Y
				,_widthDropDown,button.Bounds.Height);
			if(dropRect.Contains(x,y)){
				return true;
			}
			return false;
		}

		private void InitializeBrushes(){
			_brushTogglePushed?.Dispose();
			_brushTogglePushError?.Dispose();
			_brushMain?.Dispose();
			_brushMainError?.Dispose();
			_brushPushed?.Dispose();
			_brushPushedError?.Dispose();
			_brushNotify?.Dispose();
			_brushTogglePushed=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,255,255),Color.FromArgb(191,201,229));
			_brushTogglePushError=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,212,212),Color.FromArgb(255,118,118));
			_brushMain=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,255,255),Color.FromArgb(171,181,209));
			_brushMainError=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,192,192),Color.FromArgb(255,98,98));
			_brushPushed=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(225,225,225),Color.FromArgb(141,151,179));
			_brushPushedError=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(225,162,162),Color.FromArgb(225,68,68));			
			_brushNotify=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,231,167),Color.FromArgb(252,178,129));		
			_lineThickness=1;
			if(LayoutManager.ScaleMy()>1.3f){
				_lineThickness=2;
			}
			_penDivider?.Dispose();
			_penOutline?.Dispose();
			_penTogglePushed?.Dispose();
			_penDivider=new Pen(Color.FromArgb(180,180,180),_lineThickness);
			_penOutline=new Pen(Color.SlateGray,_lineThickness);
			//_penOutline.Alignment=PenAlignment.Inset;
			_penTogglePushed=new Pen(Color.FromArgb(132,148,220),_lineThickness); //Compared to SlateGray(112,128,144)
		}
		#endregion Methods - Private

	}

	#region Other Classes
	///<summary></summary>
	public delegate void ODToolBarButtonClickEventHandler(object sender,ODToolBarButtonClickEventArgs e);
	///<summary></summary>
	public delegate void ODToolBarButtonPageNavEventHandler(object sender,ODToolBarButtonPageNavEventArgs e);

	///<summary></summary>
	public class ODToolBarButtonClickEventArgs{
		private ODToolBarButton button;
	
		public ODToolBarButtonClickEventArgs(ODToolBarButton myButton){
			button=myButton;
		}

		///<summary></summary>
		public ODToolBarButton Button{
			get{ 
				return button;
			}
		}
	}	
	
	///<summary></summary>
	public class ODToolBarButtonPageNavEventArgs{
		private int _navValue;
		public ODToolBarButtonPageNavEventArgs(int navValue){
			_navValue=navValue;
		}

		///<summary></summary>
		public int NavValue{
			get{ 
				return _navValue;
			}
		}
	}
	#endregion Other Classes
}

/*Boilerplate example:
		using OpenDental.UI;

		private void LayoutToolBar() {
			toolBarMain.Buttons.Clear();
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),0,"","Add"));
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),1,Lan.g(this,"Edit Selected Account"),"Edit"));
		}

		private void toolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Add":
					Add_Click();//or, put short block of code here
					break;
				case "Edit":
					Edit_Click();
					break;
			}
		}


*/