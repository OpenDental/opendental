using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormMountLayoutEdit : FormODBase{
		#region Fields - Public
		public Mount MountCur;
		#endregion Fields - Public

		#region Fields - Private
		private bool _isMouseDown;
		private bool _isReordering;
		private bool _isWider;
		private List<MountItem> _listMountItems;
		/// <summary>The original point where the mouse was down.</summary>
		private Point _pointMouseDownOrig;
		/// <summary>If we are dragging, this is the original location of the item.</summary>
		private Point _pointItemOrig;
		/// <summary>To shrink or enlarge the mount to make it fit in the space available.</summary>
		private float _ratio;
		/// <summary>This is the entire area available for drawing.</summary>
		private Rectangle _rectangleBack;
		private int _reorderIndex;
		/// <summary>This is the outline of the mount.</summary>
		private Rectangle _rectangleMount;
		private int _selectedIndex=-1;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public FormMountLayoutEdit()	{
			//Required for Windows Form Designer support
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - EventHandlers - Form
		private void FormMountLayoutEdit_Load(object sender, System.EventArgs e) {
			CalcRatio();
			textWidth.Text=MountCur.Width.ToString();
			textHeight.Text=MountCur.Height.ToString();
			FillItems();
			ShowWarning();
		}

		private void FormMountDefEdit_SizeChanged(object sender, EventArgs e){
			//Invalidate();//it already does this
			CalcRatio();
		}
		#endregion Methods - EventHandlers - Form

		#region Methods - EventHandlers - Controls
		private void butAdd_Click(object sender, EventArgs e){
			MountItem mountItem=new MountItem();
			mountItem.IsNew=true;
			mountItem.MountNum=MountCur.MountNum;
			mountItem.Xpos=MountCur.Width/25;//slightly offset from any 0,0 item
			mountItem.Ypos=-MountCur.Height/25;//and up
			mountItem.Width=MountCur.Width/4;
			mountItem.Height=MountCur.Height/4;
			mountItem.ItemOrder=1;
			if(_listMountItems.Count>0){
				//use the last mountItem so that we avoid the text items
				mountItem.Width=_listMountItems[_listMountItems.Count-1].Width;
				mountItem.Height=_listMountItems[_listMountItems.Count-1].Height;
				mountItem.ItemOrder=_listMountItems.Count+1;
			}
			MountItems.Insert(mountItem);//don't bother showing the edit window?
			FillItems();
		}

		private void butDown_Click(object sender, EventArgs e){
			int selectedIdx=_selectedIndex;
			if(selectedIdx==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(selectedIdx==_listMountItems.Count-1) {//at bottom
				return;
			}
			MountItem mountItem=_listMountItems[selectedIdx];
			mountItem.ItemOrder++;
			MountItems.Update(mountItem);
			MountItem mountItemBelow=_listMountItems[selectedIdx+1];
			mountItemBelow.ItemOrder--;
			MountItems.Update(mountItemBelow);
			FillItems();
			_selectedIndex=selectedIdx+1;//visually, this is just the same item as before without moving
		}

		private void butReorder_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All orders will now be cleared.")){
				return;
			}
			if(_isReordering){
				_isReordering=false;
				butReorder.Text="Reorder All";
				labelReorder.Text="This lets you reorder all items by clicking through them in sequence";
			}
			else{//start reordering
				_selectedIndex=-1;
				for(int i=0;i<_listMountItems.Count;i++){
					if(_listMountItems[i].ItemOrder==-1){//unmounted
						continue;
					}
					_listMountItems[i].ItemOrder=0;
					MountItems.Update(_listMountItems[i]);
				}
				controlDrawing.Invalidate();
				_isReordering=true;
				butReorder.Text="Stop";
				_reorderIndex=1;
				labelReorder.Text="Click the button again to stop reordering";
			}
		}

		private void butUp_Click(object sender, EventArgs e){
			int selectedIdx=_selectedIndex;
			if(selectedIdx==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(selectedIdx==0) {//at top
				return;
			}
			MountItem mountItem=_listMountItems[selectedIdx];
			mountItem.ItemOrder--;
			MountItems.Update(mountItem);
			MountItem mountItemAbove=_listMountItems[selectedIdx-1];
			mountItemAbove.ItemOrder++;
			MountItems.Update(mountItemAbove);
			FillItems();
			_selectedIndex=selectedIdx-1;//visually, this is just the same item as before without moving
		}

		private void controlDrawing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e){
			if(e.KeyCode==Keys.Left || e.KeyCode==Keys.Right || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				e.IsInputKey=true;//don't jump to next control
			}
		}

		private void controlDrawing_KeyDown(object sender, KeyEventArgs e){
			if(_selectedIndex==-1){
				return;
			}
			//Each keypress hits the database, but it's a rare setup window and a light hit.
			if(e.KeyCode==Keys.Left){
				_listMountItems[_selectedIndex].Xpos--;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Right){
				_listMountItems[_selectedIndex].Xpos++;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Up){
				_listMountItems[_selectedIndex].Ypos--;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Down){
				_listMountItems[_selectedIndex].Ypos++;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
		}

		private void textHeight_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyData==Keys.Enter){
				textWidth.Focus();
				textWidth.SelectionStart=textWidth.Text.Length;
			}
		}

		private void textWidth_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyData==Keys.Enter){
				textHeight.Focus();
				textHeight.SelectionStart=textHeight.Text.Length;
			}
		}

		private void textHeight_Validated(object sender,EventArgs e) {
			if(!textHeight.IsValid()){
				return;
			}
			MountCur.Height=textHeight.Value;//Because MountCur.Height is used for some layout logic, but don't save to db
			CalcRatio();
			ShowWarning();
			controlDrawing.Invalidate();
		}

		private void textWidth_Validated(object sender,EventArgs e) {
			if(!textWidth.IsValid()){
				return;
			}
			MountCur.Width=textWidth.Value;//Because MountCur.Width is used for some layout logic, but don't save to db
			CalcRatio();
			ShowWarning();
			controlDrawing.Invalidate();
		}
		#endregion Methods - EventHandlers - Controls

		#region Methods - EventHandlers - Paint
		private void controlDrawing_Paint(object sender, PaintEventArgs e){
			Graphics g=e.Graphics;//alias
			g.FillRectangle(SystemBrushes.Control,_rectangleBack);
			g.DrawRectangle(Pens.Blue,_rectangleMount);
			RectangleF rectItem;
			Point point;
			string s;
			//first, draw the non-selected items
			if(_listMountItems==null){
				return;
			}
			for(int i=0;i<_listMountItems.Count;i++){
				if(_selectedIndex==i){
					continue;
				}
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				rectItem=new RectangleF(
					_rectangleMount.X+_listMountItems[i].Xpos*_ratio,
					_rectangleMount.Y+_listMountItems[i].Ypos*_ratio,
					_listMountItems[i].Width*_ratio,
					_listMountItems[i].Height*_ratio);
				g.DrawRectangle(Pens.Blue,Rectangle.Round(rectItem));
				if(_listMountItems[i].TextShowing.IsNullOrEmpty()){
					s="#"+_listMountItems[i].ItemOrder.ToString()
						+"\r\nPos: "+_listMountItems[i].Xpos.ToString()+", "+_listMountItems[i].Ypos.ToString()
						+"\r\nSize: "+_listMountItems[i].Width.ToString()+" x "+_listMountItems[i].Height.ToString()
						+"\r\nRot: ";
					s+=_listMountItems[i].RotateOnAcquire.ToString()
						+"\r\nTeeth: "+_listMountItems[i].ToothNumbers;
					point=new Point(
						(int)(_rectangleMount.X+_listMountItems[i].Xpos*_ratio+_listMountItems[i].Width*_ratio/2-g.MeasureString(s,Font).Width/2),
						(int)(_rectangleMount.Y+_listMountItems[i].Ypos*_ratio+_listMountItems[i].Height*_ratio/2-g.MeasureString(s,Font).Height/2));
					g.DrawString(s,Font,Brushes.DarkBlue,point);
				}
				else{
					s=_listMountItems[i].TextShowing;
					point=new Point((int)(_rectangleMount.X+_listMountItems[i].Xpos*_ratio),(int)(_rectangleMount.Y+_listMountItems[i].Ypos*_ratio));
					using Font font=new Font(Font.FontFamily,_listMountItems[i].FontSize*_ratio);
					g.DrawString(s,font,Brushes.DarkBlue,point);
				}
			}
			//then, draw any selected item so it draws on top
			if(_selectedIndex==-1){
				return;
			}
			rectItem=new RectangleF(
				_rectangleMount.X+_listMountItems[_selectedIndex].Xpos*_ratio,
				_rectangleMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio,
				_listMountItems[_selectedIndex].Width*_ratio,
				_listMountItems[_selectedIndex].Height*_ratio);
			g.DrawRectangle(Pens.Red,Rectangle.Round(rectItem));
			if(_listMountItems[_selectedIndex].TextShowing.IsNullOrEmpty()){
				s="#"+_listMountItems[_selectedIndex].ItemOrder.ToString()
					+"\r\nPos: "+_listMountItems[_selectedIndex].Xpos.ToString()+", "+_listMountItems[_selectedIndex].Ypos.ToString()
					+"\r\nSize: "+_listMountItems[_selectedIndex].Width.ToString()+" x "+_listMountItems[_selectedIndex].Height.ToString()
					+"\r\nRot: ";
				s+=_listMountItems[_selectedIndex].RotateOnAcquire.ToString()
					+"\r\nTeeth: "+_listMountItems[_selectedIndex].ToothNumbers;
				point=new Point(
					(int)(_rectangleMount.X+_listMountItems[_selectedIndex].Xpos*_ratio+_listMountItems[_selectedIndex].Width*_ratio/2-g.MeasureString(s,Font).Width/2),
					(int)(_rectangleMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio+_listMountItems[_selectedIndex].Height*_ratio/2-g.MeasureString(s,Font).Height/2));
				g.DrawString(s,Font,Brushes.DarkRed,point);
			}
			else{
				s=_listMountItems[_selectedIndex].TextShowing;
				point=new Point((int)(_rectangleMount.X+_listMountItems[_selectedIndex].Xpos*_ratio),(int)(_rectangleMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio));
				using Font font=new Font(Font.FontFamily,_listMountItems[_selectedIndex].FontSize*_ratio);
				g.DrawString(s,font,Brushes.DarkRed,point);
			}
		}
		#endregion Methods - EventHandlers - Paint

		#region Methods - EventHandlers - Mouse
		private void controlDrawing_MouseDown(object sender, MouseEventArgs e){
			if(_isReordering){
				int idxClicked=HitTest(e.Location);
				if(idxClicked==-1){
					return;
				}
				if(_listMountItems[idxClicked].ItemOrder!=0){
					return;
				}
				if(_listMountItems[idxClicked].TextShowing!=""){
					return;
				}
				_listMountItems[idxClicked].ItemOrder=_reorderIndex;
				MountItems.Update(_listMountItems[idxClicked]);
				_reorderIndex++;
				controlDrawing.Invalidate();
				if(_reorderIndex==_listMountItems.Count+1){//example 5==5
					//automatically stops after last one, although user could stop in the middle manually.
					_isReordering=false;
					butReorder.Text="Reorder All";
					labelReorder.Text="This lets you reorder all items by clicking through them in sequence";
				}
				return;
			}
			_isMouseDown=true;
			_pointMouseDownOrig=e.Location;
			_selectedIndex=HitTest(e.Location);
			if(_selectedIndex!=-1){
				_pointItemOrig=new Point(
					(int)(_rectangleMount.X+_listMountItems[_selectedIndex].Xpos*_ratio),
					(int)(_rectangleMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio));					
			}
			controlDrawing.Invalidate();
		}

		private void controlDrawing_MouseMove(object sender, MouseEventArgs e){
			if(_isReordering){
				return;
			}
			if(!_isMouseDown){
				return;
			}
			if(_selectedIndex==-1){
				return;
			}
			//we are dragging
			int x=(int)((e.X-_pointMouseDownOrig.X+_pointItemOrig.X-_rectangleMount.X)/_ratio);
			if(x<0){
				x=0;
			}
			if(x > MountCur.Width - _listMountItems[_selectedIndex].Width){
				x=MountCur.Width - _listMountItems[_selectedIndex].Width;
			}
			_listMountItems[_selectedIndex].Xpos=x;
			int y=(int)((e.Y-_pointMouseDownOrig.Y+_pointItemOrig.Y-_rectangleMount.Y)/_ratio);
			if(y<0){
				y=0;
			}
			if(y > MountCur.Height - _listMountItems[_selectedIndex].Height){
				y=MountCur.Height - _listMountItems[_selectedIndex].Height;
			}
			_listMountItems[_selectedIndex].Ypos=y;
			//but don't save to db yet.
			controlDrawing.Invalidate();
		}

		private void controlDrawing_MouseUp(object sender, MouseEventArgs e){
			if(_isReordering){
				return;
			}
			if(_isMouseDown && _selectedIndex!=-1){
				//save any movement
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
				//_selectedIndex remains same
			}
			_isMouseDown=false;
		}

		private void controlDrawing_MouseDoubleClick(object sender, MouseEventArgs e){
			if(_isReordering){
				return;
			}
			//mouse down will have set selected index
			if(_selectedIndex==-1){
				return;
			}
			using FormMountItemEdit formMountItemEdit=new FormMountItemEdit();
			formMountItemEdit.MountItemCur=_listMountItems[_selectedIndex];
			formMountItemEdit.IsLayout=true;
			formMountItemEdit.ShowDialog();
			FillItems();
			//If the last item in the array was deleted, we need to update the selectedIndex variable
			if(formMountItemEdit.DialogResult==DialogResult.OK && _selectedIndex>=_listMountItems.Count) {
				_selectedIndex=_listMountItems.Count-1;
			}
		}
		#endregion Methods - EventHandlers - Mouse	

		#region Methods - EventHandlers - DeleteOkCancel
		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textWidth.IsValid() 
				|| !textHeight.IsValid() ) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//these same numbers are used on FormMountItemEdit textboxes.
			//Memory issues are the basis of the mount size limit:
			//Limit size of mount to 20k x 10k pixels. =600 MB color
			//For comparison, 4K is 4000 x 2000
			//Good sensor is 1700x1300. FMX mount would then be 10,700 x 3,900 = 125 MB
			MountCur.Width=PIn.Int(textWidth.Text);
			MountCur.Height=PIn.Int(textHeight.Text);
			try{
				Mounts.Update(MountCur);//whether new or not
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


		#endregion Methods - EventHandlers - DeleteOkCancel

		#region Methods
		private void CalcRatio(){
			_rectangleBack=new Rectangle(0,0,panelSplitter.Left-1,ClientRectangle.Height-1);
			float ratioWidth=(float)_rectangleBack.Width/MountCur.Width;
			float ratioHeight=(float)_rectangleBack.Height/MountCur.Height;
			_ratio=ratioWidth;
			_isWider=false;
			if(ratioHeight<ratioWidth){
				_isWider=true;
				_ratio=ratioHeight;
			}
			float xMain=0;
			if(_isWider){
				xMain=(_rectangleBack.Width-MountCur.Width*_ratio)/2;
			}
			float yMain=0;
			if(!_isWider){
				yMain=(_rectangleBack.Height-MountCur.Height*_ratio)/2;
			}
			_rectangleMount=new Rectangle((int)xMain,(int)yMain,(int)(MountCur.Width*_ratio),(int)(MountCur.Height*_ratio));
		}

		private void FillItems(){
			_listMountItems=MountItems.GetItemsForMount(MountCur.MountNum);
			int idx=1;
			//Fix orders. They get messed up quite a bit, like when a user deletes an item.
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].TextShowing!=""){
					if(_listMountItems[i].ItemOrder!=0){
						_listMountItems[i].ItemOrder=0;
						MountItems.Update(_listMountItems[i]);
					}
					continue;
				}
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				if(_listMountItems[i].ItemOrder!=idx){
					_listMountItems[i].ItemOrder=idx;
					MountItems.Update(_listMountItems[i]);
				}
				idx++;
				//string s="#"+_listMountItemDefs[i].ItemOrder.ToString()+": X:"+_listMountItemDefs[i].Xpos.ToString()+", Y:"+_listMountItemDefs[i].Ypos.ToString()
				//	+": W:"+_listMountItemDefs[i].Width.ToString()+", H:"+_listMountItemDefs[i].Height.ToString();
				//listBoxItems.Items.Add(s);
			}
			controlDrawing.Invalidate();
		}

		///<summary>Returns the index of the item within the list, or -1.</summary>
		private int HitTest(Point point){
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				if(point.X<_rectangleMount.X+_listMountItems[i].Xpos*_ratio){
					continue;
				}
				if(point.Y<_rectangleMount.Y+_listMountItems[i].Ypos*_ratio){
					continue;
				}
				if(point.X>_rectangleMount.X+_listMountItems[i].Xpos*_ratio+_listMountItems[i].Width*_ratio){
					continue;
				}
				if(point.Y>_rectangleMount.Y+_listMountItems[i].Ypos*_ratio+_listMountItems[i].Height*_ratio){
					continue;
				}
				return i;
			}
			return -1;
		}

		private void ShowWarning(){
			labelWarning.Visible=false;
			if(_listMountItems==null){
				return;
			}
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				if(_listMountItems[i].Xpos>MountCur.Width-2
					|| _listMountItems[i].Ypos>MountCur.Height-2)
				{
					labelWarning.Visible=true;
				}
			}
		}









		/*
		///<summary>Returns null if no item.  Pass in raw point as obtained from mouse.</summary>
		private MountItemDef HitTest(Point point){

			return null;
		}*/
		#endregion Methods

		
	}
}





















