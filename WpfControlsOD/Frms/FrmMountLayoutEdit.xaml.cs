using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;
using OpenDental.Drawing;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmMountLayoutEdit : FrmODBase{
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
		private double _ratio;
		/// <summary>This is the entire area available for drawing.</summary>
		private Rect _rectBack;
		private int _reorderIndex;
		/// <summary>This is the outline of the mount.</summary>
		private Rect _rectMount;
		private int _selectedIndex=-1;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public FrmMountLayoutEdit()	{
			InitializeComponent();
			Load+=FrmMountLayoutEdit_Load;
			panelDrawing.PreviewKeyDown+=panelDrawing_KeyDown;
			panelDrawing.MouseDoubleClick+=panelDrawing_MouseDoubleClick;
			panelDrawing.PreviewMouseDown+=panelDrawing_MouseDown;
			panelDrawing.PreviewMouseMove+=panelDrawing_MouseMove;
			panelDrawing.PreviewMouseUp+=panelDrawing_MouseUp;
			panelDrawing.SizeChanged+=FormMountDefEdit_SizeChanged;
			textVIntHeight.KeyDown+=textVIntHeight_KeyDown;
			textVIntHeight.LostFocus+=TextVIntHeight_LostFocus;
			textVIntWidth.KeyDown+=textVIntWidth_KeyDown;
			textVIntWidth.LostFocus+=textVIntWidth_LostFocus;
			PreviewKeyDown+=FrmMountLayoutEdit_PreviewKeyDown;
		}

		//private void PanelDrawing_GotFocus(object sender,RoutedEventArgs e) {
			//throw new NotImplementedException();
		//}
		#endregion Constructor

		#region Methods - EventHandlers - Form
		private void FrmMountLayoutEdit_Load(object sender, System.EventArgs e) {
			Lang.F(this);
			CalcRatio();
			textVIntWidth.Text=MountCur.Width.ToString();
			textVIntHeight.Text=MountCur.Height.ToString();
			FillItems();
			ShowWarning();
			textVIntWidth.SelectAll();
		}

		private void FormMountDefEdit_SizeChanged(object sender, EventArgs e){
			//Invalidate();//it already does this
			CalcRatio();
		}

		private void FrmMountLayoutEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
			if(butDown.IsAltKey(Key.D,e)) {
				butDown_Click(this,new EventArgs());
			}
			if(butUp.IsAltKey(Key.U,e)) {
				butUp_Click(this,new EventArgs());
			}
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
			Draw();
		}

		private void butReorder_Click(object sender,EventArgs e) {
			if(_isReordering){
				_isReordering=false;
				butReorder.Text="Reorder All";
				labelReorder.Text="This lets you reorder all items by clicking through them in sequence";
				return;
			}
			//start reordering
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All orders will now be cleared.")){
				return;
			}
			_selectedIndex=-1;
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				_listMountItems[i].ItemOrder=0;
				MountItems.Update(_listMountItems[i]);
			}
			Draw();
			_isReordering=true;
			butReorder.Text="Stop";
			_reorderIndex=1;
			labelReorder.Text="Click the button again to stop reordering";
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
			Draw();
		}

		//private void controlDrawing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e){
		//	if(e.KeyCode==Keys.Left || e.KeyCode==Keys.Right || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
		//		e.IsInputKey=true;//don't jump to next control
		//	}
		//}
		private void panelDrawing_KeyDown(object sender,KeyEventArgs e) {
			if(_selectedIndex==-1){
				return;
			}
			//Each keypress hits the database, but it's a rare setup window and a light hit.
			if(e.Key==Key.Left){
				_listMountItems[_selectedIndex].Xpos--;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.Key==Key.Right){
				_listMountItems[_selectedIndex].Xpos++;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.Key==Key.Up){
				_listMountItems[_selectedIndex].Ypos--;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
			if(e.Key==Key.Down){
				_listMountItems[_selectedIndex].Ypos++;
				MountItems.Update(_listMountItems[_selectedIndex]);
				FillItems();
			}
		}

		private void textVIntHeight_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter){
				textVIntWidth.Focus();
				textVIntWidth.SelectionStart=textVIntWidth.Text.Length;
			}
		}

		private void textVIntWidth_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter){
				textVIntHeight.Focus();
				textVIntHeight.SelectionStart=textVIntHeight.Text.Length;
			}
		}

		private void TextVIntHeight_LostFocus(object sender,RoutedEventArgs e) {
			if(!textVIntHeight.IsValid()){
				return;
			}
			MountCur.Height=textVIntHeight.Value;//Because MountCur.Height is used for some layout logic, but don't save to db
			CalcRatio();
			ShowWarning();
			Draw();
		}

		private void textVIntWidth_LostFocus(object sender,RoutedEventArgs e) {
			if(!textVIntWidth.IsValid()){
				return;
			}
			MountCur.Width=textVIntWidth.Value;//Because MountCur.Width is used for some layout logic, but don't save to db
			CalcRatio();
			ShowWarning();
			Draw();
		}
		#endregion Methods - EventHandlers - Controls

		#region Methods - EventHandlers - Mouse
		private void panelDrawing_MouseDown(object sender, MouseEventArgs e){
			panelDrawing.CaptureMouse();
			if(_isReordering){
				int idxClicked=HitTest(e.GetPosition(panelDrawing));
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
				Draw();
				if(_reorderIndex==_listMountItems.Count+1){//example 5==5
					//automatically stops after last one, although user could stop in the middle manually.
					_isReordering=false;
					butReorder.Text="Reorder All";
					labelReorder.Text="This lets you reorder all items by clicking through them in sequence";
				}
				return;
			}
			_isMouseDown=true;
			_pointMouseDownOrig=e.GetPosition(panelDrawing);
			_selectedIndex=HitTest(e.GetPosition(panelDrawing));
			if(_selectedIndex!=-1){
				_pointItemOrig=new Point(
					(int)(_rectMount.X+_listMountItems[_selectedIndex].Xpos*_ratio),
					(int)(_rectMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio));					
			}
			Draw();
		}

		private void panelDrawing_MouseMove(object sender, MouseEventArgs e){
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
			Point point=e.GetPosition(panelDrawing);
			int x=(int)((point.X-_pointMouseDownOrig.X+_pointItemOrig.X-_rectMount.X)/_ratio);
			if(x<0){
				x=0;
			}
			if(x > MountCur.Width - _listMountItems[_selectedIndex].Width){
				x=MountCur.Width - _listMountItems[_selectedIndex].Width;
			}
			_listMountItems[_selectedIndex].Xpos=x;
			int y=(int)((point.Y-_pointMouseDownOrig.Y+_pointItemOrig.Y-_rectMount.Y)/_ratio);
			if(y<0){
				y=0;
			}
			if(y > MountCur.Height - _listMountItems[_selectedIndex].Height){
				y=MountCur.Height - _listMountItems[_selectedIndex].Height;
			}
			_listMountItems[_selectedIndex].Ypos=y;
			//but don't save to db yet.
			Draw();
		}

		private void panelDrawing_MouseUp(object sender, MouseButtonEventArgs e){
			panelDrawing.ReleaseMouseCapture();
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

		private void panelDrawing_MouseDoubleClick(object sender, MouseEventArgs e){
			if(_isReordering){
				return;
			}
			//mouse down will have set selected index
			if(_selectedIndex==-1){
				return;
			}
			_isMouseDown=false;
			FrmMountItemEdit frmMountItemEdit=new FrmMountItemEdit();
			frmMountItemEdit.MountItemCur=_listMountItems[_selectedIndex];
			frmMountItemEdit.IsLayout=true;
			frmMountItemEdit.ShowDialog();
			FillItems();
		}
		#endregion Methods - EventHandlers - Mouse	

		#region Methods - EventHandlers - DeleteOkCancel
		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textVIntWidth.IsValid() 
				|| !textVIntHeight.IsValid() ) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//these same numbers are used on FormMountItemEdit textboxes.
			//Memory issues are the basis of the mount size limit:
			//Limit size of mount to 20k x 10k pixels. =600 MB color
			//For comparison, 4K is 4000 x 2000
			//Good sensor is 1700x1300. FMX mount would then be 10,700 x 3,900 = 125 MB
			MountCur.Width=PIn.Int(textVIntWidth.Text);
			MountCur.Height=PIn.Int(textVIntHeight.Text);
			try{
				Mounts.Update(MountCur);//whether new or not
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			IsDialogOK=true;
		}

		#endregion Methods - EventHandlers - DeleteOkCancel

		#region Methods
		private void CalcRatio(){
			_rectBack=new Rect(0,0,panelDrawing.ActualWidth,panelDrawing.ActualHeight);
			double ratioWidth=_rectBack.Width/MountCur.Width;
			double ratioHeight=_rectBack.Height/MountCur.Height;
			_ratio=ratioWidth;
			_isWider=false;
			if(ratioHeight<ratioWidth){
				_isWider=true;
				_ratio=ratioHeight;
			}
			double xMain=0;
			if(_isWider){
				xMain=(_rectBack.Width-MountCur.Width*_ratio)/2;
			}
			double yMain=0;
			if(!_isWider){
				yMain=(_rectBack.Height-MountCur.Height*_ratio)/2;
			}
			_rectMount=new Rect(xMain,yMain,MountCur.Width*_ratio,MountCur.Height*_ratio);
		}

		private void Draw(){
			Graphics g=Graphics.ScreenInit(panelDrawing);
			g.FillRectangle(ColorOD.FromString("#FFFCFDFE"),_rectBack);
			g.DrawRectangle(Colors.Blue,_rectMount);
			Rect rectItem;
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
				rectItem=new Rect(
					_rectMount.X+_listMountItems[i].Xpos*_ratio,
					_rectMount.Y+_listMountItems[i].Ypos*_ratio,
					_listMountItems[i].Width*_ratio,
					_listMountItems[i].Height*_ratio);
				g.DrawRectangle(Colors.Blue,rectItem);
				if(_listMountItems[i].TextShowing.IsNullOrEmpty()){
					s="#"+_listMountItems[i].ItemOrder.ToString()
						+"\r\nPos: "+_listMountItems[i].Xpos.ToString()+", "+_listMountItems[i].Ypos.ToString()
						+"\r\nSize: "+_listMountItems[i].Width.ToString()+" x "+_listMountItems[i].Height.ToString()
						+"\r\nRot: ";
					s+=_listMountItems[i].RotateOnAcquire.ToString()
						+"\r\nTeeth: "+_listMountItems[i].ToothNumbers;
					Font font=new Font();
					point=new Point(
						(int)(_rectMount.X+_listMountItems[i].Xpos*_ratio+_listMountItems[i].Width*_ratio/2-g.MeasureString(s,font).Width/2),
						(int)(_rectMount.Y+_listMountItems[i].Ypos*_ratio+_listMountItems[i].Height*_ratio/2-g.MeasureString(s,font).Height/2));
					g.DrawString(s,font,Colors.DarkBlue,point.X,point.Y);
				}
				else{
					s=_listMountItems[i].TextShowing;
					point=new Point((int)(_rectMount.X+_listMountItems[i].Xpos*_ratio),(int)(_rectMount.Y+_listMountItems[i].Ypos*_ratio));
					Font font=new Font();
					font.Size=_listMountItems[i].FontSize*_ratio;
					g.DrawString(s,font,Colors.DarkBlue,point);
				}
			}
			//then, draw any selected item so it draws on top
			if(_selectedIndex==-1){
				return;
			}
			rectItem=new Rect(
				_rectMount.X+_listMountItems[_selectedIndex].Xpos*_ratio,
				_rectMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio,
				_listMountItems[_selectedIndex].Width*_ratio,
				_listMountItems[_selectedIndex].Height*_ratio);
			g.DrawRectangle(Colors.Red,rectItem);
			if(_listMountItems[_selectedIndex].TextShowing.IsNullOrEmpty()){
				s="#"+_listMountItems[_selectedIndex].ItemOrder.ToString()
					+"\r\nPos: "+_listMountItems[_selectedIndex].Xpos.ToString()+", "+_listMountItems[_selectedIndex].Ypos.ToString()
					+"\r\nSize: "+_listMountItems[_selectedIndex].Width.ToString()+" x "+_listMountItems[_selectedIndex].Height.ToString()
					+"\r\nRot: ";
				s+=_listMountItems[_selectedIndex].RotateOnAcquire.ToString()
					+"\r\nTeeth: "+_listMountItems[_selectedIndex].ToothNumbers;
				Font font=new Font();
				point=new Point(
					(int)(_rectMount.X+_listMountItems[_selectedIndex].Xpos*_ratio+_listMountItems[_selectedIndex].Width*_ratio/2-g.MeasureString(s,font).Width/2),
					(int)(_rectMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio+_listMountItems[_selectedIndex].Height*_ratio/2-g.MeasureString(s,font).Height/2));
				g.DrawString(s,font,Colors.DarkRed,point);
			}
			else{
				s=_listMountItems[_selectedIndex].TextShowing;
				point=new Point((int)(_rectMount.X+_listMountItems[_selectedIndex].Xpos*_ratio),(int)(_rectMount.Y+_listMountItems[_selectedIndex].Ypos*_ratio));
				Font font=new Font();
				font.Size=_listMountItems[_selectedIndex].FontSize*_ratio;
				g.DrawString(s,font,Colors.DarkRed,point);
			}
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
			//If the last item in the array was deleted, we need to update the selectedIndex variable
			if(_selectedIndex>=_listMountItems.Count) {
				_selectedIndex=_listMountItems.Count-1;
			}
			Draw();
		}

		///<summary>Returns the index of the item within the list, or -1.</summary>
		private int HitTest(Point point){
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				if(point.X<_rectMount.X+_listMountItems[i].Xpos*_ratio){
					continue;
				}
				if(point.Y<_rectMount.Y+_listMountItems[i].Ypos*_ratio){
					continue;
				}
				if(point.X>_rectMount.X+_listMountItems[i].Xpos*_ratio+_listMountItems[i].Width*_ratio){
					continue;
				}
				if(point.Y>_rectMount.Y+_listMountItems[i].Ypos*_ratio+_listMountItems[i].Height*_ratio){
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