using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormMountDefEdit : FormODBase{
		#region Fields - Public
		public MountDef MountDefCur;
		#endregion Fields - Public

		#region Fields - Private
		private bool _isMouseDown;
		private bool _isReordering;
		private bool _isWider;
		private List<MountItemDef> _listMountItemDefs;
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
		///<summary>Within the list _listMountItemDefs. Completely unrelated to ItemOrders.</summary>
		private int _selectedIndex=-1;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public FormMountDefEdit()	{
			//Required for Windows Form Designer support
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - EventHandlers - Form
		private void FormMountDefEdit_FormClosing(object sender, FormClosingEventArgs e){
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(MountDefCur.IsNew){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete mount?")){
					e.Cancel=true;
					return;
				}
				MountItemDefs.DeleteForMount(MountDefCur.MountDefNum);
				MountDefs.Delete(MountDefCur.MountDefNum);
			}
		}

		private void FormMountDefEdit_Load(object sender, System.EventArgs e) {
			CalcRatio();
			textDescription.Text=MountDefCur.Description;
			comboDefaultCat.Items.AddDefNone();
			comboDefaultCat.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ImageCats,isShort:true));
			comboDefaultCat.SetSelectedDefNum(MountDefCur.DefaultCat); 
			textWidth.Text=MountDefCur.Width.ToString();
			textHeight.Text=MountDefCur.Height.ToString();
			butColorBack.BackColor=MountDefCur.ColorBack;
			butColorFore.BackColor=MountDefCur.ColorFore;
			butColorTextBack.BackColor=MountDefCur.ColorTextBack;
			if(MountDefCur.ColorTextBack.ToArgb()==Color.Transparent.ToArgb()){
				checkTransparent.Checked=true;
				butColorTextBack.BackColor=MountDefCur.ColorBack;
			}
			checkFlipOnAcquire.Checked=MountDefCur.FlipOnAcquire;
			checkAdjModeAfterSeries.Checked=MountDefCur.AdjModeAfterSeries;
			textUnits.Text=MountDefs.GetScaleUnits(MountDefCur.ScaleValue);
			textScale.Value=MountDefs.GetScale(MountDefCur.ScaleValue);
			textDecimals.Value=MountDefs.GetDecimals(MountDefCur.ScaleValue);
			FillItems();
			ShowWarning();
			if(!MountDefCur.IsNew){
				return;
			}
			using FormMountDefGenerate formMountDefGenerate=new FormMountDefGenerate();
			formMountDefGenerate.MountDefCur=MountDefCur;
			formMountDefGenerate.ShowDialog();
			if(formMountDefGenerate.DialogResult!=DialogResult.OK){
				MountDefs.Delete(MountDefCur.MountDefNum);
				DialogResult=DialogResult.OK;//to avoid triggering the msgbox in FormClosing
				return;
			}
			//Show the newly generated mountDef
			CalcRatio();
			textDescription.Text=MountDefCur.Description;
			comboDefaultCat.SetSelectedDefNum(MountDefCur.DefaultCat); 
			textWidth.Text=MountDefCur.Width.ToString();
			textHeight.Text=MountDefCur.Height.ToString();
			butColorBack.BackColor=MountDefCur.ColorBack;
			butColorFore.BackColor=MountDefCur.ColorFore;
			butColorTextBack.BackColor=MountDefCur.ColorTextBack;
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
			MountItemDef mountItemDef=new MountItemDef();
			mountItemDef.IsNew=true;
			mountItemDef.MountDefNum=MountDefCur.MountDefNum;
			mountItemDef.Xpos=MountDefCur.Width/25;//slightly offset from any 0,0 item
			mountItemDef.Ypos=-MountDefCur.Height/25;//and up
			mountItemDef.Width=MountDefCur.Width/4;
			mountItemDef.Height=MountDefCur.Height/4;
			mountItemDef.ItemOrder=1;
			if(_listMountItemDefs.Count>0){
				//use the last mountItem so that we avoid the text items
				mountItemDef.Width=_listMountItemDefs[_listMountItemDefs.Count-1].Width;
				mountItemDef.Height=_listMountItemDefs[_listMountItemDefs.Count-1].Height;
				mountItemDef.ItemOrder=_listMountItemDefs.Count+1;
			}
			MountItemDefs.Insert(mountItemDef);//don't bother showing the edit window?
			FillItems();
		}

		private void butColorBack_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorBack.BackColor;
			colorDialog.ShowDialog();
			butColorBack.BackColor=colorDialog.Color;
		}

		private void butColorFore_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorFore.BackColor;
			colorDialog.ShowDialog();
			butColorFore.BackColor=colorDialog.Color;
		}

		private void butColorTextBack_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorTextBack.BackColor;
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				//if Transparent was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorTextBack.BackColor=colorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				butColorTextBack.BackColor=butColorBack.BackColor;//interpreted by user as transparent
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorTextBack.BackColor=butColorBack.BackColor;
			}
		}

		private void butGenerate_Click(object sender, EventArgs e){
			using FormMountDefGenerate formMountDefGenerate=new FormMountDefGenerate();
			formMountDefGenerate.MountDefCur=MountDefCur;
			formMountDefGenerate.ShowDialog();
			if(formMountDefGenerate.DialogResult!=DialogResult.OK){
				return;
			}
			CalcRatio();
			textWidth.Text=MountDefCur.Width.ToString();
			textHeight.Text=MountDefCur.Height.ToString();
			butColorBack.BackColor=MountDefCur.ColorBack;
			FillItems();
			ShowWarning();
		}

		private void butDown_Click(object sender, EventArgs e){
			int selectedIdx=_selectedIndex;
			if(selectedIdx==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(selectedIdx==_listMountItemDefs.Count-1) {//at bottom
				return;
			}
			MountItemDef mountItemDef=_listMountItemDefs[selectedIdx];
			mountItemDef.ItemOrder++;
			MountItemDefs.Update(mountItemDef);
			MountItemDef mountItemDefBelow=_listMountItemDefs[selectedIdx+1];
			mountItemDefBelow.ItemOrder--;
			MountItemDefs.Update(mountItemDefBelow);
			FillItems();
			_selectedIndex=selectedIdx+1;//visually, this is just the same item as before without moving
		}

		private void butReorder_Click(object sender,EventArgs e) {
			if(_isReordering){
				_isReordering=false;
				FillItems();
				_selectedIndex=-1;
				butReorder.Text="Reorder All";
				labelReorder.Text="This lets you reorder all items by clicking through them in sequence";
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All orders will now be cleared.")){
				return;
			}
			_selectedIndex=-1;
			for(int i=0;i<_listMountItemDefs.Count;i++){
				_listMountItemDefs[i].ItemOrder=0;
				MountItemDefs.Update(_listMountItemDefs[i]);
			}
			controlDrawing.Invalidate();
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
			MountItemDef mountItemDef=_listMountItemDefs[selectedIdx];
			mountItemDef.ItemOrder--;
			MountItemDefs.Update(mountItemDef);
			MountItemDef mountItemDefAbove=_listMountItemDefs[selectedIdx-1];
			mountItemDefAbove.ItemOrder++;
			MountItemDefs.Update(mountItemDefAbove);
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
				_listMountItemDefs[_selectedIndex].Xpos--;
				MountItemDefs.Update(_listMountItemDefs[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Right){
				_listMountItemDefs[_selectedIndex].Xpos++;
				MountItemDefs.Update(_listMountItemDefs[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Up){
				_listMountItemDefs[_selectedIndex].Ypos--;
				MountItemDefs.Update(_listMountItemDefs[_selectedIndex]);
				FillItems();
			}
			if(e.KeyCode==Keys.Down){
				_listMountItemDefs[_selectedIndex].Ypos++;
				MountItemDefs.Update(_listMountItemDefs[_selectedIndex]);
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
			MountDefCur.Height=textHeight.Value;//Because MountDefCur.Width is used for some layout logic, but don't save to db
			CalcRatio();
			ShowWarning();
			controlDrawing.Invalidate();
		}

		private void textWidth_Validated(object sender,EventArgs e) {
			if(!textWidth.IsValid()){
				return;
			}
			MountDefCur.Width=textWidth.Value;//Because MountDefCur.Width is used for some layout logic, but don't save to db
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
			RectangleF rectangleFItem;
			Point point;
			string s;
			//first, draw the non-selected items
			if(_listMountItemDefs.IsNullOrEmpty()){
				return;
			}
			for(int i=0;i<_listMountItemDefs.Count;i++){
				if(_selectedIndex==i){
					continue;
				}
				rectangleFItem=new RectangleF(
					_rectangleMount.X+_listMountItemDefs[i].Xpos*_ratio,
					_rectangleMount.Y+_listMountItemDefs[i].Ypos*_ratio,
					_listMountItemDefs[i].Width*_ratio,
					_listMountItemDefs[i].Height*_ratio);
				g.DrawRectangle(Pens.Blue,Rectangle.Round(rectangleFItem));
				if(_listMountItemDefs[i].TextShowing.IsNullOrEmpty()){
					using Font font2=new Font(Font.FontFamily,LayoutManager.ScaleFontODZoom(8.25f));
					s="#"+_listMountItemDefs[i].ItemOrder.ToString()
						+"\r\nPos: "+_listMountItemDefs[i].Xpos.ToString()+", "+_listMountItemDefs[i].Ypos.ToString()
						+"\r\nSize: "+_listMountItemDefs[i].Width.ToString()+" x "+_listMountItemDefs[i].Height.ToString()
						+"\r\nRot: ";
					s+=_listMountItemDefs[i].RotateOnAcquire.ToString()
						+"\r\nTeeth: "+_listMountItemDefs[i].ToothNumbers;
					point=new Point(
						(int)(_rectangleMount.X+_listMountItemDefs[i].Xpos*_ratio+_listMountItemDefs[i].Width*_ratio/2-g.MeasureString(s,font2).Width/2),
						(int)(_rectangleMount.Y+_listMountItemDefs[i].Ypos*_ratio+_listMountItemDefs[i].Height*_ratio/2-g.MeasureString(s,font2).Height/2));
					g.DrawString(s,font2,Brushes.DarkBlue,point);
				}
				else{
					s=_listMountItemDefs[i].TextShowing;
					point=new Point((int)(_rectangleMount.X+_listMountItemDefs[i].Xpos*_ratio),(int)(_rectangleMount.Y+_listMountItemDefs[i].Ypos*_ratio));
					using Font font=new Font(Font.FontFamily,LayoutManager.UnscaleMS(_listMountItemDefs[i].FontSize)*_ratio);
					g.DrawString(s,font,Brushes.DarkBlue,point);
				}
			}
			//then, draw any selected item so it draws on top
			if(_selectedIndex==-1){
				return;
			}
			rectangleFItem=new RectangleF(
				_rectangleMount.X+_listMountItemDefs[_selectedIndex].Xpos*_ratio,
				_rectangleMount.Y+_listMountItemDefs[_selectedIndex].Ypos*_ratio,
				_listMountItemDefs[_selectedIndex].Width*_ratio,
				_listMountItemDefs[_selectedIndex].Height*_ratio);
			g.DrawRectangle(Pens.Red,Rectangle.Round(rectangleFItem));
			if(_listMountItemDefs[_selectedIndex].TextShowing.IsNullOrEmpty()){
				s="#"+_listMountItemDefs[_selectedIndex].ItemOrder.ToString()
					+"\r\nPos: "+_listMountItemDefs[_selectedIndex].Xpos.ToString()+", "+_listMountItemDefs[_selectedIndex].Ypos.ToString()
					+"\r\nSize: "+_listMountItemDefs[_selectedIndex].Width.ToString()+" x "+_listMountItemDefs[_selectedIndex].Height.ToString()
					+"\r\nRot: ";
				s+=_listMountItemDefs[_selectedIndex].RotateOnAcquire.ToString()
					+"\r\nTeeth: "+_listMountItemDefs[_selectedIndex].ToothNumbers;
				point=new Point(
					(int)(_rectangleMount.X+_listMountItemDefs[_selectedIndex].Xpos*_ratio+_listMountItemDefs[_selectedIndex].Width*_ratio/2-g.MeasureString(s,Font).Width/2),
					(int)(_rectangleMount.Y+_listMountItemDefs[_selectedIndex].Ypos*_ratio+_listMountItemDefs[_selectedIndex].Height*_ratio/2-g.MeasureString(s,Font).Height/2));
				g.DrawString(s,Font,Brushes.DarkRed,point);
			}
			else{
				s=_listMountItemDefs[_selectedIndex].TextShowing;
				point=new Point((int)(_rectangleMount.X+_listMountItemDefs[_selectedIndex].Xpos*_ratio),(int)(_rectangleMount.Y+_listMountItemDefs[_selectedIndex].Ypos*_ratio));
				using Font font=new Font(Font.FontFamily,_listMountItemDefs[_selectedIndex].FontSize*_ratio);
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
				if(_listMountItemDefs[idxClicked].ItemOrder!=0){
					return;
				}
				if(_listMountItemDefs[idxClicked].TextShowing!=""){
					return;
				}
				_listMountItemDefs[idxClicked].ItemOrder=_reorderIndex;
				MountItemDefs.Update(_listMountItemDefs[idxClicked]);
				_reorderIndex++;
				controlDrawing.Invalidate();
				int countNumbered=_listMountItemDefs.Count(x=>x.TextShowing=="");
				if(_reorderIndex==countNumbered+1){//example 5==5
					//automatically stops after last one, although user could stop in the middle manually.
					_isReordering=false;
					FillItems();
					_selectedIndex=-1;
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
					(int)(_rectangleMount.X+_listMountItemDefs[_selectedIndex].Xpos*_ratio),
					(int)(_rectangleMount.Y+_listMountItemDefs[_selectedIndex].Ypos*_ratio));					
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
			if(x > MountDefCur.Width - _listMountItemDefs[_selectedIndex].Width){
				x=MountDefCur.Width - _listMountItemDefs[_selectedIndex].Width;
			}
			_listMountItemDefs[_selectedIndex].Xpos=x;
			int y=(int)((e.Y-_pointMouseDownOrig.Y+_pointItemOrig.Y-_rectangleMount.Y)/_ratio);
			if(y<0){
				y=0;
			}
			if(y > MountDefCur.Height - _listMountItemDefs[_selectedIndex].Height){
				y=MountDefCur.Height - _listMountItemDefs[_selectedIndex].Height;
			}
			_listMountItemDefs[_selectedIndex].Ypos=y;
			//but don't save to db yet.
			controlDrawing.Invalidate();
		}

		private void controlDrawing_MouseUp(object sender, MouseEventArgs e){
			if(_isReordering){
				return;
			}
			if(_isMouseDown && _selectedIndex!=-1){
				//save any movement
				MountItemDefs.Update(_listMountItemDefs[_selectedIndex]);//this technically saves even on simple clicks, but that's harmless.
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
			using FormMountItemDefEdit formMountItemDefEdit=new FormMountItemDefEdit();
			formMountItemDefEdit.MountItemDefCur=_listMountItemDefs[_selectedIndex];
			formMountItemDefEdit.ShowDialog();
			FillItems();
			//If the last item in the array was deleted, we need to update the selectedIndex variable
			if(formMountItemDefEdit.DialogResult==DialogResult.OK && _selectedIndex>=_listMountItemDefs.Count) {
				_selectedIndex=_listMountItemDefs.Count-1;
			}
		}
		#endregion Methods - EventHandlers - Mouse	

		#region Methods - EventHandlers - DeleteOkCancel
		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MountDefCur.IsNew){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete mount?")){
					return;
				}
			}
			MountItemDefs.DeleteForMount(MountDefCur.MountDefNum);
			MountDefs.Delete(MountDefCur.MountDefNum);
			if(MountDefCur.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MessageBox.Show(Lan.g(this,"Description cannot be blank."));
				return;
			}
			if(!textWidth.IsValid() 
				|| !textHeight.IsValid() 
				|| !textScale.IsValid() 
				|| !textDecimals.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//these same numbers are used on FormMountItemDefEdit textboxes.
			//Memory issues are the basis of the mount size limit:
			//Limit size of mount to 20k x 10k pixels. =600 MB color
			//For comparison, 4K is 4000 x 2000
			//Good sensor is 1700x1300. FMX mount would then be 10,700 x 3,900 = 125 MB
			MountDefCur.Description=textDescription.Text;
			MountDefCur.DefaultCat=comboDefaultCat.GetSelectedDefNum();
			Def def=Defs.GetDef(DefCat.ImageCats, MountDefCur.DefaultCat);
			if(def!=null && def.IsHidden) {
				MsgBox.Show(Lan.g(this,"Warning: This Mount's default image category is hidden. Mounts with this category will be hidden."));
			}
			MountDefCur.Width=PIn.Int(textWidth.Text);
			MountDefCur.Height=PIn.Int(textHeight.Text);
			MountDefCur.ColorBack=butColorBack.BackColor;
			MountDefCur.ColorFore=butColorFore.BackColor;
			if(checkTransparent.Checked){
				MountDefCur.ColorTextBack=Color.Transparent;
			}
			else{
				MountDefCur.ColorTextBack=butColorTextBack.BackColor;
			}
			MountDefCur.FlipOnAcquire=checkFlipOnAcquire.Checked;
			MountDefCur.AdjModeAfterSeries=checkAdjModeAfterSeries.Checked;
			MountDefCur.ScaleValue=MountDefs.SetScale((float)textScale.Value,textDecimals.Value,textUnits.Text);
			//int intBlack=System.Drawing.Color.Black.ToArgb();
			//int intTransparent=System.Drawing.Color.Transparent.ToArgb();
			//Color color=Color.FromArgb(0);
			Color color=System.Drawing.Color.Transparent;
			try{
				MountDefs.Update(MountDefCur);//whether new or not
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
			_rectangleBack=new Rectangle(0,0,panelRight.Left-1,ClientRectangle.Height-1);
			float ratioWidth=(float)_rectangleBack.Width/MountDefCur.Width;
			float ratioHeight=(float)_rectangleBack.Height/MountDefCur.Height;
			_ratio=ratioWidth;
			_isWider=false;
			if(ratioHeight<ratioWidth){
				_isWider=true;
				_ratio=ratioHeight;
			}
			float xMain=0;
			if(_isWider){
				xMain=(_rectangleBack.Width-MountDefCur.Width*_ratio)/2;
			}
			float yMain=0;
			if(!_isWider){
				yMain=(_rectangleBack.Height-MountDefCur.Height*_ratio)/2;
			}
			_rectangleMount=new Rectangle((int)xMain,(int)yMain,(int)(MountDefCur.Width*_ratio),(int)(MountDefCur.Height*_ratio));
		}

		private void FillItems(){
			_listMountItemDefs=MountItemDefs.GetForMountDef(MountDefCur.MountDefNum);
			int idx=1;//1-based
			bool isChanged=false;
			//Fix ItemOrders. They get messed up quite a bit, like when a user deletes an item.
			for(int i=0;i<_listMountItemDefs.Count;i++){
				if(_listMountItemDefs[i].TextShowing!=""){
					if(_listMountItemDefs[i].ItemOrder!=0){
						_listMountItemDefs[i].ItemOrder=0;
						isChanged=true;
						MountItemDefs.Update(_listMountItemDefs[i]);
					}
					continue;
				}
				if(_listMountItemDefs[i].ItemOrder!=idx){
					_listMountItemDefs[i].ItemOrder=idx;
					isChanged=true;
					MountItemDefs.Update(_listMountItemDefs[i]);
				}
				idx++;
				//string s="#"+_listMountItemDefs[i].ItemOrder.ToString()+": X:"+_listMountItemDefs[i].Xpos.ToString()+", Y:"+_listMountItemDefs[i].Ypos.ToString()
				//	+": W:"+_listMountItemDefs[i].Width.ToString()+", H:"+_listMountItemDefs[i].Height.ToString();
				//listBoxItems.Items.Add(s);
			}
			if(isChanged){
				_listMountItemDefs=_listMountItemDefs.OrderBy(x=>x.ItemOrder).ToList();
			}
			controlDrawing.Invalidate();
		}

		///<summary>Returns the index of the item within the list, or -1.</summary>
		private int HitTest(Point point){
			for(int i=0;i<_listMountItemDefs.Count;i++){
				if(point.X<_rectangleMount.X+_listMountItemDefs[i].Xpos*_ratio){
					continue;
				}
				if(point.Y<_rectangleMount.Y+_listMountItemDefs[i].Ypos*_ratio){
					continue;
				}
				if(point.X>_rectangleMount.X+_listMountItemDefs[i].Xpos*_ratio+_listMountItemDefs[i].Width*_ratio){
					continue;
				}
				if(point.Y>_rectangleMount.Y+_listMountItemDefs[i].Ypos*_ratio+_listMountItemDefs[i].Height*_ratio){
					continue;
				}
				return i;
			}
			return -1;
		}

		private void ShowWarning(){
			labelWarning.Visible=false;
			if(_listMountItemDefs==null){
				return;
			}
			for(int i=0;i<_listMountItemDefs.Count;i++){
				if(_listMountItemDefs[i].Xpos>MountDefCur.Width-2
					|| _listMountItemDefs[i].Ypos>MountDefCur.Height-2)
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





















