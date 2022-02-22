using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using OpenDentBusiness;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace OpenDental {
	///<summary>This control is comprised of an outer SplitContainer which houses the sorting group box on top and the graphing panel on bottom. The bottom graphing panel is also a SplitContrainer in itself. This is to accomodate scrolling of the graph region only. The x-axis region is in the bottom panel of the SplitContainer graphing panel and will not be scrolled.</summary>
	public partial class GraphScheduleDay:UserControl {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		#region Properties available in designer.
		
		private Color graphBackColor=Color.LightBlue;
		[Category("Graph")]
		[Description("Color used for graph background")]
		public Color GraphBackColor {
			get {
				return graphBackColor;
			}
			set {
				graphBackColor=value;
				Invalidate(true);
			}
		}
		private Color xAxisBackColor=Color.LightBlue;
		[Category("Graph")]
		[Description("Color used for the X-Axis background")]
		public Color XAxisBackColor {
			get {
				return xAxisBackColor;
			}
			set {
				xAxisBackColor=value;
				Invalidate(true);
			}
		}
		private Color providerBarColor=Color.SkyBlue;
		[Category("Graph")]
		[Description("Bar color used for providers")]
		public Color ProviderBarColor {
			get {
				return providerBarColor;
			}
			set {
				providerBarColor=value;
				checkBoxProviders.BackColor=value;
				Invalidate(true);
			}
		}
		private Color providerTextColor=Color.Black;
		[Category("Graph")]
		[Description("Text color used for providers")]
		public Color ProviderTextColor {
			get {
				return providerTextColor;
			}
			set {
				providerTextColor=value;
				checkBoxProviders.ForeColor=value;
				Invalidate(true);
			}
		}
		private Color employeeBarColor=Color.SkyBlue;
		[Category("Graph")]
		[Description("Bar color used for employees")]
		public Color EmployeeBarColor {
			get {
				return employeeBarColor;
			}
			set {
				employeeBarColor=value;
				checkBoxEmployees.BackColor=value;
				Invalidate(true);
			}
		}
		private Color employeeTextColor=Color.Black;
		[Category("Graph")]
		[Description("Text color used for employees")]
		public Color EmployeeTextColor {
			get {
				return employeeTextColor;
			}
			set {
				employeeTextColor=value;
				checkBoxEmployees.ForeColor=value;
				Invalidate(true);
			}
		}
		private Color practiceTextColor=Color.White;
		[Category("Graph")]
		[Description("Text color used for practice-wide notes")]
		public Color PracticeTextColor {
			get {
				return practiceTextColor;
			}
			set {
				practiceTextColor=value;
				checkBoxNotes.ForeColor=value;
				Invalidate(true);
			}
		}
		private Color practiceBarColor=Color.Red;
		[Category("Graph")]
		[Description("Bar color used for practice-wide notes")]
		public Color PracticeBarColor {
			get {
				return practiceBarColor;
			}
			set {
				practiceBarColor=value;
				checkBoxNotes.BackColor=value;
				Invalidate(true);
			}
		}
		private int startHour=0;
		[Category("Graph")]
		[Description("0-based hour to start graphing from. 0 = 12am midnight. 23 = 11pm.")]
		public int StartHour {
			get {
				return startHour;
			}
			set {
				startHour=value;
				numStartHour.Value=value;
				Invalidate(true);
			}
		}
		private int endHour=0;
		[Category("Graph")]
		[Description("0-based hour to end graphing at. 0 = 12am midnight. 23 = 11pm.")]
		public int EndHour {
			get {
				return endHour;
			}
			set {
				endHour=value;
				numEndHour.Value=value;
				Invalidate(true);
			}
		}
		private int exteriorPaddingPixels=0;
		[Category("Graph")]
		[Description("Number of pixels to pad the outside of the graph with")]
		public int ExteriorPaddingPixels {
			get {
				return exteriorPaddingPixels;
			}
			set {
				exteriorPaddingPixels=value;
				Invalidate(true);
			}
		}
		private int lineWidthPixels=1;
		[Category("Graph")]
		[Description("Size of the lines (in pixels) drawn on the graph")]
		public int LineWidthPixels {
			get {
				return lineWidthPixels;
			}
			set {
				lineWidthPixels=value;
				Invalidate(true);
			}
		}
		private int tickHeightPixels=1;
		[Category("Graph")]
		[Description("Height of the tick drawn on the x axis")]
		public int TickHeightPixels {
			get {
				return tickHeightPixels;
			}
			set {
				tickHeightPixels=value;
				Invalidate(true);
			}
		}
		private int barHeightPixels=10;
		[Category("Graph")]
		[Description("Height of each bar on the graph. Font will also size according to this height.")]
		public int BarHeightPixels {
			get {
				return barHeightPixels;
			}
			set {
				barHeightPixels=value;
				Invalidate(true);
			}
		}
		private int barSpacingPixels=10;
		[Category("Graph")]
		[Description("Spacing between each bar on the graph")]
		public int BarSpacingPixels {
			get {
				return barSpacingPixels;
			}
			set {
				barSpacingPixels=value;
				Invalidate(true);
			}
		}

		#endregion Properties available in designer.

		#region Private data.
		
		///<summary>Derived from StartHour and EndHour</summary>
		public TimeSpan TotalTime {
			get {
				return TimeSpan.FromHours(Math.Max(EndHour-StartHour,1));
			}
		}
		///<summary>Derived from StartHour</summary>
		public TimeSpan StartTime {
			get {
				return TimeSpan.FromHours(StartHour);
			}
		}
		///<summary>Derived from EndHour</summary>
		public TimeSpan EndTime {
			get {
				return TimeSpan.FromHours(EndHour);
			}
		}
		///<summary>Current sort choice</summary>
		private ScheduleListComparer _compareScheduleLists=new ScheduleListComparer();		
		///<summary>Sorted and filtered list of schedules to display</summary>		
		private List<List<Schedule>> _schedulesList=new List<List<Schedule>>();
		///<summary>Raw list of schedules from the database. These will be sorted and filtered in various ways for display purposes.</summary>		
		private List<Schedule> _schedules=new List<Schedule>();

		#endregion Private data.

		#region Ctor
		
		public GraphScheduleDay() {
			InitializeComponent();
			//set styles so that this panel draws itself
			SetStyle(ControlStyles.ResizeRedraw,true);
			SetStyle(ControlStyles.AllPaintingInWmPaint,true);
			SetStyle(ControlStyles.UserPaint,true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
		}
		
		#endregion Ctor

		#region Public interface.

		public void SetSchedules(List<Schedule> schedules) {
			//set the raw list
			_schedules=new List<Schedule>(schedules);
			//clear the existing list of lists and repopulate
			_schedulesList.Clear();
			//single entities (employees or providers) can have more than 1 schedule linked to them
			//loop through raw schedules and build a list of lists
			//each out list will account for 1 row on the graph and represent 1 employee or provider
			for(int schedIndex=0;schedIndex<schedules.Count;schedIndex++) {
				Schedule schedule=schedules[schedIndex];
				if(!checkBoxNotes.Checked && (schedule.SchedType==ScheduleType.Blockout || schedule.SchedType==ScheduleType.Practice)) {//apply filter
					continue;
				}
				if(!checkBoxEmployees.Checked && schedule.SchedType==ScheduleType.Employee) {//apply filter
					continue;
				}
				if(!checkBoxProviders.Checked && schedule.SchedType==ScheduleType.Provider) {//apply filter
					continue;
				}
				switch(schedule.SchedType) {
					case ScheduleType.Blockout:
					case ScheduleType.Practice:
						if(!checkBoxNotes.Checked) { 
							continue;
						}
						//these types always get their own separate line
						_schedulesList.Add(new List<Schedule>(new Schedule[] { schedule }));
						break;
					case ScheduleType.Provider:
					case ScheduleType.Employee:
						//these types can have multiple schedules per line
						//loop through our exsiting lists and try to find a match
						bool isFound=false;
						for(int masterIndex=0;masterIndex<_schedulesList.Count;masterIndex++) {
							List<Schedule> existingSchedules=_schedulesList[masterIndex];
							Schedule existingSchedule=existingSchedules[0];
							if(existingSchedule.SchedType!=schedule.SchedType) {
								continue;
							}
							if(existingSchedule.SchedType==ScheduleType.Provider) {
								if(existingSchedule.ProvNum==schedule.ProvNum) {
									existingSchedules.Add(schedule);
									isFound=true;
								}
							}
							else if(existingSchedule.SchedType==ScheduleType.Employee) {
								if(existingSchedule.EmployeeNum==schedule.EmployeeNum) {
									existingSchedules.Add(schedule);
									isFound=true;
								}
							}
						}
						if(!isFound) { //we didn't find a match so add a new collection
							_schedulesList.Add(new List<Schedule>(new Schedule[] { schedule }));
						}
						break;
				}
			}
			//now that the list is of lists is built, sort each individual collection by start time
			ScheduleStartComparer sortStartTime=new ScheduleStartComparer();
			for(int i=0;i<_schedulesList.Count;i++) {
				_schedulesList[i].Sort(sortStartTime);
			}
			//now that the collections are individually sorted, sort the master list of lists according to user preference	
			_schedulesList.Sort(_compareScheduleLists);
			//force redraw
			this.Invalidate(true);
		}

		#endregion Public interface.

		#region Drawing

		private int GetPanel1GraphHeight() {
			return Math.Max(this.splitContainerBottom.Panel1.ClientRectangle.Height,_schedulesList.Count*(BarSpacingPixels+BarHeightPixels));
		}

		private float GetFontHeight() {
			float emSize=1;
			while(true) {
				using(Font newFont=new Font(Font.FontFamily,emSize,Font.Style)) {
					SizeF size=TextRenderer.MeasureText("0",newFont);
					if((size.Height+.2f)<BarHeightPixels) { //still too small so increment the font size and try again
						emSize+=.1F;
						continue;
					}
					return emSize;
				}				
			}		
		}

		///<summary>Draw the graph region</summary>
		private void splitContainer_Panel1_Paint(object sender,PaintEventArgs e) {
			Pen penLine=new Pen(Color.Black,LineWidthPixels);
			Font font=new Font(this.Font.FontFamily,GetFontHeight(),this.Font.Style);
			try {
				//offset the drawing region to current scroll location
				e.Graphics.TranslateTransform(this.splitContainerBottom.Panel1.AutoScrollPosition.X,this.splitContainerBottom.Panel1.AutoScrollPosition.Y);
				//size the drawable region
				RectangleF rcBounds=new RectangleF(penLine.Width/2,penLine.Width/2,this.splitContainerBottom.Panel1.ClientRectangle.Width-penLine.Width,GetPanel1GraphHeight()+penLine.Width);
				//fill the background
				using(Brush brushBack=new SolidBrush(GraphBackColor)) {
					e.Graphics.FillRectangle(brushBack,rcBounds.Left,rcBounds.Top,rcBounds.Width,rcBounds.Height);
				}
				//draw the outline
				e.Graphics.DrawRectangle(penLine,rcBounds.Left,rcBounds.Top,rcBounds.Width,rcBounds.Height);
				//shrink the remaining drawable region to fit inside the rest of the pen width
				rcBounds.Inflate(-penLine.Width/2,-penLine.Width/2);
				//squish the drawable region horizontally so we have some outer edge buffer
				rcBounds.Inflate(-ExteriorPaddingPixels,0);
				//each minute gets this many pixels
				float pixelsPerMinute=rcBounds.Width/(float)TotalTime.TotalMinutes;
				//get the pixel width of each hour
				float hourWidth=rcBounds.Width/(float)TotalTime.TotalHours;
				for(int hour=0;hour<=(int)TotalTime.TotalHours;hour++) {
					float xPos=rcBounds.Left+(hour*hourWidth);
					//draw 15 minute dashed lines
					for(int quarter=0;quarter<=3;quarter++) {
						using(Pen penDash=new Pen(quarter==0?Color.Black:Color.LightGray,LineWidthPixels)) {							
							e.Graphics.DrawLine(penDash,xPos,rcBounds.Bottom,xPos,rcBounds.Top);
							xPos+=(hourWidth/4);
						}
					}
				}
				if(_schedulesList==null || _schedulesList.Count<=0) {
					return;
				}
				//draw the bars for each schedule
				//the combined height of the following for loop must match EXACTLY the value returned by GetPanel1GraphHeight().
				//each schedule get 1 BarSpacingPixels plus 1 BarHeightPixels
				//any change to that rule must have a corresponding change to the GetPanel1GraphHeight() function
				float yPosStart=rcBounds.Top;
				for(int i=0;i<_schedulesList.Count;i++) {
					yPosStart+=BarSpacingPixels;
					List<Schedule> schedules=_schedulesList[i];
					for(int j=0;j<schedules.Count;j++) {
						Schedule schedule=schedules[j];
						float xPosStart=Math.Max(rcBounds.Left,(rcBounds.Left+(float)schedule.StartTime.Subtract(this.StartTime).TotalMinutes*pixelsPerMinute));
						float xWidth=Math.Min(rcBounds.Right,(float)(schedule.StopTime.Subtract(schedule.StartTime).TotalMinutes*pixelsPerMinute));
						string desc="";
						Color barColor=EmployeeBarColor;
						Color textColor=EmployeeTextColor;
						if(schedule.SchedType==ScheduleType.Practice) { //this is just used to create a date-level note
							xPosStart=rcBounds.Left;
							xWidth=rcBounds.Width;
							barColor=PracticeBarColor;
							textColor=PracticeTextColor;
						}
						else if(schedule.ProvNum!=0) { //Prov
							desc=Providers.GetAbbr(schedule.ProvNum);
							barColor=ProviderBarColor;
							textColor=ProviderTextColor;
						}
						else if(schedule.EmployeeNum!=0) { //Employee
							desc=Employees.GetEmp(schedule.EmployeeNum).FName;
							barColor=EmployeeBarColor;
							textColor=EmployeeTextColor;
						}
						if(schedule.Note!="") {
							desc+=" -- "+schedule.Note;
						}
						using(Brush brushBar=new SolidBrush(barColor)) {
							e.Graphics.FillRectangle(brushBar,xPosStart,yPosStart,xWidth,BarHeightPixels);
						}
						using(Brush brushNoteText=new SolidBrush(textColor)) {
							e.Graphics.DrawString(desc,font,brushNoteText,xPosStart+2,yPosStart);
						}
					}
					yPosStart+=BarHeightPixels;
				}
				//draw a faint line indicating what time of day it is right now
				if(DateTime.Now>=DateTime.Today.AddHours(StartHour) && DateTime.Now<=DateTime.Today.AddHours(EndHour)) {
					using(Pen penNow=new Pen(Color.FromArgb(128,Color.Red),LineWidthPixels*2)) {
						int minutes = (int)DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromHours(StartHour)).TotalMinutes;
						float xPos=rcBounds.Left+(minutes*pixelsPerMinute);
						e.Graphics.DrawLine(penNow,xPos,rcBounds.Bottom,xPos,rcBounds.Top);
					}
				}
				if(this.splitContainerBottom.Panel1.AutoScrollMinSize.Height!=(int)yPosStart) {
					//create vertical scrollbar only
					this.splitContainerBottom.Panel1.AutoScrollMinSize=new Size(0,(int)yPosStart);
					this.splitContainerBottom.Invalidate(true);
				}
			}
			catch { }
			finally {
				if(penLine!=null) {
					penLine.Dispose();
				}
				if(font!=null) {
					font.Dispose();
				}
			}
		}

		///<summary>Draw the x-axis region</summary>
		private void splitContainer_Panel2_Paint(object sender,PaintEventArgs e) {
			Pen penLine=new Pen(Color.Black,LineWidthPixels);
			Brush brushForeColor=new SolidBrush(this.ForeColor);
			Font font=new Font(this.Font,this.Font.Style);
			try {
				//draw the back color as the background
				using(Brush brushBack=new SolidBrush(this.BackColor)) {
					e.Graphics.FillRectangle(brushBack,this.splitContainerBottom.Panel2.ClientRectangle);
				}
				//shrink the drawable region to accomodate half the pen width
				RectangleF rcBounds=new RectangleF(penLine.Width/2,penLine.Width/2,this.splitContainerBottom.Panel2.ClientRectangle.Width-penLine.Width,this.splitContainerBottom.Panel2.ClientRectangle.Height-penLine.Width);
				if(splitContainerBottom.Panel1.VerticalScroll.Visible) {
					rcBounds.Inflate(-((float)SystemInformation.VerticalScrollBarWidth/2),0);
					rcBounds.Offset(-((float)SystemInformation.VerticalScrollBarWidth/2),0);
					//fill whitespace left underneath the vertical scrollbar from Panel1
					e.Graphics.FillRectangle(SystemBrushes.Control,rcBounds.Right,this.splitContainerBottom.Panel2.ClientRectangle.Top,SystemInformation.VerticalScrollBarWidth,this.splitContainerBottom.Panel2.ClientRectangle.Height);				
				}
				//fill the background
				using(Brush brushBack=new SolidBrush(XAxisBackColor)) {
					e.Graphics.FillRectangle(brushBack,rcBounds.Left,rcBounds.Top,rcBounds.Width,rcBounds.Height);
				}
				//draw the outline
				e.Graphics.DrawRectangle(penLine,rcBounds.Left,rcBounds.Top,rcBounds.Width,rcBounds.Height);
				//shrink the drawable region to fit inside the rest of the pen width
				rcBounds.Inflate(-penLine.Width/2,-penLine.Width/2);
				//squish the drawable region horizontally so we have some outer edge buffer
				rcBounds.Inflate(-ExteriorPaddingPixels,0);
				//get the pixel width of each hour
				float hourWidth=rcBounds.Width/(float)TotalTime.TotalHours;
				for(int hour=0;hour<=(int)TotalTime.TotalHours;hour++) {
					DateTime dt=DateTime.Today.AddHours(StartHour+hour);
					string stringHour=dt.ToString("htt").ToLower();
					SizeF szHour=e.Graphics.MeasureString(stringHour,font);
					float xPos=rcBounds.Left+(hour*hourWidth);
					float yPos=rcBounds.Top;
					//draw the tick
					e.Graphics.DrawLine(penLine,xPos,yPos,xPos,yPos+TickHeightPixels);
					yPos+=TickHeightPixels;					
					//draw the hour
					e.Graphics.DrawString(stringHour,font,brushForeColor,xPos-(szHour.Width/2),yPos);					
				}
			}
			catch { }
			finally {
				if(penLine!=null) {
					penLine.Dispose();
				}
				if(brushForeColor!=null) {
					brushForeColor.Dispose();
				}
				if(font!=null) {
					font.Dispose();
				}
			}
		}

		#endregion Drawing

		#region Sort, Filter, Scale handlers.

		private void radioName_CheckedChanged(object sender,EventArgs e) {
			if(!radioName.Checked) {
				return;
			}
			_compareScheduleLists.Sort=ScheduleListComparer.SortBy.Name;
			_schedulesList.Sort(_compareScheduleLists);
			this.Invalidate(true);
		}

		private void radioStartTime_CheckedChanged(object sender,EventArgs e) {
			if(!radioStartTime.Checked) {
				return;
			}
			_compareScheduleLists.Sort=ScheduleListComparer.SortBy.StartTime;
			_schedulesList.Sort(_compareScheduleLists);
			this.Invalidate(true);
		}

		private void radioStopTime_CheckedChanged(object sender,EventArgs e) {
			if(!radioStopTime.Checked) {
				return;
			}
			_compareScheduleLists.Sort=ScheduleListComparer.SortBy.StopTime;
			_schedulesList.Sort(_compareScheduleLists);
			this.Invalidate(true);
		}

		///<summary>Filter our dispaly and remove Practice notes</summary>
		private void checkBoxNotes_CheckedChanged(object sender,EventArgs e) {
			SetSchedules(_schedules);
		}

		///<summary>Filter our dispaly and remove Providers</summary>
		private void checkBoxProviders_CheckedChanged(object sender,EventArgs e) {
			SetSchedules(_schedules);
		}

		///<summary>Filter our dispaly and remove Employees</summary>
		private void checkBoxEmployees_CheckedChanged(object sender,EventArgs e) {
			SetSchedules(_schedules);
		}

		private void numStartHour_ValueChanged(object sender,EventArgs e) {
			StartHour=(int)numStartHour.Value;
		}

		private void numEndHour_ValueChanged(object sender,EventArgs e) {
			EndHour=(int)numEndHour.Value;
		}

		#endregion Sort, Filter, Scale handlers.

		///<summary>Redraw the graph so the current time bar gets moved. Once per minute.</summary>
		private void timerRefresh_Tick(object sender,EventArgs e) {
			Invalidate(true);
		}

		private void GraphScheduleDay_SizeChanged(object sender,EventArgs e) {
			splitContainerMaster.SplitterDistance=LayoutManager.Scale(52);
			splitContainerBottom.SplitterDistance=splitContainerBottom.Height-LayoutManager.Scale(26);
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerMaster);
			BarHeightPixels=LayoutManager.Scale(17);
		}
	}

	///<summary>Sort a list of Schedules given a sort criteria</summary>
	public class ScheduleListComparer:IComparer<List<Schedule>> {
		public SortBy Sort=SortBy.StopTime;

		public int Compare(List<Schedule> collectionX,List<Schedule> collectionY) {
			//default sort by first schedule in the collection
			Schedule x=collectionX[0];
			Schedule y=collectionY[0];
			if(Sort==SortBy.StopTime) { //stop time means sort by last schedule in collection
				x=collectionX[collectionX.Count-1];
				y=collectionY[collectionY.Count-1];
			}
			int ret=0;
			if(x.SchedType!=y.SchedType) { //send this orders by 1) practice notes, 2) providers 3) employees
				return x.SchedType.CompareTo(y.SchedType);
			}
			else if((x.SchedType==ScheduleType.Practice && y.SchedType==ScheduleType.Practice) ||
				(x.SchedType==ScheduleType.Blockout && y.SchedType==ScheduleType.Blockout)) { //this is a note only so order alapha
				return x.Note.CompareTo(y.Note);
			}
			else if(Sort==SortBy.StartTime) {
				ret=x.StartTime.CompareTo(y.StartTime);
				if(ret==0) { //if start times are same then default to alpha
					return CompareNames(x,y);
				}
				return ret;
			}
			else if(Sort==SortBy.StopTime) {
				ret=x.StopTime.CompareTo(y.StopTime);
				if(ret==0) { //if stop times are same then default to alpha
					return CompareNames(x,y);
				}
				return ret;
			}
			else { //we got this far so sort by name
				ret=CompareNames(x,y);
				if(ret==0) { //if names are same then default to start time
					return x.StartTime.CompareTo(y.StartTime);
				}
				return ret;
			}
		}

		///<summary>Name sort order differs according to ScheduleType. This sorts accordingly.<returns></returns>
		private int CompareNames(Schedule x,Schedule y) {
			if(x.ProvNum!=y.ProvNum) { //we are dealing with a provider
				return Providers.GetProv(x.ProvNum).ItemOrder.CompareTo(Providers.GetProv(y.ProvNum).ItemOrder);
			}
			if(x.EmployeeNum!=y.EmployeeNum) { //we are dealing with an employee
				return Employees.GetEmp(x.EmployeeNum).FName.CompareTo(Employees.GetEmp(y.EmployeeNum).FName);
			}
			return 0;
		}

		public enum SortBy {
			Name,
			StartTime,
			StopTime
		}
	}

	///<summary>Sort a list of Schedules given a sort criteria</summary>
	public class ScheduleStartComparer:IComparer<Schedule> {
		public int Compare(Schedule x,Schedule y) {
			return x.StartTime.CompareTo(y.StartTime);
		}
	}
}
