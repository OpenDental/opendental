using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class PinBoard:Control {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private Bitmap _bitmapTempPinAppt;
		///<summary>This user control is how we drag appts over to the main appt area.  In the beginning, it gets added to parent, where it stays and gets reused repeatedly.  We control it from here to hide the complexity.</summary>
		private ControlDoubleBuffered contrTempPinAppt;
		private bool _isMouseDown;
		private List<PinBoardItem> _listPinBoardItems;
		///<summary>Initial position of the appointment being dragged, in coordinates of ContrAppt.</summary>
		private Point	_pointApptOrigin;
		///<summary>Position of mouse down, in coordinates of this pinboard control.</summary>
		private Point _pointMouseOrigin;
		#endregion Fields - Private

		#region Constructor and Dispose
		public PinBoard() {
			InitializeComponent();

		}

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing){//managed resources
				components?.Dispose();
				//get rid of temp appts on pinboard. =jordan I don't see any docs on why this is here. Doesn't seem quite right, but it works.
				for(int i=0;i<ListPinBoardItems.Count;i++) {
					if(PIn.Int(ListPinBoardItems[i].DataRowAppt["AptStatus"].ToString())!=(int)ApptStatus.UnschedList){
						continue;
					}
					if(PIn.DateT(ListPinBoardItems[i].DataRowAppt["AptDateTime"].ToString()).Year>1880){
						continue;
					}
					Appointment appt=null;
					try{
						appt=Appointments.GetOneApt(ListPinBoardItems[i].AptNum);
					}
					catch{
						break;//db connection no longer present.
					}
					if(appt==null){
						continue;
					}
					if(appt.AptDateTime.Year>1880) {//date was updated since put on the pinboard
						continue;
					}
					Appointments.Delete(appt.AptNum,true);
					if(Security.CurUser==null){// E.g. clicking Log Off invalidates the user.
						continue;
					}
					//Make a security log if we still have a valid user logged in. 
					string logText=Lan.g(this,"Deleted from pinboard while closing Open Dental")+": ";
					if(appt.AptDateTime.Year>1880) {
						logText+=appt.AptDateTime.ToString()+", ";
					}
					logText+=appt.ProcDescript;
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,appt.PatNum,logText);
				}
			}
			base.Dispose(disposing);
		}
		#endregion Constructor and Dispose

		#region Events - Raise
		private void OnApptMovedFromPinboard(DataRow dataRow,Bitmap bitmap,Point location) {
			if(ApptMovedFromPinboard!=null){
				ApptMovedFromPinboard(this,new ApptFromPinboardEventArgs(){DataRowAppt=dataRow,BitmapAppt=bitmap,Location=location });
			}
		}
		[Category("Appointment"),Description("Occurs when an appointment is dragged from pinboard.  Location is in coordinates of ContrAppt.")]
		public event EventHandler<ApptFromPinboardEventArgs> ApptMovedFromPinboard;

		private void OnModuleNeedsRefresh() {
			if(ModuleNeedsRefresh!=null){
				ModuleNeedsRefresh(this,new EventArgs());
			}
		}
		[Category("Appointment"),Description("Occurs when Appt module needs a refresh because pinboard has changed the appointment provider.  The Appt module should also send the pinboard selected appt to the pinboard again to refresh.")]
		public event EventHandler ModuleNeedsRefresh;

		private void OnPreparingToDragFromPinboard(DataRow dataRow) {
			if(PreparingToDragFromPinboard!=null){
				PreparingToDragFromPinboard(this,new ApptDataRowEventArgs(){DataRowAppt=dataRow });
			}
		}
		[Category("Appointment"),Description("Occurs when mouse down, and preparing to drag an appt from pinboard.")]
		public event EventHandler<ApptDataRowEventArgs> PreparingToDragFromPinboard;

		protected void OnSelectedIndexChanged() {
			if(SelectedIndexChanged!=null) {
				SelectedIndexChanged(this,new EventArgs());
			}
		}
		[Category("Appointment"),Description("Event raised when user _clicks_ to select a different appointment.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events - Raise

		#region Events - OnPaint
		protected override void OnPaint(PaintEventArgs pe) {
			Graphics g=pe.Graphics;
			try{ 
				g.Clear(Color.White);
			}
			catch{
				return;//This can happen on remote desktop when connection is lost
			}
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			//for(int i=0;i<_dataTableAppointments.Rows.Count;i++) {
			if(ListPinBoardItems==null || ListPinBoardItems.Count==0) {
				StringFormat format=new StringFormat();
				format.Alignment=StringAlignment.Center;
				format.LineAlignment=StringAlignment.Center;
				g.DrawString("Drag Appointments to this Pinboard",Font,Brushes.Gray,new RectangleF(0,0,Width,Height-20),format);
				return;
			}
			for(int i=0;i<ListPinBoardItems.Count;i++) {
				int locY=i*13;
				g.DrawImage(ListPinBoardItems[i].BitmapAppt,1,locY);
				if(_selectedIndex==i){
					DataRow dataRow=ListPinBoardItems[i].DataRowAppt;
					Pen penProvOutline;
					if(dataRow["ProvNum"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="0") {//dentist
						penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvNum"].ToString())),3f);
					}
					else if(dataRow["ProvHyg"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="1") {//hygienist
						penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvHyg"].ToString())),3f);
					}
					else {//unknown
						penProvOutline=new Pen(Color.Black,3f);//Do not use Pens.Black because we will be disposing this pen later on.
					}
					GraphicsPath graphicsPathOutline=ControlApptPanel.GetRoundedPath(
						new RectangleF(1.5f,locY+.5f,ListPinBoardItems[i].BitmapAppt.Width-2,ListPinBoardItems[i].BitmapAppt.Height-1.5f),2);
					g.DrawPath(penProvOutline,graphicsPathOutline);
					//g.DrawRectangle(penProvOutline,1.5f,locY+.5f,ListPinBoardItems[i].BitmapAppt.Width-2,ListPinBoardItems[i].BitmapAppt.Height-1);
					penProvOutline.Dispose();
				}
			}
		}

		///<summary></summary>
		private void TempPinAppt_Paint(object sender,PaintEventArgs e) {
			if(_bitmapTempPinAppt==null){
				return;
			}
			e.Graphics.DrawImage(_bitmapTempPinAppt,0,0);
			using(Pen pen=new Pen(Color.Black,1f)){
				e.Graphics.DrawLine(pen,0,contrTempPinAppt.Height-1,contrTempPinAppt.Width,contrTempPinAppt.Height-1);
			}
		}
		#endregion Events - OnPaint

		#region Events - Mouse
		
		protected override void OnMouseDown(MouseEventArgs e) {
			if(_isMouseDown) {//User right clicked while dragging appt around.
				return;
			}
			//set selected index before firing the mouse down event.
			//figure out which appt mouse is on.  Start at end and work backwards
			int index=-1;
			for(int i=ListPinBoardItems.Count-1;i>=0;i--){
				int top=13*i;
				if(e.Y<top || e.Y>top+ListPinBoardItems[i].BitmapAppt.Height){
					continue;
				}
				index=i;
				break;
			}
			bool changed=index!=SelectedIndex;
			if(changed){
				SelectedIndex=index;
			}
			base.OnMouseDown(e);
			if(changed){
				Invalidate();
				OnSelectedIndexChanged();//fires to parent, which sets selected appt in main area to -1.
			}
			//--Prepare to drag or show context menu-----------------------------------------------------
			if(SelectedIndex==-1) {
				return;
			}
			if(e.Button==MouseButtons.Right) {
				ContextMenu contextMenu=new ContextMenu();
				MenuItem menuItemProv=new MenuItem(Lan.g(this,"Change Provider"));
				menuItemProv.Click+=new EventHandler(menuItemProv_Click);
				contextMenu.MenuItems.Add(menuItemProv);
				contextMenu.Show(this,e.Location);
				return;
			}
			_isMouseDown=true;
			OnPreparingToDragFromPinboard(ListPinBoardItems[SelectedIndex].DataRowAppt);//this event goes to ContrAppt, 
			//which then sends new bitmap here via SetBitmapTempPinAppt before the code below continues.
			//So, we have now set the size of contrTempPinAppt.
			contrTempPinAppt.Visible=false;//Visible flag gets flipped when the mouse moves. We are just preparing it to be shown here.
			_pointMouseOrigin=e.Location;//local pinboard coords.
			LayoutManager.MoveLocation(contrTempPinAppt,
				new Point(Left+Parent.Left+Parent.Parent.Left,Top+Parent.Top+Parent.Parent.Top+SelectedIndex*13));
			if(contrTempPinAppt.Right<Left+Parent.Left+Parent.Parent.Left+_pointMouseOrigin.X){//appointment is very narrow
				LayoutManager.MoveLocation(contrTempPinAppt,new Point(
					Left+Parent.Left+Parent.Parent.Left+_pointMouseOrigin.X-contrTempPinAppt.Width,contrTempPinAppt.Top));//so move it to the right
			}
			_pointApptOrigin=contrTempPinAppt.Location;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!_isMouseDown) {
				return;
			}
			if(_bitmapTempPinAppt==null){//For example, if no ops visible
				_isMouseDown=false;
				return;
			}
			if((Math.Abs(e.X-_pointMouseOrigin.X)<3)//we don't want it to be visible until user has actually started moving significantly
				&&(Math.Abs(e.Y-_pointMouseOrigin.Y)<3)) {
				return;
			}
			//since this usercontrol belongs to ContrAppt, coordinates are in ContrAppt frame.
			LayoutManager.MoveLocation(contrTempPinAppt,new Point(
				_pointApptOrigin.X+e.X-_pointMouseOrigin.X,
				_pointApptOrigin.Y+e.Y-_pointMouseOrigin.Y));
			contrTempPinAppt.Visible=true;
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(!_isMouseDown) {
				return;
			}
			_isMouseDown=false;
			if((Math.Abs(e.X-_pointMouseOrigin.X)<7) && (Math.Abs(e.Y-_pointMouseOrigin.Y)<7)) { //Mouse has not moved enough to be considered an appt move.
				contrTempPinAppt.Visible=false;
				return;
			}
			if(contrTempPinAppt.Location.X>Parent.Parent.Left) { 
				contrTempPinAppt.Visible=false;
				return;
			}
			//Dragged to main appt area and released.  Don't hide it yet.
			//must use the new resized bitmap which is the proper width
			OnApptMovedFromPinboard(ListPinBoardItems[SelectedIndex].DataRowAppt,_bitmapTempPinAppt,contrTempPinAppt.Location);
		}
		#endregion Events - Mouse

		#region Events - Other
		void menuItemProv_Click(object sender,EventArgs e) {
			Appointment apt=Appointments.GetOneApt(ListPinBoardItems[SelectedIndex].AptNum);
			if(apt==null) {
				ClearAt(SelectedIndex);
				MessageBox.Show("Appointment not found.");
				return;
			}
			Appointment oldApt=apt.Copy();
			long provNum=apt.ProvNum;
			if(apt.IsHygiene) {
				provNum=apt.ProvHyg;
			}
			using FormProviderPick formPick=new FormProviderPick();
			formPick.SelectedProvNum=provNum;
			formPick.ShowDialog();
			if(formPick.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formPick.SelectedProvNum==provNum) {
				return;//provider not changed.
			}
			if(apt.IsHygiene) {
				apt.ProvHyg=formPick.SelectedProvNum;
			}
			else {
				apt.ProvNum=formPick.SelectedProvNum;
			}
			#region Provider Term Date Check
			//Prevents appointments with providers that are past their term end date from being scheduled
			//Allows for unscheduled appointments to skip the check as it will check when they go to schedule.
			if(apt.AptStatus!=ApptStatus.UnschedList) {
				string message=Providers.CheckApptProvidersTermDates(apt);
				if(message!="") {
					MessageBox.Show(this,message);//translated in Providers S class method
					return;
				}
			}
			#endregion Provider Term Date Check
			List<Procedure> procsForSingleApt=Procedures.GetProcsForSingle(apt.AptNum,false);
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change length for new provider?")){
				List<long> codeNums=new List<long>();
				for(int p=0;p<procsForSingleApt.Count;p++) {
					codeNums.Add(procsForSingleApt[p].CodeNum);
				}
				string calcPattern=Appointments.CalculatePattern(apt.ProvNum,apt.ProvHyg,codeNums,true);
				if(apt.Pattern != calcPattern) {
					if(!apt.TimeLocked || MsgBox.Show(this,MsgBoxButtons.YesNo,"Appointment length is locked.  Change length for new provider anyway?")) {
						apt.Pattern=calcPattern;
					}
				}
			}
			Appointments.Update(apt,oldApt);
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(apt.PatNum);
			bool isUpdatingFees=false;
			List<Procedure> listProcsNew=procsForSingleApt.Select(x => Procedures.ChangeProcInAppointment(apt,x.Copy())).ToList();
			if(procsForSingleApt.Exists(x => x.ProvNum!=listProcsNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
				string promptText="";
				isUpdatingFees=Procedures.ShouldFeesChange(listProcsNew,procsForSingleApt,ref promptText,procFeeHelper);
				if(isUpdatingFees) {//Made it pass the pref check.
					if(promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
							isUpdatingFees=false;
					}
				}
			}
			Procedures.SetProvidersInAppointment(apt,procsForSingleApt,isUpdatingFees,procFeeHelper);
			OnModuleNeedsRefresh();
			//this will also trigger ContrAppt to send a new appt to the pinboard.
		}
		#endregion Events - Other

		#region Properties
		[Browsable(false)]
		[DefaultValue(null)]		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<PinBoardItem> ListPinBoardItems{ 
			get{
				if(_listPinBoardItems==null){
					_listPinBoardItems=new List<PinBoardItem>();
				}
				return _listPinBoardItems; 
			}
			//set{
				//not used because need to dispose of bitmaps
			//}
		}

		///<Summary>Gets or sets the selected index, which will have dark outline.  Parent should unselect other normal appts, so that only a pinboard appt OR a normal appt is selected, not both.</Summary>
		[Browsable(false)]
		[DefaultValue(-1)]
		public int SelectedIndex{
			get{
				return _selectedIndex;
			}
			set{
				if(_selectedIndex==value){
					return;
				}
				if(ListPinBoardItems==null){
					return;
				}
				if(value > ListPinBoardItems.Count-1){
					return;
				}
				_selectedIndex=value;
				Invalidate();
			}
		}
		private int _selectedIndex=-1;
		#endregion Properties

		#region Methods - Public
		///<Summary>If adding a family, this will be called repeatedly.</Summary>
		public void AddAppointment(Bitmap bitmap,long aptNum,DataRow dataRow) {
			for(int i=0;i<ListPinBoardItems.Count;i++){
				if(ListPinBoardItems[i].AptNum==aptNum){
					//The appointment being passed in is already on the pinboard.
					//Remove the old one so that we can replace it with the new one.
					//This happens when the appointment changes provider, for example.
					ListPinBoardItems[i].BitmapAppt.Dispose();
					ListPinBoardItems.RemoveAt(i);
					break;
				}
			}
			PinBoardItem pinBoardItem=new PinBoardItem();
			pinBoardItem.AptNum=aptNum;
			pinBoardItem.BitmapAppt=new Bitmap(bitmap);//so that we can dispose of the one that was passed in
			pinBoardItem.DataRowAppt=dataRow;
			List<long> listAptNums=new List<long>(){aptNum};
			pinBoardItem.TableApptFields=Appointments.GetApptFieldsByApptNums(listAptNums);
			List<long> listPatNums=new List<long>(){PIn.Long(dataRow["PatNum"].ToString())};
			pinBoardItem.TablePatFields=Appointments.GetPatFields(listPatNums);
			ListPinBoardItems.Add(pinBoardItem);
			_selectedIndex=ListPinBoardItems.Count-1;
			Invalidate();
		}

		/// <summary></summary>
		public void ClearAt(int idx){
			if(ListPinBoardItems.Count==0){
				return;
			}
			if(idx > ListPinBoardItems.Count-1){
				return;
			}
			ListPinBoardItems[idx].BitmapAppt.Dispose();
			ListPinBoardItems.RemoveAt(idx);
			SelectedIndex=-1;
			Invalidate();
		}

		///<summary>Used by parent form when a dialog needs to be displayed, but mouse might be down.  This forces a mouse up, and cleans up any mess so that dlg can show.</summary>
		public void MouseUpForced() {
			if(contrTempPinAppt!=null) {
				contrTempPinAppt.Visible=false;
			}
			_isMouseDown=false;
		}

		///<summary></summary>
		public void HideDraggableTempApptSingle() {
			contrTempPinAppt.Visible=false;
		}

		///<summary>During mouse down, this is used to send a different size bitmap for dragging around.</summary>
		public void SetBitmapTempPinAppt(Bitmap bitmap){
			if(contrTempPinAppt==null){
				contrTempPinAppt=new ControlDoubleBuffered();
				contrTempPinAppt.Name="contrTempPinAppt";
				contrTempPinAppt.Visible=false;
				contrTempPinAppt.Size=new Size(100,100);
				contrTempPinAppt.Paint+=TempPinAppt_Paint;
				LayoutManager.Add(contrTempPinAppt,Parent.Parent.Parent);
				contrTempPinAppt.BringToFront();
			}
			if(bitmap==null){//ContrAppt is telling pinboard that we can't drag appt off.
				_bitmapTempPinAppt=null;
				return;
			}
			_bitmapTempPinAppt=new Bitmap(bitmap);
			LayoutManager.MoveSize(contrTempPinAppt,new Size(_bitmapTempPinAppt.Width,_bitmapTempPinAppt.Height));
			//Next line rounds the corners of the control so that they are invisible
			contrTempPinAppt.Region=new Region(ControlApptPanel.GetRoundedPath(new RectangleF(0,0,contrTempPinAppt.Width,contrTempPinAppt.Height),2));
		}
		#endregion Methods - Public

		#region Methods - Private
		
		#endregion Methods - Private

		
	}

	///<summary>The individual items/appointments on the pinboard.</summary>
	[Serializable]
	public class PinBoardItem{
		///<summary>Sized to be 2 pixels narrower than the pinboard width.</summary>
		public Bitmap BitmapAppt;
		public long AptNum;
		public DataRow DataRowAppt;
		///<summary>The ApptFields for this one appointment.</summary>
		public DataTable TableApptFields;
		///<summary>The PatFields for this one patient.</summary>
		public DataTable TablePatFields;
	}

}
